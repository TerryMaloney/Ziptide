#!/usr/bin/env python3
"""Validate celestial star data against the freeze-safe SunRig contract.

Structural failures block immediately. Explicit author-required placeholders and fields not yet
migrated into catalog schema v1 remain warnings until Terry authors the Moss sun values.
"""

from __future__ import annotations

import argparse
import json
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Sequence

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2
SCHEMA_VERSION = 1
AUTHOR_PREFIX = "author-required"


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""
    severity: str = "blocker"


@dataclass(frozen=True)
class Result:
    system_count: int
    star_count: int
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
            return "failure"
        if self.warnings:
            return "warning"
        return "pass"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "sunrig_contract_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "systemCount": self.system_count,
            "starCount": self.star_count,
            "blockerCount": len(self.blockers),
            "warningCount": len(self.warnings),
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


def _author_required(value: Any) -> bool:
    return _text(value) and value.strip().lower().startswith(AUTHOR_PREFIX)


def _number(value: Any) -> bool:
    return isinstance(value, (int, float)) and not isinstance(value, bool)


def validate_catalog(path: Path) -> Result:
    data = _load(path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(
            Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1.")
        )

    systems = data.get("systems")
    if not isinstance(systems, list) or not systems:
        findings.append(Finding("SYSTEMS_REQUIRED", "systems must be a non-empty list."))
        systems = []

    placements = data.get("worldPlacements")
    if not isinstance(placements, list):
        findings.append(
            Finding("WORLD_PLACEMENTS_REQUIRED", "worldPlacements must be a list.")
        )
        placements = []

    views = data.get("spaceViews")
    if not isinstance(views, list):
        findings.append(Finding("SPACE_VIEWS_REQUIRED", "spaceViews must be a list."))
        views = []

    placements_by_system: dict[str, list[dict[str, Any]]] = {}
    placements_by_space_view: dict[str, dict[str, Any]] = {}
    for index, placement in enumerate(placements):
        if not isinstance(placement, dict):
            findings.append(
                Finding("WORLD_PLACEMENT_INVALID", f"worldPlacements[{index}] must be an object.")
            )
            continue
        system_id = placement.get("systemId")
        if _text(system_id):
            placements_by_system.setdefault(system_id.strip(), []).append(placement)
        space_scene = placement.get("spaceSceneId")
        if _text(space_scene):
            placements_by_space_view[space_scene.strip()] = placement

    system_stars: dict[str, set[str]] = {}
    total_stars = 0
    for system_index, system in enumerate(systems):
        prefix = f"systems[{system_index}]"
        if not isinstance(system, dict):
            findings.append(Finding("SYSTEM_INVALID", f"{prefix} must be an object."))
            continue
        system_id = system.get("id")
        if not _text(system_id):
            findings.append(Finding("SYSTEM_ID_INVALID", f"{prefix}.id must be non-empty."))
            continue
        system_id = system_id.strip()

        stars = system.get("stars")
        if not isinstance(stars, list):
            findings.append(Finding("STARS_REQUIRED", f"{system_id}.stars must be a list."))
            stars = []
        total_stars += len(stars)
        if len(stars) < 1 or len(stars) > 2:
            findings.append(
                Finding(
                    "STAR_COUNT_INVALID",
                    f"{system_id} must declare one or two stars; found {len(stars)}.",
                )
            )

        by_id: dict[str, dict[str, Any]] = {}
        primary_ids: list[str] = []
        secondary_ids: list[str] = []
        for star_index, star in enumerate(stars):
            star_prefix = f"{system_id}.stars[{star_index}]"
            if not isinstance(star, dict):
                findings.append(Finding("STAR_INVALID", f"{star_prefix} must be an object."))
                continue
            star_id = star.get("id")
            if not _text(star_id):
                findings.append(Finding("STAR_ID_INVALID", f"{star_prefix}.id must be non-empty."))
                continue
            star_id = star_id.strip()
            if star_id in by_id:
                findings.append(Finding("STAR_ID_DUPLICATE", f"Duplicate star id {star_id}."))
            by_id[star_id] = star

            count_role = star.get("countRole")
            if count_role == "primary":
                primary_ids.append(star_id)
                if star.get("lightRole") != "SunRig-primary-key":
                    findings.append(
                        Finding(
                            "PRIMARY_LIGHT_ROLE_INVALID",
                            f"{star_id} must use lightRole SunRig-primary-key.",
                        )
                    )
            elif count_role == "secondary":
                secondary_ids.append(star_id)
                if star.get("lightRole") not in {
                    "SunRig-secondary-fill",
                    "SunRig-secondary-key",
                }:
                    findings.append(
                        Finding(
                            "SECONDARY_LIGHT_ROLE_INVALID",
                            f"{star_id} must use a SunRig secondary light role.",
                        )
                    )
            else:
                findings.append(
                    Finding(
                        "COUNT_ROLE_INVALID",
                        f"{star_id}.countRole must be primary or secondary.",
                    )
                )

            if star.get("discRole") != "shared-ground-space":
                findings.append(
                    Finding(
                        "DISC_ROLE_INVALID",
                        f"{star_id}.discRole must remain shared-ground-space.",
                    )
                )
            if not _text(star.get("temperatureFamily")):
                findings.append(
                    Finding(
                        "TEMPERATURE_FAMILY_REQUIRED",
                        f"{star_id}.temperatureFamily must be non-empty.",
                    )
                )

            for field in ("exactColor", "exactDiscAngularSize"):
                value = star.get(field)
                if not _text(value) and not _number(value) and not isinstance(value, list):
                    findings.append(
                        Finding("STAR_FIELD_REQUIRED", f"{star_id}.{field} is required.")
                    )
                elif _author_required(value):
                    findings.append(
                        Finding(
                            "AUTHOR_FIELD_UNRESOLVED",
                            f"{star_id}.{field} still requires Terry's authored value.",
                            severity="warning",
                        )
                    )

            star_direction = star.get("direction")
            matching_placements = [
                placement
                for placement in placements_by_system.get(system_id, [])
                if placement.get("primaryStar") == star_id
            ]
            if star_direction is None:
                if not matching_placements:
                    findings.append(
                        Finding(
                            "STAR_DIRECTION_MISSING",
                            f"{star_id} has no star.direction and no world placement direction fallback.",
                        )
                    )
                else:
                    findings.append(
                        Finding(
                            "STAR_FIELD_NOT_YET_MIGRATED",
                            f"{star_id}.direction is still supplied by worldPlacements[].sunDirection; migrate it into stars[].",
                            severity="warning",
                        )
                    )
                    for placement in matching_placements:
                        value = placement.get("sunDirection")
                        if not _text(value):
                            findings.append(
                                Finding(
                                    "PLACEMENT_SUN_DIRECTION_REQUIRED",
                                    f"{placement.get('worldId', '<unknown>')}.sunDirection is required.",
                                )
                            )
                        elif _author_required(value):
                            findings.append(
                                Finding(
                                    "AUTHOR_FIELD_UNRESOLVED",
                                    f"{placement.get('worldId', '<unknown>')}.sunDirection still requires keeper-derived authoring.",
                                    severity="warning",
                                )
                            )
            elif _author_required(star_direction):
                findings.append(
                    Finding(
                        "AUTHOR_FIELD_UNRESOLVED",
                        f"{star_id}.direction still requires keeper-derived authoring.",
                        severity="warning",
                    )
                )

            for field in ("keyIntensity", "flareProfile", "groundHorizonWarm"):
                value = star.get(field)
                if value is None:
                    findings.append(
                        Finding(
                            "STAR_FIELD_NOT_YET_MIGRATED",
                            f"{star_id}.{field} is required by SUNRIG_CONTRACT and has not yet migrated into catalog schema v1.",
                            severity="warning",
                        )
                    )
                elif _author_required(value):
                    findings.append(
                        Finding(
                            "AUTHOR_FIELD_UNRESOLVED",
                            f"{star_id}.{field} still requires Terry's authored value.",
                            severity="warning",
                        )
                    )

        if len(primary_ids) != 1:
            findings.append(
                Finding(
                    "PRIMARY_STAR_COUNT_INVALID",
                    f"{system_id} must have exactly one primary star; found {len(primary_ids)}.",
                )
            )
        if len(secondary_ids) > 1:
            findings.append(
                Finding(
                    "SECONDARY_STAR_COUNT_INVALID",
                    f"{system_id} may have at most one secondary star.",
                )
            )
        system_stars[system_id] = set(by_id)

        for placement in placements_by_system.get(system_id, []):
            primary = placement.get("primaryStar")
            if primary not in by_id:
                findings.append(
                    Finding(
                        "PLACEMENT_PRIMARY_STAR_MISSING",
                        f"{placement.get('worldId', '<unknown>')} references unknown primaryStar {primary!r}.",
                    )
                )
            elif primary not in primary_ids:
                findings.append(
                    Finding(
                        "PLACEMENT_PRIMARY_ROLE_INVALID",
                        f"{placement.get('worldId', '<unknown>')} primaryStar {primary!r} is not countRole primary.",
                    )
                )

    for view_index, view in enumerate(views):
        if not isinstance(view, dict):
            findings.append(Finding("SPACE_VIEW_INVALID", f"spaceViews[{view_index}] must be an object."))
            continue
        view_id = view.get("id")
        system_id = view.get("systemId")
        if not _text(view_id) or not _text(system_id):
            findings.append(
                Finding("SPACE_VIEW_IDENTITY_REQUIRED", f"spaceViews[{view_index}] needs id and systemId.")
            )
            continue
        view_id = view_id.strip()
        system_id = system_id.strip()
        primary = view.get("primaryStar")
        if primary not in system_stars.get(system_id, set()):
            findings.append(
                Finding(
                    "SPACE_VIEW_PRIMARY_STAR_MISSING",
                    f"{view_id} references unknown primaryStar {primary!r} for {system_id}.",
                )
            )
        placement = placements_by_space_view.get(view_id)
        if placement is None:
            findings.append(
                Finding(
                    "SPACE_VIEW_GROUND_PAIR_MISSING",
                    f"{view_id} has no worldPlacement sharing its spaceSceneId.",
                )
            )
        elif placement.get("primaryStar") != primary:
            findings.append(
                Finding(
                    "GROUND_SPACE_STAR_MISMATCH",
                    f"{view_id} primaryStar differs from {placement.get('worldId', '<unknown>')}.",
                )
            )
        if view.get("sunBearingMatchesGround") is not True:
            findings.append(
                Finding(
                    "SUN_BEARING_PARITY_REQUIRED",
                    f"{view_id}.sunBearingMatchesGround must remain true.",
                )
            )

    return Result(len(systems), total_stars, tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--catalog",
        type=Path,
        default=Path("docs/design/celestial_system_catalog.json"),
    )
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--json-report", type=Path)
    parser.add_argument(
        "--strict-authoring",
        action="store_true",
        help="Treat unresolved authoring warnings as validation failures.",
    )
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    try:
        result = validate_catalog(catalog)
    except InputError as exc:
        print(f"SUNRIG_CONTRACT_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    for finding in result.findings:
        print(
            f"SUNRIG_CONTRACT_{finding.severity.upper()} "
            f"{finding.code}: {finding.message}"
        )

    if result.blockers:
        return EXIT_VALIDATION_FAILED
    if args.strict_authoring and result.warnings:
        return EXIT_VALIDATION_FAILED

    print(
        "SUNRIG_CONTRACT_PASS "
        f"systems={result.system_count} stars={result.star_count} "
        f"warnings={len(result.warnings)}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
