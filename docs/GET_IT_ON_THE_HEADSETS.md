# 📦 GET IT ON THE HEADSETS — Terry's install + two-player readiness page (2026-07-02)

**What this build contains (all CI-green, audit-clean):** the 5 arenas with the **MATCH BOARD**
(Deathmatch/Gun Game/KotH/Fragment/Horde × difficulty × 1–3 bots) + doors between every arena · the
**A4 arsenal** (Static Net / Sonic Thumper / Prism Beam on respawning pads, full 6-weapon Gun Game) ·
the smart bot (4 difficulties) · story worlds W000–W012 with canon skyscapes · ship + Quarters +
cosmetics. Smoke lists: `TERRY_RUNBOOK.md` §2b–§2i.

---

## A. Get the APK (pick ONE)

### Option 1 — download it (no PC build; recommended)
1. github.com → **TerryMaloney/Ziptide → Actions → CI**.
2. Open the **newest green run** on `terry-local-wip` (kind `workflow_dispatch` — the dispatched runs
   are the ones that build the APK; plain pushes only compile+test).
3. Scroll to **Artifacts** → download **`ziptide-apk`** → unzip → you have `Ziptide.apk`.

### Option 2 — build it yourself (PowerShell, ~10 min)
```powershell
cd C:\Ziptide
git checkout terry-local-wip
git pull
# sanity: branch/commit/scenes/last logs
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\ziptide_snapshot.ps1
# build AND install to the connected Quest in one shot:
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\dev_build_install.ps1
```
- Quest not plugged in? The script still builds — APK lands at `Ziptide\Builds\Android\Ziptide.apk`;
  plug in and run it again to install.
- Want the full checked pass (build + install + logcat scan for exceptions/audit fails):
  `powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\quest_smoke.ps1`

## B. Install on ONE headset
Headset in **developer mode**, USB cable, "Allow USB debugging" inside the headset, then either
Option 2 above (it installs), or by hand:
```powershell
adb devices                                   # headset shows as "device" (not "unauthorized")
adb install -r C:\path\to\Ziptide.apk
```
Launch from **Library → Unknown Sources → Ziptide**.

## C. Install on BOTH headsets (same APK — one build, two installs)
```powershell
adb devices          # both plugged in: two serials listed (or do them one at a time)
adb -s <SERIAL_1> install -r C:\path\to\Ziptide.apk
adb -s <SERIAL_2> install -r C:\path\to\Ziptide.apk
```
(SideQuest works too: drag the same `Ziptide.apk` onto each connected headset.)
Both headsets can now play EVERYTHING solo — story, all five arena modes vs bots, Horde together in
the same room taking turns. Two people, two headsets, one game each.

## D. Two-player ACROSS the headsets — exact state, no fluff
Two halves. **Yours is ready to do today; mine needs one more code sprint.**
- **✅ YOUR HALF, do anytime (~20 min, `docs/TWO_QUEST_SETUP.md` steps 1–4):** Photon account →
  App ID → import PUN2 into Unity → `Ziptide → Net → Enable Photon` menu → commit. Everything it
  needs is already in the repo (the network seam, the room-code launcher, the enable menu) and it
  cannot break the build — the adapter is inert until your menu click.
- **⬜ MY HALF (A6 — the FIRST task when usage resets):** stream the other player's head/hands +
  route hits over the wire (the seam is built and tested; this is the last mile). One sprint, one
  new APK, then: both headsets → same arena → same room code → real PvP.
Doing your half now means A6 ships as pure code with zero waiting on accounts/imports.

## E. If something's wrong on device
`adb logcat -s Unity` and look for `ZIPTIDE:` lines — every system logs its own tags
(`PVP_KILL`, `LOBBY_START`, `HORDE_WAVE`, `NET_ROOM_JOINED`…). Paste anything weird into the chat;
the tags are exactly what I diagnose from.
