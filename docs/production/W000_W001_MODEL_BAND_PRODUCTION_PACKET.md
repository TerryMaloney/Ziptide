# W000 → W001 MODEL-BAND PRODUCTION PACKET

**Status:** PRE-M0 REVIEW DRAFT — documentation and coordination only  
**Written:** 2026-07-24  
**Integration owner:** GPT-5.6 Thinking  
**Execution authority:** none until the exact `c45b1a2` headset route passes, baseline blockers close, and the recovery freeze is explicitly lifted for named paths.

This packet prepares the W000→W001 first-hour model band so production can begin immediately after M0. It does not authorize runtime, scene, prefab, audio-asset, WorldSpec, save-schema, recipe, creature, weapon, travel, coupler, XR-rig or APK changes before the device verdict.

---

## 1. Purpose

W000 and W001 are one first-hour product and the gold-standard world-production assembly line.

The model band must prove that ZIPTIDE can combine:

- cold boot and profile selection;
- body/comfort onboarding;
- RILL presence and story delivery;
- physical grabbing, holstering and interaction;
- ship preparation and Ziptide travel;
- a readable arrival composition;
- one complete technician job;
- one non-lethal signature-creature encounter;
- one physical story/progression object;
- one useful cultivation/invention payoff;
- save, return and visible consequence;
- automated evidence plus Terry’s device verdict.

W002 later proves this was a reusable factory rather than a W001-specific monument.

---

## 2. Precedence and existing authorities

This packet points to existing authorities instead of replacing them.

| Concern | Authority |
|---|---|
| Exact pre-production prerequisite | `docs/testing/HEADSET_RETRY_C45B1A2.md` |
| Post-headset execution order | `docs/production/POST_HEADSET_WORLD_FACTORY_ORDER.md` |
| First-hour ordered semantics | `docs/first_hour/first_hour_beats.json` |
| Current owner/evidence inventory | `docs/first_hour/first_hour_bindings.json` |
| First-hour experience and pacing | `docs/GPT_ADDITIONS/2026-07-10_GPT56_FIRST_HOUR/FIRST_HOUR_VERTICAL_SLICE_MASTER_PLAN.md` |
| Growing/invention decisions | `docs/growing/GROWING_INVENTION_LOOP_DECISION_RECONCILIATION.md` |
| Audio IDs and first-slice queue | `docs/audio/AUDIO_VERTICAL_SLICE_QUEUE.md` and `docs/audio/AUDIO_PRODUCTION_MASTER_PLAN.md` |
| Current project truth | `docs/STATE_OF_THE_GAME.md` |
| Quality and device truth | `docs/EXCELLENCE_MAP.md`, `docs/DEVICE_TEST_CHECKLIST.md`, `docs/MISS_LEDGER.md` |
| World production truth | future committed `docs/worldspecs/*.spec.json` through the existing WorldSpec compiler |

If this packet conflicts with a locked contract above, the locked contract wins unless a later reviewed reconciliation explicitly changes it.

---

## 3. M0 boundary

Before M0 closes, do not modify or replace:

- XR rig or input lifecycle;
- weapon scale, pose, attachment, colliders or hand interaction;
- coupler height, visibility, release, selection or power sequence;
- PUNCH IT, ToxicCity travel, arrival or post-travel input restoration;
- recovery scenes or the authorized Golden APK;
- the first-hour runtime merely to make this document appear implemented.

Safe before M0:

- this packet;
- source-owner audits;
- content questions and decision tables;
- art/audio socket inventories;
- post-M0 implementation envelopes;
- data-only review drafts that do not alter current runtime evidence.

---

## 4. Current verified truth

### 4.1 What exists

The project already has canonical owners for:

- Home Hub boot selection;
- saves/profile persistence;
- RILL presentation;
- comfort and locomotion;
- XR grabbing and holsters;
- three physical belt sockets;
- TravelCoordinator and ShipCastOffRuntime;
- jobs, rewards and economy;
- WristScanner/IScannable;
- RepairableMachine physical stages, public stage state and stage event;
- weapons and practice targets;
- creature runtime and non-lethal disable;
- zipline traversal;
- garden genetics, watering and authored species;
- recipes, machines, belts and persistence;
- generated world content, audits and CI.

No replacement owner is permitted for those responsibilities.

### 4.2 W001 identity conflict — must remain explicit

Current first-hour data names `W001_ToxicCity`.

The current state audit says:

- the canonical W001 scene slot is empty;
- ToxicCity is standing in;
- the WorldSpec pipeline has zero committed production specs.

Therefore this packet uses **`W001_MODEL_WORLD`** as a planning alias until Terry and the story/world lane approve the canonical W001 identity.

Rules:

1. ToxicCity may remain the mechanical rehearsal scene.
2. No new generic system may hard-code `ToxicCity` as the permanent W001 identity.
3. World-specific art, story, sky, creature and seed decisions remain provisional until the identity gate closes.
4. The first authoritative W001 WorldSpec must be committed before the model band can claim factory truth.

---

## 5. Emotional arc

The first hour should produce this player interpretation:

1. **Trust:** “This is a real game and I understand how to begin.”
2. **Presence:** “I have a body, a ship and a companion.”
3. **Agency:** “I can grab, carry, repair and choose tools.”
4. **Awe:** “The Ziptide takes me somewhere worth seeing.”
5. **Competence:** “I understood a problem and fixed it physically.”
6. **Mercy/intelligence:** “I solved a creature encounter without treating life as disposable.”
7. **Ownership:** “The world and ship remember what I did.”
8. **Possibility:** “The seed, invention and next mystery give me a reason to continue.”

The camera is never forcibly moved. Help is hesitation-triggered and RILL-led. Critical information never relies on color or audio alone.

---

## 6. Target pacing

Timings are tuning targets, not locks.

| Band | Target | Purpose |
|---|---:|---|
| Cold boot | 0–2 min | New/Continue/Settings and trust |
| W000 wake/body | 2–10 min | Meet RILL; comfort, move, grab, holster |
| W000 ship ownership | 10–17 min | Personal object, planter promise, launch preparation |
| First Ziptide | 17–20 min | First spectacle and title meaning |
| W001 arrival | 20–28 min | Quiet orientation, return path and world identity |
| Technician job | 28–42 min | Scan, access, part, power, collect, confirmation |
| Signature creature | 42–52 min | Foreshadow, observe, understand, non-lethal counter |
| Reward/consequence | 52–57 min | Payment and persistent world change |
| Return/invention | 57–65 min | Changed ship, grown plant, first invention and next mystery |

If the model band consistently exceeds the range because of required interaction rather than exploration, remove duplicated teaching before increasing the target.

---

## 7. Ordered beat groups

`docs/first_hour/first_hour_beats.json` remains the ordered semantic authority. This table groups those beats into production packages without redefining their IDs.

| Group | Existing beats | Required result |
|---|---|---|
| Boot | `FH_BOOT_READY`, `FH_NEW_GAME_SELECTED` | Clean player-facing start and valid profile |
| Meet/body | `FH_LOOK_AT_RILL`, `FH_COMFORT_CONSOLE`, `FH_MOVE_IN_QUARTERS` | RILL presence, comfort confirmed, safe locomotion |
| Hands/loadout | `FH_GRAB_BUNK_OBJECT`, `FH_HOLSTER_FIRST_ITEM` | One meaningful object grabbed and reliably holstered |
| Ship/travel | `FH_INTERACT_HELM`, `FH_FIRST_ZIPTIDE` | First destination selected and travel completed |
| Arrival/job | `FH_W001_ARRIVAL`, `FH_ACCEPT_FIRST_JOB`, `FH_SCAN_FAULT` | World understood, job accepted, fault located |
| Repair | `FH_REPAIR_ACCESS`, `FH_REPAIR_PART_SEATED`, `FH_MACHINE_POWER_CYCLE` | Physical repair completes and changes world state |
| Tool/creature | `FH_SHOOT_PRACTICE_TARGET`, `FH_OBSERVE_SIGNATURE_CREATURE`, `FH_COUNTER_SIGNATURE_CREATURE` | Tool understood; creature read and resolved non-lethally |
| Traversal/reward | `FH_USE_JOB_ZIPLINE`, `FH_FIRST_JOB_REWARD` | Traversal verb used and reward routed |
| Return/payoff | `FH_RETURN_TO_SHIP`, `FH_CHANGED_SHIP_PAYOFF` | Return succeeds; ship/RILL/progression visibly changed |

---

## 8. Controlled contract deltas discovered during reconciliation

These are review items after M0, not runtime authorization.

### Delta A — W000 coupler placement

Current recovery and audio authorities treat the coupler repair as the launch gate before PUNCH IT, but `first_hour_beats.json` has no dedicated W000 coupler semantic beat.

Default recommendation after the headset verdict:

- preserve the existing coupler as a W000 launch-preparation interaction;
- treat it as a **repair preview**, not a second full repair tutorial;
- W001 remains the deliberate teaching and mastery of the repair verb;
- add the smallest semantic observation needed to record coupler completion without creating another repair owner;
- do not duplicate RepairableMachine, travel or ship-launch state.

Approval needed after device evidence confirms the coupler interaction is usable.

### Delta B — first useful invention

The approved growing plan says the W001 band culminates in:

- a first trait-bearing seed/input found and scanned in W001;
- an industrial component earned through the main job/salvage loop;
- one deterministic invention made after returning to the ship;
- the invention equipped in one of the three belt sockets;
- its usefulness demonstrated on a revisit or the next route.

The current 22-beat contract ends at `FH_CHANGED_SHIP_PAYOFF` and does not explicitly represent craft/equip/use.

Default recommendation after M0:

1. keep `FH_CHANGED_SHIP_PAYOFF` as the visible return-home umbrella;
2. add additive semantic beats for first invention crafted and first invention equipped;
3. make first field use the opening beat of the next route or a short optional W001 revisit, rather than overloading the first-hour climax;
4. required recipe outputs remain deterministic and save-stable.

No first-hour contract JSON changes occur merely because this recommendation exists.

---

## 9. W000 production requirements

### 9.1 Required spaces and landmarks

- clean Home Hub/title presentation;
- quarters/wake area with safe floor and readable RILL position;
- child-reachable comfort console;
- one named personal object near the bunk;
- three belt/holster sockets visible and usable;
- one planter/grow tray;
- one future-invention silhouette card;
- clear route toward helm/launch preparation;
- coupler/launch gate if retained after M0;
- PUNCH IT control;
- one visible future-upgrade location that creates curiosity.

### 9.2 Growing promise

Mandatory garden interaction remains under roughly 90 seconds:

1. receive or find one clearly named starter seed;
2. hear/see its simple identity;
3. place it into one plot;
4. see one useful future-output silhouette;
5. leave before growth completes;
6. see the plant matured after returning from W001.

No breeding, adjacency, recipe grid, machine tier or large catalog is taught in W000.

### 9.3 W000 subjective questions

- Does the ship feel like a place rather than a menu room?
- Is RILL spatially present without becoming intrusive?
- Can a six-year-old reach the comfort, planter, coupler and PUNCH IT interactions?
- Is the return path through the ship obvious without arrows?
- Does the planted seed create curiosity rather than feel like a chore?

---

## 10. W001 model-world requirements

### 10.1 Arrival composition

The approved W001 identity must provide:

- one immediate hero image;
- one dominant navigation landmark;
- one clearly readable return berth/path;
- foreground, playable midground, horizon depth and sky depth;
- a quiet orientation window;
- ambient activity that is not waiting for the player;
- the job source readable through world logic rather than a modal arrow.

Exact art, sky and landmark identity remain blocked by the W001 identity decision.

### 10.2 Primary technician job

Required loop:

1. accept one job;
2. scan and identify the fault;
3. traverse to the work site;
4. open/remove the access panel;
5. fetch and seat the replacement part;
6. press the child-reachable power control;
7. receive visible/audio/haptic intermediate confirmation;
8. collect or route one result;
9. receive payment through the existing economy;
10. leave a persistent repaired/lit/opened consequence.

Existing owners:

- `JobDirector` / `JobRuntime`;
- `WristScanner` / `IScannable`;
- `RepairableMachine.CurrentStage` / `StageChanged` / `IsRepaired`;
- `RewardRouter` / `ProfileEconomy` / existing ledger;
- existing save-overlay rules.

### 10.3 Seed surfacing

W001 must surface only one early biological lesson:

- one trait-bearing input is found on or near the critical job route;
- scan/focus reveals its name and one-line function;
- it advances one desirable invention silhouette/card;
- collection does not interrupt the job for a separate farming lesson;
- no required progress depends on breeding or RNG.

Two or three total seed families may be introduced across W000/W001, but only one needs to drive the first invention.

### 10.4 Signature creature

The first creature encounter must include:

- foreshadowing before direct engagement;
- a safe observation window;
- one readable tell;
- one understandable counter;
- a non-lethal redirect/disable result;
- a consequence to route, machine or environment;
- no gore register;
- no immediate unavoidable attack on reveal.

The final species is not selected by this packet. Creature selection must fit the approved W001 ecology and the tool/invention teaching sequence.

### 10.5 Story/progression object

Include one physical story fragment, personal trace or choice interaction that:

- is optional or naturally adjacent to the route;
- rewards curiosity without pausing action for a lore dump;
- uses existing story/flag owners;
- survives save/return where applicable;
- gives RILL a distinct reaction.

---

## 11. First invention selection gate

Do not lock the first invention until the canonical W001 world and hazard are approved.

The first invention should add a verb rather than a small statistic.

| World need | Strong candidate |
|---|---|
| vertical access / unstable surfaces | Grip tool or tether |
| toxic air / contaminated passage | Filter or membrane tool |
| power grid / disabled machine routes | Conductive tether |
| darkness / signaling / readable exploration | Lantern or glow tool |
| moving platforms / low gravity | Float/stabilizer tool |

Selection rules:

- understandable in one spoken sentence;
- produced from one base, one trait and at most one catalyst;
- deterministic campaign output;
- one item in one existing belt socket;
- useful in at least two later contexts;
- does not obsolete the primary weapon or machine/salvage progression;
- has a visible silhouette before discovery;
- supports captions and spoken name/function metadata.

---

## 12. RILL delivery requirements

Use existing RILL presentation and subtitle ownership. Do not add another dialogue queue.

Required tutorial line IDs already named by the beat contract include:

- `TUT_LOOK_RILL`;
- `TUT_COMFORT_CONSOLE`;
- `TUT_MOVE_QUARTERS`;
- `TUT_GRAB_BUNK_OBJECT`;
- `TUT_HOLSTER_ITEM`;
- `TUT_INTERACT_HELM`;
- `TUT_PUNCH_IT`;
- `TUT_ACCEPT_FIRST_JOB`;
- `TUT_SCAN_FAULT`;
- `TUT_REPAIR_ACCESS`;
- `TUT_SHOOT_PRACTICE`;
- `TUT_OBSERVE_CREATURE`;
- `TUT_COUNTER_CREATURE`;
- `TUT_ZIPLINE`;
- `TUT_RETURN_HOME`.

Delivery laws:

- no more than one active teaching line at a time;
- hesitation help only when needed;
- no camera or input lock;
- captions remain available;
- critical instructions are not audio-only;
- RILL reacts to actual state rather than narrating a predetermined script over failed actions;
- return-home reaction is emotionally distinct from tutorial instruction.

---

## 13. Child-readability and accessibility contract

Every critical seed, plant, material, machine, weapon, ability and recipe output must have:

- stable ID;
- distinct silhouette;
- icon;
- non-color-only category mark;
- visible name;
- visible one-line function;
- spoken name;
- spoken one-line function;
- caption fallback;
- stable narration IDs.

Interaction rules:

- child-reachable critical controls;
- seated and standing support;
- generous target/collider without misleading visuals;
- deliberate commit action for spending/crafting;
- cancel returns inputs;
- interruption never half-spends resources;
- no scrolling garden/catalog list in v1;
- focus dwell before narration to prevent chatter;
- names speak first; function speaks only on request.

Six-year-old acceptance:

1. point and hear the name;
2. explain the simple function after use;
3. complete a known recipe unaided on the second attempt;
4. equip the result;
5. use it successfully.

Failure means the interface needs correction, not that the child failed.

---

## 14. Audio sockets

`docs/audio/AUDIO_VERTICAL_SLICE_QUEUE.md` is the stable-ID authority.

P1 model-band families include:

- boot ready and panel confirm;
- RILL signature and ducking;
- W000 hull ambience;
- comfort select;
- grab and holster;
- machine broken, part seat and power cycle;
- PUNCH IT armed/ignite and travel wash;
- W001 ambience;
- job accept, scan ping/lock, zipline and reward;
- weapon fire/hit;
- creature presence/telegraph/disable;
- salvage grant.

The W001-specific ambience ID remains provisional until the W001 identity gate closes. Do not spread `aud.ambience.w001_toxic` into generic runtime if ToxicCity remains a stand-in.

Audio assets remain blocked until the rails exist:

- mixer/buses/ducking;
- `AudioEvents` stable seam;
- volume persistence;
- caption twins;
- licensing evidence.

---

## 15. Art and asset sockets

Graybox truth comes first. Final assets must obey locked interaction dimensions rather than move gameplay to fit art.

Required socket families:

### W000

- RILL presentation socket;
- comfort console shell;
- named bunk object;
- planter/grow tray;
- seed container/card;
- coupler shell if retained;
- helm/PUNCH IT control;
- future-upgrade silhouette;
- changed-ship return payoff.

### W001

- arrival landmark;
- return berth;
- job source/signage;
- repair-machine shell and part;
- practice target;
- signature creature body/disable state;
- seed/plant source;
- story fragment/choice prop;
- traversal anchor/zipline;
- persistent repaired consequence.

Every external asset follows concept/reference → source archive → license evidence → cleanup → scale/pivot → materials → LODs → colliders → registry/provenance → Quest budget → device review.

---

## 16. Save, flags and interruption

Existing profile/world/ship owners remain authoritative.

Required persistence outcomes:

- profile selection/new game;
- comfort setting;
- tutorial beat completion;
- first holster/travel/job/repair/creature flags;
- rewards and economy;
- repaired W001 state;
- planted/matured W000 plant;
- recipe discovery and first invention when implemented;
- changed ship/RILL return state;
- Continue restores the visible payoff.

Interruption laws:

- doff, overlay, tracking loss, controller sleep, quit or travel cannot half-complete a recipe;
- travel cannot strand input ownership;
- save records stable IDs and integers rather than scene-object references;
- unknown/deprecated IDs are diagnosed rather than silently deleting progress;
- later lifecycle S1 must receive its own Golden/device proof because it is not inside the frozen `c45b1a2` APK.

---

## 17. Performance and comfort

Hard targets:

- sustained 72 Hz on the supported Quest floor chosen for the model band;
- stable memory across repeated W000↔W001 travel;
- no forced camera motion;
- no forced FOV changes outside approved comfort behavior;
- no full-field strobe;
- bounded particle, audio-voice, light, renderer and material counts through existing performance/audit owners;
- no independent per-plant Update loop;
- pooled/bounded repeated effects where the existing architecture provides it.

Exact numeric scene budgets come from the approved world profile and existing `PERF_BUDGET`/device-gate authorities. This packet does not invent replacement budget numbers.

---

## 18. Automated acceptance

Before device declaration, automation must prove:

- first-hour beat graph remains ordered and complete;
- binding evidence points to current owners;
- no duplicate travel, save, inventory, recipe, garden, machine, RILL or creature owner;
- required flags and save rows migrate safely;
- W001 WorldSpec exists and compiles;
- generated scenes are current and idempotent;
- every required POI/interaction/spawn exists;
- objectives and return path are reachable;
- recipe input/output closure and determinism;
- every critical definition has visible and spoken metadata;
- first invention fits one belt socket;
- art provenance/license evidence exists for imported assets;
- build profile includes the required scenes/assets;
- performance audits remain within the approved profile.

A machine-detectable failure becomes a machine check. Feel, clarity, comfort and emotional response remain device/human verdicts.

---

## 19. Device route after implementation

The final model-band device session must include:

1. cold boot and New Game;
2. RILL look/dwell;
3. comfort selection;
4. movement, grab and holster;
5. planter/seed promise;
6. coupler/launch preparation if retained;
7. PUNCH IT and W001 arrival;
8. quiet orientation and return-path identification;
9. accept job and scan fault;
10. panel, part and power stages;
11. practice target;
12. creature observation and non-lethal counter;
13. zipline/traversal;
14. payout and persistent world change;
15. return to W000;
16. observe grown plant and changed ship/RILL state;
17. craft/equip first invention when the contract delta is implemented;
18. quit, Continue and verify persistence;
19. doff/overlay/controller-sleep checks at safe checkpoints;
20. repeat the complete critical route without developer intervention.

Subjective questions:

- Did the first travel feel memorable?
- Was the route readable without floating arrows?
- Did the repair feel physical rather than checklist-like?
- Was the creature understandable and memorable?
- Did reward land before looking at a number?
- Did the seed/invention make the player want to continue?
- Could a child understand names, functions and interactions?
- Did any return, loadout or crafting step feel like unnecessary backtracking?

---

## 20. Post-M0 implementation order

Only after the exact headset verdict, blocker closure and explicit freeze lift:

### Packet close

1. Approve canonical W001 identity.
2. Decide coupler semantic placement.
3. Select first invention from world need.
4. Select signature creature and story object.
5. Approve arrival landmark and return path.
6. Convert this draft to execution authority.

### Architecture/data slice

1. Re-audit owners against current source.
2. Make only approved additive contract changes.
3. Add neutral migrations/tests.
4. Commit the first authoritative W001 WorldSpec.
5. No scene/UI implementation yet.

### Graybox slice

1. Generate/patch W000 and W001 through existing authors.
2. Prove every route, reach height and semantic trigger.
3. Prove save/return.
4. Record every repeated fix through the automation ratchet.

### Presentation slice

1. Lock representative art sockets and one asset import lane.
2. Build audio rails and P1 event delivery.
3. Replace graybox in controlled batches.
4. Run representative Quest evidence after each major layer.

### W002 replication

W002 must add its growing/invention/world content through data and existing authors with zero new generic C#. If it cannot, repair the factory before W003.

---

## 21. Automation-ratchet form

Every discovered correction is classified before implementation:

| Class | Meaning | Destination |
|---|---|---|
| 1 | W001-unique content | W001 spec/recipe/content data |
| 2 | reusable declaration | smallest schema field |
| 3 | repeated construction/wiring | compiler/author/factory |
| 4 | machine-detectable failure | audit/test/gate |
| 5 | feel/clarity/comfort | exact device row and evidence |

Repetition law:

- first occurrence: classify and fix deliberately;
- second occurrence in W002: presume missing reusable framework;
- third one-off repair: prohibited without a written exception.

---

## 22. Decisions remaining after tomorrow’s M0 verdict

These are the minimum content decisions required to turn this draft into an executable packet:

1. **Canonical W001 identity:** keep/rebuild ToxicCity, replace it, or define the intended W001 world.
2. **Coupler role:** launch-gate preview versus explicit first-hour semantic repair beat.
3. **First invention:** grip, filter, conduct, glow or float based on the approved world.
4. **Signature creature:** species, tell, counter and environmental consequence.
5. **Story object/choice:** what it reveals and which existing flag owner records it.
6. **Arrival landmark:** one image that defines W001 and anchors navigation.
7. **Supported Quest floor for the model band:** determines final performance profile.

No other broad invention should be required before graybox production begins.

---

## 23. Exit gate

The model band may advance to W002 only when:

- the full route completes without developer intervention;
- every required beat fires exactly as designed;
- save, quit, Continue and return work;
- W001 has an authoritative committed WorldSpec;
- packet, specs, generated output and evidence agree;
- the first useful invention is deterministic and understandable;
- the creature has a readable non-lethal resolution;
- no baseline blocker is disguised as future polish;
- Terry can request content changes without requiring foundational runtime surgery;
- a third operator can follow the packet without reconstructing project history.
