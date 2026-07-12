#if UNITY_EDITOR
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class AmbientMoteTests
    {
        private GameObject _root;

        [TearDown]
        public void TearDown()
        {
            if (_root != null) Object.DestroyImmediate(_root);
            _root = null;

            foreach (var factory in Resources.FindObjectsOfTypeAll<VfxFactory>())
            {
                if (factory != null && factory.gameObject != null)
                    Object.DestroyImmediate(factory.gameObject);
            }
        }

        [Test]
        public void RouteSampling_IsStableUniqueAndNeverExceedsThree()
        {
            CollectionAssert.IsEmpty(AmbientMoteAuthor.SelectRouteIndices(0));
            CollectionAssert.AreEqual(new[] { 0 }, AmbientMoteAuthor.SelectRouteIndices(1));
            CollectionAssert.AreEqual(new[] { 0, 1 }, AmbientMoteAuthor.SelectRouteIndices(2));
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, AmbientMoteAuthor.SelectRouteIndices(3));
            CollectionAssert.AreEqual(new[] { 0, 5, 9 }, AmbientMoteAuthor.SelectRouteIndices(10));

            for (int count = 0; count < 100; count++)
            {
                int[] indices = AmbientMoteAuthor.SelectRouteIndices(count);
                Assert.LessOrEqual(indices.Length, AmbientMoteAuthor.MaxEmitters);
                Assert.AreEqual(indices.Length, indices.Distinct().Count(), "route count " + count);
                foreach (int index in indices)
                    Assert.That(index, Is.InRange(0, Mathf.Max(0, count - 1)));
            }
        }

        [Test]
        public void BiomeMapping_CoversBothCanonMoteRecipes()
        {
            Assert.AreEqual("motes_spore", AmbientMoteAuthor.RecipeFor(BiomePreset.TideFlats));
            Assert.AreEqual("motes_spore", AmbientMoteAuthor.RecipeFor(BiomePreset.CavernFloor));
            Assert.AreEqual("motes_amber", AmbientMoteAuthor.RecipeFor(BiomePreset.Dunes));
            Assert.AreEqual("motes_amber", AmbientMoteAuthor.RecipeFor(BiomePreset.Mesas));
            Assert.AreEqual("motes_amber", AmbientMoteAuthor.RecipeFor(BiomePreset.Canyon));

            foreach (string id in new[] { "motes_amber", "motes_spore" })
            {
                VfxRecipeDefinition recipe = VfxLibrary.Get(id);
                Assert.IsNotNull(recipe, id);
                Assert.AreEqual(VfxKind.Motes, recipe.kind, id);
                Assert.IsFalse(recipe.oneShot, id);
            }
        }

        [Test]
        public void MarkerAuthor_IsIdempotentCapsAtThreeAndAddsNoPhysicalComponents()
        {
            _root = new GameObject("AmbientMoteTestRoot");
            Vector3[] positions =
            {
                new Vector3(1f, 1.45f, 2f),
                new Vector3(3f, 1.45f, 4f),
                new Vector3(5f, 1.45f, 6f),
                new Vector3(7f, 1.45f, 8f),
            };

            AmbientMoteAuthor.PlaceMarkers(_root.transform, "motes_amber", positions);
            Transform authored = _root.transform.Find(AmbientMoteAuthor.RootName);
            Assert.IsNotNull(authored);
            Assert.AreEqual(AmbientMoteAuthor.MaxEmitters, authored.childCount);

            for (int i = 0; i < authored.childCount; i++)
            {
                GameObject marker = authored.GetChild(i).gameObject;
                var volume = marker.GetComponent<AmbientMoteVolume>();
                Assert.IsNotNull(volume);
                Assert.AreEqual("motes_amber", volume.RecipeId);
                Assert.AreEqual(positions[i], marker.transform.position);
                Assert.IsFalse(marker.isStatic);
                Assert.IsNull(marker.GetComponent<Collider>());
                Assert.IsNull(marker.GetComponent<Renderer>());
                Assert.IsNull(marker.GetComponent<Light>());
                Assert.IsNull(marker.GetComponent<Rigidbody>());
            }

            AmbientMoteAuthor.PlaceMarkers(
                _root.transform,
                "motes_spore",
                new[] { Vector3.up * 2f });

            Transform rebuilt = _root.transform.Find(AmbientMoteAuthor.RootName);
            Assert.IsNotNull(rebuilt);
            Assert.AreEqual(1, rebuilt.childCount);
            Assert.AreEqual(1, _root.transform.Cast<Transform>()
                .Count(child => child.name == AmbientMoteAuthor.RootName));
            Assert.AreEqual("motes_spore",
                rebuilt.GetChild(0).GetComponent<AmbientMoteVolume>().RecipeId);
        }

        [Test]
        public void RuntimeVolume_RejectsOneShotAndReturnsLoopingReservation()
        {
            _root = new GameObject("AmbientMoteRuntimeTest");
            var volume = _root.AddComponent<AmbientMoteVolume>();

            volume.Configure("impact_metal");
            Assert.IsFalse(volume.TryStart());
            Assert.IsNull(volume.System);
            Assert.AreEqual(0, VfxFactory.ActiveCount);

            volume.Configure("motes_spore");
            Assert.IsTrue(volume.TryStart());
            Assert.IsNotNull(volume.System);
            Assert.IsTrue(volume.System.main.loop);
            Assert.IsTrue(volume.IsRunning);
            Assert.AreEqual(1, VfxFactory.ActiveCount);

            Assert.IsTrue(volume.StopVolume());
            Assert.IsNull(volume.System);
            Assert.IsFalse(volume.IsRunning);
            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.AreEqual(1, VfxFactory.PooledCount);
        }

        [Test]
        public void WorldDressingBuilder_HasExactlyOneAmbientAuthorCall()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "WorldDressingBuilder.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);
            const string token = "AmbientMoteAuthor.Place(dressRoot, kit, route);";

            Assert.AreEqual(1, Count(source, token));
            int water = source.IndexOf("WaterAuthor.Place(dressRoot, kit);");
            int motes = source.IndexOf(token);
            Assert.Greater(motes, water,
                "ambient author should remain additive after the existing water pass");
        }

        [Test]
        public void RuntimeVolume_AutomaticLifecycleIsPlayModeOnlyAndStopsBothWays()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Visuals",
                "Runtime",
                "Vfx",
                "AmbientMoteVolume.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            StringAssert.Contains("if (Application.isPlaying) TryStart();", source);
            Assert.AreEqual(2, Count(source, "StopVolume();"),
                "OnDisable and OnDestroy both return the looping reservation");
            StringAssert.DoesNotContain("DontDestroyOnLoad", source);
            StringAssert.DoesNotContain("FindObjectOfType<Player", source);
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
