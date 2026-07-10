using System;
using System.Collections.Generic;
using Ziptide.Multiplayer.Bots;

namespace Ziptide.Multiplayer.Conquest
{
    /// <summary>One planned move — also the ITidefrontSync message unit for multiplayer.</summary>
    [Serializable]
    public class ConquestAction
    {
        public enum Kind { BuildDefense, BuildVessel, Attack, Pass }
        public Kind kind = Kind.Pass;
        public string planetId = "";
        public string catalogId = "";     // defense or vessel id
        public AttackOrder attack;        // when kind == Attack
    }

    /// <summary>AI difficulty knobs (data, mirroring BotProfileData's philosophy).</summary>
    [Serializable]
    public struct ConquestAiProfile
    {
        public float MinAttackOdds;       // won't attack below this estimate
        public int TargetFleetSize;       // builds vessels toward this
        public bool ReinforcesBorders;    // builds defenses on threatened planets
        public bool SavesForBigShips;     // prefers siege_lantern/null_ark when rich

        public static ConquestAiProfile Cautious => new ConquestAiProfile
        { MinAttackOdds = 0.65f, TargetFleetSize = 3, ReinforcesBorders = true, SavesForBigShips = false };

        public static ConquestAiProfile Balanced => new ConquestAiProfile
        { MinAttackOdds = 0.55f, TargetFleetSize = 4, ReinforcesBorders = true, SavesForBigShips = false };

        public static ConquestAiProfile Aggressive => new ConquestAiProfile
        { MinAttackOdds = 0.45f, TargetFleetSize = 6, ReinforcesBorders = false, SavesForBigShips = true };
    }

    /// <summary>
    /// The strategy opponent (concept doc: "collect, build, attack weak neighbors, reinforce borders").
    /// Deterministic given state + profile + seed. PlanTurn returns legal actions in play order; the
    /// caller (table runtime / headless test) applies them through ConquestState + ConquestResolver so
    /// the AI can never cheat — it uses the same API a human's table does.
    /// </summary>
    public static class ConquestAI
    {
        public static List<ConquestAction> PlanTurn(ConquestState state, int playerId, ConquestAiProfile profile, int seed)
        {
            var actions = new List<ConquestAction>();
            var me = state.GetPlayer(playerId);
            if (me == null) return actions;

            // 1. Reinforce the most-threatened border planet (adjacent to an enemy).
            if (profile.ReinforcesBorders)
            {
                PlanetNode weakest = null;
                foreach (var p in state.planets)
                {
                    if (p.ownerId != playerId || !HasEnemyNeighbor(state, p, playerId)) continue;
                    if (weakest == null || p.defenseLevel < weakest.defenseLevel) weakest = p;
                }
                if (weakest != null)
                    foreach (var d in ConquestCatalog.Defenses)
                        if (!weakest.builtDefenseIds.Contains(d.Id) && d.DefenseBonus > 0 &&
                            me.CanAfford(d.CostFlux, d.CostAlloy, d.CostBloom))
                        {
                            actions.Add(new ConquestAction { kind = ConquestAction.Kind.BuildDefense, planetId = weakest.planetId, catalogId = d.Id });
                            break; // affordability is re-verified when the action is applied
                        }
            }

            // 2. Build toward the target fleet.
            if (me.fleetVesselIds.Count < profile.TargetFleetSize)
            {
                string pick = profile.SavesForBigShips && me.CanAfford(3, 4, 0) ? "siege_lantern"
                    : me.CanAfford(2, 2, 0) ? "pulse_frigate"
                    : me.CanAfford(1, 1, 0) ? "scout_skiff" : null;
                if (pick != null)
                    actions.Add(new ConquestAction { kind = ConquestAction.Kind.BuildVessel, catalogId = pick });
            }

            // 3. Attack the weakest adjacent target whose estimated odds clear the bar.
            var best = FindBestAttack(state, playerId, profile);
            if (best != null)
            {
                // Vary the seed the caller should resolve with (kept in the action for sync parity).
                best.missionModifier = 0;
                actions.Add(new ConquestAction { kind = ConquestAction.Kind.Attack, attack = best });
            }

            if (actions.Count == 0)
                actions.Add(new ConquestAction { kind = ConquestAction.Kind.Pass });
            return actions;
        }

        /// <summary>Estimated odds for the full fleet vs a target — the same math the resolver uses.</summary>
        public static float EstimateOdds(ConquestState state, int playerId, string targetId)
        {
            var me = state.GetPlayer(playerId);
            if (me == null) return 0f;
            return EstimateOdds(state, me.fleetVesselIds, targetId);
        }

        /// <summary>Wave variant: odds for exactly these vessels — the fleet-rack picker shows what
        /// the wave you actually committed would do, not what the whole fleet could.</summary>
        public static float EstimateOdds(ConquestState state, List<string> vesselIds, string targetId)
        {
            var target = state.GetPlanet(targetId);
            if (target == null || vesselIds == null) return 0f;
            int attack = 0;
            foreach (var id in vesselIds)
            {
                var v = ConquestCatalog.Vessel(id);
                if (v != null) attack += v.Value.AttackPower;
            }
            int defense = target.defenseLevel + target.orbitalShieldLevel + target.stationedDefenseUnits;
            return ConquestRules.ComputeOdds(attack, defense);
        }

        private static AttackOrder FindBestAttack(ConquestState state, int playerId, ConquestAiProfile profile)
        {
            var me = state.GetPlayer(playerId);
            if (me == null || me.fleetVesselIds.Count == 0) return null;

            AttackOrder best = null;
            float bestOdds = 0f;
            foreach (var from in state.planets)
            {
                if (from.ownerId != playerId) continue;
                foreach (var adjId in from.adjacentPlanetIds)
                {
                    var target = state.GetPlanet(adjId);
                    if (target == null || target.ownerId == playerId) continue;
                    if (!state.CanAttack(playerId, from.planetId, adjId)) continue;
                    float odds = EstimateOdds(state, playerId, adjId);
                    if (odds >= profile.MinAttackOdds && odds > bestOdds)
                    {
                        bestOdds = odds;
                        best = new AttackOrder
                        {
                            attackerId = playerId,
                            fromPlanetId = from.planetId,
                            targetPlanetId = adjId,
                            vesselIds = new List<string>(me.fleetVesselIds), // commits the fleet
                        };
                    }
                }
            }
            return best;
        }

        private static bool HasEnemyNeighbor(ConquestState state, PlanetNode p, int playerId)
        {
            foreach (var id in p.adjacentPlanetIds)
            {
                var n = state.GetPlanet(id);
                if (n != null && n.ownerId != playerId && n.ownerId >= 0) return true;
            }
            return false;
        }
    }
}
