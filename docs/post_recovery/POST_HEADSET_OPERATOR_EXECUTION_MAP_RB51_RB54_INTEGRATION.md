# POST-HEADSET OPERATOR MAP — RB51–RB54 INTEGRATION

**Status:** PLANNING ONLY. Mandatory companion to `POST_HEADSET_OPERATOR_EXECUTION_MAP.md` until Q0 folds these decisions into the main map. This is not a second execution authority and authorizes no runtime, scene, asset, package, workflow, audio, travel, save, locomotion, story, or certified-checkpoint change before Terry records the exact Quest verdict.

**Inputs reviewed:**

- `docs/design/FIRST_HOUR_MUSIC_DIRECTION.md`
- `docs/design/WORLD_ASSEMBLY_READINESS.md`
- HANDOFF rb51–rb54
- `docs/design/ADAPTIVE_AUDIO.md`
- `docs/design/SFX_FORGE.md`
- `docs/design/CONTRACT_LEDGER_WAYFINDING.md`
- `docs/first_hour/FIRST_HOUR_DIRECTORS_CUT.md`
- `docs/first_hour/FIRST_HOUR_V2_1_RECONCILIATION.md`
- `docs/post_recovery/POST_HEADSET_OPERATOR_EXECUTION_MAP.md`
- `docs/design/SHIP_NAVIGATION_MAP_TERMINAL.md`

The certified checkpoint remains source `2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295`, run `29540554179`, APK SHA-256 `9bfe13ac0acda6718c3ae1664919cc5609385e50691b8676219c552d8e555a10`.

---

## 1. Verdict

Both additions close real gaps and should be retained.

- The music plan establishes a recognizable score genome, a controlled kinship system, a first-hour cue map, and a licensing/provenance discipline.
- The assembly-readiness audit correctly identifies that world art frameworks do not yet assemble world purpose, story wiring, authored moments, lines, lore, and a repeatable production runbook.

They do **not** alter today's headset gate or move world-scale automation ahead of the first complete hour. The first hour remains the production reference that future world assembly must learn from.

---

## 2. First-hour music — accepted direction with binding corrections

### 2.1 Creative direction accepted

The two-strand score model is accepted as planning direction:

- **Intimate strand:** ground-level, used-future, wilderness pressure, low and spacious.
- **Soar strand:** flight, vistas, first Ziptide, chapter peaks, wide and elevated.
- **Kinship dial:** K3 full identity, K2 close relative, K1 trace, K0 own voice.
- **Tide motif:** rare and earned, not pasted loudly over every world.

The separation clause is also accepted: foreground melody, midground orchestral body, background drone/texture. It is compatible with the later stem architecture and with ZIPTIDE's visual depth grammar.

### 2.2 Licensing and provenance law

No generated music ships merely because the prompt or take is emotionally successful.

For every candidate intended for a build:

1. generation must occur under terms that explicitly permit the intended commercial game use at the time of creation;
2. live terms must be rechecked on the generation/import date;
3. retain tool/plan, account receipt, generation date, track URL or stable id, exact prompt, exported source file and content hash;
4. add the asset to `music/PROMPTS_LOG.md` and `CREDITS.md` at import time;
5. free/noncommercial reference takes stay outside shipping asset paths;
6. a later cover/remaster/extend operation does not cure uncertain rights unless written terms or support confirmation explicitly say it does.

The signature motif should eventually receive meaningful human composition, arrangement or performance if Terry wants a protectable brand asset. That is a future option, not a first-hour blocker.

### 2.3 One music ownership tree

Music and SFX remain different content classes under one audio ownership tree.

- World beds use existing `AudioProfile`/`AudioDirector` paths.
- Event cues and stingers request a music cue through one `AudioDirector`-owned seam.
- `SfxPlayer` does not become the music-stinger owner.
- Gameplay systems publish semantic moments; they do not each create their own AudioSource, clip queue or mix logic.
- Adaptive stems remain later. Single-file cues are acceptable first-hour source material and may become the base stem later.

Required future semantic cue requests may include:

```text
MUSIC_TITLE_READY
MUSIC_FIRST_FLIGHT_BEGIN
MUSIC_RELAY_OR_ARTIFACT_RESONANCE
MUSIC_FIRST_ZIPTIDE_BEGIN
MUSIC_W002_DEFENSE_BEGIN
MUSIC_RETURN_HOME_CHANGED
```

Names are illustrative until audio archaeology selects the surviving API. No second global audio manager is authorized.

### 2.4 Correct the Director's Cut cue placement

Two current cue labels conflict with the versioned first-hour route.

1. `flight_punchit` is the **normal first Space Lane flight**, not a direct W000→W001 Ziptide trip. Its content binding belongs to Q6.
2. `w001_gate_wake` cannot imply that Toxic City's relay is the first true Ziptide activation. The first true Ziptide occurs only after both artifact halves join, the beacon leads home, and the key seats in the repaired coupler. Cue 5 should be renamed or rehomed as relay/artifact resonance, signer realization, or key/coupler wake. Cue 6 remains the actual first-Ziptide peak in Q10.

The artifact join/beacon sequence may deliberately use restrained ambience and SFX rather than adding a tenth full music cue. Silence is still authored.

### 2.5 Correct implementation placement

- **Q0:** create `CREDITS.md`/prompt-log rules before importing music.
- **Q2/Q3:** `w000_wake` and communication-safe low bed, only after the audio owner is confirmed.
- **Q4:** music/SFX mix contract tested against the hero Taser and RILL captions.
- **Q6:** normal first-flight cue.
- **Q8:** Toxic City bed.
- **Q9:** restrained artifact/key resonance cue or ambience transition.
- **Q10:** first-Ziptide cue plus photosensitivity/caption/mix proof.
- **Q11:** W002 bed and defense cue.
- **Q12:** changed-ship homecoming cue.
- **Q13:** title cue, final loops, loudness, ducking, thermal/voice-budget run and full-hour mix verdict.

TM-1 may use a licensed placeholder before the final identity track. A title-theme asset never blocks menu interactivity.

### 2.6 Music acceptance

A cue is not accepted from headphones alone. It must pass in the actual Quest mix with dialogue, SFX, ambience and player action.

For each cue class, record:

- correct semantic trigger and no duplicate trigger;
- loop seam or clean start/end;
- loudness and ducking at clamp-edge settings;
- caption and RILL intelligibility;
- no fatigue over the intended duration;
- no unintended horror tone in the family-facing first hour;
- performance/voice-budget behavior;
- licensing/provenance evidence complete.

---

## 3. Assembly-readiness gaps — corrected contracts

### GAP A — Ride-scenes

**Accepted as a presentation grammar, not a movement system.**

A ride-scene may stage authored events around a conveyance the player deliberately boards. The existing ship, lift, zipline, barge, vehicle or gate owner remains responsible for movement and comfort.

Binding rails:

- no generic ride runtime moves the player or rig;
- each conveyance owner explicitly opts in and defines boarding, interruption, resume, speed and exit behavior;
- head sovereignty is absolute;
- hands may remain live only where the conveyance and encounter are designed for it; shooting is not a universal requirement;
- skip does not automatically mean speeding the vehicle. The owner may shorten staging, advance at a comfort-safe clamp, or offer a diegetic continue control;
- one-per-world is a useful rarity guideline, not a reason to force a ride into every world;
- the first normal flight in Q6 is the initial reference ride-scene. A later barge/lift pilot may prove reuse.

This is a story/staging descriptor over claimed traversal and ship owners. It is not an independent GPT lane that may modify locomotion.

### GAP B — World gameplay genome

**Accepted as authoring/validation data, not a procedural mission runtime.**

The genome may describe a world's intended content shape:

- contract/job template;
- ordered semantic steps;
- encounter slots;
- reward reference;
- mystery/lore slots;
- optional ride-scene slot;
- completion and return-state declarations.

It must compile/adapt into canonical owners:

- `JobDirector` / `JobRuntime` for jobs;
- existing encounter/enemy owners for combat;
- `RewardRouter` for rewards;
- canonical story flags and world data;
- existing travel and save owners.

It may not:

- become a second first-hour contract engine;
- replace the versioned first-hour JSON;
- create a second encounter director;
- treat the RILL Proving Ground as a campaign job template;
- generate arbitrary runtime steps from prose;
- mass-author eighty worlds before a pilot passes.

The first hour is the hand-authored reference. The genome should be piloted on one later, ordinary world after the first hour is complete, then on a second sibling world before scale is claimed.

### GAP C — Lore Forge

**Accepted as a registry, slot contract and continuity audit. It does not generate final prose.**

Each lore slot needs:

- stable id;
- world/chapter/story-state ownership;
- spoiler tier;
- type: mystery object, wreck log, readable, environmental clue;
- chain predecessor/successor where applicable;
- explicit present/waived state;
- localization-ready text owner;
- Terry/story review state.

Not every world must contain every lore type. Explicit waivers are valid and preferable to repetitive filler. CI validates declared slots and chain integrity; humans validate writing quality, placement and emotional effect.

### GAP D — Flag-graph validator

**High value and feasible only in declared layers. It cannot soundly infer every arbitrary C# string mutation.**

Build it as a canonical declarative graph:

1. inventory stable flag ids and classify them;
2. register grant/consume/require edges from inspectable author data, world gating, first-hour contracts, line triggers, ending calculations and explicit runtime manifests;
3. source-scan for unregistered literal uses as a leak detector, not as the sole graph;
4. validate reachability and contradiction over the declared graph;
5. begin WARN/report-only and ratchet after the inventory is complete and low-noise.

Required flag classes include:

- progression milestone;
- unlock/gate input;
- contract/job state;
- once-latch/presentation acknowledgement;
- ending input;
- telemetry/diagnostic-only;
- terminal historical state.

Therefore, “every granted flag must be consumed” is too strict without classification. Terminal milestones, historical facts and presentation latches may legitimately have no downstream consumer. The validator should instead detect:

- required/consumed but never grantable;
- grant edge with unknown id;
- unreachable gated node;
- impossible prerequisite cycle;
- mutually exclusive ending requirements that can be simultaneously forced or never reached;
- duplicate semantic flags claiming the same truth;
- unregistered dynamic/literal flag use;
- declared terminal/telemetry exemptions without a valid classification.

Existing first-hour contract validation, `WorldGating` checks, line-trigger data, ending math and the future ledger model partially cover local slices. None currently replaces the global graph.

**Placement:** D0 inventory/schema can begin beside Q1 after recovery exit. It must not create more than one additional invisible foundation PR before visible Q2 work. Broad blocker promotion waits until after the first-hour graph and one standard-world pilot pass.

### GAP E — Per-world voice formula

**Accepted as a line-kit authoring workflow, not a quota of four spoken lines per world.**

A world kit may contain:

- RILL enter line;
- Cal response;
- gate/departure line;
- optional FollowUp;
- explicit silence/waiver entries.

Every entry carries stable text id, speaker, RILL state, chapter, spoiler tier, trigger, once/repeat policy, caption chunking result and dialogue-priority class. Drafting may occur in batches, but Terry/story review remains mandatory. One dialogue arbitration path schedules the result.

The first-hour Q3 writing/caption pass is the exemplar. Later kits ride the same localization, caption and anti-annoyance laws. Some worlds should be intentionally quiet.

### GAP F — Assembly-line runbook

**Do not wait until the very end to write any runbook. Split it into two stages.**

- **F0 skeleton:** exists early as a checklist linking the operator-map packet template to world content, art, flags, music, SFX, lines, save, performance and Quest proof. Exercise it during the first hour.
- **F1 production runbook:** finalize only after one ordinary post-first-hour world and one sibling world have traveled from data row to device PASS. Replace assumptions with measured steps and timings.

The runbook is an orchestration document, not another generator or runtime owner.

---

## 4. Revised priority

The handoff priority `D → B → A+E → C → F` is useful but incomplete because it omits the current recovery and first-hour authority.

Binding project order:

1. **Certified Quest checkpoint.**
2. **Q0/Q1:** recovery exit, governance, versioned first-hour contract; D0 flag inventory/schema may join Q1 as one bounded warn-only packet.
3. **Q2–Q13:** complete and perfect the first hour. Music, voice and the first flight are built vertically inside their existing Q packets.
4. **F0:** maintain the world-assembly checklist while the first hour is built.
5. **B0 pilot:** apply the content-genome schema to one ordinary post-first-hour world without generating a second runtime architecture.
6. **E pilot:** produce that world's reviewed line kit through the accepted dialogue path.
7. **A pilot:** add one ride-scene only where the world benefits and an existing conveyance owner can prove it safely.
8. **C0:** establish lore registry/chain audit before the project grows beyond the first small cluster.
9. **D ratchet:** promote proven graph checks as content volume grows.
10. **F1:** finalize the assembly-line runbook from two device-proven world builds.
11. Only then claim that standard worlds can be assembled repeatably.

This sequencing prevents “automation” from encoding unproven first-hour mistakes across eighty destinations.

---

## 5. Meaning of “the game builds itself”

The target is not autonomous unsupervised game creation. The target is a controlled production system where an operator or LLM can:

1. choose an approved world row and content shape;
2. populate declared schemas and slots;
3. compile through canonical authors;
4. receive deterministic validation and clear missing-data failures;
5. produce review artifacts;
6. submit a bounded Quest campaign;
7. preserve human authority over story canon, comfort, visual taste, music selection, writing quality and final fun.

A generated world is not complete because files exist or CI is green. It is complete when the world-specific acceptance contract and device verdict pass.

---

## 6. Immediate status

The rb51–rb54 changes are planning/HANDOFF/generated-verdict changes only. They do not alter the certified APK.

The latest inspected ordinary CI source run for `785ab8ba8e95fb3c4bcf7052a99eeca6e5b97e39` executed 1,049 EditMode tests: 1,048 passed and the sole failure remained `GateGap5_NoBoardClaim_RotsSilently` against the stale 2026-07-03 `SPRINT.md` active claim. The music and assembly documents introduced no executable regression. Q0 still owns the truthful board-state correction; the governance test must not be weakened.

Until the headset verdict and Q0, these documents are accepted planning inputs, not implementation commands.