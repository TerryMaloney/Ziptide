using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.3 commit 2 — EDGE FOAM: a FLAT lacy band lying ON the water just inside its
    /// perimeter (foam gathers where the water meets a canal wall). Up-facing so it catches light
    /// and reads white (a vertical ribbon went dark under an overhead key). The <see cref="FoamAlpha"/>
    /// cutout is dense at the OUTER edge (v=0, the wall/waterline) and frays inward (v=1, open water)
    /// with fBm — scroll that alpha and it reads as churn. Pure and deterministic; Quest-cheap.
    /// </summary>
    public static class WaterFoamMesh
    {
        /// <summary>Build a flat foam band around a <paramref name="sizeX"/>×<paramref name="sizeZ"/> m
        /// plane: four up-facing strips <paramref name="bandWidth"/> m wide, inset from each edge, at
        /// y=<paramref name="y"/>. UV: u runs along the edge (tiles the foam), v = 0 outer edge →
        /// 1 inner.</summary>
        public static Mesh BuildBand(float sizeX, float sizeZ, float bandWidth, float segsPerMeter, float y)
        {
            float hx = sizeX * 0.5f, hz = sizeZ * 0.5f;
            float w = Mathf.Min(bandWidth, Mathf.Min(hx, hz)); // never wider than half the plane
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();

            // Each edge: (outer corner A, outer corner B, inward direction) — a flat strip A→B, out→in.
            var edges = new[]
            {
                (new Vector2(-hx, -hz), new Vector2(hx, -hz), new Vector2(0f, 1f)),  // south, inward +z
                (new Vector2(hx, hz), new Vector2(-hx, hz), new Vector2(0f, -1f)),   // north, inward -z
                (new Vector2(hx, -hz), new Vector2(hx, hz), new Vector2(-1f, 0f)),   // east, inward -x
                (new Vector2(-hx, hz), new Vector2(-hx, -hz), new Vector2(1f, 0f)),  // west, inward +x
            };
            foreach (var (a, b, inw) in edges)
            {
                float len = Vector2.Distance(a, b);
                int segs = Mathf.Max(1, Mathf.RoundToInt(len * segsPerMeter));
                for (int s = 0; s < segs; s++)
                {
                    Vector2 o0 = Vector2.Lerp(a, b, s / (float)segs);
                    Vector2 o1 = Vector2.Lerp(a, b, (s + 1) / (float)segs);
                    Vector2 i0 = o0 + inw * w, i1 = o1 + inw * w;
                    float u0 = len * s / segs, u1 = len * (s + 1) / segs; // ~1 tile/meter
                    // Up-facing quad: outer edge (v=0) → inner edge (v=1). Winding CCW from +Y.
                    AddQuad(verts, uvs, tris,
                        new Vector3(o0.x, y, o0.y), new Vector3(o1.x, y, o1.y),
                        new Vector3(i1.x, y, i1.y), new Vector3(i0.x, y, i0.y),
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

        /// <summary>Foam coverage at a band UV (v=0 outer/wall → 1 inner/open water): dense at the
        /// wall, fraying inward, broken laterally by fBm. 1 = foam, 0 = clear.</summary>
        public static float FoamAlpha(float u, float v)
        {
            v = Mathf.Clamp01(v);
            float band = 1f - v;                                         // 1 at the wall → 0 inward
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
            t.Add(i); t.Add(i + 1); t.Add(i + 2); t.Add(i); t.Add(i + 2); t.Add(i + 3);
        }
    }
}
