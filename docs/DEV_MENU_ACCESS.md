# Controller-Free Developer Menu Access

The world-warp DevMenu must stay available during headset testing without consuming gameplay buttons.

## Open it from inside the headset

In a **development build**:

1. Raise both controllers just above your forehead.
2. Hold them close together for about two seconds.
3. The DevMenu toggles open.

Release the pose before doing it again. Repeating the gesture toggles the menu closed or open.

The gesture reads controller positions only. It does not use Y, B, sticks, triggers, grips, or menu buttons.

## Other access paths

- **Unity Editor:** press `F2`.
- **Computer-side backup:**
  ```powershell
  .\tools\dev_menu_access.ps1 -Action Open
  ```
- **Shipping build:** DevMenu access code is compiled out.

ADB is optional. It is no longer required for normal headset testing.

## Preserved gameplay controls

- B: QuickSwap
- L3: sprint/auto-run
- R3: crouch/slide
- Y: reserved for the future player menu

## Headset verification

1. Press Y and B separately and together: nothing should happen to DevMenu.
2. Hold both controllers above your forehead for two seconds: DevMenu should open.
3. Keep holding: it must not repeatedly toggle.
4. Release, repeat the gesture: it should toggle again.
5. Warp to another world and repeat: the menu must still work and remain clickable.
6. Verify a non-development build has no DevMenu behavior.

The ADB helper remains available for remote testing and recovery, but it is not the primary access method.
