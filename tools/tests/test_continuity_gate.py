from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import continuity_gate


class ContinuityGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self._touch("CLAUDE.md")
        self._touch("docs/OPERATOR_START_HERE.md")
        self._touch("docs/PRIORITIES.md")
        self._touch("docs/HANDOFF.md")
        self._touch("docs/TERRY_RUNBOOK.md")
        self._touch("docs/SPRINT.md")
        self._touch("docs/SPRINT_MULTIPLAYER.md")
        self._touch("docs/SPRINT_ART.md")
        self._touch("docs/SPRINT_ARCHITECTURE.md")
        self._touch("Ziptide/Assets/Ziptide/Visuals/.keep")
        self._touch("Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs")
        self.manifest_path = self.root / "docs/continuity/project_manifest.json"
        self.manifest = {
            "schemaVersion": 1,
            "branchOfTruth": "terry-local-wip",
            "requiredDocuments": [
                "CLAUDE.md",
                "docs/OPERATOR_START_HERE.md",
                "docs/PRIORITIES.md",
                "docs/HANDOFF.md",
                "docs/TERRY_RUNBOOK.md",
            ],
            "sharedPaths": ["docs/PRIORITIES.md", "docs/HANDOFF.md"],
            "lanes": [
                {"id": "story-ship", "board": "docs/SPRINT.md", "ownedPathPatterns": []},
                {
                    "id": "multiplayer",
                    "board": "docs/SPRINT_MULTIPLAYER.md",
                    "ownedPathPatterns": [],
                },
                {
                    "id": "art",
                    "board": "docs/SPRINT_ART.md",
                    "ownedPathPatterns": ["Ziptide/Assets/Ziptide/Visuals/**"],
                },
                {
                    "id": "architecture",
                    "board": "docs/SPRINT_ARCHITECTURE.md",
                    "ownedPathPatterns": [],
                },
            ],
            "protectedContracts": [
                {
                    "id": "travel",
                    "version": 1,
                    "decisionDoc": "CLAUDE.md",
                    "paths": [
                        "Ziptide/Assets/Ziptide/Gameplay/Runtime/World/TravelCoordinator.cs"
                    ],
                }
            ],
        }
        self._write_manifest()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _touch(self, relative_path: str) -> None:
        path = self.root / relative_path
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text("fixture\n", encoding="utf-8")

    def _write_manifest(self) -> None:
        self.manifest_path.parent.mkdir(parents=True, exist_ok=True)
        self.manifest_path.write_text(json.dumps(self.manifest), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = continuity_gate.validate_manifest(self.root, self.manifest_path)
        return {finding.code for finding in result.findings}

    def test_valid_manifest_has_no_findings(self) -> None:
        result = continuity_gate.validate_manifest(self.root, self.manifest_path)
        self.assertEqual((), result.findings)
        self.assertEqual("pass", result.status)

    def test_missing_required_document_is_reported(self) -> None:
        (self.root / "docs/PRIORITIES.md").unlink()
        self.assertIn("REQUIRED_PATH_MISSING", self._codes())

    def test_path_escape_is_rejected(self) -> None:
        self.manifest["requiredDocuments"].append("../outside.md")
        self._write_manifest()
        self.assertIn("PATH_OUTSIDE_ROOT", self._codes())

    def test_duplicate_lane_id_is_reported(self) -> None:
        self.manifest["lanes"][1]["id"] = "story-ship"
        self._write_manifest()
        self.assertIn("LANE_ID_DUPLICATE", self._codes())

    def test_invalid_contract_version_is_reported(self) -> None:
        self.manifest["protectedContracts"][0]["version"] = 0
        self._write_manifest()
        self.assertIn("CONTRACT_VERSION_INVALID", self._codes())

    def test_report_only_succeeds_but_strict_fails(self) -> None:
        (self.root / "docs/PRIORITIES.md").unlink()
        report_path = self.root / "Builds/Reports/continuity_report.json"

        report_only = continuity_gate.main(
            [
                "--root",
                str(self.root),
                "--manifest",
                str(self.manifest_path),
                "--json-report",
                str(report_path),
            ]
        )
        strict = continuity_gate.main(
            [
                "--root",
                str(self.root),
                "--manifest",
                str(self.manifest_path),
                "--strict",
            ]
        )

        self.assertEqual(continuity_gate.EXIT_OK, report_only)
        self.assertEqual(continuity_gate.EXIT_VALIDATION_FAILED, strict)
        report = json.loads(report_path.read_text(encoding="utf-8"))
        self.assertEqual("warning", report["status"])
        self.assertGreater(report["findingCount"], 0)


if __name__ == "__main__":
    unittest.main()
