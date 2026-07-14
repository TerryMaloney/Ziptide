#!/usr/bin/env python3
"""Build R0 ownership maps from the recovery scanner and system inventory.

This is a deterministic report generator. It does not infer that every lexical hit
is a defect; it groups exact evidence into the maps needed for recovery decisions.
"""

from __future__ import annotations

import argparse
import json
import sys
from collections import defaultdict
from pathlib import Path
from typing import Any, Sequence

EXIT_OK = 0
EXIT_ERROR = 1

MAP_CODES = {
    "bootstrapPersistence": {"RUNTIME_BOOTSTRAP", "EDITOR_BOOTSTRAP", "DONT_DESTROY_ON_LOAD"},
    "sceneLoading": {"DIRECT_SCENE_LOAD", "DIRECT_SCENE_LOAD_ASYNC"},
    "input": {"INPUT_BUTTON_REFERENCE", "INPUT_ACTION_REFERENCE", "FRAME_BUTTON_POLL"},
    "runtimeSurfaces": {
        "NEW_GAME_OBJECT",
        "CREATE_PRIMITIVE",
        "TEXTMESH_COMPONENT",
        "TMP_COMPONENT",
        "CANVAS_COMPONENT",
        "EVENT_SYSTEM_COMPONENT",
        "XR_UI_INPUT_MODULE",
        "XR_INTERACTABLE_COMPONENT",
        "RUNTIME_MATERIAL_CREATE",
        "SHADER_FIND",
    },
    "xriOwnership": {"XRI_MANAGER_LOOKUP", "XRI_MANAGER_CREATE", "XRI_MANAGER_ASSIGN"},
    "eventsAndSave": {
        "STATIC_EVENT_DECLARATION",
        "EVENT_DECLARATION",
        "SAVE_SYSTEM_REFERENCE",
        "PLAYER_PROFILE_REFERENCE",
        "AUTOSAVE_CALL",
    },
    "globalRender": {
        "RENDER_SETTINGS_MUTATION",
        "CAMERA_MAIN_REFERENCE",
        "VOLUME_COMPONENT",
        "CAMERA_POST_PROCESSING",
    },
    "fallbackDebt": {"FALLBACK_MARKER"},
}


class ReportLoadError(RuntimeError):
    pass


def _load(path: Path) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise ReportLoadError(f"Could not read {path}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise ReportLoadError(f"Invalid JSON in {path}: {exc}") from exc
    if not isinstance(data, dict):
        raise ReportLoadError(f"{path} must contain a JSON object.")
    return data


def _key(finding: dict[str, Any]) -> str:
    symbol = str(finding.get("symbol") or "").strip()
    path = str(finding.get("path") or "").strip()
    return symbol or path or "<unknown>"


def _entry(finding: dict[str, Any]) -> dict[str, Any]:
    return {
        "code": finding.get("code", ""),
        "path": finding.get("path", ""),
        "line": finding.get("line", 0),
        "symbol": finding.get("symbol", ""),
        "snippet": finding.get("snippet", ""),
    }


def _group_findings(findings: list[dict[str, Any]], codes: set[str]) -> list[dict[str, Any]]:
    grouped: dict[str, list[dict[str, Any]]] = defaultdict(list)
    for finding in findings:
        if finding.get("code") in codes:
            grouped[_key(finding)].append(_entry(finding))
    result: list[dict[str, Any]] = []
    for owner in sorted(grouped):
        evidence = sorted(grouped[owner], key=lambda item: (item["path"], item["line"], item["code"]))
        result.append(
            {
                "owner": owner,
                "evidenceCount": len(evidence),
                "codes": sorted({item["code"] for item in evidence}),
                "paths": sorted({item["path"] for item in evidence}),
                "evidence": evidence,
            }
        )
    return result


def build_maps(
    scan: dict[str, Any],
    inventory: dict[str, Any],
    validation: dict[str, Any] | None = None,
) -> dict[str, Any]:
    findings = scan.get("findings")
    if not isinstance(findings, list):
        raise ReportLoadError("Scanner report has no findings list.")

    maps = {name: _group_findings(findings, codes) for name, codes in MAP_CODES.items()}

    systems = inventory.get("systems")
    if not isinstance(systems, list):
        raise ReportLoadError("Inventory has no systems list.")

    exposures: dict[str, list[str]] = defaultdict(list)
    proofs: dict[str, list[str]] = defaultdict(list)
    unresolved_owners: list[dict[str, str]] = []
    for system in systems:
        if not isinstance(system, dict):
            continue
        system_id = str(system.get("id") or "<missing>")
        exposure = str(system.get("exposure") or "UNCLASSIFIED")
        exposures[exposure].append(system_id)
        for proof in system.get("currentProof") or []:
            proofs[str(proof)].append(system_id)
        candidate = str(system.get("canonicalCandidate") or "")
        if "undecided" in candidate.lower() or "unresolved" in candidate.lower():
            unresolved_owners.append({"systemId": system_id, "candidate": candidate})

    validation_findings = []
    if validation is not None:
        raw = validation.get("findings")
        if isinstance(raw, list):
            validation_findings = raw

    return {
        "tool": "recovery_contract_map",
        "sourceScanToolVersion": scan.get("toolVersion"),
        "scannedFiles": scan.get("scannedFiles", 0),
        "findingCount": scan.get("findingCount", len(findings)),
        "maps": maps,
        "inventory": {
            "systemCount": len(systems),
            "exposures": {key: sorted(value) for key, value in sorted(exposures.items())},
            "proofs": {key: sorted(value) for key, value in sorted(proofs.items())},
            "unresolvedCanonicalOwners": sorted(unresolved_owners, key=lambda item: item["systemId"]),
        },
        "validationFindings": validation_findings,
    }


def write_json(data: dict[str, Any], path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(data, indent=2) + "\n", encoding="utf-8")


def write_markdown(data: dict[str, Any], path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE R0 Ownership Maps",
        "",
        f"- Scanned files: **{data['scannedFiles']}**",
        f"- Scanner findings: **{data['findingCount']}**",
        f"- Inventory systems: **{data['inventory']['systemCount']}**",
        "",
        "This report groups exact lexical evidence. A row means the source contains the named ownership signal; it does not by itself declare a defect.",
        "",
        "## Exposure ledger",
        "",
    ]
    for exposure, systems in data["inventory"]["exposures"].items():
        lines.append(f"- **{exposure}:** {', '.join(f'`{item}`' for item in systems) or 'none'}")

    lines.extend(["", "## Unresolved canonical owners", ""])
    unresolved = data["inventory"]["unresolvedCanonicalOwners"]
    if not unresolved:
        lines.append("None.")
    else:
        for item in unresolved:
            lines.append(f"- `{item['systemId']}` — {item['candidate']}")

    for map_name, owners in data["maps"].items():
        title = map_name.replace("And", " & ").replace("bootstrapPersistence", "Bootstrap & persistence")
        title = title.replace("sceneLoading", "Scene loading").replace("runtimeSurfaces", "Runtime surfaces")
        title = title.replace("xriOwnership", "XRI ownership").replace("globalRender", "Global render")
        title = title.replace("fallbackDebt", "Fallback/prototype markers").replace("eventsAndSave", "Events & save")
        title = title.replace("input", "Input")
        lines.extend(["", f"## {title}", ""])
        if not owners:
            lines.append("No matching source signals.")
            continue
        for owner in owners:
            lines.append(f"### `{owner['owner']}` — {owner['evidenceCount']} signal(s)")
            lines.append("")
            lines.append(f"- Codes: {', '.join(f'`{code}`' for code in owner['codes'])}")
            lines.append(f"- Paths: {', '.join(f'`{p}`' for p in owner['paths'])}")
            for evidence in owner["evidence"]:
                lines.append(
                    f"  - `{evidence['path']}:{evidence['line']}` **{evidence['code']}** — `{evidence['snippet']}`"
                )
            lines.append("")

    lines.extend(["## Inventory validation findings", ""])
    if not data["validationFindings"]:
        lines.append("No validation report supplied, or no findings.")
    else:
        for finding in data["validationFindings"]:
            system = finding.get("system_id") or "repository"
            suffix = f" · `{finding.get('path')}`" if finding.get("path") else ""
            lines.append(
                f"- `{system}` **{finding.get('code', '')}**{suffix} — {finding.get('message', '')}"
            )

    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--scan",
        type=Path,
        default=Path("docs/recovery/generated/recovery_contract_scan.json"),
    )
    parser.add_argument(
        "--inventory",
        type=Path,
        default=Path("docs/recovery/system_contracts.json"),
    )
    parser.add_argument(
        "--validation",
        type=Path,
        default=Path("docs/recovery/generated/recovery_inventory_validation.json"),
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_contract_map.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_contract_map.md"),
    )
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    try:
        scan = _load(_resolve(root, args.scan))
        inventory = _load(_resolve(root, args.inventory))
        validation_path = _resolve(root, args.validation)
        validation = _load(validation_path) if validation_path.exists() else None
        data = build_maps(scan, inventory, validation)
        json_path = _resolve(root, args.json_report).resolve()
        markdown_path = _resolve(root, args.markdown_report).resolve()
        for report_path in (json_path, markdown_path):
            report_path.relative_to(root)
        write_json(data, json_path)
        write_markdown(data, markdown_path)
    except (ReportLoadError, OSError, ValueError) as exc:
        print(f"RECOVERY_MAP_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_CONTRACT_MAP systems={data['inventory']['systemCount']} "
        f"maps={len(data['maps'])} scannerFindings={data['findingCount']}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
