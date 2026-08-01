using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The shipyard crane's hook, creeping (HANGAR_AND_COUPLER_PASS §2.3). Thin translator over
    /// <see cref="CraneHookCore"/> — this file owns transforms and nothing else, so the two numbers
    /// that decide whether it reads as machinery live somewhere a test can reach them.
    ///
    /// No allocations, no lookups, no physics. It is a sine on two transforms, which is the entire
    /// budget a decorative loop deserves on a Quest.
    /// </summary>
    [DisallowMultipleComponent]
    public class CraneHookRuntime : MonoBehaviour
    {
        [SerializeField] private Transform _hook;
        [SerializeField] private Transform _cable;
        [SerializeField] private float _minDrop = CraneHookCore.DefaultMinDrop;
        [SerializeField] private float _maxDrop = CraneHookCore.DefaultMaxDrop;
        [SerializeField] private float _period = CraneHookCore.DefaultPeriod;

        private float _cableThickness = 0.08f;
        private float _hookHeight = 0.6f;

        /// <summary>Public Init, never reflection — the locked contract for runtime wiring.</summary>
        public void Init(Transform hook, Transform cable, float minDrop, float maxDrop, float period)
        {
            _hook = hook;
            _cable = cable;
            _minDrop = minDrop;
            _maxDrop = maxDrop;
            _period = period;
            if (_cable != null) _cableThickness = _cable.localScale.x;
            if (_hook != null) _hookHeight = _hook.localScale.y;
        }

        private void Update()
        {
            float drop = CraneHookCore.DropAt(Time.time, _minDrop, _maxDrop, _period);

            if (_hook != null)
            {
                Vector3 p = _hook.localPosition;
                p.y = -drop - _hookHeight * 0.5f;
                _hook.localPosition = p;
            }

            if (_cable == null) return;

            // The cable stretches. A fixed-length one detaches from the jib or from the hook, and a
            // floating cable end is more distracting than no motion at all.
            CraneHookCore.CableSpan(drop, out float centre, out float length);
            _cable.localPosition = new Vector3(0f, centre, 0f);
            _cable.localScale = new Vector3(_cableThickness, length, _cableThickness);
        }
    }
}
