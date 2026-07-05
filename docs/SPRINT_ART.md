# 🟡 ACTIVE SPRINT — THE ART & AUDIO TRACK (M6 pulled parallel, opened 2026-07-02)

> **⚡ ART-2 — THE ASSET FORGE (Terry's LLM studio) — ACTIVE 2026-07-03.** Prompt → recipe → generated
> mesh → CI-rendered turnarounds an LLM can SEE → iterate → APK. Manual: `project_art_plan/FORGE_STUDIO_GUIDE.md`.
> | # | Task | Status |
> |---|------|--------|
> | 1 | SPIKE: PhotoBooth + forge-photos workflow; verify a session can view CI-rendered PNGs | ✅ `394ed14`/`ceb3df3` — photos viewed by this session |
> | 2 | ForgeRecipeDefinition + ForgeMesh + ForgeMaterials + 13 mesh tests | ✅ `1daa597` (+winding fix `7d6f031` — CI's outwardness tests caught it) |
> | 3 | ForgeRecipeLibrary + `taser_gun_mk1` + 7 catalog tests + booth renders recipes | ✅ `602587c` |
> | 4 | Coordination: ItemDefinition.forgeRecipeId + ItemFactory hook + ForgeVisualApplier + ForgeAuthor + BuildAndroid/audit hooks + studio guide + playbook rows | ✅ `1e69879` (HANDOFF mmm) |
> | 5 | Iterate the taser via turnarounds (v2 verified in photos: solid barrel, seated back cap, raked grip — reads as a salvage stun pistol) | ✅ `e88e70e` |
> | 6 | Close: CI+photos green on head · APK dispatch · runbook §2i · HANDOFF wrap | ✅ **APK run `28630103333` green — the forged taser is in the sideloadable artifact; FORGE_* audit rules live** |
>
> **ART-2 SHIPPED 2026-07-03.** The studio loop is proven twice over (calibration + taser v1→v2 critique
> cycle). Next: **ART-3 — W001 Toxic Venice pass built WITH the Forge** (buildings/props as recipes +
> PERF_BUDGET audit rule), then audio (ART-4), creatures/gear via story-lane coordination (ART-5).
> New assets meanwhile = pure prompt work: add a recipe, push, view, iterate (FORGE_STUDIO_GUIDE.md).

> **Takeover prompt: "Read docs/SPRINT_ART.md and continue."** This is the art track's live state — it
> runs IN PARALLEL with the story track (`docs/SPRINT.md`, M4 ship) and the multiplayer track
> (`docs/SPRINT_MULTIPLAYER.md`, M7) with **zero file overlap**. Operator name for this track: **Picasso**
> (Fable 5). Roadmap slot: `GAME_PLAN.md` **M6 — Look & Sound**, promoted by Terry 2026-07-02 to run
> early as its own track ("get the art aspects AAA — especially the skyscapes, but also buildings,
> creatures, sound"). Specs: `docs/project_art_plan/` (pillars, surface families, W001 brief, Quest
> budget) + `docs/design/ADAPTIVE_AUDIO.md` + `docs/systems/ASSET_SWAP_PIPELINE.md`.

## Lane ownership (zero-overlap contract)
**This track owns:** `Ziptide/Assets/Ziptide/Visuals/**` (nobody else claims it) · new
`Editor/Patching/SkyVista*.cs` / future `Art*.cs` authoring files · new `Editor/Audit/SkyVistaAuditRules.cs`
(+ future art audit rule files) · `Content/Worlds/SkyVistas/**` (generated assets) ·
`docs/project_art_plan/**` · this sprint file.
**Not touched:** `Multiplayer/**`, `Gameplay/Runtime/Pvp/**`, `ScenePatcherArena`, `ArenaLayoutDefinition/Library`
(MP lane) · `CityBuilder`, `WorldLayoutLibrary`, `WorldStubGenerator`, `ThemeAuthor`, creatures/RILL/ship
(story lane) · scene YAML (never, per house rule).
**⚠ Shared coordination files** (append-only, 1–3 lines each, announced in `HANDOFF.md` before pushing;
rebase-before-push makes conflicts trivial):
1. `Editor/Build/BuildAndroid.cs` — one try/catch hook (vista author) after the per-scene loop, before the audit.
2. `Editor/Audit/WorldAuditRunner.cs` — one `SkyVistaAuditRules.Run(sceneReport)` call line.
3. `Tests/EditMode/Ziptide.Tests.EditMode.asmdef` — add `"Ziptide.Visuals"` reference.

## Sprint ART-1 — SKYSCAPES EVERYWHERE ("every sky should make you stop")
Data-driven **SkyVista** system: per-world movie-grade skies for all 12 story worlds + ToxicCity + the 5
arenas, implementing the canon sky progression (STORY_BIBLE: the banded gas giant that GROWS across early
worlds; the hexagonal **Shell grid** reveal 0 → faint W007 → banding W009 → full wall W012; W003's two
moons + zenith Pattern shimmer; RILL's cyan seeded in early nebulae). Attaches at the existing theme seam:
`SkyVistaDefinition` (optional field on `VisualThemeProfile`) → `SkyPlanetRig` delegates to `SkyVistaRig`
→ zero Gameplay/layout/patcher files change. Budget per world: dome + ≤3 bodies = ≤4 draw calls,
≤5 materials, ~3k tris, ~3 MB textures (runtime-baked, Bayer-dithered against VR banding).

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | Claim: this sprint file + HANDOFF entry + GAME_PLAN/CHECKLIST notes | ✅ `ce051fd` (CI green) |
| 1 | `SkyVistaDefinition` (SO: gradient/stars/nebula/≤3 celestial bodies/shell grid/shimmer/light+ambient+fog) + `SkyVistaTexture` (pure Color32 bake: value-noise stars+nebula, hex-grid layer, 4×4 Bayer dither) + 14 EditMode tests + Tests-asmdef `Ziptide.Visuals` ref ⚠ | ✅ `0166d8a` (CI green) |
| 2 | `SkyVistaRig` (runtime: 1024×512 composited dome + 0–3 body spheres, URP/Unlit, no shadows; scene tie-ins after theme base pass) + `VisualThemeProfile.skyVista` field + `SkyPlanetRig` delegation (null = legacy path, zero visual change) | ✅ `c009795` (CI green) |
| 3 | `SkyVistaLibrary` (create-only, WorldLayoutLibrary pattern): 17 canon vistas as data (ToxicCity reserved + W002–W012 + 5 arenas) + `Specs()` + 8 progression tests (grid 0→1 monotonic, giant growth, W003 moons+shimmer, RILL cyan, arena distinctness) | ✅ `5c63eae` |
| 4 | `SkyVistaAuthor.AssignAll()` (sets `skyVista` on each `Content/Worlds/Themes/<Scene>_Theme.asset` — worlds AND arenas — after the build's per-scene loop) + BuildAndroid hook ⚠ | ✅ `dc84f82` |
| 5 | `SkyVistaAuditRules` (`SKY_VISTA_MISSING`/`SKY_VISTA_INVALID` blockers; `SKY_VISTA_UNWIRED`/light-count warnings) + WorldAuditRunner call line ⚠ + `HOW_TO_CHANGE_ANYTHING.md` sky rows | ✅ `48ee4b4` |
| 6 | Close: APK dispatch green · runbook §2h 🎮 items (canon progression reads, 72fps, banding check) · HANDOFF wrap | ✅ **APK run `28616598718` green (70 MB artifact, audit-clean incl. SKY_VISTA rules)** |

## 🟡 ACTIVE — ART-3: FORGE II, THE QUALITY LEAP *(numbering fixed per qqq: ART-1 skies · ART-2 Forge ·
ART-3 THIS · ART-4 W001 via Forge+registry · ART-5 audio)*
**Plan of record: `docs/project_art_plan/FORGE_II_QUALITY_LEAP.md`** — Terry-approved 2026-07-04,
written as executable envelopes (all design decisions made; implement, don't re-litigate). Quest 3
is the confirmed perf floor. The two qqq envelopes fold in as E5.1 (building modules) + E5.2
(PERF_BUDGET gate).

| # | Task (envelope in FORGE_II doc) | Status |
|---|------|--------|
| E1.1 | UV atlas in ForgeMesh + tangents + tests (visuals unchanged) | ✅ `241f2c4`+fix `f6415f5` (CI green) |
| E1.2 | ForgeTexture rasterizer + albedo styles + schemaVersion 2 → textured taser in booth ("not flat" checkpoint) | ✅ `ba6f869`+tune `6e0509c` — CI green, checkpoint photos verified |
| R1–R4 | **ASSET FORGE RECONCILIATION** (Terry-approved 2026-07-04, map in `ASSET_FORGE_MAP.md`): R1 docs `3929246` · R2 lifecycle/lock/refs `541c6a3` · R3 staleness buckets + manifest `6cfabfc` · R4 stalker `215e110`→v3 `196f57d` (photo loop ×3: `Limb()` joint-point legs; turnarounds pass, run `28692323345`) | ✅ **ALL CLOSED** — E1.3 active |
| E1.3 | Normal/MSA/emissive maps + ONE material per asset | ✅ `8a086f7`+`15bdae9`+booth env `1e64c68`+x-ray `e0211c3` — taser checkpoint passes; atlas x-ray now in every photo artifact (HANDOFF bbbb) |
| E1.4 | ForgeBaker (build-time bake → ASTC) + gitignore + applier/booth prefer baked → **Terry photo checkpoint** | ⬜ |
| E5.2 | PERF_BUDGET audit gate (independent — do anytime, 1 commit) | ⬜ |
| P2 | New ops (Capsule/Frustum/Torus/SweepSpline/OrganicBlob) + modifiers + Quest-3 class budgets | ⬜ |
| P3 | ForgeCreatureBody + ForgeSkinnedBuilder + ⚠ coordination commit (forgeBodyId) + drone/swarmer bodies | ⬜ |
| P4 | ForgeMotor (velocity-observing gaits/waves/breath/look-at/stun-droop) + booth pose shot | ⬜ |
| E5.1 | Building-module family for `salvage_row`+`toxic_tenement` via ArtModuleRegistry (pairs with story Q2d) | ⬜ after E1.x |
| E5.3 | Flora (LeafCard+ForgeSway, 2 plants) + 3 W001 props | ⬜ |
| — | Close: guide v2 rubric · APK · runbook §2n · HANDOFF | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current:** ART-1 + ART-2 shipped APK-green (skies `28616598718`; Forge+taser `28630103333`).
  ART-3 opened; no code landed yet. Open Terry loops: §2h/§2i headset passes.
- **Next action:** execute **E1.1** exactly as specced in `FORGE_II_QUALITY_LEAP.md` §P1 (UV atlas
  in `Visuals/Runtime/Forge/ForgeMesh.cs`, tests first, visuals unchanged, 1 commit). Then E1.2.
  E5.2 (PERF gate) is a good first commit for a fresh session — independent and fully specced.
- **Story-lane request (still queued):** ToxicCity's patcher authors no theme, so its sky vista
  can't attach — one `ThemeAuthor.EnsureThemeAsset(kit)` call in `ScenePatcherToxicCity` wires it.
- **Branch:** `terry-local-wip`, house rules (pull --rebase, small CI-green commits, .meta per new
  file, no scene YAML). Operator manual: `docs/OPERATOR_START_HERE.md`. Photo loop: push any
  `Forge*`/`Visuals/Runtime/Forge/**` change → `forge-photos` workflow auto-renders → download the
  artifact → view → iterate against the FORGE_II quality rubric.

---
*Opened 2026-07-02 by Picasso (Fable 5) on Terry's directive. Working rules identical to the other tracks.*
