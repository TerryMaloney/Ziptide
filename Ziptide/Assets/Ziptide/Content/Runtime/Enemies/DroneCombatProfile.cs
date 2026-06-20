using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-only tuning for a combat-drone VARIANT (Stinger vs Sentry, etc.). Drone Combat V1 reads
    /// this if assigned; otherwise the behavior's serialized defaults apply. New enemy flavors ship as
    /// .asset files, not new code (per docs/systems/CREATURE_DRONE.md). Drop variants under a
    /// Resources/Enemies/ folder to auto-resolve by DroneZoneDef.variantId.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Drone Combat Profile", fileName = "DroneCombatProfile")]
    public class DroneCombatProfile : ScriptableObject
    {
        [Header("Detection")]
        public float detectRange = 10f;
        public float loseRange = 14f;

        [Header("Movement")]
        public float standoffDistance = 5f;
        public float orbitSpeed = 40f;
        public float verticalBob = 0.3f;
        public float patrolRadius = 3f;
        public float patrolSpeed = 20f;

        [Header("Attack")]
        public float telegraphSeconds = 0.9f;
        public float boltCooldown = 2.5f;
        public float boltSpeed = 6f;
        public float stunSeconds = 1.2f;
        [Range(0.1f, 1f)] public float slowFactor = 0.45f;
    }
}
