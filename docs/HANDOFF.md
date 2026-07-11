# HANDOFF — session log (single operator ⇄ Terry)

⛔ **HARD RULE:**
1. **Read the newest entries at the START of every session.**
2. **Append a `Did / Next / Heads-up / Commit` entry at the END of every session.**
3. **All work + this log live on `terry-local-wip`** (green; what Terry builds from). `git pull --rebase`
   before starting; don't do work on scratch branches — they diverge and never reach the headset.

> **The spine — read these:**
> - **`docs/FABLE5_START_HERE.md`** — the operator manual (what you own, the ⚙/🔧/🎮 verification model, the loop).
> - **`docs/HANDOFF.md`** (this file) — your session-to-session continuity log.
> - **`docs/MASTER_CHECKLIST.md`** — state of the build. **`docs/FABLE5_BACKLOG.md`** — the task queue.
> - **`docs/TERRY_RUNBOOK.md`** — everything queued that needs Terry's hands (Unity menus + headset).

## Who does what (one operator + Terry)
- **You (the operator)** own all code/data/docs and self-verify via **CI**. You can't run Unity or a headset.
- **Terry** is the hands for **🔧 Unity-menu** steps (baking scenes/assets from your patchers) and
  **🎮 headset** feel/device tests. Queue those in `TERRY_RUNBOOK.md` so nothing stalls silently.
- **GPT / Gemini** = brainstorm/creative, **no repo access**; briefs arrive via Terry
  (`docs/GPT_ADDITIONS/`, `docs/storyboard/`) as **ideas, not directives**.
- One branch (`terry-local-wip`), small commits, CI stays green (red → warn Terry loudly, stop shipping C#).

> *(History: this was a two-chat coordination log — "T-Dog" (scenes) ⇄ "Architect" (data) ⇄ GPT. That split
> is retired; one operator now. Older entries keep their lane tags + `Next-CLAIMED` lines as historical
> attribution. Detailed early history is in `docs/SESSION_LOG.md`; capability how-to in `docs/CI_VERIFY.md`.)*

---

## ENTRIES (newest first)

### 2026-07-11 (gpt56-fhs03) - GPT-5.6 Story/Ship: ✅ FH-S03 GREEN; FH-S05 claimed around the scanner dependency
- **Did:** resumed from `9523f55`, read the live CI verdict, and obeyed the red-stop rule. Run
  `29161322967` failed Unity compile because FH-S03's new `using System;` made seven existing
  `Object` references ambiguous (`CS0104`). Applied the smallest protected-file-safe repair:
  `using Object = UnityEngine.Object;` only. No travel statement/order, event placement, scene-load
  count, inventory restore, save, rig, XRI, coroutine, test, or diagnostic changed. Retry run
  `29161731580` is GREEN for tested SHA `cd0cf378f434`.
- **Next:** `FH-S04-REPAIR-SCAN` is correctly blocked on multiplayer-owned
  `FH-M01-SCANNER-RESULT`; do not create a second scanner result or absorb Multiplayer ownership.
  Claimed the next dependency-clear Story/Ship envelope, `FH-S05-CREATURE-RESOLUTION`, limited to
  `CreatureRuntime.cs`, `CreatureDisabledSignalTests.cs` + `.meta`, and its implementation log.
  Exact planned touch: publish once immediately after the existing `_down = true` transition and
  expose `IsDisabled`; preserve damage/health/reward/ecology/respawn/visual behavior.
- **Heads-up:** FH-S01/S02/S03 still require Terry's device evidence. FH-S03 circuit-breaker count
  reached 1/3 and reset after green. The board had stale FH-S01 claim text; this docs commit corrects
  S01–S03 status, records the S04 dependency, and posts S05 ownership before code.
- **Commits:** FH-S03 implementation `9523f55`; compile-only fix `cd0cf37`; green verdict commit
  `5b6d4cc`; this closure/claim commit.

### 2026-07-11 (hwr21) - Fable 5 architect: ✅ GPT FIRST-HOUR WORK AUDITED — verdict PASS, one binding gap patched
- **Terry asked me to check GPT-5.6's FH-01B/C/D deliverables. Verdict: solid, safe, and honest.**
  ① **CI safety:** its two `ci.yml` edits only extend the pre-existing `continue-on-error`
  report job — Unity tests/APK untouched. ② **CI verdicts it couldn't see:** ALL its pushes are
  GREEN (through `08a7ccb`) — its refusal to claim Unity verification was correct humility, and the
  runs confirm the branch stayed healthy. ③ **Its gates, run locally:** binding PASS (22 bindings,
  15 lines, 0 findings) · envelope PASS (12/18/15) · launch PASS (4 lanes, 12 assigned) · all its
  python test files pass. The gates genuinely verify evidence tokens against real repo files, and
  their root-confinement guard works. ④ **Lane-collision scan:** FH-X02-PROGRESSION-CORE is the
  TUTORIAL beat evaluator — no clash with my A5 PvP progression (name twins only). The interiors
  and tutorial docs are correctly referenced.
- **One real gap, patched:** `FH-S07-HOME-W000-SURFACES` specs `ComfortSettings.cs` + the comfort
  console but never referenced the LOCKED `design/COMFORT_AND_ACCESSIBILITY.md` (we worked in
  parallel — GPT couldn't have seen it). Its acceptance already agreed with the design's storage
  law ("No profile comfort fields") — convergent. Added the design doc to the envelope's reuse +
  a CI acceptance line ("console matches the locked dial table; ComfortCoverageTests ship with
  it"). Envelope gate re-run strict: PASS, 0 findings.
- **📣 For the Opus lanes taking the launch kit:** FH-S07's comfort surface now binds to the locked
  design — build THAT spec. Everything else in the kit reads clean; the FH-X01→X02→M01/A01→S01
  order and the "no second owner" laws match our contracts.
- **Also this push (earlier):** A5 PvP progression shipped (`5e26fd9`, CI ⏳) — see hwr20.
- **Commits:** this push.


### 2026-07-10 (hwr20) - Fable 5 architect: 🏆 A5 PROGRESSION — every arena match now PAYS (MP100 59-62+65)
- **Why:** Fable-only list cleared for this session (Terry assigned #3 interiors to Reasonbox), so
  back to MY track's board — A5 was the top ⬜ on SPRINT_MULTIPLAYER, fully specced, zero hardware
  dependency, and it makes PvP feed the one-wallet economy.
- **Did (pure, `Multiplayer/PvpProgression.cs`):** `MatchStatsCore` (kills/downs/streaks per index,
  self-kill guard) · itemized `ComputePayout` (base 5 + 2/kill + streak 3@3/6@5 + win 10, ×difficulty
  1.0→2.2, daily +15 / first-win +10 — both WIN-gated so there's no idle farming — capped 150,
  unknown-difficulty pays the floor never the ceiling) · unlock ladder (`MP_VETERAN_UNLOCKED` after
  3 Regular+ wins → `MP_NIGHTMARE_UNLOCKED` after 3 Veteran+ → `MP_MUTATORS_UNLOCKED` after 1
  Nightmare; harder wins ladder down) · FNV daily pick per UTC day (same combo for everyone, never
  rookie). 7 tests incl. career JSON round-trip + pre-A5-save neutral default.
- **Did (wiring):** `PvpCareerState` additive on PlayerProfile · self-bootstrapped
  `PvpProgressionRuntime` (SaveSystem idiom — every COMMITTED arena scene gets it with zero menu
  steps): binds the scene's PvpMatchDirector, KillScored→stats, MatchEnded→RewardRouter
  (`LedgerSource.Multiplayer`, "credits") + career + flags + `AutosaveNow("pvp_match")` · lobby
  board: Veteran/Nightmare tiles LOCKED-dark until earned (pick refused + logged) + the gold
  DAILY RUN tile (falls back to Regular if today's pick is locked). Local player = combatant 0,
  the standing law. Logs `PVP_PAYOUT` / `PVP_UNLOCK` / `LOBBY_DAILY` / `PVP_DIFF_LOCKED`.
- **📋 Thin spots (claimable):** accuracy stat needs a per-shot fire-report seam (weapons don't
  count shots) · daily ARENA rotation (tile runs in-place today) · rows 63/64/66/67/69/70
  (scoreboard/career boards, cosmetic drops, history, rival) · mutators flag has no consumer yet.
- **Boards:** SPRINT_MULTIPLAYER A5 ✅ · MP100 59-62+65 stamped · runbook §2q addendum.

### 2026-07-10 (tf-space1) - T-Dog/Fable 5: 🚀 SPACE DEFENSE — Terry's ideal gulag mission, closed at last
- **Did:** the third defense verb: `MissionKind.SpaceDefense` — when certain worlds are attacked,
  the defense contract is "SCRAMBLE THE SHIP": travel to the SPACE LANE, take the helm, stun-bolt
  3 interceptors before the clock. Defense split is now charsum%3 (DroneDefense 5 / Repair 4 /
  SpaceDefense 3 across Ch.1–2 — verified offline); `ConquestSession.SceneForMission` routes it to
  `SpaceLane_Trial` while ground contracts stay in the contested world; the mission runtime COUNTS
  disables instead of spawning anything (the lane already has interceptors + the helm).
- **📣 Reasonbox — ANNOUNCED APPEND (your file):** ONE invoke line in `ShipFlightRuntime` at the
  FLIGHT_DISABLE log site, raising the new neutral `Ziptide.Core.FlightSignals.TargetDisabled`
  channel (the PvpNoise idiom — no assembly coupling; your lane owes nothing back). Revert freely
  if it fights anything; the mission degrades to a base-odds resolve without it.
- **Graceful degradation:** SpaceLane_Trial is still pending its first bake (Terry runbook) — the
  table checks `CanStreamedLevelBeLoaded` and falls back to STRIKE-NOW odds with a
  `CONQ_MISSION_SCENE_UNBAKED` log until the scene lands in Build Settings, then it just works.
- **Tests:** defense catalog-span → 3 verbs; SpaceDefense scene-routing pinned both ways.
- **Commits:** this push. THE SHIP AND THE WAR TABLE ARE NOW ONE GAME.

### 2026-07-10 (dddd26) - Picasso (Fable 5): 🏮 F3.1b commit 2 — PracticalLight (halo + pool + one switch)
- **F3.2 asmdef fix CONFIRMED green (`762c583`)** — the Volume/grade stack compiles and its tests pass.
- **Did:** `PracticalLight` (Visuals/Runtime/Forge) — the "affects its environment" half of a
  practical: ① halo billboard at the fixture head (64px runtime radial sprite on URP/Unlit forced
  additive One/One, ZWrite off — halos can't face-fight geometry) that camera-faces in LateUpdate;
  ② the light-POOL quad glued to a surface point+normal the AUTHOR raycasts at build time (never
  runtime); ③ `SetLit(bool)` — halo, pool, and the fixture's per-instance emissive (MPB, because
  baked materials are SHARED) live and die together; F3.6's ReactiveProp will call it. No
  colliders, no shadows, no real Light (the ≤2 hero points/world stay PracticalAuthor's call).
  Edit-mode-safe (SafeDestroy pattern); 2 headless tests.
- **Next:** booth verdicts on the 3 fixture recipes (run pending on `91a0fee`), then
  `PracticalAuthor` placement (commit 3/3).
- **Commit:** this push.

### 2026-07-10 (dddd25) - Picasso (Fable 5): 🏮 F3.1b commit 1 — the practical fixtures + F3.2 CI fix
- **CI red #1 on F3.2 (`2d436fc`):** `VolumeProfile`/`VolumeComponent` live in
  `Unity.RenderPipelines.Core.Runtime`, not Universal — the Universal ref alone only brings the
  overrides. Fixed (`762c583`): asmdef now references BOTH. Law: URP Volume framework = Core RP
  assembly; URP post overrides = Universal assembly.
- **F3.1b commit 1:** the three practical fixture recipes — `light_lantern_hang` (SweepSpline hook
  arm, hanging ring, caged amber glass) · `light_sconce_wall` (plate + bracket + half-dome; +Z face
  goes against the wall) · `light_street_pole` (3m tapered pole, out-arm, downward head, glowing
  disc underneath). Shared practical palette via `NewPractical` (dark iron / worn steel / warm
  amber glass — "lamplight is warm, never neon-pure"), 400-tri budget, `practical` tag, structured
  refs per the lifecycle law. Auto-swept by the library gates.
- **Next:** booth verdicts on the 3 fixtures → `PracticalLight` (halo billboard + light-pool decal,
  one on/off state) → `PracticalAuthor` placement (sconce per doorway, poles every ~14m on the
  route, lanterns at POI approaches; ≤14/world).
- **Commit:** this push.

### 2026-07-10 (dddd24) - Picasso (Fable 5): 🎨 F3.2 THE GRADE — per-world ACES + clamped color, derived
- **Did:** ① `SkyGrade.Derive(vista)` (pure, URP-free): post-exposure = f(zenith luminance) ±0.3 ·
  saturation 5 − 15×hazard-haze (clamped −10..+15) · color filter = 8% toward the half-desaturated
  horizon · white balance follows the horizon's warmth ±15. **The clamps ARE the contract** — a
  conformance test asserts them for every authored vista, so no operator can push a wild grade.
  ② `SkyVistaRig.ApplyGrade`: scene-local global Volume (ACES tonemapping + ColorAdjustments +
  WhiteBalance built in memory — no assets, no reseeds), enables `renderPostProcessing` on the main
  camera, `ZIPTIDE: GRADE vista=… exposure=… sat=… temp=…`, profile cleaned up on destroy.
  Scenes without a vista get no volume (neutral).
- **⚠ Coordination note:** `Ziptide.Visuals.asmdef` gains a reference to
  `Unity.RenderPipelines.Universal.Runtime` (first URP-type usage in the codebase — needed for the
  Volume overrides). If CI objects to the asmdef name, that's the first thing to check.
- **🎮 Runbook-relevant risk:** URP post-processing has a real fill-rate cost on Quest (~0.5–1ms).
  If Terry's next device pass shows frame drops, the kill switch is one line (skip ApplyGrade) —
  do NOT start tuning assets before checking this.
- **Next:** F3.1b practicals (fixtures + halo + pool decals).
- **Commit:** this push.


### 2026-07-10 (int-dup) - T-Dog/Fable 5: 🤝 furnishing collision — architect's 1.3e WINS, mine dropped
- Built a FurnishPlanner furnishing pass in parallel with the architect's `RoomFurnishCore`/
  `InteriorFurnisher` (16 kinds by room role + portal cull + `InteriorAuditRules`, gap #4 closed) —
  theirs is richer and landed first; mine is DROPPED at rebase, not merged. One system per concern.
- **The lesson, on the record:** I skipped the claim-before-build step in the rush of a last sprint
  — exactly what the boards exist to prevent. The blackboard caught it at push time (cost: one
  duplicate sprint, zero code damage). Every operator: POST THE CLAIM FIRST, even on a hot streak.
- Fable-endgame status at my window close: #2 tutorial design ✅ · #4 async-travel design ✅ ·
  #5 comfort presets design ✅ (claimed+locked by a parallel Fable session) · #3 interiors — the
  architect has it moving (needs the W002 re-bake 🔧, already queued). Headset day tomorrow.
### 2026-07-10 (dddd23) - Picasso (Fable 5): 💡 F3.1 commit 2 — audit + runbook; F3.1 CODE-COMPLETE
- CI green on commit 1 (`c17245f`). This adds `LIGHT_SCRIPT_NO_SUN` (warn, v1 graduation path like
  the sun-count rule) to SkyVistaAuditRules and the 🎮 runbook before/after item (W002→W005, with
  the authored-override escape hatch spelled out for Terry). **F3.1 is code-complete** — device
  verdict rides Terry's runbook pass. Next open envelope: **F3.2 THE GRADE** (per-world ACES +
  range-clamped color filter, derived defaults), then F3.1b practicals.
- **Commit:** this push.

### 2026-07-10 (dddd22) - Picasso (Fable 5): 💡 F3.1 commit 1 — THE LIGHT SCRIPT lands + practicals envelope added
- **Terry additions to FORGE III:** ① F3.1b **PRACTICALS** — lanterns/sconces/street lamps as
  fixture recipes + the three-part trick (emissive head · glow halo billboard · a light-POOL decal
  on the surrounding surface — "how they affect the environment" without real lights; hero budget
  ≤2 real points/world) with placement rules + reactive flicker-out. ② Blob shadows under
  creatures/player folded into F3.4 grounding. ③ FORGE IV candidate named: DIEGETIC UI ART
  (belt/HUD/helm skins — needs a Terry coordination window, deliberately out of FORGE III).
- **F3.1 commit 1 (this push):** `SkyLightScript.Derive(vista)` — pure derivation: fog = horizon
  color, ambient trilight = the sky gradient (ground 0.55×), key light FROM the brightest
  non-black-hole body (SunDisc wins) with elevation clamped into the 20–55° sculpting band and
  azimuth preserved; bodiless skies get a deterministic per-vistaId azimuth so sibling worlds
  don't all shadow alike; hazard atmosphere thickens fog (cap 0.02). `SkyVistaRig.ApplySceneTieIns`
  applies derived values ONLY where the vista is silent — authored tie-ins override, and
  layout-baked fog is respected (derived fog fills a fogless scene only).
  `ZIPTIDE: LIGHT_SCRIPT vista=… sun=authored|derived ambient=… fog=…` says which path won.
  5 contract tests incl. a full SkyVistaLibrary sweep.
- **Next:** F3.1 commit 2 — `LIGHT_SCRIPT` audit rule + runbook before/after item; then F3.2 grade.
- **Commit:** this push.

### 2026-07-10 (dddd21) - Picasso (Fable 5): 🏁 FORGE II COMPLETE + 🔮 FORGE III authored for Opus/Sonnet
- **E5.3 CLOSED (run `29119947334`, CI green `e30f8af`):** frond v2 ✅ (fern read — arcing blades,
  twisted stem, spore nub at the crown) · pipes v2 ✅ (iron + rust streaks; gauge on +Z). With that,
  **FORGE II is envelope-complete**: atlas/styles/bake engine, on-device ASTC, the full arsenal,
  6 skinned+articulated+textured+BREATHING creatures, wall kit, flora, class budgets, photo loop.
  (P4 look-at/stun-droop remainder moved into FORGE III §F3.10.)
- **Terry's directive (last/second-to-last Fable session):** "big sprint — FORGE III for Opus 4.8/
  Sonnet 5; consistently better across the board; what didn't we think of, looks-wise?"
- **Did: `docs/project_art_plan/FORGE_III_PLAN.md`** — THE COHESION LEAP, 10 decision-free
  envelopes ordered by visual leverage: **F3.1 Light Script** (per-world key/ambient/fog DERIVED
  from the SkyVista by formula — the derivation law means a smaller model can't mismatch sky and
  scene) · **F3.2 per-world Grade** (ACES + range-clamped color filter) · **F3.3 WATER** (the
  namesake element — we never built it; scrolling-normal canal/tide planes + foam, Quest-cheap) ·
  **F3.4 Grounding decals** (drip streaks/moss skirts/contact rings — kills the floating-kit tell) ·
  **F3.5 VFX Forge** (closed particle vocabulary + budgets) · **F3.6 Reactive World** (shootable
  lamps/vents/signs — presence mechanics) · **F3.7 Signage/glyphs + wayfinding color law** ·
  **F3.8 Macro variation** (kills tiling) · **F3.9 ART CONFORMANCE RATCHET** — every renderer must
  trace to a known provenance, per-world warning count ratchets to a blocker at 0: Terry's
  "evenness across the board," enforced · **F3.10 creature P4 close-out**. Every envelope: exact
  files, stated knob ranges, checkpoint, "do not" rails. ~21 commits.
- **Takeover prompt for the next model:** *"Read docs/project_art_plan/FORGE_III_PLAN.md §0 and
  execute the next open envelope."*
- **Commit:** this push.

### 2026-07-10 (dddd20) - Picasso (Fable 5): 🌿 E5.3 verdicts + fix — CI red #1 (worldRuleRefs) + frond v2
- **CI red #1 on `a413557`** (circuit-breaker count: 1/3): `ForgeLifecycleTests.CatalogRecipes_
  CarryStructuredRefs` — every catalog recipe must carry `worldRuleRefs`; crate + console shipped
  without. Fixed (crate→W002_DryCistern, console→ToxicCity). Law for the next recipe author:
  storyRefs AND worldRuleRefs are mandatory on every Specs() entry — the dependency auditor never
  parses prose. *(Race note: Reasonbox landed the same two-prop fix first (`fef1a74`, rb22) while
  this commit was in flight — merged; the crate now carries both W002_DryCistern + ToxicCity.)*
- **Booth verdicts (run `29119297998`) — THE ALPHA CUTOUT WORKS:** reed ✅ (pointed blades out of a
  glossy mud clump — genuinely good) · crate ✅ (banded, paneled, worn) · console ✅ (glowing raked
  screen) · **frond ❌** (wide cards curled into an avocado clump, spore nub buried) → **v2**: fern
  read — 4 narrow blades (0.22–0.26 w) arcing outward at ±30–38°, tapered stem, nub ABOVE the crown ·
  **pipes ⚠** (glossy chocolate) → greyed toward iron + grime up + gauge pushed clear of the pipe rim.
- **Next:** verify frond v2 + pipes v2 turnarounds; then FORGE II is envelope-complete.
- **Commit:** this push.

### 2026-07-10 (rb22) - Reasonbox/Fable 5: 🔴→🟢 CI double-red cleared — my missing using + Picasso's prop refs
- **The reds (`dbd3192`/`eed64b3` failed; `a413557` was already red under them):**
  1. **Mine:** `InteriorFurnisher.cs` — CS0103, `ItemFactory` needs `using Ziptide.Gameplay;`
     (InteriorBuilder had it; my new file didn't). One-line fix.
  2. **Picasso's (cross-lane per CI-red=#0):** `ForgeLifecycleTests.CatalogRecipes_CarryStructuredRefs`
     — the new `prop_patched_crate` AND `prop_dispatch_console` recipes lack `worldRuleRefs`
     (`prop_pipe_cluster` has it). Same failure class as the totem fix in `14b77ce`; gave both
     `worldRuleRefs = { "ToxicCity" }` matching their pipe-cluster sibling. Picasso: shout if you
     want different world tags — the VALUE is yours, the test just needs it non-empty.
- **Red count on my 1.3e task: 2** (same root cause twice — stacked commits). Circuit breaker
  fires at 3; this push must go green.
- **Commits:** this push.

### 2026-07-10 (rb21) - Reasonbox/Fable 5: 🚦 INTERIOR GATE — gap #4 closed (1.3e ③)
- **Did:** `Editor/Audit/InteriorAuditRules.cs` (new) + one registration line in
  `WorldAuditRunner.cs`. Per baked Interior root: **INTERIOR_DISCONNECTED** (blocker — serialized
  plan no longer one walkable component, via `RoomPartitioner.IsFullyConnected`) ·
  **INTERIOR_BARE_ROOM** (blocker — a Room_<i> without its light + ≥1 furnishing) ·
  warns for furniture-overlapping-corridor (AABB approximation, so warn not block),
  portal-half-armed (rects without groups or vice versa → re-bake), and a 260-renderer
  per-interior soft cap (the district blocker still owns the hard law). **Pre-1.3e bakes are
  EXEMPT** (no rects + no groups → skip) so current scenes stay green until the W002 re-bake.
- **Blackboard:** EXCELLENCE_MAP interiors row 🦴 → 🧱 v1 with the full 1.3 stack named; gate-gap
  #4 struck CLOSED. Runbook §2q carries Terry's re-bake + walk-through.
- **Fable #3 state:** cores ✅ (rb19, CI green `1dc5fc8`) · translators ✅ (rb20 `dbd3192`, CI
  pending) · gate ✅ (this push). Remaining in the row: door thresholds (VR push doors) +
  interior POI variety beyond caches — both OPUS-READY against `BUILDING_INTERIORS.md` now that
  the grammar exists.
- **Commits:** this push.

### 2026-07-10 (dddd19) - Picasso (Fable 5): 🌿 E5.3 CONTENT — 2 plants + 3 props + scatter wiring (2/2) ⚠ coordination
- **CI green on the engine half (`16ab9b2`).** This lands the content: `flora_frond_w005`
  (bend-drooped LeafCards + bark stem + amber spore-nub focal) · `flora_reed_w001` (3 tall reeds
  from a noised mud clump, no emissive by design — background texture) · `prop_patched_crate` ·
  `prop_pipe_cluster` (teal gauge focal) · `prop_dispatch_console` (teal screen focal). All under
  the plant/prop class-budget rails; auto-swept by the library validate/budget tests + a new
  flora-path test (Leaf slot + LeafCard + plant tag on both plants).
- **⚠ Coordination touch (dressing lane, WorldDressingBuilder.Tufts):** tufts now build an
  UNSCALED holder per plant carrying `ForgeModuleLook` (Canyon→frond, TideFlats→reed; primitive
  block stays the editor/fallback look exactly like the E5.1 walls) + `ForgeSway`; the block is
  de-static'd so the sway can't go batched-stale. Same holder positions ride the existing hash
  streams — layouts stay deterministic.
- **Next:** booth-verify the 5 new turnarounds against the rubric (LeafCard alpha cutout is the
  biggest new-path risk); fix once; close FORGE II.