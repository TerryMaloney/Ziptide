using UnityEngine;

namespace Ziptide.Content
{
    [CreateAssetMenu(fileName = "ResourceDefinition", menuName = "Ziptide/Definitions/Resource")]
    public class ResourceDefinition : Definition
    {
        [Tooltip("Economy base value (scaled by BalanceConfig.globalResourceValueMultiplier).")]
        public double baseValue = 1.0;

        [Tooltip("Progression tier (1 = early game).")]
        public int tier = 1;

        [Tooltip("UI color for icons / readouts.")]
        public Color color = Color.white;
    }
}
