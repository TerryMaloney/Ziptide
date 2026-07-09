using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Per-world difficulty scaling (COMBAT_HEALTH_PLAN Phase C). A world's tier
    /// (<see cref="CityLayoutDefinition"/> / POI tier: 0 early · 1 mid · 2 capstone) scales how tough its
    /// creatures are, so the SAME creature id reads as a pushover on a tier-0 world and a real threat on a
    /// tier-2 one — Terry's "adjust the hardness per planet so it doesn't get repetitive" knob. Pure and
    /// deterministic; multipliers apply on TOP of the unified <see cref="CreatureBaselines"/> scale, so
    /// creatures still speak one damage language everywhere — a planet just turns the dial. Starting
    /// values; Terry tunes on device.
    /// </summary>
    public static class DifficultyScale
    {
        public const int MaxTier = 2;

        /// <summary>Creature max-health multiplier for a world tier (clamped to 0..MaxTier).</summary>
        public static float HealthMult(int tier)
        {
            switch (Clamp(tier))
            {
                case 1: return 1.5f;
                case 2: return 2.0f;
                default: return 1.0f; // tier 0
            }
        }

        /// <summary>Creature-to-player damage multiplier for a world tier (used by Phase B enemy→player).</summary>
        public static float DamageMult(int tier)
        {
            switch (Clamp(tier))
            {
                case 1: return 1.25f;
                case 2: return 1.5f;
                default: return 1.0f;
            }
        }

        /// <summary>Base health scaled for a tier, as a whole number ≥ 1 (health is an integer-scale pool).</summary>
        public static int ScaledHealth(float baseHealth, int tier)
            => Mathf.Max(1, Mathf.RoundToInt(baseHealth * HealthMult(tier)));

        /// <summary>Base damage scaled for a tier, as a whole number ≥ 1.</summary>
        public static int ScaledDamage(float baseDamage, int tier)
            => Mathf.Max(1, Mathf.RoundToInt(baseDamage * DamageMult(tier)));

        private static int Clamp(int tier) => tier < 0 ? 0 : (tier > MaxTier ? MaxTier : tier);
    }
}
