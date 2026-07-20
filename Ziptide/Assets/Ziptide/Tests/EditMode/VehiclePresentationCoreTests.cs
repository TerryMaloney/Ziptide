using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    public sealed class VehiclePresentationCoreTests
    {
        [TestCase(VehicleArchetype.Skiff, "skiff", 15)]
        [TestCase(VehicleArchetype.Hoverbike, "hoverbike", 14)]
        [TestCase(VehicleArchetype.DrillCrawler, "crawler", 20)]
        public void StarterFleet_HasDistinctHumanScalePresentation(
            VehicleArchetype archetype, string family, int minimumParts)
        {
            VehicleVisualProfile profile = VehiclePresentationCore.Resolve(archetype);

            Assert.That(profile.Family, Is.EqualTo(family));
            Assert.That(profile.MinimumParts, Is.GreaterThanOrEqualTo(minimumParts));
            Assert.That(profile.ApproximateBounds.x, Is.InRange(1.4f, 3.0f));
            Assert.That(profile.ApproximateBounds.y, Is.InRange(1.2f, 2.4f));
            Assert.That(profile.ApproximateBounds.z, Is.InRange(2.5f, 3.8f));
            Assert.That(profile.Glow.maxColorComponent, Is.GreaterThan(0.75f));
        }

        [Test]
        public void GroundVehicleParams_CannotTiltHorizon()
        {
            var def = ScriptableObject.CreateInstance<VehicleDefinition>();
            try
            {
                def.cruiseSpeed = 30f;
                def.boostMultiplier = 4f;
                VehicleVisualProfile profile = VehiclePresentationCore.Resolve(VehicleArchetype.Rover);
                FlightParams p = VehicleRuntime.ParamsFrom(def);

                Assert.That(profile.Family, Is.EqualTo("utility"));
                Assert.That(p.pitchRateDeg, Is.Zero);
                Assert.That(p.pitchClampDeg, Is.Zero);
                Assert.That(p.boostMultiplier, Is.LessThanOrEqualTo(3f));
            }
            finally
            {
                Object.DestroyImmediate(def);
            }
        }
    }
}
