#!/usr/bin/env python3
"""Validate ZIPTIDE's recovery system-contract inventory.

Default mode is report-only. The gate checks that the new recovery ledger does not
repeat the old project's ambiguous completion claims: source paths must resolve,
canonical ownership must be explicit, proof labels must be valid, and failed Quest
observations cannot also be recorded as accepted QUEST proof.
"""

from __future__ import annotations

import argparse
import json
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Sequence

TOOL_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2

PROOF_LEVELS = ("SOURCE", "CORE", "PATCHED", "PLAYMODE", "VISUAL", "APK", "QUEST")
EXPOSURE_CLASSES = (
    "GOLDEN_PATH",
    "SUPPORT",
    "PROTOTYPE_HIDDEN",
    "DIAGNOSTIC",
    "REPLACE_OR_MERGE",
    "UNCLASSIFIED",
)
QUEST_PASS_STATUSES = {"PASS", "QUEST_PROVEN", "ACCEPTED"}
UNRESOLVED_SOURCE_TOKENS = (
    "exact path",
    "path to",
    "to resolve",
    "resolve in",
    "files (",
    "runtime files",
    "repair machine",
    "objective board runtime",
    "resource bank",
    "cast-off observer",
)
REQUIRED_SYSTEM_FIELDS = (
    "id",
    "responsibility",
    "canonicalCandidate",
    "sourceFiles",
    "startup",
    "persistence",
    "runtimeCreates",
    "state",
    "currentProof",
    "questStatus",
    "exposure",
    "conflicts",
    "nextAction",
)


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    system_id: str = ""
    path: str = ""
    severity: str = "warning"


@dataclass(frozen=True)
class ValidationResult:
    inventory_path: str
    root: str
    system_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "recovery_inventory_gate",
            "toolVersion": TOOL_VERSION,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "inventoryPath": self.inventory_path,
            "systemCount": self.system_count,
            "findingCount": len(self.findings),
            "findings": [asdict(f) for f in self.findings],
        }


class InventoryLoadError(RuntimeError):
    pass


def _nonempty(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _is_failed_quest_status(status: str) -> bool:
    upper = status.upper()
    if upper in QUEST_PASS_STATUSES:
        return False
    failure_markers = (
        "FAIL",
        "BROKEN",
        "UNVERIFIED",
        "PARTIAL",
        "PAUSED",
        "OBSERVED",
        "INTRUSIVE",
        "MIXED",
        "STRUCTURAL_ONLY",
        "SOAK_UNVERIFIED",
        "INTERIM",
        "GLITCHY",
        "OPAQUE",
        "FLOATING",
    )
    return any(marker in upper for marker in failure_markers)


def load_inventory(path: Path) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise InventoryLoadError(f"Could not read inventory: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise InventoryLoadError(
            f"Inventory is not valid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(data, dict):
        raise InventoryLoadError("Inventory root must be a JSON object.")
    return data


def _validate_string_list(
    value: Any,
    *,
    field: str,
    system_id: str,
    findings: list[Finding],
    required: bool = True,
) -> list[str]:
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list.", system_id))
        return []
    valid: list[str] = []
    for index, item in enumerate(value):
        if not _nonempty(item):
            findings.append(
                Finding("FIELD_ITEM_INVALID", f"{field}[{index}] must be a non-empty string.", system_id)
            )
        else:
            valid.append(item.strip())
    if required and not valid:
        findings.append(Finding("FIELD_REQUIRED", f"{field} must contain at least one item.", system_id))
    return valid


def _resolve_source(root: Path, raw: str) -> tuple[bool, bool]:
    """Return (resolved, wildcard)."""
    normalized = raw.replace("\\", "/").strip()
    wildcard = any(token in normalized for token in ("*", "?", "["))
    if wildcard:
        return any(root.glob(normalized)), True
    candidate = (root / normalized).resolve()
    try:
        candidate.relative_to(root.resolve())
    except ValueError:
        return False, False
    return candidate.is_file(), False


def validate_inventory(root: Path, inventory_path: Path) -> ValidationResult:
    root = root.resolve()
    inventory_path = inventory_path.resolve()
    inventory = load_inventory(inventory_path)
    findings: list[Finding] = []

    if inventory.get("schemaVersion") != 1:
        findings.append(
            Finding(
                "SCHEMA_VERSION_UNSUPPORTED",
                f"schemaVersion must equal 1; got {inventory.get('schemaVersion')!r}.",
            )
        )

    declared_proofs = inventory.get("proofLevels")
    if declared_proofs != list(PROOF_LEVELS):
        findings.append(
            Finding(
                "PROOF_LEVEL_DECLARATION_DRIFT",
                "proofLevels must exactly match the recovery program taxonomy.",
            )
        )

    declared_exposures = inventory.get("exposureClasses")
    if declared_exposures != list(EXPOSURE_CLASSES):
        findings.append(
            Finding(
                "EXPOSURE_DECLARATION_DRIFT",
                "exposureClasses must exactly match the recovery program taxonomy.",
            )
        )

    systems = inventory.get("systems")
    if not isinstance(systems, list) or not systems:
        findings.append(Finding("SYSTEMS_REQUIRED", "systems must be a non-empty list."))
        systems = []

    seen_ids: set[str] = set()
    for index, system in enumerate(systems):
        if not isinstance(system, dict):
            findings.append(Finding("SYSTEM_INVALID", f"systems[{index}] must be an object."))
            continue

        system_id = system.get("id", "")
        if not _nonempty(system_id):
            findings.append(Finding("SYSTEM_ID_INVALID", f"systems[{index}].id is required."))
            system_id = f"systems[{index}]"
        else:
            system_id = system_id.strip()
            if system_id in seen_ids:
                findings.append(
                    Finding("SYSTEM_ID_DUPLICATE", "System id appears more than once.", system_id)
                )
            seen_ids.add(system_id)

        for field in REQUIRED_SYSTEM_FIELDS:
            if field not in system:
                findings.append(Finding("FIELD_MISSING", f"Required field '{field}' is missing.", system_id))

        for field in (
            "responsibility",
            "canonicalCandidate",
            "startup",
            "persistence",
            "questStatus",
            "exposure",
            "nextAction",
        ):
            if field in system and not _nonempty(system.get(field)):
                findings.append(Finding("FIELD_INVALID", f"{field} must be non-empty.", system_id))

        candidate = str(system.get("canonicalCandidate", ""))
        if "undecided" in candidate.lower() or "unresolved" in candidate.lower():
            findings.append(
                Finding(
                    "CANONICAL_OWNER_UNRESOLVED",
                    "Canonical ownership is explicitly unresolved; R0 cannot close this system yet.",
                    system_id,
                )
            )

        exposure = system.get("exposure")
        if _nonempty(exposure) and exposure not in EXPOSURE_CLASSES:
            findings.append(
                Finding("EXPOSURE_INVALID", f"Unknown exposure class '{exposure}'.", system_id)
            )

        source_files = _validate_string_list(
            system.get("sourceFiles"),
            field="sourceFiles",
            system_id=system_id,
            findings=findings,
        )
        for source in source_files:
            lower = source.lower()
            if any(token in lower for token in UNRESOLVED_SOURCE_TOKENS):
                findings.append(
                    Finding(
                        "SOURCE_PATH_UNRESOLVED",
                        "Source entry is descriptive prose rather than a repository path.",
                        system_id,
                        source,
                    )
                )
                continue
            resolved, wildcard = _resolve_source(root, source)
            if not resolved:
                findings.append(
                    Finding(
                        "SOURCE_GLOB_EMPTY" if wildcard else "SOURCE_PATH_MISSING",
                        "Source path did not resolve in the repository.",
                        system_id,
                        source,
                    )
                )

        proofs = _validate_string_list(
            system.get("currentProof"),
            field="currentProof",
            system_id=system_id,
            findings=findings,
        )
        seen_proofs: set[str] = set()
        for proof in proofs:
            if proof not in PROOF_LEVELS:
                findings.append(Finding("PROOF_INVALID", f"Unknown proof level '{proof}'.", system_id))
            if proof in seen_proofs:
                findings.append(Finding("PROOF_DUPLICATE", f"Proof '{proof}' is duplicated.", system_id))
            seen_proofs.add(proof)

        quest_status = str(system.get("questStatus", ""))
        if "QUEST" in proofs and _is_failed_quest_status(quest_status):
            findings.append(
                Finding(
                    "PROOF_QUEST_CONTRADICTION",
                    f"QUEST is recorded as accepted proof but questStatus is '{quest_status}'.",
                    system_id,
                )
            )

        for field in ("runtimeCreates", "state", "conflicts"):
            if field in system:
                _validate_string_list(
                    system.get(field),
                    field=field,
                    system_id=system_id,
                    findings=findings,
                    required=True,
                )

    gaps = inventory.get("knownInventoryGaps")
    if not isinstance(gaps, list):
        findings.append(Finding("GAPS_INVALID", "knownInventoryGaps must be a list."))

    return ValidationResult(
        inventory_path=str(inventory_path),
        root=str(root),
        system_count=len(systems),
        findings=tuple(findings),
    )


def write_json_report(result: ValidationResult, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")


def write_markdown_report(result: ValidationResult, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Recovery Inventory Validation",
        "",
        f"- Systems: **{result.system_count}**",
        f"- Findings: **{len(result.findings)}**",
        f"- Status: **{result.status.upper()}**",
        "",
    ]
    if not result.findings:
        lines.append("No inventory contradictions found.")
    else:
        current = None
        for finding in result.findings:
            if finding.system_id != current:
                current = finding.system_id
                lines.extend([f"## {current or 'Repository-level'}", ""])
            suffix = f" · `{finding.path}`" if finding.path else ""
            lines.append(f"- **{finding.code}**{suffix} — {finding.message}")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve(root: Path, value: Path) -> Path:
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
        "--inventory",
        type=Path,
        default=Path("docs/recovery/system_contracts.json"),
        help="Inventory path relative to --root.",
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_inventory_validation.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_inventory_validation.md"),
    )
    parser.add_argument("--strict", action="store_true")
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    inventory_path = _resolve(root, args.inventory).resolve()
    try:
        result = validate_inventory(root, inventory_path)
        json_path = _resolve(root, args.json_report).resolve()
        markdown_path = _resolve(root, args.markdown_report).resolve()
        for report_path in (json_path, markdown_path):
            report_path.relative_to(root)
        write_json_report(result, json_path)
        write_markdown_report(result, markdown_path)
    except (InventoryLoadError, OSError, ValueError) as exc:
        print(f"RECOVERY_INVENTORY_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    print(
        "RECOVERY_INVENTORY_VALIDATION "
        f"status={result.status.upper()} systems={result.system_count} findings={len(result.findings)}"
    )
    for finding in result.findings:
        suffix = f" path={finding.path}" if finding.path else ""
        print(
            f"RECOVERY_INVENTORY_FINDING code={finding.code} "
            f"system={finding.system_id or '-'}{suffix} message={finding.message}"
        )
    if args.strict and result.findings:
        return EXIT_VALIDATION_FAILED
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
