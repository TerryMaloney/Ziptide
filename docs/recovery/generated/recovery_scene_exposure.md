# ZIPTIDE Recovery Scene Exposure

- Build-settings scenes: **24**
- Enabled: **24**
- Prototype/legacy/unclassified scenes still enabled: **20**
- Golden destination candidates still unresolved: **2**

## Exposure counts

- **GOLDEN_DESTINATION_CANDIDATE:** 2
- **GOLDEN_PATH:** 1
- **GOLDEN_PATH_SUPPORT:** 1
- **LEGACY_TEST_EXPOSED:** 4
- **PROTOTYPE_MULTIPLAYER_EXPOSED:** 6
- **PROTOTYPE_WORLD_EXPOSED:** 10

## Enabled scene table

| # | Scene | Recovery exposure | Reason |
|---:|---|---|---|
| 0 | `_Boot` | `GOLDEN_PATH_SUPPORT` | Persistent bootstrap scene. |
| 1 | `MilestoneA_GrabCube` | `LEGACY_TEST_EXPOSED` | Legacy/test scene remains enabled in the APK. |
| 2 | `D0_City` | `LEGACY_TEST_EXPOSED` | Legacy/test scene remains enabled in the APK. |
| 3 | `SandboxTestLab` | `LEGACY_TEST_EXPOSED` | Legacy/test scene remains enabled in the APK. |
| 4 | `StarterWorld` | `LEGACY_TEST_EXPOSED` | Legacy/test scene remains enabled in the APK. |
| 5 | `ToxicCity` | `GOLDEN_DESTINATION_CANDIDATE` | Potential single destination; R0 must select exactly one. |
| 6 | `PvP_Arena01` | `PROTOTYPE_MULTIPLAYER_EXPOSED` | Multiplayer is frozen and should be hidden from recovery candidates. |
| 7 | `W000_DriftIn` | `GOLDEN_PATH` | Required first world in the recovery slice. |
| 8 | `W002_DryCistern` | `GOLDEN_DESTINATION_CANDIDATE` | Potential single destination; R0 must select exactly one. |
| 9 | `W003_GlassShelf` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 10 | `W004_BroadcastTomb` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 11 | `W005_OxidizedCanopy` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 12 | `W006_MirrorFlats` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 13 | `W007_SableStation` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 14 | `W008_SealedArchive` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 15 | `W009_Chitinwall` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 16 | `W010_TidalArray` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 17 | `W011_TheHum` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 18 | `W012_MarasLastJump` | `PROTOTYPE_WORLD_EXPOSED` | Non-golden story world remains enabled before integration proof. |
| 19 | `Arena_Chitinwall` | `PROTOTYPE_MULTIPLAYER_EXPOSED` | Multiplayer is frozen and should be hidden from recovery candidates. |
| 20 | `Arena_Cistern` | `PROTOTYPE_MULTIPLAYER_EXPOSED` | Multiplayer is frozen and should be hidden from recovery candidates. |
| 21 | `Arena_MirrorFlats` | `PROTOTYPE_MULTIPLAYER_EXPOSED` | Multiplayer is frozen and should be hidden from recovery candidates. |
| 22 | `Arena_Tidal` | `PROTOTYPE_MULTIPLAYER_EXPOSED` | Multiplayer is frozen and should be hidden from recovery candidates. |
| 23 | `Arena_Void` | `PROTOTYPE_MULTIPLAYER_EXPOSED` | Multiplayer is frozen and should be hidden from recovery candidates. |

## Recovery conclusion

The source inventory can classify a system as hidden while its scene remains enabled in the APK. R1/R3 therefore need a generated recovery build profile or an equivalent exposure gate; R0 does not edit build settings.
