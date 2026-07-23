from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import rill_visual_state_gate


class RillVisualStateGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.root = Path(__file__).resolve().parents[2]
        self.catalog = json.loads(
            (self.root / "docs/project_art_plan/rill_visual_state_catalog.json").read_text(
                encoding="utf-8"
            )
        )
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "catalog.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = rill_visual_state_gate.validate_catalog(self.root, self.path)
        return {finding.code for finding in result.findings}

    def _profile(self, state: str) -> dict:
        return next(profile for profile in self.catalog["profiles"] if profile["state"] == state)

    def test_repository_catalog_passes(self) -> None:
        result = rill_visual_state_gate.validate_catalog(self.root, self.path)
        self.assertEqual((), result.findings)
        self.assertEqual(9, result.profile_count)

    def test_every_memory_state_needs_one_profile(self) -> None:
        self.catalog["profiles"] = [
            profile for profile in self.catalog["profiles"] if profile["state"] != "EndgameD"
        ]
        self._write()
        codes = self._codes()
        self.assertIn("STATE_PROFILES_MISSING", codes)
        self.assertIn("PROFILE_COUNT_INVALID", codes)
        self.assertIn("PROFILE_ORDER_INVALID", codes)

    def test_second_emotion_runtime_remains_forbidden(self) -> None:
        self.catalog["invariants"]["secondEmotionRuntimeAllowed"] = True
        self._write()
        self.assertIn("INVARIANT_DRIFT", self._codes())

    def test_repair_panel_remains_visible_in_all_states(self) -> None:
        self.catalog["invariants"]["repairPanelVisibleAllStates"] = False
        self._write()
        self.assertIn("INVARIANT_DRIFT", self._codes())

    def test_intensities_stay_normalized(self) -> None:
        self._profile("Stirring")["channels"]["intensity"] = 1.4
        self._write()
        self.assertIn("INTENSITY_RANGE", self._codes())

    def test_glitch_state_requires_safe_burst_and_gap(self) -> None:
        rails = self._profile("Remembering")["glitchRails"]
        rails["maxBurstSeconds"] = 0.8
        rails["minimumGapSeconds"] = 1.0
        rails["fullFrameFlicker"] = True
        self._write()
        codes = self._codes()
        self.assertIn("GLITCH_BURST_INVALID", codes)
        self.assertIn("GLITCH_GAP_INVALID", codes)
        self.assertIn("FULL_FRAME_FLICKER_FORBIDDEN", codes)

    def test_current_runtime_baseline_is_recorded_exactly(self) -> None:
        self.catalog["currentRuntimeBaseline"]["Dormant"]["rgb"] = [1, 0, 0]
        self.catalog["currentRuntimeBaseline"]["laterStatesCollapsedToDefault"]["states"] = [
            "Integrated"
        ]
        self._write()
        codes = self._codes()
        self.assertIn("BASELINE_DRIFT", codes)
        self.assertIn("LATE_STATE_SET_DRIFT", codes)

    def test_personal_space_cannot_shrink_below_comfort_floor(self) -> None:
        self.catalog["invariants"]["minimumConversationDistanceMeters"] = 0.2
        self._write()
        self.assertIn("PERSONAL_SPACE_INVALID", self._codes())

    def test_profile_order_follows_story_progression(self) -> None:
        profiles = self.catalog["profiles"]
        profiles[0], profiles[1] = profiles[1], profiles[0]
        self._write()
        self.assertIn("PROFILE_ORDER_INVALID", self._codes())

    def test_source_state_contract_is_verified_against_real_code(self) -> None:
        self.catalog["stateSource"] = "docs/project_art_plan/DEVICE_GATE_PRODUCTION_PACK.md"
        self._write()
        self.assertIn("SOURCE_STATE_MISSING", self._codes())


if __name__ == "__main__":
    unittest.main()
