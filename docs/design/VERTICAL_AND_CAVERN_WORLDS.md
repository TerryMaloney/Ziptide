# VERTICAL & CAVERN WORLDS — elevation, caves, traversal (design doc)

> **Hardwiring §3 / Phase 1.** Environments that rise above the ground, go underground, and stack —
> plus the VR traversal to reach them. Status: skeleton for Fable 5.

## Current state (code survey)
`TerrainField` is a 2D height-field (fBM + domain warp). **A heightmap cannot make overhangs, caves,
or tunnels** — so every world is single-level ground today. No cavern/elevated/underground system.

## Locked decision
**Modular kits, Quest-safe. No voxel/mesh terrain** (reserve only for a rare hero set-piece if ever
justified). Hold 72–90 fps at scale.

## What to build
- [ ] **Cavern kit** — tunnel / chamber / junction / vertical-shaft / stalactite modules in
  `ArtModuleRegistry`; a `CaveBuilder` that strings them into networks; portal culling.
- [ ] **Underground layer** — caves as content-only world scenes (or an additive sub-layer) reached by
  travel; honor the boot/rig/spawn contract.
- [ ] **Elevated / mesa layer** — platforms, cliffs, floating islands above the heightfield; a
  `WorldSpec` elevation-layer field; `ScatterField` places vertical props.
- [ ] **VR-native traversal (make ziplines a signature — the game is *Ziptide*):**
  - [ ] **Ziplines** (grab + ride, velocity feel, branching lines).
  - [ ] **Elevators / lifts**, **jump-pads**, **grapple**, **climbable surfaces** (hand-over-hand,
    reuse the XRI grab layer — a headline VR feel).
- [ ] **Multi-level reachability audit** — extend `GridReachability` / `WorldReachabilityAudit` with
  step/zip/climb/elevator edges so every elevated or underground POI is provably reachable.

## Consistency-spine hooks
Cavern/vertical kits are Forge-baked + registry-driven + budgeted like buildings; caves/space/ground
are all world scenes via `TravelCoordinator`; traversal uses the shared comfort layer (climb/zip comfort).

## Technique research TODO (cite at build time)
Why heightmaps can't overhang; modular cave kit conventions; layered/stacked level design; portal
culling for caves on mobile VR; VR climbing/zipline comfort patterns. Fill `## Sourced technique` first.

## Tests + audit
CaveBuilder deterministic per seed; multi-level reachability green; portal culling verified; budget caps.

## 🚀 Room to expand
Underground biomes (crystal caverns, flooded tunnels — tide!), vertical cities with sky-bridges,
zipline *networks* as fast-travel, climbable landmark towers with vista payoffs (ties to
`SKYSCAPE_DESIGN.md`), hidden cave secrets/loot. Verticality is a differentiator — lean in.
