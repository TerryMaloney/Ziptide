# F3.5 COMMIT 3A — AMBIENT WORLD MOTES LOG

**Owner:** GPT-5.6 Thinking, temporarily authorized Picasso/Art lane  
**Authorized by:** Terry, 2026-07-11 (“take over Picasso’s lane… knock out whatever you can”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — runtime marker authored; Unity verification requested before world authoring  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.5

## Dependency proof

- `VfxRecipeDefinition` / `VfxLibrary` data rails green.
- pooled `VfxFactory` green at `538dba59574c57ca13786b5b928de116d1040d08`, run `29180768058`, 969/969 tests.
- weapon/combat impact seam remains unassigned and is excluded.

## Exact scope

New Visuals runtime:

- `Ziptide/Assets/Ziptide/Visuals/Runtime/Vfx/AmbientMoteVolume.cs` + `.meta`

New Art editor author:

- `Ziptide/Assets/Ziptide/Editor/Patching/AmbientMoteAuthor.cs` + `.meta`

One additive art-owned hook:

- `Ziptide/Assets/Ziptide/Editor/Patching/WorldDressingBuilder.cs`
  - one `AmbientMoteAuthor.Place(dressRoot, kit, route)` call after current dressing/water authors;
  - no route/scatter/water/practical behavior removed.

Tests:

- `Ziptide/Assets/Ziptide/Tests/EditMode/AmbientMoteTests.cs` + `.meta`

Closure docs:

- this log;
- `docs/SPRINT_ART.md` F3.5 row.

## Placement contract

- at most three looping mote volumes per world, preserving at least three of the six live VFX slots for interactions;
- fixed world anchors, never a player-following or rig-owned volume;
- route anchors selected deterministically: start, middle and far end with duplicate indices removed;
- each marker sits approximately chest-high above terrain and has no renderer, light, collider, physics or gameplay state;
- `TideFlats` and `CavernFloor` use `motes_spore`;
- `Dunes`, `Mesas`, `Canyon` and fallback use `motes_amber`;
- runtime volume delegates entirely to the existing `VfxFactory`, stops its looping system on disable/destroy and performs no edit-mode spawning;
- author is idempotent by replacing only its own `AmbientMotes` child under the rebuilt `Dressing` root.

## Runtime checkpoint

- runtime marker commit: `b7033208e15ecb4d88e5fce28568c947efb56d80`;
- metadata child: `0a1d37327d5235f7778a3c19510837ce4255a5fd`;
- this documentation-only commit intentionally triggers Unity CI because the immediate metadata child used `[skip ci]`;
- no editor author or shared-file hook will land until this checkpoint is green.

## Acceptance

- pure route-index and biome-mapping tests;
- ≤3 markers, deterministic placement and no collider/light/renderer;
- both mote recipe ids have a concrete world-scatter row;
- runtime volume rejects one-shot recipes, starts a looping mote through the factory and returns it on stop;
- source wiring pins exactly one author call in `WorldDressingBuilder`;
- Terry later judges visibility, density, square-card artifacts and 72 Hz in representative amber/spore worlds.

## Collision / stop rules

- Do not touch `CreatureRuntime.ReceiveHit`, weapons, damage, ecology, Gameplay, combat, scenes or prefabs.
- Do not change VFX recipe data/caps or the green factory unless a concrete defect is discovered.
- Do not begin F3.6 or F3.7 before this half is green and closed.
- Three CI reds triggers the circuit breaker.
