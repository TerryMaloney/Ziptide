using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven item definition. Swap model/prefab without changing gameplay code.
    /// </summary>
    public class ItemDefinition : ScriptableObject
    {
        [Tooltip("Unique id for this item type (e.g. pistol, medkit).")]
        public string itemId = "item";

        [Tooltip("Optional prefab to instantiate as visual; if null, use existing renderer on this object.")]
        public GameObject modelPrefab;

        [Tooltip("Forge recipe id (Resources/Forge) — the generated-mesh look. Empty = primitive visual. " +
                 "Assigned in data by ForgeAuthor; applied by ItemFactory via ForgeVisualApplier.")]
        public string forgeRecipeId = "";

        [Header("Physics (optional overrides)")]
        [Tooltip("Mass for Rigidbody. 0 = leave default.")]
        public float mass = 0.5f;

        [Tooltip("Override collider size. 0 = use mesh/bounds.")]
        public Vector3 colliderSizeOverride = Vector3.zero;

        [Header("Visual/feel tuning (zero/clear = keep ItemFactory's built-in default for this weapon type)")]
        [Tooltip("Body scale. Vector3.zero = use the factory default (need an axis at exactly 0? use 0.0001).")]
        public Vector3 visualScale = Vector3.zero;

        [Tooltip("Body color. Alpha 0 = use the factory default color.")]
        public Color visualColor = new Color(0f, 0f, 0f, 0f);

        [Tooltip("Grip attach point (local). Zero = factory default. This is where the gun sits in the hand.")]
        public Vector3 gripLocalPos = Vector3.zero;

        [Tooltip("Muzzle point (local). Zero = factory default. Bolts/rays originate here.")]
        public Vector3 muzzleLocalPos = Vector3.zero;

        [Tooltip("Grip attach rotation (local euler). Zero = the +45X convention so the barrel points where the index finger points.")]
        public Vector3 gripLocalEuler = Vector3.zero;

        [Tooltip("Laser sight color while held. Alpha 0 = the default sight color.")]
        public Color laserSightColor = new Color(0f, 0f, 0f, 0f);
    }
}
