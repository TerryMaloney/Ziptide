# ZIPTIDE — CURRENT EXECUTION CHECKLIST

**Status date:** 2026-07-11  
**Purpose:** the current cross-project checklist for any future operator. Read this after `docs/OPERATOR_START_HERE.md`. It records what is actually complete, what is waiting on Terry, what is blocked by another track, and the next safe work.

> This is the CURRENT status layer, not a replacement for the deeper sources of truth. When detail is needed, follow the linked boards and historical plans below. When this file conflicts with an older unchecked row, prefer the newest implementation log, `HANDOFF.md`, and `CI_VERDICT.md`, then correct this file.

## 1. Operating truth

- Branch: `terry-local-wip`.
- Latest gameplay-code proof: `c7b5d524b710b767a071264d6a1c552136bf08a0`, CI run `29167548682`, Unity EditMode green.
- Picasso is the active independent Art/Forge track and owns `Visuals/**`, Forge, water, art authors/audits, and `SPRINT_ART.md`.
- Architect, T-Dog, and Reasonbox are useful historical workstream names, not exclusive permanent accounts. The official model is one non-art operator plus Terry, while active concurrent claims still control file ownership.
- Never hand-edit `.unity` or `.prefab` YAML. Write an idempotent author/patcher; Terry runs it and commits generated artifacts.
- CI red is priority zero. Three reds on one task triggers the circuit breaker.

## 2. Read order and previous checklists

1. `docs/OPERATOR_START_HERE.md` — laws, verification, ownership, definition of done.
2. **This file** — current cross-project status and order.
3. `docs/CI_VERDICT.md` — whether the latest tested gameplay head is green.
4. Active board for the work:
   - General Story/Ship/gameplay: `docs/SPRINT.md`
   - Art/Picasso: `docs/SPRINT_ART.md`
   - Multiplayer: `docs/SPRINT_MULTIPLAYER.md`
   - Architecture/history: `docs/SPRINT_ARCHITECTURE.md`
5. `docs/HANDOFF.md` newest entries — current claims and collisions.
6. Deeper roadmap/history when needed:
   - `docs/GAME_PLAN.md` — milestone roadmap M0–M8.
   - `docs/EXCELLENCE_MAP.md` — quality state and missing gates.
   - `docs/PRIORITIES.md` — prior cross-track ordering.
   - `docs/MASTER_CHECKLIST.md` and `docs/FABLE5_BACKLOG.md` — older broad checklists; useful history, but some rows lag newer implementation logs.
   - `docs/first_hour/OPUS_LAUNCH_KIT.md` and `docs/first_hour/envelopes/*.json` — exact first-hour ownership/contracts.

## 3. Immediate work order — now through the next PC/headset session

### A. Work that can proceed without Terry or Picasso

- [ ] Keep this checklist and the active board synchronized whenever a row changes.
- [x] Implement the locked asynchronous-travel design in `docs/design/ASYNC_TRAVEL.md`, preserving `TravelCoordinator` as the sole owner. **Code/CI green `c7b5d52`, run `29167548682`; device frame-pacing comparison pending.**
- [ ] After each code push, wait for/read `docs/CI_VERDICT.md`; do not stack unverified code.
- [ ] Take another independent CI-only gate/quality task from §7 rather than entering Picasso files or blocked first-hour orchestration.

### B. Picasso’s next work when usage returns

- [ ] FORGE III F3.3 commit 3: runtime `ZiptideWater` + device normal baker/build hook.
- [ ] FORGE III F3.3 commit 4: `WaterAuthor` + optional `waterRects`; W001 canals/Tidefront placement.
- [ ] `FH-A01-SIGNATURE-CREATURE-PRESENTATION`: W001 species passport and complete review artifacts.
- [ ] Do not jump to later FORGE III envelopes before the current water envelope is closed and verified.

### C. Terry’s next PC/headset batch

- [ ] Run `Ziptide → First Hour → Author W000 Surfaces`; commit generated `W000_DriftIn.unity`.
- [ ] Run `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS07_TERRY_CHECKLIST.md`.
- [ ] Async travel: run five representative world-to-world trips; compare crest/frame hitch, verify spawn/XRI/holstered inventory, and check for `TRAVEL_TIMEOUT`. Details: `docs/GPT_ADDITIONS/2026-07-11_GPT56_ASYNC_TRAVEL/ASYNC_TRAVEL_IMPLEMENTATION_LOG.md`.
- [ ] Clear queued one-time authors/rebakes in `docs/TERRY_RUNBOOK.md`, including W002 interiors/WorldSpecs/SpaceLane as applicable.
- [ ] Build/install the consolidated APK and perform the device checklist; blocker findings reorder all model work.
- [ ] Start Meta paperwork that does not depend on code: developer app ID, privacy URL, Data Use Checkup, IARC.
- [ ] Decide English-only launch versus a text-table/localization seam before W013–W080 scale.

## 4. First-hour vertical slice

| Envelope | Current state | Remaining gate |
|---|---|---|
| FH-X01 contract asset | ✅ CI green | none |
| FH-X02 progression core | ✅ CI green | none |
| FH-S01 observation | ✅ CI green | headset evidence |
| FH-S02 holster | ✅ CI green | headset evidence |
| FH-S03 travel | ✅ CI green | headset evidence |
| FH-M01 scanner result | ✅ CI green | normal scanner device check |
| FH-S04 repair/scan | ✅ CI green | headset repair/identity check |
| FH-S05 creature resolution | ⛔ unbuilt | wait for Picasso FH-A01, then one additive creature-disable adapter |
| FH-S06 zipline | ✅ CI green | headset arrival/release check |
| FH-S07 Home/W000 surfaces | 🟡 code + CI green | Terry author bake + headset check |
| FH-A01 signature creature | ⛔ Picasso queue | art/CI/bake/device acceptance |
| FH-S08 W001 orchestration | ⛔ final integration | every dependency above + required bake/device evidence |

**First-hour next order:** Picasso FH-A01 → Story/Ship FH-S05 → Terry bakes/tests accumulated surfaces → FH-S08 final orchestration only after all dependency evidence exists.

## 5. Picasso / art status

### Complete or substantially complete

- [x] ART-1 skyscapes and atmosphere foundation.
- [x] ART-2 Asset Forge and forged-asset photo/CI loop.
- [x] FORGE II: textures/baking, weapons, avatar pieces, building kit, flora, class budgets, six forged creature bodies.
- [x] FORGE III F3.1 light script.
- [x] FORGE III F3.2 grade.
- [x] FORGE III F3.1b practical fixtures, halos, pools, and deterministic placement.
- [x] FORGE III F3.3 water surface/motion/foam booth proof.

### Short term

- [ ] Finish F3.3 runtime and authoring.
- [ ] Build FH-A01 signature creature.
- [ ] F3.4 grounding decals and blob shadows.
- [ ] F3.5 VFX vocabulary.
- [ ] F3.6 reactive props.

### Midterm

- [ ] F3.7 signage/wayfinding.
- [ ] F3.8 macro variation.
- [ ] F3.9 art-conformance ratchet; pilot W002 to zero unconformed visuals.
- [ ] Finish/device-verify remaining creature motion polish through `CREATURE_QUALITY_V2_LIFE_LEAP.md` after FORGE III.

### Long term

- [ ] W001 and Chapter 1 shipped-quality at 72 FPS.
- [ ] VO/audio/VFX integration and stronger world-presence layers.
- [ ] Consider FORGE IV diegetic UI art only in a Terry-approved cross-lane window.

## 6. Historical workstreams — done and remaining

### Reasonbox-associated systems

**Done:** free-flight/ship handling, space combat v1, ground vehicles v1, garden genetics/plant systems, ecology scheduling/emerge-burrow, furnished interiors and per-room culling.

**Remaining:**
- [ ] Device proof and tuning for flight, SpaceLane, vehicles, gardens, ecology, and interiors.
- [ ] Space-enemy variety.
- [ ] Vehicle catalog breadth + garage.
- [ ] Giant crops/breeding visibly playable.
- [ ] Richer habitat/species behavior and world-state transformation.

### Architect-associated systems

**Done:** WorldSpec/validator/compiler, lot/building grammar, enterable building builder, GamePool core, art registry, reachability gates, first-hour contract/progression core, durable CI verdict, **async travel code/CI**.

**Remaining:**
- [ ] Terry exports/commits `docs/worldspecs/*.spec.json`.
- [ ] W002 building/interior proof and re-bake.
- [ ] Adopt GamePool at named gameplay/MP hot-spawn sites with device verification.
- [ ] Async-travel device frame-pacing and restoration verification.
- [ ] UI readability, haptic coverage, behavior-count, and catalog-breadth gates.
- [ ] Localization architecture decision before large content scaling.

### T-Dog-associated integration work

**Done:** Quality Bar P0–P5, terrain/vistas/POIs/routes/dressing, gardens/build sockets, improved ship hull, tutorial design, async-travel design, broad scene/gameplay integration and device fixes.

**Remaining:**
- [ ] Consolidated Quest test/fix round.
- [ ] Bake pending generated surfaces/scenes.
- [ ] Weapon feel, haptics, UI reach/readability, collision and comfort tuning.
- [ ] Make the ship the primary world-select/travel hub.
- [ ] Integrate later-world systems and content without duplicate owners.

## 7. Independent CI-only queue while Terry is away

Take in order unless a live claim or CI result changes it:

1. [x] Async travel implementation (`docs/design/ASYNC_TRAVEL.md`) — code/CI green; device comparison pending.
2. [ ] Reconcile stale current-state rows in `SPRINT.md`, `MASTER_CHECKLIST.md`, and `EXCELLENCE_MAP.md` using implementation logs—not guesses.
3. [ ] UI readability/reach audit for TextMesh and interactive tiles.
4. [ ] Haptic coverage checklist document; do not invent a registry unless separately approved.
5. [ ] Creature behavior-count gate: every shipped species maps to at least three readable states.
6. [ ] Plant/vehicle catalog breadth audits.
7. [ ] `AudioDirector` unload/disposal leak hardening if still open after rechecking live code.
8. [ ] PlayMode scaffold/TravelCoordinator round-trip test only if stable in the existing CI environment.

Do not take:

- Picasso-owned `Visuals/**`, water, Forge, art authors/audits, or `SPRINT_ART.md`.
- FH-S05 before FH-A01.
- FH-S08 before all dependency and device gates.
- Photon/hardware-dependent work without Terry’s two-headset setup.
- Scene YAML edits.

## 8. Midterm project checklist

- [ ] W001/Chapter 1 art and sound at shipped quality.
- [x] Asynchronous travel code/CI; **device comfort/frame-pacing gate remains open**.
- [ ] Make credits meaningfully spendable across ship, tools, vehicles, garden/factory systems.
- [ ] Expand ship/space-combat enemy and mission variety.
- [ ] Finish vehicle garage/catalog and garden/ecology breadth.
- [ ] Complete Photon combat sync, room-code UX, avatars/voice when hardware is available.
- [ ] Hazard stingers, music stems, RILL/Transmission VO pipeline.
- [ ] Continue WorldSpec-driven W013+ authoring only after system hooks and quality gates are stable.

## 9. Long-term checklist

- [ ] Author and audit W013–W080 against the locked story bible.
- [ ] Complete all campaign branches, Transmission fragments, and four endings.
- [ ] Ship the ship as the persistent hub/world selector and progression sink.
- [ ] Complete multiplayer Arena, Photon online, and Tidefront program.
- [ ] Performance/soak hardening across all worlds and modes.
- [ ] Accessibility, localization decision, release-build hygiene, entitlement/Platform SDK.
- [ ] Meta Store assets, certification, QA matrix, and accepted submission.

## 10. Update contract

Every operator closing a meaningful chunk must:

1. Check off or rewrite the relevant row here.
2. Update its active sprint board in the same commit when required by that board.
3. Add a `HANDOFF.md` entry with Did / Next / Heads-up / Commits.
4. Add Terry-only bake/device work to `TERRY_RUNBOOK.md`.
5. Preserve links to older checklists rather than deleting history.
6. Record CI/test/device evidence honestly: code-green is not device-green.
