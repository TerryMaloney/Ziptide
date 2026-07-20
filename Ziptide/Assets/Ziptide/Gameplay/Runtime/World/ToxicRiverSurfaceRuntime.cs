using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Lightweight Quest-safe motion for prebuilt toxic-river flow ribbons and bubbles. Geometry and
    /// materials are authored by the editor builder; runtime only shifts existing transforms.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ToxicRiverSurfaceRuntime : MonoBehaviour
    {
        [SerializeField] private bool flowAlongZ = true;
        [SerializeField] private float flowSpan = 10f;
        [SerializeField] private float flowSpeed = 0.65f;
        [SerializeField] private float waveAmplitude = 0.035f;

        private Transform[] _flow;
        private Transform[] _bubbles;
        private Vector3[] _flowBase;
        private Vector3[] _bubbleBase;
        private float _damagePulse;

        public void Configure(bool alongZ, float span)
        {
            flowAlongZ = alongZ;
            flowSpan = Mathf.Max(1f, span);
        }

        private void Awake()
        {
            CacheChildren();
        }

        private void CacheChildren()
        {
            var all = GetComponentsInChildren<Transform>(true);
            var flow = new System.Collections.Generic.List<Transform>();
            var bubbles = new System.Collections.Generic.List<Transform>();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] == transform) continue;
                if (all[i].name.StartsWith("Flow_")) flow.Add(all[i]);
                else if (all[i].name.StartsWith("Bubble_")) bubbles.Add(all[i]);
            }
            _flow = flow.ToArray();
            _bubbles = bubbles.ToArray();
            _flowBase = new Vector3[_flow.Length];
            _bubbleBase = new Vector3[_bubbles.Length];
            for (int i = 0; i < _flow.Length; i++) _flowBase[i] = _flow[i].localPosition;
            for (int i = 0; i < _bubbles.Length; i++) _bubbleBase[i] = _bubbles[i].localPosition;
        }

        public void PulseDamage()
        {
            _damagePulse = 1f;
        }

        private void Update()
        {
            if (_flow == null) CacheChildren();
            _damagePulse = Mathf.MoveTowards(_damagePulse, 0f, Time.deltaTime * 3.5f);
            float half = flowSpan * 0.5f;

            for (int i = 0; i < _flow.Length; i++)
            {
                Transform t = _flow[i];
                if (t == null) continue;
                Vector3 p = _flowBase[i];
                float offset = Mathf.Repeat(Time.time * flowSpeed + i * (flowSpan / Mathf.Max(1, _flow.Length)), flowSpan) - half;
                if (flowAlongZ) p.z = offset; else p.x = offset;
                p.y += Mathf.Sin(Time.time * 1.8f + i * 0.9f) * waveAmplitude + _damagePulse * 0.025f;
                t.localPosition = p;
            }

            for (int i = 0; i < _bubbles.Length; i++)
            {
                Transform t = _bubbles[i];
                if (t == null) continue;
                Vector3 p = _bubbleBase[i];
                p.y += Mathf.Sin(Time.time * (1.2f + i * 0.07f) + i) * (waveAmplitude * 1.6f);
                float scale = 0.82f + 0.18f * Mathf.Sin(Time.time * 2.1f + i * 0.6f) + _damagePulse * 0.20f;
                t.localPosition = p;
                t.localScale = Vector3.one * Mathf.Max(0.08f, scale);
            }
        }
    }
}
