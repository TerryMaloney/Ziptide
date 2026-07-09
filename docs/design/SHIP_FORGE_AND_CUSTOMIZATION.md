# SHIP FORGE & CUSTOMIZATION — next-gen ships (design doc)

> **Hardwiring §4 / Phase 2.** Kills the "N64 Star Fox" look: a data-driven, textured, baked procedural
> ship pipeline like the creature Forge, plus Fortnite-style customization. Status: skeleton for Fable 5.
> Pairs with `SHIP_SYSTEM.md`, `SPACEFLIGHT_PHYSICS.md`, `CONSISTENCY_SPINE.md`, `ART_REGISTRY.md`.

## Current state (code survey)
`ShipHullBuilder` assembles a hull from **primitive boxes** (the N64 look). `ShipDefinition` + `SHIPS.md`
are a data seed only. No customization / skin / wrap system.

## What to build
- [ ] **Ship Forge** — a `ForgeRecipeDefinition` family for ship modules: hull core, wings, engines,
  cockpit, weapon hardpoints, fins, greebles — UV'd, textured (normal/MSA/emissive), baked to ASTC
  prefabs, exactly like the creature/weapon Forge.
- [ ] **`ShipChassisDefinition` + `ShipModuleDefinition`** — assemble a ship from a chassis + module
  slots (like Forge creatures from limb recipes). Ship **≥6 chassis** across silhouettes
  (interceptor, hauler, gunship, explorer, salvager, racer).
- [ ] **Loadout layer (functional):** swappable wings/engines/hardpoints that DO change stats
  (speed, handling, weapon slots, cargo) — the No Man's Sky / Star Citizen module model. Feeds `FlightModel`.
- [ ] **Cosmetic layer (wraps):** `ShipWrapDefinition` via the shared cosmetic layer
  (`CONSISTENCY_SPINE.md` §A) — liveries, decals, emissive palettes, material skins; **never stats**.
- [ ] **In-VR hangar** — grab-and-attach parts, apply wraps, preview; lives in the home hub. Persists
  in `PlayerProfile`.
- [ ] Hardpoints expose mount points for **space combat** (`SPACE_COMBAT.md`).

## Consistency-spine hooks
Forge + `ArtModuleRegistry` (primitive fallback if a module is unfulfilled); shared cosmetic layer;
stats live on Definitions; hangar UI is a home-hub surface.

## Technique research TODO (cite at build time)
Modular ship-building systems (NMS/Star Citizen part slots), cosmetic-wrap architecture (Fortnite —
data-driven skin layer over a base mesh), greeble/detailing for readable silhouettes at VR scale.

## Tests + audit
Chassis+module assembly deterministic; wrap apply leaves stats invariant (cosmetic golden test);
PerfBudget cap for ship prefabs; hardpoint contract test.

## Additions Bank
`SPACEFLIGHT_50.md` ship rows.

## 🚀 Room to expand
Ship interiors you can walk (ties to §2 interiors — the hub ship is the first); nameable/save-slot
ships; a shipyard progression; salvaged parts from space combat feeding the loadout; signature "hero"
chassis tied to story; a paint-booth that uses the Forge texture pipeline live. Make ship-building a loop.

## SHIP-MORE — the go-back-for-more list (Terry's ask, 2026-07-09; post-sprint additions queue)
The sprint shipped chassis/loadout/refit/hangar/decals/hum. These are the NEXT swings, in rough
value order — each one small enough to claim individually:
1. **Loadout → FlightModel feed** — resolved ShipStats drive cruise/boost/turn in Reasonbox's flight
   (the ShipDefinition seam; coordinate, then a racer FLIES like a racer). The single highest-value next step.
2. **Walk-around refit** — grab a wing/engine module OFF a rack and SNAP it onto the hull socket by
   hand (BuildSocket idiom) instead of tiles: the spec's original "grab-and-attach" fantasy.
3. **Interior grows with the chassis** — hauler gets a cargo hold room, gunship a gun deck (RoomPartitioner
   is live from 1.3 — the ship is the first customer of walkable interiors).
4. **Scorch + wear** — hull weathering accrues from FLIGHT (rings flown, boosts used), cleanable at
   the hangar wash rack; pairs with journey decals (story marks + usage marks = a lived-in ship).
5. **The ship remembers where it's been** — tiny world-emblem stickers per visited world (the manifest
   knows), a second decal rail on the starboard flank.
6. **Landing gear + berth animation** — the ship settles/vents on arrival (world-moves comfort law).
7. **Ship voice = hum + RILL** — RILL's orb docks at a cockpit perch; her lines duck the hum; the hum
   pitch-shifts with boost when the FlightModel feed lands.
8. **Photo-booth pad in the hangar** — the creature photo-loop pattern pointed at YOUR ship (share shots).
9. **Fleet slots** — own multiple loadouts (profile strings already support it: SHIP_EQUIP:slot2:*),
   swap at the hangar; the Quarters gets a fleet plaque.
10. **Hero chassis gated by story** — a seventh silhouette unlocked by C8_W062_REVELATION (the
    Architect skiff — story-shaped, not stat-better; the no-pay-to-win law extends to no-story-to-win).
11. **Ship audit rule** — seat present, hull budget, nameplate legible, no gameplay scripts on
    cosmetics (SHIP_SYSTEM.md §audit — none of it is enforced yet).
12. **Wrap EDITOR** — pick body/accent from a palette wheel in the hangar instead of preset liveries
    only (writes a runtime CosmeticDefinition-shaped entry; the paint booth the spec dreamed of).
