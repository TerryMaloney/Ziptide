# CREATURE BEHAVIOR COVERAGE GATE LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“what’s next”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 CLAIMED — evidence-backed behavior vocabulary gate in progress

## Why this is next

`docs/CURRENT_EXECUTION_CHECKLIST.md` §7 places the creature behavior-count gate immediately after the completed UI and haptic documentation rows. `docs/EXCELLENCE_MAP.md` requires every shipped creature id to carry at least three readable behavior states/modes instead of existing as one repeated chase loop.

## Protected scope

New files only:

- `docs/design/CREATURE_BEHAVIOR_COVERAGE.md`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureBehaviorCatalog.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs` + `.meta`
- this log

Closure-only docs after green:

- `docs/CURRENT_EXECUTION_CHECKLIST.md`
- `docs/EXCELLENCE_MAP.md`

## Contract

- Every committed `CreatureDefinition` under `Resources/Enemies` must map to a behavior profile.
- Every profile must expose at least three unique, non-empty readable modes.
- Generic stun/down states do not satisfy the vocabulary by themselves.
- Every listed mode carries an evidence token that must remain present in its real behavior source.
- Every profile carries a factory-wiring token that must remain present in `CityBuilder.MakeCreature`.
- Catalog ids and committed creature assets must be one-to-one; no silent orphan entries in either direction.
- A pre-build gate blocks APK generation on missing/low/drifted catalog coverage.
- EditMode tests enforce the same contract in ordinary branch CI.

## Collision rules

- Do not edit Picasso `Visuals/**`, Forge bodies/recipes, water, art audits, or `SPRINT_ART.md`.
- Do not alter creature stats, movement, damage, rewards, visual presentation, scene instances, or authored assets in this task.
- Do not count drone difficulty profiles as story creatures.
- Three CI reds triggers the circuit breaker.
