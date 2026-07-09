using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// P4b FlightModel contracts (SPACEFLIGHT_PHYSICS rails, enforced in math): determinism, speed
    /// and pitch caps, snap-only yaw, the un-exitable soft-walled lane, decay to rest, boost and
    /// reverse caps, and the roll-never-rests guarantee (comfort law v2, Terry 2026-07-09: roll
    /// exists only as a self-completing barrel roll that lands back on exactly level). Pure, headless.
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
            // One 72Hz tick at maxSpeed covers ~0.55m — start INSIDE that distance of the wall so
            // this tick actually crosses it (the first CI run started 1m out and never touched it).
            var s = default(FlightState);
            s.position = Vector3.forward * (P.laneRadius - 0.3f);
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
        public void Roll_NeverRests_BarrelRollCompletesToExactlyLevel()
        {
            // Comfort law v2: roll exists ONLY inside a barrel roll. Start one, tick it out, and
            // the ship must land back on EXACTLY 0 — no code path leaves the ship banked.
            var s = FlightModel.StartBarrelRoll(default, +1);
            Assert.AreEqual(1, s.rollDirection);

            bool sawBank = false;
            for (int i = 0; i < 72 * 3 && s.rollDirection != 0; i++) // 3s ceiling — must finish well inside
            {
                s = FlightModel.Tick(s, P, 0.5f, 0f, 1f / 72f);
                if (Mathf.Abs(s.rollDeg) > 1f) sawBank = true;
            }
            Assert.IsTrue(sawBank, "the roll should actually bank mid-maneuver");
            Assert.AreEqual(0, s.rollDirection, "the barrel roll must self-complete");
            Assert.AreEqual(0f, s.rollDeg, "and land on EXACTLY level — roll never rests");
        }

        [Test]
        public void BarrelRoll_CannotChain_AndNeverSteersTheShip()
        {
            var s = FlightModel.StartBarrelRoll(default, +1);
            var mashed = FlightModel.StartBarrelRoll(s, -1);
            Assert.AreEqual(1, mashed.rollDirection, "mid-roll requests are ignored — no washing machine");
            Assert.AreEqual(s.rollDeg, mashed.rollDeg);

            Assert.AreEqual(0, FlightModel.StartBarrelRoll(default, 0).rollDirection, "0 = no roll");

            // Rolling never changes where the ship is going: Forward ignores roll entirely.
            var banked = new FlightState { yawDeg = 40f, pitchDeg = 20f, rollDeg = 137f, rollDirection = 1 };
            var level = new FlightState { yawDeg = 40f, pitchDeg = 20f };
            Assert.Less(Vector3.Distance(FlightModel.Forward(banked), FlightModel.Forward(level)), 1e-5f);
        }

        [Test]
        public void Orientation_CarriesRoll_AndMatchesForwardWhenLevel()
        {
            var level = new FlightState { yawDeg = 123f, pitchDeg = 30f };
            Assert.Less(Vector3.Distance(FlightModel.Orientation(level) * Vector3.forward,
                FlightModel.Forward(level)), 1e-5f);

            var banked = new FlightState { rollDeg = 90f, rollDirection = 1 };
            float bankY = (FlightModel.Orientation(banked) * Vector3.right).y;
            Assert.Greater(Mathf.Abs(bankY), 0.9f, "a 90-degree roll should put the wings vertical");
        }

        [Test]
        public void Boost_MultipliesTheCap_AndReleasingDecaysBack()
        {
            var s = default(FlightState);
            for (float t = 0f; t < 20f; t += 1f / 72f)
                s = FlightModel.Tick(s, P, 1f, 0f, true, 1f / 72f);
            Assert.AreEqual(P.maxSpeed * P.boostMultiplier, s.speed, 0.01f, "boost cap = max × multiplier");

            s = Fly(s, 1f, 0f, 20f); // boost released, throttle still pinned
            Assert.AreEqual(P.maxSpeed, s.speed, 0.01f, "releasing boost must decay back to the unboosted cap");
        }

        [Test]
        public void Reverse_IsCappedToItsFraction_AndBoostScalesIt()
        {
            var s = Fly(default, -1f, 0f, 20f);
            Assert.AreEqual(-P.maxSpeed * P.reverseFraction, s.speed, 0.01f,
                "full reverse caps at the reverse fraction, never full speed backwards");
            Vector3 before = s.position;
            s = FlightModel.Tick(s, P, -1f, 0f, 1f / 72f);
            Assert.Less(Vector3.Dot(s.position - before, FlightModel.Forward(s)), 0f,
                "negative speed moves the ship backwards along Forward");

            var b = default(FlightState);
            for (float t = 0f; t < 20f; t += 1f / 72f)
                b = FlightModel.Tick(b, P, -1f, 0f, true, 1f / 72f);
            Assert.AreEqual(-P.maxSpeed * P.reverseFraction * P.boostMultiplier, b.speed, 0.01f,
                "boost backward mirrors boost forward, on the reverse cap");
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
