using System;

namespace Ziptide.Content.Traversal
{
    /// <summary>Minimal pure 3-vector for traversal math (no UnityEngine → EditMode-testable). The
    /// scene translator converts to/from UnityEngine.Vector3 at the boundary.</summary>
    public struct TVec3
    {
        public float X, Y, Z;
        public TVec3(float x, float y, float z) { X = x; Y = y; Z = z; }

        public static TVec3 operator +(TVec3 a, TVec3 b) => new TVec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static TVec3 operator -(TVec3 a, TVec3 b) => new TVec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static TVec3 operator *(TVec3 a, float s) => new TVec3(a.X * s, a.Y * s, a.Z * s);

        public float Length => (float)Math.Sqrt((double)(X * X + Y * Y + Z * Z));
        public static TVec3 Lerp(TVec3 a, TVec3 b, float t) => a + (b - a) * t;
    }

    /// <summary>
    /// THE ZIPLINE ride — the game's namesake traversal, as pure kinematics (Hardwiring Phase 1.4).
    /// A rider clipped to a cable A→B accelerates under the along-cable component of gravity, is bled by
    /// drag, and is HARD-CAPPED at a comfort speed (fast VR translation nauseates — the cap is the
    /// comfort law, not a balance knob). Straight taut line (catenary sag is a visual-only refinement
    /// for the scene). All state advances via <see cref="Step"/>; the translator reads
    /// <see cref="Position"/> each frame and feeds <see cref="Speed01"/> to ComfortCore for the vignette.
    ///
    /// Design intent (VERTICAL_AND_CAVERN_WORLDS §traversal + VR_TECHNIQUE_RESEARCH comfort notes):
    /// riders always make progress even on a flat line (you push off — <see cref="KickSpeed"/>), never
    /// exceed <see cref="MaxSpeed"/>, and always arrive (monotonic progress) so a rider can't stall
    /// mid-cable over a chasm.
    /// </summary>
    public sealed class ZiplineRide
    {
        public TVec3 A { get; }
        public TVec3 B { get; }
        public float Length { get; }
        public float MaxSpeed { get; }
        public float KickSpeed { get; }
        public float Gravity { get; }
        public float Drag { get; }

        public float Progress { get; private set; } // 0 at A → 1 at B
        public float Speed { get; private set; }     // m/s along the cable
        public bool Arrived => Progress >= 1f;

        /// <param name="maxSpeed">Comfort cap on ride speed (m/s). Clamped ≥ kickSpeed.</param>
        /// <param name="kickSpeed">Minimum ride speed — the push-off, so flat lines still move.</param>
        /// <param name="gravity">Downward accel magnitude (positive; e.g. 9.81).</param>
        /// <param name="drag">Per-second speed bleed fraction (0 = frictionless).</param>
        public ZiplineRide(TVec3 a, TVec3 b, float maxSpeed = 8f, float kickSpeed = 2f,
                           float gravity = 9.81f, float drag = 0.15f)
        {
            A = a; B = b;
            Length = (b - a).Length;
            KickSpeed = Math.Max(0.01f, kickSpeed);
            MaxSpeed = Math.Max(MaxSpeedFloor(maxSpeed), KickSpeed);
            Gravity = Math.Max(0f, gravity);
            Drag = Math.Max(0f, drag);
            Speed = KickSpeed; // you launch with a push-off, never from a dead stop
        }

        private static float MaxSpeedFloor(float v) => v <= 0f ? 8f : v;

        /// <summary>Advance the ride by dt seconds. Returns the current world position.</summary>
        public TVec3 Step(float dt)
        {
            if (dt <= 0f || Length <= 1e-4f) { Progress = 1f; Speed = 0f; return B; }
            if (Arrived) { Speed = 0f; return B; }

            // Along-cable gravity component: drop over run. B below A ⇒ positive accel.
            float drop = A.Y - B.Y;                 // >0 means downhill
            float alongG = Gravity * (drop / Length); // component of g projected onto the cable
            Speed += alongG * dt;
            Speed -= Speed * Drag * dt;             // drag bleed
            if (Speed < KickSpeed) Speed = KickSpeed;   // never stall on the wire
            if (Speed > MaxSpeed) Speed = MaxSpeed;     // COMFORT CAP — the hard law

            Progress += (Speed * dt) / Length;
            if (Progress > 1f) Progress = 1f;
            return Position;
        }

        public TVec3 Position => TVec3.Lerp(A, B, Progress);

        /// <summary>Speed normalized to [0,1] against the cap — the value ComfortCore.TargetAperture wants.</summary>
        public float Speed01 => MaxSpeed <= 0f ? 0f : Math.Min(1f, Speed / MaxSpeed);
    }
}
