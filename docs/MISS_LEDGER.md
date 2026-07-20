# MISS LEDGER — every miss is a class, every class gets a system change

**The Class Law (Terry, 2026-07-19):** any acute issue is a systemic issue. Every miss — bug,
omission, planning blind spot, process failure — is logged here with the five fields, and the
entry is not CLOSED until its SYSTEM CHANGE exists. A fix without a system change is a loan.
Format per entry: **WHAT · FOUND BY · WHY MISSED · CLASS · SYSTEM CHANGE (→ status)**.
This generalizes hwr31's device-bug ratchet ("every bug dies three deaths") to everything.
Read by every lane at session start alongside HANDOFF. Full spec: `FINISHED_GAME_BENCHMARK.md` §3.

---

## OPEN / SEEDED 2026-07-19 (the benchmark exercise's own findings — entries 1–10)

1. **WHAT:** no player-facing credits roll designed. **FOUND BY:** finished-game benchmark (B3).
   **WHY MISSED:** CREDITS.md solved the LICENSING need and its existence masked the
   EXPERIENCE need — one name, two artifacts. **CLASS:** a bookkeeping artifact masking a
   player-facing artifact of the same name. **SYSTEM CHANGE:** benchmark B3 row + EXCELLENCE_MAP
   presentation row (→ pending); credits-roll design queued with title-menu family.
2. **WHAT:** no legal/boot screens (health & safety, licenses, studio identity). **FOUND BY:**
   benchmark (B3). **WHY MISSED:** cert list covered the REQUIREMENT; nobody owned the DESIGN.
   **CLASS:** compliance item with no experience owner. **SYSTEM CHANGE:** same as #1.
3. **WHAT:** player-visible error states (save-corrupt, entitlement-fail, storage-full) have no
   authored fiction. **FOUND BY:** benchmark (B3). **WHY MISSED:** error handling lived in
   engineering docs; the FICTION layer had no slot for failure. **CLASS:** engineering states
   without presentation states. **SYSTEM CHANGE:** error-state line kit added to the voice-
   formula scope (assembly GAP E) (→ pending).
4. **WHAT:** the ending EXPERIENCE (roll-credits moment, post-credits state, completion
   ceremony, replay answer) undesigned while ending STORY is fully designed. **FOUND BY:**
   benchmark (B5). **WHY MISSED:** story lane owned the endings' MEANING; no lane owned their
   STAGING; first-hour got a Director's Cut, last-hour never did. **CLASS:** asymmetric care —
   entrances polished, exits assumed. **SYSTEM CHANGE:** benchmark B5 row + "LAST_HOUR_DIRECTORS_CUT"
   queued as a design doc (→ pending); EXCELLENCE_MAP row.
5. **WHAT:** "Ziptide" never trademark/store-collision searched. **FOUND BY:** benchmark (B6).
   **WHY MISSED:** legal-compliance list was platform-shaped (what Meta asks) — nothing asked
   what the WORLD asks. **CLASS:** compliance scoped to the platform's questionnaire.
   **SYSTEM CHANGE:** B6 gains name/IP clearance row (→ pending: Terry runs the search — an
   afternoon, HIGH priority, freeze-compatible).
6. **WHAT:** no marketing campaign plan (page copy, shot list, trailer beats, press kit,
   launch calendar, demo decision). **FOUND BY:** benchmark (B7). **WHY MISSED:** DC-4 built
   capture TOOLING and read as "marketing covered." **CLASS:** tooling mistaken for the
   deliverable it enables. **SYSTEM CHANGE:** B7 row; campaign doc queued (→ pending).
7. **WHAT:** business decisions (price, discount posture, no-MTX stance) not on record.
   **FOUND BY:** benchmark (B8). **WHY MISSED:** premium/no-MTX was implied by tone-charter
   values, never DECIDED. **CLASS:** values mistaken for decisions. **SYSTEM CHANGE:** B8 ⚖
   one-pager queued for Terry (→ pending).
8. **WHAT:** no live-ops plan (patch cadence, triage route, support channel, review policy).
   **FOUND BY:** benchmark (B9). **WHY MISSED:** every plan ends at "device pass"/launch;
   nothing forced the ship-backward view past day one. **CLASS:** planning horizon ends at
   launch. **SYSTEM CHANGE:** B9 row + live-ops one-pager queued (→ pending).
9. **WHAT:** no written bug-severity ship bar / cut list. **FOUND BY:** benchmark (B4/B1).
   **WHY MISSED:** gates enforce QUALITY continuously, so nobody defined the discrete SHIP
   BAR; cuts were deferred implicitly, never declared. **CLASS:** continuous discipline
   masking a discrete decision. **SYSTEM CHANGE:** severity-bar + CUT_LIST.md ⚖ pages queued
   (→ pending).
10. **WHAT (meta):** whole benchmark categories (B5/B7/B8/B9) were absent from ALL planning
    passes including six prior audits. **FOUND BY:** this exercise. **WHY MISSED:** planning
    grew game-outward (mechanics→art→content); nothing forced ship-backward. **CLASS:**
    inside-out planning blindness. **SYSTEM CHANGE:** ✅ THIS LEDGER + the benchmark as a
    recurring milestone gate (checklist row) + EXCELLENCE_MAP rows per category — the
    ship-backward view now has standing machinery.

## CLOSED

*(entries move here when their SYSTEM CHANGE is verified in place — the fix alone never closes
an entry)*
