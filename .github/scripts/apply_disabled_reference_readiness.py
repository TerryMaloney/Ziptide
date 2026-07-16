from pathlib import Path


def replace_once(path, old, new, label):
    p = Path(path)
    text = p.read_text(encoding="utf-8")
    count = text.count(old)
    print(f"{label}: matches={count}")
    if count != 1:
        raise SystemExit(f"{label}: expected one exact match, found {count}")
    p.write_text(text.replace(old, new), encoding="utf-8")


rig = "Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs"
test = "Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryInputSessionGuardTests.cs"

replace_once(
    rig,
    '''                if (!action.enabled)
                {
                    // A provider may own a direct action. Prepare it while the provider is still
                    // suspended, then force the settle loop to observe it on a later frame.
                    if (property.reference != null) return false;
                    action.Enable();
                    return false;
                }
''',
    '''                if (!action.enabled)
                {
                    // XRI action-based behaviours own only DIRECT actions. A disabled reference is
                    // externally managed, remains safe to read as its default value, and must not make
                    // the settle predicate impossible. Prepare only direct actions while the provider
                    // is suspended, then force a later-frame read before waking the provider.
                    if (property.reference != null) return true;
                    action.Enable();
                    return false;
                }
''',
    "disabled reference is stable; direct action is prepared",
)

replace_once(
    test,
    "using System.Collections.Generic;\n",
    "using System.Collections.Generic;\nusing System.Reflection;\n",
    "reflection namespace",
)

replace_once(
    test,
    '''            Assert.IsFalse(duplicateHeldOff.enabled,
                "Repeated consolidation changed transferred per-action state.");

            RecoveryRuntimeCensusSnapshot census = RecoveryRuntimeCensus.Capture(
''',
    '''            Assert.IsFalse(duplicateHeldOff.enabled,
                "Repeated consolidation changed transferred per-action state.");

            MethodInfo readiness = typeof(PlayerRigPersistence).GetMethod(
                "ActionReadsSafely",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(readiness,
                "The input-mutation readiness seam is missing from PlayerRigPersistence.");

            InputAction referencedAction = primaryAsset.FindAction("PrimaryAction");
            referencedAction.Disable();
            InputActionReference referencedActionRef = InputActionReference.Create(referencedAction);
            var referencedProperty = new InputActionProperty(referencedActionRef);
            try
            {
                Assert.IsTrue((bool)readiness.Invoke(null, new object[] { referencedProperty }),
                    "A disabled externally managed reference was incorrectly treated as unstable.");
            }
            finally
            {
                if (referencedActionRef != null) Object.DestroyImmediate(referencedActionRef);
            }

            var directAction = new InputAction(
                "ControlledDirectTurn",
                InputActionType.Value,
                expectedControlType: "Vector2");
            var directProperty = new InputActionProperty(directAction);
            try
            {
                Assert.IsFalse((bool)readiness.Invoke(null, new object[] { directProperty }),
                    "A disabled direct action was accepted without being prepared first.");
                Assert.IsTrue(directAction.enabled,
                    "The provider-owned direct action was not enabled while its reader was suspended.");
                Assert.IsTrue((bool)readiness.Invoke(null, new object[] { directProperty }),
                    "The prepared direct action did not become readable on the following probe.");
            }
            finally
            {
                directAction.Dispose();
            }

            RecoveryRuntimeCensusSnapshot census = RecoveryRuntimeCensus.Capture(
''',
    "existing canary proves reference/direct readiness ownership",
)
