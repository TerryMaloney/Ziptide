# 🎯 THE ONE-SHOT BUILD FRAMEWORK — finishing this game clean, then cloning the bones

**Status:** DESIGN (research synthesis, Fable 5's take). No code changed by this doc.
**Merge note:** Terry is commissioning this study from MULTIPLE models and merging the results.
This is **Fable 5's version** — structured in numbered sections so other models' versions can be
diffed against it section-by-section. Where a claim is grounded in THIS repo's actual files it says
so; where it's industry research it names the source.

Three research lanes fed this: (A) an honest audit of our own gate coverage vs. every bug class
that has ever escaped, (B) industry research on near-one-shot correctness (databases, AAA studios,
AI-multi-agent practice), (C) a framework/game split inventory of the repo + how studios keep
reusable bones alive.

---

## 0. The core finding — the machine is mostly built; "one shot" is a promotion problem, then a physics problem

Two facts, both verified against the repo:

1. **We are unusually close already.** 155 EditMode test files, 14 audit-rule sets, python contract
   gates with their own unit tests, a patch-scenes-then-audit CI job, a durable CI verdict, and —
   the headline — **a headless PlayMode lane that already boots the real `_Boot` scene, runs the
   golden travel route (Home Hub → W000 → ToxicCity → W000), simulates rig controller ray-select,
   and round-trips travel saves.** It exists (`recovery-playmode.yml`, 43 PlayMode test files) but
   is **path-filtered to ~25 hardcoded files and non-blocking**. The single biggest lever toward
   one-shot is not new technology — it's promoting what's built.
2. **"One shot" has a hard floor, and the industry knows exactly where it is.** Logic and data can
   approach zero-defect (this is what FoundationDB/TigerBeetle-grade pipelines actually deliver).
   The irreducible ~10–20% is: the translator seam (pure sim ↔ Unity lifecycle timing), device
   behavior (Adreno shaders, thermals, tracking), and FEEL (comfort, grab, mix). The winning
   strategy — stated plainly by Rare's Sea of Thieves team — is not eliminating the human/device
   loop but **shrinking it until automation guarantees nothing is broken, so 100% of scarce headset
   time goes to feel.** That is the correct definition of "one shot" for us: **Terry's headset
   session becomes a 10-minute confirmation of a known checklist, never a discovery mechanism.**

The formula: **contracts as failing CI checks + fuzzed pure sim + blocking headless PlayMode +
a fixed device script.** Everything below is how to get there and how to clone it.

---

## 1. Where we honestly are (lane A inventory)

**Gated today (blocking):** compile · 155 EditMode suites (pure seeded cores, registries, the
WiringValidator "both-sides gate" on every data seam, save round-trips, event hygiene) · the
patch-scenes-and-audit CI job running 14 audit-rule sets (`report.Blocker` pattern).

**Gated today (WARN-only or non-blocking — the promotion backlog):** PerfBudget (static tris/
materials/lights caps, awaiting baseline) · WorldContent "nothing ships invisible" · CatalogBreadth
· UiReadability · the whole `continuity-report` python job · **the entire PlayMode lane.**

**Historical failure classes (16, from `CONNECTIONS_AND_RECOVERY.md`):** input death after travel,
silent git-pull aborts, travel-to-_Boot dup singletons, inventory duplication, `ITEM_DEF_NOT_FOUND`
on device, spawn collisions, state-machine bugs, wrong bindings, patcher-skipped-in-build, XRI feel
config, **CI license expiry (the safety net going blind)**, scene-YAML hand-edit corruption, stale
test content, shipped-invisible content, proof-level conflation ("done" claimed at SOURCE level),
first-frame shader hitch. Note: **at least 4 of the 16 (input death, travel-to-boot, inventory dup,
patcher-skipped) would have been caught by the PlayMode lane if it had been blocking.**

---

## 2. The top-10 missing gates (ranked by bugs-prevented × CI-feasibility)

1. **Promote the PlayMode lane to blocking, on every source push.** ~90% built. Follow the repo's
   own `recovery_playmode_promotion.py` criteria. The single biggest lever in this entire doc.
2. **Universal string-ID cross-reference gate** — statically resolve EVERY itemId/effectId/
   speciesId/packId/sceneName literal (source + SO YAML) against its registry + build settings.
   Kills the `ITEM_DEF_NOT_FOUND` class at commit time. Pure python; no Unity needed.
3. **Universal null-serialized-ref / missing-script audit rule** — one generic pass over every
   patched scene + prefab (current rules are per-feature). Kills the broken-prefab class.
4. **Golden route × every world** — headless `TravelTo` loop over ALL build-settings scenes:
   assert `TRAVEL_OK`, zero error logs, spawn clearance, clean runtime census per world.
5. **Committed old-save corpus** — freeze a real save per released build; CI loads each through
   current serializers. Round-trips prove today's format survives itself, not that yesterday's
   survives today.
6. **Perf promotion + GC-alloc gate** — flip PerfBudget caps to Blocker post-baseline (the file
   says to); assert allocs-per-frame ≤ N on the golden route (the #1 Quest perf killer).
7. **License-expiry early warning** — a weekly cron job exercising the Unity license, alerting
   days BEFORE expiry. Converts our #1 workflow catastrophe into scheduled maintenance.
8. **Android shader-variant compile gate + warm-up manifest** — attacks the open first-frame
   hitch at its CI-reachable edge.
9. **Ratchet the WARN-only gates to blockers** on a schedule — a WARN nobody reads is the
   Test-Day-1 invisible-gardens bug waiting to recur.
10. **Log-tag contract test** — assert every contract point still emits its documented `ZIPTIDE:`
    tag, so the diagnostic instrument itself can't rot.

---

## 3. What the industry proves (lane B, named sources)

- **Deterministic simulation testing** (FoundationDB → TigerBeetle's VOPR → Antithesis): abstract
  ALL non-determinism (clock, RNG, I/O) behind injectable seams, then fuzz the pure system with
  seeded random action sequences + per-tick invariants; a failure prints the seed for exact replay.
  TigerBeetle compresses "days of runtime per minute of test time." **Our pure/seeded core law
  means this transfers almost verbatim** — a "mini-VOPR" EditMode job fuzzing item×travel×belt×
  drone interactions is the only scalable answer to emergent-combination bugs.
- **AAA testing** (Rare/Sea of Thieves GDC 2019, ported to Minecraft GDC 2022; Riot; DICE;
  Ubisoft): the test pyramid from day one — unit → single-object "actor tests" → bot-driven
  integration. Riot's number: bugs caught in automation resolve **~8× faster**. The transferable
  core for a solo dev is NOT the bot farm — it's fast reliable tests as culture + structured
  telemetry from every run (our `ZIPTIDE:` tag grammar is exactly this pattern).
- **"Parse, don't validate"** (Alexis King) — wrap string IDs in validated value types constructed
  ONLY via registry lookup, so downstream code cannot hold an unresolvable ID. Converts our
  biggest device-discovered class into a compile/audit-time failure.
- **Spec-driven + AI-multi-agent practice** (Thoughtworks, Microsoft spec-kit, Augment "harness
  engineering"): *"agents write the code; linters write the law"* — every architectural invariant
  becomes a machine-checkable rule **with remediation instructions in the error message**, so the
  model self-corrects inside the loop. Human-refined specs cut error rates up to ~50%. Our LAWS +
  gates are the validated shape; the research says: push every prose contract into a failing check.
- **Model ensembles — the validated version of Terry's multi-model plan:** best-of-N only pays off
  when the selector is an **objective verifier** (tests pass, gates green), not an LLM judge
  (FLAMe: Pass@1 21%→31%; multi-agent pipelines 77%→95%+). Protocol: same spec + same test suite
  to N models, **merge whichever branch is green; judge models only break green-green ties.**
  Convergence across models comes from shared constraints + shared executable checks, in that order.

---

## 4. THE ONE-SHOT LADDER — the staged plan (Terry's "implement in sections, check, dial in")

Each rung makes the next section of the game buildable with fewer escapes. No rung requires the
previous game section to be finished — this ladder runs alongside content work.

- **Rung 1 — PROMOTE (highest value, near-zero new code):** gates #1, #7, #9 above. The PlayMode
  lane blocking on every push + license early-warning + WARN→Blocker ratchet.
- **Rung 2 — CLOSE THE DATA SEAMS:** gates #2, #3, #4, #5. After this rung, "the data is wrong"
  and "the scene is broken" are commit-time failures, not headset discoveries.
- **Rung 3 — FUZZ THE SIM:** the mini-VOPR (seeded action fuzzing over the pure cores with
  invariants: inventory never negative, holstered-only travel holds, travel idempotent, capture
  states legal) + parse-don't-validate ID types + FsCheck-style property tests. After this rung,
  emergent cross-feature bugs are farmed by CI, not by Terry's kids.
- **Rung 4 — PERF + RENDER EDGE:** gates #6, #8. CI owns regressions; the device owns absolutes.
- **Rung 5 — THE SECTION PROTOCOL (how every remaining game section ships):**
  design doc (done — the seven) → spec/data + pure core + tests FIRST → translator + patcher →
  gates green incl. PlayMode golden route → **a 🎮 runbook entry with concrete knob values** →
  Terry's 10-minute device script confirms → dial feel → next section. One section in flight per
  track; the blackboard already enforces the rest.
- **Rung 6 — ENSEMBLE-BY-VERIFIER (the multi-model protocol):** for a hard section, N models get
  the same design doc + same acceptance tests on parallel branches; CI green selects; a judge
  breaks ties; losers' good ideas get grafted in review. The blackboard's task-envelope format
  (`GOAL/INPUTS/ACCEPTANCE/BUDGET`) is already the right handoff unit — ACCEPTANCE just becomes
  executable.

---

## 5. THE CLONABLE BONES (lane C) — what the framework IS and how to extract it without killing it

**The split, measured against the actual asmdefs (~68.5k non-test lines):** roughly **45–55% of
runtime code is framework-shaped**, and **~100% of the process/CI layer** is reusable once
parameterized. The laws forced the seams — that's why this repo is harvestable at all.

**FRAMEWORK (the bones):** `_Boot`-persistent + additive-world contract · `TravelCoordinator` ·
rig persistence + holster-travel policy · SO-definition + string-ID factory/registry pattern (the
*classes*, never the assets) · pure-seeded-core discipline · `GamePool` · save spine
(ProfileSerializer's schemaVersion migration ladder) · economy spine (ResourceLedger/RewardRouter/
IdleEngine — already resource-agnostic by string ID) · diagnostics tag grammar · audit RUNNER +
CI verdict machinery + python gate kit · spec→compile→patch pipeline ENGINE · the docs blackboard
+ THE LAWS + DoD.

**GAME (the meat, never extracted):** all art/Forge recipes/palettes · balance numbers · worlds/
specs/packs/vistas · story flags/RILL lines · every string naming a Ziptide world, item, creature.

**Known leaks to fix (in-place, one-commit-sized, safe pre-ship):** `ZiptideFlags`, `RillState`,
`SignalState`, `TransmissionProgress`, `PlantGenetics` live in Core; `ZiptideConstants` mixes
framework constants with game scene names. Also on record: the asmdef reality is Core←Visuals←
Content/Gameplay (Visuals sits BELOW Content), not the CLAUDE.md ladder as stated — acyclic and
Core-rooted, so fine, but the doc should match reality.

**The shape (industry-validated):** THIN TEMPLATE, THICK PACKAGES.
- Six UPM packages, versioned + updatable: `com.studio.core` (pool/save/economy/diag — with XR
  deps removed from this layer) · `com.studio.boot` (persistent-boot + travel contract, no XR) ·
  `com.studio.xr-boot` (rig/comfort/holsters — non-VR games skip it) · `com.studio.definitions`
  (SO+registry base classes + convention-test helpers) · `com.studio.gates` (audit runner + build
  entry points + the python gate kit; `ci_verdict.py` is already fully parameterized) ·
  `com.studio.specgen` (the deterministic spec-pipeline engine; the city grammar stays game-side).
- A thin GitHub template repo: project skeleton, `_Boot` + one sample world, the CI 4-job graph,
  and **the docs skeleton** — CLAUDE.md / OPERATOR_START_HERE with THE LAWS / empty EXCELLENCE_MAP
  / HANDOFF / SPRINT boards / `{{HUMAN}}_RUNBOOK`, all with `{{GAME}}`/`{{DIAG_PREFIX}}`
  placeholders + a `PROCESS_VERSION` stamp. **The docs skeleton is arguably the most valuable
  single asset — it's what makes any capable model an operator.** (Templates rot because they
  have no update channel — hence thin template, thick packages; and a weekly CI job that
  instantiates the template and runs it, because a template whose CI isn't run is already dead.)

**The extraction ORDER (the part that keeps it alive — Fowler's "harvested framework" +
rule-of-three):**
1. **Now:** fix the Core leaks in place. Cheap, improves Ziptide directly.
2. **Ship Ziptide first.** Frameworks harvested from a shipped game live; frameworks built
   up-front die of speculative generality. ONE exception worth doing early: the docs skeleton +
   CI/gate kit (portable today, needs zero Unity refactoring).
3. **Game 2 is the forcing function:** extract each package only when game 2 actually pulls it,
   and Ziptide immediately re-pins to the extracted package — that back-reference is what keeps
   the framework honest.
4. Template repo LAST, assembled from what game 2's bring-up proved was needed on day one.

**Enforcement that the bones never break:** asmdef dependency direction (framework never
references game — Unity fails the compile) + a `framework_purity_gate.py` in the framework's own
CI (no game namespaces, no game scene-name literals, no tuning-shaped constants) + determinism
test helpers shipped WITH the seeded-generator bases. Gate-per-quality-dimension, applied to the
framework itself.

**Risk register (how these projects die → guardrail):** template rot → thin/thick + versioned +
weekly template CI · speculative generality → no API without two shipped consumers · single-game
assumptions → the boot/xr-boot split forces the audit; make game 2 a different genre shape ·
version drift → SemVer + both games consume the same package · process drift → LAWS changes are
framework PRs propagated to all games, never local edits · premature extraction → order above.

---

## 6. The honest device-only residue (what ALWAYS needs Terry)

Real Quest framerate/thermals (CI catches *regressions*, never absolute 72Hz) · Adreno shader
precision/banding · comfort/vection ratings · tracking, grab distances, haptic feel · readability
at real panel DPI · HMD audio mix · first-frame hitch *perception* · OS lifecycle (sleep/resume,
guardian, permissions, entitlements) · store submission. **The shape for all of it: the QUEST
proof level as a fixed, named, 10-minute per-build checklist** — confirmation, never discovery.

---

## 7. For the multi-model merge (how to combine this with the other models' takes)

- Diff by section: §2's ranked gates, §4's ladder, §5's package list + extraction order are the
  three places another model is most likely to differ — adjudicate those explicitly.
- Claims grounded in repo files (the PlayMode lane's existence + path filter, the 16 failure
  classes, the asmdef census, the Core leaks) are checkable — if another model's take contradicts
  them, re-verify against the files before merging.
- The two claims I'd defend hardest: **(1) promotion beats invention** (the biggest lever is
  making existing machinery blocking, not building new machinery), and **(2) merge-by-verifier
  beats merge-by-judge** — including for merging the very documents this note describes: where
  model takes conflict on a testable claim, the test decides.

---

## 8. Do-next shortlist (when Terry green-lights — still no code today)

1. Rung 1 wholesale: PlayMode lane → blocking · license cron · WARN→Blocker ratchet schedule.
2. The string-ID cross-reference gate (#2) — cheapest kill of our worst historical class.
3. Fix the five Core leaks (one commit each) — pays Ziptide now, unblocks extraction later.
4. The mini-VOPR skeleton over ONE pure core (travel+inventory invariants) to prove the pattern.
5. Draft the docs skeleton (`{{GAME}}` placeholders) — the first framework artifact, zero risk.
