#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Ziptide.Core;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// ⚠ RETIRED FROM RUNTIME (DS-02, DEVICE_STABILIZATION_FORENSIC_PLAN, 2026-07-14). This TMP
    /// world-space canvas rendered dead/flickering on the Quest (2026-07-06 device pass) — which is
    /// why the primitive-tile <see cref="DevWarpBoard"/> exists — and having BOTH self-bootstrap
    /// produced the duplicate competing menus Terry hit on-device. The board is now the ONE
    /// authoritative dev warp interface and owns the summon gesture, F2, and the ADB gate.
    ///
    /// This class keeps NO self-bootstrap and NO summon path. It is retained only as a manual
    /// diagnostic, and even manual/scene-authored use requires the explicit DevWarpBoard diagnostic
    /// exposure. Do not re-add a RuntimeInitializeOnLoadMethod here.
    /// </summary>
    public class DevMenu : MonoBehaviour
    {
        private const int PageSize = 6;

        private GameObject _canvasGo;
        private bool _visible;
        private int _page;

        private static bool ExposureAllowed =>
            RecoveryRuntimeGate.Allows(RecoveryFeatureId.DevWarpBoard);

        private void Awake()
        {
            if (!ExposureAllowed) enabled = false;
        }

        public void Toggle()
        {
            if (!ExposureAllowed) { Hide(); return; }
            if (_visible) Hide(); else Show();
        }

        public void Show()
        {
            if (!ExposureAllowed) { Hide(); return; }
            // Rebuild fresh each time so the canvas re-registers with the CURRENT scene's EventSystem
            // after a warp (fixes "clickable only once" — the post-travel UI raycast went stale).
            if (_canvasGo != null) Destroy(_canvasGo);
            EnsureUiSession();
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

        /// <summary>
        /// The EventSystem + XRUIInputModule live in each WORLD scene, but the rig's ray interactors
        /// persist in _Boot — after a warp destroys the old scene's module, the interactors can stay
        /// bound to the dead one (destruction-order dependent). That is the "menu clickable once"
        /// regression. Re-assert the UI session on every Show: guarantee a module exists (persistent
        /// fallback if a scene forgot one) and force every UI-enabled ray interactor to re-register
        /// with the CURRENT module by toggling enableUIInteraction.
        /// </summary>
        private void EnsureUiSession()
        {
            var module = FindObjectOfType<XRUIInputModule>();
            if (module == null)
            {
                var es = FindObjectOfType<EventSystem>();
                GameObject host = es != null ? es.gameObject : null;
                if (host == null)
                {
                    host = new GameObject("__DevMenuEventSystem");
                    host.transform.SetParent(transform, false); // rides our DontDestroyOnLoad object
                    host.AddComponent<EventSystem>();
                }
                module = host.AddComponent<XRUIInputModule>();
                Debug.Log("ZIPTIDE: MENU_UI fallback_module host=" + host.name);
            }

            int rebound = 0;
            foreach (var ray in FindObjectsOfType<XRRayInteractor>())
            {
                if (!ray.enableUIInteraction) continue;
                ray.enableUIInteraction = false;
                ray.enableUIInteraction = true;
                rebound++;
            }
            Debug.Log("ZIPTIDE: MENU_UI module=" + module.gameObject.name + " raysRebound=" + rebound);
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
            int pages = Mathf.Max(1, Mathf.CeilToInt(count / (float)PageSize));
            _page = Mathf.Clamp(_page, 0, pages - 1);
            int first = _page * PageSize;
            int onPage = count == 0 ? 0 : Mathf.Min(PageSize, count - first);

            const float width = 700f;
            const float rowH = 90f;
            const float headerH = 90f;
            float height = headerH + Mathf.Max(1, onPage) * rowH + rowH * 2f + 40f;

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
            AddLabel(canvasRt, "DEV WARP  —  page " + (_page + 1) + "/" + pages, 0f, headerH, 34, new Color(0.6f, 0.85f, 1f));

            float y = -headerH;
            if (manifest == null || count == 0)
            {
                AddLabel(canvasRt, "No worlds. Run Ziptide > Dev >\nRebuild Dev World Manifest.", y, rowH, 24, Color.yellow);
                y -= rowH;
            }
            else
            {
                for (int i = first; i < first + onPage; i++)
                {
                    var w = manifest.worlds[i];
                    string scene = w.sceneName;
                    string label = string.IsNullOrEmpty(w.displayName) ? scene : w.displayName;
                    AddButton(canvasRt, label, y, rowH - 14f, new Color(0.13f, 0.16f, 0.2f),
                        () => { DevWarp.WarpToScene(scene); Hide(); });
                    y -= rowH;
                }
            }

            AddButton(canvasRt, "< PREV", y, rowH - 14f, new Color(0.1f, 0.14f, 0.24f),
                () => { _page = (_page - 1 + pages) % pages; Show(); }, 0f, 0.48f);
            AddButton(canvasRt, "NEXT >", y, rowH - 14f, new Color(0.1f, 0.14f, 0.24f),
                () => { _page = (_page + 1) % pages; Show(); }, 0.52f, 1f);
            y -= rowH;

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
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = TextAlignmentOptions.Center;
            t.enableWordWrapping = true;
        }

        private static void AddButton(RectTransform parent, string label, float yTop, float h, Color bg,
            UnityEngine.Events.UnityAction onClick, float xMin = 0f, float xMax = 1f)
        {
            var go = new GameObject("Btn_" + label, typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            TopRow(rt, yTop - 7f, h, xMin, xMax);
            go.GetComponent<Image>().color = bg;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("ZIPTIDE: MENU_CLICK btn=" + label);
                onClick();
            });

            var labelGo = new GameObject("Text", typeof(TextMeshProUGUI));
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.SetParent(rt, false);
            Stretch(lrt);
            var t = labelGo.GetComponent<TextMeshProUGUI>();
            t.text = label;
            t.fontSize = 30;
            t.color = Color.white;
            t.alignment = TextAlignmentOptions.Center;
        }

        private static void TopRow(RectTransform rt, float yTop, float h, float xMin = 0f, float xMax = 1f)
        {
            rt.anchorMin = new Vector2(xMin, 1f);
            rt.anchorMax = new Vector2(xMax, 1f);
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
