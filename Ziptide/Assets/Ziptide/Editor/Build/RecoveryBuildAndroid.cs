using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Build
{
    /// <summary>
    /// Recovery candidate Android builder. It reuses the canonical patch/audit path, then builds
    /// only the three R0-locked Golden scenes with a per-build compile define. Project-wide
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
            Ziptide.Editor.Setup.ApplyQuestPlayerDefaults.EnsureSplashDisabled();
            GoldenAPK();
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
