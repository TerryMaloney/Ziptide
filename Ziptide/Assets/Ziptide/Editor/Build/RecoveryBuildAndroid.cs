using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Build
{
    /// <summary>
    /// Recovery candidate Android builder. It reuses the canonical patch/audit path, then builds
    /// the player with a per-build compile define. Project-wide scripting symbols are not changed.
    ///
    /// Batch entrypoint:
    ///   -executeMethod Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK
    /// </summary>
    public static class RecoveryBuildAndroid
    {
        public static void PatchScenesThenGoldenAPK()
        {
            BuildAndroid.PatchScenesAndAudit();
            Ziptide.Editor.Setup.ApplyQuestPlayerDefaults.EnsureSplashDisabled();
            GoldenAPK();
        }

        public static void GoldenAPK()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string outDir = Path.Combine(projectRoot, "Builds", "Android");
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "Ziptide.apk");
            string[] sceneList = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            var options = new BuildPlayerOptions
            {
                scenes = sceneList,
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging,
                extraScriptingDefines = new[] { RecoveryBuildProfile.GoldenDefine }
            };

            Debug.Log("ZIPTIDE: BUILD_PROFILE profile=GoldenSlice define=" +
                      RecoveryBuildProfile.GoldenDefine + " output=" + outPath);

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Golden Android build failed: " + report.summary.result);

            Debug.Log("ZIPTIDE: GOLDEN_APK_BUILT path=" + outPath);
        }
    }
}
