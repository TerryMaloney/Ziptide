# System — Grow-a-Garden

**Status:** designed. See `docs/design/SYSTEMS_ARCHITECTURE.md` §Layer 2.

## 1. Purpose
A growing/farming loop that runs in real time (idle): plant, tend with a series of tools, harvest.
Ties to the Symbiotics faction (grown tech) in `docs/storyboard/SHIPS_AND_FACTIONS.md`.

## 2. How it works (intended)
**Plot** → plant a **Seed** (`PlantDefinition`) with a tool → **tend** with a *series* of tools
(water / fertilize / prune — each affects yield & speed) → grows over real time (idle) → **harvest**
with a tool. Plants vary by biome/planet.

## 3. Data + code
- `PlantDefinition` (grow time, yield, biome, tend steps), `ToolDefinition` for each tend action.
- `PlotState` in `EconomyState` (time-based growth, `IsReady`/`GrowthProgress`); idle growth via
  `IdleEngine`, applied by `ProfileEconomy`.

## 4. VR feel
Tending is hands-on (grab the right tool, use it on the plot) — shares the tool-grab verb with
Tools & Repair. Satisfying growth feedback (visible stages, harvest pop).

## 5. Open questions
How many tend steps before it's tedious? Per-plant vs per-plot tools?

## Links
`TOOLS_AND_REPAIR.md`, `docs/storyboard/SHIPS_AND_FACTIONS.md`, `docs/design/SYSTEMS_ARCHITECTURE.md`.
