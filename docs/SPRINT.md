# 🟡 ACTIVE SPRINT — M3: LIVING WORLDS (opened 2026-07-02)

> **Takeover prompt: "Read docs/SPRINT.md and continue."** Live state, updated every push. Roadmap:
> `docs/GAME_PLAN.md` (this = **M3**). Spec: `docs/systems/CREATURE_DESIGN.md` (framework rules: every
> creature = readable telegraph + existing-gear counter + non-lethal disable + evolution reason).
> Playbook: `HOW_TO_CHANGE_ANYTHING.md`. Prior sprints: `docs/sprints/` (M1+M2 closed, APKs green).

**Sprint goal:** worlds get inhabitants that aren't drones. The `CreatureBehavior` framework
(generalizing the proven drone seam), the 4 base archetypes, the Signal-reactive **Warden**, and the
first novel behaviors — authored into W002/W005/W009/W012 so the arc's biomes read alive. All ⚙CI;
collision-clean is LAW (CollideMove everywhere); no rig/PvP/XRI-sample edits.

---

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | Archive M2; open this sprint | 🟡 this commit |
| 1 | Framework: `CreatureRuntime` (health/IShockable/non-lethal disable/loot/respawn; loads `CreatureDefinition` by serialized id) + `CreatureBehaviorBase` (CollideMove, leash, player find, telegraph, contact-stun) + `CreatureZoneDef` on the layout + `CityBuilder.BuildCreatureZones` | ⬜ |
| 2 | Archetypes: `SwarmerBehavior` (boid cluster darts) + `BruiserBehavior` (telegraphed wall-slamming charge; pure `ChargeState` FSM + tests) | ⬜ |
| 3 | Archetypes: `WallCrawlerBehavior` (surface-stick, drop-lunge) + `FlyerBehavior` (hover high, dive at head, climb) | ⬜ |
| 4 | `WardenBehavior` — lawful enforcer (warn → pursue → disable, never ambush), escalates by `SignalState.Tier` (pure `WardenState` mapping + tests) | ⬜ |
| 5 | Novel behaviors (existing-gear counters): **Witness-mite** (moves only unobserved — pure gaze test), **Light-grazer** (grows in dark, shrinks in light), **Tether-swarm** (shared pool, cut the tether), **Husk-molter** (decoy husk on stun) — as `CreatureBehavior` subclasses/variants; drop to 2 if budget demands (document) | ⬜ |
| 6 | Author: W002 light-grazers in the dark cistern · W005 canopy swarmers · W009 tether-swarm + wall-crawlers on the chitin wall · W012 a dormant Warden at the gate (Signal tier 2 there — it WATCHES) + creature data assets in `CreatureVariantAuthor` | ⬜ |
| 7 | Close: HANDOFF (ccc), runbook §2e smoke, checklist, **APK dispatch green** → ✅ stamp | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** sprint opened (archives M2 → `docs/sprints/SPRINT_2026-07-02_M2_JOB.md`).
- **Next action:** Task 1. Files: `Gameplay/Runtime/Enemies/CreatureRuntime.cs` (serialized
  `creatureId`; Awake loads `Resources.Load<CreatureDefinition>("Enemies/"+id)`; health from def;
  `IShockable.Shock` → stun-tint + behavior pause; hits via existing weapon paths → `ReceiveHit`-style
  public `RegisterHit(point, taser)`; at 0 hp → DISABLE (non-lethal: crumple tint, behavior off,
  loot → profile via `ResourceCost` list, optional respawn), `ZIPTIDE: CREATURE_DOWN id=`) +
  `Gameplay/Runtime/Enemies/CreatureBehaviorBase.cs` (abstract; protected CollideMove copied from the
  proven DroneCombatBehavior SphereCast clamp ignoring creatures+rig; HomePos leash; player via
  `PlayerStunReceiver.Head`; `TouchStun(seconds, slow)` helper with per-contact cooldown; abstract
  `Tick(dt, playerDist, hasLoS)`), + `CreatureZoneDef` {id, center, radius, count, respawnDelay,
  creatureId} on `CityLayoutDefinition` + `CityBuilder.BuildCreatureZones/MakeCreature` (primitive body
  per archetype, CreatureRuntime.creatureId serialized — gotcha #7: only serialized fields cross into
  the scene; visuals in Awake).
- **Verified facts:** `CreatureDefinition` {archetype enum Swarmer/WallCrawler/Flyer/Bruiser, maxHealth,
  moveSpeed, damage, biomeId, loot(List<ResourceCost>), shockable} exists; `CreatureVariantAuthor`
  already authors `swarm_bug` (Swarmer) + `tendril` (WallCrawler) into `Resources/Enemies`; add new ids
  there (create-only). `SignalState.Tier(profile)` 0–4 (M1). `PlayerStunReceiver.ApplyStun` = non-lethal
  player hit. TextMesh only; .meta with uuid4; collider before grab (not relevant — creatures aren't
  grabbable); CollideMove is LAW.
- **Branch:** `terry-local-wip`. CI-green head: `d13656a`.

## Working rules (unchanged)
CI green per push; APK dispatch at close (`actions_run_trigger`, ci.yml, terry-local-wip). Creature
motion must be smooth, telegraphed, non-lethal; never yank the player camera (comfort rule).

---
*Opened 2026-07-02 by the operator (Fable 5) — GAME_PLAN M3.*
