namespace Ziptide.Core
{
    /// <summary>
    /// Closed runtime exposure identifiers for every automatic owner cataloged during R0.
    /// New automatic bootstraps must add an ID and catalog registration before shipping.
    /// </summary>
    public enum RecoveryFeatureId
    {
        DebugHud = 0,
        XrCameraEnforcer = 1,
        RuntimeHealthMonitor = 2,
        RuntimeInputEnabler = 3,
        RuntimeMaterialFixer = 4,
        VrBootDiagnostics = 5,
        AmbienceDirector = 6,
        ComfortVignette = 7,
        ConquestMissionInjector = 8,
        DevWarpBoard = 9,
        EcologyInjector = 10,
        PvpProgression = 11,
        QuartersCameraInjector = 12,
        SaveSystem = 13,
        FirstHourObservation = 14,
        PlayerRigPersistence = 15,
        AudioDirector = 16,
        TravelCoordinator = 17,
        SingletonValidator = 18
    }

    public enum RecoveryOwnerClassification
    {
        FeatureGated = 0,
        AlwaysRequired = 1
    }
}
