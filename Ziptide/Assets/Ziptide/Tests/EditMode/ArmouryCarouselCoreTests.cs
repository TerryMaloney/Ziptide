using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ⚖ Terry: the rack should rotate so a growing arsenal still fits a small ship, and *"once you
    /// have it then you have it"*. These pin the two things that make that work and cannot be checked
    /// in a headset: that the drum can never overlap itself, and that earning a weapon never moves the
    /// ones you already own.
    /// </summary>
    public class ArmouryCarouselCoreTests
    {
        private static PlayerProfile Profile(params string[] ownedIds)
        {
            var p = new PlayerProfile();
            foreach (string id in ownedIds) WeaponOwnership.Grant(p, id);
            return p;
        }

        // ── the drum ───────────────────────────────────────────────────────────────────

        [Test]
        public void TheDrum_HoldsTheWholeArsenal()
        {
            Assert.AreEqual(WeaponCatalog.WeaponIds.Count, ArmouryCarouselCore.Capacity,
                "a weapon with no detent has nowhere to live");
        }

        [Test]
        public void TheDrum_NeverOverlapsItself()
        {
            // The failure this exists to prevent: a diameter tuned for looks that quietly puts two
            // weapons in the same arc, which reads on device as clipping guns and costs a session.
            Assert.GreaterOrEqual(ArmouryCarouselCore.FaceArcMetres, ShipArmouryCore.SlotSpacing,
                "arc between faces must clear the slot spacing the concept was drawn to");
        }

        [Test]
        public void TheDrum_IsSmallerThanTheWallRailItReplaces()
        {
            // The entire point of spinning it. The rail spans 8 x 0.34 m of bulkhead; the drum spans
            // its diameter, whatever the arsenal grows to.
            float railSpan = ShipArmouryCore.RackSlots * ShipArmouryCore.SlotSpacing;
            Assert.Less(ArmouryCarouselCore.Diameter, railSpan * 0.25f,
                "a drum that is not dramatically smaller is not worth the mechanism");
        }

        [Test]
        public void EverySlot_LandsOnExactlyOneRingAndFace()
        {
            var seen = new HashSet<string>();
            for (int i = 0; i < ArmouryCarouselCore.Capacity; i++)
            {
                Assert.IsTrue(seen.Add(ArmouryCarouselCore.RingOf(i) + ":" + ArmouryCarouselCore.FaceOf(i)),
                    "slot " + i + " collides with another slot");
                Assert.Less(ArmouryCarouselCore.RingOf(i), ArmouryCarouselCore.Rings);
                Assert.Less(ArmouryCarouselCore.FaceOf(i), ArmouryCarouselCore.FacesPerRing);
            }
        }

        [Test]
        public void BothRings_SitInComfortableReach()
        {
            for (int i = 0; i < ArmouryCarouselCore.Capacity; i++)
            {
                float h = ArmouryCarouselCore.HeightOf(i);
                Assert.Greater(h, 0.7f, "nothing on the drum needs a crouch");
                Assert.Less(h, 1.5f, "…and nothing needs a stretch");
            }
        }

        [Test]
        public void RotatingToASlot_PresentsThatSlotsFace()
        {
            for (int i = 0; i < ArmouryCarouselCore.Capacity; i++)
            {
                float angle = ArmouryCarouselCore.AngleForSlot(i);
                Assert.AreEqual(ArmouryCarouselCore.FaceOf(i), ArmouryCarouselCore.FaceAtFront(angle),
                    "asking for slot " + i + " must actually present it");
            }
        }

        [Test]
        public void TheDrum_AlwaysSettlesSquare()
        {
            foreach (float messy in new[] { 7f, 44f, 91f, 179f, 271f, 359f, -13f })
            {
                float snapped = ArmouryCarouselCore.SnapToDetent(messy);
                Assert.AreEqual(0f, snapped % ArmouryCarouselCore.StepDegrees, 0.001f,
                    "a drum resting between weapons is a drum you cannot grab from");
            }
        }

        [Test]
        public void TheDrum_TakesTheShortWayRound()
        {
            // One click backwards must not spin 270 degrees forwards — in VR that is motion the player
            // did not ask for, right in front of their face.
            Assert.AreEqual(-90f, ArmouryCarouselCore.ShortestDelta(0f, 270f), 0.001f);
            Assert.AreEqual(90f, ArmouryCarouselCore.ShortestDelta(270f, 0f), 0.001f);
            Assert.AreEqual(0f, ArmouryCarouselCore.ShortestDelta(45f, 45f), 0.001f);
            for (float a = -720f; a <= 720f; a += 37f)
                Assert.LessOrEqual(System.Math.Abs(ArmouryCarouselCore.ShortestDelta(a, a + 200f)), 180.001f);
        }

        // ── ownership ──────────────────────────────────────────────────────────────────

        [Test]
        public void OnceYouHaveIt_YouHaveIt()
        {
            var p = Profile();
            Assert.IsFalse(WeaponOwnership.Owns(p, "prism_beam"));
            Assert.IsTrue(WeaponOwnership.Grant(p, "prism_beam"), "first time is the discovery");
            Assert.IsTrue(WeaponOwnership.Owns(p, "prism_beam"));
            Assert.IsFalse(WeaponOwnership.Grant(p, "prism_beam"),
                "picking the same gun up twice is not a discovery");
        }

        [Test]
        public void OwnershipRidesThePersistedFlags_NotANewSaveField()
        {
            // SaveSystem is a protected owner; a schema change to store what the flag set already
            // expresses would be migration risk bought for nothing.
            var p = Profile("tide_pike");
            Assert.IsTrue(p.HasFlag(WeaponOwnership.OwnedFlag("tide_pike")));
        }

        [Test]
        public void EarningAWeapon_NeverMovesTheOnesYouAlreadyHad()
        {
            // THE test. Muscle memory is the whole reason the drum uses catalog order rather than
            // acquisition order — "my pistol is two clicks left" must stay true for the campaign.
            var p = Profile("pistol", "tide_pike");
            int pistolBefore = WeaponOwnership.SlotIndexOf("pistol");
            int pikeBefore = WeaponOwnership.SlotIndexOf("tide_pike");

            WeaponOwnership.Grant(p, "static_net");
            WeaponOwnership.Grant(p, "prism_beam");

            Assert.AreEqual(pistolBefore, WeaponOwnership.SlotIndexOf("pistol"));
            Assert.AreEqual(pikeBefore, WeaponOwnership.SlotIndexOf("tide_pike"));
        }

        [Test]
        public void OwnedList_IsCatalogOrder_NotAcquisitionOrder()
        {
            var p = Profile("tide_pike", "pistol");
            List<string> owned = WeaponOwnership.OwnedIds(p);
            Assert.AreEqual(2, owned.Count);
            Assert.Less(WeaponOwnership.SlotIndexOf(owned[0]), WeaponOwnership.SlotIndexOf(owned[1]),
                "the drum reads in catalog order however the player found them");
        }

        [Test]
        public void EverySlotIndex_IsAValidDetent()
        {
            foreach (string id in WeaponCatalog.WeaponIds)
            {
                int slot = WeaponOwnership.SlotIndexOf(id);
                Assert.GreaterOrEqual(slot, 0, id + " has no detent");
                Assert.Less(slot, ArmouryCarouselCore.Capacity, id + " falls off the drum");
            }
        }

        [Test]
        public void NonWeapons_AreNeverOwned_AndCannotBeGranted()
        {
            var p = Profile();
            Assert.IsFalse(WeaponOwnership.Grant(p, "handheld_camera"), "a tool is not an armoury slot");
            Assert.IsFalse(WeaponOwnership.Grant(p, "artifact_half_a"));
            Assert.IsFalse(WeaponOwnership.Owns(p, "relay_cell"));
            Assert.IsEmpty(WeaponOwnership.OwnedIds(p));
        }

        [Test]
        public void ANullProfile_IsSafe_NotACrashOnTheHatch()
        {
            Assert.IsFalse(WeaponOwnership.Owns(null, "pistol"));
            Assert.IsFalse(WeaponOwnership.Grant(null, "pistol"));
            Assert.IsEmpty(WeaponOwnership.OwnedIds(null));
        }
    }
}
