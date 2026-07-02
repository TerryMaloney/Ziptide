using System;

namespace Ziptide.Multiplayer.Bots
{
    public enum BotState { Patrol, Hunt, Engage, TakeCover, Peek, Reposition, Retreat, Rush }

    /// <summary>What the scene tells the brain each tick (built by the PvpBot scene layer).</summary>
    [Serializable]
    public struct BotPerception
    {
        public float Now;             // injected clock — the brain never reads wall time
        public Vec3 MyPos;
        public int MyHealth;
        public bool WeaponReady;      // WeaponCharge.CanFire

        public bool CanSeeTarget;     // LOS linecast result
        public Vec3 TargetPos;
        public Vec3 TargetVel;        // for leading

        public bool IncomingThreat;   // a PvpBolt/dart heading roughly at me
        public Vec3 ThreatVel;

        public bool HasCover;         // a cover point exists that breaks LOS to the target
        public Vec3 NearestCoverPos;

        public bool AtMoveTarget;     // within arrive distance of the last decision's MoveTarget
        public bool HeardFire;        // a shot fired somewhere I can't see
        public Vec3 HeardFireAt;
        public Vec3 PatrolPoint;      // scene-suggested next waypoint when patrolling
    }

    /// <summary>What the brain wants done — the scene translates this into CollideMove/aim/fire.</summary>
    [Serializable]
    public struct BotDecision
    {
        public BotState State;
        public Vec3 MoveTarget;
        public Vec3 FaceTarget;       // point to look toward
        public float StrafeSign;      // -1/0/+1 lateral drift while moving
        public bool WantFire;         // begin the telegraph→fire cycle
        public Vec3 AimPoint;         // where to fire (lead + error applied)
        public bool WantDodge;        // burst-step now
        public Vec3 DodgeDir;
    }

    /// <summary>
    /// The pure decision core (design: docs/design/PVP_ARENA_AAA.md §A1). Deterministic: same profile +
    /// seed + perception sequence → the same decisions, so competence is CI-provable. The scene layer
    /// (PvpBot) supplies perception and executes decisions; difficulty lives entirely in
    /// <see cref="BotProfileData"/>. Telegraphs stay scene-side and are NEVER removed (kid-readable law).
    /// </summary>
    public sealed class BotBrain
    {
        private readonly BotProfileData _p;
        private readonly BotRng _rng;

        public BotState State { get; private set; } = BotState.Patrol;

        // Timers / memory
        private float _reactAt = float.MaxValue;   // when the current sighting becomes actionable
        private bool _sighted;                      // currently tracking a live sighting
        private Vec3 _lastKnownPos;
        private bool _hasLkp;
        private float _searchUntil;
        private float _stateUntil;                  // generic state deadline (peek/hide/reposition)
        private float _strafeFlipAt;
        private float _strafeSign = 1f;
        private float _engageSince;
        private float _lastThreatAt = float.MinValue; // dodge once per distinct threat window
        private bool _retreating;

        public BotBrain(BotProfileData profile, int seed)
        {
            _p = profile;
            _rng = new BotRng(seed);
        }

        /// <summary>Scene calls this when the bot takes a hit — may break it toward cover.</summary>
        public void NotifyDamaged(float now)
        {
            if (_retreating) return;
            if (_rng.NextFloat() < _p.CoverDiscipline)
            {
                State = BotState.TakeCover;
                _stateUntil = 0f; // recomputed on arrival
            }
        }

        public BotDecision Tick(in BotPerception p)
        {
            var d = new BotDecision { State = State, StrafeSign = 0f };

            // ── Sighting / reaction-time model ──────────────────────────────
            if (p.CanSeeTarget)
            {
                if (!_sighted) { _sighted = true; _reactAt = p.Now + _p.ReactionSeconds; }
                _lastKnownPos = p.TargetPos; _hasLkp = true;
            }
            else _sighted = false;
            bool reacted = p.CanSeeTarget && p.Now >= _reactAt;

            // ── Global preemptions ──────────────────────────────────────────
            // Retreat is sticky until respawn (scene resets the brain on revive).
            if (!_retreating && _p.RetreatBelowHP > 0 && p.MyHealth <= _p.RetreatBelowHP)
                _retreating = true;
            if (_retreating) State = BotState.Retreat;

            // Dodge: roll once per threat window (a new threat >0.6s after the last).
            if (p.IncomingThreat && p.Now - _lastThreatAt > 0.6f)
            {
                _lastThreatAt = p.Now;
                if (_rng.NextFloat() < _p.DodgeChance)
                {
                    d.WantDodge = true;
                    d.DodgeDir = p.ThreatVel.FlatPerp() * (_rng.NextFloat() < 0.5f ? 1f : -1f);
                }
            }

            // ── State machine ───────────────────────────────────────────────
            float dist = Vec3.FlatDistance(p.MyPos, p.TargetPos);
            switch (State)
            {
                case BotState.Patrol:
                    d.MoveTarget = p.PatrolPoint;
                    d.FaceTarget = p.PatrolPoint;
                    if (reacted) Enter(BotState.Engage, p.Now);
                    else if (p.HeardFire)
                    {
                        _lastKnownPos = p.HeardFireAt; _hasLkp = true;
                        Enter(BotState.Hunt, p.Now);
                        // React THIS tick — the decision that hears the shot already moves on it
                        // (BotBrainTests.HeardFire_PullsPatrolIntoHunt).
                        d.MoveTarget = p.HeardFireAt;
                        d.FaceTarget = p.HeardFireAt;
                    }
                    break;

                case BotState.Hunt:
                    d.MoveTarget = _hasLkp ? _lastKnownPos : p.PatrolPoint;
                    d.FaceTarget = d.MoveTarget;
                    if (reacted) Enter(BotState.Engage, p.Now);
                    else if (p.AtMoveTarget)
                    {
                        if (_searchUntil <= 0f) _searchUntil = p.Now + _p.SearchSeconds;
                        else if (p.Now >= _searchUntil) { _searchUntil = 0f; _hasLkp = false; Enter(BotState.Patrol, p.Now); }
                    }
                    break;

                case BotState.Engage:
                    if (!p.CanSeeTarget) { Enter(BotState.Hunt, p.Now); break; }
                    if (dist <= _p.RushRange) { Enter(BotState.Rush, p.Now); break; }
                    if (!p.WeaponReady && p.HasCover && _rng.NextFloat() < _p.CoverDiscipline * 0.1f)
                    { Enter(BotState.TakeCover, p.Now); break; }
                    if (p.Now - _engageSince >= _p.RepositionEvery && p.Now >= _stateUntil)
                    { Enter(BotState.Reposition, p.Now); _stateUntil = p.Now + 2.5f; break; }

                    // Hold the band around the standoff; strafe with periodic flips.
                    if (p.Now >= _strafeFlipAt)
                    { _strafeSign = -_strafeSign; _strafeFlipAt = p.Now + _p.StrafeFlipSeconds * (0.75f + _rng.NextFloat() * 0.5f); }
                    d.StrafeSign = _strafeSign;
                    if (dist > _p.EngageStandoff + _p.EngageBand) d.MoveTarget = p.TargetPos;                       // close in
                    else if (dist < _p.EngageStandoff - _p.EngageBand)                                              // back off
                        d.MoveTarget = p.MyPos + (p.MyPos - p.TargetPos).Normalized() * 2f;
                    else d.MoveTarget = p.MyPos;                                                                     // hold + strafe
                    d.FaceTarget = p.TargetPos;
                    if (reacted && p.WeaponReady && dist <= _p.FireRange)
                    { d.WantFire = true; d.AimPoint = ComputeAimPoint(p); }
                    break;

                case BotState.TakeCover:
                    d.MoveTarget = p.HasCover ? p.NearestCoverPos : p.PatrolPoint;
                    d.FaceTarget = p.TargetPos;
                    if (!p.HasCover) { Enter(BotState.Engage, p.Now); break; }
                    if (p.AtMoveTarget)
                    {
                        if (_stateUntil <= 0f) _stateUntil = p.Now + _p.HideSeconds;
                        else if (p.Now >= _stateUntil) { Enter(BotState.Peek, p.Now); _stateUntil = p.Now + _p.PeekSeconds; }
                    }
                    break;

                case BotState.Peek:
                    // Expose toward the target's last position; fire if the peek finds them.
                    d.MoveTarget = p.MyPos + ((_hasLkp ? _lastKnownPos : p.TargetPos) - p.MyPos).Normalized() * 1.5f;
                    d.FaceTarget = _hasLkp ? _lastKnownPos : p.TargetPos;
                    if (reacted && p.WeaponReady && dist <= _p.FireRange)
                    { d.WantFire = true; d.AimPoint = ComputeAimPoint(p); }
                    if (p.Now >= _stateUntil)
                        Enter(p.CanSeeTarget && dist < _p.EngageStandoff ? BotState.Engage : BotState.TakeCover, p.Now);
                    break;

                case BotState.Reposition:
                    // Flank: swing ±60° around the target at the standoff radius.
                    {
                        Vec3 fromTarget = (p.MyPos - p.TargetPos).RotatedY(_rng.NextFloat() < 0.5f ? 60f : -60f);
                        d.MoveTarget = p.TargetPos + fromTarget.Normalized() * _p.EngageStandoff;
                        d.FaceTarget = p.TargetPos;
                        if (p.AtMoveTarget || p.Now >= _stateUntil) Enter(BotState.Engage, p.Now);
                    }
                    break;

                case BotState.Retreat:
                    d.MoveTarget = p.HasCover ? p.NearestCoverPos
                        : p.MyPos + (p.MyPos - p.TargetPos).Normalized() * 4f;
                    d.FaceTarget = p.TargetPos;
                    if (reacted && p.WeaponReady && p.CanSeeTarget && dist <= _p.FireRange)
                    { d.WantFire = true; d.AimPoint = ComputeAimPoint(p); } // fights while falling back
                    break;

                case BotState.Rush:
                    d.MoveTarget = p.TargetPos;
                    d.FaceTarget = p.TargetPos;
                    if (reacted && p.WeaponReady) { d.WantFire = true; d.AimPoint = ComputeAimPoint(p); }
                    if (dist > _p.RushRange * 2f) Enter(BotState.Engage, p.Now);
                    break;
            }

            d.State = State;
            return d;
        }

        /// <summary>Lead (if the profile allows) + a deterministic error cone. Bolt speed 5 m/s (PvpBolt).</summary>
        public Vec3 ComputeAimPoint(in BotPerception p, float boltSpeed = 5f)
        {
            Vec3 aim = p.TargetPos;
            if (_p.LeadTargets && boltSpeed > 0.1f)
            {
                float t = Vec3.FlatDistance(p.MyPos, p.TargetPos) / boltSpeed;
                aim += p.TargetVel * t;
            }
            if (_p.AimErrorDegrees > 0f)
            {
                // Error scales with distance: a cone of AimErrorDegrees ≈ dist·tan(err) lateral meters.
                float dist = Vec3.FlatDistance(p.MyPos, aim);
                float maxOff = dist * (float)Math.Tan(_p.AimErrorDegrees * Math.PI / 180.0);
                Vec3 lateral = (aim - p.MyPos).FlatPerp();
                aim += lateral * (_rng.NextSigned() * maxOff);
                aim.Y += _rng.NextSigned() * maxOff * 0.5f;
            }
            return aim;
        }

        private void Enter(BotState s, float now)
        {
            State = s;
            _searchUntil = 0f;
            if (s == BotState.Engage) _engageSince = now;
            if (s == BotState.TakeCover) _stateUntil = 0f;
        }
    }
}
