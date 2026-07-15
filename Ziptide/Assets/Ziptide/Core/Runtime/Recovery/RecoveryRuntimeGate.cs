using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// Sole decision seam for automatic bootstrap exposure.
    /// The explicit default is FullDevelopment; it is never inferred from Debug.isDebugBuild.
    /// Automatic owners are migrated to Allows(featureId) in bounded follow-up commits.
    /// </summary>
    public static class RecoveryRuntimeGate
    {
        private static RecoveryExposureProfile _activeProfile = RecoveryExposureProfiles.FullDevelopment;
        private static bool _bootProfileLogged;

        public static RecoveryExposureProfile ActiveProfile => _activeProfile;
        public static string ActiveProfileName => _activeProfile.Name;
        public static IReadOnlyList<RecoveryFeatureId> ActiveFeatureIds => _activeProfile.EnabledFeatures;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _activeProfile = RecoveryExposureProfiles.FullDevelopment;
            _bootProfileLogged = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LogBootProfile()
        {
            if (_bootProfileLogged) return;
            _bootProfileLogged = true;
            Debug.Log(BuildLogLine("RECOVERY_EXPOSURE"));
        }

        public static bool Allows(RecoveryFeatureId featureId) => _activeProfile.Allows(featureId);

        public static void SetActiveProfile(RecoveryExposureProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (ReferenceEquals(_activeProfile, profile)) return;

            _activeProfile = profile;
            Debug.Log(BuildLogLine("RECOVERY_EXPOSURE_CHANGED"));
        }

        private static string BuildLogLine(string tag)
        {
            var builder = new StringBuilder(192);
            builder.Append("ZIPTIDE: ").Append(tag)
                .Append(" profile=").Append(_activeProfile.Name)
                .Append(" features=");

            var enabled = _activeProfile.EnabledFeatures;
            for (int i = 0; i < enabled.Count; i++)
            {
                if (i > 0) builder.Append(',');
                builder.Append(enabled[i]);
            }
            return builder.ToString();
        }
    }
}
