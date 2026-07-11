# FH-01A IMPLEMENTATION LOG — FIRST-HOUR BEAT CONTRACT

**Owner:** GPT-5.6 Thinking, executing a Story/Ship data-contract slice  
**Authorized by:** Terry, 2026-07-10 (“keep going”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE  
**Packet:** `FH-01A`, the documentation/data-only first slice of FH-01

## Goal

Translate the locked onboarding design and the approved first-hour master plan into one machine-readable, validated beat graph. This slice must not alter runtime behavior or claim that any proposed signal/RILL binding already exists.

## Exact claimed files

New files:

- `docs/first_hour/first_hour_beats.json`
- `docs/first_hour/README.md`
- `tools/first_hour_gate.py`
- `tools/tests/test_first_hour_gate.py`

Coordination updates:

- `docs/SPRINT.md`
- this implementation log
- `docs/SPRINT_ARCHITECTURE.md` only to release the completed FH-02A file claim while its first cloud artifact remains pending
- `docs/continuity/project_manifest.json` only to add the first-hour contract as required project truth after the file exists

Announced shared touch:

- `.github/workflows/ci.yml` — expand the already non-blocking project-report job to run all `test_*_gate.py` tests and emit `first_hour_contract_report.json`. Unity tests and Android dependencies remain unchanged.

## Explicitly excluded

- No C#, asmdef, scene, prefab, asset, `.meta`, package, or project-setting changes
- No TutorialDirector or WorldMoment runtime
- No RILL line authoring
- No actual signal adapters
- No save/profile changes
- No boot, hub, travel, rig, input, interaction, job, creature, weapon, audio, art, or UI behavior
- No claim that proposed bindings exist
- No required CI blocker; the report job remains `continue-on-error: true`

## Contract decisions

- 22 sequential beats from cold boot through the changed-ship payoff
- 15 core verbs, each taught exactly once
- Comfort remains the first interactive moment after LOOK
- No input lock and no forced rig/camera movement on any beat
- Every required beat is reachable from the start
- The final beat sets both `TUTORIAL_DONE` and `FIRST_HOUR_COMPLETE`
- RILL lines and completion signals carry explicit `proposed` / `proposed-adapter` status until implementation binds them
- Device evidence remains required for all feel-sensitive beats

## Acceptance

1. Contract source documents resolve.
2. Beat IDs and sequence numbers are unique and contiguous.
3. Required graph is acyclic, connected, and ordered.
4. Every core verb is taught exactly once.
5. Teaching beats have a hesitation line and cannot auto-complete.
6. Required beats cannot depend on optional beats.
7. Input locking and rig movement are mechanically rejected.
8. Final completion flags are set by the last required beat.
9. Validator defaults to report-only and supports future strict ratcheting.
10. Unit tests cover the major graph and safety failure classes.

## Work log

### 2026-07-10 — claim opened

**Did:**
- Re-read current `SPRINT.md`, locked onboarding design, comfort order, home-hub design, first-hour master plan, current branch head, and the pending FH-02A status.
- Confirmed no newer push or Story/Ship claim occupies the new files.
- Built the contract and validator in an isolated local fixture before repository writes.
- Ran eight unit tests successfully.
- Ran strict validation of the complete 22-beat contract: `FIRST_HOUR_REPORT status=PASS findings=0`.

**Next:**
- Add the Story/Ship board claim.
- Commit the contract, validator, tests, and documentation.
- Update the continuity manifest and non-blocking report job.
- Verify repository paths directly and inspect any available workflow evidence.

**Heads-up:**
- All line IDs and completion signals in this slice are design bindings, not claims about current code. The later runtime implementation must map them to existing diagnostics/flags where possible instead of creating duplicate ownership.
