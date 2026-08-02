# 🎮 DEVICE CHECK — THE WHOLE OF LEVEL 1

**Built from the real contract**: `docs/first_hour/first_hour_beats.json` (22 beats) — not from memory.
Every beat below is a row. Walk it in order; the order IS the level.

Mark: **✅** works · **❌** broken · **🟡** works but feels wrong · **⬜** didn't reach it

> **🟡 is the most valuable mark in this file.** "It's there but it's wrong" is the finding no log,
> test or audit can produce — it is the entire reason a device pass exists. Don't round it up to ✅.
> 🆕 = new today and never device-tested.

---

## PHASE 1 · COLD BOOT — `_Boot`

| | # | Beat | What you should get |
|---|---|---|---|
| ⬜ | 1 | `FH_BOOT_READY` | A finished title surface. Not a developer menu. |
| ⬜ | 2 | `FH_NEW_GAME_SELECTED` | New Game creates/resets the profile and enters W000 |
| ⬜ | — | Sticks dead at the menu | Push them hard — rig must not move or turn (`BOOT_HOLD`) |
| ⬜ | — | One travel, one spawn | You land once. No respawn flash, no sinking |

**If phase 1 fails, stop and send the log — nothing below is meaningful.**

---

## PHASE 2 · WAKING UP — `W000_DriftIn`

| | # | Beat | What you should get |
|---|---|---|---|
| ⬜ | 3 | `FH_LOOK_AT_RILL` | RILL greets you only *after* you look at her light |
| ⬜ | 4 | `FH_COMFORT_CONSOLE` | First interaction: seated/standing + comfort presets |
| ⬜ | 5 | `FH_MOVE_IN_QUARTERS` | Short safe move inside the quarters. Comfortable? |
| ⬜ | 6 | `FH_GRAB_BUNK_OBJECT` | One named personal object makes "grab" obvious |
| ⬜ | 7 | `FH_HOLSTER_FIRST_ITEM` | Holstering is taught, and it sticks |
| ⬜ | 8 | `FH_INTERACT_HELM` | The first destination tile is the obvious thing to touch |
| ⬜ | 9 | `FH_FIRST_ZIPTIDE` | The jump earns the title. No forced camera motion |

**Known open item:** W000's own crane is still 10 m × 2 m — the stick defect fixed in ToxicCity. It
needs a placement done with the scene open. Note whether it bothers you.

> Notes on phase 2:

---

## PHASE 3 · 🆕 ARRIVAL AT W001 — *most of today's work is here*

| | # | Check | What you should get |
|---|---|---|---|
| ⬜ | 10 | `FH_W001_ARRIVAL` | Quiet vista beat. Return path visible **first**. Ambient life |
| ⬜ | 🆕 | **THE SKY** | Olive horizon, dark teal zenith, a ~22° occluded body |
| ⬜ | 🆕 | Horizon hazy, not a hard edge | |
| ⬜ | 🆕 | Something always drifting | acid motes |
| ⬜ | 🆕 | Sky colour reaches the ground | not a dome over a grey city |
| ⬜ | 🆕 | **Is the sky better than you remember?** | ❌ → revert `78babef9` |
| ⬜ | 🆕 | Junk on the berth deck, both flanks | ~9 crates/spools/toolboxes |
| ⬜ | 🆕 | Junk is **not** in your way | you can walk without bumping |
| ⬜ | 🆕 | **The loose crate lifts** | one crate, has weight, worth nothing |
| ⬜ | 🆕 | **Look up** — gantry arches overhead | 3 arches, trusses ~6.2 m |
| ⬜ | 🆕 | Seaward end is **open** | the sky must stay the backdrop |
| ⬜ | 🆕 | It does not feel like a lid | |
| ⬜ | 🆕 | **The crane is wide**, not a stick | 4.5 m, was 2 m |
| ⬜ | 🆕 | **Rungs legible** from the walk | 30 cm — they are the ruler |
| ⬜ | 🆕 | **The hook is moving** | creeps, ~30 s round trip |
| ⬜ | 🆕 | The hook looks heavy, not animated | eases at both ends |
| ⬜ | 🆕 | Nothing clips into anything | crane moved to clear the facades |
| ⬜ | 🆕 | **Lanterns start at the QUAY** | your first step already follows them |
| ⬜ | 🆕 | No two lanterns in one spot | |
| ⬜ | — | Berths 1–5 west of yours, empty | the ships that aren't coming back |

### ⭐ THE QUESTION THIS WHOLE PASS EXISTS FOR
**Standing in the yard: does it read as a real sixteen-metre crane in a real shipyard — or still like boxes?**
> _(be blunt)_

---

## PHASE 4 · THE SHIP AS ARMOURY 🆕

| | Check | What you should get |
|---|---|---|
| ⬜ | Rack aboard your ship holds weapons | drum/carousel, 8 slots |
| ⬜ | The carousel **turns** | cycles through what you own |
| ⬜ | You start owning taser + gravity gun | granted on first boot |
| ⬜ | **Can't leave the ship unarmed** | barrier holds you |
| ⬜ | RILL tells you *why* | `ARM_YOURSELF` / `BELT_IT` |
| ⬜ | Take one, belt it, barrier opens | |
| ⬜ | Empty rack + empty belt still opens | the no-trap law — don't get stuck |

> Notes:

---

## PHASE 5 · THE CONTRACT — `FH_ACCEPT_FIRST_JOB` → `FH_MACHINE_POWER_CYCLE`

| | # | Beat | What you should get |
|---|---|---|---|
| ⬜ | 11 | `FH_ACCEPT_FIRST_JOB` | Job source legible through world logic, not a modal |
| ⬜ | — | Dispatch interior is **furnished** | not an empty box (was, until recently) |
| ⬜ | 12 | `FH_SCAN_FAULT` | Scanning names the problem and shows the next physical step |
| ⬜ | 13 | `FH_REPAIR_ACCESS` | Repair taught by opening/exposing the service point |
| ⬜ | 14 | `FH_REPAIR_PART_SEATED` | Seating the part confirms itself. **Does it feel good?** |
| ⬜ | 15 | `FH_MACHINE_POWER_CYCLE` | Power-up has a visible/audible world consequence |
| ⬜ | — | The relay's red fault strobe is findable from Dispatch | the sightline triple |

**The coupler is the thing we just designed concept art for.** How does seating a part feel *right now*,
before any of that is modelled? That's the baseline we're improving on.
> 

---

## PHASE 6 · TOOLS & THE CREATURE

| | # | Beat | What you should get |
|---|---|---|---|
| ⬜ | 16 | `FH_SHOOT_PRACTICE_TARGET` | Safe target teaches firing **before** the creature |
| ⬜ | 17 | `FH_OBSERVE_SIGNATURE_CREATURE` | Foreshadow + a safe window to read it. No instant attack |
| ⬜ | 18 | `FH_COUNTER_SIGNATURE_CREATURE` | Readable counter, non-lethal resolution |
| ⬜ | 🆕 | **You can be hurt now** | armor vignette on damage — never device-tested |
| ⬜ | 🆕 | Damage feels fair, not cheap | creature 5 vs armor 6 — one hit is most of your armor |
| ⬜ | 🆕 | Death → respawn works and isn't punishing | |

> Notes:

---

## PHASE 7 · TRAVERSAL, REWARD, RETURN

| | # | Beat | What you should get |
|---|---|---|---|
| ⬜ | 19 | `FH_USE_JOB_ZIPLINE` | Zipline sits **on** the job path. No floating arrow needed |
| ⬜ | 20 | `FH_FIRST_JOB_REWARD` | Visual + audio + haptic + a persistent world change |
| ⬜ | 21 | `FH_RETURN_TO_SHIP` | The return path was shown at arrival and still works |
| ⬜ | 22 | `FH_CHANGED_SHIP_PAYOFF` | Ship/RILL visibly changed, save reassurance lands |
| ⬜ | — | Artifact halves join at the berth | two pieces, one per hand |
| ⬜ | — | The expedition outside the wall is reachable | half B lives there |

> Notes:

---

## PHASE 8 · ⚡ PERFORMANCE — judge this separately from the art

Materials went **333 → 111** today and static batching landed. Draw calls should be materially better.
Still 1769 renderers and 1.85× the material cap.

| | Check | Answer |
|---|---|---|
| ⬜ | Smooth walking the city? | |
| ⬜ | **Better / worse / same** vs last time | ⟶ **____________** |
| ⬜ | Any specific place it drops | ⟶ **____________** |
| ⬜ | Head-turning smooth (judder is the worst kind) | |

**If it's still rough the next lever is colour quantizing** — built in plan, not shipped, because it's
the first change that would alter a pixel. **Your call.**

---

## PHASE 9 · FREE TEXT — the most useful section in this file

**What felt worst?**
> 

**What felt best?**
> 

**Anything that made you stop and think "that's broken"?**
> 

**Did the hour hold together as an hour — or is it a list of tasks?**
> 

**If I could only fix ONE thing next:**
> 

---

### Paste back
1. this file, filled in
2. `Ziptide\Builds\morning_check_result.txt` (from `tools\morning_check.ps1`)
3. the raw log if anything crashed
