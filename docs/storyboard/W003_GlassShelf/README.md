# W003 — Glass Shelf · Story & Build README

> Inherits meta from `../STORY_BIBLE.md`; honors `MASTER_BUILD_PLAN` §12. Template: `../_WORLD_TEMPLATE.md`.

**Status:** designed  ·  **Chapter:** 1  ·  **Biome/Type:** Exterior · Wind
**Faction:** none  ·  **One-line identity:** *windswept glass mesas under a huge open sky — the first time you look up and the universe looks back.*

## 1. Role in the arc
First **open-sky** world (awe beat) and first **wind hazard**; introduces **tendrils** and the **subliminal
Pattern seed** (a faint geometric shimmer at the zenith nobody comments on yet). **RILL:** Dormant. **Signal:** ~T1.

## 2. Premise (local stakes)
Cal is hired to raise wind-baffle relays so cargo skiffs can land — the wind keeps shoving everything (and
everyone) back toward the gate. Beautiful, lonely, and slightly *too* engineered.

## 3. The mystery object
The harvested **glass crystals ring at the exact frequency of W001's relay** — the first time the player can
connect two worlds themselves. Seeds "everything here is tuned to one signal."

## 4. Planet physics & biome → `BiomeDefinition`
- `hazardType`: **wind** (gusts apply a comfort-safe directional nudge; fall-risk ledges). `id`: `glass_shelf`.
- Feel: high exposed mesas, glass underfoot (careful footing), big drops (anchor/drift traversal).
- `nativeResourceIds`: `crystal`; `nativeCreatureIds`: `tendril`. `ambientTint`: pale blue-white.

## 5. Sky & atmosphere → `VisualThemeProfile` ★ awe world
- `skyGradient`: vast clear high-altitude blue → indigo; **two moons**; at the zenith a **faint geometric
  shimmer** (the first Pattern seed — subtle, uncommented). `groundTint`: glass-white.
- `planet`: two moons + the W001 banded planet slightly larger than last seen.

## 6. Machines & build/repair → `MachineDefinition`
- Signature: **wind-baffle relays** (raise/align them to calm landing zones). Build recipe spends `scrap`/
  `crystal`. Light conveyor optional (route crystal to the baffles).

## 7. Crops & resources → `ResourceNodeDefinition`
- `keyResource`: **Crystal** — harvest from glass blooms (`Harvest` function; a bridge between mining and the
  coming garden loop). Tiered tool gates the bigger blooms.

## 8. Weapons / gear found
- **Magnet-tether / anchor tool** (stay put in wind; yank crystals; swing across gaps) — the traversal
  upgrade this world is built to teach.

## 9. Abandoned ship / station
- A **crashed glider-skiff** wedged in the glass. Log: *"the wind always pushes you back toward the gate."*
  (Waker-thread #3 — the cage has subtle architecture.)

## 10. Alien enemies → `CreatureDefinition` + zones
- **Tendrils** (wind-whipped Bloom vines; `tendril`, shockable) snapping from crevices — light, environmental
  combat that rewards the anchor tool.

## 11. Missions → `JobDefinition`
| # | Beat | Learns | Step | Marker/target |
|---|------|--------|------|---------------|
| 1 | Cross the shelf to the first baffle | wind traversal | GoToMarker | `baffle_1` |
| 2 | Harvest crystal for the baffles | harvest loop | Collect (crystal) | glass blooms |
| 3 | Clear the tendrils on the ridge | tendril combat | DisableDronesCount | ridge |
| 4 | Raise + align the baffles | build payoff | Deliver/GoToMarker | `baffle_array` |

- **Reward:** credits + crystal. **Completion flag:** `W003_COMPLETE`. RILL: 2–3 Dormant lines, one almost
  noticing the zenith shimmer (*"…disregard. Sensor ghost."*).

## 12. Map & placement → world-kit (exterior)
Mesa-top spawn → crystal fields → tendril ridge → baffle array overlook. Big sightlines; verticality via
ledges + anchor points. The crashed skiff as a mid-map landmark/lore stop.

## 13. Audio → `AudioProfile`
Constant wind bed (rises/falls with gusts), glass chimes when crystals resonate, a single high tone at the
zenith you can *almost* hear.

## 14. Build recipe & cross-links
Build via `../../systems/WORLD_BLUEPRINT.md` (exterior kit). Canon §12 W003 · Meta `../STORY_BIBLE.md` ·
Gear `../../09_GEAR_AND_TOOLS.md`. Changeable-without-code: layout/jobs/sky/RILL = data; wind-nudge feel is a
patcher/runtime tune (comfort-test on device).
