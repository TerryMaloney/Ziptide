# ATMOSPHERIC FLIGHT & LANDING — fly the world, soft boundary, dock landing (VR-safe)
### Terry 2026-07-21: "fly around the world before ascending; map boundary auto-turns you with a 'leaving the mission zone' warning; getting too close to buildings/ground autocorrects; spaceships land ONLY at a dock (other craft maybe anywhere later); a landing mechanism — approach a dock, option to dock, ship autopilots the landing; plus re-entry (reverse of ascent). Meta-consistent on every world."

**Status:** 🔵 DESIGN — zero code (freeze). The in-world flight envelope + landing, sitting between
the dock and space. Extends `SPACEFLIGHT_PHYSICS` (`FlightModel.laneRadius` = the soft-wall
primitive), `LAUNCH_AND_ATMOSPHERE_TRANSITION` (ascent/descent veil), `SHIP_ALERT_AND_DAMAGE_FEEDBACK`
(warnings reuse the alert channels), `CELESTIAL_SYSTEM_CANON` §8 (per-world genome). Docks reuse
`ShipBoardingStation` + the marker idiom.

---

## §0 — THE FULL FLIGHT LOOP (round trip, all one continuous ride)

**Docked → Liftoff → free ATMOSPHERIC FLIGHT (bounded) → [ascend at the ceiling] → Veil → Space**
and the mirror: **Space → Veil → atmospheric DESCENT → approach dock → autopilot LANDING → Docked.**
Everything here is the atmospheric-flight + boundary + landing middle; the veil transitions live in
`LAUNCH_AND_ATMOSPHERE_TRANSITION`.

## §1 — FREE ATMOSPHERIC FLIGHT (fly around the world, see above it)

- After liftoff you fly the **comfort-capped `FlightModel`** (snap-yaw, clamped slow pitch, roll-
  returns-to-level) freely around the world — over the canals, past the tower, out to the
  outskirts, and UP to get the vista (the sky/curvature begins to show as you climb).
- **Two edges of the envelope:** a HORIZONTAL boundary (keeps you in the world → auto-turn, §2) and
  a VERTICAL ceiling (where ascent-to-space is offered → §6). Climbing high to sightsee is fine; you
  only leave for space when you INTENTIONALLY ascend (comfort — never accidental).

## §2 — THE MAP BOUNDARY (soft, warned, auto-turn — never a wall)

A world **flight-volume** (a bubble/dome sized per world). Two tiers, escalating, comfort-first:
- **Warning zone (outer):** approaching the edge → the alert system fires a **"leaving the mission
  zone" callout** (RILL: *"Cal — we're at the edge, turning us back"*) + a cockpit directional edge-
  light toward home. Reuses `SHIP_ALERT` proximity tier.
- **Auto-turn zone (at the edge):** the ship **gently banks and eases its heading back toward
  center** — a smooth assist, NOT a hard bounce or a snap (a wall-bounce is jarring + nauseating).
  Think "the autopilot won't let you leave," not "you hit glass."
- Never a hard stop, never a camera jerk. `FlightModel.laneRadius` is the existing seed; generalize
  it to a per-world boundary shape with the warning + eased-turn behavior. Diagnostic:
  `ZIPTIDE: FLIGHT_BOUNDARY zone=warn|turn`.

## §3 — TERRAIN & BUILDING AVOIDANCE (soft auto-correct — you can't crash)

- Proximity to ground/structures → a **warning** (reuse `SHIP_ALERT` proximity) → if you keep
  closing, a **soft auto-correct** eases the ship to **climb/veer away** so you never crash into a
  building or the deck. Flight stays forgiving; comfort preserved (no crash jolt, no camera move).
- Tunable "buffer distance" per world (a dense city needs a bigger buffer than open flats). The
  correction is gentle and legible — the player feels *helped*, not *fought* (like a good driving-
  assist). Diagnostic: `ZIPTIDE: FLIGHT_AVOID obj=… dist=…`.
- *(This is also why the ship never takes terrain collision damage in normal flight — the assist
  prevents it; debris/combat damage is the only hull threat, per `SHIP_ALERT`.)*

## §4 — LANDING: DOCK-ONLY (spaceship), AUTOPILOTED (comfort + reliable)

- **Docks = designated landing markers** (reuse `ShipBoardingStation` + `Marker_<id>`; a world
  declares its dock list in the genome). The spaceship can **only** set down at a dock.
- **The mechanism:** fly within a dock's approach range → a **DOCK PROMPT** appears (diegetic
  cockpit cue + RILL: *"Dock's clear — want me to bring us in?"*) → player **confirms** → the ship
  **autopilots the landing**: a scripted, comfortable descent — align over the pad, skids down,
  settle, engines wind down — then hands back to **walkable** (stand up, walk the deck; the existing
  boarding/marker handoff). *The player never hand-threads a precision landing* → comfort + zero
  frustration + it always works.
- **Manual approach, automatic touchdown:** you fly TO the dock yourself (skill/exploration); the
  final set-down is on rails (safety/comfort). Best of both.

## §5 — PER-VEHICLE LANDING RULES (data-driven; the "other ships" note)

- **`canLandAnywhere` flag on the vehicle/ship definition.** Spaceship = **false** (dock-only). The
  tide skiff already sets down on water; future light craft (a flyer/bike) = **true** (land
  anywhere). One flag, authorable per vehicle — so "your spaceship lands at docks, but some craft
  land anywhere" is just data, not special-case code.
- Land-anywhere craft still obey terrain avoidance (§3) and the boundary (§2); they just don't need
  a dock marker to set down.

## §6 — THE FULL DESCENT / RE-ENTRY (mirror of ascent)

**Space → re-entry Veil** (`LAUNCH_AND_ATMOSPHERE_TRANSITION` §4 reverse: plasma fire, brightness
peak masks the space→planet scene swap) → **atmospheric flight** (you're now flying the world) →
fly to a dock → **DOCK PROMPT → autopilot LANDING** (§4). So the round trip is closed and symmetric:
the same veil both ways, the same dock-land both ends. Ascending again just re-runs §0 forward.

## §7 — META-CONSISTENCY ACROSS EVERY WORLD (genome fields)

Same mechanics everywhere; per-world FLAVOR from the genome (add to `CELESTIAL_SYSTEM_CANON` §8):
```
world.flightEnvelope:
  boundaryShape/size:  → the flyable volume (dome radius / bounds)
  ceilingAltitude:     → where ascent-to-space is offered
  terrainBuffer:       → avoidance distance (bigger for dense cities)
  dockMarkers:         [<markerId>]  → the legal landing pads (spaceship)
  flyableCeilingView:  → how much curvature/vista shows at max altitude
```
A world can't ship without these (audit-enforced) → the boundary, avoidance, and landing behave
identically everywhere, tuned per world. `dockMarkers` ties to the existing spawn-marker/boarding law.

## §8 — REUSE / COMFORT / BUILD

- **Reuses:** `FlightModel` (+laneRadius seed), `ShipFlightRuntime`, `SHIP_ALERT` channels (boundary/
  proximity warnings + RILL), `ShipBoardingStation`/markers (docks + walkable handoff),
  `TravelCoordinator` (the veil swaps), the comfort layer, `FLIGHT_TRACE` instrumentation. Mostly
  composition + a boundary/avoidance/landing sequencer, not a new engine.
- **Comfort is the law:** boundary auto-turn and terrain auto-correct are GENTLE eased assists
  (never snaps/bounces), warned in advance; landing is autopiloted; the camera never jerks; ascent
  is always intentional. Device comfort-test is the gate.
- **Order (post-checkpoint):** boundary (warn + eased turn on laneRadius) → terrain avoidance → dock
  detect + prompt + autopilot landing → per-vehicle `canLandAnywhere` → wire the descent mirror →
  genome params. Freeze: all post-Golden-Checkpoint.
