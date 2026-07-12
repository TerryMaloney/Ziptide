# CINEMATIC PRESENCE CONSTITUTION

**FORGE IV CP-0 — COMPLETE · Terry-approved 2026-07-11**  
**Companion plan:** `FORGE_IV_CINEMATIC_PRESENCE.md`  
**Purpose:** the decision-complete standard future operators must use before implementing cinematic art, worlds, skies, atmosphere, composition or score.

## The promise

Ziptide should feel like a movie the player is physically standing inside.

The player should believe:

- the air has depth;
- objects have mass, history and material identity;
- the horizon is truly far away;
- planets and structures are enormous rather than painted onto a wall;
- creatures inhabit the place rather than waiting as game pieces;
- sound comes from a world beyond the current objective;
- quiet and scale are as important as action;
- every spectacular moment remains comfortable and smooth in VR.

The game does not need technical perfection to keep this promise. It must not look accidental, flat, weightless, inconsistent or visibly unfinished.

## The nine constitutional laws

### Law 1 — Perceived reality beats raw complexity

Spend geometry, texture, lighting, animation and effects where the player can perceive them. Use atmosphere, silhouette, occlusion, audio and controlled motion to sell the rest.

A distant mountain should be simple and hazy because a distant mountain is simple and hazy to the eye.

### Law 2 — Stable VR performance is part of visual quality

A cinematic effect that causes frame instability, discomfort or input latency is not cinematic. It is broken.

The target is stable 72 Hz on the standalone Quest floor. The 13.9 ms frame interval is a hard physical reality. Systems should leave headroom for gameplay, not pass only in an empty showcase.

### Law 3 — Every object belongs to one fidelity band

No asset ships without knowing whether it is:

- P0 Hero;
- P1 Interaction;
- P2 World;
- P3 Vista;
- P4 Atmosphere.

The band determines its close-read standard, LOD path, update rate, shadow eligibility and proof artifact.

### Law 4 — One physical world, not a collection of assets

Materials, wear, lighting, fog, water, signs, sound and movement must agree about the place.

A beautiful weapon in a bare box is not a cinematic world. A beautiful sky that does not affect the fog or ground is not a cinematic sky.

### Law 5 — Depth requires layers

A signature view needs:

- near framing;
- midground activity;
- far anchor;
- celestial/atmospheric scale;
- audio depth.

At least two visible layers must move at different rates. Distance must reduce contrast and saturation. A giant object must be partially occluded or atmospherically embedded.

### Law 6 — Lighting is hierarchy, not quantity

Every place has one dominant environmental light and one dominant color relationship. Practical lights create local human-scale rhythm. Real-time lights and shadows are rare, budgeted hero tools.

Many weak lights do not equal realism.

### Law 7 — The world responds

Touch, shots, wind, water, creatures, machinery and story changes should have visible and audible consequences. Responses reuse existing owners and budgeted vocabularies; they do not create a second combat, audio, haptic or world-state system.

### Law 8 — Silence and restraint are authored

Not every moment needs music, particles, dialogue or motion. Awe needs contrast. The sound of wind, suit movement, distant machinery or water may carry a scene better than another music layer.

### Law 9 — Quality must be reviewable and irreversible

Every meaningful cinematic change needs an inspectable artifact, budget evidence and a verdict. Once a pilot world is honestly clean and headset-approved, its ratchet may become blocking.

No wholesale whitelist may counterfeit completion.

---

## Fidelity-band contract

### P0 — Hero

Examples: held weapon/tool, scanner, helm control, story object, primary creature, close character.

Required:

- unmistakable silhouette from intended view;
- clean close geometry within existing class budget;
- coherent albedo/normal/MSA/emissive response;
- purposeful wear and narrative markings;
- interaction/grip landmarks;
- contact/grounding and appropriate sound;
- LOD and/or culling path when it can leave close range;
- turnarounds or creature state contact sheet.

Forbidden by default:

- 4K texture;
- multiple unique materials for invisible detail;
- permanent dynamic shadow light;
- detail that disappears at normal headset resolution.

### P1 — Interaction

Examples: doors, nearby machinery, lamps, crates, room vegetation, ordinary nearby creatures.

Required:

- readable function and state;
- material-family consistency;
- target/contact grounding;
- one visible environmental response where appropriate;
- LOD1 or reduced update path beyond the interaction bubble.

### P2 — World

Examples: building kit, streets, forest clusters, industrial runs, terrain dressing.

Required:

- shared atlas/material family;
- macro variation;
- cluster/HLOD path;
- derived/baked lighting;
- no individual dynamic shadow by default;
- provenance compatible with the conformance ratchet.

### P3 — Vista

Examples: mountain, distant city, giant wreck, crater wall, orbital structure.

Required:

- strong silhouette;
- low-frequency material/color read;
- atmosphere and scale reference;
- impostor/low-poly representation;
- no collision, gameplay logic or full-rate animation unless separately justified.

### P4 — Atmosphere

Examples: haze, spores, rain, dust, fog bands, cloud cards, distant moving lights.

Required:

- at least two motion rates in signature scenes;
- strict screen-coverage and live-count limits;
- no invisible full-world simulation;
- color relationship to sky, ground and audio bed.

---

## Existing hard ceilings preserved

These values are inherited from the live Forge systems and remain hard until evidence changes them:

| Class/system | Ceiling or law |
|---|---|
| Ship/hull hero | 15,000 triangles |
| Creature hero | 10,000 triangles |
| Handheld hero | 6,000 triangles |
| General prop | 3,000 triangles |
| Plant | 1,500 triangles |
| Normal Forge asset | one primary material; focal/emissive exception documented |
| Skyscape | dome + ≤3 bodies; ≤4 draw calls; ≤5 materials; ~3k triangles |
| Practical lighting | one directional key + ≤2 no-shadow hero point lights |
| VFX | ≤64 particles/system; ≤6 live systems |
| Water | one material/body; ≤8k triangles/body; no reflection/refraction/tessellation |
| Scene/prefab editing | author/patcher only; no hand-edited YAML |

### LOD reduction targets

- LOD1: ≤45% of LOD0 triangles.
- LOD2: ≤15% of LOD0 triangles.
- Vista/impostor: silhouette-first shell or ≤2 crossed cards where possible.
- Runtime swaps require hysteresis.
- Hidden or distant ambience does not retain full-rate animation/AI merely for decoration.

### Provisional transparency limits

Until device measurement tightens them:

- sustained transparent coverage target: ≤20% of the view;
- brief authored reveal target: ≤35%;
- exceeding either requires a measured headset proof, not a desktop screenshot.

---

## The Prospect Depth Rubric

Score every signature skyscape/reveal 0 or 1 on each item. A signature moment requires **8/9 minimum**, with items 1–5 mandatory.

1. **Near occluder:** a foreground object overlaps the sky.
2. **Midground parallax:** a readable silhouette/activity layer sits between player and horizon.
3. **Far anchor:** a distant mass has reduced contrast and remains compositionally clear.
4. **Embedded celestial scale:** planet/rings/Shell/cloud wall is partly hidden by haze, cloud, terrain or structure.
5. **Ground connection:** sky color influences fog, ambient light or ground response.
6. **Motion separation:** at least two layers move at visibly different rates.
7. **Scale anchor:** familiar object/creature/ship/light establishes size.
8. **Audio distance:** one audible layer exists beyond the immediate gameplay bubble.
9. **No flat-card tell:** no obvious border, perfectly clean pasted planet, identical-speed layers or texture-wall read.

A beautiful color gradient alone scores poorly. Depth is the standard.

---

## Awe Node contract

An Awe Node is a spatially authored opportunity for wonder. It never moves the camera or rotates the player.

Every node records:

- unique id and scene;
- purpose: arrival, crest, threshold, quiet, threat or return;
- player anchor and safe look cone;
- foreground frame;
- midground activity;
- far anchor;
- celestial/atmospheric layer;
- scale reference;
- motion layers;
- local/distant audio layers;
- optional score cue;
- temporary light/VFX allowance;
- performance tier;
- proof views;
- exit/release condition.

### Awe Node pass criteria

- understandable without forcing the player to face one exact pixel;
- depth rubric passes;
- no route obstruction or locomotion ownership;
- temporary hero spend releases after the moment;
- remains legible during gameplay rather than only from a debug camera;
- headset verdict includes awe/readability and frame stability.

---

## Lived-in material standard

A material family is incomplete until it can express believable history.

Standard masks/channels should support:

- edge and grip wear;
- dirt accumulation;
- upward-facing dust;
- floor/wall grime gradient;
- rain/drip streaks;
- damp/waterline variation;
- heat damage;
- oxidation;
- moss/spore/biological growth;
- repair patch/mismatched panel;
- printed serial/glyph/narrative marking;
- controlled emissive focal.

Wear follows use and gravity. Random noise spread evenly across an asset does not pass.

---

## Cinematic light standard

Every signature space declares:

- dominant key direction/color;
- ambient/fill relationship;
- practical-light rhythm;
- focal contrast;
- darkness floor needed for navigation/comfort;
- hero-light allowance;
- grounding method;
- device checkpoint.

Default hierarchy:

1. derived sky/key;
2. baked/ambient world response;
3. emissive practicals and fake pools/halos;
4. at most two no-shadow real hero lights;
5. temporary effect light only if separately budgeted and released.

No cinematic plan may depend on many shadowed real-time lights.

---

## Environmental motion standard

Every signature world needs at least three categories, with one category at each depth:

- **near:** dust, spores, rain, leaves, cables, cloth, water edge;
- **mid:** machinery, practical flicker, creatures, traffic, steam, vegetation waves;
- **far:** moving lights, cloud/storm drift, birds/ships, industrial pulses, celestial motion.

Motion should be desynchronized and restrained. Everything moving constantly produces noise, not life.

---

## Soundtrack and spatial-audio constitution

Every signature world has a `World Audio Identity` containing:

- tonal center/mode;
- emotional temperature;
- instrument/material palette;
- close suit/body layer;
- environment bed;
- distant world layer;
- exploration stem;
- discovery/awe stem;
- threat pulse;
- resolution/return stem;
- diegetic source layer;
- silence rules.

### Music laws

- no short obvious loop repeated indefinitely;
- no constant full arrangement during exploration;
- threat layers add to the world’s identity instead of replacing it with generic combat music;
- awe cues are sparse and tied to earned moments;
- environmental audio may carry a scene alone;
- return-home music can remember visited worlds or completed arcs;
- all transitions reuse the existing single audio owner;
- headset spatial mix is the final judge.

### Audio proof

- stem transition test;
- memory/release evidence;
- desktop mix preview;
- in-headset spatial verdict;
- quiet-scene test proving silence does not feel like missing audio.

---

## Review-artifact law

A future model must be able to see the improvement.

### Asset packet

- front/side/back/three-quarter turnarounds;
- material x-ray;
- silhouette view;
- budget card;
- LOD comparison;
- written verdict and revision.

### Creature packet

- front/side/back;
- idle/alert/telegraph/counter/disabled;
- material and eye/tell close-up;
- body/ground contact view;
- budget/performance card;
- written verdict.

### World packet

- arrival/player view;
- interaction view;
- route/midground view;
- signature vista;
- practical/night view;
- effects/atmosphere view;
- depth-layer breakdown;
- performance overlay;
- written verdict.

A compile log cannot replace visual proof.

---

## W002 and W001 proof standard

### W002 — technical proof

Must demonstrate:

- zero honest `ART_UNCONFORMED` findings before lock;
- complete provenance;
- LOD/fidelity packages;
- shared material families and macro variation;
- no unbudgeted lights/effects;
- repeatable world review packet;
- stable headset performance.

### W001 — emotional proof

Must demonstrate:

- complete first-hour visual continuity;
- Prospect-depth sky/air;
- believable toxic water and shoreline response;
- lived-in shipyard/city materials;
- readable signature creature;
- at least three Awe Nodes: arrival, first scale reveal and return/payoff;
- environmental audio identity and first score proof;
- stable performance during exploration and combat.

Neither world alone proves FORGE IV.

---

## Ratchet and promotion law

A quality warning may become a blocker only when:

1. the pilot content reaches zero honestly;
2. Terry approves the headset result;
3. all exceptions are narrow and carry a written WHY;
4. a regression test proves the warning cannot be hidden wholesale;
5. the content has a downgrade/fallback path for the Quest floor.

Visual quality is allowed to improve. It is not allowed to silently regress.

---

## CP-0 closure checklist

- [x] Cinematic realism defined.
- [x] Quest/performance constraints stated.
- [x] Fidelity bands locked.
- [x] Existing class/system ceilings preserved.
- [x] LOD targets established.
- [x] Prospect Depth Rubric established.
- [x] Awe Node semantic contract established.
- [x] Lived-in material standard established.
- [x] Lighting and environmental-motion standards established.
- [x] Soundtrack/spatial-audio contract established.
- [x] Review artifacts established.
- [x] W002 technical proof and W001 emotional proof established.
- [x] Promotion/ratchet law established.

## Next envelope

After Forge III closes and Terry completes the required device pass, begin **CP-1 — Cinematic Review Artifacts**. Do not begin by adding expensive rendering features. First make world, creature and Awe Node improvement visible to the same review loop that successfully improved the Forge creatures.
