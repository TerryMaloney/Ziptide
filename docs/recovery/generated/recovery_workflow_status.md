# Recovery Static Workflow Status

Commit: `af0edfc3af885ba54df5d20ac970192fbed30e5f`

## Tool tests

| Test file | Result |
|---|---|
| `test_recovery_contract_scan.py` | PASS |
| `test_recovery_inventory_gate.py` | PASS |
| `test_recovery_contract_map.py` | PASS |
| `test_recovery_scene_exposure.py` | PASS |
| `test_recovery_claim_audit.py` | PASS |
| `test_recovery_input_contract_scan.py` | **FAIL (1)** |

### test_recovery_input_contract_scan.py failure tail

```text
test_pairs_action_with_binding_and_finds_menu_chord (test_recovery_input_contract_scan.RecoveryInputContractScanTests.test_pairs_action_with_binding_and_finds_menu_chord) ... FAIL
test_repository_reports_cross_owner_collision (test_recovery_input_contract_scan.RecoveryInputContractScanTests.test_repository_reports_cross_owner_collision) ... ok
test_unresolved_field_binding_is_preserved (test_recovery_input_contract_scan.RecoveryInputContractScanTests.test_unresolved_field_binding_is_preserved) ... ok

======================================================================
FAIL: test_pairs_action_with_binding_and_finds_menu_chord (test_recovery_input_contract_scan.RecoveryInputContractScanTests.test_pairs_action_with_binding_and_finds_menu_chord)
----------------------------------------------------------------------
Traceback (most recent call last):
  File "/home/runner/work/Ziptide/Ziptide/tools/tests/test_recovery_input_contract_scan.py", line 36, in test_pairs_action_with_binding_and_finds_menu_chord
    self.assertEqual(1, len(chords))
AssertionError: 1 != 2

----------------------------------------------------------------------
Ran 3 tests in 0.002s

FAILED (failures=1)
```
| `test_recovery_source_resolver.py` | PASS |
| `test_recovery_event_save_graph.py` | PASS |

## Report generators

| Generator | Result |
|---|---|
| `recovery_contract_scan` | PASS |
| `recovery_inventory_validation` | PASS |
| `recovery_contract_map` | PASS |
| `recovery_scene_exposure` | PASS |
| `recovery_claim_audit` | PASS |
| `recovery_input_contract_scan` | PASS |
| `recovery_source_resolver` | PASS |
| `recovery_event_save_graph` | PASS |
