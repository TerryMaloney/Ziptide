#if UNITY_EDITOR
using UnityEngine;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// V2.5 H1 building gates (ExperienceAuditRules pattern; per non-boot scene):
    ///  - BUILDING_DOOR_BLOCKED (blocker) a doorway's outward path is walled off — raycast from each
    ///    __DOOR marker (chest height, facing out) must reach 1.5m of clear air. LotPartitioner +
    ///    BuildingGrammar make this true by construction; the gate catches regressions in the BAKED
    ///    scene (e.g., a hand-moved prop or a colliding patcher).
    ///  - BUILDING_OVER_BUDGET (blocker) renderer count under one __BUILDINGS_ district root exceeds
    ///    the Quest draw-call proxy cap.
    /// Scenes with no __BUILDINGS_ roots are exempt (buildings are opt-in per district).
    /// The same audit entrypoint also runs the legacy ToxicCity Stage A grammar/material/object gates.
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
                    // Clear exit: nothing solid within 1.5m straight out of the doorway at chest height.
                    if (Physics.Raycast(t.position, t.forward, out var hit, 1.5f, ~0, QueryTriggerInteraction.Ignore))
                        report.Blocker("BUILDING_DOOR_BLOCKED",
                            "Doorway at " + t.position.ToString("F1") + " in " + root.name +
                            " exits into '" + hit.collider.name + "' " + hit.distance.ToString("F2") + "m out. " +
                            "The lot frontage law should prevent this — check for overlapping props/patchers.");
                }
            }

            CityStageAAuditRules.Run(report);
        }
    }
}
#endif
