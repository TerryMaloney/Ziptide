using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// E5.3 FLORA — wind sway for scattered plants/props: 1–3 pivots (default: this transform)
    /// rock gently around their build-pose rotation with incommensurate two-axis sines (reads as
    /// air, never a metronome — same trick as the gait motor's Antenna role). A LOOK, never a
    /// stat: purely visual rotation, no physics, colliders untouched. The math lives in the pure
    /// <see cref="SwayRotation"/> so EditMode tests can pin it without a scene.
    /// </summary>
    public class ForgeSway : MonoBehaviour
    {
        [Tooltip("Peak lean in degrees — keep small; a plant sways, it doesn't wave.")]
        public float degrees = 4f;
        [Tooltip("Base sway frequency in Hz.")]
        public float hz = 0.4f;

        private Transform[] _pivots;
        private Quaternion[] _base;
        private int _seed;

        private void Awake()
        {
            if (_pivots == null) Bind(new[] { transform });
        }

        /// <summary>Optional: sway specific branch pivots (1–3) instead of the whole plant.
        /// Captures their current rotations as the rest pose.</summary>
        public void Bind(Transform[] pivots)
        {
            _pivots = pivots;
            _base = new Quaternion[pivots.Length];
            for (int i = 0; i < pivots.Length; i++)
                if (pivots[i] != null) _base[i] = pivots[i].localRotation;
            // Per-instance phase so a scattered field never sways in lockstep.
            _seed = GetInstanceID();
        }

        private void LateUpdate()
        {
            if (_pivots == null) return;
            for (int i = 0; i < _pivots.Length; i++)
                if (_pivots[i] != null)
                    _pivots[i].localRotation = _base[i] * SwayRotation(_seed + i * 37, Time.time, degrees, hz);
        }

        /// <summary>Pure sway pose: two-axis lean with incommensurate frequencies (1 : 1.37) and a
        /// seed-spread phase. Deterministic; |angle| stays within ~1.3 × degrees.</summary>
        public static Quaternion SwayRotation(int seed, float time, float degrees, float hz)
        {
            float phase = (seed & 1023) * 0.006135f; // ~[0, 2π) across instances
            float ax = degrees * Mathf.Sin(2f * Mathf.PI * hz * time + phase);
            float az = degrees * 0.7f * Mathf.Sin(2f * Mathf.PI * hz * 1.37f * time + phase * 1.7f);
            return Quaternion.AngleAxis(ax, Vector3.right) * Quaternion.AngleAxis(az, Vector3.forward);
        }
    }
}
