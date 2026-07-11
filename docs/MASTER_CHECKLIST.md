# ZIPTIDE — MASTER CHECKLIST

**Broad inventory of what exists and where the project is headed.**  
**Status reconciled:** 2026-07-11

> **Current execution truth:** start with [`CURRENT_EXECUTION_CHECKLIST.md`](CURRENT_EXECUTION_CHECKLIST.md).
> That file carries the live done/next/blocked order and must be updated after meaningful work. This
> Master Checklist is the broader system inventory. For historical detail, use
> [`FABLE5_BACKLOG.md`](FABLE5_BACKLOG.md), the four `SPRINT*.md` boards, and `HANDOFF.md`.

- **Status:** ✅ complete/verified · 🟢 code/CI built, device or bake evidence pending · 🟡 actively incomplete · 🔲 planned · 🔭 long-term.
- **Reality:** much of the game is mechanically substantial but still graybox or awaiting consolidated Quest testing.
- **North star:** the Ship becomes the persistent home, world selector, travel presentation, progression sink, and bridge between every mode.
- **Latest gameplay-code proof at reconciliation:** async travel tested green at `c7b5d52`, CI run `29167548682`.

## Sources of truth

- Current order/status: [`CURRENT_EXECUTION_CHECKLIST.md`](CURRENT_EXECUTION_CHECKLIST.md)
- Quality state and missing gates: [`EXCELLENCE_MAP.md`](EXCELLENCE_MAP.md)
- Milestone roadmap M0–M8: [`GAME_PLAN.md`](GAME_PLAN.md)
- Terry’s batched Unity/headset work: [`TERRY_RUNBOOK.md`](TERRY_RUNBOOK.md)
- General gameplay/Story/Ship board: [`SPRINT.md`](SPRINT.md)
- Art/Picasso: [`SPRINT_ART.md`](SPRINT_ART.md)
- Multiplayer: [`SPRINT_MULTIPLAYER.md`](SPRINT_MULTIPLAYER.md)
- Architecture history: [`SPRINT_ARCHITECTURE.md`](SPRINT_ARCHITECTURE.md)
- Deep 80-world vision: [`ZIPTIDE_MASTER_BUILD_PLAN.md`](ZIPTIDE_MASTER_BUILD_PLAN.md)

---

## ✅ FOUNDATION BUILT

### Workflow, safety, and continuity

- ✅ CI compiles and runs Unity EditMode tests on pushes; durable [`CI_VERDICT.md`](CI_VERDICT.md) records the tested SHA.
- ✅ Cloud APK pipeline, build-time authors/patchers, world audits, performance budgets, wiring guards, and circuit breaker.
- ✅ Current execution checklist, sprint boards, cross-track handoffs, and Terry runbook preserve continuity across context/model resets.
- ✅ Atomic profile writes with backup recovery; travel-target pre-flight; runtime frame/memory health logs and cleanup discipline.
- ✅ No direct scene/prefab YAML workflow: deterministic patchers/authors generate assets and scenes.

### Core data, saves, economy, and factories

- ✅ `PlayerProfile`, serializer, `SaveSystem`, autosave, backup recovery, and explicit New/Continue boot behavior.
- ✅ Definition registries/factories for resources, tools, machines, plants, creatures, recipes, balance, weapons, ships, and art modules.
- ✅ One-economy ledger + `RewardRouter`; job, PvP, conquest, salvage, belt, and progression rewards route through common chokepoints.
- ✅ Offline/world-entry economy resolution, mining, gardens, build sockets, factories, physical belts, blueprint/stamp, conductor riding, and persisted overlays.
- ✅ WorldSpec/compiler/validator, World Factory, deterministic terrain/POI/scatter/building/interior generation, and audit gates.

### VR player and interaction

- 🟢 Locomotion, snap/smooth turning, dash/jump, climbing, zipline, lift, jump pad, grapple, emergency respawn, and movement-owner suspension patterns; device tuning remains.
- 🟢 Hands, rays, grab/release rescue, belt/holsters, starter tools/weapons, scanner, haptics in several interactions, and persistent holstered inventory.
- 🟢 Device-level Cozy/Standard/Bold comfort presets, comfort console, vignette and traversal dials; W000 bake/headset evidence remains.
- 🟢 Cold-boot Home Hub with New Game, valid-save Continue, and Settings; code/CI green, device evidence pending.

### Travel and ship

- ✅ One travel owner: `TravelCoordinator`; `_Boot` remains persistent and never a destination.
- ✅ Missing-scene pre-flight aborts before save/rig side effects.
- ✅ Destination loading is asynchronous behind THE ZIPTIDE crest, activation held until ready, with a 20-second never-wedge escape; code/CI green, device frame-pacing comparison pending.
- 🟢 Boardable ship, quarters/refit/customization systems, multiple chassis/modules/liveries/journey decals, PUNCH IT presentation and repair gate.
- 🟢 SpaceLane free flight, reverse/strafe/boost/roll/snap-yaw, ring course, non-lethal ship combat and salvage; first bake/headset tuning pending.
- 🔲 Ship still needs to become the primary world-select/travel hub across shipped worlds.

### Worlds and story

- ✅ Data-driven terrain, vistas, POIs, routes, dressing, building grammar, furnished/portal-culled interiors, biome hazards, story packs, and gates.
- 🟢 W000/W001 foundation plus W002–W012 Chapter 1–2 graybox arc generate and audit; consolidated device walk remains.
- ✅ Story Bible and Transmission canon locked; full 12-chapter/80-world seed catalog and four endings designed.
- 🟢 RILL/Cal line system, named voices, physical collectibles/fragments, choices, de-garble presentation, repair jobs, visible mining, Signal state, and world story hooks; VO and broader device proof remain.
- 🟢 Creature framework, four archetypes, Warden, novel behaviors, ecology scheduling, non-lethal combat, forged bodies; species depth/device readability remain uneven.

### Art and audio

- ✅ SkyVista system and canonical sky progression.
- ✅ Asset Forge pipeline: recipes → meshes/textures/bakes → CI turnarounds → device-ready assignments, with budgets and wiring gates.
- ✅ FORGE II arsenal, avatar pieces, building kit, flora, and six articulated/textured/breathing creature bodies.
- 🟢 FORGE III light script, per-world grade, practical fixtures/halos/pools, and water surface/motion/foam proof.
- 🟡 Picasso’s immediate work: finish runtime/placement water, then the W001 signature-creature passport/review artifacts.
- 🟢 Procedural biome ambience exists; hazard stingers, adaptive music stems, VO, fuller SFX/VFX, and ducking remain.

### Multiplayer and meta-game

- ✅ PvP pure rules, N-combatant/mode cores, smart bot brain, five data-driven arenas, Gun Game/KotH/Fragment Rush/Horde, arsenal/melee/augments, rewards/unlocks/daily seed.
- 🟢 Photon presence path and head/hands avatars exist; combat/score sync, proper room-code UX, body/voice and two-headset proof remain.
- ✅ Tidefront conquest simulation, holo table, AI, builds/attacks, mission modifiers, ground/space defense, saves, fog, hotseat.
- 🟡 Photon live Tidefront sync remains after the online transport/hardware gate.

---

## 🔜 IMMEDIATE — BEFORE / DURING THE NEXT PC AND HEADSET SESSION

### Models can do without Terry

- ✅ Async travel code/CI closed; device comparison queued.
- ✅ Reconcile current checklist, sprint, Excellence Map, and this Master Checklist.
- 🔲 Build the UI readability/reach audit.
- 🔲 Write the haptic coverage checklist, then later add a registry/audit only through an approved task.
- 🔲 Add behavior-count and plant/vehicle catalog-breadth gates.
- 🔲 Recheck and harden `AudioDirector` unload/disposal only if live code still shows the leak risk.

### Picasso when usage returns

- 🔲 Finish FORGE III F3.3 runtime `ZiptideWater` and device normal baker.
- 🔲 Add `WaterAuthor`/`waterRects`, first in W001 canals and Tidefront.
- 🔲 Build `FH-A01-SIGNATURE-CREATURE-PRESENTATION`; this unblocks first-hour creature resolution.
- 🔲 Continue grounding → VFX → reactive props → macro variation/signage → art-conformance ratchet.

### Terry at the PC/headset

- 🔲 Pull `terry-local-wip` and clear the pending block in [`TERRY_RUNBOOK.md`](TERRY_RUNBOOK.md).
- 🔲 Run `Ziptide → First Hour → Author W000 Surfaces` and commit generated W000.
- 🔲 Bake/commit SpaceLane, W002 interior/world-spec changes, and other queued authors.
- 🔲 Install the consolidated APK and run the first-hour, travel, world, flight, vehicle, garden, ecology, PvP and Tidefront device passes.
- 🔲 Treat blocker findings as the new priority zero.
- 🔲 Begin Meta app ID/privacy/Data Use/IARC work and decide English-only versus localization seam.

---

## 🟡 SHORT TERM — NEXT FEW WEEKS

- 🔲 Picasso FH-A01 → Story/Ship FH-S05 → accumulated bake/device evidence → FH-S08 final W001 orchestration.
- 🔲 Make W001 and the Chapter 1 band read as a cohesive shipped game: art, water, lighting, VFX, signage, creature identity, story delivery and audio.
- 🔲 Close UI readability/reach, haptic coverage, weapon feel, space-enemy variety, vehicle garage/catalog, garden breeding/giants, creature behavior breadth.
- 🔲 Make credits visibly useful across ship upgrades, tools, factories, vehicles and gardens.
- 🔲 Complete Photon arena combat/score synchronization and two-headset room-code play.
- 🔲 Run repeated travel/performance/memory soak tests and fix all blocker regressions.

## 🟡 MIDTERM — SYSTEMS AT SCALE

- 🔲 Make the Ship the real hub/world selector and retire placeholder door-first travel on the Chapter 1 worlds.
- 🔲 Finish VO pipeline, RILL/Transmission recordings, adaptive stems, hazard stingers, SFX/VFX and world-presence layers.
- 🔲 Expand interiors, creature passports/behaviors, ecology, vehicles, gardens, factory loops, hazards and missions across world archetypes.
- 🔲 Author W013+ through WorldSpecs only after all required hooks/gates are stable.
- 🔲 Complete multiplayer polish, avatars/voice, additional arena variety and Tidefront live synchronization.

## 🔭 LONG TERM — COMPLETE AND SHIP

- 🔭 Generate, audit, and device-walk W013–W080 against the locked canon.
- 🔭 Place all Transmission fragments, branches and four endings; complete full VO/audio/presentation.
- 🔭 Ship the persistent customizable Ship as home, world selector and progression sink.
- 🔭 Complete Arena, Photon online and Tidefront programs.
- 🔭 Performance/accessibility/localization/release-build hardening across all content.
- 🔭 Meta entitlement/Platform SDK, release keystore, minimal permissions, store captures/trailer/assets, QA matrix and accepted submission.

---

## 🧪 PARKED UNTIL AFTER THE BASE GAME SHIPS

- Community player-facing world builder and world-vs-world tournaments.
- Mature/adult content variant as a swappable content layer, not a core rewrite.
- Optical-illusion/Pattern-world mechanics after comfort and visual systems are proven.
- Ranked seasons/live-ops/cloud economy features that require a backend.

---

*Update contract: meaningful status changes must update `CURRENT_EXECUTION_CHECKLIST.md`; update this broad inventory when a major system changes state. Preserve older plans and handoffs for evidence rather than deleting history.*
