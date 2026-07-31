# TONIGHT'S TEST CARD — the whole first level, in order

**One window:**
```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\level1_test.ps1
```
bakes → builds → installs → launches → captures logcat. Press **ENTER** in it when you're done.

**Second window (optional, for evidence):**
```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\quest_capture.ps1
```
**S** = screenshot · **R** = record start/stop · **M** = mark the log ("that thing, right there") · **Q** = quit.

Everything lands in one folder: `Builds\session-<timestamp>\` — log, problems.txt, tags seen, shots, clips.

> Close the Unity editor before running. First run does a full bake + build: allow ~15–20 min.
> If it stops at the device check, accept the USB-debugging prompt in the headset and re-run.

---

## What should work, in the order you'll hit it

Status is honest: **✅ built + gate-proven** · **🟨 built, never rendered a frame** (first time on device tonight) · **❌ known missing** — don't report these.

| # | Beat | Do | Status | Proof tag |
|---|---|---|---|---|
| 1 | Cold boot → Home Hub → New Game | pick New Game | ✅ | `TRAVEL_OK` once, no `DUP_SINGLETON` |
| 2 | Wake on the ship: move, grab the keepsake, holster it | — | ✅ | `INVENTORY_SAVE`, no `XRI_NOT_READY` |
| 2b | The porthole (wall past the bunk) | walk to it | 🟨 | `PORTHOLE built …` |
| 2c | **THE ARMOURY** — rack by the hatch; take a weapon, put it on your hip | try DISEMBARK empty-handed first | 🟨 **NEW** | `ARMOURY_RACK built slots=8` · refusing = `SHIP_DISEMBARK_BLOCKED verdict=HoldUnarmed`, then `HoldInHandOnly` if you hold it without belting |
| 3 | Repair the gate coupler: scan → panel → part → power | the four steps | ✅ | repair lines; launch arms |
| 4 | Board, helm, **PUNCH IT** | press it | ✅ | `FLIGHT_LAUNCH` → `FLIGHT_STREAKS` → `VEIL leg=Ascent` → `FLIGHT_DEPART` |
| 5 | Space leg: fly the 5 rings | throttle/steer | ✅ | `RING_LIGHTS next=N/5`, `FLIGHT_RING n/5` |
| 5b | The sky: ringed giant, moon, sun, starfield | look around | 🟨 | *judge it, no tag* |
| 5c | Compass ribbon on the canopy bar | turn off-course | 🟨 | marker slides / reddens |
| 5d | **Fly too close to something** | approach a wreck | 🟨 **NEW** | `FLIGHT_BOUNDS kind=… level=…` — RILL warns *before* anything is taken from you |
| 6 | Wake a tender, shoot it, salvage it | aim cone | 🟨 | `DRONE_MOOD mood=Woken/Evading`, `FLIGHT_DISABLE`, `FLIGHT_SALVAGE` |
| 6b | **The tender's arms** — stowed → deployed → slack with the panel hanging open | watch it | 🟨 **NEW** | part of `DRONE_MOOD` |
| 7 | **THE FIND** — artifact half A on the wreck past the last ring | grab it | ✅ | `SALVAGE_FIND id=artifact_half_a` |
| 8 | Onward → Toxic City | travel station | ✅ | `TRAVEL_START dest=ToxicCity` → `REENTRY_ARRIVAL` → `VEIL leg=Reentry` |
| 8b | **THE AIR** — the city now has weather: a green haze band at the horizon and acid motes drifting past your face | stand still and look | 🟨 **NEW** | `WORLD_ATMO applied=1 hazard=acid` · `SKY_ATMO motes=…` |
| 9 | **The hangar walk** — five empty berths west of yours, numbers as tally bars | walk it | 🟨 **NEW** | `QUAY_BERTHS built=5` |
| 10 | Accept the contract at Dispatch | the board | ✅ | step advances |
| 11 | Clear 5 drones → walk to the relay | the route | ✅ | step advances |
| 12 | **Repair the relay** (this could not complete before tonight) | scan → part → power | 🟨 **FIXED TODAY** | step 4 advances — **if it doesn't, that's the big one** |
| 12b | The lantern route + the sightline triple from the plaza | look | 🟨 **NEW** | `WAYFINDING lanterns=…` |
| 13 | The Husk-Molter on the colonnade | observe, then counter | 🟨 | creature lines |
| 14 | The zipline over the canal | ride it | ✅ | `ZIPLINE_RIDE_START` → `_END reason=arrived` |
| 15 | **The expedition** — crawler out through the breach to the smoke column | drive | 🟨 | contract step 5 advances |
| 15b | **Half B** in the wreck → the resonance tell (your ride goes dark ~2 s) | grab it | 🟨 | `COLLECTED item=artifact_half_b` → `RESONANCE_TELL` |
| 15c | The boat: tide skiff on the ring canal, ride twice | drive | 🟨 | `SKIFF_WATER state=afloat/aground`; `STALKER stage=Shadow→Bump→Block` |
| 16 | Back to the berth; **JOIN** the halves (one per hand) | — | 🟨 | `ARTIFACT_JOINED` → beacon thread appears |
| 17 | Follow the thread, **seat the key** | — | 🟨 | `KEY_SEATED destination=W002_DryCistern` → `ZIPTIDE_ARMED` |
| 18 | **PUNCH IT → the first Ziptide** (pressing before seating must refuse) | press it | 🟨 | `FLIGHT_BLOCKED reason=no_key` before · `ZIPTIDE_GATE depart` after |
| 19 | Arrive W002; quit and Continue | — | ✅ | `TRAVEL_OK`, `INVENTORY_RESTORE` |
| 20 | Vehicles: crawler at the quay, hoverbike at Dispatch, skiff on the canal | board/drive/exit | 🟨 | vehicle lines |

## ❌ Not built — please don't report these
Pause/settings board · title + legal/credits · **all music and VO (zero)** · all final art (everything is procedural stand-in) · W002's defend wave / garden plot / glyph plate · helm `TextMesh` labels may read badly at Quest resolution (known).

**You CAN now be hurt — this is new today and it is the one thing most likely to need retuning.**
Armor is 6, it drains, breaking it is its own warning beat, and the next hit after that kills you (death
= respawn at the world's spawn marker with full armor and brief protection). Watch for `ZIPTIDE: ARMOR
outcome=…` and `ARMOR_DEATH`.

⚖ **Two first-pass numbers that only you can settle:**
- **Drone bolts do 1** — six to break your armor, a seventh to kill. Deliberately a sustained exchange
  rather than a burst-down, because burst-down is the wrong feeling in VR.
- **Creatures do their authored damage, default 5** — so two touches kill. Deliberately harsh. **If it
  plays punishing, the creature table is what moves, not the armor.**

If dying feels cheap, unfair, or disorienting in the headset, say so plainly — that verdict is the whole
point of this beat and no test can produce it.

**Interiors are still empty boxes.** Dispatch, the Shipyard Office and the Relay Vault are a floor, a
ceiling, four walls and one accent cube each. Known, not worth a line in your notes.

## The two that matter most
**Beat 12.** Until today the contract could not get past it — the pack spawned no machine with the id the step asks for, so steps 5, 6 and the whole back half were unreachable by playing. It's fixed and gate-guarded but has never run on device. If step 4 doesn't advance after the relay powers up, stop and send that log section first.

**Beat 2c.** The ship's ramp now refuses to let you leave unarmed. It is built so it can never trap you
— if there is no weapon anywhere to take, it opens — but that safety has only ever been proven by a
test, never by a person standing on the deck. **If DISEMBARK refuses when you ARE carrying a holstered
weapon, stop and send the `SHIP_DISEMBARK_BLOCKED` line.** That is the one failure here that would cost
you the rest of the run.

## Reporting
One line per defect: **what you did → what you saw → the tag line (or that it's missing)**. `problems.txt` in the session folder is pre-filtered for exceptions, `MISSING`, `_FAIL` and `BLOCKED`. Press **M** in the capture window the moment something looks wrong — it timestamps the log so the moment is findable.
