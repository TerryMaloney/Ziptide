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

- [ ] **Brown screen returning to the test room** (`D0_City → MilestoneA`). Travel *into*
  D0_City works; the return door produces an unresponsive brown view. Suspect the toxic
  theme/fog not being reset on travel, and/or a spawn/camera problem on re-entry. *(under
  investigation)*

## 🟠 P1 — Core controls (target feel: console/Fortnite-like)

- [ ] **No locomotion in the test room.** Walking works in `D0_City` but not `MilestoneA`.
  Locomotion config is likely per-scene instead of on the persistent rig. Fix: make
  locomotion live on the persistent rig so every scene has it. *(under investigation)*
- [ ] **Jump (A button).** Currently `A` triggers a forward dash ("launches you forward").
  Want: `A` = vertical jump. The forward dash is `DashLocomotion` bound to RightHand
  primaryButton — needs to become a real jump (CharacterController + gravity).
- [ ] **Sprint (hold left thumbstick).** Not implemented. Want: hold/click left stick to run
  (raise move speed while held).
- [ ] **Choppy head-tracking in D0_City.** Performance — D0_City is unoptimized (~1800+ tris,
  per master plan R-24). Lower priority than the blockers.

## 🟡 P2 — Weapons / combat

- [ ] **Gun aim direction.** Pistol doesn't auto-orient to point forward from the hand on
  grab; player must hand-rotate it. Fix: set a proper grab attach transform so the muzzle
  faces forward.
- [ ] **Projectiles stop short.** Bullets "get stuck a ways out" instead of flying until they
  hit a drone. Fix: projectile needs sustained forward velocity / correct collision.
- [ ] **Drone hit reaction.** Drone should take a hit, "crash and die," fall to the ground,
  and remain (not vanish). Currently no proper death/ragdoll behavior.

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
