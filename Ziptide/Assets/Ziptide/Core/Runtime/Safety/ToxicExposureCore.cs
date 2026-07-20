using UnityEngine;

namespace Ziptide.Core
{
    public readonly struct ToxicExposureStep
    {
        public readonly bool Entered;
        public readonly bool Exited;
        public readonly bool DamagePulse;
        public readonly bool Lethal;
        public readonly float ExposureSeconds;

        public ToxicExposureStep(bool entered, bool exited, bool damagePulse, bool lethal,
            float exposureSeconds)
        {
            Entered = entered;
            Exited = exited;
            DamagePulse = damagePulse;
            Lethal = lethal;
            ExposureSeconds = exposureSeconds;
        }
    }

    /// <summary>
    /// Pure toxic-immersion clock. Exposure pulses at a bounded cadence, becomes lethal after a fixed
    /// continuous interval, emits lethal once, and fully resets when the player exits the volume.
    /// </summary>
    public sealed class ToxicExposureCore
    {
        public const float DefaultPulseInterval = 0.75f;
        public const float DefaultLethalSeconds = 4.5f;

        public float PulseIntervalSeconds { get; }
        public float LethalAfterSeconds { get; }
        public bool IsInside { get; private set; }
        public float ExposureSeconds { get; private set; }

        private float _nextPulseAt;
        private bool _lethalEmitted;

        public ToxicExposureCore(float pulseIntervalSeconds = DefaultPulseInterval,
            float lethalAfterSeconds = DefaultLethalSeconds)
        {
            PulseIntervalSeconds = Mathf.Max(0.15f, pulseIntervalSeconds);
            LethalAfterSeconds = Mathf.Max(PulseIntervalSeconds, lethalAfterSeconds);
            _nextPulseAt = PulseIntervalSeconds;
        }

        public ToxicExposureStep Tick(bool inside, float deltaTime)
        {
            bool entered = inside && !IsInside;
            bool exited = !inside && IsInside;

            if (exited)
            {
                Reset();
                return new ToxicExposureStep(false, true, false, false, 0f);
            }

            if (!inside)
                return new ToxicExposureStep(false, false, false, false, 0f);

            if (entered)
            {
                IsInside = true;
                ExposureSeconds = 0f;
                _nextPulseAt = PulseIntervalSeconds;
                _lethalEmitted = false;
            }

            ExposureSeconds += Mathf.Max(0f, deltaTime);
            bool pulse = ExposureSeconds >= _nextPulseAt;
            if (pulse)
            {
                while (_nextPulseAt <= ExposureSeconds)
                    _nextPulseAt += PulseIntervalSeconds;
            }

            bool lethal = !_lethalEmitted && ExposureSeconds >= LethalAfterSeconds;
            if (lethal) _lethalEmitted = true;

            return new ToxicExposureStep(entered, false, pulse, lethal, ExposureSeconds);
        }

        public void Reset()
        {
            IsInside = false;
            ExposureSeconds = 0f;
            _nextPulseAt = PulseIntervalSeconds;
            _lethalEmitted = false;
        }
    }
}
