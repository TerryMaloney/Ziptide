# WORLD BLUEPRINT — how to build any world (the repeatable recipe)

Toxic City is the reference implementation of this recipe. The whole point: **a world is data + one
thin patcher**, never bespoke geometry code. Once you've built one, world #2/#3 are mostly *authoring
an asset*. Read this before starting a new world.

## The layers (what you author vs what's shared)

| Layer | Artifact | New per world? | Where |
|------|----------|----------------|-------|
| Layout | `CityLayoutDefinition` asset | ✅ author one | `Content/City/<World>Layout.asset` |
| Geometry | `CityBuilder` (shared core) | ❌ reuse | `Editor/Patching/CityBuilder.cs` |
| Patcher | `ScenePatcher<World>` (thin shell) | ✅ ~copy + rename | `Editor/Patching/` |
| Content | `WorldPackDefinition` (scene, jobs, markers, audio) | ✅ author one | `Content/Worlds/Packs/` |
| Jobs | `JobDefinition` step assets | ✅ author | `Content/.../Jobs/` (Architect lane) |
| Story | narrative doc + in-game text data | ✅ write | `docs/storyboard/<World>/` |
| Audit | `WorldAuditRunner` | ❌ reuse (generalized) | runs in the build |

## The data model — `CityLayoutDefinition`

Everything that shapes a city is here, so re-laying-out a world (or spinning a new one) is editing an
asset in the Inspector:
- `cityId`, `seed`, `walkwayHeight` (the canonical walkable Y the audit checks), `palette`, fog, skyline.
- `districts[]` — id, anchor, bounds, heightTier, landmarks, **heroBuildings** (enterable: interior
  kind + door + interior marker), props.
- `connections[]` — the **street grid**: GroundStreet / ElevatedWalkway / Bridge / Ramp between
  districts (with `tier` for multi-level over canals).
- `canals[]` — decorative toxic sludge (colliders stripped, so never walkable / never a spawn floor).
- `droneZones[]` — where drones spawn (id, center, count, respawn, **combat** flag, variantId). NOT
  every district needs one.
- `shipyard` — berth + a static (non-flyable) ship placeholder.
- `Validate()` — pure structural check (dangling connections, hero markers, dup ids). Call it / test it.

## Build one in 5 steps

1. **Author a layout asset.** Easiest: copy `ToxicCityLayout.asset`, or let the patcher self-generate a
   default and edit it. Lay out districts + connections so the streets *read as a city you can walk*
   (a boulevard + side streets forming a loop — see Toxic City's default).
2. **Copy the patcher shell.** Duplicate `ScenePatcherToxicCity.cs` → `ScenePatcher<World>.cs`. Change
   `SceneName` + the 4 paths (scene / layout / worldpack / it derives the root from `cityId`). The body
   already just calls `CityBuilder.Build(root, kit)` + wires spawn/world-pack/travel/dispatch — leave it.
3. **Register in the build.** Add the two lines to `BuildAndroid.PatchScenesThenAPK` (an
   `EnsureInBuildSettings()` before the loop + a `Populate…()` hook inside it), mirroring the ToxicCity
   block.
4. **Add constants.** Scene/layout/worldpack paths in `ZiptideConstants`. (Root name is derived as
   `"__" + cityId.ToUpper() + "_ROOT"`, which the audit already accepts — no audit edit needed.)
5. **Author the story + jobs.** A `docs/storyboard/<World>/STORY.md`, a `WorldPackDefinition`, and the
   `JobDefinition` steps (Architect lane). Player-facing text lives in data so it's rewritable.

That's the whole clone cost: **1 asset + 1 ~shell patcher + 3 wiring lines + 1 worldpack + 1 story
doc.** No new geometry code.

## Audit / safety rules baked into the recipe (don't fight them)
- The patcher self-generates the scene headless (cloud build needs no Unity PC). **Never hand-edit
  `.unity` YAML.**
- Spawn must sit on a solid slab at `walkwayHeight` (clears `SPAWN_NO_FLOOR` / `SPAWN_BELOW_WALKWAY`).
- Canals/sludge get colliders stripped (you can't spawn-snap onto goo; the fall-net catches drops).
- Elevated/over-canal walkways + bridges get **railings** on both edges (the catwalk-fall fix).
- World scenes never contain an XR Origin / XRI manager / DontDestroyOnLoad singletons (those live in
  `_Boot`). The audit blocks the build if they do.
- City scenes need a `__…_ROOT` root and (if in `TravelSceneNames`) a travel door.

## Combat per world (Drone Combat V1)
Mark a `droneZone.combat = true` and the patcher attaches `DroneCombatBehavior` (orbit/strafe →
telegraph → non-lethal stun bolt). Variants are pure `DroneCombatProfile` assets (drop under
`Resources/Enemies/<variantId>`). Passive drones = `combat = false`. Mini-enemies / other archetypes
come later off the same `DroneRuntime`/`IShockable` base.

## Reference
- Toxic City layout/story: `docs/storyboard/W001_ToxicCity/STORY.md`, `docs/design/LEVEL1_TOXIC_VENICE.md`.
- Art identity: `docs/project_art_plan/W001_TOXIC_VENICE_ART_BRIEF.md`.
- Drone reactions: `docs/systems/CREATURE_DRONE.md`. Ship system (future exit): `docs/design/SHIP_SYSTEM.md`.
