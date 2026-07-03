using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// H2 RoomPartitioner contracts (ARCHITECTURE V2, PDF §BSP): determinism, containment,
    /// non-overlap, size laws, and above all THE ACCESS RULE — every plan is fully connected, every
    /// seed, every footprint. Pure, headless.
    /// </summary>
    public class RoomPartitionerTests
    {
        private static readonly Rect Deck = new Rect(0f, 0f, 24f, 16f);
        private const float Corridor = 1.4f;
        private const float MinArea = 9f;
        private const float MaxAspect = 2.6f;

        private static InteriorPlan Plan(int seed, Rect? footprint = null)
            => RoomPartitioner.Partition(footprint ?? Deck, Corridor, MinArea, MaxAspect, seed);

        [Test]
        public void SameSeed_IdenticalPlan()
        {
            var a = Plan(42);
            var b = Plan(42);
            Assert.AreEqual(a.Rooms.Count, b.Rooms.Count);
            Assert.AreEqual(a.Corridors.Count, b.Corridors.Count);
            for (int i = 0; i < a.Rooms.Count; i++) Assert.AreEqual(a.Rooms[i], b.Rooms[i]);
        }

        [Test]
        public void DifferentSeeds_ProduceDifferentPlans()
        {
            var a = Plan(1);
            var b = Plan(2);
            bool differ = a.Rooms.Count != b.Rooms.Count;
            if (!differ)
                for (int i = 0; i < a.Rooms.Count && !differ; i++)
                    differ = a.Rooms[i] != b.Rooms[i];
            Assert.IsTrue(differ, "two seeds produced identical interiors");
        }

        [Test]
        public void Rooms_StayInsideTheFootprint()
        {
            var plan = Plan(7);
            foreach (var room in plan.Rooms)
            {
                Assert.GreaterOrEqual(room.xMin, Deck.xMin - 0.001f);
                Assert.GreaterOrEqual(room.yMin, Deck.yMin - 0.001f);
                Assert.LessOrEqual(room.xMax, Deck.xMax + 0.001f);
                Assert.LessOrEqual(room.yMax, Deck.yMax + 0.001f);
            }
        }

        [Test]
        public void Rooms_NeverOverlapEachOther()
        {
            var plan = Plan(9);
            for (int i = 0; i < plan.Rooms.Count; i++)
                for (int j = i + 1; j < plan.Rooms.Count; j++)
                    Assert.IsFalse(plan.Rooms[i].Overlaps(plan.Rooms[j]),
                        "rooms " + i + " and " + j + " overlap");
        }

        [Test]
        public void Rooms_RespectMinimumSide()
        {
            var plan = Plan(11);
            foreach (var room in plan.Rooms)
            {
                Assert.GreaterOrEqual(room.width, RoomPartitioner.MinSide - 0.001f);
                Assert.GreaterOrEqual(room.height, RoomPartitioner.MinSide - 0.001f);
            }
        }

        [Test]
        public void AspectLaw_HoldsForSplittableRooms()
        {
            var plan = Plan(13);
            foreach (var room in plan.Rooms)
            {
                float aspect = room.width > room.height ? room.width / room.height : room.height / room.width;
                float leafArea = (room.width + 2f * RoomPartitioner.WallInset)
                               * (room.height + 2f * RoomPartitioner.WallInset);
                if (leafArea >= 2f * MinArea)
                    Assert.LessOrEqual(aspect, MaxAspect + 0.6f,
                        "a comfortably-splittable room is a sliver: " + room);
            }
        }

        [Test]
        public void TheAccessRule_EveryPlanFullyConnected_25SeedSweep()
        {
            for (int seed = 1; seed <= 25; seed++)
            {
                var plan = Plan(seed);
                Assert.Greater(plan.Rooms.Count, 1, "seed " + seed + " made a single room from a whole deck");
                Assert.IsTrue(RoomPartitioner.IsFullyConnected(plan),
                    "seed " + seed + " produced an orphan room — the access rule is broken");
            }
        }

        [Test]
        public void TheAccessRule_HoldsForOddFootprints()
        {
            var shapes = new[]
            {
                new Rect(0f, 0f, 40f, 6f),    // long thin hull deck
                new Rect(-10f, -10f, 9f, 30f),// tall tower floor
                new Rect(5f, 5f, 60f, 60f),   // big hive floor
            };
            foreach (var shape in shapes)
                for (int seed = 1; seed <= 5; seed++)
                    Assert.IsTrue(RoomPartitioner.IsFullyConnected(
                        RoomPartitioner.Partition(shape, Corridor, MinArea, MaxAspect, seed)),
                        "footprint " + shape + " seed " + seed + " not connected");
        }

        [Test]
        public void Corridors_HonorTheRequestedWidth()
        {
            var plan = Plan(17);
            foreach (var c in plan.Corridors)
                Assert.GreaterOrEqual(Mathf.Min(c.width, c.height), Corridor - 0.001f,
                    "corridor thinner than requested: " + c);
        }

        [Test]
        public void TinyFootprint_YieldsOneRoomNoCorridors()
        {
            var plan = RoomPartitioner.Partition(new Rect(0f, 0f, 4f, 3.5f), Corridor, MinArea, MaxAspect, 3);
            Assert.AreEqual(1, plan.Rooms.Count);
            Assert.AreEqual(0, plan.Corridors.Count);
        }

        [Test]
        public void DegenerateFootprint_ReturnsEmptyPlanSafely()
        {
            Assert.IsTrue(RoomPartitioner.Partition(new Rect(0f, 0f, 1f, 1f), Corridor, MinArea, MaxAspect, 3).IsEmpty);
            Assert.IsTrue(RoomPartitioner.Partition(new Rect(0f, 0f, 0f, 10f), Corridor, MinArea, MaxAspect, 3).IsEmpty);
            Assert.IsTrue(RoomPartitioner.IsFullyConnected(default(InteriorPlan)), "empty plan should count as connected");
        }

        [Test]
        public void RoomCount_GrowsWithFootprintArea()
        {
            int small = Plan(21, new Rect(0f, 0f, 12f, 10f)).Rooms.Count;
            int large = Plan(21, new Rect(0f, 0f, 48f, 40f)).Rooms.Count;
            Assert.Greater(large, small, "a 16x larger floor did not gain rooms");
        }
    }
}
