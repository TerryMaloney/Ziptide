using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE AIR HAS A SOUND — the ambience half of ADAPTIVE_AUDIO (board 5.5), sibling to
    /// AudioDirector (which owns MUSIC clips; this owns the procedural biome bed). Auto-ensured at
    /// boot (the DevMenu idiom). On every world load it derives the biome spec (BiomeAmbience) and
    /// crossfades three loop-exact synthesized layers — wind, hum, rumble — plus a one-shot
    /// scheduler for drips (caves/cisterns) and alien chirps (living exteriors). Zero assets, zero
    /// scene edits: every buffer is deterministic math the EditMode tests already pinned.
    /// Total cost: 4 AudioSources on one persistent object, ~1MB of generated clips per world.
    /// </summary>
    public class AmbienceDirector : MonoBehaviour
    {
        private const int SampleRate = 22050;
        private const float LoopSeconds = 6f;
        private const float CrossfadeSeconds = 2.5f;
        private const float MasterVolume = 0.5f;   // a bed, never a lead

        private static AmbienceDirector _instance;

        private AudioSource _wind, _hum, _rumble, _oneShots;
        private AmbienceSpec _spec;
        private string _currentBiome;
        private System.Random _rng;
        private float _nextDripAt = float.MaxValue, _nextChirpAt = float.MaxValue;
        private readonly List<AudioClip> _dripClips = new List<AudioClip>();
        private readonly List<AudioClip> _chirpClips = new List<AudioClip>();
        private readonly List<AudioClip> _ownedLoops = new List<AudioClip>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (_instance != null) return;
            var go = new GameObject("__AmbienceDirector");
            DontDestroyOnLoad(go);
            go.AddComponent<AmbienceDirector>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            _wind = NewSource(); _hum = NewSource(); _rumble = NewSource();
            _oneShots = NewSource(); _oneShots.loop = false;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Apply(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            if (_instance == this) { SceneManager.sceneLoaded -= OnSceneLoaded; _instance = null; }
        }

        private AudioSource NewSource()
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.spatialBlend = 0f;
            src.playOnAwake = false;
            src.loop = true;
            src.volume = 0f;
            return src;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Additive world loads fire for _Boot too; the WORLD scene names win.
            if (scene.name == Ziptide.Core.ZiptideConstants.SceneBoot) return;
            Apply(scene.name);
        }

        private void Apply(string sceneName)
        {
            string biome = BiomeAmbience.BiomeForScene(sceneName);
            if (biome == _currentBiome) return;
            _currentBiome = biome;
            _spec = BiomeAmbience.ForScene(sceneName);
            int seed = biome.GetHashCode();
            _rng = new System.Random(seed);

            foreach (var c in _ownedLoops) if (c != null) Destroy(c);
            _ownedLoops.Clear();
            SwapLoop(_wind, BuildLoop("amb_wind_" + biome, b => AmbienceSynth.FillWindLoop(b, seed, _spec.WindLevel, _spec.WindGustiness, SampleRate)), _spec.WindLevel > 0f ? MasterVolume : 0f);
            SwapLoop(_hum, BuildLoop("amb_hum_" + biome, b => AmbienceSynth.FillHumLoop(b, _spec.HumHz, _spec.HumLevel, SampleRate)), _spec.HumLevel > 0f ? MasterVolume : 0f);
            SwapLoop(_rumble, BuildLoop("amb_rumble_" + biome, b => AmbienceSynth.FillRumbleLoop(b, seed, _spec.RumbleLevel, SampleRate)), _spec.RumbleLevel > 0f ? MasterVolume : 0f);

            BuildOneShotBanks(seed);
            ScheduleNext(ref _nextDripAt, _spec.DripDensity);
            ScheduleNext(ref _nextChirpAt, _spec.ChirpDensity);
            Debug.Log("ZIPTIDE: AMBIENCE biome=" + biome + " wind=" + _spec.WindLevel.ToString("F2") +
                      " hum=" + _spec.HumHz + "Hz drips=" + _spec.DripDensity + "/m chirps=" + _spec.ChirpDensity + "/m");
        }

        private AudioClip BuildLoop(string name, System.Action<float[]> fill)
        {
            var data = new float[(int)(SampleRate * LoopSeconds)];
            fill(data);
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            _ownedLoops.Add(clip);
            return clip;
        }

        private void SwapLoop(AudioSource src, AudioClip clip, float targetVolume)
        {
            StartCoroutine(CrossfadeTo(src, clip, targetVolume));
        }

        private System.Collections.IEnumerator CrossfadeTo(AudioSource src, AudioClip clip, float target)
        {
            float t = 0f, start = src.volume;
            while (t < CrossfadeSeconds * 0.5f)   // fade the old bed out…
            {
                t += Time.unscaledDeltaTime;
                src.volume = Mathf.Lerp(start, 0f, t / (CrossfadeSeconds * 0.5f));
                yield return null;
            }
            src.Stop();
            src.clip = clip;
            if (target <= 0f) yield break;
            src.Play();
            t = 0f;
            while (t < CrossfadeSeconds * 0.5f)   // …and the new one in
            {
                t += Time.unscaledDeltaTime;
                src.volume = Mathf.Lerp(0f, target, t / (CrossfadeSeconds * 0.5f));
                yield return null;
            }
            src.volume = target;
        }

        private void BuildOneShotBanks(int seed)
        {
            foreach (var c in _dripClips) if (c != null) Destroy(c);
            foreach (var c in _chirpClips) if (c != null) Destroy(c);
            _dripClips.Clear(); _chirpClips.Clear();
            for (int v = 0; v < 3; v++)
            {
                if (_spec.DripDensity > 0f)
                {
                    var data = new float[(int)(SampleRate * 0.5f)];
                    AmbienceSynth.FillDrip(data, seed + v, SampleRate);
                    var clip = AudioClip.Create("amb_drip_" + v, data.Length, 1, SampleRate, false);
                    clip.SetData(data, 0);
                    _dripClips.Add(clip);
                }
                if (_spec.ChirpDensity > 0f)
                {
                    var data = new float[(int)(SampleRate * 0.6f)];
                    AmbienceSynth.FillChirp(data, seed + v, _spec.ChirpPitchHz, SampleRate);
                    var clip = AudioClip.Create("amb_chirp_" + v, data.Length, 1, SampleRate, false);
                    clip.SetData(data, 0);
                    _chirpClips.Add(clip);
                }
            }
        }

        private void ScheduleNext(ref float nextAt, float perMinute)
        {
            if (perMinute <= 0f || _rng == null) { nextAt = float.MaxValue; return; }
            // Poisson-ish spacing around the density: 0.4×–1.8× the mean interval.
            float mean = 60f / perMinute;
            nextAt = Time.time + mean * (0.4f + 1.4f * (float)_rng.NextDouble());
        }

        private void Update()
        {
            if (Time.time >= _nextDripAt && _dripClips.Count > 0)
            {
                PlayOneShot(_dripClips[_rng.Next(_dripClips.Count)], 0.5f + 0.5f * (float)_rng.NextDouble());
                ScheduleNext(ref _nextDripAt, _spec.DripDensity);
            }
            if (Time.time >= _nextChirpAt && _chirpClips.Count > 0)
            {
                PlayOneShot(_chirpClips[_rng.Next(_chirpClips.Count)], 0.6f + 0.6f * (float)_rng.NextDouble());
                ScheduleNext(ref _nextChirpAt, _spec.ChirpDensity);
            }
        }

        private void PlayOneShot(AudioClip clip, float pitch)
        {
            _oneShots.pitch = pitch;
            _oneShots.PlayOneShot(clip, MasterVolume * 0.7f);
        }
    }
}
