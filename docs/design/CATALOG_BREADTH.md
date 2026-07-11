# PLANT & VEHICLE CATALOG-BREADTH CONTRACT

**Status:** v1 structural gate + honest breadth report, 2026-07-11  
**Scope:** GardenAuthor plant specs, VehicleAuthor fleet specs, generated definition assets, world-pack garden references, biome-to-ride mapping and the future garage seam.

## Why

A large catalog can exist in code while the player repeatedly sees the same three entries. Catalog quality therefore has two separate dimensions:

1. **structural parity** — the author table, generated assets and runtime ids agree;
2. **player surfacing** — the catalog is actually reachable through worlds, seed selection, vehicles or a garage.

The first can block immediately. The second must report the current debt honestly rather than pretending the content is already surfaced.

## Plant laws

- `GardenAuthor.PlantSpecs()` remains the authoring source of truth and contains at least 20 unique species.
- Every spec id must resolve to exactly one `PlantDefinition` after build authors run.
- Every generated plant definition must map back to one spec.
- Filename, `Definition.id`, growth time and yield entries must remain usable; create-only live-asset differences are reported, not silently overwritten.
- A plant counts as **surfaced v1** when a committed/generated `WorldPackDefinition.gardens[].plantId` references it.
- The audit lists every unsurfaced id. A future seed-dispenser/almanac path may become a second accepted surfacing seam only through an explicit contract update.

Current known truth: 24 specs exist, while generated world-pack logic begins from the original three-seed ladder and preserves hand-tuned existing ids. The audit is expected to expose this breadth debt until content work closes it.

## Vehicle laws

- `VehicleAuthor.VehicleSpecs()` remains the starter-fleet source of truth and contains at least three unique rides and three distinct archetypes.
- Every spec id must resolve to exactly one `VehicleDefinition` after build authors run.
- Every generated vehicle definition must map back to one spec.
- Filename/id, archetype and positive comfort-clamped tuning fields must remain valid.
- Every starter ride id must remain present in `WorldPoiBuilder.RideForBiome`; a vehicle in the table but absent from world placement is a blocker.
- Unrepresented `VehicleArchetype` values are named as breadth debt.
- The canonical future garage seam is `Ziptide/Ship/Runtime/VehicleGarageRuntime.cs`; until it exists, the audit reports `VEHICLE_GARAGE_SURFACE_MISSING`.

Current known truth: tide skiff, dune hoverbike and cavern crawler are surfaced by biome; Rover, GravSled and Walker remain unauthored and the garage is unbuilt.

## Severity

### Blocker

- duplicate or empty author-table ids;
- invalid spec values;
- required generated asset missing during APK preprocessing;
- generated asset id/orphan/type drift;
- starter vehicle id absent from biome placement source.

### Warning

- generated create-only asset fields differ from the latest author spec but remain valid;
- plant ids not surfaced by any world pack;
- vehicle archetypes not represented by the fleet;
- garage surface absent.

Warnings are explicit backlog evidence. They do not fail ordinary CI or APK generation until the corresponding content is built and the gate is deliberately promoted.

## Verification surfaces

- `CatalogBreadthAuditRulesTests` validates the pure set/span logic, current author tables, vehicle source wiring and a non-mutating project audit.
- `CatalogBreadthBuildGate` runs before APK generation with authored assets required; it logs `ZIPTIDE: CATALOG_BREADTH_AUDIT` and throws only on blockers.
- The audit never authors, modifies or deletes assets, packs, scenes or runtime content.
