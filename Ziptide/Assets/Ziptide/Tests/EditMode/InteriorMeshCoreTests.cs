using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 1.3 — walls-from-plan. Pins: wall boxes NEVER cover walkable space (rooms and
    /// corridors stay open), non-walkable space IS covered (no see-through gaps in the wall mass),
    /// the doorway entry-carve connects to the network, and everything is deterministic.
    /// </summary>
    public class InteriorMeshCoreTests
    {
        private static readonly Rect Footprint = new Rect(0f, 0f, 14f, 12f);

        private static InteriorPlan Plan(int seed = 7)
            => RoomPartitioner.Partition(Footprint, corridorWidth: 1.2f, minRoomArea: 8f,
                maxAspect: 2.5f, seed: seed);

        [Test]
        public void Walls_NeverCoverWalkableSpace()
        {
            var plan = Plan();
            var walls = InteriorMeshCore.RasterizeWalls(Footprint, plan);
            Assert.Greater(walls.Count, 0, "a partitioned interior has wall mass");

            // Sample every room and corridor center — the guaranteed-open points.
            foreach (var open in AllWalkable(plan))
                foreach (var w in walls)
                    Assert.IsFalse(w.Contains(open.center),
                        "wall box " + w + " covers walkable center " + open.center);
        }

        [Test]
        public void NonWalkableSpace_IsCoveredByWalls()
        {
            var plan = Plan();
            var walls = InteriorMeshCore.RasterizeWalls(Footprint, plan);

            // Dense sample: every point clearly in wall territory is inside some wall box. Points
            // within one raster cell of a walkable border are legitimate slop (center-sampled
            // rasterization) and are skipped — the contract is "no see-through gaps in wall MASS".
            float slop = InteriorMeshCore.CellSize;
            for (float y = 0.3f; y < Footprint.height; y += 0.7f)
            for (float x = 0.3f; x < Footprint.width; x += 0.7f)
            {
                var p = new Vector2(x, y);
                bool nearWalkable = false;
                foreach (var r in AllWalkable(plan))
                {
                    var grown = new Rect(r.x - slop, r.y - slop, r.width + 2f * slop, r.height + 2f * slop);
                    if (grown.Contains(p)) { nearWalkable = true; break; }
                }
                if (nearWalkable) continue;
                bool inWall = false;
                foreach (var w in walls) if (w.Contains(p)) { inWall = true; break; }
                Assert.IsTrue(inWall, "gap at " + p + " — neither walkable nor wall");
            }
        }

        [Test]
        public void WithEntry_ConnectsTheDoorwayIntoTheNetwork()
        {
            var plan = Plan();
            var entry = new Vector2(0.2f, 6f); // just inside the west wall — a doorway's inner face
            InteriorMeshCore.WithEntry(plan, entry, corridorWidth: 1.1f);

            Assert.IsTrue(RoomPartitioner.IsFullyConnected(plan),
                "entry carve must preserve full connectivity");
            bool entryOpen = false;
            foreach (var r in AllWalkable(plan)) if (r.Contains(entry)) { entryOpen = true; break; }
            Assert.IsTrue(entryOpen, "the doorway's inner face must be walkable after the carve");
        }

        [Test]
        public void Deterministic_SameSeedSameWalls()
        {
            var a = InteriorMeshCore.RasterizeWalls(Footprint, Plan(11));
            var b = InteriorMeshCore.RasterizeWalls(Footprint, Plan(11));
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++) Assert.AreEqual(a[i], b[i]);
        }

        [Test]
        public void MergedBoxes_StayCoarse_QuestBudget()
        {
            var plan = Plan();
            var walls = InteriorMeshCore.RasterizeWalls(Footprint, plan);
            int cells = Mathf.RoundToInt(Footprint.width / InteriorMeshCore.CellSize)
                      * Mathf.RoundToInt(Footprint.height / InteriorMeshCore.CellSize);
            Assert.Less(walls.Count, cells / 6,
                "greedy merge must produce far fewer boxes than raw cells (renderer budget)");
        }

        private static IEnumerable<Rect> AllWalkable(InteriorPlan plan)
        {
            foreach (var r in plan.Rooms) yield return r;
            foreach (var c in plan.Corridors) yield return c;
        }
    }
}
