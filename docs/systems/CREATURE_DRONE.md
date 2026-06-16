# Creature — Drone (first enemy; the reusable template)

The drone is enemy #1 and the **template** every later creature reuses: a shared base (health, shock,
hit-location reactions, death physics) + per-variant data (look / behavior / intensity). New drone
types = new data, not new code. Part of the Creatures/Enemies section (`CREATURES.md`,
`SYSTEMS_ARCHITECTURE.md` §Layer 3).

---

## 1. Reusable structure (so creatures share logic)
- **Base behavior** lives in `DroneRuntime` today; the hit-location + death logic is split into a
  reusable helper (`HitZones`) so future creatures (`SwarmBehavior`, `WallCrawlBehavior`, …) classify
  hits the same way.
- **Variants = data.** Same drone base, change: color/material, scale, bob/spin, **intensity**
  (aggression / speed multiplier), `hitsToDown`, whether it's shockable, and which death reactions it
  favors. Tunable per-instance now; promotes to a `DroneVariantDefinition` ScriptableObject later so a
  designer drops an asset to make a new drone (matches the `Definition` pattern).
- **`IShockable`** (taser) stays the shared "can be stunned" contract.

## 2. Hit-location reactions (the brainstorm — how it goes down by where you hit it)
The taser dart already knows its **stick point**, so the drone can react by **zone** (computed in the
drone's local frame via `HitZones.Classify`). Each zone reads as a believable mechanical failure:

| Zone (where the dart sticks) | What "breaks" | How it goes down |
|---|---|---|
| **Center / core** | total system shock | seizes, then **drops straight down**, heavy, minimal spin — the clean kill |
| **Top** (rotor/lift) | propulsion/lift lost | **plunges nose-down**, tumbling forward as lift cuts |
| **Bottom** (underbelly) | impact kicks it up | **pops up briefly, stalls, belly-flops** down, wobbling |
| **Front** (sensor/eye) | blinded / targeting lost | **recoils backward**, jitters in place, then **sinks** (slow power-down) |
| **Back** (engine/thruster) | thrust runaway | **lurches forward**, sputters, **noses into the ground ahead** |
| **Left / Right** (side thruster) | asymmetric thrust | **spins out toward the hit side**, cartwheels/spirals down |

Optional later: a **glancing/limb** hit on tougher variants doesn't kill — it staggers, sheds a part,
and keeps going degraded (supports `hitsToDown > 1`).

## 3. The taser "shock → then go down" sequence (current gun = taser darts)
On dart stick (per Terry): **the electric shock is visible immediately, then it goes down** — not an
instant drop. Sequence:
1. **Instant shock VFX** at the stick point: arcing electricity over the body (cheap code-spawned
   arc segments + a flickering cyan light), the drone's material **flashes/strobes**, and it **seizes**
   (jitters/vibrates, lights flicker) for a brief `shockSeconds` (~0.4 s).
2. **Then it fails** and goes down using the **zone reaction** from §2 (direction/tumble depend on
   where it was hit).

So you see: *tase → convulse → crash*, and the crash style tells you where you hit it. Pistol/hitscan
(no stick point) uses the Center reaction.

## 4. What's built now vs later
- **Now (this pass):** `HitZones` helper; `DroneRuntime` does shock-VFX-then-zone-death with tunable
  fields (shockSeconds, intensity, colors, spin) → tunable variants per instance; taser dart routes
  its stick point into the drone. Job counting still fires via `OnDroneDisabled` (unchanged).
- **Later:** `DroneVariantDefinition` SO (data-driven variants in a registry); real VFX/particle +
  audio for the arc; `hitsToDown > 1` + limb-shed; reuse `HitZones` for other archetypes; pooling/caps.

## 5. VR feel notes
Keep the satisfying existing kill feel; the shock adds a beat of "you got it" before the drop. Don't
make `shockSeconds` long (comfort + pace). Arc VFX is emissive + brief; swap the code primitives for a
pooled particle system once art exists. Reactions should be readable at a glance across a room.

## Links
`CREATURES.md`, `TOOLS_AND_REPAIR.md` (creatures damage machines → repair), `MILESTONE_C0_WEAPONS.md`,
`docs/design/SYSTEMS_ARCHITECTURE.md`.
