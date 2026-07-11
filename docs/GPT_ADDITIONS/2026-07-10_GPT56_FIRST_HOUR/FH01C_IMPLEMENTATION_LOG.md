# FH-01C IMPLEMENTATION LOG — OWNER-SPECIFIC ADAPTER ENVELOPES

**Owner:** GPT-5.6 Thinking, cross-lane documentation/evidence slice  
**Authorized by:** Terry, 2026-07-11 (“continue… give me updates”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 IMPLEMENTED — first real Actions artifact pending  
**Packet:** `FH-01C`, implementation envelopes following FH-01A/FH-01B

## Goal

Turn the first-hour beat contract and evidence-backed binding inventory into closed implementation envelopes that Opus-class operators can execute without redesigning systems, crossing lane ownership, or creating duplicate event/save/travel/job/scanner/RILL paths.

Each envelope identifies:

- owning lane and any integration lane;
- exact beats and signal IDs covered;
- allowed new files and existing-file touches;
- API shape and existing owner to reuse;
- pure-core/test requirements;
- diagnostics and evidence classes;
- graceful no-op/fallback behavior;
- CI, bake, and device acceptance;
- dependencies and recommended execution order.

## Delivered files

Package and guide:

- `docs/first_hour/adapter_envelopes.json`
- `docs/first_hour/ADAPTER_ENVELOPES.md`
- `docs/first_hour/envelopes/FH-X01-CONTRACT-ASSET.json`
- `docs/first_hour/envelopes/FH-X02-PROGRESSION-CORE.json`
- `docs/first_hour/envelopes/FH-S01-OBSERVATION.json`
- `docs/first_hour/envelopes/FH-S02-HOLSTER.json`
- `docs/first_hour/envelopes/FH-S03-TRAVEL.json`
- `docs/first_hour/envelopes/FH-M01-SCANNER-RESULT.json`
- `docs/first_hour/envelopes/FH-S04-REPAIR-SCAN.json`
- `docs/first_hour/envelopes/FH-S05-CREATURE-RESOLUTION.json`
- `docs/first_hour/envelopes/FH-S06-ZIPLINE.json`
- `docs/first_hour/envelopes/FH-S07-HOME-W000-SURFACES.json`
- `docs/first_hour/envelopes/FH-A01-SIGNATURE-CREATURE-PRESENTATION.json`
- `docs/first_hour/envelopes/FH-S08-W001-ORCHESTRATION.json`

Validation:

- `tools/first_hour_envelope_gate.py`
- `tools/tests/test_first_hour_envelope_gate.py`

Coordination updated:

- `docs/SPRINT.md`
- this implementation log
- `docs/continuity/project_manifest.json`
- `.github/workflows/ci.yml`

## Explicit exclusions

- No C#, asmdef, scene, prefab, asset, `.meta`, package, or project-setting changes
- No runtime implementation of any envelope
- No new event bus, save schema, travel owner, job state machine, scanner, repair state machine, creature health owner, zipline mover, subtitle presenter, or ship-progression system
- No required CI blocker
- No claim that an envelope is implemented merely because it is specified
- No direct edits to another lane’s runtime files

## Final envelope split

### Architecture

1. `FH-X01-CONTRACT-ASSET` — deterministic runtime contract author and WARN-only structural audit
2. `FH-X02-PROGRESSION-CORE` — pure ordered progression and hesitation evaluator

### Story/Ship

3. `FH-S01-OBSERVATION` — gaze, movement-distance, and unrestricted arrival observation
4. `FH-S02-HOLSTER` — existing socket to semantic completion and canonical flag
5. `FH-S03-TRAVEL` — protected `TravelCoordinator` success callback
6. `FH-S04-REPAIR-SCAN` — existing repair stages plus `IScannable`
7. `FH-S05-CREATURE-RESOLUTION` — existing non-lethal disable callback
8. `FH-S06-ZIPLINE` — existing ride lifecycle callback
9. `FH-S07-HOME-W000-SURFACES` — cold boot, profile choice, comfort console, bunk prop and first helm
10. `FH-S08-W001-ORCHESTRATION` — one director, all 15 teaching lines, encounter and saved return payoff

### Multiplayer

11. `FH-M01-SCANNER-RESULT` — neutral immutable WristScanner pulse result

### Art

12. `FH-A01-SIGNATURE-CREATURE-PRESENTATION` — W001 species passport, tells, motion review and visual-provider proof

## Package result

- **12 envelopes**
- **18 non-direct completion beats covered**
- **15 teaching lines covered**
- **4 verified-direct beats reused rather than reimplemented**
- Owner distribution: Architecture 2, Art 1, Multiplayer 1, Story/Ship 8
- Dependency graph is acyclic

Verified-direct beats deliberately excluded from new implementation:

- `FH_ACCEPT_FIRST_JOB`
- `FH_MACHINE_POWER_CYCLE`
- `FH_SHOOT_PRACTICE_TARGET`
- `FH_FIRST_JOB_REWARD`

## Locked architecture decisions

- Adapters live on/reuse existing owners; there is no generalized replacement event bus.
- `TravelCoordinator` remains the only travel owner.
- `PlayerProfile.flags` remains the progression persistence path; device comfort remains in `PlayerPrefs`.
- `RillCompanion` / `RillLineLibrary` remain the only subtitle, once-latch and VO-fallback presenter.
- `JobRuntime`, `RepairableMachine`, `WristScanner`, `CreatureRuntime`, and `ZiplineRuntime` remain their state owners.
- Multiplayer exposes scan results; Story/Ship makes the repair machine scannable. Neither lane edits the other’s concern.
- Generated surfaces use idempotent patchers and stable markers; no hand-edited scene/prefab YAML.
- Device-sensitive envelopes remain yellow until Terry passes their `DEVICE` acceptance.

## Validation evidence

The exact GitHub blob SHA for every locally tested machine-readable package file matched:

- index: `4ce883a365b00a2c9a28f18fbc038e317fa65d6f`
- art envelope: `07d78e30b0c1ae42aec6c0be6b7d7de2715eccd3`
- multiplayer envelope: `efac474b11e83308aaac21e66484359220744c0c`
- observation: `71f13d08bfd2c36450817981cce9191d98620bbf`
- holster: `756cd22cba03f8a54f326a24b6488538cc0cacd1`
- travel: `b593291e69cc5c674412705862768664718a479d`
- repair/scan: `12b18fedecc100f8829a70c3894db77261e8383f`
- creature resolution: `8d9f8a6ba3ee1fc9c7737c72f66024e2879fdd17`
- zipline: `1737dfcab2912ac443b27446ca7c38579c821b70`
- home/W000: `d4480c3c88557541a1ef6b85c7545655fbe318d6`
- W001 orchestration: `ce9c1ba19200f123965c31ef9aa3f38896887be7`
- contract asset: `8ff60f9ed2568f29a950cac4bafc3af9cd0b82a5`
- progression core: `e77bc30182a044f986b0ff81e6a79280a5cc4bdf`
- validator: `2df215849584924c9f3b152b3c931d441d6cc681`
- tests: `325f69fd164886290163a09d28823ee4a229e30e`

Exact-file result:

```text
10 tests passed
0 failures
FIRST_HOUR_ENVELOPE_REPORT status=PASS findings=0 envelopes=12 beats=18 lines=15
```

The tests cover missing envelope files, uncovered beats, accidental reimplementation of verified beats, signal drift, missing line coverage, unknown/cyclic dependencies, missing existing touch files, shared-protocol requirements, and report-only versus strict exit behavior.

## Commits

- claim: `e28ae6c40abf84ef2410ca1ef00d7c949bff3572`
- package index: `9c935c6a7b2681db7c62363e1fa13d1240a46bca`
- `FH-X01`: `f8b9d6e2d09e1c0974c4809e5f0e216aec06b13f`
- `FH-X02`: `054a9eec9ba444724ab3c3a987cd18911dc775b6`
- `FH-S01`: `c77c1ed4c5338e9d0218a3fe4f75fffde33ec856`
- `FH-S02`: `e23e769620aac52bc583adbae7d5e499c8479df9`
- `FH-S03`: `8003d05092faa10b54bb5d96de6f4ab26b9962e4`
- `FH-M01`: `b3d7b737e607dd7e69eb1a4002fe5577b2ffc59a`
- `FH-S04`: `4010a8259ec794fdbea32e68134de3331d3c6eba`
- `FH-S05`: `434ac67060dd62192dd11471b150c4a2903cb5fa`
- `FH-S06`: `237c1a31213cba8515c3ce4a1a462ba32aa3d8b1`
- `FH-S07`: `84fe70185ba5db430f9accf072bea6bda74078d6`
- `FH-A01`: `b058d162c77ab434a6925a2fd20697ab8decc8b1`
- `FH-S08`: `6d4918e30d07e9c367a6a396bf133249c05f4ca7`
- validator: `22d2d1a5b0035dd2ecff1a70340d0f14e0b7079c`
- tests: `21f48a216180d9f8cf763124457e7278412c6e65`
- guide: `090bf0f58ecb18eabb6a5caef134cdcd3bfe7703`
- continuity registration: `370edffca6f9a531053b4ec58f6a9579e171e6e7`
- non-blocking CI report: `d2f8074f9b09d2f9d4f79657f11ed5e5f1be8637`
- board closure/release: `406327d4b63e8add4193ca80e257e513d470f80b`

## Cloud evidence limitation

- `fetch_commit_workflow_runs` returned no workflow runs for the CI wiring commit.
- The available connector therefore still cannot prove that Actions executed or that Unity compiled.
- FH-01C contains no Unity/C# changes, so runtime compilation was neither required nor claimed.
- The report job remains non-blocking and now includes `first_hour_envelope_report.json`.

## HANDOFF limitation

`docs/HANDOFF.md` is currently a 4,392-line shared file. The available connector exposes whole-file replacement but no safe append operation, and the local environment has no authenticated `gh`/git push path. Replacing that shared file from a truncated read would risk deleting history, so it was deliberately not modified.

Cross-lane dispatch is instead present in all safe discoverable surfaces available here:

- `docs/SPRINT.md`
- `docs/continuity/project_manifest.json`
- `docs/first_hour/ADAPTER_ENVELOPES.md`
- the twelve owner-tagged envelope JSON files
- this implementation log

The next operator with a normal local checkout should append one concise HANDOFF entry pointing each lane to its envelope IDs; it should not redesign the package.

## Next

- Architecture claims `FH-X01`, then `FH-X02`.
- Multiplayer may claim `FH-M01` independently.
- Picasso may claim `FH-A01` while continuing FORGE III.
- Story/Ship starts owner-local adapters only after Architecture's runtime contract/core exist.
- Final orchestration `FH-S08` waits until all dependencies have green CI and required device evidence.
- Runtime C# should not be pushed from this connector-only session while Unity Actions results remain unavailable.

**File claim released.**
