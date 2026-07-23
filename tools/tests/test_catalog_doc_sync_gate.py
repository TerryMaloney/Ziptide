from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import catalog_doc_sync_gate


class CatalogDocSyncGateTests(unittest.TestCase):
    def test_repository_registry_has_no_blocking(self) -> None:
        # The real registry: every design catalog registered, every doc exists.
        blocking, _ = catalog_doc_sync_gate.evaluate()
        self.assertEqual(blocking, [], f"unexpected blocking findings: {blocking}")

    def test_missing_doc_blocks(self) -> None:
        with tempfile.TemporaryDirectory() as d:
            reg = Path(d) / "reg.json"
            reg.write_text(json.dumps({
                "trackedGlobs": [],
                "staleWarnDays": 45,
                "entries": [{
                    "catalog": "docs/design/space_mission_catalog.json",
                    "doc": "docs/design/DOES_NOT_EXIST.md",
                    "owner": "test",
                    "lastReconciled": "2026-07-23",
                }],
            }), encoding="utf-8")
            blocking, _ = catalog_doc_sync_gate.evaluate(str(reg))
            self.assertTrue(any("DOC_FILE_MISSING" in b for b in blocking))

    def test_bad_date_blocks(self) -> None:
        with tempfile.TemporaryDirectory() as d:
            reg = Path(d) / "reg.json"
            reg.write_text(json.dumps({
                "trackedGlobs": [],
                "staleWarnDays": 45,
                "entries": [{
                    "catalog": "docs/design/space_mission_catalog.json",
                    "doc": "docs/design/SPACE_MISSION_TYPES.md",
                    "owner": "test",
                    "lastReconciled": "not-a-date",
                }],
            }), encoding="utf-8")
            blocking, _ = catalog_doc_sync_gate.evaluate(str(reg))
            self.assertTrue(any("BAD_LASTRECONCILED" in b for b in blocking))


if __name__ == "__main__":
    unittest.main()
