using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>The ship's flight state. Roll exists ONLY as a transient barrel-roll maneuver
    /// (comfort law v2, Terry 2026-07-09: roll never RESTS — it always completes back to exactly
    /// level, and no input can hold the ship banked). Serializable for save/netcode later.</summary>
    [Serializable]
    public struct FlightState
    {
        public Vector3 position;
        public float yawDeg;       // snap-turned only (comfort)
        public float pitchDeg;     // slow, clamped
        public float speed;        // m/s along Forward — SIGNED: negative = reverse
        public float rollDeg;      // non-zero ONLY mid barrel roll; always returns to exactly 0
        public int rollDirection;  // 0 = level; ±1 = a barrel roll is in progress
    }

    /// <summary>Per-ship tuning (ShipDefinition feeds this later; defaults are the comfort-reviewed
    /// v1 numbers from docs/design/SPACEFLIGHT_PHYSICS.md).</summary>
    [Serializable]
    public struct FlightParams
    {
        public float maxSpeed;        // m/s forward
        public float accel;           // m/s² toward throttle target
        public float pitchRateDeg;    // deg/s (slow world tilt)
        public float pitchClampDeg;   // ±
        public float yawSnapDeg;      // snap increment (never smooth yaw)
        public float laneRadius;      // bounded play space — soft wall
        public float boostMultiplier; // held-boost speed factor (like sprint over run)
        public float reverseFraction; // reverse max as a fraction of forward max
        public float rollRateDeg;     // barrel-roll speed (deg/s; 360°/this = the maneuver length)
        public float strafeFraction;  // lateral (left-stick X) max as a fraction of forward max

        public static FlightParams Default => new FlightParams
        {
            maxSpeed = 40f, accel = 12f, pitchRateDeg = 25f, pitchClampDeg = 35f,
            yawSnapDeg = 30f, laneRadius = 1800f, // < 2km: the floating-origin trigger can never fire
            boostMultiplier = 1.8f, reverseFraction = 0.4f,
            rollRateDeg = 420f, // a full barrel roll in ~0.86s — a dodge, not a sustained bank
            strafeFraction = 0.3f, // gentle slide — fills the Xbox-dead left-stick X axis
        };
    }

    /// <summary>
    /// P4b — the PURE flight core (SPACEFLIGHT_PHYSICS rails; scene translator renders WORLD motion
    /// around a static rig and applies yaw as snaps — this class never sees a camera). Laws pinned by
    /// FlightModelTests: deterministic · roll never rests (barrel roll only, self-completing to
    /// exactly level) · pitch hard-clamped · yaw only via discrete snaps · reverse capped to a
    /// fraction of forward · boost multiplies, never bypasses, the ramp · the lane is a soft-walled
    /// sphere the state can never exit · zero throttle decays to rest. Comfort is enforced in the
    /// MATH, so no future translator can break it.
    /// </summary>
    public static class FlightModel
    {
        /// <summary>Ship-forward for a state (yaw about +Y, then pitch about local X). Roll spins
        /// AROUND this axis, so it never changes where the ship is going.</summary>
        public static Vector3 Forward(FlightState s)
            => Quaternion.Euler(-s.pitchDeg, s.yawDeg, 0f) * Vector3.forward;

        /// <summary>Full visual orientation including any in-flight barrel roll. Translators render
        /// the world with the INVERSE of this.</summary>
        public static Quaternion Orientation(FlightState s)
            => Quaternion.Euler(-s.pitchDeg, s.yawDeg, -s.rollDeg);

        /// <summary>Advance one frame (no boost/strafe). Kept for existing callers/tests.</summary>
        public static FlightState Tick(FlightState s, FlightParams p, float throttle, float pitchInput, float dt)
            => Tick(s, p, throttle, pitchInput, 0f, false, dt);

        /// <summary>Advance one frame (no strafe). Kept for existing callers/tests.</summary>
        public static FlightState Tick(FlightState s, FlightParams p, float throttle, float pitchInput,
            bool boost, float dt)
            => Tick(s, p, throttle, pitchInput, 0f, boost, dt);

        /// <summary>Advance one frame. throttle ∈ [-1,1] (negative = reverse, capped by
        /// reverseFraction), pitchInput ∈ [-1,1], strafe ∈ [-1,1] (pure lateral slide, capped by
        /// strafeFraction — translation only, so it is rotation-free and comfort-safe), boost
        /// multiplies the speed target and ramp while held — releasing it decays back to the
        /// unboosted cap. Deterministic.</summary>
        public static FlightState Tick(FlightState s, FlightParams p, float throttle, float pitchInput,
            float strafe, bool boost, float dt)
        {
            if (dt <= 0f || float.IsNaN(dt)) return s;
            throttle = Mathf.Clamp(throttle, -1f, 1f);
            pitchInput = Mathf.Clamp(pitchInput, -1f, 1f);
            strafe = Mathf.Clamp(strafe, -1f, 1f);

            float boostFactor = boost ? Mathf.Max(1f, p.boostMultiplier) : 1f;
            float cap = throttle >= 0f
                ? p.maxSpeed
                : p.maxSpeed * Mathf.Clamp01(p.reverseFraction);
            float targetSpeed = throttle * cap * boostFactor;
            s.speed = Mathf.MoveTowards(s.speed, targetSpeed, p.accel * boostFactor * dt);

            s.pitchDeg = Mathf.Clamp(s.pitchDeg + pitchInput * p.pitchRateDeg * dt,
                -p.pitchClampDeg, p.pitchClampDeg);

            // Barrel roll: advances on its own and ALWAYS lands back on exactly 0 — there is no
            // code path that leaves the ship banked (comfort law v2).
            if (s.rollDirection != 0)
            {
                float rollRate = p.rollRateDeg > 0f ? p.rollRateDeg : FlightParams.Default.rollRateDeg;
                s.rollDeg += s.rollDirection * rollRate * dt;
                if (s.rollDeg * s.rollDirection >= 360f)
                {
                    s.rollDeg = 0f;
                    s.rollDirection = 0;
                }
            }

            s.position += Forward(s) * (s.speed * dt);

            // Strafe: a stateless lateral slide along the ship's right axis (same direct-velocity
            // model as walking locomotion — pure translation, never a rotation).
            if (strafe != 0f)
            {
                Vector3 right = Quaternion.Euler(-s.pitchDeg, s.yawDeg, 0f) * Vector3.right;
                s.position += right * (strafe * p.maxSpeed * Mathf.Clamp01(p.strafeFraction) * dt);
            }

            // Soft wall: the lane is a sphere. Beyond it, snap back to the surface and bleed speed —
            // you bounce off the edge of space gently instead of flying to float-jitter land.
            float r = s.position.magnitude;
            if (r > p.laneRadius && r > 0.001f)
            {
                s.position *= p.laneRadius / r;
                s.speed *= 0.5f;
            }
            return s;
        }

        /// <summary>Comfort yaw: a discrete snap (±1 step). The ONLY way heading changes.</summary>
        public static FlightState SnapYaw(FlightState s, FlightParams p, int direction)
        {
            if (direction == 0) return s;
            s.yawDeg = Mathf.Repeat(s.yawDeg + Mathf.Sign(direction) * p.yawSnapDeg, 360f);
            return s;
        }

        /// <summary>Start a barrel roll (±1 = roll direction). One at a time: mid-roll requests are
        /// ignored, so mashing the button can never chain into a washing machine.</summary>
        public static FlightState StartBarrelRoll(FlightState s, int direction)
        {
            if (direction == 0 || s.rollDirection != 0) return s;
            s.rollDirection = direction > 0 ? 1 : -1;
            return s;
        }
    }
}
