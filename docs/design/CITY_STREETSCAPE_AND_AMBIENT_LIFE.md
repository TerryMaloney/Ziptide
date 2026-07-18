# CITY STREETSCAPE & AMBIENT LIFE — the research
### What a believable downtown is actually made of, why ours reads like a toy, and the staged recipe upgrade
**Status: RESEARCH / PLAN ONLY — no code, no claims. 2026-07-18, Terry-directed.**
Owner routing: streetscape recipe = city/worlds lane (CityBuilder is shared infrastructure);
surface/material quality = Picasso's Forge programs; near-field NPCs = **unowned, new design
space** (Forge V's ambient society is distant-only by its own law — see §6). Build claims need a
HANDOFF entry per normal rules; nothing here is authorized to start until Terry sequences it.

Companion docs: `docs/project_art_plan/W001_TOXIC_VENICE_ART_BRIEF.md` (the W001 vibe target) ·
`QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md` (every number here respects it) ·
`FORGE_V_LIVING_STAGE.md` §5 (distant ambient society — we slot UNDER it, not against it) ·
`docs/design/FIRST_HOUR_DIRECTORS_CUT.md` (the beats this streetscape must serve).

---

## 1 · HONEST BASELINE — what the recipe builds today (verified in `CityBuilder.cs`, 2026-07-18)

Reading the actual builder, a Ziptide "building" today is:

- **One stretched cube** (`Cube(...)`, 3.5–5.5m wide, 4–13m tall), flat-colored from a
  2-color alternation (`building1`/`building2`).
- **Fake windows**: small inset dark cubes scattered on ONE face at 65% fill.
- **No ground floor, no door, no roofline** — the cube meets the slab raw and ends flat.
- **Ground = one big slab** per district. No curbs, no sidewalk/road distinction, no edges.
- Districts are rectangles ringed by these cubes with 25% random gaps as "streets."
- Heroes (Dispatch Hall, Relay Vault) are hollow boxes with a door gap. Props are scattered
  primitive clusters. Skyline is a ring of 26 silhouette boxes. Canals are emissive slabs.

**Why this reads "4-year-old in a game editor," precisely:** every failure is the *same*
failure — **uniformity**. Same footprint, same material everywhere, same window, same
roofline, same street width, no asymmetry, nothing at eye level, nothing moving, nobody home.
The human brain reads cities statistically; ours has the statistics of a warehouse of boxes.
This is fixable at the *recipe* level without abandoning primitives-first — composition, not
assets, is most of the gap. (Half-Life: Alyx's most-praised streets are mostly simple geometry
carrying dense *storytelling*; Walkabout Mini Golf reads as beautiful *places* with almost no
geometry detail at all — lighting, palette, silhouette and staging do the work.)

---

## 2 · THE RESEARCH — anatomy of a downtown that reads as real

Distilled from urban design (Kevin Lynch's *Image of the City*, classical facade theory, street
section practice) and two decades of game environment-art craft. Ranked roughly by
read-per-triangle on Quest.

### 2.1 The city-scale skeleton — Lynch's five elements
A legible city is made of: **paths** (streets with a hierarchy — avenue vs street vs alley),
**edges** (canal fronts, walls, elevation changes), **districts** (character zones — we have
these), **nodes** (plazas/intersections where paths meet and people gather), and **landmarks**
(tall/unique, visible from many places, used to navigate). *Our gaps:* all streets are one
width; nodes don't exist (no plaza with a center — a fountain, a statue, a market ring);
landmarks exist but aren't visible DOWN streets (no sightline discipline). **The cheapest big
win in this whole doc: aim streets AT landmarks.** A street that frames the Relay Vault's
tower at its far end turns a corridor into a composition, for zero new objects.

### 2.2 The facade grammar — base / middle / cap
Real buildings worldwide follow a three-part vertical grammar, and its absence is the #1
"toy building" tell:
- **BASE (ground floor, ~4–5m — taller than other floors):** distinct material, storefront
  or entrance, awning/canopy, signage, address. In VR this is ~90% of what the player ever
  looks at — eye height is 1.7m; the first 8m of facade is the game.
- **MIDDLE:** repeating window floors — fine to keep cheap and regular.
- **CAP:** parapet lip, cornice shadow line, roofline variation, roof clutter (tanks, AC
  boxes, antennas, vents). Rooftop clutter matters doubly for us: ziplines and verticality
  put players ABOVE the city.
*Recipe translation:* a "building" becomes a 4–7 cube composite instead of 1 cube: base slab
(different color, slightly proud), door/storefront inset, awning quad, body, parapet lip,
1–2 roof boxes. Still primitives. Reads ×10.

### 2.3 Massing variation
Vary footprint (L/T/stepped plans — two overlapping cubes suffice), height by *zone logic*
(taller near nodes, lower on alleys), and setbacks (upper floors stepped back). Rule of thumb
from city-builder art: within one street wall, ~70% conforming + ~30% breaking (one taller,
one recessed, one narrow) reads "grew over time" instead of "placed by loop."

### 2.4 The street section
Downtown canyon feel = height-to-width ratio between 1:1 and 3:1 (our tallest 13m cube on a
7m street is barely 2:1 — okay, but heights must vary). A street is layered across its width:
**building face → 0.5m frontage strip (steps, crates, planters) → sidewalk → curb (a 10cm
lip!) → road/canal.** The curb alone — one long thin cube — makes ground read as *street*
instead of *floor*. Alleys (1.5–2.5m, dark, cluttered, steam) between 30% of buildings give
the street wall depth and give players secrets to peek into.

### 2.5 The street-furniture layer (the "inhabited" signal)
In order of read-strength for a dense toxic city: **overhead catenary wires and cables**
(sagging lines between facades — thin stretched cubes; nothing says "dense old city" harder
per triangle) · street lamps with emissive heads + a warm decal pool (fake light, honoring
the 1-directional-light law) · **vertical blade signs** (the Kowloon/noir read — quad +
emissive trim, 2–3 per street) · awnings/canopies · bollards, benches, hydrant-analogues ·
crates/barrels in recesses (we have props — they need to hug walls and corners, not scatter
mid-street) · wall greebles on blank faces: pipe runs, conduit, AC boxes, vents, fire-escape
stubs.

### 2.6 Light hierarchy (the biggest "place" multiplier we already have tools for)
A city at dusk is read through its lights: **lit windows in 3 warmths** (home-warm, shop-cool,
dead-dark — currently ours are all one dark color; making ~30% of window cubes emissive with
varied warmth is a one-function change) · storefront glow spilling on the sidewalk (emissive
window + warm decal quad on ground) · blade-sign neon accents in the district accent color ·
canal underglow (have it) · ONE flickering sign per district (motion, §2.8). All emissive
trims — zero real lights — per the budget doc's own recommendation.

### 2.7 Weathering, story, and asymmetry
Grime decals at rain lines and door corners, posters/notices (quads with the oligarch glyph
decal set the art plan already defines), hazard striping at canal edges (toxic-city native),
one boarded-up storefront per street, tarps. The rule: **damage and dirt are never uniform**
— streaks run DOWN, grime pools LOW, posters cluster at eye height near doors. This is where
Toxic City's fiction shows: respirator culture, sealed lower windows, moss/corrosion at the
waterline, warning placards near canals.

### 2.8 Motion — the single strongest "alive" signal
A static world reads as a diorama no matter how detailed. Cheap motions, each a tiny
scripted transform or particle within the ≤4-active-systems budget: steam vents (1–2 per
district) · hanging cables/laundry/flags that sway (sin-wave rotation on a hinge — no cloth
sim) · sign flicker (emission keyframe) · canal drips/ripples · drifting ash motes (exists) ·
distant aircraft lights crossing the skyline (Forge V-style far mover, position=f(time)).
Three moving things per street view is the target; zero is what we have outside drones.

### 2.9 Sound as architecture
Alleys hum close and tight; plazas open up; the canal laps; a market murmurs even when
stalls are botted (crowd-murmur loop at low volume with no visible crowd is a standard and
surprisingly convincing trick). Declared per-district via the existing `AmbienceLayer`
pattern, ≤8-voice budget. The Dispatch Hall should sound *occupied* (PA murmur, stamping,
a fan) before we ever put a body in it.

### 2.10 The skyline (have it — needs layers)
One ring of boxes reads as a wall. Two rings at different radii + fog between them +
sprinkled lit windows (emissive dots) + 2–3 unique silhouettes (a tilted crane, a spire —
the Toxic Venice brief's oligarch tower and stone tower belong here) reads as a metropolis.

---

## 3 · WHY IT'LL RUN — Quest budget accounting

The entire §2 program is composed of: more cubes (composited buildings ~×4–6 object count on
facades), thin cubes (wires, curbs, pipes), quads (signs, awnings, decals, posters), emissive
material variants, 2–3 new tiny scripted motions, and audio layers. Budget math against
`QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md`:
- **Draw calls** are the real constraint (≤120 target). Answer: shared materials via the
  existing `Mat()` cache (grammar adds ~6 new palette slots, not per-object materials) +
  **static batching / GPU instancing on the generated city root** — identical primitive cubes
  sharing materials batch extremely well. The scene-object cap (≤600 target / 1000 hard) is
  the number to *audit per district* — composite buildings spend it fast; the builder gets a
  per-district object budget constant and the audit enforces it (belt-gate idiom).
- **Zero new real-time lights** (all glow = emissive), **zero transparency** (fake glass
  law), **particles stay ≤4 active** (steam vents count), **audio ≤8 voices**.
- Windows change from N cubes to fewer, larger emissive-atlas faces where density is wanted
  — the window *count* is already the biggest object spender in the scene (309 today).

---

## 4 · AMBIENT LIFE & NPCs — the research

### 4.1 The problem, honestly stated
An empty downtown isn't neutral — it actively reads as *wrong* (liminal-space effect: the
brain flags inhabited-looking spaces with no inhabitants as threatening/uncanny). Terry's
instinct is correct and it's one of the oldest findings in environment design: **you can't
set-dress your way out of emptiness; something has to move and notice you.**

### 4.2 What the fiction already gives us (use it)
Toxic City's own premise — toxic air, respirator culture, wealth stratified upward — is a
*canonical reason streets are machine-tended and thinly peopled*. The honest citizens are
indoors or up-tier; the streets belong to machines, the desperate, and dock workers. This
turns our budget constraint into world-building: a street with 2 humans and 5 working
machines is ON-fiction, not under-budget.

### 4.3 The four tiers, ranked by presence-per-cost (VR-specific)
1. **Working machines (cheapest, fits tools we HAVE):** civic sweeper drones tracing slow
   sidewalk routes, cargo drones on an overhead lane, a stall-keeper bot per market stall
   (stationary, head-yaws to face the player — "noticing" is 80% of presence), a canal skiff
   chugging a loop. Mostly `DroneRuntime` variants + baked paths (position=f(time), Forge V's
   own mover math brought near — but as *gameplay-lane citizens*, not Forge V ambience, so
   the freedom contract and ecology ownership stay clean).
2. **Stationary posted humans (biggest single win: THE DOCKMASTER):** the first-hour story
   has a Dockmaster with a kiosk — today the kiosk is unmanned. A posted figure — robed
   silhouette (a cloak/poncho hides the leg problem entirely and is respirator-culture
   native), simple idle bob + head look-at, subtitle barks via the existing RILL/SubtitleText
   path — turns "menu on a pole" into "a person gave me a job." 2–4 posted figures citywide:
   Dockmaster, a stall vendor, a shipyard mechanic leaning on a crate.
   **VR craft notes:** never let an NPC face-track continuously (glance, then look away —
   permanent eye-lock is the uncanny thing); eye-height ~1.6–1.75m; keep them OUT of the
   player's personal space (VR proxemics: <1m from the face feels like an assault); no
   facial animation needed at all — helmets/respirators/hoods are the genre's gift.
3. **A few walking pedestrians (the "someone lives here" proof):** 3–6 cloaked walkers on
   baked sidewalk loops with 2-3 pause points (look at a stall, look at the canal), simple
   sidestep-and-bark if the player blocks (never pathfinding — swap to the next baked lane).
   Bob-and-lean walk cycle on a legless robed silhouette = zero skeletal animation. This is
   the tier where cost/risk genuinely rises (they can be touched, shot, blocked — every
   interaction needs an answer, even if the answer is "it shuffles off"), which is why it's
   tier 3 not tier 1.
4. **Distant crowd illusions (Forge V §5 LS-4, already specced):** silhouette movers on
   elevated walkways/far canals, light-crawl windows. When its gate opens, it composes with
   tiers 1–3 into depth: near-machines → few near-humans → distant many.

### 4.4 What we do NOT build (standing rejections, so nobody burns a lane on them)
Full crowd simulation · skeletal humanoids with walk-cycle animation sets · facial
animation/lip sync · NPC schedules/needs · conversational NPCs (dialogue stays authored
barks + contracts; Vex Bootstrapper stays a signature, per the Director's Cut) · anything
that can shove, grab, or block the player (freedom contract).

---

## 5 · THE STAGED RECIPE UPGRADE (proposal — Terry sequences, lanes claim)

Each stage is CityBuilder/CityLayoutDefinition data + builder functions + an audit rule —
the factory stays, the recipe grows. Every stage is independently shippable and
device-checkable, and honestly none of them block on the others.

- **Stage A — the building grammar (biggest visual jump):** composite base/middle/cap
  buildings, footprint/height variation rules, 3-warmth emissive windows, parapet + roof
  clutter, palette widened to ~8 slots incl. trim + accent. *Audit: per-district object
  budget, material count.*
- **Stage B — the street layer:** curbs, frontage strips, overhead wires, lamps with fake
  pools, blade signs + awnings, alley carve-ins, prop placement rules (hug walls/corners),
  grime/poster/hazard decal quads, street-aims-at-landmark pass, plaza node kit (center
  piece + bench ring). 
- **Stage C — motion & sound:** steam vents, swaying hung items, sign flicker, skyline
  second ring + lit windows + crossing lights, per-district ambience layers incl. interior
  murmur for Dispatch Hall.
- **Stage D — ambient life tier 1+2:** machine citizens (sweeper/cargo/skiff/stall-bots) +
  posted figures (Dockmaster first — he's a first-hour story beat already). 
- **Stage E — ambient life tier 3:** the 3–6 walkers, only after D proves the read.

Rough calibration: A+B together are what move the verdict from "game editor" to "place";
C is what moves it from "place" to "alive"; D is what moves it from "alive" to "inhabited."

---

## 6 · LANE & LAW NOTES
- Forge V LS-4 (distant society) is UNTOUCHED by this doc and its "never inside P2 distance"
  law stays absolute for *its* lanes. Near citizens here are gameplay-lane actors (like
  drones/creatures today), not Forge ambience — different owner, different rules, and they
  must never gate input or shove the player (comfort laws bind them too).
- The primitive-first law of the Toxic Venice brief holds: every element above lands as
  tinted primitives behind kit IDs first, upgradeable to real meshes later without renaming.
- Nothing here starts before Terry sequences it against the first-hour work; the golden
  checkpoint + code-hardening ladder stay the gate for ALL of it.

## 7 · ACCEPTANCE (how we'll know)
Headset test, one question per stage: A/B — *"walk one street: does it feel like a street in
a city, or a corridor between boxes?"* C — *"stand still 30 seconds: does the city keep
happening without you?"* D — *"take the Dockmaster's job: did a PERSON give it to you?"*
Plus the standing 72 FPS floor and the §3 budget audits in CI.
