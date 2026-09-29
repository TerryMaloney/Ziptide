# State ownership audit — 2026-09-29

Source baseline: `6c2abc607f2037041d65813a43b126a1d5d52409` (runtime last verified by Unity CI at `0f295f58`, run 36576938375). This is a source audit, not interruption or headset proof. ORG-02 and SAVE-01 remain open: the mission/economy path is mapped below; ship, vehicles, encounters and all profile-reset consumers still need inspection.

Paths below are relative to `Ziptide/Assets/Ziptide/`.

| State | Existing owner / source | Actual lifetime and restore behavior | Required follow-up |
|---|---|---|---|
| Profile, flags, resource balances | `Core/Runtime/Persistence/PlayerProfile.cs`; `Gameplay/Runtime/Persistence/SaveSystem.cs` | In-memory profile; JSON on explicit save, pause, quit and travel autosave. Schema 3. | Do not create a second profile or wallet. |
| Disk commit / recovery | `Core/Runtime/Persistence/SaveFileStore.cs`, `ProfileSerializer.cs` | Temporary file, main/backup replacement; load valid main then valid backup. | Test failure outcomes through the existing writer; do not equate file replacement with guaranteed power-loss durability. |
| Job index, counters, early collect/repair banks | `Gameplay/Runtime/Jobs/JobRuntime.cs`, `JobDirector.cs` | Scene/runtime memory only. StartJob resets counters; banks survive StartJob in the same runtime. No capture/restore. | Add snapshot and mutation notification, including bank-only changes. |
| Job rewards and completion flags | `Content/Runtime/Jobs/JobRewards.cs` | Resources go through RewardRouter; flag assignment is idempotent, resource grants are not. | Explicit replay policy and durable scoped receipts. |
| Reward/spend history | `Core/Runtime/Economy/ResourceLedger.cs` | Profile-backed, latest 500 entries only. | Never use this capped list as permanent completion proof. |
| World completion | `JobDirector.OnJobCompleted`, `WorldGating` | Grants pack flags on any job completion. | Distinguish side-job, required-job and world completion ownership (SAVE-06). |
| Physical collectibles | `Gameplay/Runtime/Story/CollectibleRuntime.cs`; `JobDirector.CreateCollectibles` | Local collected latch, object destruction; optional profile flag. Director creates placements again on scene entry. | Audit Init and stable placement identity before implementing consumed-state restore; item type is not placement identity. |
| Repair stages | `Gameplay/Runtime/Story/RepairableMachine.cs` | Panel/part/power stages are runtime state. Completion reports to director. | Restore via an adapter; preserve tests that keep SaveSystem out of the presentation component. |
| Story choices | `Gameplay/Runtime/Story/ChoiceStation.cs` | Flags in profile; Init restores the resolved choice. | Include choice mutation in checkpoint triggers; handle inconsistent dual flags deliberately. |
| Mines / gardens | `MiningRigRuntime.cs`, `GardenPlotRuntime.cs` under `Gameplay/Runtime/Story`; profile MineState/PlotState | Existing profile overlays and offline resolution. | Reuse them; validate relation to job counters and claims. |
| Factory / belts | WorldState factory and beltFloors; `Gameplay/Runtime/Automation/BeltFloorRuntime.cs` | Existing profile state; belt init restores, edits autosave. World lookups use scene name. | Reconcile packId versus scene-name world keys before adding mission data. |
| Holstered inventory during travel | `Gameplay/Runtime/Inventory/InventoryState.cs`; `Player/PlayerRigPersistence.cs` | Static SavedItem list, consumed by restore; itemId/slotId recreated through ItemFactory and XRI socket selection. This is process memory, not disk inventory. | Report-only ownership boundary. Preserve holstered-only travel; separate restart persistence task. |
| Runtime inventory root | `Gameplay/Runtime/Inventory/InventoryPersistence.cs` | Transform adoption / runtime hierarchy. | Its name does not establish disk persistence. |
| Travel | `Gameplay/Runtime/World/TravelCoordinator.cs` | Sole scene transition owner; invokes profile autosave before travel. | Checkpoint adapter must cooperate with this owner, not introduce another scene loader. |
| First-hour progression | `Gameplay/Runtime/Tutorial/FirstHourDirector.cs` | Core initialized from flags; NewGame callback accepts an event without rebuilding the core. | Investigate stale state across profile replacement; do not claim a proven player-facing bug without lifecycle test. |

## Confirmed defects and boundaries

1. Saving a profile does not capture JobRuntime. Restored flags/resources and scene-memory objectives therefore have no common mission checkpoint.
2. `JobRewards.Grant` permits repeated resource grants. Job IDs are unique only within a pack; there is no declared replay policy. A blanket one-time guard would silently change content behavior.
3. `SaveSystem.Save()` catches write failures and returns void. `AutosaveNow` can subsequently emit `SAVE_AUTOSAVE` despite `SAVE_FAIL`. Fix the result contract before relying on that log as successful checkpoint evidence.
4. RewardRouter accepted NaN/infinite amounts and finite addition overflow. The accompanying bounded patch rejects invalid amounts, invalid existing balances and overflow before any balance/ledger mutation. It does not repair corrupt saves, make reward batches atomic, or close SAVE-05/07.
5. Job step definitions have labels but no stable step IDs. Index-only snapshots become ambiguous after content reorder.

## Work order

1. Land numeric reward guards with 11 EditMode regression cases; verify in Unity CI.
2. Finish remaining state-owner rows and content identity/replay inventory.
3. Make save success/failure observable and test failure paths without changing existing public callers unnecessarily.
4. Implement the pure checkpoint/receipt model in `MISSION_PERSISTENCE_CONTRACT.md`, then the runtime adapters and interruption suite.
5. Headset suspend/quit/re-entry acceptance stays queued for Terry. No testing is required from Terry today.
