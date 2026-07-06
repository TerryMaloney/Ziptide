using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// XR interactable kiosk to accept/start a job. Assign JobDirector or it will be found at runtime.
    /// </summary>
    public class DispatchKiosk : MonoBehaviour
    {
        [Tooltip("Job index in WorldPackDefinition.jobs to start when activated (0 = first job).")]
        [SerializeField] private int jobIndex = 0;

        private JobDirector _director;

        public void Bind(JobDirector director)
        {
            _director = director;
        }

        private void Start()
        {
            if (_director == null)
                _director = FindObjectOfType<JobDirector>();
            var interactable = GetComponent<XRSimpleInteractable>();
            if (interactable != null)
                interactable.selectEntered.AddListener(OnSelectEntered);

            // "Use the kiosk to start" is only fair if you can FIND the kiosk (Test Day 1) —
            // and fair only if it says HOW ("a menu somewhere that explains how the games work").
            ObjectiveBeacon.Attach(gameObject, new Color(0.3f, 0.85f, 0.95f));
            BuildHowToSign();
        }

        private void BuildHowToSign()
        {
            if (transform.Find("__HowToSign") != null) return;
            var go = new GameObject("__HowToSign");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, 2.1f, 0f);
            var tm = go.AddComponent<TextMesh>();
            tm.text = "CONTRACT KIOSK\n1. Point + select to accept the job\n2. Follow the objective markers\n3. Finish the steps - get paid";
            tm.characterSize = 0.012f; // characterSize x fontSize — the match-board lesson
            tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(0.85f, 0.95f, 1f);
        }

        private void OnDestroy()
        {
            var interactable = GetComponent<XRSimpleInteractable>();
            if (interactable != null)
                interactable.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (_director != null)
                _director.StartJobByIndex(jobIndex);
        }
    }
}
