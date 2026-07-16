using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// DS-01 (DEVICE_STABILIZATION_FORENSIC_PLAN) — the cold-boot hold contract, pure state seam.
    /// While the Home Hub owns `_Boot`, the rig is HELD: locomotion is suspended, the fall net is
    /// disarmed, and the rig is pinned to its boot pose. The hold releases only after a content
    /// scene's spawn has settled, which re-arms the fall net from the fresh spawn. This is the fix
    /// for the boot fall/respawn loop: the new wait-in-boot menu ran inside the old assumption that
    /// `_Boot` is transient, leaving gravity + fall recovery live in a scene with no floor.
    /// </summary>
    public sealed class BootHoldState
    {
        public bool Held { get; private set; }
        public Vector3 HoldPose { get; private set; }

        /// <summary>Enter the hold at the given pose. False if already held (idempotent).</summary>
        public bool Begin(Vector3 pose)
        {
            if (Held) return false;
            Held = true;
            HoldPose = pose;
            return true;
        }

        /// <summary>Leave the hold. False if not held (idempotent).</summary>
        public bool End()
        {
            if (!Held) return false;
            Held = false;
            return true;
        }

        /// <summary>The fall-safety net must not evaluate while the boot hold owns the rig.</summary>
        public bool SuppressesFallCheck => Held;

        /// <summary>While held, any drift beyond tolerance (residual gravity/physics) gets re-pinned.</summary>
        public bool NeedsRepin(Vector3 current, float tolerance)
        {
            return Held && (current - HoldPose).sqrMagnitude > tolerance * tolerance;
        }
    }

    /// <summary>
    /// Singleton on XR Origin root. Persists the player rig across scene loads.
    ///
    /// Key invariant: exactly ONE XRInteractionManager is alive at any time.
    /// When a new scene loads its own manager, we ADOPT that scene manager
    /// (DontDestroyOnLoad it) and SAFELY DISCARD the old one by clearing its
    /// InputActionManager asset list before Destroy — preventing the shared
    /// InputActionAsset from being disabled by InputActionManager.OnDisable.
    /// </summary>
    public class PlayerRigPersistence : MonoBehaviour
    {
        private static PlayerRigPersistence _instance;
        private XRInteractionManager _xriManager;

        [Header("Global fall safety (any gravity world; independent of WorldRuntime/FallRespawner)")]
        [Tooltip("Meters the rig may fall below its last safe spawn before a forced respawn.")]
        [SerializeField] private float hardFallLimit = 60f;
        [Tooltip("Absolute Y backstop: below this, always respawn no matter what.")]
        [SerializeField] private float absoluteFloorY = -500f;
        private Vector3 _lastSafePosition;
        private bool _hasSafePosition;

        // DS-01 cold-boot hold: while the Home Hub owns _Boot, locomotion is suspended, the fall
        // net is disarmed and the rig is pinned. Released by TeleportToMarker after spawn settles.
        private readonly BootHoldState _bootHold = new BootHoldState();
        private readonly System.Collections.Generic.List<Behaviour> _bootSuspended =
            new System.Collections.Generic.List<Behaviour>();

        // #region agent log
        // NOTE: Application.persistentDataPath must NOT be called in static initializers
        // under IL2CPP (Android) — it causes a TypeInitializationException at .cctor time.
        private static string LogPath => Path.Combine(Application.persistentDataPath, "debug-cb967f.log");
        private static int _seq;

        private static void ZLog(string hyp, string msg, string kv = "")
        {
            _seq++;
            long ts = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string line = "{\"sessionId\":\"cb967f\",\"id\":\"log_" + ts + "_" + _seq +
                          "\",\"timestamp\":" + ts + ",\"location\":\"PlayerRigPersistence.cs\"" +
                          ",\"message\":\"" + msg.Replace("\"", "'") + "\"" +
                          ",\"data\":{" + kv.Replace("\"", "'") + "},\"hypothesisId\":\"" + hyp + "\"}";
            Debug.Log("ZIPTIDE_DIAG " + line);
            try { File.AppendAllText(LogPath, line + "\n", Encoding.UTF8); } catch { }
        }

        private void LogRaySnapshot(string phase)
        {
            int cams = 0;
            var camNames = new StringBuilder();
            foreach (var cam in Camera.allCameras)
                if (cam != null && cam.isActiveAndEnabled) { cams++; camNames.Append(cam.name + ";"); }

            int total = 0, active = 0;
            var rayDetails = new StringBuilder();
            foreach (var ray in GetComponentsInChildren<XRRayInteractor>(true))
            {
                total++;
                bool on = ray.gameObject.activeInHierarchy && ray.enabled;
                if (on) active++;
                rayDetails.Append(ray.gameObject.name + "=" + (on ? "ON" : "off") + ";");
            }
            ZLog("A", phase,
                "'cams':" + cams + ",'camNames':'" + camNames + "','rays':" + total +
                ",'activeRays':" + active + ",'rayDetails':'" + rayDetails + "'");
        }

        private void LogXROriginSnapshot(string phase)
        {
            int count = 0;
            var sb = new StringBuilder();
            var t = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (t != null)
            {
                foreach (var o in FindObjectsOfType(t))
                {
                    var comp = o as Component;
                    if (comp != null) { count++; sb.Append(comp.gameObject.name + "(active=" + comp.gameObject.activeInHierarchy + ",scene=" + comp.gameObject.scene.name + ");"); }
                }
            }
            ZLog("B", phase, "'xrOriginCount':" + count + ",'origins':'" + sb + "'");
        }

        private static void LogManagerSnapshot(string phase)
        {
            int count = 0;
            var sb = new StringBuilder();
            foreach (var m in FindObjectsOfType<XRInteractionManager>(true))
            {
                if (m == null) continue;
                count++;
                sb.Append(m.gameObject.name + "(scene=" + m.gameObject.scene.name + ");");
            }
            ZLog("C", phase, "'mgrCount':" + count + ",'mgrs':'" + sb + "'");
        }
        // #endregion agent log

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Seed the fall-safety with the starting position so the net works even before the
            // first teleport-to-spawn (e.g. a world with no spawn marker).
            _lastSafePosition = transform.position;
            _hasSafePosition = true;

            // #region agent log
            ZLog("A", "Awake", "'scene':'" + gameObject.scene.name + "'");
            LogManagerSnapshot("Awake_managers");
            LogRaySnapshot("Awake_BEFORE");
            // #endregion agent log

            EnsureXRIWiring();
            EnsureBelt();
            EnsureStunReceiver();
            EnsureCreditsHud();
            EnsureRillCompanion();
            EnsurePingTool();

            // #region agent log
            LogRaySnapshot("Awake_AFTER");
            // #endregion agent log
        }

        /// <summary>
        /// Global fall-safety net. Runs on the persistent rig in EVERY scene, so it catches the
        /// "fall forever, never respawn" class of bug even in worlds with no WorldRuntime, no
        /// FallRespawner, or a missing spawn marker. This is the backstop; FallRespawner +
        /// WorldProfile.fallYThreshold stay the per-world (nicer, fade-able) first line of defence.
        /// </summary>
        private void Update()
        {
            // DS-01: while the boot hold owns the rig, the fall net is disarmed and any residual
            // drift (e.g. a physics tick before suspension landed) is pinned back to the boot pose.
            if (_bootHold.SuppressesFallCheck)
            {
                if (_bootHold.NeedsRepin(transform.position, 0.05f))
                {
                    var cc = GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;
                    transform.position = _bootHold.HoldPose;
                    if (cc != null) cc.enabled = true;
                }
                return;
            }

            if (!_hasSafePosition) return;

            float relativeFloor = _lastSafePosition.y - hardFallLimit;
            if (transform.position.y < relativeFloor || transform.position.y < absoluteFloorY)
            {
                Debug.LogWarning("ZIPTIDE: FALL_SAFETY y=" + transform.position.y.ToString("F1")
                    + " below floor=" + Mathf.Max(relativeFloor, absoluteFloorY).ToString("F1")
                    + " — forcing respawn");
                ForceRespawn();
            }
        }

        /// <summary>
        /// Force the rig back to a safe spot: the scene's spawn marker if present, else the last
        /// recorded safe position. Always leaves the player standable instead of falling endlessly.
        /// </summary>
        private void ForceRespawn()
        {
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            var marker = FindObjectOfType<SpawnMarkerRuntime>();
            if (marker != null)
            {
                transform.position = marker.transform.position;
                transform.rotation = marker.transform.rotation;
            }
            else
            {
                // No spawn marker here — return to the last safe spot, nudged up so we settle
                // onto ground rather than inside it. Guarantees we never fall forever.
                transform.position = _lastSafePosition + Vector3.up * 0.5f;
            }

            if (cc != null) cc.enabled = true;
            _lastSafePosition = transform.position;
            Debug.Log("ZIPTIDE: FALL_SAFETY_RESPAWN to " + transform.position.ToString("F2"));
        }

        /// <summary>
        /// Ensures a BeltRig (with functional holster sockets) lives on the persistent rig so the
        /// player can holster a gun and have it travel between scenes. The belt was previously only
        /// added per-scene by ScenePatcherC0, which the APK build skips — so it never appeared.
        /// </summary>
        /// <summary>Ensures the ping tool (CONTROL_SCHEME.md) lives on the persistent rig.</summary>
        private void EnsurePingTool()
        {
            if (GetComponent<PingTool>() == null)
                gameObject.AddComponent<PingTool>();
            if (GetComponent<QuickSwap>() == null)
                gameObject.AddComponent<QuickSwap>();
            if (GetComponent<PlayerAvatarRig>() == null)
                gameObject.AddComponent<PlayerAvatarRig>();
        }

        /// <summary>
        /// Ensures a <see cref="PlayerStunReceiver"/> lives on the persistent rig so non-lethal drone
        /// stun bolts (Drone Combat V1) can flash the screen + briefly slow movement in every world.
        /// Must be rig-ensured: the rig lives in _Boot/DontDestroyOnLoad, so world patchers can't add it.
        /// </summary>
        private void EnsureStunReceiver()
        {
            if (GetComponent<PlayerStunReceiver>() == null)
            {
                gameObject.AddComponent<PlayerStunReceiver>();
                Debug.Log("ZIPTIDE: STUN_RECEIVER_ENSURED on persistent rig");
            }
        }

        /// <summary>
        /// Ensures a <see cref="CreditsHud"/> lives on the persistent rig so the player's credit balance is
        /// visible in every world (the economy was paying out but had no on-screen readout). Rig-ensured for
        /// the same reason as the belt/stun receiver: the rig is in _Boot/DontDestroyOnLoad.
        /// </summary>
        private void EnsureCreditsHud()
        {
            if (GetComponent<CreditsHud>() == null)
            {
                gameObject.AddComponent<CreditsHud>();
                Debug.Log("ZIPTIDE: CREDITS_HUD_ENSURED on persistent rig");
            }
        }

        /// <summary>
        /// Ensures RILL (the story companion — orb + subtitle line delivery) lives on the persistent rig
        /// so it crosses scenes automatically, per the MASTER_BUILD_PLAN §5 spec ("ships with
        /// PlayerRigPersistence"). Same rig-ensured pattern as the stun receiver / credits HUD.
        /// </summary>
        private void EnsureRillCompanion()
        {
            if (GetComponent<RillCompanion>() == null)
            {
                gameObject.AddComponent<RillCompanion>();
                Debug.Log("ZIPTIDE: RILL_ENSURED on persistent rig");
            }
        }

        private void EnsureBelt()
        {
            if (GetComponentInChildren<BeltRig>(true) != null) return;
            var go = new GameObject(Ziptide.Core.ZiptideConstants.GoBeltRig);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<BeltRig>();
            Debug.Log("ZIPTIDE: BELT_ENSURED on persistent rig");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _instance = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // #region agent log
            ZLog("B", "OnSceneLoaded", "'scene':'" + scene.name + "'");
            LogXROriginSnapshot("OnSceneLoaded_BEFORE_Destroy");
            LogManagerSnapshot("OnSceneLoaded_BEFORE_XRIWiring");
            // #endregion agent log

            DestroyDuplicateXROrigins();

            // #region agent log
            LogXROriginSnapshot("OnSceneLoaded_AFTER_Destroy");
            // #endregion agent log

            RebindSceneSystems();

            // When TravelCoordinator is driving the load, it owns teleport, wiring, and restore.
            // Skip them here to avoid double-restore and races.
            if (TravelCoordinator.IsTravelling)
            {
                // #region agent log
                LogManagerSnapshot("OnSceneLoaded_SKIP_wiring_restore_coordinator_owns");
                // #endregion agent log
                return;
            }

            TeleportToSpawnMarker();
            EnsureXRIWiring();

            // Deferred coroutine: waits 2 frames so XRI interactors are registered before
            // attempting socket SelectEnter on holstered items.
            StartCoroutine(InventoryState.RestoreAfterTravel(transform));

            // #region agent log
            LogManagerSnapshot("OnSceneLoaded_AFTER_XRIWiring");
            LogRaySnapshot("OnSceneLoaded_AFTER");
            // #endregion agent log
        }

        public void PrepareForSceneTravel()
        {
            // #region agent log
            ZLog("A", "PrepareForSceneTravel_START", "");
            // #endregion agent log
            InventoryState.SaveBeforeTravel();
            // #region agent log
            ZLog("A", "PrepareForSceneTravel_DONE", "'savedCount':" + InventoryState.Items.Count);
            // #endregion agent log
        }

        public void EnsureXRIWiring()
        {
            // Input-mutation window: this method disables sibling actions inside live assets
            // (DisableAnchorInputActions) and re-enables whole assets (EnsurePersistentInputActions).
            // InputSystem 1.7 re-resolves its InputActionState over the next frame(s); an XRI
            // locomotion provider polling ReadValue inside that window throws the ApplyProcessors
            // NullReferenceException (proven on every post-travel arrival by PlayMode run
            // 29506225255 — SnapTurn and ContinuousTurn stacks directly after
            // ANCHOR_ACTIONS_DISABLED). Suspend the polling readers across the window; they are
            // restored two frames later, after the action state settles.
            SuspendLocomotionReadersForInputMutation();

            var allManagers = FindObjectsOfType<XRInteractionManager>(true);

            // Identify scene-local manager (lives in the active scene, not DontDestroyOnLoad).
            // The scene-local manager always has the correct InputActionManager+assets wired up.
            XRInteractionManager sceneManager = null;
            if (allManagers != null)
            {
                foreach (var m in allManagers)
                {
                    if (m != null && m.gameObject.scene == SceneManager.GetActiveScene())
                    {
                        sceneManager = m;
                        break;
                    }
                }
            }

            if (sceneManager != null && sceneManager != _xriManager)
            {
                // CRITICAL FIX: clear the OLD manager's InputActionManager asset list BEFORE
                // destroying it. InputActionManager.OnDisable calls actionAsset.Disable() which
                // would kill head-tracking and all controller input (causing the frozen view bug).
                if (_xriManager != null)
                {
                    // #region agent log
                    ZLog("C", "Discarding old XRI manager, clearing assets first",
                        "'oldMgr':'" + _xriManager.gameObject.name + "','newMgr':'" + sceneManager.gameObject.name + "'");
                    // #endregion agent log
                    SafeClearInputActionAssets(_xriManager);
                    Destroy(_xriManager.gameObject);
                }
                _xriManager = sceneManager;
                DontDestroyOnLoad(_xriManager.gameObject);
                // #region agent log
                ZLog("C", "Adopted scene manager", "'mgr':'" + _xriManager.gameObject.name + "'");
                // #endregion agent log
            }
            else if (_xriManager == null)
            {
                if (allManagers != null && allManagers.Length > 0 && allManagers[0] != null)
                {
                    _xriManager = allManagers[0];
                    DontDestroyOnLoad(_xriManager.gameObject);
                }
                else
                {
                    Debug.LogWarning("ZIPTIDE: XRI_MISSING (creating XRInteractionManager)");
                    var go = new GameObject("__XRI");
                    go.transform.SetParent(transform, false);
                    _xriManager = go.AddComponent<XRInteractionManager>();
                    DontDestroyOnLoad(_xriManager.gameObject);
                }
            }

            // Destroy any remaining duplicate managers that aren't our primary one.
            if (allManagers != null)
            {
                foreach (var m in allManagers)
                {
                    if (m != null && m != _xriManager)
                    {
                        SafeClearInputActionAssets(m);
                        Destroy(m.gameObject);
                    }
                }
            }

            EnsurePersistentInputActions();

            // Rebind only ACTIVE interactors. Skip inactive GameObjects (e.g. teleport ray GOs
            // managed by ActionBasedControllerManager) to avoid disrupting XRInteractionGroup state.
            int totalRays = 0, enabledRays = 0, skipped = 0;
            foreach (var i in GetComponentsInChildren<XRBaseInteractor>(true))
            {
                if (!i.gameObject.activeInHierarchy) { skipped++; continue; }
                if (i.interactionManager != _xriManager)
                    i.interactionManager = _xriManager;
                if (i is XRRayInteractor ray)
                {
                    totalRays++;
                    if (i.enabled) enabledRays++;
                    // Realistic reach — the rays were 10-30m (grab/aim across the room). Set at
                    // runtime here so it sticks on the live rig regardless of edit-time patching. Tunable.
                    ray.maxRaycastDistance = 2.5f;
                }
            }

            // ⭐ THE persistent bug: the right thumbstick rotated/translated the held gun/hammer instead of
            // TURNING the player. XRI's XRRayInteractor gates anchor manipulation on the serialized bool
            // m_AllowAnchorControl. The OLD reflection looked up "m_EnableAnchorControl" — a name that does
            // NOT exist on the interactor (the real field is m_AllowAnchorControl, confirmed in the XRI 2.5.4
            // Ray/Teleport/Gaze interactor prefabs), so GetField returned null and the gun kept spinning.
            // Gate it off on EVERY ray, including the INACTIVE teleport ray each hand owns (so it stays off
            // after ActionBasedControllerManager swaps rays). Belt-and-suspenders: also hard-disable the two
            // bound input actions so nothing reads the stick for anchor control.
            foreach (var ray in GetComponentsInChildren<XRRayInteractor>(true))
            {
                DisableAnchorControl(ray);
                // Force grab: a grabbed object snaps to the HAND instead of hanging out at the ray's hit
                // distance. (Device test: "pick up the gun at that distance and it stays at that distance.")
                EnableForceGrab(ray);
            }
            DisableAnchorInputActions();

            // Fix the long↔short ray-length jump: each hand has TWO ray interactors (select + teleport),
            // each with its OWN XRInteractorLineVisual, swapped by ActionBasedControllerManager. The earlier
            // fix only touched the active select ray, so the inactive teleport ray kept its long (~10m) line
            // and the drawn length "jumped" when the manager swapped which ray was active. Clamp the drawn
            // length on ALL line visuals (incl. inactive) so it's a stable short ray either way.
            foreach (var lv in GetComponentsInChildren<XRInteractorLineVisual>(true))
            {
                if (lv == null) continue;
                lv.overrideInteractorLineLength = true;
                lv.lineLength = 1.4f;   // realistic reach — 2.5 still read as "too long" on device
                DisableLineSnap(lv);    // don't stretch the line out to a target you're pointing at
            }

            // Right thumbstick was driving MOVEMENT because EnsureLocomotionRig binds BOTH hands to the
            // Move action. Drop the right-hand move binding at runtime so the right stick only TURNS (the
            // snap/smooth turn providers already own it); the left stick still moves. Public API → sticks.
            foreach (var mp in GetComponentsInChildren<ActionBasedContinuousMoveProvider>(true))
            {
                if (mp != null) mp.rightHandMoveAction = default;
            }

            int cams = 0;
            foreach (var cam in Camera.allCameras)
                if (cam != null && cam.isActiveAndEnabled) cams++;

            Debug.Log("ZIPTIDE: XRI_WIRING cams=" + cams + " rays=" + totalRays +
                      "(active=" + enabledRays + ") skipped=" + skipped);
            if (totalRays == 0) Debug.LogWarning("ZIPTIDE: NO_RAY_INTERACTORS");
        }

        // Turns the ray interactor's anchor manipulation OFF — the gate that decides whether the right
        // thumbstick rotates/translates the held object instead of being free for player turning.
        // XRI 2.5.4 exposes no compile-safe public setter and the cloud build can't verify package symbols,
        // so we set the serialized bool by cached reflection. CRITICAL: the field is m_AllowAnchorControl
        // (confirmed in the XRI Ray/Teleport/Gaze interactor prefabs) — the previous code looked up
        // "m_EnableAnchorControl", which does NOT exist, so GetField returned null and the gate was never
        // touched (that's the bug that survived every round). Log loudly if the name ever drifts again.
        private static System.Reflection.FieldInfo _anchorControlField;
        private static bool _anchorControlFieldResolved;
        private static void DisableAnchorControl(XRRayInteractor ray)
        {
            if (ray == null) return;
            if (!_anchorControlFieldResolved)
            {
                _anchorControlFieldResolved = true;
                _anchorControlField = typeof(XRRayInteractor).GetField("m_AllowAnchorControl",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (_anchorControlField == null || _anchorControlField.FieldType != typeof(bool))
                    Debug.LogWarning("ZIPTIDE: ANCHOR_FIELD_MISSING m_AllowAnchorControl — gun may still rotate");
            }
            if (_anchorControlField != null && _anchorControlField.FieldType == typeof(bool))
                _anchorControlField.SetValue(ray, false);
        }

        // Force grab on a ray interactor (serialized m_UseForceGrab, confirmed in the XRI Ray Interactor
        // prefab). With it on, grabbing a distant object reels it to the hand instead of holding it out at
        // the ray's hit distance. Reflection (no compile-verifiable public symbol in the cloud build).
        private static System.Reflection.FieldInfo _forceGrabField;
        private static bool _forceGrabResolved;
        private static void EnableForceGrab(XRRayInteractor ray)
        {
            if (ray == null) return;
            if (!_forceGrabResolved)
            {
                _forceGrabResolved = true;
                _forceGrabField = typeof(XRRayInteractor).GetField("m_UseForceGrab",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }
            if (_forceGrabField != null && _forceGrabField.FieldType == typeof(bool))
                _forceGrabField.SetValue(ray, true);
        }

        // Stop the line visual from snapping its endpoint out to whatever it's pointing at (serialized
        // m_SnapEndpointIfAvailable, confirmed in the XRI Ray Interactor prefab) — that snap is why the ray
        // "lengthened" when aimed at a grabbable. Keeps the drawn line a stable short length.
        private static System.Reflection.FieldInfo _lineSnapField;
        private static bool _lineSnapResolved;
        private static void DisableLineSnap(XRInteractorLineVisual lv)
        {
            if (lv == null) return;
            if (!_lineSnapResolved)
            {
                _lineSnapResolved = true;
                _lineSnapField = typeof(XRInteractorLineVisual).GetField("m_SnapEndpointIfAvailable",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }
            if (_lineSnapField != null && _lineSnapField.FieldType == typeof(bool))
                _lineSnapField.SetValue(lv, false);
        }

        // Belt-and-suspenders for the anchor bug: hard-disable the two input actions the right thumbstick is
        // bound to for anchor control ("Rotate Anchor" / "Translate Anchor" in the Interaction map). Even if
        // the interactor gate above is ever re-enabled, no input reaches the manipulation. Runs AFTER
        // EnsurePersistentInputActions() (which Enable()s the whole asset) and on every scene load, so the
        // two actions stay off while everything else (grab, turn, move) stays live.
        private void DisableAnchorInputActions()
        {
            var primary = _xriManager != null ? _xriManager.GetComponent<InputActionManager>() : null;
            if (primary == null || primary.actionAssets == null) return;
            int disabled = 0;
            foreach (var asset in primary.actionAssets)
            {
                if (asset == null) continue;
                foreach (var map in asset.actionMaps)
                {
                    if (map == null) continue;
                    foreach (var action in map.actions)
                    {
                        if (action == null) continue;
                        if (action.name == "Rotate Anchor" || action.name == "Translate Anchor")
                        {
                            action.Disable();
                            disabled++;
                        }
                    }
                }
            }
            Debug.Log("ZIPTIDE: ANCHOR_ACTIONS_DISABLED count=" + disabled);
        }

        /// <summary>
        /// Guarantees the persistent rig owns its input. The scene's InputActionManager often
        /// lives on a SEPARATE GameObject (e.g. "_InputActionManager") that the wiring code does
        /// not see and that is destroyed when the scene unloads on travel — its OnDisable then
        /// calls actionAsset.Disable() and permanently kills all controller input in the next
        /// scene (the dead-controller "frozen" bug). Here we (1) gather every input action asset
        /// the rig references, (2) put them on an InputActionManager on the PERSISTENT manager
        /// object, (3) clear the asset list on every other InputActionManager so their OnDisable
        /// is a no-op, and (4) (re)enable the assets so input survives scene travel.
        /// </summary>
        private void EnsurePersistentInputActions()
        {
            var assets = new System.Collections.Generic.List<InputActionAsset>();
            void Add(InputActionAsset a) { if (a != null && !assets.Contains(a)) assets.Add(a); }

            var allManagers = FindObjectsOfType<InputActionManager>(true);
            foreach (var m in allManagers)
            {
                if (m == null || m.actionAssets == null) continue;
                foreach (var a in m.actionAssets) Add(a);
            }
            foreach (var c in GetComponentsInChildren<ActionBasedController>(true))
                Add(GetActionAssetFromController(c));

            var primary = _xriManager.GetComponent<InputActionManager>()
                          ?? _xriManager.gameObject.AddComponent<InputActionManager>();

            if (assets.Count == 0)
            {
                Debug.LogWarning("ZIPTIDE: INPUT_ACTIONS_MISSING");
                return;
            }

            primary.actionAssets = assets;

            // Neutralise every other manager so its OnDisable on scene unload can't disable our assets.
            foreach (var m in allManagers)
            {
                if (m == null || m == primary) continue;
                ClearInputActionAssets(m);
            }

            // Keep the assets enabled (idempotent). Restores input if a prior unload disabled them.
            foreach (var a in assets)
                a.Enable();
        }

        private static InputActionAsset GetActionAssetFromController(ActionBasedController c)
        {
            if (c == null) return null;
            var prop = c.selectAction;
            if (prop.reference != null && prop.reference.action != null && prop.reference.action.actionMap != null)
                return prop.reference.action.actionMap.asset;
            return null;
        }

        /// <summary>Clears m_ActionAssets on an InputActionManager so its OnDisable won't disable shared assets.</summary>
        private static void ClearInputActionAssets(InputActionManager iam)
        {
            if (iam == null) return;
            try
            {
                var field = typeof(InputActionManager).GetField(
                    "m_ActionAssets", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                    field.SetValue(iam, new System.Collections.Generic.List<InputActionAsset>());
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("ZIPTIDE: ClearInputActionAssets failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Clears the action asset list on an InputActionManager via reflection BEFORE
        /// the manager is destroyed. This prevents InputActionManager.OnDisable from
        /// calling actionAsset.Disable(), which would kill all input including head tracking.
        /// </summary>
        private static void SafeClearInputActionAssets(XRInteractionManager mgr)
        {
            if (mgr == null) return;
            var iam = mgr.GetComponent<InputActionManager>();
            if (iam == null) return;
            try
            {
                var field = typeof(InputActionManager).GetField(
                    "m_ActionAssets",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                    field.SetValue(iam, new System.Collections.Generic.List<InputActionAsset>());
                // #region agent log
                ZLog("C", "SafeClearInputActionAssets: cleared assets on " + mgr.gameObject.name, "");
                // #endregion agent log
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("ZIPTIDE: SafeClearInputActionAssets failed: " + ex.Message);
            }
        }

        private void DestroyDuplicateXROrigins()
        {
            var destroyed = new System.Collections.Generic.HashSet<GameObject>();
            var xrOriginType = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (xrOriginType != null)
            {
                foreach (var obj in FindObjectsOfType(xrOriginType))
                {
                    var comp = obj as Component;
                    if (comp != null && comp.gameObject != gameObject)
                    {
                        Debug.Log("[Ziptide] Destroying duplicate XR Origin (type): " + comp.gameObject.name);
                        comp.gameObject.SetActive(false);
                        destroyed.Add(comp.gameObject);
                        Destroy(comp.gameObject);
                    }
                }
            }

            // Name-based fallback for rigs without XROrigin component attached.
            string[] knownNames = { "XR Origin", "XR Origin (XR Interaction Toolkit)", "XROrigin", "XR Rig", "XR Origin (XR Rig)" };
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (root == gameObject || destroyed.Contains(root)) continue;
                foreach (var rigName in knownNames)
                {
                    if (root.name == rigName)
                    {
                        Debug.Log("[Ziptide] Destroying duplicate XR Origin (name): " + root.name);
                        root.SetActive(false);
                        destroyed.Add(root);
                        Destroy(root);
                        break;
                    }
                }
            }

            // #region agent log
            ZLog("B", "DestroyDuplicateXROrigins done", "'destroyed':" + destroyed.Count);
            // #endregion agent log
        }

        /// <summary>
        /// DS-01 — enter/leave the cold-boot hold. BootLoader arms it before presenting the Home
        /// Hub; TeleportToMarker releases it after a content spawn settles. While held: locomotion
        /// providers (move/turn/snap/dash) are suspended and restored on release with the
        /// ShipFlightRuntime idiom, the fall net is disarmed, and the rig is pinned to its boot pose.
        /// </summary>
        public static void SetBootHold(bool held)
        {
            if (_instance == null)
            {
                Debug.LogWarning("ZIPTIDE: BOOT_HOLD no_rig held=" + held);
                return;
            }
            _instance.ApplyBootHold(held);
        }

        private void ApplyBootHold(bool held)
        {
            if (held)
            {
                if (!_bootHold.Begin(transform.position)) return;
                _bootSuspended.Clear();
                CollectForBootHold(GetComponentsInChildren<ActionBasedContinuousMoveProvider>(true));
                CollectForBootHold(GetComponentsInChildren<ActionBasedContinuousTurnProvider>(true));
                CollectForBootHold(GetComponentsInChildren<ActionBasedSnapTurnProvider>(true));
                CollectForBootHold(GetComponentsInChildren<DashLocomotion>(true));
                Debug.Log("ZIPTIDE: BOOT_HOLD on pose=" + transform.position.ToString("F2")
                    + " suspended=" + _bootSuspended.Count);
            }
            else
            {
                if (!_bootHold.End()) return;
                foreach (var b in _bootSuspended)
                    if (b != null) b.enabled = true;
                int restored = _bootSuspended.Count;
                _bootSuspended.Clear();

                // Re-arm the fall net from wherever the rig settled — this is the ONLY place fall
                // recovery arms after boot, per the forensic contract.
                _lastSafePosition = transform.position;
                _hasSafePosition = true;
                Debug.Log("ZIPTIDE: BOOT_HOLD off safe=" + transform.position.ToString("F2")
                    + " restored=" + restored);
            }
        }

        private void CollectForBootHold(Behaviour[] providers)
        {
            foreach (var b in providers)
            {
                if (b == null || !b.enabled) continue;
                b.enabled = false;
                _bootSuspended.Add(b);
            }
        }

        // ── Travel-time input-mutation window (recovery: the post-travel SnapTurn/ContinuousTurn
        //    ApplyProcessors NRE, PlayMode run 29506225255) ─────────────────────────────────────
        // Ownership rules mirror the boot hold exactly: each mechanism disables only providers it
        // found ENABLED and restores only its own set, so the two never fight. If the boot hold is
        // holding when the settle window closes, the readers are HANDED to the boot hold instead of
        // re-enabled — the hold's release-on-spawn then restores them (locomotion must never wake
        // under a held menu).
        private readonly System.Collections.Generic.List<Behaviour> _mutationSuspendedReaders =
            new System.Collections.Generic.List<Behaviour>();
        private Coroutine _mutationReaderRestore;

        private void SuspendLocomotionReadersForInputMutation()
        {
            if (_mutationReaderRestore != null) return; // window already open; restorer owns the set
            CollectForMutation(GetComponentsInChildren<ActionBasedContinuousMoveProvider>(true));
            CollectForMutation(GetComponentsInChildren<ActionBasedContinuousTurnProvider>(true));
            CollectForMutation(GetComponentsInChildren<ActionBasedSnapTurnProvider>(true));
            if (_mutationSuspendedReaders.Count == 0) return;
            _mutationReaderRestore = StartCoroutine(RestoreReadersAfterInputSettle());
        }

        private void CollectForMutation(Behaviour[] providers)
        {
            foreach (var b in providers)
            {
                if (b == null || !b.enabled) continue;
                b.enabled = false;
                _mutationSuspendedReaders.Add(b);
            }
        }

        private IEnumerator RestoreReadersAfterInputSettle()
        {
            // Frame counts are not a settle guarantee: run 29518294931 showed the FIRST poll after a
            // two-frame wait still hitting the ApplyProcessors NRE. Run 29518996619 went further —
            // a PASSING probe followed by a clean MOVE_DIAG read still preceded a ContinuousTurn NRE,
            // because the restore landed while the TRAVEL COROUTINE was still running and input churn
            // (manager adoption, asset clears, guard reasserts) had not finished. The identical route
            // reads clean in steady state (the perf-route legs), so the rule is: readers stay
            // suspended until travel has fully ENDED, then probe-settle, then two extra frames.
            yield return null;
            float travelDeadline = Time.realtimeSinceStartup + 45f; // covers the 30 s scene-load budget
            while (Time.realtimeSinceStartup < travelDeadline && TravelCoordinator.IsTravelling)
                yield return null;

            float deadline = Time.realtimeSinceStartup + 2f;
            while (Time.realtimeSinceStartup < deadline && !SuspendedReaderActionsReadSafely())
                yield return null;
            if (!SuspendedReaderActionsReadSafely())
                Debug.LogWarning("ZIPTIDE: INPUT_MUTATION_SETTLE_TIMEOUT — re-enabling readers anyway");
            yield return null;
            yield return null; // two settled frames beyond the last clean probe

            bool handedToBootHold = _bootHold.Held;
            foreach (var b in _mutationSuspendedReaders)
            {
                if (b == null) continue;
                if (handedToBootHold) _bootSuspended.Add(b);
                else b.enabled = true;
            }
            Debug.Log("ZIPTIDE: INPUT_MUTATION_READERS restored=" + _mutationSuspendedReaders.Count
                + (handedToBootHold ? " handedTo=bootHold" : ""));
            _mutationSuspendedReaders.Clear();
            _mutationReaderRestore = null;
        }

        /// <summary>True when every action the suspended readers poll reads without throwing —
        /// i.e. the InputActionState re-resolution triggered by the wiring mutation has completed.</summary>
        private bool SuspendedReaderActionsReadSafely()
        {
            foreach (var b in _mutationSuspendedReaders)
            {
                switch (b)
                {
                    case ActionBasedContinuousMoveProvider move:
                        if (!ActionReadsSafely(move.leftHandMoveAction) ||
                            !ActionReadsSafely(move.rightHandMoveAction)) return false;
                        break;
                    case ActionBasedContinuousTurnProvider turn:
                        if (!ActionReadsSafely(turn.leftHandTurnAction) ||
                            !ActionReadsSafely(turn.rightHandTurnAction)) return false;
                        break;
                    case ActionBasedSnapTurnProvider snap:
                        if (!ActionReadsSafely(snap.leftHandSnapTurnAction) ||
                            !ActionReadsSafely(snap.rightHandSnapTurnAction)) return false;
                        break;
                }
            }
            return true;
        }

        private static bool ActionReadsSafely(InputActionProperty property)
        {
            var action = property.action;
            if (action == null || !action.enabled) return true; // nothing to settle
            try
            {
                action.ReadValue<Vector2>();
                return true;
            }
            catch (System.NullReferenceException)
            {
                return false; // InputActionState still mid-re-resolve — the exact NRE the readers would hit
            }
            catch (System.InvalidOperationException)
            {
                return false; // value-type mismatch during re-resolve — equally unsafe to poll
            }
        }

        public void TeleportToSpawnMarker()
        {
            TeleportToMarker(null);
        }

        /// <summary>
        /// Move the rig to the spawn marker whose markerId matches (or the 'player' marker / first
        /// marker if id is null or not found). Used by normal travel and by the dev warp tool to
        /// drop into a specific part of a world.
        /// </summary>
        public void TeleportToMarker(string markerId)
        {
            var marker = FindMarker(markerId);
            if (marker == null)
            {
                Debug.Log("[Ziptide] No SpawnMarkerRuntime"
                    + (string.IsNullOrEmpty(markerId) ? "" : " id='" + markerId + "'")
                    + " found, keeping position.");
                // DS-01: a marker-less CONTENT scene still releases the boot hold — the player must
                // never arrive in a world frozen. (_Boot itself keeps the hold: menu still owns it.)
                if (SceneManager.GetActiveScene().name != Ziptide.Core.ZiptideConstants.SceneBoot)
                    ApplyBootHold(false);
                return;
            }
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // --- Roomscale-correct spawn ---------------------------------------------------
            // The rig ROOT is not where the player physically stands; the head (camera) is offset
            // from the root by the player's tracked position inside their physical playspace. If we
            // just slam the root onto the marker, a player standing off-center lands off-center too
            // (that's how Terry ended up 10ft left, over the goo river). So instead we:
            //   1. snap the marker's Y to the real walkable surface under it (never over goo/void),
            //   2. shift the rig so the HEAD lands on the marker XZ (cancels the playspace offset),
            //   3. drop the rig base to the snapped ground.
            Vector3 target = marker.transform.position;

            // 1. Ground-snap: cast down from just above the marker to find the actual floor.
            //    QueryTriggerInteraction.Ignore so goo/trigger volumes never count as ground.
            if (Physics.Raycast(target + Vector3.up * 3f, Vector3.down, out var hit, 12f,
                    ~0, QueryTriggerInteraction.Ignore))
            {
                target.y = hit.point.y;
            }

            // First put the root on the (ground-snapped) marker so head tracking is sampled there.
            transform.position = target;
            transform.rotation = marker.transform.rotation;

            // 2. Head-align: find the player's head and shift the rig so the head's XZ sits on the
            //    marker XZ. This cancels whatever physical offset the player has in their playspace.
            var cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                Vector3 headDelta = target - cam.transform.position;
                headDelta.y = 0f; // only correct horizontal drift; Y is owned by the ground-snap
                transform.position += headDelta;

                // 3. Lock the rig base to the snapped ground height.
                Vector3 p = transform.position;
                p.y = target.y;
                transform.position = p;
            }

            if (cc != null) cc.enabled = true;

            // Record this as the safe spot for the global fall-safety net.
            _lastSafePosition = transform.position;
            _hasSafePosition = true;

            // DS-01: the spawn has settled — release the cold-boot hold (restores locomotion and
            // re-arms the fall net from this fresh spawn). No-op when the hold isn't active.
            ApplyBootHold(false);

            Debug.Log("ZIPTIDE: SPAWN_AT marker='" + marker.markerId + "' rig=" + transform.position.ToString("F2")
                + " markerGround=" + target.ToString("F2"));
        }

        /// <summary>Find a spawn marker by id; falls back to the 'player' marker, then the first one.</summary>
        private static SpawnMarkerRuntime FindMarker(string markerId)
        {
            var all = FindObjectsOfType<SpawnMarkerRuntime>();
            if (all == null || all.Length == 0) return null;
            if (!string.IsNullOrEmpty(markerId))
                foreach (var m in all)
                    if (m != null && m.markerId == markerId) return m;
            foreach (var m in all)
                if (m != null && m.markerId == "player") return m;
            return all[0];
        }

        private void RebindSceneSystems()
        {
            var worldRuntime = FindObjectOfType<WorldRuntime>();
            if (worldRuntime == null) return;
            var fallRespawner = GetComponentInChildren<FallRespawner>(true);
            if (fallRespawner != null)
                fallRespawner.SetWorldRuntime(worldRuntime);
        }
    }
}
