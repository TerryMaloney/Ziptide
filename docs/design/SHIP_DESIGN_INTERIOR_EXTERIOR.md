# 🚀 SHIP DESIGN — INTERIOR & EXTERIOR (make it make sense, make it cool)

**Status:** DESIGN (Round 5 research synthesis). No code changed by this doc. Companion to
`docs/systems/SHIPS.md`, `docs/design/SHIP_SYSTEM.md`, `docs/design/SHIP_FORGE_AND_CUSTOMIZATION.md`,
`docs/design/HOME_HUB.md`. **Extends the existing ship system — does not fork it.** Everything maps
onto `ShipDefinition` / `ShipSlotDef` / `ShipHullBuilder` / `ShipRefit` / `ShipBoardingStation` / the
Forge visual layer.

Grounded in Star Wars/ILM (Cantwell), Star Citizen, The Expanse, No Man's Sky, Halo, Elite VR, Lone
Echo, Star Trek: Bridge Crew, Firefly/Serenity, Subnautica, Hardspace: Shipbreaker, FTL, EVE, KSP.

---

## 0. The core finding — THE SLOT is the atom that makes a ship "make sense"
The user's bar ("ships should make sense in the way they're designed, inside and out") is solved by one
idea: **each upgrade slot pairs an EXTERIOR look with an INTERIOR room.** Install a bigger engine →
the nozzle block grows *outside* AND the engine-room cell swaps to a walkable one *inside*, with a
porthole that looks out at the exact engine you installed. You never simulate continuous volume — you
**pair a mesh outside with a room prefab inside through the same slot ID.** That's the Quest-cheap way
Star Citizen's walk-in module bays feel real, and it's the heart of the whole doc.

Two supporting pillars:
- **Salvage decorates home.** In a collect-salvage game the ship's trophy shelves turn recovered parts
  into decor — collecting *is* decorating (Serenity as the show's "tenth character").
- **The two-mode loop:** *docked = walk around and physically touch everything; flying = seated at the
  helm.* This is the magic of a VR ship.

**Already built (the foundation to extend):** `ShipDefinition` (hullSize, cockpitSeatLocalPos,
boardingDoorLocalPos, flight feel, `ShipSlotDef` list: slotKind engine/scanner/cargo/plating/special +
acceptedItemIds + localPos), `ShipHullBuilder` (graybox), `ShipRefit`, `ShipLoadoutCore`,
`ShipBoardingStation`, `ShipFlightRuntime`, `ShipLocker`, the Forge visual layer. North Star: board →
cockpit → pick a world → fly out; the ship replaces the travel door and is HOME.

---

## 1. EXTERIOR design language (cool + coherent + "makes sense")
Six rules (Cantwell/ILM + Star Citizen + The Expanse + androidarts craft):
1. **Silhouette-first, always.** Fill it black — if you can't name it from the outline, redesign before
   adding one detail. One dominant shape + 1–2 secondary masses. At Quest draw distance, the silhouette IS the ship.
2. **One dominant feature; subordinate the rest.** Emphasize a hero mass by scale (big engine block,
   fat cargo spine, long sensor mast); everything else defers.
3. **Detail must claim a function.** Greebling = plausibly-purposeful (vents, hatches, pipes) where a
   real machine needs them. Filler greeble that breaks silhouette/palette lowers quality.
4. **One part, several jobs** — a canopy that's also a sensor blister; reads as clever engineering, saves polys.
5. **Manufacturer coherence.** Every part obeys a shared design language so the ship "feels made by one
   builder" (SC: Drake exposed-skeleton, MISC bare-metal; Halo UNSC-blocky vs Covenant-smooth). ZIPTIDE
   needs a shared **salvage-yard grammar** so bolt-ons never look like a junk pile.
6. **Thrust logic (The Expanse).** Ships accelerate along their length; engines point back, mass stacks
   toward the nose. Even stylized kid ships should honor this.

**Form-follows-function checklist (where each part goes):** main engines rearmost on the thrust axis
(bigger role = bigger nozzles) · maneuvering thrusters at nose+tail corners · cockpit forward/high with
a real sightline (sets scale + "which way is front") · boarding door low on the flank near the cockpit
at step-in height · cargo center-spine (mass near thrust center) · radiators/vents on outward faces
("runs hot") · sensor dishes/masts high+forward+unobstructed · landing gear underside, wide/stable ·
non-lethal "hardpoints" = tractor beams / cutters / grabber arms / magnet clamps with clear arcs.

**Archetype set (readable from the outline alone):** Rustbucket Scavenger (starter — stubby, asymmetric,
one oversized patched engine, external grabber arm, mismatched plating) · Hauler (fat cargo spine) ·
Interceptor (tiny fuselage, huge engines) · Explorer/Prospector (forward sensor mast, extra tanks) ·
Tug/Salvager (arms, winches, magnet clamps) · Hardshell (thick layered plating, exposed ribs).

**Upgrades must change the SILHOUETTE, not just reskin:** bigger engine → larger nozzle block + brighter
thrust; more cargo → swollen belly / external pods; plating → layered armor over the base hull; scanner →
a new mast breaks the roofline; special → a hero part (grabber arm) becomes the new dominant feature.

---

## 2. INTERIOR (VR) — "your ship is home"
**Cockpit/helm ergonomics:** a **reach envelope** — throttle lever, stick(s), chunky toggles on the
seat's own console at ~35–50cm so real hands find them without leaning. **Everything touched is a
physical object, not floating UI** (Star Trek: Bridge Crew testers "forgot the controllers were in their
hands"). Kid-friendly: big toggles, a pull-down throttle, a glowing **"PUNCH IT" launch lever**, a
recessed holo-readout instead of a HUD. Seat eye-height ~1.1–1.2m; canopy close enough to lean into.

**Layout blueprint (a legible vessel in the 5×3×12m starter):** one **central corridor down the 12m
axis** — **Aft:** airlock/boarding door → cargo/salvage bay (the working heart) · **Mid:** crew quarters +
a utility nook (fabricator/med) flanking the corridor · **Fore:** cockpit, raised half a step. **Scale
up by inserting corridor segments + side-rooms** (NMS freighter / Subnautica Cyclops). Doors optional so
kids dash aft-to-fore in seconds.

**The "home" toolkit:** a **trophy shelf / magnetic wall-peg grid** where recovered salvage becomes decor
(collecting = decorating) · personalization (paint, nameplate, a bunk with keepsakes) · warm light +
ambient hum · **the window** — look out at the world you're about to fly to. Use **snap-point
decoration** (Cyclops/NMS), never freeform placement.

**VR-native interactions:** docked = flip switches, grab & shelve salvage, and a **holo-map table** in the
corridor where you reach in, pinch a destination star, and it lights the launch lever in the cockpit
(Lone Echo grabbable-surfaces + wrist/console menus, never a floating panel).

---

## 3. THE EXTERIOR↔INTERIOR CORRESPONDENCE (the makes-sense heart, done cheap)
**Don't model a continuous interior — model swappable interior "cells."** Each exterior slot owns a
matching interior room prefab at a known door. Install the Large cargo module → the exterior mesh swaps
*and* the interior "cargo cell" swaps from a crate nook to a walkable hold — through the same slot ID.
Reinforce with **windows** (the engine-room porthole looks at the engine mesh you installed). Quest can't
afford dozens of unique interiors, so: **3 tiers per room type** (Rustbucket / Standard / Refit), a shared
**corridor spine** the cells plug into, **additive scene loading, one cell active at a time.**

---

## 4. THE COHERENCE RULEBOOK (a customized ship still reads as one designed object)
- **Same-chassis grammar (NMS):** only combine parts within one ship class + a unifying palette pass; a
  foreign part gets **reskinned-to-host** (inherits the hull's palette + wear). Tag every module with a
  **styleFamily**.
- **Typed sockets:** an engine hardpoint accepts only engine-shaped things (EVE/SC). Interchangeable
  because the socket is standardized, not freeform.
- **Size-class S/M/L gate:** a Large engine physically won't seat in a Small nacelle — kills the
  "tiny hull, giant engine" incoherence. Upgrading a socket's size is itself a progression step.
- **Silhouette budget:** cap protrusion; enforce rough bilateral symmetry (paired wing/engine slots).
- **Function implies form:** a cargo upgrade must look like more container volume; a scanner must have a dish.
- **Power is the master constraint (FTL):** every active module draws from the reactor, so upgrading is a
  meaningful trade-off, not "install everything."

---

## 5. THE FUNCTIONAL MODULE SET (sensible; lives inside + outside)
| Module | Job | Outside | Inside room |
|---|---|---|---|
| Propulsion | speed/handling | rear nacelles/thrusters | engine room (glowing core) |
| Power/Reactor | the budget that gates all actives | central spine glow | reactor bay w/ gauge |
| Cargo | salvage capacity | side pods/containers | walkable hold w/ racks |
| Sensors/Scanner | find salvage & doors | dish/antenna on top | cockpit console readout |
| Life support/Quarters | comfort, the "home" feel | habitation bulge/windows | bunk/den nook |
| Weapons/Utility | tractor beam, cutter, defense | wing/chin hardpoints | cockpit trigger + wing view |
| Plating | durability/identity | hull skin & armor | (visual only) |

---

## 6. PROGRESSION (salvage-as-currency)
Starter **`rustbucket_scavenger`**: dented, S sockets, tiny hold, weak reactor. The loop (Hardspace in
reverse): **salvage → craft module → grab-and-snap install → visibly better ship, inside AND out.**
Gated by **sockets, not just cash** (to fit an M engine, first refit the nacelle to M). Milestones:
Rustbucket → patched Scavenger → Journeyman specialization (hauler/scout) → capable multi-role vessel.
Every craft consumes salvage, so the world's scrap literally becomes your ship.

---

## 7. NOVEL VR SHIP IDEAS (Quest-feasible, kid-friendly)
1. **Grab-and-snap physical install** — hold the crafted module; a ghost silhouette lights the matching
   socket; push it in → it clunks home with haptics + a latch. Installing is a *physical act*, not a menu.
2. **The holo-model bench** — a palm-sized rotatable hologram of your ship (the whole `ShipRefit` UI made
   tangible): pluck a glowing slot, drop a module in, the mini-model *and* the real ship update live.
3. **Walk-through-your-upgrade** — after install, walk to the room that changed and look out its window at
   the part you just bolted on. The correspondence made *felt*.
4. **Transforming stance (stretch)** — a lever that folds wings / extends the cargo bay between "travel"
   and "work" modes: one hull, two silhouettes.

---

## 8. Architecture mapping (data-driven; additive)
- **`ShipSlotDef`** (the key change — unlocks the whole exterior↔interior model): add `sizeClass` (S/M/L
  gate), `styleFamily` (coherence tag), `interiorCellPrefabId` + `interiorDoorLocalPos` (the paired room).
- **`ShipDefinition`:** add an ordered `InteriorLayout` (room slots: `RoomType {Cockpit, Cargo, Quarters,
  Engine, Utility, Airlock}`, anchor, size), a `corridorSpineId`, per-slot `refitTier`, plus
  `seatEyeHeight`, `windowAnchors`, `trophyMountPoints[]`, and a `designLanguageId`/manufacturer tag +
  per-slot `visualMassBias` (so a bigger module visibly enlarges its hull region). All additive.
- **`ShipHullBuilder`:** extend graybox to honor "one dominant mass" proportion AND place the paired
  interior cell + a porthole aimed at each module's `localPos` (additive, visual).
- **Forge:** owns the manufacturer grammar (bevel/panel/palette), the reskin-to-host pass, and the
  used-universe wear — look stays decoupled from data. Grows the hull mass per occupied slot.
- **`ShipRefit` / `ShipLoadoutCore`:** back the holo-bench; validate `sizeClass`/`styleFamily`/socket +
  reactor power before install.
- **`ShipBoardingStation`:** extend to spawn the interior + recenter the rig at the boarding door, hand
  off to walk mode. New thin **`ShipInteriorRuntime`** owns docked-mode interactables (switches, holo-map,
  trophy grid, decoration snap points).
- **`ShipFlightRuntime`:** the walk↔seated mode toggle + the diegetic launch flow (holo-map pick → lever
  pull → `TravelTo`).
- **⛔ Report-only (per CLAUDE.md — get Terry's confirmation, device-verify, never auto-apply):** anything
  touching `cockpitSeatLocalPos`/boarding door/flight feel, additive interior-scene loading, walkable
  in-hull locomotion, seat recenter, transform stances, and **all VR comfort** (walkable interiors + moving
  ships risk vection/scale-nausea — prototype small, test on-device first).

---

## 9. What to prototype first (recommendation)
1. **`ShipSlotDef` additive fields (`sizeClass`, `styleFamily`, `interiorCellPrefabId`, `interiorDoorLocalPos`)** —
   the one change that unlocks the exterior↔interior correspondence.
2. **The starter interior blueprint** (5×3×12 central-corridor: airlock→cargo→quarters→cockpit) as a
   walkable docked space — report-only/device-verified.
3. **The holo-map table launch flow** (pinch a star → light the lever → `TravelTo`) — the diegetic
   "board → pick a world → fly out" North Star, made tangible.
4. **Grab-and-snap module install + one exterior↔interior pair** (e.g. the cargo slot: small nook ↔
   walkable hold) — proves the whole "makes sense" thesis in one vertical slice.
5. Then the trophy shelf (salvage → decor) — the "home" hook.
