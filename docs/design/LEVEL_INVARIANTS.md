# LEVEL INVARIANTS — what must be true in EVERY world, and what must be put back on the way out

**Written 2026-07-29**, from Terry's instruction: *"a list of all the things that should be correct
on a level… we need this to be consistent across all levels — things like but not limited to the
character height, the placement, anything that would be important to be consistent, especially
across basically all levels, unless we purposely adjusted and then make sure it's adjusted back when
you leave the level."*

**Why this is a separate document.** The project already has per-world specs, per-world audits and a
per-world runbook. What it has never had is the list of things that must be **identical everywhere**.
Every one of the worst bugs this project has shipped lives in that gap: the player who was fourteen
feet tall, the menu two metres out of reach, the spawn inside the ground, the walk speed that was
still wrong after leaving the arena. None of those are world bugs. They are *invariant* bugs, and no
per-world check was ever going to catch them.

**How to read it.** Every row is either **[FIXED]** — identical in every world, no exceptions — or
**[TUNABLE]** — a world may deliberately change it, and must therefore restore it on exit. A tunable
with no restore path is a bug, not a feature.

**Status key:** ✅ enforced by a gate · 🟨 enforced by one owner but not gated · ⬜ not enforced yet.

---

## 1. THE PLAYER — the body you arrive in

The rig is `DontDestroyOnLoad`. Anything a world sets on it **survives that world**. That single fact
is the source of this whole category.

| # | Invariant | Class | Status | Owner / note |
|---|---|---|---|---|
| P1 | Eye height comes from real Quest floor tracking. No synthetic adult offset is ever stacked on top. | [FIXED] | ✅ | `PlayerRigHeightContractEnforcer`, enforced **outside** the patcher's exception swallow — that swallow is what shipped the fourteen-foot player |
| P2 | Plausible eye height is 0.45–2.3 m; outside that is a blocker, not a warning | [FIXED] | ✅ | `PlayerSafetyRuntime` |
| P3 | The player arrives **standing**. A crouch is a posture inside a world, never a travel condition | [TUNABLE → restore] | 🟨 | `LevelStateContract.ForceStand` |
| P4 | The player arrives **unstunned and at full move speed** | [TUNABLE → restore] | 🟨 | `LevelStateContract` — a slow belongs to the fight that caused it |
| P5 | The player arrives **able to move**. Every locomotion suspension has an explicit release | [TUNABLE → restore] | 🟨 | `LevelStateContract`; the ONE exemption is `_Boot`, which has no floor |
| P6 | The rig is **never parented** — to a hull, a vehicle, a zipline or a lift. Movement is delta-translation only | [FIXED] | 🟨 | Locked contract; `LevelStateContract` repairs and names a violation |
| P7 | Comfort preset (Cozy/Standard/Bold) is device-level and survives a profile wipe | [FIXED] | 🟨 | `ComfortSettings` in PlayerPrefs — it belongs to the person, not the save |
| P8 | Exactly one `InputActionManager`, one XR Origin, one interaction manager | [FIXED] | ✅ | `PlayerRigPersistence`; `DUP_SINGLETON` |
| P9 | Only **holstered** items travel. Loose and in-hand items belong to their world | [FIXED] | 🟨 | Locked contract |
| P10 | The player can always escape: a pause surface with return-to-hub exists in every content world | [FIXED] | 🟨 | `PlayerMenuRuntime`, ensured by the rig — **not** as a side effect of a locomotion component (DV-02) |
| P11 | Fall safety is armed in every gravity world and re-armed from each fresh spawn | [FIXED] | 🟨 | `PlayerRigPersistence` hard fall limit + absolute floor |
| P12 | `Time.timeScale` is 1 on arrival | [FIXED] | 🟨 | `LevelStateContract` |

---

## 2. THE GROUND — where the player stands

| # | Invariant | Class | Status | Note |
|---|---|---|---|---|
| G1 | `walkwayHeight` is the world's one canonical walkable Y. District slab TOPS sit at it | [FIXED] | ✅ | Audit reads it |
| G2 | Every district has an enabled, non-trigger `*_Ground` collider | [FIXED] | ✅ | `WORLD_DISTRICT_FLOOR_MISSING` |
| G3 | The spawn is on solid ground and **not inside any collider** | [FIXED] | ✅ | `SPAWN_OVERLAP_SOLID` — this caught the tidal flat that had become a 380 m pill |
| G4 | Every walkable surface a player can reach has a collider that matches its **mesh footprint** | [FIXED] | 🟨 | A Unity cylinder's capsule collider does NOT scale like its mesh; discs get a box |
| G5 | The fall threshold sits below the lowest walkable surface, not below zero | [FIXED] | 🟨 | Per-world `fallYThreshold` + the rig's global net |
| G6 | Connections between districts are walkable end to end — no floating slabs, no gaps | [FIXED] | ✅ | `RouteContinuityAuditRules` |

---

## 3. PLACEMENT — nothing spawns inside anything

This is the category that was silently broken in the first level, and it is the one that cannot be
eyeballed. All five findings below came from computing footprints, not from looking.

| # | Invariant | Class | Status | Note |
|---|---|---|---|---|
| PL1 | A **ground** creature zone overlaps no building, facade row, prop cluster, canal or hazard volume | [FIXED] | ⬜ | Three of ToxicCity's four zones failed this on first authoring |
| PL2 | A **flying** drone zone overlaps no hazard volume and no enclosed building — it *may* pass over low props | [FIXED] | ⬜ | `Patrol_Canal` sat inside an acid canal, so the player had to stand in acid to fight it |
| PL3 | Every interactable a beat requires is inside the reach envelope (0.35–1.9 m) from a settled standing, seated **and child-height** pose | [FIXED] | ⬜ | PG-4 envelope; the Home Hub failure lives here |
| PL4 | A grabbable handle is reachable **from the ground** unless the world provides the climb | [FIXED] | ⬜ | The job zipline was strung 7 m up in a flat city: a perfect zipline nobody could ride |
| PL5 | Every `GoToMarker` id in a contract resolves to a real transform | [FIXED] | ✅ | `JOB_MARKER_MISSING`; three of four first-level steps were dead |
| PL6 | Machines and their parts sit inside the room they belong to, clear of the interior plinth | [FIXED] | ⬜ | Machine positions are world-space because the JobDirector sits at the origin — easy to get wrong |
| PL7 | The travel door is reachable and outside any hazard | [FIXED] | ⬜ | |

---

## 4. SCALE AND COMPOSITION — what the world reads as

| # | Invariant | Class | Status | Note |
|---|---|---|---|---|
| S1 | Depth order outward: playable ground → scatter/outskirts → distant skyline → horizon vista anchors | [FIXED] | ⬜ | ToxicCity's fake skyline sat at 140 m *inside* outskirts that reach 190 m, so wrecks rendered behind distant buildings |
| S2 | Horizon vista objects have **no colliders** and sit beyond the playable radius | [FIXED] | 🟨 | The gate pillars' whole meaning dies if you can walk to one |
| S3 | Every world assigns its sky/theme. A world that sets none inherits the last one | [FIXED] | ⬜ | ToxicCity showed the *old* sky for exactly this reason |
| S4 | Fog is set explicitly by every world, on or off | [FIXED] | 🟨 | `RenderSettings` comes from the loaded scene; single-load makes this safe, additive would not |
| S5 | Hero landmark ≈ 2× its tallest neighbour, and visible from the arrival frame | [TUNABLE] | ⬜ | Prospect bar |

---

## 5. AUDIO

| # | Invariant | Class | Status | Note |
|---|---|---|---|---|
| A1 | Every world has an `AudioProfile` | [FIXED] | ⬜ | Both first-level worlds shipped with none while the asset existed, unassigned |
| A2 | Every scene maps to an ambience biome | [FIXED] | 🟨 | `BiomeAmbience.BiomeForScene` |
| A3 | Music and ambience respect the player's mix; zero means silent | [FIXED] | ✅ | `AudioMixCore` |
| A4 | The mix is device-level and survives a profile wipe | [FIXED] | 🟨 | A volume belongs to the room, not the campaign |
| A5 | Every safety- or progress-critical sound has a visual twin | [FIXED] | ⬜ | The game must be completable with the sound off |

---

## 6. DATA AND STORY

| # | Invariant | Class | Status | Note |
|---|---|---|---|---|
| D1 | Every world pack declares a `player` spawn marker | [FIXED] | ✅ | |
| D2 | Every world grants its completion flags | [FIXED] | ⬜ | ToxicCity granted none, so W002 was locked forever |
| D3 | Every world's exit destination is a real, shipping, intended scene | [FIXED] | ⬜ | W000's exit pointed at a test scene |
| D4 | A world's layout asset carries every field its builder reads | [FIXED] | ⬜ | One stale asset switched off terrain, vista, POIs, hazards and creatures at once |
| D5 | No world lists **itself** as a travel destination | [FIXED] | ⬜ | Caused by an empty `sceneName` |
| D6 | Every world has ≥1 completable contract, and RILL has ≥1 line for it | [FIXED] | ⬜ | |
| D7 | A returning profile never meets a teaching beat | [FIXED] | ⬜ | FH-GAP-1, veteran skip — still unimplemented |

---

## 7. THE RESTORE LAW

> **A world may adjust the player. A world may not let the adjustment leave with them.**

Every `[TUNABLE]` above needs three things, and a tunable missing any of them is a bug:

1. **A named owner** that applies it.
2. **An explicit release** — on the owning state ending, *and* on world change.
3. **A log line on both edges**, so a leak is visible in a logcat instead of being felt as "the game
   went weird after the arena".

`LevelStateContract` is the backstop, not the excuse. It runs on the persistent rig on every world
load and restores the baseline — and when it has to repair something it logs
`LEVEL_STATE_REPAIRED` as a **warning**, naming what leaked. A silent reset would hide the bug; the
loud one tells you which system forgot.

**Known leaks it currently catches:**

| Leak | Symptom | Owner that should have released it |
|---|---|---|
| Crouch across travel | Short CharacterController and dropped camera in the next world | `DashLocomotion` |
| Stun slow across travel | *"Walk speed is wrong after the arena"* — an unexplained line on the device checklist for weeks | `PlayerStunReceiver` |
| Suspended locomotion | Cannot move on arrival | `PlayerMenuRuntime` / boot hold / vehicle mount |
| Surviving rig parent | The player is dragged by geometry from the previous world | any conveyance |

**The one exemption is `_Boot`**, and it is load-bearing: `_Boot` has no floor, so its boot hold
suspends movement deliberately. Force-enabling locomotion there would let a player walk off into
nothing — turning a safety feature into the exact fall this project has already shipped once.

---

## 8. HOW THIS GETS ENFORCED

Anything that reads only committed **data** belongs in `tools/*.py` and runs in about a second.
Anything that needs a live scene belongs in the world audit. Nothing that reads only `docs/**` ever
goes in a Unity test — that rule cost this project a 27-hour red streak once already.

| Layer | Covers | Where |
|---|---|---|
| Python preflight | §3 placement, §4 depth order, §5 A1, §6 data | `tools/level_invariants_gate.py` |
| World audit | §2 ground, §3 reach, §4 colliders | `Editor/Audit/**` |
| EditMode tests | the pure rules behind all of it | `Tests/EditMode/**` |
| PlayMode | §1 and §7 — the restore law across a real travel | `Tests/PlayMode/**` |
| Device | everything that is a *feel* question, and nothing that is not | `TERRY_RUNBOOK.md` |

**The ratchet:** every new ⬜ row that a bug proves real becomes a ✅ row in the same commit as its
fix. A fix without a gate is a loan.
