from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_claim_audit import build_report, scan_claims


class RecoveryClaimAuditTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)

    def tearDown(self) -> None:
        self.temp.cleanup()

    def test_finds_strong_claims_and_qualifiers(self) -> None:
        path = self.root / "status.md"
        path.write_text(
            "✅ SHIPPED — CI green, APK run 12345678\n"
            "Ordinary planning text\n"
            "COMPLETE\n",
            encoding="utf-8",
        )
        claims, missing = scan_claims(self.root, ("status.md",))
        self.assertEqual([], missing)
        self.assertEqual(2, len(claims))
        self.assertIn("CI", claims[0].qualifiers)
        self.assertIn("APK", claims[0].qualifiers)
        self.assertTrue(claims[1].unqualified)

    def test_reports_missing_documents(self) -> None:
        claims, missing = scan_claims(self.root, ("missing.md",))
        self.assertEqual([], claims)
        self.assertEqual(["missing.md"], missing)

    def test_report_counts_unqualified_claims(self) -> None:
        path = self.root / "status.md"
        path.write_text("DONE\nVERIFIED on Quest\n", encoding="utf-8")
        claims, missing = scan_claims(self.root, ("status.md",))
        report = build_report(claims, missing)
        self.assertEqual(2, report["claimCount"])
        self.assertEqual(1, report["unqualifiedCount"])
        self.assertEqual(1, report["qualifierCounts"]["QUEST"])


if __name__ == "__main__":
    unittest.main()
