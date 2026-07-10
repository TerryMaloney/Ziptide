# ▶ OPERATOR START HERE — the model-agnostic manual (read this first, then stop reading)

> ## 📍 STATE OF THE PROJECT — 2026-07-10 (the Fable-5 endgame sign-off)
> **THE RIVER (follow it, in order):** this file (laws + DoD) → **`docs/EXCELLENCE_MAP.md`** =
> every aspect's honest state (⬜/🦴 = not built, 🕳️ = missing gate — that IS the "what's left"
> list) → **`docs/PRIORITIES.md` rev 9** = the order, split FABLE-ONLY / OPUS-READY / TERRY →
> your lane's SPRINT board → `docs/HANDOFF.md` newest entries. Ship path: `META_STORE_READINESS.md`.
> - **Where things stand:** ~3 sessions left per Fable operator (reserve some for Terry's headset
>   run). The meta layer is DONE and CI-enforced: richness law, DoD, 13+ gates (leaks, events,
>   saves-atomic, travel pre-flight, scene coverage, never-silent audio, Prospect rubric, story
>   beats, board staleness). Tidefront complete pre-Photon; skyscape W005 awaits device verdict;
>   runtime vitals + the travel janitor watch everything.
> - **Hardest unbuilt (Fable-only):** tutorial design · interiors translator (1.3) · async-travel
>   design · comfort presets 0.4. Everything else is OPUS-READY against the map + boards.
> - 🔌 Wiring truth: `docs/WIRING_MAP.md` + `BOARD_INDEX.md` (audited 2026-07-06, still holds).

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
6. **🎨 THE RICHNESS BAR (Terry, 2026-07-10).** Terry's device verdict: too much is landing at ~10%
   of its budget — boxy, skeletal, "one primitive per idea." The bar, for EVERY lane (creatures,
   gardens, vehicles, machines, props, tables, missions — everything):
   - **Shape:** if a class budget says 10k, a hero asset should USE most of it. One box/sphere per
     concept is a placeholder, not a ship. Multi-part silhouettes, asymmetry, the Forge's full op
     vocabulary (capsule/frustum/torus/sweep/blob + taper/bend/noise) — not stacked cubes.
   - **Vocabulary breadth:** if a catalog has 8 entries, the PLAYER should meet 8, not 2. Shipping a
     system with 20% of its own data surfaced is a skeleton wearing a coat.
   - **Motion & mechanics:** the same rule for behavior — one idle bob is not a behavior set. Each
     thing that moves gets a movement vocabulary (variants, states, reactions), each mechanic its
     full verb set from its design doc.
   - Placeholder-first is STILL the law for unblocking mechanics — but a placeholder left in a
     shipped surface is now a board row, not a shrug. When you ship a v1, list its thin spots in
     HANDOFF so the fattening pass is claimable.

## ✅ DEFINITION OF DONE (mechanical — check EVERY box or the chunk is not done)
No judgment calls here; that's the point. A chunk ships when ALL of these are true:
1. **Core:** pure C# + EditMode tests pushed BEFORE/WITH the MonoBehaviour translator (LAW 2).
2. **Gate:** the aspect's guardrail exists and covers the change — check the aspect's row in
   **`docs/EXCELLENCE_MAP.md`**. If the row says 🕳️ GAP, either close the gap in this chunk or
   claim it as a named board row. Never ship into an ungated aspect silently.
3. **Richness (LAW 6):** shape uses its budget · the catalog the change touches is surfaced, not
   sampled · anything that moves has a movement vocabulary. Thin spots listed in HANDOFF as
   claimable rows.
4. **Save story:** if the player can change/earn it, it survives quit (overlay idiom) — or the
   HANDOFF says explicitly why not.
5. **Diagnostics:** `ZIPTIDE: <TAG>` logs on every new runtime behavior (the logcat contract).
6. **CI green** on the push that contains the change (LAWS 4–5).
7. **Blackboard:** board row updated · HANDOFF entry (Did/Next/Heads-up/Commits) · Terry's runbook
   gets the 🔧 menu step + 🎮 feel pass with CONCRETE knob values to react to.
8. **Map:** if the aspect's STATE changed, its EXCELLENCE_MAP row is updated in the same push.

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
