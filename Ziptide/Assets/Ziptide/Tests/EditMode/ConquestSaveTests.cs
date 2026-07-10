using NUnit.Framework;
using System.Collections.Generic;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ConquestSave contract: a played campaign round-trips field-for-field through the one-line
    /// overlay format, a loaded war resolves battles identically to the original (determinism is
    /// save-proof), garbage never crashes (null = fresh war), and saves survive the galaxy changing.
    /// </summary>
    public class ConquestSaveTests
    {
        private static ConquestState PlayedGame()
        {
            var s = ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());
            var p0 = s.GetPlayer(0);
            p0.flux = 9; p0.alloy = 6; p0.bloommatter = 2;
            s.BuildVessel(0, "pulse_frigate");
            s.BuildDefense(0, "toxic_city", "shield_spire");
            var order = new AttackOrder
            {
                attackerId = 0, fromPlanetId = "toxic_city", targetPlanetId = "dry_cistern",
                vesselIds = new List<string>(p0.fleetVesselIds),
            };
            ConquestResolver.Resolve(s, order, seed: 42);
            s.EndTurn();
            s.GetPlanet("glass_shelf").instabilityLevel = 0.4f;
            s.GetPlanet("glass_shelf").bloomContaminationLevel = 0.25f;
            return s;
        }

        [Test]
        public void RoundTrip_IsFieldForField()
        {
            var original = PlayedGame();
            string line = ConquestSave.Serialize(original);
            Assert.IsFalse(line.Contains("\n"), "must fit a single profile flag");
            Assert.IsFalse(line.Contains(" "), "no spaces — flag-safe");

            var loaded = ConquestSave.Deserialize(line, ConquestGalaxy.ChapterOneTwoSeeds());
            Assert.IsNotNull(loaded);
            Assert.AreEqual(original.turn, loaded.turn);

            for (int i = 0; i < 2; i++)
            {
                var a = original.GetPlayer(i); var b = loaded.GetPlayer(i);
                Assert.AreEqual(a.flux, b.flux, "flux p" + i);
                Assert.AreEqual(a.alloy, b.alloy, "alloy p" + i);
                Assert.AreEqual(a.bloommatter, b.bloommatter, "bloom p" + i);
                Assert.AreEqual(a.attacksThisTurn, b.attacksThisTurn, "attacks p" + i);
                CollectionAssert.AreEqual(a.fleetVesselIds, b.fleetVesselIds, "fleet p" + i);
            }
            foreach (var p in original.planets)
            {
                var q = loaded.GetPlanet(p.planetId);
                Assert.AreEqual(p.ownerId, q.ownerId, p.planetId + " owner");
                Assert.AreEqual(p.defenseLevel, q.defenseLevel, p.planetId + " def");
                Assert.AreEqual(p.orbitalShieldLevel, q.orbitalShieldLevel, p.planetId + " shield");
                Assert.AreEqual(p.stationedDefenseUnits, q.stationedDefenseUnits, p.planetId + " units");
                Assert.AreEqual(p.conflictState, q.conflictState, p.planetId + " conflict");
                Assert.AreEqual(p.instabilityLevel, q.instabilityLevel, 0.001f, p.planetId + " instab");
                Assert.AreEqual(p.bloomContaminationLevel, q.bloomContaminationLevel, 0.001f, p.planetId + " contam");
                CollectionAssert.AreEqual(p.builtDefenseIds, q.builtDefenseIds, p.planetId + " defenses");
            }
        }

        [Test]
        public void LoadedWar_ResolvesIdenticallyToTheOriginal()
        {
            var original = PlayedGame();
            var loaded = ConquestSave.Deserialize(ConquestSave.Serialize(original),
                                                  ConquestGalaxy.ChapterOneTwoSeeds());
            AttackOrder Order(ConquestState s) => new AttackOrder
            {
                attackerId = 0, fromPlanetId = "toxic_city",
                targetPlanetId = original.GetPlanet("dry_cistern").ownerId == 0 ? "glass_shelf" : "dry_cistern",
                vesselIds = new List<string>(s.GetPlayer(0).fleetVesselIds),
            };
            var r1 = ConquestResolver.Resolve(original, Order(original), seed: 99);
            var r2 = ConquestResolver.Resolve(loaded, Order(loaded), seed: 99);
            Assert.AreEqual(r1.outcome, r2.outcome);
            Assert.AreEqual(r1.odds, r2.odds, 1e-6f);
            Assert.AreEqual(r1.roll, r2.roll, 1e-6f);
        }

        [Test]
        public void Garbage_NeverCrashes_JustReturnsNull()
        {
            var seeds = ConquestGalaxy.ChapterOneTwoSeeds();
            Assert.IsNull(ConquestSave.Deserialize(null, seeds));
            Assert.IsNull(ConquestSave.Deserialize("", seeds));
            Assert.IsNull(ConquestSave.Deserialize("not_a_save", seeds));
            Assert.IsNull(ConquestSave.Deserialize("CONQ1|t=potato", seeds));
            Assert.IsNull(ConquestSave.Deserialize("CONQ1|Wtoxic_city=1,x", seeds));
        }

        [Test]
        public void HotseatMode_RidesTheSave_AndOldSavesReadAsSolo()
        {
            var s = ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());
            string line = ConquestSave.Serialize(s, hotseat: true, activeSide: 1);

            ConquestSave.ReadMode(line, out bool hs, out int side);
            Assert.IsTrue(hs); Assert.AreEqual(1, side);

            // The H record must not disturb the state parse itself.
            var loaded = ConquestSave.Deserialize(line, ConquestGalaxy.ChapterOneTwoSeeds());
            Assert.IsNotNull(loaded);
            Assert.AreEqual(s.turn, loaded.turn);

            // A pre-B4 save (no H record) reads as solo, side 0 — never a crash.
            string old = ConquestSave.Serialize(s);
            ConquestSave.ReadMode(old, out hs, out side);
            Assert.IsFalse(hs); Assert.AreEqual(0, side);
            ConquestSave.ReadMode(null, out hs, out side);
            Assert.IsFalse(hs);

            // A mangled H record clamps rather than poisons.
            ConquestSave.ReadMode("CONQ1|H=1,7|t=2", out hs, out side);
            Assert.IsTrue(hs); Assert.AreEqual(0, side, "an invalid side clamps to 0");
        }

        [Test]
        public void UnknownPlanet_IsSkipped_NotFatal()
        {
            var s = ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());
            s.GetPlanet("toxic_city").defenseLevel = 5;
            string line = ConquestSave.Serialize(s) + "|Wdeleted_world=1,9,9,9,0,0.000,0.000";
            var loaded = ConquestSave.Deserialize(line, ConquestGalaxy.ChapterOneTwoSeeds());
            Assert.IsNotNull(loaded, "a record for a removed world must not kill the save");
            Assert.AreEqual(5, loaded.GetPlanet("toxic_city").defenseLevel);
        }
    }
}
