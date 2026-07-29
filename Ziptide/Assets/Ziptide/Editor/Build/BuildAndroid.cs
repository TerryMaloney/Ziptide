using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Ziptide.Build
{
    /// <summary>
    /// Canonical build entrypoint for Android (Quest).
    /// -executeMethod: Ziptide.Build.BuildAndroid.PatchScenesThenAPK (patch then build),
    /// Ziptide.Build.BuildAndroid.PatchScenesAndAudit (the CI gate: everything except the APK), or
    /// Ziptide.Build.BuildAndroid.APK (build only).
    /// </summary>
    public static class BuildAndroid
    {
        public static void PatchScenesThenAPK()
        {
            PatchAndAudit();
            Ziptide.Editor.Setup.ApplyQuestPlayerDefaults.EnsureSplashDisabled();
            APK();
        }

        /// <summary>
        /// Per-push CI gate: full patch, author, bake, shader-safety, and audit pipeline without APK.
        /// Every hook is required. Any exception aborts instead of shipping partial generated content.
        /// </summary>
        public static void PatchScenesAndAudit()
        {
            PatchAndAudit();
            Debug.Log("ZIPTIDE: PATCH_AUDIT_OK scenes patched + audit green (no APK)");
        }

        private static void PatchAndAudit()
        {
            RunRequired("RuntimeShaderVariantGate.Validate", RuntimeShaderVariantGate.Validate);

            RunRequired("ScenePatcherBoot.PatchBootScene",
                Ziptide.Editor.Patching.ScenePatcherBoot.PatchBootScene);
            RunRequired("ScenePatcherD0.EnsureD0SceneExists",
                Ziptide.Editor.Patching.ScenePatcherD0.EnsureD0SceneExists);

            RunRequired("ScenePatcherSandbox.EnsureInBuildSettings",
                Ziptide.Editor.Patching.ScenePatcherSandbox.EnsureInBuildSettings);
            RunRequired("ScenePatcherStarterWorld.EnsureInBuildSettings",
                Ziptide.Editor.Patching.ScenePatcherStarterWorld.EnsureInBuildSettings);
            RunRequired("ScenePatcherToxicCity.EnsureInBuildSettings",
                Ziptide.Editor.Patching.ScenePatcherToxicCity.EnsureInBuildSettings);
            RunRequired("ScenePatcherPvP.EnsureInBuildSettings",
                Ziptide.Editor.Patching.ScenePatcherPvP.EnsureInBuildSettings);
            // The first level's whole middle -- flight, salvage, approach, reentry -- happens here.
            // Its patcher was complete but menu-only, so the scene had never been created and the
            // space leg simply did not exist in any build.
            RunRequired("ScenePatcherSpaceLane.EnsureInBuildSettings",
                Ziptide.Editor.Patching.ScenePatcherSpaceLane.EnsureInBuildSettings);

            RunRequired("CreatureVariantAuthor.EnsureAllAuthored",
                Ziptide.Editor.Patching.CreatureVariantAuthor.EnsureAllAuthored);
            RunRequired("BotProfileAuthor.EnsureAllAuthored",
                Ziptide.Editor.Patching.BotProfileAuthor.EnsureAllAuthored);
            RunRequired("BuildingStyleAuthor.EnsureAllAuthored",
                Ziptide.Editor.Patching.BuildingStyleAuthor.EnsureAllAuthored);
            RunRequired("CosmeticAuthor.EnsureAuthored",
                Ziptide.Editor.Patching.CosmeticAuthor.EnsureAuthored);
            RunRequired("CameraAuthor.EnsureAuthored",
                () =>
                {
                    Ziptide.Editor.Patching.CameraAuthor.EnsureAuthored();
                });
            RunRequired("RillLineAuthor.EnsureAuthored",
                Ziptide.Editor.Patching.RillLineAuthor.EnsureAuthored);
            RunRequired("FirstHourContractAuthor.EnsureAuthored",
                Ziptide.Editor.Patching.FirstHourContractAuthor.EnsureAuthored);
            RunRequired("GardenAuthor.EnsureAuthored",
                Ziptide.Editor.Patching.GardenAuthor.EnsureAuthored);
            RunRequired("VehicleAuthor.EnsureAuthored",
                Ziptide.Editor.Patching.VehicleAuthor.EnsureAuthored);
            RunRequired("EconomyAuthor.EnsureAuthored",
                Ziptide.Editor.Patching.EconomyAuthor.EnsureAuthored);
            RunRequired("WorldLayoutLibrary.EnsureAllAuthored",
                Ziptide.Editor.Patching.WorldLayoutLibrary.EnsureAllAuthored);
            RunRequired("WorldSpecCompiler.CompileAll",
                Ziptide.Editor.Spec.WorldSpecCompiler.CompileAll);
            RunRequired("WorldStubGenerator.EnsureGeneratedInBuildSettings",
                Ziptide.Editor.Patching.WorldStubGenerator.EnsureGeneratedInBuildSettings);

            RunRequired("ScenePatcherCavern.EnsureUndercroftInBuildSettings",
                () =>
                {
                    Ziptide.Editor.Patching.ScenePatcherCavern.EnsureUndercroftInBuildSettings();
                });
            RunRequired("CaveSpawnSafety.EnsureUndercroftSpawnFloor",
                () =>
                {
                    Ziptide.Editor.Patching.CaveSpawnSafety.EnsureUndercroftSpawnFloor();
                });

            RunRequired("ArenaLayoutLibrary.EnsureAllAuthored",
                Ziptide.Editor.Patching.ArenaLayoutLibrary.EnsureAllAuthored);
            RunRequired("ArenaWeaponAuthor.EnsureAllAuthored",
                Ziptide.Editor.Patching.ArenaWeaponAuthor.EnsureAllAuthored);
            RunRequired("AugmentAuthor.EnsureAllAuthored",
                Ziptide.Editor.Patching.AugmentAuthor.EnsureAllAuthored);
            RunRequired("ScenePatcherArena.EnsureAllInBuildSettings",
                Ziptide.Editor.Patching.ScenePatcherArena.EnsureAllInBuildSettings);
            RunRequired("WorldImprovementCompiler.BeginCompileSession",
                Ziptide.Editor.WorldImprovement.WorldImprovementCompiler.BeginCompileSession);

            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (!scenes[i].enabled) continue;
                string path = scenes[i].path;
                if (string.IsNullOrEmpty(path)) continue;

                string sceneName = Path.GetFileNameWithoutExtension(path);
                if (sceneName == "_Boot") continue;

                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

                RunRequired("ScenePatcherC0.PatchActiveScene:" + path,
                    Ziptide.Editor.Patching.ScenePatcherC0.PatchActiveScene);
                RunRequired("ScenePatcherD0.PatchActiveScene:" + path,
                    Ziptide.Editor.Patching.ScenePatcherD0.PatchActiveScene);
                RunRequired("ScenePatcherD1.PatchActiveScene:" + path,
                    Ziptide.Editor.Patching.ScenePatcherD1.PatchActiveScene);
                RunRequired("ScenePatcherD2.PatchActiveScene:" + path,
                    Ziptide.Editor.Patching.ScenePatcherD2.PatchActiveScene);

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherSandbox.SceneName)
                    RunRequired("ScenePatcherSandbox.PopulateActiveSandbox:" + path,
                        Ziptide.Editor.Patching.ScenePatcherSandbox.PopulateActiveSandbox);

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherStarterWorld.SceneName)
                    RunRequired("ScenePatcherStarterWorld.PopulateActiveStarterWorld:" + path,
                        Ziptide.Editor.Patching.ScenePatcherStarterWorld.PopulateActiveStarterWorld);

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherToxicCity.SceneName)
                    RunRequired("ScenePatcherToxicCity.PopulateActiveToxicCity:" + path,
                        Ziptide.Editor.Patching.ScenePatcherToxicCity.PopulateActiveToxicCity);

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherPvP.SceneName)
                    RunRequired("ScenePatcherPvP.PopulateActivePvP:" + path,
                        Ziptide.Editor.Patching.ScenePatcherPvP.PopulateActivePvP);

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherSpaceLane.SceneName)
                    RunRequired("ScenePatcherSpaceLane.PatchActiveScene:" + path,
                        Ziptide.Editor.Patching.ScenePatcherSpaceLane.PatchActiveScene);

                // The W000 first-hour surfaces -- comfort console, bunk keepsake, curated first helm.
                // These were authored by a menu item nobody had run, so the three beats they carry
                // were unreachable in every shipped APK while their code sat CI-green.
                if (sceneName == "W000_DriftIn")
                    RunRequired("FirstHourSurfaceAuthor.Author:" + path,
                        () => Ziptide.Editor.FirstHourSurfaceAuthor.Author(scene));

                RunRequired("WorldStubGenerator.PatchActiveSceneIfGenerated:" + path,
                    Ziptide.Editor.Patching.WorldStubGenerator.PatchActiveSceneIfGenerated);
                RunRequired("ScenePatcherArena.PatchActiveSceneIfArena:" + path,
                    Ziptide.Editor.Patching.ScenePatcherArena.PatchActiveSceneIfArena);
                RunRequired("WorldImprovementCompiler.CompileActiveSceneIfDeclared:" + path,
                    () =>
                    {
                        Ziptide.Editor.WorldImprovement.WorldImprovementCompiler
                            .CompileActiveSceneIfDeclared(path);
                    });

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveOpenScenes())
                    throw new Exception("Required build step failed: save scene " + path);
            }

            RunRequired("WorldImprovementCompiler.WriteReport",
                Ziptide.Editor.WorldImprovement.WorldImprovementCompiler.WriteReport);

            RunRequired("SkyVistaAuthor",
                () =>
                {
                    Ziptide.Editor.Patching.SkyVistaLibrary.EnsureAllAuthored();
                    Ziptide.Editor.Patching.SkyVistaAuthor.AssignAll();
                });
            RunRequired("ForgeAuthor",
                () =>
                {
                    Ziptide.Editor.Patching.ForgeRecipeLibrary.EnsureAllAuthored();
                    Ziptide.Editor.Patching.ForgeAuthor.AssignAll();
                });
            RunRequired("SignRecipeLibrary.EnsureAllAuthored",
                () =>
                {
                    Ziptide.Editor.Patching.SignRecipeLibrary.EnsureAllAuthored();
                });
            RunRequired("ForgeBodyLibrary.EnsureAllAuthored",
                Ziptide.Editor.Patching.ForgeBodyLibrary.EnsureAllAuthored);

            int baked = RunRequired("ForgeBaker.BakeAll",
                Ziptide.Editor.Patching.ForgeBaker.BakeAll);
            if (baked <= 0)
                throw new Exception("Required build step failed: ForgeBaker produced no assets.");
            Debug.Log("ZIPTIDE: FORGE_BAKE_OK count=" + baked);

            RunRequired("ForgeDependencyAuditor.WriteReports",
                Ziptide.Editor.Patching.ForgeDependencyAuditor.WriteReports);

            int auditBlockers = RunRequired("WorldAuditRunner.RunAll",
                Ziptide.Editor.Audit.WorldAuditRunner.RunAll);
            if (auditBlockers > 0)
                throw new Exception("World audit FAILED with " + auditBlockers +
                                    " blocker(s). See docs/AUDIT_REPORT.md.");

            RunRequired("DevWorldManifestBuilder.Rebuild",
                Ziptide.Editor.DevTools.DevWorldManifestBuilder.Rebuild);
        }

        private static void RunRequired(string step, Action action)
        {
            try
            {
                action();
                Debug.Log("ZIPTIDE: BUILD_HOOK_OK step=" + step);
            }
            catch (Exception ex)
            {
                Debug.LogError("ZIPTIDE: BUILD_HOOK_FAIL step=" + step + " exception=" + ex);
                throw new Exception("Required build step failed: " + step, ex);
            }
        }

        private static T RunRequired<T>(string step, Func<T> action)
        {
            try
            {
                T result = action();
                Debug.Log("ZIPTIDE: BUILD_HOOK_OK step=" + step);
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError("ZIPTIDE: BUILD_HOOK_FAIL step=" + step + " exception=" + ex);
                throw new Exception("Required build step failed: " + step, ex);
            }
        }

        public static void APK()
        {
            var outDir = Path.Combine(
                Directory.GetParent(Application.dataPath)!.FullName,
                "Builds",
                "Android");
            Directory.CreateDirectory(outDir);
            var outPath = Path.Combine(outDir, "Ziptide.apk");
            var sceneList = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            var options = new BuildPlayerOptions
            {
                scenes = sceneList,
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Android build failed: " + report.summary.result);
            Debug.Log("Built APK: " + outPath);
        }
    }
}
