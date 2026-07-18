# FORGE IV — CINEMATIC PRESENCE

**Canonical plan of record — Terry-approved 2026-07-11**  
**Status:** 🟡 PLANNED · **CP-0 CINEMATIC CONSTITUTION COMPLETE** · runtime implementation not started  
**Performance floor:** standalone Meta Quest / stable 72 Hz  
**North star:** a movie-quality sense of physical presence, scale and awe—achieved through perceptual discipline, not brute-force rendering.

> Ziptide does not need every polygon a film renderer could produce. It needs every visible layer to
> agree that the player is standing inside one coherent physical world. Detail is concentrated where
> the player can perceive and touch it; distance, air, sound, lighting, motion and composition do the
> rest.

## 0. Authority and relationship to existing plans

This plan is the canonical next Forge generation after the current programs:

1. Finish the remaining **FORGE III — Cohesion Leap** work and its device pass.
2. Continue the creature-specific **`CREATURE_QUALITY_V2_LIFE_LEAP.md`** program under Picasso.
3. Begin FORGE IV implementation only through the staged envelopes below.

The former narrow “FORGE IV candidate: Diegetic UI Art” in `FORGE_III_PLAN.md` is **not deleted**. It is retained as **CP-9 — Diegetic Interface Art**, because cinematic presence includes the surfaces the player touches constantly, but that envelope requires a Terry-approved cross-lane window.

This plan does not grant the art lane ownership of gameplay, travel, saves, combat, haptics, audio runtime, scene YAML or world-state logic. Cross-lane seams receive their own claim and owner before implementation.

## 1. What “cinematic realism” means in Ziptide

Cinematic realism is not maximum geometry or 4K textures everywhere. It means:

- near objects survive close VR inspection;
- materials respond coherently to light and carry believable wear;
- objects feel grounded, weighted and used;
- atmosphere creates measurable near/mid/far depth;
- the world has scale references and distant life;
- lighting has a clear hierarchy rather than many expensive lights;
- movement and sound imply a world larger than the active gameplay bubble;
- reveal points are spatially composed without taking control of the player’s head;
- the score supports wonder, danger and quiet instead of becoming constant wallpaper;
- the result remains comfortable and stable at the target frame rate.

A distant building should become simpler, lower-contrast and more atmospheric because that is how distance looks—not because the game failed to render it. The optimization and the realism should reinforce each other.

## 2. Hard truths and non-negotiable constraints

### The target platform cannot afford everywhere at once

FORGE IV must not normalize:

- full-resolution hero meshes at horizon distance;
- unique high-resolution textures on every object;
- many real-time shadowed lights;
- global true volumetrics;
- screen-space reflections or expensive refraction everywhere;
- dense transparent effects filling the view;
- constant real-time reflection updates;
- film-quality hair, cloth and facial animation on crowds;
- several heavy post-processing layers;
- thousands of individually rendered decorative objects;
- full-rate animation and AI for distant populations.

### Existing hard rails remain law

FORGE IV does not quietly raise the existing caps:

- Forge class budgets remain the hard geometry ceilings: hull 15k · creature 10k · handheld 6k · prop 3k · plant 1.5k triangles unless a separately reviewed class changes them.
- A normal Forge asset remains one primary material; explicit focal/emissive submesh exceptions must be documented.
- Skyscape baseline remains dome + ≤3 bodies, ≤4 draw calls, ≤5 materials and approximately 3k triangles.
- Practical light baseline remains one directional key plus at most two no-shadow hero point lights in a world.
- VFX remains ≤64 particles per system and ≤6 live systems until device evidence supports a different budget.
- Water remains reflection/refraction-free unless a later hand-written Quest shader proves its budget.
- No scene or prefab YAML is hand-edited.
- No visual feature is “done” without a photo/contact-sheet or headset checkpoint.

## 3. The perceptual fidelity bands

Every visible thing belongs to one band. The band determines geometry, materials, update rate, shadow eligibility and review standard.

| Band | Player relationship | Required treatment | Default simplification |
|---|---|---|---|
| **P0 Hero** | in hand, face-height, primary creature/character, story-critical machinery | strongest silhouette; full Forge material stack; close inspection; tactile wear; interaction landmarks; best available animation | existing class cap; one primary material; localized hero effects only |
| **P1 Interaction** | room-scale objects, nearby creatures, doors, props, vegetation beside the route | readable material family; grounded contact; state change; moderate geometry; audible response | LOD1 beyond interaction distance; reduced animation rate when not engaged |
| **P2 World** | buildings, streets, forests, terrain dressing, industrial systems | modular kit; shared atlases; macro variation; baked/derived lighting; clustered placement | HLOD/cluster; no individual shadows; simplified shader |
| **P3 Vista** | distant city, mountain, wreck, megastructure, forest wall | silhouette and atmospheric read first; low-frequency texture; recognizable scale anchor | impostor or low-poly silhouette; no interaction/physics; rare updates |
| **P4 Atmosphere** | air, haze, cloud, dust, spores, rain, distant traffic/light | layered motion at different speeds; depth and color extinction; spatial sound | sparse cards/particles; strict overdraw and screen-coverage limits |

### LOD law

Every P0–P2 asset family that can appear at multiple distances must have a declared reduction path:

- LOD0: close/hero form, never above the existing class ceiling.
- LOD1: target ≤45% of LOD0 triangles.
- LOD2: target ≤15% of LOD0 triangles.
- Vista/impostor: silhouette-first representation, normally ≤2 crossed cards or a very low-poly shell.
- Hysteresis is required on runtime swaps; no distance-boundary flicker.

These percentages are starting targets, not permission to create unnecessary LOD0 detail. Device evidence can ratchet them downward.

## 4. The cinematic world equation

A signature scene or reveal is not complete until all five layers exist:

1. **Near frame:** branch, doorway, cable, rock, machinery, visor edge or structure that establishes the player’s physical position.
2. **Midground activity:** route, people/creatures, lights, steam, water, traffic, machinery or vegetation with readable parallax.
3. **Far anchor:** mountain, city mass, crater rim, wreck, facility or megastructure with reduced contrast and simplified detail.
4. **Celestial/atmospheric scale:** planet, rings, Shell structure, cloud wall, storm, gate scar or orbital debris, partially occluded rather than pasted cleanly onto the sky.
5. **Audio depth:** close body/suit sound, local environmental bed and one distant layer that implies space beyond the visible gameplay area.

A flat skybox behind a flat street does not pass, even when both textures are attractive.

## 5. FORGE IV envelopes

### CP-0 — CINEMATIC CONSTITUTION AND BUDGET MATRIX — ✅ COMPLETE

Plan the rules before adding systems:

- definition of cinematic realism;
- perceptual fidelity bands;
- existing hard rails and provisional scene budgets;
- Prospect-depth skyscape rubric;
- Awe Node contract;
- soundtrack/environmental-audio contract;
- W002 technical proof and W001 emotional proof;
- proof-artifact and promotion rules.

Source: `CINEMATIC_PRESENCE_CONSTITUTION.md`.

### CP-1 — CINEMATIC REVIEW ARTIFACTS

Extend the existing Forge photo loop from isolated turnarounds into reviewable world/creature evidence:

- hero turnarounds remain;
- creature contact sheet: front/side/back, idle/alert/counter/disabled, material x-ray, budget card;
- world contact sheet: arrival, route, interaction, vista, night/practical, effects and performance-overlay views;
- Awe Node contact sheet with foreground/midground/far/celestial labels;
- machine-readable verdict file recording pass/fail, critique and next revision.

**Why first:** models must be able to see the result before automating more quality.

### CP-2 — FIDELITY PACKAGE AUTHOR

Forge produces one complete game-ready package rather than one mesh:

- LOD0/1/2 and vista/impostor form;
- collision proxy;
- material/texture assignment;
- wear, dirt, dampness, heat, biological-growth and narrative-mark masks;
- grip/interaction landmarks where relevant;
- provenance, budget and content hash;
- review subject registration.

The package author must preserve old assets by default and be create-only/idempotent.

### CP-3 — MATERIAL INTELLIGENCE AND LIVED-IN SURFACES

Canonical material families:

- painted/raw/oxidized metal;
- polymer/rubber;
- visor/glass;
- dry/wet stone;
- cloth/insulation;
- organic shell/membrane;
- corroded industrial;
- contaminated/spore-covered;
- bioluminescent surfaces.

Standardized masks produce edge wear, finger wear, floor grime, rain streaks, dust, repair patches, heat damage, moss/growth, wetness and markings without new shaders per object. Material response and provenance are audited; “factory-clean by accident” becomes visible debt.

### CP-4 — PROSPECT DEPTH / ATMOSPHERE STACK

Upgrade SkyVista from an excellent sky asset into spatial depth:

- near-air particles/cards;
- foreground occluder vocabulary;
- midground silhouette fields;
- far-landscape/vista impostors;
- partially occluded celestial scale;
- contrast/saturation falloff with distance;
- multiple motion rates;
- sky color reaching ground and fog;
- distant audio silhouette layer.

No global heavy volumetrics. The system derives from existing vista/theme data wherever possible.

### CP-5 — AWE NODE AND CINEMATIC COMPOSITION SYSTEM

Awe Nodes are authored spatial opportunities, not forced cameras. Each node declares:

- arrival/crest/threshold/quiet/threat/return purpose;
- likely player position and safe look cone;
- near frame, midground action, far anchor and celestial layer;
- scale reference;
- ambient movement;
- sound cue and optional music stem;
- temporary hero-light/VFX allowance;
- performance tier and proof views;
- exit/release condition.

The author may place markers and review cameras, but never rotates the player’s head or takes locomotion ownership.

### CP-6 — CINEMATIC LIGHT, SURFACE RESPONSE AND VFX

Connect the systems that sell physical contact:

- dominant key/fill hierarchy from the existing Light Script;
- practical fixtures, halos and pools;
- localized contact shadows/grounding;
- surface-aware impacts and environmental reactions;
- material-specific water/dust/spark/steam responses;
- temporary Awe Node hero spend that releases on exit;
- transparent screen-coverage and live-effect budgets.

Cross-lane weapon/haptic/audio wiring requires named ownership. Art owns the visible response; existing gameplay owners remain authoritative.

### CP-7 — RUNTIME FIDELITY DIRECTOR

One policy component allocates quality according to attention and current health:

- distance and visibility;
- interaction/story importance;
- current CPU/GPU/frame health;
- device thermal state (long family sessions throttle; the director sheds quality gracefully
  before the OS does it crudely — added 2026-07-18, GAP_AUDIT fold);
- active creatures, VFX and lights;
- Awe Node state;
- occlusion and room/portal state.

It may select LOD, shadow eligibility, animation update frequency, particle density, distant activity, impostors and approved dynamic-resolution/foveation settings. It does not become a second gameplay, AI or world-state owner.

### CP-8 — AUDIO FORGE / CINEMATIC SCORE

Audio is a generated world identity, not a playlist added at the end. Every signature world defines:

- tonal center and emotional temperature;
- instrument/material palette;
- close body/suit layer;
- environmental bed;
- exploration stem;
- discovery/awe stem;
- threat pulse;
- resolution/return stem;
- diegetic source layer;
- intentional-silence rules.

The score should breathe. Wind, breathing, water, distant machinery and creatures are sometimes the music. Stem transitions reuse the single `AudioDirector` ownership and its memory discipline; no second music singleton.

### CP-9 — DIEGETIC INTERFACE ART

Retains the previous FORGE IV candidate:

- belt, helm, scanner, credits, lobby, ship controls and menus receive one coherent visual language;
- panel/trim/glyph/type assets are generated through Forge;
- interaction/readability targets remain owned by existing UI/runtime systems;
- requires a Terry-approved cross-lane window because these surfaces touch many gameplay owners.

### CP-10 — TWO-WORLD PROOF

Before broad rollout:

- **W002 = technical proof:** zero unconformed visuals, complete provenance, LOD/fidelity packages, budgets, stable performance and repeatable review artifacts.
- **W001 = emotional proof:** first-hour cinematic presence, Prospect-depth sky/air, water, creature identity, lived-in materials, Awe Nodes, spatial sound and score.

FORGE IV cannot be declared successful if W002 is technically clean but emotionally flat, or W001 is beautiful but unstable.

### CP-11 — ARCHETYPE ROLLOUT AND RATCHET

Roll out by world archetype rather than blindly world-by-world:

- toxic forest/wetlands;
- industrial/city;
- cavern/interior;
- water/coastal;
- void/space;
- signature story worlds.

Each archetype inherits a proven recipe and receives one unmistakable identity. Art conformance, performance and cinematic-presence gates ratchet only after a world is genuinely proven.

## 6. Prospect skyscape standard

Every signature vista must demonstrate:

- a near occluder crossing part of the sky;
- a mid-distance silhouette with visible parallax;
- a far horizon with reduced contrast/saturation;
- a celestial or atmospheric giant partly hidden by haze/cloud/terrain;
- at least two motion rates;
- ground/fog color visibly connected to the sky;
- a familiar scale reference;
- one distant sound layer;
- no obvious flat-card edge or clean pasted-on planet read.

This extends—not replaces—`docs/systems/SKYSCAPE_DESIGN.md`.

## 7. Awe Node proof format

Each node’s review packet must include:

```yaml
id: W001_arrival_canal_reveal
scene: W001_ToxicCity
purpose: arrival_reveal
player_anchor: marker_id
safe_look_cone_degrees: 110
foreground: [door_frame, hanging_cable]
midground: [canal_water, lantern_route, passing_creature]
far_anchor: toxic_tenement_tower
celestial_layer: banded_giant_partial
scale_anchor: shipyard_crane
motion_layers: [near_spores, water_ripple, distant_light_crawl]
audio_layers: [helmet_breath, canal_bed, distant_horn]
music_cue: w001_awe_arrival
hero_budget: { point_lights: 0, vfx_systems: 1 }
proof_views: [player, wide, depth_breakdown, performance]
```

The schema may later become a ScriptableObject, but CP-0 locks the semantic contract first.

## 8. Provisional scene-level cinematic budgets

These are starting targets for Quest device proof, not automatic permission to consume them:

- Target refresh: stable 72 Hz; total frame interval 13.9 ms.
- Aim for meaningful headroom rather than living at the edge; any Awe Node that only passes in an empty scene fails.
- One directional key; ≤2 no-shadow hero point lights; hero lights release outside their moment.
- VFX: ≤6 live systems, ≤64 particles/system; sustained transparent coverage target ≤20% of the view, brief authored reveal target ≤35%, both subject to device proof.
- World-space water: one material/body, no reflections/refraction/tessellation; split/occlude bodies rather than one giant expensive mesh.
- Materials: shared/atlased by family; no unique shader per prop.
- Textures: existing Forge atlas strategy remains default; 4K textures are prohibited. A 2K exception requires a named hero asset, memory evidence and an approved downgrade path.
- Distant populations: impostor/silhouette/low-rate motion; no full-rate hidden AI/animation for ambience.
- Every new cost receives a counter, budget and proof view in the same envelope.

Device measurements may tighten these numbers. Raising them requires Terry approval and evidence.

## 9. Soundtrack standard

The soundtrack must be good enough to carry awe without covering the world:

- every world has a recognizable tonal/instrument identity;
- musical development mirrors narrative development;
- exploration loops avoid obvious short repetition;
- Awe cues are earned and sparse;
- threat adds pulse/texture without erasing the world’s identity;
- return-home material can remember where the player has been;
- diegetic sound and score can crossfade into each other;
- silence is authored;
- every transition respects memory and single-owner audio rules;
- final evaluation happens in-headset with spatial ambience, not from a desktop music file alone.

`docs/design/ADAPTIVE_AUDIO.md` remains the system-level companion specification.

## 10. Proof and promotion rules

A cinematic feature is not complete because it compiles.

Required proof, depending on feature:

- pure logic/unit tests;
- budget/audit gate;
- Forge turnarounds or contact sheets;
- world review images;
- before/after comparison;
- headset visual/comfort verdict;
- frame and memory evidence;
- explicit critique and revision record.

A warning becomes a blocker only after:

1. the relevant pilot world reaches zero honest warnings;
2. Terry confirms the result in the headset;
3. the escape hatch is specific and documented;
4. regression tests prove the ratchet cannot be bypassed by wholesale whitelisting.

## 11. Render-review loop

The existing Forge rendering workflow remains central. FORGE IV expands it:

1. render asset/world proof views;
2. inspect silhouette, materials, depth, scale, grounding and budget card;
3. write a concrete verdict—not “looks better”;
4. revise one cause at a time;
5. rerender;
6. only then queue the headset judgment.

Creature improvement should remain visible as a ladder of turnarounds exactly as it was during Forge II. FORGE IV adds equivalent world/vista/Awe Node contact sheets so environment progress is equally visible before Terry enters the headset.

## 12. Immediate status and next action

- CP-0 is complete in documentation.
- No FORGE IV runtime code is authorized merely by this plan.
- Picasso’s clean next action remains the already-boarded FORGE III work: F3.5 runtime factory or F3.7 signage, according to Terry’s assignment and lane collisions.
- Terry’s next device pass remains required before broad FORGE IV implementation.
- After Forge III/device evidence, CP-1 review artifacts is the first implementation envelope because every later quality system depends on models being able to see and critique worlds, not only isolated assets.

## 13. Definition of success

FORGE IV succeeds when:

- nearby objects feel inspectable and materially real;
- distant objects gain realism through atmospheric simplification rather than visible pop or low-quality noise;
- worlds contain near/mid/far/celestial depth;
- at least one reveal per signature world creates genuine scale and awe;
- the environment responds without drowning the screen in effects;
- the score and spatial sound create emotion while leaving room for silence;
- W002 is technically clean and W001 feels like a film the player can inhabit;
- stable standalone-Quest performance and comfort are preserved;
- quality cannot silently regress when a future smaller model adds content.
