# FIRST COMPLETE SLICE PACKET

**Prepared by:** Fable 5 (Reasonbox), senior gameplay direction pass, 2026-07-17.
**Branch:** `fable/first-complete-slice-packet` (documentation only; draft PR; DO NOT MERGE before the Quest checkpoint result).
**Status:** execution-ready packet for POST-recovery implementation. No runtime work is authorized by this document.
**Authority context:** `docs/recovery/RECOVERY_PROGRAM.md` · `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md` (candidate `2b158b4`, untouched) · `docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md` (proof lanes) · `docs/GAME_PLAN.md` · `docs/first_hour/**` (the locked beat contract this packet builds on).

**The one-sentence thesis:** the repository already contains a LOCKED, machine-readable definition of the first slice — `docs/first_hour/first_hour_beats.json` (22 required beats, 15 core verbs, one teaching beat per verb) with an evidence-backed binding inventory — and the certified golden route (`_Boot → W000_DriftIn → ToxicCity → W000_DriftIn`) is that slice's exact spine, already proven at PlayMode/APK level. The slice work is therefore BINDING and CONTENT, not architecture.

---

## 1. ACTUAL SYSTEM INVENTORY (verified in source, 2026-07-17)

| Slice requirement | Existing files (exact) | Maturity (proof-ladder terms) |
|---|---|---|
| Cold boot / Home Hub | `_Boot` scene (build index 0), `BootLoader`, `Gameplay/Runtime/Tutorial/HomeHubRuntime.cs` | PLAYMODE+APK+device — inside the certified golden route; boot/new-game interaction proven in the 43-test suite. |
| Ship / W000 | `WorldLayoutLibrary.BuildW000DriftIn`, `FirstHourSurfaceAuthor.cs` (stable markers), `CityBuilder.cs:381` (W000 berth ship + `ShipCastOffRuntime`), `FirstDestinationHelmRuntime.cs` | PLAYMODE+APK — golden scenes. Hull look is GRAYBOX (see the hero-ship packet — the companion document). |
| RILL delivery | `Gameplay/Runtime/Story/RillCompanion.cs` (rig-persistent, subtitle delivery on world-entry + flag-poll, FollowUpTracker memory), `Editor/Patching/RillLineAuthor.cs` (authored lines incl. `enter_w000`, `cal_w000`), `RillLineLibrary` (Resources/Story/RillLines) | CORE+PATCHED functional. Teaching-line NEED: binding inventory says 15 teaching lines → 3 reuse-candidates, **12 content-required**. Delivery machinery needs no new architecture. |
| Signal state | Story flags via `PlayerProfile.flags` + canonical `ZiptideFlags` (`BINDING_INVENTORY.md` names these the reusable owners); job flags granted by `WorldJobLibrary` specs | CORE proven (flag plumbing used everywhere); the SIGNAL narrative surface itself is beat-bound content. |
| Scanner | `Gameplay/Runtime/Pvp/WristScanner.cs` (+ `HoloRadar.cs`) | SOURCE/CORE; device-unproven in the slice route. Beat `FH_SCAN_FAULT` (seq 12) depends on it — binding class per inventory: adapter/composite. |
| One starter weapon | Taser (`PvpWeapon.Taser` path through `CreatureRuntime.ReceiveHit`, `PvpCombatant.DamageFor` unified scale), item assets in `Resources/Items` | CORE+PATCHED; device verdict 2026-07-14 flagged GRIP POSE defects ("gun ~45° up, hits myself") — a presentation fix owned by the R2 grip contract, not new weapon work. |
| One creature encounter | `Gameplay/Runtime/Enemies/CreatureRuntime.cs` (non-lethal disable, loot via `RewardRouter.Grant`, respawn), behaviors incl. `WardenBehavior.cs`; roster authored by `CreatureVariantAuthor` | CORE+PATCHED; behavior readability gates exist (`CreatureBehaviorReadabilityCatalog` + build gate). Beats 17–18 (`FH_OBSERVE/COUNTER_SIGNATURE_CREATURE`) are composite-required: authored ENCOUNTER content on existing machinery. |
| One repair/production interaction | `Gameplay/Runtime/Story/RepairableMachine.cs` (MachineId/IsRepaired), W000 job spec (`WorldJobLibrary` → `w000_onboard`: `.Machine("gate_coupler", …, "coupler_cell", …)` + `.Repair("gate_coupler")`); ToxicCity contract via the older `ToxicCityContractBuilder` lineage | PATCHED in W000; beats 13–15 (`FH_REPAIR_ACCESS/PART_SEATED/MACHINE_POWER_CYCLE`) place the technician loop in ToxicCity — VERIFY the ToxicCity contract carries a machine leg; if not, author one (content, not code). Known device defect: coupler repaired-state false negative (owned by hero-ship packet PR-4). |
| One garden interaction | `Content/Runtime/Economy/GardenService.cs`, `Gameplay/Runtime/Story/GardenPlotRuntime.cs` + `WateringCanRuntime.cs`, `PourCore.cs`, 24 authored species (`GardenAuthor`), genetics (`PlantGenetics.cs`) | CORE+PATCHED, device-unproven. **DIVERGENCE:** the locked 22-beat contract contains NO garden beat. See §2 decision. Garden spawning currently wires through `WorldStubGenerator` (generated worlds); presence in ToxicCity/W000 is NOT yet authored. |
| Contract/job progression | `Editor/Patching/WorldJobLibrary.cs` (per-world specs: steps, flags, rewards; `HasJobsFor` coverage-tested), job runtime components it authors | PATCHED; W000's `w000_onboard` is the authored onboarding job (helm → manifest → coupler → cast off; grants `TUTORIAL_COMPLETE`, `FIRST_TRAVEL`, `C1_W001_RILL_BOOT`). |
| Rewards / economy | `RewardRouter.Grant` (+ `LedgerSource`), `SalvageCacheRuntime.GrantTo`, one-economy audit (`EconomyAuditRules`) | CORE proven and audit-gated. Beat 20 `FH_FIRST_JOB_REWARD` binds here. |
| Save / relaunch | `SaveSystem` (canonical owner), `PlayerProfile` (atomic + backups), additive-field law | PLAYMODE+APK — quit/resume is inside the proven route evidence. |
| Travel and return payoff | `TravelCoordinator` (canonical owner), crest transition, `WorldGating` | PLAYMODE+APK+device-candidate — the certified route IS W000→ToxicCity→W000. Beat 22 `FH_CHANGED_SHIP_PAYOFF` is new-surface-class: the RETURN must LOOK different (ship state change) — content on `ShipRefit` seams. |

**Roadmap-vs-source checks:** `GAME_PLAN.md`'s ordering (story delivery → repair/production → starter gear) is consistent with the beat contract. The binding inventory's own caveat is honored throughout: bindings marked `proposed`/`adapter-required` are requirements, NOT existing events — nothing below claims otherwise.

**Binding arithmetic (from `docs/first_hour/BINDING_INVENTORY.md`, unmodified):** 22 completion bindings = 4 `verified-existing` + 9 `adapter-required` + 5 `composite-required` + 4 `new-surface-required`. This is the real size of the slice: ~9 thin observers, ~5 authored encounters/compositions, ~4 genuinely new player-facing surfaces, 12 authored teaching lines.

---

## 2. THE EXACT SLICE (chosen)

**Adopt the locked 22-beat contract as-is.** Cold boot → W000 wake (RILL, comfort console, move/grab/holster) → helm → first Ziptide → ToxicCity arrival → accept job → scan fault → repair (access/seat part/power cycle) → practice target → observe + counter the signature creature → job zipline → reward → return to ship → changed-ship payoff.

- **Starting location:** `_Boot` → `W000_DriftIn` (the ship as home).
- **Destination world:** `ToxicCity` (W001).
- **One weapon:** the taser (non-lethal canon's flagship, unified damage scale, already the teaching weapon).
- **One opponent:** the ToxicCity signature creature (existing roster id chosen at encounter-authoring time; `swarm_bug` band for teaching + one signature individual for beats 17–18).
- **One repair/production action:** the W001 technician machine loop (beats 13–15), reusing the `RepairableMachine` + job-step machinery already proven by W000's coupler.
- **One garden action (DECISION REQUIRED — §8/§9):** the locked contract has no garden beat. Recommendation: add ONE optional (non-required) beat pair — plant a seed in a small quarters planter before first departure (`GardenPlotRuntime` by the bunk, one authored `GardenSpawnDefinition`), find it grown on return — reinforcing beat 22's changed-ship payoff with zero new systems. This amends a LOCKED contract, so it ships only with Terry's explicit yes; otherwise the garden stays out of slice v1.
- **One RILL/story beat:** the wake sequence (`FH_LOOK_AT_RILL`) plus the 12 content-required teaching lines.
- **One reward:** the W001 job payout through `RewardRouter` (beat 20).
- **One visible ship/home payoff:** beat 22 — the ship changed by your trip (journey decal via `ShipRefit`, cargo visible, optional grown plant).

**Why this combination:** it is the ONLY candidate whose travel/save/boot spine is already certified at APK level on named hardware-ready evidence (the golden checkpoint route); it reuses the highest-proof systems (flags, jobs, RewardRouter, RillCompanion, RepairableMachine, CreatureRuntime); and its gap list is enumerated by an existing evidence-backed inventory rather than guesswork. Every alternative (W002 cistern loop, arena-first, flight-first) abandons certified ground for unproven scenes.

---

## 3. PLAYER EXPERIENCE SCRIPT (15–30 minutes, no developer instructions)

1. **Boot (1 min).** Headset on → `_Boot` → Home Hub: NEW GAME / CONTINUE panels, readable at arm's length, nothing else competing. Player selects; the game records the save-slot creation (`FH_BOOT_READY`, `FH_NEW_GAME_SELECTED`).
2. **Wake on your ship (3–5 min).** Quarters bunk aboard the Rustbucket. RILL's orb drifts into view and speaks (subtitles): you're the new technician; the ship is yours. Player LOOKS at RILL (`FH_LOOK_AT_RILL` — gaze binding). The comfort console glows one step away: pick Cozy/Standard/Bold (`FH_COMFORT_CONSOLE`). RILL teaches move (`FH_MOVE_IN_QUARTERS`), grab the guild manifest from the bunk shelf (`FH_GRAB_BUNK_OBJECT`), holster it (`FH_HOLSTER_FIRST_ITEM`). Every verb: one line, one hesitation timer, one action. The game records comfort choice + flags; the player has learned look/move/grab/holster without a menu.
3. **The helm and the first Ziptide (2–3 min).** RILL points forward: the helm. One destination is lit — ToxicCity (`FH_INTERACT_HELM`). PUNCH IT: the crest swallows the view, engine audio swells (`FH_FIRST_ZIPTIDE` — the certified travel path). Arrival: the toxic skyline, RILL's arrival line (`FH_W001_ARRIVAL`).
4. **The job (8–12 min).** A contract board/dispatcher within sight of the spawn: ACCEPT (`FH_ACCEPT_FIRST_JOB`). The wrist scanner pings the faulty machine — scan it (`FH_SCAN_FAULT`, teaching the scanner). Reach the machine hall (`FH_REPAIR_ACCESS`), seat the replacement part (`FH_REPAIR_PART_SEATED` — physical socket, the coupler interaction pattern), cycle power (`FH_MACHINE_POWER_CYCLE` — the machine visibly spins up; sound + light state change). En route, a practice target teaches the taser (`FH_SHOOT_PRACTICE_TARGET`). The signature creature appears on the route home — RILL: observe first (`FH_OBSERVE_SIGNATURE_CREATURE` — telegraphed behavior, readable), then counter non-lethally (`FH_COUNTER_SIGNATURE_CREATURE` — stun, never gore; it crumples, discharge arcs, loot chimes). The job zipline shortcuts the return leg (`FH_USE_JOB_ZIPLINE`).
5. **Reward (1 min).** The dispatcher pays out — a number the player SEES land (`FH_FIRST_JOB_REWARD`; `RewardRouter` toast + ledger line). RILL: "back to the ship."
6. **Return and payoff (2–3 min).** Board, PUNCH IT home (`FH_RETURN_TO_SHIP`). On the W000 berth the ship is CHANGED: first journey decal on the flank, cargo visible in the pod, (optional) the planted seed sprouted (`FH_CHANGED_SHIP_PAYOFF` → `TUTORIAL_DONE`, `FIRST_HOUR_COMPLETE`). Save happens on travel (existing autosave); quit → relaunch → CONTINUE restores exactly here.

What the game records throughout: comfort choice, taught-verb flags, job step states, creature resolution, economy ledger entries, first-hour completion flags — all in `PlayerProfile` via existing owners.

---

## 4. OWNER AND DATA-FLOW GRAPH

```
JobDefinition (WorldJobLibrary spec, authored at patch time)
  → job runtime components (scene content)              [job state]
RillLineLibrary (RillLineAuthor, Resources/Story)
  → RillCompanion (rig-persistent, PlayerRigPersistence-ensured)  [delivery]
ItemDefinition/Resources/Items → ItemFactory → held item [tools/weapons]
CreatureDefinition/Resources/Enemies → CreatureRuntime   [encounter]
RepairableMachine (MachineId) ←→ job .Machine/.Repair steps
WristScanner (rig) → scan events                         [FH_SCAN_FAULT]
GardenSpawnDefinition → GardenPlotRuntime → GardenService (optional beat)

Completion signals (per binding inventory class):
  verified-existing → read directly
  adapter-required → thin observers ONLY (no ownership change)
  composite/new-surface → FirstHourDirector-style beat tracker (see PR-1, §6)

Flags/economy/save:
  beat completion → PlayerProfile.flags (ZiptideFlags) → RillCompanion flag-poll lines
  rewards → RewardRouter.Grant(LedgerSource, …) → PlayerProfile ledger → EconomyAuditRules
  ALL persistence through SaveSystem (atomic, backups) — no new writers

Travel:
  helm select (WorldGating.MeetsRequirements) → ShipCastOffRuntime/ShipBoardingStation
  → TravelCoordinator.TravelTo (ONLY path) → crest → arrival
  input across travel: PlayerInputSessionGuard (observe only)
```

**Duplicate-owner flags (must resolve during implementation, not silently):**
1. Two helm surfaces (`FirstDestinationHelmRuntime` vs `ShipBoardingStation` helm) — consolidation is hero-ship packet PR-3; the slice consumes whichever is canonical at build time.
2. Beat tracking: the contract needs a completion-signal aggregator; `docs/first_hour/README.md` explicitly notes no `TutorialDirector` exists. Exactly ONE new owner (the beat tracker) is justified — it must be scene-content or rig-attached via existing ensure paths, never a new bootstrap, and it OBSERVES the protected owners.
3. `WorldJobLibrary` vs legacy `ToxicCityContractBuilder` lineage for W001's contract — one spec source must own the slice job; verify before authoring beats 11–20.

---

## 5. REUSE / GAP MATRIX (per beat, from the binding inventory + source)

| Beats | Class | Verdict |
|---|---|---|
| 1–2 boot/new-game | verified/adapter | **Already proven** (golden suite exercises real boot interaction). |
| 3 look-at-RILL | adapter | Implemented but unproven as a gaze signal — thin observer. |
| 4 comfort console | verified-existing candidate | `ComfortConsoleRuntime.cs` exists; wire completion. |
| 5–7 move/grab/holster | adapter | Machinery proven; needs observers + teaching lines. |
| 8–9 helm/first Ziptide | verified (travel) + adapter (helm) | Travel certified; helm completion observer thin. |
| 10 arrival | adapter | Scene-entry flag — trivial observer. |
| 11 accept job | composite | Job board surface in ToxicCity + accept signal (job machinery exists). |
| 12 scan fault | composite | WristScanner exists, device-unproven; scan-target authoring needed. |
| 13–15 repair loop | composite | `RepairableMachine` + socket pattern proven in W000; W001 instance must be authored/verified. |
| 16 practice target | adapter | Target + hit signal on existing weapon plumbing. |
| 17–18 signature creature | composite | CreatureRuntime + behaviors exist; the ENCOUNTER (placement, telegraph pacing, arena space) is authored content. |
| 19 job zipline | adapter | Zipline runtime exists (traversal 💎 row); device verdict flagged a broken cave zipline — the W001 job zipline needs its own placement + device check. |
| 20 reward | verified/adapter | `RewardRouter` proven; visible payout moment is presentation. |
| 21 return | verified | Certified route. |
| 22 changed-ship payoff | **new-surface** | The payoff presentation (decal/cargo/plant state on arrival) — content on `ShipRefit` seams; the emotional target of the whole slice. |
| Teaching lines ×15 | 3 reuse / 12 content | Author via `RillLineAuthor` (create-only, established). |
| Garden beat (optional) | composite | All machinery exists; needs one planter authored in W000 + Terry's contract-amendment approval. |
| **Deferred out of slice v1** | — | Flight (separate campaign), arenas/PvP, factories/belts, ecology surfaces beyond the one encounter, photo camera, Quarters cosmetics browsing (stub content), Tidefront. All remain `PROTOTYPE_HIDDEN` per the exposure manifest. |

---

## 6. EXACT PR SEQUENCE (eight bounded implementation PRs, post-recovery)

Global forbidden set for every PR: `TravelCoordinator`, `PlayerRigPersistence`, `PlayerInputSessionGuard`, `SaveSystem`, `BootLoader` internals; workflows; Packages; ProjectSettings; checkpoint/evidence files. Proof lanes per `RECOVERY_VERIFICATION_SYSTEM.md` §5–6. Every PR: new `ZIPTIDE:` tags listed, rollback point named.

**PR-1 — The beat tracker (the one new owner).**
Outcome: first-hour beats advance and persist; no player-visible change yet.
Files: new `Gameplay/Runtime/FirstHour/FirstHourDirector.cs` (scene-content in W000/W001 or rig-ensured via existing ensure path — NO new bootstrap), consuming `first_hour_beats.json` compiled by the existing `FirstHourContractAuthor`; adapter observers for the 9 `adapter-required` bindings.
Tests: EditMode contract-conformance (beat graph = JSON); PlayMode: scripted route advances beats 1–10 on the golden scenes. Tags: `ZIPTIDE: FH_BEAT id=… state=…`. Lanes: CI + PlayMode + Contract Scan (new owner declared). Rollback: director absent → game behaves exactly as today (observers are passive).

**PR-2 — W000 wake + teaching lines (beats 3–7).**
Outcome: the wake sequence teaches look/move/grab/holster with RILL lines.
Files: `RillLineAuthor.cs` (12 content lines authored), `FirstHourSurfaceAuthor.cs` (bunk shelf item, marker adjustments), no rig/input changes.
Tests: EditMode line-coverage (every teaching beat has its line id); PlayMode hesitation-timer behavior. Lanes: CI + PlayMode + visual capture (subtitle readability class). Rollback: lines are data; beats fall back to silent completion.

**PR-3 — W001 job spine (beats 11–15): board, scan, repair.**
Outcome: accept → scan → three-step repair, fully playable in ToxicCity.
Files: the W001 job spec (one owner — resolve §4 flag 3 first), scan-target + machine authoring in the ToxicCity patch path, `WristScanner` completion adapter.
Tests: EditMode job-spec coverage; PlayMode: scripted route completes 11–15. Lanes: CI + PlayMode + Golden Android (route scenes touched). Rollback: job spec is data; revert restores current contract.

**PR-4 — Weapon beat + grip-pose dependency (beat 16).**
Outcome: practice target teaches the taser, held CORRECTLY.
Files: target authoring; consumes the R2 grip contract fix (`ItemDefinition.gripLocalEuler` data pass) — if R2 hasn't landed, this PR carries the taser'S grip data fix only, nothing broader.
Tests: PlayMode hit-registration; visual capture of held pose at eye height. Lanes: CI + PlayMode + visual. Rollback: target removal.

**PR-5 — Signature encounter (beats 17–18).**
Outcome: observe-then-counter, readable and fair.
Files: encounter authoring (placement/arena/telegraph pacing) on existing `CreatureRuntime` + behavior; NO behavior-system changes.
Tests: PlayMode: scripted observe→stun completes both beats; readability catalog rows updated. Lanes: CI + PlayMode + visual + Golden Android. Rollback: encounter spawn flag off.

**PR-6 — Zipline leg + reward moment (beats 19–20).**
Outcome: the shortcut home + a payout the player sees.
Files: W001 zipline placement (device-verify — the cave zipline defect history demands a clearance check in PlayMode), reward toast presentation on `RewardRouter` (observer, not a router change).
Tests: PlayMode traversal along the zipline path (clearance asserts); economy ledger assert. Lanes: CI + PlayMode + visual. Rollback: zipline is scene content.

**PR-7 — The changed-ship payoff (beat 22 + 21 polish).**
Outcome: returning home LOOKS earned — first decal, cargo, completion lines, `FIRST_HOUR_COMPLETE`.
Files: `ShipRefit` decal/cargo application on the W000 berth (through the hero-ship packet's attachment contract if PR-1 of that packet has landed; else current seams), RILL payoff lines.
Tests: PlayMode full 22-beat run end-to-end green; save/relaunch asserts. Lanes: CI + PlayMode + visual + Golden Android + **Clean Package Proof** (this is the slice-complete milestone). Quest: the full §7 acceptance session. Rollback: payoff presentation flag.

**PR-8 (conditional — Terry's yes on §8 Q1) — The quarters planter.**
Outcome: plant before you leave, find it grown on return.
Files: one `GardenSpawnDefinition` + planter authoring in W000 quarters; beat-contract amendment (versioned, explicit).
Tests: EditMode contract version bump asserts; PlayMode plant→travel→return→grown. Lanes: CI + PlayMode + visual. Rollback: planter authoring removed; contract reverts to 22 beats.

---

## 7. ACCEPTANCE CONTRACT (objective; measured on the slice-complete candidate)

- **Comprehension:** a first-time player reaches ToxicCity within 12 minutes without any out-of-game instruction; every taught verb is used unprompted at least once afterwards (log-verified from beat + input tags).
- **Interaction reliability:** 0 failed grabs/holsters on correctly aimed attempts across the session log; every panel interactable answers within its collider (UI spatial evidence class green).
- **Combat feel:** practice target and creature stun both land within 3 attempts for a new player; taser held pose within ±5° of authored grip in the visual capture.
- **Repair legibility:** each of the three repair steps discoverable without RILL repeating (hesitation timer fires ≤1 time per step in the acceptance run).
- **Garden feedback (if PR-8):** planted state visibly different on return; watering interaction completes in one pour.
- **RILL readability:** subtitle contrast/size passes the existing UI readability audit at arm's length; no line overlaps geometry (facing checks green).
- **Reward clarity:** the payout moment names amount + currency on screen ≥2 s; ledger entry matches (`EconomyAuditRules` invariant).
- **Save/relaunch:** quit at ANY beat → relaunch → CONTINUE restores world, beat state, inventory, and comfort settings exactly (PlayMode matrix across 4 sampled beats + device spot-check).
- **Comfort:** all motion in the slice is player-initiated or crest-covered; comfort choice from beat 4 visibly respected (vignette strength matches preset).
- **Quest performance:** 72 Hz hold on the full route with ≤5 dropped-frame events per minute (device log), matching or beating the Linux reference artifact trend.

---

## 8. CONTENT AND ART DEPENDENCIES (minimum credible set)

Required for the QUALITY GATE (not for implementation start):
- Hero-ship exterior/interior pass — **owned by the companion hero-ship packet** (its PR-1/PR-2); the slice's beats 3–9/21–22 read wrong against the interim blocks.
- ToxicCity job-route dressing: the machine hall (existing kit/interior systems), the practice range corner, the signature-creature arena pocket — all via existing patcher/dressing systems.
- 12 RILL teaching lines + ~6 payoff/arrival lines (text; VO explicitly deferred — subtitle-first per `VO & subtitles` roadmap row).
- Signature creature: Forge-baked body already in the pipeline (`ForgeBodyLibrary`); needs its telegraph anim/tint states verified readable, not new art.
- Reward toast + decal art: one decal emblem + one toast layout (tiny).
Acceptable placeholders DURING implementation: interim hull, primitive machine hall, flat practice target. NOT acceptable at the gate: mirrored/blank text, floating unposed items, the interim hull on the payoff beat.

---

## 9. FAILURE RISKS — the ten likeliest ways this becomes another impressive backend, and the gate against each

1. **Beat tracker becomes a second state machine fighting jobs/flags** → gate: PR-1's contract test asserts the director only OBSERVES existing signals; Contract Scan must show no new writer to protected state.
2. **Teaching lines written but never bound** (12 content-required) → gate: EditMode line-coverage test fails any teaching beat without a line id.
3. **ToxicCity repair loop authored against the wrong job owner** (WorldJobLibrary vs legacy builder) → gate: §4 flag 3 resolved in PR-3's description before any authoring; one spec source asserted by test.
4. **Grip poses stay broken and the weapon beat teaches a self-shooting gun** → gate: PR-4 blocks on the visual held-pose capture matching authored grip.
5. **The signature encounter is a stat fight, not a readable moment** → gate: observe-beat requires the telegraph visible ≥2 s before aggression in PlayMode trace; readability catalog row updated.
6. **Zipline repeats the Undercroft defect (clips terrain, unrideable)** → gate: PR-6 PlayMode clearance assert along the whole cable; device beat in the acceptance session.
7. **The payoff is a log line instead of a visible ship change** → gate: beat 22's acceptance is a VISUAL diff (before/after captures of the berth ship must differ in the decal/cargo regions).
8. **Slice grows sideways** (arena, flight, factory "just this once") → gate: exposure manifest unchanged; any scene addition to the candidate profile fails the Golden build's locked scene list.
9. **Save regressions at odd beats** → gate: the save/relaunch PlayMode matrix samples mid-repair and mid-encounter beats, not just travel boundaries.
10. **CI green mistaken for done** (the pre-recovery disease) → gate: PR-7's completion claim requires the FULL ladder through Clean Package Proof + the Quest acceptance session; the proof-level ledger records `QUEST` or the claim is invalid per `RECOVERY_PROGRAM.md` §3.

---

## 10. HANDOFF

- **Branch:** `fable/first-complete-slice-packet` — this document only. Draft PR open; DO NOT MERGE before the Quest checkpoint verdict.
- **Files created:** `docs/post_recovery/FIRST_COMPLETE_SLICE_PACKET.md` (this file).
- **Evidence inspected:** `docs/first_hour/README.md`, `first_hour_beats.json` (all 22 beats), `BINDING_INVENTORY.md` (binding arithmetic quoted unmodified), `ADAPTER_ENVELOPES.md` (existence), `docs/GAME_PLAN.md`, `docs/PROJECT_COMPLETION_ROADMAP.md`, `docs/recovery/**` (program/verification/checkpoint/exposure), and source: `RillCompanion.cs`, `RillLineAuthor.cs`, `WorldJobLibrary.cs` (W000 spec verified line-by-line, W002 pattern, ToxicCity ownership flagged), `RepairableMachine.cs`, `CreatureRuntime.cs`, `WristScanner.cs`, `ComfortConsoleRuntime.cs` (existence), `HomeHubRuntime.cs`, `FirstHourSurfaceAuthor.cs`, `FirstDestinationHelmRuntime.cs`, `ShipCastOffRuntime.cs`, garden stack (`GardenService.cs`, `GardenPlotRuntime.cs`, `WateringCanRuntime.cs`, `PlantGenetics.cs`), `RewardRouter` usage sites, `SalvageCacheRuntime.cs`.
- **Unresolved questions (Terry):** ① amend the locked 22-beat contract with the optional garden beat pair, yes/no? ② signature creature identity for W001 (taste); ③ VO deferral confirmed (subtitle-only slice)?
- **Unresolved technical (first implementation session):** ToxicCity job-spec ownership (WorldJobLibrary vs legacy builder); whether `ComfortConsoleRuntime` completion is signal-ready (binding says verified-candidate); zipline placement clearance in W001.
- **Recommended first PR after recovery exits:** **PR-1 (the beat tracker + adapters)** — it converts the locked contract from paper to advancing state with zero player-visible risk, unblocks every subsequent beat PR, and its rollback is total (observers passive, director absent = today's behavior).
