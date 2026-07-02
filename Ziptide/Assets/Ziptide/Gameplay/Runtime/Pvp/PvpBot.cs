using System.Collections.Generic;
using UnityEngine;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Bots;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// AI opponent (index 1) — the scene body for the pure <see cref="BotBrain"/> (M7a A1c; design
    /// docs/design/PVP_ARENA_AAA.md §A1). This class only PERCEIVES (LOS, target velocity, incoming
    /// darts, cover points, waypoints) and EXECUTES (CollideMove, telegraph, visible PvpBolt); every
    /// decision — hunting, cover, flanking, dodging, retreating, leading — lives in the brain, and
    /// difficulty lives in a BotProfileDefinition asset (Resources/Bots/&lt;difficulty&gt;).
    /// Kept from the proven turret bot: IPvpDamageable/IScannable seams, wall-clamped CollideMove,
    /// the ALWAYS-VISIBLE telegraph + dodgeable bolt (kid-readable law), death/revive flow.
    /// Netcode note: PlayerIndex >= 0 identifies real combatants (creatures use -1 — never register them).
    /// </summary>
    public class PvpBot : MonoBehaviour, IPvpDamageable, IScannable
    {
        [Tooltip("BotProfileDefinition asset name under Resources/Bots (rookie/regular/veteran/nightmare).")]
        public string difficulty = "regular";
        public float moveSpeed = 2.5f;
        public float telegraphSeconds = 0.8f;
        [Tooltip("Bolt travel speed — slow enough to see and dodge.")]
        public float boltSpeed = 5f;
        public float reviveDelay = 2.5f;
        [Tooltip("False = stays down after a kill (Horde waves). True = the classic arena revive loop.")]
        public bool autoRevive = true;
        [Tooltip("Combatant index for N-way matches (1–3; the mode director assigns extra bots).")]
        public int playerIndex = 1;
        public LayerMask lineOfSightMask = ~0;

        public int PlayerIndex => playerIndex;
        public bool IsAlive => _combatant != null && _combatant.IsAlive && !_dead;

        // IScannable — the wrist scanner detects the opponent.
        public Transform ScanTransform => transform;
        public ScanKind ScanKind => Ziptide.Gameplay.ScanKind.Enemy;
        public bool ScanActive => IsAlive;

        private PvpCombatant _combatant;
        private BotBrain _brain;
        private BotProfileData _profile;
        private WeaponCharge _charge;
        private Transform _player;
        private PvpPlayer _playerCombatant;
        private Renderer _renderer;
        private Collider _collider;
        private Material _mat;
        private Vector3 _home;

        // Telegraph / fire
        private float _telegraphUntil;
        private bool _telegraphing;
        private Vector3 _lockedAim;      // aim point captured at telegraph start (dodgeable by design)
        private bool _dead;
        private float _reviveAt;

        // Perception caches (heavy scans at low cadence; the brain ticks every frame on cached facts)
        private const float ScanInterval = 0.2f;
        private float _nextScanAt;
        private bool _cachedThreat;
        private Vector3 _cachedThreatVel;
        private bool _cachedHasCover;
        private Vector3 _cachedCoverPos;
        private Vector3 _lastPlayerPos;
        private Vector3 _playerVel;
        private Vector3 _moveTarget;
        private float _dodgeUntil;
        private Vector3 _dodgeDir;

        // Objective magnet (A3 modes): when set, Patrol gravitates here instead of the waypoint ring —
        // KotH bots contest the hill, Fragment bots shadow the fragment. Combat states are unchanged.
        [HideInInspector] public bool hasObjective;
        [HideInInspector] public Vector3 objectivePoint;

        // Slow field (A4 Static Net): movement multiplier while inside a SlowZoneRuntime.
        private float _slowUntil;
        private float _slowFactor = 1f;

        /// <summary>Slow the bot's movement (re-applied while inside a slow zone; expires on its own).</summary>
        public void ApplySlow(float seconds, float factor)
        {
            _slowUntil = Mathf.Max(_slowUntil, Time.time + seconds);
            _slowFactor = Mathf.Clamp(factor, 0.1f, 1f);
        }

        // Nav (baked by ScenePatcherPvP/ScenePatcherArena under __PVP_BOTNAV)
        private readonly List<Vector3> _waypoints = new List<Vector3>();
        private readonly List<Vector3> _coverPoints = new List<Vector3>();
        private int _patrolIndex;

        private static readonly Color LiveColor = new Color(0.85f, 0.3f, 0.25f);
        private static readonly Color TelegraphColor = new Color(1f, 0.85f, 0.2f);
        private static readonly Color DeadColor = new Color(0.2f, 0.2f, 0.22f);

        private void Awake()
        {
            _combatant = new PvpCombatant();
            _home = transform.position;
            _renderer = GetComponentInChildren<Renderer>();
            _collider = GetComponent<Collider>();
            if (_renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null) { _mat = new Material(shader); _renderer.material = _mat; SetColor(LiveColor); }
            }
        }

        private void Start()
        {
            PvpMatchDirector.Instance?.Register(this);
            FindPlayer();
            LoadNav();

            _profile = ResolveProfile(difficulty);
            _brain = NewBrain();
            _charge = new WeaponCharge();
            _moveTarget = _home;
            Debug.Log("ZIPTIDE: PVP_BOT_BRAIN difficulty=" + difficulty);
        }

        private BotBrain NewBrain() =>
            new BotBrain(_profile, seed: (difficulty + name).GetHashCode());

        /// <summary>Live difficulty swap (lobby board) — re-resolves the profile and grows a fresh brain.</summary>
        public void SetDifficulty(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            difficulty = id;
            _profile = ResolveProfile(id);
            if (_brain != null) _brain = NewBrain(); // pre-Start: Start builds it from the new id anyway
        }

        /// <summary>Difficulty as data: the Resources asset wins; code presets are the fallback.</summary>
        public static BotProfileData ResolveProfile(string id)
        {
            var def = Resources.Load<Ziptide.Content.BotProfileDefinition>("Bots/" + id);
            if (def != null) return def.ToData();
            switch (id)
            {
                case "rookie": return BotProfileData.Rookie;
                case "veteran": return BotProfileData.Veteran;
                case "nightmare": return BotProfileData.Nightmare;
                default: return BotProfileData.Regular;
            }
        }

        private void Update()
        {
            if (_dead)
            {
                if (autoRevive && Time.time >= _reviveAt) Revive();
                return;
            }
            if (_player == null) { FindPlayer(); if (_player == null) return; }
            if (_brain == null) return;

            // ── PERCEIVE ─────────────────────────────────────────────────────
            float dt = Time.deltaTime;
            Vector3 playerPos = _player.position;
            _playerVel = dt > 0.0001f ? (playerPos - _lastPlayerPos) / dt : Vector3.zero;
            _lastPlayerPos = playerPos;

            if (Time.time >= _nextScanAt)
            {
                _nextScanAt = Time.time + ScanInterval;
                ScanThreats();
                ScanCover(playerPos);
            }

            bool los = HasLineOfSight();
            var perception = new BotPerception
            {
                Now = Time.time,
                MyPos = ToVec(transform.position),
                MyHealth = _combatant.Health,
                WeaponReady = _charge.CanFire(Time.time) && !_telegraphing,
                CanSeeTarget = los,
                TargetPos = ToVec(playerPos),
                TargetVel = ToVec(_playerVel),
                IncomingThreat = _cachedThreat,
                ThreatVel = ToVec(_cachedThreatVel),
                HasCover = _cachedHasCover,
                NearestCoverPos = ToVec(_cachedCoverPos),
                AtMoveTarget = FlatDist(transform.position, _moveTarget) < 1.2f,
                HeardFire = false, // hook for A3 multi-bot: fire events feed this
                PatrolPoint = ToVec(NextPatrolPoint()),
            };

            // ── DECIDE ───────────────────────────────────────────────────────
            var d = _brain.Tick(perception);

            // ── EXECUTE ──────────────────────────────────────────────────────
            _moveTarget = ToV3(d.MoveTarget);
            if (d.WantDodge) { _dodgeUntil = Time.time + 0.25f; _dodgeDir = ToV3(d.DodgeDir); }

            Vector3 step;
            if (Time.time < _dodgeUntil)
                step = _dodgeDir * 3f;                                  // burst-step beats the bolt
            else
            {
                Vector3 to = _moveTarget - transform.position; to.y = 0f;
                step = to.magnitude > 0.2f ? to.normalized : Vector3.zero;
                if (d.StrafeSign != 0f && los)
                {
                    Vector3 toPlayer = playerPos - transform.position; toPlayer.y = 0f;
                    step += Vector3.Cross(Vector3.up, toPlayer.normalized) * (0.6f * d.StrafeSign);
                }
            }
            float speedScale = Time.time < _slowUntil ? _slowFactor : 1f;
            Vector3 next = transform.position + step * moveSpeed * speedScale * dt;
            transform.position = CollideMove(transform.position, next);

            Vector3 face = ToV3(d.FaceTarget) - transform.position; face.y = 0f;
            if (face.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(face), dt * 5f);

            // ── FIRE CYCLE (telegraph is LAW — never removed, only paced by the profile) ──
            if (_telegraphing)
            {
                if (Time.time >= _telegraphUntil)
                {
                    _telegraphing = false;
                    SetColor(LiveColor);
                    if (HasLineOfSight() && _charge.TryFire(Time.time)) FireBolt(_lockedAim);
                }
            }
            else if (d.WantFire && _charge.CanFire(Time.time))
            {
                _telegraphing = true;
                _telegraphUntil = Time.time + telegraphSeconds * _profile.FireCooldownScale;
                _lockedAim = ToV3(_brain.ComputeAimPoint(perception, boltSpeed)); // lead + error, dodgeable
                SetColor(TelegraphColor);
            }
        }

        // ── Perception scans (cached, low cadence) ──────────────────────────
        /// <summary>Incoming threats = the player's taser darts heading roughly at us.</summary>
        private void ScanThreats()
        {
            _cachedThreat = false;
            var hits = Physics.OverlapSphere(transform.position, 7f, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                var dart = hits[i].GetComponentInParent<TaserDartProjectile>();
                if (dart == null) continue;
                var rb = dart.GetComponent<Rigidbody>();
                if (rb == null) continue;
                Vector3 vel = rb.velocity;
                Vector3 toMe = transform.position - dart.transform.position;
                if (vel.sqrMagnitude > 1f && Vector3.Dot(vel.normalized, toMe.normalized) > 0.7f)
                {
                    _cachedThreat = true;
                    _cachedThreatVel = vel;
                    return;
                }
            }
        }

        /// <summary>Nearest baked cover point that actually breaks the player's line of sight.</summary>
        private void ScanCover(Vector3 playerPos)
        {
            _cachedHasCover = false;
            float best = float.MaxValue;
            for (int i = 0; i < _coverPoints.Count; i++)
            {
                Vector3 c = _coverPoints[i];
                float d = FlatDist(transform.position, c);
                if (d >= best) continue;
                // Covered = something solid between the cover point and the player's head.
                Vector3 eye = c + Vector3.up * 0.5f;
                if (Physics.Linecast(eye, playerPos, out var hit, lineOfSightMask, QueryTriggerInteraction.Ignore)
                    && !IsPlayerRig(hit.collider.transform))
                {
                    best = d;
                    _cachedHasCover = true;
                    _cachedCoverPos = c;
                }
            }
        }

        private Vector3 NextPatrolPoint()
        {
            if (hasObjective) return objectivePoint;
            if (_waypoints.Count == 0) return _home;
            if (FlatDist(transform.position, _waypoints[_patrolIndex]) < 1.5f)
                _patrolIndex = (_patrolIndex + 1) % _waypoints.Count;
            return _waypoints[_patrolIndex];
        }

        private void LoadNav()
        {
            var nav = GameObject.Find("__PVP_BOTNAV");
            if (nav == null) return;
            foreach (Transform child in nav.transform)
            {
                if (child.name.StartsWith("Way_")) _waypoints.Add(child.position);
                else if (child.name.StartsWith("Cover_")) _coverPoints.Add(child.position);
            }
        }

        private void FireBolt(Vector3 aimPoint)
        {
            if (_playerCombatant == null) _playerCombatant = FindObjectOfType<PvpPlayer>();
            if (_playerCombatant == null || _player == null) return;
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 dir = aimPoint - origin;
            if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
            dir.Normalize();
            var go = new GameObject("PvpBolt");
            go.transform.position = origin + dir * 0.6f;
            go.AddComponent<PvpBolt>().Init(dir * boltSpeed, _playerCombatant, _player, playerIndex);
            Debug.Log("ZIPTIDE: PVP_BOT_FIRE");
        }

        public void ReceiveHit(PvpWeapon weapon, Vector3 point, Vector3 dir)
        {
            if (!IsAlive) return;
            bool killed = _combatant.ApplyHit(weapon);
            _brain?.NotifyDamaged(Time.time);
            if (weapon == PvpWeapon.Gravity)
            {
                Vector3 k = dir; k.y = 0f;
                transform.position += k.normalized * 0.6f; // small knockback
            }
            Debug.Log("ZIPTIDE: PVP_BOT_HIT weapon=" + weapon + " hp=" + _combatant.Health);
            if (killed) Die();
        }

        private void Die()
        {
            _dead = true;
            _reviveAt = Time.time + reviveDelay;
            SetColor(DeadColor);
            if (_collider != null) _collider.enabled = false;
            PvpMatchDirector.Instance?.ReportDeath(PlayerIndex);
            Debug.Log("ZIPTIDE: PVP_BOT_DOWN");
        }

        /// <summary>Wave-spawner reset (Horde): re-home the bot and give it a fresh life.</summary>
        public void ResetAt(Vector3 pos)
        {
            _home = pos;
            Revive();
        }

        private void Revive()
        {
            _combatant.Respawn();
            _dead = false;
            _telegraphing = false;
            transform.position = _home;
            if (_collider != null) _collider.enabled = true;
            SetColor(LiveColor);
            _brain = NewBrain();          // fresh brain — retreat stickiness resets with the new life
            _charge = new WeaponCharge();
            Debug.Log("ZIPTIDE: PVP_BOT_REVIVE");
        }

        private void FindPlayer()
        {
            _playerCombatant = FindObjectOfType<PvpPlayer>();
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _player = rig.GetComponentInChildren<Camera>()?.transform;
            if (_player == null && Camera.main != null) _player = Camera.main.transform;
            if (_player != null) _lastPlayerPos = _player.position;
        }

        // Stop the move at the nearest solid wall (ignoring the bot itself and the player rig).
        private Vector3 CollideMove(Vector3 from, Vector3 to)
        {
            Vector3 delta = to - from;
            float dist = delta.magnitude;
            if (dist < 0.0001f) return to;
            Vector3 dir = delta / dist;
            const float r = 0.3f;
            var hits = Physics.SphereCastAll(from, r, dir, dist, ~0, QueryTriggerInteraction.Ignore);
            float nearest = dist;
            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i].collider;
                if (col == null) continue;
                if (col.GetComponentInParent<PvpBot>() != null) continue; // self
                if (IsPlayerRig(col.transform)) continue;                 // the player
                if (hits[i].distance < nearest) nearest = hits[i].distance;
            }
            if (nearest < dist) return from + dir * Mathf.Max(0f, nearest - r);
            return to;
        }

        private bool HasLineOfSight()
        {
            if (_player == null) return false;
            Vector3 a = transform.position + Vector3.up * 0.5f;
            Vector3 b = _player.position;
            Vector3 d = b - a;
            float dist = d.magnitude;
            if (dist < 0.01f) return true;
            a += d.normalized * 0.6f;
            if (Physics.Linecast(a, b, out var hit, lineOfSightMask, QueryTriggerInteraction.Ignore))
                return IsPlayerRig(hit.collider.transform);
            return true;
        }

        private static bool IsPlayerRig(Transform t)
        {
            while (t != null)
            {
                if (t.name == "XR Origin" || t.GetComponent<PvpPlayer>() != null) return true;
                t = t.parent;
            }
            return false;
        }

        private void SetColor(Color c)
        {
            if (_mat == null) return;
            if (_mat.HasProperty("_BaseColor")) _mat.SetColor("_BaseColor", c);
            else if (_mat.HasProperty("_Color")) _mat.SetColor("_Color", c);
        }

        private static Vec3 ToVec(Vector3 v) => new Vec3(v.x, v.y, v.z);
        private static Vector3 ToV3(Vec3 v) => new Vector3(v.X, v.Y, v.Z);
        private static float FlatDist(Vector3 a, Vector3 b)
        { float dx = a.x - b.x, dz = a.z - b.z; return Mathf.Sqrt(dx * dx + dz * dz); }
    }
}
