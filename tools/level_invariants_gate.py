#!/usr/bin/env python3
"""Level invariants gate — the rules that must hold in EVERY world.

Reads committed layout and world-pack data only (about a second, no Unity), and checks the
invariants from docs/design/LEVEL_INVARIANTS.md that are decidable from data:

  PL1  a ground creature zone overlaps no building, facade row, prop cluster, canal or hazard
  PL2  a flying drone zone overlaps no hazard and no enclosed building (props are allowed)
  S1   depth order: playable ground -> outskirts -> distant skyline -> horizon anchors
  A1   every world pack has an AudioProfile
  D1   every world pack declares a `player` spawn marker
  D2   every content world grants at least one flag
  D3   every exit pack points at a real, shipping scene
  D5   no world lists itself as a travel destination

Why data-only: anything that reads committed files runs here in a second. Anything that needs a
live scene belongs in the Unity world audit. That separation is not style -- putting a docs-only
check inside a Unity test cost this project a 27-hour red streak.

Usage:
    python3 tools/level_invariants_gate.py            # report, exit 0
    python3 tools/level_invariants_gate.py --strict   # exit 2 when findings exist
"""

from __future__ import annotations

import argparse
import json
import pathlib
import re
import sys

REPO = pathlib.Path(__file__).resolve().parent.parent
CONTENT = REPO / "Ziptide" / "Assets" / "Ziptide" / "Content"
LAYOUTS = [CONTENT / "City", CONTENT / "City" / "Generated"]
PACKS = CONTENT / "Worlds" / "Packs"
SCENES = REPO / "Ziptide" / "Assets" / "Ziptide" / "Scenes"


def load_asset(path: pathlib.Path) -> dict:
    """Parse a Unity ScriptableObject .asset into a plain dict.

    Object references are replaced with a marker before parsing: `{fileID: 0}` means "unset" and
    that distinction is the whole point of the AudioProfile check.
    """
    try:
        import yaml
    except ImportError:  # pragma: no cover - PyYAML ships with the runner image
        print("PyYAML is required for the level invariants gate.", file=sys.stderr)
        raise

    raw = path.read_text(encoding="utf-8", errors="replace")
    if "MonoBehaviour:" not in raw:
        return {}
    body = raw.split("MonoBehaviour:", 1)[1]
    body = re.sub(r"\{fileID: 0\}", "'__UNSET__'", body)
    body = re.sub(r"\{fileID: [^}]*\}", "'__REF__'", body)
    try:
        parsed = yaml.safe_load("root:\n" + "\n".join("  " + line for line in body.splitlines()))
    except Exception:
        return {}
    return (parsed or {}).get("root") or {}


def rect(cx: float, cz: float, sx: float, sz: float):
    return (cx - sx / 2.0, cx + sx / 2.0, cz - sz / 2.0, cz + sz / 2.0)


def overlaps(a, b) -> bool:
    return not (a[1] <= b[0] or b[1] <= a[0] or a[3] <= b[2] or b[3] <= a[2])


def v(node, key, default=0.0) -> float:
    if not isinstance(node, dict):
        return default
    try:
        return float(node.get(key, default))
    except (TypeError, ValueError):
        return default


def collect_footprints(layout: dict):
    """Enclosed volumes (buildings, facade rows), low props, and hazards/canals."""
    enclosed, props, hazards = [], [], []

    for district in layout.get("districts") or []:
        if not isinstance(district, dict):
            continue
        anchor, bounds = district.get("anchor") or {}, district.get("bounds") or {}
        ax, az = v(anchor, "x"), v(anchor, "z")
        bx, bz = v(bounds, "x"), v(bounds, "y")
        if bx <= 0 or bz <= 0:
            continue
        did = district.get("id", "district")

        for hero in district.get("heroBuildings") or []:
            if not isinstance(hero, dict):
                continue
            local, foot = hero.get("localPos") or {}, hero.get("footprint") or {}
            enclosed.append((f"{did}/{hero.get('id', 'hero')}",
                             rect(ax + v(local, "x"), az + v(local, "z"),
                                  v(foot, "x"), v(foot, "y"))))

        # CityBuilder lines each district edge with facades inset 2 m, 3 m deep.
        for offset, along_x in ((bz / 2 - 2, True), (-(bz / 2 - 2), True),
                                (bx / 2 - 2, False), (-(bx / 2 - 2), False)):
            enclosed.append((f"{did}/facades",
                             rect(ax, az + offset, bx, 3.0) if along_x
                             else rect(ax + offset, az, 3.0, bz)))

        for prop in district.get("props") or []:
            if not isinstance(prop, dict):
                continue
            centre, size = prop.get("center") or {}, prop.get("size") or {}
            props.append((f"{did}/props",
                          rect(ax + v(centre, "x"), az + v(centre, "z"),
                               v(size, "x"), v(size, "y"))))

    for canal in layout.get("canals") or []:
        if not isinstance(canal, dict):
            continue
        centre, size = canal.get("center") or {}, canal.get("size") or {}
        hazards.append(("canal", rect(v(centre, "x"), v(centre, "z"),
                                      v(size, "x"), v(size, "y"))))

    for hazard in layout.get("hazards") or []:
        if not isinstance(hazard, dict):
            continue
        centre, size = hazard.get("center") or {}, hazard.get("size") or {}
        hazards.append((hazard.get("id", "hazard"),
                        rect(v(centre, "x"), v(centre, "z"), v(size, "x"), v(size, "z"))))

    return enclosed, props, hazards


def check_layout(path: pathlib.Path, findings: list) -> None:
    layout = load_asset(path)
    if not layout or not layout.get("districts"):
        return
    world = path.stem
    enclosed, props, hazards = collect_footprints(layout)

    def zone_rect(zone):
        centre = zone.get("center") or {}
        r = v(zone, "radius", 1.0)
        return rect(v(centre, "x"), v(centre, "z"), r * 2, r * 2)

    # PL1 — ground creatures must stand on clear, non-toxic ground.
    for zone in layout.get("creatureZones") or []:
        if not isinstance(zone, dict):
            continue
        zr = zone_rect(zone)
        hit = [n for n, r in enclosed + props + hazards if overlaps(zr, r)]
        if hit:
            findings.append(
                f"[PL1] {world}: creature zone '{zone.get('id')}' "
                f"({zone.get('creatureId')}) overlaps {sorted(set(hit))} — "
                f"creatures would spawn inside geometry or in a hazard.")

    # PL2 — flying drones may pass over low props, but never through a building or a hazard.
    for zone in layout.get("droneZones") or []:
        if not isinstance(zone, dict):
            continue
        zr = zone_rect(zone)
        hit = [n for n, r in enclosed + hazards if overlaps(zr, r)]
        if hit:
            findings.append(
                f"[PL2] {world}: drone zone '{zone.get('id')}' overlaps {sorted(set(hit))} — "
                f"the player would have to fight it inside geometry or standing in a hazard.")

    # S1 — the world must read outward. A fake distant skyline inside the scatter band puts
    # buildings in FRONT of the things they are meant to sit behind.
    rings = layout.get("rings") or {}
    if rings.get("enabled") in (1, True):
        skyline = v(layout, "skylineRingRadius")
        outskirts = v(rings, "outskirtsRadius")
        pillars = v(rings, "gatePillarDistance")
        seawall = v(rings, "seaWallRadius")
        order = [("sea wall", seawall), ("outskirts", outskirts),
                 ("skyline", skyline), ("horizon anchors", pillars)]
        present = [(name, value) for name, value in order if value > 0]
        for (an, av), (bn, bv) in zip(present, present[1:]):
            if av >= bv:
                findings.append(
                    f"[S1] {world}: {an} ({av:.0f} m) is not inside {bn} ({bv:.0f} m) — "
                    f"the world would render out of depth order.")


# Dev, sandbox and arena surfaces are not story worlds: they are deliberately reachable rooms with
# no contract and no progression, so the "grants nothing" and "no spawn marker" rules do not apply.
# They are NOT exempt from A1 -- a silent arena is still a silent arena.
NON_STORY_WORLDS = {
    "Sandbox_WorldPack", "TestRoom_WorldPack", "D0_WorldPack",
    "StarterWorld_WorldPack", "PvP_Arena01_WorldPack",
}


def check_packs(findings: list) -> None:
    if not PACKS.is_dir():
        return

    shipping = {p.stem for p in SCENES.rglob("*.unity")}
    packs = {}
    for path in sorted(PACKS.glob("*.asset")):
        data = load_asset(path)
        if data:
            packs[path.stem] = data

    for name, pack in packs.items():
        scene = pack.get("sceneName") or ""
        is_exit = "Exit" in name

        # D3 — an exit that points nowhere real is a door into a test scene.
        if not scene:
            findings.append(f"[D3] {name}: has no sceneName.")
        elif scene not in shipping:
            findings.append(f"[D3] {name}: sceneName '{scene}' is not a scene in the project.")

        if is_exit:
            continue

        # D1 — you cannot arrive in a world with no arrival point.
        markers = pack.get("spawnMarkers") or []
        if name not in NON_STORY_WORLDS and not any(
                isinstance(m, dict) and m.get("markerId") == "player" for m in markers):
            findings.append(f"[D1] {name}: no 'player' spawn marker.")

        # A1 — a world with no audio profile is a silent world. Quiet is allowed; FORGETTING is not,
        # and without an explicit declaration those two are indistinguishable from the data.
        if pack.get("audioProfile") == "__UNSET__" and not pack.get("deliberatelySilent"):
            findings.append(
                f"[A1] {name}: no AudioProfile and not marked deliberatelySilent — "
                f"cannot tell an authored-quiet world from a forgotten one.")

        # D2 — a world that grants nothing can never unlock what comes after it.
        job_flags = pack.get("jobs") or []
        if name not in NON_STORY_WORLDS and not (pack.get("flagsGranted") or job_flags):
            findings.append(
                f"[D2] {name}: grants no flags and has no jobs — nothing gated behind this "
                f"world could ever unlock.")

    # D5 — a world listing itself is a destination that goes nowhere. An EXIT pack legitimately
    # names another world's scene (that is what an exit is), so neither side may be an exit.
    for name, pack in packs.items():
        if "Exit" in name:
            continue
        scene = pack.get("sceneName") or ""
        for other, candidate in packs.items():
            if other == name or "Exit" in other:
                continue
            if candidate.get("sceneName") == scene and scene:
                findings.append(
                    f"[D5] {name} and {other} both claim scene '{scene}' — a ship helm would "
                    f"offer the world you are standing in.")
                break


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__,
                                     formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--strict", action="store_true",
                        help="exit 2 when findings exist")
    parser.add_argument("--json-report", help="write findings as JSON")
    args = parser.parse_args()

    findings: list = []
    seen = set()
    for folder in LAYOUTS:
        if not folder.is_dir():
            continue
        for path in sorted(folder.glob("*.asset")):
            if path in seen:
                continue
            seen.add(path)
            check_layout(path, findings)
    check_packs(findings)

    if findings:
        print(f"LEVEL INVARIANTS: {len(findings)} finding(s)\n")
        for finding in findings:
            print("  " + finding)
    else:
        print("LEVEL INVARIANTS: clean.")

    if args.json_report:
        out = pathlib.Path(args.json_report)
        out.parent.mkdir(parents=True, exist_ok=True)
        out.write_text(json.dumps({"findings": findings}, indent=2), encoding="utf-8")

    return 2 if (args.strict and findings) else 0


if __name__ == "__main__":
    sys.exit(main())
