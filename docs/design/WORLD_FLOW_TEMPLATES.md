# World Flow Templates — per-environment "nice flow" recipes

**Purpose:** so worlds can be **grayboxed fast, play well, and stay distinct** — Fable 5 (or anyone) picks
a template per world from the 80-catalog and authors the data, instead of re-inventing layout each time.
Each template = a **spatial pattern + pacing/beats + combat rhythm + legibility plan + story hook**, mapped
to real authorable types (`CityLayoutDefinition` / world-kit, `WorldPackDefinition` jobs, `*ZoneDef`).
Builds on `CITY_DESIGN.md` (massing/legibility) + the Story Bible (why each world exists).

## Universal flow spine (every world, any biome)
A VR world reads well when it follows a legible loop, not a maze:
1. **Arrival/orient** — spawn with a clear **landmark sightline** (you instantly see where to go). Safe beat.
2. **Path with a promise** — a main route toward the landmark; side branches that *show* a reward/lore.
3. **Escalation** — combat/hazard beats spaced by **rest nodes** (plazas/overlooks) so it breathes.
4. **The turn** — the mid-world reveal (the mystery object / a vista / a RILL beat).
5. **Objective + return** — complete the job, then a short, changed-route return (or travel-gate out).
**Pacing target ~10–20 min** standard worlds; signature worlds longer. Always: a landmark you can navigate
by (no minimap), 2+ routes at choke points (anti-camp/anti-linear), rest after every fight.

## Vary it so 80 worlds don't feel same-y (the 4 knobs)
- **Verticality** (flat ↔ towering), **density** (open ↔ tight), **hazard tempo** (calm ↔ constant),
  **combat mix** (none ↔ swarm ↔ single-elite). Pick a *different* corner of these per adjacent world so
  consecutive worlds contrast. The Bible's biome list already alternates these — honor it.

## Templates by environment (from the §12 biome column)

### CITY (W001/009/030/034/039/048) — *the workhorse*
Layered districts + street spine + node plazas. Use `CityLayoutDefinition`/`CityBuilder` directly.
- **Flow:** dock/hub → main spine (landmark at the end) → branch districts (canal/slum/market) → mission
  interior → return. **Beats:** patrol drones on the spine, a node-plaza fight, a hero-building objective.
- **Legibility:** height-step toward the landmark; color-zone districts (CITY_DESIGN P0). **Vary:** canal
  city vs vertical slum vs half-built (open) vs Pattern-converting (geometry shifts).

### UNDERGROUND (W002/017/025/058) — *descent + dark*
Vertical shaft hub → galleries → deep chamber. Light is the resource; verticality the challenge.
- **Flow:** shaft mouth (light landmark) → descend via ledges/drift-belt → mine/objective → ascend changed.
  **Beats:** swarm ambush in the dark, a collapse hazard timer, a conveyor/mine puzzle. **Vary:** flooded
  vs resonant (sound) vs Architect-stonework reveal.

### EXTERIOR-OPEN (W003/006/014/026/044) — *vista + traversal*
Big sky, long sightlines, a single dominant landmark. Wind/heat/acid as the spice.
- **Flow:** ridge spawn (see the whole map + the goal) → traverse hazard terrain (anchor/zipline) → harvest/
  build at nodes → reach the overlook objective. **Beats:** environmental hazard > combat; a crashed-wreck
  lore stop. **Vary:** glass mesas vs mirror flats vs ashfield vs glass-bloom (beauty).

### INTERIOR (W004/008/018/022/033/042/047/060/062) — *dread + lore + handcraft*
Linear-with-branches; tight, atmospheric, often combat-light. The "movie" rooms.
- **Flow:** entry → 2–3 themed rooms (puzzle/lore) → the chamber (reveal/objective) → out. **Beats:** static/
  organic hazards, environmental storytelling, a signature RILL beat. **Vary:** archive vs nursery vs
  cathedral vs tomb vs revelation chamber. *(W028/W024-style "no-job" beats are interiors stripped to pure mood.)*

### VOID (W012/019/024/028/038/050/057/063–068) — *abstract, set-piece, choice*
Floating platforms / the Shell wall. Used for capstones, edges, the branch, endings.
- **Flow:** minimal traversal, maximum framing — the sky *is* the content (the Shell, the Pattern, the
  named color). Often a single decision or a single act. **Beats:** sparse or none (W057 transit-only).
  **Vary:** the edge vs the convergence (all-hazards) vs the four endings.

### COASTAL / UNDERWATER (W010/013/020/029/032/045/052) — *tide + pressure + buoyancy*
Rhythm worlds: a recurring tide/pressure cycle gates movement and vulnerability.
- **Flow:** shore/surface → time the tide / manage depth → reach the submerged/coastal objective → out on
  the next cycle. **Beats:** the cycle *is* the puzzle; tendril/sentry combat between cycles. **Vary:** acid
  coast vs memory reef vs drowned lab vs signal-flood (light).

### PATTERN (W015/021/029/039/054/067) — *impossible geometry (see `OPTICAL_ILLUSIONS.md`)*
The universe's geometry misbehaves — anamorphic reveals, looping space, shifting streets.
- **Flow:** the path must be *solved by viewpoint/observation*, not just walked. **Beats:** Pattern enemies
  (non-lethal shove), the wrist-scanner reveals the real geometry. **Vary:** soft (early/gentle) vs lattice
  vs full Pattern-city vs broken-Pattern (the ending preview). **Comfort:** visual-not-vestibular only.

### STATION (W007/016/023/041/049/055) — *faction interiors, low-g*
Orbital faction hubs. Tight corridors, viewports framing the Shell/debris (story windows).
- **Flow:** dock → corridors (faction NPCs/standoff) → the faction objective (vault/console/defense) → out.
  **Beats:** non-lethal guard standoffs, a choice moment, a viewport reveal. **Vary:** Sable vs Warden vs Guild.

### FOREST / BIO (W005/040) — *Bloom ecology, vertical canopy*
Living overgrowth; the Splicer is the key tool. Organic verticality.
- **Flow:** ground → canopy via splice-bridges → harvest/clear the Bloom node → out. **Beats:** tendril
  swarms, spore hazard, a splice puzzle. **Vary:** oxidized canopy vs tendril farm.

## How to use this (per world)
1. From the world's `CHAPTER_*` seed, read its biome → pick the template above.
2. Author the `CityLayoutDefinition` (or world-kit notes) to the template's spatial pattern + the 4-knob
   contrast vs its neighbors; apply `CITY_DESIGN` legibility (landmark sightline, color zoning, ground floor).
3. Fill the `WorldPackDefinition` jobs to the template's beats (`JobDefinition` steps + reward + `completionFlag`).
4. Place enemy/hazard zones to the combat rhythm (rest after each fight).
5. **Reskin/rewrite cheaply:** a world's identity lives in its data (layout + palette + WorldPack), so swap
   the template or the layout asset to rewrite it wholesale; tweak fields to adjust.

*Cross-links: `CITY_DESIGN.md` · `OPTICAL_ILLUSIONS.md` · `systems/WORLD_BLUEPRINT.md` ·
`storyboard/_WORLD_TEMPLATE.md` + the 80-catalog · `ZIPTIDE_MASTER_BUILD_PLAN.md` §12.*
