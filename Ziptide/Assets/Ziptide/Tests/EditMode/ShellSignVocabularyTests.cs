#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class ShellSignVocabularyTests
    {
        [Test]
        public void GlyphBake_IsDeterministicNonemptyAndBounded()
        {
            foreach (SignDestinationClass destination in System.Enum.GetValues(typeof(SignDestinationClass)))
            {
                byte[] first = ShellGlyphBaker.BakeAlpha(64, 12345, destination);
                byte[] second = ShellGlyphBaker.BakeAlpha(64, 12345, destination);

                CollectionAssert.AreEqual(first, second, destination.ToString());
                Assert.AreEqual(64 * 64, first.Length);

                float coverage = ShellGlyphBaker.Coverage01(first);
                Assert.That(coverage, Is.InRange(0.035f, 0.30f),
                    destination + " glyph coverage should read as writing, not blank/noise");
            }
        }

        [Test]
        public void DifferentSeedsAndDestinationMarks_ProduceDifferentMasks()
        {
            byte[] seedA = ShellGlyphBaker.BakeAlpha(64, 11, SignDestinationClass.Job);
            byte[] seedB = ShellGlyphBaker.BakeAlpha(64, 12, SignDestinationClass.Job);
            byte[] travel = ShellGlyphBaker.BakeAlpha(64, 11, SignDestinationClass.Travel);
            byte[] vendor = ShellGlyphBaker.BakeAlpha(64, 11, SignDestinationClass.Vendor);

            Assert.IsFalse(seedA.SequenceEqual(seedB));
            Assert.IsFalse(seedA.SequenceEqual(travel));
            Assert.IsFalse(seedA.SequenceEqual(vendor));
            Assert.IsFalse(travel.SequenceEqual(vendor));
        }

        [Test]
        public void Bake_ClampsSizeAndEmitsWhiteAlphaPixels()
        {
            Color32[] tooSmall = ShellGlyphBaker.Bake(1, 9, SignDestinationClass.Travel);
            Color32[] tooLarge = ShellGlyphBaker.Bake(1000, 9, SignDestinationClass.Travel);

            Assert.AreEqual(ShellGlyphBaker.MinimumSize * ShellGlyphBaker.MinimumSize, tooSmall.Length);
            Assert.AreEqual(ShellGlyphBaker.MaximumSize * ShellGlyphBaker.MaximumSize, tooLarge.Length);
            Assert.IsTrue(tooSmall.Any(pixel => pixel.a > 0));
            foreach (Color32 pixel in tooSmall.Where(pixel => pixel.a > 0))
            {
                Assert.AreEqual(255, pixel.r);
                Assert.AreEqual(255, pixel.g);
                Assert.AreEqual(255, pixel.b);
            }
        }

        [Test]
        public void CanonicalHues_AreExactValidRgbHexStrings()
        {
            var hues = new Dictionary<SignDestinationClass, string>
            {
                { SignDestinationClass.Travel, ZiptideConstants.SignHueTravelHex },
                { SignDestinationClass.Job, ZiptideConstants.SignHueJobHex },
                { SignDestinationClass.Vendor, ZiptideConstants.SignHueVendorHex },
            };

            Assert.AreEqual("35D9E6", hues[SignDestinationClass.Travel]);
            Assert.AreEqual("E6A13A", hues[SignDestinationClass.Job]);
            Assert.AreEqual("55C879", hues[SignDestinationClass.Vendor]);

            foreach (var pair in hues)
            {
                Assert.AreEqual(6, pair.Value.Length);
                Assert.IsTrue(ColorUtility.TryParseHtmlString("#" + pair.Value, out Color color), pair.Key.ToString());
                Assert.Greater(color.maxColorComponent, 0.70f);
            }
        }

        [Test]
        public void AllThreeSignBodies_ValidateBuildAndStayInsideBudget()
        {
            List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>> specs = SignRecipeLibrary.Specs();
            Assert.AreEqual(3, specs.Count);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    SignRecipeLibrary.HangingRecipeId,
                    SignRecipeLibrary.WallPlateRecipeId,
                    SignRecipeLibrary.ChevronRecipeId,
                },
                specs.Select(spec => spec.Key));

            foreach (var spec in specs)
            {
                ForgeRecipeDefinition recipe = spec.Value();
                Assert.AreEqual(spec.Key, recipe.recipeId);
                Assert.AreEqual(SignRecipeLibrary.SignTriangleBudget, recipe.budgetTris);
                CollectionAssert.Contains(recipe.storyTags, "prop");
                CollectionAssert.Contains(recipe.storyTags, "sign");
                Assert.IsEmpty(recipe.Validate(), spec.Key + " validation");

                int triangles = ForgeMesh.CountTriangles(recipe);
                Assert.Greater(triangles, 0, spec.Key);
                Assert.LessOrEqual(triangles, SignRecipeLibrary.SignTriangleBudget, spec.Key);
                Assert.LessOrEqual(triangles, ForgeRecipeDefinition.ClassBudgetFor(recipe.storyTags), spec.Key);

                Mesh mesh = ForgeMesh.Build(recipe);
                Assert.Greater(mesh.vertexCount, 0, spec.Key);
                Object.DestroyImmediate(mesh);
                Object.DestroyImmediate(recipe);
            }
        }

        [Test]
        public void SignageSources_UseNoRealFontOrInteractiveUiApis()
        {
            string glyphPath = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Visuals",
                "Runtime",
                "Signage",
                "ShellGlyphBaker.cs");
            string recipePath = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "SignRecipeLibrary.cs");
            Assert.IsTrue(File.Exists(glyphPath), glyphPath);
            Assert.IsTrue(File.Exists(recipePath), recipePath);

            string glyphSource = File.ReadAllText(glyphPath);
            string recipeSource = File.ReadAllText(recipePath);
            string allSource = glyphSource + recipeSource;
            StringAssert.DoesNotContain("TextMesh", allSource);
            StringAssert.DoesNotContain("TMP_", allSource);
            StringAssert.DoesNotContain("Font", allSource);
            StringAssert.DoesNotContain("XRBaseInteractable", allSource);
            StringAssert.DoesNotContain("XRSimpleInteractable", allSource);

            // Runtime remains pure: the baker only returns pixel arrays. Commit 2 deliberately moved
            // persistent texture/material creation into the create-only Editor asset author.
            StringAssert.DoesNotContain("new Texture2D", glyphSource);
            StringAssert.Contains("new Texture2D", recipeSource);
            StringAssert.Contains("AssetDatabase.CreateAsset(texture, texturePath);", recipeSource);
            StringAssert.Contains("AssetDatabase.CreateAsset(material, materialPath);", recipeSource);
        }

        [Test]
        public void BuildAndroid_HooksSignRecipeProducerExactlyOnce()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Build",
                "BuildAndroid.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);
            const string token = "SignRecipeLibrary.EnsureAllAuthored();";
            Assert.AreEqual(1, Count(source, token));
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
