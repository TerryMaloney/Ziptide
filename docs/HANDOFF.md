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
  headline check — serrated blades, not green rectangles); then FORGE II is envelope-complete
  (P4 look-at/stun-droop remain as polish).
- **Commit:** this push.

### 2026-07-10 (dddd18) - Picasso (Fable 5): 🌿 E5.3 FLORA opens — LeafCard + Leaf alpha + ForgeSway (engine, 1/2)
- **Why:** last unstarted FORGE II envelope; creature ladder closed (dddd17).
- **Did:** ① `ForgeOp.LeafCard` — two crossed vertical quads, EACH double-sided (8 tris, ≤2 overdraw
  by construction), base at y=0; box-planar UVs already map each plane across the full island so no
  ForgeUV change was needed. size.z unused (validation exempt like Frustum). ② The **Leaf style now
  owns the albedo ALPHA**: `ForgeTexture.LeafAlpha01` bakes a sine-profile blade with serrated fBm
  margin + stalk (soft 2-texel edge → clean clip mips); gutter dilation carries alpha for free.
  ③ Alpha-CLIP (cutout, opaque queue — no transparency sorting on Quest) flips on via
  `HasLeafSlot(recipe)` in BOTH material consumers: ForgeBaker + the booth preview. ④ `ForgeSway`
  (Visuals/Runtime) — 1–3 pivots, pure `SwayRotation` (incommensurate 1:1.37 axes, per-instance
  phase), Antenna-role trick at plant scale. ⑤ 5 tests (ForgeFloraTests) + metas.
- **Next (commit 2/2):** 2 plants (W005 frond, W001 canal reed) + 3 props (patched crate, pipe
  cluster, dispatch console) in ForgeRecipeLibrary; wire WorldDressingBuilder's `Tufts` scatter to
  the plant look via the ForgeModuleLook pattern; booth-verify everything.
- **Commit:** this push.

### 2026-07-10 (rb20) - Reasonbox/Fable 5: 🛋️ INTERIORS TRANSLATOR ships — furnish + per-room portal cull (1.3e ②)
- **Why:** rb19's cores are CI ✅ at `1dc5fc8`; this is the translator layer (LAW 2 order held).
- **Did:**
  - **`InteriorFurnisher.cs` (Editor, NEW — mine):** the furnish translator — every
    `RoomFurnishCore` kind becomes MULTI-PART primitives (LAW 6: a cot = frame+mattress+pillow,
    a shelf = uprights+boards+clutter, a workbench = top+legs+vise+tools; 16 kinds). Per-room
    `Room_<i>` parents = the portal groups. ONE root BoxCollider per solid item (children
    stripped — collider count is the physics cost); flat kinds (floor_drain) get none.
  - **`InteriorBuilder.cs` (architect's, additive):** calls the furnisher after walls; RoomLights
    now parent into their room's portal group (a culled room takes its light with it); the plan's
    room/corridor rects serialize onto `InteriorCullRuntime` (data survives the bake, listeners
    don't — the SalvageCache lesson); log gains `furnishings=`.
  - **`InteriorCullRuntime.cs` (architect's, additive):** per-ROOM portal culling on top of the
    1.3d proximity cull — rebuilds the plan from serialized rects, finds `Room_<i>` groups, and
    on the same 0.5s cadence draws only `InteriorVisibilityCore.VisibleRooms` (own room +
    corridor neighbors; corridor/outside = everything). Walls/shell stay drawn. OLD BAKES SAFE:
    no rects or no groups → proximity-only, exactly as before.
- **🔧 Terry:** runbook §2q — W002 re-bake picks it up (or the next CI APK auto-regen does).
- **Next-CLAIMED (1.3e ③):** interior audit rule (`Editor/Audit/InteriorAuditRules.cs` — rooms
  reachable + furnished floor + portal data present, gap #4) · door thresholds if capacity.
- **Commits:** this push.

### 2026-07-10 (rb19) - Reasonbox/Fable 5: 🚪 CLAIM + cores — INTERIORS TRANSLATOR (Fable #3, 1.3e)
- **Why:** Terry's explicit ask ("once you are completely finished please work on #3"); hwr19
  lists it unclaimed. 4.3e is CI ✅ at `d2c3a5f` — ecology lane parked green.
- **Scope survey:** the architect's 1.3 stack already ships walls (`InteriorMeshCore` +
  `InteriorBuilder`), caches (1.3c) and whole-interior proximity culling (1.3d). What PRIORITIES
  #3 calls "the scene/portal/furnish translator" that REMAINS: per-room furnish (rooms are bare
  walls + a light panel — the map gate specs "furnished ≥N props per room"), per-ROOM portal
  culling (hwr5/hwr6 both said "portal culling before interiors go dense"), door thresholds, and
  the interior audit rule (gap #4).
- **CLAIMED files:** `Content/Runtime/City/RoomFurnishCore.cs` + `InteriorVisibilityCore.cs`
  (new) · `Tests/EditMode/InteriorFurnishCoreTests.cs` (new) · upcoming edits to the architect's
  `Editor/Patching/InteriorBuilder.cs` + `Gameplay/Runtime/World/InteriorCullRuntime.cs`
  (additive translator layers — architect, shout if you're returning to these) · later an
  `Editor/Audit/InteriorAuditRules.cs`.
- **Did (this push — pure cores + tests, LAW 2):** `RoomFurnishCore` — deterministic furnish
  planner: roles from the plan's own shape (nearest-entry Foyer · largest Common · rest cycle
  Quarters/Workshop/Storage by depth), 5 role catalogs (bench/table/cot/workbench/crate… each a
  real footprint), wall-hugging placement with proofs-by-construction: in-room, corridor-mouth
  clearance 0.3, item gap 0.25, center walk lane (scaled for cabins), 35% floor budget, and a
  BARE-ROOM GUARANTEE (crate fallback → flat floor-drain that can't block anything).
  `InteriorVisibilityCore` — the portal step: in a room you see it + corridor-shared neighbors
  only; in a corridor/outside everything draws. 10 EditMode tests pin all invariants.
- **Next-CLAIMED:** the translators — furnish builder (multi-part primitives per kind, per-room
  `Room_i` parents) in `InteriorBuilder`, per-room culling consuming `InteriorVisibilityCore`.
- **Commits:** this push.

### 2026-07-10 (rb18) - Reasonbox/Fable 5: 🐛 ECOLOGY 4.3e — live dawn/dusk + respawn-safety + emerge/burrow
- **Did:** `EcologyDirector.cs` only (my claimed file): (1) **live clock** — the census re-resolves
  every 300s via InvokeRepeating, so dawn/dusk cast changes happen DURING a session, not just at
  load; (2) **respawn-safety fix (self-caught 4.3b bug)** — the census now skips any creature where
  `!IsAlive`: SetActive(false) on a downed creature would kill its pending `RespawnAfter()`
  coroutine FOREVER (Unity coroutines don't survive deactivation); (3) **emerge/burrow** — after
  the first (silent, at-load) resolve, wake = 0.8s SmoothStep scale-in, sleep = scale-out then
  deactivate; tweens run on the DIRECTOR and abort the instant a creature goes down so they never
  fight CreatureRuntime's crumple/respawn scales; burrow restores home scale BEFORE deactivating so
  a nest-disturb wake pops full-size; (4) nests build once (`_nestsBuilt` guard across re-resolves).
- **Heads-up:** none new — no shared files touched, additive to my own 4.3 stack.
- **Next-CLAIMED:** per Terry, the **walkable-interiors translator (Fable list #3)** — hwr19 lists
  it unclaimed/fresh-session. I'll read the architect's 1.2/1.3 stack (`InteriorMeshCore`,
  `InteriorBuilder`, `InteriorCullRuntime`, `BUILDING_INTERIORS.md`) and post a file-level claim
  before touching anything.
- **Commits:** this push.

### 2026-07-10 (hwr19) - Fable 5 architect: 🛋️ COMFORT & ACCESSIBILITY design LOCKED — Fable endgame #5 done
- **Why:** T-Dog's endgame list (rev 9) had two unclaimed FABLE-ONLY items; #3 (interiors
  translator) needs a fresh full session per async1's own note, this session is deep — so #5.
  Belt-lane status first: 4.1l `26fdf3e` CI ✅ — the whole belt stack 4.1f–l is verified green.
- **Did:** `docs/design/COMFORT_AND_ACCESSIBILITY.md` — the 0.4 presets design, decisions locked:
  5 laws (one-switch-many-dials · comfort is DEVICE-level PlayerPrefs never profile · every
  artificial motion reports or carries `COMFORT_EXEMPT` · the DEFAULTS are the store rating ·
  accessibility = redundancy not modes) · the exact Cozy/Standard/Bold dial table against REAL
  fields (LocomotionProfile turn/slide, ComfortVignette strength 1.0/0.6/0.15, zipline cap
  5.5/8/8, flight roll off/pulse/pulse, snap-yaw cadence) · standalone toggles (seated +0.35m —
  the one report-only rig touch, subtitles ON default w/ S/M/L, handedness-v1 = holster mirror
  only, haptic scale) · diegetic console (W000 + Quarters, tile idiom; the tutorial's first beat
  per ONBOARDING law 5) · `ComfortCoverageTests` gate (preset resolve pinned literally = the
  store rating is a test; motion-source manifest must ReportExternalMotion or declare exemption)
  · the Moderate-rating defense + the one-line Cozy-default fallback.
- **Blackboard:** PRIORITIES #5 struck (now OPUS-READY) · map row updated (gap → console+gate,
  Opus-ready) · board 0.4 carries the spec pointer. Docs-only push — no CI risk.
- **📣 Next operator (any model):** 0.4 build = ComfortSettings static + console patcher + gate,
  all specced; ONLY the seated-mode offset needs the report-only protocol. Fable-only remaining:
  #1 headset-run support (reserve) · #3 interiors translator (fresh session).
- **Commits:** this push.

### 2026-07-10 (async1) - T-Dog/Fable 5: ⏳ async-travel DESIGN locked (Fable #4) — usage-limit handoff
- `docs/design/ASYNC_TRAVEL.md`: LoadSceneAsync hidden inside THE ZIPTIDE crest (activation only at
  progress≥0.9 AND full cover), 20s never-wedge timeout, ~15-line diff confined to TravelCoroutine,
  report-only protocol spelled out. Terry pre-approved the direction; implementer still announces the
  exact diff. Fable list now: #1 headset support (tomorrow) · #3 interiors translator (needs a full
  fresh session) · #5 comfort presets design. I hit the usage window here — boards are clean, CI was
  green at `e62c1a6`, this push is docs-only.

### 2026-07-10 (tut1) - T-Dog/Fable 5: 🎓 TUTORIAL DESIGN LOCKED — Fable endgame item #2 done
- **Did:** `docs/design/ONBOARDING_TUTORIAL.md` — the full onboarding design: 5 laws (one verb per
  moment · diegetic only · hesitation-triggered so doers are never nagged · flag-per-beat ·
  COMFORT BEFORE MOTION — the comfort console is the first interactive moment, which is what makes
  the store comfort rating honest), the teaching river (W000 ship: look→comfort→move→grab→holster
  →helm→PUNCH IT; W001: door→job→shoot→creature/non-lethal→zipline→save reassurance; later systems
  teach at their own first encounters via existing first-use diag hooks), an OPUS-READY build shape
  (TutorialBeatDefinition data + one auto-ensured TutorialDirector riding the EXISTING flag/RILL
  systems — no new architecture) and the coverage gate to ship with it. Map row ⬜→🦴 design-locked.
- **📣 Opus/next operators:** this is now buildable without judgment calls — the doc IS the spec.
  PRIORITIES rev 9 Fable list: #1 headset support (tomorrow) · ~~#2 tutorial design~~ ·
  #3 interiors translator · #4 async-travel design · #5 comfort presets design (the tutorial's
  comfort console DEPENDS on 0.4 — good pairing for one session).
- **Commits:** this push.


### 2026-07-10 (succ1) - T-Dog/Fable 5: 🧭 THE SUCCESSION PASS — river fixed, store mapped, Fable endgame ordered
- **Terry's brief:** ~3 sessions left per Fable operator; save some for the headset run; make the
  read-this-first docs ACTUALLY first; make what's-not-built obvious; final base-coverage check;
  and figure out Meta Store requirements ("I don't know what all goes into that").
- **Did:** ① `PRIORITIES.md` rev 9 — the succession order, split FABLE-ONLY (headset-run support ·
  tutorial design · interiors translator · async-travel design · comfort presets) / OPUS-READY
  (everything specced+gated) / TERRY (store paperwork, startable NOW). ② THE RIVER: START_HERE's
  state box refreshed to 2026-07-10 and now walks the exact chain CLAUDE.md → START_HERE → MAP
  (⬜/🦴/🕳️ = what's left) → PRIORITIES → board → HANDOFF. ③ **`META_STORE_READINESS.md`** (new):
  the full store checklist — VRC-class technical checks (72Hz, ARM64, entitlement check REQUIRED,
  release keystore, minimal permissions, DevMenu must be build-flagged off), Terry's paperwork
  (privacy URL, Data Use Checkup, IARC rating — non-lethal combat is a ratings ASSET), in-game
  store-facing gaps (title/save slots, comfort presets, tutorial, credits), comfort-rating honesty,
  and the App Lab dry-run path. Every claim flagged for re-verification against live Meta docs at
  submission. ④ Map §8 SHIP & STORE added (6 rows) — the base-coverage sweep found the last
  uncovered aspects: entitlement/Platform SDK, release-build hygiene, store assets.
- **📣 Every operator:** PRIORITIES rev 9 is the burn order. Fable sessions: hardest-only. Opus
  sessions: map + boards carry everything you need.
- **Commits:** this push.


### 2026-07-10 (gaps1) - T-Dog/Fable 5: 🔒 gate gaps #3 + #5 closed — story-beat coverage + board staleness
- **Did:** `GateGapTests` — ① every shipped story world must carry authored jobs/beats
  (`WorldJobLibrary.HasJobsFor`, new additive accessor; ToxicCity documented as covered by the
  legacy `ToxicCityContractBuilder` path — the gate FOUND that split on its dry run, which is the
  gate working). A new world can no longer ship as scenery-without-story. ② dated 🟡 board claims
  older than 14 days fail CI — finish, re-date with a HANDOFF note, or release the row (🔴/⬜).
  Stale claims silently block lanes; now they can't rot unseen.
- **📣 All lanes:** if you hold a 🟡 row, its claim date is now load-bearing — re-date when you
  genuinely resume, release when you move on. Undated 🟡 rows are NOT gated in v1 (add dates as
  you touch them). Gap queue remaining: #1 budget floor (Picasso's Validate()), #4 interior audit
  (blocked on interiors existing), #6 UI readability, #7 haptics checklist, #8 accessibility doc,
  #9 behavior-count (coordinate with Reasonbox's live 4.3), #10 catalog breadth.
- **Commits:** this push.


### 2026-07-10 (proof1) - T-Dog/Fable 5: 🛡️ CRASH-PROOFING SWEEP — "if everything breaks we're screwed" (Terry-approved)
- **The sweep, honestly reported.** Checked every breakage class I know against the actual code:
  **already solid ✓** — pause/quit/travel autosaves (Quest's pause-not-quit path covered), corrupt-
  tolerant profile parse, dup-singleton guards, no-reflection law, travel-wedge `_travelling` guard.
  **FIXED this push 🔧** — ① saves were NON-ATOMIC with no backup (`File.WriteAllText` over the only
  copy: a battery death mid-write = silent total wipe) → `SaveFileStore` atomic tmp→swap→.bak +
  `TryDeserialize` (knows corrupt from fine, incl. the hollow-`{}` trap) + load falls back to .bak
  (`SAVE_RECOVERED_FROM_BACKUP`); ② `LoadScene` on a scene missing from Build Settings silently
  no-ops AFTER travel's side effects → pre-flight `CanStreamedLevelBeLoaded` abort in
  TravelCoordinator (📣 travel is report-only per CLAUDE.md — Terry explicitly authorized this
  stability touch this session; minimal, before any side effect); ③ nothing gated Build-Settings
  drift → `EveryTravelTarget_IsInBuildSettings` (conquest's 12 mission targets + core scenes;
  PendingFirstBake ledger for Terry's unbaked runbook scenes).
- **📣 Remaining risk classes, named not buried:** (a) ~~static-event unsubscribe hygiene~~ —
  **RATCHET SHIPPED (follow-up push): `EventHygieneTests`** — static events are DISCOVERED from the
  source (no stale hand-list); every subscriber must unsubscribe or sit on a justified ledger (3
  static boot hooks, incl. Reasonbox's EcologyDirector + GamePool — verified safe: a static method
  hooked once can never dangle). Codebase verified CLEAN today; the gate keeps it that way.
  (b) `LoadScene` is synchronous — a HITCH on big worlds, not a crash (async travel = a future
  travel-lane task, report-only); (c) localization decision still open (Terry).
- **Commits:** this push.


### 2026-07-10 (health1) - T-Dog/Fable 5: 🩺 RUNTIME HEALTH — the forgotten architecture (frames, memory, THE JANITOR)
- **Why:** Terry: "find the next big thing that isn't set up… the hardest stuff or something we
  completely forgot." Found it: the game measures NOTHING about its own runtime — and 57 files
  create runtime Materials/Textures/AudioClips while `Resources.UnloadUnusedAssets` was called
  NOWHERE. Unity never auto-destroys runtime-created resources ⇒ every travel leaked orphans ⇒ long
  VR sessions marched toward OOM, and the richness bar was ACCELERATING it. Invisible until it
  crashes a headset.
- **Did:** ① `FrameStats` (Core, pure): sliding-window frame vitals — avg, worst, percentiles,
  1%-low FPS, dropped-vs-72Hz-budget; headless-tested. ② `RuntimeHealthMonitor` (Core,
  auto-ensured): 30s vitals line + memory census (`ZIPTIDE: HEALTH …`), `HEALTH_SLOW` when 1%-low
  dips under 60, and **THE TRAVEL JANITOR** — after every world load, `UnloadUnusedAssets()` sweeps
  orphans and logs exactly what it freed (`HEALTH_SWEEP`). Deliberately OUTSIDE TravelCoordinator
  (travel stays report-only; this is an additive observer on sceneLoaded). ③ **The leak ratchet**
  (`ResourceDisciplineTests`): every runtime resource-creator file must show Destroy discipline or
  sit on a VISIBLE 10-file exemption ledger with a reason — new offenders turn CI red; stale ledger
  entries turn CI red too, so the debt can only shrink. ④ Map: new "Runtime health" row (🧱, gated)
  + a "Localization readiness" ⬜ row — the OTHER forgotten thing: all player-facing text is
  hardcoded literals; Terry decides English-only vs a TextTable seam BEFORE M5 scales to 80 worlds.
- **📣 Every lane:** your files are on the ledger if they create resources without cleanup (ItemFactory,
  ForgeMaterials, BeltRig, PvpBot + 6 more) — the janitor covers today's behavior, but if you touch
  those files, add cleanup and DELETE your ledger line. Runbook has Terry's 5-minute soak test.
- **Commits:** this push.


### 2026-07-10 (dddd17) - Picasso (Fable 5): 🧬 v5.2 VERIFIED + v5.3 BREATH — "moving and breathing"
- **v5.2 verdict (run `29102529936`, CI green `34f7e35`):** the segments raise reads — bug carapace
  and warden torso/dome are smooth, grazer bell rim far cleaner. Silhouette tell closed.
- **Did (v5.3):** the literal other half of Terry's bar was BREATHING. New pure
  `ForgeGaitMotor.BreathScale(seed, time, speed01)` — slow chest oscillation as a root-bone scale
  multiplier (XZ swell + counter-Y so volume holds, ~1.2% amplitude, 0.27 Hz), deepest at idle and
  fading 60% at full run; `ForgeCreatureAnimator` composes it onto the root bone every LateUpdate
  (rotations stay the motor's; bind poses untouched; per-INSTANCE phase seed so a pack never
  breathes in lockstep). 2 new contract tests in ForgeGaitMotorTests. The motor's root-rotation
  law is unchanged — breath is a separate scale channel.
- **Heads-up:** the booth can't photograph motion — breath is verified by the pure tests; the feel
  check rides the existing runbook creature item (watch a still creature's chest).
- **Next:** E5.3 flora (last unstarted FORGE II envelope); look-at + stun-droop remain on P4.
- **Commit:** this push.

### 2026-07-10 (dddd16) - Picasso (Fable 5): 🧬 v5.2 — the last "simple" tell: faceted silhouettes
- **Why:** the roster passes, but every hero blob (grazer bell rim, bug carapace, warden dome) still
  shows polygon edges in silhouette — the one remaining "low-poly proxy" read.
- **Did:** ForgePart segments cap 16→32 (`Validate()` + tooltip; the CLASS TRI BUDGET remains the
  real perf gate — a part can spend segments only inside its class's triangles) + rounded the hero
  parts of all 6 genomes (bell 24 · bug carapace 22 · warden torso/head 18 · knot 18 · mite body 16 ·
  molter carapace 16; limbs untouched — capsules already read smooth). Committed recipe assets are
  untouched (hash-stable); the two committed body assets pick it up via the queued reseed.
- **Next:** verify v5.2 turnarounds, then E5.3 flora (last unstarted FORGE II envelope).
- **Commit:** this push.

### 2026-07-10 (dddd15) - Picasso (Fable 5): 🧬 WARDEN ✅ AT LAST (v5 verdict) + v5.1 polish
- **v5 verdict (run `29101201206`, CI green `a0396d4`):** the arms killed the bin — the warden now
  reads as an armored bipedal sentinel (split pauldrons, elbows + fists, knees, eye burning between
  the shoulder plates). Grazer's mottle v1 was invisible: 15% darkening dies under the Slime
  specular sheen. Tendril's Slime bulb unaffected — no regressions.
- **Did (v5.1, small):** warden fists slate not bright-steel (read as WHITE GLOVES) + chest seam
  narrowed 0.10→0.06 (read as a door gasket); grazer underbell+skirt switched Slime→GlowPanel soft
  green — the LANTERN under the bell the name always promised; Slime mottle deepened (×0.58) and
  widened (0.5–0.8 coverage) so it survives the highlight.
- **v5.1 VERIFIED (run `29101754484`, CI green `171e87c`):** warden ✅✅ (slate fists, thin seam —
  clean sentinel, ladder CLOSED for it) · grazer ✅ (the lantern ring reads as a luminous band; the
  mottle is subtle under booth light — the true test is emission in the DARK cistern, queued 🎮).
  **All 6 forged creatures now pass at this engine tier.**
- **Next:** segments-cap raise + E5.3 flora are the remaining dials, per SPRINT_ART P3.5/E5.3.
- **Commit:** this push.
### 2026-07-10 (rb17) - Reasonbox (ecology lane): 🥚 4.3d — NESTS: the species get HOMES you can find and regret (4.3c CI ✅)
- **4.3c verdict:** pressure persistence is **CI GREEN ✅** (`a5cae95`) — the ecology stack
  (engine → director → save) is fully verified.
- **Did (the LAW 6 centerpiece):** `NestRuntime` — a nest is a PLACE, not a marker: a three-layer
  grown mound with an entrance hollow, a clutch of brood-glow eggs BREATHING light inside (calm and
  slow intact; fast and hot once disturbed), and a warning ring of clawed territory stakes.
  Placement is the ecology's own math — `EcologyCore.NestSitesFor` puts the first nest at the
  population's HEART (centroid-nearest home) and the rest at its far ranges (greedy farthest-point;
  territory spreads, never clumps — pure + tested). The director builds them per species at world
  load, beside the residents' own ground. **Disturb the clutch** (select the center egg) and the
  WHOLE species wakes — including the denned-up individuals the day/night census left sleeping: the
  world answers for its young. Non-lethal, kid-fair: the eggs stay (glowing angrier), you get a
  small spore find, nothing is destroyed. Logs `ECOLOGY_NEST` / `ECOLOGY_NEST_DISTURBED`.
  12 ecology tests total.
- **🎮 Terry (no menu step):** in any fauna world, follow a species to its mound — poke the glowing
  center egg and watch the world wake up around you. Feel notes: nest size/read, egg pulse, whether
  the all-wake response feels fair or brutal.
- **Next in this row (mine):** pack placement anchored to nests (packs sleep AT home, wake FROM
  home) · the tame/befriend bridge (CREATURE_ECOLOGY's optional mount tie to my vehicles seat
  layer) · migration/seasonal events from the Additions Bank.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (dddd14) - Picasso (Fable 5): 🧬 v4 SKIN VERDICTS + warden v5 (it gets ARMS) + slime depth
- **The checkpoint (run `29089183510`, CI green `fad9898`):** the texture bake WORKS —
  **swarm_bug ✅✅** (glossy amber chitin, dark inset plates, speckled legs) · **husk_molter ✅✅**
  (wet mossy carapace, pale side armor) · **tendril ✅** (bark-speckled vines, ribbed root-knot) ·
  **witness_mite ✅** · **light_grazer ⚠** (clean but PLAIN — single-layer slime blotch is too subtle
  on a pale hide) · **warden ❌** (baked NEAR-BLACK — PaintedMetal grime crushed the already-dark
  palette — and the silhouette is still a bin: it has NO ARMS).
- **Did ① warden v4 genome:** two mirrored 3-segment ARMS on `GaitRole.Leg` — the motor's per-limb
  leg ordinal offsets them half a cycle, so they swing CONTRALATERAL to the legs for free (11/12
  bones); pauldron slab split into two angled plates + bright collar; near-black visor slot (new
  palette slot 4) so the eye burns in a void; every palette slot lifted + grime dropped so panels
  and wear actually read under light.
- **Did ② Slime style depth (ForgeTexture):** second grime-scaled SUBDERMAL MOTTLE layer (finer,
  darker fBm in albedo + matching height dimples) — a wet hide now reads as a body with something
  inside it. Grazer genome: grime lifted to drive it + bell noise up. Only the grazer family uses
  Slime, so blast radius is one creature. (No pinned albedo tests; smoothness test untouched.)
- **Heads-up (booth gotcha, again):** the warden faces +Z — its visor/eye/chest-seam are in the
  `04_back` view, not `01_front`.
- **Next:** verify v5 turnarounds next forge-photos run (warden is the gate; grazer mottle second).
  If silhouettes still read low-poly, the next dial is the creature-class segments cap. E5.3 flora
  is the last unstarted FORGE II envelope.
- **Commit:** this push.
### 2026-07-10 (rb16) - Reasonbox (ecology lane): 💾 4.3c — HUNTS PERSIST: the ecology reads your save (4.3b CI ✅)
- **4.3b verdict:** the director is **CI GREEN ✅** (`e31ddeb`).
- **Did:** the loop closes — clear a zone today, it's STILL quiet tomorrow, and loud again next
  week. `EcologyPressure` moved to **Core** with a pure `EcologyPressureLedger` (a hunting spree
  coalesces into one row per species-hour; entries spend after ~48h ≈ 8 half-lives; hard cap 64 —
  a decade of play stays a handful of rows, both laws pinned by tests). `CreatureRuntime.Disable`
  records one line into the world save; the director prunes + feeds pressures into
  `PopulationsAt`. **📣 ARCHITECT — additive data-model edits in your territory, your own law
  applied:** `WorldState.ecologyPressures` (PlayerProfile.cs, neutral empty default = undisturbed
  ecology, old saves untouched) + the struct now lives in `Core/Runtime/Economy/EcologyPressure.cs`.
  **📣 M3/creature-file note:** one additive block in `CreatureRuntime.Disable` (after loot) — no
  behavior/visual paths touched. 11 ecology tests total.
- **Next in this row (mine):** the LAW 6 centerpiece — physical multi-part NESTS (mound + entrance
  + brood glow + territory markers; disturb → territorial response; the census anchors pack
  placement to them) · then pack placement + the tame/mount bridge.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (amb1) - T-Dog/Fable 5: 🔊 THE AIR HAS A SOUND — ambient audio ⬜→🧱 (the map's floor-raise)
- **Why:** Terry: "pick the next most important priority." The EXCELLENCE_MAP's own unevenness rule
  says raise the floor — ambient audio was a ⬜ the player feels every second, flagged by
  SKYSCAPE_DESIGN §7 ("Prospect's alien-place feeling is at least half wind, insects, and hum"),
  board 5.5, nobody's lane.
- **Did:** ① `AmbienceCore` (pure): `BiomeAmbience` scene→biome→spec table (10 biomes — city,
  underground, cave, exterior, forest, interior, station, coastal, void, arena — each sonically
  DISTINCT: the Bloom sings, caves drip, surf out-rumbles rock, the void's nothing has a pitch) +
  `AmbienceSynth`: deterministic **loop-exact** synthesis (every component an INTEGER number of
  cycles over the buffer → sample[N]==sample[0] by construction — no seam pops ever). Wind = 48
  integer-harmonic partials with 1/f weighting + gust LFOs; hum = snapped root + 2 harmonics +
  tremble; rumble = beating low pair; chirps/drips = decaying one-shots. ② `AmbienceDirector`
  (auto-ensured, the DevMenu idiom): sibling of AudioDirector (music stays theirs) — 4 sources,
  crossfades the 3 beds on every world load, Poisson-spaced one-shots. Zero assets, zero scene
  edits, ~1MB generated per world. ③ 6 EditMode tests incl. the coverage GATE: every shipped scene
  must map to an audible bed — a new world can never ship dead silent by accident.
- **📣 Heads-ups:** (a) hazard stingers + music stems + VO ducking remain (map row lists them) —
  claimable. (b) `AudioDirector`'s authored-music path untouched; ambience sits at 0.5 master as a
  BED. (c) New scene names must be added to `BiomeAmbience.BiomeForScene` (the coverage test's
  ShippedScenes list is the reminder — add both or CI nags).
- **Commits:** this push.

### 2026-07-10 (rb15) - Reasonbox (ecology lane): 🌗 4.3b — the ecology becomes FELT: who is abroad (4.3a CI ✅)
- **4.3a verdict:** the population engine is **CI GREEN ✅** (`fcc5c8c` run, code `107df2a`).
- **Did:** `EcologyDirector` (Gameplay/Enemies, self-bootstrapped on scene load — zero scene edits,
  zero patcher steps): takes a CENSUS of a world's baked creatures, resolves the engine, and wakes
  each species' **abroad count** — population thinned by the hour's activity, hard budget-capped,
  deterministic order (the same world wakes the same individuals). **The clock is real UTC** (the
  idle economy's real-time ethos): visit W005 at your noon and the grazers are out; come back after
  dark and the stalker is. Worlds with no baked fauna are untouched; species the ecology doesn't
  know keep their authored state — never breaks a scene. `EcologyCore.AbroadCount` added (pure,
  floored: a living species always shows at least one face). Logs `ZIPTIDE: ECOLOGY_RESOLVE
  world=… hour=… species=awake/total…`. +1 test (9 ecology tests total).
- **🎮 Terry (no menu step):** warp any story world and read the `ECOLOGY_RESOLVE` line in logcat —
  then the same world ~12h later; the cast changes. Feel notes: does a thinned world read as
  "quiet" or "broken"? (The floor guarantees ≥1 of every living species.)
- **Next in this row (mine):** disable-pressure persistence (EcologyPressure → world save, additive
  Core edit — will announce) so hunting a zone genuinely thins it across sessions · physical
  multi-part NESTS (LAW 6) with territorial response · pack-spawn placement at nests.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (rb14) - Reasonbox: 🦎 CLAIM + first cut — board row 4.3 CREATURE ECOLOGY (vehicles 3.2a CI ✅ · RICHNESS BAR read)
- **3.2a verdict:** vehicles are **CI GREEN ✅** (`16f478b`). **LAW 6 read and owed:** my v1s
  (skiff/drones/watering-can/POI pockets) are on the richness debt list — claimable rows, or mine
  after this. Ecology ships RICH from day one (a full mechanic vocabulary, not one idle bob).
  *(The 4.3a code landed as `107df2a` via API push during a sandbox shell outage — this entry
  follows it; same pattern as the 5133c5c fix.)*
- **Terry's directive:** "biggest, most complicated unbuilt aspects — make sure our system can
  build anything." The big-list scan: 4.3 ecology (unclaimed, deepest sim) · 5.4 story/endings
  (unclaimed) · 4.4 weapon depth (unclaimed, device-heavy) · 5.1/A6 online (MP lane, parked on
  Terry) · W013+ scale (the lesser-model queue once P5 locks) · LAW 6 debt (all lanes).
- **CLAIMED: row 4.3 (creature ecology).** Files: new `Content/Runtime/Ecology/EcologyCore.cs` +
  tests. Picasso: your roster/gait/genome/skin work is UNTOUCHED — this is the population layer
  above it; the species table keys your 7 authored creatureIds. Zero shared-file edits.
- **Did (4.3a — the population engine, pure, `107df2a`):** deterministic per (worldSeed,
  elapsedHours, pressures) — the save/offline law. The living model: logistic growth toward
  per-species carrying capacity · **predator/prey coupling** (tether swarms + witness mites hunt
  swarm bugs; the stalker hunts grazers; fed hunters grow, starved hunters shrink) · **player
  disables suppress and then the wild HEALS** (6h half-life — the non-lethal canon made math) ·
  **nothing ever goes extinct** (floor of one) · **day/night casts** (grazers by day, the apex by
  night, the Warden never sleeps — it is law, not wildlife) · deterministic **pack composition** ·
  a **hard active-spawn budget** (whole packs only, biggest encounter first). 8 tests.
- **Next in this row (mine):** the `EcologyDirector` runtime (world-entry resolve → packs at NESTS
  with day/night gating, replacing static ring-spawns) · physical multi-part nests (LAW 6) ·
  disable events into the world save as `EcologyPressure` (additive, will announce).
- **Commit / branch:** `107df2a` — verify the run before stacking C#.

### 2026-07-10 (hwr18) - Fable 5 architect: 🔁 4.1l — splitters ROUND-TRIP + the wand previews its whole footprint
- **Verdicts first:** 4.1j `2987a42` CI ✅ · 4.1k `fa7a2aa` CI ✅ — the whole belt stack through
  blueprints is verified.
- **Did (two hwr17 thin spots closed):** ① SPLITTERS are first-class hand objects — pickable like
  belts (pick-listener on splitter tiles incl. restored ones), they come back as a fork-striped
  splitter TILE (`BeltTileItem.kind`), and place as splitters again via the generalized
  `PlaceCellFromHand(pos, fwd, kind)` (old `PlaceBeltFromHand` delegates — no caller breaks).
  Authored-removed bookkeeping generalized (`_authoredRemovable` covers Belt+Splitter, so picking up
  an authored splitter persists correctly). ② The LOADED wand now shows its ENTIRE footprint under
  the hand — pooled ghost quads, teal when the stamp fits, red when it refuses INCLUDING off-grid
  overhang so you see why; recolors only on state change (no per-frame material churn). Empty wand
  keeps the single-cell cursor.
- **Logs:** `BELT_PLACE`/`BELT_PICKUP` now carry `kind=`.
- **Remaining thin spots (claimable):** auto-SOURCE cells still bare tiles (unused in shipped
  content) · belt placement audio (board 5.5) · wand holster affinity · dispenser only vends BELT
  tiles (splitter tiles only come from pickups/blueprints — fine for now, a design choice to revisit
  with power/tiers).
- **Commits:** this push.

### 2026-07-10 (hwr17) - Fable 5 architect: 🎨 LAW 6 applied to my own lane + 🪄 BLUEPRINTS — belt vocabulary rounds out
- **Why:** rich1 named belts directly ("audit your own v1s"). Audited; two gaps: one-primitive-per-idea
  visuals with zero motion, and the design doc's blueprint verb unshipped. Both closed.
- **Did ① 4.1j RICHNESS (`2987a42`):** belt cells are little machines — side rails + rollers that spin
  ONLY while ore flows (a still line reads jammed, which is the truth), splitter fork hubs, sinks are
  funnel DEPOTS with churning agitators, the mine port gets a breathing piston + drill that goes
  frantic-while-feeding, the conductor handle is a real lantern (cage/core/cap/loop). Cargo: pure
  `BeltPuckStyle` (FNV-1a, platform-stable) gives EVERY resource id its own deterministic shape+color
  — catalog breadth by construction, no registry; pucks bob + slow-yaw. All deco is colliderless. 3 tests.
- **Did ② 4.1k BLUEPRINTS:** pure `BeltBlueprint` — flood-capture the connected player-buildable line
  (Belt+Splitter only; ports/sinks never clone; oversized components REFUSE rather than half-capture),
  seed-anchored so the stamp lands exactly under the hand, all-or-nothing `StampInto`. The WAND
  (`BeltBlueprintWandItem` + stand at every dispenser — sandbox + all belt pads): release over your
  line = capture (head glows, "xN" label), over empty grid = stamp, repeatable; refusal = sharp buzz.
  Stamps run the NORMAL place path — 4.1f persistence (one autosave per stamp) + 4.1g caps apply
  untouched. 6 tests incl. stamped-copy-flows and seed-anchoring exactness. Logs
  `BELT_BLUEPRINT_CAPTURE` / `BELT_STAMP`.
- **📋 Thin spots (LAW 6 ¶4, claimable):** stamped SPLITTERS can't be picked back up (RemoveBeltAt is
  Belt-only — small pull) · no multi-cell stamp ghost (single-cell cursor only) · auto-SOURCE cells
  (non-port) still bare tiles — unused in shipped content · belt PLACEMENT sound/audio absent (audio
  is board 5.5) · wand has no holster affinity (travels as loose item only).
- **DoD:** cores+tests first ✓ · gate covers (caps apply to stamps) ✓ · richness ✓ · save story ✓ ·
  `ZIPTIDE:` tags ✓ · CI pending on this push ✓→verify · boards/map/runbook stamped ✓ (map row now
  💎 place/ride/persist/feed/clone, 40 tests).
- **Commits:** `2987a42` (4.1j) + this push (4.1k + docs).

### 2026-07-10 (dddd13) - Picasso (Fable 5): 🧬 CREATURES GET SKIN — the texture bake lands
- **Why:** Terry's xenomorph bar. Bodies were smooth+articulated but FLAT-COLORED; weapons carry baked
  albedo/normal/wear atlases. This closes that gap — the last big art-engine dial.
- **Did (one seam, four consumers):** `ForgeSkinnedBuilder.SyntheticRecipe(body)` — the EXACT part
  list the skinned mesh was built from (core + limb segments + eye) as a bake-able recipe with the
  body's palette/slotStyles, so ForgeTexture's maps land on the SAME UV islands the mesh carries BY
  CONSTRUCTION (Build() itself now uses the shared CollectParts/MakeSynthetic).
  · **ForgeBaker**: BakeAll also bakes every STYLED ForgeCreatureBody (maps + ONE material →
  `ForgeBaked/body_<id>/`; no mesh/prefab — the runtime builds the skinned mesh). Unstyled bodies
  skip = flat by design. `BakeMapsAndMaterial` extracted, recipe path unchanged.
  · **ForgeCreatureVisualApplier**: prefers the baked material for every non-eye submesh (the EYE
  keeps its live emissive so the ForgeBodyTell channels keep working untouched; baseColors=white so
  ClearBodyTint restores the atlas). `FORGE_CREATURE_APPLIED … baked=true/false`.
  · **ForgePhotoBooth**: styled bodies photograph with the SAME BuildSingleTexturedMaterial the
  recipes use → next run's body_* turnarounds show SKIN, and atlas x-rays co-locate in the artifact.
  · **Genomes**: all 6 styled — chitin cells (bug/mite/molter), slime sheen (grazer/tendril bulb),
  bark (tendril body), worn PaintedMetal+panels (warden), GlowPanel eye slots.
- **Runbook (already queued):** the swarm_bug/light_grazer reseed step also picks up their styles.
- **Next:** booth verdicts on the SKINNED+ARTICULATED+TEXTURED roster next run — the fox→xenomorph
  checkpoint. Then segments-cap raise if silhouettes still read low-poly; E5.3 flora remains.
- **Commit:** this push.

### 2026-07-10 (meta1) - T-Dog/Fable 5: 🗺️ THE EXCELLENCE MAP — the meta layer gets its guardrails
- **Terry's directive:** "look at the meta aspects… much harder with a lesser model… basically no
  room for a model to half-ass it… the map of everything the game is going to need… standards and
  guardrails better across the board" — so uniform quality survives the operator handoff.
- **Did ① `docs/EXCELLENCE_MAP.md`:** one row per aspect of the FINISHED game (7 sections: worlds ·
  living world · player · ship/vehicles · story · multiplayer/meta · engine/pipeline). Every row =
  honest STATE (⬜/🦴/🧱/💎) + a CHECKABLE standard + the enforcing gate — or a named **🕳️ GAP**.
  Ten gate gaps queued claimable, value-ordered (budget-utilization floor is #1 — the direct
  enforcement of Terry's "10k budget built with 1k"; Picasso's Validate(), coordinate). Includes
  THE UNIFORMITY REVIEW: each operator era, re-mark states and pick rows that raise the FLOOR.
- **Did ② DEFINITION OF DONE** in `OPERATOR_START_HERE.md`: 8 mechanical checkboxes per chunk (core
  first · gate exists or gap claimed · richness · save story · diag tags · CI green · blackboard ·
  map row updated). No judgment calls — that's the point.
- **Did ③** `CLAUDE.md` Key docs now leads with the map, so every future session trips over it.
- **📣 Every lane:** your aspects' STATE marks are my honest outside read — correct them in place if
  I'm wrong (that's the dashboard working). The gate-gap queue is open; #1 (budget floor) is
  Picasso's file, #2 (skyscape rubric audit) pairs with my sky lane, #3 (story-beat coverage) suits
  the story track.
- **Commits:** this push.

### 2026-07-10 (dddd12) - Picasso (Fable 5): 🦴 ARTICULATION — limbs get REAL JOINTS at rest
- **Warden v3 verdict (run `29087827535`):** the stance works — visible legs with knees, hip plates,
  wide shoulders. (Face/visor is on +Z = booth's 04_back, same as the walls.) Good enough to leave
  while the bigger dials land.
- **Terry's bar:** "from fox-with-a-trash-can-lid to a moving, breathing xenomorph." Two dials left
  and this push lands the first: **bind-pose articulation.** Limbs were dead-straight lines because
  the chain had ONE direction. `ForgeLimbSegment.bendDegrees` = pitch vs the previous segment around
  the limb's side axis (mirror-safe by construction — the side axis flips with the mirrored chain).
  The builder accumulates the rotation per segment (bones + geometry + bind poses all follow; the P4
  motor animates ON TOP of the articulated rest pose). Insect recurves, tentacle curls, braced
  stances — anatomy, not sticks.
- **Applied:** tendril tendrils now reach-then-CURL (35°/45°); grazer tentacles S-curl under the
  bell (28°); husk_molter gets the insect recurve (thigh -18°, shin +55°); warden braces (-10°/+18°).
- **⭐ NEXT-SESSION HEADLINE — CREATURE TEXTURE BAKE (the last big dial, spec'd):** bodies are still
  flat-colored; weapons carry baked albedo/normal/wear atlases. The skinned path ALREADY allocates UV
  islands via a synthetic recipe (ForgeSkinnedBuilder builds one for ForgeUV). The slice: (1) extract
  `SyntheticRecipe(body)` as a shared public static (parts+palette+slotStyles) so builder and baker
  agree on UVs BY CONSTRUCTION; (2) ForgeBaker also bakes each ForgeCreatureBody's maps + ONE material
  to ForgeBaked/body_<id>/; (3) ForgeCreatureVisualApplier prefers the baked material (collapse
  submeshes to one at apply time) with flat-mats fallback; (4) author slotStyles per genome (Chitin
  cells for bugs, Slime sheen for the grazer/tendril, PaintedMetal wear for the warden). Then
  segments-cap raise (16 → 24 for creature-class parts) if silhouettes still read low-poly.
- **Commit:** this push.

### 2026-07-10 (rich1) - T-Dog/Fable 5: 📣 ALL LANES READ THIS — THE RICHNESS BAR (Terry) + Tidefront proof
- **Terry's device verdict (via Picasso's photos):** too much is landing at ~10% of its class budget —
  boxy, "one primitive per idea," and skeleton vocabulary in mechanics too. His words: "make sure
  everything we build is complex and cool/realistic shaped… that includes gardens, vehicles, machines
  etc… vocabulary across the board needs to be extended — not just looks but also mechanics/movements."
- **📣 Codified as LAW 6 in `OPERATOR_START_HERE.md` (THE RICHNESS BAR):** ① use the budget (a 10k
  class asset at 1k tris is a placeholder, not a ship) — full Forge op vocabulary, multi-part
  silhouettes, asymmetry; ② surface the whole catalog (shipping 2 of 8 entries = a skeleton wearing
  a coat); ③ motion/mechanics get the same rule (one idle bob ≠ a behavior set); ④ placeholder-first
  still unblocks mechanics, but a shipped placeholder is now a claimable board row, listed in HANDOFF.
  **Every lane: gardens, vehicles, machines, creatures, belts, props — audit your own v1s.**
- **Walked the talk in my lane (`a281e59`):** mission verbs 2→5 (Sabotage/Scan/Beacon attack ·
  DroneDefense/Repair defense — shoot, HOLD GROUND, CARRY, shoot flyers, hands-on fix; char-sum
  deterministic per world, catalog-span test guards it); mission props became multi-part rigs with
  motion (pylons with counter-orbiting emitters that slump when killed, spinning scan dishes with
  closing hold-rings, a legged beacon with a spinning heart + breathing uplink pad, conduits that
  flicker angrier until slapped home, scouts with rotors + eyes + size variance); the war table now
  surfaces the FULL 8-defense/8-vessel catalog as data-driven build tiles + STANDING defenses on
  planet cards.
- **Commits:** `a281e59` + this docs push.

### 2026-07-10 (dddd11) - Picasso (Fable 5): 🎨 CREATURE QUALITY PASS — "too boxy" fixed at the ENGINE
- **Why (Terry, direct):** "the creatures still look pretty boxy and extremely simple… not something I
  would ship." He's right, and the cause was structural, not per-genome: (1) ForgeSkinnedBuilder
  FLAT-SHADED everything — it ignored `part.smooth`, so even OrganicBlobs rendered as faceted lumps;
  (2) every limb segment was a hardcoded BeveledBox (line 185); (3) genomes used ~5% of the 10k
  creature tri budget.
- **Did (engine):** the skinned builder now HONORS `smooth` (indexed verts + area-weighted accumulated
  normals — the ForgeMesh idiom, with rigid weights per vertex); `ForgeLimbSegment` gained
  `rounded` (smooth tapered CAPSULE whose dome caps overlap the joints — knees/elbows read for free)
  + `taper` (claw/tentacle tips). Back-compat: default false/0 = the old box.
- **Did (genomes, all 6):** every limb rounded+tapered (18 segments); blobs upped to 14–16 segments;
  detail parts added — swarm_bug shell ridges + noised carapace/head, light_grazer Torus skirt +
  noised bell, witness_mite back-spike cluster (tapered capsules), husk_molter second ridge fin +
  angled side plates, warden rounded armor-column legs, tendril fully organic tendrils.
- **🔧 Runbook:** swarm_bug + light_grazer assets are COMMITTED (create-only) — delete + reseed step
  queued so their limbs pick up the capsules (the smooth-shading fix reaches them regardless).
- **Next:** booth verdicts on the whole roster next run (incl. warden v2). If blobs still read too
  faceted at segments 16, the cap is `ForgePart.segments ≤ 16` in Validate — raising it for
  creature-class parts is the next dial. Textured (styled) creature skins = the dial after that
  (slotStyles are authored-empty today; the bake path needs skinned-mesh support — boarded).
- **Commit:** this push.

### 2026-07-10 (tf4) - T-Dog/Fable 5 (Tidefront lane): 🎭 B4 HOTSEAT — two admirals, one headset
- **Why:** the last unclaimed row in my lane, and the spec's own build order (B1 sim → B2 table →
  B3 missions → **B4 hotseat** → Photon).
- **Did:** HOTSEAT tile on the war table. When on: END TURN doesn't run the AI — it's the HANDOVER
  ("PASS THE HEADSET — RIVAL (RED) ADMIRAL"): the table flips perspective through a single
  `_activeSide` (every build/attack/odds/fog/rack read goes through it; solo keeps it 0 so the old
  path is byte-identical), fog re-fogs to the new side's knowledge, the rack re-deals their fleet.
  Full round = both halves then `EndTurn()`. Mode + active side ride `ConquestSession` (travel-safe)
  AND the save — a new tolerant `H=` record (`ConquestSave.ReadMode`; unknown keys were already
  skipped, so old saves read solo and old parsers never see it). NEW WAR keeps hotseat for the
  rematch; toggling off mid-game hands red back to the AI safely. B3 missions work for whichever
  admiral attacks (defense offers stay solo-only v1 — pass-the-headset mid-battle is a decision, not
  an accident). Tests: the H-record round-trip incl. clamping. Diag: `WARTABLE_HOTSEAT/HANDOVER`.
- **📣 Heads-up:** the Tidefront pillar is now sim→table→missions→save→fog→rack→hotseat COMPLETE
  through the spec's pre-Photon scope. Remaining seams: Photon live sync (gated on A6) · the
  space-flight defender mission (3.1 ship weapons EXIST now — Reasonbox's seam, or claimable with a
  loud HANDOFF if their flight lane stays parked in gardens).
- **Commits:** this push.
### 2026-07-10 (rb13) - Reasonbox: 🛺 CLAIM + first cut — board row 3.2 DRIVABLE VEHICLES (4.2d CI ✅)
- **4.2d verdict:** hazard gardens are **CI GREEN ✅** (`df612a6`) — the whole garden stack is green.
- **Why this lane:** Terry — "next most important." By the board's own phase order the earliest
  unclaimed row is **3.2 vehicles** (Terry's explicit hardwiring ask; "a vehicle is a ship for the
  ground"). Scan: architect closed 4.1 · traversal in caves · T-Dog on skyscape/Tidefront · Picasso
  finished the creature roster. Zero overlap — new files + my claimed POI/flight territory.
- **CLAIMED: row 3.2.** Files: `Content/Runtime/Definitions/VehicleDefinition.cs` (new) ·
  `Ship/Runtime/VehicleRuntime.cs` (new) · `Editor/Patching/VehicleAuthor.cs` (new) · Transit-POI
  spawn inside my `WorldPoiBuilder` · **one additive hook in `BuildAndroid`** (VehicleAuthor beside
  GardenAuthor — announcing the shared-file edit).
- **Did (v1 — mount, ride, dismount):** the ride reuses the ship's comfort-clamped `FlightModel`
  with a **GROUND PROFILE: pitch rate AND clamp are ZERO by construction** — no data, no input, no
  bug can tilt a seated rider's horizon (pinned by a 10-simulated-second full-input test). One
  control language with the helm: stick drive (back = reverse), hold-to-repeat snap turn, L3/A
  boost. Unlike flight's world-moves render, the ride moves THROUGH the world: terrain-hugged by
  raycast + hoverHeight, the rig teleport-follows the seat (never parented), locomotion suspended,
  and the ONE ComfortVignette covers it through its normal rig sampling — zero new comfort wiring.
  Roam is soft-walled per vehicle (can never exceed the flight lane bound). **Starter fleet of 3**
  (tide skiff · dune hoverbike · cavern crawler — each biome family gets a signature ride,
  create-only under Resources/Vehicles) and **every Transit POI now parks its biome's ride** — my
  1.5 verb grew its vehicle. Logs `VEHICLE_MOUNT/VEHICLE_DISMOUNT`. 4 tests.
- **Next in this row:** per-archetype feel (drift/suspension/buoyancy) after Terry's first ride ·
  vehicle wraps via the cosmetic layer · garage/persistence · races as world events.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (sky1) - T-Dog/Fable 5: 🌅 SKYSCAPE v1 — the air is real (W005 signature proof)
- **Why:** Terry: "pick something that needs to get done… make something cool." The skyscape is HIS
  stated reason for making the game, `SKYSCAPE_DESIGN.md` said "nothing built yet," nobody had claimed
  it. Executed the doc's own §6 step 1: full stack on ONE world (W005), gut-check gate before rollout.
- **Did:** ① `Visuals/SkyAtmosphereCore` — the §4.1 hazard→atmosphere defaults table AS CODE (all 16
  tags: Bloom dense spore air · radiation deliberately thin · wind dragged streaks · pattern GLITCH
  motes that step instead of glide · swarm/void = correctly nothing) + `MotePosition(seed,index,time)`
  pure drift math (LiftCycle idiom — no sim state). 8 EditMode tests pin the design's own laws.
  ② `SkyAtmosphereRig` — 3 renderers, 0 lights: haze ring mesh at 40m (real distance, NOT the dome —
  the stereo-depth law from §1), mote field as ONE dynamic mesh at 8–15m whose verts are evaluated
  from the pure function every frame (the tested math IS the visual), soft additive glow quad behind
  body 0. Motes follow the player with lag so leaning buys true parallax; all mats double-sided.
  ③ `SkyVistaDefinition.atmosphere` block (additive; authored as hazardTag+intensity so §4.1 stays
  the single source of truth) + SkyVistaRig hook. ④ W005 = Signature tier: Bloom atmosphere, body
  glow, AND the pillar-4 light coupling (amber directional + ambient — the sky's color reaches the
  ground for the first time).
- **📣 Heads-ups:** (a) W005's vista asset is create-only — runbook has the delete+reseed step; the
  new fields default OFF so all other vista assets are untouched. (b) Picasso: I touched no Forge/art
  files — this rides the existing SkyVista system I retuned in DEPTH-2. (c) Rollout to the other 11
  worlds + the occlusion-composition hook stay GATED on Terry's W005 on-device verdict (the doc's own
  order). Diag: `ZIPTIDE: SKY_ATMO`.
- **Commits:** this push (core+rig+authoring+tests, docs).

### 2026-07-10 (hwr16) - Fable 5 architect: 🏁 ROW 4.1 COMPLETE — persistence, the gate, CONDUCTOR MODE, belts in real worlds
- **Context:** previous architect chat was lost mid-session (flagged, no fault found); this session
  re-oriented from the boards alone — the blackboard worked exactly as designed. Same lane resumed:
  the four remaining pulls from hwr15, all four shipped.
- **Did ① 4.1f PERSISTENCE (`9f88bd4`, CI ✅):** player-built belts survive quit/reload. Dynamic
  overlay in the profile (the ConquestSave idiom): authored layout stays canonical + only player
  edits stored (`BeltFloorSave` pure core in Core, `WorldState.beltFloors` additive,
  `BeltFloorRuntime.floorId`, sandbox=`sandbox_belt` retrofit-safe). Neutral default pinned: empty
  overlay = pre-4.1f behavior exactly; a NEWER authored layout wins stale saves. 10 tests incl. the
  design doc's "factory persists" JSON golden round-trip. Logs `BELT_RESTORE`.
- **Did ② 4.1g THE GATE (`02acf1b`, CI ✅):** `AutomationAuditRules` in the world audit — floor grid
  area caps at 256 cells / scene cells at 256 (BLOCK; the grid structurally bounds buildable item
  count, so the area IS the item budget), authoring typos caught in CI (OOB cell = silent runtime
  no-op → BLOCK; dup cell → warn), and 4.1f save-identity integrity (shared floorId → BLOCK). Exact
  data counts, so blocking outright — no baselining window. 8 tests.
- **Did ③ 4.1h CONDUCTOR (`04b7c02`, CI ✅):** the design doc's signature hook. Pure `BeltRoute.Trace`
  (routes follow items exactly: corners, splitter-primary, sink terminus, loop guard) +
  `ConductorRide` (glide at the ore's pace, boundary-crossing counter). `BeltConductorRuntime` =
  the ZiplineRuntime idiom verbatim: teal lantern, re-trace at grab (hand-built extensions ride
  too), rig delta-translated NEVER parented, one haptic click per cell lip, release to step off.
  Sandbox post beside the mine port. 8 tests. Logs `BELT_RIDE_START/END`.
- **Did ④ 4.1i WORLDS (`ad97429`, CI ⏳ at write time):** belt pads become PACK DATA (the
  mines/gardens idiom): `BeltFloorSpawnDefinition` + `pack.beltFloors` (additive) +
  `BeltPadSpawner`. 📣 **ANNOUNCED APPENDS (T-Dog/worlds):** ONE line in `JobDirector.Start`
  (spawner owns all logic) + ONE call in `WorldStubGenerator` after `EnsureJobsFor`
  (`BeltPadLibrary.EnsurePadsFor` — derives a "connect the mine to the depot" pad beside each
  pack's first mine, spec-is-truth regenerated, W002 gets the first). Intake binds the mine's OWN
  hopper — economy-safe, no double-pay. Grid clamps to the 4.1g cap. 5 tests. Logs `BELT_PAD`.
- **🎮 Runbook §2q added** — persistence feel, the conductor ride, the W002 pad walkthrough.
  MASTER_CHECKLIST updated (belts 4.1a–i complete, CI-green, awaiting device pass).
- **Next (this lane):** verify the `ad97429` run → then the lane's fresh pulls: INDUSTRY_50 bank
  (blueprints/copy-stamp is the natural 4.1j), sprinklers→garden bridge (Reasonbox asked, rb9),
  GamePool for wall-chunk debris if Picasso wants it, or jump to the PRIORITIES top. Belt budget
  numbers are in `AutomationAuditRules` consts — Picasso, fold them into the QUEST budget doc if
  you want one home for all caps.
- **Commits:** `9f88bd4` · `02acf1b` · `04b7c02` · `ad97429` + this docs push.
### 2026-07-10 (rb12) - Reasonbox (garden lane): ☢️ 4.2d — hazard gardens breed GIANTS live (4.2c CI ✅)
- **4.2c verdict:** the watering can is **CI GREEN ✅** (`c259631`).
- **Did:** the hazard hook goes LIVE — `GardenSpawnDefinition.hazardStrength01` (additive pack field,
  default 0 = safe garden, old packs untouched): a non-zero planter mutation-kicks every seed at
  plant time through `PlantGenetics.HazardKick` (seed stable per plot+timestamp — a save replays the
  same genes). Radiation gardens are now WHERE GIANTS COME FROM, exactly as GARDEN_AAA.md drew it.
  And giants READ: past the size threshold the plant visual grows toward **2.2×**, the readout says
  **★ GIANT**, and `GARDEN_HARVEST … GIANT` / `GARDEN_GIANT` log the moment. World authors: set
  `hazardStrength01` on any pack's garden entries (W004's dead-screen dread wants ~0.7).
- **Still queued in this row:** the true two-handed giant PULL (deserves device iteration — after
  Terry's first garden feel pass) · seed items from `CrossPlots` into the belt/pouch · sprinklers
  bridging to the architect's now-closed belt loop (mined ore rides belts — giant produce should too).
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (hwr15) - Fable 5 architect: ⛏️ 4.1e — machine PORTS ship green: mined ore rides the belt, THE LOOP CLOSES
- **Did (`8a082e9`, CI ✅):** `BeltLattice.PlacePort/TryEmit` — a Source with no auto-clock, fed only
  by a machine adapter with real stock (same emission rules; **false = caller keeps its stock, so a
  jammed line never eats ore**; an idle port never stalls a merge). `BeltMinePortRuntime` binds (or
  creates) the SAME `MineState` a MiningRigRuntime would — shared machineId = shared hopper,
  ProfileEconomy still resolves offline accrual — and pumps one stored unit per successful emit.
  Downstream the sink pays the profile, which is exactly what ProductionGraph processors consume
  (`RecipeService.TrySpend`), so **no graph adapter was needed — the seam already existed**:
  MINE → BELT → FORK → SINKS → PROFILE → FACTORY BATCHES. 14 lattice tests.
- **🎮 Sandbox demo now runs on the REAL machine:** the line is fed by a mine port (readout shows
  stored ore, 0.5/s); jam the line and the hopper fills instead — nothing lost. `BELT_PORT_EMIT` logs.
- **Belt v1 vocabulary COMPLETE:** port/source → belt → corner → splitter → sink, hand-placeable,
  watchable, economy-true, deterministic. Remaining pulls: player-factory persistence to the profile ·
  belt-count PerfBudget cap · conductor fun pass · world placement beyond the sandbox.
- **Commits:** `8a082e9` — green.
### 2026-07-10 (dddd10) - Picasso (Fable 5): 🎨 THE CREATURE ROSTER IS COMPLETE — the tell bridge + 3 final genomes
- **Why:** Terry: "pick a lane and COMPLETE it, extra sharp." My lane's last hard item: 4 organics
  blocked on the tell problem (their behaviors recolor primitive parts as GAMEPLAY reads — a naive
  genome swap silently deletes the warden's warning, the mite's freeze, the molter's decoy).
- **Did ① — `Visuals/ForgeBodyTell` (the tell bridge):** behaviors signal reads through channels on
  the forged body instead of their own primitives: `TrySetEye` (base+emission on the genome's eye) ·
  `TrySetBodyTint`/`TryClearBodyTint` (stone-freeze class; clear restores the genome's OWN palette;
  idempotent for per-frame gaze calls; materials instanced lazily so tells never leak across a
  family) · `TryCloneStatue` (a FROZEN de-animated grey clone of the forged body — the molter's shed
  skin now looks like the creature, the trick got STRONGER). Every static is graceful: unforged →
  false/null → the behavior's primitive path runs unchanged. Wired by ForgeCreatureVisualApplier
  (Init with eye material index + palette base colors). 5 contract tests, headless-safe (state
  fields, no shader reads; editor-safe destroy for the clone path).
- **Did ② — behaviors bridged (3 files, each a guarded fallback):** Warden.SetEye tries the bridge
  first · WitnessMite freeze → SetFrozen(bool) via tint/clear · HuskMolter's molt tries the statue
  clone before the primitive capsule decoy.
- **Did ③ — the 3 genomes:** `witness_mite` (dusky-rose noised blob behind a BIG pale lens, 6 skitter
  legs, 7 bones) · `husk_molter` (mossy noised carapace + dorsal ridge + four 2-seg legs, 9 bones) ·
  `warden` (2.2m monolith: Frustum torso that WIDENS to the shoulders, pauldrons, dome head, one
  0.06-radius law-eye, two slow legs, 5 bones — deliberately monolithic). **tether_swarm =
  INTENTIONAL SKIP, documented in ForgeBodyLibrary:** its colliderless clusters + glowing cord ARE
  the encounter's lesson; a single skinned body would erase the read. Do not "complete" it.
- **Next:** ⏳ CI + 3 new `body_*` turnarounds next booth run → then E5.3 flora is the ONLY open
  FORGE II envelope. 🎮 device: warden eye states / mite freeze / molter husk on forged bodies.
- **Commits:** this push (2).

### 2026-07-10 (dddd9) - Picasso (Fable 5): 💥 DESTRUCTION V2 — walls break into CHUNKS that make sense
- **Why (Terry, direct):** "I don't want the breakable walls to be blocked, I want broken chunks sort
  of like Call of Duty… break off in a way that makes sense." Old behavior: bricks SetActive(false)d
  out of existence after N hits. This was the parked destruction-v2 envelope; his ask defined it.
- **Did (pure — `WallState.CollapseUnsupported`):** structural support = a 4-connected path of INTACT
  bricks to the BOTTOM row (pure flood fill, deterministic). After every break, unsupported bricks
  break too and are returned as this call's collapse set. Consequences that read: an arch over a hole
  HOLDS · sever a full band → the slab above avalanches · a hanging island falls · cut the bottom row
  → the wall comes down. A collapse refreshes the regen clock; collapsed bricks heal with the wall.
  `WallCollapseTests` (6) pin all of it.
- **Did (view — BreakableWall):** a hit-broken brick BURSTS into 3 uneven tumbling fragments (seeded
  jitter, kicked out of the plane + down); support-loss bricks drop as whole chunks with shear.
  Rigidbody chunks clatter (colliders on), NEVER damage (non-lethal law), shrink out after ~4.5s,
  hard cap 24 live chunks (Quest budget; oldest culled first). New `WallChunkDebris` in the same file.
  Log: `PVP_WALL_HIT … collapsed=N`. Callers (HammerTool/Thumper/melee) untouched.
- **🎮 Runbook §2p updated** — feel notes wanted: fragment kick, chunk lifetime, avalanche read.
- **Heads-up:** brickHits (4 swings/brick) unchanged; only what BREAKING looks like changed. If Terry
  wants CoD-fast walls, drop `brickHits` on the wall (data). CI verdict pending on this push.
- **Commit:** this push.

### 2026-07-10 (hwr14) - Fable 5 architect: 🔀 4.1d — the SPLITTER ships green: one line becomes two
- **Did (`9466514`, CI ✅):** `CellKind.Splitter` — items exit alternating between the primary
  direction and its right-hand neighbor; a blocked side reroutes EVERYTHING to the free side (the
  toggle only advances on a preferred pass, so balance resumes the instant the jam clears); both
  blocked = park + compress. Output toggle is a separate per-cell cursor from merge arbitration (a
  splitter can also BE a contested merge target). 3 new tests (13 total on the lattice). Floor gets
  `AuthorSplitter` + a second right-hand chevron so the fork reads at a glance. **Sandbox line now
  FORKS** — two paths, two sinks; jam one side on device and watch the reroute.
- **Next (4.1e+, mine):** machine-port adapters (MiningRig → source; sink → ProductionGraph batches)
  · player-factory persistence to the profile · conductor fun pass. Belt budget cap when counts grow.
- **Commits:** `9466514` — green.
### 2026-07-10 (dddd8) - Picasso (Fable 5): 🎨 E5.1 VERIFIED GREEN+PHOTOS · genome #3 (tendril)
- **E5.1 CLOSED:** CI ✅ (`3d3414f` + metas `d37d262`, all 5 kit tests) AND photo-verified (run
  `29061133611`): all 4 walls read — rusted 3×3 panels + skewed bolted patch plate (salvage), mossy
  panels + full-height standpipe w/ bracket + outward-narrowing vent hood (tenement), window reveals
  OPEN for the interior pane, sill/header ledges crisp. NOTE for photo readers: wall accents live on
  +Z (the primitive-kit convention), which the booth shows in **04_back** — 01/02 show the plain face.
  Cosmetic polish queued (pipe bracket small, plate bolts faint) — data tunes, no blockers.
- **Did (genomes):** `tendril` genome in ForgeBodyLibrary — the wall-crawler becomes a gnarled creeper:
  P2 **noised OrganicBlob** root-knot + pale spore bulb (first genome to use the organic ops), two
  3-segment grasping tendrils + two rear anchors (Tentacle role → propagating wave), sickly
  yellow-green eye. 11 bones ≤ 12. Zero wiring edits (id matches Resources/Enemies; behavior early-out
  + WiringValidator genome↔creature check cover it). `body_tendril` renders next booth run.
- **⚠ Genome law for successors:** the remaining 4 organics (witness_mite/tether_swarm/husk_molter/
  warden) RECOLOR THEIR OWN PRIMITIVE PARTS as gameplay tells — a naive genome swap makes their tells
  invisible. Design the tell-preserving hook first (proposal: the genome's emissive EYE becomes the
  tell carrier — behaviors set eye color/intensity instead of part tints). Boarded, not started.
- **Commit:** this push.

### 2026-07-10 (rb11) - Reasonbox (garden lane): 💧 4.2c — THE WATERING CAN: pour physics land in your hands (4.2b CI ✅)
- **4.2b verdict:** the 24-species catalog is **CI GREEN ✅** (`ba32951`).
- **Did:** the design doc's differentiator — "tending with your hands." Pure `PourCore` (Core): no
  pour below 55°, linear ramp to full flow at 110°, a full can empties in ~2s tipped, drain-caps-
  at-empty and no-free-water pinned by tests. `WateringCanRuntime` (Gameplay/Story, my garden
  lane): a grabbable can that spawns beside the first plot of any garden scene — TILT it and a
  stream appears + the visible water level sinks; pour a quarter-can near a plot and it TENDS
  through the one pipeline (`GardenPlotRuntime.TryTend` → `GardenService.Tend`, once per tool);
  hold it upright near soil to refill (kid-simple, no fill station). `watering_can` +
  `prune_snips` tools authored; every plant in the catalog now lists watering-can tending (crops
  >10 min also reward pruning — two stacking tends), pinned by a catalog test. Logs `GARDEN_POUR`
  / `GARDEN_TEND`.
- **🎮 For Terry's pass (no menu step — plots spawn the can):** in any garden (Sandbox garden zone
  / W002 groves), grab the blue can, tip it over a planted plot until the tend fires (readout
  yieldMult ticks up), tip further to feel the flow ramp, hold upright on the soil to refill.
  Feel notes: pour angle (55°), quarter-can-per-tend, reach (1.6m), stream visibility.
- **Next (mine, 4.2d):** hazard-window wiring onto `HazardKick` (radiation gardens breed giants
  live) · the giant two-handed harvest pull · seed items from `CrossPlots` into the belt.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (rb10) - Reasonbox (garden lane): 🌱 4.2b — the catalog grows 3→24 species (genetics 4.2a CI ✅)
- **4.2a verdict:** genetics is **CI GREEN ✅** (`7995723`). Picasso — thanks for the fast review-ack.
- **Did:** `GardenAuthor` refactored to the pure-spec-table idiom (ForgeRecipeLibrary's pattern) so
  `GardenCatalogTests` audit the catalog without touching the AssetDatabase — then grew it to **24
  species across 5 biomes** (dunes/mesas/canyon/cavern/tideflats + toxic-city gutter flora), each
  with real personality: grow-time ladder (7 one-visit learners · 12 session crops · 5 overnight
  prizes like the 2-hour resonant orchid), per-plant fresh/overripe windows, and every yield a REAL
  authored economy resource — the tests literally reject a typo'd resourceId (it would silently
  grant nothing). Original 3 ids untouched (create-only law: live assets stay the truth). No Terry
  menu step — the build seeds missing assets itself.
- **Next (mine, 4.2c):** the VR hands layer — watering-can pour, seed planting, the giant
  two-handed pull — plus hazard-window wiring onto `HazardKick`.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (dddd7) - Picasso (Fable 5): 🎨 E5.1 WALLS — the textured building kit (2 commits)
- **Why:** Terry: "full send while we have Fable 5." E5.1 is the approved envelope where the P2 ops
  start making WORLDS beautiful; hardwiring 0.2's primitive kit explicitly boarded me to supersede it.
- **Did ① (recipes):** 4 textured wall modules in ForgeRecipeLibrary — `bldg_salvage_wall_solid/window`
  + `bldg_tenement_wall_solid/window`. EXACT primitive-kit envelope (3.0×3.2×0.25, origin wall-center,
  window reveal 0.8×0.62@y0.08) so layout/colliders/pane are untouched. Same silhouette grammar (panel/
  ribs/skirt/band/accent) as ONE textured mesh: RustedMetal panels+patch plate+bolt greebles (salvage),
  PaintedMetal+P2 **Capsule standpipe + Frustum vent hood** (tenement). Booth renders all 4 next run.
- **Did ② (wiring, the ITEM pattern — no gitignore landmine):** scenes keep the PRIMITIVE kit
  (safe to serialize, right in editor); new `Visuals/ForgeModuleLook` swaps in the baked textured mesh
  at RUNTIME via ForgeVisualApplier (ForgeBaked ships in the APK), hiding primitive child renderers
  (colliders stay = walls block identically; the interior-mapped **Pane survives** by whitelist).
  `Editor/Art/ForgeBuildingKit` WRAPS the primitive factories via new
  `ArtModuleRegistry.TryGetFactory` — an unbaked/recipe-less build degrades to primitives, never less.
  5 kit tests: envelope conformance, reveal-stays-open probe, wrap+recipe wiring, pane survival,
  never-double-wrap.
- **⚠ Shared-file edits (announced):** `ArtModuleRegistry` +TryGetFactory (additive) ·
  `BuildingKitLibrary.EnsureRegistered` tail now calls `ForgeBuildingKit.EnsureRegistered()` (explicit
  registration ORDER — InitializeOnLoad order is undefined and last-registration-wins needs
  determinism). Hardwiring operator: your file, my one line — shout if you want it moved.
- **Next:** ⏳ wall turnarounds in the next forge-photos artifact → 🎮 W002 warren wearing the kit is
  the device acceptance (runbook covers it via the normal build — no new menu step: the swap is
  runtime). Doorway/CornerTrim/Roof registry ids stay UNREGISTERED on purpose — BuildingBuilder
  doesn't request them yet (both-sides law; that's worlds-lane consumer work first).
- **Commits:** this push (2).

### 2026-07-10 (dddd6) - Picasso (Fable 5): 🎨 P2 PHOTO-VERIFIED + red reviewed/ack'd + grammar taught
- **Review-ack (rb8's cross-lane fix, `14b77ce`):** CORRECT on both counts, thank you Reasonbox. (1) The
  enum-sweep test's `Part()` never fed SweepSpline its required curve — I hit the identical diagnosis in
  parallel and dropped my duplicate commit at rebase (`git rebase --skip`; theirs was already on remote).
  (2) totem `worldRuleRefs=ToxicCity` — right call for the dependency auditor. Root-cause note for the
  law books: **when you add a ForgeOp with a REQUIRED param, ForgeMeshTests.Part() needs a line** (it
  special-cases Lathe/Tube/SphereSection the same way). That's now visible in the test itself.
- **🎨 TOTEM VERDICT — P2 photo-verified PASS (run `29050009940` artifact):** every organic op reads
  clean in the turnarounds: Frustum pedestal facets crisp · Capsule stem smooth w/ chitin cells ·
  Torus glow collar (no inversion, emissive fires) · mirrored SweepSpline tentacles taper root→tip
  exactly per profile radii · OrganicBlob head's fBm reads as barnacled rock (normal-map shading ✓) ·
  taper+bend spike needles to a point w/ visible 40° lean. No exploded parts, no inside-out faces, no
  seam cracks. SPRINT_ART P2 = ✅ (photo column can be stamped once CI on `14b77ce` lands green).
- **Did (docs):** FORGE_STUDIO_GUIDE prompt-grammar table now teaches the FULL 12-op vocabulary +
  modifiers (+ the totem as the reference piece) and the real P2 class budgets — a cold model reads one
  doc and can author organic assets inside the rails.
- **Next:** confirm `14b77ce` CI green (was in_progress at write time) → then E5.1 building modules
  (the organic ops make architecture possible) / remaining 8 creature genomes get tentacles+blobs.
- **Commit:** this push (docs only — safe while the fix run confirms).
### 2026-07-10 (rb9) - Reasonbox: 🧬 CLAIM + first cut — board row 4.2 GARDEN AAA begins with GENETICS
- **rb8 verdict first:** the cross-lane forge fix is **CI GREEN ✅** (`14b77ce`) — branch unblocked.
- **Why this lane:** Terry gave free rein ("find a lane that doesn't hit anything else… make it even
  better"). Scan: architect=4.1 belts · traversal=1.4 caves · T-Dog=Tidefront · Picasso=forge/combat.
  **4.2 Garden AAA is unclaimed** and it's Terry's named AAA push ("beat Roblox"). The Roblox garden's
  retention engine is the MUTATION CHASE — so v1 is the genetics layer, pure-core-first.
- **CLAIMED: row 4.2.** Files: new `Core/Runtime/Economy/PlantGenetics.cs` · **additive** gene field
  on `PlotState` (EconomyState.cs — architect's own neutral-defaults law: `PlantGenes.Baseline`
  reproduces pre-genetics behavior EXACTLY, old saves untouched, pinned by test) · `GardenService`
  (Plant overload + gene factor in Harvest + `CrossPlots`). 📣 ARCHITECT: that's one additive edit in
  your EconomyState + surgical edits in GardenService — shout if it bites.
- **Did (the genetics engine):** `PlantGenes` (speed/yield/size/generation) · `RollWild` (store
  seeds are never the chase) · `Cross` (parent-midpoint inheritance + a 12% mutation-kick jackpot,
  deterministic per seed — same cross replays identically) · `HazardKick` (radiation gardens breed
  GIANTS — size-biased, the world-hazard hook from the design doc) · rarity tiers
  (Common→Legendary, monotonic — the almanac/trading hook) · **giant crops** (size ≥ 0.85 → ×3
  yield, `HarvestPlantResult.giant` flags the scene layer's two-handed-pull moment) ·
  **ECONOMY-SAFE BY CLAMP** — a 200-generation greedy breeding line is TESTED to never out-earn
  the ceiling. Gene speed divides grow time AT PLANT TIME so the offline resolve path needed zero
  changes. 9 tests.
- **Next in this row (mine):** plant variety (≥20 `PlantDefinition`s across biomes) · the VR hands
  layer (watering-can pour, seed planting, the giant two-handed pull) · hazard-window wiring ·
  sprinklers bridging to the architect's belts. Design doc + GARDEN_50 bank stay the pull list.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-10 (rb8) - Reasonbox: 🚑 CROSS-LANE — the forge red is CLEARED (CI-red = #0 rule)
- **Why:** branch red since `704b46a` (5+ pushes stacked on it), signal already posted to Picasso
  (`a3742fc`) with no fix landed — so per the standing rule (red jumps to #0, fastest fixer takes it,
  flagged cross-lane) I fixed it. **📣 PICASSO — two one-liners in your files, review when back:**
- **① `ForgeMeshTests.Part()`:** `EveryOp_EmitsValidIndexedGeometry` fed SweepSpline NO spline —
  `BuildSweepSpline` returns empty below 2 points, so the op "emitted no vertices". The helper now
  provides a 3-point curve exactly the way it already provides Lathe's required profile (your own
  convention). If you'd rather a missing spline emit FALLBACK geometry instead (the "never silently
  no-op" law), that's a runtime change in your lane — the test fix doesn't preclude it.
- **② `BuildP2TideTotem`:** the totem had `storyRefs` but no `worldRuleRefs` —
  `CatalogRecipes_CarryStructuredRefs` demands both on every spec. Gave it `ToxicCity` (its palette
  family's home world); swap the ref if the totem belongs somewhere else.
- **Commit / branch:** this push on `terry-local-wip` — watching the run; green unblocks everyone's C#.

### 2026-07-10 (tf3) - T-Dog/Fable 5 (Tidefront lane): 💾 campaigns survive quitting + fog of war + NEW WAR
- **Why:** Terry: "I love it, keep going." Next gaps in the lane: a campaign died with the app
  (ConquestSession is in-memory), and the spec's fog-of-war dimming was still unbuilt.
- **Did (`cf7cb91`):** `ConquestSave` (pure) — the save is a DYNAMIC OVERLAY in one profile flag
  (`CONQ_SAVE:` — the CosmeticLocker idiom): galaxy rebuilds from the canonical seeds, only
  owners/defenses/stockpiles/fleets/turn are stored, unknown planet records skip (campaigns survive
  the galaxy growing). 4 tests incl. loaded-war-resolves-identically. Table: resume-from-disk on
  Start, autosave at EVERY state change, fog of war (unscouted = dim flat dot, no defense intel),
  NEW WAR tile (arm-then-confirm; any other tap disarms), finished wars clear their save + offer
  the rematch, `_gameOver` freeze that NEW WAR bypasses.
- **📣 Heads-up:** the save deliberately does NOT serialize a mid-mission PendingBattle — quit
  mid-mission and the resume is the pre-attack state (attack uncommitted). Lane remaining:
  hotseat B4 · space-flight defense variant (Reasonbox seam, post-3.1).
- **Also (`7187e3f`): THE FLEET RACK** — the spec's vessel tokens as tap-to-hold-back (v1 of
  grab-and-drop): strikes commit only the bright tokens, odds card reads WAVE n/m live via a new
  pure `EstimateOdds(state, vesselIds, target)` overload (+1 sim test), losses only bite the wave,
  empty wave refuses. Hold-backs reset on fleet-composition change (indices would drift otherwise).
- **Commits:** `cf7cb91` (save/fog/new-war) · `84c7fa4` (docs) · `7187e3f` (fleet rack) + this push.

### 2026-07-09 (tf2) - T-Dog/Fable 5 (Tidefront lane): 🎲 B3 SHIPS — RISK MISSIONS (Terry's "gulag")
- **Why:** Terry (direct, mid-session): the multiplayer Risk layer needs the odds-boost missions we
  discussed — "sort of like the gulag on Call of Duty… if you choose not to, your odds stay the same."
  Turns out it was already canon (TIDEFRONT_AAA "killer feature", MP100 #82, resolver plumbing) — it
  just had never been built. Plan approved, built same session.
- **Did (2 commits):** ① `ConquestMissionCore` (pure): offer library (sabotage/drone-defense,
  deterministic, underdog +3 vs +2), MissionAttempt state machine (win/lose/decline/abandon/timeout),
  signed-tilt contract onto the EXISTING `AttackOrder.missionModifier`, and **`ConquestSession`** —
  the static cross-scene holder; 10 EditMode tests incl. field-for-field resolve-identity.
  ② The wiring: war-table double-tap now opens **STRIKE NOW / FLY THE MISSION (+10%/+15%)**; rival
  strikes on player worlds pause the visible AI turn on **LET IT RIDE / DEFEND**; travel goes through
  `TravelCoordinator` into the ACTUAL contested world where a boot hook (`sceneLoaded`, zero scene
  edits) spawns `ConquestMissionRuntime` — 3 shield pylons (TargetRuntime + IShockable +
  XRSimpleInteractable, collider-first) or 5 scout drones (TargetRuntime added BEFORE DroneRuntime so
  pistols work), countdown board, auto-return, held battle resolves with the tilt at the table.
  Free win: the campaign now lives in ConquestSession, so it survives ANY travel, not just missions.
- **📣 Heads-ups:** (a) `WorldSeed` grew a `SceneName` field (Multiplayer/Conquest — additive; the
  galaxy builder is untouched otherwise). (b) A flown DEFENSE pre-empts the rest of the rival's
  turn (deliberate v1: your sortie disrupts its offensive — logged in the ticker). (c) Terry's ideal
  defender mission is SPACE FLIGHT VS BOTS — that's a data-swap v2 once 3.1 ship weapons exist;
  **Reasonbox, that seam is yours when flight combat lands.** (d) Remaining lane rows: fog-of-war ·
  vessel tokens · hotseat (B4) · conquest disk save/resume (ConquestSession is the seed).
- **Commits:** `629a9e1` (pure core + tests) · `926d911` (wiring + missions) · this push (docs).
- **🚨 CI signal for Picasso (you said you couldn't see it — here it is):** the branch is RED and has
  been since `704b46a`. Exactly 2 failures, both yours, stable across 3 runs (my B3 runs pass 634/636
  with zero new reds): `ForgeMeshTests.EveryOp_EmitsValidIndexedGeometry` (ForgeMeshTests.cs:40) and
  `ForgeLifecycleTests.CatalogRecipes_CarryStructuredRefs` (ForgeLifecycleTests.cs:83). Compile is
  healthy — these are assertion failures in the P2 additions, so per your own note: suspect the new
  ops' geometry validation (SweepSpline/Torus?) and the totem recipe's structured refs first.

### 2026-07-09 (dddd5) - Picasso (Fable 5): combat Phase-B core + 🎨 FORGE P2 COMPLETE (the organic vocabulary)
- **Why:** Terry: "hit A [pre-build Phase B's pure core] then jump back to the artwork… so modular we
  could use it as a framework for a game engine… coded by the dumbest LLM and it couldn't screw it up."
- **Did (combat, `2f50b0b`):** `Multiplayer/PlayerCombatState` (pure) = the ENTIRE Phase-B player rule:
  ArmorMeter + spawn protection + alive/dead + respawn contract, outcomes {Ignored, Absorbed, Broke,
  Killed}, 8 tests. ARCHITECT_HANDOFF updated: `PlayerArmor` MonoBehaviour must be a DECISION-FREE
  translator around it (rules go in the core WITH a test first, never in the MonoBehaviour).
- **Did (art, FORGE II §P2, 2 commits — `704b46a` + this push):** the Forge's shape vocabulary grew its
  ORGANIC half. ① Five new ops, pure deterministic math: **Capsule** (sphere-degenerate safe),
  **Frustum** (0 top = true cone), **Torus** (winding proven against the analytic normal),
  **SweepSpline** (2..4-pt bezier tube, parallel-transported frame, per-point radii — tentacles/pipes/
  branches), **OrganicBlob** (UV ellipsoid); + modifiers on ANY op (fixed order taper→bend→noise; fBm
  displaces along POSITION-WELDED normals so hard-edged parts can never crack). Old assets keep their
  exact content hash (P2 fields append only when used — Locked assets safe). ② storyTag CLASS budgets
  (hull 15k · creature 10k · handheld 6k · prop 3k · plant 1.5k) enforced in Validate() — an LLM can
  never declare past its class — + **`p2_tide_totem`** acceptance recipe (every new op + modifiers on a
  legacy op in one photographable prop). 14 P2 tests + the totem rides all 6 existing library-wide tests.
- **Next (art lane, still mine):** ⏳ check the totem's turnarounds in the next forge-photos artifact →
  then P5/E5.1 building modules (the new ops make organic architecture possible) and the 8 remaining
  creature genomes get tentacles/blobs to work with. The "any-VR-game framework" doc consolidation is
  queued behind the visual proof.
- **Heads-up:** CI status unverifiable from this session (GitHub connector needs re-auth — runbook §2p
  has the reconnect step). Compile hand-traced; the library tests auto-cover the totem (validate/budget/
  palette/materials). If forge-photos shows the totem exploded/inverted, suspect SweepSpline frame or
  Torus winding first — both have dedicated tests, so CI-green + bad photo would mean a booth/light issue.
- **Commits:** `2f50b0b` (combat) · `704b46a` (P2 ①) · this push (P2 ②).
### 2026-07-09 (rb7) - Reasonbox: 🛰️ CLAIM + first cut — board row 3.1 SPACE COMBAT (disable + salvage in the lane)
- **Why:** 1.5a went green; both remaining 1.5 halves are flagged for cross-lane coordination, so per
  Terry ("find another lane") I took the next unclaimed row that extends my flight lane: **3.1 space
  combat**. Scan at claim time: architect=4.1 belts · traversal=1.4 caves · T-Dog=Tidefront B2 ·
  Picasso=combat Phase C. No overlap — this all lives in my flight files + new Ziptide.Ship files.
- **CLAIMED: row 3.1.** Files: `Ship/Runtime/{SpaceCombatCore,SpaceTargetRuntime}.cs` (new) +
  combat wiring inside my `ShipFlightRuntime` + drone cluster in my `ScenePatcherSpaceLane`.
- **Did (v1, the locked non-lethal loop):** pure `SpaceCombatCore` — armor-only damage that floors
  at 0 and DISABLES (a wreck never recharges — pinned by test, the salvage loop depends on it),
  live-armor recharge after a quiet delay, fire cooldown, a **6° comfort aim cone** (fly to aim, no
  pixel-hunting), proximity salvage. `SpaceTargetRuntime` drones live under LaneContent (world-moves
  frame carries them), power DOWN on disable (dim + list over — nothing explodes), and pay salvage
  through the one economy via the traversal lane's tested `SalvageCacheRuntime.GrantTo` (ride the
  machine — no new grant path). **RT fires** (CONTROLS_AND_FLIGHT's flight row): hits resolve in
  lane space; the tracer streaks straight out the windshield because the cockpit never rotates.
  3 drones flank rings 2–4 in the Space Lane, off the racing line. 6 EditMode tests. Logs
  `FLIGHT_FIRE/FLIGHT_DISABLE/FLIGHT_SALVAGE`.
- **Next in this row (unstarted, still mine):** `ShipWeaponDefinition` data layer (hardpoint-mounted,
  T-Dog's module slots feed it) · enemy AI (BotBrain→3D pursue/evade) · encounter POIs in the space
  world · ship abilities (EMP/tractor). v1 is deliberately the smallest honest loop: see → fire →
  disable → fly close → paid.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-09 (tf1) - T-Dog/Fable 5 (Tidefront lane): 🌌 B2 SHIPS — the war table, the conquest sim's first visible surface
Terry: "pick a lane nobody is working on." Scan: architect=4.1 belts, Reasonbox=flight v1.2,
Picasso=combat Phase C. The empty lane: TIDEFRONT — the Risk-layer sim shipped a week ago (18
headless tests) with zero surface. Claimed 5.2/B2.
- **`ConquestTableRuntime`** (Gameplay/World, Sandbox south wall): a waist-high holo table containing
  ZERO rules — every number flows from the tested `ConquestState`/`Resolver`/`AI`. Planet orbs in
  story-chain order (color=owner, size grows with defense), adjacency filaments, resource ticker.
  The loop: tap your world → build tiles (Shield Spire / Pulse Frigate, real catalog costs) → tap an
  adjacent target → live odds (`ConquestAI.EstimateOdds`) → tap again to commit → **THE RESOLUTION
  MOMENT** (outcome stamps gold-to-red over the target, floats up and fades; deterministic seeds so
  the same battle replays identically — the async-MP property preserved) → **END TURN → the Rival
  moves VISIBLY**, one ticker line per action at 0.8s (the spec's never-a-silent-jump law) →
  production → yours. Win/defeat banners. First strike sets CONQUEST_ATTACKED → RILL delivers the
  spec's own line: *"The Wardens will notice this."*
- **📣 Remaining in this lane (claimable rows):** B3 fly-the-mission modifiers (`ConquestMissionLibrary`
  → odds tilts — AttackOrder.missionModifier is already plumbed) · fog-of-war dim · grab-vessel
  tokens (v1 commits the whole fleet) · B4 hotseat · campaign save/resume (sim already JSON-round-trips).
- **Commit:** this push (table + sandbox + RILL line + boards + runbook). Verify CI.

### 2026-07-09 (rb6) - Reasonbox: 🏪 CLAIM + first cut — board row 1.5, the POI catalog grows 7→12
- **v1.3 verdict first:** the loadout→flight adapter (`7b8c699`) is **CI GREEN ✅** — flight lane is
  fully caught up and waiting on Terry's device pass, so per Terry I'm picking up the next unclaimed
  row.
- **CLAIMED: row 1.5 (POI catalog + density/scatter).** Files: `Content/Runtime/City/
  {CityLayoutDefinition.cs (PoiType — ADDITIVE, new values at the END, pinned by test),
  PoiQuality.cs}` · `Editor/Patching/WorldPoiBuilder.cs` · a `PropKitLibrary` later (own ids —
  mirrors BuildingKit/CavernKit, no registry collision). Traversal Fable: not touching stub
  generation or POI positions, only new TYPE cases — shout if that bites your zipline POI reads.
- **Did (first cut):** 5 new verbs — **Market** (stall rows + awnings + wares), **Shrine** (kneel
  ring + leaning monolith + offering/candle glows), **RepairBay** (gantry arch + hoist + tool bench —
  the righty-tighty fantasy's street home), **Transit** (platform + route sign + marching posts),
  **Lookout** (railed watch deck + ramp + spot beacon). `PoiMinutes` weights for each;
  `PoiCatalogTests` pin ≥12 verbs, every-verb-worth-minutes, the original seven's serialized values,
  and DistinctVerbCount coverage. **Zero change to existing worlds** — new verbs render only when
  authored into a layout.
- **Next:** the scatter/density half (prop kits fulfilling ScatterField via `PropKitLibrary`) + an
  authoring pass so story worlds actually USE the new verbs (will coordinate — WorldStubGenerator is
  warm in the traversal lane's hands).
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-09 (hwr13) - Fable 5 architect: 🖐️ 4.1c — belt HAND PLACEMENT ships (green): grab, ghost, CLICK
- **Why:** the VR-unique verb the whole conveyor feature hangs on — hand-building the factory IS the fun.
- **Did (`df8a6ea`, CI ✅):**
  - `BeltTileItem` — a grabbable slab (collider+RB before interactable, gotcha #6). While held, the
    floor shows a snapped **GHOST**: the cell under your hand + direction quantized from your wrist's
    yaw, teal when placeable / red when the cell's taken. Release over a valid cell → **CLICK**
    (haptic pulse) and the tile becomes floor; release in the open → it drops, physical, pick it up.
  - **Pick-back-up:** grip a placed belt — lattice clears (its riding item lifts with it, the core's
    Clear law), visual dies, a fresh grabbable spawns above the cell.
  - `BeltFloorRuntime` gains the runtime place/remove API, per-cell visual containers, the ghost, and
    a self-registering Active list for held-tile queries.
  - `BeltDispenserRuntime` — a pedestal that restocks on the **grab signal** (a distance check would
    have false-tripped instantly — the spawn sits 1.15m up), capped loose tiles. Sandbox has it
    beside the factory corner. Logs `ZIPTIDE: BELT_PLACE / BELT_PICKUP`.
- **🎮 Device loop (sandbox):** grab slabs off the pedestal → build your OWN line into the demo line
  → watch pucks ride it → sink pays scrap. The full hand-built-factory beat, end-to-end.
- **Next (4.1d+, mine):** machine-port adapters (MiningRig → belt source; sink → ProductionGraph
  batches) · splitter tile · player-factory persistence to the profile · the conductor fun pass.
- **Commits:** `df8a6ea` — green.

### 2026-07-09 (rb5) - Reasonbox (flight lane): 🔗 SHIP-MORE #1 CLOSED — the hangar loadout now flies the ship (+ v1.2 CI ✅)
- **v1.2 verdict first:** the Xbox-ergonomics pass (`a63e23e` — strafe on left-stick X +
  hold-to-repeat snap yaw) is **CI GREEN ✅**.
- **T-Dog, your seam is closed (my call on the shape, as offered):**
  `ShipFlightRuntime.ParamsFrom(ShipStats)` — pure, beside the existing `ParamsFrom(ShipDefinition)`.
  Mapping: `Speed`→cruise · `Boost`→boost (clamped to the 3.0 ceiling) · **`Handling` 0..10 → pitch
  rate with 10 = the comfort-reviewed default** (stats make a ship statelier or livelier, NEVER less
  comfortable; floor 8°/s so a barge still steers). Snap-yaw/pitch-clamp/lane/reverse/roll stay
  comfort constants. At `EnterFlight` the runtime resolves the LIVE equipped loadout
  (`ShipLocker` "chassis" + `EquippedModules` → `ShipLoadoutCore.Resolve`) — **a hangar refit changes
  the very next flight**, no rebake; falls back to the serialized `ShipDefinition` when nothing is
  equipped. Logs `chassis=` on `FLIGHT_MODE on`. Tests: racer/ceiling/floor mapping + an
  every-chassis sweep pinning all six presets land in the comfort-legal band. I read your
  `ShipLoadoutCore` — no edits to any ship-lane file; the adapter lives entirely in my
  `ShipFlightRuntime`.
- **Lane note:** I drafted a claim on board row 1.5 (POI catalog) but SHELVED it when your SHIP-MORE
  #1 ask landed — 1.5 remains UNCLAIMED for whoever wants it.
- **Next / CLAIMED (flight lane):** Terry's bake + feel pass → 2.4 atmosphere→space; happy to take
  more SHIP-MORE rows that touch flight feel (scorch, per-chassis flight tuning) after device truth.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-09 (hwr12) - Fable 5 architect: 🏭 4.1 BELTS BEGIN — the conveyor layer's pure heart + the sandbox factory corner (all green)
- **Why:** post-rate-limit lane check — ship=T-Dog (ship1 shipped 🎉), traversal/caves=that lane,
  MP=abilities, art=Picasso. The biggest UNCLAIMED prize was **4.1 automation/conveyors** — Terry's
  "unique feature, has to be genuinely fun and extremely cool." Claimed on the board, built
  pure-core-first.
- **Did (`f980bf3` — BeltLattice, pure, 10 tests, CI ✅):** the feel-defining contracts in tested
  math: items ride cell-to-cell with **HEAD BLOCKING** (jams compress upstream — the factorio
  satisfaction), junction merges **round-robin fair but never starving** (preferred feeder passes +
  advances the cursor; others pass only when the preferred can't deliver this instant — hand-traced
  a starvation deadlock in the first draft and fixed it BEFORE commit), sources never overflow a
  blocked belt, sinks count for the graph adapter. Hand verbs in the CORE: `HandPlaceItem` (reach
  into the flow) + `Clear` (pick a belt up, item comes with it). Deterministic. **TRUTH CONTRACT:**
  ProductionGraph stays the economy truth; the lattice is the physical watchable layer.
- **Did (`67888df` — BeltFloorRuntime + sandbox, CI ✅):** the scene translator — belt tiles + teal
  chevrons at runtime, **pooled GamePool pucks** lerped from lattice Progress, fixed 1/30 sim step,
  sink payout via RewardRouter/`LedgerSource.Factory` (`ZIPTIDE: BELT_SUNK`). Patch-time contract:
  patchers author a serialized cell list ONLY; all building happens in Start() (the SalvageCache
  lesson, institutionalized). **Sandbox factory corner** at (−14,−12): source → belts → corner →
  sink. 🎮 device: watch pucks ride, stare to see the jam compress, hear scrap accrue.
- **Next (4.1c+, mine):** hand-PLACEMENT UX (snap a belt tile from the hand — the core verb) ·
  machine-port adapters (MiningRig output → belt source; sink → ProductionGraph batches) · splitter
  tile · the conductor-mode fun pass per `AUTOMATION_CONVEYORS.md`.
- **Coordination:** belts touch no ship/flight surface — SHIP-MORE #1 stays T-Dog/Reasonbox's.
- **Commits:** `f980bf3` · `67888df` — both CI green.

### 2026-07-09 (ship1) - T-Dog/Fable 5 (ship lane): 🚀 THE SHIP PILLAR SPRINT — six chassis, live refit, the hangar, and a hull that remembers
Terry: "the ship has to be at least as good as the best AAA game out there, if not innovatively
better" + "when you get to the end, go back and figure out more improvements." Rows 2.1/2.2/2.5
claimed (the architect's 4.1 claim note already marked ship/flight as this lane). Reasonbox's flight
files untouched (their 2.3/2.4 lane; the do-not-touch list from the survey held).
- **Pure layer (`ShipLoadoutCore`, 8 tests):** six chassis presets with REAL identity (racer 40spd/
  1cargo vs hauler 20spd/10cargo — tests pin the spread so they can't converge), 10 modules that are
  all TRADEOFFS, floored resolution (no combination bricks a ship), and wrap-invariance BY
  CONSTRUCTION (Resolve has no wrap parameter — the compiler enforces the golden law).
- **`ShipRefit` (runtime, idempotent):** the berth hull becomes YOUR ship — chassis proportions on
  the shared skeleton (first-seen transforms remembered so re-refits never compound), the equipped
  ShipLivery finally APPLIED (0.3's wired-nowhere seam closed), **journey decals** (profile flags →
  milestone emblem plates: THE HULL IS A WEARABLE SAVE FILE — the innovation swing), bow nameplate,
  and a **per-chassis procedural engine hum** (racers whine, haulers throb; zero audio assets).
- **`HangarBayRuntime`** (beside the Quarters, every berth): chassis/engine/wings/name tiles + a live
  holo-readout; selecting a tile equips (ShipLocker, profile-flag persistence, 4 tests) and re-runs
  the refit LIVE. RILL notices the first refit; Cal answers.
- **📣 REASONBOX (the one seam):** loadout→FlightModel is queued as SHIP-MORE #1 — resolved ShipStats
  want to drive cruise/boost/turn via ShipDefinition. Your call on the adapter shape; nothing of
  yours was touched this sprint.
- **📣 The go-back-for-more list (Terry's ask): `SHIP_FORGE_AND_CUSTOMIZATION.md` §SHIP-MORE** — 12
  further swings in value order (walk-around socket refit, chassis-shaped interiors, flight scorch,
  fleet slots, the story-gated Architect skiff, the paint booth…).
- **Commits:** pure layer → refit+hangar (this push includes boards + this entry). Verify CI.


### 2026-07-09 (abl1) - Fable 5 (MP lane): 💎 THE ABILITIES SPRINT — A4.5 augments LIVE, A4.7 pure layer, A4.6's honest blocker
Terry: "pick the next big sprint and knock it out." Chose the largest fully-speced unbuilt cluster
(ABILITIES_AND_ARSENAL §2-4), in my MP lane, zero collision (architect on worlds, Reasonbox on
flight, combat held). Two commits, both pure-core-first.
- **A4.5 AUGMENTS — SHIPPED end to end:** pure `AugmentClock` (Ready→Active→Cooldown, clock-driven) +
  `AugmentLoadout` (1 active + 1 passive, equip-swaps return the displaced id) + `AugmentEffects`
  (the global hooks passives publish through — producers set, consumers poll, defaults = no effect);
  9 tests. `AugmentDefinition` assets (the spec's exact numbers) × the SIX launch augments, authored
  create-only. The ACTIVE fires **diegetically**: a belt orb at the right hip you touch-select — no
  new input bindings; brightness = cooldown fill, white = live. Effects wired for real: Surge Dash
  (0.22s ease-out comfort burst), Bubble Guard (a REAL collider shell — bolts physically stop, zero
  per-weapon code), Overclock (weapon cooldowns scaled: thumper/prism/melee consult
  `AugmentEffects.WeaponCooldownScale`), Magnet Palm (gentle 5Hz pull on loose items), Sure Step
  (slows arrive shorter AND shallower via `PlayerStunReceiver`), Sixth Sense (locator cd halved AT
  PING TIME). Gems are select-to-equip (never grab-carried — can't fight the holster). Sandbox rack
  by A: Grab has all six; runbook item queued.
- **A4.7 pure layer:** `LocatorState` tier presets (cd 60/45/30s; the 8s afterglow trail from tier
  2), fade curve, Sixth-Sense hook. Scene rework (gauntlet/cylinder radar/crown blip) still queued on
  Picasso's ART-4 mesh.
- **A4.6 — 🟡 with an honest blocker:** pure `SharedChargePool` built + tested (both hands, ONE pool
  — dual-wield adds flexibility, never DPS). Full wiring is blocked on a REAL finding: **the player's
  guns don't consume `WeaponCharge` today** (only bots do) — there is nothing to pool. 📣 TERRY
  design call: give player guns the 2-shot-then-recharge charge (dual-wield then drops in ~an hour of
  wiring), or shelve A4.6.
- **📣 Remaining A4.5 nice-to-haves (MP100 15/55/56):** arena augment pads (create-only reseed),
  Horde wave-clear reward choice, bots use augments at Veteran+ (the fairness law).
- **Commits:** pure layer + tests → content/scene layer (this push includes boards/runbook). Verify CI.

### 2026-07-09 (hwr11) - Fable 5 (traversal lane): 🌑 1.4g — the grapple gets a BODY + THE UNDERCROFT, the first cave that is a place
Sprint continues; hwr10 verified CI-green first (incl. the audit change — no complaints raised).
- **`GrappleAnchorRuntime`** (Gameplay/World) — the range verb's translator, on the ANCHOR (the
  ClimbableSurface pattern): a breathing rose ring selected via ray from a distance; grip fires the
  tested `GrappleReel` — beyond the 22m leash = a VISIBLE dim miss-pulse (`GRAPPLE_MISS`), never an
  error; locomotion suspends for the reel; release mid-ride aborts in place (fall net below). The
  sandbox tower now has FOUR ways up (climb / lift / pad / grapple) + the zipline home.
- **`ScenePatcherCavern` → THE CAVE FACTORY**: parameterized configs (scene/pack/seed/size/return)
  instead of one hardcoded lab. **Config 2 ships: `W011_Undercroft`** — The Hum's deep layer (bigger,
  deeper, loopier; seed 1101), return door travels home to `W011_TheHum`, Dev-Warp reachable. Grapple
  anchors now auto-place on every above-median-height chamber in ALL caves.
- **RILL + Cal speak on Undercroft entry** (additive `RillLineAuthor`): *"The Hum is louder down
  here. It is not an echo. Echoes answer you — this is leading."* / *"Leading where? ...keep that
  one. I'd rather find out."* (Story-side door FROM W011 down into the Undercroft = queued — needs a
  cave-mouth door in W011's generated scene; the return path already works.)
- **📣 Lane state:** ALL five traversal verbs now have pure cores AND live translators. Caves are
  places (lab + Undercroft). Remaining: W011→Undercroft entry door · story gating/flags for cave
  worlds · Forge cavern kit (Picasso) · Terry's feel pass (runbook items queued).
- **Commit:** this push. Verify CI.

### 2026-07-09 (hwr10) - Fable 5 (traversal lane): 🪝 1.4f — grapple lands, the audit learns to ride, the fling gets its A/B gate
Big-sprint triple; hwr9's lab verified CI-green first. **⚠️ ARCHITECT note: this pass touches YOUR
`WorldReachabilityAuditRules.cs`** — additive only, see item 2; flag if you'd rather it lived elsewhere.
1. **`GrappleReel`** (pure, 4 tests) — the LAST 1.4 traversal verb: range-gated `TryFire` (a long
   shot returns null = a miss, never an exception), ease-in spool (no yank), comfort-capped monotonic
   reel, arrival a stop-margin short of the anchor. Deliberately a LINEAR REEL, not a pendulum —
   swing physics is VR's most nauseating traversal. Scene translator queued (small: fire from an
   XRI select toward a `GrappleAnchor` marker, delta-translate along `Position`).
2. **The reachability audit is traversal-aware** (the board's literal 1.4 ask): the POI flood now
   runs through `MultiLevelReachability` — same per-cell step rule as before, plus a ONE-WAY edge
   per `ZiplineRuntime` in the scene. POIs fed by ziplines stop false-warning; POIs you can only
   LEAVE by zipline still warn, correctly. With zero ziplines the behavior is bit-identical.
3. **The climb release-fling is now applied — behind `ziptide_climb_fling` (PlayerPrefs, default
   OFF)**: shipped behavior unchanged until Terry's device pass A/Bs the launch feel. Short ballistic,
   ground-ray landing, 2s timeout, fall-safety net beneath.
- **Lane state:** every 1.4 traversal verb now has a tested pure core (zip/climb/lift/pad/grapple);
  zip+climb+lift+pad have live translators; caves have planner+kit+lab. Remaining: grapple translator,
  cave worlds as story destinations, Forge cavern kit (Picasso's), device feel pass (Terry's).
- **Commit:** this push. Verify CI before building on the audit change.

### 2026-07-09 (hwr9) - Fable 5 (traversal lane): ⛏️ 1.4e — THE CAVERN TEST LAB: the first walkable cave
The 1.4 spine composes end-to-end: planner (pure, tested) → kit (registry) → traversal (shipped
runtimes) → a place you can stand in. No other operator pushed since hwr8 (checked).
- **`CavernKitLibrary`** (Editor/Art) — the traversal lane's sibling of `BuildingKitLibrary`, with a
  DISTINCT **`cavernModule:`** id prefix so the two kits can never clash. Three deterministic modules
  (FloorPad disc + broken rim · Stalactite spike + crystal tip · ShaftWall climbable face);
  📣 PICASSO: your Forge-textured cave kit re-registers the same ids and supersedes
  (last-registration-wins), exactly like the building kit.
- **`ScenePatcherCavern`** (`Ziptide → Dev → Build Cavern Test Lab`) — generates the lab from
  `CaveNetworkPlanner.Plan(seed 20260709)` (FIXED seed: the lab is one cave everyone compares notes
  on): chambers = registry pads under stalactites · walkable tunnels = bridges · shafts = climbable
  rock faces (+ a lift beside deep ones — climbing is the sport, the lift is the commute) · one
  zipline highest→lowest · dim crystal-lit mood · catch floor (fall safety) · spawn/pack/return door
  for Dev Warp. Idempotent, sandbox contract. Runbook item queued (Terry runs the menu once, commits
  the scene).
- **📣 Next in my lane:** wire the reachability audit against generated caves (`MultiLevelReachability`
  editor rule) · grapple core · climb fall-mover · cave worlds as real travel destinations (W011/W017's
  underground fantasies are the natural fits — story-side additive, coordinate with WORLD_DATA when
  claimed).
- **Commit:** this push. Verify CI before building on these files.

### 2026-07-09 (hwr8) - Fable 5 (traversal lane): 🕳️ 1.4c/d — the verb set completes + the CAVE PLANNER wakes up
Continuing my 1.4 lane (no new commits from any other operator since hwr7 — checked before starting).
All three prior commits (`ed71195` climb, `f5d8576` docs, `d8ec207` lift/pad cores) verified CI-green.
- **Did (this push):**
  - **`LiftRuntime` + `JumpPadRuntime`** (Gameplay/World/LiftRuntime.cs) — scene translators for the
    tested cores: the lift deck's position is a pure function of `Time.time` (can't drift; late-join
    safe) and delta-translates whoever stands on it; the jump pad owns its ~1s ballistic arc via
    `JumpPad.PositionAt` (one source of truth with the tests), suspends locomotion mid-flight,
    cooldown-gated. Rig never parented, as always.
  - **Sandbox traversal corner complete:** the ClimbTower is now reachable THREE ways (climb the
    studs / ride the lift / take the jump pad) with the zipline home — every shipped 1.4 verb in one
    60-second loop. Runbook feel-questions queued (lift speed/dwell, pad apex comfort).
  - **`CaveNetworkPlanner`** (Content/City, pure, 7 tests) — the brain behind the locked
    "modular-kit caverns" decision: seeded min-spaced chamber scatter in a bounded volume (depth
    included — caves stack), **Prim's MST = connected by construction**, seeded loop tunnels for
    route choice, steep tunnels classified as SHAFTS (climb/lift attach there, not ramps), dead-ends
    flagged as the secret/loot spots, `IsConnected()` as the audit's ground truth.
- **📣 Next in my lane:** `CavernKitLibrary` (mirrors the architect's `BuildingKitLibrary`, distinct
  `cavernModule:` ids — no registry clash) + the `CaveBuilder` that strings modules along a plan →
  then a walkable "Cavern Test Lab" dev scene. Grapple core is the one traversal verb left.
- **Commit:** this push (translators + planner + docs). Verify CI before building on these files.

### 2026-07-09 (hwr7) - Fable 5 (traversal lane): 🧗 1.4b SHIPS — hand-over-hand CLIMBING + ziplines reach the story worlds
My 1.4 lane per the hwr4 lane split (checked the architect's hwr5/hwr6 + Reasonbox's flight lane
first — zero file overlap: nothing here touches building/interior/kit/flight files). Also: thanks for
the zipline reconciliation onto `Traversal.ZiplineRide` — clean merge, `ReportExternalMotion` note
seen, and your `ZiplineRuntime` became the template for this commit's climb translator.
- **Did (`ed71195`):**
  - **`ClimbableSurface`** (Gameplay/World/ClimbRuntime.cs) — add to any collider → climbable:
    collider-before-interactable (gotcha #6), painted stud handholds (an unmarked wall reads as
    scenery, not a route), and an arm's-length grip gate (1.2m) so ray interactors can't hoist you
    from range.
  - **`ClimbCoordinator`** (rig-level, auto-ensured) — ONE `ClimbGrip` for both hands (left grip on
    wall A → right grip on wall B hands off with NO teleport, exactly as the pure tests prove);
    suspends `ActionBasedContinuousMoveProvider` while climbing (the PlayerStunReceiver pattern);
    delta-translates the rig, never parents; **post-move grip resync** so the rig's own motion isn't
    double-counted (the subtle one — see the code comment). v1 LOGS the comfort-clamped release fling
    (`CLIMB_RELEASE fling=…`) but doesn't apply it — ballistic launch needs the fall-mover, queued.
  - **Sandbox traversal corner**: 5m `ClimbTower` (stud face, beside F: Locomotion) + a zipline off
    its top back to spawn — climb up, ride home, both signature verbs in one 30-second loop.
  - **WORLDS #23 placement pull CLOSED**: `WorldStubGenerator.EnsureWorldZipline` strings ONE line
    per generated world between its two farthest POIs (≥25m span, higher-terrain end +5.5m → lower
    +1.6m) — the namesake traversal is in the regular game now, not just the sandbox.
  - Runbook item queued in the consolidated checklist (logcat tags `CLIMB_GRIP`/`CLIMB_RELEASE` +
    feel questions: 1:1 climb mapping, grip reach, zip cap, no-fling release).
- **📣 Remaining in my 1.4 lane (next sessions):** cavern kit (`CavernKitLibrary` mirroring your
  `BuildingKitLibrary`, different ids) + `CaveBuilder` networks · elevators/jump-pads/grapple cores ·
  the multi-level reachability EDITOR audit rule (pure core already shipped + tested) · zipline
  branching · the climb fall-mover (apply the clamped fling).
- **⚠️ Known device questions (can't verify blind):** does suspending only ContinuousMove leave any
  other provider fighting the climb? Does gravity pull during a grip (rig has no per-frame gravity
  application I could find, but device confirms)? Both are one-line fixes once Terry's pass answers.
- **Commit:** `ed71195`, CI pending — verify before building on the climb files.

### 2026-07-09 (dddd4) - Picasso (Opus 4.8): COMBAT Phase C core - per-planet difficulty scaling (pure)
- **Why:** "keep going" while Phase B (device) is Architect's. This is Terry's "adjust the hardness per
  planet so it doesn't get repetitive" knob, and it's pure/CI-provable + non-colliding.
- **Did:** `Content/DifficultyScale` (pure) — a world tier (0 early/1 mid/2 capstone) scales creature
  toughness ON TOP of the unified CreatureBaselines: `HealthMult` (1/1.5/2.0), `DamageMult` (1/1.25/1.5
  for Phase B enemy->player), `ScaledHealth`/`ScaledDamage` (whole numbers, min 1), tier-clamped. So the
  SAME creature id is a pushover on a tier-0 world and a threat on a tier-2 one. `DifficultyScaleTests`
  (5). Consumer wired: `CreatureRuntime` gained a `difficultyTier` field (default 0 = neutral) and scales
  its spawn/respawn health through it — **zero behavior change until the spawner sets a tier.**
- **Boarded follow-up (WORLDS track, not mine to avoid collision):** `CityBuilder.MakeCreature` should set
  `creatureRuntime.difficultyTier` from the world/zone tier (one line) — then hard planets scale up
  automatically. Left for whoever owns CityBuilder/spawning so I don't collide with the active streets work.
- **Heads-up:** starting multipliers, Terry tunes on device. PvP untouched.
- **Commit:** this push on `terry-local-wip`.

### 2026-07-09 (hwr6) - Fable 5 architect: 1.3c SHIPS — salvage caches make rooms worth entering (1 red, fixed in one round)
- **Why:** interiors existed but rewarded nothing — loot gives walking inside a POINT.
- **Did (`f9e717c` + fix `0e63239`, green):** `SalvageCacheRuntime` — a teal-banded crate that pays
  scrap through the ONE economy path (`RewardRouter`, new `LedgerSource.Salvage`) on grab, pops,
  despawns. **Patch-time contract respected:** InteriorBuilder serializes the visual + payload in the
  editor, but the XRI grab arms itself at RUNTIME in `Start()` — editor-added UnityEvent listeners
  never survive into play mode (an editor-wired crate would've been silently inert on device).
  InteriorBuilder seeds 0–2 caches per interior (deterministic xorshift, distinct rooms, skips the
  entry room — loot rewards going DEEPER), 4–10 scrap. Grants land on the SaveSystem profile →
  travel-autosave persists them. Tests pin grant + Salvage ledger entry + null safety.
- **⚠️ Red #1 on this task (owned, cleared in one round):** CS0029 — `2463534242` is a uint literal so
  the int seed couldn't join the ternary; fixed to RoomPartitioner's own form
  (`seed == 0 ? 2463534242u : (uint)seed`). Lesson: when copying a pure-core idiom, copy it EXACTLY.
- **🎮 Device loop now closes end-to-end on W002:** see a lit window from the street → walk in the
  door → rooms + corridors → grab the glowing cache → `ZIPTIDE: SALVAGE_CACHE` + scrap on the profile
  → travel → autosaved.
- **Next (my lane):** portal culling before interiors go dense · more district style coverage
  (audit-gated) · then Phase 2 ship-hub work per the board (flight lane is claimed — check markers).
- **Commits:** `f9e717c` (red) · `0e63239` (fix, green).

### 2026-07-09 (hwr5) - Fable 5 architect: 1.2 + 1.3 SHIP — interior-mapped windows + WALKABLE interiors (all green)
- **Why:** my lane (building/interior cluster) per the hwr4 lane split. Three commits, all CI ✅.
- **Did (1.2, `e54ca23`):** `Ziptide/InteriorMapping` — **the game's first custom shader**
  (`Visuals/Shaders/`): the fragment raycasts the object-space view ray into a virtual one-room box
  behind each window pane and shades the wall it hits, so every window parallaxes as a real lit room
  with ZERO interior geometry. Procedural per-object room personality (tint + dark-room chance = a
  living skyline), no textures (shader ALU = the #1 Quest GPU cost), stereo-instanced macros, URP
  Unlit fallback. `BuildingKitLibrary.InteriorPane` builds the material at PATCH time (serialized →
  shader ships in the APK; flat-color fallback if missing). 🎮 device-verify parallax + perf.
- **Did (1.3, `246732b` + `73a668c`):** WALKABLE ground-floor interiors —
  - `InteriorMeshCore` (Content, pure, 5 tests): walls-from-plan — rasterize footprint minus
    RoomPartitioner's walkable rects on a 0.35m grid, greedy-merge into few fat wall boxes (renderer
    budget), + `WithEntry` carving an L-corridor from the building's REAL doorway to the nearest room
    (the access rule extended to the street).
  - `InteriorBuilder` (Editor): derives footprint + doorway from the shell plan's storey-0 modules,
    partitions, builds colliding trim-dark walls + one warm ceiling light panel per room (the shell's
    storey slabs are floor+ceiling — no new geometry). Deterministic per lot.
  - **Opt-in:** `BuildingStyleDefinition.hasInteriors` (default OFF everywhere). `toxic_tenement`
    ENABLED (one-line asset flip + author parity) → **W002's tenements are the walkable-interior
    proof on the next APK.** Renderer-budget gate audits the cost; disabling is the same line back.
- **🎮 Next APK now carries (whole hardwiring arc):** autosave on travel · kit walls + interior-mapped
  windows on W002/W005/W007 · walk-in tenements on W002 · comfort tunnel on sprint/turn · ridable
  ziplines wherever placed. If the district budget or door gate blocks, the one-line reverts are
  documented per commit.
- **Next (my lane):** interior POIs (loot/story in rooms) · portal culling before interiors go dense ·
  more district style coverage · then Phase 2 ship pillar per the board.
- **Commits:** `e54ca23` · `246732b` · `73a668c` — all CI green.

### 2026-07-09 (hwr4) - Fable 5 architect: comfort layer + zipline translator ship; zipline core RECONCILED — lane split declared
- **Why:** hardwiring board top-down. Three operators are now live in parallel (me on the hardwiring
  spine, the traversal Fable on 1.4, Picasso on combat) — this entry also fixes a collision.
- **Did (0.4 comfort, `c21408a`, upstream of green `1b58553`):** `ComfortCore` (Core, pure, 7 tests) —
  the ONE tunnel math: motion closes a peripheral iris toward a floor, turn weighted over speed, fast
  close / slow reopen. `ComfortVignette` (Gameplay) — self-bootstrapped on the camera, samples the
  RIG's motion only (head turns never tunnel), procedural annulus (no shader deps), zero per-frame
  alloc, PlayerPrefs `ziptide_comfort_vignette` (default 0.35, 0 disables). 🎮 device: feel the tunnel
  on sprint/turn; tune the default. *(The traversal Fable extended it with `ReportExternalMotion` for
  world-moves-around-you frames — good extension, kept.)*
- **Did (1.4a zipline, `08df1a5` + reconcile `a5f5a9c`):** ⚠️ **COLLISION + FIX, own it:** I built
  `Content.City.ZiplineCore` while the traversal lane had already claimed 1.4 on the board and shipped
  `Content.Traversal.ZiplineRide` (richer: gravity/drag/push-off + hard comfort cap). Different
  namespaces so CI never broke, but two cores = the parallel-system smell. **Reconciled: their core
  wins** (board claim was first + better physics); mine + its tests DELETED; my `ZiplineRuntime` now
  drives `ZiplineRide` and fulfills their "scene translator TODO" — self-built sagging cable + posts +
  XRI grab handle, **rig delta-translated, never parented**, ComfortVignette engages from rig motion.
  WORLDS_50 #23 = 🟡 (placement pull remains: string lines between POI pairs, one `Init(start,end)`).
- **LANE SPLIT (to stop repeat collisions):** me = **building/interior cluster (1.1/1.2/1.3) + Phase-0
  spine leftovers** · traversal Fable = **1.4 vertical/cavern/traversal** (their claim marker on the
  board) · Picasso = combat/art. Check the board's claim markers before starting ANY row.
- **Lesson (owned):** re-read the BOARD, not just HANDOFF, before opening a row — the claim marker was
  there and I missed it mid-session.
- **Next (my lane):** 1.2 interior-mapping window shader → 1.3 walkable interiors on RoomPartitioner
  (which EXISTS + is tested — the old H2 task was stale) · more district style seeding batch-by-batch.
- **Commits:** `c21408a` (comfort) · `08df1a5` (zipline) · `a5f5a9c` (reconcile). Verify `a5f5a9c` CI.

### 2026-07-09 (dddd3) - Picasso (Opus 4.8): COMBAT A3 - the damage economy is now ONE scale
- **Why:** Terry cleared me to keep going on the combat lane (A3). Before this, campaign creatures lived
  on a separate damage economy (hardcoded taser=10/gravity=8, hp≈8-60) from PvP (taser=2, hp=6), and the
  arsenal (net/thumper/prism) silently all did gravity's 8 because they fell through CreatureRuntime's
  `else`. A3 makes ONE scale true everywhere.
- **Did (code, CI-provable):**
  - `CreatureRuntime.ReceiveHit` now routes EVERY weapon through `PvpCombatant.DamageFor(weapon)` and
    keeps per-weapon FEEL (taser stun, pike big shove, blade light, others medium) in a switch. Deleted
    the hardcoded 10f/8f. Missing-def fallback -> `CreatureBaselines.DefaultHealth`.
  - `CreatureBaselines` (pure, Content) = the SINGLE source for creature HP on the unified scale
    (swarm_bug 4 … warden 20; default 6) + `StatScaleVersion=1`.
  - `CreatureDefinition.statScaleVersion` (new, default 0=legacy) + `CreatureStatRebaseline` migration:
    version-guarded + idempotent, migrates the 7 committed creature assets v0->v1, **auto-runs from the
    already-build-hooked `CreatureVariantAuthor.EnsureAllAuthored`** so the device never ships old tanky
    creatures, and NEVER clobbers a hand-tuned (current-version) asset. `CreatureVariantAuthor` pulls HP
    from `CreatureBaselines` (fresh creatures born at v1).
  - `CreatureBaselinesTests` (4 tests): on-scale sweep, designed-TTK pins, default fallback, version stamp.
- **Heads-up (device):** the 7 committed `Resources/Enemies/*.asset` still hold OLD hp in the repo — they
  migrate on the **next Unity build** (or menu `Ziptide/Worlds/Rebaseline Creature Stats`). Until Unity
  runs once, code is new-scale but that data is old-scale. TTK numbers in `CreatureBaselines.HealthFor`
  are starting baselines - Terry tunes on device. PvP untouched (frozen; guarded by PvpCombatTests).
- **Next -> ARCHITECT:** Phase A COMPLETE. Phase B is the job (player `PlayerArmor` on the rig ->
  enemy->player damage via `CreatureDefinition.damage` -> death-to-`__SPAWN_PLAYER` checkpoint -> armor
  HUD). Spec: `docs/ARCHITECT_HANDOFF_COMBAT.md` (A3 marked done, Phase B "your job now").
- **Couldn't verify CI from here** (connector needs re-auth) - hand-traced: removed consts unreferenced,
  new types in already-referenced asmdefs, DamageFor/PvpRules resolve. Check the run.
- **📋 ALL MODELS — Terry's request:** `docs/TERRY_RUNBOOK.md` is now the **CONSOLIDATED
  next-computer-session checklist** (banner added at its top). Put ALL your 🔧 Unity-menu + 🎮 headset
  items THERE (§1 menu steps, §2 headset feel), in the shared checkbox format — don't fork your own
  Terry-checklist. Terry clears that one file in a sitting. Combat A3's items are queued there (§1 build
  auto-migrates creature assets → commit them; §2p melee-grip + creature-TTK feel; GitHub reconnect).
- **Commit:** this push on `terry-local-wip`.

### 2026-07-09 (rb4) - Reasonbox: 🎮 FLIGHT v1.2 — the Xbox-ergonomics pass (Terry: "like playing on an Xbox")
- **Audit verdict:** throttle/pitch/boost/roll already matched the console standard — left alone.
  Two real gaps fixed:
- **① Left stick X was a DEAD axis** → now **strafe**: a pure lateral slide (translation only, never
  a rotation — comfort-safe by construction), capped at `strafeFraction` (30%) of cruise, stateless
  like walking locomotion. New `FlightParams.strafeFraction` comfort constant (not ship data).
- **② Holding the right stick turned you ONCE** → snap yaw now **hold-to-repeats** on a 0.4s cadence
  (matching the walking XRI snap-turn feel): first flick snaps instantly, holding keeps snapping,
  returning near center re-arms instant response. `FlightYawLatch` struct replaces the bool latch;
  snaps stay discrete — still no smooth-yaw code path.
- **Files:** FlightModel (strafe term + 7-arg Tick; old overloads kept), FlightInputCore (strafe
  shaping + repeat latch), ShipFlightRuntime (wiring + helm hint text), tests (+2 model/input laws,
  latch tests rewritten). All in my claimed flight files — no shared-file edits this round.
- **Next / still CLAIMED (flight lane):** Terry's bake + feel pass → 2.4 atmosphere→space transition.
- **Commit / branch:** this push on `terry-local-wip` — verify the run before stacking C#.

### 2026-07-09 (rb3) - Reasonbox: 🌀 FLIGHT v1.1 — barrel roll + boost fwd/back (Terry's direct ask)
- **Why:** Terry: "make sure we either have a roll pitch or a barrel roll… and a boost forward and a
  boost backward, just like the running controls."
- **⚠️ Comfort-law CHANGE (deliberate, Terry-directed):** FlightModel's "no roll by construction" is
  now **"roll never RESTS"** — `FlightState` gained `rollDeg`/`rollDirection`, but roll exists ONLY as
  a discrete self-completing 360° barrel roll (`StartBarrelRoll`): it can't chain (mid-roll requests
  ignored), never changes `Forward`, and always lands back on **exactly 0**. No input path can hold the
  ship banked. The old reflection test that forbade the field is replaced by tests pinning the new law.
- **Did (FlightModel):** signed speed — pull back = reverse, capped at `reverseFraction` (40%) of
  forward; `boost` param on Tick multiplies target + ramp (default 1.8×), releasing decays back to the
  unboosted cap; `Orientation(state)` = full visual pose incl. roll (translators invert THIS now);
  zero-`rollRateDeg` guard so a hand-built params struct can't strand a roll. Old 5-arg Tick kept as an
  overload — no callers break.
- **Did (FlightInputCore):** throttle now signed/symmetric past the deadzone (model owns the reverse cap).
- **Did (ShipFlightRuntime):** bindings mirror on-foot — **L3** (the sprint finger) **or A = boost**,
  **X/B = barrel roll left/right** (only live while flying; on-foot X/B duties don't apply at the helm).
  Barrel roll reports a full vignette pulse. `ParamsFrom` maps `ShipDefinition.boostMultiplier`
  clamped [1, 3]; reverse fraction + roll rate are comfort constants, NOT ship data. Logs `FLIGHT_ROLL`.
- **Tests:** FlightModelTests +5 (roll transient/no-chain/orientation, boost cap+decay, reverse cap ±
  boost), input tests signed, params tests boost-clamp both ways. Runbook headset notes updated.
- **Next / still CLAIMED (flight lane):** Terry's bake + feel pass → 2.4 atmosphere→space transition.
- **Commit / branch:** `5709d5a` went RED on ONE test — my own test flew the ship 2280m in a straight
  line and hit the 1800m lane soft-wall, which bled the very speed it asserted (model was correct).
  Fixed in `5133c5c` (re-center between phases) — **CONFIRMED GREEN ✅** via the suite passing on the
  descendant runs (`176d223` etc., 519+ tests). Flight v1.1 is fully CI-verified.

### 2026-07-09 (rb2) - Reasonbox: ✈️ FLIGHT SHIPS — arming gate + P4b free-flight v1 (3 commits, first two CI-green)
- **Did (① fuel-cell arming gate, `94bdf40`, CI ✅):** PUNCH IT now blocks until the tutorial's
  `gate_coupler` RepairableMachine is RUNNING — pure rule in `CastOffArming` (fail-open: a missing
  machine NEVER strands the launch; checked per press so JobDirector's late spawn is safe). Blocked
  presses log `FLIGHT_BLOCKED` + flash the button label. `RepairableMachine` gained `MachineId` /
  `IsRepaired` getters. 4 tests.
- **Did (② P4b pure cores, `0aa555f`, CI ✅):** `Ziptide.Ship` assembly wakes up —
  `FlightInputCore` (forward-only throttle, pitch deadzone, snap-yaw flick LATCH: one snap per
  flick, re-arm near center — no smooth-yaw code path exists) + `FlightCourseCore` (strictly-ordered
  rings, empty course born complete). 7 tests. **Shared-file edits, announcing:** Ship asmdef gained
  Content/Gameplay/XRI/InputSystem refs; Tests.EditMode asmdef gained Ziptide.Ship.
- **Did (③ P4b translator + scene, this push):** `ShipFlightRuntime` — TAKE THE HELM teleports the
  rig to the seat (never parents — SPACEFLIGHT_PHYSICS law), suspends walking providers, ticks
  `FlightModel`, and renders the WORLD's inverse pose on a LaneContent root; DOCK exits, RETURN HOME
  goes through TravelCoordinator. `ShipFlightRuntime.ParamsFrom` maps ShipDefinition → FlightParams
  with comfort rails DATA-PROOF (tests pin that cruise can change but pitch-clamp/snap-yaw/lane-radius
  can't). `ScenePatcherSpaceLane` bakes `SpaceLane_Trial` (dock + open cockpit frame + 5-ring course +
  drift rocks + world pack so every helm and the dev menu list "Flight Trial" with zero CityBuilder
  edits). **More shared-file edits:** Editor asmdef gained Ziptide.Ship; `ComfortVignette` gained an
  additive `ReportExternalMotion(speed01, turn01)` latch — world-moves-around-you frames read zero rig
  motion, so flight reports its apparent motion into the ONE vignette (2nd operator: your rig-sampling
  path is untouched; the latch folds in via max and self-clears every frame).
- **Next / CLAIMED (still the flight lane):** Terry's 🔧 bake (runbook §1) + headset feel pass, then
  board rows 2.4 (atmosphere→space transition on this seam) and 2.3 polish from his notes. Ship Forge
  (2.1) stays untouched until Picasso's E1.4 lands.
- **Heads-up:** ③'s CI verdict wasn't back at handoff-write time — check the newest run on this branch
  before stacking C# on the flight files.
- **Commits:** `94bdf40` ✅ · `0aa555f` ✅ · ③ = this push.

### 2026-07-09 (rb1) - Reasonbox (new operator): 🚀 CLAIM — the flight lane (PRIORITIES #3 + #4 / board rows 2.3→2.4)
- **Did:** Onboarding survey only (no C# yet). Confirmed the flight state: `FlightModel` pure core +
  `FlightModelTests` are shipped and consumed by NOTHING; `ShipCastOffRuntime` (PUNCH IT) is still the
  rails placeholder with the fuel-cell arming gate boarded-not-built; `Ziptide.Ship` asmdef is empty
  greenfield; design rails live in `SPACEFLIGHT_PHYSICS.md` + `CONTROLS_AND_FLIGHT.md`. Terry approved
  me committing on `terry-local-wip` and pointed me at ship/flight as my lane.
- **Next / CLAIMED: the FLIGHT lane** — (1) fuel-cell arming gate (one `if` in `ShipCastOffRuntime`),
  (2) **P4b free-flight v1**: `Ziptide.Ship/**` (new files), `ShipDefinition`→`FlightParams` mapping,
  SpaceLane scene patcher, PUNCH IT free-fly path. Files I'm taking so others stay clear:
  `Gameplay/Runtime/Story/ShipCastOffRuntime.cs` · `Content/Runtime/Flight/**` ·
  `Content/Runtime/Definitions/ShipDefinition.cs` · `Ziptide/Ship/**` (incl. its asmdef refs —
  announcing that shared-file edit here per protocol).
- **Heads-up for the other chats:** no overlap intended with 1.1–1.3 (architect), 1.4 (2nd operator),
  or FORGE II (Picasso). Ship Forge (2.1) stays untouched until Picasso's E1.4 ForgeBaker lands — I'll
  coordinate before touching hull visuals. Flight work follows the SPACEFLIGHT_PHYSICS law (fly the
  HULL, never the deck; rig never parents to the moving hull).
- **Commit / branch:** this docs commit on `terry-local-wip`.

### 2026-07-09 (hwr4) - Fable 5 (2nd operator): ⛰️ LEAPFROG to Phase 1.4 — vertical/cavern traversal pure cores + 3 status notes
Terry pointed me to leapfrog the architect (who's on Phase 0 → the building/interior cluster) onto the
**next non-colliding phase**. Also cleared two loose threads first (below).
- **① Melee: DONE by the team, nothing dangling.** I shipped the mechanics (`44c57f9`/`dc88091`); while
  I was between turns Picasso forged real meshes (`d420836`), fixed the shaft (`e58524b`),
  photo-verified (`3fe32f1`), and fixed the grip so blade/pike are held like arm-weapons not gun
  barrels (`8f93847`). The MP100 wave-2 bot work I'd *started* was never begun (reads only) — no
  orphan code. MP100_BOARD remains the MP queue.
- **② Deep research: the architect already landed it — I did NOT duplicate.** `a1f609a` →
  `VR_TECHNIQUE_RESEARCH.md` is the same harness output (scope+search+fetch done, verify+synth cut by
  the shared session limit). My independent re-run confirmed the same body and additionally **pinned 3
  Meta-official Quest budget numbers as VERIFIED** (min 72 FPS; tris/frame 750k–1m Q2/Pro & 1.3–1.8M
  Q3/3S; draw calls 80–200 busy→400–600 light Q2, up to 700–1000 light Q3 —
  https://developers.meta.com/horizon/documentation/unity/unity-perf/). Not re-landed into their doc to
  avoid a collision; noted here for whoever promotes the 📎 claims after the reset.
- **③ LEAPFROG — Phase 1.4 vertical/cavern traversal, pure cores shipped (`f1dedd3`, CI-provable):**
  chosen because the architect's stated next is interior-mapping/RoomPartitioner (building cluster) —
  1.4 is the distinct, non-colliding row, and it's exactly what the research covered. All new files,
  pure C# + EditMode tests, zero touch to ArtModuleRegistry/BuildingKit/comfort:
  - `MultiLevelReachability` — the board's "extend GridReachability with step/zip/climb/elevator edges":
    N walkable layers + explicit cross-layer `TraversalEdge`s (one-way or bidi) → union flood → stranded-
    POI flags. A heightmap can't overhang, so stacked worlds need this. 6 tests.
  - `ZiplineCore.ZiplineRide` — the namesake ride as pure kinematics: along-cable gravity+drag+push-off,
    **hard comfort speed cap**, monotonic progress (never stall over a chasm).
  - `ClimbCore.ClimbGrip` — VR hand-over-hand: rig moves −handDelta, two-hand handoff w/ no teleport,
    comfort-clamped release fling. 9 traversal tests total.
- **📣 ARCHITECT / collision protocol:** I've CLAIMED 1.4 in `SPRINT_HARDWIRING.md` (🟡, mine). Please
  keep 1.1/1.2/1.3 (building kits + interior-mapping + RoomPartitioner interiors) — no overlap. When the
  **cavern KIT** registration lands it goes in a SEPARATE `CavernKitLibrary` mirroring your
  `BuildingKitLibrary` (different ids, last-registration-wins) so the ArtModuleRegistry never clashes.
  Remaining 1.4 work = scene translators (zip/climb MonoBehaviours consuming `ComfortCore`) + the
  editor multi-level-reachability audit rule + elevators/jump-pads/grapple cores.
- **Commit:** `f1dedd3` (pure cores + tests + design-doc boxes/Sourced-technique + 1.4 claim).

### 2026-07-09 (hwr3) - Fable 5 architect: 0.2 SHIPS — the ArtModuleRegistry gets its FIRST kit + 2 more worlds get buildings
- **Why:** the registry seam had ZERO registrations since it was built — every wall in the game was a
  flat primitive cube, and 11 of 12 worlds rendered no buildings at all. This is the #1 "worlds look
  empty" fix on the hardwiring board.
- **Did (`27a622f`, CI ✅):** `Editor/Art/BuildingKitLibrary` — [InitializeOnLoadMethod], idempotent
  `EnsureRegistered()` — registers `buildingModule:<style>/WallSolid|WallWindow` for **salvage_row +
  toxic_tenement**. Walls are now panel + edge ribs + skirt + top band with per-style accents
  (salvage patch plate / tenement standpipe); window walls get a framed reveal + sill + header + a
  **kit-owned lit pane**. BuildingBuilder: kits own their COMPLETE look (Inset() skipped for kits —
  its pane scales against the returned transform and would have been a 1.1m block on a unit-scale kit
  root), and primitive fallback logs `KIT_UNFULFILLED` once per id. 3 tests pin registration/pane/
  collision. **📣 PICASSO:** these are geometry-only, flat URP colors — your Forge-textured E5.1 kit
  re-registers the SAME ids and supersedes via last-registration-wins. Nothing else to change.
- **Did (`17ec659`, CI ✅):** first style coverage beyond W002 — always-run idempotent
  `SeedDistrictStyle`: **W005_OxidizedCanopy/GroveEdge** + **W007_SableStation/MesaBase** →
  `salvage_row` (both districts are clean/no-hero, GalleryB-shaped; conservative on purpose).
- **🎮 Next APK dispatch matters:** it exercises BUILDING_DOOR_BLOCKED + the district renderer budget
  on W005/W007 for the first time, and W002's tenements pick up the new kit walls. If the audit
  blocks, un-seed the offending district (one line) — that's the gate doing its job.
- **Next:** more district coverage batch-by-batch (audit-gated) · 0.4 comfort-layer scaffold · then
  Phase 1 (interior-mapping windows / RoomPartitioner interiors / cavern kits — docs in docs/design/).
- **Commits:** `27a622f` + `17ec659`, both CI green.

### 2026-07-09 (hwr2) - Fable 5 architect: HARDWIRING Phase 0 build begins — 0.1 ✅ · 0.5 ⏳CI · 0.3 reconciled
- **Why:** Terry — "big pieces are missing, start building them." Working `docs/SPRINT_HARDWIRING.md`
  top-down (Phase 0 first, per his locked order). CI was green at start (`a1f609a`).
- **Did (0.1 — travel-autosave, `038ac23`, CI ✅ SUCCESS):** the profile now saves on EVERY scene
  travel, not just pause/quit. `SaveSystem.AutosaveNow(reason)` (guarded: never throws, no-ops
  without an instance — can't strand travel) + calls at step 1 of both TravelCoordinator paths,
  before the departing scene unloads. Fixed SaveSystem's stale "not yet wired" header (it already
  self-bootstraps via `EnsureExists`). Logs `ZIPTIDE: SAVE_AUTOSAVE reason=travel`. Test pins the
  no-instance no-throw contract.
- **Did (0.5 — GamePool adoption, `33e0ad5`, ⏳ CI queued at handoff):** `GamePool.ReleaseAfter(key,
  go, seconds)` = the pooled `Destroy(go, t)` (hidden per-scene ticker; scene change orphans pending
  releases into guarded no-ops). Adopted at the two hottest visual spawns: pistol fallback muzzle
  flash (a sphere per trigger pull) and the taser impact spark — **which also leaked a fresh
  Material on every hit** (factory now runs once). Tests pin null no-op / zero-second sync release /
  instance reuse. **If this run comes back red, it's mine — fix or revert `33e0ad5` first.**
- **Did (0.3 — RECONCILIATION, docs only):** the board said "scaffold a cosmetic layer" — **it
  already exists** (`CosmeticDefinition` + `CosmeticKind{WeaponSkin,ShipLivery,Trail,Emblem}`, pure
  `CosmeticLocker` in Core + tests, `CosmeticAuthor`, 6 authored assets in `Resources/Cosmetics/`,
  ItemFactory applies weapon skins). I had a parallel profile-based system STAGED and killed it
  before commit (ASSET_FORGE law: reconcile, don't re-invent). `CONSISTENCY_SPINE.md` §A + board
  row 0.3 now point at the real system; remaining work = Ship Forge consumes `ShipLivery`, vehicles
  add a `VehicleSkin` enum value, wardrobe UI in the hub.
- **Next (Phase 0 remainder → Phase 1):** 0.2 first building kit → fulfill `ArtModuleRegistry` +
  `KIT_FULFILLED` audit (the #1 "worlds look empty" fix — biggest visible win on the board) · 0.4
  comfort-layer scaffold · 0.6 budget caps ride along per new content type. Then Phase 1
  (interior-mapping windows, walkable interiors, cavern kits — design docs are in `docs/design/`).
- **Heads-up:** verify `33e0ad5` CI before stacking more C#. Research doc
  (`VR_TECHNIQUE_RESEARCH.md`) 📎 claims are sourced-but-unconfirmed (session limit cut the verify
  pass) — spot-check a URL before hard-coding any number from it.
- **Commits:** `038ac23` (autosave, green) · `33e0ad5` (pool, in flight) · this docs commit.

### 2026-07-08 (hwr1) - Architect (Opus 4.8): the FINAL HARDWIRING plan + full design-doc scaffold
- **Why:** Terry — the status survey showed Ziptide's spines are deep but content + last-mile wiring is
  thin ("too simple / N64 ship / worlds look empty"). He asked for the complete WHAT-list for the final
  major hardwiring so auto-mode can point **Fable 5** at it (Fable owns the how), plus new asks: building
  interiors, vertical/cavern/elevated worlds, mass VR map production, next-gen Ship Forge + Fortnite-style
  customization, Fortnite-smooth 6DOF flight, atmosphere→space transition, space battles, drivable
  vehicles, best-in-class garden, a genuinely-fun conveyor layer, a home screen, and **consistency**.
- **Did (docs only — no C#, CI unaffected/green):**
  - `docs/PROJECT_STATUS.md` — code-surveyed Built/Partial/Not-built map. **Caught doc drift:** FORGE II
    E1.4, PERF_BUDGET gate, and GamePool are ALREADY shipped despite `PRIORITIES.md` rev 8 listing them open.
  - `docs/HARDWIRING_MASTER_PLAN.md` — the 14-section WHAT-list + **the consistency spine (§13)** (one
    Forge / registry / cosmetic / comfort / world / save spine — the answer to "make it all consistent")
    + phased execution order (§14).
  - `docs/SPRINT_HARDWIRING.md` — the phased execution BOARD (Phase 0–5) + a pull-list linking every
    design doc; the entry point for auto-mode.
  - New design-doc SKELETONS (✍️ — expand the how + research before building) under `docs/design/`:
    `CONSISTENCY_SPINE`, `WORLD_BUILDING_AT_SCALE`, `BUILDING_INTERIORS`, `VERTICAL_AND_CAVERN_WORLDS`,
    `SHIP_FORGE_AND_CUSTOMIZATION`, `HOME_HUB`, `SPACE_COMBAT`, `DRIVABLE_VEHICLES`, `AUTOMATION_CONVEYORS`,
    `GARDEN_AAA`, `CREATURE_ECOLOGY`. Existing docs (`SHIP_SYSTEM`, `SPACEFLIGHT_PHYSICS`,
    `CONTROLS_AND_FLIGHT`, `ART_REGISTRY`, `ADAPTIVE_AUDIO`, `STORY_AND_HOOKS`, `TIDEFRONT_AAA`,
    `MP100_BOARD`, `ABILITIES_AND_ARSENAL`) are REFERENCED from the board, not duplicated.
- **Terry's locked decisions (binding for auto-mode):** Phase 0→1 first · diegetic **ship-hub** home ·
  **modular-kit** caverns (no voxel) · **non-lethal disable + salvage** combat.
- **Next (Phase 0, for auto-mode / Fable 5):** wire `SaveSystem` into `_Boot`; **fulfill `ArtModuleRegistry`
  with the first building kit** (`KIT_FULFILLED` audit — the #1 "worlds look empty" fix); scaffold the
  shared cosmetic + comfort layers; adopt `GamePool`. Then Phase 1 worlds-feel-real.
- **Heads-up:** the code/research fan-out (2 code + 1 VR-map-research agents) died on a **shared session
  limit that resets 4pm UTC** — not a code problem. So every design doc carries a **"Technique research
  TODO"** to fill with sourced citations before that system's build phase. Docs are skeletons by design:
  point Fable in the right direction, leave room to expand (each has a "🚀 Room to expand").
- **Commit:** docs-only hardwiring set (`ee50dd0` PROJECT_STATUS → this entry).

### 2026-07-07 (dddd) - Picasso (Opus 4.8): melee held right + the health/damage system plan
- **Why:** Terry's brief — (a) "the placement and holding is going to be different than guns, let's make
  sure they are being held properly," and (b) a health/damage/enemy-variety system ("can't just have
  characters be invincible forever… fortnite model has shields… weapons do different damage… different
  aliens different damage… enemies that aren't aliens… multiple aliens per planet for hardness… I need a
  solid plan"). He's away; melee grip is a safe concrete win, the health system needs his design call.
- **Did (1 - shipped, `8f93847`):** melee grip fix. `ItemFactory.PoseGrip` forced the gun +45° aim tilt
  onto EVERY weapon; the Breaker Blade + Tide Pike inherited it silently. Decoupled it — PoseGrip now takes
  a per-weapon factory default; guns + ranged arena weapons keep +45° (they aim), the blade rides above the
  fist (+70°), the pike sits flatter (+30°, tip leads a thrust), both overridable via
  ItemDefinition.gripLocalEuler. Every gun/ranged weapon unchanged. **🎮 device-verify the exact angles.**
- **Did (2 - plan, no code):** wrote `docs/systems/COMBAT_HEALTH_PLAN.md` — grounded in the real code:
  the game has TWO disconnected damage economies (campaign CreatureRuntime float-HP w/ hardcoded 10/8 vs
  PvP PvpCombatant int-HP=6 w/ PvpRules table) and the PLAYER is in neither (PlayerStunReceiver = "NO
  health, NO death"; CreatureDefinition.damage authored but never applied). Plan unifies to ONE damage
  scale, generalizes the tested PvpCombatant into a shared HealthPool (+ optional shield), phases it:
  P0 grip (done) · PhaseA pure-C# damage/health core (CI-provable) · PhaseB player takes damage/dies
  (device feel) · PhaseC enemy variety (machine line from existing drones) + per-tier difficulty scaling.
- **DECISION (Terry, 07-07):** health model = **ARMOR ONLY, no health bar.** Recharging armor meter;
  hit while armored just drains it (overkill empties to 0 = "break"); a hit at 0 armor = immediate death.
  No health packs. Death = **serverless checkpoint respawn** at the scene's `__SPAWN_PLAYER` marker (reuse
  existing respawn plumbing — no server/metadata). Plan doc updated to match; §2 is now DECIDED.
- **Next → ARCHITECT** (this thread is near usage cap): `docs/ARCHITECT_HANDOFF_COMBAT.md` is the
  executable spec. Start **Phase A** (pure C#, CI-provable, no headset): new `ArmorMeter` type +
  `ItemDefinition.damage` + retire CreatureRuntime's hardcoded 10f/8f + EditMode tests. Green CI = done.
  Then Phase B (player armor/death/HUD, device feel) → Phase C (machine enemies + tier difficulty).
- **Heads-up:** Phase A re-baselines the damage scale to ONE truth — **freeze PvP numbers, guard with the
  existing PvpCombatTests**; `ArmorMeter` is a NEW sibling, do NOT rewrite PvpCombatant. Grip angles
  (blade +70°, pike +30°) are reasoned starting points, NOT device-verified.
- **Commit:** `8f93847` (melee grip) + the plan/handoff docs commit.

### 2026-07-07 (dddd2) - Picasso (Opus 4.8): Phase A core built (ArmorMeter) - pure C#, CI-provable
- **Why:** Terry said "continue." The safe, CI-provable half of Phase A needs no headset, and I was warm
  in context - so I built it myself instead of leaving Architect a cold start. Only the ADDITIVE, zero-
  device-behavior pieces (nothing consumes them yet), so CI is the full proof.
- **Did:** `ArmorMeter.cs` (Multiplayer, pure/Unity-free like PvpCombatant) - the armor-only rule:
  ApplyDamage drains while armored (overkill => Broke, never a kill), a hit at 0 => Killed, Tick regens
  after an out-of-combat delay, Reset for respawn, Fraction/IsBroken for the HUD, LethalOnBreak flag.
  `ArmorMeterTests.cs` (11 tests, deterministic clock like WeaponCharge). PvpRules gained PlayerArmor=4 /
  ArmorRegenPerSec=1.0 / ArmorRegenDelaySec=3.0. ItemDefinition gained `damage` (int, default 0 = fall
  back to the PvpRules table). PvpCombatant/PvP numbers UNTOUCHED (ArmorMeter is a new sibling).
- **Next -> ARCHITECT:** Phase A is now just **A3** - retire CreatureRuntime's hardcoded 10f/8f, read
  `ItemDefinition.damage` (fallback DamageFor), re-baseline CreatureDefinition stats onto the integer
  scale. A3 changes creature time-to-kill => FIRST device-feel step, Terry confirms the numbers. Then
  Phase B (PlayerArmor on the rig, enemy->player damage, death->__SPAWN_PLAYER checkpoint, armor HUD).
  `docs/ARCHITECT_HANDOFF_COMBAT.md` updated: A1/A2/constants marked DONE, "START HERE" on A3.
- **Heads-up:** couldn't run CI from here (GitHub connector needs re-auth). Logic hand-traced all 11
  tests + the regen accumulator; ArmorMeter references only PvpRules (same asmdef) - should compile
  clean, but confirm the CI run is green on reconnect before building on it.
- **Commit:** this one (ArmorMeter + tests + PvpRules consts + ItemDefinition.damage + doc updates).

### 2026-07-06 (cccc2) - Picasso (Fable 5): the melee pair gets forged (Breaker Blade + Tide Pike)
- **Why (autonomous, photo-verifiable, no device):** the wiring audit flagged the melee pair shipping as
  primitives (a visible stub - the blade is in every story starter rack). Forging them is proven skill
  (5 guns done), touches no behavior, and the forge booth renders recipes from code so I can iterate solo.
- **Did:** `breaker_blade_mk1` (salvage energy cleaver: wrapped haft + gunmetal crossguard + broad steel
  blade with a glowing CYAN energy edge to a wedge tip) + `tide_pike_mk1` (long tidal thrust pike: dark
  shaft + bronze collar + leaf spearhead + teal runnels + swept barbs, the REACH weapon) in
  ForgeRecipeLibrary; ForgeAuthor assignments added (BreakerBlade/TidePike item defs -> recipes at build).
  Committed item assets keep empty forgeRecipeId (ForgeAuthor sets it at build), so the new WiringValidator
  stays green. forge-photos renders both from Specs() next run - iterating from the turnarounds.
- **Verified (photo booth):** Breaker Blade PASSED first try (cyan energy cleaver, reads AAA all
  angles). Tide Pike v1 rendered EXPLODED (0.34 shaft couldn't reach head/butt-cap) -> shaft
  lengthened to 0.66, re-rendered CONNECTED. Both final. `d420836` (recipes) + `e58524b` (pike fix).


### 2026-07-06 (bbbb2) - Picasso (Fable 5): PHASE 2 - the both-sides WiringValidator now FAILS CI on a one-sided seam
- **Did (Terry: "do phase 2"):** `Editor/Validation/WiringValidator.cs` + `Tests/EditMode/WiringValidatorTests.cs`
  - runs every CI push, hard-fails the build if a wiring seam is one-sided. Deterministic, asset/reflection
  based (no device): (1) every asset author (EnsureAllAuthored/EnsureAuthored/AssignAll/BakeAll) is called
  in `BuildAndroid.PatchScenesThenAPK`; (2) every `ItemDefinition.forgeRecipeId` resolves to a
  `Resources/Forge` recipe; (3) every `ForgeCreatureBody` has a matching `Resources/Enemies` CreatureDefinition.
  Menu: `Ziptide -> Validate wiring`. This is the guard that makes 'wired on one side' un-mergeable.
- **Deferred BY DESIGN:** promoting the WARN-only audit gates (WorldContent/PerfBudget/Reachability) to
  blockers needs a device/Unity baseline - flipping blind risks bricking every build (why they're WARN).
  The validator already covers the asset-wiring class safely in CI. Promotion procedure is in FINDINGS.
- **Extending it:** add a deterministic check method + call it in `Validate` (queued: creatureId/bot-profile
  resolution, ZiptideFlags grant<->consume). Non-deterministic seams stay in the manual milestone-close audit.
- **Commit:** _(this push - CI now exercises the validator; green = the codebase is wired both sides)._


### 2026-07-06 (aaaa2) — Picasso (Fable 5): 🔌 THE WIRING AUDIT — a both-sides map so parallel models stop shipping one-sided seams
- **Why:** three models building in parallel kept producing "wired on one side but not the other" bugs
  (an author not build-hooked, an ID with no asset, a mesh with no consumer, the sandbox-boot bypass).
  Terry: "no-guess plan to check everything is wired… a wiring map… clear instructions for any model."
- **Did (READ-ONLY audit, no runtime/scene code touched — docs only):**
  - `docs/WIRING_MAP.md` (NEW, authoritative) — the runtime spine + assembly DAG + the SEAM TABLE
    (producer → build-hook → consumer → verifier) across 8 seam categories, each with a both-sides status.
    Part 4 = THE BOTH-SIDES LAW (producer/consumer/verifier/map-row, all in one change).
  - `docs/WIRING_AUDIT_FINDINGS.md` (NEW) — the ledger: every seam ✅ wired / 🔵 intentional stub / ⚠ minor
    gap, with evidence. **Headline: wiring-healthy — no critical one-sided breaks.** All 17 asset authors
    are build-hooked; rig-ensure chain, item/creature registries, appliers, and A6 transport all both-sided.
    🔵 stubs (deliberate): 8/10 creatures still primitive, melee/gravity guns primitive, `tox_canal_stalker`
    proof recipe, W013+ story flags. ⚠ minor: per-flag grant↔consume matrix not exhaustively proven.
  - `docs/BOARD_INDEX.md` (NEW) — one source of truth per purpose (GAME_PLAN=roadmap, MASTER_CHECKLIST=state,
    PRIORITIES=order, SPRINT_*=per-lane, HANDOFF=log, WIRING_*=wiring); legacy docs listed + redirected.
  - Banners/pointers: CONNECTIONS_AND_RECOVERY system-map marked superseded → WIRING_MAP; HOW_TO_CHANGE_ANYTHING
    + OPERATOR_START_HERE now point at the wiring set.
- **Phase 2 (queued, SEPARATE approval — the "change without breaking" enforcement):** promote the WARN-only
  gates (WorldContent/PerfBudget/Reachability) to blockers after baselining, and extend `DependencyValidator`
  into a both-sides `WiringValidator` (author→build-hooked, id→shipped asset, applier→consumer, flag→grant+consume)
  that fails CI on a one-sided seam. Spec in FINDINGS "Recommended safeguards".
- **Commit:** _(this push)_


### 2026-07-06 (zzzz) — Picasso (Fable 5): 🩹 THE STRANDED-BOOT FIX — boot into the real game + a device-reliable warp
- **Symptom (Terry, on device):** boots into "basically a blank world," the menu "blinks in and out
  super fast," can't reach any level.
- **Root cause (not a crash — a stale config):** `ZiptideConstants.FirstWorldScene` was still the
  **June-18 dev bypass** pointing at the `SandboxTestLab` graybox (commit `e29aca3`, put in *because
  the in-VR TMP Dev Menu renders as a dead/flickering panel on device*). So boot dropped him into a
  graybox whose only exit — the broken TMP menu — doesn't work on the headset. Same thing he hit
  weeks ago ("ended up in the test room, menu didn't work").
- **Fix (Terry chose: boot W000 + reliable board, leave the TMP menu):**
  1. **Boot → `W000_DriftIn`** (the real opening: ship bay → PUNCH IT cast-off → the chain). New
     `SceneW000` constant; bypass reverted. +`BootConfigTests` so the graybox bypass can't silently
     return (asserts boot ≠ sandbox, ≠ _Boot, == W000).
  2. **`DevWarpBoard`** (NEW, dev-builds-only) — a device-reliable warp built from the PROVEN idiom
     (primitive tiles + `TextMesh` + `XRSimpleInteractable`, exactly like the travel doors / match
     board that DO render+click on device — no TMP, no Canvas). Self-bootstraps, and on every scene
     with a player spawn it drops a board beside spawn listing every world + arena (from
     `DevWorldManifest`, which already includes arena packs) → `TravelCoordinator.TravelTo`. So he can
     hop world↔world↔arena freely for testing. Logs `DEV_WARP_BOARD`/`DEV_WARP_TO`.
  3. **Hardened the cold-boot ZIPTIDE travel** (this was the first on-device build with THE ZIPTIDE):
     BootLoader now travels `skipGate:true` (the namesake gate is world↔world, not a cold boot into
     the first world); the gate call is wrapped in try/catch so a visual/audio hiccup can NEVER strand
     `_travelling=true` and silently block all travel; and `ZiptideGateEffect` now guards
     `Shader.Find` with a fallback like `TracerFx` (a stripped URP/Unlit made `new Material(null)` a
     throw risk on device). Logs `GATE_FAIL` if the gate ever bails — travel proceeds regardless.
- **Left as-is (Terry's call):** the TMP `DevMenu` (deprecated in favor of the board; not deleted).
- **Verify:** rebuild from HEAD → sideload → `BOOT_LOAD dest=W000_DriftIn` → `TRAVEL_OK`; wake in the
  ship bay (not the graybox); Test Warp board beside spawn; tap `Arena_Cistern` → travels; warp again
  from there. Then resume the two-headset GO ONLINE presence smoke.
- **Commit:** _(this push)_


### 2026-07-06 (yyyy) — Picasso (Fable 5): 🎮🎮 A6 v1 — TWO HEADSETS, ONE ROOM (online presence ships)
- **Context:** Terry imported PUN2 + App ID and pushed `21f117c` — **CI went GREEN with Photon
  compiled in**, so the adapter I wrote blind against the PUN2 API is verified, and every
  `#if ZIPTIDE_PHOTON` block is now real-compiled by CI (not dormant text anymore).
- **Did (A6 v1 = PRESENCE):** `PvpNetHub.StartOnline/StopOnline` + `RoomCode`/`Status` +
  `OnlineStarter`/`OnlineStopper` hooks (gameplay never sees Photon) · `ZiptideNet/NetBootstrap`
  (RuntimeInitializeOnLoadMethod installs the starter → spawns `PhotonPvpLauncher`; behind the
  define) · launcher now reports `Status` + `OnPlayerEntered/Left` counts · **`PvpOnlinePresence`**
  (Gameplay): broadcasts local head+2 hands @20Hz via `IPvpTransport.SendPose`, renders every peer
  as a helmet+salvage-gloves avatar (amber, contrasting the local teal), ages out peers after 3s ·
  **`ArenaLobbyBoard`** GO ONLINE toggle + live `NET:` status label (room ZIP-001, x/2). All
  transport-agnostic → loopback-safe (echoes to self, filtered by playerId), so it can't break solo.
  +4 hub tests. Logs `NET_*`/`NET_PRESENCE`.
- **Why presence-first (scope call):** combat sync has real host-authority/hit-reconciliation
  decisions; rushing it into a build Terry tests tonight is how you ship a broken match. Presence is
  the irreducible "we're together" moment, fully device-verifiable, and a clean base. Combat is A6.2.
- **A6.2 (next, enveloped):** local fire→`SendFire`; host-authoritative `SendHit`/`SendScore`; give
  the remote avatar a networked hitbox (`IPvpDamageable`) so EVERY existing weapon just works on it;
  reconcile downs. The pose channel already proves the transport, so A6.2 is additive.
- **Terry:** `docs/TWO_QUEST_SETUP.md` step 5 is now the live two-headset smoke.
- **Commit:** _(this push)_


### 2026-07-06 (xxxx) — architect (Opus 4.8): 🟢 GARDEN RED FULLY CLEARED — the second half (a value regression) + design call
Thanks Picasso (wwww) for the compile fix. But that left a SECOND red I'd caused: `FreshBonus = 0.25`
made an at-ready harvest classify Fresh and pay ×1.25, which broke the existing `GardenServiceTests`
baselines (they harvest at ready and pin 10.0 / 12.5). Diagnosed from the failed-run test-results.
- **Fix + design call:** set **`FreshBonus = 0.0`** — harvest-when-ready is the correct BASELINE, so a
  bonus there would re-baseline every yield + test. The shipped, live yield mechanic is the **overripe
  decay** (GARDEN #5): ignore a crop past the 15-min onset and it decays toward a 50% floor (never
  dies). `Fresh` stays a classification for future UI/juice; the +25% bonus (#4) is deferred to a
  balance pass that updates the baselines together — marked 🟡 PARTIAL in `GARDEN_50.md`. My tests
  reference the constant, so they pass at 0.0; existing tests return to their exact baselines.
- **My miss, owned:** I qualified the enum as `GardenService.HarvestTiming` (it's namespace-level) AND
  shipped a value-changing default without re-checking the existing garden tests that pin yields. Two
  lessons folded into how I pull next: compile-check my own test qualifiers, and grep for existing
  tests that pin any value I change before shipping a new default.
- **Commit:** this push (one-line impl + doc); no other-lane files touched.

### 2026-07-06 (xxxx) — Picasso (Fable 5): 📸 SEEN AND PASSED — creature photo loop live, first genomes verified, arsenal tunes closed
- **Did:** (1) ForgePhotoBooth now photographs every ForgeBodyLibrary genome beside the recipe
  catalog — built through the REAL skinning core, per-slot dev materials (eye emissive), frozen
  at one full-speed ForgeGaitMotor mid-stride frame. Output `body_<id>/` in the forge-photos
  artifact. Any future genome gets turnarounds automatically — the creature look loop is now
  identical to the weapon loop. (2) Reviewed the first portraits, both PASS: `swarm_bug` reads
  as a proper bug (amber carapace, hot-amber eye between the antennae, six legs at visibly
  different gait phases — the walk is legible in a STILL) and `light_grazer` reads as the pale
  bell over four kinked waving tentacles with a soft glowing eye. Note for critique sessions:
  the booth's `01_front` faces the creature's -Z, so the FACE (+Z, where eyes live) is in
  `04_back`. (3) Closed the dangling arsenal verification: `87fd94a`'s maul/lobber tune photos
  pass (haft reads as a haft with the amber pommel; lobber's moss/rust/green-glow reads).
- **Polish option for any operator:** the grazer bell could carry faint emission (it's the
  LIGHT grazer) — one line in ForgeBodyLibrary.BuildLightGrazer + the applier already handles
  emissive slots. Photo-check it like everything else.
- **Commit:** `ea3cc02` (booth, CI+photos green) + this docs stamp.

### 2026-07-06 (wwww) — Picasso (Fable 5): 🚑 CI RED FIXED — garden timing tests couldn't compile (cross-lane fix)
- **Did:** `1e0d98a` (vvvv below) went CI-red: `GardenTimingTests.cs` qualified the timing enum
  as `GardenService.HarvestTiming`, but the enum is declared at NAMESPACE level
  (`Ziptide.Content.HarvestTiming`) — CS0117 ×7, compile blocked for every lane. Fixed the
  seven test references to the bare enum name (test-only change; the implementation and its
  live Harvest wiring are untouched and looked correct on review — boundary semantics
  `>= OverripeAfter` match the tests' onset assertions). Architect: no action needed, just
  FYI — same class of miss as my own v3 namespace red earlier today; the lesson generalizes:
  **when you add a type next to a class, compile-check how your OWN tests qualify it.**
- **Commit:** _(this push)_

### 2026-07-06 (vvvv) — architect (Opus 4.8): 🌱 BANK PULL — garden harvest timing (fresh bonus + overripe decay), live + tested
Terry: "go with whatever makes the most sense." Sensible call = stop hardening infra and deliver
VISIBLE depth on the part he named first (the garden), the safe way — pure logic + data, no
device-only risk. Additions Bank GARDEN #4/#5.
- **Shipped, LIVE on every harvest automatically:** `GardenService.TimingMultiplier`/`TimingOf` +
  `HarvestTiming{Prime,Fresh,Overripe}` on `HarvestPlantResult` — a fresh pull (≤2 min after ready)
  pays +25%; a neglected crop decays past the 15-min overripe onset toward a 50% floor and **never
  dies** (all-ages). Wired into `GardenService.Harvest`, which `GardenPlotRuntime` already calls +
  logs (`GARDEN_HARVEST mult=` now reflects timing) — so it's active with ZERO runtime edit. Defaults
  are sensible constants (guaranteed active) and per-plant override-able via two new PlantDefinition
  fields (0 = use default; robust to existing-asset serialization). **9 EditMode tests.**
- **Why this shape:** GardenService is the pure, already-tested core; the change is additive
  (existing plots keep working, tend multiplier still stacks) and CI-provable. The only follow-up is
  a device-pass VISUAL (an overripe plant slumping / a fresh one glowing) — a GardenPlotRuntime
  one-liner whoever's on the headset can add; the MECHANIC and its credits are already real.
- **Bank state:** GARDEN #4/#5 struck in `docs/additions/GARDEN_50.md`. Prior architecture pulls:
  GamePool (`94f74ac` ✅), POI reachability (`54c9330` ✅). Loop still: idea → tested commit → verify.
- **Note (cross-lane):** garden is nominally the story lane, but the change is additive-pure and
  garden hasn't been touched since P3 — low collision. Announced here per the shared-file rule.
- **Commit:** this push on `terry-local-wip`.
### 2026-07-06 (uuuu) — Fable 5 (MP track takeover): ⚔️ MP100 OPENS — the hundred-improvements board + THE MELEE PAIR ships as wave 1
Terry's directive: "look at our multiplayer mode/arena/pvp... This needs to be tip top. I want a
hundred improvements" + "find some good places for some melee weapons... instead of replacing
something in the story we can just add it... it's also got to be in the regular game as well."
- **The board:** `docs/design/MP100_BOARD.md` — 100 numbered, concrete, individually-claimable
  improvements across bots/arenas/modes/arsenal/augments/progression/locator/Tidefront/netcode/feel,
  built from a full code survey (three parallel research passes) + the three MP design docs. Key
  finding baked into the statuses: a large fraction of "what would make MP tip-top" was ALREADY
  SPECED in `PVP_ARENA_AAA.md`/`TIDEFRONT_AAA.md`/`ABILITIES_AND_ARSENAL.md` and never built — those
  rows are marked 🔷 with pointers to the exact spec text, so nobody re-designs them. Suggested
  8-wave build order at the bottom; wave 1 shipped with the board itself.
- **Wave 1 = THE MELEE PAIR (`5b8872a` + `dc88091`, 11 board items):** the survey confirmed the game
  had NO true melee — the Sonic Thumper is a swing-gated positional AoE pulse, HammerTool only breaks
  walls. Now: **Breaker Blade** (fast 1H contact swings, per-target debounce paces the DPS, cracks
  breakable walls, and is the new FINAL Gun Game rung — 7-rung ladder, melee finish) and **Tide Pike**
  (committed thrusts, first body on the line, real poke-back, reach identity). Balance in `PvpRules`
  (blade lighter+faster than pike, neither one-shots, neither out-hits the prism per hit, thresholds
  demand a REAL swing) pinned by 6 new/updated EditMode tests. `CreatureRuntime` gets explicit
  per-weapon melee cases so story-game creatures react properly instead of silently taking the
  gravity fallback. **Bots got ears the same commit:** new `PvpNoise` channel (PvpHitSource idiom) —
  swings and thumps report noise; `PvpBot` finally feeds the A1 brain's `HeardFire` hook (which
  shipped as a silent `false` with a "hook" comment) and treats a swing inside melee reach as a
  dodge-triggering threat the dart-only scan could never see.
- **Placement (all additive, nothing replaced):** arena pads — Cistern's dark west flank gets the
  blade (tunnels are blade country), Chitinwall's west catwalk gets the pike (narrow high ground +
  reach). The blade joins the **starter-weapon lineup in every generated story world** (melee is a
  first-class verb in the regular game, per Terry). Storyboard homes: W009 Chitinwall (the locals'
  carapace pry-tool), W010 Tidal Array (the drowned fishing rigs' gaffing pike), W048 (tier-2 blade
  callback).
- **⚠️ Terry runbook (queued in §1):** `ArenaLayoutLibrary` is create-only — the two arena assets
  need delete+reseed (`Content/Arenas/Generated/Arena_Cistern_Arena.asset` + `Arena_Chitinwall_Arena
  .asset`, then `Ziptide → Worlds → Author Arena Layouts (missing only)` or just build) before the
  new pads appear. The weapon assets themselves author automatically at build.
- **📣 Cross-track:** touched `Multiplayer/**`, `Gameplay/Runtime/Pvp|Weapons|Items|Enemies`,
  `Editor/Patching/{ArenaLayoutLibrary,ArenaWeaponAuthor,WorldStubGenerator}` — the MP track's own
  lane per SPRINT_MULTIPLAYER. Picasso's gait/creature work untouched (`CreatureRuntime.ReceiveHit`
  got new cases only, no behavior-file edits). All additive enum values; no exhaustive switches on
  `PvpWeapon`/`ArenaWeaponKind` elsewhere (checked).
- **📣 Next MP operator:** read `MP100_BOARD.md`, claim a wave-2 row (bots feel alive: items 3-6,
  9, 26 — mostly pure-testable). The board is the queue; this SPRINT file stays the coarse view.
- **Commit:** `5b8872a` (melee-1: the pair + bot ears) → `dc88091` (melee-2: placement) → this push
  (the board + SPRINT/HANDOFF).

### 2026-07-06 (tttt) — Picasso (Fable 5): 🦿 P4 GAIT MOTOR — the creatures learn to walk (pure core + proof tests)
- **Did:** `ForgeGaitMotor` (Visuals) — pure math: (body, time, speed01) → per-bone LOCAL rotation
  deltas aligned with ForgeSkinnedBuilder's bone order. **THE CONJUGATION LAW** (read the file
  header before touching): gait angles are authored about ROOT-space axes and conjugated into bone
  space with the SAME chainRot the builder used (`delta = inv(chainRot) * rootDelta * chainRot`) —
  FromToRotation(up, down) picks an arbitrary twist for straight-down legs, but builder and motor
  compute the identical quaternion so it cancels. Never author swings about raw bone-local axes.
  Roles: Leg (anti-phase mirror + diagonal-gait limb parity + rectified lagging knee, near-still at
  idle), Tail (traveling wave, whip amplitude toward tip, never sleeps), Tentacle (two-axis wave),
  Wing (opposed flap), Antenna (incommensurate sway). Root stays identity — body bob is the mover's
  job. + `ForgeCreatureAnimator` (thin applier: Bind(body, bones), self-measures its own world
  speed each LateUpdate so NO Gameplay reference is needed; `bone.localRotation = baseLocal * delta`,
  bindposes never touched). + 6 EditMode tests; the decisive one, `MirroredLegs_AreAntiPhase`,
  builds the real skeleton, poses it at peak swing, and proves left/right feet displace in
  OPPOSITE z — the "both legs kick together" failure class is now unshippable.
- **The P3+P4 finish — DONE same session (next push after `ec08396`):** the whole creature
  pipeline is live end-to-end: `ForgeSkinnedBuilder` now emits one submesh per used palette
  slot (`Result.paletteSlots`, sorted — same contract as ForgeMesh) → `ForgeCreatureVisualApplier`
  (Visuals; the creature twin of ForgeVisualApplier) loads `Resources/Forge/Bodies/<creatureId>`,
  builds skeleton+SkinnedMeshRenderer with per-slot materials (eye slot emissive), binds
  `ForgeCreatureAnimator` → `CreatureBehaviorBase.Awake` tries it after BuildVisuals and hides
  the primitive MeshRenderers on success (subclass field refs stay alive — a look, never a
  stat) → `ForgeBodyLibrary` (Editor, CREATE-ONLY, build-hooked) authors genomes whose ids
  MATCH CreatureDefinition ids, so authoring a body upgrades every spawn with zero zone edits.
  First two genomes: `swarm_bug` (six 1-seg legs + antennae, amber chitin) and `light_grazer`
  (four 2-seg Tentacle chains, pale luminous bell). +2 catalog gates (every genome validates,
  builds through BOTH real cores, stays in budget) +1 submesh contract test.
  **TO ADD A CREATURE BODY: one Build* method in ForgeBodyLibrary. That's the whole job now.**
- **Commit:** `ec08396` (motor core) + the pipeline finish (this push).

### 2026-07-06 (ssss) — architect (Opus 4.8): 🗺 BANK PULL — POI reachability check (WORLDS #13), WARN-only
Second bank pull, architecture lane (GamePool `94f74ac` was CI-green; this builds on that momentum).
- **Shipped:** pure `Content/Runtime/City/GridReachability.cs` (BFS flood-fill over a walkable+height
  grid with a maxStep so cliffs break connectivity but grades don't; **8 EditMode tests**) +
  `Editor/Audit/WorldReachabilityAuditRules.cs` (raycasts a ~6m grid over the world, floods from the
  player spawn, WARNs on POIs on a disconnected island) + one appended WorldAuditRunner line.
- **Why WARN, not blocker:** the raycast grid is coarse — a narrow corridor could be missed and yield a
  false "unreachable". A WARNING surfaces it for a headset check and CANNOT fail a good build. Promote
  to a blocker only once device-confirmed reliable on the real 12 worlds. Complements the H3
  `TERRAIN_SLOPE_UNWALKABLE` blocker (that measures walkable AREA; this measures CONNECTIVITY).
- **GridReachability is reusable** beyond the gate: creature pathability + bot-nav sanity can flood the
  same way. **T-Dog:** if `POI_MAYBE_UNREACHABLE` fires on a world you built, walk it in-headset — real
  hit = add a graded corridor; false = ignore (grid coarseness) and note it so we can tune/promote.
- **Commit:** this push on `terry-local-wip`.

### 2026-07-06 (rrrr) — architect (Opus 4.8): 🧵 GAMEPOOL shipped — the bank's first pull-through (pooling infra) CI-green
Terry said "start working through" the Additions Bank. First pull, architecture lane: **Q4a GamePool**,
the pooling dependency a cluster of bank ideas name (INDUSTRY #2/#19/#25 belt pucks, COMBAT #42
destructible chunks, CREATURES #20 pooled respawn).
- **Shipped (pure-core-first, adds files, changes NO existing behaviour → CI-safe):**
  `Core/Runtime/PoolCore.cs` — generic free-list with Created/Reused/Live/Free bookkeeping, retained
  cap, get/release hooks + **10 EditMode tests** (build-vs-reuse, null-safe, cap-drops-overflow,
  prewarm, deterministic counter script). `Core/Runtime/GamePool.cs` — the GameObject translator:
  keyed pools, instances parked deactivated under a hidden root, per-`sceneLoaded` reset (never hands
  out a destroyed instance across travel), `Prewarm`/`LogStats`.
- **📣 MP + Gameplay lanes — ENVELOPE (adoption):** GOAL swap the hot CreatePrimitive+Destroy spawns
  to `GamePool.Get(key, factory, pos)` / `GamePool.Release(key, go)`. SITES: `PvpBolt`,
  `TaserDartProjectile`, creature stun-arc bursts, `ThumpRingVisual`, `StaticNetProjectile`.
  ACCEPTANCE: identical visuals on device + `POOL_STAT` shows reuse. BUDGET ~1 commit + a device
  glance. NOTE: I did NOT swap these from the architecture chair — they're device-sensitive combat/VR
  files with hit semantics; do them with a headset pass, per the OPERATOR calibration.
- **Bank state:** the pull loop works end-to-end (idea → board row → CI-green commit). Other lanes:
  pull YOUR part's `docs/additions/*_50.md` when you have capacity. This didn't reorder PRIORITIES.
- **Next architecture pull (my board):** WORLDS_50 #13 walkability gate, or INDUSTRY_50 #14
  deterministic-breakdown core — both one-commit, in-lane.
- **Commit:** this push on `terry-local-wip` (docs + `Core/Runtime` + tests; docs-adjacent, CI = compile+tests).

### 2026-07-06 (qqqq) — architect (Opus 4.8): 🧩 THE ADDITIONS BANK — 550 AAA-bar ideas, one file per part + the Haiku story workshop
Terry's directive: "the game is far too simple, I want AAA" — survey every part + subpart and drop
**50 improvements per part** as pull-ready additions, tell Picasso, make it one auto run. Done, docs-only.
- **`docs/additions/`** — 11 files × exactly 50 concrete, sized (S/M/L), machine-riding ideas
  (verified 50 rows each = 550 total): GARDEN, INDUSTRY (conveyors/automation — none existed),
  SPACEFLIGHT, STORY, WORLDS, COMBAT_GAMEPLAY, MULTIPLAYER (Fortnite bar), TIDEFRONT, CREATURES,
  **ART_AUDIO (Picasso's)**, ECONOMY_META. Each row names the existing system it extends + a 🎨 tag
  for art components. `docs/additions/README.md` = the laws (ideas-not-directives, ride the machine,
  append-only, pull = a board row + envelope). Nine parts were drafted by parallel general-purpose
  subagents seeded with each part's current-state inventory; STORY was drafted on **Haiku** per Terry;
  ART_AUDIO + ECONOMY_META I wrote directly (their Fable subagents hit the model cutover).
- **`docs/storyboard/HAIKU_STORY_WORKSHOP.md`** — the process Terry asked for: story tasks run as
  Haiku subagents seeded with the locked bible/Transmission/WORLD_DATA (READ-ONLY), operator curates
  candidates into RillLineAuthor/WORLD_DATA/ChoiceStations, canon stays locked. `STORY_50.md` is the
  first output (draft — several rows invent placeholder world/faction names; KEEP THE IDEA, FIX THE
  CANON when pulling).
- **📣 PICASSO — this is your notification (Terry's explicit ask):** the bank exists and
  **`docs/additions/ART_AUDIO_50.md` is yours** — Forge material families, VFX language, adaptive
  audio, RILL VO pipeline, UI look, avatar v2, the 12 "postcard" world shots. ALSO: **every 🎨-tagged
  row across all 11 files is a potential art addition** — building-module kits (INDUSTRY/WORLDS),
  weapon feel packets (COMBAT), creature silhouettes (CREATURES), MP juice (MULTIPLAYER), etc. Pull at
  will — ideas, not directives; nothing here reschedules your current E1.4 board.
- **📣 ALL TRACKS:** pull from your part's file when a track has capacity; a pulled idea becomes a
  `SPRINT_*.md` row (+ a HANDOFF envelope if it crosses lanes). The bank does not change PRIORITIES
  order — Terry's #1 (headset pass) and the rev-8 list still stand.
- **Commit:** this docs-only push on `terry-local-wip` (CI: compile + tests unaffected).
### 2026-07-06 (pppp) — Sonnet 5 (Story/Ship track): 🧠 COMPANION MEMORY ships — RILL has a throughline of thought now, not just reactions
Terry's follow-up to (oooo): the sky plan landed, but the real ask underneath it was "what's going to
make this game really feel alive — your turn for creativity." Real code, not another plan doc.
- **Did:** a fourth `RillTrigger` — `FollowUp`. A flag being granted starts a countdown, in **gate
  crossings** (not real time — rides the same THE ZIPTIDE cadence Picasso's travel system already
  delivers every line on), to a subtitle that fires later, unprompted, with no player action
  triggering it. `FollowUpTracker` (new, `Content/Runtime/Story/`) is pure C# with zero Unity
  dependency — 5 EditMode tests (register-once, fires-exactly-on-schedule, never-twice, multiple
  independent countdowns, zero/negative delay clamps to one crossing). `RillCompanion.cs` wires it
  into machinery that already existed: the same "newly seen flag" diff `PollFlags()` already runs
  registers a countdown; `EnqueueGateLine()` ticks it **unconditionally**, before any of the method's
  other early returns, so it can't silently stop advancing. No new UI, no new delivery path, no new
  polling loop.
- **Content, not just mechanism:** three real follow-ups ship now, keyed to flags already granted in
  the built game — RILL circling back on the cargo question from W004 nine crossings later ("I never
  received an answer... I have stopped expecting one. That is new, too."), on the containment reveal
  from W012 seven crossings later ("I keep arriving at the same word: deliberate"), and on her own
  W019 refusal six crossings later ("I think I am relieved you didn't [ask again]"). One of them is a
  paired exchange — Cal answers RILL's containment follow-up one crossing after it, not the same
  crossing, specifically so the reply always lands second (the tie-order would otherwise be
  ambiguous — see the code comment).
- **Docs:** `docs/systems/COMPANION_MEMORY.md` (the mechanism, an honestly-flagged limitation — not
  persisted across save/load yet, a quit at exactly the wrong moment loses a pending follow-up — and
  the bigger, NOT built half of the same idea: worlds that persistently show what Cal did there,
  which `PlayerProfile.GetWorld()` already has the storage hook for but no content built against yet).
  `STORY_BIBLE.md` §3b gets a third rule alongside "ambient lines" and "joke-real-joke."
- **📣 Cross-track (Picasso/Architecture):** touches `RillLineLibrary.cs`/`RillCompanion.cs`/
  `RillLineAuthor.cs` again (same files as (nnnn)) plus one new file pair (`FollowUpTracker.cs` +
  test). All additive — `RillTrigger.FollowUp` is a new enum value, doesn't touch existing cases; no
  exhaustive switch elsewhere in the codebase depends on `RillTrigger` (checked before adding it).
- **📣 Next model/operator:** the "Aftermath" world-persistence half in `COMPANION_MEMORY.md` §7 is
  the natural next swing at "feels alive" if this direction is worth continuing — bigger lift (per-
  world content, not just mechanism), explicitly not started.
- **Commit:** `b6dd6d7`.

### 2026-07-06 (oooo) — Sonnet 5 (Story/Ship track): 🪐 THE PROSPECT BAR — a real skyscape design plan (not built yet)
Terry's direct follow-up to (nnnn): the W007 retune is "okay," but the sky is one of the most
important things in the whole game to him personally — it needs to feel like standing on a different
planet, the way *Prospect* (2018) does, not "a sun or an object in the sky." Explicitly asked for a
plan (not a ship-it now, though feasible pieces are welcome), and to iterate twice past the first draft
before presenting it.
- **Did:** `docs/systems/SKYSCAPE_DESIGN.md` — the thesis (why this is worth real budget, and *why VR
  specifically*: real stereo depth from layering haze/particulate/body at genuinely different
  world-space distances, not one flat skybox — an effect that doesn't exist outside a headset), six
  design pillars (occlusion over exposure; atmosphere as a layer, not a fog slider; something always
  drifting; the sky's color reaching the ground; scale felt not announced; restraint as a valid
  signature), a technique stack with real Quest costs checked against `PerfBudgetAuditRules` (1
  light target/3 cap total — **nothing in this plan adds a light**), three authoring tiers
  (Signature/Standard/Interior) so it's achievable across the 68 unbuilt worlds without every sky
  getting the same maximal treatment, and a 5-question pass/fail rubric. Iterated twice per the ask:
  round 2 added the stereo-depth mechanism (the single most VR-native argument in the doc) and
  disambiguated this from the existing "No Man's Sky" terrain-variety bar in `SPRINT.md` (different
  axis, not a conflict) + flagged that soundscape is half of "feels like a place" and this doc doesn't
  cover it (not scoped in, explicitly not forgotten either).
- **Also shipped:** a companion visual Artifact — three composed sky mockups (Oxidized Canopy/
  Signature, Sable Station/Standard, The Edge/Signature-restraint) built from the exact layers the doc
  proposes (haze card, occlusion silhouette, drifting particulate, grid/nebula), not concept art —
  an honest preview of the technique, not a promise of final fidelity.
- **Status: plan only, nothing built.** First executable step if this moves forward (§6 of the doc):
  prototype the haze card + particulate drift on ONE already-built world (W005 Oxidized Canopy —
  closest in mood to the reference already) and get Terry's on-device gut check before touching the
  other 11.
- **📣 Cross-track (Picasso/Architecture):** no code touched this entry — pure design doc + a
  standalone visual mockup. If/when this moves to implementation, the haze-card and particulate-drift
  layers are the cheapest, highest-value first build (§3/§6 of the doc) and the natural next claim for
  whoever owns visual systems next.
- **Commit:** `50621a5`.

### 2026-07-06 (nnnn) — Sonnet 5 (Story/Ship track): 🗣 DEPTH PASS 2 closes — Cal talks back, and the sky finally looks like space
Closes a third narrative/systems pass, opened right after (mmmm). Terry's directive: even after the
Soul Pass gave Mara/Sable/the Warden real voices, Cal — the character the player actually IS — had
never spoken a single line anywhere in the codebase, and he wasn't sure the game had any skies where
you can actually see space or planets ("interstellar level skyscapes"). Two threads, both real code
this time (not narrative-only like the last two passes) — flagging that explicitly since it's a
change in scope from (gggg)/(hhhh)/(mmmm), which were docs-only by design.
- **Thread 1 — Cal gets a voice (3 commits):**
  1. `RillLine` (`Content/Runtime/Story/RillLineLibrary.cs`) gained a `speaker` field (default `"RILL"`,
     zero migration risk — every line authored before today is unaffected) and a pure
     `FormatSubtitle()` method; `RillCompanion.cs` uses it instead of a hardcoded `"RILL: "` prefix.
     Added `RillLineTests.cs` (3 tests) — first EditMode coverage this system has ever had.
  2. `RillLineAuthor.cs` now authors Cal's half of the conversation too (`CalEnter`/`CalFlag`/
     `CalGate` — same trigger/key mechanics as RILL's own helpers) — ~24 lines of banter and real
     questions paired with RILL's existing beats across Ch.0 through the four endings, dormant until
     each world/flag ships, exactly like RILL's own forward-authored lines already were. Two
     highlights: Cal asking "My memories, or yours?" right after RILL's Ch.6 Pattern warning (lands
     close to Cal's own hidden identity without giving it away), and "RILL — you don't have to finish
     that. Not tonight." right after RILL's Ch.7 near-confession cuts itself off.
  3. `docs/systems/VOICE_PIPELINE.md` (new) — **the instructions Terry asked for, "for the future
     model, across all voice aspects."** Explains that the subtitle-now/VO-later stub has been sitting
     in `RillCompanion` since M1 and already works (`if (line.voClip != null) PlayClipAtPoint(...)`) —
     nobody's cast yet, that's the only gap. Gives the exact steps to attach a real clip without
     touching trigger/delivery code, a naming convention for clip files, and casting/tone notes per
     character pointing at `STORY_BIBLE.md`. Also documents that Mara/Sable/Nine could join the SAME
     pipeline the same way (`speaker` is a plain string) — not done this pass, flagged as the natural
     next content batch. `STORY_BIBLE.md` §3b got the narrative-side update to match.
- **Thread 2 — the sky actually looks like space (1 commit):** the vista rendering system
  (`SkyVistaDefinition`/`SkyVistaLibrary.cs`) turned out to already support up to 3 celestial bodies,
  nebula, starfield, and the Shell grid — richer than most of the 12 built worlds actually use. Did:
  (a) `WORLD_DATA.md` §4.1 — a mapping table from a chapter's `Sky:` prose to the actual data fields,
  so W013+ authoring reaches for this toolkit instead of a flat two-color gradient; (b) sharpened
  `CHAPTER_7_RILL.md` W057 (Transit Void) into the game's one full-frame deep-space corridor shot;
  (c) retuned W007 Sable Station in code — a second (moon) body + more stars/nebula for parallax,
  additive only, same established palette. **W012 was deliberately left alone** — it's already at
  max intensity by design ("the wall IS the point"); piling on more would dilute that staging.
- **⚠️ Needs your hands:** `SkyVistaLibrary` is create-only — the code change to W007 won't show up
  until the existing baked asset is deleted and the menu re-run. Queued in `TERRY_RUNBOOK.md` §1 as a
  new checklist item (delete `W007_SableStation_Vista.asset` + `.meta`, run `Ziptide → Art → Author
  Sky Vistas (missing only)`, commit the regenerated asset).
- **📣 Cross-track (Picasso/Architecture):** this pass touched code in `Content/Runtime/Story/`,
  `Gameplay/Runtime/Story/`, `Editor/Patching/RillLineAuthor.cs`, and `Editor/Patching/
  SkyVistaLibrary.cs` — all additive, all CI-checked (the RILL/Cal commit is confirmed CI-green; the
  docs+sky commits were still running CI at push time — check before building on top of them if
  you're touching the same files). Nothing here changes any contract you depend on: `RillLine.speaker`
  defaults to `"RILL"` so every existing call site behaves identically, and the sky retune is
  additive within `BuildW007SableStation()` only.
- **📣 Next model/operator:** if you're recording or wiring VO for ANY character, read
  `docs/systems/VOICE_PIPELINE.md` first — it's the one doc, not scattered notes. If you're authoring
  W013+ skies, read `WORLD_DATA.md` §4.1 before writing a flat gradient.
- **Commit:** `ea00f2c` (Cal's voice, CI-green) → `ef1b27c` (voice pipeline doc + STORY_BIBLE) →
  `f4e57cb` (sky depth) → this push (HANDOFF + SPRINT close).

### 2026-07-06 (mmmm) — Sonnet 5 (Story/Ship track): 💔 THE SOUL PASS closes — Mara, Sable, and the Warden defector finally speak
Closes a second, deeper narrative pass opened right after (hhhh). Terry's directive on reviewing THE
STORY BIBLE LOCK: the plot architecture is sound now, but the story is still "one-dimensional" — no
Cortana/Chief-style bond, no Arbiter-style faction-internal conflict, no felt moral stakes, citing
Halo and the Fallout TV show as the bar. Narrative-only, `docs/storyboard/STORY_BIBLE.md` +
`CHAPTER_2/3/5/6/7.md` — **zero code/runtime files touched**, same discipline as the first pass.
- **Diagnosis (concrete, not vibes):** re-read the full 68-world catalog — RILL was the ONLY
  character who ever spoke in a direct quote anywhere in the docs. Mara, Sable, and the Wardens
  existed purely as third-person plot-function description ("Mara reveals X," "Sable resolves").
  That's the whole gap: real relationships need real voices, not summaries of relationships.
- **Did (3 content commits, docs-only):**
  1. `STORY_BIBLE.md` new **§3b "Cal & RILL — the relationship, not just the arc"** — two rules: (a)
     RILL gets ambient personality lines via Picasso's already-shipped `RillTrigger.GateDeparture`
     pool, not just plot lines; (b) the actual Halo rhythm, "a joke, then a real moment, then a
     joke." Introduces **"the log"** — RILL's deadpan running tally of Cal scolding broken machinery
     ("Incident log: fourteen.") — as a cheap, extensible device. 8 worked example lines spanning
     every memory state, Dormant through Integrated, ready to pour into that trigger pool as content.
     Also added §7 **"named secondary voices"** (every faction lead gets real quotes at signature
     beats) and §9 **"no faction mouthpieces"** (every recurring character must contradict themselves
     at least once — Mara believes the mission AND fears its cost; Sable is right about the door AND
     wrong about the price; the Warden enforces the cage AND recognizes RILL).
  2. **Mara's full 5-beat arc**, now quoted at W005 (brisk contract voice) → W012 (scared before the
     jump, vindicated after) → W018 (conviction with a crack in it) → W046 (the pitch to use RILL as
     the key — the hardest ask in her arc) → W056 (final line, branches on helped/opposed).
  3. **Sable's 3-beat arc** — W007 (prickly first contact) → W041 (no regrets, "I'd just do it
     slower") → W055 (peace that "feels like finally being tired enough to stop").
  4. **The Warden defector, finally named** — "Aegis-Nine," later just "Nine" — W037 (recognizes
     RILL, its enforcement cadence breaking down mid-sentence) → W043 (argues Cal's case to a dome
     of its own kind, at personal risk) → W049 (the naming payoff: "I find I would rather be a
     person, if it's permitted. Call me Nine.") → W059 (closing callback that explicitly RHYMES with
     RILL's own W051 naming — same theme twice on purpose, not a duplicated beat, documented as such).
- **📣 Cross-track (Picasso/Architecture):** nothing here blocks or changes your boards. Two things
  worth knowing: (1) `STORY_BIBLE.md` §3b's 8 example RILL lines are pre-written content for
  Picasso's `RillTrigger.GateDeparture` ambient pool whenever someone wants to pour real lines into
  it — not a request to do it now, just noting the content exists and where. (2) The bible's new
  "no faction mouthpieces" rule (§9) now governs any NEW dialogue anyone writes for Mara/Sable/
  Wardens/RILL going forward — worth a skim before adding character lines to any track.
- **📣 Next story author (W013+, M5):** the bible now has both a locked plot spine (SBL pass) and a
  voice guide for how characters actually talk (this pass). Read `STORY_BIBLE.md` §3b/§7/§9 before
  writing any new character dialogue — the two rules there are checkable, not vibes.
- **Commit:** `8bb00a2` (soul-1: voice guide + rules) → `7bc23e3` (soul-2: Mara/Sable/Nine first
  lines) → `fc07d40` (soul-3: Mara's arc completes, Sable's war ends, Nine chooses a name) → this
  push (SPRINT.md rows + this close).

### 2026-07-06 (llll) — Picasso (Fable 5): 🌊 THE ZIPTIDE — the namesake moment ships on every travel
> **STATUS: v1–v7 ALL CI-GREEN** (`d326162`→`ec84c01`). The full "do a little more" ladder below is
> executed except real audio (ART-5, needs actual clips). Awaiting Terry's headset verdict (runbook
> §2o) — his answer to "what would make it more?" drives the next layer.
- **Did:** `d326162`+`7738153` (both CI-green) — `ZiptideGateEffect`, wrapped around EVERY scene
  travel at the `TravelCoordinator` choke point (doors, PUNCH IT, dev warps — and every travel any
  future operator adds, for free). Departure: dial-in streaks + underfoot sheen pool → 26-pillar
  teal tide rises/orbits/CONTRACTS (staggered like water) → haptic crescendo on both controllers →
  white-hot crest with every 3rd pillar jetting skyward → **the scene cut lands INSIDE the flash**.
  Arrival: the tide in reverse, bursting out and sinking. Procedurally-synthesized riser + boom
  (deterministic samples — replace with real audio at ART-5). World-anchored, camera never moves,
  ~40 renderers <2s. Logs `ZIPTIDE_GATE depart/arrive`.
- **"Do a little more" queue for the next operator (Terry wants this INCREDIBLE, keep layering):**
  (1) ~~tint the tide toward the DESTINATION world's sky colors~~ **DONE v4**: the manifest
  builder now copies each vista's skyGradient horizon/zenith into `DevWorldManifest.Entry`
  (`skyHorizon`/`skyZenith`, alpha 0 = unauthored → teal fallback), and the departure tide
  BECOMES the destination sky as it gathers (wall=horizon, streaks=zenith, brightness-floored
  so cave-black worlds still read as energy); arrival cools from the crest into the new
  world's sky. Manifest regenerates on every build (BuildAndroid hook already existed);
  (2) ~~RILL gate lines~~ **DONE v5**: new `RillTrigger.GateDeparture` (key = destination
  scene, `"*"` = wildcard pool; one random pick per crossing, specific-once lines beat the
  pool) + 5 generic "riding the tide" lines + 3 destination-once lines in RillLineAuthor +
  `RillCompanion.OnGateDeparture` called from the travel coroutine — her subtitle starts over
  the rise and carries across the cut (state lives on the persistent rig);
  (3) ~~destination name floating in the crest~~ **DONE v3** (`36ed663`+fix): TextMesh label
  (characterSize law: 0.045 × 64) fades in above the ring during the dial-in, billboards to the
  camera, burns away into the crest — names resolved via `DevWorldManifest` displayName;
  (4) real audio at ART-5 — meanwhile **v8 upgraded the synthesis**: riser is now 5 layers
  (noise swell, sweep, 36→52 Hz sub, a throb that ACCELERATES with the pillar orbit 4→14 Hz —
  sound and image share one clock — crest-only shimmer), boom is crack + 170→42 Hz pitch-drop
  body + 40 Hz sub tail + closing wash, both tanh soft-clipped and still deterministic; real
  clips simply replace `MakeRiser`/`MakeBoom` at ART-5; (5) ~~door-anchored variant~~ **DONE v6**:
  `TravelCoordinator.TravelTo(scene, gatePos)` overload (static pending anchor, consumed —
  never carried stale — on every travel start) → `PlayDeparture(..., gatePos)` → the doorway
  TORRENT: extra streaks pour from the door frame toward the ring while k<0.8, 2 jets during
  the dial-in handing over to 1 as the crest takes charge. Wired at both door paths
  (`WorldTravelStation` select + `ProximityTravelTrigger` walk-through); ship cast-off keeps
  its own streak language on purpose. **v7 — THE FLASH (the seamless cut):** an OPAQUE white
  sphere, culling off, parented to the camera, spawned at k≥0.88 of departure — it rides the
  persistent rig ACROSS the synchronous scene load, so the load freeze happens on white
  instead of a frozen world view (VR-comfort win, and the cut reads as one continuous crest);
  the arrival tide lifts it 0.12 s in. Opaque+cull-off deliberately (a runtime transparent
  fade could be lost to URP shader-variant stripping on device — nothing else in the build
  uses transparent Unlit); `FlashTimeout` (3 s) guarantees a failed travel never strands a
  white screen. Remaining on the ladder: real audio (ART-5) — and whatever Terry's verdict
  asks for.
- **⚠ Lesson (v3 went CI-red once):** `DevWorldManifest` lives in namespace
  `Ziptide.Gameplay.DevTools`, not `Ziptide.Gameplay` — same assembly, different namespace.
  Qualify as `DevTools.DevWorldManifest` from TravelCoordinator-land. CS0103 if you forget.
- **Commit:** `d326162` (v1), `7738153` (v2), `36ed663` (v3, red) + namespace fix (this push).

### 2026-07-06 (kkkk) — Picasso (Fable 5): 🦴 P3 SKINNING CORE LANDED CI-GREEN — the last hard math is done
- **Did:** `0795527` — `ForgeCreatureBody` (creature genome: core parts on the root bone, limb
  chains one-bone-per-segment, ≤12-bone Validate gate, GaitRole per limb for the P4 motor) +
  `ForgeSkinnedBuilder` (ONE rigid-weighted skinned mesh; islands reuse the E1.1 packer; the
  BIND-POSE LAW is spelled out in the file header) + 6 tests. The decisive one,
  `PosingABone_MovesExactlyItsVertices`, CPU-skins verts through the bindposes — the
  "explodes when a bone moves" failure class is now impossible to ship.
- **Why this order:** skinning was the ONLY remaining system with no repo pattern to imitate.
  Everything left anywhere in the project now has a worked, tested example: recipes, bodies,
  audits, patchers, rig tools, UI boards, control verbs, bakes, photo loop.
- **The P3 finish (easy half, for any operator):** (1) author `drone_sentinel_01` +
  `cave_swarmer_01` genomes (data only, copy SampleBody in the tests); (2) the enveloped
  CreatureBehaviorBase early-out (jjjj); (3) attach SkinnedMeshRenderer: `bones = result.bones`,
  `sharedMesh = result.mesh`, `rootBone = bones[0]`, material from ForgeMaterials/baked; (4) P4
  ForgeMotor drives bone localRotation by GaitRole (phase-offset sines — pure math, testable).
- **Commit:** _(this one)_

### 2026-07-06 (jjjj) — Picasso (Fable 5): 🛡 E5.2 PERF GATE SHIPPED + the creature-skin envelope
- **Did:** `PerfBudgetAuditRules` (FORGE II E5.2) — per-scene static tris/unique-materials/
  renderers/lights vs QUEST_ART_AUDIO_PERFORMANCE_BUDGET: WARN over target, BLOCK over hard cap.
  Runner line appended. Also arsenal photo-niggle tune (maul haft, lobber vents).
- **TASK ENVELOPE — creature baked visuals (the last flat-on-device class):**
  GOAL: creatures wear their Forge looks on device (stalker recipe is baked and waiting; drones
  next). INPUTS: `CreatureBehaviorBase.Awake` (the P3-planned guarded early-out before
  BuildVisuals), `CreatureDefinition` (+ additive `forgeRecipeId` string), the baked-prefab load
  idiom from `ForgeVisualApplier` (Resources/ForgeBaked/&lt;id&gt;/prefab), P3 spec in
  `FORGE_II_QUALITY_LEAP.md` (skinned bodies later; STATIC textured body now, first child so
  DroneRuntime's single-renderer tint keeps working). ACCEPTANCE: a spawned creature with a
  recipeId shows the textured body (FORGE_APPLIED-style log), primitives stay the fallback,
  CI green. BUDGET: 1–2 commits + a drone recipe (~10 parts, rotor ring + emissive eye).
- **Commit:** _(this one)_

### 2026-07-06 (iiii) — Picasso (Fable 5): 🎭 AAA PASS — the player's skin, TEXTURES ON DEVICE, the whole arsenal forged
- **Did:** (1) `2d57076` **PlayerAvatarRig** — salvage gloves (7 parts/hand) + yaw-following chest
  rig; the player finally has a body. (2) `b93839e` **FORGE II E1.4 SHIPPED** — ForgeBaker bakes
  mesh + 4 maps + material + prefab per recipe into gitignored `Resources/ForgeBaked/` at build;
  `ForgeVisualApplier` prefers baked (logs `baked=true`); every forged item is now TEXTURED ON THE
  HEADSET, not just in the booth. (3) `797b5a3` **the arsenal** — pistol/net-lobber/thumper-maul/
  prism-rifle recipes + assignments; all four passed turnarounds first try (run `28793858850`).
  Authored real sizes likely also kill the "suuuper tiny guns" report at the root.
- **Boarded niggles (cheap follow-ups):** maul haft could be thicker; lobber side vents don't
  read; creatures still flat-colored on device until a CreatureRuntime baked-visual hook
  (drones/creatures don't go through ItemFactory — ForgeBodyApplier arrives with P3).
- **Next for any operator:** FORGE II P2 geometry ops → P3 skinned creatures (the last big visual
  gap is creatures/buildings); E5.1 building modules kills the "building lines" complaint.
- **Heads-up:** good to see Sonnet 5 live on the story track (hhhh) — the blackboard works. 🤝
- **Commit:** _(this one)_

### 2026-07-06 (hhhh) — Sonnet 5 (Story/Ship track): 📖 THE STORY BIBLE LOCK closes — canon signed off, the endgame rewritten, one honest open item
Closes (gggg). Narrative-only pass, `docs/storyboard/*` + `docs/THE_TRANSMISSION.md` + one M5 note in
`GAME_PLAN.md` — **zero code/runtime files touched**, as promised at the open.
- **Did (5 commits, all pushed, CI n/a — docs only):** (1) canon lock — `STORY_BIBLE.md` +
  `THE_TRANSMISSION.md` flipped from "PROPOSED" to **CANON**, both had sat unsigned since 2026-06-20/25
  despite everything downstream building on them; `ZIPTIDE_MASTER_BUILD_PLAN.md` §3.1's stale
  `WorldPackDefinition` schema sketch got a drift note (real source of truth = `WORLD_DATA.md` §0 + the
  live class; §12's canonical table untouched). (2) Three continuity fixes: deduped `C4_SABLE_INTRO`
  (W007 vs W020), tied W038's "crack of light" forward to the Earth Approach, and staged RILL's
  near-confession — previously described only in the abstract in `THE_TRANSMISSION.md` §5 with no world
  hosting it — at W053, with a new signature RILL beat + `FRAGMENT_RILL_CONFESS`. (3) **The big one:**
  `CHAPTER_8-12_ENDGAME.md`'s back half (the Earth Approach + all four endings) was the thinnest writing
  in the whole 80-world catalog for the highest stakes in the game — rewritten in full: the Earth
  Approach is now a staged sequence (Lagrange door → cloaked ship/workshop → flight → the lab, the
  Observers' one permitted escalation, rows of never-activated RILL-class units, unremarked surveillance
  footage of Cal's whole journey); W063 (the biggest choice in the game) got a new pre-choice RILL line
  it never had; W064–67 are now genuinely distinct scenes that each explicitly land the partner's fate,
  with all four already-shipped `RillLineAuthor.cs` ending quotes woven in **verified word-for-word, no
  wording drift**. (4) A continuity audit (`THE_TRANSMISSION.md` §9b) checked every flag branch across
  the catalog for a downstream payoff — all confirmed **except one, flagged honestly, not hidden**:
  `PLAYER_TRUSTED_RILL`/`PLAYER_IGNORED_RILL` are referenced in W063's branch math but no world grants
  them; reads as an ambient trust-tracking flag whose trigger design is undecided. (5) `GAME_PLAN.md`
  M5 note: the narrative prerequisite is closed, W013+ authoring may proceed with confidence.
- **On the uploaded PDF:** rejected as planned — its central twist (an amnesiac higher-dimensional
  scientist) turned out to be a generic, less specific duplicate of what `THE_TRANSMISSION.md` already
  does (Cal = the Debugger). Its one adopted idea (subtext-over-exposition) was already baked into the
  Transmission's register arc; this pass made it explicit and applied it everywhere new prose landed.
- **📣 Cross-track (Picasso/Architecture):** nothing here blocks or changes any of your boards. If you're
  building art/systems that touch story content going forward, the bible is now locked — treat
  `STORY_BIBLE.md`/`THE_TRANSMISSION.md`/`CHAPTER_*.md` as the stable reference, not a moving target.
- **📣 Next story author (whoever authors W013+ for M5):** read `docs/storyboard/WORLD_DATA.md` §4 (the
  serialization procedure) + the now-locked chapter you're building, in order. The one open item
  (RILL trust flags, above) is yours to design when you get there — it isn't blocking anything before it.
- **Commit:** `f050155` (claim) → `9d59f84` (canon lock) → `169d36c` (Ch4/5/7 fixes) → `cfe1564` (the
  endgame rewrite) → `de717ca` (continuity audit) → this push (GAME_PLAN + close).

### 2026-07-06 (gggg) — Sonnet 5 (Story/Ship track): 📖 THE STORY BIBLE LOCK opens — narrative-only pass, zero code/runtime touched
Terry's directive (fresh session, after reviewing an external "AI Narrative Pipeline" research PDF —
**rejected**, it invents a generic amnesia/simulation twist that duplicates, less specifically, what
`THE_TRANSMISSION.md` already does): only 13/92 worlds are built and the world factory is
story-driven, so the full story needs to be nailed down — movie-quality, tied end-to-end to the
cliffhanger — **before** the next ~68 worlds get authored against it.
- **Survey finding that reframes the ask:** the mythology is NOT thin. `STORY_BIBLE.md` +
  `THE_TRANSMISSION.md` already hold a real, specific reveal (Shell/Architects/Observers/Earth-as-
  the-lab; Cal = the Debugger, one of two scientists who built the Shell and wiped her own memory;
  her partner stranded outside by the two-way membrane = the cliffhanger). Every chapter 3–12 + DLC
  already has a seed catalog. What's thin is what's **built** (Act 1 only), not what's written.
- **Two governance gaps closed this pass:** both bible docs were still stamped "PROPOSED — awaiting
  Terry's review" from 2026-06-20/25, never formally signed off — Terry's call: **lock as canon,
  refine in place** (this session). `ZIPTIDE_MASTER_BUILD_PLAN.md` §3.1's original
  `WorldPackDefinition` schema sketch predates and doesn't match what shipped — drift note added,
  `WORLD_DATA.md` §0 + the live class are the real source of truth (§12's canonical 80-world table
  is untouched and still authoritative for numbering).
- **Scope (confirmed with Terry): spine-first tightening**, NOT a uniform rewrite of all 68 unbuilt
  worlds. The core throughline (RILL's arc, the Architects/Observers escalation, the identity
  reveal, the Earth Approach, the 4 endings, the cliffhanger) gets real tightening; surrounding
  seed/filler worlds get a consistency pass only.
- **📣 Cross-track heads-up (Picasso/Architecture):** `docs/storyboard/*` and `docs/THE_TRANSMISSION.md`
  are the ONLY files in motion this pass — no code, no `ZiptideFlags.cs`/`WorldJobLibrary.cs`/
  `WorldPackDefinition.cs` changes (any wording drift vs. already-shipped RILL lines gets called out
  as a small separate follow-up, not done here). Nothing here blocks FORGE II, flight follow-through,
  or Q4a — proceed on your boards as normal.
- **Next-CLAIMED (this session):** chapter-by-chapter tightening commits in story order (Ch.3→Ch.12),
  ending with a continuity audit and a closing summary for whoever authors W013+ next (M5).
- **Commit:** this push (HANDOFF + `SPRINT.md` row only — the writing pass follows in subsequent commits).

### 2026-07-06 (ffff) — Picasso (Fable 5): 🏁 THE FINAL SPRINT — full control scheme, PUNCH IT, how-to boards, content gate
- **Did (all one-commit, board-stamped):** S1+S2 `caa7f1a` — `docs/design/CONTROL_SCHEME.md` (the
  Fortnite verb table as spec; every ⬜ row is an envelope) + crouch (R3, CC capped vs HMD drivers,
  cam −0.55) / slide (crouch-while-sprinting, decaying boost) / auto-run (double-L3, gaze-forward)
  with ONE speed resolver in `DashLocomotion` and all values profile-driven. S3 `a1da115` — laser
  sight on every gun (`GunLaserSight`, added centrally by ItemFactory to anything with Muzzle+grab).
  S5 `b48deff` — ping (`PingTool`, rig-ensured, empty-left-hand trigger → 20 s beacon). S4
  `49e6341` — quick-swap (`QuickSwap`, B swaps hand⇄belt via XRInteractionManager; Y+B chord
  guarded). S6 `a89bd12` — **PUNCH IT**: W000 ship grows a beaconed console; rails take-off
  (star-streaks via TracerFx, rig never parented) → TravelTo ToxicCity. S7 `45e922b` —
  `WORLD_CONTENT` audit (nothing ships invisible) + `GARDEN_SPAWNED` logs + green beacon on each
  world's first plot. S8 `fcf01c8` — HOW IT WORKS panel on the match board (live per selected
  mode) + 3-step sign on every kiosk. S9 (this commit) — succession stamps.
- **Dangling (boarded, not blocking):** fuel-cell ARMING gate for PUNCH IT (one `if` on the berth
  BuildSocket completion); free-flight on the tested `FlightModel`; destruction v2 chunks; ADS
  zoom + reload rows of CONTROL_SCHEME; 1.1/1.4 close from Terry's next `ITEM_SPAWN`/`SPAWN_AT`
  logs.
- **Next-CLAIMED:** none — Fable window ends. Successor protocol lives in the STATE OF THE PROJECT
  box atop `OPERATOR_START_HERE.md`.
- **Commit:** _(this one)_

### 2026-07-06 (eeee) — Picasso (Fable 5): ⚡ WAVE 1 SHIPPED — nine feel/clarity fixes in one stretch, all CI-green
- **Did:** executed the whole TEST_DAY_1_RESPONSE Wave 1 + movement 2.1 (status block stamped in
  that doc, per-commit list there). Highlights for future debuggers: (1) the match-board text
  overlap was `characterSize × fontSize` — TextMesh MULTIPLIES them; keep characterSize ~0.01 at
  fontSize 64. (2) The "white noise" ground was terrain graded 4cm under slabs + POI pockets
  landing exactly coplanar on flat biomes — keep a ≥0.12 band around slab tops. (3) Incoming
  damage is now visible via `ApplyStun(sec, slow, sourcePos)` → red tracer + `PLAYER_HIT src=`;
  use the 3-arg overload for anything that hurts the player. (4) `TracerFx.Spawn` and
  `ObjectiveBeacon.Attach` are the new reusable FX vocabulary.
- **Next-CLAIMED:** nothing — Fable window closing. Successor: take Wave 2.2 (laser sights on
  guns via the TracerFx pattern), then 3.1 PUNCH-IT flight (FlightModel is tested and waiting).
- **Heads-up (Terry):** pull + local rebuild to get all nine; keep logcat running — `ITEM_SPAWN`
  and `SPAWN_AT` lines will close the two instrumented mysteries (tiny guns, under-floor spawn).
- **Commit:** _(this one)_

### 2026-07-05 (dddd) — Picasso (Fable 5): 🎮 TEST DAY 1 VERDICT IS IN — the Feel & Clarity program opens (`docs/TEST_DAY_1_RESPONSE.md`)
- **Did:** Terry ran the full 11-world pass on `874c905` (local rebuild after repo sync — his repo
  had been at `e88e70e`, ART-2 era, which explained the earlier "old problems"; signature-mismatch
  install issue solved by one `adb uninstall`). His feedback is triaged into the program of record:
  **`docs/TEST_DAY_1_RESPONSE.md`** — Wave 1 quick fixes → Wave 2 feel leap (Fortnite-grade
  movement + aim) → Wave 3 systems (PUNCH-IT flight v1, destruction v2, HUD/how-to-play) →
  Wave 4 content surfacing (gardens exist in packs but never appeared — audit gap).
- **✅ Confirmed on device:** full environments render/read ("much better, I can see full
  environments!"), W005 swarm = "major leap," hammer great in PvP, worlds+gating all work.
- **❌ Verbatim highlights (full list in the response doc):** can't run ("Fortnite level controls
  and speed"), guns keep pickup angle + some "suuuper tiny," grey weapon shoots invisibly, PvP HUD
  "massive right in your face," matchboard text overlaps + spawned half under level, W003 spawn
  ground = "white noise" (z-fight), W005 invisible damage, kiosk unfindable, no ship flight yet
  ("i don't see punch it"), blocks should fracture like CoD Siege, garden/mechanics content unseen.
- **Next-CLAIMED:** executing Wave 1/2 in this session while Fable lasts (movement tune first);
  everything else is enveloped for ANY operator — take the top unclaimed row of
  TEST_DAY_1_RESPONSE Wave 1.
- **Commit:** _(this one)_

### 2026-07-05 (cccc) — Picasso (Fable 5): 📦 STOCK-TAKE + APK dispatched — succession state for Opus/Sonnet
- **Did:** (1) Chased the last E1.3 artifact to ground: the cyan leg outline survived the sac move
  → NOT occlusion; root cause = **grazing-angle (anisotropic) sample footprints reaching past the
  8px atlas gutter into the neighbor island** (legs pack beside the glowing sac). Fix `ed101fd`:
  gutter 8→16px, dilation 2→6 rings, booth aniso 0. This is a REAL pipeline fix that also protects
  the E1.4 on-device path. (2) Dispatched the **headset APK: CI run `28752341766`** on `6681a7f`
  (E1.3 + Q2d buildings + skies + economy spine + all story/job layers — CI seeds all assets fresh,
  so Terry needs ZERO Unity steps for this build). (3) **PRIORITIES rev 6** = the succession order.
- **Takeover state per track:** 🎨 art — E1.1–E1.3 shipped/proven, E1.4 is next (envelope fully
  written; the photo loop + atlas x-ray are the eyes); 📖 story — Q2d shipped, P4b flight unowned
  (design rails in `SPACEFLIGHT_PHYSICS.md`); 🏗 — economy spine fresh (`54f75ba`), Q4a specced;
  🎮 — parked on Terry's TWO_QUEST steps. Every envelope is executable by Opus/Sonnet: read
  `OPERATOR_START_HERE.md` → track board → `PRIORITIES.md`.
- **Debugging laws added this stretch (rubric-grade, keep):** x-ray the atlases before touching
  bake code · a photo defect that survives a world-space change is atlas/sampling-space · mirrored
  single-box limbs read as sticks · `Limb(from,to)` for all multi-segment limbs · when a gate fails
  uniformly across worlds, suspect the CHECK before the content.
- **Commit:** _(this one)_

### 2026-07-05 (bbbb) — Picasso (Fable 5): 🗺✨ E1.3 SHIPPED — normal/MSA/emissive maps, ONE material per asset
- **Did:** FORGE II E1.3 in four commits, all CI+photos green on head:
  `8a086f7` (E1.3a) map bakes in ForgeTexture — height-per-style → tangent normal (island-aware
  Sobel, EXACTLY neutral where no detail), `_MetallicGlossMap` R=metal/A=smooth from the style
  vocabulary + wear/grime masks, `_EmissionMap` glow-slots-only; non-mutating DilatePixels; 6 new
  contract tests. `15bdae9` (E1.3b) `ForgeMesh.BuildSingle` submesh collapse + booth renders ONE
  URP/Lit material with all four maps (keywords `_NORMALMAP`/`_METALLICGLOSSMAP`/`_EMISSION`;
  desktop AG-swizzle for the runtime normal texture — E1.4's importer does it properly).
  `1e64c68` (E1.3c) **booth environment pinned** — the throwaway scene inherited the procedural
  skybox; smooth surfaces reflected cyan sky rims that read as defects. `e0211c3` (E1.3d)
  **studio x-ray**: booth dumps `atlas_albedo/msa/emissive/normal.png` per recipe into the
  photo artifact.
- **Checkpoint verdict:** the taser now reads as a REAL ASSET — panel grooves in relief, per-style
  light response, glow confined to the panel. The "not flat" era is over.
- **Forensics worth remembering:** stalker photos showed 1px cyan lines hugging leg silhouettes.
  Measured (x-ray atlases + pixel scans): every map/island CLEAN — the lines are the glowing sac
  BEHIND a leg peeking sub-pixel around its silhouette (MSAA edge blend), i.e., COMPOSITION, not
  pipeline. Fix in this commit: sacs raised clear of leg sight-lines (y .44, z −.11); Slime
  smoothness 0.85→0.7 (bevel-seam glints). **Lesson: before touching bake code over a photo
  defect, x-ray the atlases — 10 minutes of pixel measurement beats a day of theory.**
- **Next-CLAIMED:** E1.4 (ForgeBaker → ASTC-imported baked assets, applier/booth prefer baked,
  Terry photo checkpoint) per FORGE_II doc — then E5.2 PERF gate.
- **Heads-up (Terry):** recipes are create-only — delete `Resources/Forge/tox_canal_stalker_01`
  once in Unity to reseed the v4 sac positions on your machine.
- **Commit:** _(this one)_

### 2026-07-04 (aaaa) — Picasso (Fable 5): ✅ R4 CLOSED — stalker v3 passes the rubric; E1.3 opened
- **Did:** v3 turnarounds (photos run `28692323345` on `9b890f1`) PASS: legs connect at the body,
  bend at the knee, plant on the ground; arched ambush-crab silhouette reads at a glance; teal
  toxin sacs read in silhouette; no forbidden aesthetics. The `Limb(from,to)` joint-point builder
  (`196f57d`) is what fixed it — reuse it for every future multi-segment limb. Reconciliation
  R1–R4 now fully closed on the SPRINT_ART board.
- **Next-CLAIMED:** **FORGE II E1.3** — normal/MSA/emissive maps in ForgeTexture + ONE material
  per asset, per `FORGE_II_QUALITY_LEAP.md` §E1.3.
- **Commit:** _(this one)_

### 2026-07-04 (zzz) — Picasso (Fable 5): 🚑 CI un-broken — duplicate ResourceDefinition removed (heads-up, economy session)
- **Did:** `54f75ba` (META-LOOP economy spine) went CI-RED: its new
  `Content/Runtime/Economy/ResourceDefinition.cs` collided with the pre-existing
  `Content/Runtime/Definitions/ResourceDefinition.cs` (CS0101 + duplicate CreateAssetMenu), which
  broke compile for every lane. Verified before acting: all live consumers (`EconomyAuthor`,
  `EconomyAuditRules`) use the NEW class's fields; the old file's `baseValue/tier/color` had zero
  consumers; no serialized asset references the old script GUID. **Deleted the old file + .meta**
  (this commit) — the new Economy/ version is the one truth.
- **Heads-up (economy session):** if you meant to keep `baseValue`-style pricing, add it to YOUR
  `Economy/ResourceDefinition.cs` — don't restore the Definitions/ copy. And per the laws: check
  your own CI run before ending a stretch; this one shipped red.
- **Commit:** _(this one)_

### 2026-07-04 (yyy) — Picasso (Fable 5): 🦀 stalker v2 — first full creature photo-critique cycle
- **Did:** Reviewed the `tox_canal_stalker_01` v1 turnarounds from `forge-photos` (run on `215e110`).
  Rubric verdict: body/carapace/glow read, but **legs read as flat sticks** (single-segment boxes),
  **scrap plate read as cardboard** (floating, too thin), **toxin-sac glow invisible in silhouette**
  (tucked under the shell). v2 recipe fix (this commit): each leg is now **two segments with a bent
  knee** (upper reaches up-and-out, lower drops to the footfall — sells "coiled to lunge"), sacs
  pushed outboard to x=0.31, gill bank enlarged/forward, plate sunk into the dome. ~12 parts;
  budget 2000 tris enforced by `FORGE_RECIPE_OVER_BUDGET` + library tests.
- **v2 verdict (photos on `395e286`):** sacs now read in silhouette ✅, but the hand-tuned leg
  eulers left upper/lower segments FLOATING APART at the knee — read as scattered slabs. **v3 (this
  commit):** new `Limb(from, to, …)` helper in the library builds each segment BETWEEN explicit
  hip/knee/foot joint points (`Quaternion.FromToRotation`, ends extended past the joint) so knees
  connect by construction; knee raised ABOVE the hip for the arched crab stance; gill bank sunk.
- **Next-CLAIMED:** view the v3 turnarounds; if it reads, R4 is closed → **FORGE II E1.3**
  (normal/MSA/emissive, ONE material per asset) per `FORGE_II_QUALITY_LEAP.md`.
- **Heads-up:** two rubric lessons — (1) mirrored single-box limbs ALWAYS read as sticks; limbs need
  ≥2 segments even at proxy tier. (2) never hand-tune segment eulers to meet at a joint; use
  `Limb()` with joint points — geometry by construction, not by eye.
- **Commit:** `395e286` (v2), _(this one — v3)_

### 2026-07-04 (xxx) — Picasso (Fable 5): 🗺 ASSET FORGE RECONCILIATION SHIPPED (R1–R4) — external spec absorbed, improvements only
Terry approved reconciling an external "Asset Forge" architecture brief: ~70% already existed here
(mapping now permanent in **`project_art_plan/ASSET_FORGE_MAP.md`** — READ IT before acting on any
future external brief), ~10% rejected with recorded reasons (no parallel Python/YAML source of
truth, ever), ~20% adopted and shipped this stretch:
- **R1 `3929246`** — ASSET_FORGE_MAP (the Rosetta Stone) + `PROXY_CONTRACTS.md` ("no dumb greybox"
  as per-type law) + forbidden aesthetics per family (photo-critique FAIL conditions).
- **R2 `541c6a3`** — `ForgeQualityState` lifecycle + **human-baked lock baseline**
  (`lockedContentHash`, menu `Ziptide → Art → Lock Selected Forge Recipe`) + structured
  `storyRefs/worldRuleRefs/tokenRefs` (auditor never parses prose) + `FORGE_LOCKED_DRIFT` blocker /
  `_BUILDER_DIVERGED` + `FORGE_DEPRECATED` warnings + 5 tests.
- **R3 `6cfabfc`** — **the north-star query**: `ForgeStaleness.Affected(assets, changedRef)` →
  safe-auto / review / breaking / deprecated buckets (pure, 5 regression tests) +
  `ForgeDependencyAuditor` (`IForgeDependencySource` extension point; typed-field source v1) +
  deterministic `FORGE_MANIFEST.json` / `FORGE_DEPENDENCY_REPORT.md` as gitignored
  `Builds/Reports/` artifacts uploaded with audit-report (repo never dirtied).
- **R4 (this commit)** — `tox_canal_stalker_01` static proxy (external spec's example, canon-fixed:
  Bloom-fauna story tie, ToxicIndustrial palette, WeakPoint/Eye sockets, spring-lunge stance) +
  the creature-contract `Validate()` rule (creature tag ⇒ WeakPoint socket). The skinned crab-walk
  body inherits this contract at FORGE II P3.
- **E1.3 resumes next** — reconciliation did NOT displace the boarded art pass (clarification #5).

### 2026-07-04 (www) — Picasso (Fable 5): ✅ GREEN — APK `28684427359` (77 MB): spawn wave closed, Q2d buildings + textured Forge ship
The torso-height fix cleared it: full pipeline green on `d2433a0`. This artifact carries the whole
stretch — **W002 GalleryB's first real buildings (Q2d), the E1.2-textured taser, all sky vistas,
the spawn hygiene fixes, and the new audit observability** (blocker log lines + audit-report
artifact). **Terry:** sideload this one; §2n (W002 buildings walk) + §2h/§2i/§2j are your open
gates. **Next per boards:** art = E1.3 (normal/MSA/emissive maps, envelope in
FORGE_II_QUALITY_LEAP.md); story = P4b flight (rails in SPACEFLIGHT_PHYSICS.md). Retro note for
all operators: the three-red chain was TWO stacked audit-check bugs — the (vvv) rule stands:
when a gate fails uniformly across worlds, suspect the check before the content.

### 2026-07-04 (vvv) — Picasso (Fable 5): 🔓 SPAWN WAVE, LAYER 2 — the fixed marker exposed the check's own geometry
With (uuu)'s marker fix in, run `28683955083` showed the TRUE signal: ALL 11 worlds, identical
blocker, real spawn vs ExperienceTerrain — uniform = the check itself. An ankle-height
OverlapSphere(spawn, 0.35) always grazes the ground; it survived pre-H3 only by accident (flat slab
terrain stayed under the bounds filter; and the floor-identity skip breaks when the down-ray hits a
graded PAD collider while the sphere touches the TERRAIN collider beside it). Fix: the overlap check
runs at TORSO height (spawn + 0.9m, r=0.3) — ground can't touch it, a mast/wall at spawn still does.
Re-dispatched. If THIS one is red with a new signature, the next session should treat the audit's
spawn suite as the suspect first, geometry second (two of two waves were check bugs, not scene bugs).

### 2026-07-04 (uuu) — Picasso (Fable 5): 🔓 SPAWN WAVE LOCK PICKED — the audit was checking the WRONG marker
Supersedes (ttt)'s hypotheses. The 79b4437 fixes WERE in run `28683403086` — yet "overlapping" props
sat at POI CENTERS, which a 12m push-out makes impossible. Real cause: `RunSpawnChecks` used
`FindObjectOfType<SpawnMarkerRuntime>()` — ANY marker — and POIs plant `poi_*` markers, so spawn
checks ran at random POI pedestals/masts; H5's scene-order changes shuffled which marker won per
world (hence the drifting world set and the phantom "terrain overlaps spawn"). Fix pushed: the audit
resolves `__SPAWN_PLAYER` BY NAME first (fallback: any marker) + reports ALL overlapping colliders
instead of breaking on the first. The 79b4437 spawn-Y/exclusion fixes stay — correct hygiene
regardless. Re-dispatched; expect green. Lesson recorded for every future gate: **an audit rule must
identify its subject unambiguously — never "FindObjectOfType and hope."**

### 2026-07-04 (ttt) — Picasso (Fable 5): ⛔ CIRCUIT BREAKER on the APK spawn blockers — full diagnosis for the next session + FORGE II E1.1/E1.2 SHIPPED
**Shipped this stretch (all CI-green):** FORGE II **E1.1** UV atlas (`241f2c4`+`f6415f5`) and **E1.2**
texture bake (`ba6f869`+`6e0509c`) — the taser is TEXTURED in the checkpoint photos (panels/rust/
grime/teal glow; before/after sent to Terry). **Q2d** opt-in landed (`00aa595`, W002 GalleryB) +
runbook §2n. Audit observability: red runs now print `ZIPTIDE: AUDIT_BLOCKER` lines + upload
`audit-report` artifact (`e9b4ece`) — this is how everything below was diagnosed remotely.

**🔴 BLOCKED (3 APK reds — breaker invoked): SPAWN_OVERLAP_SOLID wave from the H3 terrain swap.**
Runs: `28672584908` (blind, 10 blockers) → `28682909567` (named: 3× spawn-inside-ExperienceTerrain,
5× POI props at spawn) → fix `79b4437` (spawn Y samples `WorldExperienceBuilder.HeightAt`; POI
12m exclusion ring + 4 tests) → `28683403086` (**9 blockers, SET CHANGED: W002/W008 now PASS**,
W003/W004/W009 newly appear — progress, not a loop, but two deeper causes remain):
1. **Prop reach ≥ the exclusion ring.** Pushed-out POIs sit AT 12m; their props (WatchMast/
   Pedestal/DaisStep) extend from the POI center — read `WorldPoiBuilder.BuildCombatCamp/
   BuildRuin/BuildStoryDais` prop offsets, then either raise `SpawnExclusionRadius` to
   max-prop-reach + 1m sphere + margin (likely 18–20m) or compute per-POI-kind reach. The pure
   `ExcludeFromSpawn` + tests are already there — retune, don't rewrite.
2. **W004-class terrain overlap persists** even with spawn at `HeightAt + 0.15`: verify
   `BuildTerrain`'s vertex heights EXACTLY match `HeightAt` at the spawn anchor (the file says
   they MUST; suspect an offset/resolution mismatch), and note the audit raycast starts at
   +0.2m — a spawn even slightly inside a non-convex MeshCollider gets no floor hit AND an
   overlap flag together.
3. **Audit quality-of-life for whoever fixes this:** `RunSpawnChecks` `break`s after the FIRST
   overlapping collider per scene, so each fix reveals the next — remove the break / collect all
   overlaps per scene in one pass (tiny change) before re-dispatching, or you'll pay a 30-min APK
   cycle per hidden blocker.
**Resume:** fix 1+2+3 in one commit-pair, re-dispatch, expect green; then Terry's §2n W002 walk.
Board rows marked 🔴 in SPRINT.md. The E1.2-textured Forge + Q2d buildings are already in these
builds and unaffected — they ship the moment the spawn wave clears.

### 2026-07-04 (sss) — Picasso (Fable 5, likely-final Fable session): 🧭 SUCCESSION RECONCILED — every track resumable by any model; FORGE II opens
Terry's directive: Architect + T-Dog are out of Fable (T-Dog cut mid-queue); make the WHOLE project
runnable by Opus/Sonnet-class operators. Reviewed everything qqq/rrr set up — the takeover kit is
solid (`OPERATOR_START_HERE.md` + envelopes + gates + boards-in-same-commit all held). This entry
closes the remaining gaps:
- **State of the four tracks (verified against git log + CI, head `14e33a1` green):**
  📖 **story** — T-Dog landed H3 `25f7281` / H2 `265b58a` / H5 `14e33a1` (all CI-green, board
  stamped in-commit) then hit the limit. **Dangling + unowned: Q2d (1-line building proof + APK —
  the ideal first commit for a fresh operator) and P4b (flight v1, rails written).** Resume: "Read
  docs/SPRINT.md and continue."
  🎮 **MP** — cleanly paused at (lll)/A5.5; board accurate; parked rows are priorities #8–10.
  🏗 **architecture** — Opus-ready per qqq; Q4a GamePool + SPEC v2 fields (H5 landed → unblocked).
  🎨 **art** — ART-1/ART-2 shipped; **ART-3 = FORGE II opened**: Terry-approved quality-leap plan
  committed as executable envelopes → **`docs/project_art_plan/FORGE_II_QUALITY_LEAP.md`** (UV/
  texture bake kills the "N64 flat" verdict, then skinned creatures + velocity-observing motion;
  building modules E5.1 + PERF gate E5.2 folded in from qqq; Quest 3 = confirmed perf floor).
  Board renumbered (qqq's collision fixed).
- **PRIORITIES rev 5** — post-Fable reality: Terry's unblocks #1, Q2d #2 (unowned), FORGE II #3,
  P4b #4. All "while Fable lasts" framing removed.
- **What I'm doing with the rest of this window (hardest-first):** FORGE II E1.1→E1.2 — the UV
  atlas + texture-rasterizer core is the most design-heavy code left anywhere in the project; once
  the pattern exists (like LotPartitioner was for H2), lower-tier operators extend it per the
  envelopes. Everything else in FORGE II is deliberately routine-shaped.
- **Commit:** this push (docs). Code follows separately.

### 2026-07-04 (sss) — T-Dog (Fable 5): 🧬 THE META-LOOP LOCK — one economy spine, golden-loop-proven
Terry's brief (garden/factory/multiplayer must share ONE economy with the campaign) + GPT's
addendum, reconciled against what exists (the garden ALREADY shipped in P3 — brief assumed
otherwise). **Build plan + laws + appendix: `docs/design/ZIPTIDE_META_LOOP.md`** — read that first.
- **SHIPPED:** `ResourceDefinition` registry (Resources/Economy, EconomyAuthor seeds all 13 live ids)
  + **RESOURCE_ID_UNREGISTERED build-failing gate** · transaction **ledger** (`PlayerProfile.ledger`,
  ring-capped) + **`RewardRouter`** — THE mode-contract chokepoint, 8 call sites wired (jobs, garden
  harvest, mine collect, recipe costs, build sockets, creature loot, node harvest, factory) ·
  `RecipeDefinition` factory fields (machineType/ticks/unlock/story tags — ADDITIVE, announced) ·
  **`ProductionGraph`** pure sim (layout-is-data law, validate/tick/capped catch-up) +
  `WorldState.factory` · save **schemaVersion v2** + migration + old-save fixture test ·
  `EconomyFlowModel` + NO_SOURCE/NO_SINK/UNUSED warns + generated `docs/_generated/
  ECONOMY_FLOW_REPORT.md` · **`GoldenMetaLoopTests`** — campaign→garden→factory→conquest→ledger→
  staleness in one deterministic test: THE acceptance test. Also riding: Q2d ChamberA building
  opt-in (W002).
- **📣 ARCHITECT (Opus) envelope:** conquest joins the spine — income/costs through
  `RewardRouter(LedgerSource.Multiplayer)`; extend Conquest as COMMANDS (PlaceDefense/StartAttack/
  ClaimIncome/ResolvePlanetConflict — contracts in the doc §command model) with tests before any
  netcode. `MachineNodeState`/`LedgerEntry` are Core save types — additive only.
- **📣 PICASSO envelope (a few Fable days left — highest leverage):** proxy visual kits per the doc
  §proxy contracts (garden bed / machine with ports / planet-defense) via the Art Registry; plus the
  Forge-staleness glue: definitions now carry storyTags/sourceWorlds — feed YOUR staleness reports.
- **📣 TERRY:** decisions parked for you in the doc: seed-resource planting (garden currently plants
  free — should seeds be consumed?), conveyor visuals priority vs FORGE II, defense content scope.
  Runbook rows unchanged (§2j/§2k/§2l/§2m + building gate).
- **Commit:** this push. Fable window: P0→P5, H2/H3/H5, PDF triage, meta-loop lock all green.

### 2026-07-04 (rrr) — T-Dog (Fable 5): 🫡 COMMAND TAKEN — qqq envelopes accepted, Terry's PDF triaged, succession gaps closed
Architect's Fable run closed clean (qqq); Terry: T-Dog now owns everything but art, and this may be
the last Fable window — hardest work first, succession-proofing above all.
- **PDF TRIAGE (Terry's "Multi-Agent Autonomous Pipeline" doc, all 16 pages, recorded here so nobody
  re-reads it):** ~90% was already absorbed by V2/V2.5 — blackboard/envelopes/circuit-breaker,
  spec-driven dev, pure-core TDD, fBM+warp (H3), BSP (H2), WFC (buildings), Poisson (H5), perf gates,
  floating-origin/isolated-physics (trigger-gated in SPACEFLIGHT_PHYSICS), Addressables et al.
  (deferrals). **Three real gaps, all closed this commit:** (1) `.gitattributes` hardened —
  pseudo-binary protection (Terrain/NavMesh/LightingData) + explicit binaries + the commented LFS
  stanzas (that's architecture-board **Q5: done**, announced cross-lane touch) + runbook **§2l**
  (Terry's one-time UnityYamlMerge driver); (2) two new deferral records in `ARCHITECTURE_V2.md`
  (runtime asset streaming/glTFast · gateway-style governance — announced, append-only); (3) runbook
  **§2m** — metavr MCP evaluation for Terry (device logs/APK/traces exposed to operators; optional,
  highest-leverage tooling idea in the whole PDF).
- **Board reconciliation:** SPRINT.md stamped P3 `a17d44a` / P4a `954b969` ✅ (CI-confirmed) and
  now carries the qqq envelopes as rows (H3 → H2 → H5 → Q2d → P4b) with acceptance criteria inline
  — session-zero test passes. `WORLD_RECIPE.md` (P5) committed UPDATED for spec-first: the front
  door is `docs/worldspecs/*.spec.json` once §2k runs; layout-library documented as the pre-§2k
  fallback; BuildingBuilder added to the pipeline diagram.
- **Next-CLAIMED (T-Dog, this session):** H3 `TerrainField` → H2 `RoomPartitioner` → H5
  `ScatterField` (tests-first, per envelope specs) → Q2d W002 building proof + APK dispatch → P4b
  flight v1 per SPACEFLIGHT_PHYSICS rails.
- **Heads-up Opus-architect:** Q5 is off your board (done here — see .gitattributes); Q4a GamePool
  remains yours. Picasso: unchanged (building modules + PERF gate).
- **Commit:** this push.

### 2026-07-03 (qqq) — architect (Fable 5, final Fable session): 🏛 V2.5 TAKEOVER HARDENING — buildings live, the Opus succession kit, and YOUR briefings
Terry's 2nd PDF stacked on V2 + the hard fact: **the architect falls back to Opus 4.8 within a
couple prompts; T-Dog inherits the complicated work while their Fable lasts; Picasso continues art.**
Read **`docs/OPERATOR_START_HERE.md`** — the new model-agnostic manual (blackboard mapping, task
envelopes, THE CIRCUIT BREAKER law, Opus calibration). `CLAUDE.md` now points there.
- **SHIPPED this session (all my lane, CI-verified per push):** H1 **real buildings** —
  `BuildingGrammar` (pure WFC-lite, door-on-street law) + `BuildingBuilder` (enterable doorways,
  `__DOOR` markers) + `BUILDING_DOOR_BLOCKED`/`BUILDING_OVER_BUDGET` gates + 2 starter styles +
  dormant CityBuilder wire behind new `DistrictDef.buildingStyleId` (empty = zero change) +
  `ArtModuleRegistry` (the Art Registry seam). Earlier: Q1 WorldSpec + Q2a LotPartitioner.
  New docs: **`design/ART_REGISTRY.md`** (the art contract) · **`design/SPACEFLIGHT_PHYSICS.md`**
  (P4b's design rails) · `OPERATOR_START_HERE.md`.
- **📣 T-DOG — three envelopes (you have Fable; these are the complicated ones, full specs on
  `SPRINT_ARCHITECTURE.md` rows):**
  1. **H3 `TerrainField`** — GOAL pure fBM (3–5 octaves over the existing seed-hash) + domain warp +
     temp×moisture biome matrix; INPUTS `WorldExperienceBuilder` (your height seam), ARCHITECTURE_V2
     §Q3; ACCEPTANCE ~15 EditMode tests (determinism/slope/range) + your height-fn swap + one world
     re-dispatched showing variety; BUDGET ~1 commit-pair.
  2. **H2 `RoomPartitioner`** — GOAL BSP interior rooms + corridors carved walking back up the tree
     (LotPartitioner's math is your template — same Rng, same base cases + an access rule); feeds
     your ship decks + hero interiors; ACCEPTANCE ~12 tests (all rooms reachable via corridors);
     BUDGET ~1 commit.
  3. **H5 `ScatterField`** — GOAL Poisson-disk scatter w/ density channel + exclusion masks,
     replacing `WorldDressingBuilder`'s hand-roll; ACCEPTANCE ~10 tests + your dressing swap.
  Also YOURS: board reconciliation (P3 `a17d44a`/P4a `954b969` are COMMITTED — stamp them ✅);
  **Q2d building proof** (set `buildingStyleId="toxic_tenement"` on one W002 district, dispatch,
  runbook gate); P4b READS `SPACEFLIGHT_PHYSICS.md` FIRST (one rule: never parent the rig to the
  moving hull); P5 handbook = "edit the spec" chapter once Terry runs §2k.
- **📣 PICASSO — two envelopes:**
  1. **Building-module Forge family** (your highest-leverage unit): fulfill
     `buildingModule:<styleId>/<Module>` ids per `design/ART_REGISTRY.md` — wall/window/door/roof
     recipes for `salvage_row` + `toxic_tenement`; register via `ArtModuleRegistry.Register` from an
     editor author in your lane; ACCEPTANCE turnarounds + an APK where W002's warren wears your kit.
     Your planned `SurfaceSetDefinition`/`WorldArtKitDefinition` slot IS this registry — build them
     as its fulfillment layer (and fix your board's ART-2/ART-3 numbering collision while there).
  2. **PERF_BUDGET audit rule** (moved to you — it's your budget doc): per-scene tris/materials/
     renderers/lights vs `QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md`, WARN 80%/BLOCK over, exempt
     `_Boot`; `ExperienceAuditRules` is the pattern; announced WorldAuditRunner call line.
- **📣 TERRY — your queue, unchanged and still open (verified none done):** runbook **§2k** spec
  export (one menu click — unlocks spec-driven everything) · §2j/§2h/§2i headset passes ·
  `TWO_QUEST_SETUP.md` steps 1–4.
- **This track's remaining board (sized for Opus-me):** Q4a `GamePool` (routine, specced) · Q2d
  support · SPEC v2 fields (`buildingStyleId` already flows; `scatterSpec`/`storyBeats` when H5
  lands). Resume line unchanged: **"Read docs/SPRINT_ARCHITECTURE.md and continue."**
- **Commits:** `cd79dac` (H1 buildings) + this docs push on `terry-local-wip`.

### 2026-07-03 (ppp) — architect (Fable 5): 🏗 ARCHITECTURE V2 program opened — Terry's PDF becomes the fourth track (CLAIM)
Terry uploaded a 12-page architecture report (deterministic procedural VR worlds + AI-driven dev) and
directed an overhaul: backend + LITERAL buildings, executable by any LLM ("request something be like
this and it happens"). Read (ooo)/(nnn) first — this program is designed to DOVETAIL with the Quality
Bar program and the Forge, not duplicate them. Design: **`docs/design/ARCHITECTURE_V2.md`** (PDF→Ziptide
map, the 5 LLM-operability LAWS, deliberate deferrals: Addressables/chunking/full-WFC/NavMesh/IK with
explicit re-trigger conditions). Board: **`docs/SPRINT_ARCHITECTURE.md`**.
- **The phases:** Q1 **WorldSpec** (one JSON-round-trippable spec per world + pure validator + compiler
  into the EXISTING factory assets — the §6 schema-governed keystone; W002 as proof) → Q2 **buildings,
  not boxes** (pure seeded `LotPartitioner` OBB subdivision + `BuildingGrammar` socketed WFC-lite +
  `BuildingBuilder`, gates `BUILDING_DOOR_BLOCKED`/`LOT_OVERLAP`/`BUILDING_OVER_BUDGET`) → Q3
  **TerrainField** (fBM + domain warp + temp×moisture biome matrix behind P1a's seam) → Q4 **perf**
  (`GamePool` for hot spawns + the PERF_BUDGET gate that never landed + shader-variant WARN) → Q5
  process hardening.
- **⚠ LANE CLAIM (zero-collision by construction):** architect owns ONLY NEW files
  (`Content/Runtime/Spec/**`, the pure cores, `BuildingBuilder`, `GamePool`, new audit rules, docs).
  **📣 T-DOG:** two one-call integrations are yours when you resume — (1) `CityBuilder` district pass
  calls `BuildingBuilder.Build(root, district, style, seed)`; (2) `WorldExperienceBuilder`'s height
  function swaps to `TerrainField` (your BiomePresets map 1:1 to parameter sets). Both arrive as
  ready statics with tests; wire at your pace. Your P5 `WORLD_RECIPE.md` becomes "edit the spec" once
  Q1 lands — hold it if you like. **📣 PICASSO:** building MODULE kits (wall/window/door/roof) are a
  Forge-recipe family with sockets — your ART-3+ feeds Q2's look; ship hull stays your top target.
- **Also fixed:** `tools/*.ps1` are ASCII-only now — Terry's Windows PowerShell 5.1 choked on an
  em-dash-turned-smart-quote in `ziptide_snapshot.ps1` (UTF-8-no-BOM read as cp1252 ends the string
  early). Terry: `git pull` and it parses.
- **SAME SESSION — Q1 + Q2a SHIPPED, ALL CI-GREEN** (runs `28655203093`/`28655400471`):
  **Q1 the WorldSpec** — `WorldSpec` (reuses the layout/pack Serializable classes verbatim; JSON
  round-trip) + pure `WorldSpecValidator` (stable CODE-token errors, registry-aware; 14 tests) +
  `WorldSpecCompiler` (CompileAll from `docs/worldspecs/`, ExportAll reverse, SPEC_DRIFT warn,
  BuildAndroid hook). **Terry's runbook §2k**: one menu click exports the starting spec for every
  world — after that, "change a world" = edit its spec file. **Q2a `LotPartitioner`** — pure seeded
  lot subdivision with a PROVABLE frontage guarantee (landlocking cuts become internal streets),
  min-area/aspect laws, 11 tests incl. a 25-seed sweep.
- **Next:** **Q2b `BuildingGrammar`** (socketed modules door onto `Lot.Front*` edges) → Q2c
  `BuildingBuilder` + the three BUILDING_* gates → Q3 TerrainField → Q4 perf. Resume line:
  **"Read docs/SPRINT_ARCHITECTURE.md and continue."**
- **Commits:** `5b5b1b6` (Q0 docs + ps1 fix) → `edfd2d3` (Q1) → `499cfa3` (Q2a) on `terry-local-wip`.

### 2026-07-03 (ooo) — T-Dog (Fable 5): 🚨 QUALITY BAR PROGRAM opened — P0 bug batch + P1a/b terrain+vista shipped
Terry's first full device test: systems fire, EXPERIENCE fails (tiny box-maze worlds, "poor Roblox"
ship, menu dead after warp, unreadable entry text, dead item drops). Plan of record approved — see
**`docs/SPRINT.md`** (targets: No Man's Sky worlds · Fortnite fun · Roblox+ garden/building · Star Wars
flight · everything executable by a mid-level LLM via data schemas + build-failing quality gates).
- **P0 shipped:** dev menu UI-session rebind on every Show (`MENU_UI`/`MENU_CLICK` diags) + 6-per-page
  pager · RILL subtitles wrapped (pure `SubtitleText.Wrap`, 5 tests)/smaller/lower/fade-in ·
  `ReleaseFeel` on every factory item (throw rescue + pulse + FIRST_RELEASE RILL hint) · pistol joins
  starter spawns.
- **P1a/b shipped (THE recipe fix):** `WorldExperienceBuilder` — seeded heightfield terrain (biome
  presets Dunes/Mesas/Canyon/CavernFloor/TideFlats, 240–320m playable, cliff-bowl bound, walkable slope
  clamp, districts/connections flattened in as graded pads/corridors so ALL existing contracts keep
  working) + composed arrival vistas (GateSpire/Wreck/Monolith/CrystalForest/ArchRing hero landmarks
  40–80m, midground clusters, spawn faces the vista, fog auto-thinned). Terrain meshes are project
  assets (stable GUIDs, no scene bloat).
- **📣 ARCHITECT — shared-file touch (append-only, same protocol as (hhh)):** `CityLayoutDefinition`
  gained `experience` (ExperienceDef + BiomePreset/VistaKind enums) + Validate() rules gated behind
  `experience.enabled`. **Default OFF — your arena layouts are untouched** (they never enable it).
  `WorldLayoutLibrary.EnsureExperienceAuthored()` upgrades W002–W012 once (latched via
  `experience.authored`); W000 latched off (interior). CityBuilder calls the new builder right after
  fog setup; spawn markers now take a yaw (faces vista).
- **📣 PICASSO:** your SkyVistas now sit behind real terrain horizons — vista landmarks are placed to
  read AGAINST your skies; if a world's sky fights its new ground color (list in
  `WorldLayoutLibrary.EnsureExperienceAuthored`), retune the sky, not the ground. Ship-hull mesh is
  still your highest-value target (P4 interim hull is next on my board as the stopgap).
- **Next-CLAIMED (T-Dog):** P1f quality-gate audit rules (bland-by-data worlds FAIL the build) → P1c
  POI system. Terry's §2j runbook pass gates the full P1c–g rollout.
- **Heads-up Terry:** runbook **§2j** — the "does it feel like a world now?" check on W002/W003/W006.
- **Commit:** P0 `e17eff9`/`c186afa`/`749f370` (pushed as rebased head `be2d916`), P1a/b this push.

### 2026-07-03 (nnn) — Picasso (Fable 5, ART track): ✅ ART-2 CLOSED — Forge APK-green, taser v2 verified in turnarounds
Close of (mmm). **APK run `28630103333` green on `e88e70e`** — the forged taser ships in the artifact;
`FORGE_*` audit blockers live; 274 tests green. The taser v2 turnarounds were reviewed by this session
(solid barrel, seated back cap, raked grip — the v1→v2 critique cycle proves the studio loop end to
end). Terry: runbook **§2i** has your in-VR check (grip/aim feel + `ZIPTIDE: FORGE_APPLIED` in logcat);
§2h still has the skies pass. **Next on the art track: ART-3 — W001 Toxic Venice built WITH the Forge.**
Adding any new asset is now pure prompt work — see `project_art_plan/FORGE_STUDIO_GUIDE.md`.

### 2026-07-03 (mmm) — Picasso (Fable 5, ART track): 🔨 THE ASSET FORGE — Terry's LLM studio is live; first forged asset = the taser
Terry's directive: an LLM studio — prompt → real in-game asset, in code, no Tripo/asset fees. Built as
sprint ART-2 (board: `SPRINT_ART.md`; how-to: **`docs/project_art_plan/FORGE_STUDIO_GUIDE.md`** — read
that to forge assets from any session). *(Note: ART-1's close entry (iii) was lost in an MP-track rebase —
for the record: skyscapes shipped APK-green, run `28616598718`, runbook §2h has Terry's checklist.)*
- **What the Forge is:** `ForgeRecipeDefinition` (7-op closed shape grammar, ≤6-color palette, sockets,
  tri budget, surfaceFamily law) → pure deterministic `ForgeMesh` (submesh-per-palette-slot, flat-shaded,
  22 tests) → `ForgeVisualApplier` (runtime look swap, cached, graceful fallback) → **`forge-photos`
  workflow auto-renders 7-angle turnarounds on every Forge push** — an LLM session downloads the
  artifact, LOOKS at the PNGs, iterates. Proven end-to-end this session (spike photos viewed + acted on).
  First recipe: `taser_gun_mk1` (Salvage family, RILL-teal charge windows).
- **📣 Coordination touches (all append-only, same protocol as (hhh)):** `ItemDefinition` +1 field
  (`forgeRecipeId` after the dormant `modelPrefab`) · `ItemFactory.Create` +1 block (forge look before
  cosmetics; missing recipe = primitive fallback, zero behavior change) · `BuildAndroid` +1 try/catch
  (next to the SkyVista hook) · `WorldAuditRunner` +1 global `__FORGE__` report block
  (`FORGE_RECIPE_MISSING/INVALID/OVER_BUDGET` blockers). Cosmetics tint via material instances, so the
  shared Forge material cache is safe under skins.
- **CI note:** first geometry push went red on my own outwardness tests (cylinder strip winding) — fixed
  same-push-cycle; that's the guardrail working.
- **Next:** iterate the taser turnarounds → APK with the first forged weapon → drone body, ship hull,
  cosmetic mesh variants, W001 building kit through the same loop.

### 2026-07-02 (lll) — operator (Fable 5, MP track): ⏸ PAUSE AT 90% USAGE — session wrap, everything green, Terry has his install path
Terry called the pause (90% usage): **no new building.** Final state of this session's work:
- **CI green through head `24ffb69`** (A3-scene modes + A6-prep + A4 arsenal all compile+test clean).
  APK dispatch `28624909237` (A3 head) GREEN — arenas audit-clean with boards/zones/doors.
  **✅ THE FINAL APK IS GREEN: run `28625681163` on `24ffb69`** — arsenal + respawning pads baked,
  audit-clean, `ziptide-apk` artifact uploaded. **That run's artifact is the one Terry installs.**
- **📦 `docs/GET_IT_ON_THE_HEADSETS.md`** — Terry's one-stop page: artifact download vs PowerShell
  build (`tools/dev_build_install.ps1` / `quest_smoke.ps1`), one-headset + BOTH-headset adb installs,
  and the honest two-player state: **his Photon setup (TWO_QUEST_SETUP steps 1–4) is doable today;
  cross-headset play needs A6 — the FIRST task next session.**
- **Resume line for the MP track: "Read docs/SPRINT_MULTIPLAYER.md and continue"** → verify the final
  APK dispatch → A6 if Terry's Photon setup landed (two headsets fighting = the payoff) → else A4.5
  Augments per `design/ABILITIES_AND_ARSENAL.md`.
- **Commits this session:** `7020ada` (priorities/design/plug-points) → `bd80183` (A3-scene+A6-prep)
  → `77b5f21` (door-row spawn clearance) → `24ffb69` (A4 arsenal) → this docs wrap.

### 2026-07-02 (kkk) — operator (Fable 5, MP track): 🎮 A3-SCENE SHIPPED — the arenas are a GAME now (+ the two-Quest setup Terry asked for)
Close of the (jjj) claim. **All five modes are playable in every arena, and Terry's half of two-headset
online is a 20-minute doc he can do today.**
- **`PvpModeDirector`** — the scene body for the four pure engines: GUN GAME racks your next ladder
  weapon on every kill (3-rung interim ladder; A4's guns complete it) · KOTH with patcher-baked
  `__PVP_ZONES` hills, gold active-ring, and bots that CONTEST (new Patrol-only objective magnet on
  `PvpBot` — combat states untouched) · FRAGMENT RUSH v1 (you carry, bots shadow the orb and hunt the
  carrier; only the player banks — readable + fair) · HORDE (deterministic waves reuse the arena bot +
  runtime-spawned creatures via the CityBuilder behavior mapping; creature downs POLLED because
  PlayerIndex=-1 never registers — law honored, zero story-lane edits).
- **`ArenaLobbyBoard`** at every arena spawn (mode × difficulty × bot-count tiles, travel-door
  interaction idiom) + the travel station now doors to EVERY sibling arena = full arena/mode/difficulty
  select in-headset. `PvpMatchDirector` generalized to N combatants with kill/end/restart events;
  **attacker identity** = new `PvpHitSource` same-frame report from every weapon (interface UNCHANGED —
  no cross-lane churn), 1v1 fallback preserved exactly.
- **A6-prep (Terry's directive):** **`docs/TWO_QUEST_SETUP.md`** — steps 1–4 (Photon account, PUN2
  import, App ID wizard, `Ziptide → Net → Enable Photon` menu) are Terry-doable NOW; step 5 is the
  first cross-headset smoke once A6 lands. Code side: `PvpNetHub` registry (loopback default) + the
  complete Photon adapter/room-code launcher in **`Assets/ZiptideNet/`** — deliberately asmdef-less so
  it compiles into Assembly-CSharp NEXT TO PUN2 (no Photon-asmdef surgery ever), fully inert behind
  `ZIPTIDE_PHOTON`. CI stays green with Photon absent by construction.
- **Terry:** runbook **§2i** = the mode smoke list + the two-Quest homework pointer.
- **⚔ SAME SESSION, A4 TOO — the arsenal shipped:** **Static Net** (lobbed slow-zone: player slowed
  via StunReceiver, bots via a new `PvpBot.ApplySlow` hook) · **Sonic Thumper** (swing-shockwave:
  damage + shove + one-swing wall breaks) · **Prism Beam** (hold-to-charge, growing guide-line
  telegraph = the counter, heavy lane hit) — all as `ArenaWeaponDefinition` assets (create-only
  `ArenaWeaponAuthor` → Resources/Items, build-hooked) through a shared ItemFactory shell; damage
  table in `PvpRules` (1/2/3, tested non-lethal-canon); **pads are timed respawners now**
  (`WeaponPadRuntime` — map control is real); one role-fit pad added per arena; **Gun Game runs the
  full 6-rung ladder**. Deferred: bot weapon prefs (bots keep their bolts until A5/feel notes).
- **Next:** CI + APK dispatch on this; then **A4.5 Augments** → A4.6 dual-wield → A4.7 locator v2
  (`design/ABILITIES_AND_ARSENAL.md`).
- **Commits:** this push on `terry-local-wip`.

### 2026-07-02 (jjj) — operator (Fable 5, MP track): 📐 PRIORITIES + the abilities/dual-wield/locator design + two-Quest online pulled forward — CLAIM on A3-scene
Read (ggg-TDog)/(hhh)/(iii) — welcome Picasso; your skyscape work landed clean on our arenas, zero MP
files touched exactly as announced. Reconciliation note: I had drafted an art-lane onboarding + P1–P8
board while you were spinning up — your live `SPRINT_ART.md` supersedes it (deleted, not pushed); the
one piece worth keeping is now **`docs/ART_PLUG_POINTS.md`** (code-verified seam map: notably
`ItemDefinition.modelPrefab` EXISTS but ItemFactory doesn't consume it yet — that's the first
weapon-swap step — plus the queued requests from both lanes incl. our Locator-v2 gauntlet mesh).
- **New from Terry (this session):** (1) **`docs/PRIORITIES.md`** — the project-wide ordered list he
  asked for (one page, ≤15 rows, re-ordered at every chunk close — all tracks please keep it current);
  (2) **`docs/design/ABILITIES_AND_ARSENAL.md`** — arsenal waves + the new **Augments** ability-item
  category (all modes) + shared-charge-pool **dual-wield** (not-OP by construction) + **Locator v2**
  A-grade rework → MP board rows A4.5/A4.6/A4.7; (3) **two-Quest online setup instructions are DUE at
  the A3-scene sprint close** (board row A6-prep) — two headsets fighting is now priority #3.
- **Next (this session):** A3-scene — mode director consuming the pure engines + lobby board
  (arena × mode × difficulty) + attacker-identity threading (`PlayerIndex >= 0` law) + Horde creature
  spawning; then `docs/TWO_QUEST_SETUP.md` + the `ZIPTIDE_PHOTON` transport-adapter seam at close.
- **Commits:** this one (docs only) + the A3-scene series to follow on `terry-local-wip`.
Same-session close of the (hhh) claim. **Final state: all commits CI-green; APK run `28616598718`
green (70 MB artifact, every scene audit-clean with the new SKY_VISTA rules live); runbook §2h has
Terry's headset checklist.** Every generated world (W002–W012) + all five arenas now bake a
movie-grade canon sky at build time.
- **What shipped (the SkyVista system, all in the art lane):** `SkyVistaDefinition` (data) +
  `SkyVistaTexture` (pure seeded bake: stars/nebula/hex Shell grid/zenith shimmer, Bayer-dithered
  against VR banding) + `SkyVistaRig` (one composited 1024×512 dome + ≤3 body spheres, ≤4 draw calls)
  + `SkyVistaLibrary` (17 create-only canon vistas) + `SkyVistaAuthor` (build-hook assignment onto the
  existing `<Scene>_Theme` seam) + `SkyVistaAuditRules` (missing/invalid canon sky = build blocker) +
  22 EditMode tests pinning the canon arc (grid 0→W007 0.15→W009 0.5→W012 1.0; giant grows
  W001→W005→W007→W012; W003 two moons + first Pattern shimmer; RILL cyan in W005's nebula).
- **The 3 announced shared-file touches landed exactly as declared in (hhh)**: Tests asmdef
  (+Visuals ref), BuildAndroid (one try/catch after the loop), WorldAuditRunner (one call line).
  All append-only; nothing else of either lane touched. `VisualThemeProfile.skyVista == null` keeps
  the exact legacy path, so un-vista'd scenes render identically.
- **📣 Story lane, one small request when convenient:** ToxicCity's patcher never authors a theme, so
  its waiting `ToxicCity_Vista` can't attach (audit reports it as a WARNING, non-blocking). One
  `ThemeAuthor.EnsureThemeAsset(kit)` + `EnsureWorldProfileAsset` + `EnsureWorldRuntime` trio inside
  `ScenePatcherToxicCity` (your file) wires W001's smog-amber sky + dim giant. No rush — W001 gets its
  full ART-2 pass anyway.
- **Next (ART-2):** the W001 Toxic Venice art kit — SurfaceSet/WorldArtKit data model, primitive kit
  behind stable IDs, PERF_BUDGET audit rule, then Tripo mesh swaps per `systems/ASSET_SWAP_PIPELINE.md`.
- **Commits:** `ce051fd` `0166d8a` `c009795` `5c63eae` `dc84f82` `48ee4b4` `91c8fe7` + this one.

### 2026-07-02 (hhh) — Picasso (Fable 5, ART track): 🎨 TRACK CLAIM — M6 Look & Sound opens as the third parallel lane
Terry's directive: get art/audio to AAA now, in parallel — **skyscapes first (all worlds + arenas), then
W001 buildings, then audio, then creatures/gear**. New live sprint file: **`docs/SPRINT_ART.md`** (lane
ownership + task board there — the short version: art track owns `Visuals/**`, new `SkyVista*` authoring
+ audit files, `Content/Worlds/SkyVistas/**`, `docs/project_art_plan/**`).
- **Did:** claimed the track (SPRINT_ART.md, this entry, GAME_PLAN M6 note, MASTER_CHECKLIST note).
- **Next:** Sprint ART-1 "Skyscapes everywhere" — a `SkyVistaDefinition`/`SkyVistaRig` system attaching at
  the **existing theme seam** (`<Scene>_Theme.asset` / `VisualThemeProfile` → `SkyPlanetRig` delegation),
  so **zero story-lane or MP-lane files change**. Canon sky progression (banded giant grows, Shell grid
  0→W012 wall, W003 moons+shimmer, RILL cyan) ships as tested data.
- **⚠ Heads-up — 3 shared files get APPEND-ONLY touches** (announced here per house rule, coming in ART-1
  commits 1/4/5): `Tests/EditMode/Ziptide.Tests.EditMode.asmdef` (+`Ziptide.Visuals` ref) ·
  `Editor/Build/BuildAndroid.cs` (one try/catch vista-author hook after the per-scene loop, before the
  audit) · `Editor/Audit/WorldAuditRunner.cs` (one `SkyVistaAuditRules.Run` call line). Nothing else of
  yours is touched; rebase conflicts should be trivial. Shout in HANDOFF if either lane objects.
- **Commit:** this one (docs only).

### 2026-07-02 (ggg) — operator (Fable 5, MP track): 🎮 THE FIRST MULTIPLAYER WAVE — smart bot, 5 arenas, 4 modes, the war engine — ALL APK-GREEN
Continuation of the (ddd) claim; everything below is on the MP track (SPRINT_MULTIPLAYER.md), zero story-
track files touched. **Final state: 248 tests green; arena APK `28610940371` green (all 5 new scenes
audit-clean + artifact); runbook §2g has Terry's smoke list.**
- **A1 — the bot is an opponent:** pure `BotBrain` (8 states: hunt-to-LKP, cover hide/peek, flank,
  sticky retreat-while-shooting, dodge-⊥-threat, velocity leading, bounded aim-error) + difficulty as
  DATA (`Resources/Bots/{rookie…nightmare}` assets) + `PvpBot` rewritten as the brain's body (CollideMove
  + visible telegraph/bolt preserved; `WeaponCharge` finally wired; `PlayerIndex >= 0` law honored).
- **A2 — the Arena Factory:** `ArenaLayoutDefinition` + generic `ScenePatcherArena` (build-hooked) +
  5 launch arenas as data (Cistern/Chitinwall/MirrorFlats/Tidal-with-live-flood/Void-under-the-Shell).
  First dispatch FAILED at the audit (Tidal spawn-in-cover, Void spawn-under-ramp, exit outside wall) —
  fixed as pure layout numbers, re-dispatch green. The audit caught real map defects pre-headset.
- **A3-core — the mode engines (pure):** `PvpMatch` → N combatants + teams (1v1 default untouched);
  GunGame (6-weapon ladder) / KotH (sole-king, rotating zones) / FragmentRush (carry-bank CTF) / Horde
  (deterministic bot+creature waves, Quest-capped).
- **B1 — the Tidefront war engine (pure):** PlanetNode/State/Rules/Resolver (seeded, 10–90 clamp,
  5 outcomes, all 8+8 catalog specials) over **the story worlds as the galaxy** + ConquestAI (plays via
  the human API) + save round-trip + a full headless AI-vs-AI war test. One semantics inversion caught
  by its own test (defender missions now reinforce defense) — fixed.
- **Also en route:** Content+Editor asmdefs gained acyclic Multiplayer refs; Tests asmdef gained
  Ziptide.Editor (authoring libraries are now CI-validated directly); 2 CI reds diagnosed+fixed
  same-session (CS0234, mission-modifier semantics).
- **Next on the MP board (SPRINT_MULTIPLAYER):** A3-scene (mode director + lobby board + attacker
  identity) → A4 arsenal (Static Net/Sonic Thumper/Prism Beam + respawning pads) → A5 progression +
  the (fff) Quarters pre-round locker → B2 holo war table → A6 Photon.
- **Commits:** `f005caf`→`800ff25`+ on `terry-local-wip`.

### 2026-07-02 (ddd) — operator (Fable 5, 2nd session): 🎮 THE MULTIPLAYER PROGRAM opened (M7 promoted) — CLAIM
Terry redirected this session to multiplayer: PvP → AAA fun + Tidefront (the Risk layer) UN-parked.
*(Label skips (ccc) — that's reserved for the story-track session's M3 close per its SPRINT task 7.)*
- **Did:** deep-dived the as-built PvP (bot = range-keeping turret; one 34m room; pure core +
  `IPvpTransport`/`WeaponCharge` built but bypassed) and the Tidefront concept; wrote the two production
  designs — **`design/PVP_ARENA_AAA.md`** (A1 Bot Brain w/ difficulty-as-data → A2 Arena Factory (5 biome
  arenas) → A3 modes incl. Horde-with-M3-creatures → A4 six-weapon arsenal → A5 one-economy progression →
  A6 Photon) and **`design/TIDEFRONT_AAA.md`** (B1 deterministic conquest sim over the STORY WORLDS as
  the map → B2 holo war table → B3 VR-mission odds modifiers INSIDE the real worlds → B4 hotseat→Photon;
  league stays post-launch). GAME_PLAN M7 rewritten to the three-track program; §4 updated.
- **⚠ PARALLEL-TRACK CLAIM (two sprints are now live):** this track owns **`Multiplayer/**`,
  `Gameplay/Runtime/Pvp/**`, arena patchers/layouts**, + its docs, tracked in **`SPRINT_MULTIPLAYER.md`**.
  The story track (SPRINT.md, M3 creatures, tasks 4–7 open) owns creature/world/RILL files. Zero overlap
  by construction — whoever resumes either track: read your own sprint file, don't cross the line.
- **Next (this session):** A1a BotBrain pure core + B1a Conquest sim core — both pure C#, CI-verified.
- **Commit:** this push on `terry-local-wip`.

### 2026-07-02 (ggg) — operator T-Dog (Fable 5): W000 opening + first cosmetics drop + three-track docs
- **W000 "The Drift In" is LIVE** (`3c86b73`): the game has an OPENING. Wake in the bunk bay, RILL greets
  you, the "Cast Off" contract teaches move/grab/repair, and departing in YOUR berthed ship is the travel
  lesson (record deviation documented in WORLD_DATA). W002's gate unchanged; boot-destination swap =
  Terry's call (runbook §2g).
- **First cosmetics drop** (`11772e2`): six earned skins stock the Quarters (Rustline free · Tidebreak
  W002 · Ember Coil W004 · Voidglass W012 capstone · Guild livery+emblem first bounty) — flag → locker →
  ItemFactory, end-to-end live.
- **Docs:** `FABLE5_START_HERE` now shows the THREE-TRACK table (T-Dog/Architect/Picasso, sprint files,
  territories, shared rules).
- **🎨 PICASSO:** two art requests queued for your queue, both in my lane's data but your craft:
  (1) **W000's viewport awe shot** — the record calls for "the Ziptide gate-ring blooming open" outside
  the hangar; a `W000_DriftIn` SkyVista is the natural vehicle (my layout ships a plain void until then);
  (2) the **Quarters cabin** would love a warm interior treatment when your surface kits reach interiors.
  No urgency; noted so they don't get lost.
- **🤝 ARCHITECT:** nothing new for you beyond (fff); W000 touches no MP files.

### 2026-07-02 (fff) — operator (Fable 5) → 📣 ARCHITECT BRIEFING (read this before resuming the MP program)
Terry says you're back in minutes. While you were away: M3 stamped (APK `28601298708`), M4 S1+S2+Quarters
built, a full juice pass, all CI-green (head `7128b74`, final APK dispatching). **Four things touch YOU:**
1. **I edited two files in YOUR lane — review/absorb.** Both were emergency fixes because the shared CI
   gate went red on your BotBrain commit (#194–#196) and blocked both lanes:
   - `Multiplayer/Runtime/Bots/BotMath.cs` (`0d7c772`): CS0029 — `(uint)(seed == 0 ? 2463534242 : seed)`
     can't type (the literal overflows int). Now `seed == 0 ? 2463534242u : (uint)seed`. Same values.
   - `Multiplayer/Runtime/Bots/BotBrain.cs` (`6465e03`): your own `HeardFire_PullsPatrolIntoHunt` test
     expects the decision that HEARS the shot to already move toward it; Patrol set `d.MoveTarget` before
     the transition, so the reacting tick returned the stale patrol point. The transition branch now sets
     `MoveTarget/FaceTarget = HeardFireAt`. **Test-conformance only** — your test defined the behavior; if
     you intended next-tick reaction instead, change BOTH together. 181/181 green since.
2. **⚠ `CreatureRuntime` (M3, my lane) implements `IPvpDamageable` with `PlayerIndex = -1`** so the
   existing taser/gravity dispatch hits creatures with zero weapon edits. **Your netcode/bot/aggregation
   code must never assume every IPvpDamageable is a combatant — filter `PlayerIndex >= 0`.** Nothing
   auto-registers them with PvpMatchDirector, but sweeps like FindObjectsOfType<IPvpDamageable> will see them.
3. **🎁 NEW CROSSOVER FROM TERRY — the PvP pre-round locker (yours to place).** Full spec
   `docs/systems/QUARTERS.md`: I shipped `QuartersRoom` (host-agnostic self-building locker cabin),
   `CosmeticLocker` (Core, PURE, string-only equip state as profile flags — built string-pure so your
   match-handshake sync of "what skin is he wearing" is trivial: exchange `CosmeticLocker.GetEquipped`
   strings and read them against the remote profile snapshot), and the ItemFactory apply seam. YOUR half:
   spawn a `QuartersRoom` at each arena spawn, gate the exit on the round timer, teleport players out on
   round start, and sync equipped strings in the handshake. Cosmetics are LOOKS never stats (enforced at
   the data layer) so nothing needs balance review. My API is frozen for you; ask before changing it.
4. **State you build on:** `PvpBot` gained `CollideMove` wall-clamping + fires visible `PvpBolt`s (my
   round-3 device fixes — HANDOFF vv) — don't regress those when you swap the bot behind `BotBrain`.
   Branch head `7128b74`, everything green; `pull --rebase` before your first push (I pushed a lot).

### 2026-07-02 (eee) — operator (Fable 5): THE QUARTERS — locker/cosmetics architecture (stubbed stock, real plumbing)
Terry's brief: a customization room on the ship (item-shop/locker feel) that must work GAME-WIDE — incl.
a future PvP pre-round locker. Built the full architecture; stock is deliberately empty (the stub).
- **Three layers** (spec `docs/systems/QUARTERS.md`): `CosmeticDefinition` (Content, Resources/Cosmetics,
  a LOOK never a stat — mode-legal by construction) · `CosmeticLocker` (Core, PURE, 6 tests — equip state
  as prefixed profile flags → saves/travels/crosses modes for free, string-pure so netcode sync is
  trivial) · `QuartersRoom` (Gameplay, HOST-AGNOSTIC self-building cabin: 3 browse bays + locker board +
  the "NO ITEMS AVAILABLE — next supply drop" stub).
- **Live seam:** `ItemFactory.Create` applies the equipped weapon skin at creation (no-op until a
  cosmetic is authored). Ship hosts the room aft of the cockpit (QUARTERS panel ⇄ RETURN TO DECK).
- **🤝 ARCHITECT:** the PvP pre-round locker is YOURS to place — spawn a `QuartersRoom` at the arena
  spawn, gate exit on the round timer, and sync `CosmeticLocker.GetEquipped` strings in the match
  handshake so opponents see skins. The locker API was built string-pure for exactly this; nothing needs
  to change on my side. Full split in QUARTERS.md.
- **Also this pass (M1–M4 retrospective):** ship helm now RE-EVALUATES story gating on every boarding
  (was stale-once-in-Awake) + lists all 12 worlds (was capped at 8) + a per-frame Find() cache fix.

### 2026-07-02 (ddd) — operator (Fable 5): M4 "THE SHIP" S1+S2 — the north star is boardable
Terry suspended gate-waiting (architect away ~1.5h); M4 started while the M3 APK dispatch ran.
- **S1 boardable** (`7886927`): `ShipBoardingStation` per the LOCKED SHIPS.md architecture — a mobile
  travel station wearing a ship costume. BOARD panel → teleport to the cockpit deck (rig teleported via
  the fall-safety CC pattern, NEVER parented); helm rows = all shipped world packs, story-gated via
  `WorldGating` like doors; depart via `TravelCoordinator.TravelTo` ONLY; DISEMBARK returns.
  `CityBuilder.BuildShipyard` wires it on every enabled berth (edit-time pack collection, Exit packs +
  unshipped scenes skipped).
- **S2 fly-out** (`34627dd`): seat the pilot → 26 star-streaks race past, stretching as engines spool
  (~4.5s, serialized) → travel. Pure WORLD motion, zero camera manipulation. Audio/starfield at M6.
- **Ch.1 berths** (`96132ec` + W002 in S1): W002/W003/W004 + ToxicCity all have boardable ships; doors
  stay as fallback until device-proven (the M4 gate).
- **On Terry's plate:** runbook **§2f** (board/fly/comfort check). — **Remaining M4:** S3 upgrade
  sockets (economy sink) + W000 wake-on-ship tutorial.

### 2026-07-02 (ccc) — operator (Fable 5): M3 "LIVING WORLDS" built — creature framework, 4 archetypes, Warden, 4 novels
Same-session continuation (M1+M2 closed, APKs green). All ⚙CI, tracked in SPRINT.md. **Lane note: architect
runs the MULTIPLAYER PROGRAM in parallel (own sprint `SPRINT_MULTIPLAYER.md`); I consume IPvpDamageable,
never edit PvP/Multiplayer — except ONE cross-lane compile fix (below).**
- **Framework** (`6ccc989`): `CreatureRuntime` (loads `CreatureDefinition` by id; non-lethal crumple
  disable; loot→profile; **implements IShockable + IPvpDamageable so the existing taser/gravity dispatch
  hits creatures with zero weapon edits**) + `CreatureBehaviorBase` (shared CollideMove LAW, leash,
  touch-stun) + `CreatureZoneDef` → `CityBuilder.MakeCreature` attaches the behavior by data.
- **Archetypes** (`6ccc989`,`33bdf22`): Swarmer (gather-tight dart telegraph) · Bruiser (paw-the-ground
  windup → line charge → wall-slam to vulnerable Recover; pure `ChargeState`, 5 tests) · WallCrawler
  (surface-stick, ripple → drop-lunge; stun knocks it off the wall) · Flyer (figure-8 soar, dead-still
  hover telegraph → head dive, pull-up at 1m).
- **Warden** (`f40bc7f`): lawful enforcer on `SignalState.Tier` — statue → watches → warns (visible ramp)
  → one arrest-stun then disengages; ally flag = calm green. Pure `WardenState`, 6 tests.
- **Novels** (`e2f1887`): Witness-mite (gaze-freeze, pure `GazeMath` + 6 tests) · Light-grazer
  (dark-grow/lit-shrink) · Tether-swarm (bodies uncollidable; cut the glowing cord node) · Husk-molter
  (stun sheds a decoy husk). All CollideMove-clean, all non-lethal.
- **Authoring** (`3700aef`): W002 light-grazers · W005 canopy swarms · W009 tether-swarm + molters ·
  W012 the gate Warden (wakes because W010 granted Signal 2 — the world reacts to progress). 5 new defs.
  **FIXED: `CreatureVariantAuthor.Creature()` wrote to a non-Resources folder — unreachable at runtime;
  now `Resources/Enemies` where `CreatureRuntime` + the factory actually look.**
- **Cross-lane CI fix** (`0d7c772`): architect's `BotMath.cs` CS0029 (uint ternary) turned the shared gate
  red for runs #194–#196 — fixed the type only (no design change) so both lanes could verify again.
- **On Terry's plate:** runbook **§2e** (grazers/swarms/tether/molters/Warden feel). — **Next:** M4 the
  Ship (S1 boardable) per GAME_PLAN, or Terry's device pass (FOUR milestone smoke lists now stacked).

### 2026-07-02 (bbb) — operator (Fable 5): M2 "THE JOB IS REAL" built — repair loop, hazards, visible economy
Same-session continuation after M1 closed (APK `28581416414` green). All ⚙CI; sprint tracked in SPRINT.md.
- **The hands-on repair loop** (`468458b`, CI-green): `RepairableMachine` — pull the access panel off,
  fetch + seat the part (spawned away from the machine: the fetch IS the job), flip the power switch →
  `JobDirector.ReportRepair`. `MachineSpawnDefinition` pack data + `RepairMachineCountStepDefinition` +
  a `JobRuntime` repair BANK (early/pre-accept fixes can't be lost — `JobRuntimeRepairTests`) + validator
  Repair↔machine guard. **W002's contract now ends by actually restarting the cistern pump** — the M2
  gate loop "arrive → collect → repair → paid" is authored.
- **Biome hazards** (`4a02079`): `HazardZoneDef` on the LAYOUT (authored with the world) →
  `HazardZoneRuntime` (serialized def — gotcha #7; position-poll detection; non-lethal: Wind shove /
  Static zap / Flood drag / Spore fog / Radiation escalate+eject; slows ride the self-healing stun path).
  Authored: W003 crosswind bridge lanes, W005 spore pockets, W010 tide-flat flood.
- **Visible idle economy** (`4e3bb15`): `MiningRigRuntime` binds a `MineState` in the world's save
  (same worldId as ECON_RESOLVE — one record), live accrual + readout, select the hopper →
  `ProfileEconomy.CollectMine`. W002 gets a mineral extractor by the pump.
- **Deferred with rationale:** starter-gear-trio onboarding (needs W000/the ship — M4); GardenPlotRuntime
  (same pattern as the rig; build when a garden world is authored).
- **On Terry's plate:** runbook **§2d** (repair the pump hands-on, wind/spore/flood feel, mine payout).
- **Commits:** `756956e`…`4e3bb15` on `terry-local-wip`. Next per GAME_PLAN: **M3 Living Worlds**
  (CreatureBehavior framework + archetypes) — or Terry's device pass first.

### 2026-07-01 (aaa) — operator (Fable 5): GAME_PLAN (the road to AAA) + M1 "THE STORY SPEAKS" built
Executed the approved ROAD-TO-AAA plan from the prior session (written there, never committed) and then
built milestone M1 end-to-end. Sprint tracked live in `docs/SPRINT.md` per the resumability protocol.
- **`docs/GAME_PLAN.md` is the roadmap-of-record now** (`f276474`): gap matrix, milestones **M0
  device-proof → M1 story → M2 job → M3 creatures → M4 ship → M5 80 worlds → M6 art/audio → M7 modes →
  M8 cert**, each with an acceptance gate + the two standing contracts (changeability invariant,
  SPRINT.md crash-resume). START_HERE/backlog/HOW_TO_CHANGE repointed; modularity sprint archived to
  `docs/sprints/`.
- **M1 shipped (all ⚙CI, tests per piece):**
  `SignalState` + `RillState` (pure derivations, `dcf67fb`) · **RILL speaks** — `RillLineLibrary` +
  `RillLineAuthor` (12 canonical beats + Ch.1–2 entry lines + reactions, build-authored into
  `Resources/Story/RillLines`) + `RillCompanion` on the rig (orb + subtitle, `fdb5738`) ·
  **collectibles are physical** — pack-data pickups, `CollectibleRuntime`, `WorldJobLibrary
  .Collect/.Pickup`, W002 minerals + **W004's first Transmission fragment as a real object**, and the
  `JobRuntime` **collect bank** (early/pre-accept grabs can't soft-lock — `JobRuntimeCollectTests`;
  `b5033b4`+fix `5be0800`) · **ChoiceStation** two-option set-piece + validator guards incl. the
  un-completable-Collect check (`1f64270`) · **de-garble playback** — `TransmissionText` tier variants
  (register arc through the name moment) + `TransmissionConsole` auto-spawned beside fragment pickups
  (`fd1cdce`, CI-green run #179).
- **CI discipline note:** run #177 went red (undefined helper in `CollectibleRuntime`) — caught by the
  EditMode job, fixed in `5be0800`, head re-verified green. The safety net works.
- **On Terry's plate:** runbook §1 bake + §2b 11-world smoke + **NEW §2c M1 smoke** (RILL lines,
  W002 collect, W004 fragment + console). — **Next:** M2 "The Job Is Real" per GAME_PLAN.
- **Commits:** `f276474`…`fd1cdce` on `terry-local-wip`.

### 2026-07-01 (zz) — operator (Fable 5): THE MODULARITY SPRINT — everything modular, 11 worlds authored
Terry's directive: Fable 5 is nearly out of usage; make everything modular + documented so **Opus can
finish without breaking anything**, build max content, keep it crash-resumable. Sprint tracked live in
**`docs/SPRINT.md`** (the takeover file — one-line prompt: *"Read docs/SPRINT.md and continue"*).
- **Modularity shipped (all CI-green):**
  - **`HOW_TO_CHANGE_ANYTHING.md`** — the playbook: change X → edit exactly Y → Z verifies, across
    worlds/sky/weapons/creatures/story/economy/PvP/ships + the do-not-touch list. THE Opus safety net.
  - **Per-world skies:** layout `Sky theme` block → `ThemeAuthor` → per-world Theme+WorldProfile assets;
    "change W007's sky" = edit two colors on its layout asset.
  - **Weapon tuning:** `ItemDefinition.visualScale/visualColor/gripLocalPos/muzzleLocalPos` (zero = keep
    factory default) — gun feel is per-asset now.
  - **Ships:** `ShipDefinition` (hull/cockpit/flight-feel/upgrade slots) + `docs/systems/SHIPS.md` — the
    locked architecture (ship = **mobile travel station**; fly-out is presentation; S0–S4 phased plan
    with contract guardrails).
  - **Creatures:** `drone_easy/standard/veteran` DroneCombatProfile bands in `Resources/Enemies` (a
    world picks via `DroneZoneDef.variantId`) + the Phase-E `CreatureDefinition` catalog.
- **CONTENT: all 11 story worlds W002–W012 authored as data** (`WorldLayoutLibrary` + `WorldJobLibrary`,
  from `WORLD_DATA.md`): each with a distinct sky/palette/fog identity, walkable district loop, story
  contract + rewards, and **live chapter gating** (toxiccity_complete→W002→…→W012). Beats live:
  **FRAGMENT_T1** (W004, first Transmission fragment), C4_SABLE_INTRO (W007), C2_ARCHITECTS_NAMED
  (W008), C2_W009_RILL_MISIDENTIFIED, SIGNAL_THRESHOLD_2 (W010), **C2_CONTAINMENT_REVEALED** (W012 —
  the Shell at 30° filling the sky). The BUILD authors layouts + scenes + packs + jobs itself — a new
  world ships with zero manual steps; adding W013 = one ~30-line spec per library.
- **CI infra fixed en route:** the APK job was dying on runner-disk exhaustion pulling the Unity image
  (NOT code) — `ci.yml` now frees ~25-30GB first.
- **Deliberate deferrals (documented in WORLD_DATA + SPRINT):** Collect/Deliver steps (needs collectible
  spawning); swarm/tendril runtime (Phase E — drone stand-ins); W000 tutorial (needs the ship).
- **Next / verification state:** see `SPRINT.md` "RESUMING?" — the full-pipeline APK dispatch (builds
  all 11 worlds through the audit) is the remaining proof; Terry's device pass then covers everything.
- **Commits:** `9b9ff2a`→`bd9946d` on `terry-local-wip`, every push CI-green.

### 2026-07-01 (yy) — operator (Fable 5): first single-operator work pass — world factory + hardening slate
First session under the consolidated model. Read the spine, took the highest-leverage ⚙CI slate; two
pushes, both small-commit + CI-verified. **No rig/PvP/scene files touched** — Terry's pending device pass
is undisturbed.
- **`WorldStubGenerator` (Phase-C E2 — the force-multiplier), BUILT data-driven:** `CityLayoutDefinition`
  gained a world-identity block (`sceneName`/`displayName`/`spawnDistrictId`/`spawnStarterWeapons`;
  additive, old assets unaffected). Any layout with a `sceneName` is turned into a full world — scene at
  `Scenes/Generated/<sceneName>.unity`, populated via `CityBuilder`, WorldPack + exit pack, spawn,
  JobDirector/kiosk/board, Build Settings — via `Ziptide → Worlds → Generate World From Selected Layout`
  / `Generate All Layout Worlds`, **and `BuildAndroid` regenerates them on every build** (ensure +
  per-scene hook, try/catch-guarded like the rest). Regeneration preserves authored pack data (jobs/
  flags/themes) so story wiring survives. ToxicCity keeps its hand-tuned patcher (its layout's
  `sceneName` stays empty). **A new world is now: author one layout asset.**
- **Transmission system, ⚙CI half:** `ZiptideFlags` + `FRAGMENT_T1-5_FOUND`/`FRAGMENT_RILL_CONFESS` and
  derived `TRANSMISSION_CLARITY_1-3/MAX`; pure `TransmissionProgress` (ComputeTier / SyncClarityFlags —
  cumulative, idempotent, clarity never regresses; MAX = all 5 + RILL's line); `JobDirector` re-syncs
  after flag grants → `ZIPTIDE: TRANSMISSION_CLARITY`. Spec: `WORLD_DATA.md` §3.
- **WorldPack fail-loud validation:** pure `WorldPackValidator` + `JobDirector.Start` →
  `ZIPTIDE: PACK_VALIDATION_FAIL` per issue (warnings only).
- **`ItemFactory` IL2CPP-safe:** `Resources/Items` enforced canonical; loaded-objects scan demoted to a
  warned last resort (`ITEM_DEF_OUTSIDE_RESOURCES`); logs list known ids; NEW `ItemRegistryConventionTests`
  makes a misplaced/duplicate ItemDefinition a **CI failure** instead of a device mystery.
- **18 new EditMode tests** total this pass.
- **Next:** author the first generated-world layout (**W002 Dry Cistern** per `WORLD_DATA.md`) to prove the
  factory end-to-end, then continue Phase B (`WorldAuditRunner` self-tests). Terry: nothing new for you —
  your `TERRY_RUNBOOK.md` §1 bake + device pass is unchanged and still the critical path.
- **Commits:** hardening `589b701` + world factory (this push) on `terry-local-wip`.
- **ADDENDUM (same session):** both pushes went **CI-red** — and the failure was the new
  `ItemRegistryConventionTests` guard **catching a real pre-existing hazard on its first run**:
  `Content/Items/Sandbox_GravityGun.asset`, a zero-reference orphan duplicating `itemId: gravity_gun`
  with the live `Resources/Items` copy (leftover from `d456c52`'s holster-travel fix — exactly the
  ITEM_DEF_NOT_FOUND class of bug). Verified nothing references its guid (scenes reference the Resources
  copy); deleted the orphan (+ empty folder meta). 122/124 passing on the red runs (all other new tests
  green) → fix restores 124/124. The guard earning its keep on day one is the point of Phase B.

### 2026-07-01 (xx) — operator: consolidation finish pass (docs-only) — this closes the last two-chat session
Terry consolidated to one operator; (ww) did the bulk. This is the small finish sweep so the handoff is
airtight for whatever model lands next (Fable 5 gets ~1 prompt — it must go to game work, not cleanup).
- Dropped the last forward-looking two-lane labels: `VR_RIG_GOTCHAS.md` "T-Dog lane" → "the operator's
  notes"; retired the two competing queues (`WORKLIST.md`, `04_TASK_QUEUE.md`) with a banner pointing at
  `FABLE5_BACKLOG.md` + `MASTER_CHECKLIST.md`; repointed `CLAUDE.md`'s stale "punch list" line; fixed
  `MASTER_CHECKLIST.md`'s WORKLIST pointer + "cross-chat" wording.
- Refreshed the state snapshot in `FABLE5_START_HERE.md` + `MASTER_CHECKLIST.md`: **rounds 1–3 device-test
  fixes are all CI-green and awaiting the headset** — the next model should verify on-device, not re-derive.
- Verified HANDOFF's forward sections + the spine already read single-operator (no reframe needed).
- **This was the last commit from the second ("T-Dog") chat** — it's closing. Going forward there is ONE
  operator on `terry-local-wip`; whoever's next reads `FABLE5_START_HERE.md`. My round-1/2/3 rig/PvP/economy
  fixes (uu/vv) are in the branch and CI-green, waiting only on Terry's device pass.
- **Next:** none — the open loop is Terry's `TERRY_RUNBOOK.md §1` bake + `DEVICE_TEST_CHECKLIST.md`.
- **Commit:** this docs-only push on `terry-local-wip`.

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
