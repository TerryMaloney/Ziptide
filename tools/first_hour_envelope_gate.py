#!/usr/bin/env python3
"""Validate ZIPTIDE first-hour owner envelopes. Default is report-only."""

from __future__ import annotations

import argparse
import json
import sys
from collections import Counter
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Iterable, Sequence

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    envelope_id: str = ""
    path: str = ""


@dataclass(frozen=True)
class Result:
    root: str
    envelope_path: str
    envelope_count: int
    covered_beats: int
    covered_lines: int
    owner_counts: dict[str, int]
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "first_hour_envelope_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "envelopePath": self.envelope_path,
            "envelopeCount": self.envelope_count,
            "coveredBeatCount": self.covered_beats,
            "coveredLineCount": self.covered_lines,
            "ownerCounts": self.owner_counts,
            "findingCount": len(self.findings),
            "findings": [asdict(x) for x in self.findings],
        }


class LoadError(RuntimeError):
    pass


def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _load(path: Path, label: str) -> dict[str, Any]:
    try:
        raw = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise LoadError(f"Could not read {label}: {exc}") from exc
    try:
        data = json.loads(raw)
    except json.JSONDecodeError as exc:
        raise LoadError(f"{label} invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}") from exc
    if not isinstance(data, dict):
        raise LoadError(f"{label} root must be an object.")
    return data


def _strings(value: Any, field: str, findings: list[Finding], envelope_id: str = "", required: bool = False) -> list[str]:
    if value is None and not required:
        return []
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list.", envelope_id))
        return []
    result: list[str] = []
    for index, item in enumerate(value):
        if not _text(item):
            findings.append(Finding("FIELD_ITEM_INVALID", f"{field}[{index}] must be non-empty.", envelope_id))
        else:
            result.append(item.strip())
    if required and not result:
        findings.append(Finding("FIELD_REQUIRED", f"{field} needs at least one item.", envelope_id))
    return result


def _duplicates(values: Iterable[str]) -> set[str]:
    counts = Counter(values)
    return {value for value, count in counts.items() if count > 1}


def _resolve(root: Path, raw: Any, field: str) -> tuple[Path | None, Finding | None]:
    if not _text(raw):
        return None, Finding("PATH_INVALID", f"{field} must be non-empty.")
    shown = str(raw).replace("\\", "/").strip()
    path = Path(shown)
    if path.is_absolute():
        return None, Finding("PATH_ABSOLUTE", f"{field} must be repository-relative.", path=shown)
    resolved = (root / path).resolve()
    try:
        resolved.relative_to(root.resolve())
    except ValueError:
        return None, Finding("PATH_OUTSIDE_ROOT", f"{field} escapes repository root.", path=shown)
    return resolved, None


def _cycle(graph: dict[str, list[str]]) -> list[str]:
    marks: dict[str, int] = {}
    stack: list[str] = []

    def visit(node: str) -> list[str]:
        if marks.get(node) == 1:
            at = stack.index(node) if node in stack else 0
            return stack[at:] + [node]
        if marks.get(node) == 2:
            return []
        marks[node] = 1
        stack.append(node)
        for dep in graph.get(node, []):
            found = visit(dep)
            if found:
                return found
        stack.pop()
        marks[node] = 2
        return []

    for node in graph:
        found = visit(node)
        if found:
            return found
    return []


def validate(root: Path, envelope_path: Path) -> Result:
    root = root.resolve()
    package = _load(envelope_path.resolve(), "envelope package")
    findings: list[Finding] = []

    if package.get("schemaVersion") != 1:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))

    contract_path, issue = _resolve(root, package.get("contractPath"), "contractPath")
    if issue:
        findings.append(issue)
        contract = {}
    elif contract_path is None or not contract_path.is_file():
        findings.append(Finding("CONTRACT_MISSING", "contractPath does not exist.", path=str(package.get("contractPath"))))
        contract = {}
    else:
        contract = _load(contract_path, "contract")

    binding_path, issue = _resolve(root, package.get("bindingInventoryPath"), "bindingInventoryPath")
    if issue:
        findings.append(issue)
        inventory = {}
    elif binding_path is None or not binding_path.is_file():
        findings.append(Finding("BINDING_MISSING", "bindingInventoryPath does not exist.", path=str(package.get("bindingInventoryPath"))))
        inventory = {}
    else:
        inventory = _load(binding_path, "binding inventory")

    owners = set(_strings(package.get("owners"), "owners", findings, required=True))
    direct = set(_strings(package.get("directReuse"), "directReuse", findings, required=True))
    _strings(package.get("rules"), "rules", findings, required=True)

    beats = {
        row.get("id"): row
        for row in contract.get("beats", [])
        if isinstance(row, dict) and _text(row.get("id"))
    }
    signals = {beat: row.get("completionSignal", {}).get("id") for beat, row in beats.items()}
    contract_lines = {
        beat: row.get("rillLine", {}).get("id")
        for beat, row in beats.items()
        if _text(row.get("rillLine", {}).get("id"))
    }

    binding_status = {
        row.get("beatId"): row.get("status")
        for row in inventory.get("bindings", [])
        if isinstance(row, dict) and _text(row.get("beatId"))
    }
    inventory_lines = {
        row.get("beatId"): row.get("contractLineId")
        for row in inventory.get("rillLines", [])
        if isinstance(row, dict) and _text(row.get("beatId"))
    }
    verified = {beat for beat, status in binding_status.items() if status == "verified-existing"}
    if direct != verified:
        findings.append(Finding("DIRECT_REUSE_MISMATCH", f"Expected {sorted(verified)}, got {sorted(direct)}."))

    rows: list[dict[str, Any]] = []
    envelope_files = _strings(package.get("envelopeFiles"), "envelopeFiles", findings, required=True)
    for index, raw_path in enumerate(envelope_files):
        resolved, path_issue = _resolve(root, raw_path, f"envelopeFiles[{index}]")
        if path_issue:
            findings.append(path_issue)
            continue
        if resolved is None or not resolved.is_file():
            findings.append(Finding("ENVELOPE_FILE_MISSING", "Envelope file does not exist.", path=raw_path))
            continue
        try:
            row = _load(resolved, f"envelope file {raw_path}")
        except LoadError as exc:
            findings.append(Finding("ENVELOPE_FILE_INVALID", str(exc), path=raw_path))
            continue
        row["_sourcePath"] = raw_path
        rows.append(row)
    if not rows:
        findings.append(Finding("ENVELOPES_REQUIRED", "No envelope files could be loaded."))

    ids: list[str] = []
    graph: dict[str, list[str]] = {}
    owner_counts: Counter[str] = Counter()
    covered_beats: set[str] = set()
    covered_line_beats: set[str] = set()
    covered_line_ids: set[str] = set()

    for index, row in enumerate(rows):
        prefix = f"envelopes[{index}]"
        if not isinstance(row, dict):
            findings.append(Finding("ENVELOPE_INVALID", f"{prefix} must be an object."))
            continue
        envelope_id = row.get("id")
        if not _text(envelope_id):
            findings.append(Finding("ENVELOPE_ID_INVALID", f"{prefix}.id is required."))
            envelope_id = f"<invalid-{index}>"
        else:
            envelope_id = envelope_id.strip()
            ids.append(envelope_id)

        owner = row.get("owner")
        if owner not in owners:
            findings.append(Finding("OWNER_UNKNOWN", f"Unknown owner {owner!r}.", envelope_id))
        else:
            owner_counts[str(owner)] += 1

        integration = _strings(row.get("integrationOwners"), f"{prefix}.integrationOwners", findings, envelope_id)
        for item in integration:
            if item not in owners:
                findings.append(Finding("OWNER_UNKNOWN", f"Unknown integration owner {item!r}.", envelope_id))
            if item == owner:
                findings.append(Finding("OWNER_REPEATED", "integrationOwners repeats owner.", envelope_id))

        kind = row.get("kind")
        risk = row.get("risk")
        if kind not in {"contract-compiler", "pure-core", "adapter", "surface", "art-content", "orchestration"}:
            findings.append(Finding("KIND_UNKNOWN", f"Unknown kind {kind!r}.", envelope_id))
        if risk not in {"owner-local", "cross-lane", "shared-protected", "device-sensitive"}:
            findings.append(Finding("RISK_UNKNOWN", f"Unknown risk {risk!r}.", envelope_id))
        if risk in {"cross-lane", "shared-protected"} and not integration:
            findings.append(Finding("INTEGRATION_OWNER_REQUIRED", f"{risk} requires integrationOwners.", envelope_id))
        if risk == "shared-protected" and not _text(row.get("sharedProtocol")):
            findings.append(Finding("SHARED_PROTOCOL_REQUIRED", "sharedProtocol is required.", envelope_id))

        covers = _strings(row.get("covers"), f"{prefix}.covers", findings, envelope_id)
        row_signals = _strings(row.get("signals"), f"{prefix}.signals", findings, envelope_id)
        supports = _strings(row.get("supports"), f"{prefix}.supports", findings, envelope_id)
        for beat in covers + supports:
            if beat not in beats:
                findings.append(Finding("BEAT_UNKNOWN", f"Unknown beat {beat!r}.", envelope_id))
        expected_signals = [signals.get(beat) for beat in covers]
        if row_signals != expected_signals:
            findings.append(Finding("SIGNAL_MISMATCH", f"Expected {expected_signals}, got {row_signals}.", envelope_id))
        for beat in covers:
            covered_beats.add(beat)
            if beat in verified:
                findings.append(Finding("DIRECT_BEAT_REIMPLEMENTED", f"{beat} is verified-existing.", envelope_id))

        lines = row.get("lines", [])
        if not isinstance(lines, list):
            findings.append(Finding("LINES_INVALID", "lines must be a list.", envelope_id))
            lines = []
        for line_index, line in enumerate(lines):
            if not isinstance(line, dict):
                findings.append(Finding("LINE_INVALID", f"lines[{line_index}] must be an object.", envelope_id))
                continue
            beat = line.get("beat")
            line_id = line.get("id")
            if beat not in inventory_lines:
                findings.append(Finding("LINE_BEAT_UNKNOWN", f"Unknown line beat {beat!r}.", envelope_id))
                continue
            if line_id != inventory_lines.get(beat) or line_id != contract_lines.get(beat):
                findings.append(Finding("LINE_ID_MISMATCH", f"Wrong line ID for {beat}.", envelope_id))
            covered_line_beats.add(beat)
            if _text(line_id):
                covered_line_ids.add(line_id)

        deps = _strings(row.get("dependencies"), f"{prefix}.dependencies", findings, envelope_id)
        graph[envelope_id] = deps
        commits = row.get("commits")
        if not isinstance(commits, int) or isinstance(commits, bool) or not 1 <= commits <= 4:
            findings.append(Finding("COMMIT_BUDGET_INVALID", "commits must be 1..4.", envelope_id))

        new_files = _strings(row.get("newFiles"), f"{prefix}.newFiles", findings, envelope_id)
        touch_files = _strings(row.get("touchFiles"), f"{prefix}.touchFiles", findings, envelope_id)
        patterns = _strings(row.get("touchPatterns"), f"{prefix}.touchPatterns", findings, envelope_id)
        if not new_files and not touch_files and not patterns:
            findings.append(Finding("FILE_SCOPE_REQUIRED", "Envelope has no file scope.", envelope_id))
        for raw in new_files + touch_files:
            resolved, path_issue = _resolve(root, raw, "file scope")
            if path_issue:
                findings.append(Finding(path_issue.code, path_issue.message, envelope_id, path_issue.path))
                continue
            if raw.endswith((".unity", ".prefab")):
                findings.append(Finding("SCENE_YAML_FORBIDDEN", "Scene/prefab YAML is forbidden.", envelope_id, raw))
            if raw in touch_files and resolved is not None and not resolved.is_file():
                findings.append(Finding("TOUCH_FILE_MISSING", "Existing touch file does not exist.", envelope_id, raw))
        for pattern in patterns:
            if Path(pattern).is_absolute() or ".." in Path(pattern).parts or pattern.endswith((".unity", ".prefab")):
                findings.append(Finding("TOUCH_PATTERN_INVALID", "Unsafe touch pattern.", envelope_id, pattern))

        for field in ("reuse", "api", "tests", "diagnostics", "fallback"):
            _strings(row.get(field), f"{prefix}.{field}", findings, envelope_id, required=True)

        acceptance = row.get("acceptance")
        if not isinstance(acceptance, dict):
            findings.append(Finding("ACCEPTANCE_INVALID", "acceptance must be an object.", envelope_id))
        else:
            total = 0
            for label in ("CI", "BAKE", "DEVICE"):
                total += len(_strings(acceptance.get(label), f"{prefix}.acceptance.{label}", findings, envelope_id))
            if total == 0:
                findings.append(Finding("ACCEPTANCE_EMPTY", "At least one acceptance item is required.", envelope_id))
            if risk == "device-sensitive" and not acceptance.get("DEVICE"):
                findings.append(Finding("DEVICE_ACCEPTANCE_REQUIRED", "Device-sensitive work needs DEVICE acceptance.", envelope_id))

    for duplicate in sorted(_duplicates(ids)):
        findings.append(Finding("ENVELOPE_ID_DUPLICATE", "Envelope ID is duplicated.", duplicate))
    known = set(ids)
    for envelope_id, deps in graph.items():
        for dep in deps:
            if dep not in known:
                findings.append(Finding("DEPENDENCY_UNKNOWN", f"Unknown dependency {dep!r}.", envelope_id))
            if dep == envelope_id:
                findings.append(Finding("DEPENDENCY_SELF", "Self dependency.", envelope_id))
    found_cycle = _cycle({node: [dep for dep in deps if dep in known] for node, deps in graph.items()})
    if found_cycle:
        findings.append(Finding("DEPENDENCY_CYCLE", " -> ".join(found_cycle)))

    required_beats = {beat for beat, status in binding_status.items() if status != "verified-existing"}
    for beat in sorted(required_beats - covered_beats):
        findings.append(Finding("BEAT_ENVELOPE_MISSING", "Non-direct beat is uncovered.", beat))
    for beat in sorted(covered_beats - required_beats):
        findings.append(Finding("BEAT_ENVELOPE_UNEXPECTED", "Covered beat is not non-direct.", beat))
    for beat in sorted(set(inventory_lines) - covered_line_beats):
        findings.append(Finding("LINE_ENVELOPE_MISSING", "Teaching line is uncovered.", beat))
    for line_id in sorted(set(inventory_lines.values()) - covered_line_ids):
        findings.append(Finding("LINE_ID_ENVELOPE_MISSING", "Teaching line ID is uncovered.", path=str(line_id)))

    return Result(
        root=str(root),
        envelope_path=str(envelope_path.resolve()),
        envelope_count=len(rows),
        covered_beats=len(covered_beats),
        covered_lines=len(covered_line_beats),
        owner_counts=dict(sorted(owner_counts.items())),
        findings=tuple(findings),
    )


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--envelopes", type=Path, default=Path("docs/first_hour/adapter_envelopes.json"))
    parser.add_argument("--json-report", type=Path)
    parser.add_argument("--strict", action="store_true")
    args = parser.parse_args(argv)
    root = args.root.resolve()
    path = args.envelopes if args.envelopes.is_absolute() else root / args.envelopes
    try:
        result = validate(root, path)
    except LoadError as exc:
        print(f"FIRST_HOUR_ENVELOPE_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report = args.json_report if args.json_report.is_absolute() else root / args.json_report
        try:
            report = report.resolve()
            report.relative_to(root)
            report.parent.mkdir(parents=True, exist_ok=True)
            report.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")
        except (OSError, ValueError) as exc:
            print(f"FIRST_HOUR_ENVELOPE_ERROR Could not write report: {exc}", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR

    print(
        "FIRST_HOUR_ENVELOPE_REPORT "
        f"status={result.status.upper()} findings={len(result.findings)} "
        f"envelopes={result.envelope_count} beats={result.covered_beats} lines={result.covered_lines}"
    )
    for finding in result.findings:
        envelope = f" envelope={finding.envelope_id}" if finding.envelope_id else ""
        path_text = f" path={finding.path}" if finding.path else ""
        print(f"FIRST_HOUR_ENVELOPE_FINDING code={finding.code}{envelope}{path_text} message={finding.message}")

    return EXIT_VALIDATION_FAILED if args.strict and result.findings else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
