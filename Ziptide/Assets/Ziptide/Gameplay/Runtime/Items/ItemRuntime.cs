using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Runtime component for a data-driven item. Ensures Rigidbody, Collider, XRGrabInteractable and applies definition.
    /// Scene-authored items also reapply their serialized Forge look during Awake. Runtime-created items
    /// still receive their Forge look from ItemFactory after Init, once the complete item hierarchy exists.
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
            EnsurePhysicalStability();

            // ItemFactory adds this component before Init, so its definition is null during Awake and
            // the factory remains the single Forge caller for runtime-created items. A scene-authored
            // item already has its definition serialized here; reapplying at runtime replaces any stale
            // build-generated ForgeVisual child whose gitignored material asset is unavailable in the
            // current checkout/build, instead of exposing an active null-material renderer.
            if (definition != null && !string.IsNullOrEmpty(definition.forgeRecipeId))
                Ziptide.Visuals.ForgeVisualApplier.TryApply(gameObject, definition.forgeRecipeId);
        }

        /// <summary>
        /// Called by ItemFactory instead of reflection to set the definition at runtime.
        /// Immediately applies mass and movement type and ensures the final visible-collider fit is
        /// scheduled even in construction contexts where Unity has not invoked Awake yet.
        /// </summary>
        public void Init(ItemDefinition def)
        {
            definition = def;
            ApplyDefinition();
            EnsurePhysicalStability();
        }

        private void EnsurePhysicalStability()
        {
            // Start runs after ItemFactory and any Forge look finish, so the final visible hierarchy—not
            // the temporary primitive shell—defines floor support. This is idempotent for scene-authored
            // Awake and runtime-created Init paths.
            if (GetComponent<ItemPhysicalStability>() == null)
                gameObject.AddComponent<ItemPhysicalStability>();
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
