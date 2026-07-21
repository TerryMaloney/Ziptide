# CONCEPT → BUILT — how an approved sheet becomes the thing in the headset
### Terry's question (2026-07-21): "how capable is our system going to be to actually build something based off concept art like this… or the buildings… I want the whole plan."

**Status:** 🔵 PLAN — zero code (freeze). This doc is the honest capability audit + the one
route every approved concept follows. Companions: `HERO_ASSET_STRATEGY.md` (the tiers),
`TIER_C_CONCEPT_QUEUE.md` (the approved sheets), `SHIP_SCAVENGER_VISUAL_SPEC.md` +
`CITY_VISUAL_SPEC.md` (the proof this route's first step already works).

---

## §0 — The honest one-line answer

**The system is genuinely capable of building from these concepts — unevenly.** For RILL,
the artifact key, the buildings, the skyscape, and the gate effect, the machinery largely
EXISTS TODAY and the concepts are close to directly executable. For the ship it exists as an
approved plan (the B+ bridge). For close-range hero surfaces it needs the Tier-B ops that are
planned but unbuilt. For exactly ONE asset — Cal as a visible full-body character — we have a
real capability hole with no current machinery, and it needs a ⚖ decision. Everything queues
behind the Quest Golden Checkpoint like all code work.

## §1 — THE ONE ROUTE (proven twice already; every asset class below rides it)

Every concept→built conversion is the same five steps. Steps 1–2 are paper (freeze-legal and
partially DONE); steps 3–5 are post-checkpoint code/asset work.

1. **MEASURED VISUAL SPEC.** A session reads the approved keeper images and extracts numbers
   and grammar: proportions measured off the sheet, palette→material-family mapping, the
   protect-tells (the details that make it IT — RILL's repair panel, Cal's red laces), and an
   op/part decomposition. *Proven: `SHIP_SCAVENGER_VISUAL_SPEC.md` (nose 0–0.18, engine drum
   0.62–1.00, quincunx nozzles, ~14.4k-tri 7-group decomposition) and `CITY_VISUAL_SPEC.md`
   (district rings off K1, facade grammar off K4). This step is a solved skill.*
2. **RECIPE PLAN.** The spec becomes a `ForgeRecipeDefinition` plan (Tier A/B) or a WC-5
   intake plan (Tier C): which ops, which material families, which budget class, which LOD
   story. Declares the asset's tier per HERO_ASSET_STRATEGY law 1.
3. **BUILD + BOOTH LOOP.** The recipe is implemented and rendered in `ForgePhotoBooth`
   turnarounds **side-by-side with the concept sheet** — the concept is the acceptance
   target, not a mood board. Iterate op parameters until the booth render reads as the sheet
   at Quest draw distances. *Proven method: the taser and warden loops — except those ran
   BLIND (no reference), which is why the warden took five rounds. The sheets are precisely
   the fix: the booth finally has something to chase.*
   **Small tooling add (cheap, high value): a booth reference-plate mode** — render the
   turnaround next to the keeper image in one comparison PNG so Terry's verdict and CI
   artifacts capture concept-vs-built in a single glance.
4. **GATES.** The standing quality machinery, unchanged: `ArtConformanceAuditRules` +
   forge audits + budget validation (tri/texel/material caps per band) + EditMode tests.
   Tier-C imports additionally pass WC-5 intake (license record, re-material to our
   families, hero budget review).
5. **DEVICE VERDICT.** Terry sees it in the headset next build; his read against the sheet
   is the close. A miss loops to step 3 with the delta named.

## §2 — CAPABILITY BY ASSET CLASS (honest ratings against the approved sheets)

Rating = how much of the route's machinery exists TODAY / how close the built thing can get
to the sheet.

### 🟢 RILL — ~95% buildable now, Tier A/B (the easiest hero in the game)
The orb concept is almost a Forge-native description of itself: a sphere (our best
primitive) + engraved channel network (normal+emissive texture bake — FORGE II machinery) +
one riveted repair-panel decal (texture stamp) + the eye as recessed lens geometry with an
iris TEXTURE (the one part that must be authored/generated as a 2D asset — it's flat art,
not geometry). The four mood states map 1:1 onto the existing **`RillState` enum in Core**:
a material driver lerps eye-iris color + channel-emissive color/intensity per state, exactly
as the keeper's bottom row specifies. Face-distance texel law applies (she hovers close).
Missing: nothing structural — an iris texture source and the state-driver component.

### 🟢 ARTIFACT KEY — buildable now, Tier B (the held-item test case)
Two compact stone-metal halves = simple geometry + the same frozen-current channel bake as
RILL (deliberate — they're kin). Three states (separated / within-two-inches arcing /
joined-flowing) = emissive mask states + an F3.5 particle arc, all existing vocabulary.
This asset should go FIRST post-checkpoint: it exercises face-distance texel law, the
material-split law, emissive states, and the booth-reference loop on the smallest surface
area. The key is the pipeline's own tutorial level.

### 🟢 BUILDINGS / THE MOSS — the strongest systemic story we have, Tier A
Quantity architecture is exactly what the Forge is FOR, and the pieces exist in code:
**`ForgeBuildingKit`** (module recipes) + **`CityLayoutDefinition`** (district/canal data) +
the K-battery specs. Route: K4's construction grammar (concrete frame + corrugated infill,
base/middle/cap, numbered doors ⚖) becomes kit module recipes; K1/K7's measured rings become
the layout data; K3 is the street-level acceptance image ("the built street should read like
K3 squinted" — the Stage A verdict, already written as a testable sentence); K2/K5 feed the
skyline/outskirts impostor bands. GPT's city Stage A (hwr30) is the executing lane; the
specs are its reference. Missing: the kit's recipe breadth (a handful of facade/roof/door
modules) — normal build work, no new machinery.

### 🟢 SKYSCAPE — machinery SHIPPED, Tier A
The SkyVista system exists end-to-end (definitions, textures, rig, library of 17 canon
vistas, audits). Converting the dusk/panorama/night keepers = authoring vista definitions to
match (ring-arc landmark, the glowing horizon band, ≤3-body budget). This is data authoring
against existing code — the closest to "just do it" in the whole set.

### 🟡 THE GATE — layered plan written, effect work unbuilt (post-checkpoint)
`ZIPTIDE_GATE_VISUAL_DESIGN.md` §3 already routes it: ZiptideWater normals on sculpted crest
planes + emissive gradient + F3.5 spray + vertex-animated trough on one mesh + the existing
flash shell as the inside-the-wave beat; §1b entrainment = authored loose-rim pieces + a
dissolve ring (CP-3-style mask) + particle streams. All six lifecycle states have approved
references. Rating 🟡 only because it's EFFECT work judged in motion — the booth loop proves
stills; the device pass proves the breathing. Budget rail: transparent-coverage ≤35%.

### 🟡 THE SHIP — approved plan, two-stage delivery (B+ now → hero mesh later)
Stage 1 (B+ bridge, Terry-approved): Forge-built from the measured spec via the booth loop —
silhouette, masses, material story right; becomes the PERMANENT distant/berth LOD.
**`HeroShipHullBuilder` already exists in code with tests** — the hull path is started.
Stage 2 (Tier C): the paid image-to-3D month (trigger already MET) or commission converts
the ortho sheet to the hero mesh; WC-5 intake re-materials it; `ForgeVisualApplier` swaps
the visual child; gameplay never knows. Interiors: `ShipHullBuilder` graybox + module
dressing; the cockpit's chunky levers/toggles are Tier-B props (each one small and
buildable); the honest gap is small-prop DENSITY — the lived-in read comes from dozens of
tiny objects, which is assembly-line time, not missing tech.

### 🟡 WARDEN CAPITAL — easy far, gated near
As the P3-band silhouette (how it's usually seen): smooth segmented lozenge + one eye +
sterile-white seam emissive = trivially Forge-native, buildable now. As a close-flyby hero:
the smooth ceramic-bone read is exactly where razor-edge procedural geometry fails —
needs Tier-B ops (Bevel/Chamfer especially) or the Tier-C path. Plan: build the P3 version
first; promote only if a story beat actually brings the camera close (W037 recognition
beat would).

### 🔴 CAL — the one real capability hole (⚖ decision required)
The Forge has no humanoid pipeline: no skinned-character path, no rig/animation intake, and
Creature V2's vocabulary is creatures, not people in suits. BUT the honest question first:
**how much of Cal does the game actually need?** Cal is the PLAYER in VR — first-person
hands/forearms + chest rig (the avatar rig matches the sheet already). Full-body Cal appears
only in: reflections (a story device!), the title menu, and any third-person ride-scene
beats. Three routes ⚖:
1. **First-person-only + implied body** (hands, forearm tally guard, boots-with-red-laces
   visible when looking down; mirror beats staged to show the SUIT, visor toward camera).
   Zero new machinery; ships the canon tells. The VR-native answer.
2. **AI-gen/commissioned hero body + auto-rig** for the few staged full-body moments —
   Tier-C intake, one asset, animation kept minimal (idle/stand poses).
3. **Full humanoid pipeline** — NOT recommended; it's an engine-sized commitment the game's
   camera almost never rewards.
Recommendation: route 1 now, route 2 if the Director's Cut demands a full-body shot. This
is the only sheet in the set whose built version needs a scope decision rather than a plan.

## §3 — WHAT'S ACTUALLY MISSING (the complete gap list, smallest first)

1. **Booth reference-plate mode** — concept image beside the turnaround in one PNG.
   (Small editor change; makes every loop measurably honest.)
2. **Per-asset visual specs still unwritten** — RILL, key, gate states, Warden capital,
   skiff got deferred-to-build-time. Freeze-legal paper; can be written any time.
3. **Tier-B hard-surface ops** — Bevel/Chamfer, PanelInset, GreebleScatter + face-distance
   texel law + material-split validate. The single biggest read-upgrade for everything held
   or closely orbited (weapons, key, cockpit levers, Warden near-pass). Planned
   (HERO_ASSET_STRATEGY §2-B), unbuilt.
4. **RillState material driver + iris art** — the one RILL-specific piece.
5. **Building-kit recipe breadth** — the K4 grammar as actual module recipes (GPT's Stage A
   lane feeds on this).
6. **WC-5 intake** — build on first real Tier-C import, not before (standing rule).
7. **The ⚖ queue that gates starts:** Cal body route (above) · gate arena/wild fork ·
   suit-tier mapping · plus the standing art canon stack (photon-fluid, entrainment, the
   Moss, numbered doors, seal class, names).

**Notably NOT missing:** spec extraction (proven), the booth loop (proven), conformance/
budget gates (shipped), the applier swap contract (shipped), city layout data (exists),
sky vistas (shipped), the license law (written), the paid-3D trigger (met).

## §4 — SEQUENCE (all build steps post-Golden-Checkpoint; paper steps ● anytime)

- ● Write the missing visual specs (gap 2) — RILL first (smallest, freshest), then key,
  gate-state table, Warden, skiff.
- ● ⚖ Cal body route decision (recommend route 1) + the standing art-canon stack.
- Post-checkpoint, in order of leverage:
  1. **Booth reference-plate mode** (gap 1 — everything after it gets judged properly).
  2. **The artifact key** end-to-end (the pipeline tutorial: smallest asset, most laws).
  3. **Tier-B ops envelope** (gap 3 — then every held item and the cockpit inherit it).
  4. **RILL** (driver + iris + orb recipe — the companion exists the day this lands).
  5. **Building kit + Stage A city** (GPT's lane, our specs as reference).
  6. **Sky vista authoring** (data work; can interleave anywhere).
  7. **B+ ship** through `HeroShipHullBuilder` + booth loop.
  8. **Gate effect upgrade** (motion work; device-judged).
  9. **Paid-3D month** when Terry chooses → hero ship mesh + any promoted assets through
     WC-5 → applier swaps.
- The measure of done, per asset: **the booth comparison plate + Terry's headset verdict
  against the sheet.** The concept images are the acceptance tests now — that's what this
  whole concept phase bought us.
