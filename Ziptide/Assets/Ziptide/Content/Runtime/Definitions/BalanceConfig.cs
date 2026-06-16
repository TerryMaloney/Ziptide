using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>Central tuning knobs — balance the whole economy in data, no code changes.</summary>
    [CreateAssetMenu(fileName = "BalanceConfig", menuName = "Ziptide/Balance Config")]
    public class BalanceConfig : Definition
    {
        [Header("Idle")]
        [Tooltip("Max offline hours that accrue (caps away-from-game gains).")]
        public double idleCapHours = 8.0;

        [Tooltip("Default storage cap for machines / nodes when unspecified (0 = uncapped).")]
        public double defaultStorageCap = 1000.0;

        [Header("Economy multipliers")]
        public double globalResourceValueMultiplier = 1.0;
        public double mineRateMultiplier = 1.0;
        public double growthSpeedMultiplier = 1.0;

        [Header("Difficulty")]
        public float creatureDifficultyMultiplier = 1.0f;

        /// <summary>idleCapHours expressed in seconds for IdleEngine.Accrue's maxSeconds.</summary>
        public long IdleCapSeconds => (long)(idleCapHours * 3600.0);
    }
}
