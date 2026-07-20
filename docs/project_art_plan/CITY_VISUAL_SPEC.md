# TOXIC CITY VISUAL SPEC — the city, measured from Terry's approved concept kit
### Six K-levels approved in ONE batch (2026-07-20) + a bonus ship-in-context sheet with cab POV

**Status:** 🟢 REFERENCE APPROVED · canonical images at `concepts/toxic_city_kit/` (K1–K6) and
`concepts/ship_scavenger_mk1/ship_context_4panel_cabpov_v1.png`. Images outrank prose.
**Consumers:** `CityLayoutDefinition` layout data (K1) · CityBuilder Stage A — 📣 GPT hwr30
item 2's visual reference (K3/K4) · ForgeBuildingKit module recipes (K4) · skyline vista +
arrival flight (K2) · flyover dressing ring (K5) · berth/first-hour/title staging (K6) ·
S4 cockpit dressing (the cab POV panel).

## §1 — THE CITY SHAPE (read off K1 — the layout blueprint)

Concentric rings on a drowned tidal flat, harbor cut into the SOUTH face:
1. **Center: the Tower island** — the colossal leaning scrap tower (W001's WC-4 hero element,
   queue C4 — K1/K2/K5 agree on its silhouette: stacked mismatched decks, cranes at the crown,
   lean ~10–15°) rising from the densest shanty knot.
2. **Ring 2: dense shanty districts** — 4–5 irregular wedges of stacked corrugated tenements
   divided by a canal ring + radial canals; causeway/foot bridges cross at ~4 points.
3. **Ring 3: the old sea wall** — heavy concrete rampart circling the city, BREACHED in
   places (K5's breach = a gameplay/flyover landmark); wall-top shacks.
4. **Harbor wedge (S):** double-armed stone breakwater enclosing the berth basin; quays,
   cranes, containers, the moored Scrapper (K6).
5. **Ring 4: THE FLYABLE OUTSKIRTS** — tidal mud flats with: beached wreck clusters (many
   with squatter shacks aboard — dressing gold), stilt-pier shanty villages linked by plank
   causeways (W + SE), **glowing green tide pools** (NEW MOTIF — adopted: hazard-tag `acid`
   dressing + emissive decal machinery; they light the flats for night flyover), derelict
   cranes.
6. **Beyond (NE horizon): the gate pillar ring** standing in open water in the haze —
   visible from altitude and from the flats; ties every wide shot to the Ziptide.

**Layout data translation (CityLayoutDefinition):** radial-ring graph — center POI (tower) ·
ring canal + 3–4 radial canals · 4–6 district wedges · harbor wedge with breakwater · sea
wall ring with 2 breaches · outskirt scatter bands (wrecks/stilts/pools) · gate ring as a
far-anchor vista object, NOT geometry.

## §1a — THE LOGIC LAYER (K7 schematic blueprint — added 2026-07-20, same-day)

`city_K7_schematic_blueprint_v1.png` — an isometric SCHEMATIC of the city with gameplay
semantics; K1 is the LOOK, K7 is the LOGIC. Adopted reads (garbled AI text ignored where
noted):
- **Sector lettering A–G:** A-1 central leaning tower · A/B/C dense habitation wedges ·
  D/E the named-wreck outskirt sectors · industrial/sea-wall sector · **G = the ancient
  stone circle (the gate pillars) with an "anomalous energy reading" caution — the
  blueprint's own fiction pointing at the Ziptide.** Sector ids slot straight into
  CityLayoutDefinition district naming.
- **NAMED WRECKS adopted:** bulk carrier "RUSTBUCKET", trawler "SEAWEED" — wreck-thread
  device instances (each gets its log + salvage node per canon); naming wrecks is now the
  pattern for all outskirt wrecks.
- **The KEY maps 1:1 onto our systems** (adopt as the city's POI vocabulary): light relay →
  practicals · water filter point / power hub → RepairableMachine job sites · unstable
  structure ⚠ → hazard/no-build volumes · loot point → salvage nodes.
- **Pathfinding legend:** player route (green) = the cairn route; safe zones = spawn/rest
  areas; hatched **toxic spillage zones** = the acid-hazard volumes (pairs with K5's glowing
  tide pools).
- **"THE MOSS HABITATION ZONE"** — the sheet names the city. ⚖ Terry: adopt "the Moss" as
  the city/district's proper name? (W001 is "Toxic Venice" informally; the Moss reads
  in-world and kid-sayable.) Pending his call.
- Waterways numbered 1–5; central moorings/transport hub + external freighter docks confirm
  the harbor's two-tier use (player berth vs. big docks — flyover landmarks).

## §2 — SKYLINE & PALETTE (K2/K5)

Layered depth exactly per the Prospect rubric: foreground flats+wrecks → stilt fringe →
shanty mass (warm window/lantern specks, 2–3 warmths) → industrial mid-towers → THE tower
dominant ~2× any neighbor → green haze bands → pillars far. Palette: mud greys/browns +
concrete bone + rust + toxic-green haze and water + warm amber windows/lanterns + rare
cyan glints (tide pools/gate). K2's aircraft = OUR SHIP in the real shot (the W000→W001
arrival frame, Director's Cut); K5's sunset variant proves the skyline works in a second
lighting act (LS-5 evidence).

## §2a — THE SKYSCAPE (keepers 2026-07-20, `concepts/skyscape_moss/`) — the Prospect bar, met

Two keepers; **`sky_giant_dusk_hero_v1.png` is THE reference** — it passes the
`SKYSCAPE_DESIGN.md` rubric on sight: the banded giant fills half the sky with its lower edge
DISSOLVING into green haze and dark cloud banks crossing IN FRONT of the disc (not pasted-on);
rings sweep low catching the amber dusk band; one small moon high; the sky's green/amber
visibly stains clouds, water, and rooftops; the gate pillars sit on the sea horizon directly
beneath the giant (scale anchor + story anchor in one). `sky_panorama_dusk_v1.png` is the
wide/panoramic companion (near-equirect feel — candidate DIRECT reference for the dome bake's
composition).

**Vista translation (for the reserved ToxicCity `SkyVistaDefinition` when its time comes):**
- Gradient: deep green-teal zenith → green haze band → warm amber horizon strip.
- Giant: azimuth over the sea (SE of the berth), size class MAX, band tints cream/rust/amber,
  shallow ring angle, LOW elevation so city haze eats its bottom edge — the occlusion comes
  free from our fog derivation + dome layering, no trickery needed.
- **Body budget note:** the panorama shows ~6 moons; the vista baseline is ≤3 bodies — the
  bake picks the giant + 1 pale moon (+1 optional). Concept vibe survives the cap.
- Light script derives per F3.1: key from the amber horizon band (warm, low), fog = the green
  haze color — the images demonstrate exactly the sky-stains-ground behavior the deriver
  produces.
- **✅ NIGHT ACT delivered same-day (`sky_night_hero_v1.png`)** — the darkest-grade
  reference, and it PASSES the readability floor (silhouettes legible, warm lanterns vs.
  cool sky). Vista night-variant translation: dense unfamiliar star field + shimmer field ON
  (the SkyVista shimmer channel's reference) + the giant as a dark disc with lit crescent
  edge and rings — **the RING-ARC is the night sky's landmark** + one pale moon + aurora
  haze bands. **NEW MOTIF ADOPTED (emergent from the keeper): THE TIDE GLOWS AT NIGHT** —
  the horizon line carries a cold cyan luminous band (pillars silhouetted against it),
  unifying the night-glow family: horizon band + K5 tide pools + the skiff's wake = the
  Ziptide faintly visible everywhere after dark. Night is the tide's hour. (Feeds: night
  grade, LS-5 act data, the gate's ambient presence, and the skiff's "how does it read at
  night" answer — all one idea.)

## §3 — STREET LEVEL (K3 — the mood target for Stage A)

Canal canyon: 3-story leaning tenements BOTH sides · wet stone quay one side with working
clutter (nets, rope coils, barrels, crates) · lantern poles (~8 m rhythm — matches F3.1b
practicals!) · footbridge overhead with laundry lines · window warmths mixed (amber/orange,
2–3 temps — matches the 3-warmth window plan) · haze swallows the street at ~60 m (our fog
derivation does this for free) · a small boat moored (skiff family). **Stage A's verdict
test: a booth/scene capture of the built street should read like K3 squinted.**

## §4 — THE BUILDING KIT (K4 — module grammar, measured)

Six facades, one construction language: **old concrete frame + corrugated patch infill** —
base (door level, stone/concrete, numbered doors, small awnings) / middle (2–3 floors,
mismatched windows, patch panels, wires) / cap (rooftop shack + water tank + dish/aerial).
Widths ~1 room; heights 3–5 floors varied; amber accents sparse. **ADOPTED from the sheet:
NUMBERED DOORS** — big painted door numerals as the city's wayfinding vocabulary
(kid-legible, localization-free, pure signage machinery). **NOISE FLAGS (exclude at module
build):** the sheet's real-world script (Bengali shop signs) violates the alien-glyph
readables law — glyph family replaces it; also ignore the human figure + cat (the cat is
charming; a city cat CREATURE is a backlog thought, not canon).

## §5 — THE BERTH (K6 — three birds, one image)

Composition: heavy quay stone + bollards/chains foreground · dockmaster shack (C6 DELIVERED
— warm lamp, plank construction, built into the quay edge) · THE SCRAPPER moored, claw
deployed toward the water, cyan port live — **model consistency held across generations**,
so K6 = the title-menu berth scene reference AND the first-hour opening stage AND C6, in one
keeper. Floodlight rigs on poles (2×2 warm — matches the ship's light family). City rises
behind in haze layers.

## §6 — BONUS SHEET (ship context 4-panel + CAB POV)

Rear water view (engine quincunx LIT warm — the thrust-glow reference) · elevated frontal ·
opposite-side dock view · **CAB POV**: dash with small screens/toggles, wiper, side mirror,
grime-edged glass looking onto the dock — the S4 cockpit interior's framing/mood reference
(a dedicated S4 interior sheet is still worth one more generation pass for panel detail).

## §7 — Open items

⚖ adopt "numbered doors" as official city wayfinding vocab (recommended) · glyph family to
replace the sheet's real-world text (F3.7 signage lane) · K1's district count final call at
layout-data time (5 vs 6) · S4 dedicated cockpit sheet + S5 quarters still open · glowing
tide pools boarded as an outskirts dressing motif (acid-hazard family).
