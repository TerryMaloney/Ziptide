#!/usr/bin/env python3
"""Build a static event and persistence ownership graph for ZIPTIDE recovery R0.

The graph is evidence, not a correctness verdict. It records event declarations,
subscriptions, unsubscriptions, invocations, SaveSystem calls, autosave reasons and
common profile-field access with exact file/line ownership.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Iterable, Sequence

EXIT_OK = 0
EXIT_ERROR = 1
DEFAULT_ROOTS = ("Ziptide/Assets/Ziptide", "Ziptide/Assets/ZiptideNet")

NAMESPACE_RE = re.compile(r"^\s*namespace\s+([A-Za-z_][\w.]*)", re.MULTILINE)
TYPE_RE = re.compile(
    r"^\s*(?:(?:public|internal|private|protected|static|sealed|abstract|partial|readonly)\s+)*"
    r"(?:class|struct|interface)\s+([A-Za-z_]\w*)",
    re.MULTILINE,
)
EVENT_DECL_RE = re.compile(
    r"\b(?P<visibility>public|internal|protected|private)?\s*(?P<static>static\s+)?event\s+"
    r"(?P<type>[A-Za-z_][\w.<>, ?]*)\s+(?P<name>[A-Za-z_]\w*)\s*;"
)
SUBSCRIBE_RE = re.compile(
    r"(?P<target>(?:[A-Za-z_]\w*\.)*[A-Za-z_]\w*)\s*\+=\s*(?P<handler>[A-Za-z_]\w*|\([^;]+=>)"
)
UNSUBSCRIBE_RE = re.compile(
    r"(?P<target>(?:[A-Za-z_]\w*\.)*[A-Za-z_]\w*)\s*-=\s*(?P<handler>[A-Za-z_]\w*)"
)
INVOKE_RE = re.compile(r"(?P<event>[A-Za-z_]\w*)\s*(?:\?\.)?\.Invoke\s*\(")
DIRECT_DELEGATE_RE = re.compile(r"(?P<event>[A-Za-z_]\w*)\s*\?\s*\.\s*Invoke\s*\(")
AUTOSAVE_RE = re.compile(r"\b(?:SaveSystem\s*\.\s*)?AutosaveNow\s*\(\s*\"(?P<reason>[^\"]*)\"")
SAVE_CALL_RE = re.compile(
    r"\bSaveSystem\s*(?:\.\s*Instance)?\s*\.\s*(?P<member>StartNewProfile|Load|Save|AutosaveNow|Profile|HasExistingProfile)\b"
)
PROFILE_ACCESS_RE = re.compile(
    r"\b(?P<root>profile|Profile|SaveSystem\s*\.\s*Instance\s*\.\s*Profile)\s*\.\s*(?P<field>[A-Za-z_]\w*)"
)
PLAYER_PREFS_RE = re.compile(r"\bPlayerPrefs\s*\.\s*(?P<member>Get\w+|Set\w+|Delete\w+|Save)\s*\(")


@dataclass(frozen=True)
class Evidence:
    kind: str
    path: str
    line: int
    owner: str
    target: str
    detail: str
    snippet: str


def _owner(text: str) -> str:
    namespace = NAMESPACE_RE.search(text)
    type_match = TYPE_RE.search(text)
    ns = namespace.group(1) if namespace else ""
    typ = type_match.group(1) if type_match else ""
    return f"{ns}.{typ}" if ns and typ else typ or ns or "<unknown>"


def _line(text: str, offset: int) -> int:
    return text.count("\n", 0, offset) + 1


def _snippet(text: str, offset: int) -> str:
    start = text.rfind("\n", 0, offset) + 1
    end = text.find("\n", offset)
    if end < 0:
        end = len(text)
    return " ".join(text[start:end].strip().split())[:320]


def scan_text(path: str, text: str) -> list[Evidence]:
    owner = _owner(text)
    evidence: list[Evidence] = []

    for match in EVENT_DECL_RE.finditer(text):
        evidence.append(
            Evidence(
                "EVENT_DECLARE",
                path,
                _line(text, match.start()),
                owner,
                match.group("name"),
                ("static " if match.group("static") else "") + match.group("type").strip(),
                _snippet(text, match.start()),
            )
        )
    for match in SUBSCRIBE_RE.finditer(text):
        target = match.group("target")
        if "." not in target:
            continue
        evidence.append(
            Evidence(
                "EVENT_SUBSCRIBE",
                path,
                _line(text, match.start()),
                owner,
                target,
                match.group("handler"),
                _snippet(text, match.start()),
            )
        )
    for match in UNSUBSCRIBE_RE.finditer(text):
        target = match.group("target")
        if "." not in target:
            continue
        evidence.append(
            Evidence(
                "EVENT_UNSUBSCRIBE",
                path,
                _line(text, match.start()),
                owner,
                target,
                match.group("handler"),
                _snippet(text, match.start()),
            )
        )
    seen_invokes: set[tuple[str, int]] = set()
    for regex in (INVOKE_RE, DIRECT_DELEGATE_RE):
        for match in regex.finditer(text):
            line = _line(text, match.start())
            key = (match.group("event"), line)
            if key in seen_invokes:
                continue
            seen_invokes.add(key)
            evidence.append(
                Evidence(
                    "EVENT_INVOKE",
                    path,
                    line,
                    owner,
                    match.group("event"),
                    "",
                    _snippet(text, match.start()),
                )
            )
    for match in AUTOSAVE_RE.finditer(text):
        evidence.append(
            Evidence(
                "AUTOSAVE",
                path,
                _line(text, match.start()),
                owner,
                "SaveSystem.AutosaveNow",
                match.group("reason"),
                _snippet(text, match.start()),
            )
        )
    for match in SAVE_CALL_RE.finditer(text):
        evidence.append(
            Evidence(
                "SAVE_ACCESS",
                path,
                _line(text, match.start()),
                owner,
                "SaveSystem." + match.group("member"),
                "",
                _snippet(text, match.start()),
            )
        )
    for match in PROFILE_ACCESS_RE.finditer(text):
        evidence.append(
            Evidence(
                "PROFILE_FIELD_ACCESS",
                path,
                _line(text, match.start()),
                owner,
                match.group("field"),
                match.group("root").replace(" ", ""),
                _snippet(text, match.start()),
            )
        )
    for match in PLAYER_PREFS_RE.finditer(text):
        evidence.append(
            Evidence(
                "PLAYER_PREFS_ACCESS",
                path,
                _line(text, match.start()),
                owner,
                "PlayerPrefs." + match.group("member"),
                "",
                _snippet(text, match.start()),
            )
        )

    unique: dict[tuple[str, str, int, str], Evidence] = {}
    for item in evidence:
        unique[(item.kind, item.path, item.line, item.target)] = item
    return sorted(unique.values(), key=lambda item: (item.path, item.line, item.kind, item.target))


def _files(root: Path, scan_roots: Iterable[str]) -> Iterable[Path]:
    for raw in scan_roots:
        base = root / raw
        if not base.exists():
            continue
        for path in base.rglob("*.cs"):
            if any(part in {"Library", "Temp", "Build", "Builds", "Obj"} for part in path.parts):
                continue
            yield path


def scan_repository(root: Path, scan_roots: Sequence[str]) -> dict:
    all_evidence: list[Evidence] = []
    scanned = 0
    for path in sorted(set(_files(root, scan_roots))):
        text = path.read_text(encoding="utf-8", errors="replace")
        relative = str(path.relative_to(root)).replace("\\", "/")
        all_evidence.extend(scan_text(relative, text))
        scanned += 1

    kind_counts: dict[str, int] = {}
    by_target: dict[str, list[dict]] = {}
    for item in all_evidence:
        kind_counts[item.kind] = kind_counts.get(item.kind, 0) + 1
        by_target.setdefault(item.target, []).append(asdict(item))

    subscriptions = [item for item in all_evidence if item.kind == "EVENT_SUBSCRIBE"]
    unsub_pairs = {(item.owner, item.target, item.detail) for item in all_evidence if item.kind == "EVENT_UNSUBSCRIBE"}
    unmatched = [
        asdict(item)
        for item in subscriptions
        if (item.owner, item.target, item.detail) not in unsub_pairs and "=>" not in item.detail
    ]

    return {
        "tool": "recovery_event_save_graph",
        "scannedFiles": scanned,
        "evidenceCount": len(all_evidence),
        "kindCounts": dict(sorted(kind_counts.items())),
        "unmatchedNamedSubscriptionCount": len(unmatched),
        "unmatchedNamedSubscriptions": unmatched,
        "targets": {key: value for key, value in sorted(by_target.items())},
        "evidence": [asdict(item) for item in all_evidence],
    }


def write_json(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def write_markdown(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Event and Save Ownership Graph",
        "",
        f"- Scanned C# files: **{report['scannedFiles']}**",
        f"- Evidence edges: **{report['evidenceCount']}**",
        f"- Named subscriptions without matching unsubscribe in the same owner: **{report['unmatchedNamedSubscriptionCount']}**",
        "",
        "This is a static ownership graph. An unmatched row is a review target, not automatic proof of a leak; process-lifetime static hooks may be intentional.",
        "",
        "## Evidence counts",
        "",
    ]
    for kind, count in report["kindCounts"].items():
        lines.append(f"- **{kind}:** {count}")

    lines.extend(["", "## Named subscriptions without matching unsubscribe", ""])
    if not report["unmatchedNamedSubscriptions"]:
        lines.append("None.")
    else:
        for item in report["unmatchedNamedSubscriptions"]:
            lines.append(
                f"- `{item['owner']}` subscribes `{item['target']}` → `{item['detail']}` at "
                f"`{item['path']}:{item['line']}`"
            )

    lines.extend(["", "## Targets", ""])
    for target, entries in report["targets"].items():
        lines.append(f"### `{target}`")
        lines.append("")
        for item in entries:
            detail = f" · `{item['detail']}`" if item["detail"] else ""
            lines.append(
                f"- **{item['kind']}** · `{item['owner']}` · `{item['path']}:{item['line']}`{detail} — `{item['snippet']}`"
            )
        lines.append("")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--scan-root", action="append", dest="scan_roots")
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_event_save_graph.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_event_save_graph.md"),
    )
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    try:
        report = scan_repository(root, tuple(args.scan_roots or DEFAULT_ROOTS))
        json_path = _resolve(root, args.json_report).resolve()
        markdown_path = _resolve(root, args.markdown_report).resolve()
        for report_path in (json_path, markdown_path):
            report_path.relative_to(root)
        write_json(report, json_path)
        write_markdown(report, markdown_path)
    except (OSError, ValueError) as exc:
        print(f"RECOVERY_EVENT_SAVE_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_EVENT_SAVE edges={report['evidenceCount']} "
        f"unmatched={report['unmatchedNamedSubscriptionCount']}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
