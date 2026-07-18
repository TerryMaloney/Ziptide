# THE CONTRACT LEDGER — cross-world wayfinding without a quest log (GAP 3)

**Status:** 🔵 PLANNED — no code authorized (freeze). From `GAP_AUDIT_JULY2026.md` GAP 3,
commissioned 2026-07-18. **Design law inherited from the UI philosophy: no floating quest log,
no minimap — the answer to "where do I go?" must live in the world.** Renders EXISTING save
flags; adds no persistence.

## §1 — The problem, precisely

Per-world wayfinding is solved (cairn routes, objective boards, ping). The unsolved layer is
ACROSS worlds: "where was I? what's unfinished? what's next?" — trivial at 4 worlds, fatal at
40, and worst for the family pattern (a kid returning after three weeks). The fiction already
owns the answer: Cal is a CONTRACT technician. Contracts have paperwork. Paperwork lives on
the ship.

## §2 — The design: three surfaces, one truth (save flags)

- **The LEDGER (primary):** a physical board in the ship's ops space (board + F3.7 signage
  machinery — the DevWarpBoard interaction idiom, story-skinned). One row per known world:
  world glyph + name, contract status stamp (**OPEN / IN WORK / SETTLED / — NEW**, derived
  from `W0xx_COMPLETE`-family flags + job flags), and at most ONE line of "why go" text (from
  the world's data record — already authored in `WORLD_DATA.md` serialization). Grab a row →
  it becomes the gate room's highlighted destination (sets the same destination state the warp
  flow already uses). **Read order = chapter order**, so the board silently teaches progression.
- **The STAMP moment:** completing a world's contract stamps the row (audible `ledger_stamp`,
  SFX Forge id) next time you're aboard — a tiny ceremony that makes progress *physical*, and
  the LS-6 "return & memory" instinct on the ship itself.
- **RILL is the spoken layer (zero new tech):** the existing `FollowUp` machinery already
  fires "still thinking about it" lines gate-crossings later — point it at unfinished-world
  flags: *"We never settled W005. The pump will keep."* Cap: one nudge per session, never
  during a job (the anti-nag law — RILL reminds like a partner, not a to-do app).
- **The BELT glance (tertiary, later):** the wrist/belt UI's existing status surface gains
  the current contract's one-liner — the "mid-world amnesia" answer. Rides CP-9's window.

## §3 — Rules

- **One line of text per row, max.** The ledger is a table of stamps, not a journal. Lore
  lives in the worlds.
- **Derived, never authored twice:** every status is a pure function of flags; a
  `LEDGER_STATE_MISMATCH` test asserts ledger rows == flag truth so the board can never lie.
- **Kid-legible:** status is carried by stamp SHAPE + color family (colorblind-safe), name by
  glyph + text — readable pre-literacy, per the localization decision's symbols-first law.
- **New worlds appear as "— NEW" only when their gate unlocks** (flag-gated like everything);
  the board never spoils chapter-locked worlds (rows simply don't exist yet).
- **No teleport from the board.** It highlights the gate-room destination; travel stays
  physical through the gate (travel contract untouched).

## §4 — Envelopes (post-freeze; art/UI lane over existing data + one gate-room seam claim)

| Env | What | Acceptance |
|---|---|---|
| CL-1 | pure `LedgerModel` (flags → rows/status) + tests + mismatch audit | model == flags, always |
| CL-2 | the physical board (author-built in the ship scene path, board idiom, stamps) | device: readable at arm's length; a kid finds "what's next" unaided |
| CL-3 | row-grab → gate-room highlight (the one cross-lane seam: travel-adjacent, claim first) | round-trip test: grab → gate shows it → travel works unchanged |
| CL-4 | RILL FollowUp nudges (line content from the RILL/Cal line passes) + anti-nag cap | session cap test; verdict in-headset |

**Definition of done:** a returning player answers "where was I, what's next?" inside 30
seconds without reading anything longer than one line — using only the ship.
