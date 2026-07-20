using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    public sealed class ToxicCityStageAIntegrationTests
    {
        private const int IntendedPaletteSlots = 8;

        [Test]
        public void Enrich_ReplacesLegacyBoxesWithBudgetedCompositeGrammar()
        {
            GameObject city = null;
            CityLayoutDefinition kit = null;
            try
            {
                kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
                kit.seed = 1337;
                kit.walkwayHeight = 0f;
                kit.districts.Add(new DistrictDef
                {
                    id = "Test",
                    bounds = new Vector2(20f, 18f),
                    heightTier = 1,
                });

                city = new GameObject("__TEST_CITY_ROOT");
                var district = new GameObject("District_Test");
                district.transform.SetParent(city.transform, false);
                MakeLegacyFacade(district.transform, "Facade_X_0", new Vector3(-3f, 4f, 7f), new Vector3(5f, 8f, 3f));
                MakeLegacyFacade(district.transform, "Facade_Z_0", new Vector3(7f, 5f, -2f), new Vector3(3f, 10f, 5f));

                ToxicCityStageA.Summary summary = ToxicCityStageA.Enrich(city.transform, kit);
                Assert.That(summary.Districts, Is.EqualTo(1));
                Assert.That(summary.Facades, Is.EqualTo(2));
                Assert.That(summary.Materials, Is.LessThanOrEqualTo(IntendedPaletteSlots),
                    "Stage A must stay on the closed eight-slot semantic palette.");
                Assert.That(city.transform.Find("__CITY_STAGE_A"), Is.Not.Null);
                Assert.That(district.transform.Find("__CITY_STAGE_A_CURBS"), Is.Not.Null);

                foreach (string facadeName in new[] { "Facade_X_0", "Facade_Z_0" })
                {
                    Transform facade = district.transform.Find(facadeName);
                    Assert.That(facade, Is.Not.Null);
                    Assert.That(facade.Find("Base"), Is.Not.Null);
                    Assert.That(facade.Find("Middle"), Is.Not.Null);
                    Assert.That(facade.Find("Cap"), Is.Not.Null);
                    Assert.That(facade.Find("Base").GetComponent<Collider>(), Is.Not.Null);
                    Assert.That(facade.Find("Middle").GetComponent<Collider>(), Is.Not.Null);
                    Assert.That(facade.Find("Cap").GetComponent<Collider>(), Is.Not.Null);
                    Assert.That(facade.GetComponentsInChildren<Transform>(true)
                        .Any(t => t.name.StartsWith("Window_") && t.GetComponent<Collider>() == null), Is.True);
                    Assert.That(facade.GetComponentsInChildren<Transform>(true)
                        .Any(t => t.name.StartsWith("RoofUnit_") && t.GetComponent<Collider>() == null), Is.True);
                }

                int objects = ToxicCityStageA.CountObjects(district.transform);
                Assert.That(objects, Is.LessThanOrEqualTo(ToxicCityStageA.HardObjectsPerDistrict));
                Assert.That(ToxicCityStageA.CountMaterials(district.transform),
                    Is.LessThanOrEqualTo(IntendedPaletteSlots));

                ToxicCityStageA.Summary second = ToxicCityStageA.Enrich(city.transform, kit);
                Assert.That(second.Facades, Is.EqualTo(summary.Facades), "Stage A must be idempotent.");
                Assert.That(ToxicCityStageA.CountObjects(district.transform), Is.EqualTo(objects));
            }
            finally
            {
                if (city != null) Object.DestroyImmediate(city);
                if (kit != null) Object.DestroyImmediate(kit);
            }
        }

        private static void MakeLegacyFacade(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject facade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            facade.name = name;
            facade.transform.SetParent(parent, false);
            facade.transform.localPosition = position;
            facade.transform.localScale = scale;
        }
    }
}
