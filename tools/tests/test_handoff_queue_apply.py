from __future__ import annotations

import tempfile
import unittest
from pathlib import Path

import sys

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import handoff_queue_apply


VALID_ENTRY = """### 2026-07-23 (gpt-test) — queued handoff test

- **Did:** Added a test entry.
- **Next:** Continue safely.
- **Heads-up:** Test only.
- **Commit:** test.
"""


class HandoffQueueApplyTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.handoff = self.root / "docs/HANDOFF.md"
        self.queue = self.root / "docs/handoff_queue"
        self.queue.mkdir(parents=True)
        self.handoff.write_text(
            "# HANDOFF\n\n## ENTRIES — newest first\n\n### 2026-07-22 old\nold\n",
            encoding="utf-8",
        )

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def test_valid_entry_is_prepended_and_consumed(self) -> None:
        queued = self.queue / "20260723_test.md"
        queued.write_text(VALID_ENTRY, encoding="utf-8")

        result = handoff_queue_apply.apply_queue(self.root, self.handoff, self.queue)

        self.assertEqual(("docs/handoff_queue/20260723_test.md",), result.applied)
        self.assertFalse(queued.exists())
        text = self.handoff.read_text(encoding="utf-8")
        self.assertLess(text.index("queued handoff test"), text.index("2026-07-22 old"))

    def test_check_only_does_not_modify_or_consume(self) -> None:
        queued = self.queue / "20260723_test.md"
        queued.write_text(VALID_ENTRY, encoding="utf-8")
        before = self.handoff.read_text(encoding="utf-8")

        result = handoff_queue_apply.apply_queue(
            self.root, self.handoff, self.queue, check_only=True
        )

        self.assertEqual(1, len(result.applied))
        self.assertTrue(queued.exists())
        self.assertEqual(before, self.handoff.read_text(encoding="utf-8"))

    def test_existing_heading_is_skipped_and_queue_is_consumed(self) -> None:
        self.handoff.write_text(
            self.handoff.read_text(encoding="utf-8") + "\n" + VALID_ENTRY,
            encoding="utf-8",
        )
        queued = self.queue / "20260723_test.md"
        queued.write_text(VALID_ENTRY, encoding="utf-8")

        result = handoff_queue_apply.apply_queue(self.root, self.handoff, self.queue)

        self.assertEqual((), result.applied)
        self.assertEqual(("docs/handoff_queue/20260723_test.md",), result.skipped_duplicates)
        self.assertFalse(queued.exists())

    def test_invalid_entry_is_rejected_and_preserved(self) -> None:
        queued = self.queue / "20260723_bad.md"
        queued.write_text(
            "### 2026-07-23 bad\n\n- **Did:** incomplete\n",
            encoding="utf-8",
        )

        result = handoff_queue_apply.apply_queue(self.root, self.handoff, self.queue)

        self.assertTrue(result.findings)
        self.assertTrue(queued.exists())
        self.assertNotIn("incomplete", self.handoff.read_text(encoding="utf-8"))

    def test_multiple_entries_are_newest_filename_first(self) -> None:
        older = VALID_ENTRY.replace("(gpt-test)", "(gpt-a)").replace(
            "queued handoff test", "older queued entry"
        )
        newer = VALID_ENTRY.replace("(gpt-test)", "(gpt-b)").replace(
            "queued handoff test", "newer queued entry"
        )
        (self.queue / "20260723_1000.md").write_text(older, encoding="utf-8")
        (self.queue / "20260723_1100.md").write_text(newer, encoding="utf-8")

        result = handoff_queue_apply.apply_queue(self.root, self.handoff, self.queue)

        self.assertFalse(result.findings)
        text = self.handoff.read_text(encoding="utf-8")
        self.assertLess(text.index("newer queued entry"), text.index("older queued entry"))


if __name__ == "__main__":
    unittest.main()
