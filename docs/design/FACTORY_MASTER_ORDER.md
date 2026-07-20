# 🏭 THE FACTORY MASTER ORDER — merging every research into one accurate-AND-fast execution plan

**Status:** DESIGN (Fable 5 merge synthesis, 2026-07-20). No code changed by this doc.
**What this is:** Terry commissioned the "build the rest in one shot / clone the bones" study from
multiple operators. The takes are in, several are partially IMPLEMENTED, and this doc does the
merge Terry asked for: *"look at their results… fill in the blanks… accurate, large scope, but
also efficient and fast."* It reconciles the corpus, names the blanks nobody covered, and gives
ONE ordered plan with parallel lanes.

**The corpus merged here:**
- `ONE_SHOT_BUILD_FRAMEWORK.md` (Fable 5) — gate-coverage matrix vs the 16 historical failure
  classes · top-10 missing gates · one-shot ladder · ensemble-by-verifier · clonable-bones plan.
- `LLM_FIRST_BUILD_PIPELINE.md` (Architect) — the three structural faults (F1 create-only
  authors · F2 two sources of truth · F3 human-gated regeneration) · the RECIPE-HASH LAW · S1–S5.
- `PERCEPTUAL_GATE_PROGRAM.md` (Architect) — PG-1..6 spatial/perceptual gates · the RATCHET LAW ·
  the five-layer factory. **Status: PG-1..5 rule sets + EditMode tests are ALREADY LANDED**
  (`WeaponPerceptualAuditRules`, `BuildProfileTravelAuditRules`, `RouteContinuityAuditRules`,
  `InteractionReachAuditRules`, `PerceptualCoverageAuditRules` — verified on branch 2026-07-20).
- `WORLD_IMPROVEMENT_FRAMEWORK.md` (GPT) — manifest→module→deterministic-compiler rounds with its
  own hash stamping. **Status: implemented for Round 2, live on branch.**
- `WORLD_ASSEMBLY_READINESS.md` (Architect) — the six content-factory gaps A–F (ride-scenes,
  WorldContentGenome, Lore Forge, flag validator, voice kits, the assembly-line runbook).

---

## 0. The merge verdict — the models CONVERGED, so treat the shape as settled

Independently, every take arrived at the same five-layer factory: **spec-is-truth ·
deterministic generators under a hash law · a gate per quality dimension · the evidence
blackboard · the ratchet.** Multi-model agreement reached without coordination is exactly the
signal ensemble research says to trust. **The architecture debate is over; everything below is
sequencing and gaps.** Where takes differed, the differences are complementary, not conflicting:
the Architect attacked the *pipeline* (staleness), I attacked *coverage* (which bug classes have
no gate) and *the clone* (extraction), GPT built the *world-quality loop*, and Assembly Readiness
mapped the *content* half. No testable claim in one doc contradicts another — merge is additive.

---

## 1. The blanks — what NONE of the researches covered (fill these)

1. **PlayMode promotion is still nobody's work order.** My study's #1 lever — the headless lane
   that boots `_Boot` and runs the golden travel route already EXISTS but stays path-filtered and
   non-blocking — appears in no implementation queue. It would have caught 4 of the 16 historical
   failure classes. → Add to the order (Phase 1, below).
2. **Two hash laws are about to exist.** GPT's WorldImprovement compiler stamps SHA-256(manifest
   + compiler version); the Architect's S1 will stamp recipe hashes on the older `*Library`/
   `*Author` set. If these grow separately we get two staleness dialects. → ONE shared helper
   (hash format, stamp field, `ASSET_STALE_VS_RECIPE` blocker) — S1 should consume the
   WorldImprovement implementation, not parallel it.
3. **The universal string-ID cross-reference gate** (every itemId/effectId/packId/sceneName
   literal resolves against its registry — kills `ITEM_DEF_NOT_FOUND` at commit time) is in my
   top-10 but no one's queue. Cheap, pure python. → Phase 2.
4. **Emergent-interaction fuzzing** (the mini-VOPR: seeded random action sequences over the pure
   cores with per-tick invariants) — the only scalable answer to cross-feature bugs; nobody
   else's program touches it. → Phase 2.
5. **The save-corpus migration gate** and **license-expiry early-warning cron** — still unowned;
   the license one converts our #1 workflow catastrophe into scheduled maintenance. → Phase 1
   (cron is ~an hour of work).
6. **CI THROUGHPUT — the speed blank nobody studied.** Every push runs full Unity jobs; with 3–5
   operators pushing, runs queue and rebase churn compounds (this session alone hit 3 push
   rejections and 1 merge conflict). No `concurrency:` groups exist in `ci.yml`, so superseded
   pushes waste the queue. → Phase 1 throughput pack (§4).
7. **The operator-collision tax.** File ownership exists on the boards, but HANDOFF is a single
   contended file (the one conflict every operator hits). → Small fix: per-operator append
   discipline (one entry block per push, always at top — rebase-friendly), and batch doc pushes.

---

## 2. The speed thesis — the accuracy machinery IS the speed machinery

The question "accurate OR fast" is a false trade here, because our slowness is almost entirely
**round-trip cost**, and every round-trip is a missing gate or a human in a machine's seat:

| Loop | Cost today | What removes it |
|---|---|---|
| Bug discovered on headset | **days** (wait for Terry's session, diagnose from logcat, re-fix, re-build) | a gate (PG program + Phase-2 gates) — turns it into a red X in ~30 min |
| Stale asset needs Terry's bake sitting | **days** (runbook queue grows until a PC session) | the hash law S1/S2 — recipe edits propagate on push, ZERO human steps |
| CI verdict on a change | ~30 min | throughput pack: concurrency-cancel superseded runs, cache health, batched pushes |
| EditMode red | minutes | already optimal — this is why pure-core-first is the fastest lane we have |
| Operator collision / rebase churn | minutes–hours, compounding | §1.7 discipline + lane ownership already on the boards |

**Therefore: the fastest possible path to a large game runs THROUGH the gates, not around
them.** Every gate added deletes future device round-trips (the ratchet guarantees a class never
costs a second trip); the hash law deletes the human from every content iteration; the factory
turns world production from craftwork (serial, human-paced) into a pipeline (parallel,
machine-paced, human only at the feel verdict). That is how "accurate" and "fast" become the
same program.

**The scale math that justifies it:** at 80 worlds, a per-world quality floor enforced by gates
costs one CI run per world; the same floor enforced by human inspection costs 80 headset
sessions. The factory is not overhead — it IS the schedule.

---

## 3. THE MASTER ORDER — phases with parallel lanes

Rule: phases overlap across OPERATORS (lanes are parallel) but each lane works its phase order.
Everything routes through the existing boards + envelope format; acceptance is executable
wherever possible (ensemble-by-verifier: green decides, judges only break green-green ties).

### PHASE 1 — UNSTICK (remove the two humans-in-machine-seats + the queue tax)
*The wall-clock winners. Everything else compounds faster once these land.*
- **S1+S2 recipe-hash law** on the create-only author set + the bot-commit bake workflow
  (Architect's spec; GPT executing; MUST reuse the WorldImprovement hash helper per §1.2).
  Deletes the Terry-bake bottleneck from every future content change.
- **PlayMode lane → blocking on every source push** (per the repo's own promotion criteria).
  Deletes the biggest class of device round-trips.
- **Throughput pack:** `concurrency:` group per branch with cancel-in-progress on superseded
  pushes · license-expiry weekly cron with early warning · HANDOFF append discipline note in
  OPERATOR_START_HERE · operators batch doc-only pushes.
- Already landing in parallel: **PG-1..5 wiring completion + WARN→Blocker ratchet schedule.**

### PHASE 2 — SEAL THE SEAMS (make "the data is wrong" a commit-time failure)
- Universal string-ID cross-reference gate (python, no Unity).
- Universal null-serialized-ref / missing-script audit rule.
- Golden route × EVERY build-settings world (extends the now-blocking PlayMode lane).
- Save-corpus migration gate (freeze one real save per released build).
- Mini-VOPR v1 over ONE pure core pair (travel+inventory invariants, seeded, replayable).
- Perf caps WARN→Blocker post-baseline + GC-alloc budget on the golden route.

### PHASE 3 — SCALE THE CONTENT FACTORY (Assembly Readiness gaps, cheapest-protection first)
- **GAP D flag-graph validator** (pure data, protects everything — first).
- **GAP B WorldContentGenome** (the 80-world content bottleneck-breaker).
- **GAP E voice kits + GAP C Lore Forge** (slots + validation; prose stays human-reviewed).
- **GAP A ride-scenes** (comfort-legal cinematics; report-only flags apply).
- GPT's improvement rounds keep running against worlds as they exist (already live).

### PHASE 4 — THE ASSEMBLY LINE (the "builds itself" capstone)
- **GAP F: the assembly-line runbook** — written LAST, from experience: "World N from bible row
  to device pass," each step naming its framework + gate. Success sentence: one session walks
  one standard world through steps 1–5 in one sitting.
- Then worlds PIPELINE: multiple sessions each carrying a world through the line concurrently
  (ownership per world = no collisions), Terry's headset doing 10-minute feel verdicts only.
- The seven gameplay design docs (weapons feel → powers → ships → enemies → progression) enter
  as SECTIONS through the same protocol: spec → pure core + tests → translator → gates →
  runbook entry → device feel pass. Firing range (GPT lane) is the natural first section after
  Terry's headset test.

### CONTINUOUS — THE CLONE (extraction, per the harvested-framework law)
- Now: fix the five Core leaks (story code in Core, ZiptideConstants split) — one commit each.
- Docs skeleton + CI/gate kit templatization can start any time (zero Unity risk).
- UPM package extraction WAITS for game 2 (rule of three). The WorldImprovement framework's §5
  porting list is the first proven slice of the eventual template.

---

## 4. Owner map (who runs which lane — matches existing claims)

| Lane | Owner | Current order |
|---|---|---|
| Hash law S1/S2 + PG wiring + WorldImprovement rounds | GPT | PG-2→1→5→3→4 done/landing → S1 → S2 |
| PlayMode promotion + throughput pack + Phase-2 gates | any non-art operator (Fable lane offered) | §3 Phase 1 → Phase 2 order |
| Content-factory gaps D→B→E→C→A | Architect lane | Phase 3 order |
| Art/Forge (water, creature presentation) | Picasso | unchanged — FORGE III queue |
| Feel verdicts, bakes until S2 lands, device evidence | Terry | headset test first; every session's notes feed the ratchet |

Multi-model merges: same envelope + same executable acceptance to N models when a section is
hard; CI green selects; judge only on green-green. HANDOFF stays the single narrative log.

---

## 5. Definition of success (measurable, per Terry's framing)

1. **Fast:** a recipe/data change ships to an APK with ZERO human steps between push and
   artifact (S1/S2 proof: change one vista color, push, watch it regenerate + pass).
2. **Accurate:** a Terry headset session finds only NEW bug classes, never a repeat (the
   ratchet metric), and his session is a 10-minute checklist, not exploration.
3. **Large scope:** one session walks one world through the assembly line in one sitting
   (GAP F sentence) — then N sessions do N worlds in parallel.
4. **Clonable:** the WorldImprovement §5 port list + docs skeleton instantiate for a second
   game and its CI runs green on day one.
