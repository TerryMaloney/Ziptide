# ZIPTIDE First-Hour Binding Inventory

`first_hour_bindings.json` is the evidence-backed bridge between the approved first-hour experience contract and the current repository.

It answers one question for every beat:

> Does the required completion already exist, need a thin observer, need new authored content, or require a genuinely new player-facing surface?

## Current result

The 22 completion bindings classify as:

| Status | Count | Meaning |
|---|---:|---|
| `verified-existing` | 4 | Direct public event/property/current chokepoint can report the semantic completion. |
| `adapter-required` | 9 | Gameplay exists; add a thin observer/event without changing ownership. |
| `composite-required` | 5 | Coordinate two or more existing seams and/or authored encounter content. |
| `new-surface-required` | 4 | The player-facing feature itself is not currently present. |

The 15 teaching-line bindings classify as:

| Status | Count |
|---|---:|
| `reuse-candidate` | 3 |
| `content-required` | 12 |

No runtime behavior is changed by this inventory.

## Highest-value conclusion

Most first-hour gameplay does **not** require new foundational systems.

Existing reusable owners include:

- `PlayerProfile.flags` and canonical `ZiptideFlags`
- `RillCompanion` + `RillLineLibrary` + `RillLineAuthor`
- `TravelCoordinator`
- `JobDirector` + `JobRuntime`
- `RepairableMachine`
- `TargetRuntime.OnHit`
- `CreatureRuntime`
- `ZiplineRuntime`
- `WristScanner` / `IScannable`
- `ShipRefit` + `ShipJourneyDecals`
- `SaveSystem`

The genuinely new player-facing work is concentrated in:

1. title / New Game / Continue presentation;
2. comfort-console confirmation;
3. gaze, hesitation, and safe-distance tutorial observation;
4. a minimal first-destination helm surface;
5. authored first-hour RILL lines;
6. signature-creature encounter orchestration;
7. changed-ship payoff timing and acknowledgement.

## Binding status definitions

- **`verified-existing`** — a current public event, property, flag write, or single completion chokepoint can directly report the semantic state.
- **`adapter-required`** — the behavior exists, but the tutorial needs a thin neutral observer/event.
- **`content-required`** — the delivery system exists, but the requested authored line/content does not.
- **`new-surface-required`** — the requested player-facing system itself does not exist.
- **`composite-required`** — completion combines existing systems and/or a small coordinator.

A stable diagnostic token is accepted as evidence that a transition exists, but implementation should prefer a public event/property over parsing logs.

## Locked reuse decisions

### One profile/save path

Map tutorial state into `PlayerProfile.flags`. Reuse:

- `FIRST_HOLSTER`
- `FIRST_RELEASE`
- `FIRST_TRAVEL`
- `FIRST_JOB_COMPLETE`
- `TUTORIAL_COMPLETE`

The FH-01A name `TUTORIAL_DONE` is a semantic contract label. Runtime implementation should map it to `ZiptideFlags.TUTORIAL_COMPLETE` unless a deliberate migration changes the project-wide constant.

### One RILL path

`RillCompanion` remains the presenter. New tutorial lines belong in `RillLineAuthor`; hesitation logic should enqueue through the existing library/presenter rather than render subtitles independently.

### One travel path

All departure and arrival completion comes from `TravelCoordinator`. `ShipCastOffRuntime`, doors, or future helm surfaces request travel but never load scenes themselves.

### One job path

Observe `JobRuntime` / `JobDirector`. Do not create tutorial-specific job state. Acceptance, repair progress, completion, rewards, and world flags already converge there.

### One machine path

Expose or observe `RepairableMachine` stage changes. Do not recreate panel/part/power state in a tutorial component.

### One scanner path

Reuse `WristScanner` and `IScannable`. The repair fault needs a scannable target and a result adapter, not a second scanner.

### One ship consequence path

`W001_COMPLETE` already maps to `decal_first_contract`; `ShipRefit` rebuilds journey decals from saved flags. The final beat should coordinate and acknowledge this visible result rather than invent a parallel trophy system.

## Suggested implementation order after device baseline

1. **Binding adapters with no presentation:** travel destination success, job acceptance/completion, target hit, holster event, repair stage event, creature disable event, zipline event.
2. **Pure TutorialDirector evaluator:** beat open, hesitation, completion, one-shot flag mapping.
3. **One proof chain in W000:** LOOK → COMFORT → MOVE.
4. **One physical proof chain:** GRAB → HOLSTER.
5. **Helm/cast-off travel proof:** INTERACT → first Ziptide → W001 arrival.
6. **Job observer chain:** accept → scan → repair stages → reward.
7. **Signature encounter and return payoff.**
8. Device tuning and only then broader rollout.

## Validation

Run:

```bash
python3 tools/first_hour_binding_gate.py \
  --json-report Builds/Reports/first_hour_binding_report.json
```

Run all report-gate tests:

```bash
python3 -m unittest discover \
  -s tools/tests \
  -p 'test_*_gate.py' \
  -v
```

Default mode is report-only. `--strict` returns exit code `2` for findings, but must not become required CI until the inventory and eventual runtime bindings have proven accurate.

The validator enforces:

- one completion binding per contract beat;
- exact completion signal ID match;
- one RILL-line binding per teaching beat;
- closed status and owner vocabularies;
- repository-relative evidence paths;
- exact evidence tokens present in each file;
- direct evidence for every `verified-existing` claim;
- no unsupported claim that a missing tutorial line already exists.
