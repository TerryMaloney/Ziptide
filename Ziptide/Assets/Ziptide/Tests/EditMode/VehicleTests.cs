using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// DRIVABLE VEHICLES 3.2 laws: the ground profile can NEVER tilt the horizon (pitch rate AND
    /// clamp are zero by construction — no data, no input, no bug tips a seated rider), drive stats
    /// map with the shared comfort clamps, a vehicle's roam can never exceed the flight lane, and
    /// the starter fleet is unique/sane/biome-spread. Pure, headless.
    /// </summary>
    public class VehicleTests
    {
        [Test]
        public void GroundProfile_CanNeverTiltTheHorizon()
        {
            var def = ScriptableObject.CreateInstance<VehicleDefinition>();
            try
            {
                def.cruiseSpeed = 999f; // even absurd data...
                var p = VehicleRuntime.ParamsFrom(def);
                Assert.AreEqual(0f, p.pitchRateDeg, "...cannot give a ground vehicle a pitch rate");
                Assert.AreEqual(0f, p.pitchClampDeg, "...or any pitch range at all");

                // And the model holds it: full pitch input for 10 simulated seconds changes nothing.
                var s = default(FlightState);
                for (int i = 0; i < 720; i++)
                    s = FlightModel.Tick(s, p, 1f, 1f, 1f / 72f);
                Assert.AreEqual(0f, s.pitchDeg, "the horizon stays level no matter the input");
            }
            finally { Object.DestroyImmediate(def); }
        }

        [Test]
        public void DriveStats_MapWithTheSharedComfortClamps()
        {
            var def = ScriptableObject.CreateInstance<VehicleDefinition>();
            try
            {
                def.cruiseSpeed = 18f;
                def.boostMultiplier = 9f;
                def.roamRadius = 999999f;
                var p = VehicleRuntime.ParamsFrom(def);
                Assert.AreEqual(18f, p.maxSpeed);
                Assert.AreEqual(3f, p.boostMultiplier, "boost shares the ship's 3.0 ceiling");
                Assert.LessOrEqual(p.laneRadius, FlightParams.Default.laneRadius,
                    "a ride can never roam past the flight lane's floating-origin bound");
                Assert.AreEqual(FlightParams.Default.yawSnapDeg, p.yawSnapDeg,
                    "snap yaw is the one shared comfort constant");
            }
            finally { Object.DestroyImmediate(def); }
        }

        [Test]
        public void NullDefinition_StillYieldsARideableProfile()
        {
            var p = VehicleRuntime.ParamsFrom(null);
            Assert.Greater(p.maxSpeed, 0f, "a missing asset must never brick the ride");
            Assert.AreEqual(0f, p.pitchRateDeg);
        }

        [Test]
        public void StarterFleet_IsUniqueSaneAndBiomeSpread()
        {
            var specs = VehicleAuthor.VehicleSpecs();
            Assert.GreaterOrEqual(specs.Length, 3, "the starter fleet ships at least three rides");
            var ids = new HashSet<string>();
            var biomes = new HashSet<string>();
            foreach (var s in specs)
            {
                Assert.IsTrue(ids.Add(s.Id), "duplicate vehicle id: " + s.Id);
                Assert.Greater(s.Cruise, 0f, s.Id + " must move");
                Assert.That(s.Boost, Is.InRange(1f, 3f), s.Id + " boost in the shared band");
                Assert.GreaterOrEqual(s.Hover, 0f, s.Id + " hover can't be negative");
                Assert.Greater(s.Roam, 0f, s.Id + " needs somewhere to go");
                if (!string.IsNullOrEmpty(s.Biome)) biomes.Add(s.Biome);
            }
            Assert.GreaterOrEqual(biomes.Count, 3, "each biome family gets a signature ride");
        }
    }
}
