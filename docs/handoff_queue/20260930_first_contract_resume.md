# 2026-09-30 — First-contract resume integration

Authorized scope: larger persistence pass. Baseline c34002db has green Unity CI 36662089872.
Shared edits announced: additive durable world receipt list / schema migration, checkpoint physical
state fields, explicit replay/flag ownership metadata. W000 only opts into the new scene adapter.
Keep JobDirector, JobRuntime, SaveSystem, RewardRouter and RepairableMachine as existing owners.
No rig/input/travel changes; no hand-edited scenes or prefabs. New pure transaction and lifecycle
cases must accompany scene integration. Record final verification and limits before publication.

Implemented the announced scope. Added MissionCompletionReceipt, MissionRewards,
MissionCheckpointSession, JobDirector.Checkpoints and MissionResumeTests, with Unity metadata.
Updated the existing content producer, collectible/machine adapters, profile migration and atomic
save preparation. See ../architecture/FIRST_CONTRACT_RESUME_20260930.md for the full map.

Local full preflight passes (244 Python / 25 PowerShell); 26 added NUnit cases await fresh Unity CI.
Baseline c34002db is green; this candidate is not yet Unity/Android/device verified. No headset
work is requested today. Keep SAVE tasks open at campaign scope. Existing audio temporary file
was left untouched and excluded from this change. Next operator: inspect candidate CI before
expanding runtime scope, then follow deferred runbook gates.
