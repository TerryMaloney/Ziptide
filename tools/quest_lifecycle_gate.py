#!/usr/bin/env python3
"""Validate docs/runtime_lifecycle/quest_system_focus_contract.json against objective truth.

Enforced, mechanically:
- exactly the six contract states exist;
- every state specifies every required behavior axis with a non-empty value;
- every transition names states from the set and a non-empty signal, and its
  detectionSource path exists in the repository;
- every audited seam / preserved owner / haptic site / wall-clock path exists;
- implementation slices are non-empty, unique ids, and dependsOn ids resolve to
  slices or declared dependencies;
- once any state carries an `implementedBy` field, its path must exist (the S6
  ratchet: contract-vs-source drift becomes a failure, never silent).

Exit 0 = contract valid. Exit 2 = violations (each printed as GATE_FAIL line).
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

REQUIRED_STATES = (
    "Active",
    "SystemOverlay",
    "HeadsetRemoved",
    "TrackingLost",
    "Backgrounded",
    "Resuming",
)

DEFAULT_CONTRACT = Path("docs/runtime_lifecycle/quest_system_focus_contract.json")


def _exists(repo_root: Path, rel: str) -> bool:
    return (repo_root / rel).exists()


def validate(contract_path: Path, repo_root: Path) -> list[str]:
    problems: list[str] = []

    try:
        data = json.loads(contract_path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001 - any parse failure is the finding
        return [f"contract unreadable: {exc}"]

    axes = data.get("requiredBehaviorAxes") or []
    if not axes:
        problems.append("requiredBehaviorAxes missing or empty")

    states = data.get("states") or {}
    for name in REQUIRED_STATES:
        if name not in states:
            problems.append(f"state missing: {name}")
    for name in states:
        if name not in REQUIRED_STATES:
            problems.append(f"unknown state: {name}")

    for name, state in states.items():
        behaviors = (state or {}).get("behaviors") or {}
        for axis in axes:
            value = behaviors.get(axis)
            if not isinstance(value, str) or not value.strip():
                problems.append(f"state {name}: behavior axis missing/empty: {axis}")
        for axis in behaviors:
            if axis not in axes:
                problems.append(f"state {name}: undeclared behavior axis: {axis}")
        implemented_by = (state or {}).get("implementedBy")
        if implemented_by is not None:
            if not isinstance(implemented_by, str) or not _exists(repo_root, implemented_by):
                problems.append(f"state {name}: implementedBy path missing on disk: {implemented_by}")

    transitions = data.get("transitions") or []
    if not transitions:
        problems.append("transitions missing or empty")
    for i, tr in enumerate(transitions):
        frm, to = tr.get("from"), tr.get("to")
        if frm not in REQUIRED_STATES:
            problems.append(f"transition {i}: bad from-state: {frm}")
        if to not in REQUIRED_STATES:
            problems.append(f"transition {i}: bad to-state: {to}")
        if not (tr.get("signal") or "").strip():
            problems.append(f"transition {i}: empty signal")
        src = tr.get("detectionSource")
        if not src or not _exists(repo_root, src):
            problems.append(f"transition {i}: detectionSource missing on disk: {src}")

    seams = data.get("auditedSeams") or {}
    if not seams:
        problems.append("auditedSeams missing or empty")
    for key, value in seams.items():
        paths = value if isinstance(value, list) else [value]
        for rel in paths:
            if not isinstance(rel, str) or not _exists(repo_root, rel):
                problems.append(f"auditedSeams.{key}: path missing on disk: {rel}")

    owner = data.get("canonicalOwner") or {}
    for oname, opath in (owner.get("preservedOwners") or {}).items():
        if not _exists(repo_root, opath):
            problems.append(f"preservedOwners.{oname}: path missing on disk: {opath}")
    if owner.get("status") == "proposed-new-owner":
        planned = owner.get("plannedPath") or ""
        if _exists(repo_root, planned):
            problems.append(
                "canonicalOwner status is proposed-new-owner but plannedPath already exists "
                f"on disk — flip status to implemented and add implementedBy fields: {planned}"
            )

    dep_ids = {d.get("id") for d in (data.get("dependencies") or [])}
    slices = data.get("implementationSlices") or []
    if not slices:
        problems.append("implementationSlices missing or empty")
    slice_ids: set[str] = set()
    for sl in slices:
        sid = sl.get("id") or ""
        if sid in slice_ids:
            problems.append(f"duplicate slice id: {sid}")
        slice_ids.add(sid)
        if not (sl.get("evidence") or []):
            problems.append(f"slice {sid}: no acceptance evidence listed")
    for sl in slices:
        for dep in sl.get("dependsOn") or []:
            if dep not in slice_ids and dep not in dep_ids:
                problems.append(f"slice {sl.get('id')}: unresolved dependsOn: {dep}")

    return problems


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--contract", type=Path, default=DEFAULT_CONTRACT)
    parser.add_argument("--repo-root", type=Path, default=Path("."))
    parser.add_argument("--json-report", type=Path)
    args = parser.parse_args(argv)

    problems = validate(args.contract, args.repo_root)

    if args.json_report:
        args.json_report.parent.mkdir(parents=True, exist_ok=True)
        args.json_report.write_text(
            json.dumps(
                {"contract": str(args.contract), "valid": not problems, "problems": problems},
                indent=2,
            ),
            encoding="utf-8",
        )

    if problems:
        for p in problems:
            print(f"QUEST_LIFECYCLE_GATE_FAIL {p}")
        return 2
    print("QUEST_LIFECYCLE_GATE_OK states=6 contract=" + str(args.contract))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
