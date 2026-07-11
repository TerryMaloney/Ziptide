# CI-V1 IMPLEMENTATION LOG — DURABLE BRANCH VERDICT

**Owner:** GPT-5.6 Thinking, Architecture/CI coordination slice  
**Authorized by:** Terry, 2026-07-11 (“I approve it… free reign… be absolutely sure you don’t break anything”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ IMPLEMENTED AND PROVEN

## Goal

Record the outcome of each completed `terry-local-wip` CI run in a repository file that connector-only operators can read, without changing Unity test/build behavior, creating an Actions recursion loop, or allowing a stale run to overwrite a newer verdict.

## Delivered files

- `docs/CI_VERDICT.md`
- `docs/ci/CI_VERDICT_CONTRACT.md`
- `tools/ci_verdict.py`
- `tools/tests/test_ci_verdict_gate.py`
- this log
- `.github/workflows/ci.yml`
- `docs/continuity/project_manifest.json`
- `docs/SPRINT_ARCHITECTURE.md`

## Safety design

- Existing `continuity-report`, Unity `test`, and Android `build-android` logic remains unchanged.
- A final `record-verdict` job uses `needs` results and runs with `if: always()` only on `terry-local-wip` push/manual runs.
- The job receives `contents: write`; all other jobs retain their existing permissions.
- It checks that the live branch head still equals the tested SHA before writing.
- If the branch advanced or the push races, it warns and exits successfully; the newer run owns the verdict.
- The bot commit changes only `docs/CI_VERDICT.md`, includes `[skip ci]`, and that path is excluded from push triggers.
- `GREEN` requires Unity EditMode success and Android success when Android ran; ordinary branch pushes may record Android as `skipped`.
- Project-contract reports are informational/non-blocking.
- Unknown blocking job results fail closed to `RED`.
- After a successful write, the live head may be the direct generated verdict-only child of `testedSha`; a later normal commit makes the verdict stale.

## Exclusions honored

- No Unity runtime, scene, prefab, asset, package, project-setting, audit, test-runner, or APK-build behavior changed.
- No force-push.
- No workflow self-trigger loop.
- No stale-run overwrite.
- No claim that `GREEN` includes Android when `androidApk` is `skipped`.

## Proof that the mechanism is real

The first eligible recorder run did not merely write a placeholder. It exposed a genuine Unity test failure:

- red run: `29151688686`
- result: 805 passed, 1 failed
- failing test: `PvpProgressionTests.Stats_TrackKillsDownsAndStreaks`
- recorder correctly wrote `RED`

The failure was traced to an incorrect test expectation, not runtime behavior. The assertion was corrected from one down to two after an earlier down plus a self-down, with no runtime change.

The next run then recorded:

- tested SHA: `b388f1b308da8d7e269d8bd3f669f8309ef79904`
- run ID: `29151953887`
- Unity EditMode: `success`
- Android: `skipped` as expected for an ordinary branch push
- project-contract reports: `success`
- overall: `GREEN`

The generated verdict commit did not recursively start CI. The marker file remains readable through the GitHub connector.

## Test evidence

- Pure renderer suite: **9 tests passed, 0 failed**.
- Unknown results fail closed.
- Android failure records red when Android runs.
- Contract-report failure remains informational.
- Wrong branch is rejected.
- Explicit-time rendering is deterministic.
- Direct verdict-only child currency is documented and tested.

## Operational use

Read `docs/CI_VERDICT.md` and compare its `testedSha` to the live branch:

1. current if live head equals `testedSha`; or
2. current if live head is its direct generated verdict-only child and only the verdict file changed.

Any later normal commit makes the verdict stale until that commit’s run records a new verdict.

## Closure

CI-V1 is complete and the Architecture file claim is released. Future changes to the recorder require a new board claim and must not alter Unity test/build semantics incidentally.
