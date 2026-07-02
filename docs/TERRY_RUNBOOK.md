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

## 2b. NEW — the 11 story worlds (modularity sprint; they build themselves)
The build now **auto-authors and ships W002–W012** (no menu steps — they appear in the Y+B dev menu after
your next build/sideload). Quick smoke per world (a minute each, warp via Y+B):
- [ ] Spawn is on solid ground, you can walk the loop, and the **sky/mood is distinct** (W002 pitch-dark
  cistern → W003 bright mesas → W004 dead-screen dread → W005 rust dusk → W006 blinding salt → W007
  space station + HUGE gridded planet → W008 gold-lit archive → W009 amber swarm city → W010 storm coast →
  W011 dark resonance → W012 raw void + the Shell filling the sky).
- [ ] The kiosk offers the contract; the objective board updates; drones fight where expected (W002 easy /
  W005+W009 standard).
- [ ] **Story gating:** doors to W002 stay LOCKED until the ToxicCity bounty is done, then unlock in chain
  order (`TRAVEL_LOCKED` in logcat when locked — that's correct behavior now, not a bug).
- [ ] Completing W004's contract logs `WORLD_FLAGS_GRANTED` + `TRANSMISSION_CLARITY tier=1` (the first
  fragment!).
- [ ] Feel notes: which worlds' scale/colors/heights feel wrong — each is fixable by editing ITS layout
  asset fields (see `HOW_TO_CHANGE_ANYTHING.md`).

## 2c. NEW — M1 "The Story Speaks" (RILL + fragments + playback; no menu steps, all auto)
The next build carries the story layer. Smoke it while doing §2b:
- [ ] **RILL exists:** a small glowing orb hovers near your **left shoulder** in every world; entering a
  world shows a **"RILL: …" subtitle** low-center (`ZIPTIDE: RILL_LINE` in logcat). W001/ToxicCity =
  "Systems nominal. I think."
- [ ] **RILL reacts to story beats:** finishing W004's contract → the cargo question; W009 → the
  misidentify glitch line; W012 → "it's a cage."
- [ ] **W002 collect step:** the contract now has "Collect 3 mineral sample" — three small glowing
  shards on the gallery route; grab each (`ZIPTIDE: COLLECTED`). Grabbing them EARLY (before that step)
  must still count when the step arrives.
- [ ] **W004 fragment is physical:** a "?? recording" shard at the broadcast core — grabbing it logs
  `COLLECTED item=transmission_fragment flag=FRAGMENT_T1_FOUND` + `TRANSMISSION_CLARITY tier=1` and
  RILL comments.
- [ ] **Playback console:** next to where the fragment sat is a small terminal — point + select the
  screen → mostly-static text with a few legible words (`TRANSMISSION_PLAYBACK tier=1`). (It gets
  clearer in later chapters — tier 0 shows "signal too degraded" if you somehow play it pre-fragment.)
- [ ] Feel notes: RILL orb position/size, subtitle readability, line timing, shard visibility.

## 2d. NEW — M2 "The Job Is Real" (hands-on repair, hazards, visible mine; all auto)
- [ ] **W002 — the full gate loop:** accept the contract → descend → down the drones → grab the 3
  mineral samples → at the pump house, **repair the pump with your hands**: pull the rusted access
  panel off (it comes free — drop it), walk back toward the shaft for the glowing **pump valve**, carry
  it to the exposed socket (it snaps in), flip the **power switch** → lamp goes green
  (`MACHINE_STAGE`/`MACHINE_REPAIRED`) → bounty pays.
- [ ] **W002 extractor:** next to the pump, a small rig with a floating readout that ticks up — select
  the **hopper** → yield pays into your profile (`MINE_COLLECT`). Leave the world, come back later —
  it produced while you were gone (`ECON_RESOLVE`).
- [ ] **W003 wind:** the two exposed bridge crossings **shove you sideways** (tinted lanes) — lean into
  it. `(HAZARD kind=Wind enter)`
- [ ] **W005 spores / W010 flood:** standing in the tinted pockets **slows you** (flash ticks); wading
  the W010 tide flats drags hard — the bridges are the safe route. Slows must **fully clear** when you
  step out (no stuck-slow).
- [ ] Feel notes: wind strength, slow severity, repair-part snap distance, mine rate/cap.

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
