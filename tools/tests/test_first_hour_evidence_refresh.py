from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import first_hour_evidence_refresh


class FirstHourEvidenceRefreshTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.inventory_path = self.root / "docs/first_hour/first_hour_bindings.json"
        self.queue_dir = self.root / "docs/first_hour/evidence_refresh_queue"
        self.source_path = self.root / "Ziptide/Assets/Test.cs"
        self.inventory_path.parent.mkdir(parents=True, exist_ok=True)
        self.queue_dir.mkdir(parents=True, exist_ok=True)
        self.source_path.parent.mkdir(parents=True, exist_ok=True)
        self.source_path.write_text("NEW_TOKEN\n", encoding="utf-8")
        self.inventory = {
            "schemaVersion": 1,
            "bindings": [
                {
                    "beatId": "FH_TEST",
                    "evidence": [
                        {
                            "path": "Ziptide/Assets/Test.cs",
                            "tokens": ["OLD_TOKEN", "STABLE_TOKEN"],
                        }
                    ],
                }
            ],
        }
        self._write_inventory()
        self._write_request()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write_inventory(self) -> None:
        self.inventory_path.write_text(
            json.dumps(self.inventory, indent=2) + "\n", encoding="utf-8"
        )

    def _request(self) -> dict:
        return {
            "schemaVersion": 1,
            "changes": [
                {
                    "beatId": "FH_TEST",
                    "evidencePath": "Ziptide/Assets/Test.cs",
                    "oldToken": "OLD_TOKEN",
                    "newToken": "NEW_TOKEN",
                }
            ],
        }

    def _write_request(self, value: dict | None = None) -> Path:
        path = self.queue_dir / "request.json"
        path.write_text(
            json.dumps(value if value is not None else self._request(), indent=2) + "\n",
            encoding="utf-8",
        )
        return path

    def test_apply_replaces_only_requested_token_and_consumes_queue(self) -> None:
        applied = first_hour_evidence_refresh.refresh(
            self.root, self.inventory_path, self.queue_dir, check_only=False
        )
        self.assertEqual(1, len(applied))
        refreshed = json.loads(self.inventory_path.read_text(encoding="utf-8"))
        tokens = refreshed["bindings"][0]["evidence"][0]["tokens"]
        self.assertEqual(["NEW_TOKEN", "STABLE_TOKEN"], tokens)
        self.assertEqual([], list(self.queue_dir.glob("*.json")))

    def test_check_only_validates_without_writing_or_consuming(self) -> None:
        before = self.inventory_path.read_text(encoding="utf-8")
        applied = first_hour_evidence_refresh.refresh(
            self.root, self.inventory_path, self.queue_dir, check_only=True
        )
        self.assertEqual(1, len(applied))
        self.assertEqual(before, self.inventory_path.read_text(encoding="utf-8"))
        self.assertTrue((self.queue_dir / "request.json").is_file())

    def test_old_token_still_in_source_is_rejected(self) -> None:
        self.source_path.write_text("OLD_TOKEN\nNEW_TOKEN\n", encoding="utf-8")
        with self.assertRaisesRegex(first_hour_evidence_refresh.RefreshError, "still exists"):
            first_hour_evidence_refresh.refresh(
                self.root, self.inventory_path, self.queue_dir, check_only=False
            )

    def test_new_token_missing_from_source_is_rejected(self) -> None:
        self.source_path.write_text("SOMETHING_ELSE\n", encoding="utf-8")
        with self.assertRaisesRegex(first_hour_evidence_refresh.RefreshError, "not present"):
            first_hour_evidence_refresh.refresh(
                self.root, self.inventory_path, self.queue_dir, check_only=False
            )

    def test_wrong_beat_is_rejected(self) -> None:
        request = self._request()
        request["changes"][0]["beatId"] = "FH_UNKNOWN"
        self._write_request(request)
        with self.assertRaisesRegex(first_hour_evidence_refresh.RefreshError, "exactly one binding"):
            first_hour_evidence_refresh.refresh(
                self.root, self.inventory_path, self.queue_dir, check_only=False
            )

    def test_wrong_evidence_path_is_rejected(self) -> None:
        request = self._request()
        request["changes"][0]["evidencePath"] = "Ziptide/Assets/Other.cs"
        self._write_request(request)
        with self.assertRaisesRegex(first_hour_evidence_refresh.RefreshError, "evidence entry"):
            first_hour_evidence_refresh.refresh(
                self.root, self.inventory_path, self.queue_dir, check_only=False
            )

    def test_duplicate_request_is_rejected(self) -> None:
        request = self._request()
        request["changes"].append(dict(request["changes"][0]))
        self._write_request(request)
        with self.assertRaisesRegex(first_hour_evidence_refresh.RefreshError, "duplicate change"):
            first_hour_evidence_refresh.refresh(
                self.root, self.inventory_path, self.queue_dir, check_only=False
            )

    def test_no_queue_is_rejected(self) -> None:
        for path in self.queue_dir.glob("*.json"):
            path.unlink()
        with self.assertRaisesRegex(first_hour_evidence_refresh.RefreshError, "No refresh request"):
            first_hour_evidence_refresh.refresh(
                self.root, self.inventory_path, self.queue_dir, check_only=False
            )


if __name__ == "__main__":
    unittest.main()
