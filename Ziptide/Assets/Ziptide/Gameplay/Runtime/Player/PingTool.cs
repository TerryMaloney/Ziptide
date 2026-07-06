using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Fortnite's ping, VR-ized (CONTROL_SCHEME.md): pull the LEFT trigger with an EMPTY left
    /// hand and a marker beacon drops where you point (60 m ray). One active ping at a time,
    /// auto-despawns after 20 s. Solo wayfinding today; the A6 co-op seam later (the ping
    /// position is one Vector3 to sync). Rig-ensured by PlayerRigPersistence; logs PING_AT.
    /// </summary>
    public class PingTool : MonoBehaviour
    {
        private const float Range = 60f;
        private const float LifeSeconds = 20f;
        private static readonly Color PingColor = new Color(1f, 0.8f, 0.25f);

        private InputAction _ping;
        private Transform _leftHand;
        private GameObject _current;

        private void OnEnable()
        {
            if (_ping == null)
            {
                _ping = new InputAction("ZiptidePing", InputActionType.Button);
                _ping.AddBinding("<XRController>{LeftHand}/triggerPressed");
            }
            _ping.Enable();
        }

        private void OnDisable()
        {
            _ping?.Disable();
        }

        private void Update()
        {
            if (_leftHand == null)
            {
                foreach (var c in GetComponentsInChildren<ActionBasedController>(true))
                    if (c.name.ToLowerInvariant().Contains("left")) { _leftHand = c.transform; break; }
                if (_leftHand == null) return;
            }

            if (_ping == null || !_ping.WasPressedThisFrame()) return;
            if (LeftHandBusy()) return; // holding something = the trigger belongs to the item
            if (!Physics.Raycast(_leftHand.position, _leftHand.forward, out var hit, Range)) return;

            if (_current != null) Destroy(_current);
            _current = new GameObject("__Ping");
            _current.transform.position = hit.point;
            ObjectiveBeacon.Attach(_current, PingColor, 8f);
            Destroy(_current, LifeSeconds);
            Debug.Log("ZIPTIDE: PING_AT " + hit.point.ToString("F1"));
        }

        private bool LeftHandBusy()
        {
            foreach (var i in _leftHand.GetComponentsInChildren<XRBaseControllerInteractor>())
                if (i != null && i.hasSelection) return true;
            return false;
        }
    }
}
