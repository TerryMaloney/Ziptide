from pathlib import Path

path = Path("Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs")
text = path.read_text(encoding="utf-8")

method_start = text.index("        private IEnumerator RestoreReadersAfterInputSettle()")
method_end = text.index("        /// <summary>True when every action", method_start)
new_method = '''        private IEnumerator RestoreReadersAfterInputSettle()
        {
            // One owner spans chained travel. A second trip may begin after the first TRAVEL_OK but before
            // the previous two-frame settle tail closes; that is fresh input churn, not permanent corruption.
            // Travel or one unsafe consumer-equivalent read resets clean progress. Restore only after two
            // consecutive safe frames while travel is inactive. The overall deadline remains bounded and
            // failure remains closed: providers are never re-enabled after an unproven state.
            yield return null;
            float deadline = Time.realtimeSinceStartup + 45f;
            int cleanFrames = 0;
            Ziptide.Core.InputMutationSettleDecision lastDecision =
                Ziptide.Core.InputMutationSettleDecision.WaitForTravel;

            while (Time.realtimeSinceStartup < deadline)
            {
                bool travelling = TravelCoordinator.IsTravelling;
                bool actionsSafe = !travelling && SuspendedReaderActionsReadSafely();
                Ziptide.Core.InputMutationSettleStep step =
                    Ziptide.Core.InputMutationSettleCore.Advance(
                        travelling, actionsSafe, cleanFrames);
                cleanFrames = step.NextCleanFrames;
                lastDecision = step.Decision;

                if (step.Decision == Ziptide.Core.InputMutationSettleDecision.Restore)
                {
                    bool handedToBootHold = _bootHold.Held;
                    foreach (var b in _mutationSuspendedReaders)
                    {
                        if (b == null) continue;
                        if (handedToBootHold) _bootSuspended.Add(b);
                        else b.enabled = true;
                    }
                    Debug.Log("ZIPTIDE: INPUT_MUTATION_READERS restored="
                        + _mutationSuspendedReaders.Count
                        + " cleanFrames=" + cleanFrames
                        + (handedToBootHold ? " handedTo=bootHold" : ""));
                    _mutationSuspendedReaders.Clear();
                    _mutationReaderRestore = null;
                    yield break;
                }

                yield return null;
            }

            Debug.LogError("ZIPTIDE: INPUT_MUTATION_SETTLE_FAIL readers="
                + _mutationSuspendedReaders.Count
                + " decision=" + lastDecision
                + " travelling=" + TravelCoordinator.IsTravelling
                + " cleanFrames=" + cleanFrames
                + " detail=" + (string.IsNullOrEmpty(_lastInputSettleFailure)
                    ? "<no failing action captured>" : _lastInputSettleFailure));
            _mutationReaderRestore = null;
        }

'''
text = text[:method_start] + new_method + text[method_end:]

action_start = text.index("        private static bool ActionReadsSafely(InputActionProperty property,")
action_end = text.index("        private static string ActionPath(InputAction action)", action_start)
new_action = '''        private static bool ActionReadsSafely(InputActionProperty property,
            string owner, out string failure)
        {
            failure = string.Empty;
            var action = property.action;
            if (action == null) return true;
            try
            {
                if (!action.enabled)
                {
                    // XRI action-based locomotion providers own only DIRECT actions. A disabled reference
                    // is externally managed and reads its default safely. Prepare only direct actions while
                    // the provider is suspended, then require a later-frame consumer-equivalent read.
                    if (property.reference != null) return true;
                    action.Enable();
                    failure = owner + " action=" + ActionPath(action)
                        + " phase=direct_action_enabled";
                    return false;
                }

                // These exact providers consume Vector2. The safety probe must match the consumer contract;
                // a different value type is a real miswire and must stay fail-closed.
                action.ReadValue<Vector2>();
                return true;
            }
            catch (System.Exception ex)
            {
                string runtimeType;
                try { runtimeType = action.valueType != null ? action.valueType.FullName : string.Empty; }
                catch { runtimeType = "<unavailable>"; }
                failure = owner + " action=" + ActionPath(action)
                    + " expected=" + (action.expectedControlType ?? string.Empty)
                    + " runtime=" + runtimeType
                    + " enabled=" + action.enabled
                    + " reference=" + (property.reference != null)
                    + " exception=" + ex.GetType().Name + ":" + ex.Message;
                return false;
            }
        }

'''
text = text[:action_start] + new_action + text[action_end:]

if "InputActionReadKindCore" in text or "InputActionReadKind." in text:
    raise SystemExit("Obsolete value-kind resolver reference remains in runtime source")

path.write_text(text, encoding="utf-8")
print(f"Patched {path}")
