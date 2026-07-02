# 🟡 ACTIVE SPRINT — THE ART & AUDIO TRACK (M6 pulled parallel, opened 2026-07-02)

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

## Next sprints (order per Terry)
- **ART-2 — W001 Toxic Venice full pass:** SurfaceSet/WorldArtKit registries + ArtBuildPlan pipeline +
  PERF_BUDGET audit rule + primitive kit → Tripo mesh swap behind stable IDs (`W001_TOXIC_VENICE_ART_BRIEF.md`).
- **ART-3 — Audio foundation:** `AdaptiveAudioManager` (4-stem, Signal-reactive), ambience beds, SFX
  library, RILL VO (ElevenLabs) + Transmission blend-voice specs for Terry (`ADAPTIVE_AUDIO.md`).
- **ART-4 — Creatures/gear/VFX:** visual kits for creatures (spec + handoff to story lane, which owns
  creature files), weapon models via Tripo, Bloom/Pattern VFX language (`ALIEN_ORIGAMI_SURFACE_BRIEF.md`).

## ▶ RESUMING? — current state & exact next action
- **Current:** **SPRINT ART-1 CODE-COMPLETE + APK-GREEN** (run `28616598718`, 70 MB artifact,
  all scenes audit-clean with the SKY_VISTA rules live). Every generated world + arena ships a canon
  sky. Open loop: Terry's §2h headset pass (canon progression reads / 72fps / banding).
- **Next action:** start **ART-2 — W001 Toxic Venice art kit** (brief:
  `project_art_plan/W001_TOXIC_VENICE_ART_BRIEF.md`): `SurfaceSetDefinition` + `WorldArtKitDefinition`
  data model first (pure + tests, same commit pattern as ART-1), then the primitive ToxicVenice kit,
  then the PERF_BUDGET audit rule. Fold in Terry's §2h ❌s first if any arrive.
- **Story-lane request (queued, not urgent):** ToxicCity's patcher doesn't author a theme, so its
  waiting vista can't attach — one `ThemeAuthor.EnsureThemeAsset` call inside `ScenePatcherToxicCity`
  (story-owned file) wires it. Coordinate via HANDOFF when convenient; W001 gets its full art pass in
  ART-2 anyway.
- **Branch:** `terry-local-wip`, same rules as the other tracks (pull --rebase, small CI-green commits,
  .meta per new file, TextMesh only, no scene YAML).

---
*Opened 2026-07-02 by Picasso (Fable 5) on Terry's directive. Working rules identical to the other tracks.*
