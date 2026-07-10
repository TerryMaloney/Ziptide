using NUnit.Framework;
using System.Collections.Generic;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The Tidefront sim contract (docs/10_TIDEFRONT.md "first testable checks" + TIDEFRONT_AAA):
    /// odds clamped 10–90, adjacency-only attacks, resources deduct + ownership flips, structures do
    /// what they claim, dogpile shield, anti-snowball economy, full determinism, an AI that plays a
    /// legal game headless, and a save round-trip.
    /// </summary>
    public class ConquestTests
    {
        private static ConquestState NewGame() =>
            ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());

        private static ConquestState SkirmishState(out AttackOrder order, int fleetPower = 4)
        {
            // Minimal 2-planet war: p0 owns "a" with a fleet, "b" is the adjacent target.
            var s = new ConquestState();
            s.players.Add(new ConquestPlayer { playerId = 0, flux = 20, alloy = 20, bloommatter = 5 });
            s.players.Add(new ConquestPlayer { playerId = 1 });
            var a = new PlanetNode { planetId = "a", ownerId = 0 };
            var b = new PlanetNode { planetId = "b", ownerId = 1, defenseLevel = 1 };
            a.adjacentPlanetIds.Add("b"); b.adjacentPlanetIds.Add("a");
            s.planets.Add(a); s.planets.Add(b);
            for (int i = 0; i < fleetPower / 2; i++) s.GetPlayer(0).fleetVesselIds.Add("pulse_frigate");
            order = new AttackOrder
            {
                attackerId = 0, fromPlanetId = "a", targetPlanetId = "b",
                vesselIds = new List<string>(s.GetPlayer(0).fleetVesselIds),
            };
            return s;
        }

        // ── Odds & math ─────────────────────────────────────────────────────
        [Test]
        public void Odds_ClampBetween10And90()
        {
            Assert.AreEqual(0.10f, ConquestRules.ComputeOdds(0, 100), 1e-5f);
            Assert.AreEqual(0.90f, ConquestRules.ComputeOdds(100, 0), 1e-5f);
            Assert.AreEqual(0.50f, ConquestRules.ComputeOdds(5, 5), 1e-5f);
            Assert.AreEqual(0.55f, ConquestRules.ComputeOdds(6, 5), 1e-5f, "each point ≈ +5%");
        }

        [Test]
        public void WaveOdds_DropWhenVesselsAreHeldBack()
        {
            var s = SkirmishState(out _, fleetPower: 8);   // 4 frigates
            float full = ConquestAI.EstimateOdds(s, 0, "b");
            float half = ConquestAI.EstimateOdds(s, new List<string> { "pulse_frigate" }, "b");
            Assert.Less(half, full, "a partial wave must show weaker odds");
            Assert.AreEqual(full, ConquestAI.EstimateOdds(s, s.GetPlayer(0).fleetVesselIds, "b"), 1e-6f,
                "the full-fleet overload equals the wave overload fed the whole fleet");
        }

        // ── Legality ────────────────────────────────────────────────────────
        [Test]
        public void Attacks_AreAdjacentOnly_FromOwnedPlanets()
        {
            var s = NewGame();
            Assert.IsTrue(s.CanAttack(0, "toxic_city", "dry_cistern"), "own → adjacent enemy/neutral");
            Assert.IsFalse(s.CanAttack(0, "toxic_city", "the_hum"), "not adjacent");
            Assert.IsFalse(s.CanAttack(0, "dry_cistern", "glass_shelf"), "can't attack FROM a planet you don't own");
            Assert.IsFalse(s.CanAttack(1, "maras_last_jump", "maras_last_jump"), "can't attack yourself");
        }

        [Test]
        public void AttackLimit_PerTurn_IsEnforced()
        {
            var s = SkirmishState(out var order);
            s.GetPlayer(0).attacksThisTurn = ConquestRules.MaxAttacksPerTurn;
            Assert.IsFalse(s.CanAttack(0, "a", "b"));
            s.EndTurn();
            Assert.IsTrue(s.CanAttack(0, "a", "b"), "resets next turn");
        }

        // ── Building ────────────────────────────────────────────────────────
        [Test]
        public void BuildDefense_PaysAndRaisesDefense_OncePerPlanet()
        {
            var s = SkirmishState(out _);
            var p = s.GetPlayer(0);
            int alloyBefore = p.alloy;
            int defBefore = s.GetPlanet("a").defenseLevel;

            Assert.IsTrue(s.BuildDefense(0, "a", "shield_spire"));
            Assert.AreEqual(alloyBefore - 3, p.alloy, "cost deducted");
            Assert.AreEqual(defBefore + 3, s.GetPlanet("a").defenseLevel, "bonus applied");
            Assert.IsFalse(s.BuildDefense(0, "a", "shield_spire"), "one of each per planet");
            Assert.IsFalse(s.BuildDefense(0, "b", "shield_spire"), "can't build on enemy planets");
        }

        [Test]
        public void BloomBarrier_TradesContamination()
        {
            var s = SkirmishState(out _);
            Assert.IsTrue(s.BuildDefense(0, "a", "bloom_barrier"));
            Assert.Greater(s.GetPlanet("a").bloomContaminationLevel, 0f);
        }

        [Test]
        public void BuildVessel_RejectsWhenBroke()
        {
            var s = SkirmishState(out _);
            var p = s.GetPlayer(0);
            p.flux = 0; p.alloy = 0;
            Assert.IsFalse(s.BuildVessel(0, "pulse_frigate"));
            p.flux = 2; p.alloy = 2;
            Assert.IsTrue(s.BuildVessel(0, "pulse_frigate"));
        }

        // ── Resolution ──────────────────────────────────────────────────────
        [Test]
        public void Victory_FlipsOwnership_AndDestabilizes()
        {
            var s = SkirmishState(out var order, fleetPower: 20); // overwhelming
            var report = ConquestResolver.Resolve(s, order, seed: 3); // strong odds → some winning seed
            // With attack 20 vs defense 1, odds = 0.9; find a winning seed deterministically:
            int seed = 3;
            while (!report.captured && seed < 50)
                report = ConquestResolver.Resolve(RebuildAndOrder(out order, 20), order, ++seed);
            Assert.IsTrue(report.captured, "an overwhelming fleet captures within a few seeds");
            // The re-used state from the last iteration:
            var s2 = RebuildAndOrder(out var o2, 20);
            var r2 = ConquestResolver.Resolve(s2, o2, seed);
            Assert.AreEqual(0, s2.GetPlanet("b").ownerId, "ownership flipped");
            Assert.AreEqual(ConquestRules.CaptureInstability, s2.GetPlanet("b").instabilityLevel, 1e-5f);
            Assert.AreEqual(0, s2.GetPlanet("b").builtDefenseIds.Count, "structures wrecked on capture");
        }

        private static ConquestState RebuildAndOrder(out AttackOrder order, int fleetPower)
            => SkirmishState(out order, fleetPower);

        [Test]
        public void GateJammer_BlocksWithoutPiercer_PiercerIgnoresIt()
        {
            var s = SkirmishState(out var order, fleetPower: 20);
            s.GetPlanet("b").builtDefenseIds.Add("gate_jammer");
            var blocked = ConquestResolver.Resolve(s, order, seed: 5);
            Assert.AreEqual(BattleOutcome.FailedAttack, blocked.outcome, "jammer stops the wave cold");

            var s2 = SkirmishState(out var order2, fleetPower: 20);
            s2.GetPlanet("b").builtDefenseIds.Add("gate_jammer");
            s2.GetPlayer(0).fleetVesselIds.Add("gate_piercer");
            order2.vesselIds.Add("gate_piercer");
            var pierced = ConquestResolver.Resolve(s2, order2, seed: 5);
            Assert.AreNotEqual(0, pierced.attackScore, "piercer lets the attack resolve");
        }

        [Test]
        public void Shieldbreaker_IgnoresOrbitalShield()
        {
            var s = SkirmishState(out var order, fleetPower: 2);
            s.GetPlanet("b").orbitalShieldLevel = 5;
            var withShield = ConquestResolver.Resolve(s, order, seed: 5);

            var s2 = SkirmishState(out var order2, fleetPower: 2);
            s2.GetPlanet("b").orbitalShieldLevel = 5;
            s2.GetPlayer(0).fleetVesselIds.Add("shieldbreaker_barge");
            order2.vesselIds.Add("shieldbreaker_barge");
            var broken = ConquestResolver.Resolve(s2, order2, seed: 5);

            Assert.Greater(broken.odds, withShield.odds, "ignoring the shield improves the odds");
        }

        [Test]
        public void Dogpile_RaisesDefense_OnAlreadyAttackedPlanet()
        {
            var s = SkirmishState(out var order, fleetPower: 4);
            s.GetPlanet("b").conflictState = ConflictState.UnderAttack; // someone hit it this turn
            var report = ConquestResolver.Resolve(s, order, seed: 5);
            Assert.AreEqual(1 + ConquestRules.DogpileBonus, report.defenseScore, "dogpile shield active");
        }

        [Test]
        public void MissionModifiers_ShiftTheOdds_BothWays()
        {
            var s = SkirmishState(out var order, fleetPower: 4);
            var baseline = ConquestResolver.Resolve(s, order, seed: 5);

            var s2 = SkirmishState(out var order2, fleetPower: 4);
            order2.missionModifier = 3;                       // attack mission won
            var boosted = ConquestResolver.Resolve(s2, order2, seed: 5);
            Assert.Greater(boosted.odds, baseline.odds, "a won VR mission tilts the battle");

            var s3 = SkirmishState(out var order3, fleetPower: 4);
            order3.missionModifier = -3;                      // defender's mission won
            var sapped = ConquestResolver.Resolve(s3, order3, seed: 5);
            Assert.LessOrEqual(sapped.odds, baseline.odds);
        }

        [Test]
        public void FailedAttack_LosesTheWave()
        {
            var s = SkirmishState(out var order, fleetPower: 2);
            s.GetPlanet("b").defenseLevel = 30;               // hopeless (odds clamp to 10%)
            int seed = 1;
            BattleReport report = ConquestResolver.Resolve(s, order, seed);
            while (report.captured && seed < 50)              // find a losing seed (90% of them)
            { s = SkirmishState(out order, 2); s.GetPlanet("b").defenseLevel = 30; report = ConquestResolver.Resolve(s, order, ++seed); }
            Assert.IsFalse(report.captured);
            Assert.Greater(report.attackerVesselsLost.Count, 0, "losses on a failed/stalemated attack");
        }

        [Test]
        public void Determinism_SameSeed_SameWar()
        {
            var a = SkirmishState(out var orderA, fleetPower: 4);
            var b = SkirmishState(out var orderB, fleetPower: 4);
            var ra = ConquestResolver.Resolve(a, orderA, seed: 77);
            var rb = ConquestResolver.Resolve(b, orderB, seed: 77);
            Assert.AreEqual(ra.outcome, rb.outcome);
            Assert.AreEqual(ra.roll, rb.roll, 1e-6f);
            Assert.AreEqual(ra.attackerVesselsLost.Count, rb.attackerVesselsLost.Count);
        }

        // ── Economy / anti-snowball ─────────────────────────────────────────
        [Test]
        public void EndTurn_Produces_InstabilityPenalized_AndDecays()
        {
            var s = SkirmishState(out _);
            var a = s.GetPlanet("a");
            a.resourceType = ConquestResource.Flux; a.resourceProductionRate = 4f;
            a.instabilityLevel = 1f;                           // fresh capture worst-case
            int before = s.GetPlayer(0).flux;
            s.EndTurn();
            Assert.AreEqual(before + 2, s.GetPlayer(0).flux, "produces at 50% under full instability");
            Assert.AreEqual(1f - ConquestRules.InstabilityDecayPerTurn, a.instabilityLevel, 1e-5f, "instability decays");
        }

        [Test]
        public void EndTurn_ChargesFleetUpkeep()
        {
            var s = SkirmishState(out _);
            var p = s.GetPlayer(0);
            p.fleetVesselIds.Clear();
            for (int i = 0; i < ConquestRules.VesselsPerUpkeepFlux * 2; i++) p.fleetVesselIds.Add("scout_skiff");
            int before = p.flux;
            var a = s.GetPlanet("a"); a.ownerId = -1;          // no production noise
            s.EndTurn();
            Assert.AreEqual(before - 2, p.flux, "1 flux per 4 vessels");
        }

        // ── The galaxy & the AI ─────────────────────────────────────────────
        [Test]
        public void Galaxy_IsBuiltFromTheStoryWorlds_AsAWeb()
        {
            var s = NewGame();
            Assert.AreEqual(12, s.planets.Count, "W001–W012");
            Assert.AreEqual(0, s.GetPlanet("toxic_city").ownerId, "player starts at Toxic Venice");
            Assert.AreEqual(1, s.GetPlanet("maras_last_jump").ownerId, "rival holds the far gate");
            Assert.IsTrue(s.GetPlanet("toxic_city").adjacentPlanetIds.Contains("broadcast_tomb"),
                "cross-links make it a web, not a line");
        }

        [Test]
        public void AI_PlaysAFullLegalGame_Headless()
        {
            var s = NewGame();
            // Give both sides working capital so the game moves.
            foreach (var p in s.players) { p.flux = 12; p.alloy = 12; p.bloommatter = 3; }

            for (int t = 0; t < 40; t++)
            {
                foreach (var player in new[] { 0, 1 })
                {
                    var plan = ConquestAI.PlanTurn(s, player, ConquestAiProfile.Balanced, seed: t * 10 + player);
                    foreach (var action in plan)
                    {
                        switch (action.kind)
                        {
                            case ConquestAction.Kind.BuildDefense: s.BuildDefense(player, action.planetId, action.catalogId); break;
                            case ConquestAction.Kind.BuildVessel: s.BuildVessel(player, action.catalogId); break;
                            case ConquestAction.Kind.Attack:
                                if (s.CanAttack(player, action.attack.fromPlanetId, action.attack.targetPlanetId))
                                    ConquestResolver.Resolve(s, action.attack, seed: t * 100 + player);
                                break;
                        }
                    }
                }
                s.EndTurn();
                if (s.CountOwned(0) == 0 || s.CountOwned(1) == 0) break;    // someone's eliminated
            }

            int total = s.CountOwned(0) + s.CountOwned(1);
            Assert.Greater(total, 2, "the AIs expanded beyond their homeworlds");
            foreach (var p in s.players)
                Assert.GreaterOrEqual(p.flux, 0, "stockpiles never go negative");
        }

        [Test]
        public void SaveRoundTrip_ViaJsonUtility()
        {
            var s = NewGame();
            s.BuildDefense(0, "toxic_city", "shield_spire");
            s.GetPlayer(0).fleetVesselIds.Add("pulse_frigate");
            s.EndTurn();

            string json = UnityEngine.JsonUtility.ToJson(s);
            var back = UnityEngine.JsonUtility.FromJson<ConquestState>(json);

            Assert.AreEqual(s.turn, back.turn);
            Assert.AreEqual(s.planets.Count, back.planets.Count);
            Assert.AreEqual(s.GetPlanet("toxic_city").defenseLevel, back.GetPlanet("toxic_city").defenseLevel);
            Assert.AreEqual(1, back.GetPlayer(0).fleetVesselIds.Count, "fleet survives the round trip");
        }
    }
}
