using NUnit.Framework;
using Ziptide.Content.Traversal;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Phase 1.4: lift cycle determinism + jump-pad ballistics. Pure, headless.</summary>
    public class LiftCoreTests
    {
        private static readonly TVec3 Ground = new TVec3(0, 0, 0);
        private static readonly TVec3 Ledge = new TVec3(0, 6, 0);

        // ── Lift ─────────────────────────────────────────────────────────────
        [Test]
        public void Lift_DwellsAtStop_ThenTravels()
        {
            var lift = new LiftCycle(new[] { Ground, Ledge }, travelSpeed: 2f, dwellSeconds: 3f);

            var p0 = lift.Evaluate(1f, out bool dwelling0, out _);
            Assert.IsTrue(dwelling0, "t=1s is inside the 3s dwell");
            Assert.AreEqual(0f, p0.Y, 1e-4f);

            var p1 = lift.Evaluate(4.5f, out bool dwelling1, out int next);
            Assert.IsFalse(dwelling1, "t=4.5s is 1.5s into the climb");
            Assert.AreEqual(3f, p1.Y, 1e-3f, "1.5s at 2 m/s = 3m up");
            Assert.AreEqual(1, next, "heading to stop 1");
        }

        [Test]
        public void Lift_IsDeterministic_AndWraps()
        {
            var lift = new LiftCycle(new[] { Ground, Ledge }, travelSpeed: 2f, dwellSeconds: 3f);
            // Full cycle: dwell 3 + up 3 + dwell 3 + down 3 = 12s.
            Assert.AreEqual(12f, lift.CycleSeconds, 1e-3f);

            var a = lift.Evaluate(4.5f, out _, out _);
            var b = lift.Evaluate(4.5f + lift.CycleSeconds, out _, out _);
            Assert.AreEqual(a.Y, b.Y, 1e-3f, "same phase next cycle = same position (late-join safe)");
        }

        [Test]
        public void Lift_TwoStopsMinimum_IsEnforced()
        {
            Assert.Throws<System.ArgumentException>(() => new LiftCycle(new[] { Ground }));
        }

        // ── Jump pad ─────────────────────────────────────────────────────────
        [Test]
        public void JumpPad_LandsOnTheTarget()
        {
            var from = new TVec3(0, 0, 0);
            var to = new TVec3(8, 3, 2);
            var v = JumpPad.LaunchVelocity(from, to, g: 9.81f, apexMargin: 2f);

            // Fly the arc with the same math and confirm the landing.
            float apexY = 3f + 2f;
            float riseT = (float)System.Math.Sqrt(2.0 * (apexY - 0f) / 9.81);
            float fallT = (float)System.Math.Sqrt(2.0 * (apexY - 3f) / 9.81);
            var landing = JumpPad.PositionAt(from, v, 9.81f, riseT + fallT);
            Assert.AreEqual(to.X, landing.X, 0.01f);
            Assert.AreEqual(to.Y, landing.Y, 0.01f);
            Assert.AreEqual(to.Z, landing.Z, 0.01f);
        }

        [Test]
        public void JumpPad_ApexClearsTheHigherEndpoint()
        {
            var from = new TVec3(0, 5, 0);   // launching DOWN to a lower ledge
            var to = new TVec3(6, 1, 0);
            var v = JumpPad.LaunchVelocity(from, to, g: 9.81f, apexMargin: 2f);
            Assert.Greater(v.Y, 0f, "even a downhill launch arcs UP first — a flat drop reads as a stumble");

            // Peak of the arc = from.Y + vy^2/(2g); must be apexMargin above the HIGHER endpoint.
            float peak = from.Y + (v.Y * v.Y) / (2f * 9.81f);
            Assert.AreEqual(7f, peak, 0.02f, "apex = max(endpoints) + margin");
        }
    }
}
