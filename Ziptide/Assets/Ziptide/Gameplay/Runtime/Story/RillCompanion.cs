using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// RILL — the companion drone (GAME_PLAN M1; spec MASTER_BUILD_PLAN §5). Lives on the persistent
    /// rig (ensured by <see cref="PlayerRigPersistence"/>) so it crosses scenes automatically. Delivers
    /// SUBTITLE lines from the authored <see cref="RillLineLibrary"/> (Resources/Story/RillLines):
    /// on world entry (scene name) and on story-flag grants (1s profile poll — flags are granted by
    /// job/world completion, so a poll is plenty). A small hovering orb near the left shoulder gives the
    /// voice a body; its glow follows <see cref="RillState"/>. VO clips slot into the same lines at the
    /// M6 audio pass. Logs ZIPTIDE: RILL_LINE per delivery.
    /// </summary>
    public class RillCompanion : MonoBehaviour
    {
        private const float LineSeconds = 5f;
        private const float PollSeconds = 1f;
        private const string SaidFlagPrefix = "RILL_SAID_";

        private RillLineLibrary _library;
        private Transform _cam;
        private GameObject _orb;
        private Renderer _orbRenderer;
        private Material _orbMat;
        private TextMesh _text;

        private readonly Queue<RillLine> _pending = new Queue<RillLine>();
        private readonly List<RillLine> _scratch = new List<RillLine>();
        private readonly HashSet<string> _knownFlags = new HashSet<string>();
        private bool _flagsPrimed;
        private float _lineTimer;
        private float _pollTimer;

        private static readonly Color DormantColor = new Color(0.25f, 0.45f, 0.65f);
        private static readonly Color StirringColor = new Color(0.30f, 0.80f, 0.95f);
        private static readonly Color RememberingColor = new Color(0.55f, 0.85f, 0.75f);
        private static readonly Color LateColor = new Color(0.85f, 0.80f, 0.45f);

        private void Awake()
        {
            _library = Resources.Load<RillLineLibrary>("Story/RillLines");
            if (_library == null)
                Debug.LogWarning("ZIPTIDE: RILL_LINES_MISSING Resources/Story/RillLines not found");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnqueueMatching(RillTrigger.WorldEnter, scene.name);
        }

        private void Update()
        {
            EnsureVisuals();
            PollFlags();
            DriveSubtitle();
            DriveOrb();
        }

        // ── Line selection ───────────────────────────────────────────────────

        private void EnqueueMatching(RillTrigger trigger, string key)
        {
            if (_library == null) return;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;

            _scratch.Clear();
            _library.Collect(trigger, key, _scratch);
            for (int i = 0; i < _scratch.Count; i++)
            {
                var line = _scratch[i];
                if (line.once && profile != null && profile.HasFlag(SaidFlagPrefix + line.id)) continue;
                if (line.once && profile != null) profile.SetFlag(SaidFlagPrefix + line.id);
                _pending.Enqueue(line);
            }
        }

        private void PollFlags()
        {
            _pollTimer -= Time.deltaTime;
            if (_pollTimer > 0f) return;
            _pollTimer = PollSeconds;

            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || profile.flags == null) return;

            if (!_flagsPrimed)
            {
                // First sight of the profile: treat existing flags as history, not news — a returning
                // save shouldn't replay every reaction line at once (once-latches also guard this).
                _flagsPrimed = true;
                for (int i = 0; i < profile.flags.Count; i++) _knownFlags.Add(profile.flags[i]);
                return;
            }

            for (int i = 0; i < profile.flags.Count; i++)
            {
                string flag = profile.flags[i];
                if (flag == null || flag.StartsWith(SaidFlagPrefix) || !_knownFlags.Add(flag)) continue;
                EnqueueMatching(RillTrigger.FlagSet, flag);
            }
        }

        // ── Delivery ─────────────────────────────────────────────────────────

        private void DriveSubtitle()
        {
            if (_text == null) return;

            if (_lineTimer > 0f)
            {
                _lineTimer -= Time.deltaTime;
                if (_lineTimer <= 0f) _text.text = "";
            }

            if (_lineTimer <= 0f && _pending.Count > 0)
            {
                var line = _pending.Dequeue();
                _text.text = "RILL: " + line.text;
                _lineTimer = LineSeconds + line.text.Length * 0.02f; // a touch longer for long lines
                if (line.voClip != null)
                    AudioSource.PlayClipAtPoint(line.voClip, _orb != null ? _orb.transform.position : transform.position, 0.9f);
                Debug.Log("ZIPTIDE: RILL_LINE id=" + line.id);
            }
        }

        // ── Visuals (self-built; TextMesh per project convention — no TMP) ───

        private void EnsureVisuals()
        {
            if (_cam == null)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam == null) cam = Camera.main;
                if (cam != null) _cam = cam.transform;
                if (_cam == null) return;
            }

            if (_orb == null)
            {
                _orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _orb.name = "__RillOrb";
                var col = _orb.GetComponent<Collider>();
                if (col != null) Destroy(col);
                _orb.transform.localScale = Vector3.one * 0.07f;
                _orbRenderer = _orb.GetComponent<Renderer>();
                if (_orbRenderer != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Unlit");
                    if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader != null)
                    {
                        _orbMat = new Material(shader);
                        _orbRenderer.material = _orbMat;
                        _orbRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }

            if (_text == null)
            {
                var go = new GameObject("__RillSubtitle");
                _text = go.AddComponent<TextMesh>();
                _text.characterSize = 0.012f;
                _text.fontSize = 64;
                _text.anchor = TextAnchor.MiddleCenter;
                _text.alignment = TextAlignment.Center;
                _text.color = new Color(0.75f, 0.92f, 1f);
                _text.text = "";
            }
        }

        private void DriveOrb()
        {
            if (_cam == null) return;

            if (_orb != null)
            {
                // Hover near the left shoulder with a small idle bob; drift, don't snap (it's alive).
                Vector3 target = _cam.position + _cam.forward * 0.55f - _cam.right * 0.38f
                               + _cam.up * (-0.02f + Mathf.Sin(Time.time * 1.7f) * 0.02f);
                _orb.transform.position = Vector3.Lerp(_orb.transform.position, target, Time.deltaTime * 4f);

                if (_orbMat != null)
                {
                    var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                    Color c = StateColor(RillState.Compute(profile));
                    float pulse = _lineTimer > 0f ? 0.75f + 0.25f * Mathf.Sin(Time.time * 9f) : 0.55f; // brighter while speaking
                    c *= pulse;
                    if (_orbMat.HasProperty("_BaseColor")) _orbMat.SetColor("_BaseColor", c);
                    else _orbMat.color = c;
                }
            }

            if (_text != null)
            {
                // Subtitle low-center, above the credits readout, billboarded.
                _text.transform.position = _cam.position + _cam.forward * 0.9f - _cam.up * 0.27f;
                _text.transform.rotation = Quaternion.LookRotation(_text.transform.position - _cam.position);
            }
        }

        private static Color StateColor(RillMemoryState state)
        {
            switch (state)
            {
                case RillMemoryState.Dormant: return DormantColor;
                case RillMemoryState.Stirring: return StirringColor;
                case RillMemoryState.Remembering: return RememberingColor;
                default: return LateColor;
            }
        }
    }
}
