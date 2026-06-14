# Milestone D3.1 — Travel Stability Fixpack

## Goal

Fix the two persistent travel bugs that survived D2/D2.2:

1. **Gun not holstered after traveling to another scene.**
2. **Return door unresponsive** (cannot travel from Toxic City back to Practice Room).

## Root Cause Summary

| Bug | Root Cause |
|-----|-----------|
| Gun not holstered | `InventoryState.TryHolster()` only moved the item near the socket; it never called `XRInteractionManager.SelectEnter()`. XRI sockets do **not** auto-grab nearby items. |
| Return door broken | `WorldTravelStation` built the door one frame late, after the XRI wiring pass, so manager assignment was unreliable. Also no fallback path if ray-select failed. |

## Changes

| File | Change |
|------|--------|
| `InventoryState.cs` | `RestoreAfterTravel` → `IEnumerator`, `TryHolsterCoroutine` calls real `mgr.SelectEnter()` |
| `PlayerRigPersistence.cs` | Uses `StartCoroutine(InventoryState.RestoreAfterTravel(...))` (was synchronous) |
| `TravelCoordinator.cs` (new) | DontDestroyOnLoad singleton — single entry point for all travel, XRI-ready gate (5 s timeout), orchestrates save→load→restore |
| `ProximityTravelTrigger.cs` (new) | Walk-through trigger on door frames — fires scene load even if rays are broken |
| `WorldTravelStation.cs` | `LoadScene()` delegates to `TravelCoordinator.TravelTo()` |
| `ScenePatcherC0.cs` | Adds `ProximityTravelTrigger` to practice room travel station |
| `ScenePatcherD0.cs` | Adds `ProximityTravelTrigger` to D0_City travel station |
| `ScenePatcherD2.cs` | `EnsureTravelCoordinator()` — places `__TravelCoordinator` in every scene |
| `quest_smoke.ps1` | Catches `TRAVEL_FAIL`, `XRI_NOT_READY` |

## 2-Minute Quest Checklist

After building and installing (`dev_build_install.ps1`):

- [ ] Practice room loads — can see belt holsters and gun spawn
- [ ] Grab the gun and holster it on the center holster
- [ ] Walk up to or select the door → travel to Toxic City
- [ ] In Toxic City: gun should be visible and holstered on your hip
- [ ] Unholster and re-holster the gun
- [ ] Walk up to or select the return door → travel back to Practice Room
- [ ] Back in Practice Room: gun still holstered / in hand
- [ ] Controllers (rays) functional in both scenes
- [ ] No frozen view / environment moving with head

## Expected Logcat (healthy)

```
ZIPTIDE: TRAVEL_START dest=D0_City
ZIPTIDE: INVENTORY_SAVE count=1
ZIPTIDE: INVENTORY_SAVED item=pistol slot=holster_center
ZIPTIDE: XRI_READY elapsed=0.05s
ZIPTIDE: INVENTORY_RESTORE count=1
ZIPTIDE: HOLSTER_RESTORED item=Pistol slot=holster_center
ZIPTIDE: TRAVEL_OK dest=D0_City
```

## Failure Mode → Logcat Keyword

| Symptom | Keyword |
|---------|---------|
| Travel fails entirely | `ZIPTIDE: TRAVEL_FAIL` |
| XRI not ready after 5 s | `ZIPTIDE: XRI_NOT_READY` |
| Item not found in registry | `ZIPTIDE: ITEM_DEF_NOT_FOUND` |
| Socket SelectEnter failed | `ZIPTIDE: INVENTORY_RESTORE_FAIL` |
| Duplicate singleton | `ZIPTIDE: DUP_SINGLETON` |

## Known Limitations

- `TravelCoordinator.IsXRIReady()` requires ≥2 enabled `XRRayInteractor`s. If the rig only has 1 ray on device this will timeout and log `XRI_NOT_READY` but travel will proceed anyway.
- `ProximityTravelTrigger` fires on the XR Origin's character-controller collider. Ensure the `CharacterController` is on the `XR Origin` root (not on `Camera Offset`) for detection to work.
