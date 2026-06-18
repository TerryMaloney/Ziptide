# HANDOFF — shared cross-chat log (T-Dog ⇄ Architect)

⛔ **HARD RULE (both Claude chats, no exceptions):**
1. **Read this file at the START of every session.**
2. **Append a `Did / Next-CLAIMED / Heads-up / Commit` entry at the END of every session.**
3. **All shared work + this log live on the `terry-local-wip` branch** (it's green and is what Terry
   builds from). `git pull` it before starting. **Do not do shared work on scratch branches** — they
   diverge and don't reach the headset.

> Detailed history lives in `docs/SESSION_LOG.md`. This file is the quick, always-current handshake.

---

## Working agreement
- **T-Dog** = gameplay / editor / XR rig / scenes / patchers / dev tools / on-device-facing fixes.
- **Architect** = backend C# / data model / economy / registries / tests (pure, CI-verified, no headset).
- **Gemini** = creative only (ships, factions, lore, art). **No repo access** — its docs come in via
  Terry and a coding chat files them (see `docs/storyboard/`).
- **Collision rule:** new files in your own lane = go. Editing a file in the other's lane, or a shared
  file (`ZiptideConstants.cs`, `WorldPackDefinition.cs`, build settings, `STATUS.md`) = **claim it in a
  Next-CLAIMED entry first.**
- **Branch:** one branch — `terry-local-wip`. Small commits. CI must stay green (if it goes red, warn
  Terry loudly and stop shipping unverified C# — see `CLAUDE.md`).

## Project state (current, 2026-06-16)
- **CI is GREEN on `terry-local-wip`.** Unity license fixed permanently (local `.ulf` in the
  `UNITY_LICENSE` secret; manual web activation is dead — `docs/RECOVERY_STEPS.md`).
- **Backbone built + CI-green** (Architect): `PlayerProfile` + `ProfileSerializer` + `SaveSystem`
  (unwired), generic `DefinitionRegistry<T>` + Resource/Tool/Machine/Plant/Creature/Biome/Recipe/
  BalanceConfig definitions, `IdleEngine`, `EconomyState` + `ProfileEconomy`, 24 EditMode tests.
- **Gameplay fixes + tools built + CI-green** (T-Dog): step-offset error fixed; **global fall-safety
  net** (any gravity world); audit-blocker root cause fixed (`StripRigFromWorldScene` self-heals world
  scenes); **Developer Warp system** (jump to any world/marker).
- **Superseded — do NOT continue:** the original "pod-loading seam / IPodLoader / walking skeleton"
  design. It predates the backbone above + `docs/design/SYSTEMS_ARCHITECTURE.md`. Build against the
  current data model, not the old plan.

---

## Resolved / standing notes
1. ✅ **Branch converged** — Architect moved to `terry-local-wip`; the
   `claude/architect-project-onboarding-2x7h60` fork is orphaned. **Never merge that fork** (stale,
   pre-backbone, duplicate Tests asmdef). One branch from here: `terry-local-wip`.
2. **No agent needs Unity.** Both cloud agents lack Unity/headset by design. Verify via **CI** — see
   the new **`docs/CI_VERIFY.md`** for the exact how-to (read the run conclusion via GitHub tools or
   `gh`; no Unity to install). This answers Architect's capability request below.

## 📌 RULE — claim before you build (so we never double up)
Before starting ANY task, add a `Next-CLAIMED` line here saying what you're about to do. Read the
other agent's latest `Next-CLAIMED` first. If it overlaps, pick something else. This is mandatory.

---

## ENTRIES (newest first)

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
