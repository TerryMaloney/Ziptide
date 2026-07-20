# WORLD IMPROVEMENT ROUND RUNBOOK
## From evidence to a repeatable higher-quality world

This is the operating sequence for every future improvement round. It is intentionally independent of a
specific world or genre.

## 0. Recover truth

- Read newest `HANDOFF.md`, `CI_VERDICT.md`, generated recovery observations and device logs.
- Confirm the exact green source SHA.
- Read the relevant `EXCELLENCE_MAP.md` rows and design authorities.
- Do not begin feature writes while a required gate is red.

## 1. Select round scope

Choose the lowest linked quality dimensions from evidence. A standard round should improve at least
three dimensions and must state what is deliberately excluded.

Record:

- target worlds;
- starting state;
- target state;
- required modules;
- budgets;
- machine checks;
- headset observations;
- known inherited warnings.

## 2. Declare manifests first

Create or update `docs/worldimprovements/*.improvement.json`.

- exact manifests for world-specific behavior;
- generated default only for universally appropriate modules;
- increment `round` for a player-facing floor increase;
- increment `recipeVersion` for recipe meaning changes;
- use current module versions;
- cover every `requiredAspect` with at least one enabled module;
- require logic, spatial, perceptual, performance and device evidence unless a documented product law says otherwise.

Run/expect `WorldImprovementRecipeFilesTests` before implementation proceeds.

## 3. Build modules

For each new module:

- implement the portable interface;
- use only the supplied context and manifest spec;
- deterministic seed = manifest seed + seed offset;
- own one module root;
- return exact object count;
- leave a marker;
- obey budget;
- create no accidental collision, lights or resource leaks;
- log a meaningful `ZIPTIDE:` build/runtime event;
- add a deliberately broken fixture.

## 4. Compile through the canonical seam

The build runs existing world authors/patchers first, then `WorldImprovementCompiler`.

Expected diagnostics:

- `ZIPTIDE: WORLD_IMPROVEMENT_MODULE ...`
- `ZIPTIDE: WORLD_IMPROVEMENT_COMPILED ...`
- `ZIPTIDE: WORLD_IMPROVEMENT_REPORT ...`

Expected artifact:

- `Builds/Reports/world_improvement_compile.json`

No Unity menu or hand-edited scene/prefab YAML is permitted.

## 5. Machine gates

Required order:

1. Static recovery/ownership checks where applicable.
2. Unity EditMode authoritative XML: total = passed; zero failed/skipped/inconclusive.
3. Generated-scene patch and world audit: zero blockers.
4. Inspect world improvement compile report: expected scenes, manifest ids, hashes, modules and object counts.
5. Recovery/route PlayMode suite where the round touches the locked route.
6. Referenced visual sheet generation and human/LLM review stamp.
7. Exact-SHA Android artifact and independent build-profile/hash inspection.

Three CI reds on the same task invoke the circuit breaker and require a HANDOFF diagnosis before more
implementation.

## 6. Headset ledger

Carry forward every prior unresolved or recently fixed device check. Add the new round checks. Mark each:

- PASS
- FAIL
- NOT OBSERVED

The device pass judges feel, comfort, readability, scale, atmosphere and authoredness. Correctness that
could have been machine-gated but escaped becomes a new ratchet task.

## 7. Close the round

A round closes only when:

- exact source and artifact identities are recorded;
- every required evidence kind exists;
- generated sheets are reviewed and stamped;
- device ledger is recorded;
- `EXCELLENCE_MAP.md` state and gate coverage are current;
- HANDOFF contains Did / Next / Heads-up / Commit plus every ratchet line;
- the next round is selected from the weakest remaining dimensions.

## Round 2 pilot

Current pilot recipes:

- `W000_DriftIn.round2.improvement.json`
- `ToxicCity.round2.improvement.json`
- `_generated-standard.round2.improvement.json`

Portable modules in the pilot:

- arrival identity;
- route beacons;
- ambient motion;
- horizon framing.

Specialized Round 1 systems—Toxic River, city street life, vehicle fleet and hero ship—remain valid
module candidates but are not forced into the generated default until they are extracted behind
biome-appropriate manifest schemas and their own portable tests.
