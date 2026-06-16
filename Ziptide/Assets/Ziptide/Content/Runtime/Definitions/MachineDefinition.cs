using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    [CreateAssetMenu(fileName = "MachineDefinition", menuName = "Ziptide/Definitions/Machine")]
    public class MachineDefinition : Definition
    {
        [Tooltip("Consumed per second of operation.")]
        public List<ResourceCost> inputs = new List<ResourceCost>();

        [Tooltip("Produced per second of operation.")]
        public List<ResourceCost> outputs = new List<ResourceCost>();

        [Tooltip("Operations / throughput per second.")]
        public double ratePerSecond = 1.0;

        [Tooltip("Storage cap for produced output before it must be collected (0 = uncapped).")]
        public double storageCap = 100.0;

        [Tooltip("Health; reaching 0 requires repair via repairRecipe.")]
        public float maxHealth = 100f;

        public RecipeDefinition buildRecipe;
        public RecipeDefinition repairRecipe;
    }
}
