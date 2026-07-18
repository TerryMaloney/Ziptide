# GAP AUDIT — what a really good game needs that we haven't planned yet (July 2026)

**Status: 🔵 AUDIT + PROPOSALS — planning only, zero code.** Terry commissioned 2026-07-18:
*"let's think of other aspects of a really good game we may have glanced over or missed."*
Method: swept `EXCELLENCE_MAP.md`'s ~35 aspect rows, all of `docs/design/` (~45 docs),
`docs/systems/`, `docs/project_art_plan/`, and `META_STORE_READINESS.md`, then diffed against
what a shipped, family-audience, 80-world VR game requires. **The good news first:** coverage is
genuinely deep — haptics, comfort/accessibility, onboarding, combat health/death economy, store
certification (incl. entitlement + keystore + doff-mid-travel), photo/capture, progression,
first-hour, factions, captions, physics, damage — all have real plans. The list below is what's
LEFT, honestly ranked.

---

## GAP 1 — Localization discipline (decide NOW, even if we ship English-only)

**The finding:** every player-facing string in the game is a hardcoded C# literal — RILL/Cal
lines in `RillLineAuthor.cs`, signage text, job/objective text, UI labels. There is no string
table, no i18n doc, and no *decision* anywhere that says "English-only is the plan."
**Why it matters:** retrofitting localization after 80 worlds of authored text is one of the
most expensive migrations a shipped game can face — and the all-ages/family market is exactly
the market where localization multiplies reach. Worse, our TextMesh convention means each
language needs its own imported font coverage (CJK/Cyrillic glyph sets are a real constraint).
**What we do NOT need:** actual translation, or a heavyweight i18n framework, or changing a
single line today.
**The cheap fix (a discipline, not a system):** ① a one-page decision doc: target languages
(even if the answer is "English at launch, structured for more"); ② from now on, new
player-facing strings keep flowing through the AUTHOR files they already flow through (this is
accidentally our string table — the author pattern saved us); ③ one audit rule idea for later:
player-visible `TextMesh.text` assignments must trace to an author/library, not inline literals.
**Home:** new `docs/design/LOCALIZATION_DECISION.md` · **Priority: HIGH (decision), LOW (work)**
· paper-draftable now.

## GAP 2 — The SFX Forge (sound effects are half of VR presence, and only music has a plan)

**The finding:** music and ambience are well covered (`ADAPTIVE_AUDIO.md`, FORGE IV CP-8, the
ambient-audio excellence row). But **impact/interaction/foley SFX has no system**: what does
taser-on-chitin sound like vs. taser-on-steel? The DAMAGE program (rb44) explicitly requires
sound per response-matrix cell; weapon-feel docs assume punchy audio; VR sells weight through
sound more than through visuals — and nobody owns it.
**The shape of the fix (pairs 1:1 with existing systems):** an SFX vocabulary exactly like the
VFX one — a closed library (`impact_metal_soft/hard`, `impact_chitin`, `servo_strain`,
`steam_release`, `ui_confirm`…), each id referenced by the response matrix, ReactiveProp kinds,
weapons, and UI; per-material variation via the SAME material classes the damage matrix uses
(one taxonomy, three systems); pitch/volume jitter rails so 6 variants sound like 40; spatial
mixing rules (what ducks what) under the single `AudioDirector`. Booth-equivalent proof: an
"audio contact sheet" — a CI-rendered video/audio capture of the test alley firing every verb
at every material.
**Home:** `docs/design/SFX_FORGE.md`, executing beside FORGE IV CP-6/CP-8 · **Priority: HIGH**
— it's the missing half of the damage program's feel · cross-lane: audio + art + weapons.

## GAP 3 — Wayfinding at 80-world scale (per-world routes exist; the ACROSS-worlds layer doesn't)

**The finding:** inside a world we have cairn routes, objective boards, the ping system, and
world-flow templates. But nothing designs the cross-world layer: how a player (or a returning
kid who hasn't played in three weeks) answers "where was I? what was I doing? where should I go
next?" across dozens of worlds. The contract/job fiction gives us the diegetic answer — it just
was never specced.
**The shape of the fix:** a diegetic **contract ledger** on the ship (F3.7 signage + board
machinery — no floating quest log): active contracts, per-world status glyphs, "return to"
markers fed by save flags; RILL's existing FollowUp system doubles as the verbal layer
("We never finished on W005. The pump remembers."); gate room presents destinations as the
ledger's route, not a raw scene list. Zero new persistence — it RENDERS flags we already save.
**Home:** extend `docs/design/HOME_HUB.md` + a short ledger spec · **Priority: MEDIUM-HIGH**
(becomes CRITICAL past ~10 worlds) · mostly art/UI lane over existing data.

## GAP 4 — Family profiles: siblings share one headset (the family-game must-have nobody specced)

**The finding:** the save system has one `Profile`. The progression doc's family co-op ranking
(pass-and-play ghosts etc.) assumes multiple PLAYERS — but nothing designs multiple SAVES on one
device. Terry's kids are the acceptance bar for the whole game; kids sharing a Quest is the
single most predictable family usage pattern, and today player two overwrites player one.
**The shape of the fix:** 3–4 named save slots behind a diegetic pick (bunk tags in the ship
quarters — "whose bunk is this?" — not a menu); per-slot comfort presets (a 6-year-old's vignette
settings ≠ a parent's); slot-scoped once-latches (each kid gets their own first-time RILL
moments); a guest/sandbox slot that never touches story saves. Save integrity rails already
exist; this is slot plumbing + the pick UX.
**Home:** extend save-system design + `QUARTERS.md` · **Priority: HIGH for the family thesis**,
LOW urgency pre-launch · needs a save-lane claim.

## GAP 5 — Playtest protocol + privacy-safe telemetry (we tune by anecdote; kids deserve a method)

**The finding:** the factory plan names kid-testing as an acceptance bar, and Terry device-tests
constantly — but there's no *protocol* (what to watch, what to ask, how to log a session) and no
telemetry design. We already emit rich `ZIPTIDE:` logs; nobody specced turning them into
tuning evidence.
**The shape of the fix, deliberately tiny:** ① a one-page kid-session protocol (observe don't
coach; note where they stall, where they smile, where they take the headset off — the
takeoff-point IS the metric; no recordings of minors, notes only); ② an on-device, local-only
session summary written at exit from existing log counters (falls caught, deaths, time-to-first-
fun, comfort aborts, barks heard, worlds visited) — a file Terry reads after a session, nothing
transmitted, nothing personal — COPPA-clean by construction because it never leaves the device;
③ a `PLAYTEST_LOG.md` board where each session's three biggest stalls become issues.
**Home:** `docs/design/PLAYTEST_AND_TELEMETRY.md` · **Priority: MEDIUM, very cheap** · the
session-summary is a small post-freeze envelope over existing logs.

## GAP 6 — The resume moment (interruption is VR's most common event, and ours is mute)

**The finding:** store readiness covers doff/resume *technically* (no crash). Nobody designed
what resume FEELS like: a kid doffs mid-mission, dinner happens, the headset comes back on
tomorrow — the game must re-orient in five seconds. Related: session-end. VR sessions end
abruptly; nothing says goodbye.
**The shape of the fix:** RILL owns both moments with existing machinery: resume = one line
composed from saved state ("Toxic City. The relay job. You were winning.") via the line-library
pattern + a 2-second HUD-free status glance at the belt; a clean save-state indicator so parents
can call dinner without anxiety ("the game saves when the gate closes" — teach it once, in
fiction). Session-end: doff-detect already required for cert; add one autosave-on-doff rule +
RILL's next-session callback (FollowUp machinery, again).
**Home:** fold into `ONBOARDING_TUTORIAL.md` + caption/RILL line slots · **Priority: MEDIUM,
tiny cost, outsized family goodwill.**

## Smaller checks (verified, folded, or consciously deferred)

- **Photosensitivity pass:** the gate flash is a full-view luminance spike — add a
  photosensitivity review row to the comfort doc's checklist (flash duration/intensity bounds,
  an intensity option). One-line addition; do it when captions land.
- **Thermal/long-session behavior:** Quest throttles on long family sessions; CP-7's fidelity
  director is the designed answer — add thermal state to its §inputs list (one line, FORGE IV).
- **Returning-player recap:** GAP 3's ledger + GAP 6's resume line together ARE the "previously
  on Ziptide" — noted so nobody builds a third system.
- **Credits/licensing ledger:** fonts (caption v2 will import one!), audio sources, package
  licenses — add a row to `META_STORE_READINESS.md` §2 pointing at a simple `CREDITS.md` as
  things get imported. Cheap now, painful later.
- **Deliberately NOT gaps:** difficulty/kid-mode (combat health plan + encounters doc own it),
  death UX (same), settings surface (comfort doc's two-deliverable structure), map/minimap as
  UI (rejected — the ledger + cairns + RILL are our diegetic answer; a floating minimap fights
  the whole UI philosophy), achievements (progression doc's almanac/trophy shelf), NPC crowds
  (LS-4 ambient society is the deliberate, budget-honest answer).

## Priority picture (if Terry blesses the list)

1. **SFX Forge** (GAP 2) — the biggest feel-per-effort win; slots into the already-planned
   FORGE IV window beside the damage program.
2. **Localization decision** (GAP 1) — one page now prevents the worst migration later.
3. **Family profiles** (GAP 4) — the family thesis made real; needs the save-lane claim.
4. **Contract ledger wayfinding** (GAP 3) — before world count outruns memory.
5. **Playtest protocol + local telemetry** (GAP 5) — cheap, starts paying at the next kid test.
6. **Resume moment** (GAP 6) — smallest, ship with captions v2.

All of it behind the same gate ladder as everything else; items 1, 5's protocol page, and every
"shape of the fix" above are paper-draftable during the freeze.
