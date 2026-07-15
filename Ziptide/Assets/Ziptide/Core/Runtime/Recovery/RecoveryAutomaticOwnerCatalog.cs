using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    public sealed class RecoveryAutomaticOwnerRegistration
    {
        public string OwnerId { get; }
        public string SourceRelativePath { get; }
        public string Symbol { get; }
        public RecoveryFeatureId FeatureId { get; }
        public RecoveryOwnerClassification Classification { get; }

        public RecoveryAutomaticOwnerRegistration(
            string ownerId,
            string sourceRelativePath,
            string symbol,
            RecoveryFeatureId featureId,
            RecoveryOwnerClassification classification)
        {
            OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
            SourceRelativePath = sourceRelativePath ?? throw new ArgumentNullException(nameof(sourceRelativePath));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            FeatureId = featureId;
            Classification = classification;
        }
    }

    /// <summary>
    /// Code-side mirror of docs/recovery/automatic_runtime_owners.json.
    /// The PlayMode contract test fails when the R0 evidence catalog and this gate catalog drift.
    /// </summary>
    public static class RecoveryAutomaticOwnerCatalog
    {
        private static readonly RecoveryAutomaticOwnerRegistration[] Registrations =
        {
            Gated("DEBUG_HUD_BOOTSTRAP", "Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs", "Ziptide.Core.DebugHUD", RecoveryFeatureId.DebugHud),
            Gated("XR_CAMERA_ENFORCER", "Ziptide/Assets/Ziptide/Core/Runtime/EnsureXRCameraActive.cs", "Ziptide.Core.EnsureXRCameraActive", RecoveryFeatureId.XrCameraEnforcer),
            Gated("RUNTIME_HEALTH_MONITOR", "Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs", "Ziptide.Core.RuntimeHealthMonitor", RecoveryFeatureId.RuntimeHealthMonitor),
            Gated("RUNTIME_INPUT_ENABLER", "Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs", "Ziptide.Core.RuntimeInputEnabler", RecoveryFeatureId.RuntimeInputEnabler),
            Gated("RUNTIME_MATERIAL_FIXER", "Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs", "Ziptide.Core.RuntimeMaterialFixer", RecoveryFeatureId.RuntimeMaterialFixer),
            Gated("VR_BOOT_DIAGNOSTICS", "Ziptide/Assets/Ziptide/Core/Runtime/VRBootDiagnostics.cs", "Ziptide.Core.VRBootDiagnostics", RecoveryFeatureId.VrBootDiagnostics),
            Gated("AMBIENCE_DIRECTOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs", "Ziptide.Gameplay.AmbienceDirector", RecoveryFeatureId.AmbienceDirector),
            Gated("COMFORT_VIGNETTE", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs", "Ziptide.Gameplay.ComfortVignette", RecoveryFeatureId.ComfortVignette),
            Gated("CONQUEST_MISSION_INJECTOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs", "Ziptide.Gameplay.ConquestMissionRuntime", RecoveryFeatureId.ConquestMissionInjector),
            Gated("DEV_WARP_BOARD_BOOTSTRAP", "Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs", "Ziptide.Gameplay.DevTools.DevWarpBoard", RecoveryFeatureId.DevWarpBoard),
            Gated("ECOLOGY_INJECTOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs", "Ziptide.Gameplay.EcologyDirector", RecoveryFeatureId.EcologyInjector),
            Gated("PVP_PROGRESSION_BOOTSTRAP", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs", "Ziptide.Gameplay.PvpProgressionRuntime", RecoveryFeatureId.PvpProgression),
            Gated("QUARTERS_CAMERA_INJECTOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs", "Ziptide.Gameplay.QuartersCameraFeature", RecoveryFeatureId.QuartersCameraInjector),
            Required("SAVE_SYSTEM_BOOTSTRAP", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs", "Ziptide.Gameplay.SaveSystem", RecoveryFeatureId.SaveSystem),
            Gated("FIRST_HOUR_OBSERVATION_BOOTSTRAP", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs", "Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter", RecoveryFeatureId.FirstHourObservation),
            Required("PLAYER_RIG_PERSISTENCE", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs", "Ziptide.Gameplay.PlayerRigPersistence", RecoveryFeatureId.PlayerRigPersistence),
            Required("AUDIO_DIRECTOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs", "Ziptide.Gameplay.AudioDirector", RecoveryFeatureId.AudioDirector),
            Required("TRAVEL_COORDINATOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs", "Ziptide.Gameplay.TravelCoordinator", RecoveryFeatureId.TravelCoordinator),
            Gated("SINGLETON_VALIDATOR", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Diagnostics/SingletonValidator.cs", "Ziptide.Gameplay.SingletonValidator", RecoveryFeatureId.SingletonValidator)
        };

        private static readonly IReadOnlyList<RecoveryAutomaticOwnerRegistration> ReadOnlyRegistrations =
            Array.AsReadOnly(Registrations);

        public static IReadOnlyList<RecoveryAutomaticOwnerRegistration> All => ReadOnlyRegistrations;

        private static RecoveryAutomaticOwnerRegistration Gated(
            string ownerId,
            string source,
            string symbol,
            RecoveryFeatureId featureId)
        {
            return new RecoveryAutomaticOwnerRegistration(
                ownerId, source, symbol, featureId, RecoveryOwnerClassification.FeatureGated);
        }

        private static RecoveryAutomaticOwnerRegistration Required(
            string ownerId,
            string source,
            string symbol,
            RecoveryFeatureId featureId)
        {
            return new RecoveryAutomaticOwnerRegistration(
                ownerId, source, symbol, featureId, RecoveryOwnerClassification.AlwaysRequired);
        }
    }
}
