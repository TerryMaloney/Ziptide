using UnityEditor;

namespace Ziptide.Editor.Setup
{
    /// <summary>
    /// Applies Quest-safe Player settings from Cursor so you don't have to click through Unity.
    /// Menu: Ziptide > Apply Quest player defaults.
    /// </summary>
    public static class ApplyQuestPlayerDefaults
    {
        private const string MenuPath = "Ziptide/Apply Quest player defaults";

        [MenuItem(MenuPath)]
        public static void Apply()
        {
            // Switch to Android so PlayerSettings apply to the right platform
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            // Player
            PlayerSettings.companyName = "TerryMaloney";
            PlayerSettings.productName = "ZIPTIDE";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.terrymaloney.ziptide");

            // Android / Other Settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64; // Quest: ARM64 only, no ARMv7
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29; // Android 10

            // Active Input Handling: set manually in Edit > Project Settings > Player > Other Settings (Input System Package or Both)

            UnityEngine.Debug.Log("[Ziptide] Quest player defaults applied: com.terrymaloney.ziptide, IL2CPP, ARM64, API 29. Set XR Plug-in Management (OpenXR) and add scenes in Build Settings if needed.");
        }
    }
}
