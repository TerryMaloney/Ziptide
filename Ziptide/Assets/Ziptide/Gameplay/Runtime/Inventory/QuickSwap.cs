using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Fortnite weapon slots, VR-ized (CONTROL_SCHEME.md "Quick-swap"): tap B (right secondary)
    /// to swap the gun in your right hand with the one on your belt — or holster a held gun /
    /// draw a holstered one when only one side has something. The belt IS the inventory state
    /// (no new bookkeeping); transfers go through the XRInteractionManager so grab/socket rules
    /// stay authoritative. No controller-button chord is reserved for developer UI. Logs
    /// QUICK_SWAP action=swap|holster|draw.
    /// </summary>
    public class QuickSwap : MonoBehaviour
    {
        private InputAction _swap;
        private XRInteractionManager _mgr;
        private readonly List<XRBaseControllerInteractor> _rightHands = new List<XRBaseControllerInteractor>();

        private void OnEnable()
        {
            if (_swap == null)
            {
                _swap = new InputAction("ZiptideQuickSwap", InputActionType.Button);
                _swap.AddBinding("<XRController>{RightHand}/secondaryButton"); // B
            }
            _swap.Enable();
        }

        private void OnDisable()
        {
            _swap?.Disable();
        }

        private void Update()
        {
            if (_swap == null || !_swap.WasPressedThisFrame()) return;

            Resolve();
            if (_mgr == null) return;

            var held = HeldGun(out var hand);
            var holstered = HolsteredGun(out var socket);

            if (held != null && holstered != null && hand != null && socket != null)
            {
                _mgr.SelectExit((IXRSelectInteractor)hand, (IXRSelectInteractable)held);
                _mgr.SelectExit((IXRSelectInteractor)socket, (IXRSelectInteractable)holstered);
                _mgr.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)held);
                _mgr.SelectEnter((IXRSelectInteractor)hand, (IXRSelectInteractable)holstered);
                Debug.Log("ZIPTIDE: QUICK_SWAP action=swap in=" + holstered.name + " out=" + held.name);
            }
            else if (held != null && hand != null)
            {
                var empty = EmptySocket();
                if (empty == null) return;
                _mgr.SelectExit((IXRSelectInteractor)hand, (IXRSelectInteractable)held);
                _mgr.SelectEnter((IXRSelectInteractor)empty, (IXRSelectInteractable)held);
                Debug.Log("ZIPTIDE: QUICK_SWAP action=holster item=" + held.name);
            }
            else if (holstered != null && socket != null)
            {
                var freeHand = FirstRightHand();
                if (freeHand == null) return;
                _mgr.SelectExit((IXRSelectInteractor)socket, (IXRSelectInteractable)holstered);
                _mgr.SelectEnter((IXRSelectInteractor)freeHand, (IXRSelectInteractable)holstered);
                Debug.Log("ZIPTIDE: QUICK_SWAP action=draw item=" + holstered.name);
            }
        }

        private void Resolve()
        {
            if (_mgr == null) _mgr = FindObjectOfType<XRInteractionManager>();
            if (_rightHands.Count == 0)
                foreach (var i in GetComponentsInChildren<XRBaseControllerInteractor>(true))
                    if (i.name.ToLowerInvariant().Contains("right")) _rightHands.Add(i);
        }

        private XRGrabInteractable HeldGun(out XRBaseControllerInteractor hand)
        {
            foreach (var h in _rightHands)
            {
                if (h == null || !h.hasSelection) continue;
                foreach (var sel in h.interactablesSelected)
                {
                    var grab = sel as XRGrabInteractable;
                    if (grab != null && grab.GetComponent<ItemRuntime>() != null)
                    {
                        hand = h;
                        return grab;
                    }
                }
            }
            hand = null;
            return null;
        }

        private XRBaseControllerInteractor FirstRightHand()
        {
            foreach (var h in _rightHands) if (h != null && !h.hasSelection) return h;
            return null;
        }

        private XRSocketInteractor EmptySocket()
        {
            var belt = GetComponentInChildren<BeltInventory>(true);
            if (belt == null) return null;
            foreach (var s in belt.Sockets)
                if (s != null && !s.hasSelection) return s;
            return null;
        }

        private XRGrabInteractable HolsteredGun(out XRSocketInteractor socket)
        {
            var belt = GetComponentInChildren<BeltInventory>(true);
            if (belt != null)
                foreach (var s in belt.Sockets)
                    if (s != null && s.hasSelection)
                        foreach (var sel in s.interactablesSelected)
                        {
                            var grab = sel as XRGrabInteractable;
                            if (grab != null && grab.GetComponent<ItemRuntime>() != null)
                            { socket = s; return grab; }
                        }
            socket = null;
            return null;
        }
    }
}
