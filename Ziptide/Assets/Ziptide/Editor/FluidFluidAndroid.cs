using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Ziptide.Build
{
    /// <summary>
    /// Build entrypoint for Android (Quest). Use Ziptide.Build.BuildAndroid.APK for -executeMethod.
    /// </summary>
    public static class FluidFluidAndroid
    {
        public static void APK()
        {
            // Run Ziptide > Validate dependencies from the menu if you want to enforce asmdef rules.
            var outDir = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "Builds", "Android");
            Directory.CreateDirectory(outDir);

            var outPath = Path.Combine(outDir, "Ziptide.apk");

            var scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            var opts = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            var report = BuildPipeline.BuildPlayer(opts);

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Android build failed: " + report.summary.result);

            UnityEngine.Debug.Log("Built APK: " + outPath);
        }
    }
}
