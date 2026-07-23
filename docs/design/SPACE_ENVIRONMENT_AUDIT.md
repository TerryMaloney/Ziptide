# SPACE FLIGHT & ENVIRONMENT — where we are, what's missing to FEEL like space
### Terry 2026-07-21: "space flight has to include a whole actual space environment — see and feel like you're really out there: the local sun(s), stars, lots of stuff. Look at what we have so we can figure out what to build."

**Status:** 🔵 AUDIT — read of current code + design, zero code (freeze). Sources read:
`SPACEFLIGHT_PHYSICS.md`, `CONTROLS_AND_FLIGHT.md`, `SPACE_COMBAT.md`, `SHIP_SYSTEM.md`,
`ScenePatcherSpaceLane.cs`, `FlightModel.cs` + flight runtimes, `SkyVistaDefinition.cs` +
`SkyVistaLibrary.cs`. Companions: `systems/SKYSCAPE_DESIGN.md`, `SHIP_MK2_ARCHITECT_UPGRADE.md`.

---

## §0 — THE HEADLINE (the honest split)

**The VERB (flying) is a real, comfort-tuned v1. The PLACE (space) is a placeholder backdrop.**
You can already sit in a cockpit and fly a bounded ring course; but what's around you is a
near-black sky + one flat painted planet. Everything Terry is asking for — the felt SUN, depth,
the launch off a world, "lots of stuff out there" — is the PLACE, and it's mostly designed-with-
protected-seams but NOT BUILT. Good news: the cheapest wins (compose the sun + real starfield +
the launch transition) buy the biggest jump in "I'm actually in space."

## §1 — WHAT EXISTS (built + tested)

- **Flight model — real, comfort-reviewed, tested.** `FlightModel`/`FlightState`/`FlightParams`
  (`Content/Runtime/Flight`) + `ShipFlightRuntime`, `FlightInputCore`, `FlightCourseCore`: signed
  throttle, slow **clamped pitch**, **snap-yaw only**, **barrel-roll that always returns to exactly
  level**, a bounded `laneRadius` soft wall. EditMode-tested (`FlightModelTests`,
  `ShipFlightParamsTests`, `FlightCourseCoreTests`, `FlightInputCoreTests`). This is a genuine
  VR-comfort-first flight verb, not a stub.
- **A playable flight scene** — `ScenePatcherSpaceLane` builds `SpaceLane_Trial`: dock pad, an
  open **seat-locked cockpit frame**, a helm running the flight runtime, a **5-ring course**, and
  **drone salvage targets** (non-lethal). Warp-listed as "Flight Trial."
- **SkyVista backdrop system — shipped.** `SkyVistaDefinition` renders a layered dome: gradient +
  **stars** + **nebula** + Shell grid + zenith shimmer + **up to 3 celestial bodies**
  (`BandedPlanet / Moon / SunDisc / BlackHole`) + atmosphere. 17+ authored vistas. **A sun DISC
  and stars already exist — but as a painted DOME backdrop, not as scene lighting or 3D depth.**
- **Architecture seams for real space — DESIGNED (`SPACEFLIGHT_PHYSICS.md`), not built:** floating
  origin (trigger >2km), isolated interior physics ("fly the HULL, never the deck"), distant-body
  log-scaling (`DistantBodyRig`). The current lane is deliberately small so it needs none of them.
- **Space combat — designed skeleton** (`SPACE_COMBAT.md`): non-lethal disable+salvage,
  `ShipWeaponDefinition`, ability system, enemy ship AI via `BotBrain`. Not built.
- **Strategic layer — code exists:** `Conquest`/`GalaxyMap`/`PlanetNode` (the world-select galaxy).
- **The ship you'd fly — concepted:** the Scrapper + the MK2 living yacht (cockpit views done).

## §2 — THE GAP: what "feel like you're really in space" needs (Terry's ask, itemized)

Ordered by **felt-impact-per-cost** — the first three are cheap and transform the feeling:

1. **★ THE FELT SUN(s) — a `SunRig`, not a painted disc.** A real sun in VR is the scene's **key
   light**: a hard directional light + a bright disc + bloom/lens-flare + a sharp terminator on
   your hull + it *blinds* you when you turn into it. Today the lane has one dim generic
   directional light and the sun is only a dome sticker. Build: a `SunRig` that OWNS the scene's
   key light, sits at the sky-vista's sun direction, drives hull specular, and supports **binary/
   multiple suns** (Terry's "sun or suns" — 2 directional keys + 2 discs, different colors).
2. **★ COMPOSE THE REAL COSMOS INTO THE FLIGHT SCENE.** The lane uses near-black + one flat planet
   instead of the full SkyVista. Wire the flight scene to render a proper space vista: **full 360
   starfield** (drop the planetary horizon-fade), nebula, the sun(s), moons — the "lots of stuff"
   backdrop we already have the system for, just not assembled for space.
3. **★ THE LAUNCH / ATMOSPHERE TRANSITION** (designed in `SHIPS_AND_FACTIONS`, not built): flying
   UP off a world into space — the cloud/burn-in mask hiding the scene load, the planet shrinking
   below you. This single beat is what sells "I *left* the planet," and it reuses the
   TravelCoordinator + a veil effect. High emotion, modest cost.
4. **DEPTH / PARALLAX — `DistantBodyRig`.** Right now bodies are flat on the dome (no depth). To
   feel motion through space you need a few bodies at apparent depth that PARALLAX as you fly — the
   **planet you launched from** receding as a real 3D body, moons shifting, the sun staying fixed.
   (Log-distance scaling, trigger-3 in the physics doc.)
5. **OPEN VOLUME — floating origin.** The current lane is bounded (~380 units) to dodge float
   jitter. "Fly around freely in open space" needs the `FloatingOriginController` (trigger-1). Only
   required once play space exceeds ~2km — so it gates *open* space, not the *scenic* space of 1–3.
6. **SPACE CONTENT — "lots of stuff."** Asteroids, debris fields, derelict wrecks, stations, drifting
   salvage, traffic, gates seen from space. None built (combat is skeleton). This is the density that
   makes space a place, not a void — and it feeds the disable+salvage loop.
7. **SPACE VFX (F3.5 family, space set):** engine plasma trails, speed-streak particulate whipping
   past (the #1 cheap speed cue in VR), sun flare/bloom, nebula drift, thruster puffs on turns.
8. **SPACE AUDIO:** the "space is silent but your hull hums" bed + engine tone by throttle + the
   quiet-tide/comfort layer + sun/heat groans — the audio half of presence.
9. **WALK-THE-SHIP-IN-FLIGHT — isolated interior physics** (trigger-2). The MK2 is a walkable
   multi-deck yacht; letting you leave the seat and walk the deck *while it flies* is the big
   dream, and the seam is designed ("fly the hull, never the deck"). Heaviest item; last.

## §3 — RECOMMENDED BUILD ORDER (post-Golden-Checkpoint; cheap-felt-wins first)

1. **`SunRig`** (felt sun as key light + disc + flare + hull terminator; multi-sun) — §2.1.
2. **Space-vista composition** into `SpaceLane_Trial` (full starfield + nebula + sun + planet) — §2.2.
3. **Launch transition** (leave-the-world veil + planet shrink) — §2.3.
   → *After these three, the existing bounded lane already FEELS like space — highest ROI.*
4. **`DistantBodyRig`** parallax (the receding planet, moons) — §2.4.
5. **Space VFX + audio** sets (speed streaks, engine, hum) — §2.7/§2.8.
6. **Space content pass** (asteroids/derelicts/stations POIs) + hook the salvage loop — §2.6.
7. **Floating origin** when a world needs true open volume — §2.5.
8. **Space combat** build-out (weapons/abilities/AI) on the disable+salvage skeleton — `SPACE_COMBAT`.
9. **Isolated interior physics** (walk the flying ship) — the finale — §2.9.

## §4 — Guardrails / notes

- **Comfort is already the law** here (snap-yaw, clamped pitch, seat-locked frame, roll-returns-
  to-level) — keep every space addition inside it; the sun must not strobe, speed streaks must be
  subtle, parallax must not swim.
- **Reuse over new:** the SkyVista system, TravelCoordinator transition, F3.5 VFX, and BotBrain AI
  already exist — most of §2 is *composition + a few new rigs*, not green-field engines.
- **Freeze:** all of this is post-Golden-Checkpoint build work; this doc is the map, not a start.
- **The Prospect bar (`SKYSCAPE_DESIGN.md`):** the skyscape is one of the things that made Terry
  want to build this game — the felt sun + real starfield here is the space-side of that bar.
