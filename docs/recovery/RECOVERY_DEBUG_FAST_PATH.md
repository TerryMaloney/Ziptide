# Recovery debugging fast path — evidence before iteration

Use this procedure for slow Unity, XR, input, travel, persistence, build, or device-recovery blockers. It exists because the July 2026 input blocker reached the correct diagnosis and even documented the correct minimal edit, but still spent multiple source candidates and hours exploring nearby fixes before applying it.

## The case that created this rule

The failing route eventually named one exact property:

- owner: `ActionBasedSnapTurnProvider.leftHandSnapTurnAction`;
- action: embedded direct `Left Hand Snap Turn`;
- bindings: `0`;
- controls: `0`;
- exception: `InputActionState.ApplyProcessors` during `ReadValue<Vector2>()`.

The incomplete fix kept the non-null empty action disabled. That could not survive the next lifecycle step: restoring the provider ran XRI `OnEnable -> EnableAllDirectActions`, which enabled it again. The durable fix was representational, not another timing or state toggle: replace the zero-binding, reference-free property with `default(InputActionProperty)`. A null property is skipped by XRI and cannot be revived by a later `OnEnable`.

## Mandatory fast path

1. **Read before changing.** Read the newest `docs/HANDOFF.md` entries and any named blocker handoff. Search for `Recommended next edit`, `Exact remaining failure`, `Stop state`, and rejected alternatives.
2. **Pin the evidence.** Record the exact source SHA, workflow/run ID, failing test, first relevant exception, and the smallest named owner/property. Do not reason from an older generated verdict.
3. **Inspect the full artifact once.** Download the NUnit XML and Unity log. Extract the event sequence around the failure, not only the final stack trace.
4. **Audit the lifecycle that runs after the proposed fix.** Before changing enabled/disabled state, inspect every later owner that can undo it: property setters, `Awake`, `OnEnable`, `OnDisable`, restore paths, scene-load wiring, boot holds, travel completion, and provider reactivation.
5. **Prefer a durable representation.** If a later lifecycle can reverse a state toggle, that toggle is not a fix. Remove or normalize the invalid state at its owning property/data seam. For XRI specifically: a deliberately unused embedded direct action with zero bindings must be a default/null `InputActionProperty`, not a disabled non-null action.
6. **Evidence lock.** Once the artifact names the exact owner/property and the current handoff contains a bounded recommended edit, the next source commit must either:
   - implement that edit as written; or
   - document concrete evidence proving it cannot work.

   Do not introduce a new bootstrap, owner, reflection layer, retry loop, timing delay, or architecture redesign while the bounded edit remains untested.
7. **One hypothesis per source commit.** Diagnostics may accompany it, but avoid stacking multiple behavior changes into one run.
8. **Count every expensive red.** A failed recovery PlayMode route or Golden Android route counts toward the circuit breaker exactly like ordinary CI. After **two failed source candidates** on the same blocker, stop speculative editing and re-enter this document at step 1. A third candidate is allowed only when the latest artifact names a materially new mechanism.
9. **Prove determinism on the exact source SHA.** Required order for timing-sensitive XR/input recovery:
   - ordinary CI green: compile, EditMode, patch-scenes, audit;
   - full recovery PlayMode route 43/43;
   - same-SHA rerun 43/43;
   - zero repair/settle failures and zero Input System/XRI exceptions;
   - Golden Android success and clean package proof;
   - only then authorize the headset route.

## Required blocker handoff fields

A recovery handoff is incomplete unless it contains:

- exact source SHA and run IDs;
- exact failing test and exception sequence;
- named owner, property/action, bindings/controls/state;
- lifecycle owner that runs after the attempted fix;
- minimal recommended next edit;
- alternatives already disproven;
- stop conditions and exact verification ladder.

## Tooling note

A writable local checkout with `git`, `gh`, and access to the Unity test/build runner would shorten patching and live Actions inspection. It is helpful, but it would not have solved this incident by itself: the decisive edit was already present in the handoff. The primary ratchet is evidence-locked execution and lifecycle analysis, not more retries.
