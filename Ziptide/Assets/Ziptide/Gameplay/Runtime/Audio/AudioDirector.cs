using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// DontDestroyOnLoad singleton that manages background music with crossfade.
    /// Reads AudioProfile from the active scene's WorldRuntime/WorldPackDefinition.
    /// </summary>
    public class AudioDirector : MonoBehaviour
    {
        private static AudioDirector _instance;

        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioSource _active;
        private AudioClip _currentClip;
        private float _fadeTimer;
        private float _fadeDuration;
        private bool _fading;

        public static AudioDirector Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceB = gameObject.AddComponent<AudioSource>();
            ConfigureSource(_sourceA);
            ConfigureSource(_sourceB);
            _active = _sourceA;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _instance = null;
            }
        }

        private static void ConfigureSource(AudioSource src)
        {
            src.spatialBlend = 0f;
            src.playOnAwake = false;
            src.loop = true;
            src.volume = 0f;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var jd = FindObjectOfType<JobDirector>();
            WorldPackDefinition pack = jd != null ? jd.WorldPack : null;

            if (pack == null || pack.audioProfile == null || !pack.audioProfile.enabled)
            {
                FadeOut();
                return;
            }

            ApplyProfile(pack.audioProfile);
        }

        private void ApplyProfile(AudioProfile profile)
        {
            if (profile.clip == null)
            {
                Debug.LogWarning("ZIPTIDE: AUDIO_CLIP_MISSING on profile " + profile.name);
                FadeOut();
                return;
            }

            if (profile.clip == _currentClip && _active.isPlaying)
                return;

            _currentClip = profile.clip;

            var next = _active == _sourceA ? _sourceB : _sourceA;
            next.clip = profile.clip;
            next.loop = profile.loop;
            next.volume = 0f;
            next.Play();

            _fadeDuration = profile.crossfadeSeconds;
            _fadeTimer = 0f;
            _fading = true;

            _active = next;
            var targetVol = profile.volume;

            StartCoroutine(Crossfade(_active == _sourceA ? _sourceB : _sourceA, _active, targetVol));
        }

        private System.Collections.IEnumerator Crossfade(AudioSource fadeOut, AudioSource fadeIn, float targetVol)
        {
            float t = 0f;
            float startOut = fadeOut.volume;
            while (t < _fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float ratio = Mathf.Clamp01(t / _fadeDuration);
                fadeOut.volume = Mathf.Lerp(startOut, 0f, ratio);
                fadeIn.volume = Mathf.Lerp(0f, targetVol, ratio);
                yield return null;
            }
            fadeOut.Stop();
            fadeOut.volume = 0f;
            fadeIn.volume = targetVol;
            _fading = false;
        }

        private void FadeOut()
        {
            _currentClip = null;
            if (_active != null && _active.isPlaying)
            {
                StartCoroutine(FadeOutCoroutine(_active, 1f));
            }
        }

        private static System.Collections.IEnumerator FadeOutCoroutine(AudioSource src, float duration)
        {
            float start = src.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                src.volume = Mathf.Lerp(start, 0f, Mathf.Clamp01(t / duration));
                yield return null;
            }
            src.Stop();
            src.volume = 0f;
        }
    }
}
