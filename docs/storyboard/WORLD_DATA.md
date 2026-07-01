# ZIPTIDE — WORLD_DATA (story → WorldPack serialization)

**The deterministic, generator-ready bridge between the prose catalog and the data layer.** It turns each
`CHAPTER_*` / per-world README seed into a structured record that maps **1:1 onto the real authorable
types** (`WorldPackDefinition`, `JobDefinition` + step assets, `ZiptideFlags`). A `WorldStubGenerator` (or a
human authoring a WorldPack) can read a record here and produce the asset without re-reading the prose.

> **Lane:** Architect (DATA). This doc is the *source of truth for world data*; it does NOT build scenes,
> geometry, or runtime — those consume it (T-Dog). Inherits canon from `STORY_BIBLE.md` + `THE_TRANSMISSION.md`.
> **Proven pattern:** W001 is already shipped exactly this way (`ToxicCityContractBuilder` → `ToxicCity_Contract`).

---

## 0. How to read a record (the serialization key)

Each world is one record. Fields map to real types — **nothing here is invented vocabulary except where a
field is explicitly tagged `⚠ SCHEMA-GAP`** (a field the story needs but the SO doesn't have yet — see §1).

```
W### — Name
  packId:        <string>        → WorldPackDefinition.packId   (snake_case, stable, = the asset key)
  sceneName:     <string>        → WorldPackDefinition.sceneName (PascalCase, must be in Build Settings)
  displayName:   <string>        → WorldPackDefinition.displayName (travel-door label)
  flow:          <archetype>     → picks a recipe from docs/design/WORLD_FLOW_TEMPLATES.md (graybox guide; not a field)
  biome/hazard:  <id> / <hazard> → BiomeDefinition (T-Dog kit selection; informs theme)
  theme:         <hint>          → WorldPackDefinition.defaultTheme (VisualThemeProfile; sky/ground/fog/planet)
  flagsRequired: [<ZiptideFlags…>]  ⚠ SCHEMA-GAP (see §1) — entry gate
  flagsGranted:  [<ZiptideFlags…>]  ⚠ SCHEMA-GAP (see §1) — set on world-complete
  jobs:                          → WorldPackDefinition.jobs (List<JobDefinition>)
    - jobId / title
      steps:   ordered → real step assets (see §0.1)
      reward:  [{resourceId, amount}]   → JobDefinition.reward (List<ResourceCost>)
      completionFlag: <ZiptideFlags|raw> → JobDefinition.completionFlag (the ONE flag that lands today)
  spawnMarkers: [{markerId, pos, euler}] → WorldPackDefinition.spawnMarkers (+ GoToMarker targets)
  creatures:    [<creatureId>]   → CreatureDefinition ids (zone spawn; T-Dog runtime)
  fragment:     <none|tier>      → Transmission fragment + clarity tier (see §3); ties to flagsGranted
```

### 0.1 Step-type vocabulary (the only legal `steps` verbs)
Confirmed assets under `Content/Runtime/Jobs/Steps/`:
| Record verb | Asset | Params |
|---|---|---|
| `GoToMarker(markerId)` | `GoToMarkerStepDefinition` | `markerId`, `arriveDistance` (def 1.5) |
| `DisableDrones(n)` | `DisableDronesCountStepDefinition` | `count` |
| `Collect(itemId, n)` | `CollectItemIdCountStepDefinition` | `itemId`, `count` |
| `Deliver(socketId, itemId, n)` | `DeliverToSocketStepDefinition` | `socketId`, `itemId`, `count` |
| `ShootTargets(n)` | `ShootTargetsCountStepDefinition` | `count` |

**GoToMarker resolution (locked):** `JobDirector` finds the GameObject named `"Marker_" + markerId`;
`CityBuilder`/patchers create `Marker_<markerId>`. So every `GoToMarker(x)` needs a `spawnMarkers` entry
(or a patcher-placed marker) with `markerId = x`. **Combat/lore beats with no built step** (dread walks,
dialogue, "read the log") serialize as a `GoToMarker` to the beat's location — the prose tone is data the
patcher/VO layer reads, not a step type. Don't invent step verbs; if a beat needs one, that's a backlog
item, not a record field.

---

## 1. World-level flag gating — ✅ schema landed (CI-green)

**Resolved (2026-06-27, `[A]`).** `WorldPackDefinition` now has **`List<string> flagsRequired`** and
**`List<string> flagsGranted`** (both default-empty, so old assets deserialize unchanged). Logic lives in
the pure, EditMode-tested **`WorldGating`** helper (`Content/Runtime/WorldPacks/WorldGating.cs`):
- `WorldGating.MeetsRequirements(pack, profile)` — all required flags set? (empty list = always; null
  profile fails closed on a real requirement). `FirstMissingRequirement(...)` for "locked because…" UI.
- `WorldGating.GrantWorldFlags(pack, profile)` — sets all `flagsGranted` on the profile; null-safe,
  idempotent, returns the newly-set count.

**Wired:** `JobDirector.OnJobCompleted` now calls `GrantWorldFlags` (after the per-job `JobRewards.Grant`),
so a world's RILL beat + Signal threshold + `W###_COMPLETE` all land when its contract finishes — the flags
a single `JobDefinition.completionFlag` couldn't all carry. `JobDirector.Start` logs `ZIPTIDE: WORLD_LOCKED`
(non-blocking) if a world is entered without its requirements.

**Still a follow-up (`[T]`, travel/UI lane):** *enforcing* `flagsRequired` — don't *offer/allow travel to* a
locked world. That belongs at the travel/offer UI (`WorldTravelStation`/`DispatchKiosk`), which touches the
locked travel contract, so it's report-only, not done blind here. The check (`MeetsRequirements`) is ready
for that UI to call. Queued in `FABLE5_BACKLOG.md`.

**Serialization note:** records still mark the single critical flag carried by a job's `completionFlag` with
**`◀ ships today`**; the *rest* of each record's `flagsGranted` now ship for real via `JobDirector` + the new
fields. The prose `flagsGranted` lists below are now **authorable + enforced on completion** (entry-gating
pending the `[T]` UI hook).

---

## 2. Serialized worlds — Chapters 0–2 (W000–W012, the proven pattern)

These twelve are serialized in full as the reference pattern. Chapters 3–12 follow the same record shape
(seeds already in `CHAPTER_3…8-12.md` + `DLC.md`); serialize them on demand when each world is built — no
value in front-loading 68 records that will drift before they're touched.

> **BUILD STATUS (modularity sprint):** W002–W004 are **implemented from these records** via
> `WorldLayoutLibrary` + `WorldJobLibrary` (Editor) — auto-built every APK build. Deviations from the
> records, all deliberate: (1) **W002's gate is `toxiccity_complete`**, not `TUTORIAL_COMPLETE` — W000 is
> parked (needs the ship system), so the chain must be playable today; (2) **Collect/Deliver steps are
> deferred** (no collectible-spawning system yet) — contracts use GoToMarker/DisableDrones; upgrade the
> step assets when collectibles land; (3) swarm/tendril combat = **drone stand-ins** until Phase E.

> Resource ids below (`credits`, `scrap`, `signal_node`, `mineral`, `crystal`, `memory_shard`, `spore`,
> `prism`, `fuel_cell`, `data_chip`, `carapace`, `salt`, `resonator`, `jump_core`) are the `resourceId`
> strings; confirm/extend the `ResourceDefinition` registry as worlds are built. `credits` is the leash currency.

---

### W000 — The Drift In
```
packId:        w000_drift_in
sceneName:     W000_DriftIn
displayName:   Your Ship
flow:          interior (linear tutorial — see WORLD_FLOW_TEMPLATES §interior)
biome/hazard:  ship_interior / none
theme:         ship cabin; viewport on the Ziptide gate-ring blooming open (first awe shot)
flagsRequired: []                                   (entry world)
flagsGranted:  [TUTORIAL_COMPLETE, FIRST_TRAVEL]
jobs:
  - jobId: w000_onboard   title: "Cast Off"
    steps:
      - GoToMarker(helm)                            # learn move/look
      - GoToMarker(coupler)                         # learn grab/holster at the gate-coupler
      - GoToMarker(gate)                            # learn travel — drift into the network
    reward: [{credits, 0}]                          # tutorial, no payout
    completionFlag: TUTORIAL_COMPLETE               ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, cabin}, {helm}, {coupler}, {gate}]
creatures:     []
fragment:      none   (RILL pre-boot; the coupler "reads a frequency that shouldn't have a sender" — seed only)
```

### W001 — Toxic Venice  ★ signature · BUILT (the depth bar)
```
packId:        toxic_city                           (existing — ToxicCity_Contract)
sceneName:     ToxicCity
displayName:   Toxic Venice
flow:          city (layered verticality — WORLD_FLOW_TEMPLATES §city; see CITY_DESIGN)
biome/hazard:  toxic_city / toxic (sludge canals = no-stand, fall-respawn)
theme:         overcast industrial haze, sodium-orange low / bruised grey; green fog; dim banded planet low (~12°)
flagsRequired: []
flagsGranted:  [C1_W001_RILL_BOOT, SIGNAL_THRESHOLD_1]  (+ completionFlag below)
jobs:
  - jobId: toxiccity_contract   title: "Clear the Relay, Ship Out"
    steps:
      - GoToMarker(dispatch_inside)                 # report to the Dockmaster
      - DisableDrones(5)                            # clear the feral patrols (market + canal)
      - GoToMarker(relay_node)                      # re-seat the relay — it comes up WRONG
      - GoToMarker(shipyard_office)                 # return to berth, leash loosens
    reward: [{credits, 100}]
    completionFlag: toxiccity_complete              ◀ ships today (raw string, pre-dates flag consts)
spawnMarkers:  [{__SPAWN_PLAYER, berth}, {dispatch_inside}, {relay_node}, {shipyard_office}]
creatures:     [drone]                              # shockable; 2 patrols + passive trio at dispatch
fragment:      none   (the "wrong signature" mystery seeds the Transmission; first fragment is W004)
```
*RILL VO (data, not steps): boot "Systems nominal. I think." · first kill "Maintenance units shouldn't bite
back. Noted." · relay "That signature's older than this district." · ship-out "Whatever that node was
talking to — it wasn't us."*

### W002 — The Dry Cistern
```
packId:        w002_dry_cistern
sceneName:     W002_DryCistern
displayName:   The Dry Cistern
flow:          underground (descent + dark traversal — WORLD_FLOW_TEMPLATES §underground)
biome/hazard:  dry_cistern / none (verticality + darkness, no damage hazard)
theme:         pitch-dark caverns; one shaft of surface light as landmark; Architect stonework
flagsRequired: [TUTORIAL_COMPLETE]
flagsGranted:  [W002_COMPLETE]
jobs:
  - jobId: w002_pumps   title: "Restart the Cistern Pumps"
    steps:
      - GoToMarker(shaft_descent)                   # first descent, teach dark traversal
      - Collect(mineral, 5)                          # mine nodes (teaches mining)
      - DisableDrones(3)                             # swarm stand-in (skittering Bloom-bugs)*
      - GoToMarker(pump_house)                       # restart pumps (ore-bucket conveyor)
    reward: [{credits, 60}, {mineral, 5}]
    completionFlag: W002_COMPLETE                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, cistern_mouth}, {shaft_descent}, {pump_house}]
creatures:     [swarm]                               # *swarm runtime not built — use drone zone until E-phase
fragment:      none
```
> *Note: `swarm` archetype has no runtime yet (only `drone`). Until Phase E, author the combat step as
> `DisableDrones` against drone zones, or omit combat. Don't ship a `creatures:[swarm]` the runtime can't honor.

### W003 — Glass Shelf
```
packId:        w003_glass_shelf
sceneName:     W003_GlassShelf
displayName:   Glass Shelf
flow:          exterior-open (wind hazard, fall-risk ledges — WORLD_FLOW_TEMPLATES §exterior)
biome/hazard:  glass_shelf / wind (gusts shove — comfort-safe nudge)
theme:         vast clear sky, TWO moons + faint geometric shimmer at zenith (first subliminal Pattern seed)
flagsRequired: [TUTORIAL_COMPLETE]
flagsGranted:  [W003_COMPLETE]
jobs:
  - jobId: w003_baffles   title: "Raise the Wind Baffles"
    steps:
      - GoToMarker(mesa_base)
      - Collect(crystal, 4)                          # harvest from glass blooms
      - GoToMarker(baffle_relay_a)                   # raise baffle relays (wind eases as you go)
      - GoToMarker(baffle_relay_b)
    reward: [{credits, 70}, {crystal, 4}]
    completionFlag: W003_COMPLETE                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, skiff_wreck}, {mesa_base}, {baffle_relay_a}, {baffle_relay_b}]
creatures:     [tendril]                             # wind-whipped Bloom vines (runtime: Phase E)
fragment:      none   (the crystals "ring at W001's relay frequency" — Pattern seed, not a fragment)
```

### W004 — The Broadcast Tomb  ★ RILL beat · Ch.1 capstone
```
packId:        w004_broadcast_tomb
sceneName:     W004_BroadcastTomb
displayName:   The Broadcast Tomb
flow:          interior (linear-with-branches, dread pacing — WORLD_FLOW_TEMPLATES §interior)
biome/hazard:  broadcast_tomb / static (discharge arcs near live panels — scramble gear, careful pathing)
theme:         no sky; wall of dead screens, ONE flickers an alien sky; dead-grey, heavy fog, static-blue flashes
flagsRequired: [W001_COMPLETE]                       (Ch.1 capstone — gate behind the first contract)
flagsGranted:  [C1_WAKE_GUILD_INTRO, C1_W004_RILL_ASKED_CARGO, SIGNAL_THRESHOLD_1, FRAGMENT_T1_FOUND]
jobs:
  - jobId: w004_broadcast   title: "Restore the Broadcast Spine"
    steps:
      - GoToMarker(tomb_entry)                       # enter the dark station (dread / flashlight)
      - GoToMarker(junction_a)                       # route power through static junctions
      - GoToMarker(junction_b)
      - Collect(memory_shard, 1)                     # recover the final broadcast = FRAGMENT 1
      - GoToMarker(broadcast_core)                   # restore spine — RILL asks its question
    reward: [{credits, 90}, {memory_shard, 1}]
    completionFlag: W004_COMPLETE                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, tomb_entry}, {junction_a}, {junction_b}, {broadcast_core}]
creatures:     []                                    # deliberately none — dread, not combat
fragment:      T1 / clarity tier 1 (garbled junk)   → THE TRANSMISSION fragment #1 (see §3)
```
*RILL beat (locked): builds to "What are you carrying that requires containment?" — Dormant → first stir.*

### W005 — Oxidized Canopy
```
packId:        w005_oxidized_canopy
sceneName:     W005_OxidizedCanopy
displayName:   Oxidized Canopy
flow:          forest (vertical branch traversal, garden loop intro — WORLD_FLOW_TEMPLATES §forest)
biome/hazard:  oxidized_canopy / spore (haze slows you)
theme:         rust-orange dusk, falling spores like snow, banded planet larger now
flagsRequired: [W004_COMPLETE]
flagsGranted:  [W005_COMPLETE, C2_W005_JOB_COMPLETE]
jobs:
  - jobId: w005_harvest   title: "Scrub the Spores, Work the Canopy"   (Mara's first contract)
    steps:
      - GoToMarker(canopy_lift)
      - Collect(spore, 6)                            # garden/harvest loop intro (tend→harvest)
      - DisableDrones(4)                             # drone patrol (+ tendrils, Phase E)
      - GoToMarker(scrubber)                         # spore-scrubber + harvest conveyor
    reward: [{credits, 110}, {spore, 6}]
    completionFlag: C2_W005_JOB_COMPLETE             ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, forest_floor}, {canopy_lift}, {scrubber}]
creatures:     [drone, tendril]
fragment:      none   (mystery: "the Bloom is cultivating the relays" — environmental)
```

### W006 — The Mirror Flats
```
packId:        w006_mirror_flats
sceneName:     W006_MirrorFlats
displayName:   The Mirror Flats
flow:          exterior-open (puzzle-leaning, prism routing — WORLD_FLOW_TEMPLATES §exterior)
biome/hazard:  mirror_flats / reflection (glare; heat shimmer)
theme:         doubled sky — real + mirror; in the mirror a shape that isn't above you; Cal's reflection lags ½s
flagsRequired: [W005_COMPLETE]
flagsGranted:  [W006_COMPLETE]
jobs:
  - jobId: w006_prisms   title: "Align the Prism Towers"
    steps:
      - GoToMarker(flats_edge)
      - GoToMarker(prism_tower_a)                    # align towers to route a beam (a surveillance lattice)
      - GoToMarker(prism_tower_b)
      - GoToMarker(beam_collector)
    reward: [{credits, 100}, {prism, 3}]
    completionFlag: W006_COMPLETE                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, mirror_rig}, {flats_edge}, {prism_tower_a}, {prism_tower_b}, {beam_collector}]
creatures:     []                                    # no enemies — tension
fragment:      none
```

### W007 — Sable Station
```
packId:        w007_sable_station
sceneName:     W007_SableStation
displayName:   Sable Station
flow:          station (low-g corridors, viewport reveal — WORLD_FLOW_TEMPLATES §station)
biome/hazard:  sable_station / none (low-g handling)
theme:         viewports onto a world inside a faint hexagonal GRID (first clear Shell glimpse)
flagsRequired: [W006_COMPLETE]
flagsGranted:  [W007_COMPLETE, C4_SABLE_INTRO]
jobs:
  - jobId: w007_fuelrig   title: "Repair Sable's Fuel Rig"
    steps:
      - GoToMarker(airlock)
      - Collect(fuel_cell, 3)
      - Deliver(rig_socket, fuel_cell, 3)            # repair the station's fuel rig (Sable's, not Guild's)
      - GoToMarker(observation_deck)                 # the viewport Shell reveal
    reward: [{credits, 120}, {fuel_cell, 1}]
    completionFlag: C4_SABLE_INTRO                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, dock}, {airlock}, {rig_socket}, {observation_deck}]
creatures:     [guard]                               # Sable guards — non-lethal standoff (runtime: Phase E)
fragment:      none   (mystery: Sable have a map of the network with an EDGE)
```

### W008 — The Sealed Archive
```
packId:        w008_sealed_archive
sceneName:     W008_SealedArchive
displayName:   The Sealed Archive
flow:          interior (lore hub, sorting conveyor — WORLD_FLOW_TEMPLATES §interior)
biome/hazard:  sealed_archive / static (fragile data-stacks)
theme:         no sky; domed ceiling painted with a star map that has a WALL around it
flagsRequired: [W007_COMPLETE]
flagsGranted:  [W008_COMPLETE, C2_ARCHITECTS_NAMED]
jobs:
  - jobId: w008_archive   title: "Restore the Archive"
    steps:
      - GoToMarker(vault_door)
      - GoToMarker(power_core)                       # restore archive power
      - Collect(data_chip, 5)                        # sorting conveyor of data chips
      - Deliver(reader_socket, data_chip, 5)         # read (can't fully read) the Architects' records
    reward: [{credits, 120}, {data_chip, 2}]
    completionFlag: C2_ARCHITECTS_NAMED              ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, archive_mouth}, {vault_door}, {power_core}, {reader_socket}]
creatures:     []
fragment:      none   (mystery: every record stops at the same date — "the Sealing"; RILL: "I have read this before")
```

### W009 — Chitinwall  ★ RILL beat
```
packId:        w009_chitinwall
sceneName:     W009_Chitinwall
displayName:   Chitinwall
flow:          city (tight alleys, swarm combat — WORLD_FLOW_TEMPLATES §city)
biome/hazard:  chitinwall / swarm
theme:         churning insectile aurora; planet now clearly banded by the grid
flagsRequired: [W008_COMPLETE]
flagsGranted:  [W009_COMPLETE, C2_W009_RILL_MISIDENTIFIED]
jobs:
  - jobId: w009_pylons   title: "Raise the Swarm-Deterrent Pylons"
    steps:
      - GoToMarker(wall_gate)
      - DisableDrones(6)                             # signature swarm world (swarm runtime: Phase E; drone stand-in)
      - Collect(carapace, 4)
      - GoToMarker(pylon_array)                      # build + power deterrent pylons
    reward: [{credits, 130}, {carapace, 4}]
    completionFlag: C2_W009_RILL_MISIDENTIFIED       ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, undercity}, {wall_gate}, {pylon_array}]
creatures:     [swarm]                               # runtime Phase E; until then drone zones
fragment:      none   (RILL: "I know this—wait, I do not. That is new." — memory glitch)
```

### W010 — Tidal Array
```
packId:        w010_tidal_array
sceneName:     W010_TidalArray
displayName:   Tidal Array
flow:          coastal (timed high-ground vs flood — WORLD_FLOW_TEMPLATES §coastal)
biome/hazard:  tidal_array / flood (periodic — reach high ground)
theme:         a vast ringed planet pulling the tides; storm light
flagsRequired: [W009_COMPLETE]
flagsGranted:  [W010_COMPLETE, SIGNAL_THRESHOLD_2]
jobs:
  - jobId: w010_turbines   title: "Restart the Tidal Turbines"
    steps:
      - GoToMarker(shore_camp)
      - GoToMarker(turbine_a)                        # timed: cross between floods
      - GoToMarker(turbine_b)
      - Collect(salt, 4)
    reward: [{credits, 120}, {salt, 4}]
    completionFlag: W010_COMPLETE                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, fishing_wreck}, {shore_camp}, {turbine_a}, {turbine_b}]
creatures:     [tendril]                             # tide-borne Bloom (runtime: Phase E)
fragment:      none   (mystery: the tides sync to the Signal meter, not the moon)
```

### W011 — The Hum
```
packId:        w011_the_hum
sceneName:     W011_TheHum
displayName:   The Hum
flow:          underground (resonance, sound-as-hazard — WORLD_FLOW_TEMPLATES §underground)
biome/hazard:  the_hum / vibration (shakes aim/gear)
theme:         no sky; cave crystals pulse in time with the Hum
flagsRequired: [W010_COMPLETE]
flagsGranted:  [W011_COMPLETE]
jobs:
  - jobId: w011_resonators   title: "Tune the Resonator Banks"
    steps:
      - GoToMarker(tunnel_mouth)
      - Collect(resonator, 3)
      - GoToMarker(resonator_bank_a)                 # tune the banks (silencing a scream, it turns out)
      - GoToMarker(resonator_bank_b)
    reward: [{credits, 110}, {resonator, 3}]
    completionFlag: W011_COMPLETE                    ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, miners_camp}, {tunnel_mouth}, {resonator_bank_a}, {resonator_bank_b}]
creatures:     []
fragment:      none   (mystery: the Hum IS words — slowed 1000×; RILL visibly unsettled)
```

### W012 — Mara's Last Jump  ★ Ch.2 milestone
```
packId:        w012_maras_last_jump
sceneName:     W012_MarasLastJump
displayName:   Mara's Last Jump
flow:          void (radiation exposure timer, drifting debris — WORLD_FLOW_TEMPLATES §void)
biome/hazard:  void_gate / radiation (exposure timer)
theme:         raw void + the unmistakable hexagonal SHELL wall; Mara's ship arcs toward it and bounces off
flagsRequired: [W011_COMPLETE]
flagsGranted:  [W012_COMPLETE, C2_CONTAINMENT_REVEALED]
jobs:
  - jobId: w012_stabilize   title: "Stabilize the Failing Gate"
    steps:
      - GoToMarker(gantry)
      - GoToMarker(gate_core_a)                      # stabilize a failing gate so Mara can jump
      - GoToMarker(gate_core_b)
      - Collect(jump_core, 1)                        # Mara hands Cal the Jump Core (unlocks deeper travel)
    reward: [{credits, 150}, {jump_core, 1}]
    completionFlag: C2_CONTAINMENT_REVEALED          ◀ ships today
spawnMarkers:  [{__SPAWN_PLAYER, gantry}, {gate_core_a}, {gate_core_b}]
creatures:     []                                    # the void is the enemy
fragment:      none   (mystery: the Shell wall is getting closer over centuries — Mara's telemetry)
```

---

## 3. The Transmission fragment system — clarity-tier flags  `[A]`

`THE_TRANSMISSION.md` §10 — fragments are **collectibles tied to `WorldPackDefinition` + `ZiptideFlags`
clarity tiers**, one per chapter milestone, each de-garbling clearer than the last. This section is the
**data spec** (the flags + cadence). The **de-garble playback + recognition UI is `[T]`** (Adaptive Audio).

### 3.1 The flags (proposed `ZiptideFlags` additions — `[A]` code task)
Add a Transmission block to `ZiptideFlags.cs`. `FRAGMENT_T#_FOUND` is set when the fragment item is
collected (carried today via the world's `completionFlag` chain or, post-schema, `flagsGranted`).
`TRANSMISSION_CLARITY_#` is a derived progression tier (how legible the assembled message is so far).

```
// ── The Transmission (identity layer — THE_TRANSMISSION.md) ──
FRAGMENT_T1_FOUND      = "FRAGMENT_T1_FOUND";   // W004 — garbled junk; "addressed to the watchers"
FRAGMENT_T2_FOUND      = "FRAGMENT_T2_FOUND";   // W042 — "the voice is mine" (player consciously recognizes)
FRAGMENT_T3_FOUND      = "FRAGMENT_T3_FOUND";   // W047/W051 era — "we" becomes a specific person (the partner)
FRAGMENT_T4_FOUND      = "FRAGMENT_T4_FOUND";   // W060 — THE NAME MOMENT (she says your own name) + a coordinate
FRAGMENT_T5_FOUND      = "FRAGMENT_T5_FOUND";   // W062 — "you built it and chose to enter"
FRAGMENT_RILL_CONFESS  = "FRAGMENT_RILL_CONFESS"; // late — RILL's almost-said line, recovered elsewhere

TRANSMISSION_CLARITY_1 = "TRANSMISSION_CLARITY_1"; // after T1 — static, register: professional
TRANSMISSION_CLARITY_2 = "TRANSMISSION_CLARITY_2"; // after T2 — half-legible, register: confessional ("you")
TRANSMISSION_CLARITY_3 = "TRANSMISSION_CLARITY_3"; // after T3/T4 — clear, register: intimate ("I")
TRANSMISSION_CLARITY_MAX = "TRANSMISSION_CLARITY_MAX"; // full assembly — endgame only
```

### 3.2 Fragment schedule (cadence: ~one per chapter; emotional register escalates)
| # | World | Chapter | Clarity tier | Register | Beat |
|---|-------|---------|--------------|----------|------|
| T1 | **W004** Broadcast Tomb | 1 | 1 (garbled) | professional | "something is addressed to someone" — reads as junk |
| T2 | **W042** Listening Post | 6 | 2 (half) | confessional | "the voice is mine" — conscious recognition |
| T3 | **W047/W051** Architect's Chamber / RILL Names | 6 | 3 | confessional→intimate | the "we" becomes the **partner** (a specific person) |
| T4 | **W060** Architect's Tomb | 7 | 3 | intimate | **the name moment** — she says your name + a coordinate |
| T5 | **W062** Revelation | 8–12 | MAX | intimate | "you built it and chose to enter" |
| R  | **RILL almost-confession** | late | (feeds MAX) | — | the one line the directive cut off, recovered as artifact |

**Rules for the serializer:**
- A fragment world adds its `FRAGMENT_T#_FOUND` to `flagsGranted` (today: via the fragment-collect job's
  `completionFlag`; the collect step is `Collect(<fragment_item>, 1)` on a `memory_shard`-class item).
- `TRANSMISSION_CLARITY_#` is **derived**, not authored per-world: a small narrative service raises the tier
  when the matching `FRAGMENT_T#_FOUND` set is complete. That service is the `[A]` wiring task below.
- **Observers' one escalation** (late Ch.7/endgame): the T4/T5 fragments are *harder to reach* — express as
  extra `flagsRequired` / an artifact-protection step, NOT a new mechanic.

> **[A] backlog (Phase C, after the flag-fields schema):** (1) add the §3.1 block to `ZiptideFlags`; (2) a
> `TransmissionProgress` narrative service that derives `TRANSMISSION_CLARITY_#` from the `FRAGMENT_*` flags
> and exposes the current tier to the `[T]` de-garble UI; (3) define the fragment item id(s). All CI-safe,
> testable, no scene work. The audio assets (ambiguous male+female voice, partner complement) are recordings,
> tracked separately.

---

## 4. Serializing the rest (Chapters 3–12 + DLC) — the procedure

When a world from `CHAPTER_3…8-12.md` / `DLC.md` comes up for build, write its record here using §0/§0.1:
1. `packId` = `w###_snake_name`; `sceneName` = `W###_PascalName`; `displayName` from the seed.
2. `flow` = pick the archetype matching the seed's Type (City/Underground/Exterior/Interior/Void/Coastal/
   Station/Forest/Pattern) from `WORLD_FLOW_TEMPLATES.md`.
3. `flagsRequired` = the prior world's `_COMPLETE` (+ any branch gate, e.g. `C6_WARDEN_ALLY`).
4. `flagsGranted` = the seed's listed flags (RILL beat + Signal threshold + `W###_COMPLETE`). **Carry the
   single most-critical one as the last job's `completionFlag` until the schema field lands (§1).**
5. `jobs` = turn the seed's mission table into `steps` using ONLY the §0.1 verbs (combat → `DisableDrones`
   while only `drone` runtime exists; lore/dread → `GoToMarker`).
6. `creatures` = the seed's enemy id — but **only if its archetype has runtime** (today: `drone` only;
   the rest are Phase-E). Otherwise stand in with drone zones or omit.
7. `fragment` = T1–T5/R if the world is in the §3.2 schedule, else `none`.
8. Branch worlds (W037/W043/W049 Warden; W046 Mara; W063 endings) get **parallel records** per branch flag.

**Hard rules:** no invented step verbs; no `creatures` the runtime can't honor; every `GoToMarker(x)` has a
matching `spawnMarkers`/patcher marker `x`; resource ids must exist in (or be added to) the `ResourceDefinition`
registry. Keep records terse — the prose READMEs hold the why; this file holds the *what the generator needs*.

---
*Sources: `STORY_BIBLE.md`, `THE_TRANSMISSION.md`, `CHAPTER_0-1`/`CHAPTER_2`, `W001_ToxicCity/README.md`
(shipped pattern), the real `WorldPackDefinition`/`JobDefinition`/`ZiptideFlags`/`*StepDefinition` types.
Consumed by: `WorldStubGenerator` (`[T]`, Phase C), per-world WorldPack authoring, the Transmission system.*
