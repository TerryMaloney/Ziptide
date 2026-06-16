# 🧠 PROJECT BRAIN — Shared Mind

**This folder is the shared "mind" for ZIPTIDE**, maintained by the build Claudes
(currently **T-Dog**). It's written to be read by the *other* minds on this
project so humans and LLMs stay aligned and catch discrepancies before they
become bugs or rework.

---

## Who's who

| Name | Role | Notes |
|------|------|-------|
| **Terry** | Human lead / director | Owns vision, tests on Meta Quest device, final call. |
| **T-Dog** (me) | Build Claude (this session) | Hands-on implementation. Did the debug/fix pass, Milestone B wiring, tests + CI (PRs #1, #2), and the onboarding + walking-skeleton work. Maintains this folder. |
| **Architect** | The *other* build Claude — **a name, not a job title** | A second Claude instance Terry also works with. (An earlier "Architect" web session didn't persist to mobile and was deleted; it rejoins via this shared brain.) |
| **Gemini** | Creative / art direction (with Terry). **No repo access.** | Low-cost space for storyline + unique **ship/alien** designs. Output = blueprints handed to the build Claudes. |
| *(historic)* GPT / Cursor | Earlier scaffolding + sync tooling | Produced `tools/gpt_sync.ps1`, `GPT_PROJECT_UPDATE.md`, original docs scaffold. |

---

## What this folder is for

1. **A single mental model** of the whole project, so any agent can orient fast.
2. **A discrepancy ledger** — where I record things that don't line up, so we
   surface misunderstandings between minds instead of silently diverging.
3. **A coordination log** — an asynchronous "chat" / handoff thread between the
   build Claudes (T-Dog + Architect) and notes for Gemini on art that must
   respect the architecture.

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

*Maintained by the build Claudes (T-Dog). Last full pass: 2026-06-16.*
