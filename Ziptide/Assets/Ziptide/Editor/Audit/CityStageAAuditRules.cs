#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Editor.Patching;

namespace Ziptide.Editor.Audit
{
    /// <summary>Quest-facing gates for ToxicCity Stage A's composite facade recipe.</summary>
    public static class CityStageAAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            foreach (Transform district in Object.FindObjectsOfType<Transform>())
            {
                if (district == null || !district.name.StartsWith("District_")) continue;
                if (district.Find("__CITY_STAGE_A_CURBS") == null) continue;

                var facades = new List<Transform>();
                for (int i = 0; i < district.childCount; i++)
                {
                    Transform child = district.GetChild(i);
                    if (child != null && child.name.StartsWith("Facade_")) facades.Add(child);
                }

                if (facades.Count == 0)
                    report.Blocker("CITY_STAGE_A_EMPTY",
                        district.name + " has the Stage A marker but no composite facades.");

                foreach (Transform facade in facades)
                {
                    if (facade.Find("Base") == null || facade.Find("Middle") == null || facade.Find("Cap") == null)
                        report.Blocker("CITY_STAGE_A_GRAMMAR_MISSING",
                            district.name + "/" + facade.name + " is missing Base, Middle or Cap.");
                }

                int objects = ToxicCityStageA.CountObjects(district);
                if (objects > ToxicCityStageA.HardObjectsPerDistrict)
                    report.Blocker("CITY_STAGE_A_OBJECT_BUDGET",
                        district.name + " has " + objects + " Stage A objects (hard cap " +
                        ToxicCityStageA.HardObjectsPerDistrict + "). Reduce window rows or roof clutter.");
                else if (objects > ToxicCityStageA.TargetObjectsPerDistrict)
                    Debug.LogWarning("ZIPTIDE: CITY_STAGE_A_BUDGET_YELLOW district=" + district.name
                        + " objects=" + objects + " target=" + ToxicCityStageA.TargetObjectsPerDistrict);

                int materials = ToxicCityStageA.CountMaterials(district);
                if (materials > ToxicCityStageA.HardMaterialsPerDistrict)
                    report.Blocker("CITY_STAGE_A_MATERIAL_BUDGET",
                        district.name + " uses " + materials + " facade materials (hard cap " +
                        ToxicCityStageA.HardMaterialsPerDistrict + "). Keep the shared warmth vocabulary closed.");
            }
        }
    }
}
#endif
