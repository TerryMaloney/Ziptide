# First-contract resume integration — implementation specification

This extends CHECKPOINT_FOUNDATION_20260929.md. It is a design, not shipped runtime behavior.
Initial slice: W000 Cast Off (helm, manifest, coupler). Its zero-credit reward is intentional;
receipt tests must also exercise nonzero multi-resource rewards. ToxicCity's paid contract follows
using the same transaction owner. Do not silently enable all generated worlds.

## Transaction owner

Use the existing PlayerProfile and RewardRouter. A world record holds mission progress and a
separate unpruned completion receipt list. A receipt uses world/job identity for one-time contracts,
plus explicit run identity for repeatable contracts. No receipt lookup may depend on the 500-entry
ledger. Unknown replay policy is not an invitation to choose one dynamically.

Before changing any live balances, validate the complete reward list and aggregate repeated resource
IDs; reject NaN/infinity, negative amounts, invalid existing balances and overflow. Zero amounts are
valid no-ops (W000 authors credits=0). No invalid second reward may leave the first reward paid.
After validation, apply through RewardRouter, flags and receipt in the same synchronous profile
mutation. No gameplay callbacks occur in the middle. Persist progress and physical state alongside
that receipt through the existing writer. Never replace live WorldState objects wholesale: garden,
mine and belt runtimes can hold references to them.

On failed disk write: retain one coherent in-memory state, expose failure, retry serialization without
re-granting rewards. On restart: last valid main/backup determines both the balance and progress.
Retain rejected checkpoint data for diagnosis/migration. A partial/inconsistent snapshot must not be
silently used to spawn an impossible scene or grant rewards again.

## Physical-state and event ordering

- Each placed collectible gets an authored placementId, separate from itemId. Reject duplicate/missing
  placement identities before opting the pack into persistence. Repeated item types remain allowed.
- Pickup callback writes consumed placement and its flag before forwarding the logical collect event.
  A duplicate placement event contributes zero progress. Save after the whole mutation, not midway.
- Persist repair stages by machineId. The presentation component receives a silent restore method;
  SaveSystem references stay outside RepairableMachine. Restoring Part hides/removes the attached
  panel, Power also removes the loose part and activates the switch, Running applies completed
  visuals. Never replay repair haptics/audio, stage signals or ReportRepair during restore.
- A transition to Running must update stored physical state before forwarding the logical completion
  report. Subscribe/order through the existing JobDirector; do not add a second repair-state owner.
- Defer disk save until the logical/physical event chain has settled. StepChanged may occur before
  early banks drain, so it is not by itself a valid commit boundary.

## Lifecycle and rollout

Initialize checkpoint state before spawning pickups/machines. Validate the owning scene and configured
job against stored world/run/definition identities; destination packs are not owners. Restore logical
state silently, spawn physical state, bind presentation, then accept interactions.

Legacy completed flags need an explicit migration rule; absence of a new receipt must not automatically
repay an old completed campaign contract. Before activation, decide whether to seed a receipt without
payout or preserve the older contract as legacy-only. Test the selected rule with old-save fixtures.

New Game/Load must invalidate cached profile references. A departing old director cannot append its
old run to a newly created profile. Prefer a profile identity guard and an explicit reset notification
at the existing save owner, including Load, rather than assuming only HomeHub triggers replacement.

Before opting W000 into persistence, author step IDs `reach_helm`, `collect_manifest`, `repair_coupler`
and placement ID `guild_manifest_primary`; validate the actual step kind/target against each ID.
Authored names must survive reordering; use explicit metadata, not automatic index suffixes. First
contract policy: one-time economic completion; reselecting a completed contract restores completion,
not a new reward-bearing run. Do not apply this proposed policy to unrelated jobs without authoring it.

## Required proof

- Partial collect/repair, pre-accept actions, every meaningful repair stage, leave/re-enter and reload.
- Completed contract restored with correct board/machine/pickup state and no payout callbacks.
- Duplicate collect/repair/completion events; receipt survives ledger pruning.
- Multi-resource reward prevalidation, including duplicate resource IDs, invalid second entry and overflow.
- Save failure, retry, valid backup recovery, and old completion flag migration.
- New Game and Load with an existing director; no old profile writes into the new one.
- Authoring rerun preserves stable IDs and enables only the intended contract.
- Exact-source Unity EditMode + scene audit. Headset suspend/quit/comfort acceptance remains separate.
