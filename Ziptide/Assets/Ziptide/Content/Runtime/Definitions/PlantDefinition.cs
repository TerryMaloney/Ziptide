using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    [CreateAssetMenu(fileName = "PlantDefinition", menuName = "Ziptide/Definitions/Plant")]
    public class PlantDefinition : Definition
    {
        [Tooltip("Biome this plant is native to (BiomeDefinition.id).")]
        public string biomeId = "";

        [Tooltip("Real seconds from planting to harvest-ready (idle growth).")]
        public double growSeconds = 600;

        [Tooltip("Yield produced at harvest.")]
        public List<ResourceCost> harvestYield = new List<ResourceCost>();

        [Tooltip("Tool ids used to tend (water / fertilize / prune) — affect yield & speed.")]
        public List<string> tendToolIds = new List<string>();

        [Tooltip("Tool function required to harvest.")]
        public ToolFunction harvestWith = ToolFunction.Harvest;
    }
}
