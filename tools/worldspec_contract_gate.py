#!/usr/bin/env python3
"""Reconcile every docs/worldspecs/*.spec.json against the world's attached contract assets.

The spec is truth (WorldSpec law) and WorldSpecCompiler OVERWRITES layout/pack blocks wholesale,
so a spec that drops a hero marker, machine, or drone zone silently breaks the contract that
references it — the JOB_MARKER_MISSING class that once left toxiccity_complete unreachable and
W002 permanently locked. This gate fails CI the moment a spec and its world's job steps diverge,
instead of letting Terry find it on the headset.

Per spec it checks, via the world's WorldPack (pack -> jobs -> steps, all by guid):
  - every GoToMarker step's markerId exists in the spec (hero interiorMarkerId or poi_<id>);
  - every RepairMachine step's machineId exists in the spec's machines;
  - every DisableDrones step's count is coverable by the spec's droneZones;
  - every spec collectible/machine-part item id has an ItemDefinition in Resources/Items
    (mirrors WorldSpecValidator's SPEC_ITEM_UNKNOWN so the red shows in CI, not at the desk).

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

SPEC_DIR = "docs/worldspecs"
PACK_DIR = "Ziptide/Assets/Ziptide/Content/Worlds/Packs"
JOBS_DIR = "Ziptide/Assets/Ziptide/Content/Jobs"
ITEMS_DIR = "Ziptide/Assets/Ziptide/Resources/Items"

_GUID_REF = re.compile(r"guid: ([0-9a-f]{32})")
_META_GUID = re.compile(r"^guid: ([0-9a-f]{32})", re.MULTILINE)
_SCALAR = {
    "markerId": re.compile(r"^\s{2}markerId: (\S+)\s*$", re.MULTILINE),
    "machineId": re.compile(r"^\s{2}machineId: (\S+)\s*$", re.MULTILINE),
    "count": re.compile(r"^\s{2}count: (\d+)\s*$", re.MULTILINE),
    "itemId": re.compile(r"^\s{2}itemId: (\S+)\s*$", re.MULTILINE),
    "jobId": re.compile(r"^\s{2}jobId: (\S+)\s*$", re.MULTILINE),
}


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    scene: str = ""
    path: str = ""
    severity: str = "error"


@dataclass(frozen=True)
class GateResult:
    root: str
    spec_count: int
    checked_step_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        if any(f.severity == "error" for f in self.findings):
            return "fail"
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "worldspec_contract_gate",
            "toolVersion": SCHEMA_VERSION,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "specCount": self.spec_count,
            "checkedStepCount": self.checked_step_count,
            "findingCount": len(self.findings),
            "findings": [asdict(f) for f in self.findings],
        }


def _scalar(text: str, field: str) -> str | None:
    match = _SCALAR[field].search(text)
    return match.group(1) if match else None


def _block_guids(text: str, list_name: str) -> list[str]:
    """Guid refs inside one top-level serialized list (e.g. 'jobs:' / 'steps:')."""
    lines = text.splitlines()
    guids: list[str] = []
    inside = False
    for line in lines:
        if line.strip() == list_name + ":":
            inside = True
            continue
        if inside:
            if line.startswith("  - ") or line.startswith("    "):
                guids.extend(_GUID_REF.findall(line))
            else:
                inside = False
    return guids


def _guid_index(folder: Path) -> dict[str, Path]:
    """guid -> the .asset it names, for every *.asset.meta under folder."""
    index: dict[str, Path] = {}
    if not folder.is_dir():
        return index
    for meta in folder.rglob("*.asset.meta"):
        match = _META_GUID.search(meta.read_text(encoding="utf-8", errors="replace"))
        if match:
            index[match.group(1)] = meta.with_suffix("")
    return index


def _spec_marker_ids(spec: dict[str, Any]) -> set[str]:
    ids: set[str] = set()
    for district in spec.get("districts", []):
        for hero in district.get("heroBuildings", []):
            marker = hero.get("interiorMarkerId", "")
            if marker:
                ids.add(marker)
    for poi in spec.get("pois", []):
        if poi.get("id"):
            ids.add("poi_" + poi["id"])
    return ids


def _item_ids(items_dir: Path) -> set[str]:
    ids: set[str] = set()
    if not items_dir.is_dir():
        return ids
    for asset in items_dir.glob("*.asset"):
        item = _scalar(asset.read_text(encoding="utf-8", errors="replace"), "itemId")
        if item:
            ids.add(item)
    return ids


def run_gate(root: Path) -> GateResult:
    findings: list[Finding] = []
    spec_dir = root / SPEC_DIR
    spec_paths = sorted(spec_dir.glob("*.spec.json")) if spec_dir.is_dir() else []
    known_items = _item_ids(root / ITEMS_DIR)
    job_index = _guid_index(root / JOBS_DIR)
    checked_steps = 0

    for spec_path in spec_paths:
        rel = str(spec_path.relative_to(root))
        try:
            spec = json.loads(spec_path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError) as exc:
            findings.append(Finding("WSPEC_UNREADABLE", f"{rel}: {exc}", path=rel))
            continue

        scene = spec.get("sceneName", "")
        if not scene:
            findings.append(Finding("WSPEC_SCENE_MISSING", f"{rel}: sceneName is empty", path=rel))
            continue

        markers = _spec_marker_ids(spec)
        machines = {m.get("machineId", "") for m in spec.get("machines", [])}
        drone_capacity = sum(int(z.get("count", 0)) for z in spec.get("droneZones", []))

        # Spec-declared items must resolve (the Unity-side validator would reject the compile;
        # surfacing it here means the red lands in CI instead of at Terry's desk).
        for collectible in spec.get("collectibles", []):
            item = collectible.get("itemId", "")
            if item and item not in known_items:
                findings.append(Finding(
                    "WSPEC_ITEM_UNRESOLVED",
                    f"{scene}: collectible '{item}' has no ItemDefinition in {ITEMS_DIR}",
                    scene=scene, path=rel))
        for machine in spec.get("machines", []):
            part = machine.get("partItemId", "")
            if part and part not in known_items:
                findings.append(Finding(
                    "WSPEC_ITEM_UNRESOLVED",
                    f"{scene}: machine part '{part}' has no ItemDefinition in {ITEMS_DIR}",
                    scene=scene, path=rel))

        pack_path = root / PACK_DIR / f"{scene}_WorldPack.asset"
        if not pack_path.is_file():
            findings.append(Finding(
                "WSPEC_PACK_MISSING",
                f"{scene}: no WorldPack at {PACK_DIR}/{scene}_WorldPack.asset "
                "(compile the spec / run the world bake once, then commit the pack)",
                scene=scene, path=rel, severity="warning"))
            continue

        pack_text = pack_path.read_text(encoding="utf-8", errors="replace")
        for job_guid in _block_guids(pack_text, "jobs"):
            job_path = job_index.get(job_guid)
            if job_path is None or not job_path.is_file():
                findings.append(Finding(
                    "CONTRACT_JOB_UNRESOLVED",
                    f"{scene}: pack references job guid {job_guid} with no asset under {JOBS_DIR}",
                    scene=scene, path=str(pack_path.relative_to(root))))
                continue
            job_text = job_path.read_text(encoding="utf-8", errors="replace")
            job_id = _scalar(job_text, "jobId") or job_path.stem
            for step_guid in _block_guids(job_text, "steps"):
                step_path = job_index.get(step_guid)
                if step_path is None or not step_path.is_file():
                    findings.append(Finding(
                        "CONTRACT_STEP_UNRESOLVED",
                        f"{scene}: job '{job_id}' references step guid {step_guid} "
                        f"with no asset under {JOBS_DIR}",
                        scene=scene, path=str(job_path.relative_to(root))))
                    continue
                checked_steps += 1
                step_rel = str(step_path.relative_to(root))
                step_text = step_path.read_text(encoding="utf-8", errors="replace")
                marker = _scalar(step_text, "markerId")
                machine = _scalar(step_text, "machineId")
                count = _scalar(step_text, "count")
                if marker and marker not in markers:
                    findings.append(Finding(
                        "CONTRACT_MARKER_UNKNOWN",
                        f"{scene}: job '{job_id}' step '{step_path.stem}' targets marker "
                        f"'{marker}' but the spec authors no hero/poi with that id — "
                        "compiling the spec would strand this step (JOB_MARKER_MISSING class)",
                        scene=scene, path=step_rel))
                if machine and machine not in machines:
                    findings.append(Finding(
                        "CONTRACT_MACHINE_UNKNOWN",
                        f"{scene}: job '{job_id}' step '{step_path.stem}' repairs machine "
                        f"'{machine}' but the spec's machines block does not carry it",
                        scene=scene, path=step_rel))
                if marker is None and machine is None and count is not None:
                    if int(count) > drone_capacity:
                        findings.append(Finding(
                            "CONTRACT_DRONE_CAPACITY",
                            f"{scene}: job '{job_id}' step '{step_path.stem}' needs {count} "
                            f"drone disables but the spec's droneZones spawn only "
                            f"{drone_capacity}",
                            scene=scene, path=step_rel))

    return GateResult(
        root=str(root),
        spec_count=len(spec_paths),
        checked_step_count=checked_steps,
        findings=tuple(findings),
    )


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", default=".", help="repository root")
    parser.add_argument("--json-report", default="", help="optional JSON report path")
    args = parser.parse_args(argv)

    root = Path(args.root).resolve()
    if not root.is_dir():
        print(f"worldspec_contract_gate: root not found: {root}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    result = run_gate(root)

    if args.json_report:
        report_path = Path(args.json_report)
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")

    for finding in result.findings:
        stream = sys.stderr if finding.severity == "error" else sys.stdout
        print(f"[{finding.severity.upper()}] {finding.code}: {finding.message}", file=stream)
    print(f"worldspec_contract_gate: {result.status} "
          f"({result.spec_count} spec(s), {result.checked_step_count} step(s), "
          f"{len(result.findings)} finding(s))")
    return EXIT_OK if result.status != "fail" else EXIT_VALIDATION_FAILED


if __name__ == "__main__":
    raise SystemExit(main())
