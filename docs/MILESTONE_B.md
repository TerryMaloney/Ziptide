# Milestone B — Play-Area Safety + WorldProfile + Theme Switch Station

**Status:** Implemented. Verify on device before marking complete.

---

## What was added

- **WorldProfile** (ScriptableObject, Ziptide.Content): spawn position/rotation, play area size, ground Y, respawn on fall (threshold, fade seconds), defaultTheme, availableThemes list.
- **WorldRuntime** (Ziptide.Gameplay): Applies WorldProfile on Start (WorldDirector, ground scale, PlayAreaBounds, ThemeSwitchStation); `ApplyTheme(theme)`, `RespawnPlayer(rig)`.
- **PlayAreaBounds** (Ziptide.Gameplay): Four invisible BoxCollider walls around play area (Ignore Raycast layer).
- **FallRespawner** (Ziptide.Gameplay): On XR Origin; when Y &lt; fallYThreshold calls WorldRuntime.RespawnPlayer.
- **ThemeSwitchStation** (Ziptide.Gameplay): Spawns one XRSimpleInteractable button per available theme; ray-select applies that theme.
- **Editor menus:** **Ziptide > Create Default World Profile** | **Ziptide > Apply World Profile To Current Scene**.

---

## Wiring the Milestone A scene

1. Open `Assets/Ziptide/Scenes/MilestoneA_GrabCube.unity`.
2. Run **Ziptide > Apply World Profile To Current Scene** (creates Default World Profile if missing).
3. Save the scene.

---

## Verification checklist

1. **Unity opens, compiles with zero errors.**
2. **Play in Editor:** WorldRuntime applies default theme; boundary walls exist; walk into wall (collision). Intentionally move XR Origin below fallYThreshold (e.g. -2) to trigger respawn. Use Theme Switch Station (ray at a button, select) and confirm sky/planet/ground change. Grab cube still works.
3. **Build and device:** From Cursor run:
   ```powershell
   powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
   ```
4. **On Quest:** Walk into wall (collides); fall off platform and respawn at spawn; use Theme Switch Station to swap theme; grab cube still works.

---

## Stop condition

If grab cube or locomotion breaks, revert the last change and fix before continuing.

---

*When all steps pass, update docs/STATUS.md to mark Milestone B complete.*
