# BACKLOG — prioritized task queue (single operator)

**The roadmap as a pullable queue.** Every task is tagged by **who can verify it** (see `ROLES.md`):
- **⚙CI** — you write it and self-verify via CI (compile + EditMode + audit + APK). Do these freely.
- **🔧UNITY** — you write the patcher/tool C#; **Terry runs a menu** to bake the scene/asset (queue it in
  `TERRY_RUNBOOK.md`).
- **🎮DEVICE** — acceptance is feel/geometry/perf on the **headset**; Terry tests and sends notes.
- Combos like **⚙CI→🎮** = you build + CI-verify the code, Terry confirms the feel.

Work phases **A→E** roughly in order; within a phase, sorted by leverage. **No forced "next task"** — take
the highest-leverage item you can move (prefer ⚙CI when Terry's away; batch 🔧/🎮 for when he's on). Mark
`[x]` done, `[~]` in-progress. Log each pickup in `HANDOFF.md`.

> *(History: tasks used to be `[A]`/`[T]` for the two-chat split. Retired — one operator now; the tag is
> the verification class, not a lane.)*

---

## Phase A — FIX & TIE TOGETHER (make the core loop solid end-to-end)
- [x] **⚙CI Wire `ProfileEconomy.ResolveWorld` into world-entry** — DONE, CI-green (`27a5cf1`): `ProfileEconomy.EnterWorld()` from `WorldRuntime.Start()` (keyed by scene name) + 3 tests. Idle/offline accrual runs on entry. *(welcome-back UI = 🎮; optional `BalanceConfig` offline cap = ⚙CI.)*
- [ ] **⚙CI `WorldRuntime.ValidatePackDefinition()`** — fail-loud on null `packId`/`sceneName`/jobs (`ZIPTIDE: PACK_VALIDATION_FAIL`).
- [ ] **⚙CI Extract magic numbers → `ZiptideConstants.Physics`/`.Visuals`** (fall limits, gun scale, etc.).
- [ ] **⚙CI PlayMode test scaffold** — `Tests/PlayMode/Ziptide.Tests.PlayMode.asmdef` + one `TravelCoordinator` round-trip test.
- [~] **🎮DEVICE Clear the device-test bug round** — rig input/anchor/rays, gun-drop, drone wall-collision, hammer grab, "two Toxic Citys", NOACTI, PvP bot/HUD all **shipped + CI-green**; root causes in `docs/systems/VR_RIG_GOTCHAS.md`. *Awaiting Terry's on-device confirm (`DEVICE_TEST_CHECKLIST.md`); one open item: "can't run in ToxicCity" — re-test; if still wall-blocked, widen streets.*
- [ ] **🎮DEVICE Verify the Phase-A wiring on device** — bounty pays + `ECON_RESOLVE` fires + travel-gating shows no false locks on world entry.
- [ ] **🔧UNITY One-time content gen + Rebuild Dev World Manifest** committed so the new worlds ship (`TERRY_RUNBOOK.md` §1).

## Phase B — HARDEN ARCHITECTURE (close the red-team risks before scaling)
- [ ] **⚙CI `ItemFactory` IL2CPP-safe** — Resources-path convention / explicit registry instead of reflection-ish lookup.
- [ ] **⚙CI `WorldAuditRunner` self-tests** — unit-test blocker logic + patcher idempotence assertions.
- [x] **⚙CI `WorldPackDefinition` flag fields** — DONE, CI-green (`381b702`): `flagsRequired`/`flagsGranted` + pure tested `WorldGating` (MeetsRequirements / FirstMissingRequirement / GrantWorldFlags) + wired `JobDirector.OnJobCompleted` to grant world flags (+ `WORLD_LOCKED` diagnostic). 11 tests. Unblocks faithful serialization of all 80. See `WORLD_DATA.md` §1.
- [~] **🎮DEVICE Enforce `flagsRequired` at the travel doors** — DONE in `WorldTravelStation` (`aa6ed89`, CI-green): unmet prereqs → LOCKED door (visible, not enterable), `TravelCoordinator` untouched. *Awaiting device-verify once a gated world exists. Follow-up (⚙CI): the `ProximityTravelTrigger` failsafe still bypasses the gate (harmless today — points only at the exit).* DispatchKiosk job-level gating is separate / not done.
- [ ] **⚙CI NarrativeSaveSystem / RILL persistence audit** — confirm flags actually save/load + RILL crosses scenes; wire if dead. *(Note: there is no `NarrativeSaveSystem` — flags live in `PlayerProfile.flags` via `SaveSystem`; audit that path.)*
- [ ] **⚙CI CI: run EditMode (+ new PlayMode once stable) on every push** — keep the gate honest.
- [ ] **⚙CI `AudioDirector` dispose-on-unload** — stop per-travel source leaks.
- [ ] **⚙CI Scene-patcher idempotence pass** — a re-run guard + an EditMode assertion that running a patcher twice doesn't duplicate objects.
- [ ] **⚙CI `#if DEBUG`-gate per-frame diagnostic logging** (`PlayerRigPersistence` ZLog) for device perf.

## Phase C — MASS-BUILD WORLDS (story-connected, graybox-fast, easy to rewrite)
- [ ] **🔧UNITY `WorldStubGenerator` (E2)** — menu tool: for each `CityLayoutDefinition`, copy/rename a patcher, create the scene, register in build settings, auto-create its `WorldPackDefinition` (spawn markers + travel exits from layout). *(unblocks all 80 — the single biggest force-multiplier.)*
- [ ] **⚙CI Apply `WORLD_FLOW_TEMPLATES.md`** — pick a flow archetype per world from the 80-catalog; author each world's `CityLayoutDefinition` (districts/heights/palette per `CITY_DESIGN`) — DATA only.
- [~] **⚙CI Story→WorldPack serialization** — `WORLD_DATA.md` shipped: record format + step-verb vocab + **W000–W012** + the Transmission fragment flag spec. *Remaining: serialize W013–W080 on-demand as each is built (§4 procedure).*
- [ ] **⚙CI + 🔧UNITY→🎮 The Transmission fragment system** (`THE_TRANSMISSION.md` §10; spec in `WORLD_DATA.md` §3) — **⚙CI**: add `FRAGMENT_T1-5_FOUND`/`TRANSMISSION_CLARITY_*` to `ZiptideFlags` + a `TransmissionProgress` service + fragment item id(s); the voice audio assets. **🔧/🎮**: the de-garble playback + recognition UI (Adaptive Audio). Endgame payoff depends on this.
- [ ] **⚙CI `DistrictDef` schema additions** for ground-floor/door/roof/palette control (from `CITY_DESIGN` P1/P2) — data fields only.
- [ ] **🔧UNITY→🎮 `CityBuilder` consumes the new fields** — ground floors, height-stepping, palette zoning, landmark silhouettes (`CITY_DESIGN` P0→P2). Regenerate scenes; Terry judges the look.
- [ ] **🔧UNITY→🎮 Per-biome kit variants** in `CityBuilder` (underground/exterior/void/underwater/pattern) so worlds vary, not just cities.
- [ ] **Goal check:** changing a few fields tweaks a world; swapping its `CityLayoutDefinition` + WorldPack rewrites it wholesale.

## Phase D — FINISH MODES
- [ ] **⚙CI Economy loops wired live** — mining/garden/conveyor produce into the live `PlayerProfile` (services exist; connect to the world-entry resolve).
- [ ] **⚙CI PvP netcode message-model finalize** + the `IPvpTransport` PUN2-adapter contract (no Photon import needed for the seam).
- [ ] **🔧UNITY→🎮 PvP Phase 3 — import Photon PUN2** (+ App ID), implement `IPvpTransport`, remote avatar (head+2 hands), room-code join, voice. *(needs Terry's PC.)*
- [ ] **🔧UNITY→🎮 Ship / world-select hub** — board ship, cockpit, fly-out replaces the travel door (the north star).
- [ ] **🔧UNITY→🎮 Economy UI/feel** — machines/garden/conveyor in-world placement + readouts.

## Phase E — CREATURES (incl. novel, evolution-tied behaviors)
- [ ] **⚙CI `CreatureDefinition` extensions** + a generic `CreatureZoneDef` (generalize `DroneZoneDef`) — DATA + stats/loot/archetype per the 80-catalog.
- [ ] **⚙CI→🎮 `CreatureBehavior` base** — generalize the `DroneRuntime.CombatDriven` seam (+ `IShockable`/`HitZones`) into a reusable behavior interface + a `CreatureRuntime` factory that attaches behavior by archetype.
- [ ] **⚙CI→🎮 Implement the 4 archetypes** — Swarmer (boids), WallCrawler (surface-stick raycast), Flyer (hover/dive/steal), Bruiser (charge).
- [ ] **⚙CI→🎮 Implement the novel behaviors** from `CREATURE_DESIGN.md` (evolution-justified, per-biome) — one or two per chapter as worlds come online.
- [ ] **⚙CI Per-creature data + loot tables** tied to biome/economy (drops feed the crafting loop).

---
*Sources: `CODE_SCORE.md` (blockers), the 2026-06-21 readiness-gap audit, `WORLD_FLOW_TEMPLATES.md`,
`CREATURE_DESIGN.md`, `ZIPTIDE_MASTER_BUILD_PLAN.md` (E0–E10). Keep this in sync with MASTER_CHECKLIST.*
