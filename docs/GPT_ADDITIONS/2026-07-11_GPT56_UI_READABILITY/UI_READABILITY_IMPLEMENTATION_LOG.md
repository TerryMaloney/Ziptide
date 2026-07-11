# UI READABILITY / REACH AUDIT IMPLEMENTATION LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“let’s start continuing to check off stuff”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — warning-only audit implementation in progress

## Why this is next

`CURRENT_EXECUTION_CHECKLIST.md` §7 names UI readability/reach as the next independent CI-only task after async travel and dashboard reconciliation. `EXCELLENCE_MAP.md` gap #6 requires a TextMesh sizing law and reach/target checks for interactive tiles.

## Exact scope

New files:

- `docs/design/UI_READABILITY.md`
- `Ziptide/Assets/Ziptide/Editor/Audit/UiReadabilityAuditRules.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/UiReadabilityAuditRulesTests.cs` + `.meta`

Shared additive touch:

- `Ziptide/Assets/Ziptide/Editor/Audit/WorldAuditRunner.cs`
  - exactly one call to `UiReadabilityAuditRules.Run(sceneReport)`;
  - runs for Boot and world scenes;
  - no existing audit order/behavior removed.

Closure docs:

- this log;
- `CURRENT_EXECUTION_CHECKLIST.md`;
- `EXCELLENCE_MAP.md` if the gate closes green.

## v1 contract

This is **WARN-only** until it proves low-noise against real generated scenes. It audits:

1. non-empty `TextMesh` labels using the established `characterSize × fontSize` effective-scale law;
2. diegetic `XRBaseInteractable` surfaces that contain a `TextMesh` label;
3. collider presence;
4. minimum two-dimensional target face size;
5. label attachment distance to the interactable collider bounds.

Initial constants are literal and tested:

- minimum effective TextMesh scale: `1.2`;
- minimum target face dimensions: `0.10m × 0.10m`;
- maximum label-to-target separation: `0.30m`.

These values are chosen below the already-shipped first-hour tile labels/targets, so the audit catches regressions without demanding a redesign. Promotion to blocker requires a later clean real-scene report and Terry approval.

## Collision / stop rules

- No runtime UI, input, rig, interaction, art, materials, prefabs, or scene YAML changes.
- No Picasso `Visuals/**`, Forge, water, art author/audit, or `SPRINT_ART.md` files.
- Do not alter interactable colliders or text automatically; report findings only.
- Three CI reds on this task triggers the circuit breaker.
