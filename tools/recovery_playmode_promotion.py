#!/usr/bin/env python3
"""Validate whether ZIPTIDE's R1.1 PlayMode observation lane may be promoted.

Promotion requires two successful, distinct-SHA runs with non-expired artifacts, the
required Unity version, and at least one successful unrelated-descendant probe.
"""

from __future__ import annotations

import argparse
import json
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Any, Sequence

EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_NOT_PROMOTABLE = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    tested_sha: str = ""


@dataclass(frozen=True)
class Result:
    promotable: bool
    successful_runs: int
    distinct_successful_shas: int
    unrelated_successes: int
    findings: tuple[Finding, ...]

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "recovery_playmode_promotion",
            "promotable": self.promotable,
            "successfulRuns": self.successful_runs,
            "distinctSuccessfulShas": self.distinct_successful_shas,
            "unrelatedSuccesses": self.unrelated_successes,
            "findings": [asdict(item) for item in self.findings],
        }


class HistoryError(RuntimeError):
    pass


def load_history(path: Path) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise HistoryError(f"Could not read history: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise HistoryError(f"Invalid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}") from exc
    if not isinstance(data, dict):
        raise HistoryError("History root must be an object.")
    return data


def validate_history(history: dict[str, Any]) -> Result:
    findings: list[Finding] = []
    if history.get("schemaVersion") != 1:
        findings.append(Finding("SCHEMA_VERSION", "schemaVersion must equal 1."))

    requirements = history.get("requirements")
    if not isinstance(requirements, dict):
        requirements = {}
        findings.append(Finding("REQUIREMENTS_MISSING", "requirements must be an object."))

    minimum = requirements.get("minimumSuccessfulRuns", 2)
    if not isinstance(minimum, int) or isinstance(minimum, bool) or minimum < 2:
        findings.append(Finding("MINIMUM_INVALID", "minimumSuccessfulRuns must be an integer >= 2."))
        minimum = 2

    required_unity = requirements.get("unityVersion")
    observations = history.get("observations")
    if not isinstance(observations, list):
        observations = []
        findings.append(Finding("OBSERVATIONS_MISSING", "observations must be a list."))

    successful: list[dict[str, Any]] = []
    seen_run_ids: set[int] = set()
    for index, raw in enumerate(observations):
        if not isinstance(raw, dict):
            findings.append(Finding("OBSERVATION_INVALID", f"observations[{index}] must be an object."))
            continue
        sha = str(raw.get("testedSha") or "")
        run_id = raw.get("workflowRunId")
        if not sha:
            findings.append(Finding("SHA_MISSING", f"observations[{index}] has no testedSha."))
        if not isinstance(run_id, int) or isinstance(run_id, bool):
            findings.append(Finding("RUN_ID_INVALID", f"observations[{index}] has invalid workflowRunId.", sha))
        elif run_id in seen_run_ids:
            findings.append(Finding("RUN_ID_DUPLICATE", f"workflowRunId {run_id} is duplicated.", sha))
        else:
            seen_run_ids.add(run_id)

        if raw.get("outcome") != "success":
            continue
        successful.append(raw)
        if required_unity and raw.get("unityVersion") != required_unity:
            findings.append(
                Finding(
                    "UNITY_VERSION_MISMATCH",
                    f"Expected Unity {required_unity}, got {raw.get('unityVersion')!r}.",
                    sha,
                )
            )
        artifact = raw.get("artifact")
        if not isinstance(artifact, dict):
            findings.append(Finding("ARTIFACT_MISSING", "Successful run has no artifact record.", sha))
        else:
            if artifact.get("expired") is not False:
                findings.append(Finding("ARTIFACT_EXPIRED", "Successful run artifact is expired/unknown.", sha))
            if not isinstance(artifact.get("id"), int):
                findings.append(Finding("ARTIFACT_ID_INVALID", "Artifact id must be an integer.", sha))
            if not isinstance(artifact.get("sizeBytes"), int) or artifact.get("sizeBytes", 0) <= 0:
                findings.append(Finding("ARTIFACT_EMPTY", "Artifact size must be positive.", sha))

    distinct_shas = {str(item.get("testedSha") or "") for item in successful if item.get("testedSha")}
    unrelated = [item for item in successful if item.get("unrelatedDescendant") is True]

    if len(successful) < minimum:
        findings.append(
            Finding("SUCCESS_COUNT", f"Need at least {minimum} successful runs; found {len(successful)}.")
        )
    if requirements.get("requiresDistinctShas", True) and len(distinct_shas) < minimum:
        findings.append(
            Finding("DISTINCT_SHA_COUNT", f"Need at least {minimum} distinct successful SHAs; found {len(distinct_shas)}.")
        )
    if requirements.get("requiresUnrelatedDescendant", True) and not unrelated:
        findings.append(Finding("UNRELATED_DESCENDANT_MISSING", "No successful unrelated-descendant probe recorded."))

    promotable = not findings
    return Result(promotable, len(successful), len(distinct_shas), len(unrelated), tuple(findings))


def write_report(result: Result, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument(
        "--history",
        type=Path,
        default=Path("docs/recovery/playmode_observation_history.json"),
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        default=Path("Builds/Reports/recovery_playmode_promotion.json"),
    )
    parser.add_argument("--strict", action="store_true")
    return parser


def _resolve(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    try:
        result = validate_history(load_history(_resolve(root, args.history)))
        report_path = _resolve(root, args.json_report).resolve()
        report_path.relative_to(root)
        write_report(result, report_path)
    except (HistoryError, OSError, ValueError) as exc:
        print(f"RECOVERY_PLAYMODE_PROMOTION_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    print(
        "RECOVERY_PLAYMODE_PROMOTION "
        f"promotable={str(result.promotable).lower()} "
        f"successful={result.successful_runs} distinct={result.distinct_successful_shas} "
        f"unrelated={result.unrelated_successes} findings={len(result.findings)}"
    )
    for finding in result.findings:
        suffix = f" sha={finding.tested_sha}" if finding.tested_sha else ""
        print(f"PROMOTION_FINDING code={finding.code}{suffix} message={finding.message}")
    if args.strict and not result.promotable:
        return EXIT_NOT_PROMOTABLE
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
