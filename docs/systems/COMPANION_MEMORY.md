# Companion Memory — RILL brings things back up, unprompted (added 2026-07-06)

## 1. Purpose
Terry's prompt: "what's going to make this game really feel alive?" The honest answer isn't a new
visual system — it's that RILL has, until now, only ever *reacted*. Every line she's ever said fires
the moment something happens and never again. A companion who feels alive has her own throughline of
thought that keeps running whether or not the player's looking — she notices something, sits with it,
and brings it back up later, unprompted. That's the whole mechanic: **a countdown, in gate crossings,
between "RILL notices a flag" and "RILL says something about it, out of nowhere."**

## 2. How it works (shipped this pass, not just designed)
- `RillTrigger.FollowUp` (`Content/Runtime/Story/RillLineLibrary.cs`) — a fourth trigger alongside
  `WorldEnter`/`FlagSet`/`GateDeparture`. `key` = a flag RILL is watching for; `crossingsDelay` = how
  many gate departures after that flag is first noticed before the line fires on its own.
- `FollowUpTracker` (new, same folder) — pure C#, no Unity dependency, fully unit-tested
  (`FollowUpTrackerTests.cs`, 5 tests): `Register(line)` starts a countdown the moment a watched flag
  is first seen; `TickGateCrossing(ready)` decrements every pending countdown once per gate departure
  and returns whatever just hit zero.
- `RillCompanion.cs` wires it in two places: `PollFlags()` registers a countdown the instant a new
  flag it's watching appears (reusing the exact same "newly seen flag" diff the game already runs
  every second); `EnqueueGateLine()` ticks the tracker on **every** gate departure — unconditionally,
  before any of the method's other early returns — and enqueues whatever's now due into the same
  subtitle queue every other line uses. No new UI, no new delivery path — this rides the pipes that
  already exist.
- Authored in `RillLineAuthor.cs`'s new "COMPANION MEMORY" section via `FollowUp`/`CalFollowUp` helper
  functions (same shape as the existing `Flag`/`CalFlag` helpers). Three examples ship now, all keyed
  to flags already granted in the built game:
  - `C1_W004_RILL_ASKED_CARGO` → 9 crossings later: *"I never received an answer about the cargo. I
    have stopped expecting one. That is new, too."*
  - `C2_CONTAINMENT_REVEALED` → 7 crossings later: *"I have had time to think about the cage. I keep
    arriving at the same word: deliberate. I did not expect to end up there."* — and Cal answers it
    one crossing further out (8, not 7 — deliberately offset so RILL's line always resolves first,
    see the code comment in `RillLineAuthor.cs` for why the exact tie-order would otherwise be
    ambiguous): *"You've had 'a while' to think about a lot of things. I'm starting to worry you're
    outpacing me on all of them."*
  - `C3_W019_RILL_REFUSED` → 6 crossings later: *"You never asked me again why I refused. I have been
    waiting. I think I am relieved you didn't."*

## 3. Data + code
`RillTrigger.FollowUp`, `RillLine.crossingsDelay`, `FollowUpTracker`, `RillCompanion.RegisterFollowUps`/
the unconditional tick in `EnqueueGateLine`. Everything downstream (subtitle formatting, VO slotting,
the `speaker` field, the once-per-save latch) is the exact same machinery Cal's banter pass already
built — this only adds a new way for a line to become *due*, not a new way to deliver one.

## 4. VR feel
Nothing changes about how a line is delivered — it's still the same low-center subtitle, same orb.
The feel this creates is about *timing*, not presentation: the player is mid-flight through THE
ZIPTIDE, thinking about nothing in particular, and RILL says something that callbacks to a moment
several worlds behind them. That's the "alive" feeling — she was still turning it over.

## 5. Status
**Shipped, not just planned** — `RillTrigger.FollowUp`/`FollowUpTracker`/the three example lines are
real code on `terry-local-wip`, EditMode-tested. Known, documented limitation: **not persisted across
save/load.** `FollowUpTracker`'s pending countdowns live only in `RillCompanion`'s memory for the
current session — if the player quits between a flag being noticed and its countdown completing, that
follow-up is silently lost on reload (the flag will no longer read as "newly seen," so it never
re-registers). Given crossings-delays in the 6–9 range, this only matters if a player quits mid-session
at exactly the wrong moment; flagged honestly rather than silently accepted. Fixing it properly means
persisting `(flag, ticksRemaining)` pairs into `PlayerProfile` — a small, separate follow-up task, not
done here.

## 6. Open questions
- Should `crossingsDelay` scale with chapter (early game = short memory window since worlds come fast;
  late game = longer, since RILL's memory is fuller and the player's pace usually slows)? Not tuned
  for that yet — all three examples use hand-picked constants.
- Persisting pending countdowns (§5) — worth doing before this ships to more than the 3 examples above,
  or acceptable as a known gap for a while longer?

## 7. The bigger idea this is half of: "Aftermath" (pitched, not built)
Companion Memory is the half of this idea that was buildable in one pass because it reuses machinery
that already existed. The other half — **worlds that persistently show what Cal did there, not just a
flag that gates a line** — is a bigger, content-heavy lift and NOT started:
- A repaired world doesn't freeze the moment its job completes — it keeps a lifecycle. Visit a world
  Cal fixed in Chapter 2 again in Chapter 6 and it should look different: further reclaimed, more
  lived-in, evidence of the choice compounding rather than a flag silently sitting in a save file.
- Factions leave marks a player can SEE on revisit, not just remember from a completed quest log — a
  world Cal helped Sable in shows Sable presence later; a world Cal opposed them in shows the cost.
- `PlayerProfile.GetWorld(id, createIfMissing:true)` (`Core/Runtime/Persistence/PlayerProfile.cs`) already gives
  every world a persistent state bucket — the infrastructure for this half already exists too, it's
  just unused for anything but progression flags today. The lift here is content (prop-set variants
  per reclaim-level, per-faction dressing swaps), not a new architecture.
- Deliberately not scoped into this pass — it needs per-world content authoring, which is a bigger,
  slower-burn effort than a mechanism-plus-three-lines. Documented here so it doesn't get lost, and so
  whoever authors W013+ world content (M5) knows the hook (`GetWorld`) is already there to build on.
