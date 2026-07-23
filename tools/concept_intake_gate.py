#!/usr/bin/env python3
"""Validate ZIPTIDE concept-to-build intake records without Unity.

The gate keeps approved concept references, implementation specifications, and
build acceptance markers connected. It intentionally validates repository
contracts only; it does not judge artistic quality or replace booth/headset
verdicts.
"""

from __future__ import annotations

import argparse
import json
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Sequence

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
ALLOWED_STATUSES = {
    "concept-locked",
    "spec-ready-concept-unresolved",
    "intake-ready",
    "blocked",
}
ALLOWED_TIERS = {"A", "B", "B+", "C"}
REQUIRED_ASSET_FIELDS = (
    "id",
    "displayName",
    "tier",
    "status",
    "specPath",
    "sourceDocs",
    "conceptPaths",
    "requiredMarkers",
)


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class ValidationResult:
    manifest_path: str
    root: str
    asset_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "concept_intake_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "manifestPath": self.manifest_path,
            "assetCount": self.asset_count,
            "findingCount": len(self.findings),
            "findings": [asdict(finding) for finding in self.findings],
        }


class GateInputError(RuntimeError):
    """Raised when validation cannot begin safely."""


def _read_text(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8")
    except OSError as exc:
        raise GateInputError(f"Could not read {path}: {exc}") from exc


def _load_json(path: Path) -> dict[str, Any]:
    try:
        data = json.loads(_read_text(path))
    except json.JSONDecodeError as exc:
        raise GateInputError(
            f"Manifest is invalid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(data, dict):
        raise GateInputError("Manifest root must be a JSON object.")
    return data


def _is_nonempty_string(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _safe_repo_path(root: Path, raw: Any, *, field: str) -> tuple[Path | None, Finding | None]:
    if not _is_nonempty_string(raw):
        return None, Finding("PATH_INVALID", f"{field} must be a non-empty string.")
    normalized = str(raw).replace("\\", "/").strip()
    candidate = Path(normalized)
    if candidate.is_absolute():
        return None, Finding("PATH_ABSOLUTE", f"{field} must be repository-relative.", normalized)
    resolved_root = root.resolve()
    resolved = (resolved_root / candidate).resolve()
    try:
        resolved.relative_to(resolved_root)
    except ValueError:
        return None, Finding("PATH_OUTSIDE_ROOT", f"{field} escapes the repository root.", normalized)
    return resolved, None


def _string_list(value: Any, *, field: str, findings: list[Finding]) -> list[str]:
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list."))
        return []
    result: list[str] = []
    for index, item in enumerate(value):
        if not _is_nonempty_string(item):
            findings.append(
                Finding("FIELD_ITEM_INVALID", f"{field}[{index}] must be a non-empty string.")
            )
        else:
            result.append(item.strip())
    return result


def validate_manifest(root: Path, manifest_path: Path) -> ValidationResult:
    root = root.resolve()
    manifest_path = manifest_path.resolve()
    data = _load_json(manifest_path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(
            Finding(
                "SCHEMA_VERSION_UNSUPPORTED",
                f"schemaVersion must equal {SCHEMA_VERSION}; got {data.get('schemaVersion')!r}.",
                manifest_path.as_posix(),
            )
        )

    assets = data.get("assets")
    if not isinstance(assets, list) or not assets:
        findings.append(Finding("ASSETS_REQUIRED", "assets must be a non-empty list."))
        assets = []

    seen_ids: set[str] = set()
    for index, asset in enumerate(assets):
        prefix = f"assets[{index}]"
        if not isinstance(asset, dict):
            findings.append(Finding("ASSET_INVALID", f"{prefix} must be an object."))
            continue

        for field in REQUIRED_ASSET_FIELDS:
            if field not in asset:
                findings.append(Finding("FIELD_REQUIRED", f"{prefix}.{field} is required."))

        asset_id = asset.get("id")
        if not _is_nonempty_string(asset_id):
            findings.append(Finding("ASSET_ID_INVALID", f"{prefix}.id must be non-empty."))
            asset_id = f"index-{index}"
        else:
            asset_id = asset_id.strip()
            if asset_id in seen_ids:
                findings.append(Finding("ASSET_ID_DUPLICATE", f"Duplicate asset id {asset_id}."))
            seen_ids.add(asset_id)

        for field in ("displayName", "specPath"):
            if not _is_nonempty_string(asset.get(field)):
                findings.append(Finding("FIELD_INVALID", f"{prefix}.{field} must be non-empty."))

        tier = asset.get("tier")
        if tier not in ALLOWED_TIERS:
            findings.append(
                Finding("TIER_INVALID", f"{prefix}.tier must be one of {sorted(ALLOWED_TIERS)}.")
            )

        status = asset.get("status")
        if status not in ALLOWED_STATUSES:
            findings.append(
                Finding("STATUS_INVALID", f"{prefix}.status must be one of {sorted(ALLOWED_STATUSES)}.")
            )

        source_docs = _string_list(asset.get("sourceDocs"), field=f"{prefix}.sourceDocs", findings=findings)
        concept_paths = _string_list(
            asset.get("conceptPaths"), field=f"{prefix}.conceptPaths", findings=findings
        )
        markers = _string_list(
            asset.get("requiredMarkers"), field=f"{prefix}.requiredMarkers", findings=findings
        )

        if not source_docs:
            findings.append(Finding("SOURCE_DOCS_REQUIRED", f"{prefix} must name sourceDocs."))
        if not markers:
            findings.append(Finding("SPEC_MARKERS_REQUIRED", f"{prefix} must name requiredMarkers."))
        if status in {"concept-locked", "intake-ready"} and not concept_paths:
            findings.append(
                Finding("CONCEPT_PATHS_REQUIRED", f"{prefix} status {status} requires conceptPaths.")
            )

        for field_name, paths in (("sourceDocs", source_docs), ("conceptPaths", concept_paths)):
            for path_index, raw_path in enumerate(paths):
                path, issue = _safe_repo_path(
                    root, raw_path, field=f"{prefix}.{field_name}[{path_index}]"
                )
                if issue:
                    findings.append(issue)
                elif path is not None and not path.is_file():
                    findings.append(
                        Finding(
                            "REFERENCE_MISSING",
                            f"{prefix}.{field_name}[{path_index}] does not exist.",
                            raw_path,
                        )
                    )

        spec_path, issue = _safe_repo_path(root, asset.get("specPath"), field=f"{prefix}.specPath")
        if issue:
            findings.append(issue)
            continue
        assert spec_path is not None
        if not spec_path.is_file():
            findings.append(
                Finding("SPEC_MISSING", f"{prefix}.specPath does not exist.", str(asset.get("specPath")))
            )
            continue

        spec_text = _read_text(spec_path)
        for marker in markers:
            if marker not in spec_text:
                findings.append(
                    Finding(
                        "SPEC_MARKER_MISSING",
                        f"{asset_id} spec lacks required marker {marker!r}.",
                        str(asset.get("specPath")),
                    )
                )

        if status == "intake-ready" and "## Build acceptance" not in spec_text:
            findings.append(
                Finding(
                    "INTAKE_ACCEPTANCE_MISSING",
                    f"{asset_id} is intake-ready but has no Build acceptance section.",
                    str(asset.get("specPath")),
                )
            )

    return ValidationResult(
        manifest_path=manifest_path.as_posix(),
        root=root.as_posix(),
        asset_count=len(assets),
        findings=tuple(findings),
    )


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--manifest",
        type=Path,
        default=Path("docs/project_art_plan/concept_intake_manifest.json"),
    )
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    manifest = args.manifest
    if not manifest.is_absolute():
        manifest = root / manifest

    try:
        result = validate_manifest(root, manifest)
    except GateInputError as exc:
        print(f"CONCEPT_INTAKE_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report_path = args.json_report
        if not report_path.is_absolute():
            report_path = root / report_path
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            suffix = f" [{finding.path}]" if finding.path else ""
            print(f"CONCEPT_INTAKE_FAIL {finding.code}: {finding.message}{suffix}")
        return EXIT_VALIDATION_FAILED

    print(f"CONCEPT_INTAKE_PASS assets={result.asset_count} manifest={manifest}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
