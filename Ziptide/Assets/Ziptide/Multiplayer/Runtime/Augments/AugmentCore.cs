using System;

namespace Ziptide.Multiplayer.Augments
{
    /// <summary>
    /// ACTIVE-augment state machine (A4.5, spec ABILITIES_AND_ARSENAL §2) — pure + clock-driven
    /// (the WeaponCharge idiom): Ready → Active(duration) → Cooldown → Ready. Deterministic from a
    /// seconds clock, EditMode-testable, netcode-friendly (state is a pure function of activation
    /// time). Zero-duration actives (Surge Dash's instant burst) go straight to cooldown.
    /// </summary>
    public sealed class AugmentClock
    {
        public double CooldownSeconds { get; }
        public double DurationSeconds { get; }

        private double _activeUntil = double.NegativeInfinity;
        private double _readyAt = double.NegativeInfinity;

        public AugmentClock(double cooldownSeconds, double durationSeconds = 0.0)
        {
            CooldownSeconds = Math.Max(0.0, cooldownSeconds);
            DurationSeconds = Math.Max(0.0, durationSeconds);
        }

        public bool IsActive(double now) => now < _activeUntil;
        public bool IsReady(double now) => now >= _readyAt && !IsActive(now);

        /// <summary>0..1 (1 = ready). The belt orb's fill ring reads this.</summary>
        public float ReadyProgress(double now)
        {
            if (IsReady(now)) return 1f;
            double remaining = _readyAt - now;
            if (remaining <= 0) return 1f;
            if (CooldownSeconds <= 0) return 1f;
            double t = 1.0 - remaining / CooldownSeconds;
            return (float)(t < 0 ? 0 : (t > 1 ? 1 : t));
        }

        /// <summary>Returns true and starts the effect when ready; false otherwise (spam-safe).</summary>
        public bool TryActivate(double now)
        {
            if (!IsReady(now)) return false;
            _activeUntil = now + DurationSeconds;
            _readyAt = now + DurationSeconds + CooldownSeconds; // cooldown runs AFTER the effect ends
            return true;
        }
    }

    public enum AugmentKind { Active, Passive }

    /// <summary>
    /// The slot rule (the spec's whole balance: "slot scarcity IS the balance") — ONE active + ONE
    /// passive equipped at a time. Equipping into an occupied slot swaps (returns the replaced id so
    /// the scene can drop it as a pickup). Pure, testable.
    /// </summary>
    public sealed class AugmentLoadout
    {
        public string ActiveId { get; private set; }
        public string PassiveId { get; private set; }

        /// <summary>Equip an augment; returns the id it displaced in that slot (null if it was empty).</summary>
        public string Equip(AugmentKind kind, string augmentId)
        {
            if (string.IsNullOrEmpty(augmentId)) return null;
            if (kind == AugmentKind.Active)
            {
                string old = ActiveId;
                ActiveId = augmentId;
                return old;
            }
            else
            {
                string old = PassiveId;
                PassiveId = augmentId;
                return old;
            }
        }

        public bool Has(string augmentId) =>
            !string.IsNullOrEmpty(augmentId) && (ActiveId == augmentId || PassiveId == augmentId);
    }

    /// <summary>
    /// The GLOBAL effect hooks passives/actives publish through (pure statics, the PvpNoise idiom —
    /// consumers poll, producers set; every default is "no effect"). Scene code NEVER hardcodes an
    /// augment id — it consults these. Reset on loadout change/scene load by the controller.
    /// </summary>
    public static class AugmentEffects
    {
        /// <summary>Multiplies weapon fire-cooldowns/debounces (Overclock: 0.5 while active). 1 = off.</summary>
        public static float WeaponCooldownScale = 1f;

        /// <summary>0..1 resistance to slows/stuns (Sure Step: 0.5). 0 = off. Applied as: effective
        /// seconds ×= (1-r); effective slow washed toward 1 by r.</summary>
        public static float SlowResistance = 0f;

        /// <summary>Multiplies the locator's cooldown (Sixth Sense: 0.5). 1 = off.</summary>
        public static float LocatorCooldownScale = 1f;

        /// <summary>Pickup-pull reach in meters (Magnet Palm: 3). 0 = off.</summary>
        public static float MagnetReachMeters = 0f;

        public static void ResetAll()
        {
            WeaponCooldownScale = 1f;
            SlowResistance = 0f;
            LocatorCooldownScale = 1f;
            MagnetReachMeters = 0f;
        }
    }

    /// <summary>
    /// A4.6's pure heart: one charge pool BOTH hands draw from — dual-wield gives flexibility (two
    /// angles/types), never extra sustained DPS, because the math can't be beaten. Built + tested and
    /// READY, but ⚠ full wiring is BLOCKED on a real finding: the PLAYER's guns don't consume
    /// WeaponCharge today (only bots do) — there is nothing to pool until player weapons adopt a
    /// charge. Logged on the board; when they do, this drops in.
    /// </summary>
    public sealed class SharedChargePool
    {
        private readonly WeaponCharge _pool;

        public SharedChargePool(int maxCharges = -1, double rechargeSeconds = -1.0)
            => _pool = new WeaponCharge(maxCharges, rechargeSeconds);

        public int Charges => _pool.Charges;

        /// <summary>Either hand fires through the same gate — hand identity is irrelevant to the pool.</summary>
        public bool TryFire(double now) => _pool.TryFire(now);
        public bool CanFire(double now) => _pool.CanFire(now);
        public double RechargeProgress(double now) => _pool.RechargeProgress(now);
    }
}
