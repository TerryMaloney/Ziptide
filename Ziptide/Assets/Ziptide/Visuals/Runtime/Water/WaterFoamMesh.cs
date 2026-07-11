using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.3 commit 2 — EDGE FOAM: a thin double-sided ribbon standing at the water plane's
    /// perimeter (the lacy line where water laps a canal wall). The <see cref="FoamAlpha"/> cutout
    /// concentrates foam at the waterline (v=0) and frays it toward the top (v=1) with fBm, so a slow
    /// scroll of that alpha reads as churn. Pure and deterministic — the foam SHAPE is testable
    /// without a scene. Quest-cheap: a rectangle border of quads, ≤2 overdraw.
    /// </summary>
    public static class WaterFoamMesh
    {
        /// <summary>Build a foam ribbon around a <paramref name="sizeX"/>×<paramref name="sizeZ"/> m
        /// plane, <paramref name="height"/> m tall, <paramref name="segsPerMeter"/> quads per meter.
        /// UV: u runs along the edge (tiling the foam), v = 0 waterline → 1 top. Double-sided.</summary>
        public static Mesh BuildPerimeter(float sizeX, float sizeZ, float height, float segsPerMeter)
        {
            float hx = sizeX * 0.5f, hz = sizeZ * 0.5f;
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();

            // Four edges as (start, end) in XZ at y=0; the ribbon rises to +height.
            var edges = new[]
            {
                (new Vector2(-hx, -hz), new Vector2(hx, -hz)),
                (new Vector2(hx, -hz), new Vector2(hx, hz)),
                (new Vector2(hx, hz), new Vector2(-hx, hz)),
                (new Vector2(-hx, hz), new Vector2(-hx, -hz)),
            };
            foreach (var (a, b) in edges)
            {
                float len = Vector2.Distance(a, b);
                int segs = Mathf.Max(1, Mathf.RoundToInt(len * segsPerMeter));
                for (int s = 0; s < segs; s++)
                {
                    Vector2 p0 = Vector2.Lerp(a, b, s / (float)segs);
                    Vector2 p1 = Vector2.Lerp(a, b, (s + 1) / (float)segs);
                    float u0 = (len * s / segs), u1 = (len * (s + 1) / segs); // ~1 tile/meter
                    AddQuad(verts, uvs, tris,
                        new Vector3(p0.x, 0f, p0.y), new Vector3(p1.x, 0f, p1.y),
                        new Vector3(p1.x, height, p1.y), new Vector3(p0.x, height, p0.y),
                        u0, u1);
                }
            }

            var mesh = new Mesh { name = "ZiptideWaterFoam" };
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>Foam coverage at a ribbon UV (v=0 waterline → 1 top): a solid-ish foam band at
        /// the waterline fraying upward, broken laterally by fBm. 1 = foam, 0 = clear.</summary>
        public static float FoamAlpha(float u, float v)
        {
            v = Mathf.Clamp01(v);
            // Coverage falls off with height; the fBm makes the top edge lacy rather than a hard line.
            float band = 1f - v;                                         // 1 at waterline → 0 at top
            float lace = SkyVistaTexture.Fbm(u * 9f, v * 4f, 151, 2);    // 0..1 texture
            float mask = band * (0.7f + 0.6f * lace) - 0.35f;
            return Mathf.Clamp01(mask / 0.2f);                           // soft ~edge for clean clip
        }

        private static void AddQuad(List<Vector3> v, List<Vector2> uv, List<int> t,
            Vector3 a, Vector3 b, Vector3 c, Vector3 d, float u0, float u1)
        {
            int i = v.Count;
            v.Add(a); v.Add(b); v.Add(c); v.Add(d);
            uv.Add(new Vector2(u0, 0f)); uv.Add(new Vector2(u1, 0f));
            uv.Add(new Vector2(u1, 1f)); uv.Add(new Vector2(u0, 1f));
            // Front + back (double-sided, no two-sided shader needed).
            t.Add(i); t.Add(i + 2); t.Add(i + 1); t.Add(i); t.Add(i + 3); t.Add(i + 2);
            t.Add(i); t.Add(i + 1); t.Add(i + 2); t.Add(i); t.Add(i + 2); t.Add(i + 3);
        }
    }
}
