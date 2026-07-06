# ⚔️ TIDEFRONT — the conquest strategy layer — 50 ideas (bank; read README first)

Current: all pure C# + tested — PlanetNode (14 fields) over story worlds, ConquestState (economy),
ConquestRules (odds 10-90%), ConquestResolver (5 outcomes, 8 defenses + 8 vessels w/ specials),
ConquestAI (3 profiles), save round-trip, VR mission modifiers designed. B2 holo war table NOT built.
Bar: Risk meets FTL texture, played for its own sake. **Pure-core-first: mechanics = resolver/state
extensions + tests BEFORE any table UI.**

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Doctrine cards: at war start each side picks 1 of 6 doctrines (Blitz, Fortress, Raider…) applying flat resolver biases — a ConquestRules modifier struct + Resolver hook, seeded AI pick. | M | Rules, Resolver, State, AI | |
| 2 | Espionage probes: spend production to reveal a planet's defenses + garrison for 3 turns; a fog-of-war flag on the PlanetNode snapshot the player sees. | M | State, PlanetNode, Resolver | |
| 3 | Supply lines: attacks beyond friendly adjacency (BFS over ConquestGalaxy) cost +50% and -10% odds — the W001-W012 chain shape matters. | M | Galaxy, Rules, Resolver | |
| 4 | Planet traits from story biomes: each world gets 1-2 traits in the catalog (e.g. "Reef: minefields +1 charge", "City Grid: +1 production") applied in resolver + income. | M | Catalog, PlanetNode, Resolver, State | |
| 5 | Wartime events deck: a seeded 20-card deck fires one event per N turns (solar storm halves production, smuggler convoy grants a free vessel) — deterministic draw order. | M | State, Resolver, Rules | |
| 6 | Commander vessels: 4 unique flagships granting a co-attack aura (+5% odds, reroll one loss), captured — never destroyed — on defeat. | M | Catalog, Resolver, State | |
| 7 | Armistice state: after heavy losses the AI offers a fixed-turn ceasefire with tribute; breaking it early gives an "oathbreaker" -10% penalty for 5 turns. | M | AI, State, Rules | |
| 8 | Campaign seasons: every 20 turns the galaxy mutates deterministically — one link closes, one opens, one trait rotates — logged as a season chronicle. | L | Galaxy, State, Catalog | |
| 9 | Garrison veterancy: defenses that survive an attack gain a rank (+2%/rank, cap 3) — planets get a defended history, rewarding holding ground. | S | State, Resolver | |
| 10 | Attack feints: declare a feint (half cost, can't capture) that forces the AI to commit reserves, lowering that planet's real-attack odds next turn — a new outcome type. | M | Resolver, Rules, AI | |
| 11 | Repair-swarm rework: swarms may be stationed as convoys on links, healing adjacent garrisons each tick but vulnerable to interception. | M | Catalog, Resolver, State | |
| 12 | Null-ark counterplay: gate jammers on a planet cancel an incoming null ark's special — an explicit rock-paper-scissors row in the specials matrix + tests. | S | Catalog, Resolver | |
| 13 | Tiered victory: besides total control, add Economic (production lead 10 turns) and Diplomatic (all AIs in armistice) victories checked end-of-turn. | M | State, Rules | |
| 14 | War exhaustion: each attack raises exhaustion; past a threshold upkeep rises and odds drop 1%/point — mechanically ending forever-wars. | S | State, Rules | |
| 15 | VR mission modifier expansion: losing the in-world contract applies a concrete -10% and marks a "botched raid" that feeds the events deck. | S | Resolver, State | |
| 16 | Salvage economy: every battle yields salvage proportional to committed force (both sides), spendable only on defenses — keeps losers in the game. | S | State, Resolver | |
| 17 | Dogpile telegraph: the dogpile special announces one turn ahead (visible in probe intel), letting the defender pre-buy — a readable threat. | S | Catalog, Resolver, State | |
| 18 | Chokepoint bonuses: planets with exactly 2 links get an innate +5% defense "bastion" trait, computed from topology so map mutations re-derive it. | S | Galaxy, Rules | |
| 19 | AI reads the map: extend the 3 profiles with a topology weight (turtle values chokepoints, raider values high-production leaves) so profiles feel spatially distinct. | M | AI, Galaxy | |
| 20 | Blockades: park vessels on an enemy link to halve production and block reinforcements; a blockade-runner vessel special counters it. | M | State, Catalog, Resolver | |
| 21 | Deterministic battle chronicle: Resolver emits a structured event log (who fired, which special, roll values) replayable from seed — foundation for the table replay + golden tests. | M | Resolver, State | |
| 22 | Tribute demands: demand tribute from a weaker AI instead of attacking; acceptance odds from relative strength, refusal raises tension. | S | Rules, AI, State | |
| 23 | Minefield sweepers: a cheap vessel special spending its action to remove one minefield charge pre-combat — a pre-battle phase in the pipeline. | S | Catalog, Resolver | |
| 24 | Two-front penalty: defending 2+ planets in one turn takes -5% each after the first — coordinated multi-planet offensives become real strategy. | S | Resolver, Rules | |
| 25 | Homeworld designation: each start planet is a capital (+2 production, +10% defense); losing it (secured, not destroyed) triggers a government-in-exile debuff until recaptured. | S | PlanetNode, State, Rules | |
| 26 | Refit docks: convert a vessel type into another for 50% of the price difference + one turn docked — early-war fleet mistakes are correctable. | S | State, Catalog | |
| 27 | Intel decay: probe data older than 3 turns shows "stale" with widening uncertainty on garrison counts, all deterministic. | S | State, PlanetNode | |
| 28 | Casus belli: attacking without a grievance costs +1 attack limit; grievances tracked per faction pair in state. | M | State, Rules, AI | |
| 29 | Seasonal leaderboard seed: a weekly fixed-seed campaign so all players face the identical deterministic AI war and compare turn counts. | S | State, Resolver | |
| 30 | Shieldbreaker overcharge: fire at double effect but the vessel is disabled (captured if the attack fails) — a risk knob in the specials table + matrix. | S | Catalog, Resolver | |
| 31 | Rebellion pressure: planets secured this war accrue unrest if under-garrisoned; at threshold they flip to NEUTRAL (not the old owner) — a hold-what-you-take economy. | M | State, Rules, PlanetNode | |
| 32 | Neutral factions: 2-3 unowned planets start with independent garrisons; player and AI can court them with tribute or take them by force. | M | State, AI, Catalog | |
| 33 | Convoy raids as VR contracts: blockading a link offers an optional boarding contract inside the adjacent world; success upgrades the blockade to total. | M | Resolver, State, Rules | 🎨 |
| 34 | Doctrine drift: mid-war, a steep cost swaps doctrine once with a 3-turn "reorganization" debuff — tests cover the transition turn. | S | Rules, State | |
| 35 | Alarm levels: consecutive attacks on one planet raise its alarm (defender +3%/level), decaying over quiet turns — anti-spam pressure. | S | State, Resolver | |
| 36 | Ghost-fleet gambit: decoy vessels cost 25%, count for deterrence in AI threat evaluation, but evaporate on first contact; probes reveal them. | M | Catalog, AI, Resolver | |
| 37 | Link weather: each galaxy link gets a seeded weather cycle (calm/storm/rift) modifying travel cost + odds, forecast visible 2 turns ahead. | M | Galaxy, Rules, State | |
| 38 | Last-stand outcome: when a capital would fully fall, a 6th resolver outcome "heroic stand" (~10%, seeded) preserves one veteran defense — drama without randomness leaks. | S | Resolver, Catalog | |
| 39 | Production specialization: assign each planet a focus (ships/defense/intel) with a switch cooldown, layering build-order decisions onto the income tick. | M | State, PlanetNode, Rules | |
| 40 | AI intent telegraphs: the AI emits deterministic strings ("Vex masses ships near W004") derived from its actual next-action weights — honest tells for the future table. | S | AI, State | |
| 41 | Campaign chronicle export: serialize the full war (battles, events, seasons) into a compact save blob — "war so far" recaps + regression fixtures. | S | State, Resolver | |
| 42 | Mercenary market: a rotating seeded pool of 3 hireable one-battle vessels priced by demand; the AI bids too, so denial-buying is a play. | M | Catalog, State, AI | |
| 43 | Terrain-linked defenses: biome traits unlock unique defenses (Reef sells extra minefields, Foundry discounts repair swarms) — catalog availability keyed to traits. | S | Catalog, PlanetNode | |
| 44 | Escalation ladder: attack limits start at 1 and grow each season, so early war is positional and late war is total — one constant table + tests. | S | Rules, State | |
| 45 | Prisoner exchange: captured commanders/vessels enter an exchange pool; armistice can swap them, and refusing raises the AI's aggression. | M | State, AI, Catalog | |
| 46 | Full B2 holo war table: a galaxy hologram over the table, grab-a-planet to inspect, physically slam a fleet token onto a target to attack, doctrine cards as tangible slabs. | L | Table UI, Galaxy, State | 🎨 |
| 47 | Battle-resolution diorama: after each outcome, the table plays a 10s seeded miniature skirmish above the target driven entirely by the battle chronicle log. | L | Table UI, Resolver | 🎨 |
| 48 | Espionage as VR stealth: launching a probe optionally opens an infiltration mission inside that world; success upgrades stale intel to perfect for 5 turns. | L | Resolver, State, Rules | 🎨 |
| 49 | Asymmetric start "Uprising": player starts with 1 planet vs an AI holding 8, with rebellion pressure + neutrals tilted player-ward — a pure setup permutation. | L | State, AI, Galaxy | |
| 50 | Grand Admiral mode: three AI profiles fight each other AND the player in a 4-faction free-for-all with shifting armistices, leader-targeting dogpiles, per-faction doctrines — the full Risk fantasy. | L | AI, State, Rules, Resolver | |
