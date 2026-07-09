# VERTICAL & CAVERN WORLDS — elevation, caves, traversal (design doc)

> **Hardwiring §3 / Phase 1.** Environments that rise above the ground, go underground, and stack —
> plus the VR traversal to reach them. Status: skeleton for Fable 5.

## Current state (code survey)
`TerrainField` is a 2D height-field (fBM + domain warp). **A heightmap cannot make overhangs, caves,
or tunnels** — so every world is single-level ground today. No cavern/elevated/underground system.

## Locked decision
**Modular kits, Quest-safe. No voxel/mesh terrain** (reserve only for a rare hero set-piece if ever
justified). Hold 72–90 fps at scale.

## What to build
- [ ] **Cavern kit** — tunnel / chamber / junction / vertical-shaft / stalactite modules in
  `ArtModuleRegistry`; a `CaveBuilder` that strings them into networks; portal culling.
- [ ] **Underground layer** — caves as content-only world scenes (or an additive sub-layer) reached by
  travel; honor the boot/rig/spawn contract.
- [ ] **Elevated / mesa layer** — platforms, cliffs, floating islands above the heightfield; a
  `WorldSpec` elevation-layer field; `ScatterField` places vertical props.
- [ ] **VR-native traversal (make ziplines a signature — the game is *Ziptide*):**
  - [x] **Ziplines** — PURE ride kinematics shipped (`ZiplineCore.ZiplineRide`, 2026-07-09): along-cable
    gravity + drag + push-off kick + **hard comfort speed cap**, EditMode-tested. ✅ SCENE TRANSLATOR
    SHIPPED (hardwiring 1.4a, same day): `Gameplay/ZiplineRuntime` — self-built sagging cable visual +
    posts + XRI grab handle; rides `ZiplineRide` and **delta-translates the rig** (never parented), so
    the ComfortVignette engages from rig motion (no Speed01 feed needed — that path is for
    world-moves-around-you frames). *(A duplicate `Content.City.ZiplineCore` was built in parallel and
    reconciled INTO this core — deleted, sag kept as a runtime visual helper.)* TODO: branching lines +
    world placement (POI-pair stringing).
  - [x] **Climbable surfaces** — PURE hand-over-hand math shipped (`ClimbCore.ClimbGrip`, 2026-07-09):
    rig moves −handDelta, two-hand handoff with no teleport, comfort-clamped release fling, tested.
    TODO: the scene translator (XRI grab on a `Climbable` collider → apply the returned rig delta to
    the XR Origin; never parent the rig — SHIPS.md law).
  - [ ] **Elevators / lifts**, **jump-pads**, **grapple** (pure cores next, same pattern).
- [x] **Multi-level reachability audit** — PURE core shipped (`MultiLevelReachability`, 2026-07-09):
  composes N walkable grid layers (each with GridReachability's 4-connectivity + maxStep rule) with
  explicit cross-layer `TraversalEdge`s (step/zip/climb/elevator/grapple/jump, one-way or bidirectional)
  and floods the union — so a stacked/underground POI is provably reachable or flagged stranded. 6
  EditMode tests. TODO: the editor audit rule that samples a built vertical world into layers + reads
  its authored traversal links and WARNs on stranded POIs (mirrors `WorldReachabilityAuditRules`).

## Consistency-spine hooks
Cavern/vertical kits are Forge-baked + registry-driven + budgeted like buildings; caves/space/ground
are all world scenes via `TravelCoordinator`; traversal uses the shared comfort layer (climb/zip comfort).

## Sourced technique (2026-07-09 — full citation set in `VR_TECHNIQUE_RESEARCH.md`)
Backbone reference: **`docs/design/VR_TECHNIQUE_RESEARCH.md`** (the shared cited harness output). The
traversal-specific findings that shaped the cores above:
- **Why a heightmap can't overhang (the reason this whole doc exists):** a height-field stores ONE
  height per (x,z) cell, so it is a function z=f(x,y) and physically cannot represent two surfaces
  stacked over the same ground point — no caves, no overhangs, no tunnels. Overhanging geometry needs
  meshes/voxels/modular kits, not terrain. *(heightmap-vs-voxel primer:
  https://terrain.chriskempke.com/heightmaps_and_voxels/ — 📎 sourced, single-pass.)* This is exactly
  why `MultiLevelReachability` models the world as stacked layers + explicit vertical edges rather than
  a single grid.
- **VR locomotion comfort is the hard constraint on ride/climb speed**, not a tuning preference —
  Meta's own locomotion-comfort guidance drives the `ZiplineRide` **hard speed cap** and the
  `ClimbGrip` **release-fling clamp** (fast, uncapped self-motion is the primary nausea source).
  *(Meta locomotion comfort: https://developers.meta.com/horizon/design/locomotion-comfort-usability/
  — 📎 sourced.)* The scene translators must feed ride speed to `ComfortCore` for the vignette.
- **Portal culling for caves on mobile VR** (for the CaveBuilder TODO): Unity's per-frame occlusion
  costs ~1 ms CPU on Quest, so first-party guidance favors a precomputed potentially-visible-set (PVS)
  keyed to cells over runtime recompute — relevant when the cavern kit lands. *(Meta occlusion-for-
  mobile-VR blog, VERIFIED claim in the harness; see `VR_TECHNIQUE_RESEARCH.md`.)*
- Modular cave-kit conventions (fixed snapping grid, shared trim-sheet atlas → one draw material) match
  the architect's `BuildingKitLibrary` pattern — the CavernKitLibrary will mirror it (different ids).

> 📎 = sourced-but-single-pass (harness verify+synthesis was cut by a session limit); re-run the harness
> after the reset to promote to verified. The two cores above depend on the comfort finding, which is
> the best-supported of the set.

## Tests + audit
CaveBuilder deterministic per seed; multi-level reachability green; portal culling verified; budget caps.

## 🚀 Room to expand
Underground biomes (crystal caverns, flooded tunnels — tide!), vertical cities with sky-bridges,
zipline *networks* as fast-travel, climbable landmark towers with vista payoffs (ties to
`SKYSCAPE_DESIGN.md`), hidden cave secrets/loot. Verticality is a differentiator — lean in.
