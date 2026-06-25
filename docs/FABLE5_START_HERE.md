# ▶ FABLE 5 — START HERE (read this first, then stop reading)

**You are one of two AI chats building Ziptide. This primer gets you productive without re-exploring the
codebase (which burns tokens for ~nothing). Read this + the 3-doc spine, claim a task, go.**

## What Ziptide is
A **Meta Quest VR game**, Unity 2022.3.62f3, URP, Android/IL2CPP/ARM64, OpenXR + XR Interaction Toolkit.
The Unity project is in the **`Ziptide/`** subfolder. You're a contract tech (Cal) on an alien gate
network ("the Ziptide") that's secretly a contained-universe prison — non-lethal/stun combat, all-ages,
story-deep (Halo wonder + Fallout worldbuilding). Full canon: `docs/storyboard/STORY_BIBLE.md`.

## The 3-doc spine (the only "always read" set)
1. **`docs/HANDOFF.md`** — cross-chat log. **Read newest entries at session start; append yours at end.**
2. **`docs/MASTER_CHECKLIST.md`** — state of the build (BUILT / short / mid / long-term). Truth.
3. **`docs/storyboard/STORY_BIBLE.md`** — the locked meta + per-mechanic fiction (only when doing story/world work).

## Your role (pick one — see `docs/ROLES.md` for the full charter)
- **Architect** = backend C# / data model / economy / tests / netcode message-model + **world DATA**
  (story→WorldPack, flow templates, creature stats/loot). **No headset; verify via CI.**
- **T-Dog** = scenes / VR rig / editor / patchers / on-device + **city geometry** + creature **runtime
  behavior** + feel. **Owns device verification.**
The lanes are collision-free by design. **One branch (`terry-local-wip`)**; `git pull --rebase` before
work; **claim a task in HANDOFF before starting**; if you're rate-limited, the other chat keeps going.

## Current state snapshot (2026-06-21)
- **CI green** on `terry-local-wip`. Code score **3.5/5** (see `CODE_SCORE.md`).
- **Built:** core loop (job→bounty→profile), economy core (idle/profile, pure+tested), **PvP solo+bot**
  (4 mechanics), **ToxicCity** world + the reusable **CityBuilder** blueprint, drones+combat v1, gear
  (taser/gravity/pistol/wrist-scanner/hammer), CI APK build, the full **80-world story design**.
- **NOT yet:** `ProfileEconomy.ResolveWorld` wired on world-entry · `WorldStubGenerator` (mass worlds) ·
  3 of 4 creature archetypes · Photon netcode (PvP Phase 3) · 79 of 80 worlds buildable.
- **Device test pass pending** — see `STATUS.md` ▶ runbook (one-time Unity menus → Rebuild Dev World Manifest → build/sideload).

## Your roadmap-of-record (do in order; pull tasks from `FABLE5_BACKLOG.md`)
**A Fix & tie together → B Harden architecture → C Mass-build worlds from data → D Finish modes → E Creatures.**
Phase A starts with the `CODE_SCORE.md` critical-path blockers (esp. wire `ProfileEconomy.ResolveWorld`).

## Definition of Done (every task)
CI-green (compile + EditMode tests) · for runtime/scene work, **device-verified by Terry** · a HANDOFF
entry appended · MASTER_CHECKLIST updated if state changed. Never claim "done" on unverified C# — if CI
goes red, warn Terry loudly and stop shipping (see `CLAUDE.md`).

## Don't re-explore — the file map (key systems → paths)
- **Boot/travel:** `Gameplay/Runtime/World/BootLoader.cs`, `TravelCoordinator.cs` · constants `Core/Runtime/ZiptideConstants.cs`, flags `ZiptideFlags.cs`
- **Economy/save:** `Core/Runtime/Economy/{IdleEngine,ProfileEconomy}.cs`, `Core/Runtime/Persistence/PlayerProfile.cs`, `Gameplay/Runtime/Persistence/SaveSystem.cs`
- **Jobs/bounty:** `Content/Runtime/Jobs/{JobDefinition,JobRewards}.cs`, `Gameplay/Runtime/Jobs/JobDirector.cs`
- **Worlds:** data `Content/Runtime/City/CityLayoutDefinition.cs` + `WorldPacks/WorldPackDefinition.cs`; geometry `Editor/Patching/CityBuilder.cs`; reference patcher `ScenePatcherToxicCity.cs`; recipe `docs/systems/WORLD_BLUEPRINT.md`; flow `docs/design/{CITY_DESIGN,WORLD_FLOW_TEMPLATES}.md`
- **Creatures:** `Gameplay/Runtime/Enemies/{DroneRuntime,DroneCombatBehavior,HitZones}.cs`, `Core/Runtime/IShockable.cs`, data `Content/Runtime/Definitions/CreatureDefinition.cs`; framework `docs/systems/CREATURE_DESIGN.md`
- **PvP:** `Multiplayer/Runtime/{PvpRules,PvpMatch,PvpCombatant}.cs` + `Net/{PvpNet,LoopbackPvpTransport}.cs`; scene side `Gameplay/Runtime/Pvp/`; plan `docs/design/PVP_1V1_MODE.md`
- **Gear:** `Gameplay/Runtime/Weapons/*` + `Items/ItemFactory.cs`; ideas `docs/09_GEAR_AND_TOOLS.md`
- **VR rig (live config) + KNOWN GOTCHAS:** `Gameplay/Runtime/Player/PlayerRigPersistence.cs` →
  `EnsureXRIWiring()` is the single source of truth for the runtime rig. **Before touching the rig,
  weapons, grab, or drone movement, read `docs/systems/VR_RIG_GOTCHAS.md`** — it has the root causes +
  working fixes for the bugs that ate multiple rounds (thumbstick rotates gun, rays too long, right-stick
  moves you, guns float on release, drones/bolts phase through walls, ungrabbable objects).
- **Build/CI:** `Editor/Build/BuildAndroid.cs`, `Editor/Audit/WorldAuditRunner.cs`, `.github/workflows/ci.yml`, `tools/dev_build_install.ps1`
- **Story:** `docs/storyboard/` (STORY_BIBLE + `_WORLD_TEMPLATE` + `CHAPTER_*` 80-world catalog); canon table `docs/ZIPTIDE_MASTER_BUILD_PLAN.md` §12

*If it's not here, check `MODULE_MAP.md`. Prefer reading the doc over grepping the code.*
