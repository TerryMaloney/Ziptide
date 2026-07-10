using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Content.Automation;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1i — pins belts-beyond-the-sandbox: BeltPadLibrary derives one pad per
    /// mine-bearing pack (spec-is-truth, regenerated, wired to the mine's own hopper and resource),
    /// and BeltPadSpawner materializes pack data into a working pad — floor with the 4.1f save
    /// identity, authored port + sink at the line's ends, intake mirroring the mine's definition
    /// truth, dispenser + conductor present, and the 4.1g budget clamp holding.
    /// </summary>
    public class BeltPadTests
    {
        private WorldPackDefinition _pack;
        private GameObject _parent;

        [SetUp]
        public void SetUp()
        {
            _pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
            _parent = new GameObject("PadTestParent");
        }

        [TearDown]
        public void TearDown()
        {
            if (_parent != null) Object.DestroyImmediate(_parent);
            if (_pack != null) Object.DestroyImmediate(_pack);
        }

        private MineSpawnDefinition Mine(string id = "cistern_extractor")
            => new MineSpawnDefinition
            {
                id = id, resourceId = "mineral", ratePerSecond = 0.05, storageCap = 40,
                localPosition = new Vector3(10f, 1f, -5f)
            };

        [Test]
        public void Library_DerivesOnePad_FromTheFirstMine()
        {
            _pack.mines.Add(Mine());
            BeltPadLibrary.EnsurePadsFor(_pack);
            Assert.AreEqual(1, _pack.beltFloors.Count);
            var pad = _pack.beltFloors[0];
            Assert.AreEqual("belt_pad_cistern_extractor", pad.id);
            Assert.AreEqual("cistern_extractor", pad.feedMineId);
            Assert.AreEqual("mineral", pad.payoutResourceId, "the sink pays what the mine produces");

            // Spec-is-truth: re-running regenerates, never accumulates.
            BeltPadLibrary.EnsurePadsFor(_pack);
            Assert.AreEqual(1, _pack.beltFloors.Count);
        }

        [Test]
        public void Library_NoMines_NoPads_AndClearsStale()
        {
            _pack.beltFloors.Add(new BeltFloorSpawnDefinition { id = "stale" });
            BeltPadLibrary.EnsurePadsFor(_pack);
            Assert.AreEqual(0, _pack.beltFloors.Count, "derived data regenerates from this build's mines");
        }

        [Test]
        public void Spawner_MaterializesAWorkingPad()
        {
            _pack.mines.Add(Mine());
            BeltPadLibrary.EnsurePadsFor(_pack);
            BeltPadSpawner.CreateAll(_pack, _parent.transform, "W002_DryCistern");

            var floor = _parent.GetComponentInChildren<BeltFloorRuntime>();
            Assert.IsNotNull(floor, "the pad floor exists");
            Assert.AreEqual("belt_pad_cistern_extractor", floor.floorId, "4.1f save identity");

            int mid = floor.depth / 2;
            Assert.IsTrue(floor.cells.Exists(c => c.x == 0 && c.z == mid && c.kind == CellKind.Source),
                "port authored at the west-middle cell");
            Assert.IsTrue(floor.cells.Exists(c => c.x == floor.width - 1 && c.z == mid
                && c.kind == CellKind.Sink && c.resourceId == "mineral"),
                "sink authored at the east-middle cell, paying the mine's resource");

            var intake = _parent.GetComponentInChildren<BeltMinePortRuntime>();
            Assert.IsNotNull(intake, "the intake adapter exists");
            Assert.AreEqual("cistern_extractor", intake.machineId, "bound to the mine's OWN hopper");
            Assert.AreEqual("mineral", intake.resourceId, "definition truth mirrored");
            Assert.AreEqual(0.05f, intake.ratePerSecond, 1e-4f);
            Assert.AreEqual("W002_DryCistern", intake.worldId);

            Assert.IsNotNull(_parent.GetComponentInChildren<BeltDispenserRuntime>());
            var conductor = _parent.GetComponentInChildren<BeltConductorRuntime>();
            Assert.IsNotNull(conductor);
            Assert.AreSame(floor, conductor.floor, "the conductor rides THIS pad");
            Assert.AreEqual(mid, conductor.startZ, "the ride starts at the port");
        }

        [Test]
        public void Spawner_ClampsAnOverBudgetGrid()
        {
            _pack.beltFloors.Add(new BeltFloorSpawnDefinition
            {
                id = "huge", width = 99, depth = 99 // way past the 4.1g cap
            });
            BeltPadSpawner.CreateAll(_pack, _parent.transform, "W");
            var floor = _parent.GetComponentInChildren<BeltFloorRuntime>();
            Assert.IsNotNull(floor);
            Assert.LessOrEqual(floor.width * floor.depth, 256, "runtime pads can't dodge the audit gate");
        }

        [Test]
        public void Spawner_EmptyPack_IsANoOp()
        {
            BeltPadSpawner.CreateAll(_pack, _parent.transform, "W");
            Assert.AreEqual(0, _parent.transform.childCount);
            BeltPadSpawner.CreateAll(null, _parent.transform, "W");
            Assert.AreEqual(0, _parent.transform.childCount);
        }
    }
}
