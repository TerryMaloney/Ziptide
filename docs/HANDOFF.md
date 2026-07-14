# HANDOFF — session log (single operator ⇄ Terry)

## Required reading

- `docs/FABLE5_START_HERE.md`
- `docs/HANDOFF.md` — current entries
- `docs/HANDOFF_HISTORY_THROUGH_RB24.md` — exact prior history through rb24
- `docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md` — Quest device recovery plan
- `docs/MASTER_CHECKLIST.md`
- `docs/FABLE5_BACKLOG.md`
- `docs/TERRY_RUNBOOK.md`

## Rules

1. Read the newest entry before work.
2. Append Did / Next / Heads-up / Commit at session end.
3. Work on `terry-local-wip`; pull before starting.
4. Keep commits small and stop implementation when required CI is red.

---

## ENTRIES — newest first

### 2026-07-14 (rb25) — GPT forensic device-stabilization map

- **Did:** Terry’s first successful Quest build installed and launched, then exposed failures across boot/spawn, developer UI, traversal, item poses, interactions, tutorial continuity, ship controls, and visual finish.
- **Did:** GPT made no gameplay, scene, prefab, asset, or system implementation changes. It traced the reports to source and wrote [`docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md`](DEVICE_STABILIZATION_FORENSIC_PLAN.md).
- **Confirmed collisions:**
  1. The new destination menu waits in an empty `_Boot` scene while older locomotion and fall recovery remain active.
  2. `DevWarpBoard` and `DevMenu` both self-bootstrap and persist.
  3. Player-centered travel VFX receives the XR rig root instead of the tracked-head center.
  4. A universal 45-degree gun fallback is reused across newer item, holster, and melee paths.
  5. `SonicThumper` and `PvpHammer/HammerTool` are separate mallet implementations with separate pose rules.
  6. Cave zipline endpoints are authored without generated-geometry clearance, and the shown cable differs from the rider path.
  7. Physical repair, job progress, objective display, and ship cast-off consume related state through separate runtime owners.
  8. Core ship and world systems currently expose explicitly interim primitive art as final-looking content.
- **Confirmed source causes/debt:** the 45-degree fallback and zero-valued item data explain the gun pose/scale; hand and holster poses are not independently authored; weapon paths lack a complete shooter-ignore contract; the ship hull, many buildings, fake-light visuals, planets, mountains, and vistas are documented primitive/interim/fallback implementations.
- **Unresolved pending instrumentation:** Match Board selection, gate objective not advancing, ship-turn glitch, exact streetlight-square branch, and any structural difference between W000 and W004. The plan specifies evidence to collect before changing them.
- **Next:** Fable 5 begins with Phase 1 only: cold-boot safety/ownership, one developer menu, and readable world-label facing.
- **Next phases:** item/holster/melee correctness; traversal/travel/tutorial continuity; ship function; Picasso visual recovery; then multiplayer.
- **Heads-up:** multiplayer is paused. Patch/audit CI proves structural readiness, not tracked-device pose, UI facing, interaction feel, objective continuity, or art quality.
- **Heads-up for Picasso:** real ship hull, buildings, practical lights, planets/clouds, and distant vistas remain named Phase 5 work. They are parked until function stabilizes, not discarded.
- **Commit:** plan `075afd9`; HANDOFF archive/pointer update follows as documentation only.

---

## Prior history

The complete prior HANDOFF is preserved byte-for-byte in [`docs/HANDOFF_HISTORY_THROUGH_RB24.md`](HANDOFF_HISTORY_THROUGH_RB24.md). Read rb24 and rb23 there for the patch/audit CI rules and W011 collider history.
