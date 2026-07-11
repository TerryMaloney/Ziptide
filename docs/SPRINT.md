# 🟡 ACTIVE SPRINT — QUALITY BAR PROGRAM (opened 2026-07-03, post-first-device-test pivot)

> **Takeover prompt: "Read docs/SPRINT.md and continue."** Roadmap: `docs/GAME_PLAN.md`.
> **The approved plan of record:** Terry's first device test said the SYSTEMS work but the EXPERIENCE
> fails — worlds are tiny senseless box-mazes, the ship reads "poor Roblox", menu re-click bug is back,
> entry text unreadable, items drop dead. The fix: keep the world FACTORY, replace the RECIPE.
> Quality targets Terry named: **No Man's Sky** worlds · **Fortnite** fun · **Roblox+** garden/building ·
> **Star Wars** flight. Everything must end up executable by a **mid-level LLM** (data schemas +
> fill-in recipes + build-failing quality gates, never taste-dependent code).
> Prior sprint M4 (ship S1/S2/Quarters/W000): `docs/sprints/` + HANDOFF ddd–ggg. S3 sockets folded
> into P2/P4 below.

## Task board
| # | Task | Status |
|---|------|--------|
| FH-01B | **First-hour binding inventory** — GPT-5.6 claim/log: `docs/GPT_ADDITIONS/2026-07-10_GPT56_FIRST_HOUR/FH01B_IMPLEMENTATION_LOG.md`. Evidence-map all 22 completion signals + all teaching lines to existing files/tokens; classify verified/adapter/content/new-surface/composite. No runtime changes. | 🟡 GPT-5.6 CLAIMED 2026-07-11 |
| FH-01A | **First-hour beat contract** — log: `docs/GPT_ADDITIONS/2026-07-10_GPT56_FIRST_HOUR/FH01A_IMPLEMENTATION_LOG.md`. `docs/first_hour/*` + report-only Python validator/tests; 22 beats, 15 verbs exactly once, no input lock/rig motion. No runtime bindings in this slice. | 🟡 IMPLEMENTED — local exact-blob tests PASS; first Actions artifact pending; FILE CLAIM RELEASED |
| P0.1 | **DevMenu click-once fix** — UI-session rebind on Show + `MENU_CLICK`/`MENU_UI` diags | ✅ `e17eff9` CI-green |
| P0.2 | **DevMenu pager** — 6 worlds/page + PREV/NEXT | ✅ with P0.1 |
| P0.3 | **RILL subtitle readable** — `SubtitleText.Wrap` (5 tests), smaller, lower, fade-in | ✅ `c186afa` |
| P0.4 | **Release feel** — `ReleaseFeel` throw rescue + pulse + FIRST_RELEASE RILL hint | ✅ `749f370` |
| P0.5 | **Pistol visible** — joins starter spawns (3 guns) | ✅ with P0.4 |
| P1a | **Terrain, not slabs** — heightfield 240–320m, 5 biome presets, cliff bowl, slope clamp, pads/corridors graded in | ✅ `27f3ef9` CI-green |
| P1b | **Arrival vista** — 5 hero-landmark kits + midground + spawn faces it + fog auto-thin | ✅ with P1a |
| P1f | **QUALITY GATES** — WORLD_TOO_SMALL / TERRAIN_MISSING / NO_VISTA_LANDMARK (`657086a`) + POI_COUNT_LOW / VERB_VARIETY_LOW / STORY_ANCHOR_MISSING / EST_PLAY_MINUTES_LOW (`4c04320`) — all build-failing | ✅ |
| P1c | **POI system** — PoiDef + 7 pocket builders + poi_ markers + standard 8-POI ring + pure PoiQuality (5 tests) | ✅ `4c04320` |
| P1d | **Breadcrumb route** — cairns every 20m along spawn→POIs route | ✅ `45f5bf5` |
| P1e | **Dressing/scatter** — biome-keyed prop clusters, masked, static-batched | ✅ `45f5bf5` |
| P1g | **Re-recipe W002–W012** — contracts route through POIs (GoPoi/PickupAtPoi/MachineAtPoi/MineAtPoi); W000 stays interior | ✅ this commit |
| P3 | **Garden** (GardenPlotRuntime plant→grow→harvest at HarvestGrove POIs) + **BuildSocket** pay-to-build extractors persisted in WorldState | ✅ `a17d44a` CI-green |
| P4a | **Interim ship hull** — 19-part `ShipHullBuilder` silhouette replaces the 4-cube hull | ✅ `954b969` CI-green |
| P5 | **docs/WORLD_RECIPE.md** — the mid-level-LLM handbook, worked W005 example + spec-first chapter (V2 Q1 aligned) | ✅ this commit |
| — | — **TAKEOVER (2026-07-04): architect is Opus now; T-Dog owns everything but art. HANDOFF (qqq) envelopes below are THE queue.** — | — |
| H3 | **`TerrainField`** (envelope qqq): pure fBM 3–5 octaves + domain warp + temp×moisture biome matrix (each BiomePreset = parameter set); ~15 tests (same-seed identity, slope bound, output range); swap `WorldExperienceBuilder` height fn; `TERRAIN_SLOPE_UNWALKABLE` gate | ✅ this commit: TerrainField (fBM+warp+climate, 15 tests, walkable-fraction contract measured empirically) + builder swap + TERRAIN_SLOPE_UNWALKABLE gate |
| H2 | **`RoomPartitioner`** (envelope qqq): BSP rooms + corridors carved walking back up the tree; template = `Content/Runtime/City/LotPartitioner.cs` (same Rng struct + base cases + access rule); ~12 tests incl. full reachability; feeds ship decks / hero interiors | ✅ this commit: BSP + access-rule corridors (room-center endpoints ⇒ connectivity by induction), 12 tests incl. 25-seed sweep + odd footprints, all contracts pre-verified by simulation |
| H5 | **`ScatterField`** (envelope qqq): pure Poisson-disk, density channels, exclusion masks (pads/corridors/POIs/route), per-kind min spacing; ~10 tests; swap into `WorldDressingBuilder` | ✅ this commit: Bridson Poisson + moisture density channel (TerrainField.Climate) + capsule/disc masks + per-kind spacing, 11 tests, dressing swap done |
| Q2d | **Building proof**: set `buildingStyleId="toxic_tenement"` on one W002 district → APK dispatch → runbook "does it read as a place?" gate | ✅ **APK run `28684427359` GREEN (77 MB artifact)** — W002 GalleryB wears real buildings; Terry's §2n walk is the open gate |
| SPWN | **THE SPAWN WAVE — closed (HANDOFF ttt→vvv):** two layers, BOTH audit-check bugs — wrong marker (POIs plant `poi_*` markers) then ankle-height sphere always grazing ground. Fixes: `__SPAWN_PLAYER` by name · all overlaps listed · check at torso height · spawn-Y/POI-exclusion hygiene retained | ✅ `28684427359` green |
| P4b | **S4 flight scene v1** — READ `docs/design/SPACEFLIGHT_PHYSICS.md` FIRST; comfort-capped cockpit flight in a bounded SpaceLane scene (world moves, never the camera; NO floating origin — trigger not fired); pure `FlightModel` core + tests before the scene translator | 🟡 **CORE SHIPPED this commit** — FlightModel (no-roll BY CONSTRUCTION, snap-only yaw, hard pitch clamp, soft-walled ≤1.8km lane, frame-rate-stable integration; 11 tests). REMAINING (routine-shaped, any operator): ScenePatcherSpaceLane (WorldStubGenerator pattern: cockpit deck + __FLIGHT_WORLD ring course + starfield) + FlightSceneController translator (world moves inversely, rig static — S2 fly-out pattern; stick inputs via InputActionManager idiom) + helm destination pack + runbook 🎮 comfort row. The comfort laws are in the MATH — the translator cannot break them |
| ML | **META-LOOP ARCHITECTURE LOCK** (Terry's brief + GPT addendum; **build plan = `docs/design/ZIPTIDE_META_LOOP.md`**): ResourceDefinition registry + RESOURCE_ID_UNREGISTERED gate · transaction LEDGER + RewardRouter (8 chokepoints wired — the mode contract) · RecipeDefinition factory fields · ProductionGraph (validate/tick/capped catch-up) + WorldState.factory · save schemaVersion v2 + migration fixture · EconomyFlowModel + NO_SOURCE/NO_SINK/UNUSED warns + generated ECONOMY_FLOW_REPORT · **GoldenMetaLoopTests = the acceptance test**. Conquest command-model + income routing ENVELOPED to architecture track; proxy kits + Forge-staleness glue ENVELOPED to Picasso | ✅ this commit |
| A1 | **Succession gap-closures from Terry's PDF** (triage in HANDOFF rrr): `.gitattributes` (+ runbook UnityYamlMerge driver setup for Terry) · 2 new deferral records in `ARCHITECTURE_V2.md` (runtime asset streaming, gateway rate-limiting) · metavr MCP evaluation runbook item | ✅ this commit |
| — | — **STORY BIBLE LOCK — CLOSED 2026-07-06 (HANDOFF gggg→hhhh).** Terry directive: nail the story before authoring W013+. Narrative-only, `docs/storyboard/*` + `THE_TRANSMISSION.md` + one `GAME_PLAN.md` note, zero code touched, all 5 rows shipped. — | ✅ |
| SBL1 | Canon lock: `STORY_BIBLE.md` + `THE_TRANSMISSION.md` flipped PROPOSED→CANON; `MASTER_BUILD_PLAN.md` §3.1 schema-drift note | ✅ `9d59f84` |
| SBL2 | Continuity fixes: deduped `C4_SABLE_INTRO`, tied W038's crack-of-light forward, staged RILL's near-confession at W053 (`FRAGMENT_RILL_CONFESS`) | ✅ `169d36c` |
| SBL3 | **The big rewrite** — `CHAPTER_8-12_ENDGAME.md`: Earth Approach is now a full staged scene sequence; W063 got a new pre-choice RILL line; W064–W068 are genuinely distinct, each landing the partner's fate explicitly; all 4 shipped ending quotes verified word-for-word | ✅ `cfe1564` |
| SBL4 | DLC.md verified consistent (no edits needed) + continuity audit `THE_TRANSMISSION.md` §9b — every flag branch checked, one honest open item logged (`PLAYER_TRUSTED_RILL`/`IGNORED_RILL` trigger design, for M5/engineering) | ✅ `de717ca` |
| SBL5 | Close: `GAME_PLAN.md` M5 prerequisite note + closing HANDOFF (hhhh) for Picasso/Architecture/next story author | ✅ `de717ca` |
| — | — **THE SOUL PASS — CLOSED 2026-07-06 (HANDOFF hhhh to mmmm).** Terry directive after SBL: plot is solid but "one-dimensional" no Cortana/Chief-style bond, no Arbiter-style faction-internal conflict, no felt moral stakes (Halo/Fallout comparison). Narrative-only, `STORY_BIBLE.md` + `CHAPTER_2/3/5/6/7.md`, zero code touched. — | ✅ |
| SOUL1 | `STORY_BIBLE.md` §3b "Cal & RILL the relationship, not just the arc": ambient-line rule (piggybacks Picasso's shipped `RillTrigger.GateDeparture` pool) + "a joke, then a real moment, then a joke" rhythm + "the log" running gag, 8 worked example lines across every memory state | ✅ `8bb00a2` |
| SOUL2 | `STORY_BIBLE.md` §7 "named secondary voices" rule + §9 "no faction mouthpieces" rule (every recurring character must contradict themselves at least once worked examples for Mara/Sable/Aegis-Nine) | ✅ `8bb00a2` |
| SOUL3 | **Mara gets a real voice** 5-beat quoted arc: W005 (contract, brisk) to W012 (before/after the jump, scared and vindicated) to W018 (conviction cracking) to W046 (the pitch to use RILL as the key) to W056 (final line, branches on helped/opposed) | ✅ `7bc23e3` + `fc07d40` |
| SOUL4 | **Sable gets a real voice** W007 (first contact, prickly) to W041 (the last stand, no regrets but slower) to W055 (peace, quieter than winning) | ✅ `7bc23e3` + `fc07d40` |
| SOUL5 | **Warden defector named** (Aegis-Nine to "Nine") W037 (recognizes RILL, breaks cadence) to W043 (argues Cal's case at personal risk) to W049 (the naming payoff, rhymes with RILL's W051 without duplicating it) to W059 (closing callback to RILL's arc) | ✅ `7bc23e3` + `fc07d40` |
| SOUL6 | Close: closing HANDOFF (mmmm) for Picasso/Architecture the bible's relationship/voice rules now govern all future dialogue for W013+ | ✅ this commit |
| — | — **DEPTH PASS 2 — CLOSED 2026-07-06 (HANDOFF mmmm to nnnn).** Terry: Cal (the player) has never spoken a line anywhere; skies should show real "interstellar" space/planets. Real CODE this time (not narrative-only) — `Content`/`Gameplay`/`Editor` C# + docs. — | ✅ |
| D2-1 | `RillLine.speaker` field (default RILL, zero migration risk) + pure `FormatSubtitle()`; `RillCompanion` uses it; `RillLineTests.cs` (3 tests, first EditMode coverage this system has had) | ✅ `ea00f2c` CI-green |
| D2-2 | Cal's lines authored in `RillLineAuthor.cs` (`CalEnter`/`CalFlag`/`CalGate`) — ~24 banter/question lines paired with RILL's existing beats, Ch.0 through the four endings, dormant until each world ships | ✅ `ea00f2c` CI-green |
| D2-3 | `docs/systems/VOICE_PIPELINE.md` (new) — the voClip stub-now/VO-later mechanism explained, exact steps to wire a real clip, casting/tone notes per character; `STORY_BIBLE.md` §3b updated to match | ✅ `ef1b27c` |
| D2-4 | `WORLD_DATA.md` §4.1 — `Sky:` prose → `SkyVistaDefinition` field mapping table for W013+ authoring; `CHAPTER_7_RILL.md` W057 sharpened into a full-frame deep-space corridor shot | ✅ `f4e57cb` |
| D2-5 | `SkyVistaLibrary.cs` W007 Sable Station retuned (second moon body + more stars/nebula, additive only); W012 deliberately untouched (already max intensity by design); `TERRY_RUNBOOK.md` delete+reseed step queued (create-only asset mechanism) | ✅ `f4e57cb` |
| D2-6 | Close: closing HANDOFF (nnnn) for Picasso/Architecture — flags this as the first depth-pass touching real code, not just docs | ✅ this commit |

## ▶ RESUMING? — current state & exact next action
- **ACTIVE CLAIM (2026-07-11): FH-01B** — GPT-5.6 owns only the new binding inventory/validator/test files and the announced non-blocking report expansion. Resume from `FH01B_IMPLEMENTATION_LOG.md`; do not implement adapters or runtime code under this row.
- **FH-01A (2026-07-10): IMPLEMENTED, FILE CLAIM RELEASED** — first-hour contract/validator/tests are in the branch and exact committed blobs pass locally. The non-blocking project-contract Actions artifact remains pending. Runtime bindings are not part of this row; see `FH01A_IMPLEMENTATION_LOG.md`.
- **Current:** **P0–P5 of the Quality Bar Program COMPLETE and CI-green** (menu/subtitle/release
  fixes · terrain+vista · POIs+gates · route+dressing · contracts-through-POIs · garden+sockets ·
  interim hull · handbook). Rebased onto architect's takeover kit `a21fffb`; read
  **`docs/OPERATOR_START_HERE.md`** — its laws (incl. THE CIRCUIT BREAKER) govern this board too.
- **Narrative track (2026-07-06):** THE STORY BIBLE LOCK (SBL1-5) then THE SOUL PASS (SOUL1-6) both
  closed — `docs/storyboard/*` is now canon AND has real character voices (Mara/Sable/Aegis-Nine
  quoted at every signature beat, RILL's ambient-line voice guide in `STORY_BIBLE.md` §3b). W013+
  world authoring (M5) can proceed against a locked, emotionally-load-bearing bible. Zero code touched
  by either pass — next story author still needs to read `WORLD_DATA.md` §4 before authoring.
  **DEPTH PASS 2 (D2-1..6) then closed** — this one DID touch code: Cal now has a `speaker`-tagged
  line pipeline alongside RILL's (dormant, no VO cast — `docs/systems/VOICE_PIPELINE.md` is the
  wiring guide for whoever casts either character), and W007 Sable Station's sky got a real second
  body + more stars/nebula (needs a Unity-side delete+reseed, queued in `TERRY_RUNBOOK.md` §1).
  Mara/Sable/Nine joining the same voice pipeline is the natural next content batch, not yet done.
- **Next action:** **finish P4b** — the pure `FlightModel` core SHIPPED (`2bc39e0`, 11 tests;
  comfort laws are in the math). Remaining is routine-shaped: `ScenePatcherSpaceLane`
  (WorldStubGenerator pattern: cockpit deck + `__FLIGHT_WORLD` ring course + starfield) +
  `FlightSceneController` translator (world moves inversely, rig static — copy the S2 fly-out
  pattern; stick inputs via the InputActionManager idiom) + helm destination pack + runbook 🎮 row.
  Then a full APK dispatch. Session-zero check: this row + the P4b board row are the whole spec.
  **Meta-loop follow-ups live in `docs/design/ZIPTIDE_META_LOOP.md`** (conquest command
  model → architecture track; proxy kits → Picasso; ecology/mutation content → next wave).
- **Device gate:** Terry §2j/§2k runbook rows still open — his ❌s re-prioritize everything.
  §2n (NEW) = the Q2d "does it read as a place?" gate on W002 GalleryB.
- **Lane note (2026-07-04):** T-Dog's Fable ended; Picasso (art, last Fable session) covered Q2d
  and owns cross-track triage until their window ends too — after that ANY capable model resumes
  any track per `OPERATOR_START_HERE.md`.
- **Branch:** `terry-local-wip`. Q2d head: this commit.

## Working rules (unchanged)
CI green per push; SHIPS.md guardrails are law (no rig parenting, no TravelCoordinator bypass, comfort
first — never move the camera); TextMesh only (menus use TMP UGUI already in DevMenu — dev-only file);
.meta per new file; pull --rebase before push; report-only zones need Terry's sign-off.

---
*Quality Bar Program opened 2026-07-03 by T-Dog after Terry's first full device test.*
