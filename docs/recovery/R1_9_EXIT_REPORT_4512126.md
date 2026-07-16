# R1.9 Exit Report — Golden Fallback/Material Exposure

**Review date:** 2026-07-16  
**Exact source SHA:** `4512126bada89425318a9c92e96e8859c282b352`  
**Recovery PlayMode run:** `29503078505`  
**Workflow artifact:** `8377390266` (`recovery-playmode-r1-results-4512126bada89425318a9c92e96e8859c282b352`)  
**Artifact digest:** `sha256:205be42c2b71d9a0797fe7954a9a1cc787dda4c47835a877eba148a65ff9aba4`  
**NUnit:** **40 total / 40 passed / 0 failed / 0 skipped / 0 inconclusive**  
**Verdict:** **R1.9 passed and was independently sampled. R1.10 may begin. No Quest authorization.**

## What R1.9 proves

The active Golden runtime surface was inspected after the settled player-view capture in each locked route scene. The gate enumerates every enabled renderer on an active scene object, including `DontDestroyOnLoad` objects, and blocks:

- null material slots;
- materials emitted by the retired `RuntimeMaterialFixer_*` contract;
- null shaders;
- Unity `InternalErrorShader` surfaces.

It complements, rather than replaces, the existing Golden exposure profile, runtime census, runtime-artifact guard and player-surface policy that exclude forbidden owners, prototypes, Photon/DebugHUD artifacts and the legacy CreditsHud.

## Raw inspected artifacts

| Settled view | Active scene | Active profile | Active renderers | Material slots | Blockers |
|---|---|---|---:|---:|---:|
| Home Hub | `_Boot` | `GoldenSlice` | 55 | 55 | 0 |
| W000 spawn | `W000_DriftIn` | `GoldenSlice` | 196 | 202 | 0 |
| ToxicCity spawn | `ToxicCity` | `GoldenSlice` | 546 | 549 | 0 |

The JSON and Markdown artifacts are stored under:

`playmode-test-results/recovery-fallback-surfaces/`

The raw runtime log independently reports all three `RECOVERY_FALLBACK_SURFACE_OK` records with the same counts.

## Canary proof

`RecoveryFallbackSurfaceAuditTests.ActiveKnownFallbacksAreBlockers_DisabledRenderersAreIgnored` passed and proves:

- an active `RuntimeMaterialFixer_*` material is detected;
- an active null material slot is detected;
- disabled renderers are not falsely treated as exposed runtime surfaces;
- the incoming exposure profile is restored after the canary.

## Real defect caught and repaired

The first R1.9 run (`29500885566`, source `65ea8923`) executed 39 tests and failed the new gate on exactly two W000 surfaces:

- `Pistol/ForgeVisual/prefab(Clone)`;
- `TaserDartGun/ForgeVisual/prefab(Clone)`.

Both active renderers had null materials. The source cause was a real ownership gap:

1. build patching had serialized baked Forge children whose material assets live in the regenerated/gitignored `Resources/ForgeBaked` tree;
2. source-only PlayMode could therefore resolve those stale child material references as null;
3. the runtime flat-color fallback created a valid parent renderer but did not retire the previously owned child hierarchy;
4. scene-authored `ItemRuntime` objects had no Awake-time reapplication seam, while ItemFactory-created objects did.

The bounded repair:

- makes `ForgeVisualApplier` replace its entire owned child hierarchy before choosing baked or current flat-color representation;
- immediately deactivates stale children before deferred runtime destruction;
- retires parent fallback components when a baked representation is used;
- makes scene-authored `ItemRuntime` instances reapply their serialized Forge recipe during Awake;
- leaves ItemFactory as the single Forge caller for runtime-created items whose definition is not assigned until `Init`;
- adds the exact stale `prefab(Clone)` + null-material regression canary.

The second attempt (`c24c00c`, run `29502453081`) correctly produced zero tests because the new canary crossed the intentionally narrow PlayMode assembly boundary. That compile failure was fixed without adding assembly references: the test now drives the public Gameplay seam and loads the real resource as a Unity object. The final exact source compiled and passed all 40 tests.

## Independent log sampling

The final artifact contains:

- no failed or skipped NUnit result;
- no null-reference, assertion, travel-failure or XRI-not-ready record;
- the Forge ownership canary passing;
- `FORGE_APPLIED` records showing stale owned children replaced in the real route;
- clean material-surface records for all three settled views.

## Limits preserved

R1.9 does **not** claim that every active surface is approved final art or currently camera-visible. The independently reviewed screenshots still contain dark/primitive presentation debt. R1.9 proves that known emergency material replacement output, missing material slots and broken shaders are not exposed on the tested Golden runtime surface.

## Promotion

R1.9 is promoted on exact source `4512126bada89425318a9c92e96e8859c282b352` at the PlayMode/reference-renderer evidence level.

Next required order:

1. land the current-base R1.10 post-health-sweep performance/leak route;
2. require the expanded unchanged suite to pass on one exact SHA;
3. independently inspect all three performance artifacts;
4. run the exact-SHA Golden Android lane and inspect patch, bake, shader, world-audit and APK evidence;
5. independently sample the final candidate claims;
6. only then authorize Terry's bounded Quest checklist.