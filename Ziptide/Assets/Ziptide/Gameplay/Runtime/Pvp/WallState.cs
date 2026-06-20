using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    public enum WallStage { Intact, SmallHole, LargeHole }

    /// <summary>
    /// Pure breakable-wall state (no Unity → CI-testable). Hammer hits escalate Intact → SmallHole
    /// (shoot-through) → LargeHole (pass-through); the hole fully regenerates after
    /// <see cref="PvpRules.WallHoleRegenSeconds"/> from the last hit. Deterministic from a clock.
    /// </summary>
    public class WallState
    {
        public double RegenSeconds { get; }
        public WallStage Stage { get; private set; } = WallStage.Intact;

        private double _lastHitAt;
        private bool _damaged;

        public WallState(double regenSeconds = -1.0)
        {
            RegenSeconds = regenSeconds > 0 ? regenSeconds : PvpRules.WallHoleRegenSeconds;
        }

        public bool ShootThrough => Stage != WallStage.Intact;
        public bool PassThrough => Stage == WallStage.LargeHole;

        /// <summary>A hammer hit. Escalates one stage (capped at LargeHole) and resets the regen clock.</summary>
        public void Hit(double now)
        {
            if (Stage == WallStage.Intact) Stage = WallStage.SmallHole;
            else if (Stage == WallStage.SmallHole) Stage = WallStage.LargeHole;
            _lastHitAt = now;
            _damaged = Stage != WallStage.Intact;
        }

        /// <summary>Advance the clock; fully heal back to Intact once the regen window elapses.</summary>
        public void Tick(double now)
        {
            if (!_damaged) return;
            if (now - _lastHitAt >= RegenSeconds)
            {
                Stage = WallStage.Intact;
                _damaged = false;
            }
        }
    }
}
