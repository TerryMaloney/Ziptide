# GPT Additions — Tidefront Multiplayer / Planet Control Plan

Date: 2026-06-16
Label: Risk-style galaxy strategy layer

## Working name

Tidefront

This is the ship-based galaxy strategy layer that sits above the VR adventure systems.

Core pitch:

Ziptide has a holographic strategy map in the player's ship. Planets are nodes. Players capture planets, gather resources, build defenses, build attack vessels, and resolve attacks through weighted probability. Optional VR missions can improve attack or defense odds.

This makes the universe feel large without requiring every planet conflict to be fully rendered.

## Why this fits Ziptide

Tidefront lets the project scale to many planets using mostly data:

- planet metadata
- ownership
- resources
- defense values
- attack vessel values
- adjacency
- league state
- battle results
- short hologram/cutscene presentation

A planet can matter strategically before it is fully built as a VR world.

## Core planet data

```text
PlanetNode
- planetId
- displayName
- biomeType
- ownerId
- resourceType
- resourceProductionRate
- defenseLevel
- orbitalShieldLevel
- stationedDefenseUnits
- specialTraitId
- adjacentPlanetIds
- conflictState
- instabilityLevel
- bloomContaminationLevel
```

## Core resource model

Start with three resources. Do not add too many currencies early.

```text
Flux      - energy, ship power, attack vessels
Alloy     - structures, defenses, repairs
Bloommatter - risky biotech upgrades, special defenses, alien tools
```

Possible later rare resource:

```text
Gate Shards - special travel, advanced tech, rare unlocks
```

## Attack and defense loop

1. Player owns one or more planets.
2. Planets generate resources over time or per turn.
3. Player spends resources on defenses, attack vessels, or upgrades.
4. Player chooses a connected enemy/neutral planet to attack.
5. Game compares attack strength and defense strength.
6. Probability roll resolves the result.
7. Optional VR missions can modify the odds.
8. Ownership, damage, resources, and cooldowns update.

## Weighted probability model

Example:

```text
Equal power: 50% attacker win chance
Attack +1: 55%
Attack +2: 60%
Attack +3: 65%
Attack +4: 70%
Attack +5: 75%
Attack +6: 80%
```

Clamp odds so nothing becomes guaranteed.

```text
Minimum chance: 10%
Maximum chance: 90%
```

This keeps the Risk-like drama where weaker sides can sometimes win, but stronger preparation still matters.

## Battle outcomes

Avoid simple binary win/loss only.

Possible results:

- Major Victory: attacker captures planet and keeps most vessel strength.
- Costly Victory: attacker captures planet but vessel is damaged.
- Stalemate: no capture; both sides lose some strength/resources.
- Failed Attack: defender holds; attacker loses vessel strength.
- Counterstrike: rare defender-favorable outcome that damages attacker resources or nearby position.

## Planetary defenses

### Shield Spire

Reliable defense score increase.

### Drone Net

Counters small attack vessels and can appear as VR defense drones on that planet.

### Gate Jammer

Reduces attacks through gate routes.

### Decoy Beacon

Confuses scouting and hides true defense value.

### Repair Swarm

Repairs defenses over time.

### Gravity Minefield

Damages attack vessels before battle resolution.

### Resource Vault

Protects some resources if planet is captured.

### Bloom Barrier

Powerful but risky. Raises defense but increases Bloom contamination or future planet problems.

## Attack vessels / defense breakers

### Scout Skiff

Cheap recon vessel. Reveals defense estimates and special traits.

### Pulse Frigate

Basic attack vessel.

### Shieldbreaker Barge

Weak direct attack, strong against orbital shields.

### Gate Piercer

Can attack through special gate lanes or bypass limited adjacency at high cost.

### Siege Lantern

Slow but powerful defense breaker. Gives defenders time to react.

### Drone Carrier

Swarm-based attack bonus against low-defense planets.

### Null Ark

Rare late-game vessel that temporarily disables one special defense.

### Resource Harvester

Non-attack vessel that improves extraction but can become a raid target.

## VR mission connection

Tidefront should not just be a menu. Optional VR missions should affect the strategy layer.

### Attack-side missions

- Sabotage shield generator.
- Scan defense grid.
- Steal access codes.
- Plant beacon.
- Disable gate jammer.
- Rescue captured drone.

Rewards:

- enemy shield -1 or -2
- reveal true defense value
- attack vessel +1
- increase attacker win chance
- unlock alternate attack route

### Defense-side missions

- Repair shield tower.
- Power drone grid.
- Clear Bloom interference.
- Recover defense battery.
- Manually shoot down attack drones.

Rewards:

- defense +1 or +2
- reduce incoming attack strength
- protect resources
- delay attack resolution

## Ship-map presentation

Tidefront should live on a holographic table in the ship, not just a flat menu.

Player should be able to:

- walk up to the galaxy table
- rotate/scale the star map
- tap a planet
- inspect owner/resources/defense
- drag attack vessel from owned planet to target
- confirm with a button/lever
- watch holographic ships and shields resolve the conflict

This keeps strategy inside VR.

## Multiplayer structure

Separate friendly play from ranked play.

### Private Galaxy

For family/friends.

- Invite-only.
- Custom rules.
- No serious ranking.
- Alliances allowed.
- Friend groups can play freely.

### Public Ranked Galaxy

For competitive solo players.

- Solo queue only.
- No direct friend invites into the same ranked galaxy.
- No direct trading.
- Limited communication.
- Matchmaking by rank/skill.
- Seasons reset.

### Squad Galaxy

For groups.

- Squads fight other squads.
- Separate ranking from solo.
- Could be 2v2v2v2 or 3v3v3.

## Anti-ganging protections

### No friend stacking in solo ranked

Public solo ranked should not intentionally place friends into the same galaxy.

### Attack limits

Same planet can only be attacked a limited number of times per cycle.

Example:

```text
Max attacks against same planet per 12 hours: 2
After that: temporary Gate Lockdown / Stabilizer Field
```

### Dogpile defender bonus

Repeated attacks against the same owner trigger defensive scaling.

```text
First attack: normal
Second attack: defender +1
Third attack: defender +3
Fourth attack: temporary shield / lockdown
```

### Adjacency rules

Players can only attack connected planets unless using special vessels/routes.

### Fog of war

Players do not see all weakness information without scouting.

### No direct ranked trading

Prevents friend groups from funneling resources.

### Same-target cooldown

After attacking a player, attacker cannot immediately attack the same player again unless retaliation opens a conflict window.

### Bounty / pressure system

Dominant players become more visible or valuable targets, creating self-balancing pressure.

## Snowball control

### Maintenance cost

More planets require more upkeep.

### Distance penalty

Planets far from home gate are harder to defend.

### Instability

Newly captured planets produce less until stabilized.

### Underdog missions

Lower-ranked players can get comeback missions such as sabotage, hidden neutral planet discovery, or temporary defense boosts.

### Season reset

Ranked galaxies end. Nobody owns everything forever.

## League sizes

### Beginner league

```text
8 players
20-30 planets
3-5 day seasons
fast resource production
low stakes
```

### Standard league

```text
16 players
50-80 planets
1-2 week seasons
moderate progression
```

### Large league later

```text
32-64 players
100+ planets
3-4 week seasons
team events and special planets
```

## Ranked progression

Possible ranks:

```text
Cadet
Navigator
Pathfinder
Vanguard
Star Captain
Gate Warden
Architect
```

Score should reward more than conquest:

```text
League Score =
+ planets held
+ resources generated
+ successful attacks
+ successful defenses
+ missions completed
+ underdog victories
+ special objectives
- repeated failed attacks
- abandoned attacks
```

## Recommended prototype order

### Prototype 1 — Local Galaxy Sandbox

No multiplayer. No server.

- 10 planets
- 3 resource types
- neutral/enemy ownership
- build defense button
- build attack vessel button
- attack adjacent planet
- weighted probability result
- ownership changes
- local save

### Prototype 2 — AI Opponents

- AI collects resources.
- AI builds defenses.
- AI attacks weak adjacent planets.
- AI reinforces borders.

### Prototype 3 — Private Async Galaxy

- Friend/family invite-only galaxy.
- Async turns or timed actions.
- Basic cloud/server persistence if available.

### Prototype 4 — Ranked League

Only after private works.

- Season state.
- Matchmaking.
- Anti-ganging rules.
- Rank scoring.

## Implementation guidance

Build Tidefront as metadata first. Do not start with multiplayer.

Recommended systems:

```text
ConquestNode / PlanetNode
ConquestMap
ConquestState
ConquestRules
ConquestResolver
ConquestAction
ConquestSaveData
ConquestAI
ConquestPresentation
```

Keep battle resolution deterministic/testable when given a seed.

Test examples:

- attack odds clamp between 10% and 90%
- adjacent-only attack rule works
- resources deduct correctly
- planet ownership changes on victory
- defense structures modify defense score
- dogpile shield activates after repeated attacks

## Final design summary

Tidefront is the scalable galaxy strategy layer. Planets generate resources. Players build defenses and attack vessels. Attacks resolve with weighted probability. Optional VR missions modify odds. Private galaxies allow friends to play freely. Public ranked uses solo/squad-separated leagues and anti-dogpile protections.

This gives every planet a reason to exist before every planet needs to be fully rendered.