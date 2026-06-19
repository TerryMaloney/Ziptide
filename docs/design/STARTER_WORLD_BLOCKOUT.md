# Starter World Blockout (onboarding planet) — plan

**Status: v1 GRAYBOX BUILT (T-Dog) — `Editor/Patching/ScenePatcherStarterWorld.cs`.** Walkable chain of
all 10 named region roots + placeholders, auto-ships via `BuildAndroid`, warpable via Dev Warp.
**One-time:** run `Ziptide → Dev → Build Starter World (graybox)` to create `StarterWorld.unity` +
commit it; thereafter the build keeps it populated. Next: on-device scale tuning + mission wiring + art-kit swap.

Distilled from the full GPT brief:
`docs/GPT_ADDITIONS/2026-06-18_Starter_World_Blockout/01_architect_starter_world_blockout_brief.md`
(read that for the long version). This is the onboarding-scoped expansion of the Toxic Venice
identity in [`LEVEL1_TOXIC_VENICE.md`](LEVEL1_TOXIC_VENICE.md), and the first concrete user of the
world-scaling work in [`SYSTEMS_ARCHITECTURE.md`](SYSTEMS_ARCHITECTURE.md).

---

## The one rule: don't overbuild it
This is an **onboarding world**, not the whole game. It should feel bigger than today's tiny test
area but get the player to the **Ziptide / other-worlds premise quickly** (~25–35 min). Prioritize
**graybox scale, pathing, region hierarchy, landmarks, future vehicle space, and mission placeholders
over final visuals.** Placeholders for NPCs, enemies, vehicles, and the gate — art comes later.

## Fantasy
A polluted, layered city-world: spaceport + cargo, ground-vehicle depot, toxic sludge canals,
elevated highway spine, slum walkways, an open badlands edge, and a dormant Ziptide gate at the end.
"Steampunk-Venice layout logic" (canals divide, bridges connect, lower = wet/dangerous, upper =
alternate routes) — **not literal Venice.**

## Region hierarchy (named roots — one district editable at a time)
```text
WorldRoot
  Hub_DockQuarter          Zone_OutskirtsTransition
  Zone_Spaceport           Zone_BadlandsVehicleArea
  Zone_GroundVehiclePort   Zone_BadlandsMissionPocket
  Zone_ToxicCity_MainSpine Zone_DormantZiptideSite
  Zone_SludgeCanals        MissionMarkers / SpawnPoints
  Zone_SlumWalkways        VehicleTestRoutes / Landmarks / LightingAndAtmosphere
```

## Map shape (connected chain + a few branches, NOT open world)
`Hub → Spaceport (+ Vehicle Port) → Toxic City Spine → {Canals / Slum / side paths} → Outskirts →
Open Badlands → Badlands Mission Pocket → Dormant Ziptide Site`. Walkable end-to-end on foot even
before vehicles exist.

| Zone | Purpose (blockout) |
|------|--------------------|
| Hub / Dock Quarter | compact safe start, orientation, overlook, exits to spaceport + city |
| Spaceport | first mission; 2–3 pads, cargo deck, gantries, control-tower silhouette |
| Ground Vehicle Port | future speeder staging; depot bay, ramp to highway, placeholder vehicles |
| Toxic City Main Spine | elevated highway backbone; bridges, sludge below, skyline, branch shortcuts |
| Sludge Canals | signature hazard identity; sludge planes, pipes, docks, narrow side paths |
| Slum Walkways | dense vertical pedestrian vibe; stacked shacks, cables, upper/lower routes |
| Outskirts Transition | city→open shift; wall/gate, broken checkpoint, first badlands view |
| Open Badlands | bounded open area for future vehicle traversal + early combat test |
| Badlands Mission Pocket | 2nd mission; ruin/relay, cover arena, objective, sightline to gate |
| Dormant Ziptide Site | end of onboarding; gate ring placeholder, socket for recovered object |

## First-world flow (fast)
1. **Spaceport Trouble** — scan/recover/repair (cargo, relay, or clear nuisance drones) → clue points outside the city.
2. **Badlands Retrieval** — travel out (vehicle route, placeholder ok), reach a ruin, fight simple droids/hazards, recover a strange core/shard.
3. **Ziptide Activation (seed)** — the object (harmonic key / resonance core / code shard) fits the dormant gate, reads as impossible/coded, wakes the gate → broader multi-world premise revealed.

Lore: seed only — artifacts seem coded, the gate feels older than the local civilization, RILL hints
the signal isn't local tech. Do **not** reveal the full Earth/contained-universe twist in world 1.

## Leave room for (don't implement yet)
Gravity Glove manipulation · Expanded Stun Dart combat · left-wrist Scan Pulse targets · small droid
encounters · ground vehicle/speeder route · ship return loop · dormant Ziptide activation · world select.

## Acceptance checklist (graybox)
- [ ] Walkable: Hub → Spaceport → Toxic City → Outskirts → Badlands → Mission Pocket → Dormant Gate.
- [ ] Canals and walkways read as separate layers; city spine reads as the backbone.
- [ ] Ground-vehicle port + badlands have width for future vehicle testing.
- [ ] All regions are clearly named roots; placeholder mission markers + vehicle marker + Ziptide socket exist.
- [ ] No final-art polish; current working systems not broken; status/checklists updated truthfully after testing.

---

## ⚠️ Lane / ownership note (reconcile before building)
The brief assigns **"Architect = world structure / scene blockout."** That conflicts with this repo's
working agreement (`HANDOFF.md`), where **scene/editor/blockout work is T-Dog's lane** and Architect is
backend/data (no headset, can't Unity-verify scenes). Whoever builds this graybox, it's **editor/scene
work** — likely a `ScenePatcher` for the new world + `WorldPackDefinition` + spawn/mission markers,
following the existing patcher pattern. Terry + T-Dog should confirm the assignment before work starts.
