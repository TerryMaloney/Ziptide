# FORGE V — THE LIVING STAGE
### World dramaturgy: worlds that stage a story, change over time, and remember the player

**Status:** 🔵 PLANNED — no code authorized. Gate: `FORGE_V_AND_BEYOND.md` §1 ladder
(recovery exit → vertical slice → FORGE III close → FORGE IV CP-1..CP-11 → this).
**Performance floor:** unchanged — standalone Quest, stable 72 Hz, every FORGE II–IV budget intact.
**North star:** a player who never reads a menu can tell you what happened in this world last
chapter — because the world itself changed, staged it, and remembers.

> FORGE IV renders a *state* beautifully. The story bible describes *change*. Forge V is the
> generation that renders change: states, scenes, actors, weather, and memory — all expressed
> through the systems Forge I–IV already proved, never through new render paths.

---

## §0 — Rails (read every session, same force as FORGE III §0)

1. The loop is the law: change → CI green → contact sheets → LOOK → iterate → headset verdict.
2. Circuit breaker: 3 CI-reds on one task → stop, HANDOFF, escalate.
3. No scene/prefab YAML edits. All staging is data + authors + runtime ensures.
4. **The freedom contract is absolute:** no system in this generation may rotate the player's
   head, take locomotion, teleport the player, or gate their input for drama. A staged event the
   player walks away from mid-scene must degrade gracefully (actors finish or dissolve; no
   soft-lock). This is testable and IS tested (LS-2 acceptance).
5. **The derivation law extends:** a world state or weather act derives its atmosphere from the
   vista/theme it belongs to. Authored deltas ride ON TOP of derived values within stated clamps
   — never a second hand-authored atmosphere.
6. Ownership: story flags, saves, creature AI, travel, and `RILLCompanion` runtime logic belong to
   their existing owners. Forge V owns *how change reads* — the visible/audible layer only. Every
   cross-lane touch gets a named claim before implementation (seam table §8).

## §1 — Vocabulary (locked here so every later doc means the same thing)

- **World state** — a named, story-flag-selected parameterization of one world
  (`pre_bloom` / `blooming` / `scarred` / `reclaimed` / world-specific names). NOT a scene copy.
- **State delta** — the data difference between two states of one world: vista variant, grade
  variant, practical set/color weights, dressing additions/removals (by author tag, create-only),
  material mask weights (CP-3), audio identity variant (CP-8), signage content.
- **Staged event** — a finite authored moment built from the closed grammar (§3): actors + marks +
  timing + audio + optional Awe-Node linkage. Runs in world space, player fully free.
- **Act** — a weather/tide configuration of a world (LS-5). Acts are orthogonal to states:
  state = story, act = sky/water/air right now. `state × act` must stay coherent by derivation.
- **Mark** — a persistent player-caused visible change (LS-6): repaired machine lit, route opened,
  name on a manifest board.

## §2 — LS-1 · WORLD-STATE SKINS

**Why first:** every other envelope needs states to exist. Also the cheapest: it is almost
entirely re-parameterization of shipped systems.

**What:** a `WorldStateVariant` block (paper schema now; SO fields at implementation):

```yaml
worldId: W001
stateId: blooming            # closed per-world list, declared in the world recipe
storyFlagGate: bloom_ch2     # resolved by the EXISTING flag system (owner: story lane)
vistaOverride: w001_bloom    # optional; must exist in SkyVistaLibrary
gradeDelta: { saturation: +8, temp: +5 }      # inside F3.2 clamps — clamps re-asserted in tests
practicalPalette: spore_amber                  # F3.1b PracticalColor family
dressingTags: { add: [bloom_growth], remove: [] }  # author-tag granularity, create-only
maskWeights: { growth: 0.7, wetness: 0.4 }     # CP-3 mask channels
audioVariant: w001_blooming                    # CP-8 identity variant
signageSet: quarantine                          # F3.7 content set
```

**Resolution:** one `WorldStateResolver` (editor/author + runtime ensure) reads flags → picks the
state → applies deltas through each system's EXISTING apply path (`SkyVistaRig`, grade volume,
`ForgeModuleLook`, `PracticalLight`, audio director stems). No new renderers, no new materials.
**The default-state law:** every world's `default` state must equal today's shipped look
byte-for-byte (hash-stable), so LS-1 landing changes NOTHING until a flag says so.
**Acceptance:** EditMode — default state is identity (hash test); every non-default delta is inside
its host system's clamps; unknown flag → default (never blank). Audit: `WORLD_STATE_INVALID`
blocker, `WORLD_STATE_UNGATED` warn. Log `ZIPTIDE: WORLD_STATE world=W001 state=blooming`.
**Contact sheet:** per state, the CP-1 world sheet re-rendered — a state ships as a *sheet pair*
(default vs. state) or it doesn't ship. **Budget:** 3 commits (schema+resolver / W001 pilot pair /
audit+docs). **Do not:** copy scenes, fork themes, express a state as code branches in world
scripts, or let two states differ in geometry ownership.

## §3 — LS-2 · THE STAGING VOCABULARY

**Why:** story beats need a *form*. Without a closed grammar every beat becomes bespoke code —
unreviewable, unbudgeted, and eventually head-grabbing.

**The closed grammar (extend by decision, never by exception):**
`reveal` · `arrival` · `collapse` · `emergence` · `procession` · `signal` · `departure` ·
`aftermath`. Each is a template with declared slots:

```yaml
eventId: w001_barge_arrival
kind: arrival
actors: [barge_silhouette_p3, dock_crew_p3x3]   # P3-band impostors unless a named P1 hero actor
marks: [path_canal_south, mark_dock_align]       # author-placed markers, not coordinates
aweNode: w001_dock_reveal                        # optional CP-5 linkage
duration: { min_s: 20, max_s: 45 }
trigger: { flag: ch1_signal_sent, region: dock_approach }
audio: { cue: w001_horn_distant, stem: arrival } # CP-8
vfx: [steam_vent]                                # F3.5 library ids only, ≤2 systems
lights: { hero_points: 0 }                       # CP-6 hero spend, released at end
interrupt: dissolve                              # dissolve | complete_fast | persist_static
repeat: once_per_state                            # once | once_per_state | ambient_loop
```

**Runtime:** one `StageDirector` per world scene (content-side, NOT a `_Boot` singleton) that
owns ≤1 active staged event + ≤2 ambient loops. Actors are Forge visuals on spline/mark paths with
the Creature-V2 motor where alive — the director *plays* them, it does not think for them.
**The freedom contract, tested:** contract tests assert the director never references the rig
transform for writes, never calls locomotion APIs, and interrupt paths run headless.
**Acceptance:** `Validate()` clamps (duration, actor count ≤6, vfx ≤2, P-band rules); grammar
closed-enum test; interrupt-behavior tests per kind. Log `ZIPTIDE: STAGE_EVENT id=… phase=…`.
**Contact sheet:** per event, a *timeline strip* — 4 frames (before / early / peak / after) from
the player-likely position, plus the CP-1 performance view at peak.
**Budget:** 4 commits (schema+validate / director+freedom tests / pilot event / audit+sheets).
**Do not:** exceed one active staged event; spawn actors above P-band; write bespoke event
MonoBehaviours; let `persist_static` actors accumulate (aftermath converts them to dressing or
removes them).

## §4 — LS-3 · RILL CINEMATIC PRESENCE

**Why:** RILL is the emotional spine of the story (12 canon beats, master plan §5) and currently
has no staging body — a companion that is only a voice wastes the whole presence investment.

**What (visual dramaturgy ONLY — `RILLCompanion` runtime/AI stays with its owner):**
- **Blocking library:** `perch` (on declared perch markers near Awe Nodes), `orbit` (1.2–2.5 m,
  never inside 0.6 m comfort bubble, never directly behind the head), `lead` (ahead on the route,
  waits at corners), `witness` (still, watching the staged event with the player).
- **Look-at:** Creature-V2 motor look-at channel; RILL looks at what the player looks at during
  `witness`, at the player during dialogue, at the Awe anchor during `reveal`.
- **Arc growth:** per story stage, a visual growth delta on the SAME genome (size clamp ×1.0→×1.6
  across the whole game, emissive complexity, trail VFX from the F3.5 library) — data, not new
  bodies, so the silhouette stays recognizably RILL from first beat to last.
- **Voice-light coupling:** emissive pulse follows VO amplitude envelope (baked per-line envelope
  data, no runtime FFT).
**Acceptance:** blocking safety tests (comfort bubble, no behind-head hover, perch validity);
growth clamp tests; coupling is data-driven (test: no audio analysis at runtime).
**Checkpoint:** headset-first — RILL comfort cannot be judged in a booth. Booth ships the growth
ladder turnarounds (stage 1 → final) like the creature ladder.
**Budget:** 3 commits. **Do not:** pathfind (RILL floats on author marks + smoothing), emote with
head-tracking writes, or grow past clamp for drama.

## §5 — LS-4 · AMBIENT SOCIETY

**Why:** CP-4 gives depth to the *air*; this gives depth to *civilization*. Distant life is the
cheapest awe there is when it stays in band.

**What:** an `AmbientLifeDefinition` per world (library pattern): ≤4 lanes, each a P3/P4-band
mover set — silhouette barges on far canals, procession dots on a ridge road, flock impostors
(≤2 crossed cards each, ≤12 agents/lane), light-crawl windows on distant structures (emissive
mask animation, zero geometry). Two update rates only: `far` (2 Hz) and `veryfar` (0.5 Hz);
movers advance along baked paths — position is a function of time, so a lane costs one transform
write per agent per tick and **zero** AI.
**State/act aware:** lanes may gate on world state (`blooming` empties the processions) — one
more reason LS-1 lands first.
**Acceptance:** band enforcement tests (no lane inside P2 distance), update-rate contract, agent
caps, path determinism. Audit `AMBIENT_LANE_OVERBAND` blocker. Log `ZIPTIDE: AMBIENT lanes=…`.
**Contact sheet:** the CP-1 vista view re-shot with lanes on/off — the diff should read as "the
world is inhabited," not as noise. **Budget:** 3 commits.
**Do not:** let ambience approach the player (a lane that can reach P1 distance is a creature and
belongs to the ecology lane); animate at full rate; exceed two update rates.

## §6 — LS-5 · WEATHER & TIDE ACTS

**Why:** a static sky contradicts a living world; and the game is named after a tide it never shows
moving. Acts are the biggest state-change *feel* per byte.

**What:** 2–3 acts per signature world, declared in the world recipe:
- **Sky/air:** act = a vista-derived delta (cloud layer density, haze depth, key intensity −40%
  max, fog density ×0.8–1.6) — computed FROM the vista by the F3.1 deriver extension, clamped.
- **Precipitation:** F3.5 library gains `rain_sheet` / `spore_fall` / `ash_drift` kinds under the
  existing 64-particle/6-system caps; wetness ties to CP-3 mask weights (rain raises `wetness`
  globally at ≤0.15/min so surfaces darken *believably*, not switch).
- **THE TIDE:** tide level is an act input consumed by `ZiptideWater` bodies: water Y interpolates
  over minutes (≤0.5 m amplitude per world unless the recipe says otherwise), foam band re-insets,
  a `tide_line` grime decal (F3.4 machinery) marks the high line on berth walls. This is the
  envelope that may fund the deferred hand-written water shader — **only** with a device frame
  budget verdict first; stock-URP tide ships regardless.
- **Transitions:** acts crossfade over 30–120 s real time; never snap; at most one transition in
  flight; transition state is saved (no weather reroll on reload).
**Acceptance:** derivation determinism, clamp tests, transition-single-flight test, save
round-trip of act state. Log `ZIPTIDE: ACT world=… act=… t=0.42`.
**Checkpoint:** headset — rain readability and tide comfort are device verdicts; booth ships the
precipitation kinds as turnaround subjects. **Budget:** 4 commits.
**Do not:** global wetness above clamp (everything-shiny tell); thunder/lightning full-screen
flashes (comfort); more than 3 acts per world; weather that fights the state's grade.

## §7 — LS-6 · RETURN & MEMORY

**Why:** the strongest presence trick in games: the world noticed you. Almost free — it renders
flags the save system already keeps.

**What:** a `MarkDefinition` vocabulary (library): `machine_lit` (repaired machine's practicals +
signage stay on), `route_open` (cleared gate stays visibly open + its cairn lights), `name_ledger`
(player tag on the dock manifest board via F3.7 signage), `creature_truce` (a creature type's
tell shifts to neutral in this world — ForgeBodyTell channel), `garden` (a planted flora cluster
persists and grows one stage per return visit).
Marks resolve from save/story flags through the same resolver pattern as LS-1 (marks are
micro-states). CP-8's return stem may reference mark count (more marks → warmer return theme
variant) — data linkage only.
**Acceptance:** mark idempotence (re-applying is a no-op), save round-trip, unknown-mark = skip +
warn. Audit `MARK_UNRESOLVED` warn. Log `ZIPTIDE: MARK id=… applied=…`.
**Contact sheet:** first-visit vs. return sheet pair per pilot world.
**Budget:** 2 commits. **Do not:** let marks mutate geometry ownership, store world state in
scenes, or invent a second persistence path (saves own truth; marks render it).

## §8 — Cross-lane seam table (claims required BEFORE implementation)

| Seam | Owner (authoritative) | Forge V touches |
|---|---|---|
| Story flags / chapter state | story/gameplay lane | read-only resolution (LS-1/2/6) |
| Save system | gameplay lane | read-only for marks; act-state field is a NEW claim |
| `RILLCompanion` runtime | gameplay lane (master plan §5) | visual blocking/growth layer only |
| Creature AI / ecology | ecology lane | StageDirector plays actors it OWNS; never live creatures |
| `AudioDirector` | `_Boot` (locked) | stem/variant requests via existing API only |
| `TravelCoordinator` | locked contract | none — staging never travels the player |

## §9 — Order, pilot, and definition of done

Order: **LS-1 → LS-2 → LS-5 → LS-3 → LS-4 → LS-6** (states first; acts before RILL because RILL's
beats reference weather moods; society and memory close).
**Pilot = W001:** two states (`default`, `blooming`), one `arrival` staged event, two acts
(calm / spore-fall + tide swing), RILL beats 1–2 blocked, one ambient lane, three mark types.
**FORGE V is done when:** Terry walks W001 twice a chapter apart and — without being told —
describes what changed; the staged arrival lands while he is free to walk away from it; frame
time on device is within FORGE IV's measured envelope; and every state/event/act shipped with its
sheet pair and a headset verdict.
