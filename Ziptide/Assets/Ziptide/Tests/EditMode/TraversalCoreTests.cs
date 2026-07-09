using NUnit.Framework;
using Ziptide.Content.Traversal;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Phase 1.4 traversal kinematics — the zipline ride + VR hand-over-hand climb. Pure,
    /// headless; the comfort caps are asserted here because they're the law, not a tuning knob.</summary>
    public class TraversalCoreTests
    {
        // ── Zipline ──────────────────────────────────────────────────────────
        [Test]
        public void Zipline_DownhillRide_ArrivesAtB()
        {
            var ride = new ZiplineRide(new TVec3(0, 10, 0), new TVec3(10, 0, 0));
            int guard = 0;
            while (!ride.Arrived && guard++ < 10000) ride.Step(0.05f);
            Assert.IsTrue(ride.Arrived, "a downhill ride must complete");
            var p = ride.Position;
            Assert.AreEqual(10f, p.X, 1e-3f);
            Assert.AreEqual(0f, p.Y, 1e-3f);
        }

        [Test]
        public void Zipline_ProgressIsMonotonic_AndStartsAtA()
        {
            var ride = new ZiplineRide(new TVec3(0, 10, 0), new TVec3(20, 2, 0));
            Assert.AreEqual(0f, ride.Progress, 1e-6f);
            Assert.AreEqual(0f, ride.Position.X, 1e-4f, "position at progress 0 is A");

            float prev = 0f;
            for (int i = 0; i < 200 && !ride.Arrived; i++)
            {
                ride.Step(0.05f);
                Assert.GreaterOrEqual(ride.Progress, prev, "progress never goes backwards (no stall)");
                prev = ride.Progress;
            }
        }

        [Test]
        public void Zipline_NeverExceedsTheComfortSpeedCap()
        {
            // A steep drop that would accelerate well past the cap without clamping.
            var ride = new ZiplineRide(new TVec3(0, 100, 0), new TVec3(5, 0, 0), maxSpeed: 8f);
            for (int i = 0; i < 300 && !ride.Arrived; i++)
            {
                ride.Step(0.1f);
                Assert.LessOrEqual(ride.Speed, ride.MaxSpeed + 1e-4f, "the comfort cap is a hard ceiling");
            }
        }

        [Test]
        public void Zipline_FlatLine_StillMoves_ViaThePushOff()
        {
            var ride = new ZiplineRide(new TVec3(0, 5, 0), new TVec3(10, 5, 0), kickSpeed: 2f);
            Assert.GreaterOrEqual(ride.Speed, 2f, "you launch with a push-off, never a dead stop");
            int guard = 0;
            while (!ride.Arrived && guard++ < 10000) ride.Step(0.05f);
            Assert.IsTrue(ride.Arrived, "a flat line still delivers you across");
        }

        // ── Climb ────────────────────────────────────────────────────────────
        [Test]
        public void Climb_PullingAHandDown_MovesTheRigUp()
        {
            var climb = new ClimbGrip();
            climb.Grip(Hand.Left, new TVec3(0, 0, 0));
            var rigDelta = climb.MoveGrip(Hand.Left, new TVec3(0, -0.3f, 0)); // pull hand down
            Assert.AreEqual(0f, rigDelta.X, 1e-5f);
            Assert.AreEqual(0.3f, rigDelta.Y, 1e-5f, "rig climbs UP as the gripping hand pulls DOWN");
            Assert.IsTrue(climb.IsClimbing);
        }

        [Test]
        public void Climb_HandOff_HasNoTeleport()
        {
            var climb = new ClimbGrip();
            climb.Grip(Hand.Left, new TVec3(0, 0, 0));
            climb.MoveGrip(Hand.Left, new TVec3(0, -0.3f, 0));      // pull up on left
            climb.Grip(Hand.Right, new TVec3(0.2f, -0.3f, 0));      // reach up with right (becomes driver)
            var onRelease = climb.Release(Hand.Left, new TVec3(0, 0, 0), maxFling: 5f);
            Assert.AreEqual(0f, onRelease.X + onRelease.Y + onRelease.Z, 1e-5f, "handoff to the other hand = no jump");

            var rigDelta = climb.MoveGrip(Hand.Right, new TVec3(0.2f, -0.6f, 0)); // continue on right
            Assert.AreEqual(0.3f, rigDelta.Y, 1e-5f, "climb continues seamlessly on the new hand");
        }

        [Test]
        public void Climb_NonDriverHandMove_DoesNotMoveTheRig()
        {
            var climb = new ClimbGrip();
            climb.Grip(Hand.Left, new TVec3(0, 0, 0));              // driver = left
            var rigDelta = climb.MoveGrip(Hand.Right, new TVec3(1, 1, 1)); // right isn't gripping/driving
            Assert.AreEqual(0f, rigDelta.Length, 1e-5f, "a non-driving hand just tracks; it can't move you");
        }

        [Test]
        public void Climb_ReleasingTheLastHand_ClampsTheFling()
        {
            var climb = new ClimbGrip();
            climb.Grip(Hand.Left, new TVec3(0, 0, 0));
            var fling = climb.Release(Hand.Left, new TVec3(5, 5, 5), maxFling: 4f); // len ~8.66
            Assert.AreEqual(4f, fling.Length, 1e-3f, "the launch-off is comfort-clamped");
            Assert.IsFalse(climb.IsClimbing, "both hands off the wall");
        }
    }
}
