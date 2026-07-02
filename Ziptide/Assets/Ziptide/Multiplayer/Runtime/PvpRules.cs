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

        // ── A4 arsenal (design docs/design/PVP_ARENA_AAA.md §A4 + ABILITIES_AND_ARSENAL §1) ──
        // Every weapon = damage entry + a visible counter. Counters: the net is a thrown arc you can
        // sidestep; the thumper needs melee range (keep distance); the prism telegraphs a long charge
        // (break line of sight).
        public const int StaticNetDamage = 1;        // utility first — the SLOW is the payload
        public const int SonicThumperDamage = 2;     // taser-tier, but you must close to melee
        public const int PrismBeamDamage = 3;        // 2 hits kill — earned through the charge-up

        public const double StaticNetSlowSeconds = 3.0;
        public const double StaticNetSlowFactor = 0.45;   // multiplier on move speed inside the zone
        public const double StaticNetZoneSeconds = 4.0;   // zone lifetime after the net lands
        public const double StaticNetZoneRadius = 2.5;

        public const double ThumperRadius = 2.5;          // shockwave reach from the swing
        public const double ThumperShoveMeters = 2.0;     // knockback on combatants hit

        public const double PrismChargeSeconds = 1.2;     // hold-to-charge before the beam fires
        public const double PrismCooldownSeconds = 2.5;
        public const double PrismRange = 30.0;
    }
}
