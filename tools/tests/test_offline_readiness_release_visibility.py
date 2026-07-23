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


class OfflineReadinessReleaseVisibilityTests(unittest.TestCase):
    def _write_report_tool(
        self,
        root: Path,
        *,
        status: str,
        findings: list[dict[str, str]],
    ) -> str:
        tools = root / "tools"
        tools.mkdir(parents=True, exist_ok=True)
        script = tools / "fixture_release_report.py"
        payload = {"status": status, "findings": findings}
        script.write_text(
            "import argparse, json\n"
            "from pathlib import Path\n"
            "p=argparse.ArgumentParser()\n"
            "p.add_argument('--json-report', type=Path, required=True)\n"
            "a=p.parse_args()\n"
            f"payload={payload!r}\n"
            "a.json_report.parent.mkdir(parents=True, exist_ok=True)\n"
            "a.json_report.write_text(json.dumps(payload), encoding='utf-8')\n",
            encoding="utf-8",
        )
        return script.name

    def test_declared_hold_status_is_release_hold_not_failure(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            script_name = self._write_report_tool(
                root,
                status="hold",
                findings=[
                    {
                        "code": "OPEN_RELEASE_ITEM",
                        "message": "release evidence remains open",
                        "severity": "warning",
                    }
                ],
            )
            result = offline_readiness_report._run_report_tool(
                root,
                check_id="release",
                script_name=script_name,
                hold_statuses=("hold",),
            )
            self.assertEqual("release-hold", result.status)
            self.assertEqual(1, result.finding_count)

    def test_blocker_overrides_declared_hold_status(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            script_name = self._write_report_tool(
                root,
                status="hold",
                findings=[
                    {
                        "code": "FALSE_READY",
                        "message": "invalid release claim",
                        "severity": "blocker",
                    }
                ],
            )
            result = offline_readiness_report._run_report_tool(
                root,
                check_id="release",
                script_name=script_name,
                hold_statuses=("hold",),
            )
            self.assertEqual("fail", result.status)

    def test_repository_report_exposes_store_and_license_release_holds(self) -> None:
        root = Path(__file__).resolve().parents[2]
        report = offline_readiness_report.build_report(
            root,
            source_sha="deliberately-newer-than-durable-verdict",
        )
        self.assertEqual(2, report["schemaVersion"])
        self.assertEqual("hold", report["releaseCandidateStatus"])
        self.assertIn("meta_store_readiness", report["releaseHoldChecks"])
        self.assertIn("third_party_licensing", report["releaseHoldChecks"])
        self.assertEqual(
            report["releaseHoldChecks"],
            report["nextBlockingLanes"]["release"],
        )
        self.assertIn("durable_ci", report["nextBlockingLanes"]["ci"])
        self.assertEqual(
            ["headset_recovery_route"],
            report["nextBlockingLanes"]["device"],
        )
        self.assertEqual([], report["failedChecks"])

    def test_unified_report_contains_both_authoritative_gate_records(self) -> None:
        root = Path(__file__).resolve().parents[2]
        report = offline_readiness_report.build_report(
            root,
            source_sha="deliberately-newer-than-durable-verdict",
        )
        checks = {item["id"]: item for item in report["checks"]}
        self.assertEqual("release-hold", checks["meta_store_readiness"]["status"])
        self.assertEqual("release-hold", checks["third_party_licensing"]["status"])
        self.assertEqual(
            "tools/meta_store_readiness_gate.py",
            checks["meta_store_readiness"]["evidence"],
        )
        self.assertEqual(
            "tools/third_party_license_gate.py",
            checks["third_party_licensing"]["evidence"],
        )


if __name__ == "__main__":
    unittest.main()
