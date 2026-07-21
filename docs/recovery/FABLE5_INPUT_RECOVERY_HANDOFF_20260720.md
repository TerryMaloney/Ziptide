# Fable 5 handoff — Quest recovery input blocker

**Date:** 2026-07-20  
**Operator stopping:** GPT-5.6 Thinking, at Terry's request  
**Branch:** `terry-local-wip`  
**Current branch head when written:** `280d91bf244a61edfb9f08dc82959f5283c476e9` (generated failed PlayMode observation)  
**Last source commit:** `f91c098209cfe4879e2cc5bb069f19b43ce1cf6c`  
**Status:** ✅ **RESOLVED 2026-07-20 by Fable 5 (hwr34) on `c45b1a29`** — the recommended
inert-property normalization landed (`InputMutationRepairDriver.ClearInertDirectProperties`;
root cause: restore's `OnEnable → EnableAllDirectActions` re-enabled the disabled empty action).
Verification per §Verification below: CI GREEN + PlayMode **43/43 twice on the same SHA**
(run `29786603998` attempts 1+2, the determinism requirement) + Golden Android SUCCESS
(run `29786604008`) + clean package proof. **Headset retry is authorized on the
`recovery-golden-apk-c45b1a2…` artifact.** Original stop-state text preserved below for history.

## Read this first

Terry asked GPT to stop and hand this unfinished recovery blocker back to Fable 5. The work is close, but the current source is **not green**: the exact recovery PlayMode route is **42/43** on `f91c098`.

Do not restart the diagnosis. The remaining failure is named below.

## Original headset corrections already represented in the branch

The user reported:

- guns and melee weapons at the wrong held angle;
- weapons extremely small;
- coupler's red/button-looking indicator misleading;
- final coupler control too high for a child and unclear/unselectable.

Keep the existing `QuestWeaponAndCouplerRegressionTests.cs`. It currently enforces:

- human-scale bounds for every weapon family;
- neutral device pose for ranged weapons and the proven reversed Breaker Blade pose;
- collider support around final visible weapon bounds;
- no gun laser on melee tips;
- a non-interactable status indicator;
- a distinct final `PowerSwitch_PRESS` control, hidden until the part is seated;
- switch center at or below 0.80 m, complete target at or below 0.95 m, and at least 0.40 m selectable width;
- stable panel/part release without throw drift.

These corrections have not yet received the final Quest headset retry because the recovery route/input blocker is still open.

## What GPT changed during this recovery attempt

### Pipeline/compile corrections

- Restored `RuntimeInputEnabler.cs` to its original Core-only role after an attempted repair placement created an illegal Core → Gameplay assembly dependency.
- Restored the compile-green `PlayerRigPersistence.cs` settle implementation after an intermediate diagnostic edit left compiler errors.
- Added the missing recovery PlayMode trigger coverage for `RuntimeInputEnabler.cs` and the relevant device-recovery owners.

### Production repair implementation

Added:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs`
- its `.meta`

Installed it through the existing automatic owner:

- `PlayerInputSessionGuard` adds one driver to the canonical `PlayerRigPersistence` object.
- No new persistent root and no independent runtime bootstrap were added.

The driver currently:

- observes completed travel;
- uses the canonical private `_mutationSuspendedReaders` set rather than guessing from provider enabled state;
- cancels an obsolete settle coroutine when chained travel starts;
- repairs affected action maps with bounded disable/enable re-resolution while readers are suspended;
- caps repair at two attempts;
- logs exact failed action state;
- restores the canonical reader list itself after verification;
- treats direct actions with zero bindings as intentionally inert.

## Evidence progression

Do not treat any single green run as sufficient; the candidate proved timing-sensitive.

- Early candidate: compiled, then **41/43** because the repair observer used the wrong suspension signal.
- Canonical reader-list candidate: **42/43**.
- `25a81363049d63138c1517def4b410703ffcad84`:
  - first PlayMode run: **43/43**;
  - same-SHA rerun: **42/43**;
  - therefore rejected as non-deterministic.
- Diagnostic repeat named the stale action exactly:
  - owner: `ActionBasedSnapTurnProvider.leftHandSnapTurnAction`;
  - action: `<direct>/Left Hand Snap Turn`;
  - `bindings=0`, `controls=0`, `activeControl=<none>`;
  - XRI `ReadValue<Vector2>()` produced the Input System `NullReferenceException`.
- `f91c098209cfe4879e2cc5bb069f19b43ce1cf6c` recognizes both empty left-turn direct actions as inert and keeps them disabled, but is still **42/43**.

### Exact remaining failure on `f91c098`

Test:

`Ziptide.Tests.PlayMode.RecoveryGoldenPerformanceRouteTests.GoldenRoute_WaitsForHealthSweepsAndWritesThreePerformanceSamples`

Unhandled exception:

`NullReferenceException` from:

1. `UnityEngine.InputSystem.InputActionState.ApplyProcessors<TValue>`
2. `InputAction.ReadValue<Vector2>()`
3. `ActionBasedSnapTurnProvider.ReadInput()`
4. `SnapTurnProviderBase.Update()`

Relevant successful repair log immediately before the exception:

- `INPUT_MUTATION_REPAIR ... readers=5 maps=2 direct=0 inertDirect=2 preservedDisabled=6`
- `INPUT_MUTATION_READERS restored=5 handedToBootHold=0`
- `INPUT_MUTATION_REPAIR_OK attempt=1`

Meaning: the repair correctly identifies the two empty embedded left-turn actions and leaves them disabled, but after the provider is re-enabled, XRI still calls `ReadValue` on the existing non-null `leftHandSnapTurnAction.action`. Disabled is not sufficient for an embedded zero-binding action.

## Proven scene configuration

`_Boot.unity` deliberately uses right-stick-only turn controls:

- `SmoothTurn.m_LeftHandTurnAction` is an embedded direct `Left Hand Turn` action with an empty binding list.
- `SmoothTurn.m_RightHandTurnAction` is a real action reference.
- The XR-origin prefab override sets `m_LeftHandSnapTurnAction.m_UseReference = 0`.
- Runtime diagnostics prove the resulting `Left Hand Snap Turn` direct action has zero bindings and zero controls.

These are intentional inert placeholders, not bindings that can be re-resolved.

## Recommended next edit — minimal and bounded

While the readers remain suspended, normalize the two intentionally empty left-turn properties themselves, not merely their action enabled state.

Preferred location: the canonical rig wiring/repair seam (`PlayerRigPersistence.EnsureXRIWiring()` or the current repair driver's preparation pass).

For each provider:

```csharp
InputAction action = provider.leftHand...Action.action;
if (action != null && action.actionMap == null && action.bindings.Count == 0)
    provider.leftHand...Action = default;
```

Apply to:

- `ActionBasedSnapTurnProvider.leftHandSnapTurnAction`
- `ActionBasedContinuousTurnProvider.leftHandTurnAction`

Keep the right-hand referenced turn actions unchanged.

The likely reason this works: XRI's null-safe action-property path can return zero, while a non-null embedded action with zero bindings enters `InputActionState.ReadValue` and crashes.

If the public properties are not writable in this XRI version, set the backing action properties through the narrowest available API/reflection seam. Do **not** bind left stick to turn, duplicate the right action, or broadly disable turning.

## Verification required before headset authorization

1. Run ordinary CI for the exact final source SHA:
   - Unity EditMode success;
   - patch-scenes + audit success.
2. Run the full recovery PlayMode route.
3. Rerun the same PlayMode workflow job on the **same SHA**.
4. Require **43/43 twice**, with zero:
   - `INPUT_MUTATION_REPAIR_FAIL`;
   - `INPUT_MUTATION_REPAIR_ABORT`;
   - `INPUT_MUTATION_SETTLE_FAIL`;
   - Input System/XRI exceptions.
5. Require repair logs to show attempt 1 and clean reader restoration.
6. Run Golden Android for the same source SHA and inspect the artifact.
7. Only then build/install and repeat Terry's headset route for weapon scale/angle, coupler reach/selectability, and travel/input stability.

## Current durable gate records are stale or red

- `docs/recovery/generated/recovery_playmode_counts.json` currently records `f91c098` as **42/43**.
- `docs/CI_VERDICT.md` still records green for older source `25a8136`; it is stale for `f91c098`.
- `docs/recovery/generated/recovery_golden_android_observation.md` still records the old compile-broken `b62ce84` Golden Android failure; there is no authorized current APK.

## Important files

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/InputMutationRepairDriver.cs.meta`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs`
- `Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs`
- `Ziptide/Assets/Ziptide/Scenes/_Boot.unity`
- `Ziptide/Assets/Ziptide/Tests/EditMode/QuestWeaponAndCouplerRegressionTests.cs`
- `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryGoldenPerformanceRouteTests.cs`
- `.github/workflows/recovery-playmode.yml`
- `.github/workflows/recovery-golden-android.yml`

## Relevant Actions evidence

- Timing-sensitive candidate run/rerun: `29784865930` (`25a8136`)
- Current failed source run: `29785765784` (`f91c098`)

## Heads-up

The current driver uses reflection to access private `PlayerRigPersistence` mutation/boot-hold fields. It passed static ownership audits, but it is architectural debt. The fastest safe close is the two-property inert-action normalization above. After the checkpoint is stable, Fable 5 may fold the bounded repair directly into `PlayerRigPersistence` to remove the reflection seam, but do not expand scope before achieving deterministic 43/43 twice.

## Stop state

GPT stopped immediately after extracting the `f91c098` artifact and proving that XRI reads the non-null, zero-binding left snap-turn property after reader restoration. No additional runtime code changes were made after that finding.
