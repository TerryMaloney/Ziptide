from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import launch_transition_gate


class LaunchTransitionGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.root = Path(__file__).resolve().parents[2]
        self.catalog = json.loads(
            (self.root / "docs/design/launch_transition_catalog.json").read_text(encoding="utf-8")
        )
        self.celestial = self.root / "docs/design/celestial_system_catalog.json"
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "launch.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = launch_transition_gate.validate_catalog(self.root, self.path, self.celestial)
        return {finding.code for finding in result.findings}

    def _route(self, route_id: str) -> dict:
        return next(route for route in self.catalog["routes"] if route["id"] == route_id)

    def _phase(self, route_id: str, phase_id: str) -> dict:
        return next(
            phase for phase in self._route(route_id)["phases"] if phase["id"] == phase_id
        )

    def test_repository_catalog_passes(self) -> None:
        result = launch_transition_gate.validate_catalog(self.root, self.path, self.celestial)
        self.assertEqual((), result.findings)
        self.assertEqual(2, result.route_count)
        self.assertEqual(7, result.phase_count)

    def test_travel_and_camera_ownership_cannot_drift(self) -> None:
        self.catalog["owner"] = "LaunchVisual"
        self.catalog["cameraOwner"] = "ship-hull"
        self._write()
        codes = self._codes()
        self.assertIn("OWNER_INVALID", codes)
        self.assertIn("CAMERA_OWNER_INVALID", codes)

    def test_planet_and_space_heavy_scenes_cannot_coload(self) -> None:
        self.catalog["globalContract"]["planetAndSpaceHeavyScenesCoLoaded"] = True
        self._write()
        self.assertIn("GLOBAL_CONTRACT_DRIFT", self._codes())

    def test_celestial_source_fields_cannot_be_duplicated_or_drift(self) -> None:
        route = self._route("launch_w001_to_moss_orbit")
        route["sourceFields"]["primaryStar"] = "hardcoded_star"
        self._write()
        self.assertIn("SOURCE_FIELD_DRIFT", self._codes())

    def test_launch_phase_order_is_locked(self) -> None:
        phases = self._route("launch_w001_to_moss_orbit")["phases"]
        phases[1], phases[2] = phases[2], phases[1]
        self._write()
        codes = self._codes()
        self.assertIn("PHASE_ORDER_INVALID", codes)
        self.assertIn("PHASE_SEQUENCE_INVALID", codes)

    def test_veil_must_be_one_to_two_seconds_and_hold_travel_controls(self) -> None:
        veil = self._phase("launch_w001_to_moss_orbit", "veil")
        veil["durationSeconds"]["min"] = 0.1
        veil["durationSeconds"]["max"] = 0.2
        veil["durationSeconds"]["loadHoldAllowed"] = False
        veil["inputPolicy"] = "player-controlled"
        self._write()
        codes = self._codes()
        self.assertIn("VEIL_DURATION_DRIFT", codes)
        self.assertIn("VEIL_LOAD_HOLD_REQUIRED", codes)
        self.assertIn("VEIL_CONTROL_OWNER_INVALID", codes)

    def test_scene_load_hold_is_forbidden_outside_veil(self) -> None:
        self._phase("launch_w001_to_moss_orbit", "ascent")["durationSeconds"][
            "loadHoldAllowed"
        ] = True
        self._write()
        self.assertIn("LOAD_HOLD_OUTSIDE_VEIL", self._codes())

    def test_arrival_and_touchdown_wait_for_stable_input(self) -> None:
        arrive = self._phase("launch_w001_to_moss_orbit", "arrive")
        arrive["inputPolicy"] = "restore-immediately"
        touchdown = self._phase("reentry_moss_orbit_to_w001", "touchdown")
        touchdown["exitCondition"] = "ground-visible"
        self._write()
        codes = self._codes()
        self.assertIn("CONTROL_RESTORE_STABILITY", codes)
        self.assertIn("CONTROL_RESTORE_EXIT", codes)

    def test_route_must_match_celestial_destination(self) -> None:
        route = self._route("launch_w001_to_moss_orbit")
        route["destinationSpaceView"] = "wrong_space"
        self._write()
        codes = self._codes()
        self.assertIn("DESTINATION_VIEW_MISMATCH", codes)
        self.assertIn("DESTINATION_VIEW_MISSING", codes)

    def test_unreviewed_route_requires_canon_amendment(self) -> None:
        extra = json.loads(json.dumps(self.catalog["routes"][0]))
        extra["id"] = "launch_random_world"
        self.catalog["routes"].append(extra)
        self._write()
        codes = self._codes()
        self.assertIn("UNREVIEWED_ROUTE_PRESENT", codes)
        self.assertIn("UNREVIEWED_ROUTES_PRESENT", codes)
        self.assertIn("ROUTE_COUNT_INVALID", codes)

    def test_required_logs_follow_direction_and_phase(self) -> None:
        self._phase("reentry_moss_orbit_to_w001", "descent")["requiredLog"] = (
            "ZIPTIDE: LAUNCH phase=descent"
        )
        self._write()
        self.assertIn("LOG_CONTRACT_INVALID", self._codes())


if __name__ == "__main__":
    unittest.main()
