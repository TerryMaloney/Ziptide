using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ⚖ Terry: *"we don't obviously need to make every single building playable all the way through …
    /// the first and last level will probably have the most interiors."* The rationing IS the design,
    /// and these pin it, because both ways of getting it wrong are expensive and neither shows up in a
    /// diff: a city that opens every door blows the Quest's frame budget, and a city that opens none
    /// is a film set.
    /// </summary>
    public class InteriorTierCoreTests
    {
        private const float BigEnough = 40f;   // comfortably over PartitionArea
        private const float Standable = 20f;   // over MinInteriorArea, under PartitionArea

        [Test]
        public void ARouteBuilding_IsAlwaysEnterable_WhateverTheBudgetSays()
        {
            // The lesson of MISS_LEDGER #21, as a rule: a contract you cannot walk into is a broken
            // level. Budget pressure must never be able to seal Dispatch.
            Assert.AreEqual(InteriorTier.Full,
                InteriorTierCore.Evaluate(BigEnough, servesRoute: true, vestibulesAlreadySpent: 999));
        }

        [Test]
        public void OrdinaryBuildings_BuyVestibules_UntilTheBudgetRunsOut()
        {
            for (int spent = 0; spent < InteriorTierCore.VestibuleBudgetPerDistrict; spent++)
                Assert.AreEqual(InteriorTier.Vestibule,
                    InteriorTierCore.Evaluate(BigEnough, servesRoute: false, vestibulesAlreadySpent: spent),
                    "spent=" + spent);

            Assert.AreEqual(InteriorTier.Facade,
                InteriorTierCore.Evaluate(BigEnough, servesRoute: false,
                    vestibulesAlreadySpent: InteriorTierCore.VestibuleBudgetPerDistrict),
                "past the budget the honest answer is a sealed door");
        }

        [Test]
        public void MostOfACity_IsFacade_AndThatIsCorrect()
        {
            // Sanity on the ration itself: a district of twenty buildings opens a handful, not all.
            int enterable = 0, spent = 0;
            for (int i = 0; i < 20; i++)
            {
                var tier = InteriorTierCore.Evaluate(BigEnough, servesRoute: false, spent);
                if (tier != InteriorTier.Facade) { enterable++; spent++; }
            }
            Assert.AreEqual(InteriorTierCore.VestibuleBudgetPerDistrict, enterable);
            Assert.Less(enterable, 20, "a city where every door opens is not a city, it is a cost");
        }

        [Test]
        public void ARoomTooSmallToStandIn_IsSealed_EvenOnTheRoute()
        {
            // An interior the player cannot fit inside is worse than an honest sealed door — the
            // LAYOUT is what should change, and a silent tiny room hides that.
            Assert.AreEqual(InteriorTier.Facade,
                InteriorTierCore.Evaluate(InteriorTierCore.MinInteriorArea - 0.1f,
                    servesRoute: true, vestibulesAlreadySpent: 0));
        }

        [Test]
        public void OnlyBigInteriors_AreCutIntoRooms()
        {
            // Partitioning a small footprint produces corridors nobody can walk down.
            Assert.IsFalse(InteriorTierCore.ShouldPartition(Standable));
            Assert.IsTrue(InteriorTierCore.ShouldPartition(BigEnough));
            Assert.IsTrue(InteriorTierCore.ShouldPartition(InteriorTierCore.PartitionArea));
        }

        [Test]
        public void TheUsableFloor_SitsInsideTheShell()
        {
            // Furniture must never intersect the walls the shell already drew.
            var footprint = new Vector2(7f, 7f);
            Rect floor = InteriorTierCore.UsableFloor(footprint);

            Assert.Less(floor.width, footprint.x);
            Assert.Less(floor.height, footprint.y);
            Assert.AreEqual(footprint.x - InteriorTierCore.ShellInset * 2f, floor.width, 0.001f);
            Assert.AreEqual(0f, floor.center.x, 0.001f, "the floor stays centred on the building");
            Assert.AreEqual(0f, floor.center.y, 0.001f);
        }

        [Test]
        public void ATinyFootprint_NeverProducesANegativeFloor()
        {
            // A building narrower than two insets would otherwise invert the rect and hand the
            // partitioner nonsense.
            Rect floor = InteriorTierCore.UsableFloor(new Vector2(0.2f, 0.2f));
            Assert.GreaterOrEqual(floor.width, 0f);
            Assert.GreaterOrEqual(floor.height, 0f);
        }

        [Test]
        public void AreaIgnoresSignedFootprints()
        {
            Assert.AreEqual(InteriorTierCore.AreaOf(new Vector2(6f, 5f)),
                InteriorTierCore.AreaOf(new Vector2(-6f, -5f)), 0.001f);
        }

        [Test]
        public void TheThreeToxicCityHeroBuildings_AllQualify()
        {
            // Shipyard Office is 7x7 in the committed layout; Dispatch and the Relay Vault are its
            // peers. All three serve the route, so all three must come back Full — this is the test
            // that would have caught the shipped state, where every one of them was an empty box.
            float area = InteriorTierCore.AreaOf(new Vector2(7f, 7f));
            Assert.AreEqual(InteriorTier.Full,
                InteriorTierCore.Evaluate(area, servesRoute: true, vestibulesAlreadySpent: 0));
            Assert.IsTrue(InteriorTierCore.ShouldPartition(area),
                "7x7 is worth cutting into rooms rather than one bare hall");
        }
    }
}
