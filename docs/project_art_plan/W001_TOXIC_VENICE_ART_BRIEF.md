# W001 — Toxic Venice — Art Brief

First concrete art target. Built **primitive-first** (tinted cubes/quads/planes) to prove the
prompt→plan→patcher→audit pipeline, then upgraded to real assets behind the same kit IDs.
Do not import heavy assets until the primitive kit + audit pass on-device.

Last updated: 2026-06-14

---

## Vibe

A flooded, toxic lower city under a wealthy upper tier. The player walks rusted mid-level
walkways above green toxic canals, with an oligarch glass tower on one side and an older stone
garden tower on the other. Feels large via distant silhouettes + fog, not real geometry.

## Required elements (W001 v1, primitive kit)

| Element | Surface family | Role | Notes |
|---|---|---|---|
| Toxic lower canals | Toxic Earth Industrial | decorative | green underglow (emissive), below walkway |
| Rusted mid-level walkway | Salvage / Field Repair | **walkable** | the safe player route — must stay clear |
| Red glass oligarch building (right) | Upper-Class Clean Glass | blocker | fake glass, oligarch glyph decals |
| Short white stone tower (left) | Stone / Ceremonial Alien | blocker | garden tower, glyph relief |
| Distant city silhouettes | Cosmic/backdrop | decorative | sky-dome / backdrop mesh, no real geo |
| Alien/oligarch glyph decals | DecalSet `OligarchGlyphs` | decorative | on glass + stone |
| Fog | sky/atmosphere | — | sells distance; cheap depth |

## Constraints

- Obvious, **unblocked** walkable route from `__SPAWN_PLAYER` to the travel door(s).
- Within the Quest budget (`QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md`): ≤ 25 materials, 1
  directional light, ≤ 4 transparent materials (use fake glass), backdrops not real geometry.
- All elements resolve via the `ToxicVenice` art kit (no one-off materials).

## Audio (declare only; AudioDirector lives in _Boot)

- Ambience: low industrial hum + dripping water loop (`AmbienceLayerDefinition`).
- Travel stinger on enter/exit. Hazard audio reserved for Bloom tendrils.

## Build order

1. `ToxicVenice` `WorldArtKitDefinition` + surface sets (primitive materials).
2. ArtBuildPlan for the layout above → patcher places primitives → art audit passes.
3. On-device check: route walkable, 72 FPS, no runtime errors.
4. Swap primitives → real meshes/materials behind the same IDs (LODs, texture budget).
