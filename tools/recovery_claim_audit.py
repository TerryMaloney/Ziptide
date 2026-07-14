#!/usr/bin/env python3
"""Inventory strong completion claims in ZIPTIDE's current status boards.

This report does not automatically declare a claim false. It records where current
project prose says SHIPPED/COMPLETE/green/verified and whether the same line names
a concrete proof lane, so R0 can rewrite the live status layer from evidence.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Sequence

EXIT_OK = 0
EXIT_ERROR = 1

DEFAULT_DOCS = (
    "docs/CURRENT_EXECUTION_CHECKLIST.md",
    "docs/MASTER_CHECKLIST.md",
    "docs/EXCELLENCE_MAP.md",
    "docs/SPRINT.md",
    "docs/SPRINT_ART.md",
    "docs/SPRINT_MULTIPLAYER.md",
    "docs/SPRINT_ARCHITECTURE.md",
)

CLAIM_RE = re.compile(
    r"(✅|💎|\bSHIPPED\b|\bCOMPLETE\b|\bCLOSED\b|\bDONE\b|\bBUILT\b|"
    r"\bVERIFIED\b|\bGREEN\b|\bAUDIT[- ]CLEAN\b|\bDEVICE[- ]READY\b)",
    re.IGNORECASE,
)
QUALIFIERS = {
    "EDITMODE": re.compile(r"\bEditMode\b", re.IGNORECASE),
    "PLAYMODE": re.compile(r"\bPlayMode\b", re.IGNORECASE),
    "PATCHED_AUDIT": re.compile(r"\b(?:patch|audit)\b", re.IGNORECASE),
    "APK": re.compile(r"\bAPK\b|\bAndroid\b", re.IGNORECASE),
    "VISUAL": re.compile(r"\b(?:visual|photo|turnaround|screenshot)\b", re.IGNORECASE),
    "QUEST": re.compile(r"\b(?:Quest|headset|device)\b", re.IGNORECASE),
    "CI": re.compile(r"\bCI\b|\brun\s+\d{5,}\b", re.IGNORECASE),
}


@dataclass(frozen=True)
class Claim:
    path: str
    line: int
    text: str
    markers: tuple[str, ...]
    qualifiers: tuple[str, ...]

    @property
    def unqualified(self) -> bool:
        return not self.qualifiers


def scan_claims(root: Path, docs: Sequence[str]) -> tuple[list[Claim], list[str]]:
    claims: list[Claim] = []
    missing: list[str] = []
    for raw in docs:
        path = root / raw
        if not path.is_file():
            missing.append(raw)
            continue
        for line_number, line in enumerate(path.read_text(encoding="utf-8").splitlines(), start=1):
            matches = [match.group(0) for match in CLAIM_RE.finditer(line)]
            if not matches:
                continue
            qualifiers = tuple(name for name, regex in QUALIFIERS.items() if regex.search(line))
            claims.append(
                Claim(
                    path=raw,
                    line=line_number,
                    text=" ".join(line.strip().split())[:500],
                    markers=tuple(sorted(set(matches), key=str.upper)),
                    qualifiers=qualifiers,
                )
            )
    claims.sort(key=lambda item: (item.path, item.line))
    return claims, missing


def build_report(claims: list[Claim], missing: list[str]) -> dict:
    qualifier_counts: dict[str, int] = {}
    file_counts: dict[str, int] = {}
    for claim in claims:
        file_counts[claim.path] = file_counts.get(claim.path, 0) + 1
        if claim.unqualified:
            qualifier_counts["UNQUALIFIED"] = qualifier_counts.get("UNQUALIFIED", 0) + 1
        for qualifier in claim.qualifiers:
            qualifier_counts[qualifier] = qualifier_counts.get(qualifier, 0) + 1
    return {
        "tool": "recovery_claim_audit",
        "claimCount": len(claims),
        "unqualifiedCount": sum(1 for claim in claims if claim.unqualified),
        "missingDocuments": missing,
        "fileCounts": dict(sorted(file_counts.items())),
        "qualifierCounts": dict(sorted(qualifier_counts.items())),
        "claims": [
            {
                **asdict(claim),
                "unqualified": claim.unqualified,
            }
            for claim in claims
        ],
    }


def write_json(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def write_markdown(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Current-Board Completion Claim Audit",
        "",
        f"- Strong-claim lines: **{report['claimCount']}**",
        f"- No proof qualifier on the same line: **{report['unqualifiedCount']}**",
        "",
        "A finding is not automatically false. It is a line that must be reconciled with the recovery proof ledger before the current status layer can be trusted.",
        "",
        "## Counts by current document",
        "",
    ]
    for doc, count in report["fileCounts"].items():
        lines.append(f"- `{doc}`: {count}")
    lines.extend(["", "## Claims", ""])
    for claim in report["claims"]:
        qualifiers = ", ".join(f"`{item}`" for item in claim["qualifiers"]) or "**none**"
        lines.append(
            f"- `{claim['path']}:{claim['line']}` — qualifiers: {qualifiers} — {claim['text']}"
        )
    if report["missingDocuments"]:
        lines.extend(["", "## Missing configured documents", ""])
        for missing in report["missingDocuments"]:
            lines.append(f"- `{missing}`")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--doc", action="append", dest="docs")
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_claim_audit.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_claim_audit.md"),
    )
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    try:
        claims, missing = scan_claims(root, tuple(args.docs or DEFAULT_DOCS))
        report = build_report(claims, missing)
        json_path = _resolve(root, args.json_report).resolve()
        md_path = _resolve(root, args.markdown_report).resolve()
        for report_path in (json_path, md_path):
            report_path.relative_to(root)
        write_json(report, json_path)
        write_markdown(report, md_path)
    except (OSError, ValueError) as exc:
        print(f"RECOVERY_CLAIM_AUDIT_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_CLAIM_AUDIT claims={report['claimCount']} "
        f"unqualified={report['unqualifiedCount']} missingDocs={len(report['missingDocuments'])}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
