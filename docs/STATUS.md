# STATUS — LLM dashboard

**Single source of truth for "what exists". Check this first every session.**

---

## Current Milestone

- **Phase:** A — **COMPLETE** (Grab Cube on Quest).
- **Phase A.5:** World Dressing (Sky + Planet + Theme) — **COMPLETE**.
- **Phase B:** Play-Area Safety + WorldProfile + Theme Switch Station — **COMPLETE**.
- **Phase B.1:** Thumbstick Locomotion + Bounds Contract — **COMPLETE**.
- **Phase C0:** Belt + Holster + Pistol v0 + Targets — **COMPLETE**.
- **Milestone D0:** D0 City scene + Job/Tutorial v0 + travel — **COMPLETE**.
- **Milestone D1:** Toxic Venice city foundation — **COMPLETE**.
- **Milestone D2:** Controls + Persistence + City Traversal Expansion — **COMPLETE**.
- **Milestone D2.1:** Fixpack (holster tweak, pistol persistence, rays after travel, faster smooth turn) — **COMPLETE**.
- **Milestone D2.2:** Critical fixes (save/restore inventory, door manager wiring) — **COMPLETE**.
- **Milestone D3:** Audio system + Taser dart gun + Drones + Job integration — verify on device (see [MILESTONE_D3_AUDIO_TASER_DRONES.md](MILESTONE_D3_AUDIO_TASER_DRONES.md)).
- **Milestone D3.1:** Travel Stability Fixpack — TravelCoordinator + ProximityTravelTrigger + real socket SelectEnter — **COMPLETE** (see [MILESTONE_D31_TRAVEL_STABILITY.md](MILESTONE_D31_TRAVEL_STABILITY.md)).
- **Milestone D3.2:** World Integrity Audit + Spawn Safety — headless build audit, EmergencyRespawn (both grips 1s), D0_City spawn on CourtyardA — **COMPLETE** (see [MILESTONE_D32_AUDIT_SPAWN_SAFETY.md](MILESTONE_D32_AUDIT_SPAWN_SAFETY.md)).

---

## Working on device

- **Milestone A:** Scene runs on Quest; environment visible (ground, horizon, sky); cube visible and grabbable; locomotion works.
- **Milestone A.5:** Data-driven world dressing: VisualThemeProfile, SkyPlanetRig (sky + planet), WorldDirector.
- Asmdef skeleton in place (Core, Gameplay, Content, Visuals, Ship, Platform.Quest, Editor).
- **Milestone B:** WorldProfile, WorldRuntime, PlayAreaBounds, FallRespawner, ThemeSwitchStation.
- **Milestone B.1:** Thumbstick locomotion rig; CC on XR Origin root; bounds block stick movement; gravity/respawn work.
- **Milestone C0:** Belt holsters (left, center, right at hip level), pistol with fire/haptic/muzzle flash, targets.
- **Milestone D0:** D0_City scene, JobDirector, DispatchKiosk, ObjectiveBoard, DeliveryCradle, WorldTravelStation (travel between scenes).
- **Milestone D1:** Toxic Venice canal city: elevated walkways, bridges, courtyards, railings, toxic surface, building shells. Travel station uses door visuals with displayName labels.
- **Milestone D2:** LocomotionProfile (smooth/snap turn toggle), DashLocomotion (A button dash/hop), PlayerRigPersistence (DontDestroyOnLoad XR Origin + inventory across travel), SpawnMarkerRuntime, expanded city (loop routes, perimeter walkways, 5 courtyards, service catwalks, ramps, 7 buildings, full railings).
- **Milestone D2.1:** Front holster moved closer; persistent inventory root; XRI manager rebinding after travel; smooth turn default 120 deg/s.
- **Milestone D2.2:** InventoryState save/restore replaces reparenting; ItemFactory runtime recreation; door explicit manager assignment.
- **Milestone D3:** AudioProfile + AudioDirector (crossfade BGM), TaserDartGunDefinition + TaserDartGunRuntime + TaserDartProjectile (sticky darts), IShockable + DroneRuntime (hovering targets), DisableDronesCountStep job type, SingletonValidator.
- **Milestone D3.1:** TravelCoordinator singleton (save→XRI-ready gate→restore), ProximityTravelTrigger (walk-through failsafe on doors), InventoryState.RestoreAfterTravel IEnumerator with real mgr.SelectEnter socket selection.
- **Milestone D3.2:** WorldAuditRunner (headless build-time checks: spawn/door/city geometry, BLOCKER fails build), EmergencyRespawn (hold both grips 1s), D0_City spawn moved to CourtyardA (0,2.6,-16), quest_smoke scans build log for AUDIT_FAIL.
- **Ziptide menu:** Ziptide > Apply Quest player defaults; Create Milestone A scene; Create Default Visual Theme; Apply Theme To Current Scene; Create Default World Profile; Apply World Profile To Current Scene; Ensure Thumbstick Locomotion Rig; Print Locomotion Debug; Apply C0 To Current Scene; Apply D0 City To Current Scene; Apply D1 City To Current Scene; **Apply D2 Controls + Persistence To Current Scene**; Diagnostics > XR Grab Readiness.
- **Build pipeline:** `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1` (with optional `-Logcat`). Runs ScenePatcherC0 + ScenePatcherD0 + ScenePatcherD1 + **ScenePatcherD2** then APK. Optional smoke: `tools\quest_smoke.ps1`.
- **GPT sync:** `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\gpt_sync.ps1` outputs `GPT_SYNC.md`.

---

## Blocking issues

- None currently. Build and verify D3 on device.

---

## Next 3 tasks (exact)

1. **Build/install D3:** Run `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat`. On Quest: gun persists across travel, return door works, background music plays in city, drones visible, taser dart gun works.
2. **If D3 fails:** Run `adb logcat -s Unity Ziptide AndroidRuntime`; check for `ZIPTIDE: DUP_SINGLETON`, `INVENTORY_RESTORE_FAIL`, `ITEM_DEF_NOT_FOUND`, or `AUDIO_CLIP_MISSING`.
3. **Commit and push** after D3 passes: `git add -A`, `git commit -m "D3: audio system, taser dart gun, drones, job integration, inventory save/restore"`, `git push`.

---

## Repo links + build target settings

- **Repo:** https://github.com/TerryMaloney/Ziptide.git (public)
- **Build target:** Meta Quest (Unity), Android, ARM64, IL2CPP, API 29
- **Unity version:** 2022.3.62f3
- **URP:** Yes (Balanced for Android)

---

*This file prevents drift by forcing "centerline" every session.*
