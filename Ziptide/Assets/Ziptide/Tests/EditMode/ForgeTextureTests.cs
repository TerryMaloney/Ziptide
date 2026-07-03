using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE II E1.2 contract: the metadata rasterizer covers the atlas deterministically with
    /// correct slots and sane analytic edge distances, and the albedo layers actually do their
    /// jobs (wear brightens edges, grime darkens down-faces, glow panels go dark for the emissive
    /// map, the bake is never flat). Pure — no GPU.
    /// </summary>
    public class ForgeTextureTests
    {
        private const int S = 128;

        private static ForgeRecipeDefinition TwoPartRecipe()
        {
            var r = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            r.recipeId = "tex_test";
            r.palette = new[] { new Color(0.5f, 0.3f, 0.2f), new Color(0.2f, 0.6f, 0.7f) };
            r.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.5f, grime = 0.5f, panelDensity = 2f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = Color.cyan, emissiveIntensity = 2f },
            };
            r.parts = new[]
            {
                new ForgePart { name = "Body", op = ForgeOp.BeveledBox, size = new Vector3(0.2f, 0.1f, 0.3f), bevel = 0.01f, paletteSlot = 0 },
                new ForgePart { name = "Glow", op = ForgeOp.Cylinder, size = new Vector3(0.08f, 0.15f, 0.08f), segments = 8, position = new Vector3(0f, 0.1f, 0f), paletteSlot = 1 },
            };
            return r;
        }

        [Test]
        public void BakeMeta_CoversBothIslands_WithCorrectSlots()
        {
            var r = TwoPartRecipe();
            var meta = ForgeTexture.BakeMeta(r, S);
            int slot0 = 0, slot1 = 0;
            foreach (var t in meta)
            {
                if (!t.covered) continue;
                if (t.slot == 0) slot0++;
                if (t.slot == 1) slot1++;
                Assert.IsTrue(t.edge01 >= 0f && t.edge01 <= 1f, "edge01 out of range");
                Assert.IsTrue(t.up01 >= 0f && t.up01 <= 1f, "up01 out of range");
            }
            Assert.Greater(slot0, 50, "part 0 barely rasterized");
            Assert.Greater(slot1, 50, "part 1 barely rasterized");
        }

        [Test]
        public void BakeMeta_IsDeterministic()
        {
            var r = TwoPartRecipe();
            var a = ForgeTexture.BakeMeta(r, S);
            var b = ForgeTexture.BakeMeta(r, S);
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i].covered, b[i].covered, "coverage diverged at " + i);
                if (a[i].covered)
                {
                    Assert.AreEqual(a[i].slot, b[i].slot);
                    Assert.AreEqual(a[i].edge01, b[i].edge01, 1e-6f);
                }
            }
        }

        [Test]
        public void EdgeDist_Box_ZeroOnEdges_OneAtFaceCenters()
        {
            var p = new ForgePart { op = ForgeOp.BeveledBox, size = new Vector3(0.2f, 0.2f, 0.2f) };
            var b = new Bounds(Vector3.zero, p.size);
            Assert.Less(ForgeTexture.EdgeDist01(p, new Vector3(0.1f, 0.1f, 0f), b), 0.05f, "on an edge");
            Assert.Greater(ForgeTexture.EdgeDist01(p, new Vector3(0.1f, 0f, 0f), b), 0.9f, "face center");
        }

        [Test]
        public void EdgeDist_Cylinder_ZeroOnRim_HighMidSideAndCapCenter()
        {
            var p = new ForgePart { op = ForgeOp.Cylinder, size = new Vector3(0.2f, 0.3f, 0.2f), segments = 10 };
            var b = new Bounds(Vector3.zero, new Vector3(0.2f, 0.3f, 0.2f));
            Assert.Less(ForgeTexture.EdgeDist01(p, new Vector3(0.1f, 0.15f, 0f), b), 0.05f, "on the rim");
            Assert.Greater(ForgeTexture.EdgeDist01(p, new Vector3(0.1f, 0f, 0f), b), 0.6f, "side midpoint is no edge");
            Assert.Greater(ForgeTexture.EdgeDist01(p, new Vector3(0f, 0.15f, 0f), b), 0.6f, "cap center is no edge");
        }

        [Test]
        public void Albedo_WearBrightensEdges_OnMetal()
        {
            var spec = new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 1f, grime = 0f };
            var baseCol = new Color(0.2f, 0.2f, 0.5f);
            var atEdge = new ForgeTexture.Texel { covered = true, u = 0.31f, v = 0.47f, edge01 = 0f, up01 = 0.5f };
            var atFace = atEdge; atFace.edge01 = 1f;
            float edgeLum = Lum(ForgeTexture.ComposeAlbedo(baseCol, spec, atEdge));
            float faceLum = Lum(ForgeTexture.ComposeAlbedo(baseCol, spec, atFace));
            Assert.Greater(edgeLum, faceLum + 0.05f, "edge wear must brighten toward bare metal");
        }

        [Test]
        public void Albedo_GrimeDarkensDownFaces()
        {
            var spec = new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0f, grime = 1f };
            var baseCol = new Color(0.5f, 0.5f, 0.5f);
            var down = new ForgeTexture.Texel { covered = true, u = 0.11f, v = 0.83f, edge01 = 1f, up01 = 0f };
            var up = down; up.up01 = 1f;
            Assert.Less(Lum(ForgeTexture.ComposeAlbedo(baseCol, spec, down)),
                Lum(ForgeTexture.ComposeAlbedo(baseCol, spec, up)) - 0.03f,
                "grime must settle on down-facing surfaces");
        }

        [Test]
        public void Albedo_GlowPanelGoesDark_ForTheEmissiveMap()
        {
            var spec = new ForgeStyleSpec { style = ForgeStyle.GlowPanel, wear = 0f, grime = 0f };
            var t = new ForgeTexture.Texel { covered = true, u = 0.5f, v = 0.5f, edge01 = 1f, up01 = 0.5f };
            Assert.Less(Lum(ForgeTexture.ComposeAlbedo(Color.cyan, spec, t)), Lum(Color.cyan) * 0.5f);
        }

        [Test]
        public void BakedAlbedo_IsNotFlat_AndDeterministic()
        {
            var r = TwoPartRecipe();
            var meta = ForgeTexture.BakeMeta(r, S);
            var a = new Color32[S * S];
            var b = new Color32[S * S];
            ForgeTexture.BakeAlbedo(r, meta, S, a);
            ForgeTexture.BakeAlbedo(r, ForgeTexture.BakeMeta(r, S), S, b);
            CollectionAssert.AreEqual(a, b, "albedo bake must be deterministic");

            // Not flat: covered texels of slot 0 must show real luminance variance (wear/grime/noise).
            float min = 1f, max = 0f;
            var meta2 = ForgeTexture.BakeMeta(r, S);
            for (int i = 0; i < a.Length; i++)
            {
                if (!meta2[i].covered || meta2[i].slot != 0) continue;
                float l = a[i].r / 255f * 0.3f + a[i].g / 255f * 0.6f + a[i].b / 255f * 0.1f;
                if (l < min) min = l;
                if (l > max) max = l;
            }
            Assert.Greater(max - min, 0.1f, "the bake is still flat — no surface information");
        }

        [Test]
        public void Voronoi_IsDeterministic_AndBounded()
        {
            for (int i = 0; i < 20; i++)
            {
                float v = ForgeTexture.Voronoi(i * 0.37f, i * 0.61f, 7);
                Assert.AreEqual(v, ForgeTexture.Voronoi(i * 0.37f, i * 0.61f, 7));
                Assert.IsTrue(v >= 0f && v < 2.9f, "voronoi out of range " + v);
            }
        }

        private static float Lum(Color c) => c.r * 0.3f + c.g * 0.6f + c.b * 0.1f;
    }
}
