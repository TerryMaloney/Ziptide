# R1 PACKET 01 — PLAYMODE INFRASTRUCTURE SPIKE

**Status:** IMPLEMENTED — first observation pending  
**Scope:** verification infrastructure only  
**No gameplay/runtime production behavior changed**

## Purpose

Prove that Unity 2022.3.62f3 and the existing GameCI environment can run a PlayMode assembly, advance the player lifecycle by one frame, clean up tests-only objects and exit reliably.

This packet deliberately does not load a project scene, start OpenXR, create a player rig or test TravelCoordinator. Those layers are blocked until this lane proves stability.

## Files

- `Ziptide/Assets/Ziptide/Tests/PlayMode/Ziptide.Tests.PlayMode.asmdef`
- `Ziptide/Assets/Ziptide/Tests/PlayMode/PlayModeInfrastructureTests.cs`
- required Unity `.meta` files
- `.github/workflows/recovery-playmode.yml`

## Test contract

`OneFrameRunner_ExecutesLifecycleAndCleansUp`:

1. asserts the test is executing while `Application.isPlaying`;
2. creates one tests-only GameObject;
3. adds `PlayModeFrameProbe`;
4. proves `Awake` executed immediately;
5. yields one frame;
6. proves `Start` and at least one `Update` executed;
7. destroys the object;
8. yields one frame;
9. proves the object did not leak.

No real-time waits, scene YAML, XR loader or hardware dependency.

## Workflow contract

`Recovery PlayMode Observation`:

- separate from existing required CI;
- separate PlayMode Library cache;
- `testMode: playmode`;
- uploads PlayMode results even on failure;
- records tested SHA, workflow run, attempt and test outcome in `docs/recovery/generated/recovery_playmode_observation.md`;
- a red result remains visible and is not converted into a green summary;
- generated observation commits use `[skip ci]` and are outside the workflow path filter.

## Promotion gate

PlayMode remains an observation lane until:

1. this infrastructure SHA produces a green observation;
2. an unrelated descendant commit produces a second green observation;
3. both runs upload test-result artifacts;
4. neither run requires scene YAML, XR hardware or timing sleeps;
5. no existing EditMode or patch/audit requirement is weakened.

Only then may R1.2 fake-XR work begin.

## Stop rule

A red first run is an infrastructure task only. Diagnose assembly, GameCI, licensing, lifecycle or cleanup. Do not add fake rig, boot, travel or scene tests until the one-frame test is green.
