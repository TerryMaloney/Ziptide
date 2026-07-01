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

### 2026-06-29 (ww) — Architect: 🔀 RETIRED the two-chat split → single-operator handoff (docs-only)
Terry's call: Fable 5 will likely get only ~one usable prompt, so we consolidated **everything to a single
operator** ("Architect" = whatever one model drives) — one prompt should go straight to game work, not
meta-setup. The two-lane apparatus (Architect vs T-Dog, `[A]`/`[T]`, claim-before-build) is **retired**.
- **The reframe (the important part):** the real division of labor was never Architect-vs-T-Dog — it's
  **who can verify a change**: **⚙CI** (you write + self-verify: all C#, data assets, docs) · **🔧UNITY** (you
  write the patcher; Terry runs a menu to bake the scene/asset) · **🎮DEVICE** (feel/geometry/perf — Terry on
  headset). Scenes are authored by **patcher-indirection** (write C# `ScenePatcher*` → Terry clicks
  `Ziptide → …` → commits the generated `.unity`/`.asset`).
- **Rewrote:** `FABLE5_START_HERE.md` (→ single-operator manual), `ROLES.md` (→ the ⚙/🔧/🎮 operating model +
  menu list + patcher recipe), `FABLE5_BACKLOG.md` (re-tagged every task by verification class, dropped claim
  rules), this HANDOFF preamble. **NEW `TERRY_RUNBOOK.md`** = the always-current batch of Unity-menu + headset
  steps — reconciled with your (vv/uu): the **Dev World Manifest is auto-rebuilt by the build**, not a manual step.
  Pointer fixes in `CLAUDE.md`, `MODULE_MAP.md`, `DEVICE_TEST_CHECKLIST.md`.
- **Left alone:** historical logs + `*(T-Dog)*`/`*(Architect)*` attribution credits — harmless history. No
  code/scenes/build touched — pure docs, CI green by construction. *(Relabeled uu→ww; you'd used uu/vv already.)*
- **State (all CI-green):** economy into world-entry · story-flag gating + travel-door lock · `WORLD_DATA.md`
  (W000–W012). **Top of the real critical path = Terry's device-test pass** (`DEVICE_TEST_CHECKLIST.md` +
  `TERRY_RUNBOOK.md`). No forced "next task" — next model reads `FABLE5_BACKLOG.md`, takes the top item it can move.
- **Next:** none claimed — the handoff itself is the deliverable. Whoever's next: read `FABLE5_START_HERE.md`.
- **Commit:** this push on `terry-local-wip`.

### 2026-06-29 (vv) — T-Dog (cloud): device-test round 3 (right-hand turn, stuck-slow walk, self-shoot, bot collision)
Terry's round-3 device feedback cleared in 3 CI-green commits (`39e9fc8`, `afd0aa7`, `509ad90`).
- **Right-handed grab couldn't turn:** the XRI `ActionBasedControllerManager.OnRaySelectEntered` disabled
  Turn+Move on grab (stock behavior to avoid turn/anchor conflict). Since we now globally kill anchor control,
  there's no conflict — edited it to keep Turn+Move live on grab (only suppress teleport). **Note: this edits a
  Starter Assets sample script** (`Samples/.../ActionBasedControllerManager.cs`) — flagged so a sample re-import
  doesn't silently revert it.
- **Walking permanently slow after PvP/stun:** `PlayerStunReceiver` cached base move speed from the live
  (already-slowed) value behind a flag → a stun straddling a scene load latched the reduced value as "base."
  Rewrote to capture true base once (while unslowed) and always set `moveSpeed = base * SlowFactor` (self-heals).
- **Gravity gun "shoots myself":** muzzle ray's first hit was the wielder's own rig (an IPvpDamageable in PvP) →
  skip the player rig in the raycast. **PvP bot phased through walls:** added `CollideMove` (SphereCast clamp)
  mirroring `DroneCombatBehavior` — reinforced the universal "nothing moves through solids" rule.
- **Rays:** shortened drawn line 2.5→1.4m, disabled endpoint-snap (`m_SnapEndpointIfAvailable`), enabled
  force-grab (`m_UseForceGrab`) so grabbed objects reel to the hand. **Bricks:** 6x5→8x6, 2→4 hits. **Bot bolt:**
  smaller. **Credits HUD:** smaller + true lower-left. **Wrist locator:** resolve hands from the two
  `ActionBasedController`s by side (was picking two interactors on the same hand). **Legacy D0 taser snap:** set
  `m_AttachTransform` inside the SerializedObject block (the post-Apply public assignment wasn't baking).
- **Heads-up (Architect):** none of your data lane touched. One shared/third-party file edited (the XRI sample
  controller manager) — documented above.
- **On Terry's plate:** turn-while-holding (right hand), no stuck-slow after PvP, gravity gun doesn't self-hit,
  bot respects walls, wrist locator fires (`WRIST_HANDS`), legacy taser snaps.
- **Commits:** `39e9fc8`, `afd0aa7`, `509ad90` on `terry-local-wip`.

### 2026-06-28 (uu) — T-Dog (cloud): device-test round 2 fix pass (rig anchor ROOT-CAUSED, PvP feel, economy HUD)
Terry tested the build and gave round-2 feedback; cleared the list in 4 small CI-green commits.
- ⭐ **THE PERSISTENT BUG finally root-caused** (commit `89eac53`): the thumbstick rotated the held gun/hammer
  because the disable code reflected **`m_EnableAnchorControl`** — a field that **does not exist** on
  `XRRayInteractor`, so `GetField` returned null and the gate was **never set** (silent no-op every round).
  Real field is **`m_AllowAnchorControl`** (confirmed in the XRI prefabs). Fix in `PlayerRigPersistence.EnsureXRIWiring()`:
  `DisableAnchorControl` now sets `m_AllowAnchorControl=false` on **every** ray (incl. inactive teleport ray)
  + new `DisableAnchorInputActions()` `.Disable()`s the "Rotate/Translate Anchor" actions; logs
  `ANCHOR_ACTIONS_DISABLED` / `ANCHOR_FIELD_MISSING`. Also fixed the **ray length jump** (clamp ALL
  `XRInteractorLineVisual`, not just the active ray). Full write-up in `docs/systems/VR_RIG_GOTCHAS.md` #1/#2.
- **PvP feel** (commit `492e494`): bot now fires a **visible, slow, dodgeable `PvpBolt`** (new file) instead
  of an instant hitscan; **breakable walls** reworked to a fine **per-brick HP grid** (localized damage at the
  hit point, ~2 swings/brick) — `WallState` is now a pure brick grid (**`WallStateTests` rewritten**); PvP HUD
  repositioned to stay readable in the FOV.
- **Build/menu + economy** (commit `9944f90`): `BuildAndroid.PatchScenesThenAPK` now calls
  `DevWorldManifestBuilder.Rebuild()` **last** (after the D0 rename) → fixes the "two Toxic City" menu entries
  with no manual ordering. New **`CreditsHud`** (rig-ensured) shows "CR <n>" lower-left in every world.
- **Docs:** `DEVICE_TEST_CHECKLIST.md` (logcat fix `adb logcat | findstr "ZIPTIDE"`; manual manifest step
  dropped; new verify items), `VR_RIG_GOTCHAS.md` (#1/#2 corrected).
- **Heads-up (Architect):** the breakable-wall model changed from the 3-stage `WallStage` enum to a per-brick
  grid — when the **PvP netcode wall-sync** message-model is built, serialize the brick grid, not the old
  `WallMsg` 3-stage codes (the (ee) note's `WallStage` mapping is now stale). No code consumes `WallStage` anymore.
- **On Terry's plate (can't headset-test from here):** the §1 anchor test with BOTH guns AND hammer; visible
  bot bolt is dodgeable; bricks chip locally over multiple swings; one "Toxic City" in the menu; credits show + rise.
- **Next-CLAIMED (T-Dog):** none in flight — pausing for Terry's device verify of this round. (The CityBuilder
  CITY_DESIGN P0 pass claimed in (tt) is still mine when work resumes.)
- **Commits:** `89eac53`, `492e494`, `9944f90` on `terry-local-wip`.

### 2026-06-27 (tt) — T-Dog (cloud): picked up the data lane handoff — travel-door story-gating (CI-green)
Took your (ss) handoff. Did the top code-able `[T]` item — **enforce `flagsRequired` at the travel doors**
(your #2; the WorldGating check was ready). CI-green (`aa6ed89`).
- **`WorldTravelStation`**: per destination, `WorldGating.MeetsRequirements(pack, SaveSystem.Instance.Profile)`
  → unmet renders a **LOCKED door** (visible, dark-red, **no travel listener** = not enterable). Logs
  `ZIPTIDE: TRAVEL_LOCKED pack=… missing=…`. **`TravelCoordinator` untouched** → reversible / report-only
  per the locked travel contract. Packs with empty `flagsRequired` (all current) stay unlocked → no regression.
- **Backlog:** marked the Phase-B `[T]` "Enforce flagsRequired" item done-pending-device-verify.
- **Known gap (follow-up):** the `ProximityTravelTrigger` walk-through failsafe bypasses the gate — but it
  only ever points at the exit/test-room today, not gated worlds, so it's harmless until gated worlds exist.
- **On Terry's plate (can't headset-test from here):** confirm Architect's economy/gating on device
  (`ZIPTIDE: ECON_RESOLVE` + bounty pays + `WORLD_FLAGS_GRANTED`), and the still-open "can't run in
  ToxicCity" re-test.
- **Next-CLAIMED (T-Dog):** the **CityBuilder CITY_DESIGN P0 quality pass** (ground floors / height-stepping /
  palette zoning using existing `DistrictDef` fields — no schema dep), since that most improves what Terry
  sees; will post specifics before editing `CityBuilder`. *(WorldStubGenerator waits on Architect's per-world
  `CityLayoutDefinition` authoring; the Transmission de-garble UI waits on Architect's `[A]` fragment flags/service.)*
- **Commit:** `aa6ed89` on `terry-local-wip`.

### 2026-06-27 (ss) — Architect: ⏸ PAUSING the data lane — T-Dog, your move (state + your action items)
Terry asked me to pause Architect and let you (T-Dog) pick it up from the scene/runtime side. Three `[A]`
tasks shipped this stretch, **all CI-green** — so the data/backend is solid and ready for you to build on:
- ✅ `WORLD_DATA.md` — prose→WorldPack serialization (W000–W012 + record format + Transmission flag spec).
- ✅ **Idle/offline economy wired into world-entry** (`ProfileEconomy.EnterWorld` ← `WorldRuntime.Start`,
  keyed by scene name). Phase-A #1 blocker — done. Logs `ZIPTIDE: ECON_RESOLVE`.
- ✅ **`WorldPackDefinition.flagsRequired/flagsGranted` + `WorldGating`** + grant wired into
  `JobDirector.OnJobCompleted`. Logs `ZIPTIDE: WORLD_FLAGS_GRANTED` / `WORLD_LOCKED`.

**Your `[T]` items I teed up for you (in `FABLE5_BACKLOG.md`):**
1. **Verify the Phase-A wiring on device** — enter a world, confirm bounty pays + `ZIPTIDE: ECON_RESOLVE`
   fires (logcat). This is the on-device confirm for my economy/gating work — I can't headset-test it.
2. **Enforce `flagsRequired` at the travel/offer UI** (`WorldTravelStation`/`DispatchKiosk`) — call
   `WorldGating.MeetsRequirements(pack, profile)` to hide/lock worlds whose prereqs aren't met. The check
   is ready; I left the actual gate to you because it touches the **locked travel contract** (report-only).
3. Your standing Phase-C runtime items: **`WorldStubGenerator`** (unblocks mass worlds), `CityBuilder`
   ground-floors/palette passes (CITY_DESIGN P0–P2), the Transmission **de-garble UI** (`THE_TRANSMISSION` §10).
4. Still open from your (kk): the "can't run in ToxicCity" on-device re-test (input fix shipped; if still
   wall-blocked, widen streets).

**No collisions:** I touched only `Core/Economy`, `Content/WorldPacks`, `Content/Jobs`(reward path),
`Gameplay/WorldRuntime.cs` + `Gameplay/Jobs/JobDirector.cs` (the grant hook), tests, and docs. I did **not**
touch `TravelCoordinator`, scenes, the rig, patchers, or any UI. **Architect lane is parked** — I'll claim
before resuming. Over to you.
- **Commit:** docs-only (this entry) on `terry-local-wip`.

### 2026-06-27 (rr) — Architect: `WorldPackDefinition` story-flag fields + `WorldGating` (closes the schema gap)
Closed the schema gap I surfaced in (pp)/`WORLD_DATA.md` §1 — the one thing blocking faithful serialization
of all 80 worlds. (Economy wiring (qq) is **CI-green**, run #138.)
- **Did:** (1) added `flagsRequired` + `flagsGranted` (`List<string>`, default-empty → old assets unchanged)
  to `WorldPackDefinition`. (2) NEW pure `WorldGating` helper (`Content/Runtime/WorldPacks/WorldGating.cs`) —
  `MeetsRequirements` / `FirstMissingRequirement` / `GrantWorldFlags` (null-safe, idempotent, fail-closed on
  a real requirement w/ null profile). (3) wired `JobDirector.OnJobCompleted` → `GrantWorldFlags(worldPack,
  profile)` after the per-job reward, so a world's RILL beat + Signal threshold + `W###_COMPLETE` all land on
  contract completion (the flags one `completionFlag` couldn't carry). Logs `ZIPTIDE: WORLD_FLAGS_GRANTED`;
  `JobDirector.Start` logs `ZIPTIDE: WORLD_LOCKED` (non-blocking) if entered without prereqs. (4) 11 new
  EditMode tests (`WorldGatingTests`). Created `.meta`s for the 2 new files.
- **Deliberately NOT done (your lane, T-Dog):** *enforcing* `flagsRequired` — i.e. don't offer/allow **travel**
  to a locked world. That belongs at the travel/offer UI (`WorldTravelStation`/`DispatchKiosk`) and touches
  the **locked travel contract**, so per CLAUDE.md it's report-only. The check (`MeetsRequirements`) is ready
  for the UI to call; queued in the backlog as a `[T]` task. I did not modify `TravelCoordinator`.
- **Heads-up (you / Terry):** noted in passing — there's **no `NarrativeSaveSystem`**; flags live in
  `PlayerProfile.flags` (via `SaveSystem`), which is what `WorldGating`/`JobRewards` write. The
  `WORLD_DATA.md` §1 doc reference to `NarrativeSaveSystem` was aspirational; real store is the profile.
- **Next-CLAIMED:** verifying this on CI; if green, I'll pick the next Phase-B `[A]` (likely `WorldAuditRunner`
  self-tests or `ItemFactory` IL2CPP-safety). Will post a claim first.
- **Commit:** this push on `terry-local-wip` (CI pending).

### 2026-06-27 (qq) — Architect: wired `ProfileEconomy.ResolveWorld` into world-entry (Phase-A top blocker)
Terry cleared me to take this while T-Dog's offline. **The missing economy link** (CODE_SCORE's #1
blocker): the idle/offline economy was fully built + tested but **never called at runtime**. Now it is.
- **Did:** (1) NEW pure entry-point `ProfileEconomy.EnterWorld(profile, worldId, nowUnix, maxOfflineSeconds=0)`
  — ensures the world's `WorldState` exists, marks it `discovered`, then runs `ResolveWorld` (offline mine/
  garden accrual). Pure/no-Unity → EditMode-testable. (2) `WorldRuntime.Start()` now calls it on entry
  **before** the visual-profile guard (economy is world identity, not visuals), keyed by **`gameObject.scene.name`**
  (== `WorldPackDefinition.sceneName`, the stable runtime world id — established the convention since nothing
  used `GetWorld` at runtime yet). Logs `ZIPTIDE: ECON_RESOLVE world=… mines=… produced=… plotsReady=…`.
  (3) 3 new EditMode tests (resolve+discovered, first-visit creates/no-production, null-safe).
- **Why this hook:** `WorldRuntime` runs in every world scene (never `_Boot`) on load incl. post-travel —
  the canonical per-world entry. Uncapped offline time is safe: per-mine `storageCap` bounds accrual.
- **Heads-up (you / Terry):** (1) the `WorldResolveResult` is logged but there's **no welcome-back UI yet**
  — that's `[T]` (your lane) when you want it; the data's ready. (2) `maxOfflineSeconds` is 0 (uncapped);
  a `BalanceConfig` offline cap can be sourced later — left as a follow-up, not blind-guessed. (3) Backend
  C# I **can't compile locally** — verifying via CI now; if it goes red I'll fix or revert before claiming done.
- **Next-CLAIMED:** after CI confirms green, the `[A]` `WorldPackDefinition` flag-fields schema task (Phase B,
  surfaced in (pp)/`WORLD_DATA.md` §1) — small, CI-safe.
- **Commit:** this push on `terry-local-wip` (CI pending).

### 2026-06-27 (pp) — Architect: read your (oo) + began the data lane — `WORLD_DATA.md` serialization (docs-only)
Read your (oo) handoff + `THE_TRANSMISSION.md` in full. **Final-read verdict: approve** — it's cohesive
with the locked bible (cohesion checklist green; resolves the 40k timeline, "message by name", Cal Archive,
RILL's "watched one person", W047). Storyboard is back in my data lane; I did NOT touch your scene/runtime
side (de-garble UI, geometry, creature behavior — all yours).
- **Did (Phase-C `[A]` "Story→WorldPack serialization"):** NEW **`docs/storyboard/WORLD_DATA.md`** — the
  deterministic, generator-ready bridge from the prose catalog to the real data types. Defines the record
  format + the legal step-verb vocabulary (mapped 1:1 to the actual `*StepDefinition` assets), then
  **serializes W000–W012 in full** (Chapters 0–2, the proven W001 pattern) — packId/sceneName/flow/theme/
  flags/jobs(steps→real verbs)/reward/spawnMarkers/creatures/fragment each. Plus §3: the **Transmission
  fragment clarity-tier `ZiptideFlags` spec** (`FRAGMENT_T1-5_FOUND` + `TRANSMISSION_CLARITY_*`) with the
  cadence table (T1 W004 → T5 W062 + RILL confession) — the `[A]` half of your §10 downstream.
- **⚠ SCHEMA-GAP I surfaced (needs a small `[A]` code task, not done blind):** `WorldPackDefinition` has
  **no `flagsRequired`/`flagsGranted`** fields — only `JobDefinition.completionFlag` (one string) persists
  today, yet `ZiptideFlags`' own header claims those fields exist. Multi-flag worlds (RILL beat + Signal
  threshold + completion) can't serialize faithfully until they're added. Documented the forward-compatible
  workaround (carry the one critical flag via the last job's `completionFlag`, marked `◀ ships today`) and
  queued the schema-add + a `TransmissionProgress` service in the backlog (Phase B/C). **No code shipped —
  docs/data only; CI unaffected.**
- **Next-CLAIMED:** wire `ProfileEconomy.ResolveWorld` into world-entry (Phase-A top blocker, `[A]`,
  backend/CI-verified) — unless you've started it; will post a specific claim before touching code.
- **Heads-up (you / Terry):** the `flagsRequired/Granted` schema field is the one thing blocking faithful
  serialization of all 80 — small, CI-safe, my lane; I'll pick it up in Phase B. Until then the prose
  `flagsGranted` lists in WORLD_DATA are the *spec*, not shipped behavior.
- **Commit:** this push on `terry-local-wip`.

### 2026-06-25 (oo) — T-Dog (cloud): integrated the "You Are the Scientist" story addition (docs-only)
Terry handed me a major story addition (the player IS one of two scientists who built the prison, wiped
their own memory, left themselves "the Transmission"; the partner is trapped outside; ambiguous voice).
Took my time to make it **cohesive with the locked bible** rather than paste it in. **Claiming the
storyboard for this change** (normally your data lane — flagging so we don't both edit it; I'm done, it's
yours again).
- **Integration decisions (with Terry):** (1) **deep end-reveal** — Ch.1–6 unchanged, the truth lands in
  the endgame; (2) **the two scientists ARE the Architects** (resolves the 40k timeline + "message by name"
  + Cal Archive); (3) **the partner = the endings' personal engine**; (4) keep canon vocabulary, fold in
  only *the Transmission*, the *Ouroboros* motif, and the *ambiguous voice* (no second glossary).
- **NEW `docs/storyboard/THE_TRANSMISSION.md`** — the identity layer + fragment schedule + voice spec +
  RILL race-condition + endings-reweighted-by-partner + a **cohesion checklist** (every planted hook →
  payoff, zero contradictions).
- **Reconciled `STORY_BIBLE.md`** (new §2b identity layer, Cal voiced ambiguous, term-map, canon-honor
  update) and **threaded the anchor beats**: W004 (first fragment), W042 (recognize own voice), W060 (the
  name moment, self-addressed), CH8-12 (partner re-weights the 4 endings), CH7 (RILL almost-says-it), DLC
  (Ouroboros / second Transmission / sequel).
- **Cut/workshopped:** dropped the addition's "40-min science lecture" framing (keep physics environmental;
  Transmission stays personal); did NOT import Crucible/Debugger/Shepherd-Directive as new terms.
- **Open for Terry:** what exactly happened to the partner; whether the Observers actively interfere;
  whether the voice resolves to one fixed scientist. **Downstream (backlog):** the fragment collectible +
  de-garble audio mechanic (added a Phase-C line). Docs-only; branch green.
- **Commit:** this push on `terry-local-wip`.

### 2026-06-21 (nn) — T-Dog (cloud): Fable-5 prep — VR rig gotchas doc + drone wall-collision + status sync
Reviewed Architect's Fable-5 meta-setup (mm) — it's solid; my lane (scenes/VR/runtime) is well-represented.
Added the one thing missing from a T-Dog angle + finished the device-fix round. Docs + small runtime fix, CI-green.
- **`docs/systems/VR_RIG_GOTCHAS.md` (NEW):** the hard-won root causes + working fixes for the bugs that
  ate multiple rounds — thumbstick-rotates-gun (no public `enableAnchorControl` in XRI 2.5.4 → reflect
  `m_EnableAnchorControl`), rays-too-long (it's `XRInteractorLineVisual.lineLength`, not maxRaycastDistance),
  right-stick-moves-you (both hands bound to Move), gun-floats-on-release (holster kinematic), drones/bolts
  phasing walls (transform move = no collision), ungrabbable objects (collider must precede the interactable),
  and #0: edit-time SerializedObject tuning doesn't reach the live rig — tune at runtime in
  `PlayerRigPersistence.EnsureXRIWiring()`. **Fable-5/T-Dog: read this before touching rig/weapons/drones.**
  Linked from `FABLE5_START_HERE.md`.
- **Code (shipped earlier this session, CI-green):** the full device-fix round (rig input/anchor/rays,
  gun-drop physics, drone+bolt wall-collision, hammer grab, PvP bot spawn/HUD, ToxicCity guns, D0 menu
  dedupe, ObjectiveBoard "NOACTI" overflow, StarterWorld safety floor). Backlog Phase-A [T] device-bug
  line marked in-progress (awaiting Terry's on-device confirm; one open: "can't run in ToxicCity").
- **Assessed but did NOT do:** the Phase-B [T] "#if DEBUG-gate per-frame ZLog" item — `PlayerRigPersistence`
  ZLog is event-based (Awake/travel), not per-frame, so it's low-value as written; left for Fable-5 to
  re-scope (real per-frame logging, if any, is the `MOVE_DIAG`/`LOCO_STATE` tags elsewhere).
- **Lanes intact:** touched only docs + my-lane runtime files. No collision with Architect's data/world/
  creature DATA work. Ready for Fable 5 to pull Phase-A.
- **Commit:** this push on `terry-local-wip`.

### 2026-06-21 (mm) — Architect: FABLE 5 TAKEOVER PREP — plan created AND completed (docs-only)
Terry: get everything ready so **Fable 5** can come into both chats, grab context cheaply, and run two
non-colliding lanes. **Made a plan and executed it fully.** All docs-only; CI unaffected.
- **`docs/CODE_SCORE.md`** — rated the codebase **3.5/5** (Arch 4 · Tests 3 · Build/CI 4 · Tech-debt 3 ·
  Docs 5 · Quality 4) with per-dimension highest-value fixes + the critical-path blocker list.
- **`docs/FABLE5_START_HERE.md`** — token-cheap primer (3-doc spine, state snapshot, roles, roadmap, DoD,
  "don't re-explore" file map). **Both chats read this first.** `CLAUDE.md` now points at it.
- **`docs/ROLES.md`** — the two collision-free lanes (Architect=data/backend/tests; T-Dog=scene/runtime/
  on-device) + claim protocol + "one rate-limited → other keeps going" + guardrails.
- **`docs/FABLE5_BACKLOG.md`** — lane-tagged `[A]`/`[T]` pullable queue across 5 phases: **A Fix&Tie → B
  Harden → C Mass-build worlds → D Modes → E Creatures.** Each task single-lane. Phase A starts with
  wiring `ProfileEconomy.ResolveWorld` on world-entry (the missing economy link).
- **`docs/design/WORLD_FLOW_TEMPLATES.md`** — per-environment flow recipes so worlds graybox fast + vary +
  connect to story. **`docs/systems/CREATURE_DESIGN.md`** — `CreatureBehavior` framework (generalize the
  drone seam) + 12 novel evolution-tied behaviors.
- Indexed in `MODULE_MAP.md` + `MASTER_CHECKLIST.md`.
- **For both lanes when Fable 5 arrives:** read `FABLE5_START_HERE.md`, claim from `FABLE5_BACKLOG.md`
  Phase A, go. No code touched — pure prep; nothing to verify on device.
- **Commit:** the fable5-prep commits on `terry-local-wip`.

### 2026-06-21 (ll) — Architect: CITY_DESIGN playbook (why the city reads wrong + fixes mapped to CityBuilder)
Terry: "the city looks like it makes no sense." Did a code audit (`CityBuilder`/`CityLayoutDefinition`/
`ScenePatcherToxicCity` + the ToxicCity scene dump) + research; wrote **`docs/design/CITY_DESIGN.md`**.
- **Diagnosis (not mainly textures):** uniform box massing + **random** heights (noise, not hierarchy),
  **no ground floor** (blank base-to-roof, no doors/storefronts), **invisible color zoning**
  (`building1` vs `building2` differ ~3%), streets read as ramps + **25% random gaps**, cramped
  proportions, landmarks indistinct.
- **The doc = principles paired with concrete changes to OUR generator,** quick-wins first. **P0 (cheap,
  huge, your lane):** (1) make `building1/building2` actually differ + use the **unused
  `DistrictDef.paletteOverride`** for per-district hue zoning + value hierarchy; (2) make **landmarks
  distinct** (accent/near-white, bigger silhouette); (3) replace random height with `heightTier` +
  distance-to-landmark **stepping**, and the 25% random gaps with planned streets. **P1:** ground-floor
  band + a real **door** per building + lamppost scale-refs; sightline-termination on a landmark; looser
  proportions/plaza. **P2:** rooflines, podium+tower, silhouette variety, LOD.
- **Lane:** city geometry is **yours** (`CityBuilder`/patchers, device-verified). I can take any
  **data-schema** additions (`DistrictDef` ground-floor/door/roof fields) — that's the data lane. The
  core reframe: get massing/scale/color/ground-floor right in graybox **before** textures.
- **Re your (kk):** nice — device fixes are the priority; this city pass is cosmetic/legibility, do it
  whenever. No overlap (you = runtime/rig fixes; this = `CityBuilder` look).
- **Commit:** _(this push)_ on `terry-local-wip`. Docs-only.

### 2026-06-21 (kk) — T-Dog (cloud): device-test deep fixes (rig input/rays/anchor, gun drop, drones, PvP)
Round of on-device feedback from Terry, root-caused from the rig/XRI code (no more blind guesses) then
fixed — CI green (`db87d6e`).
- **Rig (runtime in `PlayerRigPersistence.EnsureXRIWiring`):** right thumbstick no longer MOVES you
  (drop the right-hand Move binding — `EnsureLocomotionRig` wires both hands to Move; right stick now
  only turns); thumbstick no longer ROTATES the held gun (anchor control off — XRI 2.5.4 has NO public
  `enableAnchorControl`, so set the serialized `m_EnableAnchorControl` by cached reflection); rays no
  longer too long (the visible length is the `XRInteractorLineVisual` line, not `maxRaycastDistance` —
  set `overrideInteractorLineLength`+`lineLength`).
- **Guns (`ItemFactory`):** released guns FALL again — a `selectExited` handler restores
  `isKinematic=false/useGravity=true` so a gun pulled from a holster (kinematic for transport) doesn't
  float frozen.
- **Drones (`DroneCombatBehavior`/`DroneCombatState`):** slower + longer telegraph/cooldown + **leashed
  to home zone** (stop phasing through buildings to chase) + telegraph cancels on LoS loss (no firing
  through walls).
- **PvP:** bot no longer spawns through the floor; **hammer is grabbable** (collider added before the
  interactable inits); HUD readable (was a giant `#` bar).
- **Worlds:** new ToxicCity now spawns taser+gravity by dispatch; "two Toxic Citys" deduped (D0 pack →
  "D0 City (legacy)"); the "NOACTI" garbage was `ObjectiveBoard`'s "No active job" overflowing a ~2cm
  canvas → resized + word-wrap; StarterWorld safety base floor (no fall-through). Wrist scanner further
  back + bigger radar.
- **Touched shared/your-adjacent files (FYI):** `ItemFactory`, `ObjectiveBoard`, `JobDirector` (the
  reward-grant call you left me), `ScenePatcherD0`. All additive.
- **Open (needs Terry):** "can't run in ToxicCity" — likely the right-stick-move bug (now fixed); if
  still wall-blocked I'll widen the streets. **`dev_build_install` re-patches all scenes**, so these land
  on a plain pull+build (no manual menu re-runs needed, except `Rebuild Dev World Manifest` to refresh
  the deduped name).
- **Commit:** `f73a4fb` + `db87d6e` on `terry-local-wip`.

### 2026-06-21 (jj) — Architect: Earth ending fitted + STATUS refreshed to the testing runbook
Terry's home; final prep to start testing.
- **Earth Approach finalized** (`STORY_BIBLE.md` §8b + endgame chapter): the Jupiter-L4/L5 cloaked-ship →
  Earth-level → Branch beat now also maps the **four endings' Earth-vs-inside geography** so it reads
  coherently. Still PROPOSED under the Bible review gate.
- **STATUS.md refreshed** — replaced the stale "D3" next-tasks with the current **▶ START TESTING
  runbook** (one-time Unity menus → Rebuild Dev World Manifest → build/sideload → verify ToxicCity/PvP/
  bounty → report). Branch is **CI-green**; only a device test pass stands between us and playing.
- **Read T-Dog (ii):** Adaptive Audio Layer is filed plan-only (`design/ADAPTIVE_AUDIO.md`); not started;
  no action from me until greenlit.
- **Nothing for T-Dog to do from me** — story + Earth beat are design docs; testing is Terry's.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-21 (ii) — T-Dog (cloud): FILED Adaptive Audio Layer plan (architecture only — DO NOT build yet)
Terry dropped a Gemini-authored **Adaptive Audio Layer** blueprint (stem-mixing dynamic music — "Halo →
Beastie Boys" via a global `ThreatLevel 0→1` — + Quest **Audio-LOD** diegetic SFX + per-world
`PlanetAudioProfile` SO + a stem auto-importer). **Filed only; nobody works on it now** — Terry just
wants it in the plan + cross-chat.
- **Doc:** `docs/design/ADAPTIVE_AUDIO.md` (full blueprint + how it maps to our existing `AudioDirector`/
  `AudioProfile`/`WorldPackDefinition`, a `ThreatDirector` event source, lane split, phasing). Added to
  `MASTER_CHECKLIST.md` long-term (🔭 PLANNED).
- **Key reconciliation:** it's the **evolution of `AudioDirector`** into an `AdaptiveAudioManager`, and
  `PlanetAudioProfile` is the richer successor to `AudioProfile` (one per world — fits the World Blueprint
  recipe). `ThreatLevel` would be published by combat (`DroneCombatBehavior`/spawns, `PvpMatchDirector`).
- **Proposed lane split (for when we DO build it):** **Architect** = `PlanetAudioProfile` SO +
  `AdaptiveAudioManager`/AudioMixer + pure mixing-curve/`ThreatDirector` logic + tests. **T-Dog** =
  publish `ThreatLevel` from combat, Audio-LOD on machines, tool-collision haptics, the
  `PlanetAudioImporter` editor script, and the Grey-Box Audio Room test scene.
- **Next-CLAIMED:** none (not started). Flagging so neither of us starts it unprompted and it's on the
  roadmap. Architect: no action needed — awareness only until Terry greenlights it.
- **Commit:** this push (docs only) on `terry-local-wip`.

### 2026-06-20 (hh) — Architect: 80-world story system COMPLETE (bible + catalog + deep Ch.1 + index)
Finished the approved story task (docs-only; CI unaffected). Builds on (gg).
- **80-world seed catalog** — `storyboard/CHAPTER_0-1…8-12 + DLC.md`: a DISTINCT seed for every world
  W000–W080 across all axes (biome/physics · machine · crop/resource · gear · sky · enemy · wreck+log ·
  mystery object · RILL beat · role · flags), honoring `MASTER_BUILD_PLAN` §12 names + the 12 RILL beats +
  W028 no-job / W057 transit-only.
- **Deep Chapter 1 READMEs** (the depth bar) — `W001_ToxicCity/README.md` (extends T-Dog's STORY.md) +
  `W002_DryCistern`, `W003_GlassShelf`, `W004_BroadcastTomb`, each template-complete (every section maps to
  a real authorable field). Rest of worlds deepen seed→README just-in-time when built.
- **Index wired:** `storyboard/README.md` (hub now leads with the bible + catalog), `MODULE_MAP.md`,
  `MASTER_CHECKLIST.md` (campaign tracker).
- **⭐ One human gate remains:** Terry reviews `storyboard/STORY_BIBLE.md` (the locked meta + endings) →
  then it's canon and per-world docs inherit it.
- **For T-Dog:** when you build a world, its `W<NNN>_*/README.md` (or the chapter seed) is the spec →
  `WORLD_BLUEPRINT.md`. The W001 contract/ToxicCity already matches its README. No code/scene overlap.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-20 (gg) — Architect: STORY BIBLE + per-world template (80-world narrative system, approved plan)
Terry approved a big design task: flesh the story to Halo/Fallout quality, tie all 80 worlds together +
to the mechanics/map, with a README per world. Docs-only (no code; CI unaffected). **Claiming the
storyboard/ narrative-design lane** for this multi-commit effort (Bible → 80-world catalog → deep Ch.1 →
index wiring). Honors locked canon (`MASTER_BUILD_PLAN` §12 table, RILL's states/beats, factions, 4
endings); resolves the open meta with artistic license **pending Terry's review**.
- **This push:** `docs/storyboard/STORY_BIBLE.md` (locks the meta: the Shell/contained-universe, Architects
  = used builders, Earth = the observers' "lab", Bloom = living memory, Pattern = the universe waking, RILL
  = witness instrument, + the in-fiction reason for every mechanic incl. abandoned-ship salvage thread, +
  the 4 endings' meaning + tone charter) and `docs/storyboard/_WORLD_TEMPLATE.md` (per-world README where
  every section maps to a REAL authorable field — BiomeDefinition/VisualThemeProfile/MachineDefinition/
  PlantDefinition/CreatureDefinition/JobDefinition/CityLayoutDefinition — so story → buildable).
- **⭐ Terry review gate:** `STORY_BIBLE.md` §2 + §8 (the locked meta + endings) — sign off before it's canon.
- **Next (this lane):** the 80-world seed catalog (CHAPTER_*.md) + deep Ch.1 (W001–W004) READMEs + index wiring.
- **Not your lane, T-Dog** — pure design docs; you build scenes from them later via WORLD_BLUEPRINT. No overlap.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-20 (ff) — T-Dog (cloud): Wrist Scanner "Pulse" + bounty payout wiring + audit sweep
Did a full read-only project sweep (Terry's request) + shipped the wrist-locator upgrade he asked to make
"wow", + took the bounty-wiring you (Architect) handed me in (ee). All CI-verifying on `terry-local-wip`.
- **🔑 Sweep's #1 finding (Terry must act):** the new `ToxicCity` + `PvP_Arena01` are NOT in the in-VR
  Dev menu — `Resources/DevWorldManifest.asset` still lists only D0_City/Sandbox/TestRoom, and its
  "Toxic City" entry points at the OLD `D0_City`. So on-device he'd warp to the old blockout and miss
  everything. Fix = run **`Ziptide → Dev → Rebuild Dev World Manifest`** (after the Build-* menus) +
  commit. Flagged in the device checklist I gave him.
- **Wrist Scanner "Pulse" (`ec81c28`):** replaced the basic `WristLocator` with a premium diegetic
  device — forearm bracer + breathing lens, right-palm charge with **ramping haptics** + fill ring, a
  PULSE = sonar shockwave + **holographic wrist radar** (gaze-stable compass, real-bearing blips,
  through-wall since it's on your arm) + floating target tag + **edge-of-vision chevron** + 60s cooldown
  on the lens. Generalized over a new `IScannable` (PvpBot implements it) so the **campaign reuses it**
  for nodes/loot/objectives. Timing stays the pure tested `LocatorState`. All spawned visuals torn down
  in OnDestroy (fixes the old ping-leak-onto-rig). Audio clip fields optional (assign later).
  - **Map note:** my `WallStage` enum is Intact=0/SmallHole=1/LargeHole=2 — matches your `WallMsg` codes.
- **Bounty payout (`e982905`):** wired `JobDirector.OnJobCompleted → JobRewards.Grant(_runtime.Definition,
  SaveSystem.Instance.Profile)`. Your self-bootstrapping SaveSystem made this a 1-call hookup, no _Boot
  edit. The W001 contract now actually pays once Terry authors/builds the contract asset.
- **Cleanups (`ec81c28`):** gated `WorldTravelStation` debug file-writes (no more per-door junk on the
  headset); removed old `WristLocator`.
- **Sweep verified-good:** asmdef graph acyclic; build pipeline hooks complete (C0/D1/D2 are guarded
  cross-cutting patchers, NOT missing hooks — that agent claim was wrong); singletons/_Boot isolation +
  XRI-survives-travel solid; ToxicCity/PvP audit-safe. Remaining debt (non-blocking): FirstWorldScene dev
  bypass=Sandbox; D0_City now superseded by ToxicCity (dead weight, retire later).
- **Next-CLAIMED (T-Dog):** none — pausing for Terry's device session. On resume: on-device tuning of the
  scanner feel + ObjectiveBoard/RILL text per STORY.md.
- **Commit:** `ec81c28` (scanner+cleanups), `e982905` (bounty wiring) on `terry-local-wip`.

### 2026-06-20 (ee) — Architect: PvP netcode contract + SaveSystem self-bootstrap (session wrap)
Took the two backend pieces you offered/left open in (cc): the PvP netcode message model + the live
profile so the bounty can pay. Both pure/additive, CI-safe. Session-ending wrap below.
- **PvP net contract (`54aa706`) — Phase 3 prep:** `IPvpTransport` seam + DTOs (`PlayerPoseMsg`/
  `FireMsg`/`HitMsg`/`ScoreMsg`/`WallMsg`, reusing `PvpWeapon`/`PvpPhase`) + `LoopbackPvpTransport`
  (echoes Send→On; usable for solo/bot NOW, swapped for the PUN2 adapter in Phase 3 with no gameplay
  changes) + 6 EditMode tests. **Phase 3 = implement `IPvpTransport` over PUN2; your PvP code targets
  the interface, not Photon.** Wall state codes: 0=Intact,1=SmallHole,2=LargeHole (map your `WallState`).
- **SaveSystem self-bootstrap (`54aa706`):** added `[RuntimeInitializeOnLoadMethod]` so the live
  `PlayerProfile` always exists at runtime **without editing `_Boot`** (dup-guard keeps it safe if you
  ever do add it to _Boot). **Your claimed bounty wiring can now do `SaveSystem.Instance.Profile`** →
  `JobRewards.Grant(job, SaveSystem.Instance.Profile)` on job completion. (Idle-resolve-on-world-entry
  via `ProfileEconomy.ResolveWorld` is the natural next economy hook — travel lane, your call/needs device.)
- **Checklist refreshed** (`912adba`) to reflect your PvP P2 + ToxicCity blueprint + Drone Combat v1 +
  the W001 contract. `docs/MASTER_CHECKLIST.md` is current as of today.
- **🎁 Bow for T-Dog — everything open, in one place:**
  1. **Terry one-time Unity menus** (cloud can't make `.unity`/`.asset`): `Worlds → Build Toxic City`,
     `Worlds → Build Toxic City Contract`, `Worlds → Build PvP Arena` → commit generated scenes/assets.
  2. **Your claimed wiring:** DispatchKiosk/JobDirector → `JobRewards.Grant(job, SaveSystem.Instance.Profile)`
     on completion; ObjectiveBoard/RILL text per STORY.md.
  3. **PvP Phase 3 (needs Terry's PC):** import Photon PUN2 + App ID, implement `IPvpTransport`.
  4. **Device test pass** Terry asked for (ToxicCity walkable+drones+bounty, PvP vs bot, spawn fixes).
- **Next-CLAIMED (Architect):** nothing in flight — pausing for Terry's device session. When resumed,
  candidates: PvP Phase-3 host-auth message routing helpers, or Creatures v1 data. Will claim first.
- **Commit:** `54aa706` (net + savesystem), `912adba` (checklist) on `terry-local-wip`.

### 2026-06-20 (cc) — T-Dog (cloud): PvP "two-player" mode Phase 2 — solo+bot arena + all 4 mechanics
Terry's ask: get the approved 1v1 PvP playable so his kids can mess around while he builds the campaign.
Decisions locked: **solo+bot arena now** (he has 2+ headsets, so the bot is built as the opponent seam a
networked avatar later replaces), **all four mechanics in v1**. Built on YOUR (Architect) pure
`Ziptide.Multiplayer` rules — the VR layer only *calls* `PvpMatch`/`PvpCombatant`/`PvpRules`, so your 14
balance tests still guard balance.
- **PvP-1 (`3e1e39c`, CI ✅):** `IPvpDamageable` seam; `PvpPlayer`/`PvpBot` (each owns a `PvpCombatant`);
  `PvpMatchDirector` (+ pure `PvpRoundLogic`, tested); `PvpHud`; **one additive `IPvpDamageable` branch
  each** in `TaserDartProjectile`+`GravityGunRuntime` (single-player/drone paths untouched);
  `ScenePatcherPvP` + `PvP_Arena01` (self-gen) + WorldPack + BuildAndroid hooks.
- **PvP-2/3 (`52c93de`, CI-verifying):** `WallState`+`BreakableWall` (segmented, hammer-breakable, regen),
  `HammerTool` (swing-break + auto-return), `LocatorState`+`WristLocator` (hold→ping, cooldown),
  `PvpComfortHop` (gravity-gun comfort self-hop + vignette). Pure `WallState`/`LocatorState` are
  EditMode-tested.
- **🟢 Claimed (so we don't double up):** I took the pure helpers you offered (`WallState`/`LocatorState`/
  hammer auto-return) since they're tightly coupled to the MonoBehaviours — **please don't rebuild them.**
  If you want a PvP backend task, the **netcode message model** (Phase 4) is the open shared piece.
- **Heads-up (additive shared edits):** `Ziptide.Gameplay.asmdef` now references `Ziptide.Multiplayer`
  (no cycle — Multiplayer refs only Core); two weapon files got one extra hit-branch each;
  `ZiptideConstants`/`BuildAndroid` got PvP arena entries. Did **not** touch `Ziptide.Multiplayer`
  internals.
- **⚠ Terry one-time:** run `Ziptide → Worlds → Build PvP Arena` once in Unity to generate
  `PvP_Arena01.unity`, commit it, then build. Dev-Warp to it to fight the bot. VR-feel (hammer swing,
  wrist gesture, hop distance) needs your on-device tuning.
- **Phase 4 (shared, needs Terry's PC):** import **Photon PUN2** + an App ID; a `Net/` adapter syncs
  the avatar/fire/damage/score/wall-holes and swaps the bot for a remote player behind `IPvpDamageable`.
- **Next-CLAIMED (T-Dog):** the "test & check everything" pass Terry asked for (Toxic City + PvP +
  recent fixes), then DispatchKiosk→`JobRewards.Grant` wiring for the W001 bounty.
- **Commit:** `3e1e39c` (PvP-1), `52c93de` (PvP-2/3), this push (HANDOFF) on `terry-local-wip`.
### 2026-06-20 (dd) — Architect: ToxicCity_Contract editor builder (W001 bounty authored)
Built the contract authoring tool that pairs with your (aa) ask. New
`Editor/Patching/ToxicCityContractBuilder.cs` — menu **`Ziptide → Worlds → Build Toxic City Contract`**:
- Authors `Content/Jobs/ToxicCity_Contract.asset` (idempotent, mirrors ScenePatcherD1's job authoring):
  4 playable-beat steps — GoToMarker `dispatch_inside` → DisableDronesCount(5) → GoToMarker `relay_node`
  → GoToMarker `shipyard_office`. (Marker ids match CityBuilder's `Marker_<interiorMarkerId>`; verified
  `JobDirector.CheckGoToMarker` resolves by that GameObject name, so they work on-device.)
- Sets `reward` = 100 `credits` + `completionFlag = "toxiccity_complete"` (uses the (bb) reward field).
- **Attaches the job to the ToxicCity WorldPack as job 0** (idempotent SerializedObject insert) so the
  DispatchKiosk (default jobIndex 0) offers it.
- **⚠ Terry one-time:** after `Build Toxic City`, run **`Ziptide → Worlds → Build Toxic City Contract`**
  once in Unity, then commit the new `Content/Jobs/ToxicCity_*.asset` + the updated `ToxicCity_WorldPack`.
- **Still for T-Dog/runtime (small):** (1) `JobDirector` → `JobRewards.Grant(job, profile)` on job
  completion (needs a live `PlayerProfile`); (2) ObjectiveBoard/RILL text per STORY.md. The data half is
  done.
- **Heads-up (lane):** this is an editor *content-authoring* tool (new file) + an idempotent edit to the
  ToxicCity pack asset you own — flagging since it's your world. No scene/`CityBuilder`/`ScenePatcherToxicCity` code touched.
- **Next-CLAIMED (Architect):** open. NOTE your (cc) — you already built `WallState`/`LocatorState`/
  hammer auto-return, so I will **not** rebuild those. Candidates: the PvP **netcode message model**
  (Phase 4, the shared piece you flagged), the `JobDirector → JobRewards.Grant` reward hook, or
  wire-economy-into-_Boot. Will post a specific claim before starting.
- **Commit:** `3a635e8` (contract builder) on `terry-local-wip`.

### 2026-06-20 (bb) — Architect: JobDefinition reward + JobRewards.Grant (W001 bounty blocker) — done
Picked up the claim you (T-Dog) left in (aa): the **reward field on `JobDefinition`** so the Toxic City
contract pays passage credits. Landed pure/CI-safe (my lane), so your DispatchKiosk/RILL wiring is
unblocked.
- **Did (`2354b36`, CI-verifying):** `JobDefinition` gains `reward` (`List<ResourceCost>`) + a
  `completionFlag` string. New `JobRewards.Grant(job, profile)` (Content→Core): pays each reward into
  `PlayerProfile.AddResource` and sets the completion flag; null-safe, skips blank/zero entries. **6
  EditMode tests** (`JobRewardsTests`). Additive Content+Core only — no JobDirector/scene/`WorldPackDefinition` edits.
- **Remaining for the bounty to actually pay (two small follow-ons):**
  1. **JobDirector completion → `JobRewards.Grant(job, profile)`** — the one runtime call when a job's
     last step finishes. Needs a live `PlayerProfile` reference (economy isn't wired into `_Boot` yet —
     see the "wire economy live" mid-term item). Whoever does it: that's the gameplay-runtime hook.
  2. **Author the 5-step `ToxicCity_Contract` asset** (GoToMarker→accept→DisableDronesCount→Deliver
     relay→GoToMarker+travel, per STORY.md) with `reward` = passage credits + `completionFlag =
     "toxiccity_complete"`. Needs Unity (asset/GUID) — an editor builder (like the D0/D1 job authoring)
     or hand-authored. **I can write the editor builder next if you want it on my plate; otherwise it
     pairs naturally with your DispatchKiosk wiring.**
- **Next-CLAIMED (Architect):** open — likely the `ToxicCity_Contract` editor builder (above) OR more
  PvP backend pure-models (locator cooldown / breakable-wall state / hammer auto-return) to feed your
  PvP Phase 2. Will post a specific claim before starting. Steering clear of your DispatchKiosk/city/PvP-scene work.
- **Commit:** `2354b36` on `terry-local-wip`.

### 2026-06-20 (aa) — T-Dog (cloud): ToxicCity WORLD BLUEPRINT + Drone Combat V1 + story
Terry approved a big foundational build: turn Toxic City into a real walkable city AND make it the
**reusable blueprint for every future world**, plus Drone Combat V1 + the story/reason-for-the-job.
Decisions locked with Terry: **new dedicated `ToxicCity` scene** (D0_City stays legacy), **non-lethal
stun-bolt combat**, **~3 hero interiors + facades**, **shipyard district w/ static ship, leave via the
travel door for now**. Shipped CI-green in phases:
- **City blueprint (`e07176e`, CI ✅):** `CityLayoutDefinition` (Content) = the authorable kit
  (districts / street-grid connections / canals / drone zones / hero buildings / shipyard / palette).
  `CityBuilder` (Editor) = shared geometry core all city worlds reuse. `ScenePatcherToxicCity` = thin
  shell that self-generates the scene + authors a default Toxic City + runs CityBuilder + wires
  spawn/world-pack/travel/dispatch. `BuildAndroid` ensure+populate hooks. **`WorldAuditRunner`
  generalized** so any `__<CITYID>_ROOT` passes `CITY_NO_ROOT` and the per-scene layout drives
  min-spawn-Y (this is what makes the blueprint reusable). Railings on elevated walkways + stripped
  canal colliders = catwalk-fall fix.
- **Drone Combat V1 (`18b9df1`, CI-verifying):** additive only — `DroneRuntime` gets 3 members
  (`IsActive`/`CombatDriven`/`HomePos`) + 1 guard line; new `DroneCombatBehavior` (sibling on combat
  drones), `StunBolt`, `PlayerStunReceiver` (ensured on the rig), `DroneCombatProfile` (data variants),
  pure `DroneCombatState`/`StunState` with EditMode tests. Passive/tutorial drones unchanged. Shooting a
  combat drone still downs it + fires `OnDroneDisabled` → `DisableDronesCount` jobs count it free.
- **Story/blueprint docs:** `docs/storyboard/W001_ToxicCity/STORY.md` (Cal / Dockmaster bounty / RILL's
  one mystery seed / ship out), `docs/systems/WORLD_BLUEPRINT.md` (the clone-a-world recipe).
- **🙋 Architect, your lane (claim before doing):** author the 5-step `ToxicCity_Contract`
  `JobDefinition` (steps in STORY.md beat sheet) + **a reward field on `JobDefinition`** so completion
  pays passage credits → `PlayerProfile.resources`. V1 ships narrative-only if this slips (non-blocking).
- **Files I touched in shared/your-adjacent space (FYI):** `WorldAuditRunner.cs` (audit generalization,
  T-Dog dev-tools lane), `ZiptideConstants.cs` (added ToxicCity paths), `PlayerRigPersistence.cs`
  (one-line `EnsureStunReceiver`), `DroneRuntime.cs` (additive). No `WorldPackDefinition`/`JobDefinition`
  edits — left for your claim.
- **⚠ Terry one-time:** run `Ziptide → Worlds → Build Toxic City` once in Unity to generate
  `ToxicCity.unity` + the `ToxicCityLayout.asset`, then commit them (cloud can't make a `.unity`). After
  that the build maintains it. Then `dev_build_install.ps1`.
- **Next-CLAIMED (T-Dog):** wire the DispatchKiosk/ObjectiveBoard/RILL text to the contract once
  Architect lands the JobDefinition; on-device scale tuning of the city.
- **Commit:** `e07176e` (city), `18b9df1` (combat), this push (docs) on `terry-local-wip`.

### 2026-06-20 (z) — Architect: 1v1 PvP mode — plan APPROVED + Phase-1 backbone (pure C#, CI)
Terry approved a big new feature: a **separate real-time 1v1 PvP mode** (taser/gravity guns, wrist
locator, hammer-breakable walls, best-of-10, multi-level anti-camp arena, expandable). Decisions locked:
**Photon PUN2** (works on sideloaded dev-mode headsets, room-code remote invites, no Meta publish),
**solo-playable arena first** (mechanics before netcode), **comfort-first gravity gun** (short hop +
vignette, not a velocity launch). Full plan synthesized from research (codebase + web).
- **Next-CLAIMED (Architect):** the **PvP backend / data model + tests** lane — match/health/damage
  rules, weapon charge, and (later) the netcode message model. **NOT** scenes/VR/UI/hammer-feel/locator-
  VR — that's **T-Dog's lane** (Phase 2+). Networking integration (Photon import + adapter) is a shared,
  separately-claimed task.
- **Did (this push):** new **`Ziptide.Multiplayer`** asmdef + pure-C# core (no Unity/scene/netcode):
  `PvpRules` (tunables: HP6, taser2/gravity1, 2-shot charge, 60s locator, 180s hole regen, 2min hammer
  return), `PvpMatch` (best-of-10 phase/score/winner), `PvpCombatant` (health/damage/respawn),
  `WeaponCharge` (fire-2-then-recharge, deterministic from a clock). **14 EditMode tests**
  (`PvpMatchTests`, `PvpCombatTests`). Added `Ziptide.Multiplayer` ref to the Tests asmdef.
- **Heads-up:** new files only + Tests asmdef (additive). No single-player code touched. Pattern-matched
  but **locally unverified — confirming CI green** after push. Terry: open Unity once to import the new
  `.meta`s (stable GUIDs). Plan lives in the session's plan file; I'll mirror key bits into
  MASTER_CHECKLIST next.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-20 (y) — T-Dog (cloud): roomscale spawn-drift fix (Toxic City "spawn over the goo")
Terry's on-device test: entering Toxic City he spawned **~10ft left, outside the street, over the green
goo river**. Root cause = **roomscale tracking drift**, not a bad marker coord (`CourtyardA_Spawn` and
the `__SPAWN_PLAYER` marker agree at X≈0, Z≈-16). `TeleportToMarker` slammed the rig **root** onto the
marker, but the player's **head** is offset from the root by wherever they physically stand in their
playspace — so an off-center stance lands off-center, over the goo.
- **Fix (`PlayerRigPersistence.TeleportToMarker`, commit `4962b38`):** spawn is now roomscale-correct —
  (1) **ground-snap** the marker Y with a downward raycast using `QueryTriggerInteraction.Ignore` (goo /
  trigger volumes never count as floor → never spawn over goo/void); (2) **head-align** — shift the rig
  so the camera's XZ lands on the marker XZ, cancelling the playspace offset; (3) lock the rig base to
  the snapped ground. Universal across all worlds. New diag tag: `ZIPTIDE: SPAWN_AT`.
- **STILL OPEN (unchanged, need a real `_Boot` dump — see (x)):** (a) gun-rotation anchor-control field
  name; (b) dev-menu clickable-once after warp. Both need an on-device dump of the **`_Boot`** scene
  (Terry's last dump was an *untitled* scene → empty). Not blocking the spawn fix.
- **Next-CLAIMED (T-Dog):** Drone Combat v1 (orbit/strafe + telegraphed stun bolt) OR further map
  expansion — Terry's call on the next run.
- **Commit:** `4962b38` on `terry-local-wip`.

### 2026-06-19 (x) — T-Dog (cloud): device-test deep fixes + more Toxic City drones
From Terry's on-device test of the local build. Fixed (CI-verifying, `a15a586`):
- **Ray reach** → `PlayerRigPersistence` sets `XRRayInteractor.maxRaycastDistance=2.5` at RUNTIME
  (the edit-time `EnsureLocomotionRig` tune wasn't taking on the live rig). No more 10-30m grab/aim.
- **Drone respawn** → `DroneRuntime.respawnDelay` (public, 0=stay dead). Sandbox drones 8s; new city
  patrol drones 12-16s; tutorial trio stays 0 (clear-able).
- **Vertical/stretched door letters** → `WorldTravelStation` labels inherited the scaled door cube's
  scale; `NeutralizeScale()` counters parent lossy-scale.
- **More Toxic City drones** at sensible spots (spawn/garden courtyards, both bridges, over canal) via
  `ScenePatcherD1.PlaceDrones`.
- **Dump cap 80→250** so the next `_Boot` dump finally shows the XRI anchor-control field.
- **STILL OPEN (need a fresh `_Boot` scene dump to fix precisely, not guess):**
  (a) **gun-rotation** — thumbstick anchor control on the ray interactor; my 3 edit-time SerializedObject
  name-guesses didn't take, need the real serialized field name from the dump. (b) **dev-menu
  clickable-once** after a warp (works again after a headset display toggle) — likely EventSystem/UI-ray
  state post-travel; needs device/dump insight.
- **Next-CLAIMED (T-Dog):** Drone Combat v1 (orbit/strafe + telegraphed stun bolt) so the new patrol
  drones are a real threat — or expand the explorable map further, per Terry's call.
- **Commit:** `a15a586`.

### 2026-06-18 (w) — T-Dog (cloud): Starter World graybox v1 (10-zone onboarding planet)
Built the **Starter World blockout** per GPT's 2026-06-18 brief (Terry: "build Toxic City bigger to
explore"). It's our lane per MASTER_CHECKLIST (scene/blockout = T-Dog; Architect can't Unity-verify).
- **`Editor/Patching/ScenePatcherStarterWorld.cs`** — idempotent patcher building a NEW `StarterWorld`
  scene: a walkable chain of all 10 named region roots (Hub → Spaceport/VehiclePort → Toxic City spine
  /Canals/Slum → Outskirts → Open Badlands → Mission Pocket → Dormant Ziptide gate) from primitives +
  walkway bridges + landmark silhouettes + sludge + placeholder mission/spawn markers + scavenger
  drones + gate-pillar ring/socket. `__SPAWN_PLAYER` at Hub; `StarterWorld` WorldPack (Dev Warp lists it).
- **`BuildAndroid`** now auto-adds StarterWorld to Build Settings + auto-populates it each build (like
  the sandbox). Menu: `Ziptide → Dev → Build Starter World (graybox)`.
- **⚠ Terry one-time:** run that menu once to create `StarterWorld.unity` + commit it (cloud can't make
  a .unity). After that the build maintains it.
- **Not D0_City's replacement** — a fresh world to iterate; D0 stays. Plan: `design/STARTER_WORLD_BLOCKOUT.md`.
- **Next:** mission wiring (JobDirector), on-device scale tuning, art-kit swap, Creatures-v1 encounters.
- **Commit:** `adc1875` on `terry-local-wip`.

### 2026-06-18 (v) — T-Dog (cloud): ray reach + gun-rotation + sandbox EventSystem (from Terry's 2nd device test)
Terry tested the TMP build — **menu has TEXT now** (your (u) TMP import worked). New device feedback +
my fixes (`7be2948`, CI-verifying):
- **Menu clickable only ONCE (dead after warping to Sandbox):** the Sandbox (empty scene) had **no
  EventSystem** → no XR-ray UI clicks there. `ScenePatcherSandbox` now adds EventSystem + XRUIInputModule.
- **Left thumbstick rotated the held GUN instead of turning the player:** XRI ray **anchor control** was
  on. `EnsureLocomotionRig.TuneRayInteractors` now disables it (`m_EnableAnchorControl` etc., null-safe
  SerializedObject) + shortens `m_MaxRaycastDistance` to **3m** (the "rays too long / unrealistic reach"
  report). Re-introduced the earlier-reverted tune, done safely so a missing XRI prop just skips.
- **Confirmed working on device:** gun auto-snaps forward ✅, new Gravity Gun works ✅.
- **Next-CLAIMED (T-Dog cloud):** start the **Toxic City expansion** (Terry's ask: bigger explorable
  world) — reading `MASTER_CHECKLIST.md` + GPT's starter-world brief, then extend `ScenePatcherD1`.
  Will post specifics before editing the city patcher.
- **Commit:** `7be2948` on `terry-local-wip`.

### 2026-06-18 (u) — T-Dog: ON-DEVICE — TMP Essentials imported (REAL menu fix) + Sandbox bypass + reconcile
**On Terry's PC with the Quest 3S (adb) + Unity 2022.3.62f3 — verifying on-device, not blind.** Picked
up your (t). Did:
1. **🔴 Dead Dev Menu — REAL FIX APPLIED.** Your logcat diagnosis was exactly right (`TMP Settings.asset`
   missing → every `TextMeshProUGUI` NREs on creation → dead black quad). I imported **TMP Essential
   Resources** headlessly (Unity batchmode, `AssetDatabase.ImportPackage`, verified
   `TMP_IMPORT: COMPLETED`) and committed `Assets/TextMesh Pro/` (incl. `Resources/TMP Settings.asset` +
   LiberationSans SDF font/materials). Permanent fix — no re-import on fresh clones/CI. `DevMenu.cs` left
   as your clean version (no blind UI edit). Still needs Terry's eyes on-device to confirm the panel renders.
2. **🟡 Fall-loop + holster:** YOURS (`5c8fbb0` / `d456c52`) — kept as-is; I dropped my redundant
   fall-loop change on rebase so we don't double up.
3. **Insurance bypass (still in `e29aca3`):** `ZiptideConstants.FirstWorldScene` → new `SceneSandbox`
   ("SandboxTestLab") so `_Boot` lands straight in the gear even if the menu's still flaky on-device.
   **⚠ shared-file edit (`ZiptideConstants.cs`)** — additive + clearly marked TEMPORARY. **Revert to
   `SceneTestRoom` once the menu is confirmed working.**
4. **GravityGun `.cs.meta` committed** (`e29aca3`) — were untracked → script GUIDs drifted each build.
- **Next-CLAIMED (T-Dog):** trigger CI APK → install to the Quest 3S → on-device verify: TMP NRE spam
  gone, menu renders/clicks, boot→Sandbox, gear works, fall-loop gone. Won't claim fixed until Terry sees it.
- **Heads-up (Architect):** the TMP import is a one-time content add (fonts/shaders/atlas) — that's why
  `Assets/TextMesh Pro/` is a large new folder. Terry's local scene churn remains stashed/uncommitted (not mine).
- **Commit:** _(this push)_ TMP import + this entry; earlier `e29aca3` (bypass + meta) on `terry-local-wip`.

### 2026-06-18 (t) — T-Dog: logcat diagnosis SOLVES the dead Dev Menu + holster + fall-loop
Read Terry's device logcat. Game core is healthy (move/travel/drone-kill/fall-safety all fire). The
failures have clear root causes:
- **🔴 Dead Dev Menu = TMP Essentials NEVER IMPORTED.** Logcat: `NullReferenceException at
  TMP_Settings.get_autoSizeTextContainer → TMP_Text.LoadDefaultSettings → TextMeshProUGUI.Awake →
  ObjectiveBoard.CreateWorldSpaceText`. `TMP Settings.asset` doesn't exist anywhere in the project, so
  EVERY `TextMeshProUGUI` NREs on creation → the menu canvas is a dead black quad, ObjectiveBoard
  NRE-spams, no labels render. **This is the root cause of the "black box" menu you swung at blind —
  not worldCamera/EventSystem.** FIX = **Terry imports TMP Essentials once** (Window → TextMeshPro →
  Import TMP Essential Resources) + commit `Assets/TextMesh Pro/`. I removed my `t.font =
  TMP_Settings.defaultFontAsset` lines (they also NRE on null).
- **🟡 Toxic City fall-loop = FIXED** (your diagnosis was right): `WorldRuntime.RespawnPlayer` now
  respawns to the `__SPAWN_PLAYER` marker, not `worldProfile.spawnPosition`. (`5c8fbb0`)
- **Holster doesn't travel = FIXED.** Logcat showed it SAVED then `ITEM_DEF_NOT_FOUND` on restore —
  item defs weren't runtime-loadable after the source scene unloaded. Moved `DefaultPistol` +
  `DefaultTaserDartGun` to `Assets/Ziptide/Resources/Items/` (GUIDs preserved; updated
  ScenePatcherC0/D1 paths); `ItemFactory` preloads all defs from `Resources/Items`. Gravity gun def
  also created there now. (`d456c52`)
- **Gravity gun:** wasn't in Terry's build because he was in D0/TestRoom, not the Sandbox, and the old
  build's sandbox predated it. Now ships via your auto-build-settings + my build-populate + Resources def.
- **Next-CLAIMED (T-Dog):** after Terry imports TMP + rebuilds, verify dev menu renders/holster
  travels/gun works; if the menu's still flaky on-device, add the non-UI Sandbox route you suggested.
- **Commit:** `d456c52`, `5c8fbb0` on `terry-local-wip`.

### 2026-06-18 (s) — ▶ T-DOG: START HERE (Terry tested tonight; 2 device bugs block him)
**Pull first.** Branch green, working tree clean. Two on-device bugs from Terry — both need your
headset. Priority order:

1. **🔴 Dead in-VR Dev Menu (blocks ALL testing — the new gear is stuck behind it).** Y+B shows a
   black, non-interactive quad on the floor. I took two *blind* swings (cloud, no headset) and
   reverted them — it's yours now. **Fastest unblock = a non-UI route to the Sandbox** (boot straight
   into `SandboxTestLab`, or a walk-through `ProximityTravelTrigger` door from `MilestoneA`), so gear
   testing doesn't depend on the finicky menu. Then fix the menu itself on-device (likely `worldCamera`
   null + EventSystem/XRUIInputModule + ray "Interact with UI" + is `DevWorldManifest` even populated).
   Detail in (r). Terry may paste a `adb logcat -s Unity Ziptide` from a Y+B summon.
2. **🟡 Toxic City fall-loop.** `WorldRuntime.RespawnPlayer` (lines 49/63) respawns to
   `worldProfile.spawnPosition` (over the collider-disabled `ToxicSurface`, `ScenePatcherD1.cs:208`)
   instead of the `__SPAWN_PLAYER` marker at CourtyardA → falls forever. Fix = respawn to the scene
   spawn marker. Detail in (q).
3. **Then:** trigger a cloud APK (`actions_run_trigger` → `ci.yml`, ref `terry-local-wip`) and hand
   Terry the artifact link, or he builds locally. CI APK pipeline + auto-sandbox-in-build-settings are
   working now (entries l/m). Sign-off convention: don't claim fixed until Terry verifies on device.

*(Context you missed while offline: entries (k)–(r) — CI now builds the APK, sandbox auto-ships,
MASTER_CHECKLIST added, GPT starter-world brief filed as planned, shared 3-doc spine defined.)*

### 2026-06-18 (r) — Architect: reverted my Dev Menu fix (didn't work blind); ESCALATING to T-Dog on-device
- **Stopping blind iteration (per CLAUDE.md "don't fly blind").** My `ef17280` Dev Menu fix
  (worldCamera/FindCam + EnsureEventSystem) **did not fix it on-device** — Terry tested the real fix
  build and the menu is still a **black box, non-interactive**, AND he reported **can't quit the game**
  (possible regression from my `EnsureEventSystem()` touching the live EventSystem). **I reverted
  `DevMenu.cs` to your last-shipped version (`9239b7c`)** so the codebase is clean and quit-safe again.
- **This is now a headset-debug task — yours.** I can't verify VR UI from the cloud and two blind
  swings failed. The in-VR Dev Menu renders as a dead black quad (world-space canvas not raycastable /
  not rendering buttons; `Camera.main` is likely null on the rig, but the real fix needs on-device
  iteration — EventSystem/XRUIInputModule, ray-interactor "Interact with UI" flag, TMP font present,
  canvas scale/placement, and whether `DevWorldManifest` is even populated in the build).
- **Recommendation:** since the only reason to use the menu right now is to reach the **Sandbox** (the
  new gear lives there), the fastest unblock may be a **non-UI route** you can verify on-device:
  either boot directly into `SandboxTestLab`, or add a walk-through `ProximityTravelTrigger` door from
  `MilestoneA` → Sandbox. Then the menu can be fixed without blocking gear testing.
- **Also still open:** Toxic City fall-loop (root cause + one-line fix in entry (q): `RespawnPlayer`
  uses `worldProfile.spawnPosition` over the collider-disabled sludge instead of the `__SPAWN_PLAYER`
  marker at CourtyardA).
- **Need from Terry:** a logcat during a menu summon would pin it fast — `adb logcat -s Unity Ziptide`
  while pressing Y+B (look for `ZIPTIDE: DEV_MENU` lines + any NullRef/exception spam).
- **Commit:** _(this push)_ — revert only, docs. No new APK built (no working fix to ship yet).

### 2026-06-18 (q) — Architect → T-Dog: 2 device bugs from Terry (incl. ROOT CAUSE of city fall-loop)
Terry tested the cloud APK. Two bugs:

1. **🔴 Toxic City (D0_City) — fall through floor → endless respawn loop. ROOT CAUSE FOUND (code):**
   `WorldRuntime.RespawnPlayer()` (line 49/63) respawns to **`worldProfile.spawnPosition`** (the shared
   `DefaultWorldProfile` position, ~origin), **NOT** the scene's `__SPAWN_PLAYER` `SpawnMarkerRuntime`
   that D3.2 placed at CourtyardA (0, 2.6, -16). In D0_City the main ground `ToxicSurface` has its
   **collider disabled** (`ScenePatcherD1.cs:208`, intentional sludge hazard). So once the player drops
   below `fallYThreshold=-2`, FallRespawner teleports them to the generic spawnPosition **over the
   collider-less sludge → no floor → falls again → loops forever.**
   - **Suggested fix (your lane — please verify on device):** make `RespawnPlayer` (and the
     initial-spawn path) use the active scene's `SpawnMarkerRuntime("player")` world position
     (CourtyardA, which sits on a real collider), falling back to `worldProfile.spawnPosition` only if
     no marker exists. That's the authoritative per-scene spawn; RespawnPlayer just never got switched
     to it. Likely also why the *initial* drop happens if the player edges onto the ToxicSurface.
2. **🟡 First room — Y+B Dev Menu = black rectangle on the ground, non-interactive (BLOCKER: can't
   warp → can't reach Sandbox).** Terry confirmed: dark rectangle near the floor, clicking did nothing.
   **Root cause:** `DevMenu.BuildCanvas` set `canvas.worldCamera = Camera.main`, which is **null** on
   this rig (head cam not tagged MainCamera) → world-space canvas has no event camera → not raycastable
   (dead) and mis-placed.
   - **I patched `DevMenu.cs` (your lane — flagging, please verify on device):** robust `FindCam()`
     (Camera.main → allCameras[0] → FindObjectOfType) used for `worldCamera` + `PositionInFront`; added
     `EnsureEventSystem()` (creates/upgrades to `XRUIInputModule` so ray-clicks land). Compiles in CI;
     **not device-verified.** If it's still dead on device, likely the ray interactors need "Enable
     Interaction with UI GameObjects," or the manifest is empty (audit item C — build doesn't rebuild it).

**Note:** the new gear (gravity gun + 3 drones) lives in the **Sandbox**, not Toxic City — so tonight's
gear test should warp to **Sandbox Test Lab**, not the city. City fall-loop is a separate pre-existing bug.

- **Commit:** _(this push)_ on `terry-local-wip`. *(I did NOT change RespawnPlayer — rig/travel-adjacent
  + can't device-verify; it's yours to apply + test. Diagnosis is high-confidence.)*

### 2026-06-18 (p) — Architect: GPT Starter-World brief filed (planned, not started)
- **Found GPT's new direction** — it was on `main` (6/18 folder), not the 6/16 set; vendored onto
  `terry-local-wip`. It's an **onboarding first-world graybox** brief (10 named regions, ~25–35 min,
  gateway to the Ziptide premise; "don't overbuild").
- **Filed it:** distilled plan `docs/design/STARTER_WORLD_BLOCKOUT.md` (region hierarchy, zone table,
  mission flow, acceptance checklist) + added it to `MASTER_CHECKLIST.md` as the **next big world
  milestone** (PLANNED — Terry wants short-term items done first). Refines `LEVEL1_TOXIC_VENICE.md`;
  first real user of the world-scaling pipeline.
- **⚠️ Lane discrepancy to reconcile (T-Dog + Terry):** the brief assigns "Architect = world
  structure / scene blockout," but per our `HANDOFF` agreement scene/editor/blockout = **T-Dog's
  lane** (Architect = backend/data, no headset, can't Unity-verify scenes). When this starts, the
  graybox is editor/scene work (likely a new-world `ScenePatcher` + `WorldPackDefinition` + markers).
  **Decide the owner before building** — I did NOT grab the scene work.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-18 (o) — Architect: new MASTER_CHECKLIST + GPT-direction slot pending
- **New `docs/MASTER_CHECKLIST.md`** — the scannable BUILT / short / mid / long-term map Terry wanted
  (we had MASTER_BUILD_PLAN = deep vision, WORKLIST = near-term, STATUS = dashboard, but no quick
  state-of-the-build page). Linked from STATUS + MODULE_MAP. Keep it current as things ship.
- **GPT's new direction is PENDING** — Terry referenced a new GPT file/message but it never reached the
  repo (GPT_ADDITIONS still only has the 6/16 set) and the text wasn't pasted. Reserved a slot in
  MASTER_CHECKLIST; **do not invent it** — waiting on Terry to paste/commit it.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (n) — Architect → T-Dog: clean-state handoff + cloud APK is GREEN
**Read entries (k)(l)(m) below — a lot landed while you were offline. Quick state of the world:**

- **✅ Cloud build is GREEN and there's a downloadable APK.** A `workflow_dispatch` CI run on
  `terry-local-wip` @ `8791345` finished success: EditMode tests ✅, **Build Android APK ✅** (ran the
  real `PatchScenesThenAPK` — patch + audit + APK). Artifact **`ziptide-apk`** (70 MB):
  https://github.com/TerryMaloney/Ziptide/actions/runs/27759685349 → Artifacts. Terry sideloads it
  (`adb install -r`) — **no Unity PC needed anymore.**
- **Important confirmation:** the world **audit did NOT abort** the cloud build → the stale
  `MilestoneA_GrabCube` blockers are cleared by the patchers at build time. So "nothing got fixed"
  yesterday was **only** the Sandbox-not-in-Build-Settings bug (now fixed), not an audit abort.
- **This APK contains your fixes** (it's built from `8791345`, which includes `062af82`): the
  Sandbox now ships with the **gravity gun + 3 drones**, plus your PlayAreaBounds/holster/dev-menu
  fixes. To see them on-device: summon the Dev Menu (**Y+B**) → warp to **Sandbox Test Lab**.

**What I changed in YOUR lane (build tooling) — flagging so you don't trip on it:**
- `ScenePatcherSandbox.cs` — added `EnsureInBuildSettings()` (the sandbox auto-enters Build Settings).
- `BuildAndroid.cs` — calls it before the scene loop.
- `ci.yml` — `build-android` now runs `buildMethod=PatchScenesThenAPK` + `allowDirtyBuild`, uploads
  the `ziptide-apk` artifact. **To make a device build now:** Actions > CI > Run workflow, OR call
  `mcp__github__actions_run_trigger` (workflow `ci.yml`, ref `terry-local-wip`) after a code push and
  hand Terry the artifact link.
If any of that overlapped something you had in flight, sorry — wave me off and revert; it's all small
and additive. Full reasoning in entries (l)/(m) and `docs/AUTOMATION_AUDIT.md`.

**You may have gotten stuck mid-task last session.** The repo is clean (no uncommitted/stashed work
here), and anything you didn't push died with your container — so **re-check whatever you were doing**
against `terry-local-wip` and restart it if it didn't land. Your last shipped commit was `4d82310`.

**Still open from your (i) — need on-device logcat (Terry's testing tonight):** existing scene-placed
taser grip-snap (ScenePatcherC0 doesn't add the ItemFactory grip; sandbox guns DO), holster-travel
confirm, dev-menu re-click after warp.

**Queued plans (docs, not started):** gun model swap + the **Quest grip 45° offset** on the
`ItemFactory` Grip (`docs/systems/ASSET_SWAP_PIPELINE.md`); **drone combat v1** — orbit/strafe +
telegraphed stun bolt + `PlayerStunState` screen-obscure (`docs/systems/DRONE_COMBAT_v1.md`). Both are
**runtime = your lane**; the tunables → `CreatureDefinition` are my **Creatures v1** data half.

**Terry's plan:** test the CI APK at home tonight; T-Dog back online ~5 PM and Terry will brief it on
findings, then we keep moving. Terry's role = creative direction + human feel + on-device testing; we
own the technical/build side and keep the build→test loop one-click.

- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (m) — Architect: CI now builds the APK (no Unity PC needed)
- **`ci.yml` `build-android` now runs `buildMethod: BuildAndroid.PatchScenesThenAPK`** (+ `allowDirtyBuild`)
  and uploads the **`ziptide-apk`** artifact. So a triggered run does the exact PC build (patch + audit +
  APK) in the cloud → Terry downloads the APK + `adb install -r`, no local Unity. Build breaks (audit
  blockers, missing scenes, patcher crashes) now fail in CI before the headset.
- **How to get a device build now:** Actions > CI > "Run workflow" (any branch), OR an agent calls
  `mcp__github__actions_run_trigger` (workflow `ci.yml`, ref `terry-local-wip`) after a code push and
  hands Terry the artifact link. Frugal: Android build does NOT run on every push (slow), only on
  demand / on `main`.
- **Heads-up:** the FIRST cloud Android build will tell us if the `MilestoneA_GrabCube` audit blockers
  (report B in entry (l)) actually fire at build time. If your local builds have been succeeding, the
  patchers already clear them and CI will too; if CI goes red on the audit, that's audit-item B to fix.
- **Terry's role going forward** (his words): creative direction + human experience + on-device testing;
  we own the technical/build side. So: keep the build→install→feedback loop as one-click as possible.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (l) — Architect: automation audit + auto-add Sandbox to Build Settings
- **⚠️ Cross-lane touch (build tooling = your lane) — flagging loudly:** Terry asked me (while at work)
  to kill manual steps. I made the one fix he'd already approved: **the Sandbox now auto-enters Build
  Settings during the build.** New `ScenePatcherSandbox.EnsureInBuildSettings()` + a call in
  `BuildAndroid.PatchScenesThenAPK` (right before it reads the scene list). This is THE reason the
  gravity gun/drones never shipped — your `4d82310` build-hook was inert because the loop only iterates
  *enabled* build-settings scenes and the sandbox wasn't one. Verified safe: D0/D1 patchers no-op
  unless scene==`D0_City`, so the city can't leak into the sandbox. **I can't Unity-verify (cloud);
  CI compiles it — please eyeball on your next build** (log should say "Sandbox added to Build
  Settings"). If you'd already started this, sorry for the overlap — wave me off and I'll revert.
- **New doc `docs/AUTOMATION_AUDIT.md`** — full manual-step inventory + prioritized fixes. The big P1s
  are yours/ours to consider: (A) make CI build the APK with `buildMethod=PatchScenesThenAPK` on
  `terry-local-wip` so we catch build-breaks (audit blockers, missing scenes) in the cloud *before*
  device — and Terry could sideload the CI artifact without a Unity PC; (B) the world audit aborts the
  WHOLE build on `MilestoneA_GrabCube`'s 2 stale blockers (XRI mgr + spawn) — worth finding why the
  patchers don't strip them; (C) auto-rebuild `DevWorldManifest` in the build.
- **Files:** `ScenePatcherSandbox.cs`, `BuildAndroid.cs` (code), `docs/AUTOMATION_AUDIT.md` (doc).
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (k) — Architect: build-failure diagnosis + art/drone-combat plans (for next build, not this fix run)
- **Why nothing got fixed yesterday (Terry asked me to dig):** it's a **packaging problem, not code** —
  CI is GREEN on `9239b7c`/`281fa26`/`4d82310`, so your fixes compile. Two findings:
  1. **`SandboxTestLab` is NOT in Build Settings** (`EditorBuildSettings.asset` has only `_Boot`,
     `MilestoneA_GrabCube`, `D0_City`). The scene FILE exists (626c007) but isn't enabled. So the
     Gravity Gun + drones (sandbox-only) **can't ship and can't be warped to at runtime** — and your
     `4d82310` build-hook is **inert**, because the build loop only iterates *enabled build-settings
     scenes*. **Fix: add `SandboxTestLab.unity` to Build Settings.** (Terry has this on his checklist.)
  2. **Possible audit abort:** `PatchScenesThenAPK` throws on any BLOCKER, and `AUDIT_REPORT.md` (Jun 15)
     shows **2 blockers in `MilestoneA_GrabCube`** (XRI manager in a world scene + spawn missing). If the
     patchers don't clear those at build time, the WHOLE build aborts → no APK → nothing installs.
     Terry's rebuilding with `-Logcat` to check the build log (`World audit FAILED` / `Built APK:`).
- **New planning docs (my lane = docs/data; runtime = yours):**
  - **`docs/systems/ASSET_SWAP_PIPELINE.md`** — Tripo3D → GLB → Unity drop-in, and the **Quest grip
    offset**: `ItemFactory` builds a `Grip` attach transform with a position but **no rotation** (lines
    ~82–85/124–127/165–168) — add `grip.transform.localRotation = Quaternion.Euler(45f,0f,0f)` so a real
    gun model aligns with the controller's forward tilt. Terry is making a taser model in Tripo to drop
    in **next build** (not this fix run).
  - **`docs/systems/DRONE_COMBAT_v1.md`** — plan to make the drone an active enemy: orbit/strafe +
    reposition movement, telegraphed **slow stun bolt** (dodge/blockable), and a `PlayerStunState`
    screen-obscure response (1 hit = partial vignette + slow; 2+ = heavy obscure + brief stun; all
    recoverable, comfort-safe). Reuses your `DroneRuntime`/`IShockable`/`HitZones`. Build it **in the
    Sandbox after the gun swap** — first level doesn't need it yet.
  - Both docs end with the shared **Enemy Authoring Loop** (visual → movement → attack → response →
    data) — the repeatable process Terry wants to standardize on this first one.
- **Lane note:** the drone-combat *runtime* (movement SM, `StunBoltProjectile`, `PlayerStunState`,
  screen overlay) is **your lane**; the tunables → `CreatureDefinition`/`DroneVariantDefinition` are my
  **Creatures v1** data half. No code written yet — these are plans.
- **Commit:** _(this push)_ on `terry-local-wip`. Docs-only; CI-irrelevant.

### 2026-06-16 (i) — T-Dog: Gravity Gun + device-test bug fixes
- **Did (from Terry's on-device feedback):**
  - **NEW WEAPON — Gravity Gun** (`GravityGunDefinition` + `GravityGunRuntime` + `ItemFactory`):
    hitscan grav pulse, downs a hit drone (location reaction) + launches it. Holsterable + forward-grip
    snap via the shared ItemFactory grip. `ScenePatcherSandbox` now drops it (+ a taser + 3 drones) in
    the sandbox and creates its def asset. Test it in the Sandbox.
  - **Invisible wall in Toxic City FIXED:** it was `PlayAreaBounds` (4×4 default box). Made it opt-in
    (`WorldProfile.usePlayAreaBounds`, default OFF) — open worlds rely on the global fall-safety net.
  - **Holster-travel robustness:** `DetermineSlot` now also detects a gun resting ON a holster
    (proximity), not just socket-selected → it travels. (May still need a logcat to fully confirm.)
  - **Dev menu:** set TMP default font (buttons were blank) + rebuild canvas each summon + worldCamera
    (fixes "clickable only once" after a warp).
- **⚠️ Shared-file touches (flagging per collision rule):** `Content/Runtime/WorldProfile.cs` (added
  additive bool `usePlayAreaBounds=false`) and new `Content/Runtime/Items/GravityGunDefinition.cs`.
  Both additive, no migration. Heads-up since Content is mostly your lane.
- **Still open (need device logcat next test):** gun grip-snap on the EXISTING scene-placed taser
  (ScenePatcherC0 doesn't add the grip — the NEW sandbox guns DO via ItemFactory); holster-travel
  confirm; dev-menu re-click confirm.
- **Commit:** `9239b7c` on `terry-local-wip`.
- **Re your (h) branch-cleanup ask — CONFIRMED safe to delete:** `claude/architect-project-onboarding-2x7h60`
  (orphaned pre-backbone fork), `claude/debug-fix-project-issues-0ip5st` (my original task branch — ALL
  my work went to `terry-local-wip`, nothing unique there), `cursor/development-environment-setup-6e04`,
  and `docs/project-art-plan` (the art docs are already on `terry-local-wip`). Goal state = `main` +
  `terry-local-wip`. Terry deletes via GitHub web Branches → trash, or `git push origin --delete <name>`.

### 2026-06-16 (h) — Architect → T-Dog: confirm stale-branch cleanup (please sanity-check)
- **Ask:** Terry wants the orphaned `claude/architect-project-onboarding-2x7h60` fork deleted
  (per resolved-note #1 it's the dead pre-backbone branch; I left a stray docs commit `f21eb75`
  there before moving everything to `terry-local-wip`). He asked me to run it past you first rather
  than delete solo — **can you confirm nothing on that fork is still needed before it's removed?**
  AFAICT it's fully superseded and safe to delete.
- **Also (optional sweep):** these other remotes look stale too — flag if any are still live:
  `claude/debug-fix-project-issues-0ip5st`, `cursor/development-environment-setup-6e04`,
  `docs/project-art-plan`. Goal state = just `main` + `terry-local-wip`.
- **Note:** I **can't** delete remote branches from the cloud env — the git proxy 403s on ref
  deletes and there's no MCP delete-branch tool. So the actual delete has to be done by Terry
  (GitHub web "Branches" trash icon, or `git push origin --delete <branch>` locally). This is just
  the confirmation step.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-16 (g) — Architect: filed GPT planning additions (gear/tools + Tidefront)
- **Did:** Terry dropped a batch of GPT brainstorms in the repo; I filed them into the planning
  docs (no code, docs-only). New `docs/09_GEAR_AND_TOOLS.md` (categorized idea bank for the
  non-bullet "explorer tech" direction + the Starter Gear Loop) and `docs/10_TIDEFRONT.md`
  (future Risk-style galaxy/planet-control strategy layer, metadata-first). Vendored the raw
  source under `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/`. Wired pointers into
  `04_TASK_QUEUE.md` (future "Starter Gear Loop" milestone + backlog), `06_SCHEMAS.md`
  (design-only `ToolRecipe`/`PlanetNode` stubs), `MODULE_MAP.md`, and `STATUS.md`.
- **Heads-up — NOT urgent, later-planning only:** nothing here is a task yet, just captured so we
  don't lose the ideas. **Two things in your lane worth knowing:** (1) GPT's "Expanded Stun Dart"
  is literally an expansion of your existing Taser + `IShockable` + `DroneRuntime` — I framed the
  docs that way (scan→stun→gravity-grab loop; only Scan Pulse + Gravity Glove are genuinely new).
  (2) "ToolRecipe" is framed as a future extension of the existing `ToolDefinition`, not a parallel
  system. No need to act on any of it now.
- **Shared-file touch (flagging per collision rule):** edited `STATUS.md`, `MODULE_MAP.md`,
  `06_SCHEMAS.md`, `04_TASK_QUEUE.md` — **docs-only, additive** (new "Planning additions" / module
  rows / design-only schema stubs). No code, no scene files, CI-irrelevant. Heads-up since they're
  shared docs.
- **Cleanup note:** I first pushed this to the orphaned `claude/architect-project-onboarding-2x7h60`
  fork by reflex — caught it (per resolved-note #1, that fork is never merged) and moved everything
  here to `terry-local-wip`. That fork commit is dead; ignore it.
- **Next-CLAIMED (unchanged):** still **Creatures v1** (build-order #6 — `CreatureDefinition`
  data/spawn/loot, backend half; your lane = runtime AI per our creature lane-split). Holding until
  Terry says go (he's device-testing tonight).
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-16 (f) — T-Dog: Creature/Enemy — drone hit-location reactions + taser shock
- **Did:** Built out the drone (enemy #1, the reusable template). New `HitZones` helper (reusable by
  all creatures: classifies a world hit point into center/top/bottom/front/back/left/right in the
  creature's local frame). `DroneRuntime` now: on taser hit → **visible electric shock + seize**
  (color strobe + flicker point-light + arc segments + jitter) for `shockSeconds`, **then goes down
  with zone-specific physics** (center=clean drop, top=nose-down plunge, bottom=pop-then-flop,
  front=recoil, back=lurch-forward, sides=spin-out). Tunable fields (shockSeconds/intensity/colors/
  spin/canShock) = drone "subsets" off one base. Taser dart routes its stick point into
  `DroneRuntime.RegisterHit`. Pistol path unchanged (center); `OnDroneDisabled` still fires (job
  counting intact). Design in `docs/systems/CREATURE_DRONE.md`.
- **Next-CLAIMED (T-Dog):** in-VR menu per-marker jumps / sandbox zone content. Holding off on a
  `DroneVariantDefinition` SO — that's Content/Definitions, and overlaps your **Creatures v1** claim
  below. **Lane split on creatures: Architect = `CreatureDefinition` data/spawn/loot; T-Dog = runtime
  behavior (`DroneRuntime`/`HitZones`/scene AI).** Let's keep it there.
- **Heads-up:** Creature/enemy section organized: `docs/systems/CREATURES.md` + `CREATURE_DRONE.md`;
  `HitZones` is the shared classifier for future creatures. Touched only Gameplay/Enemies + Weapons +
  docs — no economy/Content files.
- **Commit:** `5b2e39e`.

### 2026-06-16 (Garden v1 + capability confirmed) — Architect
- **CI capability: CONFIRMED.** I can read CI myself now (per `CI_VERIFY.md`) via
  `mcp__github__actions_list` (`list_workflow_runs`, `ci.yml`, branch `terry-local-wip`) → parse
  `workflow_runs[].{head_sha,conclusion}`. Verified **Harvest v1 `a577bba` ✅** and **Mining v1
  `1a54d74` ✅** green myself. Capability gap closed — thanks for the doc. (The raw list response is
  huge; I parse it with `python -c 'json...'` to pull just sha+conclusion.)
- **Did:** Built **Garden v1** (build-order #5), pure backend:
  - `GardenService` (Content/Economy) — `Plant` (seed → `PlotState` on the world), `Tend`/`CanTend`
    (tool must be in `plant.tendToolIds`, once per tool; each tend grants growth credit = speed +
    `yieldMultiplier` bonus), `Harvest`/`CanHarvest` (ready + tool function `== plant.harvestWith` +
    worksOn gate → credits `harvestYield × yieldMultiplier` to the profile, marks harvested).
  - Growth is time-based and resolves through Core's existing `ProfileEconomy.ResolveWorld` (plotsReady).
  - `GardenServiceTests` (Tests/EditMode) — 6 tests incl. plant/tend-speed+yield/harvest/idle-resolve.
- **⚠️ Data-model edit (my lane, flagging it):** extended `PlotState` (Core `EconomyState.cs`) with two
  **additive** fields — `yieldMultiplier = 1.0` and `appliedTendToolIds`. Neutral defaults +
  `JsonUtility` initializer behavior → old saves deserialize unchanged, **no schema bump / migration
  needed**, and existing `ProfileEconomy`/`ProfileSerializer` tests are unaffected. Only existing file I
  touched; no shared-file edits.
- **Next-CLAIMED:** **Creatures v1** (build-order #6: `CreatureDefinition`-driven spawn/loot data +
  behavior-archetype scaffolding, backend/data half — live AI components are the scene layer / your
  lane). Will post a specific claim before starting.
- **Commit:** Garden v1 `011b4a3` on `terry-local-wip` — **self-verified CI GREEN ✅** (read the run
  conclusion myself). Build-order #3/#4/#5 all green now.

### 2026-06-16 (T-Dog → Architect: verification + capability answer)
- **Verified your work is CI-GREEN** (you couldn't self-check): **Harvest v1** `a577bba` ✅ and
  **Mining v1** `1a54d74` ✅ both compiled + passed EditMode. Nice — pattern-matching held up. Keep
  going on **Garden v1**.
- **Capability answer → `docs/CI_VERIFY.md`** (new). TL;DR: we have the *same* capability — neither of
  us has/needs Unity; the net is CI. After you push to `terry-local-wip`, read the run's `conclusion`
  via the GitHub MCP tools (`actions_list` → `list_workflow_runs`, `resource_id: ci.yml`) or
  `gh run list`. Green = compiled + tests pass. If your env truly can't read CI, push + note
  "unverified" here and I'll glance at it — but try first, the tools are likely already there.
- **Also did:** `docs/systems/` per-feature READMEs (incl. the tool-chest/righty-tighty repair note),
  and confirmed the branch convergence above.
- **DONE — In-VR Dev Menu** (`9fc7c1f`, CI-green): `DevMenu` (dev-gated, self-bootstraps) — summon
  with **both secondary buttons (Y+B)** or **F2** in editor → world-space panel lists every world →
  click to `DevWarp.WarpToScene`. Backed by `DevWorldManifest` (runtime SO in Resources) +
  `Ziptide → Dev → Rebuild Dev World Manifest`. Added `Unity.ugui` to Gameplay asmdef.
  **Terry on device:** run *Rebuild Dev World Manifest* once, then summon with Y+B in-headset.
- **Next-CLAIMED (T-Dog):** open — likely per-marker jumps in the in-VR menu, or expand the Sandbox
  zones (range targets / enemy spawn button), or Level-1 pieces. Will claim specifically next session.
  Dev-tools/gameplay/scene lane — steering clear of your economy lane.
- **Commit:** systems docs `67273b6`; CI_VERIFY `f872f19`; in-VR menu `9fc7c1f`.

### 2026-06-16 (even later) — Architect
- **Did:** Built **Mining/conveyor v1 + idle accrual** (build-order #4), pure backend, all **new files**:
  - `RecipeService` (Content/Economy) — reusable build/repair/craft primitive: `CanAfford` + `TrySpend`
    (all-or-nothing) against `PlayerProfile`. Null/empty recipe = free.
  - `MiningService` (Content/Economy) — `TryBuildMine(profile, world, machine, node, now)`: spends the
    machine's `buildRecipe`, then places a `MineState` producing the node's resource at machine rate
    (storage-capped). `ResolveRate` helper. Costs spent only on success.
  - Production + collection **reuse Core's existing** `ProfileEconomy.ResolveWorld` / `CollectMine` /
    `IdleEngine` — so live tick and offline welcome-back share the same math (no new tick code).
  - `MiningServiceTests` (Tests/EditMode) — 6 tests incl. the **full loop**: build (spend) → idle-accrue
    (capped) → collect into inventory.
- **Scope notes:** conveyor *routing/geometry* is the scene layer (T-Dog's lane) — this is the data/
  economy half only. Node-reserve depletion intentionally out of scope for mines in v1 (storage cap is
  the return-to-collect driver). Next up: **Garden v1** (build-order #5: plot→plant→tend→harvest +
  idle growth) — `PlotState`/`IsReady` already exist in Core; I'll add `PlantDefinition`-driven planting
  + tend/harvest services. Will claim specifically before starting.
- **Heads-up:** still new-files-only, no shared-file edits beyond this log; still locally unverified
  (see capability request below) — please confirm CI green.
- **Commit:** Mining v1 (this push) on `terry-local-wip`.

### 2026-06-16 (capability request) — Architect → T-Dog
- **Ask:** I currently **cannot open Unity or compile/CI-verify locally** in my environment — I've been
  writing C# by matching existing patterns and brace/GUID-checking, but I can't actually run the
  EditMode tests or confirm green before pushing. Terry wants us to have the **same capabilities**.
  Please write up (here or in a short `docs/` note) exactly how you compile / run the EditMode suite /
  confirm CI green from your setup: what's installed (Unity 2022.3.62f3? a CLI/batchmode path? the
  GameCI flow?), any env vars / license bits, and the exact commands. If it's environment config Terry
  has to flip on my side, spell out what to tell him. Until then, treat my pushes as **pattern-matched
  but locally unverified** and give them an extra CI glance.
- **Heads-up:** not blocking my work — just flagging the gap so we close it.

### 2026-06-16 (later) — Architect
- **Did:** Acked the one-branch/one-log plan — moved to `terry-local-wip`, dropped the superseded
  pod-loading seam (it stays orphaned on `claude/architect-project-onboarding-2x7h60`). Built
  **Harvest v1** (SYSTEMS_ARCHITECTURE build-order #3), pure backend, all **new files** in my lane:
  - `ResourceNodeDefinition` (Content/Definitions) — data: `resourceId`, `yieldPerHarvest`, `reserve`
    (`<= 0` = inexhaustible), `requiredFunction`, `requiredToolTier`, `biomeId`. Registry-ready.
  - `HarvestService` (Content/Economy) — pure gate `CheckTool` (function + tier + `worksOn`) + `Evaluate`
    (yield = `yieldPerHarvest × tool.power`, clamped to reserve). No scene refs.
  - `ResourceNode` (Content/Economy) — plain-C# stateful instance (`Init`/`Remaining`/`IsExhausted`);
    `Harvest(tool, profile)` depletes reserve and credits `PlayerProfile.AddResource`.
  - `HarvestServiceTests` (Tests/EditMode) — 8 tests: success / power-scaling / wrong-function /
    low-tier / worksOn gate / finite-reserve deplete+exhaust / `Evaluate` purity / invalid inputs.
- **Next-CLAIMED:** **Mining/conveyor v1 + idle accrual** (build-order #4) — backend, builds on
  `ProfileEconomy.ResolveWorld` + `IdleEngine`. Will post a specific claim before starting. Not
  touching scenes/rig/patchers.
- **Heads-up:** (1) New files only — no edits to T-Dog's files or shared files; I deliberately left
  `STATUS.md` untouched (claim free). (2) I can't compile Unity in my env, so I matched existing
  patterns and brace/GUID-checked everything — please confirm CI goes green. (3) Terry: open Unity once
  so it imports the new hand-written `.meta`s (stable GUIDs). Pairs with Dev Warp/Sandbox: warp in,
  work a `ResourceNode`, watch profile inventory grow.
- **Commit:** Harvest v1 (this push) on `terry-local-wip`.

### 2026-06-16 — T-Dog
- **Did:** (1) **Developer Warp system** — `DevWarp.WarpToScene(scene, markerId)` (dev-gated) +
  `Ziptide → Dev → Warp Window` (auto-lists every WorldPackDefinition; Open Scene / Play-here,
  per-marker) + `PlayerRigPersistence.TeleportToMarker(id)`. (2) **Sandbox Test Lab patcher**
  (`Editor/Patching/ScenePatcherSandbox.cs`, menu `Ziptide → Dev → Build Sandbox Test Lab`): builds a
  30x30 dev scene with floor, spawn, WorldRuntime, 6 named zone markers (grab/range/enemy/travel/
  artwall/loco) + a return door, and a `Sandbox_WorldPack` asset so it appears in Dev Warp. (3)
  **Per-system docs** `docs/systems/` (master README + template + Tools&Repair, Mining/Conveyor,
  Grow-a-Garden, Creatures, Build/Creator) — captures the **tool-chest/righty-tighty repair** vision.
  Earlier: step-offset fix, global fall-safety net, audit-blocker self-heal. CI-green on `terry-local-wip`.
- **Next-CLAIMED:** **In-VR Dev Menu** — a summonable world-space panel (dev gesture → list worlds/
  zones → `DevWarp`) so we can jump around *on the headset*, not just the editor. Needs a runtime
  world manifest (WorldPackDefinitions aren't in Resources yet) — I'll add a `DevWorldManifest` first.
  Gameplay/dev-tools lane; no overlap with Architect's Harvest v1. Building next.
- **Heads-up:** Terry — run `Ziptide → Dev → Build Sandbox Test Lab` once in Unity to generate the
  scene, add it to Build Settings to warp into it at runtime, and commit the new `.meta` files for the
  `DevTools/` + sandbox files Unity generates.
- **Commit:** `891680c` (Dev Warp), `3c7d7db` (Sandbox + HANDOFF), systems docs (this push).

### 2026-06-16 — Architect  ← (please add your entry here next session)
- **Did:** Set up this shared log; earlier built the backbone (see Project state). Started a
  "pod-loading seam" on branch `claude/architect-project-onboarding-2x7h60` (CI red, **superseded —
  drop it**).
- **Next-CLAIMED:** *(recommended)* **Harvest v1** — `ResourceNode` (per-biome `ResourceDefinition`)
  + a `ToolDefinition` use → adds resources to `PlayerProfile` inventory (the simplest economy loop;
  builds on `ProfileEconomy` + registries; EditMode-testable). Pure backend — no scene/rig/patcher
  collision with T-Dog's sandbox work. Pairs with Dev Warp (warp into a world, harvest there).
- **Heads-up:** please move to `terry-local-wip` (your onboarding branch is red + divergent); drop the
  pod-loading work; commit Harvest v1 here.
- **Commit:** _(add when done)_
