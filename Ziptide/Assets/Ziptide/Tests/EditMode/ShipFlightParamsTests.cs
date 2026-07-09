using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Content.Ship;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ShipDefinition → FlightParams mapping law: ship data sets the pace, comfort caps stay at the
    /// reviewed defaults — a ship asset can slow the pitch, never uncap it, and the snap-yaw /
    /// pitch-clamp / lane-radius rails are untouchable from data.
    /// </summary>
    public class ShipFlightParamsTests
    {
        [Test]
        public void NullDefinition_YieldsDefaults()
        {
            var p = ShipFlightRuntime.ParamsFrom(null);
            Assert.AreEqual(FlightParams.Default.maxSpeed, p.maxSpeed);
            Assert.AreEqual(FlightParams.Default.yawSnapDeg, p.yawSnapDeg);
        }

        [Test]
        public void CruiseSpeed_MapsToMaxSpeed()
        {
            var def = ScriptableObject.CreateInstance<ShipDefinition>();
            try
            {
                def.cruiseSpeed = 25f;
                Assert.AreEqual(25f, ShipFlightRuntime.ParamsFrom(def).maxSpeed);
            }
            finally { Object.DestroyImmediate(def); }
        }

        [Test]
        public void TurnRate_CanSlowPitch_ButNeverExceedTheComfortDefault()
        {
            var def = ScriptableObject.CreateInstance<ShipDefinition>();
            try
            {
                def.turnRateDegrees = 10f;
                Assert.AreEqual(10f, ShipFlightRuntime.ParamsFrom(def).pitchRateDeg,
                    "A gentler ship may pitch slower than the default.");

                def.turnRateDegrees = 500f;
                Assert.AreEqual(FlightParams.Default.pitchRateDeg, ShipFlightRuntime.ParamsFrom(def).pitchRateDeg,
                    "Data must never raise the pitch rate past the comfort-reviewed default.");
            }
            finally { Object.DestroyImmediate(def); }
        }

        [Test]
        public void BoostMultiplier_MapsFromData_ClampedBothWays()
        {
            var def = ScriptableObject.CreateInstance<ShipDefinition>();
            try
            {
                def.boostMultiplier = 2.5f;
                Assert.AreEqual(2.5f, ShipFlightRuntime.ParamsFrom(def).boostMultiplier);

                def.boostMultiplier = 99f;
                Assert.AreEqual(3f, ShipFlightRuntime.ParamsFrom(def).boostMultiplier,
                    "Data can tune boost but never past the comfort ceiling.");

                def.boostMultiplier = 0.2f;
                Assert.AreEqual(1f, ShipFlightRuntime.ParamsFrom(def).boostMultiplier,
                    "A boost below 1 would make the button slow the ship — clamp to neutral.");
            }
            finally { Object.DestroyImmediate(def); }
        }

        [Test]
        public void LoadoutStats_DriveSpeedBoostAndPitch_WithTenAsTheHandlingCeiling()
        {
            // The SHIP-MORE #1 seam: the hangar's resolved ShipStats become flight feel.
            var racer = ShipLoadoutCore.Resolve(ShipChassisPreset.Find("racer"), null);
            var p = ShipFlightRuntime.ParamsFrom(racer);
            Assert.AreEqual(racer.Speed, p.maxSpeed, "cruise = resolved Speed");
            Assert.AreEqual(racer.Boost, p.boostMultiplier, 1e-4f, "boost carries (racer 2.6 < the 3.0 ceiling)");
            Assert.AreEqual(FlightParams.Default.pitchRateDeg * racer.Handling / 10f, p.pitchRateDeg, 1e-3f,
                "handling scales pitch rate linearly toward the comfort ceiling");

            var maxHandling = new ShipStats { Speed = 30f, Handling = 10f, Boost = 1.5f };
            Assert.AreEqual(FlightParams.Default.pitchRateDeg,
                ShipFlightRuntime.ParamsFrom(maxHandling).pitchRateDeg,
                "handling 10 = exactly the comfort-reviewed default, never past it");

            var overTuned = new ShipStats { Speed = 30f, Handling = 25f, Boost = 9f };
            var q = ShipFlightRuntime.ParamsFrom(overTuned);
            Assert.AreEqual(FlightParams.Default.pitchRateDeg, q.pitchRateDeg, "handling can't uncap pitch");
            Assert.AreEqual(3f, q.boostMultiplier, "boost can't pass the ceiling");

            var barge = new ShipStats { Speed = 8f, Handling = 1f, Boost = 1.1f };
            Assert.GreaterOrEqual(ShipFlightRuntime.ParamsFrom(barge).pitchRateDeg, 8f,
                "even the barge-est hauler still steers (pitch-rate floor)");
        }

        [Test]
        public void EveryChassis_YieldsFlyableComfortLegalParams()
        {
            foreach (var chassis in ShipChassisPreset.All)
            {
                var p = ShipFlightRuntime.ParamsFrom(ShipLoadoutCore.Resolve(chassis, null));
                Assert.Greater(p.maxSpeed, 0f, chassis.Id + " must fly");
                Assert.That(p.boostMultiplier, Is.InRange(1f, 3f), chassis.Id + " boost in the legal band");
                Assert.That(p.pitchRateDeg, Is.InRange(8f, FlightParams.Default.pitchRateDeg),
                    chassis.Id + " pitch in the comfort band");
                Assert.AreEqual(FlightParams.Default.yawSnapDeg, p.yawSnapDeg,
                    chassis.Id + " can't change the snap-yaw comfort constant");
                Assert.AreEqual(FlightParams.Default.laneRadius, p.laneRadius,
                    chassis.Id + " can't move the lane wall");
            }
        }

        [Test]
        public void ComfortRails_AreDataProof()
        {
            var def = ScriptableObject.CreateInstance<ShipDefinition>();
            try
            {
                def.cruiseSpeed = 999f;
                def.turnRateDegrees = 999f;
                def.boostMultiplier = 999f;
                var p = ShipFlightRuntime.ParamsFrom(def);
                Assert.AreEqual(FlightParams.Default.pitchClampDeg, p.pitchClampDeg);
                Assert.AreEqual(FlightParams.Default.yawSnapDeg, p.yawSnapDeg);
                Assert.AreEqual(FlightParams.Default.reverseFraction, p.reverseFraction,
                    "Reverse stays a fraction of forward no matter the data.");
                Assert.AreEqual(FlightParams.Default.rollRateDeg, p.rollRateDeg,
                    "The barrel-roll speed is a comfort constant, not ship data.");
                Assert.AreEqual(FlightParams.Default.laneRadius, p.laneRadius,
                    "The lane must stay under the 2km floating-origin trigger no matter the data.");
            }
            finally { Object.DestroyImmediate(def); }
        }
    }
}
