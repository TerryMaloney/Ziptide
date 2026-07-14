#!/usr/bin/env python3
"""Build a precise runtime input-action and legacy chord inventory for ZIPTIDE R0.

The broad contract scanner identifies files that mention input. This tool pairs
InputAction construction with AddBinding calls inside each source type and records
legacy menu-chord language such as Y+B. It is report-only.
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
DEFAULT_ROOTS = (
    "Ziptide/Assets/Ziptide",
    "Ziptide/Assets/ZiptideNet",
)

ACTION_RE = re.compile(
    r"(?P<field>_[A-Za-z_]\w*)\s*=\s*new\s+InputAction\s*\(\s*\"(?P<name>[^\"]+)\""
)
BINDING_RE = re.compile(
    r"(?P<field>_[A-Za-z_]\w*)\s*\.\s*AddBinding\s*\(\s*\"(?P<binding>[^\"]+)\"\s*\)"
)
CHORD_RE = re.compile(
    r"(?:\bY\s*\+\s*B\b|\bB\s*\+\s*Y\b|dev\s+menu\s+chord|menu\s+chord)",
    re.IGNORECASE,
)
TYPE_RE = re.compile(
    r"^\s*(?:(?:public|internal|private|protected|static|sealed|abstract|partial)\s+)*"
    r"(?:class|struct)\s+([A-Za-z_]\w*)",
    re.MULTILINE,
)
NAMESPACE_RE = re.compile(r"^\s*namespace\s+([A-Za-z_][\w.]*)", re.MULTILINE)


@dataclass(frozen=True)
class Binding:
    path: str
    symbol: str
    field: str
    action_name: str
    binding: str
    action_line: int
    binding_line: int


@dataclass(frozen=True)
class ChordReference:
    path: str
    symbol: str
    line: int
    text: str


def _line(text: str, offset: int) -> int:
    return text.count("\n", 0, offset) + 1


def _snippet(text: str, offset: int) -> str:
    start = text.rfind("\n", 0, offset) + 1
    end = text.find("\n", offset)
    if end < 0:
        end = len(text)
    return " ".join(text[start:end].strip().split())[:300]


def _symbol(text: str) -> str:
    ns = NAMESPACE_RE.search(text)
    typ = TYPE_RE.search(text)
    namespace = ns.group(1) if ns else ""
    type_name = typ.group(1) if typ else ""
    return f"{namespace}.{type_name}" if namespace and type_name else type_name or namespace


def scan_text(path: str, text: str) -> tuple[list[Binding], list[ChordReference]]:
    symbol = _symbol(text)
    actions: dict[str, tuple[str, int]] = {}
    for match in ACTION_RE.finditer(text):
        actions[match.group("field")] = (match.group("name"), _line(text, match.start()))

    bindings: list[Binding] = []
    for match in BINDING_RE.finditer(text):
        field = match.group("field")
        action_name, action_line = actions.get(field, ("<unresolved>", 0))
        bindings.append(
            Binding(
                path=path,
                symbol=symbol,
                field=field,
                action_name=action_name,
                binding=match.group("binding"),
                action_line=action_line,
                binding_line=_line(text, match.start()),
            )
        )

    chords = [
        ChordReference(
            path=path,
            symbol=symbol,
            line=_line(text, match.start()),
            text=_snippet(text, match.start()),
        )
        for match in CHORD_RE.finditer(text)
    ]
    bindings.sort(key=lambda item: (item.path, item.binding_line, item.field))
    chords.sort(key=lambda item: (item.path, item.line))
    return bindings, chords


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
    bindings: list[Binding] = []
    chords: list[ChordReference] = []
    scanned = 0
    for path in sorted(set(_files(root, scan_roots))):
        text = path.read_text(encoding="utf-8", errors="replace")
        relative = str(path.relative_to(root)).replace("\\", "/")
        file_bindings, file_chords = scan_text(relative, text)
        bindings.extend(file_bindings)
        chords.extend(file_chords)
        scanned += 1

    by_control: dict[str, list[dict]] = {}
    for binding in bindings:
        by_control.setdefault(binding.binding, []).append(asdict(binding))

    collisions = {
        control: owners
        for control, owners in sorted(by_control.items())
        if len({owner["symbol"] for owner in owners}) > 1
    }
    return {
        "tool": "recovery_input_contract_scan",
        "scannedFiles": scanned,
        "bindingCount": len(bindings),
        "chordReferenceCount": len(chords),
        "collisionControlCount": len(collisions),
        "bindings": [asdict(item) for item in bindings],
        "chordReferences": [asdict(item) for item in chords],
        "controlCollisions": collisions,
    }


def write_json(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def write_markdown(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Runtime Input Contract Scan",
        "",
        f"- Scanned C# files: **{report['scannedFiles']}**",
        f"- Runtime-created bindings: **{report['bindingCount']}**",
        f"- Legacy menu-chord references: **{report['chordReferenceCount']}**",
        f"- Controls bound by more than one owner: **{report['collisionControlCount']}**",
        "",
        "## Legacy menu-chord references",
        "",
    ]
    if not report["chordReferences"]:
        lines.append("None.")
    else:
        for item in report["chordReferences"]:
            lines.append(
                f"- `{item['path']}:{item['line']}` · `{item['symbol']}` — `{item['text']}`"
            )

    lines.extend(["", "## Runtime-created bindings", "", "| Owner | Action | Field | Binding | Source |", "|---|---|---|---|---|"])
    for item in report["bindings"]:
        lines.append(
            f"| `{item['symbol']}` | `{item['action_name']}` | `{item['field']}` | "
            f"`{item['binding']}` | `{item['path']}:{item['binding_line']}` |"
        )

    lines.extend(["", "## Cross-owner control collisions", ""])
    if not report["controlCollisions"]:
        lines.append("None.")
    else:
        for control, owners in report["controlCollisions"].items():
            lines.append(f"### `{control}`")
            lines.append("")
            for owner in owners:
                lines.append(
                    f"- `{owner['symbol']}` → `{owner['action_name']}` at "
                    f"`{owner['path']}:{owner['binding_line']}`"
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
        default=Path("Builds/Reports/recovery_input_contract_scan.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_input_contract_scan.md"),
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
        print(f"RECOVERY_INPUT_SCAN_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_INPUT_SCAN bindings={report['bindingCount']} "
        f"chords={report['chordReferenceCount']} collisions={report['collisionControlCount']}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
