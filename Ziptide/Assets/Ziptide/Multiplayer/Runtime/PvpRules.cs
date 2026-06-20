namespace Ziptide.Multiplayer
{
    /// <summary>
    /// Tunable constants for the 1v1 PvP mode (Phase 1 backbone). Pure data — no Unity, no scene, no
    /// netcode. These are the starting framework values from Terry's brief; tune on-device later.
    ///
    /// Damage model: Health 6, taser hit 2 (3 hits = kill), gravity hit 1 (half taser, 6 hits = kill).
    /// Weapon charge: fire 2 shots, then a short recharge before firing again.
    /// </summary>
    public static class PvpRules
    {
        public const int KillsToWin = 10;        // best-of-10: first to this many kills wins

        public const int MaxHealth = 6;          // player health pool
        public const int TaserDamage = 2;        // 3 taser hits => kill (6 / 2)
        public const int GravityDamage = 1;      // half taser => 6 gravity hits => kill

        public const int WeaponCharges = 2;      // shots before a recharge is required
        public const double RechargeSeconds = 1.5;

        public const double LocatorHoldSeconds = 3.0;     // wrist locator: press-and-hold to ping
        public const double LocatorCooldownSeconds = 60.0;

        public const double WallHoleRegenSeconds = 180.0; // hammer holes regenerate after ~3 min
        public const double HammerAutoReturnSeconds = 120.0; // hammer off-belt > 2 min => auto-return

        public const double SpawnProtectionSeconds = 2.0; // brief invulnerability after respawn
    }
}
