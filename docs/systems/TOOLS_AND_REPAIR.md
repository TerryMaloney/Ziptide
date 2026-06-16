# System — Tools & Repair (hands-on fixing)

**Status:** designed (direction note). **Build-later** — gated behind the core architecture
(persistence/economy/registries) + the mining/conveyor system. This captures the vision so we build
toward it; not to be built now. Decision: Terry, 2026-06-16.

## 1. Purpose
Make fixing/upgrading machines feel like *real work* in VR — not a button press. You grab the right
tool, physically attach it to a highlighted part, and turn it "righty-tighty." This is a signature
ZIPTIDE interaction (Cal is a technician — the world should make you *do* the technician's job).

## 2. How it works (intended)
- **Two-hip loadout:** a **gun** on one hip (combat) and a **tool** on the other (work) — both
  holstered via the existing belt, both grabbable. Swap tools at a **tool chest**.
- **Tool chest:** a station you open to take/return tools; different jobs need different tools.
- **Damaged parts:** when bugs/creatures attack a machine (e.g. a conveyor) past a threshold, a
  **part becomes damaged**. The damaged part **lights up** and shows a **ghost/icon of the required
  tool** floating at the attach point.
- **The repair action (the fun part):**
  1. Grab the correct tool from your hip / the chest.
  2. Bring it near the highlighted attach point → when close enough it **snaps on** (magnetic socket).
  3. **Rotate "righty-tighty"** (turn your hand/controller) to drive the repair; a radial gauge fills.
  4. On completion the part relights as healthy and the machine resumes.
- **Wrong tool** = it won't snap (or snaps but won't turn) — the icon tells you which one you need.
- **Adding parts** (upgrades) uses the same verb: snap the part/tool on, tighten to commit.

## 3. Data + code (when built)
- **`ToolDefinition`** (already exists in the data model) — function/tier/works-on; the icon shown at
  damaged parts comes from here.
- **`RepairablePart`** runtime (new): health, `requiredToolId`, attach socket transform, on-damage
  highlight, on-repair event. Lives on machine prefabs/parts.
- **`ToolAttachInteractor`** (new): the snap-on socket (reuse the holster/`XRSocketInteractor`
  pattern) + a rotation-driven progress (reuse the "righty-tighty" turn read from controller roll).
- Repair completion feeds the machine's state (ties to `MachineState` / `ProfileEconomy`).
- **Damage source:** creature attacks (see `CREATURES.md`) reduce part health.
- Reuse, don't reinvent: belt/holster for carrying tools, socket-snap for attach, the existing
  grab system for picking tools up.

## 4. VR feel
- Snap should be forgiving (generous magnetic range) so it's satisfying, not fiddly.
- The "tighten" turn should have haptic clicks + a clear radial gauge; short (≈1–2 turns), not tedious.
- Highlight + tool-ghost must read instantly across the room (emissive + worldspace icon).
- Comfort: no forced camera movement; all diegetic.

## 5. "Make it mechanically sensible" (stretch, later)
Optional later polish Terry floated: parts that actually *make sense* mechanically (a real coupling,
a valve, a bolt pattern) so repairs feel authentic rather than abstract. Nice-to-have; not v1.

## 6. Open questions (for Terry, later)
- How many tool types at the start (one wrench-equivalent, or a few)?
- Is the tool chest per-world, on the ship, or both?
- Does a failed/ignored repair have a consequence (machine offline, lost production)?

## Links
- `MINING_CONVEYOR.md` (what gets damaged/repaired), `CREATURES.md` (what damages it),
  `docs/design/SYSTEMS_ARCHITECTURE.md` (data model), `docs/design/CONTROLS_AND_FLIGHT.md` (hands/grip).
