using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
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
        public const string FirstHourOutboundSignal = "TRAVEL_W000_TO_W001_COMPLETE";
        public const string FirstHourReturnSignal = "TRAVEL_W001_TO_W000_COMPLETE";

        private static TravelCoordinator _instance;

        private bool _travelling;

        /// <summary>True while TravelCoroutine is running. PlayerRigPersistence skips wiring/restore in OnSceneLoaded when true.</summary>
        public static bool IsTravelling => _instance != null && _instance._travelling;

        /// <summary>
        /// Neutral successful-travel notification. Payload is the destination scene name. Emitted
        /// only after XRI readiness, inventory restoration and the existing TRAVEL_OK point.
        /// </summary>
        public static event Action<string> TravelCompleted;

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

            // PRE-FLIGHT (crash-proofing 2026-07-10): LoadScene on a scene missing from Build
            // Settings does NOT throw — it silently loads nothing, stranding the player mid-tide
            // AFTER the autosave and rig state changes already fired. Abort here, before any side
            // effect, and the player just stays where they are with a clear log to act on.
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError("ZIPTIDE: TRAVEL_FAIL dest=" + sceneName +
                               " reason=scene_not_in_build — aborting before any state changes " +
                               "(bake the scene / add it to Build Settings)");
                _pendingGatePos = null;
                _skipGateNext = false;
                return;
            }

            if (_instance == null)
            {
                _pendingGatePos = null;
                Debug.LogWarning("ZIPTIDE: TravelCoordinator not found – falling back to direct load");
                SaveSystem.AutosaveNow("travel_fallback");
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
            bool travelCompletionEmitted = false;
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

            // 1. Save the profile + inventory before anything is destroyed by scene unload.
            //    AutosaveNow is guarded internally — a save hiccup can never strand travel.
            SaveSystem.AutosaveNow("travel");
            if (rig != null)
                rig.PrepareForSceneTravel();
            else
                InventoryState.SaveBeforeTravel();

            // 2. Load asynchronously while the existing crest covers vision. Activation stays held
            //    until Unity has prepared the scene; a 20-second timeout releases the hold so travel
            //    can never remain wedged behind a load that does not report the normal 0.9 threshold.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (loadOperation == null)
            {
                Debug.LogWarning("ZIPTIDE: TRAVEL_FAIL dest=" + sceneName + " reason=async_load_not_started");
                _travelling = false;
                yield break;
            }

            loadOperation.allowSceneActivation = false;
            float loadElapsed = 0f;
            const float loadTimeout = 20f;
            while (loadOperation.progress < 0.9f && loadElapsed < loadTimeout)
            {
                loadElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (loadOperation.progress < 0.9f)
                Debug.LogWarning("ZIPTIDE: TRAVEL_TIMEOUT dest=" + sceneName +
                                 " elapsed=" + loadElapsed.ToString("F1") +
                                 " progress=" + loadOperation.progress.ToString("F2"));

            loadOperation.allowSceneActivation = true;
            while (!loadOperation.isDone) yield return null;

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
            {
                Debug.Log("ZIPTIDE: TRAVEL_OK dest=" + sceneName);
                if (TryPublishTravelCompleted(
                    sceneName,
                    successful: true,
                    ref travelCompletionEmitted,
                    PublishTravelCompleted))
                {
                    Debug.Log("ZIPTIDE: FIRST_HOUR_TRAVEL dest=" + sceneName +
                              " mapped=" + FirstHourTravelMapLabel(sceneName));
                }
            }

            _travelling = false;
        }

        /// <summary>
        /// Pure one-shot publication seam. Failed, duplicate and empty destinations are no-ops.
        /// </summary>
        public static bool TryPublishTravelCompleted(
            string destination,
            bool successful,
            ref bool alreadyEmitted,
            Action<string> publish)
        {
            if (!successful || alreadyEmitted || string.IsNullOrEmpty(destination)) return false;
            alreadyEmitted = true;
            publish?.Invoke(destination);
            return true;
        }

        /// <summary>Maps canonical first-hour destinations to their approved completion signal.</summary>
        public static string MapFirstHourTravelSignal(string destination)
        {
            if (string.Equals(destination, Ziptide.Core.ZiptideConstants.SceneToxicCity, StringComparison.Ordinal))
                return FirstHourOutboundSignal;
            if (string.Equals(destination, Ziptide.Core.ZiptideConstants.SceneW000, StringComparison.Ordinal))
                return FirstHourReturnSignal;
            return null;
        }

        private static string FirstHourTravelMapLabel(string destination)
        {
            string mapped = MapFirstHourTravelSignal(destination);
            if (mapped == FirstHourOutboundSignal) return "first";
            if (mapped == FirstHourReturnSignal) return "return";
            return "none";
        }

        private static void PublishTravelCompleted(string destination)
        {
            TravelCompleted?.Invoke(destination);
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
