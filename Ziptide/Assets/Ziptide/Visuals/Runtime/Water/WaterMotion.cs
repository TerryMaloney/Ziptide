using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.3 commit 2 — the pure water DYNAMICS: the scrolling-normal offset and the gentle
    /// swell/bob, both deterministic and EditMode-testable. `ZiptideWater` composes these each frame
    /// (scroll animates the normal-map offset; bob displaces the low-res plane verts). Kept pure so
    /// the motion is provable without a scene — a swell that exceeds its amplitude, or scroll that
    /// stalls, fails a test, not the headset.
    /// </summary>
    public static class WaterMotion
    {
        /// <summary>Normal-map UV offset for a layer flowing toward <paramref name="dirDeg"/> at
        /// <paramref name="speed"/> tiles/second. Direction 0° flows +U.</summary>
        public static Vector2 ScrollOffset(float dirDeg, float speed, float time)
        {
            float r = dirDeg * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(r), Mathf.Sin(r)) * (speed * time);
        }

        /// <summary>Gentle swell height at world XZ — two incommensurate sines (the Sway/gait family)
        /// so the surface never metronomes. Strictly within [-amp, amp].</summary>
        public static float BobHeight(float x, float z, float time, float amp, float freq)
        {
            float a = Mathf.Sin(freq * (x * 0.7f + z * 0.3f) + time * 0.9f);
            float b = Mathf.Sin(freq * 1.37f * (x * 0.2f - z * 0.6f) + time * 1.31f + 1.3f);
            return amp * 0.5f * (a + b);
        }
    }
}
