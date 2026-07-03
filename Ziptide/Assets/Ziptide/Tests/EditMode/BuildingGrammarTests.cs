using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>V2.5 H1 — the building grammar's contracts: determinism, exactly-one-door on a
    /// fronted face, storey bounds, module accounting, and the no-contradiction guarantee.</summary>
    public class BuildingGrammarTests
    {
        private static Lot FrontedLot(bool s = true, bool e = false, bool n = false, bool w = false) =>
            new Lot { Bounds = new Rect(0f, 0f, 12f, 9f), FrontS = s, FrontE = e, FrontN = n, FrontW = w };

        private static BuildingPlan Plan(int seed = 42, Lot? lot = null) =>
            BuildingGrammar.Plan(lot ?? FrontedLot(), BuildingStyleData.Default, seed);

        [Test]
        public void SameSeed_IdenticalPlan()
        {
            var a = Plan(); var b = Plan();
            Assert.AreEqual(a.Storeys, b.Storeys);
            Assert.AreEqual(a.DoorFace, b.DoorFace);
            Assert.AreEqual(a.DoorCell, b.DoorCell);
            Assert.AreEqual(a.Modules.Count, b.Modules.Count);
            for (int i = 0; i < a.Modules.Count; i++)
            {
                Assert.AreEqual(a.Modules[i].Module, b.Modules[i].Module, "module " + i);
                Assert.AreEqual(a.Modules[i].LocalPos, b.Modules[i].LocalPos, "pos " + i);
            }
        }

        [Test]
        public void DifferentSeeds_DifferentBuildings()
        {
            var a = Plan(1); var b = Plan(2);
            bool differs = a.Storeys != b.Storeys || a.DoorFace != b.DoorFace || a.DoorCell != b.DoorCell;
            for (int i = 0; !differs && i < Mathf.Min(a.Modules.Count, b.Modules.Count); i++)
                differs = a.Modules[i].Module != b.Modules[i].Module;
            Assert.IsTrue(differs || a.Modules.Count != b.Modules.Count);
        }

        [Test]
        public void ExactlyOneDoor_AndOnTheGroundStorey()
        {
            for (int seed = 1; seed <= 30; seed++)
            {
                var plan = Plan(seed);
                int doors = 0;
                foreach (var m in plan.Modules)
                    if (m.Module == BuildingModule.Doorway)
                    {
                        doors++;
                        Assert.AreEqual(0, m.Storey, "door above ground, seed " + seed);
                    }
                Assert.AreEqual(1, doors, "seed " + seed);
            }
        }

        [Test]
        public void Door_AlwaysOnAFrontedFace()
        {
            // A lot fronted ONLY on the east must always door east — across many seeds.
            var lot = FrontedLot(s: false, e: true);
            for (int seed = 1; seed <= 30; seed++)
            {
                var plan = BuildingGrammar.Plan(lot, BuildingStyleData.Default, seed);
                Assert.AreEqual(1, plan.DoorFace, "seed " + seed + " doored a wall with no street");
            }
        }

        [Test]
        public void UnfrontedLot_RefusesToBuild()
        {
            var lot = new Lot { Bounds = new Rect(0, 0, 12, 9) }; // no frontage flags
            Assert.IsNull(BuildingGrammar.Plan(lot, BuildingStyleData.Default, 7));
        }

        [Test]
        public void TinyLot_ReturnsNull_NotGarbage()
        {
            var lot = new Lot { Bounds = new Rect(0, 0, 2f, 2f), FrontS = true };
            Assert.IsNull(BuildingGrammar.Plan(lot, BuildingStyleData.Default, 7));
        }

        [Test]
        public void Storeys_StayInStyleRange()
        {
            var style = BuildingStyleData.Default;
            style.MinStoreys = 2; style.MaxStoreys = 3;
            for (int seed = 1; seed <= 20; seed++)
            {
                var plan = BuildingGrammar.Plan(FrontedLot(), style, seed);
                Assert.GreaterOrEqual(plan.Storeys, 2);
                Assert.LessOrEqual(plan.Storeys, 3);
            }
        }

        [Test]
        public void ModuleAccounting_WallsPerStoreyPlusTrimsSlabsRoof()
        {
            var plan = Plan(9);
            var cells = BuildingGrammar.CellCounts(FrontedLot(), BuildingStyleData.Default);
            int perimeter = 2 * (cells.x + cells.y);
            int walls = 0, trims = 0, slabs = 0, roofs = 0;
            foreach (var m in plan.Modules)
                switch (m.Module)
                {
                    case BuildingModule.WallSolid:
                    case BuildingModule.WallWindow:
                    case BuildingModule.Doorway: walls++; break;
                    case BuildingModule.CornerTrim: trims++; break;
                    case BuildingModule.FloorSlab: slabs++; break;
                    default: roofs++; break;
                }
            Assert.AreEqual(perimeter * plan.Storeys, walls, "wall cells");
            Assert.AreEqual(4 * plan.Storeys, trims, "corner trims");
            Assert.AreEqual(plan.Storeys, slabs, "floor slabs");
            Assert.AreEqual(1, roofs, "one roof");
        }

        [Test]
        public void WallPositions_SitOnTheFootprintPerimeter()
        {
            var plan = Plan(11);
            var lot = FrontedLot();
            float inset = BuildingGrammar.FootprintInset;
            foreach (var m in plan.Modules)
            {
                if (m.Module == BuildingModule.FloorSlab || m.Module == BuildingModule.RoofFlat
                    || m.Module == BuildingModule.RoofRaked) continue;
                Assert.GreaterOrEqual(m.LocalPos.x, lot.Bounds.xMin + inset - 0.01f);
                Assert.LessOrEqual(m.LocalPos.x, lot.Bounds.xMax - inset + 0.01f);
                Assert.GreaterOrEqual(m.LocalPos.y, lot.Bounds.yMin + inset - 0.01f);
                Assert.LessOrEqual(m.LocalPos.y, lot.Bounds.yMax - inset + 0.01f);
            }
        }

        [Test]
        public void EndToEnd_PartitionThenPlan_EveryLotBuildsWithALegalDoor()
        {
            // The full Q2 chain: district → lots → buildings. No lot may fail the door law.
            var lots = LotPartitioner.Partition(new Rect(-30, -24, 60, 48), 4f, 60f, 3f, 123);
            Assert.Greater(lots.Count, 0);
            int built = 0;
            foreach (var lot in lots)
            {
                var plan = BuildingGrammar.Plan(lot, BuildingStyleData.Default, 123 + built);
                if (plan == null) continue; // small lots may stay yards — legal
                built++;
                bool doorFaceFronted =
                    (plan.DoorFace == 0 && lot.FrontS) || (plan.DoorFace == 1 && lot.FrontE) ||
                    (plan.DoorFace == 2 && lot.FrontN) || (plan.DoorFace == 3 && lot.FrontW);
                Assert.IsTrue(doorFaceFronted, "door on unfronted face at " + lot.Bounds);
            }
            Assert.Greater(built, 0, "no lot in a 60x48 district produced a building");
        }
    }
}
