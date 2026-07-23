from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import concept_intake_gate


class ConceptIntakeGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.manifest_path = self.root / "docs/project_art_plan/concept_intake_manifest.json"
        self.spec_path = self.root / "docs/project_art_plan/spec.md"
        self.source_path = self.root / "docs/project_art_plan/source.md"
        self.concept_path = self.root / "concepts/keeper.png"
        self.spec_path.parent.mkdir(parents=True, exist_ok=True)
        self.concept_path.parent.mkdir(parents=True, exist_ok=True)
        self.source_path.write_text("source\n", encoding="utf-8")
        self.concept_path.write_bytes(b"png-fixture")
        self.spec_path.write_text(
            "# ASSET: TEST\n\n## Build acceptance\n\nready\n",
            encoding="utf-8",
        )
        self.manifest = {
            "schemaVersion": 1,
            "assets": [
                {
                    "id": "test_asset",
                    "displayName": "Test Asset",
                    "tier": "B",
                    "status": "intake-ready",
                    "specPath": "docs/project_art_plan/spec.md",
                    "sourceDocs": ["docs/project_art_plan/source.md"],
                    "conceptPaths": ["concepts/keeper.png"],
                    "requiredMarkers": ["# ASSET: TEST", "## Build acceptance"],
                }
            ],
        }
        self._write_manifest()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write_manifest(self) -> None:
        self.manifest_path.parent.mkdir(parents=True, exist_ok=True)
        self.manifest_path.write_text(json.dumps(self.manifest), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = concept_intake_gate.validate_manifest(self.root, self.manifest_path)
        return {finding.code for finding in result.findings}

    def test_valid_intake_passes(self) -> None:
        result = concept_intake_gate.validate_manifest(self.root, self.manifest_path)
        self.assertEqual("pass", result.status)
        self.assertEqual(1, result.asset_count)
        self.assertEqual((), result.findings)

    def test_duplicate_asset_id_is_rejected(self) -> None:
        self.manifest["assets"].append(dict(self.manifest["assets"][0]))
        self._write_manifest()
        self.assertIn("ASSET_ID_DUPLICATE", self._codes())

    def test_missing_concept_file_is_rejected(self) -> None:
        self.concept_path.unlink()
        self.assertIn("REFERENCE_MISSING", self._codes())

    def test_missing_spec_marker_is_rejected(self) -> None:
        self.spec_path.write_text("# ASSET: TEST\n", encoding="utf-8")
        self.assertIn("SPEC_MARKER_MISSING", self._codes())

    def test_intake_ready_requires_build_acceptance(self) -> None:
        self.manifest["assets"][0]["requiredMarkers"] = ["# ASSET: TEST"]
        self.spec_path.write_text("# ASSET: TEST\n", encoding="utf-8")
        self._write_manifest()
        self.assertIn("INTAKE_ACCEPTANCE_MISSING", self._codes())

    def test_unresolved_status_may_use_family_references(self) -> None:
        asset = self.manifest["assets"][0]
        asset["status"] = "spec-ready-concept-unresolved"
        asset["conceptPaths"] = []
        self._write_manifest()
        result = concept_intake_gate.validate_manifest(self.root, self.manifest_path)
        self.assertEqual("pass", result.status)

    def test_path_escape_is_rejected(self) -> None:
        self.manifest["assets"][0]["specPath"] = "../../outside.md"
        self._write_manifest()
        self.assertIn("PATH_OUTSIDE_ROOT", self._codes())

    def test_repository_manifest_is_valid(self) -> None:
        repo_root = Path(__file__).resolve().parents[2]
        manifest = repo_root / "docs/project_art_plan/concept_intake_manifest.json"
        result = concept_intake_gate.validate_manifest(repo_root, manifest)
        self.assertEqual(
            (),
            result.findings,
            "Repository concept intake manifest must remain valid: "
            + "; ".join(f"{finding.code} {finding.message}" for finding in result.findings),
        )
        self.assertGreaterEqual(result.asset_count, 4)


if __name__ == "__main__":
    unittest.main()
