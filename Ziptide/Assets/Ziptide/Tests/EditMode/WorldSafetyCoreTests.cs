using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class WorldSafetyCoreTests
    {
        [Test]
        public void SupportedPose_BecomesRecoveryAnchor()
        {
            var core = new WorldSafetyCore();
            Vector3 safe = new Vector3(3f, 2f, -4f);
            Quaternion rotation = Quaternion.Euler(0f, 65f, 0f);

            WorldSafetyDecision decision = core.Tick(safe, rotation, true, 0.02f, -5f);

            Assert.That(decision.ShouldRecover, Is.False);
            Assert.That(core.HasSafePose, Is.True);
            Assert.That(core.LastSafePosition, Is.EqualTo(safe));
            Assert.That(Quaternion.Angle(core.LastSafeRotation, rotation), Is.LessThan(0.01f));
        }

        [Test]
        public void BriefUnsupportedJump_DoesNotRecover()
        {
            var core = new WorldSafetyCore(0.55f, 0.85f);
            core.ObserveSupported(Vector3.zero, Quaternion.identity);

            WorldSafetyDecision decision = core.Tick(new Vector3(0f, 0.35f, 0f),
                Quaternion.identity, false, 0.30f, -3f);

            Assert.That(decision.ShouldRecover, Is.False);
            Assert.That(core.UnsupportedSeconds, Is.EqualTo(0.30f).Within(0.001f));
        }

        [Test]
        public void SustainedGapDrop_RecoversToLastSupportedPose()
        {
            var core = new WorldSafetyCore(0.50f, 0.75f);
            Vector3 safe = new Vector3(5f, 1.2f, 6f);
            core.ObserveSupported(safe, Quaternion.Euler(0f, 90f, 0f));

            core.Tick(new Vector3(5f, 0.8f, 6f), Quaternion.identity, false, 0.30f, -4f);
            WorldSafetyDecision decision = core.Tick(new Vector3(5f, 0.2f, 6f),
                Quaternion.identity, false, 0.25f, -4f);

            Assert.That(decision.ShouldRecover, Is.True);
            Assert.That(decision.Reason, Is.EqualTo("unsupported_gap"));
            Assert.That(decision.HasSafePose, Is.True);
            Assert.That(decision.SafePosition, Is.EqualTo(safe));
        }

        [Test]
        public void HardFallFloor_RecoversImmediately()
        {
            var core = new WorldSafetyCore(5f, 10f);
            core.ObserveSupported(new Vector3(0f, 1f, 0f), Quaternion.identity);

            WorldSafetyDecision decision = core.Tick(new Vector3(0f, -3.1f, 0f),
                Quaternion.identity, false, 0.01f, -3f);

            Assert.That(decision.ShouldRecover, Is.True);
            Assert.That(decision.Reason, Is.EqualTo("hard_floor"));
        }

        [Test]
        public void SupportedHazardBottom_DoesNotTriggerGapRecovery()
        {
            var core = new WorldSafetyCore(0.10f, 0.10f);
            core.ObserveSupported(new Vector3(0f, 0f, 0f), Quaternion.identity);

            WorldSafetyDecision decision = core.Tick(new Vector3(0f, -2f, 0f),
                Quaternion.identity, true, 2f, -5f);

            Assert.That(decision.ShouldRecover, Is.False,
                "An authored river bottom is supported space; ToxicRiverRuntime owns the consequence.");
            Assert.That(core.LastSafePosition.y, Is.EqualTo(-2f));
        }

        [Test]
        public void ResetAfterRecovery_PreventsRepeatedRecovery()
        {
            var core = new WorldSafetyCore(0.10f, 0.10f);
            core.ObserveSupported(Vector3.zero, Quaternion.identity);
            Assert.That(core.Tick(Vector3.down, Quaternion.identity, false, 0.2f, -5f).ShouldRecover, Is.True);

            core.ResetAfterRecovery(Vector3.zero, Quaternion.identity);
            WorldSafetyDecision decision = core.Tick(Vector3.zero, Quaternion.identity, true, 0.02f, -5f);

            Assert.That(decision.ShouldRecover, Is.False);
            Assert.That(core.UnsupportedSeconds, Is.Zero);
        }
    }
}
