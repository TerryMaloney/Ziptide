using System.Collections.Generic;
using UnityEngine;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Modes;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE MODE DIRECTOR (M7a A3-scene, design docs/design/PVP_ARENA_AAA.md §A3) — the thin scene body
    /// for the pure mode engines. Reads the lobby's <see cref="ArenaMatchConfig"/>, sizes the match on
    /// <see cref="PvpMatchDirector"/>, staffs the bot pool, then per mode: GUN GAME advances the ladder
    /// and racks the player's next weapon; KOTH ticks the hill (bots contest via the objective magnet);
    /// FRAGMENT RUSH runs you-carry-they-hunt (v1: bots guard, only the player banks — readable + fair);
    /// HORDE spawns deterministic bot+creature waves (creatures poll — their file is story-lane, index -1
    /// per the PlayerIndex law). Absent (legacy PvP_Arena01), nothing changes anywhere.
    /// </summary>
    public class PvpModeDirector : MonoBehaviour
    {
        public static PvpModeDirector Instance { get; private set; }

        [Tooltip("Seconds of breather between cleared Horde waves.")]
        public float hordeWaveDelay = 4f;


        private PvpMatchDirector _dir;
        private PvpPlayer _player;
        private Transform _playerHead;
        private Vector3 _playerBankPos;

        private PvpModeKind _mode;
        private GunGameState _gunGame;
        private KothState _koth;
        private FragmentRushState _frag;
        private HordeState _horde;

        // Bot pool: [0] = the patcher-built arena bot; the rest are runtime-staffed for N-way matches.
        private readonly List<PvpBot> _bots = new List<PvpBot>();
        private readonly List<GameObject> _spawnedEnemies = new List<GameObject>(); // runtime bots + wave creatures
        private readonly List<CreatureRuntime> _waveCreatures = new List<CreatureRuntime>();
        private readonly List<bool> _creatureWasAlive = new List<bool>();
        private float _nextWaveAt;
        private bool _betweenWaves;

        // Zones (patcher-baked under __PVP_ZONES) + runtime visuals.
        private struct Zone { public string Id; public Vector3 Pos; public float Radius; public Renderer Ring; }
        private readonly List<Zone> _zones = new List<Zone>();
        private readonly List<Vector3> _spawnPoints = new List<Vector3>(); // waypoint ring reused as spawn spots

        private GameObject _fragmentGo;
        private string _status = "";

        /// <summary>One line the HUD appends under the score.</summary>
        public string StatusLine => _status;
        public PvpModeKind Mode => _mode;

        private static readonly Color ZoneIdle = new Color(0.25f, 0.30f, 0.35f);
        private static readonly Color ZoneActive = new Color(0.95f, 0.78f, 0.20f);
        private static readonly Color FragmentColor = new Color(1f, 0.84f, 0.25f);

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_dir != null)
            {
                _dir.KillScored -= OnKill;
                _dir.MatchRestarted -= OnRematch;
            }
        }

        private void Start()
        {
            _dir = PvpMatchDirector.Instance;
            _player = FindObjectOfType<PvpPlayer>();
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _playerHead = rig.GetComponentInChildren<Camera>()?.transform;
            if (_playerHead == null && Camera.main != null) _playerHead = Camera.main.transform;

            var arenaBot = FindObjectOfType<PvpBot>();
            if (arenaBot != null) _bots.Add(arenaBot);

            LoadZonesAndSpawns();
            var spawnGo = GameObject.Find(Ziptide.Core.ZiptideConstants.GoSpawnPlayer);
            _playerBankPos = spawnGo != null ? spawnGo.transform.position
                           : (_playerHead != null ? _playerHead.position : Vector3.zero);

            if (_dir != null)
            {
                _dir.KillScored += OnKill;
                _dir.MatchRestarted += OnRematch;
            }
            ApplyConfig();
        }

        /// <summary>The lobby board applies a new selection here; rematches route through it too.</summary>
        public void Restart()
        {
            CleanupRound();
            ApplyConfig();
        }

        private void OnRematch() => Restart();

        // ── Configure a round from ArenaMatchConfig ──────────────────────────────────────────────
        private void ApplyConfig()
        {
            _mode = ArenaMatchConfig.Mode;
            _gunGame = null; _koth = null; _frag = null; _horde = null;
            _betweenWaves = false;
            _status = "";

            int lobbyBots = ArenaModeSetup.UsesLobbyBots(_mode) ? Mathf.Clamp(ArenaMatchConfig.BotCount, 1, 3) : 0;
            int combatants = ArenaModeSetup.CombatantCount(Mathf.Max(1, lobbyBots));
            _dir?.Reconfigure(combatants, ArenaModeSetup.KillsToWinFor(_mode));

            StaffBots(lobbyBots);

            switch (_mode)
            {
                case PvpModeKind.GunGame:
                    _gunGame = new GunGameState(combatants); // A4: the full six-weapon DefaultLadder
                    RackPlayerWeapon(0);
                    break;
                case PvpModeKind.KingOfTheHill:
                    _koth = new KothState(combatants, Mathf.Max(1, _zones.Count));
                    BuildZoneRings();
                    break;
                case PvpModeKind.FragmentRush:
                    _frag = new FragmentRushState(combatants);
                    BuildZoneRings();
                    BuildFragment();
                    break;
                case PvpModeKind.Horde:
                    _horde = new HordeState();
                    _betweenWaves = true;
                    _nextWaveAt = Time.time + hordeWaveDelay;
                    break;
            }
            Debug.Log("ZIPTIDE: PVP_MODE_START mode=" + _mode + " bots=" + lobbyBots
                + " difficulty=" + (string.IsNullOrEmpty(ArenaMatchConfig.Difficulty) ? "(arena)" : ArenaMatchConfig.Difficulty));
        }

        private void CleanupRound()
        {
            foreach (var go in _spawnedEnemies) if (go != null && !HeldByPlayer(go)) Destroy(go);
            _spawnedEnemies.Clear();
            _waveCreatures.Clear();
            _creatureWasAlive.Clear();
            if (_bots.Count > 1) _bots.RemoveRange(1, _bots.Count - 1); // runtime staff destroyed above
            if (_fragmentGo != null) { Destroy(_fragmentGo); _fragmentGo = null; }
            for (int i = 0; i < _zones.Count; i++)
                if (_zones[i].Ring != null) { Destroy(_zones[i].Ring.gameObject); var z = _zones[i]; z.Ring = null; _zones[i] = z; }
            foreach (var b in _bots)
                if (b != null) { b.hasObjective = false; b.gameObject.SetActive(true); }
        }

        /// <summary>Size the bot pool: apply the lobby difficulty to all, spawn extras at waypoints,
        /// park the arena bot in Horde (the wave spawner owns its own).</summary>
        private void StaffBots(int lobbyBots)
        {
            var arenaBot = _bots.Count > 0 ? _bots[0] : null;
            if (arenaBot != null)
            {
                arenaBot.gameObject.SetActive(lobbyBots >= 1);
                arenaBot.autoRevive = _mode != PvpModeKind.Horde;
                arenaBot.SetDifficulty(ArenaMatchConfig.Difficulty);
            }
            for (int i = 2; i <= lobbyBots; i++)
            {
                Vector3 pos = _spawnPoints.Count >= i ? _spawnPoints[i - 1] + Vector3.up * 1.1f
                            : (arenaBot != null ? arenaBot.transform.position + new Vector3(2f * (i - 1), 0f, 0f) : Vector3.up * 1.1f);
                var bot = SpawnBot(pos, i, autoRevive: true);
                _bots.Add(bot);
            }
        }

        private PvpBot SpawnBot(Vector3 pos, int index, bool autoRevive)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "PvpBot_" + index;
            go.transform.position = pos;
            var pb = go.AddComponent<PvpBot>();
            pb.playerIndex = index;
            pb.autoRevive = autoRevive;
            pb.difficulty = string.IsNullOrEmpty(ArenaMatchConfig.Difficulty)
                ? (_bots.Count > 0 && _bots[0] != null ? _bots[0].difficulty : "regular")
                : ArenaMatchConfig.Difficulty;
            _spawnedEnemies.Add(go);
            return pb;
        }

        // ── Kill routing (the match director already credited the score) ──────────────────────────
        private void OnKill(int killer, int killed)
        {
            if (_gunGame != null)
            {
                bool finished = _gunGame.OnKill(killer);
                Debug.Log("ZIPTIDE: GUNGAME_RUNG player=" + killer + " rung=" + _gunGame.Rung(killer));
                if (killer == 0 && !finished) RackPlayerWeapon(_gunGame.Rung(0));
                if (finished) _dir?.EndByModeRule();
            }
            if (_frag != null && killed == _frag.CarrierIndex)
            {
                _frag.Drop();
                PlaceFragmentAtMid();
                Debug.Log("ZIPTIDE: FRAG_DROP killed=" + killed);
            }
            if (_horde != null && killed > 0 && _horde.RegisterDown()) OnWaveClear();
        }

        private void Update()
        {
            switch (_mode)
            {
                case PvpModeKind.KingOfTheHill: TickKoth(); break;
                case PvpModeKind.FragmentRush: TickFragment(); break;
                case PvpModeKind.Horde: TickHorde(); break;
                case PvpModeKind.GunGame:
                    if (_gunGame != null)
                        _status = "GUN GAME  " + (_gunGame.Rung(0) + 1) + "/" + _gunGame.Ladder.Count
                            + "  " + PrettyItem(_gunGame.CurrentWeapon(0));
                    break;
                default: _status = ""; break;
            }
        }

        // ── KING OF THE HILL ───────────────────────────────────────────────────────────────────────
        private void TickKoth()
        {
            if (_koth == null || _zones.Count == 0 || _dir == null || _dir.Phase != PvpPhase.Active) return;
            var zone = _zones[_koth.ActiveZone % _zones.Count];

            var inside = new List<int>(4);
            if (PlayerPos(out var pp) && FlatDist(pp, zone.Pos) <= zone.Radius) inside.Add(0);
            foreach (var b in _bots)
            {
                if (b == null || !b.IsAlive || !b.gameObject.activeSelf) continue;
                b.hasObjective = true; b.objectivePoint = zone.Pos;   // contest the hill
                if (FlatDist(b.transform.position, zone.Pos) <= zone.Radius) inside.Add(b.playerIndex);
            }

            int before = _koth.ActiveZone;
            int winner = _koth.Tick(Time.time, inside);
            if (_koth.ActiveZone != before) Debug.Log("ZIPTIDE: KOTH_ROTATE zone=" + _koth.ActiveZone);
            HighlightZones(_koth.ActiveZone % _zones.Count);

            _status = "KOTH  hold " + Mathf.FloorToInt(_koth.Hold(0)) + "s/" + Mathf.FloorToInt(_koth.TargetHoldSeconds)
                + "s" + (inside.Count > 1 ? "  CONTESTED" : "");
            if (winner >= 0)
            {
                Debug.Log("ZIPTIDE: KOTH_WIN player=" + winner);
                _dir.EndByModeRule();
            }
        }

        // ── FRAGMENT RUSH (v1: the player runs it, bots hunt) ─────────────────────────────────────
        private void TickFragment()
        {
            if (_frag == null || _dir == null || _dir.Phase != PvpPhase.Active) return;
            Vector3 mid = FragMidPos();

            if (_frag.FragmentAtHome && PlayerPos(out var pp) && FlatDist(pp, mid) <= 1.1f && _frag.PickUp(0))
                Debug.Log("ZIPTIDE: FRAG_PICKUP player=0");

            bool carrying = _frag.CarrierIndex == 0;
            if (_fragmentGo != null && PlayerPos(out var headPos))
                _fragmentGo.transform.position = carrying ? headPos + Vector3.up * 0.45f : mid;

            foreach (var b in _bots)
            {
                if (b == null || !b.gameObject.activeSelf) continue;
                b.hasObjective = true;
                b.objectivePoint = carrying && _playerHead != null ? _playerHead.position : mid; // shadow the prize
            }

            if (carrying && PlayerPos(out var p2) && FlatDist(p2, _playerBankPos) <= 1.6f)
            {
                bool won = _frag.Bank(0);
                Debug.Log("ZIPTIDE: FRAG_BANK banks=" + _frag.Banks(0) + "/" + _frag.TargetBanks);
                PlaceFragmentAtMid();
                if (won) _dir.EndByModeRule();
            }

            _status = "FRAGMENT  banked " + _frag.Banks(0) + "/" + _frag.TargetBanks
                + (carrying ? "  CARRYING — RUN IT HOME" : "  fragment at mid");
        }

        // ── HORDE ──────────────────────────────────────────────────────────────────────────────────
        private void TickHorde()
        {
            if (_horde == null) return;
            if (_betweenWaves)
            {
                _status = "HORDE  wave " + (_horde.Wave + 1) + " incoming…  score " + _horde.Score;
                if (Time.time >= _nextWaveAt) StartWave();
                return;
            }
            // Creatures can't report deaths (PlayerIndex -1, story-lane file) — poll the transition.
            for (int i = 0; i < _waveCreatures.Count; i++)
            {
                var c = _waveCreatures[i];
                bool alive = c != null && c.IsAlive;
                if (_creatureWasAlive[i] && !alive)
                {
                    _creatureWasAlive[i] = false;
                    if (_horde.RegisterDown()) { OnWaveClear(); return; }
                }
            }
            _status = "HORDE  wave " + _horde.Wave + "  left " + _horde.Alive + "  score " + _horde.Score;
        }

        private void StartWave()
        {
            _betweenWaves = false;
            foreach (var go in _spawnedEnemies) if (go != null) Destroy(go); // clear last wave's downed shells
            _spawnedEnemies.Clear();
            _waveCreatures.Clear();
            _creatureWasAlive.Clear();
            if (_bots.Count > 1) _bots.RemoveRange(1, _bots.Count - 1);

            int wave = _horde.NextWave();
            int nBots = HordeState.WaveBots(wave);
            var creatures = HordeState.WaveCreatures(wave);

            var arenaBot = _bots.Count > 0 ? _bots[0] : null;
            for (int i = 0; i < nBots; i++)
            {
                int index = i + 1; // combatant indices 1..3
                Vector3 pos = SpawnPoint(i) + Vector3.up * 1.1f;
                if (i == 0 && arenaBot != null)
                {
                    arenaBot.gameObject.SetActive(true);
                    arenaBot.autoRevive = false;
                    arenaBot.ResetAt(pos); // fresh life at the wave spawn point
                }
                else
                {
                    _bots.Add(SpawnBot(pos, index, autoRevive: false));
                }
            }
            if (nBots == 0 && arenaBot != null) arenaBot.gameObject.SetActive(false);

            for (int i = 0; i < creatures.Count; i++)
            {
                var c = SpawnCreature(creatures[i], SpawnPoint(nBots + i) + Vector3.up * 0.6f);
                if (c != null) { _waveCreatures.Add(c); _creatureWasAlive.Add(true); }
                else _horde.RegisterDown(); // spawn failed — don't strand the wave counter
            }
            Debug.Log("ZIPTIDE: HORDE_WAVE wave=" + wave + " bots=" + nBots + " creatures=" + creatures.Count);
        }

        private void OnWaveClear()
        {
            _betweenWaves = true;
            _nextWaveAt = Time.time + hordeWaveDelay;
            Debug.Log("ZIPTIDE: HORDE_WAVE_CLEAR wave=" + _horde.Wave + " score=" + _horde.Score);
        }

        /// <summary>Runtime mirror of CityBuilder.MakeCreature (that one is editor-only): inactive-GO
        /// trick so creatureId is set before Awake loads the definition.</summary>
        private CreatureRuntime SpawnCreature(string creatureId, Vector3 pos)
        {
            var go = new GameObject("Horde_" + creatureId);
            go.SetActive(false);
            go.transform.position = pos;
            var rt = go.AddComponent<CreatureRuntime>();
            rt.creatureId = creatureId;
            rt.respawnDelay = 0f; // downed is downed — the wave counts it
            switch (creatureId)
            {
                case "warden": go.AddComponent<WardenBehavior>(); break;
                case "witness_mite": go.AddComponent<WitnessMiteBehavior>(); break;
                case "light_grazer": go.AddComponent<LightGrazerBehavior>(); break;
                case "tether_swarm": go.AddComponent<TetherSwarmBehavior>(); break;
                case "husk_molter": go.AddComponent<HuskMolterBehavior>(); break;
                default:
                    var def = Resources.Load<Ziptide.Content.CreatureDefinition>("Enemies/" + creatureId);
                    var archetype = def != null ? def.archetype : Ziptide.Content.CreatureArchetype.Swarmer;
                    switch (archetype)
                    {
                        case Ziptide.Content.CreatureArchetype.Bruiser: go.AddComponent<BruiserBehavior>(); break;
                        case Ziptide.Content.CreatureArchetype.WallCrawler: go.AddComponent<WallCrawlerBehavior>(); break;
                        case Ziptide.Content.CreatureArchetype.Flyer: go.AddComponent<FlyerBehavior>(); break;
                        default: go.AddComponent<SwarmerBehavior>(); break;
                    }
                    break;
            }
            go.SetActive(true);
            _spawnedEnemies.Add(go);
            return rt;
        }

        // ── Gun Game weapon rack ───────────────────────────────────────────────────────────────────
        /// <summary>The next ladder weapon appears at the player's feet-front — grab it and go. The old
        /// one stays where it fell (arena litter is honest).</summary>
        private void RackPlayerWeapon(int rung)
        {
            if (_gunGame == null || rung >= _gunGame.Ladder.Count) return;
            string itemId = _gunGame.Ladder[rung];
            Vector3 pos = PlayerPos(out var pp)
                ? pp + Vector3.ProjectOnPlane(_playerHead != null ? _playerHead.forward : Vector3.forward, Vector3.up).normalized * 0.8f
                : _playerBankPos + Vector3.forward;
            pos.y = Mathf.Max(0.9f, pos.y - 0.4f); // chest height — easy grab, not in the face
            var item = ItemFactory.Create(itemId, pos);
            if (item != null)
            {
                _spawnedEnemies.Add(item); // cleaned up with the round
                Debug.Log("ZIPTIDE: GUNGAME_RACK item=" + itemId);
            }
        }

        // ── Zones / shared helpers ─────────────────────────────────────────────────────────────────
        private void LoadZonesAndSpawns()
        {
            var group = GameObject.Find("__PVP_ZONES");
            if (group != null)
                foreach (Transform child in group.transform)
                    _zones.Add(new Zone { Id = child.name, Pos = child.position, Radius = Mathf.Max(1.5f, child.localScale.x) });

            var nav = GameObject.Find("__PVP_BOTNAV");
            if (nav != null)
                foreach (Transform child in nav.transform)
                    if (child.name.StartsWith("Way_")) _spawnPoints.Add(child.position);
        }

        private Vector3 SpawnPoint(int i) =>
            _spawnPoints.Count > 0 ? _spawnPoints[i % _spawnPoints.Count]
                                   : new Vector3(2f * i, 0f, 6f);

        private void BuildZoneRings()
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var z = _zones[i];
                if (z.Ring != null) continue;
                var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                disc.name = "ZoneRing_" + z.Id;
                disc.transform.position = new Vector3(z.Pos.x, 0.03f, z.Pos.z);
                disc.transform.localScale = new Vector3(z.Radius * 2f, 0.03f, z.Radius * 2f);
                var col = disc.GetComponent<Collider>();
                if (col != null) Destroy(col);
                ItemFactory.ApplyURPColor(disc, ZoneIdle);
                var r = disc.GetComponent<Renderer>();
                if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                z.Ring = r;
                _zones[i] = z;
            }
        }

        private void HighlightZones(int active)
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var r = _zones[i].Ring;
                if (r == null || r.material == null) continue;
                var c = i == active ? ZoneActive : ZoneIdle;
                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", c);
                else r.material.color = c;
            }
        }

        private Vector3 FragMidPos() => _zones.Count > 0 ? _zones[0].Pos : Vector3.zero;

        private void BuildFragment()
        {
            if (_fragmentGo != null) return;
            _fragmentGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _fragmentGo.name = "Fragment";
            _fragmentGo.transform.localScale = Vector3.one * 0.35f;
            var col = _fragmentGo.GetComponent<Collider>();
            if (col != null) Destroy(col);
            ItemFactory.ApplyURPColor(_fragmentGo, FragmentColor);
            PlaceFragmentAtMid();
        }

        private void PlaceFragmentAtMid()
        {
            if (_fragmentGo != null)
                _fragmentGo.transform.position = FragMidPos() + Vector3.up * 1.1f;
        }

        private bool PlayerPos(out Vector3 pos)
        {
            if (_playerHead != null) { pos = _playerHead.position; return true; }
            pos = Vector3.zero;
            return false;
        }

        /// <summary>Never Destroy() something in the player's hand — it corrupts the XR interactor
        /// (VR_RIG_GOTCHAS). A held racked weapon just becomes honest arena litter.</summary>
        private static bool HeldByPlayer(GameObject go)
        {
            var grab = go.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            return grab != null && grab.isSelected;
        }

        private static float FlatDist(Vector3 a, Vector3 b)
        { float dx = a.x - b.x, dz = a.z - b.z; return Mathf.Sqrt(dx * dx + dz * dz); }

        private static string PrettyItem(string id) => id.Replace('_', ' ').ToUpperInvariant();
    }
}
