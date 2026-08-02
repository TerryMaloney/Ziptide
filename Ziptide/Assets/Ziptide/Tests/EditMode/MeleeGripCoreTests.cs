using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Terry, 2026-08-01: "the sword is still wrong holding position (this is like the tenth time)".
    /// It kept coming back because the authored raised-blade pose was reapplied in ItemFactory and
    /// then destroyed at runtime, and nothing anywhere asserted what "held correctly" MEANS. These
    /// tests are that assertion: the tip must rise above the hand, by a believable amount, always.
    /// </summary>
    public class MeleeGripCoreTests
    {
        [Test]
        public void ABladeRidesAboveTheGripAxis_NotAlongIt()
        {
            Vector3 blade = MeleeGripCore.BladeDirectionInHandSpace(MeleeGripCore.BladeRakeDegrees);

            Assert.Greater(blade.y, 0.5f,
                "the tip must rise clearly above the hand - a blade along the grip axis is an aimed barrel");
            Assert.Greater(blade.z, 0.5f,
                "it still points forward; this is a raised sword, not a raised flag");
            Assert.AreEqual(0f, blade.x, 1e-5f, "no sideways lean - the rake is pitch only");
        }

        [Test]
        public void TheShippedRakeIsInsideTheUsefulBand()
        {
            Assert.IsTrue(MeleeGripCore.IsUsefulBladeRake(MeleeGripCore.BladeRakeDegrees));
            Assert.IsFalse(MeleeGripCore.IsUsefulBladeRake(0f), "barrel-straight is the bug we shipped");
            Assert.IsFalse(MeleeGripCore.IsUsefulBladeRake(70f),
                "70 was the authored value in ItemFactory and reads as a flagpole");
        }

        [Test]
        public void AThrustWeaponStaysNearTheGripLine()
        {
            Assert.Less(MeleeGripCore.RakeFor(thrustWeapon: true), MeleeGripCore.RakeFor(false),
                "the pike's point has to LEAD; raking it up turns a thrust into a swing");
            Vector3 pike = MeleeGripCore.BladeDirectionInHandSpace(MeleeGripCore.ThrustRakeDegrees);
            Assert.Greater(pike.z, 0.9f, "a thrust weapon points where the hand points");
        }

        [Test]
        public void RakeSurvivesTheRoundTripThroughTheAttachPose()
        {
            // The whole mechanism: XRI aligns the attach transform to the hand, so the item's world
            // rotation is hand * inverse(attachLocal). Feed a weapon whose local axis is a plain +Z
            // and check the blade comes out of that arithmetic raked, not flat.
            Vector3 axis = Vector3.forward;
            Vector3 up = Vector3.up;
            Quaternion attachLocal = MeleeGripCore.GripLocalRotation(axis, up, MeleeGripCore.BladeRakeDegrees);

            Quaternion hand = Quaternion.identity;
            Quaternion itemWorld = hand * Quaternion.Inverse(attachLocal);
            Vector3 bladeInWorld = itemWorld * axis;

            Vector3 expected = MeleeGripCore.BladeDirectionInHandSpace(MeleeGripCore.BladeRakeDegrees);
            Assert.Less(Vector3.Angle(bladeInWorld, expected), 0.5f,
                "the attach pose must deliver the rake the core promises");
        }

        [Test]
        public void RakeIsMeasuredFromTheGripAxis()
        {
            Vector3 blade = MeleeGripCore.BladeDirectionInHandSpace(MeleeGripCore.BladeRakeDegrees);
            Assert.AreEqual(MeleeGripCore.BladeRakeDegrees, Vector3.Angle(Vector3.forward, blade), 0.01f);
        }

        [Test]
        public void TheFistDoesNotSitOnTheButtOfTheWeapon()
        {
            float halfLength = 0.31f;                       // the shipped breaker blade
            float grip = MeleeGripCore.GripOffsetAlongAxis(halfLength);

            Assert.Less(grip, 0f, "the hand is behind the model origin, toward the pommel");
            Assert.Greater(grip, -halfLength,
                "some haft must remain behind the fist or the hand looks welded to the butt");
            Assert.AreEqual(MeleeGripCore.PommelClearance, halfLength + grip, 1e-5f);
        }
    }
}
