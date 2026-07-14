from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_source_resolver import build_type_index, build_report, resolve_systems


class RecoverySourceResolverTests(unittest.TestCase):
    def test_indexes_declared_types(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            source = root / "src"
            source.mkdir()
            (source / "One.cs").write_text(
                "namespace Test { public sealed class CreditsHud {} internal struct SaveState {} }",
                encoding="utf-8",
            )
            index = build_type_index(root, ("src",))
            self.assertEqual(["src/One.cs"], index["CreditsHud"])

    def test_resolver_marks_missing_and_unique_types(self) -> None:
        index = {
            "BootLoader": ["BootLoader.cs"],
            "PlayerRigPersistence": ["PlayerRigPersistence.cs"],
        }
        results = resolve_systems(index)
        boot = next(item for item in results if item.system_id == "BOOT_FLOW" and item.target_type == "BootLoader")
        credits = next(item for item in results if item.system_id == "PERSISTENT_CREDITS_HUD")
        self.assertEqual("RESOLVED", boot.status)
        self.assertEqual("MISSING", credits.status)

    def test_report_counts_statuses(self) -> None:
        index = {"BootLoader": ["A.cs"], "PlayerRigPersistence": ["A.cs", "B.cs"]}
        report = build_report(index, resolve_systems(index))
        self.assertGreater(report["statusCounts"]["RESOLVED"], 0)
        self.assertGreater(report["statusCounts"]["MISSING"], 0)
        self.assertGreater(report["statusCounts"]["AMBIGUOUS"], 0)


if __name__ == "__main__":
    unittest.main()
