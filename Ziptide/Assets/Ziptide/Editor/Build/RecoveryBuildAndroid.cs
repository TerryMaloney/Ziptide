using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Build
{
    /// <summary>
    /// Recovery candidate Android builder. It reuses the canonical patch/audit path, applies the
    /// three-scene Golden-route corrections that depend on the reduced scene set, re-audits that exact
    /// route, then builds only the R0-locked Golden scenes with a per-build compile define. Project-wide
    /// scripting symbols and EditorBuildSettings are not changed.
    ///
    /// Batch entrypoint:
    ///   -executeMethod Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK
    /// </summary>
    public static class RecoveryBuildAndroid
    {
        private static readonly string[] GoldenScenes =
        {
            "Assets/Ziptide/Scenes/_Boot.unity",
            "Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity",
            "Assets/Ziptide/Scenes/ToxicCity.unity"
        };

        private const string ToxicCityExitPackPath =
            "Assets/Ziptide/Content/Worlds/Packs/ToxicCityExit_WorldPack.asset";
        private const string GoldenBridgeName = "__RECOVERY_GOLDEN_SHIPYARD_BRIDGE";
        private static readonly Vector3 GoldenBridgeCenter = new Vector3(0f, -0.5f, -39.5f);
        private static readonly Vector3 GoldenBridgeSize = new Vector3(20f, 1f, 2f);

        /// <summary>Exact return scene authorized by the three-scene recovery profile.</summary>
        public static string GoldenExitSceneName => ZiptideConstants.SceneW000;

        /// <summary>Pure geometry contract used by EditMode tests before any scene mutation.</summary>
        public static Bounds GoldenShipyardBridgeBounds => new Bounds(GoldenBridgeCenter, GoldenBridgeSize);

        /// <summary>Runtime destinations legal in the locked recovery player (Boot is never a world row).</summary>
        public static bool IsGoldenDestinationScene(string sceneName)
        {
            return sceneName == ZiptideConstants.SceneW000 || sceneName == ZiptideConstants.SceneToxicCity;
        }

        /// <summary>Immutable scene profile consumed by PG-2 tests and the exact-profile travel gate.</summary>
        public static IReadOnlyList<string> GoldenScenePaths => GoldenScenes;

        [Serializable]
        private sealed class GoldenBuildReport
        {
            public string profile;
            public string define;
            public string result;
            public string outputPath;
            public string[] scenes;
            public ulong totalBytes;
            public double durationSeconds;
        }

        public static void PatchScenesThenGoldenAPK()
        {
            BuildAndroid.PatchScenesAndAudit();
            PatchAndValidateGoldenRoute();
            Ziptide.Editor.Setup.ApplyQuestPlayerDefaults.EnsureSplashDisabled();
            GoldenAPK();
        }

        /// <summary>
        /// The canonical patcher sees the project's full EditorBuildSettings list, while this recovery
        /// player intentionally ships only three scenes. Pin ToxicCity's return to W000, remove every
        /// serialized travel row whose scene is absent from this APK, bridge the district/berth seam, then
        /// run both the ordinary project audit and PG-2 against the exact saved artifact scene universe.
        /// </summary>
        private static void PatchAndValidateGoldenRoute()
        {
            var exitPack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(ToxicCityExitPackPath);
            if (exitPack == null)
                throw new FileNotFoundException("Golden ToxicCity exit pack is missing.", ToxicCityExitPackPath);

            exitPack.sceneName = GoldenExitSceneName;
            EditorUtility.SetDirty(exitPack);
            AssetDatabase.SaveAssets();

            int removedRows = 0;
            removedRows += PatchGoldenScene(GoldenScenes[1], addShipyardBridge: false);
            removedRows += PatchGoldenScene(GoldenScenes[2], addShipyardBridge: true);

            if (exitPack.sceneName != GoldenExitSceneName)
                throw new Exception("Golden ToxicCity exit does not target W000.");

            int projectBlockers = Ziptide.Editor.Audit.WorldAuditRunner.RunAll();
            if (projectBlockers > 0)
                throw new Exception("Golden route project audit FAILED with " + projectBlockers + " blocker(s).");

            int profileTravelBlockers = Ziptide.Editor.Audit.BuildProfileTravelAuditRules.RunScenes(GoldenScenes);
            if (profileTravelBlockers > 0)
                throw new Exception("Golden artifact-profile travel audit FAILED with "
                    + profileTravelBlockers + " blocker(s).");

            Debug.Log("ZIPTIDE: GOLDEN_ROUTE_PATCH exit=" + exitPack.sceneName
                + " bridge=" + GoldenBridgeCenter.ToString("F2")
                + " size=" + GoldenBridgeSize.ToString("F2")
                + " removedDeadTravelRows=" + removedRows
                + " projectAuditBlockers=" + projectBlockers
                + " profileTravelBlockers=" + profileTravelBlockers);
        }

        private static int PatchGoldenScene(string scenePath, bool addShipyardBridge)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int removedRows = FilterGoldenTravelDestinations(scene.name);

            if (addShipyardBridge)
            {
                var bridge = GameObject.Find(GoldenBridgeName);
                if (bridge == null)
                {
                    bridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bridge.name = GoldenBridgeName;
                }

                bridge.transform.position = GoldenBridgeCenter;
                bridge.transform.rotation = Quaternion.identity;
                bridge.transform.localScale = GoldenBridgeSize;
                var collider = bridge.GetComponent<BoxCollider>();
                if (collider == null) collider = bridge.AddComponent<BoxCollider>();
                collider.enabled = true;
                collider.isTrigger = false;

                var renderer = bridge.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader == null) shader = Shader.Find("Standard");
                    if (shader != null)
                    {
                        var material = new Material(shader);
                        var color = new Color(0.28f, 0.30f, 0.33f);
                        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
                        else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
                        renderer.sharedMaterial = material;
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }

                if (bridge.GetComponent<Collider>() == null || !bridge.GetComponent<Collider>().enabled)
                    throw new Exception("Golden ToxicCity shipyard bridge has no enabled collider.");
            }

            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, scenePath))
                throw new Exception("Failed to save Golden scene route patch: " + scenePath);
            return removedRows;
        }

        /// <summary>
        /// Artifact-profile reduction must cover every serialized pack-array owner. Filtering only the
        /// ship helm left ordinary WorldTravelStation doors pointing at editor-valid worlds absent from the
        /// three-scene APK; PG-2 correctly rejected that mismatch. Keep only the opposite Golden world.
        /// </summary>
        private static int FilterGoldenTravelDestinations(string currentSceneName)
        {
            int removed = 0;
            foreach (ShipBoardingStation station in UnityEngine.Object.FindObjectsOfType<ShipBoardingStation>(true))
                if (station != null) removed += FilterDestinationPackArray(station, currentSceneName);
            foreach (WorldTravelStation station in UnityEngine.Object.FindObjectsOfType<WorldTravelStation>(true))
                if (station != null) removed += FilterDestinationPackArray(station, currentSceneName);
            return removed;
        }

        private static int FilterDestinationPackArray(Component owner, string currentSceneName)
        {
            var serialized = new SerializedObject(owner);
            var packsProperty = serialized.FindProperty("destinationPacks");
            if (packsProperty == null) return 0;

            int removed = 0;
            var kept = new List<WorldPackDefinition>();
            for (int i = 0; i < packsProperty.arraySize; i++)
            {
                var pack = packsProperty.GetArrayElementAtIndex(i).objectReferenceValue as WorldPackDefinition;
                if (pack != null && IsGoldenDestinationScene(pack.sceneName)
                    && pack.sceneName != currentSceneName)
                    kept.Add(pack);
                else
                    removed++;
            }

            packsProperty.arraySize = kept.Count;
            for (int i = 0; i < kept.Count; i++)
                packsProperty.GetArrayElementAtIndex(i).objectReferenceValue = kept[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(owner);

            Debug.Log("ZIPTIDE: GOLDEN_TRAVEL_REDUCER owner=" + owner.GetType().Name
                + " path=" + HierarchyPath(owner.transform)
                + " kept=" + kept.Count + " removed=" + removed);
            return removed;
        }

        private static string HierarchyPath(Transform value)
        {
            if (value == null) return string.Empty;
            string path = value.name;
            while (value.parent != null)
            {
                value = value.parent;
                path = value.name + "/" + path;
            }
            return path;
        }

        public static void GoldenAPK()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            ValidateGoldenScenes(projectRoot);

            string outDir = Path.Combine(projectRoot, "Builds", "Android");
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "Ziptide.apk");

            var options = new BuildPlayerOptions
            {
                scenes = (string[])GoldenScenes.Clone(),
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging,
                extraScriptingDefines = new[] { RecoveryBuildProfile.GoldenDefine }
            };

            Debug.Log("ZIPTIDE: BUILD_PROFILE profile=GoldenSlice define=" +
                      RecoveryBuildProfile.GoldenDefine +
                      " scenes=" + string.Join(",", GoldenScenes) +
                      " output=" + outPath);

            BuildReport report = BuildPipeline.BuildPlayer(options);
            WriteBuildProfileReport(projectRoot, outPath, report);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Golden Android build failed: " + report.summary.result);

            Debug.Log("ZIPTIDE: GOLDEN_APK_BUILT scenes=" + GoldenScenes.Length + " path=" + outPath);
        }

        private static void ValidateGoldenScenes(string projectRoot)
        {
            for (int i = 0; i < GoldenScenes.Length; i++)
            {
                string scenePath = Path.Combine(projectRoot, GoldenScenes[i]);
                if (!File.Exists(scenePath))
                    throw new FileNotFoundException("Golden recovery scene is missing.", scenePath);
            }
        }

        private static void WriteBuildProfileReport(string projectRoot, string outPath, BuildReport report)
        {
            string reportDir = Path.Combine(projectRoot, "Builds", "Reports");
            Directory.CreateDirectory(reportDir);
            string reportPath = Path.Combine(reportDir, "recovery_golden_build_profile.json");

            var payload = new GoldenBuildReport
            {
                profile = "GoldenSlice",
                define = RecoveryBuildProfile.GoldenDefine,
                result = report.summary.result.ToString(),
                outputPath = outPath,
                scenes = (string[])GoldenScenes.Clone(),
                totalBytes = report.summary.totalSize,
                durationSeconds = report.summary.totalTime.TotalSeconds
            };

            File.WriteAllText(reportPath, JsonUtility.ToJson(payload, true) + Environment.NewLine);
            Debug.Log("ZIPTIDE: GOLDEN_BUILD_REPORT path=" + reportPath +
                      " result=" + payload.result +
                      " scenes=" + payload.scenes.Length);
        }
    }
}
