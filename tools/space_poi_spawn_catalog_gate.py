#!/usr/bin/env python3
"""Validate freeze-safe space POI spawn packets against their source catalogs."""

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
EXPECTED_FAMILIES = {
    "prior_waker_smallcraft",
    "salvage_tug_disabled",
    "debris_ribbon",
    "orbital_repair_relay",
    "stranded_civilian_ship",
    "ziptide_gate_approach_lane",
}
SEED_INPUTS = ["systemSeed", "sectorId", "poiFamilyId", "slotIndex"]
ALLOWED_MODES = {
    "deterministic-sector",
    "authored-or-deterministic-sector",
    "authored-landmark-only",
}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    packet_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "space_poi_spawn_catalog_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "packetCount": self.packet_count,
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


def _text(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _string_list(value: Any, *, allow_empty: bool = False) -> bool:
    return (
        isinstance(value, list)
        and (allow_empty or bool(value))
        and all(_text(item) for item in value)
    )


def validate_catalog(root: Path, spawn_path: Path, poi_path: Path, mission_path: Path) -> Result:
    root = root.resolve()
    spawn = _load(spawn_path)
    source = _load(poi_path)
    missions = _load(mission_path)
    findings: list[Finding] = []

    if spawn.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))
    if spawn.get("decisionStatus") != "proposed-contract":
        findings.append(
            Finding("DECISION_STATUS_INVALID", "Spawn packets must remain proposed-contract until runtime approval.")
        )
    if spawn.get("systemId") != source.get("systemId"):
        findings.append(Finding("SYSTEM_ID_MISMATCH", "Spawn and POI catalogs must share systemId."))
    if spawn.get("sourcePoiCatalog") != "docs/design/space_poi_catalog.json":
        findings.append(Finding("SOURCE_POI_PATH_INVALID", "sourcePoiCatalog path is not canonical."))
    if spawn.get("sourceMissionCatalog") != "docs/design/space_mission_catalog.json":
        findings.append(Finding("SOURCE_MISSION_PATH_INVALID", "sourceMissionCatalog path is not canonical."))

    scope = spawn.get("scope")
    if not isinstance(scope, dict):
        findings.append(Finding("SCOPE_REQUIRED", "scope must be an object."))
        scope = {}
    exact_scope = {
        "runtimeSpawningAuthorized": False,
        "runtimeOwner": "unassigned-post-checkpoint",
        "deterministicAcrossReloads": True,
        "deviceTimeEntropyAllowed": False,
        "networkEntropyAllowed": False,
        "floatingOriginRequired": False,
    }
    for key, expected in exact_scope.items():
        if scope.get(key) != expected:
            findings.append(
                Finding("SCOPE_LOCK_VIOLATION", f"scope.{key} must remain {expected!r}.")
            )

    seed = spawn.get("seedContract")
    if not isinstance(seed, dict):
        findings.append(Finding("SEED_CONTRACT_REQUIRED", "seedContract must be an object."))
        seed = {}
    if seed.get("algorithm") != "stable-hash-v1-proposed":
        findings.append(Finding("SEED_ALGORITHM_INVALID", "Seed algorithm must remain stable-hash-v1-proposed."))
    if seed.get("inputs") != SEED_INPUTS:
        findings.append(Finding("SEED_INPUTS_INVALID", f"Seed inputs must remain {SEED_INPUTS}."))
    if seed.get("systemSeedSource") != "author-required":
        findings.append(Finding("SYSTEM_SEED_SOURCE_INVALID", "systemSeedSource must remain author-required."))
    if seed.get("sectorGrid") != "author-required":
        findings.append(Finding("SECTOR_GRID_SOURCE_INVALID", "sectorGrid must remain author-required."))
    required_logs = seed.get("requiredLogFields")
    expected_logs = {
        "packetId",
        "poiFamilyId",
        "sectorId",
        "slotIndex",
        "resolvedSeed",
        "capDecision",
        "exclusionDecision",
    }
    if not _string_list(required_logs) or set(required_logs) != expected_logs:
        findings.append(Finding("SEED_LOG_FIELDS_INVALID", "requiredLogFields must contain the complete seed evidence set."))

    source_pois = {
        item.get("id"): item
        for item in source.get("pois", [])
        if isinstance(item, dict) and _text(item.get("id"))
    }
    mission_ids = {
        item.get("id")
        for item in missions.get("missions", [])
        if isinstance(item, dict) and _text(item.get("id"))
    }

    packets = spawn.get("packets")
    if not isinstance(packets, list):
        findings.append(Finding("PACKETS_REQUIRED", "packets must be a list."))
        packets = []
    by_family: dict[str, dict[str, Any]] = {}
    packet_ids: set[str] = set()
    for index, packet in enumerate(packets):
        prefix = f"packets[{index}]"
        if not isinstance(packet, dict):
            findings.append(Finding("PACKET_INVALID", f"{prefix} must be an object."))
            continue
        packet_id = packet.get("id")
        family_id = packet.get("poiFamilyId")
        if not _text(packet_id):
            findings.append(Finding("PACKET_ID_INVALID", f"{prefix}.id must be non-empty."))
        elif packet_id in packet_ids:
            findings.append(Finding("PACKET_ID_DUPLICATE", f"Duplicate packet id {packet_id}."))
        else:
            packet_ids.add(packet_id)
        if not _text(family_id):
            findings.append(Finding("FAMILY_ID_INVALID", f"{prefix}.poiFamilyId must be non-empty."))
            continue
        family_id = family_id.strip()
        if family_id in by_family:
            findings.append(Finding("FAMILY_PACKET_DUPLICATE", f"Duplicate packet for {family_id}."))
        by_family[family_id] = packet
        source_poi = source_pois.get(family_id)
        if source_poi is None:
            findings.append(Finding("SOURCE_POI_MISSING", f"No source POI exists for {family_id}."))
            continue

        if packet.get("adapterStatus") != "unassigned-post-checkpoint":
            findings.append(Finding("ADAPTER_STATUS_INVALID", f"{family_id} adapterStatus is premature."))
        mode = packet.get("spawnMode")
        if mode not in ALLOWED_MODES:
            findings.append(Finding("SPAWN_MODE_INVALID", f"{family_id} spawnMode is invalid."))
        cap = packet.get("perSceneCap")
        if not isinstance(cap, int) or isinstance(cap, bool) or cap < 1 or cap > 2:
            findings.append(Finding("SCENE_CAP_INVALID", f"{family_id} perSceneCap must be 1 or 2."))
        if not isinstance(packet.get("uniquePerSector"), bool):
            findings.append(Finding("UNIQUE_PER_SECTOR_BOOL_REQUIRED", f"{family_id}.uniquePerSector must be boolean."))

        seed_inputs = packet.get("seedInputs")
        if mode == "authored-landmark-only":
            if seed_inputs != []:
                findings.append(Finding("AUTHORED_LANDMARK_SEED_FORBIDDEN", f"{family_id} authored landmark cannot use deterministic seed inputs."))
        elif seed_inputs != SEED_INPUTS:
            findings.append(Finding("PACKET_SEED_INPUTS_INVALID", f"{family_id} seedInputs must remain {SEED_INPUTS}."))

        for field in ("distanceBand", "budgetClass"):
            if packet.get(field) != source_poi.get(field):
                findings.append(Finding("SOURCE_FIELD_MISMATCH", f"{family_id}.{field} differs from source POI catalog."))
        for field in ("lodPlan", "requiredSockets", "missionCompatibility", "salvageOutputs"):
            value = packet.get(field)
            source_value = source_poi.get(field)
            if value != source_value:
                findings.append(Finding("SOURCE_LIST_MISMATCH", f"{family_id}.{field} differs from source POI catalog."))
            if not _string_list(value):
                findings.append(Finding("PACKET_LIST_REQUIRED", f"{family_id}.{field} must be a non-empty string list."))
            elif len(value) != len(set(value)):
                findings.append(Finding("PACKET_LIST_DUPLICATE", f"{family_id}.{field} contains duplicates."))

        for mission_id in packet.get("missionCompatibility", []):
            if mission_id not in mission_ids:
                findings.append(Finding("MISSION_REFERENCE_MISSING", f"{family_id} references unknown mission {mission_id}."))

        story = packet.get("storyContract")
        source_story = source_poi.get("storyDrop")
        if not isinstance(story, dict):
            findings.append(Finding("STORY_CONTRACT_REQUIRED", f"{family_id} lacks storyContract."))
        elif story.get("type") != source_story.get("type") or story.get("orderedThread") != source_story.get("orderedThread"):
            findings.append(Finding("STORY_CONTRACT_MISMATCH", f"{family_id} storyContract differs from source POI."))

        exclusions = packet.get("spawnExclusions")
        if not _string_list(exclusions):
            findings.append(Finding("SPAWN_EXCLUSIONS_REQUIRED", f"{family_id} needs spawnExclusions."))
        elif len(exclusions) != len(set(exclusions)):
            findings.append(Finding("SPAWN_EXCLUSION_DUPLICATE", f"{family_id} repeats an exclusion."))

    actual_families = set(by_family)
    missing = sorted(EXPECTED_FAMILIES - actual_families)
    extra = sorted(actual_families - EXPECTED_FAMILIES)
    if missing:
        findings.append(Finding("CANON_PACKETS_MISSING", f"Missing packet families: {missing}."))
    if extra:
        findings.append(Finding("UNREVIEWED_PACKETS_PRESENT", f"Unreviewed packet families require amendment: {extra}."))
    if len(packets) != len(EXPECTED_FAMILIES):
        findings.append(Finding("PACKET_COUNT_INVALID", f"Expected 6 packets; found {len(packets)}."))

    gate = by_family.get("ziptide_gate_approach_lane")
    if gate:
        if gate.get("spawnMode") != "authored-landmark-only" or gate.get("perSceneCap") != 1:
            findings.append(Finding("GATE_LANDMARK_CONTRACT", "Gate approach must be one authored landmark only."))
        for exclusion in ("duplicate_gate_landmark", "moving_alignment_volume", "moving_traversal_plane"):
            if exclusion not in gate.get("spawnExclusions", []):
                findings.append(Finding("GATE_EXCLUSION_REQUIRED", f"Gate approach is missing exclusion {exclusion}."))

    relay = by_family.get("orbital_repair_relay")
    if relay:
        for socket in ("signal_hook", "warden_response_origin"):
            if socket not in relay.get("requiredSockets", []):
                findings.append(Finding("RELAY_SOCKET_REQUIRED", f"Relay packet is missing {socket}."))

    waker = by_family.get("prior_waker_smallcraft")
    if waker and waker.get("storyContract", {}).get("orderedThread") is not True:
        findings.append(Finding("WAKER_ORDER_REQUIRED", "Prior-waker packet must preserve ordered lore."))

    debris = by_family.get("debris_ribbon")
    if debris:
        for exclusion in ("gate_alignment_volume", "traversal_plane"):
            if exclusion not in debris.get("spawnExclusions", []):
                findings.append(Finding("DEBRIS_EXCLUSION_REQUIRED", f"Debris ribbon is missing exclusion {exclusion}."))

    return Result(len(packets), tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--catalog", type=Path, default=Path("docs/design/space_poi_spawn_catalog.json"))
    parser.add_argument("--poi-catalog", type=Path, default=Path("docs/design/space_poi_catalog.json"))
    parser.add_argument("--mission-catalog", type=Path, default=Path("docs/design/space_mission_catalog.json"))
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    poi_catalog = args.poi_catalog if args.poi_catalog.is_absolute() else root / args.poi_catalog
    mission_catalog = args.mission_catalog if args.mission_catalog.is_absolute() else root / args.mission_catalog
    try:
        result = validate_catalog(root, catalog, poi_catalog, mission_catalog)
    except InputError as exc:
        print(f"SPACE_POI_SPAWN_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"SPACE_POI_SPAWN_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(f"SPACE_POI_SPAWN_PASS packets={result.packet_count}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
