# Milestone A.5 — World Dressing (Sky + Planet + Theme Profiles)

**Status:** Implemented. Verify on device before marking complete.

---

## What was added

- **VisualThemeProfile** (ScriptableObject, Ziptide.Visuals): `groundTint`, `skyGradient`, `planet` (baseColor, accentColor, angularSizeDegrees, distance, direction, rotationSpeed, followPlayer).
- **SkyPlanetRig** (Ziptide.Visuals): Runtime sky sphere (procedural gradient) + planet sphere (procedural texture); no colliders/shadows; planet can follow player.
- **WorldDirector** (Ziptide.Gameplay): Applies a VisualThemeProfile on Start (ground tint + SkyPlanetRig); creates SkyRig at runtime.
- **Editor menus:** **Ziptide > Create Default Visual Theme (Sky + Planet)** | **Ziptide > Apply Theme To Current Scene**.

---

## Verification checklist

1. **Unity opens, compiles with zero errors.**
2. **Play in Editor:** See sky (gradient), planet (in sky), ground (tinted), and grab cube. Cube still grabbable.
3. **Existing Milestone A scene:** Open `Assets/Ziptide/Scenes/MilestoneA_GrabCube.unity`, run **Ziptide > Apply Theme To Current Scene**, save. Then Play to confirm sky + planet + cube.
4. **Build and device:** From Cursor run:
   ```powershell
   powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
   ```
5. **On Quest:** Confirm sky + planet visible, horizon clear, grab cube still works (grab, drop, locomotion).

---

## Stop condition

If grab cube or locomotion breaks, revert the last change and fix before continuing.

---

*When all steps pass, update docs/STATUS.md to mark Milestone A.5 complete.*
