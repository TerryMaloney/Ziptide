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

            try { Ziptide.Editor.Patching.ScenePatcherStarterWorld.EnsureInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] StarterWorld build-settings ensure warning: " + ex.Message); }

            try { Ziptide.Editor.Patching.ScenePatcherToxicCity.EnsureInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] ToxicCity build-settings ensure warning: " + ex.Message); }

            try { Ziptide.Editor.Patching.ScenePatcherPvP.EnsureInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] PvP arena build-settings ensure warning: " + ex.Message); }

            // Data-driven worlds: first seed any missing story-world layout assets (create-only — never
            // overwrites edits), then every CityLayoutDefinition with a sceneName gets its generated
            // scene created + enabled here and populated in the loop below. Author a layout → it ships.
            try { Ziptide.Editor.Patching.CreatureVariantAuthor.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Creature data author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.BotProfileAuthor.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Bot profile author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.BuildingStyleAuthor.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Building style author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.CosmeticAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Cosmetics author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.CameraAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Field camera author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.RillLineAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] RILL line author warning: " + ex.Message); }
            // FH-X01: compile the approved first-hour JSON into one runtime Resources asset before
            // audit/build. Invalid source fails closed inside the author and leaves base gameplay intact.
            try { Ziptide.Editor.Patching.FirstHourContractAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] First-hour contract author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.GardenAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Garden author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.VehicleAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Vehicle author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.EconomyAuthor.EnsureAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Economy author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.WorldLayoutLibrary.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] World layout library warning: " + ex.Message); }
            // ARCHITECTURE V2 Q1: committed world specs (docs/worldspecs/*.spec.json) are the editable
            // truth ON TOP of the library's seeded defaults — apply them before scenes generate.
            try { Ziptide.Editor.Spec.WorldSpecCompiler.CompileAll(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] World spec compile warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.WorldStubGenerator.EnsureGeneratedInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Generated-worlds build-settings ensure warning: " + ex.Message); }

            // W011's generated surface always owns a cave-mouth trigger. Generate and enable its
            // destination before the scene list is captured and before the travel-destination audit.
            try { Ziptide.Editor.Patching.ScenePatcherCavern.EnsureUndercroftInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] W011 undercroft build-settings ensure warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.CaveSpawnSafety.EnsureUndercroftSpawnFloor(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] W011 undercroft spawn-floor warning: " + ex.Message); }

            // PvP arenas: seed missing layout assets (create-only), then ensure their scenes ship.
            try { Ziptide.Editor.Patching.ArenaLayoutLibrary.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Arena layout library warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.ArenaWeaponAuthor.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Arena weapon author warning: " + ex.Message); }
            // A4.5 augments: the six gem definitions (create-only; WiringValidator's build-hook law).
            try { Ziptide.Editor.Patching.AugmentAuthor.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Augment author warning: " + ex.Message); }
            try { Ziptide.Editor.Patching.ScenePatcherArena.EnsureAllInBuildSettings(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Arena build-settings ensure warning: " + ex.Message); }

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

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherStarterWorld.SceneName)
                {
                    try { Ziptide.Editor.Patching.ScenePatcherStarterWorld.PopulateActiveStarterWorld(); }
                    catch (Exception ex) { Debug.LogWarning("[Ziptide] StarterWorld patcher warning for " + path + ": " + ex.Message); }
                }

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherToxicCity.SceneName)
                {
                    try { Ziptide.Editor.Patching.ScenePatcherToxicCity.PopulateActiveToxicCity(); }
                    catch (Exception ex) { Debug.LogWarning("[Ziptide] ToxicCity patcher warning for " + path + ": " + ex.Message); }
                }

                if (sceneName == Ziptide.Editor.Patching.ScenePatcherPvP.SceneName)
                {
                    try { Ziptide.Editor.Patching.ScenePatcherPvP.PopulateActivePvP(); }
                    catch (Exception ex) { Debug.LogWarning("[Ziptide] PvP arena patcher warning for " + path + ": " + ex.Message); }
                }

                // Generated worlds: if this scene belongs to a CityLayoutDefinition (by sceneName),
                // rebuild it from its data so every build reflects the current layout asset.
                try { Ziptide.Editor.Patching.WorldStubGenerator.PatchActiveSceneIfGenerated(); }
                catch (Exception ex) { Debug.LogWarning("[Ziptide] Generated-world patcher warning for " + path + ": " + ex.Message); }

                // Generated arenas: same contract for ArenaLayoutDefinitions.
                try { Ziptide.Editor.Patching.ScenePatcherArena.PatchActiveSceneIfArena(); }
                catch (Exception ex) { Debug.LogWarning("[Ziptide] Generated-world patcher warning for " + path + ": " + ex.Message); }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveOpenScenes();
            }

            // Sky vistas (art track): seed missing vista assets (create-only), then point each generated
            // theme at its canon vista. Runs after the loop so every <Scene>_Theme.asset exists.
            try { Ziptide.Editor.Patching.SkyVistaLibrary.EnsureAllAuthored(); Ziptide.Editor.Patching.SkyVistaAuthor.AssignAll(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Sky vista author warning: " + ex.Message); }

            // Forge (art track): seed missing recipe assets (create-only), then point item definitions
            // at their generated looks. ItemFactory applies them at spawn via ForgeVisualApplier.
            try { Ziptide.Editor.Patching.ForgeRecipeLibrary.EnsureAllAuthored(); Ziptide.Editor.Patching.ForgeAuthor.AssignAll(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Forge author warning: " + ex.Message); }
            // F3.7: Shell-sign bodies are a separate create-only recipe catalog; the placement author
            // consumes these ids during generated-world dressing and the baker picks them up below.
            try { Ziptide.Editor.Patching.SignRecipeLibrary.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Shell sign recipe author warning: " + ex.Message); }
            // P3+P4: seed missing creature GENOME assets (create-only, ids match CreatureDefinitions).
            // CreatureBehaviorBase applies the skinned walking body at spawn via ForgeCreatureVisualApplier.
            try { Ziptide.Editor.Patching.ForgeBodyLibrary.EnsureAllAuthored(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Forge body author warning: " + ex.Message); }
            // E1.4: bake mesh+maps+material+prefab per recipe → Resources/ForgeBaked (gitignored,
            // regenerated every build). The runtime applier prefers these textured looks on device.
            try { Ziptide.Editor.Patching.ForgeBaker.BakeAll(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Forge baker warning: " + ex.Message); }
            // R3: deterministic manifest + dependency reports → Builds/Reports (gitignored artifact).
            try { Ziptide.Editor.Patching.ForgeDependencyAuditor.WriteReports(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Forge dependency report warning: " + ex.Message); }

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

            // Rebuild the in-VR dev-menu manifest LAST, after all patchers have updated the world packs
            // (incl. the D0 → "D0 City (legacy)" rename). Doing it here means the menu is always fresh and
            // correct without a manual ordering step — fixes the "two Toxic City entries" caused by the
            // manifest being rebuilt by hand BEFORE the rename patcher ran.
            try { Ziptide.Editor.DevTools.DevWorldManifestBuilder.Rebuild(); }
            catch (Exception ex) { Debug.LogWarning("[Ziptide] Dev world manifest rebuild warning: " + ex.Message); }

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
