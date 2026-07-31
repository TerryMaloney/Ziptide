# VR / Rig Gotchas — hard-won fixes (READ before touching the XR rig, weapons, or drones)

**The operator's rig/weapons/drone notes.** These are bugs that recurred for *multiple rounds* because the obvious fix was wrong.
Each entry: symptom → real root cause → the fix that actually works (file + exact API). Don't re-derive
these — it cost real device cycles. All the runtime rig fixes live in
`Gameplay/Runtime/Player/PlayerRigPersistence.cs → EnsureXRIWiring()`, which runs on **every scene load**,
so a fix there sticks across travel (edit-time patches do NOT — see #0).

## #0 — Edit-time SerializedObject tuning does NOT reach the live rig
`Editor/Setup/EnsureLocomotionRig.cs` sets rig fields via `SerializedObject` at edit time. On device those
changes frequently **don't take** (prefab/instantiation/travel reset). **Rule: tune the rig at RUNTIME in
`PlayerRigPersistence.EnsureXRIWiring()`**, iterating `GetComponentsInChildren<XRBaseInteractor>(true)` /
move+turn providers. That method is the single source of truth for live-rig config.

## #1 — Thumbstick ROTATES/translates the held gun (or hammer) instead of turning you ⭐ THE PERSISTENT ONE
- **Symptom:** survived MULTIPLE "fixes" — the right stick kept rotating/moving the held object closer/farther.
- **REAL root cause (found 2026-06-28):** the disable code reflected the field **`m_EnableAnchorControl`** —
  **a name that does not exist** on `XRRayInteractor`. `GetField`/`FindProperty` returned `null`, so the
  anchor gate was **never actually set**; the reflection silently no-oped every single round. The actual
  serialized field is **`m_AllowAnchorControl`** (confirm it yourself: it's `m_AllowAnchorControl: 1` in the
  XRI 2.5.4 `Ray Interactor.prefab` / `Teleport Interactor.prefab` / `Gaze Interactor.prefab`). There is no
  public `enableAnchorControl` *or* `allowAnchorControl`-safe symbol we can compile-verify (no package source
  — see #8), so reflection on the **confirmed** field name is the right tool.
- **Fix (shipped):** two layers, both in `EnsureXRIWiring()`:
  1. `DisableAnchorControl(ray)` sets **`m_AllowAnchorControl=false`** by cached reflection on **every**
     `XRRayInteractor` (incl. the inactive teleport ray each hand owns). It logs `ANCHOR_FIELD_MISSING` if the
     name ever drifts again — so a future regression is **visible in logcat instead of silent**.
  2. `DisableAnchorInputActions()` hard-`.Disable()`s the **"Rotate Anchor" / "Translate Anchor"** input
     actions (after `EnsurePersistentInputActions()` enables the asset), so even if the gate is ever re-opened
     no stick input reaches anchor manipulation. Logs `ANCHOR_ACTIONS_DISABLED count=2`.
- **Lesson:** silent reflection (`GetField` returning null) is a trap — always log when the field/action isn't
  found, or a no-op masquerades as a fix for rounds. `EnsureLocomotionRig.TuneRayInteractors` also had the
  wrong name; it now lists `m_AllowAnchorControl` first.

## #2 — Interactor ray looks WAY too long, or JUMPS long↔short when you point at things
- **Root cause:** the *visible* line is `XRInteractorLineVisual`, **not** `XRRayInteractor.maxRaycastDistance`.
  Shortening maxRaycastDistance changes hit distance but not what you SEE. The **jump** is a second trap:
  each hand has **TWO** ray interactors (select + teleport, swapped by `ActionBasedControllerManager`), each
  with its **own** line visual — clamping only the active one leaves the other long, so it flickers on swap.
- **Fix (shipped):** iterate **`GetComponentsInChildren<XRInteractorLineVisual>(true)`** (ALL of them, incl.
  inactive teleport rays) and set `overrideInteractorLineLength = true; lineLength = 2.5f;` (public API, fine
  to call directly). Keep maxRaycastDistance in sync for hit reach.

## #3 — Right thumbstick MOVES you (fwd/back) instead of only turning
- **Root cause:** `EnsureLocomotionRig` binds the Move action to **both** hands; the turn providers are
  correctly on the right hand, so the right stick does both.
- **Fix (shipped):** at runtime, `moveProvider.rightHandMoveAction = default;` on every
  `ActionBasedContinuousMoveProvider` → left stick moves, right stick turns. (`default` = empty
  `InputActionProperty`; the provider reads it null-safe.)

## #4 — A grabbed gun FLOATS / freezes when released (esp. after holster-travel)
- **Root cause:** the holster sets `Rigidbody.isKinematic=true` for transport; on release `XRGrabInteractable`
  "restores" the RB to that cached kinematic state → it hangs in mid-air.
- **Fix (shipped):** `ItemFactory.RestorePhysicsOnRelease` adds a `selectExited` listener that forces
  `isKinematic=false; useGravity=true` (runs after XRGrab's restore, so it wins; holstering is unaffected
  because the socket re-sets kinematic right after it grabs).

## #5 — Drones (or projectiles) PHASE THROUGH building walls
- **Root cause:** `DroneCombatBehavior`/`StunBolt` move by `transform` with **no collider/Rigidbody**, so
  they pass straight through geometry.
- **Fix (shipped):** `DroneCombatBehavior.CollideMove` spherecasts each step and clamps at the nearest wall
  (ignoring the player rig + other drones via `GetComponentInParent<DroneRuntime>()` / an "XR Origin"
  ancestor walk). `StunBolt` raycasts its travel and is absorbed by any wall it crosses. Also: a combat
  drone cancels its telegraphed shot if it loses line-of-sight (no shooting through cover), and is
  **leashed** to its home zone. Apply this same pattern to any new transform-driven `CreatureBehavior`.

## #6 — An object can't be grabbed (XRGrabInteractable)
- **Root cause:** the grab **collider must exist BEFORE `XRGrabInteractable` initializes**. If a component's
  `Awake` adds the collider after RequireComponent already created the interactable, the interactable
  gathers an empty collider list → ungrabbable. (This is why the PvP hammer wasn't grabbable.)
- **Fix (shipped):** in the patcher, add the `BoxCollider` (+ `Rigidbody`) to the GameObject **before**
  `AddComponent<HammerTool>()` (which pulls in the interactable). Primitive-based items (Cube) already have
  a collider, so `ItemFactory.Create*` is fine; hand-built GOs are the trap.

## #7 — Patcher-built content invisible at edit-time / in the audit
Components that build their visuals in `Awake` (drones, hammer, wrist scanner, breakable walls) only
populate at **runtime**, so the saved scene + `WorldAuditRunner` see an "empty" object. That's expected —
don't "fix" it by moving construction to edit-time. The audit only cares about spawn-on-solid, the city
root, travel doors, and no-XR-Origin-in-world-scenes.

## #8 — "No package source in the cloud"
The cloud container has the XRI **DLLs but not source**, so you can't read XRI member signatures. When a
public API name is uncertain (see #1), either (a) reflect a known `[SerializeField]` field, or (b) just
push and let CI's compile catch a wrong name fast — cheaper than guessing in prose.

- **But do NOT extend that to package *behaviour*.** A wrong name costs one compile; a wrong assumption
  about a package type's **default state** costs a red test cycle and reads as a code bug. In particular,
  **never assert on a count derived from a default you cannot read** — assert on the property the feature
  guarantees ("this hand's action is null", "the real stick survived"). If a count really is the contract,
  pin the default in its own named test so the number has a stated source. (MISS_LEDGER #23: a freshly
  added XRI provider has **two** zero-binding embedded actions, not zero — Unity's serializer instantiates
  the `[SerializeField] InputAction` — and two separate tests of mine assumed otherwise.)

## #9 — A zero-binding direct XRI action still crashes after it was disabled
- **Symptom:** the recovery route logs a successful action-map repair and reader restoration, then one frame
  later throws `NullReferenceException` from `InputActionState.ApplyProcessors` through
  `ActionBasedSnapTurnProvider.ReadInput()`. The action reports `bindings=0`, `controls=0`, and no active
  control. Repeated disable/enable timing changes may appear to fix it for one run and then fail the same-SHA
  rerun.
- **REAL root cause (proved 2026-07-20):** a deliberately unused embedded direct `InputAction` remained
  **non-null**. Keeping it disabled is not durable. Re-enabling its suspended XRI provider runs
  `OnEnable -> EnableAllDirectActions`, which enables the empty action again; the next provider read enters
  the Input System with no binding state and crashes. The lifecycle after the "fix" undoes the fix.
- **Fix (shipped):** `InputMutationRepairDriver.ClearInertDirectProperties()` runs before action repair. For
  move, continuous-turn, and snap-turn providers, any action property with **no reference + direct action +
  zero bindings** is assigned `default(InputActionProperty)`. XRI's property setter disables the outgoing
  action, later `OnEnable` has nothing to revive, and `ReadInput` null-skips the property. Log:
  `ZIPTIDE: INPUT_MUTATION_INERT_CLEARED count=...`.
- **The half that was missing (found 2026-07-31):** the mutation above was correct but reachable from ONE
  trigger — `InputMutationRepairDriver.Update()` returns early until it has seen a travel. Cold boot, and
  any PlayMode test that only loads a scene, never swept, so `_Boot`'s left-hand turn/snap placeholders
  stayed enabled and the same `ApplyProcessors` NRE kept landing intermittently on whichever test read
  first. Fixed by extracting `LocomotionInertActionSweep` (one implementation) and running it from the
  driver's `OnEnable` **and** every `sceneLoaded`, as well as the post-travel repair. Log now carries
  `reason=installed|scene_loaded:<name>|post_travel_repair`.
- **Hard rule:** for an intentionally unused hand/action, **null/default property beats disabled non-null
  action** — and the repair must run on *every* path that can reach the read, not just the one where the
  bug was first seen. Before accepting any input-state fix, inspect the later provider lifecycle (`OnEnable`, restore,
  boot hold, scene load, travel rewire). If that lifecycle can reverse the state toggle, normalize the owned
  property/data instead.
- **Verification rule:** timing-sensitive input recovery is not accepted after one green route. Require
  ordinary CI green, recovery PlayMode 43/43, the same-SHA rerun 43/43, zero Input System/XRI exceptions,
  then Golden Android success. See `docs/recovery/RECOVERY_DEBUG_FAST_PATH.md`.

## Where these fixes live (so you extend, not re-add)
- Rig input/rays/anchor: `PlayerRigPersistence.EnsureXRIWiring()` + `DisableAnchorControl()` (m_AllowAnchorControl)
  + `DisableAnchorInputActions()`.
- Input mutation/inert actions: `InputMutationRepairDriver.ClearInertDirectProperties()`; evidence and
  escalation rules: `docs/recovery/RECOVERY_DEBUG_FAST_PATH.md`.
- Gun physics: `ItemFactory.RestorePhysicsOnRelease()`.
- Drone/projectile collision: `DroneCombatBehavior.CollideMove()`, `StunBolt.Update()`, `DroneCombatState`.
- Spawn correctness (roomscale head-align + ground-snap): `PlayerRigPersistence.TeleportToMarker()`.
- Locomotion rig authoring (edit-time, but remember #0): `Editor/Setup/EnsureLocomotionRig.cs`.
