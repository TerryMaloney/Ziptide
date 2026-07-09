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

        /// <summary>E1.3: Build with every palette-slot submesh collapsed into ONE — the single
        /// textured-material path (per-slot identity lives in the baked atlas maps now, so
        /// submeshes would only cost extra draw calls).</summary>
        public static Mesh BuildSingle(ForgeRecipeDefinition recipe)
        {
            var mesh = Build(recipe);
            if (mesh.subMeshCount > 1)
            {
                var all = new List<int>();
                for (int s = 0; s < mesh.subMeshCount; s++) all.AddRange(mesh.GetTriangles(s));
                mesh.subMeshCount = 1;
                mesh.SetTriangles(all, 0);
            }
            return mesh;
        }

        /// <summary>Indexed local-space geometry for one part (before transform/mirror/shading).
        /// P2 modifiers (taper → bend → noise) apply here, pre-transform, on any op.</summary>
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
                // P2 geometry richness:
                case ForgeOp.Capsule: BuildCapsule(g, part.size.x * 0.5f, part.size.y, part.segments); break;
                case ForgeOp.Frustum: BuildFrustum(g, part.size.x * 0.5f, part.size.z * 0.5f, part.size.y, part.segments); break;
                case ForgeOp.Torus: BuildTorus(g, part.size.x * 0.5f, part.size.y * 0.5f, part.segments); break;
                case ForgeOp.SweepSpline: BuildSweepSpline(g, part); break;
                case ForgeOp.OrganicBlob: BuildUvSphere(g, part.size * 0.5f, part.segments); break;
            }
            ApplyModifiers(g, part);
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

        // ── P2 ops (FORGE_II_QUALITY_LEAP §P2) ───────────────────────────────
        // Winding conventions are copied verbatim from the CI-verified ops above:
        //   strip (hi = upper ring): (hi+i, lo+j, lo+i) + (hi+i, hi+j, lo+j)
        //   top pole/cap fan:        (pole, ring+j, ring+i)
        //   bottom pole/cap fan:     (pole, ring+i, ring+j)

        private static int AddYRing(PartGeometry g, float ringRadius, float y, int n)
        {
            int start = g.vertices.Count;
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                g.vertices.Add(new Vector3(Mathf.Cos(a) * ringRadius, y, Mathf.Sin(a) * ringRadius));
            }
            return start;
        }

        private static void StitchRows(PartGeometry g, List<int> rows, int n)
        {
            for (int r = 0; r < rows.Count - 1; r++)
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    int hi = rows[r], lo = rows[r + 1];
                    g.triangles.Add(hi + i); g.triangles.Add(lo + j); g.triangles.Add(lo + i);
                    g.triangles.Add(hi + i); g.triangles.Add(hi + j); g.triangles.Add(lo + j);
                }
        }

        private static void PoleFanTop(PartGeometry g, int pole, int ring, int n)
        {
            for (int i = 0; i < n; i++)
            { int j = (i + 1) % n; g.triangles.Add(pole); g.triangles.Add(ring + j); g.triangles.Add(ring + i); }
        }

        private static void PoleFanBottom(PartGeometry g, int pole, int ring, int n)
        {
            for (int i = 0; i < n; i++)
            { int j = (i + 1) % n; g.triangles.Add(pole); g.triangles.Add(ring + i); g.triangles.Add(ring + j); }
        }

        /// <summary>Capsule: a cylinder wall with hemispherical dome caps. totalHeight includes the caps.</summary>
        private static void BuildCapsule(PartGeometry g, float radius, float totalHeight, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            int latRings = Mathf.Max(2, n / 2);
            float r = Mathf.Min(radius, totalHeight * 0.5f);
            float cylHalf = Mathf.Max(0f, totalHeight * 0.5f - r);

            int topPole = g.vertices.Count;
            g.vertices.Add(new Vector3(0f, cylHalf + r, 0f));
            var rows = new List<int>();
            for (int rr = 1; rr <= latRings; rr++) // top dome, ending on the +cylHalf equator
            {
                float lat = Mathf.PI * 0.5f * rr / latRings;
                rows.Add(AddYRing(g, Mathf.Sin(lat) * r, cylHalf + Mathf.Cos(lat) * r, n));
            }
            // Bottom dome from the -cylHalf equator down. When there is no straight wall
            // (a pure sphere) skip the duplicate equator ring.
            int firstBottom = cylHalf > 1e-5f ? 0 : 1;
            for (int rr = firstBottom; rr <= latRings - 1; rr++)
            {
                float lat = Mathf.PI * 0.5f + Mathf.PI * 0.5f * rr / latRings;
                rows.Add(AddYRing(g, Mathf.Sin(lat) * r, -cylHalf + Mathf.Cos(lat) * r, n));
            }
            int botPole = g.vertices.Count;
            g.vertices.Add(new Vector3(0f, -cylHalf - r, 0f));

            PoleFanTop(g, topPole, rows[0], n);
            StitchRows(g, rows, n);
            PoleFanBottom(g, botPole, rows[rows.Count - 1], n);
        }

        /// <summary>Frustum: truncated cone (topR = 0 → a true cone with an apex vertex).</summary>
        private static void BuildFrustum(PartGeometry g, float bottomR, float topR, float height, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            float hy = height * 0.5f;
            int bot = AddYRing(g, Mathf.Max(0.001f, bottomR), -hy, n);
            if (topR < 1e-4f)
            {
                int apex = g.vertices.Count;
                g.vertices.Add(new Vector3(0f, hy, 0f));
                PoleFanTop(g, apex, bot, n); // collapsed side strip = apex fan (cylinder winding)
            }
            else
            {
                int top = AddYRing(g, topR, hy, n);
                var rows = new List<int> { top, bot };
                StitchRows(g, rows, n);
                int topC = g.vertices.Count; g.vertices.Add(new Vector3(0f, hy, 0f));
                PoleFanTop(g, topC, top, n);
            }
            int botC = g.vertices.Count; g.vertices.Add(new Vector3(0f, -hy, 0f));
            PoleFanBottom(g, botC, bot, n);
        }

        /// <summary>Torus lying in the XZ plane. majorR = ring radius (tube centers), minorR = tube radius.</summary>
        private static void BuildTorus(PartGeometry g, float majorR, float minorR, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);   // around the ring (theta)
            int m = Mathf.Max(3, n / 2);            // around the tube (phi)
            int start = g.vertices.Count;
            for (int i = 0; i < n; i++)
            {
                float th = i * Mathf.PI * 2f / n;
                for (int j = 0; j < m; j++)
                {
                    float ph = j * Mathf.PI * 2f / m;
                    float rad = majorR + Mathf.Cos(ph) * minorR;
                    g.vertices.Add(new Vector3(Mathf.Cos(th) * rad, Mathf.Sin(ph) * minorR, Mathf.Sin(th) * rad));
                }
            }
            // Outward winding derived analytically: Cross(T_theta, T_phi) points INWARD on this
            // parametrization, so quads emit (a,d,c)+(a,c,b) — verified by the analytic-normal test.
            for (int i = 0; i < n; i++)
            {
                int i2 = (i + 1) % n;
                for (int j = 0; j < m; j++)
                {
                    int j2 = (j + 1) % m;
                    int a = start + i * m + j, b = start + i2 * m + j, c = start + i2 * m + j2, d = start + i * m + j2;
                    g.triangles.Add(a); g.triangles.Add(d); g.triangles.Add(c);
                    g.triangles.Add(a); g.triangles.Add(c); g.triangles.Add(b);
                }
            }
        }

        /// <summary>Full UV ellipsoid, pole to pole (OrganicBlob's base; noise makes it organic).</summary>
        private static void BuildUvSphere(PartGeometry g, Vector3 radii, int segments)
        {
            int n = Mathf.Clamp(segments, 3, 16);
            int latDiv = Mathf.Max(2, n / 2);
            int topPole = g.vertices.Count;
            g.vertices.Add(new Vector3(0f, radii.y, 0f));
            var rows = new List<int>();
            for (int r = 1; r < latDiv; r++)
            {
                float lat = Mathf.PI * r / latDiv;
                int row = g.vertices.Count;
                for (int i = 0; i < n; i++)
                {
                    float a = i * Mathf.PI * 2f / n;
                    g.vertices.Add(new Vector3(
                        Mathf.Cos(a) * Mathf.Sin(lat) * radii.x,
                        Mathf.Cos(lat) * radii.y,
                        Mathf.Sin(a) * Mathf.Sin(lat) * radii.z));
                }
                rows.Add(row);
            }
            int botPole = g.vertices.Count;
            g.vertices.Add(new Vector3(0f, -radii.y, 0f));
            PoleFanTop(g, topPole, rows[0], n);
            StitchRows(g, rows, n);
            PoleFanBottom(g, botPole, rows[rows.Count - 1], n);
        }

        /// <summary>Tube swept along a 2..4-point bezier with a parallel-transported frame (no twist
        /// pops). Radius = size.x/2, or per-control-point via profile[i].x. Tentacles/pipes/branches.</summary>
        private static void BuildSweepSpline(PartGeometry g, ForgePart part)
        {
            var pts = part.spline;
            if (pts == null || pts.Length < 2 || pts.Length > 4) return;
            int n = Mathf.Clamp(part.segments, 3, 16);
            int rings = Mathf.Max(4, n);
            float baseR = Mathf.Max(0.001f, part.size.x * 0.5f);

            var rows = new List<int>();
            Vector3 prevN = Vector3.zero;
            for (int rr = 0; rr <= rings; rr++)
            {
                float t = (float)rr / rings;
                Vector3 c = Bezier(pts, t);
                Vector3 T = BezierTangent(pts, t);
                Vector3 N;
                if (rr == 0)
                {
                    // Chosen so a straight -Y sweep reproduces the CI-verified cylinder layout exactly.
                    Vector3 refv = Mathf.Abs(T.y) > 0.7f ? Vector3.forward : Vector3.up;
                    N = Vector3.Cross(refv, T);
                }
                else N = prevN - T * Vector3.Dot(prevN, T); // parallel transport
                if (N.sqrMagnitude < 1e-10f) N = Vector3.Cross(Vector3.right, T);
                N.Normalize();
                prevN = N;
                Vector3 B = Vector3.Cross(T, N);
                float rad = SweepRadiusAt(part, t, baseR, pts.Length);

                int row = g.vertices.Count;
                for (int i = 0; i < n; i++)
                {
                    float a = i * Mathf.PI * 2f / n;
                    g.vertices.Add(c + (N * Mathf.Cos(a) + B * Mathf.Sin(a)) * rad);
                }
                rows.Add(row);
            }
            // A reversed sweep flips BOTH the axial order and the frame handedness, so this one strip
            // pattern stays outward for any curve direction (same cancellation as the cylinder).
            StitchRows(g, rows, n);
            int c0 = g.vertices.Count; g.vertices.Add(Bezier(pts, 0f));
            PoleFanTop(g, c0, rows[0], n);
            int c1 = g.vertices.Count; g.vertices.Add(Bezier(pts, 1f));
            PoleFanBottom(g, c1, rows[rows.Count - 1], n);
        }

        private static Vector3 Bezier(Vector3[] p, float t)
        {
            if (p.Length == 2) return Vector3.LerpUnclamped(p[0], p[1], t);
            if (p.Length == 3)
            {
                Vector3 a = Vector3.LerpUnclamped(p[0], p[1], t);
                Vector3 b = Vector3.LerpUnclamped(p[1], p[2], t);
                return Vector3.LerpUnclamped(a, b, t);
            }
            Vector3 q0 = Vector3.LerpUnclamped(p[0], p[1], t);
            Vector3 q1 = Vector3.LerpUnclamped(p[1], p[2], t);
            Vector3 q2 = Vector3.LerpUnclamped(p[2], p[3], t);
            Vector3 r0 = Vector3.LerpUnclamped(q0, q1, t);
            Vector3 r1 = Vector3.LerpUnclamped(q1, q2, t);
            return Vector3.LerpUnclamped(r0, r1, t);
        }

        private static Vector3 BezierTangent(Vector3[] p, float t)
        {
            Vector3 d = Bezier(p, Mathf.Min(1f, t + 0.001f)) - Bezier(p, Mathf.Max(0f, t - 0.001f));
            return d.sqrMagnitude > 1e-12f ? d.normalized : Vector3.up;
        }

        private static float SweepRadiusAt(ForgePart part, float t, float baseR, int ctrlCount)
        {
            var prof = part.profile;
            if (prof == null || prof.Length != ctrlCount) return baseR;
            float f = t * (ctrlCount - 1);
            int i = Mathf.Clamp(Mathf.FloorToInt(f), 0, ctrlCount - 2);
            return Mathf.Max(0.001f, Mathf.Lerp(prof[i].x, prof[i + 1].x, f - i));
        }

        // ── P2 modifiers (taper → bend → noise; local space, pre-transform, any op) ──

        private static void ApplyModifiers(PartGeometry g, ForgePart part)
        {
            if (g.vertices.Count == 0) return;
            bool taper = part.taper > 1e-4f;
            bool bend = Mathf.Abs(part.bendDegrees) > 0.01f;
            bool noise = part.noiseAmplitude > 1e-5f;
            if (!taper && !bend && !noise) return;

            Bounds b = LocalBounds(g);
            float minY = b.min.y, spanY = Mathf.Max(1e-4f, b.size.y);

            if (taper)
            {
                float k = Mathf.Clamp(part.taper, 0f, 0.95f);
                for (int i = 0; i < g.vertices.Count; i++)
                {
                    Vector3 v = g.vertices[i];
                    float s = 1f - k * Mathf.Clamp01((v.y - minY) / spanY);
                    v.x *= s; v.z *= s;
                    g.vertices[i] = v;
                }
            }
            if (bend)
            {
                // Arc bend around local X: the base slice (y = minY) stays put, the +Y axis curves
                // toward ±Z along a circle of radius spanY/bendRadians. Verified: theta=0 is identity.
                float rad = Mathf.Clamp(part.bendDegrees, -180f, 180f) * Mathf.Deg2Rad;
                float R = spanY / rad;
                for (int i = 0; i < g.vertices.Count; i++)
                {
                    Vector3 v = g.vertices[i];
                    float th = (v.y - minY) / R;
                    float arm = R - v.z;
                    v.y = minY + Mathf.Sin(th) * arm;
                    v.z = R - Mathf.Cos(th) * arm;
                    g.vertices[i] = v;
                }
            }
            if (noise)
            {
                // Displace along POSITION-WELDED vertex normals: duplicated corner verts (Quad-built
                // ops) get one shared normal, so noise can never crack a hard-edged part open.
                var normals = WeldedNormals(g);
                float amp = Mathf.Clamp(part.noiseAmplitude, 0f, 0.5f);
                float freq = Mathf.Max(0.01f, part.noiseFrequency);
                for (int i = 0; i < g.vertices.Count; i++)
                    g.vertices[i] += normals[i] * (amp * Fbm(g.vertices[i] * freq, part.noiseSeed));
            }
        }

        private static Vector3[] WeldedNormals(PartGeometry g)
        {
            var keyOf = new int[g.vertices.Count];
            var keyMap = new Dictionary<Vector3Int, int>();
            var acc = new List<Vector3>();
            for (int i = 0; i < g.vertices.Count; i++)
            {
                Vector3 v = g.vertices[i];
                var q = new Vector3Int(Mathf.RoundToInt(v.x * 10000f), Mathf.RoundToInt(v.y * 10000f),
                                       Mathf.RoundToInt(v.z * 10000f));
                if (!keyMap.TryGetValue(q, out int k)) { k = acc.Count; keyMap[q] = k; acc.Add(Vector3.zero); }
                keyOf[i] = k;
            }
            for (int t = 0; t < g.triangles.Count; t += 3)
            {
                Vector3 a = g.vertices[g.triangles[t]];
                Vector3 fn = Vector3.Cross(g.vertices[g.triangles[t + 1]] - a, g.vertices[g.triangles[t + 2]] - a);
                acc[keyOf[g.triangles[t]]] += fn;      // unnormalized cross = area-weighted
                acc[keyOf[g.triangles[t + 1]]] += fn;
                acc[keyOf[g.triangles[t + 2]]] += fn;
            }
            var result = new Vector3[g.vertices.Count];
            for (int i = 0; i < g.vertices.Count; i++)
            {
                Vector3 nv = acc[keyOf[i]];
                result[i] = nv.sqrMagnitude > 1e-12f ? nv.normalized : Vector3.up;
            }
            return result;
        }

        private static float Hash3(int x, int y, int z, int seed)
        {
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + z * 2147483629 + seed * 1013904223;
                h = (h ^ (h >> 13)) * 1103515245;
                h ^= h >> 16;
                return (h & 0x7FFFFFFF) / 2147483647f;
            }
        }

        private static float ValueNoise(Vector3 p, int seed)
        {
            int x0 = Mathf.FloorToInt(p.x), y0 = Mathf.FloorToInt(p.y), z0 = Mathf.FloorToInt(p.z);
            float fx = p.x - x0, fy = p.y - y0, fz = p.z - z0;
            fx = fx * fx * (3f - 2f * fx); fy = fy * fy * (3f - 2f * fy); fz = fz * fz * (3f - 2f * fz);
            float x00 = Mathf.Lerp(Hash3(x0, y0, z0, seed), Hash3(x0 + 1, y0, z0, seed), fx);
            float x10 = Mathf.Lerp(Hash3(x0, y0 + 1, z0, seed), Hash3(x0 + 1, y0 + 1, z0, seed), fx);
            float x01 = Mathf.Lerp(Hash3(x0, y0, z0 + 1, seed), Hash3(x0 + 1, y0, z0 + 1, seed), fx);
            float x11 = Mathf.Lerp(Hash3(x0, y0 + 1, z0 + 1, seed), Hash3(x0 + 1, y0 + 1, z0 + 1, seed), fx);
            return Mathf.Lerp(Mathf.Lerp(x00, x10, fy), Mathf.Lerp(x01, x11, fy), fz);
        }

        /// <summary>Deterministic 3-octave fBm in [-1, 1] (pure — the modifier and tests share it).</summary>
        public static float Fbm(Vector3 p, int seed)
        {
            float sum = 0f, ampSum = 0f, a = 1f;
            for (int o = 0; o < 3; o++)
            {
                sum += a * ValueNoise(p, seed + o * 101);
                ampSum += a;
                a *= 0.5f;
                p *= 2f;
            }
            return (sum / ampSum) * 2f - 1f;
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
