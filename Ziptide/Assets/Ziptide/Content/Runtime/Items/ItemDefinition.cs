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

        [Header("Physics (optional overrides)")]
        [Tooltip("Mass for Rigidbody. 0 = leave default.")]
        public float mass = 0.5f;

        [Tooltip("Override collider size. 0 = use mesh/bounds.")]
        public Vector3 colliderSizeOverride = Vector3.zero;
    }
}
