# AUTOMATION & CONVEYORS — the unique, fun factory layer (design doc)

> **Hardwiring §9 / Phase 4.** A major, signature feature. It must WORK, and it must be genuinely fun
> and *extremely cool* — a VR-native automation loop no other game quite has. **Get the fun right in
> this doc before writing code.** Status: skeleton for Fable 5 — expand this one the most.

## Current state (code survey)
`ProductionGraph` is an **abstract** factory sim (`NodeKind.Conveyor/Splitter/Combiner` are
"representative visuals only, never truth"). **Zero physical belts.** The backend math exists; the
felt, physical, satisfying layer does not.

## The thesis (why this can be special)
Keep the graph as the backend TRUTH, but build a **physical, hand-built, satisfying** automation layer
on top — the Factorio/Satisfactory dopamine, delivered with your hands in VR. The VR-unique joy is that
you **build the factory with your hands** and can **reach into the flow**. That tactility is the moat.

## What to build
- [ ] **Snap-grid build system** — placeable belts, splitters, mergers, inserters, machines. Grab a
  segment, snap it, hear the click, feel the haptic. **Placement itself must feel great** (this is the
  core fun — budget real time on snap feel, ghost preview, rotation, undo).
- [ ] **Items physically ride belts** — pooled visual items (**adopt `GamePool`**) flowing along
  segments; reach in and grab one; hand-feed a machine. Graph resolves throughput; visuals make it
  readable and satisfying.
- [ ] **Machines process recipes** (existing `ProductionGraph`/`RecipeDefinition` backend) with
  Forge-gait-style animation, glowing energy, a throughput readout you can read at a glance.
- [ ] **Power / fuel** system + **upgrade tiers** (spaghetti → elegant progression).
- [ ] **Blueprints** — copy/stamp a layout (the "I built this once, now clone it" payoff).
- [ ] **The cool/unique hooks (pick + push these):**
  - [ ] **Flow visualization** — energy/goods glow, speed reads visually.
  - [ ] **Conductor mode** — ride/zipline your own line (ties to §3 ziplines) to inspect it.
  - [ ] **Cross-system payoff** — auto-farms (§8), auto-mining, ship/vehicle fuel (§4/§7), goods → the
    economy, even auto-produced gear. The factory feeds the whole game.
  - [ ] **Hand-scale AND world-scale** — tabletop planning vs walk-the-floor scale.

## Consistency-spine hooks
Belt items use `GamePool`; machines/belts are Forge kits via `ArtModuleRegistry` (primitive fallback
first); recipes + machines are Definitions-by-id; output routes through `ProfileEconomy`; built factories
persist in `PlayerProfile`; placement uses the shared comfort/input layer.

## Technique research TODO (cite at build time)
Factorio/Satisfactory belt+throughput models; item-on-belt rendering at scale (instanced, pooled);
VR building/snap-placement UX; what makes automation *satisfying* (feedback loops, readability, progression).

## Tests + audit
Graph throughput deterministic; belt-item pooling stress test (no GC spikes); recipe resolution tests;
PerfBudget cap on active belt-item count; a "factory persists + resolves offline" golden test.

## 🚀 Room to expand (make it excellent — this is where to go big)
Trains/drones between factories; logistics puzzles; a shareable-blueprint economy (MP); factory combat
defense (creatures raid your line); modular machine "programming"; a satisfying end-game megafactory;
co-op factories over the Photon seam. This feature can carry the game — treat it like a headline.
