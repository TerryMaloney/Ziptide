using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Scene brain for a 1v1 PvP match. Owns the authoritative <see cref="PvpMatch"/>, registers the two
    /// combatants (local player index 0, opponent index 1), credits kills on death, and drives the
    /// end/rematch flow. The HUD reads from here. Combatants talk to it via the interface, so the bot
    /// swaps for a networked remote avatar (Phase 4) with no changes here.
    /// </summary>
    public class PvpMatchDirector : MonoBehaviour
    {
        public static PvpMatchDirector Instance { get; private set; }

        [Tooltip("Seconds after the match ends before it auto-restarts (rematch).")]
        public float rematchDelay = 6f;

        private PvpMatch _match;
        private readonly IPvpDamageable[] _combatants = new IPvpDamageable[2];
        private float _rematchAt;

        public PvpMatch Match => _match;
        public int Score(int i) => _match != null ? _match.Score(i) : 0;
        public PvpPhase Phase => _match != null ? _match.Phase : PvpPhase.Lobby;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
            _match = new PvpMatch();
        }

        private void Start()
        {
            _match.Begin();
            Debug.Log("ZIPTIDE: PVP_MATCH_BEGIN killsToWin=" + _match.KillsToWin);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Register(IPvpDamageable combatant)
        {
            if (combatant == null) return;
            int i = combatant.PlayerIndex;
            if (i >= 0 && i < 2) _combatants[i] = combatant;
        }

        /// <summary>A combatant died. Credit the other player; handle match end.</summary>
        public void ReportDeath(int killedIndex)
        {
            if (_match == null || _match.Phase != PvpPhase.Active) return;
            var (killer, ended) = PvpRoundLogic.ResolveDeath(_match, killedIndex);
            Debug.Log("ZIPTIDE: PVP_KILL killer=" + killer + " score=" + _match.Score(0) + "-" + _match.Score(1));
            if (ended)
            {
                _rematchAt = Time.time + rematchDelay;
                Debug.Log("ZIPTIDE: PVP_MATCH_END winner=" + _match.WinnerIndex);
            }
        }

        private void Update()
        {
            if (_match != null && _match.IsOver && Time.time >= _rematchAt)
                Rematch();
        }

        private void Rematch()
        {
            _match = new PvpMatch();
            _match.Begin();
            Debug.Log("ZIPTIDE: PVP_REMATCH");
        }
    }
}
