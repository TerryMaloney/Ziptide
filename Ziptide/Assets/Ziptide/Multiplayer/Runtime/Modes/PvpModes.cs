using System;
using System.Collections.Generic;

namespace Ziptide.Multiplayer.Modes
{
    public enum PvpModeKind { Deathmatch, GunGame, KingOfTheHill, FragmentRush, Horde }

    /// <summary>
    /// GUN GAME — every kill advances you down the weapon ladder; finish the ladder to win.
    /// Pure: the scene (mode director) maps rungs to real weapons via ItemFactory ids.
    /// </summary>
    public sealed class GunGameState
    {
        public IReadOnlyList<string> Ladder { get; }
        private readonly int[] _rung;

        public GunGameState(int playerCount, IList<string> ladder = null)
        {
            Ladder = ladder != null && ladder.Count > 0 ? new List<string>(ladder) : DefaultLadder;
            _rung = new int[Math.Max(2, playerCount)];
        }

        // The classic gun-game finish: the last rung is the melee blade — win the match with a
        // contact swing or don't win it at all (MP100 wave 1 added the rung).
        public static readonly string[] DefaultLadder =
            { "taser_dart_gun", "pistol", "gravity_gun", "static_net", "sonic_thumper", "prism_beam", "breaker_blade" };

        public int Rung(int player) => _rung[player];
        public string CurrentWeapon(int player) => Ladder[Math.Min(_rung[player], Ladder.Count - 1)];

        /// <summary>Advance the killer's ladder. True = they finished it (match over).</summary>
        public bool OnKill(int killer)
        {
            _rung[killer]++;
            return _rung[killer] >= Ladder.Count;
        }
    }

    /// <summary>
    /// KING OF THE HILL — hold the active zone to accrue seconds; the zone rotates on a timer;
    /// first to the target hold time wins. Contested (2+ inside) accrues nobody.
    /// </summary>
    public sealed class KothState
    {
        public int ZoneCount { get; }
        public float RotateEverySeconds { get; }
        public float TargetHoldSeconds { get; }
        public int ActiveZone { get; private set; }

        private readonly float[] _hold;
        private float _nextRotateAt = float.MinValue;
        private float _lastTickAt = float.MinValue;

        public KothState(int playerCount, int zoneCount, float rotateEverySeconds = 45f, float targetHoldSeconds = 90f)
        {
            ZoneCount = Math.Max(1, zoneCount);
            RotateEverySeconds = rotateEverySeconds;
            TargetHoldSeconds = targetHoldSeconds;
            _hold = new float[Math.Max(2, playerCount)];
        }

        public float Hold(int player) => _hold[player];

        /// <summary>
        /// Advance the clock with who's inside the ACTIVE zone right now. Returns the winner index the
        /// tick their hold crosses the target, else -1.
        /// </summary>
        public int Tick(float now, IList<int> playersInActiveZone)
        {
            if (_lastTickAt <= float.MinValue) { _lastTickAt = now; _nextRotateAt = now + RotateEverySeconds; return -1; }
            float dt = now - _lastTickAt;
            _lastTickAt = now;
            if (dt <= 0f) return -1;

            if (now >= _nextRotateAt)
            {
                ActiveZone = (ActiveZone + 1) % ZoneCount;
                _nextRotateAt = now + RotateEverySeconds;
            }

            if (playersInActiveZone != null && playersInActiveZone.Count == 1) // sole king only
            {
                int p = playersInActiveZone[0];
                if (p >= 0 && p < _hold.Length)
                {
                    _hold[p] += dt;
                    if (_hold[p] >= TargetHoldSeconds) return p;
                }
            }
            return -1;
        }
    }

    /// <summary>
    /// FRAGMENT RUSH — grab the fragment at mid, bank it at your base (story-flavored CTF). The
    /// carrier is exposed so the wrist locator can ping them; dropping resets it to mid (simple + fair).
    /// </summary>
    public sealed class FragmentRushState
    {
        public int TargetBanks { get; }
        public int CarrierIndex { get; private set; } = -1;
        public bool FragmentAtHome { get; private set; } = true;   // sitting at mid

        private readonly int[] _banks;

        public FragmentRushState(int playerCount, int targetBanks = 3)
        {
            TargetBanks = targetBanks;
            _banks = new int[Math.Max(2, playerCount)];
        }

        public int Banks(int player) => _banks[player];

        public bool PickUp(int player)
        {
            if (!FragmentAtHome || CarrierIndex >= 0) return false;
            CarrierIndex = player;
            FragmentAtHome = false;
            return true;
        }

        /// <summary>Carrier died/dropped — the fragment resets to mid.</summary>
        public void Drop()
        {
            CarrierIndex = -1;
            FragmentAtHome = true;
        }

        /// <summary>Carrier reached their bank. True = that bank WON the match.</summary>
        public bool Bank(int player)
        {
            if (CarrierIndex != player) return false;
            _banks[player]++;
            CarrierIndex = -1;
            FragmentAtHome = true;
            return _banks[player] >= TargetBanks;
        }
    }

    /// <summary>
    /// HORDE — survive escalating waves of bots + creatures. The composition is deterministic per
    /// wave; the scene spawns what <see cref="WaveBots"/>/<see cref="WaveCreatures"/> say and reports
    /// downs. Score = kills + wave-clear bonuses; the "result" is the wave you reached.
    /// </summary>
    public sealed class HordeState
    {
        public int Wave { get; private set; }
        public int Alive { get; private set; }
        public int Score { get; private set; }
        public bool WaveActive { get; private set; }

        public const int WaveClearBonus = 5;
        private static readonly string[] CreatureCycle = { "swarm_bug", "tendril", "swarm_bug", "warden" };

        /// <summary>Bots in wave w: one more every 3 waves, capped at 3 (Quest perf).</summary>
        public static int WaveBots(int wave) => Math.Min(1 + (wave - 1) / 3, 3);

        /// <summary>Creature ids for wave w (count grows with the wave, capped at 6 concurrent).</summary>
        public static List<string> WaveCreatures(int wave)
        {
            int count = Math.Min(1 + wave, 6);
            var list = new List<string>(count);
            for (int i = 0; i < count; i++) list.Add(CreatureCycle[(wave + i) % CreatureCycle.Length]);
            return list;
        }

        /// <summary>Start the next wave; returns its number. The scene spawns WaveBots+WaveCreatures.</summary>
        public int NextWave()
        {
            Wave++;
            Alive = WaveBots(Wave) + WaveCreatures(Wave).Count;
            WaveActive = true;
            return Wave;
        }

        /// <summary>An enemy went down. True = the wave is CLEAR (scene celebrates, then NextWave).</summary>
        public bool RegisterDown()
        {
            if (!WaveActive || Alive <= 0) return false;
            Alive--;
            Score++;
            if (Alive == 0)
            {
                WaveActive = false;
                Score += WaveClearBonus;
                return true;
            }
            return false;
        }
    }
}
