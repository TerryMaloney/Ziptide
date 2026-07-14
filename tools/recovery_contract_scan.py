#!/usr/bin/env python3
"""Report-only static inventory scan for ZIPTIDE recovery R0.

The scanner inventories cross-cutting runtime ownership signals without changing
game code or declaring findings to be defects. It is intentionally lexical:
every hit includes an exact file, line and snippet for human/LLM review.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Iterable, Sequence

TOOL_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1

DEFAULT_SCAN_ROOTS = (
    "Ziptide/Assets/Ziptide",
    "Ziptide/Assets/ZiptideNet",
)

IGNORED_PARTS = {
    ".git",
    ".idea",
    ".vs",
    "Library",
    "Temp",
    "Logs",
    "Obj",
    "Build",
    "Builds",
    "UserSettings",
}

SOURCE_SUFFIXES = {".cs"}


@dataclass(frozen=True)
class PatternRule:
    category: str
    code: str
    regex: str
    description: str
    flags: int = 0


@dataclass(frozen=True)
class Finding:
    category: str
    code: str
    description: str
    path: str
    line: int
    symbol: str
    snippet: str


@dataclass(frozen=True)
class ScanResult:
    root: str
    scan_roots: tuple[str, ...]
    scanned_files: int
    findings: tuple[Finding, ...]

    @property
    def category_counts(self) -> dict[str, int]:
        counts: dict[str, int] = {}
        for finding in self.findings:
            counts[finding.category] = counts.get(finding.category, 0) + 1
        return dict(sorted(counts.items()))

    @property
    def code_counts(self) -> dict[str, int]:
        counts: dict[str, int] = {}
        for finding in self.findings:
            counts[finding.code] = counts.get(finding.code, 0) + 1
        return dict(sorted(counts.items()))

    def to_dict(self) -> dict[str, object]:
        return {
            "tool": "recovery_contract_scan",
            "toolVersion": TOOL_VERSION,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": "report",
            "root": self.root,
            "scanRoots": list(self.scan_roots),
            "scannedFiles": self.scanned_files,
            "findingCount": len(self.findings),
            "categoryCounts": self.category_counts,
            "codeCounts": self.code_counts,
            "findings": [asdict(finding) for finding in self.findings],
        }


RULES: tuple[PatternRule, ...] = (
    PatternRule(
        "bootstrap",
        "RUNTIME_BOOTSTRAP",
        r"\[\s*RuntimeInitializeOnLoadMethod(?:Attribute)?\b",
        "RuntimeInitializeOnLoadMethod bootstrap",
    ),
    PatternRule(
        "bootstrap",
        "EDITOR_BOOTSTRAP",
        r"\[\s*(?:InitializeOnLoad|InitializeOnLoadMethod)(?:Attribute)?\b",
        "Unity editor-load bootstrap",
    ),
    PatternRule(
        "persistence",
        "DONT_DESTROY_ON_LOAD",
        r"\bDontDestroyOnLoad\s*\(",
        "Object promoted across scene loads",
    ),
    PatternRule(
        "scene_loading",
        "DIRECT_SCENE_LOAD",
        r"\bSceneManager\s*\.\s*LoadScene\s*\(",
        "Direct synchronous scene load",
    ),
    PatternRule(
        "scene_loading",
        "DIRECT_SCENE_LOAD_ASYNC",
        r"\bSceneManager\s*\.\s*LoadSceneAsync\s*\(",
        "Direct asynchronous scene load",
    ),
    PatternRule(
        "runtime_creation",
        "NEW_GAME_OBJECT",
        r"\bnew\s+GameObject\s*\(",
        "Runtime GameObject construction",
    ),
    PatternRule(
        "runtime_creation",
        "CREATE_PRIMITIVE",
        r"\bGameObject\s*\.\s*CreatePrimitive\s*\(",
        "Runtime primitive construction",
    ),
    PatternRule(
        "runtime_ui",
        "TEXTMESH_COMPONENT",
        r"(?:AddComponent\s*<\s*TextMesh\s*>|typeof\s*\(\s*TextMesh\s*\))",
        "Legacy TextMesh creation/reference",
    ),
    PatternRule(
        "runtime_ui",
        "TMP_COMPONENT",
        r"(?:AddComponent\s*<\s*TextMeshPro(?:UGUI)?\s*>|typeof\s*\(\s*TextMeshPro(?:UGUI)?\s*\))",
        "TextMeshPro creation/reference",
    ),
    PatternRule(
        "runtime_ui",
        "CANVAS_COMPONENT",
        r"(?:AddComponent\s*<\s*Canvas\s*>|typeof\s*\(\s*Canvas\s*\))",
        "Canvas creation/reference",
    ),
    PatternRule(
        "runtime_ui",
        "EVENT_SYSTEM_COMPONENT",
        r"(?:AddComponent\s*<\s*EventSystem\s*>|typeof\s*\(\s*EventSystem\s*\))",
        "EventSystem creation/reference",
    ),
    PatternRule(
        "runtime_ui",
        "XR_UI_INPUT_MODULE",
        r"(?:AddComponent\s*<\s*XRUIInputModule\s*>|typeof\s*\(\s*XRUIInputModule\s*\))",
        "XR UI input module creation/reference",
    ),
    PatternRule(
        "runtime_ui",
        "XR_INTERACTABLE_COMPONENT",
        r"(?:AddComponent\s*<\s*XR(?:Simple|Base|Grab)Interactable\s*>|typeof\s*\(\s*XR(?:Simple|Base|Grab)Interactable\s*\))",
        "XR interactable creation/reference",
    ),
    PatternRule(
        "xri_ownership",
        "XRI_MANAGER_LOOKUP",
        r"(?:FindObjectOfType|FindObjectsOfType)\s*<\s*XRInteractionManager\s*>",
        "XRInteractionManager lookup",
    ),
    PatternRule(
        "xri_ownership",
        "XRI_MANAGER_CREATE",
        r"AddComponent\s*<\s*XRInteractionManager\s*>",
        "XRInteractionManager creation",
    ),
    PatternRule(
        "xri_ownership",
        "XRI_MANAGER_ASSIGN",
        r"\.\s*interactionManager\s*=",
        "Explicit interactable interactionManager assignment",
    ),
    PatternRule(
        "input",
        "INPUT_BUTTON_REFERENCE",
        r"\b(?:buttonSouth|buttonNorth|buttonEast|buttonWest|primaryButton|secondaryButton|menuButton|yButton|bButton)\b",
        "Controller/button reference",
    ),
    PatternRule(
        "input",
        "INPUT_ACTION_REFERENCE",
        r"\bInputAction(?:Reference|Asset|Manager)?\b",
        "Input System action reference",
    ),
    PatternRule(
        "input",
        "FRAME_BUTTON_POLL",
        r"\b(?:wasPressedThisFrame|isPressed|wasReleasedThisFrame)\b",
        "Frame-polled input state",
    ),
    PatternRule(
        "events",
        "STATIC_EVENT_DECLARATION",
        r"\bpublic\s+static\s+event\b",
        "Static event declaration",
    ),
    PatternRule(
        "events",
        "EVENT_DECLARATION",
        r"\bevent\s+(?:Action|System\.Action|UnityEvent)",
        "Event declaration",
    ),
    PatternRule(
        "save_state",
        "SAVE_SYSTEM_REFERENCE",
        r"\bSaveSystem\b",
        "SaveSystem reference",
    ),
    PatternRule(
        "save_state",
        "PLAYER_PROFILE_REFERENCE",
        r"\bPlayerProfile\b",
        "PlayerProfile reference",
    ),
    PatternRule(
        "save_state",
        "AUTOSAVE_CALL",
        r"\bAutosaveNow\s*\(",
        "Autosave request",
    ),
    PatternRule(
        "global_render",
        "RENDER_SETTINGS_MUTATION",
        r"\bRenderSettings\s*\.",
        "Global RenderSettings access",
    ),
    PatternRule(
        "global_render",
        "CAMERA_MAIN_REFERENCE",
        r"\bCamera\s*\.\s*main\b",
        "Camera.main dependency",
    ),
    PatternRule(
        "global_render",
        "VOLUME_COMPONENT",
        r"(?:AddComponent\s*<\s*(?:UnityEngine\.Rendering\.)?Volume\s*>|typeof\s*\(\s*(?:UnityEngine\.Rendering\.)?Volume\s*\))",
        "Global/local Volume creation/reference",
    ),
    PatternRule(
        "global_render",
        "CAMERA_POST_PROCESSING",
        r"\brenderPostProcessing\s*=",
        "Camera post-processing mutation",
    ),
    PatternRule(
        "materials",
        "RUNTIME_MATERIAL_CREATE",
        r"\bnew\s+Material\s*\(",
        "Runtime Material allocation",
    ),
    PatternRule(
        "materials",
        "SHADER_FIND",
        r"\bShader\s*\.\s*Find\s*\(",
        "Runtime shader lookup",
    ),
    PatternRule(
        "fallback_debt",
        "FALLBACK_MARKER",
        r"\b(?:fallback|placeholder|interim|stub|plumbing|skeleton|graybox|blockout)\b",
        "Source text marks fallback/prototype debt",
        re.IGNORECASE,
    ),
    PatternRule(
        "diagnostics",
        "ZIPTIDE_LOG_TAG",
        r"\bZIPTIDE(?:_DIAG)?\s*:",
        "ZIPTIDE diagnostic tag",
    ),
)

_COMPILED_RULES = tuple((rule, re.compile(rule.regex, rule.flags)) for rule in RULES)

_NAMESPACE_RE = re.compile(r"^\s*namespace\s+([A-Za-z_][\w.]*)", re.MULTILINE)
_TYPE_RE = re.compile(
    r"^\s*(?:(?:public|internal|private|protected|static|sealed|abstract|partial|readonly)\s+)*"
    r"(?:class|struct|interface|enum)\s+([A-Za-z_]\w*)",
    re.MULTILINE,
)


def _should_ignore(path: Path) -> bool:
    return any(part in IGNORED_PARTS for part in path.parts)


def _iter_source_files(root: Path, scan_roots: Iterable[str]) -> Iterable[Path]:
    seen: set[Path] = set()
    for raw in scan_roots:
        base = (root / raw).resolve()
        if not base.exists():
            continue
        if base.is_file():
            candidates = (base,)
        else:
            candidates = base.rglob("*")
        for path in candidates:
            if not path.is_file() or path.suffix.lower() not in SOURCE_SUFFIXES:
                continue
            try:
                relative = path.resolve().relative_to(root.resolve())
            except ValueError:
                continue
            if _should_ignore(relative) or path in seen:
                continue
            seen.add(path)
            yield path


def _symbol_for_text(text: str) -> str:
    namespace_match = _NAMESPACE_RE.search(text)
    type_match = _TYPE_RE.search(text)
    namespace = namespace_match.group(1) if namespace_match else ""
    type_name = type_match.group(1) if type_match else ""
    if namespace and type_name:
        return namespace + "." + type_name
    return type_name or namespace


def _line_number(text: str, offset: int) -> int:
    return text.count("\n", 0, offset) + 1


def _line_snippet(text: str, offset: int) -> str:
    start = text.rfind("\n", 0, offset) + 1
    end = text.find("\n", offset)
    if end < 0:
        end = len(text)
    return " ".join(text[start:end].strip().split())[:240]


def scan_text(path: str, text: str) -> tuple[Finding, ...]:
    symbol = _symbol_for_text(text)
    findings: list[Finding] = []
    seen: set[tuple[str, int]] = set()
    for rule, regex in _COMPILED_RULES:
        for match in regex.finditer(text):
            line = _line_number(text, match.start())
            key = (rule.code, line)
            if key in seen:
                continue
            seen.add(key)
            findings.append(
                Finding(
                    category=rule.category,
                    code=rule.code,
                    description=rule.description,
                    path=path.replace("\\", "/"),
                    line=line,
                    symbol=symbol,
                    snippet=_line_snippet(text, match.start()),
                )
            )
    findings.sort(key=lambda item: (item.path, item.line, item.code))
    return tuple(findings)


def scan_repository(root: Path, scan_roots: Sequence[str]) -> ScanResult:
    root = root.resolve()
    findings: list[Finding] = []
    scanned_files = 0
    for path in sorted(_iter_source_files(root, scan_roots)):
        try:
            text = path.read_text(encoding="utf-8")
        except UnicodeDecodeError:
            text = path.read_text(encoding="utf-8", errors="replace")
        except OSError as exc:
            raise RuntimeError(f"Could not read {path}: {exc}") from exc
        scanned_files += 1
        relative = str(path.resolve().relative_to(root)).replace("\\", "/")
        findings.extend(scan_text(relative, text))
    findings.sort(key=lambda item: (item.category, item.code, item.path, item.line))
    return ScanResult(
        root=str(root),
        scan_roots=tuple(scan_roots),
        scanned_files=scanned_files,
        findings=tuple(findings),
    )


def write_json_report(result: ScanResult, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")


def write_markdown_report(result: ScanResult, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Recovery Contract Scan",
        "",
        f"- Scanned files: **{result.scanned_files}**",
        f"- Findings: **{len(result.findings)}**",
        f"- Scan roots: `{', '.join(result.scan_roots)}`",
        "",
        "## Category counts",
        "",
        "| Category | Count |",
        "|---|---:|",
    ]
    for category, count in result.category_counts.items():
        lines.append(f"| `{category}` | {count} |")
    lines.extend(["", "## Findings", ""])
    current_category = None
    for finding in result.findings:
        if finding.category != current_category:
            current_category = finding.category
            lines.extend([f"### {current_category}", ""])
        symbol = f" · `{finding.symbol}`" if finding.symbol else ""
        lines.append(
            f"- **{finding.code}** — `{finding.path}:{finding.line}`{symbol} — "
            f"{finding.description}. `{finding.snippet}`"
        )
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve_under_root(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parents[1],
        help="Repository root. Defaults to the parent of tools/.",
    )
    parser.add_argument(
        "--scan-root",
        action="append",
        dest="scan_roots",
        help="Repository-relative source root. Repeatable. Defaults to ZIPTIDE runtime roots.",
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_contract_scan.json"),
        help="JSON report path relative to --root.",
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_contract_scan.md"),
        help="Markdown report path relative to --root.",
    )
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    scan_roots = tuple(args.scan_roots or DEFAULT_SCAN_ROOTS)
    try:
        result = scan_repository(root, scan_roots)
        json_path = _resolve_under_root(root, args.json_report).resolve()
        markdown_path = _resolve_under_root(root, args.markdown_report).resolve()
        for report_path in (json_path, markdown_path):
            report_path.relative_to(root)
        write_json_report(result, json_path)
        write_markdown_report(result, markdown_path)
    except (OSError, RuntimeError, ValueError) as exc:
        print(f"RECOVERY_SCAN_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    print(
        "RECOVERY_CONTRACT_SCAN "
        f"files={result.scanned_files} findings={len(result.findings)} "
        f"categories={len(result.category_counts)}"
    )
    for category, count in result.category_counts.items():
        print(f"RECOVERY_CONTRACT_CATEGORY name={category} count={count}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
