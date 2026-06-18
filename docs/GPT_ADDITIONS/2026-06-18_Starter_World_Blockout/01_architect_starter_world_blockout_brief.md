# Architect Brief — Starter World Blockout

Date: 2026-06-18
Label: First-world graybox / onboarding planet plan
Audience: Architect first, with T-Dog and Gemini context

## Purpose

This document turns the current starter-world discussion into an Architect-facing implementation brief.

The goal is not to build a huge finished city yet. The goal is to graybox the playable outline of the first world so Terry can walk around in VR, test scale, understand routes, and start visualizing the actual game.

This first world should be compact enough to get the player to the Ziptide concept quickly, but rich enough to give them a taste of:

- movement and exploration
- basic weapon/tool use
- a short spaceport mission
- a short vehicle/badlands mission
- the toxic city / sludge canal identity
- the ship/portal/gate mystery
- the sense that many other planets will open up soon

## Team workflow context

Terry is coordinating multiple AI helpers:

- **Architect**: world structure, scene/blockout implementation, modular planning, architecture discipline.
- **T-Dog**: story, ship mechanics, weapons, enemies, mission concepts, and gameplay flavor.
- **Gemini**: artwork/rendering exploration, reference visuals, early art tests for villain, gun/tool, ship, environment detail, and Ziptide portal visuals.
- **GPT**: design synthesis, milestone framing, architecture guardrails, and handoff docs.

This file is intended mainly for Architect. It should be treated as a planning/build brief, not as a final story bible.

## Core design constraint

The first world should not become the whole game.

This starter planet/world is basically an onboarding world. It should feel bigger than the current tiny area, but it should not become so large or complicated that players spend hours before reaching the actual Ziptide / other-planets premise.

The player should quickly get:

1. A sense of the home civilization / starting environment.
2. A short city/spaceport mission.
3. A taste of tools/weapons.
4. A taste of vehicle or open-area traversal.
5. A simple badlands retrieval mission.
6. A discovery that leads to the Ziptide portal/gate system.
7. A clear sense that the broader game is about worlds beyond this one.

## First world fantasy

Working fantasy:

A polluted, layered city-world with a spaceport, ground vehicle port, toxic industrial canals, elevated roads, walkways, slum-like districts, and an open badlands edge beyond the city.

Visual/structural references in plain language:

- toxic city
- sludge river/canals
- steampunk Venice spatial logic
- layered walkways and bridges
- polluted industrial slums
- spaceport and cargo infrastructure
- ground vehicle/speeder depot
- highway cutting through the city
- open outskirts/badlands for vehicle testing
- dormant Ziptide/gate site at the far end of the starter sequence

Do not make it literal Venice. The useful idea is the layout logic:

- canals divide the district
- bridges connect platforms
- lower levels are wet/sludgy/dangerous
- upper walkways give alternate routes
- machinery and pipes dominate the skyline
- beauty and decay coexist

## Important story/lore context

Current long-game twist direction:

The main character is not necessarily human from Earth. The civilization the player starts in is a separate civilization. Near the end of the game, they discover Earth.

As the player and people on Earth compare artifacts and discoveries, they realize many clues across worlds point to a code or underlying structure. Eventually they break through and discover that their universe is gated/contained. There is something outside it, and something like a program around it.

At the extreme reveal, they connect through the code and see something like a webcam feed: people working in a computer lab. Their universe is inside that lab, and there are other universes too.

This sets up future expansions: new universes can be released later, similar to seasonal/cycle-based world shifts.

For the first world, do not reveal all this. Only seed the premise lightly:

- artifacts seem coded
- the gate/Ziptide system feels older than the current civilization
- the recovered object has strange machine/code behavior
- the first portal/gate is dormant until activated
- RILL or another companion can hint that the signal pattern does not match local technology

## Starter world structure

Build one connected playable blockout with clearly named region roots.

Recommended hierarchy:

```text
WorldRoot
  Hub_DockQuarter
  Zone_Spaceport
  Zone_GroundVehiclePort
  Zone_ToxicCity_MainSpine
  Zone_SludgeCanals
  Zone_SlumWalkways
  Zone_OutskirtsTransition
  Zone_BadlandsVehicleArea
  Zone_BadlandsMissionPocket
  Zone_DormantZiptideSite
  MissionMarkers
  SpawnPoints
  VehicleTestRoutes
  Landmarks
  LightingAndAtmosphere
```

This lets later sessions modify one district at a time without touching the whole world.

## Map shape

Use a simple connected chain with a few branches, not a sprawling open world.

```text
[Starter Hub / Dock Quarter]
        |
        v
[Spaceport] ---- [Ground Vehicle Port]
        |
        v
[Toxic City Main Highway / Spine]
   /        |          \
  /         |           \
[Canals] [Slum Walkways] [Industrial Side Paths]
        |
        v
[City Edge / Outskirts Transition]
        |
        v
[Open Badlands / Vehicle Test Area]
        |
        v
[Badlands Mission Pocket]
        |
        v
[Dormant Ziptide Site]
```

The player should be able to walk through the full chain, even if vehicle systems are not implemented yet.

## Zone details

### 1. Starter Hub / Dock Quarter

Purpose:

- safe start area
- orientation
- first NPC/contact placeholder
- gives a view toward the larger world
- connects to spaceport and toxic city

Blockout ingredients:

- small landing/dock platform
- a few stacked structures
- one obvious exit route
- one overlook/vantage point
- signage placeholders
- return marker

Do not overbuild. This should be compact.

### 2. Spaceport

Purpose:

- first mission location
- clear sci-fi scale moment
- introduces cargo, ships, security, and city infrastructure

Blockout ingredients:

- 2-3 landing pads
- cargo deck
- stacked crates/containers
- gantries/catwalks
- control tower silhouette
- energy/fuel pylons
- security checkpoint placeholder
- connection to ground vehicle port

First mission can happen here without needing a full enemy ecosystem yet.

### 3. Ground Vehicle Port

Purpose:

- future vehicle handoff point
- gives a believable way to leave city for badlands
- staging area for the speeder-equivalent vehicle later

Blockout ingredients:

- open garage/depot bay
- ramp to road/highway
- 1-2 parked vehicle placeholder shapes
- repair crane/arms
- route marker leading outward

Vehicle does not need to be functional yet. Leave space and route width for later testing.

### 4. Toxic City Main Spine / Highway

Purpose:

- connects spaceport to city edge
- major navigational backbone
- gives the player a sense of scale

Blockout ingredients:

- elevated or semi-elevated highway
- broken sections/ramps
- toxic sludge visible below
- bridge crossings
- industrial towers
- skyline silhouettes
- shortcuts to canal/slum layers

Make this readable. The player should understand, "This road takes me through the city."

### 5. Sludge Canals

Purpose:

- signature visual identity
- environmental hazard space
- future puzzle/traversal area

Blockout ingredients:

- green/brown sludge water planes
- canal walls
- bridges
- pipe outlets
- low docks
- pumping machinery
- toxic hazard markers
- some narrow side paths

Keep canal paths compact but memorable.

### 6. Slum Walkways

Purpose:

- dense vertical pedestrian routes
- gives city life/decay vibe
- future NPC/scavenger pockets

Blockout ingredients:

- stacked shacks/modules
- balconies
- pipes
- hanging cables
- narrow bridges
- upper/lower routes
- small market/repair stall placeholders

This is a vibe zone. It should feel dense, but do not make it a maze yet.

### 7. Outskirts Transition

Purpose:

- makes the shift from city to open world feel real
- gate between dense city and vehicle/open testing

Blockout ingredients:

- city wall/gate
- broken checkpoint
- road widens
- fewer buildings
- more open ground
- wreckage/junk
- first view of badlands

### 8. Open Badlands / Vehicle Test Area

Purpose:

- lets future vehicle/speeder movement breathe
- gives room for flying/driving around before leaving atmosphere later
- supports basic open traversal and early combat testing

Blockout ingredients:

- broad open area
- wide vehicle paths
- sparse rocks/wrecks
- a few ramps or natural ridges
- simple route loops
- obstacle clusters
- distant landmark leading to mission pocket

This should be larger than city spaces, but still bounded.

### 9. Badlands Mission Pocket

Purpose:

- second mission encounter area
- first retrieval/combat objective
- leads to Ziptide discovery

Blockout ingredients:

- isolated ruin/wreck/relay station
- small arena with cover
- simple enemy/droid/scavenger placeholders
- objective object pedestal/container
- return path
- sightline to dormant gate site or landmark

### 10. Dormant Ziptide Site

Purpose:

- end of first-world onboarding arc
- introduces the portal/gate concept
- launches the broader multi-world game

Blockout ingredients:

- circular gate platform or ring placeholder
- alien/ancient structure silhouette
- dormant power conduits
- socket for recovered object
- safe staging area around it
- strong landmark visible from afar

Do not fully implement final portal visuals yet. Use placeholder geometry and named sockets/markers so Gemini/art can iterate later.

## First mission flow

Keep this fast.

### Mission 1 — Spaceport Trouble

Player is asked to go to the spaceport and inspect/retrieve/repair something.

Possible objective:

- scan a tampered cargo crate
- recover a missing component
- restore a relay
- clear nuisance drones
- find a stolen cargo marker

Outcome:

- player returns with clue or component
- clue points outside city
- introduces that something is wrong with the local infrastructure/signals

### Mission 2 — Badlands Retrieval

Player is sent to the outskirts/badlands.

Possible setup:

- use vehicle depot route, even if vehicle is placeholder at first
- travel through open badlands
- find a protected ruin/wreck/objective site
- fight simple drones/scavengers or avoid hazards
- recover strange device/core/shard

Outcome:

- recovered object activates or reveals dormant Ziptide site
- first portal/gate mystery becomes clear

### Mission 3 Seed — Ziptide Activation

The recovered object should not just be a normal map. Better options:

- harmonic key
- resonance core
- signal prism
- gate activator
- code shard
- navigation shard
- ancient relay lens

This object can:

- fit into the dormant Ziptide site
- reveal coordinates
- wake up the gate structure
- produce impossible code/signature readings
- trigger RILL/companion concern or curiosity

## First-world pacing target

Target player experience:

- 2-5 minutes: understand start/hub/spaceport direction
- 5-10 minutes: complete first spaceport task
- 10-15 minutes: travel toward badlands / vehicle route
- 15-25 minutes: complete badlands retrieval
- 25-35 minutes: reach/activate/discover dormant Ziptide site

This is not strict, but the key point is: do not make the first world a giant prelude.

The first world should get players to the broader premise quickly.

## Graybox implementation rules

### Do

- Use simple blockout geometry.
- Name every region root clearly.
- Add landmarks and sightlines.
- Make all primary paths walkable.
- Leave enough width for future vehicles.
- Use placeholders for mission markers, NPCs, enemies, gates, and vehicles.
- Keep the hierarchy modular.
- Preserve current working systems.
- Update status/checklists truthfully after implementation and testing.

### Do not

- Over-polish visuals.
- Add final art everywhere.
- Build the entire city/planet.
- Add complex enemy AI yet.
- Add final vehicle physics yet.
- Hardcode story names into engine systems.
- Create one giant messy scene hierarchy.
- Claim any feature is complete until tested.

## Suggested blockout scale

Rough relationships only:

- Starter Hub: compact, safe, 1-2 minutes to explore.
- Spaceport: medium, 2-3 pads plus cargo/gantry area.
- Toxic City: medium, one main spine plus canal/slum branches.
- Outskirts: short transition corridor/road.
- Badlands: open enough for vehicle testing, but still bounded.
- Mission Pocket: small combat/objective arena.
- Dormant Gate Site: small landmark area.

This should feel like a vertical slice of a world, not a full open-world city.

## Art/rendering workflow notes

Gemini/art exploration can focus on a few test assets only:

- villain or antagonist direction
- one gun/tool test
- one ship test
- one environment detail test
- one Ziptide portal/gate visual test

For Architect's graybox, use simple placeholders and clear sockets/marker objects so art can replace them later.

## Planned future systems that should influence layout

Even if not implemented now, leave room for:

- Gravity Glove object manipulation
- Expanded Stun Dart combat
- left-wrist Scan Pulse targets
- small enemy/droid encounters
- ground vehicle/speeder-equivalent route
- ship return loop
- dormant Ziptide activation
- future planet/world selection

## Recommended Architect deliverable

Create or expand a playable graybox first-world scene/region that includes:

- Starter Hub / Dock Quarter
- Spaceport
- Ground Vehicle Port
- Toxic City Main Spine
- Sludge Canals
- Slum Walkways
- Outskirts Transition
- Open Badlands Vehicle Area
- Badlands Mission Pocket
- Dormant Ziptide Site

The result should be explorable on foot. It should be organized into named root objects. It should include placeholder mission markers and landmark silhouettes. It should keep the map compact enough to serve as an onboarding world.

## Suggested acceptance checklist

- Player can start in the hub and walk to the spaceport.
- Player can follow a clear route from spaceport into toxic city.
- Player can see sludge canals and walkways as separate readable layers.
- Player can reach a ground vehicle port / future vehicle staging area.
- Player can move from city edge into an open badlands area.
- Badlands area has enough space for later vehicle movement testing.
- Player can reach a mission pocket in the badlands.
- Player can reach a dormant Ziptide/gate site.
- All major regions are named clearly in hierarchy.
- Placeholder mission markers exist.
- Placeholder vehicle marker exists.
- Placeholder Ziptide activation object/socket exists.
- No final-art polish is required.
- Current known working systems are not broken.

## Claude / Architect prompt

Architect, read this file and treat it as the first-world graybox brief:

```text
docs/GPT_ADDITIONS/2026-06-18_Starter_World_Blockout/01_architect_starter_world_blockout_brief.md
```

Before building, verify the current repo state and current testing/status docs. If everything is stable, create a compact playable graybox expansion of the starter world using the zones and hierarchy described here.

Do not overbuild. This is an onboarding planet that should get the player to the Ziptide/other-worlds premise quickly. Prioritize scale, pathing, region separation, landmarks, and future extensibility over final visuals.

After implementation, update the appropriate checklist/status docs with what was actually changed and what still needs testing.
