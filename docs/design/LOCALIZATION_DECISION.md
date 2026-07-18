# LOCALIZATION — the one-page decision (GAP 1)

**Status: 🔵 PROPOSED DECISION — needs Terry's sign-off, then this page becomes law.**
From `GAP_AUDIT_JULY2026.md` GAP 1, commissioned 2026-07-18. Zero code now.

## The decision (recommended)

**English-only at launch, structured for more.** We do not translate anything today; we adopt
three cheap disciplines so that adding languages later is a content job, not a migration:

1. **All player-facing text flows through author/library files — already mostly true, now law.**
   `RillLineAuthor.cs`, signage sets, job/objective text, item names, UI labels: authored in
   central files (our accidental string table). New player-visible literals inline in runtime
   code are a review flag. This is the entire cost of the decision — a habit we largely have.
2. **IDs are the identity, text is data.** Every line/label already carries a stable id in the
   author pattern; keep it universal. A future language = the same ids with different text.
3. **Symbols beat words where possible** (already our instinct: glyphs, color families, diegetic
   icons). Every string we never write is a string we never translate — and kids who can't read
   yet benefit TODAY. This is the family-game double-win.

## Font reality (the technical constraint that forces the decision now)

The TextMesh convention means each language family needs an imported font with glyph coverage:
- Caption v2 already imports one Latin humanist sans (covers EN/FR/DE/ES/PT/IT + most of
  Latin-script localization if we ever go there).
- CJK / Cyrillic / Arabic each mean a new font import, larger atlases, and (for Arabic) shaping
  we cannot do in TextMesh — **explicitly out of scope unless the game finds a market there.**
- DECISION RIDER: any font imported from caption v2 onward gets a license line in `CREDITS.md`
  and must include the Latin-extended set (one import covers future European languages).

## What we deliberately do NOT do

- No i18n framework, string-table asset system, or locale plumbing today (a parallel toolchain
  with zero users — the standing rejection logic applies).
- No "translate the docs" ever — docs are dev-facing.
- No VO localization planning — VO itself isn't cast yet (`VOICE_PIPELINE.md`); subtitle-only
  localization is the affordable future path and our caption system is already the delivery
  vehicle (another reason captions v2 matters).

## Revisit triggers (when this page must be reopened)

- Store analytics/wishlists show a significant non-English market;
- a publisher/partner asks;
- the author-file discipline is found violated at scale (then we fix the leak, not the law).

**Sign-off:** ⚖ Terry: approve "English-only at launch, structured for more" + the three
disciplines + the font rider — or name target languages now and we spec the bigger path.
