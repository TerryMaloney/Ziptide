using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE III F3.4 grounding-decal contract: the blob shadow is a clean radial falloff (opaque
    /// centre, clear rim) and the stain is an irregular grime field that's denser at the centre and
    /// reaches both solid and clear — proving it's mottled, not a flat disc. Pure; no scene.
    /// </summary>
    public class GroundingTests
    {
        [Test]
        public void Blob_IsOpaqueCentre_ClearRim_Monotonic()
        {
            Assert.AreEqual(1f, GroundDecal.BlobAlpha(0.5f, 0.5f), 1e-4f, "opaque at the centre");
            Assert.AreEqual(0f, GroundDecal.BlobAlpha(0f, 0.5f), 1e-4f, "clear at the rim");
            Assert.AreEqual(0f, GroundDecal.BlobAlpha(0f, 0f), 1e-4f, "clear at the corner");
            float a = GroundDecal.BlobAlpha(0.5f, 0.5f);
            float b = GroundDecal.BlobAlpha(0.62f, 0.5f);
            float c = GroundDecal.BlobAlpha(0.74f, 0.5f);
            Assert.Greater(a, b); Assert.Greater(b, c); // monotonic falloff outward
        }

        [Test]
        public void Stain_IsIrregular_DenserAtCentre()
        {
            float centreAvg = 0f, rimAvg = 0f; int n = 0;
            float gmin = 1f, gmax = 0f;
            for (float t = 0f; t < 1f; t += 0.1f, n++)
            {
                centreAvg += GroundDecal.StainAlpha(0.42f + 0.16f * t, 0.5f); // near-centre band
                rimAvg += GroundDecal.StainAlpha(0.02f + 0.06f * t, 0.5f);    // far rim
            }
            for (float v = 0.1f; v < 1f; v += 0.1f)
                for (float u = 0.1f; u < 1f; u += 0.1f)
                {
                    float a = GroundDecal.StainAlpha(u, v);
                    Assert.GreaterOrEqual(a, 0f); Assert.LessOrEqual(a, 1f);
                    gmin = Mathf.Min(gmin, a); gmax = Mathf.Max(gmax, a);
                }
            Assert.Greater(centreAvg / n, rimAvg / n, "grime pools toward the centre");
            Assert.Less(gmin, 0.1f, "the stain clears somewhere (mottled, not a disc)");
            Assert.Greater(gmax, 0.4f, "the stain is solid somewhere");
        }

        [Test]
        public void BakeAlpha_RightSize_BlobCentreVsCorner()
        {
            int size = 64;
            var px = GroundDecal.BakeAlpha(size, stain: false);
            Assert.AreEqual(size * size, px.Length);
            int c = (size / 2) * size + (size / 2);
            Assert.Greater(px[c].a, 200, "blob centre texel is nearly opaque");
            Assert.AreEqual(0, px[0].a, "blob corner texel is clear");
        }
    }
}
