using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Single entry-point for all scene travel. DontDestroyOnLoad singleton.
    ///
    /// Flow:
    ///   1. SaveBeforeTravel  — snapshot + destroy current scene items
    ///   2. LoadScene         — Unity async scene load
    ///   3. WaitForXRI        — spin up to 5s until XRInteractionManager, ray interactors,
    ///                          and InputActionManager are all ready
    ///   4. RestoreAfterTravel — coroutine from InventoryState (real socket SelectEnter)
    ///   5. Log TRAVEL_OK / TRAVEL_FAIL
    /// </summary>
    public class TravelCoordinator : MonoBehaviour
    {
        private static TravelCoordinator _instance;

        private bool _travelling;

        /// <summary>True while TravelCoroutine is running. PlayerRigPersistence skips wiring/restore in OnSceneLoaded when true.</summary>
        public static bool IsTravelling => _instance != null && _instance._travelling;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("ZIPTIDE: DUP_SINGLETON TravelCoordinator – destroying extra");
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ZIPTIDE: TravelCoordinator AWAKE");
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        // Where the tide should erupt FROM (a door/gate world position), set by the anchored
        // TravelTo overload and consumed by the next travel. Static because doors call the
        // static API; cleared on every travel start so a stale anchor can never leak forward.
        private static Vector3? _pendingGatePos;

        // Consumed by the next travel: suppress THE ZIPTIDE for this one hop. Set by the cold-boot
        // path — the namesake gate is a world↔world moment; playing it in the empty _Boot on a cold
        // start is wrong UX (there's nothing to leave) and it was the only risky step running before
        // the first world exists. World-to-world travel always keeps the full gate.
        private static bool _skipGateNext;

        /// <summary>Travel with a gate anchor — THE ZIPTIDE pours out of the doorway at
        /// <paramref name="gatePos"/> instead of only ringing the player.</summary>
        public static void TravelTo(string sceneName, Vector3 gatePos)
        {
            _pendingGatePos = gatePos;
            TravelTo(sceneName);
        }

        /// <summary>Travel, optionally suppressing THE ZIPTIDE for this hop (used by the cold boot).</summary>
        public static void TravelTo(string sceneName, bool skipGate)
        {
            _skipGateNext = skipGate;
            TravelTo(sceneName);
        }

        /// <summary>
        /// Primary travel API. Falls back to direct SceneManager.LoadScene if no coordinator.
        /// </summary>
        public static void TravelTo(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;

            // _Boot is the bootstrap scene and must NEVER be a travel destination. Loading it
            // re-runs bootstrap, spawns a second set of singletons, and drops the player into the
            // contentless boot scene (the "brown screen" bug). Redirect to the first world.
            if (sceneName == Ziptide.Core.ZiptideConstants.SceneBoot)
            {
                Debug.LogWarning("ZIPTIDE: TRAVEL_BLOCKED dest=_Boot (boot is not a world) – redirecting to "
                    + Ziptide.Core.ZiptideConstants.FirstWorldScene);
                sceneName = Ziptide.Core.ZiptideConstants.FirstWorldScene;
            }

            if (_instance == null)
            {
                _pendingGatePos = null;
                Debug.LogWarning("ZIPTIDE: TravelCoordinator not found – falling back to direct load");
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                if (rig != null) rig.PrepareForSceneTravel();
                SceneManager.LoadScene(sceneName);
                return;
            }

            _instance.StartTravelCoroutine(sceneName);
        }

        private void StartTravelCoroutine(string sceneName)
        {
            // Consume the one-shot inputs even when the request is ignored — never carry them forward.
            Vector3? gatePos = _pendingGatePos;
            bool skipGate = _skipGateNext;
            _pendingGatePos = null;
            _skipGateNext = false;
            if (_travelling)
            {
                Debug.LogWarning("ZIPTIDE: TravelCoordinator already travelling – ignoring duplicate request");
                return;
            }
            StartCoroutine(TravelCoroutine(sceneName, gatePos, skipGate));
        }

        private IEnumerator TravelCoroutine(string sceneName, Vector3? gatePos, bool skipGate)
        {
            _travelling = true;
            Debug.Log("ZIPTIDE: TRAVEL_START dest=" + sceneName);

            var rig = Object.FindObjectOfType<PlayerRigPersistence>();

            // 0. THE ZIPTIDE (the game's namesake moment): the departure tide gathers around the
            //    player and crests — the scene cut lands INSIDE the crest flash, so the hard load
            //    reads as the tide taking you. Every travel path (doors, ship, warps) comes
            //    through here, so every one of them gets the moment. The tide is TINTED by the
            //    destination's sky (manifest entry) — every gate is colored by where you're going.
            var destEntry = ManifestEntryFor(sceneName);
            string destName = destEntry != null && !string.IsNullOrEmpty(destEntry.displayName)
                ? destEntry.displayName : sceneName;
            Color destHorizon = destEntry != null ? destEntry.skyHorizon : default;
            Color destZenith = destEntry != null ? destEntry.skyZenith : default;
            // The gate call is wrapped so a visual/audio hiccup can NEVER kill the coroutine before the
            // scene loads — that would leave _travelling stuck true and silently block ALL future travel
            // (the boot-strand class of bug). The WaitForSeconds is OUTSIDE the try (C# forbids yield in
            // try/catch); lead stays 0 on failure/skip so we just proceed straight to the load.
            float lead = 0f;
            if (rig != null && !skipGate)
            {
                try
                {
                    lead = ZiptideGateEffect.PlayDeparture(rig.transform.position, destName,
                        destHorizon, destZenith, gatePos);
                    // RILL rides the tide — her line starts over the rise and carries across the cut
                    // (she lives on the persistent rig).
                    RillCompanion.OnGateDeparture(sceneName);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: GATE_FAIL depart dest=" + sceneName + " – " + ex.Message);
                    lead = 0f;
                }
            }
            if (lead > 0f) yield return new WaitForSeconds(lead);

            // 1. Save inventory before anything is destroyed by scene unload.
            if (rig != null)
                rig.PrepareForSceneTravel();
            else
                InventoryState.SaveBeforeTravel();

            // 2. Load destination scene.
            SceneManager.LoadScene(sceneName);

            // 3. Wait one frame for the new scene to initialise.
            yield return null;

            // 4. Single owner of post-load: teleport, wire XRI, then wait for ready, then restore.
            var playerRig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (playerRig == null)
            {
                Debug.LogWarning("ZIPTIDE: TRAVEL_FAIL dest=" + sceneName + " reason=no_PlayerRigPersistence_after_load");
                _travelling = false;
                yield break;
            }

            playerRig.TeleportToSpawnMarker();
            playerRig.EnsureXRIWiring();

            // The receding tide releases you into the new world, colored with ITS sky. Skipped on the
            // cold boot (no departure preceded it), and guarded so an arrival-FX hiccup can't strand travel.
            if (!skipGate)
            {
                try { ZiptideGateEffect.PlayArrival(playerRig.transform.position, destHorizon, destZenith); }
                catch (System.Exception ex) { Debug.LogWarning("ZIPTIDE: GATE_FAIL arrive dest=" + sceneName + " – " + ex.Message); }
            }

            // 5. Wait for XRI to be ready (up to 5 seconds).
            float elapsed = 0f;
            const float timeout = 5f;
            bool xriReady = false;

            while (elapsed < timeout)
            {
                if (IsXRIReady())
                {
                    xriReady = true;
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!xriReady)
            {
                Debug.LogWarning("ZIPTIDE: XRI_NOT_READY after " + timeout + "s – proceeding anyway");
                Debug.LogWarning("ZIPTIDE: TRAVEL_FAIL dest=" + sceneName + " reason=XRI_NOT_READY");
            }
            else
            {
                Debug.Log("ZIPTIDE: XRI_READY elapsed=" + elapsed.ToString("F2") + "s");
            }

            // 6. Restore inventory (coroutine with frame delays for socket registration).
            yield return InventoryState.RestoreAfterTravel(playerRig.transform);

            // Runtime audit: exactly one manager after travel.
            var managers = Object.FindObjectsOfType<XRInteractionManager>();
            if (managers != null && managers.Length != 1)
                Debug.LogWarning("ZIPTIDE: AUDIT_FAIL multiple_managers_after_travel count=" + (managers?.Length ?? 0));

            if (xriReady)
                Debug.Log("ZIPTIDE: TRAVEL_OK dest=" + sceneName);

            _travelling = false;
        }

        /// <summary>The manifest entry for a scene — the tide's display name + sky tint. Null if
        /// the manifest hasn't been generated or the scene isn't in it.</summary>
        private static DevTools.DevWorldManifest.Entry ManifestEntryFor(string sceneName)
        {
            var manifest = DevTools.DevWorldManifest.Load();
            if (manifest != null && manifest.worlds != null)
                foreach (var w in manifest.worlds)
                    if (w != null && w.sceneName == sceneName)
                        return w;
            return null;
        }

        // ── XRI readiness criteria ──────────────────────────────────────────

        private static bool IsXRIReady()
        {
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr == null) return false;

            var rays = Object.FindObjectsOfType<XRRayInteractor>();
            int enabledRays = 0;
            foreach (var ray in rays)
                if (ray.isActiveAndEnabled) enabledRays++;
            if (enabledRays < 2) return false;

            var iam = Object.FindObjectOfType<InputActionManager>();
            if (iam == null) return false;

            return true;
        }
    }
}
