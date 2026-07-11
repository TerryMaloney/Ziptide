from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import ci_verdict


class CiVerdictTests(unittest.TestCase):
    def test_green_for_editmode_success_and_android_skipped(self) -> None:
        verdict = ci_verdict.calculate_verdict("success", "skipped", "success")
        self.assertEqual("GREEN", verdict.overall)

    def test_green_for_full_success(self) -> None:
        verdict = ci_verdict.calculate_verdict("success", "success", "success")
        self.assertEqual("GREEN", verdict.overall)

    def test_red_for_editmode_failure(self) -> None:
        verdict = ci_verdict.calculate_verdict("failure", "skipped", "success")
        self.assertEqual("RED", verdict.overall)

    def test_red_for_android_failure_when_run(self) -> None:
        verdict = ci_verdict.calculate_verdict("success", "failure", "success")
        self.assertEqual("RED", verdict.overall)

    def test_contract_report_is_non_blocking(self) -> None:
        verdict = ci_verdict.calculate_verdict("success", "skipped", "failure")
        self.assertEqual("GREEN", verdict.overall)
        self.assertEqual("failure", verdict.contract_reports)

    def test_unknown_result_fails_closed(self) -> None:
        verdict = ci_verdict.calculate_verdict("banana", "skipped", "success")
        self.assertEqual("RED", verdict.overall)
        self.assertEqual("unknown", verdict.editmode)

    def test_render_contains_machine_readable_payload(self) -> None:
        content = ci_verdict.render(
            repository="TerryMaloney/Ziptide",
            branch="terry-local-wip",
            sha="a" * 40,
            run_id="123",
            run_attempt="2",
            run_url="https://github.com/TerryMaloney/Ziptide/actions/runs/123",
            event_name="push",
            workflow_name="CI",
            editmode="success",
            android="skipped",
            contract_reports="success",
            recorded_at_utc="2026-07-11T12:00:00+00:00",
        )
        payload_text = content.split("```json\n", 1)[1].split("\n```", 1)[0]
        payload = json.loads(payload_text)
        self.assertEqual("GREEN", payload["overall"])
        self.assertEqual("a" * 40, payload["testedSha"])
        self.assertEqual("skipped", payload["results"]["androidApk"])
        self.assertIn("direct generated verdict-only child", payload["interpretation"]["currentWhen"])
        self.assertIn("direct verdict-only child", content)

    def test_cli_rejects_wrong_branch(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            output = Path(tmp) / "verdict.md"
            result = ci_verdict.main([
                "--output", str(output),
                "--repository", "TerryMaloney/Ziptide",
                "--branch", "main",
                "--sha", "a" * 40,
                "--run-id", "123",
                "--run-url", "https://example.invalid/run",
                "--event-name", "push",
                "--editmode-result", "success",
                "--android-result", "skipped",
                "--contract-result", "success",
            ])
            self.assertEqual(ci_verdict.EXIT_INVALID, result)
            self.assertFalse(output.exists())

    def test_cli_writes_deterministic_file_with_explicit_time(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            output = Path(tmp) / "verdict.md"
            args = [
                "--output", str(output),
                "--repository", "TerryMaloney/Ziptide",
                "--branch", "terry-local-wip",
                "--sha", "b" * 40,
                "--run-id", "456",
                "--run-attempt", "1",
                "--run-url", "https://github.com/TerryMaloney/Ziptide/actions/runs/456",
                "--event-name", "push",
                "--editmode-result", "success",
                "--android-result", "skipped",
                "--contract-result", "success",
                "--recorded-at-utc", "2026-07-11T12:00:00+00:00",
            ]
            first = ci_verdict.main(args)
            content_one = output.read_text(encoding="utf-8")
            second = ci_verdict.main(args)
            content_two = output.read_text(encoding="utf-8")
            self.assertEqual(ci_verdict.EXIT_OK, first)
            self.assertEqual(ci_verdict.EXIT_OK, second)
            self.assertEqual(content_one, content_two)


if __name__ == "__main__":
    unittest.main()
