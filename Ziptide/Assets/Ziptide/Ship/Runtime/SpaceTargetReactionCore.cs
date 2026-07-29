using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>How a salvage drone is currently reacting to the pilot.</summary>
    public enum SpaceTargetMood
    {
        /// <summary>Nobody near: slow idle bob, eye dark. The lane at rest.</summary>
        Dormant,
        /// <summary>Pilot inside wake range: eye lit, bob doubles — it has NOTICED you.</summary>
        Woken,
        /// <summary>Recently hit: sliding along its home axis so a hit is a fight, not a click.</summary>
        Evading,
    }

    /// <summary>
    /// THE DRONE REACTION LAYER (LEVEL1_SPATIAL_SCRIPT §2 — Terry: "how do the space droids act
    /// or react"). Until now the drones only bobbed and recharged: they never acknowledged the
    /// pilot, so the space leg had no inhabitants, just scenery with hitpoints. This is the pure
    /// rule set — wake with hysteresis (a drone hovering on the boundary must not strobe between
    /// moods), an evade window after each hit, and the strafe offset that makes an evading drone
    /// slide along its own axis. Non-lethal law preserved: reactions change MOTION and LIGHT
    /// only — no drone ever chases, shoots, or leaves its post.
    /// </summary>
    public static class SpaceTargetReactionCore
    {
        /// <summary>Pilot must come this close (lane metres) for a dormant drone to wake.</summary>
        public const float WakeRadius = 40f;

        /// <summary>And must get this far away before it settles again (hysteresis band).</summary>
        public const float SleepRadius = 55f;

        /// <summary>How long after a hit the drone keeps sliding.</summary>
        public const float EvadeSeconds = 3f;

        /// <summary>Peak lateral slide from home, in metres.</summary>
        public const float StrafeAmplitude = 6f;

        /// <summary>Slide oscillations per second while evading.</summary>
        public const float StrafeCyclesPerSecond = 0.35f;

        /// <summary>Idle bob is doubled once the drone has noticed you.</summary>
        public const float WokenBobMultiplier = 2f;

        /// <summary>
        /// The mood transition. Evading outranks waking (a hit drone is awake by definition), and
        /// a disabled drone has no mood at all — powered down is powered down.
        /// </summary>
        public static SpaceTargetMood Classify(SpaceTargetMood current, float pilotDistance,
            float secondsSinceHit, bool disabled)
        {
            if (disabled) return SpaceTargetMood.Dormant;
            if (secondsSinceHit >= 0f && secondsSinceHit < EvadeSeconds) return SpaceTargetMood.Evading;
            if (current == SpaceTargetMood.Dormant)
                return pilotDistance <= WakeRadius ? SpaceTargetMood.Woken : SpaceTargetMood.Dormant;
            // Woken or just-finished evading: stay awake until the pilot clears the outer band.
            return pilotDistance >= SleepRadius ? SpaceTargetMood.Dormant : SpaceTargetMood.Woken;
        }

        /// <summary>Lateral offset from the drone's home position, metres. Zero unless evading.</summary>
        public static float StrafeOffset(SpaceTargetMood mood, float time, float phase)
        {
            if (mood != SpaceTargetMood.Evading) return 0f;
            return Mathf.Sin((time * StrafeCyclesPerSecond * 2f * Mathf.PI) + phase) * StrafeAmplitude;
        }

        /// <summary>Idle bob scale for the mood — an awake drone breathes faster.</summary>
        public static float BobMultiplier(SpaceTargetMood mood) =>
            mood == SpaceTargetMood.Dormant ? 1f : WokenBobMultiplier;

        /// <summary>Eye emissive strength 0..1 for the mood (dark asleep, hot when hit).</summary>
        public static float EyeIntensity(SpaceTargetMood mood)
        {
            switch (mood)
            {
                case SpaceTargetMood.Evading: return 1f;
                case SpaceTargetMood.Woken: return 0.6f;
                default: return 0.12f;
            }
        }
    }
}
