using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven taser dart gun: fires sticky darts that shock targets.
    /// </summary>
    public class TaserDartGunDefinition : ItemDefinition
    {
        [Header("Taser Dart Gun")]
        public float fireCooldown = 0.4f;
        public float muzzleVelocity = 25f;
        public float dartMass = 0.05f;
        public float dartLifetime = 5f;
        public float stunSeconds = 3f;
        public float hitImpulse = 2f;

        [Header("Haptics")]
        [Range(0f, 1f)] public float hapticAmplitude = 0.6f;
        public float hapticDuration = 0.08f;

        [Header("Audio (optional)")]
        public AudioClip fireClip;
        public AudioClip impactClip;
    }
}
