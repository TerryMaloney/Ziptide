# ASSET FORGE MAP — external-spec vocabulary → Ziptide's living systems

**Why this doc exists:** Terry receives architecture briefs from outside advisors (GPT etc.) written
against stale snapshots of this repo. This is the ROSETTA STONE: before building anything a brief
asks for, check here — **most of "the Asset Forge" already exists, CI-proven, under these names.**
Reconciliation approved by Terry 2026-07-04 (HANDOFF xxx); improvements adopted are marked ★.

## The mapping

| Spec concept | Lives here as | Notes |
|---|---|---|
| Story Bible | `docs/storyboard/STORY_BIBLE.md` + `WORLD_DATA.md` | canon → data pipeline, richer than the spec |
| World Art Bible | `ART_DIRECTION_MASTER_PLAN.md` (8 surface families) + per-world layout sky/palette blocks + `FORGE_II_QUALITY_LEAP.md` | machine-readable via layout/vista/style fields |
| Design tokens | `ForgeStyle`/`ForgeStyleSpec` + `ForgePalettes` family law + `storyTags` | tokens-as-CODE: tested, build-failing |
| Asset Genome | `ForgeRecipeDefinition` (+ ★lifecycle/refs fields) | the asset's DNA incl. sockets/budget/`Validate()` |
| Asset Brief | `ForgeRecipeLibrary.Build*()` + FORGE_II envelopes | the "instruction packet" is reviewable code |
| Stable IDs / registry / fallback | `design/ART_REGISTRY.md` + `ArtModuleRegistry` + `forgeRecipeId`/`skyVista`/`buildingStyleId` seams | "fallback is sacred" = spec's proxy law |
| Playable Production Proxy | ItemFactory primitives w/ Grip/Muzzle contract + colliders + stable ids | ★formalized per type in `PROXY_CONTRACTS.md` |
| Upgrade-slot prefab | `ForgeVisualApplier`: contract root stable, `ForgeVisual` child replaceable | |
| Quest budgets | `QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md` + per-class caps in `Validate()` + `FORGE_*` blockers + PERF gate (E5.2) | |
| PBR contract | FORGE II E1.3/E1.4: albedo/normal/**MSA**/emissive | ORM packing rejected — URP/Lit reads MetallicGloss natively |
| Critique rubric/reports | `FORGE_STUDIO_GUIDE.md` rubric + CI photo turnarounds + `audit-report` artifact | ★forbidden-aesthetics added as fail conditions |
| Backend-agnostic generation | ART_REGISTRY §5 deferral triggers (Tripo/glTFast/Addressables) | ids never change when backends do |
| Deterministic CI backend | the Forge itself + 400+ EditMode tests, zero paid APIs | |
| Quality states + locking | ★`ForgeQualityState` + `lockedContentHash` (lock = human-baked baseline) | |
| Dependency graph + staleness | ★`ForgeDependencyAuditor` (`IForgeDependencySource`) → safe-auto/review/breaking/deprecated buckets | |
| Manifest | ★`Builds/Reports/FORGE_MANIFEST.json` (artifact-only, deterministic) | |

## Rejected — and WHY (so it stays rejected)
1. **Python/JSON-schema/YAML toolchain**: the verifier here is Unity CI (EditMode tests + audit
   blockers). A parallel schema layer = second source of truth the gates can't see. Everything it
   would validate already FAILS THE BUILD via `Validate()` + tests.
2. **New docs trees duplicating canon** (`docs/STORY_BIBLE/`, 15-file `docs/ASSET_FORGE/`): canon
   lives in `docs/storyboard/`; art law in `docs/project_art_plan/` + `design/ART_REGISTRY.md`.
3. **ORM-packed textures**: needs a custom shader for zero gain on URP (same logic that rejected
   vertex colors in FORGE II).
4. **Per-brief `generation_backend` field now**: premature — ART_REGISTRY §5 records exactly when
   external backends join.

## Extension points for the spec's "things we haven't thought of"
Audio material rules → `ADAPTIVE_AUDIO.md` + ART-5 · VFX rules → FORGE II P4/E5.3 + Bloom/Pattern
language (`ALIEN_ORIGAMI_SURFACE_BRIEF.md`) · faction iconography → `ForgePalettes` families +
`CITY_DESIGN.md` zoning · damage variants/skins → `CosmeticDefinition` seam · lighting tokens →
`SkyVistaDefinition` tie-ins · texture streaming/batching → ART_REGISTRY §5 deferrals + PERF gate.
Each attaches as data + a `Validate()` rule + an audit blocker — never as a parallel toolchain.

## The north star (Terry's, verbatim goal)
Story/world changes → affected-assets report → safe/review/breaking buckets → proxy/upgrade passes
— **without breaking stable IDs, sockets, colliders, prefab contracts, or gameplay.** Delivered by:
structured refs on every recipe + the dependency auditor + lock baselines + create-only authoring.
