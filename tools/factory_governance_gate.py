#!/usr/bin/env python3
"""Fast repository-governance checks that do not require Unity.

This mirrors the expensive EditMode checks for MISS_LEDGER field integrity and
stale sprint-board claims so operators can fail in seconds before pushing a
candidate into the Unity queue.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from datetime import date, datetime, timezone
from pathlib import Path
from typing import Sequence

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
REQUIRED_LEDGER_TOKENS = (
    "**WHAT",
    "**FOUND BY:**",
    "**WHY MISSED:**",
    "**CLASS:**",
    "**SYSTEM CHANGE:**",
)
DATE_PATTERN = re.compile(r"20\d\d-\d\d-\d\d")
LEDGER_ENTRY_PATTERN = re.compile(r"(?ms)^\s*(\d+)\.\s+(.*?)(?=^\s*\d+\.|\Z)")


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str


class GateInputError(RuntimeError):
    """Raised when the gate cannot safely inspect the repository."""


def _read(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8")
    except OSError as exc:
        raise GateInputError(f"Could not read {path}: {exc}") from exc


def _validate_miss_ledger(root: Path) -> list[Finding]:
    relative = Path("docs/MISS_LEDGER.md")
    path = root / relative
    ledger = _read(path)
    open_start = ledger.find("## OPEN")
    closed_start = ledger.find("## CLOSED")
    if open_start < 0 or closed_start <= open_start:
        return [
            Finding(
                "MISS_LEDGER_SECTIONS_INVALID",
                "MISS_LEDGER must contain an OPEN section followed by CLOSED.",
                relative.as_posix(),
            )
        ]

    entries = LEDGER_ENTRY_PATTERN.findall(ledger[open_start:closed_start])
    findings: list[Finding] = []
    if len(entries) < 10:
        findings.append(
            Finding(
                "MISS_LEDGER_ENTRY_COUNT_LOW",
                f"Expected at least 10 open entries; found {len(entries)}.",
                relative.as_posix(),
            )
        )

    for number, block in entries:
        for token in REQUIRED_LEDGER_TOKENS:
            if token not in block:
                findings.append(
                    Finding(
                        "MISS_LEDGER_FIELD_MISSING",
                        f"Open ledger entry #{number} lacks exact token {token!r}.",
                        relative.as_posix(),
                    )
                )
    return findings


def _load_paused_boards(root: Path, today: date) -> tuple[set[str], list[Finding]]:
    relative = Path("docs/recovery/paused_sprint_lanes.json")
    path = root / relative
    try:
        data = json.loads(_read(path))
    except json.JSONDecodeError as exc:
        raise GateInputError(
            f"Pause manifest is invalid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}"
        ) from exc

    findings: list[Finding] = []
    if not isinstance(data, dict) or data.get("schemaVersion") != "1":
        findings.append(
            Finding(
                "PAUSE_MANIFEST_SCHEMA_INVALID",
                "paused_sprint_lanes.json must be an object with schemaVersion '1'.",
                relative.as_posix(),
            )
        )
        return set(), findings

    paused = data.get("paused")
    if not isinstance(paused, list):
        findings.append(
            Finding(
                "PAUSE_MANIFEST_ARRAY_MISSING",
                "paused_sprint_lanes.json must contain a paused array.",
                relative.as_posix(),
            )
        )
        return set(), findings

    boards: set[str] = set()
    for index, lane in enumerate(paused):
        prefix = f"paused[{index}]"
        if not isinstance(lane, dict):
            findings.append(
                Finding("PAUSE_ENTRY_INVALID", f"{prefix} must be an object.", relative.as_posix())
            )
            continue
        board = lane.get("board")
        paused_since = lane.get("pausedSince")
        reason = lane.get("reason")
        resume_gate = lane.get("resumeGate")
        if not isinstance(board, str) or not board.strip():
            findings.append(
                Finding("PAUSE_BOARD_MISSING", f"{prefix}.board is required.", relative.as_posix())
            )
            continue
        board = board.strip()
        if board in boards:
            findings.append(
                Finding("PAUSE_BOARD_DUPLICATE", f"Pause manifest repeats {board}.", relative.as_posix())
            )
        boards.add(board)
        if not (root / "docs" / board).is_file():
            findings.append(
                Finding(
                    "PAUSE_BOARD_NOT_FOUND",
                    f"Pause manifest references missing docs/{board}.",
                    relative.as_posix(),
                )
            )
        try:
            parsed = date.fromisoformat(str(paused_since))
            if parsed > today:
                findings.append(
                    Finding(
                        "PAUSE_DATE_IN_FUTURE",
                        f"{prefix}.pausedSince is future-dated: {parsed.isoformat()}.",
                        relative.as_posix(),
                    )
                )
        except ValueError:
            findings.append(
                Finding(
                    "PAUSE_DATE_INVALID",
                    f"{prefix}.pausedSince must be YYYY-MM-DD.",
                    relative.as_posix(),
                )
            )
        if not isinstance(reason, str) or not reason.strip():
            findings.append(
                Finding("PAUSE_REASON_MISSING", f"{prefix}.reason is required.", relative.as_posix())
            )
        if not isinstance(resume_gate, str) or not resume_gate.strip():
            findings.append(
                Finding(
                    "PAUSE_RESUME_GATE_MISSING",
                    f"{prefix}.resumeGate is required.",
                    relative.as_posix(),
                )
            )
    return boards, findings


def _validate_sprint_claims(root: Path, today: date, paused_boards: set[str]) -> list[Finding]:
    docs = root / "docs"
    if not docs.is_dir():
        raise GateInputError(f"Missing docs directory: {docs}")

    findings: list[Finding] = []
    for board in sorted(docs.glob("SPRINT*.md")):
        if board.name in paused_boards:
            continue
        for line_number, line in enumerate(_read(board).splitlines(), start=1):
            if "🟡" not in line:
                continue
            parsed_dates: list[date] = []
            for match in DATE_PATTERN.findall(line):
                try:
                    parsed_dates.append(date.fromisoformat(match))
                except ValueError:
                    continue
            if not parsed_dates:
                continue
            newest = max(parsed_dates)
            age_days = (today - newest).days
            if age_days > 14:
                findings.append(
                    Finding(
                        "SPRINT_CLAIM_STALE",
                        (
                            f"Yellow board row is {age_days} days old ({newest.isoformat()}); "
                            "finish it, re-date it with a HANDOFF note, release it, or pause it explicitly."
                        ),
                        f"docs/{board.name}:{line_number}",
                    )
                )
    return findings


def validate_repository(root: Path, *, today: date | None = None) -> tuple[Finding, ...]:
    root = root.resolve()
    effective_today = today or datetime.now(timezone.utc).date()
    paused_boards, pause_findings = _load_paused_boards(root, effective_today)
    findings = [
        *_validate_miss_ledger(root),
        *pause_findings,
        *_validate_sprint_claims(root, effective_today, paused_boards),
    ]
    return tuple(findings)


def _build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parents[1],
        help="Repository root. Defaults to the parent of tools/.",
    )
    parser.add_argument(
        "--today",
        type=date.fromisoformat,
        help="Override UTC date as YYYY-MM-DD (for deterministic tests).",
    )
    parser.add_argument("--json", action="store_true", help="Emit machine-readable JSON.")
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = _build_parser().parse_args(argv)
    try:
        findings = validate_repository(args.root, today=args.today)
    except GateInputError as exc:
        print(f"factory-governance operational error: {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json:
        print(
            json.dumps(
                {
                    "tool": "factory_governance_gate",
                    "status": "pass" if not findings else "fail",
                    "findingCount": len(findings),
                    "findings": [asdict(finding) for finding in findings],
                },
                indent=2,
            )
        )
    elif findings:
        print("FAST GOVERNANCE PREFLIGHT: FAIL", file=sys.stderr)
        for finding in findings:
            print(f"- [{finding.code}] {finding.path}: {finding.message}", file=sys.stderr)
    else:
        print("FAST GOVERNANCE PREFLIGHT: PASS")

    return EXIT_OK if not findings else EXIT_VALIDATION_FAILED


if __name__ == "__main__":
    raise SystemExit(main())
