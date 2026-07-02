using System;

namespace Ziptide.Multiplayer.Modes
{
    /// <summary>
    /// Pure match-setup rules per mode (M7a A3-scene) so the scene director stays a thin translator.
    /// Kill-limited modes use PvpRules.KillsToWin; objective modes park the kill limit out of reach and
    /// end through <see cref="PvpMatch.EndByRule"/> when their own rule fires.
    /// </summary>
    public static class ArenaModeSetup
    {
        /// <summary>Sentinel "kills never end this mode" limit for objective/wave modes.</summary>
        public const int ObjectiveModeKillLimit = 9999;

        /// <summary>Combatants = the player + bots, clamped to PvpMatch's supported 2–4.</summary>
        public static int CombatantCount(int botCount)
        {
            int bots = botCount < 1 ? 1 : (botCount > 3 ? 3 : botCount);
            return 1 + bots;
        }

        public static int KillsToWinFor(PvpModeKind mode) =>
            mode == PvpModeKind.Deathmatch ? PvpRules.KillsToWin : ObjectiveModeKillLimit;

        /// <summary>Horde is the player vs the waves — the scene bot pool is owned by the wave
        /// spawner, not the lobby's bot-count pick.</summary>
        public static bool UsesLobbyBots(PvpModeKind mode) => mode != PvpModeKind.Horde;
    }
}
