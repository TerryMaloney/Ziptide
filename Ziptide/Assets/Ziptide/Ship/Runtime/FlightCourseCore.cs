using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>
    /// P4b — PURE ring-course progress for the tutorial flight hop (CONTROLS_AND_FLIGHT: "fly
    /// through a few guided rings"). Rings are passed IN ORDER: only the next ring's proximity
    /// counts, so skipping ahead can't scramble progress, and a passed ring stays passed. No scene
    /// types — the runtime feeds it FlightState.position. Pinned by FlightCourseCoreTests.
    /// </summary>
    public class FlightCourseCore
    {
        private readonly Vector3[] _rings;
        private readonly float _radiusSqr;

        public FlightCourseCore(Vector3[] rings, float ringRadius)
        {
            _rings = rings ?? new Vector3[0];
            float r = Mathf.Max(0.01f, ringRadius);
            _radiusSqr = r * r;
        }

        public int RingCount => _rings.Length;
        public int NextRing { get; private set; }
        public bool IsComplete => NextRing >= _rings.Length;

        /// <summary>Advance against the ship's current position. Returns true the frame a ring is
        /// newly passed (the runtime logs / dings on true).</summary>
        public bool Advance(Vector3 shipPosition)
        {
            if (IsComplete) return false;
            if ((shipPosition - _rings[NextRing]).sqrMagnitude > _radiusSqr) return false;
            NextRing++;
            return true;
        }
    }
}
