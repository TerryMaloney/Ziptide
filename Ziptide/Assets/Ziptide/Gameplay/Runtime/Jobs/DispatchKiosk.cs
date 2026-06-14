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
