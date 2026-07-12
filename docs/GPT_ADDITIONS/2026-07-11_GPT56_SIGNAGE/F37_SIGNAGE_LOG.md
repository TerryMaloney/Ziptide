# F3.7 — SIGNAGE & WAYFINDING LOG

**Owner:** GPT-5.6 Thinking, temporarily authorized Picasso/Art lane  
**Authorized by:** Terry, 2026-07-11 (“take over Picasso’s lane… knock out whatever you can”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 FULL ENVELOPE PUSHED — FINAL CIRCUIT-BREAKER VERIFICATION RUN REQUESTED  
**Parent plan:** `docs/project_art_plan/FORGE_III_PLAN.md` §F3.7

## Goal

Make generated worlds read as a civilization through procedural Shell-script signs and a consistent wayfinding color law—without real fonts, localization debt, interactive UI ownership or excessive lights/materials.

## Delivered envelope

### Commit 1 — vocabulary and Forge bodies

- pure deterministic `ShellGlyphBaker` with no font/TextMesh/TMP or runtime resource creation;
- `SignDestinationClass { Travel, Job, Vendor }`;
- three Forge body recipes: hanging sign, wall plate, route chevron;
- each validates/builds within the 400-triangle sign budget;
- canonical hue constants remain Core-safe hex strings:
  - travel `35D9E6`;
  - job `E6A13A`;
  - vendor/resource `55C879`;
- create-only sign recipe producer is hooked exactly once into `BuildAndroid`;
- vocabulary tests pin determinism, density, hue, body budgets and absence of real-text/UI APIs.

Commit-1 final proof:

- tested SHA `195333133759108ed18791c107767e4d835072bb`;
- green run `29182263910`.

### Commit 2 — shared authored surfaces and deterministic placement

- `SignRecipeLibrary` authors three shared opaque 64×64 Shell-glyph textures and three shared unlit materials under `Resources/Signage`;
- no runtime texture/material allocator or cleanup owner is added;
- `SignAuthor` places at most six signs at deterministic POI approaches;
- travel berth → teal hanging sign;
- jobs (`CombatCamp`, `HarvestGrove`, `MachineSite`, `StoryAnchor`) → amber wall plate;
- resource/vendor (`RuinCache`, `CaveSecret`) → green route chevron;
- signs consist of a Forge body/fallback plus a shared-material glyph quad;
- no collider, point light, shadow, TextMesh/TMP, XRI, input or gameplay state;
- `WorldDressingBuilder` receives exactly one additive `SignAuthor.Place(...)` call after ambient motes;
- placement tests pin POI mapping, stable seeds, opaque/distinct shared panels, six-sign cap, idempotence, shared materials, no interactive owner and exact wiring.

## CI history / circuit breaker

- red 1/3 — run `29181714155`: existing safety gates rejected runtime texture creation and an unhooked asset producer;
  - corrected design: baker made pure; producer build-hooked; no gate weakened.
- red 2/3 — run `29182081520`: Job glyph coverage was 32.3% against the unchanged 30% noise ceiling;
  - corrected asset: stroke width reduced from 2.5% to 2.0%; threshold remained locked.
- commit 1 then green — run `29182263910`.
- commit 2 was staged on `gpt56-staging/f37-signage` and fast-forwarded atomically to live head `c9a1c6c1ceaecbb45a7e934081f72f2ed7bfc809`.
- that staged head ended in a `[skip ci]` metadata commit, so this documentation-only commit intentionally triggers the single decisive final Unity run without changing signage code.

If this final run is red, F3.7 stops immediately at the 3/3 circuit breaker. No further corrective edit is authorized in this session.

## Locked wayfinding law

- Travel berth = teal.
- Jobs = amber.
- Vendor/resource = green until a dedicated vendor POI exists.
- Glyphs are abstract Shell script, never English or a real font.
- ≤6 signs per world.
- no point lights, shadows, colliders, XRI interactables or gameplay state.
- one shared glyph material/texture per destination class; no material instance per sign.

## Device evidence after green

- regenerate representative worlds so `ShellSignage` is present;
- verify signs read as alien civic language rather than English/noise;
- confirm teal/amber/green distinctions are recognizable but not neon UI clutter;
- inspect one travel berth, one job POI and one resource POI;
- verify Forge bodies replace fallbacks while retaining the glyph quad;
- confirm signs do not block movement, cast expensive shadows or add real lights;
- judge readability from the route approach—not only from point-blank range;
- verify no meaningful 72 Hz regression.

## Collision / stop rules

- Do not touch UI, localization, jobs, POI gameplay, travel, input, scenes/prefabs or Gameplay runtime.
- Do not turn signs into interactive menus.
- Do not modify Forge global budgets or existing recipes.
- Three CI reds triggers the circuit breaker.
