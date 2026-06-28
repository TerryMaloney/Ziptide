# Ziptide — Device Test Checklist (everything shipped, awaiting on-device verify)

**Why:** a lot of fixes/features landed CI-green (compile + EditMode) but **not headset-verified**. This is
the full pass for a real test session. Tags in `()` are the `ZIPTIDE:` logcat lines that confirm a thing
fired. Watch logcat with:
```
adb logcat -s Unity | findstr "ZIPTIDE:"
```
Mark ✅/❌ and note anything off — especially **feel** items (those are tuning, not pass/fail).

---

## 0. One-time setup (full copy-paste)

**STEP 1 — PowerShell: get the latest code**
```powershell
cd C:\Ziptide
git pull origin terry-local-wip
```

**STEP 2 — In Unity (click these in the top menu bar, in order; wait for each to finish):**
1. Ziptide → Worlds → Build Toxic City
2. Ziptide → Worlds → Build Toxic City Contract
3. Ziptide → Worlds → Build PvP Arena
4. Ziptide → Dev → Build Starter World (graybox)
5. Ziptide → Dev → Rebuild Dev World Manifest   ← LAST (needs the worlds above to exist first)

**STEP 3 — PowerShell: save the generated worlds to git** (`"nothing to commit"` is fine)
```powershell
cd C:\Ziptide
git add -A
git commit -m "Generate worlds + manifest for device test"
git push origin terry-local-wip
```

**STEP 4 — PowerShell: build + install to the headset**
```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
```
If it fails with `INSTALL_FAILED_UPDATE_INCOMPATIBLE`, run this then re-run STEP 4:
```powershell
adb uninstall com.terrymaloney.ziptide
```

**STEP 5 — PowerShell (second window): watch logs while you play**
```powershell
adb logcat -s Unity | findstr "ZIPTIDE:"
```

- [ ] Unity Console has **no red errors** before STEP 4 (only expected warnings are cosmetic:
  `frameH` unused, `AudioDirector._fadeTimer/_fading` unused — harmless).

---

## 1. Rig / locomotion / gun (the recurring trio — the big ones)
- [ ] **Right thumbstick TURNS you** (snap/smooth) and does **NOT** move you forward/back. *(the #1 recurring bug)*
- [ ] **Left thumbstick moves you.**
- [ ] **Right thumbstick does NOT rotate the held gun** when you push it. *(anchor-control fix via reflection — if the gun still spins, it means IL2CPP stripped the field and I add a preserve rule)*
- [ ] **Interactor rays are short** (~2.5 m), not stretching across the room. *(was the line-visual, not raycast distance)*
- [ ] Grab a gun → **snaps to a forward grip**; **release it → it FALLS** (doesn't freeze/float in the air). `(WEAPON pickup)`
- [ ] Holster a gun → travel to another world → pull it out → **release → it falls** (not frozen). *(holster-travel gun-drop fix)*

## 2. Dev menu / warp
- [ ] Boot lands in **Sandbox** (intentional dev bypass for now).
- [ ] **Y+B** opens the dev menu; it lists **Toxic City** (the new one), **PvP Arena**, Starter World, Sandbox.
- [ ] Only **ONE** "Toxic City" (the old D0 now shows as **"D0 City (legacy)"**). *(dedupe fix)*
- [ ] Warp works — and works **more than once / after closing & reopening** the menu. *(the old "clickable once" bug)*

## 3. Toxic City (the new blueprint world)
- [ ] **Spawn on the dispatch plaza, on solid ground — NOT over the goo / not 10ft off.** `(SPAWN_AT)`
- [ ] A **taser + gravity gun spawn by the dispatch** (so you can fight without hauling one in). `(no guns = ❌)`
- [ ] **You can move around / "run" the city** *(the open question — confirm the input fix solved it; if you're physically blocked by walls/narrow streets, say so and I'll widen them)*.
- [ ] Walk the layout — boulevard + side streets read as a city you can loop.
- [ ] **Combat drones** (Market / Canal zones): they engage, but **don't shoot too often**, **aren't too fast**, and **DON'T fly through building walls** or **shoot through walls**. `(PVP_BOT_FIRE / DRONE)`  *(feel: tell me if still too fast/spammy)*
- [ ] Shoot a drone → it **downs with the right hit-zone reaction**. `(DRONE_DOWN)`
- [ ] **ObjectiveBoard reads cleanly** — "No active job. Use the kiosk to start." — **not the clipped "NOACTI"** garbage. *(overflow fix)*
- [ ] **No random vertical/stretched letters** on the travel-door labels.
- [ ] **Run the contract:** accept at the Dispatch kiosk → do the steps → on completion the **bounty pays** and flags set. `(JOB_REWARD_GRANTED, WORLD_FLAGS_GRANTED)`
- [ ] The **travel door** by the shipyard works (returns you out).

## 4. Economy wiring (Architect's, needs your headset)
- [ ] On **entering any world**, idle/welcome-back resolves. `(ECON_RESOLVE)`
- [ ] The bounty's **passage credits actually land in your profile** (persist across travel).

## 5. Travel-door story-gating (just shipped — no gated worlds yet, so confirm NO regression)
- [ ] All travel doors are **normal/unlocked** (nothing should be locked today). *(when a world gets `flagsRequired`, an unmet one shows a dark-red **locked** door, non-enterable, logging `TRAVEL_LOCKED` — not expected yet.)*

## 6. PvP Arena (solo vs bot)
- [ ] Warp to **PvP Arena**; fight the bot.
- [ ] **Bot spawns above the floor** (not sunk through it). *(spawn-Y fix)*
- [ ] **Taser = 3 hits** down the bot; **gravity = 6 hits + knockback**. `(PVP_BOT_HIT, PVP_KILL)`
- [ ] **Gravity gun gives YOU a short comfort self-hop + vignette** when fired (not a nausea launch). `(PVP_COMFORT_HOP)`  *(feel: hop distance ok?)*
- [ ] **HUD is small/readable** — "HP x/6   You n - n Bot" low in view — **not a giant `######` block**. *(HUD fix)*
- [ ] **Hammer is grabbable** *(was the collider-ordering bug)*; a **swing breaks walls** (intact → hole → pass-through), holes regenerate. `(PVP_WALL_BREAK)`
- [ ] **Wrist scanner:** cover the wrist with the other hand → it **charges** (lens brightens, haptics ramp) → fires a **PULSE**: shockwave + a **holographic radar above your wrist** + the bot tagged + an edge-of-vision chevron; then a cooldown. `(WRIST_SCAN_PULSE)`  *(feel: it's set further back on the forearm + bigger radar now — right position/size?)*
- [ ] Getting hit by a bot bolt: **brief screen flash + slow, no death**. `(PLAYER_STUN)`
- [ ] Best-of-10 scores and **rematches**. `(PVP_MATCH_END / PVP_REMATCH)`
- [ ] Bot **doesn't phase through arena walls or shoot through them**.

## 7. Starter World
- [ ] Warp to **Starter World**; the first area you walk toward **no longer has a gap you fall through**. *(safety base floor)*
- [ ] (It's still a basic graybox — expected; not a bug.)

## 8. Universal spawn / fall safety
- [ ] In **every** world you spawn **feet-on-ground, never in goo/void**. `(SPAWN_AT)`
- [ ] If you walk off an edge, the **fall-safety respawns you** (no infinite fall). `(FALL_SAFETY / FALL_SAFETY_RESPAWN)`

## 9. Perf / cleanup
- [ ] No obvious frame drops in the city / arena.
- [ ] No `debug-cb967f.log` junk accumulating in the app's storage. *(per-door logging was gated off)*

---

## Feel notes to send back (tuning, not pass/fail)
Ray length · drone speed & fire rate · wrist-scanner size/position · gravity-hop distance · gun grip
offset · combat pacing · city scale/walkability.

## Not testable (docs-only — nothing to do)
The story integration (`THE_TRANSMISSION.md`, bible reconciliation, chapter threading) is design only —
no runtime to test. Review it on screen when you want, not on the headset.
