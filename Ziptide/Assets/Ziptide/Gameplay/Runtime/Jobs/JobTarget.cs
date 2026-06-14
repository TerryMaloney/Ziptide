using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Place on a target with TargetRuntime. When the target is hit, notifies JobDirector for ShootTargetsCountStep.
    /// </summary>
    [RequireComponent(typeof(TargetRuntime))]
    public class JobTarget : MonoBehaviour
    {
        private TargetRuntime _target;
        private JobDirector _director;

        private void Awake()
        {
            _target = GetComponent<TargetRuntime>();
        }

        private void Start()
        {
            if (_director == null)
                _director = FindObjectOfType<JobDirector>();
            if (_target != null && _target.OnHit != null)
                _target.OnHit.AddListener(OnHit);
        }

        private void OnDestroy()
        {
            if (_target != null && _target.OnHit != null)
                _target.OnHit.RemoveListener(OnHit);
        }

        public void Bind(JobDirector director)
        {
            _director = director;
        }

        private void OnHit()
        {
            if (_director != null)
                _director.ReportTargetHit();
        }
    }
}
