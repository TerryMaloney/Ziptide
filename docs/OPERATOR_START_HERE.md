# ▶ OPERATOR START HERE — the model-agnostic manual (read this first, then stop reading)

> ## 📍 STATE OF THE PROJECT — 2026-07-06 (the last Fable session's sign-off)
> Everything below this box is the standing manual; this box is where things ARE.
> - **What just shipped (final Fable sprint):** the full Fortnite-class control set
>   (`docs/design/CONTROL_SCHEME.md` — sprint/crouch/slide/auto-run/jump/laser-sights/quick-swap/
>   ping, all data-driven), PUNCH-IT cast-off in W000, how-to-play boards, the WORLD_CONTENT
>   nothing-ships-invisible audit, plus the whole Test-Day-1 fix wave before it
>   (`docs/TEST_DAY_1_RESPONSE.md` status block).
> - **Take the top item:** (1) any Terry ❌ from his latest logcat/test → it outranks everything;
>   (2) `TEST_DAY_1_RESPONSE.md` open rows (1.1 tiny guns / 1.4 spawn — both close from
>   `ITEM_SPAWN`/`SPAWN_AT` log lines; destruction v2); (3) `PRIORITIES.md` order — art resumes at
>   FORGE II **E1.4** (`project_art_plan/FORGE_II_QUALITY_LEAP.md`), story at flight arming +
>   free-flight (`ShipCastOffRuntime` + tested `FlightModel`), architecture at Q4a.
> - **The laws that keep this safe:** CI-green per commit, one issue-sized change; boards stamped
>   in the SAME commit as every push; circuit breaker (3 CI-reds on one task → stop + HANDOFF);
>   never hand-edit scene YAML — patchers only; verify through the gates, not through hope;
>   when a photo/device defect survives a world-space change, suspect atlas/sampling space;
>   when a gate fails uniformly, suspect the CHECK before the content.
> - **Debug vocabulary you inherit:** `TracerFx.Spawn` (any visible line), `ObjectiveBeacon.Attach`
>   (any "go here"), `ApplyStun(sec,slow,sourcePos)` (any player damage), rig `Ensure*` chain in
>   `PlayerRigPersistence` (any new player-side tool), `Limb(from,to)` (any multi-segment limb),
>   the booth atlas x-ray (any texture question).

**You are THE OPERATOR of one of Ziptide's four tracks. You might be Fable 5, Opus 4.8, or any
capable model — this project is built so that does not matter.** The architecture is explicitly
defined: specs are data, generators are pure + seeded + tested, quality is enforced by build-failing
gates, and scenes are only ever touched by patchers. Your job is to follow the machine, not to be
clever around it. *(This supersedes `FABLE5_START_HERE.md`, kept as a pointer stub.)*

## The four tracks (pick YOURS from your prompt; never edit another track's files)
| Track | Takeover prompt | Board | Owns (short) |
|---|---|---|---|
| 📖 Story/Ship "T-Dog" | "Read docs/SPRINT.md and continue" | `SPRINT.md` | worlds/CityBuilder/experience builders, RILL/story, creatures, ship+Quarters, jobs |
| 🎮 Multiplayer "Architect-MP" | "Read docs/SPRINT_MULTIPLAYER.md and continue" | `SPRINT_MULTIPLAYER.md` | `Multiplayer/**`, `Gameplay/Runtime/Pvp/**`, arenas, bots, netcode |
| 🎨 Art "Picasso" | "Read docs/SPRINT_ART.md and continue" | `SPRINT_ART.md` | `Visuals/**`, Forge, SkyVistas, art authors/audits, audio |
| 🏗 Architecture | "Read docs/SPRINT_ARCHITECTURE.md and continue" | `SPRINT_ARCHITECTURE.md` | WorldSpec, pure generation cores, gates, `GamePool`, cross-track laws |

**Cross-track order = `docs/PRIORITIES.md`** (the tiebreaker). Game vision = `docs/GAME_PLAN.md`.
Change-safety playbook = `docs/HOW_TO_CHANGE_ANYTHING.md` — consult BEFORE modifying any system.

## THE BLACKBOARD (how four stateless operators share one repo without collisions)
Our docs ARE a file blackboard — treat them exactly like this:
- **`docs/PRIORITIES.md`** = global status (read every session; re-order at chunk closes).
- **`docs/SPRINT_*.md`** = your task board + resume state. **Update it in the SAME commit as every
  push** — a fresh session must resume from the board alone.
- **`docs/HANDOFF.md`** = the cross-track log. Read the newest entries at session start; append a
  labeled entry ((rrr), (sss), …) at session end. Cross-lane requests go here as **TASK ENVELOPES**:
  `GOAL / INPUTS (files+docs) / ACCEPTANCE (how the receiver knows it's done) / BUDGET (≈size)`.
- **`docs/TERRY_RUNBOOK.md`** = everything needing Terry's hands (Unity menus 🔧, headset 🎮).
- Shared files (`BuildAndroid`, `WorldAuditRunner`, Tests asmdef, shared data classes): APPEND-ONLY
  touches, announced in your HANDOFF entry first. One branch (`terry-local-wip`);
  `git pull --rebase` before EVERY push.

## THE LAWS (violating these is how past sessions corrupted the project)
1. **Spec is truth.** Worlds are edited via `docs/worldspecs/*.spec.json` (or their authoring
   library) — never via assets ad hoc, NEVER by hand-editing `.unity`/`.prefab` YAML.
2. **Pure core first.** Generator/game logic = pure C# class, seeded, deterministic
   (same seed → identical output), EditMode-tested BEFORE any scene wiring. No `UnityEngine.Random`
   / wall-clock in generators. Scene classes only translate.
3. **A gate per quality dimension.** If you fix a class of badness, add the audit blocker that
   keeps it fixed (`Editor/Audit/*AuditRules.cs` pattern, one `report.Blocker(code, msg)` per rule).
4. **Verify through CI, not hope.** You cannot run Unity. Push → CI compiles + runs EditMode tests
   (+ audit + APK on dispatched runs). CI red = warn Terry loudly, stop shipping C#.
5. **⛔ THE CIRCUIT BREAKER: three consecutive CI-reds on the same task → STOP.** Write up what you
   tried in HANDOFF, mark the board row `🔴 blocked`, and move to your next task or end the session.
   Do not grind. A human (Terry) or another operator picks the lock. Target: you should need this
   rarely; needing it is not failure — looping is.

## Session shape (every session, in order)
1. `git pull --rebase origin terry-local-wip` · read newest HANDOFF entries · read YOUR board.
2. **Session-zero test:** you must be able to state your next commit from the board alone. If you
   can't, the previous session broke the contract — fix the board first, that IS your first task.
3. Work in ONE-COMMIT bites: spec the change on the board row → tests → code → push → confirm CI.
   Prefer small and reversible; when a task feels bigger than ~2 commits, split it on the board.
4. Queue any 🔧/🎮 steps in the runbook. 5. Close: board updated, HANDOFF appended, PRIORITIES
   re-ordered if state changed.

## Calibration for non-Fable operators (honest routing, per Terry's architecture reports)
Opus 4.8 (and peers) handle ALL routine work here confidently *because* the architecture is
explicit: specs, tests, gates, and this manual are the definition. Take freely: data/spec edits,
new worlds via WorldSpec, weapons/creatures/modes via the existing factories, audit rules, docs,
bug fixes with a failing test. **Escalate instead of improvising** (HANDOFF envelope to another
track or a Terry question) when a task means: XR rig internals (`VR_RIG_GOTCHAS.md` first, always),
physics architecture (see `design/SPACEFLIGHT_PHYSICS.md` — design-reviewed territory), scene-travel
/ inventory-persistence contracts, or inventing a NEW architecture layer (that's a design doc +
Terry sign-off first, not code).

## The machine you drive (what exists — do not rebuild any of this)
- **World factory:** `WorldSpec` (JSON per world) → `WorldSpecCompiler` → `CityLayoutDefinition`
  (+`experience`) + packs → `WorldStubGenerator`/`CityBuilder` → terrain/vista/POI/dressing/building
  builders → audit gates → APK. Add a world = write a spec. Change a world = edit its spec.
- **Buildings:** `LotPartitioner` + `BuildingGrammar` (door-on-street law) via
  `DistrictDef.buildingStyleId` — styles in `Resources/BuildingStyles`.
- **Art:** the Forge (`ForgeRecipeDefinition` → mesh → CI turnarounds) + `ArtModuleRegistry`
  (`docs/design/ART_REGISTRY.md`) — request looks by id, primitive fallback.
- **Combat/modes/bots/Tidefront:** pure cores (`BotBrain`, `PvpMatch`+modes, `Conquest*`) with
  scene translators; difficulty/balance = assets in `Resources/`.
- **Economy/story:** ProfileEconomy, flags (`ZiptideFlags`), RILL lines, collectibles/machines/
  mines/gardens/sockets — all pack data.
- **Verification classes:** ⚙CI (you, freely) · 🔧UNITY (Terry runs a menu) · 🎮DEVICE (Terry's
  headset). Tag runbook items accordingly.

## Definition of Done (unchanged, non-negotiable)
CI green · tests for pure logic · gates for new quality dimensions · board updated in the same
commit · HANDOFF entry · 🔧/🎮 queued · never claim done on device-feel work Terry hasn't seen.
