# 🚀 SPACEFLIGHT PHYSICS — the design T-Dog's P4b builds toward (V2.5, from Terry's 2nd PDF §3)

**Status: DESIGN ONLY — do not build until a trigger fires.** P4b v1 (comfort-capped cockpit
flight in a bounded SpaceLane scene) needs NONE of this. This doc exists so P4b's seams don't paint
us into a corner when the ambition arrives.

## Triggers (build the section only when its trigger is true)
1. **Play space exceeds ~2km** in any flight scene → **Floating Origin**.
2. **Player walks around INSIDE the ship while it flies/rotates** → **Isolated interior physics**.
3. Planets visible at scale from space → **logarithmic distant-body scaling** (cheap, can come first).

## 1. Floating origin (trigger 1)
32-bit floats jitter past ~100k units (visible artifacts far earlier in VR). Design: camera stays
near origin; a `FloatingOriginController` shifts the WORLD back by the player's offset whenever the
rig exceeds ~2,000 units, and true positions are tracked in doubles on a pure `SpaceFrame` class
(testable). Everything scene-side must be shiftable: no cached world-space positions across frames
in flight scenes (audit rule when built: `FLIGHT_CACHED_WORLDPOS`).

## 2. Isolated interior physics (trigger 2 — the "walk inside a fighting ship" problem)
Never parent the player rig to a fast/rotating ship rigidbody — interpolation desync = clipping +
nausea. Design (PDF-validated, Ixion-style):
- **Interior = its own static physics space** (separate additively-loaded scene with its own physics,
  or a layer parked at a far static coordinate). It NEVER moves. The player rig walks there normally
  — all existing locomotion/grab code works untouched.
- **Exterior hull = the thing that actually flies** in the flight scene.
- **`PhysicsAnchorSubscriber`** copies the player's interior-local pose onto a visual proxy parented
  to the moving hull (for outside observers/netcode), and copies the hull's motion into the
  interior's WINDOW/viewport rendering (the interior sees space move, not itself).
- **Boarding/deboarding** = teleport between spaces + momentum handoff at the hatch trigger —
  which is exactly the existing `TravelCoordinator`/marker idiom, so travel law is unchanged.
- **Ziptide fit:** the ship interior ALREADY exists as its own space (ship deck/Quarters built by
  patchers, entered via boarding station). That accident of history is the correct architecture —
  P4b must keep it: **fly the HULL, never the deck.** S4's cockpit is a seat in the interior space
  whose viewport renders the flight scene.

## 3. Distant bodies (trigger 3)
Don't place planets megameters away — keep them ~2–5km out and scale them logarithmically with
simulated distance (`apparentScale = k / log(trueDistance)` family). The SkyVista planets already
fake this perfectly at the skybox layer; this section only matters when a planet must PARALLAX
during flight. Seam: a `DistantBodyRig` sibling of `SkyVistaRig`.

## What P4b v1 should do NOW (the only actionable ask)
Bounded lane (≤2km), hull-only flight, cockpit = interior seat with a rendered viewport, comfort
caps (snap-turn, vignette, no roll), and **never parent the rig to the moving hull** — that single
rule keeps every future trigger buildable without rework.

*Architect (V2.5 H6). Owner on build: story/ship track. Physics-architecture changes = design-review territory (see OPERATOR_START_HERE calibration).*
