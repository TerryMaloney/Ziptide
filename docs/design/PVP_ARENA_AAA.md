# PVP ARENA — the AAA-fun program (Program A of the Multiplayer Program)

**Terry's brief:** the current PvP is a tester — one 34m room, four cover boxes, a bot that's a
range-keeping turret. Make it **AAA fun**: real levels, CoD/Fortnite-class bots, multiple weapons, real
game modes, endlessly replayable. This doc is the production design; live progress is in
`docs/SPRINT_MULTIPLAYER.md`; the roadmap slot is `GAME_PLAN.md` M7a/M7b.

## Why this will work (verified foundations — don't rebuild these)
- **`IPvpDamageable`/`IScannable`** — every weapon already routes hits through one interface; new
  weapons and new combatants (bots, remote players, creatures) plug in with zero weapon edits.
- **Pure rules core** (`PvpRules/PvpMatch/PvpCombatant/WeaponCharge`) + **`IPvpTransport`** netcode seam
  — built and tested; the live loop currently bypasses them (we thread it in A1; then Photon drops in).
- **The world factory** — arenas are cheap: layout data → patcher → shipped scene, same as story worlds.
- **M2 hazards** (wind/static/flood/spore) and **M3 creatures** (4 archetypes) — free mode content.
- **One economy** — arena winnings pay the same `PlayerProfile` the story uses.

## The fun thesis
Fun in an arena shooter = **worthy opponents** (A1) × **spaces with decisions** (A2) × **variety of
goals** (A3) × **variety of tools** (A4) × **reasons to return** (A5). Ship them in that order — a
brilliant bot makes even the current room fun tomorrow; a giant map with a turret bot is still boring.

---

## A1 — THE BOT BRAIN (highest leverage; mostly pure C#)

### Architecture (mirrors the netcode split: pure decisions, scene translation)
- **`BotBrain`** (`Multiplayer/Runtime/Bots/BotBrain.cs`, pure C#, deterministic from a seed + injected
  clock): consumes a `BotPerception` snapshot each tick, emits a `BotDecision`. EditMode-tested hard.
  - **States:** `Patrol` (waypoint wander) → `Hunt` (move to last-known-position, then search ring) →
    `Engage` (strafe + fire cycle at optimal band) → `TakeCover` (break LOS when hit/reloading) →
    `Peek` (cover-edge exposure with a fire window) → `Reposition` (flank arc to a new angle) →
    `Retreat` (low-HP kite toward pickups/cover) → `Rush` (close-range aggression / hammer).
  - **`BotPerception`** (input struct): canSeeTarget, targetPos/velocity, lastKnownPos + age, distToTarget,
    incomingProjectile(pos/vel), myHealth, myCharge, nearestCoverBreakingLOS, atWaypoint, heardFireAt.
  - **`BotDecision`** (output struct): moveTarget, strafeDir, wantFire, wantDodge, faceDir, useWeapon.
- **`BotProfileDefinition`** (Content SO — **difficulty is DATA**):
  | Field | Rookie | Regular | Veteran | Nightmare |
  |---|---|---|---|---|
  | aimErrorDegrees | 8 | 5 | 2.5 | 1 |
  | reactionSeconds | 0.9 | 0.6 | 0.35 | 0.2 |
  | leadTargets (project velocity for slow bolts) | ✕ | ✕ | ✓ | ✓ |
  | dodgeOnThreat chance | 0 | 0.25 | 0.6 | 0.9 |
  | coverDiscipline (seek cover when hit) | 0.2 | 0.5 | 0.8 | 1.0 |
  | peekSeconds / hideSeconds | 1.5/2.5 | 1.2/2 | 0.9/1.2 | 0.7/0.8 |
  | retreatBelowHP | never | 2 | 2 | 3 (proactive) |
  | fireCooldown ×PvpRules | 1.5 | 1.2 | 1.0 | 0.8 |
- **Scene half (`PvpBot` rewrite, next chunk after the pure core):** builds `BotPerception` from the
  scene (LOS linecast, `PvpBolt`/dart proximity = incoming threat, cover-point query), executes
  `BotDecision` through the existing `CollideMove`; navigation over the **waypoint graph the arena
  patcher bakes** (~30 nodes: floor ring, platform, ramps, cover shadows; simple A* — NO NavMesh, stays
  in the patcher idiom); dodge = perpendicular burst-step; keeps the canon telegraph (color + audio cue
  scale with difficulty: Nightmare telegraphs shorter, never zero — kid-readable is LAW).
- **Seam work while we're in there:** thread `PvpMatchDirector`/`PvpPlayer`/`PvpBot` through
  **`IPvpTransport`** (LoopbackPvpTransport in solo) and gate firing with **`WeaponCharge`** — the two
  known gaps; after this, Photon (A6) is an adapter, not a refactor.

### Tests (the brain is provable without a headset)
Seeded scenarios: under-fire → TakeCover within reactionSeconds; LOS lost → Hunt to LKP then search;
HP low → Retreat; telegraph seen + dodge roll → strafeDir ⊥ threat; Nightmare hits a strafing target
(lead math) where Rookie misses; determinism (same seed+inputs → same decisions).

**Gate:** Nightmare beats a casual player in the CURRENT room; Rookie loses gracefully; every state readable.

## A2 — ARENAS WORTH FIGHTING IN (the Arena Factory)
- **`ArenaLayoutDefinition`** (Content SO): bounds/tiers, platform/ramp/cover lists, **coverPoints**,
  **waypointGraph** (nodes + edges), spawnPairs, objectiveZones (KotH hills, fragment base), weaponPads,
  breakableWalls, hazardMutators (reuse `HazardZoneDef`), themeSpec (reuse the sky/palette block).
- **`ArenaLayoutLibrary`** (Editor, create-only) — five launch arenas, each themed on a story biome and
  designed to FPS-map fundamentals (figure-8 flow, ~3 lanes, no single power sightline, flank route,
  verticality, 40–60m):
  1. **Arena_Cistern** — tight + dark, glowing light-shaft center hill, tunnels = hammer-wall shortcuts.
  2. **Arena_Chitinwall** — vertical alley maze, two-tier catwalks, drop ambush routes.
  3. **Arena_MirrorFlats** — long lanes + low cover, sniper-ish Prism Beam heaven, bright glare.
  4. **Arena_TidalArray** — split islands + bridges; the **flood cycle is live** (low ground periodically
     drains-or-drowns → the map breathes on a timer).
  5. **Arena_Void** — bridges over the fall-net, the Shell overhead, gravity-shove = environmental kill
     (respawn, non-lethal canon: you "fall out" and re-enter).
- **`ScenePatcherArena`** (generalizes ScenePatcherPvP): builds any layout + wires director/HUD/pads/
  walls/scanner; `BuildAndroid` hook like generated worlds. Lobby board (dev-menu pattern) selects
  arena × mode × difficulty × mutators.

**Gate:** 5 arenas ship in one build; bots navigate all (waypoint graphs validate in audit: every node
reachable, every coverPoint breaks ≥1 spawn sightline).

## A3 — GAME MODES (replayability engine; rules = pure C# + tests)
- **`PvpMatch` generalization:** N combatants (2–4) + optional teams; kill-feed events. Bots fill slots.
- **`PvpModeDefinition`** (data) + pure mode logic:
  | Mode | Rule | Win |
  |---|---|---|
  | **Deathmatch** (exists) | frags | first to 10 |
  | **Gun Game** | each kill advances your weapon down the ladder | finish the 6-weapon ladder |
  | **King of the Hill** | hold the active zone (rotates 45s) | 90 zone-seconds |
  | **Fragment Rush** | grab the fragment at mid, bank it at your base; carrier is locator-pinged | 3 banks |
  | **Horde** | survive waves of bots + **M3 creatures** (swarmers/crawlers/flyers/bruisers escalate) | high-wave score |
- Mutators per match: hazards on/off, low-gravity hop, double-charge, walls-regen-fast.

**Gate:** every mode completable vs bots; Horde wave 10 mixes bots + 3 creature archetypes.

## A4 — ARSENAL (map control)
Spawn with taser; the rest are **weapon pads** (respawn timers — map control is the meta):
| Weapon | Role | PvP stats (PvpRules additions) | Counter |
|---|---|---|---|
| Taser (spawn) | mid-range poke | 2 dmg, 2-charge | cover |
| Gravity (pad) | knockback + comfort-hop mobility | 1 dmg + shove | range |
| Pistol (pad) | fast light hitscan | 1 dmg, 3-charge | strafing |
| **Static Net** (pad) | thrown AoE slow zone | 0 dmg, 3s slow field | leave the zone |
| **Sonic Thumper** (pad) | melee AoE shove + wall-breaker | 2 dmg close, breaks bricks | keep distance |
| **Prism Beam** (pad) | hold-to-charge lane beam | 3 dmg, long telegraph glow | break LOS during charge |
All through `IPvpDamageable` + `WeaponCharge`; bots use them via `BotProfileDefinition` weapon prefs.

**Gate:** Gun Game runs the full 6-weapon ladder; every weapon has a visible counter.

## A5 — SESSION & PROGRESSION (the "endless" part)
Match stats (accuracy/streaks/K-D) + scoreboard · **credits payout into the live `PlayerProfile`**
(the arena funds your story economy — one wallet) · unlock ladder via flags (arenas → Veteran →
Nightmare → mutators) · **daily challenge seed** (deterministic arena/mode/mutator/bot combo; same for
everyone; bonus payout) · post-match "run it back / next arena / lobby" flow.

## A6 — ONLINE (PvP Phase 3/4 — trivially enabled by A1's transport threading)
Photon PUN2 import (**Terry's PC**) → `PhotonPvpTransport : IPvpTransport` → remote avatar (head + 2
hands + held weapon from `PlayerPoseMsg`) → room-code join → Quest Party Chat first, Photon Voice later
→ Phase-4 polish (rejoin, host migration decision, spawn-protection tune, sanity anti-cheat: host
validates hits). **Bots backfill empty slots** — the A1 brain IS the remote-player stand-in by design.

## Build order & verification
A1-core (pure, tests) → A1-scene (bot rewrite + seam threading; device-check) → A2 (factory + 2 arenas,
then 3 more) → A3 (match generalization + 2 modes, then rest) → A4 (weapons in pairs) → A5 → A6.
Every push CI-green; APK dispatch per chunk; Terry's device pass rates FUN (the only gate that counts).
Change-safety: every knob above is an SO field — playbook rows land with each chunk.
