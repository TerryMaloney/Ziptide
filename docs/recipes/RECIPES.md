# ZIPTIDE — Recipes (the consistency contract)

**This is the most important doc for two-agent parallel work.** Both agents (Terry's Claude = TC,
Wife's LLM = WL) follow these recipes so every world / creature / ship / item comes out the *same
shape* — which is what lets either agent pick up the other's work ("either end of the stick").

How we actually work (the constraint that shapes everything):
- **Daytime = blind building.** No PC, no Quest. We write code against the structure, no on-device test.
- **Evening = batched verification.** Terry builds + plays 1–2h, reports by `ZIPTIDE:` tags + logcat.
- So the structure must let us build correctly *without* a headset. The four nets below make that safe.

---

## 0. The Prime Directive — every content type has the SAME five parts

| Part | What | Rule |
|------|------|------|
| **Definition** | a `ScriptableObject` of tunable data | **new types inherit `Ziptide.Content.Definition`** (gives `id` + `displayName`); add `[CreateAssetMenu(menuName="Ziptide/Definitions/…")]`; fields are `[Tooltip]`'d data only — **no logic**. (Legacy `ItemDefinition`/`WorldPackDefinition` keep their own `itemId`/`packId`.) |
| **Runtime** | a `MonoBehaviour`/class that enacts the def | `public void Init(def)` — **never reflection** (IL2CPP); reads the def, does the behavior |
| **Registry/Factory** | resolves `id → def/object` | use the generic **`DefinitionRegistry<TDef>`** (caches by `id`, warns on dup, auto-discovers via `Resources.LoadAll` from a `Resources/` subfolder) — or mirror `ItemFactory`. **Resolve by string id, never hard prefab refs.** |
| **Patcher** | idempotent **editor** script that places it in a scene | `Ziptide.Editor.Patching`, uses `PatcherUtil` (find-or-create); the patcher **is** the source of truth, not the `.unity` file |
| **Audit + Tag** | a `WorldAuditRunner` check + a `ZIPTIDE: <TAG>` log | so a blind build fails loudly at build/runtime, not silently |

If you can't express your feature as these five parts, stop and ask — don't invent a new shape.

## 1. Where files go (folders + assemblies)

Assemblies: `Core` (no deps) ← `Content` ← `Gameplay` ← `Visuals`; `Editor`, `Platform.Quest`,
`Ship`, `Tests` on top. **Core must never reference Visuals/Gameplay.**

```
Content/Runtime/<Area>/<Thing>Definition.cs      ← data (ScriptableObject)
Gameplay/Runtime/<Area>/<Thing>Runtime.cs        ← behavior (Init pattern)
Gameplay/Runtime/<Area>/<Thing>Registry.cs       ← id → def cache (if it needs one)
Editor/Patching/ScenePatcher<World>.cs           ← builds a world scene
Editor/Audit/  (WorldAuditRunner adds checks)    ← build-time validation
Core/Runtime/ZiptideConstants.cs                 ← ALL scene/asset/string literals (additive edits only)
Tests/EditMode/<Thing>Tests.cs                   ← headless logic tests (CI runs these)
```

## 2. The four nets that make blind building safe

1. **CI compiles every push** (`terry-local-wip`). Red CI = stop shipping C# until green. Check with
   `gh run list --branch terry-local-wip`.
2. **EditMode tests** (`Ziptide.Tests`) verify headless logic (save/load, idle math, registry
   resolution, balance curves) — no headset needed. **If your feature has pure logic, it gets a test.**
3. **Audit-gated build** — once the build uses `BuildAndroid.PatchScenesThenAPK`, `WorldAuditRunner`
   runs on every build; a world with spawn-below-floor / no exit door / contract violation **fails the
   build**, not the evening playtest.
4. **`ZIPTIDE:` tags** — every runtime system logs `Debug.Log("ZIPTIDE: <TAG> key=value")` so one
   logcat tells the whole story at evening verification. (Existing tags: `TRAVEL_*`, `XRI_*`,
   `INVENTORY_*`, `DRONE_DOWN`, `MOVE_DIAG`, `LOCO_STATE`, `BELT_ENSURED`, `DUP_SINGLETON`, …)

## 2b. Backbone status — what's already built (use it, don't reinvent)

These exist on `terry-local-wip` and are CI-green. Build on them:
- **`Ziptide.Content.Definition`** — base for all new content defs (`id`, `displayName`).
- **`Ziptide.Content.DefinitionRegistry<TDef>`** — generic id→def registry (auto-discovery + dup-guard).
- **Definitions** (`Content/Runtime/Definitions/`): `ResourceDefinition`, `ToolDefinition`,
  `MachineDefinition`, `PlantDefinition`, `CreatureDefinition`, `BiomeDefinition`, `RecipeDefinition`,
  `BalanceConfig`, plus `ResourceCost`. All have `[CreateAssetMenu]` — author assets in the editor.
- **Persistence** (`Core/Runtime/Persistence/`): `PlayerProfile`, `ProfileSerializer`; `SaveSystem`
  (`Gameplay`, **not yet wired into `_Boot`** — wiring is report-only).
- **Idle** (`Core/Runtime/Economy/IdleEngine.cs`): pure offline-accrual math.
- **Tests** (`Tests/EditMode/`): the `Ziptide.Tests.EditMode` assembly — add tests here for any new logic.

Still TODO (next backbone cycles): concrete registries wired at boot, harvest/mine/garden runtimes,
creature behaviors, world generator. Each follows the five-part pattern above.

## 3. Collision avoidance (two agents, one repo)

- **One patcher per world; one script per creature/ship.** Self-contained features don't merge-collide.
- **Never both edit the same `.unity`/`.prefab`/`.asset` YAML.** Full-file fileID renumbering is unmergeable.
- **Shared files** (`ZiptideConstants.cs`, `WorldPackDefinition.cs`, `EditorBuildSettings.asset`):
  additive edits only, and **claim them in `docs/SESSION_LOG.md` before editing**.
- **Log every session** in `docs/SESSION_LOG.md` (newest on top): what you touched + status + next.
- Pull before you start; small reversible commits; push often.

---

## RECIPE: add an ITEM (exemplar — already implemented)
1. `Content/Runtime/Items/<Item>Definition.cs` : `ScriptableObject`, `public string itemId`, tunables.
2. `Gameplay/Runtime/Items/<Item>Runtime.cs` (or reuse `ItemRuntime`) with `Init(def)`.
3. Add a `Create<Item>()` branch in `ItemFactory.Create()` keyed off the def type (see pistol/taser).
4. Create the `.asset` (via an `Editor/Setup` script, like `CreateDefaultWorldProfile`), give it the id.
5. Spawn it from a patcher or by `ItemFactory.Create(itemId, pos)`. Tag: `ITEM_DEF_NOT_FOUND` already covers misses.

## RECIPE: add a WORLD (the repeatable standard)
1. **`WorldPackDefinition` asset**: `packId`, `displayName`, `sceneName`, theme, audioProfile, `jobs[]`,
   `spawnMarkers[]`. (See `LEVEL1_TOXIC_VENICE.md` "World Recipe".)
2. **Scene name** → add the `.unity`, add `SceneX` const to `ZiptideConstants.cs` (additive), add to
   Build Settings (claim in log — shared file).
3. **`Editor/Patching/ScenePatcher<World>.cs`** (idempotent, `PatcherUtil`): builds geometry from a kit,
   places `__SPAWN_PLAYER` (`SpawnMarkerRuntime`) **on solid ground**, `WorldRuntime`, job objects,
   and a `ProximityTravelTrigger`/`WorldTravelStation` exit. Mirror `ScenePatcherD0`.
4. **Job chain** from existing `JobStepDefinition` types (GoToMarker, CollectItemIdCount,
   DisableDronesCount, ShootTargetsCount, DeliverToSocket) — compose, don't recode.
5. **Audit**: `WorldAuditRunner` must pass (spawn-on-solid, route, exit, perf, contract).
6. Build via `PatchScenesThenAPK`. Verify on device by `TRAVEL_OK` + `AUDIT_OK` + job-step tags.
   **Contract:** world scene is content only — NO rig/singletons; `_Boot` is never a destination.

## RECIPE: add a CREATURE (foundation being built — TC)
1. `Content/Runtime/Creatures/CreatureDefinition.cs`: `creatureId`, archetype, health, speed, damage,
   biome, loot (data only).
2. `Gameplay/Runtime/Enemies/<X>Behavior.cs` extending the `DroneRuntime`/`IShockable` base via a
   `CreatureBehavior` base class. Archetypes: `SwarmBehavior` (boids), `WallCrawlBehavior` (surface
   raycast-stick), `FlyerScavengerBehavior` (hover/dive/flee), `BruiserBehavior` (slow/tanky).
3. Movement: NavMesh (ground) / steering (fly/swarm) / surface-projection (crawl). Quest caps: max
   active per world, simple FSM, pooling, LODs.
4. Resolve via a `CreatureRegistry` (mirror `ItemFactory`). Spawn from a patcher or spawner.
5. Tag: `DRONE_DOWN`-style `CREATURE_DOWN name=…`. Test FSM transitions in EditMode where possible.

## RECIPE: add a SHIP (foundation being built — see `SHIP_SYSTEM.md`)
1. `Content/Runtime/Ships/ShipBlueprint.cs` (modular kit refs) + `ShipPartDefinition`s.
2. `Ziptide.Ship/Runtime/ShipController.cs` (arcade flight + seated cockpit rig — VR comfort) with
   `Init(blueprint)`. `ShipAssembler` builds the ship from the blueprint kit.
3. Flight input via the planned `InputRouter` (see `CONTROLS_AND_FLIGHT.md`) — don't scatter
   `InputAction` literals.
4. One full ship → seat + flight-mode → space scene + tutorial hop. Cosmetic-only monetization layer.
5. Tag: `SHIP_*`. Cockpit frame is mandatory (inner-ear reference = nausea control).

---

*Each recipe = small, CI-gated commits, verified on-device by `ZIPTIDE:` tags at evening test.*
*When in doubt, copy the nearest existing exemplar (`ItemFactory`, `ScenePatcherD0`, `DroneRuntime`).*
