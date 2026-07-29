using System;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The player's volume mix — master plus one level per bus.
    ///
    /// Stored in <see cref="PlayerPrefs"/>, deliberately NOT in the save profile: a volume setting
    /// belongs to the headset and the room, not to a campaign. It must survive deleting a save,
    /// starting a new game, or handing the headset to someone else mid-session — the same law the
    /// comfort presets already follow.
    ///
    /// This is the seam an AudioMixer asset slots behind later without touching a single call site:
    /// the mixer cannot be authored from a script, so until Terry creates one in the editor the
    /// directors read their levels from here directly, and the result is identical to the player.
    /// </summary>
    public static class AudioMixSettings
    {
        /// <summary>Raised whenever any level changes, so live sources can re-apply without polling.</summary>
        public static event Action Changed;

        private static bool _loaded;
        private static float _master;
        private static readonly float[] _buses = new float[4];

        public static float Master
        {
            get { EnsureLoaded(); return _master; }
            set { EnsureLoaded(); Set(ref _master, AudioMixCore.MasterPrefKey, value, AudioMixCore.DefaultMaster); }
        }

        public static float Get(AudioBus bus)
        {
            EnsureLoaded();
            return _buses[(int)bus];
        }

        public static void Set(AudioBus bus, float value)
        {
            EnsureLoaded();
            int index = (int)bus;
            float clamped = AudioMixCore.Clamp01(value, AudioMixCore.DefaultFor(bus));
            if (Mathf.Approximately(_buses[index], clamped)) return;

            _buses[index] = clamped;
            PlayerPrefs.SetFloat(AudioMixCore.PrefKey(bus), clamped);
            PlayerPrefs.Save();
            Publish("bus=" + bus + " value=" + clamped.ToString("F2"));
        }

        /// <summary>The level a source should play at, given the volume its author asked for.</summary>
        public static float Effective(AudioBus bus, float authoredVolume)
        {
            EnsureLoaded();
            return AudioMixCore.Effective(authoredVolume, _buses[(int)bus], _master);
        }

        /// <summary>Back to the shipped mix. Used by the settings surface's reset.</summary>
        public static void ResetToDefaults()
        {
            _loaded = true;
            _master = AudioMixCore.DefaultMaster;
            PlayerPrefs.SetFloat(AudioMixCore.MasterPrefKey, _master);
            for (int i = 0; i < _buses.Length; i++)
            {
                var bus = (AudioBus)i;
                _buses[i] = AudioMixCore.DefaultFor(bus);
                PlayerPrefs.SetFloat(AudioMixCore.PrefKey(bus), _buses[i]);
            }
            PlayerPrefs.Save();
            Publish("reset=true");
        }

        private static void Set(ref float field, string key, float value, float fallback)
        {
            float clamped = AudioMixCore.Clamp01(value, fallback);
            if (Mathf.Approximately(field, clamped)) return;
            field = clamped;
            PlayerPrefs.SetFloat(key, clamped);
            PlayerPrefs.Save();
            Publish("master value=" + clamped.ToString("F2"));
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            _master = AudioMixCore.Clamp01(
                PlayerPrefs.GetFloat(AudioMixCore.MasterPrefKey, AudioMixCore.DefaultMaster),
                AudioMixCore.DefaultMaster);

            for (int i = 0; i < _buses.Length; i++)
            {
                var bus = (AudioBus)i;
                float stored = PlayerPrefs.GetFloat(AudioMixCore.PrefKey(bus), AudioMixCore.DefaultFor(bus));
                _buses[i] = AudioMixCore.Clamp01(stored, AudioMixCore.DefaultFor(bus));
            }
        }

        private static void Publish(string detail)
        {
            Debug.Log("ZIPTIDE: AUDIO_MIX " + detail);

            var handler = Changed;
            if (handler == null) return;

            // One bad listener must not stop the rest of the game hearing about a volume change.
            var subscribers = handler.GetInvocationList();
            for (int i = 0; i < subscribers.Length; i++)
            {
                try { ((Action)subscribers[i])(); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: AUDIO_MIX_LISTENER_FAIL error=" + ex.Message);
                }
            }
        }
    }
}
