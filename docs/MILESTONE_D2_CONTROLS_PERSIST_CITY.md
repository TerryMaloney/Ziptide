# Milestone D2: Controls + Persistence + City Traversal Expansion

## Summary

Standardize locomotion controls, add dash/hop, persist player rig across scene travel, and expand the D1 city into an explorable multi-route network.

## What was added

### Controls Standardization
- **LocomotionProfile** (`Content/Runtime/Locomotion/LocomotionProfile.cs`): data-driven ScriptableObject for move speed, turn mode (smooth/snap), dash settings.
- **LocomotionDirector** (`Gameplay/Runtime/Locomotion/LocomotionDirector.cs`): applies profile at runtime; enables smooth OR snap turn provider.
- **EnsureLocomotionRig** updated: now creates BOTH `ActionBasedSnapTurnProvider` and `ActionBasedContinuousTurnProvider`. Migrates legacy "Turn" child to "SnapTurn".
- Default: smooth turn at 90 deg/s. Snap turn available as option (45 deg increments).

### Dash / Hop
- **DashLocomotion** (`Gameplay/Runtime/Locomotion/DashLocomotion.cs`): short forward burst on A button (right controller `primaryButton`).
- Uses `CharacterController.Move()` -- respects colliders, does not fight XRI providers.
- Configurable via profile: distance (3m), duration (0.15s), cooldown (0.5s), vertical lift (0.1m).

### Cross-Scene Persistence
- **PlayerRigPersistence** (`Gameplay/Runtime/Player/PlayerRigPersistence.cs`): singleton on XR Origin root, calls `DontDestroyOnLoad`. On scene load: destroys duplicate XR Origins, teleports to spawn marker, rebinds FallRespawner.
- **SpawnMarkerRuntime** (`Gameplay/Runtime/World/SpawnMarkerRuntime.cs`): placed by ScenePatcherD2 at each scene's spawn position.
- Items holstered under BeltRig persist automatically (children of persisted XR Origin). Grabbed items in hand also persist. Dropped items do not.

### City Expansion
- **CityKitDefinition** extended: `mainWalkwayLength` (60m, up from 40), `crossBridgeCount` (4), `serviceCatwalkHeight`, `rampWidth`.
- **ScenePatcherD1** expanded:
  - Side walkways now full-length (match central).
  - North + South perimeter walkways create a complete **loop route**.
  - 4 cross-bridges per side (evenly distributed).
  - 5 courtyards: Spawn, Dispatch, Garden (central), Service (left side), Outlook (right side).
  - Lower-tier **service catwalks** with railings inside the canals.
  - **Ramps** connecting walkway height to catwalk height.
  - 7 building shells (up from 5).
  - Railings on all edges including catwalks and perimeter.

### Build Pipeline
- **ScenePatcherD2** runs after D1 in `BuildAndroid.PatchScenesThenAPK()`.
- Ensures LocomotionDirector, DefaultLocomotionProfile asset, PlayerRigPersistence, and SpawnMarkerRuntime in every scene.

## Quest Test Checklist (2-3 minutes)

1. Build and install:
   ```
   powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat
   ```

2. **Test room spawn:**
   - Smooth turn works (right stick rotates smoothly, no snap jumps).
   - Left stick moves forward/back/strafe.
   - Dash works: press A button (right controller) for a short forward burst. Verify cooldown (can't spam).
   - Pistol: grab, holster, unholster -- all still working.

3. **Travel to Toxic City:**
   - Walk to travel door ("To Toxic City"), interact to travel.
   - Confirm pistol is still holstered (or in hand) after scene load.

4. **City exploration:**
   - Spawn at south courtyard (CourtyardA).
   - Walk north along central walkway to Dispatch courtyard, then Garden courtyard.
   - Cross a bridge to side walkway.
   - Walk the perimeter (north or south end) to loop back -- confirm full loop without backtracking.
   - Find Service courtyard (left side) and Outlook courtyard (right side).
   - Descend a ramp to service catwalk level (lower tier). Walk along catwalk. Ascend via other ramp.
   - Railings present on all edges (can't accidentally walk off).

5. **Travel back:**
   - Find travel door near spawn courtyard, interact to return to test room.
   - Pistol still present.

6. **If anything fails:**
   ```
   adb logcat -s Unity Ziptide AndroidRuntime
   ```

## File Summary

| File | Change |
|------|--------|
| `Content/Runtime/Locomotion/LocomotionProfile.cs` | NEW: locomotion settings SO |
| `Gameplay/Runtime/Locomotion/LocomotionDirector.cs` | NEW: applies profile at runtime |
| `Gameplay/Runtime/Locomotion/DashLocomotion.cs` | NEW: dash/hop on button press |
| `Gameplay/Runtime/Player/PlayerRigPersistence.cs` | NEW: DontDestroyOnLoad singleton |
| `Gameplay/Runtime/World/SpawnMarkerRuntime.cs` | NEW: spawn position marker |
| `Content/Runtime/City/CityKitDefinition.cs` | UPDATED: new traversal fields |
| `Editor/Patching/ScenePatcherD1.cs` | UPDATED: expanded city layout |
| `Editor/Patching/ScenePatcherD2.cs` | NEW: D2 orchestrator patcher |
| `Editor/Patching/ScenePatcherD0.cs` | UPDATED: repositioned D0 objects |
| `Editor/Setup/EnsureLocomotionRig.cs` | UPDATED: both turn providers + dash |
| `Editor/Build/BuildAndroid.cs` | UPDATED: D2 patcher in pipeline |
| `Gameplay/Ziptide.Gameplay.asmdef` | UPDATED: added Unity.InputSystem ref |
