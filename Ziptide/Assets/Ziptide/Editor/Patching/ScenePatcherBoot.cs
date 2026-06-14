using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Creates and patches the _Boot.unity scene that owns all DontDestroyOnLoad singletons.
    ///
    /// _Boot is always first in the build settings. It contains:
    ///   - XR Origin (PlayerRigPersistence, TravelCoordinator, AudioDirector, etc.)
    ///   - BootLoader (loads first world scene on Start)
    ///
    /// World scenes should NOT contain these singletons — they arrive via DontDestroyOnLoad.
    /// WorldAuditRunner.SINGLETON_IN_WORLD_SCENE warns if they are found in world scenes.
    /// </summary>
    public static class ScenePatcherBoot
    {
        private const string BootScenePath = "Assets/Ziptide/Scenes/_Boot.unity";
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("Ziptide/Apply Boot Scene Patcher")]
        public static void PatchFromMenu()
        {
            PatchBootScene();
        }

        /// <summary>
        /// Creates _Boot.unity (if needed), patches it with all singletons,
        /// and inserts it as the first enabled build scene (removing SampleScene).
        /// Called by BuildAndroid.PatchScenesThenAPK before world scene patching.
        /// </summary>
        public static void PatchBootScene()
        {
            string previousScenePath = EditorSceneManager.GetActiveScene().path;

            EnsureBootSceneFileExists();

            var scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);

            // Reuse D2 patcher to set up XR Origin + all singletons.
            // D2 patcher is idempotent; safe to call on _Boot.
            try { ScenePatcherD2.PatchActiveScene(); }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[Ziptide] Boot: D2 patcher warning: " + ex.Message);
            }

            EnsureBootLoader();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();

            // Insert _Boot as first scene in build settings and remove SampleScene.
            AddBootToFirstPositionInBuild();

            // Restore previous scene.
            if (!string.IsNullOrEmpty(previousScenePath) && File.Exists(previousScenePath))
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);

            Debug.Log("[Ziptide] _Boot scene patched. Build order updated: _Boot is now index 0.");
        }

        private static void EnsureBootSceneFileExists()
        {
            if (File.Exists(FullDiskPath(BootScenePath))) return;

            EnsureFolder("Assets/Ziptide/Scenes");

            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(newScene, BootScenePath);
            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] Created _Boot.unity at " + BootScenePath);
        }

        private static void EnsureBootLoader()
        {
            if (Object.FindObjectOfType<BootLoader>() != null) return;

            var go = new GameObject("BootLoader");
            go.AddComponent<BootLoader>();
            Undo.RegisterCreatedObjectUndo(go, "Create BootLoader");
        }

        private static void AddBootToFirstPositionInBuild()
        {
            var existing = EditorBuildSettings.scenes;

            var updated = new List<EditorBuildSettingsScene>();

            // Always first: _Boot
            updated.Add(new EditorBuildSettingsScene(BootScenePath, true));

            foreach (var entry in existing)
            {
                if (entry.path == BootScenePath) continue;   // no duplicate
                if (entry.path == SampleScenePath) continue; // remove SampleScene
                updated.Add(entry);
            }

            EditorBuildSettings.scenes = updated.ToArray();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static string FullDiskPath(string assetPath)
        {
            // Convert Assets/... to absolute disk path.
            string projectRoot = Application.dataPath.Replace("Assets", "").TrimEnd('/', '\\');
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static void EnsureFolder(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath)) return;
            string parent = Path.GetDirectoryName(assetFolderPath)?.Replace('\\', '/') ?? "Assets";
            string folderName = Path.GetFileName(assetFolderPath);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
