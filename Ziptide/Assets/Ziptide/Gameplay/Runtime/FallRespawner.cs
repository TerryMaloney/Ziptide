using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Canonical fall/gap recovery owner on the XR Origin. The original hard-Y respawn remains the final
    /// safety net, while a support probe records the last grounded pose and catches accidental holes before
    /// the player disappears below the map. Brief unsupported intervals remain legal for jumping and seams.
    /// Authored deep hazards provide a solid bottom and therefore keep ownership of their own consequences.
    /// </summary>
    public class FallRespawner : MonoBehaviour
    {
        [SerializeField] private WorldRuntime worldRuntime;
        [SerializeField] private float supportProbeRadius = 0.18f;
        [SerializeField] private float supportProbeHeight = 0.30f;
        [SerializeField] private float supportProbeDistance = 1.45f;
        [SerializeField] private float unsupportedGraceSeconds = WorldSafetyCore.DefaultUnsupportedGrace;
        [SerializeField] private float minimumDropMeters = WorldSafetyCore.DefaultMinimumDrop;

        private WorldSafetyCore _safety;
        private int _recoveryCount;

        private void Awake()
        {
            _safety = new WorldSafetyCore(unsupportedGraceSeconds, minimumDropMeters);
        }

        private void Update()
        {
            if (worldRuntime == null || worldRuntime.WorldProfile == null) return;
            var profile = worldRuntime.WorldProfile;
            if (!profile.respawnOnFall) return;
            if (_safety == null)
                _safety = new WorldSafetyCore(unsupportedGraceSeconds, minimumDropMeters);

            bool supported = HasWalkableSupport();
            WorldSafetyDecision decision = _safety.Tick(
                transform.position,
                transform.rotation,
                supported,
                Time.deltaTime,
                profile.fallYThreshold);

            if (!decision.ShouldRecover) return;

            Vector3 before = transform.position;
            if (decision.HasSafePose)
                worldRuntime.RecoverPlayerAt(transform, decision.SafePosition, decision.SafeRotation);
            else
                worldRuntime.RespawnPlayer(transform);

            _safety.ResetAfterRecovery(transform.position, transform.rotation);
            _recoveryCount++;
            Debug.Log("ZIPTIDE: GAP_RECOVERY reason=" + decision.Reason
                + " count=" + _recoveryCount
                + " unsupported=" + decision.UnsupportedSeconds.ToString("F2")
                + " from=" + before.ToString("F2")
                + " to=" + transform.position.ToString("F2")
                + " safePose=" + decision.HasSafePose);
        }

        private bool HasWalkableSupport()
        {
            Vector3 origin = transform.position + Vector3.up * supportProbeHeight;
            float distance = Mathf.Max(0.25f, supportProbeHeight + supportProbeDistance);
            RaycastHit[] hits = Physics.SphereCastAll(
                origin,
                Mathf.Max(0.04f, supportProbeRadius),
                Vector3.down,
                distance,
                ~0,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col == null || col.isTrigger) continue;
                if (col.transform == transform || col.transform.IsChildOf(transform)) continue;
                if (hits[i].normal.y < 0.35f) continue;
                return true;
            }
            return false;
        }

        /// <summary>Assign at runtime if not set in Inspector.</summary>
        public void SetWorldRuntime(WorldRuntime runtime)
        {
            worldRuntime = runtime;
        }
    }
}
