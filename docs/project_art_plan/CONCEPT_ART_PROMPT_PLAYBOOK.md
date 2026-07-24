# ZIPTIDE CONCEPT-ART PROMPT PLAYBOOK

**Status:** canonical prompt-authoring and concept-selection process; docs-only and freeze-compatible.

**Purpose:** turn image generation from one-off inspiration into a repeatable production-design process. A useful ZIPTIDE concept prompt must produce art that communicates gameplay function, scale, interaction, world identity, and build constraints—not merely an attractive science-fiction image.

**Relationship to the asset pipeline:** this document governs concept generation and keeper selection before the existing `CONCEPT_TO_BUILT_PIPELINE.md` measured-spec step. It does not authorize runtime, scene, prefab, imported-asset, or APK changes.

---

## 1. The production thesis

A concept image is valuable only when another operator can use it to make a correct design decision or build a faithful asset.

The successful loop is:

1. define the gameplay and visual contract;
2. generate broad silhouettes or compositions;
3. select the strongest structural direction;
4. refine only named failures while preserving approved features;
5. approve a keeper;
6. extract a measured visual spec;
7. route the approved asset through Forge or licensed intake;
8. compare built output against the keeper;
9. close only after a headset verdict where device judgment is required.

The prompt is therefore a compact design specification, not a mood-board request.

---

## 2. Prompt anatomy

Every production prompt should answer the following fields. Omit a field only when it truly does not apply.

### 2.1 Subject and asset role

State exactly what is being designed.

Examples:

- palm-sized seed puck;
- three-stage alien plant species sheet;
- one-slot belt utility device;
- child-reachable physical invention station;
- W001 discovery scene;
- departure/return state pair;
- six-module building-kit sheet.

Avoid broad subjects such as `alien technology`, `cool plant`, or `sci-fi machine`.

### 2.2 Gameplay function

Describe what the object enables in play.

Examples:

- filters low-grade toxic particles;
- produces a detachable crafting organ;
- teaches the first deterministic recipe;
- lets the player identify and seat two ingredient classes;
- opens an optional contaminated route;
- communicates that the ship changed while the player was away.

Function is a visual-design constraint. The resulting object should suggest its purpose before the player reads text.

### 2.3 Scale and ergonomics

Provide physical scale and use context.

Examples:

- palm-sized beside a gloved hand;
- waist-high ship cultivar;
- human-height wild plant;
- mounted in one physical belt socket;
- reachable by a seated child;
- operated with a large VR-friendly latch;
- face-distance item requiring readable materials and silhouette.

A concept without scale is not yet build-ready.

### 2.4 Family or faction grammar

State the reusable visual language that makes the asset belong to its family.

A grammar may include:

- dominant silhouette rule;
- repeated anatomy;
- material families;
- construction language;
- emissive language;
- icon language;
- wear and repair language;
- relationship between organic and industrial components.

For example, the approved Filter-family direction uses:

- muted moss-green external tissue;
- warm off-white porous membranes;
- honeycomb filtering structures;
- restrained cyan biological activity;
- square salvage frames paired with round biological cartridges;
- worn industrial metal and visible repair seams.

Do not reuse a family grammar merely because it looks attractive. A new family must share ZIPTIDE’s world language while keeping a distinct functional silhouette.

### 2.5 World and emotional context

Name the location, culture, or emotional purpose when relevant.

Examples:

- Toxic City on The Moss;
- wet industrial canal route;
- warm lived-in grow room aboard a salvage ship;
- monumental dark cistern with one daylight shaft;
- first moment of useful discovery;
- quiet return-home payoff.

This prevents generic products that could belong to any franchise.

### 2.6 Presentation format

Specify how the concept should be shown.

Useful formats:

- silhouette sheet;
- front/side/back/open views;
- growth stages on one neutral sheet;
- player-eye interaction view;
- before/after pair using the same camera and environment;
- environment establishing image;
- modular kit elevation sheet;
- component breakdown with no dependency on generated labels.

The presentation format should match the decision being made.

### 2.7 Protected features

When refining an existing result, list the approved features that must remain unchanged.

Examples:

- preserve exact silhouette and proportions;
- preserve two-plot layout and camera angle;
- preserve square/round/triangle socket grammar;
- preserve muted palette and honeycomb anatomy;
- preserve compact one-slot equipment scale.

This is the main defense against image-to-image drift.

### 2.8 Negative constraints

Name the most likely wrong directions.

Examples:

- no ordinary Earth vegetables;
- no pristine white laboratory furniture;
- no fantasy-magic glow;
- no military weapon styling;
- no full-face mask or chest harness;
- no generic corporate logo;
- no unreadable decorative greebles;
- no text-dependent interaction;
- no random second species on the sheet;
- no watermark.

Negative constraints should target probable model drift, not become an indiscriminate list.

---

## 3. Canonical structured prompt template

Use this internal blueprint before writing the final natural-language prompt.

```text
SUBJECT:

GAMEPLAY FUNCTION:

PLAYER MOMENT:

SCALE / ERGONOMICS:

DOMINANT SILHOUETTE:

FAMILY OR FACTION GRAMMAR:

MATERIAL / COLOR LANGUAGE:

WORLD / EMOTIONAL CONTEXT:

PRESENTATION FORMAT:

PROTECTED FEATURES:

VARIABLES TO EXPLORE:

AVOID:
```

The final generator prompt may be prose, but every line above should have an intentional answer.

---

## 4. ZIPTIDE-wide visual constants

These are defaults, not excuses to make every object identical.

### 4.1 World tone

- used-future 1970s science-fiction;
- grounded material believability;
- salvage-built human technology;
- monumental and precise Architect technology;
- smooth sterile Warden technology;
- family-readable silhouettes;
- cinematic composition without sacrificing game readability;
- practical wear, repair, and construction logic;
- restrained use of emissive color.

### 4.2 Color-language anchors

- tide / active crest: cyan;
- human practical and lantern warmth: amber;
- Architect channels and seals: deep amber where canon requires;
- Wardens: sterile white;
- biological families: world-authored body colors with restrained state emissives.

Do not flood every object with cyan merely to make it look science-fictional.

### 4.3 Shape-language anchors

- one dominant silhouette feature;
- secondary details must claim function;
- readable at Quest draw distance;
- readable at hand or face distance when held;
- a child should be able to describe or sketch the object after one look;
- controls and sockets use shape as well as color and text.

### 4.4 Interaction anchors

- important interaction surfaces remain reachable in seated and child tests;
- physical slots visibly match their accepted category;
- commit actions are deliberate;
- cancel or return behavior is obvious;
- state changes do not rely only on color;
- UI art should support spoken names and captions rather than requiring fluent reading.

---

## 5. Prompt archetypes

### 5.1 Species lifecycle sheet

Use for plants, creatures, fungi, biological materials, and some living machines.

Required content:

- seed/egg/source form when applicable;
- early, middle, and mature forms;
- shared anatomy across every stage;
- wild and cultivated scale when both exist;
- clearly detachable or usable output;
- one dominant functional tell;
- neutral production background unless habitat is the decision.

Failure signs:

- each stage looks like a different species;
- ordinary Earth anatomy replaces the stated alien grammar;
- mature form adds decorative structures unrelated to function;
- scale is unclear;
- the harvested component cannot be identified.

### 5.2 Held prop or equipment sheet

Use for tools, weapons, ability modules, and wearable devices.

Required content:

- exact slot or mounting role;
- front, side, in-use, and service/open view;
- hand or body scale;
- dominant input/output component;
- VR-friendly latch, grip, or control;
- clear state language;
- silhouette distinct from unrelated equipment classes.

Failure signs:

- item expands into a chest harness or backpack when one-slot was requested;
- service parts are too small for VR;
- function is communicated only by generated labels;
- generic modern product styling replaces ZIPTIDE construction language.

### 5.3 Machine or station interaction sheet

Use for crafting, refining, garden, mission, and ship controls.

Required content:

- player-eye view;
- all input and output surfaces;
- category shape grammar;
- deliberate commit control;
- cancel/return control where resources can be spent;
- locked, ready, active, complete, and fault states where relevant;
- seated/child reach intent;
- no scrolling-list dependency in physical-first systems.

Failure signs:

- attractive machine with no understandable interaction sequence;
- tiny controls;
- identical sockets for different inputs;
- catalyst or future features visible before they are taught;
- text is required to know what to do.

### 5.4 Discovery or gameplay-context scene

Use to prove whether the player will notice and understand an asset inside a world.

Required content:

- location and route role;
- player or camera scale;
- natural placement rather than isolated pedestal presentation;
- environmental evidence of function;
- one clear visual hierarchy;
- surrounding clutter below the target’s readability threshold.

Failure signs:

- target glows like a quest marker rather than belonging to the ecology;
- environment conflicts with established world canon;
- target is invisible without arrows or UI;
- composition is a poster rather than a playable route.

### 5.5 State-pair concept

Use for growth, repair, unlocking, contamination, ship changes, and story consequences.

Required content:

- same camera, object, scale, and environment;
- state A and state B labels may be added in documentation, but visuals must communicate the change without them;
- list exact allowed differences;
- forbid unrelated redesign between states.

Common pairs:

- departure / return;
- broken / repaired;
- dormant / active;
- unknown / identified;
- empty / cultivated;
- locked / mastered;
- calm / contaminated.

### 5.6 Modular kit sheet

Use for buildings, ship rooms, machine families, and repeated props.

Required content:

- module count and roles;
- shared construction grammar;
- base/middle/cap or equivalent assembly logic;
- socket locations;
- material split;
- scale reference;
- deliberate variation axes;
- no unique hero clutter masquerading as reusable kit content.

---

## 6. The three-pass generation method

### Pass 1 — breadth

Goal: decide silhouette or composition, not detail.

- generate several distinct options;
- vary only the named axes;
- keep world grammar and function constant;
- judge at thumbnail size first;
- reject attractive outputs with unreadable function.

### Pass 2 — convergence

Goal: combine one approved structure with corrected anatomy, materials, and ergonomics.

- name the chosen keeper candidate;
- preserve its approved features explicitly;
- correct only the largest failures;
- keep palette, camera, and proportions stable where already approved;
- do not add new features merely because the image has empty space.

### Pass 3 — production sheet

Goal: provide sufficient reference for measured specification and build intake.

- add scale references;
- add orthographic or open/service views;
- show state or growth progression;
- show detachable components;
- remove invented names and corrupted generated text;
- ensure the sheet communicates through shapes even if labels are discarded.

---

## 7. Delta-only refinement law

After a promising result exists, do not rewrite the prompt from scratch.

Every refinement request should contain:

1. **Preserve** — exact features that are already approved.
2. **Change** — the few named failures.
3. **Do not add** — likely drift introduced by the correction.
4. **Output format** — whether the same composition, sheet, or state pair is required.

Example:

```text
Preserve the exact compact belt-mounted silhouette, worn salvage frame, round cartridge,
front/side/open views, and current proportions. Replace only the generic shield logo with
an original layered-filter symbol. Make the cartridge visibly biological with off-white
honeycomb tissue and a moss-green rim. Add one large VR latch and a mechanical clean/blocked
indicator. Do not add a mask, chest harness, backpack, weapon grip, or extra screens.
```

This law produced a more coherent Filter family than repeated fresh generations.

---

## 8. Keeper-selection rubric

Score each candidate from 0 to 2 on every row.

| Criterion | 0 | 1 | 2 |
|---|---|---|---|
| Gameplay function | unclear | partly suggested | readable without explanation |
| Silhouette | generic/noisy | recognizable near | recognizable near and far |
| Family consistency | unrelated | partial grammar | clearly same family |
| ZIPTIDE world fit | generic franchise | some anchors | unmistakably ZIPTIDE |
| Scale/ergonomics | absent/impossible | approximate | explicit and usable |
| Material logic | decorative | partly functional | construction/anatomy make sense |
| State readability | ambiguous | needs labels | visible without labels |
| Child readability | text-dependent | understandable with help | nameable and operable |
| Build usefulness | mood only | partial reference | measured-spec ready |
| Drift control | major violations | minor violations | negatives respected |

A keeper should normally score at least 16/20 and have no zero in Gameplay Function, Scale/Ergonomics, or Build Usefulness.

An image may still be retained as an **exploration reference** when it contains one strong idea but fails keeper status.

---

## 9. Filter-family case study

The Filter experiment established the following lessons.

### 9.1 What improved the results

- changing `alien seed` into `palm-sized seed puck with a VR socket notch`;
- showing seed, sprout, juvenile, mature wild plant, and smaller ship cultivar together;
- repeating honeycomb anatomy through every growth stage;
- defining detachable Filter Organs;
- pairing square industrial frames with round biological cartridges;
- specifying one-slot belt scale;
- asking for open-cartridge and mounted views;
- using departure/return pairs for the grow tray;
- defining square, round, and triangular socket categories at the invention station;
- covering the untaught catalyst socket;
- replacing text dependence with icons and geometry.

### 9.2 Drift observed and corrected

- ornate seed became an artifact or egg rather than an introductory seed;
- ordinary houseplant leaves replaced alien filtering membranes;
- bright cyan made the discovery plant look like a quest marker;
- chest-mounted respirator became larger than a one-slot item;
- pristine countertop garden conflicted with the salvage ship;
- generic shield/lung logo conflicted with ZIPTIDE glyph language;
- tan foam cartridge lost the living biological identity;
- generated labels were corrupted and could not carry interaction meaning.

### 9.3 Resulting design grammar

- seed: palm-sized, notched, porous puck;
- wild plant: approximately human-height;
- ship cultivar: waist-high;
- harvest: detachable fist-sized porous organ;
- device: compact one-slot square frame with round living cartridge;
- grow station: two child-reachable plots, worn metal, amber practical light;
- invention station: square frame slot, round biological slot, covered triangular catalyst slot;
- state language: restrained cyan plus mechanical/shape confirmation.

This is a case study, not a requirement that later plant families copy Filter anatomy.

---

## 10. Prompt experiment record

Every serious concept experiment should record:

```yaml
experiment_id: prompt-test-<number>
asset_or_family_id: <stable proposed id>
date: <YYYY-MM-DD>
generator: <service/model/version when known>
mode: text-to-image | image-to-image
source_images: [<ids or paths>]
structured_brief:
  subject: ...
  gameplay_function: ...
  scale_ergonomics: ...
  grammar: ...
  presentation: ...
  avoid: ...
final_prompt: |
  ...
outputs:
  - output_id: ...
    verdict: reject | exploration-reference | refine | keeper
    strengths: [...]
    failures: [...]
refinement_prompt: |
  ...
keeper_id: <optional>
keeper_status: proposed | Terry-approved | superseded
feeds:
  - measured visual spec
  - recipe/intake plan
```

Store prompt provenance alongside the keeper before paid 3D generation or production intake.

---

## 11. Prompt-library design for the future meta-pipeline

The long-term reusable system should separate:

### 11.1 Stable project grammar

- style canon;
- color language;
- faction and family grammar;
- interaction/accessibility law;
- asset tier and budget class;
- presentation formats;
- banned drift patterns.

### 11.2 Per-asset structured brief

- role;
- gameplay function;
- scale;
- world placement;
- dominant feature;
- required states;
- sockets and detachable parts;
- variation axes;
- negatives.

### 11.3 Generator adapter

A future tool can translate the structured brief into service-specific prompts while preserving the same authority. The adapter may change phrasing for Nano Banana, another image model, or a future local model, but it may not silently change gameplay function, scale, protected features, or negatives.

### 11.4 Evaluation and learning

Every accepted/rejected result should improve:

- keeper phrases that consistently work;
- negative clauses that prevent drift;
- generator-specific weaknesses;
- reference-image use;
- prompt length and ordering;
- class-specific presentation templates.

The goal is not to claim that prompts automatically produce final art. The goal is to make concept generation increasingly predictable, comparable, and useful to downstream production.

---

## 12. Hard boundaries

- Concept images are references, not automatically shippable assets.
- Generated text and logos are never trusted as canon.
- A visually excellent image does not override gameplay, story, accessibility, ownership, or performance authorities.
- No runtime system is implemented merely because a concept depicts it.
- Paid generation requires current license and commercial-use verification plus provenance capture.
- A concept becomes canonical only through explicit approval and repository recording.
- A built asset closes only through the existing comparison, gates, and device-verdict process.
