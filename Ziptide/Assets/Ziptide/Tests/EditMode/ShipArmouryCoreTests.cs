using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The departure gate sits between the player and the entire rest of the game, so the test that
    /// matters most here is not "does it block" — it is "can it ever refuse when the player has no
    /// way to comply". A gate that teaches is good design; a gate that strands is a dead save.
    /// </summary>
    public class ShipArmouryCoreTests
    {
        [Test]
        public void AHolsteredWeapon_OpensTheRamp()
        {
            Assert.AreEqual(DepartureVerdict.Armed,
                ShipArmouryCore.Evaluate(weaponsOnBelt: 1, weaponsInHands: 0, weaponsReachable: 7));
            Assert.IsFalse(ShipArmouryCore.Blocks(DepartureVerdict.Armed));
        }

        [Test]
        public void EmptyHanded_WithAStockedRack_IsHeldAndTold()
        {
            var v = ShipArmouryCore.Evaluate(weaponsOnBelt: 0, weaponsInHands: 0, weaponsReachable: 8);
            Assert.AreEqual(DepartureVerdict.HoldUnarmed, v);
            Assert.IsTrue(ShipArmouryCore.Blocks(v));
            Assert.AreEqual("ARM_YOURSELF", ShipArmouryCore.CueFor(v),
                "being blocked without being told what to do is the actual failure");
        }

        [Test]
        public void CarryingButNotBelted_GetsItsOwnLine()
        {
            // The specific thing a new VR player gets wrong: they pick the gun up, walk to the ramp,
            // and cannot understand why it will not open. "Arm yourself" is the wrong sentence here.
            var v = ShipArmouryCore.Evaluate(weaponsOnBelt: 0, weaponsInHands: 1, weaponsReachable: 7);
            Assert.AreEqual(DepartureVerdict.HoldInHandOnly, v);
            Assert.IsTrue(ShipArmouryCore.Blocks(v));
            Assert.AreEqual("BELT_IT", ShipArmouryCore.CueFor(v));
            Assert.AreNotEqual(ShipArmouryCore.CueFor(DepartureVerdict.HoldUnarmed),
                ShipArmouryCore.CueFor(DepartureVerdict.HoldInHandOnly));
        }

        [Test]
        public void TheNoTrapLaw_AnEmptyShipAlwaysOpens()
        {
            // THE test. If a spawn bug leaves the rack empty, the gate must not become a soft-lock
            // that ends the run. Pressure is allowed; traps are not.
            var v = ShipArmouryCore.Evaluate(weaponsOnBelt: 0, weaponsInHands: 0, weaponsReachable: 0);
            Assert.AreEqual(DepartureVerdict.ArmedNothingToTake, v);
            Assert.IsFalse(ShipArmouryCore.Blocks(v), "a gate the player cannot satisfy must open");
            Assert.IsEmpty(ShipArmouryCore.CueFor(v), "and it must not nag about a rack that is empty");
        }

        [Test]
        public void NoReachableWeapons_ButOneInHand_StillTeachesRatherThanOpening()
        {
            // Holding the last weapon in the world is compliable — belt it. This is a hold, not a trap,
            // and it is the boundary the no-trap law must NOT swallow.
            var v = ShipArmouryCore.Evaluate(weaponsOnBelt: 0, weaponsInHands: 1, weaponsReachable: 0);
            Assert.AreEqual(DepartureVerdict.HoldInHandOnly, v);
            Assert.IsTrue(ShipArmouryCore.Blocks(v));
        }

        [Test]
        public void TheRack_HoldsTheWholeAuthoredArsenal()
        {
            // Eight weapons are authored; four of them were unobtainable before this. The rack has a
            // home for every one, so a weapon added later never has to go back on the floor.
            Assert.AreEqual(8, ShipArmouryCore.RackSlots);
        }

        [Test]
        public void RackSlots_AreEvenlySpaced_AndCentred()
        {
            float first = ShipArmouryCore.SlotLocalX(0);
            float last = ShipArmouryCore.SlotLocalX(ShipArmouryCore.RackSlots - 1);
            Assert.AreEqual(-first, last, 0.0001f, "the rail is centred on its mount");
            for (int i = 1; i < ShipArmouryCore.RackSlots; i++)
                Assert.AreEqual(ShipArmouryCore.SlotSpacing,
                    ShipArmouryCore.SlotLocalX(i) - ShipArmouryCore.SlotLocalX(i - 1), 0.0001f);
        }

        [Test]
        public void TheRail_SitsInComfortableReach()
        {
            // Nothing on the rack may need a crouch or a stretch in a headset.
            Assert.Greater(ShipArmouryCore.RailHeight, 0.9f);
            Assert.Less(ShipArmouryCore.RailHeight, 1.5f);
        }
    }
}
