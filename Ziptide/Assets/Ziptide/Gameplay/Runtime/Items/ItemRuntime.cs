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

            // Fit the final visible hierarchy during Start, after ItemFactory and any Forge look finish.
            // This keeps dropped meshes physically above the floor without making visuals own physics.
            if (GetComponent<ItemPhysicalStability>() == null)
                gameObject.AddComponent<ItemPhysicalStability>();

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
