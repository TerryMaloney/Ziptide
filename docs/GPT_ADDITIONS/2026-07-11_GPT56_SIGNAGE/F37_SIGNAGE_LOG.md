# F3.7 — SIGNAGE & WAYFINDING LOG

**Owner:** GPT-5.6 Thinking, temporarily authorized Picasso/Art lane  
**Authorized by:** Terry, 2026-07-11 (“take over Picasso’s lane… knock out whatever you can”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — commit 1 vocabulary/body work in progress  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.7

## Goal

Make generated worlds read as a civilization through procedural Shell-script signs and a consistent wayfinding color law—without real fonts, localization debt, interactive UI ownership or excessive lights/materials.

## Two-commit envelope

### Commit 1 — vocabulary and Forge bodies

New Visuals/runtime data:

- `Ziptide/Assets/Ziptide/Visuals/Runtime/Signage/ShellGlyphBaker.cs` + `.meta`
  - pure deterministic angular glyph-strip alpha bake;
  - closed `SignDestinationClass { Travel, Job, Vendor }`;
  - no font/TextMesh/TMP dependency;
  - texture creation helper owned by the future sign surface.

New art authoring:

- `Ziptide/Assets/Ziptide/Editor/Patching/SignRecipeLibrary.cs` + `.meta`
  - three Forge bodies: hanging sign, wall plate, route chevron;
  - ≤400 triangles each; `prop` + `sign` tags; Toxic Industrial family;
  - create-only assets under `Resources/Forge`.

Shared additive constants:

- `Ziptide/Assets/Ziptide/Core/Runtime/ZiptideConstants.cs`
  - travel teal, job amber, vendor green as hex strings only;
  - no UnityEngine dependency added to Core.

Tests:

- deterministic/nonempty/non-noise glyph masks;
- different seeds/destination classes differ;
- exact hue constants;
- all three sign recipes validate and build within budget;
- no real text/font API in the signage source.

### Commit 2 — shared surface and deterministic placement

Planned after commit 1 is green:

- `ShellGlyphPanel` runtime: one shared emissive material per destination class, procedural texture, no Light/collider/input;
- `SignAuthor`: ≤6 signs/world at POI approaches and route decisions;
- existing `WorldDressingBuilder` receives one additive author call;
- body uses `ForgeModuleLook` recipe id; glyph surface is a separate front quad;
- travel/job/vendor classification from existing `PoiType` only;
- placement/structure/wiring tests and Terry runbook look check.

## Locked wayfinding law

- Travel berth = teal.
- Jobs (`CombatCamp`, `HarvestGrove`, `MachineSite`, `StoryAnchor`) = amber.
- Vendor/resource (`RuinCache`, `CaveSecret`) = green until a dedicated vendor POI exists.
- Glyphs are abstract Shell script, never English or a real font.
- ≤6 signs per world.
- no point lights, shadows, colliders, XRI interactables or gameplay state.
- one body recipe + one shared glyph material/texture family; no material instance per sign.

## Collision / stop rules

- Do not touch UI, localization, jobs, POI gameplay, travel, input, scenes/prefabs or Gameplay runtime.
- Do not turn signs into interactive menus.
- Do not modify Forge global budgets or existing recipes.
- Commit 2 waits for a durable green commit-1 verdict.
- Three CI reds triggers the circuit breaker.
