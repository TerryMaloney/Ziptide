#!/usr/bin/env python3
"""Validate the first-hour Opus launch manifest. Default is report-only."""

from __future__ import annotations

import argparse
import json
import sys
from collections import Counter
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Sequence

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    lane_id: str = ""
    path: str = ""


@dataclass(frozen=True)
class Result:
    root: str
    manifest_path: str
    lane_count: int
    assigned_envelopes: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "first_hour_launch_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "manifestPath": self.manifest_path,
            "laneCount": self.lane_count,
            "assignedEnvelopeCount": self.assigned_envelopes,
            "findingCount": len(self.findings),
            "findings": [asdict(item) for item in self.findings],
        }


class LoadError(RuntimeError):
    pass


def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _load(path: Path, label: str) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise LoadError(f"Could not read {label}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise LoadError(f"{label} invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}") from exc
    if not isinstance(data, dict):
        raise LoadError(f"{label} root must be an object.")
    return data


def _strings(value: Any, field: str, findings: list[Finding], lane_id: str = "", required: bool = False) -> list[str]:
    if value is None and not required:
        return []
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list.", lane_id))
        return []
    items = [item.strip() for item in value if _text(item)]
    if len(items) != len(value):
        findings.append(Finding("FIELD_ITEM_INVALID", f"{field} contains an empty item.", lane_id))
    if required and not items:
        findings.append(Finding("FIELD_REQUIRED", f"{field} needs at least one item.", lane_id))
    return items


def _resolve(root: Path, raw: Any, field: str, lane_id: str = "") -> tuple[Path | None, Finding | None]:
    if not _text(raw):
        return None, Finding("PATH_INVALID", f"{field} must be non-empty.", lane_id)
    shown = str(raw).replace("\\", "/").strip()
    path = Path(shown)
    if path.is_absolute():
        return None, Finding("PATH_ABSOLUTE", f"{field} must be relative.", lane_id, shown)
    resolved = (root / path).resolve()
    try:
        resolved.relative_to(root.resolve())
    except ValueError:
        return None, Finding("PATH_OUTSIDE_ROOT", f"{field} escapes root.", lane_id, shown)
    return resolved, None


def validate(root: Path, manifest_path: Path) -> Result:
    root = root.resolve()
    manifest_path = manifest_path.resolve()
    manifest = _load(manifest_path, "launch manifest")
    findings: list[Finding] = []

    if manifest.get("schemaVersion") != 1:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))

    index_path, issue = _resolve(root, manifest.get("envelopeIndexPath"), "envelopeIndexPath")
    if issue:
        findings.append(issue)
        index = {}
    elif index_path is None or not index_path.is_file():
        findings.append(Finding("ENVELOPE_INDEX_MISSING", "Envelope index does not exist.", path=str(manifest.get("envelopeIndexPath"))))
        index = {}
    else:
        index = _load(index_path, "envelope index")

    envelope_by_id: dict[str, dict[str, Any]] = {}
    for raw in index.get("envelopeFiles", []):
        path, path_issue = _resolve(root, raw, "envelope file")
        if path_issue:
            findings.append(path_issue)
            continue
        if path is None or not path.is_file():
            findings.append(Finding("ENVELOPE_FILE_MISSING", "Envelope file does not exist.", path=str(raw)))
            continue
        row = _load(path, f"envelope {raw}")
        if _text(row.get("id")):
            envelope_by_id[row["id"]] = row

    for raw in _strings(manifest.get("requiredGlobalReads"), "requiredGlobalReads", findings, required=True):
        path, path_issue = _resolve(root, raw, "requiredGlobalReads")
        if path_issue:
            findings.append(path_issue)
        elif path is None or not path.is_file():
            findings.append(Finding("REQUIRED_READ_MISSING", "Required read does not exist.", path=raw))

    lanes = manifest.get("lanes")
    if not isinstance(lanes, list) or not lanes:
        findings.append(Finding("LANES_REQUIRED", "lanes must be a non-empty list."))
        lanes = []

    lane_ids: list[str] = []
    prompt_ids: list[str] = []
    assignments: list[str] = []
    expected_lanes = {"architecture", "multiplayer", "art", "story-ship"}

    for index_number, lane in enumerate(lanes):
        if not isinstance(lane, dict):
            findings.append(Finding("LANE_INVALID", f"lanes[{index_number}] must be an object."))
            continue
        lane_id = lane.get("id")
        if not _text(lane_id):
            findings.append(Finding("LANE_ID_INVALID", "Lane id is required."))
            lane_id = f"<invalid-{index_number}>"
        else:
            lane_id = lane_id.strip()
            lane_ids.append(lane_id)
        if lane_id not in expected_lanes:
            findings.append(Finding("LANE_UNKNOWN", f"Unknown lane {lane_id!r}.", lane_id))

        prompt_id = lane.get("takeoverPromptId")
        if not _text(prompt_id):
            findings.append(Finding("PROMPT_ID_INVALID", "takeoverPromptId is required.", lane_id))
        else:
            prompt_ids.append(prompt_id.strip())

        board, board_issue = _resolve(root, lane.get("board"), "board", lane_id)
        if board_issue:
            findings.append(board_issue)
        elif board is None or not board.is_file():
            findings.append(Finding("BOARD_MISSING", "Lane board does not exist.", lane_id, str(lane.get("board"))))

        allowed = _strings(lane.get("allowedEnvelopes"), "allowedEnvelopes", findings, lane_id, True)
        blocked = _strings(lane.get("blockedBy"), "blockedBy", findings, lane_id)
        _strings(lane.get("stopConditions"), "stopConditions", findings, lane_id, True)
        assignments.extend(allowed)

        for envelope_id in allowed:
            envelope = envelope_by_id.get(envelope_id)
            if envelope is None:
                findings.append(Finding("ASSIGNED_ENVELOPE_UNKNOWN", f"Unknown envelope {envelope_id}.", lane_id))
            elif envelope.get("owner") != lane_id:
                findings.append(Finding("ASSIGNED_OWNER_MISMATCH", f"{envelope_id} belongs to {envelope.get('owner')!r}.", lane_id))

        start = lane.get("startEnvelope")
        if start not in allowed:
            findings.append(Finding("START_NOT_ALLOWED", "startEnvelope must be in allowedEnvelopes.", lane_id))
        start_row = envelope_by_id.get(start)
        if start_row is not None:
            unresolved = [dep for dep in start_row.get("dependencies", []) if dep not in blocked]
            if unresolved:
                findings.append(Finding("START_DEPENDENCY_UNDECLARED", f"Start dependencies not declared in blockedBy: {unresolved}.", lane_id))
        for dep in blocked:
            if dep not in envelope_by_id:
                findings.append(Finding("BLOCKER_UNKNOWN", f"Unknown blocker {dep}.", lane_id))

        prompt = lane.get("prompt")
        if not _text(prompt):
            findings.append(Finding("PROMPT_REQUIRED", "prompt is required.", lane_id))
        else:
            if start and start not in prompt:
                findings.append(Finding("PROMPT_START_MISSING", "Prompt must name startEnvelope.", lane_id))
            if str(lane.get("board")) not in prompt:
                findings.append(Finding("PROMPT_BOARD_MISSING", "Prompt must name lane board.", lane_id))
            if "claim" not in prompt.lower():
                findings.append(Finding("PROMPT_CLAIM_MISSING", "Prompt must require claim-before-edit.", lane_id))

    for duplicate in sorted(item for item, count in Counter(lane_ids).items() if count > 1):
        findings.append(Finding("LANE_ID_DUPLICATE", "Lane id is duplicated.", duplicate))
    for duplicate in sorted(item for item, count in Counter(prompt_ids).items() if count > 1):
        findings.append(Finding("PROMPT_ID_DUPLICATE", "Prompt id is duplicated.", path=duplicate))
    for duplicate in sorted(item for item, count in Counter(assignments).items() if count > 1):
        findings.append(Finding("ENVELOPE_ASSIGNED_MULTIPLE", "Envelope is assigned to multiple lanes.", path=duplicate))

    if set(lane_ids) != expected_lanes:
        findings.append(Finding("LANE_SET_MISMATCH", f"Expected {sorted(expected_lanes)}, got {sorted(set(lane_ids))}."))
    if set(assignments) != set(envelope_by_id):
        findings.append(Finding("ENVELOPE_ASSIGNMENT_MISMATCH", f"Expected {sorted(envelope_by_id)}, got {sorted(set(assignments))}."))

    review = manifest.get("integrationReview")
    if not isinstance(review, dict) or not _text(review.get("id")) or not _text(review.get("prompt")):
        findings.append(Finding("INTEGRATION_REVIEW_INVALID", "integrationReview id and prompt are required."))

    return Result(
        root=str(root),
        manifest_path=str(manifest_path),
        lane_count=len(lanes),
        assigned_envelopes=len(set(assignments)),
        findings=tuple(findings),
    )


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--manifest", type=Path, default=Path("docs/first_hour/opus_launch_manifest.json"))
    parser.add_argument("--json-report", type=Path)
    parser.add_argument("--strict", action="store_true")
    args = parser.parse_args(argv)
    root = args.root.resolve()
    path = args.manifest if args.manifest.is_absolute() else root / args.manifest
    try:
        result = validate(root, path)
    except LoadError as exc:
        print(f"FIRST_HOUR_LAUNCH_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report = args.json_report if args.json_report.is_absolute() else root / args.json_report
        try:
            report = report.resolve()
            report.relative_to(root)
            report.parent.mkdir(parents=True, exist_ok=True)
            report.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")
        except (OSError, ValueError) as exc:
            print(f"FIRST_HOUR_LAUNCH_ERROR Could not write report: {exc}", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR

    print(
        "FIRST_HOUR_LAUNCH_REPORT "
        f"status={result.status.upper()} findings={len(result.findings)} "
        f"lanes={result.lane_count} assigned={result.assigned_envelopes}"
    )
    for finding in result.findings:
        lane = f" lane={finding.lane_id}" if finding.lane_id else ""
        path_text = f" path={finding.path}" if finding.path else ""
        print(f"FIRST_HOUR_LAUNCH_FINDING code={finding.code}{lane}{path_text} message={finding.message}")
    return EXIT_VALIDATION_FAILED if args.strict and result.findings else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
