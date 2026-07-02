# 🟡 ACTIVE SPRINT — THE MULTIPLAYER PROGRAM (M7, opened 2026-07-02)

> **Takeover prompt: "Read docs/SPRINT_MULTIPLAYER.md and continue."** This is the multiplayer track's
> live state — it runs IN PARALLEL with the story track (`docs/SPRINT.md`, currently M3 creatures) with
> **zero file overlap**: this track owns `Multiplayer/**`, `Gameplay/Runtime/Pvp/**`, arena
> patchers/layouts, and the two design docs. The story track owns creatures/worlds/RILL. If you are a
> fresh session and BOTH sprints are open, pick the one your prompt names; never edit the other's files.
> Designs (the spec source): **`docs/design/PVP_ARENA_AAA.md`** + **`docs/design/TIDEFRONT_AAA.md`**.
>
> **📣 RESUMING 2026-07-02? READ `docs/HANDOFF.md` entry (fff) FIRST** — two of this track's files were
> hot-fixed while you were away (BotMath CS0029 + BotBrain same-tick HeardFire, test-conformance), the
> M3 creature runtime now implements IPvpDamageable with PlayerIndex=-1 (filter for it), and Terry added
> a NEW crossover task for this track: the **PvP pre-round locker** (spawn the shipped `QuartersRoom`,
> gate on round timer, sync equipped-cosmetic strings in the handshake — spec `docs/systems/QUARTERS.md`).
> Roadmap slot: `GAME_PLAN.md` M7a/M7b/M7c.

**Program goal (Terry, 2026-07-02):** PvP from tester-room to AAA fun (smart bots, real arenas, modes,
weapons, endless replay) + Tidefront (the Risk layer: fleets, defenses, planet conquest vs a friend,
story worlds as the map).

---

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | Designs + GAME_PLAN M7 + this sprint file + HANDOFF claim | ✅ `f005caf` |
| A1a | **BotBrain pure core** (`Multiplayer/Runtime/Bots/`): Vec3/BotRng (deterministic), BotPerception/BotDecision, the 8-state machine (reaction-time model, LKP hunting, cover hide/peek cycle, band-holding strafe, flank reposition, sticky retreat, dodge-⊥-threat, lead + bounded aim-error cone), `BotProfileData` Rookie→Nightmare presets + 14 EditMode tests | ✅ this commit (CI pending) |
| A1b | `BotProfileDefinition` SO + 4 difficulty assets (author util) | ⬜ |
| A1c | **PvpBot scene rewrite** consuming BotBrain + waypoint graph/cover baked by patcher + threading `IPvpTransport`/`WeaponCharge` through the live loop *(touches Pvp scene files — safe: story track never edits them)* | ⬜ |
| B1a+b | **Conquest sim core COMPLETE** (`Multiplayer/Runtime/Conquest/`): PlanetNode (14-field spec) · ConquestState (players/stockpiles/turn economy: instability-penalized production, decay, fleet upkeep, attack limits) · ConquestRules (odds clamp 10–90, +5%/pt, anti-snowball constants) · ConquestResolver (seeded, 5 outcomes, vessel/defense specials: gate jammer/piercer, shieldbreaker, minefield, null-ark consumed, repair swarm, dogpile bonus, mission modifiers ± ) · ConquestCatalog (8 defenses + 8 vessels) · **ConquestGalaxy (the map IS W001–W012**, chain + cross-links, 2-player setup) · ConquestAI (3 profiles: reinforce/build/attack-best-odds, plays via the same public API as a human) + **17 EditMode tests** incl. a full headless AI-vs-AI war + JsonUtility save round-trip | ✅ this commit (CI pending) |
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
- **Current micro-step:** B1 (Conquest sim core) committed — verify CI on this push (with A1a's run).
- **⚠ ABSORB FIRST:** the story-track session (T-Dog) finished M3+M4 (creatures, Warden, boardable ship
  S1/S2, Quarters + cosmetics architecture) and left notes in **HANDOFF (fff)** for THIS track: two
  hot-fixes to absorb, a **`PlayerIndex = -1` filter caveat** (creatures implement IPvpDamageable with
  -1 — the A1c PvpBot rewrite and any PvP hit iteration MUST filter index < 0), and a **pre-round
  locker crossover with a frozen API** (Quarters cosmetics ↔ PvP pre-round). Read fff before A1c.
- **Next action:** A1b (BotProfileDefinition SO + author util) then **A1c (PvpBot scene rewrite** —
  perception→BotBrain→execution + waypoint/cover baking in ScenePatcherPvP + thread IPvpTransport +
  WeaponCharge; honor the fff caveats). Then A2 (Arena Factory).
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
