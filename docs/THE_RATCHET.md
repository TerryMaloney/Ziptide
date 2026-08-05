# THE RATCHET — how a Unity AI pass makes the NEXT level better

**Stage:** 5 — The Assembly Line *(runs during Stage 6 batches; its checkpoint closes at Stage 5)*
**Type:** law
**Version 1.0 · created 2026-08-05 (Terry-approved)**

This law does not reorder `docs/PIPELINE.md`. It is the loop that turns each Unity AI pass into
permanent capability, so Stage 6's per-chapter batches start higher than the batch before. Its
amendment is recorded in PIPELINE §A at v1.1.

---

## The problem it solves

Terry, 2026-08-05:

> *"When Unity AI does a big change like this, I want to analyze what it did and see if we can
> reverse engineer it so that we can apply some of the techniques to the future levels. And then the
> best part is, then the Unity AI does more on the next level on top of our additions and
> hypothetically gets better."*

That instinct is right, and it needs one correction to actually work:

> **The model does not learn between sessions. The SUBSTRATE learns.**

Unity AI arrives with no memory of last time. What it *does* have is the project in front of it —
the ScriptableObjects, the builders, the menu commands, the tests that go red, and any file inside
`Assets/`. So a pass only compounds if the last pass left something **in the project** for it to
stand on. Every phase below exists to make sure it does.

This is stronger than betting on model improvement, because it survives model swaps — and this
project already rotates between Fable, Opus and Sonnet. A ratchet only turns one way.

---

## ⚠️ THE DISCOVERY THAT MAKES THIS WORK: `docs/` IS INVISIBLE

Unity AI's context is the Unity **project**, not the **repository**.

`docs/` sits beside `Ziptide/`, not inside `Ziptide/Assets/`. So THE LAWS, `EXCELLENCE_MAP.md`,
`WORLD_RECIPE.md`, `CLAUDE.md`, every runbook and every handoff entry — **Unity AI has never read a
word of any of it.** Every rule this project has written for its operators has been invisible to the
one operator that can actually open a scene.

That is why passes have not been compounding. It is also why they occasionally revert something they
had no way to know was load-bearing.

**The fix is one file: `Ziptide/Assets/Plans/_ZIPTIDE_HOUSE_RULES.md`.** It is inside `Assets/`, so
it is in context. It is the *only* prose channel that reaches Unity AI, so it stays short and it
holds rules, not history.

---

## THE CARRIERS — where a harvested technique must end up

Ranked by reach. Always push a technique as far up this table as it will go.

| # | Carrier | Reach | Cost to apply to a world already built |
|---|---|---|---|
| **C1** | **Builder / pure core** (`RingCityBuilder`, `BuildingBuilder`, `ShipHullBuilder`, `*Core.cs`) | Every future world **by construction**, and every past world **on re-bake** | ~zero — re-bake |
| **C2** | **Gate / EditMode test** (`Editor/Audit/*AuditRules.cs`, `Tests/EditMode/*`, `tools/*_gate.py`) | The floor cannot be re-crossed by anyone, human or model | ~zero — CI tells you |
| **C3** | **House rule** (`Assets/Plans/_ZIPTIDE_HOUSE_RULES.md`) | Read by Unity AI next session; obeyed, not enforced | paid per world, by hand |

Two things make C1 and C2 land in Unity AI's next pass without anyone reminding it:

- It already **prefers calling an existing builder** over hand-authoring geometry. Its entire
  ToxicCity pass was two booleans, five strings and one menu command. Improve the builder and it
  picks the improvement up for free.
- Its own plan template has a **"Verification & Testing"** section that names existing test files. A
  new gate therefore walks into its next plan by itself.

C3 is the weakest carrier and the only one that costs real money at retrofit time. **A harvest that
ends at C3 has not really been harvested** — it has been written down.

---

## THE FIVE PHASES

### R1 · FREEZE — record before you touch

Capture the change while it is still a mess. The mess is the data.

- `git diff --stat`, and object counts by classID for any scene (`m_Name` histogram beats reading YAML).
- Keep the AI's own plan file (`Assets/Plans/*.md`) **verbatim, in the commit**. It is the clearest
  statement of intent you will ever get, and it is how you tell a deliberate choice from a slip.
- Do not tidy, do not revert, do not "just fix that one thing" first.

### R2 · SORT — four buckets, no fifth

Every hunk goes in exactly one: **KEEP · FIX · REVERT · NOISE**.

> **Rule: noise must be NAMED.** Unnamed noise is how a physics setting rides into the repo on a
> scene bake. It happened on the very first harvest (see A-002). "Migration noise" is not a bucket;
> `m_AutoSyncTransforms: 0 → 1` is.

REVERT needs the same proof standard as anything else: before reverting, check whether the AI was
*fixing* something. On harvest 001 the exit-door change looked like a repair and was not — but that
took two minutes to establish, and guessing either way would have been wrong half the time.

### R3 · HARVEST — name the technique, not the change

Each KEEP becomes a row:

`T-NNN | technique | what it did here | why it generalises | carrier | gate`

> **Rule, borrowed from `EXCELLENCE_MAP.md`: a technique without a gate is a wish.**

A technique is not "it built a ring city." It is the *transferable move* — "positions expressed as
fractions of total length, so rescaling can never break proportion." If you cannot state it without
naming ToxicCity, you have not found it yet.

Record **anti-techniques** with the same care. What the AI did badly is worth as much as what it did
well, because an anti-technique converts directly into a gate, and a gate is C2.

### R4 · ENCODE — a technique is not harvested until it is code

Move every KEEP into a C1/C2/C3 carrier. A harvest is **CLOSED** only when every technique has a
carrier or an explicit *"declined, because…"*. An open harvest is a board row, not a memory.

### R5 · RE-BAKE LEDGER — what the past levels get

For each encoded technique, one line: **which worlds get it free on re-bake, which need authored
work, and what that authored work costs.**

---

## R5 IS THE ANSWER TO "GO BACK AND IMPROVE THE EARLY LEVELS"

`docs/GAME_PLAN.md` warns, correctly: *"retrofitting 80 worlds is how projects die."*

Terry's plan — get halfway, go back with what you have learned, then finish — dies on that warning
**only if improvements live in prose**. It survives on this one:

> ## You do not retrofit worlds. You improve the builder and re-bake.

Worlds are generated from specs by deterministic, seeded builders (LAW 1 spec-is-truth, LAW 2
pure-core-first). A technique pushed down into C1 improves **every world already built** the next
time it bakes, for approximately nothing. That is not a special mid-project campaign; it is Tuesday.

So the "go back halfway" plan is affordable **exactly to the extent that techniques reach C1**. That
is the whole bias of this document, and it is why the carrier table is ranked and why R5 exists to
make the C3 remainder a visible, budgeted number instead of an unbounded promise.

The same ledger is what lets accumulated technique reach multiplayer, arenas and the ship shell
later: by then the carriers are shared, so the question is only ever *which builder*.

---

## Doing a harvest

1. Copy `docs/ratchet/HARVEST_001_TOXIC_CITY_RING.md` as the shape. It is the worked reference.
2. New file `docs/ratchet/HARVEST_NNN_<slug>.md`, numbered in order, never renumbered.
3. Work R1 → R5 in order. Do not start encoding before sorting; that is how a REVERT gets harvested
   as a technique.
4. Close it per `OPERATOR_START_HERE.md`'s Definition of Done: board row, HANDOFF entry, and the
   `EXCELLENCE_MAP` row if an aspect's STATE moved.
5. Append any new C3 rules to `Assets/Plans/_ZIPTIDE_HOUSE_RULES.md` **in the same commit**. That
   file is the loop. If it does not get updated, the next pass starts where the last one started.

### Briefing Unity AI for the next pass

Its plan schema is good and we adopt it rather than fight it (T-007). Give it:

- a **Key Asset & Context** manifest — the exact asset paths and the exact menu command;
- steps that **flip data and call a builder**, never "place geometry";
- a **Verification** section naming the EditMode tests that must stay green;
- and the standing instruction to read `_ZIPTIDE_HOUSE_RULES.md` first.

---

## The harvest ledger

| # | Harvest | Date | Techniques | Anti | Status |
|---|---|---|---|---|---|
| 001 | [ToxicCity ring city + SLV-01 Scrapper](ratchet/HARVEST_001_TOXIC_CITY_RING.md) | 2026-08-05 | T-001…T-007 | A-001…A-003 | 🟡 open — awaiting device evidence |

---

## Amendment log

| Ver | Date | Change | Why | Approved |
|---|---|---|---|---|
| 1.0 | 2026-08-05 | Initial protocol: R1–R5, the C1/C2/C3 carrier ranking, and the `Assets/Plans/` channel | Terry: "analyze what it did… apply the techniques to future levels… then go back through the previous levels and improve them" | Terry (chat, 2026-08-05) |
