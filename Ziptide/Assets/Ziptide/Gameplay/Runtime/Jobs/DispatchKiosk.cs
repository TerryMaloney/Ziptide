using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// XR interactable kiosk to accept/start a job. Assign JobDirector or it will be found at runtime.
    /// </summary>
    public class DispatchKiosk : MonoBehaviour
    {
        public static readonly Vector3 HowToLocalPosition = new Vector3(-0.75f, 1.25f, 0.02f);
        public const float HowToCharacterSize = 0.009f;
        public const int HowToFontSize = 64;

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
            go.transform.localPosition = HowToLocalPosition;

            var tm = go.AddComponent<TextMesh>();
            tm.text = "CONTRACT KIOSK\n1  SELECT TO ACCEPT\n2  FOLLOW MARKERS\n3  COMPLETE - GET PAID";
            tm.characterSize = HowToCharacterSize;
            tm.fontSize = HowToFontSize;
            tm.lineSpacing = 0.9f;
            tm.fontStyle = FontStyle.Bold;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(0.85f, 0.95f, 1f);

            // Legacy TextMesh is read from local -Z. The plaque owns its facing so the same kiosk
            // authoring works in W000 and ToxicCity without scene-specific Euler guesses.
            go.AddComponent<KioskHowToSignRuntime>();
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

    /// <summary>
    /// Keeps the compact kiosk plaque readable from the active player camera. TextMesh reads from -Z,
    /// so +Z points away from the viewer. This component only owns plaque facing; the kiosk owns content
    /// and placement.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    [DisallowMultipleComponent]
    public sealed class KioskHowToSignRuntime : MonoBehaviour
    {
        private Camera _camera;

        private void LateUpdate()
        {
            if (_camera == null || !_camera.isActiveAndEnabled)
            {
                _camera = Camera.main;
                if (_camera == null)
                    _camera = Object.FindFirstObjectByType<Camera>();
            }
            if (_camera == null) return;

            Vector3 awayFromViewer = transform.position - _camera.transform.position;
            if (awayFromViewer.sqrMagnitude < 0.000001f) return;
            transform.rotation = Quaternion.LookRotation(awayFromViewer.normalized, Vector3.up);
        }
    }
}
