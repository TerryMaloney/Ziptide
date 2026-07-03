# 🎯 PRIORITIES — the whole project, one ordered list (2026-07-03, rev 3 — post-device-test + PDF overhaul)

**The single answer to "what matters next, project-wide."** Updated at every track's chunk close.
Tracks: 📖 story "T-Dog" (`SPRINT.md`) · 🎮 MP "Architect-MP" (`SPRINT_MULTIPLAYER.md`) · 🎨 art "Picasso"
(`SPRINT_ART.md`) · 🏗 architecture (`SPRINT_ARCHITECTURE.md`) · 🧑 Terry.
Context shift: Terry's device verdict = "systems fire, EXPERIENCE fails" → the Quality Bar program
(T-Dog) + the ARCHITECTURE V2 program (Terry's PDF) now outrank new modes/content.

| # | Priority | Track | Why | State |
|---|----------|-------|-----|-------|
| 1 | **DEVICE RE-TEST of the quality wave** — runbook §2j+ (terrain worlds W002/W003/W006, POIs/breadcrumbs, garden/build sockets, new ship hull, forged taser, subtitles/menu fixes) | 🧑 | the P0–P4a + Forge work exists to fix his exact ❌ list; only the headset can confirm | ⏳ next Terry sitting |
| 2 | **V2-Q1: the WorldSpec** — one spec per world + validator + compiler (the "request it and it happens" keystone) | 🏗 | everything downstream (buildings, terrain, art) hangs off one editable document per world | in flight |
| 3 | **V2-Q2: buildings, not boxes** — lot partitioning + socketed building grammar + gates | 🏗 | the literal-architecture half of Terry's mandate; the biggest remaining "Roblox" reader | after Q1 |
| 4 | **ART-3: W001 Toxic Venice built WITH the Forge** (+ ToxicCity theme wiring) | 🎨 | the Forge proved the loop on one taser; now a whole world's look | Picasso resume point |
| 5 | **P5: `WORLD_RECIPE.md`** — the mid-level-LLM world handbook (post-Q1: "edit the spec") | 📖 | locks in LLM-operability; cheap after Q1 | T-Dog resume point |
| 6 | **V2-Q3: TerrainField** (fBM + domain warp + biome matrix) | 🏗 | kills the "every hill is the same hill" flatness at the math layer | after Q2 |
| 7 | **V2-Q4: perf** — GamePool + the PERF_BUDGET gate + shader WARN | 🏗 | protects 72fps before art density rises (ART-3 raises it) | after Q3 |
| 8 | **P4b: S4 free-flight** (comfort-capped cockpit) | 📖 | the Star Wars pillar's first playable slice | specced |
| 9 | **A6: two-Quest online** — avatar sync over the shipped seam (Terry's Photon setup = TWO_QUEST_SETUP steps 1–4, still his homework) | 🎮🧑 | the seam + adapter are dormant-ready; unpark when experience bar is met | parked by Terry's pause |
| 10 | **A4.5–A4.7: Augments / dual-wield / locator v2** (`design/ABILITIES_AND_ARSENAL.md`) | 🎮 | designed, buildable; after the experience overhaul | parked |
| 11 | **B2/B3: Tidefront war table + mission modifiers** | 🎮 | engine proven; needs the world quality bar first | parked |
| 12 | **M5: Chapter 3+ content at scale** — via WorldSpec once Q1/Q2 land | 📖 | content scale resumes on the NEW recipe, not the old one | after Q1/Q2 |
| 13 | **ART-4: creature/gear kits via Forge** (+ the real ship hull — Picasso's top asset target) | 🎨 | after W001 proves the world-scale Forge flow | — |
| 14 | **RILL VO + adaptive audio (ART-5/M6)** | 🎨📖 | the emotional layer once worlds feel right | — |
| 15 | **M8: cert/store/UX** | all | ship it | — |

**Standing rules:** CI-red jumps to #0 · Terry ❌s slot in at #2 · a track never blocks on another's
queue · re-ordered, never appended — keep ≤15 rows.
