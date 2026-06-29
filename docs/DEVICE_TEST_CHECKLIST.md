# Ziptide — Device Test Checklist (everything shipped, awaiting on-device verify)

**Why:** a lot of fixes/features landed CI-green (compile + EditMode) but **not headset-verified**. This is
the full pass for a real test session. Tags in `()` are the `ZIPTIDE:` logcat lines that confirm a thing
fired. Watch logcat with:
```
adb logcat | findstr "ZIPTIDE"
```
> **Logcat was blank last time** because `-s Unity` is too strict — if the player's log tag isn't exactly
> `Unity`, that filter hides everything. The command above drops `-s` and matches on `ZIPTIDE` (no colon),
> so it catches both the `ZIPTIDE:` lines and the `ZIPTIDE_DIAG` lines regardless of tag.

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

> **No manual "Rebuild Dev World Manifest" step anymore.** The build now rebuilds the manifest itself, as
> the LAST thing before the APK — *after* the D0→"D0 City (legacy)" rename — so the menu is always correct.
> (Rebuilding it by hand *before* the build is exactly what caused the two "Toxic City" entries. If you do
> click it manually it's harmless now; the build re-does it correctly.)

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
adb logcat | findstr "ZIPTIDE"
```

- [ ] Unity Console has **no red errors** before STEP 4 (only expected warnings are cosmetic:
  `frameH` unused, `AudioDirector._fadeTimer/_fading` unused — harmless).

---

## 1. Rig / locomotion / gun (the recurring trio — the big ones) ⭐ THE PERSISTENT BUG, ROOT-CAUSED THIS ROUND
- [ ] **Right thumbstick TURNS you** (snap/smooth) and does **NOT** move you forward/back. *(the #1 recurring bug)*
- [ ] **Left thumbstick moves you.**
- [ ] ⭐ **Right thumbstick does NOT rotate/translate the held gun OR hammer** while you turn (guns AND hammer).
- [ ] ⭐ **You can TURN while holding a gun in your RIGHT (dominant) hand** — last round the right-hand grab
  blocked turning (left-hand grab was fine). *(Cause: the XRI controller manager disabled Turn on grab; now it
  keeps Turn+Move live while holding, only teleport is suppressed.)* Test: grab gun **right hand**, push right
  stick → **you turn.**
- [ ] **Rays look like a realistic arm-length reach (~1.4 m)** and are **stable** — they should NOT stretch out
  to a gun/door when you point at it. *(line endpoint-snap disabled, length shortened)*
- [ ] **Grabbing a gun pulls it INTO your hand** — it should NOT stay floating out at the distance you grabbed
  it from. *(force-grab enabled)*
- [ ] Grab a gun → **snaps to a forward grip**; **release it → it FALLS** (doesn't freeze/float). `(WEAPON pickup)`
- [ ] Holster a gun → travel → pull it out → **release → it falls**. *(holster-travel gun-drop fix)*

## 1b. Credits / economy readout
- [ ] A **small** gold **"CR <number>"** readout sits in the **lower-LEFT corner** (not a big label near center). `(CREDITS_HUD_ENSURED)`
- [ ] After the ToxicCity bounty pays, the **number goes up** and **stays up across travel**. `(JOB_REWARD_GRANTED)`
  *(it stays at CR 0 until you actually complete a bounty — that's expected.)*

## 1c. Movement speed — REGRESSION CHECK (was getting stuck slow)
- [ ] **Walking speed stays NORMAL** the whole session — especially **after the PvP room** and **after getting
  hit by a drone/bot** (left thumbstick must not become permanently slow). *(stun no longer latches a reduced
  base speed; it self-heals to full the instant the stun clears.)* `(PLAYER_STUN appears, then speed returns)*

## 2. Dev menu / warp
- [ ] Boot lands in **Sandbox** (intentional dev bypass for now).
- [ ] **Y+B** opens the dev menu; it lists **Toxic City** (the new one), **PvP Arena**, Starter World, Sandbox.
- [ ] Only **ONE** "Toxic City" (the old D0 now shows as **"D0 City (legacy)"**). *(dedupe fix)*
- [ ] Warp works — and works **more than once / after closing & reopening** the menu. *(the old "clickable once" bug)*
- [ ] **"D0 City (legacy)" — the taser now SNAPS to a forward grip** when you grab it (was hanging at a weird
  angle). *(legacy gun routed to the same attach config as the ToxicCity guns)*

## 3. Toxic City (the new blueprint world)
- [ ] **Spawn on the dispatch plaza, on solid ground — NOT over the goo / not 10ft off.** `(SPAWN_AT)`
- [ ] A **taser + gravity gun spawn by the dispatch** (so you can fight without hauling one in). `(no guns = ❌)`
- [ ] **You can move around / "run" the city** *(the open question — confirm the input fix solved it; if you're physically blocked by walls/narrow streets, say so and I'll widen them)*.
- [ ] Walk the layout — boulevard + side streets read as a city you can loop.
- [ ] **Combat drones** (Market / Canal zones): they engage, but **don't shoot too often**, **aren't too fast**, and **DON'T fly through building walls** or **shoot through walls**. `(PVP_BOT_FIRE / DRONE)`  *(feel: tell me if still too fast/spammy)*
- [ ] **Drone feel — tuned this round:** they should read as **more alive** (orbit isn't a stiff fixed circle —
  they dart a little closer then back off, bob organically, a pack doesn't move in lockstep) and **slightly
  more capable** (a touch faster, fires a bit more often) — but still **fair, telegraphed, and NOT wall-phasing**.
  *(feel: too strong / too weak now? still collision-clean?)*
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
- [ ] ⭐ **The gravity gun does NOT damage/launch YOU when you fire it** (last round it "shot myself"). It
  should only hit the bot / drones. *(the muzzle ray now skips the wielder's own rig.)*
- [ ] **Gravity gun gives YOU a short comfort self-hop + vignette** when fired (intended, not a nausea launch). `(PVP_COMFORT_HOP)`  *(feel: hop distance ok?)*
- [ ] **The bot's shot is a VISIBLE, dodgeable orange bolt** — and it's **smaller now** (was too big). Step
  aside → it **misses**; walls **stop it**. `(PVP_BOT_FIRE)`
- [ ] ⭐ **The bot does NOT move through walls** — it should bump into them and go around. `(was phasing through)`
- [ ] **HUD sits low and readable** (a bit further below center this round), not in a corner / out of view.
- [ ] **Hammer walls:** now a **finer grid of smaller bricks**, and each brick takes **~4 swings** (more than
  before). A swing **chips the brick you hit** (it darkens) and opens a localized gap. `(PVP_WALL_HIT col=.. row=.. broke=..)`
- [ ] **Hammer turning:** right thumbstick **turns you, not the hammer**, and you **can turn while holding it**.
- [ ] ⭐ **Wrist scanner now WORKS:** cover the wrist with the other hand → it **charges** (lens brightens,
  haptics ramp) → fires a **PULSE** (shockwave + holo radar + bot tagged + edge chevron) → cooldown. Watch for
  `ZIPTIDE: WRIST_HANDS left=True right=True` then `WRIST_SCAN_PULSE`. *(last round it silently never fired
  because both "hands" resolved to the same controller; now resolved from the two controllers by side.)*
- [ ] Getting hit by a bot bolt: **brief screen flash + slow, no death** — and the slow **fully clears** (no
  stuck-slow afterward; see §1c). `(PLAYER_STUN)`
- [ ] Best-of-10 scores and **rematches**. `(PVP_MATCH_END / PVP_REMATCH)`
- [ ] Bot **doesn't shoot through walls**.

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
