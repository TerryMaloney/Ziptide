# TIER-1 PROP BILL — the super-specific prop list for the first four worlds
### Terry 2026-07-21: "make sure our props are super specific… the more specific the better; make inferences where needed." Done — every prop in the Tier-1 concepts, named and dimensioned.

**Status:** 🔵 SPEC — zero code (freeze). Derived by reading the approved keepers in
`concepts/w001_*`→`w004_*`. Inferences are marked *(inf)* — canon-plausible detail invented to
make the prop buildable, per Terry's license. Feeds Forge recipes (Tier A/B), the Tripo intake
(Tier C), and `CONCEPT_TO_BUILT_PIPELINE.md`. **This is the pattern for every world's prop bill.**

## §0 — The prop-spec law (how to read a row, and the rules)

**Format:** `prop_id` · WHAT + real size · MATERIAL family · WEAR · INTERACTION · TIER · [src].
- **Sizes are real meters** (VR is 1:1 — a wrong-scale prop breaks presence instantly).
- **REUSE-FIRST:** a prop that appears in ≥2 worlds is specced ONCE in §1 and only *referenced*
  later. Do not re-spec a gauge per world — the salvage grammar is one kit (`CATALOG_BREADTH`).
- **INTERACTION vocabulary:** `static` (dressing) · `grab` (hand-pickup, has collider+grab pose)
  · `machine` (a `MachineDefinition` with steps) · `hazard` (damages/scrambles) · `hero` (held
  at face distance — highest texel budget, the face-distance law).
- **TIER** per `HERO_ASSET_STRATEGY`: A = Forge-native · B = Forge-plus (needs bevel/panel/
  greeble ops or high texel) · C = hero mesh (Tripo/commission).
- **MATERIAL families** are the locked set: `used_metal` (worn steel/iron), `salvage_paint`
  (chipped hazard amber), `cast_iron_dark`, `architect_stone` (amber-glyph masonry), `glass_
  crystal`, `warm_glass` (lantern), `cyan_emissive` (signal/tech), `amber_emissive` (Architect).

---

## §1 — SHARED SALVAGE PROP FAMILIES (spec once — these repeat across ALL worlds)

*Every Tier-1 concept is built mostly from THESE. Getting this kit right is 70% of the look.*

- **`prop_pipe_run_kit`** — modular straight/elbow/T/flange pipe segments, Ø 0.15–0.6 m, in a
  few diameters. `used_metal` + rust streaks. Static (some are climb/cover). **Tier A.** The
  single most-repeated element (cistern, pump room, spine room, ship). [all]
- **`prop_wheel_valve_01`** — spoked round handwheel valve on a pipe flange, Ø 0.35 m *(inf)*.
  `cast_iron_dark`, rust, one worn-shiny grip arc where hands turn it. **grab/interact** (turns).
  **Tier A/B.** [w002 #1/#3]
- **`prop_dial_gauge_cluster`** — bank of 3–5 round analog pressure gauges, each Ø 0.12 m, on a
  small bracket plate; cracked glass on one *(inf)*. Needle + faint backlight. `used_metal`,
  glass. Static (readable). **Tier B** (face-close text/needles). [w002 #2/#3, w004 #8]
- **`prop_oil_lantern_caged`** — brass-and-glass oil lantern, wire cage, 0.28 m tall, carry ring.
  `warm_glass` + `amber_emissive` (the human worklight motif). **grab** (portable light). **Tier
  B.** [w002 #3, w004 #8] *(already queued as Tripo #26 — same asset.)*
- **`prop_cable_bundle`** — draped bundles of 5–12 ropey cables/hoses, sag physics look, 0.1–0.2 m
  thick, running floor↔wall↔ceiling. `used_metal`/rubber, grime. Static. **Tier A** (spline-
  placed). The connective tissue of every interior. [all interiors]
- **`prop_catwalk_segment`** + **`prop_railing`** — grated steel catwalk deck, 1.2 m wide
  modular; pipe-railing 1.0 m high. `used_metal`, worn tread. Static (walkable). **Tier A.**
  [w002 #1, w004 #8]
- **`prop_deck_plate`** — 1×1 m salvage floor plate, some peeled/missing (the entrainment "loose
  rim" reuse), diamond-tread. `used_metal`. Static. **Tier A.** [w003 #5, w004 #7]
- **`prop_crate_set`** — dented metal crate (0.6³ m), sealed barrel (Ø0.5×0.9 m), strapped
  tarp bundle. Stencilled *(inf, invented glyph mark — glyph-law)*. `used_metal`+`salvage_paint`.
  **grab** (small ones). **Tier A.** [w003 #5] *(= Tripo #28.)*
- **`prop_hand_tool_set`** — pipe wrench (0.4 m), pry bar, welding torch + hose, coil of hose.
  `used_metal`, oil-stained. **grab.** **Tier B** (held → texel). [w002 #3, w003 #5, w004 #8]
- **`prop_scaffold_kit`** — bolt-together salvage pipe scaffolding + plank platforms. `used_metal`.
  Static (some walkable). **Tier A.** The "crude modern build bolted onto the ancient" language.
  [w002 #2]
- **`prop_salvage_console`** — a chunky control desk: sloped face, physical levers, toggle rows,
  `prop_dial_gauge_cluster`, and ONE recessed screen (`cyan_emissive`, low-res CRT look). 1.4 m
  wide. `used_metal`. **machine/interact** (the repair-panel UI surface). **Tier B.** [w002 #3,
  w004 #7/#8] — the recurring "you operate this" object.

---

## §2 — W002 DRY CISTERN props (underground · mining · Architect stone)

- **`prop_spiral_stair_cast`** — cast-iron spiral staircase, Ø 2.2 m, modular per-turn, ornate
  Architect-adjacent railing *(inf)*. `cast_iron_dark`. Static (walkable). **Tier A.** The
  cistern's vertical signature. [w002 #1]
- **`prop_arch_masonry_kit`** — monumental arched-vault stone blocks (the cathedral read):
  keystone, voussoir, pier. 3–6 m spans. `architect_stone` (dry, no glyphs on the plain set).
  Static. **Tier A** (kit) → far walls Forge, near arches **Tier B** for bevel/wear. [w002 #1]
- **`machine_cistern_pump`** — the signature machine: a house-sized pump body, Ø1 m intake pipe,
  crank/valve inputs, a `prop_salvage_console` face, output to the conveyor. `used_metal`+rust,
  one `cyan_emissive` readout. **machine** (multi-step restart: route power → prime → open valves).
  **Tier B.** [w002 #3]
- **`machine_ore_conveyor`** — inclined chain-bucket conveyor, buckets 0.4 m, climbing a shaft;
  buckets physically carry `mineral` chunks. `used_metal`. **machine** (first conveyor lesson —
  reach in, grab a chunk). **Tier B.** [w002 #3]
- **`hero_architect_glyph_wall`** — the old-under-new reveal wall: colossal `architect_stone`
  panels with channels of `amber_emissive` glyphs (flowing frozen-current motif). 4–8 m.
  **static/hero** (story landmark; the glyph set comes from the Tier-5 Architect language sheet).
  **Tier B/C.** [w002 #2] — the amber half of the color-language canon, made physical.
- **`prop_pipe_port_ancient`** — a large dark circular port/orifice in the ancient stone, Ø2 m,
  rimmed with worn metal *(inf)* — where old meets new plumbing. `architect_stone`+`used_metal`.
  Static. **Tier A.** [w002 #2]
- **`hero_glyph_plate_w002`** — THE mystery object: the pump-control **glyph-plate**, a portable
  ~0.4 m stone tablet, dormant `amber_emissive` glyphs that light when seated; predates the city.
  **hero** (face distance; seats into `machine_cistern_pump`). **Tier C.** [inferred from README §3]
- Also present: `prop_wheel_valve_01`, `prop_dial_gauge_cluster`, `prop_cable_bundle`,
  `prop_catwalk_segment`, `prop_oil_lantern_caged`, `prop_hand_tool_set`, `prop_scaffold_kit`,
  **+ hanging stalactites** `prop_stalactite_cluster` (Tier A, dressing). [w002 #2]

## §3 — W003 GLASS SHELF props (open sky · wind · glass · Pattern seed)

- **`machine_windbaffle_relay`** — the signature machine: a 6–8 m salvage tower of welded panel
  "vanes/sails" on a bolted base, raised/aligned to calm the wind. `used_metal`+`salvage_paint`,
  one `cyan_emissive` status light. **machine** (raise + align steps). **Tier B.** [w003 #5]
- **`prop_gantry_crane`** — salvage A-frame gantry crane, ~7 m, hook + cable block, counterweight.
  `used_metal`, hazard-amber. Static (animated hook *(inf)*). **Tier B.** [w003 #5]
- **`prop_indicator_box_green`** — a small ruggedized equipment box, 0.4 m, with ONE bright green
  status lamp *(note: green is a rare accent — reserve for "powered/OK" readouts)*. **Tier A.** [w003 #5]
- **`node_glass_bloom`** — harvestable glass-crystal cluster growing from rock, 0.5–1.5 m, faceted
  `glass_crystal` (amethyst body + rare green mineral veins), faint internal ring-glow. **machine/
  grab** (Harvest with tiered tool). **Tier B.** [w003 #6]
- **`prop_host_rock_glyph`** — dark meteoric host boulder the blooms grow from, 1 m, with worn
  engraved invented glyphs + drilled sample holes *(inf)*. `architect_stone`-adjacent. Static.
  **Tier A/B.** [w003 #6] — quietly ties the glass to the Architects.
- **`hero_glass_crystal_tuned`** — THE mystery object: a fist-sized **fractal-geometric** crystal
  (not organic — its precision rhymes with the Pattern), cyan/violet internal glow, "rings at
  W001's frequency." **hero** (face distance). **Tier C.** [w003 #6]
- **`prop_wreck_glider_skiff`** — the crashed glider-skiff wedged in glass (README §9), ~4 m,
  broken wing/hull, a readable log panel. `used_metal`, ice-frosted. Static (enterable-ish).
  **Tier B/C.** [w003 #4, bottom-right module]
- Also present: `prop_crate_set`, `prop_cable_bundle`, `prop_hand_tool_set` (welding rig),
  `prop_deck_plate`. Sky/planet/moons + the zenith Pattern shimmer = **SkyVista, not props.**

## §4 — W004 BROADCAST TOMB props (dead station · dread · static)

- **`prop_dead_screen_module`** — the wall's atom: a chunky CRT-style monitor in a metal bezel,
  0.5×0.4 m, convex dead-grey glass, dust film. Tiled into a grid wall. **Tier A** (one module,
  instanced) — **and the STORY variant** `prop_live_screen_alien_sky` (2–3 of them faintly showing
  an alien sky, the only light + motion; `cyan_emissive` static-glow). [w004 #7]
- **`prop_broadcast_desk_long`** — the long fixed control counter under the screen wall, ~6 m,
  concrete-and-metal, recessed equipment bays (some empty/gutted), cable troughs. `used_metal`+
  concrete. Static (the operator's station). **Tier A/B.** [w004 #7]
- **`prop_portable_monitor_unit`** — a small standalone device on the desk, 0.3 m, handles, dead
  screen — a `grab`/inspect prop (a diegetic "log reader"). **Tier B.** [w004 #7, left foreground]
- **`machine_broadcast_spine`** — the signature machine: a multi-tier stacked-transmitter column,
  4+ m, ceramic insulators, rope-cable feeds, a `prop_salvage_console` base with a lever bank +
  brass nameplate *(inf: invented-glyph plate)*. Runs power through **`hazard_static_arc`** (arcing
  discharge between junctions — damages/scrambles gear; forces careful pathing). **machine + hazard.**
  **Tier B.** [w004 #8]
- **`prop_lever_bank`** — a row of 4–6 heavy pull levers on the console, 0.3 m throw, worn wooden
  *(inf)* grips. **grab/interact** (the repair steps). **Tier B.** [w004 #8]
- **`hero_memory_shard_w004`** — THE mystery object (triple-canon): a palm geometric crystal orb,
  internal circuitry-etch, projecting a garbled `cyan_emissive` face + floating invented glyphs
  (the first Transmission fragment; "hidden in the static is Cal's own voice"). **hero.** **Tier C.**
  [w004 #9] — also the reference for the Transmission + memory-shard families game-wide.
- Also present: `prop_cable_bundle` (heavy), `prop_oil_lantern_caged` (amber wall lanterns),
  `prop_catwalk_segment`+`prop_railing`, `prop_dial_gauge_cluster`, ceiling `prop_duct_run` *(inf,
  Tier A)*, red emergency lamps `prop_emergency_lamp` *(inf, Tier A, rare red accent = danger)*.

## §5 — THE HERO / HELD PROPS (face-distance law — highest texel, build Tier-C first)

These four are held at ~30 cm and carry the story — they get the project's top texture density,
2–3 distinct materials, and go through Tripo/hero intake before the dressing props:
`hero_glyph_plate_w002` · `hero_glass_crystal_tuned` · `hero_memory_shard_w004` · (from the
prior wave) the artifact key + RILL. **Rule:** a hero prop is never shared/instanced and never
shares an atlas — the opposite of the §1 kit.

## §6 — Accent-color discipline (locked by the Tier-1 batch)

- `amber_emissive` = **ancient Architect** stone/glyphs only. `cyan_emissive` = **signal/tide/
  tech** (screens, tuned crystal, the transmission). **green** = rare "powered/OK" readout only.
  **red** = danger/emergency only. **warm amber (`warm_glass`)** = human worklight (lanterns).
  Warden white unused here. Keep accents SCARCE — one glow per prop, or it stops meaning anything.
