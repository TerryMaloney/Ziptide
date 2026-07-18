# PLAYTEST PROTOCOL + LOCAL TELEMETRY — tuning by evidence, safe for kids (GAP 5)

**Status:** 🔵 PLANNED — protocol usable at the NEXT kid session (it's paper); the telemetry
summary is a small post-freeze envelope. From `GAP_AUDIT_JULY2026.md` GAP 5, 2026-07-18.
**Privacy law, absolute:** nothing is ever transmitted anywhere; no recordings of minors; the
only artifacts are Terry's handwritten notes and a local-on-device text file he chooses to look
at. COPPA-clean by construction because data never exists off-device.

## §1 — The kid-session protocol (one page, use immediately)

**Before:** pick ONE question per session ("does the first hour hold attention?" — never
"is the game good?"). Charge the headset. Say only: *"Play whatever you want. You can stop
whenever."* No goals, no tour.
**During — observe, don't coach.** The three golden metrics, in order of truth:
1. **The takeoff point.** When (and where) the headset comes off is the single most honest
   number a kid will ever give you. Log timestamp + what was on screen.
2. **Stalls:** anywhere they stop, wander, or ask "what do I do?" — log place + what they
   tried first. Their FIRST guess is the design's grade, not their eventual success.
3. **Delight markers:** unprompted laughter, "come look at this," replaying an action for fun,
   narrating to the room. Log what caused it — these are the fun we must protect (the
   EXCELLENCE_MAP's real currency).
**Coach only on safety/comfort.** If they're lost >2 min, note it, THEN help — the note already
paid for the session.
**After (60 seconds, their words):** "What was the best part?" · "Was anything boring or
annoying?" · "Anything make you feel weird or dizzy?" (comfort question is mandatory, every
session). Write answers verbatim; kids' phrasing carries diagnosis adults sand off.
**File it:** `docs/PLAYTEST_LOG.md` — date, player age, question, the three metrics, quotes,
and the session's **three biggest stalls** promoted to board items. A finding appearing twice
across sessions = a real issue, no debate needed.

## §2 — The local session summary (the tiny telemetry envelope, post-freeze)

We already emit rich `ZIPTIDE:` tags for everything that matters. One small component
(`SessionSummary`, under diagnostics ownership) counts a fixed, closed set in memory and, on
doff/quit, writes ONE local text file (`session_summary_<date>.txt`, atomic-write reuse):

session minutes · worlds visited · travels · falls caught by the net · player downs ·
repairs/builds completed · shots fired / hits (per verb) · comfort aborts (vignette max events,
snap-turn spikes) · barks/RILL lines heard (id counts — catches both silence AND nagging) ·
time-to-first-action from boot · takeoff context (last log tag before doff).

**Rails:** counters only — no free text, no positions-over-time, no timestamps finer than the
session, no identity beyond the save-slot tag; file lives in the app's own storage; ≤2 KB; the
closed counter list is IN CODE and adding to it is a reviewed change (`SESSION_SUMMARY_FIELD`
audit). Terry reads it after a kid session next to his §1 notes — the two views (observed +
counted) is the whole method.

## §3 — What we refuse

- Any network transmission, crash-reporting SaaS, analytics SDK, or fingerprinting — rejected
  outright at this scale and audience; revisit only at store launch for CRASH data with an
  explicit, separate Terry decision (and Meta's own platform stats may cover it).
- Session recordings/video of minors; heatmaps; per-player comparisons between the kids
  (profiles make it possible; decency makes it forbidden — summaries are per-session, and the
  slot tag exists only so comfort counts pair with the right comfort settings).
- Tuning by telemetry alone: counters find WHERE, only watching finds WHY. §1 outranks §2.

## §4 — Acceptance

Protocol: used at the next session; log file exists with three promoted stalls. Telemetry
envelope (post-freeze): summary file appears after a device session; counter tests headless;
the audit rule guards the closed list; nothing in the file a stranger could tie to a person.
