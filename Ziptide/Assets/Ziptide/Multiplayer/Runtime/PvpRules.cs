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

        // ── Campaign player ARMOR model (COMBAT_HEALTH_PLAN, decided 2026-07-07): armor-only, no health.
        // A hit while armored drains it (overkill just empties the meter = "break"); a hit at 0 armor is
        // immediate death. Armor recharges after an out-of-combat delay — no health packs. These are
        // starting framework values; tune on-device. Consumed by ArmorMeter + (Phase B) PlayerArmor.
        public const int PlayerArmor = 4;              // armor charges (the whole defensive budget)
        public const double ArmorRegenPerSec = 1.0;    // charges restored per second once regen kicks in
        public const double ArmorRegenDelaySec = 3.0;  // seconds out-of-combat before armor regenerates

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

        // ── The melee pair (MP100 wave 1, 2026-07-06 — Terry: "find good places for melee weapons").
        // TRUE contact melee, unlike the thumper's positional AoE: the blade/pike only hits what the
        // swing actually reaches. Counters: both need you INSIDE their reach — range beats them; the
        // pike's thrust is a straight line — sidestep it.
        public const int BreakerBladeDamage = 1;          // fast 1H contact swings — DPS through risk
        public const int TidePikeDamage = 2;              // slow committed thrusts — taser-tier per hit
        public const double BladeContactDebounce = 0.45;  // per-TARGET re-hit window during a swing
        public const double BladeReach = 0.9;             // contact sphere radius at the blade tip
        public const double PikeThrustDebounce = 0.9;     // recovery between thrusts
        public const double PikeReach = 1.6;              // thrust line length past the shaft tip
        public const double MeleeSwingSpeed = 2.2;        // m/s of the tip that counts as a real swing
        public const double PikeThrustSpeed = 2.6;        // m/s along the shaft axis for a thrust

        // Bots hear nearby weapon noise (swings, thumps, net lands) and investigate — the earshot
        // radius and how long a noise stays "fresh" for the perception tick.
        public const double NoiseEarshotMeters = 15.0;
        public const double NoiseFreshSeconds = 0.6;
    }
}
