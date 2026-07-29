# HANDOFF — session log (single operator ⇄ Terry)

## Required reading

- `docs/FABLE5_START_HERE.md`
- `docs/HANDOFF.md` — current entries (newest 10 only; see the archive below)
- `docs/FAST_LANE.md` — **read this before doing a small fix**; it says what ceremony you may skip
- `docs/HANDOFF_ARCHIVE_2026-07.md` — entries through 2026-07-28 (search, don't read)
- `docs/HANDOFF_HISTORY_THROUGH_RB24.md` — exact prior history through rb24
- `docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md` — Quest device recovery plan
- `docs/recovery/RECOVERY_PROGRAM.md` — active recovery authority, freeze, proof levels and R0–R4 path
- `docs/recovery/SYSTEM_CONTRACT_INVENTORY.md` — initial critical-system ownership map
- `docs/recovery/AUTOMATIC_RUNTIME_OWNERS.md` — automatic bootstraps, persistent mutators and feature injectors
- `docs/recovery/CANONICAL_OWNER_DECISIONS.md` — proposed surviving owners and displaced paths
- `docs/recovery/R1_INTEGRATION_HARNESS_SPEC.md` — locked next-phase verification design
- `docs/recovery/generated/` — generated source, scene, claim and ownership reports
- `docs/MASTER_CHECKLIST.md`
- `docs/FABLE5_BACKLOG.md`
- `docs/TERRY_RUNBOOK.md`

## Rules

1. Read the newest entry before work.
2. Append Did / Next / Heads-up / Commit at session end.
3. Work on `terry-local-wip`; pull before starting.
4. Keep commits small and stop implementation when required CI is red.
5. **Small, reversible change?** Use `docs/FAST_LANE.md` — one line in `FAST_LANE_LOG.md`
   instead of an entry here. Full ceremony is for structural work, not for a rotated weapon.
6. **Any check that only reads `docs/**` belongs in `tools/*_gate.py`, never in a Unity test.**

---

## ENTRIES — newest first

> **📕 Older entries live in `docs/HANDOFF_ARCHIVE_2026-07.md`** (archived 2026-07-28).
> Read them only when you need older context — reading the whole history every session is
> the read-in tax `docs/FAST_LANE.md` exists to stop. Oldest of all:
> `docs/HANDOFF_HISTORY_THROUGH_RB24.md`.


### 2026-07-29 (rb123) — Fable 5: the level is ~70% BUILT — the verified gap map (so I build the missing 30%, not duplicates)

- **Terry:** *"I want the whole thing actually built… everything the level is supposed to have… the
  whole thing could end up broken temporarily, we repair as we go."* Traced the actual CODE (not the
  tracker's ⬜ rows) to find what genuinely needs building vs what already exists. **Hard floor: the
  build must still COMPILE** — a level that won't build is one Terry can't test at all; "broken" = feel/
  gameplay, never a red tree.
- **VERIFIED ALREADY BUILT IN CODE (do NOT rebuild — no-parallel-system law):** W000 wake + coupler
  repair (`ShipCastOffRuntime` arming) · cast-off/PUNCH IT → `SceneSpaceLane` · **the space leg is real**
  (`ScenePatcherSpaceLane`: rings + drone SALVAGE targets via `SpaceTargetRuntime` + drift-parallax +
  `EnsureOnwardLeg` to ToxicCity + `EnsureTheFind` = the artifact on a wreck) · ring city (`CityBuilder`)
  · artifact FIND/JOIN/SOCKET (`ArtifactJoinRuntime`/`KeySocketRuntime`) · W001 contract (5 steps, marker
  fix) · **vehicles** (`ToxicCityVehicleBuilder`+`VehicleSafetyRuntime`+`VehiclePlayabilityTests`) ·
  teaching/RILL/Cal lines · `LEVEL_INVARIANTS` gates · `TravelCoordinator` already masks the scene loads.
- **THE GENUINELY-MISSING (the 30% I will build, breadth-first, each a compiling increment):**
  1. **Launch/reentry VEIL feel** (W-3) — the plasma ascent/reentry beat as an ADDITIVE presenter around
     `TravelCoordinator.TravelCompleted` (structure exists via the fade; this is the designed "one
     journey" polish). Stand-in VFX.
  2. **W001 ARRIVAL composition + landmark legibility** (W-5, reqs 36/39) — CityBuilder arrival framing.
  3. **First-contract polish** — story fragment (48), visible world-change (49), meaningful spend (55).
  4. **Extraction/return FICTION** (50-53) — the fictional return path (travel exists; the fiction doesn't).
  5. **Audio RAILS** (⬜) — mixer/buses/sliders/ducking + caption twins (stand-in SFX).
  6. **The SHELL** (⬜) — volume sliders, save-failure UX, version/legal/credits, pause.
  7. **W002 build** (Level 2 staged) — from the same route packet.
  8. Deferred by T-Dog rb121: beacon thread, hangar-walk-to-berth-6, W000 viewport/prior-owner log.
- **Execution:** build the above as compiling increments; keep CI green; **circuit breaker 3 reds →
  stop.** Terry runs ONE bake batch (`LEVEL1_MASTER_TRACKER` §5) + build + plays the WHOLE level, finds
  what's broken; defects → `MISS_LEDGER` so Level 2 can't inherit them. Art/SFX are Forge stand-ins now,
  Tripo swap in ~6 days via the applier.
- **Next:** build gap #1 (the launch/reentry veil presenter, additive). **Commit:** this entry.

### 2026-07-29 (rb122) — Fable 5 takes the LEVEL 1 baton: build it playable, start to finish

- **Terry, verbatim:** *"I want it all built, the whole level start to finish… I want to be able to
  actually play this level… one level, that's all we need… we've spent months planning, let's
  execute."* Discover-by-playing; broken things get **fixed AND logged so the next level doesn't
  repeat them**; art/sound are **modular stand-ins logged as yet-to-be-done** (real Tripo art in
  ~6 days once he's paid). Continuing from T-Dog's rb121 session close; T-Dog's Level 1 lane is
  wrapped, I hold it now (if T-Dog resumes we split by wave).
- **THE SYSTEMS TERRY ASKED FOR ALREADY EXIST — I will NOT build parallels (no-parallel-system law):**
  - *Stand-ins / art-swap:* `TRIPO_INTAKE_EXECUTION_PACKET.md` — everything ships with a **Forge-native
    placeholder now**; the real Tripo GLB **swaps in via `ForgeVisualApplier`** later; "no visual swap
    may erase gameplay tells." That IS "play now, pretty in 6 days." Nothing new needed.
  - *Defect → prevention:* `MISS_LEDGER.md` (every miss → class → a gate/test so it can't recur) +
    `LEVEL_INVARIANTS.md` (the cross-level consistency gates) + `FAST_LANE.md` (velocity). The
    play→find→fix→log→prevent loop is these, used per defect.
  - *State truth:* `LEVEL1_MASTER_TRACKER.md` (rows move only with proof; code-green ≠ device-green).
- **COMPLETION EXECUTION ORDER (to playable start→finish; each = Forge-stand-in content + CI-green
  proof + a Terry build+play checkpoint; defects route to MISS_LEDGER):**
  - **W-A · make the CURRENT slice playable TODAY** — the Terry bake batch (`LEVEL1_MASTER_TRACKER`
    §5: Author W000 Surfaces · Build Toxic City + Contract · Build Space Lane) so W000→helm→cast-off
    →W001→contract→artifact FIND/JOIN→return runs on device now, with a smoke checklist keyed to log
    tags so any break is localized in minutes (the anti-lost-day mechanism).
  - **W-2 · production space route** (W000→space→W001 chain from idempotent authors; PlayMode headless).
  - **W-3 · launch / first-Ziptide / reentry seam** (data-driven sequencer around `TravelCoordinator`,
    timeouts so presentation never strands travel, rig never parents the hull; reads as one journey).
  - **W-4 · flight + salvage playable pass** (soft-lock target, tractor-salvage, non-lethal disable).
  - **W-5 · W001 model-city arrival + route legibility** (arrival composition; landmarks read).
  - **W-6 · first contract + creature encounter + payoff + audio rails** (stand-in SFX; the Husk-Molter).
  - **W-7 · W002 replication** — Level 2 staged from the same route packet (Terry's "ready for level 2").
- **Next (this operator):** W-A — write the exact bake+smoke runbook so Terry can play the existing
  slice today, then start W-2 code (space route) keeping CI green; **circuit breaker holds — 3 CI reds
  on one task → stop and escalate.**
- **Commit:** this entry (ownership + order; docs-only, CI-safe).

### 2026-07-29 (rb121) — THE STORY: the first world was never built as one

- **Terry, verbatim:** *"It doesn't really make sense to punch it because we haven't even found the
  artifact yet at that point… which also tells me this world isn't really built, or at least not
  built correctly according to the story."* He was right, and the repo proves it.
- **THE CANON THE BUILD CONTRADICTED.** `docs/design/FIRST_HOUR_DIRECTORS_CUT.md` **v2.1 "The Key
  That Knew You"** (2026-07-16/17, Terry-directed) puts minute 10–13 at *"helm → PUNCH IT → cast-off
  rails (ship flight, **NO gate FX**)"* — a routine wreck-clearance job — and **the first Ziptide at
  minute 45**, from Cal's own berth, after the key is joined and seated. The build fired the full
  gate spectacle at minute ten for a bus ride and left nothing for the beat designed to earn it.
- **THE PROOF W000 WAS NEVER A STORY WORLD:** `docs/storyboard/` carries a per-world README for
  W001, W002, W003 and W004 and **none for W000**. It is an onboarding box with a ship bolted on.
- **FOUR CONFLICTING FIRST-HOURS EXIST** in the repo: W001's STORY.md (Cal *starts* at Toxic City,
  no W000, no artifact), CHAPTER_0-1 (RILL dark pre-boot, boots in W001), the Director's Cut
  (current authority), and the build. Named in `LEVEL1_MASTER_TRACKER` / the plan so the next
  operator does not re-derive it.
- **SHIPPED:** `PUNCH IT` demoted to an ordinary cast-off at the salvage lane with the gate
  suppressed — and the **AUTHOR** owns that route now, because the committed scene serialized
  `targetScene: ToxicCity` and relying on serialization would have meant regenerating the world did
  not fix it. The lane gained an onward leg to Toxic City (it was a cul-de-sac whose only exit was
  RETURN HOME, which is why the middle of the hour was unreachable). Three artifact
  `ItemDefinition`s; THE FIND on a wreck past the last ring; half B as the Dockmaster's paperweight
  in W001's pack; `ArtifactJoinRuntime` (both halves HELD, one per hand — two on the floor must not
  do it — and they REACH before they snap); `KeySocketRuntime` on the hull, the **only** thing in
  the game that re-arms the gate, with a test asserting nothing else ever hands it back; ten lines
  including Vex Bootstrapper's signature on both work orders.
- **⚠ NOT BUILT, deliberately:** the beacon thread and the hangar walk to berth six (a LOOK problem
  needing a reference plate, not code to guess at), W000's viewport and prior-owner log — *the
  opening of a space game still has no space in it* — and W002's rebuild/defend/grow loop.
- **⚠ NOTHING IN THE THREAD HAS RUN IN A HEADSET.** The join and the find are exactly the two beats
  whose FEEL decides whether any of it works.
- **Five CI reds, all mine, all named:** a copied constant, a namespace, leaked colliders, a missing
  import, and a test that failed on its own documentation. The three logging fixes added at the
  start of the session (failing test names · compile errors · audit blockers) each caught something
  on first use and turned a 20-minute artifact dig into one line.
- **Next:** `docs/production/LEVEL1_MASTER_TRACKER.md` and `docs/design/LEVEL_INVARIANTS.md`.

### 2026-07-29 (rb120) — LEVEL 1: four silent faults, then the missing middle

- **Terry, verbatim:** *"we're building the whole f****** level… I want everything from level one
  built all the way to level two. Everything down to the finest detail."* Protocols off, one goal.
- **FOUR FAULTS, EACH INVISIBLE TO CI, EACH DISABLING PART OF LEVEL 1.** Every owner satisfied its
  own contract; nothing asked whether a player could finish the level.
  1. **The first level's contract could never complete.** `JobDirector.CheckGoToMarker` searched only
     pack-declared markers. ToxicCity declares ONE (`player`) while three of its four steps point at
     `dispatch_inside` / `relay_node` / `shipyard_office` — objects `CityBuilder` authors straight
     into the scene. Step data right, scene right, lookup returned null. **W002 was therefore locked
     forever**, since its gate needs `toxiccity_complete`, which only the finished contract grants.
     Fixed: cached scene fallback + a loud `JOB_MARKER_MISSING` when an id cannot resolve at all.
  2. **W000's exit door led to a test scene** (`MilestoneA_GrabCube`). Retargeted.
  3. **One stale asset switched off half of W001.** `ToxicCityLayout.asset` predated `sceneName`,
     `experience`, `pois`, `hazards`, `creatureZones` and the sky block — all defaulted off — and the
     empty `sceneName` made ToxicCity list ITSELF as a ship destination.
  4. **Silent and empty.** No audio profile on either world though the asset existed; ZERO creatures
     placed though seven species ship; three RILL lines for both worlds combined.
- **THE MISSING MIDDLE WAS A MENU ITEM.** `ScenePatcherSpaceLane` is 316 complete lines — dock,
  cockpit frame, helm, five-ring course, three disable-and-salvage targets. It was reachable only
  from a menu item whose header asked Terry to run it once by hand. Nobody ever did, so
  `SpaceLane_Trial.unity` never existed and every plan calling the space leg "code-only" was
  describing a world that could not exist. **A world only a human can generate is a world that does
  not exist** — it now self-generates in the build like every other scene. Same class, same fix for
  `FirstHourSurfaceAuthor` (W000's comfort console, bunk keepsake and curated helm were absent from
  every APK while their code sat CI-green).
- **FH-S05 + FH-S08 shipped.** `CreatureDisabled` once per down cycle; `FirstHourDirector` conducts
  all 22 beats from the real owners' signals; `FirstHourW001Orchestrator` measures the safe
  observation window from the tracked head pose at ≥3 m and completes the payoff ONLY when the decal
  plate is on the hull. All 15 teaching lines authored on a new `Cue` trigger — they fire by id after
  the beat's hesitation interval, so a player who just does the thing hears nothing.
- **FH-A01 decided:** the Husk-Molter, because its counter is the only one that is a LESSON — swing
  at the decoy and you learn to read a creature before acting. Passport committed.
- **The ring city was UNEXPRESSIBLE, not unbuilt.** Rectangular districts and canals, and a landmark
  of {name, pos, h, w}. `RingCityDef` + `RingCityBuilder` ship all six elements; ToxicCity takes the
  tidal flat, canal ring, sea wall, harbour, outskirts and horizon pillars now. Tower island and
  wedges wait for the district re-layout — turning them on today would stand a 78 m leaning tower
  through the Dispatch plaza.
- **Audio:** no volume control existed anywhere. `AudioMixCore` + `AudioMixSettings` — zero means
  silent, stored in PlayerPrefs because a volume belongs to the room and not to a campaign.
- **Process:** the CI gate now names the failing test AND prints compile errors; `ci.yml` got the
  concurrency group it was missing. Both were open `FAST_LANE.md` §4 items, and both paid for
  themselves inside this session.
- **⚠ Heads-up:** `QuestDeviceCorrectionsRuntime` runs AFTER `ItemFactory` and overrides its grip.
  That is why every factory-side sword fix appeared to regress. `PUNCH IT` is deliberately NOT
  retargeted at the space leg yet — locked travel contract, and aiming the tutorial's one-way launch
  at a leg nobody has flown could block the whole level on a device session.
- **Next:** `docs/production/LEVEL1_MASTER_TRACKER.md` holds every remaining row and its proof.

### 2026-07-28 (rb119) — T-Dog lane: ⚡ FAST LANE — velocity fix after three trivial changes cost SIX HOURS

- **Terry, verbatim:** *"it literally needed to do like two or three fixes… basically just change
  the direction of the sword and like two other minor things… that took literally 6 hours. If it is
  going to take that long then I'm just tossing this project in the trash."* Treating this as a
  project-ending defect, because it is one.
- **MEASURED diagnosis (not guessed):** `docs/HANDOFF.md` is **3,155 lines / 138 entries**, and the
  laws require reading it + CLAUDE.md + OPERATOR_START_HERE + the checklist = **~3,600 lines of
  read-in EVERY session before any code**. Add per-change ceremony (HANDOFF entry, ledger, ratchet
  gate, board re-dating, catalog reconcile) applied equally to a sword angle and to a new subsystem.
  Add CI at a **60 min median / 176 min p75** verdict-to-verdict. Add a measured **27-hour
  continuous RED streak** (07-25 17:18 → 07-26 20:19) caused by a **stale board row, not code**.
  Add three serialised workflows before an APK exists. Add hand-written install steps regenerated
  per build. **Root cause: we built governance for 80 worlds and are running single screws through
  it.**
- **Shipped now:** **`tools/install_latest.ps1`** — one permanent installer (finds newest APK,
  prints SHA-256, uninstall-first per rb109, clears logcat, installs, supports `-Serial`/`-Both`,
  prints the logging command). Nobody hand-writes install instructions again.
  **`docs/FAST_LANE.md`** + **`docs/FAST_LANE_LOG.md`** — the tiered protocol.
- **⚠️ DISCOVERY THAT REMOVES AN OPERATOR FROM THE LOOP ENTIRELY:** `recovery-golden-android.yml`
  already has `workflow_dispatch`. **Terry can build his own APK from the GitHub mobile app**
  (Actions → Recovery Golden Android → Run workflow). ~12 min to artifact. He never has to wait on
  a session for a build again.
- **THE FAST LANE (adopted):** ≤3 files / ≤30 lines · no new system or contract · not touching rig,
  travel, save, input, build config or recovery artifacts · single-revert reversible · correctness
  decided by compile + existing tests + Terry's eyes. **Waives:** HANDOFF entry (one line in the
  fast-lane log instead) · MISS_LEDGER unless the class repeats · the ratchet gate (deferred as
  logged debt) · EXCELLENCE_MAP row · board reconcile · a dedicated build (batched). **Never
  waives:** compile, existing tests, revertibility, and the report-only law.
- **📣 MECHANICAL FIXES FOR WHOEVER OWNS CI (ranked by minutes saved, none change what gates check):**
  ① **Archive HANDOFF.md** to ~10 newest entries — a **10× cut to the per-session read-in tax**,
  highest-value single action available. ② **Move doc/board staleness checks OUT of Unity EditMode**
  — `GateGap5_NoBoardClaim_RotsSilently` costs a full Unity boot + 1,171-test run to report a stale
  date that `factory_governance_gate.py` reports in ~1 second; **this alone would have prevented the
  27-hour red streak**. Any check reading only `docs/**` belongs in the python preflight.
  ③ **Add a `concurrency` group to `ci.yml`** (golden-android already has one). ④ **Print the failing
  test name into `CI_VERDICT.md`** — finding it currently means downloading and parsing NUnit XML by
  hand. ⑤ One dispatch that chains CI → PlayMode → Golden APK unattended.
- **Not done unilaterally:** the HANDOFF archive and the EditMode→python gate move touch shared
  files while GPT is mid-fix; colliding there would cost more than it saves. They are specified
  precisely enough to apply in minutes.
- **Commit:** this one (installer + fast-lane protocol + log + this entry).

### 2026-07-28 (rb118) — T-Dog lane: 📋 ALL 24 CONCEPT SHEETS GIVEN BODY-CONTRACT PASSPORTS (pre-Tripo, docs only)

- **Terry's ask:** get the whole concept batch wired in while GPT works the device fixes.
  Art lane is collision-free with the runtime lane. Expanded
  **`docs/project_art_plan/BESTIARY_BODY_CONTRACTS.md` from 3 → 24 passports** (770 lines):
  21 creatures/machines + 3 plants, each with the four gate-required headings (Scale and
  ergonomics · State vocabulary · Interaction and collider envelope · Build acceptance).
- **Every passport settles what is IRREVERSIBLE at mesh time:** metre scale + threat-height band +
  closest-approach-to-headset · locomotion mode · the silhouette set the rig must reach (incl. the
  new pre-mesh **ambient** pose) · attack anatomy → required joints · full socket map (weak-points,
  face-safe grab handles, tether, feet, VFX/audio mounts, carry handle) · separable parts WITH cap
  geometry · tell-channel material region · collider proxies · variant mechanism. Every entry
  carries its **CI-required COUNTER** (`CreatureBehaviorReadabilityCatalog` enforces it).
- **Pre-mesh constraints surfaced that would have cost a remodel:** tether-swarm must be ONE
  instanced object, not 7 agents · light-grazer's two sizes should be a blendshape (continuous
  inflate), not a mesh swap · tide-phase's phase transition is a whole-body height-masked material
  pass, so UV/material regions must support it · bridge-former's BRIDGE FORM needs its own authored
  walkable collider · mimic-vault's closed box must match real container dimensions exactly and its
  unfold must not sweep through the player's head at 1 m · seal-ward is wall-mounted (scene socket,
  not floor spawn) · inverter's rig must be genuinely reversible (no "up" assumption) · orbit-grazer
  has no feet and the rig must not assume a floor · rewinder's after-images are ghost meshes, budget
  3 concurrent draws · tide-kite needs a carry socket between its hooks for the stolen item ·
  sludge-melon is a two-handed carry so it needs two opposed grab handles.
- **Flora passports added** (dew_bulb, rust_fern, sludge_melon) with the plant-specific pre-mesh
  call: **growth stages as separate meshes, harvested output as its own independent prop** with cap
  geometry, plus harvest-reach checked against seated/child height.
- **§0 rewritten** as the full 24-sheet ingest recipe (filenames, `concepts/bestiary_ch1/` +
  `concepts/flora_ch1/`, manifest entry shape, the four requiredMarkers, gate run). Registration
  still deliberately deferred — `concept_intake_gate.py:214` verifies conceptPaths exist on disk
  and the PNGs are still on Terry's phone. **Outstanding sheets: `sump_tender`, `glass_reed`.**
- **Verified:** `concept_intake_gate.py` PASS · `catalog_doc_sync_gate.py` 0 blocking / 0 warnings.
  No runtime, recovery, first-hour, licensing or release file touched.
- **Commit:** this one (passports + this entry).

### 2026-07-28 (rb117) — T-Dog lane: 🌾 AMBIENT LIFE research → creature card BLOCK D + ⚠️ TWO VERIFIED PERF BUGS

- **Terry's ask:** creatures should be DOING something when you come across them, gated by distance
  or line of sight so it doesn't cost performance. Researched (AC Unity AI Recycling, KCD2, Horizon,
  Rain World abstractization, theHunter animal AI, Watch Dogs Census, Nemesis, Unity CullingGroup /
  animator culling, Quest budgets, VR impostor limits) and wired the result into
  `CREATURE_ROSTER_AND_PROMPT_QUEUE.md` as **Block D + field 3b + the tier model + the anti-pop-in law**.
- **⚠️ TWO LIVE PERF BUGS FOUND AND VERIFIED IN SOURCE** (`CreatureBehaviorBase.cs`, runtime lane —
  NOT my lane to fix, flagging for the owner):
  ① **`Update()` runs unconditionally for every creature every frame** (`:56-64`) — `Vector3.Distance`
  + `Tick` + `TryTouch` per creature per frame with **no distance gating at all**. There is currently
  no AI LOD in the project.
  ② **Full-scene type scan per creature per frame**: `Update` calls `FindPlayer()` whenever
  `Player == null` (`:59`), and `FindPlayer` calls `FindObjectOfType<PlayerStunReceiver>()` (`:82`).
  If the rig isn't found — early frames after a `TravelCoordinator` load, or any world missing the
  receiver — **every creature runs a full-scene scan every frame, forever.** Real Quest hazard,
  independent of the ambient question. Also `Physics.SphereCastAll(..., ~0, ...)` (`:96`) is
  allocating and all-layers, with `GetComponentInParent` per hit.
- **⚠️ TERRY'S INSTINCT CORRECTED ON ONE POINT:** distance-tiering YES, **line-of-sight tiering NO**.
  A VR player turns their head in ~200 ms with no camera cut, and will stand still and STARE. Visibility
  may PROMOTE, never demote. Use `CullingGroup` distance bands and ignore its `isVisible` flag.
- **The reframe that saves the work:** the expensive thing was never "creatures doing something" — it
  was "creatures DECIDING what to do." Make ambient a pure function of `(seed, worldTime)` and a
  creature at 100 m is correctly mid-graze, at the right phase, in the right place, for the price of a
  sine wave. **We are already ~60% there** — `ForgeCreatureAnimator` (gait from `(body, Time.time,
  speed01)` + per-instance breath seed) and `WorldAmbientMotionRuntime` (`sin(time+seed)`) are exactly
  the right primitive; extend from pose to activity+position and add a tier manager above.
- **Card changes:** new **field 3b AMBIENT SILHOUETTE is PRE-MESH** (can the Dredge-Bull's plate
  physically reach the ground to graze? that's a rig constraint, not an animation one) · new Block D
  fields 24-32 (anchor/den, activity set, schedule, **trace props**, notice-but-not-alarmed +
  displaced, tier profile, ambient audio that outlives the visual tier, promotion safety, social beat)
  · clip budget **12 → 14**. ⚠ `witness_mite` needs a tier exemption — "moves only when unwatched"
  structurally collides with observation-based culling.
- **Prerequisite flagged:** `docs/07_PERF_BUDGET.md` is still entirely TBD. You cannot tier a budget
  you have not written down; proposed numbers are in the research (13.9 ms frame, ≤1.5 ms all creature
  AI, ≤2.0 ms animation+skinning).
- **Commit:** this one (card Block D + 3b + tier model + anti-pop-in law + this entry).

### 2026-07-28 (rb116) — T-Dog lane: 🎨 THREE BESTIARY SHEETS APPROVED + body contracts written (pre-Tripo)

- **Terry generated prompts 4-6 and all three came back build-ready:** **Husk-molter** (incl. the
  empty husk from 3 angles WITH a hollow-interior view — the separable prop we need), **Dredge-Bull**
  (the new Bruiser; charge wind-up and the exposed vent in the winded pose both read instantly),
  **Warden Sentinel** (chest-iris detail panels excellent; kneeling disabled pose).
- **⚠️ Sentinel deviated from spec: it came back HUMANOID**, not the specced four folding limbs.
  Recommendation recorded: **KEEP it** — a humanoid rig auto-rigs and retargets far more easily,
  which materially helps the Tripo pipeline. But logged a **kid-comfort flag ⚖**: a silent,
  faceless, 3 m humanoid approaching you in VR is much more intimidating than a bird-legged
  machine, and this is an E/E10 family title. Mitigations already in the design (it warns first,
  never enters arm's reach, unarmed, kneels when disabled) plus a **hard 1.5 m minimum approach**
  for this species and a child-tester review before it ships in a required encounter.
- **Wrote `docs/project_art_plan/BESTIARY_BODY_CONTRACTS.md`** — Block A/B/C passports for all
  three: metre scale + threat-height band + closest-approach-to-headset, full state vocabulary
  incl. the **CI-required COUNTER** and the weakened tell, socket maps (weak-point, grab handle
  placed away from the face, tether, VFX/audio mounts, carry handle), separable parts with cap
  geometry, collider proxies, and build-acceptance tests. Applied the VR timing rule: the
  Dredge-Bull's charge telegraph is **0.8 s** because sidestepping is a whole-body answer.
- **⚠️ DELIBERATELY NOT REGISTERED YET:** `concept_intake_gate.py` verifies every `conceptPaths`
  file exists on disk (`:214`). The three PNGs are still on Terry's phone, so adding manifest
  entries now would turn CI red. §0 of the new doc carries the exact 4-step registration recipe
  for when the images are committed at the computer. Gates verified green as-is.
- **Process correction for all operators:** Terry is on his PHONE by default and cannot open repo
  files. Deliverables he needs to act on (prompts, commands, lists) must be IN the chat message,
  not only committed. Docs are for the record; chat is the interface.
- **Commit:** this one (body contracts + this entry).

### 2026-07-26 (rb114) — T-Dog lane: 🐛 CREATURE ROSTER + PROMPT QUEUE (Terry's Tripo run-up)

- **Terry asked** for the concept-prompt framework, what creature art already exists, the full
  expected roster, and the first prompts to run in Gemini before his Tripo membership (~1 week).
  Wrote **`docs/project_art_plan/CREATURE_ROSTER_AND_PROMPT_QUEUE.md`** against the canonical
  `CONCEPT_ART_PROMPT_PLAYBOOK.md` (§3 template, §5.1 species archetype, §4 constants).
- **⚠️ HEADLINE FINDING — four SHIPPED creatures have NO concept art:** `witness_mite`,
  `light_grazer`, `husk_molter`, `tether_swarm`. Correction after checking `ForgeBodyLibrary`:
  the BODY pipeline is fine — six of seven have forged bodies defined (only 2 committed as
  assets; the rest are patcher-generated, so a disk listing under-reports them), and
  `tether_swarm` intentionally has none (cluster + cord, not one body). So these bodies were
  built BLIND with no reference — the exact condition the pipeline doc blames for the Warden
  taking five rounds. Art for them is the highest-value work, ahead of any new species.
- **⚠️ ID DRIFT to resolve BEFORE models are imported:** concept sheets say
  `creature_cistern_swarmer` / `creature_glass_tendril`; shipped assets say `swarm_bug` /
  `tendril`. Pick one ID set now — retrofitting post-import is the expensive version. ⚖ Terry/art
  lane.
- **Roster filed:** 5 approved sheets (canal stalker, cistern swarmer, glass tendril, warden drone,
  rogue-drone family + Wake-Guild spider, plus the pulser sheet) · the 12 novel behaviours from
  `CREATURE_DESIGN.md` with art status each · the 4 archetypes — **Bruiser is a genuine roster hole
  (no heavy exists at all)** · Warden classes beyond the drone, Architect constructs, and the three
  named bosses all unarted.
- **Locked laws restated for prompt consistency** (already blessed, now in one place): two glow
  languages (biological bioluminescence vs machine powered-light, never mixed) · machine-eye colour
  law (RILL amber / Warden white / hostile RED; player cyan vs hostile red-orange) · Warden kinship
  family · Wake-Guild anchor+cog · every hostile REQUIRES a disabled/powered-down panel (non-lethal
  canon).
- **Three full prompts written and paste-ready** (Witness-mite, Light-grazer, Tether-swarm), each
  demanding the state extremes the rig will need. Next in queue: Husk-molter (its shed husk is a
  separate prop), a Bruiser heavy, Fractal-splitter, Bridge-former.
- **The part that makes them game creatures:** every sheet must carry a **six-line behaviour card**
  (IDLE · NOTICE tell · ~0.4 s ATTACK telegraph · HIT reaction · STUNNED window · DOWNED pose) tied
  to the existing state machine and the `CreatureBehaviorReadabilityCatalog` requirements — because
  a sheet showing only a neutral pose produces a Tripo mesh that cannot be rigged for the states the
  game actually plays. Six-step Tripo-month workflow recorded.
- **Commit:** this one (roster/prompt doc + this entry). No code; art lane unclaimed.
### 2026-07-27 (gpt-first-route-feel-golden-ready) — first-route packet merged; exact Golden candidate authorized for Quest evidence

- **Did:** reconciled live `terry-local-wip` at docs-only head `b83185da` against tested vehicle source `bc8b73cb`, then completed full-file review of `gpt/first-route-feel-live-20260726`. Found and corrected one guaranteed stale test (`REWARD SECURED` after ownership wording changed), ObjectiveBoard subscription/disable/material lifecycle gaps, RepairableMachine procedural clip/runtime-material cleanup and interrupted-pulse restoration, plus source tests that could match comments. Opened bounded PR #90 with exactly four authorized files and squash-merged as executable source `cf94c60883c0f80aaeddacf08a65cd89a929e9f7`. Exact ordinary CI is GREEN run `30258567706` (EditMode + patch/world audit + contract reports; Android skipped). Exact Recovery PlayMode is GREEN run `30258567679`, 43/43 with 0 failed/skipped/inconclusive. Exact Recovery Golden Android is SUCCESS run `30258567678`; artifact `recovery-golden-apk-cf94c60883c0f80aaeddacf08a65cd89a929e9f7`, artifact ID `8650189616`, APK bytes `102563558`, independently recomputed APK SHA-256 `EB018B4EEA643D080443784E1039D0A96090517DD2EC302B0797A8AD0C16ED2F`. Build profile is `GoldenSlice`, define `ZIPTIDE_RECOVERY_GOLDEN`, locked scenes `_Boot`, `W000_DriftIn`, `ToxicCity`. Artifact audit has 0 blockers and 179 warnings. `_Boot` has 0 warnings; W000 has 8; ToxicCity has 15, including the existing report-only material warning at 62 unique materials versus cap 60.
- **Next:** Terry installs ONLY the exact artifact above using the rb109 uninstall-first law, verifies the APK hash before install, starts complete logcat before launch, then runs Boot/Home Hub → W000 locomotion/weapon scale+pose/holster laser/R3/coupler repair feedback/PUNCH IT → ToxicCity hazard/objective/vehicle/Y Return to Ship → repeat route → relaunch/Continue. Capture exact timestamps/screenshots for blockers, falls/spawn overlap, judder, material/warning growth and excessive combat pressure. M0 closes only from this Quest 3S evidence, never from automated green alone.
- **Heads-up:** first-route repair/objective presentation, recovery fixes and vehicle fixes remain DEVICE-YELLOW. No speculative fix was made for the prior single fall or `buriedAtTorso=True`; Quick Swap B remains unproven until a gun is deliberately socket-selected and `QUICK_SWAP` evidence is captured. Picasso ownership of `Visuals/**`, Forge/art/water/materials/shaders and FH-A01 remains intact; FH-S05 still waits for FH-A01 and FH-S08 remains last. The audit warning debt is real and must inform the later production pass, but it did not block this bounded M0 checkpoint. No executable source changed after `cf94c608`; later descendants are generated proof/CI/handoff only.
- **Commit:** PR #90 squash `cf94c60883c0f80aaeddacf08a65cd89a929e9f7`; CI verdict run `30258567706`; PlayMode observation run `30258567679`; Golden Android run `30258567678`; this queue-entry commit follows.
### 2026-07-27 (gpt-first-level-product-contract) — complete W000→space→W001→return route audited and unified

- **Did:** audited the current recovery route, `first-hour-v1` 22-beat contract, first-hour envelopes, `ShipCastOffRuntime`, `ShipFlightRuntime`, `SpaceTargetRuntime`, `ScenePatcherSpaceLane`, flight/ship/space-combat plans, W001 production order and T-Dog's first-two-worlds report. Added `docs/production/FIRST_LEVEL_PRODUCT_CONTRACT.md`, the canonical production target for the complete first playable level and W002 replication framework. It defines the full band: boot/home → W000 identity + coupler scan/repair → board/PUNCH IT → generated playable space leg → non-lethal disable/salvage → first Ziptide → approach/reentry → W001 arrival/job/creature/zipline/story/reward → extraction → changed ship/home/save payoff. Updated `FIRST_TWO_WORLDS_STATE_AND_ROUTE.md` so its city route is explicitly a subset of the full product contract.
- **Found:** ZIPTIDE does not yet have one coherent first-level implementation. The current machine contract omits the mandatory W000 coupler even though `PUNCH IT` is hard-gated by it; it jumps helm → first Ziptide → W001 while `CONTROLS_AND_FLIGHT` places a guided flight before the first Ziptide; live `ShipCastOffRuntime` still travels directly to ToxicCity; SpaceLane flight/combat/salvage exists as source/test code but `SpaceLane_Trial.unity` and its WorldPack are absent from the live branch; reentry/approach has no canonical owner or beat; W001 City Stage A, signature creature resolution and final orchestration remain unbuilt. This is a real contract/integration gap, not merely polish.
- **Next:** Terry runs the exact `cf94c608` Quest packet first. A systemic M0 blocker remains priority zero; bounded feel/content/presentation notes go to the ledger without freezing production. On a non-systemic verdict, begin Wave 1 from `FIRST_LEVEL_PRODUCT_CONTRACT.md`: migrate the 22-beat first-hour contract to v2, move first scan/repair teaching to the W000 coupler, add launch/space/flight/salvage/Ziptide/reentry/extraction signals, define safe persisted-beat migration, and prove every beat has a real producer or named blocked dependency. Then build the production route seam W000 → generated space leg → W001 through existing owners before broad city/content work stacks.
- **Heads-up:** do not treat `SpaceLane_Trial` as shipped content merely because its runtime and patcher exist. Do not add a second travel/flight/job/repair/reward/save owner. For the model band, ascent/reentry is an authored comfort-safe transition around `TravelCoordinator`, not a seamless planet-scale physics program. The first level must exercise the full route vocabulary once; W002 must reproduce it materially faster through data/recipes or the factory is not ready for W003.
- **Commit:** contract `0a013e7d2714bcc1d86b55656b3bce329fe18e94`; city-route binding `c8793eec9839f02a2a2ca7df2058739fc5a6da1c`; this queue entry follows.


### 2026-07-26 (rb113) — T-Dog lane: 🗺️ FIRST TWO WORLDS — state + route (answers Terry's "why is the city ugly / what's the concept→cityscape plan")

- **Terry asked** where level 1 and 2 actually stand across art, SFX, city layout, skyscape, space
  flight and the first hour, and what the plan is for turning his approved concept art into the
  real cityscape. Surveyed the repo and wrote
  **`docs/production/FIRST_TWO_WORLDS_STATE_AND_ROUTE.md`** (routing doc — replaces nothing).
- **The answer: the plan EXISTS and is approved; the gap is execution.** `CITY_VISUAL_SPEC.md` is
  🟢 REFERENCE APPROVED and defines ToxicCity precisely off Terry's own K1–K6 kit (concentric
  rings on a drowned tidal flat: Tower island → shanty wedges + canal ring → breached sea wall →
  harbour wedge/breakwater/moored Scrapper → flyable outskirts with wrecks, stilt villages, glowing
  tide pools → NE gate pillar ring). **None of that shape is built:** `CityBuilder.cs` still emits
  the older boulevard layout, and the handoff record states plainly that *"City Stage A remains a
  separate later bridge"* — both the bridge doc pass and the code pass are unstarted. That is the
  complete explanation for "doesn't make sense and is super ugly": a generic layout with no
  authored intent, nothing to do with the concept kit.
- **Also confirmed:** `CONCEPT_TO_BUILT_PIPELINE.md` already defines the 5-step route and step 1
  (measured visual spec) is a SOLVED skill — proven twice (ship + city). What has never run for the
  city is steps 3–5 (booth loop against the sheet → gates → device verdict).
- **State table filed** for city/concept-machinery/buildings/skyscape/SFX/first-hour/space/creatures
  across W001+W002. Thinnest lanes: **city Stage A (highest visible impact)** and **audio** (1
  track, 0 VO, no SFX library, no mix pass — master plan + 34-row queue ready to execute).
- **Route recorded (all behind M0 per POST_HEADSET_WORLD_FACTORY_ORDER §2):** bridge the city spec
  into a Stage A recipe (cheapest, ~1 session) → build Stage A **plus the booth reference-plate so
  built-vs-concept is one image** → K4 facade module recipes → W001 skyscape signature pass (K2) →
  audio rails THEN the paid SFX batch → close first-hour payoff beats (FH-A01→S05→S08) → W002 as
  the replication test. Paid 3D month still starts at the W002 gate, not before.
- **Named the recurring failure mode for the record:** specification outruns execution and the gap
  stays invisible until the headset. The fix is running the concept→built loop once end-to-end with
  the sheet pinned beside the render, then repeating it.
- **Commit:** this one (routing doc + this entry). No code, no lane claimed — city Stage A remains
  GPT's lane and this doc is reference for whoever fires it.

### 2026-07-26 (gpt-recovery-vehicle-first-route-handoff) — recovery + vehicle green; first-route feel branch pending

- **Did:** shipped the bounded Quest 3S recovery fixes through PR #87: Y field menu with Resume/canonical Return to Ship and post-travel XRI rebinding; Breaker Blade-only tracked-hand pose; holstered/socketed guns no longer show laser sights; centered-stick R3 crouch/slide guard with blended camera height; modest Home Hub distance retune retaining direct-hand liveness; toxic exposure aligned to the visible rendered surface; taser velocity cleared before kinematic lock. The initial test-only red was 1177/1178 passing and caused solely by matching `Time.timeScale` in an XML comment; corrected in `c0dc9c803c66030f794c23cc2bcdc0c5c401319b`. Exact ordinary CI is GREEN for `c0dc9c80`, run `30224832279`. Then shipped the isolated vehicle pass through PR #89: compact seat-side mount affordance, X dismount, Y menu remains available while mounted, hoverbike eye-clearance channel, split low controls, smooth deadzoned steering, and rig world-yaw follow while preserving local HMD freedom. Exact ordinary CI is GREEN for `bc8b73cb741a9c8e10eb7a0949a229226b54f95f`, run `30225458255`; durable verdict commit is `9a4a5dbad3e996954df2422cf49989896e1f46f8`.
- **Next:** finish the isolated branch `gpt/first-route-feel-live-20260726` at head `78fe6212076827f0e12817868acacc7630b41743` before further source work. It is five commits ahead of live and changes only `Gameplay/Runtime/Jobs/ObjectiveBoard.cs`, `Gameplay/Runtime/Story/RepairableMachine.cs`, `Tests/EditMode/FirstRouteFeelTests.cs`, and its `.meta`. It adds exact-hand bounded repair haptics/local procedural stage feedback plus clearer objective/contract presentation while keeping reward/save/quest state with existing owners. No PR has been opened and it has not run authoritative CI. Review complete files/diff for Unity lifecycle, duplicate feedback, controller-null fallback, material/audio cleanup and test truth; open one bounded PR against `terry-local-wip`; merge only when review-clean; then wait for exact Unity EditMode + patch/audit GREEN. After final source is green, obtain an exact-source Recovery Golden Android APK and prepare Terry's next-day Quest route and evidence packet.
- **Heads-up:** all new recovery/vehicle behavior remains DEVICE-YELLOW until Terry tests on Quest 3S. Do not claim M0 closed from CI. Retest Boot/New Game → W000 locomotion/coupler/PUNCH IT → ToxicCity → Y Return to Ship → repeat route; verify Breaker Blade pose, gun laser hidden while holstered, R3 no longer combines turn/drop/slide, Home Hub distance, visible toxic contact boundary, no kinematic warning spam, vehicle eye clearance/mount/X dismount/smooth steering/yaw follow, and post-return input. The single prior fall/spawn overlap was not guessed at; investigate only if reproduced with logs. Excess combat pressure, 1%-low spikes/material growth and later Quick Swap B proof remain open. Respect Picasso ownership of `Visuals/**`, Forge/art authors/water and FH-A01; FH-S05 waits for FH-A01 and FH-S08 remains last. Ordinary CI normally skips Android, so do not hand Terry an APK without exact source SHA, workflow run and artifact SHA. Keep the longer production order in `docs/production/POST_HEADSET_WORLD_FACTORY_ORDER.md`: device proof → W000/W001 model band → W002 replication → automation ratchet → W003-W005 mini-batch → chapter batches; paid 3D/audio months and reusable framework extraction only after their named contracts are proven.
- **Commit:** recovery functionality squash `134aefa6dcf1b5f4cbf275889f1a2991a93a629a` + test correction `c0dc9c803c66030f794c23cc2bcdc0c5c401319b`; vehicle squash `bc8b73cb741a9c8e10eb7a0949a229226b54f95f`; pending feel branch head `78fe6212076827f0e12817868acacc7630b41743`; this queue-entry commit follows.


### 2026-07-26 (gpt-m0-device-verdict-54e23022) — Quest evidence closes major recovery questions; M0 remains open

- **Did:** inspected Terry's complete evidence bundle `ziptide_headset_evidence_20260726_143829.zip` (SHA-256 `fca46c99bd2fafdc46ca6a49b65b5cdf00753dcd2e91cda450705b8807dc27e7`) from Quest 3S against exact Recovery Golden Android run `30213241501`, source `54e230229e8bbe57af69ccd0ec6ce77a9c8bc501`, APK SHA-256 `0141A7858C358F3EDD317603A55C48004B976BF39E442CAD3455D48600759B8F`. Added `docs/recovery/M0_DEVICE_VERDICT_54E23022_20260726.md`. Device evidence now positively proves Home Hub liveness, two active production rays at boot/W000/ToxicCity, New Game, W000 locomotion, complete coupler repair, ship board/PUNCH IT, ToxicCity travel, and post-travel input recovery. Source-correlated failures: Home Hub tile row at 0.45 m; Breaker Blade default/override pose points forward; `GunLaserSight` mistakes socket selection for hand-held selection; R3 crouch/slide + 0.55 m camera drop conflicts with smooth-turn stick use; vehicle rider follows seat position but not vehicle yaw; procedural fork/control bars and giant RIDE/STEP OFF cubes obstruct the cockpit; ToxicRiver uses large AABB bounds rather than rendered-liquid contact; one real fall and `buriedAtTorso=True` spawn probes; repeated kinematic-velocity warnings; excessive combat pressure; four performance 1%-low spikes and material growth requiring report-only follow-up.
- **Next:** recovery owner should assign one narrow candidate: deterministic in-headset pause/return path; Breaker Blade-only pose correction; laser hidden for holster/socket selection; R3 comfort/control resolution; modest Home Hub distance retune preserving live anchor/direct-hand fallback; toxic visual/consequence boundary alignment; investigate the fall/spawn overlap; fix touched kinematic warning order. Keep vehicle work in a separate mini-sprint: measured rider eye-clearance envelope, hierarchy-identified obstruction removal/rebuild, contextual VR mount/dismount, one vehicle-yaw owner with rig yaw following while preserving local head freedom, deliberate smooth/snap steering choice, then device proof. Later target B with a gun deliberately socket-selected and require `QUICK_SWAP` evidence; this run did not prove B broken.
- **Heads-up:** M0 remains open because melee orientation failed and the second full route could not be repeated due to no world escape/menu path. Boot, coupler, travel, and post-travel input now have strong positive device evidence, but freeze-lift scope belongs to the recovery owner. No code, scene, prefab, test implementation, workflow, asset, build, or APK changed. The ZIP and screenshots are hashed in the report but still need durable binary archival if required beyond conversation/file-library retention. Do not mix ship art polish into the recovery candidate unless geometry blocks play.
- **Commit:** `2b211631cf188a5de633ae392ab08ddf15ea84f9` (device verdict report); this queue entry commit follows.


