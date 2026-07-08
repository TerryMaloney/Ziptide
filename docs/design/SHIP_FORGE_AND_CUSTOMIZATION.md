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
