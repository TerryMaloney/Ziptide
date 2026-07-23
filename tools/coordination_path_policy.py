#!/usr/bin/env python3
"""Keep coordination-only path handling consistent across ZIPTIDE workflows.

Handoff queue entries are transient coordination artifacts. Creating or consuming one must not:
- enqueue expensive Unity CI,
- enqueue Fast Preflight,
- cancel a newer source CI run, or
- make a durable CI verdict stale.

Use --apply only through the controlled coordination-path workflow. Default mode is check-only.
"""

from __future__ import annotations

import argparse
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Sequence

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2

CI_PATH = Path(".github/workflows/ci.yml")
FAST_PATH = Path(".github/workflows/fast-preflight.yml")
CANCEL_PATH = Path(".github/workflows/cancel-superseded-ci.yml")

QUEUE_IGNORE_LINE = "      - docs/handoff_queue/**"
FRESHNESS_PATTERN = (
    "docs/recovery/generated/*|docs/CI_VERDICT.md|docs/HANDOFF.md|docs/handoff_queue/*)"
)
GATE_STEP = (
    "      - name: Coordination-only path policy gate\n"
    "        run: python3 tools/coordination_path_policy.py --check\n"
)


class PolicyError(RuntimeError):
    pass


@dataclass(frozen=True)
class Finding:
    path: str
    message: str


def _read(root: Path, relative: Path) -> str:
    path = root / relative
    try:
        return path.read_text(encoding="utf-8")
    except OSError as exc:
        raise PolicyError(f"Could not read {relative}: {exc}") from exc


def _write(root: Path, relative: Path, text: str) -> None:
    path = root / relative
    path.write_text(text, encoding="utf-8")


def _replace_once(text: str, old: str, new: str, label: str) -> str:
    count = text.count(old)
    if count != 1:
        raise PolicyError(f"Expected one {label} anchor; found {count}.")
    return text.replace(old, new, 1)


def apply_policy(root: Path) -> tuple[str, ...]:
    root = root.resolve()
    changed: list[str] = []

    ci = _read(root, CI_PATH)
    original = ci
    if QUEUE_IGNORE_LINE not in ci:
        ci = _replace_once(
            ci,
            "      - docs/HANDOFF.md\n  pull_request:",
            "      - docs/HANDOFF.md\n" + QUEUE_IGNORE_LINE + "\n  pull_request:",
            "ci paths-ignore",
        )
    if FRESHNESS_PATTERN not in ci:
        ci = _replace_once(
            ci,
            "docs/recovery/generated/*|docs/CI_VERDICT.md|docs/HANDOFF.md)",
            FRESHNESS_PATTERN,
            "ci verdict freshness case",
        )
    old_notice = (
        "Only generated evidence, the verdict, or docs/HANDOFF.md advanced; "
        "recording the tested SHA is safe."
    )
    new_notice = (
        "Only generated evidence, the verdict, HANDOFF, or handoff queue coordination advanced; "
        "recording the tested SHA is safe."
    )
    if old_notice in ci:
        ci = ci.replace(old_notice, new_notice, 1)
    if ci != original:
        _write(root, CI_PATH, ci)
        changed.append(CI_PATH.as_posix())

    cancel = _read(root, CANCEL_PATH)
    original = cancel
    if QUEUE_IGNORE_LINE not in cancel:
        cancel = _replace_once(
            cancel,
            "      - docs/HANDOFF.md\n  workflow_dispatch:",
            "      - docs/HANDOFF.md\n" + QUEUE_IGNORE_LINE + "\n  workflow_dispatch:",
            "cancel workflow paths-ignore",
        )
    if cancel != original:
        _write(root, CANCEL_PATH, cancel)
        changed.append(CANCEL_PATH.as_posix())

    fast = _read(root, FAST_PATH)
    original = fast
    if QUEUE_IGNORE_LINE not in fast:
        fast = _replace_once(
            fast,
            "      - terry-local-wip\n  pull_request:",
            "      - terry-local-wip\n"
            "    paths-ignore:\n"
            "      - docs/CI_VERDICT.md\n"
            "      - docs/recovery/generated/**\n"
            "      - docs/HANDOFF.md\n"
            "      - docs/handoff_queue/**\n"
            "  pull_request:",
            "fast preflight push block",
        )
    if GATE_STEP not in fast:
        fast = _replace_once(
            fast,
            "      - name: Factory governance gate\n"
            "        run: python3 tools/factory_governance_gate.py\n",
            "      - name: Factory governance gate\n"
            "        run: python3 tools/factory_governance_gate.py\n\n"
            + GATE_STEP.rstrip("\n")
            + "\n",
            "fast preflight governance step",
        )
    if fast != original:
        _write(root, FAST_PATH, fast)
        changed.append(FAST_PATH.as_posix())

    return tuple(changed)


def check_policy(root: Path) -> tuple[Finding, ...]:
    root = root.resolve()
    findings: list[Finding] = []

    ci = _read(root, CI_PATH)
    if QUEUE_IGNORE_LINE not in ci:
        findings.append(Finding(CI_PATH.as_posix(), "handoff queue is not paths-ignored"))
    if FRESHNESS_PATTERN not in ci:
        findings.append(
            Finding(CI_PATH.as_posix(), "verdict freshness does not allow consumed handoff queues")
        )

    cancel = _read(root, CANCEL_PATH)
    if QUEUE_IGNORE_LINE not in cancel:
        findings.append(
            Finding(CANCEL_PATH.as_posix(), "handoff queue can trigger superseded-run cancellation")
        )

    fast = _read(root, FAST_PATH)
    if QUEUE_IGNORE_LINE not in fast:
        findings.append(Finding(FAST_PATH.as_posix(), "handoff queue is not paths-ignored"))
    if GATE_STEP not in fast:
        findings.append(Finding(FAST_PATH.as_posix(), "coordination path policy gate is not wired"))

    return tuple(findings)


def _parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--apply", action="store_true")
    parser.add_argument("--check", action="store_true")
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = _parser().parse_args(argv)
    root = args.root.resolve()
    try:
        if args.apply:
            changed = apply_policy(root)
            print(f"COORDINATION_PATH_POLICY applied files={len(changed)}")
            for path in changed:
                print(f"- {path}")
        findings = check_policy(root)
    except (PolicyError, OSError, UnicodeError) as exc:
        print(f"COORDINATION_PATH_POLICY_ERROR: {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    for finding in findings:
        print(f"COORDINATION_PATH_POLICY_FAIL: {finding.path}: {finding.message}", file=sys.stderr)
    if findings:
        return EXIT_VALIDATION_FAILED
    print("COORDINATION_PATH_POLICY: pass")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
