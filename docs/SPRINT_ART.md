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
| E1.4 | ForgeBaker (build-time bake → ASTC) + gitignore + applier prefers baked → **TEXTURES ON DEVICE** | ✅ `b93839e` — bake hooked in BuildAndroid; FORGE_APPLIED logs baked=true/false |
| ARSENAL | pistol_scrap_mk1 · static_net_lobber · sonic_thumper_maul · prism_beam_rifle recipes + ForgeAuthor assignments (every gun textured+baked) | ✅ `797b5a3` — all four pass turnarounds (run `28793858850`); niggles boarded: maul haft thin, lobber side vents unclear |
| AVATAR | PlayerAvatarRig — salvage gloves + chest rig (the player's skin) | ✅ `2d57076` — rig-ensured; QUARTERS recolors later |
| E5.2 | PERF_BUDGET audit gate — tris/materials/renderers/lights per scene, warn at target / block at cap | ✅ `PerfBudgetAuditRules` + runner line (this commit) |
| P2 | New ops (Capsule/Frustum/Torus/SweepSpline/OrganicBlob) + modifiers + Quest-3 class budgets | ✅ **SHIPPED (2026-07-09, 2 commits)** — ① all 5 ops + taper/bend/noise modifiers (weld-safe fBm, pre-transform, fixed order taper→bend→noise) + validation contracts + old-asset hash stability + 13 P2 tests. ② storyTag class budgets (hull 15k · creature 10k · handheld 6k · prop 3k · plant 1.5k) enforced in Validate() + **`p2_tide_totem`** booth showcase (every new op + modifiers-on-a-legacy-op in one photographable prop). **⏳ verify the totem's turnarounds in the next forge-photos artifact** |
| P3 | ForgeCreatureBody + ForgeSkinnedBuilder + ⚠ coordination commit (forgeBodyId) + drone/swarmer bodies | ✅ **ORGANIC ROSTER COMPLETE (2026-07-10)** — genomes: swarm_bug · light_grazer · tendril · **witness_mite · husk_molter · warden** (the last three via the NEW `ForgeBodyTell` tell bridge: warden eye-states drive the genome's emissive eye, mite's stone-freeze = body-tint channel, molter's decoy = a frozen grey CLONE of the forged body). **tether_swarm = documented intentional skip** (its procedural clusters+cord ARE the mechanic). 5 bridge tests; genomes auto-covered by ForgeBodyLibraryTests. **PHOTO VERDICTS (run `29083744395`): tendril ✅ · witness_mite ✅ (lens glows, the stare reads) · husk_molter ✅ (ridge fin + amber eye) · warden ❌v1 "dark chess pawn" → ❌v2 "bin with a lid" → v3 sentinel proportions.** Drones = DroneRuntime path, separate seam |
| P3.5 | **CREATURE QUALITY LADDER** (Terry's bar: "fox with a trash can lid → moving breathing xenomorph") | 🟡 **v4 SHIPPED + JUDGED (2026-07-10)** — the ladder so far: v2 smooth shading honored + capsule limbs (builder fix) → v3 bind-pose articulation (`bendDegrees`) → **v4 TEXTURE BAKE** (`fad9898`, CI green): `SyntheticRecipe` seam bakes each styled genome's albedo/normal/MSA/emissive atlas; applier + booth agree by construction; eye submesh stays live-emissive so ForgeBodyTell is untouched. **v4 verdicts (run `29089183510`): swarm_bug ✅✅ glossy amber chitin · husk_molter ✅✅ wet mossy carapace · tendril ✅ bark-speckled vines · witness_mite ✅ · light_grazer ⚠ plain (slime style too subtle) · warden ❌ near-black monolith, NO ARMS = still a bin.** → **v5 (run `29101201206`): warden ✅ AT LAST** — arms (3-seg mirrored chain on GaitRole.Leg = contralateral swing for free, 11 bones), split angled pauldrons + collar, near-black visor, lifted palette: reads as an armored bipedal SENTINEL. Grazer mottle v1 vanished under the wet-sheen specular. → **v5.1 (this commit):** warden fists slate (bright steel read as white gloves) + chest seam narrowed; grazer underbell+skirt become the LANTERN (GlowPanel soft green — it's called a LIGHT grazer) + mottle deepened/widened to survive the highlight. **✅ v5.1 VERIFIED (run `29101754484`): warden ✅✅ done (slate fists, thin seam — clean sentinel) · grazer ✅ (luminous lantern ring reads; mottle subtle under booth light — real check is the dark cistern, runbook 🎮). ROSTER JUDGED SHIPPABLE at this engine tier.** → **v5.2 (this commit): segments cap 16→32** (class tri budgets stay the perf gate) + hero blobs/domes on all 6 genomes rounded (bell 24, bug carapace 22, warden torso/head 18, …) — kills the faceted-rim tell. ⏳ verify v5.2 turnarounds; then E5.3 flora |
| P4 | ForgeMotor (velocity-observing gaits/waves/breath/look-at/stun-droop) + booth pose shot | 🟡 **gaits/waves landed with the P3 pipeline** (ForgeGaitMotor + ForgeCreatureAnimator, conjugation law, 6 contract tests) · **BREATH (creature v5.3, this commit):** pure `BreathScale` channel — root-bone XZ chest swell w/ counter-Y, deepest at idle, fades 60% at full run, per-instance phase so packs desync; 2 new tests. Remaining: look-at · stun-droop · booth pose shot |
| E5.1 | Building-module family for `salvage_row`+`toxic_tenement` via ArtModuleRegistry (pairs with story Q2d) | 🟡 **WALLS SHIPPED (2026-07-10, 2 commits)** — 4 textured wall recipes (solid+window × both styles, canonical 3.0×3.2×0.25 envelope, tenement gets P2 Capsule standpipe + Frustum vent) + `ForgeBuildingKit` WRAPS the primitive kit's registry factories with a runtime `ForgeModuleLook` swap (ItemFactory pattern: scene ships primitives, device shows baked textures; interior Pane survives). 5 kit tests + booth turnarounds next run. **✅ PHOTO-VERIFIED (run `29061133611`)** — all 4 walls read (rust panels+patch plate / mossy panels+standpipe+vent; reveal open; accents on the +Z face = booth's 04_back view). CI green `3d3414f`/`d37d262`. Polish niggles (later): pipe bracket small, plate bolts faint. Doorway/CornerTrim/Roof ids = future (BuildingBuilder doesn't consume them yet — both-sides law) |
| E5.3 | Flora (LeafCard+ForgeSway, 2 plants) + 3 W001 props | 🟡 **ENGINE LANDED (commit 1/2):** `ForgeOp.LeafCard` (two crossed quads, double-sided, base y=0, 8 tris — ≤2 overdraw by construction; box-planar UVs already map each plane across the island) · Leaf style now OWNS the albedo alpha (`LeafAlpha01` sine-blade + serrated margin + stalk; ~2-texel soft edge for clean clip mips) · alpha-clip material trigger (`HasLeafSlot`) in ForgeBaker + booth preview · `ForgeSway` (1–3 pivots, pure `SwayRotation`, per-instance desync) · 5 tests (ForgeFloraTests). **CONTENT LANDED (commit 2/2):** `flora_frond_w005` (3 bend-drooped LeafCards + bark stem + glowing spore nub) · `flora_reed_w001` (3 tall reeds out of a noised mud clump) · `prop_patched_crate` · `prop_pipe_cluster` (glowing gauge) · `prop_dispatch_console` (teal screen focal) — all under class budget rails; scatter WIRED: WorldDressingBuilder Tufts now builds unscaled holders w/ ForgeModuleLook (Canyon→frond, TideFlats→reed) + ForgeSway (block de-statics so the sway isn't batched-stale). **BOOTH VERDICTS (run `29119297998`): reed ✅ (real pointed blades out of a glossy mud clump — THE ALPHA CUTOUT WORKS) · crate ✅ · console ✅ · frond ❌ v1 (wide cards curled into an avocado clump, nub buried) → v2 fern read (narrow arcing blades, nub above the crown) · pipes ⚠ (chocolate-glossy → greyed iron, gauge unburied). ONE CI RED (red #1): lifecycle contract wants worldRuleRefs on every catalog recipe — crate/console shipped without → fixed same commit.** **✅ v2 VERIFIED (run `29119947334`, CI green `e30f8af`): frond ✅ (fern read — arcing blades, twisted stem, spore nub at the crown) · pipes ✅ (iron + rust streaks; gauge on the +Z face). E5.3 CLOSED — FORGE II IS ENVELOPE-COMPLETE** (P4 look-at/stun-droop remainder moved to FORGE III §F3.10) |
| — | Close: guide v2 rubric · APK · runbook §2n · HANDOFF | ⬜ |

## ⏭ NEXT — FORGE III: THE COHESION LEAP (written 2026-07-10 for Opus 4.8 / Sonnet 5)
**Plan of record: `docs/project_art_plan/FORGE_III_PLAN.md`** — Terry's directive: "consistently
better across the board + what didn't we think of." The layer nobody sees as an asset: **light
script (derived from vistas) · per-world grade · WATER (the namesake!) · grounding decals · a VFX
vocabulary · the reactive world · signage/glyphs · macro variation · the ART CONFORMANCE RATCHET
(the evenness machine) · creature P4 close-out.** 10 envelopes, ~21 commits, every decision made,
every envelope with a checkpoint. Takeover prompt: **"Read docs/project_art_plan/FORGE_III_PLAN.md
§0 and execute the next open envelope."**

| # | FORGE III envelope | Status |
|---|------|--------|
| F3.1 | THE LIGHT SCRIPT — light rig DERIVED from the vista (fog=horizon, ambient=gradient, key from the brightest body, elevation clamped 20–55°) | 🟡 **commit 1 landed:** `SkyLightScript.Derive` (pure) + `SkyVistaRig` applies derived values wherever the vista is silent (authored tie-ins override; layout-baked fog respected; `ZIPTIDE: LIGHT_SCRIPT` log) + 5 contract tests incl. full library sweep. **commit 2:** `LIGHT_SCRIPT_NO_SUN` warn in SkyVistaAuditRules + runbook 🎮 W002/W005 before/after. **F3.1 CODE-COMPLETE** (device verdict = Terry's runbook pass) |
| F3.2 | THE GRADE — per-world ACES + range-clamped color adjustments, DERIVED (filter = 8% toward the desaturated horizon; haze desaturates; warmth follows the horizon; exposure lifts dark skies) | 🟡 **landed:** pure `SkyGrade.Derive` (the clamps ARE the contract, tested) + rig builds a scene-local global Volume (ACES + ColorAdjustments + WhiteBalance) and enables camera post; ⚠ Visuals asmdef gains `Unity.RenderPipelines.Universal.Runtime` ref; `ZIPTIDE: GRADE` log; 4 tests incl. library sweep. ⏳ CI + Terry device look (post cost on Quest!) |
| F3.1b | PRACTICALS — lanterns/sconces/street lamps + halo + light-pool decals (Terry's direct ask) | 🟡 **commit 1: the 3 fixture recipes** (`light_lantern_hang` hook+ring+caged amber glass · `light_sconce_wall` plate+bracket+half-dome (+Z against the wall) · `light_street_pole` 3m tapered pole, arm, downward head + glowing disc) — shared warm-amber practical palette (`lamplight is warm, never neon-pure`), 400-tri budget, auto-swept by library gates. **commit 2: `PracticalLight`** — halo billboard (runtime radial sprite, URP/Unlit additive One/One no-ZWrite so halos never face-fight) + light-POOL quad from the author's build-time surface point + ONE `SetLit` switch killing halo+pool+per-instance emissive (MPB — baked mats are shared) together; 2 headless tests. **✅ FIXTURE VERDICTS (run `29122261557`, CI green): lantern ✅✅ (hook, ring, cap, glowing amber body — first-try pass) · sconce ✅ (plate + drip guard + glowing half-dome) · pole ✅ (classic silhouette, amber ring glows downward — the player's view from below).** **commit 3/3 `PracticalAuthor` ✅ (Opus 4.8)** — wired into `WorldDressingBuilder.Build` after the building pass (so `__DOOR` markers exist): sconce beside ≤6 sorted doorways, poles every ~14m on the route (≤6, offset off the walk line), 2 lanterns at POI approaches; ≤14/world; each = holder + primitive Fallback + ForgeModuleLook + PracticalLight self-building halo+pool in Awake. The 2 lanterns carry the world's whole hero real-light budget (2 point lights, range 8, no shadows → 1 sun + 2 = 3 = PerfBudget cap, WARN-only). **F3.1b CODE-COMPLETE; ⏳ CI + device look (hero-light frame cost is the runbook risk).** |
| F3.3–F3.10 | water · grounding+blob shadows · VFX · reactive · signage · macro · conformance ratchet · creature close-out | ⬜ per the plan |

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
