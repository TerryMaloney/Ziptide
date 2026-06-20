namespace Ziptide.Multiplayer
{
    /// <summary>The two PvP weapons and the damage each applies.</summary>
    public enum PvpWeapon { Taser, Gravity }

    /// <summary>
    /// Pure per-player combat state: health pool + damage intake + respawn. No Unity, no netcode, so the
    /// kill rules are deterministic and testable. The networked player component (later) owns the VR
    /// body and calls into this; the host treats this as authoritative.
    /// </summary>
    public class PvpCombatant
    {
        public int MaxHealth { get; }
        public int Health { get; private set; }
        public int Deaths { get; private set; }
        public bool IsAlive => Health > 0;

        public PvpCombatant(int maxHealth = -1)
        {
            MaxHealth = maxHealth > 0 ? maxHealth : PvpRules.MaxHealth;
            Health = MaxHealth;
        }

        /// <summary>Apply damage. Returns true if this blow KILLED the combatant (health hit 0).</summary>
        public bool ApplyDamage(int amount)
        {
            if (!IsAlive || amount <= 0) return false;
            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;
                Deaths++;
                return true;
            }
            return false;
        }

        /// <summary>Convenience: apply a weapon's damage. Returns true if it killed.</summary>
        public bool ApplyHit(PvpWeapon weapon) => ApplyDamage(DamageFor(weapon));

        public void Respawn() => Health = MaxHealth;

        public static int DamageFor(PvpWeapon weapon) =>
            weapon == PvpWeapon.Taser ? PvpRules.TaserDamage : PvpRules.GravityDamage;
    }
}
