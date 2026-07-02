using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Scene brain for a PvP match. Owns the authoritative <see cref="PvpMatch"/>, registers combatants
    /// (local player index 0, bots/remotes 1–3), credits kills on death, and drives the end/rematch flow.
    /// The HUD reads from here. A3-scene: generalized from 1v1 — kill credit resolves through
    /// <see cref="PvpHitSource"/> (the weapon reports its firer the same frame) with the classic 1v1
    /// inference as the fallback, and <see cref="PvpModeDirector"/> hooks the events to run objective
    /// modes on top. With no mode director present this behaves exactly like the shipped 1v1 loop.
    /// </summary>
    public class PvpMatchDirector : MonoBehaviour
    {
        public static PvpMatchDirector Instance { get; private set; }

        [Tooltip("Seconds after the match ends before it auto-restarts (rematch).")]
        public float rematchDelay = 6f;

        /// <summary>(killer, killed) after every scored kill — mode rules hang off this.</summary>
        public event System.Action<int, int> KillScored;
        /// <summary>Winner index when the match ends (kill limit OR a mode rule).</summary>
        public event System.Action<int> MatchEnded;
        /// <summary>Fired when a rematch begins — mode/HUD state resets here.</summary>
        public event System.Action MatchRestarted;

        private PvpMatch _match;
        private readonly IPvpDamageable[] _combatants = new IPvpDamageable[4];
        private int _playerCount = 2;
        private int _killsToWin = -1;
        private float _rematchAt;

        public PvpMatch Match => _match;
        public int PlayerCount => _playerCount;
        public int Score(int i) => _match != null ? _match.Score(i) : 0;
        public PvpPhase Phase => _match != null ? _match.Phase : PvpPhase.Lobby;
        public IPvpDamageable Combatant(int i) => i >= 0 && i < _combatants.Length ? _combatants[i] : null;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
            _match = new PvpMatch();
        }

        private void Start()
        {
            _match.Begin();
            Debug.Log("ZIPTIDE: PVP_MATCH_BEGIN killsToWin=" + _match.KillsToWin + " players=" + _playerCount);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>The mode director sizes the match before/at round start (idempotent).</summary>
        public void Reconfigure(int playerCount, int killsToWin)
        {
            _playerCount = Mathf.Clamp(playerCount, 2, 4);
            _killsToWin = killsToWin;
            _match = new PvpMatch(_playerCount, killsToWin);
            _match.Begin();
            Debug.Log("ZIPTIDE: PVP_MATCH_RECONFIG players=" + _playerCount + " killsToWin=" + killsToWin);
        }

        public void Register(IPvpDamageable combatant)
        {
            if (combatant == null) return;
            int i = combatant.PlayerIndex;
            if (i >= 0 && i < _combatants.Length) _combatants[i] = combatant; // PlayerIndex law: -1 never registers
        }

        /// <summary>A combatant died. Credit the reported attacker (same-frame weapon report); fall back
        /// to the 1v1 inference so the original arena keeps its exact behavior.</summary>
        public void ReportDeath(int killedIndex)
        {
            if (_match == null || _match.Phase != PvpPhase.Active) return;
            int killer = PvpHitSource.Current;
            if (killer < 0 || killer == killedIndex || killer >= _playerCount)
                killer = PvpRoundLogic.KillerOf(killedIndex);
            bool ended = _match.RegisterKill(killer);
            Debug.Log("ZIPTIDE: PVP_KILL killer=" + killer + " killed=" + killedIndex
                + " score=" + _match.Score(0) + "-" + _match.Score(1));
            KillScored?.Invoke(killer, killedIndex);
            if (ended) EndNow();
        }

        /// <summary>Objective modes (ladder finished, hold target, banks) end the match through here.</summary>
        public void EndByModeRule()
        {
            if (_match == null || _match.Phase != PvpPhase.Active) return;
            _match.EndByRule();
            EndNow();
        }

        private void EndNow()
        {
            _rematchAt = Time.time + rematchDelay;
            Debug.Log("ZIPTIDE: PVP_MATCH_END winner=" + _match.WinnerIndex);
            MatchEnded?.Invoke(_match.WinnerIndex);
        }

        private void Update()
        {
            if (_match != null && _match.IsOver && Time.time >= _rematchAt)
                Rematch();
        }

        private void Rematch()
        {
            _match = new PvpMatch(_playerCount, _killsToWin);
            _match.Begin();
            Debug.Log("ZIPTIDE: PVP_REMATCH");
            MatchRestarted?.Invoke();
        }
    }
}
