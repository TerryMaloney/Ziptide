# System — Creatures / Enemies

**Status:** designed; drone exists today (`DroneRuntime` + `IShockable`). See `SYSTEMS_ARCHITECTURE.md` §Layer 3.

## 1. Purpose
Varied enemies that threaten the player and the economy (they damage machines → repair loop). An
extensible base so "and more" creatures are data + a behavior component, not new systems.

## 2. How it works (intended)
Archetypes: **swarmers**, **wall-crawlers**, **flyers/scavengers**, **bruisers** — plus an extensible
base. They attack the player and **damage machines/parts** (feeds Tools & Repair). Killed via gun/taser
(reuse current weapon + `IShockable`).

## 3. Data + code
- `CreatureDefinition` (archetype, health, speed, damage, biome, loot) + **composable behavior
  components** extending the `DroneRuntime`/`IShockable` base: `SwarmBehavior` (boids), `WallCrawlBehavior`
  (surface-stick), `FlyerScavengerBehavior` (hover/dive/steal/flee), `BruiserBehavior` (slow/tanky).
- Movement: NavMesh (ground), steering (flyers/swarms), raycast surface-stick (crawlers).
- **Quest caps:** max active per world, simple FSMs, GPU instancing, pooling, LODs.

## 4. VR feel
Readable telegraphs before attacks; satisfying hit/kill feedback (already good on the drone — keep it).
Don't overwhelm comfort with too many fast close-range swarmers at once.

## 5. Open questions
Per-world creature budget number? Do creatures target the player, machines, or both, and how do they pick?

## Links
`MINING_CONVEYOR.md`, `TOOLS_AND_REPAIR.md`, `docs/design/SYSTEMS_ARCHITECTURE.md`.
