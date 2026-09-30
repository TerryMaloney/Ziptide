# 2026-09-29 — Checkpoint foundation

Scope announced before shared profile edits: append WorldState.jobCheckpoints and schema-4 migration;
add a Core checkpoint DTO, authored job revision/step IDs, and opt-in JobRuntime capture/restore APIs.
No automatic JobDirector save/restore yet: physical pickup/repair state and payout receipts must be
integrated before scene restoration can be enabled. Existing content remains revision 0 (unconfigured).
Tests must cover serialization, old-save defaults, partial objectives, early banks, mismatched data,
no reward callbacks during restore, and rejected restore leaving the destination runtime unchanged.

Implemented:
- Core JobCheckpoint DTO (format version 1), ordered step contract, bank count records.
- JobDefinition.checkpointRevision (zero remains unconfigured) and JobStepDefinition.stepId.
- WorldState.jobCheckpoints; PlayerProfile schema 4; serializer initializes empty lists in legacy worlds.
- JobRuntime.TryCaptureCheckpoint / TryRestoreCheckpoint in a partial class. Scope, format, revision,
  ordered IDs/types/targets/counts/arrival distances, progress and banks validate before mutation.
  Capture/restore copy mutable lists; restore does not drain banks or invoke gameplay/payout events.
- 28 NUnit cases: six objective round trips through PlayerProfile, pre-accept banks, silent completed
  restore, fourteen corrupt/mismatched snapshots, reorder rejection, independent copies, three
  unconfigured-definition cases, schema-3 migration preserving balances and flags.

Boundaries / next:
- No existing authored mission has opted in; no automatic checkpoint writes or restores in JobDirector.
- Current profile schema can carry checkpoints, but this alone does not provide gameplay resume.
- This is logical progress only, not consumed pickup/repair presentation state, event deduplication,
  run creation/replay policy, or a payout receipt. Those remain SAVE-03/04/05/07 integration work.
- Caller must bind world identity to the owning scene and supply a deliberate run ID. Exit/destination
  packs are not world-save identities. Restore returns a diagnostic code; the future adapter owns logs.
- Definition changes are rejected rather than migrated automatically; retain rejected data for recovery.
- Finish explicit authored IDs and replay policy on the first contract, then receipts/physical-state
  transaction tests before enabling runtime restore.

Local validation: full dev_preflight.ps1 passed (governance, 244 Python gate tests, readiness reports,
25 Quest operator checks, whitespace). The new C# tests require fresh Unity CI; local preflight is not
Unity proof. Source baseline ed82c7bd has full green CI; preceding mission-guard candidate 0a434be1 was
still in Unity EditMode when this checkpoint foundation was prepared.

Before publication: preceding mission-guard candidate 0a434be1 passed Unity EditMode in run
36653015899; its scene audit was still in progress. New checkpoint candidate still requires its own CI.
