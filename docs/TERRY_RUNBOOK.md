# TERRY RUNBOOK — the stuff only you can do (Unity menus + headset)

**What this is:** the AI operator can't run Unity or the headset. Everything that needs your hands lands
here, batched, so nothing stalls waiting on you and you can clear it in one sitting. **When the operator
says "queued a step in the runbook," it's here.** Do the pending block, commit what it generates, then run
the device pass.

> How the split works (full detail in `docs/ROLES.md`): the operator writes C# **patchers**; you run a
> `Ziptide → …` menu that **generates** the `.unity`/`.asset` deterministically; you **commit** it; CI
> audits it. You never hand-edit scenes either — the menu does it.

---

## 1. Pending Unity menu steps (run in the editor, then commit the results)
Do these in order after pulling. Each generates committable assets. *(This mirrors
`DEVICE_TEST_CHECKLIST.md` §0 — that doc has the full copy-paste block.)*

- [ ] PowerShell: `cd C:\Ziptide; git pull origin terry-local-wip`
- [ ] `Ziptide → Worlds → Build Toxic City`
- [ ] `Ziptide → Worlds → Build Toxic City Contract`
- [ ] `Ziptide → Worlds → Build PvP Arena`
- [ ] `Ziptide → Dev → Build Starter World (graybox)`  *(regenerates with the safety floor)*
- [ ] `Ziptide → Dev → Build Sandbox Test Lab`  *(only if you want the sandbox refreshed)*
- [ ] **Commit** the generated `.unity` / `.asset` files (PowerShell: `git add -A; git commit -m "..."; git push origin terry-local-wip`).
- [ ] Build + install: `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1`
  *(re-runs all patchers, so scene-side fixes apply automatically.)*
- [ ] If install fails `INSTALL_FAILED_UPDATE_INCOMPATIBLE`: `adb uninstall com.terrymaloney.ziptide`, re-run.
- [ ] Unity Console has **no red errors** (harmless warnings only: `frameH` unused, `AudioDirector._fadeTimer/_fading`).

> **No manual "Rebuild Dev World Manifest" step** — the build now rebuilds the manifest itself, **last**
> (after the D0→"D0 City (legacy)" rename), so the Y+B menu is always correct and there's no duplicate
> "Toxic City". (Clicking it by hand is harmless; the build re-does it right.)

*Watch logcat during testing:* `adb logcat | findstr "ZIPTIDE"` *(dropped `-s Unity` — too strict; it hid everything last time.)*

## 2. Pending headset pass
- [ ] Run the full on-device checklist → **`docs/DEVICE_TEST_CHECKLIST.md`** (9 areas: rig/gun, dev menu,
  Toxic City, economy `ECON_RESOLVE`, travel-gating, PvP, Starter World, spawn/fall-safety, perf).
- [ ] Send the operator the **❌s + feel notes** (ray length, drone speed, scanner size, hop distance, gun
  grip, city scale). Those drive the next round.

## 3. The two open judgment calls (yours)
- **"Can you run in Toxic City?"** — was it the input bug (now fixed) or actual walls/narrow streets? If
  still wall-blocked, say so and the streets get widened.
- **Feel/tuning** across the board — it's all dial-able data; send impressions.

---

## Reference — the full editor menu map (for when the operator asks for a specific bake)
**Generate committable assets:** `Worlds → Build Toxic City` · `… Build Toxic City Contract` ·
`… Build PvP Arena` · `Worlds → Generate World From Selected Layout` / `Generate All Layout Worlds`
*(NEW — the world factory: any `CityLayoutDefinition` asset with a `sceneName` becomes a full world;
the build also auto-regenerates these, so running the menu is only needed to preview in-editor)* ·
`Dev → Build Starter World (graybox)` · `Dev → Build Sandbox Test Lab` · `Dev → Rebuild Dev World Manifest`.
**Apply to the open scene:** `Apply Boot Scene Patcher` · `Apply D0/D1/D2 … To Current Scene` ·
`Apply Theme To Current Scene`.
**Debug:** `Audit → Run Audit (All Scenes)` · `Diagnostics → Dump Scene + Rig Config` · `Dev → Warp Window`.

*Nothing here is a code change — it's baking + testing. The operator keeps this list current; if a task in
`FABLE5_BACKLOG.md` is tagged 🔧UNITY or 🎮DEVICE, its step shows up here.*
