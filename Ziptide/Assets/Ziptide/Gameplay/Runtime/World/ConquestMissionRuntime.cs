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
    /// holds an accepted mission.
    ///   ATTACK/Sabotage — three humming shield pylons near the spawn: shoot, tase, or slap each
    ///   one down before the clock runs out.
    ///   DEFENSE/DroneDefense — rival scout drones fan out overhead: down every one of them.
    /// Win/lose/timeout posts to the MissionAttempt and auto-travels you back to the table, where
    /// the held battle resolves with the tilt. Walking out through a travel door instead = decline
    /// (the table treats a non-terminal attempt as a pass — odds unchanged).
    /// </summary>
    public class ConquestMissionRuntime : MonoBehaviour
    {
        private MissionAttempt _attempt;
        private TextMesh _board;
        private Transform _boardRoot;
        private readonly HashSet<DroneRuntime> _missionDrones = new HashSet<DroneRuntime>();
        private bool _ending;

        // ── Boot hook: missions appear in ANY world without touching its scene ──
        private static bool _hooked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Hook()
        {
            if (_hooked) return;
            _hooked = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!ConquestSession.MissionActiveFor(scene.name)) return;
            if (FindObjectOfType<ConquestMissionRuntime>() != null) return;
            var go = new GameObject("__ConquestMission");
            SceneManager.MoveGameObjectToScene(go, scene);   // dies with its world, never leaks
            go.AddComponent<ConquestMissionRuntime>();
        }

        // ── Build ────────────────────────────────────────────────────────────
        private void Start()
        {
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

            if (_attempt.mission.kind == MissionKind.Sabotage) BuildPylons(anchor);
            else BuildDroneWave(anchor);

            DroneRuntime.OnDroneDisabled += OnDroneDown;
            Debug.Log("ZIPTIDE: CONQ_MISSION_SPAWNED kind=" + _attempt.mission.kind +
                      " objectives=" + _attempt.mission.objectiveCount +
                      " limit=" + _attempt.mission.timeLimitSeconds);
        }

        private void OnDestroy() => DroneRuntime.OnDroneDisabled -= OnDroneDown;

        private Vector3 FindAnchor()
        {
            var marker = GameObject.Find(ZiptideConstants.GoSpawnPlayer);
            if (marker != null) return marker.transform.position;
            var cam = Camera.main;
            return cam != null ? cam.transform.position : Vector3.zero;
        }

        /// <summary>Sabotage: pylons on a ring around the spawn, snapped to the ground.</summary>
        private void BuildPylons(Vector3 anchor)
        {
            int n = _attempt.mission.objectiveCount;
            for (int i = 0; i < n; i++)
            {
                float ang = (i + 0.5f) / n * Mathf.PI * 2f;
                Vector3 pos = anchor + new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * 12f;
                if (Physics.Raycast(pos + Vector3.up * 30f, Vector3.down, out var hit, 80f))
                    pos.y = hit.point.y;

                var pylon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pylon.name = "ShieldPylon_" + i;
                pylon.transform.SetParent(transform, true);
                pylon.transform.position = pos + Vector3.up * 1.2f;
                pylon.transform.localScale = new Vector3(0.5f, 1.2f, 0.5f);
                ItemFactory.ApplyURPColor(pylon, new Color(0.95f, 0.4f, 0.25f));
                pylon.AddComponent<ConquestPylonRuntime>().Init(this);
            }
        }

        /// <summary>Defense: scout drones fan out overhead — every one down = mission won.</summary>
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
                body.transform.localScale = Vector3.one * 0.4f;
                body.AddComponent<TargetRuntime>();   // BEFORE DroneRuntime — its Awake hooks OnHit→Kill (pistol path)
                _missionDrones.Add(body.AddComponent<DroneRuntime>());
            }
        }

        // ── The clock & the verdict ──────────────────────────────────────────
        private void Update()
        {
            if (_attempt == null || _ending) return;
            _attempt.Tick(Time.deltaTime);

            var cam = Camera.main;
            if (cam != null && _boardRoot != null)
                _boardRoot.rotation = Quaternion.LookRotation(_boardRoot.position - cam.transform.position);

            if (_attempt.IsTerminal) { StartCoroutine(EndSequence()); return; }

            int secs = Mathf.CeilToInt(_attempt.SecondsRemaining);
            _board.text = _attempt.mission.title + "\n" +
                          _attempt.objectivesDone + " / " + _attempt.mission.objectiveCount +
                          "\n" + (secs / 60) + ":" + (secs % 60).ToString("00");
            _board.color = secs <= 30 ? new Color(1f, 0.4f, 0.3f) : new Color(0.85f, 0.95f, 1f);
        }

        /// <summary>Pylons call this when they go down.</summary>
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

    /// <summary>One sabotage pylon: shoot it (TargetRuntime), tase it (IShockable), or slap it
    /// (XRSimpleInteractable) — any of the three downs it. Public sibling, never a nested
    /// MonoBehaviour (the ShipRefitBaseXf lesson).</summary>
    public class ConquestPylonRuntime : MonoBehaviour, IShockable
    {
        private ConquestMissionRuntime _mission;
        private bool _down;

        public void Init(ConquestMissionRuntime mission)
        {
            _mission = mission;
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
            transform.position += Vector3.down * 0.5f;   // sinks dead into its socket
            Debug.Log("ZIPTIDE: CONQ_PYLON_DOWN name=" + gameObject.name);
            if (_mission != null) _mission.ObjectiveDown();
        }
    }
}
