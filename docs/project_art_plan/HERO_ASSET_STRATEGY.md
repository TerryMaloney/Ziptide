# HERO ASSET STRATEGY — the honest ceiling of the Forge, and the three-tier path to AAA
### Terry's verdict (2026-07-20): "the ship… the space battle stuff… I can't see that coming out very good as is." He's right. Here's the plan.

**Status:** 🔵 RESEARCH + STRATEGY — planning only, zero code (freeze). This doc RECORDS that
`design/ART_REGISTRY.md` §5's hero-asset deferral trigger has now FIRED (the owner's verdict
is the trigger). Companions: `FORGE_VI_WORLD_COMPILER.md` WC-5 (the intake this feeds),
`ASSET_FORGE_MAP.md` (the registry laws that keep imports safe).

---

## §1 — The honest ceiling (what the Forge is, and what it can never economically be)

The Forge composes primitives through ops and bakes procedural textures. At prop/creature/
module scale this is genuinely strong — the taser "reads as a salvage stun pistol," the
creature roster shipped, the walls read. **But the evidence of the ceiling is already in our
own logs:** the warden took FIVE versions to stop being "a dark chess pawn… a bin with a lid."
That wasn't operator failure — that was silhouette design being done expensively, blind, in
3D, through op parameters. The ceiling has three faces:

1. **Designed silhouettes.** AAA hero assets start as CONCEPT ART — an artist draws intent,
   iterates in 2D where iteration is nearly free, and the model serves the drawing. Op-stacks
   explore silhouette space by parameter twiddling: slow, unguided, and capped at "good
   composition of simple forms."
2. **Detail density.** The AAA read at hero scale is panel lines, greebles, chamfered edges,
   bolt patterns, curvature-driven wear — THOUSANDS of intentional small decisions. Our ops
   have no vocabulary for them, and VR makes it worse: the ship and held weapons get
   face-distance inspection, the harshest quality test in games.
3. **Edge quality.** Procedural primitives have razor edges; real objects have bevels. Nothing
   says "programmer art" louder than a perfectly sharp 90° edge catching a specular highlight
   at 30 cm.

**Conclusion, stated plainly: the Forge was never going to make the hero ship AAA, and the
architecture always knew** — stable ids + the `ForgeVisualApplier` contract (root stable,
visual child replaceable) + ART_REGISTRY §5's "hero assets from OUTSIDE the repo" trigger
exist precisely so hero meshes can arrive from elsewhere without touching gameplay.

## §2 — THE THREE TIERS (every asset declares one; the tier picks the pipeline)

### Tier A — FORGE-NATIVE (the bulk: props · flora · building modules · practicals · most creatures)
**Verdict: already on the AAA path — change nothing.** The ceiling faces don't bite at this
scale/band; FORGE II delivered the surfaces, FORGE III the cohesion, and FORGE IV's CP-2
(LOD packages) + CP-3 (material intelligence/wear masks) finish the job. The Forge remains
the ONLY tier for anything needed in quantity — that's its whole economic point.

### Tier B — FORGE-PLUS (held weapons · hero creatures · mid landmarks — "okay but not great," closable)
The gap for guns/melee is exactly the three ceiling faces at small scale, and at THIS scale
they're cheap to fix inside the Forge:
- **New hard-surface ops:** `Bevel/Chamfer` (kill the razor edges — the single biggest
  read-upgrade available), `PanelInset` (recessed panel lines with baked AO in the seams),
  `GreebleScatter` (budgeted small-detail placement on declared faces), bolt/seam stamps in
  the texture bake.
- **The face-distance texel law:** held items get the project's HIGHEST texture density
  (they're 30 cm from the eye; a distant building can share an atlas — the gun cannot).
- **The material-split law:** a weapon reads AAA when 2–3 materials read distinctly (worn
  metal + grippy polymer + one accent/emissive) — formalize as a `Validate()` check on
  weapon-class recipes.
- **Touch-point wear:** CP-3 masks applied where hands actually grip (the applier knows the
  grip transform — derive the wear from it; nobody hand-paints).
**Home:** an envelope in the FORGE III/IV art window; booth turnarounds judge per the
existing loop. This tier stays fully procedural — determinism, hashes, CI, all preserved.

### Tier C — HERO (THE ship, exterior + interior · close-range capital ships · signature world landmarks)
**Verdict: the Forge alone does not reach AAA here; the external path is now open.** Pipeline,
in order:
1. **CONCEPT FIRST — the 2D law.** Every hero asset starts as art-directed concept images
   (image generation is fast and cheap; Terry picks silhouettes in 2D where a rejected
   version costs seconds). *The warden's five 3D rounds are the cautionary tale — silhouette
   approval belongs in 2D.* The chosen concept sheet becomes the asset's canonical reference,
   logged with the recipe.
2. **GEOMETRY — three routes, rankable per asset:**
   - **AI 3D generation (state of the art is now genuinely usable):** Tripo/Meshy-class
     image-to-3D from the approved concept — 2026 state: PBR map output, quad-dominant
     topology options, Unity-ready export, and **commercial/private licensing on paid tiers**
     (free tiers are CC-BY/attribution — same trap as Suno; generate keepers on a PAID plan,
     verify terms at purchase, CREDITS.md line per asset). Expect a CLEANUP pass (retopo
     spots, scale, pivot, material slots) — the tools are a starting point, not a finish.
   - **Marketplace kitbash:** modular sci-fi ship kits + greeble packs (proven indie path;
     licensing via CREDITS; style-conformance pass through our materials so it doesn't read
     store-bought).
   - **Commission:** ONE freelance hard-surface artist for THE ship (exterior + interior
     shell) is plausibly the single highest-leverage art dollar in the project — the ship is
     the most-stared-at object in the game (it's HOME). ⚖ Terry: budget decision.
3. **INTAKE — WC-5's gates, now activated:** budget validation (hero class may need its own
   reviewed cap — the 15k hull ceiling was written for Forge hulls; a hero ship needs a
   device-evidenced hero budget + LOD ladder) · material conformance (URP/Lit, our families —
   imported assets get re-materialed to OUR look, which is what keeps a kitbashed/AI mesh
   from breaking cohesion) · provenance + license record · booth turnarounds + Terry verdict
   like any asset. IDs never change; the applier swaps the visual child; gameplay never knows.
4. **Space battles specifically:** distant ships are P3-band — silhouette + motion + VFX
   (our strengths) carry them; only ships the player closes with need Tier-C treatment. One
   hero enemy capital + Forge-native distant fleet is the honest budget.

**Long-term Tier-C upgrade (recorded, not scheduled):** headless **Blender as a Forge
backend** — scripted (LLM-writable Python) bevel/boolean/curvature-bake passes in CI would
raise the procedural ceiling toward Tier-B++ while keeping determinism. A real option for the
Slipway era; not before the assembly line proves.

## §3 — Laws this strategy adds (small, sharp)

1. **Every asset declares its tier** (recipe field, default A); tier picks pipeline and budget
   class; audits know.
2. **The 2D law:** no Tier-C geometry before an approved concept sheet. Silhouettes are
   iterated where iteration is cheap.
3. **The license law extends:** external/AI geometry follows the music precedent — paid-tier
   generation only for keepers, terms verified at purchase date, CREDITS.md or it doesn't
   import.
4. **Conformance is non-negotiable:** imported meshes are re-materialed into our families and
   pass the same booth + audits — Tier C changes where geometry COMES FROM, never what the
   game LOOKS like.
5. **The Forge stays the default:** Tier C is for the short named list (ship, capitals,
   signature landmarks, ~one hero element per world per WC-4). If the list grows past ~a
   dozen, we've misclassified — re-read §2-A.

## §4 — Sequencing (freeze-compatible starts marked ●)

- ● **Ship concept phase** — image-gen concept sheets for THE ship (ext/int), Terry picks.
  Zero code, start anytime. The single best art hour available during the freeze.
- ● **⚖ Terry decisions:** commission budget yes/no · AI-gen tool + paid tier pick (verify
  license that day) · the hero-class budget review trigger.
- **Tier-B hard-surface ops envelope** — FORGE III/IV art window, post-freeze (booth-judged).
- **WC-5 intake build** — on first real import (the trigger HAS fired; build when the first
  Tier-C mesh is actually in hand, not speculatively).
- **Hero ship integration** — post-checkpoint, through intake + applier swap + device verdict.
