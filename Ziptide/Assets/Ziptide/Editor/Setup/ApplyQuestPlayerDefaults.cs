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

            // Disable Unity splash so VR build does not show flickering/doubled "Made with Unity" on Quest
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // Active Input Handling: set manually in Edit > Project Settings > Player > Other Settings (Input System Package or Both)

            UnityEngine.Debug.Log("[Ziptide] Quest player defaults applied: com.terrymaloney.ziptide, IL2CPP, ARM64, API 29. Set XR Plug-in Management (OpenXR) and add scenes in Build Settings if needed.");
        }

        /// <summary>
        /// Ensures Unity splash screen is disabled for the build. Called from BuildAndroid before APK() so the built app does not show the flickering "Made with Unity" screen on Quest.
        /// </summary>
        public static void EnsureSplashDisabled()
        {
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }
    }
}
