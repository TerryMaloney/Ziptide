using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Comfort-first PvP gravity gun: firing gives the SHOOTER a short self-hop in the aim direction
    /// (ground-snapped, CharacterController-safe) plus a brief vignette — mobility without a nausea-
    /// inducing velocity launch. Added by the arena patcher to the gravity gun (alongside the normal
    /// damage raycast). The decision Terry locked: comfort hop, not launch.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class PvpComfortHop : MonoBehaviour
    {
        public float hopDistance = 2.5f;

        private XRGrabInteractable _grab;
        private Transform _muzzle;

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _muzzle = transform.Find("Muzzle");
        }

        private void OnEnable() { if (_grab != null) _grab.activated.AddListener(OnFired); }
        private void OnDisable() { if (_grab != null) _grab.activated.RemoveListener(OnFired); }

        private void OnFired(ActivateEventArgs _)
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig == null) return;

            Vector3 dir = _muzzle != null ? _muzzle.forward : transform.forward;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;
            dir.Normalize();

            Vector3 target = rig.transform.position + dir * hopDistance;
            // Ground-snap so we land on a surface, never in the air or through the floor.
            if (Physics.Raycast(target + Vector3.up * 2f, Vector3.down, out var hit, 8f, ~0, QueryTriggerInteraction.Ignore))
                target.y = hit.point.y;
            else
                target.y = rig.transform.position.y;

            var cc = rig.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            rig.transform.position = target;
            if (cc != null) cc.enabled = true;

            // Brief comfort vignette (reuse the stun flash; slowFactor 1 = no slow).
            var vignette = FindObjectOfType<PlayerStunReceiver>();
            if (vignette != null) vignette.ApplyStun(0.2f, 1f);

            Debug.Log("ZIPTIDE: PVP_COMFORT_HOP");
        }
    }
}
