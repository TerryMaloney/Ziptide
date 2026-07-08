# HOME HUB — the diegetic ship home screen (design doc)

> **Hardwiring §10 / Phase 2.** The main menu IS the player's ship — you stand in it. Status: skeleton
> for Fable 5. Pairs with `SHIP_FORGE_AND_CUSTOMIZATION.md`, `CONSISTENCY_SPINE.md`, `SHIP_SYSTEM.md`.

## Locked decision
**Diegetic ship hub.** Minimal 2D title only on cold boot; everything else lives in the ship interior.

## Current state (code survey)
Boots straight into a world (`ZiptideConstants.FirstWorldScene`); only an in-VR dev menu. North star
already says "the ship = hub / world-select." `ConquestGalaxy` exists; `GalaxyMap` is named in the
master build plan.

## What to build
- [ ] **Home scene = ship interior** (a walkable interior — first customer of §2 interiors) loaded from
  `_Boot`; the rig spawns at the ship's `SpawnMarker`.
- [ ] **Cold-boot 2D title** — New / Continue → then drop into the ship.
- [ ] **Shell flow** — new game / continue (wire `SaveSystem`), save-slot UI, settings (**comfort
  presets** from the shared comfort layer).
- [ ] **World select at the helm** — a galaxy map (reuse `ConquestGalaxy` + `GalaxyMap`); selecting a
  world → `TravelCoordinator.TravelTo` with the fade transition.
- [ ] **The hub hosts the other shells** — ship **hangar** (§4), vehicle **garage** (§7), garden
  **almanac** (§8), **wardrobe** (cosmetics, §13), story recap, automation dashboard (§9).

## Consistency-spine hooks
Ship interior is a world scene via the world contract; all sub-shells read/write `PlayerProfile`;
comfort presets are the shared ones; the ship shown is the player's customized ship (§4).

## Technique research TODO (cite at build time)
Diegetic VR menu patterns (hub-world main menus); world-select UX in VR; seated-comfortable menu layout.

## Tests + audit
Boot → hub → travel → return round-trips; save-slot load restores hub state; spawn-safe audit.

## 🚀 Room to expand
The ship grows/upgrades as the player progresses (visible power fantasy); crew/companion (RILL) presence
in the hub; a window to the current docked world; the hub docks *at* worlds so home is contextual;
diegetic quest board + mail. Make the hub feel like home, not a menu.
