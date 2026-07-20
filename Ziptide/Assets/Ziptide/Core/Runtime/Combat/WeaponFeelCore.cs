using UnityEngine;

namespace Ziptide.Core
{
    public enum WeaponFeelKind { Ballistic, Electric }

    public readonly struct WeaponHapticPulse
    {
        public readonly float Delay;
        public readonly float Amplitude;
        public readonly float Duration;

        public WeaponHapticPulse(float delay, float amplitude, float duration)
        {
            Delay = delay;
            Amplitude = amplitude;
            Duration = duration;
        }
    }

    public readonly struct WeaponFeelEnvelope
    {
        public readonly WeaponHapticPulse Primary;
        public readonly WeaponHapticPulse Tail;
        public readonly WeaponHapticPulse HitConfirm;
        public readonly float VisualKick;
        public readonly float VisualReturnSeconds;

        public WeaponFeelEnvelope(WeaponHapticPulse primary, WeaponHapticPulse tail,
            WeaponHapticPulse hitConfirm, float visualKick, float visualReturnSeconds)
        {
            Primary = primary;
            Tail = tail;
            HitConfirm = hitConfirm;
            VisualKick = visualKick;
            VisualReturnSeconds = visualReturnSeconds;
        }
    }

    /// <summary>
    /// Pure bounded feel derivation. Existing weapon definitions own source amplitude, duration,
    /// recoil and cadence; this layer derives only a restrained tail and hit-confirm pulse.
    /// </summary>
    public static class WeaponFeelCore
    {
        public static WeaponFeelEnvelope Resolve(WeaponFeelKind kind, float amplitude,
            float duration, float recoilKick, float cadenceSeconds)
        {
            float amp = Mathf.Clamp01(amplitude);
            float dur = Mathf.Clamp(duration, 0.015f, 0.12f);
            float tailFactor = kind == WeaponFeelKind.Electric ? 0.52f : 0.34f;
            float tailDurationFactor = kind == WeaponFeelKind.Electric ? 0.70f : 0.46f;
            float confirmFactor = kind == WeaponFeelKind.Electric ? 0.38f : 0.30f;
            var primary = new WeaponHapticPulse(0f, amp, dur);
            var tail = new WeaponHapticPulse(dur + 0.012f,
                Mathf.Clamp01(amp * tailFactor), Mathf.Clamp(dur * tailDurationFactor, 0.012f, 0.08f));
            var confirm = new WeaponHapticPulse(0f,
                Mathf.Clamp01(amp * confirmFactor), Mathf.Clamp(dur * 0.42f, 0.012f, 0.05f));
            float kick = Mathf.Clamp(recoilKick, 0f, 0.04f);
            float returnSeconds = Mathf.Clamp(cadenceSeconds * 0.55f, 0.06f, 0.18f);
            return new WeaponFeelEnvelope(primary, tail, confirm, kick, returnSeconds);
        }
    }
}
