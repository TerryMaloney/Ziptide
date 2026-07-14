#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// THE one developer warp interface (DS-02, DEVICE_STABILIZATION_FORENSIC_PLAN). Built from the
    /// idiom PROVEN to render + click on the headset: primitive cube tiles + <see cref="TextMesh"/> +
    /// <see cref="XRSimpleInteractable"/> (identical to travel doors and the arena board). The TMP
    /// world-space <see cref="DevMenu"/> canvas — which rendered dead/flickering on-device — is
    /// retired from runtime bootstrap; this board absorbed its ownership semantics instead:
    ///
    /// - SUMMONED, never ambient: hold both controllers above the forehead for 2 s
    ///   (<see cref="DevMenuGesture"/>), F2 in the editor, or the ADB <see cref="DevAccessGate"/>
    ///   backup. No more auto-spawned board beside every scene spawn.
    /// - DISMISSIBLE: the gesture toggles, and the board carries a red CLOSE tile.
    /// - STILL, not orbiting: position + facing are fixed at Show time (re-summon to re-center) —
    ///   the old per-frame billboard is gone (it also mirrored every label, see below).
    /// - READABLE: label rotations come from <see cref="WorldLabelFacing"/>, the one facing
    ///   contract. The old billboard pointed board +Z at the camera while TextMesh reads from its
    ///   −Z side — the DS-03 mirror. Geometry keeps its +Z front; each label is yaw-flipped so its
    ///   readable face meets the viewer.
    ///
    /// Persists via a self-bootstrapped manager (dev builds only) — the ONLY
    /// RuntimeInitializeOnLoadMethod bootstrap in DevTools (a source-scan test enforces the
    /// singleton). Tiles call <see cref="TravelCoordinator.TravelTo"/>.
    /// Logs ZIPTIDE: DEV_WARP_BOARD / DEV_WARP_TO.
    /// </summary>
    public class DevWarpBoard : MonoBehaviour
    {
        private const int Columns = 3;
        private const float TileW = 0.62f, TileH = 0.17f, GapX = 0.06f, GapY = 0.06f;
        private const float SummonDistance = 1.6f;
        private const float AccessPollSeconds = 0.25f;

        private static readonly Color PanelColor = new Color(0.08f, 0.10f, 0.13f);
        private static readonly Color TileColor = new Color(0.16f, 0.30f, 0.38f);
        private static readonly Color TileHover = new Color(0.26f, 0.48f, 0.60f);
        private static readonly Color CloseColor = new Color(0.45f, 0.12f, 0.12f);
        private static readonly Color CloseHover = new Color(0.65f, 0.20f, 0.20f);
        private static readonly Color HeaderColor = new Color(0.60f, 0.85f, 1f);
        private static readonly Color LabelColor = new Color(0.90f, 0.95f, 1f);

        private readonly DevMenuGesture _gesture = new DevMenuGesture();
        private GameObject _board;
        private float _nextAccessPollAt;

        public bool Visible => _board != null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindObjectOfType<DevWarpBoard>() != null) return;
            var go = new GameObject("__DevWarpBoard");
            DontDestroyOnLoad(go);
            go.AddComponent<DevWarpBoard>();
            Debug.Log("ZIPTIDE: DEV_WARP_BOARD ready (summon: both controllers above forehead 2s; F2 in editor)");
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        // A scene change invalidates an open board (it belongs to the outgoing scene's space) —
        // close it; the player re-summons where they land. No ambient re-spawn (DS-02).
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Hide();

        private void Update()
        {
#if UNITY_EDITOR
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb != null && kb.f2Key.wasPressedThisFrame) Toggle();
#else
            if (_gesture.Tick(Time.unscaledDeltaTime))
            {
                Toggle();
                Debug.Log("ZIPTIDE: DEV_WARP_BOARD gesture_toggle visible=" + Visible);
            }

            if (Time.unscaledTime >= _nextAccessPollAt)
            {
                _nextAccessPollAt = Time.unscaledTime + AccessPollSeconds;
                // Computer-side ADB backup. The headset gesture needs no unlock.
                if (DevAccessGate.TryConsumeOpenRequest()) Show();
            }
#endif
        }

        public void Toggle()
        {
            if (Visible) Hide(); else Show();
        }

        public void Hide()
        {
            if (_board == null) return;
            Destroy(_board);
            _board = null;
            Debug.Log("ZIPTIDE: DEV_WARP_BOARD hidden");
        }

        public void Show()
        {
            Hide();

            var cam = Camera.main;
            if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
            if (cam == null)
            {
                Debug.LogWarning("ZIPTIDE: DEV_WARP_BOARD no_camera");
                return;
            }

            var manifest = DevWorldManifest.Load();
            var entries = Dedupe(manifest);
            if (entries.Count == 0)
            {
                Debug.LogWarning("ZIPTIDE: DEV_WARP_BOARD no_manifest_worlds");
                return;
            }

            // Fixed pose at summon time: SummonDistance in front of the head, geometry front (+Z,
            // where tiles + labels live) toward the viewer, upright. Never updated per-frame — the
            // old follow-the-player billboard is exactly what orbited badly on-device.
            Vector3 fwd = cam.transform.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.001f) fwd = Vector3.forward;
            fwd.Normalize();

            _board = new GameObject("DevWarpBoard");
            // Root local y=1.5 is the panel centre — drop the root so the panel meets eye height.
            _board.transform.position = cam.transform.position + fwd * SummonDistance
                + Vector3.up * (0.1f - 1.5f);
            _board.transform.rotation = Quaternion.LookRotation(-fwd, Vector3.up);

            Build(_board.transform, entries);
            Debug.Log("ZIPTIDE: DEV_WARP_BOARD shown tiles=" + entries.Count);
        }

        private static List<DevWorldManifest.Entry> Dedupe(DevWorldManifest manifest)
        {
            var list = new List<DevWorldManifest.Entry>();
            var seen = new HashSet<string>();
            if (manifest != null && manifest.worlds != null)
                foreach (var w in manifest.worlds)
                    if (w != null && !string.IsNullOrEmpty(w.sceneName) && seen.Add(w.sceneName))
                        list.Add(w);
            return list;
        }

        private void Build(Transform root, List<DevWorldManifest.Entry> entries)
        {
            int rows = Mathf.CeilToInt(entries.Count / (float)Columns) + 1; // + the CLOSE row
            float w = Columns * (TileW + GapX) + GapX;
            float h = rows * (TileH + GapY) + GapY + 0.34f;

            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "Panel";
            panel.transform.SetParent(root, false);
            panel.transform.localPosition = new Vector3(0f, 1.5f, -0.05f); // behind the tiles (away from viewer)
            panel.transform.localScale = new Vector3(w, h, 0.05f);
            var pcol = panel.GetComponent<Collider>(); if (pcol != null) pcol.enabled = false;
            Paint(panel, PanelColor);

            MakeLabel("TEST WARP  —  tap a world", new Vector3(0f, 1.5f + h / 2f + 0.1f, 0f), 0.02f, HeaderColor, root);

            float x0 = -(Columns - 1) * (TileW + GapX) * 0.5f;
            float y0 = 1.5f + h / 2f - 0.34f;
            for (int i = 0; i < entries.Count; i++)
            {
                int col = i % Columns, rowIx = i / Columns;
                var e = entries[i];
                string scene = e.sceneName;
                string label = string.IsNullOrEmpty(e.displayName) ? scene : e.displayName;
                var pos = new Vector3(x0 + col * (TileW + GapX), y0 - rowIx * (TileH + GapY), 0f);
                MakeTile(label, pos, root, TileColor, TileHover, () =>
                {
                    Debug.Log("ZIPTIDE: DEV_WARP_TO dest=" + scene);
                    Hide();
                    TravelCoordinator.TravelTo(scene);
                });
            }

            // DS-02: dismissal must live ON the board too, not only in the gesture.
            int closeRow = Mathf.CeilToInt(entries.Count / (float)Columns);
            var closePos = new Vector3(0f, y0 - closeRow * (TileH + GapY), 0f);
            MakeTile("CLOSE", closePos, root, CloseColor, CloseHover, Hide);
        }

        // ── Construction (primitives + TextMesh + XRSimpleInteractable — the device-proven idiom) ──
        private void MakeTile(string label, Vector3 localPos, Transform root, Color color, Color hover,
            System.Action onSelect)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Tile_" + label.Replace(' ', '_');
            go.transform.SetParent(root, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = new Vector3(TileW, TileH, 0.06f);
            Paint(go, color);

            var interactable = go.AddComponent<XRSimpleInteractable>();
            var mgr = FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            else StartCoroutine(BindManagerLater(interactable));
            interactable.selectEntered.AddListener(_ => onSelect());
            interactable.hoverEntered.AddListener(_ => Paint(go, hover));
            interactable.hoverExited.AddListener(_ => Paint(go, color));

            MakeLabel(label, localPos + new Vector3(0f, 0f, 0.05f), 0.009f, LabelColor, root); // on the +Z (viewer) side
        }

        private static IEnumerator BindManagerLater(XRSimpleInteractable interactable)
        {
            for (int i = 0; i < 20; i++)
            {
                yield return null;
                if (interactable == null) yield break;
                var mgr = FindObjectOfType<XRInteractionManager>();
                if (mgr != null) { interactable.interactionManager = mgr; yield break; }
            }
        }

        private void MakeLabel(string text, Vector3 localPos, float size, Color color, Transform root)
        {
            var go = new GameObject("Label_" + text.Replace(' ', '_'));
            go.transform.SetParent(root, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = Vector3.one;
            // DS-03: the viewer stands on the board's +Z side (its geometry front), and a TextMesh
            // reads from its −Z side — so every label turns via THE facing contract, never a guess.
            go.transform.localRotation = WorldLabelFacing.FaceViewer(
                localPos, localPos + Vector3.forward, yawOnly: false);
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color;
        }

        private static void Paint(GameObject go, Color c)
        {
            ItemFactory.ApplyURPColor(go, c);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
