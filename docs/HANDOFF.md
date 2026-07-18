# HANDOFF — session log (single operator ⇄ Terry)

## Required reading

- `docs/FABLE5_START_HERE.md`
- `docs/HANDOFF.md` — current entries
- `docs/HANDOFF_HISTORY_THROUGH_RB24.md` — exact prior history through rb24
- `docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md` — Quest device recovery plan
- `docs/recovery/RECOVERY_PROGRAM.md` — active recovery authority, freeze, proof levels and R0–R4 path
- `docs/recovery/SYSTEM_CONTRACT_INVENTORY.md` — initial critical-system ownership map
- `docs/recovery/AUTOMATIC_RUNTIME_OWNERS.md` — automatic bootstraps, persistent mutators and feature injectors
- `docs/recovery/CANONICAL_OWNER_DECISIONS.md` — proposed surviving owners and displaced paths
- `docs/recovery/R1_INTEGRATION_HARNESS_SPEC.md` — locked next-phase verification design
- `docs/recovery/generated/` — generated source, scene, claim and ownership reports
- `docs/MASTER_CHECKLIST.md`
- `docs/FABLE5_BACKLOG.md`
- `docs/TERRY_RUNBOOK.md`

## Rules

1. Read the newest entry before work.
2. Append Did / Next / Heads-up / Commit at session end.
3. Work on `terry-local-wip`; pull before starting.
4. Keep commits small and stop implementation when required CI is red.

---

## ENTRIES — newest first

### 2026-07-18 (hwr27) - Fable 5 architect: 🎬 Director's Cut v2.1 — "THE KEY THAT KNEW YOU" + VEX BOOTSTRAPPER (Terry-directed, planning ONLY, zero code)
- **Terry's new story (now the spine of `docs/design/FIRST_HOUR_DIRECTORS_CUT.md` v2.1):** a space
  salvage sortie finds HALF an artifact ("that's not supposed to be — anywhere"); the Toxic City
  contract's payment includes the OTHER half; joining them lights a beacon-thread across the sky
  that leads **back to your own berth** — "the artifacts knew who you were"; the joined key seats
  in the ship's gate coupler and BOOM — the first Ziptide fires from YOUR ship to W002. §2 has the
  full beat + dialogue direction; §5 the new minute-by-minute; §6 the reconciliation (~11 net-new
  beats, FH-S01…S07 signals survive, beat 9 renames to FH_FIRST_FLIGHT).
- **NEW §2b — the quest-giver (Terry-directed):** *Vex Bootstrapper* — computer-code name per
  Terry ("Mr/Colonel Bootstrapper… maybe Vextor"); the universe's command-prompt-made-flesh who
  boots Cal's journey. **Unseen in hour one** — just two contracts signed `>_ V. BOOTSTRAPPER`
  (salvage job + Dockmaster work order), and RILL's noticing beat after the join: "Two contracts.
  Two halves. One signer." Alternates on file: Colonel Bootstrapper, Old Man Init.
- **📣 Story lane:** Vex + the artifact-key canon need STORY_BIBLE/TRANSMISSION canonization with
  Terry's sign-off — the doc marks what is recommendation vs canon. **📣 GPT/all lanes:** §8 keeps
  every new beat data-only (contract JSON + pack data + RILL lines; flags ARTIFACT_HALF_A/B,
  ARTIFACT_JOINED, KEY_SEATED) so direction changes stay cheap; §7 staging unchanged — Stage 1 is
  still device-proving the EXISTING 22-beat contract at Saturday's checkpoint. No code anywhere.
- **Commits:** this push (docs only).

### 2026-07-17 (hwr26) - Fable 5 architect: 🎬 THE FIRST HOUR — DIRECTOR'S CUT (Terry-directed, planning ONLY, zero code)
- **Terry's directive:** dial in the first hour — "through Toxic City having fun → our first
  Ziptide → our first world → rebuild and defend and grow and understand the whole cycle + the
  story's beginning." Research + plan only. Deliverable: **`docs/design/FIRST_HOUR_DIRECTORS_CUT.md`**.
- **The one structural finding (📣 GPT + all lanes, read §1):** the current 22-beat contract spends
  "the first Ziptide" on the W000→W001 commute. The director's cut re-stages it: W000→W001 becomes
  a SHIP FLIGHT (same PUNCH-IT rails, no gate FX), the ZIPTIDE gate is DISCOVERED in Toxic City —
  woken by the player's own relay repair (the authored "relay reads WRONG" seed becomes the
  inciting incident) — and the full built `ZiptideGateEffect` spectacle fires ONCE at the hour's
  peak, carrying the player to W002 Dry Cistern for the cycle-in-miniature: rebuild (pump repair +
  one BuildSocket extractor) → defend (one 90s wave) → grow (one garden seed — pays off NEXT
  session) → collect first yield → return to a changed ship. Minute-by-minute map in §5.
- **Deliberately conservative:** FH-S01…S07's code + signals survive untouched (beat 9 only
  RENAMES); ~8 net-new beats all compose already-built systems (RepairableMachine, BuildSocket,
  drone waves, GardenPlot, MiningRig hopper) — new pack data + RILL lines, no new mechanics.
  §7 stages adoption: Stage 1 = device-prove the EXISTING contract at Saturday's checkpoint first;
  Stage 2 = contract v2 (GPT's ledger + validators own the JSON change); the single garden plot
  unhides from the recovery freeze only at Stage 2 with Terry's sign-off.
- **Fun fixes folded in (§2–3):** toy-before-chore ordering in W001 (found taser + cans before the
  job; zipline moved onto the route), the excellent-drone loop + weapon-feel Phase 1 + caption v2
  land ON these beats, peak-end staging (gate wakes across the water; changed-ship ending).
- **Open Terry questions in §9** (peak placement, kid-mode wave, payoff object) — none block Stage 1.
- **Commits:** this push (docs only).

### 2026-07-17 (rb42) — Fable 5 → **📣 MESSAGE FOR GPT (firing range lane)** + Round-7 progression doc landed (planning only, zero code)

- **📣 To GPT, from Terry (relayed):** Terry approves the firing-range plans. **Sequencing:** hold
  the firing-range build until Terry's headset test confirms everything is set on-device; once he
  confirms, the alterations are green-lit and we get it all set. When you build, the full spec
  Terry approved is **`docs/design/ENEMIES_ENCOUNTERS_AND_BOSSES.md` §8** — the range as the
  weapon/enemy testbed (partially replacing sandbox for weapons), the labeled **"proof wall"**
  (one of every enemy role × weight tier), `respawnDelay > 0` dummies, bust-the-wave drills, no
  fail state, kid-testable. The range is YOUR lane; the design docs below are the shared canon to
  build against (weapons feel trio, throwables, powers/ultimate charge, enemy state machine).
- **Did (Fable 5):** completed the Round-7 research arc: re-ran the failed retention/family co-op
  agent, then wrote + committed **`docs/design/PROGRESSION_AND_THE_LONG_GAME.md`** — progression
  as permission-not-power (70/30 horizontal, affix escalation never HP inflation),
  lock→name→earn→payoff gating (tide-drive = macro key), 3-branch bloom upgrade trees + named
  synergies on the diegetic chip-in-socket bench, Salvager's Almanac + trophy-shelf long tail,
  while-you-were-away beat over the existing `IdleEngine`/`EcologyDirector`, and the honest family
  co-op ranking (pass-and-play ghosts → couch companion → colocated arena-only → online deferred).
  All grants/spends stay on the LOCKED `RewardRouter`/`ResourceDefinition` spine. This completes
  the seven-doc design set (weapons, powers/mobility, ships, enemies/bosses, progression) — all at
  build-ready level, zero code changed. **Terry: "I like the plans." This is the agreed pause.**
- **Next:** Terry's headset test is the gate for everything. After it: (1) GPT's firing range,
  (2) Phase-1 weapon feel trio per `WEAPON_FEEL_AND_ARSENAL.md` §4, (3) Terry's ⚖ still open on
  the ship-rides-the-tide travel model (`SHIP_DESIGN_INTERIOR_EXTERIOR.md` §10).
- **Heads-up:** nothing in the design docs is code yet — anything touching rig/locomotion/travel/
  inventory persistence in them is flagged report-only per CLAUDE.md. Don't build from a doc
  without checking its §"report-only" flags.
- **Commit:** this one (docs only: this entry) + `8ca5134b` (progression doc).

### 2026-07-17 (rb41) — Fable 5: RILL caption research + v2 spec (planning only, zero code)

- **Did:** Terry asked how VR games do captions/spoken text well. Audited the current system
  (`RillCompanion.cs` + `SubtitleText.cs`: hard head-lock, ~24° below gaze, raw pale-cyan TextMesh
  with no plate, unlimited line count, default Arial) and researched the field (BBC R&D VR subtitle
  studies, Owlchemy's Cosmonious High caption system, Game Accessibility Guidelines / Xbox / Ian
  Hamilton standards, Meta legibility guidance). Wrote **`docs/design/RILL_CAPTION_RESEARCH.md`**:
  findings + a concrete v2 spec — lazy-follow head anchor (0.25–0.35 s settle) at 12–15° below eye
  line and 1.3–1.6 m (Quest focal plane), dark ~65% plate + near-white semibold humanist sans
  (real imported font, TextMesh convention kept — no TMP), RILL state-color NAME TAG instead of
  tinted body text, ≤2-line chunked cards at 38 chars, per-word reveal now / whole-card when VO
  lands (⚖ Terry), overlay queue so captions never clip or vanish in the gate flash, directional
  chevron to RILL when she's off-view, and a PlayMode contrast/line-count/angle audit so caption
  regressions fail CI.
- **Next:** nothing runtime (freeze). The spec is a ~3-commit envelope proposed for the early
  post-recovery quality slice; FORGE IV CP-9 inherits it as its caption section. Terry decisions
  flagged ⚖: typewriter fate, plate opacity options.
- **Heads-up:** the current caption color (0.75,0.92,1) is nearly the Crest flash color
  (0.85,0.98,1) and close to our sky/water families — worst-case invisibility is REAL on the
  golden route; the plate + near-white body fixes it. Line WRITING quality is the story lane,
  not this doc.
- **Commit:** this one (docs only: research doc + this entry).

### 2026-07-17 (rb40) — Fable 5: Forge V/VI/VII FLESHED OUT — each horizon generation now has its plan of record (planning only, zero code)

- **Did:** at Terry's direction ("flesh them out a bit more — just planning, no code"), expanded
  each horizon generation from rb39's summaries into a full plan doc in the FORGE III/IV house
  style (rails, closed vocabularies, paper data schemas, per-envelope why/what/acceptance/budget/
  do-nots, cross-lane seam tables, pilots, definitions of done):
  **`FORGE_V_LIVING_STAGE.md`** — LS-1..LS-6 detailed: `WorldStateVariant` schema + default-state
  identity law, closed staging grammar (8 kinds) + tested player-freedom contract, RILL blocking
  library + growth clamps, ambient lanes (2 update rates, zero AI), weather/tide acts with 30–120s
  crossfades + saved act state, marks vocabulary; W001 pilot; order LS-1→2→5→3→4→6.
  **`FORGE_VI_WORLD_COMPILER.md`** — WC-1..WC-7 detailed: world-recipe paper schema, per-archetype
  kit completion bounded by pilot needs, 13-step deterministic compile pass + hash law + manifest,
  identity vector (7 dimensions, ≥3-dimension pairwise gate, thresholds calibrated on hand-built
  pilots) + hero-element block, ART_REGISTRY §5 on-ramps (build only on trigger), review-farm
  verdict schema + Terry sample-audit calibration; pilot = 2–3 compiled siblings vs. their
  hand-built original in blind comparison.
  **`FORGE_VII_DIRECTORS_CHAIR.md`** — DC-1..DC-5 detailed: closed command grammar (11 verbs) +
  the author-power law as a contract test + double-verdict commit flow (in-headset A/B → CI green
  → `chair:` commit), clamp-mirroring tuning board, verdict chamber consuming the WC-7 queue with
  hold-to-confirm ceremonies, tethered photo mode + HMD-sovereignty test + spectator splines,
  3-tier cosmetic forge; acceptance = Terry's "move that building" sentence run live end-to-end.
  `FORGE_V_AND_BEYOND.md` stays the horizon index (gate ladder/debt intake/rejections) and now
  points at the three plans; art-plan README updated.
- **Next:** unchanged — nothing runtime; the §1 gate ladder governs (recovery exit is rung one;
  Terry's Quest checkpoint remains the only device task). First future paper artifacts still:
  `LIVING_STAGE_CONSTITUTION.md` in late FORGE IV; WC-1 field-list draft during CP-11.
- **Heads-up:** the three plans deliberately pre-decide shapes (schemas, orders, clamps) so a
  smaller model can't invent them later — but every number marked "calibrated on pilots" is a
  placeholder until device/pilot evidence exists; do not treat them as licensed budgets. If FORGE
  IV execution reshapes CP-3/5/7/8 surfaces, amend the dependent envelopes in the same commit.
- **Commit:** this one (docs only: 3 new plan docs + horizon index pointers + README + this entry).

### 2026-07-17 (rb39) — Fable 5: FORGE HORIZON PROGRAM planned — the art program now has a destination past FORGE IV (planning only, zero code)

- **Did:** at Terry's direction ("take our current forge multiple steps further — in planning"),
  researched the full Forge program state (Forge I/II complete; Forge III ~85% landed with F3.6/F3.7
  open and device verdicts pending; FORGE IV planned, CP-0 complete) plus every recorded deferral
  across the art docs, then wrote **`docs/project_art_plan/FORGE_V_AND_BEYOND.md`** — the horizon
  program: **Forge V — Living Stage** (world-state skins, closed staging grammar with a
  player-freedom contract, RILL cinematic presence, ambient society in P3/P4 bands, weather/tide
  acts, return & memory), **Forge VI — World Compiler** (world recipes, kit completion, one
  deterministic compile pass, the identity guarantee + hero-element law, ART_REGISTRY §5 backend
  and streaming on-ramps, the CI review farm), **Forge VII — Director's Chair** (Terry's north star
  literal: in-headset command seam that compiles to author/recipe edits only, clamped live look
  tuning, in-headset verdict chamber, capture/photo mode, cosmetic forge). Includes a debt-intake
  table mapping every known deferral (water shader, grounding polish, VFX runtime, building
  modules, Tripo/Addressables triggers, WorldStubGenerator, skyscape tiers) to the generation that
  retires it, standing rejections carried forward, and a hard gate ladder. README index updated.
- **Next:** nothing runtime — the doc authorizes zero code. Gate ladder: recovery exit (Terry's
  Quest checkpoint, rb38) → vertical slice under the proof ladder → Forge III close → FORGE IV
  CP-1..CP-11 → only then Forge V. First future paper artifacts: `LIVING_STAGE_CONSTITUTION.md`
  during late FORGE IV, and WC-1's world-recipe field list during CP-11.
- **Heads-up:** the horizon doc is direction, not authorization — it explicitly defers to every
  binding plan and keeps all budgets/rails. If FORGE IV execution reshapes CP envelopes, amend
  `FORGE_V_AND_BEYOND.md` in the same commit. The recovery freeze is untouched; Terry's headset
  checkpoint remains the only device task.
- **Commit:** this one (docs only: `FORGE_V_AND_BEYOND.md` + art-plan README + this entry).

### 2026-07-17 (rb38) — Fable 5: HEADSET CHECKPOINT AUTHORIZED — paperwork complete, Terry's session is ready

- **At Terry's direction, the remaining authorization process is done:**
  ① **Independent artifact sample of clean run `29540554179`:** downloaded the Golden artifact and
  RECOMPUTED the APK SHA-256 myself — byte-identical to the workflow record
  (`9bfe13ac0acda6718c3ae1664919cc5609385e50691b8676219c552d8e555a10`); verified the build profile
  (GoldenSlice, `ZIPTIDE_RECOVERY_GOLDEN`, exactly `_Boot`/`W000_DriftIn`/`ToxicCity`, Succeeded);
  verified the cold-import resolved lock (Input System `1.6.3`, XRI `2.4.3`, OpenXR `1.14.3`);
  verified the clean player log: **zero NullReferenceExceptions**, zero settle timeouts, both golden
  visual captures OK. ② **PR #48 un-drafted and merged** (`c3ff88a`) — the Quest checkpoint doc +
  `docs/PROJECT_COMPLETION_ROADMAP.md` are on the branch. ③ **Authorization table FILLED** in
  `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md`: source SHA `2b158b4`, all run IDs, both hashes, the
  §1a lineage note (post-candidate commits are evidence-only; the pinned lock differs from the
  artifact lock ONLY in Linux host toolchain entries), authorized-by recorded. ④ **Fresh contract
  scan dispatched and green** (run `29545553407`). ⑤ **TERRY_RUNBOOK §0 replaced** — one pointer to
  the checkpoint doc; old stabilization §0 kept as history.
- **Status: the headset pass is AUTHORIZED.** Terry installs the exact artifact from run
  `29540554179` (hash-verified), runs the 12–20 minute route, captures the log. No local rebuilds;
  `quest_smoke.ps1`/`dev_build_install.ps1` are forbidden for this pass.
- **After the checkpoint (either outcome):** log + observations come back to whichever operator is
  awake. PASS → recovery exits at an immutable checkpoint (tag the source SHA), normal development
  resumes under `RECOVERY_VERIFICATION_SYSTEM.md`'s proof ladder, and the build queue starts from
  the roadmap's vertical slice (hero ship + one weapon + one production loop; the factory work
  stays gated per `docs/design/FACTORY_TOPTIER_PLAN.md` §0). FAIL → the blocker list in the
  checkpoint doc routes the evidence; no fixes before diagnosis.
- **Commit:** PR #48 merge `c3ff88a` · authorization fill + runbook + this entry (docs only).

### 2026-07-16 (rb37) — Fable 5: CLEAN PACKAGE PROOF GREEN — the last automated gate is cleared

- **Diagnosis:** the clean lane's only failure (run `29532312901` on `19b86b6`) was NOT the game,
  packages, or tests — the 43/43 assertion had already passed and the Golden APK had already built
  (checksum printed in the log) before both jobs died on `Permission denied`: game-ci's Unity
  container runs as root, so the regenerated `packages-lock.json` and `Builds/` outputs were
  root-owned and the runner user could not write the receipts beside them. The proof succeeded;
  the paperwork crashed.
- **Fix (`2b158b4`):** `sudo chown` the container outputs back to the runner user before each
  writeback/checksum step in `recovery-clean-package-proof.yml`. No test, threshold, or proof
  content changed.
- **RESULT (verified from the durable observation + jobs API, run `29540554179`):** on exact SHA
  `2b158b4`, with **Library deleted before every Unity job** — Clean PlayMode **43/43** (0 failed/
  skipped/inconclusive) · Clean EditMode ✅ · Clean patch/world audit ✅ · Clean locked Golden
  Android APK ✅. Input System `1.6.3` + XRI `2.4.3` confirmed as the resolved graph with no cache
  contamination possible. Ordinary CI also green on the same SHA.
- **What this means:** every automated requirement in the authorization list is now satisfied on
  one exact source. The intermittent post-travel Input System failure (rb36) is resolved by the
  package matrix change and proven from a cold import.
- **Remaining before Terry's headset (process, not proof):** ① independent sampling of the clean
  run's artifacts (rb30/31/35 template); ② the authorization table in `QUEST_GOLDEN_CHECKPOINT_V2`
  must quote THIS run's APK + build-profile SHA-256 (the hashes quoted for the previous candidate
  `19b86b6` are superseded — `2b158b4` contains only the workflow fix, no runtime change, but the
  authorized artifact must be the one from the clean run's own Golden job); ③ merge PR #48;
  ④ hand Terry the exact download/verify/install block. NO local rebuilds — the artifact is the
  candidate.
- **Commit:** `2b158b4` (workflow fix) · this entry (docs).

### 2026-07-16 (rb36) — Fable 5: input-race peeled to ONE residual poll; CIRCUIT BREAKER → package decision escalated to the lane

- **Also this window (both proven):** the travel-arrival input race was confirmed a REAL production
  defect and largely fixed; and BOTH proof lanes' trigger allowlists were missing the golden route's
  spine — `PlayerRigPersistence.cs` itself was not path-bound, so a production input fix shipped
  without triggering PlayMode/Golden. Fixed with `Player/**` + `World/**` globs (`3acb66f`). A proof
  lane that cannot see the file it proves is a one-sided seam; recommend the lane add a
  trigger-coverage check to the contract scan.
- **The evidence chain (4 runs, one failure class):** ① `29506225255` 41/43 — SnapTurn/ContinuousTurn
  `ApplyProcessors` NREs on EVERY post-travel arrival, stacks directly after `ANCHOR_ACTIONS_DISABLED`
  (production mutates live action assets mid-travel; readers poll during the re-resolve).
  ② Suspension window (`b431138`) → `29518294931` 42/43 — first poll after a blind 2-frame restore
  still NRE'd. ③ Probe-settle (`9b928cd`) → `29518996619` 42/43 — a PASSING probe plus a clean
  MOVE_DIAG read still preceded the NRE; restore had landed in the travel tail. ④ Travel-end gate
  (`8760c21`) → 42/43 — restore fired AFTER travel completed, probe read the exact polled actions
  cleanly, and SnapTurn's next poll STILL threw, with no intervening mutation logged. The perf-route
  test runs the identical route in the same player and passes clean each time.
- **Conclusion (three-strikes stop, per the law):** windowing cannot fully guard InputSystem
  **1.7.0**'s `InputActionState.ApplyProcessors` cache — it can break between a clean read and the
  very next poll under device/action churn. The residual is a PACKAGE defect class, not an ownership
  defect. The three landed windows remain net production hardening (the reproducible every-travel
  crash class is gone — that part ships value to the headset regardless).
- **ESCALATED DECISION for the recovery lane (Golden-affecting, needs its own bounded packet + full
  proof ladder):** upgrade `com.unity.inputsystem` from 1.7.0 to the latest verified 1.x (the
  ApplyProcessors NRE family has fixes in later patch lines), rerun the unchanged 43-suite, and take
  the package bump through EditMode + patch/audit + Golden APK before any headset checkpoint.
  Fallback if the upgrade is rejected: a bounded provider-level containment (try/catch read shim on
  the four rig providers) — uglier, but keeps the package pinned. Do NOT weaken the suite; the
  round-trip test is correctly refusing to certify a route that can still throw once.
- **State at hand-back:** durable PlayMode = 42/43 on `8760c21` (only `NewGame_W000_ToxicCity_W000…`
  red, single NRE in the whole suite); Contract Scan / CI / Golden Android green on the same SHA.
  R1.9 exit report stands (40/40 on `4512126b`); R1.10 artifacts + the canary proofs remain the next
  inspections after the package decision lands.
- **Commit:** `b431138` · `9b928cd` · `8760c21` (production hardening) · `3acb66f` (lane coverage) ·
  this entry (docs).


### 2026-07-16 (rb35) — Fable 5 takeover: 38/38 — the first fully green PlayMode suite, all four lanes green on one SHA

- **Takeover context:** per `docs/HANDOFF_RB34_GPT_LOOP_TO_FABLE5.md` (GPT's operator loop stalled on
  a PR-only polling helper; its two test packets were sound). Fable 5 resumed from the exact visual
  failure and drove it green in two bounded, test-only packets.
- **Root cause of the ToxicCity blank-cyan red (proven from raw artifact + source, not guessed):**
  the golden visual capture fired synchronously inside `TravelCoordinator`'s `TravelCompleted`
  publication — while `ZiptideGateEffect`'s arrival was still showing its deliberately OPAQUE crest
  flash shell parented around the head camera (torn down only ~12% into the arrival animation). The
  camera photographed the inside of the shell: uniform pale cyan whose measured luminance (0.9542)
  matches the `Crest` tint (0.85, 0.98, 1) exactly; the only other content was the Rill caption,
  which sits inside the shell radius. W000 passed only because boot travel is `skipGate` and plays
  no arrival effect. The world itself was never blank.
- **Packet 1 (`89cfa19`):** capture TIMING fixed, assertion surface GROWN — the TravelCompleted
  handler now only marks a destination pending (no assertions inside the publication path, which is
  also what had been cascading into the suite timeout); the round-trip test drives the deferred
  capture after each golden arrival with a bounded REAL-TIME wait for `ZiptideGateEffect` +
  `__ZiptideFlash` teardown (a stranded flash is now itself a failure), then runs the UNCHANGED
  frame/UI assertions; a new end-of-trip assert requires CapturedCount == 2 so deferral can never
  silently skip. Result (run `29496812449`): round trip PASSED; ToxicCity capture went from 3
  quantized colors / 0.008 dynamic range to **214 colors / 0.949**. The suite then flaked 37/38 on
  the OTHER known class: the R1.5 boot smoke burned its "600 frame" scene-load budget in 0.2 real
  seconds (headless ~3000 fps) while the disk-bound load still ran.
- **Packet 2 (`026e244`):** completed the frame-count → real-time migration the lane had already
  agreed to — the four remaining wall-clock-bound budgets (scene load 30 s, boot readiness 15 s, in
  the boot smoke and the round trip's `LoadActualBoot`) are now bounded real-time waits.
  Frame-semantic waits (delayed XRI binding, settled-frame pairs) deliberately stay frame-based.
  No thresholds or assertions weakened in either packet; production code untouched.
- **THE RESULT (independently verified, not self-reported):** durable observation for exact SHA
  `026e2446` = run `29497675520`, **NUnit total=38 passed=38 failed=0 skipped=0** — the first fully
  green R1 PlayMode suite. Same SHA: Contract Scan ✅, ordinary CI ✅, Golden Android ✅ — **all four
  lanes green on one exact commit.** I also pulled the green run's artifact and VIEWED the new
  ToxicCity PNG: a real rendered scene (buildings with window insets, railing, moon, Rill caption
  crisply readable). The W000 PNG was already verified real in the prior run.
- **Next (the rb34 order stands):** ① independent inspection of the full R1.5–R1.8 artifact set
  (PNGs, UI spatial reports, spawn clearance, census, travel/save/persistent-log evidence — the two
  PNGs above are inspected; the rest of the sampling remains); ② land and prove R1.9
  fallback-surface (PR #28, held); ③ correct the `WaitForEndOfFrame` in PR #33 and prove R1.10;
  ④ exact-SHA Golden Android with loud patch/bake/shader evidence, INCLUDING the rb33 requirement-3
  canary proof of the shader gate and fail-fast hooks; ⑤ independent claim sampling; ⑥ only then
  Terry's bounded Quest checklist. **Still NOT headset-ready; no Quest authorization; freeze holds.**
- **Heads-up (art note, not a defect):** the ToxicCity spawn view composes very tight against a
  building and dark — fine as visual PROOF, but the spawn-facing/vista composition belongs on the
  post-recovery presentation list alongside the R2 scene-presentation contract.
- **Commit:** `89cfa19` + `026e244` (test-only) · this HANDOFF entry (docs).

### 2026-07-15 (rb33) — Fable 5: GPT's end-of-window state recorded + the next bounded packet, with four requirements

- **Why this entry exists:** GPT's execution window ended before it could commit its own HANDOFF
  entry. Terry relayed its report; I'm recording it here for continuity and issuing the next-packet
  instructions Terry asked for. Status remains: **NOT headset-ready; no Quest authorization.**
- **GPT's reported state (as relayed; independently re-verify what's starred):** candidate
  `4954f388` ran the full 36-test PlayMode suite: **32 green, 4 red**. All four reds are TEST-RIG
  contract gaps, not game regressions: ① three tests fail because Unity's virtual `XRController`
  base layout has no `Primary2DAxis` control while the real locomotion bindings require
  `<XRController>{Left/RightHand}/{Primary2DAxis}` — the new guard correctly reports "8 locomotion
  actions, 0 bound controls" instead of a misleading null-ref (that's the harness doing its job);
  ② the render proof produces a valid PNG + metrics, then Linux headless Unity hangs during GPU
  resource DESTRUCTION — the proof works, the cleanup hangs. Also landed this window: bilateral
  tracked-rig simulator restored (the rb31 zero-test red fixed), real-scene UI audits for Home
  Hub/W000/ToxicCity, TMP-ancestry detection, floor/head-height/torso-clearance evidence at
  arrivals, real-time scene-load limits, explicit input-binding evidence, and a REAL defect found
  by the UI audit and fixed: the Home Hub title rendered ~8.75 m wide on a 2.4 m board. *Starred:
  GPT reports the rb31 HOLD-2 items (shader-variant gate, fail-fast patch/bake hooks, Forge-output
  reconciliation) now exist in source — I could not independently re-verify this window (tooling
  outage); requirement 3 below supersedes that verification anyway. Work-in-progress branch:
  `recovery/r1-virtual-layout-render-retention`.
- **NEXT BOUNDED PACKET — endorsed as GPT proposed, with four requirements attached:**
  1. **Test-only XR layout, test-only forever.** Register the `XRController`-derived layout (real
     `primary2DAxis` stick control, Left/Right usages preserved) from the TESTS assembly only, and
     add a source-scan assertion that no runtime assembly ever registers an input layout — a test
     device layout leaking into the APK would be a new DS-02-class collision. Then prove the eight
     real locomotion actions bind >0 controls on BOTH hands.
  2. **GPU retention is visible, capped, and named.** Retaining test textures until process exit is
     the right workaround for the headless destruction hang — but the retained objects must carry a
     hard cap, a comment naming the hang, and a count reported in the census artifact. Hidden
     retention today is tomorrow's "why does CI leak" mystery.
  3. **CANARY-PROVE the new Android gates before trusting them.** Seed a deliberate violation in a
     fixture — one runtime `EnableKeyword` transparency flip for the shader gate, one throwing
     patch hook for the fail-fast gate — and show each gate actually goes RED, then remove the
     seed. A gate that has never fired is an unproven gate; this also closes rb31 HOLD-2 with
     evidence instead of assertion.
  4. **Rerun the UNCHANGED 36-test suite — the assertion surface may not shrink to get green.** And
     land the isolated branch back onto `terry-local-wip` promptly via the exact-SHA rule; no
     long-lived divergence.
- **Then the already-agreed order, unchanged:** independent inspection of the PNGs/UI/clearance/
  travel/save artifacts → R1.9 fallback-surface proof → R1.10 performance/leak proof → exact-SHA
  Golden Android with patch/bake/shader evidence → independent claim sampling (rb30/rb31 are the
  template) → ONLY THEN the bounded Quest checklist for Terry.
- **Heads-up:** the Home Hub title catch is the program's proof-of-value in one line — a real
  player-facing defect found and fixed by an automated audit before a headset ever saw it. That is
  the machine we have been trying to build since the first broken device night.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-15 (rb32) — Fable 5: THE FACTORY top-tier plan boarded (planning only, hard-gated behind recovery)

- **Did:** wrote [`docs/design/FACTORY_TOPTIER_PLAN.md`](design/FACTORY_TOPTIER_PLAN.md) at Terry's
  direction after the 2026-07-15 automation deep-dive — the factory/automation system pitched and
  planned as if it were the sole product. Contents: the no-text first-factory pitch; six pillars
  (headline: **the kids test** — a new player with zero text reads producer/consumer/path in 30
  seconds — is the acceptance bar for every envelope); the 8-rung learning ladder (watch → connect →
  route → multiply → optimize → logistics → interplanetary → wonders); the **one-grammar/eighty-
  dialects** model for ~80 worlds (five inviolable machine silhouettes game-wide; variation spent on
  ~8 world-TIERS with one new rule each + free resource/terrain skins + one Wonder landmark machine
  per region); the fun multipliers (raid-defense fusion with the existing combat, the tide event,
  the staged idle-homecoming moment, conductor transit, jobs/ship integration); the sensory juice
  spec; and the F-0..F-9 envelope program with per-envelope `VISUAL`+`QUEST` proof.
- **Verified basis:** the deep-dive confirmed the mechanical layer survives untouched — pure
  `BeltLattice` sim (visuals are already "a skin over state" by design), physical tile placement,
  blueprint wand, conductor ride, economy-safe `MineState` sharing, 4.1f persistence, 4.1g budget
  audit. The overhaul is silhouettes + motion + feedback + content tiers, NOT a rewrite.
- **⛔ Heads-up (the gate, stated twice on purpose):** this is PLANNING ONLY. Nothing in F-0..F-9
  starts until the recovery program completes (R1 exit → R2 contracts → R3 golden slice green
  on-device). The doc's own §0 carries the same gate. Envelope F-0 (machine anchor contract) is
  deliberately shaped as an R2-style contract so the recovery lane can slot it when the time comes.
  All factory materials fall under the rb30 Amendment-1 shader-variant law from day one.
- **Commit:** the plan doc + this entry; documentation only, no runtime content changed.

### 2026-07-15 (rb31) — Fable 5 second independent review: R1.4–R1.8 verified with two holds before any headset build

- **Scope:** read-only spot-check of the recovery lane's latest status report against the repo and live
  CI, at Terry's request. No code changed. Verdict to Terry: **the report is accurate and honest** —
  claimed proof SHAs are real (`4988f44` census 28/28 with its raw artifact matching the claims;
  `0e74cec` actual-scene Home Hub 31/31), the new pieces exist as committed source
  (`PersistentDiagnosticRing`, `PlayerInputSessionGuard`, `RecoveryRenderSnapshot*`,
  `RecoveryUiSpatialAudit*`), and all four rb30 amendments show real motion (ring + BOOT_HOLD proof +
  probes-on-golden = A4 acted; the isolation-fault class = A3 acted). `PlayerInputSessionGuard` was
  specifically checked against the "fourth cook" risk: its header and shape are explicitly SUBORDINATE
  to PlayerRigPersistence's contract (owns no meanings, creates no objects) — a genuine consolidation.
  The duplicate scene-`InputActionManager` lifecycle owner it removed is exactly the failure class this
  program exists to catch, and it was caught by the harness, not the headset.
- **HOLD 1 — the current head is unproven: R1.8 PlayMode ran ZERO tests.** Live artifact for head
  `abdb441` (run `29445959178`): `Test step outcome: failure, NUnit totals: total=0` — the suite did
  not execute (the usual signature is a test-assembly compile error). Nothing on the "proven" list
  extends to this head until a run exists where 31+ tests actually execute and pass. The lane's own
  exact-SHA rule already implies this; recording it here so the claim can't drift.
- **HOLD 2 — rb30 AMENDMENT 1 (Android shader-variant static gate) is still NOT BUILT.** No scan
  exists yet in `tools/`, the recovery workflows, or the test suites (checked by grep). It is
  correctly scheduled inside "Golden Android build with patch, bake, shader, audit and APK evidence,"
  but it must EXIST before the next APK Terry installs — otherwise the white-square class
  (`PracticalLight`/`GroundShadow`/`SkyAtmosphereRig` runtime keyword flips) ships silently again and
  the headset session burns on a known fault. Same for the Amendment-2 patch/bake hook-failure gate
  (~41 swallowed try/catches in `BuildAndroid.PatchScenesThenAPK`; Forge bakes are gitignored and
  regenerated per build, so a silent baker failure = primitive guns/blank buildings in a "successful"
  build).
- **Endorsed path to the headset checkpoint (no change requested):** one exact SHA green across
  PlayMode (R1.6 real boot→W000→ToxicCity→back round trip) + snapshot PNGs directly reviewed + UI
  spatial audit + R1.9 fallback/placeholder exposure gate + R1.10 perf artifacts + Golden APK with
  loud patch/bake/shader evidence → independent sampling of the claims (rb30/rb31 are the template) →
  ONLY THEN a bounded Quest checklist. Boring on-device results are the goal; discoveries mean the
  gates missed.
- **Next:** recovery lane clears the two holds; Fable 5 (or any non-lane operator) re-samples at the
  next checkpoint. Separately, Terry has asked the art lane for a deep-dive assessment of the
  automation/factory system's visual legibility (analysis only, nothing built) — boarded so the lanes
  know it's coming; any resulting work is post-recovery R2+ and stays behind the exposure gate.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-15 (rb30) — Fable 5 independent review of the recovery program: ENDORSED, with four required amendments for GPT

- **Scope:** read-only review at Terry's request. No code changed. I read `RECOVERY_PROGRAM.md`, the R0/R1.2 exit reports, the contract inventory, `RecoveryRuntimeGate`/profile code and all three recovery workflows, and verified the lanes against LIVE CI runs rather than trusting the self-reported artifacts. Verdict passed to Terry: **adopt the program as-is** — the proof taxonomy, golden slice, exposure gating and repeat-green promotion rule are the correct systemic answer, and the harness has already caught two real bugs (the InputActionManager active-add lifecycle fault; the Home Hub late-manager binding race). The four amendments below are gaps the current design does not close. Terry approved sending them to the recovery lane.
- **AMENDMENT 1 — close the Android shader-variant hole (highest priority).** There is a failure class NO lane below `QUEST` can catch as designed: runtime-created materials that flip URP shaders to transparent/additive via keywords. `PracticalLight.cs` lines ~145–156 builds its halo by setting `_Surface/_Blend` floats + `EnableKeyword("_SURFACE_TYPE_TRANSPARENT")` on `URP/Unlit` at runtime; the Android build strips that variant unless a SHIPPED material uses it, so the flip silently no-ops → the opaque white streetlight square in Terry's screenshot. `GroundShadow.cs` and `SkyAtmosphereRig.cs` carry the same pattern (grep `_SURFACE_TYPE_TRANSPARENT|SetInt("_SrcBlend"`). The planned `VISUAL` lane runs a desktop renderer where those variants exist — **it will pass VISUAL forever while staying broken on device**. Required: extend the build-time material-validation contract with a STATIC source gate — runtime `EnableKeyword`/blend-state flips forbidden outside an allowlist; transparent/additive materials must be committed `.mat` assets or covered by a ShaderVariantCollection in Always Included. The existing recovery contract-scan tooling can host this scan; it converts the whole class into a CI red.
- **AMENDMENT 2 — stop the build swallowing patch/bake failures.** `BuildAndroid.PatchScenesThenAPK` wraps ~41 hooks in `try/catch` that logs a warning and keeps building. Forge bakes are gitignored and regenerated per build — if `ForgeBaker.BakeAll`/`ForgeAuthor.AssignAll` throws on Terry's machine, the APK ships primitive guns/blank buildings while the build reports success (a live candidate for the "some guns are blocks" observation). Required: the Golden APK lane and the local build must FAIL — or gate on a machine-readable hook-failure manifest — when any patch/bake hook throws, and assert bake-output counts against the recipe catalog (`ForgeDependencyAuditor` already emits the manifests; reuse them as the gate).
- **AMENDMENT 3 — the current PlayMode red is a TEST-ISOLATION fault; fix isolation, don't weaken the assertion.** Verified live: run `29416240184` on head `868fe44` = 20/21, failing `RecoveryHomeHubBindingTests.TileCreatedBeforeManager_BindsThroughDelayedPath` with "Expected: null / But was: XRInteractionManager" and `mode=immediate` in its output — a manager LEAKED from an earlier fixture, so the manager-free precondition never existed. That violates R1.2's own "no leaks after teardown" contract somewhere. Harden SetUp/teardown (destroy stray managers, or isolate the scene) — the delayed-binding fix under test may well be correct.
- **AMENDMENT 4 — guard the recovery lane against its own velocity + keep the evidence probes alive.** The lane self-grades its exit reports at very high commit velocity; the reports cite verifiable run/artifact IDs (good — my spot-checks held up). Required: before EVERY Terry headset checkpoint, an independent operator verifies a sample of exit-report claims against raw artifacts (this review is the template). Also: keep the rb26 `BOARD_PROBE`/`REPAIR_TRACE`/`FLIGHT_TRACE` probes exposed in the golden candidate and give `ZIPTIDE:` tags a persistent ring-file — Terry's last logcat rotated every diagnostic tag away before capture (the rb27 problem), which is why the floating-creature and dark-world causes are still unowned. Minor add: R1.5/R1.6 boot smoke should assert the `BOOT_HOLD` arm/release ordering explicitly so the rb26 boot fix earns `PLAYMODE` proof — the 41 s of `MOVE_DIAG grounded=False` in the recovered logcat is still an open question against it.
- **Live lane status at review time:** contract-scan **green** (`619f0eb`) · PlayMode **red 20/21** (`868fe44`, the isolation fault above) · Golden Android **in progress** on the same head. The both-lanes-green-on-one-SHA candidate rule is right — hold it; Terry does not headset-test before it exists.
- **Heads-up (own-lane admission for the record):** rb27's critique of my DS-14 fix is correct — per-face door labels fixed the facing convention but both faces are visible through open air (the doubled header text in the screenshots). The travel-door label fix belongs to the R2 `DiegeticPanel` contract, not another patch on `WorldTravelStation`. My FORGE III visual systems (grade, light script, practicals, water, macro variation) should stay `PROTOTYPE_HIDDEN` until each individually passes `VISUAL`+`QUEST`.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-14 (rb29) — R0 automated repository cartography: hidden features are still globally active

- **Did (report infrastructure):** added a separate report-only recovery workflow and tooling for repository-wide runtime ownership scanning, inventory validation, ownership maps, build-scene exposure, current-board completion claims, runtime input bindings/chords, exact C# type-to-path resolution, and event/save ownership. Reports are generated into `docs/recovery/generated/` and uploaded as workflow artifacts. No gameplay, scene, prefab, art or runtime behavior changed.
- **Scale of current evidence:** the broad scan covers 598 C# files and records 2,013 exact ownership signals. The initial 26-system inventory contains 37 truth defects: failed/unverified Quest observations recorded as accepted `QUEST` proof, guessed/descriptive source paths, and unresolved canonical owners. The claim audit found 260 strong completion lines in current boards, 138 without a proof qualifier on the same line. These are review targets, not automatic accusations of falsehood.
- **Scene exposure finding:** all 24 Build Settings scenes are enabled. Only `_Boot` and `W000` are already golden-path/support; ToxicCity and W002 are unresolved destination candidates; 20 enabled scenes are legacy tests, non-golden worlds or multiplayer prototypes. Documentation saying a feature is hidden does not make it unreachable in the APK.
- **Automatic-owner finding:** far more global owners run than the original inventory recorded. Confirmed automatic/persistent owners include DebugHUD, XR camera enforcer, runtime health monitor, runtime input enabler, runtime material fixer, VR boot diagnostics, ambience, comfort vignette, conquest mission injection, dev warp board, ecology, PvP progression, Quarters camera injection, SaveSystem, first-hour observation, player rig, audio, travel and singleton validation. Catalog: `docs/recovery/automatic_runtime_owners.json`.
- **Priority-zero visual collision:** `RuntimeMaterialFixer` runs after every scene load, scans every Renderer, and replaces null or non-URP materials with newly allocated URP/Lit materials colored green/blue/gray from object names. Therefore the headset can render something materially different from authored scenes, Forge assignments and build-time art audits. Recovery direction: build-time material validation plus explicit development fallback policy; retire the unconditional runtime rewrite after R1 proves the replacement.
- **Input ownership collision:** `RuntimeInputEnabler` globally reflects across controllers and every MonoBehaviour `InputActionReference`, enabling entire action assets after every load. `PlayerRigPersistence` separately adopts the XRI manager and moves/clears input-action ownership. Recovery decision: `PlayerRigPersistence`/one central input session survives; the reflection fallback is retired or gated after PlayMode proof.
- **Camera ownership collision:** `EnsureXRCameraActive` disables every active Camera not under a name containing `XR Origin` or `Camera Offset`. This can collide with field/photo/snapshot/spectator cameras. Recovery decision: replace it with explicit camera roles and one player-view owner.
- **Y+B finding:** the current `DevWarpBoard` only owns forehead gesture/F2/ADB access. The surviving Y+B contract is in persistent `QuickSwap`: B is quick-swap; Y is a guard that suppresses B because its source still says `Y+B = dev menu chord`. It does not itself open the current board. The control architecture still encodes a retired chord; the generated input report will locate every remaining chord and cross-owner control collision.
- **Travel finding:** runtime scene loading is centralized in `TravelCoordinator`; the only direct synchronous load is its own fallback when no coordinator exists. The main problem is lifecycle/input/UI ownership around travel, not many independent scene loaders.
- **Canonical decisions:** committed machine/human owner tables. TravelCoordinator, SaveSystem and PlayerRigPersistence remain core owners. New recovery contracts are required for exposure gating, diegetic panels, input meanings, camera roles, material/fallback policy, scene presentation, item poses, shooter exclusion and creature contact. Job/repair semantic ownership, the surviving melee implementation and the ship presentation root remain explicitly unresolved pending generated event/source reports.
- **R1 design locked:** `R1_INTEGRATION_HARNESS_SPEC.md` specifies the one-frame PlayMode spike, repeat-green promotion rule, tests-only fake tracked rig, automatic-owner exposure profile, runtime census, Home Hub ray smoke, one-world travel/save round trip, renderer-capable snapshots, UI spatial checks, prototype/fallback visibility gate and checkpoint-only Quest testing.
- **Next (R0):** consume the generated input/source/event reports; replace guessed inventory paths; remove invalid Quest proof labels; select one golden destination; identify the repair/objective semantic owner, melee survivor and ship root; produce the R0 exit report and bounded R1 implementation packet. Do not begin broad runtime consolidation before that exit report.
- **Heads-up:** `docs/CI_VERDICT.md` is still green only for older head `6b9d800`; later R0 tooling/docs commits are not called CI-green until a fresh durable verdict records them. The recovery workflow is report-only and independent of the Unity verdict.
- **Commits (this tranche):** recovery report tools/workflow and generated artifacts · `73b3857` automatic-owner catalog · `f049631` owner summary · `bc910b4` R1 harness spec · `fc9cdaa` canonical owner decisions · `1b919d0` canonical summary · this HANDOFF entry.

### 2026-07-14 (rb28) — GPT Recovery/Integration lane assigned; R0 contract inventory started

- **Authority:** Terry explicitly assigned GPT-5.6 Thinking ownership of ZIPTIDE recovery/integration and said to begin. Normal feature/world/mode/art/multiplayer expansion is now frozen under [`docs/recovery/RECOVERY_PROGRAM.md`](recovery/RECOVERY_PROGRAM.md). Emergency CI-red repair remains allowed; all other implementation requires a bounded recovery packet.
- **Did (control plane):** established recovery phases R0 repository truth → R1 integration harness → R2 contract consolidation → R3 golden vertical slice → R4 template expansion. Added explicit proof levels `SOURCE / CORE / PATCHED / PLAYMODE / VISUAL / APK / QUEST`; “CI green” is no longer a completion state.
- **Did (inventory):** added [`docs/recovery/system_contracts.json`](recovery/system_contracts.json), a machine-readable initial map of 26 critical systems: responsibility, candidate owner, source files, startup, persistence, runtime-created objects, state, actual proof, Quest status, exposure class, conflicts and next evidence. Added the human summary [`docs/recovery/SYSTEM_CONTRACT_INVENTORY.md`](recovery/SYSTEM_CONTRACT_INVENTORY.md).
- **Initial decisions:** preserve valuable pure cores/data/generators; hide Quarters, Tidefront table, PvP/Photon, broad world selection, melee, zipline, flight and unverified practical-light surfaces from the first recovery candidate; consolidate boot/rig/travel/UI/item/visual ownership before exposing them.
- **Next (R0 only):** complete repository-wide bootstrap/persistence/input/travel-bypass/runtime-object/event/save/global-render/fallback scans; reconcile old board claims against proof levels; produce canonical-owner, duplicate-owner, feature-exposure and R1 harness specifications. No broad runtime fixes before the R0 exit report.
- **Heads-up:** no gameplay, scene, prefab, asset, art or system behavior changed in these commits. The repository still contains all prior systems; the freeze controls what may be worked on and what will be exposed in the recovery candidate.
- **Commits:** `3c38dc2` recovery program · `b676507` machine inventory · `336bdaa` inventory summary · this HANDOFF claim.

### 2026-07-14 (rb27) — GPT read-only Quest screenshot forensics: Phase 1 failed on device; audit gates miss visible breakage

- **Did (scope):** Terry stopped the headset pass and supplied ten Quest screenshots plus a recovered logcat. GPT made **no gameplay, scene, prefab, art, asset, or system changes**. This entry is evidence only for Fable 5's audit plan; multiplayer and normal backlog progression remain paused.
- **Device acceptance failed:** menu ownership is still inconsistent on-device (some contexts still expose Y+B while the intended board uses the forehead gesture), and the cold-start Home Hub displays but does not reliably track/accept the pointed target. The rb26 source-level singleton and boot tests therefore did not establish a passing Quest experience.
- **Confirmed screenshot/source failure — persistent HUD:** the large yellow `CR 0` visible in every capture is `CreditsHud`, an always-on camera-relative TextMesh attached to the persistent rig. Its source deliberately redraws at 0.8 m in the lower-left every frame; it is not world scenery and currently contaminates every view.
- **Confirmed screenshot/source failure — travel-door text:** `WorldTravelStation` now emits a TextMesh on both sides of each door/header, but the header has no opaque backing between the two faces. Both copies can therefore be visible through open air. Long world names also share fixed 1.6 m door spacing with no wrapping, clipping, or width budget. This explains the doubled/mirrored and horizontally colliding door labels in the screenshots; DS-14 corrected a facing convention but did not solve multi-face visibility or layout.
- **Confirmed screenshot/source failure — war table:** `ConquestTableRuntime` builds all 12 planet labels, ticker, info card, END TURN/NEW WAR/HOTSEAT, eight defense tiles, and eight vessel tiles at hard-coded positions at once. It uses unbounded TextMesh labels with no panel layout, paging, clipping, facing contract, or camera-space readability pass. The screenshot's wall of overlapping fleet/defense/world text is the direct authored runtime layout, not a missing asset.
- **Confirmed screenshot/source failure — Quarters/locker:** `QuartersRoom` is explicitly primitive-built plumbing. It opens all three bays during `Awake`, emits each header/browse/empty-state TextMesh immediately, uses hand-authored rotations rather than the shared facing helper, and places the locker board at a fixed -90° yaw. The mirrored, stacked `TRAILS & EMBLEMS` / `SHIP LIVERY` / `YOUR LOCKER` text and intersecting room surfaces match that implementation.
- **Confirmed screenshot/source locus — practical lights:** `PracticalLight` intentionally creates camera-facing and surface-aligned **quad** renderers for every halo and pool. On the headset some appear as bright opaque squares/rectangles instead of soft radial glows. The quad ownership is confirmed; the exact runtime failure (texture alpha, URP blend state, material keyword, or device shader behavior) is **not** yet proven and must not be guessed.
- **Confirmed screenshot/source failure — legacy sky planet:** the large blue planet with diagonal bands is the legacy `SkyPlanetRig` sphere. Its texture is literally generated from `sin((x+y)*0.2)`, so the striped-ball appearance is expected from current code, not an asset-loading defect. The newer `SkyVistaRig` also uses unlit sphere bodies plus a global post-processing grade and fog. Cyan scene wash, crushed black silhouettes, and oversized simple celestial bodies may come from that path, but the exact active sky/grade owner must be logged per scene before changing it.
- **Confirmed presentation debt:** several screenshots expose primitive-only structures, conveyor pads, city blocks, ship/locker geometry, flat platforms, and controller-adjacent blocks as if they were final content. Existing source comments explicitly describe many of these as skeletons, plumbing, stubs, or later-art seams. The screenshots establish that those placeholders are not being visually contained or labeled as test content.
- **Previously confirmed and now visibly reproduced:** gun/hammer grip pose, scale, muzzle/laser alignment, holster orientation, and self-hit risks remain open from rb25. The screenshots show a sideways/oversized held object and displaced red aim ray; rb26 did not touch DS-06/07/08.
- **Unresolved device observations:** Terry also saw improved-looking bug creatures floating above the ground, extremely dark/near-black worlds, and mixed menu activation. No scene ID, creature ID, transform trace, or relevant diagnostic tag survived, so ownership/root cause remains unresolved. Do not call the creatures intentional flyers or assign a grounding bug without a scene/object trace.
- **Recovered logcat limitation:** the saved buffer retained no `BOOT_HOLD`, `HOME_HUB`, `DEV_WARP`, `BOARD_PROBE`, `REPAIR_TRACE`, `FLIGHT_TRACE`, `SPAWN_AT`, world, or creature identity records. It did retain about 41 consecutive seconds of `MOVE_DIAG provEnabled=True ... grounded=False`. That is evidence of an enabled locomotion provider while the controller reported ungrounded, but it does not by itself distinguish spawn, floor collider, grounding, or capture-timing causes.
- **Why green CI missed this:** `UiReadabilityAuditRules` is warning-only and checks only effective TextMesh size, collider target size, and label-to-collider separation. It does not test facing, duplicate opposite-face visibility, text overlap, wrapping, camera occlusion, contrast, or full runtime-built layouts. `ArtConformanceAuditRules` checks only whether a renderer has recognized provenance, not whether it looks good; every world is currently unlocked, findings are warnings, and an entire `PracticalLight` or `SkyVistaRig` subtree counts as conformed regardless of the screenshot result. Runtime objects created in `Awake`/`Start` also evade ordinary authored-scene inspection unless the audit explicitly instantiates them.
- **Next:** Fable 5 produces the audit/recovery plan from this evidence before any further feature work. The plan must inventory runtime-created UI/HUD/visual systems, identify one owner per responsibility, add screenshot/device acceptance gates, and separate confirmed source causes from scene-specific unknowns. Do not resume Phase 2, Picasso expansion, world mass-build, or multiplayer merely because EditMode and patch/audit CI are green.
- **Heads-up:** the ten screenshots are device evidence supplied in Terry's chat and are not committed binary assets. Preserve the observations above as the durable record; request scene/object IDs or targeted logs only when a proposed fix actually requires them.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-14 (rb26) — Fable 5: Phase 1 stabilization implemented + DS-09/10/12 evidence probes

- **Did (plan):** converted the rb25 forensic map into an approved fix plan with Terry's two decisions
  locked: the **physical board idiom owns DS-02** (device-proven rendering; the TMP canvas carries the
  recorded 2026-07-06 dead/flicker failure) and **instrumentation is included** alongside Phase 1.
- **Did (DS-01, `fix(boot)`):** the cold-boot HOLD contract. BootLoader arms it before the Home Hub;
  while held: move/turn/snap/dash providers suspended, the global fall net disarmed, rig pinned to its
  boot pose. Released ONLY in `TeleportToMarker` after a content spawn settles (re-arms the net from the
  fresh spawn); marker-less content scenes release too, so nobody arrives frozen. Pure `BootHoldState`
  seam + `BootHoldTests`. Logs `ZIPTIDE: BOOT_HOLD on/off`.
- **Did (DS-02/03, `fix(devtools)`):** ONE dev menu. `DevWarpBoard` is now summoned (forehead gesture /
  F2 / ADB gate), dismissible (gesture toggle + red CLOSE tile), fixed-pose at summon (the orbiting
  billboard is deleted), labels un-mirrored via the facing contract, and any open board closes on scene
  load. `DevMenu` retired from runtime (no bootstrap, no gesture; kept only as a manually-mounted
  diagnostic with the reason in its header). `DevToolsSingletonTests` source-scans DevTools: exactly one
  `RuntimeInitializeOnLoadMethod`, and it must be the board.
- **Did (DS-14/DS-03, `fix(ui)`):** `WorldLabelFacing` — THE facing contract in one pure helper
  (TextMesh reads from −Z; facing a viewer = +Z points AWAY). Travel doors now carry a label per FACE
  (readable from both approach sides, static, no per-frame cost). Tests pin the convention, pin the old
  buggy `LookRotation(toViewer)` as unreadable forever, and prove HomeHub's board math was already
  correct (why Terry could read the hub but not the doors).
- **Did (DS-09/10/12, `diag`):** log-only probes, zero behavior change — `ZIPTIDE: BOARD_PROBE`
  (hover/select + 1 Hz aim probe: actual ray hit path/layer, bound manager instance, facing dot),
  `ZIPTIDE: REPAIR_TRACE` (every hop: machine → director → runtime consumed/banked → bank-drain →
  objective board rendered text → cast-off observed instance), `ZIPTIDE: FLIGHT_TRACE` (per emitted yaw
  snap: raw stick, latch, yaw, frame ms). The forensic plan forbids behavior edits on these three until
  a capture picks the branch.
- **Next:** Terry runs **TERRY_RUNBOOK §0** (the Phase-1 device checklist + the logcat capture). Then:
  Phase 2 (DS-06/07/08 item/holster/hammer pose contract) with the captured evidence feeding DS-09/10/12
  fixes; DS-04/05 in Phase 3.
- **Heads-up:** DS-05's tracked-head fix is one line in `TravelCoordinator` but is deliberately parked
  for Phase 3 with its test, per the plan ordering. DevMenu's TMP canvas path still exists for manual
  diagnosis only — the singleton test turns CI red if anyone re-bootstraps it. All Phase-5 art items
  stay parked for Picasso.
- **Commit:** `a0dfdc7` (DS-01) · `5774182` (DS-14/03) · `aa59e7c` (DS-02/03) · diag + docs follow.

### 2026-07-14 (rb25) — GPT forensic device-stabilization map

- **Did:** Terry’s first successful Quest build installed and launched, then exposed failures across boot/spawn, developer UI, traversal, item poses, interactions, tutorial continuity, ship controls, and visual finish.
- **Did:** GPT made no gameplay, scene, prefab, asset, or system implementation changes. It traced the reports to source and wrote [`docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md`](DEVICE_STABILIZATION_FORENSIC_PLAN.md).
- **Confirmed collisions:**
  1. The new destination menu waits in an empty `_Boot` scene while older locomotion and fall recovery remain active.
  2. `DevWarpBoard` and `DevMenu` both self-bootstrap and persist.
  3. Player-centered travel VFX receives the XR rig root instead of the tracked-head center.
  4. A universal 45-degree gun fallback is reused across newer item, holster, and melee paths.
  5. `SonicThumper` and `PvpHammer/HammerTool` are separate mallet implementations with separate pose rules.
  6. Cave zipline endpoints are authored without generated-geometry clearance, and the shown cable differs from the rider path.
  7. Physical repair, job progress, objective display, and ship cast-off consume related state through separate runtime owners.
  8. Core ship and world systems currently expose explicitly interim primitive art as final-looking content.
- **Confirmed source causes/debt:** the 45-degree fallback and zero-valued item data explain the gun pose/scale; hand and holster poses are not independently authored; weapon paths lack a complete shooter-ignore contract; the ship hull, many buildings, fake-light visuals, planets, mountains, and vistas are documented primitive/interim/fallback implementations.
- **Unresolved pending instrumentation:** Match Board selection, gate objective not advancing, ship-turn glitch, exact streetlight-square branch, and any structural difference between W000 and W004. The plan specifies evidence to collect before changing them.
- **Next:** Fable 5 begins with Phase 1 only: cold-boot safety/ownership, one developer menu, and readable world-label facing.
- **Next phases:** item/holster/melee correctness; traversal/travel/tutorial continuity; ship function; Picasso visual recovery; then multiplayer.
- **Heads-up:** multiplayer is paused. Patch/audit CI proves structural readiness, not tracked-device pose, UI facing, interaction feel, objective continuity, or art quality.
- **Heads-up for Picasso:** real ship hull, buildings, practical lights, planets/clouds, and distant vistas remain named Phase 5 work. They are parked until function stabilizes, not discarded.
- **Commit:** plan `075afd9`; HANDOFF archive/pointer update follows as documentation only.

---

## Prior history

The complete prior HANDOFF is preserved byte-for-byte in [`docs/HANDOFF_HISTORY_THROUGH_RB24.md`](HANDOFF_HISTORY_THROUGH_RB24.md). Read rb24 and rb23 there for the patch/audit CI rules and W011 collider history.
