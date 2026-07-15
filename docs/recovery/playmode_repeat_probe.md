# Recovery PlayMode Repeat Probe

This documentation-only marker verifies that the current PlayMode recovery suite remains green on an unrelated descendant commit.

It changes no Unity source, scene, asset, package, project setting, or test implementation.

## R1.3 Gameplay gate repeat

The suite now covers:

- one-frame lifecycle execution;
- the tests-only tracked rig and XRI interaction;
- the automatic-owner exposure contract;
- Core automatic-owner gates;
- exact gate wiring for nine Gameplay owners;
- GoldenSlice policy where developer and PvP surfaces remain absent while Ambience and First-Hour observation remain present.

This marker SHA must produce:

- 12 passing tests with zero failures or skips;
- a successful exact-SHA PlayMode observation;
- a test-result artifact;
- a durable observation for this exact SHA.
