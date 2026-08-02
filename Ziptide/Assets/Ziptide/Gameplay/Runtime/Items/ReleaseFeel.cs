using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Makes letting go of an item FEEL acknowledged (Quality Bar P0.4 — device report: released items
    /// "just drop", which reads as a bug). Three touches, zero gameplay change:
    /// 1. THROW RESCUE — the holster kinematic-restore listener runs in the same selectExited frame as
    ///    XRGrab's detach, and a body that was kinematic at detach time swallows the throw velocity.
    ///    We sample the item's own motion while held (it tracks the hand under VelocityTracking) and
    ///    re-apply that velocity on release if the rigidbody came out dead.
    /// 2. A brief highlight pulse on the renderer so the release visibly registers.
    /// 3. First-ever release grants <see cref="ZiptideFlags.FIRST_RELEASE"/> — RILL explains the
    ///    holster (line authored in RillLineAuthor).
    /// Added by ItemFactory to everything it builds with a grab interactable. Must be added AFTER
    /// RestorePhysicsOnRelease so its selectExited listener runs after the physics restore.
    /// </summary>
    public class ReleaseFeel : MonoBehaviour
    {
        private const int SampleCount = 6;
        private const float MaxThrowSpeed = 8f;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private Rigidbody _rb;
        private Renderer _renderer;
        private bool _held;
        private int _sampleHead;
        private readonly Vector3[] _positions = new Vector3[SampleCount];
        private readonly float[] _times = new float[SampleCount];
        private Coroutine _pulse;

        private void OnEnable()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _rb = GetComponent<Rigidbody>();
            _renderer = GetComponentInChildren<Renderer>();
            if (_grab == null) { enabled = false; return; }
            _grab.selectEntered.AddListener(OnGrabbed);
            _grab.selectExited.AddListener(OnReleased);
        }

        private void OnDisable()
        {
            if (_grab == null) return;
            _grab.selectEntered.RemoveListener(OnGrabbed);
            _grab.selectExited.RemoveListener(OnReleased);
        }

        private void Update()
        {
            if (!_held) return;
            _positions[_sampleHead % SampleCount] = transform.position;
            _times[_sampleHead % SampleCount] = Time.time;
            _sampleHead++;
        }

        private void OnGrabbed(SelectEnterEventArgs _)
        {
            _held = true;
            _sampleHead = 0;
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            _held = false;

            // Holster sockets also fire selectExited→selectEntered handoffs; only style a TRUE drop
            // (nothing selecting us anymore).
            if (_grab != null && _grab.isSelected) return;

            bool rescued = TryRescueThrow(out float speed);
            PulseHighlight();
            GrantFirstReleaseHint();
            Debug.Log("ZIPTIDE: ITEM_RELEASE item=" + gameObject.name +
                      " vel=" + speed.ToString("F2") + (rescued ? " rescued=1" : ""));
        }

        private bool TryRescueThrow(out float appliedSpeed)
        {
            appliedSpeed = _rb != null ? _rb.linearVelocity.magnitude : 0f;
            if (_rb == null || _rb.isKinematic || _sampleHead < 2) return false;

            int newest = (_sampleHead - 1) % SampleCount;
            int oldest = _sampleHead >= SampleCount ? _sampleHead % SampleCount : 0;
            float dt = _times[newest] - _times[oldest];
            if (dt <= 0.0001f) return false;

            Vector3 sampled = (_positions[newest] - _positions[oldest]) / dt;
            if (sampled.magnitude > MaxThrowSpeed) sampled = sampled.normalized * MaxThrowSpeed;

            // Only intervene when XRGrab's own detach clearly failed (dead body, live hand motion).
            if (_rb.linearVelocity.sqrMagnitude >= 0.25f || sampled.sqrMagnitude <= 0.5f) return false;
            _rb.linearVelocity = sampled;
            appliedSpeed = sampled.magnitude;
            return true;
        }

        private void PulseHighlight()
        {
            if (_renderer == null || _renderer.material == null) return;
            if (_pulse != null) StopCoroutine(_pulse);
            _pulse = StartCoroutine(PulseRoutine());
        }

        private IEnumerator PulseRoutine()
        {
            var mat = _renderer.material; // instance material (guns already get per-instance mats)
            string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : (mat.HasProperty("_Color") ? "_Color" : null);
            if (prop == null) yield break;

            Color baseColor = mat.GetColor(prop);
            const float seconds = 0.35f;
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                float glow = 1f + 0.7f * (1f - t / seconds);
                mat.SetColor(prop, baseColor * glow);
                yield return null;
            }
            mat.SetColor(prop, baseColor);
            _pulse = null;
        }

        private static void GrantFirstReleaseHint()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || profile.HasFlag(ZiptideFlags.FIRST_RELEASE)) return;
            profile.SetFlag(ZiptideFlags.FIRST_RELEASE);
        }
    }
}
