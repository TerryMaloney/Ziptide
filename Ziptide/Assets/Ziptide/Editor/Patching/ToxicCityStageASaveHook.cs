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
    /// Explicit save-boundary integration for Stage A. ScenePatcherToxicCity rebuilds its root and then
    /// saves in both menu and CI flows; enriching during sceneSaving guarantees the audited and packaged
    /// scene is the same generated output without creating a second city builder.
    /// </summary>
    [InitializeOnLoad]
    public static class ToxicCityStageASaveHook
    {
        static ToxicCityStageASaveHook()
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
                Debug.LogError("ZIPTIDE: CITY_STAGE_A_HOOK_FAIL reason=missing_layout");
                return false;
            }

            string rootName = "__" + kit.cityId.ToUpperInvariant() + "_ROOT";
            Transform cityRoot = null;
            foreach (GameObject root in scene.GetRootGameObjects())
                if (root != null && root.name == rootName) { cityRoot = root.transform; break; }
            if (cityRoot == null)
            {
                Debug.LogError("ZIPTIDE: CITY_STAGE_A_HOOK_FAIL reason=missing_root root=" + rootName);
                return false;
            }

            ToxicCityStageA.Summary summary = ToxicCityStageA.Enrich(cityRoot, kit);
            Debug.Log("ZIPTIDE: CITY_STAGE_A_HOOK scene=" + scene.name
                + " facades=" + summary.Facades + " renderers=" + summary.Renderers);
            return summary.Facades > 0;
        }
    }
}
#endif
