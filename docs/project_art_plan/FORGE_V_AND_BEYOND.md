# FORGE V AND BEYOND — THE HORIZON PROGRAM

**Status:** 🔵 PLANNED (research + direction only — Terry commissioned 2026-07-17)
**Authorization:** NONE. This document authorizes zero code. Every generation below is hard-gated
(§1). It exists so the Forge program has a destination, the same way `FORGE_IV_CINEMATIC_PRESENCE.md`
existed before its CP-0 constitution was written.
**Author context:** written during the recovery freeze, after the Quest Golden Checkpoint was
authorized (rb38) but before Terry's device pass. Nothing here weakens the recovery exit rules.

> The Forge ladder so far: **I** made assets from prompts. **II** made them AAA. **III** made them
> agree with each other. **Creature V2** made them alive. **IV** (planned) makes standing among them
> feel like a film. What's left is everything a *finished game* needs that an *art pipeline* doesn't
> yet provide: worlds that stage a story, eighty worlds instead of eight, and Terry directing all of
> it from inside the headset. That is Forge V, VI, and VII.

---

## §0 — How to read this document

- This is the **horizon layer**: direction, sequencing, entry gates, and the rails each generation
  inherits. It is deliberately one level less specific than an implementation plan. Each generation
  gets its own CP-0-style constitution **before** its first envelope, exactly as FORGE IV did.
- **Nothing here overrides** `FORGE_III_PLAN.md`, `CREATURE_QUALITY_V2_LIFE_LEAP.md`,
  `FORGE_IV_CINEMATIC_PRESENCE.md`, `CINEMATIC_PRESENCE_CONSTITUTION.md`, the class budgets, the
  72 Hz floor, or the working laws in `README.md`. If this doc and a binding plan disagree, the
  binding plan wins and this doc gets amended.
- The house laws apply to the planning itself: one envelope at a time, CI green before stacking,
  photo/contact-sheet verdicts, headset verdicts before promotion, no scene/prefab YAML edits, no
  budget raises without device evidence, no parallel toolchains (`ASSET_FORGE_MAP.md` rejections
  stay rejected).

## §1 — The gate ladder (why nothing here starts today)

Each rung must be **honestly complete** before the next opens. This is the same ordering law that
kept Forge II from shipping untextured meshes and Forge III from grading ungraded worlds.

1. **Recovery exits.** Terry's Quest Golden Checkpoint passes; the recovery exit report is written;
   the source SHA is tagged. Until then, *no* art runtime work of any generation.
2. **Post-recovery building resumes under the proof ladder** (`docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md`),
   starting from the vertical slice in `docs/PROJECT_COMPLETION_ROADMAP.md`.
3. **FORGE III closes:** F3.5 runtime + wiring, F3.6 reactive world, F3.7 signage, the F3.9 W002
   conformance lock, and the outstanding device verdicts (grade cost, hero-light frame cost, water
   y-level, street variation).
4. **FORGE IV executes CP-1 → CP-11** per its own plan, ending in the two-world proof
   (W002 technical / W001 emotional).
5. **Then Forge V opens.** Forge VI planning may begin during late FORGE IV (it is mostly pipeline,
   not scene work), but its envelopes wait for the FORGE IV archetype ratchet (CP-11) because the
   World Compiler mass-produces exactly those proven archetype recipes.
6. **Forge VII opens last** — it needs V's staging vocabulary and VI's world-production line to
   have something worth directing live.

`docs/design/FACTORY_TOPTIER_PLAN.md` keeps its own §0 gate and is **not** part of this ladder;
it re-enters via the roadmap when Terry says so.

## §2 — Research summary: the debts this program inherits

These are already-recorded deferrals scattered across the plan docs. Each future generation below
names which debts it retires, so nothing silently evaporates. (Sources: `SPRINT_ART.md` board,
`FORGE_III_PLAN.md`, `ASSET_FORGE_MAP.md`, `design/ART_REGISTRY.md` §5,
`ZIPTIDE_MASTER_BUILD_PLAN.md` §7, `PROMPT_TO_WORLD_WORKFLOW.md`, `systems/SKYSCAPE_DESIGN.md` §4.)

| Debt | Recorded where | Retires in |
|---|---|---|
| Hand-written Quest water shader (true fresnel depth-tint, two-layer cross-scroll) | F3.3 deviation note | Forge V, LS-6 (weather/tide staging needs real water states) |
| GroundingBuilder polish (prop stains, wall-base moss, player-rig shadow) | F3.4 commit 3 deferral | FORGE IV CP-3/CP-6 or Forge VI world pass — whichever touches those surfaces first |
| VfxFactory runtime + weapon-impact wiring (cross-lane with combat) | F3.5 commits 2–3 | FORGE IV CP-6 |
| Creature P4 remainder: look-at, stun-droop, booth pose shot | board P4 row | Creature V2 program |
| Doorway/CornerTrim/Roof building modules (both-sides law) | E5.1 note | Forge VI, WC-2 (kit completion is a scale prerequisite) |
| External generation backends (Tripo/marketplace at scale), glTFast import | ART_REGISTRY §5 triggers | Forge VI, WC-5 — the trigger conditions in §5 remain the law |
| Addressables / texture streaming on RAM pressure or >20 worlds | ART_REGISTRY §5 | Forge VI, WC-6 |
| WorldStubGenerator + 80-world content pipeline | master plan §7 | Forge VI, WC-1 (superseded by the archetype-recipe form) |
| Prompt-to-World ArtBuildPlan pipeline phasing | PROMPT_TO_WORLD_WORKFLOW.md | Forge VI (WC-3) and Forge VII (DC-1) split it: batch vs. live |
| Skyscape authoring tiers for 68 unbuilt worlds | SKYSCAPE_DESIGN §4 | Forge VI, WC-4 |
| Diegetic UI full rollout | FORGE IV CP-9 (window-gated) | stays CP-9; Forge VII DC-4 extends it to the director tools themselves |

---

# FORGE V — THE LIVING STAGE
### World dramaturgy: the game stops being a beautiful place and starts being a told story

**North star:** a player who never reads a menu can tell you what happened in this world last
chapter — because the world itself changed, staged it, and remembers.

FORGE IV makes a world that *feels* real at one moment in time. Forge V makes it real **across
time and story**: worlds that are different when you return, events that are staged like scenes
instead of triggered like switches, a companion who exists cinematically rather than as a floating
audio source, and ambient life that implies a society. This is where the art program finally meets
`docs/storyboard/STORY_BIBLE.md` and the RILL arc (`ZIPTIDE_MASTER_BUILD_PLAN.md` §5) — the
biggest untouched seam between "art" and "game."

**Why it must exist:** every system through FORGE IV renders a *state*. The story bible describes
*change* — the Bloom spreads, chapters scar worlds, RILL grows. Without a staging generation, story
beats will be implemented as popup text over a static set, and the entire cinematic-presence
investment gets narrated instead of shown.

### The five envelopes (LS = Living Stage)

- **LS-1 — World-State Skins.** Every signature world gets N authored *states* (pre-Bloom /
  blooming / scarred / reclaimed …) expressed through the systems that already exist: theme +
  vista variant, grade variant, practical set, dressing deltas, material mask weights (CP-3),
  audio identity variant (CP-8). A state is **data on the existing profiles** — a
  `WorldStateVariant` on `VisualThemeProfile`-adjacent data, resolved by story flags through the
  existing flag system — never a second copy of the scene. Rail: a state switch may not touch
  geometry ownership; it re-parameterizes what FORGE I–IV built.
- **LS-2 — The Staging Vocabulary.** A closed, testable event grammar (the `VfxLibrary` pattern):
  `reveal`, `arrival`, `collapse`, `emergence`, `procession`, `signal`, `departure`, `aftermath`.
  Each staged event declares actors, path/marks, Awe-Node linkage (CP-5), sound/music cue (CP-8),
  duration bounds, interrupt/skip behavior, and a *player-freedom contract* — staging NEVER takes
  the head or the legs. It composes existing pieces; it does not own creatures or travel.
  Gameplay/story owners keep authority; art owns how the moment *reads*.
- **LS-3 — RILL Cinematic Presence.** The companion's staging layer: approach/orbit/perch
  blocking relative to the player and the current Awe Node, look-at with the Creature-V2 motor,
  emissive/voice state coupling, arc-stage visual growth (per the 12 canon beats). Cross-lane:
  `RILLCompanion` runtime is a gameplay owner; Forge V supplies its *visual dramaturgy* only.
- **LS-4 — Ambient Society.** Distant life that implies a world beyond the play bubble, built
  strictly in P3/P4 bands: silhouette traffic, procession lines on far routes, light-crawl on
  distant structures, flock/swarm impostors with ≤2 update rates. Hard rail inherited from CP-7:
  no full-rate hidden AI; ambience is *rendered belief*, not simulation.
- **LS-5 — Weather & Tide Acts.** Weather as authored *acts* (not a random slider): each world
  declares 2–3 sky/precipitation/tide states wired through vista + fog derivation (F3.1's
  derivation law extends: weather derives FROM the vista state, never fights it). The namesake
  tide gets its act here — tide level as a world-state input to `ZiptideWater` bodies. This is the
  envelope that finally funds the hand-written water shader **if** device evidence approves it.
- **LS-6 — Return & Memory.** The player's mark on the world: unlocked routes stay open, repaired
  machines stay lit (visibly — practicals + signage state), the return-home music remembers
  (CP-8's return stem), photo-ledger moments. Uses save/story flags; art renders them.

**Proof standard:** a staged event or state change ships with a *before/during/after* contact
sheet triple plus an in-headset "did you feel it or read it?" verdict from Terry. New CI shape:
staging grammar validation tests (closed enum, budget clamps, freedom-contract assertions) — the
`VfxRecipeDefinition.Validate()` pattern applied to drama.

**Do not:** cutscenes that seize the camera; a second event system competing with story flags;
per-event bespoke code (the grammar is closed — extend it by decision, not by exception); weather
that breaks the derivation law; ambient life that costs real AI.

---

# FORGE VI — THE WORLD COMPILER
### Scale: eighty worlds that each feel authored, from a pipeline that doesn't need eighty months

**North star:** a new world goes from "story bible row" to "walkable, conformant, identity-bearing,
budget-clean world" in one operator session — and no player can tell which worlds were compiled.

The master plan promises ~80 worlds. FORGE I–V perfect a *per-world* craft; Forge VI industrializes
it **without lowering the bar** — the ratchet (F3.9, CP-11) is what makes mass production safe: a
compiled world must pass every gate a hand-built world passes, automatically, before a human ever
looks at it.

**Why it must exist:** without it, worlds 10–80 get built by whoever is cheapest at the time, at
whatever quality that session produces — exactly the unevenness the EXCELLENCE_MAP exists to
prevent. The compiler is the only honest answer to "how does quality stay EVEN across 80 worlds
and multiple model generations."

### The seven envelopes (WC = World Compiler)

- **WC-1 — The World Recipe.** One ScriptableObject-family genome per world (the
  `ForgeRecipeDefinition` idea, one level up): archetype (CP-11's six), story-bible refs, hazard,
  layout seed, vista + light script + grade inputs, palette family, creature roster, module kits,
  practical density, water bodies, awe-node slots, audio identity, world-state variants (LS-1).
  Everything downstream derives. This supersedes the master plan's `WorldStubGenerator` sketch in
  the current architecture's idiom: data + `Validate()` + audit blockers, no parallel toolchain.
- **WC-2 — Kit Completion.** The module vocabulary each archetype needs to compose full worlds:
  doorway/corner/roof modules (the E5.1 debt), street/canal/interior sets, per-archetype flora and
  prop families. Bounded: an archetype's kit is *done* when its pilot world composes without
  one-off geometry — not when someone runs out of ideas.
- **WC-3 — The Compile Pass.** One deterministic, create-only, idempotent pipeline run:
  recipe → layout → dressing → practicals → water → grounding → vista/light/grade → signage →
  audio identity → conformance audit → contact sheets. It is the existing `WorldDressingBuilder` /
  author chain, unified behind one entry point with one manifest — the batch half of
  `PROMPT_TO_WORLD_WORKFLOW.md`. Rerunning a compile on an untouched recipe is a no-op (hash-stable,
  the `lockedContentHash` law at world scale).
- **WC-4 — The Identity Guarantee.** The anti-samey gate, and the soul of this generation. Codified
  distinctness: per-archetype *identity budgets* (each world must differ from every sibling in ≥N
  of: silhouette skyline, palette center, vista composition, creature roster, practical rhythm,
  audio tonal center — with tested distance metrics on the recipe data, the SkyVistaLibrary
  progression-test idea generalized). Plus one **hand-authored hero element per world** — the
  compiler builds the body; a human (or Terry-verdicted session) gives it the face. Skyscape tiers
  (SKYSCAPE_DESIGN §4) slot in here: signature worlds get Tier-1 skies, compiled worlds get
  tier-appropriate derivations that still pass the Prospect rubric.
- **WC-5 — External Backend On-Ramp.** When ART_REGISTRY §5's triggers fire (hero assets from
  outside, marketplace/Tripo at scale), the import path joins *here*, behind the same gates:
  imported meshes get budget validation, material conformance, provenance records, photo verdicts.
  IDs never change when backends do — the registry law, unchanged.
- **WC-6 — Memory & Streaming.** Addressables/texture-streaming per ART_REGISTRY §5, triggered by
  RAM pressure or world count >20 — with the CP-7 fidelity director as the runtime consumer.
  Budget: load-time and memory ceilings become audited numbers per world, like tris are today.
- **WC-7 — The Review Farm.** CI renders every compiled world's CP-1 contact-sheet set; a model
  session reviews *batches* against the rubrics and produces machine-readable verdicts; only
  rubric-passing worlds reach Terry's headset queue. The Forge II photo loop, at fleet scale.
  Rail: a model verdict can **fail** a world but never **promote** one — promotion to
  locked/shippable stays a headset event.

**Proof standard:** the pilot is compiling **one already-proven archetype** into 2–3 new sibling
worlds and putting them through WC-7 + a Terry pass. The generation is proven when a compiled
sibling is indistinguishable in quality (not in identity!) from its hand-built pilot.

**Do not:** generate 80 stubs before one compiled world is proven; let compile-pass convenience
create a second scene-authoring path outside the author/patcher idiom; ship any world that never
had a human headset verdict; treat identity metrics as a substitute for the hero element; raise
any budget "because there are more worlds now."

---

# FORGE VII — THE DIRECTOR'S CHAIR
### Terry's north star, made literal: art direction from inside the headset

**North star (CLAUDE.md, verbatim):** *"Terry puts on the headset, says 'move that building / make
this do that,' and it just happens with ~zero errors."*

Everything in Forge I–VI runs through a PC session and a rebuild. Forge VII closes the final loop:
the director stands **inside** the world and directs it. This is deliberately last — live direction
is only safe when every edit routes through systems that already validate, audit, and refuse
(I–VI), and it is only *useful* when there is a staged, compiled game to direct (V–VI).

### The five envelopes (DC = Director's Chair)

- **DC-1 — The Command Seam.** In-headset (and desktop-companion) direction commands that compile
  to **recipe/author edits, never scene YAML**: "move that building" = a layout/dressing data edit
  + re-run of the relevant author; "make this lantern red" = a practical palette override in the
  world recipe. Every command produces a diff preview, applies create-only/idempotent, and lands as
  a normal commit through the normal gates. The live half of `PROMPT_TO_WORLD_WORKFLOW.md`.
  **Hard rail:** the command seam has *exactly* the powers the editor authors have — zero new
  mutation paths. If an author can't do it, the chair can't either (that's a new author envelope).
- **DC-2 — Live Look Tuning.** The parameters that are safe to touch at runtime (grade within
  F3.2's clamped ranges, light-script overrides within F3.1's bounds, fog/vista weights, audio
  stem mix) exposed to an in-headset director panel, with A/B hold-to-compare and a "write back to
  data" button that routes through DC-1. The clamps ARE the safety: the panel physically cannot
  produce an out-of-range world.
- **DC-3 — The Verdict Chamber.** Terry's review workflow in-headset: teleport rail through a
  world's contact-sheet moments (the WC-7 queue), thumbs-up/down + voice-note verdicts recorded to
  the machine-readable verdict files, promotion ceremonies (world lock, awe-node pass) as explicit
  in-headset acts. Review stops being "Terry reads PNGs on a monitor" and becomes the product
  reviewing itself.
- **DC-4 — Capture & Showcase.** Photo mode, smoothed spectator/trailer camera paths (never the
  player's head), in-world capture gallery — the tooling that markets the game and feeds store
  assets, built on the booth/contact-sheet camera work. Player-facing photo mode ships from the
  same seam (a first-class feature per the completion roadmap, not a dev leftover).
- **DC-5 — The Cosmetic Forge.** Player-facing appearance (ship skins, glove/chest-rig variants,
  quarters décor) through the existing `CosmeticDefinition` seam + Forge recipes — the visual
  economy the completion roadmap's progression sink needs. Strictly recolor/variant-tier at first
  (the `ForgeModuleLook`/palette machinery already does this); new-geometry cosmetics are ordinary
  Forge assets with ordinary budgets.

**Proof standard:** the acceptance test is Terry's sentence itself, run live: he stands in a world,
issues "move that building," and the building is moved — validated, committed, audited, still 72 Hz,
CI green afterward — with zero manual cleanup. Anything less is a demo, not the envelope.

**Do not:** invent a runtime mutation path that bypasses authors/audits (the one law that makes
this generation safe); let the director panel exist in shipping player builds un-gated
(`DevAccessGate` idiom governs); confuse DC-4's spectator camera with player-camera control
(comfort laws hold — the player's head is sovereign, even Terry's).

---

## §3 — The horizon beyond VII (recorded, not planned)

One sentence each, so future sessions know these were seen and deliberately deferred, not missed:

- **The Tide (live evolution):** seasonal/story-epoch drift of compiled worlds after ship — LS-1
  world states advanced by a calendar instead of a chapter; needs a shipped game.
- **Presence of Others:** multiplayer avatars/shared-world staging — frozen with the multiplayer
  pause; when it wakes, avatars are P0/P1-band Forge citizens like everything else.
- **Player-authored spaces:** quarters/outpost decoration beyond DC-5 cosmetics — a
  factory-plan-adjacent sandbox; explicitly behind FACTORY_TOPTIER_PLAN's own gate.
- **Accessibility & comfort art:** high-contrast/colorblind grade variants and comfort-first vista
  alternates as first-class world states — LS-1 machinery makes them cheap; scope when a real
  player base exists.

## §4 — What this program refuses to become

The standing rejections carry forward with the same force as `ASSET_FORGE_MAP.md`'s:

1. **No parallel toolchain, ever.** V/VI/VII are all expressed as data + authors + `Validate()` +
   audit rules + tests inside Unity CI. A Python world compiler or external staging DSL is a second
   source of truth the gates can't see — rejected for the same reason as the JSON-schema toolchain.
2. **No quality trapdoor at scale.** Compiled worlds pass the same gates as hand-built ones; there
   is no "background world" quality tier. Identity tiers exist (WC-4); quality tiers do not.
3. **No cinematic control theft.** Staging, awe, weather, and direction never take the player's
   head or locomotion. This was law in CP-5 and it is law in LS-2/DC-4.
4. **No budget inflation by generation.** Forge VII inherits Forge II's triangle caps unless a
   device-evidenced, Terry-approved class review changes them — the same sentence FORGE IV wrote,
   still true two generations later.
5. **No unreviewable magic.** Every generation's output remains photographable, auditable, and
   verdict-able. The moment something can't produce a contact sheet and a machine-readable verdict,
   it is outside the program.

## §5 — Immediate next actions (all planning-lane, all freeze-compatible)

1. **Nothing runtime.** The gate ladder (§1) is the schedule; recovery exit is rung one.
2. When FORGE IV reaches its back half: write **`LIVING_STAGE_CONSTITUTION.md`** (Forge V's CP-0)
   — the staging grammar, freedom contract, and world-state data shape are the three contracts to
   lock first.
3. During FORGE IV CP-11: draft **WC-1's world-recipe field list** against the story bible's world
   table, as a paper schema (no SO yet) — it will expose which archetype data FORGE IV should
   already be recording.
4. Keep this document in the `README.md` index after FORGE IV, and amend it whenever a binding
   plan lands that touches its territory.

## §6 — Definition of success (the whole ladder)

The Forge program is finished when all three sentences are true at once:

- A stranger in the headset believes the world, follows its story without reading, and cannot find
  the seam between hand-built and compiled worlds. *(V + VI)*
- Terry directs the game from inside it, and his changes land validated with zero errors. *(VII)*
- A future model session of any size can extend the game and **cannot** — not "should not" —
  ship something that breaks the look, the frame rate, or the story's memory. *(the ratchet,
  fully grown)*
