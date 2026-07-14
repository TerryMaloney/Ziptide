from __future__ import annotations

import json
import tempfile
import unittest
from pathlib import Path

from recovery_inventory_gate import (
    EXPOSURE_CLASSES,
    PROOF_LEVELS,
    validate_inventory,
)


def _system(system_id: str, source: str) -> dict:
    return {
        "id": system_id,
        "responsibility": "Own a test responsibility",
        "canonicalCandidate": "CanonicalOwner",
        "sourceFiles": [source],
        "startup": "Explicit",
        "persistence": "Scene-local",
        "runtimeCreates": ["object"],
        "state": ["state"],
        "currentProof": ["SOURCE", "CORE"],
        "questStatus": "UNVERIFIED",
        "exposure": "SUPPORT",
        "conflicts": ["none"],
        "nextAction": "Test next",
    }


def _inventory(systems: list[dict]) -> dict:
    return {
        "schemaVersion": 1,
        "status": "R0_IN_PROGRESS",
        "generatedFromBranch": "terry-local-wip",
        "asOfDate": "2026-07-14",
        "proofLevels": list(PROOF_LEVELS),
        "exposureClasses": list(EXPOSURE_CLASSES),
        "systems": systems,
        "knownInventoryGaps": [],
    }


class RecoveryInventoryGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        (self.root / "src").mkdir()
        (self.root / "src" / "Owner.cs").write_text("class Owner {}", encoding="utf-8")
        self.inventory_path = self.root / "inventory.json"

    def tearDown(self) -> None:
        self.temp.cleanup()

    def write(self, data: dict) -> None:
        self.inventory_path.write_text(json.dumps(data), encoding="utf-8")

    def codes(self, data: dict) -> list[str]:
        self.write(data)
        return [finding.code for finding in validate_inventory(self.root, self.inventory_path).findings]

    def test_valid_inventory_passes(self) -> None:
        result = validate_inventory(
            self.root,
            self._write_and_return(_inventory([_system("OWNER", "src/Owner.cs")])),
        )
        self.assertEqual((), result.findings)

    def _write_and_return(self, data: dict) -> Path:
        self.write(data)
        return self.inventory_path

    def test_duplicate_ids_are_reported(self) -> None:
        codes = self.codes(_inventory([
            _system("OWNER", "src/Owner.cs"),
            _system("OWNER", "src/Owner.cs"),
        ]))
        self.assertIn("SYSTEM_ID_DUPLICATE", codes)

    def test_unresolved_and_missing_sources_are_reported(self) -> None:
        systems = [
            _system("PROSE", "runtime files (exact paths to resolve in R0 scan)"),
            _system("MISSING", "src/Missing.cs"),
        ]
        codes = self.codes(_inventory(systems))
        self.assertIn("SOURCE_PATH_UNRESOLVED", codes)
        self.assertIn("SOURCE_PATH_MISSING", codes)

    def test_failed_quest_cannot_be_accepted_quest_proof(self) -> None:
        system = _system("QUEST_OWNER", "src/Owner.cs")
        system["currentProof"].append("QUEST")
        system["questStatus"] = "FAILED_DEVICE_ACCEPTANCE"
        codes = self.codes(_inventory([system]))
        self.assertIn("PROOF_QUEST_CONTRADICTION", codes)

    def test_unresolved_canonical_owner_is_reported(self) -> None:
        system = _system("OWNER", "src/Owner.cs")
        system["canonicalCandidate"] = "Undecided during R0"
        codes = self.codes(_inventory([system]))
        self.assertIn("CANONICAL_OWNER_UNRESOLVED", codes)

    def test_matching_source_glob_is_allowed(self) -> None:
        system = _system("OWNER", "src/*.cs")
        codes = self.codes(_inventory([system]))
        self.assertNotIn("SOURCE_GLOB_EMPTY", codes)


if __name__ == "__main__":
    unittest.main()
