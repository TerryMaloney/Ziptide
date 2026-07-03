#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// P1c of the World Experience Engine — POINTS OF INTEREST, where the gameplay lives. Each
    /// <see cref="PoiDef"/> on the layout becomes a staged 15–30m "gameplay pocket" with a VERB, built
    /// on a level pad the terrain grades itself to (WorldExperienceBuilder adds a flatten site per
    /// POI). Every POI also plants a spawn marker "poi_&lt;id&gt;" so contracts (WorldJobLibrary
    /// GoToMarker steps) and the dev warp can route THROUGH the POI network instead of raw coordinates.
    ///
    /// Verbs and their pockets:
    ///  CombatCamp   — cover ring + watch mast + live creatures (world fauna; count scales by tier)
    ///  HarvestGrove — planter rows + flora (GardenPlotRuntime lands here in P3)
    ///  MachineSite  — machine housing + pipes + socket plinth (BuildSocket placement lands in P3)
    ///  RuinCache    — broken walls + a glowing cache pedestal
    ///  CaveSecret   — leaning rock shell with one entrance + an inner glow (exploration reward)
    ///  StoryAnchor  — a dais + pylons + light beacon: the world's WORLD_DATA beat gets a STAGE
    ///  TravelBerth  — marker only (the shipyard/travel station is already the pocket)
    /// Deterministic, data-only input; a mid-level LLM authors worlds by filling the POI table
    /// (docs/WORLD_RECIPE.md) — the audit quality gates reject sets that are too small or same-y.
    /// </summary>
    public static class WorldPoiBuilder
    {
        public static void Build(Transform root, CityLayoutDefinition kit)
        {
            if (root == null || kit == null || kit.pois == null || kit.pois.Count == 0) return;
            var ex = kit.experience;
            if (ex == null || !ex.enabled) return;

            var existing = root.Find("Pois");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);
            var poisRoot = new GameObject("Pois").transform;
            poisRoot.SetParent(root, false);

            string fauna = kit.creatureZones != null && kit.creatureZones.Count > 0
                ? kit.creatureZones[0].creatureId : "swarm_bug";

            // Spawn exclusion: a POI's own props (watch masts, dais steps, pedestals) reach ~8m from
            // its center — any POI closer than that to the player spawn gets pushed straight out to
            // the exclusion ring (SPAWN_OVERLAP_SOLID on 5 worlds, diag run 28682909567).
            var spawnDistrict = kit.districts.Count > 0 ? kit.districts[0] : null;
            foreach (var d in kit.districts)
                if (d != null && d.id == kit.spawnDistrictId) spawnDistrict = d;
            Vector2 spawnXZ = spawnDistrict != null
                ? new Vector2(spawnDistrict.anchor.x, spawnDistrict.anchor.z)
                : Vector2.zero;

            foreach (var poi in kit.pois)
            {
                if (poi == null || string.IsNullOrEmpty(poi.id)) continue;
                Vector2 xz = ExcludeFromSpawn(new Vector2(poi.position.x, poi.position.z), spawnXZ);
                float y = WorldExperienceBuilder.HeightAt(kit, xz.x, xz.y);
                var pos = new Vector3(xz.x, y, xz.y);

                var poiRoot = new GameObject("__POI_" + poi.id).transform;
                poiRoot.SetParent(poisRoot, false);
                poiRoot.position = pos;

                PlantMarker(poiRoot, poi.id);

                switch (poi.type)
                {
                    case PoiType.CombatCamp: BuildCombatCamp(poiRoot, kit, poi, fauna); break;
                    case PoiType.HarvestGrove: BuildHarvestGrove(poiRoot, kit, poi); break;
                    case PoiType.MachineSite: BuildMachineSite(poiRoot, kit, poi); break;
                    case PoiType.RuinCache: BuildRuinCache(poiRoot, kit, poi); break;
                    case PoiType.CaveSecret: BuildCaveSecret(poiRoot, kit, poi); break;
                    case PoiType.StoryAnchor: BuildStoryAnchor(poiRoot, kit, poi); break;
                        // TravelBerth: marker only — the berth/travel station is already built.
                }
            }
            Debug.Log("[Ziptide] POIs built for '" + kit.cityId + "': " + kit.pois.Count +
                      " (verbs=" + PoiQuality.DistinctVerbCount(kit.pois) + ")");
        }

        // Contracts target POIs by marker id "poi_<id>" (pack spawnMarkers sync in WorldStubGenerator).
        private static void PlantMarker(Transform poiRoot, string poiId)
        {
            var go = new GameObject("__SPAWN_poi_" + poiId);
            go.transform.SetParent(poiRoot, false);
            go.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            var marker = go.AddComponent<SpawnMarkerRuntime>();
            var so = new SerializedObject(marker);
            PatcherUtil.SetString(so, "markerId", "poi_" + poiId);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ── The pockets ───────────────────────────────────────────────────────────────────────────

        private static void BuildCombatCamp(Transform root, CityLayoutDefinition kit, PoiDef poi, string fauna)
        {
            var pal = kit.palette;
            int covers = 5;
            for (int i = 0; i < covers; i++)
            {
                float a = (i / (float)covers) * Mathf.PI * 2f + poi.tier;
                float r = 6f + (i % 2) * 2.5f;
                Block(root, "Cover" + i, new Vector3(Mathf.Cos(a) * r, 0.7f, Mathf.Sin(a) * r),
                    new Vector3(2.1f, 1.4f, 1.1f), pal.metal, true)
                    .transform.localRotation = Quaternion.Euler(0f, a * Mathf.Rad2Deg, 0f);
            }
            Block(root, "WatchMast", new Vector3(0f, 3f, 0f), new Vector3(0.35f, 6f, 0.35f), pal.metal, true);
            Block(root, "MastBeacon", new Vector3(0f, 6.2f, 0f), new Vector3(0.6f, 0.6f, 0.6f),
                new Color(0.95f, 0.35f, 0.25f), false);

            int count = 2 + Mathf.Clamp(poi.tier, 0, 2);
            for (int i = 0; i < count; i++)
            {
                float a = (i / (float)count) * Mathf.PI * 2f;
                var pos = root.position + new Vector3(Mathf.Cos(a) * 7f, 0.5f, Mathf.Sin(a) * 7f);
                CityBuilder.MakeCreature(root, "Camp_" + poi.id + "_" + i, pos, fauna, 45f);
            }
        }

        private static void BuildHarvestGrove(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var soil = new Color(0.30f, 0.22f, 0.16f);
            var leaf = new Color(0.35f, 0.75f, 0.35f);
            for (int i = 0; i < 6; i++)
            {
                float x = (i % 3 - 1) * 3.2f;
                float z = (i / 3 == 0 ? -1f : 1f) * 2.4f;
                // Planter box + soil bed — the P3 garden runtime attaches to these pads.
                Block(root, "Planter" + i, new Vector3(x, 0.3f, z), new Vector3(2.6f, 0.6f, 1.6f), kit.palette.concrete, true);
                Block(root, "Soil" + i, new Vector3(x, 0.62f, z), new Vector3(2.3f, 0.08f, 1.3f), soil, false);
            }
            for (int i = 0; i < 4; i++) // wild flora frames the grove
            {
                float a = i * 1.62f;
                float h = 2.5f + (i % 2) * 1.5f;
                Block(root, "Stalk" + i, new Vector3(Mathf.Cos(a) * 7f, h * 0.5f, Mathf.Sin(a) * 7f),
                    new Vector3(0.25f, h, 0.25f), soil, true);
                Block(root, "Frond" + i, new Vector3(Mathf.Cos(a) * 7f, h + 0.3f, Mathf.Sin(a) * 7f),
                    new Vector3(1.1f, 0.5f, 1.1f), leaf, false);
            }
        }

        private static void BuildMachineSite(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var pal = kit.palette;
            Block(root, "Housing", new Vector3(0f, 1.3f, 0f), new Vector3(3.2f, 2.6f, 2.2f), pal.metal, true);
            Block(root, "Stack", new Vector3(0.9f, 3.6f, 0.4f), new Vector3(0.5f, 2.2f, 0.5f), pal.metal, true);
            for (int i = 0; i < 3; i++)
                Block(root, "Pipe" + i, new Vector3(-2.4f - i * 0.5f, 0.4f, -0.6f + i * 0.6f),
                    new Vector3(2.2f, 0.3f, 0.3f), pal.rail, true);
            // The socket plinth — P3's BuildSocket grid bolts onto this.
            Block(root, "SocketPlinth", new Vector3(0f, 0.45f, 2.6f), new Vector3(1.6f, 0.9f, 1.2f), pal.concrete, true);
            Block(root, "PlinthGlow", new Vector3(0f, 0.95f, 2.6f), new Vector3(1.2f, 0.08f, 0.8f), pal.accent, false);
        }

        private static void BuildRuinCache(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var stone = kit.experience != null ? kit.experience.vistaColor : kit.palette.concrete;
            var glow = kit.experience != null ? kit.experience.vistaAccentColor : kit.palette.accent;
            for (int i = 0; i < 4; i++) // broken wall corners
            {
                float a = i * 1.571f + 0.4f;
                float h = 1.6f + (i % 2) * 1.4f;
                Block(root, "Wall" + i, new Vector3(Mathf.Cos(a) * 5.5f, h * 0.5f, Mathf.Sin(a) * 5.5f),
                    new Vector3(3.2f, h, 0.5f), stone, true)
                    .transform.localRotation = Quaternion.Euler(0f, a * Mathf.Rad2Deg + 90f, (i % 2) * 6f - 3f);
            }
            Block(root, "Pedestal", new Vector3(0f, 0.5f, 0f), new Vector3(1.1f, 1f, 1.1f), stone, true);
            Block(root, "Cache", new Vector3(0f, 1.3f, 0f), new Vector3(0.5f, 0.5f, 0.5f), glow, false);
        }

        private static void BuildCaveSecret(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var rock = kit.experience != null ? kit.experience.groundColor * 0.7f : kit.palette.concrete;
            var glow = kit.experience != null ? kit.experience.vistaAccentColor : kit.palette.accent;
            // Five big slabs leaning to a peak, one gap left open as the way in.
            for (int i = 0; i < 5; i++)
            {
                float a = (i + 1.5f) / 6f * Mathf.PI * 2f; // gap at a=0 — the entrance
                var slab = Block(root, "Shell" + i, new Vector3(Mathf.Cos(a) * 3.4f, 2.4f, Mathf.Sin(a) * 3.4f),
                    new Vector3(4.4f, 5.6f, 0.9f), rock, true);
                slab.transform.localRotation =
                    Quaternion.Euler(0f, -a * Mathf.Rad2Deg + 90f, 0f) * Quaternion.Euler(24f, 0f, 0f);
            }
            Block(root, "InnerGlow", new Vector3(0f, 0.8f, 0f), new Vector3(0.7f, 1.6f, 0.7f), glow, false);
        }

        private static void BuildStoryAnchor(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var stone = kit.experience != null ? kit.experience.vistaColor : kit.palette.concrete;
            var glow = kit.experience != null ? kit.experience.vistaAccentColor : kit.palette.accent;
            Block(root, "Dais", new Vector3(0f, 0.25f, 0f), new Vector3(9f, 0.5f, 9f), stone, true);
            Block(root, "DaisStep", new Vector3(0f, 0.6f, 0f), new Vector3(6f, 0.3f, 6f), stone, true);
            for (int i = 0; i < 3; i++)
            {
                float a = i * 2.094f;
                Block(root, "Pylon" + i, new Vector3(Mathf.Cos(a) * 3.6f, 2.6f, Mathf.Sin(a) * 3.6f),
                    new Vector3(0.7f, 4.4f, 0.7f), stone, true);
                Block(root, "PylonGlow" + i, new Vector3(Mathf.Cos(a) * 3.6f, 4.9f, Mathf.Sin(a) * 3.6f),
                    new Vector3(0.5f, 0.5f, 0.5f), glow, false);
            }
            // The beacon — a column of light marking THE story beat from across the world.
            Block(root, "Beacon", new Vector3(0f, 14f, 0f), new Vector3(0.5f, 26f, 0.5f), glow, false);
        }

        // ── Helper ────────────────────────────────────────────────────────────────────────────────

        /// <summary>POI props reach ~8m from the POI center — keep every POI at least this far
        /// from the player spawn so no prop can overlap it (audit SPAWN_OVERLAP_SOLID).</summary>
        public const float SpawnExclusionRadius = 12f;

        /// <summary>Pure: push a POI straight out to the exclusion ring if it sits too close to
        /// spawn; POIs already outside are untouched. Degenerate (POI == spawn) pushes +X.</summary>
        public static Vector2 ExcludeFromSpawn(Vector2 poiXZ, Vector2 spawnXZ)
        {
            Vector2 delta = poiXZ - spawnXZ;
            float dist = delta.magnitude;
            if (dist >= SpawnExclusionRadius) return poiXZ;
            Vector2 dir = dist > 0.01f ? delta / dist : Vector2.right;
            return spawnXZ + dir * SpawnExclusionRadius;
        }

        private static GameObject Block(Transform parent, string name, Vector3 localPos, Vector3 scale,
            Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            if (!collider)
            {
                var col = go.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = Mat(color);
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }

        private static readonly System.Collections.Generic.Dictionary<Color, Material> _mats
            = new System.Collections.Generic.Dictionary<Color, Material>();

        private static Material Mat(Color color)
        {
            if (_mats.TryGetValue(color, out var cached) && cached != null) return cached;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var m = new Material(shader) { name = "PoiMat_" + ColorUtility.ToHtmlStringRGB(color) };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", color);
            _mats[color] = m;
            return m;
        }
    }
}
#endif
