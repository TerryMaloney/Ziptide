> Accepted as the planning baseline by Terry on 2026-09-29. Original audit below is historical; current execution status and commands are in [QUEST_RECOVERY.md](../QUEST_RECOVERY.md).

# Ziptide — whole-project inventory and revised production plan

**September 28, 2026 · Proposed plan for review · No repository changes**

Inspected baseline: TerryMaloney/Ziptide, `claude/unity6-migration` at `8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a`. This extends the separate build/recovery audit. Findings are from repository documents, source, and previously retrieved CI evidence. Unity, PowerShell, and Quest gameplay were not runnable here. “Implemented” below means identifiable code, not a certified player experience.

## 1. Assessment and scope

The project has a substantial collection of reusable mechanics, simulation cores, generators, and story design. It does not yet have a proven production system that turns the whole campaign plan into reliable playable worlds. We should preserve the useful implementation and finish the connections, persistence, content contracts, and validation that let it scale.

The newer base-game plan covers **W000–W068 plus the unnumbered Earth Approach sequence**. The older master plan's 80-world scope includes **W069–W080 as DLC**; it should not be interpreted as 80 base-game worlds. W000 is onboarding. W064–W067 represent ending branches, and W068 is the final coda. These entries are not all equivalent full-size levels.

The intended game is a Quest VR salvage/repair adventure: a personal ship, tactile tools, non-lethal encounters, exploration, strange environments, RILL, persistent progression, and Ziptide travel. Arena, Horde, and Tidefront reuse that universe and its content. Future co-op and DLC are expansion scope unless explicitly promoted into the base release.

**Production objective:** finish a representative first experience, prove reuse on different worlds, then assemble the remaining campaign in small batches using shared systems. Artistic direction belongs to Terry; mechanics, engineering, technical art integration, and production tooling belong to the assistant.

We should play short representative slices during construction. Deferring all playtesting until the full campaign exists would multiply poor VR interaction and spatial assumptions across dozens of worlds. Extensive tuning can come later; basic feel and production assumptions must be tested early.

## 2. Inventory: built versus missing

These are capability assessments, not completion percentages. Generated definitions, code paths, committed assets, and tested runtime behavior are different kinds of evidence.

| Area | Existing foundation | Missing or insufficiently demonstrated |
|---|---|---|
| Build and recovery | Unity 6 migration, patch pipeline, audits, tests, PowerShell build/install tools | Current Windows/Android/Quest proof; trustworthy fresh-evidence handling; consistent build entry point |
| Boot, rig, input, travel | Boot/rig owners, XR input, locomotion, central TravelCoordinator | Full start/continue/travel loop verified after migration; consistent ownership documentation |
| World production | WorldSpec/compiler, WorldPack, layout/building/terrain/scatter authors | Complete production schema, stable generation ownership, campaign-wide records and reproducibility |
| Campaign content | ToxicCity JSON; W000 and W002–W012 code-authored layout/job paths | These 13 numbered entries are not 13 finished levels; W013–W068 largely prose seeds |
| Objectives | Sequential job runtime, objective board, collection/repair/combat/travel-related steps | Durable partial-job checkpoints; broader sequencing/exception support; explicit completion/replay rules |
| Progression | Flags, gating, rewards, resource ledger, profile | Unified campaign graph; stable unlock IDs; producer/consumer validation; optional-job versus world-completion distinction |
| Saving | Profile serializer/store and persistent world overlays for several systems | General mission checkpoint restoration; partial repairs where needed; safe repeat rewards and migrations |
| Tools and weapons | Shared item infrastructure; taser/pistol/gravity and several ranged/melee tool implementations | Many story powers; consistent world-target responses, ownership, feedback, save and mode adapters |
| Augments | Six defined active/passive augments and runtime behavior | Complete acquisition/surfacing; mode coverage; larger planned power progression |
| Creatures and encounters | Multiple creature definitions, behavior families, drones and bots | Authored encounter packages, clear counters, campaign progression and representative device tuning |
| Hazards | Wind, static, flood, spore, radiation runtime | Flood is a slow effect, not a complete underwater system; pressure/oxygen, heat/cold and special physics need deeper support |
| Traversal | Zip/climb/lift/grapple-related systems | Full campaign placement/clearance validation; later traversal tools such as placeable zipline seeds |
| Repair and machines | Repair interactions and machine production systems | Persistent multi-stage outcomes and unified sequencing with jobs, gates, and returns |
| Mining, gardens, automation | Mining, plant authoring, genetics, belts, factory state and blueprints | Consistent acquisition/tutorial/UX; surfaced campaign use and balanced progression |
| Crafting and economy | Resource ledger, affordability/spend helper, production graph | Coherent recipes/tiers/unlocks; complete player-facing crafting loop; transaction/replay invariants |
| Ship and space | Flight, course/rings, space encounters/salvage, loadout/customization foundations | MK2 acquisition/transfer rules; campaign integration; varied authored space routes |
| Vehicles | Skiff, hoverbike, crawler definitions and specialized runtime paths | Remaining intended archetypes, garage/acquisition flow and campaign-wide validation |
| Derelicts | Space/salvage mechanics that can support them; planned families | General derelict record/factory linking interior, entry, hazard, log, loot, and persistent state |
| Narrative and RILL | Large story bible; companion cues, subtitles, line/VO hooks, transmission state | Most campaign story delivery, authored timing, complete audio content and reliable progression links |
| Choices and endings | Flag-backed two-option choice station | Full campaign branching, Earth Approach, ending orchestration, credits/replay/postgame rules |
| Audio and presentation | Audio profiles, ambience, mix/director services and playback hooks | Complete event catalog, VO/music pipeline, coverage and device mix validation |
| Arena and Horde | Match rules, modes, bot logic, weapons, some progression/reward systems | Full content inheritance, remaining surfacing and bot symmetry, integrated acceptance passes |
| Tidefront | Conquest simulation, vessels/defenses/nodes, mission/table/hotseat foundations | Full campaign breadth, polished mission integration and release-level UX |
| Online play | Photon transport, presence/pose and message interfaces | Gameplay combat synchronization, scalable peer handling, room UX and online acceptance testing |
| Art integration and performance | Procedural art/Forge, imports and auditing infrastructure | Reliable interchangeable art packages; enforced budgets; verified Quest frame time, memory, comfort and readability |

Specific counts should not become misleading progress claims. Six augments are not the complete planned power tree. Multiple enemy assets are not that many polished encounters. Twenty-four authored plant specifications are not proof of twenty-four surfaced garden experiences.

## 3. Where the current implementation diverges from the plan

### Content factory has an incomplete input contract

Only `docs/worldspecs/ToxicCity.spec.json` is committed as a WorldSpec JSON. W000 and W002–W012 use separate code-authored paths. The production matrix contains useful world-by-world design but is not yet an executable manifest.

WorldPack contains jobs, choices, belt floors and audio fields that WorldSpec does not fully express. Rings and other layout-specific content have separate paths. Therefore a world cannot yet be described consistently through one complete production record.

**Change to the plan:** extend existing definitions and adapters; do not replace working authors with a new engine. Introduce a versioned world record that references the existing canonical definitions. Migrate W000/W001 and W002 first. Make regenerating those records repeatable before migrating the remaining worlds.

### Saving is broad, but mission continuity is a gap

The profile already saves flags, resources and several persistent world systems. However, the inspected JobRuntime starts its sequential job state fresh, and JobDirector does not restore a general serialized objective checkpoint. RepairableMachine uses local stage state without a generalized profile-backed restoration path in the inspected implementation.

JobRewards explicitly allows resources to be granted twice if called twice, even where flag writes are idempotent. Whether replay should pay again must be a declared rule rather than an accidental consequence. World flags are granted on job completion, which is too broad once worlds have optional or multiple jobs.

**Change to the plan:** use the existing save/profile system with stable world/object/job IDs, checkpoint records, and reward receipts. Distinguish first completion, repeatable activity, optional objective, and world completion. Verify early collection, out-of-order actions, leaving mid-objective, resume, duplicate events, and old-save migration.

### The campaign needs an executable progression graph

Flags and gates exist, but the campaign's producer/consumer relationships, gear tiers, special routes, MK2 acquisition and ending rules are not fully reconciled. Examples include `toxiccity_complete` versus `W001_COMPLETE`, W002 scanner/headlamp versus later Prism progression, and an unassigned MK2 acquisition point.

**Change to the plan:** one campaign manifest references world IDs, routes, prerequisites, grants and ending branches. Validate missing producers, contradictory prerequisites, broken references, and unintended unreachable routes. Branch validation must understand mutually exclusive choices; all endings need not be reachable in one save.

Support nonstandard entries explicitly: W028 intentionally has no job; W057 is transit-only; Earth Approach is unnumbered. Do not force every experience into “arrive, accept job, kill quota, exit.”

### Late-game powers contain real engineering work

The arsenal plan includes systems beyond reskinned weapons: pressure/oxygen, phase behavior, object portals, placeable traversal tools and specialized world responses. Existing Prism combat does not prove a Prism puzzle receiver; existing ziplines do not prove a zipline placement tool.

**Change to the plan:** build a capability dependency map from each planned world to its required mechanics. Reuse shared effect/target interfaces and data-driven tuning, while budgeting genuinely new runtimes. Build each family before its first dependent production batch. Prototype high-risk exceptions early without building their complete worlds.

### Shared content inheritance is a contract, not yet an enforced guarantee

The July inheritance document correctly says campaign content owns identity while side modes own rules and balance. Enforcement remains incomplete. Arena/shared item relationships and future effect/skin registration need consolidation around existing owners.

Online has message interfaces, but searches found SendFire/SendHit/SendScore/SendWall declarations and transport implementations without gameplay call sites outside tests/vendor code. This is transport groundwork, not finished network combat.

**Change to the plan:** share item/creature/ship identities, presentation, feedback, ownership and effects. Modes reference those definitions through rule adapters. Preserve multiplayer seams now, but treat full online play as its own delivery milestone; do not count presence as completion.

## 4. The minimum strong skeleton

The skeleton is complete when a designer can assemble an ordinary new world without inventing its own save logic, travel flow, rewards or tool behavior.

1. **Reliable runtime spine:** new game, load, rig/input, scene travel, spawn, return, pause/recovery and clean ownership.
2. **Durable gameplay state:** objectives, encounters, repairs, inventory/unlocks and one-time outcomes survive the transitions where persistence is intended.
3. **Complete authoring contract:** world record, objective/encounter references, route/gates, hazards, economy, narrative/audio hooks, art sockets and budgets.
4. **Validated campaign graph:** stable IDs, prerequisites, branches, no-job/transit entries, endings and consistent save interpretation.
5. **Reusable mechanics vocabulary:** existing tools plus shared target responses; additional specialized mechanics introduced against real campaign requirements.
6. **Safe regeneration:** version/hash provenance, explicit authored/generated boundaries, upgradeable generated assets, preservation of approved artistic overrides.
7. **Interchangeable presentation:** stable gameplay roots with replaceable visual children, defined scale/grips/sockets/colliders, animation events, LODs and material budgets.
8. **Useful evidence:** compilation, targeted integration checks, reproducible bakes, and Quest play evidence for representative experiences.

Do not create a second wallet, save system, scene loader or parallel item catalog. The work is to strengthen the owners already present and remove ambiguity between their callers.

## 5. Revised delivery sequence

| Stage | Deliverable | Exit evidence |
|---|---|---|
| 0 — Recover | Canonical branch/toolchain; safe, truthful build/install/smoke path; reconciled runbook | Fresh build identified by commit; install/launch evidence; basic headset start/continue |
| 1 — Stabilize shared state | Mission checkpoints, explicit completion/replay rules, reward receipts, travel/save integration | Resume at meaningful interruption points; duplicate events cannot accidentally double-pay; existing saves migrate |
| 2 — Complete production contracts | Versioned world records, campaign manifest, content ownership and regeneration rules | W000/W001/W002 express required content without hidden manual setup; repeated bake preserves authored overrides |
| 3 — Finish the first experience | Approved W000→space→ToxicCity→keyed departure flow with complete feedback, persistence and spatial purpose | Terry can complete it on Quest without developer intervention; comfort and measured performance acceptable |
| 4 — Prove reuse | W002 followed by W003–W005 as a small production batch; isolated high-risk mechanic/branch prototypes | Worlds reuse the spine; differences mainly come from records/layout/art; exceptions and costs are documented |
| 5 — Produce the campaign | Chapter batches, each with its dependencies, graybox route, save/play pass and artistic review | Every batch traversable and recoverable; campaign graph remains valid; known limitations visible |
| 6 — Finish release scope | Endings/postgame, full tuning/audio/art passes, side-mode acceptance and agreed online scope | Complete target-device campaign and mode acceptance, save compatibility, performance and release checks |

Stages may overlap once their prerequisites are stable. For example, Terry can direct the next environment while shared persistence work proceeds. Do not interpret Stage 6 as delaying all sound, art, accessibility, comfort or performance work until the end; minimum quality belongs in every earlier gate.

Suggested production batches follow the existing story matrix: W006–W012 reconciliation; W013–W019; W020–W028; W029–W038; W039–W051; W052–W061; W062/Earth Approach/W063–W068. Split these into smaller working groups of roughly three to five entries. Endings require branch-aware testing rather than a single linear walkthrough.

Before broad production, exercise inexpensive test scenes for underwater/pressure behavior, object-only portals, an optional/quiet world, transit-only routing, and ending selection. These prototypes expose architecture risks; they do not substitute for finished content.

## 6. How later worlds become faster without becoming repetitive

A new world should assemble reusable components: architecture kit, terrain/vista, arrival and return, traversal pattern, hazard, encounter, machine/economy loop, story beat, rewards and audio. Its identity should come from their arrangement, scale, pacing and combinations.

Every world brief should specify:

- Its experiential purpose and contrast with adjacent worlds.
- A recognizable arrival, main landmark and readable destination.
- Playable distances and travel times, not just scene dimensions.
- Main route, worthwhile optional route, and return/exit behavior.
- Verticality, sightlines, occlusion and navigable clearances at headset scale.
- Why each area exists: discovery, challenge, interaction, reward, rest or story.
- Foreground interaction versus inaccessible skyline/vista.
- Encounter and performance budgets; persistent changes on revisit.

This directly addresses small, uninteresting procedural worlds. Adding more objects or expanding empty terrain is not sufficient. Terry approves composition and atmosphere; I implement routes, affordances, interaction density and technical constraints, then we judge them in headset.

Measure the W002 and W003–W005 batch: time to first playable route, custom runtime files required, repeated defects, manual bake steps, and rework after play. A useful proposed target is that most ordinary later worlds require no new runtime code. Do not promise a campaign schedule until that replication rate is observed. Special worlds remain exceptions with explicit engineering cost.

## 7. Ownership and decisions

| Terry — artistic direction | Assistant — mechanics and engineering | Joint acceptance |
|---|---|---|
| Mood, style, silhouettes, visual references, composition and story taste | Runtime architecture, tools, effects, objectives, progression, saves, build recovery, generation and integration | VR feel, spatial interest, challenge, narrative pacing and release quality |
| Approve world/ship/creature identity and hero moments | Implement/import presentation within stable sockets and measured budgets | Decide when a representative world is good enough to replicate |
| Review artistic options and reject weak results | Turn feedback into specific changes and maintain an evidence-backed backlog | Resolve scope and canon choices that change the intended game |

This does not require Terry to personally model, rig or produce every asset. Asset work can be implemented or sourced under Terry's direction.

Decisions to queue, without blocking this audit: reconcile the first-hour return-to-ship payoff versus newer departure route; confirm MK2 acquisition and transfer behavior; approve unlock order; decide whether online is a base-release requirement; confirm ending/replay/postgame expectations. Engineering should prepare concrete options and consequences before asking.

## 8. Organization and immediate backlog

Keep five authoritative entry points: product/canon brief, executable campaign manifest, system ownership map, delivery backlog, and evidence index. Mark older documents as historical with links to their replacements. Do not delete history or reorganize the repository before mapping dependencies.

Each backlog item needs: observed problem, owning system, affected worlds/modes, dependency, acceptance evidence and current state. Replace ambiguous “done” or diamond labels with **planned / implemented / integrated / editor-verified / device-verified**, tied to a revision. Artistic approval is a separate field, not implied by a passing test.

First implementation tickets, in dependency order:

1. Recover the exact Unity/Android toolchain and make build outcomes truthful and safe.
2. Reconcile canonical branch, first-hour route, current instructions and bake entry points.
3. Add durable job checkpoints and explicit world-completion/reward semantics through existing owners.
4. Complete the production record for W000/W001/W002 and establish regeneration ownership.
5. Encode campaign routes/unlocks and validate missing or contradictory dependencies.
6. Establish shared item/effect/encounter contracts and presentation sockets; retain working mechanics.
7. Finish and play the first experience, then prove different-world reuse.
8. Measure the first batch and revise campaign estimates before scaling.

No repository changes, branches, commits, issue creation, workflow dispatches or installations were performed for this report.

## Evidence map

All links below pin the inspected revision, so later edits do not silently change the basis of this assessment.

- [Base-game factory readiness](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/production/BASE_GAME_CONTENT_FACTORY_READINESS.md)
- [World content matrix](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/production/BASE_GAME_WORLD_CONTENT_MATRIX.md)
- [Shared inheritance contract](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/production/SHARED_CONTENT_INHERITANCE_CONTRACT.md)
- [Arsenal and power progression](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/production/ARSENAL_AND_POWER_PROGRESSION.md)
- [Older master plan and DLC scope](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/ZIPTIDE_MASTER_BUILD_PLAN.md)
- [Excellence map](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/EXCELLENCE_MAP.md) and [multiplayer sprint](https://github.com/TerryMaloney/Ziptide/blob/8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a/docs/SPRINT_MULTIPLAYER.md): useful inventories whose completion claims require source/device corroboration.

Source paths under `Ziptide/Assets/Ziptide/`:

| Finding | Primary inspected source |
|---|---|
| Schema coverage | `Content/Runtime/Spec/WorldSpec.cs`; `Content/Runtime/WorldPacks/WorldPackDefinition.cs` |
| Authored job coverage | `Editor/Patching/WorldJobLibrary.cs`; WorldLayoutLibrary |
| Job persistence and completion | `Gameplay/Runtime/Jobs/JobRuntime.cs`; `Gameplay/Runtime/Jobs/JobDirector.cs`; `Content/Runtime/WorldPacks/WorldGating.cs` |
| Reward repeat behavior | `Content/Runtime/Jobs/JobRewards.cs` |
| Existing persistence | `Core/Runtime/Persistence/PlayerProfile.cs`; serializer/store and world overlays |
| Hazard limits | `Gameplay/Runtime/World/HazardZoneRuntime.cs` |
| Choice structure | `Content/Runtime/WorldPacks/ChoiceSpawnDefinition.cs`; ChoiceStation |
| Defined augments/vehicles | `Editor/Patching/AugmentAuthor.cs`; `Editor/Patching/VehicleAuthor.cs`; corresponding runtimes |
| Online boundary | `Multiplayer/Runtime/Net/PvpNet.cs`; transport implementations and gameplay call-site search |

Uncertainty remains around actual generated scene state, current Windows setup, headset comfort/performance, and end-to-end behavior. The plan makes those explicit verification tasks rather than treating documentation or code presence as proof.
