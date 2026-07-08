# DRIVABLE VEHICLES — a per-planet fleet (design doc)

> **Hardwiring §7 / Phase 3.** The ground sibling of the ship: many vehicles per world, varied speeds
> and traversal, complex mechanics like the creatures. Status: skeleton for Fable 5. Shares the ship's
> Forge, cosmetic, comfort, and seat/mount systems (`CONSISTENCY_SPINE.md`, `SHIP_FORGE_AND_CUSTOMIZATION.md`).

## Current state (code survey)
None exist beyond the ship. Nothing driveable/rideable/mountable.

## What to build
- [ ] **`VehicleDefinition` + `VehicleModuleDefinition`** — data-driven chassis + parts, Forge-built
  and baked like ships (share the module/Forge system where possible).
- [ ] **`VehicleController`** — per-locomotion drive feel via the **shared comfort layer** (cockpit
  frame, vignette; never parent rig to a moving hull). Complex mechanics per type: suspension, hover,
  buoyancy, traction, drift.
- [ ] **Locomotion archetypes / per-biome catalog (≥8–12):** rover, hoverbike, **boat/skiff** (tide
  worlds), mech/walker, glider, **drill-crawler** (caverns — ties to §3), tram/zip-car (ties to
  ziplines), grav-sled. Varied speed + method (some climb, hover, submerge, tunnel).
- [ ] **Seat / mount / enter-exit** — shared with the ship boarding station.
- [ ] **Vehicle wraps** (`VehicleWrapDefinition`) via the one cosmetic layer.
- [ ] **Garage / spawn** in the home hub; vehicles persist in `PlayerProfile`.
- [ ] Traversal integration with §3 verticality (climbers/flyers reach elevated POIs).

## Consistency-spine hooks
Same Forge, cosmetic layer, comfort/input, seat/mount, and save spine as the ship — a vehicle is "a
ship for the ground." New vehicle = a new Definition.

## Technique research TODO (cite at build time)
VR ground-vehicle comfort; arcade drive/hover/boat physics that feel gamepad-smooth; mount/dismount UX.

## Tests + audit
Controller physics deterministic in fixed-step tests; comfort layer shared-path test; wrap stats-invariant;
PerfBudget cap per vehicle prefab.

## 🚀 Room to expand
Vehicle combat + weapon mounts (reuse ship weapons); races/time-trials as world events; vehicle-only
traversal gating (a world you can only cross by boat); creature-mounts (ride a tamed creature — bridges
to `CREATURE_ECOLOGY.md`); convoy/towing. Make each planet's signature vehicle a reason to visit.
