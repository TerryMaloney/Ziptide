# World Improvement recipes

Files ending in `.improvement.json` are the editable truth for repeatable world-quality rounds.
They are compiled headlessly by `WorldImprovementCompiler` after normal world patchers and before scene
save/audit.

## Resolution

1. An exact `sceneName` manifest wins.
2. Otherwise one `appliesToGeneratedWorlds: true` manifest may match a scene under
   `Assets/Ziptide/Scenes/Generated/`.
3. `excludedScenes` removes special scenes from a generated default.
4. Multiple matching exact or default manifests are a build error.

## Required fields

- `gameId` — portable framework namespace.
- `manifestId` — stable recipe identity.
- `sceneName` or `appliesToGeneratedWorlds`.
- `round` — player-facing quality floor revision.
- `recipeVersion` — recipe semantics revision.
- `seed` — deterministic base seed.
- `requiredAspects` — dimensions the enabled modules must cover.
- `requiredEvidence` — evidence kinds required to close the round.
- `modules` — ordered module specs with id, current version, intensity, object budget, seed offset and aspects.

## Current module ids

- `arrival_identity`
- `route_beacons`
- `ambient_motion`
- `horizon_frame`

Unknown ids, stale module versions, duplicate ids, disabled-only aspect coverage and negative budgets are
rejected before scene mutation.

## Adding a module

1. Implement `IWorldImprovementModule`.
2. Register it in `WorldImprovementModuleRegistry`.
3. Keep all outputs under the supplied module root.
4. Return the exact owned object count.
5. Leave collision off unless collision is the explicit feature and has its own audit.
6. Add pure tests and a deliberately broken scene fixture.
7. Add a manifest only to worlds where the module belongs.
8. Review the generated sheet and record the HANDOFF stamp before closure.

## Hash/currentness law

The scene stamp is SHA-256 of normalized manifest JSON plus `WorldImprovementCompiler.CompilerVersion`.
Changing recipe content or compiler meaning changes the hash. `WorldImprovementAuditRules` blocks stale
or incomplete output.
