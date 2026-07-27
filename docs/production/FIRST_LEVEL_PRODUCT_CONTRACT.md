# FIRST LEVEL PRODUCT CONTRACT — W000 → SPACE → W001 → RETURN

**Status:** CANONICAL PRODUCTION TARGET · implementation remains gated by the current M0 Quest verdict  
**Written:** 2026-07-27  
**Scope:** the complete first playable level and the reusable route framework that later worlds inherit

## 0. The blunt verdict

ZIPTIDE does **not** yet have one coherent, authoritative first-level implementation.

It currently has three partially overlapping products:

1. a recovery route that proves `_Boot → W000 → ToxicCity → W000`;
2. a 22-beat first-hour contract that moves from the W000 helm directly into the first Ziptide and W001;
3. a separate `SpaceLane_Trial` flight/combat/salvage program that is not integrated into the first-hour contract and whose generated scene/WorldPack are not committed.

Those pieces are valuable, but they are not yet one level. This document defines the one product they must become.

The first level is **not only ToxicCity/W001**. The first level is the complete band:

> cold boot/home → W000 wake and ship restoration → launch → playable space flight → non-lethal salvage → first Ziptide → destination approach/reentry → W001 arrival and job → creature/traversal/story payoff → extraction → changed ship/home/save payoff.

W001 proves the product. W002 proves the framework.

---

## 1. Authority and migration law

### Current runtime truth

Until implementation changes land, these remain the executable authorities:

- `docs/first_hour/first_hour_beats.json` and `FirstHourProgressCore` for the current 22-beat progression;
- `ShipCastOffRuntime` for W000 launch and its current direct destination;
- `TravelCoordinator` as the only scene-travel owner;
- `ShipFlightRuntime` + `FlightModel` + `FlightInputCore` + `FlightCourseCore` for flight;
- `SpaceCombatCore` + `SpaceTargetRuntime` for the current disable/salvage trial;
- `JobRuntime`/`JobDirector`, `RepairableMachine`, `RewardRouter`, `SaveSystem`, and the existing creature/traversal owners for surface gameplay.

### Production-target truth

This document is the canonical target for the **first-level product** and for the v2 first-hour migration. Where the current `first-hour-v1` sequence conflicts with this document, the conflict is a named implementation gap—not permission to reinterpret live code silently.

### No parallel-system law

The migration must extend or rebind the existing owners. It must not create:

- a second travel coordinator;
- a second flight model;
- a second repair, job, reward, save, scanner, creature, inventory, or route-progress state;
- direct `SceneManager` travel around `TravelCoordinator`;
- a first-hour-only economy or salvage path;
- a giant new “manager” that takes ownership away from the established systems.

The final first-hour director/orchestrator observes canonical signals, advances the contract, and requests existing owners. It does not reimplement their rules.

---

## 2. Contradictions that must be removed

### C1 — the mandatory W000 coupler is absent from the first-hour contract

`ShipCastOffRuntime` blocks `PUNCH IT` until `gate_coupler` is repaired, and the current Quest route requires the complete panel → part → power interaction. The 22-beat first-hour contract contains no W000 coupler beat. It instead teaches scan/repair later in W001.

**Resolution:** W000 becomes the first scan/repair tutorial. W001 uses those learned verbs in a real contract and must not pretend they are being taught for the first time again.

### C2 — the first-hour contract bypasses the flight program

Current beats move from `FH_INTERACT_HELM` to `FH_FIRST_ZIPTIDE` and then `FH_W001_ARRIVAL`. `CONTROLS_AND_FLIGHT.md` says the short guided flight occurs before the first Ziptide. Terry’s intended opening includes launch, space flight, salvage, and reentry.

**Resolution:** the model-band sequence is W000 → production space route → first Ziptide → W001 approach/reentry/arrival.

### C3 — `PUNCH IT` currently travels directly to ToxicCity

The live cast-off sequence is a six-second star-streak presentation followed by `TravelCoordinator.TravelTo(ToxicCity)`.

**Resolution:** the production route sends `PUNCH IT` to the route’s authored space leg. Completion of the required space beats unlocks the W001 approach/Ziptide leg. Recovery builds may retain a deliberately named direct-route fallback, but production progression may not silently use it.

### C4 — SpaceLane is a trial, not a route leg

The source author creates a bounded test course with five rings, three passive targets, a dock, and `RETURN HOME`. The scene and WorldPack are not committed on the live branch. It has no destination approach, reentry, landing handoff, first-hour progression integration, or W001 route data.

**Resolution:** preserve the tested flight core, but promote the scene/data into a generated production route leg with destination, approach, encounter, completion, and evidence contracts.

### C5 — reentry exists as an idea, not an implementation

There is no canonical reentry/approach owner or beat. Full seamless planet-scale flight is not required for the first model band.

**Resolution:** v1 reentry is an authored transition act around canonical travel: stable cockpit, destination growing in the view, atmospheric streak/haze, Ziptide crest hiding the scene load, and a destination arrival/descent presentation. The camera never gets animated or parented. Later seamless-atmosphere technology may replace the visual leg without changing progression or ownership.

### C6 — W001’s approved city and arrival are not the built city

The approved concentric-ring city, K2 skyscape, K3 street target, K4 facade grammar, harbor, outskirts, tower, and gate pillars are not represented by the current older boulevard graybox.

**Resolution:** W001 Stage A, facade grammar, skyscape, and arrival composition are part of this first-level product—not a disconnected art sprint.

### C7 — the first-hour payoff remains incomplete

`FH-A01` signature-creature presentation, `FH-S05` creature resolution, and `FH-S08` final orchestration remain unbuilt. The current contract’s changed-ship payoff is therefore not a proven end-to-end experience.

**Resolution:** these are required model-band exit work, not optional later polish.

---

## 3. Canonical first-level route

This is the complete required player route. Exact line wording and tuning remain content data; the order and ownership seams are locked.

### Phase A — cold boot and home choice

1. Cold launch reaches the live Home Hub.
2. New Game and valid-save Continue are readable and selectable.
3. Boot hold prevents unsafe movement while `_Boot` owns the player.
4. New Game performs one canonical travel into W000.

**Proof:** recovery boot route, real rays/direct-hand fallback, no duplicate rig/camera/input owner.

### Phase B — W000 wake, identity, and ship restoration

5. The player sees RILL and receives the first short orientation beat.
6. Comfort choice is available without blocking progress.
7. Movement and turning are taught in a safe ship/home space.
8. The player grabs and holsters one personal/tool item.
9. The player scans the failed coupler or its clearly marked fault.
10. The player pulls the access panel.
11. The player retrieves and seats the replacement part.
12. The player presses the reachable power control.
13. The ship visibly and audibly changes state; launch becomes armed.

**Teaching law:** SCAN and REPAIR are taught here once. Later repairs reinforce them.

### Phase C — board, select, and launch

14. The player boards the real ship/cockpit deck.
15. The helm identifies W001/ToxicCity as the first destination.
16. The player commits with `PUNCH IT`.
17. Cast-off/ascent presentation begins from aboard the ship.
18. `TravelCoordinator` loads the authored production space route exactly once.

**Owner law:** `ShipCastOffRuntime` owns arming/launch request; `TravelCoordinator` owns scene transition.

### Phase D — playable space tutorial

19. The player takes the helm in a stable cockpit frame.
20. Throttle/reverse and steering are demonstrated through generous route geometry.
21. Boost is required once in a safe section.
22. The player completes the short guided course or reaches the approach gate.
23. Flight remains comfort-capped; the rig is never parented to a moving hull.

**Minimum controls:** throttle/reverse, strafe/assist as currently supported, pitch/yaw, boost. Roll may remain optional in the first route even if the general flight mode supports it.

### Phase E — non-lethal space salvage

24. A controlled space target or disabled derelict is readable off the main route.
25. The player uses the ship tool/weapon to disable rather than destroy where required.
26. The disabled target visibly powers down.
27. The player flies into salvage range or uses the future canonical tractor seam.
28. Salvage pays through the one economy and persists.
29. Route progression receives one neutral `SPACE_SALVAGE_COMPLETED`-class signal; it does not own the payout.

**First-route requirement:** at least one salvage event is mandatory because this is the first proof that flight feeds the economy. Later world routes may configure encounters as optional or replace the target recipe.

### Phase F — first Ziptide, approach, and reentry

30. Course/salvage completion exposes the W001 approach anchor.
31. The first Ziptide is initiated from the space leg, not from the W000 floor.
32. The canonical crest/flash hides the exact scene load.
33. Destination presentation sells approach/reentry without moving the camera: atmosphere, haze, speed streaks, city/planet growth, and stable cockpit framing.
34. Arrival hands off once to the authored W001 spawn/arrival frame.
35. Input, hands, inventory, comfort, and saved salvage are restored before gameplay resumes.

**Upgrade seam:** a later seamless descent can replace steps 32–34 if it preserves the same signals, destination data, comfort rules, and travel owner.

### Phase G — W001 authored arrival

36. The player arrives facing a deliberate composition: harbor/route, dominant Tower, city layers, skyscape, and a visible return reference.
37. RILL names the immediate problem without a text dump.
38. The contract surface is readable and reachable.
39. The intended route is legible through landmarks and world construction, not floating-arrow dependence.

### Phase H — first real surface contract

40. The player accepts one complete W001 contract.
41. Scan is used as an already-learned verb to identify the job fault.
42. Repair/action uses the same canonical physical repair owner.
43. A safe tool/practice interaction teaches discharge before pressure begins.
44. The signature creature receives a safe observation/read window.
45. The player resolves the creature encounter non-lethally through a readable tool/environment counter.
46. The job route uses the designated zipline/traversal moment.
47. The contract pays through canonical rewards.
48. One physical story fragment or choice lands.
49. The world visibly changes because of the completed work.

### Phase I — extraction and return

50. The intended return path leads back to the ship/berth; the field menu remains a recovery/comfort escape, not the primary fiction path.
51. The player boards and commits the return.
52. A shortened extraction/ascent/Ziptide act returns through `TravelCoordinator` to W000.
53. The route cannot double-launch, double-pay, or lose inventory during return.

The first production version does not require a second full combat course on the return trip. The route framework must nevertheless support outbound and inbound space-leg recipes independently.

### Phase J — home payoff and persistence

54. W000/ship visibly reflects the first job and/or collected salvage.
55. The player sees one meaningful use of the reward loop: repair, fabrication, upgrade, unlock, or another canonical spend surface.
56. RILL/ship state delivers the first-hour payoff and next mystery.
57. Autosave completes.
58. Quit/relaunch/Continue restores a valid ship, inventory, economy, route state, and world completion state.

---

## 4. Honest current-state matrix

Legend: ✅ proven on the named automated/device layer · 🟢 code/CI exists but product/device integration pending · 🟡 partial or separate proof · 🔴 missing from the product route.

| Product slice | Current state | Existing truth | Required to close |
|---|---:|---|---|
| Exact recovery baseline | 🟢 | `cf94c608` ordinary CI green, Recovery PlayMode 43/43, Golden Android built; Quest retest pending | Terry’s two-route device verdict and blocker classification |
| Home Hub/New/Continue | 🟢 | Runtime and recovery proof exist | Latest exact-source Quest comfort/reach/readability proof |
| W000 locomotion/hands | 🟢 | Prior device proof; recovery fixes merged | Latest exact-source repeat proof |
| RILL/comfort/first item | 🟡 | Adapters/surfaces exist | Final W000 orchestration, content, device proof |
| W000 coupler | 🟢 | Real three-stage repair; launch arming; prior device completion | Add it to the machine first-hour contract; scanner/teaching integration; latest feel proof |
| Ship boarding/helm | 🟢 | Boardable ship and destination selection exist | Make it the real route entry and verify child/seated reach |
| `PUNCH IT` | 🟢 | Rails launch + direct travel to ToxicCity | Retarget production flow to authored space leg; retain bounded recovery fallback explicitly |
| Ascent to space | 🟡 | Star-streak launch presentation exists | Authored launch leg, completion signal, audio/visual polish, device comfort |
| Flight core/controls | 🟢 | `FlightModel`, input shaping, loadout mapping, course, comfort reporting | Production integration, resource/perf cleanup, device tuning |
| Space scene/world pack | 🔴 | Idempotent `ScenePatcherSpaceLane` source exists | Generated scene and WorldPack are absent from live repo; production data/author/audit/build profile required |
| Flight course | 🟡 | Five-ring trial data exists in patcher | Route-defined course, arrival gate, first-hour signals, device proof |
| Ship visuals/cockpit | 🟡 | Functional static cockpit frame and broader ship systems exist | First-route production cockpit/visibility/readability/asset pass |
| Space disable/salvage | 🟡 | Minimal three-target loop, pure rules, economy payout, logs | Production encounter data, progression signal, visual quality, device proof; later weapon/AI/POI breadth |
| Full ship weapons/abilities/AI | 🔴/later | Design exists; minimal direct fire only | Definitions, hardpoints, AI and encounter POIs when the model-band need proves them |
| First Ziptide from space | 🔴 | Gate effect exists globally; current first Ziptide departs W000 | Space-approach binding and route data |
| Reentry/landing handoff | 🔴 | No canonical owner/beat | Data-driven transition act, comfort/audit rules, W001 handoff signal |
| W001 arrival composition | 🟡 | Observation adapter and generic scene exist | Production arrival, landmark composition, city/skyscape integration |
| W001 city topology | 🔴 | Approved K1/K7 measured spec | Stage A ring/canal/wedge/harbor/outskirts recipe and generated build |
| W001 facade/street grammar | 🟡 | Building kit and interiors exist | K4 module breadth and K3 street comparison loop |
| W001 skyscape | 🟡 | SkyVista system and approved K2 references exist | Signature W001 authoring and device verdict |
| First contract | 🟡 | Job runtime, repair, objective board, rewards exist | One authored end-to-end job aligned to the route and final orchestration |
| Signature creature | 🔴 dependency | Creature systems exist | `FH-A01` art → `FH-S05` resolution → device proof |
| Zipline/traversal | 🟢 | Core/runtime/adapter built | Place on real job route and prove on device |
| Reward + visible use | 🟡 | Economy/reward chokepoints exist | Route-specific reward, one visible spend/payoff, persistence proof |
| Return/extraction | 🟡 | Direct canonical travel and field-menu escape exist | Fictional ship return route plus outbound/inbound route data |
| Changed-ship payoff | 🔴 orchestration | Beat exists in v1 contract | `FH-S08`, real visible state, content/audio, save/continue proof |
| Audio/SFX | 🔴 production | Procedural ambience/gate/repair sounds; sparse music | Stable event rails, buses/settings/ducking/captions, then first paid batch |
| Full-route performance/evidence | 🔴 | Per-scene audits and runtime health exist | One exact profile for W000→space→W001→return, artifact packet and Quest evidence |

---

## 5. Canonical ownership map

| Responsibility | Surviving owner | Framework rule |
|---|---|---|
| Boot choice and safe release | `BootLoader` / `HomeHubRuntime` | No world route may own `_Boot` locomotion or invent another home choice |
| Persistent rig/input/inventory restoration | `PlayerRigPersistence` and existing input owners | Route presentation cannot mutate or duplicate rig ownership |
| First-level sequence evaluation | `FirstHourProgressCore` + contract data | Upgrade to v2 beats; keep pure deterministic progression and persisted completed IDs |
| Coupler/physical repair | `RepairableMachine` | Same stages/signals used wherever the repair grammar appears |
| Jobs/contracts | `JobRuntime` / `JobDirector` | Route director observes; it does not advance jobs itself |
| Objective presentation | `ObjectiveBoard` | Presentation only; no reward/save/progression ownership |
| Launch arming and cast-off request | `ShipCastOffRuntime` | May select a route/destination but never load scenes directly outside `TravelCoordinator` |
| Scene travel and Ziptide load masking | `TravelCoordinator` | One travel owner for W000, space, W001 and return |
| Flight rules | `FlightModel` / `FlightInputCore` / `FlightCourseCore` | Pure math stays reusable and independent of first-hour story |
| Flight scene/body | `ShipFlightRuntime` or its bounded successor | Translator only; consumes route data and emits neutral flight signals |
| Space disable/salvage rules | `SpaceCombatCore` / `SpaceTargetRuntime` | No lethal explosion requirement; payout remains one-economy |
| Economy/rewards | `RewardRouter`, existing ledger/profile services | Salvage, job reward and spending share existing chokepoints |
| W001 world generation | WorldSpec/compiler + `CityLayoutDefinition`/`CityBuilder`/authors | No hand-edited scene YAML; W001 is real production data |
| Creature encounter | Existing creature behavior/state owner + `FH-S05` adapter | First-hour only observes the canonical resolution signal |
| Save/continue | `SaveSystem` / profile/world state | Every route leg must survive pause, travel, quit and relaunch |
| Final orchestration | `FH-S08`/story-ship lane | Coordinates presentation and hints; never becomes a second gameplay owner |

---

## 6. Reusable route framework

The first level must produce a reusable **world-episode route**, not a ToxicCity special case.

### Fixed framework stages

Every full episode may draw from this ordered vocabulary:

1. home/preparation;
2. launch requirement;
3. surface cast-off;
4. outbound space leg;
5. optional/required encounter or salvage;
6. Ziptide transit;
7. approach/reentry;
8. authored destination arrival;
9. surface contract/story/traversal;
10. extraction;
11. inbound route;
12. home payoff/save.

Later worlds may configure some legs as short, optional, substituted, or already-cleared. The first level must exercise the complete vocabulary once.

### Per-route data that must become authoritative

The implementation must place these fields in the narrowest existing schemas that can own them. Do not invent a parallel mega-schema merely to mirror this list.

- route/episode id;
- home scene and home spawn;
- destination world/scene and display identity;
- launch prerequisite and launch surface;
- outbound space scene/recipe;
- flight course/approach anchors;
- encounter recipe and required salvage count;
- Ziptide destination/visual palette/audio event ids;
- approach/reentry presentation recipe;
- destination spawn, arrival facing and landmark roles;
- primary job id and required interaction sockets;
- creature encounter id and non-lethal resolution signal;
- traversal beat id;
- story fragment/choice id;
- reward and visible payoff/spend surface;
- return/extraction route;
- persistence flags;
- performance budgets;
- automated acceptance rules;
- exact device questions/evidence tags.

### Per-world variable content

W002 and later worlds replace data/recipes, not generic runtime:

- layout/biome/city grammar;
- arrival image and landmarks;
- flight course shape;
- space encounter family;
- approach/reentry art and audio;
- contract steps and physical props;
- creature and counter;
- traversal verb;
- story beat;
- reward/payoff;
- return staging.

### Repetition law

- First world: classify each correction.
- Second world: repeated correction defaults to a missing schema/compiler/audit rule.
- Third one-off implementation is prohibited without a written exception.

---

## 7. Implementation program after the M0 verdict

This is one large product sprint delivered through bounded, green commits. Workstreams may run in parallel only after the contract/ownership pass prevents collisions.

### Wave 0 — classify tomorrow’s M0 result

- systemic rig/input/travel/save/coupler blocker: fix first;
- bounded feel/content/presentation note: ledger it without freezing the model-band program;
- preserve exact known-good APK/SHA before new source work.

### Wave 1 — first-hour v2 contract reconciliation

1. Replace the 22-beat route with a v2 sequence covering the canonical phases above.
2. Move first SCAN/REPAIR teaching to W000 coupler; make W001 reinforcement/application.
3. Add flight, salvage, approach, reentry and extraction signals.
4. Define safe v1-profile migration from completed beat IDs.
5. Update validators, envelopes, binding inventory and pure progression tests.
6. Prove every required beat has a real producer or an explicitly blocked dependency.

**Exit:** no route contradiction remains on paper or in machine-readable contract data.

### Wave 2 — production space-route generation

1. Convert the trial constants into route-owned data/recipe fields.
2. Generate and commit the production space scene/WorldPack through idempotent authors.
3. Include it in the correct development/Golden build profile intentionally.
4. Retarget production `PUNCH IT` from direct ToxicCity travel to the outbound space route.
5. Add neutral signals for helm, course, disable, salvage and approach readiness.
6. Add build-profile, route-continuity, reach and resource-discipline tests.

**Exit:** W000 → generated space route → W001 can run in PlayMode without developer intervention.

### Wave 3 — ascent, first Ziptide, approach and reentry

1. Build data-driven launch/approach/reentry presentation around the existing travel owner.
2. Preserve stable cockpit/HMD sovereignty.
3. Add timeouts/fallbacks so presentation can never strand travel.
4. Add destination identity, W001 arrival-facing and inventory/input readiness checks.
5. Add audio event seams and caption twins before final assets.

**Exit:** the route reads as one journey rather than three unrelated scene loads.

### Wave 4 — flight and salvage production pass

1. Replace trial-only labels/geometry with the first-route cockpit/course/encounter presentation.
2. Keep the minimal honest loop: fly → disable → approach → salvage → paid.
3. Correct known resource/material-instancing debt before repeated flight sessions.
4. Validate loadout stats, comfort, course readability, salvage persistence and no duplicate payout.
5. Device-tune only after exact-source automated green.

**Exit:** a first-time player can complete the flight/salvage leg comfortably without explanation from Terry.

### Wave 5 — W001 model city and arrival

1. Build City Stage A from the approved concentric-ring recipe.
2. Add booth reference-plate comparisons for aerial, arrival, skyline and K3 street views.
3. Build K4 facade module grammar.
4. Author the W001 skyscape signature pass.
5. Align arrival, return berth, contract path and city landmarks.

**Exit:** W001 reads as the approved city in comparison artifacts and on Quest.

### Wave 6 — first contract, creature, payoff and audio rails

1. Author one complete W001 contract using learned verbs.
2. Finish `FH-A01` → `FH-S05` → `FH-S08` in dependency order.
3. Place creature observation/counter and zipline on the real route.
4. Route rewards and one visible use/payoff.
5. Build mixer/bus/event/settings/ducking/caption/license rails.
6. Add final first-hour presentation using placeholders where paid assets are not yet justified.

**Exit:** the complete first level can be played, understood, completed, returned from, saved and continued.

### Wave 7 — W002 replication proof

1. Create W002 from the same episode packet/template and route vocabulary.
2. Use different world, job, creature, approach and content recipes.
3. Reject W001 world IDs, scene paths and assumptions from generic code.
4. Measure effort by spec, generation, verification, device and polish stages.

**Exit:** W002 reaches a complete loop materially faster and mostly through data/recipes. If not, repair the factory before W003.

---

## 8. Proof ladder and model-band exit gate

The first level is not complete because a test says “green.” It closes only when all named layers agree.

### Automated

- exact-source ordinary CI: EditMode + patch/world audit green;
- first-hour-v2 pure sequence tests and producer/binding coverage green;
- generated production space route and W001 outputs current, deterministic and audit-clean;
- full PlayMode route: boot → W000 → space → W001 → return → relaunch/Continue;
- no duplicate beat, payout, travel, rig, job, repair or save owner;
- exact-source Golden Android artifact inspected and hashed.

### Device

- complete route twice without developer intervention;
- W000 coupler reachable for child-height, seated and standing play;
- flight controls understandable and comfortable;
- salvage readable and persistent;
- Ziptide/approach/reentry feels continuous and never takes camera control;
- W001 arrival landmark and route readable;
- full contract/creature/zipline/reward sequence completes;
- intended ship return works; field-menu escape remains available;
- quit/Continue restores correct state;
- no blocker crash, fall loop, input loss, warning spam, cumulative material growth, or unacceptable frame pacing.

### Product

- the first level has an emotional arc, not only a systems checklist;
- each major mechanic produces a visible consequence;
- city/ship/space presentation reaches the approved reference direction;
- audio events and captions cover all progress/safety-critical moments;
- Terry can request content changes without requiring foundational runtime surgery.

---

## 9. What this document deliberately does not authorize

- Starting broad W003+ production before W002 proves reuse.
- Building seamless planet-scale flight, floating origin, or moving walkable-ship physics for the first model band.
- Rewriting `TravelCoordinator`, the persistent rig, save/profile, job, repair, reward, scanner, or economy owners.
- Hand-editing `.unity` or `.prefab` YAML.
- Buying broad 3D/audio batches before their import/event rails are stable.
- Treating the current `SpaceLane_Trial` source as a shipped first-level scene.
- Treating W001 city concept documents as implemented content.
- Closing first hour before signature creature resolution and final orchestration exist.

---

## 10. Immediate next action

1. Terry runs the exact `cf94c608` Quest packet.
2. Record the M0 verdict and classify every finding.
3. If there is no systemic blocker, start **Wave 1 immediately**—not another broad planning pass.
4. The first source sprint after Wave 1 is the production route seam: W000 `PUNCH IT` → generated space leg → W001 approach/reentry, preserving `TravelCoordinator` ownership.
5. City Stage A, route presentation, and first-hour content then proceed as coordinated workstreams under this one product contract.

The project reaches the required foundation when the route above is real, device-proven, and W002 can replace its content through data rather than a new code campaign.
