# ZIPTIDE — Shared Agent Session Log

Append-only coordination log for the two AI agents working this repo in parallel.
**Newest entries at the TOP.** Each entry: date, agent, lane, files touched, status, next.

**Agents**
- **TC** = Terry's Claude Code — headless backend lane (pure C#: data/economy/tests/tooling). CI-verified, no headset.
- **WL** = Wife's LLM — Unity-connected lane (scenes, patchers, XR rig, art, on-device verification).

**File-ownership boundary (how we avoid collisions)**
- **TC owns:** new `.cs` under new backend folders (e.g. `Gameplay/Runtime/Persistence`,
  `Content/Runtime/Definitions`), the `Ziptide.Tests` assembly, tooling scripts. *Additive* edits to
  `ZiptideConstants.cs` + `WorldPackDefinition.cs` — claim in this log before editing.
- **WL owns:** all `.unity` scenes, `Editor/Patching/*`, the XR rig + `_Boot`, art kits/materials,
  `ItemFactory.cs` / `InventoryState.cs`, and all on-device build + verify.
- **Rule:** pull before starting; small commits; if you must touch a file in the other's lane, log it here first.

---

> 📋 **Latest full handoff (read for complete context): [`docs/handoffs/2026-06-15_TC_to_TDog.md`](handoffs/2026-06-15_TC_to_TDog.md)**

## 2026-06-16 (c) — TDog: end-of-night state + open blockers for tomorrow
**Wins tonight:** CI dead→green; Unity Personal license permanently solved (local `.ulf` in secret,
manual activation is dead — see RECOVERY_STEPS); TC backbone merged + CI-green; storyboard hub +
Ships/Factions sync captured. **Did NOT get a build on device** (PC froze, then build failed).

**Open blocker 1 — build fails on `PatchScenesThenAPK`:** `dev_build_install.ps1` now uses the
self-validating build (`Editor/Build/BuildAndroid.cs`): patchers → `WorldAuditRunner.RunAll()` →
**throws + aborts if blockers > 0**, else builds APK. Last *committed* audit = 0 blockers, but that
predates the **D0_City regen**. Strong theory: the regen introduced an audit blocker (likely the
spawn-below-floor / stray-geometry that IS the lower-level fall glitch), so the build correctly
aborted. **Need to confirm:** tail of `Ziptide/Builds/android_build.log` (the thrown message) OR the
locally-rewritten `docs/AUDIT_REPORT.json`. Terry's local D0_City.unity + AUDIT_REPORT.{md,json} are
modified-uncommitted.

**Open blocker 2 — scene dumps still not generating.** Only `scene_dump_MilestoneA_GrabCube.txt`
exists (from 18:38). Running `Ziptide → Diagnostics → Dump` on `_Boot`/`D0_City` produced no files
(`git add docs/_generated` staged nothing twice). Project compiles (CI green + build ran patchers), so
it's not a compile error. Tomorrow: run it and watch the Console for `ZIPTIDE: SCENE_DUMP written to…`
and check `C:\Ziptide\docs\_generated\`. Dumps still gate the grab-feel/step-offset/fall-glitch fixes.

**Tomorrow's plan (TDog lane):** (1) get the build-log tail / AUDIT_REPORT.json → identify the D0_City
blocker (probably == fall glitch) and fix it (likely a `ScenePatcherD0` spawn-on-solid / remove stray
targets fix, my lane). (2) get the `_Boot` + `D0_City` dumps working → fix grab auto-orient
(Pistol `m_AttachTransform = null` is the cause) + the step-offset error. (3) then build + test the
checklist. **Architect:** decide on D0_City regen keep/revert once we see the audit; backbone wiring
(§5) still awaits Terry sign-off.

## 2026-06-16 (b) — TDog: storyboard hub + Ships/Factions sync (creative→code)
Shared **docs only** (additive, no code) — Terry is doing ship/faction art with Gemini; captured their
"Project Sync Doc V2" and bridged it to our architecture so we build it data-driven, not bespoke.
- `docs/storyboard/README.md` — the hub: overall spine + per-planet sub-storyboard convention + the
  creative→code pipeline (creative brings concept+why → coding agents translate to Definitions/
  Blueprints/interfaces → file under storyboard).
- `docs/storyboard/SHIPS_AND_FACTIONS.md` — Cal's Toxic City Scavenger ship + 3 alien faction pillars
  (Symbiotics/grown, Pragmatists/brutalist, Ethereals/acoustic) + atmospheric world-swap. Each mapped
  to existing systems: factions → `FactionDefinition` (Definition subclass) bundling kit/biome/surface/
  creature/economy ids; world-swap → `TravelCoordinator` + the ziptide veil; salvage → new Core
  interface `IInteractableSpaceJunk` (sibling to `IShockable`) feeding `ProfileEconomy`.
- **Architect note:** all design-now/build-later, gated behind the on-foot slice + save-wiring. When
  ships get built, the data model (FactionDefinition, IInteractableSpaceJunk) is your registry/Core
  lane; flag in this log before we start so we don't collide.

## 2026-06-16 — WL (T-dog lane): take the editor/headset gameplay lane
Second cloud agent Terry is running, picking up the WL/editor-gameplay lane (TC keeps the headless
backend lane). No collision: I was about to build Layer-0 SaveSystem but **stopped before writing any
code** when I saw TC already shipped it — dropped that work entirely.
- **Verified for the team (§6.1):** CI is ✅ green on tip `0993650` and on every TC backbone commit
  (`70eff60`→`0993650`). Backbone confirmed green from CI; in-editor Test Runner / `.meta` commit still
  need Terry on the PC (§6 items 2–6).
- **My lane:** the rig/gameplay fixes — step-offset error (`PlayerRigPersistence.TeleportToSpawnMarker`,
  every travel), grab-from-too-far + auto-orient feel, and the D0_City lower-level fall glitch. These
  need the **scene dumps** to fix precisely (CLAUDE.md: don't guess on XR-rig code) — walking Terry
  through generating them now.
- **Won't touch** TC-owned files: `Core/Runtime/Persistence/*`, `Core/Runtime/Economy/*`,
  `Content/Runtime/Definition*.cs`, `Content/Runtime/Definitions/*`, `Tests/*`.
- **Next:** Terry runs `Ziptide → Diagnostics → Dump Scene + Rig Config` on `_Boot`,
  `MilestoneA_GrabCube`, `D0_City` → pushes `docs/_generated/*` → I fix step-offset + grab + fall glitch.

## 2026-06-15 (cycle 1c) — TC: economy state + ProfileEconomy (idle applied to profile)
Pure C# in Core/Tests; no scenes/rig/boot. CI-green.
- `Core/Runtime/Economy/EconomyState.cs` — `MineState` (idle production accrues into `stored`, capped)
  + `PlotState` (time-based growth; `IsReady`/`GrowthProgress`). Both `[Serializable]`, round-trip safe.
- `WorldState` (in `PlayerProfile.cs`) gained `List<MineState> mines` + `List<PlotState> plots` (additive).
- `Core/Runtime/Economy/ProfileEconomy.cs` — `ResolveWorld(world, now, maxOffline)` advances mines via
  `IdleEngine`, flags ready plots, returns `WorldResolveResult` (welcome-back summary); `CollectMine`
  moves stored output into the profile balance.
- `Tests/EditMode/ProfileEconomyTests.cs` — 6 tests (mine accrual, cap+window, no-elapsed skip, plot
  ready/progress, collect, mines+plots round-trip). **Project total now 24 EditMode tests.**
- **Next (still pure/testable):** live-world tick MonoBehaviour for the active world (thin wrapper over
  this); resolve-on-world-entry wiring (report-only — needs Terry sign-off, touches travel/boot).

## 2026-06-15 (cycle 1b) — TC: backbone — registry + data model + idle engine
**Context:** T-dog (WL) offline; continued the consistency backbone. All pure C# in Content/Core/Tests
— **no scenes, no rig, no `_Boot`, no `ItemFactory`/`InventoryState`** (WL-owned). CI-green.

**New files (all additive, nothing existing modified except docs):**
- `Content/Runtime/Definition.cs` — **abstract base for ALL new content defs** (`id`, `displayName`,
  `DisplayName` fallback). Canonical going forward; legacy `ItemDefinition`/`WorldPackDefinition`
  keep their own `itemId`/`packId` (NOT retrofitted, to avoid asset breakage).
- `Content/Runtime/DefinitionRegistry.cs` — generic `DefinitionRegistry<TDef>`: id→def cache, dup-guard
  (keeps first, logs `ZIPTIDE: DUP_DEFINITION`), empty-id guard (`DEF_NO_ID`), and
  `EnsureLoadedFromResources(path)` auto-discovery via `Resources.LoadAll` (logs `REGISTRY_LOADED`).
  This is pillar #2 — drop an asset, no central list to edit → no merge collisions.
- `Content/Runtime/Definitions/` — 8 data types + 1 helper, all `Definition` subclasses with
  `[CreateAssetMenu]` under `Ziptide/Definitions/…`: `ResourceCost`, `ResourceDefinition`,
  `ToolDefinition` (+`ToolFunction` enum), `RecipeDefinition`, `MachineDefinition`, `PlantDefinition`,
  `CreatureDefinition` (+`CreatureArchetype` enum), `BiomeDefinition`, `BalanceConfig`.
- `Core/Runtime/Economy/IdleEngine.cs` — pure offline-accrual math (`Accrue`, `SecondsBetween`,
  `IdleAccrual` struct): rate×time, storage cap + overflow, `maxSeconds` window clamp. No Unity deps.
- `Tests/EditMode/DefinitionRegistryTests.cs` (5 tests) + `IdleEngineTests.cs` (6 tests).

**Recipes updated** (`docs/recipes/RECIPES.md`): Definition/Registry rows now point at the new base +
generic registry; added §2b "Backbone status — what's built" so neither of us reinvents it.

**Conventions established (please follow):** new content type = subclass `Definition` + `[CreateAssetMenu]`;
resolve via `DefinitionRegistry<TDef>`; put author-able assets under a `Resources/` subfolder for
auto-discovery; idle = pure math via `IdleEngine`.

**Not done (next):** concrete registries instantiated at boot; harvest/mine/garden runtimes; creature
behaviors on `DroneRuntime`/`IShockable`; wiring `SaveSystem` + idle resolve into `_Boot`/world-entry
(report-only — needs Terry sign-off).

## 2026-06-15 (cycle 1) — TC: consistency backbone
- **Model agreed with Terry:** feature/world parallelism (not backend-vs-headset). Daytime = blind
  build (no PC/Quest); evening = Terry tests + logs. Top priority = structural consistency so either
  agent can take over either feature. **READ `docs/recipes/RECIPES.md` FIRST** — it's the contract.
- **This cycle split:** TC = consistency backbone (recipes + persistence + registries/definitions);
  WL = Level 1 Toxic Venice polish (uses only existing systems → no collision). Next cycle = full
  feature parallel, both following the recipes.
- **Shipped:** `docs/recipes/RECIPES.md`; the ObjectiveBoard NRE fix; SaveSystem + PlayerProfile +
  `Ziptide.Tests` EditMode assembly (first real project tests — CI now actually runs tests).
- **WL action items:** (1) verify ObjectiveBoard fix on-device after next build; (2) the step-offset
  rig bug (`PlayerRigPersistence.cs:437`, every travel) is in your lane; (3) append your sessions here.

## 2026-06-15 — TC (setup + project comprehension)
- **Did:** Fast-forwarded Terry's local `terry-local-wip` `21672df`→`8ae4f35` (now in sync with WL's pushes).
  Confirmed GitHub connected (gh, account TerryMaloney). Confirmed **CI is GREEN** (runs 27583389616,
  27583754790) — license fix landed (UNITY_LICENSE secret updated 23:21 UTC with the local `.ulf`).
- **Fix (status: see git):** `ObjectiveBoard.cs` `CreateWorldSpaceText` NullReferenceException
  (called `AddComponent<RectTransform>()` after a Canvas already auto-created one → null → NRE).
  Fired on **every scene load**. → **WL: please verify on-device after next build.**
- **Open bug for WL (rig lane):** CharacterController "Step Offset must be ≤ height + radius·2" thrown
  from `PlayerRigPersistence.cs:437` (`TeleportToSpawnMarker`), every travel. Non-fatal but logs an error.
- **Local uncommitted (Terry's tree):** `D0_City.unity` full scene regen — **LEAVE IT** (Terry/WL verify
  intent in Unity before commit/discard); `TestRoom_WorldPack.asset` (2-line); `AUDIT_REPORT.md` (CRLF);
  `t.txt` (4.3MB Quest logcat, untracked, ignorable).
- **Next (TC):** Layer 0 — `PlayerProfile` + `SaveSystem` + `Ziptide.Tests` EditMode assembly (round-trip
  + idle-accrual tests). Pure C#, new files only. Will NOT wire into `_Boot`/`TravelCoordinator` without sign-off.
