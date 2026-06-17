using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Socket that only accepts interactables with ItemRuntime whose definition itemId is in the allowed list.
    /// </summary>
    public class HolsterSocketInteractor : XRSocketInteractor
    {
        [Tooltip("Item IDs this socket accepts (e.g. pistol).")]
        [SerializeField] private List<string> allowedItemIds = new List<string> { "pistol", "taser_dart_gun", "gravity_gun" };

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            if (!base.CanHover(interactable)) return false;
            var go = (interactable as Component)?.gameObject;
            if (go == null) return false;
            var item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return false;
            return allowedItemIds != null && allowedItemIds.Contains(item.Definition.itemId);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (!base.CanSelect(interactable)) return false;
            var go = (interactable as Component)?.gameObject;
            if (go == null) return false;
            var item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return false;
            return allowedItemIds != null && allowedItemIds.Contains(item.Definition.itemId);
        }
    }
}
