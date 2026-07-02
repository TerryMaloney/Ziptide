# TIDEFRONT — the Risk-layer production design (Program B of the Multiplayer Program)

**Terry's brief:** the "Risk aspect" — build your attack fleet, build your defenses, take planets, play
it against another person, with **our story worlds as the map**. `10_TIDEFRONT.md` holds the approved
concept (keep it — it's the canon vision doc); THIS doc is the production design that takes it to
shippable. Live progress: `docs/SPRINT_MULTIPLAYER.md`; roadmap slot: `GAME_PLAN.md` M7c.
**Status change:** promoted from "parked/post-launch" to active build by Terry (2026-07-02). Only the
ranked-league layer stays post-launch.

## The shape of the game (big picture)
A **holographic war table** (in the Sandbox now; inside your ship after M4-S1). The galaxy IS the story
worlds — every world you've visited in the campaign is a planet node you can hold, develop, and fight
over. You gather three resources, build defenses on your planets and vessels for your fleet, and attack
adjacent planets. Battles resolve by **weighted probability with rich outcomes** — and the twist that
makes it OURS: before a battle resolves, you can **fly a real VR mission into that actual world** to
tilt the odds. Strategy feeds action; action feeds strategy. Play vs the AI, pass-the-headset vs a
friend, then live over the network.

## The sim (middle picture — pure C#, 100% CI-verifiable)
All under `Multiplayer/Runtime/Conquest/` (no Unity types; the proven PvpRules/PvpMatch pattern):

- **`PlanetNode`** — the 14-field spec from 10_TIDEFRONT verbatim: planetId, displayName, biomeType,
  ownerId, resourceType, resourceProductionRate, defenseLevel, orbitalShieldLevel,
  stationedDefenseUnits, specialTraitId, adjacentPlanetIds, conflictState, instabilityLevel,
  bloomContaminationLevel.
- **`ConquestState`** — the galaxy + per-player stockpiles + turn number. **Galaxy = the story worlds:**
  a builder maps each `WorldPackDefinition`/layout (W001–W012 today, auto-grows with M5 chapters) to a
  node — biome from the layout, resourceType from the world's key resource, adjacency from the chapter
  gating chain + a few cross-links so the map isn't a line. Resource mapping: **Flux** ← signal/energy
  worlds · **Alloy** ← mineral/salvage worlds · **Bloommatter** ← spore/bloom worlds.
- **`ConquestRules`** — all constants: production per turn, build costs, odds clamp **10–90%**
  (equal = 50%, each net point ≈ +5%), instability decay, contamination effects, dogpile-defender bonus,
  per-turn attack limit.
- **`ConquestResolver`** — deterministic from a seed: attack score (vessels + traits + mission mods) vs
  defense score (defenseLevel + shield + stationed units + structures + mods) → clamped odds → roll →
  outcome table **Major Victory / Costly Victory / Stalemate / Failed Attack / Counterstrike**, each with
  defined losses/ownership/instability effects. Same seed → same war, forever (this is also the async
  anti-cheat: both clients replay the same resolution).
- **Catalogs as Definitions** (data, tunable): **8 defenses** — Shield Spire, Drone Net, Gate Jammer,
  Decoy Beacon, Repair Swarm, Gravity Minefield, Resource Vault, Bloom Barrier (strong; raises
  contamination). **8 vessels** — Scout Skiff, Pulse Frigate, Shieldbreaker Barge, Gate Piercer, Siege
  Lantern, Drone Carrier, Null Ark (rare), Resource Harvester (non-attack). Each = cost + score
  contribution + one special rule flag the resolver honors.
- **`ConquestAI`** — the opponent (difficulty tiers as data, mirroring `BotProfileDefinition`): priority
  scoring over actions (collect → reinforce weakest border → build toward counter of player's fleet →
  attack weakest adjacent with ≥60% odds; Nightmare variant probes, feints, saves for Null Ark).
- **`ConquestSave`** — own section via the SaveSystem pattern; a full game state serializes to JSON.
- **`ITidefrontSync`** — the multiplayer seam (mirror of `IPvpTransport`): `SendAction(ConquestAction)`
  / `OnAction` / turn handshake. Implementations: **LocalHotseat** (pass the headset) →
  **PhotonTidefrontSync** (live session, reuses the A6 room) → cloud-async later.

### Test suite (the doc's own checklist + more, ~40 EditMode tests)
Odds clamp 10–90 · adjacency-only attacks · resources deduct + ownership flips on victory · every
defense/vessel changes the score it claims to · dogpile shield triggers after repeated attacks on one
target · counterstrike math · contamination tradeoff · determinism (seed → identical war) · AI plays a
full legal game headless · save round-trip.

## The table (little picture — the VR surface)
- **`ConquestTableRuntime`** (patcher-built primitives now, art kit at M6): a waist-high holo table.
  Planets = glowing orbs at layout positions (color = owner; size = development; slow orbit shimmer).
  Adjacency lines; fog-of-war dimming on unscouted nodes.
- **Interactions** (XRSimpleInteractable per element, dev-menu wiring pattern): tap planet → floating
  info card (owner/resources/defenses/traits) with build buttons · grab a vessel token from your fleet
  rack and **drop it on a target** → attack confirm panel shows live odds → commit.
- **The resolution moment** (the dopamine): holo fleets converge over the target, tension pulse, outcome
  stamp (MAJOR VICTORY in gold / COUNTERSTRIKE in red), planet recolors, RILL comments via its line
  system ("The Wardens will notice this."). Skippable after first view.
- **Turn flow vs AI:** your actions → END TURN → AI acts visibly (you watch its fleets move) → produce.

## The killer feature: VR mission modifiers (this is why it's OUR Risk)
When you commit an attack (or are attacked), the confirm panel offers **"Fly the mission"** — an
OPTIONAL contract INSIDE the actual target world (they're real, shipped worlds!):
- Attack missions (from the concept doc): sabotage the shield (`RepairableMachine` in reverse — a
  Disable step), scan the defense grid (wrist-scanner objectives), plant a beacon (Collect+Deliver) →
  each maps to EXISTING job steps; completion writes a `ConquestModifier` (e.g. −2 defense, +10% odds).
- Defense missions mirror: repair the tower (the M2 repair loop verbatim), clear the Bloom, shoot down
  scout drones (DisableDrones).
- Implementation: a `ConquestMissionLibrary` authors one attack + one defense job per world (the
  WorldJobLibrary pattern, Tidefront-flagged so they only appear in conquest context); travel there via
  the normal (gated) travel path; the mission result posts back to the pending resolution.
**Result: the strategy layer makes you REPLAY the campaign worlds with stakes — endless reuse of every
world we build.**

## Multiplayer staging (infra-honest)
1. **Hotseat** — pass the headset; `LocalHotseat` sync; ships with the table. Zero infra.
2. **Live match over Photon** — both players seated at their tables in a PUN2 room (A6's import);
   `ITidefrontSync` messages; the deterministic resolver means only ACTIONS sync. No persistence infra.
3. **True async** (turns while the other player is offline) — needs a small cloud store. **Decision gate
   for Terry when we reach it:** Unity Cloud Save vs PlayFab free tier vs keep-it-live-only. The seam
   isolates the choice; nothing before it depends on it.
4. **Ranked league / seasons / anti-gank / ranks** (Cadet→Architect) — the full 10_TIDEFRONT spec —
   **post-launch**, unchanged.

## Anti-snowball & fairness (baked into ConquestRules from day one, per the concept doc)
Upkeep cost per fleet size · distance penalty · capture instability (fresh conquests produce less +
revolt risk) · dogpile-defender bonus · per-turn attack limits · underdog missions (behind players get
better mission odds offers).

## Build order & gates
**B1 sim core + tests** (this sprint — pure C#) → **B2 table vs AI** (patcher + interactions; gate:
Terry finishes a 12-planet game seated) → **B3 mission modifiers** (gate: attacking W002 offers its
mission and the result shifts the roll) → **B4 hotseat → Photon live** (gate: two headsets finish a
game) → async decision → league post-launch.
Every knob is a Definition/Rules constant; playbook rows land per chunk; SPRINT_MULTIPLAYER carries
crash-resume state.
