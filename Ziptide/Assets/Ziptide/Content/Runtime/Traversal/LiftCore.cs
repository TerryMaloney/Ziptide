using System;

namespace Ziptide.Content.Traversal
{
    /// <summary>
    /// ELEVATOR / LIFT as a pure clock (Hardwiring 1.4 — the third traversal verb). A platform cycles
    /// through ordered stops: dwell at a stop (doors-open time), then travel to the next at a
    /// comfort-capped speed. Deterministic from (stops, speeds, t) — <see cref="Evaluate"/> is a pure
    /// function of elapsed time, so the scene translator can't drift and a late-joining rider computes
    /// the same platform position as everyone else (netcode-friendly for free).
    /// The rider stands ON the platform; the translator delta-translates the rig by the platform's
    /// per-frame delta while the player overlaps it (the zipline/climb law: never parent the rig).
    /// </summary>
    public sealed class LiftCycle
    {
        public TVec3[] Stops { get; }
        public float TravelSpeed { get; }   // m/s between stops — comfort-capped by the caller's data
        public float DwellSeconds { get; }  // pause at each stop

        private readonly float[] _legSeconds;   // travel time per leg i -> i+1 (wraps)
        private readonly float _cycleSeconds;

        public LiftCycle(TVec3[] stops, float travelSpeed = 2f, float dwellSeconds = 3f)
        {
            if (stops == null || stops.Length < 2)
                throw new ArgumentException("a lift needs at least 2 stops");
            Stops = stops;
            TravelSpeed = Math.Max(0.1f, travelSpeed);
            DwellSeconds = Math.Max(0f, dwellSeconds);

            _legSeconds = new float[stops.Length];
            float total = 0f;
            for (int i = 0; i < stops.Length; i++)
            {
                float d = (stops[(i + 1) % stops.Length] - stops[i]).Length;
                _legSeconds[i] = d / TravelSpeed;
                total += DwellSeconds + _legSeconds[i];
            }
            _cycleSeconds = Math.Max(0.01f, total);
        }

        public float CycleSeconds => _cycleSeconds;

        /// <summary>Platform position at elapsed time t (t may be any value ≥ 0 — the cycle wraps).
        /// Also reports whether the platform is dwelling (doors open — safe to step on).</summary>
        public TVec3 Evaluate(float t, out bool dwelling, out int atOrNextStop)
        {
            float u = t % _cycleSeconds;
            if (u < 0f) u += _cycleSeconds;

            for (int i = 0; i < Stops.Length; i++)
            {
                if (u < DwellSeconds) { dwelling = true; atOrNextStop = i; return Stops[i]; }
                u -= DwellSeconds;
                if (u < _legSeconds[i])
                {
                    dwelling = false;
                    atOrNextStop = (i + 1) % Stops.Length;
                    float k = _legSeconds[i] <= 1e-5f ? 1f : u / _legSeconds[i];
                    return TVec3.Lerp(Stops[i], Stops[(i + 1) % Stops.Length], k);
                }
                u -= _legSeconds[i];
            }
            dwelling = true; atOrNextStop = 0;
            return Stops[0]; // numeric edge: treat as dwelling at the first stop
        }
    }

    /// <summary>
    /// JUMP PAD launch as pure ballistics (Hardwiring 1.4 — the fourth verb). Given a landing target
    /// and gravity, computes the launch velocity whose arc peaks comfortably above the higher endpoint
    /// and lands on the target. The APEX MARGIN is the comfort knob: a low flat launch reads as a
    /// stumble, a screaming skyward launch reads as nausea — a modest fixed apex above the higher end
    /// (default 2m) makes every pad's arc feel the same regardless of distance. Pure math; the scene
    /// translator applies the velocity to the rig's ballistic mover when the player steps on the pad.
    /// </summary>
    public static class JumpPad
    {
        /// <summary>Launch velocity from `from` to `to` under gravity g (positive, e.g. 9.81), with the
        /// arc apex `apexMargin` meters above the HIGHER of the two points. Always solvable: apex is
        /// above both endpoints by construction, so both square roots are of non-negative numbers.</summary>
        public static TVec3 LaunchVelocity(TVec3 from, TVec3 to, float g = 9.81f, float apexMargin = 2f)
        {
            g = Math.Max(0.01f, g);
            apexMargin = Math.Max(0.5f, apexMargin);
            float apexY = Math.Max(from.Y, to.Y) + apexMargin;

            float riseTime = (float)Math.Sqrt(2.0 * (apexY - from.Y) / g);   // up to the apex
            float fallTime = (float)Math.Sqrt(2.0 * (apexY - to.Y) / g);     // down to the landing
            float flightTime = Math.Max(0.05f, riseTime + fallTime);

            float vy = g * riseTime;                                          // vertical launch speed
            var flat = new TVec3(to.X - from.X, 0f, to.Z - from.Z);
            var horizontal = flat * (1f / flightTime);                        // constant drift over the arc
            return new TVec3(horizontal.X, vy, horizontal.Z);
        }

        /// <summary>Where the launched body is at time t after launch (for the translator's mover
        /// and the tests — same math, one source of truth).</summary>
        public static TVec3 PositionAt(TVec3 from, TVec3 velocity, float g, float t)
        {
            return new TVec3(
                from.X + velocity.X * t,
                from.Y + velocity.Y * t - 0.5f * Math.Max(0.01f, g) * t * t,
                from.Z + velocity.Z * t);
        }
    }
}
