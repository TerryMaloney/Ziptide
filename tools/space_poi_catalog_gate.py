#!/usr/bin/env python3
"""Validate the first Moss-orbit POI palette against mission and Quest rails."""

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
POI_IDS = {
    "prior_waker_smallcraft",
    "salvage_tug_disabled",
    "debris_ribbon",
    "orbital_repair_relay",
    "stranded_civilian_ship",
    "ziptide_gate_approach_lane",
}
CORE_MISSIONS = {
    "salvage_run",
    "derelict_discovery",
    "ferry_passage",
    "distress_rescue",
    "relay_seal_repair",
}
CATEGORIES = {"derelict", "ship", "field", "infrastructure", "travel-landmark"}
DISTANCE_BANDS = {
    "near-to-mid",
    "mid-to-far",
    "near-to-far-landmark",
    "near-to-system-landmark",
}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    poi_count: int
    mission_coverage_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "space_poi_catalog_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "poiCount": self.poi_count,
            "missionCoverageCount": self.mission_coverage_count,
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


def _nonempty_string_list(value: Any) -> bool:
    return isinstance(value, list) and bool(value) and all(_nonempty(item) for item in value)


def validate_catalog(root: Path, catalog_path: Path, mission_path: Path) -> Result:
    root = root.resolve()
    data = _load(catalog_path)
    missions_data = _load(mission_path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))
    if data.get("systemId") != "moss_system_working":
        findings.append(Finding("SYSTEM_ID_INVALID", "POI palette must remain Moss-system scoped."))

    scope = data.get("scope")
    if not isinstance(scope, dict):
        findings.append(Finding("SCOPE_REQUIRED", "scope must be an object."))
        scope = {}
    locked_scope = {
        "runtimeSpawningAuthorized": False,
        "combatRequired": False,
        "evaRequired": False,
        "floatingOriginRequired": False,
        "questBudgetProfile": "bounded-orbit-v1",
    }
    for key, expected in locked_scope.items():
        if scope.get(key) != expected:
            findings.append(
                Finding(
                    "SCOPE_LOCK_VIOLATION",
                    f"scope.{key} must remain {expected!r}; got {scope.get(key)!r}.",
                )
            )

    mission_ids = {
        mission.get("id")
        for mission in missions_data.get("missions", [])
        if isinstance(mission, dict) and _nonempty(mission.get("id"))
    }
    if not CORE_MISSIONS.issubset(mission_ids):
        findings.append(
            Finding("MISSION_CATALOG_INCOMPLETE", "Mission catalog lacks one or more core missions.")
        )

    budgets = data.get("budgetClasses")
    if not isinstance(budgets, dict) or not budgets:
        findings.append(Finding("BUDGET_CLASSES_REQUIRED", "budgetClasses must be non-empty."))
        budgets = {}
    for budget_id, budget in budgets.items():
        if not isinstance(budget, dict):
            findings.append(Finding("BUDGET_INVALID", f"Budget {budget_id} must be an object."))
            continue
        for field in ("maxTrianglesNear", "maxMaterials", "maxLiveObjects"):
            value = budget.get(field)
            if not isinstance(value, int) or isinstance(value, bool) or value <= 0:
                findings.append(
                    Finding("BUDGET_VALUE_INVALID", f"Budget {budget_id}.{field} must be positive integer.")
                )
        if budget.get("lodRequired") is not True:
            findings.append(Finding("LOD_REQUIRED", f"Budget {budget_id} must require LOD."))

    pois = data.get("pois")
    if not isinstance(pois, list):
        findings.append(Finding("POIS_REQUIRED", "pois must be a list."))
        pois = []
    by_id: dict[str, dict[str, Any]] = {}
    covered_missions: set[str] = set()
    for index, poi in enumerate(pois):
        prefix = f"pois[{index}]"
        if not isinstance(poi, dict):
            findings.append(Finding("POI_INVALID", f"{prefix} must be an object."))
            continue
        poi_id = poi.get("id")
        if not _nonempty(poi_id):
            findings.append(Finding("POI_ID_INVALID", f"{prefix}.id must be non-empty."))
            continue
        poi_id = poi_id.strip()
        if poi_id in by_id:
            findings.append(Finding("POI_ID_DUPLICATE", f"Duplicate POI id {poi_id}."))
        by_id[poi_id] = poi

        if not _nonempty(poi.get("displayName")):
            findings.append(Finding("DISPLAY_NAME_REQUIRED", f"{poi_id} lacks displayName."))
        if poi.get("category") not in CATEGORIES:
            findings.append(Finding("CATEGORY_INVALID", f"{poi_id} category is invalid."))
        if poi.get("distanceBand") not in DISTANCE_BANDS:
            findings.append(Finding("DISTANCE_BAND_INVALID", f"{poi_id} distanceBand is invalid."))
        if poi.get("pacifistAccessible") is not True:
            findings.append(
                Finding("PACIFIST_ACCESS_REQUIRED", f"{poi_id} must remain pacifist-accessible.")
            )
        budget_id = poi.get("budgetClass")
        if budget_id not in budgets:
            findings.append(Finding("BUDGET_CLASS_MISSING", f"{poi_id} references {budget_id!r}."))

        compatible = poi.get("missionCompatibility")
        if not _nonempty_string_list(compatible):
            findings.append(
                Finding("MISSION_COMPATIBILITY_REQUIRED", f"{poi_id} needs missionCompatibility.")
            )
            compatible = []
        for mission_id in compatible:
            if mission_id not in mission_ids:
                findings.append(
                    Finding("MISSION_REFERENCE_MISSING", f"{poi_id} references unknown {mission_id}.")
                )
            else:
                covered_missions.add(mission_id)

        outputs = poi.get("salvageOutputs")
        if not _nonempty_string_list(outputs):
            findings.append(Finding("SALVAGE_OUTPUTS_REQUIRED", f"{poi_id} needs salvageOutputs."))
        sockets = poi.get("requiredSockets")
        if not _nonempty_string_list(sockets):
            findings.append(Finding("SOCKETS_REQUIRED", f"{poi_id} needs requiredSockets."))
            sockets = []
        if len(sockets) != len(set(sockets)):
            findings.append(Finding("SOCKET_DUPLICATE", f"{poi_id} repeats a socket."))
        lod = poi.get("lodPlan")
        if not _nonempty_string_list(lod) or len(lod) < 3:
            findings.append(Finding("LOD_PLAN_REQUIRED", f"{poi_id} needs near/mid/far LOD plan."))
        if not _nonempty(poi.get("factionOwnership")):
            findings.append(Finding("FACTION_REQUIRED", f"{poi_id} lacks factionOwnership."))
        if not _nonempty(poi.get("groundSpaceEcho")):
            findings.append(Finding("GROUND_SPACE_ECHO_REQUIRED", f"{poi_id} lacks groundSpaceEcho."))
        hazards = poi.get("hazards")
        if not _nonempty_string_list(hazards):
            findings.append(Finding("HAZARDS_REQUIRED", f"{poi_id} needs explicit hazards."))

        story = poi.get("storyDrop")
        if not isinstance(story, dict):
            findings.append(Finding("STORY_DROP_REQUIRED", f"{poi_id} lacks storyDrop."))
        else:
            if not _nonempty(story.get("type")) or not _nonempty(story.get("chanceBand")):
                findings.append(Finding("STORY_DROP_INVALID", f"{poi_id} storyDrop is incomplete."))
            if not isinstance(story.get("orderedThread"), bool):
                findings.append(
                    Finding("STORY_ORDER_BOOL_REQUIRED", f"{poi_id}.storyDrop.orderedThread must be bool.")
                )

    actual_ids = set(by_id)
    missing = sorted(POI_IDS - actual_ids)
    extra = sorted(actual_ids - POI_IDS)
    if missing:
        findings.append(Finding("CANON_POIS_MISSING", f"Missing POIs: {missing}."))
    if extra:
        findings.append(
            Finding("UNREVIEWED_POIS_PRESENT", f"Unreviewed POIs require canon amendment: {extra}.")
        )
    if len(pois) != len(POI_IDS):
        findings.append(Finding("POI_COUNT_INVALID", f"Expected 6 POIs; found {len(pois)}."))

    uncovered_core = sorted(CORE_MISSIONS - covered_missions)
    if uncovered_core:
        findings.append(
            Finding("CORE_MISSION_POI_GAP", f"Core missions lack compatible POIs: {uncovered_core}.")
        )

    waker = by_id.get("prior_waker_smallcraft")
    if waker:
        story = waker.get("storyDrop", {})
        if story.get("type") != "waker-log" or story.get("orderedThread") is not True:
            findings.append(
                Finding("WAKER_THREAD_REQUIRED", "Prior-waker wreck must carry ordered waker logs.")
            )
        if "derelict_discovery" not in waker.get("missionCompatibility", []):
            findings.append(
                Finding("WAKER_DISCOVERY_REQUIRED", "Prior-waker wreck must support derelict discovery.")
            )

    relay = by_id.get("orbital_repair_relay")
    if relay:
        if "relay_seal_repair" not in relay.get("missionCompatibility", []):
            findings.append(Finding("RELAY_MISSION_REQUIRED", "Relay POI must support relay repair."))
        if "signal_hook" not in relay.get("requiredSockets", []):
            findings.append(Finding("RELAY_SIGNAL_SOCKET_REQUIRED", "Relay POI needs signal_hook."))
        if "raises_signal" not in relay.get("hazards", []):
            findings.append(Finding("RELAY_SIGNAL_EFFECT_REQUIRED", "Relay POI must name raises_signal."))

    gate = by_id.get("ziptide_gate_approach_lane")
    if gate:
        if "gate_run" not in gate.get("missionCompatibility", []):
            findings.append(Finding("GATE_RUN_REQUIRED", "Gate approach must support gate_run."))
        for socket in ("alignment_volume", "traversal_plane", "safe_arrival_anchor"):
            if socket not in gate.get("requiredSockets", []):
                findings.append(
                    Finding("GATE_SOCKET_REQUIRED", f"Gate approach is missing {socket}.")
                )

    for distress_id in ("salvage_tug_disabled", "stranded_civilian_ship"):
        poi = by_id.get(distress_id)
        if not poi:
            continue
        sockets = set(poi.get("requiredSockets", []))
        required = {"distress_beacon", "tow_attach", "repair_panel"}
        if not required.issubset(sockets):
            findings.append(
                Finding("DISTRESS_SOCKET_REQUIRED", f"{distress_id} lacks distress/tow/repair sockets.")
            )

    return Result(len(pois), len(covered_missions), tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--catalog", type=Path, default=Path("docs/design/space_poi_catalog.json"))
    parser.add_argument(
        "--missions", type=Path, default=Path("docs/design/space_mission_catalog.json")
    )
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    missions = args.missions if args.missions.is_absolute() else root / args.missions
    try:
        result = validate_catalog(root, catalog, missions)
    except InputError as exc:
        print(f"SPACE_POI_CATALOG_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"SPACE_POI_CATALOG_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(
        f"SPACE_POI_CATALOG_PASS pois={result.poi_count} missionCoverage={result.mission_coverage_count}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
