using NUnit.Framework;
using System.Collections.Generic;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1f — the belt-persistence overlay's pure contracts: an empty overlay reproduces
    /// the authored layout EXACTLY (the neutral-defaults law — old saves untouched), player edits
    /// round-trip through the profile JSON (the design doc's "factory persists" golden test), and
    /// a newer authored layout always shows through a stale save.
    /// </summary>
    public class BeltFloorSaveTests
    {
        private static BeltCellRecord Cell(int x, int z, int kind = 1, int dir = 1, string res = "")
            => new BeltCellRecord { x = x, z = z, kind = kind, dir = dir, resourceId = res };

        private static List<BeltCellRecord> Authored()
            => new List<BeltCellRecord> { Cell(0, 0), Cell(1, 0), Cell(2, 0, kind: 3, dir: 0, res: "scrap") };

        [Test]
        public void EmptyOverlay_ReproducesAuthoredExactly()
        {
            var authored = Authored();
            var outNone = BeltFloorSave.Apply(authored, null);
            var outEmpty = BeltFloorSave.Apply(authored, new BeltFloorState());
            Assert.AreEqual(authored.Count, outNone.Count);
            Assert.AreEqual(authored.Count, outEmpty.Count);
            for (int i = 0; i < authored.Count; i++)
            {
                Assert.AreSame(authored[i], outNone[i], "no overlay = the authored cells, in order");
                Assert.AreSame(authored[i], outEmpty[i], "empty overlay = pre-persistence behavior");
            }
        }

        [Test]
        public void Place_ThenApply_AddsTheCellAfterAuthored()
        {
            var f = new BeltFloorState();
            BeltFloorSave.RecordPlace(f, Cell(5, 1, dir: 0));
            var eff = BeltFloorSave.Apply(Authored(), f);
            Assert.AreEqual(4, eff.Count);
            Assert.AreEqual(5, eff[3].x);
            Assert.AreEqual(0, eff[3].dir);
        }

        [Test]
        public void Place_SameCellTwice_Upserts()
        {
            var f = new BeltFloorState();
            BeltFloorSave.RecordPlace(f, Cell(5, 1, dir: 0));
            BeltFloorSave.RecordPlace(f, Cell(5, 1, dir: 2));
            Assert.AreEqual(1, f.placed.Count, "re-placing a cell replaces it");
            Assert.AreEqual(2, f.placed[0].dir);
        }

        [Test]
        public void RemoveAuthored_ThenApply_DropsIt_AndSurvivesReRemove()
        {
            var f = new BeltFloorState();
            BeltFloorSave.RecordRemove(f, 1, 0, wasAuthored: true);
            BeltFloorSave.RecordRemove(f, 1, 0, wasAuthored: true); // dedupe
            Assert.AreEqual(1, f.removedAuthored.Count);
            var eff = BeltFloorSave.Apply(Authored(), f);
            Assert.AreEqual(2, eff.Count);
            Assert.IsNull(eff.Find(c => c.x == 1 && c.z == 0), "the removed authored cell is gone");
        }

        [Test]
        public void RemovePlaced_JustLeavesTheOverlay()
        {
            var f = new BeltFloorState();
            BeltFloorSave.RecordPlace(f, Cell(5, 1));
            BeltFloorSave.RecordRemove(f, 5, 1, wasAuthored: false);
            Assert.AreEqual(0, f.placed.Count);
            Assert.AreEqual(0, f.removedAuthored.Count, "a placed cell never lands in removedAuthored");
            Assert.AreEqual(3, BeltFloorSave.Apply(Authored(), f).Count);
        }

        [Test]
        public void RemoveAuthored_ThenPlaceOwn_PlacedWinsTheFreedCell()
        {
            var f = new BeltFloorState();
            BeltFloorSave.RecordRemove(f, 1, 0, wasAuthored: true);
            BeltFloorSave.RecordPlace(f, Cell(1, 0, dir: 3));
            var eff = BeltFloorSave.Apply(Authored(), f);
            Assert.AreEqual(3, eff.Count);
            var at = eff.Find(c => c.x == 1 && c.z == 0);
            Assert.IsNotNull(at);
            Assert.AreEqual(3, at.dir, "the player's replacement occupies the freed cell");
        }

        [Test]
        public void NewerAuthoredLayout_WinsOverAStalePlacedCell()
        {
            // The player placed at (3,0) long ago; a newer patcher now authors that cell too.
            var f = new BeltFloorState();
            BeltFloorSave.RecordPlace(f, Cell(3, 0, dir: 2));
            var authored = Authored();
            authored.Add(Cell(3, 0, dir: 1));
            var eff = BeltFloorSave.Apply(authored, f);
            Assert.AreEqual(4, eff.Count, "no duplicate cell at the contested coords");
            var at = eff.Find(c => c.x == 3 && c.z == 0);
            Assert.AreEqual(1, at.dir, "authored truth wins the collision");
        }

        [Test]
        public void GetFloor_CreatesOncePerFloorId_AndFindsItAgain()
        {
            var world = new WorldState { worldId = "SandboxTestLab" };
            Assert.IsNull(BeltFloorSave.GetFloor(world, "sandbox_belt", createIfMissing: false));
            var made = BeltFloorSave.GetFloor(world, "sandbox_belt", createIfMissing: true);
            Assert.IsNotNull(made);
            Assert.AreSame(made, BeltFloorSave.GetFloor(world, "sandbox_belt", createIfMissing: true));
            Assert.AreEqual(1, world.beltFloors.Count);
            Assert.IsNull(BeltFloorSave.GetFloor(world, "", createIfMissing: true),
                "an empty floorId never persists");
        }

        [Test]
        public void ProfileRoundTrip_PlayerFactorySurvivesTheJson()
        {
            // The design doc's golden test: a hand-built factory survives quit (serialize) and
            // reload (deserialize) — and an old save without beltFloors deserializes safely.
            var p = ProfileSerializer.NewProfile();
            var world = p.GetWorld("SandboxTestLab", createIfMissing: true);
            var f = BeltFloorSave.GetFloor(world, "sandbox_belt", createIfMissing: true);
            BeltFloorSave.RecordPlace(f, Cell(5, 1, dir: 0));
            BeltFloorSave.RecordPlace(f, Cell(5, 2, dir: 0));
            BeltFloorSave.RecordRemove(f, 1, 0, wasAuthored: true);

            var back = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(p));
            var w2 = back.GetWorld("SandboxTestLab");
            Assert.IsNotNull(w2);
            var f2 = BeltFloorSave.GetFloor(w2, "sandbox_belt", createIfMissing: false);
            Assert.IsNotNull(f2, "the floor overlay came back from disk");
            Assert.AreEqual(2, f2.placed.Count);
            Assert.AreEqual(1, f2.removedAuthored.Count);
            Assert.AreEqual(5, f2.placed[0].x);

            var eff = BeltFloorSave.Apply(Authored(), f2);
            Assert.AreEqual(4, eff.Count, "authored(3) - removed(1) + placed(2)");
        }

        [Test]
        public void OldSave_WithoutBeltFloors_DeserializesWithNeutralDefault()
        {
            // A pre-4.1f profile JSON has no beltFloors field at all.
            string oldJson = "{\"schemaVersion\":2,\"playerId\":\"p\",\"displayName\":\"Cal\"," +
                "\"createdAtUnix\":1,\"lastSavedAtUnix\":1,\"flags\":[],\"resources\":[]," +
                "\"worlds\":[{\"worldId\":\"SandboxTestLab\",\"discovered\":true,\"owned\":false," +
                "\"ownerId\":\"\",\"lastResolvedAtUnix\":1,\"mines\":[],\"plots\":[],\"factory\":[]}]," +
                "\"ledger\":[]}";
            var p = ProfileSerializer.Deserialize(oldJson);
            var world = p.GetWorld("SandboxTestLab");
            Assert.IsNotNull(world);
            Assert.IsNotNull(world.beltFloors, "additive field defaults, never null");
            Assert.AreEqual(0, world.beltFloors.Count, "old saves stay untouched");
        }
    }
}
