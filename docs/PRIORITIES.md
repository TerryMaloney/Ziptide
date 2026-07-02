# 🎯 PRIORITIES — the whole project, one ordered list (2026-07-02, rev 2)

**The single answer to "what matters next, project-wide."** Updated at every track's chunk close.
Detail lives in `GAME_PLAN.md` (milestones) + the three sprint files; this page is the cross-track order.
Tracks: 📖 story/ship "T-Dog" (`SPRINT.md`) · 🎮 MP "Architect" (`SPRINT_MULTIPLAYER.md`) · 🎨 art "Picasso"
(`SPRINT_ART.md`) · 🧑 Terry.

| # | Priority | Track | Why it's here | State |
|---|----------|-------|---------------|-------|
| 1 | **THE DEVICE PASS** — runbook §1 + §2b–§2h (story worlds, RILL, repair, creatures, ship, W000 opening, smart bot + 5 arenas, skyscapes) | 🧑 | ~seven milestones of CI-green work have never touched a headset; every feel-tuning decision downstream waits on these ❌s/notes | ⏳ blocking |
| 2 | **A3-scene: modes playable** — mode director + lobby board (arena × mode × difficulty) + attacker identity + Horde spawning | 🎮 | the engines exist; this makes the arenas an actual GAME you can replay | in flight |
| 3 | **TWO-QUEST ONLINE SETUP** — Terry's setup instructions (Photon account/App ID/PUN2 import) + the transport-adapter seam so two headsets can fight | 🎮🧑 | Terry directive 2026-07-02: instructions due at the A3-scene sprint close; Terry-side setup runs in parallel with remaining code | due next close |
| 4 | **A4: the arsenal** — Static Net / Sonic Thumper / Prism Beam runtimes + pads become timed respawners | 🎮 | Gun Game's ladder already names them; map control needs them | specced |
| 5 | **Abilities + dual-wield + locator v2** (`design/ABILITIES_AND_ARSENAL.md`) — the Augment item category (all modes), charge-pool dual-wield, wrist locator A-grade rework | 🎮 | Terry directive 2026-07-02; touches story AND arena AND Tidefront | designed → build |
| 6 | **ART-2: W001 Toxic Venice kit + PERF_BUDGET audit rule** | 🎨 | the first real surface family + the rule that protects 72fps forever (ART-1 skyscapes ✅ APK-green) | next art session |
| 7 | **M4 tail: S3 ship upgrade sockets** (+ ToxicCity theme wiring — Picasso's queued request) | 📖 | the economy's flagship sink; W000 opening shipped ✅, S3 is what's left | specced |
| 8 | **B2: the Tidefront holo war table** (vs the AI) | 🎮 | the war engine is proven; this makes it a mode Terry can sit at | engine done |
| 9 | **M5: Chapter 3 (W013–W019)** via the world factory — each batch WITH its new tool/creature/beat | 📖 | content scale resumes once the device pass validates the Ch.1–2 base | factory ready |
| 10 | **B3: Tidefront VR mission modifiers** — attacks offer real contracts inside the story worlds | 🎮 | the killer crossover; reuses existing job steps | after B2 |
| 11 | **ART-3: audio foundation + RILL VO** (adaptive stems, ambience, ElevenLabs into the existing line slots) | 🎨 | the emotional spine gets its voice; text stubs already carry the data | slots exist |
| 12 | **A6/B4: online play live** — PUN2 adapter on `IPvpTransport`, room codes, then live Tidefront | 🎮🧑 | lands on the #3 setup; bots backfill empty slots by design | seam ready |
| 13 | **M5 back half: Ch.4–12 + the 4 endings** | 📖 | the full arc | — |
| 14 | **ART-4: creature/gear/VFX kits across chapters** | 🎨 | after shapes stabilize | — |
| 15 | **M8: UX/menus/comfort/save-slots → Meta cert → store assets** | all | ship it | — |

**Standing rules:** anything CI-red jumps to #0 · Terry's ❌s/feel-notes from #1 slot in at #2 ·
a track never blocks on another track's queue (pull your own next item) · this list is re-ordered, not
appended — keep it ≤15 rows.
