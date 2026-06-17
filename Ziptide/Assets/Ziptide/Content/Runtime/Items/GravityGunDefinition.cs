using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven gravity gun: a hitscan "grav pulse" that downs a drone and launches it. The
    /// non-bullet "explorer tech" sibling to the taser (see docs/09_GEAR_AND_TOOLS.md). Holsterable
    /// like the taser.
    /// </summary>
    public class GravityGunDefinition : ItemDefinition
    {
        [Header("Gravity Gun")]
        [Tooltip("Max distance the grav pulse reaches.")]
        public float range = 25f;
        [Tooltip("Seconds between shots.")]
        public float fireCooldown = 0.6f;
        [Tooltip("Impulse applied to a hit drone (launch/knockback strength).")]
        public float launchForce = 14f;
        [Tooltip("Extra upward bias on the launch so drones pop off the ground readably.")]
        public float upwardBias = 4f;

        [Header("Haptics")]
        [Range(0f, 1f)] public float hapticAmplitude = 0.7f;
        public float hapticDuration = 0.12f;

        [Header("Audio (optional)")]
        public AudioClip fireClip;
    }
}
