using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The studio catalog's contract: every recipe in ForgeRecipeLibrary validates, builds, stays
    /// inside its triangle budget, keeps its palette on-brand for its surface family, and (for
    /// handhelds) honors the Grip/Muzzle socket contract with the Quest grip tilt.
    /// </summary>
    public class ForgeRecipeLibraryTests
    {
        private static Dictionary<string, ForgeRecipeDefinition> Catalog()
        {
            var built = new Dictionary<string, ForgeRecipeDefinition>();
            foreach (var spec in Ziptide.Editor.Patching.ForgeRecipeLibrary.Specs())
                built[spec.Key] = spec.Value();
            return built;
        }

        [Test]
        public void EveryRecipe_HasUniqueMatchingId()
        {
            var ids = new HashSet<string>();
            foreach (var kv in Catalog())
            {
                Assert.AreEqual(kv.Key, kv.Value.recipeId, "spec key and recipeId must match");
                Assert.IsTrue(ids.Add(kv.Value.recipeId), "duplicate recipeId " + kv.Value.recipeId);
            }
            Assert.Greater(ids.Count, 0);
        }

        [Test]
        public void EveryRecipe_ValidatesClean()
        {
            foreach (var kv in Catalog())
            {
                var issues = kv.Value.Validate();
                Assert.IsEmpty(issues, kv.Key + ": " + string.Join(" | ", issues));
            }
        }

        [Test]
        public void EveryRecipe_BuildsWithinItsTriangleBudget()
        {
            foreach (var kv in Catalog())
            {
                var mesh = ForgeMesh.Build(kv.Value);
                Assert.Greater(mesh.vertexCount, 0, kv.Key + " built an empty mesh");
                int tris = ForgeMesh.CountTriangles(kv.Value);
                Assert.LessOrEqual(tris, kv.Value.budgetTris,
                    kv.Key + " is over budget: " + tris + "/" + kv.Value.budgetTris + " tris");
            }
        }

        [Test]
        public void EveryRecipe_PaletteConformsToItsSurfaceFamily()
        {
            foreach (var kv in Catalog())
                foreach (var c in kv.Value.palette)
                    Assert.IsTrue(ForgePalettes.IsAllowed(kv.Value.surfaceFamily, c),
                        kv.Key + " palette color " + c + " breaks the " + kv.Value.surfaceFamily + " family law");
        }

        [Test]
        public void HandheldRecipes_HonorTheGripContract()
        {
            foreach (var kv in Catalog())
            {
                if (kv.Value.storyTags == null || System.Array.IndexOf(kv.Value.storyTags, "handheld") < 0) continue;
                ForgeSocket grip = null, muzzle = null;
                foreach (var s in kv.Value.sockets)
                {
                    if (s.name == "Grip") grip = s;
                    if (s.name == "Muzzle") muzzle = s;
                }
                Assert.IsNotNull(grip, kv.Key + " handheld without a Grip socket");
                Assert.IsNotNull(muzzle, kv.Key + " handheld without a Muzzle socket");
                // The Quest controller forward-tilt fix (ASSET_SWAP_PIPELINE §4): 40..50° on X.
                Assert.That(grip.localEuler.x, Is.InRange(40f, 50f),
                    kv.Key + " Grip socket must bake the ~45° controller tilt");
                // The muzzle must sit forward of the grip (+Z barrel convention).
                Assert.Greater(muzzle.localPosition.z, grip.localPosition.z, kv.Key + " muzzle not forward of grip");
            }
        }

        [Test]
        public void TaserMk1_IsHandSized()
        {
            var taser = Catalog()["taser_gun_mk1"];
            var mesh = ForgeMesh.Build(taser);
            Vector3 size = mesh.bounds.size;
            Assert.Less(Mathf.Max(size.x, Mathf.Max(size.y, size.z)), 0.35f, "taser bigger than a forearm");
            Assert.Greater(size.z, 0.15f, "taser too stubby to read as a pistol");
            Assert.Greater(size.y, 0.1f, "taser has no grip drop");
        }

        [Test]
        public void EveryRecipe_UsesAtMostSixSharedMaterials()
        {
            foreach (var kv in Catalog())
                Assert.LessOrEqual(ForgeMesh.UsedPaletteSlots(kv.Value).Count, 6, kv.Key);
        }
    }
}
