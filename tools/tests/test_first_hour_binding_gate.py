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

import first_hour_binding_gate


class FirstHourBindingGateTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        repo_root = Path(__file__).resolve().parents[2]
        cls.base_inventory = json.loads(
            (repo_root / "docs/first_hour/first_hour_bindings.json").read_text(encoding="utf-8")
        )
        cls.base_contract = json.loads(
            (repo_root / "docs/first_hour/first_hour_beats.json").read_text(encoding="utf-8")
        )

    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.inventory = copy.deepcopy(self.base_inventory)
        self.contract = copy.deepcopy(self.base_contract)
        self.inventory_path = self.root / "docs/first_hour/first_hour_bindings.json"
        self.contract_path = self.root / "docs/first_hour/first_hour_beats.json"
        self._write_contract()
        self._materialize_evidence()
        self._write_inventory()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write_contract(self) -> None:
        self.contract_path.parent.mkdir(parents=True, exist_ok=True)
        self.contract_path.write_text(json.dumps(self.contract), encoding="utf-8")

    def _write_inventory(self) -> None:
        self.inventory_path.parent.mkdir(parents=True, exist_ok=True)
        self.inventory_path.write_text(json.dumps(self.inventory), encoding="utf-8")

    def _all_evidence_entries(self):
        for binding in self.inventory.get("bindings", []):
            yield from binding.get("evidence", [])
        for line in self.inventory.get("rillLines", []):
            yield from line.get("evidence", [])
        for decision in self.inventory.get("globalDecisions", []):
            yield from decision.get("evidence", [])

    def _materialize_evidence(self) -> None:
        tokens_by_path: dict[str, list[str]] = {}
        for evidence in self._all_evidence_entries():
            tokens_by_path.setdefault(evidence["path"], []).extend(evidence.get("tokens", []))
        for relative_path, tokens in tokens_by_path.items():
            path = self.root / relative_path
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text("\n".join(dict.fromkeys(tokens)) + "\n", encoding="utf-8")

    def _result(self) -> first_hour_binding_gate.ValidationResult:
        return first_hour_binding_gate.validate_inventory(self.root, self.inventory_path)

    def _codes(self) -> set[str]:
        return {finding.code for finding in self._result().findings}

    def test_production_inventory_is_valid(self) -> None:
        result = self._result()
        self.assertEqual((), result.findings)
        self.assertEqual(22, result.binding_count)
        self.assertEqual(15, result.line_binding_count)
        self.assertEqual(
            {
                "adapter-required": 9,
                "composite-required": 5,
                "new-surface-required": 4,
                "verified-existing": 4,
            },
            result.status_counts,
        )

    def test_missing_completion_binding_is_reported(self) -> None:
        self.inventory["bindings"].pop()
        self._write_inventory()
        self.assertIn("BINDING_BEAT_MISSING", self._codes())

    def test_mismatched_completion_signal_is_reported(self) -> None:
        self.inventory["bindings"][0]["signalId"] = "WRONG_SIGNAL"
        self._write_inventory()
        self.assertIn("BINDING_SIGNAL_MISMATCH", self._codes())

    def test_missing_evidence_path_is_reported(self) -> None:
        self.inventory["bindings"][0]["evidence"][0]["path"] = "missing/file.cs"
        self._write_inventory()
        self.assertIn("EVIDENCE_PATH_MISSING", self._codes())

    def test_missing_evidence_token_is_reported(self) -> None:
        self.inventory["bindings"][0]["evidence"][0]["tokens"].append("TOKEN_NOT_IN_FILE")
        self._write_inventory()
        self.assertIn("EVIDENCE_TOKEN_MISSING", self._codes())

    def test_verified_existing_requires_evidence(self) -> None:
        verified = next(
            binding
            for binding in self.inventory["bindings"]
            if binding["status"] == "verified-existing"
        )
        verified["evidence"] = []
        self._write_inventory()
        codes = self._codes()
        self.assertIn("EVIDENCE_REQUIRED", codes)
        self.assertIn("VERIFIED_BINDING_WITHOUT_EVIDENCE", codes)

    def test_missing_teaching_line_binding_is_reported(self) -> None:
        self.inventory["rillLines"].pop()
        self._write_inventory()
        self.assertIn("LINE_BEAT_MISSING", self._codes())

    def test_unknown_owner_and_status_are_reported(self) -> None:
        self.inventory["bindings"][0]["owner"] = "unknown-lane"
        self.inventory["bindings"][0]["status"] = "magic"
        self._write_inventory()
        codes = self._codes()
        self.assertIn("BINDING_OWNER_UNKNOWN", codes)
        self.assertIn("BINDING_STATUS_UNKNOWN", codes)

    def test_report_only_succeeds_but_strict_fails(self) -> None:
        self.inventory["bindings"][0]["signalId"] = "WRONG_SIGNAL"
        self._write_inventory()
        report_path = self.root / "Builds/Reports/first_hour_binding_report.json"

        report_only = first_hour_binding_gate.main(
            [
                "--root",
                str(self.root),
                "--inventory",
                str(self.inventory_path),
                "--json-report",
                str(report_path),
            ]
        )
        strict = first_hour_binding_gate.main(
            [
                "--root",
                str(self.root),
                "--inventory",
                str(self.inventory_path),
                "--strict",
            ]
        )

        self.assertEqual(first_hour_binding_gate.EXIT_OK, report_only)
        self.assertEqual(first_hour_binding_gate.EXIT_VALIDATION_FAILED, strict)
        report = json.loads(report_path.read_text(encoding="utf-8"))
        self.assertEqual("warning", report["status"])
        self.assertIn(
            "BINDING_SIGNAL_MISMATCH",
            {finding["code"] for finding in report["findings"]},
        )


if __name__ == "__main__":
    unittest.main()
