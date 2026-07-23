#!/usr/bin/env python3
"""Validate RILL visual profiles against the exact story-derived memory states."""

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
    "Dormant",
    "Stirring",
    "Remembering",
    "Unsealing",
    "Integrated",
    "EndgameA",
    "EndgameB",
    "EndgameC",
    "EndgameD",
)
CURRENT_BASELINE = {
    "Dormant": ([0.25, 0.45, 0.65], "DormantColor"),
    "Stirring": ([0.30, 0.80, 0.95], "StirringColor"),
    "Remembering": ([0.55, 0.85, 0.75], "RememberingColor"),
}
LATER_STATES = ["Unsealing", "Integrated", "EndgameA", "EndgameB", "EndgameC", "EndgameD"]
LATE_RGB = [0.85, 0.80, 0.45]


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    path: str = ""


@dataclass(frozen=True)
class Result:
    profile_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "failure"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "rill_visual_state_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "profileCount": self.profile_count,
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


def _read(root: Path, raw_path: Any, *, field: str, findings: list[Finding]) -> str:
    if not isinstance(raw_path, str) or not raw_path.strip():
        findings.append(Finding("PATH_INVALID", f"{field} must be repository-relative."))
        return ""
    normalized = raw_path.replace("\\", "/").strip()
    candidate = Path(normalized)
    if candidate.is_absolute():
        findings.append(Finding("PATH_ABSOLUTE", f"{field} must be repository-relative.", normalized))
        return ""
    resolved_root = root.resolve()
    resolved = (resolved_root / candidate).resolve()
    try:
        resolved.relative_to(resolved_root)
    except ValueError:
        findings.append(Finding("PATH_OUTSIDE_ROOT", f"{field} escapes repository root.", normalized))
        return ""
    try:
        return resolved.read_text(encoding="utf-8")
    except (OSError, UnicodeError) as exc:
        findings.append(Finding("REFERENCE_MISSING", f"Could not read {field}: {exc}", normalized))
        return ""


def _file_exists(root: Path, raw_path: Any, *, field: str, findings: list[Finding]) -> None:
    if not isinstance(raw_path, str) or not raw_path.strip():
        findings.append(Finding("PATH_INVALID", f"{field} must be repository-relative."))
        return
    normalized = raw_path.replace("\\", "/").strip()
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
        findings.append(Finding("REFERENCE_MISSING", f"{field} does not exist.", normalized))


def _number(value: Any) -> bool:
    return isinstance(value, (int, float)) and not isinstance(value, bool)


def _validate_intensity(
    value: Any, *, field: str, low: float, high: float, findings: list[Finding]
) -> None:
    if not _number(value) or value < low or value > high:
        findings.append(
            Finding("INTENSITY_RANGE", f"{field} must be within {low}..{high}; got {value!r}.")
        )


def validate_catalog(root: Path, catalog_path: Path) -> Result:
    root = root.resolve()
    data = _load(catalog_path)
    findings: list[Finding] = []

    if data.get("schemaVersion") != SCHEMA_VERSION:
        findings.append(Finding("SCHEMA_VERSION_UNSUPPORTED", "schemaVersion must equal 1."))
    if data.get("assetId") != "rill_orb":
        findings.append(Finding("ASSET_ID_INVALID", "assetId must remain rill_orb."))
    if data.get("decisionStatus") != "proposed-canon":
        findings.append(
            Finding("DECISION_STATUS_INVALID", "RILL visual expansion remains proposed-canon until reviewed.")
        )
    if data.get("stateType") != "Ziptide.Core.RillMemoryState":
        findings.append(
            Finding("STATE_TYPE_INVALID", "Visual profiles must use Ziptide.Core.RillMemoryState.")
        )

    state_source = data.get("stateSource")
    presenter_source = data.get("currentPresenter")
    state_text = _read(root, state_source, field="stateSource", findings=findings)
    presenter_text = _read(root, presenter_source, field="currentPresenter", findings=findings)
    _file_exists(root, data.get("keeperConcept"), field="keeperConcept", findings=findings)

    for state in STATE_ORDER:
        if state not in state_text:
            findings.append(
                Finding("SOURCE_STATE_MISSING", f"stateSource no longer declares {state}.", str(state_source))
            )
    for token in (
        "Color c = StateColor(RillState.Compute(profile));",
        "private static Color StateColor(RillMemoryState state)",
        "case RillMemoryState.Dormant",
        "case RillMemoryState.Stirring",
        "case RillMemoryState.Remembering",
        "default: return LateColor;",
    ):
        if token not in presenter_text:
            findings.append(
                Finding("PRESENTER_SEAM_MISSING", f"Current presenter evidence missing {token!r}.", str(presenter_source))
            )

    invariants = data.get("invariants")
    if not isinstance(invariants, dict):
        findings.append(Finding("INVARIANTS_REQUIRED", "invariants must be an object."))
        invariants = {}
    exact_flags = {
        "stateDerivedFromStoryFlags": True,
        "secondEmotionRuntimeAllowed": False,
        "repairPanelVisibleAllStates": True,
        "irisAndChannelsIndependent": True,
        "fullBodyStrobeAllowed": False,
    }
    for key, expected in exact_flags.items():
        if invariants.get(key) != expected:
            findings.append(
                Finding("INVARIANT_DRIFT", f"invariants.{key} must remain {expected!r}.")
            )
    minimum_distance = invariants.get("minimumConversationDistanceMeters")
    if not _number(minimum_distance) or minimum_distance < 0.45:
        findings.append(
            Finding("PERSONAL_SPACE_INVALID", "minimumConversationDistanceMeters must be at least 0.45.")
        )
    default_distance = invariants.get("defaultConversationDistanceMeters")
    if not isinstance(default_distance, dict):
        findings.append(Finding("DEFAULT_DISTANCE_REQUIRED", "defaultConversationDistanceMeters required."))
    else:
        low = default_distance.get("min")
        high = default_distance.get("max")
        if not _number(low) or not _number(high) or low < 0.65 or high > 0.95 or low > high:
            findings.append(
                Finding("DEFAULT_DISTANCE_INVALID", "Default conversation distance must remain inside 0.65..0.95 m.")
            )
    for range_key in ("channelIntensityRange", "irisIntensityRange"):
        value = invariants.get(range_key)
        if not isinstance(value, dict) or value.get("min") != 0.0 or value.get("max") != 1.0:
            findings.append(Finding("GLOBAL_INTENSITY_RANGE", f"{range_key} must remain 0.0..1.0."))

    baseline = data.get("currentRuntimeBaseline")
    if not isinstance(baseline, dict):
        findings.append(Finding("BASELINE_REQUIRED", "currentRuntimeBaseline must be an object."))
        baseline = {}
    for state, (rgb, constant) in CURRENT_BASELINE.items():
        entry = baseline.get(state)
        if not isinstance(entry, dict) or entry.get("rgb") != rgb or entry.get("sourceConstant") != constant:
            findings.append(
                Finding("BASELINE_DRIFT", f"Current baseline for {state} must remain {rgb}/{constant}.")
            )
    late = baseline.get("laterStatesCollapsedToDefault")
    if not isinstance(late, dict):
        findings.append(Finding("LATE_BASELINE_REQUIRED", "laterStatesCollapsedToDefault required."))
    else:
        if late.get("states") != LATER_STATES:
            findings.append(
                Finding("LATE_STATE_SET_DRIFT", f"Collapsed later states must remain {LATER_STATES}.")
            )
        if late.get("rgb") != LATE_RGB or late.get("sourceConstant") != "LateColor":
            findings.append(
                Finding("LATE_BASELINE_DRIFT", "Collapsed later-state baseline must remain LateColor.")
            )

    profiles = data.get("profiles")
    if not isinstance(profiles, list):
        findings.append(Finding("PROFILES_REQUIRED", "profiles must be a list."))
        profiles = []
    by_state: dict[str, dict[str, Any]] = {}
    profile_ids: set[str] = set()
    for index, profile in enumerate(profiles):
        prefix = f"profiles[{index}]"
        if not isinstance(profile, dict):
            findings.append(Finding("PROFILE_INVALID", f"{prefix} must be an object."))
            continue
        state = profile.get("state")
        if state not in STATE_ORDER:
            findings.append(Finding("PROFILE_STATE_INVALID", f"{prefix}.state {state!r} is not canonical."))
            continue
        if state in by_state:
            findings.append(Finding("PROFILE_STATE_DUPLICATE", f"Duplicate profile for {state}."))
        by_state[state] = profile
        profile_id = profile.get("profileId")
        if not isinstance(profile_id, str) or not profile_id.strip():
            findings.append(Finding("PROFILE_ID_INVALID", f"{state} profileId must be non-empty."))
        elif profile_id in profile_ids:
            findings.append(Finding("PROFILE_ID_DUPLICATE", f"Duplicate profileId {profile_id}."))
        else:
            profile_ids.add(profile_id)
        for field in ("chapterBand", "keeperMoodSlot", "motionCue", "audioRegister"):
            if not isinstance(profile.get(field), str) or not profile[field].strip():
                findings.append(Finding("PROFILE_TEXT_REQUIRED", f"{state}.{field} must be non-empty."))
        for component in ("iris", "channels", "structuralNodes"):
            value = profile.get(component)
            if not isinstance(value, dict):
                findings.append(Finding("VISUAL_COMPONENT_REQUIRED", f"{state}.{component} required."))
                continue
            if not isinstance(value.get("family"), str) or not value["family"].strip():
                findings.append(Finding("VISUAL_FAMILY_REQUIRED", f"{state}.{component}.family required."))
            _validate_intensity(
                value.get("intensity"),
                field=f"{state}.{component}.intensity",
                low=0.0,
                high=1.0,
                findings=findings,
            )
        if not isinstance(profile.get("glitchAllowed"), bool):
            findings.append(Finding("GLITCH_BOOL_REQUIRED", f"{state}.glitchAllowed must be boolean."))
        elif profile.get("glitchAllowed"):
            rails = profile.get("glitchRails")
            if not isinstance(rails, dict):
                findings.append(Finding("GLITCH_RAILS_REQUIRED", f"{state} needs glitchRails."))
            else:
                burst = rails.get("maxBurstSeconds")
                gap = rails.get("minimumGapSeconds")
                if not _number(burst) or burst <= 0 or burst > 0.35:
                    findings.append(
                        Finding("GLITCH_BURST_INVALID", f"{state} glitch burst must be >0 and <=0.35 sec.")
                    )
                if not _number(gap) or gap < 4.0:
                    findings.append(
                        Finding("GLITCH_GAP_INVALID", f"{state} glitch gap must be at least 4 sec.")
                    )
                if rails.get("fullFrameFlicker") is not False:
                    findings.append(
                        Finding("FULL_FRAME_FLICKER_FORBIDDEN", f"{state} cannot use full-frame flicker.")
                    )

    actual_order = tuple(profile.get("state") for profile in profiles if isinstance(profile, dict))
    if actual_order != STATE_ORDER:
        findings.append(
            Finding("PROFILE_ORDER_INVALID", f"Profiles must remain ordered {STATE_ORDER}; got {actual_order}.")
        )
    missing = [state for state in STATE_ORDER if state not in by_state]
    if missing:
        findings.append(Finding("STATE_PROFILES_MISSING", f"Missing profiles: {missing}."))
    if len(profiles) != len(STATE_ORDER):
        findings.append(Finding("PROFILE_COUNT_INVALID", f"Expected 9 profiles; found {len(profiles)}."))

    return Result(len(profiles), tuple(findings))


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--catalog", type=Path, default=Path("docs/project_art_plan/rill_visual_state_catalog.json")
    )
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    root = args.root.resolve()
    catalog = args.catalog if args.catalog.is_absolute() else root / args.catalog
    try:
        result = validate_catalog(root, catalog)
    except InputError as exc:
        print(f"RILL_VISUAL_STATE_ERROR {exc}")
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        output = args.json_report if args.json_report.is_absolute() else root / args.json_report
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    if result.findings:
        for finding in result.findings:
            print(f"RILL_VISUAL_STATE_FAIL {finding.code}: {finding.message}")
        return EXIT_VALIDATION_FAILED

    print(f"RILL_VISUAL_STATE_PASS profiles={result.profile_count}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
