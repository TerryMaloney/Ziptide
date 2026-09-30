# 2026-09-29 — Mission progress and content guards

Did: fixed stale partial-progress StepText before callbacks for all five counter objective types. Added duplicate scoped job/machine/marker identity diagnostics. Replaced per-step pickup sufficiency with per-job aggregate demand, including null supply and overflow handling. Fourteen NUnit cases added (five runtime, nine validator).

Evidence: previous save-result source ed82c7bd passed full Unity CI 36621171988, including EditMode and scene audit; Android skipped. This candidate needs its own Unity run. Local full preflight passed (244 Python gates, 25 Quest operator checks, governance, readiness reporting, whitespace); new NUnit tests were not executed locally.

Identity/replay audit: 14 committed jobs and 42 packs scanned, plus authoring/runtime review. Generated step paths are ordinal, no replay policies exist, and some packs are destination descriptors. See architecture/MISSION_IDENTITY_REPLAY_AUDIT_20260929.md. Do not automatically alias all packIds to sceneNames.

Next: verify candidate CI, map JobDirector-owned packs separately from travel descriptors, then stable authored step/placement IDs and explicit replay definitions before checkpoint storage. No mission persistence/replay policy implementation in this batch. No scene, rig, inventory, travel or reward payout changes. Existing pack audit remains warning-only. Headset tests stay open.
