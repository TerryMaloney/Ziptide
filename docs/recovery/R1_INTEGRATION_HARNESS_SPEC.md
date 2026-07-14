# ZIPTIDE R1 — INTEGRATION HARNESS SPECIFICATION

**Status:** DESIGN LOCKED DURING R0; implementation begins only after the R0 exit report  
**Owner:** Recovery / Integration lane  
**Unity:** 2022.3.62f3  
**Current test package:** `com.unity.test-framework` 1.1.33  
**Important:** the Performance Testing package is not currently installed; R1 does not silently add it.

## 1. Purpose

R1 makes obvious runtime, lifecycle, UI and exposure failures reproducible without Terry wearing the Quest. It does not try to emulate Meta hardware perfectly. Quest remains the final target checkpoint after the automated evidence bundle passes.

R1 is successful when a candidate change produces one artifact bundle containing:

- tested commit SHA;
- PlayMode test results;
- active scene and recovery exposure profile;
- runtime owner/object census before and after travel;
- event and save trace;
- canonical screenshots from fixed viewpoints;
- UI overlap/facing findings;
- fallback/prototype visibility findings;
- frame/resource measurements available in the non-device environment;
- explicit list of behavior that still requires Quest validation.

## 2. Non-goals

R1 does not:

- repair all gameplay bugs;
- make headless rendering equivalent to Quest;
- enable multiplayer;
- expose every world;
- replace pure EditMode tests;
- hand-edit scenes;
- add a large testing framework before a one-frame infrastructure spike succeeds;
- declare visuals accepted because an isolated Forge booth image looks good.

## 3. Delivery envelopes

### R1.1 — Minimal PlayMode infrastructure spike

Add:

- `Ziptide/Assets/Ziptide/Tests/PlayMode/Ziptide.Tests.PlayMode.asmdef`;
- `PlayModeInfrastructureTests.cs` with one test that enters PlayMode, yields one frame and exits cleanly;
- a separate `playmode-smoke` workflow job with independent cache, results and artifact names.

Initial job state: **non-blocking observation**. It must not alter the required EditMode or patch/audit verdict.

Promotion gate:

1. first green run on the infrastructure commit;
2. second green run after an unrelated descendant commit;
3. no scene YAML, XR loader or real-time sleep dependency;
4. clean exit with test-result artifact both times;
5. only then make PlayMode required for recovery-candidate changes.

Stop rule: if the one-frame test is flaky or cannot exit cleanly, fix only the infrastructure. Do not add boot, travel or XR tests on top of an unstable lane.

### R1.2 — Fake tracked-rig fixture

Create a **tests-only** fixture assembled from code:

- root `RecoveryTestRig`;
- camera offset root;
- tracked-head Camera with configurable non-zero local X/Z offset;
- one `XRInteractionManager`;
- one `InputActionManager` using test-owned actions;
- left and right controller transforms;
- left and right ray interactors;
- optional direct interactors;
- controlled floor collider;
- deterministic spawn marker.

The fixture must not start the OpenXR loader and must not require a connected device.

Required fixture tests:

- one camera and one XRI manager;
- camera local offset survives rig teleport;
- world point from tracked head differs from rig root when configured;
- ray can hover/select a primitive `XRSimpleInteractable`;
- teardown leaves no persistent fixture objects or enabled test actions.

### R1.3 — Recovery exposure profile

Documentation alone cannot hide features. Add one build/runtime exposure contract:

- `RecoveryFeatureId` — closed enum for automatic owners and broad feature surfaces;
- `RecoveryExposureProfile` — immutable data describing allowed features;
- `RecoveryRuntimeGate.Allows(featureId)` — the only bootstrap decision seam;
- profiles:
  - `FullDevelopment` — preserves current development access while migration occurs;
  - `GoldenSlice` — only R3 golden-path and required support owners;
  - `Diagnostic` — GoldenSlice plus explicitly selected diagnostics.

The gate must control automatic bootstraps and scene hooks, not only scene selection. Initial required feature IDs include:

- Debug HUD;
- VR boot diagnostics;
- runtime material fixer;
- runtime input enabler;
- camera enforcer;
- dev warp board;
- conquest mission injection;
- PvP progression;
- Quarters camera injection;
- ecology;
- ambience;
- comfort vignette;
- runtime health monitor;
- first-hour observation.

Rules:

- default editor/development behavior remains explicit, never inferred from `Debug.isDebugBuild` alone;
- a hidden feature cannot subscribe to scene events or create persistent objects;
- the profile name and enabled IDs are logged once at boot;
- tests assert that every cataloged automatic owner has a feature ID or an approved `AlwaysRequired` classification.

### R1.4 — Runtime owner and object census

Add a test/support utility that records after `Awake`/`Start` settle:

- active scene(s);
- all root GameObjects and scene ownership;
- all `DontDestroyOnLoad` roots;
- Cameras and roles;
- `XRInteractionManager` and `InputActionManager` instances;
- Canvas/EventSystem/XRUIInputModule instances;
- persistent gameplay services;
- runtime-created `TextMesh`, TMP, primitives, Materials, Volumes and AudioSources;
- active recovery feature IDs;
- duplicate canonical owner findings;
- forbidden prototype owner findings.

Required checkpoints:

1. before boot scene load;
2. boot surface settled;
3. after first world load;
4. after destination load;
5. after return;
6. after test teardown.

The census is written as JSON and a concise Markdown diff. Assertions use stable type/role IDs, not object-count snapshots for every incidental child.

### R1.5 — Boot and Home Hub smoke

Using the actual `_Boot` scene under `GoldenSlice`:

- wait for Home Hub readiness event with a deterministic timeout;
- assert boot hold active;
- assert one player rig, tracked camera and XRI manager;
- assert no forbidden automatic owners;
- assert Home Hub tiles bind the live interaction manager;
- point the fake right ray at each visible tile and verify hover target identity;
- select Settings and verify no travel commit;
- select New or Continue and verify exactly one travel request;
- leave the menu idle for simulated frames and verify no rig drift/fall recovery loop.

No test may call the Home Hub private choice method directly as its only proof. The integration test must exercise the interactable selection path.

### R1.6 — Scene/travel/save round trip

Start with one selected destination only. R0 decides between ToxicCity and W002; R1 does not support both initially.

Golden trip:

`_Boot → W000 → selected destination → W000`

Required assertions:

- every scene is preflight-loadable;
- one travel request creates one load;
- no direct-load owner bypasses `TravelCoordinator` in the golden path;
- tracked-head offset is used for player-centered effects;
- spawn settles before boot hold/fall net/locomotion release;
- one XRI manager survives each transition;
- inventory restore completes once;
- save/autosave reasons are captured;
- return leaves no duplicate persistent owners, stale UI modules or leaked recovery fixtures;
- profile identity and selected golden-path state survive quit/reload simulation where practical.

The first round-trip test may use a no-op transition visual behind the same contract. Visual acceptance is a separate lane.

### R1.7 — Canonical visual snapshots

Create a separate `visual-smoke` job only after PlayMode is stable. It must run with a renderer-capable CI configuration; do not assume `-nographics` can produce meaningful images.

Named fixed viewpoints:

- `BOOT_HOME_FRONT`;
- `W000_SPAWN_FORWARD`;
- `W000_TOOL_RIGHT_HAND`;
- `W000_TOOL_RIGHT_HOLSTER`;
- `DESTINATION_SPAWN_FORWARD`;
- `DESTINATION_CREATURE_CONTACT`;
- `SHIP_BOARDING_APPROACH`.

Each capture stores:

- PNG;
- camera transform and projection;
- scene/profile/SHA;
- visible renderer/type list;
- active global grade/sky/light owners;
- runtime material names and shaders;
- UI panel/text/collider projected rectangles.

First acceptance is human/LLM review against a contact sheet. Automated image-difference thresholds are not introduced until the canonical references are accepted; otherwise the pipeline would lock current broken visuals.

### R1.8 — UI spatial checks

For every visible candidate panel:

- project text renderer bounds into the snapshot camera;
- identify text-text overlap above a configurable area ratio;
- identify text outside backing-panel bounds;
- identify interactable collider without visible label;
- identify label farther than its target contract allows;
- record panel front and text readable-face dot products;
- identify duplicate front/back text simultaneously visible from one viewpoint;
- record screen occupancy and minimum angular text height;
- flag screen-space or camera-relative overlays not permitted by the active profile.

These checks produce findings first. They become blockers only after calibration against accepted golden panels.

### R1.9 — Prototype/fallback visibility gate

Every player-visible runtime renderer needs a shipping classification:

- `APPROVED`;
- `DEVELOPMENT_VISUALIZATION`;
- `FALLBACK_HIDDEN`.

The GoldenSlice profile fails when a visible renderer is:

- created by `RuntimeMaterialFixer`;
- beneath a forbidden feature injector;
- marked fallback/placeholder/interim without an approved exception;
- in a prototype scene not in the recovery scene manifest;
- an opaque halo/pool quad using the unverified practical-light path;
- generated by an unregistered runtime UI surface.

The source remains available. The gate controls candidate exposure, not deletion.

### R1.10 — Performance and resource evidence

The Unity Performance Testing package is absent. R1 begins with existing/built-in evidence:

- `RuntimeHealthMonitor` frame/resource samples;
- scene-load duration;
- first settled frame after activation;
- `Resources.UnloadUnusedAssets` duration;
- Materials/Textures/Meshes/AudioClips before and after travel;
- active renderer/light/AudioSource counts;
- managed allocation deltas where stable APIs permit.

A separate reviewed package change may add `com.unity.test-framework.performance` later. It must not be bundled into the initial PlayMode spike.

## 4. Workflow topology

The target recovery workflow has independent jobs:

1. `recovery-static` — current Python inventories/maps;
2. `editmode` — existing required pure/contract tests;
3. `patch-audit` — existing generated-scene structural gate;
4. `playmode-smoke` — R1 lifecycle/integration tests;
5. `visual-smoke` — renderer-capable snapshot job;
6. `recovery-artifact-index` — combines SHAs and artifact links, never hides a failed lane;
7. `android-recovery-apk` — on checkpoint dispatch, from the same proven SHA and GoldenSlice profile.

No job may write a single “GREEN” label that implies lanes which did not run. The recovery verdict lists every lane explicitly.

## 5. Terry checkpoint policy

Terry receives an APK only when:

- static inventory has no newly unclassified automatic owner;
- EditMode and patch/audit pass;
- required PlayMode tests pass;
- visual contact sheet has no unresolved P0 layout/fallback failure;
- Android APK is built from that exact SHA;
- the checkpoint has one bounded device checklist and automatic log capture.

Quest testing is performed at recovery milestones, not after every code commit.

## 6. R1 implementation order

1. PlayMode one-frame spike.
2. Repeat-green infrastructure proof.
3. Fake tracked rig.
4. Runtime census.
5. Recovery exposure profile.
6. Boot/Home Hub smoke.
7. One-world travel/save round trip.
8. Renderer-capable snapshot spike.
9. UI spatial findings.
10. Fallback visibility findings.
11. Performance/resource artifact bundle.
12. First recovery APK checkpoint.

## 7. R1 exit gate

R1 closes only when the broken classes currently discovered first on Quest can be caught or explicitly excluded before an APK reaches Terry:

- duplicate/persistent owner;
- forbidden feature injector;
- broken Home Hub ray binding;
- travel lifecycle duplication;
- stale Y+B/menu contract;
- runtime material replacement;
- camera-role collision;
- intrusive HUD;
- overlapping/mirrored candidate UI;
- visible prototype fallback;
- creature contact/grounding failure for the selected golden creature;
- resource accumulation or major load hitch visible in available metrics.

Quest-only rendering, tracking feel and controller ergonomics remain named hardware gates, not hidden assumptions.
