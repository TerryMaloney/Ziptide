# 🟡 ACTIVE SPRINT — M4: THE SHIP (opened 2026-07-02; S1+S2 SHIPPED same-session)

> **Takeover prompt: "Read docs/SPRINT.md and continue."** Roadmap: `docs/GAME_PLAN.md` (this = **M4**,
> the north star). Architecture LOCKED: `docs/systems/SHIPS.md` (the ship is a mobile travel station;
> TravelCoordinator is the only scene path; rig is teleported, never parented). Prior sprints incl.
> M1/M2/M3 records: `docs/sprints/` — all APK-verified.

## Task board
| # | Task | Status |
|---|------|--------|
| 1 | **S1 boardable shell** — `ShipBoardingStation` (board → cockpit deck → story-gated helm → depart via TravelCoordinator → disembark) wired by `CityBuilder.BuildShipyard` on every enabled berth | ✅ `7886927` (CI #202) |
| 2 | **S2 fly-out presentation** — seat the pilot, star-streaks spool + stretch ~4.5s (pure world motion, zero camera movement), then travel | ✅ `34627dd` (CI #203) |
| 3 | **Ch.1 berths** — W002/W003/W004 + ToxicCity boardable (doors stay as fallback until device-proven) | ✅ `96132ec` (CI #204) |
| 4 | **S3 upgrade sockets** — `ShipSlotDef` sockets accept `Resources/Items` ids (engine tier → shorter fly-out; scanner tier feeds wrist scanner; cargo raises carry). DeliveryCradle socket pattern + effect wiring + tests | ⬜ NEXT |
| 5 | **W000 wake-on-ship tutorial** — the intro world: wake in the hull, RILL boot sequence, gear intro (Scan Pulse → Taser → Gravity per the deferred M2 trio), first contract, first flight to W001. Needs a small ship-interior layout + W000 gating swap (`TUTORIAL_COMPLETE` replaces `toxiccity_complete` on W002 per WORLD_DATA note) | ⬜ |
| 6 | Close: HANDOFF, runbook, checklist, **APK dispatch** → ✅ stamp | ⬜ (a fresh APK on the S1/S2 head is being dispatched now — covers M1–M4-so-far for Terry's sideload) |

## ▶ RESUMING? — current state & exact next action
- **Current:** S1+S2+Ch.1 berths CI-green (#202–#204); docs (runbook §2f, HANDOFF ddd) in `8d2a370`
  (#205 green). M3 stamped + archived this commit. A full-pipeline APK dispatch on the ship head is
  running — verify `ziptide-apk` lands, then Terry's sideload carries M1+M2+M3+M4-S1/S2.
- **Next action:** Task 4 (S3 sockets): socket panel per `ShipSlotDef` on the cockpit deck
  (`XRSocketInteractor` subclass — copy `DeliveryCradleSocketInteractor`'s allowed-ids pattern); accepted
  item id sets a profile flag/resource (`SHIP_SLOT_<slotId>_<itemId>`); effect wiring: engine tier reads
  it to scale `flyOutSeconds`. Author 2 slots on the default hull (`ShipDefinition.slots`). Tests for the
  pure tier→flyout mapping. THEN Task 5 (W000) — the big one; read WORLD_DATA W000 record first.
- **Lane:** architect owns PvP/Multiplayer (SPRINT_MULTIPLAYER.md); I consume IPvpDamageable only.
  Cross-lane fixes this session (BotMath CS0029, BotBrain same-tick test) — flagged in HANDOFF ccc.
- **Branch:** `terry-local-wip`. CI-green head: `8d2a370`.

## Working rules (unchanged)
CI green per push; SHIPS.md guardrails are law (no rig parenting, no TravelCoordinator bypass, comfort
first — never move the camera); TextMesh only; .meta per new file; pull --rebase before push.

---
*M4 opened 2026-07-02 by the operator (Fable 5). S1+S2 shipped same-session while the M3 gate ran.*
