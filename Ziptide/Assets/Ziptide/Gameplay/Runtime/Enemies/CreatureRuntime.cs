using System.Collections;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The generalized creature body (GAME_PLAN M3, spec CREATURE_DESIGN.md) — what DroneRuntime is to
    /// drones, for everything else. Loads its <see cref="CreatureDefinition"/> by SERIALIZED id (edit-time
    /// spawn, gotcha #7), owns health + the NON-LETHAL disable, and is hittable by the existing weapons
    /// with zero weapon edits: it implements <see cref="IPvpDamageable"/> (taser dart + gravity gun both
    /// route through it) and <see cref="IShockable"/> (any other stun source). A sibling
    /// <see cref="CreatureBehaviorBase"/> owns motion; this class pauses it while stunned and kills it on
    /// disable. Loot pays the profile on disable. Logs ZIPTIDE: CREATURE_DOWN id=… / CREATURE_RESPAWN.
    /// </summary>
    public class CreatureRuntime : MonoBehaviour, IShockable, IPvpDamageable
    {
        [Tooltip("CreatureDefinition asset name in Resources/Enemies (authored by CreatureVariantAuthor).")]
        public string creatureId = "swarm_bug";

        [Tooltip("Seconds before it re-forms after a disable (0 = stays down).")]
        public float respawnDelay = 0f;

        private const float TaserDamage = 10f;
        private const float GravityDamage = 8f;
        private const float TaserStunSeconds = 1.5f;

        private CreatureDefinition _def;
        private CreatureBehaviorBase _behavior;
        private Renderer[] _renderers;
        private float _health = 1f;
        private float _stunnedUntil;
        private bool _down;
        private Vector3 _homePos;
        private Vector3 _homeScale;

        private static readonly Color StunTint = new Color(0.35f, 0.85f, 1f);
        private static readonly Color DownTint = new Color(0.18f, 0.18f, 0.20f);

        /// <summary>The creature's data (null until Awake; defaults applied if the asset is missing).</summary>
        public CreatureDefinition Definition => _def;
        /// <summary>Alive and not currently stunned — behaviors gate their motion on this.</summary>
        public bool IsActive => !_down && Time.time >= _stunnedUntil;
        public Vector3 HomePos => _homePos;

        // IPvpDamageable — index -1 marks "not a combatant" (PvpMatchDirector only registers players/bots).
        public int PlayerIndex => -1;
        public bool IsAlive => !_down;

        private void Awake()
        {
            _homePos = transform.position;
            _homeScale = transform.localScale;
            _def = Resources.Load<CreatureDefinition>("Enemies/" + creatureId);
            if (_def == null)
                Debug.LogWarning("ZIPTIDE: CREATURE_DEF_MISSING id=" + creatureId);
            _health = _def != null ? _def.maxHealth : 10f;
            _behavior = GetComponent<CreatureBehaviorBase>();
            _renderers = GetComponentsInChildren<Renderer>(true);
        }

        private void Update()
        {
            // Un-stun visual restore (behaviors read IsActive themselves).
            if (!_down && _stunnedUntil > 0f && Time.time >= _stunnedUntil)
            {
                _stunnedUntil = 0f;
                Tint(Color.white, restore: true);
            }
        }

        // ── Hit entry points (existing weapon plumbing) ─────────────────────

        public void Shock(float seconds)
        {
            if (_down || (_def != null && !_def.shockable)) return;
            _stunnedUntil = Mathf.Max(_stunnedUntil, Time.time + seconds);
            Tint(StunTint);
            _behavior?.OnStunned(seconds);
        }

        public void ReceiveHit(PvpWeapon weapon, Vector3 point, Vector3 dir)
        {
            if (_down) return;
            if (weapon == PvpWeapon.Taser)
            {
                _health -= TaserDamage;
                Shock(TaserStunSeconds);
            }
            else
            {
                _health -= GravityDamage;
                // Gravity kick: shove the body along the beam (collision-clean via the behavior's mover
                // next frame — we only displace, never through walls, because behaviors re-clamp).
                transform.position += new Vector3(dir.x, 0f, dir.z).normalized * 0.5f;
            }
            if (_health <= 0f) Disable();
        }

        // ── Non-lethal disable / respawn ─────────────────────────────────────

        private void Disable()
        {
            _down = true;
            if (_behavior != null) _behavior.enabled = false;
            Tint(DownTint);
            // Crumple, don't ragdoll — readable and cheap.
            transform.localScale = new Vector3(_homeScale.x * 1.15f, _homeScale.y * 0.35f, _homeScale.z * 1.15f);

            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile != null && _def != null && _def.loot != null)
                foreach (var l in _def.loot)
                    if (l != null && !string.IsNullOrEmpty(l.resourceId))
                        profile.AddResource(l.resourceId, l.amount);

            Debug.Log("ZIPTIDE: CREATURE_DOWN id=" + creatureId + " name=" + gameObject.name);
            if (respawnDelay > 0f) StartCoroutine(RespawnAfter());
        }

        private IEnumerator RespawnAfter()
        {
            yield return new WaitForSeconds(respawnDelay);
            _down = false;
            _stunnedUntil = 0f;
            _health = _def != null ? _def.maxHealth : 10f;
            transform.position = _homePos;
            transform.localScale = _homeScale;
            Tint(Color.white, restore: true);
            if (_behavior != null) _behavior.enabled = true;
            Debug.Log("ZIPTIDE: CREATURE_RESPAWN id=" + creatureId);
        }

        private void Tint(Color c, bool restore = false)
        {
            if (_renderers == null) return;
            foreach (var r in _renderers)
            {
                if (r == null || r.material == null) continue;
                Color target = restore ? Color.white : c;
                if (r.material.HasProperty("_BaseColor"))
                {
                    if (restore) continue; // behaviors own their base colors; only overlay tints apply
                    r.material.SetColor("_BaseColor", c);
                }
                else if (r.material.HasProperty("_Color"))
                {
                    if (restore) continue;
                    r.material.color = target;
                }
            }
            if (restore && _behavior != null) _behavior.RestoreVisuals();
        }
    }
}
