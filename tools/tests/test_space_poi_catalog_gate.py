from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import space_poi_catalog_gate


class SpacePoiCatalogGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.root = Path(__file__).resolve().parents[2]
        self.catalog = json.loads(
            (self.root / "docs/design/space_poi_catalog.json").read_text(encoding="utf-8")
        )
        self.missions = self.root / "docs/design/space_mission_catalog.json"
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "catalog.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = space_poi_catalog_gate.validate_catalog(self.root, self.path, self.missions)
        return {finding.code for finding in result.findings}

    def _poi(self, poi_id: str) -> dict:
        return next(poi for poi in self.catalog["pois"] if poi["id"] == poi_id)

    def test_repository_catalog_passes(self) -> None:
        result = space_poi_catalog_gate.validate_catalog(self.root, self.path, self.missions)
        self.assertEqual((), result.findings)
        self.assertEqual(6, result.poi_count)
        self.assertGreaterEqual(result.mission_coverage_count, 5)

    def test_runtime_spawning_stays_frozen(self) -> None:
        self.catalog["scope"]["runtimeSpawningAuthorized"] = True
        self._write()
        self.assertIn("SCOPE_LOCK_VIOLATION", self._codes())

    def test_all_core_missions_need_a_place(self) -> None:
        for poi in self.catalog["pois"]:
            poi["missionCompatibility"] = [
                mission for mission in poi["missionCompatibility"] if mission != "relay_seal_repair"
            ]
        self._write()
        self.assertIn("CORE_MISSION_POI_GAP", self._codes())

    def test_prior_waker_keeps_ordered_log_thread(self) -> None:
        poi = self._poi("prior_waker_smallcraft")
        poi["storyDrop"]["orderedThread"] = False
        self._write()
        self.assertIn("WAKER_THREAD_REQUIRED", self._codes())

    def test_relay_keeps_signal_hook_and_effect(self) -> None:
        poi = self._poi("orbital_repair_relay")
        poi["requiredSockets"].remove("signal_hook")
        poi["hazards"].remove("raises_signal")
        self._write()
        codes = self._codes()
        self.assertIn("RELAY_SIGNAL_SOCKET_REQUIRED", codes)
        self.assertIn("RELAY_SIGNAL_EFFECT_REQUIRED", codes)

    def test_gate_approach_keeps_alignment_and_arrival_sockets(self) -> None:
        poi = self._poi("ziptide_gate_approach_lane")
        poi["requiredSockets"].remove("safe_arrival_anchor")
        self._write()
        self.assertIn("GATE_SOCKET_REQUIRED", self._codes())

    def test_distress_ship_needs_tow_and_repair_contract(self) -> None:
        poi = self._poi("stranded_civilian_ship")
        poi["requiredSockets"].remove("tow_attach")
        self._write()
        self.assertIn("DISTRESS_SOCKET_REQUIRED", self._codes())

    def test_budget_class_must_exist(self) -> None:
        self._poi("debris_ribbon")["budgetClass"] = "unbounded"
        self._write()
        self.assertIn("BUDGET_CLASS_MISSING", self._codes())

    def test_unreviewed_poi_requires_amendment(self) -> None:
        extra = dict(self.catalog["pois"][0])
        extra["id"] = "random_combat_arena"
        self.catalog["pois"].append(extra)
        self._write()
        codes = self._codes()
        self.assertIn("UNREVIEWED_POIS_PRESENT", codes)
        self.assertIn("POI_COUNT_INVALID", codes)

    def test_pois_remain_pacifist_accessible(self) -> None:
        self._poi("debris_ribbon")["pacifistAccessible"] = False
        self._write()
        self.assertIn("PACIFIST_ACCESS_REQUIRED", self._codes())


if __name__ == "__main__":
    unittest.main()
