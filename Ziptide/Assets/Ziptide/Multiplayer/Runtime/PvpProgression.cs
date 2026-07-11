using System.Collections.Generic;

namespace Ziptide.Multiplayer
{
    /// <summary>
    /// MP100 §F / A5 — per-match stat accumulation, pure (the PvpRules pattern: no UnityEngine,
    /// deterministic, EditMode-tested). Feed it the same events the match director already fires
    /// (KillScored) and read stats at match end.
    /// </summary>
    public sealed class MatchStatsCore
    {
        private readonly int[] _kills;
        private readonly int[] _downs;
        private readonly int[] _streak;
        private readonly int[] _bestStreak;

        public MatchStatsCore(int combatants)
        {
            int n = combatants < 1 ? 1 : combatants;
            _kills = new int[n];
            _downs = new int[n];
            _streak = new int[n];
            _bestStreak = new int[n];
        }

        public void RecordKill(int killer, int killed)
        {
            if (killer >= 0 && killer < _kills.Length && killer != killed)
            {
                _kills[killer]++;
                _streak[killer]++;
                if (_streak[killer] > _bestStreak[killer]) _bestStreak[killer] = _streak[killer];
            }
            if (killed >= 0 && killed < _downs.Length)
            {
                _downs[killed]++;
                _streak[killed] = 0; // dying ends your run
            }
        }

        public int Kills(int i) => i >= 0 && i < _kills.Length ? _kills[i] : 0;
        public int Downs(int i) => i >= 0 && i < _downs.Length ? _downs[i] : 0;
        public int BestStreak(int i) => i >= 0 && i < _bestStreak.Length ? _bestStreak[i] : 0;
    }

    /// <summary>One payout, itemized so the summary can SHOW why (readability is the reward).</summary>
    public struct PvpPayoutBreakdown
    {
        public int BaseCredits, KillBonus, StreakBonus, WinBonus, DailyBonus, FirstWinBonus;
        public double DifficultyMult;
        public int Total;
    }

    /// <summary>
    /// MP100 §F / A5 — the progression rules, pure: credits payout (economy-safe by clamp, the
    /// garden precedent), the unlock ladder (arenas → Veteran → Nightmare → mutators, MP100 #61),
    /// the daily challenge pick (deterministic per UTC day — same combo for everyone, #62), and
    /// first-win-of-the-day bookkeeping (#65). The scene layer routes the result through
    /// RewardRouter/profile flags — this file never touches a profile.
    /// </summary>
    public static class PvpProgression
    {
        // Difficulty ids follow Resources/Bots + the lobby row exactly.
        public const string Rookie = "rookie", Regular = "regular",
                            Veteran = "veteran", Nightmare = "nightmare";

        /// <summary>Unlock flag names (profile flag strings — constants live here so the pure layer
        /// and the scene layer can't drift).</summary>
        public const string FlagVeteranUnlocked = "MP_VETERAN_UNLOCKED";
        public const string FlagNightmareUnlocked = "MP_NIGHTMARE_UNLOCKED";
        public const string FlagMutatorsUnlocked = "MP_MUTATORS_UNLOCKED";

        /// <summary>Wins at Regular-or-above that unlock Veteran / wins at Veteran-or-above that
        /// unlock Nightmare / Nightmare wins that unlock mutators.</summary>
        public const int VeteranAfterWins = 3, NightmareAfterWins = 3, MutatorsAfterWins = 1;

        public const int PayoutCap = 150; // a single match can never out-earn a story job chain

        public static double DifficultyMult(string difficultyId)
        {
            switch (difficultyId)
            {
                case Regular: return 1.3;
                case Veteran: return 1.7;
                case Nightmare: return 2.2;
                case Rookie: return 1.0;
                default: return 1.0; // unknown/empty (arena default) — never a jackpot by typo
            }
        }

        public static PvpPayoutBreakdown ComputePayout(int kills, int bestStreak, bool won,
            string difficultyId, bool isDailyChallenge, bool firstWinToday)
        {
            var p = new PvpPayoutBreakdown
            {
                BaseCredits = 5,
                KillBonus = 2 * (kills < 0 ? 0 : kills),
                StreakBonus = bestStreak >= 5 ? 6 : bestStreak >= 3 ? 3 : 0,
                WinBonus = won ? 10 : 0,
                DifficultyMult = DifficultyMult(difficultyId),
                // Challenge bonuses reward the WIN, not the attempt — no daily farm by idling.
                DailyBonus = isDailyChallenge && won ? 15 : 0,
                FirstWinBonus = firstWinToday && won ? 10 : 0,
            };
            double skill = (p.BaseCredits + p.KillBonus + p.StreakBonus + p.WinBonus) * p.DifficultyMult;
            int total = (int)System.Math.Round(skill) + p.DailyBonus + p.FirstWinBonus;
            p.Total = total < 0 ? 0 : total > PayoutCap ? PayoutCap : total;
            return p;
        }

        /// <summary>Career win counts → every unlock flag currently earned (idempotent — the caller
        /// SetFlags; already-set flags are no-ops). Regular-and-above wins ladder upward so a player
        /// who skips straight to harder rows still progresses.</summary>
        public static List<string> EarnedUnlocks(int winsRegular, int winsVeteran, int winsNightmare)
        {
            var flags = new List<string>();
            if (winsRegular + winsVeteran + winsNightmare >= VeteranAfterWins)
                flags.Add(FlagVeteranUnlocked);
            if (winsVeteran + winsNightmare >= NightmareAfterWins)
                flags.Add(FlagNightmareUnlocked);
            if (winsNightmare >= MutatorsAfterWins)
                flags.Add(FlagMutatorsUnlocked);
            return flags;
        }

        /// <summary>Unix seconds → UTC day number (the first-win / daily bookkeeping unit).</summary>
        public static int UtcDayNumber(long unixSeconds)
            => unixSeconds <= 0 ? 0 : (int)(unixSeconds / 86400L);

        /// <summary>FNV-1a over the day number — platform-stable (the BeltPuckStyle precedent).</summary>
        public static uint DayHash(int dayNumber)
        {
            uint h = 2166136261u;
            for (int i = 0; i < 4; i++)
            {
                h ^= (uint)((dayNumber >> (i * 8)) & 0xFF);
                h *= 16777619u;
            }
            return h;
        }

        /// <summary>MP100 #62 — the daily challenge: a deterministic arena/mode/difficulty combo,
        /// identical for every player on the same UTC day. Difficulty stays in the earnable band
        /// (regular/veteran/nightmare — a daily is a CHALLENGE, never a rookie stroll).</summary>
        public static void PickDaily(int dayNumber, int arenaCount, int modeCount,
            out int arenaIndex, out int modeIndex, out string difficultyId)
        {
            uint h = DayHash(dayNumber);
            arenaIndex = arenaCount > 0 ? (int)(h % (uint)arenaCount) : 0;
            modeIndex = modeCount > 0 ? (int)((h >> 8) % (uint)modeCount) : 0;
            switch ((h >> 16) % 3)
            {
                case 0: difficultyId = Regular; break;
                case 1: difficultyId = Veteran; break;
                default: difficultyId = Nightmare; break;
            }
        }
    }
}
