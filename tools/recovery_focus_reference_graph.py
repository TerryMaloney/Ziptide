#!/usr/bin/env python3
"""Generate focused R0 reference graphs for unresolved recovery decisions.

The report traces exact source references for repair/objective/cast-off, melee and
ship assembly/refit contracts. It is lexical evidence with file/line/snippet and
never edits gameplay code.
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

FOCUSES: dict[str, tuple[str, ...]] = {
    "repairObjective": (
        "RepairableMachine",
        "JobRuntime",
        "JobDirector",
        "ObjectiveBoard",
        "ShipCastOffRuntime",
        "CastOffArming",
        "REPAIR_TRACE",
        "gate_coupler",
        "ReportRepair",
        "RepairMachineCountStepDefinition",
    ),
    "melee": (
        "HammerTool",
        "SonicThumperRuntime",
        "MeleeWeaponRuntime",
        "ArenaWeaponKind.SonicThumper",
        "ArenaWeaponKind.BreakerBlade",
        "ArenaWeaponKind.TidePike",
        "Muzzle",
        "HitFromHammer",
    ),
    "shipPresentation": (
        "ShipHullBuilder",
        "ShipHullBuilder.Build",
        "ShipRefit",
        "ShipRefit.Apply",
        "ShipCastOffRuntime",
        "ShipChassisPreset",
        "ShipLocker",
        "Fuselage_Aft",
        "__SHIP",
        "ShipRoot",
    ),
}

TYPE_RE = re.compile(
    r"^\s*(?:(?:public|internal|private|protected|static|sealed|abstract|partial|readonly)\s+)*"
    r"(?:class|struct|interface|enum)\s+([A-Za-z_]\w*)",
    re.MULTILINE,
)
NAMESPACE_RE = re.compile(r"^\s*namespace\s+([A-Za-z_][\w.]*)", re.MULTILINE)


@dataclass(frozen=True)
class Reference:
    focus: str
    token: str
    path: str
    line: int
    symbol: str
    snippet: str
    declaration_file: bool


def _symbol(text: str) -> str:
    ns = NAMESPACE_RE.search(text)
    typ = TYPE_RE.search(text)
    namespace = ns.group(1) if ns else ""
    type_name = typ.group(1) if typ else ""
    return f"{namespace}.{type_name}" if namespace and type_name else type_name or namespace or "<unknown>"


def _line(text: str, offset: int) -> int:
    return text.count("\n", 0, offset) + 1


def _snippet(text: str, offset: int) -> str:
    start = text.rfind("\n", 0, offset) + 1
    end = text.find("\n", offset)
    if end < 0:
        end = len(text)
    return " ".join(text[start:end].strip().split())[:360]


def scan_text(path: str, text: str) -> list[Reference]:
    symbol = _symbol(text)
    declared_names = {match.group(1) for match in TYPE_RE.finditer(text)}
    refs: list[Reference] = []
    for focus, tokens in FOCUSES.items():
        for token in tokens:
            pattern = re.compile(re.escape(token))
            for match in pattern.finditer(text):
                refs.append(
                    Reference(
                        focus=focus,
                        token=token,
                        path=path,
                        line=_line(text, match.start()),
                        symbol=symbol,
                        snippet=_snippet(text, match.start()),
                        declaration_file=token.split(".")[-1] in declared_names,
                    )
                )
    unique: dict[tuple[str, str, str, int], Reference] = {}
    for ref in refs:
        unique[(ref.focus, ref.token, ref.path, ref.line)] = ref
    return sorted(unique.values(), key=lambda item: (item.focus, item.token, item.path, item.line))


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
    refs: list[Reference] = []
    scanned = 0
    for path in sorted(set(_files(root, scan_roots))):
        text = path.read_text(encoding="utf-8", errors="replace")
        relative = str(path.relative_to(root)).replace("\\", "/")
        refs.extend(scan_text(relative, text))
        scanned += 1

    grouped: dict[str, dict[str, list[dict]]] = {}
    for ref in refs:
        grouped.setdefault(ref.focus, {}).setdefault(ref.token, []).append(asdict(ref))

    summaries: dict[str, dict] = {}
    for focus, tokens in grouped.items():
        paths = sorted({entry["path"] for entries in tokens.values() for entry in entries})
        callers = sorted(
            {
                entry["path"]
                for entries in tokens.values()
                for entry in entries
                if not entry["declaration_file"]
            }
        )
        summaries[focus] = {
            "referenceCount": sum(len(entries) for entries in tokens.values()),
            "fileCount": len(paths),
            "callerFileCount": len(callers),
            "paths": paths,
            "callerPaths": callers,
        }

    return {
        "tool": "recovery_focus_reference_graph",
        "scannedFiles": scanned,
        "referenceCount": len(refs),
        "summaries": dict(sorted(summaries.items())),
        "focuses": {
            focus: {token: entries for token, entries in sorted(tokens.items())}
            for focus, tokens in sorted(grouped.items())
        },
        "references": [asdict(ref) for ref in refs],
    }


def write_json(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def write_markdown(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Focused Recovery Reference Graph",
        "",
        f"- Scanned C# files: **{report['scannedFiles']}**",
        f"- Focused references: **{report['referenceCount']}**",
        "",
    ]
    for focus, summary in report["summaries"].items():
        lines.extend(
            [
                f"## {focus}",
                "",
                f"- References: **{summary['referenceCount']}**",
                f"- Files: **{summary['fileCount']}**",
                f"- Non-declaration caller files: **{summary['callerFileCount']}**",
                "",
            ]
        )
        for token, entries in report["focuses"][focus].items():
            lines.append(f"### `{token}`")
            lines.append("")
            for entry in entries:
                kind = "declaration" if entry["declaration_file"] else "reference"
                lines.append(
                    f"- **{kind}** · `{entry['symbol']}` · `{entry['path']}:{entry['line']}` — `{entry['snippet']}`"
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
        default=Path("Builds/Reports/recovery_focus_reference_graph.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_focus_reference_graph.md"),
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
        print(f"RECOVERY_FOCUS_GRAPH_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_FOCUS_GRAPH refs={report['referenceCount']} "
        f"focuses={len(report['summaries'])}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
