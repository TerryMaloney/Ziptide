# World Improvement recipes

Files ending in `.improvement.json` are the editable truth for repeatable world-quality rounds.
They are compiled headlessly by `WorldImprovementCompiler` after normal world patchers and before scene
save/audit.

## Resolution and history

1. Exact `sceneName` manifests beat generated-world defaults.
2. Within the exact or default class, highest `round` wins.
3. Within the same round, highest `recipeVersion` wins.
4. Two matching manifests with the same round and recipe version are a build error.
5. `excludedScenes` removes special scenes from a generated default.
6. Older recipes remain checked in as auditable history; they do not need destructive replacement.

This lets the project prove how a world improved over time while keeping one deterministic current recipe.

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

Portable presentation floor:

- `arrival_identity`
- `route_beacons`
- `ambient_motion`
- `horizon_frame`

Round 3 gameplay/story-density modules:

- `grounded_route`
- `discovery_nodes`
- `story_traces`

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
8. Review the generated evidence and record the HANDOFF stamp before closure.

## Evidence scoring

Compiler report schema 2 records every required aspect, its deterministic evidence score and the three weakest
aspects. This is coverage evidence, not an automated artistic verdict. Missing coverage remains a blocker;
thin coverage is a named warning and the next round's priority input.

## Hash/currentness law

The scene stamp is SHA-256 of normalized manifest JSON plus `WorldImprovementCompiler.CompilerVersion`.
Changing recipe content or compiler meaning changes the hash. `WorldImprovementAuditRules` blocks stale
or incomplete output.
