using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>The ship's flight state — deliberately has NO roll field (comfort law: roll never
    /// exists, so no code path can add it). Serializable for save/netcode later.</summary>
    [Serializable]
    public struct FlightState
    {
        public Vector3 position;
        public float yawDeg;    // snap-turned only (comfort)
        public float pitchDeg;  // slow, clamped
        public float speed;     // m/s along Forward
    }

    /// <summary>Per-ship tuning (ShipDefinition feeds this later; defaults are the comfort-reviewed
    /// v1 numbers from docs/design/SPACEFLIGHT_PHYSICS.md).</summary>
    [Serializable]
    public struct FlightParams
    {
        public float maxSpeed;        // m/s
        public float accel;           // m/s² toward throttle target
        public float pitchRateDeg;    // deg/s (slow world tilt)
        public float pitchClampDeg;   // ±
        public float yawSnapDeg;      // snap increment (never smooth yaw)
        public float laneRadius;      // bounded play space — soft wall

        public static FlightParams Default => new FlightParams
        {
            maxSpeed = 40f, accel = 12f, pitchRateDeg = 25f, pitchClampDeg = 35f,
            yawSnapDeg = 30f, laneRadius = 1800f, // < 2km: the floating-origin trigger can never fire
        };
    }

    /// <summary>
    /// P4b — the PURE flight core (SPACEFLIGHT_PHYSICS rails; scene translator renders WORLD motion
    /// around a static rig and applies yaw as snaps — this class never sees a camera). Laws pinned by
    /// FlightModelTests: deterministic · no roll by construction · pitch hard-clamped · yaw only via
    /// discrete snaps · the lane is a soft-walled sphere the state can never exit · zero throttle
    /// decays to rest. Comfort is enforced in the MATH, so no future translator can break it.
    /// </summary>
    public static class FlightModel
    {
        /// <summary>Ship-forward for a state (yaw about +Y, then pitch about local X). No roll.</summary>
        public static Vector3 Forward(FlightState s)
            => Quaternion.Euler(-s.pitchDeg, s.yawDeg, 0f) * Vector3.forward;

        /// <summary>Advance one frame. throttle01 ∈ [0,1], pitchInput ∈ [-1,1]. Deterministic.</summary>
        public static FlightState Tick(FlightState s, FlightParams p, float throttle01, float pitchInput, float dt)
        {
            if (dt <= 0f || float.IsNaN(dt)) return s;
            throttle01 = Mathf.Clamp01(throttle01);
            pitchInput = Mathf.Clamp(pitchInput, -1f, 1f);

            float targetSpeed = throttle01 * p.maxSpeed;
            s.speed = Mathf.MoveTowards(s.speed, targetSpeed, p.accel * dt);

            s.pitchDeg = Mathf.Clamp(s.pitchDeg + pitchInput * p.pitchRateDeg * dt,
                -p.pitchClampDeg, p.pitchClampDeg);

            s.position += Forward(s) * (s.speed * dt);

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
    }
}
