using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.3 — the water plane mesh: a flat subdivided grid in local XZ at y=0, with
    /// per-vertex UVs that TILE the water normal at a fixed world density (so a big canal and a
    /// small pool ripple at the same scale) and tangents for the normal map. Deterministic and
    /// pure (EditMode-testable). The subdivisions exist for the vertex BOB (commit 2); the fine
    /// ripples come from the scrolling normal, not geometry. Tri budget clamped ≤ 8000 (Quest).
    /// </summary>
    public static class ZiptideWaterMesh
    {
        public const int MaxTris = 8000;
        private const float UvTilesPerMeter = 0.25f; // one normal-map tile per 4m

        /// <summary>Build a <paramref name="sizeX"/>×<paramref name="sizeZ"/> m plane subdivided into
        /// <paramref name="cells"/>×<paramref name="cells"/> quads (clamped so tris ≤ MaxTris).</summary>
        public static Mesh Build(float sizeX, float sizeZ, int cells)
        {
            cells = Mathf.Max(1, cells);
            // Clamp so cells*cells*2 <= MaxTris.
            int maxCells = Mathf.FloorToInt(Mathf.Sqrt(MaxTris / 2f));
            cells = Mathf.Min(cells, maxCells);

            int verts = cells + 1;
            var vertices = new List<Vector3>(verts * verts);
            var uvs = new List<Vector2>(verts * verts);
            var normals = new List<Vector3>(verts * verts);
            var tris = new List<int>(cells * cells * 6);

            float hx = sizeX * 0.5f, hz = sizeZ * 0.5f;
            for (int z = 0; z <= cells; z++)
                for (int x = 0; x <= cells; x++)
                {
                    float fx = x / (float)cells, fz = z / (float)cells;
                    float wx = -hx + fx * sizeX, wz = -hz + fz * sizeZ;
                    vertices.Add(new Vector3(wx, 0f, wz));
                    uvs.Add(new Vector2(wx * UvTilesPerMeter, wz * UvTilesPerMeter));
                    normals.Add(Vector3.up);
                }

            for (int z = 0; z < cells; z++)
                for (int x = 0; x < cells; x++)
                {
                    int a = z * verts + x;
                    int b = a + 1;
                    int c = a + verts;
                    int d = c + 1;
                    // CCW when viewed from +Y (up-facing).
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(b); tris.Add(c); tris.Add(d);
                }

            var mesh = new Mesh { name = "ZiptideWater_" + cells };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateTangents(); // flat plane → uniform tangents for the normal map
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
