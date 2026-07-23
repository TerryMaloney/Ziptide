#!/usr/bin/env python3
"""Build one ZIPTIDE report for work that can proceed without Unity or a headset.

The report deliberately distinguishes three states:
- pass/fail: deterministic repository checks that can be resolved now;
- awaiting-ci: latest source has not received a durable Unity/audit verdict yet;
- awaiting-device: only Terry's exact authorized Quest route can close the item.
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
from typing import Any, Sequence

TOOLS_DIR = Path(__file__).resolve().parent
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import celestial_system_gate
import concept_intake_gate
import factory_governance_gate
import mk2_room_manifest_gate
import space_mission_catalog_gate

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
SCHEMA_VERSION = 1
JSON_FENCE = re.compile(r"```json\s*(\{.*?\})\s*```", re.DOTALL)


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

    with tempfile.TemporaryDirectory() as temp_dir:
        report_path = Path(temp_dir) / f"{check_id}.json"
        command = [
            sys.executable,
            str(script),
            *extra_args,
            "--json-report",
            str(report_path),
        ]
        completed = subprocess.run(
            command,
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
    findings = tuple(dict(item) if isinstance(item, dict) else {"message": str(item)} for item in raw_findings)
    failed = completed.returncode not in (0, 2) or bool(findings)
    return CheckResult(
        id=check_id,
        status="fail" if failed else "pass",
        finding_count=len(findings),
        findings=findings,
        evidence=f"tools/{script_name}",
    )


def _read_ci_verdict(root: Path, source_sha: str) -> CheckResult:
    relative = Path("docs/CI_VERDICT.md")
    path = root / relative
    try:
        text = path.read_text(encoding="utf-8")
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
            evidence=(
                f"{relative.as_posix()} tested={tested_sha or 'missing'} current={source_sha}"
            ),
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


def build_report(root: Path, *, source_sha: str | None = None) -> dict[str, Any]:
    root = root.resolve()
    if not (root / "docs").is_dir() or not (root / "tools").is_dir():
        raise ReadinessError(f"Not a ZIPTIDE repository root: {root}")
    current_sha = source_sha or _git_head(root)

    checks: list[CheckResult] = []
    checks.append(
        _direct_check(
            "factory_governance",
            type("R", (), {"findings": factory_governance_gate.validate_repository(root)})(),
            "tools/factory_governance_gate.py",
        )
    )
    checks.append(
        _direct_check(
            "concept_intake",
            concept_intake_gate.validate_manifest(
                root, root / "docs/project_art_plan/concept_intake_manifest.json"
            ),
            "docs/project_art_plan/concept_intake_manifest.json",
        )
    )
    checks.append(
        _direct_check(
            "space_missions",
            space_mission_catalog_gate.validate_catalog(
                root / "docs/design/space_mission_catalog.json"
            ),
            "docs/design/space_mission_catalog.json",
        )
    )
    checks.append(
        _direct_check(
            "celestial_systems",
            celestial_system_gate.validate_catalog(
                root / "docs/design/celestial_system_catalog.json"
            ),
            "docs/design/celestial_system_catalog.json",
        )
    )
    checks.append(
        _direct_check(
            "mk2_rooms",
            mk2_room_manifest_gate.validate_manifest(
                root, root / "docs/project_art_plan/mk2_room_socket_manifest.json"
            ),
            "docs/project_art_plan/mk2_room_socket_manifest.json",
        )
    )

    checks.extend(
        [
            _run_report_tool(
                root,
                check_id="continuity",
                script_name="continuity_gate.py",
            ),
            _run_report_tool(
                root,
                check_id="first_hour_contract",
                script_name="first_hour_gate.py",
            ),
            _run_report_tool(
                root,
                check_id="first_hour_binding",
                script_name="first_hour_binding_gate.py",
            ),
            _run_report_tool(
                root,
                check_id="first_hour_envelope",
                script_name="first_hour_envelope_gate.py",
            ),
            _run_report_tool(
                root,
                check_id="first_hour_launch",
                script_name="first_hour_launch_gate.py",
            ),
        ]
    )
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
    awaiting_ci = [check.id for check in checks if check.status == "awaiting-ci"]
    awaiting_device = [check.id for check in checks if check.status == "awaiting-device"]
    if failed:
        overall = "blocked-deterministic"
    elif awaiting_ci:
        overall = "ready-offline-awaiting-ci-and-device"
    elif awaiting_device:
        overall = "ready-offline-awaiting-device"
    else:
        overall = "ready"

    return {
        "schemaVersion": SCHEMA_VERSION,
        "tool": "offline_readiness_report",
        "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
        "root": root.as_posix(),
        "sourceSha": current_sha,
        "overall": overall,
        "failedChecks": failed,
        "awaitingCiChecks": awaiting_ci,
        "awaitingDeviceChecks": awaiting_device,
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

    output = args.output
    if not output.is_absolute():
        output = args.root.resolve() / output
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(
        "OFFLINE_READINESS_WRITTEN "
        f"overall={report['overall']} source={report['sourceSha']} output={output}"
    )
    return EXIT_VALIDATION_FAILED if report["failedChecks"] else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
