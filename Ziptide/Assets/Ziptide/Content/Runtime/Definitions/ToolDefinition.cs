using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    public enum ToolFunction { Mine, Harvest, Water, Fertilize, Prune, Repair, Build, Combat }

    [CreateAssetMenu(fileName = "ToolDefinition", menuName = "Ziptide/Definitions/Tool")]
    public class ToolDefinition : Definition
    {
        public ToolFunction function = ToolFunction.Mine;

        [Tooltip("Higher tier = stronger/faster; gates which nodes/plants it works on.")]
        public int tier = 1;

        [Tooltip("Effectiveness multiplier (speed / yield).")]
        public float power = 1f;

        [Tooltip("Resource / plant / biome ids this tool works on (empty = any).")]
        public List<string> worksOn = new List<string>();
    }
}
