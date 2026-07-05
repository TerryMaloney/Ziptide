using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// UV atlas math for Forge assets (FORGE II E1.1): every part gets its own rectangular atlas
    /// island (deterministic shelf packing, island edge ∝ √surface-area so texel density stays
    /// roughly uniform), and vertices project into their island by the op's natural mapping —
    /// box-planar (boxes/wedges/greebles), cylindrical (cylinders/tubes/lathes), spherical (domes).
    /// Islands never overlap BETWEEN parts (each part has one style, so same-part projection
    /// overlap only causes invisible edge-distance artifacts); mirrored copies reuse their source
    /// island. Pure and deterministic — the texture rasterizer (E1.2) trusts these islands as
    /// texel ownership.
    /// </summary>
    public static class ForgeUV
    {
        /// <summary>
        /// Gutter between islands in UV units (16px at a 1024 atlas). Sized for GRAZING-ANGLE
        /// sampling: anisotropic/stretched footprints on edge-on faces reach many texels past the
        /// island border, and with a thin gutter they land in the NEIGHBOR island (the E1.3 "cyan
        /// leg outline" — legs packed next to the glowing sac). Pairs with deep dilation in
        /// ForgeTexture so the reachable gutter carries the island's own colors.
        /// </summary>
        public const float Gutter = 16f / 1024f;

        public struct Island
        {
            public Rect rect;      // atlas-space region, gutter already applied
            public int partIndex;  // index into recipe.parts
        }

        // ── Island allocation ────────────────────────────────────────────────

        /// <summary>
        /// Deterministic shelf packing: islands sized ∝ √(part surface area), packed in part order
        /// into rows. A global scale is binary-searched so everything fits in [0,1]².
        /// </summary>
        public static List<Island> ComputeIslands(ForgeRecipeDefinition recipe)
        {
            var islands = new List<Island>();
            if (recipe == null || recipe.parts == null || recipe.parts.Length == 0) return islands;

            int n = recipe.parts.Length;
            var edges = new float[n];
            float maxEdge = 0f;
            for (int i = 0; i < n; i++)
            {
                var p = recipe.parts[i];
                float area = p == null ? 0.0001f : SurfaceAreaEstimate(p);
                edges[i] = Mathf.Sqrt(Mathf.Max(0.0001f, area));
                if (edges[i] > maxEdge) maxEdge = edges[i];
            }
            // Normalize so the largest island starts at edge 0.5, then shrink until packing fits.
            float scale = 0.5f / maxEdge;
            for (int attempt = 0; attempt < 24; attempt++)
            {
                if (TryPack(edges, scale, islands)) return islands;
                scale *= 0.85f;
            }
            TryPack(edges, scale, islands); // last attempt's layout regardless
            return islands;
        }

        private static bool TryPack(float[] edges, float scale, List<Island> outIslands)
        {
            outIslands.Clear();
            float x = Gutter, y = Gutter, rowH = 0f;
            for (int i = 0; i < edges.Length; i++)
            {
                float e = Mathf.Clamp(edges[i] * scale, 0.02f, 0.9f);
                if (x + e + Gutter > 1f) // new shelf row
                {
                    x = Gutter;
                    y += rowH + Gutter;
                    rowH = 0f;
                }
                if (y + e + Gutter > 1f) return false; // out of space at this scale
                outIslands.Add(new Island { rect = new Rect(x, y, e, e), partIndex = i });
                x += e + Gutter;
                if (e > rowH) rowH = e;
            }
            return true;
        }

        /// <summary>Cheap analytic surface-area estimate per op (drives island size only).</summary>
        public static float SurfaceAreaEstimate(ForgePart p)
        {
            Vector3 s = Vector3.Scale(p.size, p.scale == Vector3.zero ? Vector3.one : p.scale);
            switch (p.op)
            {
                case ForgeOp.Cylinder:
                case ForgeOp.Tube:
                case ForgeOp.Lathe:
                    return Mathf.PI * s.x * s.y + Mathf.PI * 0.5f * s.x * s.x;
                case ForgeOp.SphereSection:
                    return Mathf.PI * s.x * s.y; // ellipsoid-ish
                default: // box-family
                    return 2f * (s.x * s.y + s.y * s.z + s.x * s.z);
            }
        }

        // ── Per-op projection (local position → island-local 0..1) ──────────

        public enum Projection { BoxPlanar, Cylindrical, Spherical }

        public static Projection ProjectionFor(ForgeOp op)
        {
            switch (op)
            {
                case ForgeOp.Cylinder:
                case ForgeOp.Tube:
                case ForgeOp.Lathe:
                    return Projection.Cylindrical;
                case ForgeOp.SphereSection:
                    return Projection.Spherical;
                default:
                    return Projection.BoxPlanar;
            }
        }

        /// <summary>
        /// Project one triangle's LOCAL-space vertices to island-local UVs. Per-triangle so the
        /// cylindrical wrap seam can be fixed face-locally (flat parts have unique verts per face).
        /// <paramref name="faceNormal"/> is the local-space face normal (box projection axis pick).
        /// </summary>
        public static void ProjectTriangle(Projection proj, Vector3 la, Vector3 lb, Vector3 lc,
            Vector3 faceNormal, Bounds localBounds, out Vector2 ua, out Vector2 ub, out Vector2 uc)
        {
            switch (proj)
            {
                case Projection.Cylindrical:
                    ua = CylUV(la, localBounds); ub = CylUV(lb, localBounds); uc = CylUV(lc, localBounds);
                    FixWrap(ref ua, ref ub, ref uc);
                    return;
                case Projection.Spherical:
                    ua = SphUV(la); ub = SphUV(lb); uc = SphUV(lc);
                    FixWrap(ref ua, ref ub, ref uc);
                    return;
                default:
                    ua = BoxUV(la, faceNormal, localBounds);
                    ub = BoxUV(lb, faceNormal, localBounds);
                    uc = BoxUV(lc, faceNormal, localBounds);
                    return;
            }
        }

        private static Vector2 BoxUV(Vector3 v, Vector3 n, Bounds b)
        {
            float ax = Mathf.Abs(n.x), ay = Mathf.Abs(n.y), az = Mathf.Abs(n.z);
            float u, w;
            if (ax >= ay && ax >= az) { u = Norm(v.z, b.min.z, b.max.z); w = Norm(v.y, b.min.y, b.max.y); }
            else if (ay >= ax && ay >= az) { u = Norm(v.x, b.min.x, b.max.x); w = Norm(v.z, b.min.z, b.max.z); }
            else { u = Norm(v.x, b.min.x, b.max.x); w = Norm(v.y, b.min.y, b.max.y); }
            return new Vector2(u, w);
        }

        private static Vector2 CylUV(Vector3 v, Bounds b)
        {
            float angle = Mathf.Atan2(v.z, v.x) / (2f * Mathf.PI) + 0.5f; // 0..1
            float h = Norm(v.y, b.min.y, b.max.y);
            return new Vector2(angle, h);
        }

        private static Vector2 SphUV(Vector3 v)
        {
            Vector3 d = v.sqrMagnitude > 1e-10f ? v.normalized : Vector3.up;
            float lon = Mathf.Atan2(d.z, d.x) / (2f * Mathf.PI) + 0.5f;
            float lat = Mathf.Asin(Mathf.Clamp(d.y, -1f, 1f)) / Mathf.PI + 0.5f;
            return new Vector2(lon, lat);
        }

        /// <summary>
        /// Unwrap the 0↔1 angular seam within one triangle: verts on the low side shift +1 (making
        /// the triangle continuous), then a uniform −0.5 brings the whole triangle back inside
        /// [0,1]. The seam triangles' pattern is offset from their neighbors — a known, accepted
        /// artifact (procedural grunge styles hide it; only continuity within a triangle matters
        /// for the rasterizer's texel ownership).
        /// </summary>
        private static void FixWrap(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            float min = Mathf.Min(a.x, Mathf.Min(b.x, c.x));
            float max = Mathf.Max(a.x, Mathf.Max(b.x, c.x));
            if (max - min <= 0.5f) return;
            if (a.x < 0.5f) a.x += 1f;
            if (b.x < 0.5f) b.x += 1f;
            if (c.x < 0.5f) c.x += 1f;
            a.x -= 0.5f; b.x -= 0.5f; c.x -= 0.5f;
        }

        /// <summary>Map an island-local UV (0..1, clamped) into the island's atlas rect.</summary>
        public static Vector2 ToAtlas(Vector2 islandLocal, Rect island)
        {
            return new Vector2(
                island.x + Mathf.Clamp01(islandLocal.x) * island.width,
                island.y + Mathf.Clamp01(islandLocal.y) * island.height);
        }

        private static float Norm(float v, float min, float max)
            => max - min > 1e-6f ? Mathf.Clamp01((v - min) / (max - min)) : 0.5f;
    }
}
