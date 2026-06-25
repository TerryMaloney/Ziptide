# VR / Rig Gotchas — hard-won fixes (READ before touching the XR rig, weapons, or drones)

**T-Dog lane.** These are bugs that recurred for *multiple rounds* because the obvious fix was wrong.
Each entry: symptom → real root cause → the fix that actually works (file + exact API). Don't re-derive
these — it cost real device cycles. All the runtime rig fixes live in
`Gameplay/Runtime/Player/PlayerRigPersistence.cs → EnsureXRIWiring()`, which runs on **every scene load**,
so a fix there sticks across travel (edit-time patches do NOT — see #0).

## #0 — Edit-time SerializedObject tuning does NOT reach the live rig
`Editor/Setup/EnsureLocomotionRig.cs` sets rig fields via `SerializedObject` at edit time. On device those
changes frequently **don't take** (prefab/instantiation/travel reset). **Rule: tune the rig at RUNTIME in
`PlayerRigPersistence.EnsureXRIWiring()`**, iterating `GetComponentsInChildren<XRBaseInteractor>(true)` /
move+turn providers. That method is the single source of truth for live-rig config.

## #1 — Thumbstick ROTATES the held gun instead of turning you
- **Root cause:** XRI ray *anchor control* (`XRRayInteractor`). **XRI 2.5.4 has NO public
  `enableAnchorControl`** — using it is a CS1061 compile error (CI caught it).
- **Fix (shipped):** set the serialized field `m_EnableAnchorControl=false` by **cached reflection** at
  runtime (`PlayerRigPersistence.DisableAnchorControl`). It's a `[SerializeField]`, so it survives IL2CPP
  stripping. Reflection on an XRI serialized field is the sanctioned exception to the "no reflection" rule
  (that rule is about runtime *item creation*; there's simply no public API here).

## #2 — Interactor ray looks WAY too long
- **Root cause:** the *visible* line is `XRInteractorLineVisual`, **not** `XRRayInteractor.maxRaycastDistance`.
  Shortening maxRaycastDistance changes hit distance but not what you SEE.
- **Fix (shipped):** per ray, `lineVisual.overrideInteractorLineLength = true; lineVisual.lineLength = 2.5f;`
  (public API, fine to call directly). Keep maxRaycastDistance in sync for hit reach.

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

## Where these fixes live (so you extend, not re-add)
- Rig input/rays/anchor: `PlayerRigPersistence.EnsureXRIWiring()` + `DisableAnchorControl()`.
- Gun physics: `ItemFactory.RestorePhysicsOnRelease()`.
- Drone/projectile collision: `DroneCombatBehavior.CollideMove()`, `StunBolt.Update()`, `DroneCombatState`.
- Spawn correctness (roomscale head-align + ground-snap): `PlayerRigPersistence.TeleportToMarker()`.
- Locomotion rig authoring (edit-time, but remember #0): `Editor/Setup/EnsureLocomotionRig.cs`.
