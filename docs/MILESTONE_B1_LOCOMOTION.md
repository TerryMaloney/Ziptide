# Milestone B.1 — Thumbstick Locomotion + Bounds Contract

**Status:** Implemented. Verify on device before marking complete.

---

## Definition of done

On Quest:

- **Left thumbstick** = continuous locomotion (move).
- **Right thumbstick** = snap turn (e.g. 45°).
- **PlayAreaBounds** block **stick** locomotion (walking into a wall with stick movement stops you).
- **Grab cube** still works.

**Clarification:** Bounds block stick-driven movement only. Physical roomscale walking can still clip the camera through walls; that is expected unless camera collision/fade is added later.

---

## What was added

- **EnsureLocomotionRig** (Editor): Menu **Ziptide > Ensure Thumbstick Locomotion Rig**. Finds XR Origin, ensures LocomotionSystem, ActionBasedContinuousMoveProvider, ActionBasedSnapTurnProvider, CharacterController, CharacterControllerDriver, InputActionManager, and wires XRI Default Input Actions (Move, Snap Turn). Move speed ~1.75, snap turn 45°.
- **PlayAreaBounds** (Gameplay): Bounds walls use **Default** layer so CharacterController collides; stick locomotion is blocked at walls.
- **VerifyLocomotionStatus** (Editor): Menu **Ziptide > Print Locomotion Debug**. Logs XR Origin, InputActionManager, Move/Turn providers, CharacterController.

---

## Verification steps

1. **Unity:** Open project, compile with zero errors.
2. **Starter Assets:** If not already imported: Package Manager → XR Interaction Toolkit → Samples → **Import "Starter Assets"**.
3. **Scene:** Open `Assets/Ziptide/Scenes/MilestoneA_GrabCube.unity`. Run **Ziptide > Apply World Profile To Current Scene** if needed.
4. **Rig:** Run **Ziptide > Ensure Thumbstick Locomotion Rig**.
5. **Check:** Run **Ziptide > Print Locomotion Debug**. Confirm all items pass (XR Origin found, InputActionManager + XRI assigned, Move/Turn providers present and actions assigned, CharacterController present).
6. **Build:** Close Unity. From Cursor: `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1`
7. **On Quest:** Left stick moves, right stick turns; walk into wall with stick — movement is blocked; grab cube still works.

---

## Stop condition

If grab cube or existing locomotion breaks, revert the last change and fix before continuing. Do not proceed to portals until B.1 passes on device.

---

*When all steps pass, update docs/STATUS.md to mark Milestone B.1 complete.*
