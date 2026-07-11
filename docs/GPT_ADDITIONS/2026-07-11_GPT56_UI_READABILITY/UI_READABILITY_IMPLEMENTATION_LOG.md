# UI READABILITY / REACH AUDIT IMPLEMENTATION LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“let’s start continuing to check off stuff”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ CODE + UNITY CI GREEN — WARN-ONLY REAL-SCENE REPORT / DEVICE VALIDATION PENDING; FILE CLAIM RELEASED

## Why this was next

`CURRENT_EXECUTION_CHECKLIST.md` §7 named UI readability/reach as the next independent CI-only task after async travel and dashboard reconciliation. `EXCELLENCE_MAP.md` gap #6 required a TextMesh sizing law and reach/target checks for interactive tiles.

## Delivered files

- `docs/design/UI_READABILITY.md`
- `Ziptide/Assets/Ziptide/Editor/Audit/UiReadabilityAuditRules.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs` + `.meta`
- this log

## Shipped v1 contract

The audit is **WARN-only** until it proves low-noise against real generated scenes. It checks:

1. non-empty `TextMesh` labels using the literal `abs(characterSize) × max(1, fontSize)` effective-scale law;
2. diegetic `XRBaseInteractable` surfaces only when they contain a `TextMesh` label, so weapons and ordinary props are not misclassified as UI;
3. collider presence;
4. minimum two-dimensional target-face size;
5. label attachment distance to the interactable collider bounds.

Literal tested thresholds:

- minimum effective TextMesh scale: `1.2`;
- minimum target face dimensions: `0.10m × 0.10m`;
- maximum label-to-target separation: `0.30m`.

Finding codes:

- `UI_TEXT_TOO_SMALL`
- `UI_INTERACTABLE_NO_COLLIDER`
- `UI_TARGET_TOO_SMALL`
- `UI_LABEL_DETACHED`

## Build integration

The initial claim proposed one shared `WorldAuditRunner` call. The final implementation deliberately uses an independent `IProcessSceneWithReport` build-scene processor instead:

- every scene Unity actually processes is evaluated;
- findings log as `ZIPTIDE: UI_AUDIT scene=<...> code=<...> path=<...> message=<...>`;
- no shared audit runner, art audit, runtime surface, scene, prefab or YAML file changed;
- v1 cannot fail or mutate a build.

This narrower hook removes a shared-file collision while preserving the intended build-time coverage. Promotion to blockers or inclusion in the main Markdown audit report requires a clean generated-scene report and Terry approval.

## Tests

- constants and effective TextMesh scale are pinned literally;
- target face requires two usable dimensions—a long needle does not pass;
- small non-empty text warns while empty placeholders are ignored;
- text-bearing interactable without a collider warns;
- undersized target and detached label warn independently;
- a shipped-size attached tile produces no finding for its path;
- processor order and WARN-only severity are pinned.

## CI proof

- audit core/meta tested green: `22fdb67bebe1b87a99ef70d2bb941102e7b0eef6`, run `29167950802`;
- full processor/tests commit: `8a66d72ec1fc515395c287685e3846531013bf68`;
- corrective test commit: `a6803aba92c8f2e27c3f7f6982be4d5656adab96`;
- final CI run: `29168358234`;
- Unity EditMode: `success`;
- project-contract reports: `success`;
- Android: skipped as expected for an ordinary branch push;
- overall: `GREEN`.

Circuit breaker: `1/3` red. The red was test-assembly-only: the test imported XRI directly while `Ziptide.Tests.EditMode.asmdef` intentionally lacks that package reference. Production audit code already compiled. The correction creates the installed XRI component reflectively inside tests without widening the whole test assembly dependency surface.

## Terry / next generated-build evidence

During the next full author/build pass:

1. inspect `ZIPTIDE: UI_AUDIT` warnings in the build log;
2. confirm the W000 Home/comfort/helm surfaces are legible and selectable at normal arm/head distance;
3. report any warning that looks false-positive before changing thresholds;
4. fix actual authored sizes/targets through their owner/author, never from the audit;
5. promote no warning to a blocker until the generated-scene report is clean and device feel agrees.

## Closure

The quality-file claim is released. The implementation is code/CI green, changes no runtime behavior, and closes the missing UI readability/reach audit at WARN-only maturity. The remaining gate is real generated-scene/device calibration, not more blind code.
