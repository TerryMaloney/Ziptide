# ZIPTIDE FAST PREFLIGHT — seconds before Unity

**Purpose:** catch repository-governance and cheap Python failures before a push consumes a Unity
EditMode, scene-audit, Recovery, or Golden Android slot.

## One command on Terry's Windows checkout

```powershell
.\tools\dev_preflight.ps1
```

The wrapper runs, in order:

1. `tools/factory_governance_gate.py`
   - every open `MISS_LEDGER` entry still carries the exact five Class Law fields;
   - dated yellow sprint-board claims are no more than 14 days old unless they are explicitly paused;
   - the recovery pause manifest is valid.
2. Every fast Python gate unit test under `tools/tests/test_*_gate.py`.
3. `git diff --check` for conflict markers and whitespace errors.

A non-zero result means **do not push yet**. Repair the named local file and re-run the command.

## Why this exists

On 2026-07-23 a planning-only source candidate entered the Unity queue and went red after the
expensive runner started. The authoritative NUnit result contained two documentation-governance
failures:

- `**SYSTEM CHANGE:**` had been split across a Markdown line break in `MISS_LEDGER` entry 15;
- three old yellow sprint claims had not been renewed, released, completed, or formally paused.

Neither failure required Unity to discover. This preflight mirrors those checks in Python, including
a regression test for the split-token case.

## What this does not prove

Fast preflight never replaces:

- Unity EditMode compilation/tests;
- patch-scenes + world audit;
- Recovery PlayMode routes;
- Golden Android;
- APK install, logcat, or Terry's headset feel/reach/comfort verdict.

It only rejects cheap, deterministic mistakes early so the expensive evidence lanes are reserved for
things that actually need them.

## Operator rule

Run fast preflight immediately before every source push. If a change touches XR input, travel,
persistence, world generation, scenes, shaders, or Android build behavior, continue through all
required expensive gates after preflight passes.
