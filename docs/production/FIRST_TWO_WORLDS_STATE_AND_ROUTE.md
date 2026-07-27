# FIRST TWO WORLDS — where we actually are, and the route to a city that looks like the concept art

**Written 2026-07-26 (T-Dog lane) for Terry's question:** *"the first city's layout doesn't really
make sense and it's super ugly… we have concept art — what's the plan to translate that into the
real cityscape? Where are we at for level 1 and 2 and how should we proceed?"*
Companions (all already exist — this doc routes them, it does not replace them):
`project_art_plan/CONCEPT_TO_BUILT_PIPELINE.md` · `project_art_plan/CITY_VISUAL_SPEC.md` ·
`production/POST_HEADSET_WORLD_FACTORY_ORDER.md` · `audio/AUDIO_PRODUCTION_MASTER_PLAN.md`.

---

## 1. The headline

**The plan you're asking for already exists, is approved, and is unusually specific. The gap is
execution, not design.** The city you are looking at in the headset is the *pre-concept procedural
graybox* — it was generated before your concept kit was approved, and nothing has yet rebuilt it
against that kit. Every lane below is in the same shape: specified well, built thinly, and
deliberately queued behind M0.

## 2. Why the first city looks wrong (specific, not vague)

`CITY_VISUAL_SPEC.md` (🟢 REFERENCE APPROVED, measured off your own K1–K6 sheets) defines the city
as **concentric rings on a drowned tidal flat**: the leaning scrap Tower island at centre → dense
shanty wedges cut by a canal ring and radial canals → a breached sea wall → a harbour wedge with
double breakwater and the moored Scrapper → flyable outskirts of mud flats, beached wrecks, stilt
villages and glowing green tide pools → the gate pillar ring on the NE horizon.

**None of that shape is in the built city.** `CityBuilder.cs` (~550 lines) still generates the
older boulevard/side-street layout. The spec names `CityBuilder Stage A` as its consumer, and the
handoff record is explicit: *"City Stage A remains a separate later bridge"* — i.e. the bridge doc
pass and the code pass are both **unstarted**. That is the entire explanation for "doesn't make
sense and is ugly": you are seeing a layout with no authored intent, lit and dressed generically.

## 3. State by lane (level 1 = ToxicCity/W001, level 2 = W002)

| Lane | Level 1 (ToxicCity) | Level 2 (W002) | Honest state |
|---|---|---|---|
| **City layout/look** | procedural graybox; spec approved, unbuilt | generated graybox | 🔴 **the biggest visible gap** — Stage A is the single highest-impact unbuilt item |
| **Concept→built machinery** | route proven twice (ship spec, city spec = step 1 solved) | same | 🟢 pipeline exists; steps 3–5 need the booth loop to run |
| **Buildings/interiors** | kit + interiors exist, generic | same | 🟡 module recipes for the K4 facade grammar unbuilt |
| **Skyscape** | canonical layers v1; K2 vista sheet approved | inherits | 🟡 the Prospect bar is specified; signature pass + device verdict pending |
| **SFX / audio** | ambience beds procedural; **1 music track, 0 VO, no SFX library, no mix pass** | same | 🔴 thinnest lane in the game; master plan + 34-row asset queue written and ready |
| **First hour (W000→W001)** | 22-beat contract + gates green; FH-S05 creature, FH-A01 art, FH-S08 orchestration unbuilt | n/a | 🟡 skeleton complete, payoff beats missing |
| **Space flight** | flight runtime v1.3 + space-combat core; **0 ship assets**; 10 missions/6 POIs as validated data | n/a | 🟡 code ahead of content; SunRig contract written |
| **Creatures** | 7 species w/ behaviour; **only 2 have Forge bodies** | same | 🟡 5 render as fallbacks |

## 4. The route (do these in this order — it is already canon, just unstarted)

**Gate A — M0 passes.** Everything below is explicitly queued behind it
(`POST_HEADSET_WORLD_FACTORY_ORDER.md` §2). Do not start world work from a red baseline.

**Then, in order:**

1. **Bridge the city spec into a recipe (paper, ~1 session).** Turn `CITY_VISUAL_SPEC.md` §1–§4
   into the Stage A `CityBuilder` recipe: ring radii, wedge counts, canal widths, causeway points,
   wall breach positions, harbour geometry. This is step 2 of the concept→built route and the
   cheapest thing on this list.
2. **Build Stage A + the booth reference-plate.** Implement the recipe, and add the small tooling
   win the pipeline doc already recommends: **render the built result side-by-side with your keeper
   sheet in one comparison image.** The acceptance test is written in the spec — *"the built street
   should read like K3 squinted."* This is what converts "ugly" into "authored."
3. **Facade grammar as module recipes (K4)** — the building kit stops being generic boxes.
4. **Skyscape signature pass for W001 (K2)** — the vista and arrival flight are what sell scale;
   your own note is that the skyscape is why you started this game.
5. **Audio rails, then the first paid SFX batch.** The master plan's order is right: build the
   mixer/bus/event-ID rails first, *then* buy sound. Buying before the event list is stable wastes
   the month.
6. **Close the first hour's payoff beats** (FH-A01 → FH-S05 → FH-S08) so W000→W001 is a complete
   experience rather than a working skeleton.
7. **W002 as the replication test** — rebuild nothing by hand; if W002 needs special-case code, the
   factory isn't real yet (`POST_HEADSET_WORLD_FACTORY_ORDER.md` §6).

**Paid-asset timing (already decided, worth restating):** the 3D month starts at the W002 gate, not
before — the import/rig/LOD/collider/provenance path must be locked first. The audio month starts
after the event rails are stable.

## 5. The one thing to internalise

The project's failure mode has never been missing plans — it is that **specification outruns
execution, and the gap is invisible until the headset**. The city is the clearest example: a
measured, approved, image-backed spec sitting beside a graybox nobody has rebuilt. The fix is not
more design. It is running the concept→built loop (§4.2) once, end to end, with the sheet pinned
next to the render — and then repeating it.
