using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Core;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// MP100 §F / A5 — the progression scene body: every arena match now PAYS. Self-bootstrapped
    /// like SaveSystem (no scene edit, so every already-committed arena gets it): on each scene
    /// load it binds the scene's PvpMatchDirector, accumulates the pure MatchStatsCore from the
    /// director's existing KillScored events, and at MatchEnded routes the payout through the ONE
    /// economy path (RewardRouter, LedgerSource.Multiplayer, "credits"), updates the arena career
    /// (wins per difficulty, best streak, day stamps), applies newly-earned unlock flags
    /// (Veteran → Nightmare → mutators), and autosaves. Local player = combatant 0, the law.
    /// Logs ZIPTIDE: PVP_PAYOUT / PVP_UNLOCK.
    /// </summary>
    public class PvpProgressionRuntime : MonoBehaviour
    {
        private const int MaxCombatants = 4;

        private static PvpProgressionRuntime _instance;
        private PvpMatchDirector _director;
        private MatchStatsCore _stats = new MatchStatsCore(MaxCombatants);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.PvpProgression)) return;
            if (_instance != null) return;
            var go = new GameObject("PvpProgression");
            _instance = go.AddComponent<PvpProgressionRuntime>();
            DontDestroyOnLoad(go);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Bind();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Unbind();
        }

        private void OnSceneLoaded(Scene s, LoadSceneMode m) => Bind();

        private void Bind()
        {
            Unbind();
            _director = FindObjectOfType<PvpMatchDirector>();
            if (_director == null) return;
            _director.KillScored += OnKill;
            _director.MatchEnded += OnMatchEnded;
            _stats = new MatchStatsCore(MaxCombatants);
        }

        private void Unbind()
        {
            if (_director == null) return;
            _director.KillScored -= OnKill;
            _director.MatchEnded -= OnMatchEnded;
            _director = null;
        }

        private void OnKill(int killer, int killed) => _stats.RecordKill(killer, killed);

        private void OnMatchEnded(int winnerIndex)
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            var stats = _stats;
            _stats = new MatchStatsCore(MaxCombatants); // rematch starts clean either way
            if (profile == null) return;

            bool won = winnerIndex == 0;
            string difficulty = ArenaMatchConfig.Difficulty;
            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int day = PvpProgression.UtcDayNumber(now);
            var career = profile.pvpCareer;

            bool firstWinToday = won && career.lastFirstWinDay != day;
            // The daily pays once per day — beating it again is just a match.
            bool dailyCounts = ArenaMatchConfig.DailyChallenge && career.lastDailyWinDay != day;

            var pay = PvpProgression.ComputePayout(stats.Kills(0), stats.BestStreak(0), won,
                difficulty, dailyCounts, firstWinToday);
            if (pay.Total > 0)
                RewardRouter.Grant(profile, LedgerSource.Multiplayer, "credits", pay.Total,
                    reason: "pvp_match", relatedId: ArenaMatchConfig.Mode + "/" + difficulty,
                    worldId: gameObject.scene.name);

            career.matches++;
            if (won)
            {
                switch (difficulty)
                {
                    case PvpProgression.Regular: career.winsRegular++; break;
                    case PvpProgression.Veteran: career.winsVeteran++; break;
                    case PvpProgression.Nightmare: career.winsNightmare++; break;
                    default: career.winsRookie++; break; // rookie + arena-default
                }
                if (firstWinToday) career.lastFirstWinDay = day;
                if (dailyCounts) career.lastDailyWinDay = day;
            }
            if (stats.BestStreak(0) > career.bestStreakEver)
                career.bestStreakEver = stats.BestStreak(0);

            foreach (var flag in PvpProgression.EarnedUnlocks(
                career.winsRegular, career.winsVeteran, career.winsNightmare))
            {
                if (profile.HasFlag(flag)) continue;
                profile.SetFlag(flag);
                Debug.Log("ZIPTIDE: PVP_UNLOCK flag=" + flag);
            }

            Debug.Log("ZIPTIDE: PVP_PAYOUT credits=" + pay.Total + " won=" + won
                + " kills=" + stats.Kills(0) + " streak=" + stats.BestStreak(0)
                + " mult=" + pay.DifficultyMult.ToString("F1")
                + " daily=" + dailyCounts + " firstWin=" + firstWinToday);
            SaveSystem.AutosaveNow("pvp_match");
        }
    }
}
