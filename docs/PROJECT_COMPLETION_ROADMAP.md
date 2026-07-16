# ZIPTIDE — PROJECT COMPLETION ROADMAP

**Purpose:** one honest map from the current recovery foundation to a cohesive, premium Quest game.  
**Status date:** 2026-07-16  
**Branch of record:** `terry-local-wip`  
**Execution rule:** this roadmap defines the destination and order. `docs/HANDOFF.md`, exact-SHA recovery observations, and the current sprint board define the live micro-step.

---

## 0. How to read this roadmap

The project has enough code that the word **done** has become dangerous. A system can have definitions, factories, tests, authors, and build hooks while still being invisible, confusing, ugly, uncomfortable, or absent in the headset. This roadmap separates those conditions.

### Maturity labels

- **PROVEN** — executed in the real canonical route with inspectable evidence. For headset-dependent qualities, this requires a recorded Quest pass.
- **IMPLEMENTED / DEVICE-UNPROVEN** — meaningful runtime code and automated tests exist, but the player-facing behavior has not passed the required headset session.
- **GRAYBOX** — the loop can be exercised, but presentation, content, readability, encounter design, or authored geometry remains placeholder quality.
- **BACKEND-ONLY** — data, simulation, saving, or economy exists without an understandable physical player surface.
- **UNBUILT** — the required player-facing system or content does not exist.
- **DEFERRED** — intentionally outside the current release path.

### Quality target

“AAA” here means **premium-indie Quest-native execution**, not the asset volume of a 500-person studio:

- stable Quest frame pacing;
- coherent, cinematic art direction;
- excellent interaction and combat feel;
- readable, physically understandable VR interfaces;
- memorable creatures, weapons, places, sound, and story delivery;
- systemic variety that makes the game feel larger than its bespoke asset count;
- no major pillar left at prototype quality beside polished neighbors.

### The floor-raising rule

Do not keep polishing a strong backend while a player-facing dependency remains skeletal. The next task should usually raise the weakest pillar required by the next complete playable slice.

---

# PART I — CURRENT TRUTH

## 1. Recovery and architecture

### Current state

- **PROVEN in automation:** one travel owner, one persistent rig/input session, save/profile ownership, Golden exposure profile, runtime census, real boot/new-game interaction, W000 → ToxicCity → W000 travel/save route, visual snapshots, UI spatial evidence, spawn clearance, material/fallback audits, and Linux reference performance artifacts.
- **Still blocking headset authorization at this document’s first draft:** the unchanged 43-test route intermittently reproduces an Input System `ApplyProcessors` null-reference immediately after travel. The latest bounded ownership correction must return 43/43 with ordinary CI and Golden Android green on the same exact source before a Quest checkpoint is issued.
- **Recovery freeze remains active:** no broad feature expansion is allowed to obscure the exact device candidate.

### Completion gate

- [ ] Exact source SHA records 43/43 PlayMode, zero skips/inconclusive.
- [ ] Ordinary EditMode + patch/world audit are green on that source.
- [ ] Golden Android is green on that source with `_Boot`, `W000_DriftIn`, and `ToxicCity` only.
- [ ] Build report proves `GoldenSlice`, `ZIPTIDE_RECOVERY_GOLDEN`, Forge bake, shader-safety gate, required hook failures, scene lock, nonempty APK, and checksums.
- [ ] Raw screenshots, UI reports, spawn reports, material reports, census, travel/save, and R1.10 performance artifacts are independently reviewed.
- [ ] The immutable APK is installed and passes the bounded Quest recovery checklist.
- [ ] Recovery exit report names what is proven, what remains device-only, and what work may resume.

### Architecture after recovery

Preserve these owners unless a Terry-approved architecture packet proves replacement:

- `TravelCoordinator` — all scene/world travel.
- `PlayerRigPersistence` — persistent rig and physical input-session integration.
- `PlayerInputSessionGuard` — duplicate input manager consolidation.
- `SaveSystem` — disk/profile ownership.
- `BootLoader` — cold boot and Home Hub.
- Recovery exposure profile — what is permitted in Golden/device checkpoints.
- Definition → registry → factory → runtime owner → audit/test pipeline for scalable content.

---

## 2. Honest project maturity summary

| Pillar | Current maturity | Player-facing truth |
|---|---|---|
| Architecture, saves, travel, factories | IMPLEMENTED → increasingly PROVEN | Strong foundation; recovery is exposing remaining lifecycle collisions. |
| Ship as home/travel hub | GRAYBOX | Boardable and configurable code exists, but the last device impression was a boxy placeholder rather than a convincing home ship. |
| Free flight and space combat | IMPLEMENTED / DEVICE-UNPROVEN | SpaceLane, controls, rings, drones, stun and salvage exist in code; not yet a proven, polished campaign pillar. |
| Ground weapons/shooter combat | GRAYBOX | Arsenal and hit systems exist; weapon handling, feedback, enemies, encounter design, presentation, and breadth are far below the desired shooter bar. |
| Creatures/robots/drones | IMPLEMENTED / UNEVEN GRAYBOX | Framework, bodies, several behaviors and readable-state catalogs exist; identity, motion, animation, encounters and device readability remain uneven. |
| Worlds/levels | GRAYBOX | World factory and large terrain exist; authored architecture, silhouette, navigation, landmark hierarchy, material richness and lived-in detail remain weak. |
| Skyscapes/atmosphere | IMPLEMENTED / DEVICE-UNPROVEN | Canonical sky system exists; celestial bodies can still read as placed spheres rather than a unified distant sky. |
| Gardens/ecology | BACKEND + EARLY PHYSICAL SURFACES | Genetics, growth, watering and 24 plant specs exist; breeding, giant crops, seed discovery, gardening fantasy and feedback are not yet a coherent loop. |
| Factories/belts/building | BACKEND + GRAYBOX | Economy, production graph, machines, belts and build sockets exist; onboarding, visual language, placement, recipes and satisfying machinery remain unclear. |
| Vehicles/traversal | IMPLEMENTED / DEVICE-UNPROVEN | Multiple movement verbs and three starter rides exist; tuning, content breadth, garage and role in missions remain open. |
| Story/RILL/transmissions | DATA-RICH / PRESENTATION-THIN | Canon and line systems are strong; story delivery, VO, staging, cinematic moments and world integration lag. |
| UI/onboarding/accessibility | GRAYBOX | Home Hub, boards, comfort and audits exist; consistency, discoverability, reach, readability, save UX and complete onboarding remain open. |
| Audio/VFX/animation | EARLY | Procedural ambience and some effects exist; full weapon, creature, environment, music, VO and cinematic layers are not present. |
| PvP/Tidefront/online | LARGE CODE BASE / DEVICE-UNPROVEN | Modes, bots, arenas and conquest simulation exist; online combat sync, room UX, two-headset proof and polish remain open. |
| Release/store | UNBUILT → EARLY CHECKLIST | App ID, entitlement, privacy, keystore, permissions, store media, certification and QA remain. |

---

# PART II — PRODUCT PILLARS AND DETAILED CHECKLISTS

## 3. The first complete product slice

The next goal is not “add more systems.” It is one **shipped-quality vertical slice** proving the entire game can feel coherent:

**Cold boot → Home/ship → W000 onboarding → travel → ToxicCity/W001-quality world → scan/repair/combat/garden or production beat → reward → return to ship → visible upgrade/save payoff.**

### Slice acceptance

- [ ] A new player understands where they are, what they can touch, and what to do without developer explanation.
- [ ] The ship reads as a designed, inhabitable vehicle—not assembled primitives.
- [ ] Travel is reliable, comfortable, visually intentional, and never leaves controls dead.
- [ ] One ground encounter reaches the complete combat-quality bar below.
- [ ] One repair/automation interaction is physically legible and satisfying.
- [ ] One garden interaction is discoverable and visibly changes the world/profile.
- [ ] RILL/story delivery gives the actions meaning.
- [ ] The destination has one strong skyline, landmark hierarchy, route, interior, creature identity, soundscape and payoff.
- [ ] Return to the ship exposes a meaningful reward or upgrade.
- [ ] The whole loop survives save/relaunch and repeated travel.
- [ ] Quest performance and comfort remain within the agreed budget.

Do not scale to dozens of worlds until this slice is excellent. Every system template used here becomes the pattern for later content.

---

## 4. Quest interaction, controls, comfort and physical presence

### Current state

Locomotion, snap/smooth turn, sprint, crouch/slide, jump, dash, climbing, zipline, lift, jump pad, grapple, hands, rays, grab, holsters and comfort presets exist. Recovery has proven many ownership contracts in Linux PlayMode, not physical controller feel.

### Completion checklist

#### Reliability

- [ ] Head and hands track correctly from cold boot through every travel.
- [ ] No input action is double-owned, disabled unexpectedly, or rebuilt under a live reader.
- [ ] Move, continuous turn and snap turn work immediately after every travel.
- [ ] Menus/ship seats/vehicles suspend only the intended locomotion owners and restore exactly once.
- [ ] Dropped/holstered objects survive travel and save as designed.
- [ ] Fall recovery, respawn and standing height work in every shipped environment.

#### Feel

- [ ] Grip poses match each object’s actual handle and center of mass.
- [ ] Throwing uses believable velocity without dead drops or accidental launches.
- [ ] Holster acceptance/rejection is obvious through pose, sound, highlight and haptic response.
- [ ] Rays are stable, short enough, and do not fight direct interaction.
- [ ] Every primary verb has one bounded, correct-hand haptic signature.
- [ ] Seated and standing play are both deliberately supported or the unsupported mode is clearly stated.
- [ ] Left/right-handed options, turn options, vignette strength, subtitle size and haptic scale are player-visible.

#### Comfort

- [ ] No code moves the camera transform directly.
- [ ] Artificial motion reports to the comfort owner.
- [ ] Flight, vehicles, lifts, jump pads, grapples and ziplines have tested comfort profiles.
- [ ] No unexpected roll/horizon tilt outside explicit opt-in flight maneuvers.
- [ ] Travel flash/crest timing is comfortable and cannot trap the player inside an opaque effect.

### Next best slice

Recovery Quest checkpoint first; then one dedicated controls/weapon handling session in W000/ToxicCity, not a giant all-system test.

---

## 5. The ship: home, world selector and progression sink

### Current state

Ship definitions, boardable shell, helm, quarters, customization, modules, liveries, decals, fly-out presentation and SpaceLane code exist. The last player-facing verdict described the hull as boxy and “poor Roblox.” The ship is not yet the unquestioned home and central navigation surface.

### Product standard

The player should recognize their ship by silhouette, feel ownership of it, understand every compartment, see upgrades change it, and use it as the natural bridge between ground worlds, space missions, inventory, story and progression.

### Detailed checklist

#### Exterior

- [ ] One canonical hero silhouette with clear nose, propulsion, landing/berth logic and scale cues.
- [ ] No exposed primitive-box construction in the shipped hull.
- [ ] Materials communicate hull panels, glass, engines, wear, repair and faction history.
- [ ] Landing gear/berth contact and door placement make physical sense.
- [ ] Navigation lights, engine idle, heat, hum and small motion make it feel alive.
- [ ] LODs, collision and Quest draw-call/triangle budgets are proven.

#### Interior

- [ ] Cockpit, quarters, cargo/workbench and travel path are spatially coherent.
- [ ] Player can board, move, sit, stand and leave without teleport confusion or clipping.
- [ ] Windows show an intentional exterior/sky treatment.
- [ ] Panels are readable from their expected body position and hidden outside their compartment.
- [ ] Storage, cosmetics, loadout, ship upgrades and story objects have physical homes.
- [ ] RILL and narrative moments use the ship as a recurring character space.

#### World selection and travel

- [ ] Ship becomes the primary world selector for the shipped campaign band.
- [ ] Locked destinations clearly show requirement and progress.
- [ ] Destination preview communicates biome, mission, hazards and expected rewards.
- [ ] Confirm/cancel is clear and cannot accidentally double-travel.
- [ ] Fly-out/fly-in surrounds—but never bypasses—`TravelCoordinator`.
- [ ] Door-first placeholder travel is retired where the ship is intended to own the fantasy.

#### Upgrades and economy

- [ ] Engine, scanner, cargo, defense, weapon and comfort upgrades have visible sockets or refit surfaces.
- [ ] Every upgrade changes a real stat and a visible/audio characteristic.
- [ ] Costs are balanced against missions, salvage, gardens and factories.
- [ ] Upgrade state persists and migrates safely.
- [ ] At least one early reward visibly improves the ship during the first complete slice.

### Next best slice

Rebuild one hero ship to shipped silhouette/interior quality and make it own W000 ↔ first destination travel. Do not author a fleet until this single ship passes device review.

---

## 6. Flight and space battles

### Current state

Dedicated SpaceLane free flight, throttle/reverse, strafe, boost, pitch, snap yaw, roll, ring course, stun bolts, drones and salvage exist in code. First bake/device proof and campaign integration are incomplete.

### Product standard

“Star Wars flight” means immediate readable motion, strong cockpit reference, cinematic sound and vistas, enemies with distinct roles, satisfying aim/impact, mission purpose and comfort—not merely a vehicle controller in empty space.

### Detailed checklist

#### Flight feel

- [ ] Cockpit frame gives stable horizon/reference and readable velocity.
- [ ] Throttle, reverse, strafe, pitch, yaw, boost and roll have distinct, tunable response curves.
- [ ] Acceleration and braking communicate ship mass without sluggish input.
- [ ] Speed has visual language: particles, streaks, engine sound, vibration and passing scale objects.
- [ ] Vignette/comfort behavior is correct for every maneuver.
- [ ] Dock/undock and seat transitions never move or parent the rig incorrectly.

#### Space environment

- [ ] SpaceLane has near/mid/far depth, not objects floating against a flat backdrop.
- [ ] Celestial bodies obey scale, haze, occlusion and light direction.
- [ ] Asteroids/debris/stations provide parallax, cover, route and scale.
- [ ] Mission landmarks and navigation cues are visible without a flat HUD maze.
- [ ] Audio sells engine state, impacts, threats and objective changes.

#### Combat

- [ ] At least three enemy roles: pursuer/interceptor, ranged pressure, heavy/support.
- [ ] Each role has unique silhouette, engine trail, telegraph, movement and counter.
- [ ] Targeting is readable in VR without excessive aim assist.
- [ ] Fire cadence, projectile travel, hit confirmation, disable state and salvage are satisfying.
- [ ] Enemies coordinate without overwhelming Quest CPU or player comfort.
- [ ] Mission types include escort/intercept/disable/salvage/defense/ring pursuit—not only target shooting.
- [ ] Non-lethal law is visible: disabled craft behave differently from destroyed craft.
- [ ] Rewards feed ship progression and ground-game economy.

#### Performance and persistence

- [ ] Stable device frame pacing under maximum supported enemies/effects.
- [ ] Object pooling for projectiles/enemies/debris where measured hot paths justify it.
- [ ] No retained scene audio/material/mesh growth after repeat entry/exit.
- [ ] Mission state and rewards save atomically.

### Next best slice

One polished SpaceLane mission: launch from the hero ship, fly through a visually rich route, fight two clearly different enemy roles, salvage one disabled target, return and install a visible upgrade.

---

## 7. Ground shooter combat and weaponry

### Current state

Multiple guns, melee weapons, unified non-lethal rules, hit sources, charge concepts, lasers, pickups, Forge recipes and tests exist. The player-facing experience is not yet at the requested shooter standard.

### The “Call of Duty-level” measurable bar

The project should not claim this bar from weapon count. One weapon and one encounter must first satisfy all seven layers:

1. **Handling** — grip, two-hand support where appropriate, aim, recoil, reload/charge and holster feel intentional.
2. **Identity** — silhouette, sound, cadence, range, projectile, impact and tactical purpose are unmistakable.
3. **Feedback** — muzzle/beam, controller haptic, audio transient, target reaction, surface effect and clear hit/disable confirmation.
4. **Opponent reaction** — stagger, dodge, armor break, state change, weak point and non-lethal resolution respond to where/how the player acts.
5. **Encounter design** — cover, movement, verticality, pressure waves, flanks and objectives create decisions.
6. **Presentation/performance** — animation, effects and sound remain clean at Quest frame budget.
7. **Breadth/balance** — only after the first six are proven, expand the arsenal without redundant roles.

### Weapon checklist

#### Core handling

- [ ] Correct one- and two-handed grip poses.
- [ ] Physical/visual safety against self-hit and barrel clipping.
- [ ] Stable aim ray/sight aligned to actual muzzle.
- [ ] Recoil/charge/heat/energy behavior with clear recovery state.
- [ ] Reload or recharge interaction appropriate to each weapon fantasy.
- [ ] Drop, throw, holster, quick-swap and persistence all work.
- [ ] Left-hand use is supported or clearly restricted by design.

#### Feedback stack

- [ ] Weapon-specific muzzle/beam/projectile effect.
- [ ] Layered close/distant/reverb audio.
- [ ] Correct-hand haptic with bounded amplitude/duration.
- [ ] Impact effects differentiated by material and target type.
- [ ] Hit/weak-point/armor/disable feedback readable without flat-screen UI dependence.
- [ ] Empty/overheated/uncharged state is obvious before trigger pull.

#### Arsenal roles

- [ ] Starter taser: dependable close/mid-range disable baseline.
- [ ] Scrap pistol: precise cadence and clear weak-point role.
- [ ] Gravity tool: manipulation/control identity, not another damage gun.
- [ ] Static net: area denial/control.
- [ ] Sonic thumper: close-range crowd/structure interaction.
- [ ] Prism beam: charge/lane/precision identity.
- [ ] Breaker blade and Tide Pike: contact/reach roles with reliable collision.
- [ ] Ship weapons remain separate definitions and balance domain.
- [ ] Every new weapon has a counter-role and no duplicate niche.

### Encounter checklist

- [ ] Enemies enter from understandable positions; no cheap spawn behind player.
- [ ] Telegraph windows are fair in VR.
- [ ] Cover and traversal support player movement instead of static shooting galleries.
- [ ] Objectives require more than clearing health bars.
- [ ] Difficulty changes behavior, coordination and timing—not only health.
- [ ] Combat ends with readable non-lethal resolution and reward.
- [ ] Tutorial introduces one mechanic at a time through action, not text walls.

### Next best slice

Pick one hero weapon—likely the taser—and one signature enemy encounter. Bring handling, animation, sound, impact, AI reaction, arena layout and reward to the full bar before expanding the arsenal.

---

## 8. Creatures, bugs, aliens, robots and drones

### Current state

A data-driven creature framework, base archetypes, several novel behavior concepts, forged/skinned bodies, gaits, breathing and state-readability gates exist. Quality is uneven; some bodies/behaviors are strong in turnarounds while actual world placement, grounding, motion and encounter readability remain device-dependent.

### Product standard

Every shipped species must be recognizable from silhouette and motion alone, have an ecological reason to exist, telegraph fairly, require a distinct counter, react physically, and resolve non-lethally. Robots/drones must follow the same identity standard rather than being palette swaps.

### Detailed checklist per species

#### Identity passport

- [ ] Unique silhouette at gameplay distance.
- [ ] Scale and locomotion suited to biome/physics.
- [ ] Palette/material/eye or signal language distinct from neighboring species.
- [ ] Idle, locomotion, alert, attack, stunned and resolved poses readable.
- [ ] Evolution/ecology note and story/faction connection documented.
- [ ] Sound vocabulary distinct enough to identify off-screen.

#### Behavior and fairness

- [ ] At least three active states visible in real play.
- [ ] Pre-action telegraph with measured minimum duration.
- [ ] Existing tool/weapon counter.
- [ ] Separate disable/resolution state.
- [ ] No camera yank or unavoidable close-range surprise.
- [ ] Navigation and grounding work on real terrain, slopes, walls or air as appropriate.
- [ ] Group behavior remains performant and legible.

#### Content roles

- [ ] Ambient/non-hostile life that makes worlds feel inhabited.
- [ ] Territorial/defensive species.
- [ ] Mobile pressure species.
- [ ] Heavy/space-control species.
- [ ] Warden lawful-enforcer behavior tied to Signal.
- [ ] Robot/drone roster with distinct scout, suppressor, repair/support and heavy roles.
- [ ] Signature species for each major biome/chapter rather than one global spawn table.

#### Presentation

- [ ] Animation blends and turns do not slide or float.
- [ ] Feet/tentacles/bodies contact the environment.
- [ ] Hit zones and reactions match anatomy.
- [ ] State VFX/material changes are readable but not noisy.
- [ ] Spawn/despawn/return behavior fits ecology.
- [ ] Effects, sounds and counts fit Quest budgets.

### Next best slice

Complete the first signature-creature passport in an actual destination scene, then build the missing first-hour creature-resolution adapter around the existing neutral resolution event. Use that species as the template for every later creature.

---

## 9. Worlds, maps, architecture and level design

### Current state

WorldSpec/compiler, terrain, POIs, paths, scatter, building grammar, interiors, hazards and audit gates exist. W002–W012 generate as graybox. The last headset impression was large boxy architecture, thin detail and worlds that did not yet feel authored.

### Product standard

A generated world must still feel intentionally directed: strong arrival reveal, recognizable skyline, layered route choices, meaningful landmarks, human-scale architecture, interior/exterior logic, environmental storytelling and a distinct gameplay rhythm.

### Detailed checklist

#### Macro composition

- [ ] One readable hero landmark visible at arrival.
- [ ] Clear near/mid/far composition and atmospheric depth.
- [ ] Terrain silhouette unique to the world.
- [ ] Destination has a recognizable skyline, not repeated blocks.
- [ ] Critical routes and optional exploration read from terrain/lighting/architecture.
- [ ] World edges feel natural and intentional.

#### Route and pacing

- [ ] Main route has a deliberate sequence of reveal, task, escalation and payoff.
- [ ] Optional branches contain worthwhile reward/story/interaction.
- [ ] Breadcrumbs are world-authentic, not generic repeated markers.
- [ ] Traversal verbs have authored opportunities and alternate routes.
- [ ] No long empty walks or cluttered obstacle spam.
- [ ] Return paths and shortcuts respect VR comfort and time.

#### Architecture

- [ ] Buildings use authored module families with clear structural logic.
- [ ] No shipped facade reads as scaled cubes with arbitrary windows.
- [ ] Doors, floors, stairs, rails, cover and ceilings are human/alien-scale and navigable.
- [ ] Interiors connect logically to exteriors.
- [ ] Rooms have purpose, furniture/props, lighting and story residue.
- [ ] Repetition is broken by modules, damage, signage, color, roofline and local adaptation.
- [ ] Collision matches visible geometry.

#### Environmental interaction

- [ ] At least one repair/production verb tied to the place.
- [ ] At least one creature/ecology moment.
- [ ] At least one story object or line.
- [ ] At least one optional resource/reward interaction.
- [ ] Reactive props/environment where the player expects response.

#### Audit and device gate

- [ ] Reachability, slopes, spawn, collision, POIs, story anchor and estimated play time pass.
- [ ] Art-conformance gate rejects prototype primitives in shipped profiles.
- [ ] Occlusion/LOD/light/material budgets pass.
- [ ] Headset walk confirms navigation, scale, comfort and landmark readability.

### Next best slice

Do not broadly re-author twelve worlds. Bring one destination—ToxicCity/W001 or the chosen Chapter 1 proof—to shipped-quality architecture, route, landmark, interior, creature and audio. Convert its lessons into factory recipes and gates, then regenerate the band.

---

## 10. Skyscapes, celestial bodies, lighting and atmosphere

### Current state

SkyVista data, procedural textures, bodies, Shell progression, gradients, stars, fog and grading exist. The player’s concern is accurate: planets can read as objects placed in the scene rather than immense bodies integrated into a distant atmosphere.

### Product standard

The sky should create scale and emotion before the player moves. Celestial bodies must feel astronomically distant, partially lost in haze/light, and connected to the ground’s illumination and color.

### Detailed checklist

- [ ] Horizon haze and aerial perspective tie far terrain to the sky.
- [ ] Celestial bodies use scale, limb shading, atmospheric rim and occlusion appropriate to distance.
- [ ] Body sharpness/contrast decreases correctly relative to nearby objects.
- [ ] Lighting direction and ground color agree with visible suns/moons/planets.
- [ ] Clouds/nebulae/stars have depth layers and slow parallax/drift where appropriate.
- [ ] No body looks like a small sphere hovering near the level.
- [ ] Signature sky composition is framed by terrain/architecture at arrival.
- [ ] Day/time/weather changes are used only where they serve gameplay/story and remain performant.
- [ ] Each world has a distinct palette and sky silhouette while respecting canon progression.
- [ ] Banding, stereo artifacts, horizon seams and overbright white clipping are absent on Quest.
- [ ] Sky budgets remain bounded by the established draw/material/texture limits.

### Next best slice

Choose one “stop and stare” arrival and tune sky, haze, lighting, terrain framing and audio together on-device. Use a side-by-side screenshot rubric rather than modifying celestial objects in isolation.

---

## 11. Gardens, plants and ecology

### Current state

Twenty-four plant specs, growth, offline time, genetics, cross-pollination, watering, tending, freshness and economy hooks exist. Most seed surfacing, breeding, giant-crop presentation and the larger gardening fantasy remain incomplete.

### Product standard

Gardening must be a physical, readable, rewarding loop: discover/obtain seed → prepare/plant → water/tend → visibly change → harvest/breed → use or sell output → unlock a new possibility. Genetics must be visible, not hidden spreadsheet math.

### Detailed checklist

#### Discovery and onboarding

- [ ] Starter seeds are physically introduced through story/world rewards.
- [ ] Seed packets/specimens have recognizable visuals and labels.
- [ ] Almanac/greenhouse surface shows discovered traits without overwhelming text.
- [ ] First planting teaches soil, water, time and harvest through action.

#### Physical interactions

- [ ] Soil bed clearly shows empty/valid/occupied states.
- [ ] Seed placement feels physical and confirms acceptance.
- [ ] Watering can stream, level, refill and haptics are readable.
- [ ] Pruning/tending uses a distinct hand action and visible plant response.
- [ ] Harvest interaction matches plant form and produces a physical output.

#### Visible growth and genetics

- [ ] Stages differ in silhouette, scale, color and motion.
- [ ] Traits alter visible characteristics and output.
- [ ] Cross-pollination has a physical spatial rule the player can understand.
- [ ] Giant crops are actual large interactable results with special handling/reward.
- [ ] Rare mutations have clear cause/evidence rather than invisible RNG.
- [ ] Ecology/world conditions influence growth in understandable ways.

#### Economy and purpose

- [ ] Outputs feed recipes, ship upgrades, tools, missions or ecology restoration.
- [ ] Prices/time/yield avoid idle-game grind.
- [ ] Garden improvements are meaningful progression sinks.
- [ ] Offline growth and interrupted saves are safe and clearly reported.

### Next best slice

One complete three-species garden: starter seed, visible trait cross, giant result, physical harvest, and one factory/ship use. Do not surface all 24 plants until this loop is clear and fun.

---

## 12. Factories, belts, conveyors, crafting and base building

### Current state

Resource registry, economy ledger, recipes, production graph, machines, build sockets, extractors, physical belts, cloning/persistence and riding exist. The user-facing state is ugly and indiscernible: the simulation is ahead of its visual language and tutorial.

### Product standard

A player should understand inputs, direction, machine state, blockage, output and purpose by looking at the physical setup. Building must feel like assembling useful salvage machinery, not placing anonymous cubes.

### Detailed checklist

#### Visual language

- [ ] Inputs, outputs and accepted resource types have consistent physical shapes/colors/icons.
- [ ] Belt direction is obvious while idle and moving.
- [ ] Machine states—unpowered, waiting, working, blocked, complete, damaged—are visibly distinct.
- [ ] Moving parts correspond to actual work.
- [ ] Pipes/cables/sockets visibly connect ownership and flow.
- [ ] Prototype cubes are replaced with coherent salvage-industrial modules.

#### Placement and editing

- [ ] Placement preview shows valid/invalid collision and connection.
- [ ] Snap sockets are forgiving without unexpected rotations.
- [ ] Rotate/move/delete/refund controls are explicit and comfort-safe.
- [ ] Player cannot accidentally manipulate world anchors with locomotion controls.
- [ ] Built systems persist exactly through travel/save/reload.
- [ ] Blueprint/stamp workflow is understandable and bounded.

#### Production loop

- [ ] One complete recipe starts from world resource extraction and ends in a useful item/upgrade.
- [ ] Bottlenecks and blockages are readable and repairable.
- [ ] Throughput/time are tuned for active VR play, not long waiting.
- [ ] Manual intervention provides meaningful speed/quality benefit.
- [ ] Recipe discovery and unlocks have physical/story context.
- [ ] Economy cannot duplicate resources through travel, cloning or interrupted saves.

#### Tutorial and purpose

- [ ] First machine teaches one input → one motion → one output.
- [ ] First belt route is short, visible and rewards immediate experimentation.
- [ ] Repair mechanics connect naturally to later factory maintenance.
- [ ] Expansion supports ship, garden, weapons and mission preparation.

### Next best slice

A single polished “salvage cell” chain: extractor → visible belt → repairable processor → stun-charge output → load/use output in a weapon or ship upgrade. Make it beautiful and obvious before adding factory scale.

---

## 13. Vehicles and traversal

### Current state

Ground vehicles, flight, climbing, ziplines, lifts, jump pads and grapple exist in code; most need device tuning and mission integration. Three ride archetypes are surfaced, three are missing, and no complete garage/catalog exists.

### Checklist

- [ ] Each vehicle has a distinct role, silhouette, speed, handling and biome reason.
- [ ] Mount/dismount is clear, safe and never strands the player.
- [ ] Controls align with ship/comfort conventions.
- [ ] Terrain following, collision and soft boundaries are robust.
- [ ] Vehicle missions justify use rather than being optional toys beside walking routes.
- [ ] Garage/catalog lets the player view, unlock, choose, recolor and upgrade rides.
- [ ] Rover, GravSled and Walker gaps are deliberately filled or removed from launch scope.
- [ ] Traversal verbs are taught through authored routes and have meaningful shortcuts/rewards.
- [ ] Every artificial-motion verb has device comfort and haptic review.
- [ ] Save/travel restoration returns the player and vehicle to valid states.

### Next best slice

One destination designed around a single vehicle or traversal verb, with an objective and payoff that cannot be reached as well by ordinary walking.

---

## 14. Story, missions, RILL and cinematic delivery

### Current state

The Story Bible, Transmission, characters, world records, four endings, RILL line system, flags, choices and many mission/data hooks exist. Presentation is subtitle-heavy, VO absent, staging thin, and many worlds do not yet carry their authored story as a memorable playable scene.

### Checklist

#### First hour

- [ ] Home/ship establishes player, RILL, quarantine mystery and immediate goal.
- [ ] Fifteen teaching lines occur only when relevant and never nag veterans.
- [ ] Scan, holster, travel, repair, creature resolution and zipline are learned through play.
- [ ] Signature creature is introduced and resolved non-lethally.
- [ ] First destination has a visible ship/story payoff.
- [ ] Reload/continue resumes gracefully without repeating completed teaching.

#### Mission design

- [ ] Each shipped world has a clear arrival question, physical task, escalation and ending beat.
- [ ] Mission verbs use the actual world systems rather than generic checklist interactions.
- [ ] Optional exploration yields story, tools, ecology or meaningful resources.
- [ ] Choices have visible immediate and later consequences.
- [ ] Signal changes threats, world state and RILL response.
- [ ] Failure/decline/walk-away states are authored and recoverable.

#### Character delivery

- [ ] RILL, Cal, Mara, Sable and Nine are identifiable by language and performance.
- [ ] RILL memory changes affect lines and behavior.
- [ ] Subtitles are readable, positioned comfortably and never overlap critical UI.
- [ ] VO pipeline supports replacement/localization without scene edits.
- [ ] Ambient lines respect cooldown, context and player activity.
- [ ] Major moments use blocking, light, sound, environment and player agency—not only text.

#### Campaign

- [ ] Chapter 1 is fully staged before W013+ production scales.
- [ ] New chapter systems are introduced with the worlds that require them.
- [ ] Transmission fragments are physical, collectible and meaningfully de-garbled.
- [ ] Branch flags and four endings have end-to-end tests and actual playable staging.

### Next best slice

Close the first-hour dependency chain: signature creature presentation → neutral resolution adapter → W001 final orchestration → device proof.

---

## 15. UI, onboarding, accessibility and haptics

### Current state

Diegetic boards, Home Hub, comfort console, dev menu, HUD/subtitle systems and a WARN-level spatial audit exist. UI is not yet a single coherent product language; several surfaces remain hard to understand or reachable only through developer knowledge.

### Checklist

#### UI language

- [ ] One typography, color, depth, interaction and disabled-state system for player-facing surfaces.
- [ ] World-space text has consistent readable face, size, distance and contrast.
- [ ] Interactive targets are obvious without permanent laser clutter.
- [ ] Labels explain action/result, not internal system names.
- [ ] Save, currency, inventory and objective state appear where the player expects.
- [ ] Dev/debug surfaces are excluded from release profile.

#### Onboarding

- [ ] Every core pillar has a physical first-use moment.
- [ ] Hints trigger on hesitation, not on fixed spam timers.
- [ ] Experienced players can skip/reduce teaching.
- [ ] No instruction depends on knowing an undocumented button chord.
- [ ] Failure states explain recovery without breaking immersion.

#### Accessibility

- [ ] Seated/standing mode.
- [ ] Left/right-handed support.
- [ ] Snap angle, smooth turn, movement speed and vignette controls.
- [ ] Subtitle size/background/position options.
- [ ] Color-safe objective/enemy states.
- [ ] Haptic strength and reduced-motion options.
- [ ] Audio cues have visual equivalents where gameplay-critical.

#### Haptics

- [ ] Grab/select/holster/release.
- [ ] UI hover/select/error.
- [ ] Weapon fire/charge/empty/impact.
- [ ] Repair step/success/failure.
- [ ] Garden pour/tend/harvest.
- [ ] Climb/zip/grapple/vehicle/flight state.
- [ ] Correct hand and no duplicate XRI + custom pulse.

### Next best slice

Unify the Home Hub, ship helm, one mission kiosk, one machine and one garden surface into the same interaction/readability system; promote audit findings to blockers only after real Quest calibration.

---

## 16. Art direction, materials, animation, VFX and audio

### Current state

Forge, textures, bakes, class budgets, building modules, creature bodies, SkyVista, grade, practical lights and water proof exist. The game still contains graybox architecture, weak grounding, incomplete water, limited reactive props, early VFX/audio, and no full VO/adaptive score.

### Checklist

#### Art conformance

- [ ] Every shipped visible object resolves to an approved authored/Forge/registry source.
- [ ] No null/error materials or runtime emergency replacement surfaces.
- [ ] Prototype primitives are forbidden in shipped scenes unless explicitly art-approved.
- [ ] Material families are coherent across world, ship, weapons and creatures.
- [ ] Hero assets use enough of their class budget to avoid visibly underbuilt forms.

#### Grounding and life

- [ ] Contact shadows/decals prevent floating assets.
- [ ] Wear, dirt, leaks, cables, debris and local variation tell how a place functions.
- [ ] Water has runtime surface, depth, edge foam, reflection strategy and interaction.
- [ ] Props react to touch, impact, wind, machines and creatures.
- [ ] Animation includes idle micro-motion, weight shifts, recoil, breathing and state transitions.

#### VFX

- [ ] Coherent vocabulary for player, weapon, enemy, travel, machine, garden and environment effects.
- [ ] Effects reinforce gameplay state before spectacle.
- [ ] Overdraw, particles and simultaneous systems obey Quest caps.
- [ ] No effect obscures required VR visibility or causes comfort issues.

#### Audio

- [ ] Weapon-specific fire, mechanism, impact and tail layers.
- [ ] Creature-specific idle/telegraph/action/stun/resolution vocabulary.
- [ ] Ship/vehicle engine state and cockpit/exterior treatment.
- [ ] Machine working/blocked/damaged/completed states.
- [ ] Biome beds plus hazard and encounter stingers.
- [ ] Signal-reactive/adaptive music stems.
- [ ] RILL/Transmission/character VO with subtitle timing and ducking.
- [ ] Resource cleanup over repeated travel/device soak.

### Next best slice

Finish the W001/Chapter 1 proof destination as a full art-and-sound package: water, grounding, signature creature, architecture modules, sky/grade, reactive props, VFX, ambience and first VO-quality line pass.

---

## 17. Progression, economy and customization

### Current state

Atomic profiles, one economy, RewardRouter, ledgers, progression, ship modules, cosmetics, PvP career, conquest, garden/factory rewards and world overlays exist. The player does not yet see a compelling unified reason to engage every pillar.

### Checklist

- [ ] Early earnings have obvious, desirable purchases.
- [ ] Ship, weapons, tools, gardens, factories and vehicles each offer visible progression.
- [ ] Prices and payouts are balanced around active play time.
- [ ] No subsystem mints/spends outside the common ledger/router.
- [ ] Rewards are announced physically and reflected in the world/ship.
- [ ] Unlocks introduce new decisions rather than linear stat inflation only.
- [ ] Cosmetics are previewable, equippable and visible in hands/ship/quarters.
- [ ] Old saves migrate with neutral defaults.
- [ ] Interrupted save, travel and reward routing cannot duplicate or lose value.
- [ ] Campaign, arena and Tidefront rewards coexist without forcing unwanted modes.

### Next best slice

Make the first complete loop award a visible ship or tool upgrade, then tune costs/payout against the actual time and difficulty measured on Quest.

---

## 18. Multiplayer, PvP and Tidefront

### Current state

Bots, five arenas, modes, weapons, augments, progression, Photon presence, conquest simulation, war table, missions and hotseat exist. Combat/score sync, room-code UX, body/voice and two-headset proof remain incomplete. Recovery freeze keeps this lane paused.

### Checklist

#### Arena quality

- [ ] One arena reaches the same art, readability and combat feel bar as campaign combat.
- [ ] Bots use cover/objectives/weapons intelligently without cheating.
- [ ] Modes explain themselves physically and resolve/restart cleanly.
- [ ] Spawn safety, score, timers, pickups and progression are readable.
- [ ] Ten varied matches remain fun before content expansion.

#### Online

- [ ] Room-code create/join UI replaces hardcoded room.
- [ ] Remote head/hands/body align and interpolate cleanly.
- [ ] Host-authoritative fire, hit, disable, score and match state.
- [ ] Reconnect/leave/host-loss paths are bounded.
- [ ] Voice and mute/privacy behavior are deliberate.
- [ ] Two Quest headsets complete repeated matches without drift.
- [ ] No paid server dependency beyond approved free-tier path for testing.

#### Tidefront

- [ ] War table interaction is understandable without a spreadsheet tutorial.
- [ ] Ground/space missions visibly affect strategic outcomes.
- [ ] Fog, economy, AI and saves remain deterministic and fair.
- [ ] Hotseat handoff is private/readable.
- [ ] Photon live sync comes only after arena transport is proven.

### Next best slice

After recovery and the campaign combat slice, complete one two-headset room-code arena match. Do not add more modes until combat and synchronization are trustworthy.

---

## 19. Performance, QA, build and Meta release

### Checklist

#### Device performance

- [ ] Locked target refresh/frame budget chosen and held in representative scenes.
- [ ] CPU/GPU bottlenecks measured on Quest, not inferred from Linux reference numbers.
- [ ] World/ship/combat/creature/garden/factory/flight stress cases have budgets.
- [ ] Thermal and long-session performance tested.
- [ ] Repeated travel keeps memory/resources flat within named tolerances.
- [ ] Draw calls, materials, lights, particles, skinning and audio voices remain bounded.

#### Automated QA

- [ ] Exact-SHA EditMode, PlayMode, patch/audit and Golden Android remain mandatory.
- [ ] Workflow path coverage proves every Golden-route source path triggers its lane.
- [ ] Canary tests prove shader/build/audit gates actually fail on seeded defects.
- [ ] Scene/profile/package/build reports are durable and cannot silently skip.
- [ ] Save migration and old-profile fixtures exist for every schema change.
- [ ] Nightly/repeat route catches nondeterministic lifecycle defects.

#### Device QA matrix

- [ ] Cold/warm boot, New Game, Continue, Settings.
- [ ] Standing/seated and left/right-hand configurations.
- [ ] Every campaign world and return route.
- [ ] Every primary weapon/tool/creature interaction.
- [ ] Ship, flight, vehicle, garden, factory and traversal campaigns.
- [ ] Long save/travel/memory soak.
- [ ] Two-headset online matrix when enabled.
- [ ] Failure evidence includes exact APK SHA/checksum and log timestamps.

#### Release

- [ ] Meta app ID and entitlement.
- [ ] Privacy policy/Data Use/IARC.
- [ ] Release keystore backed up.
- [ ] Minimal permissions and diagnostics/dev surfaces disabled.
- [ ] Versioning/update/save compatibility plan.
- [ ] Icon, screenshots, trailer, description and comfort disclosures.
- [ ] Localization decision and text seam before large content scale.
- [ ] Store dry run, certification fixes and accepted submission.

---

# PART III — ORDERED PATH FORWARD

## Phase A — Finish recovery and authorize one immutable Quest checkpoint

- [ ] Close the travel-input race without weakening the 43-test route.
- [ ] Obtain same-SHA PlayMode, ordinary CI, contract scan and Golden Android green.
- [ ] Inspect all raw artifacts.
- [ ] Freeze one APK SHA/checksum.
- [ ] Run the short recovery Quest checklist only.
- [ ] Record pass/fail and logs; blocker findings remain priority zero.

**Exit:** architecture foundation is device-proven for boot, input, travel, save, W000/ToxicCity presentation and repeat route.

## Phase B — Close the recovery program cleanly

- [ ] Board exact proven owners/contracts and remaining device-only claims.
- [ ] Remove/supersede stale recovery PRs and temporary helpers.
- [ ] Preserve regression workflows and path-coverage gate.
- [ ] Reconcile `CURRENT_EXECUTION_CHECKLIST`, `MASTER_CHECKLIST`, `EXCELLENCE_MAP`, `SPRINT` and runbook against this roadmap.
- [ ] Resume only the lanes required for the first complete product slice.

## Phase C — Build the premium first-hour/first-destination slice

Order:

1. [ ] Hero ship exterior/interior and world-selection experience.
2. [ ] W000 onboarding surfaces and comfort/readability.
3. [ ] W001/ToxicCity-quality destination architecture, route and skyscape.
4. [ ] Signature creature body/behavior/presentation.
5. [ ] One full weapon/encounter at the shooter-quality bar.
6. [ ] One physical repair/automation interaction.
7. [ ] One clear garden interaction and output.
8. [ ] RILL/story staging and first meaningful reward.
9. [ ] Return to ship, visible upgrade, save/relaunch proof.
10. [ ] Quest performance/art/audio acceptance.

**Exit:** a stranger can play the first slice without explanation and wants to continue.

## Phase D — Ship flight/space-combat vertical slice

- [ ] Finish/bake SpaceLane.
- [ ] Tune cockpit and comfort on Quest.
- [ ] Build rich space depth/environment.
- [ ] Add two or three distinct enemy roles and mission structure.
- [ ] Connect salvage to visible ship upgrade.
- [ ] Prove repeat entry/exit and performance.

## Phase E — Ground combat breadth

- [ ] Use the hero weapon/encounter template to finish the starter trio.
- [ ] Complete melee and specialist weapons.
- [ ] Build enemy role matrix across creatures/robots/drones.
- [ ] Author encounters in real world geometry.
- [ ] Add animation/audio/VFX/haptics and difficulty behavior.

## Phase F — Garden/factory/base loop

- [ ] Three-species physical garden with genetics/giant crop.
- [ ] One clear extractor → belt → processor → useful output chain.
- [ ] Build/edit/tutorial visual language.
- [ ] Tie outputs to ship/weapons/missions.
- [ ] Expand species/recipes only after the loop is understood and fun.

## Phase G — Chapter 1 shipped-quality band

- [ ] Propagate proven architecture, sky, creature, combat, mission and audio templates.
- [ ] Finish water, grounding, VFX, signage, macro variation and art-conformance ratchet.
- [ ] Complete VO/audio for the band.
- [ ] Device-walk every route and fix repetition/scale/performance.
- [ ] Make the ship the consistent hub.

## Phase H — Vehicles, multiplayer and meta-game completion

- [ ] Complete vehicle families and garage after campaign use is proven.
- [ ] Finish room-code combat/score sync and two-headset proof.
- [ ] Polish one arena, then expand.
- [ ] Connect Tidefront live missions only after transport is stable.

## Phase I — Content scale and campaign completion

- [ ] Author W013+ through WorldSpecs against proven templates.
- [ ] Introduce each new tool/creature/hazard with its chapter.
- [ ] Place transmissions, choices and ending flags.
- [ ] Audit and device-sample in chapter batches.
- [ ] Never trade world count for a lower quality floor.

## Phase J — Ship and release

- [ ] Complete campaign/endings.
- [ ] Full performance/accessibility/localization/release QA.
- [ ] Store assets, certification and submission.

---

# PART IV — PRIORITY DECISIONS

## The next three floor-raising priorities after recovery

1. **One premium ship/home + first destination slice.** This unifies travel, story, worlds, combat, garden/factory reward and progression.
2. **One shooter-quality weapon + signature enemy encounter.** This establishes the real combat template before arsenal/creature multiplication.
3. **One obvious physical production loop.** This turns gardens/factories/belts from impressive backend code into understandable gameplay.

## Work explicitly not first

- Do not generate W013–W080 before the first destination template is shipped-quality.
- Do not add more weapons before one weapon meets the full handling/feedback/encounter bar.
- Do not add more creatures before one signature species passes real scene/device presentation.
- Do not expand factories before one chain is visually understandable.
- Do not expand multiplayer modes before one synchronized match is polished.
- Do not treat turnarounds, source tests or green builds as substitutes for Quest acceptance.

---

# PART V — UPDATE CONTRACT

After meaningful work:

1. Update the affected pillar’s maturity and checklist here.
2. Update `CURRENT_EXECUTION_CHECKLIST.md` with the exact next action.
3. Record exact commit/run/device evidence in `HANDOFF.md`.
4. Preserve old plans as history, but mark superseded instructions clearly.
5. Never mark a headset-dependent quality PROVEN from desktop/CI evidence alone.
