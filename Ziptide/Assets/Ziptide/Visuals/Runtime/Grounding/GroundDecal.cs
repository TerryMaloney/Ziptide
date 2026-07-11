using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.4 — GROUNDING. The pure decal-coverage fields that kill the "everything floats"
    /// tell: a soft radial <see cref="BlobAlpha"/> (contact shadow under props / blob shadow under
    /// creatures + the player) and an irregular <see cref="StainAlpha"/> (grime pooling under pipes
    /// and at wall bases). Alpha only — the placement pass lays these as flat dark alpha-blended
    /// quads just above the ground. Deterministic and EditMode-testable; no depth texture, no
    /// projector (Quest-cheap).
    /// </summary>
    public static class GroundDecal
    {
        /// <summary>Soft radial darkening: 1 at the centre → 0 at the rim (r=0.5 in UV) and beyond.
        /// The contact/blob shadow.</summary>
        public static float BlobAlpha(float u, float v)
        {
            float dx = u - 0.5f, dy = v - 0.5f;
            float r = Mathf.Sqrt(dx * dx + dy * dy) * 2f; // 0 centre → 1 at the rim
            float a = Mathf.Clamp01(1f - r);
            return a * a; // quadratic soft edge
        }

        /// <summary>Irregular grime stain within a radius — fBm-mottled so it reads as a real spill,
        /// not a disc. 1 = grime, 0 = clean.</summary>
        public static float StainAlpha(float u, float v)
        {
            float dx = u - 0.5f, dy = v - 0.5f;
            float r = Mathf.Sqrt(dx * dx + dy * dy) * 2f;
            float edge = Mathf.Clamp01(1.15f - r);                       // fades out past the rim
            float mottle = SkyVistaTexture.Fbm(u * 8f, v * 8f, 191, 3);  // 0..1 break-up
            return Mathf.Clamp01(edge * (0.35f + 0.95f * mottle) - 0.12f);
        }

        /// <summary>Bake a decal to an RGBA sheet: white RGB, alpha = the chosen field. The placement
        /// material tints it dark; keeping RGB white lets one sheet serve shadow AND stain by tint.</summary>
        public static Color32[] BakeAlpha(int size, bool stain)
        {
            var px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = (x + 0.5f) / size, v = (y + 0.5f) / size;
                    float a = stain ? StainAlpha(u, v) : BlobAlpha(u, v);
                    px[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            return px;
        }
    }
}
