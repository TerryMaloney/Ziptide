using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Local player combatant for PvP (index 0). Owns a <see cref="PvpCombatant"/>, takes damage, and
    /// respawns to its spawn marker with brief protection. Scene-scoped — the persistent rig is only
    /// teleported on respawn. In solo the bot lands hits by calling <see cref="ReceiveHit"/> directly
    /// (hitscan); Phase-4 netcode adds a networked head hit-collider for remote projectiles.
    /// </summary>
    public class PvpPlayer : MonoBehaviour, IPvpDamageable
    {
        [Tooltip("Spawn marker id to respawn at (placed by the arena patcher).")]
        public string spawnMarkerId = "player";

        public int PlayerIndex => 0;
        public bool IsAlive => _combatant != null && _combatant.IsAlive;
        public PvpCombatant Combatant => _combatant;

        private PvpCombatant _combatant;
        private PlayerRigPersistence _rig;
        private PlayerStunReceiver _flash;
        private float _protectedUntil;

        private void Awake()
        {
            _combatant = new PvpCombatant();
        }

        private void Start()
        {
            _rig = FindObjectOfType<PlayerRigPersistence>();
            _flash = FindObjectOfType<PlayerStunReceiver>();
            PvpMatchDirector.Instance?.Register(this);
        }

        public void ReceiveHit(PvpWeapon weapon, Vector3 point, Vector3 dir)
        {
            if (!IsAlive) return;
            if (Time.time < _protectedUntil) return; // spawn protection
            bool killed = _combatant.ApplyHit(weapon);
            if (_flash != null) _flash.ApplyStun(0.15f, 1f); // brief flash only (slowFactor 1 = no slow)
            Debug.Log("ZIPTIDE: PVP_PLAYER_HIT weapon=" + weapon + " hp=" + _combatant.Health);
            if (killed) Die();
        }

        private void Die()
        {
            PvpMatchDirector.Instance?.ReportDeath(PlayerIndex);
            Respawn();
        }

        public void Respawn()
        {
            _combatant.Respawn();
            _protectedUntil = Time.time + (float)PvpRules.SpawnProtectionSeconds;
            if (_rig != null) _rig.TeleportToMarker(spawnMarkerId);
            Debug.Log("ZIPTIDE: PVP_PLAYER_RESPAWN protectedFor=" + PvpRules.SpawnProtectionSeconds);
        }
    }
}
