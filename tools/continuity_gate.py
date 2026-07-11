#!/usr/bin/env python3
"""Report-only continuity checks for the ZIPTIDE repository.

Default mode never blocks work for validation findings. Use --strict only after
the project ratchets a rule from observation to enforcement.
"""

from __future__ import annotations

import argparse
import json
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Iterable, Sequence

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""
    severity: str = "warning"


@dataclass(frozen=True)
class ValidationResult:
    manifest_path: str
    root: str
    schema_version: int | None
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "continuity_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "manifestPath": self.manifest_path,
            "schemaVersion": self.schema_version,
            "findingCount": len(self.findings),
            "findings": [asdict(finding) for finding in self.findings],
        }


class ManifestLoadError(RuntimeError):
    """Raised when validation cannot begin because the manifest is unreadable."""


def _is_nonempty_string(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _safe_repo_path(root: Path, raw_path: Any, *, field: str) -> tuple[Path | None, Finding | None]:
    if not _is_nonempty_string(raw_path):
        return None, Finding("PATH_INVALID", f"{field} must be a non-empty string.")
    candidate_text = str(raw_path).replace("\\", "/").strip()
    candidate = Path(candidate_text)
    if candidate.is_absolute():
        return None, Finding("PATH_ABSOLUTE", f"{field} must be repository-relative.", candidate_text)
    resolved_root = root.resolve()
    resolved_candidate = (resolved_root / candidate).resolve()
    try:
        resolved_candidate.relative_to(resolved_root)
    except ValueError:
        return None, Finding("PATH_OUTSIDE_ROOT", f"{field} escapes the repository root.", candidate_text)
    return resolved_candidate, None


def _pattern_static_prefix(pattern: str) -> str:
    normalized = pattern.replace("\\", "/")
    wildcard_positions = [pos for token in ("*", "?", "[") if (pos := normalized.find(token)) >= 0]
    if not wildcard_positions:
        return normalized
    prefix = normalized[: min(wildcard_positions)].rstrip("/")
    return prefix or "."


def _require_path(
    root: Path,
    raw_path: Any,
    *,
    field: str,
    findings: list[Finding],
    code: str = "REQUIRED_PATH_MISSING",
) -> None:
    candidate, issue = _safe_repo_path(root, raw_path, field=field)
    if issue:
        findings.append(issue)
        return
    assert candidate is not None
    if not candidate.exists():
        findings.append(Finding(code, f"{field} does not exist.", str(raw_path)))


def _validate_string_list(
    value: Any,
    *,
    field: str,
    findings: list[Finding],
    required: bool = False,
) -> list[str]:
    if value is None and not required:
        return []
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list."))
        return []
    valid: list[str] = []
    for index, item in enumerate(value):
        if not _is_nonempty_string(item):
            findings.append(
                Finding("FIELD_ITEM_INVALID", f"{field}[{index}] must be a non-empty string.")
            )
            continue
        valid.append(item.strip())
    if required and not valid:
        findings.append(Finding("FIELD_REQUIRED", f"{field} must contain at least one path."))
    return valid


def _duplicate_values(values: Iterable[str]) -> set[str]:
    seen: set[str] = set()
    duplicates: set[str] = set()
    for value in values:
        if value in seen:
            duplicates.add(value)
        seen.add(value)
    return duplicates


def load_manifest(manifest_path: Path) -> dict[str, Any]:
    try:
        text = manifest_path.read_text(encoding="utf-8")
    except OSError as exc:
        raise ManifestLoadError(f"Could not read manifest: {exc}") from exc
    try:
        data = json.loads(text)
    except json.JSONDecodeError as exc:
        raise ManifestLoadError(
            f"Manifest is not valid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(data, dict):
        raise ManifestLoadError("Manifest root must be a JSON object.")
    return data


def validate_manifest(root: Path, manifest_path: Path) -> ValidationResult:
    root = root.resolve()
    manifest_path = manifest_path.resolve()
    manifest = load_manifest(manifest_path)
    findings: list[Finding] = []

    schema_version = manifest.get("schemaVersion")
    if schema_version != SCHEMA_VERSION:
        findings.append(
            Finding(
                "SCHEMA_VERSION_UNSUPPORTED",
                f"schemaVersion must equal {SCHEMA_VERSION}; got {schema_version!r}.",
            )
        )

    if not _is_nonempty_string(manifest.get("branchOfTruth")):
        findings.append(Finding("BRANCH_OF_TRUTH_MISSING", "branchOfTruth must be a non-empty string."))

    required_documents = _validate_string_list(
        manifest.get("requiredDocuments"),
        field="requiredDocuments",
        findings=findings,
        required=True,
    )
    for index, path in enumerate(required_documents):
        _require_path(
            root,
            path,
            field=f"requiredDocuments[{index}]",
            findings=findings,
        )
    for duplicate in sorted(_duplicate_values(required_documents)):
        findings.append(
            Finding("REQUIRED_PATH_DUPLICATE", "requiredDocuments contains a duplicate path.", duplicate)
        )

    shared_paths = _validate_string_list(
        manifest.get("sharedPaths"),
        field="sharedPaths",
        findings=findings,
    )
    for index, path in enumerate(shared_paths):
        _require_path(root, path, field=f"sharedPaths[{index}]", findings=findings)

    lanes = manifest.get("lanes")
    if not isinstance(lanes, list) or not lanes:
        findings.append(Finding("LANES_REQUIRED", "lanes must be a non-empty list."))
        lanes = []

    lane_ids: list[str] = []
    lane_boards: list[str] = []
    for index, lane in enumerate(lanes):
        prefix = f"lanes[{index}]"
        if not isinstance(lane, dict):
            findings.append(Finding("LANE_INVALID", f"{prefix} must be an object."))
            continue

        lane_id = lane.get("id")
        if not _is_nonempty_string(lane_id):
            findings.append(Finding("LANE_ID_INVALID", f"{prefix}.id must be a non-empty string."))
        else:
            lane_ids.append(lane_id.strip())

        board = lane.get("board")
        if not _is_nonempty_string(board):
            findings.append(Finding("LANE_BOARD_INVALID", f"{prefix}.board must be a non-empty string."))
        else:
            board = board.strip()
            lane_boards.append(board)
            _require_path(root, board, field=f"{prefix}.board", findings=findings, code="LANE_BOARD_MISSING")

        patterns = _validate_string_list(
            lane.get("ownedPathPatterns"),
            field=f"{prefix}.ownedPathPatterns",
            findings=findings,
        )
        for pattern_index, pattern in enumerate(patterns):
            static_prefix = _pattern_static_prefix(pattern)
            _require_path(
                root,
                static_prefix,
                field=f"{prefix}.ownedPathPatterns[{pattern_index}] prefix",
                findings=findings,
                code="OWNERSHIP_PREFIX_MISSING",
            )

        notes = lane.get("ownershipNotes")
        if notes is not None and not _is_nonempty_string(notes):
            findings.append(
                Finding("LANE_NOTES_INVALID", f"{prefix}.ownershipNotes must be a non-empty string when present.")
            )

    for duplicate in sorted(_duplicate_values(lane_ids)):
        findings.append(Finding("LANE_ID_DUPLICATE", "Lane id is declared more than once.", duplicate))
    for duplicate in sorted(_duplicate_values(lane_boards)):
        findings.append(Finding("LANE_BOARD_DUPLICATE", "Lane board is assigned more than once.", duplicate))

    contracts = manifest.get("protectedContracts", [])
    if not isinstance(contracts, list):
        findings.append(Finding("CONTRACTS_INVALID", "protectedContracts must be a list."))
        contracts = []

    contract_ids: list[str] = []
    for index, contract in enumerate(contracts):
        prefix = f"protectedContracts[{index}]"
        if not isinstance(contract, dict):
            findings.append(Finding("CONTRACT_INVALID", f"{prefix} must be an object."))
            continue

        contract_id = contract.get("id")
        if not _is_nonempty_string(contract_id):
            findings.append(Finding("CONTRACT_ID_INVALID", f"{prefix}.id must be a non-empty string."))
        else:
            contract_ids.append(contract_id.strip())

        version = contract.get("version")
        if not isinstance(version, int) or isinstance(version, bool) or version < 1:
            findings.append(
                Finding("CONTRACT_VERSION_INVALID", f"{prefix}.version must be an integer >= 1.")
            )

        decision_doc = contract.get("decisionDoc")
        _require_path(
            root,
            decision_doc,
            field=f"{prefix}.decisionDoc",
            findings=findings,
            code="CONTRACT_DECISION_DOC_MISSING",
        )

        paths = _validate_string_list(
            contract.get("paths"),
            field=f"{prefix}.paths",
            findings=findings,
            required=True,
        )
        for path_index, path in enumerate(paths):
            _require_path(
                root,
                path,
                field=f"{prefix}.paths[{path_index}]",
                findings=findings,
                code="CONTRACT_PATH_MISSING",
            )

    for duplicate in sorted(_duplicate_values(contract_ids)):
        findings.append(
            Finding("CONTRACT_ID_DUPLICATE", "Protected contract id is declared more than once.", duplicate)
        )

    return ValidationResult(
        manifest_path=str(manifest_path),
        root=str(root),
        schema_version=schema_version if isinstance(schema_version, int) else None,
        findings=tuple(findings),
    )


def write_report(result: ValidationResult, report_path: Path) -> None:
    report_path.parent.mkdir(parents=True, exist_ok=True)
    report_path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parents[1],
        help="Repository root. Defaults to the parent of tools/.",
    )
    parser.add_argument(
        "--manifest",
        type=Path,
        default=Path("docs/continuity/project_manifest.json"),
        help="Manifest path, relative to --root unless absolute.",
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        help="Optional JSON report path, relative to --root unless absolute.",
    )
    parser.add_argument(
        "--strict",
        action="store_true",
        help="Return exit code 2 when validation findings exist. Default is report-only.",
    )
    return parser


def _resolve_under_root(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    manifest_path = _resolve_under_root(root, args.manifest).resolve()

    try:
        result = validate_manifest(root, manifest_path)
    except ManifestLoadError as exc:
        print(f"CONTINUITY_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report_path = _resolve_under_root(root, args.json_report).resolve()
        try:
            report_path.relative_to(root)
        except ValueError:
            print("CONTINUITY_ERROR --json-report must remain inside --root.", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR
        try:
            write_report(result, report_path)
        except OSError as exc:
            print(f"CONTINUITY_ERROR Could not write report: {exc}", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR

    print(
        "CONTINUITY_REPORT "
        f"status={result.status.upper()} findings={len(result.findings)} "
        f"manifest={manifest_path.relative_to(root)}"
    )
    for finding in result.findings:
        suffix = f" path={finding.path}" if finding.path else ""
        print(f"CONTINUITY_FINDING code={finding.code}{suffix} message={finding.message}")

    if args.strict and result.findings:
        return EXIT_VALIDATION_FAILED
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
