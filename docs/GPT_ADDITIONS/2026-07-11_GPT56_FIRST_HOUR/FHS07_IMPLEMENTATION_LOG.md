# FH-S07 IMPLEMENTATION LOG — HOME / W000 FIRST-HOUR SURFACES

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11 (“crush that out”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE — SHARED/PROTECTED FILE CLAIM POSTED  
**Envelope:** `docs/first_hour/envelopes/FH-S07-HOME-W000-SURFACES.json`  
**Commit budget:** 3 implementation commits, each must be CI-green before the next

## Dependencies

- ✅ `FH-X01-CONTRACT-ASSET`
- ✅ `FH-X02-PROGRESSION-CORE`
- ✅ `FH-S01-OBSERVATION`
- ✅ `FH-S02-HOLSTER`

## Exact protected/shared claims

The following existing owners are protected. Before each commit, reread the live SHA and only modify the named methods/API seams:

1. `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/BootLoader.cs`
   - `Start()` only: replace unconditional immediate travel with one Home Hub choice gate.
   - Preserve `TravelCoordinator.TravelTo(target, skipGate: true)` as the sole scene-change path.
2. `Ziptide/Assets/Ziptide/Gameplay/Runtime/Persistence/SaveSystem.cs`
   - additive public `HasExistingProfile` query;
   - additive explicit `StartNewProfile()` command;
   - existing `Load()` / `Save()` / atomic writer remain the sole persistence path.
3. `Ziptide/Assets/Ziptide/Gameplay/Runtime/Locomotion/LocomotionDirector.cs`
   - additive comfort-application method only;
   - `ApplyProfile(LocomotionProfile)` remains the sole XR provider configuration owner.
4. `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs`
   - additive first-destination selection API only;
   - `TryLaunch()` and `LaunchSequence()` remain the sole launch and travel owners.

No other protected/shared file is claimed.

## New files claimed

- `Ziptide/Assets/Ziptide/Core/Runtime/ComfortSettings.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/ComfortConsoleRuntime.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstDestinationHelmRuntime.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourSurfaceAuthor.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/ComfortSettingsTests.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/HomeHubFlowTests.cs` + `.meta`
- this log

## Concurrent-lane boundary

Picasso/Opus 4.8 remains active on `Visuals/**`, Forge, water, art authors, materials, meshes, shaders, `SPRINT_ART.md`, and art-plan files. FH-S07 touches none of those. The new surface author owns only stable `__FIRST_HOUR_*` markers and runtime components; it does not author art assets or change world dressing.

## Locked behavior

- Cold boot presents New Game / Continue / Settings before W000 travel.
- Continue is absent/disabled when there is no valid profile.
- New Game is explicit; it never silently deletes or resets a save.
- Device comfort settings use `PlayerPrefs`, never `PlayerProfile`, and survive New Game.
- Cozy / Standard / Bold resolve exactly to the locked table in `docs/design/COMFORT_AND_ACCESSIBILITY.md`.
- Standard is the install default.
- Comfort confirmation is the first W000 interaction.
- One named grabbable bunk object publishes the existing semantic signal.
- The first helm exposes only W001/ToxicCity and delegates selection into the existing PUNCH IT / TravelCoordinator path.
- No second menu framework, save singleton, scene loader, locomotion provider, comfort math, launch sequence or destination truth.
- No `.unity` / `.prefab` YAML edits.

## Three-commit sequence

### Commit 1 — boot + profile choice

- `HomeHubRuntime` and pure flow state.
- `SaveSystem.HasExistingProfile` and `StartNewProfile()`.
- `BootLoader.Start()` creates the Home Hub and travels once after explicit New/Continue choice.
- Home Hub logs and neutral completion notifications.
- Tests: Continue hidden without valid save, new profile explicit, travel callback once, settings does not travel.

### Commit 2 — comfort architecture + console

- `ComfortSettings` exact preset table and device persistence.
- Additive `LocomotionDirector` comfort application.
- `ComfortConsoleRuntime` three preset tiles; applies to existing locomotion, `ComfortVignette`, and ziplines.
- Exact-table and PlayerPrefs-survives-new-profile tests.
- Comfort coverage source gate for known motion owners.

### Commit 3 — W000 prop/helm + idempotent author

- `FirstDestinationHelmRuntime` exposes only ToxicCity/W001 and delegates to `ShipCastOffRuntime`.
- Additive destination-selection API on `ShipCastOffRuntime`; launch sequence unchanged.
- Named bunk prop observer.
- `FirstHourSurfaceAuthor` stable markers:
  - `__FIRST_HOUR_COMFORT_CONSOLE`
  - `__FIRST_HOUR_BUNK_OBJECT`
  - `__FIRST_HOUR_FIRST_HELM`
- Idempotence/source tests and runbook bake/device evidence.

## Diagnostics

- `ZIPTIDE: HOME_HUB_READY continue=<bool>`
- `ZIPTIDE: HOME_HUB_CHOICE choice=<new|continue|settings>`
- `ZIPTIDE: COMFORT_PRESET preset=<cozy|standard|bold>`
- `ZIPTIDE: FIRST_HOUR_BUNK_GRAB id=<id>`
- `ZIPTIDE: FIRST_HELM_SELECTED dest=W001_ToxicCity`

## Fallbacks

- Missing art uses primitive readable surfaces.
- Missing comfort owner persists the selected preset and logs; it never moves the rig.
- Missing bunk prop or helm leaves that beat open.
- Existing PUNCH IT and DevWarpBoard remain usable; no fallback directly loads a scene.

## Collision rule

Do not edit claimed files until this log is closed or explicitly released. Live head and protected blob SHA must be reread before every implementation commit.