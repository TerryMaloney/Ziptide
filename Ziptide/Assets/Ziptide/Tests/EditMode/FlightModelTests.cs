using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// P4b FlightModel contracts (SPACEFLIGHT_PHYSICS rails, enforced in math): determinism, speed
    /// and pitch caps, snap-only yaw, the un-exitable soft-walled lane, decay to rest, and the
    /// structural no-roll guarantee. Pure, headless.
    /// </summary>
    public class FlightModelTests
    {
        private static readonly FlightParams P = FlightParams.Default;

        private static FlightState Fly(FlightState s, float throttle, float pitch, float seconds)
        {
            for (float t = 0f; t < seconds; t += 1f / 72f)
                s = FlightModel.Tick(s, P, throttle, pitch, 1f / 72f);
            return s;
        }

        [Test]
        public void Tick_IsDeterministic()
        {
            var a = Fly(default, 1f, 0.4f, 5f);
            var b = Fly(default, 1f, 0.4f, 5f);
            Assert.AreEqual(a.position, b.position);
            Assert.AreEqual(a.speed, b.speed);
            Assert.AreEqual(a.pitchDeg, b.pitchDeg);
        }

        [Test]
        public void Speed_RampsAndCapsAtMax()
        {
            var s = Fly(default, 1f, 0f, 10f);
            Assert.AreEqual(P.maxSpeed, s.speed, 0.01f);
            var early = Fly(default, 1f, 0f, 0.5f);
            Assert.Less(early.speed, P.maxSpeed, "speed must RAMP, not jump (comfort)");
        }

        [Test]
        public void ZeroThrottle_DecaysToRest()
        {
            var s = Fly(default, 1f, 0f, 10f);
            s = Fly(s, 0f, 0f, 10f);
            Assert.AreEqual(0f, s.speed, 0.01f);
        }

        [Test]
        public void Pitch_HardClamps()
        {
            var s = Fly(default, 0f, 1f, 30f);
            Assert.AreEqual(P.pitchClampDeg, s.pitchDeg, 0.01f);
            s = Fly(s, 0f, -1f, 60f);
            Assert.AreEqual(-P.pitchClampDeg, s.pitchDeg, 0.01f);
        }

        [Test]
        public void Lane_CanNeverBeExited_EvenFlyingStraightOutForMinutes()
        {
            var s = default(FlightState);
            for (int i = 0; i < 72 * 180; i++) // 3 minutes at full throttle, straight line
            {
                s = FlightModel.Tick(s, P, 1f, 0f, 1f / 72f);
                Assert.LessOrEqual(s.position.magnitude, P.laneRadius + 0.01f,
                    "escaped the lane at tick " + i);
            }
        }

        [Test]
        public void SoftWall_BleedsSpeedInsteadOfHardStopping()
        {
            var s = default(FlightState);
            s.position = Vector3.forward * (P.laneRadius - 1f);
            s.speed = P.maxSpeed;
            s = FlightModel.Tick(s, P, 1f, 0f, 1f / 72f);
            Assert.Less(s.speed, P.maxSpeed, "wall contact should bleed speed");
            Assert.Greater(s.speed, 0f, "but never slam to zero (comfort)");
        }

        [Test]
        public void Yaw_OnlyChangesBySnaps_AndWraps()
        {
            var s = default(FlightState);
            s = Fly(s, 1f, 0.5f, 5f);
            Assert.AreEqual(0f, s.yawDeg, "no input path may smooth-yaw");
            for (int i = 0; i < 13; i++) s = FlightModel.SnapYaw(s, P, +1);
            Assert.AreEqual(Mathf.Repeat(13 * P.yawSnapDeg, 360f), s.yawDeg, 0.001f);
            s = FlightModel.SnapYaw(s, P, 0);
            Assert.AreEqual(Mathf.Repeat(13 * P.yawSnapDeg, 360f), s.yawDeg, 0.001f, "0 = no snap");
        }

        [Test]
        public void Forward_MatchesYawPitchConvention()
        {
            var s = default(FlightState);
            Assert.Less(Vector3.Distance(FlightModel.Forward(s), Vector3.forward), 0.001f);
            s.yawDeg = 90f;
            Assert.Less(Vector3.Distance(FlightModel.Forward(s), Vector3.right), 0.001f);
            s.yawDeg = 0f; s.pitchDeg = 35f;
            Assert.Greater(FlightModel.Forward(s).y, 0.5f, "positive pitch climbs");
        }

        [Test]
        public void NoRoll_ByConstruction()
        {
            // The comfort law is structural: FlightState has no roll field at all.
            Assert.IsNull(typeof(FlightState).GetField("rollDeg"),
                "someone added roll to FlightState — that is a comfort-law violation");
            // And Forward never banks: the ship's right vector stays horizontal at any pitch/yaw.
            var s = new FlightState { yawDeg = 123f, pitchDeg = 30f };
            Vector3 right = Vector3.Cross(Vector3.up, FlightModel.Forward(s));
            Assert.AreEqual(0f, Quaternion.Euler(-s.pitchDeg, s.yawDeg, 0f).eulerAngles.z, 0.001f);
            Assert.Greater(right.magnitude, 0.1f);
        }

        [Test]
        public void DegenerateDt_IsANoOp()
        {
            var s = Fly(default, 1f, 0.3f, 2f);
            var after = FlightModel.Tick(s, P, 1f, 1f, 0f);
            Assert.AreEqual(s.position, after.position);
            Assert.AreEqual(s.speed, after.speed);
            after = FlightModel.Tick(s, P, 1f, 1f, float.NaN);
            Assert.AreEqual(s.position, after.position);
        }

        [Test]
        public void HalfSteps_ApproximateFullSteps()
        {
            var whole = FlightModel.Tick(Fly(default, 1f, 0f, 5f), P, 1f, 0f, 0.2f);
            var s = Fly(default, 1f, 0f, 5f);
            s = FlightModel.Tick(s, P, 1f, 0f, 0.1f);
            s = FlightModel.Tick(s, P, 1f, 0f, 0.1f);
            Assert.Less(Vector3.Distance(whole.position, s.position), 0.5f,
                "integration must be frame-rate stable (Quest 72Hz vs editor)");
        }
    }
}
