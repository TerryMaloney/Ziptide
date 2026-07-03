# 🏗 ARCHITECTURE V2 — the overhaul program (Terry's PDF, distilled into Ziptide terms)

**Source:** Terry's uploaded report *"Architecting Deterministic Procedural Worlds for VR"*
(2026-07-03) + his device verdict ("systems fire, EXPERIENCE fails"). **Mandate:** overhaul BOTH the
backend architecture and the literal in-game architecture (buildings), such that **any LLM can build
and iterate without breaking things** — request a change as data, gates catch mistakes at build time.
Execution board: **`docs/SPRINT_ARCHITECTURE.md`**. This doc is the WHY + the laws; the board is the state.

## What the PDF asks for vs what Ziptide already has
| PDF section | Status in Ziptide |
|---|---|
| §1 Spec-driven + TDD + pure Model layer | ✅ idiomatic (pure cores, EditMode tests, patcher indirection, CLAUDE.md law) — V2 adds the missing unifying SPEC (below) |
| §2 Git/CI (unityyamlmerge, GameCI, matrix) | ✅ mostly (`.gitattributes` has unityyamlmerge; GameCI green) · LFS deliberately NOT adopted — the Forge strategy makes binaries near-zero; revisit if repo binaries exceed ~200MB |
| §3 Macro terrain (fBM, domain warp, biome matrix, chunking) | 🟡 P1a shipped seeded heightfield w/ 5 presets (single-layer hash noise) → **Q3 upgrades the math**; chunking/LOD deferred (worlds are 240–400m bowls, not infinite) |
| §4 Meso architecture (BSP/OBB lots, WFC modules) | ❌ districts are authored slabs → **Q2 is the program's visible win** |
| §5 Micro (modular assembly, palette JSON, IK) | ✅ the Forge IS this for props/weapons (recipes = descriptors); creatures follow via Forge kits (Picasso ART-4); IK skipped (primitive bodies don't need it yet) |
| §6 Semantic PCG (schema-governed LLM → constraints) | ❌ each system has its own author method → **Q1 WorldSpec is the keystone** |
| §7 VR perf (pooling, batching table, Addressables, 72fps) | 🟡 gates exist for sky/world/forge; **Q4 adds pooling + the PERF_BUDGET gate**; Addressables deferred with an explicit trigger (below) |

## THE LAWS (the LLM-operability contract — additions to CLAUDE.md's canon)
1. **Spec is truth.** A world is one `WorldSpec` document. Changing a world = editing its spec (or the
   library method that authors it), never scene objects, never scattered assets. The compiler
   (`WorldSpecCompiler`) fans the spec out into the existing data assets; the factory builds from those.
2. **Pure core first.** Every generator's decisions live in a pure C# class (no UnityEngine) with
   EditMode tests, seeded and deterministic: same seed → byte-identical output. The scene/patcher half
   only translates. (BotBrain/Conquest/ForgeMesh/PoiQuality all already prove this pattern.)
3. **A gate per quality dimension.** Anything that made Terry say "not great" gets a build-failing
   audit blocker so it can never silently regress: world size/terrain/vista/POIs/verbs/play-minutes
   (shipped), sky (shipped), forge budgets (shipped), buildings + perf (this program).
4. **No hand YAML, no hidden state, no reflection.** (Existing canon, restated because V2 multiplies
   generators.)
5. **Deterministic-from-seed or it doesn't merge.** `Date.now`-style entropy, `UnityEngine.Random`
   without a seeded state push, or order-dependent dictionary iteration in a generator = review reject.

## The phases (full task detail on the board)
- **Q1 — WorldSpec** (§6): `WorldSpec` serializable class (JSON round-trip) + pure `WorldSpecValidator`
  (actionable errors, registry-aware) + editor `WorldSpecCompiler` → existing assets
  (CityLayoutDefinition/.experience, packs, POIs, sky, jobs). Proof: W002 expressed as a spec compiles
  to what ships today. Gate: `SPEC_DRIFT` WARN.
- **Q2 — Buildings, not boxes** (§4): pure seeded `LotPartitioner` (OBB recursive subdivision, min-area/
  frontage/aspect base cases) + pure `BuildingGrammar` (socketed modules, WFC-lite lowest-entropy
  assembly, doors always street-facing by construction) + `BuildingBuilder` patcher (primitives now,
  Forge kits later). Gates: `BUILDING_DOOR_BLOCKED`, `LOT_OVERLAP`, `BUILDING_OVER_BUDGET`.
- **Q3 — Terrain math v2** (§3): pure `TerrainField` = octaved fBM + domain warping + temperature×moisture
  biome matrix; `WorldExperienceBuilder` swaps its height function (T-Dog's seam, announced). Gate:
  `TERRAIN_SLOPE_UNWALKABLE`.
- **Q4 — Perf architecture** (§7): `GamePool` for hot spawns (bolts/darts/arcs/rings/nets) +
  **`PERF_BUDGET` audit** (tris/materials/renderers/lights vs `QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md`,
  WARN 80% / BLOCK over) + shader-variant WARN (SRP-batcher hygiene).
- **Q5 — Process hardening** (§1–2): LFS-ready (commented) `.gitattributes` stanzas + the V2 laws into
  `CLAUDE.md`/`HOW_TO_CHANGE_ANYTHING.md` + WorldSpec how-to.

## Deliberate deferrals (recorded so nobody re-litigates)
- **Addressables**: trigger = binary assets >500MB or >20 shipped worlds or Quest RAM pressure observed
  in traces. Today's worlds are code-generated primitives — RAM is not the failing constraint.
- **Chunking/LOD streaming**: worlds are bounded bowls by design (240–400m); revisit only if a design
  doc calls for worlds >1km.
- **Full WFC with backtracking**: the building grammar's socket sets are designed contradiction-free
  (lowest-entropy, deterministic tie-break) — simpler, provable, LLM-debuggable. Upgrade path exists if
  module diversity ever demands it.
- **NavMesh**: bots use waypoint/objective logic that already works; NavMesh joins when interiors get
  complex enough to defeat it.
- **Runtime asset streaming (glTFast/GLB + CDN + shader-variant preloading)** *(T-Dog, from Terry's
  PDF §Dynamic Asset Delivery)*: every asset ships IN the APK today (Forge meshes are code-generated,
  primitives fall back), so runtime import solves a problem we don't have. Trigger = a design that
  delivers NEW content to installed devices without a rebuild (live-ops), or asset volume forcing
  Addressables anyway. Until then: no glTFast, no ShaderVariantCollection preloads.
- **Gateway-style model governance (rate limits/routing/payload logging)** *(PDF §Model Governance)*:
  N/A at claude.ai-session scale — OPERATOR_START_HERE's circuit-breaker law (3 CI-reds → stop) and
  per-track model routing (Terry assigns sessions) ARE our equivalents. Revisit only if the pipeline
  ever runs headless on API keys.
- **IK/skeletal creatures**: primitive+Forge bodies first; IK when meshes have limbs worth planting.

## Lane split (zero-collision)
- **Architect** (this program): all NEW files — `Content/Runtime/Spec/**`, pure cores
  (LotPartitioner/BuildingGrammar/TerrainField), `BuildingBuilder`, `GamePool`, new audit rules, docs.
- **T-Dog**: two announced one-call integrations when they resume — `CityBuilder` district pass →
  `BuildingBuilder.Build(...)`; `WorldExperienceBuilder` height fn → `TerrainField`. Plus their open
  P5 (`WORLD_RECIPE.md` — becomes "edit the spec" once Q1 lands) and P4b.
- **Picasso**: Forge module kits for building modules + the real ship hull (their stated top target);
  ART-3 (W001 via Forge) proceeds unchanged.

*Opened 2026-07-03 by the architect on Terry's directive. Board: `docs/SPRINT_ARCHITECTURE.md`.*
