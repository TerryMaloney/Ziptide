# GARDEN AAA — better than Roblox "Grow a Garden" (design doc)

> **Hardwiring §8 / Phase 4.** Exceed Roblox Grow a Garden, VR-native. Status: skeleton for Fable 5.
> **Curate from `docs/additions/GARDEN_50.md`** (≈50 concrete banked ideas) — pull, don't duplicate.

## Current state (code survey)
`GardenService` grow loop + overripe decay (tested); **only 4 plants**; no hands-on tools; fresh-bonus
deferred (`FreshBonus = 0`). Offline growth via `ProfileEconomy`.

## The bar
Roblox Grow a Garden's hooks: plant variety, mutations/rarities, weather events, helper pets, offline
growth, sprinklers/automation, giant/rare crops, seasons, trading, a seed economy. Ziptide's edge is
**VR-native hands** + tie-ins to worlds, hazards, automation, and the economy.

## What to build (all extend `GardenService` / `PlantDefinition`)
- [ ] **Variety** — ≥20 plants across biomes (each a `PlantDefinition` + Forge/flora kit).
- [ ] **Genetics / mutations** — deterministic seeded genes (speed/yield/size), cross-pollination,
  **giant/rare crops** with a two-handed pull for a big payout (the screenshot moment).
- [ ] **Hands-on VR tools** — **watering can with pour physics** (tilt past ~60°), prune snips,
  holsterable seeds planted onto soil. Tending with your hands is the differentiator.
- [ ] **World/hazard interactions** — radiation → mutation rolls, static-bloom windows, flood
  auto-water, spore weeds you grab out. Gardens react to each world's weather/hazards.
- [ ] **Automation** — sprinklers, fertilizer machines (bridge into §9 conveyors).
- [ ] **Harvest juice** — pop + haptic + chime + pooled produce; daily-tally readout; a belt almanac.
- [ ] **Meta** — seasons, companion planting, a dedicated **garden hub world**, offline growth (already
  via `ProfileEconomy`), and **trading** (bridge into MP economy).

## Consistency-spine hooks
Plants are Definitions-by-id; flora is Forge/registry art (primitive fallback first); produce routes
through `ProfileEconomy`; gardens persist in `PlayerProfile`; tools use the shared grab/interaction layer.

## Technique research TODO (cite at build time)
What makes Grow a Garden retentive (mutation chase, offline dopamine, trading); VR farming interactions.

## Tests + audit
Genetics deterministic per seed; offline growth golden test; yield/timing balance tests; tool-tend tests.

## 🚀 Room to expand
A living ecosystem garden (pollinators, soil life); rare-seed hunts across worlds; a garden-to-cooking
or garden-to-crafting chain; garden competitions (MP); the garden hub as a social space. Make it a
game-within-the-game people log in daily for.
