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

import first_hour_gate


class FirstHourGateTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        production_contract = (
            Path(__file__).resolve().parents[2]
            / "docs/first_hour/first_hour_beats.json"
        )
        cls.base_contract = json.loads(production_contract.read_text(encoding="utf-8"))

    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.contract = copy.deepcopy(self.base_contract)
        for source in self.contract["sourceDocuments"]:
            path = self.root / source
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text("fixture\n", encoding="utf-8")
        self.contract_path = self.root / "docs/first_hour/first_hour_beats.json"
        self._write_contract()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write_contract(self) -> None:
        self.contract_path.parent.mkdir(parents=True, exist_ok=True)
        self.contract_path.write_text(json.dumps(self.contract), encoding="utf-8")

    def _result(self) -> first_hour_gate.ValidationResult:
        return first_hour_gate.validate_contract(self.root, self.contract_path)

    def _codes(self) -> set[str]:
        return {finding.code for finding in self._result().findings}

    def test_production_contract_is_valid(self) -> None:
        result = self._result()
        self.assertEqual((), result.findings)
        self.assertEqual(22, result.beat_count)
        self.assertEqual(15, result.core_verb_count)

    def test_duplicate_sequence_is_reported(self) -> None:
        self.contract["beats"][1]["sequence"] = 1
        self._write_contract()
        codes = self._codes()
        self.assertIn("BEAT_SEQUENCE_DUPLICATE", codes)
        self.assertIn("BEAT_SEQUENCE_NOT_CONTIGUOUS", codes)

    def test_missing_core_verb_teaching_is_reported(self) -> None:
        self.contract["beats"][2].pop("teachesVerb")
        self.contract["beats"][2].pop("hesitationSeconds")
        self.contract["beats"][2].pop("rillLine")
        self._write_contract()
        self.assertIn("CORE_VERB_UNTAUGHT", self._codes())

    def test_duplicate_core_verb_teaching_is_reported(self) -> None:
        self.contract["beats"][3]["teachesVerb"] = "LOOK"
        self._write_contract()
        self.assertIn("CORE_VERB_TAUGHT_MULTIPLE", self._codes())

    def test_required_beat_cannot_depend_on_optional_beat(self) -> None:
        self.contract["beats"][9]["required"] = False
        self._write_contract()
        self.assertIn("REQUIRED_DEPENDS_ON_OPTIONAL", self._codes())

    def test_input_lock_and_rig_motion_are_rejected(self) -> None:
        self.contract["beats"][4]["allowsInputLock"] = True
        self.contract["beats"][5]["movesPlayerRig"] = True
        self._write_contract()
        codes = self._codes()
        self.assertIn("INPUT_LOCK_FORBIDDEN", codes)
        self.assertIn("RIG_MOTION_FORBIDDEN", codes)

    def test_forward_prerequisite_is_rejected(self) -> None:
        self.contract["beats"][4]["prerequisites"] = ["FH_GRAB_BUNK_OBJECT"]
        self._write_contract()
        self.assertIn("PREREQUISITE_NOT_EARLIER", self._codes())

    def test_report_only_succeeds_but_strict_fails(self) -> None:
        self.contract["beats"][-1]["setsFlags"] = []
        self._write_contract()
        report_path = self.root / "Builds/Reports/first_hour_contract_report.json"

        report_only = first_hour_gate.main(
            [
                "--root",
                str(self.root),
                "--contract",
                str(self.contract_path),
                "--json-report",
                str(report_path),
            ]
        )
        strict = first_hour_gate.main(
            [
                "--root",
                str(self.root),
                "--contract",
                str(self.contract_path),
                "--strict",
            ]
        )

        self.assertEqual(first_hour_gate.EXIT_OK, report_only)
        self.assertEqual(first_hour_gate.EXIT_VALIDATION_FAILED, strict)
        report = json.loads(report_path.read_text(encoding="utf-8"))
        self.assertEqual("warning", report["status"])
        self.assertIn(
            "FINAL_COMPLETION_FLAGS_MISSING",
            {finding["code"] for finding in report["findings"]},
        )


if __name__ == "__main__":
    unittest.main()
