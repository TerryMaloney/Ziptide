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

import first_hour_launch_gate


class FirstHourLaunchGateTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        root = Path(__file__).resolve().parents[2]
        cls.manifest = json.loads((root / "docs/first_hour/opus_launch_manifest.json").read_text())
        cls.index = json.loads((root / "docs/first_hour/adapter_envelopes.json").read_text())
        cls.envelopes = {Path(raw).stem: json.loads((root / raw).read_text()) for raw in cls.index["envelopeFiles"]}

    def setUp(self) -> None:
        self.tmp = tempfile.TemporaryDirectory()
        self.root = Path(self.tmp.name)
        self.payload = copy.deepcopy(self.manifest)
        self.path = self.root / "docs/first_hour/opus_launch_manifest.json"
        self._write()

    def tearDown(self) -> None:
        self.tmp.cleanup()

    def _write(self) -> None:
        self.path.parent.mkdir(parents=True, exist_ok=True)
        self.path.write_text(json.dumps(self.payload))
        index_path = self.root / "docs/first_hour/adapter_envelopes.json"
        index_path.write_text(json.dumps(self.index))
        for raw in self.index["envelopeFiles"]:
            target = self.root / raw
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text(json.dumps(self.envelopes[Path(raw).stem]))
        for raw in self.payload["requiredGlobalReads"] + [lane["board"] for lane in self.payload["lanes"]]:
            target = self.root / raw
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text("fixture\n")

    def _codes(self):
        return {item.code for item in first_hour_launch_gate.validate(self.root, self.path).findings}

    def test_package_is_valid(self) -> None:
        result = first_hour_launch_gate.validate(self.root, self.path)
        self.assertEqual((), result.findings)
        self.assertEqual(4, result.lane_count)
        self.assertEqual(12, result.assigned_envelopes)

    def test_wrong_owner_is_reported(self) -> None:
        self.payload["lanes"][1]["allowedEnvelopes"].append("FH-S02-HOLSTER")
        self._write()
        self.assertIn("ASSIGNED_OWNER_MISMATCH", self._codes())

    def test_duplicate_assignment_is_reported(self) -> None:
        self.payload["lanes"][0]["allowedEnvelopes"].append("FH-M01-SCANNER-RESULT")
        self._write()
        self.assertIn("ENVELOPE_ASSIGNED_MULTIPLE", self._codes())

    def test_missing_prompt_claim_is_reported(self) -> None:
        self.payload["lanes"][0]["prompt"] = self.payload["lanes"][0]["prompt"].replace("claim", "take")
        self._write()
        self.assertIn("PROMPT_CLAIM_MISSING", self._codes())

    def test_undeclared_start_dependency_is_reported(self) -> None:
        story = next(lane for lane in self.payload["lanes"] if lane["id"] == "story-ship")
        story["blockedBy"] = []
        self._write()
        self.assertIn("START_DEPENDENCY_UNDECLARED", self._codes())

    def test_missing_required_read_is_reported(self) -> None:
        raw = self.payload["requiredGlobalReads"][0]
        (self.root / raw).unlink()
        self.assertIn("REQUIRED_READ_MISSING", self._codes())

    def test_report_only_and_strict(self) -> None:
        self.payload["lanes"][0]["startEnvelope"] = "NOPE"
        self._write()
        report = self.root / "Builds/Reports/first_hour_launch_report.json"
        normal = first_hour_launch_gate.main(["--root", str(self.root), "--manifest", str(self.path), "--json-report", str(report)])
        strict = first_hour_launch_gate.main(["--root", str(self.root), "--manifest", str(self.path), "--strict"])
        self.assertEqual(first_hour_launch_gate.EXIT_OK, normal)
        self.assertEqual(first_hour_launch_gate.EXIT_VALIDATION_FAILED, strict)
        self.assertEqual("warning", json.loads(report.read_text())["status"])


if __name__ == "__main__":
    unittest.main()
