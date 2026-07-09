using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Content.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Ship pillar 2.1/2.2 pure layer: the six chassis, loadout math, wrap invariance
    /// (by construction), and the journey-decal determinism. Headless.</summary>
    public class ShipLoadoutCoreTests
    {
        [Test]
        public void SixChassis_DistinctSilhouettes_AllSlotsResolvable()
        {
            Assert.AreEqual(6, ShipChassisPreset.All.Length, "the spec's >=6 chassis");
            var seenSil = new HashSet<ShipSilhouette>();
            var seenIds = new HashSet<string>();
            foreach (var c in ShipChassisPreset.All)
            {
                Assert.IsTrue(seenIds.Add(c.Id), "duplicate chassis id " + c.Id);
                seenSil.Add(c.Silhouette);
                Assert.IsNotNull(c.SlotIds);
                Assert.Greater(c.SlotIds.Length, 0, c.Id + " exposes no slots");
                Assert.Greater(c.BaseStats.Speed, 0f);
            }
            Assert.AreEqual(6, seenSil.Count, "every chassis is its own silhouette class");
        }

        [Test]
        public void ChassisAreNotClones_StatsSpreadIsReal()
        {
            float minSpeed = float.MaxValue, maxSpeed = float.MinValue;
            float minCargo = float.MaxValue, maxCargo = float.MinValue;
            foreach (var c in ShipChassisPreset.All)
            {
                minSpeed = System.Math.Min(minSpeed, c.BaseStats.Speed);
                maxSpeed = System.Math.Max(maxSpeed, c.BaseStats.Speed);
                minCargo = System.Math.Min(minCargo, c.BaseStats.Cargo);
                maxCargo = System.Math.Max(maxCargo, c.BaseStats.Cargo);
            }
            Assert.GreaterOrEqual(maxSpeed - minSpeed, 15f, "the racer and the hauler must FEEL different");
            Assert.GreaterOrEqual(maxCargo - minCargo, 8f, "cargo identity must be real");
        }

        [Test]
        public void Loadout_ModulesShiftStats_TradeoffsIncluded()
        {
            var chassis = ShipChassisPreset.Find("interceptor");
            var stock = ShipLoadoutCore.Resolve(chassis, null);
            var tuned = ShipLoadoutCore.Resolve(chassis, new[]
            {
                ShipModulePreset.Find("engine_tide"),   // +6 spd, +0.3 boost, -0.5 handling
                ShipModulePreset.Find("wings_razor"),   // +1.2 handling, -1 armor
            });
            Assert.AreEqual(stock.Speed + 6f, tuned.Speed, 1e-3f);
            Assert.AreEqual(stock.Handling + 0.7f, tuned.Handling, 1e-3f, "deltas stack (-0.5 + 1.2)");
            Assert.AreEqual(stock.Armor - 1f, tuned.Armor, 1e-3f, "razor foils COST armor — modules are tradeoffs");
        }

        [Test]
        public void Loadout_Floors_NoCombinationBricksAShip()
        {
            // Stack every negative onto the weakest chassis: must still fly, steer, survive.
            var racer = ShipChassisPreset.Find("racer");
            var s = ShipLoadoutCore.Resolve(racer, new[]
            {
                ShipModulePreset.Find("engine_ion"),
                ShipModulePreset.Find("wings_bulwark"),
                ShipModulePreset.Find("hardpoint_plated"),
                ShipModulePreset.Find("cargo_long"),
            });
            Assert.GreaterOrEqual(s.Speed, 8f);
            Assert.GreaterOrEqual(s.Handling, 1f);
            Assert.GreaterOrEqual(s.Armor, 1f);
        }

        [Test]
        public void WrapInvariance_ByConstruction()
        {
            // The golden law: Resolve has NO wrap parameter — cosmetics cannot reach the numbers.
            // This test documents the contract; the compiler enforces it.
            var a = ShipLoadoutCore.Resolve(ShipChassisPreset.Find("gunship"), null);
            var b = ShipLoadoutCore.Resolve(ShipChassisPreset.Find("gunship"), null);
            Assert.AreEqual(a.Speed, b.Speed, 1e-6f);
            Assert.AreEqual(a.Armor, b.Armor, 1e-6f);
        }

        // ── Journey decals ───────────────────────────────────────────────────
        [Test]
        public void Decals_AreDeterministic_AndStoryOrdered()
        {
            var flags = new HashSet<string> { "W012_COMPLETE", "W001_COMPLETE", "C2_CONTAINMENT_REVEALED" };
            var a = ShipJourneyDecals.FromFlags(flags);
            var b = ShipJourneyDecals.FromFlags(flags);
            CollectionAssert.AreEqual(a, b, "same save = same hull, everywhere, forever");
            Assert.AreEqual("decal_first_contract", a[0], "story order, not flag-list order");
            Assert.AreEqual("decal_cage_sighted", a[1]);
            Assert.AreEqual("decal_maras_jump", a[2]);
        }

        [Test]
        public void Decals_EmptyOrNullFlags_MeanACleanHull()
        {
            Assert.AreEqual(0, ShipJourneyDecals.FromFlags(null).Count);
            Assert.AreEqual(0, ShipJourneyDecals.FromFlags(new HashSet<string>()).Count);
        }

        [Test]
        public void Decals_UnrelatedFlags_EarnNothing()
        {
            var flags = new HashSet<string> { "RILL_SAID_beat01_boot", "COSM_EQUIP:shiplivery=x" };
            Assert.AreEqual(0, ShipJourneyDecals.FromFlags(flags).Count, "decals are MILESTONES, not noise");
        }
    }
}
