# RECOVERY STEPS — get the workflow back on track

Do these IN ORDER when you're at the PC. Steps 1–2 fix the "flying blind" problem; do them
before any more gameplay testing. (Latest code is already pushed to `terry-local-wip`.)

---

## ⚠️ Current status: WORKFLOW DEGRADED
- **CI cannot compile-check** — the GitHub Actions CI fails at Unity license activation
  (`Code 20110: serial invalid`). The Personal `.ulf` expired. Until fixed, broken C# can reach
  your build (that's what the `enableAnchorControl` error was). **Top priority.**

---

## STEP 1 — Re-activate the Unity license for CI (~5 min)
I will trigger the "Acquire Unity Activation File" workflow for you (or you can: GitHub → Actions
→ that workflow → Run workflow on `main`). Then:

1. Open the finished run → download the **`Unity_Activation_File`** artifact → unzip → you get a
   `.alf` file.
2. Go to **https://license.unity3d.com/manual**, sign in, upload the `.alf`.
3. Choose **Unity Personal Edition** (free) → answer the personal-use questions → download the
   resulting **`.ulf`** file.
4. Open the `.ulf` in Notepad, **select all, copy**.
5. GitHub repo → **Settings → Secrets and variables → Actions** → open **`UNITY_LICENSE`** →
   **Update secret** → paste the entire `.ulf` contents → save.
6. GitHub → Actions → **CI** → Run workflow on `terry-local-wip` (or just push). Confirm the
   **EditMode tests** job goes **green**. Green = CI compiles again = we're no longer blind.

> If activation keeps failing: the email/password secrets may also need refresh
> (`UNITY_EMAIL`, `UNITY_PASSWORD`). A Personal license sometimes needs the password secret set.

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
