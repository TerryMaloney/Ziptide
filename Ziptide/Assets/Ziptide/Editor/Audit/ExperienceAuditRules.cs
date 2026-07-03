#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// THE QUALITY GATES (Quality Bar P1f) — the mid-level-LLM insurance. Called per non-boot scene by
    /// <see cref="WorldAuditRunner"/>. A world whose layout opted into the Experience recipe must
    /// actually SHIP the experience: real terrain, a real arrival landmark, real scale. These checks
    /// read the DATA plus the opened scene, so a bland-by-data world FAILS THE BUILD instead of
    /// shipping a 30-second box-maze — the bar is enforced, not hoped for.
    ///
    /// Gate roster (grows with the program; see docs/SPRINT.md):
    ///  - WORLD_TOO_SMALL       (blocker)  worldRadius under 200m
    ///  - TERRAIN_MISSING       (blocker)  no ExperienceTerrain mesh+collider in the scene
    ///  - NO_VISTA_LANDMARK     (blocker)  no hero landmark for the arrival sightline
    ///  - POI_COUNT_LOW         (blocker)  fewer than 5 POIs — not enough to explore
    ///  - VERB_VARIETY_LOW      (blocker)  fewer than 3 distinct POI verbs — same-y gameplay
    ///  - STORY_ANCHOR_MISSING  (blocker)  no staged story beat in the world
    ///  - EST_PLAY_MINUTES_LOW  (blocker)  PoiQuality heuristic under 8 minutes
    ///  - TERRAIN_SLOPE_UNWALKABLE (blocker) TerrainField.WalkableFraction under its bar (H3)
    /// Scenes whose layout has experience disabled (arenas, interiors like W000, legacy) are exempt.
    /// </summary>
    public static class ExperienceAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            var kit = FindLayoutForScene(report.sceneName);
            var ex = kit != null ? kit.experience : null;
            if (ex == null || !ex.enabled) return;

            if (ex.worldRadius < 200f)
                report.Blocker("WORLD_TOO_SMALL",
                    "experience.worldRadius=" + ex.worldRadius.ToString("F0") + "m is under the 200m quality bar. " +
                    "Raise it on the layout asset (target 250-400) and regenerate the world.");

            var terrain = GameObject.Find("ExperienceTerrain");
            var meshCollider = terrain != null ? terrain.GetComponent<MeshCollider>() : null;
            var meshFilter = terrain != null ? terrain.GetComponent<MeshFilter>() : null;
            if (terrain == null || meshCollider == null || meshCollider.sharedMesh == null ||
                meshFilter == null || meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertexCount < 100)
            {
                report.Blocker("TERRAIN_MISSING",
                    "Layout has experience.enabled but the scene has no built ExperienceTerrain " +
                    "(mesh + collider). Regenerate: Ziptide > Worlds > Generate All Layout Worlds.");
            }

            if (ex.vista == VistaKind.None)
            {
                report.Blocker("NO_VISTA_LANDMARK",
                    "experience.vista is None — every experience world needs an arrival landmark. " +
                    "Pick a VistaKind on the layout asset.");
            }
            else
            {
                var vistaRoot = GameObject.Find("ArrivalVista");
                bool hasHero = false;
                if (vistaRoot != null)
                    foreach (Transform child in vistaRoot.transform)
                        if (child != null && child.name.StartsWith("Hero_")) { hasHero = true; break; }
                if (!hasHero)
                    report.Blocker("NO_VISTA_LANDMARK",
                        "Layout wants vista " + ex.vista + " but the scene has no ArrivalVista/Hero_* " +
                        "assembly. Regenerate: Ziptide > Worlds > Generate All Layout Worlds.");
            }

            RunPoiGates(report, kit);

            // H3: the terrain math promises MOSTLY-walkable ground (TerrainFieldTests pin it per
            // biome) — this re-measures THIS world's actual biome/amplitude/seed combination, so a
            // hand-tuned spec can never ship an unclimbable world.
            float walkable = TerrainField.WalkableFraction(
                ex.biome, ex.heightAmplitude, kit.seed, Mathf.Max(60f, ex.worldRadius) * 0.8f);
            if (walkable < TerrainField.MinWalkableFraction)
                report.Blocker("TERRAIN_SLOPE_UNWALKABLE",
                    "Only " + (walkable * 100f).ToString("F0") + "% of the terrain is walkable (bar " +
                    (TerrainField.MinWalkableFraction * 100f).ToString("F0") + "%). Lower " +
                    "experience.heightAmplitude or pick a gentler biome.");
        }

        // ── POI gates (P1c) — the "30 seconds of gameplay" rejection ─────────────────────────────

        private static void RunPoiGates(SceneAuditReport report, CityLayoutDefinition kit)
        {
            var pois = kit.pois;
            int count = 0;
            if (pois != null)
                foreach (var p in pois)
                    if (p != null) count++;

            if (count < 5)
            {
                report.Blocker("POI_COUNT_LOW",
                    "World has " + count + " POIs — the quality bar is 5-9. Author the POI table on the " +
                    "layout asset (docs/WORLD_RECIPE.md) or clear kit.pois to re-seed the standard ring.");
                return; // the remaining gates would just repeat the same root cause
            }

            int verbs = PoiQuality.DistinctVerbCount(pois);
            if (verbs < 3)
                report.Blocker("VERB_VARIETY_LOW",
                    "POI set covers only " + verbs + " distinct verbs — the bar is 3+. Mix types " +
                    "(CombatCamp / HarvestGrove / MachineSite / RuinCache / CaveSecret / StoryAnchor).");

            if (!PoiQuality.HasStoryAnchor(pois))
                report.Blocker("STORY_ANCHOR_MISSING",
                    "No StoryAnchor POI — every world stages its WORLD_DATA beat as a setpiece.");

            Vector3 spawn = Vector3.zero;
            foreach (var d in kit.districts)
                if (d != null && (d.id == kit.spawnDistrictId || spawn == Vector3.zero)) spawn = d.anchor;
            float minutes = PoiQuality.EstimatePlayMinutes(pois, spawn);
            if (minutes < 8f)
                report.Blocker("EST_PLAY_MINUTES_LOW",
                    "Estimated play is " + minutes.ToString("F1") + " min — the bar is 8+. Add POIs, " +
                    "raise tiers, or spread the network wider (PoiQuality documents the heuristic).");
        }

        private static CityLayoutDefinition FindLayoutForScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return null;
            foreach (var guid in AssetDatabase.FindAssets("t:CityLayoutDefinition"))
            {
                var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (kit != null && kit.sceneName == sceneName) return kit;
            }
            return null;
        }
    }
}
#endif
