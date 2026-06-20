using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Pure wrist-locator timing (no Unity → CI-testable): press-and-hold for
    /// <see cref="PvpRules.LocatorHoldSeconds"/> to fire a ping, then a
    /// <see cref="PvpRules.LocatorCooldownSeconds"/> cooldown. Deterministic from a clock + a held flag.
    /// </summary>
    public class LocatorState
    {
        public double HoldSeconds { get; }
        public double CooldownSeconds { get; }

        private double _holdProgress;
        private double _cooldownUntil;
        private bool _onCooldown;

        public LocatorState(double holdSeconds = -1.0, double cooldownSeconds = -1.0)
        {
            HoldSeconds = holdSeconds > 0 ? holdSeconds : PvpRules.LocatorHoldSeconds;
            CooldownSeconds = cooldownSeconds > 0 ? cooldownSeconds : PvpRules.LocatorCooldownSeconds;
        }

        /// <summary>0..1 hold charge (for the wrist ring while holding).</summary>
        public float HoldProgress => HoldSeconds > 0 ? (float)System.Math.Min(1.0, _holdProgress / HoldSeconds) : 1f;
        public bool OnCooldown => _onCooldown;

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
                _cooldownUntil = now + CooldownSeconds;
                return true;
            }
            return false;
        }
    }
}
