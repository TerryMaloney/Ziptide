using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Socket that accepts delivery items (by itemId) and notifies JobDirector for DeliverToSocketStep.
    /// </summary>
    public class DeliveryCradleSocketInteractor : XRSocketInteractor
    {
        [Tooltip("Socket id reported to JobDirector (e.g. delivery_cradle).")]
        [SerializeField] private string socketId = "delivery_cradle";

        [Tooltip("Item IDs this socket accepts for delivery.")]
        [SerializeField] private List<string> allowedItemIds = new List<string> { "crate" };

        private JobDirector _director;

        public void Bind(JobDirector director)
        {
            _director = director;
        }

        protected override void Start()
        {
            base.Start();
            if (_director == null)
                _director = FindObjectOfType<JobDirector>();
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
            if (_director == null) return;
            var go = (args.interactableObject as Component)?.gameObject;
            if (go == null) return;
            var item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return;
            _director.ReportDeliver(socketId, item.Definition.itemId);
        }
    }
}
