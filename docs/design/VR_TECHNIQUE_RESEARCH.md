# VR TECHNIQUE RESEARCH — sourced backbone for the hardwiring design docs

> **What this is.** The cited "how studios actually do it" backbone that the hardwiring design docs'
> **"Technique research TODO"** sections point to. Produced by the deep-research harness (fan-out web
> search → source fetch → adversarial verify). **The run completed scope + search + fetch and got
> partway through verify before a session limit killed the verify+synthesis phases** (29 sources, 34
> claims extracted; 3 fully verified, the rest sourced-but-unverified). So confidence is labelled per
> claim — **spot-confirm any 📎 number against its source URL before relying on it in code.**
>
> Confidence: **✅ verified** (adversarial 2–3 vote) · **📎 sourced** (direct quote from a primary/
> practitioner source; verification cut short) · **🔎 lead** (relevant source found, claims not yet
> extracted). Re-running verify+synthesis after the limit reset (9 pm UTC) will upgrade the 📎 items.

---

## 1 · Modular kit-bashing at scale  → `WORLD_BUILDING_AT_SCALE.md`
- 📎 **Modular pieces must snap with no gaps/overlaps and be reusable/swappable**, so a small set
  composes walls→floors→rooms→whole buildings. *(worldofleveldesign.com)*
- 📎 **Standardize sizes/rules very early, all pieces grid-aligned**, to guarantee interchangeability.
  *(worldofleveldesign.com)*
- 📎 **Correct pivot-point placement is essential for gap-free snapping; test modularity constantly.**
  *(worldofleveldesign.com)*
- 🔎 Leads (not yet extracted): Polycount *Modular environments* wiki; Level Design Book *env-art*
  process; Intel *Modularity for Games* (PDF); 80.lv *advanced trim-sheet textures*; Lugher3D
  *modular environment workflow*.
- **Recommended default (Quest URP):** one grid module (e.g. 1m/2m), one trim-sheet atlas + one
  material per kit family, pivots at grid corners, snap-verified in a test scene. Maps directly onto
  extending `BuildingStyle` → Forge-baked kit registered in `ArtModuleRegistry`.

## 2 · Building interiors + occlusion for mobile VR  → `BUILDING_INTERIORS.md`
- ✅ **Interior mapping is a pure tangent-space shader on flat window geometry — no room geometry
  built** — creating the illusion of lit rooms behind windows. *(github.com/Gaxil/Unity-InteriorMapping, 3-0)*
- 📎 It represents rooms with **atlas textures** (a 4×4 interior atlas + window atlases), keeping
  texture memory bounded. *(Gaxil)*
- 📎 Supports **window refraction, light projection, room-size customization, and shadow casting** from
  external occluders. *(Gaxil)*
- ✅ For fixed-camera mobile VR (Republique), **precompute a Potentially Visible Set (PVS) per camera
  location** instead of per-frame occlusion. *(developers.meta.com occlusion-culling blog, 3-0)*
- ✅ **The PVS is built on the GPU**: give each renderer a unique solid-color material, render, and read
  back which colors appear to know what's visible from that point. *(Meta, 2-0)*
- 📎 Standard per-frame frustum culling was a **significant CPU cost** on mobile VR, motivating the
  precompute. *(Meta)* · 📎 A PVS = a per-location lookup of which renderers to enable. *(Meta)*
- 📎 *(number — confirm)* The precomputed cull **reduced rendered tris/verts by up to ~80%.** *(Meta)*
- 🔎 Leads: 80.lv *fake interiors behind flat surfaces*; halisavakis *interior mapping*;
  thegamedev.guru *occlusion culling tutorial*; Unity Asset Store *Fake Interiors* free.
- **Recommended default:** interior-mapping windows on **all** buildings (bounded atlas, zero geometry);
  walkable interiors only on flagged "enterable" buildings with **portal culling** (Ziptide's interiors
  are mostly room-static → a precomputed/portal PVS fits better than runtime queries).

## 3 · Verticality, caverns, overhangs, multi-level  → `VERTICAL_AND_CAVERN_WORLDS.md`
- 🔎 **Heightmaps cannot represent multiple ground surfaces at one point** (caves, bridges, tunnels,
  overhangs) — the core reason `TerrainField` alone can't do caverns. *(ckempke *Heightmaps or Voxels*)*
- 🔎 Leads: pinwheelstud *why low-poly mesh terrain fits mobile/VR*; transvoxel.org (Transvoxel for
  seamless voxel LOD); ngildea *dual-contouring chunked terrain*. *(Claims not yet extracted — these
  confirm voxel is powerful but LOD/seam-heavy; mesh/modular is the lighter path — verify before quoting.)*
- **Recommended default (matches Terry's locked call):** modular cave/tunnel/shaft + mesa/platform
  kits placed like buildings (Quest-safe), **not** voxel terrain at scale; reserve voxel/mesh-cave for
  rare hero set-pieces only.

## 4 · Procedural + handcrafted hybrid (PCG / WFC)  → `WORLD_BUILDING_AT_SCALE.md`
- 📎 **WFC output is locally similar to a sample** — every N×N region appears in the input, frequencies
  approximate it — giving authored control over procedural output. *(github.com/mxgmn/WaveFunctionCollapse)*
- 📎 **WFC = observation–propagation loop:** collapse the lowest-Shannon-entropy cell, propagate
  constraints outward; it's a **constraint-satisfaction** method using AC-4 arc-consistency. *(mxgmn)*
- 📎 **Two models:** *overlapping* (extract N×N patterns from an image) and *simple tiled* (predefined
  tiles + explicit adjacency) — sample-driven vs hand-authored constraints. *(mxgmn)*
- 📎 **You can pin/fix specific tiles before generation** and the solver integrates them — authored
  control over procedural output. *(boristhebrave WFC tips)*
- 📎 **WFC can fail** (a cell's options all go to zero = contradiction), though rarely. *(mxgmn)*
- 🔎 Leads: GDC Vault *Continuous World Generation in No Man's Sky* (deterministic seed-driven,
  voxel→polygonize→texture pipeline); SideFX *Houdini GDC 2019* rule/spline PCG talks.
- **Recommended default:** tiled-model WFC (or the existing grammar) for building/street/room assembly,
  with **pinned POI tiles** for authored control and a **deterministic seed** per world (Ziptide already
  seeds worlds — keep it reproducible).

## 5 · Quest performance budgets & rendering  → `WORLD_BUILDING_AT_SCALE.md` + PerfBudget gate
- 📎 **~100 draw calls/frame is a warning threshold; pipeline state changes (shader/texture/mesh swaps)
  drive cost** — share materials and instance meshes. *(developers.meta.com draw-call analysis)*
- 📎 **Single-pass instanced rendering cuts the CPU draw-call burden**, but per-call cost still depends
  on state changes from the previous call. *(Meta)*
- 📎 **Shader complexity is the single largest GPU-time consumer** — logical ops (PBR vs simple diffuse)
  cost more than extra texture sampling. *(Meta)* → favor simple/mobile shaders (fits the Forge's one
  URP/Lit-ish material philosophy).
- 📎 **Alpha blending is expensive on Quest, and more costly in Linear than sRGB** color space. *(Meta)*
  → avoid transparency stacks; prefer cutout/opaque.
- 📎 Changing texture/size/filter/compression between draws has **minimal** extra cost beyond the
  material change itself. *(Meta)* → atlasing wins come from fewer material swaps, not texture size.
- 🔎 Leads: Meta *Unity performance* + Oculus *mobile performance intro* (batching, GPU instancing,
  baked lighting, LOD, fixed-foveated, streaming — verify exact numbers).
- **Recommended default:** stay under ~100 draw calls/frame; one material per kit family; single-pass
  instanced; simple shaders; opaque over alpha; static batching + GPU instancing for props; bake
  lighting; extend `PerfBudgetAuditRules` caps per new content type.

## 6 · VR 6DOF comfort + vehicle/ship customization  → `SPACEFLIGHT_PHYSICS.md`, `SHIP_FORGE_AND_CUSTOMIZATION.md`, `DRIVABLE_VEHICLES.md`
- 📎 **Peripheral-vision fade (tunnelling) reduces sim-sickness** — the subconscious leans on peripheral
  motion detection while the conscious brain ignores the vignette. *(github.com/sigtrapgames/VrTunnellingPro-Unity)*
- 📎 **Replace the periphery with static imagery** (grids, cages, skybox/cubemap, static windows,
  world-space portals) for a **stable reference frame** that counteracts motion more strongly than a
  plain vignette. *(VRTP)* — this is the cockpit-reference-frame idea, generalized.
- 📎 **VRTP can mask/exclude a static cockpit from the tunnelling effect**, and supports counter-rotation,
  counter-motion, and stepped motion-compensation. *(VRTP)* → the cockpit stays crisp while the world
  tunnels — exactly the flight-comfort model in `SPACEFLIGHT_PHYSICS.md`.
- 🔎 Leads: Meta *Locomotion Comfort & Usability* (snap vs smooth turn, reduce motion — authoritative
  Quest guidance); Unity XRI *Tunneling Vignette Controller* (built-in vignette we can drive);
  Star Citizen *Ship components* + *Ship paint* (modular **functional** components separate from a
  **cosmetic paint** layer — validates the loadout-vs-wrap split in our cosmetic layer).
- **Recommended default:** static cockpit reference frame + dynamic comfort vignette on accel/roll
  (drive Unity XRI's tunneling vignette or port VRTP's approach); analog 6DOF mapped like a gamepad;
  functional modules and cosmetic wraps as **separate** data layers (looks-never-stats).

---

## Full source list (29 fetched; quality as judged by the extractor)
**Primary / practitioner (claims extracted):**
- github.com/Gaxil/Unity-InteriorMapping — interior mapping shader (4 claims)
- developers.meta.com/horizon/blog/occlusion-culling-for-mobile-vr-developing-a-custom-solution/ (5)
- developers.meta.com/horizon/documentation/unity/po-draw-call-analysis/ (5)
- github.com/mxgmn/WaveFunctionCollapse (5)
- boristhebrave.com/2020/02/08/wave-function-collapse-tips-and-tricks/ (5)
- github.com/sigtrapgames/VrTunnellingPro-Unity (5)
- worldofleveldesign.com … modular-environment-design-101 (5)

**Leads (relevant, claims not yet extracted):** wiki.polycount.com Modular_environments ·
book.leveldesignbook.com/process/env-art · intel.com … modularityforgames.pdf · 80.lv trim-sheet +
fake-interiors articles · halisavakis interior mapping · thegamedev.guru occlusion tutorial ·
assetstore.unity.com Fake Interiors free · developers.meta.com unity-perf + locomotion-comfort ·
developer.oculus.com unity-mobile-performance-intro · gdcvault.com No Man's Sky world generation ·
sidefx.com GDC-2019 Houdini · docs.unity3d.com XRI tunneling-vignette-controller · starcitizen.tools
Ship_components + Ship_paint · ckempke UnityTerrainGeneration heightmaps_and_voxels ·
pinwheelstud.io low-poly-mesh-terrain · transvoxel.org · ngildea dual-contouring-chunked-terrain

## Status / next
Verify + synthesis were cut off by the session limit (resets 9 pm UTC). To finish: re-run the harness'
verify+synthesis pass to upgrade 📎→✅ and extract the 🔎 leads (terrain/voxel, NMS PCG, Star Citizen
customization, Meta comfort numbers). Until then, treat 📎 claims as well-sourced-but-unconfirmed and
check the URL before hard-coding any number.
