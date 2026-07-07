#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// DEVICE-RELIABLE test warp (2026-07-06). The in-VR <see cref="DevMenu"/> is a TMP world-space
    /// canvas that renders as a dead/flickering panel on-device — which stranded Terry in the boot
    /// scene. This is its replacement, built from the PROVEN idiom that actually renders + clicks on
    /// the headset: primitive cube tiles + <see cref="TextMesh"/> + <see cref="XRSimpleInteractable"/>
    /// (identical to the travel doors and the arena match board). No TMP, no Canvas.
    ///
    /// A persistent manager self-bootstraps (dev builds only) and, on every scene load that has a
    /// player spawn marker, drops a physical board beside the spawn listing every world + arena from
    /// <see cref="DevWorldManifest"/>; tapping a tile calls <see cref="TravelCoordinator.TravelTo"/>.
    /// So from ANY world or arena you can jump anywhere for testing without the flaky menu.
    /// Logs ZIPTIDE: DEV_WARP_BOARD / DEV_WARP_TO.
    /// </summary>
    public class DevWarpBoard : MonoBehaviour
    {
        private const int Columns = 3;
        private const float TileW = 0.62f, TileH = 0.17f, GapX = 0.06f, GapY = 0.06f;

        private static readonly Color PanelColor = new Color(0.08f, 0.10f, 0.13f);
        private static readonly Color TileColor = new Color(0.16f, 0.30f, 0.38f);
        private static readonly Color TileHover = new Color(0.26f, 0.48f, 0.60f);
        private static readonly Color HeaderColor = new Color(0.60f, 0.85f, 1f);
        private static readonly Color LabelColor = new Color(0.90f, 0.95f, 1f);

        private GameObject _board;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindObjectOfType<DevWarpBoard>() != null) return;
            var go = new GameObject("__DevWarpBoard");
            DontDestroyOnLoad(go);
            go.AddComponent<DevWarpBoard>();
            Debug.Log("ZIPTIDE: DEV_WARP_BOARD ready (physical warp board — beside spawn in every scene)");
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        // Billboard the board to the player (yaw + a little pitch), kept upright, so the tiles read and
        // ray-select from wherever the player stands. Cheap: one transform per frame, only while a board exists.
        private void Update()
        {
            if (_board == null) return;
            var cam = Camera.main;
            if (cam == null) return;
            Vector3 toCam = cam.transform.position - _board.transform.position;
            if (toCam.sqrMagnitude < 0.0001f) return;
            // TextMesh reads correctly from its +Z side, so point the board's +Z at the camera. Our
            // layout puts the panel behind (-z) and the labels in front (+z), so nothing occludes the text.
            _board.transform.rotation = Quaternion.LookRotation(toCam.normalized, Vector3.up);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_board != null) Destroy(_board);
            StartCoroutine(BuildWhenReady(scene));
        }

        // Wait a frame so the scene's spawn marker + the persistent XRInteractionManager are live.
        private IEnumerator BuildWhenReady(Scene scene)
        {
            yield return null;

            var marker = FindSpawnMarker();
            if (marker == null) yield break; // _Boot and content-less scenes have no player spawn — skip

            var manifest = DevWorldManifest.Load();
            var entries = Dedupe(manifest);
            if (entries.Count == 0) yield break;

            // Anchor the board a couple metres off to the player's left of the spawn (out of the
            // forward vista). Orientation is handled by the billboard in Update so the tiles are always
            // readable + tappable regardless of which way the player faces — no facing-convention guesswork.
            Vector3 pos = marker.position + marker.right * -2.4f + Vector3.up * 0.1f;
            _board = new GameObject("DevWarpBoard");
            _board.transform.position = pos;

            Build(_board.transform, entries);
            Debug.Log("ZIPTIDE: DEV_WARP_BOARD scene=" + scene.name + " tiles=" + entries.Count);
        }

        private static Transform FindSpawnMarker()
        {
            SpawnMarkerRuntime fallback = null;
            foreach (var m in FindObjectsOfType<SpawnMarkerRuntime>())
            {
                if (m == null) continue;
                fallback = m;
                if (m.markerId == "player") return m.transform;
            }
            return fallback != null ? fallback.transform : null;
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
            int rows = Mathf.CeilToInt(entries.Count / (float)Columns);
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
                MakeTile(label, pos, root, () =>
                {
                    Debug.Log("ZIPTIDE: DEV_WARP_TO dest=" + scene);
                    TravelCoordinator.TravelTo(scene);
                });
            }
        }

        // ── Construction (mirrors ArenaLobbyBoard: primitives + TextMesh + XRSimpleInteractable) ──
        private void MakeTile(string label, Vector3 localPos, Transform root, System.Action onSelect)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Tile_" + label.Replace(' ', '_');
            go.transform.SetParent(root, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = new Vector3(TileW, TileH, 0.06f);
            Paint(go, TileColor);

            var interactable = go.AddComponent<XRSimpleInteractable>();
            var mgr = FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            else StartCoroutine(BindManagerLater(interactable));
            interactable.selectEntered.AddListener(_ => onSelect());
            interactable.hoverEntered.AddListener(_ => Paint(go, TileHover));
            interactable.hoverExited.AddListener(_ => Paint(go, TileColor));

            MakeLabel(label, localPos + new Vector3(0f, 0f, 0.05f), 0.009f, LabelColor, root); // in front of the tile (toward viewer)
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
