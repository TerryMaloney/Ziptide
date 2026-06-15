# Sandbox Test Lab — Design + Build Plan

A permanent dev-only scene (`SandboxTestLab`) where any feature, art style, weapon, enemy,
or effect can be **prototyped and verified before it ships to a real world**. Decisions:
2026-06-15.

The guiding rule: **nothing ships to the game until it passes in the sandbox.** New surface
family? Prototype here. New weapon? Test here. Ziptide transition effect? Build and tune here.
The lab replaces "build it in D0_City and hope for the best."

`MilestoneA_GrabCube` stays as the pure Milestone A grab-cube verification scene (unchanged,
the recorded baseline). The sandbox is a SEPARATE scene that can be as messy as needed.

---

## Scene

**Scene name:** `SandboxTestLab`
**Scale:** 30 m × 30 m (open, flat floor). Larger than the current test room — enough to
see visual effects, test movement, and set up multi-station test rigs side by side.
**Lighting:** Neutral directional light + ambient fill (no dramatic theme). Grid decal on the
floor for spatial reference. No fog — clear sightlines for visual debugging.
**Boot contract:** Same as all world scenes — content only; XR rig + singletons live in
`_Boot`. Has `__SPAWN_PLAYER` (SpawnMarkerRuntime) and a `WorldRuntime`. No XRInteractionManager.

---

## Zones (each is a marked floor section with a sign prop)

### Zone A — Grab + Holster test
- All item types on pedestals (pistol, taser, any new items).
- A holster socket at hip height.
- Goal: verify grab-distance, snap-to-grip orientation, holster dock, and holster-rides-hip
  during locomotion. Check after every InventoryState or BeltRig change.

### Zone B — Weapon range
- Fixed targets at 3 m, 6 m, 12 m, and 20 m.
- Taser drone (hovering, stationary) for taser-fly verification.
- Moving target rail (slow back-and-forth) for tracking.
- Goal: verify bullet spawn position (no muzzle-sticking), hit detection, kill+fall.

### Zone C — Enemy sandbox
- A spawn button (interactable) that spawns a fresh drone.
- A "reset all drones" button.
- Room to run: drone AI needs clearance.
- Goal: test DroneRuntime states (hover → hit → Kill → tumble+stay-down).

### Zone D — Travel loop (ziptide test)
Two travel doors:
- **Door A** → teleports player to Door B arrival marker.
- **Door B** → teleports player back to Door A arrival marker.
This forms a tight loop using the real `ProximityTravelTrigger + TravelCoordinator.TravelTo()`
pathway, so travel bugs surface here without needing two full worlds.

The ziptide transition **visual effect** is built and tuned here before being promoted to
real world transitions (see "Ziptide Effect" section below).

### Zone E — Art prototype wall
A flat 10 m × 4 m wall that is INTENTIONALLY BLANK, waiting for prototyped surface kits.
First use: the **Alien Origami / Pattern** surface family (see
`docs/project_art_plan/ALIEN_ORIGAMI_SURFACE_BRIEF.md`).
Process: place kit meshes on this wall → verify Quest budget (≤ 72 FPS, draw call check) →
if passes, promote kit to `WorldArtKitDefinition` asset + give it an ID.

Also: origami demo section — 4–6 kit shapes (fold panel, chevron arch, star cluster, lotus
tower stub, tesseract shard, glyph band) arranged as a small corner vignette so Terry can
experience the aesthetic in VR before committing to a full world.

### Zone F — Locomotion track
A simple obstacle course: ramp up, flat catwalk, step down, a gap (jump needed).
Goal: verify walk/run/jump/sprint feel and that the lower-level-fall glitch cannot
reproduce here (no stray geometry below the floor).

---

## The Ziptide Transition Effect

Currently, world travel is an **instant cut** (SceneManager.LoadScene with no visual). The
"ziptide" is supposed to feel like being pulled through a rip in space — the namesake of
the game. This effect needs to be built and owned here before it goes into real travel.

**Planned effect (build order, each step testable in Zone D):**
1. **Fade-to-black** (0.2 s) + optional screen-edge chromatic aberration on the XR camera.
   Implemented as a `TravelFadeOverlay` on a Canvas in _Boot (DontDestroyOnLoad).
   `TravelCoordinator` signals it at `TRAVEL_START` → it fades in, travel loads, fades out.
2. **Particle burst at the door frame** (on exit only): a quick burst of teal + gold sparks
   from the `ProximityTravelTrigger` position (using a PooledVFX from _Boot), played before
   the fade triggers.
3. **Audio stinger**: a short "ziptide" sound (pitched whoosh) played by `AudioDirector` on
   `TRAVEL_START`. Pair with the fade so the cut happens at the audio peak.
4. **Arrival shimmer** (optional, comfort-safe): a 0.3 s brightness/contrast ramp-in on the
   camera after the scene loads, masking any pop-in. Similar to how RE4 VR handles room entry.

Steps 1 and 3 are the minimum viable effect (comfort-safe, low perf cost). Steps 2 and 4 are
polish. Test in Zone D: walk through door, confirm fade + stinger, confirm arrival is clean,
confirm no motion sickness (seated testers especially).

**Comfort rule:** NO camera rotation or translation during the transition. Fade-to-black is the
gold standard for VR comfort; everything else (sparks, shimmer) is additive and spatial, not
attached to the camera.

---

## Build order (CI-gated; after CI license is green)

1. **`ScenePatcherSandbox`** (editor script): creates the 30×30 room, six zones with floor
   markers, spawn marker, WorldRuntime. Idempotent (re-run safe). Add `SandboxTestLab` to
   Build Settings (keep it dev-only: `#if UNITY_EDITOR || DEVELOPMENT_BUILD`).
2. **Zone A + B setup**: pedestals and targets placed by the patcher (reuse existing
   ItemFactory + TargetRuntime + DroneRuntime patterns).
3. **Zone D travel loop**: two ProximityTravelTrigger doors pointing at each other (same scene,
   teleport via TravelCoordinator loop). Verify holster persists through the loop.
4. **Zone E origami vignette**: place the 6 kit meshes from the Alien Origami brief.
   Verify Quest perf budget before committing the kit.
5. **Ziptide fade overlay** (`TravelFadeOverlay` on _Boot Canvas): wire to TravelCoordinator.
   Test in Zone D. Then promote to all worlds.
6. **Ziptide audio stinger**: short whoosh SFX in AudioDirector, triggered on TRAVEL_START.
7. **Zone F locomotion track**: ramp/gap geometry, verify no sub-floor escape.
8. A `Ziptide > Open Sandbox` editor menu shortcut that jumps directly to the scene in Editor.

---

## Patcher architecture note

`ScenePatcherSandbox` follows the same pattern as `ScenePatcherD0`: idempotent, places all
objects procedurally, runs inside `BuildAndroid.PatchScenesThenAPK`. The sandbox scene is
never hand-authored — the patcher IS the source of truth. This means:
- Adding a new zone = add it to the patcher and re-run.
- Changing zone layout = edit the patcher (not the .unity file).
- The .unity scene file stays lean (just the patcher-placed objects).

---

## Promotion criteria (sandbox → real world)

Any feature passes the sandbox when:
- [ ] No ZIPTIDE: errors in logcat during the test.
- [ ] Quest frame rate ≥ 72 FPS in the relevant zone (GPU profiler confirms).
- [ ] The feature works on both initial entry AND after a travel round-trip.
- [ ] Terry has physically tested it on device (not just in Editor).
Only then does the feature get promoted to D0_City or another real world scene.
