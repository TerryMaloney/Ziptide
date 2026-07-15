# Recovery PlayMode Repeat Probe

This documentation-only marker verifies that the current PlayMode recovery suite remains green on an unrelated descendant commit.

It changes no Unity source, scene, asset, package, project setting, or test implementation.

## R1.3 locked Golden candidate repeat

The suite now covers:

- one-frame lifecycle execution;
- the tests-only tracked rig and XRI interaction;
- the automatic-owner exposure contract;
- repeat-green Core and Gameplay automatic-owner gates;
- explicit `FullDevelopment` versus `GoldenSlice` profile resolution;
- per-build `ZIPTIDE_RECOVERY_GOLDEN` selection without project-symbol mutation;
- Golden-default Quest scripts with an explicit FullDevelopment override;
- Ecology excluded until creature contact/grounding proof;
- a recovery APK scene list locked to `_Boot`, `W000_DriftIn`, and `ToxicCity` only.

This marker SHA must produce:

- 18 passing tests with zero failures or skips;
- a successful exact-SHA PlayMode observation;
- a test-result artifact;
- a durable observation for this exact SHA.
