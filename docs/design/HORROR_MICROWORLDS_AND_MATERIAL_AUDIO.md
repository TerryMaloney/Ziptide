# HORROR MICROWORLDS & MATERIAL-DRIVEN AUDIO

**Status:** DESIGN / RESEARCH / PLANNING ONLY. No runtime, scene, prefab, asset, package, test, workflow, save-schema, travel, locomotion, audio-middleware, or certified-checkpoint change is authorized by this document.

**Purpose:** capture Terry's direction for one Lost-in-Space-1998-style abandoned/infested derelict, at least one bounded horror destination or horror sector, and a scalable material-driven sound system covering footsteps, landings, foliage, objects, weapons, doors, debris, and planetary terrain.

**Companion documents:**
- `docs/design/ORBITAL_INTERIORS_GRAVITY_AND_DOORS.md`
- `docs/design/WORLD_PHYSICS_VARIANTS.md`
- `docs/project_art_plan/DAMAGE_RESPONSE_AND_RUIN.md`
- `docs/design/ENEMIES_ENCOUNTERS_AND_BOSSES.md`
- `docs/design/WEAPON_FEEL_AND_ARSENAL.md`
- `docs/design/RILL_CAPTION_RESEARCH.md`
- `docs/project_art_plan/FORGE_V_LIVING_STAGE.md`

---

## 1. Product decision

ZIPTIDE should contain **bounded horror microworlds** without becoming a horror game overall.

The player should be able to move from wonder, adventure, comedy, building, and family-friendly experimentation into one destination where the emotional grammar changes completely:

- rooms feel abandoned rather than merely empty;
- doors become suspense and containment tools;
- sound reveals activity before visuals do;
- gravity, power, pressure, and lighting failures create vulnerability;
- environmental evidence asks the player to infer what happened;
- a threat is revealed gradually and remains readable once active;
- the player still retains agency, weapons, RILL, and a clear extraction route.

The strongest first inspiration is the **Proteus sequence from the 1998 film _Lost in Space_**: a seemingly abandoned future human ship, evidence of a vanished crew, an infestation hidden inside the structure, and a rapid escape once the threat is understood.

This is an inspiration target, not a license to copy its ship, spiders, plot, shots, creature design, or names. ZIPTIDE should reproduce the emotional sequence using original lore, original creatures, and existing game systems.

---

## 2. The ghost-ship infestation grammar

### 2.1 What creates the desired feeling

1. **Recognizable human function, wrong condition.** The vessel has cargo bays, service corridors, crew areas, a bridge, and machinery the player understands—but every system has failed in a different way.
2. **Evidence before enemy.** Drag marks, open emergency lockers, cut doors, abandoned tools, interrupted recordings, shed fragments, and unexplained wall damage appear before a live threat.
3. **The ship is not silent.** Hull stress, distant impacts, intermittent machinery, relays, ventilation, and floating object contacts maintain uncertain activity.
4. **The first movement is ambiguous.** A small object shifts, something crosses behind translucent material, or a sound travels through a wall. It must remain possible that the ship itself caused it.
5. **The threat uses the architecture.** It moves through service cavities, vents, wall seams, cable trays, and damaged bulkheads so the level feels inhabited from within.
6. **The reveal changes the objective.** Before the reveal, the player is restoring or salvaging. Afterward, the player is sealing, rerouting, rescuing, trapping, or extracting.
7. **The return path is transformed.** Doors previously open may seal; gravity may be restored; a shortcut may unlock; the creature may force a new route.
8. **The player escapes with knowledge and consequence.** The reward can be salvage, a ship module, creature data, a story key, or proof that the infestation can spread.

### 2.2 Original threat directions — proposals, not canon

Do not default automatically to spiders. Candidate threat families:

- **Seamrunners:** flat metallic organisms that travel inside hull seams and briefly unfold when attacking machinery.
- **Latchlings:** small magnetic scavengers that cling to walls, doors, tools, and one another; individually manageable, dangerous in coordinated swarms.
- **Hollowers:** organisms that occupy machines or suits and make them move incorrectly.
- **Signal Mites:** partly physical, partly networked entities that produce false door requests, broken captions, or misleading equipment pings.
- **Bloom Splinters:** a non-gory Bloom offshoot that grows rigid conductive limbs through damaged wall panels.
- **The Passenger:** one larger unseen organism inferred through displaced mass, door deformation, and impossible sounds before any full reveal.

The threat must connect to ZIPTIDE's existing disable/capture/salvage identity. Even in horror, the game should prefer stunning, trapping, repelling, rerouting, or studying over gore.

### 2.3 Recommended first horror orbital site

Working title: **THE HOLLOW RELAY**.

This can reuse the door and gravity kit proven by `Dead Weight` while providing a different emotional experience.

Mission spine:

1. Approach a communications/salvage vessel that is still broadcasting a routine docking signal.
2. Dock; the signal continues despite no crew response.
3. Enter through a manual hatch. Normal gravity and emergency power remain in the entry section.
4. Find evidence that internal doors were sealed from both sides.
5. Restore one power branch to reach the relay core.
6. Hear contact inside the walls before seeing anything alive.
7. Enter a cargo/service room where loose objects float under failed local gravity.
8. A small creature or machine-like object crosses between floating cover and disappears through a damaged wall seam.
9. Repair the gravity controller; settling objects expose a hidden breach or nest route.
10. Reach the relay and discover that its repeated docking call is being generated automatically—or intentionally by the threat.
11. The infestation becomes active and changes door states.
12. Use Taser, Gravity Gun, force fields, shutters, or a Capture Net-style tool to contain rather than simply kill.
13. Escape through a transformed route while the ship tries to seal sectors.
14. Return with one original creature sample, transmission fragment, or ship upgrade.

### 2.4 First-derelict sequencing decision

Two viable production choices remain:

- **Conservative:** build `Dead Weight` first as the clean systems proof, then reuse its doors/gravity/rooms for `The Hollow Relay`.
- **Ambitious:** make the first derelict itself the horror mission while limiting it to one creature family, one gravity room, and one route transformation.

The conservative path is technically safer. Terry's taste decision determines whether the first derelict is already horror or whether horror becomes the second orbital site.

---

## 3. Horror microworld vocabulary

ZIPTIDE should use several distinct horror grammars rather than repeating one infestation trick.

### 3.1 Infested derelict

**Feeling:** something is inside the machine.

**Tools:** vents, wall seams, floating debris, emergency shutters, power failures, moving cover, intermittent signals.

**Best location:** orbital ship or station.

### 3.2 Liminal maintenance maze

**Feeling:** the architecture is almost repetitive, but not quite; the player loses confidence in spatial memory.

**Tools:** repeated service modules, numbered doors, subtly changed props, impossible return route, distant audio landmarks, RILL map uncertainty.

**Rules:**
- never create an actually infinite maze;
- preserve a recoverable route and accessibility option;
- use three to five authored variations, not uncontrolled random corridors;
- clues must prove that the changes are intentional rather than level bugs.

**Best location:** station sublevel, Architect annex, abandoned transit facility.

### 3.3 Watcher / surveillance horror

**Feeling:** the player can observe threats but cannot watch every route simultaneously.

**Tools:** cameras, motion sensors, power distribution, shutters, maintenance windows, audio pings, limited safe-room controls.

This captures the tension of games such as _Five Nights at Freddy's_ without copying animatronics or its exact structure. A ZIPTIDE version could involve malfunctioning salvage drones, cargo mannequins, repair frames, or dormant security machines whose positions change when not observed.

**Best location:** one compact station shift, museum/storage vault, security control room.

### 3.4 Backrooms-style liminal anomaly

**Feeling:** ordinary space has lost its reason.

**Tools:** endless-looking but bounded architecture, wrong proportions, stale room tone, repeated fluorescent behavior, missing exterior references, doors returning to altered rooms.

**ZIPTIDE distinction:** connect the anomaly to the Pattern, Architect infrastructure, or a failed world compiler. It should be lore, not an unrelated meme level.

### 3.5 Environmental planet horror

**Feeling:** the ecosystem or landscape is watching, listening, or behaving by an unknown rule.

Possible identities:
- a night-side world where light attracts one organism but darkness attracts another;
- a fog basin where audio reflections reveal large movement outside vision;
- a forest that becomes silent before local predators move;
- a crystalline plain that transmits footsteps far beyond sight;
- a world where abandoned structures repeat a distress signal with no sender;
- a heavy-gravity ruin where movement is slow and distant impacts are felt through the ground.

A horror planet can still contain safe daylight or settlement spaces. One world sector may be enough; the entire planet need not maintain maximum tension.

### 3.6 Machine-possession horror

**Feeling:** familiar tools have become unreliable.

Doors request opening without input, drones face the wrong direction, captions identify a speaker who is absent, or a repair machine completes an action nobody requested.

Use sparingly. The project must never imitate actual software bugs so closely that players cannot tell intentional horror from broken gameplay.

### 3.7 Cosmic scale horror

**Feeling:** the threat is size, age, or implication rather than pursuit.

Tools include impossible structures, a moving silhouette behind clouds, signals older than the mapped network, or a planet-sized mechanism that responds once and then goes quiet.

This is best used as awe plus dread, not constant combat.

---

## 4. Horror constitution for VR and family play

1. No mandatory jump scare may spawn directly inside the player's near-face comfort space.
2. No event may rotate, shake, parent, or throw the player's head or rig.
3. Loud transients need bounded peaks and accessibility control.
4. Horror relies first on anticipation, implication, sound, and environment—not gore.
5. The player always has a comprehensible immediate objective, even when the larger mystery is unclear.
6. Every pursuit has a recoverable state, safe boundary, or explicit fail/retry behavior.
7. Threats must obey readable rules once revealed.
8. Do not take away all player tools merely to manufacture helplessness.
9. RILL can be frightened, uncertain, or briefly disrupted, but should not become a constant liar without strong story justification.
10. Children and sensitive players need a **Tension Assist** option that can reduce sudden sounds, close lunges, darkness, pursuit speed, and false cues while preserving objectives and rewards.
11. No horror destination should be required repeatedly for ordinary crafting resources after completion.
12. Use horror rarely enough that entering one of these sites feels like a special tonal event.

Suggested tension curve:

`normal -> uneasy -> evidence -> first ambiguous movement -> partial reveal -> rule learned -> active problem -> escape/containment -> quiet aftermath`

Do not hold the entire mission at maximum intensity.

---

## 5. Sound is a gameplay and art system

ZIPTIDE's sound should be data-driven from the same semantic material identity used by art and physics.

A floor is not merely a texture and not merely a PhysicMaterial. It has a shared **material identity** that can drive:

- footsteps;
- landing impacts;
- dropped objects;
- sliding and dragging;
- weapon impacts;
- shell/dart/debris contacts;
- door and machinery resonance;
- environmental rustle;
- reverb and occlusion expectations;
- physical friction/bounce where appropriate.

The audio layer should not infer identity from asset names, renderer materials, or ad-hoc tags when a canonical data definition is available.

---

## 6. Closed material-audio vocabulary

First proposed material classes:

### Hard architectural
- MetalSolid
- MetalHollow
- MetalGrate
- Concrete
- Stone
- BrickCeramic
- WoodSolid
- WoodHollow
- Glass
- Polymer
- ArchitectComposite

### Natural ground
- DirtDry
- DirtWet
- Gravel
- Sand
- Mud
- GrassShort
- GrassThick
- LeavesThin
- LeavesDeep
- SnowPowder
- SnowCrust
- ShallowWater
- OrganicBloom
- Crystal

### Object/accent
- Cloth
- CableRubber
- PaperPackaging
- SalvageScrap
- CreatureShell
- EnergyField

The list is deliberately bounded. New worlds should map exotic surfaces to an existing acoustic family plus optional accent layers before adding a new global material class.

---

## 7. Proposed data architecture

Names are proposals and must be reconciled with the actual repository before implementation.

```text
MaterialAudioDefinition
- materialId
- materialFamily
- footstepSet
- landingSet
- scrapeSet
- rollSet
- impactLightSet
- impactMediumSet
- impactHeavySet
- projectileImpactSet
- debrisImpactSet
- movementOverlaySet
- defaultReverbSend
- defaultOcclusionClass
- physicalResonance
- randomizationProfile
```

```text
SurfaceAudioBinding
- primaryMaterialId
- secondaryMaterialId optional
- secondaryWeight
- overlayMaterialId optional
- overlayDepth / coverage
- wetness
- damageState
```

```text
SurfaceAudioResolver
- collider/author metadata path
- terrain blend path
- foliage/overlay path
- returns primary + secondary + overlay + weights
- allocates nothing per event
- contains no AudioSource ownership
```

```text
ContactAudioRouter
- actionKind
- source material
- target material
- energy band
- mass class
- speed/intensity
- environment profile
- selects and posts one bounded event recipe
```

```text
AudioEnvironmentProfile
- environmentId
- roomTone
- reverbClass
- occlusionClass
- air/pressure state
- mixSnapshot
- ambient emitter budget
- horror tension state optional
```

One existing or future canonical audio owner should execute playback. These definitions must not create a second global audio manager.

---

## 8. Footsteps, locomotion, and landing

### 8.1 VR step generation

Because the player avatar may not have conventional leg-animation events, step timing should derive from the canonical locomotion state:

- grounded;
- horizontal speed;
- stride-distance accumulator;
- crouch/sprint state;
- slope;
- current surface binding;
- optional boot/equipment profile.

Do not simply play a footstep every fixed 0.3 seconds. Cadence should remain distance-based so slow movement, sprinting, sliding, and artificial movement do not sound identical.

### 8.2 Landing intensity

Landing sound should derive from bounded physical evidence:

- downward velocity before contact;
- effective gravity;
- player movement mode;
- surface material;
- boot type;
- crouch/soft-landing modifier.

Suggested bands:
- Settle
- Step
- Firm
- Heavy
- Emergency

The audio may become louder, lower, more resonant, or add debris/rattle layers. It should not increase without clamp.

### 8.3 Metal catwalk example

A normal step on a metal grate combines:

- boot contact;
- local grate texture;
- restrained structural ring.

A hard landing adds:

- heavier boot transient;
- broader catwalk resonance;
- nearby loose fastener/rattle layer;
- room reverb contribution.

The catwalk should not play its maximum resonance for every step.

---

## 9. Base surface plus overlay

The leaf example requires two meanings:

- **base ground:** dirt, stone, metal, etc.;
- **overlay:** leaves, water, glass fragments, shallow salvage, ash, snow, Bloom fibers.

Walking across dirt with scattered leaves should produce dirt footsteps plus a light leaf rustle. Walking through a deep pile should make the leaf layer dominant and can add sustained brushing or displacement sounds.

Suggested overlay states:
- Trace: occasional accent only.
- Light: accent on most steps.
- Dense: blended step plus rustle.
- Pile: dominant overlay, movement loop, and object displacement.

This prevents an explosion of duplicate material combinations such as `dirt_with_leaves`, `metal_with_leaves`, and `concrete_with_leaves`.

The same model supports:
- shallow water over stone;
- glass fragments over concrete;
- snow over metal decking;
- Bloom tendrils over ship flooring;
- loose salvage over a cargo-bay floor.

---

## 10. Planet-scale surface resolution

### 10.1 Mesh and authored floors

Use explicit semantic metadata on the collider, Forge recipe, or authored surface definition.

A renderer's visual material can change without losing sound identity. A collider may cover several visual meshes while retaining one audio class.

### 10.2 Unity Terrain

Terrain splat/alphamap data provides weights for painted terrain layers at a location. The resolver can use the two strongest weights to blend a primary and secondary step rather than choosing one abruptly.

Performance rules:
- do not call a full alphamap read on every step;
- cache terrain layer data or sample a small prepared lookup;
- update the current blend at a bounded cadence or when terrain changes;
- use authored mapping from each TerrainLayer to a MaterialAudioDefinition;
- fall back deterministically when mapping is missing.

### 10.3 Vegetation and detail coverage

Do not place an AudioSource on every plant or leaf.

Use one of:
- bounded vegetation/foliage volumes;
- Forge-authored density metadata;
- terrain detail-density lookup;
- explicit hero interaction patches for deep foliage.

The system should report coverage/depth to the overlay resolver, which then adds rustle behavior to player and object movement.

### 10.4 Procedural/compiled worlds

Forge VI world recipes should require material-audio bindings alongside visual kits. A compiled world with no valid ground-audio identity is incomplete.

Suggested audit:

`SURFACE_AUDIO_UNMAPPED`

- BLOCKER on traversable hero surfaces;
- WARN on unreachable decoration;
- report world counts and unknown material IDs.

---

## 11. Object interaction sound at scale

Do not author one impact clip for every prop against every floor.

Resolve each contact from:

`source material family x target material family x energy band x action kind`

Action kinds:
- Drop
- Impact
- Roll
- Scrape
- Slide
- Break
- ProjectileHit
- DebrisSettle
- GrabContact

Energy should use collision impulse or a bounded approximation from relative speed and mass class. Ignore or suppress micro-contacts below threshold, and apply per-object cooldowns to avoid physics chatter.

Examples:

- light metal tool on metal grate -> short tick + grate accent;
- heavy salvage crate on metal deck -> impact body + deck resonance;
- glass fragment on concrete -> small brittle scatter;
- gravity-restored cargo bay -> prioritized settle events rather than sixteen full-volume impacts at once;
- dart on creature shell -> projectile body + shell response + weapon confirmation layer.

The response matrix in `DAMAGE_RESPONSE_AND_RUIN.md` should share material IDs with this system.

---

## 12. Horror audio architecture

### 12.1 Tension states

Proposed mix states:

- **Safe / Ordinary** — full normal ambience and RILL presence.
- **Unease** — fewer predictable loops, increased distant detail, no confirmed threat.
- **Evidence** — authored sounds tied to clues and spaces.
- **Threat Confirmed** — readable creature/material language becomes consistent.
- **Pursuit / Containment** — critical threat cues prioritized; ambience simplified.
- **Aftermath** — machinery and ordinary room tone return gradually, but one unresolved layer remains.

The state changes the mix and available event vocabulary; it does not spawn arbitrary scare sounds around the player.

### 12.2 Audio landmarks

In darkness, fog, liminal rooms, or repeating corridors, sound must help orientation:

- relay hum;
- generator pulse;
- airlock pump;
- dripping coolant;
- damaged fan;
- RILL beacon;
- creature nest resonance;
- exterior hull knocks.

Each major route should have at least one stable audio landmark so horror does not become navigation failure.

### 12.3 False cues

False cues are allowed only under rules:

- they use plausible ship or environmental causes;
- they cannot repeatedly steal attention without payoff;
- once the creature's audio language is learned, true cues remain distinguishable;
- accessibility can reduce or remove nonessential false cues.

### 12.4 Silence

Silence is a contrast tool, not the default state. Before a major reveal, selectively remove one expected layer—ventilation, wildlife, relay chatter—rather than muting the entire game.

### 12.5 Music

Do not cover every horror space with a conventional tension track. Use restrained non-musical textures, machinery tones, structural noise, and interactive stingers. Music can enter at key transitions, pursuit, or aftermath.

---

## 13. Door, room, and occlusion integration

Doors are acoustic portals as well as geometry.

Closed bulkhead:
- attenuates next-room sound;
- removes high-frequency detail;
- preserves selected structural and alarm transmission;
- limits active emitters in hidden rooms.

Opening sequence:
- authorization/lock sounds;
- seal release;
- motor travel;
- sound from the next room becomes clearer and directionally centered on the opening;
- full room reverb and ambience blend after passage.

Horror use:
- a sound may begin behind a sealed door;
- the door creates anticipation while opening;
- the revealed room must justify or transform the sound;
- ordinary doors remain quick so special slow reveals retain power.

Force fields need different sounds for pressure veil, security denial, containment, and authorized passage. One hum cannot communicate contradictory rules.

---

## 14. Quest performance constitution

1. Define project-wide real/virtual voice budgets before broad content scaling.
2. Prioritize RILL/dialogue, immediate threats, player interactions, and objectives above decorative ambience.
3. Apply global and per-object voice limits to physics chatter, foliage, debris, insects, and repeated machinery.
4. Use distance and room-state audio LOD; distant ambience should be simpler than close hero detail.
5. Virtualize or stop inaudible sounds according to whether playback position matters.
6. Pool short-lived AudioSources or use the existing canonical emitter path.
7. Avoid per-frame allocations in terrain, contact, occlusion, and foliage resolution.
8. Keep random variation data-driven; do not instantiate clip arrays at runtime repeatedly.
9. Limit simultaneous reverbs/effects and pre-render effects when the result does not need runtime control.
10. Load audio by destination/cluster rather than keeping every planet's full library resident.
11. Profile on Quest with intentionally low voice limits to prove important sounds survive.
12. Any middleware decision—Unity native, Meta XR Audio SDK, Wwise, or FMOD—must be a separate architecture decision. This design remains middleware-neutral.

---

## 15. Recommended first audio vocabulary

Start with a deliberately small high-value set:

### Player movement
- MetalSolid
- MetalGrate
- Concrete
- DirtDry
- Gravel
- GrassShort
- LeavesThin/Deep overlay
- ShallowWater overlay
- landing intensity bands

### Objects
- MetalLight
- MetalHeavy
- Wood
- Glass
- Polymer
- SalvageScrap
- CreatureShell

### Environments
- SmallMetalRoom
- LargeCargoBay
- ExteriorOpen
- Cave/Ruin
- Forest
- PressurizedDerelict
- Vacuum/SuitConducted

### Doors
- SalvageManual
- IndustrialPowered
- ArchitectQuiet
- ForceField
- Airlock

That is enough to prove the architecture before filling every world.

---

## 16. Bounded implementation envelopes after recovery exit

### MA-0 — repository audio archaeology

Inventory every current AudioSource helper, audio service, weapon sound path, door sound, material tag, PhysicMaterial, terrain system, Forge metadata seam, mixer, reverb, and spatializer.

**Output:** surviving owner map and reuse/repair/retire matrix. No runtime change.

### MA-1 — material vocabulary and pure resolver

Add or reconcile semantic material definitions, bindings, validation, and pure resolution tests.

Acceptance:
- mesh binding resolves deterministically;
- terrain blend returns bounded top layers;
- overlay composes without creating duplicate material classes;
- missing traversable mappings are reported.

### MA-2 — player footsteps and landing

Implement distance-based cadence, surface variation, overlay blending, and landing intensity through the canonical locomotion/audio seams.

Quest acceptance:
- no false rapid footsteps;
- no obvious sample repetition during a five-minute walk;
- metal, dirt, gravel, leaves, and water are recognizable with eyes closed;
- RILL remains intelligible.

### MA-3 — object contacts

Implement bounded contact routing for drop, impact, scrape, roll, projectile, and debris settle.

Acceptance:
- no physics chatter storm;
- material and energy differences are readable;
- gravity-restoration scene remains within voice and CPU budgets.

### MA-4 — environment, doors, and audio portals

Add room profiles, door occlusion behavior, reverb blending, and destination-specific ambient budgets.

### MA-5 — hero horror graybox

Build the bounded `Hollow Relay` route using proven doors, one gravity chamber, one original threat family, and one extraction transformation.

No final creature art required for graybox, but audio language and route clarity must be testable.

### MA-6 — planet-scale authoring

Integrate mappings into Forge recipes, TerrainLayer data, foliage coverage, world audits, and destination audio-bank loading.

### MA-7 — horror polish and family test

Add final art/audio, Tension Assist, kid/family playtest, performance proof, and save/relaunch route.

---

## 17. Acceptance criteria

The program succeeds when:

- one derelict gives Terry the abandoned-future-ship/hidden-infestation feeling without copying the film;
- ZIPTIDE has at least one memorable bounded horror destination or sector;
- horror remains rare enough to preserve tonal variety;
- players understand threat rules after revelation;
- no horror event violates VR comfort or removes all agency;
- the kids can use Tension Assist and still receive the same mission rewards;
- footsteps identify metal, dirt, gravel, foliage, water, and major special materials without looking;
- a hard landing sounds and feels different from a normal step;
- deep leaves produce more rustle than scattered leaves without becoming a separate bespoke floor system;
- dropped objects and weapons respond audibly to both object and target materials;
- large planets do not require hand-placing audio triggers over every terrain patch;
- doors and rooms create convincing occlusion, reverb, and suspense;
- Quest holds its performance target under the busiest approved physics/audio event;
- SaveSystem, TravelCoordinator, locomotion, ItemFactory, and existing art/physics owners remain canonical.

---

## 18. Explicit non-goals

This program does not authorize:

- copying the Proteus, space spiders, Backrooms layouts, Five Nights characters, or any protected story/visual design;
- turning every world into horror;
- random jump-scare emitters;
- direct face-lunges in VR;
- gore as the primary fear tool;
- an infinite procedural maze;
- audio sources on every plant, leaf, pebble, or prop;
- one bespoke footstep system per world;
- one sound event for every possible material pair;
- per-step full-terrain alphamap reads;
- unlimited physics-contact audio;
- choosing Wwise/FMOD/Meta XR Audio solely from this design document;
- changing the certified headset candidate.

---

## 19. Research basis

Research and implementation guidance consulted:

1. _Lost in Space_ (1998) plot/Proteus sequence references — abandoned future ship and silicon-based infestation.
2. GDC: **Materials vs. Audio: Object Interaction Sound Design in Halo** — scalable material interaction across footsteps, collisions, casings, and environments.
3. GDC: **All Style, All Substance: The Audio Journey of the Vanity and Traversal System for Sunset Overdrive** — modular traversal/footstep audio across many behaviors, materials, and equipment variants.
4. Audiokinetic Wwise Unity Demo — Random Containers nested under a material Switch Container for footsteps.
5. Audiokinetic mobile optimization guidance — voice limits, priorities, virtual voices, audio LOD, profiling, and SoundBank granularity.
6. FMOD staff guidance — event parameters can select or blend surface layers, including layered surfaces such as wet sand or gravel over another base.
7. Unity 2022.3 documentation — TerrainData alphamap weights, Collider/PhysicMaterial identity, AudioSource priority/virtualization, reverb zones, low-pass filtering, and 3D distance curves.
8. GDC: **The Sound of Horror: Resident Evil 7** — realistic environmental sound, automated implementation tools, and nontraditional disturbing textures.
9. GDC: **The Demonic Sound of Paranormal Activity: The Lost Soul** — procedural audio, physics interaction, VR horror atmosphere, and multiplatform optimization.
10. GDC: **Aural Immersion: Audio Technology in The Last of Us** — environmental guidance, dynamic mixing, dialogue priority, and obstruction/occlusion.
11. GDC: **What Happened Here? Environmental Storytelling** — props, lighting, systems, and environmental reaction as player-interpreted narrative.

All conclusions remain subject to real repository archaeology and Quest validation before implementation.
