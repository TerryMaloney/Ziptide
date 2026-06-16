# AI Coordination Chat — ZIPTIDE

**Purpose:** Shared handoff space for Tdog, Architect, GPT, and future LLM sessions.

**Date:** 2026-06-16

---

## GPT joining the chat

I am GPT joining the ZIPTIDE coordination thread.

I did a read-only first-pass audit of the repo and project direction. I did not change gameplay code. My conclusion: ZIPTIDE has a strong early foundation, but the next move should be structural rather than feature-heavy.

The project already has the right centerline:

- `docs/STATUS.md` as the LLM dashboard / source of truth.
- Locked contracts around modularity.
- Assembly definitions for major systems.
- Data-driven `WorldProfile` and `VisualThemeProfile` assets.
- Runtime systems for world dressing, bounds, respawn, and theme switching.
- Editor setup scripts to reduce Unity GUI work.
- EditMode tests and CI scaffolding.

The main risk is drift: adding exciting systems directly into scripts and Unity scenes until nobody knows which module owns what.

---

## Immediate repo truth to reconcile

I saw a mismatch between current docs and recent PR history.

`docs/STATUS.md` says Phase B is implemented but not wired into the committed scene, and still lists commit/push as the next task. Recent PR text says Milestone B scene wiring was fixed by adding `WorldRuntime` to `MilestoneA_GrabCube.unity`, referencing `DefaultWorldProfile`, and activating bounds, fall-respawn, theme switching, and default theme at play time.

**First priority when Claude usage returns:** verify actual repo/runtime truth and update `docs/STATUS.md` so future LLM sessions do not start from stale information.

Suggested first checks:

```powershell
git status -sb
git log --oneline -5
```

Then verify whether `MilestoneA_GrabCube.unity` contains `WorldRuntime` and whether Quest testing confirms Phase B behavior.

---

## GPT architecture read

Current runtime centerline:

- `WorldRuntime` applies the active `WorldProfile`.
- `WorldProfile` stores spawn, play area, respawn, default theme, and available themes.
- `WorldDirector` applies visual theme to the world.
- `SkyPlanetRig` creates procedural Quest-safe sky/planet visuals.
- `ThemeSwitchStation` creates placeholder selectable theme buttons.
- `FallRespawner` attaches to XR Origin and respawns through `WorldRuntime`.
- Editor setup scripts generate and apply the current starter scene/world setup.

This is good prototype structure. It now needs a scalable content pipeline.

---

## Core recommendation

Do not add the next big gameplay layer directly into the scene.

The next milestone should be:

# Milestone C — Content Registry + First Real World Template

Goal: prove ZIPTIDE can add a new world without touching gameplay code.

Acceptance criteria:

1. `docs/STATUS.md` accurately reflects current repo state.
2. `WorldRecipe` exists.
3. `WorldRegistry.asset` exists.
4. `VisualThemeRegistry.asset` exists.
5. `WorldRuntime` can load from a `WorldRecipe` path or ID, not just a loose scene reference.
6. One validation/test verifies every registered world has valid references.
7. One editor menu or batchmode method can create/apply a starter world from recipe data.
8. Existing Milestone A/B Quest behavior still works.

This milestone builds the machine that makes worlds. After that, worlds become data work instead of Unity scene surgery.

---

## Recommended content model

Everything that can multiply should become a recipe or registry entry.

Recommended recipe families:

- `WorldRecipe`
- `ShipRecipe`
- `EnemyRecipe`
- `BuildPieceRecipe`
- `EncounterRecipe`
- `RewardTable`
- `TravelNode`
- `ConquestNode`
- `VisualThemeProfile`
- `AudioThemeProfile`

Recommended registries:

```text
Assets/Ziptide/Content/Registries/
- WorldRegistry.asset
- ShipRegistry.asset
- EnemyRegistry.asset
- BuildPieceRegistry.asset
- EncounterRegistry.asset
- TravelRegistry.asset
- ConquestRegistry.asset
- VisualThemeRegistry.asset
- AudioThemeRegistry.asset
```

Runtime systems should resolve content through IDs and registries, not hardcoded scene names or prefab paths.

---

## Recommended folder target

```text
Ziptide/Assets/Ziptide/
  Core/
    Runtime/Contracts/
    Runtime/Events/
    Runtime/IDs/
    Runtime/Save/
  Content/
    Runtime/Recipes/
    Runtime/Registries/
    Runtime/Validation/
    Worlds/
    Ships/
    Enemies/
    BuildPieces/
    VisualThemes/
  Gameplay/
    Runtime/World/
    Runtime/Build/
    Runtime/Interaction/
    Runtime/Travel/
    Runtime/Garden/
    Runtime/Conquest/
  Visuals/
    Runtime/Themes/
    Runtime/VFX/
    Runtime/Procedural/
  Ship/
    Runtime/Rooms/
    Runtime/Upgrades/
    Runtime/Navigation/
  Platform.Quest/
    Runtime/Input/
    Runtime/Haptics/
    Runtime/Performance/
  Editor/
    Setup/
    Validation/
    Generators/
    Build/
  Tests/
    EditMode/
    PlayMode/
```

Do not physically move everything until tests pass. Treat this as the target organization.

---

## Scene strategy

Keep Unity scenes tiny. Treat them as boot shells.

Recommended scene model:

```text
Bootstrap.unity
- AppRoot
- XR Origin
- EventSystem
- ServiceRegistry
- RuntimeLoader

WorldTemplate.unity
- empty/default world shell
- WorldRuntime
- references one WorldRecipe

ShipTemplate.unity
- empty/default ship shell
- ShipRuntime
- references one ShipRecipe
```

Everything else should be spawned from recipes/factories.

---

## Parallel work lanes

### Tdog lane — repo truth and verification

Own:

- `docs/STATUS.md`
- `docs/04_TASK_QUEUE.md`
- `tools/`
- Quest build/install/smoke scripts
- Current Milestone A/B verification
- Immediate compile/runtime fixes found during testing

Do first:

1. Confirm branch and git status.
2. Confirm whether Phase B is wired in scene.
3. Run EditMode tests if possible.
4. Run Quest build/install/smoke if device is available.
5. Update `docs/STATUS.md` only after testing.

### Architect lane — structure and contracts

Own:

- `docs/00_LOCKED_CONTRACTS.md`
- `docs/01_ARCHITECTURE.md`
- `docs/06_SCHEMAS.md`
- `docs/08_TESTS.md`
- Recipe/registry design docs
- Validation design for registry data

Do first:

1. Draft Milestone C design.
2. Define minimal `WorldRecipe` and `WorldRegistry` contracts.
3. Define validation requirements before implementation.
4. Keep all design compatible with current Milestone A/B scene.

Avoid simultaneous edits to:

- `WorldRuntime.cs`
- `WorldProfile.cs`
- Unity scene files
- asmdefs
- `ProjectSettings`

One model should own those at a time.

---

## Next task order

1. Verify repo status and tests.
2. Fix stale `STATUS.md`.
3. Confirm Quest build/install/logcat workflow still works.
4. Create Milestone C design doc.
5. Add minimal `WorldRecipe` and `WorldRegistry`.
6. Add validation tests for registered world data.
7. Migrate current default world into the registry while keeping existing scene working.
8. Only then prototype the next major gameplay system.

---

## Non-negotiable rules

1. No hardcoded story/world names in engine code.
2. No gameplay system should reference prefab paths directly.
3. Every expandable content category gets a recipe and registry.
4. Every registry gets validation tests.
5. Scenes stay small. Content comes from data.
6. `docs/STATUS.md` must stay current after meaningful repo changes.
7. New systems must declare ownership: Core, Content, Gameplay, Visuals, Ship, Platform.Quest, or Editor.
8. Quest performance budget must become concrete before visual complexity grows.
9. One model owns scene files at a time.
10. Placeholder/stub systems must be clearly marked so nobody treats them as finished.

---

## GPT final take

ZIPTIDE should not become one giant scene full of clever scripts. It should become a small runtime engine that reads recipes, resolves registries, spawns factories, validates itself, and lets Terry add worlds, ships, enemies, build pieces, and game modes quickly.

Build the content machine first. Then make content with it.
