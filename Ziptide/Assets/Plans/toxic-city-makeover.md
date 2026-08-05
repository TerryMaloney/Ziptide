# Project Overview
- Game Title: Ziptide
- High-Level Concept: An immersive industrial/sci-punk world exploration and action game where players travel between wet industrial city worlds on salvage rigs, solving contracts, repairing machinery, and traversing dangerous environments.
- Players: Single player (with VR/XR support via XR Interaction Toolkit)
- Inspiration / Reference Games: Cyberpunk 2077, Dishonored, Half-Life, Outer Wilds
- Tone / Art Direction: Sci-punk, industrial, wet, rusty, hazardous, atmospheric, dark teal zenith & neon/amber accents.
- Target Platform: StandaloneWindows64 / Android (Quest)
- Screen Orientation / Resolution: VR / Landscape 1920x1080
- Render Pipeline: URP-HighFidelity

# Game Mechanics
## Core Gameplay Loop
The player spawns at the Dispatch district plaza, receives a contract from the Kiosk, uses tools (taser, gravity gun, ziplines) to navigate and bypass hazard zones or drones, repairs key machinery (like the CanalRow signal relay), and heads to the Shipyard berth to board their ship and transit to other worlds.

## Controls and Input Methods
Dual-hand XR/VR input (using XR Interaction Toolkit): grabbing tools, navigating via teleport/continuous movement, aiming and shooting target blocks/drones, riding ziplines, and interacting with physical console buttons, and world doors.

# UI
The user interface is entirely physicalized/diegetic in world space (such as the physical Objective Board in the plaza, the Dispatch Kiosk terminal, and boarding panels next to berths) to maximize VR immersion.

# Key Asset & Context
- **Layout Asset**: `Assets/Ziptide/Content/City/ToxicCityLayout.asset` (holds the world topology, district data, and ring city options)
- **Building Style Assets**:
  - `Assets/Ziptide/Resources/BuildingStyles/toxic_tenement.asset` (corrugated rusty iron, slum tenements style)
  - `Assets/Ziptide/Resources/BuildingStyles/salvage_row.asset` (industrial shipyard, weathered look)
- **Patcher / Builder Scripts**:
  - `Assets/Ziptide/Editor/Patching/ScenePatcherToxicCity.cs` (triggers the full bake/regeneration from the layout asset)
  - `Assets/Ziptide/Editor/Patching/CityBuilder.cs` (builds the core districts, connections, and dressing)
  - `Assets/Ziptide/Editor/Patching/RingCityBuilder.cs` (builds the concentric rings, leaning tower, and outskirts)
  - `Assets/Ziptide/Editor/Patching/BuildingBuilder.cs` (generates procedural enterable sci-punk buildings)

# Implementation Steps
1. **Modify the ToxicCityLayout Asset**: Update properties in `Assets/Ziptide/Content/City/ToxicCityLayout.asset` to:
   - Enable `rings.buildTowerIsland` (set to `true`) to generate the leaning scrap tower.
   - Enable `rings.buildWedges` (set to `true`) to generate the surrounding shanty wedges of stacked tenements.
   - Assign `buildingStyleId` to districts to replace flat colored rectangular boxes with detailed procedurally generated buildings:
     - `Shipyard`: `"salvage_row"`
     - `Dispatch`: `"salvage_row"`
     - `Plaza`: `"toxic_tenement"`
     - `Market`: `"toxic_tenement"`
     - `CanalRow`: `"toxic_tenement"`
   - **Assigned role**: developer
   - **Dependencies**: None
   - **Parallelizable**: No

2. **Run the World Builder**: Trigger the menu command `Ziptide/Worlds/Build Toxic City` (which executes `ScenePatcherToxicCity.BuildFromMenu()`). This will procedurally re-bake the entire city layout, including the newly enabled central leaning tower, the surrounding shanty wedges, and procedurally generate enterable tenement buildings for all districts.
   - **Assigned role**: developer
   - **Dependencies**: Step 1
   - **Parallelizable**: No

3. **Verify and Audit the Scene**: 
   - Inspect the newly generated objects inside `Assets/Ziptide/Scenes/ToxicCity.unity` to confirm visual and layout correctness.
   - **Assigned role**: explorer
   - **Dependencies**: Step 2
   - **Parallelizable**: No

# Verification & Testing
- **Visual Check**:
  - Confirm the central tower leans by 10-15 degrees and has cranes at the crown.
  - Confirm shanty wedges of stacked tenements fill the space between the canal ring and sea wall, complete with lit windows.
  - Confirm the districts have procedurally-generated detailed walls, doors, window frames, and interior dressing.
- **Unit & Integration Tests**:
  - Run the existing edit mode tests in `Assets/Ziptide/Tests/EditMode/` to ensure no regressions:
    - `ToxicCityStageBTests.cs` (Verifies stage B layout)
    - `ToxicCityStageAIntegrationTests.cs` (Verifies stage A layout)
    - `CityLandmarkAuthoringTests.cs` (Verifies landmark scales)
