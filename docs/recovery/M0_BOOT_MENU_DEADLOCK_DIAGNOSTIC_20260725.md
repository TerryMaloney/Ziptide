# M0 BOOT MENU DEADLOCK — PRODUCT + VERIFICATION DIAGNOSTIC

**Date:** 2026-07-25  
**Status:** diagnostics only; no code, scene, prefab, build, APK, or test implementation changes authorized by this document.  
**Observed device result:** M0 blocked before gameplay because the player could neither locomote to nor interact with the Home Hub menu.  
**RED-CAUSE:** `mixed` — product integration deadlock plus verification-net false confidence.  
**Current implementation owner:** T-Dog / recovery lane for the narrowly authorized Home Hub fix. This report does not compete with or replace that owner.

## 1. Executive finding

This was not merely a menu-position tuning error.

The boot scene entered a globally unwinnable state produced by several individually intentional or apparently healthy subsystems:

1. `_Boot` has no floor.
2. `BootLoader` deliberately enabled `BOOT_HOLD` so the rig could not fall or move.
3. the Home Hub created its tiles and bound them to an `XRInteractionManager`;
4. the live headset had zero active ray interactors;
5. the nearest required tile was outside comfortable direct reach;
6. no independent fallback interaction path existed;
7. `HOME_HUB_READY` was nevertheless published;
8. CI had already observed the same zero-ray held-boot state but did not classify it as a blocker.

The result was a **boot liveness deadlock**:

```text
BOOT_HOLD active
+ no locomotion
+ no floor/path
+ zero active ray interactors
+ required target outside direct reach
+ no fallback input
= no available action that can advance the game
```

The proper invariant is not “menu exists” or “tile is bound.” It is:

> While the player is held in `_Boot`, at least one complete, currently usable escape path must exist from the player's actual tracked pose to a choice that advances or safely configures the game.

## 2. Device evidence currently available

Terry's screenshots show the headset log query returning:

```text
BOOT_HOLD on pose=(0.00,0.10,0.00) suspended=2
```

with no corresponding release line during the blocked session.

The screenshots also record:

- locomotion components present and configured;
- `_Boot` spawn at approximately `(0, 0.10, 0)` with no ground below;
- `NO_RAY_INTERACTORS`;
- Home Hub tiles built and bound;
- `HOME_HUB_READY continue=false`;
- player unable to walk to or physically reach the menu.

The full Quest log has not yet been inspected by this diagnostic. It is still required to determine whether `NO_RAY_INTERACTORS` persisted for the entire session and whether there were additional warnings or lifecycle failures.

## 3. Product-side causal chain

### 3.1 Boot hold was intentional and behaved as authored

`BootLoader.Start()` arms `PlayerRigPersistence.SetBootHold(true)` before presenting the Home Hub because `_Boot` has no floor. The code states that locomotion, fall recovery, and drift must remain suspended until a content-world spawn settles.

`PlayerRigPersistence.ApplyBootHold(true)` disables enabled continuous-move, continuous-turn, snap-turn, and dash providers, records them, and pins the rig. Its release path normally occurs only after travel and `TeleportToMarker()` in a content scene.

Therefore, the lack of locomotion was not evidence that the movement provider itself was broken. It was the expected result of boot ownership.

### 3.2 Home Hub readiness meant “constructed,” not “actionable”

`HomeHubRuntime.Start()`:

1. resolves Continue state;
2. builds the surface;
3. logs `HOME_HUB_READY`;
4. publishes `BootPresentationReady`.

It does not wait for or prove:

- an active ray/direct/poke interactor;
- a bound controller action capable of selecting;
- a target inside direct-hand reach;
- a usable fallback input;
- a complete escape path while locomotion is held.

The name `HOME_HUB_READY` therefore overstates what it proves. It currently means roughly `objects_created_and_flow_initialized`.

### 3.3 Board placement assumed a ray modality

The current runtime surface is placed from a one-time `Camera.main` snapshot approximately 2.2 meters forward, with tiles additionally offset on the board.

That can be reasonable for a ray-operated board. It is not a safe direct-touch-only fallback distance while the player's rig is pinned.

A valid placement system must use an explicit interaction contract:

- ray-required board with proven active ray;
- direct-touch board inside measured reach;
- or adaptive/fallback placement based on the modalities actually live.

### 3.4 Zero rays was logged but not treated as a contextual blocker

`PlayerRigPersistence.EnsureXRIWiring()`:

- iterates active interactors;
- skips inactive hierarchies;
- counts active rays;
- logs `NO_RAY_INTERACTORS` when none are available.

It does not activate an intended menu ray, provide a direct fallback, or fail boot readiness.

A zero-ray state is not always globally fatal. It becomes fatal in this exact context:

```text
scene=_Boot
boot_hold=true
floor/path=false
home_hub_ready=true
active_rays=0
reachable_direct_choices=0
fallback_choices=0
```

The verification system lacked this contextual composition rule.

### 3.5 Manager binding did not prove player access

The Home Hub continuously repairs each tile's `XRInteractionManager` binding. That protects against manager replacement, but it proves only that the interactable knows a manager.

It does not prove that any active interactor:

- belongs to that manager;
- can hit the tile from the real tracked pose;
- has a compatible interaction layer/mask;
- has a valid Select action bound to a live controller;
- can cause an actual selection event.

## 4. Verification-net failure

### 4.1 The R1 specification asked for the right kind of proof

The locked R1 integration-harness design required the actual `_Boot` scene, boot hold, live manager binding, and selection through a tracked-rig interaction path rather than direct calls to private Home Hub methods.

It also stated that R1 should catch obvious runtime/UI failures before Terry wears the Quest.

The implementation, however, mutated the initial state before proving its liveness.

### 4.2 Exact green artifact already recorded the fatal precursor

The exact `c45b1a2` PlayMode result artifact from run `29786603998` logged the relevant sequence before test intervention:

```text
ZIPTIDE: XRI_WIRING ... rays=0(active=0) ...
ZIPTIDE: NO_RAY_INTERACTORS
ZIPTIDE: BOOT_HOLD on pose=(0.00,0.10,0.00) suspended=2
ZIPTIDE: HOME_HUB_READY continue=false
ZIPTIDE: BOARD_PROBE surface=HomeHub phase=aim ray=none ...
```

The test later logged `RECOVERY_TRACKED_RIG_SIM` after it installed/reused virtual controller devices and force-activated the actual rig's ray hierarchy.

This means the test pipeline had direct evidence of the dangerous initial state but treated it as acceptable setup rather than a blocker.

### 4.3 The synthetic controller helper masked the production failure

`RecoveryActualRigControllerSimulation.Activate()` deliberately:

- disables production `XRInputModalityManager` behavior;
- installs or reuses virtual XR controllers;
- sets a tracked head pose;
- activates controller-ray GameObject hierarchies;
- enables controller/interactor/line behaviors;
- assigns the canonical manager.

This is useful for a **synthetic XRI plumbing test**. It is not valid evidence that the production initial modality and ray activation state is live.

The test should have first asserted production-state boot liveness without modification. Synthetic activation should be a separate test phase and a separately named evidence class.

### 4.4 The test moved the ray instead of testing from the actual pose

The boot smoke and round-trip tests place the test ray approximately 1.1 meters from the selected tile, point it directly at the target, perform a raw `Physics.Raycast`, and then directly invoke manager hover/select operations.

That bypasses several failure surfaces:

- actual controller pose and roomscale offset;
- menu distance from tracked head/hands;
- natural ray aim geometry;
- production modality activation;
- Select input binding and trigger actuation;
- controller/interactor state-machine behavior;
- direct-touch fallback;
- reach accessibility for seated/young players.

The test proves that a tile collider and callback can work when an artificial ray is placed favorably and XRI selection is invoked. It does not prove that Terry can perform the first action after launch.

### 4.5 Existing EditMode tests were contract-text tests, not liveness tests

Current Home Hub EditMode coverage largely proves:

- choice-state semantics;
- exactly-once travel intent;
- Continue gating;
- source ownership and delegation strings.

Those are useful, but they do not instantiate the combined boot state or evaluate reach and modality.

### 4.6 UI readability audit measured the wrong layer

The current UI audit checks:

- text scale;
- collider presence;
- target-face size;
- label-to-collider separation.

It is warning-only and runs over authored scene objects. The Home Hub is created at runtime, so the build-scene audit cannot reliably evaluate its final runtime placement.

Even if it could see the board, it does not test:

- distance from tracked player pose;
- direct reach envelope;
- active interaction modalities;
- valid manager/mask/action path;
- whether all required controls can be selected;
- whether a held player has any escape action.

The exact visual snapshot could look centered and legible while the game remained unusable.

## 5. Root system problem

The current recovery system is strong at **component ownership** and **local contracts**, but weaker at **cross-system liveness invariants**.

This failure crossed at least five owners:

- boot-flow owner;
- persistent rig/boot-hold owner;
- input-modality/ray owner;
- runtime Home Hub owner;
- verification/audit owner.

Each subsystem satisfied a local claim:

- hold protected the no-floor boot;
- mover existed;
- menu existed;
- tiles had colliders and manager bindings;
- the synthetic XRI test could select them.

No owner proved the global user goal:

> From the actual launch state, can a real player execute the first required verb without developer intervention?

That missing goal-level proof is why a green pipeline produced a headset-blocking first-screen failure.

## 6. Binding diagnostic invariant: BOOT_LIVENESS

A candidate must be blocked before APK delivery when boot hold is active and no complete escape path exists.

### 6.1 At least one valid escape path is required

Any one of these may satisfy the invariant:

1. **Ray path**
   - active ray interactor in active hierarchy;
   - canonical interaction-manager binding;
   - compatible interaction layers/masks;
   - enabled/selectable input action;
   - required choice within ray range from actual controller pose;
   - real trigger/action selection succeeds.

2. **Direct/poke path**
   - active direct or poke interactor;
   - required choice within measured standing/seated/young-child reach envelope;
   - no collision/occlusion preventing contact;
   - real selection succeeds.

3. **Locomotion path**
   - locomotion actually enabled;
   - safe floor and navigable route exist;
   - player can reach the target without falling or leaving bounds.

4. **Explicit fallback path**
   - documented controller/button/gaze fallback;
   - independently usable in boot state;
   - does not consume gameplay buttons unexpectedly;
   - visible/spoken instructions exist.

If all four counts are zero, emit a blocking `BOOT_DEADLOCK` finding.

### 6.2 Required contextual fatal combination

At minimum, this combination must never remain warning-only:

```text
BOOT_HOLD=on
HOME_HUB_READY=true
active_ray_count=0
reachable_direct_choice_count=0
locomotion_escape=false
fallback_escape=false
```

## 7. Required evidence split

Future reports must separate these evidence classes:

### A. Production-initial-state liveness

No test harness may:

- force-enable modality managers or rays;
- reposition controllers near targets;
- move the menu;
- inject a favorable fallback;
- directly invoke `HoverEnter` or `SelectEnter`.

This phase answers: **would the unmodified runtime let the player proceed?**

### B. Synthetic XRI plumbing

Virtual controllers and forced ray activation may be used here, but the result must be named honestly. It proves collider/manager/callback plumbing, not production modality readiness.

### C. Quest hardware proof

Quest remains responsible for tracking feel, controller modality timing, ergonomics, physical reach, and final selection feel. It must not be the first place a deterministic zero-escape state is discovered.

## 8. Required pre-headset boot matrix

The boot liveness test should eventually cover at least:

| Case | Required result |
|---|---|
| standing adult head height | New Game and Settings usable |
| seated adult | New Game and Settings usable without standing |
| young-child height/reach proxy | required choices usable |
| non-zero roomscale X/Z head offset | board/controls remain accessible |
| controller modality available immediately | ray/direct path works |
| controller modality activates late | boot waits or fallback remains usable |
| zero active rays | direct/fallback path exists or candidate blocks |
| Continue unavailable | New Game and Settings remain usable |
| Continue available | all three choices remain usable |
| manager replacement during boot | tiles and active interactor converge on canonical manager |
| overlay/resume while waiting | escape path remains available |
| repeated launch/same install | no stale hold, menu, or modality state |

## 9. Runtime census additions

The settled-boot census should include:

- boot-hold state and suspended provider list;
- floor/path result under rig and under target route;
- all ray/direct/poke interactors, including inactive ones;
- `activeSelf`, `activeInHierarchy`, `enabled`, manager ID, interaction masks, max distance;
- Select action/reference/enabled/control count for intended boot interactors;
- locomotion provider enabled states;
- tracked head and controller world poses;
- each required tile's closest surface point;
- distance from head and both controller poses;
- direct-reach result under standing/seated/child envelopes;
- ray line-of-sight and range from actual pose;
- number and type of valid escape paths;
- final `BOOT_LIVENESS PASS/BLOCKER` result.

A count of ray components somewhere in an inactive hierarchy is not an active path.

## 10. Adjacent high-risk surfaces to audit before the next long headset session

### P0 — Home Hub after T-Dog's fix

The fix must prove more than a shorter distance. Verify:

- actual production ray state;
- direct reach from standing, seated, and young-child proxy;
- New Game and Settings both selectable;
- Continue variant when present;
- boot hold remains safe;
- board does not intersect head/hands;
- no regression from roomscale offset;
- selection uses real action path, not manager injection.

A placement-only fix may allow this screen to pass while leaving the zero-ray root unresolved for later ray-dependent interactions.

### P0 — boot Settings / ComfortConsoleRuntime

The Settings choice creates another runtime-built board with fixed local geometry and only one-time manager binding. It should receive the same liveness/reach test:

- all comfort tiles reachable/selectable;
- settings panel positioned relative to actual tracked pose;
- manager replacement does not strand it;
- boot hold remains intentional while settings is open;
- player can close/confirm/return without locomotion.

### P0 — next required W000 action

After New Game, identify the **first required verb** in W000 and prove it from settled spawn before handing over an APK. The next blocker must not simply move one screen deeper.

### P0 — active-ray persistence after travel

The Quest log should be checked for whether `NO_RAY_INTERACTORS` continues after W000/ToxicCity loads. If the menu fix depends only on direct reach, later ray-only controls may still be inaccessible.

### P1 — coupler and `PRESS POWER`

Existing device checklist already asks about stage clarity and child reach. Add cross-system liveness:

- required interactor actually active;
- target inside direct/ray envelope;
- prompt stage corresponds to selectable object;
- release does not strand the part;
- no required button is above seated/child reach;
- invalid stage still leaves a recoverable action.

### P1 — `PUNCH IT` and travel controls

Prove:

- prompt has an active interaction path;
- travel commit is exactly once;
- input/locomotion restore after arrival;
- no all-controls-disabled transition window persists;
- one safe fallback exists if a transition callback fails.

### P1 — every runtime-created menu or machine

`HomeHubRuntime` and `ComfortConsoleRuntime` reveal a general risk class: code-built world-space UI can bypass scene-time spatial audits.

Inventory and preflight:

- runtime Home Hub;
- comfort/settings consoles;
- dev menu/warp board where allowed;
- machine stations;
- invention/recipe boards;
- coupler prompts;
- travel/helm controls;
- any future narrated child-readable menu.

Every required runtime surface needs a registered reach/modality contract.

### P2 — post-travel and lifecycle escape paths

Audit states where one system deliberately suppresses controls:

- boot hold;
- input mutation settle;
- travel transition;
- overlay/focus loss;
- stun/disable effects;
- comfort transition;
- dev/menu mode;
- machine commit/interlock.

For each suppression owner, require:

- explicit start log;
- explicit release log;
- timeout/fail-closed behavior;
- ownership identity;
- at least one recovery path;
- test that later lifecycle code cannot silently reverse the intended state.

## 11. Process changes needed before scale-up

### 11.1 Add a “first actionable verb” delivery gate

Before Terry receives any APK, the pipeline must state and prove:

```text
Launch state:
First required player verb:
Actual tracked start pose:
Permitted interaction modalities:
Required target:
Reach/range proof:
Real input action proof:
Fallback path:
Automated evidence artifact:
Remaining Quest-only question:
```

The same pattern should apply after every mandatory transition in the bounded route.

### 11.2 Treat warnings according to context

`NO_RAY_INTERACTORS` alone may be informational in some worlds. In a held no-floor boot with a ray-positioned required menu, it is a blocker.

Logs and gates need relational rules, not only string counts.

### 11.3 Do not let a harness repair the state before observing it

A test helper may synthesize a modality only after it has captured and adjudicated the unmodified production state. Otherwise the helper can erase the exact defect being tested.

### 11.4 Grade green claims narrowly

Use labels such as:

- `FLOW_STATE_GREEN`;
- `SYNTHETIC_XRI_PLUMBING_GREEN`;
- `PRODUCTION_BOOT_LIVENESS_GREEN`;
- `QUEST_BOOT_ERGONOMICS_GREEN`.

Do not use one `PlayMode green` label to imply all four.

### 11.5 Add goal-level invariants to system ownership

Ownership inventories describe which system creates or mutates a component. Add user-goal contracts spanning owners:

- can start game;
- can reach first world;
- can recover from dropped required item;
- can exit every machine/menu state;
- can regain controls after every suppression state;
- can complete every mandatory first-hour verb at seated/child reach.

## 12. What T-Dog's current fix must not accidentally do

Based on the screenshots, T-Dog began a narrow HomeHub placement/solver change and EditMode tests before usage stopped. Before accepting or finishing it:

- inspect the uncommitted diff rather than reconstructing from screenshots;
- verify the change does not release boot hold while `_Boot` still has no floor;
- verify it does not merely place the board so close it clips into the camera/hands;
- verify it does not assume one fixed head height;
- verify it handles non-zero roomscale X/Z offsets;
- verify it does not hide `NO_RAY_INTERACTORS`;
- verify direct reach is a deliberate fallback, not an accidental side effect;
- verify Settings/Continue layouts use the same solver;
- add production-state liveness proof before synthetic ray proof;
- preserve the exact failure log and classify this as `RED-CAUSE: mixed`.

No competing code edit should be made until T-Dog's working tree/diff is recovered and ownership is explicit.

## 13. Quest log intake needed

The full Quest log is needed to answer:

- whether rays ever activated after boot;
- whether modality-manager or controller logs explain zero rays;
- whether boot hold emitted any second begin/release event;
- whether manager replacement or input asset recovery occurred;
- whether hidden exceptions preceded the blocked screen;
- whether other contextual blocker combinations already appear;
- whether lifecycle/focus events occurred during the session.

Preserve the original file rather than pasting selected lines only.

## 14. Immediate recommendation

1. Recover and review T-Dog's uncommitted diff; do not create a second fix in parallel.
2. Upload the complete Quest log.
3. Run this diagnostic against the log and classify all warnings by context.
4. Finish one narrow Home Hub candidate.
5. Require a cold PlayMode production-state `BOOT_LIVENESS` proof before any synthetic controller activation.
6. Require a rendered/reach plate for standing, seated, child, and roomscale-offset poses.
7. Build one newly named APK from the proven SHA.
8. At the next headset session, test only boot entry first; continue into the full M0 route only after the first actionable verb passes.

## 15. Stop conditions

This diagnostic does not authorize:

- direct edits to HomeHubRuntime, PlayerRigPersistence, BootLoader, modality managers, scenes, or tests;
- release of boot hold in `_Boot`;
- broad XR rig refactor;
- activation of growing, worlds, BioRefiner, conveyors, multiplayer, or other paused lanes;
- replacement APK;
- declaration that T-Dog's partial fix is correct before its diff and evidence are reviewed.
