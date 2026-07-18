# FIRST HOUR v2.1 — CONTRACT RECONCILIATION, GAPS AND BUILD PACKETS

**Status:** PLANNING ONLY. No runtime work is authorized before the certified Quest checkpoint verdict.

**Purpose:** reconcile the recovery-proven 22-beat first-hour v1 contract, its partially implemented adapters, the older First Complete Slice packet, and Director's Cut v2.1 into one versioned product target another operator can execute without guessing.

---

## 1. The conflict this document resolves

The repository currently contains two valid but different first-hour truths.

### v1 — recovery baseline

`docs/first_hour/first_hour_beats.json` defines 22 required beats:

`_Boot → W000 wake/body/helm → ToxicCity job/repair/weapon/creature/zipline/reward → W000 changed-ship return`

It is valuable because:

- the contract compiler and pure evaluator are implemented;
- many neutral owner signals are implemented;
- the certified Quest route uses the same scene spine;
- its validators and envelope package are proven.

### v2.1 — product target

`docs/design/FIRST_HOUR_DIRECTORS_CUT.md` adds the product-defining arc:

- repair the ship coupler early;
- make a normal first flight;
- fly a short Space Lane salvage sortie;
- find artifact half A;
- play with the Taser before accepting the Toxic City job;
- receive artifact half B from the Vex-backed contract;
- join the halves;
- follow the beacon to the player's own ship;
- seat the key in the earlier coupler;
- trigger the first true Ziptide;
- complete rebuild → defend → grow → collect on W002;
- return to a changed ship.

The older First Complete Slice packet says to adopt the 22 beats “as-is.” Director's Cut says roughly eleven new beats and a different order. An operator cannot execute both literally.

### Resolution

- v1 remains an immutable baseline/migration source and the recovery checkpoint route.
- v2.1 becomes a new versioned product contract after recovery exit.
- shared adapters and canonical owners are reused.
- order-dependent orchestration is rebuilt from v2.1 data rather than patched around v1 prerequisites.

---

## 2. Current implementation maturity

### Implemented and CI-green

- FH-X01 contract asset/compiler
- FH-X02 pure progression core
- FH-S01 observation adapter
- FH-S02 holster adapter
- FH-S03 travel completion signal
- FH-M01 neutral scanner result
- FH-S04 repair stages/scannable machine
- FH-S06 zipline completion
- FH-S07 boot/profile/comfort/W000 surface code

### Pending physical proof or authoring

- FH-S01/S03/M01/S04/S06 device evidence
- FH-S07 idempotent scene author/bake
- FH-S07 headset acceptance

### Unbuilt

- FH-S05 creature resolution signal; its implementation log explicitly released the claim while blocked on FH-A01
- FH-S08 final first-hour orchestration
- v2.1 contract data, bindings and content

### Status documents requiring Q0 correction

- `docs/first_hour/README.md` still describes FH-01A as contract-only.
- PR #60 assumes the first-hour adapter work is mostly future work.
- Director's Cut's phrase “FH-S01…S07 code-green” is too broad because FH-S05 is unbuilt and FH-S07 remains bake/device-yellow.

---

## 3. Versioning law

### Contract IDs

Recommended product IDs:

- `first-hour-v1-recovery-baseline`
- `first-hour-v2.1-key-that-knew-you`

Do not silently replace the bytes of v1 while preserving its ID.

### Source and runtime

- JSON remains editor/build-time source.
- `FirstHourContractAuthor` compiles the selected approved contract to one deterministic Resources asset.
- Runtime never parses JSON and never contains a second hard-coded beat list.
- `FirstHourProgressCore` remains the pure evaluator.
- One observing director translates owner events into semantic signals and persists accepted progress through existing profile flags/overlay conventions.

### Profile migration

1. **New profile after v2.1 ships:** starts v2.1 at beat 1.
2. **Profile with v1 `FIRST_HOUR_COMPLETE`:** receives the veteran-skip/completed migration; it does not replay teaching beats automatically.
3. **Partial internal/dev v1 profile:** development builds may offer explicit reset/replay. Production must use a deterministic documented mapping or restart the tutorial with informed confirmation; do not infer progress from world location alone.
4. **Existing device comfort settings:** survive profile reset because comfort is device-level.
5. **First-hour replay:** later optional feature; replay may not duplicate rewards, permanent items or story flags.

### Gate ratchet

- Contract/binding/envelope gates remain report-only during migration.
- Promote them to blocking only after v2.1 runtime signals and reports prove low-noise.
- FH-S08 closure is the earliest promotion point; full Quest proof is required before “commercial-quality first hour” status.

---

## 4. Revised experience order

The list below distinguishes contract beats from small animation/micro-interaction steps. Final IDs may be tuned during the data packet, but the semantic order is locked unless Terry changes product direction.

### Act A — boot and ownership

1. `FH_BOOT_READY`
2. `FH_NEW_GAME_SELECTED`
3. `FH_LOOK_AT_RILL`
4. `FH_COMFORT_AND_ACCESSIBILITY_CONFIRMED`
5. `FH_MOVE_IN_QUARTERS`
6. `FH_GRAB_BUNK_OBJECT`
7. `FH_HOLSTER_FIRST_ITEM`

### Act B — repair the future lock

8. `FH_COUPLER_FAULT_REVEALED`
9. `FH_COUPLER_ACCESS_OPENED`
10. `FH_COUPLER_CELL_SEATED`
11. `FH_COUPLER_POWERED`

The current 22-beat contract omits this setup even though the Director's Cut depends on it emotionally and `ShipCastOffRuntime`/W000 onboarding already use the coupler machinery.

### Act C — first normal flight and Space Lane

12. `FH_INTERACT_HELM`
13. `FH_FIRST_FLIGHT_LAUNCHED`
14. `FH_FLIGHT_CONTROLS_PROVEN`
15. `FH_SPACE_ROUTE_COMPLETED`
16. `FH_SHIP_STUN_PROVEN`
17. `FH_WRECK_SALVAGE_PROVEN`
18. `FH_ARTIFACT_HALF_A_ACQUIRED`
19. `FH_ARTIFACT_HALF_A_SECURED`

Flight is a player verb. It cannot be inserted as a six-minute activity without a teaching beat, comfort confirmation and completion semantics.

### Act D — Toxic City toy before chore

20. `FH_W001_ARRIVAL`
21. `FH_TASER_ACQUIRED`
22. `FH_SHOOT_PRACTICE_TARGET`
23. `FH_DISABLE_DUMMY_DRONE`
24. `FH_USE_DISPATCH_ZIPLINE`
25. `FH_ACCEPT_FIRST_JOB`

This order intentionally contradicts v1's current prerequisites. The v2.1 contract must encode the new order; runtime presentation may not fake it around the old graph.

### Act E — first real job

26. `FH_OBSERVE_JOB_THREAT`
27. `FH_COUNTER_JOB_THREAT`
28. `FH_SCAN_RELAY_FAULT`
29. `FH_REPAIR_ACCESS`
30. `FH_REPAIR_PART_SEATED`
31. `FH_MACHINE_POWER_CYCLE`
32. `FH_FIRST_JOB_REWARD`
33. `FH_ARTIFACT_HALF_B_ACQUIRED`
34. `FH_VEX_SIGNER_PATTERN_NOTICED`

The exact position of the signature creature versus the repair leg may be tuned for pacing, but the “toy before chore” and “reward gives half B” laws remain.

### Act F — key and first true Ziptide

35. `FH_ARTIFACT_RESONANCE`
36. `FH_ARTIFACT_JOINED`
37. `FH_BEACON_FOLLOWED`
38. `FH_KEY_SEATED`
39. `FH_FIRST_ZIPTIDE_TRANSIT`

Normal W000→W001 flight must not be called the first Ziptide in product-facing content. Existing travel still owns scene transitions; the semantic signal and spectacle differ.

### Act G — W002 whole-game cycle

40. `FH_W002_ARRIVAL`
41. `FH_REBUILD_PUMP`
42. `FH_BUILD_EXTRACTOR`
43. `FH_DEFEND_THE_PUMP`
44. `FH_COLLECT_COMBAT_SALVAGE`
45. `FH_PLANT_FIRST_SEED`
46. `FH_COLLECT_FIRST_YIELD`
47. `FH_RETURN_HOME`
48. `FH_CHANGED_SHIP_PAYOFF`

The final beat sets canonical tutorial/first-hour completion flags.

### Required verbs taught once

- LOOK
- COMFORT/ACCESSIBILITY
- MOVE
- GRAB
- HOLSTER
- SCAN
- REPAIR
- INTERACT
- LAUNCH
- FLY
- SHIP_STUN
- SHIP_SALVAGE
- SHOOT
- ZIPLINE
- ACCEPT_JOB
- OBSERVE_THREAT
- COUNTER_THREAT
- JOIN/ASSEMBLE
- BUILD
- DEFEND
- PLANT
- COLLECT
- RETURN_HOME

Repeated uses reinforce verbs but do not create duplicate teaching ownership.

---

## 5. Binding strategy

### Direct reuse

- boot/profile choice through current Home Hub owners;
- observation through FH-S01;
- holster through FH-S02;
- travel success through `TravelCoordinator`/FH-S03;
- scanner results through FH-M01;
- repair stages through FH-S04;
- zipline completion through FH-S06;
- rewards through `RewardRouter`;
- item creation through `ItemFactory`;
- save through `SaveSystem`;
- build/garden/mining through existing systems.

### Thin adapters required

- exact coupler machine identity and completion mapping;
- helm selection/normal first-flight launch completion;
- flight controls proven without becoming a second flight owner;
- ship stun and salvage semantic completion;
- designated practice target and dummy drone;
- designated real threat disable after FH-S05 is unblocked;
- artifact acquisition/security/join/key-seat events;
- extractor build, wave completion, seed plant and first yield;
- final changed-ship acknowledgement.

### Composite authored content

- Space Lane route and wreck;
- Toxic City toy area and job route;
- signature encounter;
- artifact joining/beacon sequence;
- ship-mounted gate spectacle;
- W002 rebuild/defend/grow/collect cycle;
- final return composition.

### One orchestration owner

The director:

- subscribes to neutral owner events;
- feeds semantic signals to `FirstHourProgressCore`;
- requests RILL lines by ID;
- maps accepted flags through the existing profile/save path;
- never duplicates job, repair, scanner, travel, combat, inventory, build, garden or RILL state;
- remains safe if missing, leaving base gameplay functional and required beats open.

---

## 6. Story-critical item contract

Artifact state is a first-hour blocker unless these rules are explicit.

### Canonical states

- none
- half A acquired
- half A secured
- half B acquired
- both halves available
- joined key
- key seated

### Persistence

Persist high-level flags/item identity, not transient transform or animation state:

- `ARTIFACT_HALF_A`
- `ARTIFACT_HALF_B`
- `ARTIFACT_JOINED`
- `KEY_SEATED`

The exact flag vocabulary must be added through the canonical constants/migration path, not raw strings scattered across runtime classes.

### Recovery laws

1. Artifact pieces are story-critical and cannot be permanently lost out of bounds.
2. If dropped into invalid geometry or abandoned during travel, recover to a known ship locker/quest-item receiver or deterministic recovery pedestal.
3. Holsters are not the only valid story-item persistence path; full holsters cannot soft-lock progression.
4. A save with the acquisition flag but no valid item instance reconstructs exactly one correct form.
5. A joined key deletes/retires both half instances atomically before creating the joined form.
6. Joining cannot grant duplicates through repeated collision/contact callbacks.
7. Once seated, the key is ship state, not a loose inventory item.
8. Replay/debug tools cannot duplicate permanent reward state.
9. Save/relaunch after each irreversible state resolves directly to a stable pose.
10. Artifact recovery never creates a second general inventory/save owner.

### Join accessibility

Primary interaction:

- hold both halves near each other;
- bounded magnetic pull begins near the authored threshold;
- strong haptic confirmation;
- deterministic seat and transformation.

Fallback:

- place halves on an assembly surface or hold one half while the second is socketed;
- usable seated and by a one-handed player;
- same semantic completion and reward.

---

## 7. Flight onboarding contract

The Space Lane is not “existing mechanics, therefore no tutorial work.” The player has not used flight yet.

### Minimum first sortie

- short launch and stabilization moment;
- two or three readable route gates/rings;
- one orientation/yaw teaching cue;
- one thrust/boost teaching cue appropriate to the chosen preset;
- one ship stun against a forgiving derelict drone;
- one close-range salvage action;
- clear return/docking transition.

### Comfort

- Standard/Cozy values come from the existing comfort table;
- Cozy disables barrel roll;
- no forced camera motion;
- no unexplained rig acceleration outside the flight owner;
- RILL warns before boost/rapid maneuver;
- first sortie avoids dense combat or repeated rolling;
- first flight receives its own Quest micro-campaign before full-hour integration.

### Completion signals

- controls proven by real input/state, not elapsed time;
- route completion from the flight owner;
- ship stun from the real ship weapon result;
- salvage from the canonical salvage owner;
- no tutorial-specific flight state machine.

---

## 8. W001 route contract

### Order law

`arrival vista → Taser find → free toy area → zipline to Dispatch → accept job → threat/repair route → visible reward + half B`

### Free-play law

The toy beat lasts long enough to discover:

- aim and fire;
- physical dart/impact response;
- cans or props reacting;
- dummy drone stun/recovery;
- grab/holster/reclaim;
- no failure or resource pressure.

### Job ownership

Before authoring:

1. identify whether `WorldJobLibrary` or legacy `ToxicCityContractBuilder` is current owner;
2. select one source of truth;
3. test that only one job definition/spec creates the first contract;
4. preserve `JobDirector`/`JobRuntime` as state owners;
5. route payout through `RewardRouter` once.

### Creature/drone

- first live threat has a safe read window;
- telegraph visible before aggression;
- non-lethal resolution;
- FH-S05 neutral completion signal implemented after the art/presentation dependency is proven;
- no new damage or AI framework.

---

## 9. W002 economy and fail-safe contract

### Deterministic affordability

The first extractor must be buildable regardless of optional player behavior.

Choose one explicit rule during the data packet:

- first extractor has a tutorial waiver/free blueprint; or
- W001 reward includes a reserved amount and no competing spend surface appears before W002; or
- the required amount is escrowed for the tutorial build.

Do not rely on “the player probably did not spend it.”

### Build

- one visible socket;
- one correct choice;
- physical confirmation;
- no broad factory tutorial;
- built machine remains after travel/relaunch.

### Defend

- Cozy: two drones, no fail state, pump cannot be permanently destroyed;
- Standard: approximately four drones, bounded retry/recovery;
- failure never deletes story items or paid build resources;
- post-wave salvage sweep is calm and readable.

### Grow

- one seed and one plot;
- replacement seed remains available until successful planting;
- watering/planting reachable seated;
- growth continues through existing idle/ecology ownership;
- return state is visually obvious.

### Collect

- first hopper/yield payout is guaranteed after the authored interval/event;
- no duplicate payout after save/reload;
- amount and destination are visible.

---

## 10. Save/relaunch matrix

Automated tests and device spot checks must sample more than travel boundaries.

| Checkpoint | Required restoration |
|---|---|
| after comfort selection | device settings remain applied |
| after coupler access/part/power | one authoritative stable machine stage |
| after first flight launch | valid flight/session state or safe authored restart |
| after half A | exactly one recoverable half A |
| after Taser acquisition | weapon ownership and valid reclaim path |
| mid-W001 repair | correct stage/job state, no duplicate part/reward |
| after half B | both halves available once |
| after join | joined key only, no half duplicates |
| after beacon follow | route remains discoverable |
| after key seating | mounted key and unlocked destination |
| after W002 extractor build | machine persists and cost is not charged twice |
| mid-defend | documented restart/resume rule; never ambiguous |
| after seed plant | plot state persists |
| after first yield | reward not duplicated |
| final return | key glow, map, plant/hopper state and completion flags |

The contract packet must specify whether a combat wave resumes, restarts or resolves safely after reload. It may not depend on whatever current object lifetimes happen to do.

---

## 11. Failure and recovery table

| Problem | Required response |
|---|---|
| Taser dropped out of bounds | reclaim/respawn through canonical item ownership; no duplicate permanent unlock |
| Artifact lost | deterministic story-item recovery |
| Repair part lost | authored restock/recovery, not progression reset |
| Holster full | quest item routes to safe ship receiver/locker |
| Player leaves W001 early | job and artifact state remain valid; objective resumes |
| Player tries to join one half | no-op plus readable feedback |
| Duplicate join contacts | one atomic transition |
| Extractor cannot be afforded | impossible by deterministic first-build economy rule |
| Player loses defend wave | Cozy cannot hard fail; Standard has explicit retry without resource loss |
| Seed lost | replacement until planted |
| Player ignores objective | hesitation line, objective highlight or route cue; no forced movement |
| Scene/presenter missing | base gameplay survives; required beat remains open and logs once |
| Subscriber throws | canonical gameplay action completes; later subscribers still run |

---

## 12. Accessibility and comprehension

Before artificial movement:

- Cozy/Standard/Bold;
- seated/standing where implemented;
- subtitle toggle/size;
- handedness/menu hand or a clearly documented temporary limitation;
- haptic intensity where implemented.

Commercial first-hour acceptance must include:

- left- and right-handed Taser/holster usability or an explicit supported-hand contract;
- scanner gesture usability for both handedness choices;
- seated reach to comfort, helm, coupler, artifact assembly, extractor and garden surfaces;
- no color-only affordance;
- captions readable over sky, water, gate flash and dark W002 backgrounds;
- Tension Assist is not required in the first hour unless horror content is added, which is currently forbidden.

---

## 13. Revised build packets

### FH21-0 — authority and data design

- versioned contract IDs;
- v1/v2.1 migration rules;
- exact beat list and prerequisites;
- binding inventory and envelope updates;
- no runtime/scene change.

### FH21-1 — surviving baseline proof

- run FH-S07 author/bake;
- device micro-check of current adapters and surfaces;
- update status docs from evidence.

### FH21-2 — coupler and W000 setup

- exact coupler machine bindings;
- three repair stages in the v2.1 contract;
- false-negative repair if still present;
- hero-ship/coupler presentation coordination.

### FH21-3 — contract compiler/evaluator migration

- compile v2.1;
- pure progression tests for all beats;
- migration tests;
- validator/report updates;
- no content orchestration yet.

### FH21-4 — RILL teaching and caption v2

- v2.1 line IDs/data;
- hesitation requests;
- readable caption cards;
- no second presenter.

### FH21-5 — first flight and Space Lane

- flight teaching signals;
- short route;
- ship stun and salvage;
- comfort/device proof.

### FH21-6 — artifact lifecycle

- definitions, flags, recovery, stow, atomic join model and tests;
- half A Space Lane content;
- no beacon/key spectacle yet.

### FH21-7 — hero Taser and toy area

- weapon-feel foundation;
- Taser acquisition/reclaim;
- cans/target/dummy drone;
- zipline-to-Dispatch route.

### FH21-8 — W001 contract and live encounter

- one job owner;
- scan/repair/threat/reward route;
- FH-A01 proof and FH-S05 implementation;
- half B and signer pattern.

### FH21-9 — join, beacon and key

- two-hand plus accessible fallback;
- beacon to berth six;
- key seating;
- save/recovery matrix.

### FH21-10 — first Ziptide

- ship-mounted spectacle;
- W002 unlock/transit;
- performance/comfort/device campaign.

### FH21-11 — W002 cycle

- pump repair;
- deterministic extractor build;
- Cozy/Standard defense;
- salvage, seed and first yield;
- persistence tests.

### FH21-12 — return/payoff/orchestration

- sole director completes full sequence;
- changed ship/key/map/RILL ending;
- sampled save/relaunch matrix;
- final completion flags.

### FH21-13 — full proof and quality ratchet

- full PlayMode route;
- Golden Android;
- clean proof for milestone;
- complete Quest run;
- Terry/family comprehension and taste verdict;
- promote stable contract gates.

Each packet must use the operator template in `POST_HEADSET_OPERATOR_EXECUTION_MAP.md`.

---

## 14. Quality additions that should land on the first hour

These are not separate game architectures:

- RILL caption v2;
- hero ship hull/interior/coupler presentation;
- Taser grip, aim, moving mechanism, haptics and impact;
- one excellent threat reaction;
- small semantic material-audio set for metal, grate, concrete, dirt/gravel and key object contacts;
- clear door/room audio where the first-hour spaces use doors;
- reward ceremony;
- gate spectacle and transit audio;
- W002 darkness/readability and plant/hopper state;
- no placeholder interim hull at the final payoff gate.

The RILL Proving Ground is an accelerator for the weapon/target work but remains outside the required first-hour beat contract.

---

## 15. Explicit exclusions from first-hour implementation

Do not add during v2.1 unless a new approved contract version says otherwise:

- full orbital derelict interior;
- horror microworld;
- full player zero gravity;
- broad world-physics variants;
- Grapple/jetpack/glide;
- full Ultimate system;
- broad factory/belt logistics;
- full enemy role roster;
- PvP or online co-op;
- full VO requirement;
- Forge V/VI/VII;
- a second ship or fleet;
- arbitrary side quests that dilute the artifact/cycle arc.

---

## 16. Final acceptance

The v2.1 first hour is accepted only when:

1. the exact contract asset, runtime signals and device route agree;
2. no required beat can silently auto-complete;
3. no beat moves or locks the player rig;
4. a new player reaches every objective without outside instruction;
5. the coupler repair visibly pays off at key seating;
6. normal flight and first Ziptide read as different events;
7. the Taser is fun before the job begins;
8. live threat rules are readable and non-lethal;
9. artifact state cannot be lost or duplicated;
10. the W002 economy cannot soft-lock the extractor;
11. Cozy defense cannot hard-fail the first hour;
12. the player completes repair → build → defend → grow → collect;
13. return state survives relaunch;
14. the ship is visibly changed;
15. performance and comfort are acceptable for the full route;
16. Terry and the kids can explain the cycle and name a memorable moment.

Until those conditions are met, the project may have a functioning route or a compelling plan, but it does not yet have a perfected first hour.