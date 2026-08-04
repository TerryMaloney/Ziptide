# ORBITAL INTERIORS, GRAVITY SECTORS & DOOR FAMILY

**Status:** DESIGN / RESEARCH / PLANNING ONLY. No runtime, scene, prefab, asset, package, test, workflow, save-schema, travel, locomotion, or certified-checkpoint change is authorized by this document.

**Purpose:** define how ZIPTIDE can support space stations, derelict ships, orbital salvage sites, low-gravity interior sections, gravity-restoration missions, and a reusable family of doors without creating parallel travel, physics, interaction, save, or audio systems.

**Companion documents:**
- `docs/design/FIRST_HOUR_DIRECTORS_CUT.md`
- `docs/design/SHIP_DESIGN_INTERIOR_EXTERIOR.md`
- `docs/design/WORLD_PHYSICS_VARIANTS.md`
- `docs/project_art_plan/DAMAGE_RESPONSE_AND_RUIN.md`
- `docs/design/ENEMIES_ENCOUNTERS_AND_BOSSES.md`
- `docs/design/WEAPON_FEEL_AND_ARSENAL.md`
- `docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md`

---

## 1. Core product decision

ZIPTIDE should treat stations, wrecks, and large derelicts as **orbital destinations**: they are destination-sized missions with the same legitimacy as a planet world, even when they are not planets.

They may include:
- an exterior approach or space-combat beat;
- docking or breach entry;
- a coherent interior route;
- power, pressure, security, gravity, or structural state changes;
- salvage, enemy, repair, and story objectives;
- a return to the player ship through the canonical travel path.

Recommended player-facing category names:
1. **Orbital Sites** — preferred umbrella term.
2. Derelict Sites.
3. Salvage Stations.
4. Void Sites.

An orbital site is not a special minigame with separate ownership. It remains normal data and content routed through the existing destination/travel/save architecture.

---

## 2. Research findings translated into ZIPTIDE rules

### 2.1 Zero gravity works best when motion is self-authored

Ready At Dawn's `Lone Echo` used hands to push, pull, grab arbitrary surfaces, and gently sail through zero gravity. The important lesson is not merely "zero gravity is cool"; it is that the player's own hand action explains the motion.

**ZIPTIDE rule:** any future full player zero-g mode must be hand-authored, rail/handle-authored, or explicitly thruster-authored. The game may not suddenly accelerate the rig because a room changed state.

### 2.2 Zero gravity should reveal new routes, not only change numbers

Motive's `Dead Space` remake expanded zero-g into full 360-degree traversal and used it to open alternate navigation, exploration, and environmental challenges inside a coherent ship.

**ZIPTIDE rule:** a gravity-off sector must change what the player can reach or how a problem is solved. A room containing floating cans but no changed route, tool use, target, or repair beat is visual garnish, not a complete gravity encounter.

### 2.3 Stations feel real when their systems and spaces make sense together

Arkane's `Prey` treated Talos I as a coherent, interconnected place with departments, utilities, routes, exterior access, and consequences that made sense within the station.

**ZIPTIDE rule:** every orbital site needs a simple functional diagram before art production:

`dock/breach -> access control -> service route -> objective system -> reward/extraction`

Rooms must exist for an understandable reason: cargo, habitation, hydroponics, power, comms, fabrication, security, research, coolant, docking, maintenance, or transit.

### 2.4 Doors are a whole-system design problem

The industry "Door Problem" correctly asks not just whether a door opens, but how players identify usable doors, what happens when something blocks one, how allies/enemies/held items cross it, what saves remember, and how art/audio/level design communicate its state.

**ZIPTIDE rule:** do not build isolated animated door prefabs. Build one state contract with interchangeable motion, art, audio, access, pressure, and occlusion adapters.

### 2.5 Closed doors are useful performance and audio boundaries

Unity supports doors as occlusion portals for rendering. Closed doors can also drive audio occlusion or low-pass behavior, allowing sound to leak through differently before and after opening.

**ZIPTIDE rule:** major bulkheads should be treated as render, physics-activity, encounter, and audio boundaries—not only moving meshes.

---

## 3. Orbital destination product shapes

### 3.1 Derelict salvage mission

Recommended first orbital-interior mission because it reuses the strongest current themes.

**Shape:**
1. Approach wreck in the ship.
2. Optional short exterior drone or debris beat.
3. Dock or attach salvage collar.
4. Enter through a manual pressure hatch.
5. Restore local power.
6. Cross a gravity-off cargo/service sector.
7. Repair the gravity controller.
8. Watch loose objects settle and routes change.
9. Reach the protected salvage/story object.
10. Return through the restored vessel and extract.

**Player promise:** "I entered a dead machine, understood what failed, repaired it, and changed how the whole place behaved."

### 3.2 Occupied or failing station mission

**Shape:**
1. Space battle or defensive approach.
2. Dock while station defenses or drones remain active.
3. Move through powered public areas and failed service areas.
4. Use access, power, force-field, and pressure doors.
5. Complete one station-scale objective: restore comms, defend reactor, rescue cargo, remove a hostile controller, or repair docking systems.
6. Return to the ship with a visible station-state change.

### 3.3 Research ring / anomaly annex

Later destination for carefully bounded physics anomalies.

Use small authored zones for:
- `static_surge`;
- `mag_lift`;
- `tide_pull`;
- object-only gravity pockets;
- unusual air density or wind in damaged pressurized tunnels.

These remain rare and story-anchored. Do not combine all anomalies in one site.

### 3.4 First-hour relationship

The Director's Cut already uses an exterior Space Lane salvage sortie. Do **not** add a full derelict interior to the first hour.

A strong later callback is:
- hour one: salvage an artifact half from the exterior of a wreck;
- later mission: return to or find a related derelict and enter its interior.

This makes the universe feel connected without expanding onboarding scope.

---

## 4. Gravity architecture: two deliberately separate tiers

### Tier A — object-physics gravity sector

This is the recommended first implementation.

The player keeps normal locomotion or uses diegetic magnetic boots. Loose objects, debris, projectiles, hanging cables, particles, and selected enemies respond to the sector's local gravity state.

Advantages:
- strong visual and mechanical identity;
- low locomotion risk;
- real floating, bumping, throwing, and settling objects;
- compatible with the existing comfort constitution;
- useful before a full zero-g player controller exists.

### Tier B — full player zero-g locomotion

This is a separate foundational campaign, not an extension checkbox on Tier A.

Potential later controls:
- grab surface and pull;
- push off surfaces;
- handhold rails;
- limited wrist/hand thrusters;
- optional auto-align or horizon assistance;
- explicit comfort settings and tutorial space.

This tier touches player movement, fall recovery, input ownership, comfort, save/reload, and travel transitions. It requires its own clean proof and focused Quest campaign.

---

## 5. Room-local gravity law

### 5.1 Do not use global `Physics.gravity` for one room

Unity's `Physics.gravity` affects all gravity-enabled rigidbodies in the physics scene. Writing it for one cargo bay would also affect unrelated rooms, projectiles, debris, and systems sharing the same physics scene.

For room-local gravity, use one of these controlled approaches:

1. **Per-body sector force — preferred for the first version.**
   - registered bodies disable default gravity while inside the sector;
   - one sector owner applies the sector acceleration in `FixedUpdate`;
   - original gravity/drag state is restored on exit;
   - only approved physics categories participate.

2. **Dedicated local physics scene — later, only if needed.**
   - useful for a fully isolated room or prediction/test annex;
   - more complex because simulation, object movement between scenes, queries, and lifetime become explicit;
   - not required for the first derelict.

### 5.2 Proposed data contract

Names are proposed and must be reconciled with real repository conventions before implementation.

```text
GravitySectorDefinition
- sectorId
- gravityScale
- gravityDirection
- linearDrag
- angularDrag
- affectsLooseProps
- affectsProjectiles
- affectsDebris
- affectsEnemies
- affectsPlayer (default false)
- restorationMode
- visualTellId
- audioProfileId
```

```text
GravitySectorRuntime
- owns sector membership
- snapshots original Rigidbody state
- applies deterministic local acceleration
- restores original state exactly once
- emits state changes
- never owns travel, save, or player locomotion
```

```text
GravityAffectedBody
- category
- stable body id when persistence is required
- original-state snapshot
- sector membership token
```

### 5.3 Initial physics categories

Closed first vocabulary:
- LooseProp
- SalvageChunk
- Projectile
- Throwable
- Debris
- Drone
- StoryItem

The player is not in the default list.

### 5.4 Active-body budget

Use the existing debris/physics budget as the governing ceiling. Proposed first room target:
- 8–12 deliberately placed floating props;
- no more than 16 simultaneously active loose rigidbodies in the hero chamber;
- simple primitive or compound colliders;
- sleeping and distance deactivation outside the active room;
- no decorative rigidbody clutter merely because it can float.

The exact cap is moved only by Quest evidence.

---

## 6. Gravity-restoration sequence

The user fantasy is valid: enter a failed low-gravity room, repair the system, and watch normal gravity return.

The safe sequence should be authored, readable, and deterministic.

### Recommended Tier-A sequence

1. Outer bulkhead opens.
2. RILL announces gravity failure before the player enters.
3. Magnetic boots or baseline locomotion remain active.
4. Loose objects float and respond to contact, weapons, and airflow.
5. Player repairs the gravity controller using existing repair verbs.
6. Warning lights and audio countdown begin.
7. Sector stops accepting new loose objects at the boundary.
8. Gravity ramps for **objects only** over a short controlled interval.
9. Objects settle with capped impact speed and staged audio.
10. Doors, routes, machinery, or salvage access change because gravity is restored.
11. World state records `gravity_restored` rather than saving every object's transform.

### Later full-player sequence

A player-affecting transition is allowed only when:
- the sector is sealed;
- the player has clear warning;
- the player is holding a rail, inside a brace zone, or otherwise stable;
- head pose is never altered;
- input response remains immediate;
- motion mode changes through the canonical locomotion owner;
- fall recovery thresholds update with the effective gravity;
- reload inside either state is tested;
- the transition can be skipped or softened in a comfort preset if needed.

No dramatic event is allowed to throw or rotate the player when artificial gravity returns.

---

## 7. Door system: one state owner, many presentations

### 7.1 Proposed state machine

```text
Unpowered
Locked
Sealed
Unlocking
Cycling
Opening
Open
Closing
Jammed
Breached
```

Not every door uses every state. The shared state machine defines legal transitions and authoritative meaning.

### 7.2 Proposed architecture

```text
DoorDefinition
- doorId
- motionKind
- accessKind
- pressureRole
- powerRequirement
- obstructionBehavior
- openTime / closeTime
- holdOpenTime
- audioProfileId
- visualRecipeId
- portalProfileId
```

```text
DoorRuntime
- sole state owner
- evaluates requested transition
- checks power/access/pressure/obstruction
- emits state events
- restores authoritative state after load
- never directly loads scenes
```

```text
DoorMotionDriver
- LinearSingle
- LinearSplit
- VerticalShutter
- Hinge
- Iris
- FieldDissolve
```

```text
DoorAdapters
- DoorAudioAdapter
- DoorOcclusionAdapter
- DoorAccessAdapter
- DoorPressureAdapter
- DoorVfxAdapter
- DoorTravelRequestAdapter (requests TravelCoordinator only)
```

### 7.3 Door state is not saved as animation time

Persist the authoritative condition:
- powered/unpowered;
- locked/unlocked;
- sealed/unsealed;
- jammed/repaired;
- breached/intact.

On load, resolve directly to the correct stable pose. Do not resume a door halfway through an animation unless there is a proven player-facing need.

---

## 8. Door family vocabulary

### 8.1 Split sliding bulkhead

**Use:** common station corridor and ship door.

**Art:** two thick panels, visible tracks, compression seals, edge lights.

**Mechanics:** automatic or button-controlled; ideal render/audio portal.

**Audio:** request chirp -> lock clack -> motor start -> travel loop -> seal hiss -> end-stop thud.

### 8.2 Vertical blast shutter

**Use:** combat lock, emergency containment, cargo access.

**Art:** heavy ribs, floor receiver, overhead machinery, warning stripes.

**Mechanics:** dramatic sightline reveal; can be partially jammed; never drops onto the player.

**Audio:** deep motor, chain/gear texture, heavy final impact.

### 8.3 Manual wheel or lever hatch

**Use:** powerless derelict, maintenance access, pressure door.

**Art:** visible dogs/bolts, wheel, gasket, inspection window.

**Mechanics:** physical VR interaction; player turns or pulls through a limited range, then the latch releases.

**Audio:** ratchet, metal strain, seal pop, hinge groan.

### 8.4 Iris aperture

**Use:** docking collar, high-tech research sector, compact circular passage.

**Art:** overlapping blades or segmented ring.

**Mechanics:** clear center opening; collision must match aperture; no invisible square blocker.

**Audio:** layered segment movement, circular lock sequence.

### 8.5 Force-field seal

**Use:** security, pressure retention, containment, faction-specific technology.

**Art:** visible emitters, boundary frame, animated field direction, distortion kept Quest-safe.

**Mechanics:** collision and access rules must be explicit:
- blocks everything;
- blocks enemies/weapons but permits authorized player;
- pressure-only veil;
- one-way containment.

Do not use one visual for contradictory behaviors.

**Audio:** constant low hum, proximity shimmer, contact pulse, pitch-rise on unlock, clean power-down tail.

### 8.6 Airlock pair

**Use:** exterior transition, pressure change, docking, breach entry.

**Mechanics:** two doors with one interlock owner; only one side may be open unless the site is already depressurized or breached.

**Audio:** seal confirmation, pressure equalization, venting, warning cadence, hull resonance.

### 8.7 Damaged or jammed variant

This is a state/presentation variant, not a separate door architecture.

Possible resolutions:
- restore power;
- replace fuse;
- clear debris;
- use Gravity Gun on obstruction;
- cut mechanical lock;
- crawl through partial opening;
- breach once, permanently changing world state.

---

## 9. VR safety constitution for doors

1. A door never pushes, rotates, parents, or teleports the player.
2. A closing door checks body, head, hands, held items, and story-critical objects.
3. On obstruction, default behavior is **pause then reopen**.
4. No crush damage in ordinary player-facing doors.
5. Hands may not become trapped between collision volumes.
6. Visible open space must match collider clearance.
7. Passage width must account for held weapons and large salvage objects.
8. Force fields must never remain as invisible blockers after their effect disappears.
9. Usable and decorative doors must be visually distinguishable without relying on color alone.
10. Door controls must be reachable seated and standing where feasible.
11. A destination-transition door only requests `TravelCoordinator`; it does not call scene loading directly.
12. Airlock state must survive interruption, reload, and travel cancellation without opening both sides incorrectly.

---

## 10. Art direction

### 10.1 Doors should identify faction and condition at a glance

Use one mechanical grammar per culture or manufacturer:
- **Rustbucket / salvage:** mismatched plates, exposed repairs, manual overrides, uneven movement.
- **Industrial station:** standardized split bulkheads, hazard labels, service panels, robust seals.
- **Architect technology:** precise seams, quiet motion, impossible thinness, restrained light.
- **Bloom corruption:** mechanical door remains readable but growth interrupts seams, power, or motion.

### 10.2 Motion sells weight

The panel's acceleration, small hesitation, seal compression, track vibration, and final stop matter more than excessive polygon detail.

Suggested motion phases:
1. unlock movement;
2. seal release;
3. primary travel;
4. end damping;
5. lock confirmation.

### 10.3 Gravity-off visual vocabulary

Use a small curated set:
- loose tools and packaging;
- slow rotating salvage;
- hanging straps no longer hanging;
- condensation beads or dust drifting;
- floor arrows and orientation marks;
- inactive gravity emitters;
- magnetic-boot sparks or sole lights;
- objects collected against vents or filters;
- longer projectile and debris arcs;
- cables and fabric responding slowly.

The room must still have a readable floor, ceiling, and objective direction.

### 10.4 Gravity-restored payoff

When power returns:
- emitters light in sequence;
- floating debris aligns and settles;
- machinery bearings load up;
- loose straps fall;
- fluid or particles change behavior;
- a blocked route, lift, machine, or salvage container becomes usable.

The state change must affect gameplay, not only animation.

### 10.5 Force-field rendering on Quest

Prefer:
- simple layered transparent meshes;
- edge emission;
- scrolling normal/noise;
- local contact ripple;
- restrained distortion or no distortion;
- bounded particles at emitters.

Avoid expensive full-screen refraction, deep transparent stacking, or large overlapping particle sheets.

---

## 11. Audio direction

### 11.1 Door audio is a sequence, not one clip

Each powered physical door should support layers for:
- request/authorization;
- lock release;
- seal break;
- motor startup;
- movement loop;
- obstruction or jam;
- end stop;
- reseal;
- lock confirmation.

The same state machine can drive different material/faction profiles.

### 11.2 Use doors as acoustic portals

A closed bulkhead should alter sound from the next room:
- reduced level;
- low-pass filtering;
- material-dependent leakage;
- alarm or machinery transmitted through the hull;
- sound direction shifting toward the doorway as it opens.

Opening should reveal sound slightly before the full visual reveal when useful for anticipation.

### 11.3 Airlock sound vocabulary

- pressure warning;
- latch verification;
- gas flow or pump cycle;
- structural creak;
- suit/RILL confirmation;
- silence or suit-conducted mix only when the space is actually vacuum.

Low gravity inside a pressurized room is **not silent**.

### 11.4 Gravity restoration audio

Recommended progression:
1. dead room tone;
2. repair contact sounds;
3. generator spin-up;
4. rising low-frequency field hum;
5. warning count or RILL line;
6. small objects begin ticking against surfaces;
7. larger capped impacts;
8. stable machinery resonance and new ambient loop.

The audio should let a player understand the state change with eyes closed.

### 11.5 Force-field audio

The field's sound communicates its rule:
- pressure veil: soft continuous sheet and airflow;
- security barrier: sharper electrical pulse and denied-contact snap;
- containment field: unstable modulation and warning rhythm;
- authorized passage: pitch resolves before collision disables.

---

## 12. Gameplay uses for doors

Doors may provide:
- pacing and anticipation;
- sightline control;
- encounter containment;
- stealth routes;
- pressure or atmosphere logic;
- power-routing consequences;
- alternate access through repair, key, Gravity Gun, weapon, or crawl route;
- stateful return changes;
- render and audio culling;
- safe subscene/loading concealment;
- story evidence through damage, emergency sealing, or failed overrides.

Do not make every door a puzzle. Most should be quick, predictable infrastructure so special doors remain meaningful.

---

## 13. Proposed first hero mission: "Dead Weight"

Working title only.

### Mission spine

1. RILL identifies a drifting cargo tender with a valuable registry ping.
2. Player approaches through debris; optional one-drone exterior encounter.
3. Ship attaches to a damaged docking collar.
4. Manual wheel hatch establishes powerless-door interaction.
5. Interior emergency lights show gravity offline beyond the next bulkhead.
6. Split bulkhead opens into the floating cargo bay.
7. Player uses magnetic boots/baseline locomotion while props float.
8. Taser, Gravity Gun, throwables, and salvage interaction visibly behave differently.
9. One drone uses the room's floating cover.
10. Player restores the gravity relay.
11. Objects settle; a jammed cargo shutter becomes operable.
12. Behind it: salvage reward, story clue, or ship module.
13. Return path is visibly transformed and faster.
14. Extract to the hero ship.

### Why this is a good first orbital mission

It proves:
- one manual hatch;
- one powered split bulkhead;
- one cargo shutter;
- one object-gravity sector;
- one gravity-restoration beat;
- one enemy interaction;
- one stateful return path;
- one docking/extraction flow.

It does not require full player zero-g, a huge station, or many new enemies.

---

## 14. Proposed second hero mission: "Station Blackout"

Later mission pairing exterior and interior systems.

1. Defend or approach a failing station.
2. Dock under emergency conditions.
3. Restore one power branch.
4. Choose which subsystem receives limited power first:
   - security doors;
   - gravity;
   - life support;
   - cargo lift.
5. Different doors and routes become available.
6. Force-field containment fails or changes behavior.
7. Complete a short defense/repair encounter.
8. Station exterior visibly powers back up.

The mission reuses the same door and gravity contracts while demonstrating systemic choice.

---

## 15. Performance and Quest contract — proposed, not locked

### Hero orbital-interior target

- closed doors divide the site into small active cells;
- only adjacent rooms remain fully active;
- 8–12 active floating props in the hero gravity room;
- no more than 16 loose active rigidbodies without device evidence;
- existing world debris cap remains authoritative;
- primitive/compound colliders for moving props and doors;
- one transparent force-field layer in normal view, limited overlap;
- pooled sparks, dust, contact ripples, and impact VFX;
- door audio loops stop when stable;
- no per-frame allocations in gravity membership or obstruction checks;
- no room-local global `Physics.gravity` writes;
- every major opaque bulkhead considered for occlusion-portal treatment;
- 72 Hz target maintained through door reveal and gravity-restoration event.

### Physics stability requirements

- fixed-step forces only;
- capped object velocity before gravity restoration;
- deterministic cleanup on exit;
- projectiles cannot remain permanently registered after despawn;
- no duplicate sector membership;
- held objects retain stable grab behavior;
- story items cannot escape mission bounds.

---

## 16. Persistence contract

Persist high-level state:
- gravity repaired;
- power branch repaired;
- door unlocked/jammed/breached;
- salvage claimed;
- story item acquired;
- station objective completed.

Do not persist every floating prop transform by default.

On reload:
- derive door poses from authoritative state;
- respawn nonessential loose props from deterministic authored points;
- restore story-critical objects through their existing item/profile identity;
- apply sector state before loose bodies settle;
- prevent a repair reward from granting twice.

SaveSystem remains the only disk/profile owner.

---

## 17. Relationship to current owners

- `TravelCoordinator` remains the only travel owner.
- `PlayerRigPersistence` remains the persistent rig/fall-recovery owner.
- the existing locomotion owner remains the only player-motion writer.
- SaveSystem remains the only persistence owner.
- `ItemFactory` remains the canonical item/weapon construction path.
- `WORLD_PHYSICS_VARIANTS.md` remains the world-level profile authority.
- this document owns only the proposed **orbital-interior and room-local gravity/door design**.

A door may request travel but never load a scene directly.

A gravity sector may influence approved bodies but never become a second locomotion or save manager.

---

## 18. Bounded implementation envelopes after recovery exit

### OI-1 — Repository archaeology and contract lock

**Outcome:** exact inventory of any existing door, hatch, force-field, docking, pressure, gravity, and space-interior seams.

**Changes:** documentation and pure schemas/tests only if approved.

**Proof:** focused tests and ordinary CI.

### OI-2 — Door state core

**Outcome:** pure deterministic door state machine with access, power, pressure, jam, obstruction, and reload rules.

**No art required. No scene required.**

**Proof:** EditMode tests and ordinary CI.

### OI-3 — Door presentation kit

**Outcome:** three physical presentations using the same state owner:
- split sliding bulkhead;
- vertical shutter;
- manual wheel hatch.

Includes audio and occlusion adapters.

**Proof:** visual capture, focused PlayMode interaction tests, Quest reach/obstruction check.

### OI-4 — Object-gravity sector

**Outcome:** bounded room-local gravity affecting approved rigidbodies while player locomotion stays unchanged.

**Proof:** pure force/membership tests, PlayMode, clean proof because physics lifecycle is foundational, focused Quest stability test.

### OI-5 — "Dead Weight" graybox mission

**Outcome:** dock, enter, cross floating cargo bay, restore gravity, open cargo shutter, collect reward, return.

**Proof:** ordinary CI, route PlayMode, visual capture, Golden Android, targeted Quest mission.

### OI-6 — Force-field and airlock family

**Outcome:** one explicit force-field rule and one safe airlock-pair owner.

**Proof:** access/collision tests, audio/visual capture, Quest readability and obstruction check.

### OI-7 — Full player zero-g prototype, later only

**Outcome:** hands-first or rail/thruster-authored locomotion in a dedicated test annex.

**Proof:** foundational clean proof, multiple comfort presets, repeated headset sessions, immediate rollback boundary.

This envelope does not enter campaign content until physical comfort is proven.

### OI-8 — Space battle to station mission integration

**Outcome:** exterior encounter transitions into docking/interior objective and returns without duplicating travel or inventory ownership.

**Proof:** full Golden route plus Quest campaign.

---

## 19. Acceptance criteria

The program is succeeding when:

- Terry and the kids understand which doors are usable, locked, unpowered, dangerous, or decorative without explanation;
- no door traps or pushes the player, hands, or held item;
- closed bulkheads noticeably improve sightline, audio, and active-room control;
- the low-gravity room changes weapon, object, enemy, and traversal behavior—not only decoration;
- restoring gravity creates a memorable visual/audio/mechanical payoff;
- the first orbital site feels like a functional vessel rather than unrelated corridors;
- return traversal reflects the system the player repaired;
- state survives save/relaunch without duplicate rewards or broken door poses;
- the public range remains baseline physics while later physics tests use explicit pre-entry profiles;
- full player zero-g remains separated until it is comfortable on device.

---

## 20. Explicit non-goals

The first orbital-interior program does not require:
- a seamless explorable galaxy;
- every station room simulated simultaneously;
- whole-world zero gravity;
- inverted gravity;
- mid-combat global gravity storms;
- full decompression simulation;
- realistic fluid dynamics;
- hundreds of floating objects;
- online co-op synchronization;
- destructible every-door geometry;
- player crush damage;
- custom door code for every art style;
- a full player zero-g controller in the first mission.

---

## 21. Research basis

Primary/developer sources consulted:

1. Ready At Dawn / GDC — **It's All in the Hands: VR Animation and Locomotion Systems in Lone Echo**.
2. EA Motive — **Inside Dead Space #1: Remaking a Classic** and **Inside Dead Space #3: Aboard the Ishimura**.
3. EA Motive — **Dead Space Audio Developer Livestream**.
4. Arkane lead-designer interviews on Talos I's coherent, systemic station design.
5. Liz England — **The Door Problem**.
6. Unity 2022.3 documentation — `Physics.gravity`, `Rigidbody.AddForce`, local physics scenes, and multi-scene physics.
7. Unity documentation — Occlusion Portals and Audio Low Pass Filter.
8. Unity VR comfort guidance — avoid unexplained acceleration, preserve player agency, use sealed/occluded transitions where needed.

These examples inform the design; ZIPTIDE should still validate every comfort, performance, interaction, and readability decision on Quest.
