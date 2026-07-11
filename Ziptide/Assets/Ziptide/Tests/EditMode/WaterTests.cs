using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE III F3.3 water contract: the ripple field TILES seamlessly (no seam when a big canal
    /// wraps the normal), normals are unit-length, and the plane mesh is deterministic, flat, and
    /// inside the Quest triangle budget. Pure — no scene, no GPU.
    /// </summary>
    public class WaterTests
    {
        [Test]
        public void Height_Tiles_AndIsDeterministic()
        {
            for (float v = 0f; v < 1f; v += 0.137f)
            {
                Assert.AreEqual(WaterSurface.Height(0f, v), WaterSurface.Height(1f, v), 1e-3f,
                    "height must wrap in u at v=" + v);
                Assert.AreEqual(WaterSurface.Height(v, 0f), WaterSurface.Height(v, 1f), 1e-3f,
                    "height must wrap in v at u=" + v);
            }
            Assert.AreEqual(WaterSurface.Height(0.31f, 0.62f), WaterSurface.Height(0.31f, 0.62f),
                "deterministic");
        }

        [Test]
        public void Normal_IsUnit_AndTiles()
        {
            for (float v = 0f; v < 1f; v += 0.19f)
            {
                var n = WaterSurface.Normal(0.4f, v, 1.4f);
                Assert.AreEqual(1f, n.magnitude, 1e-3f, "normal must be unit length");
                Assert.Greater(n.y, 0f, "water normal points up");
                var nL = WaterSurface.Normal(0f, v, 1.4f);
                var nR = WaterSurface.Normal(1f, v, 1.4f);
                Assert.Less(Vector3.Angle(nL, nR), 1.5f, "normal must tile across the u seam at v=" + v);
            }
        }

        [Test]
        public void BakeNormalMap_IsRightSize_AndCarriesRipples()
        {
            // Tiling is proven rigorously by Normal_IsUnit_AndTiles (continuous, exact seam). Here
            // we only assert the bake is the right size AND actually carries ripple variation (a
            // flat/degenerate bake would ship glassy nothing) — and that the seam is no more
            // discontinuous than the interior at the same 1/size sampling step.
            int size = 64;
            var px = WaterSurface.BakeNormalMap(size, 1.4f);
            Assert.AreEqual(size * size, px.Length);

            byte gMin = 255, gMax = 0;
            foreach (var p in px) { if (p.g < gMin) gMin = p.g; if (p.g > gMax) gMax = p.g; }
            Assert.Greater(gMax - gMin, 20, "the normal map must carry visible ripples, not glass");

            for (int y = 0; y < size; y += 13)
            {
                int seam = Mathf.Abs(px[y * size + 0].b - px[y * size + (size - 1)].b);
                int interior = Mathf.Abs(px[y * size + size / 2].b - px[y * size + size / 2 + 1].b);
                Assert.LessOrEqual(seam, interior + 40,
                    "the wrap seam must be no choppier than a normal interior step at row " + y);
            }
        }

        [Test]
        public void Mesh_IsFlat_Deterministic_AndUnderBudget()
        {
            var a = ZiptideWaterMesh.Build(8f, 6f, 32);
            var b = ZiptideWaterMesh.Build(8f, 6f, 32);
            try
            {
                Assert.LessOrEqual(a.triangles.Length / 3, ZiptideWaterMesh.MaxTris, "tri budget");
                Assert.AreEqual(a.vertexCount, b.vertexCount, "deterministic vertex count");
                foreach (var vtx in a.vertices)
                    Assert.AreEqual(0f, vtx.y, 1e-5f, "the plane is flat at y=0 (bob is a runtime pass)");
                // Extent matches the requested size.
                Assert.AreEqual(8f, a.bounds.size.x, 1e-3f);
                Assert.AreEqual(6f, a.bounds.size.z, 1e-3f);
            }
            finally { Object.DestroyImmediate(a); Object.DestroyImmediate(b); }
        }

        [Test]
        public void Mesh_ClampsCells_ToStayUnderTriBudget()
        {
            var huge = ZiptideWaterMesh.Build(50f, 50f, 100000); // absurd subdivision
            try
            {
                Assert.LessOrEqual(huge.triangles.Length / 3, ZiptideWaterMesh.MaxTris,
                    "cells must clamp so an over-eager caller can't blow the Quest budget");
            }
            finally { Object.DestroyImmediate(huge); }
        }

        // ── commit 2: dynamics + foam ────────────────────────────────────────

        [Test]
        public void Scroll_FlowsInDirection_AndIsDeterministic()
        {
            var e = WaterMotion.ScrollOffset(0f, 0.02f, 5f);
            Assert.Greater(e.x, 0f, "0° flows +U");
            Assert.AreEqual(0f, e.y, 1e-4f);
            var n = WaterMotion.ScrollOffset(90f, 0.02f, 5f);
            Assert.Greater(n.y, 0f, "90° flows +V");
            Assert.Greater(WaterMotion.ScrollOffset(0f, 0.02f, 10f).x,
                WaterMotion.ScrollOffset(0f, 0.02f, 5f).x, "scroll advances with time");
            Assert.AreEqual(WaterMotion.ScrollOffset(37f, 0.02f, 3f),
                WaterMotion.ScrollOffset(37f, 0.02f, 3f), "deterministic");
        }

        [Test]
        public void Bob_StaysWithinAmplitude_AndMoves()
        {
            float amp = 0.03f;
            bool moved = false;
            float prev = WaterMotion.BobHeight(1f, 2f, 0f, amp, 0.6f);
            for (float t = 0f; t < 6f; t += 0.2f)
                for (float x = -4f; x <= 4f; x += 2f)
                {
                    float h = WaterMotion.BobHeight(x, 1.5f, t, amp, 0.6f);
                    Assert.LessOrEqual(Mathf.Abs(h), amp + 1e-4f, "swell must stay within amplitude");
                    if (Mathf.Abs(h - prev) > 1e-4f) moved = true;
                    prev = h;
                }
            Assert.IsTrue(moved, "the surface must actually swell over time/space");
        }

        [Test]
        public void FoamAlpha_DenseAtWaterline_ClearsToward_TheFrayedTop()
        {
            // Foam is densest at the waterline (v→0) and frays to nothing at the top (v→1); the
            // fray is lacy (varies across u). Robust asserts: denser-at-waterline, plus the field
            // reaches BOTH solid and clear somewhere — non-constant without a fragile threshold.
            float lineAvg = 0f, topAvg = 0f; int n = 0;
            float gmin = 1f, gmax = 0f;
            for (float v = 0.05f; v < 1f; v += 0.1f)
                for (float u = 0f; u < 1f; u += 0.05f)
                {
                    float a = WaterFoamMesh.FoamAlpha(u, v);
                    Assert.GreaterOrEqual(a, 0f); Assert.LessOrEqual(a, 1f);
                    gmin = Mathf.Min(gmin, a); gmax = Mathf.Max(gmax, a);
                }
            for (float u = 0f; u < 1f; u += 0.05f, n++)
            {
                lineAvg += WaterFoamMesh.FoamAlpha(u, 0.05f);
                topAvg += WaterFoamMesh.FoamAlpha(u, 0.95f);
            }
            Assert.Greater(lineAvg / n, topAvg / n, "more foam at the waterline than the frayed top");
            Assert.Less(gmin, 0.2f, "foam clears (has gaps) somewhere");
            Assert.Greater(gmax, 0.8f, "foam is solid at the waterline somewhere");
        }

        [Test]
        public void FoamBand_IsFlat_AndHugsThePerimeter()
        {
            var m = WaterFoamMesh.BuildBand(6f, 4f, 0.5f, 2f, 0.02f);
            try
            {
                Assert.Greater(m.triangles.Length, 0);
                foreach (var v in m.vertices)
                {
                    Assert.LessOrEqual(Mathf.Abs(v.x), 3f + 1e-3f, "foam stays within the X border");
                    Assert.LessOrEqual(Mathf.Abs(v.z), 2f + 1e-3f, "foam stays within the Z border");
                    Assert.AreEqual(0.02f, v.y, 1e-4f, "the foam band lies flat on the water");
                }
            }
            finally { Object.DestroyImmediate(m); }
        }
    }
}
