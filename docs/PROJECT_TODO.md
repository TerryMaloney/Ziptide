> **2026-09-29 save-result follow-up:** SaveSystem now offers TrySave/TryAutosaveNow; failed writes return false, retain the prior timestamp and cannot emit SAVE_AUTOSAVE. Existing void callers remain compatible. Three disk-store regression tests added. Reward patch 0c36bfc4 passed Unity EditMode in run 36620111632; this follow-up needs its own CI. See [handoff](handoff_queue/20260929_save_results.md).

> **2026-09-29 engineering start:** [State ownership audit](architecture/STATE_OWNERSHIP_AUDIT_20260929.md) and [mission persistence design](architecture/MISSION_PERSISTENCE_CONTRACT.md) recorded. ORG-02/SAVE-01 are partially audited, not closed. Numeric reward guards and 11 regression cases added; current candidate requires Unity CI. Next: finish identity/replay inventory and save-result failure contract.

# Ziptide — comprehensive delivery checklist

Updated September 29, 2026. Planning baseline: `production/WHOLE_PROJECT_PLAN_20260928.md`.
Source baseline: recovery branch `claude/quest-recovery-20260929`, published head `0f295f582a4e4da8956abd21326e427ecb2b19e4`.

## Objective

Build a dependable general game and content-production system so ordinary later worlds are
primarily records, layout, presentation and tuning. Prove the first full experience, then W002
reuse, then W003–W005 throughput. Preserve distinctive worlds and Terry's artistic control.
Do not promise that unusual mechanics or the remaining campaign will be instant after Level 1.

Base scope: **W000–W068 plus Earth Approach**. W069–W080 are DLC. Online/co-op release scope
is a decision, not an assumed prerequisite for finishing the campaign. Existing side-mode work
is retained and assessed. These 69 numbered entries are not 69 equivalent full-size levels.

**Terry cannot test today.** No PC/headset task is due today. Source investigation, records,
contracts and test design can proceed. Runtime integration needs healthy current CI; VR feel,
spatial acceptance and target-device performance cannot be closed without device evidence.

## How to use this list

- **N — Now:** assistant can investigate/prepare from source without Terry or a headset.
- **C — Code/editor:** assistant-owned implementation or automated verification; Unity/Android
  execution must run in a capable environment. Not every C item is immediately unblocked.
- **T — Terry decision/art:** assistant prepares concrete alternatives/examples; Terry approves direction.
- **H — Hardware/play:** PC/headset/session access required; no test requested today.
- **138 tracked tasks: 137 open, one automated-verification task closed.** Some strengthen or verify implemented systems; they do not
  imply that those systems are absent. The baseline above each group explains what exists.
- Group dependencies are prerequisites, not a ban on preparatory analysis. A decision-dependent
  implementation waits for the relevant decision. Small fixes may proceed independently when safe.
- Complete a row only with its acceptance evidence. Track intermediate state as specified,
  implemented, generated, automated-verified, device-verified, and artist-approved as applicable.
  A passing test is not artistic acceptance, and code presence is not integration.
- When starting a row, add owner, exact affected paths, evidence links, remaining risk and a
  bounded next edit. Split oversized implementation rows into child tickets at that point.

## Established evidence, not remaining work

- [x] Whole-project source audit and planning baseline accepted by Terry.
- [x] Recovery branch published through the GitHub connection; Git tree hashes match the approved local changes.
- [x] Build scripts no longer force-close Unity/delete project locks; shared preflight and per-session artifacts implemented.
- [x] Contract generation added to the shared build path; install-only and fresh boot/capture checks implemented.
- [x] Local 244 Python tests and 25 PowerShell behavior checks passed; PowerShell runtime was 7.4.6 on Linux.
- [x] GitHub Fast Preflight passed for `0f295f58`, run [36576938114](https://github.com/TerryMaloney/Ziptide/actions/runs/36576938114).
- [x] Unity compile/EditMode and generated-scene audit passed for `0f295f58`: [run 36576938375](https://github.com/TerryMaloney/Ziptide/actions/runs/36576938375). The Android APK job was skipped. This does not establish current PlayMode or device proof.
- [ ] Exact-candidate Android APK, Windows execution and Quest acceptance: no current proof.

## Work to start without a headset

| Order | Ticket | Concrete output | Why it comes first |
|---|---|---|---|
| 1 | BLD-02 | Check current-engine PlayMode coverage and prepare the exact route verification | Compile and scene audit are green; current PlayMode proof remains distinct |
| 2 | ORG-02 + SAVE-01 | System-owner/state-lifetime map with source paths | Prevents duplicate save/reward/travel implementations |
| 3 | GRAPH-01/02 + MECH-01 | Campaign producer/consumer and capability dependency inventory | Finds missing prerequisites before content multiplication |
| 4 | SAVE-02/05/07 | Bounded checkpoint/replay/transaction design and interruption cases | Defines what must survive interruption before adding more missions |
| 5 | WORLD-01/02 | Schema coverage table and minimal production-record extension | Makes W000/W001/W002 reproducible through the existing factory |
| 6 | GRAPH-03/04/05 | Small decision packet with conflicts, proposed choices and consequences | Lets Terry decide canon/art direction asynchronously |
| 7 | SAVE-03/05/07 | First tested persistence implementation, once dependencies/CI are ready | High-value reusable engineering before later worlds |
| 8 | WORLD-06 | Complete pilot records, then regenerate and validate in Unity | Proves that the skeleton can actually produce the game |

These are the active priorities. Remaining work below is queued rather than all being claimed
simultaneously. Do not expand campaign production to compensate for an unresolved shared-system defect.

## Delivery gates

| Gate | Exit condition |
|---|---|
| Recovery | Current-engine compile/tests/audit + exact APK + reliable start/travel/resume on Quest |
| Shared systems | Durable objectives/repairs/rewards, validated progression, defined ownership and tested exceptions |
| Production records | W000/W001/W002 rebuild without hidden manual steps or lost approved overrides |
| First experience | Complete approved route with usable controls, spatial purpose, feedback and measured performance |
| Replication | W002 then W003–W005 demonstrate reuse, contrast and measurable reduction in custom work |
| Campaign | Chapter batches pass route/state/art/performance review; all intended endings remain reachable |
| Release | Shipping scope passes device/upgrade/platform checks; licensing/admin holds closed; Terry authorizes release |

## ORG — Project truth, scope and organization

**Existing baseline:** Hundreds of useful documents exist, but branch, engine, route and completion claims conflict.

**Dependencies:** None; this is source reconciliation, not a rewrite.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| ORG-01 | N | Make this the active delivery backlog and route older checklists here. | Current entry points link here; older design detail remains intact and explicitly historical where superseded. |
| ORG-02 | N | Build a system ownership map for boot, input, travel, save, inventory, jobs, economy, generation and presentation. | Every state-changing operation has an identified existing owner and callers; competing writers are recorded. |
| ORG-03 | N | Reconcile branches and the old PR queue by patch equivalence. | Each old PR is classified integrated, still useful, conflicting or obsolete; none merged/deleted wholesale. |
| ORG-04 | N | Index current evidence by source commit, build profile, APK hash and date. | Code, generated content, automated results, headset results and artistic approval cannot be confused. |
| ORG-05 | T | Prepare a bounded release-scope decision for online play and side modes. | Terry can choose from concrete scope/cost consequences; co-op and DLC are not silently added to base acceptance. |
| ORG-06 | N | Replace stale timeline promises with measured batch throughput. | W002 and W003-W005 measure effort, custom code and rework before campaign dates are estimated. |

## BLD — Build, installation and diagnostic recovery

**Existing baseline:** Recovery tooling is published at 0f295f58. Local 244 Python and 25 PowerShell checks pass; GitHub Fast Preflight passed. Unity EditMode and generated-scene audit passed in run 36576938375; APK was skipped.

**Dependencies:** Source baseline ORG; live Unity CI must be healthy before further runtime changes.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| BLD-01 | C — DONE | Complete Unity compile/EditMode and generated-scene audit for the recovery candidate. | Passed at `0f295f58`, run [36576938375](https://github.com/TerryMaloney/Ziptide/actions/runs/36576938375); APK was skipped and device acceptance remains open. |
| BLD-02 | C | Obtain current-engine PlayMode route proof and inspect test coverage. | Boot, travel, input and persistence routes run on Unity 6000.2.9f1; historical Unity 2022 results are not reused as proof. |
| BLD-03 | H | Inventory and preserve Terry's existing Windows checkout and local-only assets. | Branch, dirty files, editor/modules and last error are known; original work is retained before any generated build. |
| BLD-04 | H | Verify exact editor, Android SDK/NDK/OpenJDK, license and Windows PowerShell behavior. | Preflight and a real Android build succeed on the development PC; missing prerequisites fail clearly. |
| BLD-05 | C | Test the build/install failure paths beyond helper-unit coverage. | Process failure, stale APK, moved manifest, hash mismatch, install failure and missing logs produce correct exit codes; Windows 5.1 behavior is exercised. |
| BLD-06 | C | Verify post-bake job/pack/route invariants on the same generated output used by the APK. | Six-step ToxicCity job, legal travel destinations and intended scene/profile contents survive the full generation pass. |
| BLD-07 | C | Prepare an exact-source FullDevelopment APK and install package. | APK, SHA-256, build log, profile, source state and manifest are retained; required generation ran successfully. |
| BLD-08 | H | Install the exact candidate, cold-launch twice and record boot evidence. | Device is explicit; install/launch/profile proof succeeds; no save-clearing workaround or stale log is counted as success. |
| BLD-09 | N | Unify local and CI preflight commands and keep blocking/advisory checks explicit. | The same repository gate set runs locally and in CI; unit tests alone cannot masquerade as content validation. |
| BLD-10 | N | Make capture and diagnostics a reliable defect-report path. | Screenshots, video, marks, package identity, persistent logs and performance traces correlate to one session; failed transfers preserve originals. |

## RUN — Runtime spine, controls and recovery

**Existing baseline:** Boot ownership and TravelCoordinator exist; Unity 6 end-to-end behavior and several lifecycle interactions still need proof.

**Dependencies:** ORG-02 and BLD-01 before implementation; headset acceptance remains open.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| RUN-01 | N | Trace all bootstrap, scene-load, restore and late-update writers. | Input bindings, rig height, item poses and presentation have lifecycle maps; no extra repair loop is added by default. |
| RUN-02 | C | Repair verified New Game / Continue / profile-switch state leaks. | Existing profile, fresh profile, second New Game in one session and reload reset tutorial/job latches correctly. |
| RUN-03 | C | Verify travel failure, cancellation, return and repeated arrival recovery. | TravelCoordinator remains sole travel owner; failed transitions restore usable controls and a valid destination/state. |
| RUN-04 | C | Verify item ownership across grab, holster, scene travel and restore. | Only the intended holstered items travel; there is no duplicate inventory or silently lost owned item. |
| RUN-05 | C | Close pause, resume, quit, return-to-ship and settings integration gaps. | Each path is reachable and repeatable from walking, combat, repair, flight and vehicles without corrupting state. |
| RUN-06 | H | Calibrate physical controls and comfort on the chosen Quest hardware. | Hands/rays, grips, reach, standing/seated height, left/right options, turns, crouch and recenter remain comfortable after repeated travel. |

## SAVE — Durable mission state and reward correctness

**Existing baseline:** Profile serialization, world overlays and economy owners exist. General job checkpoints and deliberate reward-replay behavior are not established.

**Dependencies:** ORG-02; design can proceed now. C# integration waits for BLD-01.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| SAVE-01 | N | Map every mutable game state to its save owner and intended lifetime. | Jobs, repairs, encounters, pickups, unlocks, choices, machines, gardens, factories, ships and vehicles each have a restore rule. |
| SAVE-02 | N | Specify stable save IDs and the mission-checkpoint schema within PlayerProfile. | World/object/job/step IDs are stable across bakes; schema changes are additive/migratable; no parallel save system is created. |
| SAVE-03 | C | Implement and restore partial objective progress. | Collect, defeat, repair and ordered/multi-stage objectives resume at declared checkpoints after leave, quit and reload. |
| SAVE-04 | C | Persist meaningful repair stages and mission-owned world changes. | Previously fitted parts, power states and required world consequences agree with the restored objective. |
| SAVE-05 | C | Define and implement completion receipts and explicit replay policy. | Duplicate callbacks cannot accidentally pay twice; repeatable jobs can pay according to a declared rule. |
| SAVE-06 | C | Separate optional-job completion from world and campaign completion. | Finishing a side activity cannot unlock a world exit or story flag that it does not own. |
| SAVE-07 | C | Test interruption between completion, payout, save and travel. | Restart yields a consistent mission/reward state without value loss or duplication through the existing economy owner. |
| SAVE-08 | C | Add migration, missing-content and corrupted-save recovery cases. | Old fixture saves load safely; renamed/removed IDs and failed writes have deliberate fallback and useful diagnostics. |
| SAVE-09 | C | Verify restored presentation follows restored state. | Boards, RILL, doors, machines, inventory and reward displays agree; a saved flag cannot leave an unusable scene. |
| SAVE-10 | H | Run repeated interruption/resume tests in headset. | Pause, suspend, app close and re-entry at representative objective points preserve progress and usability. |

## GRAPH — Campaign graph, progression and canon conflicts

**Existing baseline:** Flags and canonical world matrix exist. Some unlock/flag names and acquisition placements conflict.

**Dependencies:** ORG-02 and SAVE-02; Terry approves canon changes, not routine data transcription.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| GRAPH-01 | N | Extract a campaign manifest from the approved world matrix and story records. | W000-W068 plus Earth Approach have stable IDs, route types, prerequisites, grants and source references. |
| GRAPH-02 | N | Reconcile aliases and producers/consumers for story and completion flags. | toxiccity_complete/W001_COMPLETE and other mismatches are explicit mappings or decisions, never silent duplicates. |
| GRAPH-03 | T | Resolve first-hour return-home payoff versus keyed departure to W002. | One approved ordered route preserves the intended payoff; obsolete bindings and cards are identified before rewiring. |
| GRAPH-04 | T | Resolve W002 scanner/headlamp versus later Prism acquisition. | Tool introduction and every dependent puzzle remain coherent with the approved unlock order. |
| GRAPH-05 | T | Resolve MK2 acquisition, old-ship disposition and inventory/storage transfer. | One story point and complete transfer/revisit behavior are specified before building dependent chapters. |
| GRAPH-06 | N | Validate reachability with mutually exclusive choices and required tools. | Each intended ending has a valid path; softlocks, missing grants, impossible prerequisites and premature unlocks are reported. |
| GRAPH-07 | N | Model quiet, no-job, transit-only and ending entries explicitly. | W028, W057, Earth Approach and W064-W068 do not require fake combat/jobs to advance. |
| GRAPH-08 | T | Prepare ending, replay and postgame behavior for approval. | Branch completion, return to earlier content, save retention and replay rewards have explicit policies. |

## WORLD — Complete world records and safe generation

**Existing baseline:** One committed WorldSpec JSON exists. W000 and W002-W012 use code-authored paths; much later content is story data.

**Dependencies:** GRAPH identity contract; SAVE stable IDs; retain existing compilers and builders.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| WORLD-01 | N | Compare WorldSpec, WorldPack, layout assets and generators field by field. | Every required route, ring-city, mission, encounter, hazard, reward, story and audio field has an authority and known coverage gap. |
| WORLD-02 | N | Define a versioned world-production record that references existing definitions. | Ordinary worlds can declare their full playable experience without hidden menu steps or new subsystem owners. |
| WORLD-03 | C | Implement missing schema/compiler mappings, including ring-city settings. | Export/compile round trips preserve the declared world and fail on unsupported/missing critical data. |
| WORLD-04 | C | Separate authored overrides, generated output and create-only defaults. | Rebuilding updates stale recipe output while preserving Terry-approved art overrides; ownership is explicit. |
| WORLD-05 | C | Record recipe/compiler versions and test repeatable generation. | Same inputs reproduce expected output; second bake has no unexplained changes; outdated generated assets are detected. |
| WORLD-06 | C | Express W000/W001/W002 through complete production records. | Every required interaction and transition can be rebuilt in a clean checkout without manual repair. |
| WORLD-07 | C | Add world validation for references, spawns, routes, clearances and mission placement. | Broken references, inaccessible required objectives and out-of-profile destinations fail with actionable locations. |
| WORLD-08 | N | Create the authoring template and a short one-world runbook. | A new world brief becomes data, generated content, checks and a review packet through one documented process. |

## MECH — Shared tools, effects, hazards and late-game capabilities

**Existing baseline:** Combat, tools, six augments and some hazard/traversal behavior exist; several campaign powers need real engineering.

**Dependencies:** GRAPH unlock map and WORLD record contract; retain working item/effect owners.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| MECH-01 | N | Map each campaign entry to required mechanics and first-use dependencies. | Every missing capability has its earliest dependent world, consumers, save implications and a bounded prototype. |
| MECH-02 | N | Define tool-target response and mode-adapter contracts. | Combat hits, puzzle receivers, repairs, harvesting and world effects share identity without forcing identical mode balance. |
| MECH-03 | C | Complete baseline weapon/tool handling and feedback integration. | Grip, aim, fire, reload/resource use, range, hit response and haptics are testable and stable across travel. |
| MECH-04 | C | Complete actual acquisition/equipment paths for augments and upgrades. | Owned/unlocked/equipped state survives reload; all shipped entries are obtainable and visibly useful. |
| MECH-05 | C | Prototype underwater pressure, oxygen and buoyant traversal. | W013/W032 requirements have explicit failure/recovery/comfort behavior; the existing flood slow effect is not misclassified as underwater support. |
| MECH-06 | C | Prototype object portals, phase effects and changing geometry. | Object ownership, collision, restore, traversal boundaries and comfort limits are proven before dependent worlds are produced. |
| MECH-07 | C | Prototype placeable ziplines/platforms and their validation. | Placement, clearance, reachability, costs, cleanup and persistence work without trapping players or bypassing locked progression. |
| MECH-08 | C | Complete campaign-required heat/cold, static, flood, radiation and special-physics responses. | Each required hazard has readable cues, counters, bounded effects and save/travel cleanup; only needed variants are built. |
| MECH-09 | N | Turn approved prototypes into reusable authoring examples. | A world consumes the mechanic through data; exceptions are documented instead of copied into one-off runtimes. |

## NPC — Creatures, enemies, encounters and fairness

**Existing baseline:** Creature families, drones and bot logic exist; roster presence is not proof of encounter quality.

**Dependencies:** MECH response contract, SAVE encounter policy, WORLD placement.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| NPC-01 | T | Approve species identity and the W001 signature encounter brief. | Silhouette, role, behavior, habitat, interaction tells and non-lethal resolution match Terry's direction. |
| NPC-02 | C | Complete per-species behavior and readable state transitions. | Idle, patrol, notice, challenge, attack/defense, interruption and resolution are appropriate to the species and observable. |
| NPC-03 | C | Build reusable encounter records and spawn/respawn rules. | Density, quotas, activation, escape, persistence and rewards are configurable without bespoke mission code. |
| NPC-04 | C | Verify navigation, grounding, sightlines and bounded pursuit. | Enemies do not attack through walls, spawn inside players or strand mandatory objectives outside reachable space. |
| NPC-05 | C | Integrate enemy/creature rewards and difficulty progression. | Loot and completion follow shared economy/state rules; chapters change challenge deliberately rather than only inflating health. |
| NPC-06 | H | Test counters, readability, pressure and fun in representative encounters. | Players can understand and respond to threats; chosen density remains playable and performant. |

## SHIP — Ship, spaceflight, vehicles and derelicts

**Existing baseline:** Flight, salvage, ship customization, skiff/hoverbike/crawler and route builders exist. Product integration remains incomplete.

**Dependencies:** RUN ownership, SAVE state and GRAPH acquisition rules.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| SHIP-01 | C | Make ship home, world selection, storage and upgrades a coherent loop. | Owned gear, unlocked routes and meaningful purchases are legible and persist through travel/reload. |
| SHIP-02 | T | Prepare functional ship-interior/exterior briefs and module sockets. | Terry approves identity, room purpose and sightlines; engineering defines usable clearances, mounts and interaction surfaces. |
| SHIP-03 | C | Complete launch/course/encounter/salvage/reentry sequencing. | Required space actions cannot be bypassed accidentally and recover correctly after interruption or failure. |
| SHIP-04 | C | Build varied data-authored space routes and enemy roles. | Route differences affect navigation and decisions; bounds and encounter resets remain predictable. |
| SHIP-05 | C | Complete vehicle acquisition, mount, drive, dismount and restore behavior. | Existing vehicles have clear roles and reliable camera/input ownership; other planned archetypes are scoped explicitly. |
| SHIP-06 | C | Create a reusable derelict record and representative playable wreck. | Entry, interior route, hazard, log, salvage, exit and revisits share established systems and stable IDs. |
| SHIP-07 | H | Test cockpit, flight bounds, vehicle eye clearance and motion comfort. | View is unobstructed; head freedom, steering, dismount, return controls and comfort hold across the loop. |

## ECON — Economy, crafting, gardens and automation

**Existing baseline:** Mining, plant/genetics catalogs, crafting/economy helpers and factory systems exist; player-facing acquisition and purpose need integration.

**Dependencies:** SAVE transactions/checkpoints; GRAPH unlocks; shared content identities.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| ECON-01 | N | Map sources, sinks, recipes, tiers and first useful purchases. | Every early activity has a legible purpose; campaign progress does not accidentally require optional multiplayer rewards. |
| ECON-02 | C | Route all grants/spends through the existing economy transaction owner. | Campaign, crafting, garden, factory, arena and Tidefront cannot bypass replay or affordability policy. |
| ECON-03 | C | Complete crafting/build placement, rotation, removal and refund rules. | Invalid placement fails clearly; relocation/removal preserves intended value and saved state. |
| ECON-04 | C | Complete mining/conveyor/machine production as one usable loop. | Inputs, routing, blockage, output collection and upgrades are visible and restore correctly. |
| ECON-05 | C | Complete seed discovery, planting, care, harvest and genetics surfacing. | Shipped plant breadth is obtainable; growth differences matter and can be understood without reading design documents. |
| ECON-06 | C | Validate offline progression, caps and time anomalies. | Quit/reload and clock changes cannot create runaway growth/resources; legitimate progress remains consistent. |
| ECON-07 | H | Tune interaction reach, clarity, economy pacing and usefulness. | A player can learn the loop, earn a desirable upgrade and return without UI confusion or grind hiding missing content. |

## UX — Menus, onboarding, accessibility and player feedback

**Existing baseline:** Home Hub, field menu, objectives, scanner and comfort foundations exist. Earlier device failures must not be assumed fixed.

**Dependencies:** RUN lifecycle and SAVE state; inspect design/COMFORT_AND_ACCESSIBILITY.md and LOCALIZATION_DECISION.md.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| UX-01 | N | Inventory every player-facing menu and state transition. | Title/New/Continue/settings/pause/objectives/inventory/crafting/travel/death or recovery/credits have explicit paths and owners. |
| UX-02 | C | Repair navigation, focus, repeated selection and scene-change behavior. | No dead second click, blocked controller ray, stranded submenu or inconsistent return/back behavior. |
| UX-03 | C | Unify visual hierarchy, readable distances and direct/ray affordances. | Interactive controls are recognizable and reachable; long labels, status changes and errors fit the shared components. |
| UX-04 | C | Finish tutorial observation, signature-creature dependencies and final orchestration. | First-hour teaching follows the approved route; optional hints and veteran behavior do not falsely complete objectives. |
| UX-05 | C | Make accessibility/settings persistent and consistently applied. | Supported handedness, seated height, turning, captions, audio/haptic options and input alternatives survive travel/reload. |
| UX-06 | N | Apply the existing localization decision to all new content records. | Text uses the declared seam and expansion limits; older missing-key/fallback behavior is identified before campaign scale. |
| UX-07 | H | Test onboarding and menus with a player who does not know the design. | The first session communicates goals and controls without developer explanation; headset text and affordances are readable. |

## ART — Spatial design, art integration, animation and atmosphere

**Existing baseline:** The salvage tug and drowned ring-city identity are strong. Existing generation/Forge tools need richer composition and safer reusable asset boundaries.

**Dependencies:** Terry owns approval; WORLD preserves overrides; PERF supplies measured limits.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| ART-01 | T | Approve a spatial brief for each production world before detailed dressing. | Arrival, landmark, main/optional route, elevations, sightlines, distances and quiet/challenge moments have a purpose. |
| ART-02 | N | Define interchangeable asset contracts and measurable inspection views. | Scale, grips, sockets, pivots, collider responsibilities, LODs and materials allow visual swaps without changing gameplay. |
| ART-03 | C | Prepare ship, city and interior review renders at real player height. | Arrival, cockpit, dispatch, market, relay, flats and return have useful images; renders are labeled with source and camera. |
| ART-04 | T | Approve modular architecture and hero-object exemplars. | Silhouettes, room proportions, weathering, lighting and material language meet Terry's direction before multiplying variants. |
| ART-05 | C | Integrate approved assets, animation and VFX through stable gameplay roots. | Collisions, grips, hitboxes and interaction anchors survive art replacement; placeholder-critical assets are tracked. |
| ART-06 | C | Finish spatial legibility and useful interiors on the critical route. | Required entrances/routes are readable; rooms provide interaction, story, discovery or rest rather than empty shells. |
| ART-07 | C | Integrate skies, lighting, water and atmosphere coherently. | World-specific mood is preserved within VR comfort and performance constraints; unrelated global settings do not drift. |
| ART-08 | N | Harvest successful world/art techniques into builders and checks. | Each accepted improvement is reusable or intentionally local; the next world benefits from the work. |
| ART-09 | H | Approve lived-in scale, composition and spatial interest in headset. | Terry judges actual presence and route pacing; object counts and scene radius do not substitute for that verdict. |

## AUDIO — Sound, music, RILL and narrative delivery

**Existing baseline:** Playback and story hooks exist; complete event coverage, authored delivery and final audio remain open.

**Dependencies:** GRAPH canon/route, SAVE restored state, NPC/MECH events; Terry owns tone.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| AUDIO-01 | N | Build an event-to-audio/haptics/dialogue coverage inventory. | Important interactions, threats, repairs, travel, rewards and errors have feedback slots and an implementation state. |
| AUDIO-02 | T | Approve sound palette, music behavior and RILL delivery examples. | A small representative set establishes direction before large audio batches or paid generation. |
| AUDIO-03 | C | Implement feedback/mix priorities and bounded audio resource use. | Important cues remain audible; spatialization, concurrency, cleanup and saved settings behave consistently. |
| AUDIO-04 | C | Author per-world narrative line kits with triggers and interruption rules. | Required information survives missed lines/quit/travel; repeated visits do not replay every reveal. |
| AUDIO-05 | C | Integrate Transmission fragments, choices and ending delivery. | Story beats follow actual progression, agree with saved choices and preserve quiet/no-combat sequences. |
| AUDIO-06 | H | Tune timing, captions, intelligibility and emotional pacing in headset. | RILL, music and effects support the player's attention without obscuring threats, objectives or atmosphere. |

## PERF — Performance budgets and technical quality

**Existing baseline:** Static CI counts flag 1,992 ToxicCity renderers and 707 unique materials in a space-lane group; these are not measured headset draw calls or frame rates.

**Dependencies:** BLD current build; profiling access needed for budget calibration, not for source investigation.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| PERF-01 | N | Identify material allocation, duplicated geometry and runtime resource ownership hotspots. | Known builders/runtime paths have concrete allocation causes and bounded optimization candidates. |
| PERF-02 | C | Consolidate repeated materials/meshes without erasing approved variation. | Measured allocation reductions preserve appearance; repeated bakes/travel do not grow instances unexpectedly. |
| PERF-03 | N | Define representative performance scenes and capture protocol. | Dense city, cockpit, flight/combat, vehicles, gardens/factories and transition peaks use reproducible routes. |
| PERF-04 | H | Measure CPU/GPU frame time, memory and thermal behavior on target Quest hardware. | 72 fps project target is assessed with actual traces and long-session evidence; target-device scope is recorded. |
| PERF-05 | C | Optimize measured bottlenecks and enforce calibrated build budgets. | LOD/culling/batching/pooling/resource limits address real costs; hard-cap violations cannot remain silent warnings indefinitely. |
| PERF-06 | C | Add seeded bad-content tests for key guards and cross-domain generation changes. | Material/profile/reference failures actually fail; unexpected travel or ProjectSettings changes require review. |
| PERF-07 | H | Run repeated-travel and long-session stability checks. | Memory, audio voices and object/material counts stay within agreed tolerances and gameplay remains responsive. |

## SLICE — First complete experience and replication proof

**Existing baseline:** W000/W001 route content exists in several forms; W002 is the reuse test, not another bespoke rescue project.

**Dependencies:** BLD/RUN/SAVE/GRAPH/WORLD plus required MECH/NPC/UX/ART/AUDIO/PERF items.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| SLICE-01 | C | Complete the approved W000 teaching and launch sequence. | New Game, movement, keepsake/holster, scanner/repair and launch work in the agreed order with correct state. |
| SLICE-02 | C | Complete the space-to-city-to-flats-to-key route and its approved payoff. | Course, salvage/half A, dispatch, encounters, relay, traversal, expedition/half B and onward route are connected and resumable. |
| SLICE-03 | C | Close failure/retry and alternative-order gaps across the first experience. | Early collection, wrong-order actions, travel away, quit and repeated return cannot strand the player or duplicate rewards. |
| SLICE-04 | H | Complete the first experience twice with save/resume and no developer intervention. | Functional, comfort, presentation and performance verdicts are separate and tied to one APK. |
| SLICE-05 | C | Build W002 from the same production records and shared systems. | Its cistern identity and puzzles differ while travel/save/rewards/tools require no copied runtime workaround. |
| SLICE-06 | H | Accept W002 and record the reuse cost. | Time to first playable, custom runtime changes, manual fixes and post-play rework are measured. |
| SLICE-07 | C | Produce W003-W005 as a small contrasting batch. | Glass Shelf, Broadcast Tomb and Oxidized Canopy demonstrate different pace/space/verbs through reusable systems. |
| SLICE-08 | H | Review the batch and decide whether to scale. | No recurring fundamental failure; ordinary worlds are mostly content/layout/tuning work; special-mechanic exceptions remain budgeted. |

## CAMP — Campaign batches and endgame

**Existing baseline:** W006-W012 have authored foundations; W013-W068 are mostly design seeds. All require production acceptance.

**Dependencies:** SLICE-08 before broad assembly; high-risk prototypes may precede it. Each batch also requires its capability map.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| CAMP-01 | N | Create a production/evidence row for every world and special sequence. | All 69 numbered entries plus Earth Approach have a brief, dependencies, state and acceptance links; none is labeled finished from a story seed. |
| CAMP-02 | C | Reconcile and complete W006-W012 in small working groups. | Authored content matches campaign records, unlocks, saves and distinct world briefs. |
| CAMP-03 | C | Produce W013-W019 after underwater/portal/choice dependencies are proven. | Records, grayboxes and completed routes cover the chapter without unresolved mandatory mechanics. |
| CAMP-04 | C | Produce W020-W028 with explicit quiet and no-job exceptions. | Branch state, traversal tools, signature encounters and W024/W028 intent survive production. |
| CAMP-05 | C | Produce W029-W038 after phase/geometry/advanced-machine dependencies. | Different world forms use tested shared systems and preserve cross-world consequences. |
| CAMP-06 | C | Produce W039-W051 in groups of roughly three to five entries. | Each group has complete routes, persistence, presentation review and known technical limits before expansion. |
| CAMP-07 | C | Produce W052-W061 with late-game capability and continuity checks. | Earlier choices, powers and story reveals are consumed consistently; required paths remain reachable. |
| CAMP-08 | C | Produce W062, Earth Approach and W063-W068 as a branch-aware endgame. | Every intended ending and coda path works from a valid save; credits/replay/postgame follow the approved policy. |
| CAMP-09 | H | Accept each batch and then complete campaign-wide regression routes. | Every world/ending has evidence; branch and old-save cases are tested rather than inferred from one linear playthrough. |

## MODES — Arena, Horde, Tidefront and optional online scope

**Existing baseline:** Substantial side-mode code exists. Photon presence/transport does not establish synchronized gameplay.

**Dependencies:** ORG-05 release decision; shared MECH/SAVE/ECON owners and a reliable campaign baseline.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| MODES-01 | N | Reconcile shared content identity and legitimate mode-specific overrides. | Tools, creatures, ships, skins and feedback are inherited; damage/balance overrides are explicit and validated. |
| MODES-02 | C | Finish a representative Arena/Horde experience before adding breadth. | Match rules, bot fairness, spawn safety, score, restart, rewards and presentation work for repeated matches. |
| MODES-03 | C | Complete Tidefront's local war-table and mission loop. | Strategic outcomes, ground/space missions, economy, saves and hotseat handoff are coherent and readable. |
| MODES-04 | N | Specify the smallest complete online test slice if included in scope. | Room creation/join, authority, sync, disconnect, privacy and operating-cost assumptions are explicit. |
| MODES-05 | C | Implement authoritative gameplay synchronization and room UX if approved. | Fire, hit/disable, score, match state, reconnect and host-loss work through the chosen transport, not only avatars. |
| MODES-06 | H | Run two-headset acceptance when online is in scope. | Repeated matches, latency/disconnect cases and shared content stay consistent; voice/mute behavior is tested if shipped. |

## REL — Release, maintenance and evidence closure

**Existing baseline:** Repository release checklist has open administrative, licensing and packaging holds. Current policy compliance must be rechecked at release time.

**Dependencies:** Accepted shipping scope and campaign; side-mode/online gates only for included features.

| ID | Work mode / state | Task | Acceptance evidence |
|---|---|---|---|
| REL-01 | N | Audit licenses and provenance for code, packages, models, textures, music and voices. | Every shipped asset has usable evidence; existing Photon distribution hold is resolved or the dependency is excluded deliberately. |
| REL-02 | T | Confirm target devices, age audience, localization, support and data behavior. | Release settings and public materials match the actual product; unresolved decisions are prepared with concrete options. |
| REL-03 | T | Complete account/app administration and required public policy/support pages. | Meta organization/App ID and declarations are handled by the account owner; current official requirements are verified before submission. |
| REL-04 | C | Prepare signing, release configuration and build hygiene checks. | Keys are securely backed up; version/signature, ARM64/IL2CPP, permissions and shipping feature flags are verified; dev surfaces are disabled appropriately. |
| REL-05 | C | Validate platform lifecycle and entitlement behavior for distribution. | Focus loss, suspend/resume, offline/error paths and entitlement use the current requirements and do not corrupt progress. |
| REL-06 | H | Run the release acceptance matrix and fix blocking defects. | Campaign branches, controls/settings, tools/modes, saves/updates and performance pass on supported hardware. |
| REL-07 | T | Approve store copy, screenshots, trailer and comfort disclosures. | Materials show real shipping behavior and approved art, with no unimplemented-feature claims. |
| REL-08 | C | Prepare update, rollback, save-compatibility and support diagnostics procedures. | A versioned release can be reproduced, diagnosed and upgraded without avoidable save loss. |
| REL-09 | T | Submit, resolve certification feedback and authorize release. | Distribution happens only after acceptance and Terry's release authorization; certification success is not assumed. |

## Per-world production ledger

This is a coverage ledger, not a new story plan. Names and source maturity below are taken
from `production/BASE_GAME_WORLD_CONTENT_MATRIX.md`; maturity is historical design/authoring
state, **not current playability**. No listed entry is declared accepted by this checklist.
Each entry must pass the reusable checklist below; maintain evidence links when work starts.

| World / sequence | Source maturity | Production disposition |
|---|---|---|
| W000 The Drift In | `MODEL+CONCEPT` — old WorldData job must migrate to first-level v2 | Open — Pilot experience |
| W001 Toxic Venice | `MODEL+CONCEPT` — topology, signature creature and final orchestration open | Open — Pilot experience |
| W002 The Dry Cistern | `RECORD+CONCEPT` — replication-world target | Open — Reuse proof |
| W003 Glass Shelf | `RECORD+CONCEPT` | Open — First small batch |
| W004 The Broadcast Tomb | `RECORD+CONCEPT` — first Transmission fragment | Open — First small batch |
| W005 Oxidized Canopy | `RECORD` | Open — First small batch |
| W006 The Mirror Flats | `RECORD` | Open — Reconcile existing authored path |
| W007 Sable Station | `RECORD` — guard/social runtime must be reconciled | Open — Reconcile existing authored path |
| W008 The Sealed Archive | `RECORD` | Open — Reconcile existing authored path |
| W009 Chitinwall | `RECORD` | Open — Reconcile existing authored path |
| W010 Tidal Array | `RECORD` | Open — Reconcile existing authored path |
| W011 The Hum | `RECORD` | Open — Reconcile existing authored path |
| W012 Mara’s Last Jump | `RECORD` | Open — Reconcile existing authored path |
| W013 Memory Reef | `SEED` | Open — Produce from canon; special route as declared |
| W014 Ashfield Relay | `SEED` | Open — Produce from canon; special route as declared |
| W015 The Soft Geometry | `SEED` | Open — Produce from canon; special route as declared |
| W016 Sable Outpost B | `SEED` | Open — Produce from canon; special route as declared |
| W017 The Long Descent | `SEED` | Open — Produce from canon; special route as declared |
| W018 Wake Guild HQ | `SEED` — faction hub packet required | Open — Produce from canon; special route as declared |
| W019 The Unmade World | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W020 Copper Shore | `SEED` | Open — Produce from canon; special route as declared |
| W021 The Lattice | `SEED` | Open — Produce from canon; special route as declared |
| W022 Bloom Nursery | `SEED` | Open — Produce from canon; special route as declared |
| W023 Sable Vault | `SEED` | Open — Produce from canon; special route as declared |
| W024 The Named Color | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W025 Echoes of 9 | `SEED` | Open — Produce from canon; special route as declared |
| W026 The Scaffold | `SEED` | Open — Produce from canon; special route as declared |
| W027 Warden Graveyard | `SEED` | Open — Produce from canon; special route as declared |
| W028 The World That Won’t Complete | `SEED+SPECIAL` — explicit no-job route type | Open — Produce from canon; special route as declared |
| W029 Pattern Shore | `SEED` | Open — Produce from canon; special route as declared |
| W030 The Low City | `SEED` | Open — Produce from canon; special route as declared |
| W031 Warden Watch | `SEED` | Open — Produce from canon; special route as declared |
| W032 The Drowned Lab | `SEED` | Open — Produce from canon; special route as declared |
| W033 Bloom Cathedral | `SEED` | Open — Produce from canon; special route as declared |
| W034 The Half-Built City | `SEED` | Open — Produce from canon; special route as declared |
| W035 Signal Crest | `SEED` | Open — Produce from canon; special route as declared |
| W036 The Last Relay | `SEED+SPECIAL` — choice state | Open — Produce from canon; special route as declared |
| W037 Warden Standoff | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W038 The Edge | `SEED` | Open — Produce from canon; special route as declared |
| W039 Pattern City | `SEED` | Open — Produce from canon; special route as declared |
| W040 The Tendril Farm | `SEED` | Open — Produce from canon; special route as declared |
| W041 Sable Last Stand | `SEED` | Open — Produce from canon; special route as declared |
| W042 The Listening Post | `SEED` | Open — Produce from canon; special route as declared |
| W043 Warden Parliament | `SEED+SPECIAL` — social/branch packet | Open — Produce from canon; special route as declared |
| W044 The Glass Bloom | `SEED` | Open — Produce from canon; special route as declared |
| W045 Signal Flood | `SEED` | Open — Produce from canon; special route as declared |
| W046 Mara’s Final Offer | `SEED+SPECIAL` — dialogue/choice | Open — Produce from canon; special route as declared |
| W047 The Architect’s Chamber | `SEED` | Open — Produce from canon; special route as declared |
| W048 Chitinwall 2 | `SEED` | Open — Produce from canon; special route as declared |
| W049 Warden Ally Base | `SEED` | Open — Produce from canon; special route as declared |
| W050 The Convergence | `SEED` | Open — Produce from canon; special route as declared |
| W051 RILL Names Itself | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W052 The Memory Sea | `SEED` | Open — Produce from canon; special route as declared |
| W053 RILL’s First World | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W054 The Broken Pattern | `SEED` | Open — Produce from canon; special route as declared |
| W055 Sable Peace | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W056 The Wake Guild End | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W057 Transit Void | `SEED+SPECIAL` — transit-only route type | Open — Produce from canon; special route as declared |
| W058 The Second Cistern | `SEED` | Open — Produce from canon; special route as declared |
| W059 Warden’s Homeland | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W060 The Architect’s Tomb | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W061 RILL’s Last Memory | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W062 The Revelation Chamber | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W063 The Branching | `SEED+SPECIAL` — four-way branch | Open — Produce from canon; special route as declared |
| W064 Ending A — Reseal/Steward | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W065 Ending B — Cross/Break Out | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W066 Ending C — Forget/Reset | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W067 Ending D — Become/Pattern | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| W068 The Final World | `SEED+SPECIAL` | Open — Produce from canon; special route as declared |
| Earth Approach (unnumbered) | Special endgame sequence in accepted plan | Open — explicit sequence record, transitions and branch-aware acceptance |

### Required for every produced world or special sequence

- [ ] Approved purpose and contrast with nearby worlds; retain deliberate quiet/no-job entries.
- [ ] Spatial brief: arrival, landmark, routes, distances/times, useful interiors, elevation and sightlines.
- [ ] Complete production record with stable IDs, capabilities, entry/exit and required content references.
- [ ] Valid prerequisites, obtainable tools, explicit objective/choice/reward rules and persistent changes.
- [ ] Clean generation and data validation; no hidden manual setup or accidental cross-domain edits.
- [ ] Playable start-to-exit route, optional paths where intended, interruption/retry and return behavior.
- [ ] Art/animation/VFX/audio/text integration and recorded placeholders; Terry's artistic review.
- [ ] Representative device performance, comfort and readability evidence.
- [ ] Revisit/save/upgrade behavior and applicable branch tests; defects linked to their shared owner.
- [ ] Source/build/evidence linked and acceptance recorded before the entry is called complete.

## Decision queue for Terry — asynchronous, not a test request

| Decision | Engineering preparation | What can proceed without the decision |
|---|---|---|
| First-hour payoff and route | Compare the two existing contracts; propose a coherent beat sequence | Ownership mapping, checkpoint design, toolchain verification |
| W002 tool unlock conflict | List chapter references and dependent puzzle requirements | Generic record/schema work and flag analysis |
| MK2 acquisition/transfer | Show candidate story placements and save/inventory consequences | Ship storage/identity audit |
| Online/base side-mode scope | Show smallest complete modes, extra acceptance/cost and deferral options | Shared identities and offline campaign engineering |
| Ending/replay/postgame | Present choices with branch/save/reward implications | Branch-aware graph validation |
| Artistic exemplars | Prepare actual review images/briefs for ship, city, signature creature and interfaces | Asset socket contracts and technical constraints |

## Limits and maintenance

This consolidates the audited plan and existing detailed roadmaps into actionable delivery
work. It is not a fresh proof of every implementation, a new feature wishlist or a promise of
AAA scope on an indie schedule. New gaps go under their owning system; existing work is inspected
before replacement. Do not inflate completion by counting documentation or data rows as gameplay.

Use `PROJECT_COMPLETION_ROADMAP.md` for detailed product-quality criteria,
`EXCELLENCE_MAP.md` for existing quality guards,
`production/SHARED_CONTENT_INHERITANCE_CONTRACT.md` for cross-mode ownership,
`production/ARSENAL_AND_POWER_PROGRESSION.md` for planned capabilities,
`production/BASE_GAME_WORLD_CONTENT_MATRIX.md` for campaign coverage,
`design/COMFORT_AND_ACCESSIBILITY.md` and `design/LOCALIZATION_DECISION.md` for existing decisions,
`META_STORE_READINESS.md` for the release evidence ledger, and `QUEST_RECOVERY.md` for operator commands.
When these disagree, open a concrete reconciliation task rather than silently choosing convenient prose.

The first implementation target after this checklist is **the shared-state ownership and
mission-persistence specification**, alongside the campaign dependency inventory. Hardware
acceptance remains queued for a day Terry can test.
