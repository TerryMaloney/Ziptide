# CREATURE BEHAVIOR READABILITY CONTRACT

**Status:** v1 machine-gated contract · 2026-07-11  
**Purpose:** prevent a creature from shipping as one motion loop wearing a finished body.

## Law

Every shipped `CreatureDefinition` must have exactly one machine-readable profile with:

- at least **three distinct active states** recognizable through motion, posture, light, spacing, or an exposed affordance;
- one named **telegraph state** that makes the next action fair in VR;
- one named **counter/vulnerability state** that tells the player what to do;
- one separate **non-lethal disabled/resolved state**;
- the expected authored archetype and real runtime behavior type;
- a source-evidence token for every active state;
- an exact `CityBuilder.MakeCreature` factory-wiring token.

Catalog IDs and committed creature-definition IDs are one-to-one. A profile without an asset, an asset
without a profile, missing behavior source, lost evidence token, archetype drift, missing behavior class,
or missing factory route is a blocker.

Generic runtime states such as stun/down/respawn are recorded separately as the resolution state. They do
not satisfy the three-active-state species vocabulary by themselves.

## One source of truth

`Ziptide.Content.CreatureBehaviorReadabilityCatalog` is the only behavior-quality catalog. It is metadata,
not a runtime behavior owner. Runtime ownership remains:

- `CityBuilder.MakeCreature` — chooses the existing behavior component;
- `CreatureBehaviorBase` subclasses — own motion and readable behavior;
- `CreatureRuntime` — owns damage, stun, non-lethal disable, loot and respawn;
- Picasso / Forge — owns bodies, materials, articulation and visual presentation.

The Editor audit adds verification, never movement or presentation.

## Current roster vocabulary

| Creature | Existing behavior | Active readable states | Telegraph / counter | Disabled state |
|---|---|---|---|---|
| `swarm_bug` | `SwarmerBehavior` | patrol/orbit · gather · dart | gather / gather | stunned/down |
| `tendril` | `WallCrawlerBehavior` | surface patrol · ripple · drop/lunge · return | ripple / ripple | stunned/grounded |
| `light_grazer` | `LightGrazerBehavior` | dark grow · dark approach · lit shrink/recoil | lit recoil / lit recoil | stunned/down |
| `witness_mite` | `WitnessMiteBehavior` | unobserved idle · unobserved stalk · observed freeze | freeze / freeze | stunned/down |
| `tether_swarm` | `TetherSwarmBehavior` | cluster weave · engaged standoff weave · tether node exposed | node exposed / node exposed | colony disabled |
| `husk_molter` | `HuskMolterBehavior` | stalk · molt escape · cooldown vulnerability | molt / cooldown | stunned/down |
| `warden` | `WardenBehavior` | watch · warn · arrest/disengage · ally calm | warn / ally calm | stunned/stand-down |

These labels summarize behavior already present in M3. They do not add animation or gameplay promises.

## Enforcement

Two complementary layers use the same catalog:

1. **EditMode CI tests** verify profile structure, exact asset coverage, archetype/type agreement, source tokens, and factory routing.
2. **`CreatureBehaviorBuildGate`** runs before APK generation and throws on any blocker, so a direct build cannot bypass the quality law.

Audit log prefix: `ZIPTIDE: CREATURE_BEHAVIOR_AUDIT`.

## Adding a creature

1. Author its `CreatureDefinition` under `Resources/Enemies` with exact filename/id parity.
2. Implement or select its `CreatureBehaviorBase` subclass.
3. Wire it through `CityBuilder.MakeCreature`.
4. Add one canonical profile with honest active states, telegraph, counter, disabled state, source tokens, archetype and factory token.
5. Add focused pure behavior tests when the new state machine contains logic worth isolating.
6. Pass Unity CI and the APK pre-build gate.
7. Verify on Quest that the states are actually readable, fairly timed, comfortable and fun.

Do not satisfy the gate by renaming phases of one chase loop, counting generic stun/down states, or adding
metadata that is not present in the real behavior source.
