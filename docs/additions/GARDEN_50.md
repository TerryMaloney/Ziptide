# 🌱 GARDEN — 50 improvement ideas (bank; read `docs/additions/README.md` first)

Current: GardenPlotRuntime 3-state loop (EMPTY→GROWING→READY), pure GardenService, 4 plants, offline
growth via ProfileEconomy. Bar: Stardew/Animal-Crossing depth, VR-native (hands matter).

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Discrete growth stages: add `stageCount` + per-stage scale/shape curve to `PlantDefinition` so `GardenPlotRuntime.ShowPlant` snaps through sprout→leaf→bud silhouettes instead of one continuous lerp. | S | PlantDefinition, GardenPlotRuntime | 🎨 |
| 2 | Harvest pop: on `GARDEN_HARVEST`, fire a chime via `AudioDirector` + a 10-particle pooled leaf burst — the "Stardew ka-ching" the loop lacks. | S | GardenPlotRuntime, AudioDirector | 🎨 |
| 3 | READY haptics: when a hand hovers a READY plot's `XRSimpleInteractable`, send a soft haptic pulse so ripeness is felt before seen. | S | GardenPlotRuntime, XRI hover | |
| 4 | Fresh-harvest bonus: `freshWindowSeconds` on `PlantDefinition`; `GardenService.Harvest` grants +25% within the window after ready — rewards checking in, EditMode-testable. | S | GardenService, PlantDefinition | |
| 5 | Overripe droop: after ready + `overripeAfterSeconds`, yield decays toward a 50% floor (never dies — all-ages) and the plant visually slumps. | S | GardenService, PlotState, GardenPlotRuntime | |
| 6 | Yield preview: EMPTY-state readout lists `harvestYield` ("dew_bulb → 4 mineral, 12cr") so planting is an informed choice. | S | GardenPlotRuntime | |
| 7 | Biome affinity: compare `PlantDefinition.biomeId` to the world biome; off-biome grows 1.5× slower with a "struggling" note. | S | GardenService, WorldProfile | |
| 8 | Ready-on-arrival ping: when ProfileEconomy resolves offline growth on entry, log `ZIPTIDE: GARDEN_READY count=N` + a short sting so idle harvests aren't missed. | S | ProfileEconomy, AudioDirector | |
| 9 | Seeded hue jitter: hash `PlotState.plotId` into small deterministic hue/height variance so a row reads as living plants, not clones. | S | GardenPlotRuntime | 🎨 |
| 10 | Growth chimes: soft AudioDirector tick at 25/50/75% GrowthProgress when the player is near — the idle timer gets a heartbeat. | S | GardenPlotRuntime, AudioDirector | 🎨 |
| 11 | Plant lore: add `loreText` to `PlantDefinition` on the EMPTY readout, pulling worldbuilding into the dirt. | S | PlantDefinition, GardenPlotRuntime | |
| 12 | Ghost preview: EMPTY plots show a faint translucent hologram of the mature plant so players see what they're committing to. | S | GardenPlotRuntime | 🎨 |
| 13 | Seed rotation: `seedChoices` list on `GardenSpawnDefinition`; each replant cycles/seed-picks the next plant. | S | GardenSpawnDefinition, GardenPlotRuntime | |
| 14 | Compost carryover: harvesting leaves `storedMulch` for a +10% start bonus next `PlotState` — replanting feels cumulative. | S | GardenService, PlotState | |
| 15 | Harvest streak: consecutive on-time harvests stack +5% (cap +25%) in PlotState, reset on a missed window. | S | GardenService, PlotState | |
| 16 | Gloves tier 2: `field_gloves_2` ToolDefinition (higher power) buyable with credits; GardenService already scales tend by power. | S | ToolDefinition, economy | |
| 17 | Wind-blown crops: on Wind worlds, plant visuals sway with amplitude tied to hazard intensity (seeded phase). | S | GardenPlotRuntime, hazards | 🎨 |
| 18 | Tend hint glow: while GROWING with tends remaining, the soil bed pulses faintly; goes still once spent — teaches tending without text. | S | GardenPlotRuntime | |
| 19 | Daily tally: readout footer shows "harvested today: N" from `LedgerSource.Garden` entries. | S | GardenPlotRuntime, ledger | |
| 20 | Tend telemetry: log `ZIPTIDE: GARDEN_TEND plot= tool= mult=` on each `GardenService.Tend` for device balance tuning. | S | GardenService, diagnostics | |
| 21 | Watering can v1: a `ToolFunction.Water` ToolDefinition + grabbable can; holding it to soil calls `GardenService.Tend` — the first hands-on tend tool. | M | ToolDefinition, ItemFactory, GardenPlotRuntime | 🎨 |
| 22 | Pour physics: the can only tends when tilted past ~60°, emitting a pooled water stream; credit accrues over 2s of correct pour. | M | watering tool, GardenPlotRuntime | 🎨 |
| 23 | Prune snips: GROWING plants sprout 2-3 highlighted overgrowth nubs; snipping each with a Prune tool applies the tend in thirds. | M | GardenService, GardenPlotRuntime, ToolDefinition | 🎨 |
| 24 | Physical seeds: seeds become holsterable ItemFactory items; planting = placing the seed onto the soil trigger, consuming it. | M | ItemFactory, GardenPlotRuntime, inventory | |
| 25 | Flood irrigation: on Flood worlds, active flood auto-applies a Water tend to open plots once per cycle — hazards as weather. | M | GardenService, hazards | |
| 26 | Radiation mutation: on Radiation worlds, harvest rolls a seeded chance to substitute a `glow_` variant worth 3× — deterministic. | M | GardenService, RewardRouter, resources | |
| 27 | Spore weeds: Spore worlds seed 1-2 weed clumps on GROWING plots that pause growth until hand-yanked out (grab pop). | M | GardenPlotRuntime, PlotState, hazards | 🎨 |
| 28 | Static bloom: a plant whose yield doubles if harvested while the Static hazard is active (crackle VFX signals the window). | M | PlantDefinition, GardenService, hazards | 🎨 |
| 29 | Sprinkler machine: a MachineDefinition that auto-Waters plots in radius each cycle, buyable — the idle-automation rung. | M | machines, GardenService | |
| 30 | Fertilizer chain: a machine recipe converts rust_fern yield into `fertilizer_charge` tool uses — crops feed better crops. | M | machines, ToolDefinition, economy | |
| 31 | Garden jobs: JobDirector quests ("harvest 5 dew_bulb here"), progress read from `LedgerSource.Garden` entries — zero new tracking. | M | JobDirector, ledger | |
| 32 | Companion planting: `companionIds` on PlantDefinition; pads within 2m of a growing companion get +15% yield. | M | PlantDefinition, GardenService, GardenSpawnDefinition | |
| 33 | Cross-pollination: two adjacent READY plots of different species roll a seeded chance for a hybrid seed (`hybridOf` field) with merged stats. | M | PlantDefinition, GardenService, ItemFactory | |
| 34 | Nibbler drone: a non-lethal pest drone visits the grove and siphons growth until stunned; `DRONE_DOWN` refunds the progress. | M | drone system, GardenService, POI | 🎨 |
| 35 | Clean-pull harvest: grabbing a READY plant and pulling smoothly upward (velocity-gated) grants +20% — hands literally matter. | M | GardenPlotRuntime, XRI grab | |
| 36 | Harvest basket: harvest spawns 2-4 pooled grabbable produce; tossing them into a basket within 20s banks a bonus. | M | GardenPlotRuntime, ItemFactory, RewardRouter | 🎨 |
| 37 | Plant scanner: a scanner tool over a plot projects a hologram card (growth %, tends, forecast) — diegetic UI. | M | ToolDefinition, GardenPlotRuntime | 🎨 |
| 38 | Row authoring: `gridCount`/`gridSpacing` on GardenSpawnDefinition expands one entry into a row of pads with derived plotIds. | M | GardenSpawnDefinition, JobDirector | |
| 39 | Soil quality: per-world `soilQuality` on WorldProfile multiplying growth, improvable +0.1 per N harvests (WorldState). | M | WorldProfile, WorldState, GardenService | |
| 40 | Export crops: each biome's signature plant yields a `seed_<biome>` only plantable off-biome at a premium — a seed trade across worlds. | M | PlantDefinition, resources, economy | |
| 41 | Real plant meshes: per-stage mesh/material lists on PlantDefinition swapped by the stage logic — authored silhouettes, one draw call each. | L | PlantDefinition, GardenPlotRuntime, art pipeline | 🎨 |
| 42 | Pad expansion: purchasable extra planter pads at HarvestGrove, persisted as owned-pad ids in WorldState, spawned next entry. | L | WorldState, JobDirector, WorldPoiBuilder, economy | |
| 43 | Plant genetics: PlotState carries seeded speed/yield/size genes; hybrids/replants inherit with deterministic mutation — the perfect-gene endgame. | L | GardenService, PlotState, PlantDefinition | |
| 44 | Seasons: a profile season index rotating each biome's viable plant set + garden tint via VisualThemeProfile; off-season offers alternates. | L | ProfileEconomy, PlantDefinition, VisualThemeProfile | 🎨 |
| 45 | Greenhouse dome: a HarvestGrove upgrade (WorldPoiBuilder + WorldState flag) shielding enclosed pads from hazards, permitting off-biome/season planting. | L | WorldPoiBuilder, WorldState, hazards | 🎨 |
| 46 | Garden hub world: a dedicated greenhouse world pack via TravelCoordinator, dense with pads + a dashboard of every world's plot states. | L | WorldPackDefinition, TravelCoordinator, GardenService | |
| 47 | Symbiotics gardener: an NPC helper hired per world (WorldState flag) giving one free offline tend per plot per cycle via ProfileEconomy. | L | ProfileEconomy, WorldState, POI | 🎨 |
| 48 | Prompt-to-plant: extend the project_art_plan pipeline to generate plant stage meshes/textures per biome from prompts, imported by a patcher. | L | art pipeline, editor patchers, PlantDefinition | 🎨 |
| 49 | Giant crops: a seeded rare roll flags a plot to grow 3× size, needing a two-handed pull for 5× yield with a big pop — the screenshot moment. | L | GardenService, GardenPlotRuntime, AudioDirector | 🎨 |
| 50 | Belt almanac: a belt-mounted almanac summarizing every world's plots (ready counts, time-to-ready) from the profile's WorldState list. | M | belt, GardenService, PlotState | 🎨 |
