using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>Per-palette-slot material style — the whole surface vocabulary (closed enum, like ForgeOp).</summary>
    public enum ForgeStyle
    {
        PaintedMetal, BareMetal, RustedMetal, Chitin, Slime, Stone, Bark, Leaf, GlowPanel
    }

    /// <summary>Style parameters for one palette slot (parallel array to the recipe palette).</summary>
    [System.Serializable]
    public class ForgeStyleSpec
    {
        public ForgeStyle style = ForgeStyle.PaintedMetal;
        [Range(0f, 1f)] public float wear = 0.35f;        // bright bare-metal at part edges
        [Range(0f, 1f)] public float grime = 0.35f;       // dark matte settling on down-facing texels
        [Range(0f, 8f)] public float panelDensity = 0f;   // panel lines per island (0 = none)
        [Range(0.05f, 1f)] public float cellSize = 0.25f; // chitin/stone cell scale
        public Color emissive = Color.black;               // GlowPanel color (E1.3 bakes the map)
        [Range(0f, 6f)] public float emissiveIntensity = 2f;
    }

    /// <summary>
    /// FORGE II E1.2 — the texture bake. A tiny software rasterizer walks the SAME part/island/
    /// projection data ForgeMesh emits and fills per-texel metadata {slot, island-local uv,
    /// analytic edge distance, world-up factor}; style layers then compose the ALBEDO (normal/MSA/
    /// emissive land in E1.3 on the same metadata). Edge distance is ANALYTIC per op (we know the
    /// shapes parametrically — no adjacency, no raycasts): exact wear on real edges, never on
    /// coplanar triangulation seams. Pure, deterministic, EditMode-testable. Reuses
    /// SkyVistaTexture's proven noise/dither.
    /// </summary>
    public static class ForgeTexture
    {
        public struct Texel
        {
            public bool covered;
            public byte slot;
            public float u, v;          // island-local 0..1
            public float edge01;        // 0 at a geometric edge → 1 deep inside a face
            public float up01;          // 0 down-facing → 1 up-facing (world)
        }

        // ── Metadata rasterization ──────────────────────────────────────────

        /// <summary>Rasterize the recipe into an atlas-space metadata grid (size×size).</summary>
        public static Texel[] BakeMeta(ForgeRecipeDefinition recipe, int size)
        {
            var grid = new Texel[size * size];
            if (recipe == null || recipe.parts == null) return grid;

            var islands = ForgeUV.ComputeIslands(recipe);
            var islandByPart = new Dictionary<int, Rect>();
            foreach (var isl in islands) islandByPart[isl.partIndex] = isl.rect;

            for (int pi = 0; pi < recipe.parts.Length; pi++)
            {
                var part = recipe.parts[pi];
                if (part == null || !islandByPart.TryGetValue(pi, out var island)) continue;
                RasterizePart(part, island, size, grid);
            }
            return grid;
        }

        private static void RasterizePart(ForgePart part, Rect island, int size, Texel[] grid)
        {
            var local = ForgeMesh.BuildPart(part);
            var proj = ForgeUV.ProjectionFor(part.op);
            Bounds lb = LocalBounds(local);
            var rot = Quaternion.Euler(part.eulerRotation);
            byte slot = (byte)Mathf.Clamp(part.paletteSlot, 0, 255);

            for (int t = 0; t < local.triangles.Count; t += 3)
            {
                Vector3 la = local.vertices[local.triangles[t]];
                Vector3 lc0 = local.vertices[local.triangles[t + 1]];
                Vector3 lc1 = local.vertices[local.triangles[t + 2]];
                Vector3 ln = Vector3.Cross(lc0 - la, lc1 - la);
                if (ln.sqrMagnitude < 1e-14f) continue;
                ln.Normalize();
                float up01 = Mathf.Clamp01((rot * ln).y * 0.5f + 0.5f);

                ForgeUV.ProjectTriangle(proj, la, lc0, lc1, ln, lb, out var ua, out var ub, out var uc);
                Vector2 pa = ForgeUV.ToAtlas(ua, island) * size;
                Vector2 pb = ForgeUV.ToAtlas(ub, island) * size;
                Vector2 pc = ForgeUV.ToAtlas(uc, island) * size;

                int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(pa.x, Mathf.Min(pb.x, pc.x))));
                int maxX = Mathf.Min(size - 1, Mathf.CeilToInt(Mathf.Max(pa.x, Mathf.Max(pb.x, pc.x))));
                int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(pa.y, Mathf.Min(pb.y, pc.y))));
                int maxY = Mathf.Min(size - 1, Mathf.CeilToInt(Mathf.Max(pa.y, Mathf.Max(pb.y, pc.y))));

                float denom = (pb.y - pc.y) * (pa.x - pc.x) + (pc.x - pb.x) * (pa.y - pc.y);
                if (Mathf.Abs(denom) < 1e-9f) continue;

                for (int y = minY; y <= maxY; y++)
                    for (int x = minX; x <= maxX; x++)
                    {
                        float px = x + 0.5f, py = y + 0.5f;
                        float w0 = ((pb.y - pc.y) * (px - pc.x) + (pc.x - pb.x) * (py - pc.y)) / denom;
                        float w1 = ((pc.y - pa.y) * (px - pc.x) + (pa.x - pc.x) * (py - pc.y)) / denom;
                        float w2 = 1f - w0 - w1;
                        if (w0 < -0.001f || w1 < -0.001f || w2 < -0.001f) continue;

                        Vector3 lp = la * w0 + lc0 * w1 + lc1 * w2;
                        Vector2 iuv = ua * w0 + ub * w1 + uc * w2;
                        grid[y * size + x] = new Texel
                        {
                            covered = true,
                            slot = slot,
                            u = iuv.x,
                            v = iuv.y,
                            edge01 = EdgeDist01(part, lp, lb),
                            up01 = up01
                        };
                    }
            }
        }

        /// <summary>
        /// Analytic distance-to-nearest-geometric-edge, normalized 0 (on an edge) → 1 (face
        /// center), per op family. Exact where it matters (boxes/cylinders — the wear carriers);
        /// organic ops return 1 (no hard edges).
        /// </summary>
        public static float EdgeDist01(ForgePart part, Vector3 lp, Bounds lb)
        {
            Vector3 ext = lb.extents;
            switch (part.op)
            {
                case ForgeOp.Cylinder:
                case ForgeOp.Tube:
                case ForgeOp.Frustum: // rim circles too (bottom rim dominant; same normalization)
                {
                    // Edges = the two cap-rim CIRCLES: distance to the nearer rim combines the
                    // vertical gap to that cap AND the radial gap to the rim radius (a side-wall
                    // midpoint is far from both rims even though its radius matches).
                    float dy = ext.y - Mathf.Abs(lp.y);
                    float radius = Mathf.Sqrt(lp.x * lp.x + lp.z * lp.z);
                    float dr = Mathf.Max(ext.x, 0.0001f) - radius; // + inside, - outside (caps)
                    float rimDist = Mathf.Sqrt(dy * dy + dr * dr);
                    float norm = Mathf.Max(0.0001f, Mathf.Min(ext.x, ext.y));
                    return Mathf.Clamp01(rimDist / norm * 2f);
                }
                case ForgeOp.SphereSection:
                case ForgeOp.Lathe:
                case ForgeOp.Capsule:
                case ForgeOp.Torus:
                case ForgeOp.SweepSpline:
                case ForgeOp.OrganicBlob:
                    return 1f; // smooth/organic: no hard edges
                default: // box family (BeveledBox, Wedge, GreebleStrip)
                {
                    // Distance to the nearest pair-of-faces boundary: how close the point is to
                    // TWO extents at once (a box edge is where two |coord| = extent lines meet).
                    float dx = ext.x - Mathf.Abs(lp.x - lb.center.x);
                    float dy = ext.y - Mathf.Abs(lp.y - lb.center.y);
                    float dz = ext.z - Mathf.Abs(lp.z - lb.center.z);
                    // Sort: the smallest is ~0 (we're ON a face); the SECOND smallest is the
                    // distance to the nearest edge of that face.
                    float a = Mathf.Min(dx, Mathf.Min(dy, dz));
                    float c = Mathf.Max(dx, Mathf.Max(dy, dz));
                    float b = dx + dy + dz - a - c;
                    float norm = Mathf.Max(0.0001f, Mathf.Min(ext.x, Mathf.Min(ext.y, ext.z)));
                    return Mathf.Clamp01(b / norm);
                }
            }
        }

        // ── Albedo composition ──────────────────────────────────────────────

        private static readonly Color BareMetalBright = new Color(0.72f, 0.73f, 0.76f);

        /// <summary>Compose the albedo atlas from metadata + styles. Deterministic; Bayer-dithered.</summary>
        public static void BakeAlbedo(ForgeRecipeDefinition recipe, Texel[] meta, int size, Color32[] pixels)
        {
            if (pixels == null || pixels.Length != size * size) return;

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int i = y * size + x;
                    var tx = meta[i];
                    if (!tx.covered)
                    {
                        pixels[i] = new Color32(12, 12, 13, 255); // gutter fill (dilated below)
                        continue;
                    }
                    Color baseCol = SlotColor(recipe, tx.slot);
                    var spec = SlotStyle(recipe, tx.slot);
                    pixels[i] = SkyVistaTexture.DitherTo32(ComposeAlbedo(baseCol, spec, tx), x, y);
                }

            DilatePixels(meta, size, pixels);
        }

        /// <summary>One texel's albedo — the layer stack from FORGE_II_QUALITY_LEAP §P1.</summary>
        public static Color ComposeAlbedo(Color baseCol, ForgeStyleSpec spec, Texel tx)
        {
            float n = SkyVistaTexture.Fbm(tx.u * 14f, tx.v * 14f, 31, 3);
            Color c = baseCol * (0.92f + 0.16f * n); // base value noise

            switch (spec.style)
            {
                case ForgeStyle.RustedMetal:
                {
                    float rust = SkyVistaTexture.Fbm(tx.u * 7f + 3f, tx.v * 7f + 9f, 47, 3);
                    if (rust > 0.55f)
                        c = Color.Lerp(c, new Color(0.45f, 0.23f, 0.10f), (rust - 0.55f) / 0.45f * 0.8f);
                    break;
                }
                case ForgeStyle.Chitin:
                {
                    float f1 = Voronoi(tx.u / spec.cellSize, tx.v / spec.cellSize, 53);
                    c *= 0.75f + 0.35f * Mathf.SmoothStep(0f, 0.35f, f1);   // dark cell borders
                    break;
                }
                case ForgeStyle.Stone:
                {
                    float m = SkyVistaTexture.Fbm(tx.u * 9f, tx.v * 9f, 61, 3);
                    c = Color.Lerp(c, c * 0.7f, Mathf.SmoothStep(0.55f, 0.8f, m));
                    break;
                }
                case ForgeStyle.Bark:
                {
                    float stripe = Mathf.Abs(Mathf.Sin(tx.v * 40f + SkyVistaTexture.Fbm(tx.u * 5f, tx.v * 5f, 71, 2) * 6f));
                    c *= 0.75f + 0.3f * stripe;
                    break;
                }
                case ForgeStyle.Slime:
                {
                    float blotch = SkyVistaTexture.Fbm(tx.u * 6f, tx.v * 6f, 83, 2);
                    c = Color.Lerp(c, c * 1.25f, Mathf.SmoothStep(0.5f, 0.75f, blotch) * 0.6f);
                    // Subdermal mottle (creature v4 loop: one bright blotch layer baked PLAIN on a
                    // pale hide) — a finer darker pass that reads as organs/veins under the skin;
                    // grime scales its strength so the knob stays meaningful for wet hides.
                    float mottle = SkyVistaTexture.Fbm(tx.u * 14f, tx.v * 14f, 89, 2);
                    // (v5 photo tune: 0.72@0.6-0.85 vanished under the wet-sheen specular — wider
                    // coverage + deeper darkening so the pattern survives the highlight.)
                    c = Color.Lerp(c, c * 0.58f, Mathf.SmoothStep(0.5f, 0.8f, mottle) * (0.3f + 0.7f * spec.grime));
                    break;
                }
                case ForgeStyle.Leaf:
                {
                    float vein = Mathf.Abs(Mathf.Sin(tx.u * 3.1416f) * Mathf.Sin(tx.v * 28f));
                    c = Color.Lerp(c, c * 0.7f, Mathf.SmoothStep(0.85f, 1f, vein));
                    break;
                }
                case ForgeStyle.GlowPanel:
                    c *= 0.25f; // dark base — the glow is the E1.3 emissive map (booth preview tints)
                    break;
            }

            // Panel lines (any style with density > 0): thin dark grid in island space.
            if (spec.panelDensity > 0.01f)
            {
                float pu = Frac(tx.u * spec.panelDensity);
                float pv = Frac(tx.v * spec.panelDensity);
                float line = Mathf.Min(Mathf.Min(pu, 1f - pu), Mathf.Min(pv, 1f - pv));
                if (line < 0.02f) c *= 0.8f;
            }

            // Edge wear: bright bare metal where edge01 is small (metal-family styles only).
            bool metalFamily = spec.style == ForgeStyle.PaintedMetal || spec.style == ForgeStyle.BareMetal
                || spec.style == ForgeStyle.RustedMetal || spec.style == ForgeStyle.GlowPanel;
            if (metalFamily && spec.wear > 0.01f)
            {
                float band = 0.05f + 0.18f * spec.wear;
                float wearNoise = SkyVistaTexture.ValueNoise(tx.u * 40f, tx.v * 40f, 97);
                float w = Mathf.SmoothStep(band, 0f, tx.edge01) * (0.5f + 0.5f * wearNoise) * spec.wear;
                c = Color.Lerp(c, BareMetalBright, w);
            }

            // Grime: settles on down-facing surfaces, breaks up with noise. Darker + murkier.
            // (v2 tune from the first checkpoint photos: capped — full-strength grime crushed
            // down-faces to black bands.)
            if (spec.grime > 0.01f)
            {
                float g = spec.grime * (1f - tx.up01) * (0.5f + 0.5f * SkyVistaTexture.Fbm(tx.u * 10f + 7f, tx.v * 10f + 3f, 103, 2));
                g = Mathf.Min(g, 0.55f);
                c = Color.Lerp(c, new Color(c.r * 0.5f, c.g * 0.51f, c.b * 0.47f), Mathf.Clamp01(g));
            }

            return c;
        }

        // ── E1.3: height field → normal map ─────────────────────────────────

        /// <summary>
        /// One texel's surface height, 0.5 neutral. EXACTLY 0.5 when the style adds no relief
        /// (the map-content contract: normal map neutral where no detail) — features use the SAME
        /// masks/seeds as the albedo layers so relief and color align texel-for-texel.
        /// </summary>
        public static float ComposeHeight(ForgeStyleSpec spec, Texel tx)
        {
            float h = 0.5f;

            switch (spec.style)
            {
                case ForgeStyle.RustedMetal:
                {
                    float rust = SkyVistaTexture.Fbm(tx.u * 7f + 3f, tx.v * 7f + 9f, 47, 3);
                    if (rust > 0.55f) h -= (rust - 0.55f) / 0.45f * 0.12f; // rust pits in
                    break;
                }
                case ForgeStyle.Chitin:
                {
                    float f1 = Voronoi(tx.u / spec.cellSize, tx.v / spec.cellSize, 53);
                    h -= (1f - Mathf.SmoothStep(0f, 0.35f, f1)) * 0.18f;  // cell-border grooves
                    break;
                }
                case ForgeStyle.Stone:
                {
                    float m = SkyVistaTexture.Fbm(tx.u * 9f, tx.v * 9f, 61, 3);
                    h -= Mathf.SmoothStep(0.55f, 0.8f, m) * 0.15f;        // cracks
                    break;
                }
                case ForgeStyle.Bark:
                {
                    float stripe = Mathf.Abs(Mathf.Sin(tx.v * 40f + SkyVistaTexture.Fbm(tx.u * 5f, tx.v * 5f, 71, 2) * 6f));
                    h += (stripe - 0.5f) * 0.2f;                           // ridges
                    break;
                }
                case ForgeStyle.Slime:
                {
                    float blotch = SkyVistaTexture.Fbm(tx.u * 6f, tx.v * 6f, 83, 2);
                    h += Mathf.SmoothStep(0.5f, 0.75f, blotch) * 0.12f;    // wet bumps
                    float mottle = SkyVistaTexture.Fbm(tx.u * 14f, tx.v * 14f, 89, 2);
                    h -= Mathf.SmoothStep(0.6f, 0.85f, mottle) * 0.07f;    // subdermal dimples
                    break;
                }
                case ForgeStyle.Leaf:
                {
                    float vein = Mathf.Abs(Mathf.Sin(tx.u * 3.1416f) * Mathf.Sin(tx.v * 28f));
                    h += Mathf.SmoothStep(0.85f, 1f, vein) * 0.1f;         // raised veins
                    break;
                }
                case ForgeStyle.GlowPanel:
                    h -= 0.06f; // panel face sits slightly inset in its housing
                    break;
            }

            // Panel-line grooves (same grid as the albedo lines).
            if (spec.panelDensity > 0.01f)
            {
                float pu = Frac(tx.u * spec.panelDensity);
                float pv = Frac(tx.v * spec.panelDensity);
                float line = Mathf.Min(Mathf.Min(pu, 1f - pu), Mathf.Min(pv, 1f - pv));
                if (line < 0.02f) h -= 0.18f;
            }

            // Edge wear rounds the corner off (same band as the albedo wear).
            bool metalFamily = spec.style == ForgeStyle.PaintedMetal || spec.style == ForgeStyle.BareMetal
                || spec.style == ForgeStyle.RustedMetal || spec.style == ForgeStyle.GlowPanel;
            if (metalFamily && spec.wear > 0.01f)
            {
                float band = 0.05f + 0.18f * spec.wear;
                h -= Mathf.SmoothStep(band, 0f, tx.edge01) * 0.1f * spec.wear;
            }

            return h;
        }

        private const float NormalStrength = 6f; // height-delta → slope scale (groove ≈ 45°)

        /// <summary>
        /// Tangent-space normal map: per-texel height (ComposeHeight) → central differences →
        /// encoded RGB. Neighbors outside the texel's island fall back to its own height, so
        /// island borders stay flat instead of reading as cliffs. Uncovered texels are neutral.
        /// </summary>
        public static void BakeNormal(ForgeRecipeDefinition recipe, Texel[] meta, int size, Color32[] pixels)
        {
            if (pixels == null || pixels.Length != size * size) return;

            var height = new float[size * size];
            for (int i = 0; i < height.Length; i++)
                height[i] = meta[i].covered ? ComposeHeight(SlotStyle(recipe, meta[i].slot), meta[i]) : 0.5f;

            var neutral = new Color32(128, 128, 255, 255);
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int i = y * size + x;
                    if (!meta[i].covered) { pixels[i] = neutral; continue; }

                    float c = height[i];
                    float Sample(int nx, int ny)
                    {
                        if (nx < 0 || ny < 0 || nx >= size || ny >= size) return c;
                        int n = ny * size + nx;
                        return meta[n].covered && meta[n].slot == meta[i].slot ? height[n] : c;
                    }
                    float dx = (Sample(x - 1, y) - Sample(x + 1, y)) * NormalStrength;
                    float dy = (Sample(x, y - 1) - Sample(x, y + 1)) * NormalStrength;
                    Vector3 n3 = new Vector3(dx, dy, 1f).normalized;
                    pixels[i] = new Color32(
                        (byte)Mathf.RoundToInt((n3.x * 0.5f + 0.5f) * 255f),
                        (byte)Mathf.RoundToInt((n3.y * 0.5f + 0.5f) * 255f),
                        (byte)Mathf.RoundToInt((n3.z * 0.5f + 0.5f) * 255f), 255);
                }
        }

        // ── E1.3: metallic-smoothness (URP _MetallicGlossMap: R=metallic, A=smoothness) ──

        /// <summary>Per-style base surface response (metallic, smoothness).</summary>
        public static Vector2 StyleMetalSmooth(ForgeStyle style)
        {
            switch (style)
            {
                case ForgeStyle.PaintedMetal: return new Vector2(0.15f, 0.45f);
                case ForgeStyle.BareMetal: return new Vector2(0.85f, 0.55f);
                case ForgeStyle.RustedMetal: return new Vector2(0.55f, 0.35f);
                case ForgeStyle.Chitin: return new Vector2(0f, 0.55f);
                case ForgeStyle.Slime: return new Vector2(0f, 0.7f); // wet — the hide's tell (0.85 glinted on bevel seams)
                case ForgeStyle.Stone: return new Vector2(0f, 0.18f);
                case ForgeStyle.Bark: return new Vector2(0f, 0.15f);
                case ForgeStyle.Leaf: return new Vector2(0f, 0.4f);
                case ForgeStyle.GlowPanel: return new Vector2(0.2f, 0.5f);
                default: return new Vector2(0f, 0.4f);
            }
        }

        /// <summary>One texel's (metallic, smoothness) — same masks as the albedo layers.</summary>
        public static Vector2 ComposeMetalSmooth(ForgeStyleSpec spec, Texel tx)
        {
            Vector2 ms = StyleMetalSmooth(spec.style);

            if (spec.style == ForgeStyle.RustedMetal)
            {
                float rust = SkyVistaTexture.Fbm(tx.u * 7f + 3f, tx.v * 7f + 9f, 47, 3);
                if (rust > 0.55f)
                {
                    float k = (rust - 0.55f) / 0.45f * 0.8f;
                    ms.x = Mathf.Lerp(ms.x, 0.1f, k);  // rust is not metallic
                    ms.y = Mathf.Lerp(ms.y, 0.15f, k); // and very rough
                }
            }

            bool metalFamily = spec.style == ForgeStyle.PaintedMetal || spec.style == ForgeStyle.BareMetal
                || spec.style == ForgeStyle.RustedMetal || spec.style == ForgeStyle.GlowPanel;
            if (metalFamily && spec.wear > 0.01f)
            {
                float band = 0.05f + 0.18f * spec.wear;
                float wearNoise = SkyVistaTexture.ValueNoise(tx.u * 40f, tx.v * 40f, 97);
                float w = Mathf.SmoothStep(band, 0f, tx.edge01) * (0.5f + 0.5f * wearNoise) * spec.wear;
                ms.x = Mathf.Lerp(ms.x, 0.9f, w);      // worn edges show bare metal
                ms.y = Mathf.Lerp(ms.y, 0.65f, w);
            }

            if (spec.grime > 0.01f)
            {
                float g = spec.grime * (1f - tx.up01) * (0.5f + 0.5f * SkyVistaTexture.Fbm(tx.u * 10f + 7f, tx.v * 10f + 3f, 103, 2));
                g = Mathf.Min(g, 0.55f);
                ms.x *= 1f - 0.5f * g;                 // grime dulls both
                ms.y *= 1f - 0.7f * g;
            }

            return ms;
        }

        /// <summary>Bake the _MetallicGlossMap atlas: R=metallic, G=255 (unused), B=0, A=smoothness.</summary>
        public static void BakeMSA(ForgeRecipeDefinition recipe, Texel[] meta, int size, Color32[] pixels)
        {
            if (pixels == null || pixels.Length != size * size) return;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (!meta[i].covered) { pixels[i] = new Color32(0, 255, 0, 100); continue; }
                Vector2 ms = ComposeMetalSmooth(SlotStyle(recipe, meta[i].slot), meta[i]);
                pixels[i] = new Color32(
                    (byte)Mathf.RoundToInt(Mathf.Clamp01(ms.x) * 255f), 255, 0,
                    (byte)Mathf.RoundToInt(Mathf.Clamp01(ms.y) * 255f));
            }
            DilatePixels(meta, size, pixels);
        }

        // ── E1.3: emissive map (GlowPanel slots only) ───────────────────────

        /// <summary>Brightest glow intensity in the recipe — the material's _EmissionColor scalar;
        /// per-slot intensity is baked into the map relative to it.</summary>
        public static float MaxEmissiveIntensity(ForgeRecipeDefinition recipe)
        {
            float max = 0f;
            if (recipe != null && recipe.slotStyles != null)
                foreach (var s in recipe.slotStyles)
                    if (s != null && s.style == ForgeStyle.GlowPanel && s.emissiveIntensity > max)
                        max = s.emissiveIntensity;
            return max;
        }

        /// <summary>Bake the _EmissionMap atlas: glow-slot texels carry their color (scaled to the
        /// recipe's brightest glow, soft fbm breakup); everything else is black.</summary>
        public static void BakeEmissive(ForgeRecipeDefinition recipe, Texel[] meta, int size, Color32[] pixels)
        {
            if (pixels == null || pixels.Length != size * size) return;
            float maxI = Mathf.Max(0.0001f, MaxEmissiveIntensity(recipe));
            var black = new Color32(0, 0, 0, 255);

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int i = y * size + x;
                    var tx = meta[i];
                    var spec = tx.covered ? SlotStyle(recipe, tx.slot) : null;
                    if (spec == null || spec.style != ForgeStyle.GlowPanel) { pixels[i] = black; continue; }
                    float rel = Mathf.Clamp01(spec.emissiveIntensity / maxI)
                        * (0.85f + 0.15f * SkyVistaTexture.Fbm(tx.u * 5f, tx.v * 5f, 113, 2));
                    pixels[i] = SkyVistaTexture.DitherTo32(spec.emissive * rel, x, y);
                }
            DilatePixels(meta, size, pixels);
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        public static Color SlotColor(ForgeRecipeDefinition r, int slot)
            => r != null && r.palette != null && slot < r.palette.Length ? r.palette[slot] : Color.magenta;

        public static ForgeStyleSpec SlotStyle(ForgeRecipeDefinition r, int slot)
            => r != null && r.slotStyles != null && slot < r.slotStyles.Length && r.slotStyles[slot] != null
                ? r.slotStyles[slot]
                : DefaultStyle;

        private static readonly ForgeStyleSpec DefaultStyle = new ForgeStyleSpec();

        /// <summary>Voronoi F1 (distance to nearest jittered cell point), deterministic.</summary>
        public static float Voronoi(float x, float y, int seed)
        {
            int cx = Mathf.FloorToInt(x), cy = Mathf.FloorToInt(y);
            float best = 8f;
            for (int oy = -1; oy <= 1; oy++)
                for (int ox = -1; ox <= 1; ox++)
                {
                    float jx = cx + ox + SkyVistaTexture.Hash01(cx + ox, cy + oy, seed);
                    float jy = cy + oy + SkyVistaTexture.Hash01(cx + ox, cy + oy, seed + 1);
                    float d = (x - jx) * (x - jx) + (y - jy) * (y - jy);
                    if (d < best) best = d;
                }
            return Mathf.Sqrt(best);
        }

        /// <summary>
        /// Deep dilation of covered colors into uncovered gutter texels. Six rings fill most of
        /// the 16px gutter from each side, so even grazing-angle (anisotropic) sample footprints
        /// that reach past an island border read the island's OWN colors, never the neighbor's.
        /// NON-MUTATING on meta — a local coverage copy tracks the growing ring, so one BakeMeta
        /// result can feed every map bake (albedo/normal/MSA/emissive) untouched.
        /// </summary>
        private static void DilatePixels(Texel[] meta, int size, Color32[] px)
        {
            var covered = new bool[meta.Length];
            for (int i = 0; i < meta.Length; i++) covered[i] = meta[i].covered;

            for (int ring = 0; ring < 6; ring++)
            {
                var copy = (Color32[])px.Clone();
                var grown = (bool[])covered.Clone();
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        int i = y * size + x;
                        if (covered[i]) continue;
                        for (int oy = -1; oy <= 1; oy++)
                            for (int ox = -1; ox <= 1; ox++)
                            {
                                int nx = x + ox, ny = y + oy;
                                if (nx < 0 || ny < 0 || nx >= size || ny >= size) continue;
                                if (covered[ny * size + nx]) { px[i] = copy[ny * size + nx]; grown[i] = true; goto next; }
                            }
                        next: ;
                    }
                covered = grown;
            }
        }

        private static float Frac(float f) => f - Mathf.Floor(f);

        private static Bounds LocalBounds(ForgeMesh.PartGeometry g)
        {
            if (g.vertices.Count == 0) return new Bounds(Vector3.zero, Vector3.one * 0.001f);
            var b = new Bounds(g.vertices[0], Vector3.zero);
            for (int i = 1; i < g.vertices.Count; i++) b.Encapsulate(g.vertices[i]);
            return b;
        }
    }
}
