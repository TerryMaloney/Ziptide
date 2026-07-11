#!/usr/bin/env python3
"""Validate ZIPTIDE's first-hour binding inventory against repository evidence.

Default mode is report-only. --strict is reserved for a later explicit ratchet.
"""

from __future__ import annotations

import argparse
import json
import sys
from collections import Counter
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Sequence

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    beat_id: str = ""
    path: str = ""
    severity: str = "warning"


@dataclass(frozen=True)
class ValidationResult:
    inventory_path: str
    contract_path: str
    root: str
    binding_count: int
    line_binding_count: int
    status_counts: dict[str, int]
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "first_hour_binding_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "inventoryPath": self.inventory_path,
            "contractPath": self.contract_path,
            "bindingCount": self.binding_count,
            "lineBindingCount": self.line_binding_count,
            "statusCounts": self.status_counts,
            "findingCount": len(self.findings),
            "findings": [asdict(finding) for finding in self.findings],
        }


class InventoryLoadError(RuntimeError):
    pass


def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _load(path: Path, label: str) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise InventoryLoadError(f"Could not read {label}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise InventoryLoadError(
            f"{label} is invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(data, dict):
        raise InventoryLoadError(f"{label} root must be an object.")
    return data


def _safe_path(root: Path, raw: Any, field: str) -> tuple[Path | None, Finding | None]:
    if not _text(raw):
        return None, Finding("EVIDENCE_PATH_INVALID", f"{field} must be non-empty.")
    shown = str(raw).replace("\\", "/").strip()
    candidate = Path(shown)
    if candidate.is_absolute():
        return None, Finding("EVIDENCE_PATH_ABSOLUTE", f"{field} must be relative.", path=shown)
    resolved = (root / candidate).resolve()
    try:
        resolved.relative_to(root.resolve())
    except ValueError:
        return None, Finding("EVIDENCE_PATH_OUTSIDE_ROOT", f"{field} escapes root.", path=shown)
    return resolved, None


def _strings(
    value: Any,
    field: str,
    findings: list[Finding],
    beat_id: str = "",
    required: bool = False,
) -> list[str]:
    if value is None and not required:
        return []
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list.", beat_id))
        return []
    result: list[str] = []
    for index, item in enumerate(value):
        if not _text(item):
            findings.append(
                Finding("FIELD_ITEM_INVALID", f"{field}[{index}] must be non-empty.", beat_id)
            )
        else:
            result.append(item.strip())
    if required and not result:
        findings.append(Finding("FIELD_REQUIRED", f"{field} requires an item.", beat_id))
    return result


def _evidence(
    root: Path,
    value: Any,
    field: str,
    findings: list[Finding],
    beat_id: str,
) -> int:
    if not isinstance(value, list):
        findings.append(Finding("EVIDENCE_LIST_INVALID", f"{field} must be a list.", beat_id))
        return 0
    if not value:
        findings.append(Finding("EVIDENCE_REQUIRED", f"{field} requires evidence.", beat_id))
        return 0

    valid = 0
    for index, entry in enumerate(value):
        prefix = f"{field}[{index}]"
        if not isinstance(entry, dict):
            findings.append(Finding("EVIDENCE_INVALID", f"{prefix} must be an object.", beat_id))
            continue
        path, issue = _safe_path(root, entry.get("path"), f"{prefix}.path")
        if issue:
            findings.append(Finding(issue.code, issue.message, beat_id, issue.path))
            continue
        assert path is not None
        shown = str(entry.get("path"))
        if not path.is_file():
            findings.append(Finding("EVIDENCE_PATH_MISSING", f"{prefix} is missing.", beat_id, shown))
            continue
        tokens = _strings(entry.get("tokens"), f"{prefix}.tokens", findings, beat_id, True)
        try:
            source = path.read_text(encoding="utf-8")
        except (OSError, UnicodeError) as exc:
            findings.append(Finding("EVIDENCE_READ_FAILED", str(exc), beat_id, shown))
            continue
        missing = False
        for token in tokens:
            if token not in source:
                missing = True
                findings.append(
                    Finding(
                        "EVIDENCE_TOKEN_MISSING",
                        f"Token not found: {token!r}.",
                        beat_id,
                        shown,
                    )
                )
        if tokens and not missing:
            valid += 1
    return valid


def _duplicates(values: list[str]) -> set[str]:
    return {value for value in values if values.count(value) > 1}


def validate_inventory(root: Path, inventory_path: Path) -> ValidationResult:
    root = root.resolve()
    inventory_path = inventory_path.resolve()
    inventory = _load(inventory_path, "binding inventory")
    findings: list[Finding] = []

    if inventory.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))

    contract_path, issue = _safe_path(root, inventory.get("contractPath"), "contractPath")
    if issue:
        findings.append(issue)
        contract_path = root / "__invalid_contract__"
        contract = {}
    elif contract_path is None or not contract_path.is_file():
        findings.append(
            Finding("CONTRACT_PATH_MISSING", "contractPath does not exist.", path=str(inventory.get("contractPath")))
        )
        contract = {}
    else:
        contract = _load(contract_path, "first-hour contract")

    allowed_status = set(_strings(inventory.get("allowedBindingStatuses"), "allowedBindingStatuses", findings, required=True))
    allowed_line_status = set(_strings(inventory.get("allowedLineStatuses"), "allowedLineStatuses", findings, required=True))
    allowed_owner = set(_strings(inventory.get("allowedOwners"), "allowedOwners", findings, required=True))

    contract_beats = contract.get("beats", [])
    if not isinstance(contract_beats, list):
        findings.append(Finding("CONTRACT_BEATS_INVALID", "Contract beats must be a list."))
        contract_beats = []
    contract_by_id = {
        beat["id"]: beat
        for beat in contract_beats
        if isinstance(beat, dict) and _text(beat.get("id"))
    }
    teaching = {
        beat_id: beat
        for beat_id, beat in contract_by_id.items()
        if _text(beat.get("teachesVerb"))
    }

    bindings = inventory.get("bindings", [])
    if not isinstance(bindings, list):
        findings.append(Finding("BINDINGS_INVALID", "bindings must be a list."))
        bindings = []
    binding_ids: list[str] = []
    counts: Counter[str] = Counter()

    for index, binding in enumerate(bindings):
        if not isinstance(binding, dict):
            findings.append(Finding("BINDING_INVALID", f"bindings[{index}] must be an object."))
            continue
        beat_id = binding.get("beatId")
        if not _text(beat_id):
            findings.append(Finding("BINDING_BEAT_ID_INVALID", "beatId is required."))
            beat_id = f"<invalid-{index}>"
        else:
            beat_id = beat_id.strip()
            binding_ids.append(beat_id)

        beat = contract_by_id.get(beat_id)
        if beat is None:
            findings.append(Finding("BINDING_BEAT_UNKNOWN", "beatId is not in contract.", beat_id))
        else:
            expected = beat.get("completionSignal", {}).get("id")
            if binding.get("signalId") != expected:
                findings.append(
                    Finding(
                        "BINDING_SIGNAL_MISMATCH",
                        f"Expected {expected!r}; got {binding.get('signalId')!r}.",
                        beat_id,
                    )
                )

        status = binding.get("status")
        if status not in allowed_status:
            findings.append(Finding("BINDING_STATUS_UNKNOWN", f"Unknown status {status!r}.", beat_id))
        else:
            counts[str(status)] += 1
        if binding.get("owner") not in allowed_owner:
            findings.append(Finding("BINDING_OWNER_UNKNOWN", f"Unknown owner {binding.get('owner')!r}.", beat_id))
        for owner in _strings(binding.get("integrationOwners"), f"bindings[{index}].integrationOwners", findings, beat_id):
            if owner not in allowed_owner:
                findings.append(Finding("BINDING_OWNER_UNKNOWN", f"Unknown owner {owner!r}.", beat_id))

        if not _text(binding.get("recommendedBinding")):
            findings.append(Finding("RECOMMENDATION_REQUIRED", "recommendedBinding is required.", beat_id))
        _strings(binding.get("reuseOwners"), f"bindings[{index}].reuseOwners", findings, beat_id, True)
        _strings(binding.get("doNotDuplicate"), f"bindings[{index}].doNotDuplicate", findings, beat_id, True)
        valid_evidence = _evidence(root, binding.get("evidence"), f"bindings[{index}].evidence", findings, beat_id)
        if status == "verified-existing" and valid_evidence == 0:
            findings.append(
                Finding("VERIFIED_BINDING_WITHOUT_EVIDENCE", "verified-existing requires direct evidence.", beat_id)
            )

    for duplicate in sorted(_duplicates(binding_ids)):
        findings.append(Finding("BINDING_BEAT_DUPLICATE", "Beat has multiple bindings.", duplicate))
    for beat_id in sorted(set(contract_by_id) - set(binding_ids)):
        findings.append(Finding("BINDING_BEAT_MISSING", "Beat has no binding.", beat_id))

    lines = inventory.get("rillLines", [])
    if not isinstance(lines, list):
        findings.append(Finding("LINE_BINDINGS_INVALID", "rillLines must be a list."))
        lines = []
    line_ids: list[str] = []
    for index, line in enumerate(lines):
        if not isinstance(line, dict):
            findings.append(Finding("LINE_BINDING_INVALID", f"rillLines[{index}] must be an object."))
            continue
        beat_id = line.get("beatId")
        if not _text(beat_id):
            findings.append(Finding("LINE_BEAT_ID_INVALID", "beatId is required."))
            beat_id = f"<invalid-line-{index}>"
        else:
            beat_id = beat_id.strip()
            line_ids.append(beat_id)

        beat = teaching.get(beat_id)
        if beat is None:
            findings.append(Finding("LINE_BEAT_UNKNOWN", "beatId is not a teaching beat.", beat_id))
        else:
            expected = beat.get("rillLine", {}).get("id")
            if line.get("contractLineId") != expected:
                findings.append(
                    Finding("LINE_ID_MISMATCH", f"Expected {expected!r}; got {line.get('contractLineId')!r}.", beat_id)
                )

        status = line.get("status")
        if status not in allowed_line_status:
            findings.append(Finding("LINE_STATUS_UNKNOWN", f"Unknown line status {status!r}.", beat_id))
        if line.get("owner") not in allowed_owner:
            findings.append(Finding("BINDING_OWNER_UNKNOWN", f"Unknown owner {line.get('owner')!r}.", beat_id))
        for owner in _strings(line.get("integrationOwners"), f"rillLines[{index}].integrationOwners", findings, beat_id):
            if owner not in allowed_owner:
                findings.append(Finding("BINDING_OWNER_UNKNOWN", f"Unknown owner {owner!r}.", beat_id))

        existing = line.get("existingLineId")
        if status in {"verified-existing", "reuse-candidate"} and not _text(existing):
            findings.append(Finding("EXISTING_LINE_ID_REQUIRED", f"{status} requires existingLineId.", beat_id))
        if status == "content-required" and existing not in (None, ""):
            findings.append(Finding("CONTENT_REQUIRED_HAS_EXISTING_ID", "content-required cannot claim existingLineId.", beat_id))
        if not _text(line.get("recommendedBinding")):
            findings.append(Finding("RECOMMENDATION_REQUIRED", "recommendedBinding is required.", beat_id))
        _evidence(root, line.get("evidence"), f"rillLines[{index}].evidence", findings, beat_id)

    for duplicate in sorted(_duplicates(line_ids)):
        findings.append(Finding("LINE_BEAT_DUPLICATE", "Teaching beat has multiple lines.", duplicate))
    for beat_id in sorted(set(teaching) - set(line_ids)):
        findings.append(Finding("LINE_BEAT_MISSING", "Teaching beat has no line binding.", beat_id))

    decisions = inventory.get("globalDecisions", [])
    if not isinstance(decisions, list):
        findings.append(Finding("GLOBAL_DECISIONS_INVALID", "globalDecisions must be a list."))
        decisions = []
    decision_ids: list[str] = []
    for index, decision in enumerate(decisions):
        if not isinstance(decision, dict):
            findings.append(Finding("GLOBAL_DECISION_INVALID", f"globalDecisions[{index}] must be an object."))
            continue
        decision_id = decision.get("id")
        if not _text(decision_id):
            findings.append(Finding("GLOBAL_DECISION_ID_INVALID", "Decision id is required."))
            decision_id = f"<invalid-decision-{index}>"
        else:
            decision_ids.append(decision_id.strip())
        if not _text(decision.get("decision")):
            findings.append(Finding("GLOBAL_DECISION_TEXT_INVALID", "Decision text is required.", path=str(decision_id)))
        _evidence(root, decision.get("evidence"), f"globalDecisions[{index}].evidence", findings, str(decision_id))
    for duplicate in sorted(_duplicates(decision_ids)):
        findings.append(Finding("GLOBAL_DECISION_ID_DUPLICATE", "Decision id is duplicated.", path=duplicate))

    return ValidationResult(
        inventory_path=str(inventory_path),
        contract_path=str(contract_path),
        root=str(root),
        binding_count=len(bindings),
        line_binding_count=len(lines),
        status_counts=dict(sorted(counts.items())),
        findings=tuple(findings),
    )


def write_report(result: ValidationResult, report_path: Path) -> None:
    report_path.parent.mkdir(parents=True, exist_ok=True)
    report_path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--inventory", type=Path, default=Path("docs/first_hour/first_hour_bindings.json"))
    parser.add_argument("--json-report", type=Path)
    parser.add_argument("--strict", action="store_true")
    args = parser.parse_args(argv)

    root = args.root.resolve()
    inventory = args.inventory if args.inventory.is_absolute() else root / args.inventory
    try:
        result = validate_inventory(root, inventory)
    except InventoryLoadError as exc:
        print(f"FIRST_HOUR_BINDING_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report = args.json_report if args.json_report.is_absolute() else root / args.json_report
        try:
            report.resolve().relative_to(root)
            write_report(result, report.resolve())
        except ValueError:
            print("FIRST_HOUR_BINDING_ERROR report path must remain inside root.", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR
        except OSError as exc:
            print(f"FIRST_HOUR_BINDING_ERROR Could not write report: {exc}", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR

    print(
        "FIRST_HOUR_BINDING_REPORT "
        f"status={result.status.upper()} findings={len(result.findings)} "
        f"bindings={result.binding_count} lines={result.line_binding_count}"
    )
    for finding in result.findings:
        beat = f" beat={finding.beat_id}" if finding.beat_id else ""
        path = f" path={finding.path}" if finding.path else ""
        print(f"FIRST_HOUR_BINDING_FINDING code={finding.code}{beat}{path} message={finding.message}")

    return EXIT_VALIDATION_FAILED if args.strict and result.findings else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
