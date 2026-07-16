from pathlib import Path


def replace_once(path: str, old: str, new: str, label: str) -> None:
    p = Path(path)
    text = p.read_text(encoding="utf-8")
    count = text.count(old)
    print(f"{label}: matches={count}")
    if count != 1:
        raise SystemExit(f"{label}: expected one exact match, found {count}")
    p.write_text(text.replace(old, new), encoding="utf-8")


playmode = ".github/workflows/recovery-playmode.yml"
golden = ".github/workflows/recovery-golden-android.yml"
ci = ".github/workflows/ci.yml"

replace_once(
    playmode,
    "      - 'Ziptide/Assets/Ziptide/Core/Runtime/Recovery/**'\n",
    "      - 'Ziptide/Assets/Ziptide/Core/Runtime/Recovery/**'\n"
    "      - 'Ziptide/Packages/**'\n",
    "PlayMode package trigger coverage",
)
replace_once(
    playmode,
    "          key: Library-playmode-r1-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n"
    "          restore-keys: |\n"
    "            Library-playmode-r1-\n"
    "            Library-\n",
    "          # Package-matrix changes must import from a clean Library. Exact-key reuse is safe;\n"
    "          # cross-hash fallback restores are forbidden because they mix compiled package assemblies.\n"
    "          key: Library-playmode-r1-clean-v2-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n",
    "PlayMode clean Library key",
)
replace_once(
    playmode,
    "            docs/recovery/generated/recovery_playmode_observation.md\n",
    "            docs/recovery/generated/recovery_playmode_observation.md\n"
    "            ${{ env.PROJECT_PATH }}/Packages/packages-lock.json\n",
    "PlayMode resolved lock artifact",
)

replace_once(
    golden,
    "      - 'Ziptide/Assets/Ziptide/Core/Runtime/Recovery/**'\n",
    "      - 'Ziptide/Assets/Ziptide/Core/Runtime/Recovery/**'\n"
    "      - 'Ziptide/Packages/**'\n",
    "Golden package trigger coverage",
)
replace_once(
    golden,
    "          key: Library-recovery-golden-android-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n"
    "          restore-keys: |\n"
    "            Library-recovery-golden-android-\n"
    "            Library-android-\n"
    "            Library-\n",
    "          # Never restore a Unity Library compiled against a different package graph.\n"
    "          key: Library-recovery-golden-android-clean-v2-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n",
    "Golden clean Library key",
)
replace_once(
    golden,
    "            ${{ env.PROJECT_PATH }}/Builds/Reports/*\n",
    "            ${{ env.PROJECT_PATH }}/Builds/Reports/*\n"
    "            ${{ env.PROJECT_PATH }}/Packages/packages-lock.json\n",
    "Golden resolved lock artifact",
)

replace_once(
    ci,
    "          key: Library-test-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n"
    "          restore-keys: |\n"
    "            Library-test-\n"
    "            Library-\n",
    "          key: Library-test-clean-v2-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n",
    "EditMode clean Library key",
)
replace_once(
    ci,
    "          path: test-results\n",
    "          path: |\n"
    "            test-results\n"
    "            ${{ env.PROJECT_PATH }}/Packages/packages-lock.json\n",
    "EditMode resolved lock artifact",
)
replace_once(
    ci,
    "          key: Library-audit-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n"
    "          restore-keys: |\n"
    "            Library-audit-\n"
    "            Library-\n",
    "          key: Library-audit-clean-v2-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n",
    "Audit clean Library key",
)
replace_once(
    ci,
    "            ${{ env.PROJECT_PATH }}/Builds/Reports/*\n",
    "            ${{ env.PROJECT_PATH }}/Builds/Reports/*\n"
    "            ${{ env.PROJECT_PATH }}/Packages/packages-lock.json\n",
    "Audit resolved lock artifact",
)
replace_once(
    ci,
    "          key: Library-android-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n"
    "          restore-keys: |\n"
    "            Library-android-\n"
    "            Library-\n",
    "          key: Library-android-clean-v2-${{ hashFiles(format('{0}/Assets/**', env.PROJECT_PATH), format('{0}/Packages/**', env.PROJECT_PATH), format('{0}/ProjectSettings/**', env.PROJECT_PATH)) }}\n",
    "Android clean Library key",
)
