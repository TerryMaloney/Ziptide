# D4 — Boot + World Architecture Contract

**Status:** Architecture contract. Code largely already conforms (see Survey). This locks the
rules so art/audio/world growth doesn't reintroduce the old instability.

Last updated: 2026-06-14

---

## 1. Read-only survey (current reality)

**Branch:** active work is on `terry-local-wip` (contains the real, current project; `main` is behind).

**Build scenes (`EditorBuildSettings`, in order):**
1. `_Boot.unity` — index 0, persistent boot scene ✅
2. `MilestoneA_GrabCube.unity` — test room
3. `D0_City.unity` — Toxic Venice blockout

**Runtime ownership scan:**

| System | `_Boot` | `MilestoneA` | `D0_City` | Verdict |
|---|---|---|---|---|
| XR Origin / rig | ✅ owns | ❌ | ❌ | correct |
| XRInteractionManager | ✅ (via rig) | ❌ | ❌ | correct |
| InputActionManager | ✅ (via rig) | ❌ | ❌ | correct |
| TravelCoordinator | ✅ | ❌ | ❌ | correct |
| PlayerRigPersistence | ✅ | ❌ | ❌ | correct |
| AudioDirector | ✅ | ❌ | ❌ | correct |
| WorldRuntime | n/a | ✅ | ✅ | correct (content) |
| SpawnMarkerRuntime | n/a | (check) | ✅ (`__SPAWN_PLAYER`) | content |
| WorldTravelStation / ProximityTravelTrigger | n/a | (door) | ✅ both | content |

**Conclusion:** Boot-owned runtime is **real**, and world scenes are **already content-only**.
The architecture GPT's packet asks for is ~80% in place. The remaining risks are: build uses
`BuildAndroid.APK` (no patch/audit), travel uses **Single** scene load rather than additive,
and the theme/visual layer needs the reset discipline documented in §4.

---

## 2. The contract (mechanism-agnostic)

These invariants hold regardless of whether worlds load Single or Additive.

**`_Boot` OWNS (DontDestroyOnLoad, created once, never unloaded):**
- XR Origin / player rig, `XRInteractionManager`, `InputActionManager`
- `TravelCoordinator`, `PlayerRigPersistence`, `InventoryPersistence` / `ItemFactory`
- `AudioDirector` (+ music/ambience), Save/progression, persistent diagnostics

**World scenes MAY contain (content only):**
- geometry, props, enemies, job objects, interactables, art-kit placements
- `SpawnMarkerRuntime`, `WorldTravelStation` / `ProximityTravelTrigger`
- `WorldRuntime` (+ `WorldDirector` it spawns), ambience zones, local world metadata

**World scenes MUST NOT contain:**
- XR Origin, `XRInteractionManager`, `InputActionManager`
- `TravelCoordinator`, global `AudioDirector`, Save/Narrative singletons
- persistent `InventoryPersistence`, persistent diagnostics

---

## 3. Loading model — decision

GPT's packet recommends **additive** world loading under a persistent `_Boot`. ZIPTIDE
currently uses **Single** scene load (`SceneManager.LoadScene`) where the boot-owned systems
survive via `DontDestroyOnLoad`.

**Decision: keep Single-load for now.** It already satisfies every invariant in §2, and the
project just regained stability after a hard input-wiring bug. An additive refactor is a
non-trivial change (manual scene activation, light/skybox/audio ownership per loaded scene,
two scenes alive at once on a Quest memory budget) with no functional gain over the working
Single-load model. Revisit additive only if/when we need seamless streaming between worlds.

The travel **sequence** below is what matters and applies to either model.

---

## 4. Travel sequence (TravelCoordinator — the only travel entry point)

1. `_Boot` already loaded; boot systems alive.
2. `TravelCoordinator.TravelTo(sceneName)` — the **only** travel API (no direct
   `SceneManager.LoadScene` in gameplay; `WorldTravelStation` and `ProximityTravelTrigger`
   both route here ✅).
3. Save inventory/held/holstered state (`PrepareForSceneTravel`).
4. Load destination (Single).
5. Resolve destination `SpawnMarkerRuntime`; move XR Origin so the camera lands at spawn.
6. **Reset world-global visual state** (see §5) so the previous world's look doesn't bleed in.
7. Rebind interactors to the persistent `XRInteractionManager`; re-assert input actions
   (`EnsureXRIWiring` / `EnsurePersistentInputActions`).
8. Wait for XRI ready (rays active, manager present). Fail loudly: `ZIPTIDE: XRI_NOT_READY`.
9. Restore inventory only after XRI passes.
10. Log `ZIPTIDE: TRAVEL_OK` / `TRAVEL_FAIL reason=…`.

---

## 5. Visual-state reset on travel (open issue → contract)

**Symptom under investigation:** returning `D0_City → MilestoneA` shows a "brown screen."
There is **no `RenderSettings` fog code**, so the cause is most likely the `SkyPlanetRig`
sky/planet objects or a failed return-travel — not global fog.

**Contract going forward:** world dressing is owned by per-scene `WorldRuntime` →
`WorldDirector` → `SkyPlanetRig`. On every scene load the active world must **fully define**
its sky/planet/ground/ambient so nothing carries over. Any sky/planet object parented to the
**persistent rig** (so it follows the player) MUST be torn down and rebuilt by the incoming
world's `WorldDirector`, or the previous world's sky persists. This is the next bug to fix and
needs a return-trip device log to confirm which path is leaking.

---

## 6. Audit rules (enforced by WorldAuditRunner; some proposed)

Existing/affirmed blockers: `SPAWN_MISSING`, `SPAWN_NO_FLOOR`, `SPAWN_OVERLAP_SOLID`,
`SPAWN_TRAPPED`, `TRAVEL_NO_DOOR`, `TRAVEL_DEST_NOT_IN_BUILD`, `CITY_NO_ROOT`.

Add (architecture enforcement):
- `WORLD_CONTRACT_FAIL` — a world scene contains a boot-owned system (§2 forbidden list).
- `BOOT_SCENE_MISSING` — first build scene is not `_Boot`.
- `WORLD_NO_PACK` — world scene has no `WorldPackDefinition` via `WorldRuntime`.

**Build must run the audit.** Switch `dev_build_install.ps1` from `BuildAndroid.APK` to
`BuildAndroid.PatchScenesThenAPK` so every build patches + audits (blockers abort the APK).

---

## 7. Diagnostic tags (keep using)

`TRAVEL_START`, `TRAVEL_OK`, `TRAVEL_FAIL`, `XRI_READY`, `XRI_NOT_READY`, `DUP_SINGLETON`,
`PROXIMITY_TRAVEL`, `AUDIT_OK`, `AUDIT_FAIL`, `INPUT_ACTIONS_MISSING`, `NO_RAY_INTERACTORS`.
Add for art/audio: `ART_AUDIT_FAIL`, `AUDIO_AUDIT_FAIL`, `MISSING_ART_KIT`,
`MISSING_AUDIO_THEME`, `PERF_BUDGET_WARN`, `PERF_BUDGET_FAIL`.
