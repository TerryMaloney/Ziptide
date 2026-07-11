# A5 TEST HOTFIX — DOWNS ASSERTION

**Owner:** GPT-5.6 Thinking  
**Authorized by:** Terry, 2026-07-11 (free rein with breakage caution)  
**Trigger:** Durable CI verdict run `29151688686` recorded Unity EditMode RED  
**Scope:** one assertion in `Ziptide/Assets/Ziptide/Tests/EditMode/PvpProgressionTests.cs`; no runtime changes

## Finding

`Stats_TrackKillsDownsAndStreaks` expected `Downs(1) == 1` after this sequence:

1. combatant 0 kills combatant 1 — downs(1) becomes 1;
2. later combatant 1 self-kills — kill credit is correctly guarded, while the down correctly counts;
3. downs(1) therefore becomes 2.

`MatchStatsCore.RecordKill` matches the authored A5 contract: a self-kill grants no kill credit but the defeated combatant still receives a down. The failing assertion forgot the earlier down.

## Fix

Change only the expected value from 1 to 2 and clarify the assertion message. Runtime `MatchStatsCore`, payout, unlocks, profile, lobby, and scene behavior remain untouched.

## Evidence

- Run: `29151688686`
- Unity result: 805 passed, 1 failed
- Failed test: `Ziptide.Tests.EditMode.PvpProgressionTests.Stats_TrackKillsDownsAndStreaks`
- Actual: 2; previous expected: 1

## Acceptance

- New branch CI must record Unity EditMode success in `docs/CI_VERDICT.md`.
- Android may remain `skipped` on the ordinary branch push.
- No runtime file changes.
