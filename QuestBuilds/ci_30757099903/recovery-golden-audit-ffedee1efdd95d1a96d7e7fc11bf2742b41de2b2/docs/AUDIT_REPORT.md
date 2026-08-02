# World Audit Report

**Total Blockers:** 0  
**Total Warnings:** 183

## Scene: __FORGE__
Blockers: 0  Warnings: 0

_No issues found._

## Scene: __ECONOMY__
Blockers: 0  Warnings: 11

- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'carapace' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'crystal' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'data_chip' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'fuel_cell' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'jump_core' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'memory_shard' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'prism' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'resonator' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'salt' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'scrap' — dead-end resource.
- [WARNING] RESOURCE_NO_SINK: Nothing consumes 'stun_charge_cell' — dead-end resource.

## Scene: __WORLD_PACKS__
Blockers: 0  Warnings: 0

_No issues found._

## Scene: __FIRST_HOUR__
Blockers: 0  Warnings: 0

_No issues found._

## Scene: _Boot
Blockers: 0  Warnings: 0

_No issues found._

## Scene: MilestoneA_GrabCube
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): GrabbableCube=1/1  Ground=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 2 of 2 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. GrabbableCube, Ground  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: D0_City
Blockers: 0  Warnings: 7

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=8/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __D1_CITY_ROOT/Patrol_BridgeW=3/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __D1_CITY_ROOT/Drone_3=3/1  __D1_CITY_ROOT/Patrol_Garden=3/1  __D1_CITY_ROOT/Patrol_Canal=3/1  __D1_CITY_ROOT/Drone_1=3/1  __D1_CITY_ROOT/Drone_2=3/1  __D1_CITY_ROOT/Patrol_Spawn=3/1  __D1_CITY_ROOT/Patrol_BridgeE=3/1  Ground=1/1  __D1_CITY_ROOT/CourtyardA_Spawn=1/1  __D1_CITY_ROOT/PerimeterNorth=1/1  +70 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at Pistol is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ Pistol
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at TaserDartGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ TaserDartGun
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Story' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] ART_UNCONFORMED: 133 of 133 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY/IdentityPylon_R, __D1_CITY_ROOT/BridgeRailLB_3, __D1_CITY_ROOT/CatwalkRailR_Inner, __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS/RouteBeacon_0/Post, __D1_CITY_ROOT/CatwalkRailL_Outer, __D1_CITY_ROOT/Building_R1, Pistol, __D1_CITY_ROOT/BridgeR_0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: SandboxTestLab
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): SandboxDrone_2=1/1  TaserDartGun=1/1  augment_bubble_guard=1/1  ZonePost_loco=1/1  augment_surge_dash=1/1  augment_sixth_sense=1/1  ZonePost_travel=1/1  SandboxFloor=1/1  FieldCamera=1/1  GravityGun=1/1  ZonePost_enemy=1/1  augment_magnet_palm=1/1  ClimbTower=1/1  ZonePost_range=1/1  SandboxDrone_1=1/1  augment_sure_step=1/1  SandboxDrone_0=1/1  ZonePost_grab=1/1  +2 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at FieldCamera is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ FieldCamera
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at TaserDartGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ TaserDartGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at SandboxDrone_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ SandboxDrone_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at SandboxDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ SandboxDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at SandboxDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ SandboxDrone_1
- [WARNING] ART_UNCONFORMED: 20 of 20 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. SandboxDrone_2, ZonePost_grab, SandboxDrone_0, augment_sure_step, SandboxDrone_1, ZonePost_range, ClimbTower, augment_magnet_palm  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: StarterWorld
Blockers: 0  Warnings: 27

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): WorldRoot/Zone_BadlandsMissionPocket=31/8  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=20/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=18/3  WorldRoot/Zone_DormantZiptideSite=13/13  WorldRoot/Zone_Spaceport=9/9  WorldRoot/Zone_BadlandsVehicleArea=9/9  WorldRoot/Zone_SlumWalkways=8/8  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=8/3  WorldRoot/Zone_ToxicCity_MainSpine=6/6  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  WorldRoot/Zone_SludgeCanals=4/4  WorldRoot/Zone_OutskirtsTransition=4/4  WorldRoot/Zone_GroundVehiclePort=3/3  WorldRoot/Hub_DockQuarter=3/3  Walk_Spaceport_Spine=1/1  +9 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 83 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] ART_UNCONFORMED: 173 of 173 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0, WorldRoot/Zone_DormantZiptideSite/GatePillar_5, WorldRoot/Zone_Spaceport/LandingPad_2, WorldRoot/Zone_DormantZiptideSite/GatePillar_6, WorldRoot/Hub_DockQuarter/HubSign, WorldRoot/Zone_SlumWalkways/Shack_1, WorldRoot/Zone_BadlandsMissionPocket/Cover_2, WorldRoot/Zone_GroundVehiclePort/DepotBay  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: ToxicCity
Blockers: 0  Warnings: 15

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __TOXIC_CITY_ROOT/District_Shipyard=204/30  __TOXIC_CITY_ROOT/__RING_CITY=187/7  __TOXIC_CITY_ROOT/District_Quay=176/32  __TOXIC_CITY_ROOT/District_Dispatch=165/28  __TOXIC_CITY_ROOT/District_Market=164/17  __TOXIC_CITY_ROOT/District_CanalRow=158/29  __TOXIC_CITY_ROOT/District_Plaza=136/17  __TOXIC_CITY_ROOT/__CITY_WAYFINDING=82/5  __TOXIC_CITY_ROOT/__TOXIC_RIVERS=78/7  __TOXIC_CITY_ROOT/District_Colonnade=77/17  __TOXIC_CITY_ROOT/Shipyard=68/12  __TOXIC_CITY_ROOT/Skyline=51/3  __TOXIC_CITY_ROOT/__QUAY_BERTHS=35/4  __TOXIC_CITY_ROOT/__FLATS_EXPEDITION_SITE=32/19  __TOXIC_CITY_ROOT/__SHIPYARD_APPROACH=24/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=18/3  __TOXIC_CITY_ROOT/__TOXIC_CITY_VEHICLES=18/10  __TOXIC_CITY_ROOT/Connections=16/2  +12 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] CITY_NO_COURTYARD: No courtyard found in city scene. City may lack safe platform areas.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __TOXIC_CITY_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Market_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Market_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Canal_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Canal_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Market_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Market_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Canal_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Canal_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Market_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Market_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_1
- [WARNING] PERF_TRIS_OVER_TARGET: static triangles = 159956 over the target 150000 (cap 400000) — budget attention needed.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 112 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1770 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 1769 of 1770 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES/StoryTrace_3/FieldCanister, __TOXIC_CITY_ROOT/District_Shipyard/Facade_Z_0/Awning, __TOXIC_CITY_ROOT/District_Dispatch/Facade_X_0/Cap, __TOXIC_CITY_ROOT/__RING_CITY/CanalRing/Canal_7, __TOXIC_CITY_ROOT/District_Dispatch/Facade_X_0/Awning, __TOXIC_CITY_ROOT/District_Shipyard/Landmark_Crane/Rung_22, __TOXIC_CITY_ROOT/District_Dispatch/Facade_Z_0/Window_0_1_ShopCool, __TOXIC_CITY_ROOT/District_Shipyard/Facade_X_1/Awning  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: PvP_Arena01
Blockers: 0  Warnings: 3

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __PVP_ARENA_ROOT/TaserDartGun=2/5  __PVP_ARENA_ROOT/Cover_3=1/1  __PVP_ARENA_ROOT/Floor=1/1  __PVP_ARENA_ROOT/Wall_W=1/1  __PVP_ARENA_ROOT/PvpBot=1/1  __PVP_ARENA_ROOT/Cover_4=1/1  __PVP_ARENA_ROOT/Cover_1=1/1  __PVP_ARENA_ROOT/Wall_S=1/1  __PVP_ARENA_ROOT/Wall_E=1/1  __PVP_ARENA_ROOT/Ramp_S=1/1  __PVP_ARENA_ROOT/Wall_N=1/1  __PVP_ARENA_ROOT/Ramp_N=1/1  __PVP_ARENA_ROOT/GravityGun=1/1  __PVP_ARENA_ROOT/Platform=1/1  __PVP_ARENA_ROOT/Cover_2=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __PVP_ARENA_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __PVP_ARENA_ROOT/GravityGun
- [WARNING] ART_UNCONFORMED: 15 of 16 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __PVP_ARENA_ROOT/Cover_3, __PVP_ARENA_ROOT/TaserDartGun, __PVP_ARENA_ROOT/Floor, __PVP_ARENA_ROOT/Wall_W, __PVP_ARENA_ROOT/PvpBot, __PVP_ARENA_ROOT/Cover_4, __PVP_ARENA_ROOT/Cover_1, __PVP_ARENA_ROOT/Wall_S  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W000_DriftIn
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __W000_DRIFT_IN_ROOT/District_BerthBay=78/7  __W000_DRIFT_IN_ROOT/Shipyard=68/12  __W000_DRIFT_IN_ROOT/District_BunkBay=27/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=12/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=8/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __W000_DRIFT_IN_ROOT/Pistol=2/5  __W000_DRIFT_IN_ROOT/TaserDartGun=2/5  __W000_DRIFT_IN_ROOT/breaker_blade=1/1  __W000_DRIFT_IN_ROOT/GravityGun=1/1  __FIRST_HOUR_BUNK_OBJECT/KeepsakeVisual=1/1  __W000_DRIFT_IN_ROOT/Connections=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __W000_DRIFT_IN_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __W000_DRIFT_IN_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __W000_DRIFT_IN_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __W000_DRIFT_IN_ROOT/GravityGun
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Story' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 37 over the target 25 (cap 60) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 221 of 223 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __W000_DRIFT_IN_ROOT/District_BerthBay/Landmark_GantryCrane/Rung_18, __W000_DRIFT_IN_ROOT/District_BerthBay/Facade_X_0/Win, __W000_DRIFT_IN_ROOT/Shipyard/Ship_Static_Placeholder/Wing_R, __W000_DRIFT_IN_ROOT/Shipyard/Ship_Static_Placeholder/Canopy, __W000_DRIFT_IN_ROOT/Shipyard/Ship_Static_Placeholder/HullShell_Bow, __W000_DRIFT_IN_ROOT/District_BunkBay/Facade_Z_0/Win, __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME/HorizonLandmark_1/Crown, __W000_DRIFT_IN_ROOT/District_BerthBay/Landmark_GantryCrane/LadderStringer_b  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W002_DryCistern
Blockers: 0  Warnings: 11

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __DRY_CISTERN_ROOT/Dressing=528/8  __DRY_CISTERN_ROOT/__BUILDINGS_GalleryB=133/122  __DRY_CISTERN_ROOT/District_ChamberA=74/16  __DRY_CISTERN_ROOT/Shipyard=68/12  __DRY_CISTERN_ROOT/District_DeepShaft=65/4  __DRY_CISTERN_ROOT/Pois=62/10  __DRY_CISTERN_ROOT/District_CisternMouth=22/4  __DRY_CISTERN_ROOT/ArrivalVista=22/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __DRY_CISTERN_ROOT/Connections=4/1  __DRY_CISTERN_ROOT/Drones=3/1  __DRY_CISTERN_ROOT/TaserDartGun=2/5  __DRY_CISTERN_ROOT/Pistol=2/5  +5 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __DRY_CISTERN_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __DRY_CISTERN_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_1
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'dry_cistern' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 186 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1039 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 611 of 1039 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __DRY_CISTERN_ROOT/Dressing/Route/Cairn_10/Mid, __DRY_CISTERN_ROOT/Dressing/Scatter/Prop_61/Debris0, __DRY_CISTERN_ROOT/Dressing/Scatter/Prop_70/Rock2, __DRY_CISTERN_ROOT/District_ChamberA/Facade_Z_0/Win, __DRY_CISTERN_ROOT/Dressing/Scatter/Prop_41/Rock0, __DRY_CISTERN_ROOT/District_ChamberA/Hero_PumpHouse/Room_0/shelf/Board1, __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES/DiscoveryNode_0/DiscoveryCore/DiscoveryStatus, __DRY_CISTERN_ROOT/District_CisternMouth/Facade_X_1/Win  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W003_GlassShelf
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __GLASS_SHELF_ROOT/Dressing=483/7  __GLASS_SHELF_ROOT/District_RelayShelfA=76/4  __GLASS_SHELF_ROOT/Shipyard=68/12  __GLASS_SHELF_ROOT/Pois=62/10  __GLASS_SHELF_ROOT/District_RelayShelfB=59/4  __GLASS_SHELF_ROOT/District_MesaBase=56/4  __GLASS_SHELF_ROOT/District_SkiffWreck=32/4  __GLASS_SHELF_ROOT/ArrivalVista=19/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __GLASS_SHELF_ROOT/Connections=8/2  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __GLASS_SHELF_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'glass_shelf' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 40 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 911 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 891 of 911 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE/ArrivalTrace_4, __GLASS_SHELF_ROOT/Shipyard/Ship_Static_Placeholder/Fuselage_Rib_2, __GLASS_SHELF_ROOT/Pois/__POI_story/Beacon, __GLASS_SHELF_ROOT/Dressing/Route/Cairn_14/Light, __GLASS_SHELF_ROOT/Dressing/Route/Cairn_28/Base, __GLASS_SHELF_ROOT/District_SkiffWreck/Facade_X_0, __GLASS_SHELF_ROOT/District_RelayShelfA/Facade_X_0/Win, __GLASS_SHELF_ROOT/ArrivalVista/Midground_62/Rock2  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W004_BroadcastTomb
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __BROADCAST_TOMB_ROOT/Dressing=396/8  __BROADCAST_TOMB_ROOT/District_BroadcastCore=81/4  __BROADCAST_TOMB_ROOT/District_JunctionB=78/14  __BROADCAST_TOMB_ROOT/District_JunctionA=77/14  __BROADCAST_TOMB_ROOT/Shipyard=68/12  __BROADCAST_TOMB_ROOT/Pois=62/10  __BROADCAST_TOMB_ROOT/ArrivalVista=27/3  __BROADCAST_TOMB_ROOT/District_TombEntry=24/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=11/2  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=9/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __BROADCAST_TOMB_ROOT/Connections=4/1  __BROADCAST_TOMB_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'broadcast_tomb' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 51 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 864 of 884 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __BROADCAST_TOMB_ROOT/Dressing/Scatter/Prop_47/Rock0, __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE/RouteTrace_1_7, __BROADCAST_TOMB_ROOT/Dressing/Scatter/Prop_51/Rock0, __BROADCAST_TOMB_ROOT/Dressing/Route/Cairn_6/Mid, __BROADCAST_TOMB_ROOT/District_JunctionB/Facade_X_1/Win, __BROADCAST_TOMB_ROOT/District_BroadcastCore/Facade_Z_0/Win, __BROADCAST_TOMB_ROOT/Shipyard/Ship_Static_Placeholder/Nacelle_R, __BROADCAST_TOMB_ROOT/Dressing/Route/Cairn_28/Base  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W005_OxidizedCanopy
Blockers: 0  Warnings: 12

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __OXIDIZED_CANOPY_ROOT/Dressing=643/8  __OXIDIZED_CANOPY_ROOT/__BUILDINGS_GroveEdge=203/187  __OXIDIZED_CANOPY_ROOT/District_Scrubber=82/15  __OXIDIZED_CANOPY_ROOT/District_CanopyLift=75/4  __OXIDIZED_CANOPY_ROOT/Pois=62/10  __OXIDIZED_CANOPY_ROOT/District_ForestFloor=25/4  __OXIDIZED_CANOPY_ROOT/ArrivalVista=19/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __OXIDIZED_CANOPY_ROOT/Skyline=10/1  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __OXIDIZED_CANOPY_ROOT/Connections=6/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __OXIDIZED_CANOPY_ROOT/Drones=4/1  __OXIDIZED_CANOPY_ROOT/TaserDartGun=2/5  __OXIDIZED_CANOPY_ROOT/Pistol=2/5  +5 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __OXIDIZED_CANOPY_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __OXIDIZED_CANOPY_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_3 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_3
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_1
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'oxidized_canopy' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 240 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1186 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 664 of 1186 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __OXIDIZED_CANOPY_ROOT/Dressing/Scatter/Prop_33/Rock2, __OXIDIZED_CANOPY_ROOT/District_Scrubber/Hero_SporeScrubber/Room_3/lamp/Base, __OXIDIZED_CANOPY_ROOT/District_Scrubber/Hero_SporeScrubber/Room_3/shelf/Board0, __OXIDIZED_CANOPY_ROOT/Pois/__POI_works/Pipe1, __OXIDIZED_CANOPY_ROOT/District_CanopyLift/Facade_Z_0/Win, __OXIDIZED_CANOPY_ROOT/Skyline/Silhouette_0, __OXIDIZED_CANOPY_ROOT/Dressing/Route/Cairn_25/Light, __OXIDIZED_CANOPY_ROOT/Dressing/Scatter/Prop_14/Rock1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W006_MirrorFlats
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __MIRROR_FLATS_ROOT/Dressing=791/8  __MIRROR_FLATS_ROOT/Pois=62/10  __MIRROR_FLATS_ROOT/District_BeamCollector=59/4  __MIRROR_FLATS_ROOT/District_PrismTowerA=30/4  __MIRROR_FLATS_ROOT/District_FlatsEdge=30/4  __MIRROR_FLATS_ROOT/ArrivalVista=18/3  __MIRROR_FLATS_ROOT/District_PrismTowerB=15/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __MIRROR_FLATS_ROOT/Connections=4/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __MIRROR_FLATS_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'mirror_flats' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 30 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1057 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 617 of 1057 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __MIRROR_FLATS_ROOT/Dressing/Route/Cairn_31/Base, __MIRROR_FLATS_ROOT/ArrivalVista/Midground_31/Rock0, __MIRROR_FLATS_ROOT/Dressing/Route/Cairn_37/Light, __MIRROR_FLATS_ROOT/Dressing/Scatter/Prop_84/Shard1, __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION/AmbientMote_1, __MIRROR_FLATS_ROOT/Dressing/Scatter/Prop_49/Debris2, __MIRROR_FLATS_ROOT/District_PrismTowerA/Facade_Z_0/Win, __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS/RouteBeacon_0/Cap  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W007_SableStation
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __SABLE_STATION_ROOT/Dressing=444/7  __SABLE_STATION_ROOT/District_FuelRig=66/4  __SABLE_STATION_ROOT/District_AirlockRow=63/13  __SABLE_STATION_ROOT/Pois=62/10  __SABLE_STATION_ROOT/District_ObservationDeck=53/4  __SABLE_STATION_ROOT/District_Dock=24/4  __SABLE_STATION_ROOT/ArrivalVista=20/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/2  __SABLE_STATION_ROOT/Connections=6/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __SABLE_STATION_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'sable_station' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 37 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 773 of 793 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __SABLE_STATION_ROOT/Dressing/Route/Cairn_20/Light, __SABLE_STATION_ROOT/District_ObservationDeck/Facade_X_0/Win, __SABLE_STATION_ROOT/Dressing/Route/Cairn_11/Mid, __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS/RouteBeacon_0/Post, __SABLE_STATION_ROOT/District_ObservationDeck/Facade_X_1/Win, __SABLE_STATION_ROOT/Dressing/Scatter/Prop_37/Rock0, __SABLE_STATION_ROOT/Dressing/Route/Cairn_24/Base, __SABLE_STATION_ROOT/Dressing/Scatter/Prop_3/Rock1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W008_SealedArchive
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __SEALED_ARCHIVE_ROOT/Dressing=276/7  __SEALED_ARCHIVE_ROOT/District_PowerCore=91/15  __SEALED_ARCHIVE_ROOT/District_VaultDoorHall=88/16  __SEALED_ARCHIVE_ROOT/District_ReaderHall=82/4  __SEALED_ARCHIVE_ROOT/Pois=62/10  __SEALED_ARCHIVE_ROOT/District_ArchiveMouth=21/3  __SEALED_ARCHIVE_ROOT/ArrivalVista=17/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=11/2  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=9/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __SEALED_ARCHIVE_ROOT/Connections=4/1  __SEALED_ARCHIVE_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'sealed_archive' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 41 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 688 of 708 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __SEALED_ARCHIVE_ROOT/District_VaultDoorHall/Hero_VaultDoor/VaultDoor_Wall_Zn_b, __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS/RouteBeacon_2/Cap, __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY/IdentityPylon_L, __SEALED_ARCHIVE_ROOT/District_VaultDoorHall/Hero_VaultDoor/VaultDoor_Floor, __SEALED_ARCHIVE_ROOT/District_ArchiveMouth/ArchiveMouth_Ground, __SEALED_ARCHIVE_ROOT/Pois/__POI_works/PlinthGlow, __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME/HorizonLandmark_3/Crown, __SEALED_ARCHIVE_ROOT/District_VaultDoorHall/Hero_VaultDoor/Room_0/shelf/Clutter1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W009_Chitinwall
Blockers: 0  Warnings: 13

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __CHITINWALL_ROOT/Dressing=420/8  __CHITINWALL_ROOT/District_WallGate=68/12  __CHITINWALL_ROOT/Pois=62/10  __CHITINWALL_ROOT/District_PylonArray=58/4  __CHITINWALL_ROOT/District_HiveMarket=50/5  __CHITINWALL_ROOT/District_Undercity=29/4  __CHITINWALL_ROOT/Skyline=22/1  __CHITINWALL_ROOT/ArrivalVista=21/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __CHITINWALL_ROOT/Connections=9/2  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=7/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __CHITINWALL_ROOT/Drones=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __CHITINWALL_ROOT/Pistol=2/5  +4 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __CHITINWALL_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __CHITINWALL_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Market_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Market_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Pylons_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Pylons_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Pylons_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Pylons_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Market_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Market_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Pylons_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Pylons_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Market_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Market_1
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'chitinwall' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 51 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 785 of 807 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __CHITINWALL_ROOT/Dressing/Scatter/Prop_39/Rock0, __CHITINWALL_ROOT/Dressing/Scatter/Prop_114/Rib1, __CHITINWALL_ROOT/District_PylonArray/Facade_Z_0/Win, __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES/DiscoveryNode_0/DiscoveryCore/DiscoveryStatus, __CHITINWALL_ROOT/District_WallGate/Facade_X_0/Win, __CHITINWALL_ROOT/Dressing/Scatter/Prop_72/Rib2, __CHITINWALL_ROOT/ArrivalVista/Hero_ArchRing/Arch0/PillarR, __CHITINWALL_ROOT/Pois/__POI_story/Beacon  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W010_TidalArray
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __TIDAL_ARRAY_ROOT/Dressing=714/8  __TIDAL_ARRAY_ROOT/Pois=62/10  __TIDAL_ARRAY_ROOT/District_TurbineB=55/4  __TIDAL_ARRAY_ROOT/District_TurbineA=41/4  __TIDAL_ARRAY_ROOT/District_SaltWorks=34/4  __TIDAL_ARRAY_ROOT/District_ShoreCamp=31/4  __TIDAL_ARRAY_ROOT/ArrivalVista=26/3  __TIDAL_ARRAY_ROOT/Connections=12/2  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __TIDAL_ARRAY_ROOT/Canals=2/1  __TIDAL_ARRAY_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'tidal_array' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 31 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1025 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 629 of 1025 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __TIDAL_ARRAY_ROOT/Dressing/Route/Cairn_30/Base, __TIDAL_ARRAY_ROOT/Dressing/Route/Cairn_11/Light, __TIDAL_ARRAY_ROOT/Dressing/Scatter/Prop_62/Debris0, __TIDAL_ARRAY_ROOT/Dressing/Scatter/Prop_138/Shard1, __TIDAL_ARRAY_ROOT/Pois/__POI_camp_b/MastBeacon, __TIDAL_ARRAY_ROOT/Connections/ElevatedWalkway_TurbineA_SaltWorks/Railing_R, __TIDAL_ARRAY_ROOT/Dressing/Route/Cairn_38/Light, __TIDAL_ARRAY_ROOT/Pois/__POI_camp_a/Cover0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W011_TheHum
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __THE_HUM_ROOT/Dressing=347/7  __THE_HUM_ROOT/District_ResonatorA=74/12  __THE_HUM_ROOT/District_ResonatorB=72/15  __THE_HUM_ROOT/Pois=62/10  __THE_HUM_ROOT/District_TunnelMouth=28/4  __THE_HUM_ROOT/ArrivalVista=17/3  __THE_HUM_ROOT/District_MinersCamp=17/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=12/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=12/2  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __THE_HUM_ROOT/Connections=4/1  __THE_HUM_ROOT/__CaveMouth=4/4  __THE_HUM_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'the_hum' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 43 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 676 of 696 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __THE_HUM_ROOT/Dressing/Scatter/Prop_25/Rock1, __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY/IdentityPylon_R, __THE_HUM_ROOT/District_ResonatorB/Hero_BankB/Room_0/stool/Leg, __THE_HUM_ROOT/Connections/GroundStreet_TunnelMouth_ResonatorA, __THE_HUM_ROOT/District_ResonatorB/Facade_X_1/Win, __THE_HUM_ROOT/Dressing/Scatter/Prop_76/Shard0, __THE_HUM_ROOT/ExperienceTerrain, __THE_HUM_ROOT/Dressing/Route/Cairn_5/Light  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W012_MarasLastJump
Blockers: 0  Warnings: 6

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __MARAS_LAST_JUMP_ROOT/Dressing=655/7  __MARAS_LAST_JUMP_ROOT/District_GateCoreA=79/12  __MARAS_LAST_JUMP_ROOT/District_GateCoreB=69/13  __MARAS_LAST_JUMP_ROOT/Pois=62/10  __MARAS_LAST_JUMP_ROOT/ArrivalVista=28/3  __MARAS_LAST_JUMP_ROOT/District_LaunchPoint=26/4  __MARAS_LAST_JUMP_ROOT/District_Gantry=17/4  __MARAS_LAST_JUMP_ROOT/Connections=12/2  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=9/3  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=8/2  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __MARAS_LAST_JUMP_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'maras_last_jump' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 37 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1012 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 992 of 1012 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __MARAS_LAST_JUMP_ROOT/Connections/Bridge_GateCoreA_GateCoreB/Railing_L, __MARAS_LAST_JUMP_ROOT/Dressing/Route/Cairn_24/Base, __MARAS_LAST_JUMP_ROOT/Pois/__POI_grove/Frond2, __MARAS_LAST_JUMP_ROOT/Dressing/Scatter/Prop_120/Rock0, __MARAS_LAST_JUMP_ROOT/Dressing/Scatter/Prop_164/Rock1, __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME/HorizonLandmark_3/Crown, __MARAS_LAST_JUMP_ROOT/District_Gantry/Facade_Z_0/Win, __MARAS_LAST_JUMP_ROOT/Dressing/Scatter/Prop_3/Rock2  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Chitinwall
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_CHITINWALL_ROOT/Pad_pistol=2/2  __ARENA_ARENA_CHITINWALL_ROOT/Cover_4=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_N=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Floor=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_E=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Ramp_E=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_S=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Pad_sonic_thumper=1/1  __ARENA_ARENA_CHITINWALL_ROOT/PvpBot=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Catwalk_W=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_8=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Catwalk_E=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_W=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_7=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_2=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Ramp_W=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_1=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Pad_gravity_gun=1/1  +4 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 23 of 23 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_CHITINWALL_ROOT/Cover_4, __ARENA_ARENA_CHITINWALL_ROOT/Cover_6, __ARENA_ARENA_CHITINWALL_ROOT/Cover_3, __ARENA_ARENA_CHITINWALL_ROOT/Pad_gravity_gun, __ARENA_ARENA_CHITINWALL_ROOT/Cover_1, __ARENA_ARENA_CHITINWALL_ROOT/Ramp_W, __ARENA_ARENA_CHITINWALL_ROOT/Cover_2, __ARENA_ARENA_CHITINWALL_ROOT/Pad_pistol  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Cistern
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_CISTERN_ROOT/Wall_N=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_6=1/1  __ARENA_ARENA_CISTERN_ROOT/Ramp_N=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_static_net=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_CISTERN_ROOT/PvpBot=1/1  __ARENA_ARENA_CISTERN_ROOT/Wall_E=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_2=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_3=1/1  __ARENA_ARENA_CISTERN_ROOT/Wall_S=1/1  __ARENA_ARENA_CISTERN_ROOT/LightShaftHill=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_1=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_5=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_4=1/1  __ARENA_ARENA_CISTERN_ROOT/Ramp_S=1/1  __ARENA_ARENA_CISTERN_ROOT/Wall_W=1/1  __ARENA_ARENA_CISTERN_ROOT/Floor=1/1  +1 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 19 of 19 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_CISTERN_ROOT/Wall_N, __ARENA_ARENA_CISTERN_ROOT/Wall_W, __ARENA_ARENA_CISTERN_ROOT/Ramp_S, __ARENA_ARENA_CISTERN_ROOT/Cover_4, __ARENA_ARENA_CISTERN_ROOT/Cover_5, __ARENA_ARENA_CISTERN_ROOT/Cover_1, __ARENA_ARENA_CISTERN_ROOT/LightShaftHill, __ARENA_ARENA_CISTERN_ROOT/Wall_S  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_MirrorFlats
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_pistol=2/2  __ARENA_ARENA_MIRRORFLATS_ROOT/PvpBot=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_2=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Ramp_W=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_E=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_5=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_1=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Ramp_E=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Perch_W=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_4=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_0=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Floor=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Perch_E=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_N=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_W=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_prism_beam=1/1  +2 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 21 of 21 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_pistol, __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_prism_beam, __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_W, __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_N, __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_taser_dart_gun, __ARENA_ARENA_MIRRORFLATS_ROOT/Perch_E, __ARENA_ARENA_MIRRORFLATS_ROOT/Floor, __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Tidal
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_TIDAL_ROOT/Floor=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_E=1/1  __ARENA_ARENA_TIDAL_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_TIDAL_ROOT/Island_NE=1/1  __ARENA_ARENA_TIDAL_ROOT/Island_SE=1/1  __ARENA_ARENA_TIDAL_ROOT/Cover_2=1/1  __ARENA_ARENA_TIDAL_ROOT/Cover_4=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_W=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_S=1/1  __ARENA_ARENA_TIDAL_ROOT/Island_SW=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_N=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_S=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_N=1/1  __ARENA_ARENA_TIDAL_ROOT/Pad_static_net=1/1  __ARENA_ARENA_TIDAL_ROOT/PvpBot=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_E=1/1  __ARENA_ARENA_TIDAL_ROOT/Pad_pistol=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_W=1/1  +4 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 22 of 22 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_TIDAL_ROOT/Floor, __ARENA_ARENA_TIDAL_ROOT/Cover_3, __ARENA_ARENA_TIDAL_ROOT/Island_NW, __ARENA_ARENA_TIDAL_ROOT/Wall_W, __ARENA_ARENA_TIDAL_ROOT/Pad_pistol, __ARENA_ARENA_TIDAL_ROOT/Bridge_E, __ARENA_ARENA_TIDAL_ROOT/PvpBot, __ARENA_ARENA_TIDAL_ROOT/Pad_static_net  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Void
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_VOID_ROOT/Cover_3=1/1  __ARENA_ARENA_VOID_ROOT/Wall_S=1/1  __ARENA_ARENA_VOID_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_VOID_ROOT/Hub=1/1  __ARENA_ARENA_VOID_ROOT/Walk_NS=1/1  __ARENA_ARENA_VOID_ROOT/Cover_2=1/1  __ARENA_ARENA_VOID_ROOT/Cover_4=1/1  __ARENA_ARENA_VOID_ROOT/Up_S=1/1  __ARENA_ARENA_VOID_ROOT/Ring_N=1/1  __ARENA_ARENA_VOID_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_VOID_ROOT/Wall_W=1/1  __ARENA_ARENA_VOID_ROOT/Wall_N=1/1  __ARENA_ARENA_VOID_ROOT/Ring_S=1/1  __ARENA_ARENA_VOID_ROOT/Pad_prism_beam=1/1  __ARENA_ARENA_VOID_ROOT/PvpBot=1/1  __ARENA_ARENA_VOID_ROOT/Up_N=1/1  __ARENA_ARENA_VOID_ROOT/Pad_pistol=1/1  __ARENA_ARENA_VOID_ROOT/Wall_E=1/1  +3 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 21 of 21 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_VOID_ROOT/Cover_3, __ARENA_ARENA_VOID_ROOT/Walk_EW, __ARENA_ARENA_VOID_ROOT/Wall_E, __ARENA_ARENA_VOID_ROOT/Pad_pistol, __ARENA_ARENA_VOID_ROOT/Up_N, __ARENA_ARENA_VOID_ROOT/PvpBot, __ARENA_ARENA_VOID_ROOT/Pad_prism_beam, __ARENA_ARENA_VOID_ROOT/Ring_S  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: SpaceLane_Trial
Blockers: 0  Warnings: 3

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __SPACELANE_ROOT/LaneContent=711/707  __SPACELANE_ROOT/CockpitFrame=3/3  __SPACELANE_ROOT/DockPad=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 711 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] ART_UNCONFORMED: 715 of 715 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __SPACELANE_ROOT/LaneContent/Ring_3/Truss/Coil_3, __SPACELANE_ROOT/LaneContent/DebrisField/HullPlate_14/Chevrons, __SPACELANE_ROOT/LaneContent/Ring_3/Truss/Fin_6, __SPACELANE_ROOT/LaneContent/Ring_1/Truss/Coil_0/Ribs, __SPACELANE_ROOT/LaneContent/DebrisField/CargoPod_21/DriveBand_B, __SPACELANE_ROOT/LaneContent/DebrisField/TrussSection_0/Chord_0, __SPACELANE_ROOT/LaneContent/DebrisField/DroneArm_26/Fore, __SPACELANE_ROOT/LaneContent/Ring_1/Truss/NumeralCollar/Chevron_2  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W011_Undercroft
Blockers: 0  Warnings: 3

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): CavePad_3/Rim4=1/1  CavePad_6/Rim2=1/1  CaveStal_5_0/Spike=1/1  CaveStal_4_0/Spike=1/1  CaveLink_0_7/Bridge=1/1  __SPAWN_FLOOR=1/1  CaveStal_4_2/CrystalTip=1/1  CaveStal_1_2/Spike=1/1  CavePad_7/Rim6=1/1  CaveLink_10_2/Bridge=1/1  CavePad_4/Rim2=1/1  CaveStal_6_2/Spike=1/1  CaveStal_10_2/CrystalTip=1/1  CavePad_6/Rim5=1/1  CavePad_5/Rim0=1/1  CavePad_5/Rim4=1/1  CavePad_1/Floor=1/1  CaveStal_7_2/CrystalTip=1/1  +160 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 178 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] ART_UNCONFORMED: 178 of 178 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. CavePad_3/Rim4, CaveStal_7_0/CrystalTip, CaveStal_9_2/Spike, CaveStal_3_2/CrystalTip, CavePad_0/Rim7, CavePad_0/Rim5, CavePad_10/Rim5, CaveLink_4_9/Bridge  Give each a Forge recipe, or add a whitelist line WITH its WHY.

