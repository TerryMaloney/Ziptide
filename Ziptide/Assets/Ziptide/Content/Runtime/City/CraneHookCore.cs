using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// ONE THING THAT MOVES, SLOWLY (HANGAR_AND_COUPLER_PASS §2.3).
    ///
    /// A single large object in constant motion sells the size of a space better than ten static ones,
    /// and it is the cheapest possible "the world is running without me" signal — nobody has to
    /// interact with it, nobody has to be told about it. The crane already has a jib and a hook; this
    /// makes the hook creep.
    ///
    /// Two numbers decide whether it reads as machinery or as a bug, and both are counter-intuitive:
    ///
    /// • <b>Slow.</b> A loaded hook creeps. Anything above ~0.35 m/s stops looking like tonnage on a
    ///   cable and starts looking like an animation playing at the wrong rate — the eye is very good at
    ///   this and gets it wrong in exactly one direction.
    /// • <b>Eased at both ends.</b> A triangle wave reverses instantly, and an instant reversal on a
    ///   heavy object is the single most obvious tell that nothing here has mass. The cosine costs the
    ///   same and fixes it.
    ///
    /// Pure so both can be proven rather than eyeballed, and because "is the hook moving at a
    /// believable speed" is not a question a screenshot can answer.
    /// </summary>
    public static class CraneHookCore
    {
        /// <summary>Above this the hook stops reading as weight on a cable.</summary>
        public const float MaxBelievableSpeed = 0.35f;

        /// <summary>A full down-and-up. Long enough that a passing player sees motion, not a cycle.</summary>
        public const float DefaultPeriod = 30f;

        /// <summary>Closest the hook comes to the jib.</summary>
        public const float DefaultMinDrop = 1.2f;

        /// <summary>Furthest it descends. 3 m of travel against a 16 m mast — visible, not theatrical.</summary>
        public const float DefaultMaxDrop = 4.2f;

        /// <summary>
        /// How far below the jib the hook hangs at this moment. Eased at both ends of the travel, so
        /// the reversal has weight instead of snapping.
        /// </summary>
        public static float DropAt(float time, float minDrop, float maxDrop, float period)
        {
            if (period <= 0.01f) return minDrop;
            float phase = Mathf.Repeat(time / period, 1f);
            float t = 0.5f - 0.5f * Mathf.Cos(phase * 2f * Mathf.PI);
            return Mathf.Lerp(minDrop, maxDrop, t);
        }

        /// <summary>Fastest the hook ever moves — at the midpoint of a cosine, not at the ends.</summary>
        public static float PeakSpeed(float minDrop, float maxDrop, float period)
        {
            if (period <= 0.01f) return 0f;
            return Mathf.Abs(maxDrop - minDrop) * Mathf.PI / period;
        }

        /// <summary>Is this loop slow enough to read as machinery rather than as a glitch?</summary>
        public static bool IsCalm(float minDrop, float maxDrop, float period)
            => PeakSpeed(minDrop, maxDrop, period) <= MaxBelievableSpeed;

        /// <summary>
        /// Where the cable's midpoint sits and how long it is, given the hook's current drop. The line
        /// has to stretch: a fixed-length cable with a moving hook detaches from one end or the other,
        /// and that is more distracting than no motion at all.
        /// </summary>
        public static void CableSpan(float drop, out float centreOffset, out float length)
        {
            length = Mathf.Max(0.01f, drop);
            centreOffset = -drop * 0.5f;
        }
    }
}
