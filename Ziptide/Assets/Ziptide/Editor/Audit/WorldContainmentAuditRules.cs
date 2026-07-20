#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Structural anti-void gate. City districts and connection slabs must retain real support colliders.
    /// ToxicCity additionally must replace every declared decorative canal with a complete authored toxic
    /// river basin, so an apparent gap is either walkable structure or explicit gameplay—not empty space.
    /// </summary>
    public static class WorldContainmentAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            CheckDistrictFloors(report);
            CheckConnections(report);
            if (report.sceneName == ScenePatcherToxicCity.SceneName)
                CheckToxicCityRivers(report);
        }

        private static void CheckDistrictFloors(SceneAuditReport report)
        {
            foreach (Transform district in Object.FindObjectsOfType<Transform>())
            {
                if (district == null || !district.name.StartsWith("District_")) continue;
                Collider floor = null;
                for (int i = 0; i < district.childCount; i++)
                {
                    Transform child = district.GetChild(i);
                    if (child == null || !child.name.EndsWith("_Ground")) continue;
                    floor = child.GetComponent<Collider>();
                    break;
                }
                if (floor == null || !floor.enabled || floor.isTrigger)
                    report.Blocker("WORLD_DISTRICT_FLOOR_MISSING",
                        district.name + " has no enabled solid *_Ground collider. Players can fall through the district.",
                        district.name);
            }
        }

        private static void CheckConnections(SceneAuditReport report)
        {
            Transform connections = Object.FindObjectsOfType<Transform>()
                .FirstOrDefault(t => t != null && t.name == "Connections");
            if (connections == null) return;

            for (int i = 0; i < connections.childCount; i++)
            {
                Transform slab = connections.GetChild(i);
                if (slab == null || slab.GetComponent<Renderer>() == null) continue;
                Collider col = slab.GetComponent<Collider>();
                if (col == null || !col.enabled || col.isTrigger)
                    report.Blocker("WORLD_CONNECTION_FLOOR_MISSING",
                        "Connection '" + slab.name + "' has no enabled solid collider. The route contains an accidental gap.",
                        "Connections/" + slab.name);
            }
        }

        private static void CheckToxicCityRivers(SceneAuditReport report)
        {
            CityLayoutDefinition layout = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(
                ZiptideConstants.PathToxicCityLayout);
            int expected = layout != null && layout.canals != null ? layout.canals.Count : 0;

            GameObject legacy = GameObject.Find("Canals");
            if (legacy != null)
                report.Blocker("TOXIC_RIVER_LEGACY_CANAL",
                    "ToxicCity still contains the decorative-only Canals root. ToxicCityRiverBuilder did not replace it.",
                    legacy.name);

            Transform riverRoot = Object.FindObjectsOfType<Transform>()
                .FirstOrDefault(t => t != null && t.name == ToxicCityRiverBuilder.RootName);
            if (expected > 0 && riverRoot == null)
            {
                report.Blocker("TOXIC_RIVER_ROOT_MISSING",
                    "ToxicCity declares " + expected + " canal region(s) but has no generated toxic-river root.");
                return;
            }
            if (riverRoot == null) return;

            ToxicRiverRuntime[] rivers = riverRoot.GetComponentsInChildren<ToxicRiverRuntime>(true);
            if (rivers.Length != expected)
                report.Blocker("TOXIC_RIVER_COUNT_MISMATCH",
                    "ToxicCity declares " + expected + " river(s) but generated " + rivers.Length + ".",
                    ToxicCityRiverBuilder.RootName);

            foreach (ToxicRiverRuntime runtime in rivers)
            {
                if (runtime == null) continue;
                Transform river = runtime.transform;
                string path = ToxicCityRiverBuilder.RootName + "/" + river.name;
                if (!river.name.StartsWith(ToxicCityRiverBuilder.RiverPrefix))
                    report.Blocker("TOXIC_RIVER_MARKER_MISSING",
                        "ToxicRiverRuntime is not under the canonical river marker prefix.", path);

                RequireSolid(report, river.Find("BasinFloor"), "TOXIC_RIVER_FLOOR_MISSING", path,
                    "River basin has no solid bottom; it would become another fall-through gap.");
                RequireSolid(report, river.Find("Bank_W"), "TOXIC_RIVER_BANK_MISSING", path, "West bank missing or non-solid.");
                RequireSolid(report, river.Find("Bank_E"), "TOXIC_RIVER_BANK_MISSING", path, "East bank missing or non-solid.");
                RequireSolid(report, river.Find("Bank_S"), "TOXIC_RIVER_BANK_MISSING", path, "South bank missing or non-solid.");
                RequireSolid(report, river.Find("Bank_N"), "TOXIC_RIVER_BANK_MISSING", path, "North bank missing or non-solid.");

                Transform surface = river.Find("ToxicSurface");
                if (surface == null)
                    report.Blocker("TOXIC_RIVER_SURFACE_MISSING", "River has no visible ToxicSurface.", path);
                else
                {
                    Collider surfaceCollider = surface.GetComponent<Collider>();
                    if (surfaceCollider != null && surfaceCollider.enabled)
                        report.Blocker("TOXIC_RIVER_SURFACE_SOLID",
                            "Visible toxic surface must not masquerade as safe floor support.", path + "/ToxicSurface");
                }

                if (river.GetComponentInChildren<ToxicRiverSurfaceRuntime>(true) == null)
                    report.Blocker("TOXIC_RIVER_MOTION_MISSING",
                        "River has no flow/bubble presentation runtime.", path);
                if (runtime.BoundsSize.y < 1f || runtime.LethalAfterSeconds <= 0f)
                    report.Blocker("TOXIC_RIVER_RUNTIME_INVALID",
                        "River lethal envelope is not configured with meaningful depth/timing.", path);

                int flow = river.GetComponentsInChildren<Transform>(true)
                    .Count(t => t.name.StartsWith("Flow_"));
                int bubbles = river.GetComponentsInChildren<Transform>(true)
                    .Count(t => t.name.StartsWith("Bubble_"));
                if (flow < ToxicCityRiverBuilder.FlowRibbonCount || bubbles < ToxicCityRiverBuilder.BubbleCount)
                    report.Blocker("TOXIC_RIVER_PRESENTATION_INCOMPLETE",
                        "River presentation has flow=" + flow + " bubbles=" + bubbles
                        + "; expected at least " + ToxicCityRiverBuilder.FlowRibbonCount + "/"
                        + ToxicCityRiverBuilder.BubbleCount + ".", path);
            }
        }

        private static void RequireSolid(SceneAuditReport report, Transform value, string code,
            string path, string message)
        {
            Collider collider = value != null ? value.GetComponent<Collider>() : null;
            if (collider == null || !collider.enabled || collider.isTrigger)
                report.Blocker(code, message, path + "/" + (value != null ? value.name : "missing"));
        }
    }
}
#endif
