# ZIPTIDE First-Hour Beat Contract

`first_hour_beats.json` is the machine-readable experience contract for the first commercial-quality hour:

**cold boot → W000 ship → first Ziptide → W001 technician loop → signature creature resolution → changed-ship return**

It translates the approved master plan and locked onboarding design into data that another model can implement without inventing sequence, tutorial order, safety rules, persistence expectations, or evidence requirements.

## Current status

**FH-01A is contract-only.**

The JSON contains proposed bindings for RILL line IDs and completion signals. A binding marked `proposed` or `proposed-adapter` is a requirement for later implementation, not proof that a matching runtime event already exists.

This slice does not add a `TutorialDirector`, author lines, change saves, wire signals, alter scenes, or modify gameplay.

## Contract shape

Each beat declares:

- stable ID and contiguous sequence;
- phase and world;
- prerequisites;
- required/optional status;
- one completion signal and its binding status;
- zero or one newly taught verb;
- hesitation timing and RILL line for teaching beats;
- flags set on completion;
- persistence scope;
- required evidence classes;
- hard safety values for input lock, rig motion, and auto-completion;
- implementation notes.

The current contract contains:

- **22 required beats**
- **15 core verbs**
- exactly one teaching beat per core verb
- `TUTORIAL_DONE` and `FIRST_HOUR_COMPLETE` on the final payoff beat

## Run locally

From the repository root:

```bash
python3 tools/first_hour_gate.py \
  --json-report Builds/Reports/first_hour_contract_report.json
```

Default behavior is report-only. Findings are printed and written to JSON, but the command returns success.

Preview future enforcement:

```bash
python3 tools/first_hour_gate.py --strict
```

Strict mode returns exit code `2` when findings exist. Do not make this required CI until runtime implementation exists and the report has proven low-noise.

## Run tests

```bash
python3 -m unittest discover \
  -s tools/tests \
  -p 'test_*_gate.py' \
  -v
```

The first-hour tests use temporary repository fixtures and do not alter the checkout.

## Important laws enforced now

- Source documents must exist.
- Beat IDs and sequence numbers must be unique.
- Sequence must be contiguous.
- Every prerequisite must exist and occur earlier.
- Required beats cannot depend on optional beats.
- Every required beat must be reachable from the root.
- Every core verb is taught exactly once.
- Teaching beats require a hesitation interval and RILL line.
- Every beat requires an explicit completion signal.
- No beat may lock input.
- No beat may translate or rotate the player rig.
- No beat may silently auto-complete.
- The final required beat must set all declared completion flags.

## Binding status meanings

- `proposed`: approved content identifier that still needs authoring.
- `proposed-adapter`: approved semantic signal that should be mapped onto an existing diagnostic/event/flag where possible.
- `verified-existing`: confirmed current repository binding. Use only with evidence.

A later implementation should prefer adapters around current signals over a second event system.

## Ratcheting order

1. Data contract and report-only validator — this slice.
2. Audit the proposed signal/line bindings against current code.
3. Reuse existing diagnostics and flags where available.
4. Add the smallest pure evaluation core.
5. Add one runtime proof beat.
6. Validate on device.
7. Expand the translator.
8. Promote only stable structural checks to required CI.

The contract defines experience order. Existing project ownership, save, travel, input, RILL, and world systems remain authoritative for implementation.
