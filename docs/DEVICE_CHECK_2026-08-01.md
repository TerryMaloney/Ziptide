# 🎮 DEVICE CHECK — the 2026-08-01 Level 1 pass

**Fill this in with the headset on (or right after). Mark each row and paste the whole file back.**
`tools/morning_check.ps1` answers the log half automatically; **everything here is the half only your
eyes can answer.**

Mark: **✅** works · **❌** broken · **🟡** works but feels wrong · **⬜** didn't get to it

> A **🟡** is the most valuable answer in this document. "It's there but it's wrong" is the finding
> that never shows up in a log, a test or an audit — it is the entire reason a device pass exists.
> Don't smooth it into a ✅.

---

## A · Does it start at all *(if any of these fail, stop and send the log)*

| | Check | Expected |
|---|---|---|
| ⬜ | App launches, no crash on the menu | You reach the destination menu |
| ⬜ | Sticks are dead at the menu | Push them hard — the rig must not move or turn |
| ⬜ | One travel, one spawn | Pick a world; you land once, no respawn flash |
| ⬜ | Hands work | Both controllers tracked, rays present |

---

## B · 🆕 THE FIRST WALK — everything below is new today

### B1 · The berth deck, off the ramp

| | Check | Expected | If wrong |
|---|---|---|---|
| ⬜ | Junk is **there** | ~9 crates/spools/toolboxes down both flanks of your hull | absent → the approach author didn't run |
| ⬜ | Junk is **not in your way** | You can walk the deck without bumping anything | blocked → corridor width is one number |
| ⬜ | Junk is **close enough to notice** | You pass within a stride of it | too far → `MaxOffset` |
| ⬜ | **The loose crate is grabbable** | One crate lifts, has weight, drops. Worth nothing. | can't grab → static-batch exclusion failed |

**How does the deck read now vs. last time?**
> _(your words)_

### B2 · Look up — the gantry roof

| | Check | Expected | If wrong |
|---|---|---|---|
| ⬜ | There **is** something overhead | 3 arches + 2 runners over the landward half |
| ⬜ | You have to **look up** to take it in | Trusses at 6.2 m |
| ⬜ | **The sky is still there** | Seaward end open — horizon and haze unblocked | sealed → covered fraction |
| ⬜ | Lamps read as lamps | 3 amber, hanging at 4.6 m |
| ⬜ | It does **not** feel like a lid | | claustrophobic → raise or shorten |

### B3 · The crane — does the yard have a size now?

| | Check | Expected | If wrong |
|---|---|---|---|
| ⬜ | Crane is visibly **wide**, not a stick | 4.5 m, was 2 m |
| ⬜ | **Rungs are legible** from the walk | 30 cm apart — the whole trick | invisible → they're the ruler, say so |
| ⬜ | Walkway + cab read as person-sized | |
| ⬜ | **The hook is moving** | Creeps down and up, ~30 s round trip | still → static-batch exclusion failed |
| ⬜ | The hook looks **heavy**, not animated | Eases at both ends, never snaps | wrong → speed or easing |
| ⬜ | Cable stays attached at both ends | Stretches with the hook | detached → `CableSpan` |
| ⬜ | Nothing clips into anything | Crane moved to x=5 to clear the facades | clipping → placement |

**⭐ THE QUESTION THIS WHOLE PASS EXISTS FOR: standing in the yard, does it now read as a real
sixteen-metre crane in a real shipyard — or still like boxes?**
> _(your words — be blunt)_

### B4 · The lanterns

| | Check | Expected |
|---|---|---|
| ⬜ | Lanterns start **at the quay**, not at Dispatch | Your first step already follows them |
| ⬜ | Following them gets you to Dispatch | |
| ⬜ | No two lanterns in the same spot | Was a z-fighting pair on Dispatch |
| ⬜ | Unlit streets read as "not the job" | The other half of the grammar |

---

## C · ⚠️ THE SKY — nobody has ever seen this

**ToxicCity rendered a generic shared sky for its whole life. It now renders the one its own layout
has always authored: olive horizon, dark teal zenith, a 22° occluded body, with the acid haze in
front of it.** It bakes clean and the audit says the vista is attached — but no human has looked.

| | Check | Expected | If wrong |
|---|---|---|---|
| ⬜ | The sky **changed** from what you remember | | unchanged → the theme didn't bind |
| ⬜ | Horizon is hazy, not a hard edge | |
| ⬜ | There is a **body** in the sky | ~22° across, partly occluded |
| ⬜ | Something is always drifting | Acid motes |
| ⬜ | Sky colour reaches the ground | Not a dome floating over a grey city |
| ⬜ | **It looks better than before** | | ❌ → revert `78babef9` |

**Against `docs/systems/SKYSCAPE_DESIGN.md` — the Prospect bar — does this clear it?**
> _(your words)_

---

## D · ⚡ PERFORMANCE — the number that matters most

Materials went **333 → 111** and static batching landed, so draw calls should be materially better
than last time. Still 1.85× the material cap, and 1769 renderers.

| | Check | Expected |
|---|---|---|
| ⬜ | Frame rate **feels** smooth walking the city | |
| ⬜ | Better, worse, or the same as last time? | ⟶ **_______________** |
| ⬜ | Any specific place it drops | ⟶ **_______________** |
| ⬜ | Turning your head is smooth | Judder here is the worst kind |

**If it's still rough, the next lever is colour quantizing** — the last change that deletes nothing.
It's built in plan, not shipped, because it's the first thing that would alter a pixel. **Your call.**

---

## E · Everything else in the hour *(as far as you get)*

| | Check | Notes |
|---|---|---|
| ⬜ | Ship's armoury rack — weapons on it, carousel turns | |
| ⬜ | Can't leave the ship unarmed; RILL says why | |
| ⬜ | **You can be hurt now** — armor vignette on damage | New; never device-tested |
| ⬜ | Dispatch interior is furnished, not an empty box | |
| ⬜ | The relay repair completes | |
| ⬜ | Artifact halves join at the berth | |
| ⬜ | Creatures behave / can be dealt with | |

---

## F · Free text — the most useful section

**What felt worst?**
> 

**What felt best?**
> 

**Anything that made you stop and think "that's broken"?**
> 

**If you could only fix one thing next:**
> 

---

### Paste back
1. this file, filled in
2. `Ziptide\Builds\morning_check_result.txt` (from `tools\morning_check.ps1`)
3. the raw log if anything crashed
