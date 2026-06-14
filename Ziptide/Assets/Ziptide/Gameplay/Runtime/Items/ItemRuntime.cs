using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Runtime component for a data-driven item. Ensures Rigidbody, Collider, XRGrabInteractable and applies definition.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class ItemRuntime : MonoBehaviour
    {
        [SerializeField] private ItemDefinition definition;

        public ItemDefinition Definition => definition;

        private void Awake()
        {
            ApplyDefinition();
        }

        /// <summary>
        /// Called by ItemFactory instead of reflection to set the definition at runtime.
        /// Immediately applies mass and movement type.
        /// </summary>
        public void Init(ItemDefinition def)
        {
            definition = def;
            ApplyDefinition();
        }

        private void ApplyDefinition()
        {
            if (definition == null) return;
            var rb = GetComponent<Rigidbody>();
            if (rb != null && definition.mass > 0f)
                rb.mass = definition.mass;
            var grab = GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
    }
}
