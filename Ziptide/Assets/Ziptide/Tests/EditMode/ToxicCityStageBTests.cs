using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class ToxicCityStageBTests
    {
        [Test]
        public void Build_AddsBudgetedColliderFreeStreetLifeAndIsIdempotent()
        {
            GameObject city = null;
            CityLayoutDefinition kit = null;
            try
            {
                city = new GameObject("__TEST_CITY_ROOT");
                Transform district = new GameObject("District_Test").transform;
                district.SetParent(city.transform, false);
                kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
                kit.districts.Add(new DistrictDef
                {
                    id = "Test",
                    bounds = new Vector2(22f, 18f),
                });

                ToxicCityStageB.Summary summary = ToxicCityStageB.Build(city.transform, kit);
                Transform marker = city.transform.Find(ToxicCityStageB.RootName);
                Transform street = district.Find(ToxicCityStageB.DistrictRootName);

                Assert.That(marker, Is.Not.Null);
                Assert.That(street, Is.Not.Null);
                Assert.That(summary.Districts, Is.EqualTo(1));
                Assert.That(street.GetComponentsInChildren<Transform>(true)
                    .Count(t => t.name.StartsWith("StreetLamp_")), Is.EqualTo(ToxicCityStageB.LampsPerDistrict));
                Assert.That(street.Find("DistrictSign"), Is.Not.Null);
                Assert.That(street.GetComponent<CityStreetLifeRuntime>(), Is.Not.Null);
                Assert.That(street.GetComponentsInChildren<Collider>(true), Is.Empty,
                    "Stage B is presentation-only and may not introduce traversal snags.");

                int objects = ToxicCityStageB.CountObjects(street);
                Assert.That(objects, Is.LessThanOrEqualTo(ToxicCityStageB.HardObjectsPerDistrict));
                Assert.That(summary.Materials, Is.LessThanOrEqualTo(ToxicCityStageB.HardMaterials));

                foreach (Transform moving in street.GetComponentsInChildren<Transform>(true)
                             .Where(t => t.name.StartsWith("Steam_") || t.name.StartsWith("LampGlow_")))
                {
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(moving.gameObject);
                    Assert.That((flags & StaticEditorFlags.BatchingStatic) == 0, Is.True,
                        moving.name + " is animated and must stay dynamic.");
                }

                ToxicCityStageB.Summary second = ToxicCityStageB.Build(city.transform, kit);
                Assert.That(second.Districts, Is.EqualTo(1));
                Assert.That(ToxicCityStageB.CountObjects(street), Is.EqualTo(objects));
            }
            finally
            {
                if (city != null) Object.DestroyImmediate(city);
                if (kit != null) Object.DestroyImmediate(kit);
            }
        }
    }
}
