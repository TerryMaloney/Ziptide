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
