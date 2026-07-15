# ZIPTIDE R1.2 Exit Report — Fake Tracked-Rig Fixture

**Status:** PROMOTED  
**Unity:** 2022.3.62f3  
**Production runtime changes:** none

## Delivered

Tests-only PlayMode composition now creates and tears down:

- one inactive-while-building `RecoveryTestRig` root;
- camera-offset root and tracked-head Camera with configurable non-zero X/Z offset;
- one `XRInteractionManager`;
- one `InputActionManager` with a test-owned `InputActionAsset`;
- left/right controller transforms and `XRRayInteractor` components;
- controlled floor collider and deterministic spawn marker;
- transactional cleanup for constructor failures and inactive partial fixtures.

The fixture never starts OpenXR and does not require a headset.

## Failure found and corrected

The first R1.2 run failed because `InputActionManager` was added while its GameObject was active. Its enable lifecycle ran before the runtime-created action-asset list was initialized. The constructor then threw before the fixture field assignment completed, leaving a partial rig and causing the next test to see a duplicate XRI manager.

The correction builds the full composition beneath an inactive root, initializes every dependency, activates once at the end, wraps construction with named stages and destroys partial state immediately on failure. Teardown also removes inactive named test objects independently of fixture assignment.

## Promotion proof

### Baseline

- Tested SHA: `abe23adb11dbebd7f46af72b740d6df39e2b9dc3`
- Workflow run: `29378493064`
- Artifact: `8328816689`
- Result: 3 passed, 0 failed, 0 skipped

### Unrelated descendant

- Tested SHA: `e2d55ecb3e762ab6167b49713791aa1b44f93117`
- Workflow run: `29378733205`
- Artifact: `8328899835`
- Result: 3 passed, 0 failed, 0 skipped
- Change class: documentation-only repeat marker

## Passing contracts

1. The PlayMode lifecycle advances and cleans up.
2. Exactly one camera, XRI manager and input manager exist under the test rig.
3. Test-owned actions enable successfully.
4. Tracked-head local offset survives rig teleport.
5. Tracked-head world position differs from the rig root.
6. The right ray hits, hovers and selects an `XRSimpleInteractable` through the XRI manager.
7. No test rig, floor, spawn marker, target or enabled test action leaks after teardown.

## Next bounded layer

R1.3 adds the recovery exposure contract and proves every cataloged automatic owner has either a closed feature ID or an explicit `AlwaysRequired` classification. Existing bootstraps are not switched until the contract and catalog tests are green.
