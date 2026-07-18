# FAMILY PROFILES — siblings share one headset without eating each other's saves (GAP 4)

**Status:** 🔵 PLANNED — no code authorized (freeze). From `GAP_AUDIT_JULY2026.md` GAP 4,
commissioned 2026-07-18. **Cross-lane: the save system is a locked-integrity lane — a named
save-lane claim is REQUIRED before any implementation.** This doc is the design half.

## §1 — Today's reality (audited)

`SaveSystem` (persistent, `_Boot`) owns ONE `PlayerProfile` at ONE `SavePath` (atomic write +
backup + migration via pure `ProfileSerializer` — good bones). Two kids on one Quest = the
second one either continues the first one's story or wipes it. For a family game whose
acceptance bar is literally Terry's kids, this is the deepest UX hole we have.

## §2 — The design

- **Slots:** 4 named slots + 1 guest. Data shape: today's save file, path-parameterized
  (`profile_slot<N>.json` beside the existing path; slot 0 = the current file, so **existing
  saves are untouched by construction** — migration is a rename-free no-op).
- **The diegetic pick — bunks, not menus:** the ship's quarters gains a bunk wall (existing
  quarters + signage machinery): each claimed slot is a bunk with a name tag (F3.7 letterforms)
  and small personalization (kid picks a tag color — `CosmeticDefinition` seam, trivially).
  Boot flow: after `_Boot`, the NEW GAME / CONTINUE moment becomes "whose bunk?" — point, grab
  your tag, go. First boot with one existing save shows that save as the already-claimed bunk.
  A kid can read a NAME TAG years before a menu.
- **Per-slot, always:** story flags/resources/marks (the whole `PlayerProfile`), comfort
  settings (a 6-year-old's vignette/snap-turn ≠ a parent's — comfort prefs move from global
  `PlayerPrefs` keys to slot-scoped keys, with the global value as the migration default),
  once-latches (each kid gets their own first-time RILL moments — this falls out free, the
  latches live IN the profile), caption options, equipped cosmetics.
- **Shared, deliberately:** device-level settings (volume, IPD-ish), unlocked SANDBOX/guest
  content, the photo gallery (family album is a feature, not a leak — ✅ Terry approved
  2026-07-18: gallery stays family-shared).
- **Guest slot:** full play, never writes story flags past a session, resets on exit — the
  "friend comes over" and "demo at a family party" answer, and it protects the kids' saves
  from enthusiastic visitors.
- **Deletion:** hold-to-confirm on the bunk tag + RILL asks once ("This clears the bunk. All
  of it. Sure?") + the existing backup file gives one accidental-deletion undo (surface it).

## §3 — What this must NOT touch

- `ProfileSerializer`/atomic-write/backup/migration internals — the integrity spine is proven;
  slots parameterize the PATH, nothing else.
- Travel/boot contracts: slot pick happens in the existing pre-NEW-GAME window, never
  mid-session; **no hot slot-switching** (doff → menu → other bunk is the flow).
- Multiplayer identity (paused lane) — when it wakes, slot = local identity input, claim then.

## §4 — Envelopes (post-freeze, after the save-lane claim)

| Env | What | Acceptance |
|---|---|---|
| FP-1 | path parameterization + slot registry + guest semantics (pure logic + serializer tests; slot-0 back-compat test is THE gate) | existing-save untouched proof |
| FP-2 | comfort/caption prefs → slot scope with global-default migration | pref round-trip per slot |
| FP-3 | bunk wall UX (quarters author + tags + pick flow + delete) | device: a kid picks their bunk unaided |
| FP-4 | RILL slot-awareness (greets by tag name; per-slot FollowUp clocks — already profile-scoped by construction) | verdicts in-headset |

**Definition of done:** two kids play on the same headset for a week and neither ever sees the
other's story state; a guest session leaves no trace; slot 0 users never noticed anything change.
