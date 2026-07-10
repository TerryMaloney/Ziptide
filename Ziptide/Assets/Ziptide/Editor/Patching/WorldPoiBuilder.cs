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
    /// Hardwiring 1.5 verbs (catalog grows 7→12; streets get civic life):
    ///  Market       — stall rows + awnings + crate stacks (future vendor/NPC pocket)
    ///  Shrine       — kneel ring + monolith + offering glow (quiet story flavor, scannable later)
    ///  RepairBay    — gantry arch + tool bench + crane (the tool-chest repair fantasy's street home)
    ///  Transit      — platform + route sign (the future vehicle/transit hookup made visible)
    ///  Lookout      — raised watch deck + rails (a vantage verb — climb up, read the world)
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
                    case PoiType.Market: BuildMarket(poiRoot, kit, poi); break;
                    case PoiType.Shrine: BuildShrine(poiRoot, kit, poi); break;
                    case PoiType.RepairBay: BuildRepairBay(poiRoot, kit, poi); break;
                    case PoiType.Transit: BuildTransit(poiRoot, kit, poi); break;
                    case PoiType.Lookout: BuildLookout(poiRoot, kit, poi); break;
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

        // ── Hardwiring 1.5 pockets (catalog verbs 8–12) ─────────────────────────────────────────────

        private static void BuildMarket(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var pal = kit.palette;
            var awning = new Color(0.75f, 0.45f, 0.25f);
            int stalls = 3 + Mathf.Clamp(poi.tier, 0, 2);
            for (int i = 0; i < stalls; i++)
            {
                // Two facing rows with a walk lane between — a street you shop down.
                float x = (i % 2 == 0 ? -1f : 1f) * 3.2f;
                float z = (i / 2 - 1) * 3.4f;
                var stall = new GameObject("Stall" + i).transform;
                stall.SetParent(root, false);
                stall.localPosition = new Vector3(x, 0f, z);
                stall.localRotation = Quaternion.Euler(0f, x < 0f ? 90f : -90f, 0f);
                Block(stall, "Counter", new Vector3(0f, 0.55f, 0f), new Vector3(2.4f, 1.1f, 0.9f), pal.concrete, true);
                Block(stall, "PostL", new Vector3(-1.05f, 1.5f, -0.35f), new Vector3(0.12f, 3f, 0.12f), pal.metal, true);
                Block(stall, "PostR", new Vector3(1.05f, 1.5f, -0.35f), new Vector3(0.12f, 3f, 0.12f), pal.metal, true);
                Block(stall, "Awning", new Vector3(0f, 2.9f, 0.2f), new Vector3(2.8f, 0.12f, 1.8f), awning, false)
                    .transform.localRotation = Quaternion.Euler(-12f, 0f, 0f);
                Block(stall, "Wares", new Vector3(0f, 1.25f, 0f), new Vector3(1.6f, 0.3f, 0.5f), pal.accent, false);
            }
            Block(root, "CrateStack", new Vector3(0f, 0.5f, 6f), new Vector3(1f, 1f, 1f), pal.metal, true);
            Block(root, "CrateTop", new Vector3(0.2f, 1.3f, 5.8f), new Vector3(0.7f, 0.7f, 0.7f), pal.metal, true);
        }

        private static void BuildShrine(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var stone = kit.experience != null ? kit.experience.vistaColor : kit.palette.concrete;
            var glow = kit.experience != null ? kit.experience.vistaAccentColor : kit.palette.accent;
            Block(root, "KneelRing", new Vector3(0f, 0.15f, 0f), new Vector3(6.5f, 0.3f, 6.5f), stone, true);
            Block(root, "Monolith", new Vector3(0f, 2.6f, 0f), new Vector3(1.1f, 4.6f, 0.7f), stone, true)
                .transform.localRotation = Quaternion.Euler(0f, 25f, 2.5f); // aged lean
            Block(root, "OfferingBowl", new Vector3(0f, 0.5f, 2.1f), new Vector3(0.8f, 0.4f, 0.8f), stone, true);
            Block(root, "OfferingGlow", new Vector3(0f, 0.75f, 2.1f), new Vector3(0.5f, 0.12f, 0.5f), glow, false);
            for (int i = 0; i < 4; i++) // low candle posts around the ring
            {
                float a = i * 1.571f + 0.785f;
                Block(root, "Candle" + i, new Vector3(Mathf.Cos(a) * 3f, 0.55f, Mathf.Sin(a) * 3f),
                    new Vector3(0.18f, 1.1f, 0.18f), stone, true);
                Block(root, "CandleGlow" + i, new Vector3(Mathf.Cos(a) * 3f, 1.2f, Mathf.Sin(a) * 3f),
                    new Vector3(0.24f, 0.24f, 0.24f), glow, false);
            }
        }

        private static void BuildRepairBay(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var pal = kit.palette;
            // Open gantry arch — drive/walk the broken thing in under it.
            Block(root, "GantryL", new Vector3(-3f, 2f, 0f), new Vector3(0.5f, 4f, 0.5f), pal.metal, true);
            Block(root, "GantryR", new Vector3(3f, 2f, 0f), new Vector3(0.5f, 4f, 0.5f), pal.metal, true);
            Block(root, "GantryBeam", new Vector3(0f, 4.1f, 0f), new Vector3(6.5f, 0.4f, 0.6f), pal.metal, true);
            Block(root, "HoistArm", new Vector3(1f, 3.6f, 0f), new Vector3(0.25f, 1.2f, 0.25f), pal.rail, true);
            Block(root, "HoistHook", new Vector3(1f, 2.9f, 0f), new Vector3(0.4f, 0.25f, 0.4f), pal.accent, false);
            // Tool bench — the tool-chest/righty-tighty fantasy's street-side home.
            Block(root, "Bench", new Vector3(-1.8f, 0.5f, 3.2f), new Vector3(2.6f, 1f, 1f), pal.concrete, true);
            Block(root, "BenchTools", new Vector3(-1.8f, 1.1f, 3.2f), new Vector3(2.2f, 0.18f, 0.7f), pal.accent, false);
            Block(root, "PartsBin", new Vector3(2.4f, 0.45f, 3f), new Vector3(1.2f, 0.9f, 1.2f), pal.metal, true);
        }

        private static void BuildTransit(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var pal = kit.palette;
            Block(root, "Platform", new Vector3(0f, 0.2f, 0f), new Vector3(7f, 0.4f, 3.5f), pal.concrete, true);
            Block(root, "Bench", new Vector3(-1.8f, 0.75f, -1f), new Vector3(2.2f, 0.5f, 0.7f), pal.metal, true);
            Block(root, "SignPost", new Vector3(2.6f, 1.8f, -1.2f), new Vector3(0.15f, 3.2f, 0.15f), pal.metal, true);
            Block(root, "SignBoard", new Vector3(2.6f, 3.1f, -1.2f), new Vector3(1.6f, 0.9f, 0.12f), pal.accent, false);
            for (int i = 0; i < 3; i++) // route posts marching off — the line continues somewhere
                Block(root, "RoutePost" + i, new Vector3(4.5f + i * 2.2f, 0.7f, 0.8f),
                    new Vector3(0.2f, 1.4f, 0.2f), pal.rail, true);

            // 3.2: the Transit stop parks the biome's signature RIDE (VehicleRuntime resolves the
            // definition by id at runtime — no hard refs). The verb finally has its vehicle.
            var ride = new GameObject("Vehicle_" + poi.id);
            ride.transform.SetParent(root, false);
            ride.transform.localPosition = new Vector3(0f, 0.45f, 2.6f);
            var vr = ride.AddComponent<Ziptide.Ship.VehicleRuntime>();
            var so = new SerializedObject(vr);
            PatcherUtil.SetString(so, "vehicleId", RideForBiome(kit));
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static string RideForBiome(CityLayoutDefinition kit)
        {
            var biome = kit.experience != null ? kit.experience.biome : BiomePreset.TideFlats;
            switch (biome)
            {
                case BiomePreset.Dunes:
                case BiomePreset.Mesas: return "dune_hoverbike";
                case BiomePreset.CavernFloor: return "cavern_crawler";
                default: return "tide_skiff";
            }
        }

        private static void BuildLookout(Transform root, CityLayoutDefinition kit, PoiDef poi)
        {
            var pal = kit.palette;
            var glow = kit.experience != null ? kit.experience.vistaAccentColor : kit.palette.accent;
            for (int i = 0; i < 4; i++) // deck legs
            {
                float x = (i % 2 == 0 ? -1f : 1f) * 1.6f;
                float z = (i / 2 == 0 ? -1f : 1f) * 1.6f;
                Block(root, "Leg" + i, new Vector3(x, 1.75f, z), new Vector3(0.35f, 3.5f, 0.35f), pal.metal, true);
            }
            Block(root, "Deck", new Vector3(0f, 3.6f, 0f), new Vector3(4.4f, 0.3f, 4.4f), pal.concrete, true);
            for (int i = 0; i < 4; i++) // rails — a vantage you can lean on, not fall off
            {
                float a = i * 1.571f;
                Block(root, "Rail" + i, new Vector3(Mathf.Cos(a) * 2.1f, 4.35f, Mathf.Sin(a) * 2.1f),
                    new Vector3(i % 2 == 0 ? 0.15f : 4.4f, 1.1f, i % 2 == 0 ? 4.4f : 0.15f), pal.rail, true);
            }
            // Access ramp — walkable up (climb studs can join later via the traversal lane's kit).
            Block(root, "Ramp", new Vector3(0f, 1.7f, 4.6f), new Vector3(1.8f, 0.25f, 5.6f), pal.concrete, true)
                .transform.localRotation = Quaternion.Euler(-38f, 0f, 0f);
            Block(root, "SpotBeacon", new Vector3(0f, 5.6f, 0f), new Vector3(0.35f, 2.2f, 0.35f), glow, false);
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
