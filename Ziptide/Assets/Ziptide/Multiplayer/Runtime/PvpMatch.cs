namespace Ziptide.Multiplayer
{
    /// <summary>Lifecycle phase of a PvP match.</summary>
    public enum PvpPhase { Lobby, Countdown, Active, Ended }

    /// <summary>
    /// Pure match state: phase + per-player score + optional TEAMS + win condition. No Unity, no
    /// netcode — the authoritative rules live here so they are deterministic and EditMode-testable.
    /// Generalized for M7a A3 from the original 1v1: N combatants (2–4: bots fill slots), optional
    /// team assignment (team score = summed members). The default constructor stays the classic 1v1
    /// so every existing consumer/test is untouched.
    /// </summary>
    public class PvpMatch
    {
        public int KillsToWin { get; }
        public int PlayerCount { get; }
        public PvpPhase Phase { get; private set; }

        private readonly int[] _scores;
        private readonly int[] _teamOf;     // team id per player (default: everyone their own team)

        public PvpMatch(int killsToWin = -1) : this(2, killsToWin) { }

        public PvpMatch(int playerCount, int killsToWin = -1, int[] teams = null)
        {
            PlayerCount = playerCount < 2 ? 2 : (playerCount > 4 ? 4 : playerCount);
            KillsToWin = killsToWin > 0 ? killsToWin : PvpRules.KillsToWin;
            _scores = new int[PlayerCount];
            _teamOf = new int[PlayerCount];
            for (int i = 0; i < PlayerCount; i++)
                _teamOf[i] = (teams != null && i < teams.Length) ? teams[i] : i; // FFA by default
            Phase = PvpPhase.Lobby;
        }

        public int Score(int playerIndex) => _scores[Clamp(playerIndex)];
        public int TeamOf(int playerIndex) => _teamOf[Clamp(playerIndex)];

        /// <summary>Summed score of every player on the team.</summary>
        public int TeamScore(int teamId)
        {
            int total = 0;
            for (int i = 0; i < PlayerCount; i++)
                if (_teamOf[i] == teamId) total += _scores[i];
            return total;
        }

        public bool IsOver => Phase == PvpPhase.Ended;

        /// <summary>The winning PLAYER index once ended (-1 while running) — the highest scorer.
        /// Use <see cref="WinnerTeam"/> for the team result in team play.</summary>
        public int WinnerIndex
        {
            get
            {
                if (!IsOver) return -1;
                int best = 0;
                for (int i = 1; i < PlayerCount; i++)
                    if (_scores[i] > _scores[best]) best = i;
                return best;
            }
        }

        /// <summary>The winning TEAM once ended (-1 while running).</summary>
        public int WinnerTeam
        {
            get
            {
                if (!IsOver) return -1;
                int bestTeam = _teamOf[0];
                int bestScore = TeamScore(bestTeam);
                for (int i = 1; i < PlayerCount; i++)
                {
                    int t = _teamOf[i];
                    int s = TeamScore(t);
                    if (s > bestScore) { bestScore = s; bestTeam = t; }
                }
                return bestTeam;
            }
        }

        public void Begin()
        {
            for (int i = 0; i < PlayerCount; i++) _scores[i] = 0;
            Phase = PvpPhase.Active;
        }

        /// <summary>
        /// Credit a kill to <paramref name="killerIndex"/>. Returns true if this kill ENDED the match
        /// (the killer's TEAM total reached the target). Ignored unless the match is Active.
        /// </summary>
        public bool RegisterKill(int killerIndex)
        {
            if (Phase != PvpPhase.Active) return false;
            int i = Clamp(killerIndex);
            _scores[i]++;
            if (TeamScore(_teamOf[i]) >= KillsToWin)
            {
                Phase = PvpPhase.Ended;
                return true;
            }
            return false;
        }

        /// <summary>Modes that end by their own rule (KotH time, ladder finish, wave wipe) close the
        /// match through here so phase/winner semantics stay in one place.</summary>
        public void EndByRule()
        {
            if (Phase == PvpPhase.Active) Phase = PvpPhase.Ended;
        }

        private int Clamp(int i) => i < 0 ? 0 : (i >= PlayerCount ? PlayerCount - 1 : i);
    }
}
