# ZIPTIDE — Systems Docs (per-feature READMEs)

One short README per gameplay system, so each part has a clear "how it works / how it's *supposed*
to work" spec. This complements (does not replace) the big picture:
- `docs/design/SYSTEMS_ARCHITECTURE.md` — the overall architecture + build order.
- `docs/HANDOFF.md` — the single-operator continuity log (read first every session).
- `docs/storyboard/` — story/world/ship creative + how it maps to code.

**Each system doc follows this template** (keep it short, update as we build):
1. **Purpose** — what it is in one line.
2. **How it works (intended)** — the loop / mechanics, player-facing.
3. **Data + code** — which `Definition` types, registries, runtimes, interfaces.
4. **VR feel** — how it should feel in the headset (interaction, comfort).
5. **Status** — designed / building / done; what's gated behind what.
6. **Open questions** — decisions still needed from Terry.

## Index
| System | Doc | Status |
|---|---|---|
| Tools & Repair (hands-on fixing) | [`TOOLS_AND_REPAIR.md`](TOOLS_AND_REPAIR.md) | designed |
| Mining / Conveyor (tycoon loop) | [`MINING_CONVEYOR.md`](MINING_CONVEYOR.md) | designed |
| Grow-a-Garden | [`GROW_A_GARDEN.md`](GROW_A_GARDEN.md) | designed |
| Creatures / enemies | [`CREATURES.md`](CREATURES.md) | designed |
| Creature — Drone (enemy #1 template) | [`CREATURE_DRONE.md`](CREATURE_DRONE.md) | building |
| Drone Combat v1 (move / attack / hit-response) | [`DRONE_COMBAT_v1.md`](DRONE_COMBAT_v1.md) | designed (after gun swap) |
| Asset Swap Pipeline (model → Unity + VR grip) | [`ASSET_SWAP_PIPELINE.md`](ASSET_SWAP_PIPELINE.md) | designed |
| Build / Creator mode | [`BUILD_CREATOR_MODE.md`](BUILD_CREATOR_MODE.md) | designed |

> Add a new system = copy the template into `docs/systems/<NAME>.md`, add a row here, and claim it in
> `docs/HANDOFF.md` before writing code for it.
