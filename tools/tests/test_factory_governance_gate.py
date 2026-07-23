from __future__ import annotations

import json
import sys
import tempfile
import unittest
from datetime import date
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import factory_governance_gate


class FactoryGovernanceGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        (self.root / "docs/recovery").mkdir(parents=True)
        (self.root / "docs/recovery/paused_sprint_lanes.json").write_text(
            json.dumps({"schemaVersion": "1", "paused": []}),
            encoding="utf-8",
        )
        self._write_valid_ledger()
        (self.root / "docs/SPRINT_TEST.md").write_text(
            "# Test board\n| 1 | ⬜ unclaimed | notes |\n",
            encoding="utf-8",
        )

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write_valid_ledger(self) -> None:
        entries = []
        for number in range(1, 11):
            entries.append(
                f"{number}. **WHAT:** item. **FOUND BY:** test. **WHY MISSED:** test. "
                "**CLASS:** test. **SYSTEM CHANGE:** test.\n"
            )
        (self.root / "docs/MISS_LEDGER.md").write_text(
            "# Ledger\n\n## OPEN\n\n" + "".join(entries) + "\n## CLOSED\n",
            encoding="utf-8",
        )

    def _codes(self) -> set[str]:
        return {
            finding.code
            for finding in factory_governance_gate.validate_repository(
                self.root,
                today=date(2026, 7, 23),
            )
        }

    def test_valid_fixture_passes(self) -> None:
        self.assertEqual(set(), self._codes())

    def test_split_system_change_token_fails_fast(self) -> None:
        ledger = (self.root / "docs/MISS_LEDGER.md").read_text(encoding="utf-8")
        ledger = ledger.replace("**SYSTEM CHANGE:**", "**SYSTEM\nCHANGE:**", 1)
        (self.root / "docs/MISS_LEDGER.md").write_text(ledger, encoding="utf-8")
        self.assertIn("MISS_LEDGER_FIELD_MISSING", self._codes())

    def test_stale_yellow_claim_is_reported(self) -> None:
        (self.root / "docs/SPRINT_TEST.md").write_text(
            "# Test board\n| 1 | 🟡 CLAIMED by lane (2026-07-01) | notes |\n",
            encoding="utf-8",
        )
        self.assertIn("SPRINT_CLAIM_STALE", self._codes())

    def test_released_row_is_not_treated_as_stale(self) -> None:
        (self.root / "docs/SPRINT_TEST.md").write_text(
            "# Test board\n| 1 | ⬜ released (2026-07-01) | notes |\n",
            encoding="utf-8",
        )
        self.assertNotIn("SPRINT_CLAIM_STALE", self._codes())

    def test_valid_pause_exempts_named_board(self) -> None:
        (self.root / "docs/SPRINT_TEST.md").write_text(
            "# Test board\n| 1 | 🟡 CLAIMED by lane (2026-07-01) | notes |\n",
            encoding="utf-8",
        )
        (self.root / "docs/recovery/paused_sprint_lanes.json").write_text(
            json.dumps(
                {
                    "schemaVersion": "1",
                    "paused": [
                        {
                            "board": "SPRINT_TEST.md",
                            "pausedSince": "2026-07-10",
                            "reason": "Recovery freeze",
                            "resumeGate": "Authorized headset pass",
                        }
                    ],
                }
            ),
            encoding="utf-8",
        )
        self.assertNotIn("SPRINT_CLAIM_STALE", self._codes())

    def test_current_repository_passes_fast_gate(self) -> None:
        repo_root = Path(__file__).resolve().parents[2]
        findings = factory_governance_gate.validate_repository(
            repo_root,
            today=date(2026, 7, 23),
        )
        self.assertEqual((), findings)


if __name__ == "__main__":
    unittest.main()
