# Milestone C0 — Belt, Pistol, Targets

**Definition of done:** Build runs ScenePatcher then APK; on Quest: grab pistol, holster on right hip, unholster, pull trigger for hitscan; three targets react (color change / wobble) when hit.

---

## 2-minute test (after implementation)

1. **From Cursor:**  
   `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat`

2. **On Quest:**
   - Grab the pistol (cube-shaped, ~1.2 m in front of spawn).
   - Holster on right hip (socket accepts pistol).
   - Unholster by grabbing again.
   - Pull trigger to fire (hitscan); hit the three targets in front — each should show visible response (e.g. red → green, optional wobble), then reset after ~1–2 s.

---

## Failure modes and logcat

- **Pistol not spawning:** ScenePatcher may not have run or `DefaultPistol.asset` missing; run **Ziptide > Apply C0 To Current Scene** in Unity or re-run build (PatchScenesThenAPK).
- **Cannot holster:** HolsterSocketInteractor only accepts items with `ItemRuntime` whose `definition.itemId` is in allowed list (e.g. `"pistol"`). Ensure DefaultPistol has `itemId = "pistol"`.
- **No haptics / no hit feedback:** Check controller interactor and TargetRuntime on targets; ensure layers allow raycast hit.

**Logcat (clear then filter):**
```bash
adb logcat -c
adb logcat -s Unity Ziptide AndroidRuntime ActivityManager
```

---

## What C0 adds (idempotent, additive)

- **ScenePatcherC0:** Ensures BeltRig, HolsterRight (HolsterSocketInteractor), one Pistol (ItemRuntime + PistolRuntime + Muzzle), three TargetRuntime cubes. Does not remove/rename existing objects (WorldRuntime, ThemeSwitchStation, cube, etc.).
- **Build:** `dev_build_install.ps1` calls `BuildAndroid.PatchScenesThenAPK` so patching runs every build.
- **Menu:** **Ziptide > Apply C0 To Current Scene** for one-off scene patch without building.

---

*Keep this doc in sync with STATUS.md and 06_SCHEMAS.md.*
