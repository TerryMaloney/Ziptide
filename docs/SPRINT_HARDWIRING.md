# 🛠️ SPRINT — THE FINAL HARDWIRING (Fable 5 execution board)

> **This is the execution board for `docs/HARDWIRING_MASTER_PLAN.md`.** The master plan is the WHAT;
> this board is the ORDER + the pull-list. Fable 5 works top-down: open the phase, open each row's
> design doc, build it pure-core-first (pure C# + EditMode tests → thin scene/patcher translator),
> keep CI green, device-pass, HANDOFF. **You own the how — every row leaves deliberate room to expand;
> the "🚀 Room to expand" section in each design doc is an invitation, not a fence.**
>
> **Read first:** `HARDWIRING_MASTER_PLAN.md` §0 (execution contract) + §13 (the consistency spine —
> build everything *through* it) · `CLAUDE.md` (locked contracts) · your phase's design docs below.
>
> **Sourced technique backbone:** `docs/design/VR_TECHNIQUE_RESEARCH.md` — the cited "how studios do it"
> reference that backfills each design doc's *Technique research TODO* (modular kits, interior mapping,
> PVS/occlusion, WFC, Quest budgets, VR comfort/customization). Verify+synthesis was cut short by a
> session limit — 📎 claims are sourced-but-unconfirmed; re-run the harness after the 9pm-UTC reset.
>
> **Locked build-shape decisions (Terry):** Phase 0→1 first · diegetic ship-hub home · modular-kit
> caverns (no voxel) · non-lethal disable+salvage combat.

Legend: ⬜ not started · 🟡 in progress · ✅ shipped · 📄 design doc exists · ✍️ design doc = skeleton
(expand before/while building) · 🔗 reuse existing system.

---

## PHASE 0 — Foundations & the consistency spine  *(build this first; it unblocks everything)*
The goal: stand up the shared spines from master-plan §13 so every later system snaps onto them, and
close the two silent gaps (nothing persists; worlds render primitives).

| # | Row | Design doc | Notes |
|---|-----|-----------|-------|
| 0.1 | **Wire `SaveSystem` into `_Boot` + travel-autosave** | 🔗 `SYSTEMS_ARCHITECTURE.md` | Logic is built + tested (`ProfileSerializer`); just not wired. Autosave on travel + on quit. Nothing persists until this lands. |
| 0.2 | **First building kit → fulfill `ArtModuleRegistry`** | 📄 `ART_REGISTRY.md` + ✍️ `WORLD_BUILDING_AT_SCALE.md` | Register ≥1 real Forge-baked kit so worlds stop falling back to primitive boxes. Add a `KIT_FULFILLED` audit rule. **The #1 visible fix.** |
| 0.3 | **Cosmetic layer** — ✅ ALREADY BUILT (reconciled): `CosmeticDefinition`+`CosmeticLocker`+`CosmeticAuthor`, 6 authored, ItemFactory applies | ✍️ `CONSISTENCY_SPINE.md` §A | Do NOT rebuild. Remaining: Ship Forge consumes `ShipLivery`; add `VehicleSkin` enum value; wardrobe UI in the hub. |
| 0.4 | **Movement + comfort layer scaffold** | ✍️ `CONSISTENCY_SPINE.md` + 🔗 `CONTROL_SCHEME.md` | Shared input map + comfort presets (vignette, snap/smooth, assist) + seat/mount + stabilized reference-frame helper. Character/ship/vehicle all consume it. |
| 0.5 | **`GamePool` adoption** | 🔗 `SYSTEMS_ARCHITECTURE.md` | Swap `Instantiate` at projectile/creature-spawn call-sites onto the tested pool. Belt items (Phase 4) depend on this pattern. |
| 0.6 | **Audit + PerfBudget caps for new content types** | 🔗 `PerfBudgetAuditRules` | Every new type (kit/interior/ship/vehicle/belt) gets a budget cap + an audit rule as it's introduced. |

---

## PHASE 1 — Worlds feel real  *(biggest visible win)*

| # | Row | Design doc | Notes |
|---|-----|-----------|-------|
| 1.1 | **Building kit system (≥4 kits) + biome→kit/palette** | ✍️ `WORLD_BUILDING_AT_SCALE.md` + 📄 `CITY_DESIGN.md` | Modular Forge kits per biome; grammar/WFC assembly already exists. |
| 1.2 | **Interior-mapping window shader (all buildings)** | ✍️ `BUILDING_INTERIORS.md` | Faked lit rooms behind windows, zero geometry. Cheapest city-readability win. |
| 1.3 | **Walkable interiors on enterable buildings** | ✍️ `BUILDING_INTERIORS.md` | `RoomPartitioner` → interior mesh + interior kit + portal culling + doors + interior POIs. |
| 1.4 | 🟡 **Vertical & cavern kits + traversal** — *CLAIMED by Fable 5 (2nd), leapfrogging the architect's building/interior cluster (2026-07-09)* | ✍️ `VERTICAL_AND_CAVERN_WORLDS.md` | Modular tunnel/chamber/shaft + mesa/platform + floating structures; ziplines, elevators, grapple, climb. Multi-level reachability audit. **Pure cores shipped first (`MultiLevelReachability` + `ZiplineCore` + `ClimbCore`, EditMode-tested); cavern-KIT registration goes in a separate `CavernKitLibrary` (mirrors the architect's `BuildingKitLibrary`, different ids — no registry collision).** |
| 1.5 | **POI catalog + density/scatter pass** | ✍️ `WORLD_BUILDING_AT_SCALE.md` + 🔗 `ScatterField` | ≥12 POI types stamped; real prop kits so streets have life. |

---

## PHASE 2 — The ship pillar & the shell

| # | Row | Design doc | Notes |
|---|-----|-----------|-------|
| 2.1 | 🟡 **Ship Forge (chassis + modules, ≥6 chassis)** — *CLAIMED T-Dog/Fable (ship pillar sprint 2026-07-09)* | ✍️ `SHIP_FORGE_AND_CUSTOMIZATION.md` + 📄 `SHIP_SYSTEM.md` | **v1 SHIPPED: six chassis presets (pure, tested) + ShipRefit reproportions the berth hull live (silhouette/livery/journey-decals/nameplate/per-chassis hum).** Picasso's baked Forge meshes supersede through the same parent (ShipHullBuilder's invitation). |
| 2.2 | 🟡 **Ship loadout + wraps** — *CLAIMED T-Dog/Fable* | ✍️ `SHIP_FORGE_AND_CUSTOMIZATION.md` | **v1 SHIPPED: ShipLoadoutCore (10 modules, tradeoffs, floors, wrap-invariance by construction) + ShipLocker persistence + livery apply (the 0.3 wired-nowhere seam CLOSED).** Remaining: loadout→FlightModel feed (coordinate the ShipDefinition seam with Reasonbox). |
| 2.3 | **`FlightController` — Fortnite-smooth 6DOF + comfort** | 📄 `SPACEFLIGHT_PHYSICS.md` + 📄 `CONTROLS_AND_FLIGHT.md` | Drive `FlightModel` from VR input; cockpit reference frame; never parent rig to hull. |
| 2.4 | **Atmosphere→space transition + space world scene** | 📄 `SPACEFLIGHT_PHYSICS.md` | Fade cutscene (`ZiptideTransitionEffect`) → `TravelCoordinator` → space = another world scene. |
| 2.5 | 🟡 **Diegetic ship-hub home screen + world select** — *partial, T-Dog/Fable* | ✍️ `HOME_HUB.md` | **HANGAR surface SHIPPED (live refit tiles + holo stat readout, beside the Quarters in every berth).** Helm/world-select already existed (ShipBoardingStation). Remaining: cold-boot title, save slots, galaxy-map visual upgrade, garage/almanac surfaces. |

---

## PHASE 3 — Space & ground travel

| # | Row | Design doc | Notes |
|---|-----|-----------|-------|
| 3.1 | **Space combat — weapons / abilities / enemy ships / salvage** | ✍️ `SPACE_COMBAT.md` | Non-lethal disable + salvage; adapt `BotBrain` to 3D; armor-only damage. |
| 3.2 | **Drivable vehicles — catalog + controller + garage** | ✍️ `DRIVABLE_VEHICLES.md` | Ground sibling of the ship; shares Forge, wraps, comfort, seat/mount. |

---

## PHASE 4 — Systems depth

| # | Row | Design doc | Notes |
|---|-----|-----------|-------|
| 4.1 | 🟡 **Automation / conveyor layer** — *CLAIMED by Fable 5 architect (2026-07-09, post-rate-limit); ship/flight = T-Dog's, traversal/caves = traversal lane's, art = Picasso's* | ✍️ `AUTOMATION_CONVEYORS.md` | Physical hand-built belts; items ride belts (GamePool); conductor mode; the unique feature. **Pure cores first: BeltLattice (grid placement + item flow + blocking/compression), tested; then the scene translators (snap-place, riding items, machine feeds).** |
| 4.2 | **Garden AAA (beat Roblox)** | ✍️ `GARDEN_AAA.md` | 20+ plants, genetics, watering-can pour physics, giant crops, hazard tie-ins, automation. |
| 4.3 | **Creature ecology** | ✍️ `CREATURE_ECOLOGY.md` + 🔗 `CREATURES_50.md` | Nests, packs, territory, predator/prey, population sim on the ~10 behaviors + Forge gaits. |
| 4.4 | **Weapon depth (ADS / reload / feel)** | 📄 `ABILITIES_AND_ARSENAL.md` + 🔗 `CONTROL_SCHEME.md` | Ground + ship weapon feel kept consistent. |

---

## PHASE 5 — Multiplayer, narrative & polish

| # | Row | Design doc | Notes |
|---|-----|-----------|-------|
| 5.1 | **Online combat sync (A6.2)** | 🔗 `PVP_1V1_MODE.md` | Host-authoritative fire/hit/score over the live Photon presence transport. |
| 5.2 | **Tidefront VR war table (B2/B3)** | 📄 `TIDEFRONT_AAA.md` | Holo-table + mission modifiers over the built, tested sim. |
| 5.3 | **MP progression / augments (MP100)** | 📄 `MP100_BOARD.md` | Stats, credits, unlock ladders, augments, dual-wield. |
| 5.4 | **Story delivery + endings** | 📄 `STORY_AND_HOOKS.md` + 🔗 Haiku workshop | RILL memory/callbacks; beats for worlds that exist; wire the 4 endings. |
| 5.5 | **Adaptive audio + RILL VO** | 📄 `ADAPTIVE_AUDIO.md` | Layered stems + ducking; VO into `RillLineLibrary`. |

---

## Standing rules for this board
- **Pure-core-first.** Pure C# + EditMode tests before any MonoBehaviour/scene work.
- **Ride the machine.** Extend existing systems (Forge, ArtModuleRegistry, WorldSpec, Definitions,
  TravelCoordinator, ProfileEconomy). New content = a new asset, not new code.
- **Placeholder-first art.** Ship the mechanic behind a primitive via the registry fallback; hand the
  look to the Forge/Picasso via `ART_REGISTRY.md` ids. Never block a mechanic on art.
- **Gate everything.** Audit rule + PerfBudget cap per new content type. CI green every commit;
  circuit breaker at 3 reds → stop + HANDOFF.
- **Expand freely.** Each design doc's "🚀 Room to expand" is where you make it excellent — add
  systems, deepen loops, propose new worlds/mechanics, as long as the consistency spine (§13) holds.
