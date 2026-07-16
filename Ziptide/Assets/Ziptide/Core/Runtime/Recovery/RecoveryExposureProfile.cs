using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    public sealed class RecoveryExposureProfile
    {
        private readonly HashSet<RecoveryFeatureId> _enabled;
        private readonly IReadOnlyList<RecoveryFeatureId> _ordered;

        public string Name { get; }
        public IReadOnlyList<RecoveryFeatureId> EnabledFeatures => _ordered;

        internal RecoveryExposureProfile(string name, IEnumerable<RecoveryFeatureId> enabled)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name is required.", nameof(name));
            if (enabled == null)
                throw new ArgumentNullException(nameof(enabled));

            Name = name;
            _enabled = new HashSet<RecoveryFeatureId>(enabled);

            foreach (var registration in RecoveryAutomaticOwnerCatalog.All)
            {
                if (registration.Classification == RecoveryOwnerClassification.AlwaysRequired &&
                    !_enabled.Contains(registration.FeatureId))
                {
                    throw new InvalidOperationException(
                        "Recovery profile '" + name + "' omitted always-required owner " + registration.OwnerId + ".");
                }
            }

            var ordered = new List<RecoveryFeatureId>(_enabled);
            ordered.Sort((a, b) => ((int)a).CompareTo((int)b));
            _ordered = Array.AsReadOnly(ordered.ToArray());
        }

        public bool Allows(RecoveryFeatureId featureId) => _enabled.Contains(featureId);
    }

    public static class RecoveryExposureProfiles
    {
        // Clean-package proof marker (2026-07-16): the bounded Input System 1.6.3 / XRI 2.4.3
        // matrix must import from an empty Unity Library and pass the unchanged PlayMode, EditMode,
        // patch/audit and Golden Android routes before it can become the headset candidate.
        private static readonly RecoveryFeatureId[] GoldenFeatures =
        {
            RecoveryFeatureId.RuntimeHealthMonitor,
            RecoveryFeatureId.AmbienceDirector,
            RecoveryFeatureId.ComfortVignette,
            // Ecology remains compiled and available in FullDevelopment, but the R0 manifest
            // requires named-creature contact/grounding proof before it can enter GoldenSlice.
            RecoveryFeatureId.SaveSystem,
            RecoveryFeatureId.FirstHourObservation,
            RecoveryFeatureId.PlayerRigPersistence,
            RecoveryFeatureId.PlayerInputSessionGuard,
            RecoveryFeatureId.AudioDirector,
            RecoveryFeatureId.TravelCoordinator,
            RecoveryFeatureId.SingletonValidator
        };

        private static readonly HashSet<RecoveryFeatureId> ApprovedDiagnostics =
            new HashSet<RecoveryFeatureId>
            {
                RecoveryFeatureId.DebugHud,
                RecoveryFeatureId.VrBootDiagnostics,
                RecoveryFeatureId.DevWarpBoard
            };

        public static RecoveryExposureProfile FullDevelopment { get; } =
            new RecoveryExposureProfile("FullDevelopment", AllFeatureIds());

        public static RecoveryExposureProfile GoldenSlice { get; } =
            new RecoveryExposureProfile("GoldenSlice", GoldenFeatures);

        public static RecoveryExposureProfile Diagnostic(params RecoveryFeatureId[] additionalDiagnostics)
        {
            var enabled = new HashSet<RecoveryFeatureId>(GoldenFeatures);
            if (additionalDiagnostics != null)
            {
                for (int i = 0; i < additionalDiagnostics.Length; i++)
                {
                    var featureId = additionalDiagnostics[i];
                    if (!ApprovedDiagnostics.Contains(featureId))
                    {
                        throw new ArgumentException(
                            "Feature '" + featureId + "' is not an approved diagnostic surface.",
                            nameof(additionalDiagnostics));
                    }
                    enabled.Add(featureId);
                }
            }
            return new RecoveryExposureProfile("Diagnostic", enabled);
        }

        public static bool IsApprovedDiagnostic(RecoveryFeatureId featureId) =>
            ApprovedDiagnostics.Contains(featureId);

        private static IEnumerable<RecoveryFeatureId> AllFeatureIds()
        {
            var values = Enum.GetValues(typeof(RecoveryFeatureId));
            for (int i = 0; i < values.Length; i++)
                yield return (RecoveryFeatureId)values.GetValue(i);
        }
    }
}
