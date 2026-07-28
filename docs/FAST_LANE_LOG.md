# FAST LANE LOG

One line per fast-lane change. This REPLACES a HANDOFF entry for changes that qualify under
`docs/FAST_LANE.md` §3. Format: `YYYY-MM-DD · what · files · why · [debt: gate deferred]`

Newest first.

2026-07-28 · Board-staleness check moved out of Unity EditMode into the python preflight · `GateGapTests.cs` · a docs-only check was costing a full Unity run and blocking the pipeline (caused the 27-hour red streak) · no debt — `factory_governance_gate.py` already enforced it identically
2026-07-28 · HANDOFF archived to newest 10 entries · `HANDOFF.md`, `HANDOFF_ARCHIVE_2026-07.md` · 3,155 → 277 lines; the per-session read-in tax was a measured cause of the six-hour trivial-fix cycle · no debt
