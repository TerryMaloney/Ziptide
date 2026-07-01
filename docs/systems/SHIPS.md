# SHIPS — the flying-vehicle system (the north star), modular from day one

**Vision (Terry):** the ship replaces the travel door. You walk to your berthed hull, board it, sit in
the cockpit, pick a world, and FLY out — eventually a full flyable vehicle with upgrades. This doc is the
architecture + phased build plan so any operator (Opus included) can build it without breaking the locked
contracts. **The data layer already exists:** `ShipDefinition` (`Content/Runtime/Definitions/`) — hull,
cockpit seat, boarding door, flight feel, upgrade slots, reachable worlds.

## The one architectural decision (locked here)
**The ship is a mobile `WorldTravelStation`, not a scene-streaming vehicle.** Scene changes go through
`TravelCoordinator.TravelTo` — the ONLY legal path (locked contract #1). Flying between worlds is
therefore staged:
1. Board + cockpit + destination select = a travel station wearing a ship costume. Same story-gating as
   doors (`WorldGating.MeetsRequirements` — the ship UI must consult it exactly like `WorldTravelStation`).
2. The "flight" is presentation: a cinematic fly-out (cockpit shake, world recedes / hyperspace tunnel)
   played AROUND the travel, not a physics sim across scenes. Comfort-first (vignette from
   `ShipDefinition.flightVignette`; mirrors the gravity-hop comfort model).
3. TRUE free-flight (if ever) is a dedicated flight SCENE (space arena world you travel into), not
   streaming — that keeps `_Boot`/rig/travel contracts intact forever.

## Phases (each independently shippable; 🔧 = Terry bakes, 🎮 = Terry feel-tests)
- **S0 (done):** static berth ship — `ShipyardBerthDef` in the layout; `CityBuilder.BuildShipyard`
  builds the placeholder hull. `ShipDefinition` data authored (this sprint).
- **S1 — Boardable shell (🔧→🎮):** `ShipBoardingStation` (Gameplay): a boarding trigger at
  `boardingDoorLocalPos` teleports the player INTO a small cockpit interior (same scene — just geometry);
  a cockpit "helm" interactable lists destinations (reuse `DevWorldManifest`/pack list + `WorldGating`)
  and calls `TravelCoordinator.TravelTo`. Replaces the exit door on worlds with a berth. ~1 patcher +
  1 runtime script; no contract changes.
- **S2 — Fly-out presentation (🎮):** on confirm: seat-lock (brief), engine audio (AudioProfile), a
  5–8s exterior camera-safe shake + window starfield, then travel. All cosmetic, comfort-tunable from
  `ShipDefinition`.
- **S3 — Upgrades (⚙CI+🎮):** `ShipSlotDef` sockets accept items (`Resources/Items` ids) — engine tier
  changes fly-out length/boost feel, scanner tier feeds the wrist scanner, cargo raises carry limits.
  Pure data + a socket interactable; economy sink lands here.
- **S4 — Free-flight arena (🎮, optional/late):** a dedicated `SpaceLane` world where the cockpit gets a
  real flight controller (`cruiseSpeed`/`turnRateDegrees`); entered via normal travel. Never streams scenes.

## How to change things (playbook entries)
| I want to… | Edit |
|---|---|
| Ship size/cockpit/door/flight feel/vignette | the `ShipDefinition` asset fields |
| Which worlds a hull can reach | `reachablePackIds` (empty = all unlocked; story-gating still applies) |
| Add an upgrade socket | add a `ShipSlotDef` to `slots` |
| Where the berth sits in a world | the layout's `ShipyardBerthDef` (`berthCenter`/`shipLocalPos` etc.) |
| A second hull | new `ShipDefinition` asset |

## Guardrails for whoever builds S1+
- NEVER bypass `TravelCoordinator`; never make the ship a rig parent (the rig belongs to `_Boot`).
- Seat = teleport-anchor + comfort vignette, not physics parenting of the XR Origin.
- The cockpit destination UI must be world-space + TMP (remember the Dev-Menu TMP-Essentials lesson) and
  consult `WorldGating.MeetsRequirements` so locked worlds show locked (same as doors, `TRAVEL_LOCKED`).
- Read `docs/systems/VR_RIG_GOTCHAS.md` before ANY seat/anchor work.
