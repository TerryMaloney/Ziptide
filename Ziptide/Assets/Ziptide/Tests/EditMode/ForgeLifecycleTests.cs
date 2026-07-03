using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Reconciliation R2 contract: the content hash is deterministic and sensitive to look-defining
    /// changes but blind to lifecycle bookkeeping; Locked requires a baked baseline; the catalog
    /// carries structured refs so staleness detection never parses prose.
    /// </summary>
    public class ForgeLifecycleTests
    {
        private static ForgeRecipeDefinition Recipe()
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "hash_test";
            r.palette = new[] { Color.red, Color.blue };
            r.parts = new[]
            {
                new ForgePart { name = "A", op = ForgeOp.BeveledBox, size = new Vector3(0.2f, 0.1f, 0.3f), bevel = 0.01f },
                new ForgePart { name = "B", op = ForgeOp.Cylinder, size = new Vector3(0.1f, 0.2f, 0.1f), segments = 8, paletteSlot = 1 },
            };
            r.sockets = new[] { new ForgeSocket { name = "Grip", localEuler = new Vector3(45f, 0f, 0f) },
                                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.2f) } };
            return r;
        }

        [Test]
        public void ContentHash_IsDeterministic()
        {
            Assert.AreEqual(Recipe().ComputeContentHash(), Recipe().ComputeContentHash());
        }

        [Test]
        public void ContentHash_ChangesWhenTheLookChanges()
        {
            string baseline = Recipe().ComputeContentHash();

            var sized = Recipe(); sized.parts[0].size = new Vector3(0.25f, 0.1f, 0.3f);
            Assert.AreNotEqual(baseline, sized.ComputeContentHash(), "part size must change the hash");

            var recolored = Recipe(); recolored.palette[0] = Color.green;
            Assert.AreNotEqual(baseline, recolored.ComputeContentHash(), "palette must change the hash");

            var resocketed = Recipe(); resocketed.sockets[0].localEuler = new Vector3(40f, 0f, 0f);
            Assert.AreNotEqual(baseline, resocketed.ComputeContentHash(), "socket pose must change the hash");
        }

        [Test]
        public void ContentHash_IgnoresLifecycleBookkeeping()
        {
            string baseline = Recipe().ComputeContentHash();
            var r = Recipe();
            r.qualityState = ForgeQualityState.ProductionReady;
            r.storyRole = "totally different prose";
            r.storyRefs = new[] { "something_else" };
            r.lockedContentHash = "deadbeefdeadbeef";
            Assert.AreEqual(baseline, r.ComputeContentHash(),
                "locking/refs/prose must not move the hash they pin");
        }

        [Test]
        public void Locked_WithoutBakedBaseline_FailsValidation()
        {
            var r = Recipe();
            r.qualityState = ForgeQualityState.Locked;
            r.lockedContentHash = "";
            Assert.IsNotEmpty(r.Validate());

            r.lockedContentHash = r.ComputeContentHash();
            Assert.IsEmpty(r.Validate(), "a properly baked lock validates clean");
        }

        [Test]
        public void CatalogRecipes_CarryStructuredRefs()
        {
            foreach (var spec in Ziptide.Editor.Patching.ForgeRecipeLibrary.Specs())
            {
                var r = spec.Value();
                Assert.IsTrue(r.storyRefs != null && r.storyRefs.Length > 0,
                    spec.Key + " needs storyRefs — the dependency auditor never parses prose");
                Assert.IsTrue(r.worldRuleRefs != null && r.worldRuleRefs.Length > 0,
                    spec.Key + " needs worldRuleRefs");
            }
        }
    }
}
