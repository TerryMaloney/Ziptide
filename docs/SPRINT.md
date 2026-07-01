# 🟡 ACTIVE SPRINT — M1: THE STORY SPEAKS (opened 2026-07-01)

> **Takeover prompt for any fresh model: "Read docs/SPRINT.md and continue."** This file is the live
> state — **updated in the same commit as every push** so a rate-limit cutoff strands nothing. The
> roadmap-of-record is `docs/GAME_PLAN.md` (this sprint = milestone **M1**). Change-safety playbook:
> `docs/HOW_TO_CHANGE_ANYTHING.md`. Previous sprint's record: `docs/sprints/SPRINT_2026-07-01_MODULARITY.md`.

**Sprint goal:** the game gets its VOICE — RILL speaks (text-subtitle first, VO slots in at M6), the
Signal becomes a number any system can read, Transmission fragments become physical pickups (real
Collect steps), and story choices become a placeable set-piece. All ⚙CI. Constraints: small commits;
don't touch rig/PvP/XRI-sample files beyond a single `Ensure*` call (device round pending);
CI green per push; APK dispatch at the end.

---

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | GAME_PLAN.md (roadmap-of-record M0–M8) + pointer updates + this sprint open | ✅ `f276474` |
| 1 | `SignalState` + `RillState` (Core, pure, tested) — Signal tier 0–4 from flags; RILL memory-state machine from flags | ✅ this commit |
| 2 | `RillLineLibrary` (Content SO) + `RillLineAuthor` (Editor, authors the 12 arc beats + W001–W012 entry lines into `Resources/Story/RillLines.asset`) + `RillCompanion` (Gameplay: orb + TextMesh subtitle, world-enter/flag/job triggers) + `EnsureRillCompanion()` on the rig | ✅ this commit |
| 3 | Collectibles: `CollectibleSpawnDefinition` (+ `WorldPackDefinition.collectibles`), `CollectibleRuntime` (grab → `JobDirector.ReportCollect` + flag), JobDirector runtime spawn, `WorldJobLibrary` `Collect()`/`Pickup()` verbs, **W002 mineral + W004 fragment converted to REAL Collect steps**, `JobRuntime` early-grab BANK (anti-soft-lock) + 5 tests | ✅ this commit |
| 4 | `ChoiceStation` (two-option interactable → writes flag; pack-data spawnable) + validator choice/collectible checks (incl. the un-completable-Collect guard) + 5 tests | ✅ this commit |
| 5 | De-garble playback: `TransmissionText` (Core, 5 tier variants incl. the name moment, tested) + `TransmissionConsole` (select → render tier; auto-spawns beside fragment pickups) | ✅ this commit |
| 6 | Close: HANDOFF entry, runbook/checklist RILL+fragment smoke items, **APK dispatch green** | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** Task 5 committed — `TransmissionText.Render(tier)` (Core, pure: 5 authored
  variants walking the canon register arc static→professional→confessional→intimate→**the name
  moment**, `TransmissionTextTests`) + `TransmissionConsole` (select → renders the CURRENT tier on its
  screen; re-select after new fragments = clearer message). JobDirector auto-spawns a console beside
  any FRAGMENT_* pickup — W004's broadcast core has the full loop TODAY: grab fragment → clarity rises
  → play it back. **CI note:** run #177 (task 3) went RED — `ApplyUnlit` undefined in
  CollectibleRuntime; fixed in `5be0800` (this push). #178 fails the same way; superseded.
- **Next action:** verify CI green on this push → Task 6 close-out: HANDOFF entry (next tag after zz),
  TERRY_RUNBOOK + DEVICE_TEST_CHECKLIST smoke items (RILL speaks on entry in W001–W012 + beat lines,
  W002 mineral collect, W004 fragment pickup + console playback), MASTER_CHECKLIST state line, then
  **dispatch the APK workflow** (`actions_run_trigger` on ci.yml, branch terry-local-wip, ~20–30 min)
  and confirm `build-android` + `ziptide-apk` artifact green → stamp this file ✅ COMPLETE.
- **Branch:** `terry-local-wip`. CI-green: `f276474`, `dcf67fb`, `fdb5738`. RED: `b5033b4`/`1f64270`
  (fixed by `5be0800`).

## Specs (condensed — execute without re-deriving; verified against code this session)
- **Flags API:** `PlayerProfile.HasFlag/SetFlag` (`Core/Runtime/Persistence/PlayerProfile.cs:29-34`);
  live profile via `SaveSystem.Instance.Profile` (self-bootstrapping). All flag names in `ZiptideFlags`.
- **RillState mapping** (MASTER_BUILD_PLAN §5.1 enum · derive, never store): EndgameA–D ⇐
  `C12_W063_ENDING_A..D` · Integrated ⇐ `C6_W051_RILL_NAMED` · Unsealing ⇐ `W028_COMPLETE` or
  `C4_W028_NO_JOB` · Remembering ⇐ `C2_CONTAINMENT_REVEALED` (W012 capstone) or `C3_W013_MEMORY_SHARD` ·
  Stirring ⇐ `W004_COMPLETE` · else Dormant.
- **SignalState:** tier 4 ⇐ `SIGNAL_MAX`, 3 ⇐ `SIGNAL_THRESHOLD_3`, 2 ⇐ `_2` (granted by W010),
  1 ⇐ `_1` (granted by W004), else 0. Log consumer-side `ZIPTIDE: SIGNAL_TIER`.
- **The 12 RILL arc beats** (MASTER_BUILD_PLAN §5.2 — exact lines there): W001 boot · W004 cargo
  question · W009 misidentify · W013 memory shard · W019 refusal · W024 color · W028 unprompted ·
  W037 Warden staredown · W039 Pattern warning · W051 name choice · W062 revelation · W068 endings.
  W001–W012 beats trigger on their flag grants (flags already granted by `WorldJobLibrary` specs);
  W013+ beats are authored NOW in the library data but fire when those worlds/flags exist (M5).
  Also author one short world-entry line per W001–W012 (register: Dormant=terse/functional,
  Stirring=curious — per STORY_BIBLE).
- **RillCompanion:** ensured from `PlayerRigPersistence` (same pattern as `EnsureStunReceiver`/
  `EnsureCreditsHud` — one added call, nothing else touched on the rig). Small hovering orb near the
  left shoulder + TextMesh subtitle (NO TMP — project convention) low-center (CreditsHud/PvpHud
  positioning pattern). Triggers: `SceneManager.sceneLoaded` (WorldEnter by sceneName) + 1s profile
  flag-diff poll (FlagSet lines). Queue lines, ~4–5s display, `ZIPTIDE: RILL_LINE id=<id>`.
  Lines load from `Resources/Story/RillLines` (authored asset).
- **Collectibles:** `JobDirector.ReportCollect(itemId)` EXISTS (`JobDirector.cs:102`) →
  `JobRuntime.ReportCollect` counts against `CollectItemIdCountStepDefinition{itemId,count}` (both
  exist). Missing = the physical pickup + spawn data. Add `[Serializable] CollectibleSpawnDefinition`
  {itemId, displayName, localPosition, flagOnCollect, accentColor} + `WorldPackDefinition.collectibles`
  list; JobDirector spawns them at Start (Marker_ pattern — runtime objects, no scene YAML);
  `CollectibleRuntime` self-builds visual (collider BEFORE XRGrabInteractable — gotcha #6!), on first
  select → ReportCollect + SetFlag(flagOnCollect) + `TransmissionProgress.SyncClarityFlags` + destroy;
  `ZIPTIDE: COLLECTED item=<id>`. `WorldJobLibrary`: `Collect(itemId,count)` verb + `Pickup(itemId,pos,
  flag)` entries; convert **W002** (+3 `mineral_sample` pickups in the gallery, Collect step after the
  drones) and **W004** (fragment pickup at `broadcast_core`, `flagOnCollect=FRAGMENT_T1_FOUND`, Collect
  step last — keep the pack's flagsGranted as-is; SetFlag is idempotent). Update WORLD_DATA deferred notes.
- **ChoiceStation:** self-building two-panel interactable (XRSimpleInteractable pattern from
  `WorldTravelStation.CreateDoorway` — manager wiring + retry included), `Init(prompt, labelA, flagA,
  labelB, flagB)`; select → SetFlag + `ZIPTIDE: CHOICE_MADE flag=<f>` + lock both panels. Pack-data:
  `[Serializable] ChoiceSpawnDefinition` + pack list + JobDirector spawn (same pattern). No world
  authors one yet (first use W019/W043 — M5); mechanism + tests only.
- **Don't touch:** `TravelCoordinator`, PvP files, XRI samples, rig internals beyond the one Ensure call.

## Working rules (unchanged)
CI green after every push (red → stop + fix; `CLAUDE.md`). Never hand-edit scene YAML. New `.cs` files
need `.meta` files with fresh GUIDs (python uuid4 hex — see existing pattern). EditMode tests live in
`Ziptide/Assets/Ziptide/Tests/EditMode/`. Commit trailers per repo convention.

---
*Sprint opened 2026-07-01 by the operator (Fable 5, T-Dog account) executing GAME_PLAN M1. On close this
file gets the ✅ stamp + moves to `docs/sprints/`.*
