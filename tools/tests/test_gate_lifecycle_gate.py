from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import gate_lifecycle_gate


class GateLifecycleGateTests(unittest.TestCase):
    def setUp(self) -> None:
        root = Path(__file__).resolve().parents[2]
        source = root / "docs/project_art_plan/gate_lifecycle_catalog.json"
        self.catalog = json.loads(source.read_text(encoding="utf-8"))
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "catalog.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = gate_lifecycle_gate.validate_catalog(self.path)
        return {finding.code for finding in result.findings}

    def _state(self, state_id: str) -> dict:
        return next(state for state in self.catalog["states"] if state["id"] == state_id)

    def test_repository_catalog_passes(self) -> None:
        result = gate_lifecycle_gate.validate_catalog(self.path)
        self.assertEqual((), result.findings)
        self.assertEqual(7, result.state_count)
        self.assertEqual(9, result.transition_count)

    def test_camera_and_travel_ownership_cannot_drift(self) -> None:
        self.catalog["cameraOwner"] = "gate-effect"
        self.catalog["transitionOwner"] = "visual-script"
        self._write()
        codes = self._codes()
        self.assertIn("CAMERA_OWNER_INVALID", codes)
        self.assertIn("TRAVEL_OWNER_INVALID", codes)

    def test_normal_state_cannot_exceed_transparency_rail(self) -> None:
        self._state("open")["transparentCoverageMax"] = 0.6
        self._write()
        self.assertIn("TRANSPARENCY_CAP_EXCEEDED", self._codes())

    def test_traversal_full_view_requires_travel_shell(self) -> None:
        self._state("traversal")["layers"].remove("travel_shell")
        self._write()
        self.assertIn("TRAVEL_SHELL_REQUIRED", self._codes())

    def test_particle_cap_is_enforced(self) -> None:
        self._state("entrainment_surge")["particleBudget"] = 65
        self._write()
        self.assertIn("PARTICLE_CAP_EXCEEDED", self._codes())

    def test_collapse_cannot_become_instant(self) -> None:
        self._state("collapse")["durationSeconds"]["min"] = 0.2
        self._write()
        self.assertIn("COLLAPSE_TOO_FAST", self._codes())

    def test_exit_must_wait_for_floor_horizon_and_input(self) -> None:
        self._state("exit_reform")["exitCondition"] = "scene-loaded"
        self._write()
        self.assertIn("EXIT_STABILITY_REQUIRED", self._codes())

    def test_camera_motion_and_strobe_rails_are_locked(self) -> None:
        self.catalog["globalRails"]["cameraMotionAllowed"] = True
        self.catalog["globalRails"]["fullFieldStrobeAllowed"] = True
        self._write()
        codes = self._codes()
        self.assertIn("GLOBAL_RAIL_DRIFT", codes)

    def test_unreviewed_transition_requires_amendment(self) -> None:
        self.catalog["allowedTransitions"].append(["dormant", "open"])
        self._write()
        self.assertIn("UNREVIEWED_TRANSITIONS_PRESENT", self._codes())

    def test_required_log_matches_state_id(self) -> None:
        self._state("waking")["requiredLog"] = "ZIPTIDE: WRONG"
        self._write()
        self.assertIn("LOG_CONTRACT_INVALID", self._codes())


if __name__ == "__main__":
    unittest.main()
