from pathlib import Path


def replace_once(path, old, new, label):
    p = Path(path)
    text = p.read_text(encoding="utf-8")
    count = text.count(old)
    print(f"{label}: matches={count}")
    if count != 1:
        raise SystemExit(f"{label}: expected one exact current-source match, found {count}")
    p.write_text(text.replace(old, new), encoding="utf-8")


rig = "Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs"
guard = "Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerInputSessionGuard.cs"
test = "Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs"

replace_once(rig,
'''                        if (action.name == "Rotate Anchor" || action.name == "Translate Anchor")
                        {
                            action.Disable();
                            disabled++;
                        }
''',
'''                        if (action.name == "Rotate Anchor" || action.name == "Translate Anchor")
                        {
                            if (!action.enabled) continue;
                            action.Disable();
                            disabled++;
                        }
''',
"anchor disables only on change")

replace_once(rig,
'''            // Keep the assets enabled (idempotent). Restores input if a prior unload disabled them.
            foreach (var a in assets)
                a.Enable();
''',
'''            // Asset.Enable() is not idempotent for a partially enabled asset: it re-enables every
            // intentionally disabled action and rebuilds shared InputActionState. Recover only an
            // asset that is completely disabled; preserve a live asset's exact per-action state.
            int recoveredAssets = 0;
            foreach (var a in assets)
            {
                if (a == null || a.enabled) continue;
                a.Enable();
                recoveredAssets++;
            }
            if (recoveredAssets > 0)
                Debug.Log("ZIPTIDE: INPUT_ASSET_RECOVERED count=" + recoveredAssets);
''',
"persistent rig preserves live asset state")

replace_once(rig,
'''            if (!SuspendedReaderActionsReadSafely())
                Debug.LogWarning("ZIPTIDE: INPUT_MUTATION_SETTLE_TIMEOUT — re-enabling readers anyway");
            yield return null;
            yield return null; // two settled frames beyond the last clean probe
''',
'''            if (!SuspendedReaderActionsReadSafely())
            {
                // Fail closed. Re-enabling a reader with a known-unsafe InputActionState recreates the
                // proven Quest crash candidate; disabled locomotion plus a blocking error is safer.
                Debug.LogError("ZIPTIDE: INPUT_MUTATION_SETTLE_FAIL readers="
                    + _mutationSuspendedReaders.Count);
                _mutationReaderRestore = null;
                yield break;
            }
            yield return null;
            yield return null; // two settled frames beyond the last clean probe
''',
"input settle fails closed")

replace_once(rig,
'''            var action = property.action;
            if (action == null || !action.enabled) return true; // nothing to settle
            try
            {
                action.ReadValue<Vector2>();
''',
'''            var action = property.action;
            if (action == null) return true;
            try
            {
                if (!action.enabled)
                {
                    // A provider may own a direct action. Prepare it while the provider is still
                    // suspended, then force the settle loop to observe it on a later frame.
                    if (property.reference != null) return false;
                    action.Enable();
                    return false;
                }
                action.ReadValue<Vector2>();
''',
"direct actions prepare before provider wake")

replace_once(guard,
'''            primary.actionAssets = assets;
            primary.enabled = true;
            for (int i = 0; i < assets.Count; i++)
                if (assets[i] != null) assets[i].Enable();

            int disabled = 0;
''',
'''            var fullyDisabledAssets = new List<InputActionAsset>();
            var intentionallyDisabledActions = new List<InputAction>();
            for (int i = 0; i < assets.Count; i++)
            {
                InputActionAsset asset = assets[i];
                if (asset == null) continue;
                if (!asset.enabled)
                {
                    fullyDisabledAssets.Add(asset);
                    continue;
                }
                foreach (InputActionMap map in asset.actionMaps)
                    foreach (InputAction action in map.actions)
                        if (action != null && !action.enabled)
                            intentionallyDisabledActions.Add(action);
            }

            primary.actionAssets = assets;
            primary.enabled = true;

            // InputActionManager.OnEnable may enable every assigned asset. Restore the exact disabled
            // actions from assets that were already live before manager activation.
            for (int i = 0; i < intentionallyDisabledActions.Count; i++)
            {
                InputAction action = intentionallyDisabledActions[i];
                if (action != null && action.enabled) action.Disable();
            }

            int recoveredAssets = 0;
            for (int i = 0; i < fullyDisabledAssets.Count; i++)
            {
                InputActionAsset asset = fullyDisabledAssets[i];
                if (asset == null) continue;
                if (!asset.enabled) asset.Enable();
                recoveredAssets++;
            }

            int disabled = 0;
''',
"input guard preserves per-action state")

replace_once(guard,
'''            Debug.Log("ZIPTIDE: INPUT_SESSION_CANONICAL manager=" + HierarchyPath(primary.transform)
                + " assets=" + assets.Count + " duplicatesDisabled=" + disabled
                + " reason=" + reason);
''',
'''            Debug.Log("ZIPTIDE: INPUT_SESSION_CANONICAL manager=" + HierarchyPath(primary.transform)
                + " assets=" + assets.Count + " duplicatesDisabled=" + disabled
                + " recoveredAssets=" + recoveredAssets + " reason=" + reason);
''',
"input guard reports recovery")

replace_once(test,
'''            InputActionAsset primaryAsset = CreateAsset("PrimaryAsset", "PrimaryAction");
            primary.actionAssets = new List<InputActionAsset> { primaryAsset };
            primaryAsset.Enable();
''',
'''            InputActionAsset primaryAsset = CreateAsset("PrimaryAsset", "PrimaryAction");
            InputAction primaryHeldOff = primaryAsset.FindActionMap("TestMap")
                .AddAction("Rotate Anchor", InputActionType.Value);
            primary.actionAssets = new List<InputActionAsset> { primaryAsset };
            primaryAsset.Enable();
            primaryHeldOff.Disable();
''',
"canary primary action state")

replace_once(test,
'''            InputActionAsset duplicateAsset = CreateAsset("SceneAsset", "SceneAction");
            duplicate.actionAssets = new List<InputActionAsset> { duplicateAsset };
            duplicateAsset.Enable();
''',
'''            InputActionAsset duplicateAsset = CreateAsset("SceneAsset", "SceneAction");
            InputAction duplicateHeldOff = duplicateAsset.FindActionMap("TestMap")
                .AddAction("Translate Anchor", InputActionType.Value);
            duplicate.actionAssets = new List<InputActionAsset> { duplicateAsset };
            duplicateAsset.Enable();
            duplicateHeldOff.Disable();
''',
"canary transferred action state")

replace_once(test,
'''            Assert.IsTrue(primaryAsset.enabled, "Primary action asset was not enabled.");
            Assert.IsTrue(duplicateAsset.enabled, "Transferred scene action asset was not enabled.");

            int disabledAgain = PlayerInputSessionGuard.Consolidate("controlled_test_repeat");
''',
'''            Assert.IsTrue(primaryAsset.enabled, "Primary action asset was not enabled.");
            Assert.IsTrue(duplicateAsset.enabled, "Transferred scene action asset was not enabled.");
            Assert.IsFalse(primaryHeldOff.enabled,
                "Consolidation re-enabled a primary action that production intentionally disabled.");
            Assert.IsFalse(duplicateHeldOff.enabled,
                "Consolidation re-enabled a transferred action that production intentionally disabled.");

            int disabledAgain = PlayerInputSessionGuard.Consolidate("controlled_test_repeat");
''',
"canary first pass assertions")

replace_once(test,
'''            Assert.IsFalse(duplicate.enabled);
            Assert.AreEqual(0, duplicate.actionAssets.Count);

            RecoveryRuntimeCensusSnapshot census = RecoveryRuntimeCensus.Capture(
''',
'''            Assert.IsFalse(duplicate.enabled);
            Assert.AreEqual(0, duplicate.actionAssets.Count);
            Assert.IsFalse(primaryHeldOff.enabled,
                "Repeated consolidation changed primary per-action state.");
            Assert.IsFalse(duplicateHeldOff.enabled,
                "Repeated consolidation changed transferred per-action state.");

            RecoveryRuntimeCensusSnapshot census = RecoveryRuntimeCensus.Capture(
''',
"canary repeated pass assertions")
