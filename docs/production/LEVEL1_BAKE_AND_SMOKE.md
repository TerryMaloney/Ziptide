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
| 2 | `Ziptide → Worlds → Compile World Specs (docs-worldspecs)` | `SPEC_APPLIED ToxicCity` (a one-time `SPEC_DRIFT` **warning** after this first compile is formatting noise — run `Export All World Specs (JSON)` once and commit to silence it; a red `SPEC_REJECTED` is real, send it) | the layout/pack assets updated from `docs/worldspecs/ToxicCity.spec.json` — the expanded city (Quay + Dockmaster booth, Colonnade, Canal One) as data. **Must run BEFORE step 3** or the bake uses the old prototype layout |
| 3 | `Ziptide → Worlds → Build Toxic City` | `[Ziptide]` city build + audit lines | `ToxicCity.unity` populated (ring city, dispatch, quay, colonnade, **ReentryArrival**) |
| 4 | `Ziptide → Worlds → Build Toxic City Contract` | contract/job lines | the 5-step W001 contract |
| 5 | `Ziptide → Worlds → Build Space Lane (Flight Trial)` | `Space lane built: … (5 rings)` | `SpaceLane_Trial.unity` (dock, helm, rings, salvage drones, THE FIND, onward leg) |
| 6 | `Ziptide → Worlds → Generate All Layout Worlds` | W002 + other layout worlds generated and added to Build Settings | `W002_DryCistern.unity` — **the destination of the first Ziptide.** (The APK build does this for you via `WorldStubGenerator.EnsureGeneratedInBuildSettings`; run it by hand only if you are testing in the editor) |
| 7 | **File → Save Project**, then `git add -A` + commit + push | — | the baked scenes/assets land in the repo so the cloud lanes can finally see them |

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
| 2b | **NEW — look out the PORTHOLE** (wall past the bunk, standing eye height) | walk to it | `PORTHOLE built …` at scene start; judge whether the window sells "I'm on a ship in space" |
| 3 | Repair the coupler (scan → panel → part → power) | machine steps | repair/`BELT_ENSURED` lines; launch arms |
| 4 | Board, helm → **PUNCH IT** | press it | `FLIGHT_LAUNCH` → `FLIGHT_STREAKS` → **`VEIL leg=Ascent phase=start/Build/Peak`** → `FLIGHT_DEPART … gate=suppressed` (blocked = `FLIGHT_BLOCKED reason=unarmed`) |
| 5 | Space leg: fly the 5 rings | throttle/steer | **`RING_LIGHTS next=N/5`** each ring (next ring chases amber, passed go green); `FLIGHT_RING n/5` |
| 5b | **NEW — the sky.** Look around: ringed giant, sibling moon, sun, full starfield | — | no tag — judge it. Same bearing as the ground sky is intentional |
| 5c | **NEW — the compass ribbon** on the canopy bar: turn away from the course | — | marker slides / goes red when the ring is behind you |
| 6 | Fly at a drone (wake), shoot it, then fly close to salvage | aim cone | **`DRONE_MOOD … mood=Woken/Evading`**, `FLIGHT_DISABLE`, `FLIGHT_SALVAGE` + tractor beam + pluck |
| 7 | **THE FIND** — the artifact half on the wreck past the last ring, among drifting scrap | grab it | `SALVAGE_FIND id=artifact_half_a` |
| 8 | Onward leg → Toxic City | travel station | `TRAVEL_START dest=ToxicCity` → `TRAVEL_OK` → `REENTRY_ARRIVAL from=SpaceLane_Trial world=ToxicCity` **→ `VEIL leg=Reentry`** (the burn clears as you arrive) |
| 9 | Accept the contract; run steps 1–4 (dispatch → 5 drones → relay → repair) | dispatch board | step advances; **any `JOB_MARKER_MISSING` = send immediately** (the level-locking class) |
| 9b | **NEW — the Husk-Molter** meets you on the colonnade between Market Row and the relay | — | creature spawn/behaviour lines; judge the observation distance |
| 10 | **NEW — THE EXPEDITION (step 5).** Take the crawler from the quay, out through the sea-wall breach, follow the wall to the smoke column | drive it | `FLATS_SITE built …` at bake; contract step 5 advances at the wreck |
| 10b | **Grab half B** in the wreck's cargo cage → **the resonance tell** (your ride goes dark ~2 s) | grab it | `COLLECTED item=artifact_half_b` → **`RESONANCE_TELL lights=… instruments=…`** → `phase=recovered` |
| 10c | **NEW — THE BOAT.** Take the tide skiff on the ring canal; ride it twice, linger | drive it | **`SKIFF_WATER state=afloat/aground`** (try to drive onto land — it should nudge you back) · **`STALKER stage=Shadow` → `Bump` → `Block`** (ride 1 must be shadow only) |
| 11 | Return to the berth; **JOIN** the halves (one per hand, they reach + snap) | — | `ARTIFACT_JOINED at=…` → the **beacon thread** appears, pointing at your ship |
| 11b | Follow the thread to the hull; **seat the key** | — | `KEY_SEATED destination=W002_DryCistern` → `ZIPTIDE_ARMED` |
| 11c | **PUNCH IT → THE FIRST ZIPTIDE** (pressing it before seating must read "NO DESTINATION / seat the key") | press it | `FLIGHT_BLOCKED reason=no_key` before · `FLIGHT_DEPART … gate=full berth=…` + `ZIPTIDE_GATE depart` after |
| 12 | Arrive W002; payoff + save | — | `TRAVEL_OK`; `INVENTORY_RESTORE`; relaunch → Continue restores |
| 13 | Vehicles: board, drive, exit (crawler at the quay, hoverbike at Dispatch, skiff on the canal) | — | vehicle lines; note view/yaw feel (DV-06/07) |

## §4 — REPORTING (what turns a bug into a fix)

For each break, one line each: **beat # · what you did · what happened · the nearest `ZIPTIDE:`
log line (or "no tag fired") · SHA from §2.** Paste the batch; every item goes to a fix + a
`MISS_LEDGER` class so Level 2 can't inherit it. Feel notes (comfort, reach, scale, speed) are
first-class bugs too — that's what device days are FOR.

## §5 — Known-in-advance (don't burn time on these)

- **Art/SFX are Forge stand-ins everywhere** — logged for the Tripo pass (~6 days). Judge function
  and feel, not looks. Every visual added this session (ring lamps, plasma veils, porthole, smoke
  column, wreck, stalker, beacon thread) is a PROCEDURAL v1 by design: ugly is expected and
  tunable, missing is not.
- ~~The reentry beat is v1 seam~~ → **the plasma veil now plays on both ends** (`VEIL leg=…`).
- ~~W000 has no space viewport~~ → **the porthole is in** (beat 2b).
- ~~the beacon thread is a deferred LOOK problem~~ → **it's in** (beat 11).
- **The drive is ~270 m, not the ~700 m the spatial script claimed** — the city shell is only 190 m
  in radius. Judge whether the flats stretch feels like a journey at that length; if it needs to be
  longer the site moves outward, which is a one-line change.
- **The berth pads for berths 1–5 (the "hangar walk") are still NOT built** — the quay is a district
  with the Dockmaster's booth, not a row of neighbour ships.
- W002 is the arrival scene only: its defend wave, garden plot and glyph plate are **not built yet**.
- DV-05 (R3 crouch), DV-09 (buried-torso spawn report), DV-13 (perf lows) are open known device
  items — confirm/deny, don't diagnose.
