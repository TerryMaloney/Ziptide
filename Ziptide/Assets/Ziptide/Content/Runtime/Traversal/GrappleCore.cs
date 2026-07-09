using System;

namespace Ziptide.Content.Traversal
{
    /// <summary>
    /// GRAPPLE reel — the last 1.4 traversal verb, as pure kinematics. Fire a hook at an anchor and
    /// the rig is REELED along the straight line toward it: ease-in to a comfort-capped reel speed,
    /// monotonic progress (a grapple that stalls mid-air is a fail state, not a feature), arrival at
    /// a stop margin short of the anchor so you land beside the ledge, not inside it.
    ///
    /// DELIBERATELY A LINEAR REEL, NOT A PENDULUM: swing physics is the single most nauseating
    /// traversal in VR (uncontrolled lateral acceleration) — the Meta comfort guidance that capped the
    /// zipline caps harder here. Straight-line pull at bounded speed reads as elevator-diagonal, which
    /// devices tolerate well. Range gating lives here too so the scene layer can't fire cross-map.
    /// </summary>
    public sealed class GrappleReel
    {
        public TVec3 From { get; }
        public TVec3 Anchor { get; }
        public float Length { get; }
        public float ReelSpeed { get; }
        public float StopMargin { get; }

        public float Progress { get; private set; }   // 0 at fire point → 1 at the stop point
        public float Speed { get; private set; }
        public bool Arrived => Progress >= 1f;

        public const float MaxRangeDefault = 22f;     // the scene layer's fire gate
        private const float EaseInSeconds = 0.35f;    // spool-up so the pull starts soft, not a yank

        private float _age;

        /// <summary>Null when the anchor is out of range or degenerate — the scene layer treats a null
        /// reel as "the hook didn't catch" (a miss, never an exception).</summary>
        public static GrappleReel TryFire(TVec3 from, TVec3 anchor,
            float maxRange = MaxRangeDefault, float reelSpeed = 7f, float stopMargin = 1.1f)
        {
            float dist = (anchor - from).Length;
            if (dist < stopMargin * 1.5f || dist > Math.Max(1f, maxRange)) return null;
            return new GrappleReel(from, anchor, reelSpeed, stopMargin);
        }

        private GrappleReel(TVec3 from, TVec3 anchor, float reelSpeed, float stopMargin)
        {
            From = from;
            Anchor = anchor;
            StopMargin = Math.Max(0.2f, stopMargin);
            Length = Math.Max(0.01f, (anchor - from).Length - StopMargin);
            ReelSpeed = Math.Max(0.5f, reelSpeed);
        }

        /// <summary>Advance the reel. Returns the rig position for this instant.</summary>
        public TVec3 Step(float dt)
        {
            if (dt <= 0f || Arrived) return Position;
            _age += dt;
            // Ease-in: speed ramps over EaseInSeconds to the cap, then holds. Never decays —
            // monotonic progress is the contract.
            float ramp = Math.Min(1f, _age / EaseInSeconds);
            Speed = ReelSpeed * ramp;
            Progress += (Speed * dt) / Length;
            if (Progress > 1f) Progress = 1f;
            return Position;
        }

        /// <summary>The stop point sits StopMargin short of the anchor along the reel line.</summary>
        public TVec3 Position
        {
            get
            {
                var dir = Anchor - From;
                float full = dir.Length;
                if (full < 1e-5f) return From;
                var unit = dir * (1f / full);
                return From + unit * (Length * Math.Min(1f, Progress));
            }
        }

        /// <summary>Speed normalized against the cap — the ComfortCore vignette feed.</summary>
        public float Speed01 => ReelSpeed <= 0f ? 0f : Math.Min(1f, Speed / ReelSpeed);
    }
}
