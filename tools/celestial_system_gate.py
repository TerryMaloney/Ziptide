#!/usr/bin/env python3
"""Validate shared ground/space celestial-system contracts."""

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
DECISION_STATUSES = {"proposed-canon", "locked-canon"}
PARALLAX_BANDS = {"near-launch-body", "mid-body", "distant-major-body", "deep-background"}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    system_count: int
    world_count: int
    space_view_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "celestial_system_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "systemCount": self.system_count,
            "worldCount": self.world_count,
            "spaceViewCount": self.space_view_count,
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
        raise InputError("Catalog root must be an object.")
    return data


def _nonempty(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _unique_index(
    items: Any,
    *,
    collection_name: str,
    id_field: str,
    findings: list[Finding],
) -> dict[str, dict[str, Any]]:
    if not isinstance(items, list) or not items:
        findings.append(Finding("COLLECTION_REQUIRED", f"{collection_name} must be non-empty."))
        return {}
    result: dict[str, dict[str, Any]] = {}
    for index, item in enumerate(items):
        prefix = f"{collection_name}[{index}]"
        if not isinstance(item, dict):
            findings.append(Finding("ITEM_INVALID", f"{prefix} must be an object."))
            continue
        item_id = item.get(id_field)
        if not _nonempty(item_id):
            findings.append(Finding("ID_INVALID", f"{prefix}.{id_field} must be non-empty."))
            continue
        item_id = item_id.strip()
        if item_id in result:
            findings.append(Finding("ID_DUPLICATE", f"Duplicate {collection_name} id {item_id}."))
        result[item_id] = item
    return result


def validate_catalog(path: Path) -> Result:
    data = _load(path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(
            Finding("SCHEMA_VERSION_UNSUPPORTED", f"schemaVersion must equal {SCHEMA_VERSION}.")
        )

    systems = _unique_index(
        data.get("systems"), collection_name="systems", id_field="id", findings=findings
    )
    worlds = _unique_index(
        data.get("worldPlacements"),
        collection_name="worldPlacements",
        id_field="worldId",
        findings=findings,
    )
    views = _unique_index(
        data.get("spaceViews"), collection_name="spaceViews", id_field="id", findings=findings
    )

    bodies_by_system: dict[str, dict[str, dict[str, Any]]] = {}
    stars_by_system: dict[str, set[str]] = {}
    backgrounds_by_system: dict[str, str] = {}

    for system_id, system in systems.items():
        if system.get("decisionStatus") not in DECISION_STATUSES:
            findings.append(
                Finding("DECISION_STATUS_INVALID", f"System {system_id} has invalid decisionStatus.")
            )
        if not _nonempty(system.get("displayName")):
            findings.append(Finding("DISPLAY_NAME_REQUIRED", f"System {system_id} lacks displayName."))

        stars = system.get("stars")
        if not isinstance(stars, list) or not 1 <= len(stars) <= 2:
            findings.append(Finding("STAR_COUNT_INVALID", f"System {system_id} must have 1–2 stars."))
            stars = []
        star_ids: set[str] = set()
        for index, star in enumerate(stars):
            if not isinstance(star, dict) or not _nonempty(star.get("id")):
                findings.append(Finding("STAR_INVALID", f"System {system_id} star {index} is invalid."))
                continue
            star_id = star["id"].strip()
            if star_id in star_ids:
                findings.append(Finding("STAR_ID_DUPLICATE", f"System {system_id} repeats star {star_id}."))
            star_ids.add(star_id)
            for field in ("temperatureFamily", "lightRole", "discRole"):
                if not _nonempty(star.get(field)):
                    findings.append(
                        Finding("STAR_FIELD_REQUIRED", f"System {system_id} star {star_id} lacks {field}.")
                    )
        stars_by_system[system_id] = star_ids

        bodies = system.get("bodies")
        if not isinstance(bodies, list) or not bodies:
            findings.append(Finding("BODIES_REQUIRED", f"System {system_id} must declare bodies."))
            bodies = []
        body_index: dict[str, dict[str, Any]] = {}
        for index, body in enumerate(bodies):
            if not isinstance(body, dict) or not _nonempty(body.get("id")):
                findings.append(Finding("BODY_INVALID", f"System {system_id} body {index} is invalid."))
                continue
            body_id = body["id"].strip()
            if body_id in body_index:
                findings.append(Finding("BODY_ID_DUPLICATE", f"System {system_id} repeats {body_id}."))
            body_index[body_id] = body
            if body.get("parallaxBand") not in PARALLAX_BANDS:
                findings.append(
                    Finding("PARALLAX_BAND_INVALID", f"Body {body_id} has invalid parallaxBand.")
                )
        for body_id, body in body_index.items():
            parent = body.get("parentBody")
            if parent is not None and parent not in body_index:
                findings.append(
                    Finding("BODY_PARENT_MISSING", f"Body {body_id} references missing parent {parent}.")
                )
        bodies_by_system[system_id] = body_index

        background = system.get("deepBackground")
        if not isinstance(background, dict):
            findings.append(Finding("BACKGROUND_REQUIRED", f"System {system_id} lacks deepBackground."))
        else:
            background_id = background.get("starfieldId")
            if not _nonempty(background_id):
                findings.append(Finding("STARFIELD_REQUIRED", f"System {system_id} lacks starfieldId."))
            else:
                backgrounds_by_system[system_id] = background_id.strip()
            if background.get("sharedAcrossSystem") is not True:
                findings.append(
                    Finding("BACKGROUND_SHARE_REQUIRED", f"System {system_id} must share one background.")
                )

        palette = system.get("spaceObjectPalette")
        if not isinstance(palette, dict):
            findings.append(Finding("OBJECT_PALETTE_REQUIRED", f"System {system_id} lacks object palette."))
        else:
            for field in ("asteroidFamilies", "derelictClasses", "trafficFactions"):
                values = palette.get(field)
                if not isinstance(values, list) or not values:
                    findings.append(
                        Finding("PALETTE_FIELD_REQUIRED", f"System {system_id} palette lacks {field}.")
                    )
            if palette.get("groundSpaceEchoRequired") is not True:
                findings.append(
                    Finding("GROUND_SPACE_ECHO_REQUIRED", f"System {system_id} must preserve object echoes.")
                )

    for world_id, world in worlds.items():
        system_id = world.get("systemId")
        if system_id not in systems:
            findings.append(Finding("WORLD_SYSTEM_MISSING", f"World {world_id} references {system_id!r}."))
            continue
        if world.get("decisionStatus") not in DECISION_STATUSES:
            findings.append(Finding("DECISION_STATUS_INVALID", f"World {world_id} has invalid status."))
        body_index = bodies_by_system.get(system_id, {})
        star_ids = stars_by_system.get(system_id, set())
        for field in ("bodyId", "parentBody"):
            if world.get(field) not in body_index:
                findings.append(
                    Finding("WORLD_BODY_MISSING", f"World {world_id} {field} references {world.get(field)!r}.")
                )
        siblings = world.get("visibleSiblingBodies")
        if not isinstance(siblings, list):
            findings.append(Finding("VISIBLE_BODIES_REQUIRED", f"World {world_id} needs visibleSiblingBodies."))
            siblings = []
        for sibling in siblings:
            if sibling not in body_index:
                findings.append(
                    Finding("VISIBLE_BODY_MISSING", f"World {world_id} references missing {sibling}.")
                )
        if world.get("primaryStar") not in star_ids:
            findings.append(Finding("WORLD_STAR_MISSING", f"World {world_id} primaryStar is invalid."))
        if world.get("groundVistaMode") != "derived-through-atmosphere":
            findings.append(
                Finding("GROUND_VIEW_DERIVATION_REQUIRED", f"World {world_id} must derive its ground view.")
            )
        intensity = world.get("shellGridIntensity")
        if not isinstance(intensity, (int, float)) or isinstance(intensity, bool) or not 0 <= intensity <= 1:
            findings.append(Finding("SHELL_GRID_RANGE", f"World {world_id} shellGridIntensity must be 0..1."))
        if world.get("spaceSceneId") not in views:
            findings.append(Finding("WORLD_SPACE_VIEW_MISSING", f"World {world_id} lacks a valid spaceSceneId."))

    for view_id, view in views.items():
        system_id = view.get("systemId")
        if system_id not in systems:
            findings.append(Finding("VIEW_SYSTEM_MISSING", f"Space view {view_id} references {system_id!r}."))
            continue
        body_index = bodies_by_system.get(system_id, {})
        star_ids = stars_by_system.get(system_id, set())
        for body_field in ("originBody",):
            if view.get(body_field) not in body_index:
                findings.append(Finding("VIEW_BODY_MISSING", f"Space view {view_id} {body_field} invalid."))
        required = view.get("requiredBodies")
        if not isinstance(required, list) or not required:
            findings.append(Finding("VIEW_REQUIRED_BODIES", f"Space view {view_id} needs bodies."))
            required = []
        for body_id in required:
            if body_id not in body_index:
                findings.append(Finding("VIEW_BODY_MISSING", f"Space view {view_id} references {body_id}."))
        if view.get("primaryStar") not in star_ids:
            findings.append(Finding("VIEW_STAR_MISSING", f"Space view {view_id} primaryStar invalid."))
        if view.get("backgroundId") != backgrounds_by_system.get(system_id):
            findings.append(
                Finding("BACKGROUND_MISMATCH", f"Space view {view_id} does not use system background.")
            )
        for flag in ("atmosphereRemoved", "fullHorizon", "sunBearingMatchesGround", "bodyBearingsMatchGround"):
            if view.get(flag) is not True:
                findings.append(Finding("VIEW_PARITY_REQUIRED", f"Space view {view_id} must set {flag}=true."))

    moss = worlds.get("W001")
    if moss:
        if moss.get("systemId") != "moss_system_working":
            findings.append(Finding("MOSS_SYSTEM_DRIFT", "W001 must remain in moss_system_working."))
        if moss.get("parentBody") != "moss_ringed_giant":
            findings.append(Finding("MOSS_PARENT_DRIFT", "W001 must remain a moon of the ringed giant."))
        if "moss_sibling_grey_moon" not in moss.get("visibleSiblingBodies", []):
            findings.append(Finding("MOSS_SIBLING_REQUIRED", "W001 must retain the sibling grey moon."))
        if moss.get("shellGridIntensity") != 0.0:
            findings.append(Finding("MOSS_GRID_BASELINE", "W001 shell grid baseline must remain 0."))
        system = systems.get("moss_system_working", {})
        if len(system.get("stars", [])) != 1:
            findings.append(Finding("MOSS_SINGLE_STAR_REQUIRED", "W001 begins under one warm star."))

    return Result(len(systems), len(worlds), len(views), tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--catalog", type=Path, default=Path("docs/design/celestial_system_catalog.json")
    )
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    try:
        result = validate_catalog(catalog)
    except InputError as exc:
        print(f"CELESTIAL_SYSTEM_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"CELESTIAL_SYSTEM_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(
        "CELESTIAL_SYSTEM_PASS "
        f"systems={result.system_count} worlds={result.world_count} views={result.space_view_count}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
