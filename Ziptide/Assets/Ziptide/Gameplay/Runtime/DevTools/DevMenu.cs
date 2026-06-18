#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// In-VR developer menu — a summonable world-space panel listing every world (from
    /// <see cref="DevWorldManifest"/>) with a button to warp there via <see cref="DevWarp"/>. Lets us
    /// jump around on the headset, not just in the editor. Dev-only (compiled out of shipping builds);
    /// self-bootstraps, so no scene setup needed.
    ///
    /// Summon: both controllers' secondary buttons (Y + B) together, or F2 in the editor.
    /// v1 is world-level (default spawn); per-marker jumps are in the editor Warp Window already.
    /// </summary>
    public class DevMenu : MonoBehaviour
    {
        private GameObject _canvasGo;
        private bool _visible;
        private bool _comboWasDown;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (FindObjectOfType<DevMenu>() != null) return;
            var go = new GameObject("__DevMenu");
            DontDestroyOnLoad(go);
            go.AddComponent<DevMenu>();
            Debug.Log("ZIPTIDE: DEV_MENU ready (summon: hold both secondary buttons Y+B, or F2 in editor)");
        }

        private void Update()
        {
            if (SummonEdge()) Toggle();
        }

        // True on the frame the summon combo is first pressed.
        private bool SummonEdge()
        {
#if UNITY_EDITOR
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb != null && kb.f2Key.wasPressedThisFrame) return true;
#endif
            bool ly = false, ry = false;
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.secondaryButton, out ly);
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out ry);
            bool combo = ly && ry;
            bool edge = combo && !_comboWasDown;
            _comboWasDown = combo;
            return edge;
        }

        public void Toggle()
        {
            if (_visible) Hide(); else Show();
        }

        public void Show()
        {
            // Rebuild fresh each time so the canvas re-registers with the CURRENT scene's EventSystem
            // after a warp (fixes "clickable only once" — the post-travel UI raycast went stale).
            if (_canvasGo != null) Destroy(_canvasGo);
            BuildCanvas();
            PositionInFront();
            _canvasGo.SetActive(true);
            _visible = true;
            Debug.Log("ZIPTIDE: DEV_MENU shown");
        }

        public void Hide()
        {
            if (_canvasGo != null) _canvasGo.SetActive(false);
            _visible = false;
        }

        private void PositionInFront()
        {
            var cam = Camera.main;
            if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
            if (cam == null) return;
            Vector3 fwd = cam.transform.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.001f) fwd = Vector3.forward;
            fwd.Normalize();
            _canvasGo.transform.position = cam.transform.position + fwd * 1.6f + Vector3.up * 0.0f;
            _canvasGo.transform.rotation = Quaternion.LookRotation(fwd, Vector3.up);
        }

        private void BuildCanvas()
        {
            var manifest = DevWorldManifest.Load();
            int count = manifest != null ? manifest.worlds.Count : 0;

            const float width = 700f;
            const float rowH = 90f;
            const float headerH = 90f;
            float height = headerH + Mathf.Max(1, count) * rowH + 40f;

            _canvasGo = new GameObject("DevMenuCanvas");
            _canvasGo.transform.SetParent(transform, false);
            var canvas = _canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            _canvasGo.AddComponent<TrackedDeviceGraphicRaycaster>();
            var canvasRt = canvas.GetComponent<RectTransform>();
            canvasRt.sizeDelta = new Vector2(width, height);
            _canvasGo.transform.localScale = Vector3.one * 0.0012f;

            AddPanel(canvasRt, new Color(0.05f, 0.06f, 0.08f, 0.92f));
            AddLabel(canvasRt, "DEV WARP  —  pick a world", 0f, headerH, 34, new Color(0.6f, 0.85f, 1f));

            float y = -headerH;
            if (manifest == null || count == 0)
            {
                AddLabel(canvasRt, "No worlds. Run Ziptide > Dev >\nRebuild Dev World Manifest.", y, rowH, 24, Color.yellow);
            }
            else
            {
                foreach (var w in manifest.worlds)
                {
                    string scene = w.sceneName;
                    string label = string.IsNullOrEmpty(w.displayName) ? scene : w.displayName;
                    AddButton(canvasRt, label, y, rowH - 14f, new Color(0.13f, 0.16f, 0.2f),
                        () => { DevWarp.WarpToScene(scene); Hide(); });
                    y -= rowH;
                }
            }

            AddButton(canvasRt, "Close", y, rowH - 14f, new Color(0.25f, 0.1f, 0.1f), Hide);
        }

        private static void AddPanel(RectTransform parent, Color color)
        {
            var go = new GameObject("Panel", typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            Stretch(rt);
            go.GetComponent<Image>().color = color;
        }

        private static void AddLabel(RectTransform parent, string text, float yTop, float h, int size, Color color)
        {
            var go = new GameObject("Label", typeof(TextMeshProUGUI));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            TopRow(rt, yTop, h);
            var t = go.GetComponent<TextMeshProUGUI>();
            t.font = TMP_Settings.defaultFontAsset;
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = TextAlignmentOptions.Center;
            t.enableWordWrapping = true;
        }

        private static void AddButton(RectTransform parent, string label, float yTop, float h, Color bg, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject("Btn_" + label, typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            TopRow(rt, yTop - 7f, h);
            go.GetComponent<Image>().color = bg;
            go.GetComponent<Button>().onClick.AddListener(onClick);

            var labelGo = new GameObject("Text", typeof(TextMeshProUGUI));
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.SetParent(rt, false);
            Stretch(lrt);
            var t = labelGo.GetComponent<TextMeshProUGUI>();
            t.font = TMP_Settings.defaultFontAsset;
            t.text = label;
            t.fontSize = 30;
            t.color = Color.white;
            t.alignment = TextAlignmentOptions.Center;
        }

        // Anchors a rect to the top, full-width, at a downward offset yTop with height h.
        private static void TopRow(RectTransform rt, float yTop, float h)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.offsetMin = new Vector2(20f, 0f);
            rt.offsetMax = new Vector2(-20f, 0f);
            rt.anchoredPosition = new Vector2(0f, yTop);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, h);
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
#endif
