# World Audit Report

**Total Blockers:** 2  
**Total Warnings:** 2

## Scene: _Boot
Blockers: 0  Warnings: 0

_No issues found._

## Scene: MilestoneA_GrabCube
Blockers: 2  Warnings: 1

- [BLOCKER] WORLD_SCENE_HAS_XRI_MANAGER: World scene must not contain XRInteractionManager. It must live only in _Boot (DontDestroyOnLoad). @ XR Interaction Manager
- [BLOCKER] SPAWN_MISSING: No SpawnMarkerRuntime or __SPAWN_PLAYER found in scene.
- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.

## Scene: D0_City
Blockers: 0  Warnings: 1

- [WARNING] TRAVEL_NO_COORDINATOR: No TravelCoordinator in scene. It may arrive as DontDestroyOnLoad from another scene, but verify patcher ran.

