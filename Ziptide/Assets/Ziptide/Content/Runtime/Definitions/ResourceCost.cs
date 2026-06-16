using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>A quantity of a resource, addressed by id — recipe/machine inputs &amp; outputs, loot, yields.</summary>
    [Serializable]
    public class ResourceCost
    {
        [Tooltip("Resource id (matches a ResourceDefinition.id).")]
        public string resourceId;

        [Tooltip("Amount required / produced.")]
        public double amount;
    }
}
