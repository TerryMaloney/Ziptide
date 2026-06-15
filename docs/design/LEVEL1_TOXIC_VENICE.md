# Level 1 — Toxic Venice (W001 / D0_City): design + the World Recipe

The plan to nail ONE polished, guided-but-explorable level — which becomes the repeatable
standard for building every future world. Tutorial style = **mix** (light prompts for first-time
actions, then diegetic). Built on the EXISTING patcher/job framework (ScenePatcherD0/D1/D2 +
JobDirector + WorldPackDefinition). Decisions: 2026-06-15.

---

## The level as a guided loop (with room to wander)
The city already exists: central + side walkways (Y=2.5), canals, bridges, ramps, 7 blockout
buildings, 5 courtyards (Spawn Z=-16, Dispatch Z=0, Garden Z=18, Service, Outlook), drones, a
taser, a travel door. We turn it into a beat-paced tutorial that ends with a flight teaser.

**Critical path (guided):** Spawn courtyard → walk to Dispatch (learn move/look) → pick up the
taser at the weapon stand (learn grab/holster) → clear 3 drones (learn aim/shoot) → enter the
**one open building** for a job objective (learn interact) → reach the **landing pad** and fly a
short guided hop (flight teaser) → return / step into the **ziptide** (first world travel).

**Explorable (optional):** the Garden and Outlook courtyards, side catwalks, and dead-end alcoves
hold lore readables / RILL barks but no required objectives — so the player can wander and feel the
world without getting lost. Keep the critical path visually obvious (lighting, a subtle path
material/decal, objective markers); let exploration be clearly "off the main line."

## Beat sheet (each beat teaches one thing — mix of prompt + diegetic)
| Beat | Player learns | How it's taught | Job step type (reuse) |
|------|---------------|-----------------|------------------------|
| Arrive at spawn | look around, move | brief on-screen prompt, then a waypoint to Dispatch | GoToMarkerStepDefinition |
| Reach Dispatch | the job board / objectives | DispatchKiosk + ObjectiveBoard populates | (kiosk accept) |
| Grab the taser | grab + holster | prompt at the weapon stand once | CollectItemIdCountStepDefinition |
| Clear 3 drones | aim + fire | diegetic (drones engage) | DisableDronesCountStepDefinition |
| Enter the building | interact / door | waypoint + a door interactable | GoToMarkerStepDefinition |
| Building objective | a "repair the node" interact | diegetic; RILL reacts oddly (mystery seed) | DeliverToSocket or a new InteractStep |
| Landing pad → flight | flight controls | prompt-guided short hop (see CONTROLS_AND_FLIGHT) | GoToMarker + flight task |
| Ziptide out | world travel | the travel door / ship | TravelCoordinator.TravelTo |

RILL boot line fires on the first objective; RILL's W004-style question is teased at the building
node ("This wasn't a utility node…").

## Fixes this level needs (known issues)
- **Lower-level fall glitch:** player can drop to the Y=1.5 catwalks / stray targets under the city.
  Fix via: spawn-on-solid verification, railings/`PlayAreaBounds` on the critical path edges, and
  removing or repurposing the orphaned `Target1/2/3`. **Resolve precisely using the scene dump**
  (`Ziptide > Diagnostics > Dump Scene + Rig Config`) so we edit against real positions.
- **One enterable building:** today all 7 are solid blockouts. Open ONE — recommend an **interior
  room built in-scene** (a hollow shell + door interactable) over an additive sub-scene, to respect
  the boot/Single-load contract and keep it simple. The other 6 stay facades (explorable silhouette).
- **Complete the job chain:** current job is only "go to dispatch + kill 3 drones." Extend to the
  full beat sheet so the level has a beginning, middle, and an exit.
- **Guidance:** populate the ObjectiveBoard, add a waypoint marker that points at the current
  objective, and a subtle path cue on the critical route.

## THE WORLD RECIPE (repeatable standard for every future world)
Every world is built the same way so we never hand-craft from scratch (and the audit keeps us honest):
1. **WorldPackDefinition** asset: `packId`, `displayName`, `sceneName`, theme, audio theme, job
   chain ref, spawn list, exit destinations, required/granted flags.
2. **Scene patcher** (idempotent, like ScenePatcherD0/D1): builds geometry from a kit + places
   `__SPAWN_PLAYER`, `WorldRuntime`, job objects, travel doors. One patcher owns one world.
3. **Job chain** from existing `JobStepDefinition` types (GoToMarker, CollectItemId,
   DisableDronesCount, ShootTargetsCount, DeliverToSocket) — compose, don't recode.
4. **Build via `BuildAndroid.PatchScenesThenAPK`** so patchers + `WorldAuditRunner` run every build
   (blockers like spawn-below-floor abort the build — this is how we stop the lower-level class of bug).
5. **Verify** on-device by `ZIPTIDE:` tags: `TRAVEL_OK`, `AUDIT_OK`, job-step logs.

Nailing Toxic Venice produces: the building-interior pattern, the guidance pattern, the flight-teaser
pattern, and the job-chain template — all reusable. That's the framework.

## Build order (small, testable, CI-gated — do AFTER CI is green)
1. Switch `dev_build_install.ps1` → `PatchScenesThenAPK` (patch + audit every build).
2. Fix spawn/lower-level (scene dump → spawn-on-solid + bounds). Verify no fall.
3. Extend D0 job chain to the full beat sheet (reuse step types).
4. Open one building interior + door interactable.
5. Add guidance (ObjectiveBoard + waypoint marker).
6. Add the landing pad + short flight task (see CONTROLS_AND_FLIGHT.md).
7. Polish: audio ambience, RILL lines, lore readables.
