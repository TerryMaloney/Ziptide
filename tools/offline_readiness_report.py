#!/usr/bin/env python3
"""Build one ZIPTIDE readiness report for work possible without Unity or Quest.

Statuses remain intentionally distinct:
- pass/fail: deterministic repository evidence available now;
- warning: report-only drift that remains visible but does not block unrelated work;
- release-hold: development may continue, but the named evidence forbids a release claim;
- awaiting-ci: the current source is newer than the durable Unity/audit verdict;
- awaiting-device: Terry's exact authorized Quest route is the only valid closer.
"""

from __future__ import annotations

import argparse
import json
import re
import subprocess
import sys
import tempfile
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Callable, Sequence

TOOLS_DIR = Path(__file__).resolve().parent
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import celestial_system_gate
import concept_intake_gate
import factory_governance_gate
import gate_lifecycle_gate
import launch_transition_gate
import mk2_room_manifest_gate
import rill_visual_state_gate
import space_mission_catalog_gate
import space_poi_catalog_gate
import space_poi_spawn_catalog_gate

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
SCHEMA_VERSION = 2
JSON_FENCE = re.compile(r"```json\s*(\{.*?\})\s*```", re.DOTALL)
FAIL_SEVERITIES = {"error", "failure", "fatal", "blocker"}
FAIL_STATUSES = {"fail", "failure", "blocked", "red"}


@dataclass(frozen=True)
class CheckResult:
    id: str
    status: str
    finding_count: int
    findings: tuple[dict[str, Any], ...]
    evidence: str


class ReadinessError(RuntimeError):
    pass


def _finding_dict(finding: Any) -> dict[str, Any]:
    if hasattr(finding, "__dataclass_fields__"):
        return asdict(finding)
    if isinstance(finding, dict):
        return dict(finding)
    return {"message": str(finding)}


def _direct_check(check_id: str, result: Any, evidence: str) -> CheckResult:
    findings = tuple(_finding_dict(item) for item in getattr(result, "findings", ()))
    return CheckResult(
        id=check_id,
        status="pass" if not findings else "fail",
        finding_count=len(findings),
        findings=findings,
        evidence=evidence,
    )


def _run_report_tool(
    root: Path,
    *,
    check_id: str,
    script_name: str,
    extra_args: Sequence[str] = (),
    hold_statuses: Sequence[str] = (),
) -> CheckResult:
    script = root / "tools" / script_name
    if not script.is_file():
        return CheckResult(
            id=check_id,
            status="fail",
            finding_count=1,
            findings=({"code": "TOOL_MISSING", "message": f"Missing {script_name}."},),
            evidence=f"tools/{script_name}",
        )

    temp_parent = root / "Builds" / "Reports"
    temp_parent.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(prefix="offline-readiness-", dir=temp_parent) as temp_dir:
        report_path = Path(temp_dir) / f"{check_id}.json"
        completed = subprocess.run(
            [
                sys.executable,
                str(script),
                *extra_args,
                "--json-report",
                str(report_path),
            ],
            cwd=root,
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            check=False,
        )
        if not report_path.is_file():
            return CheckResult(
                id=check_id,
                status="fail",
                finding_count=1,
                findings=(
                    {
                        "code": "REPORT_NOT_WRITTEN",
                        "message": completed.stdout[-2000:],
                        "exitCode": completed.returncode,
                    },
                ),
                evidence=f"tools/{script_name}",
            )
        try:
            payload = json.loads(report_path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError) as exc:
            return CheckResult(
                id=check_id,
                status="fail",
                finding_count=1,
                findings=({"code": "REPORT_INVALID", "message": str(exc)},),
                evidence=f"tools/{script_name}",
            )

    raw_findings = payload.get("findings", [])
    findings = tuple(
        dict(item) if isinstance(item, dict) else {"message": str(item)}
        for item in raw_findings
    )
    operational_failure = completed.returncode not in (0, EXIT_VALIDATION_FAILED)
    severe_finding = any(
        str(finding.get("severity", "warning")).strip().lower() in FAIL_SEVERITIES
        for finding in findings
    )
    payload_status = str(payload.get("status", "pass")).strip().lower()
    normalized_hold_statuses = {
        str(value).strip().lower() for value in hold_statuses if str(value).strip()
    }
    if operational_failure or severe_finding or payload_status in FAIL_STATUSES:
        status = "fail"
    elif payload_status in normalized_hold_statuses:
        status = "release-hold"
    elif findings or payload_status == "warning":
        status = "warning"
    else:
        status = "pass"
    return CheckResult(
        id=check_id,
        status=status,
        finding_count=len(findings),
        findings=findings,
        evidence=f"tools/{script_name}",
    )


def _read_ci_verdict(root: Path, source_sha: str) -> CheckResult:
    relative = Path("docs/CI_VERDICT.md")
    try:
        text = (root / relative).read_text(encoding="utf-8")
    except OSError as exc:
        return CheckResult(
            id="durable_ci",
            status="fail",
            finding_count=1,
            findings=({"code": "CI_VERDICT_MISSING", "message": str(exc)},),
            evidence=relative.as_posix(),
        )

    match = JSON_FENCE.search(text)
    if not match:
        return CheckResult(
            id="durable_ci",
            status="fail",
            finding_count=1,
            findings=({"code": "CI_VERDICT_JSON_MISSING", "message": "No fenced JSON payload."},),
            evidence=relative.as_posix(),
        )
    try:
        payload = json.loads(match.group(1))
    except json.JSONDecodeError as exc:
        return CheckResult(
            id="durable_ci",
            status="fail",
            finding_count=1,
            findings=({"code": "CI_VERDICT_JSON_INVALID", "message": str(exc)},),
            evidence=relative.as_posix(),
        )

    tested_sha = str(payload.get("testedSha", ""))
    overall = str(payload.get("overall", "UNKNOWN")).upper()
    if source_sha and source_sha != "unknown" and tested_sha != source_sha:
        return CheckResult(
            id="durable_ci",
            status="awaiting-ci",
            finding_count=0,
            findings=(),
            evidence=f"{relative.as_posix()} tested={tested_sha or 'missing'} current={source_sha}",
        )
    if overall != "GREEN":
        return CheckResult(
            id="durable_ci",
            status="fail",
            finding_count=1,
            findings=(
                {
                    "code": "CI_NOT_GREEN",
                    "message": f"Durable verdict is {overall} for {tested_sha or 'unknown'}.",
                    "results": payload.get("results", {}),
                },
            ),
            evidence=relative.as_posix(),
        )
    return CheckResult(
        id="durable_ci",
        status="pass",
        finding_count=0,
        findings=(),
        evidence=f"{relative.as_posix()} tested={tested_sha}",
    )


def _git_head(root: Path) -> str:
    completed = subprocess.run(
        ["git", "rev-parse", "HEAD"],
        cwd=root,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.DEVNULL,
        check=False,
    )
    value = completed.stdout.strip()
    return value if completed.returncode == 0 and value else "unknown"


def _deterministic_checks(root: Path) -> list[CheckResult]:
    direct: tuple[tuple[str, Callable[[], Any], str], ...] = (
        (
            "factory_governance",
            lambda: type(
                "GovernanceResult",
                (),
                {"findings": factory_governance_gate.validate_repository(root)},
            )(),
            "tools/factory_governance_gate.py",
        ),
        (
            "concept_intake",
            lambda: concept_intake_gate.validate_manifest(
                root, root / "docs/project_art_plan/concept_intake_manifest.json"
            ),
            "docs/project_art_plan/concept_intake_manifest.json",
        ),
        (
            "space_missions",
            lambda: space_mission_catalog_gate.validate_catalog(
                root / "docs/design/space_mission_catalog.json"
            ),
            "docs/design/space_mission_catalog.json",
        ),
        (
            "celestial_systems",
            lambda: celestial_system_gate.validate_catalog(
                root / "docs/design/celestial_system_catalog.json"
            ),
            "docs/design/celestial_system_catalog.json",
        ),
        (
            "mk2_rooms",
            lambda: mk2_room_manifest_gate.validate_manifest(
                root, root / "docs/project_art_plan/mk2_room_socket_manifest.json"
            ),
            "docs/project_art_plan/mk2_room_socket_manifest.json",
        ),
        (
            "gate_lifecycle",
            lambda: gate_lifecycle_gate.validate_catalog(
                root / "docs/project_art_plan/gate_lifecycle_catalog.json"
            ),
            "docs/project_art_plan/gate_lifecycle_catalog.json",
        ),
        (
            "space_pois",
            lambda: space_poi_catalog_gate.validate_catalog(
                root,
                root / "docs/design/space_poi_catalog.json",
                root / "docs/design/space_mission_catalog.json",
            ),
            "docs/design/space_poi_catalog.json",
        ),
        (
            "space_poi_spawn_packets",
            lambda: space_poi_spawn_catalog_gate.validate_catalog(
                root,
                root / "docs/design/space_poi_spawn_catalog.json",
                root / "docs/design/space_poi_catalog.json",
                root / "docs/design/space_mission_catalog.json",
            ),
            "docs/design/space_poi_spawn_catalog.json",
        ),
        (
            "launch_transition",
            lambda: launch_transition_gate.validate_catalog(
                root,
                root / "docs/design/launch_transition_catalog.json",
                root / "docs/design/celestial_system_catalog.json",
            ),
            "docs/design/launch_transition_catalog.json",
        ),
        (
            "rill_visual_states",
            lambda: rill_visual_state_gate.validate_catalog(
                root, root / "docs/project_art_plan/rill_visual_state_catalog.json"
            ),
            "docs/project_art_plan/rill_visual_state_catalog.json",
        ),
    )
    checks = [_direct_check(check_id, runner(), evidence) for check_id, runner, evidence in direct]
    for check_id, script_name in (
        ("sunrig_contract", "sunrig_contract_gate.py"),
        ("continuity", "continuity_gate.py"),
        ("first_hour_contract", "first_hour_gate.py"),
        ("first_hour_binding", "first_hour_binding_gate.py"),
        ("first_hour_envelope", "first_hour_envelope_gate.py"),
        ("first_hour_launch", "first_hour_launch_gate.py"),
    ):
        checks.append(_run_report_tool(root, check_id=check_id, script_name=script_name))
    checks.append(
        _run_report_tool(
            root,
            check_id="third_party_licensing",
            script_name="third_party_license_gate.py",
            hold_statuses=("warning",),
        )
    )
    checks.append(
        _run_report_tool(
            root,
            check_id="meta_store_readiness",
            script_name="meta_store_readiness_gate.py",
            hold_statuses=("hold",),
        )
    )
    return checks


def build_report(root: Path, *, source_sha: str | None = None) -> dict[str, Any]:
    root = root.resolve()
    if not (root / "docs").is_dir() or not (root / "tools").is_dir():
        raise ReadinessError(f"Not a ZIPTIDE repository root: {root}")
    current_sha = source_sha or _git_head(root)

    checks = _deterministic_checks(root)
    checks.append(_read_ci_verdict(root, current_sha))
    checks.append(
        CheckResult(
            id="headset_recovery_route",
            status="awaiting-device",
            finding_count=0,
            findings=(),
            evidence=(
                "artifact=recovery-golden-apk-c45b1a2… run=29786604008 "
                "card=docs/testing/HEADSET_RETRY_C45B1A2.md"
            ),
        )
    )

    failed = [check.id for check in checks if check.status == "fail"]
    warnings = [check.id for check in checks if check.status == "warning"]
    release_holds = [check.id for check in checks if check.status == "release-hold"]
    awaiting_ci = [check.id for check in checks if check.status == "awaiting-ci"]
    awaiting_device = [check.id for check in checks if check.status == "awaiting-device"]
    if failed:
        overall = "blocked-deterministic"
    elif awaiting_ci and awaiting_device:
        overall = "ready-offline-awaiting-ci-and-device"
    elif awaiting_ci:
        overall = "ready-offline-awaiting-ci"
    elif awaiting_device:
        overall = "ready-offline-awaiting-device"
    elif release_holds:
        overall = "ready-offline-release-held"
    else:
        overall = "ready"

    return {
        "schemaVersion": SCHEMA_VERSION,
        "tool": "offline_readiness_report",
        "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
        "root": root.as_posix(),
        "sourceSha": current_sha,
        "overall": overall,
        "releaseCandidateStatus": "hold" if release_holds else "ready",
        "failedChecks": failed,
        "warningChecks": warnings,
        "releaseHoldChecks": release_holds,
        "awaitingCiChecks": awaiting_ci,
        "awaitingDeviceChecks": awaiting_device,
        "nextBlockingLanes": {
            "deterministic": failed,
            "ci": awaiting_ci,
            "device": awaiting_device,
            "release": release_holds,
        },
        "checks": [asdict(check) for check in checks],
        "backAtComputer": [
            "Run tools/check_dev_capabilities.ps1",
            "Install/test exact recovery-golden-apk-c45b1a2 artifact",
            "Run docs/testing/HEADSET_RETRY_C45B1A2.md twice",
            "Attach checkpoint/logcat evidence for any failure",
        ],
    }


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--output", type=Path, default=Path("Builds/Reports/offline_readiness.json")
    )
    parser.add_argument("--source-sha")
    args = parser.parse_args(argv)

    try:
        report = build_report(args.root, source_sha=args.source_sha)
    except (ReadinessError, OSError, ValueError) as exc:
        print(f"OFFLINE_READINESS_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    output = args.output if args.output.is_absolute() else args.root.resolve() / args.output
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(
        "OFFLINE_READINESS_WRITTEN "
        f"overall={report['overall']} release={report['releaseCandidateStatus']} "
        f"source={report['sourceSha']} output={output}"
    )
    return EXIT_VALIDATION_FAILED if report["failedChecks"] else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
