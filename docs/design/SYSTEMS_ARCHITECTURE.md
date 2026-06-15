# ZIPTIDE — Systems Architecture (economy, worlds, creatures, profiles)

The structural blueprint for the big game systems. Built on the existing data-driven framework
(Definition + Runtime.Init pattern, Job system, TravelCoordinator, WorldAuditRunner). Decisions
captured from Terry 2026-06-15.

## Vision (locked decisions)
- **Persistence:** LOCAL save profile + **idle/offline progression** now. Profile is clean
  serializable "metadata" designed to go **online later**.
- **Future multiplayer = "Risk on top of the game":** async **dueling for planets** — the more you
  play, the more planets you can win; climb **leaderboards**. **No pay-to-win** (progress by play only).
- **Worlds:** hybrid **hand-built + procedural**, and **editable** (open a generated world and tweak
  it). World **categories/biomes** with distinct architecture; procedural composes them; we hand-add
  story/set-pieces per world. Plus a **Mod/Creator mode** — same game but free access to any planet
  + edit tools, no prerequisites.
- **Build order:** persistence + data model FIRST.
- **Creatures:** swarmers, wall-crawlers, flyers/scavengers, bruisers — and an extensible base for more.

## Guiding principles
- Everything is **data** (ScriptableObjects) resolved by **ID via registries**; reuse the existing
  `Definition + Runtime.Init(def)` pattern (IL2CPP-safe, no reflection).
- **Idle is math, not objects** — away worlds advance by computation, only the active world spawns
  live machines/plants/creatures (Quest perf).
- **Profile = the unit of future sync.** Build it online-ready now; server-authoritative later
  (anti clock-cheat + duel resolution).
- Quest performance is a **design constraint** (hard caps, GPU instancing, LODs, pooling).

---

## Layer 0 — Persistence & Profile (BUILD FIRST)
- **`SaveSystem`** (the planned `NarrativeSaveSystem`, expanded): DontDestroyOnLoad in `_Boot`,
  JSON in `Application.persistentDataPath`, versioned/migration-safe, autosave on travel/quit.
  API: `SetFlag/HasFlag`, `GetProfile`, `Save/Load`.
- **`PlayerProfile`** (serializable): `playerId`, `displayName`, timestamps, `flags`,
  `resources{id→amount}`, `unlocked{tools/machines/recipes}`,
  `worlds{worldId→WorldState}`, leaderboard stats. `WorldState` = discovered/owned, `ownerId`
  (for future conquest), `plots[]`, `mines[]`, `lastResolvedAt`, story state.
- **Offline/idle engine:** on load / world entry, `dt = now − lastResolvedAt`; advance mines &
  gardens by `dt` (capped, e.g. storage-limited / max ~8–24h) → "welcome back" summary. Pure math.
  When online later, resolve with **server time**.
- EditMode tests for save round-trip + idle accrual math (CI-checkable, no headset needed).

## Layer 1 — Data model (add alongside existing definitions)
- **Expand `WorldPackDefinition`** with the missing fields (worldType/biome, chapter, hazardType,
  enemyType[], keyResource[], flagsRequired/Granted, isProcedural, perfTier, etc.).
- **New definition types** (same Definition+Init pattern): `ResourceDefinition`, `ToolDefinition`,
  `MachineDefinition`, `PlantDefinition`, `CreatureDefinition`, `BiomeDefinition`,
  `RecipeDefinition` (build/repair costs), `BalanceConfig` (central tuning).
- **Registries** mirroring `ItemFactory`'s cache: Resource/Tool/Machine/Plant/Creature/Biome.

## Layer 2 — Economy
- **Mining / conveyor (Roblox-tycoon loop):** resource **Node** (per biome) → **Extractor** (build/fix)
  → **Conveyor** → **Collector/Storage** → profile resources. Tick-based when present; **idle accrual**
  computes stored-since-lastResolved (capped by storage → encourages return).
- **Growing / garden:** **Plot** → plant **Seed** (`PlantDefinition`) with a tool → **tend** with a
  *series* of tools (water/fertilize/prune; affect yield & speed) → grows over real time (idle) →
  **harvest** with a tool. Per-biome plants.
- **Tools/machines vary by planet:** `ToolDefinition` (function/tier/works-on), `MachineDefinition`
  (inputs/outputs/rate/health, build+repair via `RecipeDefinition`). Biome gates which tools/machines
  a planet needs — drives variety and progression.

## Layer 3 — Creatures
- **`CreatureDefinition`** (archetype, health, speed, damage, biome, loot) + **composable behavior
  components** extending the `DroneRuntime`/`IShockable` base: `SwarmBehavior` (boids/steering),
  `WallCrawlBehavior` (surface-projected movement), `FlyerScavengerBehavior` (hover/dive/steal/flee),
  `BruiserBehavior` (slow/tanky), + extensible `CreatureBehavior` base for "and more."
- Movement: NavMesh (ground), steering (flyers/swarms), raycast surface-stick (crawlers).
- **Quest caps:** max active creatures per world, simple FSMs, GPU instancing, pooling, LODs.

## Layer 4 — Worlds (hybrid, editable, mod mode)
- **World = `WorldPackDefinition` + `BiomeDefinition` + a generator/patcher.** Biomes (toxic city,
  desert, ice, bio-jungle, orbital station, derelict megastructure ship, void…) each = a modular
  **kit** (meshes/materials) + native plants/creatures/resources/hazards + generation rules.
- **Procedural generator** composes a world from a biome kit + a layout graph (spawn → objectives →
  resource nodes → exit), then **writes the layout to a data asset you can open and edit** in-editor
  — so generated worlds are tweakable and hand-polished with story/set-pieces.
- **Hand-built story worlds** (Toxic Venice, the derelict ship, key stations) authored via the World
  Recipe directly. `WorldAuditRunner` validates every world (spawn-on-solid, route, perf, contract).
- **Mod/Creator mode:** a single `ModeService` switch that (a) unlocks travel to ANY world with no
  prerequisites, (b) enables a free-fly + place/edit toolset, (c) bypasses gating — normal play
  unaffected.

## Layer 5 — Multiplayer "Risk" layer (LAST, design-for-now)
- Profile metadata is the sync unit. Online: profiles + world ownership live server-side.
- **Async planet-duels:** a challenge resolves **server-side** by comparing each profile's
  build/defense/army metadata — **no real-time netcode for v1**. Win → ownership transfers →
  leaderboards. No pay-to-win. Build after the solo loop is fun; profile designed now so it slots in.

## Balance
- **`BalanceConfig`** ScriptableObject: resource values, mine/grow rates, idle caps, tool tiers,
  machine throughput, creature difficulty curve, progression pacing. Tune in data. EditMode harness
  sanity-checks curves.

## Research references (patterns to follow)
- **Idle/offline:** timestamp-delta accrual + cap + welcome-back (tycoon / Grow-a-Garden / AdVenture
  Capitalist). Server time when online.
- **Conveyor/factory:** node-graph tick sim (Factorio/Satisfactory-lite; Roblox dropper→belt→collector).
- **Async conquest:** territory + async battle resolution + leaderboards (no realtime netcode v1).
- **Creature AI:** boids (swarm), NavMesh (ground), surface projection (crawlers); behavior-component FSMs.
- **Procedural-but-editable:** generate-to-data, then hand-edit (tile kits / grammar / WFC per biome).

## Build order (each = small, CI-gated commits; on-device verify by ZIPTIDE: tags)
0. **PREREQ:** CI green (Unity license) + scene dumps. (see `RECOVERY_STEPS.md`)
1. `SaveSystem` + `PlayerProfile` + offline-idle engine (+ EditMode tests).
2. Data model: expand `WorldPackDefinition` + add the new definition types + registries.
3. Harvest v1: node + tool → profile inventory (simplest loop).
4. Mining/conveyor v1 (+ idle accrual).
5. Garden v1 (plot→plant→tend→harvest + idle growth).
6. Creatures v1 (4 behaviors).
7. World generator v1 (one biome, generate-to-editable-data + audit).
8. Mod/creator mode switch.
9. Balance config + tuning.
10. (Separate epic) Online + Risk-style planet conquest.
