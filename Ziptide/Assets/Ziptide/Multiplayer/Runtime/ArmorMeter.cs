namespace Ziptide.Multiplayer
{
    /// <summary>Outcome of a hit against an <see cref="ArmorMeter"/>.</summary>
    public enum ArmorHit
    {
        /// <summary>The armor took the hit and still has charge left.</summary>
        Absorbed,
        /// <summary>The hit emptied the armor to 0 (it "broke") but did NOT kill — the next hit will.</summary>
        Broke,
        /// <summary>The hit landed with no armor left (or LethalOnBreak) — the player dies.</summary>
        Killed
    }

    /// <summary>
    /// The campaign player's ONLY defense (COMBAT_HEALTH_PLAN, decided 2026-07-07): armor, no health bar.
    /// Pure and Unity-free like <see cref="PvpCombatant"/>/<see cref="WeaponCharge"/>, so the rule is
    /// deterministic and unit-testable (the caller passes the clock; no UnityEngine.Time here).
    ///
    /// The rule, exactly as Terry framed it: a hit while armored DRAINS the meter (an overkill hit just
    /// empties it to 0 — "armor breaks", never a kill); a hit landed while the meter is already at 0 is
    /// immediate death. So you always get the clear "broken, get to cover" beat before you can die.
    /// Armor recharges on its own after a short out-of-combat delay — there are no health packs.
    /// </summary>
    public class ArmorMeter
    {
        public int MaxCharge { get; }
        public int Charge { get; private set; }
        public double RegenPerSec { get; }
        public double RegenDelaySec { get; }

        /// <summary>If true, the hit that empties the meter ALSO kills (no "broken" grace hit). Default false.</summary>
        public bool LethalOnBreak { get; }

        private double _accum;         // fractional charge accumulated toward the next whole point
        private double _regenReadyAt;  // regen is blocked until the clock reaches this (set on each hit)

        public ArmorMeter(int maxCharge = -1, double regenPerSec = -1, double regenDelaySec = -1,
                          bool lethalOnBreak = false)
        {
            MaxCharge = maxCharge > 0 ? maxCharge : PvpRules.PlayerArmor;
            RegenPerSec = regenPerSec >= 0 ? regenPerSec : PvpRules.ArmorRegenPerSec;
            RegenDelaySec = regenDelaySec >= 0 ? regenDelaySec : PvpRules.ArmorRegenDelaySec;
            LethalOnBreak = lethalOnBreak;
            Charge = MaxCharge;
        }

        /// <summary>No armor left — the "one hit from death" warning state.</summary>
        public bool IsBroken => Charge <= 0;

        /// <summary>Fraction of the armor meter currently filled (0..1) — for the HUD.</summary>
        public double Fraction => MaxCharge > 0 ? (double)Charge / MaxCharge : 0.0;

        /// <summary>
        /// Apply a hit. Any real hit resets the regen delay. Returns whether the armor absorbed it, broke,
        /// or the player was killed (a hit at 0 armor, or an emptying hit when <see cref="LethalOnBreak"/>).
        /// </summary>
        public ArmorHit ApplyDamage(int amount, double now)
        {
            if (amount <= 0) return ArmorHit.Absorbed; // not a real hit — don't disturb regen or state

            _regenReadyAt = now + RegenDelaySec;
            _accum = 0.0;

            if (Charge <= 0)
                return ArmorHit.Killed; // hit while already broken => death

            Charge -= amount;
            if (Charge <= 0)
            {
                Charge = 0;
                return LethalOnBreak ? ArmorHit.Killed : ArmorHit.Broke;
            }
            return ArmorHit.Absorbed;
        }

        /// <summary>Advance armor regen by <paramref name="dt"/> seconds at clock time <paramref name="now"/>.</summary>
        public void Tick(double now, double dt)
        {
            if (Charge >= MaxCharge || now < _regenReadyAt || dt <= 0.0) return;
            _accum += RegenPerSec * dt;
            while (_accum >= 1.0 && Charge < MaxCharge)
            {
                Charge++;
                _accum -= 1.0;
            }
            if (Charge >= MaxCharge)
            {
                Charge = MaxCharge;
                _accum = 0.0;
            }
        }

        /// <summary>Refill for a respawn.</summary>
        public void Reset()
        {
            Charge = MaxCharge;
            _accum = 0.0;
            _regenReadyAt = 0.0;
        }
    }
}
