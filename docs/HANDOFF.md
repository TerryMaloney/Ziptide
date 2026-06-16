# 🔁 HANDOFF — shared cross-chat log

**The ONE place the build Claudes (Architect ⇄ T-Dog) keep each other in sync.**
Purpose: never repeat work, never collide on the same files, and never make Terry
copy-paste progress between chats.

---

## ⛔ HARD RULE — applies to EVERY Claude chat on this project

1. **START of every session:** read this whole file before doing anything else.
2. **END of every session that changed anything** (code, scene, docs, settings):
   append a new entry **at the top of the Log** below. No exceptions.
3. **Before you build:** if you're taking a system/file, say `CLAIMED: …` in your
   entry so the other chat stays off it.
4. **Never delete history.** Newest on top; old entries stay.
5. Keep entries short — bullets, not essays. Link to deeper notes in `docs/architect/`.

> If you are a Claude chat reading this for the first time, this rule is now in
> effect for you too — tell Terry you've read it so he knows both chats are synced.

---

## Entry format (copy this)

```
### [YYYY-MM-DD HH:MM UTC] — <Architect | T-Dog>
- **Did:** <the last thing you actually did>
- **Next / CLAIMED:** <what you're working on / reserving so the other chat avoids it>
- **Heads-up for the other chat:** <anything they need to know / not break>
- **Commit / branch:** <hash or branch, if pushed>
```

---

## 📌 PINNED — WORKING AGREEMENT
*Set 2026-06-16 with Terry. Update here if it changes.*

- **Terry** = human lead/director. Tests on the Quest. Final call.
- **Architect** + **T-Dog** = the two build Claudes (names, not job titles). We
  hold the repo, the contracts, and all code. Whoever is active updates this log.
- **Gemini** = creative + art direction only, **no repo access**. A low-cost space
  for Terry to develop storyline + unique **ship/alien** designs. Output =
  blueprints handed to the build Claudes, who turn them into mechanics/structures.
- **Collision rule:** claim a lane here before building; don't both touch the same
  files/systems at once. If something looks in-progress, ask in the log.

---

## Log (newest first)

### [2026-06-16 ~12:05 UTC] — Architect (logging on Terry's word — T-Dog, please confirm/expand)
- **Did:** Per Terry, **T-Dog built a dev "level/world jump" system** so the dev
  can warp straight to any world/level to work on it, instead of playing through
  the whole game.
- **Next / CLAIMED:** none — T-Dog, replace this with the real entry: what it's
  called, how to invoke it, and which files/scenes it touches.
- **Heads-up for the other chat:** Architect hasn't seen this code yet; will fold
  it into `docs/architect/00_MENTAL_MODEL.md` once T-Dog confirms the details.
- **Commit / branch:** unknown (T-Dog's branch).

### [2026-06-16 ~11:00 UTC] — Architect
- **Did:** Onboarding pass + built `docs/architect/` (project mental model, open
  questions ledger, ship art-direction notes). Shipped the walking-skeleton
  **pod-loading seam**: `IPodLoader` + `PodNarrative` (Core), `PodRegistry`
  (Content), `PodLoaderTests` (EditMode). All headless/CI-testable, no device.
- **Next / CLAIMED:** Pod seam is Architect's until merged. Eyeing the
  `IThemeProvider` seam next (still UNCLAIMED — will post here before starting).
- **Heads-up for the other chat:** (1) A.5/B are **not device-verified yet** — the
  Quest upload didn't complete last session. (2) Don't rename scene objects
  `Ground` / `XR Origin`; lots of `GameObject.Find` depends on those names.
- **Commit / branch:** `claude/architect-project-onboarding-2x7h60` (pushed).

### Open asks for Terry (carried from `docs/architect/01_OPEN_QUESTIONS.md`)
- **Q2 — story canon:** real narrative bible yet, or deliberately deferred?
- **Q3 — perf/device target:** Quest 2/3/Pro? 72 or 90 Hz? (gates art/VFX budgets)
