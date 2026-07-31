# ZIPTIDE — CURRENT EXECUTION CHECKLIST

**Status date:** 2026-07-11  
**Purpose:** the current cross-project checklist for any future operator. Read this after `docs/OPERATOR_START_HERE.md`. It records what is actually complete, what is waiting on Terry, what is blocked by another track, and the next safe work.

> This is the CURRENT status layer, not a replacement for the deeper sources of truth. When detail is needed, follow the linked boards and historical plans below. When this file conflicts with an older unchecked row, prefer the newest implementation log, `HANDOFF.md`, and `CI_VERDICT.md`, then correct this file.


## 2026-07-31 — the last PlayMode red has a real fix; the headset test has not happened yet

Read **`docs/HANDOFF.md` rb133** first. Short version:

- **The `ApplyProcessors` NRE is fixed at its real cause** (`e0fad407`): the repair was correct but
  only ran after a travel, so cold boot and scene-load-only PlayMode tests never swept the
  zero-binding left-hand turn/snap placeholders. Now swept at install, on every scene load, and in
  the post-travel repair (`LocomotionInertActionSweep`). **Not yet proven** — an intermittent defect
  needs the PlayMode lane green ×3 on one SHA before anyone drops the qualifier.
- **Every WorldPack is now validated at build time** (`d55c81fa`), WARN-first — MISS_LEDGER #21's
  open half. Promote `WORLD_PACK_INVALID` to blocker after one clean audit run.
- **Terry did not get on the headset.** `tools/level1_test.ps1` + `tools/quest_capture.ps1` and
  `docs/production/TONIGHT_TEST_CARD.md` (20 ordered beats, honest ✅/🟨/❌) are ready and unchanged.
- Still unbuilt: W002 defend wave / garden plot / glyph plate · pause+settings board ·
  title + legal/credits · all music and VO · all final art.

## ⚠ 2026-07-29 — OPERATOR TAKEOVER IN FLIGHT

Terry is driving home to test on the headset. The full takeover packet is **`docs/HANDOFF.md`
entry rb130** — read it before anything else. Short version:

- Head `30f3ed84`. **CI, Fast Preflight and Golden Android are green (run-level).**
- **One open red, pre-existing and diagnosed:** `Recovery PlayMode Observation` 41/43 — four
  uncatalogued `[RuntimeInitializeOnLoadMethod]` bootstraps. Fix shape is written out in rb130 §4.
  Does not block the headset test.
- **Built today:** the five Catch keepers as geometry · the tender's tool arms (were dead code) ·
  the lighting law derived from the sun bearing · THE BOUNDS LADDER (graduated flight correction +
  40 no-repeat RILL lines) · THE CITY'S COMPASS (lantern route + sightline triple, both CI-enforced
  laws) · `scrap` registered in the economy · two stale evidence tokens refreshed.
- **Not built:** zipline · berths 1–5 quay pads · W002 defend wave/garden/glyph plate · pause board ·
  title/credits · all music and VO · all final art.
- **Nothing is in a scene yet** — Terry must run `LEVEL1_BAKE_AND_SMOKE.md` §1 steps 1–6 in order
  (step 2 before step 3) before any of it exists to play.

## 1. Operating truth

- Branch: `terry-local-wip`.
- Latest gameplay/quality-code proof: `e8d18d67732032d79a2806f1e517d19d1aa9330d`, CI run `29170675599`, Unity EditMode green. Later branch commits are unrelated descendants and preserve this cleanup.
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
   - `docs/MASTER_CHECKLIST.md` — reconciled broad inventory.
   - `docs/FABLE5_BACKLOG.md` — older expanded queue/history; use newer logs for current state.
   - `docs/first_hour/OPUS_LAUNCH_KIT.md` and `docs/first_hour/envelopes/*.json` — exact first-hour ownership/contracts.

## 3. Immediate work order — now through the next PC/headset session

### A. Work that can proceed without Terry or Picasso

- [x] Establish and synchronize the current checklist, `SPRINT.md`, `MASTER_CHECKLIST.md`, and `EXCELLENCE_MAP.md`.
- [x] Implement the locked asynchronous-travel design. **Code/CI green `c7b5d52`, run `29167548682`; device frame-pacing comparison pending.**
- [x] Add the WARN-only UI readability/reach build audit. **Code/CI green `a6803ab`, run `29168358234`; generated-scene/device calibration pending.**
- [x] Write `docs/design/HAPTIC_COVERAGE.md` from inspected existing owners. **Documentation-level gap closed; runtime/device coverage remains deliberately open.**
- [x] Build the canonical creature behavior-readability gate. **Seven shipped IDs, ≥3 active states each, telegraph/counter/resolution, source/factory evidence and APK pre-build blocker; code/CI green `d9b6caf`, run `29169591530`.**
- [x] Build plant and vehicle catalog-breadth audits without touching Picasso visual assets. **Structural CI/APK gate green `b7a6cfa`, run `29170323869`; content warnings remain for unsurfaced plants, three missing vehicle families and no garage.**
- [x] Harden `AudioDirector` transition/disposal ownership. **Retired persistent sources now release clips; overlapping fades are serialized; code/CI green `e8d18d6`, run `29170675599`; long-session Quest memory soak remains device evidence.**
- [x] Exhaust the independent CI-only queue. **PlayMode/travel round-trip work was correctly deferred because the existing workflow proves EditMode only; no unproven second CI lane was introduced.**
- [ ] After each code push, wait for/read `docs/CI_VERDICT.md`; do not stack unverified code.
- [ ] Continue only with explicitly claimed gameplay/content rows or Terry/Picasso-dependent work; the §7 independent queue is complete.

### B. Picasso’s next work when usage returns

- [ ] FORGE III F3.3 commit 3: runtime `ZiptideWater` + device normal baker/build hook.
- [ ] FORGE III F3.3 commit 4: `WaterAuthor` + optional `waterRects`; W001 canals/Tidefront placement.
- [ ] `FH-A01-SIGNATURE-CREATURE-PRESENTATION`: W001 species passport and complete review artifacts.
- [ ] Do not jump to later FORGE III envelopes before the current water envelope is closed and verified.

### C. Terry’s next PC/headset batch

- [ ] Run `Ziptide → First Hour → Author W000 Surfaces`; commit generated `W000_DriftIn.unity`.
- [ ] Run `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS07_TERRY_CHECKLIST.md`.
- [ ] Async travel: run five representative trips; compare crest/frame hitch, verify spawn/XRI/holstered inventory, and check `TRAVEL_TIMEOUT`.
- [ ] Inspect the next build log for `ZIPTIDE: UI_AUDIT`; verify W000/Home/comfort/helm labels and targets in-headset before any audit warning is promoted to a blocker.
- [ ] Use `docs/design/HAPTIC_COVERAGE.md` during the same pass: note whether XRI already pulses on grab/select and which silent P0 verb is most noticeable—holster, release, UI select, repair or zipline.
- [ ] Sample the seven entries in `docs/design/CREATURE_BEHAVIOR_READABILITY.md`; confirm their state changes are actually recognizable and fairly timed in-headset.
- [ ] Inspect `ZIPTIDE: CATALOG_BREADTH_AUDIT` after the authored build. Confirm the three starter rides are present by biome; treat plant/vehicle breadth warnings as named content debt, not build failure.
- [ ] During repeated world travel, inspect `ZIPTIDE: HEALTH_SWEEP ... clips=` and total memory; retired world music should no longer remain pinned by `AudioDirector`.
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
- [ ] Vehicle catalog content: Rover, GravSled, Walker and a garage/catalog surface.
- [ ] Plant catalog surfacing beyond the original three-seed world-pack ladder; reconcile starter asset tending drift.
- [ ] Giant crops/breeding visibly playable.
- [ ] Richer habitat/species behavior and world-state transformation.

### Architect-associated systems

**Done:** WorldSpec/validator/compiler, lot/building grammar, enterable building builder, GamePool core, art registry, reachability gates, first-hour contract/progression core, durable CI verdict, async travel code/CI, synchronized project dashboards, WARN-only UI readability/reach build audit, documentation-level haptic coverage inventory, canonical creature behavior-readability CI/APK gate, plant/vehicle catalog structural CI/APK audit, and `AudioDirector` transition/resource cleanup.

**Remaining:**
- [ ] Terry exports/commits `docs/worldspecs/*.spec.json`.
- [ ] W002 building/interior proof and re-bake.
- [ ] Adopt GamePool at named gameplay/MP hot-spawn sites with device verification.
- [ ] Async-travel device frame-pacing and restoration verification.
- [ ] UI audit generated-scene/device calibration; then decide whether any warning can graduate to blocker.
- [ ] Haptic source/device audit for remaining `❓` owners, then one owner-scoped P0 runtime task—not a broad new system.
- [ ] Catalog breadth content closures: seed-dispenser/almanac/world-pack surfacing plan, deliberate starter-plant migration/reseed, missing vehicle families and garage.
- [ ] Localization architecture decision before large content scaling.

### T-Dog-associated integration work

**Done:** Quality Bar P0–P5, terrain/vistas/POIs/routes/dressing, gardens/build sockets, improved ship hull, tutorial design, async-travel design, broad scene/gameplay integration and device fixes.

**Remaining:**
- [ ] Consolidated Quest test/fix round.
- [ ] Bake pending generated surfaces/scenes.
- [ ] Weapon feel, owner-scoped haptics, UI device readability, collision and comfort tuning.
- [ ] Make the ship the primary world-select/travel hub.
- [ ] Integrate later-world systems and content without duplicate owners.

## 7. Independent CI-only queue while Terry is away

The ordered queue is complete. Item 8 was a conditional feasibility check and correctly stopped because the current workflow has no proven PlayMode lane.

1. [x] Async travel implementation — code/CI green; device comparison pending.
2. [x] Reconcile `SPRINT.md`, `MASTER_CHECKLIST.md`, and `EXCELLENCE_MAP.md` against implementation logs.
3. [x] UI readability/reach audit — WARN-only build-scene processor + tests green; real-scene/device calibration pending.
4. [x] Haptic coverage checklist — evidence inventory and implementation order documented; runtime/device rows remain open.
5. [x] Creature behavior-count/readability gate — one canonical catalog, source/factory evidence, CI tests and APK blocker green.
6. [x] Plant/vehicle catalog breadth audit — structural CI/APK blockers green; exact surfacing/asset/garage debt remains warning-level content work.
7. [x] `AudioDirector` unload/disposal leak hardening — stopped sources release clips, one transition owner, lifecycle tests green `e8d18d6`, run `29170675599`; device memory soak remains.
8. [x] PlayMode scaffold/TravelCoordinator feasibility — **deferred by contract**: existing CI runs `testMode: editmode` only, with no proven PlayMode assembly/job/history. See `GPT_ADDITIONS/2026-07-11_GPT56_PLAYMODE_FEASIBILITY/PLAYMODE_FEASIBILITY_LOG.md`.

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
