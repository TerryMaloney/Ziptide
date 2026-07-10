using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE II P4 — the thin runtime applier for <see cref="ForgeGaitMotor"/>. Bind() it to a
    /// built skeleton once; every LateUpdate it measures its OWN world speed (no Gameplay
    /// assembly reference needed — behaviors just move the GameObject and the gait follows),
    /// smooths it, evaluates the motor, and composes the deltas onto the captured build-pose
    /// local rotations: <c>bone.localRotation = baseLocal[i] * delta[i]</c>.
    /// BIND-POSE LAW: rotations only — bindposes are never touched after Build.
    /// </summary>
    public class ForgeCreatureAnimator : MonoBehaviour
    {
        [Tooltip("World speed (m/s) that reads as full gait (speed01 = 1).")]
        public float fullSpeed = 1.6f;

        private ForgeCreatureBody _body;
        private Transform[] _bones;
        private Quaternion[] _baseLocal;
        private Quaternion[] _delta;
        private Vector3 _baseRootScale;
        private int _breathSeed;
        private Vector3 _lastPos;
        private float _speed01;
        private bool _bound;

        /// <summary>Wire the animator to a built skeleton (ForgeSkinnedBuilder.Result.bones).</summary>
        public void Bind(ForgeCreatureBody body, Transform[] bones)
        {
            _body = body;
            _bones = bones;
            _baseLocal = new Quaternion[bones.Length];
            _delta = new Quaternion[bones.Length];
            for (int i = 0; i < bones.Length; i++)
                if (bones[i] != null) _baseLocal[i] = bones[i].localRotation;
            _baseRootScale = bones.Length > 0 && bones[0] != null ? bones[0].localScale : Vector3.one;
            // Stable per-INSTANCE phase (not per-species): a pack of the same body must not
            // breathe in lockstep, so hash the instance id, not the bodyId.
            _breathSeed = GetInstanceID();
            _lastPos = transform.position;
            _bound = true;
        }

        private void LateUpdate()
        {
            if (!_bound || _body == null || _bones == null) return;
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            // Self-measured speed, smoothed — a teleport spike decays instead of kicking the gait.
            Vector3 pos = transform.position;
            float v = (pos - _lastPos).magnitude / dt;
            _lastPos = pos;
            _speed01 = Mathf.Lerp(_speed01, Mathf.Clamp01(v / Mathf.Max(0.01f, fullSpeed)), dt * 6f);

            ForgeGaitMotor.Evaluate(_body, Time.time, _speed01, _delta);
            for (int i = 1; i < _bones.Length && i < _delta.Length; i++)
                if (_bones[i] != null)
                    _bones[i].localRotation = _baseLocal[i] * _delta[i];

            // The breath channel: root-bone scale only (rotations stay the motor's; bind poses
            // untouched). The whole body swells with the root, limbs included — alive, not idle.
            if (_bones.Length > 0 && _bones[0] != null)
                _bones[0].localScale = Vector3.Scale(_baseRootScale,
                    ForgeGaitMotor.BreathScale(_breathSeed, Time.time, _speed01));
        }
    }
}
