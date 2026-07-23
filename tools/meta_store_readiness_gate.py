#!/usr/bin/env python3
"""Validate ZIPTIDE's Meta Store release-readiness contract.

Development mode validates structure and truthfulness while allowing an explicit release hold.
--require-release-ready converts every open required item into a blocking result.
"""
from __future__ import annotations

import argparse
import json
import sys
from dataclasses import asdict, dataclass
from datetime import date, datetime, timezone
from pathlib import Path
from typing import Any, Sequence
from urllib.parse import urlparse

SCHEMA_VERSION = 1
UNKNOWN = "verification-required"
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
ALLOWED_STATUSES = {
    "not-started", "in-progress", "owner-input-required", "decision-required",
    "blocked", "verified", "waived", "not-applicable"
}
OPEN_STATUSES = ALLOWED_STATUSES - {"verified", "waived", "not-applicable"}

@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    severity: str
    requirement_id: str = ""
    path: str = ""

@dataclass(frozen=True)
class Result:
    manifest_path: str
    requirement_count: int
    release_hold_count: int
    findings: tuple[Finding, ...]

    @property
    def blockers(self) -> tuple[Finding, ...]:
        return tuple(x for x in self.findings if x.severity == "blocker")

    @property
    def warnings(self) -> tuple[Finding, ...]:
        return tuple(x for x in self.findings if x.severity == "warning")

    @property
    def status(self) -> str:
        if self.blockers:
            return "fail"
        if self.release_hold_count or self.warnings:
            return "hold"
        return "pass"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "meta_store_readiness_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "manifestPath": self.manifest_path,
            "requirementCount": self.requirement_count,
            "releaseHoldCount": self.release_hold_count,
            "blockerCount": len(self.blockers),
            "warningCount": len(self.warnings),
            "findings": [asdict(x) for x in self.findings],
        }

class GateError(RuntimeError):
    pass

def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())

def _https(value: Any) -> bool:
    if not _text(value) or value == UNKNOWN:
        return False
    parsed = urlparse(value)
    return parsed.scheme == "https" and bool(parsed.netloc)

def _safe_existing_path(root: Path, raw: Any) -> bool:
    if not _text(raw):
        return False
    candidate = Path(raw)
    if candidate.is_absolute():
        return False
    resolved = (root / candidate).resolve()
    try:
        resolved.relative_to(root.resolve())
    except ValueError:
        return False
    return resolved.exists()

def _load(path: Path) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise GateError(str(exc)) from exc
    except json.JSONDecodeError as exc:
        raise GateError(f"invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}") from exc
    if not isinstance(data, dict):
        raise GateError("manifest root must be an object")
    return data

def validate(root: Path, manifest_path: Path, *, require_release_ready: bool = False, today: date | None = None) -> Result:
    root = root.resolve()
    manifest_path = manifest_path.resolve()
    data = _load(manifest_path)
    findings: list[Finding] = []
    today = today or date.today()

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1", "blocker"))
    if not _text(data.get("contractId")):
        findings.append(Finding("CONTRACT_ID_MISSING", "contractId is required", "blocker"))

    policy = data.get("policy")
    if not isinstance(policy, dict):
        findings.append(Finding("POLICY_INVALID", "policy must be an object", "blocker"))
    elif set(policy.get("allowedStatuses", [])) != ALLOWED_STATUSES:
        findings.append(Finding("STATUS_POLICY_DRIFT", "allowedStatuses must match the gate's complete status vocabulary", "blocker"))

    reviewed = data.get("reviewedAgainst")
    if not isinstance(reviewed, dict) or not _safe_existing_path(root, reviewed.get("document")):
        findings.append(Finding("REVIEW_SOURCE_INVALID", "reviewedAgainst.document must reference an existing repository file", "blocker"))

    candidate = data.get("releaseCandidate")
    if not isinstance(candidate, dict) or candidate.get("status") not in {"hold", "ready"}:
        findings.append(Finding("CANDIDATE_STATUS_INVALID", "releaseCandidate.status must be hold or ready", "blocker"))

    requirements = data.get("requirements")
    if not isinstance(requirements, list) or not requirements:
        findings.append(Finding("REQUIREMENTS_INVALID", "requirements must be a non-empty array", "blocker"))
        requirements = []

    seen: set[str] = set()
    hold_count = 0
    for item in requirements:
        if not isinstance(item, dict):
            findings.append(Finding("REQUIREMENT_INVALID", "each requirement must be an object", "blocker"))
            continue
        rid = item.get("id") if _text(item.get("id")) else ""
        if not rid:
            findings.append(Finding("REQUIREMENT_ID_MISSING", "requirement id is required", "blocker"))
            continue
        if rid in seen:
            findings.append(Finding("REQUIREMENT_ID_DUPLICATE", f"duplicate requirement id {rid}", "blocker", rid))
        seen.add(rid)
        for field in ("category", "title", "owner", "nextAction"):
            if not _text(item.get(field)):
                findings.append(Finding("REQUIREMENT_FIELD_MISSING", f"{field} is required", "blocker", rid))
        if item.get("required") is not True:
            findings.append(Finding("REQUIREMENT_NOT_REQUIRED", "all entries in this release contract must be required=true", "blocker", rid))
        status = item.get("status")
        if status not in ALLOWED_STATUSES:
            findings.append(Finding("STATUS_INVALID", f"invalid status {status!r}", "blocker", rid))
            continue
        evidence = item.get("evidence")
        if not isinstance(evidence, list):
            findings.append(Finding("EVIDENCE_INVALID", "evidence must be an array", "blocker", rid))
            evidence = []
        for path in evidence:
            if not _safe_existing_path(root, path):
                findings.append(Finding("EVIDENCE_PATH_MISSING", f"evidence path does not exist: {path}", "blocker", rid, str(path)))
        vrc_ids = item.get("vrcIds")
        if not isinstance(vrc_ids, list) or any(not _text(x) for x in vrc_ids):
            findings.append(Finding("VRC_IDS_INVALID", "vrcIds must be an array of non-empty strings", "blocker", rid))

        if "url" in item and status == "verified" and not _https(item.get("url")):
            findings.append(Finding("VERIFIED_URL_INVALID", "verified URL requirements must contain an HTTPS URL", "blocker", rid))
        if status == "verified":
            if not evidence:
                findings.append(Finding("VERIFIED_WITHOUT_EVIDENCE", "verified requirements need durable evidence", "blocker", rid))
            if not _text(item.get("verifiedAt")):
                findings.append(Finding("VERIFIED_AT_MISSING", "verified requirements need verifiedAt", "blocker", rid))
        elif status == "waived":
            if not _text(item.get("waiverReason")) or not _text(item.get("approvedBy")):
                findings.append(Finding("WAIVER_INCOMPLETE", "waived requirements need waiverReason and approvedBy", "blocker", rid))
        elif status == "not-applicable":
            if not _text(item.get("applicabilityReason")):
                findings.append(Finding("N_A_REASON_MISSING", "not-applicable requirements need applicabilityReason", "blocker", rid))
        else:
            hold_count += 1
            severity = "blocker" if require_release_ready else "warning"
            findings.append(Finding("RELEASE_REQUIREMENT_OPEN", f"required item remains {status}", severity, rid))

        renewal = item.get("renewal")
        if renewal is not None:
            if not isinstance(renewal, dict) or renewal.get("cadence") != "annual":
                findings.append(Finding("RENEWAL_INVALID", "renewal must declare annual cadence", "blocker", rid))
            elif status == "verified":
                raw_due = renewal.get("nextDue")
                try:
                    due = date.fromisoformat(raw_due)
                except (TypeError, ValueError):
                    findings.append(Finding("RENEWAL_DUE_INVALID", "verified annual requirements need an ISO nextDue date", "blocker", rid))
                else:
                    if due <= today:
                        findings.append(Finding("RENEWAL_OVERDUE", f"annual renewal due {due.isoformat()}", "blocker", rid))

    if isinstance(candidate, dict) and candidate.get("status") == "ready" and hold_count:
        findings.append(Finding("FALSE_READY_CLAIM", "releaseCandidate cannot be ready while required items are open", "blocker"))
    if require_release_ready and isinstance(candidate, dict) and candidate.get("status") != "ready":
        findings.append(Finding("CANDIDATE_ON_HOLD", "releaseCandidate.status must be ready in release mode", "blocker"))

    return Result(manifest_path.relative_to(root).as_posix(), len(requirements), hold_count, tuple(findings))

def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--manifest", type=Path, default=Path("docs/store_readiness/meta_store_release_contract.json"))
    parser.add_argument("--json-report", type=Path)
    parser.add_argument("--require-release-ready", action="store_true")
    args = parser.parse_args(argv)
    root = args.root.resolve()
    manifest = args.manifest if args.manifest.is_absolute() else root / args.manifest
    try:
        result = validate(root, manifest, require_release_ready=args.require_release_ready)
    except (GateError, OSError, UnicodeError) as exc:
        print(f"META_STORE_READINESS_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR
    if args.json_report:
        out = args.json_report if args.json_report.is_absolute() else root / args.json_report
        out.parent.mkdir(parents=True, exist_ok=True)
        out.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")
    for finding in result.findings:
        stream = sys.stderr if finding.severity == "blocker" else sys.stdout
        print(f"{finding.severity.upper()} {finding.code} {finding.requirement_id}: {finding.message}", file=stream)
    print(f"META_STORE_READINESS status={result.status} requirements={result.requirement_count} holds={result.release_hold_count} blockers={len(result.blockers)} warnings={len(result.warnings)}")
    return EXIT_VALIDATION_FAILED if result.blockers else EXIT_OK

if __name__ == "__main__":
    raise SystemExit(main())
