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

import space_mission_catalog_gate


class SpaceMissionCatalogGateTests(unittest.TestCase):
    def setUp(self) -> None:
        repo_root = Path(__file__).resolve().parents[2]
        source = repo_root / "docs/design/space_mission_catalog.json"
        self.catalog = json.loads(source.read_text(encoding="utf-8"))
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "catalog.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = space_mission_catalog_gate.validate_catalog(self.path)
        return {finding.code for finding in result.findings}

    def test_repository_catalog_passes(self) -> None:
        result = space_mission_catalog_gate.validate_catalog(self.path)
        self.assertEqual((), result.findings)
        self.assertEqual(10, result.mission_count)

    def test_scope_lock_cannot_drift_to_newtonian(self) -> None:
        self.catalog["scope"]["flightModel"] = "newtonian"
        self._write()
        self.assertIn("SCOPE_LOCK_VIOLATION", self._codes())

    def test_core_mission_must_remain_pacifist_viable(self) -> None:
        mission = next(m for m in self.catalog["missions"] if m["id"] == "salvage_run")
        mission["pacifistViable"] = False
        self._write()
        self.assertIn("CORE_PACIFIST_REQUIRED", self._codes())

    def test_relay_repair_must_raise_signal(self) -> None:
        mission = next(m for m in self.catalog["missions"] if m["id"] == "relay_seal_repair")
        mission["signalEffect"] = "none"
        self._write()
        self.assertIn("SIGNAL_HOOK_REQUIRED", self._codes())

    def test_unknown_step_requires_contract_amendment(self) -> None:
        self.catalog["missions"][0]["steps"].append("InventedStep")
        self._write()
        self.assertIn("STEP_UNKNOWN", self._codes())

    def test_unreviewed_extra_mission_is_rejected(self) -> None:
        extra = copy.deepcopy(self.catalog["missions"][0])
        extra["id"] = "random_combat_filler"
        self.catalog["missions"].append(extra)
        self._write()
        codes = self._codes()
        self.assertIn("UNREVIEWED_MISSIONS_PRESENT", codes)
        self.assertIn("MISSION_COUNT_INVALID", codes)

    def test_the_approach_stays_unique_and_chapter_eight(self) -> None:
        mission = next(m for m in self.catalog["missions"] if m["id"] == "the_approach")
        mission["repeatable"] = True
        mission["unlockChapter"] = 7
        self._write()
        codes = self._codes()
        self.assertIn("APPROACH_UNIQUE_REQUIRED", codes)
        self.assertIn("APPROACH_CHAPTER_INVALID", codes)

    def test_core_missions_cannot_gain_timers(self) -> None:
        mission = next(m for m in self.catalog["missions"] if m["id"] == "ferry_passage")
        mission["failurePolicy"] = "fail-after-60-seconds"
        self._write()
        self.assertIn("TIMER_POLICY_VIOLATION", self._codes())


if __name__ == "__main__":
    unittest.main()
