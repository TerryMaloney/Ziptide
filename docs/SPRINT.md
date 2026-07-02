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
| 0 | Archive M2; open this sprint | ✅ `99b2491` |
| 1 | Framework: `CreatureRuntime` (IShockable + **IPvpDamageable** — taser dart AND gravity gun hit creatures with ZERO weapon edits; non-lethal crumple disable; loot→profile; respawn) + `CreatureBehaviorBase` (shared CollideMove LAW, leash, touch-stun w/ cooldown, MakePart) + `CreatureZoneDef` + `CityBuilder.BuildCreatureZones`/`MakeCreature` (behavior attached by archetype at edit time) | ✅ this commit |
| 2 | Archetypes: `SwarmerBehavior` (boid cluster; bodies GATHER TIGHT as the dart telegraph) + `BruiserBehavior` (windup paw-the-ground telegraph → straight line charge → wall-slam skips to vulnerable Recover; stun interrupts mid-charge) + pure `ChargeState` + 5 tests | ✅ this commit |
| 3 | Archetypes: `WallCrawlerBehavior` (8-ray wall find, surface-stick + normal-align, ripple telegraph → drop-lunge, stun knocks it OFF the wall) + `FlyerBehavior` (figure-8 soar, dead-still hover telegraph w/ folded wings → head dive, pulls up at 1m, stun cancels to Climb) — factory switch complete for all 4 archetypes | ✅ this commit |
| 4 | `WardenBehavior` — the Shell's immune system: statue at tier 0 → WATCHES at tier 1 → WARNS when crowded at tier 2 (eye ramps red over the window) → PURSUES only if you stand ground, one lawful arrest-stun then disengages; backing off de-escalates; C6_WARDEN_ALLY = calm green. Pure `WardenState` + 6 tests. Factory: creatureId "warden" special-case | ✅ this commit |
| 5 | Novel behaviors (existing-gear counters): **Witness-mite** (moves only unobserved — pure gaze test), **Light-grazer** (grows in dark, shrinks in light), **Tether-swarm** (shared pool, cut the tether), **Husk-molter** (decoy husk on stun) — as `CreatureBehavior` subclasses/variants; drop to 2 if budget demands (document) | ⬜ |
| 6 | Author: W002 light-grazers in the dark cistern · W005 canopy swarmers · W009 tether-swarm + wall-crawlers on the chitin wall · W012 a dormant Warden at the gate (Signal tier 2 there — it WATCHES) + creature data assets in `CreatureVariantAuthor` | ⬜ |
| 7 | Close: HANDOFF (ccc), runbook §2e smoke, checklist, **APK dispatch green** → ✅ stamp | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** Tasks 1+2 committed (one compiling unit). KEY DESIGN FACTS: creatures are hit
  through the EXISTING weapon dispatch — the taser dart checks DroneRuntime → **IPvpDamageable** →
  IShockable, the gravity gun checks DroneRuntime → IPvpDamageable — so `CreatureRuntime` implements
  IShockable + IPvpDamageable (PlayerIndex = -1, never registered with PvpMatchDirector) and NO weapon
  file was touched. Behavior subclass is attached at EDIT time by `CityBuilder.MakeCreature` switching
  on the CreatureDefinition's archetype (AssetDatabase load from `Assets/Ziptide/Resources/Enemies/`);
  visuals/colliders built at RUNTIME in the behavior's Awake→BuildVisuals (gotcha #7); each behavior
  keeps ≥1 keepCollider part so weapons can hit it. WallCrawler/Flyer currently FALL BACK to Swarmer in
  the factory switch — replace when task 3 lands.
- **DONE (task 3):** WallCrawler + Flyer as specced below; factory switch handles all four archetypes.
- **Old task-3 spec (kept for reference):** `WallCrawlerBehavior` (raycast to the nearest wall within ~6m,
  stick to its surface — position on hit point + align to normal — crawl along it toward the player's
  wall-adjacent point; when player within ~3m, DROP + lunge once, then re-climb; telegraph = a ripple
  scale pulse before the drop) + `FlyerBehavior` (hover at home +4–6m, slow figure-8; when player in
  range: telegraph hover-pause → DIVE at the head (CollideMove-clamped), pull up at 1m, climb back;
  contact = base touch-stun). Both extend `CreatureBehaviorBase`, update the `MakeCreature` switch.
  Then Task 4 (Warden + pure WardenState tests) → 5 (novel: Witness-mite gaze-freeze w/ pure
  `IsObserved(headFwd, toCreature, cos)` test · Light-grazer dark-grow/light-shrink · Tether-swarm
  shared-pool + cuttable tether · Husk-molter decoy-on-stun; drop to 2 if budget, document) →
  6 (author zones: W002 light_grazer, W005 swarm_bug canopy, W009 tether_swarm + tendril crawlers,
  W012 warden at the gate; add creature defs to `CreatureVariantAuthor`) → 7 close + APK.
- **LANE NOTE (Terry 2026-07-02): architect is working PvP/multiplayer IN PARALLEL — `Gameplay/Runtime/Pvp/`, `Multiplayer/`, `ScenePatcherPvP`, `Net/` are ARCHITECT'S. Don't touch; only CONSUME IPvpDamageable; pull --rebase before every push.**
- **Branch:** `terry-local-wip`. CI-green through task 1-2 (`6ccc989` #191 pending at last check).

## Working rules (unchanged)
CI green per push; APK dispatch at close (`actions_run_trigger`, ci.yml, terry-local-wip). Creature
motion must be smooth, telegraphed, non-lethal; never yank the player camera (comfort rule).

---
*Opened 2026-07-02 by the operator (Fable 5) — GAME_PLAN M3.*
