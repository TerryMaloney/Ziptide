# CREATURE BEHAVIOR READABILITY CONTRACT

**Status:** v1 machine-gated contract, 2026-07-11  
**Purpose:** prevent a creature from shipping as one motion loop wearing a finished body.

## Law

Every shipped `CreatureDefinition` must have a machine-readable quality profile with:

- **at least three distinct active states** the player can recognize from motion, posture, light, spacing,
  or an exposed affordance;
- one named **telegraph state** that makes the next action fair in VR;
- one named **counter/vulnerability state** that tells the player what to do;
- one separate **non-lethal disabled/resolved state**;
- the actual runtime behavior class and expected authored archetype.

The three-state floor does not claim animation polish. It proves the behavioral vocabulary exists and keeps
future species from silently entering the shipped catalog with only an idle bob or chase loop.

## Ownership

`CreatureBehaviorReadabilityCatalog` is quality metadata only. It does not instantiate, tick, disable,
reward, respawn, or visually present creatures. Runtime ownership remains:

- `CityBuilder.MakeCreature` — chooses the existing behavior component;
- `CreatureBehaviorBase` subclasses — own motion and readable behavior;
- `CreatureRuntime` — owns damage, stun, non-lethal disable, loot and respawn;
- Picasso / Forge — owns the body, materials, articulation and visual presentation.

## Current roster vocabulary

| Creature | Existing runtime behavior | Active readable states | Disabled/resolved state |
|---|---|---|---|
| `swarm_bug` | `SwarmerBehavior` | patrol/orbit · gather telegraph · dart attack | stunned/down |
| `tendril` | `WallCrawlerBehavior` | surface patrol · ripple telegraph · drop lunge | stunned/grounded |
| `light_grazer` | `LightGrazerBehavior` | dark idle/grow · dark approach · lit shrink/recoil | stunned/down |
| `witness_mite` | `WitnessMiteBehavior` | unobserved idle · unobserved stalk · observed freeze | stunned/down |
| `tether_swarm` | `TetherSwarmBehavior` | cluster weave · tether node exposed · tether severed | colony disabled |
| `husk_molter` | `HuskMolterBehavior` | stalk · molt escape · cooldown vulnerability | stunned/down |
| `warden` | `WardenBehavior` | watch · warn · arrest/disengage · ally calm | stunned/stand-down |

These names summarize behavior already shipped in M3; they do not add new runtime promises.

## CI gate

`CreatureBehaviorReadabilityTests` scans every `CreatureDefinition` asset under
`Assets/Ziptide/Resources/Enemies` and fails when:

- an asset has no profile;
- active state count is below three;
- states are empty or duplicated;
- telegraph/counter does not name an active state;
- disabled state is absent or duplicates an active state;
- profile archetype disagrees with the asset;
- behavior type does not exist;
- a profile has no corresponding shipped asset.

Adding a creature therefore requires one explicit vocabulary entry in the same change that adds its asset.
Runtime expansion remains a separate task whenever the honest profile cannot reach the three-state floor.
