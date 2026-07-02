# 🟡 ACTIVE SPRINT — THE MULTIPLAYER PROGRAM (M7, opened 2026-07-02)

> **Takeover prompt: "Read docs/SPRINT_MULTIPLAYER.md and continue."** This is the multiplayer track's
> live state — it runs IN PARALLEL with the story track (`docs/SPRINT.md`, currently M3 creatures) with
> **zero file overlap**: this track owns `Multiplayer/**`, `Gameplay/Runtime/Pvp/**`, arena
> patchers/layouts, and the two design docs. The story track owns creatures/worlds/RILL. If you are a
> fresh session and BOTH sprints are open, pick the one your prompt names; never edit the other's files.
> Designs (the spec source): **`docs/design/PVP_ARENA_AAA.md`** + **`docs/design/TIDEFRONT_AAA.md`**.
> Roadmap slot: `GAME_PLAN.md` M7a/M7b/M7c.

**Program goal (Terry, 2026-07-02):** PvP from tester-room to AAA fun (smart bots, real arenas, modes,
weapons, endless replay) + Tidefront (the Risk layer: fleets, defenses, planet conquest vs a friend,
story worlds as the map).

---

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | Designs + GAME_PLAN M7 + this sprint file + HANDOFF claim | 🔄 this commit |
| A1a | **BotBrain pure core** (`Multiplayer/Runtime/Bots/`): BotPerception/BotDecision structs, the 8-state utility machine, seeded determinism + `BotProfileData` (pure mirror of the SO fields) + ~15 EditMode tests | ⬜ next |
| A1b | `BotProfileDefinition` SO + 4 difficulty assets (author util) | ⬜ |
| A1c | **PvpBot scene rewrite** consuming BotBrain + waypoint graph/cover baked by patcher + threading `IPvpTransport`/`WeaponCharge` through the live loop *(touches Pvp scene files — safe: story track never edits them)* | ⬜ |
| B1a | **Conquest sim core** (`Multiplayer/Runtime/Conquest/`): PlanetNode, ConquestState (+ story-world galaxy builder), ConquestRules, ConquestResolver (seeded, clamp 10–90, 5 outcomes) + ~25 tests | ⬜ next |
| B1b | Defense/vessel catalogs as pure data tables in ConquestRules (Definitions SO pass comes with B2) + ConquestAI + save round-trip + ~15 more tests | ⬜ |
| A2 | Arena Factory: `ArenaLayoutDefinition` + `ArenaLayoutLibrary` (5 arenas) + `ScenePatcherArena` + BuildAndroid hook | ⬜ |
| A3 | PvpMatch → N combatants/teams + Gun Game + KotH + Fragment Rush + Horde (creature waves) | ⬜ |
| A4 | Arsenal: Static Net, Sonic Thumper, Prism Beam + pads + WeaponCharge wiring + bot weapon prefs | ⬜ |
| A5 | Progression: match stats, credits payout, unlock flags, daily seed | ⬜ |
| B2 | Holo war table vs ConquestAI (Sandbox placement) | ⬜ |
| B3 | VR mission modifiers (ConquestMissionLibrary → real world contracts → odds mods) | ⬜ |
| B4 | Hotseat sync → (after A6) Photon live sync | ⬜ |
| A6 | Photon PUN2 online (needs Terry's PC) + polish | ⬜ |
| — | Close: HANDOFF, checklist, playbook rows per chunk, APK dispatch green | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** committing the program docs (designs, GAME_PLAN M7, this file, HANDOFF claim).
- **Next action:** build **A1a (BotBrain pure core)** then **B1a (Conquest sim core)** — both pure C#,
  no scene edits, fully CI-verifiable. Specs live in the two design docs (§A1 / §"The sim"); mirror the
  PvpRules/PvpMatch pure-core pattern (`Multiplayer/Runtime/`, no Unity types, EditMode tests in
  `Tests/EditMode/`). New folders under Multiplayer/Runtime need folder + file `.meta`s (python-uuid
  pattern used everywhere this session).
- **Verified facts (don't re-derive):** PvP live loop currently BYPASSES `IPvpTransport` and never uses
  `WeaponCharge` — A1c threads both. Bot today = range-keeper (spec of its exact behavior + gaps is in
  PVP_ARENA_AAA "Why this will work"). The Multiplayer asmdef is pure C# (no Unity refs) — **keep
  BotBrain/Conquest free of UnityEngine**; SOs (BotProfileDefinition etc.) go in **Content**, with pure
  mirror structs in Multiplayer (the PvpRules pattern). Tests asmdef already references Multiplayer.
- **Branch:** `terry-local-wip` @ `33bdf22`+ (story track M3 is mid-flight — tasks 4–7 of SPRINT.md
  are THEIRS; do not touch creature files).

## Working rules
CI green per push; SPRINT_MULTIPLAYER updated in the same commit as every push; APK dispatch at chunk
boundaries; comfort + non-lethal + readable-telegraph canon applies to bots and arenas exactly as to
creatures; never yank the player camera.

---
*Opened 2026-07-02 by the operator (Fable 5) — GAME_PLAN M7, Terry-directed multiplayer focus.*
