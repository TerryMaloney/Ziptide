#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE WORLD EXPERIENCE ENGINE, stage 1 of the Quality Bar Program (P1a terrain + P1b vista).
    /// Consumes <see cref="CityLayoutDefinition.experience"/> and builds, under the same city root:
    ///
    ///  1. TERRAIN — a seeded heightfield ground mesh (250–400m playable) shaped by the biome preset,
    ///     rising into a natural cliff bowl at worldRadius so the bound reads as landscape, not a wall.
    ///     Districts/connections/berth/spawn are FLATTENED INTO the terrain (smooth graded pads and
    ///     corridors), so the existing recipe's slabs become settlements on real land and every
    ///     authored contract keeps working. The mesh is saved as a project asset (stable GUID, no
    ///     scene-YAML bloat) and gets a MeshCollider — walkable by construction: amplitude is clamped
    ///     against the noise wavelength so slopes stay under comfortable walking grade.
    ///  2. ARRIVAL VISTA — a hero landmark assembly (40–80m: gate spire / wreck / monolith / crystal
    ///     forest / arch ring) at vistaDistance along vistaDirection from spawn, plus midground rock
    ///     clusters staged between, so the first look into the world is composed, with depth. Fog is
    ///     auto-thinned so the landmark actually reads through it.
    ///
    /// Deterministic: all shapes come from a seed-keyed integer hash (NOT UnityEngine.Random), so
    /// regeneration is stable regardless of build order. Pure data in → geometry out: a mid-level LLM
    /// authors worlds by filling the ExperienceDef table (docs/WORLD_RECIPE.md), never by editing this.
    /// </summary>
    public static class WorldExperienceBuilder
    {
        private const string TerrainFolder = "Assets/Ziptide/Content/Worlds/Terrain";
        private const float CliffHeight = 38f;       // bowl rim rise at the world edge
        private const float BaseWavelength = 85f;    // dominant noise feature size (m)
        private const float MaxSlopeRatio = 0.24f;   // amplitude clamp: rise per meter ≈ 13° worst case
        private const float CellSize = 5f;           // target mesh resolution (m per quad)

        /// <summary>Build terrain + vista for the kit. No-op when the experience block is disabled.</summary>
        public static void Build(Transform root, CityLayoutDefinition kit)
        {
            var ex = kit != null ? kit.experience : null;
            if (root == null || ex == null || !ex.enabled) return;

            var flats = CollectFlattenSites(kit);
            BuildTerrain(root, kit, ex, flats);
            BuildVista(root, kit, ex);
            TuneAtmosphere(ex);
        }

        /// <summary>World yaw (degrees) the player spawn should face — toward the vista. Null vista = 0.</summary>
        public static float SpawnYawDegrees(CityLayoutDefinition kit)
        {
            var ex = kit != null ? kit.experience : null;
            if (ex == null || !ex.enabled || ex.vista == VistaKind.None) return 0f;
            var d = new Vector2(ex.vistaDirection.x, ex.vistaDirection.z);
            if (d.sqrMagnitude < 0.001f) return 0f;
            return Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;
        }

        // ── Deterministic value noise (seed-keyed hash; NEVER UnityEngine.Random) ────────────────

        private static float Hash01(int x, int z, int seed)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + z * 668265263 + seed * 1274126177);
                h = (h ^ (h >> 13)) * 1274126177u;
                return ((h ^ (h >> 16)) & 0xFFFFFF) / (float)0x1000000;
            }
        }

        // ── Shared height pipeline (BuildTerrain vertices + HeightAt placement MUST match) ────────
        // H3 (ARCHITECTURE V2 Q3): the biome math lives in the PURE, tested Content.TerrainField
        // (fBM + domain warp + per-biome parameter sets, internal slope-safety clamp). This builder
        // only adds the cliff-bowl rim and the flatten pipeline — scene code translates.

        private static float Amp(ExperienceDef ex) => Mathf.Min(ex.heightAmplitude, BaseWavelength * MaxSlopeRatio);
        private static float Radius(ExperienceDef ex) => Mathf.Max(60f, ex.worldRadius);

        /// <summary>Biome height + cliff-bowl rim, BEFORE any flattening. Relative to walkwayHeight.</summary>
        private static float RawHeight(CityLayoutDefinition kit, ExperienceDef ex, float x, float z)
        {
            float h = TerrainField.Height(ex.biome, x, z, Amp(ex), kit.seed);
            float radius = Radius(ex);
            float r = Mathf.Sqrt(x * x + z * z);
            float rim = Mathf.Clamp01((r - radius * 0.82f) / (radius * 0.18f));
            return h + rim * rim * CliffHeight;
        }

        /// <summary>
        /// Final terrain height (world Y) at an XZ point — the placement API for POI/marker/prop
        /// builders. Identical math to the mesh vertices, so placed objects sit ON the ground.
        /// </summary>
        public static float HeightAt(CityLayoutDefinition kit, float x, float z)
        {
            var ex = kit != null ? kit.experience : null;
            if (ex == null || !ex.enabled) return kit != null ? kit.walkwayHeight : 0f;
            var flats = CollectFlattenSites(kit);
            float h = RawHeight(kit, ex, x, z);
            float target;
            float w = FlattenWeight(new Vector2(x, z), flats, out target);
            return kit.walkwayHeight + Mathf.Lerp(h, target, w);
        }

        // ── Flatten sites: districts, connection corridors, berth, POI pads ──────────────────────

        private struct FlatSite
        {
            public Vector2 a, b;        // segment (a==b for a disc)
            public float innerRadius;   // fully flat inside this
            public float feather;       // blend band beyond it
            public float targetY;       // height (rel. walkway) the site grades to; NaN = walkway pad
        }

        private static List<FlatSite> CollectFlattenSites(CityLayoutDefinition kit)
        {
            var flats = new List<FlatSite>();
            var ex = kit.experience;

            foreach (var d in kit.districts)
            {
                if (d == null) continue;
                var c = new Vector2(d.anchor.x, d.anchor.z);
                float r = Mathf.Max(d.bounds.x, d.bounds.y) * 0.5f + 6f;
                flats.Add(new FlatSite { a = c, b = c, innerRadius = r, feather = 18f, targetY = float.NaN });
            }

            // POI pads (P1c): a level pocket graded to the NATURAL height at its center, so distant
            // POIs sit in the landscape instead of sinking to spawn level. TravelBerth POIs skip —
            // they stand on existing berth/district pads.
            foreach (var p in kit.pois)
            {
                if (p == null || p.type == PoiType.TravelBerth) continue;
                var c = new Vector2(p.position.x, p.position.z);
                flats.Add(new FlatSite
                {
                    a = c, b = c, innerRadius = 13f, feather = 15f,
                    targetY = RawHeight(kit, ex, c.x, c.y)
                });
            }

            foreach (var conn in kit.connections)
            {
                if (conn == null) continue;
                DistrictDef from = null, to = null;
                foreach (var d in kit.districts)
                {
                    if (d == null) continue;
                    if (d.id == conn.fromDistrictId) from = d;
                    if (d.id == conn.toDistrictId) to = d;
                }
                if (from == null || to == null) continue;
                flats.Add(new FlatSite
                {
                    a = new Vector2(from.anchor.x, from.anchor.z),
                    b = new Vector2(to.anchor.x, to.anchor.z),
                    innerRadius = conn.width * 0.5f + 3f,
                    feather = 14f,
                    targetY = float.NaN
                });
            }

            if (kit.shipyard != null && kit.shipyard.enabled)
            {
                var c = new Vector2(kit.shipyard.berthCenter.x, kit.shipyard.berthCenter.z);
                float r = Mathf.Max(kit.shipyard.berthSize.x, kit.shipyard.berthSize.y) * 0.5f + 6f;
                flats.Add(new FlatSite { a = c, b = c, innerRadius = r, feather = 18f, targetY = float.NaN });
            }

            return flats;
        }

        private static float DistToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float len2 = ab.sqrMagnitude;
            if (len2 < 0.0001f) return Vector2.Distance(p, a);
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / len2);
            return Vector2.Distance(p, a + ab * t);
        }

        /// <summary>0 = leave terrain; 1 = force to the winning site's target height. The strongest
        /// site decides the target (NaN target = the -0.04 walkway pad).</summary>
        private static float FlattenWeight(Vector2 p, List<FlatSite> flats, out float targetY)
        {
            float w = 0f;
            targetY = -0.04f;
            for (int i = 0; i < flats.Count; i++)
            {
                float dist = DistToSegment(p, flats[i].a, flats[i].b);
                float t = 1f - Mathf.Clamp01((dist - flats[i].innerRadius) / flats[i].feather);
                t = t * t * (3f - 2f * t); // smoothstep the band
                if (t > w)
                {
                    w = t;
                    targetY = float.IsNaN(flats[i].targetY) ? -0.04f : flats[i].targetY;
                }
            }
            return w;
        }

        // ── Terrain mesh ──────────────────────────────────────────────────────────────────────────

        private static void BuildTerrain(Transform root, CityLayoutDefinition kit, ExperienceDef ex, List<FlatSite> flats)
        {
            float radius = Radius(ex);
            float extent = radius + 50f; // run past the rim so the bowl edge has ground behind it
            int res = Mathf.Clamp(Mathf.RoundToInt(extent * 2f / CellSize), 32, 220);

            var verts = new Vector3[(res + 1) * (res + 1)];
            var uvs = new Vector2[verts.Length];
            var tris = new int[res * res * 6];

            for (int zi = 0; zi <= res; zi++)
            {
                for (int xi = 0; xi <= res; xi++)
                {
                    float x = -extent + xi * (extent * 2f / res);
                    float z = -extent + zi * (extent * 2f / res);

                    // Same pipeline as HeightAt: raw biome+rim height, then graded pads/corridors.
                    float h = RawHeight(kit, ex, x, z);
                    float target;
                    float flat = FlattenWeight(new Vector2(x, z), flats, out target);
                    h = Mathf.Lerp(h, target, flat);

                    int i = zi * (res + 1) + xi;
                    verts[i] = new Vector3(x, kit.walkwayHeight + h, z);
                    uvs[i] = new Vector2(x, z) / 19f;
                }
            }

            int t6 = 0;
            for (int zi = 0; zi < res; zi++)
            {
                for (int xi = 0; xi < res; xi++)
                {
                    int i = zi * (res + 1) + xi;
                    tris[t6++] = i; tris[t6++] = i + res + 1; tris[t6++] = i + 1;
                    tris[t6++] = i + 1; tris[t6++] = i + res + 1; tris[t6++] = i + res + 2;
                }
            }

            var mesh = EnsureTerrainMeshAsset(kit.cityId);
            mesh.Clear();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            EditorUtility.SetDirty(mesh);

            var existing = root.Find("ExperienceTerrain");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);
            var go = new GameObject("ExperienceTerrain");
            go.transform.SetParent(root, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = MakeMat("TerrainMat_" + kit.cityId, ex.groundColor);
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;

            Debug.Log("[Ziptide] ExperienceTerrain '" + kit.cityId + "' biome=" + ex.biome +
                      " radius=" + radius + " verts=" + verts.Length);
        }

        private static Mesh EnsureTerrainMeshAsset(string cityId)
        {
            System.IO.Directory.CreateDirectory(TerrainFolder);
            string path = TerrainFolder + "/" + cityId + "_Terrain.asset";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh == null)
            {
                mesh = new Mesh { name = cityId + "_Terrain" };
                AssetDatabase.CreateAsset(mesh, path);
            }
            return mesh;
        }

        // ── Arrival vista (P1b) ───────────────────────────────────────────────────────────────────

        private static void BuildVista(Transform root, CityLayoutDefinition kit, ExperienceDef ex)
        {
            if (ex.vista == VistaKind.None) return;

            var spawnDistrict = kit.districts.Count > 0 ? kit.districts[0] : null;
            foreach (var d in kit.districts)
                if (d != null && d.id == kit.spawnDistrictId) spawnDistrict = d;
            Vector3 spawn = spawnDistrict != null ? spawnDistrict.anchor : Vector3.zero;

            Vector3 dir = new Vector3(ex.vistaDirection.x, 0f, ex.vistaDirection.z).normalized;
            if (dir.sqrMagnitude < 0.5f) dir = Vector3.forward;
            Vector3 heroPos = spawn + dir * ex.vistaDistance;
            Vector3 side = Vector3.Cross(Vector3.up, dir);

            var existing = root.Find("ArrivalVista");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);
            var vistaRoot = new GameObject("ArrivalVista").transform;
            vistaRoot.SetParent(root, false);

            BuildHeroLandmark(vistaRoot, kit, ex, heroPos, dir);

            // Midground interest: staged rock/ruin clusters so the sightline has depth, offset off
            // the direct line so they frame the hero instead of hiding it.
            for (int i = 0; i < 3; i++)
            {
                float f = 0.32f + 0.18f * i;
                float lateral = (i % 2 == 0 ? 1f : -1f) * (22f + 14f * Hash01(i, 7, kit.seed));
                Vector3 c = spawn + dir * (ex.vistaDistance * f) + side * lateral;
                BuildRockCluster(vistaRoot, kit, ex, c, 5f + 4f * i, i * 31);
            }
        }

        private static void BuildHeroLandmark(Transform parent, CityLayoutDefinition kit, ExperienceDef ex,
            Vector3 pos, Vector3 dir)
        {
            float H = Mathf.Clamp(ex.vistaHeight, 25f, 90f);
            var hero = new GameObject("Hero_" + ex.vista).transform;
            hero.SetParent(parent, false);
            hero.localPosition = new Vector3(pos.x, kit.walkwayHeight, pos.z);
            hero.localRotation = Quaternion.LookRotation(-dir, Vector3.up); // face the spawn

            Color body = ex.vistaColor;
            Color glow = ex.vistaAccentColor;

            switch (ex.vista)
            {
                case VistaKind.GateSpire:
                    // Tapered stacked shaft + a glowing halo ring near the top.
                    for (int i = 0; i < 6; i++)
                    {
                        float w = Mathf.Lerp(H * 0.16f, H * 0.05f, i / 5f);
                        float segH = H / 6f;
                        Block(hero, "Seg" + i, new Vector3(0f, segH * (i + 0.5f), 0f),
                            new Vector3(w, segH * 1.02f, w), body);
                    }
                    Ring(hero, new Vector3(0f, H * 0.82f, 0f), H * 0.18f, H * 0.018f, glow);
                    break;

                case VistaKind.Wreck:
                    // A colossal hull broken mid-ship, bow buried nose-down.
                    var bow = Block(hero, "Bow", new Vector3(-H * 0.18f, H * 0.22f, 0f),
                        new Vector3(H * 0.16f, H * 0.55f, H * 0.28f), body);
                    bow.transform.localRotation = Quaternion.Euler(0f, 0f, 34f);
                    var stern = Block(hero, "Stern", new Vector3(H * 0.28f, H * 0.16f, H * 0.06f),
                        new Vector3(H * 0.5f, H * 0.24f, H * 0.26f), body);
                    stern.transform.localRotation = Quaternion.Euler(4f, 14f, -9f);
                    for (int i = 0; i < 4; i++) // exposed ribs between the halves
                        Block(hero, "Rib" + i, new Vector3(H * (0.02f + 0.05f * i), H * 0.3f, 0f),
                            new Vector3(H * 0.015f, H * (0.42f - 0.05f * i), H * 0.015f), glow);
                    break;

                case VistaKind.Monolith:
                    Block(hero, "Slab", new Vector3(0f, H * 0.5f, 0f),
                        new Vector3(H * 0.28f, H, H * 0.07f), body);
                    for (int i = 0; i < 5; i++) // orbiting broken fragments, frozen mid-air
                    {
                        float a = i * 1.257f;
                        Block(hero, "Frag" + i,
                            new Vector3(Mathf.Cos(a) * H * 0.3f, H * (0.55f + 0.09f * i), Mathf.Sin(a) * H * 0.22f),
                            Vector3.one * H * (0.045f - 0.004f * i), glow);
                    }
                    break;

                case VistaKind.CrystalForest:
                    for (int i = 0; i < 7; i++)
                    {
                        float a = i * 0.897f;
                        float h = H * (0.45f + 0.55f * Hash01(i, 3, kit.seed));
                        var c = Block(hero, "Crystal" + i,
                            new Vector3(Mathf.Cos(a) * H * 0.22f, h * 0.45f, Mathf.Sin(a) * H * 0.22f),
                            new Vector3(h * 0.09f, h, h * 0.09f), i % 2 == 0 ? glow : body);
                        c.transform.localRotation = Quaternion.Euler(
                            (Hash01(i, 5, kit.seed) - 0.5f) * 24f, a * Mathf.Rad2Deg, (Hash01(i, 9, kit.seed) - 0.5f) * 24f);
                    }
                    break;

                case VistaKind.ArchRing:
                    for (int i = 0; i < 3; i++) // receding arches — a road of giants
                    {
                        float s = 1f - 0.22f * i;
                        var arch = new GameObject("Arch" + i).transform;
                        arch.SetParent(hero, false);
                        arch.localPosition = new Vector3(0f, 0f, -H * 0.5f * i);
                        Block(arch, "PillarL", new Vector3(-H * 0.22f * s, H * 0.35f * s, 0f),
                            new Vector3(H * 0.07f * s, H * 0.7f * s, H * 0.07f * s), body);
                        Block(arch, "PillarR", new Vector3(H * 0.22f * s, H * 0.35f * s, 0f),
                            new Vector3(H * 0.07f * s, H * 0.7f * s, H * 0.07f * s), body);
                        Block(arch, "Beam", new Vector3(0f, H * 0.72f * s, 0f),
                            new Vector3(H * 0.5f * s, H * 0.07f * s, H * 0.07f * s), i == 0 ? glow : body);
                    }
                    break;
            }
        }

        private static void BuildRockCluster(Transform parent, CityLayoutDefinition kit, ExperienceDef ex,
            Vector3 center, float scale, int salt)
        {
            var cluster = new GameObject("Midground_" + salt).transform;
            cluster.SetParent(parent, false);
            cluster.localPosition = new Vector3(center.x, kit.walkwayHeight, center.z);
            int n = 3 + (int)(Hash01(salt, 1, kit.seed) * 3f);
            for (int i = 0; i < n; i++)
            {
                float a = Hash01(salt, i * 2 + 2, kit.seed) * Mathf.PI * 2f;
                float r = Hash01(salt, i * 2 + 3, kit.seed) * scale * 0.8f;
                float s = scale * (0.5f + Hash01(salt, i + 40, kit.seed));
                var rock = Block(cluster, "Rock" + i,
                    new Vector3(Mathf.Cos(a) * r, s * 0.32f, Mathf.Sin(a) * r),
                    new Vector3(s, s * 0.75f, s * 0.9f), ex.vistaColor * 0.8f);
                rock.transform.localRotation = Quaternion.Euler(
                    (Hash01(salt, i + 60, kit.seed) - 0.5f) * 30f,
                    Hash01(salt, i + 70, kit.seed) * 360f,
                    (Hash01(salt, i + 80, kit.seed) - 0.5f) * 30f);
            }
        }

        /// <summary>A glowing ring approximated by 10 rotated thin blocks (no torus primitive).</summary>
        private static void Ring(Transform parent, Vector3 center, float radius, float thickness, Color color)
        {
            var ring = new GameObject("Ring").transform;
            ring.SetParent(parent, false);
            ring.localPosition = center;
            const int segs = 10;
            for (int i = 0; i < segs; i++)
            {
                float a = (i / (float)segs) * Mathf.PI * 2f;
                float segLen = 2f * radius * Mathf.Sin(Mathf.PI / segs) * 1.1f;
                var seg = Block(ring, "S" + i,
                    new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f),
                    new Vector3(segLen, thickness, thickness), color);
                seg.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg + 90f);
            }
        }

        private static GameObject Block(Transform parent, string name, Vector3 localPos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = MakeMat("VistaMat_" + ColorUtility.ToHtmlStringRGB(color), color);
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }

        private static readonly Dictionary<string, Material> _mats = new Dictionary<string, Material>();

        private static Material MakeMat(string name, Color c)
        {
            if (_mats.TryGetValue(name, out var cached) && cached != null) return cached;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var m = new Material(shader) { name = name };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", c);
            _mats[name] = m;
            return m;
        }

        // ── Atmosphere: the vista must READ — thin fog until the landmark survives it ─────────────

        private static void TuneAtmosphere(ExperienceDef ex)
        {
            if (!RenderSettings.fog || ex.vista == VistaKind.None) return;
            float maxDensity = 1.0f / Mathf.Max(60f, ex.vistaDistance);
            if (RenderSettings.fogDensity > maxDensity)
                RenderSettings.fogDensity = maxDensity;
        }
    }
}
#endif
