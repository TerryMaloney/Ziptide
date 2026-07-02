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
| A1b | `BotProfileDefinition` SO (Content, +Multiplayer asmdef ref — acyclic) + `BotProfileAuthor` (create-only, build-wired: rookie/regular/veteran/nightmare into `Resources/Bots`) | ✅ this commit (CI pending) |
| A1c | **PvpBot rewrite: the brain is in the arena.** Perceive (LOS, player velocity, incoming-dart scan @5Hz, baked cover/waypoints) → `BotBrain.Tick` → execute (CollideMove kept, dodge burst-steps, strafe, telegraph LAW kept, `WeaponCharge`-gated visible `PvpBolt` at the brain's led+erred aim). `ScenePatcherPvP.BuildBotNav` bakes Way_1-8 patrol ring (incl. platform top) + Cover_P1-8 shadow spots. Brain+charge reset on revive. *(Deliberate re-scope: `IPvpTransport` threading of the whole match loop moved to A6-prep — the bot rewrite doesn't need it and scoping it here risked the match flow before a device pass.)* | ✅ this commit (CI pending) |
| B1a+b | **Conquest sim core COMPLETE** (`Multiplayer/Runtime/Conquest/`): PlanetNode (14-field spec) · ConquestState (players/stockpiles/turn economy: instability-penalized production, decay, fleet upkeep, attack limits) · ConquestRules (odds clamp 10–90, +5%/pt, anti-snowball constants) · ConquestResolver (seeded, 5 outcomes, vessel/defense specials: gate jammer/piercer, shieldbreaker, minefield, null-ark consumed, repair swarm, dogpile bonus, mission modifiers ± ) · ConquestCatalog (8 defenses + 8 vessels) · **ConquestGalaxy (the map IS W001–W012**, chain + cross-links, 2-player setup) · ConquestAI (3 profiles: reinforce/build/attack-best-odds, plays via the same public API as a human) + **17 EditMode tests** incl. a full headless AI-vs-AI war + JsonUtility save round-trip | ✅ this commit (CI pending) |
| A2 | **Arena Factory BUILT**: `ArenaLayoutDefinition` (geometry/nav/spawns/objectiveZones/weaponPads/hazards/sky + Validate) + `ScenePatcherArena` (generic shell: data geometry, per-arena sky via ThemeAuthor overloads, Way_/Cover_P nav, difficulty-tagged bot, pads, breakwalls, hazards via HazardZoneRuntime, pack+exit, Build Settings; build-hooked like generated worlds) + `ArenaLayoutLibrary` (**5 launch arenas**: Cistern dark hill-fight · Chitinwall catwalk alleys · MirrorFlats marksman lanes (veteran) · Tidal islands w/ live flood mutator · Void Shell-gate bridges (nightmare)) + 7 `ArenaLibraryTests` (validate-clean, unique ids, real nav, armed+objectives, 3+ difficulty tiers, distinct skies, waypoints-in-bounds). Original PvP_Arena01 untouched. | ✅ this commit (CI pending) |
| A3-core | **Mode engines (pure) BUILT**: `PvpMatch` generalized to N combatants (2–4) + teams (team score = summed members; default ctor stays 1v1 — zero consumer changes) + `EndByRule` · `Modes/PvpModes.cs`: **GunGameState** (6-weapon default ladder), **KothState** (sole-king accrual, contested=nobody, zone rotation), **FragmentRushState** (single carrier, drop-resets-to-mid, bank-to-win), **HordeState** (deterministic escalating waves of bots+creatures, capped for Quest perf, clear bonuses) + 11 tests | ✅ this commit (CI pending) |
| A3-scene | Mode director consuming the engines + lobby board (arena × mode × difficulty × mutators) + attacker-identity threading for N-way kill credit + Horde creature spawning | ⬜ next |
| A4 | Arsenal: Static Net, Sonic Thumper, Prism Beam + pads + WeaponCharge wiring + bot weapon prefs | ⬜ |
| A5 | Progression: match stats, credits payout, unlock flags, daily seed | ⬜ |
| A5.5 | **Pre-round locker** (fff crossover, frozen API in `systems/QUARTERS.md`): `QuartersRoom` per arena spawn, round-timer exit gate, teleport-out on round start, equipped-cosmetic strings in the match handshake | ⬜ |
| B2 | Holo war table vs ConquestAI (Sandbox placement) | ⬜ |
| B3 | VR mission modifiers (ConquestMissionLibrary → real world contracts → odds mods) | ⬜ |
| B4 | Hotseat sync → (after A6) Photon live sync | ⬜ |
| A6 | Photon PUN2 online (needs Terry's PC) + polish | ⬜ |
| — | Close: HANDOFF, checklist, playbook rows per chunk, APK dispatch green | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** A2 (Arena Factory + 5 arenas) committed — verify CI. **A1's APK gate is
  ✅ GREEN** (run `28608110578`, full pipeline + artifact): the smart bot + 4 difficulty profiles are
  in a sideloadable build. Tests asmdef now references Ziptide.Editor (editor-only tests can validate
  the authoring libraries — ArenaLibraryTests uses it).
- **✅ THE ARENA APK IS GREEN** (run `28610940371` on `800ff25`): all five arenas authored, generated,
  **audit-clean**, and in the `ziptide-apk` artifact — after one failed dispatch whose blockers
  (Tidal spawn-in-cover, Void spawn-under-ramp, exit outside the wall) were diagnosed and fixed as
  pure layout data. **Everything shipped this session is now APK-verified end to end:** the smart bot
  (4 difficulty assets) in six fighting spaces, the four mode engines, N-player/team matches, and the
  Tidefront conquest sim. 248 tests green.
- **Next action after arena APK green:** **A3-scene** (mode director + lobby board + attacker
  identity: extend `IPvpDamageable.ReceiveHit` context or a hit-source registry so N-way kills credit
  correctly — design against the fff `PlayerIndex >= 0` law; Horde spawning via `CreatureVariantAuthor`
  defs + `HordeState.WaveCreatures`). Then A4 arsenal (Static Net / Sonic Thumper / Prism Beam runtimes
  + pads become timed respawners — `respawnSeconds` already in data; the Gun Game ladder names all six).
- **✅ fff BRIEFING ABSORBED (constraints for all remaining A-tasks):**
  1. Cross-fixes reviewed + accepted: `BotMath` uint-literal fix (CS0029, same values) and `BotBrain`
     same-tick HeardFire reaction (my test defined that contract — keeping it).
  2. **`PlayerIndex >= 0` LAW:** `CreatureRuntime` implements `IPvpDamageable` with `PlayerIndex = -1`.
     Bot targeting, match registration, hit aggregation, and netcode must filter `PlayerIndex >= 0` —
     never assume every damageable is a combatant.
  3. **A5.5 (new board task): the PvP pre-round locker** — `QuartersRoom` is host-agnostic + frozen API
     (`docs/systems/QUARTERS.md`): spawn one at each arena spawn, gate its exit on the round timer,
     teleport out on round start, sync `CosmeticLocker.GetEquipped` strings in the match handshake
     (string-pure by design for exactly this). Cosmetics are looks-never-stats — no balance review.
  4. A1c must NOT regress: `PvpBot`'s `CollideMove` wall-clamping + visible `PvpBolt` firing.
- **Next action:** verify CI on the A1b+A1c push, then **A2 (Arena Factory)**: `ArenaLayoutDefinition`
  (bounds/tiers/cover/waypoints/spawnPairs/objectiveZones/weaponPads/hazardMutators/themeSpec) +
  `ArenaLayoutLibrary` (5 arenas per PVP_ARENA_AAA §A2) + `ScenePatcherArena` (generalize ScenePatcherPvP
  — reuse its Cube/Ramp/nav helpers) + BuildAndroid hook. Then A3 (PvpMatch → N combatants + modes).
  NOTE for A1c device pass (runbook entry pending at close): watch `PVP_BOT_BRAIN difficulty=regular`,
  bot now patrols a ring, hunts your last position when you break LOS, ducks behind cover blocks when
  hit, and retreats at low HP. Difficulty = edit `Resources/Bots/*.asset`.
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
