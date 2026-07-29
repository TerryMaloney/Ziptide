using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>Where a ring stands relative to the course: already flown, the one to fly NEXT,
    /// or further ahead.</summary>
    public enum RingLampState { Future, Next, Passed }

    /// <summary>
    /// THE LAMP-CHASE (LEVEL1_SPATIAL_SCRIPT §2) — pure color math for the ring course's
    /// wayfinding read: the NEXT ring wears a bright amber window that sweeps around its
    /// segments ("go HERE"), passed rings settle to dim green ("done"), future rings wait in
    /// dim amber. Procedural v1 per the stand-in law — Tripo/VFX re-skins later, the read
    /// ships now. RingCourseLightsRuntime applies this; tests pin it without a scene.
    /// </summary>
    public static class RingLampChaseCore
    {
        /// <summary>Full sweeps of the chase window around the next ring, per second.</summary>
        public const float ChaseCyclesPerSecond = 0.8f;

        /// <summary>Fraction of the ring the bright window covers at any instant.</summary>
        public const float ChaseWindowFraction = 0.3f;

        public static readonly Color PassedColor = new Color(0.18f, 0.55f, 0.28f);
        public static readonly Color FutureColor = new Color(0.40f, 0.27f, 0.10f);
        public static readonly Color NextBaseColor = new Color(0.90f, 0.55f, 0.20f);
        public static readonly Color NextChaseColor = new Color(1.00f, 0.92f, 0.45f);

        public static RingLampState Classify(int ringIndex, int nextRingIndex)
        {
            if (ringIndex < nextRingIndex) return RingLampState.Passed;
            return ringIndex == nextRingIndex ? RingLampState.Next : RingLampState.Future;
        }

        /// <summary>
        /// 0..1 brightness of the sweeping window at one segment. The window travels in segment
        /// order, wraps forever, and peaks at the window's center — a chasing lamp, not a strobe.
        /// </summary>
        public static float ChaseWeight(int segment, int segmentCount, float time)
        {
            if (segmentCount <= 0) return 0f;
            float phase = time * ChaseCyclesPerSecond - (float)segment / segmentCount;
            phase -= Mathf.Floor(phase);
            if (phase >= ChaseWindowFraction) return 0f;
            return 1f - Mathf.Abs(phase / ChaseWindowFraction * 2f - 1f);
        }

        public static Color SegmentColor(RingLampState state, int segment, int segmentCount, float time)
        {
            switch (state)
            {
                case RingLampState.Passed: return PassedColor;
                case RingLampState.Next:
                    return Color.Lerp(NextBaseColor, NextChaseColor,
                        ChaseWeight(segment, segmentCount, time));
                default: return FutureColor;
            }
        }
    }
}
