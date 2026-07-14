# Recovery Static Workflow Status

Commit: `6e8824d0573110146b43c7c46146fa1e7cd72590`

## Tool tests

| Test file | Result |
|---|---|
| `test_recovery_contract_scan.py` | PASS |
| `test_recovery_inventory_gate.py` | PASS |
| `test_recovery_contract_map.py` | PASS |
| `test_recovery_scene_exposure.py` | PASS |
| `test_recovery_claim_audit.py` | PASS |
| `test_recovery_input_contract_scan.py` | PASS |
| `test_recovery_source_resolver.py` | PASS |
| `test_recovery_event_save_graph.py` | PASS |
| `test_recovery_focus_reference_graph.py` | **FAIL (1)** |

### test_recovery_focus_reference_graph.py failure tail

```text
test_duplicate_token_hits_on_one_line_are_deduplicated_per_token (test_recovery_focus_reference_graph.RecoveryFocusReferenceGraphTests.test_duplicate_token_hits_on_one_line_are_deduplicated_per_token) ... ok
test_marks_declaration_and_reference_lines (test_recovery_focus_reference_graph.RecoveryFocusReferenceGraphTests.test_marks_declaration_and_reference_lines) ... FAIL
test_repository_groups_three_focuses (test_recovery_focus_reference_graph.RecoveryFocusReferenceGraphTests.test_repository_groups_three_focuses) ... ok

======================================================================
FAIL: test_marks_declaration_and_reference_lines (test_recovery_focus_reference_graph.RecoveryFocusReferenceGraphTests.test_marks_declaration_and_reference_lines)
----------------------------------------------------------------------
Traceback (most recent call last):
  File "/home/runner/work/Ziptide/Ziptide/tools/tests/test_recovery_focus_reference_graph.py", line 21, in test_marks_declaration_and_reference_lines
    self.assertTrue(hammer[0].declaration_file)
AssertionError: False is not true

----------------------------------------------------------------------
Ran 3 tests in 0.002s

FAILED (failures=1)
```

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
| `recovery_focus_reference_graph` | PASS |
