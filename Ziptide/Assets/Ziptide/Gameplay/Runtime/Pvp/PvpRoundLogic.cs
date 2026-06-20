using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Pure round-resolution helpers (no Unity) so the kill→score routing is CI-testable. The MonoBehaviour
    /// director just calls these on top of Architect's authoritative <see cref="PvpMatch"/>.
    /// </summary>
    public static class PvpRoundLogic
    {
        /// <summary>In 1v1, whoever didn't die gets the kill.</summary>
        public static int KillerOf(int killedIndex) => killedIndex == 1 ? 0 : 1;

        /// <summary>Credit the killer for a death; returns the killer index and whether the match ended.</summary>
        public static (int killer, bool ended) ResolveDeath(PvpMatch match, int killedIndex)
        {
            int killer = KillerOf(killedIndex);
            bool ended = match.RegisterKill(killer);
            return (killer, ended);
        }
    }
}
