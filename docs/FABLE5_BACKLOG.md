# FABLE 5 — BACKLOG (lane-tagged, pullable)

**The roadmap as a task queue.** `[A]` = Architect (data/backend/tests), `[T]` = T-Dog (scene/runtime/
on-device). Tasks within a phase are sorted by leverage; **each task is single-lane** so the two chats
never touch the same files — when one is rate-limited the other pulls the next task in its lane. Claim in
`HANDOFF.md` before starting. DoD = CI-green + (runtime) device-verified + HANDOFF entry. Mark `[x]` done.

> Do phases in order (A→E), but **both lanes work the current phase in parallel.** Don't run phases ahead blind.

---

## Phase A — FIX & TIE TOGETHER (make the core loop solid end-to-end)
- [ ] **[A] Wire `ProfileEconomy.ResolveWorld` into world-entry** — call it from `WorldRuntime`/post-travel so idle/welcome-back actually runs. *(highest leverage — the missing economy link)*
- [ ] **[A] `WorldRuntime.ValidatePackDefinition()`** — fail-loud on null `packId`/`sceneName`/jobs (`ZIPTIDE: PACK_VALIDATION_FAIL`).
- [ ] **[A] Extract magic numbers → `ZiptideConstants.Physics`/`.Visuals`** (fall limits, gun scale, etc.).
- [ ] **[A] PlayMode test scaffold** — `Tests/PlayMode/Ziptide.Tests.PlayMode.asmdef` + one `TravelCoordinator` round-trip test.
- [~] **[T] Clear the device-test bug round** (kk + ll entries) — rig input/anchor/rays, gun-drop, drone
  wall-collision, hammer grab, "two Toxic Citys", NOACTI, PvP bot/HUD all **shipped + CI-green**; root
  causes captured in `docs/systems/VR_RIG_GOTCHAS.md`. *Awaiting Terry's on-device confirm; one open
  item: "can't run in ToxicCity" (input bug now fixed — re-test; if still wall-blocked, widen streets).*
- [ ] **[T] Verify the Phase-A wiring on device** — bounty pays + idle resolves on world entry after [A] lands.
- [ ] **[T] One-time content gen + Rebuild Dev World Manifest** committed so the new worlds ship (STATUS runbook).

## Phase B — HARDEN ARCHITECTURE (close the red-team risks before scaling)
- [ ] **[A] `ItemFactory` IL2CPP-safe** — Resources-path convention / explicit registry instead of reflection-ish lookup.
- [ ] **[A] `WorldAuditRunner` self-tests** — unit-test blocker logic + patcher idempotence assertions.
- [ ] **[A] `WorldPackDefinition` flag fields** — add `List<string> flagsRequired`/`flagsGranted` (the SO lacks them; `ZiptideFlags`' header wrongly claims they exist). `WorldRuntime` gates entry on required + sets granted via `NarrativeSaveSystem` on complete. *Unblocks faithful serialization of all 80 (multi-flag worlds — `completionFlag` only carries one). See `WORLD_DATA.md` §1.*
- [ ] **[A] NarrativeSaveSystem / RILL persistence audit** — confirm flags actually save/load + RILL crosses scenes; wire if dead.
- [ ] **[A] CI: run EditMode (+ new PlayMode once stable) on every push** — keep the gate honest.
- [ ] **[T] `AudioDirector` dispose-on-unload** — stop per-travel source leaks.
- [ ] **[T] Scene-patcher idempotence pass** — running a patcher twice must not duplicate objects (manual + a guard).
- [ ] **[T] `#if DEBUG`-gate per-frame diagnostic logging** (`PlayerRigPersistence` ZLog) for device perf.

## Phase C — MASS-BUILD WORLDS (story-connected, graybox-fast, easy to rewrite)
- [ ] **[T] `WorldStubGenerator` (E2)** — menu tool: for each `CityLayoutDefinition`, copy/rename a patcher, create the scene, register in build settings, auto-create its `WorldPackDefinition` (spawn markers + travel exits from layout). *(unblocks all 80)*
- [ ] **[A] Apply `WORLD_FLOW_TEMPLATES.md`** — pick a flow archetype per world from the 80-catalog; author each world's `CityLayoutDefinition` (districts/heights/palette per `CITY_DESIGN`) — DATA only.
- [~] **[A] Story→WorldPack serialization** — `docs/storyboard/WORLD_DATA.md` shipped: record format + legal step-verb vocabulary + **W000–W012 serialized in full** (the proven pattern) + the Transmission fragment flag spec. *Remaining: serialize W013–W080 on-demand as each is built (§4 procedure); blocked on the schema-gap task below for faithful multi-flag gating.*
- [ ] **[A]+[T] The Transmission fragment system** (`storyboard/THE_TRANSMISSION.md` §10; flag spec + cadence now in `WORLD_DATA.md` §3) — `[A]` add the `FRAGMENT_T1-5_FOUND`/`TRANSMISSION_CLARITY_*` block to `ZiptideFlags` + a `TransmissionProgress` service that derives the clarity tier from collected fragments + the fragment item id(s); the ambiguous/partner voice audio assets; `[T]` the de-garble playback + recognition UI (ties to the Adaptive Audio layer). The endgame's emotional payoff depends on this.
- [ ] **[A] `DistrictDef` schema additions** for ground-floor/door/roof/palette control (from `CITY_DESIGN` P1/P2) — data fields only.
- [ ] **[T] `CityBuilder` consumes the new fields** — ground floors, height-stepping, palette zoning, landmark silhouettes (the `CITY_DESIGN` P0→P2 passes).
- [ ] **[T] Per-biome kit variants** in `CityBuilder` (underground/exterior/void/underwater/pattern) so worlds vary, not just cities.
- [ ] **Goal check:** changing a few fields tweaks a world; swapping its `CityLayoutDefinition` + WorldPack rewrites it wholesale.

## Phase D — FINISH MODES
- [ ] **[A] Economy loops wired live** — mining/garden/conveyor produce into the live `PlayerProfile` (services exist; connect to world-entry resolve).
- [ ] **[A] PvP netcode message-model finalize** + the `IPvpTransport` PUN2-adapter contract (no Photon import needed for the seam).
- [ ] **[T] PvP Phase 3 — import Photon PUN2** (+ App ID), implement `IPvpTransport`, remote avatar (head+2 hands), room-code join, voice. *(needs Terry's PC)*
- [ ] **[T] Ship / world-select hub** — board ship, cockpit, fly-out replaces the travel door (the north star).
- [ ] **[T] Economy UI/feel** — machines/garden/conveyor in-world placement + readouts.

## Phase E — CREATURES (incl. novel, evolution-tied behaviors)
- [ ] **[A] `CreatureDefinition` extensions** + a generic `CreatureZoneDef` (generalize `DroneZoneDef`) — DATA + stats/loot/archetype per the 80-catalog.
- [ ] **[T] `CreatureBehavior` base** — generalize the `DroneRuntime.CombatDriven` seam (+ `IShockable`/`HitZones`) into a reusable behavior interface + a `CreatureRuntime` factory that attaches behavior by archetype.
- [ ] **[T] Implement the 4 archetypes** — Swarmer (boids), WallCrawler (surface-stick raycast), Flyer (hover/dive/steal), Bruiser (charge).
- [ ] **[T] Implement the novel behaviors** from `CREATURE_DESIGN.md` (evolution-justified, per-biome) — one or two per chapter as worlds come online.
- [ ] **[A] Per-creature data + loot tables** tied to biome/economy (drops feed the crafting loop).

---
*Sources: `CODE_SCORE.md` (blockers), the 2026-06-21 readiness-gap audit, `WORLD_FLOW_TEMPLATES.md`,
`CREATURE_DESIGN.md`, `ZIPTIDE_MASTER_BUILD_PLAN.md` (E0–E10). Keep this in sync with MASTER_CHECKLIST.*
