# Milestone D1: Toxic Venice City Foundation

## Summary

Adds the first real city layout to the D0_City scene: "Toxic Venice" -- an elevated canal/bridge city above toxic sludge.

## What was added

- **CityKitDefinition** (ScriptableObject): data-driven config for canal width, walkway height, bridge width, railing params, toxic color, seed.
- **ScenePatcherD1**: idempotent editor patcher that generates the city layout under `__D1_CITY_ROOT`:
  - Toxic surface plane (green, non-walkable, visual only)
  - Central elevated walkway + two side walkways
  - Two canals with retaining walls
  - 3 bridges per canal side connecting walkways
  - 3 courtyards (Spawn, Dispatch, Garden) at walkway height
  - Planters on the upper garden courtyard
  - Railings along all dangerous edges and bridge sides
  - 5 building shells (block primitives, no interiors)
- **Travel door**: WorldTravelStation now renders as a door frame instead of a flat sign panel. Labels use `WorldPackDefinition.displayName` ("To Toxic City", "To Test Room").
- **Holster fix**: belt holsters repositioned to actual hip sides instead of bunched in front.
- Old D0_Blockout removed (replaced by D1 city geometry).
- D0 gameplay objects (DispatchKiosk, ObjectiveBoard, DeliveryCradle) repositioned to elevated courtyards.

## Quest Test Checklist (2 minutes)

1. **Test Room spawn**: holsters at hip sides (left/right), not bunched in front. Center holster at front.
2. **Travel door**: visible door frame labeled "To Toxic City". Point ray and pull trigger to travel.
3. **D0_City loads**: elevated walkways and bridges over toxic green surface below.
4. **Walk the loop**: central walkway -> bridge -> side walkway -> bridge back. Railings present on edges.
5. **No fall-through**: walkways and bridges have colliders. Walking on them feels solid.
6. **Toxic zone visible**: green surface clearly visible below walkway level.
7. **Pistol + holsters**: still work in the city scene.
8. **Dispatch kiosk**: visible on Courtyard B. Job system still wired.
9. **Travel back**: door on Courtyard A labeled "To Test Room" returns to test scene.

## Build & Verify

```
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat
```

## Files

| Type | Path |
|------|------|
| SO | `Assets/Ziptide/Content/Runtime/City/CityKitDefinition.cs` |
| Patcher | `Assets/Ziptide/Editor/Patching/ScenePatcherD1.cs` |
| Modified | `Assets/Ziptide/Editor/Patching/ScenePatcherD0.cs` |
| Modified | `Assets/Ziptide/Editor/Build/BuildAndroid.cs` |
| Modified | `Assets/Ziptide/Gameplay/Runtime/WorldTravelStation.cs` |
| Modified | `Assets/Ziptide/Content/Runtime/WorldPacks/WorldPackDefinition.cs` |
| Modified | `Assets/Ziptide/Gameplay/Runtime/Inventory/BeltRig.cs` |
| Modified | `Assets/Ziptide/Editor/Patching/ScenePatcherC0.cs` |
