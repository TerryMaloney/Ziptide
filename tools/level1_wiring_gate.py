#!/usr/bin/env python3
"""Prove every Level 1 feature is CREATED and DRIVEN, not merely written.

⚖ Terry, 2026-07-29: *"double check that everything that's claimed to have been made is actually
made and also make sure everything is hooked together properly. I'm skeptical I'm going to get to
the headset and find a mostly working first level."*

That skepticism is well aimed. The failure mode this gate exists for is the one a diff cannot show:
a runtime class nobody ever adds to a GameObject, a pure core nobody calls, or a companion line
nobody fires all look exactly like finished work. Each feature below therefore has to satisfy three
independent claims:

  1. EXISTS  — the source file is there;
  2. CREATED — something instantiates it (AddComponent/EnsureComponent/a static Play call);
  3. DRIVEN  — something calls the logic that makes it do its job.

A feature that fails 2 or 3 is dead code wearing a feature's name, and it is a CI red here rather
than a wasted headset session there.

Exit codes: 0 ok · 1 operational error · 2 validation failed.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2

ASSETS = "Ziptide/Assets/Ziptide"
SPEC = "docs/worldspecs/ToxicCity.spec.json"
SEARCH_DIRS = ("Gameplay", "Ship", "Editor", "Core", "Content", "Visuals")

# (feature, source file, regex proving something CREATES it, regex proving something DRIVES it)
# ⚠ ADDING A FEATURE HERE? Run `python3 -m unittest discover -s tools/tests -p 'test_*_gate.py'`
# before you push. The gate's own tests stage a synthetic project that must satisfy EVERY row, so a
# new row fails them until its call sites are added to the fixture in test_level1_wiring_gate.py.
# Running the gate is not the same as running the gate's tests — that mistake cost a CI red.
FEATURES: tuple[tuple[str, str, str, str], ...] = (
    ("ring sequencer", "Ship/Runtime/RingCourseLightsRuntime.cs",
     r"AddComponent<RingCourseLightsRuntime>|EnsureComponent<RingCourseLightsRuntime>",
     r"RingLampChaseCore\."),
    ("drone reactions", "Ship/Runtime/SpaceTargetReactionCore.cs",
     r"AddComponent<SpaceTargetRuntime>", r"ObservePilot\("),
    ("debris tumble", "Ship/Runtime/DriftTumbleRuntime.cs",
     r"AddComponent<DriftTumbleRuntime>", r"RateFor\("),
    ("salvage confirm", "Ship/Runtime/SalvageTractorFx.cs",
     r"SalvageTractorFx\.Play", r"SalvageApproach01\("),
    ("atmosphere veil", "Gameplay/Runtime/World/AtmosphereVeilEffect.cs",
     r"AtmosphereVeilEffect\.Play", r"AtmosphereVeilCore\."),
    ("reentry owner", "Gameplay/Runtime/World/ReentryArrivalRuntime.cs",
     r"EnsureComponent<ReentryArrivalRuntime>|AddComponent<ReentryArrivalRuntime>",
     r"ReentryArrivalCore\.ShouldPlay"),
    ("w000 porthole", "Gameplay/Runtime/World/PortholeRuntime.cs",
     r"EnsureComponent<PortholeRuntime>|AddComponent<PortholeRuntime>", r"PortholeStarfieldCore\.Bake"),
    ("helm compass", "Ship/Runtime/HelmCompassRuntime.cs",
     r"AddComponent<HelmCompassRuntime>", r"TryGetCourseBearing\("),
    ("expedition site", "Editor/Patching/FlatsSiteAuthor.cs",
     r"FlatsSiteAuthor\.Build", r"BreachAzimuthDegrees\("),
    ("resonance tell", "Gameplay/Runtime/Story/ResonanceTellRuntime.cs",
     r"ResonanceTellRuntime\.PlayAt", r"ResonanceTellCore\.Power"),
    ("tell reaches vehicle", "Ship/Runtime/VehicleRuntime.cs",
     r"IResonanceSensitive", r"SetInstrumentPower\("),
    ("skiff water lock", "Ship/Runtime/SkiffWaterLockRuntime.cs",
     r"AddComponent<SkiffWaterLockRuntime>", r"CanalWaterCore\.IsNavigable"),
    ("canal stalker", "Gameplay/Runtime/Enemies/CanalStalkerBehavior.cs",
     r"AddComponent<CanalStalkerBehavior>", r"CanalStalkerCore\.Stage"),
    ("beacon thread", "Gameplay/Runtime/Story/BeaconThreadRuntime.cs",
     r"AddComponent<BeaconThreadRuntime>", r"ZiptideFlags\.ARTIFACT_JOINED"),
    ("key-gated launch", "Gameplay/Runtime/Story/ShipCastOffRuntime.cs",
     r"ConfigureKeyGate\(", r"keyRequired"),
    ("the throw", "Editor/Patching/CityBuilder.cs", r"BuildTheThrow\(", r"MuzzleGantry"),
    ("bounds ladder", "Content/Runtime/Flight/FlightBoundsCore.cs",
     r"FlightBoundsCore\.Evaluate\(", r"FlightBoundsVoiceCore\.ShouldSpeak\("),
    ("tender tool arms", "Ship/Runtime/SpaceTargetRuntime.cs",
     r'"AccessPanel"', r"PoseForMood\(mood"),
    ("city compass", "Editor/Patching/CityWayfindingAuthor.cs",
     r"CityWayfindingAuthor\.Build\(", r"WayfindingCore\.LanternPositions\("),
    ("relay fault strobe", "Gameplay/Runtime/World/FaultStrobeRuntime.cs",
     r"AddComponent<FaultStrobeRuntime>", r"SetPropertyBlock\("),
    ("quay berths", "Editor/Patching/QuayBerthAuthor.cs",
     r"QuayBerthAuthor\.Build\(", r"QuayBerthCore\.PadCentres\("),
    # The contract's repair step and the pack machine it repairs are written in two different
    # files by hand (generated worlds pair them automatically; ToxicCity does not). They were
    # unpaired, so step 4 of 6 could never complete. This binds the halves: lose either and CI reds.
    ("relay repair machine", "Editor/Patching/ToxicCityContractBuilder.cs",
     r"machineId = ToxicCityContractBuilder\.RelayMachineId",
     r'RepairMachine\("ToxicCity_S4_RelayRepair"'),
    # Dispatch is where the first level's contract is accepted, and it was a floor, a ceiling, four
    # walls and one accent cube. The furnish core existed and nothing in this city called it — which
    # is exactly the shape of failure this gate exists for: finished-looking work nobody invokes.
    ("hero interiors", "Content/Runtime/City/InteriorTierCore.cs",
     r"InteriorTierCore\.Evaluate\(", r"InteriorFurnisher\.Furnish\("),
    # SKYSCAPE pillar 3 on the first planet. The atmosphere stack was built and tested against W005,
    # a world the first level never reaches, while ToxicCity shipped with perfectly still air. The
    # failure this guards is the quiet one: a binder authored into the scene that never applies,
    # which looks exactly like working weather in a diff.
    ("world atmosphere", "Gameplay/Runtime/World/WorldAtmosphereBinder.cs",
     r"AddComponent<Ziptide\.Gameplay\.WorldAtmosphereBinder>|AddComponent<WorldAtmosphereBinder>",
     r"_rig\.Apply\(_vista, player\)"),
    # THE FIRST LEVEL'S OWN SKY. ToxicCity pointed at the shared DefaultWorldProfile, so the olive
    # horizon and 22-degree occluded body its layout has always authored were never once rendered —
    # and because `VisualThemeProfile.skyVista` is the seam the vista system rides, there was also
    # nowhere to hang W001 Toxic Venice. BOTH `SkyVistaLibrary` ("reserved: assigned once ToxicCity
    # gains a theme") and `SkyVistaAuthor` ("the vista asset waits") described this in comments, and
    # it waited anyway. A TODO in a comment is not a task, because nothing ever asks it whether it is
    # done. This is that question, asked on every push.
    ("first level sky", "Editor/Patching/ThemeAuthor.cs",
     r"ThemeAuthor\.EnsureThemeAsset\(kit\)",
     r"ThemeAuthor\.EnsureWorldProfileAsset\(kit, theme\)"),
)

# Ids the city bake and the contract depend on. A spec that loses one of these strands a step.
REQUIRED_SPEC_IDS = (
    "dockmaster_booth", "relay_node", "shipyard_office", "dispatch_inside",
    "tox_canal_stalker_01", "artifact_half_b", "waker_log_flats", "Quay", "Colonnade",
)

# Companion cues added for the Catch. Unlike the TUT_* teaching lines (which the first-hour beat
# contract fires by data), these are fired directly, so a caller must exist in code.
DIRECT_CUES = ("CATCH_DEAD_RING", "CATCH_OVERRUN", "CATCH_THE_FIND", "ARTIFACT_JOIN_HINT")


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    feature: str = ""
    severity: str = "error"


@dataclass(frozen=True)
class GateResult:
    root: str
    checked: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        if any(f.severity == "error" for f in self.findings):
            return "fail"
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "level1_wiring_gate",
            "toolVersion": SCHEMA_VERSION,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "featuresChecked": self.checked,
            "findingCount": len(self.findings),
            "findings": [asdict(f) for f in self.findings],
        }


def _sources(root: Path) -> list[tuple[str, str]]:
    out: list[tuple[str, str]] = []
    for sub in SEARCH_DIRS:
        d = root / ASSETS / sub
        if not d.is_dir():
            continue
        for f in d.rglob("*.cs"):
            out.append((str(f), f.read_text(encoding="utf-8", errors="replace")))
    return out


def run_gate(root: Path) -> GateResult:
    findings: list[Finding] = []
    corpus = _sources(root)

    def matches(pattern: str, exclude: str = "") -> int:
        rx = re.compile(pattern)
        return sum(1 for path, text in corpus
                   if (not exclude or exclude not in path) and rx.search(text))

    for feature, src, creator, driver in FEATURES:
        source = root / ASSETS / src
        if not source.is_file():
            findings.append(Finding("WIRING_SOURCE_MISSING",
                                    f"{feature}: source file {src} does not exist", feature))
            continue
        if matches(creator) == 0:
            findings.append(Finding(
                "WIRING_NEVER_CREATED",
                f"{feature}: nothing in the project creates it (no match for /{creator}/) — "
                "the class exists but no bake or runtime path ever instantiates it",
                feature))
        if matches(driver) == 0:
            findings.append(Finding(
                "WIRING_NEVER_DRIVEN",
                f"{feature}: nothing calls the logic that makes it work (no match for /{driver}/)",
                feature))

    spec_path = root / SPEC
    if not spec_path.is_file():
        findings.append(Finding("WIRING_SPEC_MISSING", f"{SPEC} does not exist"))
    else:
        spec_text = spec_path.read_text(encoding="utf-8")
        for needed in REQUIRED_SPEC_IDS:
            if needed not in spec_text:
                findings.append(Finding(
                    "WIRING_SPEC_ID_MISSING",
                    f"the world spec no longer carries '{needed}' — the bake or a contract step "
                    "depends on it", needed))

    for cue in DIRECT_CUES:
        if matches(rf'SayById\("{cue}"\)') == 0:
            findings.append(Finding(
                "WIRING_CUE_NEVER_FIRED",
                f"companion cue {cue} is authored but nothing calls SayById for it", cue))

    return GateResult(str(root), len(FEATURES), tuple(findings))


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", default=".")
    parser.add_argument("--json-report", default="")
    args = parser.parse_args(argv)

    root = Path(args.root).resolve()
    if not root.is_dir():
        print(f"level1_wiring_gate: root not found: {root}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    result = run_gate(root)
    if args.json_report:
        p = Path(args.json_report)
        p.parent.mkdir(parents=True, exist_ok=True)
        p.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    for f in result.findings:
        print(f"[{f.severity.upper()}] {f.code}: {f.message}", file=sys.stderr)
    print(f"level1_wiring_gate: {result.status} "
          f"({result.checked} features, {len(result.findings)} finding(s))")
    return EXIT_OK if result.status != "fail" else EXIT_VALIDATION_FAILED


if __name__ == "__main__":
    raise SystemExit(main())
