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
    ///  - WORLD_TOO_SMALL     (blocker)  worldRadius under 200m
    ///  - TERRAIN_MISSING     (blocker)  no ExperienceTerrain mesh+collider in the scene
    ///  - NO_VISTA_LANDMARK   (blocker)  no hero landmark for the arrival sightline
    ///  - POI_COUNT_LOW / VERB_VARIETY_LOW / EST_PLAY_MINUTES_LOW / STORY_ANCHOR_MISSING land WITH the
    ///    POI schema (P1c) — a gate must never precede the content that satisfies it, or CI reddens
    ///    for every lane.
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
