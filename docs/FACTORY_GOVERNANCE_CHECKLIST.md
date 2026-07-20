# FACTORY GOVERNANCE CHECKLIST — every build round

**Stage:** 9 — Benchmark & Cert *(reused as a cross-stage ratchet)*  
**Type:** law

This is a standing gate, not a planning memo. `FactoryGovernanceFilesTests` reads it on every
EditMode run. A world/gameplay round is not complete until these rows remain structurally present
and its new misses have been fed through the Class Law. This checklist does not reorder
`docs/PIPELINE.md`; its full benchmark verdict closes at Stage 9 while its structural checks run
earlier to prevent late discovery.

## Start of every operator session

- [ ] Read `docs/PIPELINE.md` for the current stage, allowed overlap and checkpoint block.
- [ ] Read `docs/HANDOFF.md` newest-first.
- [ ] Read `docs/MISS_LEDGER.md`; do not close an entry until its SYSTEM CHANGE is verified.
- [ ] Read the applicable `docs/EXCELLENCE_MAP.md` rows and current improvement compile evidence.
- [ ] Confirm the claimed lane does not collide with another operator.

## Close of every improvement round

- [ ] Exact source receives ordinary CI, generated-scene audit, required PlayMode route and exact-profile build evidence.
- [ ] `world_improvement_compile.json` identifies current recipe hashes, module output and weakest aspects.
- [ ] Every newly discovered miss records **WHAT · FOUND BY · WHY MISSED · CLASS · SYSTEM CHANGE**.
- [ ] A symptom fix without a verified system change remains OPEN.
- [ ] Human headset time is reserved for feel, comfort and fun—not machine-detectable omissions.
- [ ] No work claims a later pipeline stage or checkpoint without a same-commit `docs/PIPELINE.md` amendment and version bump.

## Finished-game ship-backward benchmark

The canonical detailed benchmark is `docs/design/FINISHED_GAME_BENCHMARK.md`; the B1–B10 source
is `docs/design/FINISHED_GAME_BENCHMARK_RB.md`. Re-score these at every major milestone and before
any feature/content freeze. A category may be green, explicitly cut, or Terry-decision pending; it
may not silently disappear.

| ID | Finished-game category | Standing evidence / owner seam |
|---|---|---|
| B1 | Feature complete | scope board + explicit cut list + ship bar |
| B2 | Content complete | world/content genomes + assembly report |
| B3 | Presentation complete | title/settings/credits/legal/error-state presentation |
| B4 | Quality bars met | CI, audits, performance/soak, save and cert evidence |
| B5 | Player lifecycle complete | first hour, midgame, ending experience, completion/replay state |
| B6 | Compliance & legal | rating/privacy/licenses/name clearance/platform requirements |
| B7 | Store & marketing | listing kit, capture plan, trailer/screenshots/campaign calendar |
| B8 | Business decided | Terry-owned price/discount/monetization decisions on record |
| B9 | Live operation ready | patch/triage/support/review/update policy |
| B10 | Preservation & ops | keystore/backups/reproducible builds/migrations/source safety |

## Class Law acceptance

1. The ledger contains all five mandatory fields for every numbered OPEN entry.
2. B1 through B10 remain present in the benchmark and in this recurring checklist.
3. The CI gate fails when either structure disappears.
4. Closing a ledger entry names the exact implemented gate, checklist row, genome field or law.
