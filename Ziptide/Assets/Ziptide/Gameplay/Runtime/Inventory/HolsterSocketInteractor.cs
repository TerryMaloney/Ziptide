using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Canonical belt socket. It accepts declared portable items, remains the real XRI selector while
    /// an item is holstered, and derives a belt pose separately from the item's hand grip. This prevents
    /// a holstered gun from remaining in an ambiguous hand/parent state and lets repair parts use the
    /// player's free hand without fighting the weapon.
    /// </summary>
    public class HolsterSocketInteractor : XRSocketInteractor
    {
        private static readonly HashSet<string> DefaultAllowedIds = new HashSet<string>
        {
            "pistol", "taser_dart_gun", "gravity_gun", "handheld_camera", "breaker_blade"
        };

        [Tooltip("Item IDs this socket accepts.")]
        [SerializeField] private List<string> allowedItemIds = new List<string>
        {
            "pistol", "taser_dart_gun", "gravity_gun", "handheld_camera", "breaker_blade"
        };

        private bool _firstHolsterReported;

        public static event Action<string> ItemHolstered;

        public static bool AllowsItemId(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && DefaultAllowedIds.Contains(itemId);
        }

        protected override void Start()
        {
            base.Start();
            selectEntered.AddListener(OnSelectEnteredCallback);
            selectExited.AddListener(OnSelectExitedCallback);
        }

        protected override void OnDestroy()
        {
            selectEntered.RemoveListener(OnSelectEnteredCallback);
            selectExited.RemoveListener(OnSelectExitedCallback);
            base.OnDestroy();
        }

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && ItemIdAllowed(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && ItemIdAllowed(interactable);
        }

        private bool ItemIdAllowed(IXRInteractable interactable)
        {
            GameObject go = (interactable as Component)?.gameObject;
            if (go == null) return false;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return false;
            return allowedItemIds != null && allowedItemIds.Contains(item.Definition.itemId);
        }

        private void OnSelectEnteredCallback(SelectEnterEventArgs args)
        {
            GameObject go = (args.interactableObject as Component)?.gameObject;
            if (go == null) return;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            XRGrabInteractable grab = go.GetComponent<XRGrabInteractable>();
            if (item == null || item.Definition == null || grab == null) return;

            Rigidbody body = go.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.useGravity = false;
                body.isKinematic = true;
            }

            StartCoroutine(ApplySocketPoseAfterSelection(go.transform, grab, item.Definition.itemId));

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            string itemId = item.Definition.itemId;
            FirstHourHolsterSignal.TryReport(itemId, profile, ref _firstHolsterReported, PublishItemHolstered);
            Debug.Log("ZIPTIDE: HOLSTER_SELECT_ENTER item=" + itemId + " socket=" + gameObject.name
                + " selectors=" + grab.interactorsSelecting.Count);
        }

        private IEnumerator ApplySocketPoseAfterSelection(Transform item, XRGrabInteractable grab, string itemId)
        {
            yield return null;
            if (item == null || grab == null || !grab.isSelected) yield break;

            Transform socketAnchor = attachTransform != null ? attachTransform : transform;
            Transform itemGrip = grab.attachTransform;
            Quaternion desiredRoot = transform.rotation * HolsterPoseCore.Resolve(itemId, gameObject.name);
            if (itemGrip != null)
                socketAnchor.rotation = desiredRoot * itemGrip.localRotation;
            else
                socketAnchor.rotation = desiredRoot;

            // Let XRI resolve one frame with the item-specific anchor, then verify the socket still owns it.
            yield return null;
            bool owned = false;
            foreach (IXRSelectInteractor interactor in grab.interactorsSelecting)
                if (ReferenceEquals(interactor, this)) { owned = true; break; }
            Debug.Log((owned ? "ZIPTIDE: HOLSTER_POSE_APPLIED" : "ZIPTIDE: HOLSTER_OWNERSHIP_FAIL")
                + " item=" + itemId + " socket=" + gameObject.name
                + " rootForward=" + item.forward.ToString("F2"));
        }

        private void OnSelectExitedCallback(SelectExitEventArgs args)
        {
            GameObject go = (args.interactableObject as Component)?.gameObject;
            ItemRuntime item = go != null ? go.GetComponent<ItemRuntime>() : null;
            Debug.Log("ZIPTIDE: HOLSTER_SELECT_EXIT item="
                + (item != null && item.Definition != null ? item.Definition.itemId : "UNKNOWN")
                + " socket=" + gameObject.name);
        }

        private static void PublishItemHolstered(string itemId)
        {
            ItemHolstered?.Invoke(itemId);
        }
    }
}
