# LEVEL 1 MASTER TRACKER — every requirement, its owner, and the proof that closes it

**Purpose:** Terry's instruction was *"everything from level one built all the way to level two…
every last tiny detail. If you miss one little itty bitty detail you fail."* Nothing here is
remembered. Every requirement is a row, and a row only closes against a named proof.

**Authority:** [`FIRST_LEVEL_PRODUCT_CONTRACT.md`](FIRST_LEVEL_PRODUCT_CONTRACT.md) defines the
product; this file tracks its execution. Where a row disagrees with the newest CI verdict or device
log, the evidence wins and this file gets corrected in the same commit.

**State legend:**
`⬜ UNBUILT` · `🟨 CODE` (compiles + tests, not in a scene) · `🟦 BAKED` (in committed content)
· `🟩 DEVICE` (Terry has seen it work) · `✅ DONE`

---

## 0. What changed in this pass (2026-07-29)

Four faults were found that each silently disabled a load-bearing part of Level 1. None were visible
to CI, because every owner satisfied its own contract:

1. **The first level's contract could never complete.** `JobDirector` resolved `GoToMarker` steps only
   against pack-declared markers; ToxicCity declared one (`player`) while three of its four steps
   pointed at markers the city builder authors straight into the scene. Fixed with a cached scene
   fallback plus a loud `JOB_MARKER_MISSING` diagnostic. **This also unlocked Level 2** — W002 is
   gated on `toxiccity_complete`, which only the completed contract grants.
2. **W000's exit door led to a test scene** (`MilestoneA_GrabCube`). Retargeted.
3. **One stale asset switched off half of W001.** `ToxicCityLayout.asset` predated `sceneName`,
   `experience`, `pois`, `hazards`, `creatureZones` and the sky block, so all of it loaded as
   defaults — and the empty `sceneName` made ToxicCity list *itself* as a ship destination.
4. **The first hour was silent and empty.** No audio profile on either world despite the asset
   existing; zero creatures placed despite seven authored species; three RILL lines for both worlds
   combined and none of the 15 teaching lines.

---

## 1. Phase A–J — the 58 route requirements

Numbers are `FIRST_LEVEL_PRODUCT_CONTRACT.md` §3.

| # | Requirement | State | Proof that closes it |
|---|---|---|---|
| **A. Cold boot and home choice** | | | |
| 1–2 | Home Hub reachable; New/Continue readable and selectable | 🟨 | Quest reach/readability pass at the shipped 0.65 m tile row (DV-01) |
| 3 | Boot hold prevents unsafe movement | 🟦 | Recovery PlayMode boot route |
| 4 | New Game performs one canonical travel | 🟦 | `TRAVEL_OK` once, no duplicate |
| **B. W000 wake, identity, ship restoration** | | | |
| 5 | RILL's first orientation beat | 🟦 | `enter_w000` + `cal_w000` authored; device subtitle read |
| 6 | Comfort choice, non-blocking | 🟨 | Bake `Author W000 Surfaces`, then device |
| 7 | Movement/turning taught safely | 🟨 | Observation adapter placed by the same bake |
| 8 | Grab + holster one personal item | 🟨 | Bunk keepsake exists only after the bake |
| 9–12 | Scan the coupler → panel → part → power | 🟦 | Machine authored in the W000 pack; device repair pass |
| 13 | Ship visibly/audibly arms launch | 🟦 | `ShipCastOffRuntime` arming on `gate_coupler` |
| **C. Board, select, launch** | | | |
| 14–16 | Board, helm names W001, `PUNCH IT` | 🟦 | Device: seated and child reach at the helm |
| 17 | Cast-off presentation from aboard | 🟨 | Star-streak exists; authored launch leg pending |
| 18 | Travel loads the authored space route once | ⬜ | **Wave 2** — no space scene exists yet |
| **D. Playable space tutorial** | | | |
| 19–23 | Helm, throttle/steer, required boost, course, comfort cap | ⬜ | **Wave 2/4** — `SpaceLane_Trial.unity` has never been created |
| **E. Non-lethal space salvage** | | | |
| 24–29 | Readable target → disable → power-down → salvage → paid → one neutral signal | ⬜ | **Wave 4** |
| **F. First Ziptide, approach, reentry** | | | |
| 30–35 | Approach anchor, Ziptide from the space leg, crest hides the load, reentry act, one handoff, state restored | ⬜ | **Wave 3**; camera never animated or parented |
| **G. W001 authored arrival** | | | |
| 36 | Deliberate arrival composition | ⬜ | **Wave 5** — needs the ring city |
| 37 | RILL names the problem without a text dump | 🟦 | `react_w001_arrived` + `cal_w001_arrival` authored |
| 38 | Contract surface readable and reachable | 🟦 | Dispatch kiosk + objective board; device readability |
| 39 | Route legible through landmarks | ⬜ | **Wave 5** |
| **H. First real surface contract** | | | |
| 40 | One complete W001 contract accepted | 🟦 | 5 steps, all reachable after the marker fix |
| 41 | SCAN used as an already-learned verb | 🟦 | Teaching moves to W000 in the v2 contract (Wave 1) |
| 42 | Repair uses the canonical owner | 🟦 | `signal_relay` + `RepairMachineCountStep` |
| 43 | Safe discharge practice before pressure | 🟦 | Practice target placed on the route |
| 44 | Safe creature observation window | 🟨 | Orchestrator measures it; needs the FH-A01 species |
| 45 | Non-lethal creature resolution | 🟨 | FH-S05 signal shipped; encounter tuning pending |
| 46 | The designated zipline on the job route | 🟦 | Plaza → CanalRow line placed |
| 47 | Contract pays through canonical rewards | 🟦 | `JobRewards.Grant`, 100 credits |
| 48 | One physical story fragment or choice | ⬜ | Story lane |
| 49 | The world visibly changes | 🟨 | Relay powered; visible consequence pending |
| **I. Extraction and return** | | | |
| 50–53 | Fiction return path, board, commit, shortened act, no double-pay | 🟨 | Travel + field menu exist; the *fiction* route does not |
| **J. Home payoff and persistence** | | | |
| 54 | Ship visibly reflects the first job | 🟨 | Orchestrator verifies the decal PLATE, not the flag |
| 55 | One meaningful spend | ⬜ | Economy lane |
| 56 | RILL delivers the payoff and next mystery | 🟦 | `react_w001_job`, `react_first_job` authored |
| 57–58 | Autosave; quit → Continue restores all five categories | 🟨 | Full-route PlayMode + device relaunch |

---

## 2. The 22 first-hour beats

Producers are now wired for all 22 (`FirstHourDirector` + `FirstHourW001Orchestrator`); a test reads
`first_hour_beats.json` and fails if any beat loses its producer.

| Group | State | Remaining |
|---|---|---|
| 1–2 boot/new game | 🟨 | Bake + device |
| 3–8 W000 body and helm | 🟨 | **One Terry bake** (`Ziptide → First Hour → Author W000 Surfaces`) unblocks five at once |
| 9 first Ziptide | 🟦 | Moves origin to the space leg in the v2 contract |
| 10 arrival | 🟨 | Adapter placed by the bake |
| 11–16 job, scan, repair, power, practice | 🟦 | Device pass |
| 17–18 observe + counter the creature | 🟨 | FH-A01 species passport |
| 19–20 zipline + reward | 🟦 | Device pass |
| 21–22 return + changed-ship payoff | 🟨 | Payoff requires the visible decal, never auto-completes |

**All 15 teaching lines are authored** on the `Cue` trigger — they fire only after that beat's
hesitation interval, by id. A player who simply does the thing hears nothing.

---

## 3. Open device failures (M0, 2026-07-26)

| ID | State | Note |
|---|---|---|
| DV-01 Home Hub too close | 🟨 fixed | Tiles at 0.65 m; both reach gates now read the shipped constants |
| DV-02 no way out of a world | 🟨 fixed | Escape hatch moved onto the persistent rig, not a locomotion side effect |
| DV-03 sword orientation | 🟩 | Thrust carry at `fc598346`; **`QuestDeviceCorrectionsRuntime` overrides `ItemFactory` — that is why factory-side fixes kept "regressing"** |
| DV-04 holstered laser | ✅ | Controller-hand-only selection |
| DV-05 R3 crouch on the turn stick | ⬜ | ⚠ report-only (input actions) |
| DV-06/07 vehicle view + yaw | ⬜ | Vehicle mini-sprint |
| DV-08 toxic boundary | ✅ | Samples the visible surface |
| DV-09 spawns report buried torso | ⬜ | Both W000 and ToxicCity |
| DV-10 holster after travel unproven | ⬜ | Instrument first |
| DV-11 kinematic velocity warnings | ✅ | Velocity zeroed before the body locks |
| DV-12 no QA-safe route profile | ⬜ | Process |
| DV-13 1% lows ~36 fps, materials 206→414 | ⬜ | Perf pass |

---

## 4. What Level 1 still needs

| Item | State | Note |
|---|---|---|
| **The space leg** | 🟨 | The scene now GENERATES in the build — its patcher was menu-only, which is why it had never existed. Reachable from the ship's destination list. Still unbuilt: the route beats, the neutral signals, the reentry act, and retargeting `PUNCH IT` (locked travel contract — after a headset pass) |
| **The ring city** | 🟨 | The blocker was the data model, not effort: wedges, a canal ring and a leaning tower were unexpressible. `RingCityDef` + `RingCityBuilder` ship them; ToxicCity takes the tidal flat, canal ring, sea wall, harbour, outskirts and horizon pillars now. Tower island and wedges wait for the district re-layout |
| **FH-A01** | 🟨 | Species decided and documented (`W001_SIGNATURE_CREATURE_PASSPORT.md` — the Husk-Molter). Remaining: the contact sheets, and the pair shot that decides whether the molt is fair |
| **Audio rails** | ⬜ | Mixer, buses, event registry, sliders, ducking, non-speech captions. Then assets. Both first-level worlds now at least have music assigned |
| **The shell** | ⬜ | Volume sliders, save-failure UX, version display, legal screens, credits, the doff/resume × 7 matrix |
| **Level 2** | 🟨 | No longer locked — the contract that grants `toxiccity_complete` can complete. Still owed: W002 built from the same route packet and measured for replication speed |

---

## 5. The Terry batch (one PC session, everything at once)

1. `Ziptide → First Hour → Author W000 Surfaces` → commit `W000_DriftIn.unity`
2. `Ziptide → Worlds → Build Toxic City` + `Build Toxic City Contract`
3. `Ziptide → Worlds → Build Space Lane (Flight Trial)` → commit scene + pack
4. W002: delete the stale layout asset, reseed, re-bake interiors + portal culling
5. `Ziptide → Worlds → Export All World Specs (JSON)` → commit `docs/worldspecs/*.spec.json`
6. Create-only reseeds: W005/W007 vistas, both arena assets, `swarm_bug` + `light_grazer` bodies
7. Commit the 7 migrated `Resources/Enemies/*.asset` stat rebaselines

---

## 6. Update contract

A row moves state only with its proof in the same commit: a CI run id, a PlayMode result, or a device
log line. `code-green is not device-green` — a 🟨 row that has never been in a headset stays 🟨 no
matter how many tests pass.
