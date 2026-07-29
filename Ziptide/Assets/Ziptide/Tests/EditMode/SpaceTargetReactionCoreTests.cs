using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Terry asked how the space droids act and react; the honest answer was "they bob." These
    /// tests pin the reaction rules that answer it: wake with a hysteresis band (no mood strobing
    /// at the boundary — that would be a light flicker in a headset), an evade window that
    /// outranks waking, a slide that starts and ends at home, and the non-lethal law that a
    /// disabled drone reacts to nothing at all.
    /// </summary>
    public sealed class SpaceTargetReactionCoreTests
    {
        [Test]
        public void DormantDrone_WakesOnlyInsideWakeRadius()
        {
            Assert.AreEqual(SpaceTargetMood.Dormant, SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Dormant, SpaceTargetReactionCore.WakeRadius + 1f, -1f, false));
            Assert.AreEqual(SpaceTargetMood.Woken, SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Dormant, SpaceTargetReactionCore.WakeRadius - 1f, -1f, false));
        }

        [Test]
        public void WokenDrone_StaysAwakeThroughTheHysteresisBand()
        {
            // Between wake and sleep radius an already-woken drone must NOT drop back to dormant,
            // or a pilot hovering at 45 m would see the eye flicker on and off every frame.
            float between = (SpaceTargetReactionCore.WakeRadius + SpaceTargetReactionCore.SleepRadius) * 0.5f;
            Assert.AreEqual(SpaceTargetMood.Woken, SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Woken, between, -1f, false));
            Assert.AreEqual(SpaceTargetMood.Dormant, SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Woken, SpaceTargetReactionCore.SleepRadius + 1f, -1f, false));
        }

        [Test]
        public void RecentHit_EvadesEvenFromFarAway()
        {
            Assert.AreEqual(SpaceTargetMood.Evading, SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Dormant, 500f, 0.2f, false));
        }

        [Test]
        public void EvadeWindow_ExpiresBackToWokenWhenThePilotIsNear()
        {
            var after = SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Evading, 10f, SpaceTargetReactionCore.EvadeSeconds + 0.1f, false);
            Assert.AreEqual(SpaceTargetMood.Woken, after);
        }

        [Test]
        public void DisabledDrone_HasNoMood()
        {
            Assert.AreEqual(SpaceTargetMood.Dormant, SpaceTargetReactionCore.Classify(
                SpaceTargetMood.Evading, 1f, 0.1f, true));
        }

        [Test]
        public void StrafeOffset_IsZeroUnlessEvading()
        {
            Assert.AreEqual(0f, SpaceTargetReactionCore.StrafeOffset(SpaceTargetMood.Dormant, 3f, 0.4f));
            Assert.AreEqual(0f, SpaceTargetReactionCore.StrafeOffset(SpaceTargetMood.Woken, 3f, 0.4f));
        }

        [Test]
        public void StrafeOffset_StaysInsideItsAmplitude()
        {
            for (float t = 0f; t < 12f; t += 0.07f)
            {
                float o = SpaceTargetReactionCore.StrafeOffset(SpaceTargetMood.Evading, t, 1.1f);
                Assert.LessOrEqual(Mathf.Abs(o), SpaceTargetReactionCore.StrafeAmplitude + 0.001f);
            }
        }

        [Test]
        public void StrafeOffset_ActuallyTravels()
        {
            float quarter = 0.25f / SpaceTargetReactionCore.StrafeCyclesPerSecond;
            float a = SpaceTargetReactionCore.StrafeOffset(SpaceTargetMood.Evading, 0f, 0f);
            float b = SpaceTargetReactionCore.StrafeOffset(SpaceTargetMood.Evading, quarter, 0f);
            Assert.Greater(Mathf.Abs(b - a), 1f, "an evading drone that doesn't move isn't evading");
        }

        [Test]
        public void BobAndEye_EscalateWithTheMood()
        {
            Assert.AreEqual(1f, SpaceTargetReactionCore.BobMultiplier(SpaceTargetMood.Dormant));
            Assert.AreEqual(SpaceTargetReactionCore.WokenBobMultiplier,
                SpaceTargetReactionCore.BobMultiplier(SpaceTargetMood.Woken));
            Assert.Less(SpaceTargetReactionCore.EyeIntensity(SpaceTargetMood.Dormant),
                SpaceTargetReactionCore.EyeIntensity(SpaceTargetMood.Woken));
            Assert.Less(SpaceTargetReactionCore.EyeIntensity(SpaceTargetMood.Woken),
                SpaceTargetReactionCore.EyeIntensity(SpaceTargetMood.Evading));
        }

        [Test]
        public void WakeBandIsOrdered()
        {
            Assert.Less(SpaceTargetReactionCore.WakeRadius, SpaceTargetReactionCore.SleepRadius,
                "sleep radius must sit outside wake radius or the hysteresis inverts");
        }

        [Test]
        public void DriftTumble_IsSlowAndPerObject()
        {
            var a = DriftTumbleRuntime.RateFor(new Vector3(3f, 1f, 7f));
            var b = DriftTumbleRuntime.RateFor(new Vector3(-4f, 2f, 11f));
            Assert.AreNotEqual(a, b, "a debris field must not turn in unison");
            foreach (float axis in new[] { a.x, a.y, a.z, b.x, b.y, b.z })
                Assert.LessOrEqual(Mathf.Abs(axis), DriftTumbleRuntime.MaxRateDegrees + 0.001f);
        }

        [Test]
        public void DriftTumble_IsDeterministic()
        {
            Assert.AreEqual(DriftTumbleRuntime.RateFor(new Vector3(3f, 1f, 7f)),
                DriftTumbleRuntime.RateFor(new Vector3(3f, 1f, 7f)));
        }
    }
}
