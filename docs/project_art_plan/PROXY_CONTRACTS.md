# PROXY CONTRACTS — "no dumb greybox": what every asset type MUST carry from its first proxy

**The law:** a proxy is a production contract, not trash. It is playable NOW (correct scale, sockets,
colliders, stable id, story refs) and upgrades to final art behind the same id without touching a
consumer. Enforced by `ForgeRecipeDefinition.Validate()` + `FORGE_*` audit blockers, not by review.

## Required per asset type (storyTag → contract)

| Tag | Required sockets/markers | Scale class | Also required |
|---|---|---|---|
| `handheld` | `Grip` (localEuler.x 40–50° — the Quest controller tilt) + `Muzzle` forward of grip | ≤0.35m | charge/emission slot (a `GlowPanel` palette slot) for readability |
| `creature` | `WeakPoint` socket + gait role (FORGE II P3 `ForgeCreatureBody`) | per `CreatureDefinition` | story tie per `CREATURE_DESIGN.md` (Bloom/Warden/faction) + telegraph readability |
| `buildingModule` | snap points per `design/ART_REGISTRY.md` id family | module grid | door-on-street law compatibility (`BuildingGrammar`) |
| `prop` | `Interact` anchor if interactive | ≤4m | collider on the contract root, never the visual child |
| `shipPart` | berth/seat/door markers per `systems/SHIPS.md` | hull class | never parent the rig (SHIPS law) |

Upgrade path (identical for every type): primitive fallback → Forge proxy → textured (E1.2+) →
baked prefab (E1.4) → imported hero pass (ART_REGISTRY §5 triggers) — the id, sockets, colliders,
and gameplay scripts never change. `qualityState` tracks where each asset sits; `Locked` pins an
approved result behind a human-baked content hash.

## Forbidden aesthetics (photo-critique FAIL conditions, all families)
- N64-flat untextured geometry (post-E1.2 there is no excuse)
- clean-chrome generic sci-fi in lived-in families (Salvage/ToxicIndustrial)
- functionless spikes/greebles — every detail implies maintenance, function, or growth
- direct franchise lookalikes (xenomorph, lightsaber, portal-gun silhouettes)
- generic bug/zombie/dinosaur creature fallbacks — every creature needs an evolution reason
  (world physics) + a story tie (canon law)
Per-family bans live in `ART_DIRECTION_MASTER_PLAN.md`'s table (e.g. AlienOrigami: no rivets, no
grime, no repair tape — it was built to last).
