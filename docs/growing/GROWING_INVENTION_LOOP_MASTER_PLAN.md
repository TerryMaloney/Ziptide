# ZIPTIDE GROWING + INVENTION LOOP — INITIAL MASTER PLAN

**Status:** PROVISIONAL DESIGN AUTHORITY FOR REVIEW — no runtime implementation authorized by this document.

**Created:** 2026-07-23

**Purpose:** Define one coherent long-term loop connecting seeds, cultivation, biological alchemy, mining, machine processing, crafting, equipment progression, ship preparation, limited field loadouts, accessibility, and later multiplayer variation. This document packages Terry's current direction for red-team/blue-team review before any system is locked or built.

**Review packet:** `docs/growing/GROWING_INVENTION_LOOP_REVIEW_PACKET.md`

**Protected prerequisite:** The exact M0 headset recovery route remains first. Nothing in this document authorizes changes to the frozen recovery candidate, rig/input lifecycle, weapons, coupler, travel, or recovery scenes before the device verdict.

---

## 1. Current proven project foundation

The project already contains meaningful pieces of the future loop. This plan must extend them rather than invent a second garden, economy, inventory, crafting, or automation stack.

### Existing or substantially built

- A persistent player profile and one canonical economy/reward path.
- Holster/belt inventory and ship/home-hub concepts.
- Garden genetics and watering foundations.
- Twenty-four authored plant species in the current project inventory, with three represented as direct assets and the remaining species currently authored through patcher/library paths.
- Deterministic, persistent belts and machines as an automation foundation.
- World jobs, salvage, mining-adjacent resource concepts, collections, weapons, abilities, and progression data.
- Catalog-breadth audits that already expose unsurfaced plant content rather than pretending all authored plants are reachable.
- Forge/registry, provenance, performance-budget, save-migration, CI, and device-evidence systems that future assets and recipes must use.

### Existing gaps this plan must address

- Most authored plant species are not deliberately surfaced to players.
- Seeds do not yet form a strong, understandable discovery and progression economy.
- Breeding/genetics exist more strongly as system foundations than as a polished, rewarding player loop.
- Giant crops and visible breeding outcomes remain incomplete.
- The relationship between cultivation, mining, machines, crafting, abilities, weapons, and loadout choice is not yet a single governed design.
- Conveyor and machine foundations exist, but their player-facing purpose, readable machine vocabulary, processing tiers, and relationship to recipes require deeper design.
- Kid-readable catalog navigation and spoken menu labels are not yet established as a project-wide content contract.
- The exact field-loadout capacity and slot rules must be source-audited before they become canon; Terry's current recollection is approximately three belt slots.

---

## 2. Design thesis

The garden is not a decorative farming minigame and not primarily a creature-collection system.

It is one half of the player's **invention economy**:

> Explore worlds, discover biological and mineral inputs, bring them home, cultivate or process them, combine them into useful technology, choose a limited loadout, and return to the worlds with new ways to solve problems.

The two production families are:

1. **Biological cultivation** — seeds, spores, fruit, fibers, enzymes, membranes, living catalysts, mutations, and rare reproductive lines.
2. **Industrial processing** — ore, salvage, crystals, machine parts, alloys, cells, precision components, and refined structural materials.

High-value equipment should often require both paths. The garden and machines should therefore feel complementary rather than like two disconnected crafting games.

### Intended player fantasy

- I found something strange and useful.
- I understand what it might become.
- I chose how to cultivate or process it.
- I created a new capability.
- I chose what to carry.
- I returned to a world with a different strategy.

---

## 3. Non-negotiable design pillars

### 3.1 Useful, not decorative

Every major plant family, processed material, buildable machine, crafted component, weapon upgrade, or ability must serve at least one clear function:

- exploration;
- traversal;
- protection;
- non-lethal combat/control;
- creature interaction;
- harvesting;
- repair;
- crafting/processing;
- ship/garden efficiency;
- story or world access;
- optional multiplayer variation.

Cosmetic variants may exist, but the core loop cannot depend on collecting attractive objects with no gameplay purpose.

### 3.2 Understandable experimentation

The player should be able to reason about combinations through families, traits, catalysts, and visible recipe clues. The system must not become hundreds of arbitrary combinations that require an external wiki.

### 3.3 Deterministic required progression

Required campaign equipment and story progression cannot depend on low-probability breeding or limited-time rotations. Important recipes must be discoverable and dependable.

### 3.4 Optional surprise

Mutation, rare traits, unusual offspring, cosmetic variants, and multiplayer rotations may create surprise without blocking completion.

### 3.5 Limited field choices

The ship contains the player's collection. The field loadout contains the player's current strategy. Carry limits preserve meaningful choice and prevent crafting progression from erasing all challenge.

### 3.6 Physical first, automation later

Players should manually learn a transformation before machines automate it. Automation removes understood repetition; it must not replace the enjoyable discovery and interaction layers.

### 3.7 Child-readable by design

Every buildable machine, plant family, seed, recipe output, ability, weapon, and important material must have:

- a stable name;
- a unique, recognizable symbol or silhouette;
- consistent category marking;
- visible text;
- a spoken label when highlighted or focused;
- a concise spoken functional description on request;
- no requirement that a young player read dense recipe text unaided.

The Minecraft-like recognition goal is: a child can learn the symbol and object shape, while narration tells them what they selected and what it does.

---

## 4. The complete macro loop

1. **Discover** — encounter a seed, biological sample, mineral, machine part, blueprint clue, or planetary signature item.
2. **Identify** — scan it to reveal family, known traits, origin, potential uses, and undiscovered properties.
3. **Choose a path** — cultivate, breed, process, preserve, spend, or hold for a known recipe.
4. **Transform** — use a plot, nursery, extractor, refiner, assembler, or later conveyor-connected line.
5. **Produce** — gain seeds, recurring plants, refined materials, components, consumables, upgrades, weapons, or abilities.
6. **Prepare** — inspect visible goals and assemble a limited field loadout aboard the ship.
7. **Return** — revisit or enter a world with new traversal, control, harvesting, repair, or creature-interaction options.
8. **Learn and expand** — discoveries unlock more recipes, processing tiers, seed families, machine functions, and strategic combinations.

This loop must complement the machine/salvage battle economy rather than replace it. Combat, disable, salvage, and machine-resource acquisition remain major sources of industrial progression. Cultivation adds strategic capabilities and cross-system ingredients without becoming the sole path to power.

---

## 5. Cultivation as biological alchemy

### 5.1 Plot recipe model

A cultivation plot may accept multiple seed inputs rather than only one seed. A recipe can use three conceptual roles:

- **Base mass** — common seeds or biological material that establish the plant body.
- **Trait inputs** — seeds/materials that contribute a capability such as conductivity, buoyancy, thermal resistance, adhesion, glow, filtration, or elasticity.
- **Catalyst** — a rarer seed, mineral, enzyme, environmental condition, or processed component that determines the advanced result.

Illustrative structure only:

- eight base seeds;
- two trait seeds;
- one catalyst;
- one resulting plant, component, consumable, or advanced seed.

Exact counts must be tuned for comprehension, pacing, inventory pressure, and child usability. The design law matters more than the example numbers.

### 5.2 Known recipes versus discovery

Recipes can exist in three states:

1. **Visible goal** — the player sees a desired weapon/ability/component and the broad ingredient families needed.
2. **Partially discovered** — known ingredients are shown; unknown ones appear as silhouettes, trait clues, planet hints, or question marks.
3. **Mastered** — exact recipe, repeat behavior, output, and automation eligibility are known.

Required recipes should gain reliable discovery paths through world progression, RILL guidance, scans, experiments, recovered records, or machine analysis.

### 5.3 Plot outputs

A completed plot may produce one of several output types:

- a recurring harvest plant;
- a one-time biological component;
- a consumable effect;
- a seed of the same family;
- a higher-tier seed;
- a hybrid seed;
- a living catalyst;
- a garden-support entity or sprite-like helper;
- a component used with refined minerals in equipment assembly.

The output must be stated before the player commits rare ingredients unless uncertainty is explicitly part of an optional experiment.

---

## 6. Adjacency, reproduction, and breeding

Two mature compatible plots placed beside each other may produce reproductive outcomes.

### Same-family adjacency

Possible results:

- replacement seeds;
- improved yield;
- retained parental traits;
- a chance at a stable higher-quality variant;
- a reliable way to sustain a useful plant line.

### Different-family adjacency

Possible results:

- hybrid seeds;
- one inherited trait from each parent;
- a rare mutation;
- a new catalyst line;
- a cosmetic or sprite-associated variant.

### Fairness law

- Campaign-critical results are deterministic or protected by a guaranteed progression rule.
- Randomness may affect optional rarity, efficiency, appearance, or unusual trait combinations.
- Failed experiments must still produce understandable value or learning; they cannot simply erase hours of collection without warning.
- The system must state whether a pairing is compatible, unknown, unstable, or impossible.

---

## 7. Seeds, plants, and traits

Each seed should be more than an undifferentiated inventory count.

### Required seed identity fields

- stable ID;
- child-readable display name;
- spoken name;
- unique icon/silhouette;
- family;
- planet/biome origin;
- rarity or progression band;
- known traits;
- undiscovered-trait state;
- compatible plot/condition types;
- likely output families;
- campaign availability rule;
- multiplayer-rotation eligibility;
- provenance and art registry references.

### Recommended conceptual layers

1. **Species/family** — what biological line it belongs to.
2. **Traits** — what functional properties it can contribute.
3. **Temperament/stability** — stable, volatile, symbiotic, parasitic, social, nocturnal, fast-growing, and similar behavior categories.

Traits should have functional logic. For example:

- conductive biological material contributes to static-control equipment;
- flexible fiber contributes to tethers, nets, seals, or wearable adaptations;
- filtration contributes to environmental protection;
- buoyant sacs contribute to lift or fall-control systems;
- adhesive roots contribute to climbing, anchoring, or repair compounds.

The system should teach a reusable language rather than isolated recipes.

---

## 8. Industrial processing and mining relationship

Mining and salvage form the parallel industrial path.

### Machine progression

Buildable machines may advance through capability tiers rather than only faster output:

- basic sorting and crushing;
- extraction and purification;
- alloying or crystal stabilization;
- precision component fabrication;
- biological/industrial fusion;
- advanced assembly.

Upgrades may unlock new material classes, recipe slots, processing temperatures/fields, efficiency, reliability, or automation connections.

### Cross-path recipes

High-value equipment should often combine:

- one industrial frame/core;
- one biological functional material;
- one catalyst or planetary signature component;
- optional modifiers that change behavior rather than only damage numbers.

This protects the importance of machine-resource gameplay while giving cultivation a necessary, useful role.

---

## 9. Visible goals and the invention catalog

The ship should contain an invention catalog resembling a readable crafting bench, research board, and loadout planner.

### Catalog cards

Each buildable item should display:

- recognizable icon or 3D preview;
- stable name;
- spoken name on focus;
- one-sentence spoken purpose;
- category;
- field-slot cost;
- known required components;
- unknown components as silhouettes or clues;
- which production paths are involved: garden, mining/machine, salvage, world discovery, or combined;
- discovered source hints;
- current inventory counts;
- whether the recipe is available, incomplete, locked, experimental, or mastered.

### Narration behavior

Provisional accessibility target for review:

- Focus/highlight speaks the item name after a short anti-chatter delay.
- A dedicated input repeats the name and reads the concise function.
- Another deliberate action reads recipe requirements and current counts.
- Narration does not constantly overlap RILL, alerts, or important gameplay audio.
- Speech rate and narration volume are adjustable.
- Icons, category shapes, and spatial position remain consistent for players who learn visually.
- Critical information is never available only through speech or only through color.

Exact input, timing, voice source, localization strategy, and audio-ducking behavior require architecture/device review.

---

## 10. Equipment, abilities, and limited loadouts

Each world may introduce a signature weapon, ability, tool, biological adaptation, or machine technology. Later invention recipes allow upgrades, sidegrades, and combinations.

### Progression principle

Prefer new verbs and strategic options over simple numerical escalation.

Examples of functional categories:

- gravity manipulation;
- static control;
- sonic interaction;
- filtration or environmental adaptation;
- creature lure/repel/communication;
- traversal assistance;
- salvage or harvesting specialization;
- repair and machine-control tools;
- signal concealment or detection.

### Loadout principle

- The ship stores the full collection.
- The player chooses a limited field kit before departure.
- Heavy or unusually powerful items may consume more capacity.
- Loadout limits must be understandable visually and through narration.
- Returning to the ship to change strategy should feel like preparation, not punishment.
- Worlds should support multiple useful loadout approaches without requiring every possible tool.

The exact number and types of slots remain an open design/source-audit question. Do not lock “two abilities plus one weapon” or any other arrangement until existing belt/holster behavior and device usability are reviewed.

---

## 11. World-design implications

The invention loop creates freedom only if worlds contain systems that respond to different tools.

A world may offer alternate approaches such as:

- repair or reroute machinery;
- grow or deploy an organic bridge/support;
- use gravity, static, sonic, or traversal tools;
- lure, calm, avoid, or redirect a creature;
- process a local resource in place;
- return later with a specialized harvesting or environmental loadout.

### Anti-lockout law

Story-critical completion must always have a dependable path using equipment the player can reasonably possess. Optional shortcuts, secrets, high-yield resources, alternative resolutions, and replay variety may reward specialized loadouts.

### World factory integration

Future WorldSpecs/production packets should eventually be able to declare:

- signature discovery;
- available seed/material families;
- machine-processing opportunities;
- required and optional capability tags;
- loadout-sensitive routes;
- recipe clues;
- garden or mining progression outputs;
- replay opportunities created by later equipment.

These schema extensions are proposals only until Architect review.

---

## 12. First-hour and tiered introduction

The system must unfold gradually.

### Introductory band

- one visible grow plot or tray;
- a very small number of seed families;
- one clear known recipe;
- one simple nurture/commit action;
- one functional output used soon afterward;
- spoken names and visible symbols from the first interaction;
- no dense genetics or automation menu.

### Second-world expansion

- additional seed inputs;
- one adjacency/reproduction lesson;
- one garden-plus-machine combined recipe;
- one equipment goal visible before all ingredients are known;
- first meaningful loadout decision.

### Mid-game expansion

- hybridization;
- higher-tier machines;
- biological/industrial fusion;
- optional mutations;
- automated movement/processing of mastered recipes;
- planet-specific resource and equipment identities;
- deeper strategic loadouts.

### Late-game expansion

- complex multi-stage recipes;
- rare cross-world combinations;
- specialized garden helpers;
- high-capability machine lines;
- advanced sidegrades and world-revisit opportunities;
- optional mastery collections and multiplayer variants.

The first hour teaches one understandable action and one useful payoff. It must not expose the complete system at once.

---

## 13. Conveyor and automation relationship

Conveyors should connect cultivation, processing, storage, and assembly after the player understands those transformations manually.

Possible mature flow:

> plot harvest → collection/storage → biological extractor → refined biological component → combined assembler with processed mineral → equipment component or consumable

### Automation laws

- Only mastered recipes are eligible for unattended automation unless explicitly designed otherwise.
- Every machine has a visible and spoken identity.
- Inputs, outputs, blocked states, and errors are readable without small text.
- Automation saves repetition but does not bypass progression discovery.
- Persistent state must survive travel, quit, migration, interruption, and old saves.
- Throughput must be Quest-budgeted and visually legible.
- Conveyor planning remains a later dedicated design lane; this master plan defines its purpose but does not pretend the current conveyor design is complete.

---

## 14. Sprite-like garden life

Small lovable plant-associated entities may support collection appeal without replacing function.

Potential roles:

- reveal or hint at hidden traits;
- improve reproduction or yield for one family;
- collect dropped seeds;
- indicate compatible nearby plots;
- react to rare mutations;
- provide ambient ship life;
- serve as cosmetic or multiplayer variants.

They should represent and assist the cultivation system rather than become mandatory combat pets. Their exact place remains open for review because they can easily inflate art, AI, save, and content scope.

---

## 15. Campaign versus multiplayer variation

### Campaign

- stable authored seed availability;
- permanent discoveries;
- dependable progression recipes;
- no required limited-time ingredients;
- no fear-of-missing-out pressure;
- suitable for offline play.

### Multiplayer or rotating challenge layer

May rotate:

- available optional seed families;
- mutation traits;
- garden conditions;
- experimental recipes;
- arena/challenge modifiers;
- cosmetic variants;
- temporary production bonuses.

Rotation must not invalidate campaign completion or make a child feel that necessary equipment disappeared. The online/live-content implications, moderation burden, schedule ownership, save boundary, and Photon dependency require T-Dog and Architect red-team review.

---

## 16. Economy and balance boundaries

The garden must matter without overwhelming the primary salvage/machine progression.

### Proposed division of value

- **Machine/salvage path:** primary structural progression, weapon frames, major machinery, industrial upgrades, battle-earned resources.
- **Garden path:** functional biological traits, adaptations, modifiers, consumables, reproduction, catalysts, and some specialized ability components.
- **Combined path:** advanced equipment, sidegrades, rare capabilities, and high-value strategic tools.

### Anti-grind laws

- No critical recipe should demand tedious bulk collection without automation or a guaranteed acquisition route.
- Common seed quantities should be readable and reasonably attainable.
- Rare catalysts should come from memorable discoveries, challenges, or controlled progression rather than unbounded random drops.
- Failed experiments should produce knowledge or salvageable value.
- Inventory limits, duplicate handling, seed storage, and recipe tracking must prevent menu clutter.
- Upgrades should expand choices more often than they simply increase damage.

---

## 17. Accessibility and child-usability contract

This is a core requirement, not optional polish.

Every interactable catalog or build menu must support recognition through at least three channels:

1. **Visual identity** — object silhouette/icon/preview.
2. **Text identity** — stable readable name and concise function.
3. **Spoken identity** — name on focus and optional expanded description/requirements.

### Additional provisional requirements

- Large, reachable VR targets.
- Stable menu layout and category placement.
- Category symbols that do not rely only on color.
- Current/required counts presented visually and spoken on request.
- Unknown ingredients represented consistently.
- Confirmation before spending rare or progression-critical inputs.
- Undo/cancel before a recipe commits where technically possible.
- No punishment for accidental menu focus changes.
- Narration can be enabled by default for the introductory flow or offered clearly during onboarding.
- Narration, subtitles, RILL, warnings, and menu speech require one audio-priority/ducking policy.
- A six-year-old usability pass is treated as valuable internal evidence even if the store rating targets older children.

The review team must determine which requirements belong in a reusable project-wide `AccessibleCatalogItem`/narration contract rather than only the garden UI.

---

## 18. Content and technical data boundaries

The exact architecture remains for review, but the design should avoid duplicating existing owners.

Potential data authorities may include:

- seed/plant definitions;
- trait definitions;
- cultivation recipes;
- breeding compatibility rules;
- material definitions;
- machine definitions and processing recipes;
- equipment/ability recipes;
- catalog presentation/narration metadata;
- loadout capacity tags;
- campaign availability;
- multiplayer rotation eligibility.

### Required cross-cutting integrations

- one economy/reward owner;
- one inventory/profile owner;
- one save/migration route;
- one audio event and narration policy;
- Forge/registry/provenance for visuals;
- licensing-at-import;
- performance budgets;
- WorldSpec/production-packet references;
- deterministic validation and CI;
- exact device evidence.

No new service should be created merely because this plan uses a new name for an existing responsibility.

---

## 19. Principal risks to red-team

1. **Scope explosion:** garden + breeding + sprites + mining + machines + conveyors + crafting + loadouts + multiplayer can become several games inside one game.
2. **Recipe opacity:** arbitrary combinations can become wiki-dependent and frustrating for children.
3. **Inventory burden:** many seeds, traits, materials, and components can overwhelm storage and menus.
4. **Economy dominance:** cultivation could trivialize salvage/combat progression or become irrelevant beside it.
5. **One optimal build:** loadout freedom may collapse if one combination dominates most worlds.
6. **Retrofit burden:** worlds may require new capability tags and alternate paths before content scale.
7. **Automation removes play:** conveyors could turn satisfying physical actions into passive waiting.
8. **Randomness blocks fun:** rare breeding outcomes can create grind or perceived unfairness.
9. **Kid narration fatigue:** focus speech can chatter, overlap, or slow expert players.
10. **Audio/content cost:** every spoken label and description increases localization/audio production obligations.
11. **Quest performance:** plants, growth animation, helpers, machines, belts, and item flow can multiply runtime objects.
12. **Save/migration complexity:** genetics, adjacency, automation state, rotating inventory, and ongoing production create persistent-state risk.
13. **Online rotation scope:** live scheduling and temporary content may require backend/operations work inconsistent with the current offline-first plan.
14. **Art burden:** many seed/plant variants and sprites may outrun the asset pipeline.
15. **Feature timing:** implementing this before W001/W002 production truth could distract from the current world-factory objective.

---

## 20. Review decisions required before locking

The three reviews should recommend answers, not merely list concerns.

1. What is the smallest fun campaign version that preserves the long-term idea?
2. Which existing garden/genetics/machine systems survive as canonical owners?
3. What exact recipe grammar makes combinations understandable?
4. How should deterministic recipes and optional breeding randomness coexist?
5. What should plants produce in the first hour, early game, and mid-game?
6. How should biological and industrial progression divide power?
7. What loadout limit creates choice without frustrating children?
8. How should spoken menu narration work without chatter or audio conflicts?
9. Which data belongs in generic reusable schemas versus ZIPTIDE content?
10. What needs to be present in W001, W002, and only later?
11. What machine/conveyor work must be postponed until the manual loop is proven?
12. Should sprite-like helpers exist in v1, later, or only as presentation?
13. Is multiplayer rotation compatible with the offline-first/release plan, and if so, when?
14. What audits/tests/device checks prevent the system from drifting or becoming unreadable?
15. What should be explicitly cut to protect the core game schedule?

---

## 21. Initial recommended minimum vertical slice

This is a proposal for review, not an authorized build.

### W000/W001 minimum

- one accessible grow plot;
- three clearly differentiated seed families maximum;
- one deterministic multi-input recipe;
- one plot output with immediate gameplay use;
- one same-family adjacency/reproduction demonstration or deferred visible tease;
- one invention-catalog card with icon, name, spoken label, function, and ingredient progress;
- one combined biological + industrial recipe visible as a future goal;
- one simple loadout choice that changes a world interaction;
- no conveyor automation yet;
- no random campaign blocker;
- no multiplayer rotation;
- no large sprite roster.

### W002 replication proof

- additional recipe or trait family;
- one different cultivation result;
- one garden-plus-machine combination;
- one reason to change loadout;
- reuse the same UI, narration, save, recipe, and catalog contracts;
- prove the framework rather than add W002-specific runtime code.

### Expansion gate

Do not build the broad system until the first two worlds prove:

- a child can identify, hear, select, and complete a recipe;
- the output is useful soon enough to feel meaningful;
- the recipe language is understandable;
- the save survives interruption and return;
- the loadout choice changes play;
- the implementation reuses existing owners;
- the second world is primarily data/content work.

---

## 22. Product-framework value

If proven across ZIPTIDE worlds, the generic portion could later become part of the reusable game-production framework:

- trait-based resource definitions;
- deterministic and experimental recipe grammar;
- accessible catalog cards with narration metadata;
- cultivation/processing/assembly pipelines;
- persistent machine state;
- loadout-capacity contracts;
- WorldSpec capability tags;
- audit and device-evidence templates.

That extraction remains later work. The first responsibility is to make the system excellent and useful inside ZIPTIDE.

---

## 23. Current authority and next step

This document is the packaged initial proposal. It does not supersede existing garden, economy, machine, inventory, first-hour, accessibility, or world-factory authorities until the reviews identify the correct surviving owners and Terry adjudicates the major choices.

Next:

1. Reasonbox performs systemic fun/progression/economy review.
2. Architect performs architecture/data/ownership/automation review.
3. T-Dog performs device/accessibility/release/abuse-risk review.
4. GPT reconciles agreements, conflicts, and recommended cuts into a revised decision document.
5. Terry decides the disputed design calls.
6. Implementation remains behind the M0 headset verdict and the W000→W001 production order.
