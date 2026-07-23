#!/usr/bin/env python3
"""Validate launch/re-entry sequencing against celestial and comfort contracts."""

from __future__ import annotations

import argparse
import json
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Sequence

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
ROUTE_PHASES = {
    "launch_w001_to_moss_orbit": ("liftoff", "ascent", "veil", "arrive"),
    "reentry_moss_orbit_to_w001": ("reentry_veil", "descent", "touchdown"),
}
SOURCE_FIELDS = {
    "atmosphereColor",
    "atmosphereThickness",
    "gravity",
    "veilTint",
    "primaryStar",
    "parentBody",
    "visibleSiblingBodies",
}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    route_count: int
    phase_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "launch_transition_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "routeCount": self.route_count,
            "phaseCount": self.phase_count,
            "findingCount": len(self.findings),
            "findings": [asdict(item) for item in self.findings],
        }


class InputError(RuntimeError):
    pass


def _load(path: Path) -> dict[str, Any]:
    try:
        text = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise InputError(f"Could not read {path}: {exc}") from exc
    try:
        data = json.loads(text)
    except json.JSONDecodeError as exc:
        raise InputError(
            f"Invalid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(data, dict):
        raise InputError("Catalog root must be an object.")
    return data


def _nonempty(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _number(value: Any) -> bool:
    return isinstance(value, (int, float)) and not isinstance(value, bool)


def validate_catalog(root: Path, launch_path: Path, celestial_path: Path) -> Result:
    root = root.resolve()
    data = _load(launch_path)
    celestial = _load(celestial_path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))
    if data.get("owner") != "TravelCoordinator":
        findings.append(Finding("OWNER_INVALID", "TravelCoordinator must own the sequence."))
    if data.get("cameraOwner") != "XR-rig":
        findings.append(Finding("CAMERA_OWNER_INVALID", "XR-rig must remain camera owner."))
    if data.get("celestialCatalog") != "docs/design/celestial_system_catalog.json":
        findings.append(
            Finding("CELESTIAL_CATALOG_INVALID", "Launch contract must point to the canonical celestial catalog.")
        )

    global_contract = data.get("globalContract")
    if not isinstance(global_contract, dict):
        findings.append(Finding("GLOBAL_CONTRACT_REQUIRED", "globalContract must be an object."))
        global_contract = {}
    exact_global = {
        "planetAndSpaceHeavyScenesCoLoaded": False,
        "intentionalPlayerTriggerRequired": True,
        "cameraMotionAllowed": False,
        "cameraRollAllowed": False,
        "forcedFovChangeAllowed": False,
        "fullFieldStrobeAllowed": False,
        "comfortVignetteOptional": True,
        "minimumVeilFadeSeconds": 0.25,
        "controlRestoreRequiresStableFloorHorizonInput": True,
        "shipPersistsAcrossSwap": True,
    }
    for key, expected in exact_global.items():
        if global_contract.get(key) != expected:
            findings.append(
                Finding(
                    "GLOBAL_CONTRACT_DRIFT",
                    f"globalContract.{key} must remain {expected!r}; got {global_contract.get(key)!r}.",
                )
            )

    systems = {
        system.get("id"): system
        for system in celestial.get("systems", [])
        if isinstance(system, dict) and _nonempty(system.get("id"))
    }
    worlds = {
        world.get("worldId"): world
        for world in celestial.get("worldPlacements", [])
        if isinstance(world, dict) and _nonempty(world.get("worldId"))
    }
    views = {
        view.get("id"): view
        for view in celestial.get("spaceViews", [])
        if isinstance(view, dict) and _nonempty(view.get("id"))
    }
    w001 = worlds.get("W001", {})

    routes = data.get("routes")
    if not isinstance(routes, list):
        findings.append(Finding("ROUTES_REQUIRED", "routes must be a list."))
        routes = []
    by_id: dict[str, dict[str, Any]] = {}
    phase_count = 0
    for route_index, route in enumerate(routes):
        prefix = f"routes[{route_index}]"
        if not isinstance(route, dict):
            findings.append(Finding("ROUTE_INVALID", f"{prefix} must be an object."))
            continue
        route_id = route.get("id")
        if not _nonempty(route_id):
            findings.append(Finding("ROUTE_ID_INVALID", f"{prefix}.id must be non-empty."))
            continue
        route_id = route_id.strip()
        if route_id in by_id:
            findings.append(Finding("ROUTE_ID_DUPLICATE", f"Duplicate route {route_id}."))
        by_id[route_id] = route
        expected_phases = ROUTE_PHASES.get(route_id)
        if expected_phases is None:
            findings.append(Finding("UNREVIEWED_ROUTE_PRESENT", f"Route {route_id} requires canon amendment."))

        if route.get("worldId") != "W001" or route.get("systemId") != "moss_system_working":
            findings.append(Finding("ROUTE_PLACEMENT_INVALID", f"{route_id} must remain W001/Moss scoped."))
        if route.get("systemId") not in systems:
            findings.append(Finding("ROUTE_SYSTEM_MISSING", f"{route_id} references missing system."))

        if route.get("direction") == "planet-to-space":
            if route.get("originBody") != w001.get("bodyId"):
                findings.append(Finding("ORIGIN_BODY_MISMATCH", f"{route_id} originBody must come from W001."))
            if route.get("destinationSpaceView") != w001.get("spaceSceneId"):
                findings.append(
                    Finding("DESTINATION_VIEW_MISMATCH", f"{route_id} destination must match W001 spaceSceneId.")
                )
            if route.get("destinationSpaceView") not in views:
                findings.append(Finding("DESTINATION_VIEW_MISSING", f"{route_id} references missing space view."))
        elif route.get("direction") == "space-to-planet":
            if route.get("originSpaceView") != w001.get("spaceSceneId"):
                findings.append(Finding("ORIGIN_VIEW_MISMATCH", f"{route_id} origin must match W001 spaceSceneId."))
            if route.get("destinationBody") != w001.get("bodyId"):
                findings.append(
                    Finding("DESTINATION_BODY_MISMATCH", f"{route_id} destinationBody must come from W001.")
                )
        else:
            findings.append(Finding("DIRECTION_INVALID", f"{route_id} direction is invalid."))

        source_fields = route.get("sourceFields")
        if not isinstance(source_fields, dict):
            findings.append(Finding("SOURCE_FIELDS_REQUIRED", f"{route_id} needs sourceFields."))
            source_fields = {}
        if set(source_fields) != SOURCE_FIELDS:
            findings.append(
                Finding(
                    "SOURCE_FIELD_SET_INVALID",
                    f"{route_id} sourceFields must equal {sorted(SOURCE_FIELDS)}.",
                )
            )
        for field in SOURCE_FIELDS:
            expected = f"worldPlacements[W001].{field}"
            if source_fields.get(field) != expected:
                findings.append(
                    Finding("SOURCE_FIELD_DRIFT", f"{route_id}.{field} must reference {expected}.")
                )
            if field not in w001:
                findings.append(Finding("CELESTIAL_FIELD_MISSING", f"W001 lacks celestial field {field}."))

        phases = route.get("phases")
        if not isinstance(phases, list):
            findings.append(Finding("PHASES_REQUIRED", f"{route_id} phases must be a list."))
            phases = []
        phase_count += len(phases)
        actual_order = tuple(
            phase.get("id") for phase in phases if isinstance(phase, dict)
        )
        if expected_phases is not None and actual_order != expected_phases:
            findings.append(
                Finding(
                    "PHASE_ORDER_INVALID",
                    f"{route_id} phases must remain {expected_phases}; got {actual_order}.",
                )
            )
        for phase_index, phase in enumerate(phases):
            if not isinstance(phase, dict):
                findings.append(Finding("PHASE_INVALID", f"{route_id} phase {phase_index} invalid."))
                continue
            phase_id = phase.get("id")
            if not _nonempty(phase_id):
                findings.append(Finding("PHASE_ID_INVALID", f"{route_id} phase id missing."))
                continue
            phase_id = phase_id.strip()
            if phase.get("sequence") != phase_index + 1:
                findings.append(
                    Finding("PHASE_SEQUENCE_INVALID", f"{route_id}/{phase_id} sequence must be {phase_index + 1}.")
                )
            duration = phase.get("durationSeconds")
            if not isinstance(duration, dict):
                findings.append(Finding("DURATION_REQUIRED", f"{route_id}/{phase_id} lacks duration."))
                duration = {}
            low = duration.get("min")
            high = duration.get("max")
            if not _number(low) or not _number(high) or low < 0 or high < low:
                findings.append(Finding("DURATION_INVALID", f"{route_id}/{phase_id} duration invalid."))
            if not isinstance(duration.get("loadHoldAllowed"), bool):
                findings.append(
                    Finding("LOAD_HOLD_BOOL_REQUIRED", f"{route_id}/{phase_id} loadHoldAllowed must be bool.")
                )
            layers = phase.get("visualLayers")
            if not isinstance(layers, list) or not layers or not all(_nonempty(item) for item in layers):
                findings.append(Finding("VISUAL_LAYERS_REQUIRED", f"{route_id}/{phase_id} needs visual layers."))
            for field in ("inputPolicy", "audioCue", "exitCondition", "requiredLog"):
                if not _nonempty(phase.get(field)):
                    findings.append(Finding("TEXT_FIELD_REQUIRED", f"{route_id}/{phase_id}.{field} required."))

            is_veil = phase_id in {"veil", "reentry_veil"}
            if is_veil:
                if low != 1.0 or high != 2.0:
                    findings.append(Finding("VEIL_DURATION_DRIFT", f"{route_id}/{phase_id} must be 1–2 seconds."))
                if duration.get("loadHoldAllowed") is not True:
                    findings.append(Finding("VEIL_LOAD_HOLD_REQUIRED", f"{route_id}/{phase_id} must allow load hold."))
                if phase.get("inputPolicy") != "TravelCoordinator-owned-hold":
                    findings.append(
                        Finding("VEIL_CONTROL_OWNER_INVALID", f"{route_id}/{phase_id} controls belong to TravelCoordinator.")
                    )
                for required_layer in ("transition_shell", "smooth_brightness_peak"):
                    if required_layer not in phase.get("visualLayers", []):
                        findings.append(
                            Finding("VEIL_LAYER_REQUIRED", f"{route_id}/{phase_id} missing {required_layer}.")
                        )
            elif duration.get("loadHoldAllowed") is True:
                findings.append(
                    Finding("LOAD_HOLD_OUTSIDE_VEIL", f"{route_id}/{phase_id} cannot hide a load outside veil.")
                )

            if phase_id in {"arrive", "touchdown"}:
                if phase.get("inputPolicy") != "restore_after_floor_horizon_input_stable":
                    findings.append(
                        Finding("CONTROL_RESTORE_STABILITY", f"{route_id}/{phase_id} must wait for stable floor/horizon/input.")
                    )
                if phase.get("exitCondition") != "player_control_restored":
                    findings.append(
                        Finding("CONTROL_RESTORE_EXIT", f"{route_id}/{phase_id} must end after control restoration.")
                    )

            expected_prefix = "ZIPTIDE: LAUNCH phase=" if route.get("direction") == "planet-to-space" else "ZIPTIDE: REENTRY phase="
            expected_log_phase = "veil" if phase_id == "reentry_veil" else phase_id
            if phase.get("requiredLog") != expected_prefix + expected_log_phase:
                findings.append(
                    Finding("LOG_CONTRACT_INVALID", f"{route_id}/{phase_id} requiredLog is incorrect.")
                )

    actual_routes = set(by_id)
    expected_routes = set(ROUTE_PHASES)
    missing = sorted(expected_routes - actual_routes)
    if missing:
        findings.append(Finding("CANON_ROUTES_MISSING", f"Missing routes: {missing}."))
    extra = sorted(actual_routes - expected_routes)
    if extra:
        findings.append(Finding("UNREVIEWED_ROUTES_PRESENT", f"Unreviewed routes: {extra}."))
    if len(routes) != 2:
        findings.append(Finding("ROUTE_COUNT_INVALID", f"Expected 2 routes; found {len(routes)}."))

    return Result(len(routes), phase_count, tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--catalog", type=Path, default=Path("docs/design/launch_transition_catalog.json")
    )
    parser.add_argument(
        "--celestial", type=Path, default=Path("docs/design/celestial_system_catalog.json")
    )
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    celestial = args.celestial if args.celestial.is_absolute() else root / args.celestial
    try:
        result = validate_catalog(root, catalog, celestial)
    except InputError as exc:
        print(f"LAUNCH_TRANSITION_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"LAUNCH_TRANSITION_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(f"LAUNCH_TRANSITION_PASS routes={result.route_count} phases={result.phase_count}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
