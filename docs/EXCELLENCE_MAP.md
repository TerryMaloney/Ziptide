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
noted; EditMode tests are gates too. *(Last status reconciliation: 2026-07-11.)*

---

## 1 · WORLDS & SPACE

| Aspect | State | The standard (checkable) | Guardrail |
|---|---|---|---|
| Terrain & biomes | 🧱 v1 (fBM+warp heightfields, biome matrix, 12 worlds) | Every world's ground is distinct at a glance; walkable slopes gated; no repeated-feeling worlds | `WorldAuditRunner` + `WorldContentAuditRules`; terrain tests |
| POIs & world dressing | 🧱 v1 (POI verbs, scatter, paths) | Every POI reachable; streets/interiors carry prop kits, not emptiness | `WorldReachabilityAuditRules` |
| Buildings & city | 🧱 v1 (grammar/WFC, kits registering) | Biome→kit/palette mapping; no primitive-box fallbacks in shipped worlds | `BuildingAuditRules`; registry fallback logs |
| Interiors | 🧱 v1 (partitioned/furnished/per-room portal cull; W002 re-bake pending) | Enterable buildings have connected, furnished, portal-culled interiors | `InteriorAuditRules` + `InteriorFurnishCoreTests` |
| Vertical/caverns/traversal | 💎 (zip/climb/lift/pad/grapple + cave worlds) | Every traversal verb usable; multi-level reachability proven | reachability one-way edges + traversal test suites |
| Skyscape & atmosphere | 🧱 v1 (canonical layers, signature worlds pending device verdict) | Prospect bar: drift, hazy horizon, occluded body, sky color reaches ground | `SkyVistaAuditRules` + signature-rubric tests |
| Travel & gates | 💎 code (one `TravelCoordinator`, gated doors, THE ZIPTIDE, pre-flight, async load behind crest; device frame check pending) | One travel path; all targets valid; no frozen-world hitch inside crest | locked contract; `CrashProofingTests`; `FirstHourTravelSignalTests` |

## 2 · THE LIVING WORLD

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Creatures (bodies) | 🧱→💎 in flight (Forge genomes, roster complete) | Hero creatures use their class budget; no primitive stand-ins; tells hold | `ForgeAuditRules` + class budgets; 🕳️ budget-utilization floor |
| Creature behavior | 🦴 (~10 base behaviors) | Each shipped species has ≥3 readable states and reacts to player/hazards | behavior tests; 🕳️ per-species behavior-count check |
| Gardens | 🧱 v1 (24 species, genetics, watering) | Genetics visibly matter; interactions are hand verbs; giants/breeding playable | garden tests; 🕳️ plant-catalog breadth audit |
| Automation/belts | 💎 | Place/ride/persist/feed/clone; deterministic flow; machinery motion follows work | `AutomationAuditRules` + tests |
| Ambient audio | 🧱 v1 (procedural biome beds) | Every biome has bed + stingers; VO ducking | `AmbienceTests`; remaining stingers/stems/ducking |

## 3 · THE PLAYER

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Locomotion & comfort | 🧱 v1.1 (move/snap/vignette; Cozy/Standard/Bold and console code-green; device pending) | Player-visible presets; every artificial motion reports/suspends correctly and never parents rig | locked contract; traversal tests; `ComfortSettingsTests` + `ComfortCoverageTests` |
| Hands & interaction | 💎 (grab/holster/belt/tools; collider-first) | Everything interactive answers within reach; collider before interactable | `VR_RIG_GOTCHAS.md`; wiring tests |
| Weapons & combat feel | 🧱 v1 (unified non-lethal scale) | Every weapon distinct in hand, cadence, recoil, sound and tactile response | combat tests; 🕳️ weapon-feel/device rows |
| Abilities/augments | 🧱 v1 (6 live) | Full set; visible state; bot/human symmetry where applicable | augment tests + WiringValidator |
| Player progression/saves | 💎 (atomic profile + backups, overlays, one economy) | Nothing earned/built lost on quit or interrupted save | serializer/round-trip/crash recovery/economy gates |
| UI/UX & menus | 🧱 v1 code (Home Hub, boards, comfort, diegetic surfaces; bake/device pending) | Diegetic-first; readable at arm's length; usable target faces; consistent save presentation | `HomeHubFlowTests`; **`UiReadabilityAuditRules` + tests/build WARN processor**; device calibration pending; richer save slots remain |

## 4 · THE SHIP & VEHICLES

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Ship customization | 💎 (chassis/modules/refit/liveries/decals/hums) | Loadout visibly changes ship and flight | ShipLoadout/Locker tests |
| Flight | 💎 v1.3 | Cockpit reference; full arcade vocabulary; never parent rig | flight tests + comfort law |
| Space combat | 🧱 v1 (stun, disable, salvage) | Non-lethal; varied enemy ships; economy payout | combat/economy tests; 🕳️ enemy variety |
| Vehicles | 🧱 v1 (3 rides) | Shares Forge/comfort/mount patterns; garage surface | vehicle tests; 🕳️ catalog breadth + garage |

## 5 · STORY & CHARACTERS

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Narrative spine | 💎 | Movie-tight setup/payoff; four distinct endings | canon/continuity tests |
| Character voices | 💎 (RILL/Cal/Mara/Sable/Nine) | No faction mouthpieces; identifiable without names | voice guide + line tests |
| Story delivery in-game | 🦴 (built-world beats) | Every shipped world carries jobs/lines/choices; endings wired | story-beat coverage baseline; extend with per-world line query |
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
| Performance budgets | 🧱 | Every new content type gets cap + audit | `PerfBudgetAuditRules` |
| Art pipeline (Forge) | 💎 | All shipped look traced to Forge/registry; hero photo loop | Forge/audit/library/wiring tests |
| Wiring integrity | 💎 | Producer + consumer + verifier + map row | `WiringValidatorTests` |
| Save integrity | 💎 | Overlay idiom; neutral old-save defaults | per-system round trips |
| CI & verification | 💎 | Green per push; red stops code; 3-red breaker | operating laws + durable verdict |
| Runtime health | 🧱 v1 (vitals, census, janitor, async travel) | 1%-low ≥60; memory flat over travel soak; resources clean | health/resource tests + device soak |
| Localization readiness | ⬜ decision needed | One text seam or explicitly English-only launch | 🕳️ Terry decision before M5 scale |
| Docs & blackboard | 🧱 v1.1 | Current checklist routes to detailed/history boards | staleness/session-zero gates |
| Onboarding/tutorial | 🧱 in flight (most adapters + W000 surfaces code-green; A01/S05/S08 remain) | Cold player learns W000→W001 through moments; hesitation-only hints | first-hour contracts/tests + bake/device gate |
| Accessibility | 🦴→🧱 in flight | Presets + subtitle size + seated/handedness/haptic scale + color-safe palettes | preset tests; remaining non-preset implementation rows |
| Haptics | 🦴 inventory complete (scanner explicit; inspected first-hour owners mapped) | Every hand/body verb has a distinct, bounded, correct-hand signature with no duplicate XRI pulse | **`docs/design/HAPTIC_COVERAGE.md` closes the doc-level gap**; runtime/source/device rows remain; no registry exists |

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

## THE GATE-GAP QUEUE

1. **Budget-utilization floor** — Picasso coordination.
2. ~~Skyscape signature rubric~~ — CLOSED 2026-07-10.
3. ~~Story-beat coverage baseline~~ — CLOSED 2026-07-10.
4. ~~Interior audit~~ — CLOSED 2026-07-10.
5. ~~Board-staleness flag~~ — CLOSED 2026-07-10.
6. ~~UI readability/reach audit~~ — **CLOSED at WARN-only maturity 2026-07-11** (`UiReadabilityAuditRules` + tests/build processor); generated-scene/device calibration precedes any blocker promotion.
7. ~~Haptic coverage checklist~~ — **CLOSED at documentation level 2026-07-11** (`design/HAPTIC_COVERAGE.md`); runtime owner rows remain individually open.
8. ~~Accessibility design doc~~ — CLOSED 2026-07-10; non-preset controls remain implementation rows.
9. **Behavior-count check** — every shipped creature id maps to ≥3 behavior states.
10. **Plant/vehicle catalog breadth** — same shape as existing catalog-span tests.

## THE UNIFORMITY REVIEW

When a new model takes over—or monthly—walk this map top to bottom and honestly re-mark every state.
Choose the next three rows per lane to raise the floor, not merely polish the strongest system.
