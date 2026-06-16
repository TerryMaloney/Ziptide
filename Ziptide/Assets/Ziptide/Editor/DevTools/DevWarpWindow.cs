#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.DevTools
{
    /// <summary>
    /// Developer navigation window — jump straight to any world while building. Lists every
    /// WorldPackDefinition (auto-discovered, so new worlds appear with no code change) plus the
    /// _Boot scene, each with:
    ///   • Open Scene   — open it in the editor to edit it.
    ///   • Play here →  — enter play from _Boot and auto-warp into this world (and a chosen marker).
    /// Editor-only; never shipped. Pairs with the runtime <c>DevWarp</c> service + <c>DevWarpPlayHook</c>.
    /// </summary>
    public class DevWarpWindow : EditorWindow
    {
        // Shared with DevWarpPlayHook via SessionState (survives the domain reload entering play mode).
        public const string PendingSceneKey = "Ziptide.DevWarp.PendingScene";
        public const string PendingMarkerKey = "Ziptide.DevWarp.PendingMarker";

        private const string BootScene = "_Boot";
        private Vector2 _scroll;

        [MenuItem("Ziptide/Dev/Warp Window")]
        public static void Open()
        {
            GetWindow<DevWarpWindow>("Dev Warp").Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Developer Warp", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Jump straight to the world you're building. 'Open Scene' edits it; 'Play here' runs " +
                "from _Boot and warps in. Editor/dev only — never shipped to players.", MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("_Boot (canonical entry)", GUILayout.Width(200));
                if (GUILayout.Button("Open Scene")) OpenSceneByName(BootScene);
                if (GUILayout.Button("Play from _Boot")) PlayFrom(null, null);
            }
            EditorGUILayout.Space();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            var packs = LoadAllWorldPacks();
            if (packs.Count == 0)
                EditorGUILayout.HelpBox("No WorldPackDefinition assets found yet.", MessageType.None);

            foreach (var pack in packs)
            {
                if (pack == null) continue;
                string label = string.IsNullOrEmpty(pack.displayName) ? pack.packId : pack.displayName;

                EditorGUILayout.LabelField(label + "   (" + pack.sceneName + ")", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Open Scene")) OpenSceneByName(pack.sceneName);
                    if (GUILayout.Button("Play here →")) PlayFrom(pack.sceneName, null);
                }

                if (pack.spawnMarkers != null && pack.spawnMarkers.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    foreach (var m in pack.spawnMarkers)
                    {
                        if (m == null) continue;
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("• " + m.markerId, GUILayout.Width(180));
                            if (GUILayout.Button("Play here →", GUILayout.Width(110)))
                                PlayFrom(pack.sceneName, m.markerId);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();
        }

        private static List<WorldPackDefinition> LoadAllWorldPacks()
        {
            var list = new List<WorldPackDefinition>();
            foreach (var guid in AssetDatabase.FindAssets("t:WorldPackDefinition"))
            {
                var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (pack != null) list.Add(pack);
            }
            return list;
        }

        private static string FindScenePath(string sceneName)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Scene"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == sceneName) return path;
            }
            return null;
        }

        private static void OpenSceneByName(string sceneName)
        {
            string path = FindScenePath(sceneName);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("[Ziptide] Dev Warp: scene not found: " + sceneName);
                return;
            }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }

        /// <summary>Stash a pending warp, open _Boot, and enter play. DevWarpPlayHook warps in once running.</summary>
        private static void PlayFrom(string sceneName, string markerId)
        {
            SessionState.SetString(PendingSceneKey, sceneName ?? string.Empty);
            SessionState.SetString(PendingMarkerKey, markerId ?? string.Empty);
            OpenSceneByName(BootScene);
            if (!Application.isPlaying) EditorApplication.isPlaying = true;
        }
    }
}
#endif
