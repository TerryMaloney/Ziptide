using System;
using System.Collections.Generic;
using Ziptide.Multiplayer.Bots;

namespace Ziptide.Multiplayer.Conquest
{
    public enum BattleOutcome { MajorVictory, CostlyVictory, Stalemate, FailedAttack, Counterstrike }

    /// <summary>An attack order — also the unit of multiplayer sync (ITidefrontSync sends these).</summary>
    [Serializable]
    public class AttackOrder
    {
        public int attackerId;
        public string fromPlanetId = "";
        public string targetPlanetId = "";
        public List<string> vesselIds = new List<string>();   // committed from the attacker's fleet
        public int missionModifier = 0;                        // signed VR-mission tilt: >0 attacker won (+attack), <0 defender won (+defense)
    }

    [Serializable]
    public class BattleReport
    {
        public BattleOutcome outcome;
        public float odds;
        public float roll;
        public int attackScore;
        public int defenseScore;
        public bool captured;
        public List<string> attackerVesselsLost = new List<string>();
        public int defenderUnitsLost;
    }

    /// <summary>
    /// Deterministic battle resolution (docs/10_TIDEFRONT.md math, production rules in
    /// docs/design/TIDEFRONT_AAA.md). Same state + order + seed → the same report, forever — that's the
    /// test story AND the async-multiplayer anti-cheat (both clients replay the identical resolution).
    /// Vessel/defense specials are honored by id from ConquestCatalog.
    /// </summary>
    public static class ConquestResolver
    {
        public static BattleReport Resolve(ConquestState state, AttackOrder order, int seed)
        {
            var report = new BattleReport();
            var attacker = state.GetPlayer(order.attackerId);
            var target = state.GetPlanet(order.targetPlanetId);
            if (attacker == null || target == null) { report.outcome = BattleOutcome.FailedAttack; return report; }
            if (!state.CanAttack(order.attackerId, order.fromPlanetId, order.targetPlanetId))
            { report.outcome = BattleOutcome.FailedAttack; return report; }

            // Validate the committed wave against the fleet the attacker actually owns (duplicate-safe:
            // you can commit two pulse_frigates only if you own two).
            var wave = new List<string>();
            foreach (var id in order.vesselIds)
                if (CountIn(wave, id) < CountIn(attacker.fleetVesselIds, id))
                    wave.Add(id);
            // A gate jammer blocks the attack entirely unless a gate piercer is in the wave.
            bool hasPiercer = wave.Contains("gate_piercer");
            if (target.builtDefenseIds.Contains("gate_jammer") && !hasPiercer)
            { report.outcome = BattleOutcome.FailedAttack; return report; }

            // ── Scores ───────────────────────────────────────────────────────
            bool ignoreShield = false, halvesDroneNet = false;
            int attackScore = order.missionModifier > 0 ? order.missionModifier : 0;
            foreach (var id in wave)
            {
                var v = ConquestCatalog.Vessel(id);
                if (v == null) continue;
                attackScore += v.Value.AttackPower;
                if (v.Value.Special == "ignores_shield") ignoreShield = true;
                if (v.Value.Special == "halves_drone_net") halvesDroneNet = true;
            }

            int defenseScore = target.defenseLevel + target.stationedDefenseUnits;
            if (!ignoreShield) defenseScore += target.orbitalShieldLevel;
            if (halvesDroneNet && target.builtDefenseIds.Contains("drone_net")) defenseScore -= 1;
            if (target.conflictState == ConflictState.UnderAttack) defenseScore += ConquestRules.DogpileBonus;
            // missionModifier is a signed tilt: >0 = the ATTACKER won their VR mission (added to attack
            // above); <0 = the DEFENDER won theirs — those points reinforce the defense.
            if (order.missionModifier < 0) defenseScore -= order.missionModifier;
            if (defenseScore < 0) defenseScore = 0;

            report.attackScore = attackScore;
            report.defenseScore = defenseScore;
            report.odds = ConquestRules.ComputeOdds(attackScore, defenseScore);

            // ── The roll (deterministic) ─────────────────────────────────────
            var rng = new BotRng(seed);
            report.roll = rng.NextFloat();
            report.outcome = Classify(report.roll, report.odds);

            Apply(state, order, wave, target, attacker, report);
            attacker.attacksThisTurn++;
            return report;
        }

        private static BattleOutcome Classify(float roll, float odds)
        {
            if (roll < odds * ConquestRules.MajorVictoryFraction) return BattleOutcome.MajorVictory;
            if (roll < odds) return BattleOutcome.CostlyVictory;
            if (roll < odds + ConquestRules.StalemateWindow) return BattleOutcome.Stalemate;
            if (roll < odds + ConquestRules.FailedWindow) return BattleOutcome.FailedAttack;
            return BattleOutcome.Counterstrike;
        }

        private static void Apply(ConquestState state, AttackOrder order, List<string> wave,
                                  PlanetNode target, ConquestPlayer attacker, BattleReport report)
        {
            bool minefield = target.builtDefenseIds.Contains("gravity_minefield");

            switch (report.outcome)
            {
                case BattleOutcome.MajorVictory:
                    Capture(target, order.attackerId);
                    report.captured = true;
                    LoseVessels(attacker, wave, minefield ? 1 : 0, report);         // clean win — minefield still bites
                    break;

                case BattleOutcome.CostlyVictory:
                    Capture(target, order.attackerId);
                    report.captured = true;
                    LoseVessels(attacker, wave,
                        (int)Math.Ceiling(wave.Count * ConquestRules.CostlyVictoryLossFraction) + (minefield ? 1 : 0), report);
                    break;

                case BattleOutcome.Stalemate:
                    LoseVessels(attacker, wave,
                        (int)Math.Ceiling(wave.Count * ConquestRules.StalemateLossFraction), report);
                    report.defenderUnitsLost = (target.stationedDefenseUnits + 1) / 2;
                    target.stationedDefenseUnits -= report.defenderUnitsLost;
                    target.conflictState = ConflictState.UnderAttack;
                    break;

                case BattleOutcome.FailedAttack:
                    LoseVessels(attacker, wave, wave.Count, report);
                    target.conflictState = ConflictState.UnderAttack;
                    break;

                case BattleOutcome.Counterstrike:
                    LoseVessels(attacker, wave, wave.Count, report);
                    var from = state.GetPlanet(order.fromPlanetId);
                    if (from != null)
                    {
                        from.instabilityLevel = Math.Min(1f, from.instabilityLevel + 0.3f);
                        from.conflictState = ConflictState.Contested;
                    }
                    break;
            }

            // The Null Ark is consumed no matter what.
            if (wave.Contains("null_ark") && !report.attackerVesselsLost.Contains("null_ark"))
            {
                attacker.fleetVesselIds.Remove("null_ark");
                report.attackerVesselsLost.Add("null_ark");
            }

            // Repair swarm: the planet patches itself up after any battle it survived.
            if (!report.captured && target.builtDefenseIds.Contains("repair_swarm") && target.stationedDefenseUnits < 1)
                target.stationedDefenseUnits = 1;
        }

        private static void Capture(PlanetNode target, int newOwner)
        {
            target.ownerId = newOwner;
            target.instabilityLevel = ConquestRules.CaptureInstability;
            target.defenseLevel = Math.Max(1, target.defenseLevel / 2);
            target.stationedDefenseUnits = 0;
            target.conflictState = ConflictState.Contested;
            // Structures don't change hands intact — the fight wrecks them.
            target.builtDefenseIds.Clear();
        }

        private static void LoseVessels(ConquestPlayer attacker, List<string> wave, int count, BattleReport report)
        {
            for (int i = 0; i < count && i < wave.Count; i++)
            {
                attacker.fleetVesselIds.Remove(wave[i]);
                report.attackerVesselsLost.Add(wave[i]);
            }
        }

        private static int CountIn(List<string> list, string id)
        {
            int n = 0;
            for (int i = 0; i < list.Count; i++) if (list[i] == id) n++;
            return n;
        }
    }
}
