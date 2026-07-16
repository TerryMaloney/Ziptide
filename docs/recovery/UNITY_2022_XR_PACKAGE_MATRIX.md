# Unity 2022 XR package matrix — bounded recovery decision

**Decision date:** 2026-07-16  
**Scope:** package-only compatibility probe for the residual travel-time `InputActionState.ApplyProcessors` failure.  
**Assertion surface:** unchanged 43-test Recovery PlayMode suite.

## Evidence

The project uses Unity `2022.3.62f3`, but its direct package matrix was:

- `com.unity.inputsystem` `1.7.0`;
- `com.unity.xr.interaction.toolkit` `2.5.4`.

The committed lock proves XRI `2.5.4` directly requires Input System `1.7.0`. Unity documents those package generations for Unity 2023.x, while the 2022.3 line is Input System 1.6 and XRI 2.4.

After multiple bounded runtime ownership/window corrections, the canonical route either reproduced the package NRE or correctly failed closed because the affected actions did not become safely readable. Recovery rb36 therefore escalated the remaining failure to the package layer.

## Bounded matrix

- Input System: `1.7.0` → `1.6.3`
- XR Interaction Toolkit: `2.5.4` → `2.4.3`

`1.6.3` and `2.4.3` are the latest patch releases in the Unity-2022-compatible 1.6/2.4 lines exposed by Unity's package documentation. The two packages move together because XRI 2.5 requires Input 1.7.

OpenXR and XR Plug-in Management are deliberately unchanged in this packet. They already compile/build the locked Golden APK, and changing providers/configuration simultaneously would make the result ambiguous.

## Lock-file policy

The stale `packages-lock.json` is removed in this probe so Unity Package Manager resolves a truthful lock from the new direct manifest. Before promotion, the resolved lock must be captured and committed, followed by one final exact-source proof run.

## Required result

1. project resolves/imports under Unity 2022.3.62f3;
2. ordinary EditMode and patch/world audit green;
3. unchanged PlayMode suite executes 43 tests with 43 passed;
4. Golden Android succeeds on the same exact source;
5. resolved package lock is committed and the same gates repeat;
6. only then may the Quest checkpoint be authorized.

If the compatible matrix does not compile or the same route remains red, revert the package packet atomically. Do not weaken the route or stack another speculative runtime timing window.
