# THE RESUME MOMENT — interruption is VR's most common event; ours should be graceful (GAP 6)

**Status:** 🔵 PLANNED — no code authorized (freeze). From `GAP_AUDIT_JULY2026.md` GAP 6,
2026-07-18. Smallest of the six gaps; ships naturally beside captions v2. Store cert already
requires doff/resume to be crash-safe (`META_STORE_READINESS.md` §1) — this doc makes it FEEL
designed instead of merely surviving.

## §1 — The three moments

**① Doff (headset off).** Rule: **doff = autosave** (the guarded `SaveSystem` autosave path —
never throws, already exists for travel). One log tag `ZIPTIDE: SESSION_DOFF`. Nothing visual —
the player is already gone. This also feeds the playtest takeoff-context counter (GAP 5).
**② Re-don, same session (dinner-length pause).** OS resumes the app; we add ONE beat: RILL's
caption, composed from live state — current world + active objective one-liner (the same
one-line data the contract ledger uses — one truth, three surfaces): *"Toxic City. The relay
job. You were winning."* No menus, no modal, play continues. Line pattern via the existing
library (a `Resume` trigger family), pool of ~6 phrasings + the ledger-derived clause so it
never goes stale.
**③ Next-day / next-week return (cold boot, CONTINUE).** After the bunk pick (family profiles),
RILL delivers the recap line — same composition, warmer register: *"Welcome back. We were on
W005 — the pump job. It kept."* Plus the FollowUp system's natural behavior (she's been
thinking) makes long absences feel HELD rather than paused. Target: re-oriented in ≤5 seconds,
zero reading required beyond one caption.

## §2 — Teach the save, once, in fiction

Parents need to trust "you can stop anytime." One line, first time a gate closes behind the
player (once-latch): RILL: *"Gate's closed — logged and saved. It saves every time, so leave
whenever life calls."* After that, the ledger stamp + doff-autosave make it true silently.
(Kid-parent detente is a FEATURE: "the game saves when you doff" ends every dinner argument.)

## §3 — Rules

- Resume lines follow every caption law (v2 spec) and the bark constitution's priority-yield —
  a resume line never talks over a story beat already playing.
- No resume line if the pause was <60 s (mask fidget ≠ interruption); no stacking with the
  world-enter line (resume suppresses it — one voice, one beat).
- The recap clause comes from data, not authored per world (80-world rule); worlds may
  override with one authored phrase in their record if the derived one reads poorly.
- Comfort: re-don never resumes mid-motion — if the player was in traversal/flight, they
  resume held at safe rest (locomotion owner's existing safe-state machinery; claim required).

## §4 — Envelope (post-freeze, one small pass)

Doff-autosave hook + `SESSION_DOFF/RESUME` tags + `Resume` trigger family + recap composition
(pure, tested: flags → clause) + the once-latch save-teach line. Cross-lane touches: doff hook
(platform/lifecycle owner), safe-rest resume (locomotion). Acceptance: device — doff mid-job,
re-don → one correct caption, play continues; cold boot next day → bunk → recap ≤5 s; the
<60 s fidget shows nothing.
