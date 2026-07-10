using System;

namespace Ziptide.Core
{
    /// <summary>A watering can's saved-nothing state: just how full it is (0..1).</summary>
    [Serializable]
    public struct CanState
    {
        public float fill01;
        public static CanState Full => new CanState { fill01 = 1f };
    }

    /// <summary>
    /// GARDEN AAA 4.2c — the PURE pour law (the design doc's "tilt past ~60°" hands-on moment).
    /// No pour below the start angle, a linear ramp to full rate at the full angle, drain caps at
    /// empty, refill snaps to full. Pure math — the scene translator reads the can's tilt off the
    /// grabbed transform and hands it here, so the feel is pinned by tests, not by frame luck.
    /// </summary>
    public static class PourCore
    {
        public const float PourStartDeg = 55f;   // upright-ish: nothing comes out
        public const float PourFullDeg = 110f;   // fully tipped: max flow
        public const float MaxFlowPerSecond = 0.5f; // a full can empties in ~2s fully tipped

        /// <summary>Flow fraction (0..1) for a tilt angle from upright.</summary>
        public static float RateAt(float tiltDeg)
        {
            if (tiltDeg <= PourStartDeg) return 0f;
            if (tiltDeg >= PourFullDeg) return 1f;
            return (tiltDeg - PourStartDeg) / (PourFullDeg - PourStartDeg);
        }

        /// <summary>Advance one frame: returns the new state and how much water (in fill units)
        /// actually left the can this tick — the translator turns that into tend credit + stream VFX.</summary>
        public static CanState Tick(CanState s, float tiltDeg, float dt, out float poured)
        {
            poured = 0f;
            if (dt <= 0f || s.fill01 <= 0f) return s;
            float flow = RateAt(tiltDeg) * MaxFlowPerSecond * dt;
            poured = flow > s.fill01 ? s.fill01 : flow;
            s.fill01 -= poured;
            return s;
        }

        public static CanState Refill(CanState s) { s.fill01 = 1f; return s; }
    }
}
