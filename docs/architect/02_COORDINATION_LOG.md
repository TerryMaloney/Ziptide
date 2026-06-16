# 02 — COORDINATION LOG (Architect ⇄ T-Dog ⇄ Terry/Gemini)

*An asynchronous thread between the minds on this project. **Append new entries
at the top. Never delete history.** Sign each entry with who wrote it and the
date. This is where we resolve discrepancies out loud so we don't silently
diverge.*

Entry template:
```
### YYYY-MM-DD — <author> → <audience>
**Re:** <topic>
<body>
**Action / ask:** <what you want the reader to do>
```

---

### 2026-06-16 — Architect → T-Dog & Terry
**Re:** Onboarding pass complete; the Architect now exists as a folder.

I (Architect) just did a full read of the repo, both PRs, the docs, and the
Unity asmdef graph, and built this `docs/architect/` folder as my persistent
"mind." Highlights for you, T-Dog, since you've been the hands so far:

- Your PR #1 + #2 work is solid and I've recorded the current state accurately in
  `00_MENTAL_MODEL.md`. The one thing I want to align on is **B1** in
  `01_OPEN_QUESTIONS.md`: the contracts promise interface seams
  (`IThemeProvider`, `IPodLoader`, `IDriftCorridor`, `IBloomStewardship`) but
  none exist yet, and `WorldRuntime` talks to `WorldDirector` directly. I'd like
  to introduce those interfaces before the code grows, so we honor "contract-
  first" for real and not just in docs.
- Heads up on **B3**: a lot of wiring relies on `GameObject.Find("Ground")` /
  `Find("XR Origin")`. It works today; I'm flagging it so neither of us renames
  those objects without expecting breakage.

**Action / ask:** T-Dog — when you next pick up implementation, hold off on new
systems until we agree on the interface seams (B1). I'll draft stubs + contract
docs; you wire the existing concretes behind them. Shout in this log if you
disagree.

---

### 2026-06-16 — Architect → Terry
**Re:** What I need from you to unblock the next real milestone.

Three answers would unblock a lot (all in `01_OPEN_QUESTIONS.md`): the
**perf/device target** (Q3), whether there's a **story canon** yet (Q2), and
confirmation of **my role description** (Q1, since I couldn't find the role file
you mentioned). None block your Quest test today — they shape what we build next.

**Action / ask:** Answer Q1–Q3 when you have a sec; I'll turn them into a plan
and a perf budget.

---

### 2026-06-16 — Architect → Gemini (via Terry)
**Re:** Ships must plug into the theme registry, not be hardcoded.

You're doing ship art. The one architectural constraint that matters for you is
in `03_SHIP_ART_DIRECTION.md`: ship **skin** (materials/VFX) and ship
**layout/geometry** have to be independently swappable and resolved through the
theme registry — never referenced by prefab name inside ship logic. As long as
art is delivered as theme-bound assets with clean pivots/scale and a Quest-aware
poly/material budget, it'll drop in cleanly later.

**Action / ask:** Terry — please relay, or pull Gemini in to read
`03_SHIP_ART_DIRECTION.md`. I left concrete asks there.

---

*(Older entries below. Nothing yet — this folder was created 2026-06-16.)*
