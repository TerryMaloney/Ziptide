# ZIPTIDE Durable CI Verdict Contract

`docs/CI_VERDICT.md` is a generated, repository-visible record of the most recent eligible CI run for the live `terry-local-wip` head.

It exists so connector-only operators can verify the tested SHA and outcome without requiring GitHub Actions API permissions.

## Scope

The recorder summarizes the existing jobs. It does not replace or modify them:

- `test` — Unity EditMode compilation/tests; blocking for the verdict.
- `build-android` — blocking when it actually runs; normally `skipped` on an ordinary branch push.
- `continuity-report` — informational/non-blocking project-contract reports.

A `GREEN` verdict means:

1. Unity EditMode job result is `success`.
2. Android result is `success` when the Android job ran, or `skipped` when it was not scheduled.

Any unknown, failed, cancelled or timed-out blocking result records `RED`.

A green branch-push verdict does **not** claim an APK was built when `androidApk` is `skipped`.

## Write eligibility

The final `record-verdict` job may write only when all conditions are true:

- event is `push` or `workflow_dispatch`;
- ref is exactly `refs/heads/terry-local-wip`;
- all earlier jobs have reached a terminal result;
- the checked-out live branch head still equals the workflow's tested SHA.

If the branch advanced, the job exits successfully without writing. The newer workflow run owns the verdict.

If a push races after the freshness check and the bot push is rejected, the job emits a warning and exits successfully. It never force-pushes.

## Loop prevention

The generated commit:

- changes only `docs/CI_VERDICT.md`;
- uses the repository `GITHUB_TOKEN`;
- includes `[skip ci]`;
- is also excluded by the CI workflow's `paths-ignore` rule.

These are redundant safeguards. The verdict write must not start another CI run.

## Permissions

Only the final recorder job receives `contents: write`. Existing test/report/build jobs retain their prior permissions and behavior.

## Machine-readable payload

The file contains one JSON block with:

- schema version;
- overall verdict;
- branch and tested SHA;
- workflow run ID, attempt and URL;
- event and UTC record time;
- Unity EditMode, Android and project-contract job results;
- interpretation notes.

Operators must compare `testedSha` to the live branch head before treating the verdict as current.

## Failure behavior

The recorder is deliberately non-destructive:

- renderer validation failure makes the recorder job fail but does not alter Unity outputs;
- stale branch head produces no write;
- rejected race push produces a warning, not a force-push;
- missing write permission leaves the previous verdict intact and visible as stale;
- no fallback may fabricate `GREEN`.

## Local tests

```bash
python3 -m unittest discover \
  -s tools/tests \
  -p 'test_ci_verdict_gate.py' \
  -v
```

The renderer is `tools/ci_verdict.py` and uses only the Python standard library.
