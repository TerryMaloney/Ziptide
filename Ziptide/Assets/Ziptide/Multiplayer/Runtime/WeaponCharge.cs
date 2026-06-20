namespace Ziptide.Multiplayer
{
    /// <summary>
    /// "Fire N shots, then recharge" model for a PvP weapon (Terry's "two shots then you need to
    /// recharge"). Pure + deterministic from a clock in seconds (mirrors the economy's IdleEngine style),
    /// so it's EditMode-testable with no real time. When emptied, a recharge window starts; once it
    /// elapses the weapon refills to full.
    /// </summary>
    public class WeaponCharge
    {
        public int MaxCharges { get; }
        public double RechargeSeconds { get; }
        public int Charges { get; private set; }

        private double _rechargeReadyAt;
        private bool _recharging;

        public WeaponCharge(int maxCharges = -1, double rechargeSeconds = -1.0)
        {
            MaxCharges = maxCharges > 0 ? maxCharges : PvpRules.WeaponCharges;
            RechargeSeconds = rechargeSeconds > 0 ? rechargeSeconds : PvpRules.RechargeSeconds;
            Charges = MaxCharges;
        }

        /// <summary>Advance the clock: completes a pending recharge once its window has elapsed.</summary>
        public void Tick(double now)
        {
            if (!_recharging) return;
            if (now >= _rechargeReadyAt)
            {
                Charges = MaxCharges;
                _recharging = false;
            }
        }

        public bool CanFire(double now)
        {
            Tick(now);
            return Charges > 0;
        }

        /// <summary>Consume a charge if available. Returns false when empty/recharging.</summary>
        public bool TryFire(double now)
        {
            Tick(now);
            if (Charges <= 0) return false;
            Charges--;
            if (Charges == 0)
            {
                _recharging = true;
                _rechargeReadyAt = now + RechargeSeconds;
            }
            return true;
        }

        /// <summary>Fraction 0..1 of the way through the current recharge (1 = ready / full).</summary>
        public double RechargeProgress(double now)
        {
            if (!_recharging) return 1.0;
            double remaining = _rechargeReadyAt - now;
            if (remaining <= 0) return 1.0;
            double t = 1.0 - (remaining / RechargeSeconds);
            return t < 0 ? 0 : (t > 1 ? 1 : t);
        }
    }
}
