using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.3d / BUILDING_INTERIORS — interior proximity culling: an interior's renderers
    /// only draw when the player is close enough to possibly see them. The Quest-honest v1 of the
    /// portal/PVS finding (VR_TECHNIQUE_RESEARCH §2: per-frame occlusion is the CPU cost to avoid —
    /// so this checks on a slow cadence, not per frame): from across a district, walk-in tenements
    /// cost ZERO draw calls.
    ///
    /// Attached to each "Interior" root by InteriorBuilder at patch time; everything else happens at
    /// runtime in Start() (patch-time listener wiring never survives — the SalvageCache lesson).
    /// Colliders are NEVER culled — walls stay solid even when invisible, so physics/audio raycasts
    /// and a player glitching inside a far building all behave.
    /// </summary>
    public class InteriorCullRuntime : MonoBehaviour
    {
        [Tooltip("Extra visibility margin (m) beyond the interior's own bounds.")]
        public float margin = 10f;

        private const float CheckInterval = 0.5f; // slow cadence — the whole point vs per-frame

        private Renderer[] _renderers;
        private Transform _rig;
        private Vector3 _center;
        private float _radius;
        private float _nextCheck;
        private bool _visible = true;

        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            if (_renderers == null || _renderers.Length == 0) { enabled = false; return; }

            // World-space bounds of the whole interior → one center + radius test.
            var b = _renderers[0].bounds;
            for (int i = 1; i < _renderers.Length; i++) b.Encapsulate(_renderers[i].bounds);
            _center = b.center;
            _radius = b.extents.magnitude + margin;

            // Stagger the first check so many interiors don't all test on the same frame.
            _nextCheck = Time.time + (GetInstanceID() & 7) * 0.07f;
        }

        private void Update()
        {
            if (Time.time < _nextCheck) return;
            _nextCheck = Time.time + CheckInterval;

            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                if (rig == null) return;
                _rig = rig.transform;
            }

            bool near = (_rig.position - _center).sqrMagnitude <= _radius * _radius;
            if (near == _visible) return;
            _visible = near;
            for (int i = 0; i < _renderers.Length; i++)
                if (_renderers[i] != null) _renderers[i].enabled = near;
        }
    }
}
