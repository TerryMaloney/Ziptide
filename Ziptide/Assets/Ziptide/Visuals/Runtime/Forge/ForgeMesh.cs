using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Pure deterministic mesh construction for Forge recipes — the 3D sibling of SkyVistaTexture.
    /// Ops emit indexed local-space geometry; <see cref="Build"/> transforms, mirrors, explodes
    /// flat-shaded parts (crisp low-poly read), computes normals itself (never RecalculateNormals —
    /// determinism), and combines everything into ONE mesh with a submesh per used palette slot
    /// (URP/Lit ignores vertex colors; shared palette materials keep draw calls flat). No GPU or
    /// scene dependency in the geometry math, so EditMode tests can assert vertices.
    /// </summary>
    public static class ForgeMesh
    {
        public class PartGeometry
        {
            public readonly List<Vector3> vertices = new List<Vector3>();
            public readonly List<int> triangles = new List<int>();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Total triangle count for the whole recipe (mirrors included) — budget tests/audit.</summary>
        public static int CountTriangles(ForgeRecipeDefinition recipe)
        {
            if (recipe == null || recipe.parts == null) return 0;
            int tris = 0;
            foreach (var p in recipe.parts)
            {
                if (p == null) continue;
                var g = BuildPart(p);
                tris += (g.triangles.Count / 3) * (p.mirrorX ? 2 : 1);
            }
            return tris;
        }

        /// <summary>Palette slots actually referenced by parts, ascending — one submesh/material each.</summary>
        public static List<int> UsedPaletteSlots(ForgeRecipeDefinition recipe)
        {
            var used = new SortedSet<int>();
            if (recipe != null && recipe.parts != null)
                foreach (var p in recipe.parts)
                    if (p != null) used.Add(p.paletteSlot);
            return new List<int>(used);
        }

        /// <summary>Build the combined mesh. Deterministic: same recipe → identical vertex data.
        /// FORGE II: every part projects into its own UV-atlas island (ForgeUV) and the mesh carries
        /// uv0 + tangents so the texture bake (E1.2+) has texel ownership and normal-map support.</summary>
        public static Mesh Build(ForgeRecipeDefinition recipe)
        {
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var slotTris = new Dictionary<int, List<int>>();

            var islands = ForgeUV.ComputeIslands(recipe);
            var islandByPart = new Dictionary<int, Rect>();
            foreach (var isl in islands) islandByPart[isl.partIndex] = isl.rect;

            if (recipe != null && recipe.parts != null)
                for (int pi = 0; pi < recipe.parts.Length; pi++)
                {
                    var part = recipe.parts[pi];
                    if (part == null) continue;
                    var local = BuildPart(part);
                    Rect island = islandByPart.TryGetValue(pi, out var r) ? r : new Rect(0f, 0f, 1f, 1f);
                    AppendInstance(local, part, island, mirrored: false, verts, normals, uvs, SlotList(slotTris, part.paletteSlot));
                    if (part.mirrorX)
                        AppendInstance(local, part, island, mirrored: true, verts, normals, uvs, SlotList(slotTris, part.paletteSlot));
                }

            var mesh = new Mesh { name = recipe != null ? "Forge_" + recipe.recipeId : "Forge" };
            if (verts.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            var slots = UsedPaletteSlots(recipe);
            mesh.subMeshCount = Mathf.Max(1, slots.Count);
            for (int i = 0; i < slots.Count; i++)
                mesh.SetTriangles(slotTris[slots[i]], i);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents(); // tangents follow our UVs; normals stay ours
            return mesh;
        }

        /// <summary>Indexed local-space geometry for one part (before transform/mirror/shading).</summary>
        public static PartGeometry BuildPart(ForgePart part)
        {
            var g = new PartGeometry();
            switch (part.op)
            {
                case ForgeOp.BeveledBox: BuildBeveledBox(g, part.size, Mathf.Max(0f, part.bevel)); break;
                case ForgeOp.Cylinder: BuildCylinder(g, part.size.x * 0.5f, part.size.z * 0.5f, part.size.y, part.segments); break;
                case ForgeOp.Tube: BuildTube(g, part.size.x * 0.5f, part.size.y, part.wallThickness, part.segments); break;
                case ForgeOp.Lathe: BuildLathe(g, part.profile, part.size.x, part.size.y, part.segments); break;
                case ForgeOp.SphereSection: BuildSphereSection(g, part.size * 0.5f, Mathf.Clamp(part.bevel <= 0f ? 1f : part.bevel, 0.05f, 1f), part.segments); break;
                case ForgeOp.Wedge: BuildWedge(g, part.size); break;
                case ForgeOp.GreebleStrip: BuildGreebleStrip(g, part.size, part.segments); break;
            }
            return g;
        }

        // ── Assembly ─────────────────────────────────────────────────────────

        private static List<int> SlotList(Dictionary<int, List<int>> map, int slot)
        {
            if (!map.TryGetValue(slot, out var list)) { list = new List<int>(); map[slot] = list; }
            return list;
        }

        private static void AppendInstance(PartGeometry local, ForgePart part, Rect island, bool mirrored,
            List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> tris)
        {
            var rot = Quaternion.Euler(part.eulerRotation);
            var scl = part.scale == Vector3.zero ? Vector3.one : part.scale;
            var proj = ForgeUV.ProjectionFor(part.op);
            Bounds localBounds = LocalBounds(local);

            Vector3 Xform(Vector3 v)
            {
                v = Vector3.Scale(v, scl);
                v = rot * v;
                v += part.position;
                if (mirrored) v.x = -v.x;
                return v;
            }

            if (part.smooth)
            {
                // Indexed: shared verts, accumulated normals, per-VERTEX UVs (a smooth surface can't
                // hold per-face UVs — the one wrap-seam column this leaves is a documented artifact).
                int baseIndex = verts.Count;
                var acc = new Vector3[local.vertices.Count];
                for (int i = 0; i < local.vertices.Count; i++)
                {
                    Vector3 lv = local.vertices[i];
                    verts.Add(Xform(lv));
                    ForgeUV.ProjectTriangle(proj, lv, lv, lv, lv, localBounds, out var uv, out _, out _);
                    uvs.Add(ForgeUV.ToAtlas(uv, island));
                }
                for (int t = 0; t < local.triangles.Count; t += 3)
                {
                    int a = local.triangles[t], b = local.triangles[t + 1], c = local.triangles[t + 2];
                    if (mirrored) { int tmp = b; b = c; c = tmp; } // winding flip
                    Vector3 fn = FaceNormal(verts[baseIndex + a], verts[baseIndex + b], verts[baseIndex + c]);
                    acc[a] += fn; acc[b] += fn; acc[c] += fn;
                    tris.Add(baseIndex + a); tris.Add(baseIndex + b); tris.Add(baseIndex + c);
                }
                foreach (var n in acc)
                    normals.Add(n.sqrMagnitude > 1e-12f ? n.normalized : Vector3.up);
            }
            else
            {
                // Exploded: unique verts per face, hard edges, per-FACE UVs with wrap fix.
                for (int t = 0; t < local.triangles.Count; t += 3)
                {
                    int a = local.triangles[t], b = local.triangles[t + 1], c = local.triangles[t + 2];
                    if (mirrored) { int tmp = b; b = c; c = tmp; }
                    Vector3 la = local.vertices[a], lb = local.vertices[b], lc = local.vertices[c];
                    Vector3 localFn = FaceNormal(la, lb, lc);
                    ForgeUV.ProjectTriangle(proj, la, lb, lc, localFn, localBounds,
                        out var ua, out var ub, out var uc);
                    Vector3 va = Xform(la), vb = Xform(lb), vc = Xform(lc);
                    Vector3 fn = FaceNormal(va, vb, vc);
                    int i0 = verts.Count;
                    verts.Add(va); verts.Add(vb); verts.Add(vc);
                    normals.Add(fn); normals.Add(fn); normals.Add(fn);
                    uvs.Add(ForgeUV.ToAtlas(ua, island));
                    uvs.Add(ForgeUV.ToAtlas(ub, island));
                    uvs.Add(ForgeUV.ToAtlas(uc, island));
                    tris.Add(i0); tris.Add(i0 + 1); tris.Add(i0 + 2);
                }
            }
        }

        private static Bounds LocalBounds(PartGeometry g)
        {
            if (g.vertices.Count == 0) return new Bounds(Vector3.zero, Vector3.one * 0.001f);
            var b = new Bounds(g.vertices[0], Vector3.zero);
            for (int i = 1; i < g.vertices.Count; i++) b.Encapsulate(g.vertices[i]);
            return b;
        }

        private static Vector3 FaceNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 n = Vector3.Cross(b - a, c - a);
            return n.sqrMagnitude > 1e-14f ? n.normalized : Vector3.up;
        }

        // ── Ops ──────────────────────────────────────────────────────────────

        private static void Quad(PartGeometry g, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            int i = g.vertices.Count;
            g.vertices.Add(a); g.vertices.Add(b); g.vertices.Add(c); g.vertices.Add(d);
            g.triangles.Add(i); g.triangles.Add(i + 1); g.triangles.Add(i + 2);
            g.triangles.Add(i); g.triangles.Add(i + 2); g.triangles.Add(i + 3);
        }

        private static void Tri(PartGeometry g, Vector3 a, Vector3 b, Vector3 c)
        {
            int i = g.vertices.Count;
            g.vertices.Add(a); g.vertices.Add(b); g.vertices.Add(c);
            g.triangles.Add(i); g.triangles.Add(i + 1); g.triangles.Add(i + 2);
        }

        private static void BuildBeveledBox(PartGeometry g, Vector3 size, float bevel)
        {
            Vector3 h = size * 0.5f;
            float c = Mathf.Min(bevel, Mathf.Min(h.x, Mathf.Min(h.y, h.z)) * 0.9f);

            if (c <= 0f)
            {
                // Plain box, 6 quads with outward winding (viewed from outside = counter-clockwise
                // for Unity's clockwise-front convention: order chosen per face below).
                Quad(g, V(h, 1, -1, -1), V(h, 1, 1, -1), V(h, 1, 1, 1), V(h, 1, -1, 1));      // +X
                Quad(g, V(h, -1, -1, 1), V(h, -1, 1, 1), V(h, -1, 1, -1), V(h, -1, -1, -1));  // -X
                Quad(g, V(h, -1, 1, -1), V(h, -1, 1, 1), V(h, 1, 1, 1), V(h, 1, 1, -1));      // +Y
                Quad(g, V(h, -1, -1, 1), V(h, -1, -1, -1), V(h, 1, -1, -1), V(h, 1, -1, 1));  // -Y
                Quad(g, V(h, -1, -1, 1), V(h, 1, -1, 1), V(h, 1, 1, 1), V(h, -1, 1, 1));      // +Z
                Quad(g, V(h, 1, -1, -1), V(h, -1, -1, -1), V(h, -1, 1, -1), V(h, 1, 1, -1));  // -Z
                return;
            }

            // Chamfered box: per corner (sx,sy,sz) three verts — one pulled in per axis.
            // Index helpers: corner(sx,sy,sz) → base of its 3 verts [xFace, yFace, zFace].
            var cornerBase = new Dictionary<int, int>();
            int Key(int sx, int sy, int sz) => (sx + 1) * 9 + (sy + 1) * 3 + (sz + 1);
            foreach (int sx in new[] { -1, 1 })
                foreach (int sy in new[] { -1, 1 })
                    foreach (int sz in new[] { -1, 1 })
                    {
                        cornerBase[Key(sx, sy, sz)] = g.vertices.Count;
                        g.vertices.Add(new Vector3(sx * h.x, sy * (h.y - c), sz * (h.z - c))); // on ±X face
                        g.vertices.Add(new Vector3(sx * (h.x - c), sy * h.y, sz * (h.z - c))); // on ±Y face
                        g.vertices.Add(new Vector3(sx * (h.x - c), sy * (h.y - c), sz * h.z)); // on ±Z face
                    }
            int VX(int sx, int sy, int sz) => cornerBase[Key(sx, sy, sz)];
            int VY(int sx, int sy, int sz) => cornerBase[Key(sx, sy, sz)] + 1;
            int VZ(int sx, int sy, int sz) => cornerBase[Key(sx, sy, sz)] + 2;

            void IQuad(int a, int b, int cc, int d)
            {
                g.triangles.Add(a); g.triangles.Add(b); g.triangles.Add(cc);
                g.triangles.Add(a); g.triangles.Add(cc); g.triangles.Add(d);
            }
            void ITri(int a, int b, int cc) { g.triangles.Add(a); g.triangles.Add(b); g.triangles.Add(cc); }

            // 6 inset faces.
            IQuad(VX(1, -1, -1), VX(1, 1, -1), VX(1, 1, 1), VX(1, -1, 1));       // +X
            IQuad(VX(-1, -1, 1), VX(-1, 1, 1), VX(-1, 1, -1), VX(-1, -1, -1));   // -X
            IQuad(VY(-1, 1, -1), VY(-1, 1, 1), VY(1, 1, 1), VY(1, 1, -1));       // +Y
            IQuad(VY(-1, -1, 1), VY(-1, -1, -1), VY(1, -1, -1), VY(1, -1, 1));   // -Y
            IQuad(VZ(-1, -1, 1), VZ(1, -1, 1), VZ(1, 1, 1), VZ(-1, 1, 1));       // +Z
            IQuad(VZ(1, -1, -1), VZ(-1, -1, -1), VZ(-1, 1, -1), VZ(1, 1, -1));   // -Z

            // 12 edge chamfer quads (X/Y edges run along Z, X/Z along Y, Y/Z along X).
            foreach (int sx in new[] { -1, 1 })
                foreach (int sy in new[] { -1, 1 })
                {
                    // edge between ±X and ±Y faces, spanning z=-1..1
                    if (sx * sy > 0) IQuad(VX(sx, sy, -1), VY(sx, sy, -1), VY(sx, sy, 1), VX(sx, sy, 1));
                    else IQuad(VX(sx, sy, 1), VY(sx, sy, 1), VY(sx, sy, -1), VX(sx, sy, -1));
                }
            foreach (int sx in new[] { -1, 1 })
                foreach (int sz in new[] { -1, 1 })
                {
                    if (sx * sz > 0) IQuad(VZ(sx, -1, sz), VX(sx, -1, sz), VX(sx, 1, sz), VZ(sx, 1, sz));
                    else IQuad(VZ(sx, 1, sz), VX(sx, 1, sz), VX(sx, -1, sz), VZ(sx, -1, sz));
                }
            foreach (int sy in new[] { -1, 1 })
                foreach (int sz in new[] { -1, 1 })
                {
                    if (sy * sz > 0) IQuad(VY(-1, sy, sz), VZ(-1, sy, sz), VZ(1, sy, sz), VY(1, sy, sz));
                    else IQuad(VY(1, sy, sz), VZ(1, sy, sz), VZ(-1, sy, sz), VY(-1, sy, sz));
                }

            // 8 corner triangles, wound outward.
            foreach (int sx in new[] { -1, 1 })
                foreach (int sy in new[] { -1, 1 })
                    foreach (int sz in new[] { -1, 1 })
                    {
                        if (sx * sy * sz > 0) ITri(VX(sx, sy, sz), VY(sx, sy, sz), VZ(sx, sy, sz));
                        else ITri(VX(sx, sy, sz), VZ(sx, sy, sz), VY(sx, sy, sz));
                    }
        }

        private static Vector3 V(Vector3 h, int sx, int sy, int sz) => new Vector3(sx * h.x, sy * h.y, sz * h.z);

        private static void BuildCylinder(PartGeometry g, float radiusX, float radiusZ, float height, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            float hy = height * 0.5f;
            int topStart = g.vertices.Count;
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                g.vertices.Add(new Vector3(Mathf.Cos(a) * radiusX, hy, Mathf.Sin(a) * radiusZ));
            }
            int botStart = g.vertices.Count;
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                g.vertices.Add(new Vector3(Mathf.Cos(a) * radiusX, -hy, Mathf.Sin(a) * radiusZ));
            }
            // Side (outward). Winding verified against Cross(b-a, c-a)·radial > 0 — the first cut of
            // this strip was inside-out and CI's ConvexOps_FaceOutward caught it.
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                g.triangles.Add(topStart + i); g.triangles.Add(botStart + j); g.triangles.Add(botStart + i);
                g.triangles.Add(topStart + i); g.triangles.Add(topStart + j); g.triangles.Add(botStart + j);
            }
            // Caps.
            int topC = g.vertices.Count; g.vertices.Add(new Vector3(0f, hy, 0f));
            int botC = g.vertices.Count; g.vertices.Add(new Vector3(0f, -hy, 0f));
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                g.triangles.Add(topC); g.triangles.Add(topStart + j); g.triangles.Add(topStart + i);
                g.triangles.Add(botC); g.triangles.Add(botStart + i); g.triangles.Add(botStart + j);
            }
        }

        private static void BuildTube(PartGeometry g, float outerR, float height, float wall, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            float innerR = Mathf.Max(0.001f, outerR - wall);
            float hy = height * 0.5f;
            int oTop = g.vertices.Count, oBot = oTop + n, iTop = oBot + n, iBot = iTop + n;
            for (int ring = 0; ring < 4; ring++)
            {
                float r = ring < 2 ? outerR : innerR;
                float y = (ring % 2 == 0) ? hy : -hy;
                for (int i = 0; i < n; i++)
                {
                    float a = i * Mathf.PI * 2f / n;
                    g.vertices.Add(new Vector3(Mathf.Cos(a) * r, y, Mathf.Sin(a) * r));
                }
            }
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                // Outer wall (radially outward).
                g.triangles.Add(oTop + i); g.triangles.Add(oBot + j); g.triangles.Add(oBot + i);
                g.triangles.Add(oTop + i); g.triangles.Add(oTop + j); g.triangles.Add(oBot + j);
                // Inner wall (radially inward — opposite strip winding).
                g.triangles.Add(iTop + i); g.triangles.Add(iBot + i); g.triangles.Add(iBot + j);
                g.triangles.Add(iTop + i); g.triangles.Add(iBot + j); g.triangles.Add(iTop + j);
                // Top ring cap (+Y).
                g.triangles.Add(oTop + i); g.triangles.Add(iTop + i); g.triangles.Add(iTop + j);
                g.triangles.Add(oTop + i); g.triangles.Add(iTop + j); g.triangles.Add(oTop + j);
                // Bottom ring cap (-Y).
                g.triangles.Add(oBot + i); g.triangles.Add(iBot + j); g.triangles.Add(iBot + i);
                g.triangles.Add(oBot + i); g.triangles.Add(oBot + j); g.triangles.Add(iBot + j);
            }
        }

        private static void BuildLathe(PartGeometry g, Vector2[] profile, float radiusScale, float height, int segments)
        {
            if (profile == null || profile.Length < 2) return;
            int n = Mathf.Clamp(segments, 3, 16);
            int rings = profile.Length;
            int start = g.vertices.Count;
            for (int r = 0; r < rings; r++)
            {
                float rad = Mathf.Max(0f, profile[r].x) * radiusScale;
                float y = (Mathf.Clamp01(profile[r].y) - 0.5f) * height;
                for (int i = 0; i < n; i++)
                {
                    float a = i * Mathf.PI * 2f / n;
                    g.vertices.Add(new Vector3(Mathf.Cos(a) * rad, y, Mathf.Sin(a) * rad));
                }
            }
            for (int r = 0; r < rings - 1; r++)
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    int lo = start + r * n, hi = start + (r + 1) * n;
                    g.triangles.Add(hi + i); g.triangles.Add(lo + j); g.triangles.Add(lo + i);
                    g.triangles.Add(hi + i); g.triangles.Add(hi + j); g.triangles.Add(lo + j);
                }
            // Caps where the profile is open.
            if (profile[0].x > 0.001f)
            {
                int c = g.vertices.Count;
                g.vertices.Add(new Vector3(0f, (Mathf.Clamp01(profile[0].y) - 0.5f) * height, 0f));
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    g.triangles.Add(c); g.triangles.Add(start + i); g.triangles.Add(start + j);
                }
            }
            if (profile[rings - 1].x > 0.001f)
            {
                int top = start + (rings - 1) * n;
                int c = g.vertices.Count;
                g.vertices.Add(new Vector3(0f, (Mathf.Clamp01(profile[rings - 1].y) - 0.5f) * height, 0f));
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    g.triangles.Add(c); g.triangles.Add(top + j); g.triangles.Add(top + i);
                }
            }
        }

        private static void BuildSphereSection(PartGeometry g, Vector3 radii, float latFraction, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            int latRings = Mathf.Max(2, n / 2);
            float latSpan = Mathf.PI * latFraction; // from the top pole downward
            int start = g.vertices.Count;
            g.vertices.Add(new Vector3(0f, radii.y, 0f)); // top pole
            for (int r = 1; r <= latRings; r++)
            {
                float lat = latSpan * r / latRings;           // 0 at pole
                float y = Mathf.Cos(lat) * radii.y;
                float ringScale = Mathf.Sin(lat);
                for (int i = 0; i < n; i++)
                {
                    float a = i * Mathf.PI * 2f / n;
                    g.vertices.Add(new Vector3(Mathf.Cos(a) * ringScale * radii.x, y, Mathf.Sin(a) * ringScale * radii.z));
                }
            }
            // Pole fan.
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                g.triangles.Add(start); g.triangles.Add(start + 1 + j); g.triangles.Add(start + 1 + i);
            }
            // Ring strips.
            for (int r = 0; r < latRings - 1; r++)
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    int hi = start + 1 + r * n, lo = start + 1 + (r + 1) * n;
                    g.triangles.Add(hi + i); g.triangles.Add(lo + j); g.triangles.Add(lo + i);
                    g.triangles.Add(hi + i); g.triangles.Add(hi + j); g.triangles.Add(lo + j);
                }
            // Open-bottom cap fan (also closes a full sphere's tiny bottom ring; harmless).
            int last = start + 1 + (latRings - 1) * n;
            float capY = Mathf.Cos(latSpan) * radii.y;
            int cc = g.vertices.Count;
            g.vertices.Add(new Vector3(0f, capY, 0f));
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                g.triangles.Add(cc); g.triangles.Add(last + i); g.triangles.Add(last + j);
            }
        }

        private static void BuildWedge(PartGeometry g, Vector3 size)
        {
            Vector3 h = size * 0.5f;
            // Bottom quad, back wall at -Z, slope from top back edge to bottom front edge, 2 side tris.
            Vector3 bl0 = new Vector3(-h.x, -h.y, -h.z), bl1 = new Vector3(h.x, -h.y, -h.z);
            Vector3 bf0 = new Vector3(-h.x, -h.y, h.z), bf1 = new Vector3(h.x, -h.y, h.z);
            Vector3 tb0 = new Vector3(-h.x, h.y, -h.z), tb1 = new Vector3(h.x, h.y, -h.z);
            Quad(g, bf0, bl0, bl1, bf1);   // bottom (-Y)
            Quad(g, bl1, bl0, tb0, tb1);   // back (-Z)
            Quad(g, bf0, bf1, tb1, tb0);   // slope (+Z/+Y)
            Tri(g, bl0, bf0, tb0);         // -X side
            Tri(g, bf1, bl1, tb1);         // +X side
        }

        private static void BuildGreebleStrip(PartGeometry g, Vector3 size, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            float cell = size.z / n;
            for (int i = 0; i < n; i++)
            {
                float h01 = 0.3f + 0.7f * Hash01(i, n);
                float bh = size.y * h01;
                Vector3 c = new Vector3(0f, -size.y * 0.5f + bh * 0.5f, -size.z * 0.5f + (i + 0.5f) * cell);
                Vector3 half = new Vector3(size.x * 0.5f, bh * 0.5f, cell * 0.4f);
                // Inline box (plain, 12 tris) at offset c.
                Quad(g, c + V(half, 1, -1, -1), c + V(half, 1, 1, -1), c + V(half, 1, 1, 1), c + V(half, 1, -1, 1));
                Quad(g, c + V(half, -1, -1, 1), c + V(half, -1, 1, 1), c + V(half, -1, 1, -1), c + V(half, -1, -1, -1));
                Quad(g, c + V(half, -1, 1, -1), c + V(half, -1, 1, 1), c + V(half, 1, 1, 1), c + V(half, 1, 1, -1));
                Quad(g, c + V(half, -1, -1, 1), c + V(half, -1, -1, -1), c + V(half, 1, -1, -1), c + V(half, 1, -1, 1));
                Quad(g, c + V(half, -1, -1, 1), c + V(half, 1, -1, 1), c + V(half, 1, 1, 1), c + V(half, -1, 1, 1));
                Quad(g, c + V(half, 1, -1, -1), c + V(half, -1, -1, -1), c + V(half, -1, 1, -1), c + V(half, 1, 1, -1));
            }
        }

        /// <summary>Deterministic integer hash → [0,1) (independent of SkyVistaTexture on purpose).</summary>
        public static float Hash01(int i, int n)
        {
            unchecked
            {
                int h = i * 374761393 + n * 668265263 + 1013904223;
                h = (h ^ (h >> 13)) * 1103515245;
                h ^= h >> 16;
                return (h & 0x7FFFFFFF) / 2147483647f;
            }
        }
    }
}
