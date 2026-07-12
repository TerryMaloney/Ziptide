#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class ShellSignPlacementTests
    {
        private GameObject _root;
        private readonly List<Material> _materials = new List<Material>();

        [TearDown]
        public void TearDown()
        {
            if (_root != null) Object.DestroyImmediate(_root);
            foreach (Material material in _materials)
                if (material != null) Object.DestroyImmediate(material);
            _materials.Clear();
            _root = null;
        }

        [Test]
        public void PoiVocabulary_MapsToThreeLockedClassesAndBodies()
        {
            Assert.AreEqual(SignDestinationClass.Travel,
                SignAuthor.DestinationClassFor(PoiType.TravelBerth));
            Assert.AreEqual(SignRecipeLibrary.HangingRecipeId,
                SignAuthor.RecipeFor(PoiType.TravelBerth));

            foreach (PoiType type in new[]
                     {
                         PoiType.CombatCamp,
                         PoiType.HarvestGrove,
                         PoiType.MachineSite,
                         PoiType.StoryAnchor,
                     })
            {
                Assert.AreEqual(SignDestinationClass.Job, SignAuthor.DestinationClassFor(type), type.ToString());
                Assert.AreEqual(SignRecipeLibrary.WallPlateRecipeId, SignAuthor.RecipeFor(type), type.ToString());
            }

            foreach (PoiType type in new[] { PoiType.RuinCache, PoiType.CaveSecret })
            {
                Assert.AreEqual(SignDestinationClass.Vendor, SignAuthor.DestinationClassFor(type), type.ToString());
                Assert.AreEqual(SignRecipeLibrary.ChevronRecipeId, SignAuthor.RecipeFor(type), type.ToString());
            }
        }

        [Test]
        public void StableSeed_IsDeterministicAndSensitiveToIdentity()
        {
            int first = SignAuthor.StableSeed(42, "poi_alpha", PoiType.StoryAnchor, 0);
            Assert.AreEqual(first, SignAuthor.StableSeed(42, "poi_alpha", PoiType.StoryAnchor, 0));
            Assert.AreNotEqual(first, SignAuthor.StableSeed(43, "poi_alpha", PoiType.StoryAnchor, 0));
            Assert.AreNotEqual(first, SignAuthor.StableSeed(42, "poi_beta", PoiType.StoryAnchor, 0));
            Assert.AreNotEqual(first, SignAuthor.StableSeed(42, "poi_alpha", PoiType.RuinCache, 0));
            Assert.AreNotEqual(first, SignAuthor.StableSeed(42, "poi_alpha", PoiType.StoryAnchor, 1));
        }

        [Test]
        public void SharedPanelAssets_AreOpaqueDistinctAndPathStable()
        {
            var images = new Dictionary<SignDestinationClass, Color32[]>();
            foreach (SignDestinationClass destination in System.Enum.GetValues(typeof(SignDestinationClass)))
            {
                Color32[] pixels = SignRecipeLibrary.BuildPanelPixels(destination);
                images.Add(destination, pixels);
                Assert.AreEqual(SignRecipeLibrary.GlyphTextureSize * SignRecipeLibrary.GlyphTextureSize,
                    pixels.Length);
                Assert.IsTrue(pixels.All(pixel => pixel.a == 255), destination + " panel must remain opaque");
                Assert.Greater(pixels.Select(pixel => pixel.r + pixel.g + pixel.b).Distinct().Count(), 8,
                    destination + " panel needs real glyph contrast");
                StringAssert.EndsWith("shell_glyph_" + destination.ToString().ToLowerInvariant() + ".asset",
                    SignRecipeLibrary.GlyphTexturePath(destination));
                StringAssert.EndsWith("shell_glyph_" + destination.ToString().ToLowerInvariant() + ".mat",
                    SignRecipeLibrary.GlyphMaterialPath(destination));
            }

            Assert.IsFalse(images[SignDestinationClass.Travel]
                .SequenceEqual(images[SignDestinationClass.Job]));
            Assert.IsFalse(images[SignDestinationClass.Job]
                .SequenceEqual(images[SignDestinationClass.Vendor]));
        }

        [Test]
        public void PlacePlans_IsIdempotentCapsAtSixAndUsesSharedMaterials()
        {
            _root = new GameObject("ShellSignPlacementRoot");
            Material travel = NewMaterial("Travel");
            Material job = NewMaterial("Job");
            Material vendor = NewMaterial("Vendor");
            var byClass = new Dictionary<SignDestinationClass, Material>
            {
                { SignDestinationClass.Travel, travel },
                { SignDestinationClass.Job, job },
                { SignDestinationClass.Vendor, vendor },
            };

            var plans = new List<SignAuthor.SignPlan>();
            for (int i = 0; i < 8; i++)
            {
                SignDestinationClass destination = (SignDestinationClass)(i % 3);
                plans.Add(new SignAuthor.SignPlan
                {
                    name = "Sign_" + i,
                    recipeId = destination == SignDestinationClass.Travel
                        ? SignRecipeLibrary.HangingRecipeId
                        : destination == SignDestinationClass.Job
                            ? SignRecipeLibrary.WallPlateRecipeId
                            : SignRecipeLibrary.ChevronRecipeId,
                    destinationClass = destination,
                    position = new Vector3(i, 1.65f, i * 2f),
                    forward = Vector3.back,
                    seed = i
                });
            }

            int placed = SignAuthor.PlacePlans(
                _root.transform,
                plans,
                destination => byClass[destination]);
            Assert.AreEqual(SignAuthor.MaxSigns, placed);

            Transform authored = _root.transform.Find(SignAuthor.RootName);
            Assert.IsNotNull(authored);
            Assert.AreEqual(SignAuthor.MaxSigns, authored.childCount);
            for (int i = 0; i < authored.childCount; i++)
            {
                Transform sign = authored.GetChild(i);
                var look = sign.GetComponent<ForgeModuleLook>();
                Assert.IsNotNull(look, sign.name);
                CollectionAssert.Contains(look.keepChildren, SignAuthor.GlyphChildName);
                Assert.IsNull(sign.GetComponentInChildren<Collider>(), sign.name);
                Assert.IsNull(sign.GetComponentInChildren<Light>(), sign.name);
                Assert.IsNull(sign.GetComponentInChildren<TextMesh>(), sign.name);

                Transform glyph = sign.Find(SignAuthor.GlyphChildName);
                Assert.IsNotNull(glyph, sign.name);
                Renderer renderer = glyph.GetComponent<Renderer>();
                Assert.IsNotNull(renderer, sign.name);
                Assert.AreSame(byClass[plans[i].destinationClass], renderer.sharedMaterial, sign.name);
                Assert.IsFalse(renderer.receiveShadows);
            }

            int rebuilt = SignAuthor.PlacePlans(
                _root.transform,
                new[] { plans[0] },
                destination => byClass[destination]);
            Assert.AreEqual(1, rebuilt);
            Assert.AreEqual(1, _root.transform.Cast<Transform>()
                .Count(child => child.name == SignAuthor.RootName));
            Assert.AreEqual(1, _root.transform.Find(SignAuthor.RootName).childCount);
        }

        [Test]
        public void WorldDressing_WiresSignAuthorExactlyOnceAfterAtmosphere()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "WorldDressingBuilder.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);
            const string signToken = "SignAuthor.Place(dressRoot, kit, route);";
            Assert.AreEqual(1, Count(source, signToken));
            Assert.Greater(source.IndexOf(signToken),
                source.IndexOf("AmbientMoteAuthor.Place(dressRoot, kit, route);"));
        }

        [Test]
        public void SignAuthor_IntroducesNoInteractiveOrRealLightOwner()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "SignAuthor.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            StringAssert.DoesNotContain("TextMesh", source);
            StringAssert.DoesNotContain("TMP_", source);
            StringAssert.DoesNotContain("XRBaseInteractable", source);
            StringAssert.DoesNotContain("XRSimpleInteractable", source);
            StringAssert.DoesNotContain("AddComponent<Light>", source);
            StringAssert.DoesNotContain("DontDestroyOnLoad", source);
            StringAssert.Contains("sharedMaterial = glyphMaterial", source);
        }

        private Material NewMaterial(string name)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            Assert.IsNotNull(shader);
            var material = new Material(shader) { name = name };
            _materials.Add(material);
            return material;
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
