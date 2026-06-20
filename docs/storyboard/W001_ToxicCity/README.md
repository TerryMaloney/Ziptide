# W001 — Toxic Venice · Story & Build README

> Deep, template-complete world doc (the depth bar for all worlds). Build beat-sheet for the team lives
> in `STORY.md` (T-Dog's) — this README is the comprehensive design that drives the data/build. Inherits
> meta from `../STORY_BIBLE.md`; honors `../../ZIPTIDE_MASTER_BUILD_PLAN.md` §12 + the W001 RILL beat.

**Status:** building (graybox exists — `ScenePatcherToxicCity`)  ·  **Chapter:** 1  ·  **Biome/Type:** City · Bloom
**Faction:** Wake Guild  ·  **One-line identity:** *a flooded canal-city of rust and oligarch glass, and your first repair that comes up **wrong**.*

## 1. Role in the arc
The onboarding world and the **first turn of the screw**: a routine contract that ends with a relay whose
signature "doesn't match local tech." It plants the whole game in one offhand line. **RILL state:**
Dormant. **Locked RILL beat:** boot — *"Systems nominal. I think."* **Signal:** 0 → Threshold 1.

## 2. Premise (local stakes)
Cal is broke and berthed at the Toxic City shipyard; she can't undock until she clears a contract and
earns passage credits (the leash). The Wake Guild posts a bounty via Dispatch: the district's maintenance
drones have gone feral around the canals and a signal-relay node dropped offline, freezing cargo clearance
— so Cal *literally can't leave* until it's fixed. Mercenary, ordinary, no destiny. Class divide is the
whole set: toxic sludge canals and slum walkways below, oligarch glass towers gleaming above, alien glyphs
on everything.

## 3. The mystery object
**The signal-relay node.** When Cal re-seats it, it powers up with a handshake **that isn't local tech** —
an older, wrong signature. RILL notices; Cal shrugs it off and ships out. (Pays off across Ch.2 → W062.)

## 4. Planet physics & biome → `BiomeDefinition`
- `hazardType`: **toxic** (sludge canals are no-stand; falling in = fall-respawn). `id`: `toxic_city`.
- Feel: layered verticality — wet/dangerous lower canals, mid walkways, unreachable upper towers. Standard
  gravity. Bridges connect platforms; lower routes are riskier shortcuts.
- `nativeResourceIds`: `signal_node`, `scrap`; `nativeCreatureIds`: `drone`; `nativePlantIds`: (none yet —
  Bloom is hazard here, not crop). `ambientTint`: sickly green-grey. `artKitId`: `toxic_industrial` *(stub)*.

## 5. Sky & atmosphere → `VisualThemeProfile`
- `skyGradient`: overcast industrial haze, sodium-orange low, bruised grey high. `groundTint`: damp concrete.
  Fog on (low density, green tint).
- `planet`: a dim **banded planet** low on the horizon (`angularSizeDegrees` ~12, slow rotation, doesn't
  follow player) — small now; it grows across Ch.2 as a visual "you're getting closer to the truth" cue.

## 6. Machines & build/repair → `MachineDefinition` + `RecipeDefinition`
- Signature: the **signal-relay node** — Cal re-seats/repairs it. *In-fiction it's a containment **seal**,
  not a relay* (the player only learns this later). Simple repair recipe (spend `scrap`).
- Teaches the repair loop; no conveyor yet (conveyor/build depth arrives W002+).

## 7. Crops & resources → `ResourceNodeDefinition` / `ResourceDefinition`
- `keyResource`: **Signal Node** (quest item / first resource). Scattered `scrap` nodes for the repair recipe.
- No farming here (W001 is the city/combat intro; harvest loop opens at W005 Oxidized Canopy).

## 8. Weapons / gear found
- Starts with the **taser** (non-lethal: drones are malfunctioning machines, not people) + the **left-wrist
  scanner** (find objectives/nodes). First sandbox for both. Ref `../../09_GEAR_AND_TOOLS.md`.

## 9. Abandoned ship / station
- Cal's own berthed **scavenger hull** doubles as the world's "wreck." Log (prior owner): *"third time the
  dock's eaten my deposit — this place doesn't want you to leave."* First entry in the hidden waker-log thread.

## 10. Alien enemies → `CreatureDefinition` + `DroneZoneDef`
- **Feral maintenance drones** (`drone`, shockable, light loot). Two combat patrols (market + canal) + a
  passive tutorial trio at Dispatch. Non-lethal taser disable; downing them clears the contract step.
  (Already built via `ScenePatcherToxicCity` drone zones + Drone Combat v1.)

## 11. Missions → `JobDefinition` (already authored: `ToxicCity_Contract`)
| # | Beat | Player learns | Job step | Marker |
|---|------|---------------|----------|--------|
| 1 | Report to the Dockmaster | orient / accept | GoToMarker | `dispatch_inside` |
| 2 | Clear the feral drones | aim / dodge / disable | DisableDronesCount(5) | market + canal patrols |
| 3 | Re-seat the signal relay (comes up *wrong*) | the mystery seed | GoToMarker | `relay_node` |
| 4 | Return to your berth & ship out | the leash loosens | GoToMarker | `shipyard_office` |

- **Reward:** 100 `credits` (passage). **Completion flag:** `toxiccity_complete`. **(Built — `ToxicCityContractBuilder`.)**
- **RILL lines (data):** boot *"Systems nominal. I think."* · first kill *"Maintenance units shouldn't bite
  back. Noted."* · relay *"That's not a maintenance handshake. That signature's older than this district."*
  · ship-out *"Berth's clear. Whatever that node was talking to — it wasn't us."*
- **Flags:** required: none (entry world). granted: `C1_W001_RILL_BOOT`, `toxiccity_complete`, `SIGNAL_THRESHOLD_1`.

## 12. Map & placement → `CityLayoutDefinition` (built: `ToxicCity_WorldPack` + `ToxicCityLayout`)
Districts: Shipyard (berth + static ship), Dispatch (spawn + DispatchHall job-giver, `dispatch_inside`),
Plaza, Market (combat drones), CanalRow (bridge + canal patrol), RelayVault (mission interior, `relay_node`).
Verticality + multiple routes already in; toxic surface collider stripped (hazard), elevated walkways railed.

## 13. Audio → `AudioProfile`
Low industrial drone + distant machinery + dripping; a lonely synth motif when the relay reads wrong.

## 14. Build recipe & cross-links
- **Already built** via `../../systems/WORLD_BLUEPRINT.md` (the ToxicCity blueprint is the reference impl).
- Canon: `MASTER_BUILD_PLAN` §12 W001 · Meta: `../STORY_BIBLE.md` · Beats: `STORY.md` · Factions:
  `../SHIPS_AND_FACTIONS.md` · Gear: `../../09_GEAR_AND_TOOLS.md`.
- **Changeable without code:** story beats, RILL lines, reward, drone placement = data/patcher. Only
  brand-new mechanics need code.
