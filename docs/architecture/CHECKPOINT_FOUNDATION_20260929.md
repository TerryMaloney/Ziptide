> **2026-09-30 implementation:** [W000 integration, verification and rollout limits](FIRST_CONTRACT_RESUME_20260930.md). The baseline/design below is historical.

# Logical checkpoint foundation — implemented API, runtime integration pending

This is the first implementation stage of [MISSION_PERSISTENCE_CONTRACT.md](MISSION_PERSISTENCE_CONTRACT.md).
It does not yet enable automatic scene resume. Existing jobs retain revision zero and current behavior.

## Ownership and schema

PlayerProfile schema 4 adds `WorldState.jobCheckpoints`. Legacy worlds receive empty lists; flags,
resources and existing overlays remain intact. Checkpoints carry their own format version 1, canonical
world scope, caller-supplied run ID, job ID, authored definition revision, current step ID, completion,
current counter, ordered step contract and early collect/repair banks. The record contains no Unity
objects and no reward receipt. The repeated worldId is a validation guard against using a record in a
different world; the future owner must verify it against the containing WorldState.

`JobDefinition.checkpointRevision > 0` opts a definition into the API; every step then requires a
nonblank unique `stepId`. These are stable authoring fields, never synthesized from indices/labels.
An ordered semantic contract records step kind, target IDs, required count and arrival distance so
changing or reordering a definition without bumping its revision cannot silently reinterpret progress.
Unknown step types are rejected until explicitly supported.

## Runtime APIs

- `TryCaptureCheckpoint(worldId, runId, out checkpoint, out error)` returns an independent snapshot.
  A runtime with no job can capture early banks. An active legacy/unconfigured job returns false.
- `TryRestoreCheckpoint(definition, worldId, runId, checkpoint, out error)` validates all input before
  modifying the destination. It restores counters, step text and banks without firing StepChanged or
  JobCompleted and without consuming banked work. Presentation must be refreshed by the owner afterward.
- Restore failure returns a diagnostic code and leaves the destination untouched. No automatic reset,
  migration, payout, scene spawn or file IO occurs. Caller retains rejected records for recovery.

A completed checkpoint is not proof that rewards were paid. Do not add a caller that grants rewards
on restore. Completion receipts and in-memory/disk transaction behavior must be implemented together.

## Acceptance covered by the new test suite

28 cases cover six objective kinds through profile JSON, pre-accept work, completed restore without
callbacks, world/run/job/revision/format/step/target/type/count mismatch, bad counters/banks, content
reordering, copy isolation, unconfigured definitions and legacy save migration. These are authored
NUnit tests; consult the exact-source CI result before calling them verified.

## Remaining integration sequence

1. Author stable IDs and a declared replay policy for the first supported contract; map its owning
   JobDirector scene separately from destination packs. Select explicit pending/accepted run identity.
2. Add durable completion receipts and coherent reward batches using the existing economy owner.
3. Capture logical mutations (including bank-only events) together with consumed placements and repair
   stages. Add restore adapters and profile replacement lifecycle handling.
4. Wire JobDirector to the profile and observable save outcome; refresh presentation without replaying
   interactions. Test failure/retry, reload, duplicate callbacks and ledger pruning.
5. Headset interruption acceptance. SAVE-03 and mission resume remain open until these integrations pass.
