#!/usr/bin/env python3
"""Validate the MK2 room order, gameplay sockets, and skin invariants."""

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
ROOM_ORDER = (
    "cockpit",
    "lounge_galley",
    "sleeping_quarters",
    "greenhouse",
    "drive_core",
    "workshop",
    "airlock",
)
CONCEPT_STATUSES = {
    "keeper-locked",
    "keeper-missing-required",
    "spec-required",
    "derived-from-graft",
}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    room_count: int
    socket_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "mk2_room_manifest_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "roomCount": self.room_count,
            "socketCount": self.socket_count,
            "findingCount": len(self.findings),
            "findings": [asdict(finding) for finding in self.findings],
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
        raise InputError("Manifest root must be an object.")
    return data


def _nonempty(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _string_list(value: Any, *, field: str, findings: list[Finding]) -> list[str]:
    if not isinstance(value, list):
        findings.append(Finding("LIST_REQUIRED", f"{field} must be a list."))
        return []
    result: list[str] = []
    for index, item in enumerate(value):
        if not _nonempty(item):
            findings.append(Finding("LIST_ITEM_INVALID", f"{field}[{index}] must be non-empty."))
        else:
            result.append(item.strip())
    return result


def _validate_repo_path(root: Path, raw: str, *, field: str, findings: list[Finding]) -> None:
    normalized = raw.replace("\\", "/")
    candidate = Path(normalized)
    if candidate.is_absolute():
        findings.append(Finding("PATH_ABSOLUTE", f"{field} must be repository-relative.", normalized))
        return
    resolved_root = root.resolve()
    resolved = (resolved_root / candidate).resolve()
    try:
        resolved.relative_to(resolved_root)
    except ValueError:
        findings.append(Finding("PATH_OUTSIDE_ROOT", f"{field} escapes repository root.", normalized))
        return
    if not resolved.is_file():
        findings.append(Finding("CONCEPT_MISSING", f"{field} does not exist.", normalized))


def validate_manifest(root: Path, path: Path) -> Result:
    root = root.resolve()
    data = _load(path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(
            Finding("SCHEMA_VERSION_UNSUPPORTED", f"schemaVersion must equal {SCHEMA_VERSION}.")
        )
    if data.get("shipId") != "ship_mk2_architect":
        findings.append(Finding("SHIP_ID_INVALID", "shipId must remain ship_mk2_architect."))

    scale = data.get("scaleMeters")
    if not isinstance(scale, dict):
        findings.append(Finding("SCALE_REQUIRED", "scaleMeters must be an object."))
        scale = {}
    expected_ranges = (
        ("lengthMin", "lengthMax", 25.0, 30.0),
        ("interiorClearHeightMin", "interiorClearHeightMax", 2.1, 2.5),
        ("primaryCorridorWidthMin", "primaryCorridorWidthMax", 1.1, 1.5),
    )
    for low_key, high_key, canonical_low, canonical_high in expected_ranges:
        low = scale.get(low_key)
        high = scale.get(high_key)
        if not isinstance(low, (int, float)) or isinstance(low, bool):
            findings.append(Finding("SCALE_VALUE_INVALID", f"{low_key} must be numeric."))
            continue
        if not isinstance(high, (int, float)) or isinstance(high, bool):
            findings.append(Finding("SCALE_VALUE_INVALID", f"{high_key} must be numeric."))
            continue
        if low > high:
            findings.append(Finding("SCALE_RANGE_INVALID", f"{low_key} exceeds {high_key}."))
        if low != canonical_low or high != canonical_high:
            findings.append(
                Finding(
                    "SCALE_CANON_DRIFT",
                    f"{low_key}/{high_key} must remain {canonical_low}/{canonical_high} until amended.",
                )
            )

    skin = data.get("skinContract")
    if not isinstance(skin, dict):
        findings.append(Finding("SKIN_CONTRACT_REQUIRED", "skinContract must be an object."))
        skin = {}
    for flag in (
        "gameplaySocketsRemainStable",
        "navigationVolumesRemainStable",
        "collisionOwnershipRemainsStable",
        "fixedIdentityZonesRemainVisible",
        "roomShellsMaySwap",
    ):
        if skin.get(flag) is not True:
            findings.append(Finding("SKIN_INVARIANT_REQUIRED", f"skinContract.{flag} must be true."))

    rooms = data.get("rooms")
    if not isinstance(rooms, list):
        findings.append(Finding("ROOMS_REQUIRED", "rooms must be a list."))
        rooms = []

    room_by_id: dict[str, dict[str, Any]] = {}
    socket_count = 0
    for index, room in enumerate(rooms):
        prefix = f"rooms[{index}]"
        if not isinstance(room, dict):
            findings.append(Finding("ROOM_INVALID", f"{prefix} must be an object."))
            continue
        room_id = room.get("id")
        if not _nonempty(room_id):
            findings.append(Finding("ROOM_ID_INVALID", f"{prefix}.id must be non-empty."))
            continue
        room_id = room_id.strip()
        if room_id in room_by_id:
            findings.append(Finding("ROOM_ID_DUPLICATE", f"Duplicate room id {room_id}."))
        room_by_id[room_id] = room

        if room.get("sequence") != index + 1:
            findings.append(
                Finding("ROOM_SEQUENCE_INVALID", f"{room_id} sequence must equal {index + 1}.")
            )
        if room.get("conceptStatus") not in CONCEPT_STATUSES:
            findings.append(Finding("CONCEPT_STATUS_INVALID", f"{room_id} has invalid status."))
        if not _nonempty(room.get("displayName")):
            findings.append(Finding("DISPLAY_NAME_REQUIRED", f"{room_id} lacks displayName."))

        concepts = _string_list(
            room.get("conceptPaths"), field=f"{prefix}.conceptPaths", findings=findings
        )
        status = room.get("conceptStatus")
        if status in {"keeper-locked", "derived-from-graft"} and not concepts:
            findings.append(Finding("KEEPER_REQUIRED", f"{room_id} status {status} requires conceptPaths."))
        if status == "keeper-missing-required" and concepts:
            findings.append(
                Finding("MISSING_KEEPER_STATUS_INVALID", f"{room_id} cannot have keeper paths while marked missing.")
            )
        for concept_index, concept in enumerate(concepts):
            _validate_repo_path(
                root,
                concept,
                field=f"{prefix}.conceptPaths[{concept_index}]",
                findings=findings,
            )

        sockets = _string_list(
            room.get("requiredSockets"), field=f"{prefix}.requiredSockets", findings=findings
        )
        if not sockets:
            findings.append(Finding("SOCKETS_REQUIRED", f"{room_id} must declare requiredSockets."))
        if len(sockets) != len(set(sockets)):
            findings.append(Finding("SOCKET_DUPLICATE", f"{room_id} repeats a local socket id."))
        if room_id != "cockpit" and not any(socket.startswith("route_") for socket in sockets):
            findings.append(Finding("ROUTE_SOCKET_REQUIRED", f"{room_id} needs at least one route socket."))
        socket_count += len(sockets)

        fixed = _string_list(
            room.get("fixedIdentityZones"), field=f"{prefix}.fixedIdentityZones", findings=findings
        )
        skinnable = _string_list(
            room.get("skinnableZones"), field=f"{prefix}.skinnableZones", findings=findings
        )
        if not fixed:
            findings.append(Finding("FIXED_IDENTITY_REQUIRED", f"{room_id} needs fixed identity zones."))
        if not skinnable:
            findings.append(Finding("SKINNABLE_ZONES_REQUIRED", f"{room_id} needs skinnable zones."))
        overlap = sorted(set(fixed) & set(skinnable))
        if overlap:
            findings.append(
                Finding("SKIN_ZONE_OVERLAP", f"{room_id} zones cannot be fixed and skinnable: {overlap}.")
            )

    actual_order = tuple(room.get("id") for room in rooms if isinstance(room, dict))
    if actual_order != ROOM_ORDER:
        findings.append(
            Finding("ROOM_ORDER_INVALID", f"Room order must remain {ROOM_ORDER}; got {actual_order}.")
        )
    if len(rooms) != len(ROOM_ORDER):
        findings.append(Finding("ROOM_COUNT_INVALID", f"Expected 7 rooms; found {len(rooms)}."))

    sleeping = room_by_id.get("sleeping_quarters")
    if sleeping:
        sockets = set(sleeping.get("requiredSockets", []))
        required_bunks = {f"bunk_0{i}" for i in range(1, 5)}
        if not required_bunks.issubset(sockets):
            findings.append(Finding("FOUR_BUNKS_REQUIRED", "Sleeping quarters must preserve four bunk sockets."))
        if sleeping.get("conceptStatus") != "keeper-missing-required":
            findings.append(
                Finding("BEDROOM_KEEPER_GAP_REQUIRED", "Bedroom remains explicitly keeper-missing until art lands.")
            )

    cockpit = room_by_id.get("cockpit")
    if cockpit:
        reach = cockpit.get("essentialControlReachMeters")
        if not isinstance(reach, dict):
            findings.append(Finding("CONTROL_REACH_REQUIRED", "Cockpit needs essentialControlReachMeters."))
        else:
            low = reach.get("min")
            high = reach.get("max")
            if not isinstance(low, (int, float)) or not isinstance(high, (int, float)):
                findings.append(Finding("CONTROL_REACH_INVALID", "Cockpit reach values must be numeric."))
            elif low < 0.45 or high > 0.95 or low >= high:
                findings.append(
                    Finding("CHILD_REACH_INVALID", "Essential cockpit controls must remain in the 0.45–0.95 m reach band.")
                )

    required_socket_rules = {
        "greenhouse": {"watering_can_rack", "automation_input", "automation_output"},
        "drive_core": {"artifact_key_joined", "coupler_core"},
        "workshop": {"fabricator_primary", "automation_input", "automation_output"},
        "airlock": {"dock_alignment", "grabber_arm_hardpoint"},
    }
    for room_id, required in required_socket_rules.items():
        room = room_by_id.get(room_id)
        if not room:
            continue
        sockets = set(room.get("requiredSockets", []))
        missing = sorted(required - sockets)
        if missing:
            findings.append(
                Finding("CANON_SOCKET_MISSING", f"{room_id} is missing canonical sockets {missing}.")
            )

    return Result(len(rooms), socket_count, tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--manifest", type=Path, default=Path("docs/project_art_plan/mk2_room_socket_manifest.json")
    )
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    manifest = args.manifest if args.manifest.is_absolute() else root / args.manifest
    try:
        result = validate_manifest(root, manifest)
    except InputError as exc:
        print(f"MK2_ROOM_MANIFEST_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"MK2_ROOM_MANIFEST_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(f"MK2_ROOM_MANIFEST_PASS rooms={result.room_count} sockets={result.socket_count}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
