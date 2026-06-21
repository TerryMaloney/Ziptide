using UnityEngine;
using TMPro;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// World-space UI that shows current job title and step checklist. Uses TextMeshPro on a world-space Canvas.
    /// </summary>
    public class ObjectiveBoard : MonoBehaviour
    {
        [Tooltip("Optional: assign or will be found by name.")]
        [SerializeField] private JobDirector jobDirector;

        private TextMeshProUGUI _tmp;

        public void Bind(JobDirector director)
        {
            jobDirector = director;
            RefreshText();
        }

        private void Awake()
        {
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
                RefreshText();
            }
        }

        private void OnDestroy()
        {
            if (jobDirector != null && jobDirector.Runtime != null)
                jobDirector.Runtime.StepChanged -= OnStepChanged;
        }

        public void RefreshText()
        {
            if (_tmp == null) _tmp = GetComponentInChildren<TextMeshProUGUI>(true);
            if (_tmp == null) return;

            if (jobDirector == null || jobDirector.Runtime == null || jobDirector.Runtime.Definition == null)
            {
                _tmp.text = "No active job.\nUse the kiosk to start.";
                return;
            }

            var r = jobDirector.Runtime;
            string title = r.Definition.title;
            string step = r.StepText;
            if (r.IsComplete)
                _tmp.text = title + "\n\nComplete!";
            else
                _tmp.text = title + "\n\n" + step;
        }

        private void OnStepChanged()
        {
            RefreshText();
        }

        private static TextMeshProUGUI CreateWorldSpaceText(Transform parent)
        {
            var canvasGo = new GameObject("ObjectiveCanvas");
            canvasGo.transform.SetParent(parent, false);
            canvasGo.transform.localPosition = Vector3.zero;
            canvasGo.transform.localRotation = Quaternion.identity;
            // ~1m x 0.5m readable board. The old 2x1 @ 0.01 scale was ~2cm wide, so text overflowed to
            // "NO ACTI..." (the on-screen garbage Terry saw). Bigger rect + small scale fixes it.
            canvasGo.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            // Adding a Canvas auto-creates the RectTransform; AddComponent<RectTransform> would
            // return null (a GameObject can't hold two Transform-type components), causing the
            // NRE on the next line. Use the existing one.
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
            tmp.text = "No active job.";
            return tmp;
        }
    }
}
