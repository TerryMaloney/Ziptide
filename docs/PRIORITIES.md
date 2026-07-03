# 🎯 PRIORITIES — the whole project, one ordered list (2026-07-03, rev 4 — THE TAKEOVER)

**The single answer to "what matters next, project-wide."** Updated at every track's chunk close.
Tracks: 📖 T-Dog (`SPRINT.md`) · 🎮 MP (`SPRINT_MULTIPLAYER.md`) · 🎨 Picasso (`SPRINT_ART.md`) ·
🏗 architecture (`SPRINT_ARCHITECTURE.md`) · 🧑 Terry.
**Context: Fable 5 is ending — architect is on Opus 4.8 imminently; T-Dog inherits the complicated
work while their Fable lasts; Picasso continues art.** Manual: `docs/OPERATOR_START_HERE.md`.

| # | Priority | Track | Why | State |
|---|----------|-------|-----|-------|
| 1 | **TERRY'S THREE UNBLOCKS** — §2k spec export (one click → spec-driven everything) · the §2j/§2h/§2i headset passes · TWO_QUEST steps 1–4 | 🧑 | every track's next gate waits on one of these; none done yet | ⏳ blocking |
| 2 | **T-Dog Fable window: the three envelopes** — H3 TerrainField → H2 RoomPartitioner → H5 ScatterField (specs in HANDOFF qqq / SPRINT_ARCHITECTURE) + board reconciliation | 📖 | the last complicated cores, built while Fable lasts | envelopes sent |
| 3 | **Q2d building proof** — `buildingStyleId` on a W002 district + APK + "does it read as a place?" | 📖/🏗 | turns H1 from dormant code into the visible overhaul win | 1-line opt-in |
| 4 | **Picasso: building-module Forge family** (`ART_REGISTRY.md`) + board numbering fix | 🎨 | W002's warren wears real art through the registry — the whole art-integration model proven | envelope sent |
| 5 | **Picasso: PERF_BUDGET audit rule** (moved from architecture — their budget doc) | 🎨 | protects 72fps before kit density rises | envelope sent |
| 6 | **Architecture (Opus): Q4a GamePool** + SPEC v2 fields as H-cores land | 🏗 | routine, fully specced — right-sized for the fallback | board-ready |
| 7 | **P5 `WORLD_RECIPE.md`** as the "edit the spec" handbook (post-§2k) | 📖 | locks the LLM-operability story for every future operator | after #1 |
| 8 | **P4b free-flight v1** — per `design/SPACEFLIGHT_PHYSICS.md` rails (never parent the rig to the hull) | 📖 | the Star Wars pillar, now with design rails | after envelopes |
| 9 | **ART: W001 Toxic Venice via Forge + registry** | 🎨 | the world-scale art proof | after #4 |
| 10 | **A6 two-Quest online** (avatar sync on the shipped seam) | 🎮🧑 | unparks when Terry's steps 1–4 land + experience bar met | parked |
| 11 | **A4.5–A4.7 Augments/dual-wield/locator** (`ABILITIES_AND_ARSENAL.md`) | 🎮 | designed, buildable by any operator | parked |
| 12 | **B2/B3 Tidefront table + mission modifiers** | 🎮 | engine proven | parked |
| 13 | **M5 content at scale via WorldSpec** | 📖 | scale on the NEW recipe | after #2/#3 |
| 14 | **RILL VO / adaptive audio** | 🎨📖 | after worlds feel right | — |
| 15 | **M8 cert/store/UX** | all | ship it | — |

**Standing rules:** CI-red jumps to #0 · Terry ❌s slot at #2 · a track never blocks on another's
queue · circuit breaker: 3 CI-reds on one task → stop + HANDOFF write-up · ≤15 rows, re-ordered not appended.
