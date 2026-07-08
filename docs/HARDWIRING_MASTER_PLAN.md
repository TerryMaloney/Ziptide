# 🛠️ ZIPTIDE — THE FINAL HARDWIRING MASTER PLAN

> **What this is.** The complete scope of *everything that still needs to be built* before Ziptide is
> the AAA game Terry is aiming for — the not-built pillars, the half-wired systems, and the major new
> asks (interiors, vertical/cavern worlds, next-gen ship design + customization, Fortnite-smooth
> flight, space battles, drivable vehicles, best-in-class garden, a genuinely fun automation layer,
> a home screen). This is a **WHAT list, not a HOW list** — it enumerates requirements, subsystems,
> data, and mechanics so that **Fable 5 owns the implementation** and auto-mode can drop this as the
> execution backlog.
>
> **Ground truth.** Built from a full code survey at head `d6c887f` (see `docs/PROJECT_STATUS.md`).
> The world/movement/research deep-dives were interrupted by a shared session limit; the "how studios
> do it" notes here are from established VR/Quest production technique and should be **backed with
> live-sourced citations in each system's own design doc** before that doc's build phase starts.
>
> **Read alongside:** `docs/PROJECT_STATUS.md` (current state) · `CLAUDE.md` (locked contracts) ·
> `docs/D4_BOOT_ADDITIVE_WORLD_ARCHITECTURE.md` (world/rig contract) · `docs/additions/*_50.md`
> (idea banks to pull from) · `docs/ZIPTIDE_MASTER_BUILD_PLAN.md` (80-world north star).
>
> **▶ To EXECUTE this, open `docs/SPRINT_HARDWIRING.md`** — the phased board (Phase 0–5) that turns this
> WHAT-list into an ordered pull-list and links every per-system design doc under `docs/design/`.

---

## 0 · How to execute this (Fable 5 / auto-mode contract)

1. **Every system below becomes its own design doc first** (the L-feature law): `docs/design/<SYSTEM>.md`
   with the *how*, the data schema, the tests, and the sourced technique research — authored at the
   start of that system's phase, then built pure-core-first (pure C# + EditMode tests, then the thin
   scene/patcher translator). Pull matching rows from the Additions Bank instead of re-inventing.
2. **Nothing breaks a locked contract.** `_Boot` owns the rig; world scenes are content-only;
   `TravelCoordinator.TravelTo` is the only scene change; **never parent the XR rig to a moving hull**;
   non-lethal/all-ages canon; looks-never-stats; definitions-by-string-id, no runtime reflection;
   `ZIPTIDE:` diagnostic tags; Quest budgets enforced by the PerfBudget audit gate.
3. **Every new content type ships with (a) a Definition (ScriptableObject), (b) an audit rule, (c) a
   PerfBudget cap, (d) an ArtModuleRegistry fallback** so logic is never blocked on art.
4. **CI stays the safety net.** Green after every commit; circuit breaker at 3 reds → stop + HANDOFF.
5. **The consistency spine (§13) is law, not a suggestion** — it is *the* answer to "make it all
   consistent across the game." Build every new system *through* those shared spines.

---

## 1 · WORLD-BUILDING AT SCALE — the mass VR map production system

**Why first:** every world-facing ask (interiors, verticality, caverns, density) rides one production
system. Today the generation *spine* is real (`WorldSpec → WorldSpecCompiler → CityLayout →
Experience/Poi/Dressing/Building builders`, `TerrainField` wired, 8 audit gates) but the **content
seam is empty** — `ArtModuleRegistry` has zero registered kits, so every module falls back to a
primitive box. That single gap is why worlds read as empty lots.

### How studios do this (technique backbone — cite in the design doc)
- **Modular kit-bashing:** a small library of grid-snapping, reusable pieces (wall / floor / roof /
  pillar / trim / greeble) builds hundreds of consistent structures. One **trim sheet + texture atlas
  per kit** → one material → one draw-call family. Ziptide already bakes atlases in the Forge (E1.x).
- **Interior mapping (parallax shader):** fakes lit rooms behind windows with **zero geometry** — the
  cheapest possible way to make a whole VR city read as inhabited. Use on *every* building.
- **Real walkable interiors** only on designated "enterable" buildings (see §2), gated by portal
  culling so only the current room renders.
- **POI stamping:** hand-authored set-pieces (shop, shrine, garden grove, machine bay, story beat)
  stamped into procedural layouts at valid anchors — the handcrafted/procedural hybrid.
- **PCG with authored control:** grammar/WFC for building + street assembly; seeded per-world so a
  layout is reproducible and tunable; biome definitions pick the kit + palette per world for coherence.
- **Quest budgets (the PerfBudget gate enforces):** single-pass-instanced, static batching + GPU
  instancing for props, texture atlases + mipmaps, baked lighting (no realtime shadows), aggressive
  occlusion/portal culling, LODs + distance impostors/billboards, MSAA + fixed-foveated. Hold 72–90 fps.
- **Streaming:** additive chunk loading + LOD for large worlds on top of the existing additive arch.

### What must be built
- [ ] **Building kit system** — extend `BuildingStyle` into a *kit*: a family of Forge-baked modules
  (facade panels, windows, doors, roof caps, balconies, greebles) **registered in
  `ArtModuleRegistry`**. Ship **≥4 real kits** (salvage_row, toxic_tenement + 2 new biomes) so worlds
  stop falling back to primitives. *(This is the highest-visible-win item in the whole plan.)*
- [ ] **Interior-mapping window shader** (URP) + a window material variant applied by the building
  builder — instant "lit rooms" on all buildings, no geometry cost.
- [ ] **Biome→kit/palette mapping** in `WorldSpec` so each world coherently selects its look.
- [ ] **POI catalog** — extend `WorldPoiBuilder` with a registry of authored POI prefabs stamped at
  layout anchors; ≥12 POI types (market, shrine, garden grove, repair bay, transit, story node…).
- [ ] **Density/scatter pass** — fulfill `ScatterField` + `WorldDressingBuilder` with real prop kits
  (registry-driven) so streets have life (crates, pipes, foliage, signage, debris).
- [ ] **Per-content-type PerfBudget caps** + a `WORLD_CONTENT`/`KIT_FULFILLED` audit rule so an empty
  registry blocks the build instead of silently shipping boxes.
- **Bank rows:** pull from `WORLDS_50.md` (traversal/secrets/weather) as kits land.

---

## 2 · BUILDING INTERIORS

**Current:** buildings render as exteriors only; there is no way inside. `RoomPartitioner` (BSP room
graph) exists as a pure core but is **not wired to any walkable interior**.

### What must be built (two tiers — do both)
- [ ] **Tier A — faked interiors everywhere:** the interior-mapping window shader from §1 (every
  building looks inhabited at zero cost).
- [ ] **Tier B — real walkable interiors on "enterable" buildings:**
  - [ ] `enterable` flag + interior-kit id on `BuildingStyle`.
  - [ ] **Interior kit** (floor / wall / ceiling / doorway / stair / furniture / fixture modules)
    registered in `ArtModuleRegistry`, Forge-baked, atlas'd.
  - [ ] `RoomPartitioner` → **interior mesh builder**: BSP rooms → walkable geometry + doorways +
    a **portal/occlusion system** so only the current room draws (mandatory for Quest).
  - [ ] **Door + threshold system** (grab/push VR doors; travel-trigger doorways where an interior is
    its own additive sub-scene for big buildings).
  - [ ] **Interior POI props** — loot, machines, garden plots, NPCs, story nodes placed by a room-POI
    pass, all registry-driven.
  - [ ] Interior audit rule (reachability + spawn-safe + budget).
- **Consistency:** interiors use the *same* Forge kit + registry + budget discipline as exteriors.
- **Own design doc:** `docs/design/BUILDING_INTERIORS.md`.

---

## 3 · EXTERIOR ENVIRONMENTS — verticality, caverns, elevated terrain

**Current:** `TerrainField` is a 2D height-field (fBM + domain warp). **A heightmap mathematically
cannot make overhangs, caves, or tunnels** — so today every world is single-level ground. Terry wants
environments that *rise above the ground*, caverns, and varied vertical structure.

### How studios do this (technique backbone — cite in the design doc)
- Heightmaps → no overhangs. Options for caves/verticality: **mesh/voxel terrain** (marching cubes /
  transvoxel / dual contouring — powerful but heavy for Quest at scale), **modular cavern kits**
  (authored tunnel/chamber/shaft segments placed like buildings — Quest-friendly, the recommended
  default), **layered sub-levels** (an elevated "mesa/platform" layer + an underground layer over the
  same heightfield), and **vertical POIs** (towers, spires, floating structures).
- Recommended for Quest: **modular cavern + vertical kits + layered sub-levels**, *not* full voxel —
  reserve voxel/mesh-cave for a few hero set-pieces.

### What must be built
- [ ] **Cavern kit** — tunnel / chamber / junction / vertical-shaft / stalactite modules in
  `ArtModuleRegistry`; a `CaveBuilder` that strings them into networks; portal culling.
- [ ] **Underground world-layer** (or dedicated cave world scenes reached by travel) — caves as
  content-only world scenes honoring the boot/rig contract.
- [ ] **Elevated/mesa layer** — platforms, cliffs, floating islands above the heightfield; a
  `WorldSpec` elevation-layer field.
- [ ] **Vertical traversal (VR-native, on-brand):** **ziplines** (the game is literally *Ziptide* —
  make them a signature), **elevators/lifts**, **jump-pads**, **grapple**, and **climbable surfaces**
  (VR hand-over-hand climbing is a headline feel — reuse the XRI grab layer).
- [ ] **Verticality-aware reachability audit** (extend `GridReachability` / `WorldReachabilityAudit`
  to multi-level with step/zip/climb edges) so POIs above ground are provably reachable.
- **Bank rows:** `WORLDS_50.md` traversal/secrets rows.
- **Own design doc:** `docs/design/VERTICAL_AND_CAVERN_WORLDS.md`.

---

## 4 · SPACESHIP DESIGN + CUSTOMIZATION — the next-gen ship

**Current:** `ShipHullBuilder` assembles a hull from **primitive boxes** — the "N64 Star Fox" look
Terry called out. No customization/skin/wrap system. `ShipDefinition` + `SHIPS.md` exist as a data
seed only.

**Thesis:** do for ships exactly what the Forge did for creatures — a data-driven, textured, baked,
**next-gen procedural ship pipeline**, plus a Fortnite-style cosmetic layer.

### What must be built
- [ ] **Ship Forge** — a Forge recipe family for ship modules: **hull core, wings, engines, cockpit,
  weapon hardpoints, fins, greebles** — each a `ForgeRecipeDefinition`, UV'd, textured (normal/MSA/
  emissive), baked to ASTC prefabs like the creature/weapon Forge.
- [ ] **`ShipChassisDefinition` + `ShipModuleDefinition` catalog** — assemble a ship from a chassis +
  module slots (like Forge creatures assemble from limb recipes). Ship **≥6 chassis** across silhouettes
  (interceptor, hauler, gunship, explorer…).
- [ ] **Loadout layer (functional parts):** swappable wings/engines/hardpoints that **do** change
  stats (speed, handling, weapon slots) — the No Man's Sky / Star Citizen module model.
- [ ] **Cosmetic layer (Fortnite wraps):** `ShipWrapDefinition` — liveries, decals, emissive palettes,
  material skins that **never change stats** (looks-never-stats law). Fully swappable. This is the
  "wraps like Fortnite" ask.
- [ ] **In-VR hangar / customization station** — grab-and-attach parts, apply wraps, preview; lives in
  the home/ship-hub (§10). Selections persist in `PlayerProfile`.
- [ ] Hardpoints feed **space combat** (§6). Ship stats feed **flight** (§5).
- **Consistency:** shares the Forge, the ArtModuleRegistry, and the *one cosmetic layer* (§13) with
  vehicles/weapons/avatar. **Own design doc:** `docs/design/SHIP_FORGE_AND_CUSTOMIZATION.md`.

---

## 5 · SPACE FLIGHT — Fortnite-smooth 6DOF + the atmosphere→space transition

**Current:** `FlightModel` is **pure math only** (tested), unwired. `ShipCastOffRuntime` is a rails
"PUNCH IT" takeoff. There is **no playable flight scene**. Terry wants flight that feels like a gamepad
in Fortnite — *more than turn-and-look*, "almost like using the character controls except for the ship."

### How VR does smooth flight without nausea (technique backbone — cite in the design doc)
- **Cockpit reference frame:** the player sits in a **fixed cockpit** and the *world* moves around it —
  the single biggest anti-nausea technique for vehicles. This also satisfies the **"never parent the
  rig to a moving hull"** contract: the rig sits in a stabilized cockpit frame; the ship + world move
  relative to it, the rig is never rigidly parented to a physics body.
- **Dynamic comfort vignette / tunneling** that tightens on acceleration, roll, and boost.
- **Full 6DOF mapped like a gamepad:** left stick = throttle + yaw, right stick = pitch + roll,
  triggers = boost / air-brake, face buttons = fire / abilities, with deadzones + response curves for a
  smooth analog feel. Optional flight-assist (auto-level, speed cap) as a comfort default.

### What must be built
- [ ] **`FlightController`** (scene-side) driving the ship rig from `FlightModel` + VR input — full
  pitch/yaw/roll/throttle/strafe/boost/brake, tuned to feel like a gamepad (this is the core deliverable).
- [ ] **Cockpit-relative stabilized frame** + **comfort vignette** + comfort presets (assist on/off,
  vignette strength, snap/smooth). Seated-first.
- [ ] **Atmosphere flight** — fly freely around the planet (bounded flight volume above a world).
- [ ] **Atmosphere→space transition:** an exit trigger (altitude/zone) → **fade cutscene**
  (`ZiptideTransitionEffect`, already named in the master plan) → `TravelCoordinator.TravelTo` the
  **space world scene**; re-entry reverses it. Space is *just another world scene* — Terry's explicit
  render-saving design.
- [ ] **Space world scene** — space SkyVista, no terrain, floating POIs (stations, derelicts,
  asteroids), unconstrained 6DOF.
- [ ] Flight tuning/telemetry (`ZIPTIDE: FLIGHT_*`) for on-device feel passes.
- **Bank rows:** `SPACEFLIGHT_50.md`. **Own design doc:** extend `design/SPACEFLIGHT_PHYSICS.md`.

---

## 6 · SPACE COMBAT — ship weapons, abilities, battles, salvage

**Current:** none. Depends on §4 (hardpoints) + §5 (flight + space world).

### What must be built
- [ ] **`ShipWeaponDefinition`** — hardpoint-mounted, data-driven (blaster, missile, beam, mining
  laser). Fires from ship hardpoints; consistent with the ground `ItemDefinition` philosophy.
- [ ] **Ship ability system** — boost, shield, EMP, cloak, deployable drone, tractor/salvage beam
  (button-mapped, cooldown-based).
- [ ] **Damage model = armor-only** (consistent with the locked combat decision: recharging armor,
  no health bar) — disable, don't destroy; **non-lethal canon** = you *disable/salvage* enemy ships.
- [ ] **Enemy ship AI** — adapt `BotBrain` patterns to 3D flight (pursue / evade / strafe / formation);
  turrets; a capital-ship set-piece.
- [ ] **Space encounters as POIs** — patrols, ambushes, derelict salvage, escort — stamped into the
  space world; tie rewards into `ProfileEconomy` (salvage → resources → ship upgrades).
- **Bank rows:** `SPACEFLIGHT_50.md` combat rows. **Own design doc:** `docs/design/SPACE_COMBAT.md`.

---

## 7 · DRIVABLE VEHICLES — a per-planet fleet

**Current:** none exist beyond the ship. Terry wants a whole section: many vehicles per planet, varying
speeds/traversal methods, complex mechanics like the creatures.

**Thesis:** a vehicle system that is the **ground sibling of the ship** — same Forge, same customization,
same comfort/input layer, same seat/mount system.

### What must be built
- [ ] **`VehicleDefinition` + `VehicleModuleDefinition`** — data-driven chassis + parts, forged like ships.
- [ ] **`VehicleController`** — drive feel per locomotion type with the shared comfort layer (cockpit
  frame, vignette). Complex mechanics per type: suspension, hover, buoyancy, traction, drift.
- [ ] **Locomotion archetypes / per-biome catalog (≥8–12 concepts):** rover, hoverbike, **boat/skiff**
  (tide worlds — on-brand), mech/walker, glider, drill-crawler (caverns), tram/zip-car, grav-sled.
  Varying speed + traversal (some climb, some fly low, some go underwater/underground).
- [ ] **Seat / mount / enter-exit system** (shared with the ship's boarding station).
- [ ] **Vehicle wraps** (`VehicleWrapDefinition`) via the one cosmetic layer (§13).
- [ ] **Garage / spawn** in the home-hub; vehicles persist in `PlayerProfile`.
- [ ] Traversal integration with §3 verticality (climbers/flyers reach elevated POIs).
- **Own design doc:** `docs/design/DRIVABLE_VEHICLES.md`.

---

## 8 · GROW-A-GARDEN — better than Roblox "Grow a Garden"

**Current:** `GardenService` grow loop + overripe decay (tested), **only 4 plants**, no hands-on tools,
fresh-bonus deferred. `GARDEN_50.md` already banks ~50 concrete ideas — **pull from it, don't duplicate.**

**Bar:** exceed Roblox Grow a Garden — its hooks are variety, mutations/rarities, weather events, pets
that help, offline growth, sprinklers/automation, giant/rare crops, seasons, trading, a seed economy.
Ziptide's edge is **VR-native hands** (plant/water/harvest physically) + tie-ins to worlds, hazards,
automation, and the economy.

### What must be built (all extend `GardenService` / `PlantDefinition`)
- [ ] **Plant variety** — ≥20 plants across biomes (each a `PlantDefinition` + Forge/flora kit).
- [ ] **Genetics / mutations** — deterministic seeded genes (speed/yield/size), cross-pollination,
  **giant/rare crops** with a two-handed pull for a big payout (the screenshot moment).
- [ ] **Hands-on VR tools** — **watering can with pour physics** (tilt past ~60°), prune snips, seed
  placement (holsterable seeds → plant onto soil); tending literally with your hands.
- [ ] **World/hazard interactions** — radiation → mutation rolls, static-bloom windows, flood
  auto-water, spore weeds you grab out — gardens react to the world's weather/hazards.
- [ ] **Automation** — sprinklers, fertilizer machines (bridges into §9 conveyor layer).
- [ ] **Harvest juice** — pop + haptic + chime + pooled produce; a daily-tally readout; a belt almanac.
- [ ] **Meta** — seasons, companion planting, a dedicated **garden hub world**, offline growth (already
  via `ProfileEconomy`), and **trading** (bridges into MP economy).
- **Own design doc:** `docs/design/GARDEN_AAA.md` (curated from `GARDEN_50.md`).

---

## 9 · CONVEYOR / AUTOMATION — the unique, fun, *cool* factory layer

**Current:** `ProductionGraph` is an **abstract** factory sim; `NodeKind.Conveyor/Splitter/Combiner`
are "representative visuals only, never truth." **Zero physical belts.** Terry: this is a major, unique
feature — it has to work *and* be genuinely fun and extremely cool.

**Thesis:** keep the graph as the backend truth, but build a **physical, VR-native, satisfying**
automation layer on top — the Factorio/Satisfactory dopamine, delivered with your hands.

### What must be built
- [ ] **Physical build system on a snap grid** — placeable **belts, splitters, mergers, inserters,
  machines**; grab a segment, snap it, hear the click, feel the haptic. Hand-placement is the VR-unique
  joy — make placement itself satisfying.
- [ ] **Items physically ride belts** — pooled visual items (**adopt `GamePool`**) flowing along
  segments; reach in and grab one; hand-feed a machine. The graph resolves throughput; the visuals make
  it *readable and satisfying*.
- [ ] **Machines process recipes** (existing `ProductionGraph`/`RecipeDefinition` as backend) with
  Forge-gait-style animation, glowing energy, throughput readouts.
- [ ] **Power/fuel system** + upgrade tiers (spaghetti → elegant progression) + **blueprints** (copy a
  layout).
- [ ] **The "cool/unique" hooks** — flow visualization, the "watch it run" moment, a **conductor mode**
  (ride/zipline your own line — ties to §3 ziplines), and cross-system payoff: auto-farms (§8),
  auto-mining, ship/vehicle fuel (§4/§7), goods → economy, even auto-production of gear.
- **Bank rows:** `INDUSTRY_50.md` (conveyors + automation). **Own design doc (do this one carefully —
  it's the fun-defining feature):** `docs/design/AUTOMATION_CONVEYORS.md`.

---

## 10 · HOME SCREEN / SHELL

**Current:** boots straight into a world (`ZiptideConstants.FirstWorldScene`); only an in-VR dev menu.
No real home screen. The north star already says **"the ship = hub / world-select."**

### What must be built
- [ ] **Diegetic home = the player's ship interior / quarters** — the main menu *is* a place you stand
  in (fits the north star, avoids a flat 2D menu). Minimal 2D title only on cold boot.
- [ ] **Shell flow** — new game / continue (wire `SaveSystem`, §12), settings (**comfort presets**),
  save-slot UI.
- [ ] **World select via a galaxy map / helm** — reuse `ConquestGalaxy` + the named `GalaxyMap`; travel
  via `TravelCoordinator`.
- [ ] **The hub hosts the other shells** — ship **hangar** (§4), vehicle **garage** (§7), garden
  **almanac** (§8), **wardrobe** (cosmetics, §13), story recap.
- **Own design doc:** `docs/design/HOME_HUB.md`.

---

## 11 · REMAINING NOT-BUILT SYSTEMS

- [ ] **Adaptive audio** — `AdaptiveAudioManager`: layered stems (explore / tension / combat), ducking,
  per-biome beds, state-driven crossfades. Today only `AudioDirector` scene crossfade. (`ART_AUDIO_50`.)
- [ ] **RILL voice-over** — VO for the 72+ authored lines; clips slot into `RillLineLibrary` beside the
  existing subtitles; subtitle/VO sync. (Consider AI-VO pipeline; keep bible canon locked.)
- [ ] **Creature ecology** — nests, packs, territory, predator/prey, day-night, population sim + spawn
  director; build on the ~10 behaviors + Forge gait pipeline. (`CREATURES_50`.)
- [ ] **Weapon depth** — ADS, reload / magazine, recoil + haptic feel; the remaining `CONTROL_SCHEME`
  rows; keep ground + ship weapon feel consistent. (`COMBAT_GAMEPLAY_50`.)

---

## 12 · REMAINING PARTIAL SYSTEMS — finish the last mile

- [ ] **Save wiring** — `SaveSystem` into `_Boot` + travel-autosave (serialize/migrate logic is already
  built + tested; it's just not wired). **Do this in Phase 0 — nothing persists until it's done.**
- [ ] **Building kits** — fulfill `ArtModuleRegistry` (covered in §1; the #1 "worlds look empty" fix).
- [ ] **GamePool adoption** — swap `Instantiate` at projectile / belt-item / creature-spawn call-sites
  onto the tested pool (belt items in §9 depend on this).
- [ ] **Online combat sync (A6.2)** — host-authoritative fire / hit / score over the live Photon
  presence transport (presence already streams at 20 Hz; combat authority is the missing half).
- [ ] **Story delivery** — RILL memory + callbacks, author beats for the worlds that *exist*, wire the
  4 dialogue-only endings into real ending flow; drive from the Haiku workshop. (`STORY_50`.)
- [ ] **Tidefront war table (B2/B3)** — the VR holo-table + mission modifiers over the built, tested sim.
- [ ] **MP progression / augments (MP100)** — stats, credits, unlock ladders, augments, dual-wield.

---

## 13 · THE CONSISTENCY SPINE (Terry's "make it all consistent" — this is law)

Every system above is built *through* these shared spines so the game feels like one thing:

| Shared spine | What rides it |
|---|---|
| **The Forge** (procedural → UV → texture → bake → atlas) | buildings, interiors, cavern kits, ships, vehicles, creatures, weapons, props, flora — one art pipeline, one material philosophy, one budget. |
| **`ArtModuleRegistry`** (id → kit, primitive fallback) | every kit/module/skin registered by id; logic never blocks on art. |
| **Definitions-by-string-id** (ScriptableObjects, no reflection) | new plant / ship / vehicle / building / weapon = a new asset, never new code. |
| **One cosmetic layer** (`WrapDefinition` / `SkinDefinition`, looks-never-stats) | ship wraps, vehicle skins, weapon skins, avatar — one wardrobe UI, one rule. |
| **One movement + comfort layer** (input map, vignette, presets, seat/mount, stabilized frame) | character, ship, vehicle — same feel, same comfort, same rig-safety contract. |
| **One world contract** (`TravelCoordinator` + `SpawnMarker` + `_Boot` rig + fade) | ground, cavern, elevated, space — all just world scenes; rig never parented to a hull. |
| **One economy + save spine** (`PlayerProfile` / `ProfileEconomy` / `SaveSystem`) | gardens, factories, ships, vehicles, cosmetics, story flags, progress — one profile, offline-resolved, persisted. |
| **One audit + budget discipline** (`*AuditRules` + PerfBudget + CI) | every new content type gets a gate; CI stays the safety net. |

---

## 14 · EXECUTION SEQUENCING (for auto-mode)

Phased so dependencies resolve and each phase is CI-green + device-verifiable. Pure-core-first,
tests, audit gate, device pass, HANDOFF at every step.

- **Phase 0 — Foundations & consistency (unblocks everything):** wire `SaveSystem` into `_Boot`;
  scaffold the shared **cosmetic layer** and **comfort/input layer** (§13); **fulfill `ArtModuleRegistry`
  with the first building kit**; **GamePool adoption**; add audit gates + PerfBudget caps for the new
  content types. *Closes "nothing persists" + starts closing "worlds look empty."*
- **Phase 1 — Worlds feel real:** building kits (§1) + interior mapping + first walkable interiors (§2);
  verticality + cavern kits + traversal / ziplines / climb (§3); POI + density passes. *Biggest visible win.*
- **Phase 2 — The ship pillar:** Ship Forge + chassis / modules / wraps (§4); in-VR hangar; the
  `FlightController` Fortnite-smooth 6DOF + comfort (§5); atmosphere→space transition + space world;
  **home-screen / ship-hub + world select (§10).**
- **Phase 3 — Space & ground travel:** space combat — weapons / abilities / enemy ships / salvage (§6);
  drivable vehicles catalog + controller + garage (§7).
- **Phase 4 — Systems depth:** the automation / conveyor layer (§9 — its own design doc first, get the
  *fun* right); garden AAA (§8); creature ecology + weapon depth (§11).
- **Phase 5 — Multiplayer, narrative & polish:** online combat sync; Tidefront war table; MP progression
  / augments (§12); story delivery + endings; adaptive audio + RILL VO (§11).

> **Each phase = its own design doc(s) first, then pure-core-first build, then a device pass.** Pull
> matching rows from the Additions Bank as you enter each system. Keep CI green; honor every locked
> contract in §0.

---

### Locked decisions (Terry, this session — binding for auto-mode)
1. **First to build:** **Phase 0 → Phase 1** (foundations, then worlds-feel-real). Do *not* jump to the
   ship pillar first — it rides on the Phase 0 foundations.
2. **Home screen:** **diegetic ship hub** (§10) — the menu *is* the ship interior; helm/galaxy world
   select; hangar / garage / almanac / wardrobe live there. Minimal 2D title only on cold boot.
3. **Caverns / verticality:** **modular kits, Quest-safe** (§3) — authored tunnel/chamber/shaft + mesa/
   platform modules + an elevated sub-layer. **No voxel/mesh terrain** (reserve only for rare hero
   set-pieces if ever justified). Hold 72–90 fps at scale.
4. **Combat lethality:** **non-lethal disable + salvage** (§6) — EMP/armor-break, salvage the wreck into
   the economy. Consistent with the whole game's all-ages / non-lethal canon, ground and space.
