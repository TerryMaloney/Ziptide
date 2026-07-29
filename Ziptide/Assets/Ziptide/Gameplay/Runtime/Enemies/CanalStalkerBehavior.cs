using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE CANAL STALKER (⚖ Terry, bestiary tox_canal_stalker_01) — the amphibious thing that lives
    /// in the toxic canals and plays against your HULL, never against you on foot. Ride one, a low
    /// wake runs alongside and nothing happens. Ride two, it shoulders the boat: a real nudge, a
    /// deep thud, and the skiff yaws about ten degrees. Ride three, if you keep loitering, it puts
    /// itself across the channel and waits — then gives way.
    ///
    /// It is never lethal and never leaves the water: on land it simply is not there, which is why
    /// the canals are the only place it exists. Escalation lives in <see cref="CanalStalkerCore"/>;
    /// this is the body and the water it stays in. Logs ZIPTIDE: STALKER stage=… .
    /// </summary>
    public class CanalStalkerBehavior : CreatureBehaviorBase
    {
        public float swimSpeed = 3.4f;
        public float waterLevel = -0.35f;   // the canal surface the wake rides at

        private static readonly Color HideColor = new Color(0.18f, 0.26f, 0.20f);
        private static readonly Color StripeColor = new Color(0.85f, 0.72f, 0.18f);

        private Transform _boat;
        private StalkerStage _stage = StalkerStage.Absent;
        private float _linger;
        private float _blockStarted = float.NegativeInfinity;
        private bool _stunned;
        private float _stunUntil;

        // How many separate trips the player has taken through this stretch. Static because the
        // creature can respawn between rides and the escalation is about the PLAYER's history with
        // it, not about one instance's lifetime.
        private static int _rideCount;
        private static bool _rideOpen;

        protected override void BuildVisuals()
        {
            // Low and long: what you see is a back and a wake, not a face. The hazard stripes are
            // the only bright thing on it, so the water reads as "something is under there".
            MakePart("Back", PrimitiveType.Capsule, Vector3.zero,
                new Vector3(0.34f, 0.62f, 0.34f), HideColor, keepCollider: true);
            MakePart("Head", PrimitiveType.Capsule, new Vector3(0f, 0f, 0.75f),
                new Vector3(0.24f, 0.28f, 0.24f), HideColor * 0.85f);
            MakePart("Stripe_L", PrimitiveType.Cube, new Vector3(-0.16f, 0.08f, 0.1f),
                new Vector3(0.05f, 0.04f, 0.9f), StripeColor);
            MakePart("Stripe_R", PrimitiveType.Cube, new Vector3(0.16f, 0.08f, 0.1f),
                new Vector3(0.05f, 0.04f, 0.9f), StripeColor);
            MakePart("Tail", PrimitiveType.Cube, new Vector3(0f, 0f, -0.85f),
                new Vector3(0.12f, 0.10f, 0.7f), HideColor * 0.7f);
        }

        protected override void Tick(float dt, float dist)
        {
            if (_stunned && Time.time >= _stunUntil) _stunned = false;

            _boat = FindBoat();
            bool present = _boat != null;

            if (present)
            {
                if (!_rideOpen) { _rideOpen = true; _rideCount++; }
                _linger += dt;
            }
            else
            {
                _rideOpen = false;
                _linger = 0f;
            }

            var stage = CanalStalkerCore.Stage(_rideCount, _linger, present, _stunned);
            if (stage != _stage)
            {
                _stage = stage;
                if (stage == StalkerStage.Block) _blockStarted = Time.time;
                Debug.Log("ZIPTIDE: STALKER stage=" + stage + " ride=" + _rideCount
                    + " linger=" + _linger.ToString("F0"));
            }

            switch (_stage)
            {
                case StalkerStage.Absent: Patrol(dt); break;
                case StalkerStage.Shadow: Escort(dt); break;
                case StalkerStage.Bump: Shoulder(dt); break;
                case StalkerStage.Block: Block(dt); break;
            }

            // Stay in the water, whatever it is doing. A stalker on a plaza is a different animal.
            Vector3 p = transform.position;
            transform.position = new Vector3(p.x, Runtime.HomePos.y + waterLevel, p.z);
        }

        /// <summary>Nobody aboard: drift around its home stretch.</summary>
        private void Patrol(float dt)
        {
            Vector3 home = Runtime.HomePos + Vector3.up * waterLevel;
            Vector3 wander = home + new Vector3(Mathf.Sin(Time.time * 0.25f) * 4f, 0f,
                Mathf.Cos(Time.time * 0.19f) * 4f);
            CollideMove(Leashed(Vector3.MoveTowards(transform.position, wander, swimSpeed * 0.45f * dt)));
        }

        /// <summary>THE SHADOW: hold station off the boat's flank. Never closes.</summary>
        private void Escort(float dt)
        {
            if (_boat == null) { Patrol(dt); return; }
            Vector3 flank = _boat.position - _boat.right * CanalStalkerCore.ShadowDistance;
            flank.y = Runtime.HomePos.y + waterLevel;
            CollideMove(Leashed(Vector3.MoveTowards(transform.position, flank, swimSpeed * dt)));
            FaceToward(_boat.position, dt, 3f);
        }

        /// <summary>THE BUMP: close, shoulder the hull, then peel off. A warning with a thud.</summary>
        private void Shoulder(float dt)
        {
            if (_boat == null) { Patrol(dt); return; }
            Vector3 target = _boat.position;
            target.y = Runtime.HomePos.y + waterLevel;
            CollideMove(Leashed(Vector3.MoveTowards(transform.position, target, swimSpeed * 1.25f * dt)));
            FaceToward(_boat.position, dt, 6f);

            if (Vector3.Distance(transform.position, _boat.position) > 1.6f) return;

            // The nudge itself: a small yaw the vehicle's assist recovers from. Deliberately not a
            // shove — throwing the player's ride would be a comfort event, not a warning.
            _boat.rotation *= Quaternion.Euler(0f, CanalStalkerCore.BumpYawDegrees * dt * 2f, 0f);
        }

        /// <summary>THE BLOCK: sit across the channel ahead, then yield. It never traps you.</summary>
        private void Block(float dt)
        {
            if (_boat == null) { Patrol(dt); return; }

            if (Time.time - _blockStarted > CanalStalkerCore.BlockYieldSeconds)
            {
                Escort(dt);   // it made its point
                return;
            }

            Vector3 ahead = _boat.position + _boat.forward * 6f;
            ahead.y = Runtime.HomePos.y + waterLevel;
            CollideMove(Leashed(Vector3.MoveTowards(transform.position, ahead, swimSpeed * 1.4f * dt)));
            FaceToward(_boat.position, dt, 8f);
        }

        /// <summary>The player's answer works: a stunned stalker drops back to escorting.</summary>
        public override void OnStunned(float seconds)
        {
            _stunned = true;
            _stunUntil = Time.time + Mathf.Max(seconds, 1f);
            Debug.Log("ZIPTIDE: STALKER stunned=" + seconds.ToString("F1"));
        }

        /// <summary>
        /// The boat, if the player is actually aboard one nearby. Found by component so this file
        /// stays out of the Ship assembly; a moored skiff with nobody in it is not a target — the
        /// stalker plays against a PILOT, not against parked furniture.
        /// </summary>
        private Transform FindBoat()
        {
            if (Player == null) return null;
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                if (behaviour.GetType().Name != "SkiffWaterLockRuntime") continue;
                float toPlayer = Vector3.Distance(behaviour.transform.position, Player.position);
                if (toPlayer > 3f) continue;                    // player is not aboard
                if (Vector3.Distance(behaviour.transform.position, transform.position) > leashRadius * 2f)
                    continue;                                   // not in this stalker's stretch
                return behaviour.transform;
            }
            return null;
        }

        /// <summary>Test seam: forget the player's ride history (the escalation is session-scoped).</summary>
        public static void ResetRideHistory()
        {
            _rideCount = 0;
            _rideOpen = false;
        }
    }
}
