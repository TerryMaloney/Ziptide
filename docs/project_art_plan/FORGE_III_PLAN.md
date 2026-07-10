# FORGE III — THE COHESION LEAP (written for Opus 4.8 / Sonnet 5 operators)

**Author: Picasso (Fable 5), 2026-07-10 — one of the last Fable sessions. This document is the
handoff.** FORGE I gave us assets from prompts. FORGE II gave those assets AAA surfaces (atlas
textures, styles, budgets) and extended them to creatures, buildings, and flora. **FORGE III is the
layer nobody sees as an asset: light, color, water, grounding, motion, and the gates that keep
every lane EVEN.** It is deliberately written so a smaller model cannot screw it up: every envelope
is decision-free, every knob has a stated range, every result has a photo or test checkpoint.

> **Terry's directive (2026-07-10):** "How can we consistently make everything across the board
> even better, and what did we not think to add that's going to be important art-wise or
> mechanics-wise? What goes into a really excellent game looks-wise that we haven't covered?"
> This plan is the answer, ordered by visual leverage per engineering hour.

---

## §0 — READ FIRST, EVERY SESSION (Opus/Sonnet rails)

1. **The loop is the law.** Change → push to `terry-local-wip` → CI green → `forge-photos` (for
   assets) or the named EditMode tests (for systems) → LOOK at the result → iterate. Never mark an
   envelope done without its checkpoint. Never stack a second change on an unverified first one.
2. **Circuit breaker:** 3 CI-reds on one task → stop, write a HANDOFF entry describing the wall,
   move to the next envelope. Do not thrash.
3. **One envelope per work session.** They are ordered; do them in order unless Terry redirects.
4. **You do not make design decisions.** Every decision is already in this file. If you hit a real
   undecided fork, that's a HANDOFF question for Terry, not a coin flip.
5. House rules still bind: no scene/prefab YAML edits, .meta per new file, boards updated in the
   SAME commit, `ZIPTIDE:` log tags, `Debug.Log("ZIPTIDE: TAG key=value")`, small commits.
6. **The derivation law (new, FORGE III's core idea):** atmosphere values (fog, ambient, grade)
   are DERIVED from the world's SkyVista + theme by formula, never authored twice. A smaller model
   can't make the fog disagree with the sky if the fog is computed FROM the sky.

Key existing seams you will touch (read before editing):
`Visuals/Runtime/SkyVistas/SkyVistaRig.cs` · `Visuals/Runtime/Forge/ForgeTexture.cs` (styles) ·
`Editor/Patching/ThemeAuthor.cs` + `VisualThemeProfile` · `Editor/Audit/WorldAuditRunner.cs` ·
`Editor/Patching/WorldDressingBuilder.cs` · `docs/project_art_plan/FORGE_STUDIO_GUIDE.md`.

---

## F3.1 — THE LIGHT SCRIPT (highest leverage in the whole plan)

**Why:** AAA look is mostly lighting. We author movie-grade SKIES, then light the scene with
whatever default the patcher happened to set. The mismatch is the single biggest remaining gap.

**What:** a `LightScript` block on `VisualThemeProfile` (new serializable class, all fields with
defaults): key-light azimuth/elevation/color/intensity, fill ambient (sky/equator/ground trilight),
fog color/density/height falloff.
**The derivation:** an editor method `LightScriptDeriver.Derive(SkyVistaDefinition v)` computes
defaults — key color = the vista's dominant celestial body tint (or its light color field), key
azimuth points AT that body, fog color = the vista's horizon gradient color, ambient sky = zenith
gradient color, ambient ground = fog color × 0.6. A per-world author may then override ONLY
intensity ±30% and elevation within 20–55°. `SkyVistaAuthor.AssignAll` calls the deriver.
**Runtime:** `SkyVistaRig` (already per-scene) applies the LightScript to the scene's directional
light + `RenderSettings` (ambient trilight mode, fog).

**Acceptance:** EditMode tests — derivation is deterministic; fog color equals the vista horizon
color; every world theme carries a non-default LightScript after AssignAll. Audit rule
`LIGHT_SCRIPT_MISSING` (blocker) / `LIGHT_SCRIPT_UNDERIVED` (warn) in WorldAuditRunner.
**Photo checkpoint:** none in booth (scene-level) — Terry runbook item: W002 + W005 before/after.
**Budget:** 2 commits. **Do not:** add real-time shadows beyond the one directional (Quest),
touch URP asset settings, or invent per-object lights (PerfBudget gate counts lights).

## F3.2 — THE GRADE (per-world color grading)

**Why:** filmic unification. One tonemapper + a per-world color filter is what makes screenshots
look like the SAME finished game instead of assorted assets.

**What:** one URP global `Volume` profile per world, generated (create-only) by a new
`GradeAuthor` from theme data: ACES tonemapping ON everywhere; per-world `ColorAdjustments`
(post-exposure −0.3..+0.3, saturation −10..+15, color filter = a derived tint at strength ≤0.08)
and `WhiteBalance` (temp −15..+15). Derivation: filter tint = vista horizon color desaturated 50%.
The `_Boot` scene gets NO volume (menus stay neutral). Wire: the world scene's root gets the
Volume component via the existing scene patcher hook (`ThemeAuthor` pattern — runtime ensure, not
YAML).
**Acceptance:** tests — every world theme has a grade asset; ACES on; values inside the stated
ranges (a conformance test literally asserts the ranges so an operator CANNOT push a wild grade).
Audit rule `GRADE_MISSING` warn → promote to blocker once all worlds pass.
**Budget:** 2 commits. **Do not:** enable bloom/DoF/motion blur/vignette post (Quest cost; the
comfort vignette is a separate gameplay system), or per-scene volume blending.

## F3.3 — WATER (the game is called ZIPTIDE — and we have no water)

**Why:** the namesake element barely renders. W001 is toxic VENICE; the tidefront is a core set
piece. Cheap, good VR water is a solved problem and it is nowhere in the codebase.

**What:** `ZiptideWater` (Visuals/Runtime): a subdivided plane mesh (64×64 max) + ONE material on
URP/Lit-compatible shader graph-free path: two scrolling normal-map layers (bake a tileable water
normal in `ForgeTexture` — reuse the fBm; two samples, directions 25° apart, speeds 0.02/0.031),
depth tint via camera-facing fresnel approximation (vertex trick, no depth texture on Quest),
specular smoothness 0.85, and **edge foam cards** (LeafCard-style alpha strips along authored
shore edges, alpha from a foam fBm, slow scroll). Vertex bob: ±0.03m two-sine (Sway math family).
An editor `WaterAuthor` places water planes from `CityLayoutDefinition` canal/shore data where the
layout declares them (new optional `waterRects` field, default empty = no water, zero risk to
existing worlds).
**Acceptance:** booth gets a `water_tile` subject (flat lighting read of the material) + tests for
mesh determinism/budget (≤8k tris per plane, ≤1 material). Runbook: W001 canal look on device.
**Budget:** 3 commits (mesh+material · foam+bob · WaterAuthor wiring).
**Do not:** attempt reflections, refraction, depth-texture effects, or tessellation. Quest floor.

## F3.4 — GROUNDING (the amateur-tell killer)

**Why:** objects sitting on clean ground with no contact response is THE most recognizable
"asset kit" tell. Grounding decals make scenes read as *places where time passes*.

**What:** `ForgeTexture` gains two bakeable decal sheets (one 512 atlas, create-only asset):
**stain** (drip streaks — vertical fBm smear, alpha soft) and **ring/contact** (radial dark
falloff, alpha). A `GroundingBuilder` editor pass (called from the same hook as
WorldDressingBuilder) places alpha-blended decal QUADS (flat, y+0.005, no colliders, static) by
deterministic rules: under every `Standpipe`/pipe-cluster prop → drip streak; at building wall
bases → a moss/grime skirt strip (reuse stain, tinted theme moss color); under crates/consoles →
contact ring. Budget: ≤40 decal quads per world, one shared material (alpha-blended, unlit-dark
multiply look via low-alpha black-brown).
**Acceptance:** tests — placement determinism from kit.seed, budget cap, no colliders. Runbook
look: W002 street "objects belong" check. **Budget:** 2 commits.
**Do not:** use URP decal projectors (cost), or place decals on dynamic objects.

## F3.5 — THE VFX FORGE (a particle vocabulary with rails)

**Why:** the air is empty and impacts are silent visually. Particles are the cheapest presence
multiplier in VR — and today every effect would be hand-rolled without budgets.

**What:** `VfxRecipeDefinition` (SO, closed enum `VfxKind { Impact, Muzzle, SteamVent, Motes,
Sparks, Drips }` + count/size/speed/color/lifetime ranges) + `VfxFactory.Spawn(id, pos, normal)`
resolving by string id from `Resources/Vfx` (ItemFactory pattern) + a `VfxLibrary` authoring the
starter set: `impact_metal`, `impact_stone`, `muzzle_taser`, `steam_vent`, `motes_amber` (W005),
`motes_spore`, `sparks_short`. Implementation: ONE pooled `ParticleSystem` per kind configured
from the recipe (no sub-emitters, no collision, no lights, max 64 particles per system,
`VfxRecipeDefinition.Validate()` enforces caps).
**Wiring (both sides!):** weapons call `VfxFactory.Spawn("impact_metal", hit)` from the existing
hit path in `CreatureRuntime.ReceiveHit`/wall hits; `WorldDressingBuilder` scatters ≤3 mote
emitters per world keyed by biome. Every id in the library must have a caller or a scatter row —
add the id list to the wiring tests.
**Acceptance:** recipe validation tests + caller-coverage test. Runbook: shoot a wall/creature,
see the impact; stand in W005, see motes. **Budget:** 3 commits (core+recipes · weapon wiring ·
world wiring). **Do not:** exceed 64 particles/system or 6 live systems (Validate + a runtime
clamp), use mesh particles, or add particle lights/collision.

## F3.6 — THE REACTIVE WORLD (art meets mechanics)

**Why:** Terry asked "mechanics-wise of the environment." In VR, the world responding to YOU is
presence. All the pieces exist (tells, VFX, damage routing) — nothing connects them to dressing.

**What:** `ReactiveProp` (Gameplay/Runtime, small): a component the dressing/building passes put on
eligible props with a closed `ReactionKind { LightFlickerOut, SteamBurst, SparkShower, Shatter }`.
It implements the existing damageable interface the guns already hit (same seam as BreakableWall —
find it, don't invent one): on hit → play its reaction (VFX id + emissive tell via the
ForgeBodyTell color-channel pattern + optional renderer swap to a "dead" state), one-shot or
cooldown 30s, never blocks, never drops loot. Wire: street lamps flicker out (and their halo
sprite disables), standpipes vent steam, signs spark then dim, small crates shatter into 3 chunk
debris (reuse WallChunkDebris).
**Acceptance:** pure state tests (hit → reacted → cooldown), wiring test (every ReactionKind has
a VFX id that exists). Runbook: shoot a lamp/pipe/sign in W002. **Budget:** 2 commits.
**Do not:** make reactions affect navigation/collision or spawn physics beyond the existing debris
cap (MaxLiveDebris shared).

## F3.7 — SIGNAGE & WAYFINDING (worlds read as a civilization)

**Why:** inhabited places have WRITING and light that means something. We have GlowPanel and
nothing to say with it.

**What:** a `GlyphBaker` in ForgeTexture: bakes abstract Shell-script glyph strips (procedural —
rows of angular fBm-seeded strokes; NOT English) into the emissive map of a new `sign_*` recipe
family (3 recipes: hanging sign, wall plate, route chevron) with per-world emissive color pulled
from the theme accent. **The wayfinding law:** route chevrons use ONE hue per destination class
(travel berth = the existing travel-door teal; jobs = amber; vendor = green — constants in
ZiptideConstants). `SignAuthor` hangs signs at POI approaches via the existing dressing hook
(≤6 signs per world).
**Acceptance:** booth turnarounds for the 3 sign recipes (glyphs read as writing, not noise) +
placement determinism test. **Budget:** 2 commits.
**Do not:** render real text/fonts (localization trap, fill-rate cost) or exceed theme colors.

## F3.8 — MACRO VARIATION (kills the tiling read on big surfaces)

**Why:** large walls/streets repeat their 1024 atlas visibly at distance. AAA surfaces layer a
low-frequency variation on top. Cheapest possible version:

**What ①:** `ForgeTexture.ComposeAlbedo` gains a final LOW-frequency layer for `buildingModule` +
future `street` tagged recipes: `c *= 0.94 + 0.12 * Fbm(u*1.7, v*1.7, seed 173, 2)` — inside the
existing style stack so every baked module varies across its own atlas.
**What ②:** per-INSTANCE variation: `ForgeModuleLook` gains `tintJitter` (default 0.05) —
multiplies `_BaseColor` by 1±jitter from a position-hash so identical wall modules differ
slightly down a street. MaterialPropertyBlock, not material instances (batching stays intact —
verify with the Frame Debugger note in the perf doc).
**Acceptance:** existing atlas x-rays show the variation; a test pins determinism; PerfBudget
stays green (no new materials). **Budget:** 1 commit. **Do not:** add texture arrays or a second
UV channel — this is deliberately the cheap version.

## F3.9 — THE CONFORMANCE GATE (the "consistently better across the board" machine)

**Why:** Terry's exact ask — EVENNESS. The only way quality stays even across lesser-model lanes
is a gate that refuses unfinished visuals, per world, forever.

**What:** `ArtConformanceAuditRules` in the audit runner: every Renderer in a shipped world scene
must trace to a known provenance — a Forge recipe/baked prefab, a registered kit module, the
SkyVista rig, water/decals/VFX/signs from F3.x authors, or an explicit whitelist entry (file:
`Editor/Audit/art_conformance_whitelist.txt`, one scene:objectName per line, each line requires a
comment WHY). Everything else = `ART_UNCONFORMED` — starts WARN with a count; **the ratchet:** a
world that reaches 0 warnings gets added to the `conformanceLockedWorlds` list, where any
regression is a BLOCKER. Report the per-world count in every audit summary so Terry sees the
number shrink.
**Acceptance:** the rule + ratchet tests; W002 driven to 0 as the pilot. **Budget:** 2 commits.
**Do not:** whitelist wholesale to get to zero — every line needs its WHY.

## F3.10 — CREATURE POLISH REMAINDER (P4 close-out)

Look-at (head bone yaws ≤30° toward the player within 8m — the warden's eye already tracks via
behavior; this moves the HEAD; pure math in ForgeGaitMotor, conjugation law applies) ·
stun-droop (taser stun → all limb chains sag 15° + breath amplitude ×2 — wire from the existing
stun state) · distance culling (SkinnedMeshRenderer → `ForgeCreatureVisualApplier` sets
`localBounds`-based LOD: beyond 40m swap to the frozen statue clone trick from ForgeBodyTell,
re-skin within 35m; hysteresis so it never pops at the boundary). Tests per the ForgeGaitMotor
pattern. **Budget:** 2 commits.

---

## Execution order & why
F3.1 light → F3.2 grade (both derive from vistas; together they transform every existing world
for ~4 commits of work) → F3.3 water (namesake; W001/tidefront identity) → F3.4 grounding →
F3.5 VFX → F3.6 reactive → F3.8 macro variation (1 commit, slot it anywhere) → F3.7 signage →
F3.9 conformance gate (turn on early at WARN — ideally right after F3.2 — ratchet as worlds
finish) → F3.10 creature close-out. Estimated 21 commits total; every one independently green.

## What is deliberately NOT in FORGE III
Real-time shadows beyond the key light · reflections/refraction · post bloom/DoF · texture
arrays/streaming · real text · GPU particles/VFX Graph · terrain systems · any networking of
visuals. Each is a Quest-budget or complexity trap for a smaller model. If one seems needed,
that's a Terry conversation, not a commit.

## Session-end contract (unchanged from FORGE II)
Boards in the same commit (SPRINT_ART row + HANDOFF entry) · runbook items for every device-only
check · `docs/EXCELLENCE_MAP.md` row updated when an aspect's state changes · this file's envelope
statuses updated as they close (add ✅/🟡 markers inline).
