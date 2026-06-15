# RECOVERY STEPS — get the workflow back on track

Do these IN ORDER when you're at the PC. Steps 1–2 fix the "flying blind" problem; do them
before any more gameplay testing. (Latest code is already pushed to `terry-local-wip`.)

---

## ✅ Current status: WORKFLOW HEALTHY (2026-06-15)
CI is green: it compiles the project + runs EditMode tests on every push to `terry-local-wip`.
Keep it that way — CLAUDE.md's rule stands (if CI goes red, warn loudly + stop shipping C#).

---

## STEP 1 — Re-activate the Unity license for CI (only if CI starts failing on license again)
**IMPORTANT — the website route is DEAD.** Unity **no longer supports manual activation of
Personal licenses** (`license.unity3d.com/manual` says so and only accepts paid Plus/Pro serials).
The old `.alf` → upload → `.ulf` flow **will never work for a free license** — don't try it. That
dead end is what caused the long `Code 20110: serial invalid` fight.

**The method that actually works — grab the `.ulf` your PC already has:**
1. Open **Unity Hub → (gear) Preferences/Settings → Licenses**. If no **Personal** license is
   listed, click **Add → Get a free personal license**. (This step writes the license file.)
2. In File Explorer open **`C:\ProgramData\Unity\`** (View → Hidden items if needed) and open
   **`Unity_lic.ulf`**. Confirm it's the REAL license, not a request file: a real `.ulf` has
   `<DeveloperData>`, `<Features>`, `<SerialMasked>`, `<Entitlements ... Tag="UnityPersonal">` and a
   `<Signature>` block. (A *request* file has ONLY `MachineBindings` + `MachineID` — wrong file.)
   - If that folder is missing, search C: for `Unity_lic.ulf` but verify it has the
     `<DeveloperData>`/`<Signature>` sections (ignore stale request files like old 2017 ones).
3. **Select all → copy** the `.ulf` contents.
4. GitHub repo → **Settings → Secrets and variables → Actions** → **`UNITY_LICENSE`** →
   **Update secret** → clear it, paste the entire `.ulf`, **save**.
5. Re-run CI (push, or Actions → CI → failed run → **Re-run failed jobs**). Success looks like
   **`Successfully returned ULF license with serial number`** in the activation log.

> `UNITY_EMAIL` / `UNITY_PASSWORD` secrets must also be set (CI log says
> `User *** logged in successfully` when they're right). Those were fine throughout — only the
> `.ulf` was the problem. The `.ulf` is machine-activated and can expire; redo this when it does.
> Also: `ci.yml` needs `permissions: checks: write`, else the run fails at the very end on
> "Resource not accessible by integration" even though compile + tests passed.

## STEP 2 — Give Claude eyes on the rig + scenes (~3 min)
This lets me SEE your XR rig interactor settings, spawn positions, and stray geometry instead of
guessing — the key to fixing grab feel and the lower-level glitch in one shot.

1. In Unity, open **`MilestoneA_GrabCube`** → menu **Ziptide → Diagnostics → Dump Scene + Rig
   Config**. (If the menu errors/doesn't appear, the new editor script didn't compile — tell me
   and delete `Assets/Ziptide/Editor/Audit/RigDumpExporter.cs`.)
2. Open **`_Boot`** → run the dump again.
3. Open **`D0_City`** → run the dump again.
4. Commit + push the generated files:
   ```powershell
   cd C:\Ziptide
   git add docs/_generated
   git commit -m "scene dumps for AI visibility"
   git push
   ```
5. Tell me they're pushed. I'll read them and fix grab-distance/orientation and the
   D0_City lower-level/test-target glitch precisely.

## STEP 3 — Resume testing (only after 1 & 2)
```powershell
cd C:\Ziptide
git pull            # if it complains: git reset --hard origin/terry-local-wip
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\ziptide_snapshot.ps1   # confirm commit
```
Test the holster-rides-the-hip fix, then report by `ZIPTIDE:` tags.

---

## The standing rule (also in CLAUDE.md)
If CI is red / can't compile, I must **warn loudly and stop shipping unverified code** until it's
fixed. We don't work blind. The goal is: headset on → "move that building / make this do that" →
it just happens with ~zero errors — which only works when the CI + audit safety net is healthy.

## Seeing the headset itself
I can't view the Quest directly. Closest options: (a) the `ZIPTIDE:` logcat tags (already our
main signal), (b) you can screen-mirror/record the Quest and paste screenshots — I can read
images. For the editor/scenes, the scene-dump in Step 2 is our "eyes."
