from __future__ import annotations

import copy
import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import third_party_license_gate


class ThirdPartyLicenseGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        self.manifest_path = self.root / "docs/licensing/third_party_manifest.json"
        self.credits_path = self.root / "docs/CREDITS.md"
        self.packages_path = self.root / "Ziptide/Packages/manifest.json"
        self.external_path = self.root / "Ziptide/Assets/Photon"
        self.evidence_path = self.root / "docs/licensing/photon-license.txt"

        self.external_path.mkdir(parents=True)
        self.credits_path.parent.mkdir(parents=True)
        self.packages_path.parent.mkdir(parents=True)
        self.evidence_path.parent.mkdir(parents=True, exist_ok=True)
        self.evidence_path.write_text("verified license evidence\n", encoding="utf-8")
        self.packages_path.write_text(
            json.dumps({"dependencies": {"com.unity.inputsystem": "1.6.3"}}),
            encoding="utf-8",
        )
        self.credits_path.write_text(
            "Machine authority: `docs/licensing/third_party_manifest.json`\n\n"
            "### photon-unity-sdk\nVerified test entry.\n",
            encoding="utf-8",
        )
        self.manifest = {
            "schemaVersion": 1,
            "policy": {
                "creditsPath": "docs/CREDITS.md",
                "packagesManifest": "Ziptide/Packages/manifest.json",
                "unknownValue": "verification-required",
                "approvedDistributionStatus": "approved",
                "unityPackagePrefix": "com.unity.",
            },
            "knownExternalAssetRoots": ["Ziptide/Assets/Photon"],
            "entries": [self._approved_entry()],
        }
        self._write_manifest()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _approved_entry(self) -> dict:
        return {
            "id": "photon-unity-sdk",
            "creditsAnchor": "photon-unity-sdk",
            "category": "networking-sdk",
            "provider": "Photon Engine / Exit Games",
            "repositoryPath": "Ziptide/Assets/Photon",
            "packageId": "",
            "owner": "multiplayer",
            "exactVersion": "1.2.3-test",
            "sourceRecord": "test-fixture",
            "licenseId": "test-license",
            "licenseEvidencePath": "docs/licensing/photon-license.txt",
            "obligations": "test-only",
            "distributionStatus": "approved",
            "shipsInCurrentRecoveryCandidate": False,
            "notes": "fixture",
        }

    def _write_manifest(self) -> None:
        self.manifest_path.parent.mkdir(parents=True, exist_ok=True)
        self.manifest_path.write_text(json.dumps(self.manifest), encoding="utf-8")

    def _result(self) -> third_party_license_gate.ValidationResult:
        return third_party_license_gate.validate_manifest(self.root, self.manifest_path)

    def _codes(self) -> set[str]:
        return {finding.code for finding in self._result().findings}

    def test_approved_complete_entry_passes(self) -> None:
        result = self._result()
        self.assertEqual("pass", result.status)
        self.assertEqual((), result.findings)

    def test_production_manifest_has_no_blockers(self) -> None:
        repo_root = Path(__file__).resolve().parents[2]
        result = third_party_license_gate.validate_manifest(
            repo_root,
            repo_root / "docs/licensing/third_party_manifest.json",
        )
        self.assertEqual((), result.blockers)
        self.assertEqual("warning", result.status)
        self.assertIn("LICENSE_RESEARCH_PENDING", {item.code for item in result.warnings})
        self.assertIn("DISTRIBUTION_ON_HOLD", {item.code for item in result.warnings})

    def test_existing_known_root_must_be_declared(self) -> None:
        self.manifest["entries"] = []
        self._write_manifest()
        self.assertIn("KNOWN_ROOT_UNDECLARED", self._codes())

    def test_credits_anchor_is_required(self) -> None:
        self.credits_path.write_text(
            "Machine authority: `docs/licensing/third_party_manifest.json`\n",
            encoding="utf-8",
        )
        self.assertIn("CREDITS_ANCHOR_MISSING", self._codes())

    def test_non_unity_package_requires_entry(self) -> None:
        self.packages_path.write_text(
            json.dumps(
                {
                    "dependencies": {
                        "com.unity.inputsystem": "1.6.3",
                        "com.vendor.external": "2.0.0",
                    }
                }
            ),
            encoding="utf-8",
        )
        self.assertIn("NON_UNITY_PACKAGE_UNDECLARED", self._codes())

    def test_approved_entry_cannot_keep_unknown_terms(self) -> None:
        self.manifest["entries"][0]["licenseId"] = "verification-required"
        self._write_manifest()
        codes = self._codes()
        self.assertIn("LICENSE_RESEARCH_PENDING", codes)
        self.assertIn("APPROVED_WITH_UNKNOWN_TERMS", codes)

    def test_unapproved_content_cannot_claim_recovery_distribution(self) -> None:
        entry = self.manifest["entries"][0]
        entry["distributionStatus"] = "hold-verification"
        entry["shipsInCurrentRecoveryCandidate"] = True
        self._write_manifest()
        self.assertIn("UNAPPROVED_RECOVERY_DISTRIBUTION", self._codes())

    def test_report_only_allows_known_hold_but_strict_warnings_fails(self) -> None:
        entry = self.manifest["entries"][0]
        for field in (
            "exactVersion",
            "sourceRecord",
            "licenseId",
            "licenseEvidencePath",
            "obligations",
        ):
            entry[field] = "verification-required"
        entry["distributionStatus"] = "hold-verification"
        self._write_manifest()
        report_path = self.root / "Builds/Reports/third_party_license.json"

        report_only = third_party_license_gate.main(
            [
                "--root",
                str(self.root),
                "--manifest",
                str(self.manifest_path),
                "--json-report",
                str(report_path),
            ]
        )
        strict = third_party_license_gate.main(
            [
                "--root",
                str(self.root),
                "--manifest",
                str(self.manifest_path),
                "--strict-warnings",
            ]
        )

        self.assertEqual(third_party_license_gate.EXIT_OK, report_only)
        self.assertEqual(third_party_license_gate.EXIT_VALIDATION_FAILED, strict)
        report = json.loads(report_path.read_text(encoding="utf-8"))
        self.assertEqual("warning", report["status"])
        self.assertEqual(0, report["blockerCount"])


if __name__ == "__main__":
    unittest.main()
