namespace Ziptide.Core
{
    public enum RecoveryBuildProfileKind
    {
        FullDevelopment = 0,
        GoldenSlice = 1
    }

    /// <summary>
    /// Compile-time recovery build selection. The player define is supplied per build through
    /// BuildPlayerOptions.extraScriptingDefines, so project-wide scripting symbols are never mutated.
    /// No profile is inferred from Debug.isDebugBuild.
    /// </summary>
    public static class RecoveryBuildProfile
    {
        public const string GoldenDefine = "ZIPTIDE_RECOVERY_GOLDEN";

        public static RecoveryBuildProfileKind ResolveKind(bool goldenDefinePresent) =>
            goldenDefinePresent
                ? RecoveryBuildProfileKind.GoldenSlice
                : RecoveryBuildProfileKind.FullDevelopment;

        public static RecoveryExposureProfile ResolveProfile(bool goldenDefinePresent) =>
            goldenDefinePresent
                ? RecoveryExposureProfiles.GoldenSlice
                : RecoveryExposureProfiles.FullDevelopment;

        public static RecoveryBuildProfileKind CompiledKind
        {
            get
            {
#if ZIPTIDE_RECOVERY_GOLDEN
                return RecoveryBuildProfileKind.GoldenSlice;
#else
                return RecoveryBuildProfileKind.FullDevelopment;
#endif
            }
        }

        public static RecoveryExposureProfile CompiledProfile =>
            ResolveProfile(CompiledKind == RecoveryBuildProfileKind.GoldenSlice);
    }
}
