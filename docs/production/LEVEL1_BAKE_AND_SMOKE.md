# LEVEL 1 — BAKE & SMOKE: the one-session runbook (bake it, build it, play it, report it)
### For Terry's next PC session. Goal: turn the committed generators into a playable level, play the whole route, and make any failure a ONE-LINE report instead of a lost day.

**Why this exists:** the playable scenes are bake-generated, not committed (rb124) — until this
batch runs there is no loadable level. And the last headset day died to setup + one opaque bug;
every step here therefore names its proof log so a break is localized in minutes.

---

## §1 — THE BAKE BATCH (Unity editor, in this order; ~15 min)

Open the project, let it compile clean (0 errors in Console), then run each menu item and check
its Console line before the next:

| # | Menu | Expect in Console | Produces |
|---|---|---|---|
| 1 | `Ziptide → First Hour → Author W000 Surfaces` | W000 authored/surfaces lines | `W000_DriftIn.unity` content (wake room, comfort, keepsake, adapters) |
| 2 | `Ziptide → Worlds → Build Toxic City` | `[Ziptide]` city build + audit lines | `ToxicCity.unity` populated (ring city, dispatch, zipline, **ReentryArrival**) |
| 3 | `Ziptide → Worlds → Build Toxic City Contract` | contract/job lines | the 5-step W001 contract |
| 4 | `Ziptide → Worlds → Build Space Lane (Flight Trial)` | `Space lane built: … (5 rings)` | `SpaceLane_Trial.unity` (dock, helm, rings, salvage drones, THE FIND, onward leg) |
| 5 | (if listed) `Ziptide → Worlds → Build W002 …` / Cavern | W002 lines | `W002_DryCistern.unity` |
| 6 | **File → Save Project**, then `git add -A` + commit + push | — | the baked scenes/assets land in the repo so the cloud lanes can finally see them |

**If a bake errors:** screenshot/copy the FIRST red Console line + the menu name. Don't debug —
send it; that one line is usually enough.

## §2 — BUILD + INSTALL

Per `TERRY_RUNBOOK.md`: `tools/ziptide_snapshot.ps1` (record the SHA) → `tools/dev_build_install.ps1`.
If the build reports `Scripts have compiler errors` → STOP, send the first error line + SHA.

## §3 — THE PLAY ROUTE (the whole level, in story order) + the log tag per beat

Run `adb logcat -s Unity` (or `tools/quest_smoke.ps1`) alongside. Each beat: what to DO → the
tag that proves it fired. A beat that fails = send its tag line (or its absence) + what you saw.

| # | Beat | Do | Proof tag |
|---|---|---|---|
| 1 | Cold boot → W000 | New Game | `TRAVEL_OK` (once), no `DUP_SINGLETON` |
| 2 | Wake, move, grab keepsake, holster | — | `INVENTORY_SAVE`, no `XRI_NOT_READY` |
| 3 | Repair the coupler (scan → panel → part → power) | machine steps | repair/`BELT_ENSURED` lines; launch arms |
| 4 | Board, helm → **PUNCH IT** | press it | `FLIGHT_LAUNCH` → `FLIGHT_STREAKS` → `FLIGHT_DEPART` (blocked = `FLIGHT_BLOCKED` — coupler not repaired) |
| 5 | Space leg: fly the 5 rings | throttle/steer | `LOCO_STATE`/flight lines; course progress |
| 6 | Disable + salvage the drones (optional/pacifist-skippable) | aim cone | `DRONE_DOWN` ×N |
| 7 | **THE FIND** — the artifact half on the wreck past the last ring | grab it | item pickup line (`ITEM_…`/inventory) |
| 8 | Onward leg → Toxic City | travel station | `TRAVEL_START dest=ToxicCity` → `TRAVEL_OK` **→ `REENTRY_ARRIVAL from=SpaceLane_Trial world=ToxicCity`** (NEW — proves the reentry owner) |
| 9 | Accept the contract; run all 5 steps | dispatch board | step advances; **any `JOB_MARKER_MISSING` = send immediately** (the level-locking class) |
| 10 | Half B from the Dockmaster; **JOIN** the halves (one per hand, they reach + snap) | — | artifact join line |
| 11 | Seat the key → the gate re-arms → **the FIRST ZIPTIDE** from the berth | — | gate/travel lines (full tide FX — this is the earned beat) |
| 12 | Return home; payoff + save | — | `INVENTORY_RESTORE`; relaunch → Continue restores |
| 13 | Vehicles (if placed): board, drive, exit | — | vehicle lines; note view/yaw feel (DV-06/07) |

## §4 — REPORTING (what turns a bug into a fix)

For each break, one line each: **beat # · what you did · what happened · the nearest `ZIPTIDE:`
log line (or "no tag fired") · SHA from §2.** Paste the batch; every item goes to a fix + a
`MISS_LEDGER` class so Level 2 can't inherit it. Feel notes (comfort, reach, scale, speed) are
first-class bugs too — that's what device days are FOR.

## §5 — Known-in-advance (don't burn time on these)

- **Art/SFX are Forge stand-ins everywhere** — logged for the Tripo pass (~6 days). Judge function
  and feel, not looks.
- The reentry beat is v1 seam (log + routing) — the plasma veil visual is queued art.
- W000 has no space viewport yet; the hangar walk + beacon thread are deferred LOOK problems (rb121).
- DV-05 (R3 crouch), DV-09 (buried-torso spawn report), DV-13 (perf lows) are open known device
  items — confirm/deny, don't diagnose.
