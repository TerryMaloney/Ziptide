# System — Mining / Conveyor (tycoon loop)

**Status:** designed; first economy target after Harvest v1. See `docs/design/SYSTEMS_ARCHITECTURE.md` §Layer 2.

## 1. Purpose
The Roblox-tycoon loop: extract a resource, move it along a belt, collect it into storage, and have it
accrue while you're away (idle).

## 2. How it works (intended)
Resource **Node** (per biome) → **Extractor** (build/repair) → **Conveyor** → **Collector/Storage** →
`PlayerProfile` resources. Runs tick-based while you're in the world; **idle accrual** computes
stored-since-`lastResolvedAt`, capped by storage (cap encourages you to come back and empty it).

## 3. Data + code
- `ResourceDefinition`, `MachineDefinition` (extractor/conveyor/collector: inputs/outputs/rate/health,
  build+repair via `RecipeDefinition`), `BiomeDefinition` gates which machines a planet needs.
- `MineState` / machine state in `EconomyState`; idle math via `IdleEngine`; applied by `ProfileEconomy`.
- A live-world tick MonoBehaviour drives the active world; away worlds = pure math.
- **Machines take damage** from creatures → fixed via **Tools & Repair** (`TOOLS_AND_REPAIR.md`).

## 4. VR feel
Belts visibly move items; building/placing a machine is a physical snap-place; repairing is hands-on
(see Tools & Repair). Keep part counts within the Quest budget (instancing, pooling).

## 5. Open questions
Placement: free-place vs grid/snap? How granular is "build" (whole machine vs modular parts)?

## Links
`TOOLS_AND_REPAIR.md`, `CREATURES.md`, `docs/design/SYSTEMS_ARCHITECTURE.md`.
