# 2026-09-29 — Checkpoint CI infrastructure recovery

Source d864b94f0c925b8f20cbe945eecaa986da6d451b, run 36653673770 attempt 1,
EditMode job 109693367964 failed before Unity execution:
`Failed to resolve the latest game-ci CLI release: GitHub API returned 403.`
There is no compiler/NUnit verdict from that attempt. Failed jobs were retried once;
at observation the replacement job 109716584233 is executing Run EditMode tests.

Bounded durable correction: ci.yml test runner cliVersion pinned to v0.1.69, the exact CLI
restored from cache in successful run 36621171988 / job 109586875913. Upstream v4
src/download-cli.ts confirms an explicit version bypasses resolveLatestTag() and selects
that version's cache/download directly. Existing Unity 6000.2.9f1 version, tests, gates,
credentials and permissions remain unchanged. No secret values read or copied.

Upstream sources inspected:
- https://github.com/game-ci/unity-test-runner/blob/v4/action.yml
- https://github.com/game-ci/unity-test-runner/blob/v4/src/download-cli.ts

No new gameplay C# is authorized by a passing local preflight alone. Resume expansion after
Unity verification is healthy. This infra failure is distinct from a failing source candidate;
if the same infrastructure error recurs, use the pinned runner rather than blind retries.

Attempt 2 completed: Unity compiled and the 28 JobCheckpointTests cases passed. One older
PhotoAlbumTests assertion failed (Expected 3, actual 4) because it pinned CurrentSchemaVersion
to v3. Corrected the test to require migration to the current schema and at least v3 photo
support, preserving the empty/non-null album checks. This is the only observed NUnit failure.
New run required on the correction; do not call the entire candidate green yet.
