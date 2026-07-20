#if UNITY_EDITOR
using UnityEngine;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// V2.5 H1 building gates plus the invoked generated-world architecture aggregators.
    /// </summary>
    public static class BuildingAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            foreach (var root in Object.FindObjectsOfType<Transform>())
            {
                if (root == null || !root.name.StartsWith("__BUILDINGS_")) continue;

                int renderers = root.GetComponentsInChildren<Renderer>(true).Length;
                if (renderers > Ziptide.Editor.Patching.BuildingBuilder.MaxRenderersPerDistrict)
                    report.Blocker("BUILDING_OVER_BUDGET",
                        root.name + " has " + renderers + " renderers (cap " +
                        Ziptide.Editor.Patching.BuildingBuilder.MaxRenderersPerDistrict + "). Raise minLotArea " +
                        "or lower maxStoreys on the district's BuildingStyleDefinition and regenerate.");

                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name != "__DOOR") continue;
                    if (Physics.Raycast(t.position, t.forward, out var hit, 1.5f, ~0,
                            QueryTriggerInteraction.Ignore))
                        report.Blocker("BUILDING_DOOR_BLOCKED",
                            "Doorway at " + t.position.ToString("F1") + " in " + root.name +
                            " exits into '" + hit.collider.name + "' " + hit.distance.ToString("F2") + "m out. " +
                            "The lot frontage law should prevent this — check for overlapping props/patchers.");
                }
            }

            CityStageAAuditRules.Run(report);
            WorldContainmentAuditRules.Run(report);
            FullSendPresentationAuditRules.Run(report);
        }
    }
}
#endif
