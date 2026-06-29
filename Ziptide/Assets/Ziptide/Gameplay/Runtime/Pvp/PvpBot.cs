using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// AI opponent (index 1) — a stand-in for a networked remote player so the kids can fight solo now.
    /// Roams toward the player at a standoff, telegraphs, then lands a hitscan "taser" shot. Implements
    /// <see cref="IPvpDamageable"/> so the player's taser/gravity guns down it. On death it revives at its
    /// spawn. Phase-4 netcode replaces this with a real avatar behind the same interface.
    /// </summary>
    public class PvpBot : MonoBehaviour, IPvpDamageable, IScannable
    {
        public float detectRange = 22f;
        public float standoffDistance = 8f;
        public float moveSpeed = 2.5f;
        public float fireRange = 14f;
        public float telegraphSeconds = 0.8f;
        public float fireCooldown = 1.8f;
        [Tooltip("Bolt travel speed — slow enough to see and dodge.")]
        public float boltSpeed = 5f;
        public float reviveDelay = 2.5f;
        public LayerMask lineOfSightMask = ~0;

        public int PlayerIndex => 1;
        public bool IsAlive => _combatant != null && _combatant.IsAlive && !_dead;

        // IScannable — the wrist scanner detects the opponent (and campaign nodes/loot later).
        public Transform ScanTransform => transform;
        public ScanKind ScanKind => Ziptide.Gameplay.ScanKind.Enemy;
        public bool ScanActive => IsAlive;

        private PvpCombatant _combatant;
        private Transform _player;
        private PvpPlayer _playerCombatant;
        private Renderer _renderer;
        private Collider _collider;
        private Material _mat;
        private Vector3 _home;
        private float _nextFireReady;
        private float _telegraphUntil;
        private bool _telegraphing;
        private bool _dead;
        private float _reviveAt;

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
        }

        private void Update()
        {
            if (_dead)
            {
                if (Time.time >= _reviveAt) Revive();
                return;
            }

            if (_player == null) { FindPlayer(); if (_player == null) return; }

            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist > detectRange) return;

            // Move to maintain standoff.
            Vector3 to = _player.position - transform.position; to.y = 0f;
            float d = to.magnitude;
            Vector3 step = Vector3.zero;
            if (d > standoffDistance + 0.5f) step = to.normalized;
            else if (d < standoffDistance - 0.5f) step = -to.normalized;
            // strafe a little so it isn't a sitting duck
            step += Vector3.Cross(Vector3.up, to.normalized) * 0.4f * Mathf.Sin(Time.time);
            // Collision-aware move: clamp the step at the nearest wall so the bot never phases through solid
            // geometry (universal rule — nothing moves through walls unless explicitly allowed). Mirrors
            // DroneCombatBehavior.CollideMove.
            Vector3 next = transform.position + step * moveSpeed * Time.deltaTime;
            transform.position = CollideMove(transform.position, next);
            if (to.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(to.x, 0f, to.z)), Time.deltaTime * 4f);

            // Fire cycle: telegraph then hitscan.
            bool los = HasLineOfSight();
            if (_telegraphing)
            {
                if (Time.time >= _telegraphUntil)
                {
                    _telegraphing = false;
                    SetColor(LiveColor);
                    if (dist <= fireRange && los) FireAtPlayer();
                    _nextFireReady = Time.time + fireCooldown;
                }
            }
            else if (Time.time >= _nextFireReady && dist <= fireRange && los)
            {
                _telegraphing = true;
                _telegraphUntil = Time.time + telegraphSeconds;
                SetColor(TelegraphColor);
            }
        }

        private void FireAtPlayer()
        {
            if (_playerCombatant == null) _playerCombatant = FindObjectOfType<PvpPlayer>();
            if (_playerCombatant == null || _player == null) return;
            // Spawn a VISIBLE, slow bolt the player can see and dodge (was an instant hitscan that landed
            // with nothing on screen). The bolt registers the PvP hit itself when it reaches the player.
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 dir = (_player.position - origin);
            if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
            dir.Normalize();
            var go = new GameObject("PvpBolt");
            go.transform.position = origin + dir * 0.6f;
            go.AddComponent<PvpBolt>().Init(dir * boltSpeed, _playerCombatant, _player);
            Debug.Log("ZIPTIDE: PVP_BOT_FIRE");
        }

        public void ReceiveHit(PvpWeapon weapon, Vector3 point, Vector3 dir)
        {
            if (!IsAlive) return;
            bool killed = _combatant.ApplyHit(weapon);
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

        private void Revive()
        {
            _combatant.Respawn();
            _dead = false;
            _telegraphing = false;
            transform.position = _home;
            if (_collider != null) _collider.enabled = true;
            SetColor(LiveColor);
            Debug.Log("ZIPTIDE: PVP_BOT_REVIVE");
        }

        private void FindPlayer()
        {
            _playerCombatant = FindObjectOfType<PvpPlayer>();
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _player = rig.GetComponentInChildren<Camera>()?.transform;
            if (_player == null && Camera.main != null) _player = Camera.main.transform;
        }

        // Stop the move at the nearest solid wall (ignoring the bot itself and the player rig). Without this
        // the bot moved by raw transform writes and walked through arena walls.
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
    }
}
