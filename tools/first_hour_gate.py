#!/usr/bin/env python3
"""Validate the ZIPTIDE first-hour beat contract.

Default mode is report-only. Use --strict only after the project explicitly
ratchets this contract into required CI.
"""

from __future__ import annotations

import argparse
import json
import sys
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Iterable, Sequence

SCHEMA_VERSION = 1
EXIT_OK = 0
EXIT_OPERATIONAL_ERROR = 1
EXIT_VALIDATION_FAILED = 2


@dataclass(frozen=True)
class Finding:
    code: str
    message: str
    beat_id: str = ""
    path: str = ""
    severity: str = "warning"


@dataclass(frozen=True)
class ValidationResult:
    contract_path: str
    root: str
    schema_version: int | None
    beat_count: int
    core_verb_count: int
    findings: tuple[Finding, ...]

    @property
    def status(self) -> str:
        return "pass" if not self.findings else "warning"

    def to_dict(self) -> dict[str, Any]:
        return {
            "tool": "first_hour_gate",
            "toolVersion": 1,
            "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
            "status": self.status,
            "root": self.root,
            "contractPath": self.contract_path,
            "schemaVersion": self.schema_version,
            "beatCount": self.beat_count,
            "coreVerbCount": self.core_verb_count,
            "findingCount": len(self.findings),
            "findings": [asdict(finding) for finding in self.findings],
        }


class ContractLoadError(RuntimeError):
    """Raised when validation cannot begin because the contract is unreadable."""


def _is_nonempty_string(value: Any) -> bool:
    return isinstance(value, str) and bool(value.strip())


def _duplicate_values(values: Iterable[str]) -> set[str]:
    seen: set[str] = set()
    duplicates: set[str] = set()
    for value in values:
        if value in seen:
            duplicates.add(value)
        seen.add(value)
    return duplicates


def _safe_repo_path(root: Path, raw_path: Any, *, field: str) -> tuple[Path | None, Finding | None]:
    if not _is_nonempty_string(raw_path):
        return None, Finding("SOURCE_PATH_INVALID", f"{field} must be a non-empty string.")
    candidate_text = str(raw_path).replace("\\", "/").strip()
    candidate = Path(candidate_text)
    if candidate.is_absolute():
        return None, Finding(
            "SOURCE_PATH_ABSOLUTE",
            f"{field} must be repository-relative.",
            path=candidate_text,
        )
    resolved_root = root.resolve()
    resolved_candidate = (resolved_root / candidate).resolve()
    try:
        resolved_candidate.relative_to(resolved_root)
    except ValueError:
        return None, Finding(
            "SOURCE_PATH_OUTSIDE_ROOT",
            f"{field} escapes the repository root.",
            path=candidate_text,
        )
    return resolved_candidate, None


def _string_list(
    value: Any,
    *,
    field: str,
    findings: list[Finding],
    required: bool = False,
    beat_id: str = "",
) -> list[str]:
    if value is None and not required:
        return []
    if not isinstance(value, list):
        findings.append(Finding("FIELD_TYPE_INVALID", f"{field} must be a list.", beat_id=beat_id))
        return []
    valid: list[str] = []
    for index, item in enumerate(value):
        if not _is_nonempty_string(item):
            findings.append(
                Finding(
                    "FIELD_ITEM_INVALID",
                    f"{field}[{index}] must be a non-empty string.",
                    beat_id=beat_id,
                )
            )
            continue
        valid.append(item.strip())
    if required and not valid:
        findings.append(
            Finding("FIELD_REQUIRED", f"{field} must contain at least one item.", beat_id=beat_id)
        )
    return valid


def load_contract(contract_path: Path) -> dict[str, Any]:
    try:
        text = contract_path.read_text(encoding="utf-8")
    except OSError as exc:
        raise ContractLoadError(f"Could not read contract: {exc}") from exc
    try:
        data = json.loads(text)
    except json.JSONDecodeError as exc:
        raise ContractLoadError(
            f"Contract is not valid JSON at line {exc.lineno}, column {exc.colno}: {exc.msg}"
        ) from exc
    if not isinstance(data, dict):
        raise ContractLoadError("Contract root must be a JSON object.")
    return data


def validate_contract(root: Path, contract_path: Path) -> ValidationResult:
    root = root.resolve()
    contract_path = contract_path.resolve()
    contract = load_contract(contract_path)
    findings: list[Finding] = []

    schema_version = contract.get("schemaVersion")
    if schema_version != SCHEMA_VERSION:
        findings.append(
            Finding(
                "SCHEMA_VERSION_UNSUPPORTED",
                f"schemaVersion must equal {SCHEMA_VERSION}; got {schema_version!r}.",
            )
        )

    if not _is_nonempty_string(contract.get("contractId")):
        findings.append(Finding("CONTRACT_ID_INVALID", "contractId must be a non-empty string."))

    source_documents = _string_list(
        contract.get("sourceDocuments"),
        field="sourceDocuments",
        findings=findings,
        required=True,
    )
    for index, raw_path in enumerate(source_documents):
        candidate, issue = _safe_repo_path(root, raw_path, field=f"sourceDocuments[{index}]")
        if issue:
            findings.append(issue)
            continue
        assert candidate is not None
        if not candidate.exists():
            findings.append(
                Finding(
                    "SOURCE_DOCUMENT_MISSING",
                    f"sourceDocuments[{index}] does not exist.",
                    path=raw_path,
                )
            )

    core_verbs = _string_list(
        contract.get("coreVerbs"),
        field="coreVerbs",
        findings=findings,
        required=True,
    )
    for duplicate in sorted(_duplicate_values(core_verbs)):
        findings.append(Finding("CORE_VERB_DUPLICATE", "coreVerbs contains a duplicate.", path=duplicate))

    completion_flags = set(
        _string_list(
            contract.get("completionFlags"),
            field="completionFlags",
            findings=findings,
            required=True,
        )
    )
    allowed_evidence = set(
        _string_list(
            contract.get("allowedEvidence"),
            field="allowedEvidence",
            findings=findings,
            required=True,
        )
    )
    binding_statuses = set(
        _string_list(
            contract.get("bindingStatuses"),
            field="bindingStatuses",
            findings=findings,
            required=True,
        )
    )

    beats = contract.get("beats")
    if not isinstance(beats, list) or not beats:
        findings.append(Finding("BEATS_REQUIRED", "beats must be a non-empty list."))
        beats = []

    beat_ids: list[str] = []
    sequence_values: list[int] = []
    parsed_beats: list[dict[str, Any]] = []
    beat_by_id: dict[str, dict[str, Any]] = {}
    taught: dict[str, list[str]] = {verb: [] for verb in core_verbs}

    for index, beat in enumerate(beats):
        prefix = f"beats[{index}]"
        if not isinstance(beat, dict):
            findings.append(Finding("BEAT_INVALID", f"{prefix} must be an object."))
            continue

        beat_id = beat.get("id")
        if not _is_nonempty_string(beat_id):
            findings.append(Finding("BEAT_ID_INVALID", f"{prefix}.id must be a non-empty string."))
            beat_id = f"<invalid-{index}>"
        else:
            beat_id = beat_id.strip()
            beat_ids.append(beat_id)

        sequence = beat.get("sequence")
        if not isinstance(sequence, int) or isinstance(sequence, bool) or sequence < 1:
            findings.append(
                Finding("BEAT_SEQUENCE_INVALID", f"{prefix}.sequence must be an integer >= 1.", beat_id)
            )
        else:
            sequence_values.append(sequence)

        required = beat.get("required")
        if not isinstance(required, bool):
            findings.append(Finding("BEAT_REQUIRED_INVALID", f"{prefix}.required must be boolean.", beat_id))

        if beat.get("allowsInputLock") is not False:
            findings.append(
                Finding("INPUT_LOCK_FORBIDDEN", "First-hour beats may not lock player input.", beat_id)
            )
        if beat.get("movesPlayerRig") is not False:
            findings.append(
                Finding("RIG_MOTION_FORBIDDEN", "First-hour beats may not translate or rotate the player rig.", beat_id)
            )
        if beat.get("autoComplete") is not False:
            findings.append(
                Finding("AUTO_COMPLETE_FORBIDDEN", "First-hour beats require explicit completion signals.", beat_id)
            )

        prerequisites = _string_list(
            beat.get("prerequisites"),
            field=f"{prefix}.prerequisites",
            findings=findings,
            beat_id=beat_id,
        )
        evidence = _string_list(
            beat.get("evidence"),
            field=f"{prefix}.evidence",
            findings=findings,
            required=True,
            beat_id=beat_id,
        )
        for evidence_class in evidence:
            if evidence_class not in allowed_evidence:
                findings.append(
                    Finding(
                        "EVIDENCE_CLASS_UNKNOWN",
                        f"Unknown evidence class {evidence_class!r}.",
                        beat_id,
                    )
                )

        completion_signal = beat.get("completionSignal")
        if not isinstance(completion_signal, dict):
            findings.append(
                Finding("COMPLETION_SIGNAL_INVALID", "completionSignal must be an object.", beat_id)
            )
        else:
            if not _is_nonempty_string(completion_signal.get("id")):
                findings.append(
                    Finding("COMPLETION_SIGNAL_ID_INVALID", "completionSignal.id is required.", beat_id)
                )
            signal_status = completion_signal.get("status")
            if signal_status not in binding_statuses:
                findings.append(
                    Finding(
                        "BINDING_STATUS_UNKNOWN",
                        f"completionSignal.status {signal_status!r} is not allowed.",
                        beat_id,
                    )
                )

        teaches_verb = beat.get("teachesVerb")
        if teaches_verb is not None:
            if teaches_verb not in taught:
                findings.append(
                    Finding("TAUGHT_VERB_UNKNOWN", f"Unknown teachesVerb {teaches_verb!r}.", beat_id)
                )
            else:
                taught[teaches_verb].append(beat_id)

            hesitation = beat.get("hesitationSeconds")
            if not isinstance(hesitation, (int, float)) or isinstance(hesitation, bool) or hesitation <= 0:
                findings.append(
                    Finding(
                        "HESITATION_INVALID",
                        "Teaching beats require hesitationSeconds > 0.",
                        beat_id,
                    )
                )

            rill_line = beat.get("rillLine")
            if not isinstance(rill_line, dict) or not _is_nonempty_string(rill_line.get("id")):
                findings.append(
                    Finding("RILL_LINE_REQUIRED", "Teaching beats require a rillLine.id.", beat_id)
                )
            else:
                line_status = rill_line.get("status")
                if line_status not in binding_statuses:
                    findings.append(
                        Finding(
                            "BINDING_STATUS_UNKNOWN",
                            f"rillLine.status {line_status!r} is not allowed.",
                            beat_id,
                        )
                    )

        sets_flags = _string_list(
            beat.get("setsFlags"),
            field=f"{prefix}.setsFlags",
            findings=findings,
            beat_id=beat_id,
        )

        parsed = {
            "id": beat_id,
            "sequence": sequence,
            "required": required,
            "prerequisites": prerequisites,
            "setsFlags": sets_flags,
        }
        parsed_beats.append(parsed)
        beat_by_id[beat_id] = parsed

    for duplicate in sorted(_duplicate_values(beat_ids)):
        findings.append(Finding("BEAT_ID_DUPLICATE", "Beat id is declared more than once.", duplicate))
    for duplicate in sorted(_duplicate_values(str(value) for value in sequence_values)):
        findings.append(
            Finding("BEAT_SEQUENCE_DUPLICATE", "Beat sequence is declared more than once.", path=duplicate)
        )

    if sequence_values:
        expected = list(range(1, len(sequence_values) + 1))
        if sorted(sequence_values) != expected:
            findings.append(
                Finding(
                    "BEAT_SEQUENCE_NOT_CONTIGUOUS",
                    f"Beat sequences must be contiguous 1..{len(sequence_values)}.",
                )
            )

    for verb, owners in taught.items():
        if not owners:
            findings.append(Finding("CORE_VERB_UNTAUGHT", f"Core verb {verb} is not taught.", path=verb))
        elif len(owners) > 1:
            findings.append(
                Finding(
                    "CORE_VERB_TAUGHT_MULTIPLE",
                    f"Core verb {verb} is taught by multiple beats: {owners}.",
                    path=verb,
                )
            )

    for beat in parsed_beats:
        beat_id = beat["id"]
        sequence = beat["sequence"]
        for prerequisite_id in beat["prerequisites"]:
            prerequisite = beat_by_id.get(prerequisite_id)
            if prerequisite is None:
                findings.append(
                    Finding(
                        "PREREQUISITE_UNKNOWN",
                        f"Unknown prerequisite {prerequisite_id!r}.",
                        beat_id,
                    )
                )
                continue
            if isinstance(sequence, int) and isinstance(prerequisite["sequence"], int):
                if prerequisite["sequence"] >= sequence:
                    findings.append(
                        Finding(
                            "PREREQUISITE_NOT_EARLIER",
                            f"Prerequisite {prerequisite_id} must occur earlier.",
                            beat_id,
                        )
                    )
            if beat["required"] is True and prerequisite["required"] is False:
                findings.append(
                    Finding(
                        "REQUIRED_DEPENDS_ON_OPTIONAL",
                        f"Required beat depends on optional beat {prerequisite_id}.",
                        beat_id,
                    )
                )

    required_beats = [beat for beat in parsed_beats if beat["required"] is True]
    if required_beats:
        ordered_required = sorted(
            required_beats,
            key=lambda beat: beat["sequence"] if isinstance(beat["sequence"], int) else 10**9,
        )
        start = ordered_required[0]
        if start["prerequisites"]:
            findings.append(
                Finding("FIRST_REQUIRED_HAS_PREREQUISITE", "The first required beat must be a root.", start["id"])
            )

        reachable: set[str] = {start["id"]}
        changed = True
        while changed:
            changed = False
            for beat in ordered_required:
                if beat["id"] in reachable:
                    continue
                if beat["prerequisites"] and all(prereq in reachable for prereq in beat["prerequisites"]):
                    reachable.add(beat["id"])
                    changed = True
        for beat in ordered_required:
            if beat["id"] not in reachable:
                findings.append(
                    Finding("REQUIRED_BEAT_UNREACHABLE", "Required beat is not reachable from the root.", beat["id"])
                )

        final = ordered_required[-1]
        missing_final_flags = sorted(completion_flags.difference(final["setsFlags"]))
        if missing_final_flags:
            findings.append(
                Finding(
                    "FINAL_COMPLETION_FLAGS_MISSING",
                    f"Final required beat must set: {missing_final_flags}.",
                    final["id"],
                )
            )

    return ValidationResult(
        contract_path=str(contract_path),
        root=str(root),
        schema_version=schema_version if isinstance(schema_version, int) else None,
        beat_count=len(beats),
        core_verb_count=len(core_verbs),
        findings=tuple(findings),
    )


def write_report(result: ValidationResult, report_path: Path) -> None:
    report_path.parent.mkdir(parents=True, exist_ok=True)
    report_path.write_text(json.dumps(result.to_dict(), indent=2) + "\n", encoding="utf-8")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parents[1],
        help="Repository root. Defaults to the parent of tools/.",
    )
    parser.add_argument(
        "--contract",
        type=Path,
        default=Path("docs/first_hour/first_hour_beats.json"),
        help="Contract path, relative to --root unless absolute.",
    )
    parser.add_argument(
        "--json-report",
        type=Path,
        help="Optional JSON report path, relative to --root unless absolute.",
    )
    parser.add_argument(
        "--strict",
        action="store_true",
        help="Return exit code 2 when findings exist. Default is report-only.",
    )
    return parser


def _resolve_under_root(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    root = args.root.resolve()
    contract_path = _resolve_under_root(root, args.contract).resolve()

    try:
        result = validate_contract(root, contract_path)
    except ContractLoadError as exc:
        print(f"FIRST_HOUR_ERROR {exc}", file=sys.stderr)
        return EXIT_OPERATIONAL_ERROR

    if args.json_report:
        report_path = _resolve_under_root(root, args.json_report).resolve()
        try:
            report_path.relative_to(root)
        except ValueError:
            print("FIRST_HOUR_ERROR --json-report must remain inside --root.", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR
        try:
            write_report(result, report_path)
        except OSError as exc:
            print(f"FIRST_HOUR_ERROR Could not write report: {exc}", file=sys.stderr)
            return EXIT_OPERATIONAL_ERROR

    print(
        "FIRST_HOUR_REPORT "
        f"status={result.status.upper()} findings={len(result.findings)} "
        f"beats={result.beat_count} verbs={result.core_verb_count} "
        f"contract={contract_path.relative_to(root)}"
    )
    for finding in result.findings:
        beat = f" beat={finding.beat_id}" if finding.beat_id else ""
        path = f" path={finding.path}" if finding.path else ""
        print(f"FIRST_HOUR_FINDING code={finding.code}{beat}{path} message={finding.message}")

    if args.strict and result.findings:
        return EXIT_VALIDATION_FAILED
    return EXIT_OK


if __name__ == "__main__":
    raise SystemExit(main())
