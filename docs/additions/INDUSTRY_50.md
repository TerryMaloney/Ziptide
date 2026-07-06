# ⚙️ INDUSTRY — machines, mines, conveyors & automation — 50 ideas (bank; read README first)

Current: RepairableMachine (3 physical stages), MiningRigRuntime (idle + offline accrual),
BuildSocketRuntime, pure MiningService/ProductionGraph/EconomyState (abstract graph — NO physical
belts). Bar: Factorio-lite physical automation, Quest-scaled, VR hands.

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | `BeltSegmentRuntime` placeable pack object: ItemFactory id `belt_straight` via MachineSpawnDefinition, snap-grid placed like BuildSocketRuntime, moving items from a source port into any hopper/machine intake. | L | Content, Gameplay, WorldPacks, ItemFactory | |
| 2 | Items on belts ride as GPU-instanced pucks from `GamePool`, hard-capped ~48 movers/world; beyond cap, pucks merge into labeled stacks. | M | Gameplay, GamePool, Visuals | 🎨 |
| 3 | Output-port flag on MiningRigRuntime: when a belt is docked, accrued ore auto-ejects onto the belt instead of piling in the hopper. | S | MiningRigRuntime, BeltSegmentRuntime | |
| 4 | Corner/riser/downhill belt variants as extra ItemFactory ids reusing BeltSegmentRuntime with different path splines — pure data. | S | ItemFactory, WorldPacks | |
| 5 | Splitter/merger piece: one intake alternates output between two exits round-robin (deterministic), via MachineSpawnDefinition. | M | BeltSegmentRuntime, Content | |
| 6 | Filter gate on the splitter: a resource-ID field routes `mineral` left, `scrap` right — same runtime + a whitelist check. | S | BeltSegmentRuntime, pack data | |
| 7 | Depot intake machine: a belt terminus banking arrivals into EconomyState, bridging physical belts to the abstract ProductionGraph. | M | EconomyState, ProductionGraph, Content | |
| 8 | Refinery machine (MachineSpawnDefinition + ProductionGraph recipe): eats `mineral` from a belt intake, emits `refined_ingot` from an output port. | M | ProductionGraph, Content, Gameplay | |
| 9 | Multi-stage chains: ingot→component→device recipes realized as 2-3 chained refineries per pack. | L | ProductionGraph, WorldPacks, MiningService | |
| 10 | Power grid: generator + pylon pack objects; unpowered rigs/belts idle at zero, and the breaker reuses RepairableMachine's flip-Power stage. | L | Content, Gameplay, RepairableMachine, MiningService | |
| 11 | Hand-crank flywheel on the generator: physically spin it (two-hand XR grab) to bootstrap the grid after a blackout, rising whine. | M | Gameplay, AudioDirector | 🎨 |
| 12 | Overclock dial: a rotary knob on each rig multiplying MiningService rate 1-2× but raising deterministic breakdown odds — grab and crank. | M | MiningRigRuntime, MiningService | |
| 13 | Breakdown events reusing RepairableMachine's exact 3 stages on rigs/refineries, fired from a seeded schedule so machines visibly stall. | M | RepairableMachine, MiningRigRuntime, MiningService | |
| 14 | Deterministic breakdown seed = worldSeed + dayIndex + machineId hash, computed in pure MiningService for EditMode assertion. | S | MiningService, Tests | |
| 15 | New tags: `ZIPTIDE: BELT_PLACED/BELT_JAM/POWER_ON/OFF/REFINE_OK` in ZiptideConstants, logged by the new runtimes. | S | Core, ZiptideConstants | |
| 16 | Belt-jam micro-event: a puck wedges at a corner (red flag); fix by grabbing it out by hand via existing XR grab. | M | BeltSegmentRuntime, Gameplay | |
| 17 | Wrench item (ItemFactory): holding it halves each RepairableMachine stage threshold — a belt-worthy tool to carry. | S | ItemFactory, RepairableMachine | |
| 18 | DispatchKiosk job "fix N breakdowns before dusk" via JobDirector, counting RepairableMachine completions vs the seeded schedule. | M | JobDirector, DispatchKiosk, RepairableMachine | |
| 19 | Logistics drone reusing the drone runtime: docks at a hopper, ferries a crate to the depot, capped 3 pooled/world. | L | Drone runtime, MiningRigRuntime, EconomyState, GamePool | |
| 20 | Drone dock pad as a BuildSocketRuntime purchase: pay credits, pad rises, unlocks one logistics route persisted in WorldState. | M | BuildSocketRuntime, WorldState | |
| 21 | Rig tier upgrades: re-pay an occupied BuildSocketRuntime to swap MiningRigRuntime Mk1→Mk2→Mk3 (model swap + higher rate). | M | BuildSocketRuntime, ItemFactory, MiningService | |
| 22 | Vein depletion: each site has a finite deterministic reserve in pack data; when dry, the rig slumps and you relocate it. | M | MiningService, MiningRigRuntime, WorldPacks | |
| 23 | Handheld ore scanner: geiger-style click rate rises near undiscovered vein POIs, guiding the next BuildSocketRuntime buy. | M | ItemFactory, Gameplay, AudioDirector | 🎨 |
| 24 | Mine-shaft POI: a MachineSite variant WorldPoiBuilder expands into a descending shaft room with 2 sockets, veins, cart-track stub. | L | WorldPoiBuilder, MachineSite, WorldPacks | |
| 25 | Mine cart on a closed spline loop: one pooled cart carries visible ore mounds hopper→depot, pure kinematic transform (no physics). | L | Gameplay, GamePool, Visuals | 🎨 |
| 26 | Hand-crank elevator for shaft access: crank a wheel to descend, vignette + fixed cage for VR comfort. | M | Gameplay, LocomotionProfile | |
| 27 | Two-hand hopper scoop: cupping both hands in the hopper collects everything with a cascading rattle + per-item haptic ticks. | S | MiningRigRuntime, AudioDirector | 🎨 |
| 28 | Analog rate gauge on each rig: a needle driven by current MiningService rate — overclock/power readable at a glance. | S | MiningRigRuntime, Visuals | 🎨 |
| 29 | Offline accrual receipt: on return, the rig prints a grab-and-tear paper ticket itemizing what accrued — replaces a floating popup. | S | MiningRigRuntime, ItemFactory | 🎨 |
| 30 | Persist placed belt layouts in WorldState like BuildSocketRuntime rigs (segment ids + grid poses), restored by the patcher. | M | WorldState, BeltSegmentRuntime | |
| 31 | Blueprint ghost placement: hold a segment and drag a snapped chain of ghost previews, confirm to spawn all — the core builder verb. | L | Gameplay, BeltSegmentRuntime, BuildSocketRuntime | |
| 32 | Stack merging on belts: identical adjacent pucks combine into a count-badged stack — readability + fewer draw calls. | S | BeltSegmentRuntime, Visuals | |
| 33 | WorldRuntime enforces the global mover cap: over it, oldest belt items silently bank to the nearest depot (`BELT_CAP` log), never a drop. | S | WorldRuntime, EconomyState | |
| 34 | Pneumatic tube for verticals: item despawns at intake, an emissive pulse travels, respawns at outlet — zero physics, cheap. | M | BeltSegmentRuntime, Visuals, AudioDirector | 🎨 |
| 35 | Emissive flow chevrons on belt surfaces, tinted from VisualThemeProfile, scrolling at belt speed. | S | Visuals, VisualThemeProfile | 🎨 |
| 36 | Hopper overflow mound: 3 capped swap-in mesh states (low/half/overflowing) driven by count — a neglected rig visible across the world. | S | MiningRigRuntime, Visuals | 🎨 |
| 37 | Machine audio states via AudioDirector: idle-hum / working-chug / broken-clank loops, distance-culled beyond 15m. | S | AudioDirector, Gameplay | 🎨 |
| 38 | Breakdown telegraph: stage-1 pops the panel ajar with a pooled spark burst — faults spotted without any HUD. | S | RepairableMachine, Visuals | 🎨 |
| 39 | Creature interference: a critter (CreatureRuntime) nests on a machine and stalls it; one StunBolt shoos it — non-lethal pest control. | M | CreatureRuntime, StunBolt, RepairableMachine | |
| 40 | Steam vents that periodically puff and knock items off belts (items only, never the camera), placed as a routing hazard. | S | WorldPacks, BeltSegmentRuntime | 🎨 |
| 41 | Weigh station: stack ore on a scale platter, needle swings, slam a lever to convert the pile to credits via EconomyState — a tactile sell verb. | M | EconomyState, Gameplay | |
| 42 | Depot sorting table: hand-drop mixed arrivals into matching color bins within a timer for a bonus, existing grab only. | S | Gameplay, EconomyState | |
| 43 | Wrist ratio readout: pure ProductionGraph bottleneck calc (which stage is starved) as one wrist-UI line, EditMode-testable. | M | ProductionGraph, Gameplay | |
| 44 | Belt speed tier consumable: sink `refined_ingot` at the depot to bump global belt speed one tier (3 max), stored in WorldState. | S | EconomyState, WorldState, BeltSegmentRuntime | |
| 45 | Belt bridge piece so lines cross without merging — one extra ItemFactory id, same runtime, raised spline. | S | ItemFactory, BeltSegmentRuntime | |
| 46 | New `EconomyAuditRules` check: every belt chain must terminate at a hopper/intake/depot; dangling belts fail CI audit. | S | EconomyAuditRules, Editor | |
| 47 | `FoundryBuilder` editor patcher: generates a starter line (rig→belt→refinery→depot) into a pack from data, idempotent — no hand-edited scenes. | M | Editor/Patching, WorldPacks | |
| 48 | New "Foundry" world pack authored via WorldSpecCompiler: an industry world whose machines/veins/belts all come from WorldSpec data. | L | WorldSpec, WorldSpecCompiler, WorldPacks | 🎨 |
| 49 | Assembler machine: two belt intakes (ingot + scrap) combine into one output via a two-input ProductionGraph recipe — first multi-input node. | L | ProductionGraph, Content, BeltSegmentRuntime | |
| 50 | Automation onboarding arc via JobDirector: chained jobs "place your first belt → power a refinery → launch a drone route," each gating the next BuildSocketRuntime unlock. | L | JobDirector, BuildSocketRuntime, WorldState | |
