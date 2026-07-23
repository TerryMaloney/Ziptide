#!/usr/bin/env python3
"""Validate the machine-readable ZIPTIDE space mission catalog."""

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
CATEGORIES = {"core", "story-gated", "unique"}
CORE_IDS = {
    "salvage_run",
    "derelict_discovery",
    "ferry_passage",
    "distress_rescue",
    "relay_seal_repair",
}
ALL_IDS = CORE_IDS | {
    "warden_evasion",
    "escort_assist",
    "sable_faction_op",
    "gate_run",
    "the_approach",
}
ALLOWED_STEPS = {
    "AcceptCargo",
    "AlignGate",
    "AnswerBeacon",
    "ApproachEarth",
    "BoardArchitectShip",
    "ChooseBranch",
    "ChooseEnding",
    "Deliver",
    "DisableOrSabotage",
    "Dock",
    "DockOrInspect",
    "EscortTo",
    "EvadePatrol",
    "Extract",
    "InspectRelay",
    "NavigateTo",
    "ReachSafeLane",
    "RecoverLog",
    "Rendezvous",
    "RepairOrTow",
    "RepairRelay",
    "ScanDerelict",
    "StabilizeArrival",
    "TractorSalvage",
    "TraverseGate",
}
REQUIRED_SCOPE = {
    "flightModel": "arcade",
    "evaV1": False,
    "punitiveFuel": False,
    "targeting": "soft-lock",
    "hotasRequired": False,
    "combatRole": "optional-spice",
    "timedCoreMissions": False,
}
REQUIRED_FIELDS = (
    "id",
    "displayName",
    "category",
    "unlockChapter",
    "repeatable",
    "pacifistViable",
    "combatPosture",
    "faction",
    "storyTie",
    "steps",
    "rewardChannels",
    "signalEffect",
    "failurePolicy",
)


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    mission_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "space_mission_catalog_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "missionCount": self.mission_count,
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


def validate_catalog(path: Path) -> Result:
    data = _load(path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(
            Finding(
                "SCHEMA_VERSION_UNSUPPORTED",
                f"schemaVersion must equal {SCHEMA_VERSION}.",
                path.as_posix(),
            )
        )

    scope = data.get("scope")
    if not isinstance(scope, dict):
        findings.append(Finding("SCOPE_REQUIRED", "scope must be an object."))
        scope = {}
    for key, expected in REQUIRED_SCOPE.items():
        if scope.get(key) != expected:
            findings.append(
                Finding(
                    "SCOPE_LOCK_VIOLATION",
                    f"scope.{key} must remain {expected!r}; got {scope.get(key)!r}.",
                )
            )

    missions = data.get("missions")
    if not isinstance(missions, list):
        findings.append(Finding("MISSIONS_REQUIRED", "missions must be a list."))
        missions = []

    seen: set[str] = set()
    mission_by_id: dict[str, dict[str, Any]] = {}
    for index, mission in enumerate(missions):
        prefix = f"missions[{index}]"
        if not isinstance(mission, dict):
            findings.append(Finding("MISSION_INVALID", f"{prefix} must be an object."))
            continue

        for field in REQUIRED_FIELDS:
            if field not in mission:
                findings.append(Finding("FIELD_REQUIRED", f"{prefix}.{field} is required."))

        mission_id = mission.get("id")
        if not _nonempty(mission_id):
            findings.append(Finding("MISSION_ID_INVALID", f"{prefix}.id must be non-empty."))
            continue
        mission_id = mission_id.strip()
        if mission_id in seen:
            findings.append(Finding("MISSION_ID_DUPLICATE", f"Duplicate id {mission_id}."))
        seen.add(mission_id)
        mission_by_id[mission_id] = mission

        if mission.get("category") not in CATEGORIES:
            findings.append(Finding("CATEGORY_INVALID", f"{prefix}.category is invalid."))
        chapter = mission.get("unlockChapter")
        if not isinstance(chapter, int) or isinstance(chapter, bool) or not 1 <= chapter <= 8:
            findings.append(Finding("CHAPTER_INVALID", f"{prefix}.unlockChapter must be 1..8."))
        for field in ("repeatable", "pacifistViable"):
            if not isinstance(mission.get(field), bool):
                findings.append(Finding("BOOLEAN_REQUIRED", f"{prefix}.{field} must be boolean."))
        for field in ("displayName", "combatPosture", "faction", "storyTie", "signalEffect", "failurePolicy"):
            if not _nonempty(mission.get(field)):
                findings.append(Finding("TEXT_REQUIRED", f"{prefix}.{field} must be non-empty."))

        steps = mission.get("steps")
        if not isinstance(steps, list) or not steps:
            findings.append(Finding("STEPS_REQUIRED", f"{prefix}.steps must be non-empty."))
        else:
            for step in steps:
                if step not in ALLOWED_STEPS:
                    findings.append(
                        Finding("STEP_UNKNOWN", f"{prefix} uses unregistered step {step!r}.")
                    )

        rewards = mission.get("rewardChannels")
        if not isinstance(rewards, list) or not rewards or not all(_nonempty(v) for v in rewards):
            findings.append(
                Finding("REWARDS_REQUIRED", f"{prefix}.rewardChannels must be non-empty strings.")
            )

        failure = mission.get("failurePolicy")
        if _nonempty(failure) and "no-timer" not in failure:
            findings.append(
                Finding("TIMER_POLICY_VIOLATION", f"{prefix} must retain a no-timer failure policy.")
            )

    missing = sorted(ALL_IDS - seen)
    extra = sorted(seen - ALL_IDS)
    if missing:
        findings.append(Finding("CANON_MISSIONS_MISSING", f"Missing canonical missions: {missing}."))
    if extra:
        findings.append(
            Finding(
                "UNREVIEWED_MISSIONS_PRESENT",
                f"Catalog contains unreviewed mission ids: {extra}; amend the canon before adding them.",
            )
        )
    if len(missions) != 10:
        findings.append(Finding("MISSION_COUNT_INVALID", f"Expected 10 missions; found {len(missions)}."))

    for mission_id in CORE_IDS:
        mission = mission_by_id.get(mission_id)
        if not mission:
            continue
        if mission.get("category") != "core":
            findings.append(Finding("CORE_CATEGORY_INVALID", f"{mission_id} must remain core."))
        if mission.get("repeatable") is not True:
            findings.append(Finding("CORE_REPEATABLE_REQUIRED", f"{mission_id} must be repeatable."))
        if mission.get("pacifistViable") is not True:
            findings.append(Finding("CORE_PACIFIST_REQUIRED", f"{mission_id} must be pacifist-viable."))

    relay = mission_by_id.get("relay_seal_repair")
    if relay and relay.get("signalEffect") != "raises":
        findings.append(
            Finding("SIGNAL_HOOK_REQUIRED", "relay_seal_repair must explicitly raise Signal.")
        )

    approach = mission_by_id.get("the_approach")
    if approach:
        if approach.get("category") != "unique" or approach.get("repeatable") is not False:
            findings.append(
                Finding("APPROACH_UNIQUE_REQUIRED", "the_approach must remain unique and non-repeatable.")
            )
        if approach.get("unlockChapter") != 8:
            findings.append(Finding("APPROACH_CHAPTER_INVALID", "the_approach must unlock in chapter 8."))

    sable = mission_by_id.get("sable_faction_op")
    if sable and sable.get("pacifistViable") is not True:
        findings.append(
            Finding("SABLE_BRANCH_REQUIRED", "Sable operations must preserve ally/oppose/refuse completion.")
        )

    return Result(len(missions), tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--catalog", type=Path, default=Path("docs/design/space_mission_catalog.json")
    )
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    try:
        result = validate_catalog(catalog)
    except InputError as exc:
        print(f"SPACE_MISSION_CATALOG_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"SPACE_MISSION_CATALOG_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(f"SPACE_MISSION_CATALOG_PASS missions={result.mission_count}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
