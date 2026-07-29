using System;

namespace Ziptide.Core
{
    /// <summary>The mix buses. Order is serialized into player prefs — append only, never reorder.</summary>
    public enum AudioBus
    {
        Music = 0,
        Ambience = 1,
        Sfx = 2,
        Voice = 3,
    }

    /// <summary>
    /// The mix maths, with no Unity, no prefs and no clock — so the one rule that actually protects a
    /// player's ears is testable.
    ///
    /// The game shipped with no player volume control at all: one hard-coded 0.35 on the music profile
    /// and a hard-coded 0.5 on the ambience bed. A VR game a child plays with headphones on needs a
    /// master and per-bus level, and it needs them to be honest — a slider at zero must mean SILENT,
    /// not "quiet enough that you probably won't notice".
    /// </summary>
    public static class AudioMixCore
    {
        /// <summary>Anything at or below this is treated as true silence, not a whisper.</summary>
        public const float SilenceEpsilon = 0.001f;

        public const float DefaultMaster = 0.8f;
        public const float DefaultMusic = 0.7f;
        public const float DefaultAmbience = 0.8f;
        public const float DefaultSfx = 1f;
        public const float DefaultVoice = 1f;

        public static float DefaultFor(AudioBus bus)
        {
            switch (bus)
            {
                case AudioBus.Music: return DefaultMusic;
                case AudioBus.Ambience: return DefaultAmbience;
                case AudioBus.Voice: return DefaultVoice;
                default: return DefaultSfx;
            }
        }

        /// <summary>Clamp a slider to 0..1, treating NaN as "the author meant the default".</summary>
        public static float Clamp01(float value, float fallback)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return fallback;
            if (value < 0f) return 0f;
            return value > 1f ? 1f : value;
        }

        /// <summary>
        /// The level a source should actually play at: the author's mix, scaled by the bus, scaled by
        /// master. Zero on ANY of the three is exactly zero out — a muted bus is muted, and no amount
        /// of authored loudness overrides a player who turned it off.
        /// </summary>
        public static float Effective(float authored, float busVolume, float masterVolume)
        {
            float a = Clamp01(authored, 0f);
            float b = Clamp01(busVolume, 0f);
            float m = Clamp01(masterVolume, 0f);

            if (a <= SilenceEpsilon || b <= SilenceEpsilon || m <= SilenceEpsilon) return 0f;
            return a * b * m;
        }

        /// <summary>
        /// Story ducking: how far a bed drops while a line is being spoken. Returns the multiplier to
        /// apply, never the new volume, so a caller cannot accidentally duck a duck.
        /// </summary>
        public static float DuckMultiplier(bool voiceActive, float duckTo = 0.35f)
        {
            if (!voiceActive) return 1f;
            return Clamp01(duckTo, 0.35f);
        }

        /// <summary>Pref key for one bus. Stable strings — renaming one silently resets a player's mix.</summary>
        public static string PrefKey(AudioBus bus)
        {
            return "ziptide.audio." + Enum.GetName(typeof(AudioBus), bus).ToLowerInvariant();
        }

        public const string MasterPrefKey = "ziptide.audio.master";
    }
}
