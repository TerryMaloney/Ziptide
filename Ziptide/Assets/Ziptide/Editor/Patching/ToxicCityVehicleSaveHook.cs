#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>Save-boundary integration for ToxicCity's visible starter fleet.</summary>
    [InitializeOnLoad]
    public static class ToxicCityVehicleSaveHook
    {
        static ToxicCityVehicleSaveHook()
        {
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            ApplyToScene(scene);
        }

        public static bool ApplyToScene(Scene scene)
        {
            if (!scene.IsValid() || scene.name != ScenePatcherToxicCity.SceneName) return false;
            CityLayoutDefinition kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(
                ZiptideConstants.PathToxicCityLayout);
            if (kit == null)
            {
                Debug.LogError("ZIPTIDE: TOXIC_CITY_VEHICLE_HOOK_FAIL reason=missing_layout");
                return false;
            }

            string rootName = "__" + kit.cityId.ToUpperInvariant() + "_ROOT";
            Transform cityRoot = null;
            foreach (GameObject root in scene.GetRootGameObjects())
                if (root != null && root.name == rootName) { cityRoot = root.transform; break; }
            if (cityRoot == null)
            {
                Debug.LogError("ZIPTIDE: TOXIC_CITY_VEHICLE_HOOK_FAIL reason=missing_root root=" + rootName);
                return false;
            }

            ToxicCityVehicleBuilder.Summary summary = ToxicCityVehicleBuilder.Build(cityRoot, kit);
            Debug.Log("ZIPTIDE: TOXIC_CITY_VEHICLE_HOOK vehicles=" + summary.Vehicles
                + " pads=" + summary.Pads);
            return summary.Vehicles == ToxicCityVehicleBuilder.ExpectedVehicleCount;
        }
    }
}
#endif
