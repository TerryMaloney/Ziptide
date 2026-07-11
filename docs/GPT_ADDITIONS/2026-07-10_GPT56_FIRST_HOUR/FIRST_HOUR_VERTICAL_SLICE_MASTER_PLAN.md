# ZIPTIDE — FIRST-HOUR VERTICAL SLICE MASTER PLAN

**Status:** PROPOSED PLAN OF EXECUTION — documentation approved by Terry; runtime packets require named implementation approval  
**Integration owner:** GPT-5.6 Thinking  
**Working branch:** `terry-local-wip`  
**Written:** 2026-07-10  
**Primary target:** cold boot → W000 ship → Ziptide launch → W001 complete gameplay loop → visibly changed ship return

> This document does not replace `GAME_PLAN.md`, `EXCELLENCE_MAP.md`, `PRIORITIES.md`, any lane sprint board, or FORGE III. It is the cross-lane integration plan for proving that ZIPTIDE's many strong systems combine into one commercial-quality hour. A named packet enters a lane board only after Terry assigns it.

---

# 1. The decision

ZIPTIDE has enough system breadth to support a large game. The current risk is not a lack of ideas. It is that architecture, content systems, story, art systems, multiplayer systems, and world-generation systems have advanced faster than the complete player experience.

The next major project proof is therefore not another broad feature family or another large batch of worlds. It is one hour in which every major discipline works together:

1. The game presents itself professionally from cold boot.
2. The player understands their body and tools without reading a manual.
3. RILL becomes a presence rather than a text-delivery mechanism.
4. The ship begins to feel like home.
5. W001 looks, sounds, moves, and reacts like an inhabited location.
6. One technician job is physically satisfying and clearly communicated.
7. One creature encounter is memorable, readable, planet-specific, and non-lethal.
8. The player receives a reward they can see and feel.
9. The return home proves that actions and progression persist.
10. The whole route survives headset interruptions and sustains the performance target.

This first hour becomes the quality recipe that later worlds must follow. The world factory should multiply a proven excellent recipe, not multiply an incomplete one.

---

# 2. Existing foundations that must be preserved

This plan assumes and protects the project's current laws:

- one working branch: `terry-local-wip`;
- four established implementation lanes with owned paths;
- spec/data first;
- pure deterministic core before Unity translators;
- generated scenes through patchers, never hand-edited scene or prefab YAML;
- one quality gate per failure class;
- CI green per implementation bite;
- three-red circuit breaker;
- save-overlay and migration-safe persistence;
- `ZIPTIDE:` diagnostics for runtime behavior;
- board, handoff, runbook, and Excellence Map updates at chunk close;
- no device-feel completion claim until Terry verifies it.

This plan also preserves the active execution order of `docs/project_art_plan/FORGE_III_PLAN.md`. Picasso continues FORGE III rather than being redirected into a competing art roadmap.

---

# 3. Relationship to the previous GPT architecture packet

The previous packet remains technically useful:

`docs/GPT_ADDITIONS/2026-07-10_Post_Fable_Handoff/POST_FABLE_ARCHITECTURE_AND_PICASSO_PACKET.md`

Its proposals are integrated here as follows:

| Previous proposal | Integrated destination |
|---|---|
| Machine-readable project manifest, protected contracts, evidence manifest, guarded closure | **FH-02 Continuity & Evidence** |
| `WorldMomentDefinition` / presentation conductor | **FH-04 World Moments** |
| Surface response, VFX, audio, decal, and haptic routing | **FH-03 Unified Feedback Language** |
| `WorldPresenceProfile` and horizon/mid-distance layer | **FH-07 Living W001** |
| Authored encounter pacing definitions | **FH-06 W001 Loop** and **FH-08 Signature Creature Encounter** |
| Canonical world-view and creature-motion review artifacts | **FH-07C** and **FH-08B** |
| Saved world-state visual overlays | **FH-10 Return-Home & Visible Consequence** |
| Canonical creature species passport | **FH-08A Creature Species Contract** |
| Forge and imported-hero visual providers | **FH-08C Visual Provider Seam** |
| `CreatureMotionIntent`, contact, state vocabulary, secondary motion, tells, voice, habitat affordances | **FH-08D–FH-08H** |
| Species families and planet morphs | Later content-scale program after the pilot Signature species |

Nothing from the previous packet should be implemented merely because it appears there. The packet IDs below are the controlled route into implementation.

---

# 4. What “AAA” means for this project

For ZIPTIDE, AAA does not mean matching the asset volume of a 500-person studio. It means a premium Quest-native experience with:

- immediate usability;
- strong art direction and visual cohesion;
- reliable 72 Hz on the supported floor;
- confident physical interaction;
- distinct audio identity;
- authored pacing;
- memorable creatures and places;
- polished feedback on every important player action;
- emotional character continuity;
- persistence and visible consequence;
- no obvious programmer surfaces in the finished path;
- enough systemic depth that a small content team can create a large-feeling game.

The first-hour target is the minimum proof of that claim.

---

# 5. The player journey

Exact timings are tuning targets, not cutscene locks. The player remains free and the camera is never forcibly moved.

## 0–2 minutes — Cold boot and trust

**Experience:**
- Clean title surface: New Game / Continue / Settings.
- Save choice is clear and fast.
- No developer menu, sandbox language, build diagnostics, or debug-first presentation.
- Loading communicates progress and does not appear frozen.

**Quality test:** the player knows this is a finished game before entering VR interaction.

## 2–10 minutes — Wake in W000 and meet RILL

**Experience:**
- The player wakes inside the ship.
- RILL is visually and spatially present.
- The first interaction is the comfort console.
- LOOK, MOVE, GRAB, HOLSTER, and INTERACT are taught by situations, with hesitation-triggered RILL help only when needed.
- The ship offers a small amount of optional inspection that reveals character and world rather than exposition dumps.

**Quality test:** a new player learns the body without floating tutorial prompts or input lockouts.

## 10–17 minutes — The ship becomes yours

**Experience:**
- The player sees the helm, storage, one personal object, one visibly inactive future-upgrade location, and evidence of RILL's incomplete memory.
- The player performs one simple ship interaction that has physical feedback.
- The helm presents the first destination without exposing a complex galaxy interface.

**Quality test:** the ship reads as a home and machine, not a menu room.

## 17–20 minutes — First Ziptide launch

**Experience:**
- The launch uses world movement, sound, lighting, ship vibration/haptics, and RILL timing.
- No forced camera motion.
- The destination reveal is staged so the player naturally faces the important composition but may look anywhere.

**Quality test:** this is the first memorable spectacle and the game's title earns meaning.

## 20–28 minutes — W001 arrival and orientation

**Experience:**
- Immediate hero composition and readable return path.
- A short quiet beat lets the player absorb the world.
- Ambient motion and audio prove the location exists beyond the objective.
- The job source is visible through world logic and wayfinding, not an intrusive arrow.
- The player sees one distant event or moving silhouette that is not for them.

**Quality test:** W001 reads as a place, not a generated arena.

## 28–42 minutes — The technician fantasy

**Experience:**
- Accept a clear job.
- Scan the problem.
- Reach the work site using one traversal verb.
- Open or expose the machine.
- Seat/replace/route a physical part.
- Power-cycle or test it.
- Collect or route one resulting resource/object.
- Receive clear intermediate confirmation at every step.

**Quality test:** the job is a satisfying sequence of hand verbs, not a checklist disguised as interaction.

## 42–52 minutes — Signature creature encounter

**Experience:**
- The creature is foreshadowed through sound, traces, environmental behavior, or a distant silhouette.
- The player has a safe observation window.
- Its tell and counter are readable without a HUD explanation.
- The encounter uses the environment and one recently learned tool.
- The creature is disabled or redirected, never treated as disposable target practice.
- Its conclusion changes the route, machine state, or environment.

**Quality test:** the player can describe what was unique about the creature and how they solved it.

## 52–57 minutes — Reward and consequence

**Experience:**
- The job resolves clearly.
- Payment has sound, haptic, visual, and world-state feedback.
- RILL reacts to what actually happened.
- A world element remains repaired, lit, opened, calmed, or otherwise changed.

**Quality test:** success is felt before the player looks at a number.

## 57–65 minutes — Return home and desire to continue

**Experience:**
- Return travel is reliable and quick.
- The ship visibly acknowledges progress: repaired system, trophy/object, unlocked station, changed light/audio, RILL behavior, or tool upgrade.
- The next destination or mystery is teased without a long menu.
- Save reassurance is explicit but diegetic.

**Quality test:** the player understands why they should continue and trusts that progress was remembered.

---

# 6. Vertical-slice release gates

The first hour is not “done” because all its classes compile. It is complete only when all categories below pass.

## Technical

- Sustained target frame rate on the supported Quest floor.
- No blocker exceptions or audit failures.
- Cold boot, New Game, Continue, travel, save, return, pause/resume, controller sleep/wake, and headset removal recover safely.
- No duplicate singleton, input, travel, inventory, or save ownership.
- Memory remains stable across repeated W000↔W001 travel.

## Player comprehension

- A cold player completes the route without external explanation.
- Every core verb has exactly one teaching moment.
- No more than one active teaching prompt/line at a time.
- Objective state and next actionable location are understandable.
- Critical state never relies only on color or audio.

## Physical feel

- Grabs align and release naturally.
- Holster and retrieval are dependable.
- Weapons/tools sit correctly in either hand.
- Every important hand verb has a tuned haptic signature.
- Hit, repair, pickup, payment, and travel feedback are synchronized across visual/audio/haptic channels.

## Art and world presence

- No visible primitive fallback on the critical path unless deliberately approved as the final style.
- W000 and W001 each have coherent light, grade, atmosphere, practicals, grounding, signage, and performance budgets.
- W001 has foreground, playable midground, horizon depth, and sky depth.
- At least one ambient process continues independently of the player.

## Creature quality

- The Signature species has a complete passport: world reason, body source, motion profile, tell, counter, audio, habitat, disable state, and encounter role.
- Contact/movement reads without obvious sliding or lockstep repetition.
- Alert, telegraph, action, recovery, stun, and disable are visually distinct.
- Motion review artifact and device encounter both pass.

## Story and emotion

- RILL's opening, first concern/joke, first useful collaboration, and return-home reaction are distinct beats.
- No lore monologue blocks action.
- At least one optional observation rewards curiosity.
- The ending of the hour creates a clear unanswered question.

## Persistence and reward

- Job completion, rewards, repaired world state, tutorial completion, comfort settings, and ship change survive quit/reload.
- Unknown or older save records degrade safely.
- The visible payoff is present on Continue, not only immediately after the event.

---

# 7. Parallel-operator coordination model

The four Opus accounts retain the existing four tracks. GPT-5.6 is integration/review, not an unrestricted fifth code lane.

## Story/Ship Opus

Owns:
- W000/W001 player journey;
- TutorialDirector translator and RILL beat content;
- ship hub/title/save flow inside established Story/Ship ownership;
- W001 job sequence and saved consequences;
- return-home payoff.

Does not invent neutral cross-lane APIs. Requests them through architecture envelopes.

## Multiplayer Opus

Owns:
- weapon and combat feel within Multiplayer/PvP ownership;
- bot/encounter proof surfaces in its lanes;
- integration of neutral feedback APIs into MP-owned callers;
- multiplayer regressions caused by shared feedback changes.

Does not change campaign Story/Ship interaction code merely because the same weapon exists there.

## Picasso Opus

Owns:
- FORGE III continuation;
- W000/W001 visual cohesion;
- fixture, water, grounding, VFX, reactive, signage, conformance, and creature-art work;
- creature visual provider implementations and motion presentation after architecture contracts exist;
- audio art/content and later diegetic UI art within coordinated windows.

Does not alter gameplay AI, stats, saves, or encounter outcome logic.

## Architecture Opus

Owns:
- neutral schemas and pure cores;
- cross-lane registries/contracts;
- deterministic validation;
- audit gates and evidence generation;
- WorldSpec/compilers and project continuity tooling.

Does not own the final feel of a lane translator or absorb content responsibilities from other tracks.

## GPT-5.6 integration owner

Owns only when explicitly approved:
- cross-lane plan maintenance;
- contract review before implementation;
- collision and dependency review;
- audits of whether implementation matches the approved packet;
- difficult debugging or a specifically claimed implementation slice;
- updating the integration log and reporting evidence to Terry.

## Shared-file rule

`BuildAndroid.cs`, `WorldAuditRunner.cs`, test asmdefs, shared definitions, travel, save, rig, and input are coordination surfaces. A lane must:

1. announce the intended one-line/append touch before editing;
2. identify the owning packet;
3. preserve neutral fallback behavior;
4. keep the change to the smallest possible diff;
5. rebase immediately before push;
6. stop if another operator has changed the same region.

---

# 8. Execution waves

## Wave 0 — Tomorrow's device truth

**Purpose:** establish the real baseline before stacking feel-sensitive work.

- Terry runs the existing device checklist and records pass/fail plus feel notes.
- Any recurring rig, turning, grabbing, holster, scanner, spawn, travel, save, or frame problem jumps ahead of this plan.
- CI-green is not accepted as proof of headset feel.
- While device results are unavailable, only documentation, pure cores with complete fallbacks, and non-invasive evidence tooling are candidates for implementation.

**Exit:** a short baseline report separates blockers, feel tuning, and verified foundations.

## Wave 1 — Safe foundations

- FH-02 Continuity & Evidence
- FH-03A Feedback data/pure resolution
- FH-04A World Moment pure core
- FH-08A Species passport schema/audit
- Review-artifact designs

These must not change current gameplay when no new data is assigned.

## Wave 2 — Opening and body

- FH-01 final first-hour experience contract
- FH-05 Home Hub/W000
- Comfort console
- Tutorial teaching river
- Critical body/interaction fixes from Wave 0

## Wave 3 — W001 complete loop

- FH-06 technician job sequence
- Feedback integration into scan/repair/reward
- Story/world moments
- Save overlay for repaired state

## Wave 4 — Presence, creature, and audio

- Finish required FORGE III envelopes
- FH-07 Living W001
- FH-08 Signature species and encounter
- FH-09 Audio Identity

## Wave 5 — Return payoff and hardening

- FH-10 visible ship consequence
- FH-11 UI/accessibility/readability
- FH-12 interruption, soak, performance, release-path testing

## Wave 6 — Replication decision

Only after the first-hour gate passes:

- extract the proven world/encounter/presentation recipe;
- update `WORLD_RECIPE.md` and relevant generators;
- decide which systems graduate from first-hour-specific to all-world requirements;
- resume content scaling with the raised quality floor.

---

# 9. Named implementation packets

Each packet below must be copied into the owning sprint board as smaller rows before implementation. “Proposed first slice” means suggested, not authorized.

## FH-00 — DEVICE BASELINE AND BLOCKER TRIAGE

**Owner:** Terry for headset execution; assigned lane handles each failure  
**Dependencies:** current APK and runbook  
**Touch risk:** none until results arrive

### Goal

Produce a truthful baseline of the existing player body, W001 loop, visuals, travel, saves, combat, and performance.

### Inputs

- `docs/TERRY_RUNBOOK.md`
- `docs/DEVICE_TEST_CHECKLIST.md`
- current CI/APK evidence

### Output

A compact result table:

- `BLOCKER`: prevents continuing the first hour;
- `FEEL`: functional but not acceptable;
- `PASS`: verified on device;
- `NOT REACHED`: content unavailable or test interrupted.

Every failure includes scene, action, expected behavior, actual behavior, reproducibility, and relevant `ZIPTIDE:` tag.

### Acceptance

No ambiguous “seemed okay” on critical body functions. This result controls Wave 1 ordering.

---

## FH-01 — FIRST-HOUR EXPERIENCE CONTRACT

**Owner:** Story/Ship, reviewed by GPT-5.6  
**Dependencies:** FH-00 findings  
**Implementation:** documentation/data only initially

### Goal

Turn Section 5 into the exact canonical beat list for W000/W001.

### Build shape

A data-oriented `FirstHourBeat` catalog or extension of existing tutorial/storyboard data, not a second narrative framework. Each beat declares:

- stable beat ID;
- scene/world;
- prerequisite flags/state;
- player verb;
- presentation moment ID where needed;
- RILL line or optional silence;
- completion signal;
- persistent result;
- fallback if the player performs it early or out of order;
- whether it is required or optional;
- evidence class: CI / Unity bake / device.

### Gate

- required-beat graph connected from New Game to FIRST_HOUR_COMPLETE;
- no duplicate teaching of the same verb;
- every referenced line, world, item, moment, and flag exists;
- no required beat depends on optional exploration;
- Continue resumes at a valid beat.

### Proposed first slice

Author and validate the beat graph without changing runtime behavior.

---

## FH-02 — CONTINUITY AND VERIFICATION EVIDENCE

**Owner:** Architecture  
**Dependencies:** none  
**Touch risk:** low, non-Unity Python/reporting first

### Goal

Make the strong human coordination rules mechanically inspectable so four Opus accounts plus GPT can work safely.

### Build shape

1. `docs/continuity/project_manifest.json` indexes branch of truth, operator manual, global boards, lane boards, owned paths, shared paths, and protected contracts.
2. `tools/continuity_gate.py` starts report-only.
3. `Builds/Reports/build_evidence.json` records commit, tests, audit result, APK hash, content counts, and outstanding human checks.
4. Later guarded closure checks that evidence belongs to the current commit.

### Initial checks

- required truth documents exist;
- lane ownership patterns do not overlap unexpectedly;
- protected-contract path list is valid;
- active board claims have dates/owners;
- current dashboards do not contradict explicit closed/open tokens;
- build evidence references the same commit.

### Laws

- report-only before blocker promotion;
- no attempt to semantically interpret arbitrary prose;
- explicit IDs/tokens only;
- no Unity license required for the first stage;
- false positives fixed before ratcheting.

### Proposed first slice

Manifest schema + report-only document/path validation. No C# and no CI blocker.

---

## FH-03 — UNIFIED FEEDBACK LANGUAGE

**Owner:** Architecture core; each lane integrates owned callers; Picasso supplies visual/audio content  
**Dependencies:** existing VFX/audio/haptic seams; FH-00 feel results

### Goal

Every important action receives a coordinated response instead of each feature hardcoding isolated effects.

### FH-03A — Data and pure resolution

Proposed definitions:

- `SurfaceResponseDefinition`
- `FeedbackEventDefinition`
- `HapticProfileDefinition`
- registries keyed by stable IDs
- pure resolution from event + surface + intensity + accessibility multiplier to output instructions

Core verbs include:

- grab;
- release;
- holster;
- draw;
- fire;
- charge ready;
- impact;
- successful scan;
- failed scan;
- part seated;
- machine powered;
- collectible acquired;
- job step complete;
- job paid;
- travel engage;
- creature alert/stun/disable;
- UI confirm/error.

### FH-03B — Neutral runtime router

A thin router emits existing audio/VFX/haptic operations. It must not own gameplay outcome.

### FH-03C — Two proof integrations

Use two contrasting actions:

1. Taser fire/impact.
2. Repair-part seat and power-cycle.

Do not touch every caller until the proof is device-tuned.

### Gates

- every referenced feedback ID resolves;
- haptic strength obeys comfort setting;
- caller coverage is report-only, then ratcheted;
- no unbounded AudioSource/ParticleSystem creation;
- simultaneous budgets enforced;
- critical feedback has at least two channels where accessibility requires redundancy.

### Device acceptance

Terry rates timing, strength, clarity, fatigue, and whether repeated use becomes irritating.

---

## FH-04 — DECLARATIVE WORLD MOMENTS

**Owner:** Architecture core; Story/Ship translators/content; Picasso presentation assets  
**Dependencies:** existing flags, RILL lines, registered scene IDs, feedback/audio/VFX APIs

### Goal

Stage tutorials, reveals, ship launches, creature entrances, machine events, rewards, and story beats without one bespoke MonoBehaviour per moment.

### FH-04A — Pure core

`WorldMomentDefinition` uses a closed action vocabulary such as:

- wait for time/proximity/flag/interaction/scan/job state;
- play RILL/subtitle line;
- trigger feedback/audio/VFX ID;
- set registered object state;
- set practical light state;
- open/lock registered passage;
- begin/advance objective;
- signal creature presentation state;
- branch on flag;
- grant flag;
- complete and persist.

The pure core evaluates current state and produces commands. It does not search scenes.

### FH-04B — Translator

The Unity translator resolves stable registered IDs and executes commands. Missing targets log and degrade according to per-action policy.

### Laws

- never translate or rotate the player's camera/rig;
- every wait has a timeout/fallback policy;
- moments are resume-safe and skip-safe;
- completion is idempotent;
- object references are stable IDs, never scene-name searches as primary logic;
- early player action satisfies the beat rather than breaking it;
- every action enum has tests and an interpreter.

### Proof moments

1. Comfort-console hesitation line.
2. First Ziptide launch presentation.
3. W001 repaired-machine light/audio return.

### Proposed first slice

Pure command evaluation with three harmless action types and zero scene wiring.

---

## FH-05 — HOME HUB AND W000 OPENING

**Owner:** Story/Ship; Picasso for visual surfaces; Architecture for requested neutral cores  
**Dependencies:** FH-00, FH-01, comfort design, existing ship systems, FH-04 proof

### Goal

Replace the development-first boot flow with a finished opening and make the ship emotionally and mechanically legible.

### Components

- minimal cold-boot title;
- New Game / Continue / settings/save choice;
- ship interior spawn;
- comfort console as first interaction;
- W000 teaching river;
- RILL physical/visual presence;
- simple helm destination selection;
- launch sequence;
- release-build debug isolation.

### Quality requirements

- no modal tutorial;
- no more than one highlighted affordance at a time;
- readable seated and standing;
- controls recover after recenter and controller sleep;
- Continue loads a stable ship state;
- optional ship observations add character but cannot block departure;
- the ship contains at least one visible future-growth promise.

### Gates

- boot→ship→travel round trip;
- save-slot restore;
- tutorial beat coverage;
- comfort preset total-resolution and motion-source coverage;
- title/release debug configuration audit;
- UI reach/readability audit when FH-11 lands.

### Device acceptance

A cold player reaches W001 without Terry explaining controls.

---

## FH-06 — W001 TECHNICIAN LOOP AND ENCOUNTER PACING

**Owner:** Story/Ship; Architecture supplies any neutral encounter/job core; Picasso supplies presentation assets  
**Dependencies:** FH-01, FH-03 proof, FH-04, existing jobs/machines/collectibles

### Goal

Deliver one complete arrive → understand → scan → repair → collect → encounter → paid loop.

### Machine interaction sequence

The first repair must contain multiple hand-readable states:

1. identify fault;
2. expose access point;
3. obtain or retrieve correct part/tool;
4. seat/route/adjust it;
5. power-cycle/test;
6. observe success/failure response;
7. collect resulting object/resource or open route.

No step exists only as a timer or button labeled “repair.”

### Encounter pacing definition

Use authored phases:

- foreshadow;
- observation/safe read;
- engage;
- escalation or environmental change;
- counter window;
- recovery;
- non-lethal resolution;
- consequence.

The encounter definition specifies roles, budget, required environmental counter, audio/threat targets, failure recovery, and completion condition.

### Gates

- complete job graph connected;
- every hand step has feedback ID;
- no impossible state after early pickup, dropped item, travel interruption, or reload;
- reward routes through the one economy;
- completion and world consequence persist;
- encounter can be completed without unexplained damage trading.

### Device acceptance

Terry judges clarity, physical satisfaction, pacing, and whether any step feels like placeholder interaction.

---

## FH-07 — LIVING W001

**Owner:** Picasso for visual/audio presence assets; Story/Ship for world placement/data; Architecture for pure seeded scheduling/data contracts  
**Dependencies:** ongoing FORGE III, W001 stable route

### Goal

Make W001 feel inhabited and larger than its playable pocket.

### FH-07A — World presence profile

Budgeted, deterministic categories:

- ambient movers: fans, pumps, cables, gauges, signs, background machinery;
- atmosphere pulses: drizzle, steam, spores, dust gusts, distant pressure/static events;
- occupancy cues: lit windows, radio fragments, door cycles, work lights, silhouettes;
- distant life: ship cards, canal movement, far drones/flocks;
- micro-events: vent burst, lamp flicker, distant call, machinery cycle;
- seed, density, cooldown, tier, and performance caps.

### FH-07B — Horizon/mid-distance layer

Bridge the gap between playable geometry and SkyVista using:

- skyline silhouettes;
- distant towers/bridges;
- emissive window masks;
- moving ship or canal cards;
- limited parallax depth bands.

### FH-07C — Canonical world review views

Register arrival, route, hero POI, interior threshold, and horizon views. CI publishes a contact sheet with budget metrics and conformance counts.

Pixel differences do not fail CI. Structural metrics do.

### Requirements

- one ambient process independent of the player;
- one occupancy cue;
- one distant-depth cue;
- no identical micro-event schedule on every visit;
- deterministic seed for testing;
- pooled/limited effects;
- all additions stay within Quest budgets.

### Acceptance

A static screenshot reads as cohesive, and a thirty-second observation shows multiple subtle signs of ongoing life without visual noise.

---

## FH-08 — SIGNATURE CREATURE LIFE PROGRAM

**Owner:** Architecture contracts; Picasso visuals/motion/audio; Story/Ship behavior/encounter/ecology integration  
**Dependencies:** FH-03, FH-06 encounter shape, existing CreatureRuntime/behavior and Forge pipeline

### Goal

Build one planet-specific creature that proves the production path from gameplay concept to memorable living organism.

### FH-08A — Creature species passport

One canonical definition binds:

- identity and tier;
- native world/biome/physics;
- evolution reason and story tie;
- gameplay behavior profile;
- encounter role;
- counters and non-lethal resolution;
- body visual source;
- socket/weak-point/contact map;
- motion profile;
- tell profile;
- voice profile;
- habitat affordances;
- LOD/performance tier;
- ecology response and saved consequence where relevant.

The existing `CreatureDefinition` is evolved or wrapped; do not create competing stat ownership.

### FH-08B — Motion review artifact

For every Signature species, publish:

- front/side/three-quarter silhouette;
- human scale reference;
- material x-ray;
- idle;
- locomotion phases 0/25/50/75%;
- alert;
- telegraph;
- action apex;
- recovery;
- stun;
- disable.

### FH-08C — Visual provider seam

Two implementations behind one neutral result contract:

- Forge-generated provider;
- imported-hero provider.

Both supply renderers, skeleton/bone map, stable sockets, weak points, contact anchors, tell channels, bounds/LOD data, and owned cleanup.

Gameplay knows only creature/species IDs.

### FH-08D — Motion intent

Gameplay publishes neutral visual intent:

- local velocity/acceleration;
- angular velocity and desired direction;
- grounded/surface normal;
- movement mode;
- behavior state;
- action phase;
- alertness;
- stun amount.

Visuals consume intent but never choose targets or move gameplay objects.

### FH-08E — Contact and weight

Tiered procedural contact:

- planted foot/anchor phases;
- limited foot locking;
- slope/support-plane body adaptation;
- turn/acceleration poses;
- wall/ceiling transition handling where required;
- tentacle anchors distinct from expressive tendrils.

No full ragdoll dependency.

### FH-08F — State motion and secondary life

Minimum Signature vocabulary:

- rest;
- locomotion;
- redirect/turn;
- notice/alert;
- telegraph;
- action apex;
- recovery;
- stun;
- disable;
- one social/ecology action.

Secondary spring motion may drive tails, sensors, fins, sacs, ribbons, or plates with distance culling.

### FH-08G — Tells and voice

The state must be readable through multiple channels:

- silhouette/posture;
- sensor/head direction;
- emissive/material state;
- audio grammar;
- optional VFX;
- environmental reaction.

Voice categories include idle, distant call, alert, telegraph, action, stun, disable, and social/nest where appropriate, with cooldown and Audio LOD rules.

### FH-08H — Habitat affordances

World data provides stable points/tags such as ground, wall, ceiling, water, perch, feeding, hiding, conductive, resonant, machine, canal, nest, and hazard edge. Creatures select compatible affordances rather than searching arbitrary scene names.

### Pilot selection law

The first pilot should not be another standard armored biped. Choose a species whose planet adaptation changes its body plan and locomotion. It must demonstrate the architecture rather than merely use it.

### Gates

- complete passport references;
- required sockets/anchors;
- behavior-state count;
- telegraph/counter/disable coverage;
- voice profile;
- native affordance/evolution/story fields;
- budget utilization floor for Signature tier;
- LOD and active-count cap;
- motion review artifact generated;
- device encounter approved by Terry.

---

## FH-09 — AUDIO IDENTITY

**Owner:** Picasso/audio; Architecture for pure mixing/profile core; Story/Ship for RILL/character integration; lane callers publish neutral threat/events  
**Dependencies:** FH-03 feedback, existing ambience/audio profile, final first-hour beats

### Goal

Give the first hour a recognizable sonic identity rather than treating sound as final decoration.

### Layers

- ship room tone and machinery identity;
- RILL voice/subtitle timing and ducking;
- Ziptide launch build/release;
- W001 biome bed;
- hazard and presence stingers;
- technician interaction feedback;
- tool/weapon identities;
- Signature creature voice grammar;
- success/payment/return-home motifs;
- adaptive tension/combat layer.

### Architecture

- evolve existing audio profile/manager rather than create a second global owner;
- deterministic threat-to-mix core;
- phase-aligned stems where used;
- Audio LOD for distant repeated sources;
- voice concurrency and priority;
- central volume/mix categories;
- subtitles remain authoritative accessibility output.

### Gates

- every first-hour beat requiring sound has a referenced event/clip/fallback;
- never-silent world coverage remains green;
- VO ducks relevant mix without muting critical cues;
- audio voice cap enforced;
- repeated actions do not produce fatigue-inducing peaks;
- critical audio cues have visual redundancy.

### Device acceptance

Headset mix check in quiet and normal environments; Terry rates spatial clarity, emotional impact, fatigue, and whether RILL remains intelligible during action.

---

## FH-10 — RETURN-HOME PAYOFF AND SAVED CONSEQUENCE

**Owner:** Story/Ship; Architecture for overlay rules; Picasso for visible variants  
**Dependencies:** FH-05, FH-06, progression/save foundations

### Goal

End the first hour with visible proof that the player changed both world and home.

### World consequence

At least one W001 state persists:

- machine remains repaired;
- route/light remains restored;
- creature pressure/behavior changes;
- area occupancy/audio changes;
- access point remains opened.

### Ship consequence

At least one visible, useful change:

- powered console or room;
- installed recovered component;
- displayed object/trophy;
- RILL memory presentation changes;
- tool station activates;
- ship light/audio identity evolves.

### Overlay law

- stable state IDs;
- neutral default equals pre-feature behavior;
- unknown records skip;
- migration-safe;
- scene assets remain immutable;
- visual and gameplay variants resolve from the same saved truth without dual ownership.

### Gate

New Game → complete loop → return → quit → Continue shows the same changed ship and W001 state.

---

## FH-11 — UI, READABILITY, COMFORT, AND ACCESSIBILITY

**Owner:** Story/Ship UI surfaces; Picasso visual skin; Architecture/readability gates; all lanes comply  
**Dependencies:** locked comfort design, first-hour surfaces

### Goal

Remove programmer surfaces and make every required decision comfortable and readable.

### Required surfaces

- title/save/settings;
- comfort console;
- helm destination;
- objective/job state;
- machine state;
- rewards/credits;
- pause/recenter/return;
- subtitles;
- tool cooldown/state where necessary.

### Laws

- diegetic first, but clarity wins over decoration;
- seated and standing reach;
- no critical tiny text;
- no color-only status;
- large hit areas;
- no precision laser-menu requirement for common actions;
- subtitle size and speaker identity;
- haptic intensity control;
- reduced-flash path;
- left-handed layout decision honored;
- development surfaces stripped or locked in release.

### Gate

A UI audit checks text size/distance, interactable reach/hit area, missing labels, color-only state, and release-debug exposure. Ratchet after pilot surfaces pass.

---

## FH-12 — HARDENING, SOAK, AND RELEASE-PATH PROOF

**Owner:** Architecture for tests/audits; each lane fixes its failures; Terry runs device cases  
**Dependencies:** all prior packets

### Scenarios

- cold boot;
- New Game and Continue;
- pause during each major beat;
- headset removal during travel, machine interaction, creature encounter, and save;
- controller sleep/wake;
- recenter seated/standing;
- dropped required item;
- early pickup or early encounter trigger;
- travel away and return mid-job where allowed;
- quit immediately after reward;
- ten W000↔W001 travel cycles;
- prolonged idle and combat/presence stress;
- low-memory/performance observation;
- release build with debug surfaces disabled.

### Evidence

- CI test/audit report;
- APK hash and commit;
- world/creature review artifacts;
- `ZIPTIDE:` log capture;
- Terry runbook results;
- memory/frame census;
- known limitations listed honestly.

### Exit

The route is suitable for an unfamiliar external playtester.

---

# 10. First proposed implementation slices

These are intentionally small and mostly non-invasive. Terry must approve a named slice before code changes.

## Candidate A — `FH-02A`

Continuity manifest plus report-only path/document validator.

- No Unity files.
- No behavior change.
- Best first proof that GPT-5.6 can contribute safely inside the project process.

## Candidate B — `FH-03A`

Feedback data definitions and pure resolution tests only.

- No callers changed.
- Existing haptics/audio/VFX continue unchanged.
- Establishes the contract for later lane integrations.

## Candidate C — `FH-04A`

World Moment pure evaluator with a minimal command vocabulary.

- No scene translator.
- No current tutorials or story beats change.
- Tests define deterministic/resume-safe behavior.

## Candidate D — `FH-08A`

Creature species passport schema plus completeness report/audit in warning mode.

- Existing creature definitions remain authoritative for stats.
- No visual or behavior changes.
- Reveals exactly what data each current species lacks.

## Candidate E — `FH-08B`

Creature motion contact-sheet design/workflow, after checking existing Forge PhotoBooth seams.

- Evidence-only.
- No gameplay change.
- Gives Picasso and GPT a better way to review motion quality.

Recommended starting order after the device baseline:

`FH-02A → FH-03A → FH-04A → FH-08A`, with existing lane work continuing in parallel.

---

# 11. Approval language

To prevent accidental scope expansion, Terry can authorize work with:

- `AOK IMPLEMENT FH-02A`
- `AOK IMPLEMENT FH-03A`
- `AOK IMPLEMENT FH-04A`
- `AOK IMPLEMENT FH-08A`

An approval authorizes only the named packet's first slice and its required documentation/log updates. It does not authorize later sub-packets, broad refactors, or unrelated cleanup.

Before starting an approved slice, GPT-5.6 will:

1. re-read current priorities, newest handoff entries, and the owning lane board;
2. confirm no new claim occupies the files;
3. post/update its exact work claim;
4. list proposed files and shared touches;
5. implement in the smallest reversible bite;
6. inspect CI and stop on red according to project law;
7. update its work log and tell Terry exactly what changed.

---

# 12. What is deliberately deferred

Until the first hour passes:

- mass W013–W080 authoring;
- broad new creature roster expansion;
- raising all creature budgets globally;
- fully dynamic weather simulation;
- unrelated new crafting/economy categories;
- ranked/live-service systems;
- large multiplayer expansion unrelated to first-hour regressions;
- final store trailer/screenshots;
- wholesale UI framework replacement;
- a unique AnimatorController graph per creature;
- broad refactors merely for cleanliness;
- replacing existing factories or travel/save/rig contracts.

Current active work may continue when already claimed, but this plan will not create new detours in those categories.

---

# 13. Risk controls

## Risk: four models implement the same neutral API

**Control:** architecture owns the contract; PRIORITIES holds the active coordination notice; other lanes wait for an assigned integration envelope.

## Risk: planning documents become a second roadmap

**Control:** this document does not directly assign tasks. Approved packets are copied into existing lane boards and then governed there.

## Risk: first-hour work becomes an endless polish project

**Control:** explicit gates and one defined route. Optional content cannot block release of the slice.

## Risk: device problems invalidate assumptions

**Control:** FH-00 preempts the planned order; no feel-sensitive task closes without Terry.

## Risk: new architecture destabilizes existing behavior

**Control:** pure core first, neutral defaults, unassigned data preserves current behavior, warning mode before blockers, proof integration before broad rollout.

## Risk: art ambition exceeds Quest performance

**Control:** class/tier budgets, active-count caps, review artifacts with metrics, device profiling, Forge as scalable default, imported hero assets only behind the same contracts.

## Risk: repo logging becomes inconsistent

**Control:** GPT work remains in the active coordination file; global claim is visible in PRIORITIES; each approved implementation must also update the owning board, normal HANDOFF, runbook, and Excellence Map as required by existing laws.

---

# 14. Definition of success for this plan

This plan succeeds when it produces—not merely describes—the following outcome:

A new player starts ZIPTIDE without help, feels comfortable in the body, becomes curious about RILL and the ship, experiences a memorable first Ziptide transition, understands and completes a satisfying technician job in a living W001, solves a distinctive creature encounter through observation and tools, receives a meaningful reward, returns to a changed home, quits, continues, and finds everything remembered—at stable performance, without visible development scaffolding.

Once that hour works, the project has a credible path from its exceptional architecture to an exceptional game.
