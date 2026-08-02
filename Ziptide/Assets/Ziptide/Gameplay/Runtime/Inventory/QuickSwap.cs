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
        private readonly List<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor> _rightHands = new List<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>();

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
                _mgr.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)hand, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)held);
                _mgr.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)socket, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)holstered);
                _mgr.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)socket, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)held);
                _mgr.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)hand, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)holstered);
                Debug.Log("ZIPTIDE: QUICK_SWAP action=swap in=" + holstered.name + " out=" + held.name);
            }
            else if (held != null && hand != null)
            {
                var empty = EmptySocket();
                if (empty == null) return;
                _mgr.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)hand, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)held);
                _mgr.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)empty, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)held);
                Debug.Log("ZIPTIDE: QUICK_SWAP action=holster item=" + held.name);
            }
            else if (holstered != null && socket != null)
            {
                var freeHand = FirstRightHand();
                if (freeHand == null) return;
                _mgr.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)socket, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)holstered);
                _mgr.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)freeHand, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)holstered);
                Debug.Log("ZIPTIDE: QUICK_SWAP action=draw item=" + holstered.name);
            }
        }

        private void Resolve()
        {
            if (_mgr == null) _mgr = FindObjectOfType<XRInteractionManager>();
            if (_rightHands.Count == 0)
                foreach (var i in GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>(true))
                    if (i.name.ToLowerInvariant().Contains("right")) _rightHands.Add(i);
        }

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable HeldGun(out UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor hand)
        {
            foreach (var h in _rightHands)
            {
                if (h == null || !h.hasSelection) continue;
                foreach (var sel in h.interactablesSelected)
                {
                    var grab = sel as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
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

        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor FirstRightHand()
        {
            foreach (var h in _rightHands)
                if (h != null && !h.hasSelection) return h;
            return null;
        }

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable HolsteredGun(out HolsterSocketInteractor socket)
        {
            foreach (var s in GetComponentsInChildren<HolsterSocketInteractor>(true))
            {
                if (s == null || !s.hasSelection) continue;
                var grab = s.interactablesSelected[0] as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
                if (grab != null)
                {
                    socket = s;
                    return grab;
                }
            }
            socket = null;
            return null;
        }

        private HolsterSocketInteractor EmptySocket()
        {
            foreach (var s in GetComponentsInChildren<HolsterSocketInteractor>(true))
                if (s != null && !s.hasSelection) return s;
            return null;
        }
    }
}
