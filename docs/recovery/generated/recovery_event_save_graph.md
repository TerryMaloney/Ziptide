# ZIPTIDE Event and Save Ownership Graph

- Scanned C# files: **764**
- Evidence edges: **824**
- Named subscriptions without matching unsubscribe in the same owner: **60**

This is a static ownership graph. An unmatched row is a review target, not automatic proof of a leak; process-lifetime static hooks may be intentional.

## Evidence counts

- **AUTOSAVE:** 8
- **EVENT_DECLARE:** 49
- **EVENT_INVOKE:** 61
- **EVENT_SUBSCRIBE:** 138
- **EVENT_UNSUBSCRIBE:** 84
- **PLAYER_PREFS_ACCESS:** 20
- **PROFILE_FIELD_ACCESS:** 383
- **SAVE_ACCESS:** 81

## Named subscriptions without matching unsubscribe

- `Ziptide.Content.HarvestPlantResult` subscribes `plot.yieldMultiplier` → `TendYieldBonusPerPower` at `Ziptide/Assets/Ziptide/Content/Runtime/Economy/GardenService.cs:127`
- `Ziptide.Content.FlightState` subscribes `s.rollDeg` → `s` at `Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs:104`
- `Ziptide.Content.FlightState` subscribes `s.position` → `Forward` at `Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs:112`
- `Ziptide.Content.FlightState` subscribes `s.position` → `right` at `Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs:119`
- `Ziptide.Content.Ship.ShipStats` subscribes `s.Speed` → `m` at `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:145`
- `Ziptide.Content.Ship.ShipStats` subscribes `s.Handling` → `m` at `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:146`
- `Ziptide.Content.Ship.ShipStats` subscribes `s.Boost` → `m` at `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:147`
- `Ziptide.Content.Ship.ShipStats` subscribes `s.Cargo` → `m` at `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:148`
- `Ziptide.Content.Ship.ShipStats` subscribes `s.Armor` → `m` at `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:149`
- `Ziptide.Core.WorldResolveResult` subscribes `result.totalProduced` → `acc` at `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ProfileEconomy.cs:37`
- `Ziptide.Editor.DevTools.DevWarpPlayHook` subscribes `EditorApplication.playModeStateChanged` → `OnPlayModeChanged` at `Ziptide/Assets/Ziptide/Editor/DevTools/DevWarpPlayHook.cs:19`
- `Ziptide.Gameplay.BeltConductorRuntime` subscribes `_rig.position` → `_handle` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:136`
- `Ziptide.Gameplay.BeltCellSpec` subscribes `rail.transform.position` → `rail` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:274`
- `Ziptide.Gameplay.BeltCellSpec` subscribes `plate.transform.position` → `q` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:297`
- `Ziptide.Gameplay.BeltMinePortRuntime` subscribes `_mine.stored` → `_mine` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:128`
- `Ziptide.Gameplay.BruiserBehavior` subscribes `transform.position` → `new` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/BruiserBehavior.cs:61`
- `Ziptide.Gameplay.CreatureRuntime` subscribes `transform.position` → `flat` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:120`
- `Ziptide.Gameplay.CreatureRuntime` subscribes `transform.position` → `flat` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:121`
- `Ziptide.Gameplay.CreatureRuntime` subscribes `transform.position` → `flat` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:122`
- `Ziptide.Gameplay.StunBolt` subscribes `transform.position` → `step` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs:65`
- `Ziptide.Gameplay.ObjectiveBoard` subscribes `runtime.StepChanged` → `OnStepChanged` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:93`
- `Ziptide.Gameplay.ObjectiveBoard` subscribes `runtime.JobCompleted` → `OnJobCompleted` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:94`
- `Ziptide.Gameplay.BootHoldState` subscribes `transform.position` → `headDelta` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:1032`
- `Ziptide.Gameplay.TurnModeCore` subscribes `p.y` → `RecoveryEyeHeight` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs:258`
- `Ziptide.Gameplay.QuestDeviceCorrectionsRuntime` subscribes `transform.position` → `handAttach` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs:332`
- `Ziptide.Gameplay.PvpBolt` subscribes `transform.position` → `step` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs:67`
- `Ziptide.Gameplay.PvpBot` subscribes `transform.position` → `k` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:346`
- `Ziptide.Gameplay.ArtifactJoinRuntime` subscribes `item.transform.position` → `delta` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:142`
- `Ziptide.Gameplay.MiningRigRuntime` subscribes `_mine.stored` → `_mine` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:114`
- `Ziptide.Gameplay.WeaponPoseCore` subscribes `bot.transform.position` → `transform` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:209`
- `Ziptide.Gameplay.SonicThumperRuntime` subscribes `bot.transform.position` → `shoveDir` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:78`
- `Ziptide.Gameplay.CityStreetLifeRuntime` subscribes `p.y` → `phase` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/CityStreetLifeRuntime.cs:52`
- `Ziptide.Gameplay.CityStreetLifeRuntime` subscribes `p.x` → `Mathf` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/CityStreetLifeRuntime.cs:53`
- `Ziptide.Gameplay.ClimbableSurface` subscribes `_rig.position` → `new` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs:218`
- `Ziptide.Gameplay.ConquestMissionRuntime` subscribes `SceneManager.sceneLoaded` → `OnSceneLoaded` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:45`
- `Ziptide.Gameplay.ConquestMissionRuntime` subscribes `_rigRoot.transform.position` → `Vector3` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:414`
- `Ziptide.Gameplay.ConquestMissionRuntime` subscribes `transform.localPosition` → `Vector3` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:498`
- `Ziptide.Gameplay.HazardZoneRuntime` subscribes `_rig.transform.position` → `dir` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:111`
- `Ziptide.Gameplay.HazardZoneRuntime` subscribes `_rig.transform.position` → `outDir` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:134`
- `Ziptide.Gameplay.LiftRuntime` subscribes `_rig.position` → `delta` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/LiftRuntime.cs:88`
- `Ziptide.Gameplay.ReentryArrivalRuntime` subscribes `SceneManager.sceneUnloaded` → `OnSceneUnloaded` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ReentryArrivalRuntime.cs:61`
- `Ziptide.Gameplay.ToxicRiverSurfaceRuntime` subscribes `p.y` → `Mathf` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ToxicRiverSurfaceRuntime.cs:78`
- `Ziptide.Gameplay.ToxicRiverSurfaceRuntime` subscribes `p.y` → `Mathf` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ToxicRiverSurfaceRuntime.cs:88`
- `Ziptide.Gameplay.ZiplineRuntime` subscribes `_rig.position` → `delta` at `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:217`
- `Ziptide.Multiplayer.Bots.BotPerception` subscribes `aim.Y` → `_rng` at `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Bots/BotBrain.cs:237`
- `Ziptide.Multiplayer.Conquest.ConquestPlayer` subscribes `planet.defenseLevel` → `d` at `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestState.cs:103`
- `Ziptide.Ship.VehicleSafetyRuntime` subscribes `p.y` → `MountedRecoveryEyeHeight` at `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs:121`
- `Ziptide.Tests.EditMode.FieldCameraCompletionTests` subscribes `SceneManager.sceneLoaded` → `OnSceneLoaded` at `Ziptide/Assets/Ziptide/Tests/EditMode/FieldCameraCompletionTests.cs:82`
- `Ziptide.Tests.EditMode.FirstRouteFeelTests` subscribes `runtime.JobCompleted` → `OnJobCompleted` at `Ziptide/Assets/Ziptide/Tests/EditMode/FirstRouteFeelTests.cs:61`
- `Ziptide.Tests.EditMode.HeadsetBuildBlockerRegressionTests` subscribes `EditorSceneManager.sceneOpened` → `OnSceneOpened` at `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs:71`
- `Ziptide.Tests.EditMode.HomeHubFlowTests` subscribes `castOff.DestinationSelected` → `destination` at `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:184`
- `Ziptide.Tests.EditMode.PvpNetTests` subscribes `t.OnFire` → `m` at `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:27`
- `Ziptide.Tests.EditMode.PvpNetTests` subscribes `t.OnHit` → `m` at `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:41`
- `Ziptide.Tests.EditMode.PvpNetTests` subscribes `t.OnScore` → `m` at `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:56`
- `Ziptide.Tests.EditMode.PvpNetTests` subscribes `t.OnWall` → `m` at `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:70`
- `Ziptide.Tests.EditMode.PvpNetTests` subscribes `t.OnPose` → `m` at `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:90`
- `Ziptide.Tests.EditMode.ZiplineSignalTests` subscribes `_rig.position` → `delta` at `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:132`
- `Ziptide.Tests.PlayMode.RecoveryFallbackSurfaceReport` subscribes `report.materialSlotCount` → `materials` at `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:96`
- `Ziptide.Tests.PlayMode.RecoveryTestRig` subscribes `Root.transform.position` → `delta` at `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:121`
- `Ziptide.Visuals.SkyAtmosphereRig` subscribes `position.y` → `eyeY` at `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs:235`

## Targets

### `Accent`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Ship.VehicleRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:150` · `profile` — `ObjectiveBeacon.Attach(gameObject, profile.Accent, 5f);`

### `ActiveStateEvidence`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:120` · `profile` — `foreach (var state in profile.ActiveStateEvidence)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:26` · `profile` — `Assert.AreEqual(profile.ActiveStates.Count, profile.ActiveStateEvidence.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:38` · `profile` — `foreach (var state in profile.ActiveStateEvidence)`

### `ActiveStates`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:248` · `profile` — `foreach (string state in profile.ActiveStates)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:26` · `profile` — `Assert.AreEqual(profile.ActiveStates.Count, profile.ActiveStateEvidence.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:27` · `profile` — `Assert.GreaterOrEqual(profile.ActiveStates.Count,`

### `Add`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.SkyVistaRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:117` · `profile` — `return profile.Add<T>(true);`

### `AddResource`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:94` · `profile` — `double total = profile.AddResource(resourceId, amount);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:110` · `profile` — `profile.AddResource(resourceId, -amount);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:61` · `profile` — `profile.AddResource("credits", 10);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:55` · `profile` — `profile.AddResource("scrap", 10);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:57` · `profile` — `profile.AddResource("gear", 2);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:71` · `profile` — `profile.AddResource("scrap", 10); // gear missing`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:76` · `profile` — `profile.AddResource("gear", 5);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:93` · `profile` — `profile.AddResource("scrap", 8);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:120` · `profile` — `profile.AddResource("scrap", 2); // not enough`

### `Allows`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:90` · `profile` — `Assert.IsTrue(profile.Allows(registration.FeatureId), registration.OwnerId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:93` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.RuntimeHealthMonitor));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:94` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.AmbienceDirector));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:95` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.ComfortVignette));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:96` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.FirstHourObservation));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:97` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.SingletonValidator));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:99` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.EcologyInjector),`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:101` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.DebugHud));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:102` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.XrCameraEnforcer));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:103` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.RuntimeInputEnabler));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:104` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.RuntimeMaterialFixer));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:105` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.VrBootDiagnostics));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:106` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.DevWarpBoard));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:107` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.ConquestMissionInjector));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:108` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.PvpProgression));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:109` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.QuartersCameraInjector));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:110` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.NetBootstrap));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:120` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.DebugHud));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:121` · `profile` — `Assert.IsTrue(profile.Allows(RecoveryFeatureId.DevWarpBoard));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:122` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.VrBootDiagnostics),`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryExposureProfileTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryExposureProfileTests.cs:124` · `profile` — `Assert.IsFalse(profile.Allows(RecoveryFeatureId.RuntimeMaterialFixer),`

### `Application.logMessageReceived`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootHoldOrderingTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootHoldOrderingTests.cs:37` · `_capture` — `Application.logMessageReceived += _capture;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootHoldOrderingTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootHoldOrderingTests.cs:44` · `_capture` — `Application.logMessageReceived -= _capture;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:42` · `_logCallback` — `Application.logMessageReceived += _logCallback;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:53` · `_logCallback` — `if (_logCallback != null) Application.logMessageReceived -= _logCallback;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:55` · `_logCallback` — `Application.logMessageReceived += _logCallback;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:66` · `_logCallback` — `if (_logCallback != null) Application.logMessageReceived -= _logCallback;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:65` · `_logCallback` — `Application.logMessageReceived += _logCallback;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:76` · `_logCallback` — `if (_logCallback != null) Application.logMessageReceived -= _logCallback;`

### `Application.logMessageReceivedThreaded`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Core.PersistentDiagnosticRing` · `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/PersistentDiagnosticRing.cs:41` · `Capture` — `Application.logMessageReceivedThreaded -= Capture;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Core.PersistentDiagnosticRing` · `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/PersistentDiagnosticRing.cs:62` · `Capture` — `Application.logMessageReceivedThreaded -= Capture;`
- **EVENT_SUBSCRIBE** · `Ziptide.Core.PersistentDiagnosticRing` · `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/PersistentDiagnosticRing.cs:63` · `Capture` — `Application.logMessageReceivedThreaded += Capture;`

### `ApproximateBounds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:20` · `profile` — `Assert.That(profile.ApproximateBounds.x, Is.InRange(1.4f, 3.0f));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:21` · `profile` — `Assert.That(profile.ApproximateBounds.y, Is.InRange(1.2f, 2.4f));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:22` · `profile` — `Assert.That(profile.ApproximateBounds.z, Is.InRange(2.5f, 3.8f));`

### `Assert`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CrashProofingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CrashProofingTests.cs:90` · `profile` — `// The public Deserialize contract is unchanged: garbage still yields a fresh profile.`

### `AudioMixSettings.Changed`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:41` · `OnMixChanged` — `AudioMixSettings.Changed += OnMixChanged;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:49` · `OnMixChanged` — `AudioMixSettings.Changed -= OnMixChanged;`

### `BehaviorSourceRelativePath`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:111` · `profile` — `string sourcePath = ResolveAssetRelativeSource(profile.BehaviorSourceRelativePath);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:127` · `profile` — `profile.BehaviorSourceRelativePath);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:135` · `profile` — `profile.BehaviorSourceRelativePath);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:29` · `profile` — `Assert.IsFalse(string.IsNullOrWhiteSpace(profile.BehaviorSourceRelativePath), profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:33` · `profile` — `profile.BehaviorSourceRelativePath);`

### `BehaviorTypeName`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:245` · `profile` — `if (string.IsNullOrWhiteSpace(profile.BehaviorTypeName)) errors.Add("BEHAVIOR_TYPE_EMPTY");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:98` · `profile` — `"Ziptide.Gameplay." + profile.BehaviorTypeName,`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:104` · `profile` — `profile.BehaviorTypeName + "'.");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:36` · `profile` — `StringAssert.Contains(profile.BehaviorTypeName, source, profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:107` · `profile` — `"Ziptide.Gameplay." + profile.BehaviorTypeName,`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:69` · `profile` — `"Ziptide.Gameplay." + profile.BehaviorTypeName,`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:73` · `profile` — `definition.id + " points at missing behavior type " + profile.BehaviorTypeName);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:75` · `profile` — `profile.BehaviorTypeName + " must remain a CreatureBehaviorBase subclass");`

### `BootPresentationReady`

- **EVENT_DECLARE** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:114` · `static Action<bool>` — `public static event Action<bool> BootPresentationReady;`

### `Changed`

- **EVENT_DECLARE** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:22` · `static Action` — `public static event Action Changed;`

### `ChoiceSelected`

- **EVENT_DECLARE** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:116` · `static Action<HomeHubChoice>` — `public static event Action<HomeHubChoice> ChoiceSelected;`

### `ComfortConsoleRuntime.PresetConfirmed`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:154` · `OnComfortConfirmed` — `ComfortConsoleRuntime.PresetConfirmed += OnComfortConfirmed;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:172` · `OnComfortConfirmed` — `ComfortConsoleRuntime.PresetConfirmed -= OnComfortConfirmed;`

### `CounterState`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:263` · `profile` — `if (!states.Contains(profile.CounterState))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:264` · `profile` — `errors.Add("COUNTER_NOT_ACTIVE:" + profile.CounterState);`

### `CreatureDisabled`

- **EVENT_DECLARE** · `Ziptide.Gameplay.CreatureRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:64` · `static System.Action<CreatureRuntime>` — `public static event System.Action<CreatureRuntime> CreatureDisabled;`

### `CreatureId`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:244` · `profile` — `if (string.IsNullOrWhiteSpace(profile.CreatureId)) errors.Add("CREATURE_ID_EMPTY");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:306` · `profile` — `if (profile == null || string.IsNullOrEmpty(profile.CreatureId)) continue;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:307` · `profile` — `lookup.Add(profile.CreatureId, profile);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:40` · `profile` — `"Creature '" + profile.CreatureId + "' profile error: " + error);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:42` · `profile` — `if (string.IsNullOrWhiteSpace(profile.CreatureId)) continue;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:43` · `profile` — `if (!profilesById.TryAdd(profile.CreatureId, profile))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:45` · `profile` — `"Readability catalog contains duplicate id '" + profile.CreatureId + "'.");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:85` · `profile` — `if (profile != null && !assetIds.Contains(profile.CreatureId))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:87` · `profile` — `"Readability profile '" + profile.CreatureId +`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:103` · `profile` — `"Creature '" + profile.CreatureId + "' references missing/invalid behavior type '" +`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:115` · `profile` — `"Creature '" + profile.CreatureId + "' behavior source is missing: " + sourcePath);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:126` · `profile` — `"Creature '" + profile.CreatureId + "' has an empty state/evidence token.",`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:133` · `profile` — `"Creature '" + profile.CreatureId + "' state '" + state.StateName +`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:157` · `profile` — `"Creature '" + (profile != null ? profile.CreatureId : "<null>") +`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:25` · `profile` — `Assert.IsTrue(ids.Add(profile.CreatureId), "duplicate profile " + profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:28` · `profile` — `CreatureBehaviorReadabilityCatalog.MinimumActiveStates, profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:29` · `profile` — `Assert.IsFalse(string.IsNullOrWhiteSpace(profile.BehaviorSourceRelativePath), profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:30` · `profile` — `Assert.IsFalse(string.IsNullOrWhiteSpace(profile.FactoryEvidenceToken), profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:36` · `profile` — `StringAssert.Contains(profile.BehaviorTypeName, source, profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:40` · `profile` — `Assert.IsNotNull(state, profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:41` · `profile` — `Assert.IsFalse(string.IsNullOrWhiteSpace(state.StateName), profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:43` · `profile` — `profile.CreatureId + "/" + state.StateName);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:45` · `profile` — `profile.CreatureId + "/" + state.StateName + " evidence drifted");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:70` · `profile` — `Assert.IsTrue(assetIds.Contains(profile.CreatureId), "orphan profile " + profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:84` · `profile` — `profile.CreatureId + " factory wiring drifted");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:110` · `profile` — `Assert.IsNotNull(behaviorType, profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:32` · `profile` — `Assert.IsTrue(ids.Add(profile.CreatureId),`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:33` · `profile` — `"duplicate readability profile for " + profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:37` · `profile` — `profile.CreatureId + " readability profile invalid: " + string.Join(", ", errors));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:79` · `profile` — `Assert.IsTrue(assetIds.Contains(profile.CreatureId),`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:80` · `profile` — `"stale/invented readability profile has no shipped CreatureDefinition: " + profile.CreatureId);`

### `CreatureRuntime.CreatureDisabled`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:159` · `OnCreatureDisabled` — `CreatureRuntime.CreatureDisabled += OnCreatureDisabled;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:177` · `OnCreatureDisabled` — `CreatureRuntime.CreatureDisabled -= OnCreatureDisabled;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.CreatureDisabledSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs:25` · `OnDisabled` — `CreatureRuntime.CreatureDisabled += OnDisabled;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.EditMode.CreatureDisabledSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs:36` · `OnDisabled` — `CreatureRuntime.CreatureDisabled -= OnDisabled;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.CreatureDisabledSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs:105` · `thrower` — `CreatureRuntime.CreatureDisabled += thrower;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.EditMode.CreatureDisabledSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs:115` · `thrower` — `CreatureRuntime.CreatureDisabled -= thrower;`

### `DestinationSelected`

- **EVENT_DECLARE** · `Ziptide.Gameplay.ShipCastOffRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs:59` · `Action<string>` — `public event Action<string> DestinationSelected;`

### `DisabledState`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:266` · `profile` — `if (string.IsNullOrWhiteSpace(profile.DisabledState))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:268` · `profile` — `else if (states.Contains(profile.DisabledState))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:269` · `profile` — `errors.Add("DISABLED_STATE_DUPLICATES_ACTIVE:" + profile.DisabledState);`

### `DroneRuntime.OnDroneDisabled`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:63` · `OnDroneDisabled` — `DroneRuntime.OnDroneDisabled += OnDroneDisabled;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:71` · `OnDroneDisabled` — `DroneRuntime.OnDroneDisabled -= OnDroneDisabled;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:97` · `OnDroneDown` — `DroneRuntime.OnDroneDisabled += OnDroneDown;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:107` · `OnDroneDown` — `DroneRuntime.OnDroneDisabled -= OnDroneDown;`

### `EditorApplication.playModeStateChanged`

- **EVENT_SUBSCRIBE** · `Ziptide.Editor.DevTools.DevWarpPlayHook` · `Ziptide/Assets/Ziptide/Editor/DevTools/DevWarpPlayHook.cs:19` · `OnPlayModeChanged` — `EditorApplication.playModeStateChanged += OnPlayModeChanged;`

### `EditorSceneManager.sceneOpened`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Editor.Audit.AuditPhysicsSync` · `Ziptide/Assets/Ziptide/Editor/Audit/AuditPhysicsSync.cs:19` · `OnSceneOpened` — `EditorSceneManager.sceneOpened -= OnSceneOpened;`
- **EVENT_SUBSCRIBE** · `Ziptide.Editor.Audit.AuditPhysicsSync` · `Ziptide/Assets/Ziptide/Editor/Audit/AuditPhysicsSync.cs:20` · `OnSceneOpened` — `EditorSceneManager.sceneOpened += OnSceneOpened;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.HeadsetBuildBlockerRegressionTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs:71` · `OnSceneOpened` — `StringAssert.Contains("EditorSceneManager.sceneOpened += OnSceneOpened;", sync);`

### `EditorSceneManager.sceneSaving`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityRiverSaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverSaveHook.cs:21` · `OnSceneSaving` — `EditorSceneManager.sceneSaving -= OnSceneSaving;`
- **EVENT_SUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityRiverSaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityRiverSaveHook.cs:22` · `OnSceneSaving` — `EditorSceneManager.sceneSaving += OnSceneSaving;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityStageASaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageASaveHook.cs:21` · `OnSceneSaving` — `EditorSceneManager.sceneSaving -= OnSceneSaving;`
- **EVENT_SUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityStageASaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageASaveHook.cs:22` · `OnSceneSaving` — `EditorSceneManager.sceneSaving += OnSceneSaving;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityStageBSaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageBSaveHook.cs:17` · `OnSceneSaving` — `EditorSceneManager.sceneSaving -= OnSceneSaving;`
- **EVENT_SUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityStageBSaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityStageBSaveHook.cs:18` · `OnSceneSaving` — `EditorSceneManager.sceneSaving += OnSceneSaving;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityVehicleSaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleSaveHook.cs:17` · `OnSceneSaving` — `EditorSceneManager.sceneSaving -= OnSceneSaving;`
- **EVENT_SUBSCRIBE** · `Ziptide.Editor.Patching.ToxicCityVehicleSaveHook` · `Ziptide/Assets/Ziptide/Editor/Patching/ToxicCityVehicleSaveHook.cs:18` · `OnSceneSaving` — `EditorSceneManager.sceneSaving += OnSceneSaving;`

### `ExpectedArchetype`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:78` · `profile` — `if (definition.archetype != profile.ExpectedArchetype)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:81` · `profile` — `" disagrees with profile " + profile.ExpectedArchetype + ".", path);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:65` · `profile` — `Assert.AreEqual(definition.archetype, profile.ExpectedArchetype, definition.id);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorReadabilityTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs:61` · `profile` — `Assert.AreEqual(definition.archetype, profile.ExpectedArchetype,`

### `FactoryEvidenceToken`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:154` · `profile` — `if (profile == null || string.IsNullOrWhiteSpace(profile.FactoryEvidenceToken) ||`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Audit.CreatureBehaviorAuditRules` · `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs:155` · `profile` — `!factorySource.Contains(profile.FactoryEvidenceToken))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:30` · `profile` — `Assert.IsFalse(string.IsNullOrWhiteSpace(profile.FactoryEvidenceToken), profile.CreatureId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CreatureBehaviorAuditRulesTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs:83` · `profile` — `StringAssert.Contains(profile.FactoryEvidenceToken, source,`

### `Family`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Ship.VehicleRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleRuntime.cs:153` · `profile` — `+ " family=" + profile.Family + " parts=" + parts`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:18` · `profile` — `Assert.That(profile.Family, Is.EqualTo(family));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:37` · `profile` — `Assert.That(profile.Family, Is.EqualTo("utility"));`

### `FirstDestinationHelmRuntime.FirstDestinationSelected`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:156` · `OnFirstDestinationSelected` — `FirstDestinationHelmRuntime.FirstDestinationSelected += OnFirstDestinationSelected;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:174` · `OnFirstDestinationSelected` — `FirstDestinationHelmRuntime.FirstDestinationSelected -= OnFirstDestinationSelected;`

### `FirstDestinationSelected`

- **EVENT_DECLARE** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:14` · `static Action<string>` — `public static event Action<string> FirstDestinationSelected;`

### `FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:155` · `OnBunkObjectGrabbed` — `FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed += OnBunkObjectGrabbed;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:173` · `OnBunkObjectGrabbed` — `FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed -= OnBunkObjectGrabbed;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:208` · `handler` — `FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed += handler;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:220` · `handler` — `FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed -= handler;`

### `FirstHourObservationAdapter.Instance.SignalCompleted`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:163` · `Accept` — `FirstHourObservationAdapter.Instance.SignalCompleted += Accept;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:181` · `Accept` — `FirstHourObservationAdapter.Instance.SignalCompleted -= Accept;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.FirstHourDirectorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourDirectorTests.cs:107` · `Accept` — `StringAssert.Contains("FirstHourObservationAdapter.Instance.SignalCompleted += Accept", source);`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.EditMode.FirstHourDirectorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourDirectorTests.cs:108` · `Accept` — `StringAssert.Contains("FirstHourObservationAdapter.Instance.SignalCompleted -= Accept", source);`

### `GetResource`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.RecipeService` · `Ziptide/Assets/Ziptide/Content/Runtime/Economy/RecipeService.cs:21` · `profile` — `if (profile.GetResource(c.resourceId) < c.amount) return false;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:109` · `profile` — `if (profile.GetResource(resourceId) < amount) return false;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.CreditsHud` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/CreditsHud.cs:84` · `Profile` — `credits = (long)System.Math.Floor(save.Profile.GetResource(CreditsResourceId));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.BuildSocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:92` · `profile` — `if (profile.GetResource("credits") < _def.buildCost)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:88` · `profile` — `Assert.AreEqual(10.0, profile.GetResource("glowfruit_fruit"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:94` · `profile` — `Assert.AreEqual(10.0, profile.GetResource("glowfruit_fruit"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:140` · `profile` — `Assert.AreEqual(12.5, profile.GetResource("glowfruit_fruit"), 1e-9); // 10 * 1.25`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:80` · `profile` — `Assert.AreEqual(3, profile.GetResource("mineral"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:90` · `profile` — `Assert.AreEqual(2, profile.GetResource("spore"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:100` · `profile` — `Assert.AreEqual(1, profile.GetResource("stun_charge_cell"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:101` · `profile` — `Assert.AreEqual(0, profile.GetResource("spore"), "factory consumed the bio input");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:102` · `profile` — `Assert.AreEqual(1, profile.GetResource("mineral"), "3 - 2 consumed");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.HarvestServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:63` · `profile` — `Assert.AreEqual(5.0, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.HarvestServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:66` · `profile` — `Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.HarvestServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:100` · `profile` — `Assert.AreEqual(0.0, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.HarvestServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:152` · `profile` — `Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.HarvestServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HarvestServiceTests.cs:156` · `profile` — `Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9); // no further credit`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:33` · `profile` — `Assert.AreEqual(50, profile.GetResource("credits"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:34` · `profile` — `Assert.AreEqual(3, profile.GetResource("scrap"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:46` · `profile` — `Assert.AreEqual(50, profile.GetResource("credits"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:63` · `profile` — `Assert.AreEqual(60, profile.GetResource("credits"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:74` · `profile` — `Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9); // nothing deducted on failure`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:78` · `profile` — `Assert.AreEqual(6.0, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:79` · `profile` — `Assert.AreEqual(3.0, profile.GetResource("gear"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:106` · `profile` — `Assert.AreEqual(3.0, profile.GetResource("scrap"), 1e-9); // 8 - 5 build cost`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:128` · `profile` — `Assert.AreEqual(2.0, profile.GetResource("scrap"), 1e-9); // untouched`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:170` · `profile` — `Assert.AreEqual(100.0, profile.GetResource("ore"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ProfileEconomyTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:114` · `profile` — `Assert.AreEqual(42.0, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.SalvageCacheTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/SalvageCacheTests.cs:21` · `profile` — `Assert.AreEqual(7, profile.GetResource("scrap"), 1e-9);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.SalvageCacheTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/SalvageCacheTests.cs:36` · `profile` — `Assert.AreEqual(0, profile.GetResource("scrap"), 1e-9, "nothing credited");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:374` · `profile` — `Assert.AreEqual(ProbeAmount, profile.GetResource(ProbeResource),`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:387` · `profile` — `Assert.AreEqual(ProbeAmount, profile.GetResource(ProbeResource),`

### `GetWorld`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.WorldResolveResult` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ProfileEconomy.cs:66` · `profile` — `var world = profile.GetWorld(worldId, createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:119` · `profile` — `var world = profile.GetWorld(gameObject.scene.name, createIfMissing);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.BeltMinePortRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:54` · `profile` — `var world = profile.GetWorld(worldId, createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.CreatureRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:151` · `profile` — `profile.GetWorld(gameObject.scene.name, createIfMissing: true).ecologyPressures,`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.EcologyDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:95` · `profile` — `? profile.GetWorld(world, createIfMissing: true).ecologyPressures`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.BuildSocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:47` · `profile` — `return profile != null ? profile.GetWorld(_worldId, createIfMissing: true) : null;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.GardenPlotRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:91` · `profile` — `var world = profile.GetWorld(_worldId, createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.GardenPlotRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:123` · `profile` — `var world = profile.GetWorld(_worldId, createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.MiningRigRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:42` · `profile` — `var world = profile.GetWorld(_worldId, createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:57` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:80` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:107` · `profile` — `var world = profile.GetWorld("w", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:124` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenServiceTests.cs:166` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenTimingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs:42` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenTimingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs:142` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GardenTimingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GardenTimingTests.cs:160` · `profile` — `var world = profile.GetWorld("garden_world", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:83` · `profile` — `var world = profile.GetWorld("W002_DryCistern", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:94` · `profile` — `var world = profile.GetWorld("d0_city", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:121` · `profile` — `var world = profile.GetWorld("d0_city", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:141` · `profile` — `var world = profile.GetWorld("w", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.MiningServiceTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/MiningServiceTests.cs:158` · `profile` — `var world = profile.GetWorld("d0_city", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ProfileEconomyTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:71` · `profile` — `var w = profile.GetWorld("ToxicCity", createIfMissing: true);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ProfileEconomyTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:79` · `profile` — `Assert.IsTrue(profile.GetWorld("ToxicCity").discovered); // entry marks discovery`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ProfileEconomyTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:80` · `profile` — `Assert.AreEqual(1100L, profile.GetWorld("ToxicCity").lastResolvedAtUnix);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ProfileEconomyTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:87` · `profile` — `Assert.IsNull(profile.GetWorld("NewWorld")); // not present yet`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ProfileEconomyTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ProfileEconomyTests.cs:92` · `profile` — `var ws = profile.GetWorld("NewWorld");`

### `Glow`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:23` · `profile` — `Assert.That(profile.Glow.maxColorComponent, Is.GreaterThan(0.75f));`

### `GoldenSlice`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RecoveryPlayerSurfacePolicy` · `Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryPlayerSurfacePolicy.cs:21` · `profile` — `// profile. GoldenSlice and Diagnostic inherit the clean player-view contract.`

### `HasFlag`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.ProductionGraph` · `Ziptide/Assets/Ziptide/Content/Runtime/Economy/ProductionGraph.cs:75` · `profile` — `if (!string.IsNullOrEmpty(r.unlockFlag) && !profile.HasFlag(r.unlockFlag)) continue;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.WorldGating` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:28` · `profile` — `if (!profile.HasFlag(flag)) return false;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.WorldGating` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:44` · `profile` — `if (profile == null || !profile.HasFlag(flag)) return flag;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.WorldGating` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:62` · `profile` — `if (string.IsNullOrEmpty(flag) || profile.HasFlag(flag)) continue;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:35` · `profile` — `if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_A)) return RillMemoryState.EndgameA;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:36` · `profile` — `if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_B)) return RillMemoryState.EndgameB;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:37` · `profile` — `if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_C)) return RillMemoryState.EndgameC;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:38` · `profile` — `if (profile.HasFlag(ZiptideFlags.C12_W063_ENDING_D)) return RillMemoryState.EndgameD;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:40` · `profile` — `if (profile.HasFlag(ZiptideFlags.C6_W051_RILL_NAMED)) return RillMemoryState.Integrated;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:41` · `profile` — `if (profile.HasFlag(ZiptideFlags.W028_COMPLETE) ||`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:42` · `profile` — `profile.HasFlag(ZiptideFlags.C4_W028_NO_JOB)) return RillMemoryState.Unsealing;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:43` · `profile` — `if (profile.HasFlag(ZiptideFlags.C2_CONTAINMENT_REVEALED) ||`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:44` · `profile` — `profile.HasFlag(ZiptideFlags.C3_W013_MEMORY_SHARD)) return RillMemoryState.Remembering;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.RillState` · `Ziptide/Assets/Ziptide/Core/Runtime/RillState.cs:45` · `profile` — `if (profile.HasFlag(ZiptideFlags.W004_COMPLETE)) return RillMemoryState.Stirring;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.SignalState` · `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs:23` · `profile` — `if (profile.HasFlag(ZiptideFlags.SIGNAL_MAX)) return TierMax;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.SignalState` · `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs:24` · `profile` — `if (profile.HasFlag(ZiptideFlags.SIGNAL_THRESHOLD_3)) return 3;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.SignalState` · `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs:25` · `profile` — `if (profile.HasFlag(ZiptideFlags.SIGNAL_THRESHOLD_2)) return 2;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.SignalState` · `Ziptide/Assets/Ziptide/Core/Runtime/SignalState.cs:26` · `profile` — `if (profile.HasFlag(ZiptideFlags.SIGNAL_THRESHOLD_1)) return 1;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:27` · `profile` — `bool t1 = profile.HasFlag(ZiptideFlags.FRAGMENT_T1_FOUND);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:28` · `profile` — `bool t2 = profile.HasFlag(ZiptideFlags.FRAGMENT_T2_FOUND);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:29` · `profile` — `bool t3 = profile.HasFlag(ZiptideFlags.FRAGMENT_T3_FOUND);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:30` · `profile` — `bool t4 = profile.HasFlag(ZiptideFlags.FRAGMENT_T4_FOUND);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:31` · `profile` — `bool t5 = profile.HasFlag(ZiptideFlags.FRAGMENT_T5_FOUND);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:32` · `profile` — `bool rill = profile.HasFlag(ZiptideFlags.FRAGMENT_RILL_CONFESS);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:62` · `profile` — `if (profile.HasFlag(flag)) return 0;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.WardenBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/WardenBehavior.cs:45` · `profile` — `bool ally = profile != null && profile.HasFlag(ZiptideFlags.C6_WARDEN_ALLY);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourHolsterSignal` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/FirstHourHolsterSignal.cs:21` · `profile` — `if (profile != null && profile.HasFlag(ZiptideFlags.FIRST_HOLSTER))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ReleaseFeel` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ReleaseFeel.cs:129` · `profile` — `if (profile == null || profile.HasFlag(ZiptideFlags.FIRST_RELEASE)) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ArenaLobbyBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:250` · `profile` — `return !profile.HasFlag(col == 2 ? PvpProgression.FlagVeteranUnlocked`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:134` · `profile` — `if (profile.HasFlag(flag)) continue;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:63` · `profile` — `_joined = profile != null && profile.HasFlag(ZiptideFlags.ARTIFACT_JOINED);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:110` · `profile` — `return profile.HasFlag(ZiptideFlags.ARTIFACT_HALF_A)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:111` · `profile` — `&& profile.HasFlag(ZiptideFlags.ARTIFACT_HALF_B);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ChoiceStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:35` · `profile` — `bool aTaken = profile != null && !string.IsNullOrEmpty(_def.flagA) && profile.HasFlag(_def.flagA);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ChoiceStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:36` · `profile` — `bool bTaken = profile != null && !string.IsNullOrEmpty(_def.flagB) && profile.HasFlag(_def.flagB);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.KeySocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/KeySocketRuntime.cs:43` · `profile` — `if (profile != null && profile.HasFlag(ZiptideFlags.KEY_SEATED))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:104` · `profile` — `if (_scratch[i].once && profile != null && profile.HasFlag(SaidFlagPrefix + _scratch[i].id))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:113` · `profile` — `if (profile.HasFlag(SaidFlagPrefix + line.id)) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:135` · `profile` — `if (profile.HasFlag(SaidFlagPrefix + line.id)) return false;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:153` · `profile` — `if (line.once && profile != null && profile.HasFlag(SaidFlagPrefix + line.id)) continue;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SpaceSalvageItemRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/SpaceSalvageItemRuntime.cs:44` · `profile` — `if (profile != null && !string.IsNullOrEmpty(grantsFlag) && profile.HasFlag(grantsFlag))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourW001Orchestrator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:159` · `profile` — `if (profile == null || !profile.HasFlag(ZiptideFlags.W001_COMPLETE)) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ProximityTravelTrigger` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ProximityTravelTrigger.cs:47` · `profile` — `if (profile == null || !profile.HasFlag(requiredFlag))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.QuartersRoom` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:152` · `profile` — `bool isOwned = string.IsNullOrEmpty(c.ownedFlag) || (profile != null && profile.HasFlag(c.ownedFlag));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:34` · `profile` — `Assert.IsTrue(profile.HasFlag(ZiptideFlags.FIRST_HOLSTER));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:55` · `profile` — `Assert.IsFalse(profile.HasFlag(ZiptideFlags.FIRST_HOLSTER));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:119` · `profile` — `Assert.IsTrue(profile.HasFlag(ZiptideFlags.FIRST_HOLSTER));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:45` · `profile` — `Assert.IsTrue(profile.HasFlag("toxiccity_complete"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:87` · `profile` — `Assert.IsTrue(profile.HasFlag("C1_W001_RILL_BOOT"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:88` · `profile` — `Assert.IsTrue(profile.HasFlag("SIGNAL_THRESHOLD_1"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:89` · `profile` — `Assert.IsTrue(profile.HasFlag("W001_COMPLETE"));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:373` · `profile` — `Assert.IsTrue(profile.HasFlag(ProbeFlag), "Recovery probe flag was lost from live state.");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:386` · `profile` — `Assert.IsTrue(profile.HasFlag(ProbeFlag), "Recovery probe flag was not persisted to disk.");`

### `HolsterSocketInteractor.ItemHolstered`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:157` · `OnItemHolstered` — `HolsterSocketInteractor.ItemHolstered += OnItemHolstered;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:175` · `OnItemHolstered` — `HolsterSocketInteractor.ItemHolstered -= OnItemHolstered;`

### `HomeHubRuntime.BootPresentationReady`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:152` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady += OnBootReady;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:170` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady -= OnBootReady;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:38` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady += OnBootReady;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:45` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady -= OnBootReady;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:43` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady += OnBootReady;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:55` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady -= OnBootReady;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:56` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady += OnBootReady;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:68` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady -= OnBootReady;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:66` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady += OnBootReady;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:78` · `OnBootReady` — `HomeHubRuntime.BootPresentationReady -= OnBootReady;`

### `HomeHubRuntime.ChoiceSelected`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.HomeHubAnchorLockRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockRuntime.cs:33` · `OnChoiceSelected` — `HomeHubRuntime.ChoiceSelected += OnChoiceSelected;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.HomeHubAnchorLockRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockRuntime.cs:39` · `OnChoiceSelected` — `HomeHubRuntime.ChoiceSelected -= OnChoiceSelected;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.HomeHubAnchorLockRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubAnchorLockRuntime.cs:45` · `OnChoiceSelected` — `HomeHubRuntime.ChoiceSelected -= OnChoiceSelected;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:44` · `OnChoice` — `HomeHubRuntime.ChoiceSelected += OnChoice;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:56` · `OnChoice` — `HomeHubRuntime.ChoiceSelected -= OnChoice;`

### `HomeHubRuntime.NewGameProfileCreated`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:153` · `OnNewGameProfile` — `HomeHubRuntime.NewGameProfileCreated += OnNewGameProfile;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:171` · `OnNewGameProfile` — `HomeHubRuntime.NewGameProfileCreated -= OnNewGameProfile;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:57` · `OnNewGameProfileCreated` — `HomeHubRuntime.NewGameProfileCreated += OnNewGameProfileCreated;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:69` · `OnNewGameProfileCreated` — `HomeHubRuntime.NewGameProfileCreated -= OnNewGameProfileCreated;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:67` · `OnNewGameProfileCreated` — `HomeHubRuntime.NewGameProfileCreated += OnNewGameProfileCreated;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:79` · `OnNewGameProfileCreated` — `HomeHubRuntime.NewGameProfileCreated -= OnNewGameProfileCreated;`

### `HomeHubRuntime.SettingsRequested`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:45` · `OnSettingsRequested` — `HomeHubRuntime.SettingsRequested += OnSettingsRequested;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryBootSceneSmokeTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryBootSceneSmokeTests.cs:57` · `OnSettingsRequested` — `HomeHubRuntime.SettingsRequested -= OnSettingsRequested;`

### `Idempotent`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:10` · `profile` — `/// runtime, from the profile. Idempotent + re-runnable (the hangar calls it live on every equip):`

### `InputSystem.onAfterUpdate`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryVirtualXrLayoutBootstrap` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:59` · `AuditBilateralBindings` — `InputSystem.onAfterUpdate -= AuditBilateralBindings;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryVirtualXrLayoutBootstrap` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:60` · `AuditBilateralBindings` — `InputSystem.onAfterUpdate += AuditBilateralBindings;`

### `InputSystem.onDeviceChange`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryVirtualXrLayoutBootstrap` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:57` · `OnDeviceChange` — `InputSystem.onDeviceChange -= OnDeviceChange;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryVirtualXrLayoutBootstrap` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryVirtualXrLayoutBootstrap.cs:58` · `OnDeviceChange` — `InputSystem.onDeviceChange += OnDeviceChange;`

### `ItemHolstered`

- **EVENT_DECLARE** · `Ziptide.Gameplay.HolsterSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:30` · `static Action<string>` — `public static event Action<string> ItemHolstered;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.HolsterSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:214` — `ItemHolstered?.Invoke(itemId);`

### `JobCompleted`

- **EVENT_DECLARE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:14` · `Action` — `public event Action JobCompleted;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:182` — `JobCompleted?.Invoke();`

### `Joined`

- **EVENT_DECLARE** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:41` · `static System.Action` — `public static event System.Action Joined;`

### `KillScored`

- **EVENT_DECLARE** · `Ziptide.Gameplay.PvpMatchDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:22` · `System.Action<int, int>` — `public event System.Action<int, int> KillScored;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.PvpMatchDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:86` — `KillScored?.Invoke(killer, killedIndex);`

### `Length`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.ForgeMesh` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMesh.cs:406` · `profile` — `if (profile == null || profile.Length < 2) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.ForgeMesh` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeMesh.cs:408` · `profile` — `int rings = profile.Length;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.ForgePart` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeRecipeDefinition.cs:265` · `profile` — `if (p.op == ForgeOp.Lathe && (p.profile == null || p.profile.Length < 2 || p.profile.Length > 8))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.ForgePart` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeRecipeDefinition.cs:287` · `profile` — `if (p.profile != null && p.profile.Length > 0)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.ForgePart` · `Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeRecipeDefinition.cs:289` · `profile` — `if (p.profile.Length != p.spline.Length)`

### `Lets`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DevTools.DevWarp` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarp.cs:10` · `profile` — `/// the explicit diagnostic exposure profile. Lets us jump straight to any world (and a named spawn`

### `MatchEnded`

- **EVENT_DECLARE** · `Ziptide.Gameplay.PvpMatchDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:24` · `System.Action<int>` — `public event System.Action<int> MatchEnded;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.PvpMatchDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:102` — `MatchEnded?.Invoke(_match.WinnerIndex);`

### `MatchRestarted`

- **EVENT_DECLARE** · `Ziptide.Gameplay.PvpMatchDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:26` · `System.Action` — `public event System.Action MatchRestarted;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.PvpMatchDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpMatchDirector.cs:116` — `MatchRestarted?.Invoke();`

### `Merely`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:29` · `profile` — `/// True only when the atomic save store can supply a valid main or backup profile. Merely`

### `MinAttackOdds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Multiplayer.Conquest.ConquestAction` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestAI.cs:134` · `profile` — `if (odds >= profile.MinAttackOdds && odds > bestOdds)`

### `MinimumParts`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.VehiclePresentationCoreTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VehiclePresentationCoreTests.cs:19` · `profile` — `Assert.That(profile.MinimumParts, Is.GreaterThanOrEqualTo(minimumParts));`

### `NamedBunkObjectGrabbed`

- **EVENT_DECLARE** · `Ziptide.Gameplay.FirstDestinationHelmRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs:111` · `static Action<string>` — `public static event Action<string> NamedBunkObjectGrabbed;`

### `NewGameProfileCreated`

- **EVENT_DECLARE** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:115` · `static Action<PlayerProfile>` — `public static event Action<PlayerProfile> NewGameProfileCreated;`

### `Null`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.WorldGating` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:50` · `profile` — `/// Set every flag in <see cref="WorldPackDefinition.flagsGranted"/> on the profile. Null-safe and`

### `OnDroneDisabled`

- **EVENT_DECLARE** · `Ziptide.Gameplay.DroneRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:47` · `static System.Action<DroneRuntime>` — `public static event System.Action<DroneRuntime> OnDroneDisabled;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.DroneRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneRuntime.cs:187` — `OnDroneDisabled?.Invoke(this);`

### `OnFire`

- **EVENT_DECLARE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:18` · `Action<FireMsg>` — `public event Action<FireMsg> OnFire;`
- **EVENT_INVOKE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:24` — `public void SendFire(FireMsg msg) => OnFire?.Invoke(msg);`
- **EVENT_DECLARE** · `Ziptide.Multiplayer.PlayerPoseMsg` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:84` · `Action<FireMsg>` — `event Action<FireMsg> OnFire;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:156` · `System.Action<FireMsg>` — `public event System.Action<FireMsg> OnFire;`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:161` — `public void SendFire(FireMsg m) => OnFire?.Invoke(m);`
- **EVENT_DECLARE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:37` · `Action<FireMsg>` — `public event Action<FireMsg> OnFire;`
- **EVENT_INVOKE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:91` — `OnFire?.Invoke(new FireMsg`

### `OnHit`

- **EVENT_INVOKE** · `Ziptide.Gameplay.TargetRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Targets/TargetRuntime.cs:63` — `OnHit?.Invoke();`
- **EVENT_DECLARE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:19` · `Action<HitMsg>` — `public event Action<HitMsg> OnHit;`
- **EVENT_INVOKE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:25` — `public void SendHit(HitMsg msg) => OnHit?.Invoke(msg);`
- **EVENT_DECLARE** · `Ziptide.Multiplayer.PlayerPoseMsg` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:85` · `Action<HitMsg>` — `event Action<HitMsg> OnHit;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:157` · `System.Action<HitMsg>` — `public event System.Action<HitMsg> OnHit;`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:162` — `public void SendHit(HitMsg m) => OnHit?.Invoke(m);`
- **EVENT_DECLARE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:38` · `Action<HitMsg>` — `public event Action<HitMsg> OnHit;`
- **EVENT_INVOKE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:98` — `OnHit?.Invoke(new HitMsg`

### `OnPlayerStunned`

- **EVENT_DECLARE** · `Ziptide.Gameplay.PlayerStunReceiver` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:16` · `static System.Action` — `public static event System.Action OnPlayerStunned;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.PlayerStunReceiver` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerStunReceiver.cs:65` — `OnPlayerStunned?.Invoke();`

### `OnPose`

- **EVENT_DECLARE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:17` · `Action<PlayerPoseMsg>` — `public event Action<PlayerPoseMsg> OnPose;`
- **EVENT_INVOKE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:23` — `public void SendPose(PlayerPoseMsg msg) => OnPose?.Invoke(msg);`
- **EVENT_DECLARE** · `Ziptide.Multiplayer.PlayerPoseMsg` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:83` · `Action<PlayerPoseMsg>` — `event Action<PlayerPoseMsg> OnPose;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:155` · `System.Action<PlayerPoseMsg>` — `public event System.Action<PlayerPoseMsg> OnPose;`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:160` — `public void SendPose(PlayerPoseMsg m) => OnPose?.Invoke(m);`
- **EVENT_DECLARE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:36` · `Action<PlayerPoseMsg>` — `public event Action<PlayerPoseMsg> OnPose;`
- **EVENT_INVOKE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:81` — `OnPose?.Invoke(new PlayerPoseMsg`

### `OnScore`

- **EVENT_DECLARE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:20` · `Action<ScoreMsg>` — `public event Action<ScoreMsg> OnScore;`
- **EVENT_INVOKE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:26` — `public void SendScore(ScoreMsg msg) => OnScore?.Invoke(msg);`
- **EVENT_DECLARE** · `Ziptide.Multiplayer.PlayerPoseMsg` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:86` · `Action<ScoreMsg>` — `event Action<ScoreMsg> OnScore;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:158` · `System.Action<ScoreMsg>` — `public event System.Action<ScoreMsg> OnScore;`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:163` — `public void SendScore(ScoreMsg m) => OnScore?.Invoke(m);`
- **EVENT_DECLARE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:39` · `Action<ScoreMsg>` — `public event Action<ScoreMsg> OnScore;`
- **EVENT_INVOKE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:105` — `OnScore?.Invoke(new ScoreMsg`

### `OnWall`

- **EVENT_DECLARE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:21` · `Action<WallMsg>` — `public event Action<WallMsg> OnWall;`
- **EVENT_INVOKE** · `Ziptide.Multiplayer.LoopbackPvpTransport` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/LoopbackPvpTransport.cs:27` — `public void SendWall(WallMsg msg) => OnWall?.Invoke(msg);`
- **EVENT_DECLARE** · `Ziptide.Multiplayer.PlayerPoseMsg` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNet.cs:87` · `Action<WallMsg>` — `event Action<WallMsg> OnWall;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:159` · `System.Action<WallMsg>` — `public event System.Action<WallMsg> OnWall;`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:164` — `public void SendWall(WallMsg m) => OnWall?.Invoke(m);`
- **EVENT_DECLARE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:40` · `Action<WallMsg>` — `public event Action<WallMsg> OnWall;`
- **EVENT_INVOKE** · `ZiptideNet.PhotonPvpTransport` · `Ziptide/Assets/ZiptideNet/PhotonPvpTransport.cs:112` — `OnWall?.Invoke(new WallMsg { panelId = (int)d[0], state = (int)d[1] });`

### `OnlineStopper`

- **EVENT_INVOKE** · `Ziptide.Multiplayer.PvpNetHub` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs:62` — `OnlineStopper?.Invoke();`

### `PlayerPrefs.DeleteKey`

- **PLAYER_PREFS_ACCESS** · `Ziptide.Tests.EditMode.ComfortSettingsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:15` — `PlayerPrefs.DeleteKey(ComfortSettings.PresetPrefKey);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Tests.EditMode.ComfortSettingsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:16` — `PlayerPrefs.DeleteKey("ziptide_comfort_vignette");`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Tests.EditMode.ComfortSettingsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:22` — `PlayerPrefs.DeleteKey(ComfortSettings.PresetPrefKey);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Tests.EditMode.ComfortSettingsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:23` — `PlayerPrefs.DeleteKey("ziptide_comfort_vignette");`

### `PlayerPrefs.GetFloat`

- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:92` — `PlayerPrefs.GetFloat(AudioMixCore.MasterPrefKey, AudioMixCore.DefaultMaster),`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:98` — `float stored = PlayerPrefs.GetFloat(AudioMixCore.PrefKey(bus), AudioMixCore.DefaultFor(bus));`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.ComfortVignette` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:51` — `_strength = Mathf.Clamp01(PlayerPrefs.GetFloat(PrefKey, DefaultStrength));`

### `PlayerPrefs.GetInt`

- **PLAYER_PREFS_ACCESS** · `Ziptide.Core.ComfortDialSet` · `Ziptide/Assets/Ziptide/Core/Runtime/ComfortSettings.cs:74` — `int raw = PlayerPrefs.GetInt(PresetPrefKey, (int)DefaultPreset);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.ClimbableSurface` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs:166` — `if (PlayerPrefs.GetInt("ziptide_climb_fling", 0) == 1 && flingV.sqrMagnitude > 0.25f)`

### `PlayerPrefs.Save`

- **PLAYER_PREFS_ACCESS** · `Ziptide.Core.ComfortDialSet` · `Ziptide/Assets/Ziptide/Core/Runtime/ComfortSettings.cs:86` — `PlayerPrefs.Save();`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:49` — `PlayerPrefs.Save();`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:72` — `PlayerPrefs.Save();`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:82` — `PlayerPrefs.Save();`

### `PlayerPrefs.SetFloat`

- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:48` — `PlayerPrefs.SetFloat(AudioMixCore.PrefKey(bus), clamped);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:65` — `PlayerPrefs.SetFloat(AudioMixCore.MasterPrefKey, _master);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:70` — `PlayerPrefs.SetFloat(AudioMixCore.PrefKey(bus), _buses[i]);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.AudioMixSettings` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioMixSettings.cs:81` — `PlayerPrefs.SetFloat(key, clamped);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Gameplay.ComfortVignette` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs:69` — `PlayerPrefs.SetFloat(PrefKey, _strength);`

### `PlayerPrefs.SetInt`

- **PLAYER_PREFS_ACCESS** · `Ziptide.Core.ComfortDialSet` · `Ziptide/Assets/Ziptide/Core/Runtime/ComfortSettings.cs:85` — `PlayerPrefs.SetInt(PresetPrefKey, (int)preset);`
- **PLAYER_PREFS_ACCESS** · `Ziptide.Tests.EditMode.ComfortSettingsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs:97` — `PlayerPrefs.SetInt(ComfortSettings.PresetPrefKey, 999);`

### `PresetConfirmed`

- **EVENT_DECLARE** · `Ziptide.Gameplay.ComfortConsoleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs:15` · `static Action<ComfortPreset>` — `public static event Action<ComfortPreset> PresetConfirmed;`

### `Pure`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.HarvestPlantResult` · `Ziptide/Assets/Ziptide/Content/Runtime/Economy/GardenService.cs:48` · `profile` — `/// real time (idle), then harvest with the right tool to credit the yield to the profile. Pure`

### `PvpNetHub.TransportChanged`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpOnlinePresence` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:45` · `Rebind` — `PvpNetHub.TransportChanged += Rebind;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpOnlinePresence` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:51` · `Rebind` — `PvpNetHub.TransportChanged -= Rebind;`

### `ReinforcesBorders`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Multiplayer.Conquest.ConquestAction` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestAI.cs:52` · `profile` — `if (profile.ReinforcesBorders)`

### `RideEnded`

- **EVENT_DECLARE** · `Ziptide.Gameplay.ZiplineRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:31` · `System.Action<string, float>` — `public event System.Action<string, float> RideEnded;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:126` · `System.Action<string, float>` — `StringAssert.Contains("public event System.Action<string, float> RideEnded;", source);`

### `RideStarted`

- **EVENT_DECLARE** · `Ziptide.Gameplay.ZiplineRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:28` · `System.Action` — `public event System.Action RideStarted;`
- **EVENT_DECLARE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:125` · `System.Action` — `StringAssert.Contains("public event System.Action RideStarted;", source);`

### `Root.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTestRig` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTestRig.cs:121` · `delta` — `Root.transform.position += delta;`

### `SaveSystem.AutosaveNow`

- **AUTOSAVE** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:169` · `belt_edit` — `if (autosave) SaveSystem.AutosaveNow("belt_edit");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:169` — `if (autosave) SaveSystem.AutosaveNow("belt_edit");`
- **AUTOSAVE** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:195` · `belt_stamp` — `SaveSystem.AutosaveNow("belt_stamp");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:195` — `SaveSystem.AutosaveNow("belt_stamp");`
- **AUTOSAVE** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:207` · `belt_edit` — `SaveSystem.AutosaveNow("belt_edit");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:207` — `SaveSystem.AutosaveNow("belt_edit");`
- **AUTOSAVE** · `Ziptide.Gameplay.SystemFocusStateMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:139` · `system_overlay` — `SaveSystem.AutosaveNow("system_overlay");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.SystemFocusStateMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:139` — `SaveSystem.AutosaveNow("system_overlay");`
- **AUTOSAVE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:143` · `pvp_match` — `SaveSystem.AutosaveNow("pvp_match");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:143` — `SaveSystem.AutosaveNow("pvp_match");`
- **AUTOSAVE** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:119` · `travel_fallback` — `SaveSystem.AutosaveNow("travel_fallback");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:119` — `SaveSystem.AutosaveNow("travel_fallback");`
- **AUTOSAVE** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:187` · `travel` — `SaveSystem.AutosaveNow("travel");`
- **SAVE_ACCESS** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:187` — `SaveSystem.AutosaveNow("travel");`
- **AUTOSAVE** · `Ziptide.Tests.EditMode.SaveAutosaveTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs:17` · `travel` — `Assert.DoesNotThrow(() => SaveSystem.AutosaveNow("travel"));`
- **SAVE_ACCESS** · `Ziptide.Tests.EditMode.SaveAutosaveTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/SaveAutosaveTests.cs:17` — `Assert.DoesNotThrow(() => SaveSystem.AutosaveNow("travel"));`

### `SaveSystem.HasExistingProfile`

- **SAVE_ACCESS** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:144` — `bool canContinue = SaveSystem.HasExistingProfile;`
- **SAVE_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:85` — `Assert.IsFalse(SaveSystem.HasExistingProfile,`
- **SAVE_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:95` — `Assert.IsFalse(SaveSystem.HasExistingProfile,`

### `SaveSystem.Load`

- **SAVE_ACCESS** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:261` — `SaveSystem.Instance.Load();`
- **SAVE_ACCESS** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:164` — `StringAssert.Contains("SaveSystem.Instance.Load();", source);`
- **SAVE_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:153` — `SaveSystem.Instance.Load();`

### `SaveSystem.Profile`

- **SAVE_ACCESS** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:117` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:618` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BeltMinePortRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:52` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.CreatureRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:141` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.EcologyDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:93` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.NestRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/NestRuntime.cs:125` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.WardenBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/WardenBehavior.cs:43` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.HolsterSocketInteractor` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs:173` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ItemFactory` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs:87` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ReleaseFeel` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ReleaseFeel.cs:128` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:44` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:373` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:48` — `/// economy/bounty payout use SaveSystem.Instance.Profile from anywhere.`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ArenaLobbyBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs:248` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:93` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:62` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:108` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:160` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BuildSocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:46` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.BuildSocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/BuildSocketRuntime.cs:88` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ChoiceStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:34` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ChoiceStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:112` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:118` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.GardenPlotRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:89` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.GardenPlotRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/GardenPlotRuntime.cs:121` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.KeySocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/KeySocketRuntime.cs:42` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.KeySocketRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/KeySocketRuntime.cs:104` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.MiningRigRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:40` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.MiningRigRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:139` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:97` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:132` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:146` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:175` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:290` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.SpaceSalvageItemRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/SpaceSalvageItemRuntime.cs:43` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.SpaceSalvageItemRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/SpaceSalvageItemRuntime.cs:117` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.TransmissionConsole` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/TransmissionConsole.cs:69` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:128` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:366` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:373` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.FirstHourW001Orchestrator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:158` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:361` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:76` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:84` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:104` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:414` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:96` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:108` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ProximityTravelTrigger` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ProximityTravelTrigger.cs:46` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.QuartersPhotoWall` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:25` — `PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.QuartersRoom` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:143` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.QuartersRoom` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:188` — `var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.QuartersRoom` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:219` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.SalvageCacheRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/SalvageCacheRuntime.cs:84` — `double granted = GrantTo(SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null,`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:81` — `var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:111` — `var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:161` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:181` — `SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null) ?? "?")))`
- **SAVE_ACCESS** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:33` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.WorldTravelStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:64` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Gameplay.WorldTravelStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs:149` — `+ " missing=" + (WorldGating.FirstMissingRequirement(pack, SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null) ?? "?"));`
- **SAVE_ACCESS** · `Ziptide.Ship.ShipFlightRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:121` — `var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;`
- **SAVE_ACCESS** · `Ziptide.Ship.SpaceTargetRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/SpaceTargetRuntime.cs:125` — `SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null,`
- **SAVE_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:132` — `PlayerProfile live = SaveSystem.Instance.Profile;`
- **SAVE_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:370` — `PlayerProfile profile = SaveSystem.Instance.Profile;`

### `SaveSystem.StartNewProfile`

- **SAVE_ACCESS** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:255` — `PlayerProfile profile = SaveSystem.Instance.StartNewProfile();`
- **SAVE_ACCESS** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:163` — `StringAssert.Contains("SaveSystem.Instance.StartNewProfile()", source);`

### `SavesForBigShips`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Multiplayer.Conquest.ConquestAction` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestAI.cs:73` · `profile` — `string pick = profile.SavesForBigShips && me.CanAfford(3, 4, 0) ? "siege_lantern"`

### `ScanResultPublished`

- **EVENT_DECLARE** · `Ziptide.Gameplay.WristScanner` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanner.cs:43` · `static System.Action<WristScanResult>` — `public static event System.Action<WristScanResult> ScanResultPublished;`

### `SceneManager.sceneLoaded`

- **EVENT_SUBSCRIBE** · `Ziptide.Core.GamePool` · `Ziptide/Assets/Ziptide/Core/Runtime/GamePool.cs:50` · `(_, __) =>` — `SceneManager.sceneLoaded += (_, __) => ResetForNewScene();`
- **EVENT_SUBSCRIBE** · `Ziptide.Core.RuntimeHealthMonitor` · `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs:49` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Core.RuntimeHealthMonitor` · `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs:55` · `OnSceneLoaded` — `if (_instance == this) { SceneManager.sceneLoaded -= OnSceneLoaded; _instance = null; }`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.AmbienceDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs:58` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.AmbienceDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs:64` · `OnSceneLoaded` — `if (_instance == this) { SceneManager.sceneLoaded -= OnSceneLoaded; _instance = null; }`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:40` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:48` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.DevTools.DevWarpBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:79` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.DevTools.DevWarpBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs:82` · `OnSceneLoaded` — `private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.SingletonValidator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Diagnostics/SingletonValidator.cs:31` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.SingletonValidator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Diagnostics/SingletonValidator.cs:38` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.EcologyDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs:44` · `(scene, mode) =>` — `SceneManager.sceneLoaded += (scene, mode) => Ensure(scene);`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.QuartersCameraFeature` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:16` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.QuartersCameraFeature` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs:17` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.LevelStateContract` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/LevelStateContract.cs:41` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.LevelStateContract` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/LevelStateContract.cs:46` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PlayerInputSessionGuard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:32` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PlayerInputSessionGuard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:40` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PlayerInputSessionGuard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs:41` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PlayerMenuRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:36` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PlayerMenuRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerMenuRuntime.cs:41` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BootHoldState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:159` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.BootHoldState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:350` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.TurnModeCore` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs:152` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.TurnModeCore` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs:158` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.QuestDeviceCorrectionsRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs:45` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.QuestDeviceCorrectionsRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs:51` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:49` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:55` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:56` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:61` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:150` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:168` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourW001Orchestrator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:66` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourW001Orchestrator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs:72` · `OnSceneLoaded` — `SceneManager.sceneLoaded -= OnSceneLoaded;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:45` · `OnSceneLoaded` — `SceneManager.sceneLoaded += OnSceneLoaded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Ship.VehicleSafetyRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs:38` · `OnSceneLoadedStatic` — `if (_hooked) SceneManager.sceneLoaded -= OnSceneLoadedStatic;`
- **EVENT_SUBSCRIBE** · `Ziptide.Ship.VehicleSafetyRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs:47` · `OnSceneLoadedStatic` — `SceneManager.sceneLoaded += OnSceneLoadedStatic;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.AudioDirectorLifecycleTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs:58` · `OnSceneLoaded` — `Assert.AreEqual(1, Count(source, "SceneManager.sceneLoaded += OnSceneLoaded;"));`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.EditMode.AudioDirectorLifecycleTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs:59` · `OnSceneLoaded` — `Assert.AreEqual(1, Count(source, "SceneManager.sceneLoaded -= OnSceneLoaded;"));`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.FieldCameraCompletionTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FieldCameraCompletionTests.cs:82` · `OnSceneLoaded` — `StringAssert.Contains("SceneManager.sceneLoaded += OnSceneLoaded", feature);`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:44` · `OnBootSceneLoadedBeforeStart` — `SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:63` · `OnBootSceneLoadedBeforeStart` — `SceneManager.sceneLoaded += OnBootSceneLoadedBeforeStart;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryActualSceneSnapshotTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryActualSceneSnapshotTests.cs:76` · `OnBootSceneLoadedBeforeStart` — `SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;`

### `SceneManager.sceneUnloaded`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ReentryArrivalRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ReentryArrivalRuntime.cs:61` · `OnSceneUnloaded` — `SceneManager.sceneUnloaded += OnSceneUnloaded;`

### `SetFlag`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.JobRewards` · `Ziptide/Assets/Ziptide/Content/Runtime/Jobs/JobRewards.cs:34` · `profile` — `profile.SetFlag(job.completionFlag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.WorldGating` · `Ziptide/Assets/Ziptide/Content/Runtime/WorldPacks/WorldGating.cs:63` · `profile` — `profile.SetFlag(flag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:21` · `profile` — `profile.SetFlag(Prefix + targetKey + "=" + cosmeticId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.ShipLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:21` · `profile` — `if (!string.IsNullOrEmpty(id)) profile.SetFlag(key + id);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.TransmissionProgress` · `Ziptide/Assets/Ziptide/Core/Runtime/TransmissionProgress.cs:63` · `profile` — `profile.SetFlag(flag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ReleaseFeel` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ReleaseFeel.cs:130` · `profile` — `profile.SetFlag(ZiptideFlags.FIRST_RELEASE);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:135` · `profile` — `profile.SetFlag(flag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ChoiceStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ChoiceStation.cs:113` · `profile` — `if (profile != null) profile.SetFlag(flag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.CollectibleRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/CollectibleRuntime.cs:121` · `profile` — `profile.SetFlag(_flagOnCollect);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:114` · `profile` — `profile.SetFlag(SaidFlagPrefix + line.id);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:136` · `profile` — `profile.SetFlag(SaidFlagPrefix + line.id);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:154` · `profile` — `if (line.once && profile != null) profile.SetFlag(SaidFlagPrefix + line.id);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SpaceSalvageItemRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/SpaceSalvageItemRuntime.cs:118` · `profile` — `if (profile != null && !string.IsNullOrEmpty(grantsFlag)) profile.SetFlag(grantsFlag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:380` · `profile` — `profile.SetFlag(flag);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:384` · `profile` — `if (flag == ContractDoneFlag) profile.SetFlag(ZiptideFlags.TUTORIAL_COMPLETE);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.HangarBayRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HangarBayRuntime.cs:99` · `profile` — `profile.SetFlag("SHIP_REFIT"); // RILL notices the first refit`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.FirstHourDirectorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourDirectorTests.cs:137` · `profile` — `StringAssert.Contains("profile.SetFlag(ZiptideFlags.TUTORIAL_COMPLETE)", source);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:88` · `profile` — `profile.SetFlag(ZiptideFlags.FIRST_HOLSTER);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:35` · `profile` — `profile.SetFlag("W001_COMPLETE");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:44` · `profile` — `profile.SetFlag("W001_COMPLETE");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:69` · `profile` — `profile.SetFlag("W001_COMPLETE");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:73` · `profile` — `profile.SetFlag("W002_COMPLETE");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:96` · `profile` — `profile.SetFlag("W001_COMPLETE");`

### `SettingsRequested`

- **EVENT_DECLARE** · `Ziptide.Gameplay.HomeHubFlowState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs:117` · `static Action` — `public static event Action SettingsRequested;`

### `SignalCompleted`

- **EVENT_DECLARE** · `Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:38` · `Action<string>` — `public event Action<string> SignalCompleted;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.Tutorial.FirstHourObservationAdapter` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs:193` — `SignalCompleted?.Invoke(_activeSignalId);`

### `StageChanged`

- **EVENT_DECLARE** · `Ziptide.Gameplay.RepairableMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs:31` · `System.Action<RepairStage>` — `public event System.Action<RepairStage> StageChanged;`

### `StateChanged`

- **EVENT_DECLARE** · `Ziptide.Gameplay.SystemFocusStateMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:76` · `System.Action<LifecycleState, LifecycleState>` — `public event System.Action<LifecycleState, LifecycleState> StateChanged;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.SystemFocusStateMachine` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/SystemFocusLifecycle.cs:141` — `StateChanged?.Invoke(previous, next);`

### `StepChanged`

- **EVENT_DECLARE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:13` · `Action` — `public event Action StepChanged;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:47` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:72` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:107` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:132` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:146` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:160` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:187` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:234` — `StepChanged?.Invoke();`
- **EVENT_INVOKE** · `Ziptide.Gameplay.JobRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs:275` — `StepChanged?.Invoke();`

### `TargetDisabled`

- **EVENT_INVOKE** · `Ziptide.Ship.ShipFlightRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs:416` — `Ziptide.Core.FlightSignals.TargetDisabled?.Invoke(hit.name); // announced append (tf-space1): Tidefront space-defense counts these`

### `TargetFleetSize`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Multiplayer.Conquest.ConquestAction` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestAI.cs:71` · `profile` — `if (me.fleetVesselIds.Count < profile.TargetFleetSize)`

### `TelegraphState`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:261` · `profile` — `if (!states.Contains(profile.TelegraphState))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Content.CreatureBehaviorStateEvidence` · `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs:262` · `profile` — `errors.Add("TELEGRAPH_NOT_ACTIVE:" + profile.TelegraphState);`

### `This`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:104` · `profile` — `/// Both halves collected, per the profile. This is a flag read, not a scene search.`

### `TransportChanged`

- **EVENT_DECLARE** · `Ziptide.Multiplayer.PvpNetHub` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs:25` · `static Action<IPvpTransport>` — `public static event Action<IPvpTransport> TransportChanged;`
- **EVENT_INVOKE** · `Ziptide.Multiplayer.PvpNetHub` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs:30` — `TransportChanged?.Invoke(Active);`

### `TravelCompleted`

- **EVENT_DECLARE** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:38` · `static Action<string>` — `public static event Action<string> TravelCompleted;`
- **EVENT_INVOKE** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:330` — `TravelCompleted?.Invoke(destination);`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.FirstHourTravelSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourTravelSignalTests.cs:124` — `StringAssert.DoesNotContain("TravelCompleted?.Invoke(sceneName)",`

### `TravelCoordinator.TravelCompleted`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:158` · `OnTravelCompleted` — `TravelCoordinator.TravelCompleted += OnTravelCompleted;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:176` · `OnTravelCompleted` — `TravelCoordinator.TravelCompleted -= OnTravelCompleted;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:58` · `OnTravelCompleted` — `TravelCoordinator.TravelCompleted += OnTravelCompleted;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs:70` · `OnTravelCompleted` — `TravelCoordinator.TravelCompleted -= OnTravelCompleted;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenTravelVisualCapture` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenTravelVisualCapture.cs:41` · `MarkCompletedDestinationPending` — `TravelCoordinator.TravelCompleted -= MarkCompletedDestinationPending;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryGoldenTravelVisualCapture` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenTravelVisualCapture.cs:42` · `MarkCompletedDestinationPending` — `TravelCoordinator.TravelCompleted += MarkCompletedDestinationPending;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:68` · `OnTravelCompleted` — `TravelCoordinator.TravelCompleted += OnTravelCompleted;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:80` · `OnTravelCompleted` — `TravelCoordinator.TravelCompleted -= OnTravelCompleted;`

### `TryGet`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.SkyVistaRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs:116` · `profile` — `if (profile.TryGet<T>(out var existing)) return existing;`

### `Uses`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:370` · `profile` — `// Pay the job's reward + set its completion flag into the live profile. Uses Architect's`

### `WristScanner.ScanResultPublished`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:160` · `OnScanResult` — `WristScanner.ScanResultPublished += OnScanResult;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:178` · `OnScanResult` — `WristScanner.ScanResultPublished -= OnScanResult;`

### `Ziptide.Core.FlightSignals.TargetDisabled`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:108` · `OnFlightTargetDisabled` — `if (_flightHooked) Ziptide.Core.FlightSignals.TargetDisabled -= OnFlightTargetDisabled;`

### `_bound.OnPose`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpOnlinePresence` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:52` · `OnRemotePose` — `if (_bound != null) _bound.OnPose -= OnRemotePose;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpOnlinePresence` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:58` · `OnRemotePose` — `if (_bound != null) _bound.OnPose -= OnRemotePose;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpOnlinePresence` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpOnlinePresence.cs:60` · `OnRemotePose` — `if (_bound != null) _bound.OnPose += OnRemotePose;`

### `_dir.KillScored`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpModeDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:71` · `OnKill` — `_dir.KillScored -= OnKill;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpModeDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:94` · `OnKill` — `_dir.KillScored += OnKill;`

### `_dir.MatchRestarted`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpModeDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:72` · `OnRematch` — `_dir.MatchRestarted -= OnRematch;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpModeDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpModeDirector.cs:95` · `OnRematch` — `_dir.MatchRestarted += OnRematch;`

### `_director.KillScored`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:76` · `OnKill` — `_director.KillScored += OnKill;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:84` · `OnKill` — `_director.KillScored -= OnKill;`

### `_director.MatchEnded`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:77` · `OnMatchEnded` — `_director.MatchEnded += OnMatchEnded;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:85` · `OnMatchEnded` — `_director.MatchEnded -= OnMatchEnded;`

### `_job.JobCompleted`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:221` · `OnJobCompleted` — `_job.JobCompleted += OnJobCompleted;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:247` · `OnJobCompleted` — `_job.JobCompleted -= OnJobCompleted;`

### `_job.StepChanged`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:220` · `OnJobStepChanged` — `_job.StepChanged += OnJobStepChanged;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:246` · `OnJobStepChanged` — `_job.StepChanged -= OnJobStepChanged;`

### `_mine.stored`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BeltMinePortRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltMinePortRuntime.cs:128` · `_mine` — `_mine.stored += _mine.ratePerSecond * Time.deltaTime;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.MiningRigRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/MiningRigRuntime.cs:114` · `_mine` — `_mine.stored += _mine.ratePerSecond * Time.deltaTime;`

### `_onGet`

- **EVENT_INVOKE** · `Ziptide.Core.PoolCore` · `Ziptide/Assets/Ziptide/Core/Runtime/PoolCore.cs:47` — `_onGet?.Invoke(item);`

### `_onRelease`

- **EVENT_INVOKE** · `Ziptide.Core.PoolCore` · `Ziptide/Assets/Ziptide/Core/Runtime/PoolCore.cs:56` — `_onRelease?.Invoke(item);`

### `_rig.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BeltConductorRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltConductorRuntime.cs:136` · `_handle` — `if (_rig != null) _rig.position += _handle.position - before; // delta — NEVER parent`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ClimbableSurface` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ClimbRuntime.cs:218` · `new` — `_rig.position += new Vector3(delta.X, delta.Y, delta.Z); // delta-translate — NEVER parent`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.LiftRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/LiftRuntime.cs:88` · `delta` — `if (onDeck) _rig.position += delta; // delta-translate — NEVER parent the rig`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ZiplineRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:217` · `delta` — `if (_rig != null) _rig.position += delta; // delta-translate — NEVER parent the rig`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:132` · `delta` — `Assert.AreEqual(1, Count(source, "if (_rig != null) _rig.position += delta;"));`

### `_rig.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.HazardZoneRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:111` · `dir` — `_rig.transform.position += dir * _def.strength * Time.deltaTime;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.HazardZoneRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:134` · `outDir` — `_rig.transform.position += outDir.normalized * 0.8f * Time.deltaTime;`

### `_rigRoot.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:414` · `Vector3` — `_rigRoot.transform.position += Vector3.down * 0.25f;`

### `_runtime.JobCompleted`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:62` · `OnJobCompleted` — `_runtime.JobCompleted += OnJobCompleted;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:70` · `OnJobCompleted` — `_runtime.JobCompleted -= OnJobCompleted;`

### `_runtime.StepChanged`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:61` · `OnStepChanged` — `_runtime.StepChanged += OnStepChanged;`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.JobDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs:69` · `OnStepChanged` — `_runtime.StepChanged -= OnStepChanged;`

### `_subscribedRuntime.JobCompleted`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:104` · `OnJobCompleted` — `_subscribedRuntime.JobCompleted -= OnJobCompleted;`

### `_subscribedRuntime.StepChanged`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:103` · `OnStepChanged` — `_subscribedRuntime.StepChanged -= OnStepChanged;`

### `addTile`

- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoveryHomeHubBindingTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryHomeHubBindingTests.cs:77` — `addTile.Invoke(hub, new object[]`

### `aim.Y`

- **EVENT_SUBSCRIBE** · `Ziptide.Multiplayer.Bots.BotPerception` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Bots/BotBrain.cs:237` · `_rng` — `aim.Y += _rng.NextSigned() * maxOff * 0.5f;`

### `autoRunDoubleTapWindow`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.LocomotionDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/LocomotionDirector.cs:100` · `profile` — `float tapWindow = profile != null ? profile.autoRunDoubleTapWindow : 0.35f;`

### `availableThemes`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:82` · `profile` — `if (theme != null && !profile.availableThemes.Contains(theme))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:83` · `profile` — `profile.availableThemes.Add(theme);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:57` · `profile` — `profile.availableThemes = available;`

### `boltCooldown`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:108` · `profile` — `boltCooldown = profile.boltCooldown;`

### `boltSpeed`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:109` · `profile` — `boltSpeed = profile.boltSpeed;`

### `bot.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.WeaponPoseCore` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/MeleeWeaponRuntime.cs:209` · `transform` — `if (bot != null) bot.transform.position += transform.forward * 0.75f;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.SonicThumperRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/SonicThumperRuntime.cs:78` · `shoveDir` — `bot.transform.position += shoveDir * (float)PvpRules.ThumperShoveMeters * 0.5f;`

### `callback`

- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.VfxFactoryTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/VfxFactoryTests.cs:140` — `callback.Invoke(pooled, null);`

### `castOff.DestinationSelected`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.HomeHubFlowTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs:184` · `destination` — `castOff.DestinationSelected += destination => seen = destination;`

### `clip`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ScenePatcherD2` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs:197` · `profile` — `profile.clip = clip;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:80` · `profile` — `if (profile.clip == null)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:87` · `profile` — `if (profile.clip == _currentClip && _active != null && _active.isPlaying)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:90` · `profile` — `_currentClip = profile.clip;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:97` · `profile` — `next.clip = profile.clip;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.AudioDirectorLifecycleTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs:72` · `profile` — `int assignNext = source.IndexOf("next.clip = profile.clip;", clearNext);`

### `crossfadeSeconds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ScenePatcherD2` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs:200` · `profile` — `profile.crossfadeSeconds = 2f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:108` · `profile` — `Mathf.Max(0f, profile.crossfadeSeconds)));`

### `crouchSpeedFactor`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.LocomotionDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/LocomotionDirector.cs:99` · `profile` — `float crouch = profile != null ? profile.crouchSpeedFactor : 0.55f;`

### `defaultTheme`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:81` · `profile` — `profile.defaultTheme = theme;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:47` · `profile` — `profile.defaultTheme = defaultTheme;`

### `detectRange`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:100` · `profile` — `detectRange = profile.detectRange;`

### `effect`

- **EVENT_INVOKE** · `Ziptide.Gameplay.HazardZoneRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/HazardZoneRuntime.cs:146` — `effect?.Invoke();`

### `enabled`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ScenePatcherD2` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs:201` · `profile` — `profile.enabled = true;`

### `fallYThreshold`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:79` · `profile` — `profile.fallYThreshold = groundY - 3f; // below any canal/hazard depth`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:45` · `profile` — `profile.fallYThreshold = -2f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FallRespawner` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/FallRespawner.cs:43` · `profile` — `profile.fallYThreshold);`

### `flags`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:27` · `profile` — `if (profile == null || profile.flags == null || string.IsNullOrEmpty(targetKey)) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:29` · `profile` — `for (int i = profile.flags.Count - 1; i >= 0; i--)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:30` · `profile` — `if (profile.flags[i] != null && profile.flags[i].StartsWith(keyPrefix))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:31` · `profile` — `profile.flags.RemoveAt(i);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:37` · `profile` — `if (profile == null || profile.flags == null || string.IsNullOrEmpty(targetKey)) return null;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:39` · `profile` — `for (int i = 0; i < profile.flags.Count; i++)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:40` · `profile` — `if (profile.flags[i] != null && profile.flags[i].StartsWith(keyPrefix))`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.CosmeticLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/CosmeticLocker.cs:41` · `profile` — `return profile.flags[i].Substring(keyPrefix.Length);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.ShipLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:20` · `profile` — `profile.flags.RemoveAll(f => f != null && f.StartsWith(key));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.ShipLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:26` · `profile` — `if (profile == null || profile.flags == null || string.IsNullOrEmpty(slot)) return null;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.ShipLocker` · `Ziptide/Assets/Ziptide/Core/Runtime/ShipLocker.cs:28` · `profile` — `foreach (var f in profile.flags)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:87` · `Profile` — `" resources=" + Profile.resources.Count + " flags=" + Profile.flags.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:176` · `profile` — `if (profile == null || profile.flags == null) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:183` · `profile` — `for (int i = 0; i < profile.flags.Count; i++) _knownFlags.Add(profile.flags[i]);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:187` · `profile` — `for (int i = 0; i < profile.flags.Count; i++)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.RillCompanion` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RillCompanion.cs:189` · `profile` — `string flag = profile.flags[i];`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:129` · `profile` — `if (profile == null || profile.flags == null) return done;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:131` · `profile` — `for (int i = 0; i < profile.flags.Count; i++)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FirstHourDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourDirector.cs:133` · `profile` — `string flag = profile.flags[i];`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:78` · `profile` — `profile.flags.RemoveAll(f => f.StartsWith(ConquestSave.FlagPrefix, System.StringComparison.Ordinal));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:79` · `profile` — `profile.flags.Add(ConquestSave.FlagPrefix + ConquestSave.Serialize(_state, _hotseat, _activeSide));`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ConquestTableRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestTableRuntime.cs:86` · `profile` — `foreach (var f in profile.flags)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:149` · `profile` — `if (profile == null || profile.flags == null) return;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.ShipRefit` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipRefit.cs:150` · `profile` — `var earned = ShipJourneyDecals.FromFlags(profile.flags);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.FirstHourHolsterAdapterTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourHolsterAdapterTests.cs:101` · `profile` — `Assert.AreEqual(1, profile.flags.FindAll(f => f == ZiptideFlags.FIRST_HOLSTER).Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:54` · `profile` — `Assert.AreEqual(0, profile.flags.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:101` · `profile` — `Assert.AreEqual(2, profile.flags.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.WorldGatingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/WorldGatingTests.cs:109` · `profile` — `Assert.AreEqual(0, profile.flags.Count);`

### `groundTint`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:27` · `profile` — `profile.groundTint = new Color(0.42f, 0.5f, 0.45f, 1f);`

### `groundY`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:77` · `profile` — `profile.groundY = groundY;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:43` · `profile` — `profile.groundY = 0f;`

### `item.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ArtifactJoinRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ArtifactJoinRuntime.cs:142` · `delta` — `item.transform.position += delta;`

### `json`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:21` · `profile` — `private const string FileName = "profile.json";`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CrashProofingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CrashProofingTests.cs:41` · `profile` — `string path = P("profile.json");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.CrashProofingTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/CrashProofingTests.cs:52` · `profile` — `string path = P("profile.json");`

### `kind`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ReactivePropAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ReactivePropAuthor.cs:57` · `profile` — `if (profile.kind == ReactionKind.LightFlickerOut &&`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ReactivePropAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ReactivePropAuthor.cs:66` · `profile` — `reactive.Configure(profile.kind, proxy);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ReactivePropAuthorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs:44` · `profile` — `Assert.AreEqual(row.Value, profile.kind, row.Key);`

### `lastSavedAtUnix`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:107` · `Profile` — `Profile.lastSavedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();`

### `ledger`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:45` · `profile` — `if (profile.ledger == null) profile.ledger = new List<LedgerEntry>();`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:46` · `profile` — `profile.ledger.Add(entry);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:47` · `profile` — `while (profile.ledger.Count > MaxEntries) profile.ledger.RemoveAt(0);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:55` · `profile` — `foreach (var e in profile.ledger)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:66` · `profile` — `for (int i = profile.ledger.Count - 1; i >= 0 && shown < maxLines; i--)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.LedgerEntry` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ResourceLedger.cs:68` · `profile` — `var e = profile.ledger[i];`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:112` · `profile` — `foreach (var e in profile.ledger) sourcesSeen.Add(e.source);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:122` · `profile` — `foreach (var e in profile.ledger)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:130` · `profile` — `Assert.AreEqual(profile.ledger.Count, loaded.ledger.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:175` · `profile` — `Assert.AreEqual(0, profile.ledger.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.GoldenMetaLoopTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/GoldenMetaLoopTests.cs:178` · `profile` — `Assert.AreEqual(ResourceLedger.MaxEntries, profile.ledger.Count, "ring cap holds");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.SalvageCacheTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/SalvageCacheTests.cs:22` · `profile` — `Assert.Greater(profile.ledger.Count, 0, "every grant flows through the ledger");`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.SalvageCacheTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/SalvageCacheTests.cs:23` · `profile` — `var last = profile.ledger[profile.ledger.Count - 1];`

### `line.RideEnded`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:51` · `(reason, progress) =>` — `line.RideEnded += (reason, progress) =>`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:93` · `(reason, progress) =>` — `line.RideEnded += (reason, progress) => throw new InvalidOperationException("end expected");`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:94` · `(reason, progress) =>` — `line.RideEnded += (reason, progress) => laterEnds++;`

### `line.RideStarted`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:32` · `() =>` — `line.RideStarted += () => starts++;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:91` · `() =>` — `line.RideStarted += () => throw new InvalidOperationException("start expected");`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:92` · `() =>` — `line.RideStarted += () => laterStarts++;`

### `loop`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ScenePatcherD2` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs:199` · `profile` — `profile.loop = true;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:98` · `profile` — `next.loop = profile.loop;`

### `loseRange`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:101` · `profile` — `loseRange = profile.loseRange;`

### `method`

- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.AudioDirectorLifecycleTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs:37` — `Assert.DoesNotThrow(() => method.Invoke(null, new object[] { source }));`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.AudioDirectorLifecycleTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/AudioDirectorLifecycleTests.cs:50` — `Assert.DoesNotThrow(() => method.Invoke(null, new object[] { null }));`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.HeadsetBuildBlockerRegressionTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/HeadsetBuildBlockerRegressionTests.cs:173` — `() => method.Invoke(null, new object[] { "RECOVERY_CANARY", throwingHook }));`
- **EVENT_INVOKE** · `Ziptide.Tests.EditMode.ZiplineSignalTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs:184` — `method.Invoke(line, args);`
- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoveryCoreBootstrapGateTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryCoreBootstrapGateTests.cs:123` — `method.Invoke(null, null);`
- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoveryGameplayBootstrapGateTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGameplayBootstrapGateTests.cs:112` — `method.Invoke(null, null);`
- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoverySceneTestIsolation` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoverySceneTestIsolation.cs:141` — `method.Invoke(null, null);`

### `name`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:82` · `profile` — `Debug.LogWarning("ZIPTIDE: AUDIO_CLIP_MISSING on profile " + profile.name);`

### `onSelect`

- **EVENT_INVOKE** · `Ziptide.Gameplay.QuartersRoom` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersRoom.cs:262` — `interactable.selectEntered.AddListener(_ => onSelect?.Invoke());`
- **EVENT_INVOKE** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:334` — `interactable.selectEntered.AddListener(_ => onSelect?.Invoke());`

### `onSubscriberFailure`

- **EVENT_INVOKE** · `Ziptide.Gameplay.WristScanTarget` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/WristScanResult.cs:125` — `onSubscriberFailure?.Invoke(ex);`
- **EVENT_INVOKE** · `Ziptide.Gameplay.RepairStageSignals` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs:75` — `onSubscriberFailure?.Invoke(ex);`

### `orbitSpeed`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:103` · `profile` — `orbitSpeed = profile.orbitSpeed;`

### `p.x`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.CityStreetLifeRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/CityStreetLifeRuntime.cs:53` · `Mathf` — `p.x += Mathf.Sin(Time.time * 0.55f + i) * 0.08f;`

### `p.y`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.TurnModeCore` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerSafetyRuntime.cs:258` · `RecoveryEyeHeight` — `p.y += RecoveryEyeHeight - eyeHeight;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.CityStreetLifeRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/CityStreetLifeRuntime.cs:52` · `phase` — `p.y += phase * 1.15f;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ToxicRiverSurfaceRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ToxicRiverSurfaceRuntime.cs:78` · `Mathf` — `p.y += Mathf.Sin(Time.time * 1.8f + i * 0.9f) * waveAmplitude`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ToxicRiverSurfaceRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ToxicRiverSurfaceRuntime.cs:88` · `Mathf` — `p.y += Mathf.Sin(Time.time * (1.2f + i * 0.07f) + i)`
- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.ZiplineRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs:83` · `Mathf` — `p.y -= Mathf.Max(0f, sagMeters) * 4f * t * (1f - t);`
- **EVENT_SUBSCRIBE** · `Ziptide.Ship.VehicleSafetyRuntime` · `Ziptide/Assets/Ziptide/Ship/Runtime/VehicleSafetyRuntime.cs:121` · `MountedRecoveryEyeHeight` — `p.y += MountedRecoveryEyeHeight - eyeHeight;`

### `patrolRadius`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:105` · `profile` — `patrolRadius = profile.patrolRadius;`

### `patrolSpeed`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:106` · `profile` — `patrolSpeed = profile.patrolSpeed;`

### `photos`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.PhotoCaptureCamera` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/PhotoCaptureCamera.cs:99` · `Profile` — `CapturedPhoto evicted = PhotoAlbum.Add(save.Profile.photos, captured);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.QuartersPhotoWall` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/QuartersPhotoWall.cs:26` · `profile` — `List<CapturedPhoto> photos = profile != null ? profile.photos : null;`

### `planet`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:35` · `profile` — `profile.planet.baseColor = new Color(0.4f, 0.5f, 0.7f, 1f);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:36` · `profile` — `profile.planet.accentColor = new Color(0.25f, 0.35f, 0.5f, 1f);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:37` · `profile` — `profile.planet.angularSizeDegrees = 15f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:38` · `profile` — `profile.planet.distance = 50f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:39` · `profile` — `profile.planet.direction = new Vector3(0f, 0.5f, 0.866f).normalized;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:40` · `profile` — `profile.planet.rotationSpeed = 5f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:41` · `profile` — `profile.planet.followPlayer = true;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.SkyPlanetRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:177` · `profile` — `VisualThemeProfile.PlanetSettings p = profile.planet;`

### `planet.defenseLevel`

- **EVENT_SUBSCRIBE** · `Ziptide.Multiplayer.Conquest.ConquestPlayer` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestState.cs:103` · `d` — `planet.defenseLevel += d.DefenseBonus;`

### `plate.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:297` · `q` — `plate.transform.position += q * new Vector3(0f, 0f, cellSize * 0.36f);`

### `playAreaSize`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:42` · `profile` — `profile.playAreaSize = new Vector2(4f, 4f);`

### `playerId`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Core.ProfileSerializer` · `Ziptide/Assets/Ziptide/Core/Runtime/Persistence/ProfileSerializer.cs:38` · `profile` — `if (string.IsNullOrEmpty(profile.playerId)) { profile = null; return false; }`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:86` · `Profile` — `Debug.Log("ZIPTIDE: SAVE_LOAD playerId=" + Profile.playerId +`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:98` · `Profile` — `Debug.Log("ZIPTIDE: SAVE_NEW_PROFILE playerId=" + Profile.playerId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:372` · `profile` — `Assert.AreEqual(expectedPlayerId, profile.playerId);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryTravelSaveRoundTripTests.cs:385` · `profile` — `Assert.AreEqual(expectedPlayerId, profile.playerId);`

### `plot.plantedAtUnix`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Content.HarvestPlantResult` · `Ziptide/Assets/Ziptide/Content/Runtime/Economy/GardenService.cs:131` · `credit` — `plot.plantedAtUnix -= credit;`

### `plot.yieldMultiplier`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.HarvestPlantResult` · `Ziptide/Assets/Ziptide/Content/Runtime/Economy/GardenService.cs:127` · `TendYieldBonusPerPower` — `plot.yieldMultiplier += TendYieldBonusPerPower * power;`

### `position.y`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.QuestDeviceCorrectionsRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs:209` · `eyeHeight` — `position.y -= eyeHeight - CorrectedEyeHeight;`
- **EVENT_SUBSCRIBE** · `Ziptide.Visuals.SkyAtmosphereRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyAtmosphereRig.cs:235` · `eyeY` — `position.y += eyeY;`

### `proxyCenter`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ReactivePropAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ReactivePropAuthor.cs:151` · `profile` — `proxyObject.transform.localPosition = profile.proxyCenter;`

### `proxySize`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ReactivePropAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ReactivePropAuthor.cs:159` · `profile` — `proxy.size = profile.proxySize;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ReactivePropAuthorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs:45` · `profile` — `Assert.Greater(profile.proxySize.x, 0f, row.Key);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ReactivePropAuthorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs:46` · `profile` — `Assert.Greater(profile.proxySize.y, 0f, row.Key);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.ReactivePropAuthorTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/ReactivePropAuthorTests.cs:47` · `profile` — `Assert.Greater(profile.proxySize.z, 0f, row.Key);`

### `publish`

- **EVENT_INVOKE** · `Ziptide.Gameplay.FirstHourHolsterSignal` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/FirstHourHolsterSignal.cs:29` — `publish?.Invoke(itemId);`
- **EVENT_INVOKE** · `Ziptide.Gameplay.RepairStageSignals` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs:50` — `publish?.Invoke(designatedMachine);`
- **EVENT_INVOKE** · `Ziptide.Gameplay.TravelCoordinator` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs:306` — `publish?.Invoke(destination);`

### `pvpCareer`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.PvpProgressionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs:102` · `profile` — `var career = profile.pvpCareer;`

### `rail.transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BeltCellSpec` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Automation/BeltFloorRuntime.cs:274` · `rail` — `rail.transform.position += rail.transform.right * (cellSize * 0.44f * side);`

### `readiness`

- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoveryInputSessionGuardTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:110` — `Assert.IsTrue((bool)readiness.Invoke(null, new object[] { referencedProperty }),`
- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoveryInputSessionGuardTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:125` — `Assert.IsFalse((bool)readiness.Invoke(null, new object[] { directProperty }),`
- **EVENT_INVOKE** · `Ziptide.Tests.PlayMode.RecoveryInputSessionGuardTests` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs:129` — `Assert.IsTrue((bool)readiness.Invoke(null, new object[] { directProperty }),`

### `report.materialSlotCount`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.PlayMode.RecoveryFallbackSurfaceReport` · `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryFallbackSurfaceAudit.cs:96` · `materials` — `report.materialSlotCount += materials.Length;`

### `resources`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.SaveSystem` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs:87` · `Profile` — `" resources=" + Profile.resources.Count + " flags=" + Profile.flags.Count);`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Tests.EditMode.JobRewardsTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/JobRewardsTests.cs:72` · `profile` — `Assert.AreEqual(0, profile.resources.Count, "blank id and zero amount are ignored");`

### `respawnFadeSeconds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:46` · `profile` — `profile.respawnFadeSeconds = 0f;`

### `respawnOnFall`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:78` · `profile` — `profile.respawnOnFall = true;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:44` · `profile` — `profile.respawnOnFall = true;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.FallRespawner` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/FallRespawner.cs:33` · `profile` — `if (!profile.respawnOnFall) return;`

### `result.totalProduced`

- **EVENT_SUBSCRIBE** · `Ziptide.Core.WorldResolveResult` · `Ziptide/Assets/Ziptide/Core/Runtime/Economy/ProfileEconomy.cs:37` · `acc` — `result.totalProduced += acc.added;`

### `runtime.JobCompleted`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:94` · `OnJobCompleted` — `runtime.JobCompleted += OnJobCompleted;`
- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.FirstRouteFeelTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/FirstRouteFeelTests.cs:61` · `OnJobCompleted` — `StringAssert.Contains("runtime.JobCompleted += OnJobCompleted", source);`

### `runtime.StepChanged`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ObjectiveBoard` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/ObjectiveBoard.cs:93` · `OnStepChanged` — `runtime.StepChanged += OnStepChanged;`

### `s.Armor`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.Ship.ShipStats` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:149` · `m` — `s.Armor += m.DArmor;`

### `s.Boost`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.Ship.ShipStats` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:147` · `m` — `s.Boost += m.DBoost;`

### `s.Cargo`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.Ship.ShipStats` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:148` · `m` — `s.Cargo += m.DCargo;`

### `s.Handling`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.Ship.ShipStats` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:146` · `m` — `s.Handling += m.DHandling;`

### `s.Speed`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.Ship.ShipStats` · `Ziptide/Assets/Ziptide/Content/Runtime/Ship/ShipLoadoutCore.cs:145` · `m` — `s.Speed += m.DSpeed;`

### `s.fill01`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Core.CanState` · `Ziptide/Assets/Ziptide/Core/Runtime/PourCore.cs:41` · `poured` — `s.fill01 -= poured;`

### `s.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.FlightState` · `Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs:112` · `Forward` — `s.position += Forward(s) * (s.speed * dt);`
- **EVENT_SUBSCRIBE** · `Ziptide.Content.FlightState` · `Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs:119` · `right` — `s.position += right * (strafe * p.maxSpeed * Mathf.Clamp01(p.strafeFraction) * dt);`

### `s.rollDeg`

- **EVENT_SUBSCRIBE** · `Ziptide.Content.FlightState` · `Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs:104` · `s` — `s.rollDeg += s.rollDirection * rollRate * dt;`

### `skyGradient`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultVisualTheme` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultVisualTheme.cs:33` · `profile` — `profile.skyGradient = sky;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.SkyPlanetRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:148` · `profile` — `Gradient grad = profile.skyGradient;`

### `skyVista`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.SkyPlanetRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:37` · `profile` — `if (profile.skyVista != null)`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Visuals.SkyPlanetRig` · `Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs:43` · `profile` — `_vistaRig.ApplyVista(profile.skyVista, playerTransform);`

### `slowFactor`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:111` · `profile` — `slowFactor = profile.slowFactor;`

### `spawnEuler`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:41` · `profile` — `profile.spawnEuler = Vector3.zero;`

### `spawnPosition`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.ApplyWorldProfileToCurrentScene` · `Ziptide/Assets/Ziptide/Editor/Setup/ApplyWorldProfileToCurrentScene.cs:54` · `profile` — `go.transform.position = profile.spawnPosition + Vector3.forward * 1.5f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Setup.CreateDefaultWorldProfile` · `Ziptide/Assets/Ziptide/Editor/Setup/CreateDefaultWorldProfile.cs:40` · `profile` — `profile.spawnPosition = Vector3.zero;`

### `st.position`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Gameplay.ShipBoardingStation` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ShipBoardingStation.cs:260` · `transform` — `st.position -= transform.forward * speed * Time.deltaTime;`

### `standoffDistance`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:102` · `profile` — `standoffDistance = profile.standoffDistance;`

### `stunSeconds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:110` · `profile` — `stunSeconds = profile.stunSeconds;`

### `t.OnFire`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:27` · `m` — `t.OnFire += m => { got = m; calls++; };`

### `t.OnHit`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:41` · `m` — `t.OnHit += m => got = m;`

### `t.OnPose`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:90` · `m` — `t.OnPose += m => got = m;`

### `t.OnScore`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:56` · `m` — `t.OnScore += m => got = m;`

### `t.OnWall`

- **EVENT_SUBSCRIBE** · `Ziptide.Tests.EditMode.PvpNetTests` · `Ziptide/Assets/Ziptide/Tests/EditMode/PvpNetTests.cs:70` · `m` — `t.OnWall += m => got = m;`

### `target.stationedDefenseUnits`

- **EVENT_UNSUBSCRIBE** · `Ziptide.Multiplayer.Conquest.AttackOrder` · `Ziptide/Assets/Ziptide/Multiplayer/Runtime/Conquest/ConquestResolver.cs:129` · `report` — `target.stationedDefenseUnits -= report.defenderUnitsLost;`

### `telegraphSeconds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:107` · `profile` — `telegraphSeconds = profile.telegraphSeconds;`

### `transform.localPosition`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.ConquestMissionRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs:498` · `Vector3` — `transform.localPosition += Vector3.down * 0.02f; // each slap seats it deeper`

### `transform.position`

- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BruiserBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/BruiserBehavior.cs:61` · `new` — `transform.position += new Vector3(Mathf.Sin(Time.time * 30f) * 0.008f, 0f, 0f);`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.CreatureRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:120` · `flat` — `case PvpWeapon.TidePike: transform.position += flat * 0.75f; break; // committed thrust`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.CreatureRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:121` · `flat` — `case PvpWeapon.BreakerBlade: transform.position += flat * 0.3f; break; // light stagger`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.CreatureRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs:122` · `flat` — `default: transform.position += flat * 0.5f; break; // gravity/net/etc kick`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.StunBolt` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/StunBolt.cs:65` · `step` — `transform.position += step;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.BootHoldState` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs:1032` · `headDelta` — `transform.position += headDelta;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.QuestDeviceCorrectionsRuntime` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/QuestDeviceCorrectionsRuntime.cs:332` · `handAttach` — `transform.position += handAttach.position - grip.position;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpBolt` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBolt.cs:67` · `step` — `transform.position += step;`
- **EVENT_SUBSCRIBE** · `Ziptide.Gameplay.PvpBot` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpBot.cs:346` · `k` — `transform.position += k.normalized * 0.6f; // small knockback`

### `usePlayAreaBounds`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ThemeAuthor` · `Ziptide/Assets/Ziptide/Editor/Patching/ThemeAuthor.cs:80` · `profile` — `profile.usePlayAreaBounds = false; // open spaces — the global fall net handles edges`

### `verticalBob`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.DroneCombatBehavior` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/DroneCombatBehavior.cs:104` · `profile` — `verticalBob = profile.verticalBob;`

### `volume`

- **PROFILE_FIELD_ACCESS** · `Ziptide.Editor.Patching.ScenePatcherD2` · `Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs:198` · `profile` — `profile.volume = 0.35f;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:103` · `profile` — `_authoredVolume = profile.volume;`
- **PROFILE_FIELD_ACCESS** · `Ziptide.Gameplay.AudioDirector` · `Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AudioDirector.cs:107` · `profile` — `AudioMixSettings.Effective(Ziptide.Core.AudioBus.Music, profile.volume),`

