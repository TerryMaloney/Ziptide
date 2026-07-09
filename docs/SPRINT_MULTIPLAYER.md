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
| A3-scene | **MODES ARE PLAYABLE**: `PvpModeDirector` (scene body for all four engines: GunGame racks the player's next ladder weapon · KotH ticks patcher-baked `__PVP_ZONES` hills with bots CONTESTING via the new objective-magnet · FragmentRush v1 you-carry-bots-hunt · Horde waves spawn bots + runtime creatures w/ death-polling per the PlayerIndex law) + `ArenaLobbyBoard` (mode × difficulty × bot-count tiles at every arena spawn; arena select = the travel station now doors to EVERY sibling arena) + attacker identity (`PvpHitSource` same-frame report from every weapon → N-way kill credit, 1v1 fallback intact) + `PvpMatchDirector` generalized (N combatants, kill/end/restart events) + HUD mode line + 5 new tests | ✅ this commit (CI pending) |
| A4 | **THE ARSENAL SHIPPED**: `ArenaWeaponDefinition` (kind→runtime via ItemFactory) + **Static Net** (lobbed arc → `SlowZoneRuntime`; slows player via StunReceiver + bots via new `PvpBot.ApplySlow`) + **Sonic Thumper** (swing-triggered shockwave: damage+shove+breaks walls, HammerTool idiom) + **Prism Beam** (hold-to-charge with growing guide-line telegraph → heavy lane beam, cancel-free) + `PvpRules` damage table (net 1 / thumper 2 / prism 3, tested: lethal-eventually, never one-shot) + **pads are timed respawners** (`WeaponPadRuntime`, runtime-spawned) + `ArenaWeaponAuthor` (create-only → Resources/Items, build-hooked) + one role-fit new pad per arena + Gun Game runs the FULL 6-rung DefaultLadder + 5 tests. *(Deferred: bot weapon prefs — bots keep bolts; prefs land with A5 stats or on Terry's feel notes.)* | ✅ this commit (CI pending) |
| A4.5 | **Augments** — ✅ SHIPPED (abilities sprint, 2026-07-09): pure cores (AugmentClock/Loadout/Effects, 9 tests) + AugmentDefinition + the SIX launch augments live (dash/bubble/overclock/magnet/sure-step/sixth-sense, spec numbers) + belt-orb diegetic trigger + sandbox rack. Remaining: arena pads (create-only reseed) + Horde reward + bot symmetry at Veteran+ (MP100 15/55/56). | ✅ |
| A4.6 | **Dual-wield** (§3): 🟡 pure `SharedChargePool` BUILT+TESTED (both hands, one pool). Full wiring **BLOCKED on a real finding:** the PLAYER's guns don't consume `WeaponCharge` today (only bots do) — nothing to pool until player weapons adopt a charge. Design call for Terry: give player guns the 2-shot charge (then dual-wield drops in), or shelve. | 🟡 |
| A4.7 | **Locator v2** (§4): 🟡 pure layer SHIPPED (abilities sprint): tier presets (cd 60/45/30s, 8s afterglow from tier 2), afterglow fade curve, Sixth-Sense cd scale at ping time — tested. Remaining: gauntlet form + cylinder radar + crown blip scene work (Picasso's mesh, ART-4). | 🟡 |
| A6-prep | **Two-Quest online setup SHIPPED**: `docs/TWO_QUEST_SETUP.md` (Terry's steps 1–4 doable TODAY; step 5 = first smoke after A6) + `PvpNetHub` transport registry (loopback default) + the full Photon adapter/room-code launcher in `Assets/ZiptideNet/` (Assembly-CSharp, NO asmdef — sidesteps PUN2 asmdef surgery; 100% inert behind `ZIPTIDE_PHOTON`) + `Ziptide → Net → Enable/Disable Photon` menu | ✅ this commit (CI pending) |
| — | — **MP100 (2026-07-06): Terry ordered "a hundred improvements" — the full numbered board is `docs/design/MP100_BOARD.md`.** Wave 1 SHIPPED (rows below); waves 2–8 are the queue, each item claim-able individually. The rows A4.5/A4.6/A4.7/A5/A5.5/B2-B4/A6 below are now ALSO board items (51-58/48/71-75/59-70/68/76-85/86-92) — the board is the finer-grained view, this table stays the coarse one. — | — |
| MP100-W1 | **THE MELEE PAIR** — Breaker Blade (true contact melee, per-target debounce, cracks walls, the new 7th Gun Game rung) + Tide Pike (thrust line, reach identity) + PvpRules balance (6 new tests) + explicit CreatureRuntime melee cases + **bots get ears** (PvpNoise feeds the A1 brain's silent HeardFire hook; swings inside reach = dodge-triggering threat) + pads (Cistern/Chitinwall — runbook reseed queued) + starter-lineup blade in every story world + storyboard homes (W009/W010/W048, additive) | ✅ `5b8872a`+`dc88091` |
| A5 | Progression: match stats, credits payout, unlock flags, daily seed *(= MP100 items 59-70)* | ⬜ |
| A5.5 | **Pre-round locker** (fff crossover, frozen API in `systems/QUARTERS.md`): `QuartersRoom` per arena spawn, round-timer exit gate, teleport-out on round start, equipped-cosmetic strings in the match handshake | ⬜ |
| B2 | Holo war table vs ConquestAI (Sandbox placement) — ✅ v1 SHIPPED (T-Dog, 2026-07-09): tap-build-strike loop, live odds, resolution stamps, visible AI turn, RILL's "The Wardens will notice this." | ✅ |
| B3 | VR mission modifiers — ✅ v1 SHIPPED (T-Dog, 2026-07-09, Terry's "gulag"): STRIKE NOW / FLY THE MISSION on every strike (+10%, underdog +15%), sabotage pylons in the actual target world, LET IT RIDE / DEFEND when the rival hits yours (scout-drone shootdown), ConquestSession travel round-trip, walked-out = declined. v2 seam: space-flight defense variant once 3.1 ship weapons exist. | ✅ |
| B4 | Hotseat sync → (after A6) Photon live sync | ⬜ |
| A6 v1 | **ONLINE PRESENCE SHIPPED** (Picasso, Fable 5): PUN2 imported+committed by Terry (`21f117c`, CI-green WITH Photon) → `PvpNetHub.StartOnline/StopOnline` hooks + `ZiptideNet/NetBootstrap` installs the starter (behind `ZIPTIDE_PHOTON`) → `PvpOnlinePresence` broadcasts local head+2 hands @20Hz and renders every peer as a helmet+gloves avatar (amber, vs local teal) → `ArenaLobbyBoard` **GO ONLINE** tile + live NET status label (room `ZIP-001`, x/2 count). Transport-agnostic (loopback-safe); 4 new hub tests. Logs `NET_*`. | ✅ (CI pending) |
| A6.2 | **Combat sync** (next): local fire→`SendFire`, host-authoritative `SendHit`/`SendScore`, the remote avatar gets a networked hitbox (`IPvpDamageable`) so existing weapons just work; reconcile downs. The pose channel already proves the transport end-to-end. | ⬜ |
| A6.3 | Polish: IK'd body (not floating head+hands), Photon Voice, room-code entry UI (replace hardcoded `ZIP-001`), spectator | ⬜ |
| — | Close: HANDOFF, checklist, playbook rows per chunk, APK dispatch green | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step (2026-07-02, 3rd session):** A3-scene + A6-prep + **A4 arsenal** all committed —
  **verify CI on the A4 push, then dispatch a fresh APK** (arena scenes re-bake with zones/board/
  all-arena doors/new pads; ArenaWeaponAuthor seeds the three definitions at build). After that:
  **A4.5 Augments** per `design/ABILITIES_AND_ARSENAL.md` §2 (AugmentDefinition + pure AugmentEffects
  registry first — ⚙CI), then A4.6 dual-wield, A4.7 locator v2, A5 progression + A5.5 locker.
- **A3-scene design notes (don't re-derive):** attacker identity = `PvpHitSource` static same-frame
  report (interface unchanged — CreatureRuntime is story-lane); bots contest objectives via
  `PvpBot.hasObjective/objectivePoint` (Patrol magnet only — combat states untouched); Fragment v1 =
  only the player banks (bots guard/hunt — readable + fair); Horde reuses the arena bot as wave-bot #1
  (`ResetAt`), extra combatant indices 1–3, creatures polled for downs (PlayerIndex -1 never
  registers); racked weapons are never Destroy()ed while held (XR gotcha).
- **Earlier state:** A2 arenas + A1 smart bot APK-green (runs `28610940371` / `28608110578`).
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
- **📐 New Terry directives absorbed (2026-07-02, designed → board):** `docs/design/ABILITIES_AND_ARSENAL.md`
  adds A4.5 (Augments — the ability-item category, all modes), A4.6 (shared-charge-pool dual-wield),
  A4.7 (Locator v2 rework). Build order A4 → A4.5 → A4.6 → A4.7. The project-wide cross-track order now
  lives in **`docs/PRIORITIES.md`**. And: **the two-Quest online setup instructions are DUE at this
  sprint's close** (A6-prep row below) — Terry wants two headsets fighting; his setup steps run in
  parallel with our remaining code.
- **🎨 The art track is LIVE** (`SPRINT_ART.md`, operator "Picasso"): ART-1 skyscapes shipped APK-green —
  our five arenas got canon `SkyVista` skies via the theme seam, ZERO MP files touched (their announced
  appends: Tests asmdef / BuildAndroid hook / WorldAuditRunner line). Their lane = `Visuals/**` + art
  authoring/audit files; the verified seam map is `docs/ART_PLUG_POINTS.md`. The Locator-v2 gauntlet mesh
  is queued with them (ART-4).
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
