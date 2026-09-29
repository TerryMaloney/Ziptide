# 2026-09-29 — Observable save results

Did:
- SaveFileStore.TryWriteProfile uses the existing serializer/atomic writer, returns success or an error, and restores the prior timestamp if serialization/write fails. Gameplay changes remain in memory for retry.
- SaveSystem.TrySave and TryAutosaveNow return actual outcomes. SAVE_AUTOSAVE only follows success. Existing Save/AutosaveNow void entry points, pause/quit and travel behavior remain compatible.
- Added three real-filesystem tests: saved timestamp round trip; blocked temporary-file path followed by successful retry (old main preserved, one reward/ledger entry, flags retained, old snapshot becomes backup); null profile rejected without files. Extended absent-instance autosave assertion.
- HomeHub's source contract now requires one result-bearing store call instead of one direct WriteAtomic call. Storage ownership remains unchanged.

Evidence:
- Prior reward patch 0c36bfc42e8ddd45eb4f1f0a297d83e61170ad30: Unity EditMode job 109583036910 PASS in run 36620111632. Scene audit was still running at observation. Android skipped.
- This candidate needs fresh Unity CI; no headset evidence claimed.

Next:
- Confirm candidate CI, then finish scoped world/job/step identity and replay-policy content inventory before checkpoint schema implementation.
- Runtime presentation/log behavior and actual pause/quit remain part of integration/device acceptance. File-store tests do not prove headset interruption behavior.

Boundaries:
- No mission checkpoint, completion receipts, profile reset redesign, inventory, input, travel ownership or scene edits.
- Existing void callers still continue on save failure. New result-bearing APIs enable later policy decisions; this patch does not block travel or retry automatically.
- New Game retains its existing profile replacement behavior and initial timestamp convention. A new profile's timestamp alone does not prove successful persistence. New Game failure UX remains a separate task.
- Main/backup atomic writer behavior is unchanged. No claim of guaranteed preservation of unsaved progress or power-loss durability.

Local validation: full tools/dev_preflight.ps1 PASS under PowerShell 7.4.6 Linux (governance, 244 Python gate tests, offline readiness report, 25 Quest operator checks, whitespace). New NUnit tests require Unity CI; they were not executed by this local gate.
