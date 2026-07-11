using System.Collections;
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
        private Coroutine _transitionRoutine;

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
            if (_instance != this) return;

            SceneManager.sceneLoaded -= OnSceneLoaded;
            CancelTransition();
            StopAndClear(_sourceA);
            StopAndClear(_sourceB);
            _instance = null;
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

            if (profile.clip == _currentClip && _active != null && _active.isPlaying)
                return;

            _currentClip = profile.clip;
            CancelTransition();

            AudioSource fadeOut = _active != null ? _active : _sourceA;
            AudioSource next = fadeOut == _sourceA ? _sourceB : _sourceA;
            StopAndClear(next);

            next.clip = profile.clip;
            next.loop = profile.loop;
            next.volume = 0f;
            next.Play();

            _active = next;
            _transitionRoutine = StartCoroutine(Crossfade(
                fadeOut,
                next,
                profile.volume,
                Mathf.Max(0f, profile.crossfadeSeconds)));
        }

        private IEnumerator Crossfade(
            AudioSource fadeOut,
            AudioSource fadeIn,
            float targetVolume,
            float duration)
        {
            float startOut = fadeOut != null ? fadeOut.volume : 0f;
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float ratio = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
                if (fadeOut != null) fadeOut.volume = Mathf.Lerp(startOut, 0f, ratio);
                if (fadeIn != null) fadeIn.volume = Mathf.Lerp(0f, targetVolume, ratio);
                yield return null;
            }

            StopAndClear(fadeOut);
            if (fadeIn != null) fadeIn.volume = targetVolume;
            _transitionRoutine = null;
        }

        private void FadeOut()
        {
            _currentClip = null;
            CancelTransition();

            AudioSource fadeOut = _active;
            AudioSource inactive = fadeOut == _sourceA ? _sourceB : _sourceA;
            StopAndClear(inactive);

            if (fadeOut != null && fadeOut.isPlaying)
                _transitionRoutine = StartCoroutine(FadeOutCoroutine(fadeOut, 1f));
            else
                StopAndClear(fadeOut);
        }

        private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
        {
            float start = source != null ? source.volume : 0f;
            float t = 0f;
            while (source != null && t < duration)
            {
                t += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(start, 0f,
                    duration > 0f ? Mathf.Clamp01(t / duration) : 1f);
                yield return null;
            }

            StopAndClear(source);
            _transitionRoutine = null;
        }

        private void CancelTransition()
        {
            if (_transitionRoutine == null) return;
            StopCoroutine(_transitionRoutine);
            _transitionRoutine = null;
        }

        private static void StopAndClear(AudioSource source)
        {
            if (source == null) return;
            source.Stop();
            source.volume = 0f;
            source.clip = null;
        }
    }
}
