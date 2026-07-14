from __future__ import annotations

import sys
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_playmode_promotion import validate_history


def _observation(sha: str, run_id: int, *, unrelated: bool = False, outcome: str = "success") -> dict:
    return {
        "testedSha": sha,
        "workflowRunId": run_id,
        "attempt": 1,
        "outcome": outcome,
        "scope": "R1.1",
        "unityVersion": "2022.3.62f3",
        "changeClass": "unrelated" if unrelated else "baseline",
        "unrelatedDescendant": unrelated,
        "artifact": None
        if outcome != "success"
        else {
            "id": run_id + 1000,
            "name": "recovery-playmode-r1-results",
            "sizeBytes": 100,
            "expired": False,
        },
    }


def _history(observations: list[dict]) -> dict:
    return {
        "schemaVersion": 1,
        "requirements": {
            "minimumSuccessfulRuns": 2,
            "requiresDistinctShas": True,
            "requiresUnrelatedDescendant": True,
            "requiresArtifact": True,
            "unityVersion": "2022.3.62f3",
        },
        "observations": observations,
    }


class RecoveryPlayModePromotionTests(unittest.TestCase):
    def test_promotes_two_distinct_greens_with_unrelated_probe(self) -> None:
        result = validate_history(
            _history([
                _observation("a" * 40, 1),
                _observation("b" * 40, 2, unrelated=True),
            ])
        )
        self.assertTrue(result.promotable)
        self.assertEqual(0, len(result.findings))

    def test_rejects_successes_without_unrelated_probe(self) -> None:
        result = validate_history(
            _history([
                _observation("a" * 40, 1),
                _observation("b" * 40, 2),
            ])
        )
        self.assertFalse(result.promotable)
        self.assertIn("UNRELATED_DESCENDANT_MISSING", {item.code for item in result.findings})

    def test_rejects_missing_artifact(self) -> None:
        first = _observation("a" * 40, 1)
        second = _observation("b" * 40, 2, unrelated=True)
        second["artifact"] = None
        result = validate_history(_history([first, second]))
        self.assertIn("ARTIFACT_MISSING", {item.code for item in result.findings})

    def test_skipped_run_does_not_count(self) -> None:
        result = validate_history(
            _history([
                _observation("a" * 40, 1),
                _observation("b" * 40, 2, unrelated=True, outcome="skipped"),
            ])
        )
        self.assertEqual(1, result.successful_runs)
        self.assertFalse(result.promotable)


if __name__ == "__main__":
    unittest.main()
