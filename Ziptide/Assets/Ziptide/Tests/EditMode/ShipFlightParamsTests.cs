using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
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
        public void ComfortRails_AreDataProof()
        {
            var def = ScriptableObject.CreateInstance<ShipDefinition>();
            try
            {
                def.cruiseSpeed = 999f;
                def.turnRateDegrees = 999f;
                var p = ShipFlightRuntime.ParamsFrom(def);
                Assert.AreEqual(FlightParams.Default.pitchClampDeg, p.pitchClampDeg);
                Assert.AreEqual(FlightParams.Default.yawSnapDeg, p.yawSnapDeg);
                Assert.AreEqual(FlightParams.Default.laneRadius, p.laneRadius,
                    "The lane must stay under the 2km floating-origin trigger no matter the data.");
            }
            finally { Object.DestroyImmediate(def); }
        }
    }
}
