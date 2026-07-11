# FH-S05 IMPLEMENTATION LOG — FIRST-HOUR CREATURE RESOLUTION ADAPTER

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11 (“Continue ZIPTIDE from the latest GitHub state”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE — FILE CLAIM POSTED  
**Envelope:** `docs/first_hour/envelopes/FH-S05-CREATURE-RESOLUTION.json`

## Goal

Expose the existing non-lethal creature disable outcome as a neutral owner signal without changing combat, health, rewards, ecology, respawn, visuals, or encounter orchestration:

- `FH_COUNTER_SIGNATURE_CREATURE` → `SIGNATURE_CREATURE_REDIRECTED_OR_DISABLED`.

## Claimed files

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/CreatureRuntime.cs`
- `Ziptide/Assets/Ziptide/Tests/EditMode/CreatureDisabledSignalTests.cs` + `.meta`
- this log

## Additive touch announcement

The only planned runtime touch is on the existing owner, immediately after its established `_down = true` transition. The adapter will expose `IsDisabled`, publish one neutral disable notification per down cycle, and identify the existing runtime/species so the later orchestrator can filter the designated signature instance.

## Locked exclusions

- no damage, health, stun, reward, ecology, respawn, animation, audio, collider, AI, or visual changes;
- no first-hour/tutorial fields in `CreatureRuntime`;
- no second creature state machine or event bus;
- no signature-instance filtering inside the creature owner;
- no profile writes, travel, RILL, scanner, job, or orchestration ownership.

## Planned tests

- first disable publishes once;
- damage or disable handling while already down does not repeat;
- respawn/reset permits a later owner event;
- `IsDisabled` remains a direct view of the existing `_down` state;
- reward and ecology calls remain after the established down transition and preserve their order;
- source guard confirms no changes to damage, health, reward, ecology, respawn or visuals.

## Diagnostics

`ZIPTIDE: FIRST_HOUR_CREATURE id=<id> matched=<bool> outcome=disabled`

The owner event remains neutral; `matched` filtering and this first-hour diagnostic belong to the later orchestrator, not `CreatureRuntime`.

## Fallback

No subscriber leaves creature behavior unchanged. Non-signature disables remain valid neutral events but never advance the first hour until the orchestrator filters the designated instance.

## Collision rule

Do not edit the claimed files until this log is closed or explicitly released.