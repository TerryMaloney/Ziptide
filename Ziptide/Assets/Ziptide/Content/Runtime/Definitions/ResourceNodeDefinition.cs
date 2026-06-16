using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A harvestable resource node type (per biome): yields a resource when worked with a suitable
    /// tool. Data-only and resolved by id via <see cref="DefinitionRegistry{TDef}"/>; runtime
    /// depletion lives on <see cref="ResourceNode"/> (Definition + Runtime.Init pattern).
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceNodeDefinition", menuName = "Ziptide/Definitions/Resource Node")]
    public class ResourceNodeDefinition : Definition
    {
        [Tooltip("Resource id this node yields (should match a ResourceDefinition id).")]
        public string resourceId = "";

        [Tooltip("Base amount granted per successful harvest (scaled by tool power).")]
        public double yieldPerHarvest = 1.0;

        [Tooltip("Total amount the node holds before it is exhausted. <= 0 means inexhaustible.")]
        public double reserve = 0.0;

        [Tooltip("Tool function required to work this node (Mine for ore-like nodes, Harvest for plants).")]
        public ToolFunction requiredFunction = ToolFunction.Mine;

        [Tooltip("Minimum tool tier required; a weaker tool cannot work the node.")]
        public int requiredToolTier = 1;

        [Tooltip("Biome this node is native to (optional; also lets a biome-scoped tool match it).")]
        public string biomeId = "";
    }
}
