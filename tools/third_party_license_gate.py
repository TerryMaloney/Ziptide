#!/usr/bin/env python3
"""Validate ZIPTIDE's third-party licensing ledger without inventing license facts.

Structural omissions and undeclared imports are blockers. Known imports whose exact license research
is still pending remain visible warnings and are forbidden from release distribution.
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
UNKNOWN = "verification-required"
ALLOWED_DISTRIBUTION = {"approved", "hold-verification", "excluded-from-build", "removed"}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    severity: str
    entry_id: str = ""
    path: str = ""


@dataclass(frozen=True)
class ValidationResult:
    manifest_path: str
    entry_count: int
    findings: tuple[Finding, ...]

    @property
    def blockers(self) -> tuple[Finding, ...]:
        return tuple(item for item in self.findings if item.severity == "blocker")

    @property
    def warnings(self) -> tuple[Finding, ...]:
        return tuple(item for item in self.findings if item.severity == "warning")

    @property
    def status(self) -> str:
        if self.blockers:
            return "fail"
        if self.warnings:
            return "warning"
        return "pass"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "third_party_license_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "manifestPath": self.manifest_path,
            "entryCount": self.entry_count,
            "blockerCount": len(self.blockers),
            "warningCount": len(self.warnings),
            "findings": [asdict(item) for item in self.findings],
        }


class LicenseGateError(RuntimeError):
    pass


def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _load_json(path: Path, label: str) -> dict[str, Any]:
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise LicenseGateError(f"Could not read {label}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise LicenseGateError(
            f"{label} is invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(payload, dict):
        raise LicenseGateError(f"{label} root must be an object.")
    return payload


def _safe_path(root: Path, raw: Any) -> Path | None:
    if not _text(raw):
        return None
    candidate = Path(str(raw).replace("\\", "/").strip())
    if candidate.is_absolute():
        return None
    resolved = (root / candidate).resolve()
    try:
        resolved.relative_to(root.resolve())
    except ValueError:
        return None
    return resolved


def validate_manifest(root: Path, manifest_path: Path) -> ValidationResult:
    root = root.resolve()
    manifest_path = manifest_path.resolve()
    manifest = _load_json(manifest_path, "third-party manifest")
    findings: list[Finding] = []

    if manifest.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1.", "blocker"))

    policy = manifest.get("policy")
    if not isinstance(policy, dict):
        findings.append(Finding("POLICY_INVALID", "policy must be an object.", "blocker"))
        policy = {}

    credits_raw = policy.get("creditsPath")
    credits_path = _safe_path(root, credits_raw)
    credits_text = ""
    if credits_path is None:
        findings.append(Finding("CREDITS_PATH_INVALID", "policy.creditsPath must be a safe relative path.", "blocker"))
    elif not credits_path.is_file():
        findings.append(Finding("CREDITS_PATH_MISSING", "Credits ledger is missing.", "blocker", path=str(credits_raw)))
    else:
        credits_text = credits_path.read_text(encoding="utf-8")
        manifest_reference = manifest_path.relative_to(root).as_posix()
        if manifest_reference not in credits_text:
            findings.append(Finding("CREDITS_MANIFEST_BACKLINK_MISSING", f"Credits ledger must reference {manifest_reference}.", "blocker", path=str(credits_raw)))

    packages_raw = policy.get("packagesManifest")
    packages_path = _safe_path(root, packages_raw)
    dependencies: dict[str, Any] = {}
    if packages_path is None:
        findings.append(Finding("PACKAGES_PATH_INVALID", "policy.packagesManifest must be a safe relative path.", "blocker"))
    elif not packages_path.is_file():
        findings.append(Finding("PACKAGES_PATH_MISSING", "Unity package manifest is missing.", "blocker", path=str(packages_raw)))
    else:
        package_payload = _load_json(packages_path, "Unity package manifest")
        raw_dependencies = package_payload.get("dependencies", {})
        if not isinstance(raw_dependencies, dict):
            findings.append(Finding("PACKAGES_DEPENDENCIES_INVALID", "dependencies must be an object.", "blocker", path=str(packages_raw)))
        else:
            dependencies = raw_dependencies

    unity_prefix = policy.get("unityPackagePrefix", "com.unity.")
    if not _text(unity_prefix):
        findings.append(Finding("UNITY_PREFIX_INVALID", "policy.unityPackagePrefix must be non-empty.", "blocker"))
        unity_prefix = "com.unity."

    known_roots_raw = manifest.get("knownExternalAssetRoots", [])
    if not isinstance(known_roots_raw, list):
        findings.append(Finding("KNOWN_ROOTS_INVALID", "knownExternalAssetRoots must be a list.", "blocker"))
        known_roots: set[str] = set()
    else:
        known_roots = {str(item).replace("\\", "/").strip() for item in known_roots_raw if _text(item)}
        if len(known_roots) != len(known_roots_raw):
            findings.append(Finding("KNOWN_ROOT_INVALID", "Every known external root must be a unique non-empty string.", "blocker"))

    entries_raw = manifest.get("entries", [])
    if not isinstance(entries_raw, list):
        findings.append(Finding("ENTRIES_INVALID", "entries must be a list.", "blocker"))
        entries_raw = []

    ids: set[str] = set()
    paths: set[str] = set()
    package_ids: set[str] = set()
    declared_roots: set[str] = set()
    required_fields = (
        "id",
        "creditsAnchor",
        "category",
        "provider",
        "repositoryPath",
        "owner",
        "exactVersion",
        "sourceRecord",
        "licenseId",
        "licenseEvidencePath",
        "obligations",
        "distributionStatus",
    )
    research_fields = ("exactVersion", "sourceRecord", "licenseId", "licenseEvidencePath", "obligations")

    for index, entry in enumerate(entries_raw):
        if not isinstance(entry, dict):
            findings.append(Finding("ENTRY_INVALID", f"entries[{index}] must be an object.", "blocker"))
            continue
        entry_id = str(entry.get("id", "")).strip()
        for field in required_fields:
            if not _text(entry.get(field)):
                findings.append(Finding("FIELD_REQUIRED", f"{field} must be non-empty.", "blocker", entry_id))

        if entry_id in ids:
            findings.append(Finding("ENTRY_ID_DUPLICATE", f"Duplicate id {entry_id!r}.", "blocker", entry_id))
        ids.add(entry_id)

        repository_raw = str(entry.get("repositoryPath", "")).replace("\\", "/").strip()
        if repository_raw in paths:
            findings.append(Finding("REPOSITORY_PATH_DUPLICATE", f"Duplicate repositoryPath {repository_raw!r}.", "blocker", entry_id, repository_raw))
        paths.add(repository_raw)
        declared_roots.add(repository_raw)
        repository_path = _safe_path(root, repository_raw)
        if repository_path is None:
            findings.append(Finding("REPOSITORY_PATH_INVALID", "repositoryPath must be safe and relative.", "blocker", entry_id, repository_raw))
        elif not repository_path.exists():
            findings.append(Finding("REPOSITORY_PATH_MISSING", "Declared imported content is missing.", "blocker", entry_id, repository_raw))

        anchor = str(entry.get("creditsAnchor", "")).strip()
        if credits_text and f"### {anchor}" not in credits_text:
            findings.append(Finding("CREDITS_ANCHOR_MISSING", f"Credits ledger lacks section '### {anchor}'.", "blocker", entry_id, str(credits_raw)))

        package_id = str(entry.get("packageId", "")).strip()
        if package_id:
            if package_id in package_ids:
                findings.append(Finding("PACKAGE_ID_DUPLICATE", f"Duplicate packageId {package_id!r}.", "blocker", entry_id))
            package_ids.add(package_id)
            if package_id not in dependencies:
                findings.append(Finding("PACKAGE_ID_NOT_INSTALLED", f"Declared package {package_id!r} is not installed.", "blocker", entry_id, str(packages_raw)))

        status = str(entry.get("distributionStatus", "")).strip()
        if status not in ALLOWED_DISTRIBUTION:
            findings.append(Finding("DISTRIBUTION_STATUS_UNKNOWN", f"Unknown distributionStatus {status!r}.", "blocker", entry_id))

        unresolved = [field for field in research_fields if entry.get(field) == UNKNOWN]
        if unresolved:
            findings.append(Finding("LICENSE_RESEARCH_PENDING", "Verification required for: " + ", ".join(unresolved) + ".", "warning", entry_id, repository_raw))
        if status == "hold-verification":
            findings.append(Finding("DISTRIBUTION_ON_HOLD", "Entry is present but not approved for release distribution.", "warning", entry_id, repository_raw))
        if status == "approved":
            if unresolved:
                findings.append(Finding("APPROVED_WITH_UNKNOWN_TERMS", "Approved entries cannot contain verification-required fields.", "blocker", entry_id, repository_raw))
            evidence_raw = entry.get("licenseEvidencePath")
            evidence_path = _safe_path(root, evidence_raw)
            if evidence_path is None or not evidence_path.is_file():
                findings.append(Finding("APPROVED_EVIDENCE_MISSING", "Approved entry requires an existing licenseEvidencePath.", "blocker", entry_id, str(evidence_raw)))

        if entry.get("shipsInCurrentRecoveryCandidate") is True and status != "approved":
            findings.append(Finding("UNAPPROVED_RECOVERY_DISTRIBUTION", "An unapproved entry cannot ship in the current recovery candidate.", "blocker", entry_id, repository_raw))

    for known_root in sorted(known_roots):
        known_path = _safe_path(root, known_root)
        if known_path is None:
            findings.append(Finding("KNOWN_ROOT_PATH_INVALID", "Known root must be safe and relative.", "blocker", path=known_root))
        elif known_path.exists() and known_root not in declared_roots:
            findings.append(Finding("KNOWN_ROOT_UNDECLARED", "Existing external asset root has no licensing entry.", "blocker", path=known_root))

    for package_id in sorted(dependencies):
        if not package_id.startswith(str(unity_prefix)) and package_id not in package_ids:
            findings.append(Finding("NON_UNITY_PACKAGE_UNDECLARED", f"Installed non-Unity package {package_id!r} has no licensing entry.", "blocker", path=str(packages_raw)))

    return ValidationResult(
        manifest_path=manifest_path.relative_to(root).as_posix(),
        entry_count=len(entries_raw),
        findings=tuple(findings),
    )


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--manifest", type=Path, default=Path("docs/licensing/third_party_manifest.json"))
    parser.add_argument("--json-report", type=Path)
    parser.add_argument("--strict-warnings", action="store_true")
    args = parser.parse_args(argv)

    root = args.root.resolve()
    manifest_path = args.manifest if args.manifest.is_absolute() else root / args.manifest
    try:
        result = validate_manifest(root, manifest_path)
    except (LicenseGateError, OSError, ValueError) as exc:
        print(f"THIRD_PARTY_LICENSE_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    payload = result.to_dict()
    if args.json_report:
        report_path = args.json_report if args.json_report.is_absolute() else root / args.json_report
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(json.dumps(payload, indent=2, sort_keys=True) + "\n", encoding="utf-8")

    print(
        "THIRD_PARTY_LICENSE "
        f"status={result.status} entries={result.entry_count} "
        f"blockers={len(result.blockers)} warnings={len(result.warnings)}"
    )
    for finding in result.findings:
        print(f"[{finding.severity}] {finding.code}: {finding.message}")

    if result.blockers or (args.strict_warnings and result.warnings):
        return EXIT_VALIDATION_FAILED
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
