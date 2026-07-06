# 💰 ECONOMY & META-PROGRESSION — 50 ideas (bank; read README first)

Current: one-economy spine (META-LOOP — credits/resources flow between jobs, mines, gardens, arena
winnings, Tidefront via ProfileEconomy + ProductionGraph + flow report), PlayerProfile save (flags/
resources/credits), Quarters CosmeticDefinition locker (looks-never-stats LAW), ship upgrade sockets
(designed), collectibles + Transmission fragments, daily-seed (designed), unlock flags. Bar: the pull
of a great roguelite meta — three goals visible at three horizons. **No gacha / real-money ever.**

| # | Idea | Size | Systems touched | 🎨 |
|---|------|------|-----------------|----|
| 1 | Three-horizon goal card on the wrist: one daily, one mid, one long objective read from flags + a `GoalService` pure core — the "always something next" HUD. | M | ProfileEconomy, flags, wrist UI | |
| 2 | Collection album in Quarters: per-world completion rings (POIs found, creatures logged, fragments) from save data — a physical book you flip through. | M | PlayerProfile, Quarters | 🎨 |
| 3 | Milestone contracts chaining across systems: "harvest 50 → the seed vendor unlocks → sell 10 hybrids → the greenhouse unlocks" as flag-gated JobDefinition chains. | M | JobDirector, flags, economy | |
| 4 | Faction reputation tracks: a per-faction rep float in the profile paying distinct cosmetics at thresholds — reputation you SEE on your gear. | M | PlayerProfile, CosmeticDefinition | 🎨 |
| 5 | Weekly mutator playlist: a seeded rotation of arena/world modifiers labeled "This Week", refreshed on a date hash — a reason to return weekly. | S | daily-seed, ArenaLobbyBoard | |
| 6 | Ship figurehead trophies from Tidefront victories displayed on a Quarters shelf — the war layer's meta payoff made physical. | M | Tidefront, Quarters, PlayerProfile | 🎨 |
| 7 | Barter vendor whose 3-slot stock rotates on the daily seed — a deterministic "check the shop today" loop. | M | economy, daily-seed, ItemFactory | |
| 8 | Prestige / New Game+ flags: completing the arc sets a `ng_plus` flag that raises world difficulty + unlocks a cosmetic tier, keeping the same save. | L | PlayerProfile, flags, WorldGating | |
| 9 | Credit ledger view: the flow report surfaced as a diegetic Quarters terminal showing where your credits came from this session (jobs/mines/arena). | S | ProfileEconomy flow report, Quarters | |
| 10 | Resource-to-credit exchange desk: a physical vendor converting surplus resources at a daily-seed rate — a sink that makes hoarding a choice. | S | EconomyState, daily-seed | |
| 11 | First-time-in-world bonus: a one-shot credit + RILL line the first time you enter each world, tracked by a `visited_<world>` flag — rewards exploration. | S | flags, ProfileEconomy, RILL | |
| 12 | Streak bonus: consecutive days played (real-time, profile-stored) grant an escalating daily credit stipend, capped — a gentle habit loop, no FOMO punishment. | S | PlayerProfile, ProfileEconomy | |
| 13 | Cosmetic unlock toasts: a framed "NEW SKIN" reveal in Quarters when a threshold flips, with the item spinning on a plinth. | S | CosmeticDefinition, Quarters | 🎨 |
| 14 | Bounty board escalation: completing a world's contract raises its future bounty tier (harder, pays more) — worlds you've mastered stay worth revisiting. | M | JobDirector, flags, economy | |
| 15 | Wallet HUD polish: a persistent, comfort-placed credit counter that ticks up with a coin-cascade sound on payout — the ka-ching the economy currently lacks. | S | wrist UI, AudioDirector | 🎨 |
| 16 | Achievement flags: ~40 named `ZiptideFlags` achievements (first hybrid, 100 tags, all worlds) surfaced in a Quarters case — completionist scaffolding. | M | ZiptideFlags, Quarters | 🎨 |
| 17 | Seasonal cosmetic drop: a dated batch of looks-only cosmetics that becomes claimable at a season boundary from the daily-seed clock. | M | CosmeticDefinition, daily-seed | 🎨 |
| 18 | Loadout presets in the locker: save/name 3 weapon+cosmetic loadouts, equipped by grabbing a labeled card — quality-of-life the arena loop wants. | M | Quarters locker, PlayerProfile | |
| 19 | Vendor haggle micro-game: a physical lever/dial that risks a worse price for a chance at a better one, seeded per interaction — a tiny VR skill on buying. | S | economy, Gameplay | |
| 20 | Fragment gallery: the Transmission fragments displayed as a decode wall in Quarters, filling in as you collect — the story meta made visual. | M | TransmissionProgress, Quarters | 🎨 |
| 21 | Ship upgrade tiers as the flagship sink: S3 scanner/engine/cargo sockets priced in the thousands, each a visible hull change — the endgame credit destination. | M | S3 sockets, ShipHullBuilder, economy | 🎨 |
| 22 | Garden-to-arena crossover: a cosmetic weapon wrap unlocked only by harvesting a rare hybrid — meta reasons to touch every system. | S | garden, CosmeticDefinition | |
| 23 | Daily challenge with a leaderboard-lite: the daily-seed run records your best result locally + a "beat yesterday" prompt — solo competition. | M | daily-seed, PlayerProfile | |
| 24 | Contract variety weights: JobDirector picks from a wider verb pool (escort, defend, deliver, survey) weighted by what you've done least — anti-repetition. | M | JobDirector | |
| 25 | Resource depots as a mid sink: build a home-world storage vault (BuildSocketRuntime) that passively appreciates stored resources — invest-and-wait. | M | BuildSocketRuntime, EconomyState, WorldState | |
| 26 | Cosmetic dye system: looks-only per-cosmetic color slots the player sets in the locker — massive expression from few assets, stats untouched. | M | CosmeticDefinition, Quarters | 🎨 |
| 27 | RILL commentary on milestones: she reacts to your 100th tag, first hybrid, last fragment — the meta gets a voice. | S | RILL lines, flags | |
| 28 | Trophy room progression: an empty Quarters wing that fills with earned dioramas (a downed Warden, a won war) as flags flip — visible mastery. | L | Quarters, flags, PlayerProfile | 🎨 |
| 29 | Weekly "big spend" vendor: a rotating high-cost premium cosmetic/ship-part that takes a week of play to afford — the long-horizon carrot. | M | economy, daily-seed | 🎨 |
| 30 | Job reputation with the guild: a track that unlocks better contract types + a guild livery, deepening the "contract tech" fantasy. | M | JobDirector, flags, CosmeticDefinition | |
| 31 | Collectible sets with a set bonus: cosmetic-only — completing a themed set unlocks a matching Quarters decoration (never a stat). | S | collectibles, CosmeticDefinition, Quarters | 🎨 |
| 32 | Idle economy dashboard: a Quarters terminal listing every mine/garden/socket across all worlds with time-to-full — manage the empire from home. | M | ProfileEconomy, WorldState, Quarters | |
| 33 | First-clear vs repeat payouts: contracts pay a big first-clear bonus then a smaller repeat rate — steers players toward new content without walling old. | S | JobDirector, flags, economy | |
| 34 | Cosmetic vending from arena medals: MP medals convert to a currency spent only on arena cosmetics — a self-contained MP meta loop. | M | MP medals, CosmeticDefinition | 🎨 |
| 35 | Photo-mode unlocks: frames/filters/stickers earned through play, applied to Quarters photo-wall shots — creative meta reward. | M | photo mode, CosmeticDefinition | 🎨 |
| 36 | Tidefront campaign rewards: winning a conquest season grants a unique ship paint + a title shown on your locker — the strategy layer's meta bridge. | M | Tidefront, CosmeticDefinition, Quarters | 🎨 |
| 37 | Resource rarity tiers surfaced in UI (common → rare → exotic color-coded) so the economy reads at a glance and rare drops feel special. | S | EconomyState, UI | 🎨 |
| 38 | Gift/trade shrine (single-player flavor): leave a resource offering at a world shrine for a seeded cosmetic-or-credit return next visit — a whimsical sink. | S | economy, daily-seed, POI | |
| 39 | Contract difficulty selector paying proportionally: pick Rookie→Nightmare on any job for a scaled payout — player-set risk/reward. | S | JobDirector, economy | |
| 40 | Meta tutorial nudges: the goal card gently points a new player at the next unlockable system ("try planting a seed") until they've touched each loop. | M | GoalService, flags | |
| 41 | Cosmetic loadout on the avatar: earned glove/chest-rig skins (avatar v1+) equipped in the locker — the player's own body as a reward canvas. | M | avatar, CosmeticDefinition, Quarters | 🎨 |
| 42 | Bank interest event: a weekly seeded market swing changing exchange rates ± so timing your sells matters — light economic texture. | S | EconomyState, daily-seed | |
| 43 | Completion percentage: a single headline "Ziptide 34%" number in Quarters aggregating all collection/flag progress — the completionist's north star. | M | PlayerProfile, flags | |
| 44 | Named world records: the profile stores your best per-world (fastest clear, most harvested) shown on a world-select card — personal history. | M | PlayerProfile, WorldGating | |
| 45 | Cosmetic crafting: spend surplus resources at a Quarters bench to craft (not buy) looks-only items — a resource sink with a creative payoff. | M | EconomyState, CosmeticDefinition, Quarters | 🎨 |
| 46 | The "living locker": Quarters visibly upgrades (lighting, furniture, size) as your total progress climbs — home reflects the journey. | L | Quarters, flags, PlayerProfile | 🎨 |
| 47 | Endgame prestige currency: a rare currency earned only post-arc, spent on NG+ exclusive cosmetics + the four-ending gallery. | L | PlayerProfile, flags, CosmeticDefinition | 🎨 |
| 48 | Cross-system daily rotation: the daily seed picks ONE system to spotlight (2× garden yield today, cheaper ship parts tomorrow) — rotates what you log in for. | M | daily-seed, ProfileEconomy | |
| 49 | Guild-hall hub world: a social/vendor world (via TravelCoordinator) housing every vendor, the album, the trophy room, and the war table — the meta's home base. | L | TravelCoordinator, Quarters systems, economy | 🎨 |
| 50 | Full progression arc doc + `ProgressionService`: a pure core defining the unlock DAG (what gates what) with tests, so the whole meta is one auditable, LLM-editable graph. | L | new pure core, flags, all systems | |
