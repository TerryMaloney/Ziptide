#!/usr/bin/env python3
"""Apply explicit stale-token refresh requests to the first-hour evidence inventory.

This tool never edits runtime source. A request is accepted only when:
- it identifies one exact beat and evidence source path,
- the old inventory token occurs exactly once in that evidence entry,
- the old token is absent from the current source file, and
- the replacement token is present in the current source file.

That makes source drift auditable without weakening first_hour_binding_gate.py.
"""

from __future__ import annotations

import argparse
import json
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Sequence

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


class RefreshError(RuntimeError):
    pass


@dataclass(frozen=True)
class AppliedRefresh:
    request_path: str
    beat_id: str
    evidence_path: str
    old_token: str
    new_token: str


def _load_json(path: Path, label: str) -> dict[str, Any]:
    try:
        value = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise RefreshError(f"Could not read {label}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise RefreshError(
            f"{label} is invalid JSON at {exc.lineno}:{exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(value, dict):
        raise RefreshError(f"{label} root must be an object.")
    return value


def _text(value: Any, field: str) -> str:
    if not isinstance(value, str) or not value.strip():
        raise RefreshError(f"{field} must be a non-empty string.")
    return value.strip()


def _safe_path(root: Path, raw: Any, field: str) -> Path:
    shown = _text(raw, field).replace("\\", "/")
    candidate = Path(shown)
    if candidate.is_absolute():
        raise RefreshError(f"{field} must be repository-relative: {shown}")
    resolved = (root / candidate).resolve()
    try:
        resolved.relative_to(root.resolve())
    except ValueError as exc:
        raise RefreshError(f"{field} escapes the repository root: {shown}") from exc
    return resolved


def _find_binding(inventory: dict[str, Any], beat_id: str) -> dict[str, Any]:
    bindings = inventory.get("bindings")
    if not isinstance(bindings, list):
        raise RefreshError("Inventory bindings must be a list.")
    matches = [
        item
        for item in bindings
        if isinstance(item, dict) and item.get("beatId") == beat_id
    ]
    if len(matches) != 1:
        raise RefreshError(
            f"Expected exactly one binding for {beat_id!r}; found {len(matches)}."
        )
    return matches[0]


def _find_evidence(binding: dict[str, Any], evidence_path: str) -> dict[str, Any]:
    entries = binding.get("evidence")
    if not isinstance(entries, list):
        raise RefreshError("Binding evidence must be a list.")
    normalized = evidence_path.replace("\\", "/")
    matches = [
        item
        for item in entries
        if isinstance(item, dict)
        and str(item.get("path", "")).replace("\\", "/") == normalized
    ]
    if len(matches) != 1:
        raise RefreshError(
            f"Expected exactly one evidence entry for {normalized!r}; found {len(matches)}."
        )
    return matches[0]


def apply_request(
    root: Path,
    inventory: dict[str, Any],
    request_path: Path,
    *,
    mutate: bool,
) -> list[AppliedRefresh]:
    request = _load_json(request_path, f"refresh request {request_path}")
    if request.get("schemaVersion") != SCHEMA_VERSION:
        raise RefreshError(f"{request_path}: schemaVersion must equal {SCHEMA_VERSION}.")

    changes = request.get("changes")
    if not isinstance(changes, list) or not changes:
        raise RefreshError(f"{request_path}: changes must be a non-empty list.")

    applied: list[AppliedRefresh] = []
    seen: set[tuple[str, str, str]] = set()
    for index, change in enumerate(changes):
        if not isinstance(change, dict):
            raise RefreshError(f"{request_path}: changes[{index}] must be an object.")
        beat_id = _text(change.get("beatId"), f"changes[{index}].beatId")
        evidence_path = _text(
            change.get("evidencePath"), f"changes[{index}].evidencePath"
        ).replace("\\", "/")
        old_token = _text(change.get("oldToken"), f"changes[{index}].oldToken")
        new_token = _text(change.get("newToken"), f"changes[{index}].newToken")
        if old_token == new_token:
            raise RefreshError(f"{request_path}: changes[{index}] oldToken equals newToken.")

        key = (beat_id, evidence_path, old_token)
        if key in seen:
            raise RefreshError(f"{request_path}: duplicate change for {key!r}.")
        seen.add(key)

        binding = _find_binding(inventory, beat_id)
        evidence = _find_evidence(binding, evidence_path)
        tokens = evidence.get("tokens")
        if not isinstance(tokens, list) or not all(isinstance(item, str) for item in tokens):
            raise RefreshError(
                f"{request_path}: {beat_id}/{evidence_path} tokens must be strings."
            )
        old_count = tokens.count(old_token)
        if old_count != 1:
            raise RefreshError(
                f"{request_path}: expected old token exactly once in inventory for "
                f"{beat_id}/{evidence_path}; found {old_count}."
            )
        if new_token in tokens:
            raise RefreshError(
                f"{request_path}: replacement token is already present for "
                f"{beat_id}/{evidence_path}."
            )

        source_path = _safe_path(root, evidence_path, f"changes[{index}].evidencePath")
        if not source_path.is_file():
            raise RefreshError(f"{request_path}: source file is missing: {evidence_path}")
        source = source_path.read_text(encoding="utf-8")
        if old_token in source:
            raise RefreshError(
                f"{request_path}: old token still exists in current source; refresh is not justified: "
                f"{beat_id}/{evidence_path}."
            )
        if new_token not in source:
            raise RefreshError(
                f"{request_path}: replacement token is not present in current source: "
                f"{beat_id}/{evidence_path}."
            )

        if mutate:
            tokens[tokens.index(old_token)] = new_token
        applied.append(
            AppliedRefresh(
                request_path=request_path.as_posix(),
                beat_id=beat_id,
                evidence_path=evidence_path,
                old_token=old_token,
                new_token=new_token,
            )
        )
    return applied


def refresh(
    root: Path,
    inventory_path: Path,
    queue_dir: Path,
    *,
    check_only: bool,
) -> list[AppliedRefresh]:
    root = root.resolve()
    inventory_path = inventory_path.resolve()
    queue_dir = queue_dir.resolve()
    inventory = _load_json(inventory_path, "first-hour inventory")
    requests = sorted(queue_dir.glob("*.json")) if queue_dir.is_dir() else []
    if not requests:
        raise RefreshError(f"No refresh request JSON files found in {queue_dir}.")

    applied: list[AppliedRefresh] = []
    for request_path in requests:
        applied.extend(
            apply_request(root, inventory, request_path, mutate=not check_only)
        )

    if not check_only:
        inventory_path.write_text(
            json.dumps(inventory, indent=2, ensure_ascii=False) + "\n",
            encoding="utf-8",
        )
        for request_path in requests:
            request_path.unlink()
    return applied


def _parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", default=".")
    parser.add_argument(
        "--inventory", default="docs/first_hour/first_hour_bindings.json"
    )
    parser.add_argument(
        "--queue-dir", default="docs/first_hour/evidence_refresh_queue"
    )
    parser.add_argument("--check", action="store_true")
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = _parser().parse_args(argv)
    root = Path(args.root).resolve()
    try:
        applied = refresh(
            root,
            root / args.inventory,
            root / args.queue_dir,
            check_only=args.check,
        )
    except (RefreshError, OSError, UnicodeError) as exc:
        print(f"FIRST_HOUR_EVIDENCE_REFRESH_ERROR: {exc}", file=sys.stderr)
        return EXIT_VALIDATION_FAILED

    mode = "validated" if args.check else "applied"
    print(f"FIRST_HOUR_EVIDENCE_REFRESH: {mode} {len(applied)} token refreshes")
    for item in applied:
        print(f"- {item.beat_id} :: {item.evidence_path}")
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
