using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    public class CityLayoutDefinitionTests
    {
        private static CityLayoutDefinition MakeValid()
        {
            var kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
            kit.walkwayHeight = 0f;
            kit.districts.Add(new DistrictDef { id = "A" });
            kit.districts.Add(new DistrictDef
            {
                id = "B",
                heroBuildings = { new HeroBuildingDef { id = "Hall", interior = InteriorKind.JobGiver, interiorMarkerId = "inside" } },
            });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "A", toDistrictId = "B" });
            return kit;
        }

        [Test]
        public void ValidLayout_HasNoIssues()
        {
            var kit = MakeValid();
            CollectionAssert.IsEmpty(kit.Validate());
        }

        [Test]
        public void Connection_ToUnknownDistrict_IsFlagged()
        {
            var kit = MakeValid();
            kit.connections.Add(new ConnectionDef { fromDistrictId = "A", toDistrictId = "Nowhere" });
            CollectionAssert.IsNotEmpty(kit.Validate());
        }

        [Test]
        public void HeroBuilding_WithInteriorButNoMarker_IsFlagged()
        {
            var kit = MakeValid();
            kit.districts.Add(new DistrictDef
            {
                id = "C",
                heroBuildings = { new HeroBuildingDef { id = "Bad", interior = InteriorKind.Mission, interiorMarkerId = "" } },
            });
            CollectionAssert.IsNotEmpty(kit.Validate());
        }

        [Test]
        public void DuplicateDistrictId_IsFlagged()
        {
            var kit = MakeValid();
            kit.districts.Add(new DistrictDef { id = "A" });
            CollectionAssert.IsNotEmpty(kit.Validate());
        }
    }
}
