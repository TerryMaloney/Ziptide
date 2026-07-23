from __future__ import annotations

import copy
import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import quest_lifecycle_gate

REPO_ROOT = TOOLS_DIR.parent
CONTRACT = REPO_ROOT / "docs/runtime_lifecycle/quest_system_focus_contract.json"


class QuestLifecycleGateTests(unittest.TestCase):
    """Mutation tests: the committed contract passes; every enforcement class bites."""

    def _mutated(self, mutate) -> list[str]:
        data = json.loads(CONTRACT.read_text(encoding="utf-8"))
        mutate(data)
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "contract.json"
            path.write_text(json.dumps(data), encoding="utf-8")
            return quest_lifecycle_gate.validate(path, REPO_ROOT)

    def test_committed_contract_is_valid(self) -> None:
        problems = quest_lifecycle_gate.validate(CONTRACT, REPO_ROOT)
        self.assertEqual([], problems)

    def test_missing_state_fails(self) -> None:
        problems = self._mutated(lambda d: d["states"].pop("TrackingLost"))
        self.assertTrue(any("state missing: TrackingLost" in p for p in problems))

    def test_unknown_state_fails(self) -> None:
        def mutate(d):
            d["states"]["Dreaming"] = copy.deepcopy(d["states"]["Active"])
        problems = self._mutated(mutate)
        self.assertTrue(any("unknown state: Dreaming" in p for p in problems))

    def test_missing_behavior_axis_fails(self) -> None:
        def mutate(d):
            del d["states"]["SystemOverlay"]["behaviors"]["haptics"]
        problems = self._mutated(mutate)
        self.assertTrue(any("SystemOverlay" in p and "haptics" in p for p in problems))

    def test_empty_axis_value_fails(self) -> None:
        def mutate(d):
            d["states"]["Resuming"]["behaviors"]["audio"] = "  "
        problems = self._mutated(mutate)
        self.assertTrue(any("Resuming" in p and "audio" in p for p in problems))

    def test_transition_to_invalid_state_fails(self) -> None:
        def mutate(d):
            d["transitions"][0]["to"] = "Limbo"
        problems = self._mutated(mutate)
        self.assertTrue(any("bad to-state: Limbo" in p for p in problems))

    def test_rotted_detection_source_fails(self) -> None:
        def mutate(d):
            d["transitions"][0]["detectionSource"] = "Ziptide/Assets/DoesNotExist.cs"
        problems = self._mutated(mutate)
        self.assertTrue(any("detectionSource missing on disk" in p for p in problems))

    def test_rotted_audited_seam_fails(self) -> None:
        def mutate(d):
            d["auditedSeams"]["save"] = "Ziptide/Assets/Gone/SaveSystem.cs"
        problems = self._mutated(mutate)
        self.assertTrue(any("auditedSeams.save" in p for p in problems))

    def test_rotted_haptic_site_in_list_fails(self) -> None:
        def mutate(d):
            d["auditedSeams"]["hapticSites"][0] = "Ziptide/Assets/Gone/Buzz.cs"
        problems = self._mutated(mutate)
        self.assertTrue(any("hapticSites" in p for p in problems))

    def test_implemented_by_ratchet_bites(self) -> None:
        def mutate(d):
            d["states"]["Active"]["implementedBy"] = "Ziptide/Assets/Gone/SystemFocusLifecycle.cs"
        problems = self._mutated(mutate)
        self.assertTrue(any("implementedBy path missing" in p for p in problems))

    def test_unresolved_slice_dependency_fails(self) -> None:
        def mutate(d):
            d["implementationSlices"][1]["dependsOn"] = ["D99"]
        problems = self._mutated(mutate)
        self.assertTrue(any("unresolved dependsOn: D99" in p for p in problems))

    def test_slice_without_evidence_fails(self) -> None:
        def mutate(d):
            d["implementationSlices"][0]["evidence"] = []
        problems = self._mutated(mutate)
        self.assertTrue(any("no acceptance evidence" in p for p in problems))

    def test_cli_exit_codes(self) -> None:
        self.assertEqual(0, quest_lifecycle_gate.main([
            "--contract", str(CONTRACT), "--repo-root", str(REPO_ROOT),
        ]))
        with tempfile.TemporaryDirectory() as tmp:
            bad = Path(tmp) / "bad.json"
            bad.write_text("{not json", encoding="utf-8")
            self.assertEqual(2, quest_lifecycle_gate.main([
                "--contract", str(bad), "--repo-root", str(REPO_ROOT),
            ]))


if __name__ == "__main__":
    unittest.main()
