# Milestone D3.2 — World Integrity Audit + Spawn Safety

## Goal

Stop discovering scene bugs in-headset. Add a headless build-time audit that catches broken spawns, missing travel doors, and city geometry problems **before** the APK is generated.

## Changes

| File | Action |
|------|--------|
| `Assets/Ziptide/Editor/Audit/WorldAuditReport.cs` (new) | Data model: AuditSeverity, AuditFinding, SceneAuditReport, WorldAuditReport |
| `Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs` (new) | Runs all checks, writes docs/AUDIT_REPORT.md + AUDIT_REPORT.json, logs AUDIT_OK/AUDIT_FAIL |
| `Assets/Ziptide/Editor/Build/BuildAndroid.cs` | Calls WorldAuditRunner.RunAll() before APK; throws on blockers |
| `Assets/Ziptide/Gameplay/Runtime/Player/EmergencyRespawn.cs` (new) | Hold both grips 1s → respawn, log ZIPTIDE: EMERGENCY_RESPAWN |
| `Assets/Ziptide/Editor/Patching/ScenePatcherD2.cs` | D0_City spawn moved to (0, 2.6, -16) on CourtyardA; adds EmergencyRespawn to XR Origin |
| `tools/quest_smoke.ps1` | Scans `Builds/android_build.log` for ZIPTIDE: AUDIT_FAIL before device logcat checks |
| `docs/AUDIT_REPORT.md` (generated) | Human-readable audit output — regenerated every build |
| `docs/AUDIT_REPORT.json` (generated) | Machine-readable audit output |

---

## How to run the audit

### Automatically (every build)

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat
```

The audit runs as part of `BuildAndroid.PatchScenesThenAPK()`. If any BLOCKER is found, the build fails before generating the APK.

### Manually from Unity menu

`Ziptide > Audit > Run Audit (All Scenes)`

---

## What BLOCKER vs Warning means

| Severity | Meaning | Effect on build |
|----------|---------|-----------------|
| **BLOCKER** | Player will be stuck, in void, or unable to return from scene | **Fails the build** |
| **Warning** | Missing recommended feature (ramp, courtyard, coordinator) | Build proceeds; fix when convenient |

---

## Audit checks

### Spawn checks (all scenes)

| Code | Severity | Description |
|------|----------|-------------|
| `SPAWN_MISSING` | Blocker | No `SpawnMarkerRuntime` or `__SPAWN_PLAYER` object |
| `SPAWN_NO_FLOOR` | Blocker | No collider within 6m below spawn (would fall into void) |
| `SPAWN_OVERLAP_SOLID` | Blocker | Solid collider overlaps spawn radius (would spawn inside geometry) |
| `SPAWN_BELOW_WALKWAY` | Blocker | Spawn Y is below `walkwayHeight - 0.25m` in city scenes |
| `SPAWN_TRAPPED` | Blocker | Fewer than 2/8 radial directions unblocked within 1.5m (hard stuck) |

### Travel checks (all scenes)

| Code | Severity | Description |
|------|----------|-------------|
| `TRAVEL_NO_COORDINATOR` | Warning | No TravelCoordinator in scene |
| `TRAVEL_NO_DOOR` | Blocker | Travel scenes must have ≥1 WorldTravelStation or ProximityTravelTrigger |
| `TRAVEL_DEST_NOT_IN_BUILD` | Blocker | Referenced destination scene not in enabled build scenes |

### City geometry checks (D0_City only)

| Code | Severity | Description |
|------|----------|-------------|
| `CITY_NO_ROOT` | Blocker | `__D1_CITY_ROOT` not found; D1 patcher did not run |
| `CITY_NO_RAMP` | Warning | No ramp found; all-level access may be limited |
| `CITY_NO_COURTYARD` | Warning | No courtyard found; no safe platform areas |

---

## How to read AUDIT_REPORT.md

The file is regenerated every build at `docs/AUDIT_REPORT.md`.

- Each scene section lists findings as `[BLOCKER] CODE: message @ path` or `[WARNING] CODE: message @ path`.
- Fix all BLOCKERs before the APK will build.
- `docs/AUDIT_REPORT.json` has the same data in structured form for scripts.

---

## Emergency Respawn (anti-stuck)

**Hold Left Grip + Right Grip for 1 second** at any time to teleport back to the scene's spawn marker.

- Triggers `WorldRuntime.RespawnPlayer()` if available, otherwise teleports to `SpawnMarkerRuntime`.
- Logs `ZIPTIDE: EMERGENCY_RESPAWN` to logcat.
- Component `EmergencyRespawn` is added to the XR Origin by `ScenePatcherD2` in every scene.

---

## Spawn fix: D0_City

Spawn position in D0_City moved from `(0, 2.6, -12)` to `(0, 2.6, -16)` to align with `CourtyardA_Spawn` in the D1 city layout. This puts the player on the elevated concrete courtyard instead of potentially between walkways.

---

## 2-Minute Quest Verification

After build/install:

1. Start in Test Room — can see belt, holsters, gun
2. Travel to Toxic City — spawn should be on elevated concrete courtyard, not in canal
3. Look down — toxic green surface should be below you
4. Hold both grips for 1 second — you should respawn on the courtyard, logcat shows `ZIPTIDE: EMERGENCY_RESPAWN`
5. Travel back to Test Room — door works
6. On next build, check `docs/AUDIT_REPORT.md` — should show `ZIPTIDE: AUDIT_OK blockers=0`

## Failure modes

| Symptom | Logcat / build log keyword |
|---------|---------------------------|
| Build fails before APK | `ZIPTIDE: AUDIT_FAIL blockers=N` in build log |
| Spawn still in void | `SPAWN_NO_FLOOR` in AUDIT_REPORT.md |
| Player spawns inside geometry | `SPAWN_OVERLAP_SOLID` |
| Spawn below toxic surface | `SPAWN_BELOW_WALKWAY` |
| Cannot get unstuck | Check `ZIPTIDE: EMERGENCY_RESPAWN` absent from logcat |
