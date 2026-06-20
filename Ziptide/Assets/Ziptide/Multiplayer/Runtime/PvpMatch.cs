namespace Ziptide.Multiplayer
{
    /// <summary>Lifecycle phase of a PvP match.</summary>
    public enum PvpPhase { Lobby, Countdown, Active, Ended }

    /// <summary>
    /// Pure 1v1 match state: phase + per-player score + win condition. No Unity, no netcode — the
    /// authoritative rules live here so they are deterministic and EditMode-testable. The networked
    /// match director (later) drives this and replicates the result; it does not re-implement the rules.
    /// Player indices are 0 and 1.
    /// </summary>
    public class PvpMatch
    {
        public int KillsToWin { get; }
        public PvpPhase Phase { get; private set; }

        private readonly int[] _scores = new int[2];

        public PvpMatch(int killsToWin = -1)
        {
            KillsToWin = killsToWin > 0 ? killsToWin : PvpRules.KillsToWin;
            Phase = PvpPhase.Lobby;
        }

        public int Score(int playerIndex) => _scores[Clamp(playerIndex)];
        public bool IsOver => Phase == PvpPhase.Ended;

        /// <summary>0 or 1 once ended; -1 while the match is still running.</summary>
        public int WinnerIndex => !IsOver ? -1 : (_scores[0] >= KillsToWin ? 0 : 1);

        public void Begin()
        {
            _scores[0] = 0;
            _scores[1] = 0;
            Phase = PvpPhase.Active;
        }

        /// <summary>
        /// Credit a kill to <paramref name="killerIndex"/>. Returns true if this kill ENDED the match.
        /// Ignored unless the match is Active.
        /// </summary>
        public bool RegisterKill(int killerIndex)
        {
            if (Phase != PvpPhase.Active) return false;
            int i = Clamp(killerIndex);
            _scores[i]++;
            if (_scores[i] >= KillsToWin)
            {
                Phase = PvpPhase.Ended;
                return true;
            }
            return false;
        }

        private static int Clamp(int i) => i < 0 ? 0 : (i > 1 ? 1 : i);
    }
}
