# ZIPTIDE — Active Worklist

**Purpose:** the single, current "what are we fixing right now" list. Updated every session.
Long-term vision lives in `ZIPTIDE_MASTER_BUILD_PLAN.md`; this file is the short-horizon punch list.

Last updated: 2026-06-14

---

## ✅ Fixed this session

- **Dead controllers / "frozen screenshot" on launch.** The rig's `InputActionManager`
  (with the input action asset) lived on a standalone `_InputActionManager` object that the
  wiring code ignored and that got destroyed on scene travel — disabling all controller input.
  `PlayerRigPersistence.EnsureXRIWiring` now consolidates the input action asset onto the
  persistent rig, neutralizes the doomed managers, and keeps actions enabled across loads.
  *Result: controllers, grab, theme-switch, and first travel all work.*
- **`.gitignore` was eating `Assets/Ziptide/Editor/Build/`** (the `[Bb]uild*/` rule). Build
  pipeline is now tracked and backed up.
- **GameCI** workflow live (compile + EditMode tests on push). Unity license activated.

---

## 🔴 P0 — Hard blockers

- [x] **Brown screen returning to the test room** — FIXED. The return door's destination
  (`TestRoom_WorldPack.sceneName` and a `ProximityTravelTrigger`) pointed at `_Boot`, which
  re-ran bootstrap, spawned duplicate singletons, and dumped the player in the contentless boot
  scene. Retargeted to `MilestoneA_GrabCube`; added a `TravelCoordinator` guard so `_Boot` can
  never be a travel destination again.

## 🟠 P1 — Core controls (target feel: console/Fortnite-like)

- [x] **Jump (A button)** — FIXED. `A` is now a vertical jump (was a forward dash).
- [x] **Sprint (click left thumbstick)** — FIXED. Hold L3 for ~1.9× move speed.
- [ ] **No locomotion in the test room.** Walking works in `D0_City` but not `MilestoneA`.
  D0_City has a per-scene `LocomotionDirector` that sets the move provider's speed; MilestoneA
  doesn't. Fix: apply the default `LocomotionProfile` from the persistent rig / on every scene.
  *(needs a "tried to walk in test room" device log to confirm before fixing)*
- [ ] **Choppy head-tracking in D0_City.** Performance — D0_City is unoptimized. Lower priority.

## 🟡 P2 — Weapons / combat

- [x] **Gun aim direction** — FIXED. Guns now use the fixed Grip attach (`useDynamicAttach=false`)
  so they snap to a forward-facing orientation on grab (verify feel on device).
- [ ] **Taser dart stops short.** The "stuck bullet" is the taser dart sticking on its first
  collision (likely the muzzle/player or low launch velocity). Needs `TaserDartGunRuntime`
  launch velocity + collision-ignore review.
- [ ] **Drone hit reaction.** Drone should take a hit, crash, fall, and remain (not vanish).
- [ ] **Holster on waist.** Put the gun on the belt and grab it back. `BeltRig` +
  `HolsterSocketInteractor` exist but aren't wired onto the persistent rig in these scenes.
- [ ] **Grab distance.** Limit grab to a sensible range (currently grabbable from far via ray).

## ✅ Also fixed (travel robustness)

- [x] **Gun lost on travel** (`ITEM_DEF_NOT_FOUND`). `ItemFactory` now caches item definitions
  once loaded so inventory restore after travel can resolve them.

---

## 🛠️ Workflow improvements (proposed)

1. **Build with scene patching, not build-only.** `dev_build_install.ps1` calls
   `BuildAndroid.APK` (no patching). The master plan's intended build is
   `BuildAndroid.PatchScenesThenAPK`, which runs the scene patchers **and the
   `WorldAuditRunner`** (catches spawn-below-floor, missing-door, etc. — exactly the class of
   bug that can cause the brown screen). Switching to it makes every build self-validating.
2. **One-shot log capture.** Add a script that captures a full play session's logcat and pulls
   the on-device diagnostic file (`debug-cb967f.log` from `persistentDataPath`) in one command,
   so diagnosing on-device issues doesn't need manual multi-step capture.
3. **Stop committing logs to git.** Gitignore `*.txt` logcat dumps and `debug-*.log` so the
   repo stays clean (several multi-MB logs got committed during debugging).
4. **CI as the safety net.** Every push runs compile + EditMode tests, so we catch breaks
   before they reach the headset.

---

## Project status (from master plan)

- **Playable now:** `MilestoneA_GrabCube` (test room), `D0_City` (Toxic Venice blockout).
- **Systems present:** VR locomotion, pistol/holster/belt, scene travel, jobs, drones, taser,
  audio, world audit.
- **Long-term:** 80 worlds / 12 chapters / RILL companion — see master plan Phases E0–E7.
  Current focus is solidifying the 2-scene vertical slice (this worklist) before scaling.
