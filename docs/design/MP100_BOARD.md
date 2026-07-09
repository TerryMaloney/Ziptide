# MP100 — THE HUNDRED (multiplayer/arena/PvP to tip-top)

**Terry's directive (2026-07-06):** "look at our multiplayer mode/arena/pvp... This needs to be tip
top. I want a hundred improvements." Plus: melee weapons, additive, in BOTH PvP and the regular game.

**What this board is:** 100 numbered, concrete improvements, each one small enough to be a
commit-or-two, sourced from a full-code survey (what's actually built vs. gaps), the three design
docs (`PVP_ARENA_AAA.md` / `TIDEFRONT_AAA.md` / `ABILITIES_AND_ARSENAL.md` — much of this was
already SPECED, never built), and fresh design where the specs run out. Statuses: ✅ shipped (this
pass) · 🔷 speced in a design doc (build against the quoted spec) · ⬜ designed here (this line is
the spec). Any operator can pick any unblocked row; claim in `SPRINT_MULTIPLAYER.md` first.

**Wave 1 shipped with this board (11 items ✅ below):** the melee pair + bot ears + placement.

---

## A. Bots (1–15) — from "range-keeper with a brain" to "reads like a player"
| # | Improvement | Status |
|---|---|---|
| 1 | **Bots get ears** — `PvpNoise` static channel; melee swings/thumps report, bot perception feeds the A1 brain's `HeardFire` hook (shipped silent since A1) | ✅ `5b8872a` |
| 2 | **Melee threat awareness** — a fresh swing-noise inside melee reach counts as `IncomingThreat` (dodge-triggering); the dart-only scan could never see it | ✅ `5b8872a` |
| 3 | Generalize the threat scan beyond `TaserDartProjectile` — thrown nets and charging prisms should register as dodgeable threats | ⬜ |
| 4 | Bots react to the Prism's guide-line telegraph — break LOS during the enemy's charge like a human would | ⬜ |
| 5 | **Weapon-pad seeking** — low-priority Patrol magnet toward the nearest armed pad; bots stop being bolt-only (`SPRINT_MULTIPLAYER` A4 explicitly deferred this) | ⬜ |
| 6 | Bots USE the arsenal — per-weapon bot fire behavior (net lob at LKP, thumper rush inside RushRange, prism from perches) | ⬜ |
| 7 | Bots climb the Gun Game ladder too — their kills advance their rung; the racked weapon swap is cosmetic for them (fire profile changes) | ⬜ |
| 8 | Multi-bot LKP sharing — one bot spots you, its squad hears about it (`HeardFire` re-broadcast through PvpNoise on sighting) | ⬜ |
| 9 | Strafe-accuracy tradeoff — a bot's own strafe adds to its aim error (they currently strafe with no accuracy cost; humans can't) | ⬜ |
| 10 | Spawn-protection awareness — bots don't waste charge on a protected respawner | ⬜ |
| 11 | Per-arena patrol personality — Mirror Flats bots hold perches, Cistern bots hug tunnels (per-arena waypoint weights in `ArenaLayoutDefinition`) | ⬜ |
| 12 | Horde wave-boss variant — every 5th wave, one bot spawns with Nightmare profile + a visible crown tint | ⬜ |
| 13 | Nightmare dual-wield (cosmetic pairing, cadence unchanged) — 🔷 `ABILITIES_AND_ARSENAL.md` §3 quotes it exactly | 🔷 |
| 14 | Bot kill-feed personality — 4 short name pools per difficulty ("RK-7", "VET-BRANDT"...) so the feed reads like a lobby, not an index | ⬜ |
| 15 | Bots use augments at Veteran+ (fairness = symmetry — the §2 balance law) once #41–45 land | 🔷 |

## B. Arenas (16–30) — variety the current five don't have
| # | Improvement | Status |
|---|---|---|
| 16 | **Melee pads placed** — Cistern breaker_blade (tunnel flank), Chitinwall tide_pike (catwalk) | ✅ `dc88091` |
| 17 | Multi-zone KotH — every arena gets 2–3 `objectiveZones` so `KothState`'s rotation logic (built, tested, never exercised) actually rotates | ⬜ |
| 18 | Second arsenal pad per arena — every arena currently has exactly one non-starter pad; give each a second, role-fit choice | ⬜ |
| 19 | Hazards beyond Tidal — only 1 of 5 arenas has a live hazard; add one biome-fit hazard each (Cistern: rising dark-water pools; Void: gravity flux zones) | ⬜ |
| 20 | Arena #6 "The Locker" — small/intimate (24×24) close-quarters box for melee/thumper brawls; all five current arenas cluster 40–60m | ⬜ |
| 21 | Arena #7 "The Sprawl" — large (80×80) with vehicle-less long rotations; prism country, teaches the locator | ⬜ |
| 22 | Arena #8 "The Stack" — true multi-story (3 floors + drop shafts); nothing today goes above one elevated tier | ⬜ |
| 23 | A moving platform element (patcher-baked mover on a spline, comfort-safe: never under the player's feet without a rail) | ⬜ |
| 24 | Match mutators wired to the lobby board — low-grav / double-charge / walls-regen-fast exist in the A3 design (🔷 PVP_ARENA_AAA), zero UI today | 🔷 |
| 25 | Night/alt-sky variants per arena via a second `SkyVistaDefinition` — cheap variety, the vista system already supports it | ⬜ |
| 26 | Anti-camp spawn logic — respawn picks the spawn-pair point FARTHER from the killer's position | ⬜ |
| 27 | Void's fall-out flow polish — the "environmental kill" respawn should have its own feedback beat (fall VO + score event), non-lethal canon | 🔷 |
| 28 | Tidal flood TELEGRAPHS — a rising-water klaxon + shoreline glow 3s before the flood cycle (the map breathes; players should hear it inhale) | ⬜ |
| 29 | Per-arena music stinger set via `AudioDirector` profiles (match start / last-kill / overtime) | ⬜ |
| 30 | Lobby board arena PREVIEW — a palm-sized diorama of the selected arena's geometry above the board (the data to build it is the layout asset itself) | ⬜ |

## C. Modes (31–40)
| # | Improvement | Status |
|---|---|---|
| 31 | **Gun Game melee finish** — ladder extended to 7 rungs, breaker_blade last (the classic) | ✅ `5b8872a` |
| 32 | Fragment Rush symmetric — bots can pick up/carry/bank (v1 explicitly shipped player-only; the state machine already supports any index) | ⬜ |
| 33 | KotH zone-rotation live once #17 lands (engine's done; it's an arena-data gap) | ⬜ |
| 34 | New mode: JUGGERNAUT — one over-healthed player (or bot) vs everyone; kills as the juggernaut score double; killing it makes you it | ⬜ |
| 35 | New mode: BLADE BRAWL — melee-only FFA (pads disabled, everyone spawns with a blade); the melee pack's showcase mode | ⬜ |
| 36 | Horde biome-matched waves — waves draw creature types from the ARENA's biome (swarm city sends swarms, void sends Pattern) instead of one global cycle | ⬜ |
| 37 | Horde between-wave shop — spend wave-clear credits at a pedestal (ammo charge refill / augment reroll / heal) during the 10s intermission | ⬜ |
| 38 | Round-intro countdown — 3-2-1-GO with spawn-door visual + a RILL announcer line (speaker field already supports "ANNOUNCER") | ⬜ |
| 39 | Post-match podium — winner's spawn platform lifts, scoreboard hologram, run-it-back / next-arena / lobby buttons (🔷 A5 flow speced) | 🔷 |
| 40 | Mutator + mode + arena as one shareable "card" — the daily-seed format (#54) doubles as a custom-match code | ⬜ |

## D. Arsenal & melee (41 already ✅ ×4, 41–50)
| # | Improvement | Status |
|---|---|---|
| 41 | **BREAKER BLADE** — true contact melee (per-target debounce, sustained-pressure identity, cracks walls) | ✅ `5b8872a` |
| 42 | **TIDE PIKE** — thrust melee (first body on the line, reach identity, real poke-back) | ✅ `5b8872a` |
| 43 | **Creatures take melee properly** — explicit per-weapon `CreatureRuntime` cases (story-game melee vs bruisers/crawlers/swarms) | ✅ `5b8872a` |
| 44 | **Story homes** — blade: W009/W048 (+ starter lineup everywhere); pike: W010's drowned fishing rigs | ✅ `dc88091` |
| 45 | Melee swing trail — a short ribbon behind the blade tip while above swing speed (readability = the telegraph law applied to the player) | ⬜ |
| 46 | Per-weapon haptic signatures — distinct grip pulse per weapon on fire/swing/hit (one table, one helper, all runtimes) | ⬜ |
| 47 | Energy Shield Disc — block projectiles on the forearm, throwable boomerang (the idea bank's only defense item; melee's natural counter-partner) | 🔷 |
| 48 | Dual-wield `DualWieldCoordinator` — shared charge pool, heavy weapons `twoHandedOnly` with haptic-buzz refusal | 🟡 pure `SharedChargePool` built+tested; full wiring BLOCKED: player guns don't consume WeaponCharge today (only bots do) — nothing to pool until they adopt one |
| 49 | Arc Rifle (chains to a 2nd target) — the design doc's named "Next" weapon | 🔷 |
| 50 | Weapon pad polish — respawn timer ring on the pad + floating item name label (characterSize×fontSize lesson applies) | ⬜ |

## E. Augments (51–58) — 🔷 the whole system is speced in ABILITIES_AND_ARSENAL §2
| # | Improvement | Status |
|---|---|---|
| 51 | `AugmentDefinition : ItemDefinition` + pure `AugmentEffects` registry + slot runtime (1 active + 1 passive — "slot scarcity IS the balance") | ✅ abilities sprint |
| 52 | Surge Dash (active, 8s cd — the gravity-hop burst any direction) | ✅ abilities sprint |
| 53 | Bubble Guard (active, 2s projectile shield, 20s cd) + Overclock (active, 4s faster recharge, 30s cd) | ✅ abilities sprint (overclock = weapon-cooldown scale — player guns carry no charge to recharge; see A4.6 note) |
| 54 | Magnet Palm (passive, 3m pickup pull) + Sure Step (passive, hazard slows −50%) + Sixth Sense (passive, locator cd halved + threat ping) | ✅ abilities sprint (threat-ping half queued) |
| 55 | Augment pads in arenas (pick-up-on-touch, drop-on-death = map control) | 🔷 |
| 56 | Horde wave-clear augment reward choice (pick 1 of 2 at wave milestones) | 🔷 |
| 57 | Story placement — one augment per chapter tied to biome/creature (collectible-style) | 🔷 |
| 58 | Quarters augment display shelf (the collection is the trophy case) | 🔷 |

## F. Progression & session (59–70) — 🔷 A5 speced, zero built
| # | Improvement | Status |
|---|---|---|
| 59 | Match stats — accuracy / best streak / K-D tracked per match (pure `MatchStats` core first, tested) | 🔷 |
| 60 | Credits payout into the live `PlayerProfile` — single-wallet economy with story mode (the RewardRouter chokepoint pattern already exists) | 🔷 |
| 61 | Unlock ladder via flags — arenas → Veteran → Nightmare → mutators | 🔷 |
| 62 | **Daily challenge seed** — deterministic arena/mode/mutator/bot combo, same for everyone, bonus payout | 🔷 |
| 63 | Scoreboard hologram — live K-D board at the lobby + end-of-match summary | ⬜ |
| 64 | Career stats board in Quarters (total kills, favorite weapon, nemesis difficulty) | ⬜ |
| 65 | First-win-of-the-day bonus (pairs with #62) | ⬜ |
| 66 | Per-mode personal-best board ("your best Horde wave: 12") | ⬜ |
| 67 | Cosmetic drops from matches — feed the existing `CosmeticLocker` (string-pure, already shipped) | ⬜ |
| 68 | **A5.5 pre-round locker** — `QuartersRoom` at arena spawn, round-timer exit gate, cosmetic strings in the handshake (the fff crossover, frozen API) | 🔷 |
| 69 | Match history — last 10 results in the profile (feeds #64/#66) | ⬜ |
| 70 | "Rival" system — the bot difficulty that's beaten you most gets a nameplate + a payout bounty | ⬜ |

## G. Locator v2 (71–75) — 🔷 fully speced in ABILITIES_AND_ARSENAL §4
| # | Improvement | Status |
|---|---|---|
| 71 | `LocatorState` pure extension — afterglow window + tier params (range 20/30/40m, cd 60/45/30s), tested first | ✅ abilities sprint |
| 72 | 8s afterglow trail — through-wall silhouette fade on tagged enemies | 🔷 |
| 73 | Cylinder radar — blips get elevation; enemy blips pulse with distance | 🔷 |
| 74 | Fragment-carrier crown blip + scan-kind colors (enemy red / objective gold / loot cyan / node green) | 🔷 |
| 75 | Tier upgrades via the ship's S3 scanner slot — "the first upgrade-socket payoff" | 🔷 |

## H. Tidefront (76–85) — 🔷 the sim is built+tested; everything visible is not
| # | Improvement | Status |
|---|---|---|
| 76 | `ConquestTableRuntime` — the waist-high holo war table (patcher primitives now, art kit at M6) | 🔷 |
| 77 | Planet orbs — color=owner, size=development, slow orbit shimmer; adjacency lines; fog-of-war dim | 🔷 |
| 78 | Tap-planet info card with build buttons (XRSimpleInteractable per element, dev-menu wiring pattern) | 🔷 |
| 79 | Grab-a-vessel-token → drop on target → live-odds confirm → commit (the core interaction) | 🔷 |
| 80 | THE RESOLUTION MOMENT — fleets converge, tension pulse, outcome stamp ("MAJOR VICTORY" gold / "COUNTERSTRIKE" red), planet recolors, RILL comments | 🔷 |
| 81 | Visible AI turn — you watch its fleets move (never a silent state jump) | 🔷 |
| 82 | `ConquestMissionLibrary` — fly-the-mission VR contracts (attack: sabotage/scan/beacon; defense: repair/clear/shoot-down) writing `ConquestModifier`s (−2 def, +10% odds) | 🔷 |
| 83 | Hotseat pass-the-headset sync (`LocalHotseat`, zero infra) | 🔷 |
| 84 | Campaign save/resume (JsonUtility round-trip already tested in the sim — needs the save slot + resume flow) | ⬜ |
| 85 | `ITidefrontSync` seam + Photon action sync (actions-only — the deterministic resolver means nothing else syncs) | 🔷 |

## I. Netcode & online (86–92)
| # | Improvement | Status |
|---|---|---|
| 86 | Thread the LIVE match loop through `IPvpTransport` — the seam exists but the shipped loop bypasses it (the A1c re-scope debt; blocks everything online) | 🔷 |
| 87 | PUN2 import + first two-Quest smoke (Terry's PC; `TWO_QUEST_SETUP.md` steps 1–4 are already written) | 🔷 |
| 88 | Remote avatar — head + hands + held weapon from `PlayerPoseMsg` | 🔷 |
| 89 | Room-code join UI on the lobby board | 🔷 |
| 90 | Host-validates-hits sanity anti-cheat | 🔷 |
| 91 | Bots backfill empty online slots (design intent, not just fallback) | 🔷 |
| 92 | Rejoin + host-migration decision (explicitly a decide-then-build item — don't improvise it mid-match) | 🔷 |
| — | *Note: melee online = `FireMsg`/`HitMsg` already carry weapon ids; the melee pair needs a swing event msg — fold into #86.* | |

## J. Feel, UX & polish (93–100)
| # | Improvement | Status |
|---|---|---|
| 93 | Kill feed — 3-line fading TextMesh feed on the HUD ("YOU downed VET-BRANDT — Tide Pike") | ⬜ |
| 94 | Damage-direction indicator — a brief rim tick toward the hit source (comfort-safe: HUD-space, never a screen flash) | ⬜ |
| 95 | Hit-marker haptics — short grip pulse on a CONFIRMED hit (pairs with #46's per-weapon signatures) | ⬜ |
| 96 | Death→respawn flow — brief drift-up spectate + fade instead of the hard cut (never yank the camera — drift is opt-in comfort-tested) | ⬜ |
| 97 | Comfort audit of every shove effect (thumper/pike/gravity knockback on the PLAYER) against the comfort canon — document max displacement | ⬜ |
| 98 | Announcer lines via the RillLine pipeline (speaker="ANNOUNCER") — mode intro, overtime, match point; reuses the whole subtitle/VO stub system | ⬜ |
| 99 | Scoreboard + rules panel theming per arena (palette from the layout asset — the data's already there) | ⬜ |
| 100 | **The MP smoke checklist** — a 10-minute headset script covering every mode × 2 arenas + melee + locator, added to `quest_smoke.ps1` docs; the "tip-top" gate is that this list stays green | ⬜ |

---

## Suggested build order (waves)
1. **Wave 1 — ✅ SHIPPED with this board:** melee pair + bot ears + placement (items 1, 2, 16, 31, 41, 42, 43, 44 + runbook/docs).
2. **Wave 2 — bots feel alive:** 3, 4, 5, 6, 9, 26 (all bot/spawn logic, mostly pure-testable).
3. **Wave 3 — modes get deep:** 17, 32, 33, 35, 38, 93, 94, 95 (arena data + mode polish).
4. **Wave 4 — augments:** 51–58 (one system, speced end-to-end; pure registry first).
5. **Wave 5 — progression:** 59–70 (pure stats core → payout wiring → boards).
6. **Wave 6 — locator + new arenas:** 71–75, 20, 21, 22.
7. **Wave 7 — Tidefront visible:** 76–85.
8. **Wave 8 — online:** 86–92 (gated on Terry's PC for PUN2).
Every wave ends with an APK dispatch + a `TERRY_RUNBOOK.md` device row. The circuit breaker
(3 CI-reds on one task → stop and escalate) applies per item.
