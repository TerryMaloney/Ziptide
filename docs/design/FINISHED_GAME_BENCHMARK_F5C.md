# FINISHED-GAME BENCHMARK — third run (Fable 5 C-lane): the platform/audience layer + fresh verdicts
### SIBLING of canonical `FINISHED_GAME_BENCHMARK.md` (hwr33) and `FINISHED_GAME_BENCHMARK_RB.md` (rb56). Per the canonical §0 protocol this run ADDS rows and re-verifies; unique rows are merged into the canonical §5c. Kept for the multi-model record.

**Status:** RESEARCH — zero code. 2026-07-23, Terry-directed (multi-model, third run).
**Method:** three lanes — (1) fresh evidence audit of 18 completeness categories against the
2026-07-23 branch (every claim cites a path), (2) industry finished-game checklist research
(55 items, MUST/SHOULD/POLISH, sourced), (3) Meta Horizon Store + kids/COPPA certification
research (sourced). Where this run confirms hwr33's verdicts it says nothing; below is only
what's NEW or CHANGED.

---

## §1 — VERDICT UPDATES to the canonical tables (branch moved since hwr33's 07-20 greps)

| Canonical row | Old verdict | New verdict + evidence (2026-07-23) |
|---|---|---|
| 2.4 Localization decision | ❌ undecided | **✅ DECIDED** — `docs/design/LOCALIZATION_DECISION.md` "APPROVED Terry 2026-07-18: English-only at launch, structured for more" (+3 disciplines + font rider). `CURRENT_EXECUTION_CHECKLIST.md:71` still lists it open — stale row, fix the checklist. |
| 1.7 Save management | 🟠 | 🟠 unchanged, but **multi-profile is now PLANNED** (`docs/design/FAMILY_PROFILES.md` — the family/sibling need) — the save-slot UX should be designed once, against that spec, not twice. |
| 0.6/0.7 Store kit, IARC/privacy | ❌ no doc | **📋 now owned** — `docs/META_STORE_READINESS.md` exists (entitlement "NOT BUILT — needs Platform SDK + app ID", release keystore "NOT SET UP — debug-signed", privacy/IARC/DUC/store-assets unchecked boxes). Planning exists; zero executed. |
| 2.6 Photo/social | 🟠 | **✅ code HAVE** — working in-fiction Field Camera + saved album (`Gameplay/Runtime/Photo/*`, `Core/Runtime/Persistence/PhotoAlbum.cs`, profile schema v3). Export/share remains open. |
| 0.8 audio licensing | 📋 | 📋 — but the mandated **licensing ledger file still does not exist** (`CREDITS.md` required "at import time" by `META_STORE_READINESS.md` §2 + GAP_AUDIT; Photon is already imported at `Ziptide/Assets/Photon/`). The rule is already being violated — start the file NOW (one commit, freeze-safe). |
| (new fact) | — | `DEVICE_TEST_CHECKLIST.md` contains **zero** doff/Guardian/battery/interrupt rows even though `META_STORE_READINESS.md` §1 names them — the interrupt matrix exists as one unchecked sentence. Cheap fix: add the rows now so every future device session exercises them. |
| (new fact) | — | Storage-full/save-failure: `SAVE_FAIL` is logged as a warning and **silently swallowed** — no user-facing handling anywhere (matches MISS_LEDGER #3's error-fiction class; adds the ENGINEERING half: retry/free-space prompt policy). |

## §2 — NEW ROWS this run adds (not in hwr33 or rb56)

### 2a · The audience-certification chain (the big one for a FAMILY game)
Meta has **two separate declarations**: the IARC content rating (what's IN the game) AND an
**age-group self-certification** (who it's designed FOR): "Teens and Adults 13+" vs **"Mixed
Ages" (10–12 preteens + up)**. The chain nobody's store planning covers:
- **Preteen accounts are parent-managed and can only see/get age-appropriate apps → a family
  title NOT self-certified "Mixed Ages" is effectively INVISIBLE to the 10–12 audience we are
  building for.** This is not polish; it's market access.
- Mixed Ages obligations: **no advertising at all** · COPPA/GDPR-K compliance contractually on
  the developer · if any Platform SDK feature is used, the **Get Age Category API must be
  implemented within 30 days of self-certifying** (60 days → SDK features cut; noncompliance →
  store removal).
- COPPA practical rule (FTC enforcement is real — 2025 Apitor): persistent identifiers without
  parental consent ONLY under the internal-operations exception (crash, security,
  functionality). **No third-party analytics SDK with device IDs.** Our
  `PLAYTEST_AND_TELEMETRY.md` local-only stance is not just cautious — it is the compliant
  architecture; Meta dashboard crash/ANR is the sanctioned crash answer. The DUC + privacy
  policy must SAY what we do.
- **E vs E10 anchors:** Beat Saber/Walkabout = E; **Moss = E10 for sword-vs-beetles "Fantasy
  Violence."** Honest expectation: non-lethal drone-zapping is plausibly E only if targets stay
  clearly mechanical, effects abstract, combat not the relentless core; frequent combat tips it
  E10 (still fine for 10–12). Answer IARC honestly — misrating is a store-removal offense.

### 2b · VRC specifics with teeth (beyond the canonical Tier-0 rows)
- **The 4-second rule:** head-tracked graphics or a VR loading indicator within 4s of launch —
  black screen = cert fail. (Our async-travel crest hold is the right pattern; BOOT needs it too.)
- **Focus-aware rendering:** while the Universal Menu is up you must KEEP RENDERING, hide
  hands/controllers, and ignore input — not just "pause." (Input.4, a top first-time failure.)
- **Top actual first-time failures** (Meta's own common-failures list): text/taglines on cover
  art (Asset.2/.5) · not pausing on doff/menu (Functional.2) · no offline notice (Functional.7)
  · focus-awareness (Input.4) · missing entitlement (Security.1) · 72 FPS failing **under
  thermal load in release build** (devs test in-editor). Store art is a compliance surface —
  screenshots must be unretouched gameplay, no overlaid logos/UI.
- **The Meta VRC test plan is a downloadable spreadsheet and the `VRC Validator` tool pre-runs
  much of it.** Highest-value single action in this whole run: pull that test plan into our gate
  world — a `docs/VRC_TEST_PLAN.md` checklist now, VRC Validator in the release lane later. It
  is the ratchet law applied to cert: never discover a VRC in review.
- **App Lab is gone** (merged into Meta Horizon Store, Aug 2024): no curated-quality gate
  anymore — VRC compliance IS the whole gate; an "Early Access" badge replaces the soft-launch
  role. Also: **Quest 2 sunsets** (no feature updates past 2026; major titles shipping 3-only) —
  the device-target decision (Quest 3/3S primary, Quest 2 only if it holds 72) belongs on paper ⚖.

### 2c · The kid-body + kid-testing layer
- **Shorter-player mode is the canonical family feature** — Owlchemy's "Smaller Human Mode"
  (added because kids/seated players couldn't reach counters) is the named precedent; hwr33 row
  1.4 lists height recal as missing — this run adds: it's not one slider, it's a MODE (world-fit,
  reach ceilings ≤ short-arm reach — our PG-4 reach audit already encodes 0.35–1.9 m, i.e. the
  gate exists before the feature; rare and good).
- **Session chunking is a platform reality:** preteen accounts default to a 2h daily limit and
  Meta recommends breaks every ~30 min → the game must satisfy in 10–20 min chunks with
  save-anywhere/frequent checkpoints. (Our doff-autosave ✅ is half; the DESIGN half — "any
  session ends clean in ≤20 min" — should be a pacing law in the world genome.)
- **Kid playtest methodology** (for the beta phase): kids say yes to please adults — ask "is
  this for kids younger or older than you?" instead; observe, don't interview; 3-point picture
  scales; frame the kid as a helper; parents present not hovering. Sources: PlaytestCloud /
  Player Research / Joan Ganz Cooney toolkit / GDC 2025 "When Playtesters Are Children."
  `PLAYTEST_AND_TELEMETRY.md`'s kid-session protocol should absorb this list.

### 2d · Process rows (industry-checklist findings not yet on any board)
- **DECLARED milestones:** "the milestone most indie projects never formally declare is why
  they never finish." Alpha (feature-complete, no new features after) and Content Lock are
  *decisions with dates on a board*, not vibes. Proposal: add the milestone declarations as
  checklist rows the benchmark re-score keys off (rb56 §3 made the benchmark recurring; this
  gives it its trigger points).
- **Update-over-install is a standard cert/QA case we never test:** update an old install with
  old save data (not just fresh install) before every release — the classic post-launch
  corruption source. One runbook row + eventually a CI job pairing old-APK-save + new build
  (pairs with my save-corpus gate from `ONE_SHOT_BUILD_FRAMEWORK.md` §2.5).
- **Post-launch cadence, with data:** the family-title growth pattern is a **named content
  update every 4–8 weeks for year one** (Walkabout 6–8-week course drops for five years;
  Demeo free adventures; Little Cities named updates). Our world-pack architecture is built for
  exactly this — the live-ops one-pager (ledger #8) should promise a cadence the 80-world
  pipeline can actually feed.
- **Small-but-COMPLETE calibration:** Moss shipped 2.5 polished hours and was criticized for
  length, never completeness; Gorilla Tag's bare launch was priced (free) and framed (early
  access) accordingly. A paid family 1.0 = few worlds FINISHED + real ending + flawless
  comfort/options floor + visible "more coming" — supports hwr33 §5's "v1 ending = defined
  stopping beat," now with evidence.

## §3 — Reconciliation notes for the merge
- This run **confirms** (fresh greps, no change): pause/system menu ❌ · audio sliders ❌ ·
  player-facing difficulty/assist ❌ (GAP_AUDIT's "not a gap" deflection points at system docs,
  not a player surface) · colorblind ❌ · controls-remap deferred · version display ❌ ·
  credits roll ❌ · platform achievements ❌ (no Platform SDK at all).
- The canonical §5 SHIP SHELL lane + rb56's decisions/ledger machinery both stand; this run's
  §2 rows slot INTO them (2a→SS compliance envelopes + META_STORE_READINESS update; 2b→the
  VRC_TEST_PLAN checklist + release lane; 2c→comfort/accessibility + playtest docs; 2d→boards).
- Sources for every §2 claim are in the two research reports this run distilled (Meta VRC
  guidelines/common-failures/age-groups/parent-managed accounts, FTC/COPPA 2025 guidance, ESRB
  rating pages, Owlchemy accessibility statement, Road to VR comfort checklist, Mighty
  Coconut/Resolution/Purple Yonder launch histories).

## §4 — Recommended additions to the close-out order (extends canonical §5, doesn't reorder it)
1. **Now, freeze-safe, zero code:** create `CREDITS.md` licensing ledger (Photon backlog first)
   · add the interrupt rows to `DEVICE_TEST_CHECKLIST.md` · fix the stale localization row ·
   pull the VRC test plan into `docs/VRC_TEST_PLAN.md` · ⚖ device-target page (Quest 3/3S).
2. **With the SHIP SHELL lane:** the Mixed Ages decision + Get-Age-Category obligation goes ON
   the entitlement/Platform-SDK envelope (same SDK, same commit family); shorter-player MODE
   joins the comfort/settings mini-program (PG-4 already gates its reach law).
3. **At beta:** kid-playtest methodology into the playtest protocol; update-over-install into
   the RC smoke script; declared Alpha/Content-Lock rows onto the checklist.
