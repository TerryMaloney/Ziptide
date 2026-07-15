using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// TIDEFRONT B3 — the mission ITSELF (Terry's "gulag"): when the war table sends you into a
    /// contested world with a pending battle, this spawns the 2–3 minute contract there. No scene
    /// edits — a boot hook watches scene loads and only builds when ConquestSession says this scene
    /// holds an accepted mission. FIVE contract verbs (the richness bar — the spec's full catalog,
    /// each a different thing your BODY does):
    ///   ATTACK  · Sabotage — shield pylons (plinth/column/crown/orbiters): shoot, tase, or slap.
    ///           · Scan — grid nodes with spinning dishes: STAND in the ring until the sweep fills.
    ///           · Beacon — grab the humming tripod beacon and CARRY it to the uplink pad.
    ///   DEFENSE · DroneDefense — rotor scouts overhead (each one a body/rotor/eye rig): down them.
    ///           · Repair — arcing conduit junctions: slap each one back into its socket, 3 hits.
    /// Win/lose/timeout posts to the MissionAttempt and auto-travels you back to the table, where
    /// the held battle resolves with the tilt. Walking out through a travel door instead = decline.
    /// </summary>
    public class ConquestMissionRuntime : MonoBehaviour
    {
        private MissionAttempt _attempt;
        private TextMesh _board;
        private Transform _boardRoot;
        private readonly HashSet<DroneRuntime> _missionDrones = new HashSet<DroneRuntime>();
        private bool _ending;

        // Beacon-run state (the carry verb).
        private Transform _beacon, _uplinkPad, _padRing;

        // ── Boot hook: missions appear in ANY world without touching its scene ──
        private static bool _hooked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Hook()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.ConquestMissionInjector)) return;
            if (_hooked) return;
            _hooked = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.ConquestMissionInjector)) return;
            if (!ConquestSession.MissionActiveFor(scene.name)) return;
            if (FindObjectOfType<ConquestMissionRuntime>() != null) return;
            var go = new GameObject("__ConquestMission");
            SceneManager.MoveGameObjectToScene(go, scene);   // dies with its world, never leaks
            go.AddComponent<ConquestMissionRuntime>();
        }

        // ── Build ────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.ConquestMissionInjector))
                enabled = false;
        }

        private void Start()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.ConquestMissionInjector))
            {
                enabled = false;
                return;
            }

            _attempt = ConquestSession.Pending != null ? ConquestSession.Pending.attempt : null;
            if (_attempt == null || _attempt.phase != MissionPhase.Accepted) { Destroy(gameObject); return; }

            Vector3 anchor = FindAnchor();
            transform.position = anchor;

            _boardRoot = new GameObject("MissionBoard").transform;
            _boardRoot.SetParent(transform, false);
            _boardRoot.position = anchor + new Vector3(0f, 2.6f, 2.5f);
            _board = _boardRoot.gameObject.AddComponent<TextMesh>();
            _board.characterSize = 0.02f; _board.fontSize = 64;   // the characterSize×fontSize lesson
            _board.anchor = TextAnchor.MiddleCenter;
            _board.alignment = TextAlignment.Center;
            _board.color = new Color(0.85f, 0.95f, 1f);

            switch (_attempt.mission.kind)
            {
                case MissionKind.Sabotage: BuildPylons(anchor); break;
                case MissionKind.Scan: BuildScanGrid(anchor); break;
                case MissionKind.Beacon: BuildBeaconRun(anchor); break;
                case MissionKind.Repair: BuildConduits(anchor); break;
                default: BuildDroneWave(anchor); break;
            }

            DroneRuntime.OnDroneDisabled += OnDroneDown;
            Debug.Log("ZIPTIDE: CONQ_MISSION_SPAWNED kind=" + _attempt.mission.kind +
                      " objectives=" + _attempt.mission.objectiveCount +
                      " limit=" + _attempt.mission.timeLimitSeconds);
        }

        private bool _flightHooked;

        private void OnDestroy()
        {
            DroneRuntime.OnDroneDisabled -= OnDroneDown;
            if (_flightHooked) Ziptide.Core.FlightSignals.TargetDisabled -= OnFlightTargetDisabled;
        }

        private void OnFlightTargetDisabled(string targetName) => ObjectiveDown();

        private Vector3 FindAnchor()
        {
            var marker = GameObject.Find(ZiptideConstants.GoSpawnPlayer);
            if (marker != null) return marker.transform.position;
            var cam = Camera.main;
            return cam != null ? cam.transform.position : Vector3.zero;
        }

        private Vector3 RingPoint(Vector3 anchor, int i, int n, float radius)
        {
            float ang = (i + 0.5f) / n * Mathf.PI * 2f;
            Vector3 pos = anchor + new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * radius;
            if (Physics.Raycast(pos + Vector3.up * 30f, Vector3.down, out var hit, 80f))
                pos.y = hit.point.y;
            return pos;
        }

        // ── Sabotage: pylons that look like INFRASTRUCTURE, not boxes ────────
        private void BuildPylons(Vector3 anchor)
        {
            int n = _attempt.mission.objectiveCount;
            for (int i = 0; i < n; i++)
            {
                var root = new GameObject("ShieldPylon_" + i);
                root.transform.SetParent(transform, true);
                root.transform.position = RingPoint(anchor, i, n, 12f);

                Part(root, PrimitiveType.Cylinder, new Vector3(0f, 0.12f, 0f),
                     new Vector3(1.5f, 0.12f, 1.5f), new Color(0.25f, 0.24f, 0.28f));      // plinth
                Part(root, PrimitiveType.Cylinder, new Vector3(0f, 1.1f, 0f),
                     new Vector3(0.55f, 1.0f, 0.55f), new Color(0.5f, 0.32f, 0.22f));      // column
                Part(root, PrimitiveType.Cylinder, new Vector3(0f, 2.05f, 0f),
                     new Vector3(0.3f, 0.35f, 0.3f), new Color(0.4f, 0.28f, 0.22f));       // taper neck
                var crown = Part(root, PrimitiveType.Sphere, new Vector3(0f, 2.6f, 0f),
                     Vector3.one * 0.62f, new Color(1f, 0.45f, 0.2f));                     // hot crown
                // Two counter-orbiting field emitters — a pylon that's ALIVE until you kill it.
                Orbiter(root.transform, 0.85f, 2.6f, 70f, new Color(1f, 0.6f, 0.25f));
                Orbiter(root.transform, 1.05f, 2.35f, -45f, new Color(0.95f, 0.4f, 0.2f));

                // Hittable via the crown: collider (from primitive) BEFORE interactables — gotcha #6.
                var pylon = crown.AddComponent<ConquestPylonRuntime>();
                pylon.Init(this, root);
            }
        }

        // ── Scan: hold your ground inside the ring while the dish sweeps ─────
        private void BuildScanGrid(Vector3 anchor)
        {
            int n = _attempt.mission.objectiveCount;
            for (int i = 0; i < n; i++)
            {
                var root = new GameObject("ScanNode_" + i);
                root.transform.SetParent(transform, true);
                root.transform.position = RingPoint(anchor, i, n, 14f);

                Part(root, PrimitiveType.Cylinder, new Vector3(0f, 0.1f, 0f),
                     new Vector3(1.1f, 0.1f, 1.1f), new Color(0.2f, 0.26f, 0.3f));         // base
                Part(root, PrimitiveType.Cylinder, new Vector3(0f, 1.0f, 0f),
                     new Vector3(0.12f, 0.9f, 0.12f), new Color(0.45f, 0.5f, 0.55f));      // mast
                var dish = Part(root, PrimitiveType.Sphere, new Vector3(0f, 1.95f, 0f),
                     new Vector3(0.7f, 0.18f, 0.7f), new Color(0.35f, 0.8f, 0.9f));        // dish
                dish.AddComponent<ConquestSpinRuntime>().Init(Vector3.up, 120f);
                var ring = Part(root, PrimitiveType.Cylinder, new Vector3(0f, 0.06f, 0f),
                     new Vector3(3.2f, 0.02f, 3.2f), new Color(0.3f, 0.85f, 0.95f, 0.5f)); // hold-zone ring

                root.AddComponent<ConquestScanNodeRuntime>().Init(this, ring.transform);
            }
        }

        // ── Beacon: the carry verb — heavy cargo, a destination, a hum ───────
        private void BuildBeaconRun(Vector3 anchor)
        {
            // The beacon spawns far out on the ring; the uplink pad sits near the spawn.
            var beaconRoot = new GameObject("StrikeBeacon");
            beaconRoot.transform.SetParent(transform, true);
            beaconRoot.transform.position = RingPoint(anchor, 0, 1, 22f) + Vector3.up * 0.4f;
            for (int leg = 0; leg < 3; leg++)
            {
                float a = leg / 3f * Mathf.PI * 2f;
                var l = Part(beaconRoot, PrimitiveType.Cylinder,
                    new Vector3(Mathf.Cos(a) * 0.16f, -0.18f, Mathf.Sin(a) * 0.16f),
                    new Vector3(0.05f, 0.22f, 0.05f), new Color(0.3f, 0.3f, 0.34f));
                l.transform.localRotation = Quaternion.Euler(Mathf.Sin(a) * 25f, 0f, Mathf.Cos(a) * -25f);
            }
            Part(beaconRoot, PrimitiveType.Cylinder, new Vector3(0f, 0.05f, 0f),
                 new Vector3(0.22f, 0.1f, 0.22f), new Color(0.5f, 0.5f, 0.55f));            // housing
            var bulb = Part(beaconRoot, PrimitiveType.Sphere, new Vector3(0f, 0.3f, 0f),
                 Vector3.one * 0.24f, new Color(0.4f, 0.95f, 0.6f));                        // green heart
            bulb.AddComponent<ConquestSpinRuntime>().Init(Vector3.up, 200f);
            // The whole tripod is the grabbable (body collider first, then grab).
            var bodyCol = beaconRoot.AddComponent<BoxCollider>();
            bodyCol.size = new Vector3(0.5f, 0.8f, 0.5f);
            var rb = beaconRoot.AddComponent<Rigidbody>();
            rb.mass = 4f;   // heavy — it should FEEL like contraband
            beaconRoot.AddComponent<XRGrabInteractable>();
            _beacon = beaconRoot.transform;

            var pad = new GameObject("UplinkPad");
            pad.transform.SetParent(transform, true);
            pad.transform.position = RingPoint(anchor, 0, 1, 5f);
            Part(pad, PrimitiveType.Cylinder, new Vector3(0f, 0.05f, 0f),
                 new Vector3(2.2f, 0.05f, 2.2f), new Color(0.2f, 0.5f, 0.35f));
            var pulse = Part(pad, PrimitiveType.Cylinder, new Vector3(0f, 0.09f, 0f),
                 new Vector3(1.6f, 0.02f, 1.6f), new Color(0.4f, 0.95f, 0.6f));
            _padRing = pulse.transform;
            _uplinkPad = pad.transform;
        }

        // ── Repair: slap the arcing junctions home (3 hits each) ─────────────
        private void BuildConduits(Vector3 anchor)
        {
            int n = _attempt.mission.objectiveCount;
            for (int i = 0; i < n; i++)
            {
                var root = new GameObject("Conduit_" + i);
                root.transform.SetParent(transform, true);
                root.transform.position = RingPoint(anchor, i, n, 10f);

                Part(root, PrimitiveType.Cube, new Vector3(0f, 0.7f, 0f),
                     new Vector3(0.7f, 1.4f, 0.45f), new Color(0.26f, 0.3f, 0.34f));       // cabinet
                var pipeL = Part(root, PrimitiveType.Cylinder, new Vector3(-0.45f, 1.15f, 0f),
                     new Vector3(0.1f, 0.45f, 0.1f), new Color(0.5f, 0.55f, 0.6f));
                pipeL.transform.localRotation = Quaternion.Euler(0f, 0f, 55f);              // kinked feed
                var pipeR = Part(root, PrimitiveType.Cylinder, new Vector3(0.45f, 1.15f, 0f),
                     new Vector3(0.1f, 0.45f, 0.1f), new Color(0.5f, 0.55f, 0.6f));
                pipeR.transform.localRotation = Quaternion.Euler(0f, 0f, -55f);
                var gap = Part(root, PrimitiveType.Cube, new Vector3(0f, 1.42f, 0f),
                     new Vector3(0.3f, 0.14f, 0.14f), new Color(1f, 0.9f, 0.4f));           // the arc gap

                var junction = gap.AddComponent<ConquestConduitRuntime>();
                junction.Init(this);
            }
        }

        // ── Defense: scouts that read as MACHINES — body, rotor, eye ─────────
        private void BuildDroneWave(Vector3 anchor)
        {
            int n = _attempt.mission.objectiveCount;
            for (int i = 0; i < n; i++)
            {
                float ang = (i + 0.5f) / n * Mathf.PI * 2f;
                float dist = 8f + (i % 3) * 3f;
                Vector3 pos = anchor + new Vector3(Mathf.Cos(ang) * dist, 3.2f + (i % 2) * 1.4f,
                                                   Mathf.Sin(ang) * dist);
                var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                body.name = "RivalScout_" + i;
                body.transform.SetParent(transform, true);
                body.transform.position = pos;
                body.transform.localScale = Vector3.one * (0.36f + (i % 3) * 0.06f);   // size variance

                var rotor = Part(body, PrimitiveType.Cylinder, new Vector3(0f, 0.75f, 0f),
                     new Vector3(1.7f, 0.03f, 1.7f), new Color(0.3f, 0.3f, 0.32f));
                rotor.AddComponent<ConquestSpinRuntime>().Init(Vector3.up, 700f + i * 60f);
                if (rotor.TryGetComponent<Collider>(out var rc)) Destroy(rc);   // rotor is visual only
                Part(body, PrimitiveType.Cube, new Vector3(0f, 0.05f, 0.5f),
                     new Vector3(0.25f, 0.18f, 0.2f), new Color(0.95f, 0.2f, 0.15f));   // the eye

                body.AddComponent<TargetRuntime>();   // BEFORE DroneRuntime — its Awake hooks OnHit→Kill (pistol path)
                _missionDrones.Add(body.AddComponent<DroneRuntime>());
            }
        }

        /// <summary>A colored primitive child part (the multi-part rig helper).</summary>
        private static GameObject Part(GameObject parent, PrimitiveType type, Vector3 localPos,
                                       Vector3 localScale, Color color)
        {
            var p = GameObject.CreatePrimitive(type);
            p.name = parent.name + "_" + type;
            p.transform.SetParent(parent.transform, false);
            p.transform.localPosition = localPos;
            p.transform.localScale = localScale;
            ItemFactory.ApplyURPColor(p, color);
            return p;
        }

        private static void Orbiter(Transform parent, float radius, float height, float degPerSec, Color c)
        {
            var pivot = new GameObject("OrbiterPivot").transform;
            pivot.SetParent(parent, false);
            pivot.localPosition = new Vector3(0f, height, 0f);
            pivot.gameObject.AddComponent<ConquestSpinRuntime>().Init(Vector3.up, degPerSec);
            var chip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chip.name = "Orbiter";
            if (chip.TryGetComponent<Collider>(out var cc)) Object.Destroy(cc);
            chip.transform.SetParent(pivot, false);
            chip.transform.localPosition = new Vector3(radius, 0f, 0f);
            chip.transform.localScale = new Vector3(0.14f, 0.05f, 0.05f);
            ItemFactory.ApplyURPColor(chip, c);
        }

        // ── The clock & the verdict ──────────────────────────────────────────
        private void Update()
        {
            if (_attempt == null || _ending) return;
            _attempt.Tick(Time.deltaTime);

            var cam = Camera.main;
            if (cam != null && _boardRoot != null)
                _boardRoot.rotation = Quaternion.LookRotation(_boardRoot.position - cam.transform.position);

            // The carry verb: beacon on the pad = mission objective complete.
            if (_beacon != null && _uplinkPad != null)
            {
                if (_padRing != null)   // the pad breathes so you can find it from across the map
                    _padRing.localScale = new Vector3(1.6f + Mathf.Sin(Time.time * 2.5f) * 0.25f, 0.02f,
                                                      1.6f + Mathf.Sin(Time.time * 2.5f) * 0.25f);
                Vector3 a = _beacon.position, b = _uplinkPad.position;
                a.y = 0f; b.y = 0f;
                if (Vector3.Distance(a, b) < 2.2f)
                {
                    _beacon = null;   // planted — stop checking
                    ObjectiveDown();
                }
            }

            if (_attempt.IsTerminal) { StartCoroutine(EndSequence()); return; }

            int secs = Mathf.CeilToInt(_attempt.SecondsRemaining);
            _board.text = _attempt.mission.title + "\n" +
                          _attempt.objectivesDone + " / " + _attempt.mission.objectiveCount +
                          "\n" + (secs / 60) + ":" + (secs % 60).ToString("00");
            _board.color = secs <= 30 ? new Color(1f, 0.4f, 0.3f) : new Color(0.85f, 0.95f, 1f);
        }

        /// <summary>Objective sources (pylons, scan nodes, conduits, the beacon pad) call this.</summary>
        public void ObjectiveDown()
        {
            if (_attempt == null) return;
            _attempt.CompleteObjective();
            Debug.Log("ZIPTIDE: CONQ_MISSION_OBJECTIVE done=" + _attempt.objectivesDone +
                      "/" + _attempt.mission.objectiveCount);
        }

        private void OnDroneDown(DroneRuntime drone)
        {
            if (_missionDrones.Remove(drone)) ObjectiveDown();
        }

        private IEnumerator EndSequence()
        {
            _ending = true;
            bool won = _attempt.phase == MissionPhase.Won;
            _board.text = won ? "MISSION COMPLETE\nreturning to the table…"
                              : "MISSION FAILED\nreturning to the table…";
            _board.color = won ? new Color(1f, 0.85f, 0.3f) : new Color(1f, 0.35f, 0.3f);
            Debug.Log("ZIPTIDE: CONQ_MISSION_" + (won ? "WIN" : "LOSE") +
                      " tilt=" + _attempt.ResultTilt());

            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            profile?.SetFlag("CONQUEST_MISSION_FLOWN");

            yield return new WaitForSeconds(2.5f);
            string back = string.IsNullOrEmpty(ConquestSession.ReturnScene)
                ? ZiptideConstants.SceneSandbox : ConquestSession.ReturnScene;
            TravelCoordinator.TravelTo(back, transform.position);
        }
    }

    /// <summary>Generic slow/fast rotator for mission rigs (dishes, rotors, orbiter pivots).
    /// Public sibling, never a nested MonoBehaviour (the ShipRefitBaseXf lesson).</summary>
    public class ConquestSpinRuntime : MonoBehaviour
    {
        private Vector3 _axis = Vector3.up;
        private float _degPerSec = 90f;

        public void Init(Vector3 axis, float degPerSec) { _axis = axis; _degPerSec = degPerSec; }

        private void Update() => transform.Rotate(_axis, _degPerSec * Time.deltaTime, Space.Self);
    }

    /// <summary>One sabotage pylon: shoot the crown (TargetRuntime), tase it (IShockable), or slap
    /// it (XRSimpleInteractable) — any of the three downs it. On death the crown goes dark, the
    /// orbiters stop, the whole pylon slumps.</summary>
    public class ConquestPylonRuntime : MonoBehaviour, IShockable
    {
        private ConquestMissionRuntime _mission;
        private GameObject _rigRoot;
        private bool _down;

        public void Init(ConquestMissionRuntime mission, GameObject rigRoot)
        {
            _mission = mission;
            _rigRoot = rigRoot;
            // Collider exists from the primitive BEFORE the interactable — gotcha #6.
            gameObject.AddComponent<TargetRuntime>().OnHit.AddListener(GoDown);
            gameObject.AddComponent<XRSimpleInteractable>().selectEntered
                      .AddListener(_ => GoDown());
        }

        public void Shock(float seconds) => GoDown();

        private void GoDown()
        {
            if (_down) return;
            _down = true;
            ItemFactory.ApplyURPColor(gameObject, new Color(0.18f, 0.18f, 0.2f));
            if (_rigRoot != null)
            {
                foreach (var spin in _rigRoot.GetComponentsInChildren<ConquestSpinRuntime>())
                    spin.enabled = false;                                    // the emitters die
                _rigRoot.transform.rotation *= Quaternion.Euler(0f, 0f, 7f); // it slumps
                _rigRoot.transform.position += Vector3.down * 0.25f;
            }
            Debug.Log("ZIPTIDE: CONQ_PYLON_DOWN name=" + gameObject.name);
            if (_mission != null) _mission.ObjectiveDown();
        }
    }

    /// <summary>Scan node: stand inside the floor ring until the sweep completes (~3s). Drift out
    /// and the lock decays — the HOLD-GROUND verb, no weapon involved.</summary>
    public class ConquestScanNodeRuntime : MonoBehaviour
    {
        private const float HoldRadius = 3.4f;
        private const float FillSeconds = 3f;

        private ConquestMissionRuntime _mission;
        private Transform _ring;
        private float _fill;
        private bool _done;

        public void Init(ConquestMissionRuntime mission, Transform ring)
        { _mission = mission; _ring = ring; }

        private void Update()
        {
            if (_done) return;
            var cam = Camera.main;
            if (cam == null) return;
            Vector3 a = cam.transform.position, b = transform.position;
            a.y = 0f; b.y = 0f;
            bool inside = Vector3.Distance(a, b) <= HoldRadius;
            _fill = Mathf.Clamp01(_fill + (inside ? Time.deltaTime / FillSeconds
                                                  : -Time.deltaTime / (FillSeconds * 2f)));
            if (_ring != null)
            {
                float s = Mathf.Lerp(3.2f, 0.6f, _fill);   // the ring CLOSES on you as the scan locks
                _ring.localScale = new Vector3(s, 0.02f, s);
            }
            if (_fill >= 1f)
            {
                _done = true;
                if (_ring != null) ItemFactory.ApplyURPColor(_ring.gameObject, new Color(0.4f, 0.95f, 0.6f));
                Debug.Log("ZIPTIDE: CONQ_SCAN_LOCK name=" + gameObject.name);
                if (_mission != null) _mission.ObjectiveDown();
            }
        }
    }

    /// <summary>Repair junction: three good slaps (or shots/tase) reseat the arc gap. Flickers
    /// angrily until fixed, then holds a steady green.</summary>
    public class ConquestConduitRuntime : MonoBehaviour, IShockable
    {
        private const int HitsToFix = 3;

        private ConquestMissionRuntime _mission;
        private int _hits;
        private bool _fixed;
        private Renderer _renderer;

        public void Init(ConquestMissionRuntime mission)
        {
            _mission = mission;
            _renderer = GetComponent<Renderer>();
            gameObject.AddComponent<TargetRuntime>().OnHit.AddListener(Hit);
            gameObject.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => Hit());
        }

        public void Shock(float seconds) => Hit();

        private void Update()
        {
            if (_fixed || _renderer == null) return;
            // Angry arc flicker — density falls as it gets reseated.
            float rate = 8f - _hits * 2f;
            float f = Mathf.PerlinNoise(Time.time * rate, GetInstanceID() % 97);
            var c = Color.Lerp(new Color(0.5f, 0.4f, 0.15f), new Color(1f, 0.95f, 0.5f), f);
            var mat = _renderer.material;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            else mat.color = c;
        }

        private void Hit()
        {
            if (_fixed) return;
            _hits++;
            transform.localPosition += Vector3.down * 0.02f;   // each slap seats it deeper
            Debug.Log("ZIPTIDE: CONQ_CONDUIT_HIT hits=" + _hits + "/" + HitsToFix);
            if (_hits < HitsToFix) return;
            _fixed = true;
            ItemFactory.ApplyURPColor(gameObject, new Color(0.4f, 0.95f, 0.6f));
            if (_mission != null) _mission.ObjectiveDown();
        }
    }
}
