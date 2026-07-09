namespace Ziptide.Multiplayer
{
    /// <summary>What a hit on the campaign player actually did (Phase B's whole decision surface).</summary>
    public enum PlayerHitOutcome
    {
        /// <summary>No effect — spawn protection was active, or the player is already dead.</summary>
        Ignored,
        /// <summary>Armor took it; still charged.</summary>
        Absorbed,
        /// <summary>Armor emptied to 0 — the "broken, get to cover" beat. Next hit kills.</summary>
        Broke,
        /// <summary>Dead. The shell teleports to __SPAWN_PLAYER and calls <see cref="Respawn"/>.</summary>
        Killed
    }

    /// <summary>
    /// The ENTIRE campaign-player combat rule, pure (COMBAT_HEALTH_PLAN Phase B core): armor-only
    /// defense (<see cref="ArmorMeter"/>) + spawn protection + alive/dead state + the respawn contract.
    /// Deterministic from a passed-in clock like PvpCombatant/WeaponCharge, so every decision Phase B's
    /// device shell makes is unit-tested HERE — the MonoBehaviour (`PlayerArmor`) should be a thin
    /// translator: forward hits in, read outcomes out, never decide anything itself.
    ///
    /// The rule (Terry, 2026-07-07): armor drains while charged (overkill only breaks it — never a
    /// kill), a hit at 0 armor kills, armor recharges after an out-of-combat delay, death respawns at
    /// the scene's spawn marker with full armor + a brief protection window. No health bar exists.
    /// </summary>
    public class PlayerCombatState
    {
        public ArmorMeter Armor { get; }
        public bool IsAlive { get; private set; } = true;
        public int Deaths { get; private set; }

        private readonly double _spawnProtectionSec;
        private double _protectedUntil;

        public PlayerCombatState(ArmorMeter armor = null, double spawnProtectionSec = -1)
        {
            Armor = armor ?? new ArmorMeter();
            _spawnProtectionSec = spawnProtectionSec >= 0 ? spawnProtectionSec
                                                          : PvpRules.SpawnProtectionSeconds;
        }

        /// <summary>Brief post-respawn invulnerability window (also covers world entry if armed).</summary>
        public bool IsProtected(double now) => now < _protectedUntil;

        /// <summary>Armor at 0 and alive — one hit from death (drives the HUD warning state).</summary>
        public bool IsBroken => IsAlive && Armor.IsBroken;

        /// <summary>Arm the protection window without a respawn (e.g. on world entry after travel).</summary>
        public void GrantProtection(double now) => _protectedUntil = now + _spawnProtectionSec;

        /// <summary>
        /// Apply an incoming hit. Protection and death make it <see cref="PlayerHitOutcome.Ignored"/>;
        /// otherwise the armor rule decides. A Killed outcome flips <see cref="IsAlive"/> — the shell
        /// must then respawn (teleport + <see cref="Respawn"/>); this type never auto-revives.
        /// </summary>
        public PlayerHitOutcome ApplyDamage(int amount, double now)
        {
            if (!IsAlive || amount <= 0 || IsProtected(now)) return PlayerHitOutcome.Ignored;

            switch (Armor.ApplyDamage(amount, now))
            {
                case ArmorHit.Absorbed: return PlayerHitOutcome.Absorbed;
                case ArmorHit.Broke: return PlayerHitOutcome.Broke;
                default:
                    IsAlive = false;
                    Deaths++;
                    return PlayerHitOutcome.Killed;
            }
        }

        /// <summary>Advance armor regen. Dead players don't regen — respawn refills instead.</summary>
        public void Tick(double now, double dt)
        {
            if (IsAlive) Armor.Tick(now, dt);
        }

        /// <summary>Back at the spawn marker: alive, full armor, protection armed.</summary>
        public void Respawn(double now)
        {
            IsAlive = true;
            Armor.Reset();
            GrantProtection(now);
        }
    }
}
