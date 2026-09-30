# Mission identity and replay audit — 2026-09-29

Baseline: ed82c7bd022c9fcd751eab04fc30e9a5ad8122f3. Unity CI 36621171988 is green (EditMode and scene audit); Android skipped. This is source/committed-asset evidence, not an inventory of all generated runtime worlds. Relates to SAVE-02/05 and the proposed mission persistence contract.

## What exists

A read-only scan of `Ziptide/Assets/Ziptide/**/*.asset`, matching the JobDefinition and WorldPackDefinition script GUIDs, found **14 committed job assets and 42 pack assets**. The 14 job IDs are distinct in those files. This is not a global uniqueness guarantee: JobDefinition's declared scope is within a pack. Thirteen have completion flags; DroneTutorialJob does not. No definition has an explicit replay policy, revision or stable step ID.

| Authoring source | Committed job IDs | Checkpoint implication |
|---|---|---|
| `Editor/Patching/WorldJobLibrary.cs` | w000_onboard, w002_pumps, w003_baffles, w004_broadcast, w005_harvest, w006_prisms, w007_fuelrig, w008_archive, w009_pylons, w010_turbines, w011_resonators, w012_stabilize | Generated step asset names use `_S1`, `_S2`, etc. Reordering steps changes semantic meaning at a path. Asset path/index cannot substitute for authored stable step identity. |
| `Editor/Patching/ToxicCityContractBuilder.cs` | toxiccity_contract | Hand-authored contract also needs an explicit policy and authored step identities. Its completion flag alone is not proof of reward payment. |
| `Content/Jobs/DroneTutorialJob.asset` | drone_tutorial | No completion flag. Do not infer repeatability or award a new payout merely because the flag is blank. |

Source paths in this table are relative to `Ziptide/Assets/Ziptide/`.

## Pack identity is not automatically world-save identity

All 42 scanned pack IDs differ from their sceneName fields. Some are normal spelling aliases; others are destination packs. Examples from committed data:

| packId | sceneName | Required treatment |
|---|---|---|
| toxic_city | ToxicCity | Resolve identity against the owning JobDirector scene before checkpointing. |
| w000_drift_in | W000_DriftIn | Existing overlay lookup uses scene name; preserve that ownership. |
| toxic_city_exit | W000_DriftIn | Destination descriptor; do not migrate a ToxicCity checkpoint into W000 based on this field. |
| dry_cistern_exit | MilestoneA_GrabCube | Destination descriptor; not a saved Dry Cistern world record. |
| test_room | _Boot | Do not interpret this as permission to make _Boot a travel target or mission-save world. |

Before adding an alias map, inventory **which pack is assigned to each JobDirector**, separately from door/destination references. Mapping every pack indiscriminately would combine unrelated state. No aliases or existing save keys are changed in this batch.

## Replay behavior today

`JobDirector.StartJobByIndex` calls StartJob whenever a valid job index is selected; it does not check a persisted completion receipt. JobRuntime prevents another completion callback after the current run is complete, but starting the job again resets that guard. JobRewards grants resources again. Profile flags being idempotent does not make the payout idempotent.

Proposed policy for review during implementation: authored campaign contracts should declare one-time economic completion; optional replay may reset a run's objectives without granting another campaign reward. Deliberately repeatable jobs should create explicit run IDs and receipt rules. **This proposal is not applied to existing content.** Distinguish reward replay from physical pickup/repair reset and from world-completion flag ownership.

## Three bounded fixes accompanying this audit

1. JobRuntime now rebuilds StepText before notifying listeners of partial collect/deliver/shoot/drone/repair progress. Five parameterized tests verify listeners see 1/2, completion occurs once, and later reports do not repeat completion.
2. WorldPackValidator reports duplicate job IDs within a pack, duplicate machine IDs and duplicate spawn-marker IDs. Same job IDs in different packs remain legal. Four cases verify the scope.
3. Collect validation sums requirements across ordered steps of each job, treats a null pickup list as no supply, and uses a long total to avoid integer overflow. Five cases cover shortage, sufficient repeated pickups, overflow, null supply and separate-job scope.

The validator is consumed by existing JobDirector diagnostics and WorldPackAuditRules (warning severity). These changes add no new blocker policy, save schema or runtime persistence. Repeated collectible item types remain legal; they are not placement IDs. The static supply check assumes pack-authored pickups, as before; it does not model external item sources or campaign-wide selection/replay.

## Next bounded implementation

- Add explicit step and placement IDs to authoring sources, plus a definition revision and declared replay policy. Preserve those IDs across reordering; reject duplicates in the owning scope.
- Verify the JobDirector-to-pack identity map in generated content.
- Build pure snapshot/receipt tests before wiring capture/restore. Restore must not invoke reward callbacks.
- Keep SAVE-02/03/04/05/07 open until their full acceptance criteria are met. This batch does not provide mission resume.
