# Storyboard — Ships & Factions (creative ↔ code sync)

Captures the creative sync (Terry + wife + Gemini, "Project Sync Doc V2", 2026-06-16) **plus** the
coding-lane translation that maps each idea onto ZIPTIDE's existing architecture so it stays modular
and Quest-safe. Creative section = their vision (kept as given); **Code translation** = how TDog/
Architect will build it with the Definition/Blueprint/registry patterns we already have.

Cross-refs: `../design/SHIP_SYSTEM.md`, `../design/SYSTEMS_ARCHITECTURE.md`,
`../design/CONTROLS_AND_FLIGHT.md`, `../ART_AUDIO_CONTENT_ARCHITECTURE.md`, `../design/STORY_AND_HOOKS.md`.

---

## 1. Clean-architecture goal (agreed both sides)
All ship/world/faction systems are **decoupled and data-driven** so we can swap interactions,
environments, and ship modules as the game scales from the first test flight to the 4X endgame.
**Code translation:** this is exactly our existing rule — content is `Definition` ScriptableObjects
resolved by **id via registries** (`DefinitionRegistry<TDef>`), behavior via small interfaces
(`IShockable`-style), never hard refs. New ship/faction content follows the same pattern → no new
paradigm, just new data + a few interfaces.

## 2. Environmental & transition mechanics
**Creative:**
- **Boundary system** — on test flights, leaving the designated path triggers a *diegetic* warning
  (cockpit UI + audio), not a hard wall.
- **Atmospheric transition (world-swap)** — leaving/entering atmosphere plays a visual mask (dense
  cloud, atmospheric burn-in) that *hides a load*: unload the planet scene, load the space/orbital
  scene (or vice versa), seamlessly.

**Code translation:**
- World-swap = our **`TravelCoordinator`** path (the only way scenes change). The cloud/burn-in mask
  is the **ziptide transition effect** already specced in `../design/SANDBOX_TEST_LAB.md` (fade +
  stinger, comfort-safe, **no camera move**) — reused as the atmosphere veil. Space scene streams
  *around the persistent ship* exactly as `SHIP_SYSTEM.md` describes (ship is the constant).
- Boundary = a lightweight `FlightBoundary` MonoBehaviour on the flight path that raises an event the
  cockpit UI/audio subscribe to (tie into `CONTROLS_AND_FLIGHT.md`'s flight scaffold). Diegetic, not a wall.
- Quest perf: only one scene's heavy meshes loaded at a time; the mask covers the unload/load frame.

## 3. Cal's starter ship — Toxic City Scavenger
**Creative:** utilitarian blue-collar salvage rig. Asymmetrical, heavily recycled; exposed heat
sinks, oversized floodlights, chipped hazard paint. **VR cockpit** = commercial transport cab: wide
panoramic view, reinforced roll cages, **floor windows** (vertigo as the ground drops away). Chunky
mechanical **levers + physical toggle switches**. Salvage via grappling arms / tractor beams / cargo
alignment through a universal **`IInteractableSpaceJunk`** interface.

**Code translation:**
- This is the **first `ShipDefinition` + `ShipBlueprint`** in `SHIP_SYSTEM.md`'s pipeline. Kit + paint
  pull from the **Toxic Earth Industrial** + **Salvage / Field Repair** surface families
  (`../ART_AUDIO_CONTENT_ARCHITECTURE.md`) — reuse, no bespoke materials.
- Cockpit = the **seat anchor + windows** the ship audit already requires; floor windows are just
  glass panels in the interior template. Levers/toggles = `XRSimpleInteractable`-driven cockpit
  controls feeding the same flight input events as `CONTROLS_AND_FLIGHT.md`.
- **`IInteractableSpaceJunk`** = a Core interface (sibling to `IShockable`): anything salvageable
  implements it; grapple/tractor tools call it; salvage banks resources into the profile via
  `ProfileEconomy` (the mining/salvage loop in `SYSTEMS_ARCHITECTURE.md`). One interface, many tools
  and many junk types — decoupled.

## 4. Alien faction design pillars
Each faction's engineering reflects the world that shaped it. **Code translation (shared):** each
faction becomes a **`FactionDefinition`** (Definition subclass) that bundles ids for: a **ship kit**,
a **biome** (`BiomeDefinition`), a **surface family**, its **creature archetypes**
(`CreatureDefinition`), and its **economy mechanic**. A world references a faction; the ship assembler
+ world generator read these ids. Adding a faction = author data, not code.

| Pillar | Creative aesthetic | Lore / mechanic tie-in | Code translation (existing systems) |
|---|---|---|---|
| **A. Symbiotics** (grown tech) | asymmetrical organic rib-struts, hardened carapace, bioluminescent interfaces; reusable organic modules | **Grow-a-Garden**: industry via domesticated hyper-accelerated flora/fauna | Biome ↔ **Waterworld/Biofilter** surface family; economy ↔ **garden** loop (`PlantDefinition`, plot→tend→harvest); ship greebles = instanced organic kit pieces; bioluminescent UI = emissive material set |
| **B. Pragmatists** (brutalist modular) | massive blocky geometric "floating factories"; snapping shipping containers + mechanical tracks | **Conveyor-belt factory** building; cyber-humanoids from resource-starved worlds | Economy ↔ **mining/conveyor** loop (`MachineDefinition`, node→extractor→conveyor→storage); ship kit = modular container parts snapped by **`ShipAssembler`** (perfect fit for the kit pipeline); needs a new **Brutalist** surface family entry |
| **C. Ethereals** (acoustic/resonant) | symmetrical, mathematical (tuning forks, obsidian triangles); low-poly but PBR-heavy (reflective metal, dark glass); cymatic doorways | **"Stargate"** atmospheric/spatial anomalies; tech harnessing sound/frequency | Biome ↔ **Stone/Ceremonial Alien** + **Alien Origami/Pattern** families (`../project_art_plan/ALIEN_ORIGAMI_SURFACE_BRIEF.md`); anomalies ↔ TravelCoordinator gateway nodes; cymatic doorway = shader/VFX + a `ResonantGate` interactable |

**Lore alignment (flag for creative):** the spine is Cal-is-alien + the containment network +
Earth-earned-at-the-end (`STORY_AND_HOOKS.md`). Open question for Terry/wife/Gemini: do these factions
**know** about the containment? Are they sealed *inside* the network too, or are some the *builders*
(the "Architects" hinted in `ZiptideFlags`)? Their answer decides whether a faction is ally, obstacle,
or revelation — and we slot that into the per-planet sub-storyboards.

## 5. Implementation pipeline (next steps)
- **Coding (design-now, build-later — gated behind the on-foot slice + save-wiring):**
  1. `IInteractableSpaceJunk` interface in Core (sibling to `IShockable`) + a stub salvage tool.
  2. `FactionDefinition` (Definition subclass) bundling kit/biome/surface/creature/economy ids.
  3. Atmospheric world-swap = reuse `TravelCoordinator` + the ziptide veil; `FlightBoundary` event.
  4. First ship end-to-end = the Toxic City Scavenger via `SHIP_SYSTEM.md`'s build order.
- **Creative:** finalize ship blueprints + the storyboard flow Toxic City → first space encounter;
  for each faction, deliver the AI concept image + the "why this species built it this way."

## 6. Why this stays Quest-safe
One ship/scene heavy-loaded at a time (mask hides swaps); modular kits + GPU-instanced greebles;
faction look carried by **shared surface-family materials**, not per-ship bespoke shaders; salvage/
creatures/economy obey the existing per-world caps in `SYSTEMS_ARCHITECTURE.md`.
