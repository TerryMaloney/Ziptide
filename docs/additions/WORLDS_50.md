# 🌍 WORLDS — exploration / traversal / secrets / weather — 50 ideas (bank; read README first)

Current: data-driven worlds (WorldSpec → patchers): TerrainField (fBM+warp+biome matrix), arrival
vistas, 7-verb POI network, breadcrumb cairns, ScatterField props, generated buildings (enterable
doorways), RoomPartitioner interiors, hazard volumes, SkyVista skies, quality gates. Bar: No Man's
Sky / BotW exploration joy on 300m Quest worlds. **Every new feature should be a spec-drivable field.**

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Day/night as a pure theme lerp: WorldSpec `dayNightCurve` + duration; SkyVista/VisualThemeProfile blend two canon keyframes on a seeded clock — no lighting rebuild. | S | WorldSpec, SkyVista, VisualThemeProfile | 🎨 |
| 2 | Altitude fog gradient: WorldSpec `fogByAltitude` feeds terrain/sky fog so valleys read misty, summits clear — free depth on 300m worlds. | S | WorldSpec, TerrainField, SkyVista | 🎨 |
| 3 | Breadcrumb cairns get a dusk-activated emissive pulse keyed to the day/night clock — the night nav layer, zero new geometry. | S | cairn builder, VisualThemeProfile | 🎨 |
| 4 | POI discovery sting: first entry into any POI radius fires an AudioDirector one-shot + a diegetic name toast, logged `POI_DISCOVERED`. | S | POI network, AudioDirector | |
| 5 | Hero-landmark visibility gate: quality gates raycast from spawn to the vista landmark and fail the build if terrain/buildings occlude it. | S | quality gates, SpawnMarkerRuntime, landmark kits | |
| 6 | Scatter clearing masks: ScatterField subtracts a seeded falloff disk around each POI so props frame, not clutter — radius a WorldSpec field. | S | ScatterField, POI network | |
| 7 | Biome ambient beds: the biome matrix maps each cell to an AudioDirector loop (wind grass, cave drip) crossfaded as the player crosses cells. | S | biome matrix, AudioDirector | |
| 8 | Surface-aware footsteps: the biome ID under the player selects footstep/hand-plant audio sets, deterministic from the matrix. | S | TerrainField, biome matrix, AudioDirector | |
| 9 | TravelBerth beacon pillars: each berth spawns a vertical light column sized from WorldSpec, readable from anywhere on the map. | S | POI (TravelBerth), VisualThemeProfile | 🎨 |
| 10 | Seeded meteor streaks in night skies: SkyVista spawns 2-3 shader-billboard shooting stars per night phase — pure vertex anim, no particle budget. | S | SkyVista | 🎨 |
| 11 | RuinCache glint: unlooted caches emit a slow sparkle billboard that dies on loot — the NMS "something's there" wink. | S | POI (RuinCache) | |
| 12 | Cliff strata banding: TerrainField shader adds height/slope-keyed color bands from a WorldSpec gradient so raw fBM reads as geology. | S | TerrainField shader, WorldSpec | 🎨 |
| 13 | Walkability gate: quality gates flood-fill the heightfield from spawn and fail the world if any POI is unreachable under max slope. | S | quality gates, TerrainField, POI network | |
| 14 | Cave-mouth dressing: CaveSecret entrances get a prop kit (arch stones, roots, moss) scattered by ScatterField with an entrance mask. | S | POI (CaveSecret), ScatterField | 🎨 |
| 15 | Arrival nameplate: a diegetic sign/monolith at spawn renders the world's display name + biome tags from WorldSpec. | S | SpawnMarkerRuntime, WorldSpec, landmark kits | |
| 16 | Wind hazard cues: Wind volumes stream leaf/dust billboards + pan AudioDirector wind along the vector so intensity is felt before it pushes. | S | hazard (Wind), AudioDirector | |
| 17 | Post-flood wetness: when a Flood recedes, a timed wet-decal/darkened-albedo lingers on terrain in the footprint (seeded duration). | S | hazard (Flood), TerrainField shader | 🎨 |
| 18 | Belt compass pips: discovered POIs register bearing pips on the belt HUD — no map screen, VR-comfortable glanceable nav. | S | POI network, belt | |
| 19 | Night particle pockets: fireflies/embers/spores as tiny instanced emitters placed by ScatterField only in `nightLife`-flagged cells. | S | ScatterField, biome matrix | 🎨 |
| 20 | Biome cairn variants: cairns swap prop prefab per biome cell (ice slabs, scrap, bone stacks) via a WorldSpec variant table. | S | cairn builder, biome matrix, WorldSpec | 🎨 |
| 21 | Weather state machine: WorldSpec `weatherStates[]` on a seeded schedule, each lerping VisualThemeProfile and scaling hazard intensity (storm ramps Wind/Static). | M | WorldSpec, hazards, VisualThemeProfile, SkyVista | |
| 22 | True cave POIs: CaveSecret runs RoomPartitioner BSP under the heightfield — 3-5 rooms, a shaft entrance, interior scatter, a deep RuinCache. | M | RoomPartitioner, POI network, TerrainField, ScatterField | |
| 23 | 🟡 PARTIAL (hardwiring 1.4a): the MECHANIC shipped — pure `ZiplineCore` (sag + ease-in, tested) + `ZiplineRuntime` (grab handle, delta-translates the rig; ComfortVignette engages from rig motion). REMAINING: world placement — a patcher/WorldSpec pass stringing lines between POI pairs (one `Init(start,end)` per line). | M | WorldSpec, POI network, zipline builder | |
| 24 | Climbing surfaces: WorldSpec `climbability` tags BuildingGrammar walls + steep terrain bands with XRI climb interactables, comfort-height gated. | M | BuildingGrammar, TerrainField, XRI | |
| 25 | Secret vaults: a `vaultPuzzle` spec enum (lens align, pressure plates, sound sequence) gates a RoomPartitioner vault under a RuinCache — no combat. | M | RoomPartitioner, POI network, WorldSpec | |
| 26 | Timed world events: a seeded scheduler drops a telegraphed meteor (sky streak → impact glow) spawning temporary HarvestGrove nodes at a deterministic site. | M | SkyVista, POI (HarvestGrove), event scheduler | |
| 27 | Ambient creature flocks: GPU-instanced boid quads (birds, drones, motes), species/count from the biome matrix, orbiting landmarks — non-lethal, LOD-culled. | M | biome matrix, landmark kits, boid system | 🎨 |
| 28 | Rivers from flow accumulation: TerrainField computes downhill flow, carves a channel, lays a ribbon water mesh doubling as a mild Flood path. | M | TerrainField, hazard (Flood) | |
| 29 | Berth tunnels: two TravelBerths get a RoomPartitioner-extruded lit underground corridor between them — a fast-travel-by-foot shortcut with reverb. | M | RoomPartitioner, POI (TravelBerth) | |
| 30 | Vista towers: WorldSpec `vistaTower` makes BuildingGrammar emit one tall building with a stairwell to a rooftop overlook — the BotW tower moment. | M | BuildingGrammar, RoomPartitioner, WorldSpec | |
| 31 | Reverb zones: interiors/caves/canyons register AudioDirector snapshot volumes at build time (from RoomPartitioner bounds + terrain concavity). | M | AudioDirector, RoomPartitioner, TerrainField | |
| 32 | Updraft vents: repurpose hazard-volume plumbing as gentle non-lethal lift columns at spec-marked sites, lofting the player to ledges with vignette. | M | hazards, WorldSpec, POI network | |
| 33 | Gondola line: a slow seated cable-car between two landmarks (spec pair) — the comfort-safe scenic traversal + a moving vista platform. | M | landmark kits, WorldSpec, spline rider | |
| 34 | Photo spots: spec-tagged vista markers where framing the hero landmark in a held viewfinder item stamps a collectible — teaches players to look. | M | WorldSpec, landmark kits, ItemFactory | |
| 35 | Ruin-field lot type: LotPartitioner gains a `ruined` class where BuildingGrammar emits collapsed shells with climbable rubble ramps + exposed rooms hiding caches. | M | LotPartitioner, BuildingGrammar, POI (RuinCache) | 🎨 |
| 36 | Tidal cycle: a water plane height lerps on a seeded timer, submerging/revealing sandbar paths and a low-tide-only RuinCache — time-gated secrets from one float. | M | WorldSpec, TerrainField, POI network | |
| 37 | Hazard telegraphing: Spore/Radiation volumes ramp particle density + audio 30s before peak on the weather schedule — hazards as readable weather. | M | hazards, weather scheduler, AudioDirector | |
| 38 | Lore terminals: StoryAnchor POIs spawn readable terminals whose text comes from WorldSpec lore fields, chaining a 3-stop story trail across the graph. | M | POI (StoryAnchor), WorldSpec | |
| 39 | Harvestable flora: ScatterField places interactable plants resolving through ItemFactory ids per biome — foraging density a spec dial, loot registry-driven. | M | ScatterField, ItemFactory, biome matrix | |
| 40 | Enterable landmarks: each of the 5 hero kits gets one RoomPartitioner chamber behind a doorway, holding that world's StoryAnchor payoff. | M | landmark kits, RoomPartitioner, POI network | 🎨 |
| 41 | Sky-island layer: WorldSpec `skyLayer` spawns 2-3 elevated terrain patches reached by updrafts + linked by ziplines, each hosting a POI — vertical density. | L | TerrainField, POI network, zipline/updraft | |
| 42 | Multi-level cave dungeon: stacked RoomPartitioner floors under terrain with its own underground biome, connected shafts, a bottom vault — a full shrine-scale secret. | L | RoomPartitioner, biome matrix, ScatterField, AudioDirector | |
| 43 | Seasons as a spec axis: WorldSpec `season` permutes the biome matrix, scatter sets, SkyVista palette, and hazard mix — one seed, four visit-worthy variants. | L | WorldSpec, biome matrix, ScatterField, SkyVista, hazards | 🎨 |
| 44 | Living POI schedules: CombatCamps light fires at night, MachineSites run day cycles, HarvestGroves bloom at dawn — a seeded per-POI timeline off the clock. | L | POI network, day/night clock, AudioDirector | |
| 45 | Boatable water: lakes from the flow pass get a buoyant raft with paddle locomotion (vector-follows-hands, vignette on), opening water routes + shore secrets. | L | TerrainField lakes, raft locomotion, LocomotionProfile | |
| 46 | Moving weather fronts: storm cells as volumes sweeping the heightfield on seeded paths, modulating local hazard/fog/audio by position — weather you shelter from. | L | weather scheduler, hazards, SkyVista, TerrainField | |
| 47 | Caravan wanderers: non-lethal NPC caravans walking the cairn/POI network on seeded loops, pausing at berths — trade/lore stops that inhabit the graph. | L | POI network, cairn paths, NPC walker, ItemFactory | |
| 48 | Expedition contracts: a generator chains 3-4 POI verbs into a seeded objective route (survey → recover → light the beacon), a replayable through-line per world. | L | POI network, WorldSpec, belt HUD | |
| 49 | Subterranean world type: WorldSpec `subterranean` inverts the build — cavern-ceiling heightfield, bioluminescent scatter, cavern-dome sky, POIs in chambers. | L | TerrainField, SkyVista, RoomPartitioner, biome matrix | 🎨 |
| 50 | Hydraulic erosion pass: a bounded droplet-erosion step after fBM+warp carves gullies/deltas/switchbacks, with a quality-gate rejecting worlds below a ruggedness score. | L | TerrainField, quality gates | |
