# FH-X01 IMPLEMENTATION LOG — FIRST-HOUR CONTRACT ASSET

**Owner:** GPT-5.6 Thinking, Architecture lane  
**Authorized by:** Terry, 2026-07-11 (“keep going”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE  
**Envelope:** `docs/first_hour/envelopes/FH-X01-CONTRACT-ASSET.json`

## Goal

Compile the approved `docs/first_hour/first_hour_beats.json` into one deterministic Unity Resources asset so runtime code consumes the contract without parsing JSON or maintaining a second hard-coded beat list.

## Exact scope

New runtime/editor/test files and their `.meta` files:

- `Ziptide/Assets/Ziptide/Content/Runtime/Tutorial/FirstHourContractDefinition.cs`
- `Ziptide/Assets/Ziptide/Editor/Patching/FirstHourContractAuthor.cs`
- `Ziptide/Assets/Ziptide/Editor/Audit/FirstHourContractAuditRules.cs`
- `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourContractAuthorTests.cs`

Generated asset path:

- `Ziptide/Assets/Ziptide/Resources/Tutorial/FirstHourContract.asset` created/updated by the author during Unity bake/build; do not hand-author YAML.

Shared/protected append-only touches in the second implementation commit:

- `Ziptide/Assets/Ziptide/Editor/Build/BuildAndroid.cs`
- `Ziptide/Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs`

Coordination:

- `docs/SPRINT_ARCHITECTURE.md`
- this log

## Locked behavior

- Preserve all 22 beat IDs and order, prerequisite IDs, completion signals, hint delays, RILL line IDs, grant flags, persistence/evidence fields, and the source hash.
- Use `JsonUtility` only in the Editor author; runtime loads a ScriptableObject through `Resources`.
- Author is create-or-update and deterministic.
- No runtime JSON parser and no fallback beat list.
- Missing, drifted, or invalid asset produces WARN-only audit codes:
  - `FIRST_HOUR_CONTRACT_ASSET_MISSING`
  - `FIRST_HOUR_CONTRACT_ASSET_DRIFT`
  - `FIRST_HOUR_CONTRACT_ASSET_INVALID`
- Missing asset disables tutorial orchestration only; base gameplay remains playable.

## Planned two implementation commits

1. Runtime definition + deterministic author + tests.
2. Audit rules + append-only build/audit hooks.

The connector will use Git tree commits so each planned slice remains one atomic commit.

## Acceptance

- 22 beats and 15 teaching lines imported.
- Repeat import is identical.
- Invalid input returns stable codes.
- Source hash is stable and matches the approved JSON bytes.
- Unity EditMode CI green.
- Android compile/audit proof remains required before final closure.

## Coordination note

P0.6 controller-free DevMenu work remains a separate Story/Ship claim and does not overlap these files.
