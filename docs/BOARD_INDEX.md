# BOARD INDEX — which doc is the truth (read this to know where to look)

Three models across three lanes generated many boards. This names the ONE source of truth per purpose so
no model lands on a stale one. **Audited 2026-07-06.**

## CANONICAL — the docs to read/update
| Purpose | THE doc | Notes |
|---|---|---|
| **How everything is wired** | `WIRING_MAP.md` | + `WIRING_AUDIT_FINDINGS.md` (the both-sides ledger) |
| **How to change a thing safely** | `HOW_TO_CHANGE_ANYTHING.md` | edit → then → verify, per system |
| **The rules you cannot break** | `00_LOCKED_CONTRACTS.md` | rig ownership, travel, assembly DAG |
| **Roadmap of record (milestones)** | `GAME_PLAN.md` | M0–M8; supersedes FABLE5_BACKLOG's phase list |
| **Current build state (built/next)** | `MASTER_CHECKLIST.md` | the scannable "state of the build" |
| **Cross-track priority order** | `PRIORITIES.md` | which lane does what next |
| **Per-lane live boards** | `SPRINT_ART.md` · `SPRINT_MULTIPLAYER.md` · `SPRINT_ARCHITECTURE.md` · `SPRINT.md` (story) | one per lane; zero file overlap |
| **Session-to-session log** | `HANDOFF.md` | append an entry every session (newest first) |
| **What Terry runs (Unity/headset)** | `TERRY_RUNBOOK.md` · `TEST_DAY.md` | menu steps + device checklist |
| **Getting on the headset** | `GET_IT_ON_THE_HEADSETS.md` · `TWO_QUEST_SETUP.md` | build/sideload + two-player |
| **New/lesser model? Start here** | `OPERATOR_START_HERE.md` | the four laws + the spine |

## RETIRED / LEGACY — do not use as the queue (kept for history)
- `WORKLIST.md`, `04_TASK_QUEUE.md` — retired 2026-07-01 → use GAME_PLAN / the SPRINT_* boards.
- `03_MILESTONES.md`, `MILESTONE_*.md` (A5, B, B1, C0, D0–D3…) — historical milestone notes; superseded by
  GAME_PLAN + MASTER_CHECKLIST.
- `STATUS.md` — a thin pointer to HANDOFF; read HANDOFF directly.
- `CONNECTIONS_AND_RECOVERY.md` — its **system-map** section is superseded by `WIRING_MAP.md`; its
  **root-cause table** is still valid debugging history.
- `FABLE5_BACKLOG.md` — its phase list is superseded by GAME_PLAN; still useful as a tagged idea bank.

## The one law that keeps this from re-sprawling
When you finish a chunk, update **only** the canonical doc for that purpose (+ a HANDOFF entry). If you feel
the urge to create a NEW board, first check this index — the slot almost certainly exists. New wiring →
add a row to `WIRING_MAP.md` (Part 4 says how).
