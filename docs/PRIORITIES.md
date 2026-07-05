# 🎯 PRIORITIES — the whole project, one ordered list (2026-07-05, rev 7 — TEST DAY 1 VERDICT)

> **rev 7 change:** Terry's first full device pass landed (HANDOFF dddd). Per the standing rules
> his ❌ list is now **#2–#3**: `docs/TEST_DAY_1_RESPONSE.md` **Wave 1** (10 one-commit quick
> fixes — tiny guns, grip pose, tracers, arena spawn, W003 z-fight, hazard tells, HUD size,
> matchboard text, kiosk beacon, subtitle) then **Wave 2** (Fortnite-grade movement + aim v2).
> Wave 3 items map onto existing rows (flight = P4b, destruction v2 = MP lane, HUD/how-to = new
> UI row). E1.4 (old #2) slides to #4 — still the biggest visual win. Everything below shifts.

**The single answer to "what matters next, project-wide."** Updated at every track's chunk close.
Tracks: 📖 story (`SPRINT.md`) · 🎮 MP (`SPRINT_MULTIPLAYER.md`) · 🎨 Picasso (`SPRINT_ART.md`) ·
🏗 architecture (`SPRINT_ARCHITECTURE.md`) · 🧑 Terry.
**Context: Fable 5 access ends within days. This revision is the SUCCESSION ORDER for Opus/Sonnet
operators: every row is either fully specced in a doc or a boarded envelope. Start protocol:
`docs/OPERATOR_START_HERE.md` → your track's board → this list.**

| # | Priority | Track | Why | State |
|---|----------|-------|-----|-------|
| 1 | **TERRY HEADSET PASS on APK run `28752341766`** (sha `6681a7f`) — §2h skies · §2i forged taser · §2n W002 tenements · §2b 11 worlds · §2c/§2d story/jobs · economy `ECON_*` logs. His ❌s become #2. Plus §2k spec export + TWO_QUEST steps 1–4 | 🧑 | every track's next gate waits on device truth; APK dispatched and downloadable — zero Unity steps needed | ⏳ building |
| 2 | **FORGE II E1.4 — ForgeBaker (bake→ASTC prefabs)** per `FORGE_II_QUALITY_LEAP.md` §E1.4: puts the E1.3 TEXTURED look ON DEVICE (today's runtime is still flat-color fallback) | 🎨 | the single biggest visible win per commit left on the board; E1.1–E1.3 shipped + photo-proven | ⬜ next art |
| 3 | **E5.2 PERF_BUDGET audit gate** (1 commit, zero dependencies, fully specced) | 🎨 | ideal FIRST commit for any fresh operator to learn the loop safely | ⬜ unowned |
| 4 | **P4b free-flight v1** — `design/SPACEFLIGHT_PHYSICS.md` rails (pure FlightModel + tests first; never parent the rig to the hull) | 📖 | the Star Wars pillar; design rails written | ⬜ unowned |
| 5 | **Architecture: Q4a GamePool + SPEC v2 fields** (`scatterSpec`/`storyBeats`) | 🏗 | routine, fully specced — right-sized for any operator | board-ready |
| 6 | **META-LOOP economy follow-through** — the one-economy spine landed (`54f75ba`+`9b890f1` dup fix); wire the flow report into Terry-facing balance passes | 📖/🏗 | keep the new spine honest before content scales on it | 🟡 fresh |
| 7 | **P5 `WORLD_RECIPE.md`** as the "edit the spec" handbook (post-§2k) | 📖 | locks LLM-operability for every future operator | after #1 |
| 8 | **FORGE II P2 geometry ops → P3 skinned creatures → P4 motion** (envelopes in the FORGE_II doc; stalker's WeakPoint contract already law) | 🎨 | creatures+movement are Terry's second quality wave | after #2 |
| 9 | **E5.1 building modules + ART-4 W001 Toxic Venice via Forge+registry** | 🎨 | the world-scale art proof; pairs with shipped Q2d buildings | after #2 |
| 10 | **A6 two-Quest online** (avatar sync on the shipped seam) | 🎮🧑 | unparks when Terry's steps 1–4 land | parked |
| 11 | **A4.5–A4.7 Augments/dual-wield/locator** (`ABILITIES_AND_ARSENAL.md`) | 🎮 | designed, buildable by any operator | parked |
| 12 | **B2/B3 Tidefront table + mission modifiers** | 🎮 | engine proven | parked |
| 13 | **M5 content at scale via WorldSpec** | 📖 | scale on the NEW recipe | after #1/#7 |
| 14 | **RILL VO / adaptive audio (ART-5)** | 🎨📖 | after worlds feel right | — |
| 15 | **M8 cert/store/UX** | all | ship it | — |

**Recently CLOSED (don't redo):** Q2d building proof (shipped, APK `28684427359` + ChamberA fix
`9d30dd8`) · ASSET FORGE reconciliation R1–R4 (`ASSET_FORGE_MAP.md` is the law against re-invention) ·
FORGE II E1.1–E1.3 (UVs, textures, normal/MSA/emissive, ONE material, booth env pinned, atlas x-ray
in every photo artifact) · stalker proxy through 4 photo-critique cycles (`Limb()` joint-point law).

**Standing rules:** CI-red jumps to #0 · Terry ❌s slot at #2 · a track never blocks on another's
queue · circuit breaker: 3 CI-reds on one task → stop + HANDOFF write-up · ≤15 rows, re-ordered not appended.
