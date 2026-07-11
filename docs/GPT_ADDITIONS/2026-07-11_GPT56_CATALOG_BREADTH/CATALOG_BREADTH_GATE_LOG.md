# PLANT / VEHICLE CATALOG-BREADTH AUDIT LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“what’s next”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — truth-reporting breadth audit in progress

## Why this is next

`CURRENT_EXECUTION_CHECKLIST.md` §7 item 6 and `EXCELLENCE_MAP.md` gap #10 call for plant/vehicle catalog breadth. Existing tests prove the plant spec table has 24 balanced entries and vehicle math is comfortable, but they do not prove authored assets match the specs or that the player can actually reach the full catalog.

## Evidence found before implementation

- `GardenAuthor.PlantSpecs()` contains 24 species and already has strong pure-table tests.
- `WorldStubGenerator` originally assigned only `dew_bulb`, `rust_fern`, and `glass_reed` to generated garden plots; existing pack plant ids are intentionally preserved.
- `VehicleAuthor.VehicleSpecs()` contains three starter rides: tide skiff, dune hoverbike, cavern crawler.
- `WorldPoiBuilder.RideForBiome` surfaces all three starter rides by biome.
- `VehicleArchetype` defines six families; only three are authored and no garage surface exists yet.

Therefore this task must not falsely mark content breadth complete. It will separate **structural parity** (which can block) from **player-surfacing breadth** (initially report/WARN until the content is actually expanded).

## Exact scope

New files only:

- `docs/design/CATALOG_BREADTH.md`
- `Ziptide/Assets/Ziptide/Editor/Audit/CatalogBreadthAuditRules.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/CatalogBreadthAuditRulesTests.cs` + `.meta`
- this log

Closure-only docs after green:

- `docs/CURRENT_EXECUTION_CHECKLIST.md`
- `docs/EXCELLENCE_MAP.md`

## Contract

### Blockers

- every plant spec has one committed `PlantDefinition` asset with matching id/core fields;
- every committed plant asset maps back to one spec;
- every vehicle spec has one committed `VehicleDefinition` asset with matching id/archetype/core fields;
- every committed vehicle asset maps back to one spec;
- every starter vehicle id remains present in the biome-to-ride source mapping.

### Warnings / breadth debt

- authored plant ids not referenced by any committed `WorldPackDefinition.gardens` entry;
- fewer than the full plant catalog is player-surfaced;
- vehicle archetypes with no authored ride;
- missing garage/catalog-browse surface.

The warning report names exact missing ids/archetypes so the next content task is bounded and measurable.

## Collision rules

- Do not edit plant/vehicle runtime, author tables, world packs, WorldStubGenerator, WorldPoiBuilder, scenes or prefabs in this audit task.
- Do not touch Picasso `Visuals/**`, Forge, vehicle/plant art, water, art audits, or `SPRINT_ART.md`.
- No automatic asset/content mutation; audit only.
- Three CI reds triggers the circuit breaker.
