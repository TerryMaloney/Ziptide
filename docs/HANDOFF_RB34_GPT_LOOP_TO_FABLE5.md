# HANDOFF rb34 — GPT recovery-loop failure and Fable 5 takeover

**Date:** 2026-07-16  
**Audience:** Fable 5 / next recovery operator  
**Authority:** Terry explicitly requested this handoff because GPT repeatedly appeared to remain in a “sleeping for 90 seconds” loop instead of advancing the recovery lane.  
**Status:** **NOT headset-ready. No Quest authorization. Recovery freeze remains active.**

## Why this handoff exists

GPT recovered the prior session state correctly and made two bounded test-harness repair packets, but then repeatedly used a passive wait/poll pattern while waiting for the push-triggered Recovery PlayMode workflow. In Terry’s UI this remained displayed as “sleeping for 90 seconds” for roughly twenty minutes and looked identical to the stall that ended the previous recovery chat.

The operational mistake was not a Unity/project deadlock. The GitHub helper GPT first used for workflow discovery (`fetch_commit_workflow_runs`) returns pull-request-triggered runs, while the canonical Recovery PlayMode workflow runs on **push to `terry-local-wip`**. It therefore returned no run, and GPT kept waiting instead of switching immediately to the durable evidence path:

- poll `docs/recovery/generated/recovery_playmode_observation.md` on `terry-local-wip`;
- inspect branch-head bot commits;
- or fetch the known workflow run/artifact once the durable observation records its run ID.

No background automation was actually running during the repeated UI sleep state. Treat this as an operator/tooling-loop failure. Do not repeat the polling strategy.

## Canonical branch and current source

- Source of truth: `terry-local-wip`, **not `main`**.
- Current source repair candidate: `15e751d4e1e98ec68645b39a7cfdeef81e4e8a56`.
- Later branch-head commits are generated report/verdict commits and do not replace that source SHA.
- Ordinary CI for `15e751d4` is green:
  - workflow run `29494329374`;
  - EditMode success;
  - patch-scenes + audit success;
  - project contract reports success;
  - Android skipped as expected for an ordinary source push.
- Ordinary CI green is **not** R1 integration clearance.

## What GPT changed in this takeover window

### PR #34 — merged as `5aba5984a7a333af861db6d80dddd6a1f3da3fcd`

Test-only packet. It:

1. replaced the Linux-batchmode `WaitForEndOfFrame` dependency in `RecoveryActualSceneSnapshotTests` with ordinary settled frames; `RecoveryRenderSnapshot.Capture` still performs the explicit synchronous `Camera.Render`, and its assertions were not weakened;
2. queued explicit neutral state for test-only XR sticks;
3. added stronger bilateral/readability diagnostics for the virtual XR layout.

Result: Recovery PlayMode run `29493423841` improved from the prior **36/38** to **37/38**. The visual timeout was closed. One Input System failure remained.

### PR #35 — merged as `15e751d4e1e98ec68645b39a7cfdeef81e4e8a56`

Test-only packet. Evidence showed the simulator had preserved only `InputActionAsset.enabled`, then called `asset.Enable()`, which silently re-enabled production-disabled `Rotate Anchor` and `Translate Anchor` actions. The packet snapshots/restores exact per-action enabled state, scopes the audit to the test-owned virtual devices, and reads Move/Turn actions using their declared value types.

This removed the prior Input System/SnapTurn processor failure. Do not reopen that issue unless a new artifact reproduces it.

## Latest exact Recovery PlayMode result

Durable observation:

- tested SHA: `15e751d4e1e98ec68645b39a7cfdeef81e4e8a56`;
- workflow run: `29494329424`;
- artifact ID: `8373877528`;
- NUnit: **38 total, 37 passed, 1 failed, 0 skipped, 0 inconclusive**.

The one remaining failure is now visual, not XR input:

`Ziptide.Tests.PlayMode.RecoveryTravelSaveRoundTripTests.NewGame_W000_ToxicCity_W000_PreservesCompositionAndDiskState`

The failure is emitted by `RecoveryGoldenTravelVisualCapture` at ToxicCity arrival:

`R1_7_ACTUAL_TOXIC_CITY_SPAWN has too little color information; likely blank/unrendered. Expected greater than 4, but was 3.`

Exact ToxicCity capture metrics:

- PNG: `r1_7_actual_toxic_city_spawn.png`;
- size: 960 × 960, 79,182 bytes;
- SHA-256: `88aa1db267b8e6fb501e85b1ce7c8049f20f823540fc3c51cc738306f8c04099`;
- active scene: `ToxicCity`;
- camera: `XR Origin/Camera Offset/Main Camera`;
- quantized color count: **3**;
- average luminance: **0.9541867594**;
- luminance standard deviation: **0.0007195412**;
- dynamic range: **0.0078839216**;
- clear flags: `Skybox`.

Independent visual inspection of the artifact shows an almost uniform pale-cyan frame, with only extremely faint content/text near the bottom. This is a real failed visual proof. Do **not** lower the color or dynamic-range threshold merely to obtain 38/38.

The test log shows the canonical travel reached W000 and proceeded to ToxicCity; the failure occurred when the destination visual observer synchronously captured ToxicCity from the persistent player camera during the `TravelCompleted` publication path. The prior XR exception is absent. The test later times out because the unhandled visual assertion interrupts the travel-completion route.

## Fable 5’s immediate bounded task

Start by independently re-reading the raw run `29494329424` artifact and the following source, without trusting this interpretation blindly:

- `RecoveryGoldenTravelVisualCapture.cs`;
- `RecoveryRenderSnapshot.cs`;
- `RecoveryTravelSaveRoundTripTests.cs`;
- `TravelCoordinator.cs` around `PublishTravelCompleted`;
- ToxicCity spawn-marker/camera/renderer/lighting ownership;
- generated ToxicCity scene/runtime census evidence, if present.

Then isolate why the camera sees almost only the cyan clear/sky result at the exact ToxicCity completion checkpoint. Distinguish with controlled evidence among:

- capture timing relative to destination settlement/rendering;
- wrong tracked-head pose or camera orientation at ToxicCity spawn;
- spawn marker placement/clearance;
- camera culling/renderer visibility;
- scene lighting/material/sky exposure;
- actual ToxicCity composition missing from the player view.

Do not assume which branch is correct. Add targeted diagnostics/canaries first if the artifact does not already decide it. Keep the existing visual assertion surface intact.

After a bounded fix, rerun the unchanged 38-test suite. The required outcome is an exact-SHA **38/38**, followed by independent inspection of all PNG, UI, spawn-clearance, census, travel, save, and persistent-log artifacts.

## Held work — do not merge yet

- **PR #28 / R1.9 fallback-surface audit:** still held until the current integration route is green and independently inspected.
- **PR #33 / R1.10 performance/leak packet:** still held. Its current source contains another `WaitForEndOfFrame` in the Linux reference-renderer route, the same dependency that caused the earlier 180-second visual timeout. Correct and prove that before merging it.
- Do not begin feature expansion, multiplayer, Forge/Picasso expansion, mass-world work, factory implementation, or broad cleanup.
- Do not authorize a headset build merely because ordinary CI is green.

## Required path after 38/38

1. independently inspect raw R1.5–R1.8 evidence;
2. land and prove R1.9;
3. correct, land, and prove R1.10;
4. build the exact-SHA Golden Android candidate with loud patch/bake/shader evidence;
5. independently sample the claims against raw artifacts;
6. only then issue Terry a bounded Quest checklist.

## Heads-up for Fable 5

The recovery program itself is still functioning: it caught the Home Hub title scale defect, input lifecycle faults, batch-mode capture stall, and now a nearly blank ToxicCity player view before a headset checkpoint. The immediate problem is not that the project is unrecoverable; it is that GPT’s operator loop stopped presenting progress and failed to switch polling methods promptly.

Take over from the exact visual failure above, not from the stale 36/38 or 37/38 Input System diagnosis.

## Commit

Documentation-only takeover handoff. No runtime, scene, prefab, asset, test assertion, or workflow behavior changed by this file.