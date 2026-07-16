from __future__ import annotations

import hashlib
import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_evidence_audit import (
    FALLBACK_STEMS,
    PERFORMANCE_STEMS,
    RUNTIME_ARTIFACT_STEMS,
    SNAPSHOT_STEMS,
    UI_STEMS,
    audit_artifact,
)


class RecoveryEvidenceAuditTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.sha = "a" * 40
        self._build_base_bundle()

    def tearDown(self) -> None:
        self.temp.cleanup()

    def test_green_raw_bundle_passes_without_optional_gates(self) -> None:
        report = audit_artifact(self.root, self.sha, minimum_tests=38)
        self.assertEqual("GREEN", report["outcome"])
        self.assertEqual(0, report["findingCount"])
        self.assertEqual(38, report["nunit"]["passed"])

    def test_sha_mismatch_and_png_tamper_are_blockers(self) -> None:
        png = (
            self.root
            / "playmode-test-results"
            / "recovery-snapshots"
            / f"{SNAPSHOT_STEMS[0]}.png"
        )
        png.write_bytes(b"tampered")
        report = audit_artifact(self.root, "b" * 40, minimum_tests=38)
        codes = {finding["code"] for finding in report["findings"]}
        self.assertEqual("RED", report["outcome"])
        self.assertIn("TESTED_SHA_MISMATCH", codes)
        self.assertIn("SNAPSHOT_HASH_MISMATCH", codes)
        self.assertIn("SNAPSHOT_SIZE_MISMATCH", codes)

    def test_optional_fallback_and_performance_gates_require_raw_artifacts(self) -> None:
        report = audit_artifact(
            self.root,
            self.sha,
            minimum_tests=38,
            require_fallback=True,
            require_performance=True,
        )
        missing = [
            finding
            for finding in report["findings"]
            if finding["code"] == "ARTIFACT_MISSING"
        ]
        self.assertEqual(len(FALLBACK_STEMS) + len(PERFORMANCE_STEMS), len(missing))
        self.assertEqual("RED", report["outcome"])

    def _build_base_bundle(self) -> None:
        results = self.root / "playmode-test-results"
        results.mkdir(parents=True)
        (results / "recovery-tested-sha.txt").write_text(self.sha + "\n", encoding="utf-8")
        (results / "playmode-results.xml").write_text(
            '<test-run total="38" passed="38" failed="0" skipped="0" inconclusive="0" />\n',
            encoding="utf-8",
        )

        snapshots = results / "recovery-snapshots"
        snapshots.mkdir()
        for index, stem in enumerate(SNAPSHOT_STEMS):
            raw = (f"fake-png-{index}-" * 200).encode("utf-8")
            (snapshots / f"{stem}.png").write_bytes(raw)
            (snapshots / f"{stem}.json").write_text(
                json.dumps(
                    {
                        "pngSha256": hashlib.sha256(raw).hexdigest(),
                        "pngBytes": len(raw),
                        "quantizedColorCount": 32,
                        "dynamicRange": 0.5,
                        "nearBlackRatio": 0.1,
                        "nearWhiteRatio": 0.1,
                        "transparentRatio": 0.0,
                    }
                ),
                encoding="utf-8",
            )

        ui = results / "recovery-ui-spatial"
        ui.mkdir()
        for stem in UI_STEMS:
            (ui / f"{stem}.json").write_text(
                json.dumps({"findings": []}), encoding="utf-8"
            )

        census = results / "recovery-census"
        census.mkdir()
        for stem in RUNTIME_ARTIFACT_STEMS:
            (census / f"{stem}.runtime-artifacts.json").write_text(
                json.dumps({"findings": []}), encoding="utf-8"
            )


if __name__ == "__main__":
    unittest.main()
