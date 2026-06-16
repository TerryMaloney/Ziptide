# 🧠 ARCHITECT — Shared Mind

**This folder is the Architect's brain for ZIPTIDE.** It is written to be read by
the *other* minds on this project so that humans and LLMs stay aligned and
catch discrepancies before they become bugs or rework.

---

## Who's who

| Name | Role | Notes |
|------|------|-------|
| **Terry** | Human lead / director | Owns vision, tests on Meta Quest device, final call. |
| **Architect** (me) | Systems architect / keeper of the contracts | Holds the whole-project model. Guards modularity (00_LOCKED_CONTRACTS). Writes plans, reviews structure, keeps the docs honest. Does **not** randomly rewrite working code. |
| **T-Dog** | Build LLM (Claude) | Hands-on implementation. Did the debug/fix pass, Milestone B wiring, tests + CI (PRs #1, #2). |
| **Gemini** | Art direction (with Terry) | Currently focused on **ship** visuals. |
| *(historic)* GPT / Cursor | Earlier scaffolding + sync tooling | Produced `tools/gpt_sync.ps1`, `GPT_PROJECT_UPDATE.md`, original docs scaffold. |

---

## What this folder is for

1. **A single mental model** of the whole project, so any agent can orient fast.
2. **A discrepancy ledger** — where I record things that don't line up, so we
   surface misunderstandings between minds instead of silently diverging.
3. **A coordination log** — an asynchronous "chat" / handoff thread between the
   Architect and T-Dog (and notes for Gemini on art that must respect the
   architecture).

## How to use it (for any LLM picking up work)

1. Read `docs/STATUS.md` first (the canonical dashboard — owned by everyone).
2. Then read this folder, in order:
   - `00_MENTAL_MODEL.md` — how the whole thing fits together.
   - `01_OPEN_QUESTIONS.md` — known gaps, risks, and discrepancies. **Check
     before building** so you don't trip an unresolved decision.
   - `02_COORDINATION_LOG.md` — the running thread. **Append, don't overwrite.**
   - `03_SHIP_ART_DIRECTION.md` — how ships plug into the architecture (for Gemini/Terry).

## Rules for this folder

- **Append-only log.** `02_COORDINATION_LOG.md` is a thread; add new dated
  entries at the top, never delete history.
- **This folder is opinion + map, not law.** The law lives in
  `docs/00_LOCKED_CONTRACTS.md`. If something here contradicts the contracts,
  the contracts win and this folder is wrong and must be fixed.
- **Keep it honest.** If something is unverified, say "unverified." If I'm
  guessing, I label it a guess.

---

*Maintained by the Architect. Last full pass: 2026-06-16.*
