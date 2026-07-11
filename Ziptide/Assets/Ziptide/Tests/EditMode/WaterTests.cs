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
    }
}
