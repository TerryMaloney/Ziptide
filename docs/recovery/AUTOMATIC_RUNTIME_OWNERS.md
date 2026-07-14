# ZIPTIDE Automatic Runtime Owners

**Status:** R0 evidence layer  
**Machine source:** [`automatic_runtime_owners.json`](automatic_runtime_owners.json)

## Executive finding

A recovery feature is not actually hidden merely because its scene or UI is omitted from the golden-path plan. ZIPTIDE currently contains many automatic bootstraps, persistent services and global scene hooks that can still mutate or inject content into every loaded scene.

The first catalog contains **18 confirmed automatic or persistent owners**. They divide into four groups.

## 1. Preserve as golden-path/support owners

These are required or useful, but must be counted and tested as one composition:

- `PlayerRigPersistence` — persistent rig, XRI manager adoption, fall safety and several ensured player services.
- `TravelCoordinator` — sole intended travel owner.
- `SaveSystem` — sole profile/disk owner.
- `AudioDirector` + `AmbienceDirector` — two persistent audio composers that need one lifecycle and resource census.
- `ComfortVignette` — the intended single comfort iris.
- `RuntimeHealthMonitor` — useful metrics/janitor, but its post-load `UnloadUnusedAssets` cost must be measured.
- `FirstHourObservationAdapter` — neutral only while inactive and unique.
- `EcologyDirector` — allowed only in the selected golden world/creature path.
- `SingletonValidator` — useful concept, but its current three-type warning list is too narrow and will be replaced by the R1 runtime census.

## 2. Diagnostics that must become explicit opt-in

- `DebugHUD` automatically creates a persistent, top-sorting screen-space Canvas and is enabled by default in development builds.
- `VRBootDiagnostics` enumerates and logs every camera and renderer after every scene load.
- `DevWarpBoard` is the intended single development interface, but it remains an automatic persistent input owner.

These are retained, not deleted. The recovery candidate must not expose them automatically unless a named diagnostic profile enables them.

## 3. Global mutators that must be replaced or merged

### `RuntimeMaterialFixer` — priority zero visual collision

After every scene load it scans every renderer. Any null or non-URP material is replaced at runtime with a newly allocated URP/Lit material colored from the object name:

- floor/ground/plane → greenish;
- cube/grabbable → blue;
- everything else → gray.

This means the final headset rendering can differ materially from the authored scene, Forge assignment, build audit and screenshot assumptions. It also creates runtime materials outside the authored art pipeline. The recovery direction is build-time validation plus an explicit development fallback policy—not an unconditional runtime rewrite.

### `RuntimeInputEnabler` — duplicate input-session owner

After every scene load it reflects across all controller components and all MonoBehaviours with `InputActionReference` fields, then enables whole action assets. `PlayerRigPersistence` separately adopts the XR interaction manager and moves/clears input-action ownership. Both currently claim global input-session repair.

The recovery direction is one canonical input session owned with the persistent rig. The broad reflection fallback is retired or explicitly gated only after R1 proves the replacement.

### `EnsureXRCameraActive` — global name-based camera owner

After every scene load it disables every active camera that is not parented under an object whose name includes `XR Origin` or `Camera Offset`. That can collide with field cameras, photo/render cameras, spectator cameras and future snapshot fixtures.

The recovery direction is an explicit camera-role registry and one player-view owner, not name-based global mutation.

## 4. Feature injectors that violate “hidden means unreachable”

These systems globally hook scene loading even when their feature is meant to be parked:

- `ConquestMissionRuntime` can inject a mission board, TextMesh, primitives, interactables, drones/objectives and return travel into any world selected by `ConquestSession`.
- `PvpProgressionRuntime` persists globally, binds arena match directors and writes rewards/profile state.
- `QuartersCameraFeature` globally hooks scene loads and adds a camera dock, handheld camera and photo wall to every `QuartersRoom`.

They remain in source, but the recovery build profile must prevent their hooks from running.

## Confirmed stale Y+B contract

The current `DevWarpBoard` opens through the forehead gesture, F2 or ADB. The surviving Y+B language is in `QuickSwap`:

- B is bound to quick-swap;
- Y is bound as `_menuGuard`;
- when Y is held, B is ignored because the source comment still says `Y+B = dev menu chord`.

`QuickSwap` does not itself open the current menu. This proves the control architecture still encodes a retired chord even after menu ownership changed. The generated input-contract report will locate every remaining chord reference and every cross-owner button collision.

## Recovery consequence

R1 must introduce a **recovery exposure profile** that controls automatic bootstraps and scene hooks, not merely build-scene lists. R2 then replaces global fallback behavior with explicit contracts:

1. input session;
2. camera roles;
3. visual/material validation;
4. diagnostic activation;
5. feature-injector gating;
6. runtime owner census.

No runtime owner was disabled during R0. This document records the required dispositions before implementation begins.
