#!/usr/bin/env python3
"""Resolve recovery inventory systems to exact C# source paths.

The inventory began from historical docs and intentionally contained descriptive or
incorrect source paths. This report indexes every declared C# type and resolves a
reviewed target-type list per system. It does not edit the inventory automatically.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Iterable, Sequence

EXIT_OK = 0
EXIT_ERROR = 1
DEFAULT_ROOTS = ("Ziptide/Assets/Ziptide", "Ziptide/Assets/ZiptideNet")
TYPE_RE = re.compile(
    r"^\s*(?:(?:public|internal|private|protected|static|sealed|abstract|partial|readonly)\s+)*"
    r"(?:class|struct|interface|enum)\s+([A-Za-z_]\w*)",
    re.MULTILINE,
)

SYSTEM_TARGETS: dict[str, tuple[str, ...]] = {
    "BOOT_FLOW": ("BootLoader",),
    "XR_RIG": ("PlayerRigPersistence", "LocomotionDirector", "EmergencyRespawn"),
    "HOME_HUB_UI": ("HomeHubRuntime", "HomeHubFlowState"),
    "DEV_MENU": ("DevWarpBoard", "DevMenu", "DevMenuGesture", "DevAccessGate", "QuickSwap"),
    "TRAVEL": ("TravelCoordinator", "ZiptideGateEffect", "WorldTravelStation"),
    "SAVE_PROFILE": ("SaveSystem", "PlayerProfile", "ProfileSerializer"),
    "PERSISTENT_CREDITS_HUD": ("CreditsHud",),
    "TRAVEL_STATION_UI": ("WorldTravelStation", "WorldLabelFacing"),
    "QUARTERS_UI": ("QuartersRoom", "QuartersCameraFeature", "QuartersPhotoWall"),
    "CONQUEST_TABLE_UI": ("ConquestTableRuntime", "ConquestSession"),
    "ITEM_PRESENTATION": ("ItemDefinition", "ItemFactory", "ItemRuntime", "ForgeVisualApplier"),
    "HOLSTER": ("BeltRig", "HolsterSocketInteractor", "InventoryState"),
    "WEAPON_FIRE": ("PistolRuntime", "TaserDartGunRuntime", "TaserDartProjectile"),
    "MELEE": ("HammerTool", "SonicThumperRuntime", "SonicThumper", "PvpHammer"),
    "ZIPLINE": ("ScenePatcherCavern", "ZiplineRuntime", "ZiplineCore"),
    "WORLD_FACTORY": ("WorldSpec", "WorldSpecCompiler", "WorldStubGenerator", "CityBuilder"),
    "SKY_AND_GRADE": ("SkyPlanetRig", "SkyVistaRig", "SkyGrade", "SkyVistaDefinition"),
    "PRACTICAL_LIGHTING": ("PracticalLight", "PracticalAuthor"),
    "ART_FORGE": ("ForgeVisualApplier", "ForgeRecipeLibrary", "ArtConformanceAuditRules"),
    "CREATURE_PRESENTATION_AND_GROUNDING": (
        "CreatureRuntime",
        "ForgeCreatureVisualApplier",
        "GroundShadow",
        "EcologyDirector",
    ),
    "SHIP_PRESENTATION_AND_REFIT": ("ShipHullBuilder", "ShipRefit", "ShipLocker"),
    "SHIP_FLIGHT": ("ShipFlightRuntime",),
    "REPAIR_OBJECTIVE": (
        "JobDirector",
        "JobRuntime",
        "ResourceBank",
        "WorldObjectiveBoard",
        "GateCoupler",
        "ShipCastOff",
    ),
    "PVP_AND_PHOTON": ("PvpMatchDirector", "PvpNetHub", "NetBootstrap", "PvpOnlinePresence"),
    "AUDIO": ("AudioDirector", "AmbienceDirector"),
    "CI_AND_AUDIT": ("BuildAndroid", "WorldAuditRunner", "UiReadabilityAuditRules"),
}


@dataclass(frozen=True)
class Resolution:
    system_id: str
    target_type: str
    status: str
    paths: tuple[str, ...]


def _source_files(root: Path, scan_roots: Iterable[str]) -> Iterable[Path]:
    for raw in scan_roots:
        base = root / raw
        if not base.exists():
            continue
        for path in base.rglob("*.cs"):
            if any(part in {"Library", "Temp", "Build", "Builds", "Obj"} for part in path.parts):
                continue
            yield path


def build_type_index(root: Path, scan_roots: Sequence[str]) -> dict[str, list[str]]:
    index: dict[str, list[str]] = {}
    for path in sorted(set(_source_files(root, scan_roots))):
        text = path.read_text(encoding="utf-8", errors="replace")
        relative = str(path.relative_to(root)).replace("\\", "/")
        for match in TYPE_RE.finditer(text):
            index.setdefault(match.group(1), []).append(relative)
    return {key: sorted(set(paths)) for key, paths in sorted(index.items())}


def resolve_systems(index: dict[str, list[str]]) -> list[Resolution]:
    results: list[Resolution] = []
    for system_id, targets in SYSTEM_TARGETS.items():
        for target in targets:
            paths = tuple(index.get(target, ()))
            status = "RESOLVED" if len(paths) == 1 else "AMBIGUOUS" if len(paths) > 1 else "MISSING"
            results.append(Resolution(system_id, target, status, paths))
    return results


def build_report(index: dict[str, list[str]], resolutions: list[Resolution]) -> dict:
    counts: dict[str, int] = {}
    for item in resolutions:
        counts[item.status] = counts.get(item.status, 0) + 1
    by_system: dict[str, list[dict]] = {}
    for item in resolutions:
        by_system.setdefault(item.system_id, []).append(asdict(item))
    return {
        "tool": "recovery_source_resolver",
        "indexedTypeCount": len(index),
        "targetCount": len(resolutions),
        "statusCounts": dict(sorted(counts.items())),
        "systems": {key: value for key, value in sorted(by_system.items())},
    }


def write_json(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def write_markdown(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Recovery Source Resolver",
        "",
        f"- Indexed C# types: **{report['indexedTypeCount']}**",
        f"- Requested type resolutions: **{report['targetCount']}**",
        "",
        "## Resolution counts",
        "",
    ]
    for status, count in report["statusCounts"].items():
        lines.append(f"- **{status}:** {count}")
    for system_id, entries in report["systems"].items():
        lines.extend(["", f"## {system_id}", ""])
        for item in entries:
            paths = ", ".join(f"`{p}`" for p in item["paths"]) or "none"
            lines.append(f"- `{item['target_type']}` — **{item['status']}** — {paths}")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--scan-root", action="append", dest="scan_roots")
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_source_resolver.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_source_resolver.md"),
    )
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    try:
        index = build_type_index(root, tuple(args.scan_roots or DEFAULT_ROOTS))
        report = build_report(index, resolve_systems(index))
        json_path = _resolve(root, args.json_report).resolve()
        markdown_path = _resolve(root, args.markdown_report).resolve()
        for report_path in (json_path, markdown_path):
            report_path.relative_to(root)
        write_json(report, json_path)
        write_markdown(report, markdown_path)
    except (OSError, ValueError) as exc:
        print(f"RECOVERY_SOURCE_RESOLVER_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_SOURCE_RESOLVER types={report['indexedTypeCount']} "
        f"targets={report['targetCount']} statuses={report['statusCounts']}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
