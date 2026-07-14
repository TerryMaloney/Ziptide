# Device Stabilization Forensic Plan

**Status:** research-only handoff for Fable 5 / Picasso / GPT  
**Scope:** no gameplay, scene, asset, or system implementation changes were made during this pass.  
**Source of device observations:** Terry's first successful post-audit Quest build, 2026-07-14.  
**Multiplayer:** paused until the single-player/device baseline below is stable.

## Evidence rules

This plan deliberately separates three kinds of conclusions:

- **CONFIRMED SOURCE CAUSE / COLLISION** — the device symptom and the responsible source contract line up directly.
- **CONFIRMED SOURCE LOCUS; RUNTIME ROOT UNPROVEN** — the exact subsystem is identified, but the final failing branch needs device instrumentation before changing behavior.
- **EXPLICIT PLACEHOLDER / CONTENT DEBT** — the source itself says the current visual is a blockout, fallback, interim asset, or primitive approximation.

No unresolved item below is assigned a cause merely because it sounds plausible.

---

## Executive finding

The build is not suffering from one global rendering failure. It is a stack of independently valid systems that were never reconciled on-device:

1. a new cold-boot menu now holds the player inside an intentionally empty `_Boot` scene while older locomotion and fall recovery remain active;
2. two different developer-warp interfaces both self-bootstrap and persist;
3. authored traversal paths are not checked against the generated cave geometry they cross;
4. item pose, hand pose, holster pose, muzzle origin, and physical scale do not share one contract;
5. tutorial repair, objective progress, and cast-off presentation have separate state consumers and insufficient runtime evidence when they disagree;
6. the ship and world presentation layers still contain explicitly documented interim primitive art;
7. structural CI correctly proves “buildable and auditable,” but it does not prove interaction feel, pose correctness, UI facing, visual finish, or objective continuity on a tracked Quest.

The repair order must therefore be **function before art**, while saving the ship and visual work as named Picasso follow-ups rather than allowing it to disappear.

---

# Failure map

## DS-01 — Cold boot menu allows falling and repeated respawn

**Classification:** CONFIRMED NEW/LEGACY COLLISION  
**Device observation:** the destination menu appears immediately; moving causes falling and repeated respawn.

### Source chain

- [`ScenePatcherBoot.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherBoot.cs) intentionally keeps `_Boot` content-only: persistent XR rig and boot services, not a playable floor.
- [`ScenePatcherD2.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs) still installs the full locomotion rig, `LocomotionDirector`, `PlayerRigPersistence`, and emergency recovery in `_Boot`.
- [`BootLoader.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/World/BootLoader.cs) now waits for a user destination choice instead of immediately leaving `_Boot`.
- [`HomeHubRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/HomeHubRuntime.cs) creates the destination board and tiles in front of the camera, but creates no safe floor and does not suspend locomotion or fall recovery.
- [`PlayerRigPersistence.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs) records a safe position, detects falling below its threshold, and teleports back. In an empty scene with locomotion active, that becomes a fall/respawn loop.

### Exact collision

The **new wait-in-boot menu flow** is running inside the **old assumption that `_Boot` is transient and immediately loads playable content**.

### Required correction

Create one explicit cold-boot state contract:

- locomotion and fall recovery are disarmed while destination selection is visible;
- the rig is anchored to a verified safe boot pose, or `_Boot` receives a deliberately authored safety floor;
- fall recovery arms only after a content scene is loaded and its spawn has settled;
- destination selection can transition exactly once.

### Proof gate

- Cold boot can remain open for 60 seconds without Y loss or respawn.
- Thumbstick input cannot move the rig while the menu owns focus.
- Selecting a destination produces one load and one spawn-settle event.

---

## DS-02 — Multiple competing developer warp menus

**Classification:** CONFIRMED LEGACY/NEW COLLISION  
**Device observation:** multiple map-jump menus appear; one is backward, follows the player poorly, and cannot be dismissed.

### Source chain

- [`DevMenu.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs) self-bootstraps in development builds, persists across scenes, is summoned by the forehead gesture, and supports explicit show/hide/close behavior.
- [`DevWarpBoard.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs) also self-bootstraps, persists, and describes itself as a replacement for the flaky menu. It creates a physical board, repeatedly relocates it around scene spawns, billboards it toward the camera, and has no close path.
- [`DevMenuGesture.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenuGesture.cs) implements the intended both-controllers-above-head gesture.
- [`DEV_MENU_ACCESS.md`](DEV_MENU_ACCESS.md) confirms the forehead gesture is current and Y+B is retired.

### Exact collision

Two separately designed replacement systems are both live. The gesture is not the legacy problem; the duplicate menu ownership is.

### Required correction

- Choose one authoritative runtime developer menu. Recommended owner: summonable `DevMenu` because it already has hide/close semantics.
- Disable or remove `DevWarpBoard` from automatic bootstrap; retain only behind an explicit diagnostic flag if it still has a unique use.
- Add a singleton assertion covering both types, not merely each type individually.
- Add a device-facing orientation contract for all world-space text and panels.

### Proof gate

- Exactly one dev interface exists after boot and after five world transitions.
- Forehead gesture toggles it open and closed.
- Panel front normal faces the tracked head when opened; it does not orbit behind the player while walking.

---

## DS-03 — Backward/mirrored developer board

**Classification:** CONFIRMED SOURCE LOCUS; FINAL FACING ERROR REQUIRES DEVICE PROOF  
**Device observation:** test warp board text is backward.

### Source locus

[`DevWarpBoard.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs) uses legacy `TextMesh` labels and computes board rotation with its own billboard formula. There is no shared UI-facing helper or tested front-normal convention.

### Required correction

Do not flip arbitrary axes until the authoritative menu decision in DS-02 is complete. Then standardize one helper that takes:

- viewer position;
- panel front axis;
- text front axis;
- optional fixed-yaw behavior.

### Proof gate

A screenshot from the intended approach side must show readable, non-mirrored title and every destination label.

---

## DS-04 — Undercroft zipline intersects the cave floor and cannot be ridden

**Classification:** CONFIRMED AUTHOR/RUNTIME CONTRACT FAILURE  
**Device observation:** the line enters and passes through the floor; riding fails.

### Source chain

- [`ScenePatcherCavern.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherCavern.cs) chooses the highest and lowest chamber centers, applies fixed vertical offsets, and authors one direct endpoint-to-endpoint chord. It does not sample cave walls, floors, bridge pads, tunnel shells, or player-radius clearance.
- The same cave patcher independently creates chambers, tunnels, bridges, shafts, and pads. Nothing guarantees that the direct chord remains in open space.
- [`ZiplineRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs) draws a sagging visual cable, but advances the rider along a straight A-to-B interpolation and moves the XR rig directly. The visual and physical route are not the same curve.
- [`ZiplineCore.cs`](../Ziptide/Assets/Ziptide/Content/Runtime/Traversal/ZiplineCore.cs) is a straight-line travel model with no collision or route-clearance concept.

### Exact failure

The author assumes any chamber-to-chamber chord is traversable; the generated cave makes no such guarantee. The cable shown to the player also does not match the route used to move the rig.

### Required correction

- Author route nodes or a sampled curve with player-radius clearance against generated cave solids.
- Make the visible cable and rider route use the identical sampled path.
- Reject/remove the zipline at generation time when no valid route exists.
- Add world-audit sampling along the complete path, including entry and exit volumes.

### Proof gate

- Audit capsule samples clear every segment.
- Player can grab/enter, ride end-to-end, and exit without penetrating geometry or triggering fall recovery.

---

## DS-05 — Ziptide gate appears in front of the player instead of around them

**Classification:** CONFIRMED COORDINATE-SPACE CONTRACT MISMATCH  
**Device observation:** effect appears offset in front rather than surrounding the player.

### Source chain

- [`ZiptideGateEffect.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiptideGateEffect.cs) correctly builds its primitive ring/pool around the center it is given.
- [`TravelCoordinator.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs) supplies `rig.transform.position` for departure and arrival effects.
- In room-scale XR, the XR Origin root and the tracked camera/head can have a real horizontal offset.

### Exact failure

The effect contract expects a player-centered world point, while the caller provides the XR rig root. Those are not equivalent after room-scale movement.

### Required correction

- For player-centered transitions, derive center from tracked head XZ projected to the appropriate floor/rig height.
- Keep separately authored door/gate anchors as an explicit alternate mode.
- Add a test with a non-zero camera-local offset under the XR Origin.

### Visual debt

The effect is intentionally composed from primitive cubes/cylinders. Correct placement first; richer presentation is a later VFX/Forge task.

---

## DS-06 — Guns are tiny, point approximately 45 degrees upward, and can hit the player

**Classification:** CONFIRMED POSE/SCALE CONTRACT FAILURE  
**Device observation:** test-room and arena guns are tiny, aim upward, and can hit the player.

### Source chain

- [`ItemFactory.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs) defines `GunGripTilt` as a hardcoded 45-degree X rotation and applies it whenever an item has no explicit authored grip rotation.
- [`DefaultPistol.asset`](../Ziptide/Assets/Ziptide/Resources/Items/DefaultPistol.asset) and [`DefaultTaserDartGun.asset`](../Ziptide/Assets/Ziptide/Resources/Items/DefaultTaserDartGun.asset) leave the relevant pose/scale values at zero, selecting factory defaults.
- Factory fallback dimensions are small real-world meter values and are the visible source when the richer Forge look does not establish the expected physical envelope.
- [`PistolRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/PistolRuntime.cs) raycasts directly from muzzle forward against general physics, without an owner/self exclusion layer contract.
- [`TaserDartGunRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartGunRuntime.cs) spawns its projectile only a short distance in front of the muzzle.
- [`TaserDartProjectile.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Weapons/TaserDartProjectile.cs) accepts its first collision and contains no shooter/owner ignore contract.

### Exact failures

- The 45-degree aim error is the hardcoded fallback pose, not player technique.
- Tiny fallback dimensions are selected by zero-valued item data.
- Self-hit is permitted by both weapon paths because neither establishes a robust owner-ignore contract.

### Required correction

Create one item-pose schema with separately authored:

- right-hand grip pose;
- left-hand grip pose where needed;
- muzzle forward axis and muzzle clearance;
- left/right holster pose;
- physical envelope/scale.

Remove implicit gun tilt as a universal fallback. Add shooter colliders/layers to the ignore contract for hitscan and projectiles.

### Proof gate

For every starter weapon: right hand, left hand where supported, left holster, right holster, aim line, blank-space fire, close-wall fire, and self-collider exclusion.

---

## DS-07 — Waist belt inherits the bad weapon angle

**Classification:** CONFIRMED MISSING HOLSTER-POSE CONTRACT

### Source chain

- [`BeltRig.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs) follows the camera and creates sockets without per-item attach transforms or an authored socket rotation contract.
- [`HolsterSocketInteractor.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs) filters accepted items but does not define their holstered pose.
- Items therefore retain or inherit the same ambiguous attachment orientation used in-hand.

### Required correction

Holster pose must be explicit and independent from hand-grip pose. Each item definition needs left/right socket offsets and rotations, with a standard fallback that is neutral—not `GunGripTilt`.

---

## DS-08 — Hammer strikes sideways

**Classification:** CONFIRMED DUPLICATE IMPLEMENTATION/POSING PATHS  
**Device observation:** hammer head is sideways relative to the intended swing.

### Source chain

- The arena `SonicThumper` is a mallet but [`ItemFactory.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Items/ItemFactory.cs) assigns it the gun fallback tilt.
- [`SonicThumper.asset`](../Ziptide/Assets/Ziptide/Resources/Items/SonicThumper.asset) leaves its explicit grip rotation at zero, selecting that fallback.
- Separately, [`ScenePatcherArena.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs) spawns a `PvpHammer` using [`HammerTool.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/HammerTool.cs), which builds its own primitive handle/head and also lacks an authored grip attach pose.

### Exact collision

There are two hammer/mallet implementations with unrelated construction and pose rules. The next device pass must identify which object Terry handled, but both source paths are currently under-specified.

### Required correction

- Consolidate to one hammer item contract, or name and purpose them distinctly.
- Author an explicit impact axis and grip pose.
- Test that a natural wrist swing presents the hammer head—not its side—to the target.

---

## DS-09 — Arena Match Board is difficult or impossible to select

**Classification:** CONFIRMED SOURCE LOCUS; RUNTIME ROOT UNPROVEN

### Source locus

- [`ArenaLobbyBoard.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/ArenaLobbyBoard.cs) generates many small cube tiles with `XRSimpleInteractable`, primitive colliders, and whichever `XRInteractionManager` is first found.
- [`ScenePatcherArena.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherArena.cs) places and rotates the board near spawn.
- The competing developer board from DS-02 can occupy the same general interaction area.

### What is not yet proven

Source alone does not establish whether the failure is:

- board-facing direction;
- ray hover target/layer;
- collider size;
- interaction-manager binding;
- another panel occluding the ray;
- controller ray state.

### Required evidence before behavior changes

Temporarily log, while aiming/selecting:

- hovered object path;
- collider/layer hit;
- bound interaction manager instance;
- panel normal dot camera direction;
- select-entered event.

Then fix the evidenced branch. The likely UX improvement of larger tiles is not a substitute for that diagnosis.

---

## DS-10 — Gate coupler looks repaired, but the objective still requests repair

**Classification:** CONFIRMED MULTIPLE STATE CONSUMERS; DIVERGENCE ROOT UNPROVEN

### Source chain

- [`W000_DriftIn_WorldPack.asset`](../Ziptide/Assets/Ziptide/Content/Worlds/Packs/W000_DriftIn_WorldPack.asset) defines machine ID `gate_coupler`.
- [`W000_DriftIn_S3.asset`](../Ziptide/Assets/Ziptide/Content/Jobs/Generated/W000_DriftIn_S3.asset) requires one repair of the exact same ID.
- [`RepairableMachine.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs) owns the physical stages. On the final switch it sets `Running`, calls `JobDirector.ReportRepair(machineId)`, logs `MACHINE_REPAIRED`, and emits `StageChanged`.
- [`JobDirector.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobDirector.cs) spawns machines from pack data and forwards repair reports into `JobRuntime`.
- [`JobRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Jobs/JobRuntime.cs) accepts matching repairs and even banks repairs completed before the repair step becomes active.
- [`ShipCastOffRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs) separately observes the machine's repaired state to arm the final ship interaction.

### What is proven

The IDs and intended event path match. Static source inspection does **not** expose an obvious ID mismatch. Therefore assigning the bug to spelling, early repair, or missing bank logic would be guessing.

### Required evidence before correction

Capture a single repair sequence with:

- `JobDirector` instance ID and world pack;
- `RepairableMachine` instance ID, machine ID, and stage transitions;
- `MACHINE_REPAIRED` log;
- `JobRuntime` definition, current step index/type, repair bank before/after, and whether `AdvanceStep` fired;
- `ObjectiveBoard` refresh and rendered step text;
- `ShipCastOffRuntime` observed machine instance ID.

This will distinguish duplicate machine/director instances, event delivery, state reset, or stale presentation without speculation.

---

## DS-11 — “PUNCH IT” cannot be punched

**Classification:** CONFIRMED UX/IMPLEMENTATION MISMATCH

### Source chain

[`ShipCastOffRuntime.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/ShipCastOffRuntime.cs) labels the control `PUNCH IT`, but implements it as a cube with `XRSimpleInteractable.selectEntered`. No physical punch/contact detector is attached.

### Exact failure

The label promises a physical strike; the implementation requires a ray/select interaction.

### Required correction

Choose one truth:

- implement a real hand/contact punch target; or
- rename and visually present it as a selectable launch control.

Do not preserve the current mixed contract.

---

## DS-12 — “Your Ship” turning is glitchy

**Classification:** CONFIRMED SOURCE LOCUS; RUNTIME ROOT UNPROVEN

### Source chain

- [`ShipFlightRuntime.cs`](../Ziptide/Assets/Ziptide/Ship/Runtime/ShipFlightRuntime.cs) keeps the cockpit/rig still and applies the inverse ship pose to the complete world root. It suspends walking locomotion providers while flight is active.
- [`FlightInputCore.cs`](../Ziptide/Assets/Ziptide/Ship/Runtime/FlightInputCore.cs) maps right-stick X to 30-degree snap yaw with a held-stick repeat every 0.4 seconds and re-arms near center.
- [`FlightModel.cs`](../Ziptide/Assets/Ziptide/Content/Runtime/Flight/FlightModel.cs) changes heading only through discrete snap yaw; pitch and world inversion are applied separately.

### What is not yet proven

“Glitchy” could be repeated snap cadence, duplicate input, incorrect locomotion suspension, inverse-world transform behavior, a scene root outside the flight root, or frame spikes. Source inspection does not select one.

### Required evidence before correction

Log one frame stream containing:

- raw right stick;
- latch armed/repeat time;
- emitted yaw snap;
- flight state yaw/pitch;
- world-root applied rotation;
- enabled walking turn providers;
- frame time around each visual jump.

Then correct the demonstrated branch and verify locomotion restoration when leaving flight.

---

## DS-13 — Ships are block assemblies; Broadcast Tomb appears to use a different bad ship

**Classification:** EXPLICIT PLACEHOLDER / CONTENT DEBT, WITH ONE SHARED GENERATOR

### Source chain

- [`ScenePatcherStarterWorld.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ScenePatcherStarterWorld.cs) explicitly describes its environment as a primitive graybox prioritizing scale and path over visuals.
- [`ShipHullBuilder.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/ShipHullBuilder.cs) explicitly labels the current hull `INTERIM` and `STOPGAP`; it builds the silhouette from cube primitives.
- [`CityBuilder.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs) calls that same `ShipHullBuilder` for every enabled city-world shipyard.
- [`W000_DriftIn_Layout.asset`](../Ziptide/Assets/Ziptide/Content/City/Generated/W000_DriftIn_Layout.asset) and [`W004_BroadcastTomb_Layout.asset`](../Ziptide/Assets/Ziptide/Content/City/Generated/W004_BroadcastTomb_Layout.asset) specify the same ship size and rotation. Their palettes and surrounding environments differ.

### Exact conclusion

The repository intends one shared interim hull generator for W000 and W004. The source does not contain two authored final ships. If they appear structurally different on-device after a clean regeneration, capture both scene hierarchies/screenshots before assuming which scene is stale or which visual pass differs.

### Required plan

- **Now:** keep the interim hull but make boarding, coupler state, launch, and flight controls reliable.
- **Named Picasso follow-up:** replace `ShipHullBuilder` with a real Forge-built modular hull while preserving the current boarding/cockpit/coupler anchor contract.
- Create one canonical ship definition and require all parked/flight representations to use it.

This art work is parked, not discarded.

---

## DS-14 — Door text is mirrored

**Classification:** CONFIRMED MISSING FACING CONTRACT

### Source chain

[`WorldTravelStation.cs`](../Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs) creates world-space `TextMesh` labels as children without an explicit front-normal rotation or shared viewer-facing helper. The device observation confirms the resulting approach-side text is mirrored.

### Required correction

Create one world-label authoring helper with explicit panel front and text front axes. Migrate travel doors and other runtime `TextMesh` surfaces to it, then add an approach-side facing audit/test.

---

## DS-15 — Mara’s Last Jump streetlight shows a square instead of believable light

**Classification:** CONFIRMED PLACEHOLDER/FAKE-LIGHT PATH; EXACT QUEST MATERIAL FAILURE NEEDS CAPTURE

### Source chain

- [`PracticalAuthor.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/PracticalAuthor.cs) creates primitive fallback light fixtures, attaches Forge look replacement, and deliberately reserves real Unity lights for a very small subset. Street poles generally use fake halo/pool presentation.
- [`PracticalLight.cs`](../Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/PracticalLight.cs) creates the halo/pool from billboarded quad primitives and generated additive material.
- [`ForgeModuleLook.cs`](../Ziptide/Assets/Ziptide/Visuals/Runtime/Forge/ForgeModuleLook.cs) keeps primitive fallback children when the Forge recipe cannot be applied.

### Exact conclusion

The absence of a real point light on most street poles is intentional budget policy. The visible square is located in the fake halo/fallback path, but source inspection alone cannot distinguish failed alpha softness from a failed Forge swap.

### Required evidence and correction

- Log whether `light_street_pole` Forge application succeeded.
- Capture the rendered halo material/shader and fallback-child active state on Quest.
- Repair the fake-light visual within budget; do not solve it by adding unrestricted real lights.

---

## DS-16 — Buildings read as blank boxes

**Classification:** EXPLICIT FALLBACK / PARTIAL ART FULFILLMENT

### Source chain

- [`BuildingBuilder.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/BuildingBuilder.cs) resolves wall modules through `ArtModuleRegistry`; when unfulfilled, it logs `KIT_UNFULFILLED` and creates primitive cube walls.
- [`ArtModuleRegistry.cs`](../Ziptide/Assets/Ziptide/Editor/Art/ArtModuleRegistry.cs) explicitly defines primitive fallback behavior for unfulfilled module IDs.
- [`ForgeBuildingKit.cs`](../Ziptide/Assets/Ziptide/Editor/Art/ForgeBuildingKit.cs) currently fulfills only a limited subset of salvage-row and toxic-tenement wall modules.
- [`CityBuilder.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/CityBuilder.cs) also builds baseline facade and landmark silhouettes from cubes.

### Exact conclusion

This is not one unexplained shader outage. Large portions of the current city language are intentionally primitive or unfulfilled.

### Required plan

- Emit a per-world art-fulfillment report: requested module ID, fulfilled recipe, or fallback.
- Picasso prioritizes the most repeated visible modules first.
- Structural/audit green must no longer be described as visual completion.

---

## DS-17 — Planets lack clouds/detail and Chitinwall’s planet reads flat

**Classification:** EXPLICIT PROCEDURAL LIMIT

### Source chain

- [`SkyPlanetRig.cs`](../Ziptide/Assets/Ziptide/Visuals/Runtime/SkyPlanetRig.cs) is the legacy planet path: unlit sphere and low-resolution procedural stripe texture.
- [`SkyVistaRig.cs`](../Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaRig.cs) is the newer path: unlit primitive sphere with a CPU-baked texture and tight draw-call budget.
- [`SkyVistaTexture.cs`](../Ziptide/Assets/Ziptide/Visuals/Runtime/SkyVistas/SkyVistaTexture.cs) supports banding/noise/storm features but has no independent cloud layer. Its lighting/phase cue is texture math rather than a lit sphere.

### Exact conclusion

No cloud layer currently exists. The flat read is consistent with the unlit body and simple texture terminator; the device report establishes that the current cues are insufficient.

### Required Picasso/visual plan

- strengthen limb and terminator cues;
- add a budgeted independent cloud layer or baked cloud channel;
- preserve the draw-call budget;
- require Quest screenshots for every arena/world sky, not only edit-mode texture tests.

---

## DS-18 — Distant mountains, objects, and landmarks are too simple

**Classification:** EXPLICIT PROCEDURAL BLOCKOUT

### Source chain

- [`WorldExperienceBuilder.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/WorldExperienceBuilder.cs) builds terrain at a coarse target cell size and assembles arrival vistas—spire, wreck, monolith, crystals, arches, rock clusters—from primitive blocks.
- [`WorldDressingBuilder.cs`](../Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs) still uses block cairns and primitive rock/debris clusters for much of the middle distance.
- The arrival ring is explicitly approximated with a small number of rotated blocks.

### Exact conclusion

The distant forms look simple because the implementation is still a procedural primitive blockout, not because Quest failed to load a hidden final asset set.

### Required Picasso plan

After functional stabilization, replace the highest-silhouette-frequency primitives with Forge modules/mesh families while retaining seeded placement and LOD/budget rules.

---

## DS-19 — Two-Quest helper script fails to parse in Windows PowerShell

**Classification:** CONFIRMED TOOLING FAILURE; MULTIPLAYER DEFERRED

[`tools/two_quest_test.ps1`](../tools/two_quest_test.ps1) contains non-ASCII status punctuation in the region Windows PowerShell reported as an unterminated string. The APK itself installed successfully through direct ADB, but this helper must be normalized and parser-tested before multiplayer resumes.

Do not prioritize this above the single-player stabilization phases.

---

# Legacy/new collision register

| Collision | Older assumption/system | Newer system | Result |
|---|---|---|---|
| Boot ownership | `_Boot` is transient; locomotion/fall safety can stay active | Destination menu waits in `_Boot` | fall/respawn loop |
| Developer UI ownership | physical persistent `DevWarpBoard` replaces old menu | persistent summonable `DevMenu` also bootstraps | duplicate, conflicting menus |
| XR center | rig root represents player center | room-scale tracked head moves relative to rig | Ziptide effect offset |
| Item pose | one factory fallback rotation is adequate | Forge looks, different weapons, holsters, and two-handed/melee items | 45° guns, sideways hammer, bad belt pose |
| Hammer ownership | standalone `PvpHammer` | itemized `SonicThumper` mallet | two pose/impact contracts |
| Traversal authoring | endpoint chord is sufficient | procedurally generated cave solids | line penetrates floor/walls |
| Tutorial state | physical machine state is enough | job objective and cast-off each consume it separately | divergence is possible and under-instrumented |
| Ship representation | primitive static placeholder | broader travel/flight/tutorial systems now present it as a core home | functional expectations exceed art state |
| Planet rendering | legacy stripe sphere | newer procedural vista sphere | both remain unlit/simple; no cloud contract |
| Structural verification | compile/audit green defines technical readiness | real tracked-device UX and art expectations | build passes while game remains unusable |

---

# Ordered stabilization program

## Phase 0 — Freeze and evidence preservation

- Keep multiplayer paused.
- Preserve the first successfully installed APK and its logs as the baseline.
- Add no new worlds, weapons, menus, or progression systems until Phase 4 passes.
- Every unresolved item receives targeted runtime tags before behavioral edits.

**Exit:** one reproducible issue list and one log/screenshot package per cluster.

## Phase 1 — Player survival and UI ownership

1. DS-01 cold-boot locomotion/fall contract.
2. DS-02/03 choose one developer menu, remove duplicate bootstrap, correct facing/dismissal.
3. DS-14 standard world-label facing helper.

**Exit:** boot, stand, choose world, summon/dismiss one dev menu, read all door text.

## Phase 2 — Hands, items, and interaction truth

1. DS-06/07 explicit weapon hand and holster poses, scale, muzzle, self-ignore.
2. DS-08 consolidate hammer path and impact axis.
3. DS-09 instrument and repair Match Board selection from evidence.
4. DS-11 make `PUNCH IT` either a real punch or an honestly labeled select control.

**Exit:** starter item matrix passes in Sandbox and arena on Quest.

## Phase 3 — Traversal and objective continuity

1. DS-04 geometry-safe zipline path and matching cable/rider curve.
2. DS-05 tracked-head-centered Ziptide effect.
3. DS-10 instrument gate-coupler machine → job → board → cast-off state and fix the evidenced break.

**Exit:** W000 tutorial completes once; Undercroft zipline rides end-to-end; travel effect surrounds the player.

## Phase 4 — Ship function before ship art

1. DS-12 instrument flight input/transform path and repair the demonstrated turning fault.
2. Verify boarding, launch, exit, and walking-locomotion restoration.
3. Establish one canonical ship anchor/representation contract across W000, W004, parked ship, and flight lane.

**Exit:** reliable ship loop using interim hull.

## Phase 5 — Named Picasso visual recovery

1. DS-13 real modular ship hull.
2. DS-15 practical-light halo/fixture quality.
3. DS-16 building module fulfillment.
4. DS-17 dimensional planets/clouds.
5. DS-18 distant terrain/landmark mesh families.
6. Enrich Ziptide VFX after DS-05 placement is stable.

**Exit:** Quest screenshot review, not source-only approval.

## Phase 6 — Resume multiplayer

1. Repair and parser-test `two_quest_test.ps1` under the exact Windows PowerShell version Terry uses.
2. Reinstall the now-stable build on both headsets.
3. Presence test first; combat synchronization remains its own later gate.

---

# Required verification changes

The new patch-scenes/world-audit CI gate remains necessary, but these device failures show it is not sufficient. Add distinct contracts rather than weakening the existing audit:

- **Boot-state contract:** no locomotion/fall recovery while menu owns `_Boot`.
- **Developer-UI singleton contract:** exactly one runtime dev interface across both classes.
- **World-label facing contract:** front normal/readability from intended approach.
- **Traversal clearance audit:** player-radius samples along authored zipline curve.
- **Item pose contract:** explicit hand/holster/muzzle data; no universal 45-degree fallback.
- **Owner-ignore combat contract:** hitscan/projectiles cannot collide with shooter rig.
- **Tutorial event trace:** machine, job runtime, objective board, and cast-off consume the same machine instance/state.
- **Art-fulfillment report:** world-by-world fallback inventory.
- **Quest review gate:** screenshots/video for menu, hands, ship, lighting, buildings, and sky.

---

# First implementation packet for Fable 5

When usage resets, start only with **Phase 1**. Do not mix ship art or sky art into that patch.

1. Re-read DS-01 through DS-03 and inspect the generated `_Boot` scene.
2. Choose the authoritative dev menu and record the retirement path for the other.
3. Implement boot-state locomotion/fall suspension and deterministic arming after content spawn.
4. Add the boot and dev-menu singleton tests.
5. Run EditMode + patch/audit CI.
6. Produce one Quest build for Terry with a focused checklist:
   - wait at boot without falling;
   - attempt thumbstick movement while menu is open;
   - choose one world;
   - open/close the single dev menu three times;
   - move and turn with menu closed;
   - read one travel door from the approach side.

Only after that device pass should Phase 2 begin.
