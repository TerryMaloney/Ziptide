# SPACE MISSION INTEGRATION — mapping the mission catalog onto the jobs/economy/flags/Signal machinery
### GPT offline-queue item #4 (rb assist, 2026-07-23). The concrete build spec: how each of the 10 space mission types becomes a `JobDefinition` on existing systems.

**Status:** 🔵 DESIGN/BUILD SPEC — zero code (freeze). Turns `SPACE_MISSION_TYPES.md` +
`space_mission_catalog.json` + `space_poi_catalog.json` into an implementable mapping onto the
SHIPPED jobs system. Hand-off target: GPT (engine-side implementation, post-checkpoint).

> **Machine catalog:** `docs/design/space_mission_catalog.json` + `docs/design/space_poi_catalog.json` — CI-enforced; keep in sync (`docs/CATALOG_DOC_RECONCILE.md`).

---

## §1 — THE EXISTING MACHINERY WE MAP ONTO (verified in code)

- **`JobDefinition`** (`Content/Runtime/Jobs/JobDefinition.cs`): `jobId · title · List<JobStepDefinition>
  steps · List<ResourceCost> reward · string completionFlag`. A space mission is just a `JobDefinition`.
- **Existing step assets** (`Content/Runtime/Jobs/Steps/`): `GoToMarker · DisableDronesCount ·
  CollectItemIdCount · DeliverToSocket · ShootTargetsCount`.
- **`WorldPackDefinition.jobs`** holds a world's/system's jobs (space missions author here).
- **`ProfileEconomy`** = rewards (salvage → parts/credits → ship upgrades/passage).
- **Flags:** `ZiptideFlags` + `SIGNAL_THRESHOLD_*` + `PLAYER_*` (completionFlag + Signal + branch).
- **POI families** (`space_poi_catalog.json`): `prior_waker_smallcraft · salvage_tug_disabled ·
  debris_ribbon · orbital_repair_relay · stranded_civilian_ship · ziptide_gate_approach_lane` — the
  destinations a mission's `GoToMarker` steps point at (spawned per GPT's future POI spawn packets, item #5).

## §2 — NEW SPACE STEP DEFINITIONS TO ADD (the only new code; small, mirrors existing steps)

Each is a `JobStepDefinition` subclass with the same authoring pattern as the shipped steps:

| New step | Params | Completes when | Reuses |
|---|---|---|---|
| `TractorSalvageStep` | `targetPoiId`, `count` | tractor-beam pulls N wreck parts in | grabber/tractor + `CollectItemIdCount` idiom |
| `ScanDerelictStep` | `poiId` (+ `readLog:bool`) | derelict scanned / log read | `GoToMarker` + a read-trigger |
| `EscortToStep` | `wardId`, `destMarker` | ward reaches dest alive | `GoToMarker` + a ward-alive check |
| `EvadePatrolStep` | `zoneId`, `mode:evade\|disable` | cleared zone (evaded OR disabled) | `DisableDronesCount` (disable branch) |
| `RepairRelayStep` | `relayId`, `signalDelta` | relay repaired → **raises Signal** | `DeliverToSocket`/repair + `SIGNAL_THRESHOLD_*` |
| `AnswerBeaconStep` | `beaconId`, `resolve:tow\|defend\|repair` | distress resolved | tow/repair + `GoToMarker` |

*That's the whole new surface — 6 small step assets. Everything else is existing steps + data.*

## §3 — THE 10 MISSIONS → JOB TEMPLATES (step sequence · POI · reward · flag · Signal)

| Mission (`id`) | Step sequence | POI family | Reward | completionFlag | Signal |
|---|---|---|---|---|---|
| `salvage_run` | GoToMarker → (EvadePatrol?) → TractorSalvage(part,n) | prior_waker_smallcraft / debris_ribbon | parts+credits | `JOB_SALVAGE_<w>` | – |
| `derelict_discovery` | GoToMarker → ScanDerelict(readLog) → Collect(mysteryObj) | prior_waker_smallcraft | lore+part | `LORE_WRECK_<n>` (ordered thread) | – |
| `ferry_passage` | Collect(cargo) → GoToMarker(dock) → Deliver(cargo) | (dock↔dock) | credits (passage) | `JOB_FERRY_<w>` | – |
| `distress_rescue` | AnswerBeacon(tow\|defend\|repair) | stranded_civilian_ship / salvage_tug_disabled | credits+goodwill | `RESCUE_<n>` | – |
| `relay_seal_repair` | GoToMarker → RepairRelay(signalDelta) | orbital_repair_relay | parts | `JOB_RELAY_<w>` | **+Signal** |
| `warden_evasion` | EvadePatrol(evade\|disable) | (patrol zone) | passage/none | `WARDEN_ZONE_<n>` | reads Signal (aggression) |
| `escort_assist` | EscortTo(ward,dest) | (Guild/Nine transport) | faction rep | `ESCORT_<faction>_<n>` | – |
| `sable_faction_op` | GoToMarker → ShootTargets/Disable → objective | (target site) | faction rep | `SABLE_OP_<n>` (branch) | possible +Signal |
| `gate_run` | GoToMarker(gate lane) → (transit) | ziptide_gate_approach_lane | unlock system | `GATE_RUN_<sys>` | – |
| `the_approach` | GoToMarker(Lagrange) → ScanDerelict(cloaked ship) → board → fly-to-Earth | (bespoke endgame) | story | `APPROACH_DONE` | – |

Notes: `relay_seal_repair` is the Signal knob (bible §7 — repairing wakes the universe → Warden
aggression via `warden_evasion` reading Signal). `derelict_discovery`'s `LORE_WRECK_<n>` flags are
ORDERED so the prior-waker logs read as the hidden second story (bible §5/§7). Pacifist path holds:
every CORE row completes without a Shoot/Disable step (EvadePatrol has an `evade` mode); only
`sable_faction_op` is combat-forward.

## §4 — AUTHORING FLOW (how a space mission gets made — World-Compiler-ready)

1. Pick the mission `id` from the catalog → its job template (§3).
2. Point its `GoToMarker`/POI steps at POI ids from the system's `space_poi_catalog` families
   (spawned by GPT's future POI spawn packets, item #5).
3. Set `reward` (ProfileEconomy resource ids) + `completionFlag` (+ `signalDelta` if a relay).
4. Add the `JobDefinition` to the system/world `WorldPackDefinition.jobs`.
5. Gate availability by chapter/faction flags (`SPACE_MISSION_TYPES` §3 progression map).
→ So a space mission = **data + (once) the 6 new step assets**. No bespoke per-mission code.

## §5 — BUILD ORDER / HAND-OFF (GPT)

1. Add the 6 `JobStepDefinition` subclasses (§2) + EditMode tests (mirror the existing step tests).
2. Wire `RepairRelayStep` → `SIGNAL_THRESHOLD_*`; `warden_evasion` reads Signal for aggression.
3. Author one job per mission type as a **template** on the Moss system pack (proves the loop).
4. Connect to GPT's POI spawn packets (item #5) so `GoToMarker` targets resolve.
5. Economy rewards through `ProfileEconomy`; flags through `ZiptideFlags`.
- **Freeze:** all post-Golden-Checkpoint. This spec + the catalogs are the contract. **⚖ for Terry:**
  per-chapter mission density; exact `signalDelta` per relay; whether `gate_run` is a job or pure traversal.
