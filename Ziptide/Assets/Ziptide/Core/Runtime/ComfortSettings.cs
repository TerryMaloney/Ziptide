using UnityEngine;

namespace Ziptide.Core
{
    public enum ComfortPreset
    {
        Cozy = 0,
        Standard = 1,
        Bold = 2
    }

    /// <summary>
    /// Immutable resolved comfort dials. This is data only: Gameplay translators apply the values to
    /// the owners they already control. A zero shipped flight ceiling means "retain shipped value".
    /// </summary>
    public readonly struct ComfortDialSet
    {
        public readonly bool smoothTurn;
        public readonly float snapTurnAngle;
        public readonly float smoothTurnSpeed;
        public readonly float vignetteStrength;
        public readonly float slideBoost;
        public readonly float slideSeconds;
        public readonly bool dashEnabled;
        public readonly float ziplineMaxSpeed;
        public readonly bool flightRollEnabled;
        public readonly float flightYawRepeatSeconds;
        public readonly bool useShippedFlightBoost;
        public readonly float flightBoostCeiling;

        public ComfortDialSet(
            bool smoothTurn,
            float snapTurnAngle,
            float smoothTurnSpeed,
            float vignetteStrength,
            float slideBoost,
            float slideSeconds,
            bool dashEnabled,
            float ziplineMaxSpeed,
            bool flightRollEnabled,
            float flightYawRepeatSeconds,
            bool useShippedFlightBoost,
            float flightBoostCeiling)
        {
            this.smoothTurn = smoothTurn;
            this.snapTurnAngle = snapTurnAngle;
            this.smoothTurnSpeed = smoothTurnSpeed;
            this.vignetteStrength = vignetteStrength;
            this.slideBoost = slideBoost;
            this.slideSeconds = slideSeconds;
            this.dashEnabled = dashEnabled;
            this.ziplineMaxSpeed = ziplineMaxSpeed;
            this.flightRollEnabled = flightRollEnabled;
            this.flightYawRepeatSeconds = flightYawRepeatSeconds;
            this.useShippedFlightBoost = useShippedFlightBoost;
            this.flightBoostCeiling = flightBoostCeiling;
        }
    }

    /// <summary>
    /// Device-level comfort preset storage and the locked Cozy / Standard / Bold dial table.
    /// It intentionally never references PlayerProfile: comfort belongs to the player's body/device
    /// and survives New Game or profile replacement.
    /// </summary>
    public static class ComfortSettings
    {
        public const string PresetPrefKey = "ziptide_comfort_preset";
        public const ComfortPreset DefaultPreset = ComfortPreset.Standard;

        public static ComfortPreset CurrentPreset
        {
            get
            {
                int raw = PlayerPrefs.GetInt(PresetPrefKey, (int)DefaultPreset);
                return raw >= (int)ComfortPreset.Cozy && raw <= (int)ComfortPreset.Bold
                    ? (ComfortPreset)raw
                    : DefaultPreset;
            }
        }

        public static void SelectPreset(ComfortPreset preset)
        {
            if (preset < ComfortPreset.Cozy || preset > ComfortPreset.Bold)
                preset = DefaultPreset;
            PlayerPrefs.SetInt(PresetPrefKey, (int)preset);
            PlayerPrefs.Save();
        }

        public static ComfortDialSet Resolve(ComfortPreset preset)
        {
            switch (preset)
            {
                case ComfortPreset.Cozy:
                    return new ComfortDialSet(
                        smoothTurn: false,
                        snapTurnAngle: 45f,
                        smoothTurnSpeed: 0f,
                        vignetteStrength: 1f,
                        slideBoost: 1f,
                        slideSeconds: 0f,
                        dashEnabled: true,
                        ziplineMaxSpeed: 5.5f,
                        flightRollEnabled: false,
                        flightYawRepeatSeconds: 0.55f,
                        useShippedFlightBoost: false,
                        flightBoostCeiling: 1.8f);

                case ComfortPreset.Bold:
                    return new ComfortDialSet(
                        smoothTurn: true,
                        snapTurnAngle: 0f,
                        smoothTurnSpeed: 120f,
                        vignetteStrength: 0.15f,
                        slideBoost: 1.35f,
                        slideSeconds: 0.8f,
                        dashEnabled: true,
                        ziplineMaxSpeed: 8f,
                        flightRollEnabled: true,
                        flightYawRepeatSeconds: 0.4f,
                        useShippedFlightBoost: true,
                        flightBoostCeiling: 0f);

                default:
                    return new ComfortDialSet(
                        smoothTurn: false,
                        snapTurnAngle: 30f,
                        smoothTurnSpeed: 0f,
                        vignetteStrength: 0.6f,
                        slideBoost: 1.35f,
                        slideSeconds: 0.8f,
                        dashEnabled: true,
                        ziplineMaxSpeed: 8f,
                        flightRollEnabled: true,
                        flightYawRepeatSeconds: 0.4f,
                        useShippedFlightBoost: true,
                        flightBoostCeiling: 0f);
            }
        }
    }
}
