# ZIPTIDE R0 Ownership Maps

- Scanned files: **745**
- Scanner findings: **2718**
- Inventory systems: **22**

This report groups exact lexical evidence. A row means the source contains the named ownership signal; it does not by itself declare a defect.

## Exposure ledger

- **DIAGNOSTIC:** `DEV_DIAGNOSTICS`
- **GOLDEN_PATH:** `BOOT_FLOW`, `JOB_REPAIR_OBJECTIVE`, `SHIP_PRESENTATION_ROOT`, `TRAVEL`
- **PROTOTYPE_HIDDEN:** `MELEE`, `MULTIPLAYER_TIDEFRONT`
- **REPLACE_OR_MERGE:** `CAMERA_POLICY`, `CREATURE_CONTACT`, `HUD_POLICY`, `INPUT_CONTRACT`, `ITEM_PRESENTATION`, `MATERIAL_FALLBACK_POLICY`, `RUNTIME_UI`, `SCENE_PRESENTATION`, `WEAPON_OWNER_COLLISION`, `XR_RIG_INPUT_SESSION`
- **SUPPORT:** `AUDIO_COMPOSITION`, `CI_INTEGRATION_HARNESS`, `RECOVERY_EXPOSURE`, `SAVE_PROFILE`, `WORLD_FACTORY`

## Unresolved canonical owners

None.

## Bootstrap & persistence

### `Ziptide.Core.DebugHUD` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs:22` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs:29` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(s_Root);`

### `Ziptide.Core.EnsureXRCameraActive` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/EnsureXRCameraActive.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/EnsureXRCameraActive.cs:11` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Core.RecoveryRuntimeGate` — 2 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryRuntimeGate.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryRuntimeGate.cs:23` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryRuntimeGate.cs:31` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`

### `Ziptide.Core.RuntimeHealthMonitor` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs:33` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs:39` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`

### `Ziptide.Core.RuntimeInputEnabler` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:15` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Core.RuntimeMaterialFixer` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:14` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Core.VRBootDiagnostics` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/VRBootDiagnostics.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/VRBootDiagnostics.cs:14` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Editor.Art.BuildingKitLibrary` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:27` **EDITOR_BOOTSTRAP** — `[InitializeOnLoadMethod]`

### `Ziptide.Editor.Art.CavernKitLibrary` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:22` **EDITOR_BOOTSTRAP** — `[InitializeOnLoadMethod]`

### `Ziptide.Editor.Art.ForgeBuildingKit` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/ForgeBuildingKit.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/ForgeBuildingKit.cs:16` **EDITOR_BOOTSTRAP** — `/// (last-registration-wins needs deterministic order, and [InitializeOnLoadMethod] order isn't).`

### `Ziptide.Editor.Audit.AuditPhysicsSync` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/AuditPhysicsSync.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/AuditPhysicsSync.cs:14` **EDITOR_BOOTSTRAP** — `[InitializeOnLoad]`

### `Ziptide.Editor.DevTools.DevWarpPlayHook` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/DevTools/DevWarpPlayHook.cs`
  - `Ziptide/Assets/Ziptide/Editor/DevTools/DevWarpPlayHook.cs:14` **EDITOR_BOOTSTRAP** — `[InitializeOnLoad]`

### `Ziptide.Editor.Patching.ToxicCityRiverSaveHook` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverSaveHook.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverSaveHook.cs:16` **EDITOR_BOOTSTRAP** — `[InitializeOnLoad]`

### `Ziptide.Editor.Patching.ToxicCityStageASaveHook` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageASaveHook.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageASaveHook.cs:16` **EDITOR_BOOTSTRAP** — `[InitializeOnLoad]`

### `Ziptide.Editor.Patching.ToxicCityStageBSaveHook` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageBSaveHook.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageBSaveHook.cs:12` **EDITOR_BOOTSTRAP** — `[InitializeOnLoad]`

### `Ziptide.Editor.Patching.ToxicCityVehicleSaveHook` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleSaveHook.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleSaveHook.cs:12` **EDITOR_BOOTSTRAP** — `[InitializeOnLoad]`

### `Ziptide.Gameplay.AmbienceDirector` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs:35` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs:41` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.AudioDirector` — 1 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:32` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Gameplay.BootHoldState` — 4 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:158` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:434` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(_xriManager.gameObject);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:444` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(_xriManager.gameObject);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:452` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(_xriManager.gameObject);`

### `Ziptide.Gameplay.ComfortVignette` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:31` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Gameplay.ConquestMissionRuntime` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:39` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Gameplay.DevTools.DevWarp` — 1 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarp.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarp.cs:78` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.DevTools.DevWarpBoard` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:60` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:66` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.EcologyDirector` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:40` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Gameplay.FirstHourDirector` — 3 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:56` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:61` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:69` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Gameplay.FirstHourW001Orchestrator` — 3 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:52` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:57` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:65` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Gameplay.HomeHubAnchorLockInstallerRuntime` — 3 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockInstallerRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockInstallerRuntime.cs:11` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockInstallerRuntime.cs:17` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockInstallerRuntime.cs:22` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.LifecycleState` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:84` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:89` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.PlayerInputSessionGuard` — 2 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:29` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:36` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Gameplay.PlayerMenuRuntime` — 1 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:166` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(_menuRoot);`

### `Ziptide.Gameplay.PvpProgressionRuntime` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:26` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:33` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.QuartersCameraFeature` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:12` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`

### `Ziptide.Gameplay.RepairPartSafetyInstallerRuntime` — 3 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyInstallerRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyInstallerRuntime.cs:12` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyInstallerRuntime.cs:18` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyInstallerRuntime.cs:23` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(go);`

### `Ziptide.Gameplay.SaveSystem` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:50` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:68` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Gameplay.SingletonValidator` — 1 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Diagnostics/SingletonValidator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Diagnostics/SingletonValidator.cs:30` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Gameplay.TravelCoordinator` — 1 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:49` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter` — 3 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:45` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:51` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(go);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:64` **DONT_DESTROY_ON_LOAD** — `DontDestroyOnLoad(gameObject);`

### `Ziptide.Ship.VehicleSafetyRuntime` — 2 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs:35` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs:42` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`

### `Ziptide.Tests.EditMode.DevToolsSingletonTests` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/DevToolsSingletonTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/DevToolsSingletonTests.cs:17` **RUNTIME_BOOTSTRAP** — `private const string BootstrapMarker = "[RuntimeInitializeOnLoadMethod";`

### `Ziptide.Tests.EditMode.EventHygieneTests` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/EventHygieneTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/EventHygieneTests.cs:26` **RUNTIME_BOOTSTRAP** — `"Gameplay/Runtime/World/ConquestMissionRuntime.cs", // [RuntimeInitializeOnLoadMethod] static hook`

### `Ziptide.Tests.EditMode.HeadsetBuildBlockerRegressionTests` — 1 signal(s)

- Codes: `EDITOR_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs:70` **EDITOR_BOOTSTRAP** — `StringAssert.Contains("[InitializeOnLoad]", sync);`

### `Ziptide.Tests.PlayMode.RecoveryGateBypassTests` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGateBypassTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGateBypassTests.cs:61` **RUNTIME_BOOTSTRAP** — `var marker = new Regex(@"(?m)^\s*\[RuntimeInitializeOnLoadMethod(?:\s*\(|\s*\])");`

### `Ziptide.Tests.PlayMode.RecoveryGoldenTravelVisualCapture` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenTravelVisualCapture.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenTravelVisualCapture.cs:36` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`

### `Ziptide.Tests.PlayMode.RecoveryRenderSnapshotTests` — 1 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:166` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(go);`

### `Ziptide.Tests.PlayMode.RecoveryVirtualXrLayoutBootstrap` — 1 signal(s)

- Codes: `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:53` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`

### `ZiptideNet.NetBootstrap` — 2 signal(s)

- Codes: `DONT_DESTROY_ON_LOAD`, `RUNTIME_BOOTSTRAP`
- Paths: `Ziptide/Assets/ZiptideNet/NetBootstrap.cs`
  - `Ziptide/Assets/ZiptideNet/NetBootstrap.cs:20` **RUNTIME_BOOTSTRAP** — `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`
  - `Ziptide/Assets/ZiptideNet/NetBootstrap.cs:42` **DONT_DESTROY_ON_LOAD** — `Object.DontDestroyOnLoad(_launcherGo);`


## Scene loading

### `Ziptide.Gameplay.TravelCoordinator` — 2 signal(s)

- Codes: `DIRECT_SCENE_LOAD`, `DIRECT_SCENE_LOAD_ASYNC`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:122` **DIRECT_SCENE_LOAD** — `SceneManager.LoadScene(sceneName);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:196` **DIRECT_SCENE_LOAD_ASYNC** — `AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);`

### `Ziptide.Tests.EditMode.FirstHourTravelSignalTests` — 3 signal(s)

- Codes: `DIRECT_SCENE_LOAD`, `DIRECT_SCENE_LOAD_ASYNC`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:118` **DIRECT_SCENE_LOAD** — `Assert.AreEqual(1, Count(source, "SceneManager.LoadScene(sceneName);"),`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:121` **DIRECT_SCENE_LOAD_ASYNC** — `"SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);"),`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:134` **DIRECT_SCENE_LOAD_ASYNC** — `int asyncLoad = source.IndexOf("SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);");`

### `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` — 1 signal(s)

- Codes: `DIRECT_SCENE_LOAD_ASYNC`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:64` **DIRECT_SCENE_LOAD_ASYNC** — `AsyncOperation bootLoad = SceneManager.LoadSceneAsync(`

### `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` — 1 signal(s)

- Codes: `DIRECT_SCENE_LOAD_ASYNC`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:67` **DIRECT_SCENE_LOAD_ASYNC** — `AsyncOperation load = SceneManager.LoadSceneAsync(`

### `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` — 1 signal(s)

- Codes: `DIRECT_SCENE_LOAD_ASYNC`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:145` **DIRECT_SCENE_LOAD_ASYNC** — `AsyncOperation load = SceneManager.LoadSceneAsync(`

### `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` — 1 signal(s)

- Codes: `DIRECT_SCENE_LOAD_ASYNC`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:185` **DIRECT_SCENE_LOAD_ASYNC** — `AsyncOperation load = SceneManager.LoadSceneAsync(`


## Input

### `Ziptide.Core.RuntimeInputEnabler` — 8 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:20` **INPUT_ACTION_REFERENCE** — `var assetsEnabled = new HashSet<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:28` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = GetAssetFromController(c);`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:40` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = GetAssetFromInputActionReferences(mb);`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:49` **INPUT_ACTION_REFERENCE** — `Debug.Log($"[Ziptide] RuntimeInputEnabler: enabled {totalAssets} InputActionAsset(s). Controllers={controllersProcessed}, Other={otherProcessed}.");`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:52` **INPUT_ACTION_REFERENCE** — `private static InputActionAsset GetAssetFromController(ActionBasedController c)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:64` **INPUT_ACTION_REFERENCE** — `private static InputActionAsset GetAssetFromInputActionReferences(MonoBehaviour mb)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:71` **INPUT_ACTION_REFERENCE** — `if (f.FieldType != typeof(InputActionReference)) continue;`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs:72` **INPUT_ACTION_REFERENCE** — `var refVal = f.GetValue(mb) as InputActionReference;`

### `Ziptide.Editor.Audit.RigDumpExporter` — 1 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/RigDumpExporter.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/RigDumpExporter.cs:26` **INPUT_ACTION_REFERENCE** — `"Target", "Pistol", "Taser", "CharacterController", "Rigidbody", "InputActionManager",`

### `Ziptide.Editor.Patching.LocomotionContractEnforcer` — 9 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:28` **INPUT_ACTION_REFERENCE** — `InputActionReference turnReference = FindReference("Turn");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:73` **INPUT_ACTION_REFERENCE** — `InputActionReference smoothRef = smoothSo.FindProperty("m_RightHandTurnAction.m_Reference")?.objectReferenceValue`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:74` **INPUT_ACTION_REFERENCE** — `as InputActionReference;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:83` **INPUT_ACTION_REFERENCE** — `InputActionReference snapRef = snapSo.FindProperty("m_RightHandSnapTurnAction.m_Reference")?.objectReferenceValue`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:84` **INPUT_ACTION_REFERENCE** — `as InputActionReference;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:90` **INPUT_ACTION_REFERENCE** — `throw new InvalidOperationException("TURN_CONTRACT: smooth and snap providers share one InputActionReference");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:97` **INPUT_ACTION_REFERENCE** — `private static InputActionReference FindReference(string actionName)`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:104` **INPUT_ACTION_REFERENCE** — `InputActionReference reference = assets[i] as InputActionReference;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/LocomotionContractEnforcer.cs:113` **INPUT_ACTION_REFERENCE** — `InputActionReference reference = assets[i] as InputActionReference;`

### `Ziptide.Editor.Setup.EnsureLocomotionRig` — 13 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:49` **INPUT_ACTION_REFERENCE** — `InputActionAsset inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath);`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:56` **INPUT_ACTION_REFERENCE** — `// InputActionManager in scene`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:82` **INPUT_ACTION_REFERENCE** — `InputActionReference leftMove = FindActionReference(inputActionsPath, "XRI LeftHand Locomotion", "Move");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:83` **INPUT_ACTION_REFERENCE** — `InputActionReference rightMove = FindActionReference(inputActionsPath, "XRI RightHand Locomotion", "Move");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:102` **INPUT_ACTION_REFERENCE** — `InputActionReference rightSnap = FindActionReference(inputActionsPath, "XRI RightHand Locomotion", "Snap Turn");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:114` **INPUT_ACTION_REFERENCE** — `InputActionReference rightTurn = FindActionReference(inputActionsPath, "XRI RightHand Locomotion", "Snap Turn");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:243` **INPUT_ACTION_REFERENCE** — `private static void EnsureInputActionManager(InputActionAsset inputAsset)`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:245` **INPUT_ACTION_REFERENCE** — `var manager = Object.FindObjectOfType<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:252` **INPUT_ACTION_REFERENCE** — `Undo.RegisterCreatedObjectUndo(parent, "InputActionManager");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:255` **INPUT_ACTION_REFERENCE** — `manager = parent.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:256` **INPUT_ACTION_REFERENCE** — `if (manager == null) manager = parent.AddComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:294` **INPUT_ACTION_REFERENCE** — `private static InputActionReference FindActionReference(string assetPath, string mapName, string actionName)`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:299` **INPUT_ACTION_REFERENCE** — `if (o is InputActionReference refAsset && refAsset.action != null)`

### `Ziptide.Editor.Setup.SetupMilestoneAScene` — 5 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:77` **INPUT_ACTION_REFERENCE** — `// InputActionManager: enable XRI Default Input Actions at runtime`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:78` **INPUT_ACTION_REFERENCE** — `var inputManager = managerGo.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:80` **INPUT_ACTION_REFERENCE** — `inputManager = managerGo.AddComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:84` **INPUT_ACTION_REFERENCE** — `var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(xriInputActionsPath);`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:86` **INPUT_ACTION_REFERENCE** — `inputManager.actionAssets = new List<InputActionAsset> { inputAsset };`

### `Ziptide.Editor.Setup.VerifyLocomotionStatus` — 5 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/VerifyLocomotionStatus.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/VerifyLocomotionStatus.cs:10` **INPUT_ACTION_REFERENCE** — `/// Logs locomotion rig status: XR Origin, InputActionManager, Move/Turn providers, CharacterController.`
  - `Ziptide/Assets/Ziptide/Editor/Setup/VerifyLocomotionStatus.cs:32` **INPUT_ACTION_REFERENCE** — `// InputActionManager + action asset`
  - `Ziptide/Assets/Ziptide/Editor/Setup/VerifyLocomotionStatus.cs:33` **INPUT_ACTION_REFERENCE** — `var inputManager = Object.FindObjectOfType<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/VerifyLocomotionStatus.cs:36` **INPUT_ACTION_REFERENCE** — `Debug.Log("[Ziptide Locomotion] InputActionManager: NOT PRESENT.");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/VerifyLocomotionStatus.cs:50` **INPUT_ACTION_REFERENCE** — `Debug.Log("[Ziptide Locomotion] InputActionManager: PRESENT. Action assets: " + count + (hasXri ? ", XRI Default assigned." : ", XRI Default NOT assigned."));`

### `Ziptide.Editor.Validation.XRGrabReadiness` — 7 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:37` **INPUT_ACTION_REFERENCE** — `var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath);`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:40` **INPUT_ACTION_REFERENCE** — `results.Add("FAIL: Asset at " + inputActionsPath + " is not an InputActionAsset.");`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:57` **INPUT_ACTION_REFERENCE** — `// 3) Scene has InputActionManager with input actions assigned`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:58` **INPUT_ACTION_REFERENCE** — `var inputManager = Object.FindObjectOfType<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:61` **INPUT_ACTION_REFERENCE** — `results.Add("WARN: No InputActionManager in scene. RuntimeInputEnabler will enable actions at runtime; or add Input Action Manager and assign XRI Default Input Actions.");`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:68` **INPUT_ACTION_REFERENCE** — `results.Add("FAIL: InputActionManager has no action assets assigned. Assign XRI Default Input Actions.");`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:72` **INPUT_ACTION_REFERENCE** — `results.Add("PASS: InputActionManager has " + count + " action asset(s) assigned.");`

### `Ziptide.Gameplay.BootHoldState` — 24 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:59` **INPUT_ACTION_REFERENCE** — `/// InputActionManager asset list before Destroy — preventing the shared`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:60` **INPUT_ACTION_REFERENCE** — `/// InputActionAsset from being disabled by InputActionManager.OnDisable.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:405` **INPUT_ACTION_REFERENCE** — `// The scene-local manager always has the correct InputActionManager+assets wired up.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:421` **INPUT_ACTION_REFERENCE** — `// CRITICAL FIX: clear the OLD manager's InputActionManager asset list BEFORE`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:422` **INPUT_ACTION_REFERENCE** — `// destroying it. InputActionManager.OnDisable calls actionAsset.Disable() which`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:603` **INPUT_ACTION_REFERENCE** — `var primary = _xriManager != null ? _xriManager.GetComponent<InputActionManager>() : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:628` **INPUT_ACTION_REFERENCE** — `/// Guarantees the persistent rig owns its input. The scene's InputActionManager often`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:633` **INPUT_ACTION_REFERENCE** — `/// the rig references, (2) put them on an InputActionManager on the PERSISTENT manager`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:634` **INPUT_ACTION_REFERENCE** — `/// object, (3) clear the asset list on every other InputActionManager so their OnDisable`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:639` **INPUT_ACTION_REFERENCE** — `var assets = new System.Collections.Generic.List<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:640` **INPUT_ACTION_REFERENCE** — `void Add(InputActionAsset a) { if (a != null && !assets.Contains(a)) assets.Add(a); }`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:642` **INPUT_ACTION_REFERENCE** — `var allManagers = FindObjectsOfType<InputActionManager>(true);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:651` **INPUT_ACTION_REFERENCE** — `var primary = _xriManager.GetComponent<InputActionManager>()`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:652` **INPUT_ACTION_REFERENCE** — `?? _xriManager.gameObject.AddComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:683` **INPUT_ACTION_REFERENCE** — `private static InputActionAsset GetActionAssetFromController(ActionBasedController c)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:692` **INPUT_ACTION_REFERENCE** — `/// <summary>Clears m_ActionAssets on an InputActionManager so its OnDisable won't disable shared assets.</summary>`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:693` **INPUT_ACTION_REFERENCE** — `private static void ClearInputActionAssets(InputActionManager iam)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:698` **INPUT_ACTION_REFERENCE** — `var field = typeof(InputActionManager).GetField(`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:701` **INPUT_ACTION_REFERENCE** — `field.SetValue(iam, new System.Collections.Generic.List<InputActionAsset>());`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:710` **INPUT_ACTION_REFERENCE** — `/// Clears the action asset list on an InputActionManager via reflection BEFORE`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:711` **INPUT_ACTION_REFERENCE** — `/// the manager is destroyed. This prevents InputActionManager.OnDisable from`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:717` **INPUT_ACTION_REFERENCE** — `var iam = mgr.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:721` **INPUT_ACTION_REFERENCE** — `var field = typeof(InputActionManager).GetField(`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:725` **INPUT_ACTION_REFERENCE** — `field.SetValue(iam, new System.Collections.Generic.List<InputActionAsset>());`

### `Ziptide.Gameplay.DashLocomotion` — 9 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`, `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:40` **INPUT_ACTION_REFERENCE** — `private InputAction _jumpAction;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:41` **INPUT_ACTION_REFERENCE** — `private InputAction _sprintAction;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:42` **INPUT_ACTION_REFERENCE** — `private InputAction _crouchAction;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:43` **INPUT_ACTION_REFERENCE** — `private InputAction _rightStickAction;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:111` **INPUT_ACTION_REFERENCE** — `_jumpAction = new InputAction("ZiptideJump", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:112` **INPUT_BUTTON_REFERENCE** — `_jumpAction.AddBinding("<XRController>{RightHand}/primaryButton"); // A`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:116` **INPUT_ACTION_REFERENCE** — `_sprintAction = new InputAction("ZiptideSprint", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:121` **INPUT_ACTION_REFERENCE** — `_crouchAction = new InputAction("ZiptideCrouch", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/DashLocomotion.cs:126` **INPUT_ACTION_REFERENCE** — `_rightStickAction = new InputAction("ZiptideCrouchTurnGuard", InputActionType.Value);`

### `Ziptide.Gameplay.DevTools.DevWarpBoard` — 1 signal(s)

- Codes: `FRAME_BUTTON_POLL`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:93` **FRAME_BUTTON_POLL** — `if (kb != null && kb.f2Key.wasPressedThisFrame) Toggle();`

### `Ziptide.Gameplay.EmergencyRespawn` — 4 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:16` **INPUT_ACTION_REFERENCE** — `private InputAction _leftGrip;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:17` **INPUT_ACTION_REFERENCE** — `private InputAction _rightGrip;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:23` **INPUT_ACTION_REFERENCE** — `_leftGrip = new InputAction("EmergencyLeft", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:27` **INPUT_ACTION_REFERENCE** — `_rightGrip = new InputAction("EmergencyRight", InputActionType.Button);`

### `Ziptide.Gameplay.InputMutationRepairDriver` — 11 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:301` **INPUT_ACTION_REFERENCE** — `var directActions = new HashSet<InputAction>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:313` **INPUT_ACTION_REFERENCE** — `var disabledBefore = new List<InputAction>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:314` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in map.actions)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:322` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in disabledBefore)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:332` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in directActions)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:399` **INPUT_ACTION_REFERENCE** — `private static bool IsInertDirectAction(InputAction action)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:409` **INPUT_ACTION_REFERENCE** — `HashSet<InputAction> directActions)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:435` **INPUT_ACTION_REFERENCE** — `HashSet<InputAction> directActions)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:437` **INPUT_ACTION_REFERENCE** — `InputAction action = property.action;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:520` **INPUT_ACTION_REFERENCE** — `InputAction action = property.action;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:571` **INPUT_ACTION_REFERENCE** — `private static string ActionPath(InputAction action)`

### `Ziptide.Gameplay.PingTool` — 2 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PingTool.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PingTool.cs:19` **INPUT_ACTION_REFERENCE** — `private InputAction _ping;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PingTool.cs:27` **INPUT_ACTION_REFERENCE** — `_ping = new InputAction("ZiptidePing", InputActionType.Button);`

### `Ziptide.Gameplay.PlayerInputSessionGuard` — 17 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:15` **INPUT_ACTION_REFERENCE** — `/// asset to the InputActionManager beside it. Scene-authored InputActionManagers must then be`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:21` **INPUT_ACTION_REFERENCE** — `/// InputActionManager the only enabled manager. A scene-generation latch prevents the retained`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:65` **INPUT_ACTION_REFERENCE** — `/// Transfer the union of all action assets to the InputActionManager attached to the sole`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:78` **INPUT_ACTION_REFERENCE** — `InputActionManager primary = canonicalXri.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:88` **INPUT_ACTION_REFERENCE** — `InputActionManager[] managers = UnityEngine.Object.FindObjectsOfType<InputActionManager>(true);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:89` **INPUT_ACTION_REFERENCE** — `var assets = new List<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:94` **INPUT_ACTION_REFERENCE** — `var fullyDisabledAssets = new List<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:95` **INPUT_ACTION_REFERENCE** — `var intentionallyDisabledActions = new List<InputAction>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:98` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = assets[i];`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:106` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in map.actions)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:114` **INPUT_ACTION_REFERENCE** — `// InputActionManager.OnEnable may enable every assigned asset. Restore the exact disabled`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:118` **INPUT_ACTION_REFERENCE** — `InputAction action = intentionallyDisabledActions[i];`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:125` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = fullyDisabledAssets[i];`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:134` **INPUT_ACTION_REFERENCE** — `InputActionManager manager = managers[i];`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:138` **INPUT_ACTION_REFERENCE** — `manager.actionAssets = new List<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:186` **INPUT_ACTION_REFERENCE** — `private static void AddAssets(InputActionManager manager, List<InputActionAsset> assets)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:189` **INPUT_ACTION_REFERENCE** — `foreach (InputActionAsset asset in manager.actionAssets)`

### `Ziptide.Gameplay.PlayerMenuRuntime` — 3 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`, `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:22` **INPUT_ACTION_REFERENCE** — `private InputAction _toggleAction;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:32` **INPUT_ACTION_REFERENCE** — `_toggleAction = new InputAction("ZiptidePlayerMenu", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:33` **INPUT_BUTTON_REFERENCE** — `_toggleAction.AddBinding("<XRController>{LeftHand}/secondaryButton");`

### `Ziptide.Gameplay.QuickSwap` — 3 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`, `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs:18` **INPUT_ACTION_REFERENCE** — `private InputAction _swap;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs:26` **INPUT_ACTION_REFERENCE** — `_swap = new InputAction("ZiptideQuickSwap", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs:27` **INPUT_BUTTON_REFERENCE** — `_swap.AddBinding("<XRController>{RightHand}/secondaryButton"); // B`

### `Ziptide.Gameplay.TravelCoordinator` — 2 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:18` **INPUT_ACTION_REFERENCE** — `/// and InputActionManager are all ready`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:358` **INPUT_ACTION_REFERENCE** — `var iam = Object.FindObjectOfType<InputActionManager>();`

### `Ziptide.Gameplay.TurnModeCore` — 2 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs:15` **INPUT_ACTION_REFERENCE** — `/// only after its InputAction can safely resolve and read a Vector2. This prevents XRI from calling`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs:47` **INPUT_ACTION_REFERENCE** — `public static bool IsTurnActionReady(InputAction action)`

### `Ziptide.Ship.ShipFlightRuntime` — 17 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`, `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:62` **INPUT_ACTION_REFERENCE** — `private InputAction _leftStick;`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:63` **INPUT_ACTION_REFERENCE** — `private InputAction _rightStick;`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:64` **INPUT_ACTION_REFERENCE** — `private InputAction _boostStickClick; // L3 — same finger as sprint on foot`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:65` **INPUT_ACTION_REFERENCE** — `private InputAction _boostButton; // A — the CONTROLS_AND_FLIGHT boost button`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:66` **INPUT_ACTION_REFERENCE** — `private InputAction _rollLeftButton; // X`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:67` **INPUT_ACTION_REFERENCE** — `private InputAction _rollRightButton; // B`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:68` **INPUT_ACTION_REFERENCE** — `private InputAction _fireAction; // RT — fire ship weapon (CONTROLS_AND_FLIGHT)`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:139` **INPUT_ACTION_REFERENCE** — `_leftStick = new InputAction("ZiptideFlightThrottle", InputActionType.Value);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:141` **INPUT_ACTION_REFERENCE** — `_rightStick = new InputAction("ZiptideFlightSteer", InputActionType.Value);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:143` **INPUT_ACTION_REFERENCE** — `_boostStickClick = new InputAction("ZiptideFlightBoostL3", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:145` **INPUT_ACTION_REFERENCE** — `_boostButton = new InputAction("ZiptideFlightBoostA", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:146` **INPUT_BUTTON_REFERENCE** — `_boostButton.AddBinding("<XRController>{RightHand}/primaryButton"); // A`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:147` **INPUT_ACTION_REFERENCE** — `_rollLeftButton = new InputAction("ZiptideFlightRollL", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:148` **INPUT_BUTTON_REFERENCE** — `_rollLeftButton.AddBinding("<XRController>{LeftHand}/primaryButton"); // X`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:149` **INPUT_ACTION_REFERENCE** — `_rollRightButton = new InputAction("ZiptideFlightRollR", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:150` **INPUT_BUTTON_REFERENCE** — `_rollRightButton.AddBinding("<XRController>{RightHand}/secondaryButton"); // B`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:151` **INPUT_ACTION_REFERENCE** — `_fireAction = new InputAction("ZiptideFlightFire", InputActionType.Button);`

### `Ziptide.Ship.VehicleRuntime` — 8 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`, `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:41` **INPUT_ACTION_REFERENCE** — `private InputAction _leftStick, _rightStick, _boostL3, _boostA, _dismountX;`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:97` **INPUT_ACTION_REFERENCE** — `_leftStick = new InputAction("ZiptideRideThrottle", InputActionType.Value);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:99` **INPUT_ACTION_REFERENCE** — `_rightStick = new InputAction("ZiptideRideSteer", InputActionType.Value);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:101` **INPUT_ACTION_REFERENCE** — `_boostL3 = new InputAction("ZiptideRideBoostL3", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:103` **INPUT_ACTION_REFERENCE** — `_boostA = new InputAction("ZiptideRideBoostA", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:104` **INPUT_BUTTON_REFERENCE** — `_boostA.AddBinding("<XRController>{RightHand}/primaryButton");`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:105` **INPUT_ACTION_REFERENCE** — `_dismountX = new InputAction("ZiptideRideDismount", InputActionType.Button);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:106` **INPUT_BUTTON_REFERENCE** — `_dismountX.AddBinding("<XRController>{LeftHand}/primaryButton");`

### `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` — 1 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:154` **INPUT_ACTION_REFERENCE** — `StringAssert.DoesNotContain("InputAction", socketSource + helperSource);`

### `Ziptide.Tests.EditMode.FirstHourObservationCoreTests` — 1 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs:189` **INPUT_ACTION_REFERENCE** — `StringAssert.DoesNotContain("InputAction", source);`

### `Ziptide.Tests.EditMode.RecoveryPlayabilityDeviceTests` — 1 signal(s)

- Codes: `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/RecoveryPlayabilityDeviceTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RecoveryPlayabilityDeviceTests.cs:21` **INPUT_BUTTON_REFERENCE** — `StringAssert.Contains("<XRController>{LeftHand}/secondaryButton", source);`

### `Ziptide.Tests.EditMode.VehiclePlayabilityTests` — 1 signal(s)

- Codes: `INPUT_BUTTON_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePlayabilityTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePlayabilityTests.cs:43` **INPUT_BUTTON_REFERENCE** — `StringAssert.Contains("<XRController>{LeftHand}/primaryButton", source);`

### `Ziptide.Tests.PlayMode.RecoveryActualRigControllerSimulation` — 15 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:80` **INPUT_ACTION_REFERENCE** — `public readonly InputActionAsset Asset;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:82` **INPUT_ACTION_REFERENCE** — `public InputActionAssetState(InputActionAsset asset)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:90` **INPUT_ACTION_REFERENCE** — `public readonly InputAction Action;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:93` **INPUT_ACTION_REFERENCE** — `public InputActionEnabledState(InputAction action)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:297` **INPUT_ACTION_REFERENCE** — `InputActionManager inputManager = canonicalManager.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:300` **INPUT_ACTION_REFERENCE** — `"The canonical XRInteractionManager has no InputActionManager to refresh.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:302` **INPUT_ACTION_REFERENCE** — `foreach (InputActionAsset asset in inputManager.actionAssets)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:306` **INPUT_ACTION_REFERENCE** — `"The canonical InputActionManager owns no action assets for tracked-rig simulation.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:357` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = _inputAssetStates[i].Asset;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:361` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in map.actions)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:380` **INPUT_ACTION_REFERENCE** — `private static bool IsLocomotionAction(InputAction action)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:421` **INPUT_ACTION_REFERENCE** — `var seen = new HashSet<InputAction>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:424` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = _inputAssetStates[i].Asset;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:428` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in map.actions)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:457` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = _inputAssetStates[i].Asset;`

### `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` — 4 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:83` **INPUT_ACTION_REFERENCE** — `InputActionManager inputManager = null;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:90` **INPUT_ACTION_REFERENCE** — `inputManager = UnityEngine.Object.FindObjectOfType<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:100` **INPUT_ACTION_REFERENCE** — `Assert.IsNotNull(inputManager, "The actual _Boot scene has no InputActionManager.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:114` **INPUT_ACTION_REFERENCE** — `Assert.AreEqual(1, ActiveManagerCount(settled, "InputActionManager"));`

### `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` — 5 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:199` **INPUT_ACTION_REFERENCE** — `InputActionManager inputManager = manager.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:201` **INPUT_ACTION_REFERENCE** — `"The canonical interaction manager lost its InputActionManager after travel.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:207` **INPUT_ACTION_REFERENCE** — `var seen = new HashSet<InputAction>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:208` **INPUT_ACTION_REFERENCE** — `foreach (InputActionAsset asset in inputManager.actionAssets)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:213` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in map.actions)`

### `Ziptide.Tests.PlayMode.RecoveryInputSessionGuardTests` — 19 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:18` **INPUT_ACTION_REFERENCE** — `private readonly List<InputActionAsset> _assets = new List<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:52` **INPUT_ACTION_REFERENCE** — `InputActionManager primary = primaryHost.AddComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:53` **INPUT_ACTION_REFERENCE** — `InputActionAsset primaryAsset = CreateAsset("PrimaryAsset", "PrimaryAction");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:54` **INPUT_ACTION_REFERENCE** — `InputAction primaryHeldOff = primaryAsset.FindActionMap("TestMap")`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:56` **INPUT_ACTION_REFERENCE** — `primary.actionAssets = new List<InputActionAsset> { primaryAsset };`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:61` **INPUT_ACTION_REFERENCE** — `InputActionManager duplicate = duplicateHost.AddComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:62` **INPUT_ACTION_REFERENCE** — `InputActionAsset duplicateAsset = CreateAsset("SceneAsset", "SceneAction");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:63` **INPUT_ACTION_REFERENCE** — `InputAction duplicateHeldOff = duplicateAsset.FindActionMap("TestMap")`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:65` **INPUT_ACTION_REFERENCE** — `duplicate.actionAssets = new List<InputActionAsset> { duplicateAsset };`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:74` **INPUT_ACTION_REFERENCE** — `Assert.IsTrue(primary.enabled, "The canonical InputActionManager was disabled.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:75` **INPUT_ACTION_REFERENCE** — `Assert.IsFalse(duplicate.enabled, "The scene duplicate InputActionManager stayed enabled.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:104` **INPUT_ACTION_REFERENCE** — `InputAction referencedAction = primaryAsset.FindAction("PrimaryAction");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:106` **INPUT_ACTION_REFERENCE** — `InputActionReference referencedActionRef = InputActionReference.Create(referencedAction);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:118` **INPUT_ACTION_REFERENCE** — `var directAction = new InputAction(`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:143` **INPUT_ACTION_REFERENCE** — `if (record.category == "InputActionManager" && record.active && record.enabled)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:147` **INPUT_ACTION_REFERENCE** — `"The runtime census still sees more than one enabled InputActionManager.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:157` **INPUT_ACTION_REFERENCE** — `private InputActionAsset CreateAsset(string assetName, string actionName)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:159` **INPUT_ACTION_REFERENCE** — `InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:173` **INPUT_ACTION_REFERENCE** — `InputActionManager[] input = Resources.FindObjectsOfTypeAll<InputActionManager>();`

### `Ziptide.Tests.PlayMode.RecoveryPlayModeInputEnvironment` — 1 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryPlayModeInputEnvironment.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryPlayModeInputEnvironment.cs:11` **INPUT_ACTION_REFERENCE** — `/// has tracked left/right controllers before _Boot resolves its InputAction assets; Linux CI did not.`

### `Ziptide.Tests.PlayMode.RecoveryRuntimeCensusSnapshot` — 5 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensus.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensus.cs:252` **INPUT_ACTION_REFERENCE** — `InputActionManager input = go.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensus.cs:254` **INPUT_ACTION_REFERENCE** — `snapshot.managers.Add(ComponentRecord("InputActionManager", input, "INPUT_SESSION"));`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensus.cs:351` **INPUT_ACTION_REFERENCE** — `int activeInput = CountActive(snapshot.managers, "InputActionManager");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensus.cs:356` **INPUT_ACTION_REFERENCE** — `AddFinding(snapshot, "DUPLICATE_INPUT_ACTION_MANAGER", "BLOCKER", "InputActionManager", "",`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensus.cs:357` **INPUT_ACTION_REFERENCE** — `"Active InputActionManager count is " + activeInput + "; expected at most one.");`

### `Ziptide.Tests.PlayMode.RecoveryRuntimeCensusTests` — 1 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs:84` **INPUT_ACTION_REFERENCE** — `Assert.AreEqual(1, Count(snapshot.managers, "InputActionManager", true));`

### `Ziptide.Tests.PlayMode.RecoveryTestRig` — 6 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:31` **INPUT_ACTION_REFERENCE** — `public InputActionManager InputManager { get; private set; }`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:32` **INPUT_ACTION_REFERENCE** — `public InputActionAsset ActionAsset { get; private set; }`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:54` **INPUT_ACTION_REFERENCE** — `ActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:62` **INPUT_ACTION_REFERENCE** — `var inputHost = new GameObject("InputActionManager");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:64` **INPUT_ACTION_REFERENCE** — `InputManager = inputHost.AddComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:66` **INPUT_ACTION_REFERENCE** — `InputManager.actionAssets = new List<InputActionAsset> { ActionAsset };`

### `Ziptide.Tests.PlayMode.RecoveryTestRigTests` — 2 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs:54` **INPUT_ACTION_REFERENCE** — `Assert.AreEqual(1, _fixture.Root.GetComponentsInChildren<InputActionManager>(true).Length);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs:56` **INPUT_ACTION_REFERENCE** — `Assert.IsTrue(_fixture.ActionMap.enabled, "InputActionManager did not enable the tests-only action asset.");`

### `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` — 11 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:110` **INPUT_ACTION_REFERENCE** — `Assert.AreEqual(1, ActiveManagerCount(bootCensus, "InputActionManager"));`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:278` **INPUT_ACTION_REFERENCE** — `InputActionManager input = CanonicalInputManager();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:285` **INPUT_ACTION_REFERENCE** — `"The canonical InputActionManager owns no action assets at Home Hub settlement.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:310` **INPUT_ACTION_REFERENCE** — `InputActionManager input = CanonicalInputManager();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:336` **INPUT_ACTION_REFERENCE** — `Assert.AreEqual(1, ActiveManagerCount(census, "InputActionManager"));`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:391` **INPUT_ACTION_REFERENCE** — `private static InputActionManager CanonicalInputManager()`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:394` **INPUT_ACTION_REFERENCE** — `InputActionManager input = manager.GetComponent<InputActionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:396` **INPUT_ACTION_REFERENCE** — `"The canonical XRInteractionManager has no InputActionManager beside it.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:397` **INPUT_ACTION_REFERENCE** — `Assert.IsTrue(input.enabled, "The canonical InputActionManager is disabled.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:401` **INPUT_ACTION_REFERENCE** — `private static int[] InputAssetIds(InputActionManager manager, bool assertEnabled)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:406` **INPUT_ACTION_REFERENCE** — `foreach (InputActionAsset asset in manager.actionAssets)`

### `Ziptide.Tests.PlayMode.RecoveryVirtualXrLayoutBootstrap` — 6 signal(s)

- Codes: `INPUT_ACTION_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:125` **INPUT_ACTION_REFERENCE** — `var seenActions = new HashSet<InputAction>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:126` **INPUT_ACTION_REFERENCE** — `InputActionManager[] managers =`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:127` **INPUT_ACTION_REFERENCE** — `UnityEngine.Object.FindObjectsOfType<InputActionManager>(true);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:130` **INPUT_ACTION_REFERENCE** — `InputActionManager manager = managers[managerIndex];`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:132` **INPUT_ACTION_REFERENCE** — `foreach (InputActionAsset asset in manager.actionAssets)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:137` **INPUT_ACTION_REFERENCE** — `foreach (InputAction action in map.actions)`


## Runtime surfaces

### `Ziptide.Core.DebugHUD` — 3 signal(s)

- Codes: `CANVAS_COMPONENT`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs:28` **NEW_GAME_OBJECT** — `s_Root = new GameObject("Ziptide_DebugHUD");`
  - `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs:31` **CANVAS_COMPONENT** — `var canvas = s_Root.AddComponent<Canvas>();`
  - `Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs:37` **NEW_GAME_OBJECT** — `var go = new GameObject("Text");`

### `Ziptide.Core.GamePool` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/GamePool.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/GamePool.cs:21` **CREATE_PRIMITIVE** — `/// var go = GameObject.CreatePrimitive(...); ... Destroy(go, life);`
  - `Ziptide/Assets/Ziptide/Core/Runtime/GamePool.cs:39` **NEW_GAME_OBJECT** — `var go = new GameObject("__GAMEPOOL");`
  - `Ziptide/Assets/Ziptide/Core/Runtime/GamePool.cs:130` **NEW_GAME_OBJECT** — `var go = new GameObject("__GAMEPOOL_TICKER");`

### `Ziptide.Core.RuntimeHealthMonitor` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs:38` **NEW_GAME_OBJECT** — `var go = new GameObject("__RuntimeHealth");`

### `Ziptide.Core.RuntimeMaterialFixer` — 2 signal(s)

- Codes: `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:19` **SHADER_FIND** — `Shader urpLit = Shader.Find(URPShaderName);`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:51` **RUNTIME_MATERIAL_CREATE** — `Material fallback = new Material(urpLit);`

### `Ziptide.Editor.Art.BuildingKitLibrary` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:62` **NEW_GAME_OBJECT** — `var root = new GameObject("Kit_" + styleId + (windowReveal ? "_WallWindow" : "_WallSolid"));`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:103` **CREATE_PRIMITIVE** — `var pipe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:122` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:131` **SHADER_FIND** — `var interior = Shader.Find("Ziptide/InteriorMapping");`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:134` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(interior);`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:146` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.Art.CavernKitLibrary` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:37` **NEW_GAME_OBJECT** — `var root = new GameObject("Kit_Cavern_FloorPad");`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:39` **CREATE_PRIMITIVE** — `var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:57` **CREATE_PRIMITIVE** — `var stone = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:73` **NEW_GAME_OBJECT** — `var root = new GameObject("Kit_Cavern_Stalactite");`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:75` **CREATE_PRIMITIVE** — `var spike = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:84` **CREATE_PRIMITIVE** — `var tip = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:101` **NEW_GAME_OBJECT** — `var root = new GameObject("Kit_Cavern_ShaftWall");`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:103` **CREATE_PRIMITIVE** — `var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Art/CavernKitLibrary.cs:112` **CREATE_PRIMITIVE** — `var ledge = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.FirstHourSurfaceAuthor` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourSurfaceAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourSurfaceAuthor.cs:79` **NEW_GAME_OBJECT** — `var marker = new GameObject(markerName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourSurfaceAuthor.cs:97` **CREATE_PRIMITIVE** — `var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.Patching.AmbientMoteAuthor` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/AmbientMoteAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/AmbientMoteAuthor.cs:94` **NEW_GAME_OBJECT** — `var root = new GameObject(RootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/AmbientMoteAuthor.cs:102` **NEW_GAME_OBJECT** — `var marker = new GameObject("AmbientMote_" + i + "_" + recipeId);`

### `Ziptide.Editor.Patching.BuildingBuilder` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:37` **NEW_GAME_OBJECT** — `var root = new GameObject("__BUILDINGS_" + district.id);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:106` **NEW_GAME_OBJECT** — `var bRoot = new GameObject("Building_" + lot.Bounds.x.ToString("F0") + "_" + lot.Bounds.y.ToString("F0"));`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:198` **NEW_GAME_OBJECT** — `var frame = new GameObject("DoorFrame");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:210` **NEW_GAME_OBJECT** — `var marker = new GameObject("__DOOR");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:218` **CREATE_PRIMITIVE** — `var pane = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:230` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.Patching.CaveSpawnSafety` — 1 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/CaveSpawnSafety.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CaveSpawnSafety.cs:39` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.Patching.CityBuilder` — 11 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:69` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:70` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:71` **RUNTIME_MATERIAL_CREATE** — `m = new Material(shader) { name = "CityMat_" + ColorUtility.ToHtmlStringRGB(c) };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:80` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:223` **CREATE_PRIMITIVE** — `var w = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:266` **NEW_GAME_OBJECT** — `var marker = new GameObject("Marker_" + hb.interiorMarkerId);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:347` **CREATE_PRIMITIVE** — `var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:438` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:483` **NEW_GAME_OBJECT** — `var go = new GameObject("Hazard_" + h.id);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:512` **CREATE_PRIMITIVE** — `var drone = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:536` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Editor.Patching.CityStageAPrimitiveFactory` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/CityStageAPrimitiveFactory.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityStageAPrimitiveFactory.cs:23` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityStageAPrimitiveFactory.cs:70` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityStageAPrimitiveFactory.cs:71` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityStageAPrimitiveFactory.cs:72` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader) { name = "CityStageA_" + slot };`

### `Ziptide.Editor.Patching.ForgeBaker` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ForgeBaker.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeBaker.cs:164` **NEW_GAME_OBJECT** — `var go = new GameObject("ForgeBaked_" + recipe.recipeId);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeBaker.cs:211` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeBaker.cs:215` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader);`

### `Ziptide.Editor.Patching.ForgePhotoBooth` — 25 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:87` **NEW_GAME_OBJECT** — `var root = new GameObject("Forge_" + spec.Key);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:110` **NEW_GAME_OBJECT** — `var root = new GameObject("GroundDecals");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:111` **CREATE_PRIMITIVE** — `var ground = GameObject.CreatePrimitive(PrimitiveType.Quad);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:117` **RUNTIME_MATERIAL_CREATE** — `var gm = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothGround" };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:117` **SHADER_FIND** — `var gm = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothGround" };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:129` **CREATE_PRIMITIVE** — `var q = GameObject.CreatePrimitive(PrimitiveType.Quad);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:142` **RUNTIME_MATERIAL_CREATE** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothDecal_" + name };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:142` **SHADER_FIND** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothDecal_" + name };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:168` **NEW_GAME_OBJECT** — `var root = new GameObject("ZiptideWater");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:181` **RUNTIME_MATERIAL_CREATE** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothWater" };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:181` **SHADER_FIND** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothWater" };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:191` **NEW_GAME_OBJECT** — `var foam = new GameObject("Foam");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:206` **RUNTIME_MATERIAL_CREATE** — `var fm = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothWaterFoam" };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:206` **SHADER_FIND** — `var fm = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothWaterFoam" };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:223` **NEW_GAME_OBJECT** — `var root = new GameObject("ForgeBody_" + id);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:238` **RUNTIME_MATERIAL_CREATE** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:238` **SHADER_FIND** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:283` **NEW_GAME_OBJECT** — `var root = new GameObject("ForgeCalibration");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:287` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(type);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:293` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:293` **SHADER_FIND** — `var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:356` **RUNTIME_MATERIAL_CREATE** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothSingle_" + recipe.recipeId };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:356` **SHADER_FIND** — `var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothSingle_" + recipe.recipeId };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:411` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:424` **NEW_GAME_OBJECT** — `var camGo = new GameObject("Booth_Camera");`

### `Ziptide.Editor.Patching.InteriorBuilder` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/InteriorBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorBuilder.cs:77` **NEW_GAME_OBJECT** — `var root = new GameObject("Interior");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorBuilder.cs:88` **CREATE_PRIMITIVE** — `var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorBuilder.cs:109` **CREATE_PRIMITIVE** — `var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorBuilder.cs:137` **NEW_GAME_OBJECT** — `var cacheGo = new GameObject("SalvageCache");`

### `Ziptide.Editor.Patching.InteriorFurnisher` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/InteriorFurnisher.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorFurnisher.cs:37` **NEW_GAME_OBJECT** — `var node = new GameObject("Room_" + i);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorFurnisher.cs:45` **NEW_GAME_OBJECT** — `var go = new GameObject(item.Kind);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/InteriorFurnisher.cs:202` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(type);`

### `Ziptide.Editor.Patching.PatcherUtil` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/PatcherUtil.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PatcherUtil.cs:49` **NEW_GAME_OBJECT** — `go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PatcherUtil.cs:64` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Editor.Patching.PracticalAuthor` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:28` **NEW_GAME_OBJECT** — `var root = new GameObject("Practicals").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:105` **NEW_GAME_OBJECT** — `var fixtureLift = new GameObject("LanternFixture");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:141` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:144` **CREATE_PRIMITIVE** — `var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:175` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:176` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:177` **RUNTIME_MATERIAL_CREATE** — `_fixtureMat = new Material(shader) { name = "PracticalFixture_Iron" };`

### `Ziptide.Editor.Patching.ReactivePropAuthor` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ReactivePropAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ReactivePropAuthor.cs:149` **NEW_GAME_OBJECT** — `var proxyObject = new GameObject(HitProxyName);`

### `Ziptide.Editor.Patching.ScenePatcherArena` — 11 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:149` **NEW_GAME_OBJECT** — `var nav = new GameObject("__PVP_BOTNAV");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:164` **NEW_GAME_OBJECT** — `var group = new GameObject("__PVP_ZONES");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:169` **NEW_GAME_OBJECT** — `var go = new GameObject("Zone_" + z.zoneId);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:193` **CREATE_PRIMITIVE** — `var bot = GameObject.CreatePrimitive(PrimitiveType.Capsule);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:221` **NEW_GAME_OBJECT** — `var go = new GameObject(w.wallName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:232` **NEW_GAME_OBJECT** — `var go = new GameObject("PvpHammer");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:247` **NEW_GAME_OBJECT** — `var hazardRoot = new GameObject("Hazards");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:252` **NEW_GAME_OBJECT** — `var go = new GameObject("Hazard_" + h.id);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:399` **NEW_GAME_OBJECT** — `return new GameObject(name).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:404` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs:417` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Editor.Patching.ScenePatcherBoot` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherBoot.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherBoot.cs:94` **NEW_GAME_OBJECT** — `var go = new GameObject("BootLoader");`

### `Ziptide.Editor.Patching.ScenePatcherC0` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:68` **NEW_GAME_OBJECT** — `GameObject beltGo = new GameObject("BeltRig");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:132` **NEW_GAME_OBJECT** — `triggerGo = new GameObject(triggerName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:161` **NEW_GAME_OBJECT** — `var hipGo = new GameObject(socketName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:177` **NEW_GAME_OBJECT** — `var attachGo = new GameObject("Attach");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:215` **CREATE_PRIMITIVE** — `GameObject pistol = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:226` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:248` **NEW_GAME_OBJECT** — `var muzzle = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:285` **CREATE_PRIMITIVE** — `GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:290` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs:293` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Editor.Patching.ScenePatcherCavern` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs:153` **NEW_GAME_OBJECT** — `var link = new GameObject(linkName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs:187` **CREATE_PRIMITIVE** — `var span = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs:218` **NEW_GAME_OBJECT** — `var lift = new GameObject("ShaftLift");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs:238` **NEW_GAME_OBJECT** — `var anchor = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs:254` **NEW_GAME_OBJECT** — `var zip = new GameObject("CaveZipline");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs:290` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Plane);`

### `Ziptide.Editor.Patching.ScenePatcherD0` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:94` **CREATE_PRIMITIVE** — `var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:294` **NEW_GAME_OBJECT** — `triggerGo = new GameObject(triggerName);`

### `Ziptide.Editor.Patching.ScenePatcherD1` — 15 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:141` **NEW_GAME_OBJECT** — `var root = new GameObject(CityRoot);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:168` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:169` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:182` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader) { name = name };`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:203` **CREATE_PRIMITIVE** — `var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:387` **RUNTIME_MATERIAL_CREATE** — `new Material(_concreteMat) { name = "PlanterMat", color = new Color(0.35f, 0.28f, 0.20f) });`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:617` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:623` **CREATE_PRIMITIVE** — `var finL = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:629` **CREATE_PRIMITIVE** — `var finR = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:657` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:672` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:691` **NEW_GAME_OBJECT** — `var muzzle = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:702` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:705` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs:740` **CREATE_PRIMITIVE** — `var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.Patching.ScenePatcherPvP` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:116` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:126` **NEW_GAME_OBJECT** — `var go = new GameObject("PvpHammer");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:176` **NEW_GAME_OBJECT** — `var nav = new GameObject("__PVP_BOTNAV");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:203` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:210` **CREATE_PRIMITIVE** — `var bot = GameObject.CreatePrimitive(PrimitiveType.Capsule);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:234` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherPvP.cs:257` **NEW_GAME_OBJECT** — `return new GameObject(name).transform;`

### `Ziptide.Editor.Patching.ScenePatcherSandbox` — 16 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:158` **CREATE_PRIMITIVE** — `var drone = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:184` **NEW_GAME_OBJECT** — `var floor = new GameObject("SandboxBeltFloor");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:209` **NEW_GAME_OBJECT** — `var mine = new GameObject("SandboxBeltMine");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:222` **NEW_GAME_OBJECT** — `var disp = new GameObject("SandboxBeltDispenser");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:236` **NEW_GAME_OBJECT** — `var stand = new GameObject("SandboxBeltWandStand");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:247` **NEW_GAME_OBJECT** — `var go = new GameObject("SandboxBeltConductor");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:261` **CREATE_PRIMITIVE** — `var tower = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:268` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:271` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:281` **NEW_GAME_OBJECT** — `var zip = new GameObject("SandboxZipline");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:291` **NEW_GAME_OBJECT** — `var lift = new GameObject("SandboxLift");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:299` **NEW_GAME_OBJECT** — `var pad = new GameObject("SandboxJumpPad");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:320` **NEW_GAME_OBJECT** — `var table = new GameObject("SandboxWarTable");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:330` **NEW_GAME_OBJECT** — `var anchor = new GameObject("SandboxGrapple");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:382` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Plane);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSandbox.cs:419` **CREATE_PRIMITIVE** — `post = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Editor.Patching.ScenePatcherSpaceLane` — 15 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:66` **NEW_GAME_OBJECT** — `var root = new GameObject("__SPACELANE_ROOT").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:96` **NEW_GAME_OBJECT** — `var frame = new GameObject("CockpitFrame").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:118` **NEW_GAME_OBJECT** — `var lane = new GameObject("LaneContent").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:129` **NEW_GAME_OBJECT** — `var rocks = new GameObject("DriftRocks").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:138` **CREATE_PRIMITIVE** — `var rock = GameObject.CreatePrimitive(rng.Next(2) == 0 ? PrimitiveType.Cube : PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:153` **NEW_GAME_OBJECT** — `var drone = new GameObject("Drone_" + index);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:157` **CREATE_PRIMITIVE** — `var hull = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:166` **CREATE_PRIMITIVE** — `var wing = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:175` **CREATE_PRIMITIVE** — `var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:188` **NEW_GAME_OBJECT** — `var ring = new GameObject("Ring_" + index).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:195` **CREATE_PRIMITIVE** — `var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:292` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:305` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:306` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherSpaceLane.cs:308` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Editor.Patching.ScenePatcherStarterWorld` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:217` **CREATE_PRIMITIVE** — `var drone = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:253` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:280` **CREATE_PRIMITIVE** — `if (go == null) { go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; }`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:324` **NEW_GAME_OBJECT** — `GameObject go = t != null ? t.gameObject : new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:334` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(type);`

### `Ziptide.Editor.Patching.ScenePatcherToxicCity` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs:196` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Editor.Patching.ShipHullBuilder` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:225` **CREATE_PRIMITIVE** — `GameObject go = GameObject.CreatePrimitive(primitive);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:261` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:262` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:263` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader) { name = "HeroShip_" + slot };`

### `Ziptide.Editor.Patching.SignAuthor` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:99` **NEW_GAME_OBJECT** — `var root = new GameObject(RootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:120` **NEW_GAME_OBJECT** — `var holder = new GameObject(string.IsNullOrEmpty(plan.name) ? "ShellSign" : plan.name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:130` **CREATE_PRIMITIVE** — `var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:151` **CREATE_PRIMITIVE** — `var glyph = GameObject.CreatePrimitive(PrimitiveType.Quad);`

### `Ziptide.Editor.Patching.SignRecipeLibrary` — 4 signal(s)

- Codes: `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/SignRecipeLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignRecipeLibrary.cs:150` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignRecipeLibrary.cs:151` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Unlit/Texture");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignRecipeLibrary.cs:152` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignRecipeLibrary.cs:159` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader)`

### `Ziptide.Editor.Patching.ToxicCityRiverBuilder` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:47` **NEW_GAME_OBJECT** — `var root = new GameObject(RootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:78` **NEW_GAME_OBJECT** — `var river = new GameObject(RiverPrefix + index).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:107` **NEW_GAME_OBJECT** — `Transform motion = new GameObject("SurfaceMotion").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:147` **NEW_GAME_OBJECT** — `Transform root = new GameObject(name).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:155` **NEW_GAME_OBJECT** — `var label = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:159` **TEXTMESH_COMPONENT** — `var text = label.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:177` **CREATE_PRIMITIVE** — `GameObject go = GameObject.CreatePrimitive(type);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:199` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:200` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverBuilder.cs:201` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader) { name = "ToxicRiver_" + slot };`

### `Ziptide.Editor.Patching.ToxicCityStageA` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageA.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageA.cs:98` **NEW_GAME_OBJECT** — `var marker = new GameObject(MarkerName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageA.cs:121` **NEW_GAME_OBJECT** — `var root = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageA.cs:229` **NEW_GAME_OBJECT** — `var root = new GameObject("__CITY_STAGE_A_CURBS");`

### `Ziptide.Editor.Patching.ToxicCityStageB` — 11 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:47` **NEW_GAME_OBJECT** — `Transform root = new GameObject(RootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:73` **NEW_GAME_OBJECT** — `Transform street = new GameObject(DistrictRootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:123` **NEW_GAME_OBJECT** — `Transform root = new GameObject("StreetLamp_" + index).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:139` **NEW_GAME_OBJECT** — `Transform root = new GameObject("DistrictSign").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:146` **NEW_GAME_OBJECT** — `GameObject label = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:150` **TEXTMESH_COMPONENT** — `TextMesh text = label.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:162` **NEW_GAME_OBJECT** — `Transform root = new GameObject("Vent_" + index).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:209` **CREATE_PRIMITIVE** — `GameObject go = GameObject.CreatePrimitive(primitive);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:231` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:232` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageB.cs:233` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader) { name = "CityStageB_" + slot };`

### `Ziptide.Editor.Patching.ToxicCityVehicleBuilder` — 11 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:39` **NEW_GAME_OBJECT** — `Transform root = new GameObject(RootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:66` **NEW_GAME_OBJECT** — `Transform bay = new GameObject("VehicleBay_" + id).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:70` **CREATE_PRIMITIVE** — `GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:85` **NEW_GAME_OBJECT** — `GameObject vehicle = new GameObject("Vehicle_" + id);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:92` **NEW_GAME_OBJECT** — `GameObject label = new GameObject("BayLabel");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:95` **TEXTMESH_COMPONENT** — `TextMesh text = label.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:117` **NEW_GAME_OBJECT** — `Transform preview = new GameObject(PreviewRootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:163` **CREATE_PRIMITIVE** — `GameObject part = GameObject.CreatePrimitive(primitive);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:189` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:190` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleBuilder.cs:191` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader) { name = "VehiclePad_" + ColorUtility.ToHtmlStringRGB(color) };`

### `Ziptide.Editor.Patching.WaterAuthor` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WaterAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WaterAuthor.cs:23` **NEW_GAME_OBJECT** — `var go = new GameObject("BerthWater");`

### `Ziptide.Editor.Patching.WorldDressingBuilder` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:37` **NEW_GAME_OBJECT** — `var dressRoot = new GameObject("Dressing").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:94` **NEW_GAME_OBJECT** — `var cairnRoot = new GameObject("Route").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:113` **NEW_GAME_OBJECT** — `var cairn = new GameObject("Cairn_" + n).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:128` **NEW_GAME_OBJECT** — `var scatterRoot = new GameObject("Scatter").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:159` **NEW_GAME_OBJECT** — `var cluster = new GameObject("Prop_" + n).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:226` **NEW_GAME_OBJECT** — `var holder = new GameObject("TuftPlant" + i);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:296` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:309` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:310` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:311` **RUNTIME_MATERIAL_CREATE** — `m = new Material(shader) { name = "DressMat_" + ColorUtility.ToHtmlStringRGB(color) };`

### `Ziptide.Editor.Patching.WorldExperienceBuilder` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:262` **NEW_GAME_OBJECT** — `var go = new GameObject("ExperienceTerrain");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:305` **NEW_GAME_OBJECT** — `var vistaRoot = new GameObject("ArrivalVista").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:325` **NEW_GAME_OBJECT** — `var hero = new GameObject("Hero_" + ex.vista).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:389` **NEW_GAME_OBJECT** — `var arch = new GameObject("Arch" + i).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:406` **NEW_GAME_OBJECT** — `var cluster = new GameObject("Midground_" + salt).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:428` **NEW_GAME_OBJECT** — `var ring = new GameObject("Ring").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:445` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:464` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:465` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:466` **RUNTIME_MATERIAL_CREATE** — `var m = new Material(shader) { name = name };`

### `Ziptide.Editor.Patching.WorldPoiBuilder` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:43` **NEW_GAME_OBJECT** — `var poisRoot = new GameObject("Pois").transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:66` **NEW_GAME_OBJECT** — `var poiRoot = new GameObject("__POI_" + poi.id).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:95` **NEW_GAME_OBJECT** — `var go = new GameObject("__SPAWN_poi_" + poiId);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:229` **NEW_GAME_OBJECT** — `var stall = new GameObject("Stall" + i).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:291` **NEW_GAME_OBJECT** — `var ride = new GameObject("Vehicle_" + poi.id);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:355` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:380` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:381` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldPoiBuilder.cs:382` **RUNTIME_MATERIAL_CREATE** — `var m = new Material(shader) { name = "PoiMat_" + ColorUtility.ToHtmlStringRGB(color) };`

### `Ziptide.Editor.Patching.WorldStubGenerator` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:124` **NEW_GAME_OBJECT** — `var root = new GameObject(rootName).transform;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:198` **NEW_GAME_OBJECT** — `var mouth = new GameObject(MouthName);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:206` **CREATE_PRIMITIVE** — `var j = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:214` **CREATE_PRIMITIVE** — `var lintel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:221` **NEW_GAME_OBJECT** — `var label = new GameObject("MouthLabel");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:224` **TEXTMESH_COMPONENT** — `var tm = label.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:230` **NEW_GAME_OBJECT** — `var doorway = new GameObject("MouthTrigger");`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:280` **NEW_GAME_OBJECT** — `var zipGo = new GameObject(ZipName);`

### `Ziptide.Editor.Setup.ApplyThemeToCurrentScene` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/ApplyThemeToCurrentScene.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/ApplyThemeToCurrentScene.cs:31` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("WorldDirector");`

### `Ziptide.Editor.Setup.ApplyWorldProfileToCurrentScene` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/ApplyWorldProfileToCurrentScene.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/ApplyWorldProfileToCurrentScene.cs:32` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("WorldRuntime");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/ApplyWorldProfileToCurrentScene.cs:53` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("ThemeSwitchStation");`

### `Ziptide.Editor.Setup.EnsureLocomotionRig` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:249` **NEW_GAME_OBJECT** — `GameObject parent = interactionManager != null ? interactionManager.gameObject : new GameObject("_InputActionManager");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:274` **NEW_GAME_OBJECT** — `GameObject child = new GameObject(name);`

### `Ziptide.Editor.Setup.SetupMilestoneAScene` — 11 signal(s)

- Codes: `CREATE_PRIMITIVE`, `EVENT_SYSTEM_COMPONENT`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`, `XR_UI_INPUT_MODULE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:64` **NEW_GAME_OBJECT** — `xrOrigin = new GameObject("XR Origin (add via GameObject > XR > XR Origin (VR))");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:71` **NEW_GAME_OBJECT** — `managerGo = new GameObject("XR Interaction Manager");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:96` **NEW_GAME_OBJECT** — `eventSystemGo = new GameObject("EventSystem");`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:97` **EVENT_SYSTEM_COMPONENT** — `eventSystemGo.AddComponent<EventSystem>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:100` **XR_UI_INPUT_MODULE** — `eventSystemGo.AddComponent<XRUIInputModule>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:104` **CREATE_PRIMITIVE** — `var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:116` **CREATE_PRIMITIVE** — `var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:123` **XR_INTERACTABLE_COMPONENT** — `var grab = cube.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:207` **SHADER_FIND** — `Shader shader = Shader.Find(urpLitShaderName);`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:215` **RUNTIME_MATERIAL_CREATE** — `mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:259` **NEW_GAME_OBJECT** — `var go = new GameObject("WorldDirector");`

### `Ziptide.Editor.WorldImprovement.ArrivalIdentityModule` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Editor/WorldImprovement/StandardWorldImprovementModules.cs`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/StandardWorldImprovementModules.cs:41` **NEW_GAME_OBJECT** — `var labelObject = new GameObject("IdentityLabel");`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/StandardWorldImprovementModules.cs:44` **TEXTMESH_COMPONENT** — `var label = labelObject.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/StandardWorldImprovementModules.cs:83` **NEW_GAME_OBJECT** — `var beacon = new GameObject("RouteBeacon_" + i).transform;`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/StandardWorldImprovementModules.cs:158` **NEW_GAME_OBJECT** — `var tower = new GameObject("HorizonLandmark_" + i).transform;`

### `Ziptide.Editor.WorldImprovement.GroundedRouteModule` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:141` **NEW_GAME_OBJECT** — `Transform node = new GameObject("DiscoveryNode_" + index).transform;`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:152` **CREATE_PRIMITIVE** — `GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:173` **NEW_GAME_OBJECT** — `GameObject labelObject = new GameObject("DiscoveryStatus");`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:177` **TEXTMESH_COMPONENT** — `TextMesh label = labelObject.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:215` **NEW_GAME_OBJECT** — `Transform trace = new GameObject("StoryTrace_" + i).transform;`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:237` **NEW_GAME_OBJECT** — `Transform trace = new GameObject("StoryTrace_Fallback").transform;`

### `Ziptide.Editor.WorldImprovement.WorldImprovementCompiler` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementCompiler.cs`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementCompiler.cs:89` **NEW_GAME_OBJECT** — `var rootObject = new GameObject(RootName);`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementCompiler.cs:107` **NEW_GAME_OBJECT** — `var moduleRoot = new GameObject("__WIM_" + safeName).transform;`

### `Ziptide.Editor.WorldImprovement.WorldImprovementModuleResult` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementModules.cs`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementModules.cs:47` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementModules.cs:48` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementModules.cs:49` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementModules.cs:58` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader) { name = "WIM_" + hex };`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementModules.cs:87` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(primitive);`

### `Ziptide.Gameplay.AmbienceDirector` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs:40` **NEW_GAME_OBJECT** — `var go = new GameObject("__AmbienceDirector");`

### `Ziptide.Gameplay.ArenaLobbyBoard` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:69` **CREATE_PRIMITIVE** — `var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:89` **CREATE_PRIMITIVE** — `var rulesPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:289` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:296` **XR_INTERACTABLE_COMPONENT** — `var interactable = go.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:332` **NEW_GAME_OBJECT** — `var go = new GameObject("Label_" + text.Replace(' ', '_'));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:336` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.AugmentController` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs:41` **NEW_GAME_OBJECT** — `var go = new GameObject("__AugmentController");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs:99` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs:105` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs:173` **CREATE_PRIMITIVE** — `_bubble = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs:183` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/AugmentController.cs:186` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.AugmentPickupRuntime` — 1 signal(s)

- Codes: `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/AugmentPickupRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/AugmentPickupRuntime.cs:13` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRSimpleInteractable))]`

### `Ziptide.Gameplay.BeltBlueprintWandItem` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:27` **NEW_GAME_OBJECT** — `var go = new GameObject("BeltBlueprintWand");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:37` **CREATE_PRIMITIVE** — `var shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:44` **CREATE_PRIMITIVE** — `var guard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:52` **CREATE_PRIMITIVE** — `var head = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:62` **NEW_GAME_OBJECT** — `var labelGo = new GameObject("WandCount");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:63` **TEXTMESH_COMPONENT** — `_countLabel = labelGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:79` **XR_INTERACTABLE_COMPONENT** — `_grab = gameObject.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:173` **CREATE_PRIMITIVE** — `var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltBlueprintWandItem.cs:179` **CREATE_PRIMITIVE** — `var cup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`

### `Ziptide.Gameplay.BeltCellSpec` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:227` **NEW_GAME_OBJECT** — `var cellRoot = new GameObject("Cell_" + c.x + "_" + c.z);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:232` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:248` **CREATE_PRIMITIVE** — `var chev = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:320` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(prim);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:442` **NEW_GAME_OBJECT** — `_bpGhostRoot = new GameObject("BlueprintGhost");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:447` **CREATE_PRIMITIVE** — `var quad = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:486` **NEW_GAME_OBJECT** — `_ghost = new GameObject("BeltGhost");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:488` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:494` **CREATE_PRIMITIVE** — `var chev = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:584` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(prim);`

### `Ziptide.Gameplay.BeltConductorRuntime` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:41` **CREATE_PRIMITIVE** — `var post = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:51` **CREATE_PRIMITIVE** — `var handleGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:61` **CREATE_PRIMITIVE** — `var core = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:68` **CREATE_PRIMITIVE** — `var cap = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:76` **CREATE_PRIMITIVE** — `var loop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:88` **XR_INTERACTABLE_COMPONENT** — `var grab = handleGo.AddComponent<XRSimpleInteractable>();`

### `Ziptide.Gameplay.BeltDispenserRuntime` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltDispenserRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltDispenserRuntime.cs:23` **CREATE_PRIMITIVE** — `var post = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltDispenserRuntime.cs:29` **CREATE_PRIMITIVE** — `var band = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.BeltMinePortRuntime` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:80` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:87` **CREATE_PRIMITIVE** — `var drill = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:99` **CREATE_PRIMITIVE** — `var piston = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:109` **NEW_GAME_OBJECT** — `var readoutGo = new GameObject("PortReadout");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:110` **TEXTMESH_COMPONENT** — `_readout = readoutGo.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.BeltPadSpawner` — 6 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:24` **NEW_GAME_OBJECT** — `var root = new GameObject("BeltPads");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:37` **NEW_GAME_OBJECT** — `var go = new GameObject("BeltPad_" + def.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:51` **NEW_GAME_OBJECT** — `var intakeGo = new GameObject("BeltPadIntake_" + def.feedMineId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:75` **NEW_GAME_OBJECT** — `var disp = new GameObject("BeltPadDispenser_" + def.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:80` **NEW_GAME_OBJECT** — `var stand = new GameObject("BeltPadWandStand_" + def.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltPadSpawner.cs:87` **NEW_GAME_OBJECT** — `var cond = new GameObject("BeltPadConductor_" + def.id);`

### `Ziptide.Gameplay.BeltRig` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:81` **NEW_GAME_OBJECT** — `GameObject go = new GameObject(holsterName);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:99` **NEW_GAME_OBJECT** — `GameObject attachGo = new GameObject("HolsterAttach");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:109` **CREATE_PRIMITIVE** — `GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:125` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:126` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:127` **RUNTIME_MATERIAL_CREATE** — `_holsterMat = new Material(shader) { name = "HolsterMarker_Mat" };`

### `Ziptide.Gameplay.BeltTileItem` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltTileItem.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltTileItem.cs:29` **NEW_GAME_OBJECT** — `var go = new GameObject("BeltTileItem");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltTileItem.cs:40` **CREATE_PRIMITIVE** — `var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltTileItem.cs:46` **CREATE_PRIMITIVE** — `var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltTileItem.cs:56` **CREATE_PRIMITIVE** — `var fork = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltTileItem.cs:73` **XR_INTERACTABLE_COMPONENT** — `_grab = gameObject.AddComponent<XRGrabInteractable>();`

### `Ziptide.Gameplay.BootHoldState` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:324` **NEW_GAME_OBJECT** — `var go = new GameObject(Ziptide.Core.ZiptideConstants.GoBeltRig);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:449` **NEW_GAME_OBJECT** — `var go = new GameObject("__XRI");`

### `Ziptide.Gameplay.BootLoader` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/BootLoader.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/BootLoader.cs:34` **NEW_GAME_OBJECT** — `: new GameObject("__HOME_HUB_RUNTIME").AddComponent<HomeHubRuntime>();`

### `Ziptide.Gameplay.BreakableWall` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:107` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:115` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:116` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:119` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:172` **CREATE_PRIMITIVE** — `var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:183` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:184` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/BreakableWall.cs:187` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.BuildSocketRuntime` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:53` **CREATE_PRIMITIVE** — `var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:59` **XR_INTERACTABLE_COMPONENT** — `var interactable = frame.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:65` **CREATE_PRIMITIVE** — `_holo = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:74` **NEW_GAME_OBJECT** — `var readoutGo = new GameObject("Readout");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:75` **TEXTMESH_COMPONENT** — `_readout = readoutGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:113` **NEW_GAME_OBJECT** — `var rigGo = new GameObject("BuiltRig_" + _def.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:157` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:158` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:160` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.CameraRuntime` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/CameraRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/CameraRuntime.cs:65` **NEW_GAME_OBJECT** — `var lensGo = new GameObject("Lens");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/CameraRuntime.cs:70` **CREATE_PRIMITIVE** — `var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/CameraRuntime.cs:78` **CREATE_PRIMITIVE** — `var screen = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.ChoiceStation` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:45` **CREATE_PRIMITIVE** — `var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:53` **NEW_GAME_OBJECT** — `var prompt = new GameObject("Prompt");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:54` **TEXTMESH_COMPONENT** — `var tm = prompt.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:72` **CREATE_PRIMITIVE** — `var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:80` **XR_INTERACTABLE_COMPONENT** — `var interactable = panel.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:86` **NEW_GAME_OBJECT** — `var labelGo = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:87` **TEXTMESH_COMPONENT** — `var tm = labelGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:137` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:138` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:140` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.ClimbableSurface` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs:30` **XR_INTERACTABLE_COMPONENT** — `if (grab == null) grab = gameObject.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs:89` **CREATE_PRIMITIVE** — `var stud = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs:132` **NEW_GAME_OBJECT** — `var go = new GameObject("__ClimbCoordinator");`

### `Ziptide.Gameplay.CollectibleRuntime` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:52` **XR_INTERACTABLE_COMPONENT** — `var grab = gameObject.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:58` **CREATE_PRIMITIVE** — `var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:69` **NEW_GAME_OBJECT** — `var labelGo = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:70` **TEXTMESH_COMPONENT** — `var tm = labelGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:100` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:101` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:103` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.ComfortConsoleRuntime` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:64` **CREATE_PRIMITIVE** — `var board = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:83` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:90` **XR_INTERACTABLE_COMPONENT** — `var interactable = tile.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:99` **NEW_GAME_OBJECT** — `var go = new GameObject("Label_" + text.Replace('\n', '_'));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:102` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:121` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:122` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:124` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader);`

### `Ziptide.Gameplay.ComfortVignette` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:41` **NEW_GAME_OBJECT** — `var go = new GameObject("ComfortVignette");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:125` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:126` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Unlit/Color");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:127` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader) { color = Color.black };`

### `Ziptide.Gameplay.ConquestMissionRuntime` — 15 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:53` **NEW_GAME_OBJECT** — `var go = new GameObject("__ConquestMission");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:79` **NEW_GAME_OBJECT** — `_boardRoot = new GameObject("MissionBoard").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:82` **TEXTMESH_COMPONENT** — `_board = _boardRoot.gameObject.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:136` **NEW_GAME_OBJECT** — `var root = new GameObject("ShieldPylon_" + i);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:164` **NEW_GAME_OBJECT** — `var root = new GameObject("ScanNode_" + i);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:186` **NEW_GAME_OBJECT** — `var beaconRoot = new GameObject("StrikeBeacon");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:207` **XR_INTERACTABLE_COMPONENT** — `beaconRoot.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:210` **NEW_GAME_OBJECT** — `var pad = new GameObject("UplinkPad");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:227` **NEW_GAME_OBJECT** — `var root = new GameObject("Conduit_" + i);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:257` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:279` **CREATE_PRIMITIVE** — `var p = GameObject.CreatePrimitive(type);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:290` **NEW_GAME_OBJECT** — `var pivot = new GameObject("OrbiterPivot").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:294` **CREATE_PRIMITIVE** — `var chip = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:398` **XR_INTERACTABLE_COMPONENT** — `gameObject.AddComponent<XRSimpleInteractable>().selectEntered`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:477` **XR_INTERACTABLE_COMPONENT** — `gameObject.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => Hit());`

### `Ziptide.Gameplay.ConquestTableRuntime` — 12 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:145` **CREATE_PRIMITIVE** — `var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:164` **CREATE_PRIMITIVE** — `var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:171` **XR_INTERACTABLE_COMPONENT** — `orb.AddComponent<XRSimpleInteractable>().selectEntered`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:191` **CREATE_PRIMITIVE** — `var line = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:299` **NEW_GAME_OBJECT** — `_rackRoot = new GameObject("FleetRack");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:306` **CREATE_PRIMITIVE** — `var tok = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:313` **XR_INTERACTABLE_COMPONENT** — `tok.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => ToggleToken(idx));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:378` **NEW_GAME_OBJECT** — `_offerPanel = new GameObject("BattleOffer");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:715` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:721` **XR_INTERACTABLE_COMPONENT** — `tile.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => onSelect());`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:728` **NEW_GAME_OBJECT** — `var go = new GameObject("Txt_" + (text.Length > 12 ? text.Substring(0, 12) : text));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:731` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.CreatureBehaviorBase` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs:146` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(type);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs:159` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs:160` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs:163` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.CreatureRuntime` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:205` **CREATE_PRIMITIVE** — `var arc = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:217` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:218` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:221` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.CreditsHud` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs:48` **NEW_GAME_OBJECT** — `var go = new GameObject("CreditsHudText");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs:50` **TEXTMESH_COMPONENT** — `_text = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.DevTools.DevMenu` — 11 signal(s)

- Codes: `CANVAS_COMPONENT`, `EVENT_SYSTEM_COMPONENT`, `NEW_GAME_OBJECT`, `TMP_COMPONENT`, `XR_UI_INPUT_MODULE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:82` **NEW_GAME_OBJECT** — `host = new GameObject("__DevMenuEventSystem");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:84` **EVENT_SYSTEM_COMPONENT** — `host.AddComponent<EventSystem>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:86` **XR_UI_INPUT_MODULE** — `module = host.AddComponent<XRUIInputModule>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:128` **NEW_GAME_OBJECT** — `_canvasGo = new GameObject("DevMenuCanvas");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:130` **CANVAS_COMPONENT** — `var canvas = _canvasGo.AddComponent<Canvas>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:171` **NEW_GAME_OBJECT** — `var go = new GameObject("Panel", typeof(Image));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:180` **NEW_GAME_OBJECT** — `var go = new GameObject("Label", typeof(TextMeshProUGUI));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:180` **TMP_COMPONENT** — `var go = new GameObject("Label", typeof(TextMeshProUGUI));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:195` **NEW_GAME_OBJECT** — `var go = new GameObject("Btn_" + label, typeof(Image), typeof(Button));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:206` **NEW_GAME_OBJECT** — `var labelGo = new GameObject("Text", typeof(TextMeshProUGUI));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:206` **TMP_COMPONENT** — `var labelGo = new GameObject("Text", typeof(TextMeshProUGUI));`

### `Ziptide.Gameplay.DevTools.DevWarp` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarp.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarp.cs:77` **NEW_GAME_OBJECT** — `var go = new GameObject("__DevWarpRunner");`

### `Ziptide.Gameplay.DevTools.DevWarpBoard` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:65` **NEW_GAME_OBJECT** — `var go = new GameObject("__DevWarpBoard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:160` **NEW_GAME_OBJECT** — `_board = new GameObject("DevWarpBoard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:188` **CREATE_PRIMITIVE** — `var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:225` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:232` **XR_INTERACTABLE_COMPONENT** — `var interactable = go.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:338` **NEW_GAME_OBJECT** — `var go = new GameObject("Label_" + text.Replace(' ', '_'));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:346` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.DispatchKiosk` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:44` **NEW_GAME_OBJECT** — `var go = new GameObject("__HowToSign");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:48` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.DroneCombatBehavior` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:222` **NEW_GAME_OBJECT** — `var go = new GameObject("StunBolt");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:273` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:274` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:275` **RUNTIME_MATERIAL_CREATE** — `_threatMaterial = new Material(shader) { name = "DroneThreatPresentation" };`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:287` **CREATE_PRIMITIVE** — `_telegraphFx = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:303` **NEW_GAME_OBJECT** — `var line = new GameObject("__ThreatAimLine");`

### `Ziptide.Gameplay.DroneRuntime` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:64` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:65` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:68` **RUNTIME_MATERIAL_CREATE** — `_mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:127` **NEW_GAME_OBJECT** — `var fxGo = new GameObject("ShockFX");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:152` **CREATE_PRIMITIVE** — `var arc = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:166` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:167` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:170` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.EcologyDirector` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:56` **NEW_GAME_OBJECT** — `var go = new GameObject("EcologyDirector");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:206` **NEW_GAME_OBJECT** — `var go = new GameObject("Nest_" + s.CreatureId + "_" + i);`

### `Ziptide.Gameplay.FirstDestinationHelmRuntime` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:52` **CREATE_PRIMITIVE** — `var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:59` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:66` **XR_INTERACTABLE_COMPONENT** — `var interactable = tile.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:76` **NEW_GAME_OBJECT** — `var go = new GameObject("Label_W001");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:79` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:124` **XR_INTERACTABLE_COMPONENT** — `if (grab == null) grab = gameObject.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:131` **NEW_GAME_OBJECT** — `var label = new GameObject("Label_BUNK_KEEPSAKE");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:134` **TEXTMESH_COMPONENT** — `var tm = label.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.FirstHourDirector` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:60` **NEW_GAME_OBJECT** — `var go = new GameObject("__FirstHourDirector");`

### `Ziptide.Gameplay.FirstHourW001Orchestrator` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:56` **NEW_GAME_OBJECT** — `var go = new GameObject("__FirstHourW001Orchestrator");`

### `Ziptide.Gameplay.GardenPlotRuntime` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:56` **CREATE_PRIMITIVE** — `var soil = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:62` **XR_INTERACTABLE_COMPONENT** — `var interactable = soil.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:67` **CREATE_PRIMITIVE** — `_plantVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:76` **NEW_GAME_OBJECT** — `var readoutGo = new GameObject("Readout");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:77` **TEXTMESH_COMPONENT** — `_readout = readoutGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:249` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:250` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:252` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.GrappleAnchorRuntime` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/GrappleAnchorRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/GrappleAnchorRuntime.cs:38` **CREATE_PRIMITIVE** — `var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/GrappleAnchorRuntime.cs:57` **XR_INTERACTABLE_COMPONENT** — `if (grab == null) grab = gameObject.AddComponent<XRSimpleInteractable>();`

### `Ziptide.Gameplay.GravityGunRuntime` — 5 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:14` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:39` **NEW_GAME_OBJECT** — `var m = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:150` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:151` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GravityGunRuntime.cs:154` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.GunLaserSight` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:90` **NEW_GAME_OBJECT** — `var go = new GameObject("__LaserSight");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:98` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:101` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.HammerTool` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:12` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:78` **CREATE_PRIMITIVE** — `var handle = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:86` **CREATE_PRIMITIVE** — `var head = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.HangarBayRuntime` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:47` **CREATE_PRIMITIVE** — `var board = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:128` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:134` **XR_INTERACTABLE_COMPONENT** — `var grab = tile.AddComponent<XRSimpleInteractable>(); // collider exists (primitive)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:143` **NEW_GAME_OBJECT** — `var go = new GameObject("Txt_" + text);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:146` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.HazardZoneRuntime` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:57` **CREATE_PRIMITIVE** — `var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:67` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:68` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:71` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.HoloRadar` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:43` **CREATE_PRIMITIVE** — `var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:51` **CREATE_PRIMITIVE** — `var sweep = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:61` **CREATE_PRIMITIVE** — `var center = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:75` **CREATE_PRIMITIVE** — `var blip = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:130` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:131` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HoloRadar.cs:132` **RUNTIME_MATERIAL_CREATE** — `m = new Material(shader);`

### `Ziptide.Gameplay.HolsterSocketInteractor` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:88` **NEW_GAME_OBJECT** — `GameObject poseGo = new GameObject("HolsterPose_" + item.Definition.itemId + "_" + key);`

### `Ziptide.Gameplay.HomeHubAnchorLockInstallerRuntime` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockInstallerRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockInstallerRuntime.cs:21` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("__HomeHubAnchorLockInstaller");`

### `Ziptide.Gameplay.HomeHubChoice` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:270` **NEW_GAME_OBJECT** — `var go = new GameObject("__HOME_HUB_COMFORT_SETTINGS");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:294` **CREATE_PRIMITIVE** — `var board = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:326` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:333` **XR_INTERACTABLE_COMPONENT** — `var interactable = tile.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:477` **NEW_GAME_OBJECT** — `var go = new GameObject("Label_" + text.Replace(' ', '_').Replace('\n', '_'));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:480` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:499` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:500` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:502` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader);`

### `Ziptide.Gameplay.HuskMolterBehavior` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/HuskMolterBehavior.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/HuskMolterBehavior.cs:55` **CREATE_PRIMITIVE** — `husk = GameObject.CreatePrimitive(PrimitiveType.Capsule);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/HuskMolterBehavior.cs:62` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/HuskMolterBehavior.cs:63` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/HuskMolterBehavior.cs:66` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.InventoryPersistence` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryPersistence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryPersistence.cs:15` **NEW_GAME_OBJECT** — `var go = new GameObject("__InventoryRoot");`

### `Ziptide.Gameplay.ItemFactory` — 28 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:98` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(r.sharedMaterial);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:174` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:188` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:196` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:208` **NEW_GAME_OBJECT** — `var muzzle = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:218` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:232` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:240` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:251` **NEW_GAME_OBJECT** — `var muzzle = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:265` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:279` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:287` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:296` **NEW_GAME_OBJECT** — `var lens = new GameObject("Lens");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:308` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:322` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:330` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:341` **NEW_GAME_OBJECT** — `var muzzle = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:415` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:428` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:436` **NEW_GAME_OBJECT** — `var gripGo = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:445` **NEW_GAME_OBJECT** — `var muzzleGo = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:466` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:474` **XR_INTERACTABLE_COMPONENT** — `go.AddComponent<XRSimpleInteractable>(); // collider exists first (gotcha #6)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:482` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:490` **XR_INTERACTABLE_COMPONENT** — `go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:502` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:503` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:505` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.ItemRuntime` — 1 signal(s)

- Codes: `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemRuntime.cs:14` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`

### `Ziptide.Gameplay.JobCollectible` — 1 signal(s)

- Codes: `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobCollectible.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobCollectible.cs:11` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`

### `Ziptide.Gameplay.JobDirector` — 15 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:138` **NEW_GAME_OBJECT** — `var root = new GameObject("SpawnMarkers");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:142` **NEW_GAME_OBJECT** — `var go = new GameObject("Marker_" + m.markerId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:154` **NEW_GAME_OBJECT** — `var root = new GameObject("BuildSockets");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:160` **NEW_GAME_OBJECT** — `var go = new GameObject("Socket_" + s.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:171` **NEW_GAME_OBJECT** — `var root = new GameObject("Gardens");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:178` **NEW_GAME_OBJECT** — `var go = new GameObject("Garden_" + g.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:194` **NEW_GAME_OBJECT** — `var root = new GameObject("Mines");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:200` **NEW_GAME_OBJECT** — `var go = new GameObject("Mine_" + m.id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:211` **NEW_GAME_OBJECT** — `var root = new GameObject("Machines");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:216` **NEW_GAME_OBJECT** — `var go = new GameObject("Machine_" + m.machineId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:227` **NEW_GAME_OBJECT** — `var root = new GameObject("Choices");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:232` **NEW_GAME_OBJECT** — `var go = new GameObject("Choice_" + c.choiceId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:244` **NEW_GAME_OBJECT** — `var root = new GameObject("Collectibles");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:249` **NEW_GAME_OBJECT** — `var go = new GameObject("Collectible_" + c.itemId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:258` **NEW_GAME_OBJECT** — `var console = new GameObject("TransmissionConsole_" + c.itemId);`

### `Ziptide.Gameplay.LifecycleState` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:88` **NEW_GAME_OBJECT** — `var go = new GameObject("SystemFocusLifecycle");`

### `Ziptide.Gameplay.LiftRuntime` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/LiftRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/LiftRuntime.cs:53` **CREATE_PRIMITIVE** — `var deck = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/LiftRuntime.cs:127` **CREATE_PRIMITIVE** — `var pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`

### `Ziptide.Gameplay.MiningRigRuntime` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:68` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:75` **CREATE_PRIMITIVE** — `var drill = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:85` **CREATE_PRIMITIVE** — `var hopper = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:92` **XR_INTERACTABLE_COMPONENT** — `var interactable = hopper.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:97` **NEW_GAME_OBJECT** — `var readoutGo = new GameObject("Readout");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:98` **TEXTMESH_COMPONENT** — `_readout = readoutGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:152` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:153` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:155` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.NestRuntime` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:39` **CREATE_PRIMITIVE** — `var layer = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:50` **CREATE_PRIMITIVE** — `var mouth = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:61` **CREATE_PRIMITIVE** — `var egg = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:71` **XR_INTERACTABLE_COMPONENT** — `var interactable = egg.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:82` **CREATE_PRIMITIVE** — `var stake = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:142` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:143` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:145` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.ObjectiveBeacon` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs:25` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("__ObjectiveBeacon");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs:36` **CREATE_PRIMITIVE** — `GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs:44` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs:45` **RUNTIME_MATERIAL_CREATE** — `_mat = shader != null ? new Material(shader) : null;`

### `Ziptide.Gameplay.ObjectiveBoard` — 11 signal(s)

- Codes: `CANVAS_COMPONENT`, `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `TMP_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:216` **NEW_GAME_OBJECT** — `_toastRoot = new GameObject("__CONTRACT_TOAST");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:218` **CREATE_PRIMITIVE** — `GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:226` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:227` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:230` **RUNTIME_MATERIAL_CREATE** — `_toastMaterial = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:236` **NEW_GAME_OBJECT** — `GameObject text = new GameObject("Text");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:240` **TEXTMESH_COMPONENT** — `_toastText = text.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:277` **NEW_GAME_OBJECT** — `var canvasGo = new GameObject("ObjectiveCanvas");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:283` **CANVAS_COMPONENT** — `var canvas = canvasGo.AddComponent<Canvas>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:290` **NEW_GAME_OBJECT** — `var textGo = new GameObject("ObjectiveText");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:298` **TMP_COMPONENT** — `var tmp = textGo.AddComponent<TextMeshProUGUI>();`

### `Ziptide.Gameplay.PhotoCaptureCamera` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs:158` **NEW_GAME_OBJECT** — `var cameraObject = new GameObject("__PHOTO_CAPTURE_CAMERA");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs:184` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ??`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs:185` **SHADER_FIND** — `Shader.Find("Unlit/Texture") ?? Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs:188` **RUNTIME_MATERIAL_CREATE** — `_viewfinderMaterial = new Material(shader) { name = "FieldCameraViewfinder" };`

### `Ziptide.Gameplay.PingTool` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PingTool.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PingTool.cs:52` **NEW_GAME_OBJECT** — `_current = new GameObject("__Ping");`

### `Ziptide.Gameplay.PistolRuntime` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs:13` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs:44` **NEW_GAME_OBJECT** — `var m = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs:140` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);`

### `Ziptide.Gameplay.PlayAreaBounds` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/PlayAreaBounds.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/PlayAreaBounds.cs:34` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Gameplay.PlayerAvatarRig` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerAvatarRig.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerAvatarRig.cs:50` **NEW_GAME_OBJECT** — `var root = new GameObject("__Glove");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerAvatarRig.cs:74` **NEW_GAME_OBJECT** — `var torso = new GameObject("__AvatarTorso");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerAvatarRig.cs:93` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.PlayerMenuRuntime` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:165` **NEW_GAME_OBJECT** — `_menuRoot = new GameObject("__PLAYER_FIELD_MENU");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:168` **CREATE_PRIMITIVE** — `GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:186` **CREATE_PRIMITIVE** — `GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:193` **XR_INTERACTABLE_COMPONENT** — `XRSimpleInteractable interactable = tile.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:223` **NEW_GAME_OBJECT** — `GameObject label = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:226` **TEXTMESH_COMPONENT** — `TextMesh mesh = label.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:245` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:246` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:248` **RUNTIME_MATERIAL_CREATE** — `Material material = new Material(shader);`

### `Ziptide.Gameplay.PlayerStunReceiver` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:95` **CREATE_PRIMITIVE** — `_flashGo = GameObject.CreatePrimitive(PrimitiveType.Quad);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:106` **SHADER_FIND** — `var shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:107` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:108` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.PrismBeamRuntime` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:15` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:99` **CREATE_PRIMITIVE** — `_guide = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PrismBeamRuntime.cs:133` **CREATE_PRIMITIVE** — `var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.PvpBolt` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs:32` **CREATE_PRIMITIVE** — `var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs:41` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs:42` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs:43` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.PvpBot` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:105` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:106` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:107` **RUNTIME_MATERIAL_CREATE** — `if (shader != null) { _mat = new Material(shader); _renderer.material = _mat; SetColor(LiveColor); }`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:332` **NEW_GAME_OBJECT** — `var go = new GameObject("PvpBolt");`

### `Ziptide.Gameplay.PvpComfortHop` — 1 signal(s)

- Codes: `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpComfortHop.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpComfortHop.cs:12` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`

### `Ziptide.Gameplay.PvpHud` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpHud.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpHud.cs:28` **NEW_GAME_OBJECT** — `var go = new GameObject("PvpHudText");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpHud.cs:30` **TEXTMESH_COMPONENT** — `_text = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.PvpModeDirector` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:184` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:368` **NEW_GAME_OBJECT** — `var go = new GameObject("Horde_" + creatureId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:441` **CREATE_PRIMITIVE** — `var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:472` **CREATE_PRIMITIVE** — `_fragmentGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);`

### `Ziptide.Gameplay.PvpOnlinePresence` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:134` **NEW_GAME_OBJECT** — `var root = new GameObject("__RemotePlayer_" + id);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:135` **NEW_GAME_OBJECT** — `var head = new GameObject("Head").transform; head.SetParent(root.transform, false);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:152` **NEW_GAME_OBJECT** — `var g = new GameObject(name).transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:166` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:193` **NEW_GAME_OBJECT** — `var go = new GameObject("__PvpOnlinePresence");`

### `Ziptide.Gameplay.PvpProgressionRuntime` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:31` **NEW_GAME_OBJECT** — `var go = new GameObject("PvpProgression");`

### `Ziptide.Gameplay.QuartersCameraFeature` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:54` **NEW_GAME_OBJECT** — `Transform root = new GameObject(FeatureRootName).transform; root.SetParent(transform, false);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:56` **NEW_GAME_OBJECT** — `Transform dock = new GameObject("FieldCameraDock").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:58` **CREATE_PRIMITIVE** — `GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:68` **NEW_GAME_OBJECT** — `Transform wall = new GameObject("PhotoWallHost").transform;`

### `Ziptide.Gameplay.QuartersPhotoWall` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:23` **NEW_GAME_OBJECT** — `_root = new GameObject(RootName).transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:45` **NEW_GAME_OBJECT** — `Transform holder = new GameObject("Photo_" + slot).transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:94` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture") ?? Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:96` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader) { name = name };`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:103` **CREATE_PRIMITIVE** — `GameObject go = GameObject.CreatePrimitive(type);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:118` **NEW_GAME_OBJECT** — `var go = new GameObject("EmptyNotice"); go.transform.SetParent(_root, false); go.transform.localPosition = position;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:119` **TEXTMESH_COMPONENT** — `TextMesh text = go.AddComponent<TextMesh>(); text.text = value; text.anchor = TextAnchor.MiddleCenter;`

### `Ziptide.Gameplay.QuartersRoom` — 16 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:106` **NEW_GAME_OBJECT** — `var bay = new GameObject("Bay_" + kind);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:115` **NEW_GAME_OBJECT** — `var header = new GameObject("Header");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:116` **TEXTMESH_COMPONENT** — `var htm = header.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:161` **NEW_GAME_OBJECT** — `var empty = new GameObject("EmptyNotice");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:162` **TEXTMESH_COMPONENT** — `var tm = empty.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:203` **NEW_GAME_OBJECT** — `var board = new GameObject("LockerBoard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:204` **TEXTMESH_COMPONENT** — `_lockerBoard = board.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:234` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:248` **NEW_GAME_OBJECT** — `var root = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:252` **CREATE_PRIMITIVE** — `var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:259` **XR_INTERACTABLE_COMPONENT** — `var interactable = plate.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:266` **NEW_GAME_OBJECT** — `var textGo = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:267` **TEXTMESH_COMPONENT** — `var tm = textGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:284` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:285` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:287` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.RepairPartSafetyInstallerRuntime` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyInstallerRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyInstallerRuntime.cs:22` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("__RepairPartSafetyInstaller");`

### `Ziptide.Gameplay.RepairPartSafetyRuntime` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairPartSafetyRuntime.cs:40` **NEW_GAME_OBJECT** — `GameObject attach = new GameObject("RepairPartGrip");`

### `Ziptide.Gameplay.RepairableMachine` — 20 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:95` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:102` **CREATE_PRIMITIVE** — `var lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:111` **NEW_GAME_OBJECT** — `var labelGo = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:112` **TEXTMESH_COMPONENT** — `_label = labelGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:121` **CREATE_PRIMITIVE** — `var socket = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:132` **NEW_GAME_OBJECT** — `var panel = new GameObject("Panel");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:141` **XR_INTERACTABLE_COMPONENT** — `var panelGrab = panel.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:147` **CREATE_PRIMITIVE** — `var panelVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:154` **CREATE_PRIMITIVE** — `var sw = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:164` **XR_INTERACTABLE_COMPONENT** — `var swInteractable = sw.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:169` **NEW_GAME_OBJECT** — `var switchLabelGo = new GameObject("PowerSwitchLabel");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:173` **TEXTMESH_COMPONENT** — `var switchLabel = switchLabelGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:189` **NEW_GAME_OBJECT** — `var part = new GameObject("Part_" + _def.partItemId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:197` **XR_INTERACTABLE_COMPONENT** — `var partGrab = part.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:214` **CREATE_PRIMITIVE** — `var partVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:220` **NEW_GAME_OBJECT** — `var partLabel = new GameObject("PartLabel");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:221` **TEXTMESH_COMPONENT** — `var ptm = partLabel.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:468` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:469` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:472` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader);`

### `Ziptide.Gameplay.RillCompanion` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:241` **CREATE_PRIMITIVE** — `_orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:249` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:250` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:253` **RUNTIME_MATERIAL_CREATE** — `_orbMat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:262` **NEW_GAME_OBJECT** — `var go = new GameObject("__RillSubtitle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:263` **TEXTMESH_COMPONENT** — `_text = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.SalvageCacheRuntime` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:46` **XR_INTERACTABLE_COMPONENT** — `var grab = body.gameObject.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:55` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:62` **CREATE_PRIMITIVE** — `var band = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.SaveSystem` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:54` **NEW_GAME_OBJECT** — `var go = new GameObject(GoName);`

### `Ziptide.Gameplay.ShipBoardingStation` — 19 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:56` **NEW_GAME_OBJECT** — `var hangar = new GameObject("HangarBay");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:70` **NEW_GAME_OBJECT** — `var room = new GameObject("Quarters");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:157` **NEW_GAME_OBJECT** — `_helmRowsRoot = new GameObject("HelmRows").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:209` **NEW_GAME_OBJECT** — `var streakRoot = new GameObject("__FlyOutStreaks").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:212` **CREATE_PRIMITIVE** — `var sGo = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:220` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:221` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:224` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:237` **NEW_GAME_OBJECT** — `var countGo = new GameObject("__DepartReadout");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:238` **TEXTMESH_COMPONENT** — `var countText = countGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:307` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:321` **NEW_GAME_OBJECT** — `var root = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:324` **CREATE_PRIMITIVE** — `var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:331` **XR_INTERACTABLE_COMPONENT** — `var interactable = plate.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:338` **NEW_GAME_OBJECT** — `var textGo = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:339` **TEXTMESH_COMPONENT** — `var tm = textGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:356` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:357` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:359` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.ShipCastOffRuntime` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:89` **CREATE_PRIMITIVE** — `var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:100` **CREATE_PRIMITIVE** — `var button = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:107` **NEW_GAME_OBJECT** — `var label = new GameObject("Label_PUNCH_IT");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:112` **TEXTMESH_COMPONENT** — `var tm = label.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:121` **XR_INTERACTABLE_COMPONENT** — `var interactable = button.AddComponent<XRSimpleInteractable>();`

### `Ziptide.Gameplay.ShipRefit` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:153` **NEW_GAME_OBJECT** — `var rail = new GameObject("JourneyDecals").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:160` **CREATE_PRIMITIVE** — `var plate = GameObject.CreatePrimitive(PrimitiveType.Quad);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:183` **NEW_GAME_OBJECT** — `var go = new GameObject("ShipNameplate");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:188` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`

### `Ziptide.Gameplay.SonicThumperRuntime` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:15` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:82` **CREATE_PRIMITIVE** — `var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`

### `Ziptide.Gameplay.StaticNetGunRuntime` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs:13` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs:48` **CREATE_PRIMITIVE** — `var net = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs:98` **NEW_GAME_OBJECT** — `var zone = new GameObject("SlowZone");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs:127` **CREATE_PRIMITIVE** — `_disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`

### `Ziptide.Gameplay.StunBolt` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs:30` **CREATE_PRIMITIVE** — `var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs:39` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs:40` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs:41` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.TaserDartGunRuntime` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs:12` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs:46` **NEW_GAME_OBJECT** — `var m = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs:99` **CREATE_PRIMITIVE** — `var dart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`

### `Ziptide.Gameplay.TaserDartProjectile` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartProjectile.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartProjectile.cs:105` **CREATE_PRIMITIVE** — `var spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartProjectile.cs:114` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartProjectile.cs:115` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartProjectile.cs:118` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.TetherSwarmBehavior` — 5 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs:29` **NEW_GAME_OBJECT** — `_clusterA = new GameObject("ClusterA").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs:32` **NEW_GAME_OBJECT** — `_clusterB = new GameObject("ClusterB").transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs:58` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs:59` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/TetherSwarmBehavior.cs:62` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.ThemeSwitchStation` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/ThemeSwitchStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/ThemeSwitchStation.cs:51` **CREATE_PRIMITIVE** — `GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/ThemeSwitchStation.cs:61` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/ThemeSwitchStation.cs:64` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/ThemeSwitchStation.cs:74` **XR_INTERACTABLE_COMPONENT** — `var interactable = button.AddComponent<XRSimpleInteractable>();`

### `Ziptide.Gameplay.TracerFx` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TracerFx.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TracerFx.cs:18` **NEW_GAME_OBJECT** — `var go = new GameObject("__Tracer");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TracerFx.cs:34` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TracerFx.cs:35` **RUNTIME_MATERIAL_CREATE** — `var mat = shader != null ? new Material(shader) : new Material(Shader.Find("Sprites/Default"));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TracerFx.cs:35` **SHADER_FIND** — `var mat = shader != null ? new Material(shader) : new Material(Shader.Find("Sprites/Default"));`

### `Ziptide.Gameplay.TransmissionConsole` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:32` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:39` **CREATE_PRIMITIVE** — `var screen = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:48` **XR_INTERACTABLE_COMPONENT** — `var interactable = screen.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:54` **NEW_GAME_OBJECT** — `var textGo = new GameObject("ScreenText");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:55` **TEXTMESH_COMPONENT** — `_screenText = textGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:96` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:97` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:99` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:50` **NEW_GAME_OBJECT** — `var go = new GameObject("__FirstHourObservationAdapter");`

### `Ziptide.Gameplay.WateringCanRuntime` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:38` **NEW_GAME_OBJECT** — `var go = new GameObject("WateringCan");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:44` **XR_INTERACTABLE_COMPONENT** — `var grab = go.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:59` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:65` **CREATE_PRIMITIVE** — `var spout = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:74` **CREATE_PRIMITIVE** — `var level = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:82` **CREATE_PRIMITIVE** — `var stream = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:155` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:156` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:158` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.WeaponImpactFx` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/WeaponImpactFx.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/WeaponImpactFx.cs:25` **CREATE_PRIMITIVE** — `GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/WeaponImpactFx.cs:34` **CREATE_PRIMITIVE** — `GameObject ray = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.WeaponPoseCore` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:60` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRGrabInteractable))]`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:104` **NEW_GAME_OBJECT** — `GameObject go = new GameObject(SemanticGripName);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:216` **CREATE_PRIMITIVE** — `GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);`

### `Ziptide.Gameplay.WorldDebrisBudget` — 1 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WorldDebrisBudget.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WorldDebrisBudget.cs:51` **CREATE_PRIMITIVE** — `var chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.WorldDirector` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldDirector.cs:57` **NEW_GAME_OBJECT** — `GameObject rigGo = new GameObject("SkyRig");`

### `Ziptide.Gameplay.WorldDiscoveryNodeRuntime` — 1 signal(s)

- Codes: `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/WorldDiscoveryNodeRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/WorldDiscoveryNodeRuntime.cs:14` **XR_INTERACTABLE_COMPONENT** — `[RequireComponent(typeof(XRSimpleInteractable))]`

### `Ziptide.Gameplay.WorldRuntime` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs:146` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("WorldDirector");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs:184` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("PlayAreaBounds");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs:200` **NEW_GAME_OBJECT** — `GameObject go = new GameObject("ThemeSwitchStation");`

### `Ziptide.Gameplay.WorldTravelStation` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:88` **NEW_GAME_OBJECT** — `var doorRoot = new GameObject("TravelDoor_" + (pack.packId ?? "?"));`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:108` **NEW_GAME_OBJECT** — `var door = new GameObject("Door");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:117` **XR_INTERACTABLE_COMPONENT** — `var interactable = door.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:174` **CREATE_PRIMITIVE** — `var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:226` **NEW_GAME_OBJECT** — `var go = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:227` **TEXTMESH_COMPONENT** — `var tm = go.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:241` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:242` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:244` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader);`

### `Ziptide.Gameplay.WristScanner` — 11 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:109` **NEW_GAME_OBJECT** — `_bracer = new GameObject("__WristScanner");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:114` **CREATE_PRIMITIVE** — `var body = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:120` **CREATE_PRIMITIVE** — `var lens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:128` **CREATE_PRIMITIVE** — `var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:202` **NEW_GAME_OBJECT** — `var go = new GameObject("__HoloRadar");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:264` **CREATE_PRIMITIVE** — `var tag = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:276` **CREATE_PRIMITIVE** — `_edgeChevron = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:307` **CREATE_PRIMITIVE** — `var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:371` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:372` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:373` **RUNTIME_MATERIAL_CREATE** — `m = new Material(shader);`

### `Ziptide.Gameplay.ZiplineRuntime` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:99` **CREATE_PRIMITIVE** — `var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:112` **CREATE_PRIMITIVE** — `var handleGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:122` **XR_INTERACTABLE_COMPONENT** — `var grab = handleGo.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:131` **CREATE_PRIMITIVE** — `var post = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Gameplay.ZiptideGateEffect` — 10 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:99` **NEW_GAME_OBJECT** — `var go = new GameObject(departure ? "__ZiptideDepart" : "__ZiptideArrive");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:112` **NEW_GAME_OBJECT** — `var go = new GameObject("__Destination");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:114` **TEXTMESH_COMPONENT** — `_label = go.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:124` **RUNTIME_MATERIAL_CREATE** — `/// `new Material(null)` throw, and the gate runs on the travel path (a throw there could`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:128` **SHADER_FIND** — `var shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:129` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:130` **RUNTIME_MATERIAL_CREATE** — `return new Material(shader);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:141` **CREATE_PRIMITIVE** — `var p = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:153` **CREATE_PRIMITIVE** — `var pool = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:230` **CREATE_PRIMITIVE** — `_flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);`

### `Ziptide.Ship.ShipFlightRuntime` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:170` **CREATE_PRIMITIVE** — `var console = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:182` **NEW_GAME_OBJECT** — `var statusGo = new GameObject("FlightStatus");`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:186` **TEXTMESH_COMPONENT** — `_statusText = statusGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:194` **CREATE_PRIMITIVE** — `_returnPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:201` **XR_INTERACTABLE_COMPONENT** — `var ret = _returnPanel.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:216` **CREATE_PRIMITIVE** — `var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:223` **XR_INTERACTABLE_COMPONENT** — `var interactable = tile.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:230` **NEW_GAME_OBJECT** — `var label = new GameObject("Label");`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:234` **TEXTMESH_COMPONENT** — `var tm = label.AddComponent<TextMesh>();`

### `Ziptide.Ship.VehicleRuntime` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`, `TEXTMESH_COMPONENT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:124` **NEW_GAME_OBJECT** — `Transform visual = new GameObject(VisualRootName).transform;`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:139` **NEW_GAME_OBJECT** — `GameObject labelGo = new GameObject("RideLabel");`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:142` **TEXTMESH_COMPONENT** — `_label = labelGo.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:271` **CREATE_PRIMITIVE** — `_mountAffordance = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:278` **XR_INTERACTABLE_COMPONENT** — `XRSimpleInteractable interactable = _mountAffordance.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:287` **CREATE_PRIMITIVE** — `GameObject go = GameObject.CreatePrimitive(primitive);`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:549` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:550` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:552` **RUNTIME_MATERIAL_CREATE** — `material = new Material(shader) { name = "Vehicle_" + ColorUtility.ToHtmlStringRGB(color) };`

### `Ziptide.Tests.EditMode.AmbientMoteTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/AmbientMoteTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/AmbientMoteTests.cs:69` **NEW_GAME_OBJECT** — `_root = new GameObject("AmbientMoteTestRoot");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/AmbientMoteTests.cs:114` **NEW_GAME_OBJECT** — `_root = new GameObject("AmbientMoteRuntimeTest");`

### `Ziptide.Tests.EditMode.ArtConformanceAuditRulesTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ArtConformanceAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ArtConformanceAuditRulesTests.cs:21` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.EditMode.AudioDirectorLifecycleTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs:27` **NEW_GAME_OBJECT** — `_object = new GameObject("AudioDirectorLifecycleTest");`

### `Ziptide.Tests.EditMode.AutomationAuditRulesTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/AutomationAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/AutomationAuditRulesTests.cs:22` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.EditMode.BeltPadTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/BeltPadTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/BeltPadTests.cs:26` **NEW_GAME_OBJECT** — `_parent = new GameObject("PadTestParent");`

### `Ziptide.Tests.EditMode.BuildProfileTravelAuditRulesTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/BuildProfileTravelAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/BuildProfileTravelAuditRulesTests.cs:33` **NEW_GAME_OBJECT** — `go = new GameObject("BrokenGoldenExit");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/BuildProfileTravelAuditRulesTests.cs:59` **NEW_GAME_OBJECT** — `go = new GameObject("ValidGoldenExit");`

### `Ziptide.Tests.EditMode.CreatureDisabledSignalTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs:27` **NEW_GAME_OBJECT** — `_creature = new GameObject("Creature_swarm_bug");`

### `Ziptide.Tests.EditMode.DispatchKioskSpatialContractTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/DispatchKioskSpatialContractTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/DispatchKioskSpatialContractTests.cs:24` **NEW_GAME_OBJECT** — `GameObject sign = new GameObject("KioskSignFacingTest");`

### `Ziptide.Tests.EditMode.ForgeBodyTellTests` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs:17` **NEW_GAME_OBJECT** — `var host = new GameObject("bare");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs:30` **NEW_GAME_OBJECT** — `var host = new GameObject("host");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs:31` **NEW_GAME_OBJECT** — `var vis = new GameObject(ForgeCreatureVisualApplier.VisualChildName);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs:84` **NEW_GAME_OBJECT** — `var unforged = new GameObject("unforged");`

### `Ziptide.Tests.EditMode.GamePoolReleaseAfterTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/GamePoolReleaseAfterTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GamePoolReleaseAfterTests.cs:23` **NEW_GAME_OBJECT** — `var go = GamePool.Get("hw_test_zero", () => new GameObject("hw_test_zero"), Vector3.one);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GamePoolReleaseAfterTests.cs:32` **NEW_GAME_OBJECT** — `var again = GamePool.Get("hw_test_zero", () => new GameObject("hw_test_zero_SHOULD_NOT_BUILD"), Vector3.zero);`

### `Ziptide.Tests.EditMode.GroundingTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/GroundingTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GroundingTests.cs:62` **NEW_GAME_OBJECT** — `var host = new GameObject("Host");`

### `Ziptide.Tests.EditMode.HeadsetBuildBlockerRegressionTests` — 1 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs:55` **CREATE_PRIMITIVE** — `StringAssert.Contains("GameObject.CreatePrimitive(PrimitiveType.Cube)", safety);`

### `Ziptide.Tests.EditMode.HeroShipHullBuilderTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HeroShipHullBuilderTests.cs:18` **NEW_GAME_OBJECT** — `ship = new GameObject("Ship_Static_Placeholder");`

### `Ziptide.Tests.EditMode.HomeHubFlowTests` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:134` **NEW_GAME_OBJECT** — `StringAssert.Contains("new GameObject(\"__HOME_HUB_RUNTIME\").AddComponent<HomeHubRuntime>()", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:165` **NEW_GAME_OBJECT** — `StringAssert.Contains("new GameObject(\"__HOME_HUB_COMFORT_SETTINGS\")", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:178` **NEW_GAME_OBJECT** — `var go = new GameObject("castoff-test");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:203` **NEW_GAME_OBJECT** — `var go = new GameObject("bunk-test");`

### `Ziptide.Tests.EditMode.InteractionReachAuditRulesTests` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs:47` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs:52` **CREATE_PRIMITIVE** — `control = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs:56` **XR_INTERACTABLE_COMPONENT** — `control.AddComponent<XRSimpleInteractable>();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs:78` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs:83` **CREATE_PRIMITIVE** — `control = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/InteractionReachAuditRulesTests.cs:87` **XR_INTERACTABLE_COMPONENT** — `control.AddComponent<XRSimpleInteractable>();`

### `Ziptide.Tests.EditMode.JobDirectorMarkerResolutionTests` — 8 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:22` **NEW_GAME_OBJECT** — `_director = new GameObject("JobDirector");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:36` **NEW_GAME_OBJECT** — `_authored = new GameObject("Hero_DispatchHall");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:37` **NEW_GAME_OBJECT** — `var marker = new GameObject("Marker_dispatch_inside");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:50` **NEW_GAME_OBJECT** — `_authored = new GameObject("__TOXIC_CITY_ROOT");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:51` **NEW_GAME_OBJECT** — `var district = new GameObject("District_Shipyard");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:53` **NEW_GAME_OBJECT** — `var hero = new GameObject("Hero_ShipyardOffice");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:55` **NEW_GAME_OBJECT** — `var marker = new GameObject("Marker_shipyard_office");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:66` **NEW_GAME_OBJECT** — `_authored = new GameObject("Marker_relay_node");`

### `Ziptide.Tests.EditMode.M0SystemicDeviceRegressionTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/M0SystemicDeviceRegressionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/M0SystemicDeviceRegressionTests.cs:62` **NEW_GAME_OBJECT** — `GameObject rig = new GameObject("TurnReadinessRig");`

### `Ziptide.Tests.EditMode.PerceptualCoverageAuditRulesTests` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/PerceptualCoverageAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PerceptualCoverageAuditRulesTests.cs:19` **CREATE_PRIMITIVE** — `root = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PerceptualCoverageAuditRulesTests.cs:43` **CREATE_PRIMITIVE** — `root = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PerceptualCoverageAuditRulesTests.cs:65` **CREATE_PRIMITIVE** — `root = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PerceptualCoverageAuditRulesTests.cs:67` **CREATE_PRIMITIVE** — `GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cylinder);`

### `Ziptide.Tests.EditMode.PracticalLightTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/PracticalLightTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PracticalLightTests.cs:16` **NEW_GAME_OBJECT** — `host = new GameObject("PracticalHost");`

### `Ziptide.Tests.EditMode.QuestWeaponAndCouplerRegressionTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/QuestWeaponAndCouplerRegressionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/QuestWeaponAndCouplerRegressionTests.cs:122` **NEW_GAME_OBJECT** — `machineRoot = new GameObject("QuestCouplerRegression");`

### `Ziptide.Tests.EditMode.ReactivePracticalSwapTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePracticalSwapTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePracticalSwapTests.cs:14` **NEW_GAME_OBJECT** — `var root = new GameObject("ReactiveStreetPole");`

### `Ziptide.Tests.EditMode.ReactivePropAuthorTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs:172` **NEW_GAME_OBJECT** — `var value = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs:179` **NEW_GAME_OBJECT** — `var value = new GameObject(name);`

### `Ziptide.Tests.EditMode.ReactivePropTests` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropTests.cs:80` **NEW_GAME_OBJECT** — `var host = new GameObject("ReactivePractical");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropTests.cs:103` **NEW_GAME_OBJECT** — `var host = new GameObject("ReactiveSteam");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropTests.cs:119` **CREATE_PRIMITIVE** — `var host = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropTests.cs:188` **RUNTIME_MATERIAL_CREATE** — `StringAssert.DoesNotContain("new Material(", source);`

### `Ziptide.Tests.EditMode.RepairableMachineSignalTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:240` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.EditMode.ResourceDisciplineTests` — 1 signal(s)

- Codes: `RUNTIME_MATERIAL_CREATE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ResourceDisciplineTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ResourceDisciplineTests.cs:38` **RUNTIME_MATERIAL_CREATE** — `private static readonly string[] CreatorMarkers = { "new Material(", "new Texture2D(", "AudioClip.Create(" };`

### `Ziptide.Tests.EditMode.RoundThreeWorldImprovementModulesTests` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/RoundThreeWorldImprovementModulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RoundThreeWorldImprovementModulesTests.cs:25` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RoundThreeWorldImprovementModulesTests.cs:30` **NEW_GAME_OBJECT** — `spawn = new GameObject("__SPAWN_PLAYER");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RoundThreeWorldImprovementModulesTests.cs:31` **NEW_GAME_OBJECT** — `markerA = new GameObject("Marker_RoundThree_A");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RoundThreeWorldImprovementModulesTests.cs:33` **NEW_GAME_OBJECT** — `markerB = new GameObject("Marker_RoundThree_B");`

### `Ziptide.Tests.EditMode.RouteContinuityAuditRulesTests` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/RouteContinuityAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RouteContinuityAuditRulesTests.cs:15` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RouteContinuityAuditRulesTests.cs:40` **CREATE_PRIMITIVE** — `route = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RouteContinuityAuditRulesTests.cs:111` **CREATE_PRIMITIVE** — `GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Tests.EditMode.ShellSignPlacementTests` — 5 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ShellSignPlacementTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShellSignPlacementTests.cs:94` **NEW_GAME_OBJECT** — `_root = new GameObject("ShellSignPlacementRoot");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShellSignPlacementTests.cs:201` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShellSignPlacementTests.cs:202` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Unlit/Color");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShellSignPlacementTests.cs:203` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShellSignPlacementTests.cs:205` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader) { name = name };`

### `Ziptide.Tests.EditMode.ToxicCityRiverBuilderTests` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityRiverBuilderTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityRiverBuilderTests.cs:20` **NEW_GAME_OBJECT** — `city = new GameObject("__TEST_TOXIC_CITY_ROOT");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityRiverBuilderTests.cs:21` **NEW_GAME_OBJECT** — `Transform legacy = new GameObject("Canals").transform;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityRiverBuilderTests.cs:23` **CREATE_PRIMITIVE** — `GameObject legacySlab = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Tests.EditMode.ToxicCityStageAIntegrationTests` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageAIntegrationTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageAIntegrationTests.cs:30` **NEW_GAME_OBJECT** — `city = new GameObject("__TEST_CITY_ROOT");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageAIntegrationTests.cs:31` **NEW_GAME_OBJECT** — `var district = new GameObject("District_Test");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageAIntegrationTests.cs:78` **CREATE_PRIMITIVE** — `GameObject facade = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Tests.EditMode.ToxicCityStageBTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageBTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageBTests.cs:20` **NEW_GAME_OBJECT** — `city = new GameObject("__TEST_CITY_ROOT");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityStageBTests.cs:21` **NEW_GAME_OBJECT** — `Transform district = new GameObject("District_Test").transform;`

### `Ziptide.Tests.EditMode.ToxicCityVehicleBuilderTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityVehicleBuilderTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ToxicCityVehicleBuilderTests.cs:19` **NEW_GAME_OBJECT** — `city = new GameObject("__TEST_CITY_ROOT");`

### `Ziptide.Tests.EditMode.UiReadabilityAuditRulesTests` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs:55` **TEXTMESH_COMPONENT** — `var smallText = small.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs:61` **TEXTMESH_COMPONENT** — `var emptyText = empty.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs:138` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs:159` **TEXTMESH_COMPONENT** — `var text = label.AddComponent<TextMesh>();`

### `Ziptide.Tests.EditMode.WaterTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WaterTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WaterTests.cs:174` **NEW_GAME_OBJECT** — `var go = new GameObject("Water");`

### `Ziptide.Tests.EditMode.WeaponFeelRuntimeTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponFeelRuntimeTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponFeelRuntimeTests.cs:13` **NEW_GAME_OBJECT** — `var host = new GameObject("WeaponFeelHost");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponFeelRuntimeTests.cs:61` **NEW_GAME_OBJECT** — `var host = new GameObject("WeaponFeelNullHand");`

### `Ziptide.Tests.EditMode.WeaponPerceptualAuditRulesTests` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponPerceptualAuditRulesTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponPerceptualAuditRulesTests.cs:96` **CREATE_PRIMITIVE** — `GameObject weapon = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponPerceptualAuditRulesTests.cs:100` **NEW_GAME_OBJECT** — `Transform grip = new GameObject("Grip").transform;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WeaponPerceptualAuditRulesTests.cs:105` **NEW_GAME_OBJECT** — `Transform muzzle = new GameObject("Muzzle").transform;`

### `Ziptide.Tests.EditMode.WorldImprovementCompilerTests` — 5 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementCompilerTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementCompilerTests.cs:31` **NEW_GAME_OBJECT** — `host = new GameObject("ModuleMarkerSerializationFixture");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementCompilerTests.cs:55` **CREATE_PRIMITIVE** — `floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementCompilerTests.cs:60` **NEW_GAME_OBJECT** — `spawn = new GameObject("__SPAWN_PLAYER");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementCompilerTests.cs:102` **NEW_GAME_OBJECT** — `root = new GameObject(WorldImprovementCompiler.RootName);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementCompilerTests.cs:106` **NEW_GAME_OBJECT** — `var moduleRoot = new GameObject("__WIM_ARRIVAL_IDENTITY");`

### `Ziptide.Tests.EditMode.WristScannerResultTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WristScannerResultTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WristScannerResultTests.cs:212` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.EditMode.ZiplineSignalTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:160` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.PlayMode.PlayModeInfrastructureTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/PlayModeInfrastructureTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/PlayModeInfrastructureTests.cs:20` **NEW_GAME_OBJECT** — `var host = new GameObject("__RECOVERY_PLAYMODE_FRAME_PROBE");`

### `Ziptide.Tests.PlayMode.RecoveryBootHoldOrderingTests` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootHoldOrderingTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootHoldOrderingTests.cs:62` **NEW_GAME_OBJECT** — `var rigHost = new GameObject("__RECOVERY_BOOT_ORDER_RIG");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootHoldOrderingTests.cs:66` **NEW_GAME_OBJECT** — `var bootHost = new GameObject("__RECOVERY_BOOT_ORDER_LOADER");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootHoldOrderingTests.cs:92` **NEW_GAME_OBJECT** — `var markerHost = new GameObject("__RECOVERY_BOOT_ORDER_MARKER");`

### `Ziptide.Tests.PlayMode.RecoveryCoreBootstrapGateTests` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryCoreBootstrapGateTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryCoreBootstrapGateTests.cs:84` **NEW_GAME_OBJECT** — `var cameraHost = new GameObject("__RECOVERY_CORE_GATE_CAMERA");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryCoreBootstrapGateTests.cs:88` **CREATE_PRIMITIVE** — `var rendererHost = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Tests.PlayMode.RecoveryFallbackSurfaceAuditTests` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:13` **NEW_GAME_OBJECT** — `var root = new GameObject("__RECOVERY_FALLBACK_CANARY_ROOT");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:19` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:20` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:21` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Unlit/Color");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:24` **CREATE_PRIMITIVE** — `GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:29` **RUNTIME_MATERIAL_CREATE** — `fallbackMaterial = new Material(shader)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:35` **CREATE_PRIMITIVE** — `GameObject nullSlot = GameObject.CreatePrimitive(PrimitiveType.Sphere);`

### `Ziptide.Tests.PlayMode.RecoveryForgeVisualOwnershipTests` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:44` **CREATE_PRIMITIVE** — `_item = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:51` **XR_INTERACTABLE_COMPONENT** — `var grab = _item.AddComponent<XRGrabInteractable>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:54` **NEW_GAME_OBJECT** — `var grip = new GameObject("Grip");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:60` **NEW_GAME_OBJECT** — `var muzzle = new GameObject("Muzzle");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:64` **NEW_GAME_OBJECT** — `var visual = new GameObject(ForgeVisualName);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryForgeVisualOwnershipTests.cs:66` **CREATE_PRIMITIVE** — `GameObject stale = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Tests.PlayMode.RecoveryGateBypassTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGateBypassTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGateBypassTests.cs:110` **NEW_GAME_OBJECT** — `int launcherCreate = netSource.IndexOf("new GameObject(\"__PhotonPvpLauncher\")", startOnline, StringComparison.Ordinal);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGateBypassTests.cs:154` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.PlayMode.RecoveryGoldenSurfacePolicyTests` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenSurfacePolicyTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenSurfacePolicyTests.cs:34` **NEW_GAME_OBJECT** — `var host = new GameObject("__RECOVERY_GOLDEN_CREDITS_HUD");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenSurfacePolicyTests.cs:50` **NEW_GAME_OBJECT** — `var host = new GameObject("__RECOVERY_FULL_CREDITS_HUD");`

### `Ziptide.Tests.PlayMode.RecoveryHomeHubBindingTests` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:68` **NEW_GAME_OBJECT** — `var hubHost = new GameObject("__RECOVERY_HOME_HUB_BIND_TEST");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:94` **NEW_GAME_OBJECT** — `var earlyHost = new GameObject("__RECOVERY_EARLY_XRI_MANAGER");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:112` **NEW_GAME_OBJECT** — `var replacementHost = new GameObject("__RECOVERY_REPLACEMENT_XRI_MANAGER");`

### `Ziptide.Tests.PlayMode.RecoveryInputSessionGuardTests` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:152` **NEW_GAME_OBJECT** — `var host = new GameObject(name);`

### `Ziptide.Tests.PlayMode.RecoveryPresentationGuardTests` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryPresentationGuardTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryPresentationGuardTests.cs:167` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryPresentationGuardTests.cs:175` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryPresentationGuardTests.cs:184` **TEXTMESH_COMPONENT** — `TextMesh text = go.AddComponent<TextMesh>();`

### `Ziptide.Tests.PlayMode.RecoveryRenderSnapshotTests` — 6 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:68` **NEW_GAME_OBJECT** — `var cameraHost = new GameObject("__RECOVERY_SNAPSHOT_CAMERA");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:215` **CREATE_PRIMITIVE** — `GameObject go = GameObject.CreatePrimitive(type);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:223` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:224` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Unlit/Color");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:225` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Standard");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRenderSnapshotTests.cs:227` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader) { name = name + "_Material" };`

### `Ziptide.Tests.PlayMode.RecoveryRuntimeArtifactGuardTests` — 3 signal(s)

- Codes: `CANVAS_COMPONENT`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeArtifactGuardTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeArtifactGuardTests.cs:33` **NEW_GAME_OBJECT** — `var debug = new GameObject("Ziptide_DebugHUD");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeArtifactGuardTests.cs:34` **CANVAS_COMPONENT** — `debug.AddComponent<Canvas>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeArtifactGuardTests.cs:35` **NEW_GAME_OBJECT** — `var photon = new GameObject("PhotonMono");`

### `Ziptide.Tests.PlayMode.RecoveryRuntimeCensusTests` — 4 signal(s)

- Codes: `CANVAS_COMPONENT`, `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs:49` **CANVAS_COMPONENT** — `canvasHost.AddComponent<Canvas>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs:52` **TEXTMESH_COMPONENT** — `textHost.AddComponent<TextMesh>().text = "CENSUS";`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs:57` **CREATE_PRIMITIVE** — `GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs:143` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`

### `Ziptide.Tests.PlayMode.RecoverySpawnClearanceAuditTests` — 4 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySpawnClearanceAuditTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySpawnClearanceAuditTests.cs:26` **NEW_GAME_OBJECT** — `var rig = new GameObject("__RECOVERY_SPAWN_RIG");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySpawnClearanceAuditTests.cs:28` **NEW_GAME_OBJECT** — `var cameraHost = new GameObject("__RECOVERY_SPAWN_HEAD");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySpawnClearanceAuditTests.cs:34` **CREATE_PRIMITIVE** — `var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySpawnClearanceAuditTests.cs:53` **CREATE_PRIMITIVE** — `var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);`

### `Ziptide.Tests.PlayMode.RecoveryTestRig` — 9 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:42` **NEW_GAME_OBJECT** — `Root = new GameObject(RootName);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:49` **NEW_GAME_OBJECT** — `var managerHost = new GameObject("InteractionManager");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:62` **NEW_GAME_OBJECT** — `var inputHost = new GameObject("InputActionManager");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:70` **NEW_GAME_OBJECT** — `var offset = new GameObject("Camera Offset");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:74` **NEW_GAME_OBJECT** — `var head = new GameObject("Main Camera");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:92` **CREATE_PRIMITIVE** — `Floor = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:99` **NEW_GAME_OBJECT** — `var spawn = new GameObject(SpawnName);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:126` **NEW_GAME_OBJECT** — `var controller = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:135` **NEW_GAME_OBJECT** — `var rayHost = new GameObject(name);`

### `Ziptide.Tests.PlayMode.RecoveryTestRigTests` — 2 signal(s)

- Codes: `CREATE_PRIMITIVE`, `XR_INTERACTABLE_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs:78` **CREATE_PRIMITIVE** — `var target = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs:85` **XR_INTERACTABLE_COMPONENT** — `var interactable = target.AddComponent<XRSimpleInteractable>();`

### `Ziptide.Tests.PlayMode.RecoveryUiSpatialAuditTests` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `TEXTMESH_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryUiSpatialAuditTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryUiSpatialAuditTests.cs:27` **NEW_GAME_OBJECT** — `var cameraHost = new GameObject("__RECOVERY_UI_CAMERA");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryUiSpatialAuditTests.cs:74` **NEW_GAME_OBJECT** — `var go = new GameObject("__RECOVERY_UI_" + value.Replace(' ', '_'));`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryUiSpatialAuditTests.cs:78` **TEXTMESH_COMPONENT** — `TextMesh text = go.AddComponent<TextMesh>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryUiSpatialAuditTests.cs:96` **NEW_GAME_OBJECT** — `var go = new GameObject("__RECOVERY_UI_TMP_PROBE");`

### `Ziptide.Visuals.ForgeCreatureVisualApplier` — 2 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureVisualApplier.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureVisualApplier.cs:31` **NEW_GAME_OBJECT** — `var vis = new GameObject(VisualChildName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureVisualApplier.cs:86` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(baseMat) { name = baseMat.name + "_Eye" };`

### `Ziptide.Visuals.ForgeMaterials` — 2 signal(s)

- Codes: `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMaterials.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMaterials.cs:20` **SHADER_FIND** — `var shader = Shader.Find(URPLitShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMaterials.cs:22` **RUNTIME_MATERIAL_CREATE** — `var mat = new Material(shader) { name = "ForgeMat_" + ColorUtility.ToHtmlStringRGB(color) };`

### `Ziptide.Visuals.ForgeSkinnedBuilder` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs:42` **NEW_GAME_OBJECT** — `var rootGo = new GameObject("Skeleton_" + (body != null ? body.bodyId : "null"));`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs:210` **NEW_GAME_OBJECT** — `var tmp = new GameObject("__synthParts");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs:251` **NEW_GAME_OBJECT** — `var boneGo = new GameObject("Bone_" + tag + "_" + s);`

### `Ziptide.Visuals.ForgeVisualApplier` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeVisualApplier.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeVisualApplier.cs:49` **NEW_GAME_OBJECT** — `GameObject vis = existing != null ? existing.gameObject : new GameObject(VisualChildName);`

### `Ziptide.Visuals.GroundShadow` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Grounding/GroundShadow.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Grounding/GroundShadow.cs:27` **NEW_GAME_OBJECT** — `var go = new GameObject(ChildName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Grounding/GroundShadow.cs:44` **SHADER_FIND** — `Shader shader = Shader.Find(AlphaShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Grounding/GroundShadow.cs:63` **RUNTIME_MATERIAL_CREATE** — `_material = new Material(shader)`

### `Ziptide.Visuals.PracticalLight` — 3 signal(s)

- Codes: `CREATE_PRIMITIVE`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs:108` **CREATE_PRIMITIVE** — `var go = GameObject.CreatePrimitive(PrimitiveType.Quad);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs:134` **SHADER_FIND** — `Shader shader = Shader.Find(AdditiveShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs:166` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader)`

### `Ziptide.Visuals.SkyAtmosphereRig` — 3 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs:362` **NEW_GAME_OBJECT** — `var go = new GameObject(name);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs:380` **SHADER_FIND** — `Shader shader = Shader.Find(shaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs:388` **RUNTIME_MATERIAL_CREATE** — `var material = new Material(shader)`

### `Ziptide.Visuals.SkyPlanetRig` — 7 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:68` **NEW_GAME_OBJECT** — `var go = new GameObject("SkyVistaRig");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:95` **CREATE_PRIMITIVE** — `_skyRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:104` **SHADER_FIND** — `Shader unlit = Shader.Find(URPUnlitShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:106` **RUNTIME_MATERIAL_CREATE** — `_skyMaterial = new Material(unlit);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:124` **CREATE_PRIMITIVE** — `_planetRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:129` **SHADER_FIND** — `Shader unlit = Shader.Find(URPUnlitShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:131` **RUNTIME_MATERIAL_CREATE** — `_planetMaterial = new Material(unlit);`

### `Ziptide.Visuals.SkyVistaRig` — 8 signal(s)

- Codes: `CREATE_PRIMITIVE`, `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:72` **NEW_GAME_OBJECT** — `_gradeVolume = new GameObject("SkyGradeVolume");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:127` **NEW_GAME_OBJECT** — `var go = new GameObject("SkyAtmosphere");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:149` **CREATE_PRIMITIVE** — `_domeRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:158` **SHADER_FIND** — `Shader unlit = Shader.Find(URPUnlitShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:160` **RUNTIME_MATERIAL_CREATE** — `_domeMaterial = new Material(unlit)`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:211` **SHADER_FIND** — `Shader unlit = Shader.Find(URPUnlitShaderName);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:221` **CREATE_PRIMITIVE** — `inst.go = GameObject.CreatePrimitive(PrimitiveType.Sphere);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:236` **RUNTIME_MATERIAL_CREATE** — `inst.material = new Material(unlit)`

### `Ziptide.Visuals.VfxFactory` — 6 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:68` **NEW_GAME_OBJECT** — `var root = new GameObject(FactoryObjectName)`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:181` **NEW_GAME_OBJECT** — `var go = new GameObject("VFX_POOL_" + kind + "_" + _all.Count);`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:337` **SHADER_FIND** — `Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:338` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:339` **SHADER_FIND** — `if (shader == null) shader = Shader.Find("Sprites/Default");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/VfxFactory.cs:347` **RUNTIME_MATERIAL_CREATE** — `_sharedMaterial = new Material(shader)`

### `Ziptide.Visuals.ZiptideWater` — 4 signal(s)

- Codes: `NEW_GAME_OBJECT`, `RUNTIME_MATERIAL_CREATE`, `SHADER_FIND`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs:50` **SHADER_FIND** — `var lit = Shader.Find("Universal Render Pipeline/Lit");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs:65` **RUNTIME_MATERIAL_CREATE** — `_waterMat = new Material(lit) { name = "ZiptideWater" };`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs:95` **NEW_GAME_OBJECT** — `var go = new GameObject("Foam");`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs:113` **RUNTIME_MATERIAL_CREATE** — `_foamMat = new Material(lit) { name = "ZiptideWaterFoam" };`

### `ZiptideNet.NetBootstrap` — 1 signal(s)

- Codes: `NEW_GAME_OBJECT`
- Paths: `Ziptide/Assets/ZiptideNet/NetBootstrap.cs`
  - `Ziptide/Assets/ZiptideNet/NetBootstrap.cs:41` **NEW_GAME_OBJECT** — `_launcherGo = new GameObject("__PhotonPvpLauncher");`


## XRI ownership

### `Ziptide.Editor.Audit.WorldAuditRunner` — 1 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs:161` **XRI_MANAGER_LOOKUP** — `var managers = Object.FindObjectsOfType<XRInteractionManager>();`

### `Ziptide.Editor.Setup.EnsureLocomotionRig` — 1 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:248` **XRI_MANAGER_LOOKUP** — `var interactionManager = Object.FindObjectOfType<XRInteractionManager>();`

### `Ziptide.Editor.Setup.SetupMilestoneAScene` — 3 signal(s)

- Codes: `XRI_MANAGER_CREATE`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:69` **XRI_MANAGER_LOOKUP** — `if (Object.FindObjectOfType<XRInteractionManager>() == null)`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:72` **XRI_MANAGER_CREATE** — `managerGo.AddComponent<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:75` **XRI_MANAGER_LOOKUP** — `managerGo = Object.FindObjectOfType<XRInteractionManager>().gameObject;`

### `Ziptide.Editor.Validation.XRGrabReadiness` — 1 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs`
  - `Ziptide/Assets/Ziptide/Editor/Validation/XRGrabReadiness.cs:48` **XRI_MANAGER_LOOKUP** — `var interactionManager = Object.FindObjectOfType<XRInteractionManager>();`

### `Ziptide.Gameplay.ArenaLobbyBoard` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:297` **XRI_MANAGER_LOOKUP** — `var mgr = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:298` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.BootHoldState` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_CREATE`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:140` **XRI_MANAGER_LOOKUP** — `foreach (var m in FindObjectsOfType<XRInteractionManager>(true))`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:402` **XRI_MANAGER_LOOKUP** — `var allManagers = FindObjectsOfType<XRInteractionManager>(true);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:451` **XRI_MANAGER_CREATE** — `_xriManager = go.AddComponent<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:478` **XRI_MANAGER_ASSIGN** — `i.interactionManager = _xriManager;`

### `Ziptide.Gameplay.BuildSocketRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:60` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:61` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.ChoiceStation` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:81` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:82` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:127` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:128` **XRI_MANAGER_ASSIGN** — `if (mgr != null) { interactable.interactionManager = mgr; yield break; }`

### `Ziptide.Gameplay.CollectibleRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:53` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:54` **XRI_MANAGER_ASSIGN** — `if (mgr != null) grab.interactionManager = mgr;`

### `Ziptide.Gameplay.ComfortConsoleRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:91` **XRI_MANAGER_LOOKUP** — `var manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:92` **XRI_MANAGER_ASSIGN** — `if (manager != null) interactable.interactionManager = manager;`

### `Ziptide.Gameplay.DevTools.DevWarpBoard` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:233` **XRI_MANAGER_LOOKUP** — `var mgr = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:234` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:261` **XRI_MANAGER_LOOKUP** — `var mgr = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:262` **XRI_MANAGER_ASSIGN** — `if (mgr != null) { interactable.interactionManager = mgr; yield break; }`

### `Ziptide.Gameplay.FirstDestinationHelmRuntime` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:67` **XRI_MANAGER_LOOKUP** — `var manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:68` **XRI_MANAGER_ASSIGN** — `if (manager != null) interactable.interactionManager = manager;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:125` **XRI_MANAGER_LOOKUP** — `var manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:126` **XRI_MANAGER_ASSIGN** — `if (manager != null) grab.interactionManager = manager;`

### `Ziptide.Gameplay.GardenPlotRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:63` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:64` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.HammerTool` — 3 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:41` **XRI_MANAGER_ASSIGN** — `if (_grab != null && _grab.interactionManager == null)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:43` **XRI_MANAGER_LOOKUP** — `var mgr = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs:44` **XRI_MANAGER_ASSIGN** — `if (mgr != null) _grab.interactionManager = mgr;`

### `Ziptide.Gameplay.HomeHubChoice` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:335` **XRI_MANAGER_LOOKUP** — `if (manager == null) manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:341` **XRI_MANAGER_ASSIGN** — `interactable.interactionManager = manager;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:379` **XRI_MANAGER_LOOKUP** — `XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:383` **XRI_MANAGER_ASSIGN** — `interactable.interactionManager = manager;`

### `Ziptide.Gameplay.InventoryState` — 3 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryState.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryState.cs:161` **XRI_MANAGER_LOOKUP** — `if (manager == null) manager = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryState.cs:167` **XRI_MANAGER_ASSIGN** — `socket.interactionManager = manager;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryState.cs:168` **XRI_MANAGER_ASSIGN** — `grab.interactionManager = manager;`

### `Ziptide.Gameplay.MiningRigRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:93` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:94` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.NestRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:72` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:73` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.PlayerInputSessionGuard` — 1 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:173` **XRI_MANAGER_LOOKUP** — `UnityEngine.Object.FindObjectsOfType<XRInteractionManager>(true);`

### `Ziptide.Gameplay.PlayerMenuRuntime` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:194` **XRI_MANAGER_LOOKUP** — `XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:195` **XRI_MANAGER_ASSIGN** — `if (manager != null) interactable.interactionManager = manager;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:204` **XRI_MANAGER_LOOKUP** — `XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:216` **XRI_MANAGER_ASSIGN** — `interactable.interactionManager = manager;`

### `Ziptide.Gameplay.QuartersRoom` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:260` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:261` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.QuickSwap` — 1 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/QuickSwap.cs:75` **XRI_MANAGER_LOOKUP** — `if (_mgr == null) _mgr = FindObjectOfType<XRInteractionManager>();`

### `Ziptide.Gameplay.RepairableMachine` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:442` **XRI_MANAGER_LOOKUP** — `XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:443` **XRI_MANAGER_ASSIGN** — `if (manager != null) interactable.interactionManager = manager;`

### `Ziptide.Gameplay.ShipBoardingStation` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:332` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:333` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.ShipCastOffRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:122` **XRI_MANAGER_LOOKUP** — `var mgr = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:123` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Gameplay.TransmissionConsole` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:49` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:50` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:87` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:88` **XRI_MANAGER_ASSIGN** — `if (mgr != null) { interactable.interactionManager = mgr; yield break; }`

### `Ziptide.Gameplay.TravelCoordinator` — 2 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:274` **XRI_MANAGER_LOOKUP** — `var managers = Object.FindObjectsOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:349` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`

### `Ziptide.Gameplay.WateringCanRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:45` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/WateringCanRuntime.cs:46` **XRI_MANAGER_ASSIGN** — `if (mgr != null) grab.interactionManager = mgr;`

### `Ziptide.Gameplay.WorldDiscoveryNodeRuntime` — 3 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/WorldDiscoveryNodeRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/WorldDiscoveryNodeRuntime.cs:43` **XRI_MANAGER_ASSIGN** — `if (_interactable.interactionManager == null)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/WorldDiscoveryNodeRuntime.cs:44` **XRI_MANAGER_ASSIGN** — `_interactable.interactionManager = FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/WorldDiscoveryNodeRuntime.cs:44` **XRI_MANAGER_LOOKUP** — `_interactable.interactionManager = FindObjectOfType<XRInteractionManager>();`

### `Ziptide.Gameplay.WorldTravelStation` — 4 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:119` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:122` **XRI_MANAGER_ASSIGN** — `interactable.interactionManager = mgr;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:264` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:267` **XRI_MANAGER_ASSIGN** — `interactable.interactionManager = mgr;`

### `Ziptide.Ship.ShipFlightRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:245` **XRI_MANAGER_LOOKUP** — `var mgr = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:246` **XRI_MANAGER_ASSIGN** — `if (mgr != null) interactable.interactionManager = mgr;`

### `Ziptide.Ship.VehicleRuntime` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:279` **XRI_MANAGER_LOOKUP** — `XRInteractionManager manager = Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:280` **XRI_MANAGER_ASSIGN** — `if (manager != null) interactable.interactionManager = manager;`

### `Ziptide.Tests.PlayMode.RecoveryActualRigControllerSimulation` — 1 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:548` **XRI_MANAGER_ASSIGN** — `ray.interactionManager = canonicalManager;`

### `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` — 1 signal(s)

- Codes: `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:120` **XRI_MANAGER_LOOKUP** — `XRInteractionManager manager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();`

### `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:89` **XRI_MANAGER_LOOKUP** — `manager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:132` **XRI_MANAGER_ASSIGN** — `(settings.interactionManager == null || newGame.interactionManager == null); frame++)`

### `Ziptide.Tests.PlayMode.RecoveryHomeHubBindingTests` — 5 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_CREATE`, `XRI_MANAGER_LOOKUP`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:45` **XRI_MANAGER_LOOKUP** — `Assert.IsNull(UnityEngine.Object.FindObjectOfType<XRInteractionManager>(),`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:92` **XRI_MANAGER_ASSIGN** — `if (interactable.interactionManager == null)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:96` **XRI_MANAGER_CREATE** — `earlyHost.AddComponent<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:97` **XRI_MANAGER_ASSIGN** — `for (int frame = 0; frame < 10 && interactable.interactionManager == null; frame++)`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:114` **XRI_MANAGER_CREATE** — `var replacement = replacementHost.AddComponent<XRInteractionManager>();`

### `Ziptide.Tests.PlayMode.RecoveryInputSessionGuardTests` — 1 signal(s)

- Codes: `XRI_MANAGER_CREATE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:51` **XRI_MANAGER_CREATE** — `primaryHost.AddComponent<XRInteractionManager>();`

### `Ziptide.Tests.PlayMode.RecoveryRuntimeCensusTests` — 1 signal(s)

- Codes: `XRI_MANAGER_CREATE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeCensusTests.cs:109` **XRI_MANAGER_CREATE** — `extra.AddComponent<XRInteractionManager>();`

### `Ziptide.Tests.PlayMode.RecoveryTestRig` — 2 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`, `XRI_MANAGER_CREATE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:51` **XRI_MANAGER_CREATE** — `InteractionManager = managerHost.AddComponent<XRInteractionManager>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:141` **XRI_MANAGER_ASSIGN** — `ray.interactionManager = InteractionManager;`

### `Ziptide.Tests.PlayMode.RecoveryTestRigTests` — 1 signal(s)

- Codes: `XRI_MANAGER_ASSIGN`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs:86` **XRI_MANAGER_ASSIGN** — `interactable.interactionManager = _fixture.InteractionManager;`


## events & Save

### `Ziptide.Content.BuildMineStatus` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Economy/MiningService.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/MiningService.cs:49` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile, WorldState world,`

### `Ziptide.Content.JobDefinition` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobDefinition.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobDefinition.cs:24` **PLAYER_PROFILE_REFERENCE** — `[Tooltip("Optional PlayerProfile flag set when the job completes (e.g. 'toxiccity_complete'). " +`

### `Ziptide.Content.JobRewards` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobRewards.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobRewards.cs:6` **PLAYER_PROFILE_REFERENCE** — `/// Pays out a job's completion reward into a <see cref="PlayerProfile"/>: grants each resource in`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobRewards.cs:18` **PLAYER_PROFILE_REFERENCE** — `public static void Grant(JobDefinition job, PlayerProfile profile)`

### `Ziptide.Content.ProductionGraph` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Economy/ProductionGraph.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/ProductionGraph.cs:61` **PLAYER_PROFILE_REFERENCE** — `public static int Tick(List<MachineNodeState> nodes, PlayerProfile profile, string worldId,`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/ProductionGraph.cs:101` **PLAYER_PROFILE_REFERENCE** — `public static int CatchUp(List<MachineNodeState> nodes, PlayerProfile profile, string worldId,`

### `Ziptide.Content.RecipeService` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Economy/RecipeService.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/RecipeService.cs:7` **PLAYER_PROFILE_REFERENCE** — `/// <see cref="PlayerProfile"/> inventory — the shared primitive behind build / repair / craft.`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/RecipeService.cs:13` **PLAYER_PROFILE_REFERENCE** — `public static bool CanAfford(PlayerProfile profile, RecipeDefinition recipe)`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/RecipeService.cs:28` **PLAYER_PROFILE_REFERENCE** — `public static bool TrySpend(PlayerProfile profile, RecipeDefinition recipe)`

### `Ziptide.Content.ResourceNode` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Economy/ResourceNode.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/ResourceNode.cs:30` **PLAYER_PROFILE_REFERENCE** — `public HarvestResult Harvest(ToolDefinition tool, PlayerProfile profile)`

### `Ziptide.Content.TendStatus` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Economy/GardenService.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Economy/GardenService.cs:151` **PLAYER_PROFILE_REFERENCE** — `public static HarvestPlantResult Harvest(PlayerProfile profile, PlotState plot, PlantDefinition plant, ToolDefinition tool, long nowUnix)`

### `Ziptide.Content.WorldGating` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:7` **PLAYER_PROFILE_REFERENCE** — `/// <see cref="PlayerProfile"/>: whether its required flags are met, and granting its world-level`
  - `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:19` **PLAYER_PROFILE_REFERENCE** — `public static bool MeetsRequirements(WorldPackDefinition pack, PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:37` **PLAYER_PROFILE_REFERENCE** — `public static string FirstMissingRequirement(WorldPackDefinition pack, PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:54` **PLAYER_PROFILE_REFERENCE** — `public static int GrantWorldFlags(WorldPackDefinition pack, PlayerProfile profile)`

### `Ziptide.Core.ComfortPreset` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/ComfortSettings.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/ComfortSettings.cs:62` **PLAYER_PROFILE_REFERENCE** — `/// It intentionally never references PlayerProfile: comfort belongs to the player's body/device`

### `Ziptide.Core.CosmeticLocker` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:17` **PLAYER_PROFILE_REFERENCE** — `public static void Equip(PlayerProfile profile, string targetKey, string cosmeticId)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:25` **PLAYER_PROFILE_REFERENCE** — `public static void Unequip(PlayerProfile profile, string targetKey)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:35` **PLAYER_PROFILE_REFERENCE** — `public static string GetEquipped(PlayerProfile profile, string targetKey)`

### `Ziptide.Core.LedgerEntry` — 6 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:42` **PLAYER_PROFILE_REFERENCE** — `public static void Append(PlayerProfile profile, LedgerEntry entry)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:51` **PLAYER_PROFILE_REFERENCE** — `public static double SumFor(PlayerProfile profile, string resourceId)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:61` **PLAYER_PROFILE_REFERENCE** — `public static string Explain(PlayerProfile profile, string resourceId, int maxLines = 20)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:84` **PLAYER_PROFILE_REFERENCE** — `/// Direct <see cref="PlayerProfile.AddResource"/> calls outside this class (and migration/tests)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:90` **PLAYER_PROFILE_REFERENCE** — `public static double Grant(PlayerProfile profile, string source, string resourceId, double amount,`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:105` **PLAYER_PROFILE_REFERENCE** — `public static bool TrySpend(PlayerProfile profile, string source, string resourceId, double amount,`

### `Ziptide.Core.PlayerProfile` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/PlayerProfile.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/PlayerProfile.cs:13` **PLAYER_PROFILE_REFERENCE** — `public class PlayerProfile`

### `Ziptide.Core.ProfileSerializer` — 12 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:6` **PLAYER_PROFILE_REFERENCE** — `/// Pure (no file IO) serialize / deserialize / migrate for <see cref="PlayerProfile"/>.`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:7` **SAVE_SYSTEM_REFERENCE** — `/// Kept separate from SaveSystem so it is fully unit-testable in EditMode — no headset, no disk.`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:12` **PLAYER_PROFILE_REFERENCE** — `public static string Serialize(PlayerProfile profile, bool prettyPrint = false)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:22` **PLAYER_PROFILE_REFERENCE** — `public static PlayerProfile Deserialize(string json)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:30` **PLAYER_PROFILE_REFERENCE** — `public static bool TryDeserialize(string json, out PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:34` **PLAYER_PROFILE_REFERENCE** — `try { profile = JsonUtility.FromJson<PlayerProfile>(json); }`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:43` **PLAYER_PROFILE_REFERENCE** — `public static PlayerProfile NewProfile()`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:46` **PLAYER_PROFILE_REFERENCE** — `return new PlayerProfile`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:48` **PLAYER_PROFILE_REFERENCE** — `schemaVersion = PlayerProfile.CurrentSchemaVersion,`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:56` **PLAYER_PROFILE_REFERENCE** — `private static void Migrate(PlayerProfile p)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:79` **PLAYER_PROFILE_REFERENCE** — `if (p.schemaVersion < PlayerProfile.CurrentSchemaVersion)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:80` **PLAYER_PROFILE_REFERENCE** — `p.schemaVersion = PlayerProfile.CurrentSchemaVersion;`

### `Ziptide.Core.RecoveryAutomaticOwnerRegistration` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryAutomaticOwnerCatalog.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryAutomaticOwnerCatalog.cs:52` **SAVE_SYSTEM_REFERENCE** — `Required("SAVE_SYSTEM_BOOTSTRAP", "Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs", "Ziptide.Gameplay.SaveSystem", RecoveryFeatureId.SaveSystem),`

### `Ziptide.Core.RecoveryExposureProfile` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryExposureProfile.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryExposureProfile.cs:49` **SAVE_SYSTEM_REFERENCE** — `RecoveryFeatureId.SaveSystem,`

### `Ziptide.Core.RecoveryFeatureId` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryFeatureId.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryFeatureId.cs:22` **SAVE_SYSTEM_REFERENCE** — `SaveSystem = 13,`

### `Ziptide.Core.RillMemoryState` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:30` **PLAYER_PROFILE_REFERENCE** — `public static RillMemoryState Compute(PlayerProfile profile)`

### `Ziptide.Core.ShipLocker` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:7` **PLAYER_PROFILE_REFERENCE** — `/// idiom exactly: string entries "SHIP_EQUIP:&lt;slot&gt;=&lt;id&gt;" in PlayerProfile.flags, so the`
  - `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:15` **PLAYER_PROFILE_REFERENCE** — `public static void Equip(PlayerProfile profile, string slot, string id)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:24` **PLAYER_PROFILE_REFERENCE** — `public static string GetEquipped(PlayerProfile profile, string slot)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:36` **PLAYER_PROFILE_REFERENCE** — `public static List<string> EquippedModules(PlayerProfile profile, IEnumerable<string> slotIds)`

### `Ziptide.Core.SignalState` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs:4` **PLAYER_PROFILE_REFERENCE** — `/// Pure derivation of the Signal tier from the SIGNAL_* flags on a <see cref="PlayerProfile"/>.`
  - `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs:20` **PLAYER_PROFILE_REFERENCE** — `public static int Tier(PlayerProfile profile)`

### `Ziptide.Core.TransmissionProgress` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:5` **PLAYER_PROFILE_REFERENCE** — `/// <see cref="PlayerProfile"/> (spec: docs/storyboard/WORLD_DATA.md §3 / THE_TRANSMISSION.md §3).`
  - `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:23` **PLAYER_PROFILE_REFERENCE** — `public static int ComputeTier(PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:47` **PLAYER_PROFILE_REFERENCE** — `public static int SyncClarityFlags(PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:60` **PLAYER_PROFILE_REFERENCE** — `private static int SetIfMissing(PlayerProfile profile, string flag)`

### `Ziptide.Core.WorldResolveResult` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ProfileEconomy.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ProfileEconomy.cs:62` **PLAYER_PROFILE_REFERENCE** — `public static WorldResolveResult EnterWorld(PlayerProfile profile, string worldId, long nowUnix,`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ProfileEconomy.cs:72` **PLAYER_PROFILE_REFERENCE** — `public static double CollectMine(PlayerProfile profile, MineState mine)`

### `Ziptide.Gameplay.ArenaLobbyBoard` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:248` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.BeltCellSpec` — 9 signal(s)

- Codes: `AUTOSAVE_CALL`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:24` **SAVE_SYSTEM_REFERENCE** — `/// <see cref="LedgerSource.Factory"/>) onto the live SaveSystem profile.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:117` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:169` **AUTOSAVE_CALL** — `if (autosave) SaveSystem.AutosaveNow("belt_edit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:169` **SAVE_SYSTEM_REFERENCE** — `if (autosave) SaveSystem.AutosaveNow("belt_edit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:195` **AUTOSAVE_CALL** — `SaveSystem.AutosaveNow("belt_stamp");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:195` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.AutosaveNow("belt_stamp");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:207` **AUTOSAVE_CALL** — `SaveSystem.AutosaveNow("belt_edit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:207` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.AutosaveNow("belt_edit");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:618` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.BeltMinePortRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:52` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.BuildSocketRuntime` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:46` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:88` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.ChoiceStation` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:34` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:112` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.CollectibleRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:118` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.ComfortConsoleRuntime` — 3 signal(s)

- Codes: `EVENT_DECLARATION`, `PLAYER_PROFILE_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:11` **PLAYER_PROFILE_REFERENCE** — `/// the rig, writes PlayerProfile, creates a second vignette, or owns traversal physics.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:15` **EVENT_DECLARATION** — `public static event Action<ComfortPreset> PresetConfirmed;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:15` **STATIC_EVENT_DECLARATION** — `public static event Action<ComfortPreset> PresetConfirmed;`

### `Ziptide.Gameplay.ComfortVignette` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:13` **SAVE_SYSTEM_REFERENCE** — `/// Self-bootstrapped like SaveSystem (no scene edit); strength is a device-level setting in`

### `Ziptide.Gameplay.ConquestMissionRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:361` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.ConquestTableRuntime` — 4 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:76` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:84` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:104` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:414` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.CreatureRuntime` — 3 signal(s)

- Codes: `EVENT_DECLARATION`, `SAVE_SYSTEM_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:64` **EVENT_DECLARATION** — `public static event System.Action<CreatureRuntime> CreatureDisabled;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:64` **STATIC_EVENT_DECLARATION** — `public static event System.Action<CreatureRuntime> CreatureDisabled;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:141` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.CreditsHud` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs:82` **SAVE_SYSTEM_REFERENCE** — `var save = SaveSystem.Instance;`

### `Ziptide.Gameplay.DroneRuntime` — 2 signal(s)

- Codes: `EVENT_DECLARATION`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:47` **EVENT_DECLARATION** — `public static event System.Action<DroneRuntime> OnDroneDisabled;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:47` **STATIC_EVENT_DECLARATION** — `public static event System.Action<DroneRuntime> OnDroneDisabled;`

### `Ziptide.Gameplay.EcologyDirector` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:93` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.FirstDestinationHelmRuntime` — 4 signal(s)

- Codes: `EVENT_DECLARATION`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:14` **EVENT_DECLARATION** — `public static event Action<string> FirstDestinationSelected;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:14` **STATIC_EVENT_DECLARATION** — `public static event Action<string> FirstDestinationSelected;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:111` **EVENT_DECLARATION** — `public static event Action<string> NamedBunkObjectGrabbed;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:111` **STATIC_EVENT_DECLARATION** — `public static event Action<string> NamedBunkObjectGrabbed;`

### `Ziptide.Gameplay.FirstHourDirector` — 7 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:126` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:126` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:254` **PLAYER_PROFILE_REFERENCE** — `private void OnNewGameProfile(PlayerProfile profile) => Accept("NEW_GAME_PROFILE_CREATED");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:364` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:364` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:371` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:371` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.FirstHourHolsterSignal` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/FirstHourHolsterSignal.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/FirstHourHolsterSignal.cs:15` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile,`

### `Ziptide.Gameplay.FirstHourW001Orchestrator` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:158` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:158` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.GardenPlotRuntime` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:89` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:121` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.HangarBayRuntime` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:96` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:108` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.HolsterSocketInteractor` — 4 signal(s)

- Codes: `EVENT_DECLARATION`, `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:30` **EVENT_DECLARATION** — `public static event Action<string> ItemHolstered;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:30` **STATIC_EVENT_DECLARATION** — `public static event Action<string> ItemHolstered;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:173` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:173` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.HomeHubChoice` — 16 signal(s)

- Codes: `EVENT_DECLARATION`, `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:72` **SAVE_SYSTEM_REFERENCE** — `/// all persistence to SaveSystem and all scene change to the callback supplied by BootLoader.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:110` **EVENT_DECLARATION** — `public static event Action<bool> BootPresentationReady;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:110` **STATIC_EVENT_DECLARATION** — `public static event Action<bool> BootPresentationReady;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:111` **EVENT_DECLARATION** — `public static event Action<PlayerProfile> NewGameProfileCreated;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:111` **PLAYER_PROFILE_REFERENCE** — `public static event Action<PlayerProfile> NewGameProfileCreated;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:111` **STATIC_EVENT_DECLARATION** — `public static event Action<PlayerProfile> NewGameProfileCreated;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:112` **EVENT_DECLARATION** — `public static event Action<HomeHubChoice> ChoiceSelected;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:112` **STATIC_EVENT_DECLARATION** — `public static event Action<HomeHubChoice> ChoiceSelected;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:113` **EVENT_DECLARATION** — `public static event Action SettingsRequested;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:113` **STATIC_EVENT_DECLARATION** — `public static event Action SettingsRequested;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:140` **SAVE_SYSTEM_REFERENCE** — `bool canContinue = SaveSystem.HasExistingProfile;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:225` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.Instance == null)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:251` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance.StartNewProfile();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:251` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance.StartNewProfile();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:256` **SAVE_SYSTEM_REFERENCE** — `// Load remains SaveSystem's sole profile recovery/migration path.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:257` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.Instance.Load();`

### `Ziptide.Gameplay.ItemFactory` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:87` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.JobDirector` — 3 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:44` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:371` **SAVE_SYSTEM_REFERENCE** — `// JobRewards.Grant + the self-bootstrapping SaveSystem (so no _Boot edit needed). This is the`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:373` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.JobRuntime` — 2 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:13` **EVENT_DECLARATION** — `public event Action StepChanged;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:14` **EVENT_DECLARATION** — `public event Action JobCompleted;`

### `Ziptide.Gameplay.LifecycleState` — 6 signal(s)

- Codes: `AUTOSAVE_CALL`, `EVENT_DECLARATION`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:66` **SAVE_SYSTEM_REFERENCE** — `/// wires real detection. SaveSystem's own pause-save is intentionally KEPT beneath this`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:68` **SAVE_SYSTEM_REFERENCE** — `/// Always-required safety class (like SaveSystem) — deliberately NOT RecoveryRuntimeGate-gated;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:76` **EVENT_DECLARATION** — `public event System.Action<LifecycleState, LifecycleState> StateChanged;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:137` **SAVE_SYSTEM_REFERENCE** — `// (The OS-pause path is already covered by SaveSystem's own hook — layered defense.)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:139` **AUTOSAVE_CALL** — `SaveSystem.AutosaveNow("system_overlay");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:139` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.AutosaveNow("system_overlay");`

### `Ziptide.Gameplay.MiningRigRuntime` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:40` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:139` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.NestRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:125` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.PhotoCaptureCamera` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs:97` **SAVE_SYSTEM_REFERENCE** — `SaveSystem save = SaveSystem.Instance;`

### `Ziptide.Gameplay.PlayerStunReceiver` — 2 signal(s)

- Codes: `EVENT_DECLARATION`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:16` **EVENT_DECLARATION** — `public static event System.Action OnPlayerStunned;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:16` **STATIC_EVENT_DECLARATION** — `public static event System.Action OnPlayerStunned;`

### `Ziptide.Gameplay.ProximityTravelTrigger` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ProximityTravelTrigger.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ProximityTravelTrigger.cs:46` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.PvpMatchDirector` — 3 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:22` **EVENT_DECLARATION** — `public event System.Action<int, int> KillScored;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:24` **EVENT_DECLARATION** — `public event System.Action<int> MatchEnded;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:26` **EVENT_DECLARATION** — `public event System.Action MatchRestarted;`

### `Ziptide.Gameplay.PvpProgressionRuntime` — 4 signal(s)

- Codes: `AUTOSAVE_CALL`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:10` **SAVE_SYSTEM_REFERENCE** — `/// like SaveSystem (no scene edit, so every already-committed arena gets it): on each scene`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:93` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:143` **AUTOSAVE_CALL** — `SaveSystem.AutosaveNow("pvp_match");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:143` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.AutosaveNow("pvp_match");`

### `Ziptide.Gameplay.QuartersPhotoWall` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:25` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:25` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.QuartersRoom` — 3 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:143` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:188` **SAVE_SYSTEM_REFERENCE** — `var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:219` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.ReleaseFeel` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ReleaseFeel.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ReleaseFeel.cs:128` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.RepairableMachine` — 1 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:31` **EVENT_DECLARATION** — `public event System.Action<RepairStage> StageChanged;`

### `Ziptide.Gameplay.RillCompanion` — 5 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:97` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:132` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:146` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:175` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:290` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.SalvageCacheRuntime` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:18` **SAVE_SYSTEM_REFERENCE** — `/// SaveSystem profile, which travel-autosave persists.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:84` **SAVE_SYSTEM_REFERENCE** — `double granted = GrantTo(SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null,`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:93` **PLAYER_PROFILE_REFERENCE** — `public static double GrantTo(PlayerProfile profile, string resource, double pay)`

### `Ziptide.Gameplay.SaveSystem` — 10 signal(s)

- Codes: `AUTOSAVE_CALL`, `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:8` **PLAYER_PROFILE_REFERENCE** — `/// Owns the live <see cref="PlayerProfile"/> and persists it to disk as JSON in`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:17` **SAVE_SYSTEM_REFERENCE** — `public class SaveSystem : MonoBehaviour`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:19` **SAVE_SYSTEM_REFERENCE** — `public static SaveSystem Instance { get; private set; }`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:22` **SAVE_SYSTEM_REFERENCE** — `private const string GoName = "SaveSystem";`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:24` **PLAYER_PROFILE_REFERENCE** — `public PlayerProfile Profile { get; private set; }`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:47` **SAVE_SYSTEM_REFERENCE** — `/// The Awake dup-guard makes this safe even if SaveSystem is later added to _Boot. Lets the`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:48` **SAVE_SYSTEM_REFERENCE** — `/// economy/bounty payout use SaveSystem.Instance.Profile from anywhere.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:55` **SAVE_SYSTEM_REFERENCE** — `go.AddComponent<SaveSystem>();`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:94` **PLAYER_PROFILE_REFERENCE** — `public PlayerProfile StartNewProfile()`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:123` **AUTOSAVE_CALL** — `public static void AutosaveNow(string reason)`

### `Ziptide.Gameplay.ShipBoardingStation` — 4 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:81` **SAVE_SYSTEM_REFERENCE** — `var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:111` **SAVE_SYSTEM_REFERENCE** — `var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:161` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:181` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null) ?? "?")))`

### `Ziptide.Gameplay.ShipCastOffRuntime` — 1 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:39` **EVENT_DECLARATION** — `public event Action<string> DestinationSelected;`

### `Ziptide.Gameplay.ShipRefit` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:33` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:120` **PLAYER_PROFILE_REFERENCE** — `private static void ApplyLivery(GameObject shipRoot, PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:145` **PLAYER_PROFILE_REFERENCE** — `private static void ApplyDecals(Transform root, PlayerProfile profile)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:176` **PLAYER_PROFILE_REFERENCE** — `private static void ApplyNameplate(Transform root, PlayerProfile profile, ShipChassisPreset chassis)`

### `Ziptide.Gameplay.TransmissionConsole` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:69` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.TravelCoordinator` — 6 signal(s)

- Codes: `AUTOSAVE_CALL`, `EVENT_DECLARATION`, `SAVE_SYSTEM_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:38` **EVENT_DECLARATION** — `public static event Action<string> TravelCompleted;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:38` **STATIC_EVENT_DECLARATION** — `public static event Action<string> TravelCompleted;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:119` **AUTOSAVE_CALL** — `SaveSystem.AutosaveNow("travel_fallback");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:119` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.AutosaveNow("travel_fallback");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:187` **AUTOSAVE_CALL** — `SaveSystem.AutosaveNow("travel");`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:187` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.AutosaveNow("travel");`

### `Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter` — 1 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:38` **EVENT_DECLARATION** — `public event Action<string> SignalCompleted;`

### `Ziptide.Gameplay.WardenBehavior` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/WardenBehavior.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/WardenBehavior.cs:43` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Gameplay.WorldRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs:46` **SAVE_SYSTEM_REFERENCE** — `var save = SaveSystem.Instance;`

### `Ziptide.Gameplay.WorldTravelStation` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:64` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:149` **SAVE_SYSTEM_REFERENCE** — `+ " missing=" + (WorldGating.FirstMissingRequirement(pack, SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null) ?? "?"));`

### `Ziptide.Gameplay.WristScanner` — 2 signal(s)

- Codes: `EVENT_DECLARATION`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:43` **EVENT_DECLARATION** — `public static event System.Action<WristScanResult> ScanResultPublished;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:43` **STATIC_EVENT_DECLARATION** — `public static event System.Action<WristScanResult> ScanResultPublished;`

### `Ziptide.Gameplay.ZiplineRuntime` — 2 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:28` **EVENT_DECLARATION** — `public event System.Action RideStarted;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:31` **EVENT_DECLARATION** — `public event System.Action<string, float> RideEnded;`

### `Ziptide.Multiplayer.Conquest.ConquestResource` — 1 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/PlanetNode.cs`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/PlanetNode.cs:14` **PLAYER_PROFILE_REFERENCE** — `/// so the Gameplay-side save can JsonUtility it, same as PlayerProfile.`

### `Ziptide.Multiplayer.LoopbackPvpTransport` — 5 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:17` **EVENT_DECLARATION** — `public event Action<PlayerPoseMsg> OnPose;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:18` **EVENT_DECLARATION** — `public event Action<FireMsg> OnFire;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:19` **EVENT_DECLARATION** — `public event Action<HitMsg> OnHit;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:20` **EVENT_DECLARATION** — `public event Action<ScoreMsg> OnScore;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:21` **EVENT_DECLARATION** — `public event Action<WallMsg> OnWall;`

### `Ziptide.Multiplayer.PvpNetHub` — 2 signal(s)

- Codes: `EVENT_DECLARATION`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs:25` **EVENT_DECLARATION** — `public static event Action<IPvpTransport> TransportChanged;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs:25` **STATIC_EVENT_DECLARATION** — `public static event Action<IPvpTransport> TransportChanged;`

### `Ziptide.Multiplayer.PvpNetRole` — 5 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:83` **EVENT_DECLARATION** — `event Action<PlayerPoseMsg> OnPose;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:84` **EVENT_DECLARATION** — `event Action<FireMsg> OnFire;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:85` **EVENT_DECLARATION** — `event Action<HitMsg> OnHit;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:86` **EVENT_DECLARATION** — `event Action<ScoreMsg> OnScore;`
  - `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:87` **EVENT_DECLARATION** — `event Action<WallMsg> OnWall;`

### `Ziptide.Ship.ShipFlightRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:114` **SAVE_SYSTEM_REFERENCE** — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`

### `Ziptide.Ship.SpaceTargetRuntime` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/SpaceTargetRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/SpaceTargetRuntime.cs:74` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null,`

### `Ziptide.Tests.EditMode.ComfortSettingsTests` — 2 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:106` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem.", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:121` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem.", source);`

### `Ziptide.Tests.EditMode.CosmeticLockerTests` — 6 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs:16` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs:26` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs:39` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs:48` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs:65` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/CosmeticLockerTests.cs:74` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`

### `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` — 6 signal(s)

- Codes: `EVENT_DECLARATION`, `PLAYER_PROFILE_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:16` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:42` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:87` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:107` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:148` **EVENT_DECLARATION** — `StringAssert.Contains("public static event Action<string> ItemHolstered", socketSource);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:148` **STATIC_EVENT_DECLARATION** — `StringAssert.Contains("public static event Action<string> ItemHolstered", socketSource);`

### `Ziptide.Tests.EditMode.FirstHourObservationCoreTests` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs:184` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs:185` **PLAYER_PROFILE_REFERENCE** — `StringAssert.DoesNotContain("PlayerProfile", source);`

### `Ziptide.Tests.EditMode.FirstHourTravelSignalTests` — 2 signal(s)

- Codes: `EVENT_DECLARATION`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:116` **EVENT_DECLARATION** — `StringAssert.Contains("public static event Action<string> TravelCompleted", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:116` **STATIC_EVENT_DECLARATION** — `StringAssert.Contains("public static event Action<string> TravelCompleted", source);`

### `Ziptide.Tests.EditMode.GardenServiceTests` — 5 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:56` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:79` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:106` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:123` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:165` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.GardenTimingTests` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs:41` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs:141` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs:159` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.GoldenMetaLoopTests` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:129` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, loaded.schemaVersion);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:164` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, p.schemaVersion);`

### `Ziptide.Tests.EditMode.HarvestServiceTests` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:55` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:93` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:142` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.HomeHubFlowTests` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:145` **SAVE_SYSTEM_REFERENCE** — `string source = Read("Gameplay", "Runtime", "Persistence", "SaveSystem.cs");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:149` **PLAYER_PROFILE_REFERENCE** — `StringAssert.Contains("public PlayerProfile StartNewProfile()", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:162` **SAVE_SYSTEM_REFERENCE** — `StringAssert.Contains("SaveSystem.Instance.StartNewProfile()", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:163` **SAVE_SYSTEM_REFERENCE** — `StringAssert.Contains("SaveSystem.Instance.Load();", source);`

### `Ziptide.Tests.EditMode.JobRewardsTests` — 6 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:28` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:40` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:52` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:60` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:69` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:78` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.MiningServiceTests` — 6 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:54` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:70` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:92` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:119` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:140` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:157` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.PhotoAlbumTests` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/PhotoAlbumTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PhotoAlbumTests.cs:92` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, p.schemaVersion, "migrated forward to v3");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PhotoAlbumTests.cs:93` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(3, PlayerProfile.CurrentSchemaVersion);`

### `Ziptide.Tests.EditMode.PlayerProfileTests` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/PlayerProfileTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PlayerProfileTests.cs:11` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PlayerProfileTests.cs:21` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PlayerProfileTests.cs:34` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`

### `Ziptide.Tests.EditMode.ProfileEconomyTests` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:70` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:86` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:102` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:110` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.ProfileSerializerTests` — 2 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileSerializerTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileSerializerTests.cs:46` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, a.schemaVersion);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileSerializerTests.cs:55` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, p.schemaVersion);`

### `Ziptide.Tests.EditMode.PvpNetTests` — 5 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:155` **EVENT_DECLARATION** — `public event System.Action<PlayerPoseMsg> OnPose;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:156` **EVENT_DECLARATION** — `public event System.Action<FireMsg> OnFire;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:157` **EVENT_DECLARATION** — `public event System.Action<HitMsg> OnHit;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:158` **EVENT_DECLARATION** — `public event System.Action<ScoreMsg> OnScore;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:159` **EVENT_DECLARATION** — `public event System.Action<WallMsg> OnWall;`

### `Ziptide.Tests.EditMode.RepairableMachineSignalTests` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:210` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:211` **PLAYER_PROFILE_REFERENCE** — `StringAssert.DoesNotContain("PlayerProfile", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:230` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs:231` **PLAYER_PROFILE_REFERENCE** — `StringAssert.DoesNotContain("PlayerProfile", source);`

### `Ziptide.Tests.EditMode.SaveAutosaveTests` — 4 signal(s)

- Codes: `AUTOSAVE_CALL`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs:8` **SAVE_SYSTEM_REFERENCE** — `/// call from TravelCoordinator with NO live SaveSystem (EditMode has none — the runtime`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs:16` **SAVE_SYSTEM_REFERENCE** — `Assert.IsNull(SaveSystem.Instance, "EditMode should have no bootstrapped SaveSystem");`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs:17` **AUTOSAVE_CALL** — `Assert.DoesNotThrow(() => SaveSystem.AutosaveNow("travel"));`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs:17` **SAVE_SYSTEM_REFERENCE** — `Assert.DoesNotThrow(() => SaveSystem.AutosaveNow("travel"));`

### `Ziptide.Tests.EditMode.ShipLockerTests` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:13` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:26` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ShipLockerTests.cs:38` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`

### `Ziptide.Tests.EditMode.StoryStateTests` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/StoryStateTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/StoryStateTests.cs:12` **PLAYER_PROFILE_REFERENCE** — `private static PlayerProfile With(params string[] flags)`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/StoryStateTests.cs:14` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/StoryStateTests.cs:24` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(0, SignalState.Tier(new PlayerProfile()));`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/StoryStateTests.cs:46` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(RillMemoryState.Dormant, RillState.Compute(new PlayerProfile()));`

### `Ziptide.Tests.EditMode.TransmissionProgressTests` — 3 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/TransmissionProgressTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/TransmissionProgressTests.cs:12` **PLAYER_PROFILE_REFERENCE** — `private static PlayerProfile With(params string[] flags)`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/TransmissionProgressTests.cs:14` **PLAYER_PROFILE_REFERENCE** — `var p = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/TransmissionProgressTests.cs:22` **PLAYER_PROFILE_REFERENCE** — `Assert.AreEqual(0, TransmissionProgress.ComputeTier(new PlayerProfile()));`

### `Ziptide.Tests.EditMode.WorldGatingTests` — 8 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:28` **PLAYER_PROFILE_REFERENCE** — `Assert.IsTrue(WorldGating.MeetsRequirements(Pack(), new PlayerProfile()));`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:34` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:43` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:52` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:68` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:81` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:95` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:107` **PLAYER_PROFILE_REFERENCE** — `var profile = new PlayerProfile();`

### `Ziptide.Tests.EditMode.WristScannerResultTests` — 3 signal(s)

- Codes: `EVENT_DECLARATION`, `SAVE_SYSTEM_REFERENCE`, `STATIC_EVENT_DECLARATION`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WristScannerResultTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WristScannerResultTests.cs:179` **EVENT_DECLARATION** — `StringAssert.Contains("public static event System.Action<WristScanResult> ScanResultPublished", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WristScannerResultTests.cs:179` **STATIC_EVENT_DECLARATION** — `StringAssert.Contains("public static event System.Action<WristScanResult> ScanResultPublished", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WristScannerResultTests.cs:205` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem", source);`

### `Ziptide.Tests.EditMode.ZiplineSignalTests` — 4 signal(s)

- Codes: `EVENT_DECLARATION`, `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:125` **EVENT_DECLARATION** — `StringAssert.Contains("public event System.Action RideStarted;", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:126` **EVENT_DECLARATION** — `StringAssert.Contains("public event System.Action<string, float> RideEnded;", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:152` **PLAYER_PROFILE_REFERENCE** — `StringAssert.DoesNotContain("PlayerProfile", source);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:153` **SAVE_SYSTEM_REFERENCE** — `StringAssert.DoesNotContain("SaveSystem", source);`

### `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:34` **SAVE_SYSTEM_REFERENCE** — `_saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);`

### `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` — 4 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:41` **PLAYER_PROFILE_REFERENCE** — `private PlayerProfile _newGameProfile;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:46` **SAVE_SYSTEM_REFERENCE** — `_saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:85` **SAVE_SYSTEM_REFERENCE** — `Assert.IsFalse(SaveSystem.HasExistingProfile,`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:354` **PLAYER_PROFILE_REFERENCE** — `private void OnNewGameProfileCreated(PlayerProfile profile) => _newGameProfile = profile;`

### `Ziptide.Tests.PlayMode.RecoverySaveFileBackup` — 1 signal(s)

- Codes: `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySaveFileBackup.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySaveFileBackup.cs:10` **SAVE_SYSTEM_REFERENCE** — `/// must be allowed to exercise SaveSystem's real persistentDataPath without destroying a local`

### `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` — 16 signal(s)

- Codes: `PLAYER_PROFILE_REFERENCE`, `SAVE_SYSTEM_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:40` **PLAYER_PROFILE_REFERENCE** — `private PlayerProfile _newGameProfile;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:57` **SAVE_SYSTEM_REFERENCE** — `_saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:95` **SAVE_SYSTEM_REFERENCE** — `Assert.IsFalse(SaveSystem.HasExistingProfile,`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:132` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile live = SaveSystem.Instance.Profile;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:132` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile live = SaveSystem.Instance.Profile;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:153` **SAVE_SYSTEM_REFERENCE** — `SaveSystem.Instance.Load();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:279` **SAVE_SYSTEM_REFERENCE** — `SaveSystem save = FindRequired<SaveSystem>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:311` **SAVE_SYSTEM_REFERENCE** — `SaveSystem save = FindRequired<SaveSystem>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:318` **SAVE_SYSTEM_REFERENCE** — `Assert.AreEqual(identity.SaveSystemId, save.GetInstanceID(), "SaveSystem identity changed.");`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:369` **SAVE_SYSTEM_REFERENCE** — `Assert.IsNotNull(SaveSystem.Instance);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:370` **PLAYER_PROFILE_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance.Profile;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:370` **SAVE_SYSTEM_REFERENCE** — `PlayerProfile profile = SaveSystem.Instance.Profile;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:380` **SAVE_SYSTEM_REFERENCE** — `Assert.IsTrue(File.Exists(SaveSystem.SavePath),`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:382` **SAVE_SYSTEM_REFERENCE** — `string json = File.ReadAllText(SaveSystem.SavePath);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:383` **PLAYER_PROFILE_REFERENCE** — `Assert.IsTrue(ProfileSerializer.TryDeserialize(json, out PlayerProfile profile),`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:420` **PLAYER_PROFILE_REFERENCE** — `private void OnNewGameProfileCreated(PlayerProfile profile) => _newGameProfile = profile;`

### `ZiptideNet.PhotonPvpTransport` — 5 signal(s)

- Codes: `EVENT_DECLARATION`
- Paths: `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs`
  - `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:36` **EVENT_DECLARATION** — `public event Action<PlayerPoseMsg> OnPose;`
  - `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:37` **EVENT_DECLARATION** — `public event Action<FireMsg> OnFire;`
  - `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:38` **EVENT_DECLARATION** — `public event Action<HitMsg> OnHit;`
  - `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:39` **EVENT_DECLARATION** — `public event Action<ScoreMsg> OnScore;`
  - `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:40` **EVENT_DECLARATION** — `public event Action<WallMsg> OnWall;`


## Global render

### `Ziptide.Editor.Patching.CityBuilder` — 4 signal(s)

- Codes: `RENDER_SETTINGS_MUTATION`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:106` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fog = kit.fogEnabled;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:109` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogMode = FogMode.ExponentialSquared;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:110` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogColor = kit.fogColor;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs:111` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogDensity = kit.fogDensity;`

### `Ziptide.Editor.Patching.ForgePhotoBooth` — 13 signal(s)

- Codes: `RENDER_SETTINGS_MUTATION`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:397` **RENDER_SETTINGS_MUTATION** — `var prevSkybox = RenderSettings.skybox;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:398` **RENDER_SETTINGS_MUTATION** — `var prevAmbientMode = RenderSettings.ambientMode;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:399` **RENDER_SETTINGS_MUTATION** — `var prevAmbientLight = RenderSettings.ambientLight;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:400` **RENDER_SETTINGS_MUTATION** — `var prevReflectionMode = RenderSettings.defaultReflectionMode;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:401` **RENDER_SETTINGS_MUTATION** — `RenderSettings.skybox = null;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:402` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:403` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientLight = new Color(0.16f, 0.16f, 0.17f);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:404` **RENDER_SETTINGS_MUTATION** — `RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:405` **RENDER_SETTINGS_MUTATION** — `RenderSettings.customReflectionTexture = null;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:468` **RENDER_SETTINGS_MUTATION** — `RenderSettings.skybox = prevSkybox;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:469` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientMode = prevAmbientMode;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:470` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientLight = prevAmbientLight;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgePhotoBooth.cs:471` **RENDER_SETTINGS_MUTATION** — `RenderSettings.defaultReflectionMode = prevReflectionMode;`

### `Ziptide.Editor.Patching.WorldExperienceBuilder` — 3 signal(s)

- Codes: `RENDER_SETTINGS_MUTATION`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:477` **RENDER_SETTINGS_MUTATION** — `if (!RenderSettings.fog || ex.vista == VistaKind.None) return;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:479` **RENDER_SETTINGS_MUTATION** — `if (RenderSettings.fogDensity > maxDensity)`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs:480` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogDensity = maxDensity;`

### `Ziptide.Editor.Setup.EnsureLocomotionRig` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/EnsureLocomotionRig.cs:215` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Editor.WorldImprovement.WorldImprovementCompiler` — 1 signal(s)

- Codes: `RENDER_SETTINGS_MUTATION`
- Paths: `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementCompiler.cs`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/WorldImprovementCompiler.cs:233` **RENDER_SETTINGS_MUTATION** — `Color fog = RenderSettings.fog ? RenderSettings.fogColor : new Color(0.20f, 0.23f, 0.27f, 1f);`

### `Ziptide.Gameplay.ArenaLobbyBoard` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:214` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.BeltMinePortRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:161` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.BeltRig` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs:43` **CAMERA_MAIN_REFERENCE** — `if (cam == null) cam = Camera.main;`

### `Ziptide.Gameplay.BuildSocketRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:148` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.CollectibleRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:91` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.ConquestMissionRuntime` — 3 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:117` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:309` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:439` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.CreatureBehaviorBase` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs:84` **CAMERA_MAIN_REFERENCE** — `if (Camera.main != null) Player = Camera.main.transform;`

### `Ziptide.Gameplay.CreditsHud` — 2 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs:42` **CAMERA_MAIN_REFERENCE** — `if (_cam == null && Camera.main != null) _cam = Camera.main.transform;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs:69` **CAMERA_MAIN_REFERENCE** — `if (Camera.main != null) _cam = Camera.main.transform; else return;`

### `Ziptide.Gameplay.DevTools.DevMenu` — 2 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:103` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:132` **CAMERA_MAIN_REFERENCE** — `canvas.worldCamera = Camera.main;`

### `Ziptide.Gameplay.DevTools.DevWarpBoard` — 2 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:136` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:271` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.DispatchKiosk` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/DispatchKiosk.cs:100` **CAMERA_MAIN_REFERENCE** — `_camera = Camera.main;`

### `Ziptide.Gameplay.DroneCombatBehavior` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:191` **CAMERA_MAIN_REFERENCE** — `if (Camera.main != null) _player = Camera.main.transform;`

### `Ziptide.Gameplay.FirstHourW001Orchestrator` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:102` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.GardenPlotRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:203` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.HomeHubAnchorLockRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockRuntime.cs:72` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.HomeHubChoice` — 3 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:167` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:279` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:409` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.JobDirector` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:399` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.MiningRigRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:126` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.ObjectiveBeacon` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ObjectiveBeacon.cs:57` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.ObjectiveBoard` — 2 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:190` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:285` **CAMERA_MAIN_REFERENCE** — `canvas.worldCamera = Camera.main;`

### `Ziptide.Gameplay.PlayerMenuRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:73` **CAMERA_MAIN_REFERENCE** — `if (cam == null) cam = Camera.main;`

### `Ziptide.Gameplay.PvpBot` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:387` **CAMERA_MAIN_REFERENCE** — `if (_player == null && Camera.main != null) _player = Camera.main.transform;`

### `Ziptide.Gameplay.PvpHud` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpHud.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpHud.cs:21` **CAMERA_MAIN_REFERENCE** — `if (_cam == null && Camera.main != null) _cam = Camera.main.transform;`

### `Ziptide.Gameplay.PvpModeDirector` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:82` **CAMERA_MAIN_REFERENCE** — `if (_playerHead == null && Camera.main != null) _playerHead = Camera.main.transform;`

### `Ziptide.Gameplay.PvpOnlinePresence` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:102` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.QuestDeviceCorrectionsRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs:298` **CAMERA_MAIN_REFERENCE** — `Camera camera = Camera.main;`

### `Ziptide.Gameplay.RepairableMachine` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:243` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.RillCompanion` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:234` **CAMERA_MAIN_REFERENCE** — `if (cam == null) cam = Camera.main;`

### `Ziptide.Gameplay.ShipBoardingPresentationGuard` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingPresentationGuard.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingPresentationGuard.cs:184` **CAMERA_MAIN_REFERENCE** — `if (_viewer == null) _viewer = Camera.main;`

### `Ziptide.Gameplay.ShipCastOffRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:193` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Gameplay.StaticNetGunRuntime` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/StaticNetWeapon.cs:124` **CAMERA_MAIN_REFERENCE** — `if (_playerHead == null && Camera.main != null) _playerHead = Camera.main.transform;`

### `Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:111` **CAMERA_MAIN_REFERENCE** — `Camera camera = Camera.main;`

### `Ziptide.Gameplay.ViewerSideWorldLabel` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ViewerSideWorldLabel.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ViewerSideWorldLabel.cs:61` **CAMERA_MAIN_REFERENCE** — `if (_viewer == null) _viewer = Camera.main;`

### `Ziptide.Gameplay.WorldDirector` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldDirector.cs:47` **CAMERA_MAIN_REFERENCE** — `Camera cam = Camera.main;`

### `Ziptide.Gameplay.WristScanner` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:73` **CAMERA_MAIN_REFERENCE** — `if (_cam == null && Camera.main != null) _cam = Camera.main.transform;`

### `Ziptide.Gameplay.ZiptideGateEffect` — 2 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:185` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:228` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Tests.PlayMode.RecoveryTestRigTests` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRigTests.cs:55` **CAMERA_MAIN_REFERENCE** — `Assert.AreEqual(_fixture.HeadCamera, Camera.main);`

### `Ziptide.Visuals.PracticalLight` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs:91` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`

### `Ziptide.Visuals.SkyAtmosphereRig` — 1 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs:221` **CAMERA_MAIN_REFERENCE** — `var camera = Camera.main;`

### `Ziptide.Visuals.SkyVistaDefinition` — 1 signal(s)

- Codes: `RENDER_SETTINGS_MUTATION`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaDefinition.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaDefinition.cs:52` **RENDER_SETTINGS_MUTATION** — `[Tooltip("If enabled, RenderSettings.ambientLight is set to ambientColor.")]`

### `Ziptide.Visuals.SkyVistaRig` — 18 signal(s)

- Codes: `CAMERA_MAIN_REFERENCE`, `CAMERA_POST_PROCESSING`, `RENDER_SETTINGS_MUTATION`, `VOLUME_COMPONENT`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:93` **VOLUME_COMPONENT** — `if (vol == null) vol = _gradeVolume.AddComponent<UnityEngine.Rendering.Volume>();`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:100` **CAMERA_MAIN_REFERENCE** — `var cam = Camera.main;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:104` **CAMERA_POST_PROCESSING** — `if (camData != null) camData.renderPostProcessing = true;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:305` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:306` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientLight = vista.ambientColor;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:310` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:311` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientSkyColor = derived.ambientSky;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:312` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientEquatorColor = derived.ambientEquator;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:313` **RENDER_SETTINGS_MUTATION** — `RenderSettings.ambientGroundColor = derived.ambientGround;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:318` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fog = true;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:319` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogMode = FogMode.Exponential;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:320` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogColor = vista.fogColor;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:321` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogDensity = vista.fogDensity;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:323` **RENDER_SETTINGS_MUTATION** — `else if (!RenderSettings.fog)`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:325` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fog = true;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:326` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogMode = FogMode.Exponential;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:327` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogColor = derived.fogColor;`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:328` **RENDER_SETTINGS_MUTATION** — `RenderSettings.fogDensity = derived.fogDensity;`


## Fallback/prototype markers

### `Ziptide.Content.Automation.BeltDir` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Automation/BeltLattice.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Automation/BeltLattice.cs:232` **FALLBACK_MARKER** — `// Splitters pick their exit per item (alternating, blocked-side fallback);`

### `Ziptide.Content.BuildingModule` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/City/BuildingGrammar.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/City/BuildingGrammar.cs:7` **FALLBACK_MARKER** — `/// the Art Registry (docs/design/ART_REGISTRY.md) — primitive fallback until a kit exists.</summary>`

### `Ziptide.Content.BuildingStyleDefinition` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/City/BuildingStyleDefinition.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/City/BuildingStyleDefinition.cs:8` **FALLBACK_MARKER** — `/// primitive fallback renderer; `surfaceFamily` + `styleId` are the Art Registry keys Picasso's`
  - `Ziptide/Assets/Ziptide/Content/Runtime/City/BuildingStyleDefinition.cs:39` **FALLBACK_MARKER** — `[Header("Primitive fallback palette")]`

### `Ziptide.Content.CityLayoutDefinition` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/City/CityLayoutDefinition.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/City/CityLayoutDefinition.cs:372` **FALLBACK_MARKER** — `/// <summary>The shipyard berth + a static (non-flyable) ship placeholder.</summary>`

### `Ziptide.Content.CosmeticKind` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CosmeticDefinition.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CosmeticDefinition.cs:20` **FALLBACK_MARKER** — `/// stub until a drop is authored (create a CosmeticAuthor entry like every other data author).`

### `Ziptide.Content.Ecology.EcologySpecies` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Ecology/EcologyCore.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Ecology/EcologyCore.cs:8` **FALLBACK_MARKER** — `/// BotProfileData idiom: code presets are the fallback truth; SO authoring can mirror later).`

### `Ziptide.Content.FirstHourContractReference` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Tutorial/FirstHourContractDefinition.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Tutorial/FirstHourContractDefinition.cs:124` **FALLBACK_MARKER** — `/// must never depend on a fabricated fallback list.`

### `Ziptide.Content.InteriorPlan` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/City/RoomPartitioner.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/City/RoomPartitioner.cs:136` **FALLBACK_MARKER** — `// Same point (single-room subtree pair after inset) — a stub keeps the invariant.`

### `Ziptide.Content.Ship.ShipSilhouette` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:23` **FALLBACK_MARKER** — `/// <summary>A chassis as PURE data (the BotProfileData idiom — code presets are the fallback truth;`

### `Ziptide.Content.ShipDefinition` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/ShipDefinition.cs`
  - `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/ShipDefinition.cs:23` **FALLBACK_MARKER** — `[Header("Hull (graybox proportions — CityBuilder's berth ship uses these)")]`

### `Ziptide.Content.WorldImprovementModuleSpec` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Content/WorldImprovement/WorldImprovementManifest.cs`
  - `Ziptide/Assets/Ziptide/Content/WorldImprovement/WorldImprovementManifest.cs:111` **FALLBACK_MARKER** — `public Color ResolvePrimary(Color fallback) => primaryColor.a > 0.001f ? primaryColor : fallback;`
  - `Ziptide/Assets/Ziptide/Content/WorldImprovement/WorldImprovementManifest.cs:112` **FALLBACK_MARKER** — `public Color ResolveAccent(Color fallback) => accentColor.a > 0.001f ? accentColor : fallback;`
  - `Ziptide/Assets/Ziptide/Content/WorldImprovement/WorldImprovementManifest.cs:113` **FALLBACK_MARKER** — `public Color ResolveGlow(Color fallback) => glowColor.a > 0.001f ? glowColor : fallback;`

### `Ziptide.Core.CosmeticLocker` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:8` **FALLBACK_MARKER** — `/// with the existing profile plumbing for free and never needs its own persistence. One equipped`

### `Ziptide.Core.RuntimeMaterialFixer` — 6 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:51` **FALLBACK_MARKER** — `Material fallback = new Material(urpLit);`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:52` **FALLBACK_MARKER** — `fallback.name = "RuntimeMaterialFixer_" + r.gameObject.name;`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:53` **FALLBACK_MARKER** — `fallback.hideFlags = HideFlags.HideAndDontSave;`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:55` **FALLBACK_MARKER** — `if (fallback.HasProperty(Shader.PropertyToID(ShaderBaseColor)))`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:56` **FALLBACK_MARKER** — `fallback.SetColor(ShaderBaseColor, color);`
  - `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs:58` **FALLBACK_MARKER** — `r.sharedMaterial = fallback;`

### `Ziptide.Core.SaveFileStore` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/SaveFileStore.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/SaveFileStore.cs:32` **FALLBACK_MARKER** — `// fallback still never leaves us without at least one complete file.`

### `Ziptide.Core.TransmissionText` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionText.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionText.cs:4` **FALLBACK_MARKER** — `/// The Transmission's PLAYBACK TEXT per clarity tier (GAME_PLAN M1 stub; full audio lands at the`

### `Ziptide.Core.ZiptideConstants` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Core/Runtime/ZiptideConstants.cs`
  - `Ziptide/Assets/Ziptide/Core/Runtime/ZiptideConstants.cs:20` **FALLBACK_MARKER** — `// 2026-07-06 (Picasso): REVERTED the June-18 Sandbox dev-bypass — booting into the graybox left`

### `Ziptide.Editor.Art.ArtModuleRegistry` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/ArtModuleRegistry.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/ArtModuleRegistry.cs:30` **FALLBACK_MARKER** — `/// <summary>Consumers: try the registry; false = build your primitive fallback.</summary>`

### `Ziptide.Editor.Art.BuildingKitLibrary` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Art/BuildingKitLibrary.cs:18` **FALLBACK_MARKER** — `/// the primitive-fallback placement, and BuildingBuilder's window pane Inset() lands in the`

### `Ziptide.Editor.Audit.FirstHourContractAuditRules` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/FirstHourContractAuditRules.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/FirstHourContractAuditRules.cs:11` **FALLBACK_MARKER** — `/// it must never become a world-build blocker or invent a fallback beat list.`

### `Ziptide.Editor.Audit.FullSendPresentationAuditRules` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/FullSendPresentationAuditRules.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/FullSendPresentationAuditRules.cs:38` **FALLBACK_MARKER** — `path + " has " + renderers.Length + " hull renderers; minimum hero fallback is "`
  - `Ziptide/Assets/Ziptide/Editor/Audit/FullSendPresentationAuditRules.cs:67` **FALLBACK_MARKER** — `path + " has only " + nonCube + " non-cube hull primitives; hero fallback regressed toward a block stack.", path);`

### `Ziptide.Editor.Audit.RouteContinuityAuditRules` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/RouteContinuityAuditRules.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/RouteContinuityAuditRules.cs:78` **FALLBACK_MARKER** — `// Conservative fallback for uncommon collider types. Generated route slabs are BoxColliders;`

### `Ziptide.Editor.Audit.WorldAuditRunner` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs`
  - `Ziptide/Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs:454` **FALLBACK_MARKER** — `return 1.0f; // fallback: at least 1m above ground`

### `Ziptide.Editor.Patching.BuildingBuilder` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:11` **FALLBACK_MARKER** — `/// ArtModuleRegistry ("buildingModule:&lt;styleId&gt;/&lt;Module&gt;") with primitive fallback, so Picasso's`
  - `Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs:182` **FALLBACK_MARKER** — `Debug.LogWarning("[Ziptide] KIT_UNFULFILLED id=" + regId + " (primitive fallback walls)");`

### `Ziptide.Editor.Patching.FirstHourContractImportResult` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourContractAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourContractAuthor.cs:58` **FALLBACK_MARKER** — `/// Runtime code never parses JSON and no fallback beat list exists.`

### `Ziptide.Editor.Patching.ForgeBaker` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ForgeBaker.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeBaker.cs:14` **FALLBACK_MARKER** — `/// build so stale/primitive fallback content cannot hide behind a successful APK result.`

### `Ziptide.Editor.Patching.ForgeRecipeLibrary` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:82` **FALLBACK_MARKER** — `/// the tenement's plumbing (Capsule standpipe, Frustum vent hood).`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:191` **FALLBACK_MARKER** — `// Tenement plumbing — P2 ops earning their keep in architecture:`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeRecipeLibrary.cs:358` **FALLBACK_MARKER** — `/// glowing gauge (the emissive focal). The tenement wall's plumbing language, freestanding.</summary>`

### `Ziptide.Editor.Patching.IForgeDependencySource` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ForgeDependencyAuditor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ForgeDependencyAuditor.cs:25` **FALLBACK_MARKER** — `/// FORGE_MANIFEST.json answers "what exists / what's playable / what's placeholder / what`

### `Ziptide.Editor.Patching.PracticalAuthor` — 10 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:13` **FALLBACK_MARKER** — `/// lanterns at POI approaches. Each practical = an editor-visible PRIMITIVE fallback +`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:100` **FALLBACK_MARKER** — `// the post fallback stays visible either way — the lantern needs its post.`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:135` **FALLBACK_MARKER** — `/// <summary>Holder + editor-visible primitive fallback + ForgeModuleLook (skipped when`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:144` **FALLBACK_MARKER** — `var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:145` **FALLBACK_MARKER** — `fallback.name = "Fallback";`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:146` **FALLBACK_MARKER** — `var col = fallback.GetComponent<Collider>();`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:148` **FALLBACK_MARKER** — `fallback.transform.SetParent(go.transform, false);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:149` **FALLBACK_MARKER** — `fallback.transform.localPosition = fallbackCenter;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:150` **FALLBACK_MARKER** — `fallback.transform.localScale = fallbackSize;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs:151` **FALLBACK_MARKER** — `var fr = fallback.GetComponent<Renderer>();`

### `Ziptide.Editor.Patching.ScenePatcherD0` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs:15` **FALLBACK_MARKER** — `/// Idempotent scene patcher for D0 City: ensures D0_City scene exists, blockout (plaza, terraces, alley, railings), and runtime objects (JobDirector, DispatchKiosk, ObjectiveBoard, DeliveryCradle). Call from BuildAndroid or menu.`

### `Ziptide.Editor.Patching.ScenePatcherStarterWorld` — 10 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:14` **FALLBACK_MARKER** — `/// Graybox of the STARTER WORLD (onboarding planet) per`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:18` **FALLBACK_MARKER** — `/// blockout only (platforms, ramps, landmark silhouettes, placeholder markers) — scale + pathing +`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:35` **FALLBACK_MARKER** — `// ── Colors (graybox palette) ────────────────────────────────────────`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:44` **FALLBACK_MARKER** — `[MenuItem("Ziptide/Dev/Build Starter World (graybox)")]`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:54` **FALLBACK_MARKER** — `"Built/updated " + SceneName + " graybox.\n\nWarp to it via Ziptide > Dev > Warp Window, "`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:87` **FALLBACK_MARKER** — `// Safety base floor: a single walkable slab just under the platforms so the graybox's gaps`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:93` **FALLBACK_MARKER** — `// Region roots (named exactly per the brief) + their graybox content.`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:231` **FALLBACK_MARKER** — `// Dormant gate ring (ring of pillars as a placeholder for the portal).`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:327` **FALLBACK_MARKER** — `// Pure placeholder transform — named marker for missions/spawns; no renderer/collider.`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs:392` **FALLBACK_MARKER** — `pack.displayName = "Starter World (graybox)";`

### `Ziptide.Editor.Patching.ShipHullBuilder` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:9` **FALLBACK_MARKER** — `/// Shared hero-ship fallback used by every berth. It preserves the named refit skeleton and overall`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:41` **FALLBACK_MARKER** — `// Refit skeleton: these exact direct-child names remain authoritative.`
  - `Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs:55` **FALLBACK_MARKER** — `// Curved shell volumes soften the refit skeleton without replacing it.`

### `Ziptide.Editor.Patching.SignAuthor` — 7 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:130` **FALLBACK_MARKER** — `var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:131` **FALLBACK_MARKER** — `fallback.name = "FallbackBody";`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:132` **FALLBACK_MARKER** — `fallback.transform.SetParent(holder.transform, false);`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:133` **FALLBACK_MARKER** — `fallback.transform.localScale = bodyScale;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:134` **FALLBACK_MARKER** — `fallback.isStatic = true;`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:135` **FALLBACK_MARKER** — `Collider bodyCollider = fallback.GetComponent<Collider>();`
  - `Ziptide/Assets/Ziptide/Editor/Patching/SignAuthor.cs:137` **FALLBACK_MARKER** — `Renderer bodyRenderer = fallback.GetComponent<Renderer>();`

### `Ziptide.Editor.Patching.WorldDressingBuilder` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs:224` **FALLBACK_MARKER** — `// the primitive block stays the editor/fallback look) + ForgeSway (wind). The block`

### `Ziptide.Editor.Patching.WorldJobLibrary` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldJobLibrary.cs:346` **FALLBACK_MARKER** — `private const float GoArriveDistance = 3.5f; // generous for graybox marker-at-building targets`

### `Ziptide.Editor.Patching.WorldLayoutLibrary` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldLayoutLibrary.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldLayoutLibrary.cs:686` **FALLBACK_MARKER** — `// High-ground crossings over the tide flats — the flood-timing feel in graybox.`

### `Ziptide.Editor.Patching.WorldStubGenerator` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:37` **FALLBACK_MARKER** — `EditorUtility.DisplayDialog("World Stub Generator",`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:42` **FALLBACK_MARKER** — `EditorUtility.DisplayDialog("World Stub Generator",`
  - `Ziptide/Assets/Ziptide/Editor/Patching/WorldStubGenerator.cs:50` **FALLBACK_MARKER** — `EditorUtility.DisplayDialog("World Stub Generator",`

### `Ziptide.Editor.Setup.SetupMilestoneAScene` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs`
  - `Ziptide/Assets/Ziptide/Editor/Setup/SetupMilestoneAScene.cs:63` **FALLBACK_MARKER** — `Debug.LogWarning("[Ziptide] XR Origin prefab not found. Add it manually: GameObject > XR > XR Origin (VR). Creating empty placeholder.");`

### `Ziptide.Editor.WorldImprovement.GroundedRouteModule` — 5 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:61` **FALLBACK_MARKER** — `int fallback = Mathf.Min(6, budget);`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:62` **FALLBACK_MARKER** — `for (int i = 0; i < fallback; i++)`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:64` **FALLBACK_MARKER** — `float angle = i * Mathf.PI * 2f / fallback;`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:235` **FALLBACK_MARKER** — `if (count == 0 && context.TryGround(origin + Vector3.forward * 6f, out Vector3 fallback))`
  - `Ziptide/Assets/Ziptide/Editor/WorldImprovement/RoundThreeWorldImprovementModules.cs:239` **FALLBACK_MARKER** — `trace.position = fallback;`

### `Ziptide.Gameplay.BeltMinePortRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:79` **FALLBACK_MARKER** — `// A stub drill over the port — enough to read "this is where the ore comes from".`

### `Ziptide.Gameplay.BootHoldState` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:755` **FALLBACK_MARKER** — `// Name-based fallback for rigs without XROrigin component attached.`

### `Ziptide.Gameplay.CollectibleRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:57` **FALLBACK_MARKER** — `// Visual: a small spinning "shard" (stretched cube reads as a crystal at graybox quality).`

### `Ziptide.Gameplay.CreatureRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:92` **FALLBACK_MARKER** — `// ── Hit entry points (existing weapon plumbing) ─────────────────────`

### `Ziptide.Gameplay.DevTools.DevMenu` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs:70` **FALLBACK_MARKER** — `/// fallback if a scene forgot one) and force every UI-enabled ray interactor to re-register`

### `Ziptide.Gameplay.EmergencyRespawn` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs:71` **FALLBACK_MARKER** — `// Fallback: teleport to SpawnMarkerRuntime.`

### `Ziptide.Gameplay.FirstHourW001Orchestrator` — 5 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:132` **FALLBACK_MARKER** — `CreatureRuntime fallback = null;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:138` **FALLBACK_MARKER** — `if (fallback == null) fallback = candidate;`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:143` **FALLBACK_MARKER** — `if (fallback != null && !_speciesFallbackLogged)`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:147` **FALLBACK_MARKER** — `+ " using=" + fallback.creatureId);`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:149` **FALLBACK_MARKER** — `return fallback;`

### `Ziptide.Gameplay.GazeMath` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/GazeMath.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/GazeMath.cs:7` **FALLBACK_MARKER** — `/// "Observed" = inside the viewer's forward cone AND within range — the graybox stand-in for`

### `Ziptide.Gameplay.GunLaserSight` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/GunLaserSight.cs:30` **FALLBACK_MARKER** — `/// Alpha 0 keeps the default sight color (ItemDefinition fallback idiom). Init repeats the melee`

### `Ziptide.Gameplay.HazardZoneRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:56` **FALLBACK_MARKER** — `// A thin tinted floor slab marks the zone (graybox read; VFX at the M6 art pass).`

### `Ziptide.Gameplay.IPvpDamageable` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/IPvpDamageable.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/IPvpDamageable.cs:8` **FALLBACK_MARKER** — `/// now, a networked remote avatar in Phase 4). Weapons route hits through this so the same plumbing`

### `Ziptide.Gameplay.InputMutationRepairDriver` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs:338` **FALLBACK_MARKER** — `// inert placeholder, not a binding that can be repaired. Keep it disabled so XRI never`

### `Ziptide.Gameplay.InventoryState` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryState.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/InventoryState.cs:92` **FALLBACK_MARKER** — `// Legacy parent fallback remains readable for one migration cycle, but proximity alone is`

### `Ziptide.Gameplay.ItemFactory` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:41` **FALLBACK_MARKER** — `// the generated mesh — a look, never a stat; missing recipe = graceful primitive fallback.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:135` **FALLBACK_MARKER** — `// LAST-RESORT fallback — only sees definitions something already loaded (a scene reference).`

### `Ziptide.Gameplay.JobDirector` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:281` **FALLBACK_MARKER** — `/// scene markers are the fallback.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:283` **FALLBACK_MARKER** — `/// ⚠ WHY THE FALLBACK EXISTS. This used to search only <see cref="_markerTransforms"/> — the`

### `Ziptide.Gameplay.LightGrazerBehavior` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/LightGrazerBehavior.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/LightGrazerBehavior.cs:8` **FALLBACK_MARKER** — `/// (W002's cistern). Graybox light source = the player's ATTENTION: facing it up close "shines your`

### `Ziptide.Gameplay.PvpBot` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:136` **FALLBACK_MARKER** — `/// <summary>Difficulty as data: the Resources asset wins; code presets are the fallback.</summary>`

### `Ziptide.Gameplay.PvpMatchDirector` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:11` **FALLBACK_MARKER** — `/// inference as the fallback, and <see cref="PvpModeDirector"/> hooks the events to run objective`

### `Ziptide.Gameplay.PvpRoundLogic` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpRoundLogic.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpRoundLogic.cs:11` **FALLBACK_MARKER** — `/// <summary>Fallback inference when no weapon reported its firer: in 1v1 whoever didn't die gets`

### `Ziptide.Gameplay.QuartersRoom` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:15` **FALLBACK_MARKER** — `/// stub — the ROOM and its plumbing are the deliverable.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:130` **FALLBACK_MARKER** — `// Open every bay once on build so the stub state is visible without a press.`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:160` **FALLBACK_MARKER** — `// THE STUB: the room exists before its stock does.`

### `Ziptide.Gameplay.RillCompanion` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:101` **FALLBACK_MARKER** — `// Drop already-said once-lines BEFORE the wildcard fallback, so a spent specific`

### `Ziptide.Gameplay.ShipRefit` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:9` **FALLBACK_MARKER** — `/// SHIP PILLAR 2.1/2.2 — THE REFIT: turns the berth's shared hull skeleton into YOUR ship, at`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:13` **FALLBACK_MARKER** — `/// same skeleton, deck/door offsets untouched (they key off the unchanged root). Picasso's`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:46` **FALLBACK_MARKER** — `// ── Chassis proportions (six silhouettes on one skeleton) ────────────`

### `Ziptide.Gameplay.ToxicRiverRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ToxicRiverRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ToxicRiverRuntime.cs:14` **FALLBACK_MARKER** — `/// logged fallback when the visual surface cannot be resolved.`

### `Ziptide.Gameplay.TransmissionConsole` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:9` **FALLBACK_MARKER** — `/// The de-garble playback console (GAME_PLAN M1 stub; audio at M6): a small terminal that, when`

### `Ziptide.Gameplay.WeaponPoseCore` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:23` **FALLBACK_MARKER** — `Vector3 fallback = Mathf.Abs(Vector3.Dot(axis, Vector3.up)) < 0.9f`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:25` **FALLBACK_MARKER** — `up = Vector3.ProjectOnPlane(fallback, axis);`

### `Ziptide.Gameplay.WorldRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldRuntime.cs:78` **FALLBACK_MARKER** — `// and create a respawn-fall loop. Marker first, profile only as fallback.`

### `Ziptide.Gameplay.ZiptideGateEffect` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs`
  - `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs:123` **FALLBACK_MARKER** — `/// <summary>URP/Unlit with a hard fallback — a stripped shader on device would make`

### `Ziptide.Ship.ShipFlightRuntime` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs`
  - `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:452` **FALLBACK_MARKER** — `// ── Rig plumbing (ShipBoardingStation patterns — teleport, never parent) ───────────────────`

### `Ziptide.Tests.EditMode.BootConfigTests` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/BootConfigTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/BootConfigTests.cs:8` **FALLBACK_MARKER** — `/// at the SandboxTestLab graybox "temporarily," and it silently stayed for weeks — booting testers`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/BootConfigTests.cs:11` **FALLBACK_MARKER** — `/// the sandbox graybox, and never _Boot itself.`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/BootConfigTests.cs:26` **FALLBACK_MARKER** — `"boot must not land in the SandboxTestLab graybox — that stranded testers behind the broken dev menu");`

### `Ziptide.Tests.EditMode.ConquestMissionTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ConquestMissionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ConquestMissionTests.cs:41` **FALLBACK_MARKER** — `// The richness bar: shipping 2 of 5 contract kinds is a skeleton. Across the canonical`

### `Ziptide.Tests.EditMode.FirstHourTravelSignalTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:119` **FALLBACK_MARKER** — `"the no-coordinator fallback remains the only synchronous load");`

### `Ziptide.Tests.EditMode.ForgeBodyTellTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBodyTellTests.cs:75` **FALLBACK_MARKER** — `"an eyeless genome must NOT swallow the tell — the behavior needs its fallback");`

### `Ziptide.Tests.EditMode.ForgeBuildingKitTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBuildingKitTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeBuildingKitTests.cs:93` **FALLBACK_MARKER** — `id + " lost the structured primitive fallback (editor/no-bake look)");`

### `Ziptide.Tests.EditMode.ForgeGaitMotorTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeGaitMotorTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ForgeGaitMotorTests.cs:9` **FALLBACK_MARKER** — `/// the real skeleton, applies a motor pose the way ForgeCreatureAnimator does, and proves the`

### `Ziptide.Tests.EditMode.ItemRegistryConventionTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/ItemRegistryConventionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/ItemRegistryConventionTests.cs:11` **FALLBACK_MARKER** — `/// device (the loaded-objects fallback only sees assets a loaded scene happens to reference — a`

### `Ziptide.Tests.EditMode.JobDirectorMarkerResolutionTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/JobDirectorMarkerResolutionTests.cs:12` **FALLBACK_MARKER** — `/// never granted, which left W002 permanently locked. These tests pin the scene fallback.`

### `Ziptide.Tests.EditMode.RecoveryPlayabilityDeviceTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/RecoveryPlayabilityDeviceTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/RecoveryPlayabilityDeviceTests.cs:116` **FALLBACK_MARKER** — `"The no-ray direct-hand fallback must remain valid for the three-tile layout.");`

### `Ziptide.Tests.EditMode.WorldImprovementManifestTests` — 7 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:45` **FALLBACK_MARKER** — `WorldImprovementManifest fallback = MakeManifest();`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:46` **FALLBACK_MARKER** — `fallback.sceneName = string.Empty;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:47` **FALLBACK_MARKER** — `fallback.appliesToGeneratedWorlds = true;`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:48` **FALLBACK_MARKER** — `fallback.excludedScenes = new[] { "Arena" };`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:49` **FALLBACK_MARKER** — `Assert.That(fallback.AppliesTo("W101_Other", "Assets/Scenes/Generated/W101_Other.unity"), Is.True);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:50` **FALLBACK_MARKER** — `Assert.That(fallback.AppliesTo("Arena", "Assets/Scenes/Generated/Arena.unity"), Is.False);`
  - `Ziptide/Assets/Ziptide/Tests/EditMode/WorldImprovementManifestTests.cs:51` **FALLBACK_MARKER** — `Assert.That(fallback.AppliesTo("W101_Other", "Assets/Scenes/Handmade/W101_Other.unity"), Is.False);`

### `Ziptide.Tests.PlayMode.RecoveryActualRigControllerSimulation` — 4 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:16` **FALLBACK_MARKER** — `/// This helper borrows that pair, creates only a missing fallback device, fixes the tracked-head pose`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:631` **FALLBACK_MARKER** — `XRRayInteractor fallback = null;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:640` **FALLBACK_MARKER** — `if (fallback == null) fallback = ray;`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualRigControllerSimulation.cs:644` **FALLBACK_MARKER** — `return fallback;`

### `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:125` **FALLBACK_MARKER** — `"Global fallback mutators are not diagnostic surfaces.");`

### `Ziptide.Tests.PlayMode.RecoveryFallbackSurfaceAuditTests` — 5 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:24` **FALLBACK_MARKER** — `GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:25` **FALLBACK_MARKER** — `fallback.name = "__RECOVERY_FALLBACK_CANARY_MATERIAL";`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:26` **FALLBACK_MARKER** — `fallback.transform.SetParent(root.transform, false);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:27` **FALLBACK_MARKER** — `Renderer fallbackRenderer = fallback.GetComponent<Renderer>();`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAuditTests.cs:52` **FALLBACK_MARKER** — `"The canary produced unexpected fallback-surface findings.");`

### `Ziptide.Tests.PlayMode.RecoveryFallbackSurfaceReport` — 6 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:52` **FALLBACK_MARKER** — `/// Test-owned R1.9 material/fallback exposure layer. The existing Golden profile, runtime census,`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:65` **FALLBACK_MARKER** — `public const string ArtifactDirectoryName = "recovery-fallback-surfaces";`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:71` **FALLBACK_MARKER** — `throw new ArgumentException("Fallback-surface label is required.", nameof(label));`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:196` **FALLBACK_MARKER** — `Assert.Fail(label + " exposed known fallback/material blockers. JSON=" + paths.JsonPath`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:249` **FALLBACK_MARKER** — `text.AppendLine("# Recovery Fallback Surface Audit — " + report.label);`
  - `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:276` **FALLBACK_MARKER** — `if (string.IsNullOrWhiteSpace(stem)) stem = "fallback-surfaces";`

### `Ziptide.Visuals.ForgeCreatureAnimator` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureAnimator.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureAnimator.cs:7` **FALLBACK_MARKER** — `/// built skeleton once; every LateUpdate it measures its OWN world speed (no Gameplay`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureAnimator.cs:28` **FALLBACK_MARKER** — `/// <summary>Wire the animator to a built skeleton (ForgeSkinnedBuilder.Result.bones).</summary>`

### `Ziptide.Visuals.ForgeCreatureVisualApplier` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureVisualApplier.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureVisualApplier.cs:9` **FALLBACK_MARKER** — `/// this builds the skinned walking body under a "ForgeVisual" child (skeleton + one`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeCreatureVisualApplier.cs:55` **FALLBACK_MARKER** — `: Color.magenta; // loud fallback — Validate() should have caught this`

### `Ziptide.Visuals.ForgeGaitMotor` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeGaitMotor.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeGaitMotor.cs:31` **FALLBACK_MARKER** — `/// identity (body bob/lean is the behavior mover's job, not the skeleton's); the root gets`

### `Ziptide.Visuals.ForgeMaterials` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMaterials.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMaterials.cs:37` **FALLBACK_MARKER** — `: Color.magenta; // loud fallback — Validate() should have caught this`

### `Ziptide.Visuals.ForgeSkinnedBuilder` — 3 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs:8` **FALLBACK_MARKER** — `/// rigid-weighted skinned mesh + its bone skeleton: root bone carries the core parts, each`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs:30` **FALLBACK_MARKER** — `/// <summary>Build mesh + skeleton. Caller owns the returned skeletonRoot GameObject.</summary>`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeSkinnedBuilder.cs:41` **FALLBACK_MARKER** — `// ── Skeleton ────────────────────────────────────────────────────`

### `Ziptide.Visuals.ForgeVisualApplier` — 2 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeVisualApplier.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeVisualApplier.cs:58` **FALLBACK_MARKER** — `// Clear every previously-owned child before choosing the currently available baked/fallback`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeVisualApplier.cs:64` **FALLBACK_MARKER** — `// The runtime flat-color mesh stays as the dev fallback when no bake shipped.`

### `Ziptide.Visuals.ZiptideWater` — 1 signal(s)

- Codes: `FALLBACK_MARKER`
- Paths: `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs`
  - `Ziptide/Assets/Ziptide/Visuals/Runtime/Water/ZiptideWater.cs:9` **FALLBACK_MARKER** — `/// fallback), a scrolling normal offset (<see cref="WaterMotion"/>), a gentle low-res vertex`

## Inventory validation findings

No validation report supplied, or no findings.
