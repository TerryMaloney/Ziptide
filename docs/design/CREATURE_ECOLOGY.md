# CREATURE ECOLOGY — living worlds (design doc)

> **Hardwiring §11 / Phase 4.** Turn ~10 isolated creature behaviors into an ecology. Status: skeleton
> for Fable 5. **Curate from `docs/additions/CREATURES_50.md`.** Pairs with the Forge gait pipeline.

## Current state (code survey)
~10 behavior components exist (Swarmer, WallCrawler, Flyer, Bruiser, HuskMolter, LightGrazer,
TetherSwarm, WitnessMite, Warden, DroneCombat) + a deep, tested Forge gait/skinning pipeline
(`ForgeGaitMotor`). Only **2 Forge creature bodies authored** (light_grazer, swarm_bug). **No ecology**
— no nests, packs, territory, or population.

## What to build
- [ ] **Spawn director + population sim** — per-world population budgets, spawn/despawn by zone, day/night.
- [ ] **Nests / territory** — creatures anchor to homes; territorial responses; you can find/disturb nests.
- [ ] **Packs / social behavior** — coordinated swarms, leaders, flocking (build on existing behaviors).
- [ ] **Predator / prey / food web** — creatures interact with each other and the garden/world.
- [ ] **More Forge bodies** — author additional creature genomes through the Forge (Picasso/art).
- [ ] **Tameable / companion** (optional, ties to §7 mounts) — a creature you befriend and ride.

## Consistency-spine hooks
Behaviors are data-driven (CreatureDefinition + profiles); bodies are Forge-baked via the registry;
non-lethal canon (disable/stun, never gore); population state persists per world in `PlayerProfile`.

## Technique research TODO (cite at build time)
Ecology/spawn-director patterns (Far Cry / RDR2 ambient life), flocking/boids, VR-safe creature threat
telegraphs (kid-readable).

## Tests + audit
Spawn director deterministic per seed + budget-capped; population sim golden test; behavior FSM tests;
PerfBudget cap on active creature count.

## 🚀 Room to expand
Migration/seasonal events; creature-caused world change (a bloom spreads); creature taming/breeding loop
(ties to garden genetics); boss composites from Forge parts; creatures that raid factories (§9) or gardens
(§8). Living worlds are a Terry pillar — make worlds feel inhabited.
