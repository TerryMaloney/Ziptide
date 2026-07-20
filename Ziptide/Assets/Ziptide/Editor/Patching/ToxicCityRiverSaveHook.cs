#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Save-boundary integration for Toxic River. ToxicCity's canonical patcher rebuilds the city root
    /// and saves in both menu and CI paths, so this hook upgrades the decorative canal output immediately
    /// before the same scene bytes are audited and packaged.
    /// </summary>
    [InitializeOnLoad]
    public static class ToxicCityRiverSaveHook
    {
        static ToxicCityRiverSaveHook()
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
            var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(ZiptideConstants.PathToxicCityLayout);
            if (kit == null)
            {
                Debug.LogError("ZIPTIDE: TOXIC_RIVER_HOOK_FAIL reason=missing_layout");
                return false;
            }

            string rootName = "__" + kit.cityId.ToUpperInvariant() + "_ROOT";
            Transform cityRoot = null;
            foreach (GameObject root in scene.GetRootGameObjects())
                if (root != null && root.name == rootName) { cityRoot = root.transform; break; }
            if (cityRoot == null)
            {
                Debug.LogError("ZIPTIDE: TOXIC_RIVER_HOOK_FAIL reason=missing_root root=" + rootName);
                return false;
            }

            ToxicCityRiverBuilder.Summary summary = ToxicCityRiverBuilder.Build(cityRoot, kit);
            Debug.Log("ZIPTIDE: TOXIC_RIVER_HOOK scene=" + scene.name
                + " rivers=" + summary.Rivers
                + " renderers=" + summary.Renderers
                + " colliders=" + summary.SolidColliders);
            return kit.canals.Count == 0 || summary.Rivers == kit.canals.Count;
        }
    }
}
#endif
