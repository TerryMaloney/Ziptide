using Ziptide.Multiplayer.Modes;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The lobby board's selection, carried across the arena's match restarts (and scene loads — it's
    /// static on purpose, the same pattern as travel intent). The mode director reads it on every
    /// (re)configure; the board writes it. Defaults = the classic 1-bot deathmatch every arena shipped
    /// with, so an untouched board changes nothing.
    /// </summary>
    public static class ArenaMatchConfig
    {
        public static PvpModeKind Mode = PvpModeKind.Deathmatch;

        /// <summary>Bot difficulty id (Resources/Bots). Empty = keep the arena layout's authored default.</summary>
        public static string Difficulty = "";

        /// <summary>Opponent bots for lobby-bot modes (clamped 1–3; combatants cap at 4).</summary>
        public static int BotCount = 1;

        /// <summary>A5 (MP100 #62): this match is today's daily challenge — the progression runtime
        /// pays the bonus on a win (once per UTC day). Set by the DAILY tile, cleared by START.</summary>
        public static bool DailyChallenge;

        public static void Reset()
        {
            Mode = PvpModeKind.Deathmatch;
            Difficulty = "";
            BotCount = 1;
            DailyChallenge = false;
        }
    }
}
