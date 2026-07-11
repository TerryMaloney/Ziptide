#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// P1d + P1e of the World Experience Engine — the passes that make a world READ.
    ///
    /// P1d BREADCRUMB ROUTE: glowing cairns every ~20m along the nearest-neighbor route
    /// spawn → POIs → (StoryAnchor placed by the ring on the vista sightline). Navigation by
    /// landmark, not luck: from any cairn you can see the next one and usually the beacon.
    ///
    /// P1e→H5 DRESSING/SCATTER: biome-keyed prop clusters (rocks, flora, debris, crystals, bones)
    /// placed by the PURE ScatterField (Poisson blue-noise — no clumps, no bald patches) with a
    /// moisture-driven density channel (TerrainField.Climate: vegetation pools where the water is)
    /// and exclusion masks over pads, corridors, POI pockets and the cairn route. Detail is a PASS,
    /// not hand-placement: a mid-level LLM tunes two knobs (density, palette) per world.
    ///
    /// Everything is deterministic from kit.seed, placed at terrain height, marked static for
    /// batching, and collider-free below knee height (nothing here may block CollideMove).
    /// </summary>
    public static class WorldDressingBuilder
    {
        private const float CairnSpacing = 20f;
        private const float RouteClearance = 7f;    // no scatter this close to the route
        private const float PoiClearance = 16f;     // no scatter inside POI pockets

        public static void Build(Transform root, CityLayoutDefinition kit)
        {
            var ex = kit != null ? kit.experience : null;
            if (root == null || ex == null || !ex.enabled) return;

            var existing = root.Find("Dressing");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);
            var dressRoot = new GameObject("Dressing").transform;
            dressRoot.SetParent(root, false);

            var route = BuildRoute(kit);
            BuildCairns(dressRoot, kit, ex, route);
            ScatterBiomeProps(dressRoot, kit, ex, route);
            // FORGE III F3.1b — practical lights. Runs AFTER BuildingBuilder (CityBuilder order:
            // buildings line 48, dressing line 57), so the __DOOR markers the sconce pass reads
            // already exist. Parented to dressRoot → cleared+rebuilt with the rest of the dressing.
            PracticalAuthor.Place(dressRoot, kit, route);
            // FORGE III F3.3 — water fills the shipyard berth (the tidefront read), if the world has one.
            WaterAuthor.Place(dressRoot, kit);
        }

        // ── P1d: the route + cairns ───────────────────────────────────────────────────────────────

        /// <summary>Nearest-neighbor polyline: spawn district → every non-berth POI.</summary>
        private static List<Vector2> BuildRoute(CityLayoutDefinition kit)
        {
            var route = new List<Vector2>();
            Vector3 spawn = kit.districts.Count > 0 ? kit.districts[0].anchor : Vector3.zero;
            foreach (var d in kit.districts)
                if (d != null && d.id == kit.spawnDistrictId) spawn = d.anchor;
            route.Add(new Vector2(spawn.x, spawn.z));

            var remaining = new List<PoiDef>();
            if (kit.pois != null)
                foreach (var p in kit.pois)
                    if (p != null && p.type != PoiType.TravelBerth) remaining.Add(p);

            var at = route[0];
            while (remaining.Count > 0)
            {
                int best = 0;
                float bestDist = float.MaxValue;
                for (int i = 0; i < remaining.Count; i++)
                {
                    var q = new Vector2(remaining[i].position.x, remaining[i].position.z);
                    float dd = Vector2.Distance(at, q);
                    if (dd < bestDist) { bestDist = dd; best = i; }
                }
                at = new Vector2(remaining[best].position.x, remaining[best].position.z);
                route.Add(at);
                remaining.RemoveAt(best);
            }
            return route;
        }

        private static void BuildCairns(Transform parent, CityLayoutDefinition kit, ExperienceDef ex, List<Vector2> route)
        {
            if (route.Count < 2) return;
            var cairnRoot = new GameObject("Route").transform;
            cairnRoot.SetParent(parent, false);

            Color stone = ex.groundColor * 0.75f;
            Color glow = ex.vistaAccentColor;
            int n = 0;
            for (int leg = 0; leg < route.Count - 1; leg++)
            {
                Vector2 a = route[leg], b = route[leg + 1];
                float len = Vector2.Distance(a, b);
                int steps = Mathf.FloorToInt(len / CairnSpacing);
                for (int s = 1; s < steps; s++) // skip endpoints — pads/POIs mark themselves
                {
                    Vector2 p = Vector2.Lerp(a, b, s / (float)steps);
                    // Alternate slightly off the walking line so cairns frame the path.
                    Vector2 side = new Vector2(-(b - a).y, (b - a).x).normalized * (s % 2 == 0 ? 1.8f : -1.8f);
                    p += side;
                    float y = WorldExperienceBuilder.HeightAt(kit, p.x, p.y);

                    var cairn = new GameObject("Cairn_" + n).transform;
                    cairn.SetParent(cairnRoot, false);
                    cairn.position = new Vector3(p.x, y, p.y);
                    Block(cairn, "Base", new Vector3(0f, 0.25f, 0f), new Vector3(0.7f, 0.5f, 0.7f), stone, n);
                    Block(cairn, "Mid", new Vector3(0.05f, 0.65f, 0f), new Vector3(0.5f, 0.35f, 0.5f), stone, n);
                    Block(cairn, "Light", new Vector3(0f, 0.95f, 0f), new Vector3(0.22f, 0.22f, 0.22f), glow, n);
                    n++;
                }
            }
        }

        // ── P1e→H5: biome scatter via the pure ScatterField (Poisson blue-noise + climate density) ─

        private static void ScatterBiomeProps(Transform parent, CityLayoutDefinition kit, ExperienceDef ex, List<Vector2> route)
        {
            var scatterRoot = new GameObject("Scatter").transform;
            scatterRoot.SetParent(parent, false);

            float R = Mathf.Max(60f, ex.worldRadius) * 0.86f; // stay inside the rim climb

            // Exclusion masks: the route, every POI pocket, every pad — gameplay space stays clean.
            var masks = new List<ScatterMask>();
            for (int i = 0; i < route.Count - 1; i++)
                masks.Add(ScatterMask.Capsule(route[i], route[i + 1], RouteClearance));
            if (kit.pois != null)
                foreach (var poi in kit.pois)
                    if (poi != null)
                        masks.Add(ScatterMask.Disc(new Vector2(poi.position.x, poi.position.z), PoiClearance));
            foreach (var d in kit.districts)
                if (d != null)
                    masks.Add(ScatterMask.Disc(new Vector2(d.anchor.x, d.anchor.z),
                        Mathf.Max(d.bounds.x, d.bounds.y) * 0.5f + 10f));
            if (kit.shipyard != null && kit.shipyard.enabled)
                masks.Add(ScatterMask.Disc(new Vector2(kit.shipyard.berthCenter.x, kit.shipyard.berthCenter.z),
                    Mathf.Max(kit.shipyard.berthSize.x, kit.shipyard.berthSize.y) * 0.5f + 10f));

            // Density follows MOISTURE (TerrainField.Climate) — vegetation and texture pool where the
            // water is, thin out on the dry ridges. Kind 0 is the rare landmark prop (lonely by law).
            var points = ScatterField.Generate(R, 20f, 4, kit.seed,
                masks, (x, z) => 0.30f + 0.55f * TerrainField.Climate(x, z, kit.seed).y,
                new float[] { 70f, 0f, 0f, 0f });

            int n = 0;
            foreach (var pt in points)
            {
                float y = WorldExperienceBuilder.HeightAt(kit, pt.Position.x, pt.Position.y);
                var cluster = new GameObject("Prop_" + n).transform;
                cluster.SetParent(scatterRoot, false);
                cluster.position = new Vector3(pt.Position.x, y, pt.Position.y);
                BuildPropCluster(cluster, ex, kit.seed, n, pt.Kind);
                n++;
            }
        }

        /// <summary>One prop cluster: kind 0 = the biome's RARE feature; 1-3 = its common texture.</summary>
        private static void BuildPropCluster(Transform at, ExperienceDef ex, int seed, int salt, int kind)
        {
            Color ground = ex.groundColor;
            Color glow = ex.vistaAccentColor;

            switch (ex.biome)
            {
                case BiomePreset.Dunes:
                    if (kind == 0) Bones(at, new Color(0.75f, 0.72f, 0.65f), seed, salt);
                    else Rocks(at, ground * 0.8f, seed, salt, 2, 0.9f);
                    break;
                case BiomePreset.Mesas:
                    if (kind == 0) Shards(at, glow, seed, salt, 0.9f);
                    else Rocks(at, ground * 0.7f, seed, salt, 3, 1.3f);
                    break;
                case BiomePreset.Canyon:
                    if (kind == 0) Debris(at, new Color(0.35f, 0.30f, 0.28f), seed, salt);
                    else if (kind == 1) Rocks(at, ground * 0.75f, seed, salt, 3, 1.1f);
                    else Tufts(at, new Color(0.35f, 0.55f, 0.30f), seed, salt, ForgeRecipeLibrary.FrondRecipeId);
                    break;
                case BiomePreset.CavernFloor:
                    if (kind == 0) Shards(at, glow, seed, salt, 1.6f); // the big glow crystal
                    else if (kind == 1) Rocks(at, ground * 0.6f, seed, salt, 2, 1.0f);
                    else Shards(at, glow, seed, salt, 0.8f);
                    break;
                case BiomePreset.TideFlats:
                    if (kind == 0) Shards(at, glow, seed, salt, 0.6f);
                    else if (kind == 1) Debris(at, ground * 0.7f, seed, salt);
                    else Tufts(at, new Color(0.30f, 0.55f, 0.50f), seed, salt, ForgeRecipeLibrary.ReedRecipeId);
                    break;
                default:
                    Rocks(at, ground * 0.8f, seed, salt, 2, 0.9f);
                    break;
            }
        }

        private static void Rocks(Transform at, Color c, int seed, int salt, int count, float scale)
        {
            for (int i = 0; i < count; i++)
            {
                float s = scale * (0.5f + Hash01(salt, 40 + i, seed));
                var b = Block(at, "Rock" + i,
                    new Vector3((Hash01(salt, 50 + i, seed) - 0.5f) * 3f, s * 0.3f, (Hash01(salt, 60 + i, seed) - 0.5f) * 3f),
                    new Vector3(s, s * 0.7f, s * 0.85f), c, salt + i);
                b.transform.localRotation = Quaternion.Euler(
                    (Hash01(salt, 70 + i, seed) - 0.5f) * 30f, Hash01(salt, 80 + i, seed) * 360f, 0f);
            }
        }

        private static void Tufts(Transform at, Color c, int seed, int salt, string plantRecipeId)
        {
            for (int i = 0; i < 4; i++)
            {
                float h = 0.4f + Hash01(salt, 90 + i, seed) * 0.7f;
                // E5.3 FLORA (art-lane coordination, HANDOFF dddd19): each tuft rides an UNSCALED
                // holder that carries ForgeModuleLook (swaps in the baked LeafCard plant on device;
                // the primitive block stays the editor/fallback look) + ForgeSway (wind). The block
                // must not be static: the holder sways, and a batched child would render stale.
                var holder = new GameObject("TuftPlant" + i);
                holder.transform.SetParent(at, false);
                holder.transform.localPosition = new Vector3(
                    (Hash01(salt, 100 + i, seed) - 0.5f) * 2.4f, 0f, (Hash01(salt, 110 + i, seed) - 0.5f) * 2.4f);
                holder.transform.localRotation = Quaternion.Euler(0f, Hash01(salt, 115 + i, seed) * 360f, 0f);
                var block = Block(holder.transform, "Tuft" + i, new Vector3(0f, h * 0.5f, 0f),
                    new Vector3(0.1f, h, 0.1f), c, salt + i);
                block.isStatic = false;
                var look = holder.AddComponent<Ziptide.Visuals.ForgeModuleLook>();
                look.recipeId = plantRecipeId;
                look.keepChildren = new string[0];
                holder.AddComponent<Ziptide.Visuals.ForgeSway>();
            }
        }

        private static void Shards(Transform at, Color c, int seed, int salt, float scale)
        {
            int n = 1 + (int)(Hash01(salt, 120, seed) * 3f);
            for (int i = 0; i < n; i++)
            {
                float h = scale * (0.6f + Hash01(salt, 130 + i, seed) * 1.2f);
                var b = Block(at, "Shard" + i,
                    new Vector3((Hash01(salt, 140 + i, seed) - 0.5f) * 1.6f, h * 0.45f, (Hash01(salt, 150 + i, seed) - 0.5f) * 1.6f),
                    new Vector3(h * 0.18f, h, h * 0.18f), c, salt + i);
                b.transform.localRotation = Quaternion.Euler(
                    (Hash01(salt, 160 + i, seed) - 0.5f) * 28f, Hash01(salt, 170 + i, seed) * 360f,
                    (Hash01(salt, 180 + i, seed) - 0.5f) * 28f);
            }
        }

        private static void Debris(Transform at, Color c, int seed, int salt)
        {
            for (int i = 0; i < 3; i++)
            {
                Block(at, "Debris" + i,
                    new Vector3((Hash01(salt, 190 + i, seed) - 0.5f) * 2.8f, 0.12f, (Hash01(salt, 200 + i, seed) - 0.5f) * 2.8f),
                    new Vector3(0.6f + Hash01(salt, 210 + i, seed) * 0.6f, 0.24f, 0.4f), c, salt + i)
                    .transform.localRotation = Quaternion.Euler(0f, Hash01(salt, 220 + i, seed) * 360f, 0f);
            }
        }

        private static void Bones(Transform at, Color c, int seed, int salt)
        {
            // A rib arc — something big died here long ago.
            int ribs = 3 + (int)(Hash01(salt, 230, seed) * 2f);
            for (int i = 0; i < ribs; i++)
            {
                float h = 1.2f + Hash01(salt, 240 + i, seed) * 1.4f;
                var b = Block(at, "Rib" + i, new Vector3(i * 0.9f - ribs * 0.45f, h * 0.4f, 0f),
                    new Vector3(0.14f, h, 0.14f), c, salt + i);
                b.transform.localRotation = Quaternion.Euler(0f, 0f, (i % 2 == 0 ? 1f : -1f) * (18f + i * 4f));
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────────────────────

        private static float Hash01(int x, int z, int seed)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + z * 668265263 + seed * 1274126177);
                h = (h ^ (h >> 13)) * 1274126177u;
                return ((h ^ (h >> 16)) & 0xFFFFFF) / (float)0x1000000;
            }
        }

        private static readonly Dictionary<Color, Material> _mats = new Dictionary<Color, Material>();

        private static GameObject Block(Transform parent, string name, Vector3 localPos, Vector3 scale, Color color, int salt)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.isStatic = true; // static batching — hundreds of props must not mean hundreds of draws
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col); // dressing never blocks movement or bullets
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                if (!_mats.TryGetValue(color, out var m) || m == null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader == null) shader = Shader.Find("Standard");
                    m = new Material(shader) { name = "DressMat_" + ColorUtility.ToHtmlStringRGB(color) };
                    if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
                    else if (m.HasProperty("_Color")) m.SetColor("_Color", color);
                    _mats[color] = m;
                }
                r.sharedMaterial = m;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }
    }
}
#endif
