# DLC — Worlds W069–W080 (post-launch)

**Status:** **Disabled in build settings until DLC launch** (stub `WorldPackDefinition` assets with
`isDLC=true, isBlockout=true`, per `MASTER_BUILD_PLAN` §12). These are **design seeds only** — deliberately
loose (per the Bible's "still open" list), to be fleshed once the base game ships and we know which ending
the community gravitates to.

**DLC premise hook (from the Bible):** the **dark fifth gate** at W063 + the fact that *the contained
universe is one of many*. The DLC opens a **second contained universe** (a new "season"/cycle), reachable
only after a base-game ending — letting other endings, other Cals, and the **Observers** themselves become
playable territory. Reuses every system; new biome/art kits + a new faction.

**Sequel / identity-layer hook (`THE_TRANSMISSION.md` §8):** Game 1 ends with Cal knowing who she is and
the gate open — her **partner** is on the other side, and so is the level *above* the Architects. The
sequel is **going through**, and there is a **second Transmission**: Cal recorded a new self-message before
crossing (it's what she does now). **W075 The Cal Archive = the Ouroboros made literal** — a vault of every
prior Debugger cycle; proof the self-wipe is a loop she's run before.

> Seeds are intentionally brief. Promote to full `_WORLD_TEMPLATE.md` READMEs when DLC is greenlit.

| World | Working name | Seed hook (biome · enemy · twist) |
|------|--------------|-----------------------------------|
| W069 | The Fifth Gate | Void · Pattern · the dark gate opens; bridge from a base ending into the new cycle |
| W070 | New Cycle Dawn | Exterior · none · a fresh contained universe booting — you arrive as the *new* RILL-class witness |
| W071 | The Observed | Station · Warden · the first world that *knows* it's watched from the start |
| W072 | Glass From The Other Side | Interior · none · played partly **outside** the Shell — in the lab |
| W073 | The Second Bloom | Forest · tendril · a Bloom that learned from the first universe's mistakes |
| W074 | Architects' Rivals | City · drone · the people the Architects worked *against* |
| W075 | The Cal Archive | Interior · none · a vault of every prior Cal across cycles |
| W076 | Pattern Born | Pattern · Pattern · a universe that started awake (the D-ending lineage) |
| W077 | The Wardens Who Left | Exterior · none · Wardens who chose ending B in a past cycle |
| W078 | The Knock | Void · all · meet (carefully) the thing the Shell kept *out* |
| W079 | Author's Room | Interior · none · the closest the game comes to the Observers' true faces |
| W080 | The Open Universe | Void · none · DLC finale — a universe that chose to stay awake *and* free |

**Build note:** DLC worlds inherit the same data pipeline (`CityLayoutDefinition`/`WorldPackDefinition`/
definitions); the only new code is whatever a "second cycle" framing needs (likely just new content + a
boot flag), keeping DLC mostly *data*, per the world-scaling pipeline goal.

*Catalog complete (W000–W080). Index: `storyboard/README.md`. Canonical table: `MASTER_BUILD_PLAN` §12.*
