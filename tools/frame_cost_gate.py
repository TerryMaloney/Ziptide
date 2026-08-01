#!/usr/bin/env python3
"""Flag per-frame cost that a Quest cannot afford.

⚖ Terry, 2026-08-01, after a Gemini review of the workflow: the advice was to hardcode Meta Quest
performance constraints into the AI prompts. Prompts do not enforce — this project has repeatedly
learned that documents drift (MISS_LEDGER #24, #25) — so the constraint gets a GATE instead, in the
same shape as `level1_wiring_gate.py` and with the WARN-first ratchet `PerfBudgetAuditRules` uses.

The gap this closes is real and was self-inflicted. `PerfBudgetAuditRules` gates STATIC scene cost
(triangles, materials, renderers, lights). Nothing gated RUNTIME cost, and neither CLAUDE.md nor
EXCELLENCE_MAP said a word about allocations per frame — which is how `ShipArmouryRuntime.Update`
shipped two `FindObjectsOfType` scans running four times a second, written by the same session that
had just spent hours on verification discipline.

WHAT IT FLAGS, and why only these:
  • FindObjectOfType / FindObjectsOfType — scans every object in the scene AND allocates. There is no
    case where this belongs in a per-frame method; the fix is always to cache or to be handed the
    reference.
  • Instantiate — the object-pooling rule. Churning GameObjects per frame is the classic Quest
    frame-time killer.
  • LINQ (.Where/.Select/.OrderBy/.ToList/.ToArray/.Any/.First) — allocates enumerators and arrays
    every call.
  • Vector3.Distance — a square root where `sqrMagnitude` almost always does.

Deliberately NOT flagged, because a gate that cries wolf on a correct pattern gets ignored — which is
worse than no gate:
  • GetComponent — usually the cached `if (_x == null) _x = GetComponent<>()` idiom.
  • A NULL-GUARDED find (`if (_x == null) _x = FindObjectOfType<>()`) — that runs once, not per frame.
    The first version of this gate flagged those and was ~40% false positives on its own baseline.
  • A bare `Destroy(gameObject)` — a projectile ending its own life is correct; it is the
    `Instantiate` side that pooling fixes.

Per-frame means Update / FixedUpdate / LateUpdate, found by brace-matching the method body — PLUS one
level of private helpers called from them. That indirection is not optional: the violation that
prompted this gate (`ShipArmouryRuntime`'s two `FindObjectsOfType` scans) lives in `CountOnBelt`, a
helper, and a gate that only read `Update` bodies would have missed the very thing it was built for.

Exit codes: 0 ok/warn · 1 operational error · 2 validation failed (--strict only).
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

# Directories whose per-frame cost reaches the headset. Editor code runs in the editor only.
RUNTIME_DIRS = ("Core", "Content", "Gameplay", "Ship", "Visuals", "Multiplayer")

PER_FRAME_METHODS = ("Update", "FixedUpdate", "LateUpdate")

# (code, regex, why it costs)
PATTERNS: tuple[tuple[str, str, str], ...] = (
    ("FRAME_FIND_OBJECT", r"\bFindObjectsOfType\s*<|\bFindObjectOfType\s*<",
     "scans every object in the scene and allocates; cache the reference or have it passed in"),
    ("FRAME_INSTANTIATE", r"\bInstantiate\s*\(",
     "per-frame GameObject churn; use an object pool"),
    ("FRAME_LINQ", r"\.(Where|Select|OrderBy|ToList|ToArray|Any|First|FirstOrDefault)\s*\(",
     "LINQ allocates an enumerator every call"),
    ("FRAME_SQRT_DISTANCE", r"Vector3\.Distance\s*\(",
     "square root per call; compare sqrMagnitude against a squared threshold"),
)

# Known, reviewed exceptions. Each needs a reason — an unexplained waiver is how a gate rots.
WAIVERS: dict[str, str] = {}


@dataclass(frozen=True)
class Finding:
    code: str
    file: str
    line: int
    method: str
    snippet: str
    why: str
    severity: str = "warning"


def _runtime_sources(root: Path) -> list[Path]:
    out: list[Path] = []
    for sub in RUNTIME_DIRS:
        d = root / ASSETS / sub
        if not d.is_dir():
            continue
        out.extend(p for p in d.rglob("*.cs") if "/Tests/" not in p.as_posix())
    return sorted(out)


def _method_bodies(text: str) -> list[tuple[str, int, int]]:
    """Return (name, start_index, end_index) for each per-frame method body, brace-matched."""
    bodies: list[tuple[str, int, int]] = []
    for name in PER_FRAME_METHODS:
        # A declaration, not a call: optional modifiers, the name, (), then a brace.
        for m in re.finditer(rf"\b(?:private|public|protected|internal|virtual|override|\s)*void\s+{name}\s*\([^)]*\)\s*\{{", text):
            start = text.index("{", m.start())
            depth, i = 0, start
            while i < len(text):
                if text[i] == "{":
                    depth += 1
                elif text[i] == "}":
                    depth -= 1
                    if depth == 0:
                        break
                i += 1
            bodies.append((name, start, i))
    return bodies


def _named_method_body(text: str, name: str) -> tuple[int, int] | None:
    """Brace-matched body of a single named method, or None."""
    m = re.search(rf"\b(?:private|protected|internal|static|\s)*[\w<>\[\],\s]+\s+{re.escape(name)}\s*\([^)]*\)\s*\{{", text)
    if not m:
        return None
    start = text.index("{", m.start())
    depth, i = 0, start
    while i < len(text):
        if text[i] == "{":
            depth += 1
        elif text[i] == "}":
            depth -= 1
            if depth == 0:
                return (start, i)
        i += 1
    return None


def _per_frame_regions(text: str) -> list[tuple[str, int, int]]:
    """Per-frame method bodies PLUS the bodies of same-file helpers they call.

    One level only. That is enough to catch the case this gate was built for — an expensive scan
    hidden one hop from Update — without chasing the whole call graph and reporting the engine.
    """
    regions = _method_bodies(text)
    seen = {name for name, _, _ in regions}
    for name, start, end in list(regions):
        body = text[start:end]
        for call in set(re.findall(r"\b([A-Z][A-Za-z0-9_]*)\s*\(", body)):
            if call in seen or call in PER_FRAME_METHODS:
                continue
            found = _named_method_body(text, call)
            if found is None:
                continue
            seen.add(call)
            regions.append((f"{name}->{call}", found[0], found[1]))
    return regions


def scan(root: Path) -> list[Finding]:
    findings: list[Finding] = []
    for path in _runtime_sources(root):
        try:
            text = path.read_text(encoding="utf-8", errors="replace")
        except OSError:
            continue
        if not any(m in text for m in PER_FRAME_METHODS):
            continue

        rel = path.relative_to(root).as_posix()
        for name, start, end in _per_frame_regions(text):
            body = text[start:end]
            for code, pattern, why in PATTERNS:
                for hit in re.finditer(pattern, body):
                    abs_index = start + hit.start()
                    line = text.count("\n", 0, abs_index) + 1
                    key = f"{rel}:{line}"
                    if key in WAIVERS:
                        continue
                    snippet = text[text.rfind("\n", 0, abs_index) + 1: text.find("\n", abs_index)].strip()
                    if _is_cached_lookup(code, snippet):
                        continue
                    findings.append(Finding(code, rel, line, name, snippet[:140], why))
    return findings


def _is_cached_lookup(code: str, snippet: str) -> bool:
    """A null-guarded assignment resolves once and then never again."""
    if code != "FRAME_FIND_OBJECT":
        return False
    return "== null" in snippet or "?? " in snippet or "??=" in snippet


def main(argv: list[str]) -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--root", default=".", help="repository root")
    ap.add_argument("--json", default="", help="write the report here")
    ap.add_argument("--strict", action="store_true",
                    help="exit 2 on any finding (promote after a clean baseline)")
    args = ap.parse_args(argv)

    root = Path(args.root).resolve()
    if not (root / ASSETS).is_dir():
        print(f"frame_cost_gate: no Unity assets under {root / ASSETS}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    findings = scan(root)
    report: dict[str, Any] = {
        "tool": "frame_cost_gate",
        "toolVersion": SCHEMA_VERSION,
        "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
        "strict": args.strict,
        "findingCount": len(findings),
        "findings": [asdict(f) for f in findings],
    }
    if args.json:
        Path(args.json).write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")

    if not findings:
        print("frame_cost_gate: pass (no per-frame allocation hotspots)")
        return EXIT_OK

    by_code: dict[str, int] = {}
    for f in findings:
        by_code[f.code] = by_code.get(f.code, 0) + 1

    print(f"frame_cost_gate: {len(findings)} finding(s) — " +
          ", ".join(f"{c}={n}" for c, n in sorted(by_code.items())))
    for f in findings[:40]:
        print(f"  {f.file}:{f.line} [{f.code}] in {f.method}() — {f.why}")
        print(f"      {f.snippet}")
    if len(findings) > 40:
        print(f"  … and {len(findings) - 40} more")

    return EXIT_VALIDATION_FAILED if args.strict else EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
