# PLANT / VEHICLE CATALOG-BREADTH AUDIT LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“what’s next”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ STRUCTURAL CI + APK GATE GREEN · HONEST BREADTH DEBT RECORDED · FILE CLAIM RELEASED

## Why this was next

`CURRENT_EXECUTION_CHECKLIST.md` §7 item 6 and `EXCELLENCE_MAP.md` gap #10 called for plant/vehicle catalog breadth. Existing tests proved the plant spec table had 24 balanced entries and vehicle math was comfortable, but did not prove authored assets matched the specs or that the player could actually reach the full catalog.

## Evidence found before implementation

- `GardenAuthor.PlantSpecs()` contains 24 species and already has strong pure-table tests.
- `WorldStubGenerator` begins generated garden plots with only `dew_bulb`, `rust_fern`, and `glass_reed`; existing pack plant ids are intentionally preserved.
- The checked-in starter plant assets predate current tending fields; at least `dew_bulb.asset` has an empty `tendToolIds` list while the current spec includes `watering_can`.
- `VehicleAuthor.VehicleSpecs()` contains three starter rides: tide skiff, dune hoverbike, cavern crawler.
- `WorldPoiBuilder.RideForBiome` surfaces all three starter rides by biome.
- `VehicleArchetype` defines six families; Rover, GravSled and Walker have no authored ride.
- No canonical vehicle garage/catalog-browse surface exists.

The task therefore separates **structural parity**—which can block—from **player-surfacing breadth**, which remains explicit warning debt until content work closes it.

## Delivered scope

- `docs/design/CATALOG_BREADTH.md`
- `Ziptide/Assets/Ziptide/Editor/Audit/CatalogBreadthAuditRules.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/CatalogBreadthAuditRulesTests.cs` + `.meta`
- this log

No plant/vehicle runtime, author table, world pack, generator, POI builder, scene, prefab, visual, Forge, water or Picasso-owned file changed.

## Structural blockers

The audit/build gate now blocks:

- plant or vehicle author tables below their minimum breadth floor;
- empty/duplicate spec ids or invalid core data;
- generated definition assets with empty/duplicate/orphan ids;
- filename/id drift or unusable growth/yield/comfort data;
- missing generated assets during APK preprocessing, after the normal build authors have run;
- starter vehicle ids removed from `WorldPoiBuilder.RideForBiome`.

## Truth-reporting warnings

The audit names, without pretending they are complete:

- create-only plant/vehicle asset fields that drift from the latest author specs;
- plant assets absent from the checked-out branch but created by the normal build author;
- authored plant ids not referenced by any world-pack garden;
- unauthored vehicle archetypes;
- the absent vehicle garage/catalog surface.

These warnings are designed to shrink to zero. The tests explicitly avoid requiring today’s debt to remain, so adding content makes CI greener rather than breaking the gate.

## Verification surfaces

- `CatalogBreadthAuditRulesTests` pins minimum table breadth, unique ids, stable set-difference math, current missing vehicle-family calculation, no structural blockers in ordinary CI mode, actionable findings and build-gate order.
- `CatalogBreadthBuildGate` runs at callback order `835`, after the creature gate, logs `ZIPTIDE: CATALOG_BREADTH_AUDIT`, and throws only on blockers.
- APK preprocessing requires authored assets; ordinary branch CI treats build-generated-but-uncommitted assets as visible warnings.

## Verification proof

- core audit green head: `b706e95f79b538f46a57bb689885b907d3040144`
- core CI run: `29170149603`
- final tests head: `b7a6cfa945759de828b973f993df47ed0071b3ae`
- final CI run: `29170323869`
- Unity EditMode: `success`
- project-contract reports: `success`
- Android: skipped as expected for ordinary branch CI; the pre-build gate runs during APK generation
- overall: `GREEN`
- circuit-breaker reds recorded: `0/3`; one reserved-keyword typo was caught and corrected before a durable red verdict

## Remaining content work

The audit gap is closed; the content breadth is not:

1. Surface the remaining plant species through a deliberate seed-dispenser/almanac/world-pack plan instead of random assignment.
2. Reconcile the committed starter plant assets with current watering/pruning specs through an explicit reseed or migration—not silent audit mutation.
3. Author the missing Rover, GravSled and Walker families only when each has a distinct biome/mechanical role.
4. Build the garage/catalog surface and then promote its warning to a structural requirement.
5. Terry still judges garden and vehicle feel, visibility and usefulness in the consolidated Quest pass.

## Handoff — Did / Next / Heads-up / Commits

**Did:** added one truth-reporting plant/vehicle breadth gate with structural CI/APK blockers and warning-level content debt. It measures author tables, assets, world-pack plant references, biome vehicle placement, missing archetypes and the garage seam without mutating content.

**Next:** recheck `AudioDirector` unload/disposal ownership before changing it. If the previously suspected leak is already covered by explicit cleanup or the travel janitor, close the stale queue row with evidence rather than editing code. Otherwise take one narrow cleanup task and wait for CI.

**Heads-up:** this audit intentionally exposes that 24 authored plant specs are not equivalent to 24 player-reachable seeds. It also records three missing vehicle families and no garage. Those are bounded content rows, not reasons to weaken the gate.

**Commits:** claim `217d569`; design `985c967`; audit `bf6e801` + meta `bb610b2`; compile correction `b706e95`; tests `7f97ee1` + meta `385f122`; progress-safe test correction `b7a6cfa`; final run `29170323869`.

## Closure

The protected-file claim is released. Gap #10 is closed at audit maturity; plant/vehicle breadth remains an explicit content-and-device backlog rather than an invisible assumption.
