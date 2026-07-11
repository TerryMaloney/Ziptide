# CREATURE BEHAVIOR-COUNT GATE LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“what’s next”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — behavior richness gate in progress

## Goal

Close `CURRENT_EXECUTION_CHECKLIST.md` §7 item 5 and `EXCELLENCE_MAP.md`’s creature-behavior gap:
**every shipped `CreatureDefinition` must map to at least three distinct readable active behavior states.**

## Exact scope

New, non-visual files only:

- `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs` + `.meta`
- `docs/design/CREATURE_BEHAVIOR_READABILITY.md`
- this log

Closure docs after green:

- `docs/CURRENT_EXECUTION_CHECKLIST.md`
- `docs/EXCELLENCE_MAP.md`

## Gate shape

The catalog is quality metadata, **not a second behavior owner**. `CityBuilder.MakeCreature` and the existing
`CreatureBehaviorBase` subclasses remain the only runtime wiring/motion path.

For every `CreatureDefinition` asset under `Resources/Enemies`, EditMode CI will require:

1. a readability profile keyed by exact creature id;
2. at least 3 non-empty, distinct **active** state ids;
3. a telegraph state that belongs to that active-state set;
4. a counter/vulnerability state that belongs to that active-state set;
5. a separate non-empty non-lethal disabled/resolved state;
6. expected archetype matches the authored asset;
7. named behavior class exists in the existing `Ziptide.Gameplay` assembly;
8. every catalog profile is backed by a shipped asset, preventing stale invented entries.

Initial roster is the seven assets already authored by `CreatureVariantAuthor` / `CreatureBaselines`:
`swarm_bug`, `tendril`, `light_grazer`, `witness_mite`, `tether_swarm`, `husk_molter`, `warden`.

## Collision / stop rules

- Do not edit Picasso `Visuals/**`, Forge, creature meshes, materials, animation, passports, or art audits.
- Do not edit behavior MonoBehaviours, `CreatureRuntime`, `CityBuilder`, creature assets, scenes or prefabs.
- Do not claim runtime richness that is not evidenced by the existing M3 behavior implementation.
- This task ships as one small catalog/test gate; real behavior expansion remains a separately device-gated task.
- Three CI reds triggers the circuit breaker.
