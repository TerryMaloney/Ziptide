# 🗺️ THE EXCELLENCE MAP — every aspect of the game, its standard, and its guardrail

**What this is (Terry, 2026-07-10):** "we need to make sure there's basically no room for a model to
half-ass it… the map of everything the game is going to need… we need standards and guardrails —
we already have some but they need to be better across the board."

This is that map. **One row per aspect of the finished game.** Each row answers four questions a
mid-level model can act on without judgment calls:
1. **STATE** — what actually exists (⬜ nothing · 🦴 skeleton · 🧱 v1 solid · 💎 rich · numbers, not vibes).
2. **THE STANDARD** — what *excellent* means for this aspect, in CHECKABLE terms.
3. **THE GUARDRAIL** — the gate that mechanically enforces it (a CI audit/test), or **🕳️ GAP** — a
   named, claimable missing gate. A standard without a gate is a wish.
4. **Where the truth lives** — the design doc + the code entry point.

**How to use it (any model, any session):**
- Before building in an aspect: read its row → its doc → build to the STANDARD → your DoD (below in
  `OPERATOR_START_HERE.md`) requires it. The current work order lives in `CURRENT_EXECUTION_CHECKLIST.md`.
- At chunk close: if you changed an aspect's STATE, update its row. This file is a dashboard, not
  an archive — stale rows are bugs.
- **The unevenness rule:** if your lane's rows are all 💎 while a neighboring aspect you depend on
  is 🦴, the right next task is often THEIR row, not more polish on yours. Terry's exact fear is
  "some parts really good and other parts not that great."

Legend: gates live in `Ziptide/Assets/Ziptide/Editor/Audit/*AuditRules.cs` (BLOCK = CI-red) unless
noted; EditMode tests are gates too. *(Last status reconciliation: 2026-07-20.)*

---

## 1 · WORLDS & SPACE

| Aspect | State | The standard (checkable) | Guardrail |
|---|---|---|---|
| Terrain & biomes | 🧱 v1 (fBM+warp heightfields, biome matrix, 12 worlds) | Every world's ground is distinct at a glance; walkable slopes gated; no repeated-feeling worlds | `WorldAuditRunner` + `WorldContentAuditRules`; terrain tests |
| POIs & world dressing | 🧱 v1 (POI verbs, scatter, paths) | Every POI reachable; streets/interiors carry prop kits, not emptiness | `WorldReachabilityAuditRules` + `RouteContinuityAuditRules` candidate |
| Buildings & city | 🧱→💎 in flight (Stage A/B + Round 1 ToxicCity pass) | Biome→kit/palette mapping; no primitive-box fallbacks in shipped worlds; street identity and motion fit budget | `BuildingAuditRules`; City Stage A/B audits; registry fallback logs |
| Interiors | 🧱 v1 (partitioned/furnished/per-room portal cull; W002 re-bake pending) | Enterable buildings have connected, furnished, portal-culled interiors | `InteriorAuditRules` + `InteriorFurnishCoreTests` |
| Vertical/caverns/traversal | 💎 (zip/climb/lift/pad/grapple + cave worlds) | Every traversal verb usable; multi-level reachability proven | reachability one-way edges + traversal test suites |
| Skyscape & atmosphere | 🧱 v1 (canonical layers, signature worlds pending device verdict) | Prospect bar: drift, hazy horizon, occluded body, sky color reaches ground | `SkyVistaAuditRules` + signature-rubric tests |
| Travel & gates | 💎 code (one `TravelCoordinator`, gated doors, THE ZIPTIDE, pre-flight, async load behind crest; device frame check pending) | One travel path; all targets valid in the actual shipped profile; no frozen-world hitch inside crest | locked contract; `BuildProfileTravelAuditRules`; `CrashProofingTests`; `FirstHourTravelSignalTests` |
| Repeatable world improvement | 🟡 framework v1 candidate (Round 2 recipes for W000, ToxicCity and generated worlds; exact CI pending) | Every declared round is deterministic, versioned, budgeted, aspect-covered, recipe-current and evidenced; exact recipes override defaults | `WorldImprovementCompiler`; `WorldImprovementAuditRules`; manifest/compiler/recipe tests; `WORLD_IMPROVEMENT_FRAMEWORK.md` |

## 2 · THE LIVING WORLD

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Creatures (bodies) | 🧱→💎 in flight (Forge genomes, roster complete) | Hero creatures use their class budget; no primitive stand-ins; tells hold | `ForgeAuditRules` + class budgets; 🕳️ budget-utilization floor |
| Creature behavior | 🧱 structural v1 (7 shipped ids; ≥3 active states each; device readability/polish pending) | Every shipped species has readable active vocabulary, a fair telegraph, a counter and separate non-lethal resolution; actual Quest timing/readability still matters | `CreatureBehaviorReadabilityCatalog` + both readability/audit test suites + `CreatureBehaviorBuildGate`; headset quality check remains |
| Gardens | 🧱 structural v1 (24 authored specs, genetics/watering; most seed surfacing and starter-asset reconciliation remain) | Genetics visibly matter; interactions are hand verbs; every authored species is deliberately reachable; giants/breeding playable | garden tests + **`CatalogBreadthAuditRules`/tests/APK gate**; warnings name unsurfaced ids and create-only asset drift |
| Automation/belts | 💎 | Place/ride/persist/feed/clone; deterministic flow; machinery motion follows work | `AutomationAuditRules` + tests |
| Ambient audio | 🧱 v1 (procedural biome beds) | Every biome has bed + stingers; VO ducking | `AmbienceTests`; remaining stingers/stems/ducking |

## 3 · THE PLAYER

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Locomotion & comfort | 🧱 v1.1 (move/snap/vignette; Cozy/Standard/Bold and console code-green; device pending) | Player-visible presets; every artificial motion reports/suspends correctly and never parents rig | locked contract; traversal tests; `ComfortSettingsTests` + `ComfortCoverageTests` |
| Hands & interaction | 💎 code / device calibration ongoing | Everything interactive answers within reach; collider before interactable | `InteractionReachAuditRules`; `VR_RIG_GOTCHAS.md`; wiring tests |
| Weapons & combat feel | 🧱→💎 in flight (Round 1 recoil/haptics/audio/impact; device pending) | Every weapon distinct in hand, cadence, recoil, sound and tactile response; actual bounds and held axis valid | combat tests; `WeaponPerceptualAuditRules`; headset feel ledger |
| Abilities/augments | 🧱 v1 (6 live) | Full set; visible state; bot/human symmetry where applicable | augment tests + WiringValidator |
| Player progression/saves | 💎 (atomic profile + backups, overlays, one economy) | Nothing earned/built lost on quit or interrupted save | serializer/round-trip/crash recovery/economy gates |
| UI/UX & menus | 🧱 v1 code (Home Hub, boards, comfort, diegetic surfaces; bake/device pending) | Diegetic-first; readable at arm's length; usable target faces; consistent save presentation | `HomeHubFlowTests`; **`UiReadabilityAuditRules` + tests/build WARN processor**; device calibration pending; richer save slots remain |

## 4 · THE SHIP & VEHICLES

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Ship customization | 💎 (chassis/modules/refit/liveries/decals/hums; hero hull Round 1) | Loadout visibly changes a production-quality ship without breaking boarding/refit anchors | ShipLoadout/Locker tests; `FullSendPresentationAuditRules` |
| Flight | 💎 v1.3 | Cockpit reference; full arcade vocabulary; never parent rig | flight tests + comfort law |
| Space combat | 🧱 v1 (stun, disable, salvage) | Non-lethal; varied enemy ships; economy payout | combat/economy tests; 🕳️ enemy variety |
| Vehicles | 🧱→💎 in flight (three distinct ToxicCity rides, edge/dismount safety; 3/6 archetypes, no garage) | Shares Forge/comfort/mount patterns; every authored ride reaches a world; distinct missing families and a usable garage/catalog surface complete the vocabulary | vehicle tests + `FullSendPresentationAuditRules` + **`CatalogBreadthAuditRules`/tests/APK gate** |

## 5 · STORY & CHARACTERS

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Narrative spine | 💎 | Movie-tight setup/payoff; four distinct endings | canon/continuity tests |
| Character voices | 💎 (RILL/Cal/Mara/Sable/Nine) | No faction mouthpieces; identifiable without names | voice guide + line tests |
| Story delivery in-game | 🦴 (built-world beats) | Every shipped world carries jobs/lines/choices; endings wired | story-beat coverage baseline; 🕳️ `WorldContentGenome`, per-world line-kit and flag-graph gates |
| VO & subtitles | 🦴 (subtitle system, no VO) | Full voice pipeline; readable subtitles | subtitle tests; VO unbuilt |

## 6 · MULTIPLAYER & META-GAME

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| PvP arena | 🧱 v1 | Full MP100 ladder; bot/human symmetry | PvP tests + MP100 board |
| Tidefront conquest | 💎 | Deterministic complete war, missions affect outcomes | sim/catalog/save tests |
| Online sync | 🦴 (Photon presence) | Host-authoritative combat/actions; two-headset proof | transport tests; hardware gate |
| Economy | 💎 | Every payout through RewardRouter | `EconomyAuditRules` |

## 7 · ENGINE, PIPELINE & QUALITY

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Performance budgets | 🧱 | Every new content type gets cap + audit | `PerfBudgetAuditRules`; per-module manifest budgets |
| Art pipeline (Forge) | 💎 | All shipped look traced to Forge/registry; hero photo loop | Forge/audit/library/wiring tests |
| Wiring integrity | 💎 | Producer + consumer + verifier + map row | `WiringValidatorTests` |
| Save integrity | 💎 | Overlay idiom; neutral old-save defaults | per-system round trips |
| CI & verification | 💎 | Green per push; red stops code; 3-red breaker; recipe-only rounds trigger route and APK proof | operating laws + durable verdict + synchronized recovery workflow paths |
| Generated-content currentness | 🟡 framework output protected; legacy authors still open | What ships is a pure function of current recipe + compiler; no stale committed asset can mask intent | `WorldImprovementHashCore` + `WorldImprovementAuditRules`; 🕳️ general `ASSET_STALE_VS_RECIPE` for old `*Author/*Library` set |
| Runtime health | 🧱 v1 (vitals, census, janitor, async travel) | 1%-low ≥60; memory flat over travel soak; resources clean | health/resource tests + device soak |
| Localization readiness | ⬜ decision needed | One text seam or explicitly English-only launch | 🕳️ Terry decision before M5 scale |
| Docs & blackboard | 🧱→💎 in flight | Current checklist routes to detailed/history boards; every round emits machine-readable expected-output evidence | staleness/session-zero gates + `world_improvement_compile.json` + round runbook |
| Onboarding/tutorial | 🧱 in flight (most adapters + W000 surfaces code-green; A01/S05/S08 remain; **review 2026-07-11: FH-GAP-1..4 added to `first_hour/ADAPTER_ENVELOPES.md` — veteran skip · runbook mirroring · gate promotion · map linkage**) | Cold player learns W000→W001 through moments; hesitation-only hints; veterans never nagged | first-hour contracts/tests + `first_hour_gate.py` in CI (report-only → BLOCKING at S08 close) |
| Accessibility | 🦴→🧱 in flight | Presets + subtitle size + seated/handedness/haptic scale + color-safe palettes | preset tests; remaining non-preset implementation rows |
| Haptics | 🦴→🧱 in flight (weapon feel signatures now live) | Every hand/body verb has a distinct, bounded, correct-hand signature with no duplicate XRI pulse | `docs/design/HAPTIC_COVERAGE.md`; weapon feel tests; broader runtime/device registry remains open |

---

## 8 · SHIP & STORE

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Meta Store readiness | 🦴 (checklist; nothing submitted) | Every store box checked; dry-run clean | store checklist; Terry paperwork |
| Entitlement + Platform SDK | ⬜ | Entitlement check at boot | 🕳️ requires App ID |
| Release build hygiene | ⬜ | Keystore backed up; DevMenu/diagnostics off; minimal permissions | 🕳️ build-flag audit |
| Onboarding/tutorial | 🧱 in flight | Cold player completes W000–W001 unaided; return payoff persists | first-hour gates; A01→S05→S08 + device evidence |
| Comfort rating | 🧱 code | Presets honestly support rating | literal tests + headset check |
| Store assets | ⬜ | Correct icon/trailer/screens | Picasso after visuals land |

## THE GATE-COVERAGE MATRIX

This is the generated-thinking layer Architect requested: aspects with zero coverage are where the next
repeat failure will escape. A dimension is not “closed” merely because one gate exists; it is closed when
its relevant rows have machine evidence and the human-only remainder is explicit.

| Quality dimension | Current gate coverage | Remaining gap |
|---|---|---|
| Logical correctness | EditMode suites, schema validators, wiring/economy/save contracts | `WorldContentGenome`, Lore Forge and flag-graph validators |
| Spatial correctness | `WorldContainmentAuditRules`, `RouteContinuityAuditRules`, `InteractionReachAuditRules`, spawn/travel checks | broaden route graph beyond named anchors; deliberate high-interactable waiver schema |
| Perceptual correctness | `WeaponPerceptualAuditRules`, `PerceptualCoverageAuditRules`, city/ship/fleet presentation audits | full PG-5 mannequin/grid and arrival-view contact-sheet automation + mandatory review stamp |
| Performance | `PerfBudgetAuditRules`, Forge budgets, per-module object budgets | promote calibrated over-cap warnings to blockers; device thermal/1%-low baseline |
| Build-profile truth | `BuildProfileTravelAuditRules`, locked Golden profile verification | generalize profile-aware checks beyond travel to jobs/packs/flags |
| Recipe currentness | World Improvement hash/stamp/module version audit | apply ensure-current/hash law to legacy authors and bot-commit generated assets |
| Device correctness | Recovery PlayMode, exact-SHA Golden APK, log/route ledger | PG-6 device agent bus and automated screenshot/route inspection |
| Feel / fun / comfort | Terry headset verdict | intentionally human; framework ensures his time is spent here rather than finding machine-detectable errors |

## THE GATE-GAP QUEUE

1. **PG-5 referenced contact sheets + mandatory review stamp** — mechanical visibility exists; full mannequin/grid/arrival-view workflow remains.
2. **Legacy recipe-hash law** — apply ensure-current + `ASSET_STALE_VS_RECIPE` to create-only authors/libraries.
3. **Flag-graph validator** — cheapest assembly-scale story insurance.
4. **WorldContentGenome** — scalable jobs/encounters/rewards purpose layer.
5. **Lore Forge + per-world line kits** — slots and continuity at 80-world scale.
6. **Ride-scenes** — comfort-legal cinematic moments through consented conveyances.
7. **Budget-utilization floor** — Picasso coordination.
8. ~~Skyscape signature rubric~~ — CLOSED 2026-07-10.
9. ~~Story-beat coverage baseline~~ — CLOSED 2026-07-10.
10. ~~Interior audit~~ — CLOSED 2026-07-10.
11. ~~Board-staleness flag~~ — CLOSED 2026-07-10.
12. ~~UI readability/reach audit~~ — **CLOSED at structural maturity**; headset calibration remains.
13. ~~Haptic coverage checklist~~ — **CLOSED at documentation level**; runtime/device rows remain.
14. ~~Accessibility design doc~~ — CLOSED 2026-07-10; non-preset controls remain implementation rows.
15. ~~Creature behavior-count/readability check~~ — **CLOSED at structural code/CI maturity**; headset readability remains.
16. ~~Plant/vehicle catalog breadth audit~~ — **CLOSED at structural audit maturity**; content breadth remains.

## THE UNIFORMITY REVIEW

When a new model takes over—or monthly—walk this map top to bottom and honestly re-mark every state.
Choose the next three rows per lane to raise the floor, not merely polish the strongest system. For a
world-improvement round, use `WORLD_IMPROVEMENT_ROUND_RUNBOOK.md` and select the lowest linked aspects
whose dependencies are ready.
