#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>ToxicCity Stage B object/material/traversal gates.</summary>
    public static class CityStageBAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            if (report.sceneName != ScenePatcherToxicCity.SceneName) return;

            Transform marker = Object.FindObjectsOfType<Transform>()
                .FirstOrDefault(t => t != null && t.name == ToxicCityStageB.RootName);
            if (marker == null)
            {
                report.Blocker("CITY_STAGE_B_MISSING",
                    "ToxicCity has no Stage B street-life marker. Save hook did not run.");
                return;
            }

            var materialIds = new HashSet<int>();
            int districtCount = 0;
            foreach (Transform district in Object.FindObjectsOfType<Transform>())
            {
                if (district == null || !district.name.StartsWith("District_")) continue;
                districtCount++;
                Transform street = district.Find(ToxicCityStageB.DistrictRootName);
                if (street == null)
                {
                    report.Blocker("CITY_STAGE_B_DISTRICT_MISSING",
                        district.name + " has no street-life layer.", district.name);
                    continue;
                }

                int lamps = street.GetComponentsInChildren<Transform>(true)
                    .Count(t => t.name.StartsWith("StreetLamp_"));
                if (lamps != ToxicCityStageB.LampsPerDistrict)
                    report.Blocker("CITY_STAGE_B_LAMP_COUNT",
                        district.name + " has " + lamps + " street lamps; expected "
                        + ToxicCityStageB.LampsPerDistrict + ".", district.name);
                if (street.Find("DistrictSign") == null)
                    report.Blocker("CITY_STAGE_B_SIGN_MISSING",
                        district.name + " has no district identity sign.", district.name);
                if (street.GetComponent<CityStreetLifeRuntime>() == null)
                    report.Blocker("CITY_STAGE_B_MOTION_MISSING",
                        district.name + " has no street-life motion runtime.", district.name);

                Collider[] colliders = street.GetComponentsInChildren<Collider>(true);
                if (colliders.Length > 0)
                    report.Blocker("CITY_STAGE_B_DECORATIVE_COLLIDER",
                        district.name + " has " + colliders.Length
                        + " collider(s) under presentation-only street life.", district.name);

                int objects = ToxicCityStageB.CountObjects(street);
                if (objects > ToxicCityStageB.HardObjectsPerDistrict)
                    report.Blocker("CITY_STAGE_B_OBJECT_BUDGET",
                        district.name + " has " + objects + " Stage B objects (cap "
                        + ToxicCityStageB.HardObjectsPerDistrict + ").", district.name);
                else if (objects > ToxicCityStageB.TargetObjectsPerDistrict)
                    Debug.LogWarning("ZIPTIDE: CITY_STAGE_B_BUDGET_YELLOW district="
                        + district.name + " objects=" + objects);

                foreach (Renderer renderer in street.GetComponentsInChildren<Renderer>(true))
                    if (renderer != null && renderer.sharedMaterial != null &&
                        renderer.sharedMaterial.name.StartsWith("CityStageB_"))
                        materialIds.Add(renderer.sharedMaterial.GetInstanceID());

                foreach (Transform moving in street.GetComponentsInChildren<Transform>(true))
                {
                    if (!moving.name.StartsWith("Steam_") && !moving.name.StartsWith("LampGlow_")) continue;
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(moving.gameObject);
                    if ((flags & StaticEditorFlags.BatchingStatic) != 0)
                        report.Blocker("CITY_STAGE_B_MOVING_STATIC",
                            moving.name + " is animated but marked batching-static.", district.name + "/" + moving.name);
                }
            }

            if (districtCount == 0)
                report.Blocker("CITY_STAGE_B_NO_DISTRICTS", "ToxicCity contains no district roots.");
            if (materialIds.Count > ToxicCityStageB.HardMaterials)
                report.Blocker("CITY_STAGE_B_MATERIAL_BUDGET",
                    "Stage B uses " + materialIds.Count + " semantic materials (cap "
                    + ToxicCityStageB.HardMaterials + ").", ToxicCityStageB.RootName);
        }
    }
}
#endif
