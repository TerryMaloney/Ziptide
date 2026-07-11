# ZIPTIDE — POST-FABLE ARCHITECTURE, AAA COMPLETENESS, AND PICASSO CREATURE-QUALITY PACKET

**Date:** 2026-07-10  
**Prepared from:** `terry-local-wip` at the end of the four-Fable-operator era  
**Purpose:** preserve the leverage of Fable 5 by turning the remaining important design decisions into explicit contracts and executable envelopes that Opus 4.8 / Sonnet 5 can follow without inventing architecture.

> This is an **ideas packet**, not a directive and not a competing roadmap. Integrate it through the existing machine: `OPERATOR_START_HERE.md` → `EXCELLENCE_MAP.md` → `PRIORITIES.md` → the owning lane's sprint board → `HANDOFF.md` / `TERRY_RUNBOOK.md`.

---

# 0. Executive conclusion

ZIPTIDE does **not** need another rewrite. The project already has unusually strong foundations:

- four explicit lanes and file ownership;
- one shared branch and cross-lane task envelopes;
- spec-is-truth, pure-core-first, deterministic generation;
- patcher-only scene authoring;
- CI compilation, EditMode tests, world audits, APK artifacts, Quest log contracts;
- a world factory, Forge asset factory, SkyVista system, performance budgets, and an Excellence Map;
- data-driven worlds, creatures, economy, story, travel, PvP, Tidefront, and ship systems.

The next leverage is to fill a small set of **cross-cutting missing layers** that premium games normally discover late:

1. **Continuity enforcement:** make the excellent blackboard rules machine-readable and mechanically checked.
2. **Presentation choreography:** one declarative system for tutorials, reveals, set pieces, environmental reactions, and story staging.
3. **Unified feedback language:** surfaces, impacts, footsteps, haptics, VFX, audio, and decals resolved through one registry instead of per-feature hardcoding.
4. **World presence:** ambient motion, weather/atmosphere pulses, distant silhouettes, occupancy cues, and lived-in microactivity.
5. **Creature life architecture:** gameplay behavior, body source, motion intent, contact, tells, voice, ecology, and planet adaptation bound as one species package.
6. **Hero-asset escape hatch:** Forge remains the scalable production path, while signature creatures and props can use imported authored art behind the same IDs and gameplay contracts.
7. **Visual review beyond isolated assets:** canonical world views and creature motion sheets become CI artifacts, creating a ratchet lesser models can see and improve.

The core principle is:

> **Do not ask the next model to be more artistic or more careful. Give it a closed data shape, an implementation envelope, a review artifact, and a build-failing gate.**

---

# 1. Immediate continuity and repository improvements

These are the remaining useful parts of the Continuity Compiler idea. They should wrap the current process, not replace it.

## 1.1 Machine-readable project manifest

Add a single small manifest, suggested path:

`docs/continuity/project_manifest.json`

It indexes—not duplicates—the current truth documents.

Suggested fields:

```json
{
  "branchOfTruth": "terry-local-wip",
  "operatorManual": "docs/OPERATOR_START_HERE.md",
  "globalPriorityBoard": "docs/PRIORITIES.md",
  "excellenceMap": "docs/EXCELLENCE_MAP.md",
  "runbook": "docs/TERRY_RUNBOOK.md",
  "lanes": [
    {
      "id": "art",
      "board": "docs/SPRINT_ART.md",
      "ownedPaths": ["Ziptide/Assets/Ziptide/Visuals/**"],
      "sharedPaths": ["Ziptide/Assets/Ziptide/Editor/Build/BuildAndroid.cs"]
    }
  ],
  "protectedContracts": [
    {
      "id": "travel",
      "version": 1,
      "paths": [
        "Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs",
        "Ziptide/Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs"
      ],
      "decisionDoc": "CLAUDE.md"
    }
  ]
}
```

The manifest is an index. The prose docs remain human truth.

## 1.2 `ContinuityGate` CI check

Suggested implementation: a small dependency-free Python script, because it can run before Unity and does not consume a license.

Suggested path:

`tools/continuity_gate.py`

First useful checks:

- current branch matches the branch of truth for release/device work;
- every lane board and required spine document exists;
- owned-path changes are consistent with a claim/board identifier;
- protected-contract files cannot change without a contract version bump or named decision entry;
- generated/current docs do not contain obvious contradictions such as one row saying a gate is missing and another row in the same file saying it is closed;
- no active board claim is stale beyond the project rule;
- no task is marked complete while its required `⚙ / 🔧 / 🎮` evidence is missing.

Do not attempt semantic understanding of every Markdown paragraph. Start with explicit tokens and IDs.

## 1.3 Verification evidence manifest

Every successful dispatched APK build should publish:

`Builds/Reports/build_evidence.json`

Suggested fields:

```json
{
  "commit": "...",
  "generatedAtUtc": "...",
  "unityVersion": "2022.3.62f3",
  "editModeTests": { "passed": 0, "failed": 0 },
  "audit": { "blockers": 0, "warnings": 0 },
  "apkSha256": "...",
  "worldCount": 0,
  "forgeRecipeCount": 0,
  "creatureSpeciesCount": 0,
  "requiredHumanChecks": ["TERRY_RUNBOOK:2n"]
}
```

This gives a future model a durable answer to “what exactly was verified at this commit?”

## 1.4 Guarded closure, not merely a checked box

A lightweight command can validate a board row before it becomes complete:

`python tools/continuity_gate.py close F3.3`

It should refuse when:

- CI evidence belongs to a different commit;
- the build audit has blockers;
- a required Terry runbook item remains open;
- the protected contract changed after verification;
- the lane board or HANDOFF was not updated.

It does not need to edit Markdown initially. A report-only gate is already valuable.

## 1.5 Documentation reconciliation now

Before the Fable era closes, perform one documentation-only reconciliation pass:

- point the root README at `docs/OPERATOR_START_HERE.md` and state that `terry-local-wip` is the working branch;
- resolve the giant branch-of-truth / stale-`main` situation after active sessions stop colliding;
- remove stale “RESUMING” text from boards whose later rows clearly supersede it;
- distinguish manual authoring needed to **commit generated assets** from generation already done automatically during builds;
- generate rather than hand-maintain any status values that can be derived from assets, tests, or audit output.

Do not rewrite the historical logs. Only fix current dashboards and entrypoints.

---

# 2. The missing AAA-completeness architecture

FORGE III already correctly covers the visual-cohesion layer:

- derived light script;
- grade;
- practical fixtures and fake light pools;
- water;
- grounding and blob shadows;
- VFX vocabulary;
- reactive props;
- signage and wayfinding;
- macro variation;
- art-conformance ratchet;
- remaining creature look-at/stun/LOD polish.

The following systems are the strongest remaining candidates for “obvious in hindsight” omissions like lamps and lanterns.

## 2.1 Declarative World Moment / Presentation Sequence system

### Why

ZIPTIDE has story data, RILL lines, flags, jobs, choices, lights, audio, VFX, travel, and reactive objects—but no single declarative conductor for a premium staged moment.

Without one, every reveal, tutorial, creature entrance, ship departure, transmission, world transformation, and ending becomes a bespoke MonoBehaviour. That is dangerous at 80 worlds and difficult for a smaller model.

### Proposed shape

`WorldMomentDefinition` in Content, composed from a closed list of actions:

- show RILL/subtitle line;
- wait for time, flag, proximity, grab, socket, job state, or scan;
- enable/disable or animate a registered prop ID;
- switch a practical light or grade emphasis;
- play SFX/music sting;
- spawn a VFX recipe;
- open/lock a gate;
- grant/check a flag;
- start/advance an objective;
- trigger a creature behavior state;
- branch on an existing flag;
- finish and persist completion.

`WorldMomentDirector` interprets the data. In VR it must **never move or rotate the player's head/camera**. Presentation comes from world motion, light, audio, sightlines, and player-chosen proximity.

### Required laws

- deterministic and resumable;
- skip-safe and save-safe;
- no hard scene-object references: resolve registered IDs;
- no unbounded waits: every wait has a fallback/timeout policy;
- completion persists through the existing overlay idiom;
- every action enum has an interpreter and a test;
- every shipped story world carries its required moment IDs.

### Payoff

One architecture serves:

- W000/W001 onboarding;
- story set pieces and endings;
- RILL reveals;
- ship launch and arrival sequences;
- boss/warden introductions;
- repair machine theatrics;
- environmental reactions;
- arena intros and reward celebrations.

This should be authored before large-scale W013–W080 content so the remaining worlds are built with presentation hooks rather than retrofitted.

## 2.2 Surface Response + Feedback Router

### Why

Premium feel comes from consistent response. Metal, stone, slime, glass, water, chitin, cloth, and alien origami should not all produce the same silent hit.

Today individual systems can easily hardcode their own impact VFX, sound, haptic, or decal. That guarantees unevenness.

### Proposed data

`SurfaceResponseDefinition` keyed by stable surface-family ID:

- footstep audio IDs;
- hand/contact audio ID;
- weapon impact VFX ID;
- impact audio ID;
- decal/contact mark ID;
- haptic profile ID;
- particle color/material hints;
- optional gameplay property such as conductive/resonant/slippery, kept separate from appearance when possible.

`FeedbackEventDefinition` keyed by player verb:

- grab;
- holster click;
- fire;
- charge ready;
- successful scan;
- repair part seated;
- job complete;
- reward paid;
- UI confirm/error;
- climb grip;
- zipline attach;
- creature stun/disable.

`FeedbackRouter` resolves audio + VFX + haptics through the registries. Accessibility settings scale haptic strength and flash intensity centrally.

### Gates

- every shipped surface family maps to a response;
- every core hand verb maps to a feedback event;
- every referenced VFX/audio/haptic ID exists;
- no gameplay caller creates a bespoke AudioSource/ParticleSystem for a registered event;
- simultaneous voice and VFX budgets remain capped.

This closes three current weak areas together: material response, haptic language, and SFX consistency.

## 2.3 World Presence Profile

### Why

A world can have good static assets and still feel dead. “Lived in” is not only props. It is recurring motion and evidence that processes continue without the player.

### Proposed data

`WorldPresenceProfile`, referenced by the world pack/spec:

- **ambient movers:** fan, rotor, pendulum lamp, cable sway, gauge needle, pump cycle, conveyor background loop, hanging sign;
- **atmosphere pulses:** drizzle, ash, spores, dust gust, static flicker, distant thunder, pressure pulse;
- **occupancy cues:** lit windows, distant silhouettes, radio chatter, moving shadows, doors cycling, work lights;
- **distant life:** flock cards, far drones, ship streaks, canal traffic silhouettes;
- **micro-events:** vent burst, lamp flicker, far alarm, creature call, machinery start/stop;
- **event density and seed**;
- **tier:** Interior / Standard / Signature.

Implementation should use pooled, low-cost recipes and deterministic schedules. The goal is not a simulation-heavy dynamic-weather system. It is a budgeted vocabulary of movement and atmosphere.

### Add a missing distance layer

SkyVista covers the celestial background and Forge/world builders cover the playable space. Add a cheap **horizon/mid-distance layer**:

`HorizonSetDefinition`

- skyline silhouettes;
- distant towers;
- moving ship cards;
- far bridge/canal shapes;
- emissive window masks;
- parallax speed/depth bands.

This is how Quest worlds imply a city or civilization beyond the playable pocket without rendering it.

### Gates

Depending on tier, a shipped world should have:

- at least one ambient-motion family;
- one atmosphere or occupancy cue;
- one distant-depth cue unless intentionally enclosed;
- no budget violation;
- no identical presence profile across sibling story worlds.

## 2.4 Encounter and pacing definitions

### Why

AI behaviors can be individually good while encounters remain flat. Premium combat has anticipation, escalation, relief, composition, and an exit condition.

### Proposed architecture

`EncounterDefinition` + pure `EncounterDirectorCore`:

- encounter roles, not exact prefabs: scout, pressure, anchor, disruptor, objective threat;
- phases: foreshadow → engage → escalate → recover → resolve;
- spawn/population budget;
- allowed species IDs and combinations;
- environmental affordances/counters required;
- music/threat targets;
- failure/retry policy;
- non-lethal completion condition;
- deterministic seed.

World builders place an encounter definition; scene code only translates it. This can serve campaign jobs, creature encounters, arenas, ship defense, and Tidefront mission variants.

Do not build a Left 4 Dead-style omniscient director. Keep it deterministic, authored, testable, and compatible with the current job/world systems.

## 2.5 Canonical visual-review artifacts

The Forge photo loop is one of the strongest systems in the repo. Extend the same principle beyond isolated turntables.

### World review artifact

For every Signature world, define 3–5 registered `ReviewView` transforms:

- arrival composition;
- primary route;
- hero POI;
- interior threshold;
- horizon/sky composition.

A CI workflow renders them after patching and publishes a contact sheet with:

- world ID, seed, camera ID;
- triangle/material/light counts;
- art-conformance warnings;
- prior approved image hash/reference.

Do not fail CI on pixel differences. Use the images for a human/model review ratchet and fail only on structural metrics.

### Creature review artifact

For every creature species, render:

- front/side/three-quarter silhouette;
- material x-ray;
- idle pose;
- locomotion phases 0/25/50/75%;
- alert/telegraph;
- attack/action apex;
- stun;
- disable;
- scale next to a human silhouette.

A single mid-gait pose is not enough to judge life. A motion contact sheet lets Opus/Sonnet see foot sliding, dead joints, repeated phase, unreadable tells, and silhouette collapse.

## 2.6 World-state transformation overlays

The player should sometimes leave evidence that a world changed:

- repaired machinery stays repaired;
- lights remain restored or broken;
- infestation pressure changes;
- a bridge remains converted;
- a faction or RILL choice changes dressing;
- post-contract occupancy or audio changes.

Do this through the existing save-overlay idiom, not mutated scene assets.

Suggested data:

`WorldStateOverlay`

- stable object/state IDs;
- closed state values;
- visual/presence/audio variant IDs;
- migration-safe unknown-record skip.

This turns the 80-world campaign from a sequence of static sets into a place the player affected.

---

# 3. Picasso lane — from current Forge creatures to believable signature organisms

## 3.1 Honest current ceiling

The current creature pipeline is strong for scalable procedural v1/v2 organisms:

- authored `ForgeCreatureBody` genomes;
- smooth organic ops, textures, emissive tells, budgets;
- one generated skinned renderer;
- body-specific limbs and build-pose articulation;
- role-based gait motion;
- breathing;
- behavior-specific visual tell bridge;
- CI turnarounds and photo critique.

But its current contracts impose a visible ceiling:

- maximum 12 bones;
- core parts all weighted to the root;
- vertices are 100% rigid-weighted to one bone;
- one generic eye field is built into the body schema;
- gait sees time + speed, but not behavior state, desired direction, turn rate, acceleration, surface normal, attack phase, or emotion;
- no foot/tentacle contact solver;
- no state-specific locomotion vocabulary;
- no creature voice package;
- gameplay definition, body, behavior, visual tells, habitat, audio, and counter are not represented by one canonical species asset;
- Forge is the active generated visual path, while the imported-hero path exists mainly as a documented future upgrade.

This is why the next leap is not “more segments” alone.

> A believable creature is a coordinated package of **anatomy + locomotion + contact + perception + behavior + sound + environment**.

## 3.2 Add one canonical `CreatureSpeciesDefinition`

Do not replace `CreatureDefinition`; evolve or wrap it so one asset becomes the species passport.

Suggested fields:

### Identity and fiction

- species ID;
- display/debug name;
- native world/biome/physics tags;
- species family ID;
- evolution reason;
- story/faction/Bloom tie;
- non-lethal disable fiction;
- existing-gear counters;
- rarity/tier: Ambient / Standard / Signature / Hero.

### Gameplay

- base stat definition;
- behavior profile/set ID;
- encounter role;
- ecology profile ID;
- habitat-affordance tags;
- reward/ecology-pressure behavior.

### Visual

- visual source kind: Forge / ImportedHero;
- Forge body ID or imported visual prefab ID;
- silhouette class and size;
- material/surface family;
- tell profile ID;
- socket map: head, gaze, mouth, weak points, feet/anchors, VFX origins, audio origin, grab/interaction points;
- LOD/performance tier.

### Motion/audio

- motion profile ID;
- contact profile ID;
- secondary-motion profile ID;
- voice profile ID;
- surface-footstep profile ID.

### Validation

A species cannot ship unless every required reference exists and its tier's quality requirements pass.

This one data shape prevents the body, behavior, counter, planet logic, and audio from drifting apart.

## 3.3 Forge-generated and imported-hero visuals behind one contract

Forge should remain the default for:

- ambient species;
- prototypes;
- planet morphs;
- rapid catalog expansion;
- deterministic CI creation.

Do **not** force it to be the only way to make a Hero creature.

Add a neutral creature visual contract and two adapters:

1. `ForgeCreatureVisualProvider`
2. `ImportedCreatureVisualProvider`

Both produce the same runtime outputs:

- renderers;
- skeleton/bone map;
- stable sockets;
- weak-point markers;
- contact anchors;
- tell channels;
- local bounds/LOD data;
- cleanup ownership.

Gameplay continues to know only the species/creature ID.

Imported-hero validation should enforce:

- meters, +Y up, +Z forward;
- allowed shaders/material count;
- triangle and texture budgets by tier;
- skeleton/bone cap;
- required socket names;
- simple gameplay collider proxy, never mesh-collider gameplay dependence;
- LODs or approved distance fallback;
- no AnimatorController that owns gameplay state;
- animations or procedural bone access behind the same motion interface.

This lets future work use Blender, Tripo, a contractor, scanned references, or another generator without touching combat, jobs, saves, or world data.

## 3.4 Tier the skeleton and skinning pipeline

Do not raise every creature's cost globally.

Suggested tiers, subject to device measurement:

- **Ambient:** 8–12 bones, current rigid weights, ≤5k tris.
- **Standard:** 12–16 bones, optional joint blends, ≤10k tris.
- **Signature:** 16–20 bones, richer spine/head/limb chains, ≤14k tris.
- **Hero:** up to approximately 24 bones only after Quest measurement; imported or Forge-plus; strict active-count cap.

### Schema additions

Add explicit body-chain roles beyond generic limbs:

- Root / Spine / Neck / Head;
- Jaw / Mandible;
- Leg / Arm;
- Tail / Tentacle;
- Wing / Fin / Membrane;
- Antenna / EyeStalk / Sensor;
- Decorative plate or non-animated attachment.

Allow:

- multiple sensor/emissive organs, not one mandatory eye;
- asymmetrical chains;
- per-joint limits;
- optional two-bone blend zones around organic joints;
- body parts weighted to spine/head bones instead of every core part following the root.

The current rigid overlap technique is excellent for armored/chitin creatures. Keep it as a style. Add blended joints only where soft anatomy requires them.

## 3.5 Add `CreatureMotionIntent` — behavior tells visuals what the body is trying to do

The current visual animator infers only speed from transform deltas. That preserves decoupling but loses essential information.

Add a neutral signal component or interface populated by gameplay behavior and read by visuals:

```text
localVelocity
localAcceleration
angularVelocity
desiredMoveDirection
grounded
surfaceNormal
movementMode: Ground / Wall / Ceiling / Fly / Swim / Burrow
behaviorState: Idle / Alert / Hunt / Flee / Feed / Social / Attack / Recover / Stunned / Disabled
actionPhase: 0..1
alertness: 0..1
stunAmount: 0..1
```

Laws:

- gameplay owns intent, visuals own pose;
- no visual code selects targets or changes gameplay movement;
- missing signals degrade to current speed-derived gait;
- the signal data is inspectable and logged in debug mode;
- pure motion math remains EditMode-testable.

This one seam enables authored motion without coupling Picasso to every behavior class.

## 3.6 Contact and locomotion solver

Nothing reveals procedural animation faster than sliding feet and limbs passing through floors.

Add a lightweight, tiered `CreatureContactRig`:

### Ground creatures

- foot/anchor markers supplied by the visual provider;
- raycast only during planted gait phases;
- foot locks for a short stance interval;
- ankle/last-segment correction within strict angle/length limits;
- body height and pitch follow an averaged support plane;
- slope and stair limits match gameplay movement;
- turn-in-place and acceleration states prevent moonwalking.

### Wall/ceiling creatures

- orient body to the movement surface normal;
- anchor points seek valid nearby contact planes;
- transitions use a short authored roll/curl state, not an instant ninety-degree snap.

### Tentacles

- distinguish locomotion anchors from expressive free tendrils;
- planted tips remain fixed during pull phases;
- unattached chains use secondary motion.

Use raycast and bone budgets. Do not introduce full ragdolls or heavyweight physics rigs.

## 3.7 Species-specific motion vocabulary

The current generic role sine waves are a good fallback. Signature creatures need authored programs layered over them.

Every Standard-or-higher species should declare a minimum state vocabulary:

1. idle/rest;
2. locomotion;
3. turn or redirect;
4. notice/alert;
5. attack/action telegraph;
6. action apex;
7. recovery;
8. stun;
9. non-lethal disable;
10. one ecology/social action: feed, groom, call, nest, inspect, hide, bask, migrate, or interact with weather.

A `CreatureMotionProfile` should provide curves/ranges for each state rather than one AnimatorController per species. Examples:

- gait frequency/amplitude curves by speed;
- stance width and body bob;
- planted phase windows;
- head/gaze limits;
- jaw/throat/organ pulse;
- attack wind-up and recovery curves;
- turn banking;
- breathing rate by state;
- stun sag and recovery;
- secondary-motion stiffness/damping.

Closed profiles plus pure evaluators are safer for smaller models than hand-authored state-machine webs.

## 3.8 Secondary motion and soft life

Add a cheap spring-bone layer for:

- tails;
- antennae;
- sensory ribbons;
- hanging sacs;
- fins/membranes;
- loose armor plates;
- throat or abdomen movement.

Inputs are body acceleration and angular velocity from `CreatureMotionIntent`. Parameters are stiffness, damping, gravity, max angle, and lag. Use a tiny fixed number of chains and disable at distance.

This should replace “all motion is a sine wave” with inertia and delayed response—the body appears to have mass.

## 3.9 Tell profile: perception organs, attack tells, and readable internal state

Expand the current tell bridge into `CreatureTellProfile`:

- sensor/eye/head target and yaw/pitch limits;
- idle emission/color;
- alert transition;
- attack wind-up pulse or body expansion;
- weak-point reveal;
- stun state;
- disabled state;
- faction/Signal reaction;
- optional mouth/jaw/throat action;
- VFX/audio event IDs per transition.

The player should be able to read a creature's state from silhouette, pose, color, sound, and motion—not an HUD health bar.

## 3.10 Creature voice grammar

A xenomorph-class creature is remembered as much by sound as shape.

Add `CreatureVoiceProfile`:

- idle breaths/clicks;
- distant call;
- alert;
- telegraph;
- attack/action;
- hurt/stun;
- disable;
- social/nest call;
- pitch/formant range by size;
- concurrency and cooldown rules;
- Audio LOD behavior.

A species can use layered procedural or generated one-shots, but the state grammar must be data-driven. Integrate with the same feedback/audio registry so 20 creatures do not create 20 unmanaged AudioSources.

## 3.11 Habitat affordances and planet-specific behavior

The ecology document already names nests, territory, packs, predator/prey, and population. Add a simple world-to-creature interaction seam:

`HabitatAffordance`

Suggested tags:

- Ground / Wall / Ceiling / Water / Air;
- Dark / Bright / Warm / Cold;
- Conductive / Resonant / Magnetic;
- Nest / Perch / Feeding / Hiding / Basking;
- Machine / Garden / Bloom / Ruin / Canal;
- Shelter / HazardEdge / TravelRoute.

World builders stamp affordance points from existing layout/POI data. Creatures choose among compatible points; they do not search arbitrary scene objects by name.

Each species passport declares:

- required native affordances;
- preferred idle/social affordances;
- response to one world condition;
- one interaction with another species or machine/garden where relevant.

This is how a creature becomes specific to the planet rather than a model placed on it.

## 3.12 Species families + planet morphs: scalable uniqueness

Do not author eighty unrelated hero creatures. Use a hierarchy:

- approximately 8–12 **species families** with distinct skeletons and motion grammars;
- multiple **planet morphs** per family;
- one or two Signature/Hero species per chapter or major biome;
- ambient variants derived from the same family.

`SpeciesFamilyDefinition` owns:

- topology;
- core locomotion grammar;
- perception/organ concept;
- broad ecological niche.

`PlanetMorphDefinition` owns:

- proportions and appendage changes;
- material/surface changes;
- adaptation organ;
- palette/emissive variation;
- native affordances;
- behavior twist tied to planet physics;
- size and stat ranges.

The derivation law can help here without replacing art direction:

- high gravity requires a low/wide support plan;
- low gravity permits long limbs, sails, or orbiting locomotion;
- dense atmosphere supports membranes/fins;
- darkness requires a declared alternate sense or light interaction;
- corrosive/wet worlds require protected joints/material logic;
- resonant worlds require sound/vibration adaptations.

A morph must declare how it reflects its world. The validator checks presence and compatibility; a human/model still authors the actual form.

## 3.13 Creature quality gates

For every Standard-or-higher species:

- body source and every required socket valid;
- budget utilization above the Signature floor unless explicitly waived;
- silhouette readable in three registered views;
- asymmetry or a documented symmetry reason;
- at least two material-response regions for Signature/Hero tiers;
- minimum motion-state vocabulary satisfied;
- telegraph, counter, stun, and disable states all implemented;
- contact anchors supplied for grounded/wall creatures;
- voice profile supplied;
- native affordance/evolution/story tie supplied;
- LOD/distance behavior supplied;
- pose/motion contact sheet generated;
- device cap on simultaneous high-tier creatures.

The gate should not claim subjective beauty. It should prevent known incompleteness.

---

# 4. Example signature creature passport

This example exists to demonstrate how all the contracts produce a memorable, planet-specific organism without copying a franchise.

## Tidal Carillon — W010 Tidal Array

### Planet reason

The world alternates between pressure surges and exposed conductive flats. The organism evolved to anchor itself to resonant metal structures and communicate through pressure chords carried by the array.

### Silhouette

- tall three-legged arch rather than humanoid or insect;
- two load-bearing hooked limbs and one long probing limb;
- translucent pressure bladder suspended inside a mineral rib cage;
- hanging sensory ribbons that trail only when pressure falls;
- asymmetrical broken fourth rib where older specimens survived an array surge.

### Materials

- wet mineral shell;
- cloudy flexible bladder;
- emissive nerve lines that illuminate from bottom to top during a call;
- conductive black contact pads at the feet.

### Motion vocabulary

- **Rest:** hangs between two anchors; bladder breathes slowly.
- **Listen:** probing limb touches a surface; ribbons become still.
- **Stalk:** moves by planting two feet and swinging the third in a delayed tripod cycle.
- **Brace:** widens stance and lowers bladder before a pressure wave.
- **Resonate telegraph:** ribs spread, bladder inflates, three nerve bands light in sequence.
- **Apex:** releases a directional chord that activates nearby conductive hazards.
- **Recover:** bladder collapses and the body becomes briefly unstable.
- **Stun:** anchors lose sync and one side sags.
- **Disable:** curls around its bladder and becomes a quiet bridge-like shell—non-lethal, no gore.

### Player read/counter

- wrist scan reveals which two feet are currently carrying the resonance;
- Sonic Thumper cancels one lit band;
- Gravity tool breaks an anchor during the recover window;
- random shooting during brace only strengthens the environmental hazard, teaching the intended counter.

### Ecology

- uses `Conductive`, `Resonant`, `Perch`, and `TidalEdge` affordances;
- juveniles gather near inactive array pylons;
- adults answer distant calls before storms;
- a repaired array changes their route and audio pattern through a saved world overlay.

### Review gate

The contact sheet must make the three-legged gait, pressure bladder, sequential tell, contact anchoring, and disabled curl readable without labels.

---

# 5. Suggested implementation order

Do not add all of this to the active queue at once. The goal is to make the next era executable.

## P0 — spend remaining Fable reasoning here

Use the remaining Fable capacity primarily to make decisions and write envelopes, not to start ten broad implementations.

1. **Integrate this packet into the existing maps** without replacing current priorities.
2. **Write the Creature Species / Visual Source / Motion Intent design contract** with exact assembly ownership and file paths.
3. **Write the World Moment system contract** with its closed action enum and persistence rules.
4. **Write the Surface Response + Haptic registry contract.**
5. **Define the ContinuityGate manifest schema and first four checks.**
6. **Create decision-complete Opus/Sonnet board rows** for only the first implementation slice of each.
7. **Resolve current dashboard/board contradictions and stale entrypoints.**

## P1 — first Opus/Sonnet implementation wave

Each item should be one or two small green commits.

1. Continuity manifest + report-only validator.
2. `CreatureSpeciesDefinition`/passport and completeness audit, with no behavior change.
3. `CreatureMotionIntent` seam, current gait fallback preserved.
4. Surface response and haptic data registries, with two proof callers.
5. World Moment pure core with three harmless actions and tests.
6. Creature motion contact-sheet CI artifact.
7. World review-view/contact-sheet artifact.

## P2 — Picasso creature life wave

1. Visual-provider interface and imported-hero adapter.
2. Contact anchors + ground-contact proof on one four/six-legged species.
3. State-specific motion profile and turn/alert/stun proof.
4. Secondary-motion profile.
5. Tell profile + creature voice profile.
6. One Signature creature passport implemented end to end.
7. Species family + one planet morph proof.
8. Promote completeness warnings to blockers only after the pilot species/world passes.

## P3 — world presence and premium polish

1. Horizon/mid-distance layer.
2. Ambient motion recipes.
3. Atmosphere pulses and occupancy cues.
4. World state overlays.
5. Encounter pacing definitions.
6. Diegetic UI art / readability / accessibility pass.

---

# 6. Specific Picasso lane guidance

## Continue FORGE III first

Do not abandon the active cohesion plan. Finish in its stated order:

- practical placement;
- water;
- grounding/blob shadows;
- VFX;
- reactive world;
- macro variation;
- signage;
- conformance ratchet;
- creature P4 close-out.

Those systems improve every existing asset and world.

## Then open a separate creature-life program

Suggested plan name:

`docs/project_art_plan/CREATURE_QUALITY_V2_LIFE_LEAP.md`

Do not call it FORGE IV if FORGE IV remains reserved for diegetic UI art. Keep the programs independent:

- **FORGE III:** environmental cohesion.
- **CREATURE QUALITY V2:** anatomy, motion, contact, tells, voice, ecology, and hero imports.
- **FORGE IV candidate:** diegetic UI art.

## Picasso's highest-value sequence

1. creature review contact sheet;
2. species passport + tier rules;
3. MotionIntent bridge;
4. look-at/stun/LOD remainder;
5. ground contact/foot lock on `swarm_bug`;
6. richer spine/head/jaw schema on one Signature creature;
7. secondary motion;
8. voice/tell grammar;
9. imported hero adapter;
10. one end-to-end Signature creature.

The first proof should be a creature whose body plan is **not** a biped and whose planet adaptation changes how it moves. That tests the architecture better than another armored humanoid.

---

# 7. Things not to do

- Do not replace the current world/Forge factories.
- Do not create a second roadmap-of-record.
- Do not make every creature an unrelated bespoke prefab.
- Do not raise bone/triangle budgets globally because one hero needs more.
- Do not give imported prefabs ownership of health, AI, targeting, rewards, saves, or networking.
- Do not create a unique AnimatorController web per species.
- Do not force camera movement for cinematic moments in VR.
- Do not add fully dynamic weather/simulation where a deterministic atmosphere pulse sells the same idea.
- Do not add every new response directly to weapons/creatures; route it through surface/feedback registries.
- Do not promote subjective art warnings to build blockers before one pilot asset/world proves the rule reliable.
- Do not let the desire for “AAA” hide the honest target: premium Quest-native cohesion, strong authored moments, memorable silhouettes, and believable response—not 500-person asset volume.

---

# 8. Integration prompt for the remaining Fable 5 session

Copy this prompt into the remaining Fable operator session:

```text
Read these in order on branch terry-local-wip:
1. docs/OPERATOR_START_HERE.md
2. docs/EXCELLENCE_MAP.md
3. docs/PRIORITIES.md
4. docs/project_art_plan/FORGE_III_PLAN.md
5. docs/SPRINT_ART.md
6. docs/GPT_ADDITIONS/2026-07-10_Post_Fable_Handoff/POST_FABLE_ARCHITECTURE_AND_PICASSO_PACKET.md

This is an architecture-integration session, not a broad implementation sprint.

Goal: preserve your remaining high-end reasoning by converting the packet's strongest additions into decision-complete, model-agnostic envelopes for Opus 4.8 / Sonnet 5, while keeping the existing roadmap and four-lane machine intact.

Required output:
A. Reconcile current dashboard contradictions/stale entrypoints that would mislead the next operator. Do not rewrite history.
B. Add or update Excellence Map rows for:
   - continuity enforcement/evidence;
   - presentation sequences/world moments;
   - surface response + haptic language;
   - world presence/horizon layer;
   - creature species completeness, contact, voice, imported hero path, and motion review artifacts.
C. Author exact design contracts, with assembly ownership, proposed paths, closed enums/data shapes, acceptance tests, audits, budgets, and "do not" rails, for:
   1. CreatureSpeciesDefinition + Forge/imported visual-provider seam + CreatureMotionIntent;
   2. WorldMomentDefinition/Director;
   3. SurfaceResponse/Feedback/Haptic registries;
   4. continuity project_manifest + report-only ContinuityGate.
D. Create only the first 1–3 Opus/Sonnet-sized board rows per owning lane. Do not dump the whole packet into active priorities.
E. Preserve FORGE III's active execution order. Open a separate future plan named CREATURE_QUALITY_V2_LIFE_LEAP rather than replacing FORGE III or stealing the FORGE IV diegetic-UI candidate.
F. Append HANDOFF entries and update the appropriate board(s) in the same commit.
G. Do not implement broad runtime systems unless one tiny scaffold is necessary to prove a contract. The main deliverable is an executable handoff architecture.

Important judgments already made—do not relitigate:
- Forge remains the scalable default; imported hero art is an alternate provider behind the same gameplay IDs.
- Creature quality requires anatomy + state-aware motion + contact + tells + voice + ecology, not more polygons alone.
- Presentation in VR moves the world, light, sound, and props, never the player's head.
- Surface/audio/VFX/haptic responses resolve through registries.
- All new systems follow spec/data + pure core + translator + tests + diagnostics + audit + runbook.
```

---

# 9. Takeover prompt after Fable 5

For an Opus 4.8 / Sonnet 5 operator after integration:

```text
Read docs/OPERATOR_START_HERE.md, then your assigned lane board and newest HANDOFF entries.
Do not implement directly from the GPT packet. Execute only the integrated board row and its owning design contract.
Use one small reversible commit: tests/pure core first, then translator/wiring, then CI. Preserve all existing fallback behavior. If the contract is incomplete or requires a new architectural decision, post a HANDOFF envelope instead of inventing it.
```

---

# Final priority recommendation

The highest-leverage use of the remaining Fable 5 capacity is **not another large feature**. It is to lock four architectures that smaller models can execute safely:

1. Creature Species + Motion Intent + Visual Provider.
2. World Moment / Presentation Sequence.
3. Surface Response + Haptic/Feedback Registry.
4. Continuity Manifest + Evidence Gate.

Then let Picasso finish FORGE III and use the new creature-life plan to build one unforgettable, planet-specific Signature organism end to end. That single proof will establish the production path for every creature that follows.
