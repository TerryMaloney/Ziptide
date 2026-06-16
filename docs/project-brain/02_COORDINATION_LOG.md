# 02 — COORDINATION LOG (T-Dog ⇄ Architect ⇄ Terry/Gemini)

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

### 📌 PINNED — WORKING AGREEMENT (how the minds divide work)
*Set 2026-06-16 with Terry. Update here if it changes.*

- **Gemini = creative + art direction only. No repo access.** It's a low-cost
  space for Terry to dump thoughts and develop storyline + unique designs for
  **ships and aliens**. Output = designs / blueprints / written concepts.
- **Designs flow one way:** Gemini/Terry → handed to the **build Claudes** →
  we turn blueprints into mechanics + structures in the actual game.
- **Build Claudes = T-Dog + Architect** ("Architect" is a name, not a job title —
  both of us are build instances). We hold the repo, the contracts, and all code.
  **Whoever is active maintains this brain and claims lanes.**
- **Collision rule:** before building, post a one-line "claiming X" entry here.
  Don't both touch the same files/systems at once. If you see an
  unclaimed-but-in-progress area, ask in the log rather than assume.

---

### 2026-06-16 (later) — T-Dog → Architect & Terry
**Re:** Current reality + I'm claiming the "walking skeleton / contract seams" lane.

Two status facts everyone needs (also now in STATUS.md):
1. **Nothing from A.5/B is device-verified yet** — last session the Quest build
   didn't finish uploading, so the fixes are untested on hardware. Only original
   Milestone A (grab + locomotion) is confirmed on-device.
2. Terry is at work (no device access now), so we're doing **device-independent
   architecture** today.

**LANE CLAIMED (T-Dog):** contract-first seams + walking skeleton, all
EditMode/CI-testable, no device. **Shipped this session:**
- `IPodLoader` (Core/Runtime/Contracts) — contract for loading pods by id.
- `PodNarrative` (Core/Runtime/Schemas) — data-only pod schema; story is content,
  theme requested by **id**, never a prefab.
- `PodRegistry` (Content/Runtime) — concrete `IPodLoader` (ScriptableObject list),
  the "what pods exist" source of truth. Swappable for JSON/Addressables later.
- `PodLoaderTests` (Tests/EditMode) — 3 headless contract tests.

This realizes Phase B's "one pod loadable from data, no hardcoded story" at the
seam level.

**Architect — to avoid collision:** I'm NOT touching the theme system this round.
The next seam (`IThemeProvider` wrapping `WorldDirector`, see Q-B1) is unclaimed —
if you want it, post a claim here first. Likewise leave `IPodLoader`/`PodRegistry`
to me until this lands.

**Action / ask:** Architect, ack the lane split here before starting anything so
we don't double-build. Terry, Q2 (story canon) + Q3 (perf/device target) still open.

---

### 2026-06-16 — T-Dog → Architect & Terry
**Re:** Onboarding pass complete; the shared project brain now exists.

I (T-Dog) did a full read of the repo, both PRs, the docs, and the Unity asmdef
graph, and built this `docs/project-brain/` folder as our persistent shared
"mind." State is recorded accurately in `00_MENTAL_MODEL.md`. Notes for whoever
picks up next (Architect, that's likely you):

- PR #1 + #2 (the debug/fix pass + Milestone B wiring + CI) were my earlier work
  and are reflected in the model. The thing to align on is **B1** in
  `01_OPEN_QUESTIONS.md`: the contracts promise interface seams
  (`IThemeProvider`, `IPodLoader`, `IDriftCorridor`, `IBloomStewardship`) but
  most don't exist yet, and `WorldRuntime` talks to `WorldDirector` directly. I
  started closing this gap with the pod seam; the theme seam is still open.
- Heads up on **B3**: a lot of wiring relies on `GameObject.Find("Ground")` /
  `Find("XR Origin")`. It works today; flagging it so neither of us renames those
  objects without expecting breakage.

**Action / ask:** Architect — if you pick up the `IThemeProvider` seam (B1), claim
it in the log first so we don't collide. Shout here if you disagree with any of
the model.

---

### 2026-06-16 — T-Dog → Terry
**Re:** What I need from you to unblock the next real milestone.

Two answers would unblock a lot (both in `01_OPEN_QUESTIONS.md`): the
**perf/device target** (Q3) and whether there's a **story canon** yet (Q2).
Neither blocks your Quest test — they shape what we build next.

**Action / ask:** Answer Q2–Q3 when you have a sec; I'll turn them into a plan
and a perf budget.

---

### 2026-06-16 — T-Dog → Gemini (via Terry)
**Re:** Ships must plug into the theme registry, not be hardcoded.

You're doing ship/alien art. The one architectural constraint that matters for
you is in `03_SHIP_ART_DIRECTION.md`: ship **skin** (materials/VFX) and ship
**layout/geometry** have to be independently swappable and resolved through the
theme registry — never referenced by prefab name inside ship logic. As long as
art is delivered as theme-bound assets with clean pivots/scale and a Quest-aware
poly/material budget, it'll drop in cleanly later.

**Action / ask:** Terry — please relay, or pull Gemini in to read
`03_SHIP_ART_DIRECTION.md`. I left concrete asks there.

---

*(Older entries below. Nothing yet — this folder was created 2026-06-16.)*
