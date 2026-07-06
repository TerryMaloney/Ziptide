# 🐛 CREATURES — living-world AI & ecology — 50 ideas (bank; read README first)

Current: 10 behavior archetypes over CreatureRuntime/CreatureBehaviorBase (Bruiser, DroneCombat,
Flyer, HuskMolter, LightGrazer, Swarmer, TetherSwarm, WallCrawler, Warden, WitnessMite), data-driven
CreatureDefinition, non-lethal disables, IShockable plumbing, creature zones as layout data, Horde
consumes them. Bar: reads as an ECOSYSTEM (Rain World energy, scaled). Kid-readable telegraph LAW.
New behaviors = pure state cores + tests (the WardenState/ChargeState pattern).

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Locator-pulse fear: Swarmers scatter and WitnessMites freeze 3s when the pulse fires — a stimulus hook on CreatureBehaviorBase. | S | SwarmerBehavior, WitnessMite, locator pulse | |
| 2 | Loot-by-disable-method: CreatureRuntime records a DisableCause (shock/crumple/knockdown); CreatureDefinition loot keys off it — shocked drones drop intact parts, crumpled ones scrap. | S | CreatureRuntime, CreatureDefinition loot | |
| 3 | Pack role: a `guardOfNestId` field on CreatureZoneDef links one Bruiser to a Swarmer nest — attacking the nest aggros the Bruiser via its ChargeState telegraph. | S | CreatureZoneDef, Bruiser, ChargeState | |
| 4 | Nocturnal/diurnal flag on CreatureDefinition; the zone spawner only activates matching creatures for the current world-time, so districts differ at night. | S | CreatureDefinition, spawner, world-time | |
| 5 | Produce luring: garden produce dropped on the ground becomes a grazer attractor — LightGrazers path to it and eat it, riding the item-ID factory lookup. | S | LightGrazer, ItemFactory, garden produce | |
| 6 | HuskMolter decoys emit a Swarmer-attractor ping, so a shed husk pulls a cluster off you — one stimulus line in SwarmerBehavior. | S | HuskMolter, SwarmerBehavior | |
| 7 | Flyer perch points as CreatureZoneDef layout data; Flyers rest between patrols instead of hovering forever (perf + readability). | S | CreatureZoneDef, Flyer | |
| 8 | Deterministic per-zone spawn seed = zone ID + world seed, so the same creatures appear in the same spots run-to-run. | S | CreatureZoneDef, spawner | |
| 9 | Warden permit: carrying a holstered `permit` item makes Wardens skip the watch/arrest escalation in WardenState. | S | WardenState, ItemFactory, belt | |
| 10 | CreatureVariantAuthor generates deterministic "elder" variants (1.5× size, slower, richer loot) per archetype — one rare elder per district. | S | CreatureVariantAuthor, CreatureDefinition | |
| 11 | LightGrazers actually dim the lamp they graze on; it re-brightens when they're scared off — grazing becomes visible ecology. | S | LightGrazer, world lamp props | |
| 12 | WallCrawler pre-lunge telegraph: ceiling dust puff + a rising click 1s before the drop, kid-readable per the LAW. | S | WallCrawler, telegraph VFX/SFX | 🎨 |
| 13 | TetherSwarm audio hint: the vulnerable cord node hums louder and pulses, teaching the puzzle without text. | S | TetherSwarmBehavior | |
| 14 | One pooled audio emitter per Swarmer cluster instead of per-creature — Quest perf win, no gameplay change. | S | SwarmerBehavior, AudioDirector | |
| 15 | Shed husks are lootable chitin (crafting material) — a non-lethal harvest loop: scare the molter, take the husk, leave it alive. | S | HuskMolter, CreatureDefinition loot | |
| 16 | Thrown produce interrupts a Bruiser charge: it stops, eats, resets ChargeState — a kid-readable non-lethal counter. | S | Bruiser, ChargeState, produce | |
| 17 | LightGrazer belly-glow encodes fed state — a fed grazer glows bright, a starving one dim — zone health legible at a glance. | S | LightGrazer, emissive material | 🎨 |
| 18 | `ZIPTIDE: ECOLOGY zone= pop= fed= disabled=` census log per zone per minute so headset logcat can diagnose population bugs. | S | CreatureRuntime, diagnostics | |
| 19 | Creatures self-repair only inside their home nest radius — nests matter, and luring them away first is rewarded. | S | CreatureRuntime, nest data | |
| 20 | Hard per-zone population cap + pooled respawn in CreatureZoneDef (pool prewarmed at load) — the perf contract every idea below rides on. | S | CreatureZoneDef, spawner pooling | |
| 21 | Nests as objects: a `NestState` pure core (health, respawn timer, brood count) + tests (WardenState pattern); nests respawn their archetype until crumpled. | M | NestState core, CreatureZoneDef, Tests | 🎨 |
| 22 | Staged predation vignette: at dawn at flagged POIs, a Flyer snatches one Swarmer and carries it to a perch — deterministic, scheduled from zone data. | M | Flyer, SwarmerBehavior, world-time scheduler | |
| 23 | WitnessMite reporting: on seeing you disable a creature in a Warden district, the mite beelines to the nearest Warden, which investigates. | M | WitnessMite, WardenState | |
| 24 | Creature-vs-creature law: Wardens shock and arrest rampaging Bruisers via the IShockable path — extend WardenState targets beyond the player, with tests. | M | WardenState, IShockable, Bruiser | |
| 25 | Swarm drummer role: one Swarmer per cluster gets a visible drum telegraph that speed-buffs the rest; disabling it (non-lethal) calms the cluster. | M | SwarmerBehavior, CreatureDefinition role | |
| 26 | Shock chaining: taser hits arc between IShockable Swarmers within 2m, capped at 4 jumps with a clear arc visual — pure `ChainResolver` core + tests. | M | IShockable, stun plumbing, Tests | |
| 27 | Flyers scavenge: a Flyer picks up crumpled small creatures and hauls them to its perch, where loot accumulates — raid the perch, not each corpse. | M | Flyer, CreatureRuntime crumple | |
| 28 | Flyer rooftop nests with stealable eggs; the parent's dive has a long shadow + screech telegraph, and eggs are a premium trade good. | M | Flyer, NestState, loot/economy | 🎨 |
| 29 | Territory scent posts: Bruisers mark zone corners and Swarmers avoid marked ground — a pure `TerritoryGrid` core sampled by both, with tests. | M | TerritoryGrid core, Bruiser, SwarmerBehavior | |
| 30 | Buildable feeding station attracting N grazers on a schedule; fed grazers boost nearby garden yield — a produce-creature-produce loop. | M | LightGrazer, garden, placed props | |
| 31 | Horde draws waves from real nearby CreatureZoneDef populations and depletes them — after a horde night the district is visibly emptier until nests respawn. | M | Horde mode, CreatureZoneDef, NestState | |
| 32 | HuskMolter growth persists across visits via save keyed by zone+creature seed — the thing that grows when unseen genuinely remembers you. | M | HuskMolter, persistence, zone seed | |
| 33 | Symbiosis: LightGrazers perch on Bruiser backs and scatter when it aggros — the scatter itself is the kid-readable early-warning telegraph. | M | LightGrazer, Bruiser, ChargeState | 🎨 |
| 34 | Stimulus bus: a pure `StimulusField` core (noise/light/scent events with decay) all archetypes sample instead of bespoke distance checks — one tested system replacing ten. | M | StimulusField core, CreatureBehaviorBase, Tests | |
| 35 | Weather/time modifiers on CreatureDefinition (rain surfaces WallCrawlers onto streets, night doubles Swarmer cluster radius) — data-driven, no new code paths. | M | CreatureDefinition, weather/world-time | |
| 36 | Drone salvage minigame: a stunned DroneCombat unit can be held and "unscrewed" (grab + twist) for intact-part loot before the stun expires — riskier, richer. | M | DroneCombat, IShockable, loot | |
| 37 | WitnessMite camera flash: after 3s observing you it fires a bright flash telegraph and summons a Warden patrol — swat or scare it during the wind-up. | M | WitnessMite, WardenState, telegraph | |
| 38 | Warden pound POI: arrested creatures get escorted to a caged pound; players can free them (a witnessed crime) or trade for release. | M | WardenState, CityLayoutDefinition POI, economy | |
| 39 | TetherSwarm nest variant: cord nodes anchor to a NestState nest — crumpling the nest collapses the whole tether web at once, an alternate solution. | M | TetherSwarmBehavior, NestState | |
| 40 | Zone alarm state: disables raise a decaying fear level making survivors skittish (flee earlier, graze less) for N minutes — a pure `AlarmState` core with tests. | M | AlarmState core, CreatureZoneDef, behaviors | |
| 41 | Tameable companion: taser-stun then offer garden produce during the stun window to imprint; `CompanionState` core + tests, one max, travels only holstered-in-a-carrier. | L | CompanionState core, IShockable, TravelCoordinator, belt | |
| 42 | Migration: flocks relocate between POIs on world-time along deterministic routes authored in CityLayoutDefinition — dusk sees Flyer skeins crossing the city. | L | CityLayoutDefinition, world-time, Flyer/Swarmer | |
| 43 | Ecology-lite sim: per-zone food + population counters tick on world-time (grazing depletes, nests replenish); over-hunting empties a district for days — pure `EcologyLedger` core, deterministic. | L | EcologyLedger core, CreatureZoneDef, NestState | |
| 44 | Apex roamer: one rare district-crossing elder Bruiser with long ground-shake telegraphs; lesser creatures flee its path via StimulusField, broadcasting its approach. | L | CreatureVariantAuthor, StimulusField, ChargeState | 🎨 |
| 45 | Hive finale: a building-scale TetherSwarm as the Horde capstone — a multi-node cord puzzle where each severed node disables one spawn vent, non-lethal all the way. | L | TetherSwarmBehavior, Horde mode, NestState | |
| 46 | Ranger job line: catch-and-relocate contracts (JobDefinitions) — stun a target, cage it, carry the cage through a travel door to its destination zone, paid on delivery. | L | JobDefinition, IShockable, TravelCoordinator | |
| 47 | Individual creature memory: a small per-creature ledger (fed/shocked counts by deterministic ID) shifts approach-vs-flee thresholds — the grazer you feed daily greets you. | L | CreatureRuntime, save, behaviors | |
| 48 | Predation director: a scheduler staging creature-vs-creature encounters at POIs from zone data + a seed, guaranteeing ~2 food-web moments per session. | L | scheduler core, all behaviors, CityLayoutDefinition | |
| 49 | Shoreline biome set: aquatic variants reusing existing cores on a swim plane (surface Swarmer shoals, a wading Bruiser) + a pier-fishing lure using produce bait. | L | CreatureBehaviorBase, CreatureDefinition, new zone type | 🎨 |
| 50 | Ecology showcase world: a small preserve scene (via TravelCoordinator, content-only) demonstrating the full chain — nests, grazers, predation, migration — doubling as the systems' integration test bed. | L | world scene, CreatureZoneDef, all creature systems | |
