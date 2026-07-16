#!/usr/bin/env python3
"""Independently audit a ZIPTIDE recovery PlayMode artifact bundle.

This tool never trusts generated status prose. It requires an embedded tested SHA,
parses raw NUnit XML, recomputes screenshot hashes, and inspects the named Golden
runtime/UI/fallback/performance artifacts directly.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import sys
import xml.etree.ElementTree as ET
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Any, Iterable, Sequence

EXIT_OK = 0
EXIT_FAILED = 1

SNAPSHOT_STEMS = (
    "r1_7_actual_home_hub",
    "r1_7_actual_w000_spawn",
    "r1_7_actual_toxic_city_spawn",
)
UI_STEMS = (
    "r1_8_actual_home_hub",
    "r1_8_actual_w000_spawn",
    "r1_8_actual_toxic_city_spawn",
)
RUNTIME_ARTIFACT_STEMS = (
    "r1_5_actual_boot_home_settled",
    "r1_5_actual_boot_after_settings",
    "r1_6_boot_before_new_game",
    "r1_6_w000_first_arrival",
    "r1_6_toxic_city_arrival",
    "r1_6_w000_return",
)
FALLBACK_STEMS = (
    "r1_9_actual_home_hub",
    "r1_9_actual_w000_spawn",
    "r1_9_actual_toxic_city_spawn",
)
PERFORMANCE_STEMS = (
    "r1_10_w000_first_post_sweep",
    "r1_10_toxic_city_post_sweep",
    "r1_10_w000_return_post_sweep",
)


@dataclass(frozen=True)
class Finding:
    severity: str
    code: str
    path: str
    message: str


def _finding(code: str, path: Path | str, message: str) -> Finding:
    return Finding("BLOCKER", code, str(path), message)


def _load_json(path: Path, findings: list[Finding]) -> dict[str, Any] | None:
    if not path.is_file():
        findings.append(_finding("ARTIFACT_MISSING", path, "Required JSON artifact is missing."))
        return None
    try:
        value = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as exc:
        findings.append(_finding("ARTIFACT_JSON_INVALID", path, str(exc)))
        return None
    if not isinstance(value, dict):
        findings.append(_finding("ARTIFACT_JSON_ROOT_INVALID", path, "Expected a JSON object."))
        return None
    return value


def _audit_nunit(root: Path, minimum_tests: int, findings: list[Finding]) -> dict[str, int]:
    path = root / "playmode-test-results" / "playmode-results.xml"
    result = {"total": 0, "passed": 0, "failed": 0, "skipped": 0, "inconclusive": 0}
    if not path.is_file():
        findings.append(_finding("NUNIT_XML_MISSING", path, "Raw PlayMode XML is missing."))
        return result
    try:
        xml_root = ET.parse(path).getroot()
    except (OSError, ET.ParseError) as exc:
        findings.append(_finding("NUNIT_XML_INVALID", path, str(exc)))
        return result
    test_run = xml_root if xml_root.tag == "test-run" else xml_root.find(".//test-run")
    if test_run is None:
        findings.append(_finding("NUNIT_TEST_RUN_MISSING", path, "No test-run element was found."))
        return result
    for key in result:
        try:
            result[key] = int(test_run.attrib.get(key, "0"))
        except ValueError:
            findings.append(_finding("NUNIT_COUNT_INVALID", path, f"Invalid {key} count."))
    if result["total"] < minimum_tests:
        findings.append(_finding(
            "NUNIT_TEST_COUNT_TOO_LOW", path,
            f"Executed {result['total']} tests; expected at least {minimum_tests}."))
    if result["failed"] != 0 or result["passed"] != result["total"]:
        findings.append(_finding(
            "NUNIT_NOT_ALL_GREEN", path,
            f"total={result['total']} passed={result['passed']} failed={result['failed']} "
            f"skipped={result['skipped']} inconclusive={result['inconclusive']}."))
    return result


def _audit_snapshot(root: Path, stem: str, findings: list[Finding]) -> None:
    directory = root / "playmode-test-results" / "recovery-snapshots"
    json_path = directory / f"{stem}.json"
    png_path = directory / f"{stem}.png"
    data = _load_json(json_path, findings)
    if not png_path.is_file():
        findings.append(_finding("SNAPSHOT_PNG_MISSING", png_path, "Required PNG is missing."))
        return
    raw = png_path.read_bytes()
    if data is None:
        return
    expected_hash = str(data.get("pngSha256", "")).lower()
    actual_hash = hashlib.sha256(raw).hexdigest()
    if expected_hash != actual_hash:
        findings.append(_finding(
            "SNAPSHOT_HASH_MISMATCH", png_path,
            f"metadata={expected_hash or 'missing'} actual={actual_hash}."))
    if int(data.get("pngBytes", -1)) != len(raw):
        findings.append(_finding(
            "SNAPSHOT_SIZE_MISMATCH", png_path,
            f"metadata={data.get('pngBytes')} actual={len(raw)}."))
    numeric_rules = (
        ("quantizedColorCount", 4, "greater"),
        ("dynamicRange", 0.01, "greater"),
        ("nearBlackRatio", 0.999, "less"),
        ("nearWhiteRatio", 0.999, "less"),
        ("transparentRatio", 0.10, "less"),
    )
    for key, limit, direction in numeric_rules:
        try:
            value = float(data[key])
        except (KeyError, TypeError, ValueError):
            findings.append(_finding("SNAPSHOT_METRIC_MISSING", json_path, f"Missing/invalid {key}."))
            continue
        bad = value <= limit if direction == "greater" else value >= limit
        if bad:
            findings.append(_finding(
                "SNAPSHOT_METRIC_BLOCKER", json_path,
                f"{key}={value} violates {direction}-than {limit}."))


def _audit_findings_file(path: Path, findings: list[Finding], label: str) -> None:
    data = _load_json(path, findings)
    if data is None:
        return
    values = data.get("findings")
    if not isinstance(values, list):
        findings.append(_finding("FINDINGS_ARRAY_MISSING", path, f"{label} omitted findings array."))
        return
    blockers = [item for item in values if isinstance(item, dict) and item.get("severity") == "BLOCKER"]
    if blockers:
        codes = sorted({str(item.get("code", "UNKNOWN")) for item in blockers})
        findings.append(_finding(
            f"{label}_BLOCKERS", path,
            f"{len(blockers)} blocker(s): {', '.join(codes)}."))


def _audit_runtime_artifact(path: Path, findings: list[Finding]) -> None:
    data = _load_json(path, findings)
    if data is None:
        return
    values = data.get("findings")
    if not isinstance(values, list):
        findings.append(_finding("RUNTIME_FINDINGS_ARRAY_MISSING", path, "Missing findings array."))
    elif values:
        codes = sorted({str(item.get("code", "UNKNOWN")) for item in values if isinstance(item, dict)})
        findings.append(_finding(
            "RUNTIME_ARTIFACT_FINDINGS", path,
            f"{len(values)} runtime finding(s): {', '.join(codes)}."))


def _audit_performance(path: Path, findings: list[Finding]) -> None:
    data = _load_json(path, findings)
    if data is None:
        return
    evidence = str(data.get("healthSweepEvidence", ""))
    if "ZIPTIDE: HEALTH_SWEEP" not in evidence:
        findings.append(_finding("PERFORMANCE_SWEEP_EVIDENCE_MISSING", path, evidence or "missing"))
    if int(data.get("sampledFrames", 0)) < 120:
        findings.append(_finding(
            "PERFORMANCE_FRAME_COUNT_TOO_LOW", path,
            f"sampledFrames={data.get('sampledFrames')}."))
    interpretation = str(data.get("interpretation", ""))
    if "not a Quest device budget" not in interpretation:
        findings.append(_finding(
            "PERFORMANCE_INTERPRETATION_UNSAFE", path,
            "Reference-renderer result is not explicitly separated from Quest proof."))


def audit_artifact(
    artifact_root: Path,
    expected_sha: str,
    minimum_tests: int = 38,
    require_fallback: bool = False,
    require_performance: bool = False,
) -> dict[str, Any]:
    root = artifact_root.resolve()
    findings: list[Finding] = []
    sha_path = root / "playmode-test-results" / "recovery-tested-sha.txt"
    if not sha_path.is_file():
        findings.append(_finding("TESTED_SHA_MISSING", sha_path, "Artifact does not embed its tested SHA."))
        actual_sha = ""
    else:
        actual_sha = sha_path.read_text(encoding="utf-8").strip()
        if actual_sha != expected_sha:
            findings.append(_finding(
                "TESTED_SHA_MISMATCH", sha_path,
                f"expected={expected_sha} actual={actual_sha}."))

    counts = _audit_nunit(root, minimum_tests, findings)
    for stem in SNAPSHOT_STEMS:
        _audit_snapshot(root, stem, findings)
    ui_dir = root / "playmode-test-results" / "recovery-ui-spatial"
    for stem in UI_STEMS:
        _audit_findings_file(ui_dir / f"{stem}.json", findings, "UI")
    census_dir = root / "playmode-test-results" / "recovery-census"
    for stem in RUNTIME_ARTIFACT_STEMS:
        _audit_runtime_artifact(census_dir / f"{stem}.runtime-artifacts.json", findings)
    if require_fallback:
        directory = root / "playmode-test-results" / "recovery-fallback-surfaces"
        for stem in FALLBACK_STEMS:
            _audit_findings_file(directory / f"{stem}.json", findings, "FALLBACK")
    if require_performance:
        directory = root / "playmode-test-results" / "recovery-performance"
        for stem in PERFORMANCE_STEMS:
            _audit_performance(directory / f"{stem}.json", findings)

    return {
        "tool": "recovery_evidence_audit",
        "expectedSha": expected_sha,
        "embeddedSha": actual_sha,
        "artifactRoot": str(root),
        "minimumTests": minimum_tests,
        "nunit": counts,
        "requires": {
            "fallback": require_fallback,
            "performance": require_performance,
        },
        "findingCount": len(findings),
        "findings": [asdict(item) for item in findings],
        "outcome": "GREEN" if not findings else "RED",
    }


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("artifact_root", type=Path)
    parser.add_argument("--expected-sha", required=True)
    parser.add_argument("--minimum-tests", type=int, default=38)
    parser.add_argument("--require-fallback", action="store_true")
    parser.add_argument("--require-performance", action="store_true")
    parser.add_argument("--json-report", type=Path)
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    report = audit_artifact(
        args.artifact_root,
        args.expected_sha,
        args.minimum_tests,
        args.require_fallback,
        args.require_performance,
    )
    output = json.dumps(report, indent=2) + "\n"
    if args.json_report:
        args.json_report.parent.mkdir(parents=True, exist_ok=True)
        args.json_report.write_text(output, encoding="utf-8")
    else:
        print(output, end="")
    print(
        f"RECOVERY_EVIDENCE_AUDIT outcome={report['outcome']} "
        f"findings={report['findingCount']} tests={report['nunit']['total']}",
        file=sys.stderr,
    )
    return EXIT_OK if report["outcome"] == "GREEN" else EXIT_FAILED


if __name__ == "__main__":
    raise SystemExit(main())
