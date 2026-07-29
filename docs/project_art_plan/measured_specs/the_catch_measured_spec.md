# MEASURED SPEC — THE CATCH (keepers approved 2026-07-29)

Numbers pulled off the approved keeper images so the Forge proxy is a *reproduction*, not an
impression. Source: PROMPT_TEST_09 generations, all five accepted first pass.
Governed by `CONCEPT_TO_BUILT_PIPELINE.md`; consumed by `ScenePatcherSpaceLane` + `CityBuilder`.

**Provenance note:** generated `internal_reference` art. Stencilled strings visible in the keepers
(`CP-0974`, `SERVICER-9`, `HULL ZONE-3`, `P/N 99`, `P/N 600`, `SECTION A-12`, `M-DRONE S/N 40`,
`MINERAL FREIGHT TARGET ORE`) are **adopted as canon nomenclature** — they are consistent with the
house dialect and cost nothing to keep. Generated glyphs are not otherwise authoritative.

---

## 1 · `catch_ring_mk1`

| Feature | Measured | Notes |
|---|---|---|
| Bore (clear inner opening) | **12 m** | four cargo pods wide — the scale anchor |
| Outer truss diameter | ~17 m | truss band is ~2.5 m deep radially |
| Structure | **DOUBLE ring** | outer deep truss ring with X cross-bracing + inner smooth rim band. *This is the keeper's biggest departure from my prompt and it is better — the inner band gives the lamps a home and reads as a machined running surface.* |
| Coil housings | **16**, on the inner face | ribbed casings, blocky, long axis tangential |
| Radiator fins | **16**, on the outer rim | flat rectangles standing radially outward, ~3 m long, heat-darkened at the outer end only |
| Sequencer lamps | ~40 small amber dots | on the **inner rim band**, evenly spaced — small |
| Numeral collar | one flat plate, bottom of ring | large numeral + yellow/black chevrons below it |
| Service spar | one boom, lower right | thin mast with a small array at its end |
| Damage (CATCH 3 only) | one coil housing scorched black | the orthographic view shows a second darkened housing — read as heat-staining, not a second failure |
| Palette | warm grey-brown alloy / tan bronze; off-white collar; amber lamps | **no cool greys** — the ring is warmer than my procedural v1 |
| Depth | shallow — ring depth ≈ 1/6 of bore | it is a hoop, not a tunnel |

## 2 · `cargo_pod_mk1`

| Feature | Measured | Notes |
|---|---|---|
| Length | **~3 m** | one quarter of the ring bore ✓ |
| Form | double-tapered capsule | tapered nose cone + cylindrical mid + tapered tail |
| Drive bands | **TWO black collars** at the waist | *departure from my prompt (I said one)* — a pair reads better and implies the track grips twice |
| Stencil | `CP-0974` + two chevron blocks | pod serial convention: `CP-####` |
| Transponder | one small amber lamp near the nose | |
| End cap | circular hatch, cross-pattern doors, `CARGO ACCESS` | radial ribs around it |
| Burst state | splits **at the waist**, aft section crumpled | ore + dust plume trailing |
| Palette | galvanised silver-grey, warm rust staining | brighter than the ring — freight is newer than infrastructure |

## 3 · `ring_tender_drone_mk1`

| Feature | Measured | Notes |
|---|---|---|
| Body | rounded box, ~1.6 m | soft-cornered cube — *far friendlier than my prompt implied, and correct: this thing is bureaucratic, not hostile* |
| Sensor eye | ONE, large, centred, circular | dark when dormant → **red-orange glow** when woken |
| Indicator lamps | 2 small amber, flanking the eye | |
| Stencil | `SERVICER-9` across the body face | unit-number convention: `SERVICER-#` |
| Arms | **2**, multi-jointed, clamp ends | folded flat against the flanks when dormant, deployed when woken |
| Side pods | 2 rounded housings on the flanks | thruster/reaction mass |
| Evading | cold-gas puffs from 3–4 points | white, brief |
| Disabled | tipped on its axis, eye dark, **access panel hangs open** exposing a green board + red/yellow wiring | this is the salvage read |
| Palette | pale bone-grey/tan body, dark grey arms | amber + red-orange are the ONLY emissives |

## 4 · `orbital_debris_kit` — 8 typed pieces

| # | Piece | Canon string | Size |
|---|---|---|---|
| 1 | truss section, torn both ends | `SECTION A-12` | ~6 m |
| 2 | magnetic arrestor unit, copper windings spilling | `MAGNETIC ARRESTOR UNIT` | ~1.5 m |
| 3 | hull plate torn in two | `HULL ZONE-3 DEEP SPACE P/N 99` | ~3 m |
| 4 | crushed pod end cap | — | ~1.2 m |
| 5 | severed drone manipulator arm | `M-DRONE S/N 40` | ~2 m |
| 6 | tangled conduit bundle | — | ~1.5 m |
| 7 | burst freight container spilling black ore | `MINERAL FREIGHT TARGET ORE P/N 600` | ~6 m |
| 8 | bent radiator fin, buckled | — | ~3 m |

**Key read:** every piece is a broken part of something else in this pack. Copper is the only
non-grey colour in the kit and it appears twice (2, 5) — that is the "torn open" tell.

## 5 · `the_throw_mass_driver`

| Feature | Measured | Notes |
|---|---|---|
| Track | long shallow rise on **repeated piers**, climbing right | reaches a ridge, not a tower |
| Running lights | **one continuous amber line** the full length | this is the hero read at distance and it must be unbroken |
| Coil houses | ~4 lit boxy structures at intervals along the track | the same amber |
| Terminus | heavy gantry on the ridge crest | |
| Foreground | harbour cranes + stilt shanties with warm interior lights | already in the city's vocabulary |
| Sky | heavy amber haze; **ringed giant prominent, centre-left, low** | matches the Moss celestial canon |
| Water | green-brown, reflective, tidal flats | matches ToxicCity palette |

**⚖ This keeper is the strongest of the five and it validates the whole idea:** the Throw reads as
the reason the town exists. Terry: *"maybe one of the most unique things we have in our game so far…
it's really starting to become an actual world not just recycled sci-fi."*

---

## 6 · THE LIGHTING LAW (⚖ Terry, 2026-07-29)

> *"whatever the sun direction is, that's the direction the lighting should come from… the sun is
> essentially more or less at our back so the objects are lit up ahead of us, at least to a degree,
> maybe a little bit of shadow."*

**The rule:** in the space lane the sun sits **behind and above the pilot's shoulder**, so everything
ahead is *lit*, not silhouetted. Flying into a black sun-side corridor would waste every one of
these keepers.

Implementation contract:
- the Moss-orbit vista's `SunDisc` bearing is **(0.18, 0.62, −0.76)** — up and BEHIND, since the
  corridor runs +Z. That was already correct and is now load-bearing, so it is pinned by a test.
- the lane's directional light **must be derived from that bearing**, not hand-authored: light
  travels along **−sunDirection = (−0.18, −0.62, 0.76)**. Previously the patcher hard-coded
  `Euler(35, −30, 0)`, which pointed somewhere unrelated to the sun in the sky — the exact
  inconsistency Terry spotted.
- one key light, hard, warm; deep shadows; a dim cool ambient so unlit faces read as material
  rather than as holes.
