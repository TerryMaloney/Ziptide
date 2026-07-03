# FORGE II — THE QUALITY LEAP (flat "N64" → No Man's Sky class, on demand)

**Status: APPROVED by Terry 2026-07-04 (plan-mode review). This is the art track's plan of record —
sprint ART-3.** Written as task envelopes so ANY operator (Fable/Opus/Sonnet) can execute; the
design decisions are all made — do not re-litigate them, implement them. Board: `docs/SPRINT_ART.md`.

**Why:** Terry's verdict on the first forged asset (taser v2): "N64 level." Correct — Forge v1
outputs flat solid colors: no UVs, no texture maps, no bones, no motion. NMS-class visuals on Quest
(3 = the confirmed perf floor) are mid-poly meshes wearing RICH BAKED TEXTURES + procedural life.
Key insight making this cheap here: Forge ops are parametric, so edges/crevices/orientation are
known analytically — wear/grime/panel detail bakes exactly, with a small software rasterizer, no
sculpting or external tools. The texture baker extends the proven `SkyVistaTexture` pattern.

---

## P1 — UVs + baked texture sets *(FIRST — fixes the flat look on everything)*

**E1.1 UV atlas** — GOAL: `ForgeMesh` gains uv0. Per-part rectangular atlas islands (deterministic
grid allocation weighted by part surface area, 8px gutters at 1024); projection per op family: box
(BeveledBox/Wedge/GreebleStrip — per-face dominant axis), cylindrical (Cylinder/Tube/Lathe —
angle×height), spherical (SphereSection). `mesh.RecalculateTangents()` after (tangents follow our
UVs; normals stay ours). INPUTS: `Visuals/Runtime/Forge/ForgeMesh.cs`. ACCEPTANCE: tests — UVs
inside island bounds, no cross-island overlap, texel density within 2× across parts, determinism.
Visuals unchanged this commit. BUDGET: 1 commit.

**E1.2 Texture rasterizer + albedo styles** — GOAL: `ForgeTexture` (new, Visuals, sibling of
SkyVistaTexture): software-rasterize mesh triangles into atlas space (barycentric fill) writing
per-texel metadata {slot, face-normal world-up factor, distance-to-edge}; then style layers compose
ALBEDO from a per-palette-slot `ForgeStyle` enum (PaintedMetal, BareMetal, RustedMetal, Chitin,
Slime, Stone, Bark, Leaf, GlowPanel) + params (wear, grime, panelDensity, cellSize): base+value
noise → edge wear (bright at edge dist) → grime (down-facing + noise, dark/matte) → panel lines →
scratches → per-style patterns (voronoi chitin cells, fbm bark, gloss slime blotches).
`ForgeRecipeDefinition` gains `ForgeStyleSpec[] slotStyles` (parallel to palette; schemaVersion→2).
ACCEPTANCE: bake determinism tests + booth shows a visibly textured taser (the "not flat"
checkpoint — run the photo loop). BUDGET: 2 commits.

**E1.3 Normal/MSA/emissive maps + one material per asset** — GOAL: height field accumulated per
layer → Sobel → tangent-space normal map; `_MetallicGlossMap` (R=metallic, A=smoothness);
`_EmissionMap` for GlowPanel slots (keyword `_EMISSION`). Submeshes collapse to ONE textured URP/Lit
material per asset (keywords `_NORMALMAP`,`_METALLICGLOSSMAP`). ACCEPTANCE: map-content tests
(normal map neutral where no detail, emissive only on glow slots) + booth iteration. BUDGET: 1–2
commits.

**E1.4 Bake-to-asset pipeline** — GOAL: `ForgeBaker` (editor, build-hooked next to ForgeAuthor):
per recipe → Mesh asset + 4 PNGs + Material + Prefab under `Assets/Ziptide/Resources/ForgeBaked/
<id>/`; **gitignored** (add entry) but generated in every build workspace → Unity imports PNGs with
ASTC + mips (uncompressed runtime 1024² sets would be 12–16MB/asset — this is the fix).
TextureImporter per map (normal type; linear for MSA). `ForgeVisualApplier` prefers the baked
prefab; runtime albedo-only build stays as dev fallback (skip runtime normal maps — encoding
mismatch). **PhotoBooth renders the baked path** (bake inside RenderAllBatch first) so critique
matches shipping. ACCEPTANCE: forge-photos artifact shows the baked taser passing the quality
rubric (below); Terry checkpoint with photos. BUDGET: 2 commits.

## P2 — Geometry richness
GOAL: new ops **Capsule, Frustum** (cone at topRadius 0), **Torus, SweepSpline** (tube along 2–4-pt
bezier, per-point radius — tentacles/pipes/branches), **OrganicBlob** (sphere + fBm displacement);
per-part modifiers on any op: `noiseAmplitude/Frequency/Seed` (along normals, pre-transform),
`taper`, `bendDegrees`. No subdivision pass (segments ≤24 suffice on Quest 3). Budgets by storyTag
class: handheld 6k · creature 10k · hull 15k · prop 3k · plant 1.5k (update `Validate()` + audit).
ACCEPTANCE: per-op geometry/outwardness/determinism tests (copy ForgeMeshTests patterns). BUDGET:
2 commits.

## P3 — Skinned creature bodies
GOAL: `ForgeCreatureBody` SO (torso/head parts + `ForgeLimb[]` chains {attach, segment templates,
joint axis, gait role Leg/Tentacle/Wing/Tail/Antenna} + skin styles + emissive eyes + motion
params) + `ForgeSkinnedBuilder` (bone hierarchy ≤12 bones, ONE rigid-weighted SkinnedMeshRenderer =
1 draw call). ⚠ ONE coordination commit (HANDOFF-announced, mirrors the shipped ItemFactory
pattern): `CreatureDefinition.forgeBodyId` append + ~6-line guarded early-out in
`CreatureBehaviorBase.BuildVisuals` → `ForgeBodyApplier.TryApply` (art lane; skinned renderer FIRST
child so DroneRuntime's single-renderer tint still works; missing body = primitive fallback).
First bodies: patrol drone (plated, rotor ring, emissive eye) + W002 cave swarmer (6-leg chitin).
ACCEPTANCE: skin-weight/bindpose/determinism tests + photo-loop iteration on both bodies. BUDGET:
3 commits.

## P4 — Procedural motion
GOAL: `ForgeMotor` (Visuals MonoBehaviour): passively observes its own transform velocity
(LateUpdate deltas, smoothed) — ZERO behavior-code coupling. Drives bones by gait role: legs =
phase-offset hip sines + counter-phase knees + |sin| lift (no IK); tentacle/tail propagating waves;
breathing pulse; head look-at velocity; idle sway; **stun droop** triggered by observing the root
scale-crumple `CreatureRuntime.Disable` already applies. Params in `ForgeCreatureBody`
(skitter/stride/hover/slither, freq, amp). Booth gains a mid-gait **pose shot**. ACCEPTANCE:
deterministic pose math tests + drone/swarmer look alive in pose shots. BUDGET: 2 commits.

## P5 — Building modules, flora, props *(the qqq envelopes fold in here)*

**E5.1 Building-module family (envelope from architect, PRIORITIES #4):** fulfill
`buildingModule:<styleId>/<Module>` ids (WallSolid/WallWindow/Doorway/CornerTrim/RoofFlat/RoofRaked)
for `salvage_row` + `toxic_tenement` per `design/ART_REGISTRY.md`, as Forge recipes with P1
textures (RustedMetal/PaintedMetal + GlowPanel windows), registered via `ArtModuleRegistry.Register`
from an editor author in the art lane. ACCEPTANCE: turnarounds pass the rubric + an APK where
W002's warren wears the kit (pairs with story-track Q2d). BUDGET: 2–3 commits.

**E5.2 PERF_BUDGET gate (envelope, PRIORITIES #5):** per-scene tris/materials/renderers/lights vs
`QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md` — WARN at 80%, BLOCK over, exempt `_Boot`;
`ExperienceAuditRules` is the pattern; one announced WorldAuditRunner call line. BUDGET: 1 commit.
*(Can be done any time — no dependency on P1–P4.)*

**E5.3 Flora + props:** LeafCard part type (static textured quad pairs, Leaf style albedo+alpha
cutout, ≤2 overdraw layers) + `ForgeSway` (1–3 branch pivots) + 2 plants (W005 frond, W001 canal
reed) + 3 props (patched crate, pipe cluster, dispatch console). Scatter placement rides H5
`ScatterField` ids (`scatterKind:<id>` per the registry). BUDGET: 2 commits.

## The quality rubric (goes in FORGE_STUDIO_GUIDE v2; check EVERY photo iteration)
Silhouette interesting at 3 distances · visible edge wear · one emissive focal point · grime
grounding · material contrast (≥2 styles) · story/family read. An asset that fails the rubric
doesn't get shown to Terry — iterate first.

## Risks (accepted, mitigations chosen)
Rasterizer bake time (seconds, build-time only) · RecalculateTangents (deterministic per mesh) ·
ASTC via TextureImporter (standard) · SkinnedMeshRenderer ≤12 rigid bones (trivial on Quest 3) ·
leaf-card overdraw (budgeted + gated) · DroneRuntime first-renderer tint (applier guarantees order)
· schema creep (closed enums + schemaVersion 2).

*Authored by Picasso (Fable 5) 2026-07-04 from the approved plan; envelopes E5.1/E5.2 accepted from
architect's qqq briefing.*
