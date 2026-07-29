# LEVEL 1 — SPATIAL SCRIPT: where everything IS, how far, what between, how you find your way
### Terry 2026-07-29: "placement of everything, how far apart, all the little stuff in between, how much space to explore, how do we know where we're going, are the rings there immediately, what do they look like, how do the droids act." This is that document — the level-design layer the summaries skipped.

**Status legend per line — the honesty layer this doc exists for:**
- **[M]** MEASURED — this number is in committed code/data today (cited).
- **[∅]** EMPTY — the committed data block for this is literally empty; nothing places it.
- **[S]** SPEC'D HERE — designed in this doc (grounded in the approved K1/K7 city spec, the
  Director's Cut, and VR metrics: walk ≈ 2 m/s, comfortable landmark sightline ≥ 8° tall).
  **[S] rows are the build orders** — the generators get extended until every [S] becomes [M].

Sources: `ScenePatcherSpaceLane.cs` · `FlightModel.cs` · `SpaceTargetRuntime.cs` ·
`ToxicCityLayout.asset` (committed) · `CITY_VISUAL_SPEC.md` (K1/K7 measured concepts) ·
`FIRST_HOUR_DIRECTORS_CUT.md` v2.1 · `FirstHourSurfaceAuthor.cs`.

---

## 1 · W000 — THE SHIP INTERIOR (min 0–13)

The whole playable space is the junker's interior: **a ~6×10 m two-room deck** — quarters aft,
work bay + helm forward. Small ON PURPOSE: the first ten minutes teach hands, not navigation.
- **[M]** Surfaces (comfort console, keepsake, observation adapter) are authored onto EXISTING
  anchors by `FirstHourSurfaceAuthor` (anchor-relative, not world coordinates).
- **[S] The eyeline chain does the wayfinding — no markers:** you wake facing the bunk (keepsake
  within 1 m, grab #1) → the doorway frames the work bay (coupler machine 4 m ahead, its fault
  lamp blinking amber — the only moving light) → repairing it lights the corridor strip that leads
  6 m to the helm ladder. Each task's END points at the next task's LIGHT. Nothing else is lit
  bright; the player cannot be lost in two rooms.
- **[S] The in-between:** quarters clutter (3–5 grabbable trinkets, bunk tags), the humming wall
  the artifact will later resonate against, ONE porthole with starfield (the "no space in a space
  game" fix, stand-in skybox decal). **[∅]** viewport, prior-owner log (rb121 deferred).

## 2 · SPACE LANE — THE SORTIE (min 13–19)

**The arena [M]:** a bounded corridor ~**90 m wide × 45 m tall × 420 m long** (soft `laneRadius`
wall). Player flies at **max 40 m/s, accel 12 m/s²** (`FlightParams.Default`); full length ≈ 15 s
at full throttle — but the RINGS bend the path, so the honest sortie is 3–4 minutes.

**Are the rings there immediately? YES [M]** — all five are static scene geometry, visible from
the dock the moment the streaks clear. No spawning, no sequence-reveal. Positions (lane frame,
z = meters downrange):
`R1 (0, 3, 60) · R2 (14, 7, 130) · R3 (−8, 12, 210) · R4 (−28, 6, 295) · R5 (0, 10, 380)`
— a gentle climb with two S-turns, all inside the 35° pitch clamp. Visual radius **6 m**, pass
radius 7 m (forgiving). **[S] look:** rusted salvage-buoy hoops — scaffold ring + 4 blinking
amber lamps; the NEXT ring in sequence runs its lamps chasing, passed rings go dim green. That
lamp-chase IS the wayfinding. **[∅]** ring sequencing lights (today all rings look identical —
stand-in prims; the "which ring is next" read does not exist yet).

**The drones [M behavior]:** three derelict security drones flank the course OFF the racing line:
`D1 (26, 9, 170) · D2 (−22, 14, 250) · D3 (−38, 8, 330)` — 20–30 m off-axis so pacifists simply
fly past. **How they act TODAY:** they hover-bob in place (±0.8 m sine), armor 6 that RECHARGES
between hits, and when disabled they power down, list to a 24° roll, and become salvage (6 scrap
via tractor range). **They do not pursue, aim, fire, or notice you.** **[S] v2 reaction layer
(the "act or react" Terry asked for):** on player approach <40 m the drone wakes — red eye lights,
bob quickens, a warning chirp; if fired on it strafes ±6 m along its home axis while recharging;
disabled = eye dies dark (the rogue-drone sheet's exact states). Never lethal, never chases past
its 60 m tether. **[∅]** all of that reaction layer.

**The wreck + THE FIND [M placed / [S] dressed]:** the breached hauler sits past R5 at
`≈ (0, 8, 405)` — `EnsureTheFind` stakes the artifact half there today. **[S] the moment:** you
must STOP (throttle to zero — the hover the design already supports) inside a 12 m debris shell;
among 8–10 tumbling scrap pieces exactly ONE doesn't scan (no outline, no name); grabbing it kills
the local hum for half a second. **[S] in-between texture:** 28 drift rocks [M, seeded] plus
**[∅]** two waker-log pings and the distant Moss limb + ringed giant on the skybox (celestial
match — today the backdrop is near-black + one flat planet).

**Wayfinding in space [S]:** three devices, no HUD — the ring lamp-chase (primary), RILL bearings
on hesitation >10 s ("port and up, Cal"), and the helm compass ribbon (the cockpit's one
instrument, pointing at the active objective). **[∅]** all three.

## 3 · TOXIC CITY — THE MOSS (min 19–45)

**What's committed TODAY [M]:** the outer shell only — skyline ring radius **250 m**, **26**
impostor towers **30–90 m** tall, toxic fog (density 0.018), amber-green sky, the banded planet at
22° angular size. **[∅] AND THIS IS THE HEADLINE:** the committed layout's `districts:`,
`canals:`, `connections:`, `creatureZones:`, `droneZones:`, `hazards:`, `pois:`, `shipyard:`,
`experience:` blocks are **EMPTY**. The playable city interior — the actual level design — has no
data. (The ring-city geometry commit built earthworks: tidal flat, sea wall, harbor basin,
outskirts; the DISTRICT layer inside it is the unbuilt part.)

**[S] THE PLAYABLE CITY PLAN (from the approved K1/K7 spec, scaled to the 250 m shell):**
- **Walkable core ≈ 140 m across** — 8–10 min to see everything at walk speed; the job route is
  a 400 m loop. Beyond it: the harbor (you), canals, then vista-only wedges to the skyline ring.
- **BERTH SIX (start/end):** harbor south edge, `(0, 0, −70)` city frame. Your ship IS the
  landmark home — its floodlights + the (later glowing) key socket visible from anywhere on the
  route. Berths 1–5 line the quay westward at 14 m spacing (the hangar walk's runway, rb121's
  deferred beat — the beacon thread descends them one by one).
- **THE QUAY (toy beat):** 60 m of dockfront east of berth six: crane, crates, the **taser on a
  barrel at (12, 0, −62)** with cans + the dummy drone 8 m further — you literally walk past the
  toy leaving the ship; nothing gates it. The Dockmaster's booth (paperweight half B inside, on
  the desk) closes the quay at (30, 0, −58).
- **CANAL ONE:** a **14 m-wide green canal** cuts east–west at z = −40, separating quay from
  city. **The zipline** spans it: platform at (18, 6, −52) — up one obvious stair — to Dispatch
  plaza (10, 2, −28). 40 m ride, ~6 s. Below, the water steams [hazard: standing in it drains,
  never kills]. The canal is why the zipline exists; the zipline is how you learn traversal.
- **DISPATCH PLAZA (job hub):** 30 m square at (0, 2, −20): the contract board + kiosk on the
  north face, objective board beside it. From the plaza's center **all three route landmarks are
  visible:** the RELAY MAST up-left (NW, red fault strobe), the LEANING TOWER dead north (the
  skyline anchor, 90 m, always orientation-north), your SHIP's floodlights behind you (south =
  home). That one sightline triple is the city's whole compass.
- **THE JOB LOOP (5 drones → creature → relay), ~400 m:** drones D1/D2 on Market Row (a 6 m
  street NW from the plaza, 60 m walk), D3 on the ring-walk bridge over Canal Two, D4/D5 flanking
  the relay yard gate. **The creature meets you ON the route** — the Husk-Molter's zone is the
  40 m colonnade between Market Row and the relay yard (observation window from behind the
  colonnade rail at 15 m; counter-verb taught by RILL cue). **THE RELAY** stands in a walled yard
  at (−45, 3, 25): scan → panel → part → power, the W000 ceremony again — and it reads WRONG
  (the resonance seed). Loop returns along Canal One's north walk → zipline back or the low
  bridge at (−20, 0, −40).
- **[S] in-between texture (the "little stuff"):** numbered doors (the adopted motif) on every
  facade; 3 tide-pool puddles that glow at night; hanging lanterns (amber) marking the job route
  ONLY — lantern = "the job goes this way," their absence = off-route exploring; two grabbable
  junk piles; one locked arch labeled in glyphs (chapter-2 tease). Explorable off-route space:
  TWO side alleys + the harbor east mole — small, rewarded with scrap + one waker-log, walled by
  fog/water beyond.
- **[∅]** every [S] item in this section: districts, canal geometry as gameplay, zipline
  placement as data, drone/creature zone data, relay yard, lantern wayfinding, numbered doors,
  the Dockmaster booth interior, the hangar-walk beacon staging.

## 3b · THE EXPEDITION — half B is OUTSIDE TOWN, and you DRIVE there (⚖ Terry-directed 2026-07-29)

**Story change of record:** half B is NOT the Dockmaster's desk paperweight (v2.1's staging). The
Dockmaster only has the LEAD: a work order for a site **outside the sea wall** where instruments
died the same week the relay went wrong. **You take the VEHICLE.** (The DC §8 modifiability rule
exists for exactly this — the halves' sources are data fields; this is Terry exercising it.)
- **[M] machinery exists:** `ToxicCityVehicleBuilder` + `VehicleRuntime` + `VehicleSafetyRuntime`
  + `VehiclePlayabilityTests` — the drivable ground vehicle is real code. **[∅]** its garage
  placement, the route, and the site.
- **[S] THE GARAGE:** a vehicle pad beside the Dockmaster's booth at (34, 0, −56) — you get the
  work order and the keys in the same breath. RILL: *"He's paying us to drive AWAY from the
  paying job. I've stopped being surprised."*
- **[S] THE ROUTE (~700 m, 2–3 min drive — the level's breathing room):** out the harbor's east
  gate (the sea-wall BREACH from the K-concepts, drivable ramp at (60, 0, −45)) → onto the tidal
  flats OUTSIDE the wall — open mud, wreck ribs, tide pools, the city shrinking behind you (the
  one place you SEE the whole ring from ground level — the postcard) → north along the wall's
  outer face → the site.
- **[S] THE SITE — the crashed survey skiff:** a half-buried wreck at flat coordinates
  ≈ (140, 0, 120), nose-down in the mud, instruments fried. Half B sits in its cracked cargo
  cage: same fracture face, humming. Around it: the FIRST WAKER-LOG of the outside thread + a
  scatter of salvage. Grabbing half B kills every instrument on the vehicle for 2 s (the
  resonance tell, taught before the join explains it).
- **[S] wayfinding:** the work order pins a smoke column visible from the breach (site burn-off
  stack, amber); driving = follow the wall then the smoke. Way home = the leaning tower over the
  wall, always visible.
- **[∅]** all of §3b's placement + the drive route + the site (vehicle CODE is the only built part).

## 3c · THE CANALS BY BOAT — the stalker interaction (⚖ Terry-directed 2026-07-29)

**The canals are a waterway, not scenery: the tide SKIFF runs them, and the CANAL STALKER lives
in them.** The lizard-thing's whole identity (bestiary sheet: amphibious ambush, low-slung,
eyeless-read head, hazard-stripe glow) plays against your hull, not on land.
- **[M]** canal stalker recipe (`tox_canal_stalker_01`) + approved sheet + skiff concepts exist.
  **[∅]** skiff as a pilotable vehicle, canal water volumes as navigable routes, and ALL stalker
  water behavior.
- **[S] THE BOAT LEG:** a skiff dock under the zipline platform at (16, 0, −48) — the canal ring
  (Canal One east–west + the ring canal, ~300 m of navigable water, 14 m wide) is the slow
  alternative route to the relay yard: zipline = fast/high, skiff = slow/low with salvage
  floating in the water. Skiff drives like the ground vehicle family (VehicleRuntime variant,
  water-locked, comfort-capped ~6 m/s).
- **[S] THE STALKER INTERACTION (non-lethal, three-stage escalation, never a jump scare):**
  1. **THE SHADOW (always, ride one):** a low wake parallels the skiff at 8–10 m, hazard-stripes
     faintly visible under the green water. RILL: *"We're being escorted."* It never touches you.
  2. **THE BUMP (ride two+, or lingering):** it shoulders the hull — a real physics nudge + deep
     thud + haptics; the skiff yaws ~10° (assist recovers). Warning, not attack.
  3. **THE BLOCK:** it surfaces AHEAD, jaw open — a stop-and-choose beat: taser-stun it (it
     sinks, sulks, gone for the session) OR cut throttle and drift 10 s (it loses interest —
     the pacifist read, taught by RILL: *"It's territorial, not hungry. Big difference."*).
  Cozy comfort preset caps it at stage 1. All three stages are canal-only — it NEVER exits the
  water (the rail/colonnade observation window in §3 is this same animal seen from land).
- **[S] why you ride at all:** two canal-only salvage floats + the underside view of the city
  (numbered pilings, tide-glow at the waterline) + it's the fastest way back from the relay yard
  with heavy salvage.

## 4 · THE FIRST ZIPTIDE + W002 (min 45–63)

- **[M]** Key seat → gate re-arm → travel is code-real (`KeySocketRuntime` exclusivity tested).
- **[S] staging:** you punch it FROM berth six; the tide erupts around the hull at 8 m radius
  (pillars clear of the quay cranes); arrival is the cistern's flooded gallery, the pump hall
  40 m ahead lit by ONE shaft of surface light (the wayfinding: walk to the light).
- **W002 data [M]:** layout/job/steps assets committed. **[∅]** the scene, the pump hall
  placement, swarm zone, extractor socket, garden plot, glyph-plate — the whole rebuild/defend/
  grow staging (the biggest unbuilt chunk of the hour).

## 5 · THE BUILD ORDER THIS DOC CREATES

Every [S] above, in dependency order — each lands as generator/data extensions until it's [M]:
1. City interior plan → `ToxicCityLayout.asset` blocks filled (districts/canals/pois/zones as
   §3's coordinates) + CityBuilder consumes them. **This is the level design landing as data.**
   Includes the §3b flats gate/breach + garage pad and the §3c skiff dock + navigable canal data.
2. Zipline/dispatch/relay/creature/drone placements per §3 (job route becomes walkable truth).
3. **The expedition (§3b):** garage + breach ramp + flats route + the crashed survey skiff site
   + half-B relocation there (contract step data: the Dockmaster gives the LEAD, not the half).
4. **The boat leg (§3c):** skiff as water-locked vehicle variant + canal volumes + the stalker's
   three-stage water behavior (shadow/bump/block, comfort-capped, non-lethal).
5. Lantern-route + sightline-triple wayfinding pass (the compass).
6. Space lane: ring lamp-chase + drone reaction layer + find-moment dressing + skybox celestial
   match per §2.
7. Hangar walk (berths 1–6 quay) + beacon thread staging (the rb121 look problem, now placed).
8. W002 staging per §4.
9. W000 porthole + eyeline-chain lighting pass per §1.
### ✅ STATUS AFTER THE BUILD PASS (rb127, 2026-07-29) — what of the list above now exists

Rows 1–4, 6, 7 (thread only) and 9 are **built as committed code/data**; they become geometry on
Terry's next bake. Precisely:
- **1 ✅** `docs/worldspecs/ToxicCity.spec.json` — the city as a spec (7 districts incl. the new
  Quay + Colonnade, Canal One, connections, zones), compiled by `WorldSpecCompiler`.
- **2 ✅ partly** — creature/drone zones and the relay/dispatch/quay markers are spec data; the
  **zipline and lantern route are still [∅]**.
- **3 ✅** `FlatsSiteAuthor` (wreck, cargo cage, burn-off column, scatter, breach ramps computed
  from the wall's own formula) + half B relocated in the spec + contract step 5 + the resonance
  tell. **⚠ the drive is ~270 m, not ~700 m** — the shell is only 190 m in radius (§3b corrected).
- **4 ✅** `CanalWaterCore`/`SkiffWaterLockRuntime` (the **ring canal** at r=74 is the ~465 m
  waterway — it existed as geometry all along) + `CanalStalkerCore`/`CanalStalkerBehavior`
  (shadow → bump → block) + the `tox_canal_stalker_01` CreatureDefinition.
- **5 ❌ NOT BUILT** — lantern route + sightline triple.
- **6 ✅** ring lamp-chase, drone wake/evade reactions, find debris shell, **the Moss-orbit sky**
  (giant/sibling moon/sun/starfield on the ground sky's bearings), plus the ascent + reentry
  plasma veils and the helm compass ribbon.
- **7 ◐** the **beacon thread is built**; the berth 1–5 quay pads are **[∅]**.
- **8 ❌ NOT BUILT** — W002's defend wave, garden plot, glyph plate.
- **9 ✅** the W000 porthole (`PortholeStarfieldCore` + `PortholeRuntime`).

**Rule:** a row here flips [S]→[M] only when the number exists in committed code/data — the same
honesty contract as the tracker. Note code-green ≠ device-green: every ✅ above means COMMITTED
AND COMPILING, not seen in a headset. **⚖ of record (Terry, 2026-07-29):** half B moved from the
Dockmaster's desk to the outside-town expedition site; the canal stalker's boat interaction is
canon; both exercised through the DC §8 data-modifiability rule — the minute-map stretches
(~min 27–41 becomes contract + expedition + optional boat leg) and the join now happens back at
the berth with both halves after the drive home.
