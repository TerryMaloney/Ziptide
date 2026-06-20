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
            transform.position += step * moveSpeed * Time.deltaTime;
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
            if (_playerCombatant == null) return;
            _playerCombatant.ReceiveHit(PvpWeapon.Taser, _player != null ? _player.position : transform.position, transform.forward);
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
