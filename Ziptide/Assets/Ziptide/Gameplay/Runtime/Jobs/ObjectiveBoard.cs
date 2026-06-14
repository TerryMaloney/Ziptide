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
            canvasGo.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            var rt = canvasGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(2f, 1f);

            var textGo = new GameObject("ObjectiveText");
            textGo.transform.SetParent(canvasGo.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector3.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.text = "No job.";
            return tmp;
        }
    }
}
