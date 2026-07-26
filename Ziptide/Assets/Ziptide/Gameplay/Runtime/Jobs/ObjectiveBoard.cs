using System.Collections;
using UnityEngine;
using TMPro;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// World-space contract board bound to the canonical JobRuntime. It shows the current job and step,
    /// and owns presentation-only feedback derived from that state: a short world-space objective card
    /// when the step changes and a one-time completion card/pulse. It owns no quest progress or rewards.
    /// </summary>
    public class ObjectiveBoard : MonoBehaviour
    {
        [Tooltip("Optional: assign or will be found by name.")]
        [SerializeField] private JobDirector jobDirector;

        private TextMeshProUGUI _tmp;
        private string _announcedJobId;
        private int _announcedStep = -1;
        private bool _completionPresented;
        private Coroutine _boardPulse;
        private Coroutine _toastRoutine;
        private GameObject _toastRoot;
        private TextMesh _toastText;
        private Renderer _toastPanel;
        private Vector3 _baseScale;

        public void Bind(JobDirector director)
        {
            jobDirector = director;
            RefreshText();
        }

        private void Awake()
        {
            _baseScale = transform.localScale;
            _tmp = GetComponentInChildren<TextMeshProUGUI>(true);
            if (_tmp == null)
                _tmp = CreateWorldSpaceText(transform);
        }

        private void Start()
        {
            if (jobDirector == null)
                jobDirector = FindObjectOfType<JobDirector>();
            if (jobDirector != null)
            {
                jobDirector.Runtime.StepChanged += OnStepChanged;
                jobDirector.Runtime.JobCompleted += OnJobCompleted;
                RefreshText();
            }
        }

        private void OnDestroy()
        {
            if (jobDirector != null && jobDirector.Runtime != null)
            {
                jobDirector.Runtime.StepChanged -= OnStepChanged;
                jobDirector.Runtime.JobCompleted -= OnJobCompleted;
            }
            if (_toastRoot != null) Destroy(_toastRoot);
        }

        public void RefreshText()
        {
            if (_tmp == null) _tmp = GetComponentInChildren<TextMeshProUGUI>(true);
            if (_tmp == null) return;

            if (jobDirector == null || jobDirector.Runtime == null || jobDirector.Runtime.Definition == null)
            {
                _tmp.color = new Color(0.82f, 0.86f, 0.90f);
                _tmp.text = "CONTRACT BOARD\n\nNo active contract\nSelect one at Dispatch";
                _announcedJobId = null;
                _announcedStep = -1;
                _completionPresented = false;
                return;
            }

            JobRuntime runtime = jobDirector.Runtime;
            string title = string.IsNullOrEmpty(runtime.Definition.title)
                ? "CONTRACT" : runtime.Definition.title;
            int totalSteps = runtime.Definition.steps != null ? runtime.Definition.steps.Count : 0;
            int shownStep = totalSteps > 0 ? Mathf.Clamp(runtime.CurrentStepIndex + 1, 1, totalSteps) : 0;

            if (runtime.IsComplete)
            {
                _tmp.color = new Color(0.42f, 1f, 0.68f);
                _tmp.text = "CONTRACT COMPLETE\n" + title + "\n\nAll objectives complete";
                if (!_completionPresented)
                {
                    _completionPresented = true;
                    ShowToast("CONTRACT COMPLETE", title + "\nROUTE COMPLETE", true);
                    StartBoardPulse();
                }
            }
            else
            {
                _tmp.color = new Color(0.80f, 0.93f, 1f);
                _tmp.text = title + "\nCONTRACT " + shownStep + "/" + totalSteps + "\n\n" + runtime.StepText;

                string jobId = runtime.Definition.jobId ?? runtime.Definition.name;
                if (_announcedJobId != jobId || _announcedStep != runtime.CurrentStepIndex)
                {
                    _announcedJobId = jobId;
                    _announcedStep = runtime.CurrentStepIndex;
                    _completionPresented = false;
                    ShowToast("NEW OBJECTIVE", runtime.StepText, false);
                }
            }

            Debug.Log("ZIPTIDE: REPAIR_TRACE hop=board director=" + jobDirector.GetInstanceID()
                + " step=" + runtime.CurrentStepIndex + " complete=" + runtime.IsComplete
                + " text=\"" + runtime.StepText + "\"");
        }

        private void OnStepChanged()
        {
            RefreshText();
        }

        private void OnJobCompleted()
        {
            RefreshText();
        }

        private void ShowToast(string header, string body, bool complete)
        {
            Camera cam = Camera.main;
            if (cam == null) return;
            BuildToastIfNeeded();
            if (_toastRoot == null || _toastText == null) return;

            Vector3 flat = cam.transform.forward;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.0001f) flat = Vector3.forward;
            flat.Normalize();
            _toastRoot.transform.SetPositionAndRotation(
                cam.transform.position + flat * 0.95f + Vector3.down * 0.14f,
                Quaternion.LookRotation(flat, Vector3.up));

            _toastText.text = header + "\n" + body;
            Tint(_toastPanel, complete ? new Color(0.10f, 0.42f, 0.28f) : new Color(0.08f, 0.24f, 0.38f));
            _toastRoot.SetActive(true);
            if (_toastRoutine != null) StopCoroutine(_toastRoutine);
            _toastRoutine = StartCoroutine(HideToastAfter(complete ? 3.0f : 2.2f));
            Debug.Log("ZIPTIDE: CONTRACT_TOAST kind=" + (complete ? "complete" : "objective")
                + " step=" + (jobDirector != null ? jobDirector.Runtime.CurrentStepIndex : -1));
        }

        private void BuildToastIfNeeded()
        {
            if (_toastRoot != null) return;
            _toastRoot = new GameObject("__CONTRACT_TOAST");

            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "Panel";
            panel.transform.SetParent(_toastRoot.transform, false);
            panel.transform.localScale = new Vector3(0.78f, 0.24f, 0.035f);
            Collider collider = panel.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            _toastPanel = panel.GetComponent<Renderer>();
            Paint(_toastPanel, new Color(0.08f, 0.24f, 0.38f));

            GameObject text = new GameObject("Text");
            text.transform.SetParent(panel.transform, false);
            text.transform.localPosition = new Vector3(0f, 0f, -0.56f);
            text.transform.localScale = new Vector3(1f / 0.78f, 1f / 0.24f, 1f / 0.035f) * 0.16f;
            _toastText = text.AddComponent<TextMesh>();
            _toastText.characterSize = 0.045f;
            _toastText.fontSize = 56;
            _toastText.anchor = TextAnchor.MiddleCenter;
            _toastText.alignment = TextAlignment.Center;
            _toastText.color = new Color(0.94f, 0.98f, 1f);
            _toastRoot.SetActive(false);
        }

        private IEnumerator HideToastAfter(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            if (_toastRoot != null) _toastRoot.SetActive(false);
            _toastRoutine = null;
        }

        private void StartBoardPulse()
        {
            if (_boardPulse != null) StopCoroutine(_boardPulse);
            _boardPulse = StartCoroutine(BoardPulseRoutine());
        }

        private IEnumerator BoardPulseRoutine()
        {
            const float seconds = 0.42f;
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                float p = Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / seconds));
                transform.localScale = Vector3.Lerp(_baseScale, _baseScale * 1.07f, p);
                yield return null;
            }
            transform.localScale = _baseScale;
            _boardPulse = null;
        }

        private static TextMeshProUGUI CreateWorldSpaceText(Transform parent)
        {
            var canvasGo = new GameObject("ObjectiveCanvas");
            canvasGo.transform.SetParent(parent, false);
            canvasGo.transform.localPosition = Vector3.zero;
            canvasGo.transform.localRotation = Quaternion.identity;
            canvasGo.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            var rt = canvasGo.GetComponent<RectTransform>();
            if (rt == null) rt = canvasGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400f, 220f);

            var textGo = new GameObject("ObjectiveText");
            textGo.transform.SetParent(canvasGo.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector3.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 26;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.margin = new Vector4(12f, 8f, 12f, 8f);
            tmp.text = "CONTRACT BOARD\n\nNo active contract";
            return tmp;
        }

        private static void Paint(Renderer renderer, Color color)
        {
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            Material material = new Material(shader);
            renderer.sharedMaterial = material;
            Tint(renderer, color);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private static void Tint(Renderer renderer, Color color)
        {
            if (renderer == null || renderer.material == null) return;
            if (renderer.material.HasProperty("_BaseColor")) renderer.material.SetColor("_BaseColor", color);
            else if (renderer.material.HasProperty("_Color")) renderer.material.SetColor("_Color", color);
        }
    }
}
