# ZIPTIDE First-Hour Opus Launch Kit

This file is the human-readable companion to `opus_launch_manifest.json`.

Use one prompt per connected Opus account. Each account must still recheck the live branch, read the newest HANDOFF entries, post its normal board/file claim, and obey the envelope’s CI, bake, device, ownership, and commit-budget rules.

The machine-readable source of ownership and dispatch truth is:

- `docs/first_hour/opus_launch_manifest.json`
- `docs/first_hour/adapter_envelopes.json`
- `docs/first_hour/envelopes/*.json`

## Start order

1. Architecture starts `FH-X01-CONTRACT-ASSET`.
2. Multiplayer may independently start `FH-M01-SCANNER-RESULT`.
3. Picasso may independently start `FH-A01-SIGNATURE-CREATURE-PRESENTATION` while continuing FORGE III.
4. Architecture completes `FH-X02-PROGRESSION-CORE` after `FH-X01` is green.
5. Story/Ship may then start `FH-S01-OBSERVATION` and later claim one Story/Ship envelope at a time.
6. `FH-S08-W001-ORCHESTRATION` is last. It waits for every dependency and required device evidence.

## Architecture Opus

Copy this prompt into the Architecture account:

```text
Read CLAUDE.md, docs/OPERATOR_START_HERE.md, docs/SPRINT_ARCHITECTURE.md, docs/first_hour/ADAPTER_ENVELOPES.md, and docs/first_hour/envelopes/FH-X01-CONTRACT-ASSET.json. Recheck the branch and newest HANDOFF entries, then claim FH-X01 before editing. Implement only that envelope in its stated commit budget. Do not begin FH-X02 until FH-X01 is CI-green. Do not change beat order or create a runtime JSON parser.
```

Architecture owns only:

- `FH-X01-CONTRACT-ASSET`
- `FH-X02-PROGRESSION-CORE`

Stop immediately on CI red, at the three-red circuit breaker, or when a proposed solution would redesign travel, save, rig, input, or presentation ownership.

## Multiplayer Opus

Copy this prompt into the Multiplayer account:

```text
Read CLAUDE.md, docs/OPERATOR_START_HERE.md, docs/SPRINT_MULTIPLAYER.md, docs/first_hour/ADAPTER_ENVELOPES.md, and docs/first_hour/envelopes/FH-M01-SCANNER-RESULT.json. Recheck the branch and newest HANDOFF entries, then claim FH-M01 before editing. Expose only the neutral immutable WristScanner pulse result. Preserve scanner gesture, cooldown, visuals, haptics, filtering, and PvP behavior. Do not reference Story/Ship classes or add campaign state to Multiplayer files.
```

Multiplayer owns only:

- `FH-M01-SCANNER-RESULT`

Story/Ship later consumes the neutral result. Multiplayer does not make the repair machine scannable and does not add tutorial state.

## Picasso / Art Opus

Copy this prompt into the Art account:

```text
Read CLAUDE.md, docs/OPERATOR_START_HERE.md, docs/SPRINT_ART.md, docs/project_art_plan/FORGE_III_PLAN.md section 0, docs/first_hour/ADAPTER_ENVELOPES.md, and docs/first_hour/envelopes/FH-A01-SIGNATURE-CREATURE-PRESENTATION.json. Recheck the branch and newest HANDOFF entries, then claim FH-A01 before editing. Continue FORGE III; do not replace it. Deliver the W001 species passport and review artifacts while preserving gameplay IDs, colliders, sockets, stats, AI, rewards, and encounter outcome ownership.
```

Art owns only:

- `FH-A01-SIGNATURE-CREATURE-PRESENTATION`

The result must be a complete planet-specific species passport and review artifact set—not merely a prettier generic proxy. It remains device-yellow until Terry can identify the species, tell, counter, locomotion, and disable state in the headset.

## Story/Ship Opus

Copy this prompt into the Story/Ship account:

```text
Read CLAUDE.md, docs/OPERATOR_START_HERE.md, docs/SPRINT.md, docs/first_hour/ADAPTER_ENVELOPES.md, and docs/first_hour/envelopes/FH-S01-OBSERVATION.json. Recheck the branch and newest HANDOFF entries. Do not implement first-hour runtime code until Architecture has shipped FH-X01 and FH-X02 with green CI. Then claim exactly one Story/Ship envelope, starting with FH-S01, and stay inside its file scope and commit budget. Final orchestration FH-S08 waits for every dependency and required device evidence.
```

Story/Ship owns:

- `FH-S01-OBSERVATION`
- `FH-S02-HOLSTER`
- `FH-S03-TRAVEL`
- `FH-S04-REPAIR-SCAN`
- `FH-S05-CREATURE-RESOLUTION`
- `FH-S06-ZIPLINE`
- `FH-S07-HOME-W000-SURFACES`
- `FH-S08-W001-ORCHESTRATION`

Do not begin runtime work until Architecture `FH-X02` is green. Do not create a second event, save, travel, job, scanner, repair, creature, zipline, or RILL owner.

## GPT-5.6 integration review

Use this review instruction before implementation, after the first green commit, and before closure:

```text
Review the claimed envelope against its JSON. Check ownership, exact file scope, duplicate-system risk, API shape, tests, diagnostics, fallback, dependency evidence, commit budget, and whether device-sensitive work remains yellow until Terry verifies it. Do not absorb the lane’s implementation work or redesign an approved envelope without a documented blocker.
```

## What “green” means

A lane may call an implementation bite green only when:

- the normal Unity EditMode workflow is green;
- the envelope’s own tests are green;
- protected/shared touches were announced and reread before writing;
- no replacement owner or duplicate state was introduced;
- required generated/bake artifacts are present;
- device-sensitive items remain yellow until Terry passes them.

A report-only Python gate passing is not a substitute for Unity compile or headset evidence.

## Validation

Run:

```bash
python3 tools/first_hour_launch_gate.py \
  --json-report Builds/Reports/first_hour_launch_report.json
```

Run all project-contract tests:

```bash
python3 -m unittest discover \
  -s tools/tests \
  -p 'test_*_gate.py' \
  -v
```

The launch validator checks that all four lanes exist, every envelope is assigned exactly once to its owner, prompts name their board/start envelope/claim requirement, Story/Ship’s Architecture blocker is explicit, and required reads and boards exist.
