# THE FINISHED-GAME BENCHMARK (rb-lane sibling take) — ten categories, scored against our map
### SIBLING of the canonical `FINISHED_GAME_BENCHMARK.md` (hwr33, evidence-verdict form) — unique rows already merged into its §5b; this file kept for the multi-model record

**Status:** 🔵 RESEARCH — zero code (freeze). Terry, 2026-07-19: *"we're missing important parts
of what anyone would call a finished game — research benchmarks for a completed game and compare
to what we have."* Companion: `MISS_LEDGER.md` (new — every gap found here is logged under the
Class Law, §3).

---

## §1 — The benchmark: ten categories every finished game clears

Synthesized from industry milestone practice (first-playable → alpha/feature-complete →
content-complete → beta → cert → gold) plus ship-checklist practice. A game is FINISHED when
all ten are green — most studios track only the first four formally, which is exactly how the
last six get discovered late.

| # | Category | What it demands |
|---|---|---|
| **B1** | Feature complete | every mechanic/mode present and frozen; cut list explicit |
| **B2** | Content complete | all worlds/chapters/endings built; every "later" labeled DLC or cut |
| **B3** | Presentation complete | title, options, credits roll, legal/boot screens, loading states, every error a player can see handled in-fiction |
| **B4** | Quality bars met | bug severity bars, perf floor, soak/thermal, cert (VRC) pass, save integrity |
| **B5** | Player-lifecycle complete | onboarding → midgame → endgame → THE ENDING EXPERIENCE → completion state → replay answer (NG+/other-endings/sandbox) |
| **B6** | Compliance & legal | age rating, privacy, licenses/attributions, NAME CLEARANCE (trademark), platform T&Cs |
| **B7** | Store & marketing | store page, trailer, screenshots, description, press kit, launch calendar, wishlist/demo strategy |
| **B8** | Business decided | price, launch discount, regional pricing, monetization stance ON RECORD |
| **B9** | Live operation ready | patch pipeline, crash/issue triage route, support/community channel, review-response plan, update cadence promise |
| **B10** | Preservation & ops | keystore/backups, reproducible builds, source safety, version-migration path |

## §2 — The scorecard (honest, per category)

**B1 Feature complete — PLANNED-STRONG.** Deepest coverage in the project (weapons/enemies/
progression/physics/damage/staging docs). Open: game-mode variants (Terry's own flag), the cut
list as a formal artifact (we defer well; we never DECLARE cuts — a `CUT_LIST.md` costs a page
and ends scope-creep debates).
**B2 Content complete — PLANNED.** 80-world map, chapters, 4 endings, assembly line + genomes.
Open: the Earth kit (scheduled), DLC boundary is named (W069–80) ✅.
**B3 Presentation complete — GAPS FOUND.** Title menu now planned (rb50); options planned;
captions planned. **MISSING: the credits ROLL** (CREDITS.md is a licensing ledger, not the
player-facing credits experience — for this game it should be diegetic and personal: the
manifest of everyone/everything, RILL's last log); **MISSING: legal/boot screens** (health &
safety, licenses, studio identity — 30 seconds of cert-required real estate nobody designed);
**MISSING: the error-state fiction** (save-corrupt recovery prompt, missing-entitlement, out-of-
space — each needs ONE authored in-fiction line instead of a raw dialog).
**B4 Quality bars — PLANNED-STRONG.** The gate machine + PG program + store readiness + soak.
Open: a written bug-severity bar (what ships: zero S1/S2, ≤N S3 — one table, decided by Terry).
**B5 Player lifecycle — THE BIGGEST REAL GAP.** First hour: gold-plated. Endings: designed as
STORY. **MISSING: the ending as EXPERIENCE** — the roll-credits moment (what does the player DO
during credits in VR? sit in the ship? — a design opportunity most VR games waste), the
post-credits return (which save state? what does the world say?), the completion state (what
does 100% mean, what marks it — the trophy shelf exists, its CEREMONY doesn't), and the replay
answer (other endings: reload the W063 branch save? NG+? — one decision, currently nowhere).
**B6 Compliance — MOSTLY COVERED, ONE HOLE.** Store readiness covers rating/privacy/data.
**MISSING: name clearance** — "Ziptide" has never been trademark/store-collision searched; a
rename after store launch is catastrophic; a search costs an afternoon. DO THIS EARLY.
**B7 Store & marketing — HALF-COVERED.** DC-4 covers capture/trailer TOOLING. **MISSING: the
campaign** — store page copy, screenshot shot-list (the boot beauty shot feeds it),
30/60/90-second trailer beat sheets, press kit, launch calendar, wishlist-page timing, demo/
trial decision (a First-Hour demo is almost free given the Director's Cut). Planning-only doc,
zero code, high leverage.
**B8 Business — NOT ON RECORD.** Premium one-purchase is implied everywhere (no MTX anywhere in
the docs — good) but price point, discount posture, and the no-MTX stance are DECISIONS that
belong on paper before the store page exists. One ⚖ page.
**B9 Live operation — MISSING ENTIRELY.** Post-launch is nowhere: patch cadence promise,
crash-triage route (Meta dashboard → issue → lane), player-support channel (even just an
email + FAQ page), review-response policy, update roadmap communication. One doc, mostly
process; the pipeline half already exists (CI → store upload is 90% of a patch pipeline).
**B10 Preservation — MOSTLY COVERED.** Keystore law ✅, reproducible builds ✅ (golden/hash),
save migration ✅ (ProfileSerializer). Open: an off-site backup rule for the keystore +
LFS/asset sources (one runbook line).

## §3 — THE CLASS LAW (Terry's feedback law, formalized — supersedes nothing, generalizes hwr31)

hwr31's ratchet covers device-found BUGS ("every bug dies three deaths"). Terry's law tonight
extends it to EVERYTHING: *"if we find out we were missing something or something was broken,
that should be fed back to figure out why, and prevent that entire CLASS from happening again.
Any acute issue is a systemic issue."*

**The law:** every miss — bug, omission, planning blind spot, process failure — gets a
`MISS_LEDGER.md` entry with five mandatory fields:
1. **WHAT** was missing/broken (the acute issue);
2. **FOUND BY** (which activity caught it — headset, benchmark, review, CI, kid-test);
3. **WHY MISSED** — which layer of the system failed to see it (no genome slot? no gate? no
   checklist row? no law? nobody owned the category?). *This field is the whole point:* it
   names the SYSTEMIC hole, not the symptom;
4. **CLASS** — the general family this belongs to (stated so a stranger can recognize the next
   member);
5. **SYSTEM CHANGE** — the gate/checklist-row/genome-field/law/benchmark-category that now
   exists so the class cannot be silently skipped again. **A fix without a system change is a
   loan** (hwr31's phrase, now project-wide law).

**Working the law tonight (this document's own findings, logged as the ledger's seed entries):**
the benchmark exercise found ~10 misses; each is in `MISS_LEDGER.md` with WHY MISSED analyzed.
The meta-entry: *why did we miss whole categories?* → because our planning grew from the GAME
outward (mechanics→art→content) and nothing forced the SHIP-BACKWARD view (what does launch
day need?). The system change: **this benchmark becomes a recurring gate** — re-scored at every
milestone (checklist row in `CURRENT_EXECUTION_CHECKLIST.md`), so category-blindness can't
recur; and the EXCELLENCE_MAP gains rows for B3/B5/B7/B8/B9 so every category has an owner.

## §4 — Priorities from the scorecard (recommended order)

1. **Name clearance (B6)** — an afternoon, and every day of delay raises the cost of a bad
   answer. Do it during the freeze; it's not code.
2. **The decisions pages (B8 + B4's severity bar + B1's cut list)** — three ⚖ one-pagers.
3. **Ending-experience design (B5)** — a real design doc; the last hour deserves first-hour care.
4. **Marketing campaign doc (B7)** + demo decision.
5. **Live-ops one-pager (B9)** + B3's credits/legal/error-fiction designs.
6. All of it planning-lane, freeze-compatible; nothing touches the gate ladder.

## §5 — Multi-model merge note

Per `ONE_SHOT_FRAMEWORK.md` §8: takes merge by UNION of benchmark categories (any category any
model names gets scored or explicitly rejected), vocabulary must map to repo nouns, and the
merged benchmark supersedes this one and becomes the recurring gate of §3.
