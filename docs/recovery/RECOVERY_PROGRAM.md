# ZIPTIDE RECOVERY PROGRAM

**Program owner:** GPT-5.6 Thinking, Recovery / Integration lane  
**Authorized by:** Terry, 2026-07-14  
**Status:** ACTIVE — R0 repository truth and contract inventory  
**Branch:** `terry-local-wip`

## 1. Why this program exists

ZIPTIDE has substantial pure logic, generators, content, art tooling, and prototype gameplay. The current failure is not a lack of systems. It is a lack of whole-product integration proof.

The repository has repeatedly treated these states as if they were equivalent:

- source exists;
- pure/EditMode tests pass;
- scenes patch and audit;
- an isolated Forge/photo-booth render looks acceptable;
- an APK builds;
- the complete runtime is usable and presentable on Quest.

They are not equivalent. This program replaces that ambiguity with explicit proof levels and a single integration path.

## 2. Recovery authority and freeze

Until this document is explicitly superseded in `docs/HANDOFF.md`:

1. **Normal feature expansion is frozen.** Do not add worlds, modes, weapons, progression layers, visual subsystems, multiplayer features, or catalog breadth.
2. **Multiplayer implementation remains paused.** Existing code is preserved; no A6.2+ work resumes.
3. **Picasso/Forge expansion is paused.** Existing recipes and tooling are preserved; no new broad visual envelope begins.
4. **No world mass-build.** W013+ authoring remains blocked.
5. **No scene or prefab YAML edits.** Existing patcher/author law remains in force.
6. **No destructive rewrite.** The recovery program preserves useful pure cores, data, generators, saves, story, and art recipes unless evidence shows replacement is safer.
7. **One recovery owner.** Cross-cutting integration work is assigned through this program. Other operators may receive bounded packets only after the owning contract is written.

Emergency CI-red repair is allowed, but it must not smuggle in unrelated feature work.

## 3. New proof taxonomy

Every player-facing feature must carry one current proof level:

| Level | Meaning | Allowed claim |
|---|---|---|
| `SOURCE` | Code/data exists. | “Implemented in source.” |
| `CORE` | Pure/EditMode tests prove deterministic logic. | “Core-proven.” |
| `PATCHED` | Authors/patchers generate valid content and structural audit passes. | “Generated and structurally valid.” |
| `PLAYMODE` | Runtime scene lifecycle, `Awake`/`Start`, integration and cleanup pass in automated PlayMode. | “Runtime-integration proven.” |
| `VISUAL` | Canonical screenshots pass layout/facing/contrast/fallback checks. | “Visually integrated on reference renderer.” |
| `APK` | Android APK path builds from the same tested SHA. | “Android build proven.” |
| `QUEST` | Terry accepts the defined device checklist on named hardware/build. | “Quest-proven.” |

`CI GREEN` is not a proof level. It is a summary of whichever lanes actually ran.

A feature may not be described as “done,” “complete,” “shipped,” “excellent,” or “device-ready” unless its required final proof level is recorded.

## 4. Recovery phases

### R0 — Repository truth and system contracts

No runtime behavior changes.

Deliverables:

- system-contract inventory;
- bootstrap/singleton map;
- persistent-object map;
- runtime-created-object map;
- event producer/consumer map;
- save ownership map;
- feature proof-level ledger;
- duplicate-owner and unowned-responsibility report;
- recovery exposure list: golden-path, hidden prototype, approved support system;
- corrected project status generated from evidence rather than optimistic board prose.

Exit gate: every critical player-facing responsibility has exactly one proposed canonical owner or an explicit unresolved decision.

### R1 — Integration harness

Build the missing verification layer before broad fixes:

1. minimal Unity PlayMode CI job;
2. repeat-green proof on an unrelated descendant commit;
3. fake tracked-head/controllers fixture without Quest hardware;
4. runtime hierarchy/object census;
5. scene lifecycle and travel/save smoke tests;
6. deterministic screenshot capture at named viewpoints;
7. text overlap/facing/panel-bound checks;
8. fallback/prototype visibility checks;
9. performance and allocation samples;
10. artifact bundle containing logs, hierarchy, screenshots, metrics, and tested SHA.

Exit gate: obvious runtime/UI/lifecycle failures are reproducible without Terry wearing the headset.

### R2 — Contract consolidation

Fix failure classes, not individual screenshots:

- one boot/travel state machine;
- one developer-menu owner and access contract;
- one diegetic-panel/layout/facing framework;
- one persistent HUD policy;
- one item grip/holster/muzzle/scale/impact-axis contract;
- one shooter/self-ignore contract;
- one creature grounding/contact contract;
- one objective/progression state owner per loop;
- one active sky/grade/light owner per scene;
- one visible-fallback shipping policy.

Displaced owners are removed from runtime or hidden behind an explicit diagnostic feature flag.

### R3 — Golden vertical slice

Only this path is exposed in the recovery build:

`Cold boot → choose/resume → W000 → travel to one world → use one correctly posed tool → resolve one creature non-lethally → complete one job → receive and spend one reward → use/board the ship → return → quit and resume`

Everything not required for this path is preserved but hidden from the candidate build.

Exit gate:

- EditMode/core green;
- patch/audit green;
- PlayMode integration green;
- visual snapshots accepted;
- Android APK from the tested SHA;
- performance budget accepted;
- one bounded Quest checklist accepted.

### R4 — Template-driven expansion

Only after R3 passes:

- expand through accepted templates;
- lesser models modify data/specs and bounded adapters;
- no new cross-cutting owner without a design/contract review;
- every new feature inherits integration, visual, performance, APK, and device gates from its template.

## 5. Golden-path exposure classes

Every runtime feature receives one exposure class:

- `GOLDEN_PATH` — required for R3 and held to all gates;
- `SUPPORT` — required infrastructure, not directly player-facing;
- `PROTOTYPE_HIDDEN` — retained in source but unavailable in recovery candidate builds;
- `DIAGNOSTIC` — development-only, explicit invocation, never automatic player exposure;
- `REPLACE_OR_MERGE` — competing or invalid owner; cannot remain active without consolidation;
- `UNCLASSIFIED` — R0 has not resolved it yet; blocked from recovery candidate exposure.

## 6. Initial golden-path scope

Included candidates:

- `_Boot`, `BootLoader`, `HomeHubRuntime`;
- persistent XR rig and exactly one interaction manager;
- `TravelCoordinator`;
- one W000 flow;
- one destination world selected after inventory review;
- one item/tool and one holster path;
- one grounded creature and one non-lethal resolution;
- one job/reward/economy path;
- one ship boarding/use moment;
- save, quit, continue, and return travel;
- minimal readable diagnostics.

Initially hidden:

- war table/Tidefront presentation;
- Quarters customization room;
- all nonessential worlds and arenas;
- PvP and Photon surfaces;
- experimental vehicles, gardens, factories, broad catalogs;
- unverified sky/grade/practical-light variants;
- visible primitive fallbacks not required to prove mechanics.

This is exposure control, not deletion.

## 7. Operator rules during recovery

1. Read this file, `docs/recovery/SYSTEM_CONTRACT_INVENTORY.md`, and the newest HANDOFF entry first.
2. Work from a named recovery packet.
3. Do not infer ownership from class names or old sprint labels; use the contract inventory.
4. A new runtime bootstrap, singleton, persistent object, input binding, save writer, scene loader, camera/HUD attachment, or global render owner requires recovery-owner review.
5. Tests must reproduce the integration failure class, not merely restate the implementation.
6. Warning-only gates cannot support a completion claim.
7. Device-only uncertainty remains explicit. Do not guess.
8. Terry tests only checkpoint builds that already passed the available automated lanes.

## 8. R0 work order

1. Critical path: boot, rig, input, interaction manager, travel, spawn/fall, save.
2. Runtime presentation: Home Hub, developer menu, travel station, HUD, Quarters, war table.
3. Item presentation: factory, grip, holster, muzzle, projectiles, melee.
4. World/runtime generation: WorldSpec/compiler, patchers, builders, runtime-added content.
5. Visual globals: sky, grade, fog, lighting, practicals, water, fallback art.
6. Creature grounding/behavior/presentation.
7. Ship/flight/repair/objective flow.
8. Multiplayer and Tidefront.
9. Audio, diagnostics, health, performance and cleanup.
10. CI/audit coverage against all categories above.

## 9. Completion rule for R0

R0 is complete only when the inventory can answer, for every critical system:

- What responsibility does it own?
- How does it start?
- Does it persist?
- What runtime objects does it create?
- Which inputs/events does it consume and produce?
- What state does it read/write?
- What other system claims the same responsibility?
- What proof level has it actually reached?
- Is it exposed in the recovery candidate?
- What is the next evidence or consolidation action?

Until then, no broad implementation sprint begins.
