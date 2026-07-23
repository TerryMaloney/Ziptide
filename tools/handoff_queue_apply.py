#!/usr/bin/env python3
"""Prepend validated queued entries to docs/HANDOFF.md and consume the queue files.

This exists for connector-driven sessions that can create small files safely but cannot replace the
entire large handoff without risking truncation. Queue files remain ordinary Markdown and are only
applied when they satisfy the established Did / Next / Heads-up / Commit structure.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Sequence

MARKER = "## ENTRIES — newest first"
HEADING = re.compile(r"^### \d{4}-\d{2}-\d{2} .+", re.MULTILINE)
REQUIRED_LABELS = ("- **Did:**", "- **Next:**", "- **Heads-up:**", "- **Commit:**")
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class ApplyResult:
    applied: tuple[str, ...]
    skipped_duplicates: tuple[str, ...]
    findings: tuple[Finding, ...]

    def to_dict(self) -> dict:
        return {
            "applied": list(self.applied),
            "skippedDuplicates": list(self.skipped_duplicates),
            "findingCount": len(self.findings),
            "findings": [asdict(item) for item in self.findings],
        }


class HandoffQueueError(RuntimeError):
    pass


def _normalize_entry(text: str) -> str:
    return text.strip() + "\n"


def _validate_entry(path: Path, text: str) -> tuple[str | None, list[Finding]]:
    findings: list[Finding] = []
    headings = HEADING.findall(text)
    if len(headings) != 1 or not text.lstrip().startswith("### "):
        findings.append(
            Finding(
                "ENTRY_HEADING_INVALID",
                "Queue entry must begin with exactly one dated level-3 heading.",
                path.as_posix(),
            )
        )
        heading = None
    else:
        heading = headings[0]
    for label in REQUIRED_LABELS:
        if label not in text:
            findings.append(
                Finding(
                    "ENTRY_SECTION_MISSING",
                    f"Queue entry is missing required label {label!r}.",
                    path.as_posix(),
                )
            )
    if MARKER in text or text.startswith("# HANDOFF"):
        findings.append(
            Finding(
                "ENTRY_CONTAINS_HANDOFF_STRUCTURE",
                "Queue entry must contain one entry only, not handoff document structure.",
                path.as_posix(),
            )
        )
    return heading, findings


def apply_queue(root: Path, handoff_path: Path, queue_dir: Path, *, check_only: bool = False) -> ApplyResult:
    root = root.resolve()
    handoff_path = handoff_path.resolve()
    queue_dir = queue_dir.resolve()
    if not handoff_path.is_file():
        raise HandoffQueueError(f"Handoff file is missing: {handoff_path}")
    if not queue_dir.exists():
        return ApplyResult((), (), ())

    handoff = handoff_path.read_text(encoding="utf-8")
    if MARKER not in handoff:
        raise HandoffQueueError(f"Handoff marker is missing: {MARKER}")

    queue_paths = sorted(queue_dir.glob("*.md"), reverse=True)
    valid_entries: list[tuple[Path, str, str]] = []
    findings: list[Finding] = []
    skipped: list[str] = []
    seen_headings: set[str] = set()

    for path in queue_paths:
        text = _normalize_entry(path.read_text(encoding="utf-8"))
        heading, entry_findings = _validate_entry(path, text)
        findings.extend(entry_findings)
        if entry_findings or heading is None:
            continue
        if heading in seen_headings:
            findings.append(
                Finding(
                    "QUEUE_HEADING_DUPLICATE",
                    f"Multiple queued entries use heading {heading!r}.",
                    path.as_posix(),
                )
            )
            continue
        seen_headings.add(heading)
        if heading in handoff:
            skipped.append(path.relative_to(root).as_posix())
            continue
        valid_entries.append((path, heading, text))

    if findings:
        return ApplyResult((), tuple(skipped), tuple(findings))

    if not check_only and valid_entries:
        insertion = "\n".join(text.rstrip() for _, _, text in valid_entries) + "\n\n"
        marker_with_gap = MARKER + "\n"
        handoff = handoff.replace(marker_with_gap, marker_with_gap + "\n" + insertion, 1)
        handoff_path.write_text(handoff, encoding="utf-8")

    if not check_only:
        for path, _, _ in valid_entries:
            path.unlink()
        for relative in skipped:
            (root / relative).unlink(missing_ok=True)

    return ApplyResult(
        applied=tuple(path.relative_to(root).as_posix() for path, _, _ in valid_entries),
        skipped_duplicates=tuple(skipped),
        findings=(),
    )


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--handoff", type=Path, default=Path("docs/HANDOFF.md"))
    parser.add_argument("--queue-dir", type=Path, default=Path("docs/handoff_queue"))
    parser.add_argument("--check", action="store_true")
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    handoff = args.handoff if args.handoff.is_absolute() else root / args.handoff
    queue_dir = args.queue_dir if args.queue_dir.is_absolute() else root / args.queue_dir
    try:
        result = apply_queue(root, handoff, queue_dir, check_only=args.check)
    except (HandoffQueueError, OSError, UnicodeError) as exc:
        print(f"HANDOFF_QUEUE_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report_path = args.json_report if args.json_report.is_absolute() else root / args.json_report
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    for finding in result.findings:
        print(f"{finding.code}: {finding.message} ({finding.path})", file=sys.stderr)
    print(
        "HANDOFF_QUEUE "
        f"applied={len(result.applied)} skipped={len(result.skipped_duplicates)} "
        f"findings={len(result.findings)} check={args.check}"
    )
    return EXIT_VALIDATION_FAILED if result.findings else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
