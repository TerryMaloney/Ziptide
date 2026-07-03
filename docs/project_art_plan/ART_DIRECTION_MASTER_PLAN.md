# Art Direction Master Plan

The visual identity of ZIPTIDE. Used to author kits and validate prompt-driven art.

Last updated: 2026-06-14

---

## Visual pillars

1. **Cyberpunk working-class space** — lived-in, repaired, utilitarian; not glossy sci-fi.
2. **Cosmic scale without rendering giant worlds** — vistas/fog/backdrops imply vastness.
3. **Alien symbolic surfaces** — glyphs/patterns carry meaning, not just decoration.
4. **Tool-built / field-repaired infrastructure** — Cal is a technician; the world shows it.
5. **Small world, large universe** — dense playable pockets inside an implied huge network.

## Surface families (each becomes a `SurfaceSetDefinition`)

| Family | Look | Use |
|---|---|---|
| Toxic Earth Industrial | rust, grime, green underglow | lower canals, W001 |
| Upper-Class Clean Glass | red/tinted fake glass, sharp trims | oligarch buildings |
| Salvage / Field Repair | patched metal, tape, mismatched parts | walkways, props |
| Alien Origami / Pattern | folded geometry, glyph decals | Pattern worlds |
| Cosmic / Deep Space | starfields, nebula backdrops | sky/void worlds |
| Waterworld / Biofilter | wet, algal, translucent | coastal/underwater |
| Stone / Ceremonial Alien | pale carved stone, glyph relief | towers, shrines |
| Black Hole Fringe | extreme contrast, lensing tricks | endgame edge worlds |

## First target: W001 Toxic Venice

See `W001_TOXIC_VENICE_ART_BRIEF.md`. Built first as a **primitive kit** (tinted cubes/quads)
to prove the pipeline, then upgraded to real assets behind the same IDs.

## Authoring rule

Every visual element belongs to a surface family + kit and resolves by ID (see
`ART_AUDIO_CONTENT_ARCHITECTURE.md`). No bespoke one-off materials in scenes.

## Forbidden aesthetics (photo-critique FAIL conditions — reconciliation 2026-07-04)

**All families:** N64-flat untextured geometry · functionless spikes/greebles · direct franchise
lookalikes · generic bug/zombie/dinosaur creatures (evolution reason + story tie required).
**Per family:** Toxic Earth Industrial / Salvage — no clean chrome, no showroom finishes; wear
tells the maintenance story. Upper-Class Clean Glass — no grime (the class divide IS the contrast).
Alien Origami / Pattern — no rivets, pipes, or repair tape; zero grime; only the 5-material
teal/gold/amber/black/white language. Cosmic — no opaque "space walls"; vastness via backdrops.
Stone/Ceremonial — no medieval-fantasy tropes; glyphs are geometric, never alphabetic.
Full list + enforcement: `PROXY_CONTRACTS.md` (rubric) + `ForgePalettes` (tested color law).
