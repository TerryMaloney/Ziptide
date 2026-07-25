# WORLD RECIPE GAP AUDIT — what a COMPLETE world needs vs what the factory can make today
### The full inventory, the missing layers, and the missing archetype axis (derelicts, stations, interiors)
**Status: RESEARCH / GAP AUDIT — planning only, zero code. 2026-07-24, Terry-directed ("really dig into it").**
Companions: `FORGE_VI_WORLD_COMPILER.md` (the compile/hash horizon this feeds) ·
`FORGE_V_LIVING_STAGE.md` (states/weather/far-society — planned, not re-proposed here) ·
`CITY_STREETSCAPE_AND_AMBIENT_LIFE.md` (hwr29 street/NPC tiers) · `QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md`
(the per-world budget "formula" Terry remembers — it exists; §8 extends it per-element) ·
`GROWING_INVENTION_LOOP_DECISION_RECONCILIATION.md` (plants/machines the worlds must surface).

---

## 1 · WHAT WE HAVE (the honest inventory — it is genuinely a lot, all file-verified 2026-07-24)

The current recipe (`WorldSpec` JSON → `WorldSpecCompiler` → builders → audits) already declares:

| Layer | Machinery | Evidence |
|---|---|---|
| Terrain | 5 biome presets, radius 250–400m (audited), relief clamp, walkable slopes | `ExperienceDef`, `BiomePreset` |
| Arrival awe | 5 hero-vista kinds, spawn FACES the vista, fog auto-thin, sightline discipline | `VistaKind`, vistaDirection/Distance/Height |
| Sky | sky-vista library (moons/nebulae), planet-in-sky, gradient, skyline ring | `SkyVistaLibrary`, `WorldSpec` sky fields |
| Gameplay pockets | **12 POI verbs** (CombatCamp, HarvestGrove, MachineSite, RuinCache, CaveSecret, StoryAnchor, TravelBerth, Market, Shrine, RepairBay, Transit, **Lookout**), tiered 0–2, 5–9 per world with ≥3 verbs ENFORCED by audit | `PoiType`, `WorldPoiBuilder`, quality gates |
| Dressing | biome-keyed prop clusters via pure Poisson blue-noise `ScatterField` (rocks/flora/debris/crystals/bones), route+POI clearance | `WorldDressingBuilder` |
| Wayfinding | cairn chains along the route | `BuildCairns` |
| Ecology | creature/drone/hazard zones, `EcologyCore`, `BiomeDefinition.nativeCreatureIds` | zone defs |
| Story hooks | jobs per world (GateGap3 enforces "no world without beats"), flags gating, StoryAnchor POI | `WorldJobLibrary`, `WorldGating` |
| Economy surfacing | packs declare machines/mines/gardens/sockets/collectibles | `WorldPackDefinition` |
| Sound | per-world ambience layers, travel stingers (declared) | `AmbienceLayerDefinition` |
| Budgets | per-world totals (drawcalls/tris/materials/lights/objects) + perf audits + reachability audit + PG gates | budget doc + `Editor/Audit/*` |
| Cities | the whole `CityBuilder` lane + `CITY_VISUAL_SPEC` measured values | hwr29/GPT lane |
| Biome identity | `BiomeDefinition`: hazardType (free-form: "toxic", "vacuum"…), artKitId, native resources/creatures/plants, ambient tint | `BiomeDefinition.cs` |

**Planned elsewhere (do not re-plan):** world states & staged events & weather acts & DISTANT
ambient life (Forge V) · compile+hash law & kit completion (Forge VI) · near-street NPC tiers
(hwr29 research) · plants/machines catalog (growing reconciliation).

## 2 · THE GAP TAXONOMY — the missing layers of a complete world

Verdicts: ❌ = nothing exists · 🟠 = fragment exists, no owning design.

### 2.1 ❌ **THE ARCHETYPE AXIS** (the biggest hole — see §3)
Every recipe assumes: open surface + terrain + sky dome + ONE directional sun + full gravity +
breathable-with-hazard-flavor atmosphere. There is no INTERIOR archetype at all — no derelict
ship, no station, no cave-complex-as-world, no megastructure. `hazardType="vacuum"` is a string
with no physics, no loop, no lighting model behind it. This blocks a third of the story bible
(space salvage, the Transmission's sealed places, W007 Sable Station is a *name* on a surface
recipe) and the Director's Cut's own space-sortie beat.

### 2.2 ❌ **Inhabitants** (Terry: "an entirely skipped aspect" — confirmed)
Zero NPC code. The only "Dockmaster" is a label on a kiosk. Missing, from the recipe's view:
- `InhabitantDefinition` (id, role, silhouette kit, voice/bark set, narration text — rides the
  growing lane's spokenName seam);
- **placement slots on POI verbs** — a Market declares 2 vendor slots, a TravelBerth declares a
  dockmaster slot, a Shrine a keeper slot. Population becomes DATA on the existing grammar;
- the presence tiers already researched (machine citizens → posted figures → walkers, hwr29);
- the budget row (2–4 posted figures/world; hooded/respirator kits dodge facial anim forever).

### 2.3 🟠 **Flora split: harvest vs scenery**
`PlantDefinition` = harvestable only (the 24 species). Scenery vegetation exists ONLY as scatter
primitives. Missing: a `SceneryFloraKit` per biome — non-interactive, GPU-instanced, 2 LOD
tiers, wind-sway shader budgetted, **canopy/giant class for awe framing** — plus the rule that
harvest plants must remain VISUALLY DISTINCT from scenery (a readability law: if you can't tell
what's harvestable at 10m, the garden loop dies in the field). `BiomeDefinition.nativePlantIds`
already exists as the natural anchor for both lists.

### 2.4 🟠 **The viewpoint/awe contract** (Lookout exists; the CONTRACT doesn't)
`Lookout` builds a pocket, but nothing defines what makes it work: a composed **view target**
(vista + skyscape + midground layers verified visible from the pad), **reveal staging** (the
route conceals then reveals — the peak-end rule spatialized), a **photo hook** (the Field
Camera's `PhotoComposition` already scores subjects — lookouts should BE authored
high-score spots), a place-name reveal, one RILL line slot, a bench/anchor prop. One
`ViewpointDef` extension on the POI + an audit ("every world ≥2 composed viewpoints; view
target actually visible from pad" — raycastable, PG-3 style) closes it.

### 2.5 🟠 **Place-story layer** (the story OF the location, not jobs IN it)
Jobs exist; environmental storytelling has no owner: **toponymy** (named districts/streets/
landmarks + a name-reveal moment on entry), **echo/log objects** (found audio/text — the
derelict loop's backbone, also zero code), **dressing-with-intent** (StoryAnchor is one pocket;
no grammar for "this scatter tells what happened here" — the abandoned meal, the barricade, the
scorch line), and **state scars** (Forge V owns state *rendering*; the recipe must own which
scars a world DECLARES). Proposal: a `placeStory` block in WorldSpec: name set, 3–5 story
beats-as-dressing declarations, echo-object list, one "what happened here" line the player can
reconstruct without reading anything.

### 2.6 🟠 **Traversal network layer**
Ziplines/climb exist as one-off patchers (Cavern/Sandbox); canals only in cities. No recipe
vocabulary for: zipline runs as declared routes, climb walls, moving platforms/ferries, jump
pads — the VERTICAL fun layer. Should become optional `traversal` entries in WorldSpec
(kind + endpoints), built by one TraversalBuilder, audited by route-continuity (PG-3 already
samples routes — extend to declared traversal).

### 2.7 🟠 **Light-as-design layer**
The 1-directional-sun law is right for surfaces but the recipe has no *lighting intent* field:
time-of-dusk mood per world, practical-light density (the city spec's 2–3 window warmths),
darkness pockets (CaveSecret has no darkness model, no player lamp — also blocks §3 entirely).
Needs: per-world light script reference (F3.1 derivation exists for vistas — point the recipe
at it), a `darknessAllowed` flag with the player-lamp item as its unlock, emissive-practical
budget row.

### 2.8 🟠 **Water/liquid layer** — canals (city) and toxic pools (hazard zones) only. No
generic liquid volume (lake/shore/waterfall as vista), no interaction rule (wade? skiff? die?).
Small, but every biome list wants it. One `LiquidRegionDef` + surface shader within
transparent-material budget.

### 2.9 ❌ **Per-element budget formula** (Terry: "I think we may already have a formula" — half-true)
Per-WORLD caps exist and are audited. What doesn't exist is the **allocation template**: how the
120-drawcall / 600-object budget SPLITS across terrain, dressing, POIs, NPCs, flora, traversal,
practicals — per archetype. Without it, every element lane spends until the audit screams.
Proposal (§8): a `BudgetEnvelope` table per archetype with per-class ceilings, checked by the
existing PerfBudget audit per generated ROOT (dressing root, POI root, flora root…), so an
overspend names the class that overspent.

### 2.10 🟠 **World Definition-of-Done v2** — audits enforce size/POIs/reachability/budget;
nothing enforces: ≥2 composed viewpoints, ≥1 place-story reconstruction, inhabitant slots
filled, harvest-vs-scenery distinctness, soundscape present, name set present. One checklist =
one audit rule set = the EXCELLENCE_MAP "world" row becomes mechanical.

## 3 · THE MISSING ARCHETYPE: INTERIORS — derelict ships, stations, sealed places

The design sketch (all composing EXISTING owners; nothing here invents a second stack):

**3.1 Substrate.** `WorldArchetype` enum on WorldSpec (additive, default `Surface` — zero
pollution of the 12 existing packs): `Surface | Interior | Hybrid`. Interior worlds replace
terrain with a **DeckLayout**: rooms + corridors + shafts as the district/connection grammar
re-skinned (rooms ARE districts, corridors ARE connections — the schema barely changes; the
BUILDER changes). InteriorAuditRules already exists for hollow heroes; it grows into the deck
auditor (route continuity, reach, clearance — PG-3/PG-4 apply verbatim).

**3.2 Lighting grammar (the sun law flips).** No directional light. Interiors are lit by
emissive practicals + 0–2 approved small realtimes (already the budget doc's escape hatch) +
**the player lamp** (a belt item — new `handheld_lamp`, ItemFactory branch, holster-legal;
doubles as the darkness unlock for CaveSecret). **Derelicts weaponize this: restoring power IS
the visual progress meter** — each `RepairableMachine` in the power chain flips a bank of
practicals from dead → emergency-amber → alive, and the ship visibly wakes as you fix it.
That's the loop AND the lighting model AND the budget answer (emissive state swaps, zero new
realtime lights) in one mechanic.

**3.3 Gravity tiers (comfort-first).** `full | low | magnetized` — deliberately NOT free zero-g
v1: free 6-DoF drift is a comfort minefield and a locomotion rewrite. `low` = existing
locomotion with modified jump/fall curves (LocomotionProfile data). `magnetized` = "mag-boots":
normal walking, the FICTION of zero-g (objects float — rigidbody gravity off for props is
cheap and spectacular), comfort laws fully intact. Free-float is a Forge-V-era experiment, on
file, not in v1. This gets 90% of the fantasy for 10% of the risk.

**3.4 Airlock & pressure loop.** A compartment state machine on the deck graph:
`pressurized | vacuum | venting`, doors as paired gates (the door-pair FSM composes
RepairableMachine door-panels + flags). Suit air = a field resource (ResourceLedger; the Moss
Filter family from the growing lane is LITERALLY this — the bio path makes your air gear).
Venting = staged hazard (audio + particles within caps). Breach rooms are optional-path salvage
prizes. The depressurize/repressurize cycle is a 20-second ritual that makes stations feel REAL
— and it's a door with a timer, not a physics system.

**3.5 Docking arrival.** Surface worlds arrive by berth; stations arrive by **dock ring**:
`ShipCastOffRuntime`/PUNCH-IT rails in reverse + clamp thunk + airlock as the arrival corridor
(the travel crest hides the scene swap exactly as today — TravelCoordinator untouched).
`TravelBerth` POI gains a `dockKind: pad | ring | breach` field. `breach` (cutting in through a
hull gap) is the derelict variant and the space-salvage beat from the Director's Cut.

**3.6 The two interior loops (this is the content payoff):**
- **DERELICT (dead):** dark → lamp → restore power chain → systems wake (lights/doors/gravity
  section by section) → salvage + echo-logs tell what happened → SOMETHING responded to the
  power coming on (ecology re-uses drone/creature waves) → take the prize, get out. Verbs:
  RepairBay/RuinCache/CaveSecret/StoryAnchor — **all four already exist**; the archetype just
  re-skins their pockets into compartments.
- **INHABITED (alive):** lit, pressurized, LOUD (ambience + murmur), Market/Shrine/Transit
  verbs + inhabitant slots (§2.2), services, a no-weapons sanctuary rule (holster-enforced —
  the holster contract already owns this), story contacts (`>_ V. BOOTSTRAPPER` posts jobs
  SOMEWHERE — this is where). Same deck substrate, opposite dressing/sound/light tables.
- One derelict can BECOME inhabited after restoration — that's a Forge V world-state delta
  riding on this archetype for free, and it's the most Ziptide sentence in this document:
  *you fix a dead place and life comes back.*

**3.7 Budget profile.** Interiors trade terrain (0 tris) for room detail: the §8 envelope gives
the archetype its own split (more objects/practicals, no skyline, no scatter field, tighter
culling by design — rooms are natural occlusion volumes; likely the EASIEST archetype to hold
72fps in, which is why it's strange we built it last).

## 4 · WHAT THIS COSTS (rough, honest)
§3 substrate+lighting+one derelict pocket: ~3–4 slices of belt-lane size. §2.2 inhabitants v1
(slots + posted figures): ~2 slices. §2.3/2.4/2.5 (flora split, viewpoint contract,
place-story): ~1 slice each, mostly data + one builder pass each. §2.9 budget envelope: 1 slice
(constants + audit split). None of it before M0; almost all of it is Growth-Round-shaped
(observable, ≤3 commits per item).

## 5 · RECOMMENDED SEQUENCE (feeds Round 02+, Terry adjudicates)
1. **Budget envelope + DoD v2** (§2.9/§2.10) — cheap, makes every later element accountable.
2. **Viewpoint contract + flora split** (§2.4/§2.3) — biggest awe-per-commit on existing worlds.
3. **Inhabitant slots + 2 posted figures** in ToxicCity (§2.2) — kills the "empty downtown".
4. **Place-story block** on W001/W002 (§2.5).
5. **Interior archetype pilot: ONE small derelict** as the Director's Cut space-sortie pocket
   (§3) — proves deck substrate, lamp, power-restoration loop, breach docking in one 10-minute
   experience the FIRST HOUR already wants.
6. Full station world (inhabited loop) only after the pilot's device verdict.

## 6 · LANE ROUTING
Archetype substrate/builders = worlds lane (Architect) · inhabitant defs/narration = growing
lane's seam + hwr29 tiers · flora kits + viewpoint art = Picasso/GPT concept lane (playbook
families: "derelict corridor kit", "scenery flora sheet", "posted inhabitant silhouettes") ·
budget envelope + audits = factory lane · all sequencing behind M0 + the world-factory order.
