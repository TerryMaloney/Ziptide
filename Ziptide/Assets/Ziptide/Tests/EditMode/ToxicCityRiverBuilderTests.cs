using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class ToxicCityRiverBuilderTests
    {
        [Test]
        public void Build_ReplacesDecorativeCanalWithSupportedLethalBasin()
        {
            GameObject city = null;
            CityLayoutDefinition kit = null;
            try
            {
                city = new GameObject("__TEST_TOXIC_CITY_ROOT");
                Transform legacy = new GameObject("Canals").transform;
                legacy.SetParent(city.transform, false);
                GameObject legacySlab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                legacySlab.name = "Canal_0";
                legacySlab.transform.SetParent(legacy, false);

                kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
                kit.walkwayHeight = 0f;
                kit.canals.Add(new CanalRegionDef
                {
                    center = new Vector3(4f, 0f, -3f),
                    size = new Vector2(12f, 24f),
                    depth = 2.4f,
                });

                ToxicCityRiverBuilder.Summary summary = ToxicCityRiverBuilder.Build(city.transform, kit);

                Assert.That(city.transform.Find("Canals"), Is.Null,
                    "The old decorative-only canal owner must be retired.");
                Transform root = city.transform.Find(ToxicCityRiverBuilder.RootName);
                Assert.That(root, Is.Not.Null);
                Transform river = root.Find(ToxicCityRiverBuilder.RiverPrefix + "0");
                Assert.That(river, Is.Not.Null);
                Assert.That(summary.Rivers, Is.EqualTo(1));

                Transform floor = river.Find("BasinFloor");
                Assert.That(floor, Is.Not.Null);
                Assert.That(floor.GetComponent<Collider>(), Is.Not.Null);
                Assert.That(floor.GetComponent<Collider>().enabled, Is.True);

                Transform surface = river.Find("ToxicSurface");
                Assert.That(surface, Is.Not.Null);
                Assert.That(surface.GetComponent<Collider>(), Is.Null,
                    "The visible surface must never become spawn support.");

                string[] banks = { "Bank_W", "Bank_E", "Bank_S", "Bank_N" };
                foreach (string bank in banks)
                {
                    Transform t = river.Find(bank);
                    Assert.That(t, Is.Not.Null, bank);
                    Assert.That(t.GetComponent<Collider>(), Is.Not.Null, bank);
                }

                var runtime = river.GetComponent<ToxicRiverRuntime>();
                Assert.That(runtime, Is.Not.Null);
                Assert.That(runtime.RiverId, Is.EqualTo("toxic_city_river_0"));
                Assert.That(runtime.LethalAfterSeconds, Is.EqualTo(4.5f).Within(0.01f));
                Assert.That(runtime.BoundsSize.y, Is.GreaterThan(2f));

                var motion = river.GetComponentInChildren<ToxicRiverSurfaceRuntime>(true);
                Assert.That(motion, Is.Not.Null);
                Transform motionRoot = motion.transform;
                Assert.That(motionRoot.GetComponentsInChildren<Transform>(true)
                    .Count(t => t.name.StartsWith("Flow_")), Is.EqualTo(ToxicCityRiverBuilder.FlowRibbonCount));
                Assert.That(motionRoot.GetComponentsInChildren<Transform>(true)
                    .Count(t => t.name.StartsWith("Bubble_")), Is.EqualTo(ToxicCityRiverBuilder.BubbleCount));

                foreach (Transform moving in motionRoot.GetComponentsInChildren<Transform>(true)
                             .Where(t => t.name.StartsWith("Flow_") || t.name.StartsWith("Bubble_")))
                {
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(moving.gameObject);
                    Assert.That((flags & StaticEditorFlags.BatchingStatic) == 0, Is.True,
                        moving.name + " is animated and must not be batching-static.");
                }

                int firstRendererCount = summary.Renderers;
                ToxicCityRiverBuilder.Summary second = ToxicCityRiverBuilder.Build(city.transform, kit);
                Assert.That(second.Rivers, Is.EqualTo(1));
                Assert.That(second.Renderers, Is.EqualTo(firstRendererCount),
                    "Repeated save hooks must be idempotent.");
            }
            finally
            {
                if (city != null) Object.DestroyImmediate(city);
                if (kit != null) Object.DestroyImmediate(kit);
            }
        }
    }
}
