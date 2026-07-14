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

### 2026-07-14 (rb26) — Fable 5: Phase 1 stabilization implemented + DS-09/10/12 evidence probes

- **Did (plan):** converted the rb25 forensic map into an approved fix plan with Terry's two decisions
  locked: the **physical board idiom owns DS-02** (device-proven rendering; the TMP canvas carries the
  recorded 2026-07-06 dead/flicker failure) and **instrumentation is included** alongside Phase 1.
- **Did (DS-01, `fix(boot)`):** the cold-boot HOLD contract. BootLoader arms it before the Home Hub;
  while held: move/turn/snap/dash providers suspended, the global fall net disarmed, rig pinned to its
  boot pose. Released ONLY in `TeleportToMarker` after a content spawn settles (re-arms the net from the
  fresh spawn); marker-less content scenes release too, so nobody arrives frozen. Pure `BootHoldState`
  seam + `BootHoldTests`. Logs `ZIPTIDE: BOOT_HOLD on/off`.
- **Did (DS-02/03, `fix(devtools)`):** ONE dev menu. `DevWarpBoard` is now summoned (forehead gesture /
  F2 / ADB gate), dismissible (gesture toggle + red CLOSE tile), fixed-pose at summon (the orbiting
  billboard is deleted), labels un-mirrored via the facing contract, and any open board closes on scene
  load. `DevMenu` retired from runtime (no bootstrap, no gesture; kept only as a manually-mounted
  diagnostic with the reason in its header). `DevToolsSingletonTests` source-scans DevTools: exactly one
  `RuntimeInitializeOnLoadMethod`, and it must be the board.
- **Did (DS-14/DS-03, `fix(ui)`):** `WorldLabelFacing` — THE facing contract in one pure helper
  (TextMesh reads from −Z; facing a viewer = +Z points AWAY). Travel doors now carry a label per FACE
  (readable from both approach sides, static, no per-frame cost). Tests pin the convention, pin the old
  buggy `LookRotation(toViewer)` as unreadable forever, and prove HomeHub's board math was already
  correct (why Terry could read the hub but not the doors).
- **Did (DS-09/10/12, `diag`):** log-only probes, zero behavior change — `ZIPTIDE: BOARD_PROBE`
  (hover/select + 1 Hz aim probe: actual ray hit path/layer, bound manager instance, facing dot),
  `ZIPTIDE: REPAIR_TRACE` (every hop: machine → director → runtime consumed/banked → bank-drain →
  objective board rendered text → cast-off observed instance), `ZIPTIDE: FLIGHT_TRACE` (per emitted yaw
  snap: raw stick, latch, yaw, frame ms). The forensic plan forbids behavior edits on these three until
  a capture picks the branch.
- **Next:** Terry runs **TERRY_RUNBOOK §0** (the Phase-1 device checklist + the logcat capture). Then:
  Phase 2 (DS-06/07/08 item/holster/hammer pose contract) with the captured evidence feeding DS-09/10/12
  fixes; DS-04/05 in Phase 3.
- **Heads-up:** DS-05's tracked-head fix is one line in `TravelCoordinator` but is deliberately parked
  for Phase 3 with its test, per the plan ordering. DevMenu's TMP canvas path still exists for manual
  diagnosis only — the singleton test turns CI red if anyone re-bootstraps it. All Phase-5 art items
  stay parked for Picasso.
- **Commit:** `a0dfdc7` (DS-01) · `5774182` (DS-14/03) · `aa59e7c` (DS-02/03) · diag + docs follow.

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
