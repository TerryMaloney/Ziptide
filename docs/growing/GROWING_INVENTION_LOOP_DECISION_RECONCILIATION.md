# ZIPTIDE GROWING + INVENTION LOOP — DECISION RECONCILIATION

**Status:** RECONCILED DESIGN AUTHORITY — implementation remains gated behind the exact M0 headset verdict, explicit recovery-freeze lift, and the canonical W000→W001 production order.

**Date:** 2026-07-23

**Inputs reconciled:**

- `docs/growing/GROWING_INVENTION_LOOP_MASTER_PLAN.md`
- `docs/growing/reviews/REASONBOX_GROWING_LOOP_REVIEW.md`
- `docs/growing/reviews/ARCHITECT_GROWING_LOOP_REVIEW.md`
- `docs/growing/reviews/TDOG_GROWING_LOOP_REVIEW.md`
- `docs/production/POST_HEADSET_WORLD_FACTORY_ORDER.md`

This document replaces the provisional proposal wherever they conflict. It does not authorize implementation before M0.

---

## 1. Final verdict

Adopt the central thesis:

> The garden is one half of ZIPTIDE's invention economy. Players discover biological and industrial inputs, bring them home, transform them through cultivation and machines, create useful capabilities, choose three items for the field, and return to worlds with different ways to solve problems.

Build it as an **extension of existing tested owners**, not as a new garden, crafting, inventory, economy, machine, or save stack.

The initial implementation must be approximately one-third the breadth of the provisional concept. It proves one satisfying loop before adding breeding depth, conveyor automation, helpers, rotations, large catalogs, or many processing tiers.

The emotional payoff is:

> I found this. I understood it. I made something with it. I chose to bring it. It changed what I could do.

---

## 2. Consensus decisions — lock now

All three reviewers agree on these principles.

### 2.1 Extend existing owners only

Canonical owners are binding:

- `PlantGenetics` owns instance genetics, crossing randomness, rarity and giant thresholds.
- `GardenService` owns plant, tend, harvest and adjacency-cross lifecycle.
- `PlantDefinition` and `GardenAuthor` own authored species.
- `RecipeDefinition` and `RecipeService` own every transformation recipe.
- `MachineNodeState` owns machine processing, recipe progress and persistence.
- `ResourceLedger` and `RewardRouter` own counts, spending and rewards.
- `ItemDefinition`, `ItemFactory`, inventory and holsters own equipment identity and field carrying.
- `PlayerProfile` and `ProfileSerializer` own discovery and persistent progression.
- `WorldPackDefinition`, WorldSpecs and existing authors own world surfacing.
- existing catalog-breadth, reach, readability, economy, performance, save and CI systems own verification.

Forbidden:

- a second inventory;
- a second crafting service;
- a second genetics/randomness path;
- a second machine or conveyor state graph;
- garden-specific parallel economy counts;
- runtime-generated species definitions;
- hard-coded world-specific generic services.

### 2.2 One universal recipe family

Extend `RecipeDefinition`; do not create separate plot, refining and assembly recipe systems.

The eventual schema may support:

- domain: plot, machine or assembly;
- role-tagged inputs: base, trait and catalyst;
- trait-based requirements;
- machine or plot requirements;
- stable output ID and amount;
- campaign and multiplayer applicability;
- unlock/discovery data;
- readable clue text.

Legacy recipe fields remain supported so existing content does not require a destructive migration.

### 2.3 Recipe determinism

**Recipes never roll.** A committed recipe produces the authored output every time.

Only `PlantGenetics` may introduce optional variation in plant instances, using its existing seeded, save-stable behavior.

- Required campaign chains: zero RNG end to end.
- Optional rarity: genetics or authored rare catalysts only.
- Runtime-created hybrid species: prohibited.
- Hybrid species: authored definitions unlocked deterministically.

A visible bounded-pity system may later support optional rare genetic outcomes, with a recommended maximum of five crosses. It is not required for the first slice.

### 2.4 Three-input physical grammar

The v1 physical recipe cap is:

- one base input;
- one trait input;
- optional one catalyst.

Maximum: three deliberate physical insertions before commit.

The former `8 + 2 + 1` illustration is cut from v1. Large quantities may exist as abstract stored-resource costs in later industrial recipes, but the child-facing physical interaction may not require eleven repeated insertions.

Every recipe must be explainable as one spoken sentence. Examples:

- `salvage frame + glow fiber = lantern`;
- `alloy body + gripping vine = climbing tool`;
- `filter membrane + sealed frame + spore catalyst = hazard mask`.

If a recipe cannot be described simply, it is too opaque or contains too many responsibilities.

### 2.5 Biological and industrial paths couple by function

Do not force both paths into every valuable recipe merely to make both economies mandatory.

Default design language:

- industrial materials form the **body**: frame, structure, power housing, durability, machinery;
- biological materials provide the **behavior**: grip, glow, filter, float, conduct, flex, repel, attract, adapt.

Combined inventions should feel logically combined, not artificially gated.

No recipe may require more than one genuinely scarce ingredient in v1.

### 2.6 Three-slot field kit

The existing belt has exactly three holster sockets. Lock v1 to:

- three carried items;
- one item per socket;
- no abstract capacity menu;
- no weight tiers or two-slot heavy items in v1.

The ship holds the collection. The visible belt is the current strategy.

One mid-world swap location may be used where needed to prevent a wrong loadout guess from forcing a complete travel loop. It must be an authored world facility, not a universal inventory-anywhere loophole.

### 2.7 Child-readable physical interaction

Garden and invention v1 follows a **no scrolling lists** law.

Use:

- recognizable physical seed pucks or containers;
- shaped or clearly marked base/trait/catalyst sockets;
- physical catalog cards or tiles;
- stable icons and silhouettes;
- text labels;
- non-color category encoding;
- physical commit/cancel controls;
- clear before-and-after resource counts;
- rare-resource confirmation and undo where technically safe.

A six-year-old should be able to learn the object and symbol even before reading the text.

### 2.8 Accessible narration contract

Text metadata is the authority. Add project-wide accessible label fields rather than garden-only narration data:

- spoken name;
- one-sentence spoken function;
- visible/caption name;
- visible/caption function;
- stable item/definition ID.

Delivery contract:

- name speaks after approximately 0.5 seconds of stable focus;
- one-sentence function speaks only on request;
- full recipe requirements and counts remain visual/caption information;
- RILL and safety alerts outrank menu narration;
- all speech uses one governed narration bus and ducking policy;
- captions function before narration audio is produced.

Use prerecorded or batch-generated stable-ID audio clips for shipping. Runtime on-device TTS is cut. The text-first schema remains the source, so clips can be replaced, localized or regenerated without changing gameplay data.

### 2.9 Interruption and pressure laws

- Recipe commit is atomic: all inputs spend and processing starts, or nothing changes.
- Doff, system overlay, tracking loss, controller sleep, quit or travel may not create half-spent recipes.
- Cultivation contains no real-time-pressure failure step.
- Plants may mature while the player is away, but no required plant dies or permanently fails because a child removed the headset or stopped playing.

### 2.10 Physical first, automation later

Players manually perform and understand a transformation before automating it.

Automation may execute only mastered recipes. It removes repetition; it does not hide undiscovered logic or replace the enjoyable first encounter.

---

## 3. Initial content vocabulary

### 3.1 Seed families

Begin with **six functional families**, regrouping the existing 24 authored species rather than creating 24 new parallel concepts.

The final names require a content pass, but the family verbs should be simple enough for a child to say and remember. Candidate functional language:

- glow;
- grip;
- float;
- filter;
- conduct/zap;
- flex/grow.

A family can contain multiple species, appearances, origins and genetic variants while sharing a recognizable mechanical vocabulary.

### 3.2 Trait language

Traits are verbs or immediately understandable capabilities, not scientific stat labels.

Good:

- glows;
- sticks;
- floats;
- filters;
- conducts;
- stretches;
- repels;
- attracts.

Avoid traits that require reading several paragraphs before the player knows why they matter.

### 3.3 Early catalog size

Targets by the end of W002:

- six families defined in data;
- two or three families actually introduced through W000/W001;
- eight to ten catalog cards visible as known goals or silhouettes;
- four or five recipes craftable;
- one plot recipe;
- one simple machine/refining recipe;
- one combined biological/industrial invention;
- no more than extractor/BioRefiner and Assembler as meaningful early processing machines.

These are initial tuning targets, not reasons to bloat the first hour.

---

## 4. Campaign progression curve

### 4.1 W000 — promise without a menu lesson

W000 introduces cultivation only through one planter beat:

1. receive or find one clearly identified starter seed;
2. place it in one plot before departure;
3. see one silhouette catalog card showing a future useful output;
4. leave for the first world;
5. discover on return that the plant grew while the player was away.

W000 does **not** teach full recipes, breeding, machine tiers, adjacency or a large catalog.

Mandatory garden time stays below roughly 90 seconds and functions as a departure/return bookend.

### 4.2 W001 — first useful invention

W001 performs the real seed-surfacing work:

1. find and scan the first trait-bearing biological input;
2. narration/caption states its name and simple function;
3. the catalog reveals or advances one desirable invention card;
4. earn an industrial frame or component through the primary job/salvage loop;
5. return to the ship;
6. complete one deterministic recipe using the physical grammar;
7. equip the resulting invention in one of the three belt sockets;
8. use it to solve or improve one world interaction on a revisit or next route.

The first invention should demonstrate the thesis, not merely improve a number. A lantern, gripping tool, filter or conductive tether is stronger than `+5% damage`.

### 4.3 W002 — replication and first real choice

W002 proves the system generalizes:

- cultivation content is authored through the same data and authors;
- no new generic C# is required;
- introduce adjacency crossing or a simple authored hybrid unlock;
- introduce the first combined bio/industrial recipe if W001 used only a simpler recipe;
- present the first meaningful full-belt choice;
- provide one mid-world swap location if the route tests loadout preparation;
- include one revisit payoff using an earlier invention;
- prove save, quit, travel and return behavior.

**Architecture pass/fail:** W002 content must be data-only. If it needs generic runtime code, stop and repair the framework before W003.

### 4.4 W003–W005 — throughput mini-batch

Each world contributes at most one headline invention-economy addition:

- one seed family introduction;
- one catalyst;
- one processing tier;
- one sidegrade;
- or one new use for an existing trait.

Do not introduce a family, catalyst and machine tier simultaneously in one ordinary world.

The mini-batch proves that specs, authors, gates and representative device tests are accelerating production.

### 4.5 Mid-game

After the manual loop is proven:

- deeper authored hybrid lines;
- visible optional genetic rarity/pity;
- extractor and assembler upgrades;
- mastered-recipe automation;
- cross-world combinations;
- planet-signature catalysts;
- revisits that expose alternate routes or solutions;
- specialized field kits without increasing the three-slot count.

### 4.6 Late game

Late progression expands strategy rather than only increasing power:

- stronger sidegrades and combinations;
- advanced biological adaptations;
- industrial machine efficiency;
- rare but deterministic campaign catalysts;
- multiple valid approaches to difficult jobs and environments;
- automated production of already-mastered essentials;
- optional collection/genetics depth.

---

## 5. Economy balance

Initial tuning target:

- 60% of structural progression value from machine/salvage/mining;
- 25% from cultivation and biological traits;
- 15% from memorable combined inventions.

This is a starting balance target, not immutable canon. Instrument it through existing ledger sources and review actual play behavior.

Cultivation must not become the only meaningful progress made in a session. Garden actions primarily bookend world activity; the primary game remains exploration, jobs, non-lethal conflict, machines, salvage and discovery.

Anti-grind laws:

- known required recipes disclose their path;
- the player is not repeatedly blocked by several scarce ingredients;
- no required chain depends on random breeding;
- no important campaign item depends on a limited-time rotation;
- repeat collection may be automated only after mastery;
- new capability rewards are preferred over small numerical upgrades.

---

## 6. Architecture and data model

### 6.1 Minimum genuinely new authorities

1. **Trait definitions**
   - stable ID;
   - readable name and icon/silhouette identity;
   - spoken name/function metadata;
   - functional verb/category;
   - optional compatibility tags.

2. **Recipe extension**
   - domain;
   - role-tagged inputs;
   - trait requirements;
   - clue/discovery information;
   - deterministic output.

3. **Recipe discovery state**
   - Unknown;
   - Seen;
   - Partially Known;
   - Mastered.

4. **Project-wide accessible-definition metadata**
   - spoken name/function text;
   - caption fallback;
   - stable narration IDs.

5. **Existing holster capacity enforcement**
   - fixed three sockets;
   - v1 one item per socket.

### 6.2 Save and migration

Persist IDs and integers only.

Recommended additions:

- recipe discovery rows by recipe ID;
- discovered trait IDs;
- plot recipe ID and input-lineage IDs only where required.

Old saves receive empty neutral lists. Unknown/deprecated IDs are preserved and diagnosed rather than silently deleted.

Multiplayer or rotating content state must never enter the offline campaign profile.

### 6.3 World integration

WorldSpecs and world packs declare surfacing and capability opportunities through optional additive data. Do not add mandatory fields that invalidate all existing worlds.

Every production packet may declare:

- introduced seed family or catalyst;
- source/scan location;
- recipe clue;
- machine availability;
- invention payoff interaction;
- optional alternate route capability;
- representative device test.

---

## 7. Implementation order after M0

No implementation starts until:

1. the exact M0 recovery test is complete;
2. baseline blockers are closed;
3. the recovery freeze is explicitly lifted for these files;
4. W000→W001 production-band ownership is assigned.

Then implement in bounded slices.

### Slice A — pure data and persistence foundations

- source audit immediately before editing;
- TraitDefinition or equivalent existing-definition extension;
- RecipeDefinition backward-compatible extension;
- recipe discovery states;
- profile migration and neutral old-save defaults;
- pure EditMode tests;
- no scenes or UI.

### Slice B — early authored data

- define six family concepts using existing species;
- author only the two or three W001-exposed families;
- one deterministic plot recipe;
- one industrial component recipe;
- one combined invention goal;
- catalog clue/silhouette data;
- provenance and recipe-currentness evidence.

### Slice C — physical planter and catalog

- W000 planter beat;
- one plot accepting the limited physical grammar;
- one physical card/tile catalog surface;
- no scrolling list;
- visible costs, unknown silhouettes and output function;
- atomic commit/cancel behavior;
- narration text/caption seam.

### Slice D — W001 surfacing and invention payoff

- seed/resource appears through authored world data;
- scan advances discovery state;
- job/salvage supplies the industrial component;
- ship recipe creates the first useful item;
- one world interaction reads the capability through existing item/interaction ownership;
- belt enforces three items.

### Slice E — gates and device evidence

Add or extend blocking checks for:

- recipe closure and resolvable IDs;
- campaign determinism;
- campaign/rotation separation;
- discovery-state save round trips;
- narration metadata coverage;
- catalog reach/readability;
- capacity sanity;
- unsurfaced seed families;
- generated-content currentness;
- object/performance budgets.

Device-only evidence covers:

- six-year-old comprehension;
- child/seated/standing reach;
- focus dwell and narration chatter;
- atomic commit interrupted by doff/overlay;
- growth and output readability;
- three-slot equipping;
- 72 Hz and memory behavior;
- travel, quit and return.

### Slice F — W002 replication

- new species, recipe and catalog rows through existing authors;
- zero generic C#;
- first adjacency lesson;
- first full-belt strategic choice;
- first representative revisit payoff;
- compare implementation effort against W001.

If W002 is not substantially faster, repair the framework before W003.

---

## 8. Accessible interaction specification

### 8.1 Identity contract

Every visible seed, plant family, material, machine, recipe output, ability and weapon must have:

- stable ID;
- unique silhouette or physical form;
- icon;
- non-color category marker;
- visible name;
- visible one-line function;
- spoken name text;
- spoken one-line function text;
- stable narration event IDs.

### 8.2 Focus and speech

- focus must remain stable for approximately 0.5 seconds before speaking;
- moving quickly across objects must not trigger a stream of names;
- repeated focus receives a cooldown;
- function speech requires an explicit request;
- RILL, safety and progress alerts preempt catalog narration;
- captions remain available even when speech is not produced;
- full ingredient lists are not automatically narrated.

### 8.3 Physical recipe board

Use visibly different roles:

- base socket;
- trait socket;
- catalyst socket.

Roles require shape, symbol and text—not color alone.

Before commit, show:

- inserted inputs;
- resulting known output or silhouette;
- whether the result is required, optional or undiscovered;
- rare-input warning;
- what will remain after spending.

Commit uses one deliberate physical action. Cancel removes inputs without loss. Interrupted commit is atomic.

### 8.4 Kid-understanding gate

`Name it, say it, use it`:

A six-year-old should be able to:

1. point to a card/object and hear its name;
2. explain one trait in their own words after using it;
3. complete a known recipe without assistance on the second attempt;
4. equip the produced item into the belt;
5. use it at the intended world interaction.

Failures are design evidence, never blamed on the child.

---

## 9. Performance and technical budgets

Lock budget patterns before content multiplication.

- no independent per-plant `Update()` loop;
- use material, shader, blendshape, shared scheduler or bounded animation ownership for growth;
- pool produced objects and seed props;
- plots and machines receive manifest-style object and effect caps;
- narration clip memory participates in the audio budget;
- ship garden receives a dedicated performance-budget row when the ship interior exists;
- sprites/helpers remain parked because they multiply AI, animation, objects and save state.

Representative future soak target:

- 20 active plots;
- three active processing machines;
- normal catalog and narration usage;
- stable memory and 72 Hz device behavior.

The soak target is a stress envelope, not a requirement that the early garden expose 20 plots.

---

## 10. Machines and conveyors

### Keep now

- cultivation plot;
- existing BioRefiner/extractor role;
- existing Assembler role;
- manual processing of the first mastered inputs;
- visible machine identity, icon, spoken name/function and tier requirement.

### Keep later

- additional machine tiers;
- conveyor routing of mastered recipes;
- bulk processing;
- automated sorting;
- alternate production lines;
- mine-to-refiner-to-assembler chains.

Conveyor planning begins after W002 proves that the manual transformation loop is enjoyable and understandable. The conveyor system automates known recipes; it does not become a second discovery interface.

---

## 11. Sprites and living helpers

Park sprites as gameplay systems for v1.

Allowed later:

- ambient visual life;
- cosmetic reactions;
- non-persistent presentation companions;
- garden movement that makes the room feel alive.

A functional helper system—with AI, jobs, boosts, collection, persistence or breeding effects—requires a separate cost/benefit review after the core invention loop and Forge creature pipeline are proven.

---

## 12. Multiplayer and rotating content

Campaign laws:

- deterministic;
- offline complete;
- no required limited-time recipes;
- no fear-of-missing-out progression;
- no campaign-save dependency on rotation state.

Park online seed rotations until online multiplayer actually exists and its release/data/operations model is approved.

A later safe freshness option is a local deterministic date-hash variation:

- computed on device;
- optional variety only;
- no backend;
- rewards never permanently expire;
- nothing required becomes unavailable.

---

## 13. Keep now / keep later / cut

### KEEP NOW — post-M0 W000/W001 band

- invention-economy thesis;
- six-family functional taxonomy;
- verb-based trait language;
- universal deterministic recipe extension;
- three-input physical cap;
- discovery states and silhouette cards;
- project-wide accessible label metadata;
- captions plus stable narration IDs;
- prerecorded/batch-generated name and function clips as the shipping path;
- W000 planter beat;
- W001 seed surfacing and first useful invention;
- three-slot belt rule;
- atomic commit and no-pressure cultivation;
- recipe/save/catalog/accessibility/performance gates.

### KEEP EARLY — W002 or immediately after proof

- adjacency crossing as player-facing play;
- authored hybrid unlocks;
- first full-belt choice;
- first mid-world swap station where justified;
- combined bio/industrial invention;
- optional visible rarity/pity;
- extractor and assembler progression.

### KEEP LATER

- conveyor automation of mastered recipes;
- larger processing chains;
- deeper genetics and giant-crop goals;
- expanded invention board;
- planet-signature catalysts;
- cross-world advanced recipes;
- ambient sprite-like presentation;
- local date-hash optional rotations;
- heavy/multi-slot gear only after device evidence supports added complexity.

### CUT OR PARK

- 8+2+1 physical insertion recipes;
- blanket requirement that every valuable item uses both paths;
- runtime-generated species;
- second inventory/crafting/economy/machine systems;
- runtime on-device TTS;
- automatic narration of complete recipe lists;
- scrolling catalog lists in v1;
- functional sprite/helper system in v1;
- online seed rotations before online release;
- required RNG progression;
- cultivation failures caused by absence or headset interruption.

---

## 14. Top failure tests

The design is disproven or requires major correction if any of these occur.

1. **Comprehension failure**
   - a child cannot identify the role sockets, understand a trait or repeat a known recipe after one guided demonstration.

2. **Chore failure**
   - the physical recipe interaction becomes repetitive, tiring or slower than the reward justifies.

3. **Economy failure**
   - gardening replaces salvage/jobs as the dominant required progression path, or combined recipes feel like arbitrary double-gating.

4. **Loadout failure**
   - three slots create frequent mandatory backtracking rather than interesting preparation.

5. **Architecture failure**
   - W002 requires generic runtime code or introduces another owner for existing data.

6. **Narration failure**
   - focus movement produces chatter, overlaps RILL/safety cues, or creates an unsustainable clip-production burden.

7. **Save/interruption failure**
   - doff, overlay, quit, travel or controller loss consumes ingredients twice, loses inputs or corrupts growth state.

8. **Performance failure**
   - active plots, machines, props or narration break the Quest frame/memory envelope.

9. **Pacing failure**
   - garden sessions interrupt story/world momentum or routinely exceed the short departure/return bookend role.

10. **Reward failure**
    - inventions primarily change numbers instead of unlocking memorable new actions or approaches.

---

## 15. Decisions resolved versus remaining

### Resolved by review consensus

- adopt the invention-economy thesis;
- extend current owners;
- use one deterministic recipe family;
- keep genetics as the only randomness;
- cap v1 physical recipes at three role-based inputs;
- lock v1 field carrying to three one-item sockets;
- use physical cards/sockets, not scrolling lists;
- use text-authoritative accessible labels with prerecorded/batch-generated shipping speech and caption fallback;
- defer adjacency depth to W002;
- defer conveyors until the manual loop is proven;
- park functional sprites and online rotation;
- keep campaign offline and deterministic.

### Remaining Terry approval points

These are recommendations ready for approval, not unresolved technical unknowns:

1. Approve the W000 minimum as planter + one future silhouette only.
2. Approve the W001 climax as the first useful deterministic invention after returning to the ship.
3. Approve the initial six-family functional taxonomy and verb-first naming direction.
4. Approve three one-item belt sockets for v1, with heavy/multi-slot rules deferred.
5. Approve prerecorded or batch-generated narration clips as the shipping path, with captions available first.
6. Approve W002 as the first adjacency/breeding lesson and the zero-new-generic-C# architecture gate.

If Terry approves the reconciled direction, the plan remains queued behind M0. The first implementation work is Slice A only after the explicit freeze lift.

---

## 16. Definition of success

The growing/invention loop succeeds when a child can:

1. discover a strange resource in a world;
2. understand its simple functional trait;
3. see something desirable it can help create;
4. return to the ship and complete a short physical transformation;
5. choose the invention as one of three carried items;
6. use it to approach a world differently;
7. later automate only the repetitive transformations already understood.

The framework succeeds when W002 adds its seeds, recipes and invention payoff entirely through existing data and authors, with no new generic runtime code and substantially less production effort than W001.