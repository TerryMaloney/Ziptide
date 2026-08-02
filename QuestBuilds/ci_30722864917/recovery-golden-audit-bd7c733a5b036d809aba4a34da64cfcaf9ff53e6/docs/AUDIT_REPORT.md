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

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=8/3  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __D1_CITY_ROOT/Drone_1=3/1  __D1_CITY_ROOT/Drone_2=3/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __D1_CITY_ROOT/Patrol_Canal=3/1  __D1_CITY_ROOT/Patrol_BridgeE=3/1  __D1_CITY_ROOT/Patrol_BridgeW=3/1  __D1_CITY_ROOT/Patrol_Garden=3/1  __D1_CITY_ROOT/Drone_3=3/1  __D1_CITY_ROOT/Patrol_Spawn=3/1  __D1_CITY_ROOT/RailingPerimSouth=1/1  __D1_CITY_ROOT/PerimeterSouth=1/1  __D1_CITY_ROOT/CourtyardD_Service=1/1  +70 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at Pistol is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ Pistol
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at TaserDartGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ TaserDartGun
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Story' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] ART_UNCONFORMED: 133 of 133 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __D1_CITY_ROOT/BridgeRailLB_2, __D1_CITY_ROOT/Building_R3, __D1_CITY_ROOT/Building_R1, __D1_CITY_ROOT/RailingOuterL, __D1_CITY_ROOT/Patrol_Garden, __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION/AmbientMote_1, __D1_CITY_ROOT/BridgeR_0, __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION/AmbientMote_0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: SandboxTestLab
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): SandboxDrone_2=1/1  augment_overclock=1/1  TaserDartGun=1/1  ZonePost_loco=1/1  augment_surge_dash=1/1  ZonePost_travel=1/1  ClimbTower=1/1  augment_sure_step=1/1  SandboxFloor=1/1  GravityGun=1/1  ZonePost_enemy=1/1  FieldCamera=1/1  augment_magnet_palm=1/1  ZonePost_range=1/1  SandboxDrone_1=1/1  SandboxDrone_0=1/1  augment_bubble_guard=1/1  ZonePost_grab=1/1  +2 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at FieldCamera is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ FieldCamera
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at TaserDartGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ TaserDartGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at SandboxDrone_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ SandboxDrone_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at SandboxDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ SandboxDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at SandboxDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ SandboxDrone_1
- [WARNING] ART_UNCONFORMED: 20 of 20 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. SandboxDrone_2, ZonePost_grab, augment_bubble_guard, SandboxDrone_0, SandboxDrone_1, ZonePost_range, augment_magnet_palm, FieldCamera  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: StarterWorld
Blockers: 0  Warnings: 27

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): WorldRoot/Zone_BadlandsMissionPocket=31/8  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=20/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=18/3  WorldRoot/Zone_DormantZiptideSite=13/13  WorldRoot/Zone_Spaceport=9/9  WorldRoot/Zone_BadlandsVehicleArea=9/9  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=8/3  WorldRoot/Zone_SlumWalkways=8/8  WorldRoot/Zone_ToxicCity_MainSpine=6/6  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  WorldRoot/Zone_OutskirtsTransition=4/4  WorldRoot/Zone_SludgeCanals=4/4  WorldRoot/Zone_GroundVehiclePort=3/3  WorldRoot/Hub_DockQuarter=3/3  Walk_Spine_Slum=1/1  +9 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_1
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 83 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] ART_UNCONFORMED: 173 of 173 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE/RouteTrace_2_3, WorldRoot/Zone_BadlandsMissionPocket/ScavengerDrone_0, WorldRoot/Zone_DormantZiptideSite/GatePillar_5, WorldRoot/Zone_Spaceport/LandingPad_2, WorldRoot/Zone_DormantZiptideSite/GatePillar_6, WorldRoot/Hub_DockQuarter/HubSign, WorldRoot/Zone_SlumWalkways/Shack_1, WorldRoot/Zone_BadlandsMissionPocket/Cover_2  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: ToxicCity
Blockers: 0  Warnings: 15

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __TOXIC_CITY_ROOT/District_Shipyard=204/30  __TOXIC_CITY_ROOT/__RING_CITY=187/7  __TOXIC_CITY_ROOT/District_Quay=176/32  __TOXIC_CITY_ROOT/District_Dispatch=165/28  __TOXIC_CITY_ROOT/District_Market=164/17  __TOXIC_CITY_ROOT/District_CanalRow=158/29  __TOXIC_CITY_ROOT/District_Plaza=136/17  __TOXIC_CITY_ROOT/__CITY_WAYFINDING=82/5  __TOXIC_CITY_ROOT/__TOXIC_RIVERS=78/7  __TOXIC_CITY_ROOT/District_Colonnade=77/17  __TOXIC_CITY_ROOT/Shipyard=68/12  __TOXIC_CITY_ROOT/Skyline=51/3  __TOXIC_CITY_ROOT/__QUAY_BERTHS=35/4  __TOXIC_CITY_ROOT/__FLATS_EXPEDITION_SITE=32/19  __TOXIC_CITY_ROOT/__SHIPYARD_APPROACH=24/4  __TOXIC_CITY_ROOT/__TOXIC_CITY_VEHICLES=18/10  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=18/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=16/4  +12 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] CITY_NO_COURTYARD: No courtyard found in city scene. City may lack safe platform areas.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __TOXIC_CITY_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Market_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Market_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Canal_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Canal_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Canal_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Canal_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Market_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Market_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Tutorial_Dispatch_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __TOXIC_CITY_ROOT/Drones/Patrol_Market_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __TOXIC_CITY_ROOT/Drones/Patrol_Market_0
- [WARNING] PERF_TRIS_OVER_TARGET: static triangles = 159956 over the target 150000 (cap 400000) — budget attention needed.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 112 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1770 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 1769 of 1770 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __TOXIC_CITY_ROOT/District_CanalRow/__STREET_LIFE/StreetLamp_0/Post, __TOXIC_CITY_ROOT/District_Market/Facade_X_0/Antenna, __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES/DiscoveryNode_2/DiscoveryCore, __TOXIC_CITY_ROOT/__RING_CITY/CanalRing/Canal_15, __TOXIC_CITY_ROOT/__RING_CITY/Outskirts/StiltVillage_0/Hut_3, __TOXIC_CITY_ROOT/District_Colonnade/Facade_X_0/Awning, __TOXIC_CITY_ROOT/District_Quay/Landmark_QuayCrane/Rung_3, __TOXIC_CITY_ROOT/District_Quay/Facade_X_1/RoofUnit_1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: PvP_Arena01
Blockers: 0  Warnings: 3

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __PVP_ARENA_ROOT/TaserDartGun=2/5  __PVP_ARENA_ROOT/Wall_E=1/1  __PVP_ARENA_ROOT/Wall_S=1/1  __PVP_ARENA_ROOT/Wall_N=1/1  __PVP_ARENA_ROOT/Ramp_N=1/1  __PVP_ARENA_ROOT/Cover_2=1/1  __PVP_ARENA_ROOT/Ramp_S=1/1  __PVP_ARENA_ROOT/Floor=1/1  __PVP_ARENA_ROOT/Platform=1/1  __PVP_ARENA_ROOT/Cover_1=1/1  __PVP_ARENA_ROOT/Wall_W=1/1  __PVP_ARENA_ROOT/Cover_3=1/1  __PVP_ARENA_ROOT/Cover_4=1/1  __PVP_ARENA_ROOT/PvpBot=1/1  __PVP_ARENA_ROOT/GravityGun=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __PVP_ARENA_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __PVP_ARENA_ROOT/GravityGun
- [WARNING] ART_UNCONFORMED: 15 of 16 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __PVP_ARENA_ROOT/Wall_E, __PVP_ARENA_ROOT/Wall_S, __PVP_ARENA_ROOT/TaserDartGun, __PVP_ARENA_ROOT/Wall_N, __PVP_ARENA_ROOT/Ramp_N, __PVP_ARENA_ROOT/Cover_2, __PVP_ARENA_ROOT/Ramp_S, __PVP_ARENA_ROOT/Floor  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W000_DriftIn
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __W000_DRIFT_IN_ROOT/Shipyard=68/12  __W000_DRIFT_IN_ROOT/District_BerthBay=42/4  __W000_DRIFT_IN_ROOT/District_BunkBay=27/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=12/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=8/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __W000_DRIFT_IN_ROOT/TaserDartGun=2/5  __W000_DRIFT_IN_ROOT/Pistol=2/5  __W000_DRIFT_IN_ROOT/GravityGun=1/1  __FIRST_HOUR_BUNK_OBJECT/KeepsakeVisual=1/1  __W000_DRIFT_IN_ROOT/breaker_blade=1/1  __W000_DRIFT_IN_ROOT/Connections=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __W000_DRIFT_IN_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __W000_DRIFT_IN_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __W000_DRIFT_IN_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __W000_DRIFT_IN_ROOT/breaker_blade
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Story' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 36 over the target 25 (cap 60) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 185 of 187 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION/AmbientMote_7, __W000_DRIFT_IN_ROOT/District_BunkBay/Facade_X_0/Win, __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION/AmbientMote_4, __W000_DRIFT_IN_ROOT/District_BunkBay/Facade_Z_0/Win, __W000_DRIFT_IN_ROOT/Shipyard/Ship_Static_Placeholder/RadarDish, __W000_DRIFT_IN_ROOT/District_BerthBay/Facade_X_1/Win, __W000_DRIFT_IN_ROOT/Shipyard/Ship_Static_Placeholder/WingFlap_R, __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME/HorizonLandmark_4/Mass  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W002_DryCistern
Blockers: 0  Warnings: 11

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __DRY_CISTERN_ROOT/Dressing=528/8  __DRY_CISTERN_ROOT/__BUILDINGS_GalleryB=133/122  __DRY_CISTERN_ROOT/District_ChamberA=74/16  __DRY_CISTERN_ROOT/Shipyard=68/12  __DRY_CISTERN_ROOT/District_DeepShaft=65/4  __DRY_CISTERN_ROOT/Pois=62/10  __DRY_CISTERN_ROOT/ArrivalVista=22/3  __DRY_CISTERN_ROOT/District_CisternMouth=22/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __DRY_CISTERN_ROOT/Connections=4/1  __DRY_CISTERN_ROOT/Drones=3/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=2/1  __DRY_CISTERN_ROOT/Pistol=2/5  +5 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __DRY_CISTERN_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __DRY_CISTERN_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __DRY_CISTERN_ROOT/Drones/Patrol_Gallery_0
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'dry_cistern' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 186 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1039 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 611 of 1039 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __DRY_CISTERN_ROOT/Dressing/Scatter/Prop_78/Rock2, __DRY_CISTERN_ROOT/Pois/__POI_grove/Planter5, __DRY_CISTERN_ROOT/Dressing/Scatter/Prop_110/Rock0, __DRY_CISTERN_ROOT/Dressing/Scatter/Prop_107/Rock2, __DRY_CISTERN_ROOT/Dressing/Route/Cairn_5/Mid, __DRY_CISTERN_ROOT/District_ChamberA/Facade_Z_0/Win, __DRY_CISTERN_ROOT/District_DeepShaft/Facade_X_0, __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME/HorizonLandmark_3/Crown  Give each a Forge recipe, or add a whitelist line WITH its WHY.

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
- [WARNING] ART_UNCONFORMED: 891 of 911 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __GLASS_SHELF_ROOT/Dressing/Route/Cairn_23/Mid, __GLASS_SHELF_ROOT/Dressing/Scatter/Prop_87/Rock2, __GLASS_SHELF_ROOT/District_RelayShelfB/Facade_Z_0/Win, __GLASS_SHELF_ROOT/District_RelayShelfB/Facade_Z_1/Win, __GLASS_SHELF_ROOT/ArrivalVista/Hero_Monolith/Frag3, __GLASS_SHELF_ROOT/Pois/__POI_works/PlinthGlow, __GLASS_SHELF_ROOT/Dressing/Scatter/Prop_86/Rock0, __GLASS_SHELF_ROOT/Dressing/Scatter/Prop_74/Rock1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W004_BroadcastTomb
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __BROADCAST_TOMB_ROOT/Dressing=396/8  __BROADCAST_TOMB_ROOT/District_BroadcastCore=81/4  __BROADCAST_TOMB_ROOT/District_JunctionB=78/14  __BROADCAST_TOMB_ROOT/District_JunctionA=77/14  __BROADCAST_TOMB_ROOT/Shipyard=68/12  __BROADCAST_TOMB_ROOT/Pois=62/10  __BROADCAST_TOMB_ROOT/ArrivalVista=27/3  __BROADCAST_TOMB_ROOT/District_TombEntry=24/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=11/2  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=9/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __BROADCAST_TOMB_ROOT/Connections=4/1  __BROADCAST_TOMB_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'broadcast_tomb' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 51 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 864 of 884 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE/RouteTrace_1_7, __BROADCAST_TOMB_ROOT/Dressing/Scatter/Prop_57/Rock1, __BROADCAST_TOMB_ROOT/Dressing/Scatter/Prop_119/Rock0, __BROADCAST_TOMB_ROOT/Dressing/Route/Cairn_6/Mid, __BROADCAST_TOMB_ROOT/Dressing/Scatter/Prop_125/Rock1, __BROADCAST_TOMB_ROOT/ArrivalVista/Midground_62/Rock4, __BROADCAST_TOMB_ROOT/District_JunctionB/Hero_ScreenWallB/ScreenWallB_Wall_Zn_b, __BROADCAST_TOMB_ROOT/District_BroadcastCore/Facade_X_0/Win  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W005_OxidizedCanopy
Blockers: 0  Warnings: 12

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __OXIDIZED_CANOPY_ROOT/Dressing=643/8  __OXIDIZED_CANOPY_ROOT/__BUILDINGS_GroveEdge=203/187  __OXIDIZED_CANOPY_ROOT/District_Scrubber=82/15  __OXIDIZED_CANOPY_ROOT/District_CanopyLift=75/4  __OXIDIZED_CANOPY_ROOT/Pois=62/10  __OXIDIZED_CANOPY_ROOT/District_ForestFloor=25/4  __OXIDIZED_CANOPY_ROOT/ArrivalVista=19/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __OXIDIZED_CANOPY_ROOT/Skyline=10/1  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __OXIDIZED_CANOPY_ROOT/Connections=6/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __OXIDIZED_CANOPY_ROOT/Drones=4/1  __OXIDIZED_CANOPY_ROOT/TaserDartGun=2/5  __OXIDIZED_CANOPY_ROOT/Pistol=2/5  +5 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __OXIDIZED_CANOPY_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __OXIDIZED_CANOPY_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_3 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_3
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __OXIDIZED_CANOPY_ROOT/Drones/Patrol_Canopy_0
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'oxidized_canopy' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 240 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1186 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 664 of 1186 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __OXIDIZED_CANOPY_ROOT/Dressing/Route/Cairn_19/Base, __OXIDIZED_CANOPY_ROOT/Dressing/Route/Cairn_33/Mid, __OXIDIZED_CANOPY_ROOT/Dressing/Scatter/Prop_43/Rock1, __OXIDIZED_CANOPY_ROOT/District_CanopyLift/Facade_X_1/Win, __OXIDIZED_CANOPY_ROOT/Dressing/Scatter/Prop_46/Rock1, __OXIDIZED_CANOPY_ROOT/Dressing/Route/Cairn_27/Base, __OXIDIZED_CANOPY_ROOT/ArrivalVista/Midground_31/Rock1, __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY/IdentityLabel  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W006_MirrorFlats
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __MIRROR_FLATS_ROOT/Dressing=791/8  __MIRROR_FLATS_ROOT/Pois=62/10  __MIRROR_FLATS_ROOT/District_BeamCollector=59/4  __MIRROR_FLATS_ROOT/District_PrismTowerA=30/4  __MIRROR_FLATS_ROOT/District_FlatsEdge=30/4  __MIRROR_FLATS_ROOT/ArrivalVista=18/3  __MIRROR_FLATS_ROOT/District_PrismTowerB=15/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __MIRROR_FLATS_ROOT/Connections=4/1  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __MIRROR_FLATS_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'mirror_flats' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 30 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1057 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 617 of 1057 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __MIRROR_FLATS_ROOT/District_BeamCollector/Facade_Z_1/Win, __MIRROR_FLATS_ROOT/Dressing/Route/Cairn_10/Light, __MIRROR_FLATS_ROOT/Dressing/Route/Cairn_6/Light, __MIRROR_FLATS_ROOT/Dressing/Scatter/Prop_137/Debris2, __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION/AmbientMote_5, __MIRROR_FLATS_ROOT/Dressing/Route/Cairn_3/Light, __MIRROR_FLATS_ROOT/Dressing/Scatter/Prop_121/Debris0, __MIRROR_FLATS_ROOT/Pois/__POI_story/Pylon1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W007_SableStation
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __SABLE_STATION_ROOT/Dressing=444/7  __SABLE_STATION_ROOT/District_FuelRig=66/4  __SABLE_STATION_ROOT/District_AirlockRow=63/13  __SABLE_STATION_ROOT/Pois=62/10  __SABLE_STATION_ROOT/District_ObservationDeck=53/4  __SABLE_STATION_ROOT/District_Dock=24/4  __SABLE_STATION_ROOT/ArrivalVista=20/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __SABLE_STATION_ROOT/Connections=6/2  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/2  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __SABLE_STATION_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'sable_station' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 37 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 773 of 793 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __SABLE_STATION_ROOT/Pois/__POI_ruin/Cache, __SABLE_STATION_ROOT/Pois/__POI_cave/InnerGlow, __SABLE_STATION_ROOT/District_AirlockRow/Hero_Airlock/Room_0/lamp/Shade, __SABLE_STATION_ROOT/Dressing/Route/Cairn_14/Base, __SABLE_STATION_ROOT/District_ObservationDeck/Facade_Z_0/Win, __SABLE_STATION_ROOT/Dressing/Scatter/Prop_63/Rock0, __SABLE_STATION_ROOT/Dressing/Scatter/Prop_0/Rock1, __SABLE_STATION_ROOT/District_FuelRig/Facade_X_1  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W008_SealedArchive
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __SEALED_ARCHIVE_ROOT/Dressing=276/7  __SEALED_ARCHIVE_ROOT/District_PowerCore=91/15  __SEALED_ARCHIVE_ROOT/District_VaultDoorHall=88/16  __SEALED_ARCHIVE_ROOT/District_ReaderHall=82/4  __SEALED_ARCHIVE_ROOT/Pois=62/10  __SEALED_ARCHIVE_ROOT/District_ArchiveMouth=21/3  __SEALED_ARCHIVE_ROOT/ArrivalVista=17/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=11/2  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=9/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __SEALED_ARCHIVE_ROOT/Connections=4/1  __SEALED_ARCHIVE_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'sealed_archive' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 41 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 688 of 708 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __SEALED_ARCHIVE_ROOT/Dressing/Scatter/Prop_68/Shard0, __SEALED_ARCHIVE_ROOT/District_ReaderHall/Facade_X_0/Win, __SEALED_ARCHIVE_ROOT/Pois/__POI_grove/Planter2, __SEALED_ARCHIVE_ROOT/District_PowerCore/Hero_CoreHousing/Room_0/lamp/Shade, __SEALED_ARCHIVE_ROOT/Dressing/Scatter/Prop_71/Shard1, __SEALED_ARCHIVE_ROOT/Dressing/Route/Cairn_26/Mid, __SEALED_ARCHIVE_ROOT/Dressing/Scatter/Prop_31/Rock0, __SEALED_ARCHIVE_ROOT/Dressing/Route/Cairn_10/Base  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W009_Chitinwall
Blockers: 0  Warnings: 13

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __CHITINWALL_ROOT/Dressing=420/8  __CHITINWALL_ROOT/District_WallGate=68/12  __CHITINWALL_ROOT/Pois=62/10  __CHITINWALL_ROOT/District_PylonArray=58/4  __CHITINWALL_ROOT/District_HiveMarket=50/5  __CHITINWALL_ROOT/District_Undercity=29/4  __CHITINWALL_ROOT/Skyline=22/1  __CHITINWALL_ROOT/ArrivalVista=21/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __CHITINWALL_ROOT/Connections=9/2  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=8/4  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=7/2  __CHITINWALL_ROOT/Drones=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=6/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __CHITINWALL_ROOT/TaserDartGun=2/5  +4 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __CHITINWALL_ROOT/GravityGun is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/GravityGun
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: ItemRuntime at __CHITINWALL_ROOT/breaker_blade is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/breaker_blade
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Market_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Market_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Market_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Market_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Pylons_1 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Pylons_1
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Pylons_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Pylons_2
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Pylons_0 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Pylons_0
- [WARNING] VISUAL_COVERAGE_PRIMITIVE_FALLBACK: DroneRuntime at __CHITINWALL_ROOT/Drones/Swarm_Market_2 is represented by one bare primitive renderer; keep visible but replace with authored presentation. @ __CHITINWALL_ROOT/Drones/Swarm_Market_2
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'chitinwall' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 51 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 785 of 807 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __CHITINWALL_ROOT/District_WallGate/Facade_X_1/Win, __CHITINWALL_ROOT/Pois/__POI_grove/Stalk2, __CHITINWALL_ROOT/District_WallGate/Hero_GateHouse/Room_1/coat_rack/Peg0, __CHITINWALL_ROOT/District_PylonArray/Facade_X_1/Win, __CHITINWALL_ROOT/Skyline/Silhouette_10, __CHITINWALL_ROOT/District_PylonArray/Facade_X_1/Win, __CHITINWALL_ROOT/Dressing/Scatter/Prop_24/Rock1, __CHITINWALL_ROOT/Dressing/Scatter/Prop_7/Rock0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W010_TidalArray
Blockers: 0  Warnings: 8

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __TIDAL_ARRAY_ROOT/Dressing=714/8  __TIDAL_ARRAY_ROOT/Pois=62/10  __TIDAL_ARRAY_ROOT/District_TurbineB=55/4  __TIDAL_ARRAY_ROOT/District_TurbineA=41/4  __TIDAL_ARRAY_ROOT/District_SaltWorks=34/4  __TIDAL_ARRAY_ROOT/District_ShoreCamp=31/4  __TIDAL_ARRAY_ROOT/ArrivalVista=26/3  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __TIDAL_ARRAY_ROOT/Connections=12/2  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=6/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=4/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=3/3  __TIDAL_ARRAY_ROOT/Canals=2/1  __TIDAL_ARRAY_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Discovery' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_IMPROVEMENT_ASPECT_THIN: Quality aspect 'Interaction' has thin evidence score=64 modules=1 objects=4; prioritize it in the next improvement round. @ __WORLD_IMPROVEMENT_ROUND
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'tidal_array' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 31 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1025 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 629 of 1025 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __TIDAL_ARRAY_ROOT/District_TurbineB/Facade_X_0/Win, __TIDAL_ARRAY_ROOT/District_TurbineA/Facade_Z_0/Win, __TIDAL_ARRAY_ROOT/Dressing/Route/Cairn_18/Mid, __TIDAL_ARRAY_ROOT/Dressing/Scatter/Prop_77/Debris1, __TIDAL_ARRAY_ROOT/Dressing/Scatter/Prop_127/Debris2, __TIDAL_ARRAY_ROOT/Dressing/Scatter/Prop_49/Debris0, __TIDAL_ARRAY_ROOT/Pois/__POI_grove/Stalk1, __TIDAL_ARRAY_ROOT/Dressing/Route/Cairn_14/Mid  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W011_TheHum
Blockers: 0  Warnings: 5

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __THE_HUM_ROOT/Dressing=347/7  __THE_HUM_ROOT/District_ResonatorA=74/12  __THE_HUM_ROOT/District_ResonatorB=72/15  __THE_HUM_ROOT/Pois=62/10  __THE_HUM_ROOT/District_TunnelMouth=28/4  __THE_HUM_ROOT/ArrivalVista=17/3  __THE_HUM_ROOT/District_MinersCamp=17/4  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=12/2  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=12/3  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __THE_HUM_ROOT/__CaveMouth=4/4  __THE_HUM_ROOT/Connections=4/1  __THE_HUM_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'the_hum' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 43 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 676 of 696 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __THE_HUM_ROOT/Pois/__POI_grove/Stalk2, __THE_HUM_ROOT/Dressing/Scatter/Prop_105/Rock0, __THE_HUM_ROOT/Dressing/Scatter/Prop_37/Shard1, __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE/RouteTrace_2_2, __THE_HUM_ROOT/Dressing/Scatter/Prop_110/Rock1, __THE_HUM_ROOT/Dressing/Scatter/Prop_109/Rock0, __THE_HUM_ROOT/Dressing/Route/Cairn_3/Light, __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES/DiscoveryNode_1/PedestalRing  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W012_MarasLastJump
Blockers: 0  Warnings: 6

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __MARAS_LAST_JUMP_ROOT/Dressing=655/7  __MARAS_LAST_JUMP_ROOT/District_GateCoreA=79/12  __MARAS_LAST_JUMP_ROOT/District_GateCoreB=69/13  __MARAS_LAST_JUMP_ROOT/Pois=62/10  __MARAS_LAST_JUMP_ROOT/ArrivalVista=28/3  __MARAS_LAST_JUMP_ROOT/District_LaunchPoint=26/4  __MARAS_LAST_JUMP_ROOT/District_Gantry=17/4  __MARAS_LAST_JUMP_ROOT/Connections=12/2  __WORLD_IMPROVEMENT_ROUND/__WIM_STORY_TRACES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_DISCOVERY_NODES=12/4  __WORLD_IMPROVEMENT_ROUND/__WIM_HORIZON_FRAME=10/3  __WORLD_IMPROVEMENT_ROUND/__WIM_ROUTE_BEACONS=9/3  __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE=8/2  __WORLD_IMPROVEMENT_ROUND/__WIM_AMBIENT_MOTION=7/1  __WORLD_IMPROVEMENT_ROUND/__WIM_ARRIVAL_IDENTITY=5/4  __MARAS_LAST_JUMP_ROOT/ExperienceTerrain=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] WORLD_CONTENT_SOCKET_MISSING: Pack 'maras_last_jump' defines 1 build socket(s) but the scene contains no BuildSocketRuntime — the mechanic is unreachable in this world.
- [WARNING] PERF_MATERIALS_OVER_TARGET: unique materials = 37 over the target 25 (cap 60) — budget attention needed.
- [WARNING] PERF_RENDERERS_OVER_TARGET: renderers = 1012 over the target 900 (cap 2500) — budget attention needed.
- [WARNING] PERF_LIGHTS_OVER_TARGET: real-time lights = 3 over the target 1 (cap 3) — budget attention needed.
- [WARNING] ART_UNCONFORMED: 992 of 1012 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __MARAS_LAST_JUMP_ROOT/Dressing/Route/Cairn_10/Base, __MARAS_LAST_JUMP_ROOT/Pois/__POI_works/PlinthGlow, __WORLD_IMPROVEMENT_ROUND/__WIM_GROUNDED_ROUTE/RouteTrace_2_3, __MARAS_LAST_JUMP_ROOT/Dressing/Scatter/Prop_104/Rock1, __MARAS_LAST_JUMP_ROOT/District_GateCoreB/Facade_X_0/Win, __MARAS_LAST_JUMP_ROOT/District_GateCoreA/Facade_Z_0/Win, __MARAS_LAST_JUMP_ROOT/Dressing/Scatter/Prop_143/Rock1, __MARAS_LAST_JUMP_ROOT/Dressing/Scatter/Prop_95/Rock0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Chitinwall
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_CHITINWALL_ROOT/Pad_pistol=2/2  __ARENA_ARENA_CHITINWALL_ROOT/Cover_1=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_7=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_E=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_5=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Floor=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_W=1/1  __ARENA_ARENA_CHITINWALL_ROOT/PvpBot=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Pad_sonic_thumper=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Catwalk_W=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_3=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_4=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Cover_6=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Ramp_W=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Catwalk_E=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Wall_N=1/1  __ARENA_ARENA_CHITINWALL_ROOT/Pad_taser_dart_gun=1/1  +4 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 23 of 23 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_CHITINWALL_ROOT/Cover_1, __ARENA_ARENA_CHITINWALL_ROOT/Wall_S, __ARENA_ARENA_CHITINWALL_ROOT/Ramp_E, __ARENA_ARENA_CHITINWALL_ROOT/Pad_taser_dart_gun, __ARENA_ARENA_CHITINWALL_ROOT/Wall_N, __ARENA_ARENA_CHITINWALL_ROOT/Catwalk_E, __ARENA_ARENA_CHITINWALL_ROOT/Ramp_W, __ARENA_ARENA_CHITINWALL_ROOT/Pad_pistol  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Cistern
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_CISTERN_ROOT/Cover_6=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_1=1/1  __ARENA_ARENA_CISTERN_ROOT/PvpBot=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_pistol=1/1  __ARENA_ARENA_CISTERN_ROOT/Wall_W=1/1  __ARENA_ARENA_CISTERN_ROOT/Ramp_N=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_CISTERN_ROOT/Wall_S=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_4=1/1  __ARENA_ARENA_CISTERN_ROOT/Pad_static_net=1/1  __ARENA_ARENA_CISTERN_ROOT/Ramp_S=1/1  __ARENA_ARENA_CISTERN_ROOT/Floor=1/1  __ARENA_ARENA_CISTERN_ROOT/Wall_E=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_5=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_3=1/1  __ARENA_ARENA_CISTERN_ROOT/LightShaftHill=1/1  __ARENA_ARENA_CISTERN_ROOT/Cover_2=1/1  +1 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 19 of 19 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_CISTERN_ROOT/Cover_6, __ARENA_ARENA_CISTERN_ROOT/LightShaftHill, __ARENA_ARENA_CISTERN_ROOT/Cover_3, __ARENA_ARENA_CISTERN_ROOT/Cover_5, __ARENA_ARENA_CISTERN_ROOT/Wall_E, __ARENA_ARENA_CISTERN_ROOT/Floor, __ARENA_ARENA_CISTERN_ROOT/Ramp_S, __ARENA_ARENA_CISTERN_ROOT/Pad_static_net  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_MirrorFlats
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_pistol=2/2  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_5=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Ramp_W=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Floor=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Perch_W=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_N=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_2=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_W=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/PvpBot=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Wall_S=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_prism_beam=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_1=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Ramp_E=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Perch_E=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_4=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_0=1/1  __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_taser_dart_gun=1/1  +2 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 21 of 21 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_5, __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_taser_dart_gun, __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_0, __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_4, __ARENA_ARENA_MIRRORFLATS_ROOT/Perch_E, __ARENA_ARENA_MIRRORFLATS_ROOT/Ramp_E, __ARENA_ARENA_MIRRORFLATS_ROOT/LowCover_1, __ARENA_ARENA_MIRRORFLATS_ROOT/Pad_prism_beam  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Tidal
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_TIDAL_ROOT/Cover_3=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_N=1/1  __ARENA_ARENA_TIDAL_ROOT/PvpBot=1/1  __ARENA_ARENA_TIDAL_ROOT/Pad_pistol=1/1  __ARENA_ARENA_TIDAL_ROOT/Island_SE=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_S=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_N=1/1  __ARENA_ARENA_TIDAL_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_W=1/1  __ARENA_ARENA_TIDAL_ROOT/Cover_1=1/1  __ARENA_ARENA_TIDAL_ROOT/Floor=1/1  __ARENA_ARENA_TIDAL_ROOT/Bridge_W=1/1  __ARENA_ARENA_TIDAL_ROOT/Pad_static_net=1/1  __ARENA_ARENA_TIDAL_ROOT/Island_NE=1/1  __ARENA_ARENA_TIDAL_ROOT/Wall_S=1/1  __ARENA_ARENA_TIDAL_ROOT/Cover_4=1/1  __ARENA_ARENA_TIDAL_ROOT/Island_SW=1/1  __ARENA_ARENA_TIDAL_ROOT/Cover_2=1/1  +4 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 22 of 22 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_TIDAL_ROOT/Cover_3, __ARENA_ARENA_TIDAL_ROOT/Island_NW, __ARENA_ARENA_TIDAL_ROOT/Bridge_E, __ARENA_ARENA_TIDAL_ROOT/Cover_2, __ARENA_ARENA_TIDAL_ROOT/Island_SW, __ARENA_ARENA_TIDAL_ROOT/Cover_4, __ARENA_ARENA_TIDAL_ROOT/Wall_S, __ARENA_ARENA_TIDAL_ROOT/Island_NE  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: Arena_Void
Blockers: 0  Warnings: 2

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __ARENA_ARENA_VOID_ROOT/Up_S=1/1  __ARENA_ARENA_VOID_ROOT/Cover_2=1/1  __ARENA_ARENA_VOID_ROOT/Pad_gravity_gun=1/1  __ARENA_ARENA_VOID_ROOT/Floor=1/1  __ARENA_ARENA_VOID_ROOT/Wall_E=1/1  __ARENA_ARENA_VOID_ROOT/Up_N=1/1  __ARENA_ARENA_VOID_ROOT/Pad_prism_beam=1/1  __ARENA_ARENA_VOID_ROOT/Pad_pistol=1/1  __ARENA_ARENA_VOID_ROOT/Wall_N=1/1  __ARENA_ARENA_VOID_ROOT/Wall_S=1/1  __ARENA_ARENA_VOID_ROOT/Wall_W=1/1  __ARENA_ARENA_VOID_ROOT/Walk_EW=1/1  __ARENA_ARENA_VOID_ROOT/Ring_N=1/1  __ARENA_ARENA_VOID_ROOT/Cover_1=1/1  __ARENA_ARENA_VOID_ROOT/PvpBot=1/1  __ARENA_ARENA_VOID_ROOT/Cover_3=1/1  __ARENA_ARENA_VOID_ROOT/Pad_taser_dart_gun=1/1  __ARENA_ARENA_VOID_ROOT/Cover_4=1/1  +3 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] ART_UNCONFORMED: 21 of 21 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __ARENA_ARENA_VOID_ROOT/Up_S, __ARENA_ARENA_VOID_ROOT/Walk_NS, __ARENA_ARENA_VOID_ROOT/Cover_4, __ARENA_ARENA_VOID_ROOT/Pad_taser_dart_gun, __ARENA_ARENA_VOID_ROOT/Cover_3, __ARENA_ARENA_VOID_ROOT/PvpBot, __ARENA_ARENA_VOID_ROOT/Cover_1, __ARENA_ARENA_VOID_ROOT/Ring_N  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: SpaceLane_Trial
Blockers: 0  Warnings: 3

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): __SPACELANE_ROOT/LaneContent=711/707  __SPACELANE_ROOT/CockpitFrame=3/3  __SPACELANE_ROOT/DockPad=1/1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 711 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] ART_UNCONFORMED: 715 of 715 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. __SPACELANE_ROOT/LaneContent/DebrisField/FreightContainer_16/Ore_5, __SPACELANE_ROOT/LaneContent/Ring_0/Truss/ServiceSpar/Rung_1, __SPACELANE_ROOT/LaneContent/Ring_1/Truss/Brace_11, __SPACELANE_ROOT/LaneContent/Ring_4/Lamps/Lamp_4, __SPACELANE_ROOT/LaneContent/Ring_4/Truss/Girder_9, __SPACELANE_ROOT/LaneContent/DebrisField/DroneArm_26/Clamp_L, __SPACELANE_ROOT/LaneContent/Ring_4/Lamps/Lamp_14, __SPACELANE_ROOT/LaneContent/DebrisField/ArrestorUnit_23/Winding_0  Give each a Forge recipe, or add a whitelist line WITH its WHY.

## Scene: W011_Undercroft
Blockers: 0  Warnings: 3

**Cost by group** (renderers/materials — renderers partition, materials are counted per group so shared ones appear more than once): CaveStal_1_2/CrystalTip=1/1  CavePad_1/Rim1=1/1  CavePad_2/Rim6=1/1  CaveStal_8_2/Spike=1/1  CaveLink_8_3/Bridge=1/1  CavePad_5/Rim7=1/1  CavePad_10/Rim3=1/1  CavePad_10/Rim2=1/1  CaveStal_1_1/Spike=1/1  CaveStal_6_0/Spike=1/1  CaveStal_2_0/Spike=1/1  CaveStal_6_1/Spike=1/1  CavePad_0/Rim5=1/1  CavePad_3/Floor=1/1  CaveLink_8_10/Bridge=1/1  CavePad_10/Rim4=1/1  CavePad_8/Rim7=1/1  CavePad_7/Rim3=1/1  +160 more group(s)

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.
- [WARNING] PERF_MATERIALS_OVER_CAP: unique materials = 178 exceeds the HARD CAP 60 (QUEST_ART_AUDIO_PERFORMANCE_BUDGET) — promote this to a blocker after baselining.
- [WARNING] ART_UNCONFORMED: 178 of 178 renderer(s) have no known art provenance (Forge look / kit module / SkyVista / water / grounding / practical / whitelist).  e.g. CaveStal_1_2/CrystalTip, CavePad_10/Rim5, CavePad_10/Rim0, CavePad_9/Rim0, CavePad_8/Rim5, CavePad_8/Rim6, CavePad_1/Rim4, CaveLink_10_2/Bridge  Give each a Forge recipe, or add a whitelist line WITH its WHY.

