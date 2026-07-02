# 🎯 GAME_PLAN — THE ROAD TO AAA (graybox → shipped Meta Quest game)

**This is the roadmap-of-record.** It supersedes `FABLE5_BACKLOG.md`'s phase list as the map;
the backlog is now the **current-milestone queue** (it holds the expanded tasks of whichever
milestone is active). The active sprint's live state is always `docs/SPRINT.md`.

> **Honest framing (read once, then stop relitigating it):** "AAA" for a solo owner + an LLM team
> means **premium-indie at Quest-native quality** — locked 72fps, cohesive kit-based art, full VO,
> a complete 80-world story arc, store-certified. That is achievable with this architecture (data-driven
> worlds, patcher indirection, CI-verified everything). A 500-person-studio *asset volume* is not, and
> this plan does not pretend otherwise: it buys "big game" feel through **systemic variety**
> (biomes/creatures/tools/story) rather than bespoke art per square meter.

---

## §1 The gap matrix — what AAA requires vs what exists

| Aspect | HAVE (verified) | MISSING (this plan's work) |
|---|---|---|
| **Core loop** | jobs→bounty→economy wired; story gating; 12 worlds | The repair FANTASY (machines as hands-on gameplay), collectibles (real Collect/Deliver), in-world mining/garden placement |
| **Story delivery** | flags, Transmission data layer, WORLD_DATA records | **RILLCompanion** (the emotional spine — specced in MASTER_BUILD_PLAN §14, zero code), Signal-reactive worlds, choice set-pieces (W043/W046/W063), de-garble UI |
| **Worlds** | world factory + 11 authored graybox (W002–W012) | 68 more; per-biome hazard mechanics (wind/static/flood/spore/radiation); W000 tutorial (needs ship) |
| **Creatures** | drones + difficulty variants + data catalog | `CreatureBehavior` framework, 4 archetypes, Wardens, 12 novel behaviors |
| **Gear** | 5 items, data-tunable visuals | Scan-Pulse/Gravity-Glove starter trio, ~8 story-required tools, ToolRecipe generalization |
| **Ship** | data model (`ShipDefinition`) + locked architecture (`SHIPS.md`) | S1 boardable → S2 fly-out → S3 upgrades/salvage → cockpit |
| **Art** | primitives + per-world palettes/skies | THE ENTIRE art pass: SurfaceSet kits, ArtBuildPlan pipeline, real meshes/LODs, baked lighting, VFX, perf-budget audit |
| **Audio** | AudioProfile hooks | RILL VO (ElevenLabs), the Transmission voice (male+female blend), adaptive 4-stem music, SFX library |
| **Modes** | PvP solo+bot done | PvP Photon (Ph.3-4); Tidefront stays parked/post-launch |
| **UX / Ship-it** | dev menu, dev sideload | Title/settings/comfort/save UX, accessibility, Meta store cert, trailer/store assets, QA |

---

## §2 The milestone map (ordered by DEPENDENCY, not preference)

**Ordering rationale:** systems before scale (the 68 remaining worlds must be authored WITH hazards/
VO-hooks/creature-hooks — retrofitting 80 worlds is how projects die) · art after shape stabilizes
(the most expensive thing to redo) · cert last. Each milestone runs as a **sprint** (SPRINT.md protocol,
§3) and ends at its **acceptance gate**.

### M0 — Device-Proof *(NOW, Terry-gated 🎮)*
The pending device pass on the 11-world build; fix the ❌s that come back.
- 🎮 Run `TERRY_RUNBOOK.md` §1 bake + `DEVICE_TEST_CHECKLIST.md` (incl. the §2b 11-world smoke list).
- ⚙ Fix every ❌; re-dispatch APK.
- **Gate:** Terry walks W001→W012 with working rig/guns/travel; no blocker ❌s open.
- *Rationale: everything below layers on a confirmed base. M1 can proceed in parallel (all ⚙CI).*

### M1 — The Story Speaks *(all ⚙CI-heavy — the current sprint)*
The game gets its voice: RILL talks, the Signal is a number worlds can read, fragments are physical.
- ⚙ **RILLCompanion**: text-subtitle delivery FIRST (VO clips slot in later); `RillState` pure
  memory-state machine (per `rillBehaviorNote` + RILLMemoryState in MASTER_BUILD_PLAN §14); a
  `RillLineLibrary` data asset carrying the ~12 arc beats from `WORLD_DATA.md`/`STORY_BIBLE.md`;
  world-space panel (TextMesh — the no-TMP convention); triggers on world-entry/flag-grant/job-complete.
- ⚙ **SignalState** service: tier derived from flags (pure, tested); worlds/Wardens/tides read one number.
- ⚙ **Collectible/mystery-object system**: physical pickups that credit `CollectItemIdCount` steps →
  unblocks real Collect/Deliver contracts AND makes Transmission fragments actual objects you grab.
- ⚙ **Choice set-piece** (`ChoiceStation`): two-option interactable writing flags — covers every branch
  beat in the 80-world catalog; RILL reacts.
- ⚙ **De-garble playback stub**: `TransmissionProgress` tier → tier's text variant (audio in M6).
- **Gate:** CI green incl. new tests; APK dispatch green; Terry's next pass **hears/reads RILL in
  W001–W012**, picks up a real fragment in W004, makes one choice.

### M2 — The Job Is Real *(⚙ + 🎮 feel)*
The contract-tech fantasy becomes hands-on, not fetch-quests.
- ⚙ **Machine repair loop**: multi-step hands-on fixes (open panel → socket part → power cycle) via
  sockets/tools; a `MachineDefinition`-driven repair contract step type.
- ⚙ In-world **mining/garden placement** (backends exist — `EconomyState`/GardenService; place them
  as world objects that feed the live profile).
- ⚙ **Per-biome hazard zones**: wind/static/flood/spore/radiation components authored from layout data.
- ⚙ **Starter-gear trio** onboarding order: Scan Pulse → Taser → Gravity Glove (per gear catalog).
- **Gate:** one world (W002) plays a full "arrive → scan → repair → collect → paid" loop on-device.

### M3 — Living Worlds *(⚙ framework + 🎮 tuning)*
- ⚙ **CreatureBehavior framework** (generalize `DroneRuntime.CombatDriven` seam) + the 4 archetypes:
  Swarmer (boids), WallCrawler (surface-stick), Flyer (hover/dive/steal), Bruiser (charge).
- ⚙ **Wardens** (Signal-reactive, per SignalState tier).
- ⚙ First 4 **novel behaviors**, chosen for existing-gear counters: Witness-mite, Light-grazer,
  Tether-swarm, Husk-molter (`CREATURE_DESIGN.md`).
- **Gate:** W005/W009 encounters use non-drone archetypes; collision-clean; Terry rates them "alive."

### M4 — The Ship *(the north star 🚀; spec `docs/systems/SHIPS.md`)*
- ⚙/🔧 **S1 boardable shell** (dock in worlds, walk in, sit) → **S2 fly-out presentation** (board →
  cockpit → pick world → `TravelCoordinator` under a window animation) → **S3 upgrades +
  `IInteractableSpaceJunk` salvage** → ⚙ **W000 tutorial** (wake on the ship — needs S1/S2).
- **Gate:** travel doors retired in favor of the ship on at least the Ch.1 worlds; W000 playable.

### M5 — Content at Scale *(the world factory earns its keep)*
- ⚙ Ch.3–6 (W013–W051) in chapter batches via `WorldLayoutLibrary`/`WorldJobLibrary` — **each batch adds
  its chapter's new tool/creature/story beat so content and systems grow together (NOT retrofit).**
- ⚙ Then Ch.7–12 + the 4 endings (W052–W080); Transmission fragments T2–T5 placed; endings gated.
- **Gate:** all 80 worlds generate + audit green in one build; the full arc is walkable start→finish.

### M6 — Look & Sound *(PROMOTED by Terry 2026-07-02: runs NOW as its own parallel track — sprint file
`docs/SPRINT_ART.md` ("Picasso"), zero file overlap with the story + MP tracks. Order: skyscapes
everywhere → W001 art kit → audio foundation → creatures/gear/VFX.)*
- ⚙/🔧 The art pipeline as specced (`project_art_plan/`): `SurfaceSetDefinition` kits → ArtBuildPlan →
  a **PERF_BUDGET audit rule** (tri counts/draw calls per world fail the build if blown) → swap
  primitives per kit family, **W001 first** as the proof.
- ⚙ RILL VO (ElevenLabs) + the Transmission recordings (male+female blend) slotted into M1's line data;
  `AdaptiveAudioManager` (4-stem, Signal-reactive per `ADAPTIVE_AUDIO.md`); SFX library; VFX pass.
- **Gate:** W001 & the Ch.1 band look/sound shipped-quality at 72fps on-device.

### M7 — THE MULTIPLAYER PROGRAM *(promoted + expanded by Terry 2026-07-02; runs in parallel with
M3+ as its own track — own sprint file `docs/SPRINT_MULTIPLAYER.md`, zero file overlap with the story
milestones. Designs: `docs/design/PVP_ARENA_AAA.md` + `docs/design/TIDEFRONT_AAA.md`.)*
- **M7a — Arena (PvP → AAA fun):** A1 Bot Brain (pure utility AI + difficulty-as-data; thread
  `IPvpTransport` + `WeaponCharge` while rewiring) → A2 Arena Factory (5 biome arenas w/ cover points +
  waypoint graphs) → A3 Modes (N-combatant matches; Gun Game / KotH / Fragment Rush / Horde-with-M3-
  creatures) → A4 Arsenal (6 weapons, pad pickups) → A5 Progression (credits into the ONE economy,
  unlocks, daily seed). **Gate:** a stranger plays 10 varied matches and wants an 11th.
- **M7b — Online:** Photon PUN2 (Terry's PC) behind the already-threaded transport → remote avatar →
  room codes → voice → polish. **Gate:** two headsets play a room-code match.
- **M7c — Tidefront (the Risk layer — UN-parked):** B1 conquest sim (pure, deterministic, story-worlds-
  as-map) → B2 holo war table vs AI → B3 **VR mission modifiers** (attacks/defenses offer real contracts
  inside the actual worlds) → B4 hotseat → Photon live. **Gate:** two players finish a war; missions
  change outcomes. *(League/seasons/ranks stay post-launch, §4.)*

### M8 — Ship It
- ⚙/🎮 Title/settings/comfort/accessibility/saves UX · perf hardening to the budget doc · Meta store
  cert checklist · store assets/trailer · QA matrix across all worlds/modes.
- **Gate:** store submission accepted.

---

## §3 The two standing contracts (what makes this LLM-changeable + crash-proof)

### The changeability invariant
Every system ships as: **data model (ScriptableObject) + code-authoring library (create-only, never
overwrites edits) + a `HOW_TO_CHANGE_ANYTHING.md` row + EditMode tests + `ZIPTIDE:` diagnostic tags +
an audit rule if it can break a build.** Story changes flow ONE pipeline:
`STORY_BIBLE.md → WORLD_DATA.md record → library spec → next build`.
"Change the story and the game follows" — mechanically true, and the playbook row is the contract.

### The resumability protocol
Each milestone runs as a sprint with a live **`docs/SPRINT.md`** (task board + "▶ RESUMING?" exact next
action), **updated in the same commit as every push** — a rate-limit cutoff strands nothing. Completed
sprints archive to `docs/sprints/`. This file (GAME_PLAN) is the standing map. The takeover prompt for
any fresh model stays one line: **"Read docs/SPRINT.md and continue."**

---

## §4 Parked (post-launch — do not build before M8)
Tidefront **ranked league/seasons/async-cloud-store** (the core Tidefront game is ACTIVE — M7c) ·
community world-builder · adult-variant content split · anything requiring a live-ops backend.

---
*Written 2026-07-01 by the operator (Fable 5), executing the approved ROAD-TO-AAA plan. Spec pointers:
`ZIPTIDE_MASTER_BUILD_PLAN.md` (§5 gear, §14 RILL), `project_art_plan/`, `systems/CREATURE_DESIGN.md`,
`systems/SHIPS.md`, `design/PVP_1V1_MODE.md`, `design/ADAPTIVE_AUDIO.md`, `storyboard/WORLD_DATA.md`.*
