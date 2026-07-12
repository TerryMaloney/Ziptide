using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Socket that only accepts interactables with ItemRuntime whose definition itemId is in the
    /// allowed list. A valid accepted selection also publishes the first-hour ITEM_HOLSTERED
    /// semantic signal once without changing socket rules or forcing a save.
    /// </summary>
    public class HolsterSocketInteractor : XRSocketInteractor
    {
        [Tooltip("Item IDs this socket accepts (e.g. pistol).")]
        [SerializeField] private List<string> allowedItemIds = new List<string> { "pistol", "taser_dart_gun", "gravity_gun", "handheld_camera" };

        private bool _firstHolsterReported;

        /// <summary>
        /// Published once for the first valid holster when the profile has not already completed it.
        /// The payload is the accepted ItemDefinition.itemId.
        /// </summary>
        public static event Action<string> ItemHolstered;

        protected override void Start()
        {
            base.Start();
            selectEntered.AddListener(OnSelectEnteredCallback);
        }

        protected override void OnDestroy()
        {
            selectEntered.RemoveListener(OnSelectEnteredCallback);
            base.OnDestroy();
        }

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            if (!base.CanHover(interactable)) return false;
            return ItemIdAllowed(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (!base.CanSelect(interactable)) return false;
            return ItemIdAllowed(interactable);
        }

        private bool ItemIdAllowed(IXRInteractable interactable)
        {
            var go = (interactable as Component)?.gameObject;
            if (go == null) return false;
            var item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return false;
            return allowedItemIds != null && allowedItemIds.Contains(item.Definition.itemId);
        }

        private void OnSelectEnteredCallback(SelectEnterEventArgs args)
        {
            var go = (args.interactableObject as Component)?.gameObject;
            if (go == null) return;

            var item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return;

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            string itemId = item.Definition.itemId;
            if (!FirstHourHolsterSignal.TryReport(
                itemId,
                profile,
                ref _firstHolsterReported,
                PublishItemHolstered))
            {
                return;
            }

            Debug.Log("ZIPTIDE: FIRST_HOLSTER item=" + itemId + " socket=" + gameObject.name);
        }

        private static void PublishItemHolstered(string itemId)
        {
            ItemHolstered?.Invoke(itemId);
        }
    }
}
