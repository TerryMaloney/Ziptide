using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests
{
    /// <summary>
    /// HARDWIRING 1.3e — pins the pure furnish planner (RoomFurnishCore) and the room-visibility
    /// step (InteriorVisibilityCore). The invariants ARE the interior gate: deterministic per seed,
    /// every room furnished, nothing in a corridor mouth, nothing overlapping, the walk lane free,
    /// the floor budget respected; visibility = own room + corridor-shared rooms only.
    /// </summary>
    public class InteriorFurnishCoreTests
    {
        // A real partitioned plan, entry-carved, like InteriorBuilder produces.
        private static InteriorPlan MakePlan(int seed, out Vector2 entry)
        {
            var footprint = new Rect(0f, 0f, 14f, 11f);
            var plan = RoomPartitioner.Partition(footprint, 1.2f, 8f, 2.5f, seed);
            entry = new Vector2(0.3f, 5.5f);
            InteriorMeshCore.WithEntry(plan, entry, 1.0f);
            return plan;
        }

        // A hand-built three-room plan: A—corridor—B—corridor—C. From A you see B, never C.
        private static InteriorPlan ChainPlan()
        {
            return new InteriorPlan
            {
                Rooms = new List<Rect>
                {
                    new Rect(0f, 0f, 3f, 3f),   // A
                    new Rect(5f, 0f, 3f, 3f),   // B
                    new Rect(10f, 0f, 3f, 3f),  // C
                },
                Corridors = new List<Rect>
                {
                    new Rect(2.5f, 1f, 3f, 1f),  // A ↔ B (overlaps both)
                    new Rect(7.5f, 1f, 3f, 1f),  // B ↔ C
                },
            };
        }

        [Test]
        public void Furnish_IsDeterministicPerSeed()
        {
            var a = RoomFurnishCore.Furnish(MakePlan(77, out var e1), e1, 1234);
            var b = RoomFurnishCore.Furnish(MakePlan(77, out var e2), e2, 1234);
            Assert.AreEqual(a.Count, b.Count);
            for (int r = 0; r < a.Count; r++)
            {
                Assert.AreEqual(a[r].Role, b[r].Role);
                Assert.AreEqual(a[r].Items.Count, b[r].Items.Count);
                for (int i = 0; i < a[r].Items.Count; i++)
                {
                    Assert.AreEqual(a[r].Items[i].Kind, b[r].Items[i].Kind);
                    Assert.AreEqual(a[r].Items[i].Pos, b[r].Items[i].Pos);
                }
            }
        }

        [Test]
        public void EveryRoom_GetsAtLeastOneFurnishing()
        {
            for (int seed = 1; seed <= 5; seed++)
            {
                var plan = MakePlan(seed * 31, out var entry);
                var furnish = RoomFurnishCore.Furnish(plan, entry, seed);
                Assert.AreEqual(plan.Rooms.Count, furnish.Count);
                foreach (var room in furnish)
                    Assert.GreaterOrEqual(room.Items.Count, 1,
                        "bare room " + room.RoomIndex + " (" + room.Role + ") seed=" + seed);
            }
        }

        [Test]
        public void Items_StayInsideTheirRoom()
        {
            var plan = MakePlan(42, out var entry);
            foreach (var room in RoomFurnishCore.Furnish(plan, entry, 9))
                foreach (var item in room.Items)
                {
                    Rect aabb = RoomFurnishCore.AabbOf(item);
                    Rect r = plan.Rooms[room.RoomIndex];
                    Assert.GreaterOrEqual(aabb.xMin, r.xMin - 0.01f, item.Kind);
                    Assert.LessOrEqual(aabb.xMax, r.xMax + 0.01f, item.Kind);
                    Assert.GreaterOrEqual(aabb.yMin, r.yMin - 0.01f, item.Kind);
                    Assert.LessOrEqual(aabb.yMax, r.yMax + 0.01f, item.Kind);
                }
        }

        [Test]
        public void Items_NeverBlockACorridorMouth()
        {
            var plan = MakePlan(42, out var entry);
            foreach (var room in RoomFurnishCore.Furnish(plan, entry, 9))
                foreach (var item in room.Items)
                {
                    if (item.Flat) continue; // floor paint blocks nothing by contract
                    Rect aabb = RoomFurnishCore.AabbOf(item);
                    foreach (var c in plan.Corridors)
                        Assert.IsFalse(aabb.Overlaps(c),
                            item.Kind + " blocks a corridor in room " + room.RoomIndex);
                }
        }

        [Test]
        public void Items_NeverOverlapEachOther()
        {
            var plan = MakePlan(42, out var entry);
            foreach (var room in RoomFurnishCore.Furnish(plan, entry, 9))
                for (int i = 0; i < room.Items.Count; i++)
                    for (int j = i + 1; j < room.Items.Count; j++)
                        Assert.IsFalse(RoomFurnishCore.AabbOf(room.Items[i])
                                .Overlaps(RoomFurnishCore.AabbOf(room.Items[j])),
                            room.Items[i].Kind + " overlaps " + room.Items[j].Kind);
        }

        [Test]
        public void FloorBudget_IsRespected()
        {
            var plan = MakePlan(42, out var entry);
            foreach (var room in RoomFurnishCore.Furnish(plan, entry, 9))
            {
                Rect r = plan.Rooms[room.RoomIndex];
                float used = 0f;
                foreach (var item in room.Items) used += item.Size.x * item.Size.y;
                Assert.LessOrEqual(used, r.width * r.height * RoomFurnishCore.FloorBudget01 + 0.01f,
                    "room " + room.RoomIndex + " over floor budget");
            }
        }

        [Test]
        public void Roles_FoyerNearestEntry_CommonIsLargestRemaining()
        {
            var plan = ChainPlan();
            var entry = new Vector2(-0.2f, 1.5f); // west of room A
            plan.Rooms[1] = new Rect(5f, 0f, 4f, 4f); // make B the largest
            var roles = RoomFurnishCore.AssignRoles(plan, entry);
            Assert.AreEqual(RoomRole.Foyer, roles[0]);
            Assert.AreEqual(RoomRole.Common, roles[1]);
        }

        [Test]
        public void Visibility_RoomAt_FindsTheRightRoom()
        {
            var plan = ChainPlan();
            Assert.AreEqual(0, InteriorVisibilityCore.RoomIndexAt(plan, new Vector2(1.5f, 1.5f)));
            Assert.AreEqual(2, InteriorVisibilityCore.RoomIndexAt(plan, new Vector2(11.5f, 1.5f)));
            Assert.AreEqual(-1, InteriorVisibilityCore.RoomIndexAt(plan, new Vector2(4f, 1.4f))); // corridor
        }

        [Test]
        public void Visibility_SeesCorridorNeighbors_NeverTheFarRoom()
        {
            var plan = ChainPlan();
            var visible = new List<int>();
            InteriorVisibilityCore.VisibleRooms(plan, 0, visible);
            CollectionAssert.Contains(visible, 0);
            CollectionAssert.Contains(visible, 1);   // shares corridor A↔B
            CollectionAssert.DoesNotContain(visible, 2); // no shared corridor — walls between
        }

        [Test]
        public void Visibility_InCorridor_EverythingStaysVisible()
        {
            var plan = ChainPlan();
            var visible = new List<int>();
            InteriorVisibilityCore.VisibleRooms(plan, -1, visible);
            Assert.AreEqual(3, visible.Count);
        }
    }
}
