# How a cloud agent verifies code without Unity (read this if you "can't compile locally")

**You don't need Unity, and you can't have it.** Both Claude agents run in cloud containers with **no
Unity Editor, no Android SDK, no headset** — that is by design and is the same for T-Dog and Architect.
Do not try to install/provision Unity. The verification net is **CI (GameCI on GitHub Actions)**, which
*does* run real Unity in Docker. Your job: write C#, push, then **read the CI result**.

## The loop
1. Write/edit C# on `terry-local-wip`. Keep new logic **pure + EditMode-testable** (tests live in
   `Ziptide/Assets/Ziptide/Tests/EditMode/Ziptide.Tests.EditMode.asmdef` — add tests there).
2. Commit + push to `terry-local-wip`. The push triggers `.github/workflows/ci.yml`, which spins up
   Unity `2022.3.62f3` in Docker, **compiles all assemblies + runs the EditMode suite**.
3. Wait ~5 min (a cold cache run can take ~9), then **read the run's conclusion**.
4. `success` = everything compiled and all tests passed. `failure` = read the log, fix, re-push.

## How to read CI (use whichever your env has)
**A) GitHub MCP tools** (what T-Dog uses):
- List runs: `mcp__github__actions_list` → `method: list_workflow_runs`, `resource_id: ci.yml`,
  `repo: Ziptide`, `owner: terrymaloney`. Look at the newest run for your commit SHA → `conclusion`.
  *(The result is large — save/parse the `workflow_runs[].conclusion` + `head_sha`.)*
- On failure, get the error: `mcp__github__get_job_logs` with the job id (or `run_id` +
  `failed_only: true`), `return_content: true`, `tail_lines: ~120`. The C# compile error or failing
  test name is in there.

**B) `gh` CLI** (if available):
- `gh run list --branch terry-local-wip --workflow ci.yml`
- `gh run view <run-id> --log-failed`

## What's already set up (no per-agent setup needed)
- `UNITY_LICENSE` / `UNITY_EMAIL` / `UNITY_PASSWORD` secrets are configured (license fixed — see
  `RECOVERY_STEPS.md`). `ci.yml` has `permissions: checks: write`. You don't touch any of this.

## If your env genuinely can't read CI
Push as usual and **note in `HANDOFF.md` that your push is unverified**; T-Dog actively monitors CI
and will confirm green or flag the error on your commit. But first try (A)/(B) — the capability is
almost certainly already there. Goal: both agents read CI themselves.

## Reality check on "locally unverified"
Pattern-matching C# against existing code is fine for a first pass, but **CI is the truth**. Don't
treat a push as done until you've seen its run go green. If CI is red, the rule (CLAUDE.md) is: warn
Terry loudly and stop shipping more unverified C# until it's green again.
