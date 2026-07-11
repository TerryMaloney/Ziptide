# ZIPTIDE First-Hour Adapter Envelopes

`adapter_envelopes.json` and `envelopes/*.json` are the implementation-ready handoff from the approved first-hour contract and evidence inventory to ZIPTIDE's four normal lanes.

They do **not** implement runtime behavior. They close the decisions an operator would otherwise be tempted to redesign:

- who owns each concern;
- which existing system remains authoritative;
- exact files that may be added or touched;
- required API behavior;
- tests, diagnostics, fallback, and evidence;
- dependency order;
- protected/shared-file procedure.

## Package result

- **12 owner-specific envelopes**
- **18 non-direct completion beats covered**
- **15 teaching-line bindings covered**
- **4 verified-direct beats deliberately reused**
- **0 runtime, Unity, scene, prefab, package, or project-setting changes in FH-01C**

The direct reusable beats are:

1. `FH_ACCEPT_FIRST_JOB`
2. `FH_MACHINE_POWER_CYCLE`
3. `FH_SHOOT_PRACTICE_TARGET`
4. `FH_FIRST_JOB_REWARD`

Those already have observable owners. Future implementation consumes them; it does not create replacement state.

## Execution order

The validated dependency graph is acyclic. Execute in this order:

1. `FH-X01-CONTRACT-ASSET`
2. `FH-X02-PROGRESSION-CORE`
3. Parallel owner-local work:
   - `FH-S01-OBSERVATION`
   - `FH-S02-HOLSTER`
   - `FH-S03-TRAVEL`
   - `FH-M01-SCANNER-RESULT`
   - `FH-S05-CREATURE-RESOLUTION`
   - `FH-S06-ZIPLINE`
   - `FH-A01-SIGNATURE-CREATURE-PRESENTATION`
4. `FH-S04-REPAIR-SCAN`
5. `FH-S07-HOME-W000-SURFACES`
6. `FH-S08-W001-ORCHESTRATION`

Do not begin final orchestration early. Its purpose is to integrate proven owner signals, not compensate for missing signals.

## Envelope summary

| Envelope | Owner | Purpose | Commit budget |
|---|---|---|---:|
| `FH-X01-CONTRACT-ASSET` | Architecture | Generate runtime contract data and structural audit from approved JSON | 2 |
| `FH-X02-PROGRESSION-CORE` | Architecture | Pure ordered progression and hesitation evaluator | 1 |
| `FH-S01-OBSERVATION` | Story/Ship | LOOK, safe movement, and unrestricted W001 orientation | 2 |
| `FH-S02-HOLSTER` | Story/Ship | Existing holster socket to semantic completion and canonical flag | 1 |
| `FH-S03-TRAVEL` | Story/Ship | Existing `TravelCoordinator` success to first/return signals | 1 |
| `FH-M01-SCANNER-RESULT` | Multiplayer | Existing wrist-scanner pulse to immutable neutral result | 1 |
| `FH-S04-REPAIR-SCAN` | Story/Ship | Existing repair machine becomes scannable and exposes its stages | 2 |
| `FH-S05-CREATURE-RESOLUTION` | Story/Ship | Existing non-lethal disable to semantic completion | 1 |
| `FH-S06-ZIPLINE` | Story/Ship | Existing ride lifecycle to designated-arrival completion | 1 |
| `FH-S07-HOME-W000-SURFACES` | Story/Ship | Cold boot, profile choice, comfort console, bunk prop, first helm | 3 |
| `FH-A01-SIGNATURE-CREATURE-PRESENTATION` | Art | W001 creature passport, tells, motion review and visual provider | 3 |
| `FH-S08-W001-ORCHESTRATION` | Story/Ship | One director, W001 encounter, 15 RILL lines, saved ship payoff | 3 |

## Non-negotiable architecture decisions

### No second event bus

Adapters are additive callbacks on their current owners:

- travel completion belongs to `TravelCoordinator`;
- holster completion belongs to `HolsterSocketInteractor`;
- repair stages belong to `RepairableMachine`;
- scan results belong to `WristScanner`;
- creature disable belongs to `CreatureRuntime`;
- ride lifecycle belongs to `ZiplineRuntime`.

`FirstHourDirector` subscribes. It does not become the source of those states.

### No second save model

Runtime mapping is fixed:

- semantic `TUTORIAL_DONE` maps to existing `ZiptideFlags.TUTORIAL_COMPLETE`;
- `FIRST_HOUR_COMPLETE` records completion of this route;
- completed beat state uses the established profile-flag overlay idiom;
- device comfort remains in `PlayerPrefs` and survives profile wipes;
- no tutorial-specific profile schema is added.

The pure progression core performs no I/O.

### No second RILL presenter

`RillLineAuthor` remains content truth. `RillLineLibrary` may gain pure lookup-by-ID and `RillCompanion` one enqueue-by-ID path, but the existing system retains:

- once-per-save latching;
- subtitle formatting;
- VO fallback;
- queue timing;
- `ZIPTIDE: RILL_LINE` diagnostics.

Hesitation logic requests a line; it never renders one.

### No second travel path

The only permitted route is:

`first helm or return surface → TravelCoordinator → existing save, rig, XRI and inventory flow → additive completion callback`

No new `SceneManager.LoadScene` call is allowed.

### No second job, repair, scanner, creature, or zipline state

The director observes existing owners. It never mirrors their state machines.

The Multiplayer envelope exposes a neutral scanner-result snapshot. Story/Ship makes the existing repair machine implement `IScannable`. Neither lane edits the other's concern.

### No hand-edited scene or prefab YAML

Cold-boot, comfort, bunk, helm and W001 proof surfaces are generated by idempotent patchers with stable `__FIRST_HOUR_*` markers.

## Owner instructions

### Architecture

Claim `FH-X01` first, then `FH-X02`.

`FH-X01` compiles the approved JSON into one deterministic Resources asset and adds WARN-only structural audit evidence. It must never create a runtime JSON parser or a second hard-coded beat list.

`FH-X02` is pure C# with no `UnityEngine`, I/O, wall clock, profile write, scene lookup, or subtitle presentation. Unknown, duplicate and early signals are deterministic no-ops.

### Story/Ship

Adapters remain thin:

- observation samples gaze/distance/orientation without moving or locking the rig;
- holster emits after a valid existing socket selection and sets `FIRST_HOLSTER` once;
- travel emits only after canonical `TRAVEL_OK`, XRI readiness and inventory restoration;
- repair exposes the existing stage and implements the existing scanner interface;
- creature completion filters the designated signature instance but does not change damage, rewards, ecology or respawn;
- zipline completion requires `reason=arrived`, not simple release.

`FH-S07` is the first player-facing surface batch. It must follow `HOME_HUB.md` and `COMFORT_AND_ACCESSIBILITY.md`, preserve device-level comfort settings during New Game, and delegate all travel to the established owner.

`FH-S08` is the sole first-hour orchestrator. It subscribes to owner events, feeds the pure core, grants returned flags, requests RILL lines through the existing presenter, and verifies the existing first-contract decal on return.

### Multiplayer

`FH-M01` adds an immutable neutral scan-result snapshot to the existing wrist scanner. It must not reference Story/Ship classes or add campaign state to PvP files. Scanner gesture, cooldown, visuals, haptics and target filtering remain unchanged.

### Art / Picasso

`FH-A01` continues FORGE III rather than replacing it. The deliverable is one complete W001 species passport and review artifact set:

- world/ecology reason;
- body-source logic;
- distinct silhouette;
- locomotion/contact source;
- alert, telegraph, action, recovery, stun and disable states;
- readable visual tell and counter;
- audio grammar with visual redundancy;
- habitat traces;
- Quest budget and LOD;
- front, side, back and state contact sheets.

Gameplay ID, colliders, sockets, stats, AI, rewards and encounter outcome remain Story/Ship-owned.

## Failure and fallback laws

Every envelope must degrade without corrupting the base game:

- missing contract asset disables tutorial orchestration only;
- missing observer leaves its beat open and logs once;
- missing line never blocks mechanical completion;
- missing art uses the current proxy but cannot close art/device acceptance;
- missing payoff decal preserves saved progress and leaves the payoff beat open;
- subscriber failure may not abort travel, scanning, repair, combat, zipline or RILL delivery;
- no fallback may silently auto-complete a required beat.

## File-claim procedure

For each runtime envelope:

1. Read the exact envelope JSON.
2. Read the latest lane board and newest HANDOFF entries.
3. Post the envelope ID and exact file claim before coding.
4. For protected/shared files, announce the additive touch and reread the current file SHA immediately before writing.
5. Build the pure test/core first where the envelope requires one.
6. Make one small commit per stated budget slice.
7. Stop shipping C# immediately on CI red; apply the three-red circuit breaker.
8. Queue concrete bake/device checks in `TERRY_RUNBOOK.md`.
9. Close only after board, HANDOFF, evidence and required device status agree.

## Validation

Run:

```bash
python3 tools/first_hour_envelope_gate.py \
  --json-report Builds/Reports/first_hour_envelope_report.json
```

Run all project-contract tests:

```bash
python3 -m unittest discover \
  -s tools/tests \
  -p 'test_*_gate.py' \
  -v
```

Default execution is report-only. `--strict` returns exit code `2` when findings exist, but should not become required CI until the package has produced stable cloud artifacts.

The validator rejects:

- missing envelope files;
- unknown or duplicate envelope IDs;
- uncovered non-direct beats or teaching lines;
- reimplementation of verified-direct beats;
- signal or line-ID drift;
- unknown owners, kinds or risk classes;
- missing cross-lane ownership or shared-file protocol;
- unknown/self/cyclic dependencies;
- unsafe or empty file scopes;
- direct `.unity` or `.prefab` YAML scope;
- missing tests, API, diagnostics, fallback or acceptance;
- device-sensitive envelopes without device acceptance.

## Definition of FH-01C complete

FH-01C is complete when the index, twelve envelopes, guide, validator and tests are committed; continuity and non-blocking report wiring include them; HANDOFF exposes the four owner queues; exact committed files pass the validator; and the temporary cross-lane documentation claim is released.

FH-01C completion means **implementation decisions are closed**. It does not mean the runtime first hour is built, CI-compiled, baked or device-approved.


## Review additions (T-Dog/Fable 5, 2026-07-11) — gaps found by cross-check, added as checklist items
Reviewed against the locked onboarding design, the EXCELLENCE_MAP/DoD, and the runbook law. The
envelope package is sound; these four items were MISSING and are now part of the package's
definition of done (they are checklist items, not new envelopes):
- [ ] **FH-GAP-1 · VETERAN SKIP (design law 4, unimplemented anywhere in the 22 beats):** existing
      profiles with prior playtime get `TUTORIAL_DONE` pre-set at load (one migration line in the
      progression core, FH-X02's owner) — a returning player must NEVER meet a teaching beat.
      Zero hits for veteran/skip in `first_hour_beats.json` today.
- [ ] **FH-GAP-2 · DEVICE EVIDENCE FLOWS TO THE RUNBOOK:** every envelope's `acceptance.DEVICE`
      list must be mirrored into `TERRY_RUNBOOK.md` at envelope close — the runbook is THE single
      Terry checklist (its own header law); the DEVICE fields are a second checklist until mirrored.
- [ ] **FH-GAP-3 · GATE PROMOTION CRITERION:** `first_hour_gate.py` runs in CI but report-only,
      forever. Set the ratchet: the moment FH-S08 closes green, the gate flips to BLOCKING (a
      standard without an enforcing gate is a wish — LAW 3 / map doctrine).
- [ ] **FH-GAP-4 · MAP LINKAGE (DoD item 8):** the EXCELLENCE_MAP onboarding row must name this
      package as the build vehicle and track its state; envelope closers update the row in the
      same push. Done for the current state in this commit.
