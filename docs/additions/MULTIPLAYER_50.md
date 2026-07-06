# 🎮 MULTIPLAYER & ARENA — the Fortnite bar — 50 ideas (bank; read README first)

Current: 5 arenas, match board (mode × difficulty × 1-3 bots), 5 modes (Deathmatch/GunGame/KotH/
FragmentRush/Horde), smart BotBrain, respawning weapon pads, breakable walls, attacker-identity kill
credit, Quarters pre-round locker (designed), Photon seam dormant, Tidefront separate. Bar: Fortnite-
grade session pull. (destruction v2 in flight elsewhere — not duplicated.)

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Supply drop pod: PvpModeDirector timer picks a random arena POI, fires a sky beacon + 10s countdown, then a pod lands holding a top-ladder weapon; BotBrain treats it as a contestable objective. | M | PvpModeDirector, ArenaLayoutDefinition, WeaponPadRuntime, BotBrain | |
| 2 | Storm-tide variant: ride the live-flood hazard to shrink the arena in announced rings, pushing everyone to high ground for a "last dry rooftop" finale. | M | flood hazard, PvpModeDirector, ArenaLayoutDefinition, PvpHud | |
| 3 | Comeback surge: pure PvpModes rule (tests first) — the last-place player's dash/special charge regens 1.5× faster, killstreak-free rubber-banding. | S | PvpModes engines, PvpModeDirector | |
| 4 | Kill feed on PvpHud fed by attacker-identity: weapon icon + who splashed whom, so every elimination reads as a beat. | S | PvpHud, attacker-identity | |
| 5 | Announcer stingers via AudioDirector on PvpModeDirector events ("First splash!", "Match point!", "Zone moving!") — one-shot, low mix cost. | S | AudioDirector, PvpModeDirector | |
| 6 | Rematch lever: at match end ArenaLobbyBoard spawns a physical REQUEUE / NEXT ARENA pull-handle at the podium — one grab, never a menu. | S | ArenaLobbyBoard, PvpMatchDirector | |
| 7 | Bot personalities: 4 named bots (Rusty, Zippy, Tank, Sparks) mapping to per-personality BotBrain weight presets + short voice barks. | M | BotBrain, difficulty profiles, AudioDirector | |
| 8 | Round-based Gun Game: pure-engine rounds (best-of-3) with a 20s intermission teleporting players into the Quarters locker to pick a taunt/trail. | M | GunGame engine, PvpMatchDirector, Quarters locker | |
| 9 | Daily challenge seed: date-hash on ArenaLobbyBoard picks arena + mode + one modifier, labeled "Today's Match" with a completion stamp. | S | ArenaLobbyBoard, PvpMatchDirector | |
| 10 | Spectator drone after elimination: two fixed comfort-safe orbit cameras the downed player swaps between until respawn. | M | PvpModeDirector, PlayerRigPersistence, PvpHud | |
| 11 | Emote wheel on the off-hand controller: 6 kid-friendly emotes with Photon-seam-ready sync stubs so they work day one of two-Quest play. | M | input actions, Photon seam, Quarters cosmetics | 🎨 |
| 12 | Bounty beacon: when a player leads by 3+, PvpModeDirector pins a glowing marker over them and their tags are worth double; bots re-weight targeting. | S | PvpModeDirector, PvpModes, BotBrain, PvpHud | |
| 13 | Weapon pad frenzy minute: mid-match event dropping WeaponPadRuntime respawn timers to zero for 60s, announced by a pad-color shift + stinger. | S | WeaponPadRuntime, PvpModeDirector, AudioDirector | |
| 14 | Final-minute double points: pure PvpModes multiplier in the last 60s with a HUD pulse — close matches stay winnable to the buzzer. | S | PvpModes engines, PvpHud | |
| 15 | Medal toasts: post-tag popups ("Comeback Kid", "Wallbreaker", "Zone Boss") computed pure from match events, surfaced on PvpHud + the end card. | S | PvpModes, PvpHud, attacker-identity | |
| 16 | Mercy governor: BotBrain silently steps difficulty down one notch if the player is down 5+, back up when they rally — invisible dynamic difficulty. | S | BotBrain difficulties, PvpMatchDirector | |
| 17 | KotH zone telegraph: 5s before rotation, a light pillar rises at the next zone from arena data so the sprint-to-contest race becomes the heartbeat. | S | KingOfTheHill engine, ArenaLayoutDefinition, PvpHud | |
| 18 | Match stats card: podium hologram of accuracy, favorite weapon, longest hold, nemesis bot — a screenshot-worthy end beat. | S | PvpMatchDirector, PvpHud, attacker-identity | |
| 19 | Secret cache: one breakable wall per arena hides a stocked weapon pad, marked by a faint crack decal — rewards arena knowledge like Fortnite loot spots. | S | breakable walls, WeaponPadRuntime, ArenaLayoutDefinition | |
| 20 | Mutator dial on ArenaLobbyBoard: one optional twist per match (low-grav pads, fast floods, tiny-heads) as pure mode-config flags with tests. | S | ArenaLobbyBoard, PvpModes, PvpModeDirector | |
| 21 | Bot grudge memory: BotBrain remembers who last tagged it and briefly biases target toward revenge — rivalries become legible. | S | BotBrain states | |
| 22 | Floating nameplates with 3-pip health over bots and (Photon-ready) players, distance-faded for Quest fill-rate — instant fight readability. | S | PvpHud, BotBrain, Photon seam | |
| 23 | Team Deathmatch 2v2: pure team-scoring engine (tests first), one bot squads with the player, using spawn markers split into two sides. | M | new pure engine, PvpModeDirector, BotBrain, SpawnMarkerRuntime | |
| 24 | Last Splash Standing: elimination mode (pure engine) — no respawns, downed players ride the spectator drone, shrinking flood forces the endgame. | M | new pure engine, flood hazard, spectator drone, PvpMatchDirector | |
| 25 | Tag You're It: infection mode where tagged players join the "soaked" team; pure engine on attacker-identity, ends when one dry player remains. | M | new pure engine, attacker-identity, PvpModeDirector | |
| 26 | Juggernaut: one combatant gets triple health + heavy weapon; everyone scores by tagging them; crown passes on takedown — pure engine reusing kill-credit. | M | new pure engine, PvpModes, BotBrain, PvpHud | |
| 27 | Boss bot event: mid-match an oversized Horde creature spawns at a POI beacon; the final-tag player gets a score burst — reuses the Horde spawner in PvP. | M | Horde spawner, PvpModeDirector, BotBrain | |
| 28 | Bot squad tactics: leader/follower pairing where one suppresses while the other flanks, as two coordinated BotBrain states + a shared blackboard. | M | BotBrain states, PvpBot | |
| 29 | Moving hill: KotH variant where the zone glides along a spline between arena waypoints instead of teleporting — a payload-lite running fight. | M | KingOfTheHill engine, ArenaLayoutDefinition | |
| 30 | Double Fragment Rush: pure-engine mutator spawning two fragments so matches fork into split-push decisions; tests cover simultaneous-carry scoring. | S | FragmentRush engine | |
| 31 | Vault event: a timed vault door at a random POI unlocks mid-match with a horde-of-one guardian; inside, a one-per-match super-soaker pad. | M | PvpModeDirector, breakable walls, WeaponPadRuntime, Horde spawner | |
| 32 | Career rank: persistent XP from match results via the save layer, shown as a rank badge on your Quarters locker door + lobby nameplate. | M | PvpMatchDirector, persistence, Quarters locker | |
| 33 | Featured playlist rotation: ArenaLobbyBoard highlights a weekly "Featured" tile (arena+mode+mutator from a seed table) with bonus XP — Fortnite LTMs. | M | ArenaLobbyBoard, PvpMatchDirector, daily-seed | |
| 34 | Podium ceremony: top-3 (bots included) posed on a rising podium with a confetti burst and their chosen emote, camera-safe, skippable via the rematch lever. | M | PvpMatchDirector, emote wheel, Quarters cosmetics | 🎨 |
| 35 | Night-ops arena variants: each arena gets a dusk/night sky + emissive-lamp lighting preset selectable on the board — same geometry, new mood. | M | ArenaLayoutDefinition skies, ScenePatcherArena, VisualThemeProfile | 🎨 |
| 36 | Crowd drones: 4-6 spectator drones orbiting the arena rim that cheer (chirp + spin) on multi-tags and captures, off PvpModeDirector events. | M | PvpModeDirector, AudioDirector, drone assets | 🎨 |
| 37 | Ready-check totem: ArenaLobbyBoard gains a two-slot ready pedestal built against the Photon seam interfaces now, so two-Quest lobbies need zero rework later. | M | ArenaLobbyBoard, Photon seam | |
| 38 | Hit-confirm juice: distinct haptic + rising-pitch chime per Gun Game rung on tags, and a heavier thud for takedowns — pure feedback, no balance change. | S | PvpHud, AudioDirector, haptics | |
| 39 | Winning-moment freeze: on match point PvpMatchDirector captures the final tag as a framed photo displayed at the podium and pinned in Quarters. | M | PvpMatchDirector, Quarters locker, PvpHud | 🎨 |
| 40 | Jump pads and ziplines as ArenaLayoutDefinition placeable entries with comfort-tuned arcs (fixed velocity, vignette); bots path them via BotBrain hints. | M | ArenaLayoutDefinition, comfort locomotion, BotBrain | |
| 41 | Nemesis intro: at match start the HUD calls out your rival ("Rusty wants a rematch!") from last match's attacker-identity history — a per-session storyline. | S | attacker-identity, PvpHud, save | |
| 42 | Bot chatter radio: light bark lines on state transitions (spotted, retreating, contesting) — kid-readable telegraphs that double as tactical info. | S | BotBrain states, AudioDirector | |
| 43 | Overtime rule: pure PvpModes tie-breaker — sudden-death 60s with all pads locked to the top weapon and one center zone; tests cover every mode's tie path. | S | PvpModes engines, WeaponPadRuntime | |
| 44 | Score-chase HUD: a slim "gap to leader" ribbon (+2/-3) pulsing on lead changes so second place knows what one more tag buys. | S | PvpHud, PvpModes | |
| 45 | Payload escort mode: pure engine (tests first) pushing a hover-cart along a spline while bots defend checkpoints; reuses moving-hill spline + Horde waves. | L | new pure engine, arena splines, BotBrain, Horde spawner | |
| 46 | Splashdown Royale: 8-bot battle-royale-lite — loot-only pads, no respawns, storm-flood rings, spectator drone on elimination — the flagship "one more match" mode. | L | new pure engine, flood hazard, WeaponPadRuntime, BotBrain, spectator drone | |
| 47 | Sixth arena "Rooftops": a vertical skyline layout built entirely from ArenaLayoutDefinition + ScenePatcherArena, around ziplines, wind gusts, a rising-flood finale. | L | ArenaLayoutDefinition, ScenePatcherArena, hazards | 🎨 |
| 48 | Locker progression: unlockable weapon wraps, trails, podium emotes earned from medals/rank, browsed and equipped physically in the Quarters locker. | L | Quarters locker, career rank, save, item registry | 🎨 |
| 49 | Tournament night: a 3-match bracket (mode rotates, difficulty escalates, mutators stack) tracked on ArenaLobbyBoard with a trophy for the Quarters shelf — a 20-min arc. | L | ArenaLobbyBoard, PvpMatchDirector, career rank, Quarters | |
| 50 | Hazard director: per-arena scheduled event tables in ArenaLayoutDefinition (flood surge at 2:00, pad frenzy at 4:00, boss at 5:00) — every arena a signature rhythm. | L | ArenaLayoutDefinition, PvpModeDirector, all mid-match events | |
