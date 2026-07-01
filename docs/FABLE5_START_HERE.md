# ▶ START HERE — the single-operator manual (read this first, then stop reading)

> **🎯 THE ROADMAP-OF-RECORD IS `docs/GAME_PLAN.md`** (graybox → shipped, milestones M0–M8) — read it
> after this file. The ACTIVE sprint's live state is **`docs/SPRINT.md`** (takeover prompt: "Read
> docs/SPRINT.md and continue"). **`docs/HOW_TO_CHANGE_ANYTHING.md` is the change-safety playbook —
> consult it BEFORE modifying any system.** *(Modularity sprint ✅ 2026-07-01: 11 story worlds W002–W012
> build from data every APK, audit-verified — record in `docs/sprints/`.)*

**You are THE operator building Ziptide. You own the whole project — data, backend, tests, editor
patchers, world/story content, docs. There is no second chat and no "lane" to pick.** Terry is your only
teammate: he's the **hands** for the things you physically can't do (run Unity, wear the headset). This
primer makes you productive without re-exploring the codebase (which burns tokens for ~nothing). Read this
+ the spine, then work.

> *(History: this was a two-chat project — "Architect" (data) + "T-Dog" (scenes). That split is retired;
> one operator now owns both. Old `*(T-Dog)*`/`*(Architect)*` credits in the logs are just historical
> attribution.)*

## What Ziptide is
A **Meta Quest VR game**, Unity 2022.3.62f3, URP, Android/IL2CPP/ARM64, OpenXR + XR Interaction Toolkit.
The Unity project is in the **`Ziptide/`** subfolder. You're a contract tech (Cal) on an alien gate
network ("the Ziptide") that's secretly a contained-universe prison — non-lethal/stun combat, all-ages,
story-deep (Halo wonder + Fallout worldbuilding). Full canon: `docs/storyboard/STORY_BIBLE.md` (+ the
identity layer `THE_TRANSMISSION.md`).

## The spine (the only "always read" set)
1. **`docs/HANDOFF.md`** — your session-to-session log. **Read the newest entries at session start; append
   what you did + what's next at the end.** (It used to be a cross-chat log; now it's your continuity notes.)
2. **`docs/MASTER_CHECKLIST.md`** — state of the build (BUILT / short / mid / long-term). Truth.
3. **`docs/FABLE5_BACKLOG.md`** — the prioritized task queue, tagged by *who can verify it* (see below).
4. **`docs/TERRY_RUNBOOK.md`** — everything queued that needs Terry's hands (Unity menus + headset).
5. **`docs/storyboard/STORY_BIBLE.md`** — locked meta + per-mechanic fiction (only when doing story/world work).

## The one distinction that actually matters: who can verify it
You can't run Unity or a headset — Terry can. So every piece of work is one of three classes (the backlog
tags each task this way):
- **⚙ CI — you do it AND self-verify.** All C# gameplay/editor code, world/job **data assets**, docs. Push,
  then read CI: it **compiles + runs EditMode tests + runs `WorldAuditRunner` + builds the Android APK**.
  Green = verified. This is most of the work; do it freely.
- **🔧 UNITY — you write the code; Terry runs a menu to bake it.** Scenes are authored by **patcher
  indirection**: you write/commit a C# `ScenePatcher*`/builder, Terry clicks `Ziptide → …` in the editor to
  generate the `.unity`/`.asset`, then commits it. You **never hand-edit scene YAML.** Queue the menu step
  in `TERRY_RUNBOOK.md`.
- **🎮 HEADSET — only Terry can confirm.** Rig/input/feel, walkability, combat balance, VR UI, perf. You
  spec it in code + data; Terry tests on-device and sends back ❌s + feel notes. See `DEVICE_TEST_CHECKLIST.md`.

## Your work loop
1. Pull `terry-local-wip` (`git pull --rebase`). 2. Pick the highest-leverage backlog task (any class).
3. Write code/data/docs. 4. Push; **confirm CI green** (if red, warn Terry loudly and stop shipping C# —
see `CLAUDE.md`). 5. For any 🔧/🎮 step, **append it to `TERRY_RUNBOOK.md`** so Terry can batch it. 6. Log
it in `HANDOFF.md`; update `MASTER_CHECKLIST.md` if state changed.

## Current state snapshot (2026-07-01)
- **CI green** on `terry-local-wip`. Code score **3.5/5** (`CODE_SCORE.md`). One branch, single operator.
- **Built:** core loop (job→bounty→profile), economy core **now wired into world-entry** (`ECON_RESOLVE`),
  **story-flag gating** (`WorldPackDefinition.flagsRequired/Granted` + `WorldGating` + travel-door lock),
  **PvP solo+bot** (4 mechanics), **ToxicCity** world + the reusable **CityBuilder**, drones+combat v1, gear
  (taser/gravity/pistol/wrist-scanner/hammer), CI APK build, the full **80-world story design** +
  `WORLD_DATA.md` serialization (W000–W012).
- **3 rounds of device-test fixes shipped + CI-green** (don't re-derive these — see `HANDOFF.md` uu/vv +
  `docs/systems/VR_RIG_GOTCHAS.md`): the persistent thumbstick-rotates-gun bug (real field
  `m_AllowAnchorControl`), turn-while-holding (right hand), rays shortened + force-grab, stuck-slow-walk
  self-heal, gravity-gun no-self-hit, PvP bot wall-collision, visible dodgeable bot bolt, per-brick
  breakable walls, credits HUD, wrist-locator hand resolution, drone "lifelike" feel, legacy-D0 gun snap.
  **All awaiting on-device confirmation** — that's the open loop, not new code.
- **Pending Terry:** the full **device-test pass** (`DEVICE_TEST_CHECKLIST.md` + `TERRY_RUNBOOK.md`) — a lot
  is CI-green but not yet headset-verified. His ❌s/feel-notes drive the next round.
- **Not yet:** `WorldStubGenerator` (mass worlds) · 3 of 4 creature archetypes · Photon netcode (PvP Ph.3) ·
  79 of 80 worlds buildable · Transmission fragment system.

## Roadmap-of-record: `docs/GAME_PLAN.md` (milestones M0–M8, dependency-ordered)
**M0 device-proof → M1 story speaks → M2 the job is real → M3 living worlds → M4 the ship → M5 content
at scale → M6 look & sound → M7 modes → M8 ship it.** Each milestone runs as a sprint (`docs/SPRINT.md` =
live state); `FABLE5_BACKLOG.md` holds the current milestone's expanded queue. Prefer ⚙ CI work when
Terry's away; batch 🔧/🎮 for when he's on. *(The old A–E phase list is retired into GAME_PLAN's milestones.)*

## Definition of Done (every task)
CI-green (compile + EditMode + audit) · for 🔧/🎮 work, **Terry-verified** · a HANDOFF entry appended ·
MASTER_CHECKLIST updated if state changed. **Never claim "done" on unverified C#.**

## Don't re-explore — the file map (key systems → paths)
- **Boot/travel:** `Gameplay/Runtime/World/BootLoader.cs`, `TravelCoordinator.cs` · constants `Core/Runtime/ZiptideConstants.cs`, flags `ZiptideFlags.cs`
- **Economy/save:** `Core/Runtime/Economy/{IdleEngine,ProfileEconomy}.cs` (`EnterWorld` = world-entry resolve), `Core/Runtime/Persistence/PlayerProfile.cs`, `Gameplay/Runtime/Persistence/SaveSystem.cs`
- **Jobs/bounty/gating:** `Content/Runtime/Jobs/{JobDefinition,JobRewards}.cs`, `Content/Runtime/WorldPacks/{WorldPackDefinition,WorldGating}.cs`, `Gameplay/Runtime/Jobs/JobDirector.cs`, travel-lock in `Gameplay/Runtime/WorldTravelStation.cs`
- **Worlds:** data `Content/Runtime/City/CityLayoutDefinition.cs` + `WorldPacks/WorldPackDefinition.cs`; geometry `Editor/Patching/CityBuilder.cs`; reference patcher `ScenePatcherToxicCity.cs`; recipe `docs/systems/WORLD_BLUEPRINT.md`; flow `docs/design/{CITY_DESIGN,WORLD_FLOW_TEMPLATES}.md`; serialization `docs/storyboard/WORLD_DATA.md`
- **Creatures:** `Gameplay/Runtime/Enemies/{DroneRuntime,DroneCombatBehavior,HitZones}.cs`, `Core/Runtime/IShockable.cs`, data `Content/Runtime/Definitions/CreatureDefinition.cs`; framework `docs/systems/CREATURE_DESIGN.md`
- **PvP:** `Multiplayer/Runtime/{PvpRules,PvpMatch,PvpCombatant}.cs` + `Net/{PvpNet,LoopbackPvpTransport}.cs`; scene side `Gameplay/Runtime/Pvp/`; plan `docs/design/PVP_1V1_MODE.md`
- **Gear:** `Gameplay/Runtime/Weapons/*` + `Items/ItemFactory.cs`; ideas `docs/09_GEAR_AND_TOOLS.md`
- **VR rig (live config) + KNOWN GOTCHAS:** `Gameplay/Runtime/Player/PlayerRigPersistence.cs` →
  `EnsureXRIWiring()` is the single source of truth for the runtime rig. **Before touching the rig,
  weapons, grab, or drone movement, read `docs/systems/VR_RIG_GOTCHAS.md`** — root causes + working fixes
  for the bugs that ate multiple rounds.
- **Editor menus (🔧 Terry runs these):** `Editor/Patching/*` + `Editor/DevTools/*` — see `docs/ROLES.md`
  for the full menu list + the patcher-indirection recipe.
- **Build/CI:** `Editor/Build/BuildAndroid.cs`, `Editor/Audit/WorldAuditRunner.cs`, `.github/workflows/ci.yml`, `tools/dev_build_install.ps1`
- **Story:** `docs/storyboard/` (STORY_BIBLE + THE_TRANSMISSION + `_WORLD_TEMPLATE` + `CHAPTER_*` 80-world catalog); `WORLD_DATA.md` = prose→`WorldPackDefinition` serialization; canon table `docs/ZIPTIDE_MASTER_BUILD_PLAN.md` §12

*If it's not here, check `MODULE_MAP.md`. Prefer reading the doc over grepping the code.*
