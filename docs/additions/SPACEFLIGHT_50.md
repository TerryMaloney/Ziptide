# 🚀 SPACEFLIGHT & THE SHIP — 50 ideas (bank; read README + `design/SPACEFLIGHT_PHYSICS.md`)

Current: ShipBoardingStation, walkable deck + Quarters, ShipHullBuilder (19-part hull),
ShipCastOffRuntime ("PUNCH IT"), FlightModel (comfort math, tested, UNWIRED). Rails: never parent the
rig to the moving hull; interior = static physics space; floating origin deferred until >2km. Bar:
Squadrons cockpit feel at Quest comfort.

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Cockpit throttle lever: a grabbable lever feeding FlightModel throttle in 4 detents (no analog creep = predictable accel), reusing the PUNCH IT interaction. | S | FlightModel, ShipCastOffRuntime | |
| 2 | Engine audio bed: AudioDirector layer whose pitch/volume reads FlightModel thrust, with a distinct brake-burn stinger. | S | AudioDirector, FlightModel | 🎨 |
| 3 | Velocity-stretched starfield: GPU-instanced star-streak billboards whose stretch reads FlightModel speed — speed sense without moving the interior. | S | FlightModel, Visuals | 🎨 |
| 4 | Speed-scaled vignette: comfort vignette intensity ramps with FlightModel lateral/angular velocity, tuned via LocomotionProfile (data, not code). | S | FlightModel, LocomotionProfile | |
| 5 | `ZIPTIDE: FLIGHT_STATE` tag logging FlightModel velocity, clamp hits, comfort-cap engagements every 2s — on-device flight bugs become logcat-diagnosable. | S | FlightModel, ZiptideConstants | |
| 6 | Docking clamp feedback: haptic thunk + AudioDirector clamp sound when ShipBoardingStation completes board/deboard. | S | ShipBoardingStation, AudioDirector | 🎨 |
| 7 | Snap-yaw steering: FlightModel yaw quantized to snap-turn increments (no smooth roll/yaw ever), smooth-heading interp on the hull only, never the rig. | S | FlightModel, LocomotionProfile | |
| 8 | Hull-stress creaks: short AudioDirector groans when FlightModel clamps a g-load/turn rate — invisible comfort clamps become diegetic feedback. | S | FlightModel, AudioDirector | 🎨 |
| 9 | Cockpit glass grime: static scratch/dust decal on the ShipHullBuilder viewport part — static geometry, interior stays inert. | S | ShipHullBuilder, Visuals | 🎨 |
| 10 | Ship nameplate: stencil letters in Quarters set a persisted ship-name string shown on the hull exterior. | S | Quarters, ShipHullBuilder | 🎨 |
| 11 | "Recall to ship" belt item (ItemFactory string id) calling TravelCoordinator.TravelTo(deck) with the standard fade — no new load path. | S | ItemFactory, TravelCoordinator, belt | |
| 12 | NAV table hologram: read-only world-space display in Quarters rendering the Tidefront sim's fleet positions as instanced dots. | S | Tidefront sim, Quarters | 🎨 |
| 13 | Cast-off vignette choreography: ShipCastOffRuntime drives a scripted vignette-in→hold→out curve so the most motion-heavy moment is most protected. | S | ShipCastOffRuntime, LocomotionProfile | |
| 14 | Docking voice callouts: AudioDirector checklist lines triggered by FlightModel approach-cone thresholds. | S | FlightModel, AudioDirector | 🎨 |
| 15 | Move ship scene names, dock layer, flight-tag literals into ZiptideConstants before flight wiring (no-raw-strings rule). | S | ZiptideConstants, Core | |
| 16 | EditMode suite for FlightModel arrival-brake + comfort-clamp math (inputs → exact stop distances) ahead of scene wiring. | S | FlightModel, Tests, CI | |
| 17 | Locker flight trophies: downed-drone shells and race medallions as ItemFactory cosmetics hangable in Quarters. | S | ItemFactory, Quarters | 🎨 |
| 18 | Cruise-lock chime: a one-press hold-heading/speed toggle in FlightModel + confirmation chime, for zero-input segments (comfort). | S | FlightModel, AudioDirector | |
| 19 | WorldPackDefinition `orbitalFlavor` field (approach text + skybox tint) read by ShipCastOffRuntime at launch — authored cast-offs. | S | WorldPackDefinition, ShipCastOffRuntime, VisualThemeProfile | 🎨 |
| 20 | Pre-flight snapshot: ziptide_snapshot.ps1 dumps last FLIGHT_STATE/TRAVEL_* tags so first flight sessions are one-script diagnosable. | S | tools, diagnostics | |
| 21 | Docking minigame v1: hold the hull inside FlightModel's approach cone through a 3-stage speed gate, scored on centering; failure = soft auto-wave-off (non-lethal). | M | FlightModel, ShipBoardingStation | |
| 22 | Cargo-run contracts: JobDirector issues space-lane hauls; crates are ItemFactory items lashed in the hold, delivery pays on deboard. | M | JobDirector, ItemFactory, TravelCoordinator | |
| 23 | Lashed-cargo travel rule: hold sockets count as "holstered" for the travel contract — only lashed crates survive TravelTo (`CARGO_LASHED` log). | M | Inventory persistence, TravelCoordinator | |
| 24 | Asteroid slalom: deterministic-seed GPU-instanced clusters with ring gates; FlightModel weaves at capped turn rate, vignette auto-ramps. | M | FlightModel, Visuals, LocomotionProfile | |
| 25 | Pursuit escape: non-lethal tractor drones latch a slow-beam on the hull; break lock via asteroid cover or a speed gate — full vignette during the drag. | M | FlightModel, drone AI, LocomotionProfile | |
| 26 | Ship hull health as RepairableMachine stages (sparking panel → venting pipe → dead console) on interior parts, repaired mid-flight. | M | RepairableMachine, ShipHullBuilder | |
| 27 | Engine socket upgrades: S3 sockets accept engine-module items swapping FlightModel accel/brake profiles — always inside comfort caps. | M | S3 sockets, FlightModel, ItemFactory | |
| 28 | Autopilot lane cruise: FlightModel follows an authored spline while the player walks the interior freely (legal per the rails). | M | FlightModel, ship interior | |
| 29 | Landing-pad arrivals: a completed docking cone at a world's orbital marker triggers TravelCoordinator.TravelTo(world) behind the fade — flight as the travel front-end. | M | FlightModel, TravelCoordinator, SpawnMarkerRuntime | |
| 30 | Scanner ping minigame: a console pulse; returns as spatialized AudioDirector pings + holo blips revealing derelicts/lodes at seed positions. | M | AudioDirector, FlightModel, Content | |
| 31 | Charge economy: cast-off + cruise burn a ship-charge resource; recharge via deliveries/pads, shown on a Quarters gauge — gives the S3 battery a job. | M | ShipCastOffRuntime, JobDirector, S3 sockets | |
| 32 | Comm hails: NPC ships hail with 2-choice dialogue (yield / outrun / negotiate) — all non-lethal, outcomes feed JobDirector reputation. | M | JobDirector, AudioDirector, Content | |
| 33 | Fleet escort contracts: fly formation alongside a Tidefront fleet for one leg; staying in-formation shifts that battle's sim odds. | M | Tidefront sim, FlightModel, JobDirector | |
| 34 | Solar-wind gusts: timed lateral drift FlightModel must counter-trim, each pre-announced by a rising tone + vignette. | M | FlightModel, AudioDirector, LocomotionProfile | |
| 35 | Cockpit MFDs: two world-space instrument screens (speed/heading/cone/hull) fed by FlightModel state — no RenderTextures, Quest-cheap. | M | FlightModel, UI, Visuals | 🎨 |
| 36 | Hull paint schemes: VisualThemeProfile material sets across the 19 hull parts, unlocked as cosmetics, picked at the Quarters locker. | M | ShipHullBuilder, VisualThemeProfile, Quarters | 🎨 |
| 37 | External photo drone orbiting the parked hull, snapping screenshots to a Quarters photo wall — drone moves, rig never does. | M | ItemFactory, Quarters, Visuals | 🎨 |
| 38 | Emergency wave-off: at final hull-damage stage, FlightModel auto-brakes to the nearest pad and forces dock — dramatic, non-lethal, fade + vignette throughout. | M | RepairableMachine, FlightModel, TravelCoordinator | |
| 39 | Derelict beacon jobs: JobDirector posts "investigate signal" contracts whose targets are ShipBoardingStation-dockable derelict shells spawned from the seed. | M | JobDirector, ShipBoardingStation | |
| 40 | Ambient lane traffic: GPU-instanced NPC freighter silhouettes on splines, density read from the Tidefront control map — pure visuals, zero physics. | M | Tidefront sim, Visuals | 🎨 |
| 41 | Wire FlightModel to a playable W000 orbit sandbox: cast-off into a bounded (<2km) space with rings, one pad, one derelict — the first end-to-end flight slice. | L | FlightModel, ShipCastOffRuntime, TravelCoordinator | |
| 42 | Derelict boarding dungeons: RoomPartitioner generates deterministic multi-room derelict interiors (breached compartments, loot, a RepairableMachine to restore power); dock via ShipBoardingStation. | L | RoomPartitioner, ShipBoardingStation, RepairableMachine | |
| 43 | Station hub world: a dockable station scene with jobs board, socket vendor, paint shop, reached only through the docking minigame — flight's "town". | L | TravelCoordinator, JobDirector, S3 sockets | |
| 44 | Disable-and-tow interdiction: non-lethally EMP-stall a rogue freighter (drone-down logic), then tow-beam it to a station for payout. | L | FlightModel, drone AI, JobDirector | |
| 45 | Floating-origin behind EditMode tests: implement the deferred >2km origin shift as a pure transform-rebase pass before any long-haul lane ships. | L | Core, FlightModel, Tests | |
| 46 | Race circuits: seeded ring courses per world orbit with ghost-replay of your best run; medals feed locker trophies; speed capped at the comfort clamp. | L | FlightModel, Content, Quarters | |
| 47 | Carrier landing: dock inside a moving Tidefront flagship — velocity-match via FlightModel, then a hard handoff makes the hangar the new static interior (never parenting the rig mid-motion). | L | FlightModel, Tidefront sim, ShipBoardingStation | |
| 48 | Engineering deck: a new walkable ship room housing RepairableMachines (reactor, gravity ring, scrubbers) whose stages apply real FlightModel debuffs until fixed. | L | ShipHullBuilder, RepairableMachine, FlightModel | |
| 49 | Blockade-running beat: at a contested world, thread a patrol-drone gauntlet (all non-lethal capture) to deliver relief cargo; success flips sim control pressure. | L | Tidefront sim, FlightModel, JobDirector | |
| 50 | Full "Squadrons cockpit" A/V pass: layered engine/ambience stems, MFD glow, hull rattle cues, per-theme cast-off skyboxes unified under VisualThemeProfile within Quest budgets. | L | AudioDirector, VisualThemeProfile, ShipHullBuilder | 🎨 |
