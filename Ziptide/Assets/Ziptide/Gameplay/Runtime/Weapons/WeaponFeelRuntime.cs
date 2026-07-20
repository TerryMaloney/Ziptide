using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Presentation-only fire feedback shared by pistol and taser owners. It animates ForgeVisual rather
    /// than the item root, so aim, muzzle, collider, Rigidbody and XR attach truth never move. Definition
    /// values remain authoritative; WeaponFeelCore derives only a bounded tail and hit-confirm pulse.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WeaponFeelRuntime : MonoBehaviour
    {
        private WeaponFeelKind _kind;
        private WeaponFeelEnvelope _envelope;
        private string _weaponId = "weapon";
        private bool _configured;
        private bool _logged;
        private Transform _visual;
        private Vector3 _visualBasePosition;
        private Quaternion _visualBaseRotation;
        private Coroutine _recoilRoutine;
        private static AudioClip _ballisticClip;
        private static AudioClip _electricClip;

        public WeaponFeelEnvelope Envelope => _envelope;

        public void Configure(string weaponId, WeaponFeelKind kind, float amplitude,
            float duration, float recoilKick, float cadenceSeconds)
        {
            _weaponId = string.IsNullOrEmpty(weaponId) ? "weapon" : weaponId;
            _kind = kind;
            _envelope = WeaponFeelCore.Resolve(kind, amplitude, duration, recoilKick, cadenceSeconds);
            _configured = true;
        }

        public void Fire(XRBaseControllerInteractor hand, AudioSource source, AudioClip authoredClip)
        {
            if (!_configured) return;
            if (hand != null) StartCoroutine(FireHaptics(hand));
            PlayAudio(source, authoredClip);
            StartVisualKick();
            if (!_logged)
            {
                _logged = true;
                Debug.Log("ZIPTIDE: WEAPON_FEEL_READY id=" + _weaponId
                    + " kind=" + _kind
                    + " primary=" + _envelope.Primary.Amplitude.ToString("F2")
                    + " tail=" + _envelope.Tail.Amplitude.ToString("F2")
                    + " kick=" + _envelope.VisualKick.ToString("F3")
                    + " fallbackAudio=" + (authoredClip == null));
            }
        }

        public void ConfirmHit(XRBaseControllerInteractor hand)
        {
            if (!_configured || hand == null) return;
            hand.SendHapticImpulse(_envelope.HitConfirm.Amplitude, _envelope.HitConfirm.Duration);
        }

        private IEnumerator FireHaptics(XRBaseControllerInteractor hand)
        {
            hand.SendHapticImpulse(_envelope.Primary.Amplitude, _envelope.Primary.Duration);
            if (_envelope.Tail.Amplitude <= 0f) yield break;
            yield return new WaitForSeconds(_envelope.Tail.Delay);
            if (hand != null)
                hand.SendHapticImpulse(_envelope.Tail.Amplitude, _envelope.Tail.Duration);
        }

        private void StartVisualKick()
        {
            ResolveVisual();
            if (_visual == null || _envelope.VisualKick <= 0f) return;
            if (_recoilRoutine != null) StopCoroutine(_recoilRoutine);
            _visual.localPosition = _visualBasePosition;
            _visual.localRotation = _visualBaseRotation;
            _recoilRoutine = StartCoroutine(VisualKickSequence());
        }

        private IEnumerator VisualKickSequence()
        {
            float snapSeconds = Mathf.Min(0.035f, _envelope.VisualReturnSeconds * 0.30f);
            Vector3 kicked = _visualBasePosition + Vector3.back * _envelope.VisualKick;
            float t = 0f;
            while (_visual != null && t < snapSeconds)
            {
                t += Time.deltaTime;
                _visual.localPosition = Vector3.Lerp(_visualBasePosition, kicked,
                    snapSeconds > 0f ? Mathf.Clamp01(t / snapSeconds) : 1f);
                yield return null;
            }

            t = 0f;
            while (_visual != null && t < _envelope.VisualReturnSeconds)
            {
                t += Time.deltaTime;
                float p = _envelope.VisualReturnSeconds > 0f
                    ? Mathf.Clamp01(t / _envelope.VisualReturnSeconds) : 1f;
                float eased = 1f - (1f - p) * (1f - p);
                _visual.localPosition = Vector3.Lerp(kicked, _visualBasePosition, eased);
                yield return null;
            }
            RestoreVisual();
            _recoilRoutine = null;
        }

        private void ResolveVisual()
        {
            if (_visual != null) return;
            _visual = transform.Find("ForgeVisual");
            if (_visual == null) return;
            _visualBasePosition = _visual.localPosition;
            _visualBaseRotation = _visual.localRotation;
        }

        private void PlayAudio(AudioSource source, AudioClip authoredClip)
        {
            if (source == null) return;
            AudioClip clip = authoredClip != null ? authoredClip : GetFallbackClip(_kind);
            if (clip != null) source.PlayOneShot(clip);
        }

        private static AudioClip GetFallbackClip(WeaponFeelKind kind)
        {
            if (kind == WeaponFeelKind.Electric)
            {
                if (_electricClip == null) _electricClip = BuildElectricClip();
                return _electricClip;
            }
            if (_ballisticClip == null) _ballisticClip = BuildBallisticClip();
            return _ballisticClip;
        }

        private static AudioClip BuildBallisticClip()
        {
            const int sampleRate = 44100;
            const float duration = 0.115f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            uint noise = 0x1234ABCDu;
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float attack = Mathf.Clamp01(t / 0.0025f);
                float env = attack * Mathf.Exp(-t * 31f);
                noise = noise * 1664525u + 1013904223u;
                float n = ((noise >> 8) & 0xFFFFu) / 32768f - 1f;
                float body = Mathf.Sin(2f * Mathf.PI * 118f * t) * Mathf.Exp(-t * 24f);
                float crack = n * Mathf.Exp(-t * 78f);
                data[i] = Mathf.Clamp((body * 0.48f + crack * 0.34f) * env, -0.85f, 0.85f);
            }
            var clip = AudioClip.Create("Pistol_ProceduralBody", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildElectricClip()
        {
            const int sampleRate = 44100;
            const float duration = 0.17f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float p = t / duration;
                float frequency = Mathf.Lerp(780f, 210f, p);
                float carrier = Mathf.Sin(2f * Mathf.PI * frequency * t);
                float buzz = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * 92f * t));
                float env = Mathf.Sin(Mathf.PI * Mathf.Clamp01(p)) * Mathf.Exp(-t * 5f);
                data[i] = Mathf.Clamp((carrier * 0.42f + buzz * 0.11f) * env, -0.75f, 0.75f);
            }
            var clip = AudioClip.Create("Taser_ProceduralCharge", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private void OnDisable()
        {
            RestoreVisual();
            _recoilRoutine = null;
        }

        private void OnDestroy()
        {
            // Clips are shared across weapons. The last owner releases them so scene reloads and EditMode
            // fixture teardown cannot leak procedural native audio resources.
            WeaponFeelRuntime[] remaining = FindObjectsOfType<WeaponFeelRuntime>(true);
            if (remaining.Length > 1) return;
            DestroyClip(ref _ballisticClip);
            DestroyClip(ref _electricClip);
        }

        private static void DestroyClip(ref AudioClip clip)
        {
            if (clip == null) return;
            if (Application.isPlaying) Object.Destroy(clip);
            else Object.DestroyImmediate(clip);
            clip = null;
        }

        private void RestoreVisual()
        {
            if (_visual == null) return;
            _visual.localPosition = _visualBasePosition;
            _visual.localRotation = _visualBaseRotation;
        }
    }
}
