# WORLD IMPROVEMENT FRAMEWORK
## A portable, repeatable quality round for any generated world

**Status:** implementation in progress on `gpt/world-improvement-framework-round2`.
**Authority:** Terry's direction that every world should receive repeated across-the-board improvement
rounds, and that the finished factory must be reusable for another game.

This framework turns the successful ToxicCity full-send into a repeatable manufacturing step. It does
**not** claim that one generic pass makes every world excellent. It guarantees that each declared round
is deterministic, bounded, current, audited and evidenced, and that biome-specific modules can be added
without changing the compiler.

---

## 1. The portable contract

The reusable framework has seven layers:

1. **Manifest** — `docs/worldimprovements/*.improvement.json` declares the game id, target scene,
   improvement round, recipe version, colors, module versions, budgets, quality aspects and required
   evidence. Exact-scene manifests override the standard generated-world default.
2. **Module seam** — `IWorldImprovementModule` receives only a scene context plus manifest data and
   returns an owned object count and summary. Modules do not know the build pipeline or other modules.
3. **Deterministic compiler** — `WorldImprovementCompiler` runs after existing world patchers, replaces
   one owned root, executes enabled modules in manifest order and writes a compile report.
4. **Recipe-hash law** — the compiler stamps SHA-256(manifest JSON + compiler version) into the scene.
   A recipe or compiler change therefore changes identity; stale output cannot masquerade as current.
5. **Excellence audit** — `WorldImprovementAuditRules` blocks stale hashes, missing/duplicate/empty
   modules, version drift, object-budget overruns, uncovered quality aspects and undeclared evidence.
6. **Evidence blackboard** — `Builds/Reports/world_improvement_compile.json`, the normal audit report,
   NUnit artifacts, referenced sheets and device verdicts let another model reconstruct exactly what
   was expected and what passed.
7. **Ratchet** — every device-found class becomes a fix + gate + HANDOFF line. The framework should
   encounter new classes over time, never repeatedly rediscover the same class.

Only the **recipes and module implementations** are game-specific. The manifest schema, compiler,
stamps, hash law, budgets, audit and round protocol are portable.

---

## 2. Current standard modules

| Module | Quality dimensions | Contract |
|---|---|---|
| `arrival_identity` | Identity, perception | A readable landmark and world label near supported arrival space; no gameplay collision. |
| `route_beacons` | Navigation, spatial safety, perception | Grounded visual markers beside spawn, POIs and critical travel controls; never on the route and never collidable. |
| `ambient_motion` | Ambience, perception | A bounded set of deterministic moving motes; one runtime owner, no resource allocation, no collider motion. |
| `horizon_frame` | Identity, atmosphere, perception | Low-cost distant silhouettes that add depth and visual rhythm without blocking traversal. |

These are intentionally biome-neutral. Toxic rivers, city street life, specialized vehicles, creature
sets, weather events, gardens and ride-scenes are **specialized modules** that a manifest opts into only
when the world's data supports them.

### Module law

Every module must:

- be deterministic from manifest seed + `seedOffset`;
- own one named root and leave a `WorldImprovementModuleMarker`;
- be idempotent because the compiler replaces the framework root;
- declare a current version and reject a stale manifest version;
- report object count and remain within manifest budget;
- declare the quality aspects it covers;
- create no hidden traversal collision unless collision is the module's explicit purpose and separately audited;
- add a deliberately broken fixture proving its blocker before promotion;
- emit `ZIPTIDE:` diagnostics for meaningful build/runtime state.

---

## 3. The improvement-round protocol

A round is a controlled floor-raising operation, not an open-ended polish sprint.

### A. Select the floor

Read `EXCELLENCE_MAP.md`, generated audit evidence and the latest device notes. Select weak neighboring
aspects rather than polishing the strongest system. Every round must touch multiple dimensions, but no
aspect is included without a checkable target.

### B. Declare before building

Create or revise a manifest:

- increment `round` when the player-facing quality floor rises;
- increment `recipeVersion` when recipe meaning changes without changing the compiler;
- name every module and exact module version;
- set a hard object budget;
- list the quality aspects and evidence kinds required to close;
- use exact-world recipes for special behavior and the generated default only for genuinely universal behavior.

### C. Compile through one seam

The canonical build order is:

`WorldSpecCompiler / world patchers -> WorldImprovementCompiler -> save scenes -> global audits -> APK`.

No module may require a Unity menu, hand-edited scene YAML or deletion of a stale asset. A build either
contains the recipe's exact output or fails loudly.

### D. Pass all machine dimensions

A round is machine-green only when:

- logical tests pass;
- profile-aware travel gates pass;
- route continuity and reach envelopes pass;
- the improvement stamp/hash/modules/aspects pass;
- scene performance budgets pass or remain explicitly warning-level debt;
- visual coverage and referenced sheets exist;
- the relevant PlayMode route passes;
- the exact Android artifact carries the same source and recipe identity.

### E. Spend human headset time only on feel

The headset verdict covers comfort, fun, readability, scale perception, atmosphere and whether the world
feels authored. Those cannot be guaranteed by code. Every failure is logged as a new ratchet candidate.

### F. Lock and score the next round

Record PASS / FAIL / NOT OBSERVED by aspect. Update the excellence map. The next round selects the lowest
unlocked dimensions and dependencies, not whichever feature is easiest to embellish.

---

## 4. Relationship to the larger game factory

This framework solves **repeatable quality improvement of world output**. It is one layer of the larger
five-layer factory described in `PERCEPTUAL_GATE_PROGRAM.md`:

- spec-is-truth;
- deterministic generators;
- gate per quality dimension;
- evidence blackboard;
- ratchet.

The Architect's `WORLD_ASSEMBLY_READINESS.md` identifies the remaining content-factory gaps that are not
pretended away here:

1. flag-graph validator;
2. `WorldContentGenome` for job/encounter/reward purpose;
3. comfort-legal ride-scenes;
4. per-world voice kits;
5. Lore Forge slots and chain validation;
6. the final world assembly-line runbook.

The Architect's `LLM_FIRST_BUILD_PIPELINE.md` adds the production requirement: all create-only authors
must eventually obey the same recipe-hash/currentness law, with a bot-committed bake state and a
pre-flight impact report. The World Improvement framework implements that law for its own outputs now;
expanding it to the older author/library set is the next factory-hardening stage.

---

## 5. Porting this framework to another game

Copy these generic pieces:

- `Content/WorldImprovement/*`;
- `Editor/WorldImprovement/*`;
- `Editor/Audit/WorldImprovementAuditRules.cs`;
- the EditMode framework tests;
- the build-hook placement and report upload;
- this manifest/runbook structure.

Then replace only:

- game id and scene recipes;
- module registry implementations;
- quality aspect vocabulary if the new genre needs different dimensions;
- performance budgets;
- device route and human feel checklist.

A fantasy game could register shrine landmarks, firefly ambience and horse-route markers. A science-
fiction game can register industrial pylons, toxic rivers and hover vehicles. Both use the same manifest,
hash, compiler, marker, budget, audit, evidence and ratchet machinery.

---

## 6. Definition of done for framework v1

- PG-1 through PG-4 compile and pass with deliberately broken fixtures.
- W000, ToxicCity and standard generated worlds resolve the correct Round 2 manifest.
- Compiling twice leaves exactly one owned root and stable hash.
- Standard modules create no colliders or real-time lights.
- Every required module/aspect/evidence kind is audited.
- `world_improvement_compile.json` lists every affected scene and object count.
- Ordinary CI and generated-scene audit are green on the exact source.
- Recovery PlayMode and Golden Android are green for the locked route.
- Referenced visual sheets are reviewed and stamped.
- Device test ledger covers old regressions plus the new Round 2 additions.
