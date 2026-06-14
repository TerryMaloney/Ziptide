# Milestone D0 — City + Job

**D0 City scene, data-driven Job/Tutorial v0, travel between test room and D0.**

---

## Two-minute test (after implementation)

1. **From Cursor**
   ```powershell
   powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat
   ```

2. **On Quest**
   - **Test room:** Holster left/right works; pistol fires; targets react.
   - Use **TravelStation** (world button) to go to D0 City.
   - In D0: accept job at **DispatchKiosk**; follow **ObjectiveBoard** steps; complete job.
   - Use **TravelStation** in D0 to travel back to test room.

---

## Failure modes + logcat

- **Black ball / holster:** Belt front marker removed; left + right holsters raised. If holsters missing, run **Ziptide > Apply C0 To Current Scene** and rebuild.
- **No travel button:** Ensure **WorldTravelStation** exists in scene; C0 patcher adds it to test room with D0 destination; D0 patcher adds it with test room destination. Re-run build so both patchers run.
- **Job not starting:** Ensure **JobDirector** has **WorldPackDefinition** (D0_WorldPack) with at least one **JobDefinition** in `jobs` list. Create job assets under `Assets/Ziptide/Content/Jobs/` and add to pack.
- **ObjectiveBoard empty:** **JobDirector** and **ObjectiveBoard** find each other at runtime; ensure both exist in D0 scene (patcher creates them). Accept a job at kiosk first.

**Logcat commands**
```powershell
adb logcat -c
adb logcat -s Unity Ziptide AndroidRuntime ActivityManager
```
Logcat saved to `Builds/quest_logcat.log` when using `dev_build_install.ps1 -Logcat`.

**Optional smoke script** (build, install, capture logcat, scan for exceptions):
```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\quest_smoke.ps1
```
Exits non-zero if `Exception`, `NullReferenceException`, or `AndroidRuntime FATAL` appear in logcat.

---

## Schemas

See [06_SCHEMAS.md](06_SCHEMAS.md): **WorldPackDefinition**, **JobDefinition**, **JobStepDefinition** (GoToMarker, CollectItemIdCount, DeliverToSocket, ShootTargetsCount), **SpawnMarkerDefinition**.

---

*Keep test room as first scene in Build Settings; D0 patcher appends D0_City.*
