# First-contract resume integration — 2026-09-30

## Scope and behavior

W000's generated first contract now opts into mission persistence. WorldJobLibrary assigns revision 1,
three semantic step IDs, placement ID `guild_manifest_primary`, OneTime replay policy and explicit
world-completion ownership. Authoring fails if the canonical contract changes shape. No scene or
prefab YAML was edited; activation requires the normal editor/build generation path.

JobDirector owns MissionCheckpointSession and restores it before spawning content. The profile stores
logical objective progress and early-action banks, consumed pickup placements, and all four coupler
stages. Consumed pickups are omitted; repair presentation restores silently without replaying
interaction events, audio, haptics or repair credit. Accepting an active/completed contract cannot reset it.
A profile-reference guard prevents a departing scene from writing into a New Game/Load replacement.

Schema 5 adds per-world mission receipts, independent of the pruned transaction ledger. Checkpoint
format 2 adds physical state. Unknown versions, mismatched identities/revisions, duplicate records,
invalid stages and logical/physical disagreement are rejected without overwriting the checkpoint.
Unactivated format-1 foundation snapshots are not silently upgraded into physical progress.

MissionRewards validates every reward and aggregate balance before any payout, completion flag or
receipt mutation. One-time jobs pay once per world/job; the transaction API also supports explicit
repeatable run IDs. Existing one-time completion flags migrate to receipts without repayment.
W000 intentionally pays zero credits; tests use nonzero rewards to exercise duplicate-payment protection.

Normal interaction saves settle in LateUpdate after tutorial listeners. SaveSystem's BeforeSave hook
also captures pending mission state for pause, quit and travel. SaveFileStore remains the single atomic
writer: failed preparation aborts the disk write, failed IO preserves the old disk state and timestamp,
and live receipts make retries safe. Save preparation must be synchronous and cannot recursively save.

## Evidence

- Starting source c34002db: Unity EditMode and scene patch/audit green in CI 36662089872.
- Added 26 NUnit cases covering early actions, all repair stages, logical/physical disagreement,
  malformed snapshots, schema migration, reward validation/overflow, world/run scope, ledger pruning,
  legacy import, disk failure/retry/backup and save preparation. Four presentation cases inject
  references without XR selection/physics; they prove silent stage visibility, not headset interaction.
- Local full preflight: 244 Python tests, 25 PowerShell operator checks, governance/readiness and
  whitespace checks passed. These do not compile C# or execute NUnit.
- This integration requires its own Unity EditMode and scene patch/audit result after publication.
  Android build and headset verification remain open. Do not treat baseline CI as candidate proof.

## Boundaries and next gates

Only a single explicitly configured one-time job per pack is supported by the scene adapter. Other
worlds retain their prior behavior. Repeatable scene run creation, campaign-wide migration and shared
physical sources across multiple jobs are not enabled. SAVE-03/04/05/07 remain open at project scope.

This resumes W000 when entered through existing travel; it does not add last-scene or player-position
restoration. The existing global first-hour New Game reset gap and travel's save-failure policy are
separate open work. Hardware checks must cover cold reload at each repair stage, pickup-before-job,
completion/revisit, pause/resume, tutorial continuity and New Game scene transition.

Next: inspect exact-source Unity results, fix any concrete failures before further gameplay expansion,
then generate/build and perform the deferred device sequence in TERRY_RUNBOOK.md. Extend the policy
only after that first integrated contract is verified.
