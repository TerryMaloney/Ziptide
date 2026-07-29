using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The approved concentric city was not merely unbuilt — it was UNEXPRESSIBLE. Districts were
    /// axis-aligned rectangles, canals were rectangles, and a landmark was {name, pos, height, width},
    /// so a wedge, a ring canal, a breached sea wall and a tower that leans had nowhere to live in the
    /// data. These tests pin the shape of the model that fixes that, and the ordering law that keeps a
    /// concentric city concentric.
    /// </summary>
    public sealed class RingCityLayoutTests
    {
        private static RingCityDef Approved()
        {
            // The defaults are measured off the approved K1/K7 sheets; an author only overrides.
            return new RingCityDef { enabled = true };
        }

        [Test]
        public void DisabledRings_ValidateCleanly_SoExistingWorldsAreUntouched()
        {
            var rings = new RingCityDef { enabled = false, islandRadius = -99f };
            Assert.IsEmpty(rings.Validate(),
                "a layout that never opts in must not be judged by rules it does not use");
        }

        [Test]
        public void TheApprovedDefaults_AreValid()
        {
            Assert.IsEmpty(Approved().Validate());
        }

        [Test]
        public void TheRingsMustNest_OutwardsFromTheTowerIsland()
        {
            var rings = Approved();
            Assert.Less(rings.islandRadius, rings.canalRingRadius);
            Assert.Less(rings.canalRingRadius, rings.wedgeOuterRadius);
            Assert.Less(rings.wedgeOuterRadius, rings.seaWallRadius);
            Assert.Less(rings.seaWallRadius, rings.outskirtsRadius);
            Assert.Less(rings.outskirtsRadius, rings.gatePillarDistance);
        }

        [Test]
        public void ACanalRingInsideTheIsland_IsRejected()
        {
            var rings = Approved();
            rings.canalRingRadius = rings.islandRadius - 1f;
            Assert.IsNotEmpty(rings.Validate());
        }

        [Test]
        public void GatePillarsInsideTheOutskirts_AreRejected()
        {
            // They are a horizon anchor that ties every wide shot to the Ziptide, not geometry the
            // player can walk to. Bringing them inside the playable radius destroys both readings.
            var rings = Approved();
            rings.gatePillarDistance = rings.outskirtsRadius - 10f;
            Assert.IsNotEmpty(rings.Validate());
        }

        [Test]
        public void ATowerWithoutALean_OrWithAnAbsurdOne_IsRejectedAtTheEdges()
        {
            var rings = Approved();
            Assert.IsEmpty(rings.Validate(), "the approved 12 degree lean is valid");

            rings.towerLeanDegrees = 45f;
            Assert.IsNotEmpty(rings.Validate(), "a 45 degree tower stops reading as a lean");

            rings.towerLeanDegrees = -3f;
            Assert.IsNotEmpty(rings.Validate());
        }

        [Test]
        public void MoreBreachesThanWedges_LeavesNoWall()
        {
            var rings = Approved();
            rings.seaWallBreachCount = rings.wedgeCount;
            Assert.IsNotEmpty(rings.Validate());
        }

        [Test]
        public void TooFewWedges_CannotReadAsARingOfDistricts()
        {
            var rings = Approved();
            rings.wedgeCount = 2;
            Assert.IsNotEmpty(rings.Validate());
        }

        [Test]
        public void TheLayoutsOwnValidate_SurfacesRingProblems()
        {
            var kit = UnityEngine.ScriptableObject.CreateInstance<CityLayoutDefinition>();
            kit.rings = Approved();
            kit.rings.seaWallRadius = 1f; // inside the wedges

            var issues = kit.Validate();
            Assert.IsNotEmpty(issues, "a broken ring topology must fail the layout, not just its own block");

            UnityEngine.Object.DestroyImmediate(kit);
        }
    }
}
