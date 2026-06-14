# Milestone D2.1: Fixpack (surgical)

## Goal
- Fix front holster being too far out.
- Ensure pistol/items persist across `WorldTravelStation` travel.
- Ensure rays/grab still work after travel (no disappearing controllers/rays).
- Slightly faster smooth turn default.

## Key numbers
- **Smooth turn**: **120 deg/s** (was 90).
- **Front holster Z offset**: **0.09m** (was 0.15m).

## Quest checklist (2 minutes)
1. Run: `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1 -Logcat`
2. Start in City: **rays visible** + **grab works**.
3. Holster pistol (right or front). Travel to Test Room. **Pistol still present + holstered.** Rays still visible.
4. Unholster in Test Room, fire once, re-holster. Travel back to City. **Pistol still present.** Rays still visible.
5. Confirm smooth turn feels faster (120 deg/s).
6. Confirm front holster marker is closer (less “sticking out”) and marker sphere not huge.

## Failure modes (logcat)
- `ZIPTIDE: XRI_MISSING`
- `ZIPTIDE: INPUT_ACTIONS_MISSING`
- `ZIPTIDE: NO_RAY_INTERACTORS`

