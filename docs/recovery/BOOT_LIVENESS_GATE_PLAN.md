# BOOT LIVENESS — why the net missed an unplayable build, and the gates that make the class impossible

**Date:** 2026-07-25 · **Lane:** T-Dog (implementation owner for the Home Hub fix, rb110)
**Companion:** `M0_BOOT_MENU_DEADLOCK_DIAGNOSTIC_20260725.md` (GPT — analysis + BOOT_LIVENESS
invariant). This document does not repeat that analysis. It records **independent verification of
its central claim**, names the general class, and specifies the **ordered, implementable gate
plan** so a lower-cost model can build it without redoing the investigation.

---

## 1. The one-sentence answer to "why didn't we catch it?"

**The verification harness disabled the exact production component whose misbehaviour caused the
bug, force-activated the thing that was broken, and then certified the repaired state as green** —
while the raw evidence of the real failure sat in the same artifact, logged and ignored.

## 2. Independently verified (I re-checked GPT's load-bearing claims against source)

| Claim | Verified? | Evidence |
|---|---|---|
| The green PlayMode artifact already logged `rays=0(active=0)` + `NO_RAY_INTERACTORS` before any test intervention | **CONFIRMED** (GPT's read of run `29786603998`) | quoted sequence in the diagnostic §4.2 |
| The harness disables the production input-modality owner | **CONFIRMED — I read it** | `RecoveryActualRigControllerSimulation.DisableModalityManagers()` sets `manager.enabled = false` on every `XRInputModalityManager` (`:493-503`) — that component is precisely what decides whether ray interactors go active |
| The harness then force-activates ray hierarchies | **CONFIRMED — I read it** | `ray.enabled = true` (`:547`), `if (!go.activeSelf) go.SetActive(true)` (`:569`), `behaviour.enabled = true` (`:596`) |
| The harness fakes the head pose instead of using tracking | **CONFIRMED** | `SetTrackedHeadPose(...)` walks the camera chain disabling `TrackedPoseDriver` (`:505-536`) and forces height 1.65 |
| `NO_RAY_INTERACTORS` counts only ACTIVE interactors | **CONFIRMED — I read it** | `PlayerRigPersistence` skips `!gameObject.activeInHierarchy` **before** the count (`:456-469`); five ray interactors do exist in `_Boot.unity` at `maxRaycastDistance=3` |
| The Home Hub is runtime-created, so scene audits cannot see it | **CONFIRMED** | `HomeHubRuntime.BuildSurface` creates board/tiles via `CreatePrimitive` at runtime; `UiReadabilityAuditRules` runs over authored scene objects |

## 3. The general class (this is bigger than one menu)

**CLASS: "component-green, goal-dead."** Every owner satisfied its local contract — the hold held,
the mover existed, the tiles bound, the synthetic test selected them — and no gate asked the only
question that matters to a player:

> From the ACTUAL launch state, with NOTHING repaired by the harness, can a real player execute the
> next required verb?

Three amplifiers made it invisible:
1. **Harness-heals-then-certifies.** A test that mutates initial state before observing it converts
   a blocker into setup. (This is the deepest one, and it is not boot-specific.)
2. **Warning without context.** `NO_RAY_INTERACTORS` is fatal *when combined with* boot hold +
   out-of-reach targets, harmless otherwise. Our gates grade findings individually, never in
   combination.
3. **Runtime-created content is unaudited.** Every audit we own inspects authored scenes. Anything
   built at runtime — the hub, the comfort console, every future runtime menu or machine — is a
   blind spot by construction.

## 4. THE GATE PLAN (ordered; each row states what it BLOCKS)

**G1 — Production-state-first liveness assertion (do first; smallest, highest value).**
Every recovery PlayMode test that later calls the synthetic activation MUST, before touching
anything, record the untouched production state and assert boot liveness on it. Implementation:
add `AssertProductionBootLiveness()` to run at the top of the boot smoke/round-trip tests,
capturing `activeRayCount`, `reachableDirectChoiceCount`, `locomotionEscape`, `fallbackEscape`.
*Blocks:* the exact failure that shipped — a build where all four are zero.

**G2 — Harness honesty law (make the amplifier structurally impossible).**
A test may not mutate initial state before asserting it. Enforce mechanically with a Python gate
over `Tests/PlayMode/**`: any file calling the simulation's `Activate()`/force-enable helpers must
contain a preceding production-state assertion call, and the mutated phase must log a **separately
named evidence class** (`RECOVERY_SYNTHETIC_*` never counts as production proof).
*Blocks:* any future test silently repairing what it is supposed to prove.

**G3 — Contextual fatal combinations (fix "warning nobody reads").**
A small rule table evaluated over the PlayMode log: the combination
`BOOT_HOLD=on ∧ HOME_HUB_READY=true ∧ active_rays=0 ∧ reachable_direct=0 ∧ locomotion_escape=false`
emits a **blocking** `BOOT_DEADLOCK`, even though each term alone is a warning.
*Blocks:* the whole family of "individually fine, jointly fatal" states.

**G4 — Runtime-created interactable census (close the blind spot).**
At PlayMode time, after the boot surface is built, enumerate every interactable that exists in the
live scene (not the authored one) and assert: has a collider, bound to the canonical manager,
inside the reach envelope from the ACTUAL tracked pose, and selectable by at least one active
modality. Reuses PG-4's envelope numbers (0.35–1.9 m height) but measured at runtime.
*Blocks:* the comfort console, and every runtime menu/machine we build from here on.

**G5 — First-actionable-verb delivery gate (the process rule with teeth).**
No APK is handed to Terry until the candidate proves, from settled spawn in production state, that
the **first required verb of that build** is executable. For this candidate that is "select NEW
GAME"; for the next it is the first W000 verb. Records as a named evidence line in the runbook.
*Blocks:* handing over a headset session that cannot begin.

**G6 — Reach proxies (kid-critical, cheap once G4 exists).**
Run G4's envelope check at three proxy heights — standing adult, seated, young child — since the
family audience is the target. *Blocks:* "works for Terry, not for his kids."

**Sequencing rationale:** G1 and G2 are days of work and would have caught this exact build. G3 and
G4 generalise the class. G5 is process, free to adopt now. G6 rides G4.

## 5. Grading law (adopt immediately, costs nothing)

- **"CI green" never means "playable."** The durable verdict must state which proof level it
  reached (SOURCE / CORE / PATCHED / PLAYMODE / VISUAL / APK / QUEST — the ladder already exists in
  `RECOVERY_PROGRAM.md`; this failure was PLAYMODE-green and QUEST-dead).
- **Any evidence produced after a harness mutation is a different evidence class** and may never be
  cited as production proof.
- **Every device-found failure must die three deaths** (fix + gate + `RATCHET:` line) — this one is
  fixed (rb110) and its gates are G1–G4 above; it is not closed until they exist.

## 6. Honest limits

None of these gates prove *fun*, comfort, or readability — those stay human. They prove the game is
**operable**: that a player who puts on the headset has at least one move available. That is a
lower bar than quality and a much higher bar than what we had.
