# Mission persistence contract — proposed implementation sequence

Status: design, not implemented. Based on the [source audit](STATE_OWNERSHIP_AUDIT_20260929.md). Tracks SAVE-02/03/04/05/07/08/09. Continue using PlayerProfile, ProfileSerializer, SaveFileStore, JobRuntime, JobDirector and RewardRouter.

## Identity and compatibility

- Resolve one canonical world key. Existing overlays use scene name while job content also exposes packId; inventory current aliases and collisions before migration. Do not silently allocate two WorldState records for one world.
- Scope job identity by canonical world and authored jobId. Give steps stable authored IDs and jobs a definition revision. Step labels, list indices, object names and positions are not durable identity.
- Give each collectible placement and repair machine a stable world-scoped ID. Repeated items with the same itemId remain separate placements.
- Persist a run identifier only for explicitly repeatable jobs. Content must declare one-time, repeatable or another concrete policy; do not infer it from a blank completionFlag.
- Add serializable lists to the existing profile schema with neutral defaults. Old saves retain balances/flags; do not fabricate partial progress or assume an old completion flag proves a payout receipt without a verified mapping.
- Unknown IDs and changed revisions must retain recoverable data, emit diagnostics and follow an explicit migration/reset rule. Reordering a step must not silently redirect saved counters to another objective.

## Snapshot contents and restore ordering

A mission snapshot needs scoped job/run identity, definition revision, current stable step ID, typed counters, completion status, early collect/repair banks, consumed placement IDs and meaningful repair stages. Store logical state, not Unity object references or transforms. Keep existing mine/garden/factory overlays authoritative for their domains.

Capture all mutations, including reports banked before a job starts. StepChanged alone is insufficient. Restoration must install the saved state without emitting completion/reward callbacks; afterward presentation reads that state. Subscribe/restore/spawn ordering must prevent spawned pickups or repair callbacks from inventing new progress during reconstruction.

Within one run, repeat delivery of the same identified event must be harmless. A second legitimate placement/event must still count. Counter clamping alone does not distinguish duplicates from legitimate repeated actions.

## Completion and durable receipts

The pure completion operation validates the entire reward batch and policy first. It checks a durable receipt scoped to world/job/run, then changes mission status, balances, ledger entries, completion flags and receipt together in the same profile transaction. A partial reward batch must not leave a receipt marking full success. All resource changes continue through RewardRouter.

Receipts are persistent completion data, separate from the 500-entry ledger. One-time receipts must survive ledger pruning. Repeatable runs require deliberate new-run creation; scene reload and duplicate callbacks cannot create a run.

Save the coherent profile through the existing writer. A write failure must report failure and preserve a retryable state. Retrying persistence must not reapply the in-memory reward. After restart, the last valid main/backup snapshot determines both mission and reward state together. This allows rollback to the last successful checkpoint after a crash; it is not a promise to retain unsaved progress.

World flags are evaluated by a separate explicit world-completion rule. Completing an optional job cannot automatically award every pack flag.

## Profile reset and presentation

A new profile must detach or reset cached job/world/tutorial state before the next mutation. No old runtime object may write its previous run into the new profile. Inventory/travel ownership changes require a separately reviewed bounded task.

Boards, machine stages, collectible visibility, choices, doors and RILL should derive from restored logical state. RepairableMachine remains a presentation/interaction component; a gameplay adapter owns persistence. Replaying visuals must not replay economy effects.

## Acceptance matrix

| Case | Required result |
|---|---|
| Early pickup/repair, save, restart, then start job | Bank and physical state agree; event counted once |
| Each supported objective partially complete, leave/re-enter | Same stable step and counters restored |
| Duplicate completion callback before and after reload | One payout and receipt for that run |
| More than 500 unrelated ledger entries | One-time receipt still prevents another payout |
| Explicit second repeatable run | New run can pay once; prior run remains closed |
| Invalid/overflowing reward inside multi-resource batch | No partial completion, balance, ledger or receipt mutation |
| Crash before successful write / after successful write | Entire old snapshot / entire new snapshot recovered |
| Failed disk write followed by retry | Failure observable; no second in-memory payout |
| Invalid main, valid backup | Restore a coherent older mission/reward snapshot |
| Old schema, missing content, reordered definitions | Deliberate migration/fallback, no invented rewards |
| New Game with live mission/tutorial objects | No old-run state leaks into the replacement profile |
| Restored repair/choice/pickup state | Presentation and usable objective agree |

Implement tests for the pure model first, then serializer fixtures, runtime adapter tests, and finally headset interruption routes. This design does not close any device acceptance gate.
