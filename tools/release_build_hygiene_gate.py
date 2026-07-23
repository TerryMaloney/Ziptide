#!/usr/bin/env python3
"""Release build hygiene gate.

Validates the ACTUAL build configuration (ProjectSettings, build scripts, shipped
content) against docs/release/release_build_contract.json.

Two modes:
- Development (default): structural errors, parse failures, and contract
  contradictions are BLOCKERS; known release gaps report as RELEASE-HOLD findings
  (exit 0) so ordinary work is not permanently red while the release profile is
  honestly incomplete.
- --require-release-ready: every hold becomes a blocker — a build cannot claim
  release readiness while any hygiene gap is open.

Reads only. Never edits Unity files, packages, Photon files, or signing state.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any, Sequence

SCHEMA_VERSION = 1
ALLOWED_MANUAL_STATUSES = {"hold", "verified", "waived"}
SEVERITIES = ("blocker", "hold", "warning", "info")


class HygieneGateError(Exception):
    pass


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    severity: str
    path: str = ""


@dataclass
class Result:
    mode: str
    facts: dict[str, Any] = field(default_factory=dict)
    findings: list[Finding] = field(default_factory=list)

    def add(self, code: str, message: str, severity: str, path: str = "") -> None:
        if severity not in SEVERITIES:
            raise HygieneGateError(f"internal: bad severity {severity!r}")
        self.findings.append(Finding(code, message, severity, path))

    def count(self, severity: str) -> int:
        return sum(1 for f in self.findings if f.severity == severity)


def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _load_json(path: Path, label: str) -> dict[str, Any]:
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise HygieneGateError(f"Could not read {label}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise HygieneGateError(
            f"{label} is invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(payload, dict):
        raise HygieneGateError(f"{label} root must be an object.")
    return payload


def _read_lines(path: Path, label: str) -> list[str]:
    try:
        return path.read_text(encoding="utf-8", errors="replace").splitlines()
    except OSError as exc:
        raise HygieneGateError(f"Could not read {label}: {exc}") from exc


def parse_scalar(lines: Sequence[str], key: str) -> str | None:
    """First `  key: value` occurrence at any indent; returns stripped value ('' if empty)."""
    pattern = re.compile(rf"^\s*{re.escape(key)}:\s*(.*)$")
    for line in lines:
        match = pattern.match(line)
        if match:
            return match.group(1).strip()
    return None


def parse_platform_dict(lines: Sequence[str], key: str) -> dict[str, str]:
    """Parse a `key:` block of `  Platform: value` children (Unity YAML shape)."""
    out: dict[str, str] = {}
    header = re.compile(rf"^(\s*){re.escape(key)}:\s*$")
    for index, line in enumerate(lines):
        match = header.match(line)
        if not match:
            continue
        base_indent = len(match.group(1))
        for child in lines[index + 1:]:
            child_match = re.match(r"^(\s*)([A-Za-z0-9_]+):\s*(.*)$", child)
            if not child_match or len(child_match.group(1)) <= base_indent:
                break
            out[child_match.group(2)] = child_match.group(3).strip()
        break
    return out


def parse_defines(lines: Sequence[str]) -> dict[str, list[str]]:
    """Per-platform scripting define symbols (`scriptingDefineSymbols:` block)."""
    raw = parse_platform_dict(lines, "scriptingDefineSymbols")
    return {platform: [d for d in value.split(";") if d] for platform, value in raw.items()}


def validate_contract(contract: dict[str, Any]) -> list[Finding]:
    findings: list[Finding] = []
    if contract.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("CONTRACT_SCHEMA_UNSUPPORTED", "schemaVersion must equal 1.", "blocker"))
    expected = contract.get("expected")
    if not isinstance(expected, dict):
        findings.append(Finding("CONTRACT_EXPECTED_INVALID", "expected must be an object.", "blocker"))
    sources = contract.get("sources")
    if not isinstance(sources, dict) or not all(
        _text(sources.get(k)) for k in ("projectSettings", "buildScript", "packagesManifest")
    ):
        findings.append(Finding(
            "CONTRACT_SOURCES_INVALID",
            "sources must name projectSettings, buildScript, and packagesManifest paths.",
            "blocker"))
    manual = contract.get("manualChecks", [])
    if not isinstance(manual, list):
        findings.append(Finding("CONTRACT_MANUAL_INVALID", "manualChecks must be a list.", "blocker"))
        manual = []
    seen: set[str] = set()
    for index, row in enumerate(manual):
        if not isinstance(row, dict):
            findings.append(Finding("MANUAL_ROW_INVALID", f"manualChecks[{index}] must be an object.", "blocker"))
            continue
        row_id = str(row.get("id", "")).strip()
        if not row_id:
            findings.append(Finding("MANUAL_ID_REQUIRED", f"manualChecks[{index}] needs an id.", "blocker"))
        if row_id in seen:
            findings.append(Finding("MANUAL_ID_DUPLICATE", f"Duplicate manual check id {row_id!r}.", "blocker"))
        seen.add(row_id)
        status = str(row.get("status", "")).strip()
        if status not in ALLOWED_MANUAL_STATUSES:
            findings.append(Finding(
                "MANUAL_STATUS_UNKNOWN",
                f"manualChecks[{row_id or index}] status {status!r} not in {sorted(ALLOWED_MANUAL_STATUSES)}.",
                "blocker"))
        for key in ("title", "owner", "nextAction"):
            if not _text(row.get(key)):
                findings.append(Finding(
                    "MANUAL_FIELD_REQUIRED",
                    f"manualChecks[{row_id or index}] requires non-empty {key}.",
                    "blocker"))
        if status == "verified" and not row.get("evidence"):
            findings.append(Finding(
                "MANUAL_VERIFIED_WITHOUT_EVIDENCE",
                f"manualChecks[{row_id}] is 'verified' but lists no evidence.",
                "blocker"))
        if status == "waived" and not _text(row.get("waiverReason")):
            findings.append(Finding(
                "MANUAL_WAIVER_UNJUSTIFIED",
                f"manualChecks[{row_id}] is 'waived' without a waiverReason.",
                "blocker"))
    return findings


def run_gate(root: Path, contract_path: Path, require_release_ready: bool) -> Result:
    root = root.resolve()
    result = Result(mode="release" if require_release_ready else "development")
    contract = _load_json(contract_path, "release build contract")
    result.findings.extend(validate_contract(contract))
    if result.count("blocker"):
        return result

    expected = contract["expected"]
    sources = contract["sources"]

    def src(name: str) -> Path:
        return root / str(sources[name]).replace("\\", "/")

    hold = "blocker" if require_release_ready else "hold"

    # ---- ProjectSettings facts -------------------------------------------------
    ps_path = src("projectSettings")
    if not ps_path.is_file():
        result.add("PROJECT_SETTINGS_MISSING", "ProjectSettings.asset not found.", "blocker", str(ps_path))
        return result
    lines = _read_lines(ps_path, "ProjectSettings.asset")

    app_ids = parse_platform_dict(lines, "applicationIdentifier")
    result.facts["applicationIdentifierAndroid"] = app_ids.get("Android", "")
    if app_ids.get("Android", "") != expected.get("applicationIdentifierAndroid"):
        result.add(
            "APP_ID_MISMATCH",
            f"Android applicationIdentifier is {app_ids.get('Android', '')!r}; contract expects "
            f"{expected.get('applicationIdentifierAndroid')!r}.",
            "blocker", str(sources["projectSettings"]))

    backend = parse_platform_dict(lines, "scriptingBackend").get("Android", "")
    result.facts["scriptingBackendAndroid"] = backend
    if backend != str(expected.get("scriptingBackendAndroid")):
        result.add("IL2CPP_REQUIRED", f"Android scriptingBackend is {backend!r}; expected IL2CPP (1).",
                   "blocker", str(sources["projectSettings"]))

    arch = parse_scalar(lines, "AndroidTargetArchitectures") or ""
    result.facts["androidTargetArchitectures"] = arch
    if arch != str(expected.get("androidTargetArchitectures")):
        result.add("ARM64_ONLY_REQUIRED", f"AndroidTargetArchitectures is {arch!r}; expected ARM64-only (2).",
                   "blocker", str(sources["projectSettings"]))

    min_sdk_raw = parse_scalar(lines, "AndroidMinSdkVersion") or "0"
    result.facts["androidMinSdkVersion"] = min_sdk_raw
    try:
        if int(min_sdk_raw) < int(expected.get("androidMinSdkVersionAtLeast", 0)):
            result.add("MIN_SDK_TOO_LOW", f"AndroidMinSdkVersion {min_sdk_raw} is below the contract floor.",
                       "blocker", str(sources["projectSettings"]))
    except ValueError:
        result.add("MIN_SDK_UNPARSEABLE", f"AndroidMinSdkVersion {min_sdk_raw!r} is not an integer.",
                   "blocker", str(sources["projectSettings"]))

    target_sdk = parse_scalar(lines, "AndroidTargetSdkVersion") or "0"
    result.facts["androidTargetSdkVersion"] = target_sdk
    if expected.get("requireExplicitTargetSdk") and target_sdk == "0":
        result.add("TARGET_SDK_UNPINNED", "AndroidTargetSdkVersion is 0 (auto); release requires an explicit pin.",
                   hold, str(sources["projectSettings"]))

    bundle_version = parse_scalar(lines, "bundleVersion") or ""
    version_code = parse_scalar(lines, "AndroidBundleVersionCode") or ""
    result.facts["bundleVersion"] = bundle_version
    result.facts["androidBundleVersionCode"] = version_code
    if not bundle_version or not version_code:
        result.add("VERSION_FIELDS_MISSING", "bundleVersion / AndroidBundleVersionCode must be set.",
                   "blocker", str(sources["projectSettings"]))

    keystore = parse_scalar(lines, "AndroidKeystoreName") or ""
    keyalias = parse_scalar(lines, "AndroidKeyaliasName") or ""
    result.facts["androidKeystoreConfigured"] = bool(keystore and keyalias)
    if not (keystore and keyalias):
        result.add("KEYSTORE_UNCONFIGURED", "No release keystore/alias configured — builds are debug-signed.",
                   hold, str(sources["projectSettings"]))

    defines = parse_defines(lines)
    android_defines = defines.get("Android", [])
    result.facts["androidDefines"] = android_defines
    forbidden = [d for d in expected.get("releaseForbiddenDefinesAndroid", []) if d in android_defines]
    result.facts["releaseForbiddenDefinesPresent"] = forbidden
    if forbidden:
        result.add(
            "FORBIDDEN_DEFINES_PRESENT",
            "Release-forbidden defines active on Android: " + ", ".join(forbidden) + ".",
            hold, str(sources["projectSettings"]))

    # ---- Build script facts ----------------------------------------------------
    build_path = src("buildScript")
    if not build_path.is_file():
        result.add("BUILD_SCRIPT_MISSING", "Build script not found.", "blocker", str(sources["buildScript"]))
    else:
        build_text = build_path.read_text(encoding="utf-8", errors="replace")
        dev_patterns = [p for p in expected.get("devBuildFlagPatterns", []) if p in build_text]
        result.facts["devBuildFlagPatternsFound"] = dev_patterns
        if dev_patterns:
            result.add(
                "DEV_BUILD_FLAGS_HARDCODED",
                "Build script hardcodes development flags: " + ", ".join(dev_patterns) +
                " — every APK from this path is a development build.",
                hold, str(sources["buildScript"]))
        release_method = str(expected.get("releaseBuildMethodName", "")).strip()
        has_release_method = bool(release_method) and re.search(
            rf"\b{re.escape(release_method)}\s*\(", build_text) is not None
        result.facts["releaseBuildMethodPresent"] = has_release_method
        if release_method and not has_release_method:
            result.add(
                "RELEASE_METHOD_MISSING",
                f"No {release_method}() entry point exists — there is no way to produce a non-development build.",
                hold, str(sources["buildScript"]))

    # ---- Shipped-content facts -------------------------------------------------
    demo_present = [
        rel for rel in expected.get("demoContentRoots", [])
        if (root / str(rel).replace("\\", "/")).exists()
    ]
    result.facts["demoContentPresent"] = demo_present
    if demo_present:
        result.add(
            "DEMO_CONTENT_PRESENT",
            f"{len(demo_present)} demo/dead-weight roots exist and would ship or bloat the project "
            "(strip at release): " + ", ".join(demo_present) + ".",
            hold)

    photon_settings_rel = sources.get("photonServerSettings")
    if _text(photon_settings_rel):
        photon_path = root / str(photon_settings_rel).replace("\\", "/")
        app_id = ""
        if photon_path.is_file():
            match = re.search(r"AppIdRealtime:\s*(\S+)", photon_path.read_text(encoding="utf-8", errors="replace"))
            app_id = match.group(1).strip() if match else ""
        result.facts["photonAppIdCommitted"] = bool(app_id)
        if app_id:
            result.add(
                "PHOTON_APPID_SHIPS",
                "A live Photon AppIdRealtime is committed in a Resources asset and ships in every build.",
                hold, str(photon_settings_rel))

    packages_path = src("packagesManifest")
    if not packages_path.is_file():
        result.add("PACKAGES_MANIFEST_MISSING", "Unity package manifest not found.", "blocker",
                   str(sources["packagesManifest"]))
    else:
        dependencies = _load_json(packages_path, "Unity package manifest").get("dependencies", {})
        prefix = str(expected.get("unityPackagePrefix", "com.unity."))
        allowed = set(expected.get("allowedNonUnityPackages", []))
        rogue = sorted(
            pid for pid in dependencies
            if not pid.startswith(prefix) and pid not in allowed
        )
        result.facts["nonUnityPackages"] = rogue
        if rogue:
            result.add(
                "NON_UNITY_PACKAGE_UNDECLARED",
                "Non-Unity packages outside the contract allowlist: " + ", ".join(rogue) +
                " (declare in the contract + licensing manifest).",
                "blocker", str(sources["packagesManifest"]))

    apk_path = root / "Builds" / "Android" / "Ziptide.apk"
    if apk_path.is_file():
        size = apk_path.stat().st_size
        result.facts["apkBytes"] = size
        if size > int(expected.get("maxApkBytes", 1 << 30)):
            result.add("APK_OVER_SIZE_CAP", f"APK is {size} bytes — over the contract cap.", "blocker",
                       "Builds/Android/Ziptide.apk")
    else:
        result.facts["apkBytes"] = None
        result.add("APK_SIZE_UNMEASURED", "No local APK artifact; size evidence pending a build.", "info")

    # ---- Manual rows -----------------------------------------------------------
    for row in contract.get("manualChecks", []):
        row_id = str(row.get("id", "")).strip()
        status = str(row.get("status", "")).strip()
        required = bool(row.get("required"))
        if status == "hold" and required:
            result.add(
                f"MANUAL_HOLD:{row_id}",
                f"{row.get('title')} — {row.get('nextAction')}",
                hold)
        elif status == "verified":
            for evidence in row.get("evidence", []):
                evidence_path = root / str(evidence).replace("\\", "/")
                if not evidence_path.is_file():
                    result.add(
                        f"MANUAL_EVIDENCE_MISSING:{row_id}",
                        f"Verified manual check {row_id!r} cites missing evidence {evidence!r}.",
                        "blocker")
    return result


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--contract", type=Path,
                        default=Path("docs/release/release_build_contract.json"))
    parser.add_argument("--json-report", type=Path)
    parser.add_argument("--require-release-ready", action="store_true")
    args = parser.parse_args(argv)

    contract_path = args.contract if args.contract.is_absolute() else args.root / args.contract
    try:
        result = run_gate(args.root, contract_path, args.require_release_ready)
    except HygieneGateError as exc:
        print(f"RELEASE_BUILD_HYGIENE error: {exc}", file=sys.stderr)
        return 1

    blockers = result.count("blocker")
    holds = result.count("hold")
    status = "blocked" if blockers else ("release-hold" if holds else "ok")
    print(f"RELEASE_BUILD_HYGIENE mode={result.mode} status={status} "
          f"blockers={blockers} holds={holds} warnings={result.count('warning')}")
    for finding in result.findings:
        print(f"[{finding.severity}] {finding.code}: {finding.message}"
              + (f" ({finding.path})" if finding.path else ""))

    if args.json_report:
        report_path = args.json_report if args.json_report.is_absolute() else args.root / args.json_report
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(json.dumps({
            "schemaVersion": SCHEMA_VERSION,
            "gate": "release_build_hygiene",
            "mode": result.mode,
            "status": status,
            "counts": {sev: result.count(sev) for sev in SEVERITIES},
            "facts": result.facts,
            "findings": [finding.__dict__ for finding in result.findings],
        }, indent=2) + "\n", encoding="utf-8")

    return 1 if blockers else 0


if __name__ == "__main__":
    sys.exit(main())
