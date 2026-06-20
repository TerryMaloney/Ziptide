using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A small holographic radar disc that hovers above the wrist after a scan pulse. It orients to the
    /// player's gaze (a stable compass), sweeps a rotating line, and shows each detected target as a
    /// real-bearing blip whose size/pulse scales with proximity. Because it lives on your arm, it reads
    /// targets THROUGH walls — that's the standout. Spawned + lifetime-managed by <see cref="WristScanner"/>.
    /// </summary>
    public class HoloRadar : MonoBehaviour
    {
        public float discRadius = 0.06f;
        public Color baseColor = new Color(0.20f, 0.85f, 1f);
        public Color enemyColor = new Color(1f, 0.35f, 0.30f);

        private Transform _head;
        private float _range = 30f;
        private Transform _sweep;
        private readonly List<Transform> _targets = new List<Transform>();
        private readonly List<GameObject> _blips = new List<GameObject>();
        private static readonly Dictionary<Color, Material> _mats = new Dictionary<Color, Material>();

        public void Configure(Transform head, float range)
        {
            _head = head;
            _range = Mathf.Max(1f, range);
            BuildDiscIfNeeded();
        }

        public void SetTargets(List<Transform> targets)
        {
            _targets.Clear();
            if (targets != null) _targets.AddRange(targets);
            RebuildBlips();
        }

        private void BuildDiscIfNeeded()
        {
            if (_sweep != null) return;

            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "Disc";
            StripCollider(disc);
            disc.transform.SetParent(transform, false);
            disc.transform.localScale = new Vector3(discRadius * 2f, 0.002f, discRadius * 2f);
            Paint(disc, new Color(baseColor.r, baseColor.g, baseColor.b, 0.25f));

            // Rotating sweep line.
            var sweep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sweep.name = "Sweep";
            StripCollider(sweep);
            sweep.transform.SetParent(transform, false);
            sweep.transform.localScale = new Vector3(0.004f, 0.004f, discRadius);
            sweep.transform.localPosition = new Vector3(0f, 0.002f, discRadius * 0.5f);
            Paint(sweep, baseColor);
            _sweep = sweep.transform;

            // "You" marker at center.
            var center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            center.name = "Self";
            StripCollider(center);
            center.transform.SetParent(transform, false);
            center.transform.localScale = Vector3.one * 0.008f;
            Paint(center, baseColor);
        }

        private void RebuildBlips()
        {
            foreach (var b in _blips) if (b != null) Destroy(b);
            _blips.Clear();
            for (int i = 0; i < _targets.Count; i++)
            {
                var blip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                blip.name = "Blip_" + i;
                StripCollider(blip);
                blip.transform.SetParent(transform, false);
                Paint(blip, enemyColor);
                _blips.Add(blip);
            }
        }

        private void Update()
        {
            if (_head == null) return;

            // Keep the disc a gaze-stable compass: +Z = where the player looks (flattened).
            Vector3 fwd = _head.forward; fwd.y = 0f;
            if (fwd.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(fwd.normalized, Vector3.up);

            if (_sweep != null)
                _sweep.RotateAround(transform.position, transform.up, 200f * Time.deltaTime);

            for (int i = 0; i < _blips.Count; i++)
            {
                var blip = _blips[i];
                var tgt = i < _targets.Count ? _targets[i] : null;
                if (blip == null) continue;
                if (tgt == null) { blip.SetActive(false); continue; }
                blip.SetActive(true);

                Vector3 delta = tgt.position - _head.position; delta.y = 0f;
                float dist = delta.magnitude;
                float t = Mathf.Clamp01(dist / _range);
                Vector3 dirLocal = transform.InverseTransformDirection(delta.normalized);
                Vector3 local = new Vector3(dirLocal.x, 0f, dirLocal.z).normalized * (t * discRadius);
                blip.transform.localPosition = new Vector3(local.x, 0.004f, local.z);

                // Closer = bigger + faster pulse.
                float pulse = 0.6f + 0.4f * Mathf.Sin(Time.time * Mathf.Lerp(10f, 3f, t));
                float size = Mathf.Lerp(0.018f, 0.009f, t) * pulse;
                blip.transform.localScale = Vector3.one * size;
            }
        }

        private static void StripCollider(GameObject go)
        {
            var c = go.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }

        private static void Paint(GameObject go, Color c)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            if (!_mats.TryGetValue(c, out var m) || m == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                m = new Material(shader);
                m.color = c;
                if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
                _mats[c] = m;
            }
            r.sharedMaterial = m;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
