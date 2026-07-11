# CREATURE BEHAVIOR COVERAGE GATE LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** Terry, 2026-07-11 (“what’s next”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 MERGED TO ONE CANONICAL GATE — FINAL UNITY CI VERIFICATION PENDING; CLAIM HELD

## Why this was next

`docs/CURRENT_EXECUTION_CHECKLIST.md` §7 places the creature behavior-count gate immediately after the completed UI and haptic rows. `docs/EXCELLENCE_MAP.md` requires every shipped creature id to carry at least three readable active states instead of one repeated chase loop.

## Collision and resolution

Two implementations began concurrently despite this claim:

- the original claimed implementation added source-evidence tokens, exact factory tokens and an APK pre-build blocker;
- the concurrent implementation added a cleaner Content-layer catalog with expected archetype, explicit telegraph/counter states and a separate disabled/resolved state.

They were deliberately merged instead of leaving two sources of truth.

**Canonical source:**

- `Ziptide/Assets/Ziptide/Content/Runtime/Definitions/CreatureBehaviorReadabilityCatalog.cs`
- `docs/design/CREATURE_BEHAVIOR_READABILITY.md`

The duplicate Gameplay catalog and duplicate design page were deleted.

## Final scope

- canonical Content catalog + metadata;
- `Ziptide/Assets/Ziptide/Editor/Audit/CreatureBehaviorAuditRules.cs` + metadata;
- `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorReadabilityTests.cs` + metadata;
- `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureBehaviorAuditRulesTests.cs` + metadata;
- canonical design contract;
- this implementation log.

No creature behavior, stats, rewards, damage, movement, visual, Forge, scene, prefab or art-author files changed.

## Final contract

Every committed `CreatureDefinition` under `Resources/Enemies` must have exactly one canonical profile with:

- at least three unique active readable states;
- a telegraph state drawn from the active vocabulary;
- a counter/vulnerability state drawn from the active vocabulary;
- a separate non-lethal disabled/resolved state;
- expected archetype and resolvable `CreatureBehaviorBase` type;
- behavior-source path and one exact source token per active state;
- exact `CityBuilder.MakeCreature` factory evidence.

The catalog and committed assets are one-to-one. Drone difficulty profiles are excluded because they are not `CreatureDefinition` assets.

## Enforcement

1. `CreatureBehaviorReadabilityTests` verifies profile structure, assets, filename/id parity, archetype, type and factory routes.
2. `CreatureBehaviorAuditRulesTests` verifies source evidence, one-to-one coverage and the complete project audit.
3. `CreatureBehaviorBuildGate` runs before APK generation and throws when any blocker exists.

Audit prefix: `ZIPTIDE: CREATURE_BEHAVIOR_AUDIT`.

## Current roster

- `swarm_bug`: patrol/orbit · gather · dart
- `tendril`: wall patrol · ripple telegraph · lunge · return
- `light_grazer`: dark growth · dark approach · lit recoil
- `witness_mite`: unobserved idle · unobserved stalk · observed freeze
- `tether_swarm`: cluster weave · engaged standoff · exposed tether node
- `husk_molter`: stalk · molt escape · cooldown vulnerability
- `warden`: watch · warn · arrest/disengage · ally calm

## Verification state

A final ordinary CI run is intentionally being triggered by this commit after the merged head stabilized. Do not close the checklist row or release the claim until `docs/CI_VERDICT.md` records this exact head green, or its direct generated verdict-only child.

Circuit breaker reds for the merged creature-gate task: `0/3` at this stamp.
