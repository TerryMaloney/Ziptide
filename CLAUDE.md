# CLAUDE.md — ZIPTIDE project guide for AI sessions

Read this first every session. It is the contract for how to work in this repo without breaking it.

## What this is
A **Meta Quest VR game**. Unity **2022.3.62f3**, **URP**, Android/IL2CPP/ARM64, OpenXR + XR
Interaction Toolkit **2.5.4**. The Unity project lives in the **`Ziptide/`** subfolder.

## Architecture (do not violate)
- **`_Boot` scene** (build index 0, persistent) owns ALL runtime: the XR Origin rig,
  `XRInteractionManager`, `InputActionManager`, `TravelCoordinator`, `PlayerRigPersistence`,
  `AudioDirector`, belt, diagnostics. It is never unloaded and **never a travel destination**.
- **World scenes** (`MilestoneA_GrabCube`, `D0_City`, …) are **content only**: geometry, props,
  `SpawnMarkerRuntime` (`__SPAWN_PLAYER`), `WorldRuntime`, guns/targets/drones, travel doors.
  They must NOT contain the rig or any rig-owned singleton.
- **Assemblies:** `Ziptide.Core` (no deps) ← `Content` ← `Gameplay` ← `Visuals`; `Editor`,
  `Platform.Quest`, `Tests` on top. Core must never reference Visuals.

## State management
- **Data is ScriptableObjects** (`WorldProfile`, `VisualThemeProfile`, `LocomotionProfile`,
  `*Definition` items/jobs, `WorldPackDefinition`). Gameplay resolves content **by string ID
  through factories/registries** (`ItemFactory`), never by hard asset refs to specific prefabs.
- **Runtime singletons** are few and live in `_Boot` as DontDestroyOnLoad: `PlayerRigPersistence`,
  `TravelCoordinator`, `AudioDirector`. Guard duplicates in `Awake` and log `ZIPTIDE: DUP_SINGLETON`.

## Locked contracts
1. `TravelCoordinator.TravelTo(scene)` is the ONLY way to change scenes. No direct
   `SceneManager.LoadScene` in gameplay.
2. `_Boot` is never a travel destination (there's a guard — keep it).
3. Only **holstered** items travel between scenes; loose/in-hand items belong to their scene.
4. One `InputActionManager` (on the rig) owns input; keep its action asset enabled across loads.
5. No `System.Reflection` for runtime item creation — use public `Init()` methods.

## Naming & conventions
- Runtime diagnostics: `Debug.Log("ZIPTIDE: <TAG> key=value")`. Existing tags: `TRAVEL_START/OK/FAIL`,
  `XRI_READY/NOT_READY`, `INPUT_ACTIONS_MISSING`, `NO_RAY_INTERACTORS`, `DUP_SINGLETON`,
  `INVENTORY_SAVE/RESTORE`, `ITEM_DEF_NOT_FOUND`, `DRONE_DOWN`, `LOCO_STATE`, `MOVE_DIAG`, `BELT_ENSURED`.
- Scene/layer/asset path/string literals go in `ZiptideConstants.cs` (Core). No raw scene-name strings.
- C# files LF, 4-space indent, match surrounding style.

## Build / test / verify (PowerShell on the user's PC)
- Snapshot (run first): `tools/ziptide_snapshot.ps1` — branch, commit, build scenes, last ZIPTIDE tags.
- Build + install to Quest: `tools/dev_build_install.ps1` (uses `Ziptide.Build.BuildAndroid.APK`).
- Smoke: `tools/quest_smoke.ps1`.
- CI (`.github/workflows/ci.yml`) compiles + runs EditMode tests on push to `main`, `claude/**`,
  `terry-local-wip`. Treat CI red as a blocker.
- **The cloud agent cannot run Unity or a headset.** All gameplay fixes are verified by the user
  on-device. Diagnose from the `ZIPTIDE:` logcat tags, not guesses.

## Working rules
- One issue-sized change per commit; small and reversible. Branch for features.
- Commit to the branch the user builds from (`terry-local-wip`) so fixes reach the headset.
- **Auto-fix OK:** scripts, docs, audits, idempotent patchers, registry refresh, formatting.
- **Report-only (get confirmation):** XR rig ownership, scene travel, input actions, inventory
  persistence, deleting scene objects, build-settings changes, global material/shader changes.
- **Never** hand-edit `.unity`/`.prefab` YAML blind for anything non-trivial — it has corrupted
  scenes before. Prefer runtime ensures or editor patchers, and have the user verify in Unity.

## Key docs
- `docs/CONNECTIONS_AND_RECOVERY.md` — system map + every root cause + prevention plan (read this).
- `docs/D4_BOOT_ADDITIVE_WORLD_ARCHITECTURE.md` — boot/world contract.
- `docs/WORKLIST.md` — current punch list.
- `docs/project_art_plan/` — art/audio/prompt-to-world pipeline.
- `docs/ZIPTIDE_MASTER_BUILD_PLAN.md` — long-term 80-world vision.
