# ZIPTIDE — System Connections & Recovery Plan

The "what is supposed to be connected, how, and how we stop it breaking" document.
Written after a deep debug pass that took the game from fully frozen to a working core loop.

Last updated: 2026-06-15

---

## 1. The system map (what connects to what)

```
_Boot scene (index 0, persistent — owns ALL runtime)
  XR Origin (DontDestroyOnLoad)
    ├─ Camera + controllers + ray/direct interactors
    ├─ XRInteractionManager + InputActionManager  ← the ONE input owner
    ├─ Locomotion System (move / snap turn / smooth turn providers) + CharacterController
    ├─ DashLocomotion        ← jump (A) + sprint (L3) + keeps move actions live
    ├─ PlayerRigPersistence  ← owns travel wiring, belt, XRI rebinding
    └─ BeltRig (ensured at runtime) → Holster sockets (left/center/right)
  TravelCoordinator (DontDestroyOnLoad) ← the ONLY way to change scenes
  AudioDirector / Save / diagnostics (DontDestroyOnLoad)
  BootLoader → on Start, TravelCoordinator.TravelTo(FirstWorldScene)

World scene (MilestoneA_GrabCube, D0_City, … — CONTENT ONLY, loaded Single)
  ├─ Ground / geometry / props
  ├─ __SPAWN_PLAYER (SpawnMarkerRuntime)   ← where the rig is teleported on arrival
  ├─ WorldRuntime (+ WorldDirector → SkyPlanetRig)  ← per-scene visuals/theme
  ├─ guns / targets / drones / job objects (scene-local content)
  └─ WorldTravelStation / ProximityTravelTrigger → TravelCoordinator.TravelTo(dest)
```

### Travel flow (every transition goes through this)
`TravelCoordinator.TravelTo(scene)` → save HOLSTERED inventory → `SceneManager.LoadScene` (Single)
→ teleport rig to `__SPAWN_PLAYER` → `EnsureXRIWiring` (rebind interactors, keep input actions
enabled, disable ray anchor-control, ensure belt) → wait for XRI ready → restore holstered items
→ `TRAVEL_OK`.

### The non-negotiable rules
1. World scenes contain **no** XR rig / XRInteractionManager / InputActionManager / TravelCoordinator / AudioDirector / persistent inventory. Those live only in `_Boot`.
2. `_Boot` is **never** a travel destination (it's the bootstrap).
3. `TravelCoordinator.TravelTo` is the **only** travel entry point.
4. Only **holstered** items travel between scenes; loose/in-hand items belong to their scene.

---

## 2. Root causes found & fixed this session

| Symptom | Real cause | Fix |
|---|---|---|
| Frozen "screenshot", dead controllers | InputActionManager (with the action asset) lived on a standalone object the wiring ignored, and died on travel → input disabled | Consolidate the action asset onto the persistent rig, keep it enabled across loads (`EnsurePersistentInputActions`) |
| "Nothing I fixed worked" for hours | `git pull` aborted every time on `D0_City.unity` churn → builds never contained the fixes | `.gitattributes` marks Unity YAML binary-EOL (no phantom diffs); `git reset --hard origin/<branch>` to force-sync |
| Brown screen on return | return door pointed at `_Boot` → re-ran bootstrap, duplicate singletons | retarget door to `MilestoneA_GrabCube` + `TravelCoordinator` guard blocking `_Boot` |
| Guns pile up each round-trip; grey gun follows you | inventory saved+restored ALL loose items on top of each scene's own spawned guns | only **holstered** items travel |
| Gun lost on travel (`ITEM_DEF_NOT_FOUND`) | factory only found already-loaded definitions | cache definitions once loaded |
| Taser darts stick at the muzzle | dart collided with the gun/player on spawn | ignore gun + player colliders on launch, spawn further out |
| Drone shot but floats back up | drone had a "Recovering" state | kill = stop hover, fall under gravity, stay down |
| `A` launched you forward | `A` was bound to a forward dash | `A` = vertical jump; L3 = sprint |
| No belt | `BeltRig` only added per-scene by a patcher the build skips; holsters were markers, not sockets | ensure `BeltRig` on the persistent rig at runtime + give holsters real `HolsterSocketInteractor` sockets |
| Grab reels in/out, hard to orient | ray interactor anchor-control on | disable anchor control |

---

## 3. Known issues still open (next session)

1. **Lower-level glitch / test targets in toxic city.** `D0_City.unity` contains `Ground` +
   `Target1/2/3` (test-room-style content) **and** `ToxicSurface`. The player falls through to a
   leftover test floor with targets beneath the city. **Fix approach:** remove the stray
   `Ground`+`Target1/2/3` (or relocate spawn above solid `ToxicSurface`) — do this in Unity with
   visual verification, OR rebuild D0_City via `ScenePatcherD0` + `WorldAuditRunner` (which has a
   `SPAWN_BELOW_WALKWAY` blocker for exactly this). Needs a careful, verified scene edit.
2. **One gun (turquoise) in both scenes.** Guns are committed scene objects (build skips
   patchers). Decide: edit the committed gun objects, or switch the build to `PatchScenesThenAPK`
   so `ScenePatcherC0/D1` author them consistently.
3. **Grab-from-too-far.** Anchor control is now off; if far-grab is still unwanted, restrict guns
   to near/direct grab via interaction layers (needs rig interactor config).
4. **Jumpy look on entry then smooth.** First-frame shader/asset warm-up. Low priority; consider
   a shader warm-up or a brief fade-in on travel.
5. **Verify first-load movement** (force-enable move actions fix) and **belt** on device.

---

## 4. How we stop this from breaking again (the plan)

**Build = self-validating.** Move `dev_build_install.ps1` to `BuildAndroid.PatchScenesThenAPK`
so every build runs the scene patchers **and `WorldAuditRunner`** (spawn/floor/door/contract
blockers abort the build). This catches lower-level/spawn bugs before the headset.

**CI compiles every push.** `ci.yml` now runs on `terry-local-wip` too — a bad API or syntax
error is caught in GitHub before you waste a local build. Keep the EditMode tests green.

**One source of truth per concern.** Input = one InputActionManager on the rig. Travel = only
`TravelCoordinator.TravelTo`. Inventory-that-travels = only holstered. Don't reintroduce
per-scene copies of rig-owned systems.

**The session discipline (do every time):**
```
1. powershell tools\ziptide_snapshot.ps1     # confirm branch + commit
2. git pull   (if it complains: git reset --hard origin/terry-local-wip)
3. build + install
4. snapshot again → confirm Commit matches what you expect
5. play, then capture: adb logcat -d | Out-File t.txt; Select-String t.txt "ZIPTIDE:"
6. report by tag (TRAVEL_OK / _FAIL, XRI_READY, LOCO_STATE, MOVE_DIAG, DRONE_DOWN, BELT_ENSURED)
```

**Tag everything.** Runtime systems log `ZIPTIDE: <TAG>` so a single logcat tells the whole
story. Diagnose from tags, not guesses.

**Small, reversible commits + known-good tags.** When a build plays well:
`git tag ziptide-known-good-YYYY-MM-DD && git push --tags` for a clean rollback point.
