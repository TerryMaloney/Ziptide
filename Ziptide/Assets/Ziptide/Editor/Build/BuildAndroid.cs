using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Ziptide.Build
{
    /// <summary>
    /// Canonical build entrypoint for Android (Quest).
    /// -executeMethod: Ziptide.Build.BuildAndroid.PatchScenesThenAPK (patch then build) or Ziptide.Build.BuildAndroid.APK (build only).
    /// </summary>
    public static class BuildAndroid
    {
        /// <summary>
        /// Opens each enabled build scene, runs ScenePatcherC0/D0/D1/D2 per scene, saves, then builds APK. Idempotent; safe for batchmode.
        /// </summary>
        public static void PatchScenesThenAPK()
        {
            // Step 1: Patch _Boot scene first (creates file if missing, inserts as first build scene).
            try { Ziptide.Editor.Patching.ScenePatcherBoot.PatchBootScene(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Boot patcher warning: " + ex.Message); }

            Ziptide.Editor.Patching.ScenePatcherD0.EnsureD0SceneExists();

            // Ensure the dev Sandbox scene is enabled in Build Settings BEFORE we read the list, so the
            // loop below opens + populates it and it ships (and can be warped to at runtime). Without
            // this the sandbox gear/drones never reach the headset. Idempotent; safe in batchmode.
            try { Ziptide.Editor.Patching.ScenePatcherSandbox.EnsureInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Sandbox build-settings ensure warning: " + ex.Message); }

            // Reload scenes list — ScenePatcherBoot/Sandbox may have modified it (added _Boot/Sandbox, removed SampleScene).
            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (!scenes[i].enabled) continue;
                string path = scenes[i].path;
                if (string.IsNullOrEmpty(path)) continue;

                // _Boot was already patched by ScenePatcherBoot — skip world patchers for it.
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (sceneName == "_Boot") continue;

                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

                try { Ziptide.Editor.Patching.ScenePatcherC0.PatchActiveScene(); }
                catch (Exception ex) { Debug.LogWarning("[Ziptide] C0 patcher warning for " + path + ": " + ex.Message); }

                try { Ziptide.Editor.Patching.ScenePatcherD0.PatchActiveScene(); }
                catch (Exception ex) { Debug.LogWarning("[Ziptide] D0 patcher warning for " + path + ": " + ex.Message); }

                try { Ziptide.Editor.Patching.ScenePatcherD1.PatchActiveScene(); }
                catch (Exception ex) { Debug.LogWarning("[Ziptide] D1 patcher warning for " + path + ": " + ex.Message); }

                try { Ziptide.Editor.Patching.ScenePatcherD2.PatchActiveScene(); }
                catch (Exception ex) { Debug.LogWarning("[Ziptide] D2 patcher warning for " + path + ": " + ex.Message); }

                // The sandbox is generated procedurally (gear + drones + zones) — populate it here so
                // every build has the content without a manual "Build Sandbox Test Lab" menu step.
                if (sceneName == Ziptide.Editor.Patching.ScenePatcherSandbox.SceneName)
                {
                    try { Ziptide.Editor.Patching.ScenePatcherSandbox.PopulateActiveSandbox(); }
                    catch (Exception ex) { Debug.LogWarning("[Ziptide] Sandbox patcher warning for " + path + ": " + ex.Message); }
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveOpenScenes();
            }

            // Run world integrity audit BEFORE building APK. Any BLOCKER aborts the build.
            int auditBlockers = 0;
            try
            {
                auditBlockers = Ziptide.Editor.Audit.WorldAuditRunner.RunAll();
            }
            catch (Exception auditEx)
            {
                Debug.LogWarning("[Ziptide] Audit threw exception: " + auditEx.Message);
            }
            if (auditBlockers > 0)
                throw new Exception("World audit FAILED with " + auditBlockers + " blocker(s). See docs/AUDIT_REPORT.md.");

            Ziptide.Editor.Setup.ApplyQuestPlayerDefaults.EnsureSplashDisabled();
            APK();
        }

        /// <summary>
        /// Build APK only (no scene patching). Use when scene is already prepared in Editor.
        /// </summary>
        public static void APK()
        {
            var outDir = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "Builds", "Android");
            Directory.CreateDirectory(outDir);
            var outPath = Path.Combine(outDir, "Ziptide.apk");
            var sceneList = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            var opts = new BuildPlayerOptions
            {
                scenes = sceneList,
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            var report = BuildPipeline.BuildPlayer(opts);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Android build failed: " + report.summary.result);
            Debug.Log("Built APK: " + outPath);
        }
    }
}
