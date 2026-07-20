using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Lightweight deterministic motion for generated ambient markers. It owns only its children and
    /// never moves colliders, gameplay roots or the player. Authored by the improvement compiler.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WorldAmbientMotionRuntime : MonoBehaviour
    {
        [SerializeField, Range(0f, 0.35f)] private float bobAmplitude = 0.12f;
        [SerializeField, Range(0.1f, 3f)] private float bobSpeed = 0.65f;
        [SerializeField, Range(0f, 0.25f)] private float pulseAmplitude = 0.08f;
        [SerializeField] private int seed;

        private Transform[] _parts;
        private Vector3[] _basePositions;
        private Vector3[] _baseScales;

        public void Configure(float bob, float speed, float pulse, int deterministicSeed)
        {
            bobAmplitude = Mathf.Clamp(bob, 0f, 0.35f);
            bobSpeed = Mathf.Clamp(speed, 0.1f, 3f);
            pulseAmplitude = Mathf.Clamp(pulse, 0f, 0.25f);
            seed = deterministicSeed;
            Capture();
        }

        private void OnEnable() => Capture();

        private void Capture()
        {
            int count = transform.childCount;
            _parts = new Transform[count];
            _basePositions = new Vector3[count];
            _baseScales = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                Transform part = transform.GetChild(i);
                _parts[i] = part;
                _basePositions[i] = part.localPosition;
                _baseScales[i] = part.localScale;
            }
        }

        private void Update()
        {
            if (_parts == null || _parts.Length != transform.childCount) Capture();
            float now = Time.time * bobSpeed;
            for (int i = 0; i < _parts.Length; i++)
            {
                Transform part = _parts[i];
                if (part == null) continue;
                float phase = ((seed & 1023) * 0.013f) + i * 1.618f;
                float wave = Mathf.Sin(now + phase);
                part.localPosition = _basePositions[i] + Vector3.up * (wave * bobAmplitude);
                part.localScale = _baseScales[i] * (1f + wave * pulseAmplitude);
            }
        }

        private void OnDisable()
        {
            if (_parts == null) return;
            for (int i = 0; i < _parts.Length; i++)
            {
                if (_parts[i] == null) continue;
                _parts[i].localPosition = _basePositions[i];
                _parts[i].localScale = _baseScales[i];
            }
        }
    }
}
