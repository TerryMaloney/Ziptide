from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import offline_readiness_report


class OfflineReadinessReportTests(unittest.TestCase):
    def test_ci_verdict_distinguishes_stale_from_red(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            docs = root / "docs"
            docs.mkdir()
            payload = {
                "overall": "GREEN",
                "testedSha": "old-sha",
                "results": {"unityEditMode": "success"},
            }
            (docs / "CI_VERDICT.md").write_text(
                "# Verdict\n\n```json\n" + json.dumps(payload) + "\n```\n",
                encoding="utf-8",
            )
            stale = offline_readiness_report._read_ci_verdict(root, "new-sha")
            self.assertEqual("awaiting-ci", stale.status)
            self.assertEqual(0, stale.finding_count)

            payload["overall"] = "RED"
            payload["testedSha"] = "new-sha"
            (docs / "CI_VERDICT.md").write_text(
                "# Verdict\n\n```json\n" + json.dumps(payload) + "\n```\n",
                encoding="utf-8",
            )
            red = offline_readiness_report._read_ci_verdict(root, "new-sha")
            self.assertEqual("fail", red.status)
            self.assertEqual("CI_NOT_GREEN", red.findings[0]["code"])

    def test_matching_green_ci_verdict_passes(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            docs = root / "docs"
            docs.mkdir()
            payload = {"overall": "GREEN", "testedSha": "same-sha", "results": {}}
            (docs / "CI_VERDICT.md").write_text(
                "```json\n" + json.dumps(payload) + "\n```\n",
                encoding="utf-8",
            )
            result = offline_readiness_report._read_ci_verdict(root, "same-sha")
            self.assertEqual("pass", result.status)
            self.assertEqual(0, result.finding_count)

    def test_direct_check_preserves_finding_details(self) -> None:
        finding = offline_readiness_report.factory_governance_gate.Finding(
            "CODE", "message", "path"
        )
        result_obj = type("R", (), {"findings": (finding,)})()
        result = offline_readiness_report._direct_check("test", result_obj, "evidence")
        self.assertEqual("fail", result.status)
        self.assertEqual(1, result.finding_count)
        self.assertEqual("CODE", result.findings[0]["code"])

    def test_repository_report_has_no_deterministic_failures(self) -> None:
        root = Path(__file__).resolve().parents[2]
        report = offline_readiness_report.build_report(
            root,
            source_sha="deliberately-newer-than-durable-verdict",
        )
        self.assertEqual([], report["failedChecks"])
        self.assertIn("durable_ci", report["awaitingCiChecks"])
        self.assertEqual(["headset_recovery_route"], report["awaitingDeviceChecks"])
        self.assertEqual("ready-offline-awaiting-ci-and-device", report["overall"])
        ids = {check["id"] for check in report["checks"]}
        for required in (
            "factory_governance",
            "concept_intake",
            "space_missions",
            "celestial_systems",
            "mk2_rooms",
            "continuity",
            "first_hour_contract",
            "first_hour_binding",
            "first_hour_envelope",
            "first_hour_launch",
            "durable_ci",
            "headset_recovery_route",
        ):
            self.assertIn(required, ids)


if __name__ == "__main__":
    unittest.main()
