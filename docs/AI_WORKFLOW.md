# ZIPTIDE — AI Dev Workflow

How Terry + Claude Code + GitHub + Unity work together without breaking the game.

---

## One-time machine setup

### Unity Smart Merge (UnityYAMLMerge) — prevents scene/prefab corruption on merge
`.gitattributes` already routes `*.unity/*.prefab/*.asset` through `merge=unityyamlmerge`.
Register the driver once (PowerShell, matches your Unity version path):
```powershell
$uy = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Data\Tools\UnityYAMLMerge.exe"
git config --global merge.unityyamlmerge.name "Unity SmartMerge"
git config --global merge.unityyamlmerge.driver "`"$uy`" merge -p `"%O`" `"%B`" `"%A`" `"%A`""
git config --global merge.unityyamlmerge.recursive binary
```
Now if two branches both edit a scene, git uses Unity's structured merge instead of mangling YAML.

---

## The session loop (do this every time)
```
1. tools\ziptide_snapshot.ps1            # branch + commit + build scenes + last ZIPTIDE tags
2. git pull                              # if it complains: git reset --hard origin/terry-local-wip
3. tools\dev_build_install.ps1           # build APK + install to Quest
4. tools\ziptide_snapshot.ps1            # confirm the Commit you expect is what's built
5. play, then: adb logcat -d | Out-File t.txt; Select-String t.txt "ZIPTIDE:|LOCO_STATE|MOVE_DIAG"
6. report by tag — diagnose from tags, not guesses
```
When a build plays well, tag it: `git tag ziptide-known-good-YYYY-MM-DD; git push --tags`.

---

## The "perfect loop" for a feature
1. **Plan** — Claude outlines the change (one issue-sized packet, not "improve the game").
2. **Branch** — `feature/*`, `art/*`, `audio/*`, or `recovery/*` (keep `main` near-stable).
3. **Execute** — Claude edits only the allowed files; runtime ensures / editor patchers over blind
   scene YAML edits.
4. **Verify** — CI compiles + EditMode tests on push; then Terry runs the on-device loop above.
   Merge only when green and verified on the headset.

---

## Auto-fix vs report-only
**Safe to auto-fix:** C# scripts, docs, audit/test code, idempotent scene patchers, registry
refresh, formatting, build/tooling scripts.

**Report-only (confirm before changing):** XR rig ownership, scene travel, input actions,
inventory persistence, walkable-route colliders, deleting scene objects, build-settings changes,
global material/shader changes. For these: `AUDIT_FAIL → suggested fix → human approval`.

**Never** hand-edit `.unity`/`.prefab` YAML blind for non-trivial changes — that has corrupted
scenes before. Use runtime ensures or editor patchers and verify in Unity.

---

## Tooling notes (future, optional)
- **MCP (Unity bridge):** stage it **read-only first** — inspect hierarchy, components, scene list,
  console logs, asset/GUID problems. Only later allow approved writes (run known menu commands /
  patchers, create ScriptableObjects). Never let it freestyle-edit scenes or change XR/Input/Boot.
- **OneJS / "AI Game Developer" live-C# bridges:** powerful for instant reflection without
  recompiling, but **hold until the project is stable** — they let an agent run code in the live
  editor, which is high-risk for exactly the rig/travel/input systems we just stabilized. Revisit
  once the 2-scene vertical slice is locked and CI is trusted.
- **GitHub cloud agents / Copilot:** only for isolated docs/CI/small PRs — never the main
  Quest-debugging loop (they can't wear the headset).
