# POST-RECOVERY MASTER EXECUTION PACKET

**Prepared by:** Fable 5 (Reasonbox), chief-integration pass, 2026-07-17.
**Branch:** `fable/post-recovery-master-integration` (documentation only; draft PR; DO NOT MERGE before the Quest checkpoint result).
**Integrates:** `fable/hero-ship-production-packet` (`docs/post_recovery/HERO_SHIP_PRODUCTION_PACKET.md`, PR #59) and `fable/first-complete-slice-packet` (`docs/post_recovery/FIRST_COMPLETE_SLICE_PACKET.md`, PR #60). Both specialist branches were produced this same session from the same base and were re-read for this integration.
**Certified candidate untouched:** `2b158b4` per `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md`.

---

## 1. DOCUMENTATION RECONCILIATION — contradictions and stale statuses (verified line-level, 2026-07-17)

| Document | Stale/contradictory claim (exact) | Current truth | Disposition |
|---|---|---|---|
| `docs/PROJECT_COMPLETION_ROADMAP.md` | Line ~48: Input System `ApplyProcessors` NRE "still blocking headset authorization" | Resolved at rb36/rb37 (package matrix + clean 43/43); checkpoint AUTHORIZED at rb38 | Post-verdict: add a dated status header pointing at the checkpoint doc; keep body as the long-term roadmap. Do not rewrite history — annotate. |
| `docs/SPRINT.md` | Header: "🟡 ACTIVE SPRINT — QUALITY BAR PROGRAM (opened 2026-07-03)" with takeover prompt "read SPRINT.md and continue" | Sprint lanes were PAUSED by recovery (`docs/recovery/paused_sprint_lanes.json`); following that takeover prompt today would violate the freeze | Mark header FROZEN-BY-RECOVERY with pointer to this packet; the sprint concept resumes only as the packet PR queue. |
| `docs/CURRENT_EXECUTION_CHECKLIST.md` | "Status date 2026-07-11"; "latest proof `e8d18d6`" | Superseded by the entire recovery program and checkpoint evidence | Post-verdict: rewrite §1 "Operating truth" from the recovery-exit record; keep the rest as history with a dated banner. |
| `docs/GAME_PLAN.md` vs `PROJECT_COMPLETION_ROADMAP.md` | GAME_PLAN: "This is the roadmap-of-record"; ROADMAP also functions as the completion map | Two documents claim the same ownership | Decide ownership (below). Recommendation: ROADMAP = product completion map (what done means); GAME_PLAN = design-intent narrative it references. One line added to each stating this. |
| `docs/FABLE5_BACKLOG.md` | "Current-milestone queue" | Pre-recovery queue; several rows describe now-frozen or superseded work | Mark ARCHIVED; the milestone queue after the verdict is the integrated PR queue (§4). |
| `docs/HANDOFF.md` | (authoritative, current) | rb38 = checkpoint authorized | No change; HANDOFF remains the session log of record. |
| `docs/TERRY_RUNBOOK.md` | §0 already replaced with the checkpoint pointer (rb38) | Current | No change. |

**Recommended ownership going forward:**
- **Long-term roadmap:** `PROJECT_COMPLETION_ROADMAP.md` (annotated, never silently rewritten).
- **Current milestone:** `docs/post_recovery/POST_RECOVERY_MASTER_EXECUTION_PACKET.md` §4 (this queue) until it is exhausted, then a successor packet.
- **Active sprint:** retired as a concept; bounded PRs from packets replace sprint prose.
- **Device tasks:** `TERRY_RUNBOOK.md` only.
- **Recovery evidence:** `docs/recovery/**` (append-only).
- **Model takeover:** `docs/OPERATOR_START_HERE.md`, updated post-verdict to point here (§7 provides the block).

Reconciliation is itself PR-0 in the queue below — docs-only, mergeable immediately after the verdict, before any implementation PR.

---

## 2. TWO-OUTCOME SWITCH (tomorrow's Quest checkpoint)

### A. PASS
1. Record the recovery-exit entry in `HANDOFF.md` (template: verdict per checklist section of `QUEST_GOLDEN_CHECKPOINT.md`, log/evidence links, Terry's per-beat notes).
2. Tag the source SHA `2b158b4…` as the immutable recovery checkpoint (`recovery-golden-1` or Terry's preferred tag name).
3. Merge, in order: PR #59 + PR #60 + this PR (planning docs), then execute **PR-0** (reconciliation edits of §1).
4. Open the first implementation branch: **Q1 (slice beat tracker)** per §4 — the model-takeover block in §7 is the exact prompt.
5. Exposure stays golden-locked; features leave `PROTOTYPE_HIDDEN` only by entering the queue.

### B. FAIL
1. **Stop at evidence intake.** Collect: full logcat, Terry's per-step checklist verdicts, screenshots if any, and nothing else. No fixes, no rebuilds, no new branches.
2. Route each failure through the blocker routing in `QUEST_GOLDEN_CHECKPOINT.md`; classify against the R1 evidence classes (which automated lane SHOULD have caught it, and why it didn't — that gap is itself a finding).
3. The three packet PRs stay parked as drafts — they remain valid planning regardless of the verdict.
4. Diagnosis happens ONLY from the certified APK's evidence; the next candidate goes back through the full ladder (`RECOVERY_VERIFICATION_SYSTEM.md` §7).

---

## 3. INTEGRATION OF THE TWO SPECIALIST PACKETS

**File-claim overlaps found and resolved:**
- `ShipRefit.cs`: claimed by Hero PR-1/PR-6 (attachment re-target) and Slice PR-7 (payoff application). Resolution: Hero PR-1 defines the attachment contract FIRST; Slice PR-7 consumes it. If the verdict forces slice-first sequencing, Slice PR-7 ships against current scale-based seams and Hero PR-6 migrates it — both packets already wrote this fallback.
- `FirstHourSurfaceAuthor.cs` / W000 markers: Slice PR-2 (bunk/teaching surfaces) and Hero PR-2 (interior dress). Resolution: Slice PR-2 owns MARKERS, Hero PR-2 owns GEOMETRY; simultaneous claims forbidden — sequence them (Q4 before Q6).
- The helm: Hero PR-3 (consolidation to ONE helm) vs Slice beats 8–9 bindings. Resolution: slice binds completion observers to `ShipCastOffRuntime` (stable spine), NOT to helm UI internals — then consolidation can land without re-binding. This constraint is now binding on Slice PR-1's adapter design.
- `ShipCastOffRuntime.cs`: Hero PR-4 (coupler repair) and Slice observers. Resolution: Hero PR-4 lands before Slice PR-3 (the W001 job spine depends on trusting the repair→arm signal).

**Dependency verdicts:**
- Ship-vs-gameplay: the slice does NOT wait for the hero hull except at its final payoff/quality gate (Slice §8 already states this). Art and systems run parallel.
- Art-vs-systems: hull bake (Hero PR-1) is the only art-lane blocker for the slice's quality gate; everything else is dressing.
- Serial constraints: beat tracker → all beat PRs; coupler fix → W001 job spine; hull bake → interior dress → payoff visuals.

---

## 4. THE FIRST 8 MERGEABLE PRs (integrated queue; Q0 first, then by readiness)

Every PR inherits: forbidden owners (`TravelCoordinator`, `PlayerRigPersistence`, `PlayerInputSessionGuard`, `SaveSystem`, `BootLoader`, workflows, Packages, ProjectSettings, checkpoint/evidence); rollback point stated; visual artifact = the R1 capture set unless noted.

| # | Title | Outcome (player-visible) | Depends on | Owns (likely files) | Lane routing (minimum) | Quest gate |
|---|---|---|---|---|---|---|
| Q0 | Docs reconciliation (§1) | None (truthful boards) | Verdict recorded | The six §1 docs only | none (docs) | — |
| Q1 | Slice beat tracker + adapters (Slice PR-1) | None yet (beats advance silently) | Q0 | new `Gameplay/Runtime/FirstHour/FirstHourDirector.cs` + observers | EditMode + PlayMode + Contract Scan | — |
| Q2 | Coupler arming repair (Hero PR-4) | Repair → PUNCH IT works | Q0 | `ShipCastOffRuntime.cs`, `RepairableMachine.cs`, new PlayMode test | EditMode + PlayMode + Golden Android | repair→launch beat |
| Q3 | Hero hull bake + berth audit (Hero PR-1) | The ship looks like a ship | Q0 (art lane, parallel with Q1/Q2) | hull recipe, `ShipHullBuilder.cs`, `CityBuilder.cs` berth audit | EditMode + patch/audit + visual + Golden Android | silhouette session (Hero §7) |
| Q4 | W000 wake + teaching lines (Slice PR-2) | The game teaches itself | Q1 | `RillLineAuthor.cs`, `FirstHourSurfaceAuthor.cs` (markers) | EditMode + PlayMode + visual | wake sequence |
| Q5 | W001 job spine (Slice PR-3) | Accept→scan→repair playable | Q1, Q2 | W001 job spec (single owner resolved first), scan/machine authoring | EditMode + PlayMode + Golden Android | job route |
| Q6 | Ship interior dress (Hero PR-2) | Boarding reads as entering | Q3; markers frozen by Q4 | hull interior modules, `ShipBoardingStation` dressing, `ShipRefit` attach points | EditMode + PlayMode + visual + Golden Android | interior walk + panel readability |
| Q7 | ONE helm (Hero PR-3) | One consistent destination surface | Q5 (bindings on stable spine per §3) | `ShipBoardingStation.cs` helm, retire `FirstDestinationHelmRuntime.cs` | Full clean-lane suite green + Contract Scan + Golden Android | selection flow feel |
| Q8 | Weapon beat + taser grip data (Slice PR-4) | A weapon that points forward | Q1; R2 grip-contract decision or bounded taser-only data fix | target authoring, `Resources/Items` taser grip data | EditMode + PlayMode + visual (held-pose capture) | held pose + practice target |

(The remaining packet PRs — encounter, zipline/reward, changed-ship payoff, refit/cosmetics, fly-out, flight feel, optional garden — queue behind these in their packets' stated order; the payoff PR closes the slice with the Clean Package Proof + full Quest acceptance session.)

**Early-value guarantee:** Q2 and Q3 are player-visible within the first three merges (a working launch gate and a real ship); the queue never spends more than two consecutive PRs on invisible infrastructure (only Q0/Q1).

---

## 5. PROOF-LADDER ROUTING (per `RECOVERY_VERIFICATION_SYSTEM.md` §5–6)

- **Docs-only (Q0, packets):** no Unity lanes; Contract Scan optional.
- **Pure/EditMode-only changes:** ordinary CI suffices; do NOT burn PlayMode/Golden on data-table tweaks (e.g., a grip-angle data fix iterates on CI, its PR closes with one visual capture).
- **Anything touching golden-route scenes or their patch path (Q2, Q4–Q7):** ordinary CI + PlayMode observation minimum; Golden Android before the PR closes.
- **New runtime owner (Q1 only):** + Contract Scan mandatory.
- **Slice-complete and any future device candidate:** Clean Package Proof + Quest checklist — the ONLY rungs that authorize "done/device-ready" claims (`RECOVERY_PROGRAM.md` §3).
- **Nothing is exempt:** foundational changes (helm consolidation Q7) run the FULL clean suite even though they "should be equivalent" — equivalence is what the suite is for.

---

## 6. CONCURRENCY MAP (non-overlapping lanes + circuit breakers)

| Lane | Owner-model tier | File territory | Queue items | Circuit breaker |
|---|---|---|---|---|
| Systems/gameplay | mid-level+ | `Gameplay/Runtime/FirstHour/**`, job specs, observers, `Story/` repair wiring | Q1, Q2, Q5, Q8+ | 2 consecutive red runs on one PR → stop, post evidence to HANDOFF, wait for review |
| Art/ship | Picasso-tier + Forge tooling | hull recipe, `ShipHullBuilder`, interior modules, `ShipRefit` attach contract, cosmetics authoring | Q3, Q6, refit/cosmetics | a bake that fails its visual capture twice → fall back to interim hull, park the recipe |
| Story/audio prep | any tier (content) | `RillLineAuthor` lines, subtitle content, payoff lines | Q4 + line batches | lines that fail readability audit → content revision only, never audit relaxation |
| Device validation | Terry + one operator | runbook sessions, evidence intake | gates for Q2/Q3/Q6/Q7 + slice acceptance | any device FAIL → that lane's queue freezes until the evidence is classified |
| Integration authority | one senior operator at a time | this packet, queue ordering, conflict resolution | continuous | two lanes claiming one file → both stop; integrator resolves in writing before either proceeds |

---

## 7. MODEL-TAKEOVER BLOCK (copy-paste prompt for Q1 after a PASS verdict)

```
You are implementing ZIPTIDE queue item Q1: the first-hour beat tracker.

Read first, in order:
1. docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md (proof lanes; obey §6 routing)
2. docs/post_recovery/POST_RECOVERY_MASTER_EXECUTION_PACKET.md (this queue; you are Q1)
3. docs/post_recovery/FIRST_COMPLETE_SLICE_PACKET.md §4, §6 PR-1 (your spec)
4. docs/first_hour/README.md + first_hour_beats.json + BINDING_INVENTORY.md
5. docs/HANDOFF.md newest entry (verify the Quest verdict was PASS and Q0 merged)

Build: a FirstHourDirector that consumes the compiled beat contract
(FirstHourContractAuthor's runtime asset) and advances/persists beat state, plus
thin observers for exactly the 9 adapter-required bindings. Scene-content or
rig-ensured via existing ensure paths — NO new bootstrap, NO
RuntimeInitializeOnLoadMethod, NO scene/prefab YAML edits.

Bind completion observers to stable spines (ShipCastOffRuntime, TravelCoordinator
arrival signals via existing events, PlayerProfile flags) — NOT to helm UI
internals (helm consolidation lands later as Q7).

Never modify: TravelCoordinator, PlayerRigPersistence, PlayerInputSessionGuard,
SaveSystem, BootLoader, workflows, Packages, ProjectSettings, scenes, prefabs,
generated evidence. Beat state persists through existing SaveSystem seams as
additive PlayerProfile fields with neutral defaults (old saves must deserialize
unchanged).

Tests: EditMode contract-conformance (director graph == JSON contract);
PlayMode: scripted golden-route run advances beats 1–10. Log tag:
"ZIPTIDE: FH_BEAT id=<id> state=<state>".

Definition of done: EditMode + PlayMode + Contract Scan green; PR into
terry-local-wip; HANDOFF entry; the 43-test clean suite untouched and green.
Rollback: with the director absent, runtime behavior is byte-identical to today.
If anything in the spec contradicts what you find in source, STOP and post the
contradiction to HANDOFF instead of improvising.
```

---

## 8. HANDOFF

- **Branch:** `fable/post-recovery-master-integration` — this document only. Draft PR; DO NOT MERGE before the Quest verdict.
- **Files created:** `docs/post_recovery/POST_RECOVERY_MASTER_EXECUTION_PACKET.md` (this file).
- **Source branches integrated:** `fable/hero-ship-production-packet` (PR #59), `fable/first-complete-slice-packet` (PR #60) — overlaps resolved in §3; both packets' fallback clauses verified present.
- **Evidence inspected beyond the two packets:** line-level staleness checks quoted in §1 (`PROJECT_COMPLETION_ROADMAP.md:48`, `SPRINT.md` header, `CURRENT_EXECUTION_CHECKLIST.md` §1, `GAME_PLAN.md` header); `docs/recovery/paused_sprint_lanes.json` (existence), `QUEST_GOLDEN_CHECKPOINT.md`, `RECOVERY_VERIFICATION_SYSTEM.md`, `HANDOFF.md` rb36–rb38.
- **Unresolved decisions (Terry):** the doc-ownership recommendation (§1) needs a yes; tag name for the checkpoint; the three taste/scope questions carried in the specialist packets (hero §8, slice §10).
- **Exact first action after Terry reports:** **PASS →** §2A steps 1–4 (record, tag, merge the three planning PRs, execute Q0, then hand a mid-level model the §7 block for Q1). **FAIL →** §2B evidence intake only; nothing merges; diagnosis from the certified APK's log and observations exclusively.
