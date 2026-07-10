using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Art;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// E5.1 textured building kit: the wall recipes honor the primitive kit's canonical module
    /// envelope (3.0 × 3.2 × 0.25, origin at wall center — layout/collider parity), and the registry
    /// upgrade WRAPS the primitive factory (module still structurally primitive + carries the runtime
    /// ForgeModuleLook swap; the interior Pane survives on window modules).
    /// </summary>
    public class ForgeBuildingKitTests
    {
        private static readonly string[] WallRecipeIds =
        {
            "bldg_salvage_wall_solid", "bldg_salvage_wall_window",
            "bldg_tenement_wall_solid", "bldg_tenement_wall_window",
        };

        private static ForgeRecipeDefinition Recipe(string id)
        {
            foreach (var spec in Ziptide.Editor.Patching.ForgeRecipeLibrary.Specs())
                if (spec.Key == id) return spec.Value();
            Assert.Fail("recipe " + id + " missing from ForgeRecipeLibrary.Specs()");
            return null;
        }

        [Test]
        public void WallRecipes_ExistAndValidateClean()
        {
            foreach (var id in WallRecipeIds)
            {
                var r = Recipe(id);
                var issues = r.Validate();
                Assert.IsEmpty(issues, id + ": " + string.Join(" | ", issues));
            }
        }

        [Test]
        public void WallRecipes_HonorTheCanonicalModuleEnvelope()
        {
            foreach (var id in WallRecipeIds)
            {
                var mesh = ForgeMesh.Build(Recipe(id));
                Vector3 size = mesh.bounds.size;
                Vector3 center = mesh.bounds.center;
                Assert.AreEqual(3.0f, size.x, 0.02f, id + " must span the 3.0m module width (rib to rib)");
                Assert.AreEqual(3.2f, size.y, 0.02f, id + " must span the 3.2m storey height");
                Assert.Less(size.z, 0.9f, id + " protrudes too far off the wall plane");
                Assert.AreEqual(0f, center.x, 0.15f, id + " off-center on X");
                Assert.AreEqual(0f, center.y, 0.05f, id + " off-center on Y (origin = wall center)");
            }
        }

        [Test]
        public void WindowRecipes_LeaveTheRevealOpen_ForTheInteriorPane()
        {
            // No geometry may cover the reveal center (0, 0.08) at the panel plane — the
            // interior-mapped Pane child renders there. Probe: no vertex inside a small box
            // around the reveal center within the panel depth.
            foreach (var id in new[] { "bldg_salvage_wall_window", "bldg_tenement_wall_window" })
            {
                var mesh = ForgeMesh.Build(Recipe(id));
                foreach (var v in mesh.vertices)
                {
                    bool inHole = Mathf.Abs(v.x) < 0.3f && Mathf.Abs(v.y - 0.08f) < 0.2f && Mathf.Abs(v.z) < 0.09f;
                    Assert.IsFalse(inHole, id + " has geometry inside the window reveal at " + v);
                }
            }
        }

        [Test]
        public void Registry_WrapsThePrimitive_WithTheRuntimeSwap()
        {
            ArtModuleRegistry.Clear();
            BuildingKitLibrary.EnsureRegistered(); // chains ForgeBuildingKit at its tail

            foreach (var styleId in new[] { "salvage_row", "toxic_tenement" })
                foreach (var window in new[] { false, true })
                {
                    string id = "buildingModule:" + styleId + "/" + (window ? "WallWindow" : "WallSolid");
                    Assert.IsTrue(ArtModuleRegistry.TryBuild(id, out var go), id + " unfulfilled");
                    try
                    {
                        var look = go.GetComponent<ForgeModuleLook>();
                        Assert.IsNotNull(look, id + " lost the runtime texture swap");
                        Assert.AreEqual(
                            Ziptide.Editor.Patching.ForgeRecipeLibrary.WallRecipeId(styleId, window),
                            look.recipeId, id + " wired to the wrong recipe");
                        Assert.Greater(go.transform.childCount, 2,
                            id + " lost the structured primitive fallback (editor/no-bake look)");
                        if (window)
                            Assert.IsNotNull(FindDeep(go.transform, "Pane"),
                                id + " lost the interior-mapped pane");
                    }
                    finally { Object.DestroyImmediate(go); }
                }
        }

        [Test]
        public void ReRegistration_NeverDoubleWraps()
        {
            ArtModuleRegistry.Clear();
            BuildingKitLibrary.EnsureRegistered();
            BuildingKitLibrary.EnsureRegistered(); // a second pass re-seeds primitives, then re-wraps

            Assert.IsTrue(ArtModuleRegistry.TryBuild("buildingModule:salvage_row/WallSolid", out var go));
            try
            {
                Assert.AreEqual(1, go.GetComponents<ForgeModuleLook>().Length,
                    "re-registering stacked duplicate swap components");
            }
            finally { Object.DestroyImmediate(go); }
        }

        private static Transform FindDeep(Transform root, string name)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t;
            return null;
        }
    }
}
