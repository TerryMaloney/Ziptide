# CREATURE BEHAVIOR COVERAGE LAW

**Status:** v1 gate contract · 2026-07-11  
**Scope:** committed story-creature definitions under `Assets/Ziptide/Resources/Enemies`.

## Why

A creature is not finished merely because it has a model, health, and a chase loop. A shipped species
must communicate a small readable vocabulary: what it does when undisturbed, how it responds to the
player, what it telegraphs, and what counter-window the player can recognize.

This gate prevents future creature IDs from entering the shipped catalog with only one repeated state.
It does not tune movement, stats, combat, rewards, or art.

## Law

1. Every committed `CreatureDefinition` in `Resources/Enemies` maps to exactly one behavior profile.
2. Every behavior profile contains at least **three unique, named, non-empty readable modes**.
3. Generic runtime states such as `stunned`, `disabled`, `down`, or `respawn` do not count toward the
   species vocabulary. Those belong to `CreatureRuntime`, not the species behavior.
4. Every mode carries a source evidence token. If the real behavior implementation loses that token,
   CI fails until the catalog is deliberately reconciled.
5. Every creature profile carries a `CityBuilder.MakeCreature` factory token. An authored creature that
   cannot receive its intended behavior is a build blocker.
6. Catalog IDs and committed creature-definition IDs are one-to-one. No orphan asset and no imaginary
   catalog entry may ship.
7. The gate is structural. Device testing still decides whether the states are visually readable,
   fairly timed, comfortable, and fun.

## Current shipped vocabulary

| Creature ID | Behavior | Readable vocabulary pinned by the gate |
|---|---|---|
| `swarm_bug` | `SwarmerBehavior` | idle skitter · orbit/standoff · gather-and-dart |
| `tendril` | `WallCrawlerBehavior` | wall stalk · ripple telegraph · drop/lunge · return |
| `light_grazer` | `LightGrazerBehavior` | dark growth · dark advance · lit shrink/recoil |
| `witness_mite` | `WitnessMiteBehavior` | observed freeze · unobserved ready · unobserved stalk |
| `tether_swarm` | `TetherSwarmBehavior` | ambient pair weave · engaged standoff weave · vulnerable tether-node tell |
| `husk_molter` | `HuskMolterBehavior` | return/home · stalk · molt-and-escape |
| `warden` | `WardenBehavior` | dormant · watch · warn · pursue · ally |

The table describes behavior—not animation clips. Picasso may replace or improve the visible body and
animation while these gameplay meanings remain stable.

## Gate outputs

- `CREATURE_BEHAVIOR_PROFILE_MISSING`
- `CREATURE_BEHAVIOR_PROFILE_DUPLICATE`
- `CREATURE_BEHAVIOR_STATES_LOW`
- `CREATURE_BEHAVIOR_STATE_INVALID`
- `CREATURE_BEHAVIOR_GENERIC_STATE_COUNTED`
- `CREATURE_BEHAVIOR_ASSET_ID_EMPTY`
- `CREATURE_BEHAVIOR_ASSET_ID_DUPLICATE`
- `CREATURE_BEHAVIOR_PROFILE_ORPHAN`
- `CREATURE_BEHAVIOR_SOURCE_MISSING`
- `CREATURE_BEHAVIOR_EVIDENCE_DRIFT`
- `CREATURE_BEHAVIOR_FACTORY_DRIFT`

## Adding a creature

1. Author its `CreatureDefinition` under `Resources/Enemies`.
2. Implement or select its `CreatureBehaviorBase` subclass.
3. Wire it through `CityBuilder.MakeCreature`.
4. Add one catalog profile with at least three genuine readable modes and stable code evidence tokens.
5. Add focused behavior tests when the new state machine contains logic worth isolating.
6. Run Unity EditMode CI and then verify the vocabulary in-headset.

Do not satisfy this gate by renaming three phases of the same chase, counting generic stun/down states,
or adding dead metadata that is not present in the real source.
