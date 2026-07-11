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

import first_hour_envelope_gate


class FirstHourEnvelopeGateTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        root = Path(__file__).resolve().parents[2]
        cls.base_index = json.loads(
            (root / "docs/first_hour/adapter_envelopes.json").read_text(encoding="utf-8")
        )
        cls.base_contract = json.loads(
            (root / "docs/first_hour/first_hour_beats.json").read_text(encoding="utf-8")
        )
        cls.base_inventory = json.loads(
            (root / "docs/first_hour/first_hour_bindings.json").read_text(encoding="utf-8")
        )
        cls.base_envelopes = {}
        for relative in cls.base_index["envelopeFiles"]:
            row = json.loads((root / relative).read_text(encoding="utf-8"))
            cls.base_envelopes[row["id"]] = row

    def setUp(self) -> None:
        self.tmp = tempfile.TemporaryDirectory()
        self.root = Path(self.tmp.name)
        self.index = copy.deepcopy(self.base_index)
        self.contract = copy.deepcopy(self.base_contract)
        self.inventory = copy.deepcopy(self.base_inventory)
        self.envelopes = copy.deepcopy(self.base_envelopes)
        self.index_path = self.root / "docs/first_hour/adapter_envelopes.json"
        self._write_all()
        self._materialize_touches()

    def tearDown(self) -> None:
        self.tmp.cleanup()

    def _write_all(self) -> None:
        folder = self.root / "docs/first_hour"
        folder.mkdir(parents=True, exist_ok=True)
        self.index_path.write_text(json.dumps(self.index), encoding="utf-8")
        (folder / "first_hour_beats.json").write_text(json.dumps(self.contract), encoding="utf-8")
        (folder / "first_hour_bindings.json").write_text(json.dumps(self.inventory), encoding="utf-8")
        for relative in self.index["envelopeFiles"]:
            envelope_id = Path(relative).stem
            path = self.root / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text(json.dumps(self.envelopes[envelope_id]), encoding="utf-8")

    def _materialize_touches(self) -> None:
        for row in self.envelopes.values():
            for raw in row.get("touchFiles", []):
                path = self.root / raw
                path.parent.mkdir(parents=True, exist_ok=True)
                path.write_text("// fixture\n", encoding="utf-8")

    def _row(self, envelope_id: str) -> dict:
        return self.envelopes[envelope_id]

    def _result(self):
        return first_hour_envelope_gate.validate(self.root, self.index_path)

    def _codes(self):
        return {finding.code for finding in self._result().findings}

    def test_production_package_is_valid(self) -> None:
        result = self._result()
        self.assertEqual((), result.findings)
        self.assertEqual(12, result.envelope_count)
        self.assertEqual(18, result.covered_beats)
        self.assertEqual(15, result.covered_lines)
        self.assertEqual(
            {"architecture": 2, "art": 1, "multiplayer": 1, "story-ship": 8},
            result.owner_counts,
        )

    def test_missing_envelope_file_is_reported(self) -> None:
        missing = self.index["envelopeFiles"][0]
        (self.root / missing).unlink()
        self.assertIn("ENVELOPE_FILE_MISSING", self._codes())

    def test_uncovered_beat_is_reported(self) -> None:
        row = self._row("FH-S02-HOLSTER")
        row["covers"] = []
        row["signals"] = []
        self._write_all()
        self.assertIn("BEAT_ENVELOPE_MISSING", self._codes())

    def test_verified_direct_beat_cannot_be_covered(self) -> None:
        row = self._row("FH-S01-OBSERVATION")
        row["covers"].append("FH_ACCEPT_FIRST_JOB")
        row["signals"].append("FIRST_JOB_ACCEPTED")
        self._write_all()
        codes = self._codes()
        self.assertIn("DIRECT_BEAT_REIMPLEMENTED", codes)
        self.assertIn("BEAT_ENVELOPE_UNEXPECTED", codes)

    def test_signal_mismatch_is_reported(self) -> None:
        self._row("FH-S03-TRAVEL")["signals"] = ["WRONG", "WRONG"]
        self._write_all()
        self.assertIn("SIGNAL_MISMATCH", self._codes())

    def test_missing_line_is_reported(self) -> None:
        self._row("FH-S08-W001-ORCHESTRATION")["lines"].pop()
        self._write_all()
        codes = self._codes()
        self.assertIn("LINE_ENVELOPE_MISSING", codes)
        self.assertIn("LINE_ID_ENVELOPE_MISSING", codes)

    def test_unknown_dependency_and_cycle_are_reported(self) -> None:
        self._row("FH-X01-CONTRACT-ASSET")["dependencies"] = [
            "FH-X02-PROGRESSION-CORE",
            "NO_SUCH",
        ]
        self._row("FH-X02-PROGRESSION-CORE")["dependencies"] = ["FH-X01-CONTRACT-ASSET"]
        self._write_all()
        codes = self._codes()
        self.assertIn("DEPENDENCY_UNKNOWN", codes)
        self.assertIn("DEPENDENCY_CYCLE", codes)

    def test_missing_touch_file_is_reported(self) -> None:
        raw = self._row("FH-S02-HOLSTER")["touchFiles"][0]
        (self.root / raw).unlink()
        self.assertIn("TOUCH_FILE_MISSING", self._codes())

    def test_shared_protocol_is_required(self) -> None:
        self._row("FH-S03-TRAVEL").pop("sharedProtocol")
        self._write_all()
        self.assertIn("SHARED_PROTOCOL_REQUIRED", self._codes())

    def test_report_only_succeeds_but_strict_fails(self) -> None:
        self._row("FH-S03-TRAVEL")["signals"] = ["WRONG", "WRONG"]
        self._write_all()
        report = self.root / "Builds/Reports/first_hour_envelope_report.json"
        normal = first_hour_envelope_gate.main([
            "--root", str(self.root),
            "--envelopes", str(self.index_path),
            "--json-report", str(report),
        ])
        strict = first_hour_envelope_gate.main([
            "--root", str(self.root),
            "--envelopes", str(self.index_path),
            "--strict",
        ])
        self.assertEqual(first_hour_envelope_gate.EXIT_OK, normal)
        self.assertEqual(first_hour_envelope_gate.EXIT_VALIDATION_FAILED, strict)
        payload = json.loads(report.read_text(encoding="utf-8"))
        self.assertEqual("warning", payload["status"])
        self.assertIn("SIGNAL_MISMATCH", {row["code"] for row in payload["findings"]})


if __name__ == "__main__":
    unittest.main()
