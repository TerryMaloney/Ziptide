# 01 — OPEN QUESTIONS, RISKS & DISCREPANCIES

*The discrepancy ledger. When something doesn't line up between minds or between
docs and code, it goes here so we catch misunderstandings early. Each item has a
status: 🔴 needs Terry · 🟡 needs a builder decision · 🟢 resolved/noted.*

---

## 🔴 For Terry (human decisions)

### Q1. There was no existing "Architect role" file/folder
You said a file + folder already explains the Architect's role. I searched the
whole repo, all issues, and both PRs — I couldn't find one. So I **created** this
`docs/architect/` folder and wrote the role into `README.md` based on the
contracts + how the team is actually working. **Please confirm the role
description is right**, or point me at the file you meant if it lives somewhere
outside this repo (Drive? a chat?). I have Drive/Gmail/Calendar access and can go
look if you tell me where.

### Q2. Story / lore canon is unconfirmed
The docs use "Basalt Coast", "RILL" voice, "Bloom / stewardship / origami" as
*examples*. I'm treating them as placeholders. **Is there a real narrative bible
yet, or is story deliberately deferred?** This matters because the whole
architecture is built to keep story swappable, but we still need *one* canon to
build the first real content pack against.

### Q3. Perf budget is entirely TBD
`07_PERF_BUDGET.md` has no numbers (frame rate, draw calls, tris, memory). For
Quest this is the single biggest silent risk — art (ships!) and VFX (Bloom) will
balloon without a budget. **Which device(s) are we targeting (Quest 2 / 3 / Pro)
and what frame rate (72 vs 90 Hz)?** I can draft a budget once I know the floor.

### Q4. ⚠️ Stray "Body Map" commit in history
Commit `78c9d2e` "Body Map final fixes: tap vs long-press separation,
drag-to-organ hit boxes, empty state improvements" is in this repo's history but
describes what looks like a **different project** (an anatomy/medical-style app),
and I see no corresponding code in the tree. Most likely a contaminated commit
message or a stray cherry-pick during init. Not harmful, but it's confusing for
anyone reading history. **Want me to leave it (just documented here) or note it?**
I would *not* rewrite published history without your say-so.

### Q5. CI secrets
CI needs `UNITY_LICENSE` / `UNITY_EMAIL` / `UNITY_PASSWORD` repo secrets, and
there's a one-time `activation.yml` workflow to generate the license. **Have
these been set?** If not, CI is currently failing at activation and the green/red
checks are meaningless until it's done.

---

## 🟡 For builders (T-Dog / Architect to decide & log)

### B1. The contract interfaces don't exist yet
The contracts name `IPodLoader`, `IThemeProvider`, `IBloomStewardship`,
`IDriftCorridor` as the backbone — but none exist as code. Today's "theme system"
(`WorldDirector` / `VisualThemeProfile`) is a concrete implementation with **no
interface seam**, and `WorldRuntime` calls `WorldDirector` directly. Before the
codebase grows, we should introduce at least `IThemeProvider` and `IPodLoader`
so the concretes sit behind seams (matches "contract-first"). **Proposal: I
draft these interface stubs + contract docs next; T-Dog wires existing concretes
behind them.** Low risk, high payoff.

### B2. `Ziptide.Content` → `Ziptide.Visuals` dependency
`WorldProfile` (in Content) holds `VisualThemeProfile` (in Visuals), so Content
references Visuals. Legal today (the only banned edge is Core→Visuals), but if
Content grows engine-like logic this becomes a smell. Watch it; if it bites,
introduce a theme-id/handle in Core that Content references instead of a direct
Visuals type.

### B3. Runtime object creation vs. authored scene
`WorldRuntime` builds bounds / theme station / respawner **at runtime via
`GameObject.Find` / `FindObjectOfType`**. It works and is convenient, but
`Find`-by-name is fragile (renaming "Ground" or "XR Origin" silently breaks it)
and adds first-frame cost. Fine for now; revisit if we add more scenes. Consider
serialized references over name lookups as the scene count grows.

### B4. Procedural textures every theme switch
`SkyPlanetRig` regenerates 256×256 sky + planet textures on the CPU on each
`ApplyProfile`. Cheap once, but if themes switch often it churns GC. Minor;
note for the perf pass.

---

## 🟢 Verified / noted (no action)

- ✅ **Core↛Visuals law holds** — verified by reading asmdefs + there's a test.
- ✅ **Milestone B is wired into the committed scene** (was the big gap PR #1 fixed).
- ✅ **Module/assembly layout matches the architecture doc.**
- 🟢 Two earlier PRs (#1 debug/fix + Milestone B wiring; #2 CI activation fix)
  are merged. Current `main` is `fd628c8`.
