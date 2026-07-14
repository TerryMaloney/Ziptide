#!/usr/bin/env python3
"""Inventory the Unity build-scene exposure for ZIPTIDE recovery R0.

The tool is report-only. It parses EditorBuildSettings.asset and classifies enabled
scenes by recovery exposure so prototype content cannot be mistaken for hidden just
because the system inventory says it should be hidden.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Sequence

EXIT_OK = 0
EXIT_ERROR = 1

_ENABLED_RE = re.compile(r"^\s*-\s+enabled:\s+([01])\s*$")
_PATH_RE = re.compile(r"^\s*path:\s+(.+?)\s*$")


@dataclass(frozen=True)
class SceneEntry:
    index: int
    enabled: bool
    path: str
    scene_name: str
    exposure: str
    reason: str


def classify_scene(path: str) -> tuple[str, str]:
    name = Path(path).stem
    normalized = path.replace("\\", "/")
    if name == "_Boot":
        return "GOLDEN_PATH_SUPPORT", "Persistent bootstrap scene."
    if name == "W000_DriftIn":
        return "GOLDEN_PATH", "Required first world in the recovery slice."
    if name in {"ToxicCity", "W002_DryCistern"}:
        return "GOLDEN_DESTINATION_CANDIDATE", "Potential single destination; R0 must select exactly one."
    if name in {"MilestoneA_GrabCube", "D0_City", "SandboxTestLab", "StarterWorld"}:
        return "LEGACY_TEST_EXPOSED", "Legacy/test scene remains enabled in the APK."
    if name == "PvP_Arena01" or "/Arenas/" in normalized:
        return "PROTOTYPE_MULTIPLAYER_EXPOSED", "Multiplayer is frozen and should be hidden from recovery candidates."
    if re.fullmatch(r"W\d{3}_.+", name):
        return "PROTOTYPE_WORLD_EXPOSED", "Non-golden story world remains enabled before integration proof."
    return "UNCLASSIFIED_EXPOSED", "Scene is enabled but has no recovery classification."


def parse_build_settings(text: str) -> list[SceneEntry]:
    lines = text.splitlines()
    entries: list[SceneEntry] = []
    pending_enabled: bool | None = None
    for line in lines:
        enabled_match = _ENABLED_RE.match(line)
        if enabled_match:
            pending_enabled = enabled_match.group(1) == "1"
            continue
        path_match = _PATH_RE.match(line)
        if path_match and pending_enabled is not None:
            path = path_match.group(1).strip()
            exposure, reason = classify_scene(path)
            entries.append(
                SceneEntry(
                    index=len(entries),
                    enabled=pending_enabled,
                    path=path,
                    scene_name=Path(path).stem,
                    exposure=exposure,
                    reason=reason,
                )
            )
            pending_enabled = None
    return entries


def build_report(entries: list[SceneEntry]) -> dict:
    enabled = [entry for entry in entries if entry.enabled]
    counts: dict[str, int] = {}
    for entry in enabled:
        counts[entry.exposure] = counts.get(entry.exposure, 0) + 1
    hidden_but_enabled = [
        entry
        for entry in enabled
        if entry.exposure
        in {
            "LEGACY_TEST_EXPOSED",
            "PROTOTYPE_MULTIPLAYER_EXPOSED",
            "PROTOTYPE_WORLD_EXPOSED",
            "UNCLASSIFIED_EXPOSED",
        }
    ]
    candidates = [entry for entry in enabled if entry.exposure == "GOLDEN_DESTINATION_CANDIDATE"]
    return {
        "tool": "recovery_scene_exposure",
        "sceneCount": len(entries),
        "enabledCount": len(enabled),
        "exposureCounts": dict(sorted(counts.items())),
        "hiddenButEnabledCount": len(hidden_but_enabled),
        "goldenDestinationCandidateCount": len(candidates),
        "findings": {
            "hiddenButEnabled": [asdict(entry) for entry in hidden_but_enabled],
            "goldenDestinationCandidates": [asdict(entry) for entry in candidates],
        },
        "scenes": [asdict(entry) for entry in entries],
    }


def write_json(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def write_markdown(report: dict, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = [
        "# ZIPTIDE Recovery Scene Exposure",
        "",
        f"- Build-settings scenes: **{report['sceneCount']}**",
        f"- Enabled: **{report['enabledCount']}**",
        f"- Prototype/legacy/unclassified scenes still enabled: **{report['hiddenButEnabledCount']}**",
        f"- Golden destination candidates still unresolved: **{report['goldenDestinationCandidateCount']}**",
        "",
        "## Exposure counts",
        "",
    ]
    for exposure, count in report["exposureCounts"].items():
        lines.append(f"- **{exposure}:** {count}")
    lines.extend(["", "## Enabled scene table", "", "| # | Scene | Recovery exposure | Reason |", "|---:|---|---|---|"])
    for scene in report["scenes"]:
        if not scene["enabled"]:
            continue
        lines.append(
            f"| {scene['index']} | `{scene['scene_name']}` | `{scene['exposure']}` | {scene['reason']} |"
        )
    lines.extend(
        [
            "",
            "## Recovery conclusion",
            "",
            "The source inventory can classify a system as hidden while its scene remains enabled in the APK. "
            "R1/R3 therefore need a generated recovery build profile or an equivalent exposure gate; R0 does not edit build settings.",
        ]
    )
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def _resolve(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--build-settings",
        type=Path,
        default=Path("Ziptide/ProjectSettings/EditorBuildSettings.asset"),
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_scene_exposure.json"),
    )
    parser.add_argument(
        "--markdown-report",
        type=Path,
        default=Path("Builds/Reports/recovery_scene_exposure.md"),
    )
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    try:
        settings_path = _resolve(root, args.build_settings)
        text = settings_path.read_text(encoding="utf-8")
        entries = parse_build_settings(text)
        if not entries:
            raise ValueError("No scene entries found in EditorBuildSettings.asset.")
        report = build_report(entries)
        json_path = _resolve(root, args.json_report).resolve()
        md_path = _resolve(root, args.markdown_report).resolve()
        for report_path in (json_path, md_path):
            report_path.relative_to(root)
        write_json(report, json_path)
        write_markdown(report, md_path)
    except (OSError, ValueError) as exc:
        print(f"RECOVERY_SCENE_EXPOSURE_ERROR {exc}", file=sys.stderr)
        return EXIT_ERROR
    print(
        f"RECOVERY_SCENE_EXPOSURE scenes={report['sceneCount']} enabled={report['enabledCount']} "
        f"hiddenButEnabled={report['hiddenButEnabledCount']}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
