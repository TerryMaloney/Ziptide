# FAST LANE LOG

One line per fast-lane change. This REPLACES a HANDOFF entry for changes that qualify under
`docs/FAST_LANE.md` §3. Format: `YYYY-MM-DD · what · files · why · [debt: gate deferred]`

Newest first.

2026-07-28 · Board-staleness check moved out of Unity EditMode into the python preflight · `GateGapTests.cs` · a docs-only check was costing a full Unity run and blocking the pipeline (caused the 27-hour red streak) · no debt — `factory_governance_gate.py` already enforced it identically
2026-07-28 · HANDOFF archived to newest 10 entries · `HANDOFF.md`, `HANDOFF_ARCHIVE_2026-07.md` · 3,155 → 277 lines; the per-session read-in tax was a measured cause of the six-hour trivial-fix cycle · no debt
2026-07-28 · Rig height contract enforced OUTSIDE ScenePatcherBoot's exception swallow · `ScenePatcherBoot.cs` · D2's failures on _Boot were downgraded to a warning, so a throwing height contract shipped a 14-foot player silently · ⚠ touches rig height (report-only class) — applying an already-approved contract that was never reaching the rig
2026-07-28 · Breaker Blade set to a thrust carry (tip leads, ~20° above horizontal) · `QuestDeviceCorrectionsRuntime.cs` · the blade was forced to a ~68° raised carry by a runtime component that runs AFTER ItemFactory and overrides it — which is why every factory grip fix "regressed" · one tunable vector
