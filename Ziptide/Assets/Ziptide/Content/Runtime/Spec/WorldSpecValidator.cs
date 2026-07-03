using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Known-id sets the validator checks references against. Null set = that check is skipped
    /// (permissive), so the validator stays pure: the editor compiler fills these from the real
    /// registries (Resources folders, sky library); tests fill them from literals.
    /// </summary>
    public class WorldSpecRegistry
    {
        public HashSet<string> CreatureIds;
        public HashSet<string> ItemIds;
        public HashSet<string> PlantIds;
        public HashSet<string> SkyVistaIds;

        /// <summary>All checks that need registries are skipped — structural rules still run.</summary>
        public static WorldSpecRegistry Permissive() => new WorldSpecRegistry();
    }

    /// <summary>
    /// PURE spec validation (ARCHITECTURE V2 Q1) — actionable errors, never silent. Every message
    /// starts with a stable CODE token so tests and tooling can assert on failures mechanically.
    /// Mirrors (and extends) CityLayoutDefinition.Validate's structural rules at the SPEC level, plus
    /// the predicate rules the quality gates enforce later — catching them at author time instead of
    /// build time is the whole point.
    /// </summary>
    public static class WorldSpecValidator
    {
        public const float MinPoiSpacing = 25f;
        public const int MinPois = 5;
        public const int MinDistinctVerbs = 3;

        public static List<string> Validate(WorldSpec spec, WorldSpecRegistry registry)
        {
            var issues = new List<string>();
            if (spec == null) { issues.Add("SPEC_NULL: no spec."); return issues; }
            registry = registry ?? WorldSpecRegistry.Permissive();

            // ── Identity ────────────────────────────────────────────────────
            if (string.IsNullOrEmpty(spec.sceneName))
                issues.Add("SPEC_SCENE_MISSING: sceneName is empty (e.g. \"W002_DryCistern\").");
            if (string.IsNullOrEmpty(spec.cityId))
                issues.Add("SPEC_CITYID_MISSING: cityId is empty (drives root/pack ids).");
            if (spec.seed == 0)
                issues.Add("SPEC_SEED_ZERO: seed must be nonzero — determinism law (pick any int).");
            if (float.IsNaN(spec.walkwayHeight) || float.IsInfinity(spec.walkwayHeight))
                issues.Add("SPEC_WALKWAY_INVALID: walkwayHeight is not finite.");

            // ── Experience ──────────────────────────────────────────────────
            var ex = spec.experience;
            if (ex != null && ex.enabled)
            {
                if (ex.worldRadius < 60f)
                    issues.Add("SPEC_EXPERIENCE_RADIUS: worldRadius " + ex.worldRadius + " < 60 (quality gate fails tiny worlds; target 250-400).");
                if (ex.heightAmplitude < 0f)
                    issues.Add("SPEC_EXPERIENCE_AMPLITUDE: heightAmplitude is negative.");
                if (ex.vista != VistaKind.None && new Vector2(ex.vistaDirection.x, ex.vistaDirection.z).sqrMagnitude < 0.001f)
                    issues.Add("SPEC_VISTA_DIRECTION: vista " + ex.vista + " has no XZ direction to face.");
            }

            // ── POIs (the gates catch these at build; the spec catches them at author time) ──
            var poiIds = new HashSet<string>();
            var verbs = new HashSet<PoiType>();
            bool hasStoryAnchor = false;
            for (int i = 0; i < spec.pois.Count; i++)
            {
                var p = spec.pois[i];
                if (p == null || string.IsNullOrEmpty(p.id)) { issues.Add("SPEC_POI_ID: POI #" + i + " has no id."); continue; }
                if (!poiIds.Add(p.id)) issues.Add("SPEC_POI_DUPLICATE: POI id '" + p.id + "' repeats.");
                verbs.Add(p.type);
                if (p.type == PoiType.StoryAnchor) hasStoryAnchor = true;
                if (ex != null && ex.enabled)
                {
                    float r = new Vector2(p.position.x, p.position.z).magnitude;
                    if (r > ex.worldRadius)
                        issues.Add("SPEC_POI_OUT_OF_BOUNDS: POI '" + p.id + "' at " + r.ToString("F0") + "m > worldRadius " + ex.worldRadius + ".");
                }
                for (int j = i + 1; j < spec.pois.Count; j++)
                {
                    var q = spec.pois[j];
                    if (q == null) continue;
                    float dx = p.position.x - q.position.x, dz = p.position.z - q.position.z;
                    if (dx * dx + dz * dz < MinPoiSpacing * MinPoiSpacing)
                        issues.Add("SPEC_POI_TOO_CLOSE: '" + p.id + "' and '" + q.id + "' are < " + MinPoiSpacing + "m apart.");
                }
            }
            if (ex != null && ex.enabled)
            {
                if (spec.pois.Count < MinPois)
                    issues.Add("SPEC_POI_COUNT_LOW: " + spec.pois.Count + " POIs < " + MinPois + " (the POI_COUNT_LOW gate will fail the build).");
                if (verbs.Count < MinDistinctVerbs && spec.pois.Count > 0)
                    issues.Add("SPEC_POI_VERBS_LOW: " + verbs.Count + " distinct verbs < " + MinDistinctVerbs + ".");
                if (spec.flagsGranted.Count > 0 && !hasStoryAnchor)
                    issues.Add("SPEC_STORY_ANCHOR_MISSING: world grants flags but has no StoryAnchor POI (the beat needs a place).");
            }

            // ── Layout ──────────────────────────────────────────────────────
            var districtIds = new HashSet<string>();
            foreach (var d in spec.districts)
            {
                if (d == null || string.IsNullOrEmpty(d.id)) { issues.Add("SPEC_DISTRICT_ID: district with no id."); continue; }
                if (!districtIds.Add(d.id)) issues.Add("SPEC_DISTRICT_DUPLICATE: district id '" + d.id + "' repeats.");
            }
            foreach (var c in spec.connections)
            {
                if (c == null) continue;
                if (!districtIds.Contains(c.fromDistrictId))
                    issues.Add("SPEC_CONNECTION_UNKNOWN: connection from unknown district '" + c.fromDistrictId + "'.");
                if (!districtIds.Contains(c.toDistrictId))
                    issues.Add("SPEC_CONNECTION_UNKNOWN: connection to unknown district '" + c.toDistrictId + "'.");
            }
            if (!string.IsNullOrEmpty(spec.spawnDistrictId) && districtIds.Count > 0 && !districtIds.Contains(spec.spawnDistrictId))
                issues.Add("SPEC_SPAWN_DISTRICT_UNKNOWN: spawnDistrictId '" + spec.spawnDistrictId + "' is not a district.");

            // ── Registry references ─────────────────────────────────────────
            if (registry.CreatureIds != null)
                foreach (var z in spec.creatureZones)
                    if (z != null && !registry.CreatureIds.Contains(z.creatureId))
                        issues.Add("SPEC_CREATURE_UNKNOWN: '" + z.creatureId + "' not in Resources/Enemies. Known: " + Join(registry.CreatureIds));
            if (registry.PlantIds != null)
                foreach (var g in spec.gardens)
                    if (g != null && !registry.PlantIds.Contains(g.plantId))
                        issues.Add("SPEC_PLANT_UNKNOWN: '" + g.plantId + "' not in Resources/Garden. Known: " + Join(registry.PlantIds));
            if (registry.ItemIds != null)
            {
                foreach (var c in spec.collectibles)
                    if (c != null && !registry.ItemIds.Contains(c.itemId))
                        issues.Add("SPEC_ITEM_UNKNOWN: collectible '" + c.itemId + "' not in the item registry.");
                foreach (var m in spec.machines)
                    if (m != null && !registry.ItemIds.Contains(m.partItemId))
                        issues.Add("SPEC_ITEM_UNKNOWN: machine part '" + m.partItemId + "' not in the item registry.");
            }
            if (registry.SkyVistaIds != null && !string.IsNullOrEmpty(spec.skyVistaId)
                && !registry.SkyVistaIds.Contains(spec.skyVistaId))
                issues.Add("SPEC_SKY_UNKNOWN: skyVistaId '" + spec.skyVistaId + "' not in the sky library.");

            // ── Economy sanity ──────────────────────────────────────────────
            var packIds = new HashSet<string>();
            foreach (var m in spec.mines)
            {
                if (m == null) continue;
                if (!packIds.Add("mine:" + m.id)) issues.Add("SPEC_PACK_ID_DUPLICATE: mine id '" + m.id + "' repeats.");
                if (m.ratePerSecond <= 0 || m.storageCap <= 0)
                    issues.Add("SPEC_MINE_RATE: mine '" + m.id + "' needs ratePerSecond > 0 and storageCap > 0.");
            }
            foreach (var s in spec.sockets)
            {
                if (s == null) continue;
                if (!packIds.Add("socket:" + s.id)) issues.Add("SPEC_PACK_ID_DUPLICATE: socket id '" + s.id + "' repeats.");
                if (s.buildCost <= 0)
                    issues.Add("SPEC_SOCKET_COST: socket '" + s.id + "' needs buildCost > 0.");
            }
            foreach (var g in spec.gardens)
                if (g != null && !packIds.Add("garden:" + g.id))
                    issues.Add("SPEC_PACK_ID_DUPLICATE: garden id '" + g.id + "' repeats.");

            return issues;
        }

        private static string Join(HashSet<string> set)
        {
            var sb = new System.Text.StringBuilder();
            int n = 0;
            foreach (var s in set) { if (n++ > 0) sb.Append(','); sb.Append(s); if (n >= 12) { sb.Append(",…"); break; } }
            return sb.ToString();
        }
    }
}
