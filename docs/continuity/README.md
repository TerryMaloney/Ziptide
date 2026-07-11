# ZIPTIDE Continuity Report

`project_manifest.json` is a machine-readable index of the existing project truth. It does not replace the operator manual, lane boards, HANDOFF, runbook, Excellence Map, or design documents.

## Current status

**FH-02A is observation-only.** The validator reports explicit structural problems but does not block Unity tests, Android builds, or another operator's work.

## Run locally

From the repository root:

```bash
python3 tools/continuity_gate.py \
  --json-report Builds/Reports/continuity_report.json
```

Default behavior:

- validates manifest structure;
- rejects absolute and repository-escaping paths;
- confirms required documents, lane boards, ownership-pattern roots, shared paths, protected decision documents, and protected paths exist;
- reports duplicate lane/contract IDs and invalid versions;
- writes a stable JSON report when requested;
- returns success even when validation findings exist.

Future strict behavior can be previewed with:

```bash
python3 tools/continuity_gate.py --strict
```

`--strict` returns exit code `2` when findings exist. Do not add `--strict` to required CI until the report has run cleanly long enough to establish that its rules are accurate and low-noise.

## Run tests

```bash
python3 -m unittest discover \
  -s tools/tests \
  -p 'test_continuity_gate.py' \
  -v
```

The tests use temporary fake repositories. They do not modify the working checkout.

## Stable finding codes in v1

- `SCHEMA_VERSION_UNSUPPORTED`
- `BRANCH_OF_TRUTH_MISSING`
- `REQUIRED_PATH_MISSING`
- `REQUIRED_PATH_DUPLICATE`
- `PATH_INVALID`
- `PATH_ABSOLUTE`
- `PATH_OUTSIDE_ROOT`
- `FIELD_TYPE_INVALID`
- `FIELD_ITEM_INVALID`
- `FIELD_REQUIRED`
- `LANES_REQUIRED`
- `LANE_INVALID`
- `LANE_ID_INVALID`
- `LANE_ID_DUPLICATE`
- `LANE_BOARD_INVALID`
- `LANE_BOARD_MISSING`
- `LANE_BOARD_DUPLICATE`
- `LANE_NOTES_INVALID`
- `OWNERSHIP_PREFIX_MISSING`
- `CONTRACTS_INVALID`
- `CONTRACT_INVALID`
- `CONTRACT_ID_INVALID`
- `CONTRACT_ID_DUPLICATE`
- `CONTRACT_VERSION_INVALID`
- `CONTRACT_DECISION_DOC_MISSING`
- `CONTRACT_PATH_MISSING`

## Ratcheting policy

A report check may become blocking only after:

1. it has run against the real repository;
2. every initial finding has been classified as a real defect or fixed false positive;
3. the owning architecture row explicitly proposes promotion;
4. the change is announced before touching required CI;
5. Terry approves the promotion or it is already covered by an approved packet.

Protected contracts are currently indexed by ID, version, decision document, and paths. FH-02A does not fingerprint file contents or require version bumps. Those are later FH-02 stages after the report-only foundation proves reliable.
