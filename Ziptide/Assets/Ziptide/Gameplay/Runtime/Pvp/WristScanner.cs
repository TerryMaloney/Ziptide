using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Premium left-wrist scanner ("Pulse"). A diegetic forearm device: cover it with your right hand to
    /// charge (lens brightens, charge ring fills, haptics ramp), then it auto-fires a sonar PULSE — a
    /// shockwave + a holographic radar that unfolds above your wrist (reads targets through walls), a
    /// floating tag + distance on each target, an edge-of-vision chevron to snap your head the right way,
    /// then a 60s cooldown shown on the lens. Timing is the pure, tested <see cref="LocatorState"/>;
    /// generalized over <see cref="IScannable"/> so the campaign reuses it for nodes/loot/objectives.
    /// All spawned visuals are cleaned up on teardown (no leaks onto the persistent rig).
    /// </summary>
    public class WristScanner : MonoBehaviour
    {
        [Header("Gesture / range")]
        public float wristTouchDistance = 0.18f;
        public float scanRange = 30f;
        public float scanDuration = 6f;

        [Header("Feel (tune on-device)")]
        public float chargeHapticMax = 0.6f;
        public float pulseHapticAmp = 0.9f;
        public float pulseHapticDur = 0.12f;

        [Header("Audio (optional — assign clips for the full effect)")]
        public AudioClip chargeClip;
        public AudioClip pingClip;
        public AudioClip readyClip;

        private static readonly Color IdleColor = new Color(0.18f, 0.7f, 0.95f);
        private static readonly Color ChargeColor = new Color(0.4f, 1f, 1f);
        private static readonly Color CooldownColor = new Color(1f, 0.55f, 0.15f);
        private static readonly Color EnemyColor = new Color(1f, 0.35f, 0.30f);

        private readonly LocatorState _state = new LocatorState();

        private Transform _cam;
        private Transform _leftHand;
        private Transform _rightHand;
        private XRBaseControllerInteractor _leftInteractor;
        private XRBaseControllerInteractor _rightInteractor;

        private GameObject _bracer;       // parented to the left controller (persistent rig!) — must be cleaned up
        private Renderer _lens;
        private Transform _ring;
        private Renderer _ringRenderer;

        private HoloRadar _radar;
        private GameObject _edgeChevron;
        private Renderer _chevronRenderer;
        private readonly List<GameObject> _tags = new List<GameObject>();
        private readonly List<Transform> _targets = new List<Transform>();

        private float _scanActiveUntil;
        private bool _wasCharging;
        private bool _wasOnCooldown;
        private static readonly Dictionary<Color, Material> _mats = new Dictionary<Color, Material>();

        private void Start()
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _cam = rig.GetComponentInChildren<Camera>()?.transform;
            if (_cam == null && Camera.main != null) _cam = Camera.main.transform;
            FindHands();
            BuildBracer();
        }

        private void FindHands()
        {
            if (_cam == null) return;
            // Resolve hands from the CONTROLLERS (one per hand), not interactors — each hand has SEVERAL
            // interactors (direct + ray + teleport), so the old interactors[0]/[1] could pick two on the
            // SAME hand, leaving _rightHand on the left side and the cover-wrist gesture never firing.
            var controllers = FindObjectsOfType<ActionBasedController>();
            if (controllers == null || controllers.Length < 2) return;
            ActionBasedController left = null, right = null;
            float leftDot = float.MaxValue, rightDot = float.MinValue;
            foreach (var c in controllers)
            {
                if (c == null) continue;
                float d = Vector3.Dot(c.transform.position - _cam.position, _cam.right);
                if (d < leftDot) { leftDot = d; left = c; }   // most-left controller
                if (d > rightDot) { rightDot = d; right = c; } // most-right controller
            }
            if (left == null || right == null || left == right) return;
            _leftHand = left.transform;
            _rightHand = right.transform;
            // Haptics still go through an interactor under each controller (may be null — guarded at use).
            _leftInteractor = left.GetComponentInChildren<XRBaseControllerInteractor>();
            _rightInteractor = right.GetComponentInChildren<XRBaseControllerInteractor>();
            Debug.Log("ZIPTIDE: WRIST_HANDS left=" + (_leftHand != null) + " right=" + (_rightHand != null)
                + " leftInteractor=" + (_leftInteractor != null) + " rightInteractor=" + (_rightInteractor != null));
        }

        private void BuildBracer()
        {
            if (_bracer != null || _leftHand == null) return;

            _bracer = new GameObject("__WristScanner");
            _bracer.transform.SetParent(_leftHand, false);
            _bracer.transform.localPosition = new Vector3(0f, 0.02f, -0.12f); // sit further back on the forearm
            _bracer.transform.localRotation = Quaternion.identity;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body"; StripCollider(body);
            body.transform.SetParent(_bracer.transform, false);
            body.transform.localScale = new Vector3(0.045f, 0.015f, 0.05f);
            Paint(body, new Color(0.1f, 0.11f, 0.13f));

            var lens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lens.name = "Lens"; StripCollider(lens);
            lens.transform.SetParent(_bracer.transform, false);
            lens.transform.localScale = new Vector3(0.02f, 0.002f, 0.02f);
            lens.transform.localPosition = new Vector3(0f, 0.009f, 0f);
            Paint(lens, IdleColor);
            _lens = lens.GetComponent<Renderer>();

            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring"; StripCollider(ring);
            ring.transform.SetParent(_bracer.transform, false);
            ring.transform.localScale = new Vector3(0.03f, 0.001f, 0.03f);
            ring.transform.localPosition = new Vector3(0f, 0.008f, 0f);
            Paint(ring, IdleColor);
            _ring = ring.transform;
            _ringRenderer = ring.GetComponent<Renderer>();
        }

        private void Update()
        {
            if (_leftHand == null || _rightHand == null) { FindHands(); if (_leftHand == null) return; }
            if (_bracer == null) BuildBracer();
            if (_bracer == null) return;

            bool held = Vector3.Distance(_rightHand.position, _bracer.transform.position) <= wristTouchDistance;
            bool firedNow = _state.Tick(Time.time, Time.deltaTime, held);

            UpdateDeviceVisuals(held);

            if (firedNow) Pulse();

            // Maintain the active scan (radar/tags/chevron follow live targets), then fold it away.
            if (Time.time < _scanActiveUntil) UpdateActiveScan();
            else if (_radar != null || _tags.Count > 0) FoldScan();
        }

        private void UpdateDeviceVisuals(bool held)
        {
            bool charging = held && !_state.OnCooldown;

            if (charging)
            {
                if (!_wasCharging) PlayClip(chargeClip, _bracer.transform.position, 0.4f);
                float p = _state.HoldProgress;
                SetColor(_lens, Color.Lerp(IdleColor, ChargeColor, p));
                if (_ring != null) _ring.localScale = new Vector3(0.03f * (0.6f + 0.6f * p), 0.001f, 0.03f * (0.6f + 0.6f * p));
                SetColor(_ringRenderer, ChargeColor);
                if (_leftInteractor != null) _leftInteractor.SendHapticImpulse(chargeHapticMax * p, 0.03f);
            }
            else if (_state.OnCooldown)
            {
                float cd = _state.CooldownProgress(Time.time); // 0..1 -> 1 ready
                SetColor(_lens, Color.Lerp(CooldownColor, IdleColor, cd));
                SetColor(_ringRenderer, Color.Lerp(CooldownColor, IdleColor, cd));
                if (_ring != null) _ring.localScale = new Vector3(0.018f + 0.012f * cd, 0.001f, 0.018f + 0.012f * cd);
                _wasOnCooldown = true;
            }
            else
            {
                if (_wasOnCooldown) { _wasOnCooldown = false; PlayClip(readyClip, _bracer.transform.position, 0.5f); FlashReady(); }
                float breathe = 0.7f + 0.3f * Mathf.Sin(Time.time * 2f);
                SetColor(_lens, IdleColor * breathe);
                if (_ring != null) _ring.localScale = new Vector3(0.022f, 0.001f, 0.022f);
                SetColor(_ringRenderer, IdleColor * 0.5f);
            }
            _wasCharging = charging;
        }

        private void Pulse()
        {
            GatherTargets();

            // Haptic thump + sonar audio.
            if (_leftInteractor != null) _leftInteractor.SendHapticImpulse(pulseHapticAmp, pulseHapticDur);
            if (_rightInteractor != null) _rightInteractor.SendHapticImpulse(pulseHapticAmp, pulseHapticDur);
            PlayClip(pingClip, _bracer.transform.position, 0.7f);

            StartCoroutine(Shockwave(_bracer.transform.position));

            // Holographic radar above the wrist.
            if (_radar == null)
            {
                var go = new GameObject("__HoloRadar");
                go.transform.SetParent(_bracer.transform, false);
                go.transform.localPosition = new Vector3(0f, 0.16f, 0f);
                _radar = go.AddComponent<HoloRadar>();
                _radar.discRadius = 0.1f; // bigger, more readable once it unfolds
                _radar.Configure(_cam, scanRange);
            }
            _radar.gameObject.SetActive(true);
            _radar.SetTargets(_targets);

            BuildTags();
            EnsureChevron();

            _scanActiveUntil = Time.time + scanDuration;
            Debug.Log("ZIPTIDE: WRIST_SCAN_PULSE targets=" + _targets.Count);
        }

        private void GatherTargets()
        {
            _targets.Clear();
            if (_cam == null) return;
            var scannables = FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in scannables)
            {
                if (mb is IScannable s && s.ScanActive && s.ScanTransform != null)
                {
                    if (Vector3.Distance(s.ScanTransform.position, _cam.position) <= scanRange)
                        _targets.Add(s.ScanTransform);
                }
            }
        }

        private void UpdateActiveScan()
        {
            // Tags follow targets.
            for (int i = 0; i < _tags.Count; i++)
            {
                var tag = _tags[i];
                var tgt = i < _targets.Count ? _targets[i] : null;
                if (tag == null) continue;
                if (tgt == null) { tag.SetActive(false); continue; }
                tag.SetActive(true);
                tag.transform.position = tgt.position + Vector3.up * 1.3f;
                if (_cam != null) tag.transform.rotation = Quaternion.LookRotation(tag.transform.position - _cam.position);
            }

            UpdateChevron();
        }

        private void BuildTags()
        {
            foreach (var t in _tags) if (t != null) Destroy(t);
            _tags.Clear();
            for (int i = 0; i < _targets.Count; i++)
            {
                var tag = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tag.name = "__ScanTag_" + i; StripCollider(tag);
                tag.transform.localScale = new Vector3(0.18f, 0.18f, 0.02f);
                tag.transform.localRotation = Quaternion.Euler(0f, 0f, 45f); // diamond
                Paint(tag, EnemyColor);
                _tags.Add(tag);
            }
        }

        private void EnsureChevron()
        {
            if (_edgeChevron != null || _cam == null) return;
            _edgeChevron = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _edgeChevron.name = "__ScanChevron"; StripCollider(_edgeChevron);
            _edgeChevron.transform.SetParent(_cam, false);
            _edgeChevron.transform.localScale = new Vector3(0.03f, 0.03f, 0.005f);
            Paint(_edgeChevron, EnemyColor);
            _chevronRenderer = _edgeChevron.GetComponent<Renderer>();
            _edgeChevron.SetActive(false);
        }

        private void UpdateChevron()
        {
            if (_edgeChevron == null || _cam == null || _targets.Count == 0 || _targets[0] == null)
            { if (_edgeChevron != null) _edgeChevron.SetActive(false); return; }

            Vector3 vp = _cam.GetComponent<Camera>() != null
                ? _cam.GetComponent<Camera>().WorldToViewportPoint(_targets[0].position)
                : new Vector3(0.5f, 0.5f, 1f);
            Vector2 c = new Vector2(vp.x - 0.5f, vp.y - 0.5f);
            bool behind = vp.z < 0f;
            if (behind) c = -c;
            bool offscreen = behind || Mathf.Abs(c.x) > 0.42f || Mathf.Abs(c.y) > 0.42f;
            if (!offscreen) { _edgeChevron.SetActive(false); return; }

            _edgeChevron.SetActive(true);
            Vector2 ring = c.sqrMagnitude > 1e-4f ? c.normalized * 0.32f : new Vector2(0f, 0.32f);
            _edgeChevron.transform.localPosition = new Vector3(ring.x, ring.y, 0.5f);
            _edgeChevron.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(ring.y, ring.x) * Mathf.Rad2Deg + 45f);
        }

        private IEnumerator Shockwave(Vector3 pos)
        {
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "__ScanShockwave"; StripCollider(ring);
            ring.transform.position = pos;
            Paint(ring, ChargeColor);
            float t = 0f;
            const float dur = 0.5f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float s = Mathf.Lerp(0.1f, 2.2f, t / dur);
                ring.transform.localScale = new Vector3(s, 0.004f, s);
                yield return null;
            }
            Destroy(ring);
        }

        private void FoldScan()
        {
            if (_radar != null) _radar.gameObject.SetActive(false);
            foreach (var t in _tags) if (t != null) Destroy(t);
            _tags.Clear();
            if (_edgeChevron != null) _edgeChevron.SetActive(false);
        }

        private void FlashReady()
        {
            if (_lens != null) SetColor(_lens, ChargeColor);
        }

        private void OnDestroy()
        {
            // Everything we spawned — including the bracer parented to the PERSISTENT controller and the
            // chevron parented to the persistent camera — must be torn down so nothing leaks across worlds.
            if (_bracer != null) Destroy(_bracer);
            if (_radar != null) Destroy(_radar.gameObject);
            if (_edgeChevron != null) Destroy(_edgeChevron);
            foreach (var t in _tags) if (t != null) Destroy(t);
            _tags.Clear();
        }

        private void PlayClip(AudioClip clip, Vector3 pos, float vol)
        {
            if (clip != null) AudioSource.PlayClipAtPoint(clip, pos, vol);
        }

        private static void StripCollider(GameObject go)
        {
            var c = go.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }

        private static void SetColor(Renderer r, Color c)
        {
            if (r == null) return;
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", c);
            else if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_Color")) r.material.color = c;
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
