# Milestone D3: Audio System + Taser Dart Gun + Drones + Job Integration

## Summary
- **AudioProfile** ScriptableObject for data-driven music per world.
- **AudioDirector** DontDestroyOnLoad singleton with two AudioSources for crossfade.
- **WorldPackDefinition** extended with optional `audioProfile` field.
- **TaserDartGunDefinition** extends ItemDefinition with projectile + stun parameters.
- **TaserDartGunRuntime** fires dart projectiles on trigger; darts stick on collision.
- **TaserDartProjectile** sticks to targets, calls `IShockable.Shock()` and `TargetRuntime.Hit()`.
- **IShockable** interface in Core for shock-compatible entities.
- **DroneRuntime** hovering drone (Active/Shocked/Recovering states). Fires `OnDroneDisabled` event.
- **DisableDronesCountStepDefinition** job step type.
- **JobRuntime** and **JobDirector** updated to count drone disables.
- **SingletonValidator** runtime check for duplicate singletons after scene travel.
- **InventoryState** save/restore system replaces reparenting-based persistence.
- **ItemFactory** runtime factory to recreate items by itemId after scene travel.
- Holsters now accept `taser_dart_gun` in addition to `pistol`.

## D2.2 Critical Fixes
- **Gun persistence**: Items are serialized to a static list before scene load and recreated cleanly in the new scene, avoiding XRI selection state corruption.
- **Return door**: WorldTravelStation explicitly assigns XRInteractionManager with retry coroutine.
- **Door position**: D0_City return door moved to (0, 2.6, -10) directly ahead of spawn.

## 2-Minute Quest Test Checklist
1. Build and install:
   ```
   powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat
   ```
2. Launch app on Quest.
3. **Test Room**: Pick up pistol, holster it, pick up taser dart gun (teal cube on table).
4. Grab pistol from holster, shoot targets.
5. Walk through door to Toxic City.
6. **Verify**: Pistol/taser recreated in new scene (near player or in holster).
7. **Verify**: Background music plays in Toxic City.
8. **Verify**: 3 drones visible hovering above walkways (red spheres with fins).
9. Shoot a drone with taser dart gun: dart sticks, drone turns blue, drops, then recovers.
10. Walk to return door, travel back to Test Room.
11. **Verify**: Music stops (or stays silent) in Test Room.
12. **Verify**: Pistol/taser recreated again.

## Failure Modes & Logcat Keywords
| Keyword | Meaning |
|---------|---------|
| `ZIPTIDE: DUP_SINGLETON` | Multiple instances of a critical singleton |
| `ZIPTIDE: INVENTORY_SAVE` | Item save before travel (expected) |
| `ZIPTIDE: INVENTORY_RESTORE` | Item restore after travel (expected) |
| `ZIPTIDE: INVENTORY_RESTORE_FAIL` | ItemFactory could not create item |
| `ZIPTIDE: ITEM_DEF_NOT_FOUND` | Missing ItemDefinition asset |
| `ZIPTIDE: AUDIO_CLIP_MISSING` | AudioProfile has no clip (warning only) |
| `ZIPTIDE: DRONE_SHOCKED` | Drone was shocked (expected) |
| `ZIPTIDE: DOOR_NO_MANAGER` | Door couldn't find XRInteractionManager |
| `ZIPTIDE: DOOR_MANAGER_RETRY_OK` | Manager found on retry |

## Known Limitations
- Taser tether (pull-back mechanic) not implemented yet.
- Drones don't have patrol paths; they bob in place.
- Audio crossfade may not smooth if travel is very fast.
- Dart gun has no custom model (uses teal cube primitive).
