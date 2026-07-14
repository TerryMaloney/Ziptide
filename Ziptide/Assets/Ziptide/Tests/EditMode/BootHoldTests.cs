using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// DS-01 (DEVICE_STABILIZATION_FORENSIC_PLAN) — the cold-boot hold contract, pure seam.
    /// The device bug: the Home Hub waits in a floorless `_Boot` while gravity locomotion + the
    /// fall net stay live → fall/respawn loop. These tests pin the state machine that prevents it:
    /// held ⇒ fall checks suppressed + drift re-pinned; hold/release are one-shot idempotent.
    /// </summary>
    public class BootHoldTests
    {
        [Test]
        public void Begin_Holds_StoresPose_AndIsIdempotent()
        {
            var s = new BootHoldState();
            var pose = new Vector3(1f, 2f, 3f);

            Assert.IsTrue(s.Begin(pose), "first Begin enters the hold");
            Assert.IsTrue(s.Held);
            Assert.AreEqual(pose, s.HoldPose);

            Assert.IsFalse(s.Begin(new Vector3(9f, 9f, 9f)), "second Begin is a no-op");
            Assert.AreEqual(pose, s.HoldPose, "the original boot pose is preserved");
        }

        [Test]
        public void End_ReleasesOnce_ThenNoOps()
        {
            var s = new BootHoldState();
            Assert.IsFalse(s.End(), "End before Begin is a no-op (no double provider-restore)");

            s.Begin(Vector3.zero);
            Assert.IsTrue(s.End(), "first End releases");
            Assert.IsFalse(s.Held);
            Assert.IsFalse(s.End(), "second End is a no-op");
        }

        [Test]
        public void FallCheck_SuppressedOnlyWhileHeld()
        {
            var s = new BootHoldState();
            Assert.IsFalse(s.SuppressesFallCheck, "fall net normal before boot hold");

            s.Begin(Vector3.zero);
            Assert.IsTrue(s.SuppressesFallCheck, "fall net DISARMED while the menu owns _Boot");

            s.End();
            Assert.IsFalse(s.SuppressesFallCheck, "fall net re-armed after release");
        }

        [Test]
        public void Repin_FiresOnDriftBeyondTolerance_OnlyWhileHeld()
        {
            var s = new BootHoldState();
            var pose = new Vector3(0f, 1.6f, 0f);

            Assert.IsFalse(s.NeedsRepin(pose + Vector3.down * 10f, 0.05f),
                "no re-pin when not held");

            s.Begin(pose);
            Assert.IsFalse(s.NeedsRepin(pose + Vector3.up * 0.01f, 0.05f),
                "tracking jitter inside tolerance is left alone");
            Assert.IsTrue(s.NeedsRepin(pose + Vector3.down * 0.5f, 0.05f),
                "a gravity tick's drop gets pinned back — the loop cannot start");

            s.End();
            Assert.IsFalse(s.NeedsRepin(pose + Vector3.down * 0.5f, 0.05f),
                "after release the world owns the rig again");
        }

        [Test]
        public void HoldReleaseHold_SecondBootCycleWorks()
        {
            // A full app relaunch isn't the only path: warp back to _Boot must be holdable again.
            var s = new BootHoldState();
            s.Begin(Vector3.zero);
            s.End();
            Assert.IsTrue(s.Begin(new Vector3(0f, 5f, 0f)), "a fresh hold after release re-enters");
            Assert.AreEqual(new Vector3(0f, 5f, 0f), s.HoldPose);
        }
    }
}
