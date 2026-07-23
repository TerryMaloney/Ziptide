from __future__ import annotations

import json
import sys
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import ci_verdict


class CiVerdictCoordinationPolicyTests(unittest.TestCase):
    def _render(self) -> str:
        return ci_verdict.render(
            repository="TerryMaloney/Ziptide",
            branch="terry-local-wip",
            sha="1234567890abcdef",
            run_id="42",
            run_attempt="1",
            run_url="https://example.invalid/run/42",
            event_name="push",
            workflow_name="CI",
            editmode="success",
            patch_audit="success",
            android="skipped",
            contract_reports="success",
            recorded_at_utc="2026-07-23T00:00:00+00:00",
        )

    def test_rendered_verdict_names_handoff_queue_as_coordination_only(self) -> None:
        rendered = self._render()
        self.assertIn("docs/handoff_queue/**", rendered)
        self.assertIn("coordination entries", rendered)

    def test_json_interpretation_matches_markdown_freshness_rule(self) -> None:
        rendered = self._render()
        payload_text = rendered.split("```json\n", 1)[1].split("\n```", 1)[0]
        payload = json.loads(payload_text)
        current_when = payload["interpretation"]["currentWhen"]
        self.assertIn("docs/handoff_queue/**", current_when)
        self.assertEqual("GREEN", payload["overall"])


if __name__ == "__main__":
    unittest.main()
