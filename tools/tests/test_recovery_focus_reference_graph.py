from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_focus_reference_graph import scan_repository, scan_text


class RecoveryFocusReferenceGraphTests(unittest.TestCase):
    def test_marks_declaration_and_reference_lines(self) -> None:
        declaration = "namespace X { public class HammerTool { void A(){ HitFromHammer(); } } }"
        refs = scan_text("HammerTool.cs", declaration)
        hammer = [item for item in refs if item.token == "HammerTool"]
        self.assertTrue(hammer)
        self.assertTrue(hammer[0].declaration_file)

    def test_repository_groups_three_focuses(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            source = root / "src"
            source.mkdir()
            (source / "A.cs").write_text(
                "class A { void X(){ JobRuntime r = null; HammerTool h = null; ShipRefit.Apply(null); } }",
                encoding="utf-8",
            )
            report = scan_repository(root, ("src",))
            self.assertIn("repairObjective", report["summaries"])
            self.assertIn("melee", report["summaries"])
            self.assertIn("shipPresentation", report["summaries"])

    def test_duplicate_token_hits_on_one_line_are_deduplicated_per_token(self) -> None:
        refs = scan_text("A.cs", "class A { void X(){ ShipRefit.Apply(x); ShipRefit.Apply(y); } }")
        apply_refs = [item for item in refs if item.token == "ShipRefit.Apply"]
        self.assertEqual(1, len(apply_refs))


if __name__ == "__main__":
    unittest.main()
