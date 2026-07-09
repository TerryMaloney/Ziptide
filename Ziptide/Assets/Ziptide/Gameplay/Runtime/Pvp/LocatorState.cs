using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Pure wrist-locator timing (no Unity → CI-testable): press-and-hold for
    /// <see cref="PvpRules.LocatorHoldSeconds"/> to fire a ping, then a
    /// <see cref="PvpRules.LocatorCooldownSeconds"/> cooldown. Deterministic from a clock + a held flag.
    /// A4.7 additions (ABILITIES_AND_ARSENAL §4): an AFTERGLOW window after each ping (tagged blips
    /// stay lit and fade over it) + tier presets (the ship S3 scanner-slot payoff) + the Sixth Sense
    /// hook (<see cref="AugmentEffects.LocatorCooldownScale"/> halves the cooldown while equipped —
    /// consulted at ping time, so equipping mid-cooldown doesn't retroactively shorten one).
    /// </summary>
    public class LocatorState
    {
        public double HoldSeconds { get; }
        public double CooldownSeconds { get; }

        /// <summary>How long a fired ping keeps tagged blips glowing (0 = the pre-A4.7 flash-only read).</summary>
        public double AfterglowSeconds { get; }

        private double _holdProgress;
        private double _cooldownUntil;
        private double _afterglowUntil = double.NegativeInfinity;
        private bool _onCooldown;

        public LocatorState(double holdSeconds = -1.0, double cooldownSeconds = -1.0,
                            double afterglowSeconds = 0.0)
        {
            HoldSeconds = holdSeconds > 0 ? holdSeconds : PvpRules.LocatorHoldSeconds;
            CooldownSeconds = cooldownSeconds > 0 ? cooldownSeconds : PvpRules.LocatorCooldownSeconds;
            AfterglowSeconds = afterglowSeconds > 0 ? afterglowSeconds : 0.0;
        }

        /// <summary>A4.7 tier presets — cooldown 60/45/30s; the afterglow trail arrives at tier 2.
        /// (Range 20/30/40m is the SCENE's field; the pure state carries the timing halves.)</summary>
        public static LocatorState ForTier(int tier)
        {
            switch (tier)
            {
                case 2: return new LocatorState(-1.0, 45.0, 8.0);
                case 3: return new LocatorState(-1.0, 30.0, 8.0);
                default: return new LocatorState(-1.0, 60.0, 0.0);
            }
        }

        /// <summary>0..1 hold charge (for the wrist ring while holding).</summary>
        public float HoldProgress => HoldSeconds > 0 ? (float)System.Math.Min(1.0, _holdProgress / HoldSeconds) : 1f;
        public bool OnCooldown => _onCooldown;

        /// <summary>True while the last ping's afterglow window is open (blips stay lit, fading).</summary>
        public bool InAfterglow(double now) => now < _afterglowUntil;

        /// <summary>0..1 remaining afterglow (1 just pinged → 0 expired) — the blip fade curve.</summary>
        public float AfterglowRemaining(double now)
        {
            if (AfterglowSeconds <= 0 || !InAfterglow(now)) return 0f;
            return (float)System.Math.Min(1.0, (_afterglowUntil - now) / AfterglowSeconds);
        }

        /// <summary>0..1 cooldown progress (1 = ready).</summary>
        public float CooldownProgress(double now)
        {
            if (!_onCooldown) return 1f;
            double remaining = _cooldownUntil - now;
            if (remaining <= 0) return 1f;
            return (float)System.Math.Min(1.0, 1.0 - remaining / CooldownSeconds);
        }

        /// <summary>
        /// Advance one tick. Returns true the instant a ping fires (held long enough, not on cooldown).
        /// </summary>
        public bool Tick(double now, double dt, bool held)
        {
            if (_onCooldown && now >= _cooldownUntil) _onCooldown = false;

            if (_onCooldown || !held)
            {
                if (!held) _holdProgress = 0;
                return false;
            }

            _holdProgress += dt;
            if (_holdProgress >= HoldSeconds)
            {
                _holdProgress = 0;
                _onCooldown = true;
                // Sixth Sense: the scale is read AT PING TIME (equip mid-cooldown never rewrites one).
                double scale = AugmentEffects.LocatorCooldownScale;
                if (scale <= 0) scale = 1.0;
                _cooldownUntil = now + CooldownSeconds * scale;
                _afterglowUntil = now + AfterglowSeconds;
                return true;
            }
            return false;
        }
    }
}
