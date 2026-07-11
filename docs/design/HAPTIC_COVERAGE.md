# ZIPTIDE — HAPTIC COVERAGE CHECKLIST

**Status:** documentation-level coverage gate, 2026-07-11  
**Runtime registry:** not built or approved  
**Purpose:** name every player-facing hand/body verb that needs tactile confirmation, record what actually exists, and prevent new interactions from shipping with silent hands.

> This document closes the missing **coverage checklist**, not the missing runtime feedback. A checked
> evidence row means the current owner was inspected; it does not mean Terry has approved the feel.
> Runtime changes remain separate, owner-scoped, CI-tested and device-yellow until headset verification.

## 1. Laws

1. **The interaction owner owns its haptic moment.** Do not add a second grab, holster, weapon, repair,
   traversal or UI state machine merely to vibrate a controller.
2. **Haptics never decide gameplay.** Missing controller/interactor hardware must degrade to silence while
   the action completes normally.
3. **Pulse the hand that did the work.** Use both hands only for a deliberately body-scale event such as
   the existing scanner pulse.
4. **No duplicate acknowledgement.** Before adding a pulse, verify XRI or another owner is not already
   producing the same confirmation on the same callback.
5. **Continuous feedback must be bounded.** Charge/drag/turn feedback may ramp, but must not issue an
   unbounded full-strength impulse every frame.
6. **Comfort scaling comes later through the locked device settings.** `COMFORT_AND_ACCESSIBILITY.md`
   reserves a haptic-scale control; this checklist does not invent its runtime implementation.
7. **Every runtime haptic task ships with a null-controller fallback, an owner/source test, a diagnostic,
   and Terry’s headset verdict.** CI can prove placement and bounds, not subjective feel.

## 2. Evidence legend

- ✅ **Explicit:** a project-owned haptic call and tuning fields are visible in the current owner.
- 🟡 **Partial:** some tactile response exists, but the full verb/state coverage or device verdict is open.
- ⬜ **No explicit project pulse in the inspected owner.** This is a claimable implementation row.
- ❓ **Not source-audited yet.** Never convert this to “missing” or “done” by assumption.

Generic platform/XRI behavior is not counted as a ZIPTIDE signature unless the project explicitly
configures and tests it.

## 3. Current evidence inventory

| Player verb / moment | Current owner or evidence | State | What is proven / what remains |
|---|---|---:|---|
| Wrist scanner charge | `Gameplay/Runtime/Pvp/WristScanner.cs` | ✅ | Left-hand ramp is explicit: `chargeHapticMax` up to `0.6`, `0.03s` impulses while charging. Device feel still needs the normal scanner pass. |
| Wrist scanner pulse | `WristScanner.Pulse()` | ✅ | Both hands receive `pulseHapticAmp=0.9`, `pulseHapticDur=0.12s`; this is the only inspected complete two-hand signature. |
| Generic item grab | XRI grab owners / `ItemFactory` | ❓ | No project-wide signature or coverage gate identified. Audit actual weapon/tool owners before adding anything; avoid doubling a package-level pulse. |
| True item release / throw | `Gameplay/Runtime/Items/ReleaseFeel.cs` | ⬜ | Owner restores throw velocity and flashes the renderer, but has no explicit haptic acknowledgement. |
| Holster insertion | `Gameplay/Runtime/Inventory/HolsterSocketInteractor.cs` | ⬜ | Accepted `selectEntered` owns the exact successful moment and first-hour event; no explicit pulse. Ideal first implementation seam. |
| Holster rejection / wrong item | `HolsterSocketInteractor.CanHover/CanSelect` | ⬜ | Rejection is silent. Do not pulse every failed hover; any future feedback needs debounce and device proof. |
| Belt/holster proximity lip | `Gameplay/Runtime/Inventory/BeltRig.cs` | ⬜ | Functional socket/marker exists; no tactile edge/proximity contract. Lower priority than successful insertion. |
| Diegetic tile selection | Home Hub, comfort console, helm, job/choice boards | ⬜ | Current `XRSimpleInteractable.selectEntered` surfaces have no common project confirmation. Start with successful selection only; do not change menu ownership. |
| Repair: pull access panel | `Gameplay/Runtime/Story/RepairableMachine.cs` | ⬜ | Stage transition and physical grab are explicit; no tactile stage confirmation. |
| Repair: part seats | `RepairableMachine.SeatPart()` | ⬜ | Exact magnetic-seat success moment exists; currently visual/state/log only. High-value tactile “clunk” row. |
| Repair: power switch / machine running | `RepairableMachine.OnSwitchFlipped()` | ⬜ | Exact completion moment exists; no explicit pulse. Keep machine/job ownership unchanged. |
| Zipline handle acquired / ride starts | `Gameplay/Runtime/World/ZiplineRuntime.BeginRide()` | ⬜ | Exact ride-start callback exists; no explicit grip/start confirmation. |
| Zipline arrival | `ZiplineRuntime.EndRide("arrived")` | ⬜ | Exact canonical arrival reason exists; no explicit completion pulse. Early release should not use the same signature. |
| Zipline early release | `ZiplineRuntime.EndRide("released")` | ⬜ | Separate owner reason exists; future feedback must remain distinct/subtle and never change first-hour completion. |
| Jump takeoff / landing | `Gameplay/Runtime/Locomotion/DashLocomotion` | ⬜ | Body state is explicit; no haptic call in inspected owner. Landing needs a reliable transition before any pulse is added. |
| Sprint / crouch / slide | `DashLocomotion` | ⬜ | State changes/logs exist; no tactile state change. Slide start is the highest-value of these; avoid continuous locomotion buzz. |
| Climb grip / release | climbing owner not yet source-audited in this pass | ❓ | Mandatory before the haptic runtime sprint closes because the Excellence Map names climb-grip as a required signature. |
| Weapon trigger/fire | weapon-specific runtime owners | ❓ | The playbook previously implied definition-level haptics, but current base `ItemDefinition` has no haptic fields. Audit each weapon runtime; do not claim data tuning exists. |
| Weapon charge / ready / dry-fire | weapon-specific runtime owners | ❓ | Needs per-weapon inventory. Distinct weapons should not all share an undifferentiated buzz. |
| Weapon/projectile impact | target/weapon owners | ❓ | Determine whether feedback belongs to firing hand, target visuals/audio only, or both; never infer a hit from haptic presentation. |
| Creature non-lethal disable | `CreatureRuntime` owner, FH-S05 pending | ❓ | Coordinate after Picasso FH-A01 and FH-S05; art tell and gameplay disable remain primary. |
| Build socket placement / purchase | build-socket owner | ❓ | Requires source audit; successful commit and insufficient-credit failure should not share a signature. |
| Garden plant/tend/harvest | garden runtime owners | ❓ | Requires source audit and headset feel. Pouring/continuous tending must be bounded. |
| Vehicle mount/dismount/boost | vehicle owners | ❓ | Requires source audit; cockpit/vehicle vibration must not become constant nausea-inducing noise. |
| Ship helm engage / boost / weapon | `Ziptide.Ship` owners | ❓ | Requires source audit with comfort/device pass; use the control hand, preserve world-moves/rig-static law. |
| Damage/stun received | player stun/combat owners | ❓ | Audit whether existing visual slow/flash has tactile feedback. Must respect haptic-scale accessibility. |
| Reward/job completion | job/economy presentation owner | ❓ | Lower priority; never make payout depend on haptics. |

## 4. Runtime implementation order

Each row below is a separate small owner-scoped task. Do not bundle all haptics into one risky cross-game commit.

### P0 — first-hour tactile river

- [ ] Successful item grab/release evidence and one non-duplicated signature per real hand action.
- [ ] Successful holster insertion.
- [ ] Successful diegetic tile selection for Home/comfort/helm/job/choice surfaces.
- [ ] Repair panel pulled, replacement part seated, power restored—three distinct moments.
- [ ] Weapon fire for the starter weapon set after each runtime owner is audited.
- [ ] Zipline grip/start and canonical arrival; early release remains distinct.
- [x] Scanner charge and pulse exist; Terry verifies feel.

### P1 — body and traversal

- [ ] Jump takeoff/landing transition.
- [ ] Slide start.
- [ ] Climb grip/release.
- [ ] Damage/stun received.
- [ ] Ship helm engage/boost/fire.
- [ ] Vehicle mount/dismount/boost.

### P2 — building, economy, and world interaction

- [ ] Build placement success/failure.
- [ ] Garden plant/tend/harvest.
- [ ] Machine/factory collection.
- [ ] Job/reward completion.
- [ ] Reactive-world interactions only where the player physically caused them.

## 5. Required proof for every runtime row

A runtime haptic row is not complete until it has:

- [ ] exact existing owner callback/state transition named;
- [ ] no duplicate state machine or input binding;
- [ ] controller/interactor-null fallback that leaves gameplay unchanged;
- [ ] amplitude/duration bounded and visible in data or named constants;
- [ ] source/behavior test proving the pulse occurs only at the intended transition;
- [ ] diagnostic such as `ZIPTIDE: HAPTIC verb=<id> hand=<left|right|both>` without per-frame spam;
- [ ] device test on Cozy/Standard/Bold and future haptic-scale extremes;
- [ ] no duplicated pulse from XRI/package defaults;
- [ ] current checklist and this table updated.

## 6. Explicitly not built by this document

- No `HapticRegistry`, `HapticManager`, global singleton, ScriptableObject catalog, new input owner, or
  automatic source-rewrite.
- No guessed amplitudes for missing verbs.
- No changes to weapons, locomotion, repair, zipline, UI, ship, vehicles, gardens, art, or scenes.
- No audit blocker yet. The next implementation phase should first inventory the remaining ❓ source
  owners, then choose the smallest P0 owner task and device-test it before scaling.

## 7. Terry headset review

During the consolidated pass, note:

1. scanner charge/pulse: too weak, good, or annoying;
2. whether holstering, releasing, selecting a tile, repairing and starting/finishing a zipline feel
   strangely silent;
3. any existing package/XRI pulse on grab/select that could be doubled by future code;
4. whether either controller ever vibrates when the opposite hand performs the action;
5. any prolonged vibration or rapid repeated pulses;
6. preferred relative ordering—micro confirmation < action click < weapon/impact < world-scale pulse.

Those observations determine the first runtime row; they do not block this documentation-level gate.
