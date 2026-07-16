from pathlib import Path


def load(path: str) -> list[str]:
    return Path(path).read_text(encoding="utf-8").splitlines()


def save(path: str, lines: list[str]) -> None:
    Path(path).write_text("\n".join(lines) + "\n", encoding="utf-8")


def ensure_after(lines: list[str], anchor: str, value: str, label: str) -> None:
    if value in lines:
        print(f"{label}: already present")
        return
    matches = [i for i, line in enumerate(lines) if line == anchor]
    print(f"{label}: anchor matches={len(matches)}")
    if len(matches) != 1:
        raise SystemExit(f"{label}: expected one anchor, found {len(matches)}")
    lines.insert(matches[0] + 1, value)


def replace_cache(lines: list[str], old_prefix: str, new_prefix: str, label: str) -> None:
    matches = [i for i, line in enumerate(lines) if old_prefix in line]
    if not matches:
        matches = [i for i, line in enumerate(lines) if new_prefix in line]
        if len(matches) == 1:
            print(f"{label}: already migrated")
            return
    print(f"{label}: key matches={len(matches)}")
    if len(matches) != 1:
        raise SystemExit(f"{label}: expected one cache key, found {len(matches)}")
    i = matches[0]
    lines[i] = lines[i].replace(old_prefix, new_prefix)
    key_indent = len(lines[i]) - len(lines[i].lstrip(" "))
    j = i + 1
    if j < len(lines) and lines[j].strip() == "restore-keys: |":
        del lines[j]
        while j < len(lines):
            line = lines[j]
            if not line.strip():
                break
            indent = len(line) - len(line.lstrip(" "))
            if indent <= key_indent:
                break
            del lines[j]
    print(f"{label}: broad restore fallback removed")


def ensure_artifact_after(lines: list[str], anchor_contains: str, value: str, label: str) -> None:
    if any(line.strip() == value.strip() for line in lines):
        print(f"{label}: already present")
        return
    matches = [i for i, line in enumerate(lines) if anchor_contains in line]
    print(f"{label}: anchor matches={len(matches)}")
    if len(matches) != 1:
        raise SystemExit(f"{label}: expected one artifact anchor, found {len(matches)}")
    indent = lines[matches[0]][: len(lines[matches[0]]) - len(lines[matches[0]].lstrip(" "))]
    lines.insert(matches[0] + 1, indent + value.strip())


playmode_path = ".github/workflows/recovery-playmode.yml"
golden_path = ".github/workflows/recovery-golden-android.yml"
ci_path = ".github/workflows/ci.yml"

playmode = load(playmode_path)
ensure_after(
    playmode,
    "      - 'Ziptide/Assets/Ziptide/Core/Runtime/Recovery/**'",
    "      - 'Ziptide/Packages/**'",
    "PlayMode package trigger coverage",
)
replace_cache(
    playmode,
    "Library-playmode-r1-",
    "Library-playmode-r1-clean-v2-",
    "PlayMode clean Library key",
)
ensure_artifact_after(
    playmode,
    "docs/recovery/generated/recovery_playmode_observation.md",
    "${{ env.PROJECT_PATH }}/Packages/packages-lock.json",
    "PlayMode resolved lock artifact",
)
save(playmode_path, playmode)

golden = load(golden_path)
ensure_after(
    golden,
    "      - 'Ziptide/Assets/Ziptide/Core/Runtime/Recovery/**'",
    "      - 'Ziptide/Packages/**'",
    "Golden package trigger coverage",
)
replace_cache(
    golden,
    "Library-recovery-golden-android-",
    "Library-recovery-golden-android-clean-v2-",
    "Golden clean Library key",
)
ensure_artifact_after(
    golden,
    "${{ env.PROJECT_PATH }}/Builds/Reports/*",
    "${{ env.PROJECT_PATH }}/Packages/packages-lock.json",
    "Golden resolved lock artifact",
)
save(golden_path, golden)

ci = load(ci_path)
replace_cache(ci, "Library-test-", "Library-test-clean-v2-", "EditMode clean Library key")
replace_cache(ci, "Library-audit-", "Library-audit-clean-v2-", "Audit clean Library key")
replace_cache(ci, "Library-android-", "Library-android-clean-v2-", "Android clean Library key")

if not any("${{ env.PROJECT_PATH }}/Packages/packages-lock.json" in line for line in ci):
    matches = [i for i, line in enumerate(ci) if line.strip() == "path: test-results"]
    print(f"EditMode resolved lock artifact: anchor matches={len(matches)}")
    if len(matches) != 1:
        raise SystemExit(f"EditMode resolved lock artifact: expected one path anchor, found {len(matches)}")
    i = matches[0]
    indent = ci[i][: len(ci[i]) - len(ci[i].lstrip(" "))]
    ci[i : i + 1] = [
        indent + "path: |",
        indent + "  test-results",
        indent + "  ${{ env.PROJECT_PATH }}/Packages/packages-lock.json",
    ]
else:
    print("EditMode resolved lock artifact: already present")

ensure_artifact_after(
    ci,
    "${{ env.PROJECT_PATH }}/Builds/Reports/*",
    "${{ env.PROJECT_PATH }}/Packages/packages-lock.json",
    "Audit resolved lock artifact",
)
save(ci_path, ci)
