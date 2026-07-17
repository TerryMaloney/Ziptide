# 📈 PROGRESSION & THE LONG GAME — power curve, upgrade trees, retention, family play

**Status:** DESIGN (Round 7 research synthesis). No code changed by this doc. Companion to
`WEAPON_FEEL_AND_ARSENAL.md` · `POWERS_MOBILITY_AND_EQUIP.md` · `SHIP_DESIGN_INTERIOR_EXTERIOR.md` ·
`ENEMIES_ENCOUNTERS_AND_BOSSES.md`. **This EXTENDS the LOCKED economy spine
(`docs/design/ZIPTIDE_META_LOOP.md`) — it does not fork it.** Every grant/spend described here routes
through `RewardRouter.Grant/TrySpend` + the ledger; every resource named here must be a
`ResourceDefinition`; all profile additions are additive `PlayerProfile`/`WorldState` fields with a
`schemaVersion` migration. There is NO parallel "progression currency" — combat salvage IS the
economy's salvage.

Grounded in Destiny, Hades, Deep Rock Galactic, Monster Hunter, Metroid/Metroidvanias, Splatoon,
Animal Crossing, Minecraft, No Man's Sky, Walkabout Mini Golf, Pokémon Snap, KTANE, Trackmania.

---

## 0. The core finding — progression is PERMISSION, not power

The games that stay fun for years (DRG, Monster Hunter, Splatoon) grow the player **sideways** —
new verbs, new tools, new places — and barely grow them upward. The games that burn out (Destiny's
light-level treadmill) inflate numbers and then inflate enemies to match, which cancels to zero and
feels like a hamster wheel.

**ZIPTIDE's rule: ~70% horizontal / 30% vertical.** New augments, new throwables, new ship modules,
new worlds, new creatures in the almanac = the long game. Damage/stat tiers exist but cap early
(~3 tiers per weapon). Difficulty escalates through **enemy affixes and composition** (the enemies
doc's data-only affix system), **never through HP inflation**.

The unifying atom (matching the other docs' atoms — salvage, the slot, the state machine):
**every unlock is a KEY.** The tide-drive is the macro key (opens a world cluster). An augment is a
micro key (opens an obstacle class — gecko walls, heavy-grade drones, deep water). Progression is
the rhythm of showing the player a lock, naming the key, and letting them earn it.

---

## 1. The power curve — logarithmic ramp, affix escalation

- **Shape:** fast early growth, flattening curve, 3–4 deliberate power SPIKES (the chapter-unlock
  hero augments from the powers doc §3). Between spikes, growth is horizontal.
- **Escalation without inflation:** later worlds field heavier weight/armor tiers, meaner affix
  combos, denser encounter compositions (enemies doc §3–4) — the same drone with `shielded` +
  `skittish` plays completely differently at zero balance cost. A returning player's old gear is
  never obsolete (Monster Hunter: the starter hammer still hunts).
- **The Destiny warning, on record:** if a future system ever needs "+5% damage per level" to feel
  like progress, the system is broken — cut it. Describe every upgrade by **what it lets you DO**,
  never by stat soup ("+12% coil efficiency" is banned; "your net can hold a Hauler" is the format).

---

## 2. Gating — lock → name → earn → payoff (the Metroidvania loop)

1. **Show the lock first.** The player bounces off a sealed heavy-grade door, a too-far ledge, a
   deep-water shaft — *before* the key exists in their world.
2. **Name the key.** RILL / the holo-bench names the exact gadget that solves it ("that's a job for
   a Gecko Grip"). Anticipation now has a target.
3. **Earn it** through the three acquisition lanes (powers doc §3): found (fast) / crafted from
   salvage (medium) / chapter challenge (slow, ceremonial).
4. **First use = the promise paid off** — return to the lock and open it. This beat is the
   progression system's dopamine surface; never let a key unlock silently in a menu.

**Cadence:** ONE headline unlock per session, telegraphed a session ahead (the always-visible
"next craft" card, §4). The **tide-drive** is the macro version — one per world-cluster, the ship
doc's special-slot module, the biggest lock/key pair in the game.

---

## 3. Upgrade trees — the 3-branch bloom

Per upgrade surface (see §3.1), the tree is a **bloom**: 3 branches × ~3 tiers, each tier a binary
either/or choice, capped by one **Overclock capstone** ("the Spark"). Small enough for a kid to hold
in their head; the either/or is where build identity lives (DRG proved slot scarcity + binary picks
beat sprawling trees).

- **Complexity gates with progress:** branch 1 opens with the item; branches 2–3 bloom as the item
  is USED. A new player sees one choice, not a wall.
- **Earn vs install (the two-key lock):** **using** an item earns tier choices (upgrade-through-use,
  free and celebratory — the item levels because YOU threw it a hundred times), **salvage installs**
  the chosen chip (a ledgered `RewardRouter.TrySpend`). Playing earns the right; salvage pays the cost.
- **Respec is free and physical:** pull the chip out of the socket at the holo-bench, snap in the
  other one. No respec tax — kids experiment, experimenting is the game.
- **Soft archetypes via named synergies (Hades Duo Boons):** no classes. Cross-item combos with
  NAMES — e.g. **"Comet Catch"** (Gravity Glove pull-tier + Surge Dash = catch mid-dash) — light up
  on the bench when both halves are installed. Archetypes (Gravity-Master, Gunslinger, Trapper)
  *emerge* from what you chose; the synergy names make the build feel authored.

### 3.1 The five upgrade surfaces (each owns a distinct verb)
| Surface | Verb | Identity anchor |
|---|---|---|
| **Gravity Glove** | pull/catch/throw | THE identity item — its bloom is the deepest |
| **Primary weapon** | disable at range | feel-tier + behavior chips (weapons doc) |
| **Throwable pouch** | area control | capacity + behavior chips |
| **Augment loadout** | the special moment | slot scarcity IS the tree (powers doc §5) |
| **Ship** | range + home | slots/modules (ship doc §4–6); tide-drive = macro key |

### 3.2 The diegetic bench — one interface for ALL of it
The ship's **holo-bench** (ship doc §7) generalizes: every upgrade is a **physical chip snapped into
a glowing socket** on a holo-model of the gear. Hover a chip → ghost-preview of what changes, shown
ON the gear (the net visibly widens). Pull it out → respec. No 2D skill-tree screen exists anywhere.

---

## 4. The session hook — no wasted sessions, visible next goal

- **The salvage ledger guarantee:** every session banks *something* — salvage persists (ledgered),
  use-XP persists, almanac entries persist. There is no failure state that voids a session (enemies
  doc: capture escapes cost opportunity, never inventory).
- **The "next craft" card:** the bench always shows the nearest affordable/announced unlock and its
  salvage bar — the anticipation meter the player carries between sessions ("almost enough for the
  Grapple…" — powers doc §0).
- **While-you-were-away (the Animal Crossing hook):** `IdleEngine` (already built — capped offline
  accrual) surfaces as a diegetic beat: return to the ship → a hopper of accumulated parts, ecology
  diffs (new nests, cross-bred plants via `EcologyDirector`/`PlantGenetics` elapsed-time math), RILL
  narrating what happened. **Growth-while-away + curiosity + no expiry + no loss.** "The world
  missed you," never "you missed the world."
- **"Today's Salvage Contract":** a date-hash rotating bonus objective through `RewardRouter` — a
  stamp, not a timer. No streak loss, no expiring unique rewards, no login guilt. (The predatory
  versions — Duolingo streak-guilt, pay-to-skip appointment timers — are banned by name.)

---

## 5. The long-tail engine — the Salvager's Almanac & the trophy shelf

Collection is the kid-retention engine (Pokédex, Critterpedia, Hollow Knight's Hunter's Journal):
a visible grid of silhouettes turns every unknown into a goal.

- **The Salvager's Almanac:** a **physical logbook prop** on the ship (Moss-style book beats
  floating UI in VR) — one silhouette page per creature/drone/item `*Definition` id. Filling an
  entry = disable + salvage it (the combat loop gets a collection payoff, Hunter's-Journal style).
  Entries are voiced by RILL in-character. Seasonal/biome availability gives gentle cross-month
  return reasons.
- **The trophy shelf is spatial and shareable:** salvage-becomes-decor on the ship's magnetic peg
  grid (`trophyMountPoints[]`, already designed). Grabbable, rearrangeable, scale-honest (a mounted
  drone chassis, not a 10cm figurine). The collection you can *walk a parent through* is social; a
  menu is not — this is the museum insight from Animal Crossing.
- **Photo mode feeds it:** a camera prop with a Pokémon-Snap-style almanac requirement —
  "photograph it intact before you salvage it" — collection + family sharing + marketing shots in
  one self-contained system.

---

## 6. Family co-op — the honest engineering ranking

Terry's kids play; family value per engineering dollar is the metric. Ranked:

1. **Pass-and-play + ghosts (build first — days of work, zero netcode).** Local in-game profiles
   (NOT Meta accounts — Quest is single-account-centric), per-profile scores, a hand-over flow, and
   **ghost replays** ("beat Maya's drone time" — a translucent recording of the sibling's arena
   run, Trackmania-style deterministic input replay on the pure `PvpModes` engines). Transforms a
   one-headset multi-kid household. Pure-C#-testable.
2. **Asymmetric couch companion (weeks — the KTANE model).** Quest casting + a spotter role: a
   local-network phone-browser page (WebSocket, no app store) showing the almanac/salvage manifest/
   drone weak points, with a "call out the target" button. One-directional data, no physics sync,
   no voice netcode (same room). Highest "family on the couch" magic after pass-and-play.
3. **Two-headset colocated (months — only if a second Quest exists).** Meta's Colocation Discovery
   + Shared Spatial Anchors solve *alignment only*, not netcode. A grab-everything physics game
   means per-rigidbody ownership transfer, throw authority handoff, contested-grab reconciliation —
   the classic VR netcode nightmare (Demeo/Walkabout got away with it because their physics is
   sparse). **If ever: scope to ONE sparse mode (the PvP arena, which already has the dormant
   Photon seam) — never the open salvage worlds.**
4. **Full online co-op — defer indefinitely.** Netcode + relay/matchmaking + voice + join-in-
   progress across `TravelCoordinator` loads + the holster-travel inventory contract + under-13
   account policy + a permanent testing tax on every future feature. A project-killer for a small
   team. Keep the seam warm: pure engines and interfaces stay netcode-shaped, ship nothing online.

- **Sibling gift-leaving (zero-infra social):** a mailbox prop on the ship; per-local-profile item
  transfer routed through `RewardRouter` (ledgered — no dupe exploits).
- **The cozy↔action rhythm is the family surface:** ship hub ↔ tide expedition already encodes
  Monster Hunter's Astera↔hunt / DRG's rig↔mission pattern. The cozy half (trophy mounting, almanac
  reading, ship decorating, pointless-on-purpose goofing) is what parents and kids share — make the
  return-through-the-gate exhale beat *mandatory-feeling*: gate → welcome-back summary → trophy
  mounting → RILL follow-up. All four systems exist.

---

## 7. Architecture mapping (additive on the LOCKED spine)

**Extends cleanly (fields/enums/data on existing systems):**
- **`AugmentDefinition`** += `branch` / `tier` / `prerequisiteEffectId` (the bloom tree as data).
- **`ItemDefinition`** += use-XP fields (`useXp`, `tierChoicesEarned`) — upgrade-through-use.
- **`PlayerProfile`** += almanac flags per definition id (`firstDisabledAtUnix`, `timesSalvaged`),
  local profile list, trophy placement per mount point — all additive + `schemaVersion` migration.
- **Salvage reconciliation (the law):** combat salvage is a `ResourceDefinition` granted via
  `RewardRouter.Grant` on capture/salvage and spent via `TrySpend` at the bench. The ultimate
  `ChargeMeter` (powers doc) SUBSCRIBES to those grant events — it never mints its own resource.
- **`IdleEngine` / `EcologyDirector` / `PlantGenetics`** — the while-you-were-away beat is a
  presenter over already-built elapsed-time systems.
- **`PvpModes` pure engines** — ghost replays are deterministic input recordings; no netcode.

**Genuinely new (small, isolated):**
- **`ProgressionGate` / `MilestoneDefinition`** ScriptableObjects — lock/key pairs as data (which
  obstacle class, which key id, which world cluster the tide-drive opens).
- **`SynergyDefinition`** ScriptableObject — named cross-item combos (the Duo-Boon layer).
- **`CollectionLog` service** (pure, Core) — almanac state over definition ids + the logbook prop.
- **Ghost recorder/replayer** for arena modes; **photo mode** (camera prop, render-to-texture,
  gallery save); **the "next craft" card** on the bench.

**⛔ Report-only (per CLAUDE.md):** local profiles touch save/persistence (get confirmation on the
profile model before build); anything the bench does to held items intersects inventory persistence;
casting/companion networking is new surface area — flag before building.

**Open gap (on record):** difficulty curve tuning (`difficultyTier` reading salvage-earned /
worlds-cleared) needs Terry's on-device feel pass before numbers are locked.

---

## 8. What to prototype first (recommendation)

1. **While-you-were-away beat** — surface `IdleEngine` + ecology diffs diegetically (hopper, RILL).
   Nearly all systems exist; the Animal Crossing hook for days of work.
2. **Salvager's Almanac + trophy persistence** — the long-tail engine; data-layer flags + the
   logbook prop.
3. **The bloom tree on ONE surface (the Gravity Glove)** — `branch/tier` fields + the chip-in-socket
   bench flow + one named synergy ("Comet Catch"). Prove the whole §3 loop on the identity item.
4. **Local profiles + pass-and-play ghosts** — the family multiplier for a one-headset household.
5. **The first lock/key pair** — one `ProgressionGate` (a heavy-grade door) + its named key + the
   first-use payoff beat. Prove §2's rhythm end-to-end.
