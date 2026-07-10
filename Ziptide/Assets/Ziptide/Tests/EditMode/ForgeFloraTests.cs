using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// E5.3 flora contract: the LeafCard op emits exactly the two crossed double-sided quads it
    /// promises, the Leaf style's baked alpha actually cuts a leaf (stalk + blade in, margin +
    /// beyond-tip out), the sway math stays bounded/deterministic/desynced, and the alpha-clip
    /// material trigger fires only on Leaf-styled recipes.
    /// </summary>
    public class ForgeFloraTests
    {
        [Test]
        public void LeafCard_EmitsTwoCrossedDoubleSidedQuads()
        {
            var g = ForgeMesh.BuildPart(new ForgePart
            {
                op = ForgeOp.LeafCard, size = new Vector3(0.4f, 0.9f, 0f)
            });
            Assert.AreEqual(16, g.vertices.Count, "4 quads (2 planes × 2 sides) × 4 verts");
            Assert.AreEqual(24, g.triangles.Count, "8 tris — the ≤2-overdraw budget shape");

            // Both planes present, both sides wound: face normals must cover ±Z and ±X.
            bool pz = false, nz = false, px = false, nx = false;
            for (int t = 0; t < g.triangles.Count; t += 3)
            {
                Vector3 a = g.vertices[g.triangles[t]];
                Vector3 n = Vector3.Cross(g.vertices[g.triangles[t + 1]] - a,
                    g.vertices[g.triangles[t + 2]] - a).normalized;
                if (n.z > 0.9f) pz = true;
                if (n.z < -0.9f) nz = true;
                if (n.x > 0.9f) px = true;
                if (n.x < -0.9f) nx = true;
            }
            Assert.IsTrue(pz && nz && px && nx, "card must render from every side "
                + "(+Z=" + pz + " -Z=" + nz + " +X=" + px + " -X=" + nx + ")");

            // Grows from the ground: base at y=0, top at size.y.
            float minY = float.MaxValue, maxY = float.MinValue;
            foreach (var v in g.vertices) { minY = Mathf.Min(minY, v.y); maxY = Mathf.Max(maxY, v.y); }
            Assert.AreEqual(0f, minY, 1e-5f, "base sits at y=0");
            Assert.AreEqual(0.9f, maxY, 1e-5f, "top reaches size.y");
        }

        [Test]
        public void LeafCard_SizeZUnused_ValidatesClean()
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "test_leaf";
            r.palette = new[] { Color.green };
            r.parts = new[] { new ForgePart { op = ForgeOp.LeafCard, size = new Vector3(0.3f, 0.8f, 0f) } };
            Assert.IsEmpty(r.Validate(), "LeafCard must not require a z extent");
            Object.DestroyImmediate(r);
        }

        [Test]
        public void LeafAlpha_CutsALeaf_StalkAndBladeIn_MarginAndTipOut()
        {
            Assert.AreEqual(1f, ForgeTexture.LeafAlpha01(0.5f, 0.05f), 0.01f, "the stalk is solid");
            Assert.AreEqual(1f, ForgeTexture.LeafAlpha01(0.5f, 0.45f), 0.01f, "the blade center is solid");
            Assert.AreEqual(0f, ForgeTexture.LeafAlpha01(0.02f, 0.45f), 0.01f, "outside the margin is cut");
            Assert.AreEqual(0f, ForgeTexture.LeafAlpha01(0.6f, 0.995f), 0.01f, "past the pointed tip is cut");
            Assert.AreEqual(ForgeTexture.LeafAlpha01(0.41f, 0.6f), ForgeTexture.LeafAlpha01(0.41f, 0.6f),
                "deterministic");
        }

        [Test]
        public void Sway_Bounded_Deterministic_SeedsDesync()
        {
            const float deg = 4f;
            float maxAngle = 0f;
            bool moved = false, desynced = false;
            Quaternion prev = ForgeSway.SwayRotation(9, 0f, deg, 0.4f);
            for (float t = 0f; t < 6f; t += 0.21f)
            {
                var q = ForgeSway.SwayRotation(9, t, deg, 0.4f);
                maxAngle = Mathf.Max(maxAngle, Quaternion.Angle(q, Quaternion.identity));
                if (Quaternion.Angle(q, prev) > 0.2f) moved = true;
                prev = q;
                if (Quaternion.Angle(q, ForgeSway.SwayRotation(700, t, deg, 0.4f)) > 0.2f) desynced = true;
            }
            Assert.LessOrEqual(maxAngle, deg * 1.35f, "sway lean stays bounded");
            Assert.IsTrue(moved, "the plant must actually move");
            Assert.IsTrue(desynced, "different seeds must desync a scattered field");
            Assert.AreEqual(ForgeSway.SwayRotation(9, 1.3f, deg, 0.4f),
                ForgeSway.SwayRotation(9, 1.3f, deg, 0.4f), "deterministic");
        }

        [Test]
        public void HasLeafSlot_FiresOnlyOnLeafStyledRecipes()
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "test_leaf_mat";
            r.palette = new[] { Color.green, Color.gray };
            r.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Bark },
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal },
            };
            Assert.IsFalse(ForgeTexture.HasLeafSlot(r), "no Leaf slot yet");
            r.slotStyles[0].style = ForgeStyle.Leaf;
            Assert.IsTrue(ForgeTexture.HasLeafSlot(r), "Leaf slot must trigger the cutout material");
            Object.DestroyImmediate(r);
        }
    }
}
