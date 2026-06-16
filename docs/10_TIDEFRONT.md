# 10 — TIDEFRONT (galaxy strategy layer)

**Future metadata-first planet-control layer. Design-only. Not an immediate
multiplayer implementation — Tidefront stays design-only until the
content/registry architecture is stable.**

> Source brainstorm: `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/03_tidefront_multiplayer_plan.md`.
> This module is the curated summary; the source file has the full detail.

---

## Pitch

A holographic strategy map in the player's ship. Planets are nodes. Players
capture planets, gather resources, build defenses, build attack vessels, and
resolve attacks via **weighted probability**. Optional VR missions modify the
odds. This lets the universe feel large without rendering every planet conflict —
**a planet can matter strategically before it's built as a VR world.**

---

## Build order (do not skip ahead)

1. **Local Galaxy Sandbox** — no server. ~10 planets, 3 resources, neutral/enemy
   ownership, build-defense / build-vessel / attack-adjacent buttons, weighted
   resolution, ownership changes, local save.
2. **AI Opponents** — collect, build, attack weak neighbors, reinforce borders.
3. **Private Async Galaxy** — invite-only friends/family, async/timed turns,
   basic cloud persistence.
4. **Ranked League** — *only after private works.* Seasons, matchmaking,
   anti-ganging rules, rank scoring.

**Implementation guidance: metadata first.** Keep battle resolution
deterministic/testable from a seed — fits the existing pure-C# + EditMode-test
backbone (this is squarely Architect-lane data work when it starts).

---

## Core data (design-only — see [`docs/06_SCHEMAS.md`](06_SCHEMAS.md))

```text
PlanetNode
- planetId, displayName, biomeType, ownerId
- resourceType, resourceProductionRate
- defenseLevel, orbitalShieldLevel, stationedDefenseUnits
- specialTraitId, adjacentPlanetIds
- conflictState, instabilityLevel, bloomContaminationLevel
```

Suggested systems (separate strategy layer; keep off the Boot/world runtime):
`ConquestNode/PlanetNode`, `ConquestMap`, `ConquestState`, `ConquestRules`,
`ConquestResolver`, `ConquestAction`, `ConquestSaveData`, `ConquestAI`,
`ConquestPresentation`. Reuse existing `DefinitionRegistry<T>` / `SaveSystem`
where it fits rather than inventing parallel infra.

---

## Resources

Start with **three**; don't add currencies early.

| Resource | Use |
|----------|-----|
| **Flux** | energy, ship power, attack vessels |
| **Alloy** | structures, defenses, repairs |
| **Bloommatter** | risky biotech upgrades, special defenses, alien tools |

Possible later rare resource: **Gate Shards** (special travel, advanced tech).

---

## Battle resolution

Compare attack vs defense, roll weighted probability, optionally modified by VR
missions. **Clamp 10%–90%** so nothing is guaranteed (equal = 50%, each +1 ≈ +5%).

Outcomes are richer than win/lose: **Major Victory**, **Costly Victory**,
**Stalemate**, **Failed Attack**, **Counterstrike**.

- **Defenses:** Shield Spire, Drone Net, Gate Jammer, Decoy Beacon, Repair Swarm,
  Gravity Minefield, Resource Vault, Bloom Barrier (powerful but raises contamination).
- **Attack vessels:** Scout Skiff, Pulse Frigate, Shieldbreaker Barge, Gate Piercer,
  Siege Lantern, Drone Carrier, Null Ark (rare), Resource Harvester (non-attack).

---

## VR mission connection

Tidefront isn't just a menu — optional VR missions feed the strategy layer.
Attack-side (sabotage shield, scan grid, steal codes, plant beacon, disable
jammer, rescue drone) → reduce enemy defense / reveal value / +attack / better
odds / new route. Defense-side (repair tower, power grid, clear Bloom, recover
battery, shoot down drones) → +defense / reduce incoming / protect resources /
delay resolution.

Presentation lives on a **holographic table in the ship**: walk up, rotate/scale
the star map, tap a planet, drag a vessel from owned planet to target, confirm,
watch holographic ships resolve. Keep strategy inside VR.

---

## Multiplayer (far future)

- **Private Galaxy** — invite-only, custom rules, alliances, no serious ranking.
- **Public Ranked** — solo queue, no friend stacking, limited comms, seasons reset.
- **Squad Galaxy** — squads vs squads, separate ranking.

**Anti-ganging / balance:** no friend stacking in solo ranked, per-cycle attack
limits, dogpile defender bonus, adjacency rules, fog of war, no ranked trading,
same-target cooldown, bounty pressure. **Snowball control:** upkeep cost,
distance penalty, capture instability, underdog missions, season resets.

Ranks: Cadet → Navigator → Pathfinder → Vanguard → Star Captain → Gate Warden →
Architect. Score rewards holding/generating/attacking/defending/missions/underdog
wins; penalizes repeated failed and abandoned attacks.

---

## First testable checks (when prototyped)

- Attack odds clamp between 10% and 90%.
- Adjacent-only attack rule works.
- Resources deduct correctly; ownership changes on victory.
- Defense structures modify defense score.
- Dogpile shield activates after repeated attacks.

---

*Tidefront stays design-only until the content/registry architecture is stable.
Build it as metadata first; multiplayer is the last step, not the first.*
