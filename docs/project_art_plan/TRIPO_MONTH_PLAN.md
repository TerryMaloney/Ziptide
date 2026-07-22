# THE TRIPO MONTH — generation day plan + the 30-prompt priority list
### Terry commissioned 2026-07-21: one paid month, 50–300 generations, "spend the rest of today generating our top-priority 3D stuff." No code changes — clean APK stands.

**Status:** 🔵 ACTIVE GENERATION PHASE — freeze-compatible by construction: generated meshes
land in a holding area OUTSIDE the Unity project (`art_intake/` at repo root, or Terry's
drive), so the build is untouched until post-checkpoint intake. Companions:
`HERO_ASSET_STRATEGY.md` (tiers + license law), `CONCEPT_TO_BUILT_PIPELINE.md` (the route
these meshes enter through), `TIER_C_CONCEPT_QUEUE.md` (the approved sheets = the inputs).

---

## §0 — DAY-ONE LICENSE CHECKLIST (do this BEFORE the first generation — the Suno lesson)

1. Buy the PAID plan (any paid tier grants commercial use; free tier is CC-BY — never ship it).
2. **Screenshot the license/terms page + your plan level + today's date.** Save to
   `art_intake/_LICENSE_RECORD/`. Rights attach at creation time; this is the proof.
3. Confirm the terms say outputs remain licensed to you after the subscription ends
   (perpetual license to generated outputs). If unclear, screenshot the FAQ answer too.
4. Every KEPT asset gets a `CREDITS.md` line at intake: asset id · tool + plan · date ·
   source concept image. No line, no import — the standing law.

## §1 — WHERE FILES LAND (zero build impact)

- `art_intake/<asset_id>/` at repo root — NOT under `Ziptide/Assets/`, so Unity and the APK
  never see it. Keep only the KEEPER per asset (best GLB + its texture set), not every
  variant — meshes are big; the repo carries winners, Terry's drive carries the rest.
- Per asset drop a short `MANIFEST.md`: source concept file(s) used · generation settings ·
  which variant won and why · known cleanup needs (scale/pivot/topology notes).
- Naming: `<asset_id>_gen_v<N>.glb` matching the concept folder ids
  (`ship_scavenger_mk1`, `rill_drone`, `artifact_key`, …).

## §2 — THE PLUG-IN PLAN (how each mesh actually WORKS in game — post-checkpoint wiring)

The architecture was built for this: **gameplay never references meshes** — items resolve by
string ID through `ItemFactory`; visuals arrive through the `ForgeVisualApplier` contract
(root stays stable and owns colliders/grab points; the visual child is swappable). So
"plugging in" is always the same move: the generated mesh (after cleanup) becomes the visual
child; the gameplay object doesn't change. Per asset class:

- **THE SHIP:** root keeps colliders, seats, coupler socket, boarding door triggers. The
  Tripo hull = visual child (later the B+ Forge hull becomes the distant LOD).
  **Generate the GRABBER ARM as a SEPARATE asset** — it articulates, so it needs its own
  pivoted segments; a fused arm on the hull would be decorative only.
- **RILL:** the orb mesh replaces the current orb visual under the RILL runtime. The
  `RillState` driver (to be built) lerps eye-iris + channel emissive per state. Honest note:
  Tripo's emissive map won't isolate her channels the way the mood row needs — plan on ONE
  re-bake pass of the emissive mask on our side. The mesh is still the win (shell, panel,
  lens geometry).
- **ARTIFACT KEY:** two halves = two item visuals under existing item IDs; the join
  ceremony/proximity gameplay is untouched. Generate each half separately so the interlock
  faces are real geometry.
- **WEAPONS (taser, gravity gun, melee):** visual swap via `forgeRecipeId`/applier; grip
  transforms and XR attach points stay ours. Face-distance texel law applies — these get the
  highest-res textures in the game.
- **CAL:** use Tripo's auto-rig on the humanoid output. Per the pending ⚖ (route 1), the
  body is used for mirror beats/title menu, not full third-person — minimal animation need.
- **LANDMARKS/DRESSING (tower, booth, crane, wrecks, props):** placed as world content
  through recipes/registry like any Forge asset; distant impostors stay Forge.
- **EVERY asset gets the cleanup pass at intake (WC-5):** scale to real meters · pivot
  placed (base for props, grip for held) · tri budget check vs. band (decimate if needed) ·
  materials converted to URP/Lit and RE-MATERIALED into our families (this is what keeps an
  AI mesh from breaking cohesion) · LOD ladder · booth turnaround + your verdict vs. the
  concept sheet. All of that is post-checkpoint code/editor work; today is generation only.

## §2b — AUTHOR FOR VARIANCE (Terry's "500 Cals / reskin guns on a whim" — the build-once rule)

**Full method: `SKIN_AND_VARIANT_SYSTEM.md`.** The short version that binds THIS month: you build
each character + hero weapon + the MK2 ship + rooms as a mesh **ONCE**, and all the color/design
variety comes from a cheap SKIN layer afterward — NEVER regenerate geometry for a variant. So when
generating/cleaning a skinnable asset, set it up for skins up front:
- **one clean UV layout · SEPARATED material zones** (Cal = suit/gloves/straps/visor/accents; gun =
  body/grip/barrel/emissive — so a tint can hit just one region) · a **FIXED-zone list** (identity
  anchors never reskinned: bird trinket, red laces, tally guard; the ship's grafted salvage) ·
  **attach points** for modular swap-parts · a neutral mid-grey base texture.
- Variants then = `SkinDefinition`s (parametric tints = free/infinite · authored skin textures =
  minutes each · modular parts = combinatorial). **Don't spend Tripo credits on variants** — one
  clean base per asset; the skin system makes the 500. Quarters/ship/room skins use the same system.

## §3 — GENERATION WORKFLOW (get the most out of every credit)

- **Image-to-3D is the primary mode** — our approved concepts ARE the prompts. Feed ONE
  object per image: **CROP the keeper** to just the asset (cut Terry-excluded noise — the
  glove on RILL's sheet, scenery, other objects, text labels).
- **Use multi-view input where you have it** (paid tiers accept front/side/back): split the
  ship and skiff ortho sheets into separate single-view crops — dramatically better hulls.
- 3/4-view single images work fine for everything else.
- Settings: PBR ON · quad topology option if offered · highest texture size offered ·
  for Cal pick the character/auto-rig path.
- **Variant discipline (the booth rubric applies):** generate 3–5 per asset, judge by
  SILHOUETTE first (fill-it-black), keep ONE winner, log it in the manifest. Don't polish a
  bad silhouette with re-rolls — recrop or reword instead.
- Text prompts below are for assets WITHOUT approved sheets. Best practice: run the text
  through nano banana FIRST (free) to get a 2D concept you approve, then image-to-3D — the
  2D law, and it saves paid credits. Direct text-to-3D is acceptable for simple props.
- Text-prompt grammar for 3D: describe the OBJECT ONLY — no scene, no lighting mood, no
  "at dusk on a dock." Append to every text prompt: *"single object, centered, neutral
  background, game asset."*

## §4 — THE 30 PROMPTS (priority order; generate top-down until credits thin)

Budget math: ~125–150 credits covers all 30 with variants (fits the 300 plan with retries to
spare). **If the month turns out to be a 50-credit tier: STOP after Tier 1** (~45 credits) —
that alone is the North Star set.

### TIER 1 — THE NORTH STAR SET (~45–50 credits; do these first, in this order)

1. **SLV-01 Scrapper hull** (no arm) — IMAGE: ortho sheet split into front/side/rear crops +
   hero shot (`concepts/ship_scavenger_mk1/`). Text hint: *"small asymmetric salvage
   spaceship, one oversized rear engine drum, raised forward cab, mismatched welded plating,
   chipped amber hazard paint, no grabber arm."* — 12 gens.
2. **Scrapper grabber arm** (separate, articulable) — TEXT: *"heavy industrial articulated
   grabber arm with three-finger claw, salvage crane aesthetic, hydraulic pistons, worn
   metal, chipped amber hazard paint, folded pose, single object, centered, neutral
   background, game asset."* — 4 gens.
3. **RILL** — IMAGE: front orb + back orb crops (`concepts/rill_drone/`, **crop out the
   glove**). Use the neutral/bright-eye state. — 5 gens.
4. **Artifact key half A** — IMAGE: crop of the separated-state half (`concepts/artifact_key/`). — 3 gens.
5. **Artifact key half B** — IMAGE: crop of the other half. — 3 gens.
6. **Tide skiff** — IMAGE: ortho sheet crops (`concepts/skiff_tide_mk1/`). — 5 gens.
7. **Warden capital** — IMAGE: hero crop (`concepts/warden_capital/`); its smooth read is
   easy for these tools. — 4 gens.
8. **Cal MK.IV** — IMAGE: MK.IV hero sheet (`concepts/cal_goliath_suit/`). Text hint:
   *"standing A-pose, arms slightly away from body."* Character/auto-rig path. — 8 gens.
9. **Gate pillar cluster** (ONE weathered monolith group — the gate's geometry; the tide
   stays shader/VFX) — IMAGE: crop a single pillar cluster from
   `concepts/ziptide_gate/gate_resting_pillars_v1`. — 4 gens.

### TIER 2 — HELD WEAPONS (~16 credits; face-distance = biggest visible upgrade per credit)

10. **Taser stun pistol** — TEXT (or nano-banana first): *"handheld salvage stun pistol,
    chunky industrial tool aesthetic, worn cast metal body, grippy wrapped polymer grip, one
    exposed cyan emissive coil, taped repairs, used-future, single object, centered, neutral
    background, game asset."* — 5 gens.
11. **Gravity gun** — TEXT: *"heavy two-handed industrial gravity projector tool, salvaged
    crane-magnet aesthetic, open ring emitter at the muzzle, thick cables, worn metal with
    chipped amber hazard stripes, one cyan emissive core, used-future, single object,
    centered, neutral background, game asset."* — 5 gens.
12. **Breaker bar (melee)** — TEXT: *"heavy industrial pry bar melee weapon, forged worn
    steel, wrapped grip, salvage tool aesthetic, dents and scratches, single object,
    centered, neutral background, game asset."* — 3 gens.
13. **Belt cutter multi-tool** — TEXT: *"compact industrial plasma cutter hand tool, worn
    metal, folded blade guard, small amber indicator lamp, utility belt tool, used-future,
    single object, centered, neutral background, game asset."* — 3 gens.

### TIER 3 — THE MOSS: LANDMARKS + OUTSKIRTS DRESSING (~33 credits; what makes levels LOOK like the concepts)

14. **The leaning tenement tower** (far landmark one-off; the tiling street kit stays
    Forge) — IMAGE: tower crops from K1/K2 (`concepts/toxic_city_kit/`). — 5 gens.
15. **Dockmaster booth** — IMAGE: K6 crop. — 4 gens.
16. **Harbor crane** — IMAGE: K6 crop, or TEXT: *"tall dockside salvage crane, lattice mast,
    counterweight, hanging hook block, rusted industrial, patched repairs, single object,
    centered, neutral background, game asset."* — 4 gens.
17. **Stilt pier segment** — TEXT: *"wooden and scrap-metal pier on tall stilts, tidal flat
    dock segment, barnacle-crusted legs, rope rails, weathered planks, single object,
    centered, neutral background, game asset."* — 3 gens.
18. **The RUSTBUCKET wreck** — TEXT: *"beached rusted salvage barge shipwreck, broken hull
    plates, listing to one side, holes and exposed ribs, industrial cargo ship, heavily
    weathered, single object, centered, neutral background, game asset."* — 4 gens.
19. **Sea-wall breach chunk** — TEXT: *"massive broken concrete sea wall segment, exposed
    rebar, tide-stained base, monumental scale ruin fragment, single object, centered,
    neutral background, game asset."* — 3 gens.
20. **Canal footbridge** — TEXT: *"narrow improvised canal footbridge of welded scrap metal
    and planks, handrails of pipe and rope, patched repairs, industrial shanty aesthetic,
    single object, centered, neutral background, game asset."* — 3 gens.
21. **Shanty facade block** (ONE backdrop building, not the modular kit) — IMAGE: best
    single-building crop from K4 (avoid the panels with real-world script). — 4 gens.
22. **Channel-marker buoy** (canal wayfinding prop) — TEXT: *"rusted harbor channel marker
    buoy with a small lantern cage on top, peeling paint, tide stains, single object,
    centered, neutral background, game asset."* — 2 gens.

### TIER 4 — LIVED-IN DENSITY: SHIP INTERIOR + PROPS (~20 credits; the cozy-industrial read)

23. **Pilot seat** — IMAGE: seat crop from `ship_S4_cockpit_v1`. — 3 gens.
24. **Control console cluster** (throttle quadrant + levers + toggles as one desk unit) —
    IMAGE: console crop from S4. — 4 gens.
25. **Bunk pod module** — IMAGE: single-bunk crop from `ship_S5_quarters_v1`. — 3 gens.
26. **Oil lantern** (the lantern-amber motif made physical — dock, skiff pole, booth) —
    TEXT: *"worn brass and glass oil lantern with wire cage, warm amber glass, carry ring,
    dents and patina, single object, centered, neutral background, game asset."* — 2 gens.
27. **Floodlight cluster** — TEXT: *"industrial floodlight cluster on a mounting bracket,
    four mismatched lamps, cables, rust and chipped paint, salvage aesthetic, single object,
    centered, neutral background, game asset."* — 2 gens.
28. **Cargo set** (crate + barrel + strapped bundle — the salvage grammar's everywhere-props) —
    TEXT: *"set of three used-future cargo props: dented metal crate with stencils, rusted
    barrel, tarp-wrapped bundle with straps, industrial salvage, centered, neutral
    background, game asset."* — 3 gens.
29. **Contract ledger + stamp** (dockmaster/ship desk hero prop) — TEXT: *"thick worn
    leather-bound ledger book with brass clasp and a heavy metal ink stamp beside it, dog-
    eared pages, utilitarian office prop, single object, centered, neutral background, game
    asset."* — 2 gens.

### TIER 5 — OPPORTUNISTIC (~5 credits; only if the day still has room)

30. **Hostile spider drone** (the parked Wake-Guild design — enemy class, NOT RILL) — IMAGE:
    spider-drone crop from the Cal sheets (`concepts/cal_goliath_suit/`). — 3 gens.
    *(Optional 30b if credits remain: the hex SEAL from `artifact_hex_seal_variant_v1` — 2
    gens; glyph-law caution rides with it.)*

**Deliberately NOT on the list:** creatures (Creature V2 owns them — the Forge is genuinely
good there) · the skyscape (SkyVista is texture/data, not mesh) · the gate tide/entrainment
(shader + VFX, not mesh — only its pillars are geometry, #9) · the city as a whole and the
tiling building kit (modular precision is exactly what AI gen is bad at — the kit stays
Forge; gens buy one-off landmarks only).

## §5 — END-OF-DAY CLOSE

Keeper GLBs + manifests into `art_intake/` (or drive + manifests in repo), license record
saved, CREDITS lines drafted, HANDOFF entry listing what was generated and what won. Then
the backlog waits for the Golden-Checkpoint gate like everything else — intake (§2 cleanup +
wiring) is the first post-checkpoint art work, in the `CONCEPT_TO_BUILT_PIPELINE.md` §4
order.
