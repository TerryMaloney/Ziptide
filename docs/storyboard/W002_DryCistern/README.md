# W002 — The Dry Cistern · Story & Build README

> Inherits meta from `../STORY_BIBLE.md`; honors `MASTER_BUILD_PLAN` §12. Template: `../_WORLD_TEMPLATE.md`.

**Status:** designed  ·  **Chapter:** 1  ·  **Biome/Type:** Underground · (no hazard, dark/verticality)
**Faction:** none  ·  **One-line identity:** *your first descent — a pitch-black ancient waterworks under the city, older than anything above it.*

## 1. Role in the arc
First time underground; teaches **mining + dark traversal + the conveyor**. The first quiet hint that the
city is built on something far older (Architect stonework). **RILL:** Dormant. **Signal:** ~Threshold 1.

## 2. Premise (local stakes)
A contract to restart the cistern pumps that feed Toxic City — payment Cal needs to keep moving. The deeper
she digs, the older and stranger the construction gets. A skittering swarm has nested in the dark.

## 3. The mystery object
A **pump-control glyph-plate** that predates the city by millennia — and matches the glyphs on W001's relay.
First proof the "infrastructure" is layered over something ancient.

## 4. Planet physics & biome → `BiomeDefinition`
- `hazardType`: **none** (the threat is darkness, verticality, and the swarm). `id`: `cistern_deep`.
- Feel: pitch-black caverns, echoing, headlamp-dependent, drop-shafts (drift/anchor traversal). A single
  surface light-shaft as a navigational landmark.
- `nativeResourceIds`: `mineral`; `nativeCreatureIds`: `swarm`. `ambientTint`: near-black blue.

## 5. Sky & atmosphere → `VisualThemeProfile`
- No sky (underground). The "sky moment" = the distant surface light-shaft far above; phosphor veins in rock.
- `groundTint`: wet stone; heavy fog/darkness; minimal ambient.

## 6. Machines & build/repair → `MachineDefinition` + conveyor
- Signature: **cistern pumps** + an **ore-bucket conveyor** (the game's first real conveyor lesson —
  route mined mineral to a collection point). Build/repair recipes spend `scrap`/`mineral`.

## 7. Crops & resources → `ResourceNodeDefinition`
- `keyResource`: **Mineral** (first true mining nodes — `requiredFunction: Mine`, tiered tool gates depth).
  No crops (underground).

## 8. Weapons / gear found
- **Headlamp + scanner upgrade** (see in the dark / tag ore). Possibly a basic **mining tool** tier-up.

## 9. Abandoned ship / station
- A **flooded digger rig** abandoned mid-bore. Log: *"the pumps were never for water."* (Waker-thread #2.)

## 10. Alien enemies → `CreatureDefinition` + zones
- **Swarm** (`swarmer` archetype, shockable, small loot) nesting in the dark — first swarm combat;
  the Static Net / Sonic tools shine here. Light count, ambush-flavored.

## 11. Missions → `JobDefinition`
| # | Beat | Learns | Step | Marker/target |
|---|------|--------|------|---------------|
| 1 | Descend to the pump floor | dark traversal | GoToMarker | `pump_floor` |
| 2 | Clear the swarm nest | swarm combat | DisableDronesCount/ShootTargets | nest |
| 3 | Mine + route mineral on the conveyor | mining + conveyor | Collect (mineral) | ore nodes → cradle |
| 4 | Restart the pumps & surface | payoff | GoToMarker | `cistern_control` |

- **Reward:** credits + first `mineral` stock. **Completion flag:** `W002_COMPLETE`. RILL: 2–3 Dormant lines
  (*"This stonework is wrong for the period."*).

## 12. Map & placement → `CityLayoutDefinition` (underground variant) / world-kit
Vertical shaft hub → pump floor → nest cavern → control room. Spawn at shaft mouth; mission interior at
control. Heavy verticality; the surface light-shaft as the readability anchor.

## 13. Audio → `AudioProfile`
Dripping, deep reverb, low pump-thrum that returns as the machines come online; skittering for the swarm.

## 14. Build recipe & cross-links
Build via `../../systems/WORLD_BLUEPRINT.md` (underground kit). Canon §12 W002 · Meta `../STORY_BIBLE.md` ·
Gear `../../09_GEAR_AND_TOOLS.md`. Changeable-without-code: layout/jobs/RILL = data; new dark-traversal feel
may need a patcher tweak.
