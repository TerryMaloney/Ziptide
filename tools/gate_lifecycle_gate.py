#!/usr/bin/env python3
"""Validate the ZIPTIDE gate visual lifecycle and comfort rails."""

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
STATE_ORDER = (
    "dormant",
    "waking",
    "open",
    "traversal",
    "exit_reform",
    "collapse",
    "entrainment_surge",
)
FULL_VIEW_STATES = {"traversal", "exit_reform"}
REQUIRED_TRANSITIONS = {
    ("dormant", "waking"),
    ("waking", "open"),
    ("open", "traversal"),
    ("traversal", "exit_reform"),
    ("exit_reform", "open"),
    ("open", "collapse"),
    ("collapse", "dormant"),
    ("open", "entrainment_surge"),
    ("entrainment_surge", "open"),
}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    state_count: int
    transition_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "gate_lifecycle_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "stateCount": self.state_count,
            "transitionCount": self.transition_count,
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


def validate_catalog(path: Path) -> Result:
    data = _load(path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))
    if data.get("gateId") != "ziptide_gate_hero":
        findings.append(Finding("GATE_ID_INVALID", "gateId must remain ziptide_gate_hero."))
    if data.get("transitionOwner") != "TravelCoordinator":
        findings.append(
            Finding("TRAVEL_OWNER_INVALID", "TravelCoordinator must own traversal state changes.")
        )
    if data.get("cameraOwner") != "XR-rig":
        findings.append(Finding("CAMERA_OWNER_INVALID", "The XR rig must remain camera owner."))

    rails = data.get("globalRails")
    if not isinstance(rails, dict):
        findings.append(Finding("GLOBAL_RAILS_REQUIRED", "globalRails must be an object."))
        rails = {}
    exact_rails = {
        "maxTransparentCoverage": 0.35,
        "maxLiveParticles": 64,
        "maxLiveParticleSystems": 6,
        "minimumFadeSeconds": 0.25,
        "cameraMotionAllowed": False,
        "forcedFovChangeAllowed": False,
        "fullFieldStrobeAllowed": False,
        "restoreControlBeforeStableFloor": False,
        "childReachControlMaxMeters": 0.95,
    }
    for key, expected in exact_rails.items():
        if rails.get(key) != expected:
            findings.append(
                Finding(
                    "GLOBAL_RAIL_DRIFT",
                    f"globalRails.{key} must remain {expected!r}; got {rails.get(key)!r}.",
                )
            )

    states = data.get("states")
    if not isinstance(states, list):
        findings.append(Finding("STATES_REQUIRED", "states must be a list."))
        states = []
    by_id: dict[str, dict[str, Any]] = {}
    for index, state in enumerate(states):
        prefix = f"states[{index}]"
        if not isinstance(state, dict):
            findings.append(Finding("STATE_INVALID", f"{prefix} must be an object."))
            continue
        state_id = state.get("id")
        if not _nonempty(state_id):
            findings.append(Finding("STATE_ID_INVALID", f"{prefix}.id must be non-empty."))
            continue
        state_id = state_id.strip()
        if state_id in by_id:
            findings.append(Finding("STATE_ID_DUPLICATE", f"Duplicate state {state_id}."))
        by_id[state_id] = state
        if state.get("sequence") != index + 1:
            findings.append(
                Finding("STATE_SEQUENCE_INVALID", f"{state_id} sequence must be {index + 1}.")
            )
        if not isinstance(state.get("looping"), bool):
            findings.append(Finding("LOOPING_BOOL_REQUIRED", f"{state_id}.looping must be boolean."))

        duration = state.get("durationSeconds")
        if not isinstance(duration, dict):
            findings.append(Finding("DURATION_REQUIRED", f"{state_id} lacks durationSeconds."))
            duration = {}
        low = duration.get("min")
        high = duration.get("max")
        if not _number(low) or not _number(high) or low < 0 or high < 0 or low > high:
            findings.append(Finding("DURATION_INVALID", f"{state_id} has invalid duration range."))
        if not isinstance(duration.get("authorDriven"), bool):
            findings.append(
                Finding("AUTHOR_DRIVEN_BOOL_REQUIRED", f"{state_id}.durationSeconds.authorDriven must be boolean.")
            )

        layers = state.get("layers")
        if not isinstance(layers, list) or not layers or not all(_nonempty(item) for item in layers):
            findings.append(Finding("LAYERS_REQUIRED", f"{state_id} must declare visual layers."))
        elif len(layers) != len(set(layers)):
            findings.append(Finding("LAYER_DUPLICATE", f"{state_id} repeats a visual layer."))

        coverage = state.get("transparentCoverageMax")
        if not _number(coverage) or coverage < 0 or coverage > 1:
            findings.append(Finding("TRANSPARENCY_RANGE", f"{state_id} coverage must be 0..1."))
        elif state_id not in FULL_VIEW_STATES and coverage > rails.get("maxTransparentCoverage", 0.35):
            findings.append(
                Finding(
                    "TRANSPARENCY_CAP_EXCEEDED",
                    f"{state_id} exceeds normal transparent coverage rail.",
                )
            )
        elif state_id in FULL_VIEW_STATES and "travel_shell" not in layers and state_id == "traversal":
            findings.append(
                Finding("TRAVEL_SHELL_REQUIRED", "Traversal full-view coverage requires travel_shell.")
            )

        particles = state.get("particleBudget")
        if not isinstance(particles, int) or isinstance(particles, bool) or particles < 0:
            findings.append(Finding("PARTICLE_BUDGET_INVALID", f"{state_id} particleBudget must be >=0 integer."))
        elif particles > rails.get("maxLiveParticles", 64):
            findings.append(Finding("PARTICLE_CAP_EXCEEDED", f"{state_id} exceeds global particle cap."))

        for field in ("audioCue", "controlPolicy", "exitCondition", "requiredLog"):
            if not _nonempty(state.get(field)):
                findings.append(Finding("TEXT_FIELD_REQUIRED", f"{state_id}.{field} must be non-empty."))
        expected_log = f"ZIPTIDE: GATE_VISUAL_STATE {state_id}"
        if state.get("requiredLog") != expected_log:
            findings.append(
                Finding("LOG_CONTRACT_INVALID", f"{state_id} log must equal {expected_log!r}.")
            )

    actual_order = tuple(state.get("id") for state in states if isinstance(state, dict))
    if actual_order != STATE_ORDER:
        findings.append(
            Finding("STATE_ORDER_INVALID", f"State order must remain {STATE_ORDER}; got {actual_order}.")
        )
    if len(states) != len(STATE_ORDER):
        findings.append(Finding("STATE_COUNT_INVALID", f"Expected 7 states; found {len(states)}."))

    waking = by_id.get("waking")
    if waking:
        duration = waking.get("durationSeconds", {})
        if duration.get("min") != 1.0 or duration.get("max") != 2.5:
            findings.append(Finding("WAKING_DURATION_DRIFT", "Waking must remain 1.0–2.5 seconds."))
    collapse = by_id.get("collapse")
    if collapse and collapse.get("durationSeconds", {}).get("min", 0) < 0.75:
        findings.append(Finding("COLLAPSE_TOO_FAST", "Collapse minimum must remain at least 0.75 seconds."))
    traversal = by_id.get("traversal")
    if traversal:
        if traversal.get("controlPolicy") != "TravelCoordinator-owned-hold":
            findings.append(
                Finding("TRAVERSAL_CONTROL_OWNER", "TravelCoordinator must hold controls during traversal.")
            )
    exit_state = by_id.get("exit_reform")
    if exit_state:
        if exit_state.get("exitCondition") != "floor-horizon-input-stable":
            findings.append(
                Finding("EXIT_STABILITY_REQUIRED", "Exit must wait for floor, horizon, and input stability.")
            )

    transitions_raw = data.get("allowedTransitions")
    transitions: set[tuple[str, str]] = set()
    if not isinstance(transitions_raw, list):
        findings.append(Finding("TRANSITIONS_REQUIRED", "allowedTransitions must be a list."))
        transitions_raw = []
    for index, item in enumerate(transitions_raw):
        if not isinstance(item, list) or len(item) != 2 or not all(_nonempty(v) for v in item):
            findings.append(Finding("TRANSITION_INVALID", f"allowedTransitions[{index}] is invalid."))
            continue
        pair = (item[0].strip(), item[1].strip())
        if pair in transitions:
            findings.append(Finding("TRANSITION_DUPLICATE", f"Duplicate transition {pair}."))
        transitions.add(pair)
        if pair[0] not in by_id or pair[1] not in by_id:
            findings.append(Finding("TRANSITION_STATE_MISSING", f"Transition {pair} uses unknown state."))
    missing = sorted(REQUIRED_TRANSITIONS - transitions)
    extra = sorted(transitions - REQUIRED_TRANSITIONS)
    if missing:
        findings.append(Finding("CANON_TRANSITIONS_MISSING", f"Missing transitions: {missing}."))
    if extra:
        findings.append(
            Finding("UNREVIEWED_TRANSITIONS_PRESENT", f"Unreviewed transitions require amendment: {extra}.")
        )

    return Result(len(states), len(transitions), tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--catalog", type=Path, default=Path("docs/project_art_plan/gate_lifecycle_catalog.json")
    )
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    try:
        result = validate_catalog(catalog)
    except InputError as exc:
        print(f"GATE_LIFECYCLE_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"GATE_LIFECYCLE_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(
        f"GATE_LIFECYCLE_PASS states={result.state_count} transitions={result.transition_count}"
    )
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
