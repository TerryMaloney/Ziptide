namespace Ziptide.Core
{
    /// <summary>
    /// Compile-time constants for asset paths, scene names, layer names, and magic strings.
    /// Centralises all string literals that would otherwise be scattered across patchers and runtime code.
    /// </summary>
    public static class ZiptideConstants
    {
        // ── Scene Names ────────────────────────────────────────────────────
        public const string SceneBoot           = "_Boot";
        public const string SceneTestRoom       = "MilestoneA_GrabCube";
        public const string SceneD0City         = "D0_City";
        public const string SceneSandbox        = "SandboxTestLab";
        public const string SceneSample         = "SampleScene";

        /// <summary>Scene loaded by _Boot on Start. Change this to redirect the entry point.</summary>
        // ⚠ TEMPORARY DEV BYPASS (2026-06-18, T-Dog): boot straight into the Sandbox so the gravity gun +
        // drones are reachable WITHOUT the in-VR Dev Menu (currently renders as a dead black panel
        // on-device). REVERT to SceneTestRoom once the Dev Menu renders/clicks on the headset.
        // See docs/HANDOFF.md entry (t).
        public const string FirstWorldScene     = SceneSandbox;

        // ── Asset Paths ────────────────────────────────────────────────────
        public const string PathDefaultWorldProfile   = "Assets/Ziptide/Content/World/DefaultWorldProfile.asset";
        public const string PathDefaultCityKit        = "Assets/Ziptide/Content/City/DefaultCityKit.asset";
        public const string PathD0WorldPack           = "Assets/Ziptide/Content/WorldPacks/D0_WorldPack.asset";
        public const string PathTestRoomWorldPack     = "Assets/Ziptide/Content/WorldPacks/TestRoom_WorldPack.asset";
        public const string PathInputActionAsset      = "Assets/Ziptide/Platform/Quest/ZiptideActions.inputactions";
        public const string PathLocomotionProfile     = "Assets/Ziptide/Content/DefaultLocomotionProfile.asset";

        // ── Layer Names ────────────────────────────────────────────────────
        public const string LayerDefault        = "Default";
        public const string LayerInteractable   = "Interactable";
        public const string LayerEnvironment    = "Environment";
        public const string LayerPlayer         = "Player";
        public const string LayerUI             = "UI";
        public const string LayerBullet         = "Bullet";

        // ── Tag Names ──────────────────────────────────────────────────────
        public const string TagPlayer           = "Player";
        public const string TagSpawnMarker      = "SpawnMarker";

        // ── Diagnostic Log Prefixes ────────────────────────────────────────
        public const string DiagPrefix         = "ZIPTIDE: ";
        public const string DiagTravelStart    = "ZIPTIDE: TRAVEL_START";
        public const string DiagTravelOk       = "ZIPTIDE: TRAVEL_OK";
        public const string DiagTravelFail     = "ZIPTIDE: TRAVEL_FAIL";
        public const string DiagXriReady       = "ZIPTIDE: XRI_READY";
        public const string DiagXriNotReady    = "ZIPTIDE: XRI_NOT_READY";
        public const string DiagAuditOk        = "ZIPTIDE: AUDIT_OK";
        public const string DiagAuditFail      = "ZIPTIDE: AUDIT_FAIL";
        public const string DiagDupSingleton   = "ZIPTIDE: DUP_SINGLETON";
        public const string DiagProximityTravel = "ZIPTIDE: PROXIMITY_TRAVEL";

        // ── Singleton GameObject Names ─────────────────────────────────────
        public const string GoXROrigin          = "XR Origin";
        public const string GoTravelCoordinator = "TravelCoordinator";
        public const string GoAudioDirector     = "AudioDirector";
        public const string GoNarrativeSystem   = "NarrativeSaveSystem";
        public const string GoRILL              = "RILL";

        // ── Patcher Sentinel GameObject Names ─────────────────────────────
        public const string GoSpawnPlayer       = "__SPAWN_PLAYER";
        public const string GoCityRoot          = "__D1_CITY_ROOT";
        public const string GoWorldPackHolder   = "__WORLD_PACK";
        public const string GoBeltRig           = "BeltRig";
        public const string GoWorldTravelStation = "WorldTravelStation";

        // ── Performance Budgets ────────────────────────────────────────────
        /// <summary>Target frame rate for Quest 2/3.</summary>
        public const int TargetFrameRate        = 72;
        /// <summary>Maximum tris per world scene at build time.</summary>
        public const int MaxTrisPerScene        = 200_000;
        /// <summary>Maximum active AudioSources at once.</summary>
        public const int MaxAudioSources        = 8;
    }
}
