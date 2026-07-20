from pathlib import Path

path = Path("Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs")
text = path.read_text(encoding="utf-8")

old_fields = """        private readonly System.Collections.Generic.List<Behaviour> _mutationSuspendedReaders =
            new System.Collections.Generic.List<Behaviour>();
        private Coroutine _mutationReaderRestore;
"""
new_fields = """        private readonly System.Collections.Generic.List<Behaviour> _mutationSuspendedReaders =
            new System.Collections.Generic.List<Behaviour>();
        private Coroutine _mutationReaderRestore;
        private string _lastInputSettleFailure = string.Empty;
"""

old_error = """                Debug.LogError(\"ZIPTIDE: INPUT_MUTATION_SETTLE_FAIL readers=\"
                    + _mutationSuspendedReaders.Count);
"""
new_error = """                Debug.LogError(\"ZIPTIDE: INPUT_MUTATION_SETTLE_FAIL readers=\"
                    + _mutationSuspendedReaders.Count + \" detail=\"
                    + (string.IsNullOrEmpty(_lastInputSettleFailure)
                        ? \"<no failing action captured>\" : _lastInputSettleFailure));
"""

old_methods = """        /// <summary>True when every action the suspended readers poll reads without throwing —
        /// i.e. the InputActionState re-resolution triggered by the wiring mutation has completed.</summary>
        private bool SuspendedReaderActionsReadSafely()
        {
            foreach (var b in _mutationSuspendedReaders)
            {
                switch (b)
                {
                    case ActionBasedContinuousMoveProvider move:
                        if (!ActionReadsSafely(move.leftHandMoveAction) ||
                            !ActionReadsSafely(move.rightHandMoveAction)) return false;
                        break;
                    case ActionBasedContinuousTurnProvider turn:
                        if (!ActionReadsSafely(turn.leftHandTurnAction) ||
                            !ActionReadsSafely(turn.rightHandTurnAction)) return false;
                        break;
                    case ActionBasedSnapTurnProvider snap:
                        if (!ActionReadsSafely(snap.leftHandSnapTurnAction) ||
                            !ActionReadsSafely(snap.rightHandSnapTurnAction)) return false;
                        break;
                }
            }
            return true;
        }

        private static bool ActionReadsSafely(InputActionProperty property)
        {
            var action = property.action;
            if (action == null) return true;
            try
            {
                if (!action.enabled)
                {
                    // XRI action-based behaviours own only DIRECT actions. A disabled reference is
                    // externally managed, remains safe to read as its default value, and must not make
                    // the settle predicate impossible. Prepare only direct actions while the provider
                    // is suspended, then force a later-frame read before waking the provider.
                    if (property.reference != null) return true;
                    action.Enable();
                    return false;
                }
                action.ReadValue<Vector2>();
                return true;
            }
            catch (System.NullReferenceException)
            {
                return false; // InputActionState still mid-re-resolve — the exact NRE the readers would hit
            }
            catch (System.InvalidOperationException)
            {
                return false; // value-type mismatch during re-resolve — equally unsafe to poll
            }
        }
"""

new_methods = """        /// <summary>True when every action the suspended readers poll reads without throwing —
        /// i.e. the InputActionState re-resolution triggered by the wiring mutation has completed.</summary>
        private bool SuspendedReaderActionsReadSafely()
        {
            _lastInputSettleFailure = string.Empty;
            foreach (var b in _mutationSuspendedReaders)
            {
                switch (b)
                {
                    case ActionBasedContinuousMoveProvider move:
                        if (!ActionReadsSafely(move.leftHandMoveAction,
                                \"ContinuousMove.leftHandMoveAction\", out _lastInputSettleFailure) ||
                            !ActionReadsSafely(move.rightHandMoveAction,
                                \"ContinuousMove.rightHandMoveAction\", out _lastInputSettleFailure)) return false;
                        break;
                    case ActionBasedContinuousTurnProvider turn:
                        if (!ActionReadsSafely(turn.leftHandTurnAction,
                                \"ContinuousTurn.leftHandTurnAction\", out _lastInputSettleFailure) ||
                            !ActionReadsSafely(turn.rightHandTurnAction,
                                \"ContinuousTurn.rightHandTurnAction\", out _lastInputSettleFailure)) return false;
                        break;
                    case ActionBasedSnapTurnProvider snap:
                        if (!ActionReadsSafely(snap.leftHandSnapTurnAction,
                                \"SnapTurn.leftHandSnapTurnAction\", out _lastInputSettleFailure) ||
                            !ActionReadsSafely(snap.rightHandSnapTurnAction,
                                \"SnapTurn.rightHandSnapTurnAction\", out _lastInputSettleFailure)) return false;
                        break;
                }
            }
            return true;
        }

        private static bool ActionReadsSafely(InputActionProperty property,
            string owner, out string failure)
        {
            failure = string.Empty;
            var action = property.action;
            if (action == null) return true;
            try
            {
                if (!action.enabled)
                {
                    // XRI action-based behaviours own only DIRECT actions. A disabled reference is
                    // externally managed, remains safe to read as its default value, and must not make
                    // the settle predicate impossible. Prepare only direct actions while the provider
                    // is suspended, then force a later-frame read before waking the provider.
                    if (property.reference != null) return true;
                    action.Enable();
                    failure = owner + \" action=\" + ActionPath(action)
                        + \" phase=direct_action_enabled\";
                    return false;
                }

                string runtimeType = action.valueType != null ? action.valueType.FullName : string.Empty;
                Ziptide.Core.InputActionReadKind kind = Ziptide.Core.InputActionReadKindCore.Resolve(
                    action.expectedControlType, runtimeType);
                switch (kind)
                {
                    case Ziptide.Core.InputActionReadKind.Scalar:
                        action.ReadValue<float>();
                        break;
                    case Ziptide.Core.InputActionReadKind.Vector2:
                        action.ReadValue<Vector2>();
                        break;
                    default:
                        action.ReadValueAsObject();
                        break;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                string runtimeType;
                try { runtimeType = action.valueType != null ? action.valueType.FullName : string.Empty; }
                catch { runtimeType = \"<unavailable>\"; }
                failure = owner + \" action=\" + ActionPath(action)
                    + \" expected=\" + (action.expectedControlType ?? string.Empty)
                    + \" runtime=\" + runtimeType
                    + \" enabled=\" + action.enabled
                    + \" reference=\" + (property.reference != null)
                    + \" exception=\" + ex.GetType().Name + \":\" + ex.Message;
                return false;
            }
        }

        private static string ActionPath(InputAction action)
        {
            if (action == null) return \"<null>\";
            string map = action.actionMap != null ? action.actionMap.name : \"<direct>\";
            return map + \"/\" + action.name;
        }
"""

for label, old, new in (
    ("fields", old_fields, new_fields),
    ("error", old_error, new_error),
    ("methods", old_methods, new_methods),
):
    count = text.count(old)
    if count != 1:
        raise SystemExit(f"Expected exactly one {label} block, found {count}")
    text = text.replace(old, new, 1)

path.write_text(text, encoding="utf-8")
print(f"Patched {path}")
