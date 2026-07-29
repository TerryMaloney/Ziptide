"""Tests for tools/worldspec_contract_gate.py — the spec ↔ contract reconcile gate.

The gate exists because WorldSpecCompiler overwrites layout/pack blocks wholesale: a spec that
drops a marker/machine/drone zone strands the contract step that references it (the
JOB_MARKER_MISSING class). These tests pin both directions — a faithful fixture passes, and each
divergence class produces its named finding.
"""

import json
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

import worldspec_contract_gate as gate


JOB_GUID = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
STEP_GO_GUID = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
STEP_REPAIR_GUID = "cccccccccccccccccccccccccccccccc"
STEP_DRONES_GUID = "dddddddddddddddddddddddddddddddd"


def _write(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def _meta(path: Path, guid: str) -> None:
    _write(path, "fileFormatVersion: 2\nguid: " + guid + "\n")


def _spec(markers=("dispatch_inside",), machines=("signal_relay",),
          drone_count=5, item_id="artifact_half_b", part_id="relay_cell"):
    return {
        "sceneName": "ToxicCity",
        "districts": [
            {"id": "Dispatch", "heroBuildings": [
                {"id": "Hall", "interiorMarkerId": m} for m in markers]},
        ],
        "pois": [],
        "droneZones": [{"id": "Z", "count": drone_count}],
        "machines": [{"machineId": m, "partItemId": part_id} for m in machines],
        "collectibles": [{"itemId": item_id}],
    }


class WorldspecContractGateTests(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.root = Path(self._tmp.name)

    def tearDown(self):
        self._tmp.cleanup()

    # ── fixture builders ────────────────────────────────────────────────────

    def _stage(self, spec=None, marker="dispatch_inside", machine="signal_relay",
               drones=5):
        _write(self.root / gate.SPEC_DIR / "ToxicCity.spec.json",
               json.dumps(spec if spec is not None else _spec()))

        _write(self.root / gate.PACK_DIR / "ToxicCity_WorldPack.asset",
               "MonoBehaviour:\n  packId: toxic_city\n  jobs:\n"
               "  - {fileID: 11400000, guid: " + JOB_GUID + ", type: 2}\n"
               "  spawnMarkers: []\n")

        jobs = self.root / gate.JOBS_DIR
        _write(jobs / "Contract.asset",
               "MonoBehaviour:\n  jobId: toxiccity_contract\n  steps:\n"
               "  - {fileID: 11400000, guid: " + STEP_GO_GUID + ", type: 2}\n"
               "  - {fileID: 11400000, guid: " + STEP_REPAIR_GUID + ", type: 2}\n"
               "  - {fileID: 11400000, guid: " + STEP_DRONES_GUID + ", type: 2}\n"
               "  reward: []\n")
        _meta(jobs / "Contract.asset.meta", JOB_GUID)

        _write(jobs / "S1_Go.asset", "MonoBehaviour:\n  markerId: " + marker + "\n")
        _meta(jobs / "S1_Go.asset.meta", STEP_GO_GUID)
        _write(jobs / "S2_Repair.asset",
               "MonoBehaviour:\n  machineId: " + machine + "\n  count: 1\n")
        _meta(jobs / "S2_Repair.asset.meta", STEP_REPAIR_GUID)
        _write(jobs / "S3_Drones.asset", "MonoBehaviour:\n  count: " + str(drones) + "\n")
        _meta(jobs / "S3_Drones.asset.meta", STEP_DRONES_GUID)

        items = self.root / gate.ITEMS_DIR
        _write(items / "ArtifactHalfB.asset", "MonoBehaviour:\n  itemId: artifact_half_b\n")
        _write(items / "RelayCell.asset", "MonoBehaviour:\n  itemId: relay_cell\n")

    def _codes(self, result):
        return [f.code for f in result.findings]

    # ── the contract both ways ──────────────────────────────────────────────

    def test_faithful_spec_and_contract_pass(self):
        self._stage()
        result = gate.run_gate(self.root)
        self.assertEqual("pass", result.status, msg=str(result.findings))
        self.assertEqual(3, result.checked_step_count)

    def test_spec_dropping_the_marker_fails(self):
        self._stage(spec=_spec(markers=("some_other_room",)))
        result = gate.run_gate(self.root)
        self.assertEqual("fail", result.status)
        self.assertIn("CONTRACT_MARKER_UNKNOWN", self._codes(result))

    def test_spec_dropping_the_machine_fails(self):
        self._stage(spec=_spec(machines=()))
        result = gate.run_gate(self.root)
        self.assertIn("CONTRACT_MACHINE_UNKNOWN", self._codes(result))

    def test_spec_without_enough_drones_fails(self):
        self._stage(spec=_spec(drone_count=2))
        result = gate.run_gate(self.root)
        self.assertIn("CONTRACT_DRONE_CAPACITY", self._codes(result))

    def test_poi_markers_satisfy_goto_steps(self):
        spec = _spec(markers=())
        spec["pois"] = [{"id": "relay_site"}]
        self._stage(spec=spec, marker="poi_relay_site")
        result = gate.run_gate(self.root)
        self.assertNotIn("CONTRACT_MARKER_UNKNOWN", self._codes(result))

    def test_unresolvable_item_fails(self):
        self._stage(spec=_spec(part_id="phantom_cell"))
        result = gate.run_gate(self.root)
        self.assertIn("WSPEC_ITEM_UNRESOLVED", self._codes(result))

    def test_missing_pack_is_a_warning_not_a_fail(self):
        _write(self.root / gate.SPEC_DIR / "W099.spec.json",
               json.dumps({"sceneName": "W099", "districts": [], "droneZones": [],
                           "machines": [], "collectibles": []}))
        result = gate.run_gate(self.root)
        self.assertEqual("warning", result.status)
        self.assertIn("WSPEC_PACK_MISSING", self._codes(result))

    def test_unparseable_spec_fails(self):
        _write(self.root / gate.SPEC_DIR / "Broken.spec.json", "{not json")
        result = gate.run_gate(self.root)
        self.assertEqual("fail", result.status)
        self.assertIn("WSPEC_UNREADABLE", self._codes(result))

    def test_dangling_step_guid_fails(self):
        self._stage()
        (self.root / gate.JOBS_DIR / "S1_Go.asset.meta").unlink()
        result = gate.run_gate(self.root)
        self.assertIn("CONTRACT_STEP_UNRESOLVED", self._codes(result))

    def test_no_specs_passes_vacuously(self):
        result = gate.run_gate(self.root)
        self.assertEqual("pass", result.status)
        self.assertEqual(0, result.spec_count)

    def test_main_exit_codes(self):
        self._stage()
        self.assertEqual(gate.EXIT_OK, gate.main(["--root", str(self.root)]))
        self._stage(spec=_spec(markers=("elsewhere",)))
        self.assertEqual(gate.EXIT_VALIDATION_FAILED,
                         gate.main(["--root", str(self.root)]))

    def test_json_report_is_written(self):
        self._stage()
        report = self.root / "report.json"
        gate.main(["--root", str(self.root), "--json-report", str(report)])
        payload = json.loads(report.read_text(encoding="utf-8"))
        self.assertEqual("worldspec_contract_gate", payload["tool"])
        self.assertEqual("pass", payload["status"])


if __name__ == "__main__":
    unittest.main()
