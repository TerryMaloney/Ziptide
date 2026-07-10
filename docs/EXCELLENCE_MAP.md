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
  `OPERATOR_START_HERE.md`) requires the GUARDRAIL to exist or a 🕳️ row to be claimed.
- At chunk close: if you changed an aspect's STATE, update its row. This file is a dashboard, not
  an archive — stale rows are bugs.
- **The unevenness rule:** if your lane's rows are all 💎 while a neighboring aspect you depend on
  is 🦴, the right next task is often THEIR row, not more polish on yours. Terry's exact fear is
  "some parts really good and other parts not that great."

Legend: gates live in `Ziptide/Assets/Ziptide/Editor/Audit/*AuditRules.cs` (BLOCK = CI-red) unless
noted; EditMode tests are gates too. *(Last full audit of this table: 2026-07-10.)*

---

## 1 · WORLDS & SPACE

| Aspect | State | The standard (checkable) | Guardrail |
|---|---|---|---|
| Terrain & biomes | 🧱 v1 (fBM+warp heightfields, biome matrix, 12 worlds) | Every world's ground is distinct at a glance (biome matrix row per world, no two identical seeds); walkable slopes gated; "No Man's Sky bar" = no repeated-feeling worlds | `WorldAuditRunner` + `WorldContentAuditRules`; terrain tests |
| POIs & world dressing | 🧱 v1 (12 POI verbs, scatter, paths) | ≥12 POI types stamped per the catalog; every POI reachable; streets/interiors carry prop kits, not emptiness | `WorldReachabilityAuditRules` (flood + traversal edges) |
| Buildings & city | 🧱 v1 (grammar/WFC, kits registering) | Biome→kit/palette mapping for ≥4 kits; no primitive-box fallbacks in shipped worlds (`KIT_FULFILLED`) | `BuildingAuditRules`; registry fallback logs |
| Interiors | 🦴 (RoomPartitioner core only) | Enterable buildings have partitioned, furnished, portal-culled interiors; windows read lit (interior mapping) | 🕳️ GAP — no interior audit yet (rooms reachable, furnished ≥N props, portal budget) |
| Vertical/caverns/traversal | 💎 (zip/climb/lift/pad/grapple + 2 cave worlds) | Every traversal verb usable in ≥1 shipped world; multi-level reachability proven | `WorldReachabilityAuditRules` (one-way edges) + 5 core test suites |
| Skyscape & atmosphere | 🧱 v1 (layers built, W005 signature, 11 worlds pending verdict) | THE PROSPECT BAR (`systems/SKYSCAPE_DESIGN.md` §5 rubric): something drifting, hazy horizon, occluded body, sky color reaches the ground; tiered (Signature/Standard/Interior) | `SkyVistaAuditRules` + SkyAtmosphere tests + **§5 Signature-rubric gate (`GateGap2_SignatureRubric…` — gap #2 CLOSED 2026-07-10)** |
| Travel & gates | 💎 (TravelCoordinator law, gated doors, THE ZIPTIDE; **pre-flight guard 2026-07-10** — a missing scene aborts BEFORE side effects instead of silently stranding) | One travel path, always; story-gated where flagged; every travel target in Build Settings | Locked contract in `CLAUDE.md`; `CrashProofingTests.EveryTravelTarget_IsInBuildSettings` gate; travel tests |

## 2 · THE LIVING WORLD

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Creatures (bodies) | 🧱→💎 in flight (Forge genomes, roster complete, smooth shading landing) | RICHNESS BAR: hero creatures USE their 10k class budget; every creature ≥Forge-genome quality (no primitive stand-ins in shipped worlds); WeakPoint/tell contracts hold | `ForgeAuditRules` + class budgets in `Validate()`; 🕳️ GAP — budget-UTILIZATION floor (warn <30% of class budget on Signature assets) |
| Creature behavior | 🦴 (~10 base behaviors) | Each species has a movement vocabulary (≥3 states: idle/alert/hunt or flee), reacts to player AND to hazards; ecology (nests/packs/territory) per `CREATURE_ECOLOGY.md` | Behavior tests exist per behavior; 🕳️ GAP — per-species behavior-count check |
| Gardens | 🧱 v1 (24 species, genetics, watering can) | Genetics affect visible outcomes; every interaction is a HAND verb (pour/tend/prune/harvest); giants/breeding playable | Garden core tests; 🕳️ GAP — plant-catalog breadth audit (all species reachable in shipped worlds) |
| Automation/belts | 💎 (place/ride/persist/feed/clone; LAW-6 richness pass 2026-07-10) | Belts buildable in any world with a pad; item flow deterministic; conductor mode; blueprint copy/stamp; machinery moves only while ore does | `AutomationAuditRules` (area caps, save identity) + 40 tests |
| Ambient audio | 🧱 v1 (2026-07-10: procedural biome beds — wind/hum/rumble loops + drip/chirp one-shots, 10 biomes, crossfade on travel) | Every biome has a bed (wind/insects/hum) + hazard stingers; sky tiers get matching air-tone; ducking under VO | `AmbienceTests` incl. **never-silent coverage gate over every shipped scene**; remaining: hazard stingers · music stems (5.5) · VO ducking (post-VO) |

## 3 · THE PLAYER

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Locomotion & comfort | 🧱 v1 (move/snap/vignette; suspension idioms per traversal verb) | Comfort presets (vignette/snap/smooth/seated) player-visible; EVERY new motion source suspends/restores stick-move and never parents the rig | Locked contract; per-verb tests; 🕳️ GAP — comfort-preset UI unbuilt (0.4) |
| Hands & interaction | 💎 (grab/holster/belt/tools; collider-first law) | Everything interactive answers to hands within 1.6m reach idioms; collider BEFORE interactable everywhere | `VR_RIG_GOTCHAS.md` law; wiring tests |
| Weapons & combat feel | 🧱 v1 (guns/melee/taser unified onto one damage scale) | ADS/reload/haptics per `ABILITIES_AND_ARSENAL.md`; every weapon distinct in hand (cadence/recoil/sound), not stat-only | Combat core tests + damage-scale migration guard; 🕳️ GAP — weapon-feel checklist rows (4.4) |
| Abilities/augments | 🧱 v1 (6 augments live) | Full A4.5–A4.7 set incl. dual-wield when charge economy exists; every augment has visible state (orb brightness idiom) | Augment tests + WiringValidator (author build-hook) |
| Player progression/saves | 💎 (profile, autosave, ledger economy, conquest/belt overlays; **ATOMIC writes + .bak recovery 2026-07-10** — a mid-write battery death can no longer wipe progress) | NOTHING the player builds/earns is lost on quit OR on a crash mid-save — every new system ships WITH its save story (the overlay idiom) | `ProfileSerializer` tests + save round-trips + `CrashProofingTests` corrupt-and-recover gate; `EconomyAuditRules` |
| UI/UX & menus | 🦴 (dev menu, boards, diegetic surfaces) | Diegetic-first (ship hub law); cold-boot title + save slots; readable at arm's length (characterSize×fontSize lesson) | 🕳️ GAP — no UI readability/reach audit; HOME_HUB rows open (2.5) |

## 4 · THE SHIP & VEHICLES

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Ship customization | 💎 (6 chassis, 10 modules, refit, liveries, journey decals, hums) | Loadout visibly changes the SHIP (silhouette/sound) and the FLIGHT (FlightModel feed — landed rb v1.3) | ShipLoadout/Locker tests; wrap-invariance by construction |
| Flight | 💎 v1.3 (6DOF, comfort, Xbox parity, loadout feed) | Fortnite-smooth bar; cockpit reference frame; reverse/boost/roll vocabulary; never parent the rig | Flight core tests; comfort law |
| Space combat | 🧱 v1 (stun bolts, disable+salvage) | Non-lethal law absolute (nothing explodes); enemy VARIETY (bot profiles in 3D); salvage pays through the one economy | Space combat tests; `EconomyAuditRules`; 🕳️ GAP — enemy-ship variety row (3.1 continues) |
| Vehicles | 🧱 v1 (3 rides, mount/drive/dismount) | Ground sibling of the ship: shares Forge, wraps, comfort, seat/mount; a garage surface | Vehicle tests (rb 3.2a); 🕳️ GAP — vehicle catalog breadth + garage |

## 5 · STORY & CHARACTERS

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Narrative spine | 💎 (bible locked, 12 chapters tightened, continuity audited) | Movie-tight: every beat set up + paid off; subtext rule (surface topic ≠ real thing said); the 4 endings distinct | Continuity-audit appendix; canon tests (`SkyVistaTests` pin canon arcs; RILL line tests) |
| Character voices | 💎 (RILL/Cal/Mara/Sable/Nine voiced, speaker field) | No faction mouthpieces; every named character passes the "could you tell who said it with the name removed" test | Voice-guide in STORY_BIBLE §3b; `VOICE_PIPELINE.md`; line tests |
| Story delivery in-game | 🦴 (flag lines + gate lines wired; beats for built worlds only) | Every shipped world carries its storyboard beats (jobs, RILL lines, choice stations); endings wired | 🕳️ GAP — per-world story-beat coverage audit (board 5.4) |
| VO & subtitles | 🦴 (subtitle system solid; no VO) | VO pipeline per `VOICE_PIPELINE.md`; subtitles readable law | Subtitle tests; VO unbuilt (5.5) |

## 6 · MULTIPLAYER & META-GAME

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| PvP arena | 🧱 v1 (1v1, bots, melee pack, locator) | Full MP100 ladder items; bot/human symmetry (bots use what players use) | PvP rules tests; MP100 board |
| Tidefront conquest | 💎 (sim→table→5 mission verbs→save→fog→rack→hotseat) | Deterministic resolve law (seed = replay); full catalog surfaced; every remaining seam named on boards | 18+ sim tests + mission catalog-span test + save round-trips |
| Online sync | 🦴 (Photon presence seam; A6 parked on hardware) | Actions-only sync over the deterministic resolver; host-authoritative combat | Transport seam tests; A6 gate = two headsets |
| Economy (one economy) | 💎 (ledger, RewardRouter, flow reports) | EVERY payout routes through RewardRouter — no side-channel grants, ever | `EconomyAuditRules` (BLOCK on side-channel) |

## 7 · ENGINE, PIPELINE & QUALITY (the meta of the meta)

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Performance budgets | 🧱 (budgets + caps for lights/renderers/tris per class) | Every NEW content type gets a cap + audit rule the same commit it's introduced (law) | `PerfBudgetAuditRules`; 0.6 standing rule |
| Art pipeline (Forge) | 💎 (ops, modifiers, budgets, photo loop, tell bridge) | All shipped look comes through Forge/registry ids (no ad-hoc meshes); photo-critique loop for hero assets | `ForgeAuditRules` + library tests + WiringValidator |
| Wiring integrity | 💎 | Every author/ensure system is build-hooked (both sides or CI-red) | `WiringValidatorTests` (the gate that caught AugmentAuthor) |
| Save integrity | 💎 | Overlay idiom everywhere; neutral default = pre-feature behavior exactly; unknown records skip | Per-system round-trip tests (belt/conquest/profile) |
| CI & verification | 💎 (compile+tests+audits per push; circuit breaker) | CI green per push; red = warn Terry loudly + stop C#; 3 reds = stop | THE LAWS 4–5 |
| Runtime health (frames & memory) | 🧱 v1 (2026-07-10: FrameStats vitals + memory census + THE TRAVEL JANITOR — the game finally sweeps its own orphans) | 1%-low ≥ 60fps in every world (`HEALTH_SLOW` flags misses); memory census FLAT across a 10-travel soak; every runtime resource creator destroys what it makes or sits on the visible ledger | `ResourceDisciplineTests` leak ratchet (ledger can only shrink) + `HEALTH/HEALTH_SWEEP` logcat contract + runbook soak test |
| Localization readiness | ⬜ **FORGOTTEN — decision needed** | Every player-facing string reaches the eye through ONE seam (a string table / text provider), so a language can be added without touching 200 files. Today ALL text is hardcoded literals — cheap to fix per-file now, brutal to retrofit at 80 worlds | 🕳️ GAP — Terry decides: English-only launch (document it, close the row) or adopt a TextTable seam before M5 scales content. No code moves until he calls it |
| Docs & blackboard | 🧱 | Boards current at chunk close; HANDOFF entry per session; runbook is THE single Terry list; this map current | Session-zero test (LAWS); 🕳️ GAP — no staleness check (a board row marked 🟡 >7 days = flag) |
| Onboarding/tutorial | ⬜ | A new player learns move/grab/holster/travel inside W000–W001 without a menu; every verb taught by a moment, not text | 🕳️ GAP — no tutorial design doc yet — needs authoring before M8 |
| Accessibility | ⬜ | Comfort presets + subtitle size + one-handed mode decision + colorblind-safe palettes (the war table is color-coded!) | 🕳️ GAP — no doc, no audit. Cheap early, expensive late |
| Haptics | 🦴 (belt clicks, scattered) | Every hand verb has a haptic signature (grab/fire/climb-grip/zip/belt-lip) | 🕳️ GAP — no haptic coverage checklist |

---

## 8 · SHIP & STORE (the last mile — see `META_STORE_READINESS.md` for the full checklist)

| Aspect | State | The standard | Guardrail |
|---|---|---|---|
| Meta Store readiness | 🦴 (checklist doc'd 2026-07-10; nothing submitted) | Every box in `META_STORE_READINESS.md` checked; App Lab VRC pre-scan clean | 🕳️ GAP — the doc IS the gate for now; §2 paperwork is Terry's, startable NOW |
| Entitlement + Platform SDK | ⬜ | Entitlement check in first seconds of boot (Meta rejects without it) | 🕳️ GAP — needs App ID from dashboard first |
| Release build hygiene | ⬜ | Release keystore (owned + backed up); DevMenu/diagnostics build-flagged OFF; minimal manifest permissions | 🕳️ GAP — build-flag audit is board-row sized |
| Onboarding/tutorial | ⬜ | A cold player learns move/grab/holster/travel in W000–W001, taught by moments not text | 🕳️ GAP — needs the design doc first (Fable-priority: it shapes W000) |
| Comfort rating | 🦴 (mechanics comfortable-by-design; no presets UI) | Presets ship + default sensibly → honest "Moderate" rating | 0.4 row; store-facing, not optional |
| Store assets (icon/trailer/screens) | ⬜ | Meta's exact sizes; in-headset captures | Picasso's lane when visuals land |

## THE GATE-GAP QUEUE (claimable, in rough value order)
Each 🕳️ above, as one board-row-sized task. Claiming one = add the audit/checklist + a HANDOFF note.
1. **Budget-utilization floor** (Forge `Validate()` WARN when a Signature-tier asset uses <30% of its
   class budget) — the direct enforcement of Terry's "10k budget, built with 1k" complaint. *(Picasso's
   file — coordinate.)*
2. ~~Skyscape §5-rubric audit~~ — **CLOSED 2026-07-10** (Signature tier gated in `SkyAtmosphereTests`; extend to Standard tier when the rollout lands).
3. ~~Story-beat coverage~~ — **CLOSED 2026-07-10** (`GateGap3_EveryStoryWorld_CarriesAuthoredBeats`; ToxicCity documented as legacy-builder-covered; extend to RILL-line coverage when the line registry grows a per-world query).
4. **Interior audit** — rooms reachable + furnished floor (≥N registry props per room).
5. ~~Board-staleness flag~~ — **CLOSED 2026-07-10** (`GateGap5_NoBoardClaim_RotsSilently`: dated 🟡 claims older than 14 days fail CI — finish, re-date, or release the row).
6. **UI readability audit** — TextMesh sizing law + reach distances on interactive tiles.
7. **Haptic coverage checklist** — doc-level first; audit when a haptic registry exists.
8. **Accessibility doc** — decisions Terry must make once, cheaply, now (doc, then rules).
9. **Behavior-count check** — every shipped creature id maps to ≥3 behavior states.
10. **Plant/vehicle catalog breadth** — same shape as the mission catalog-span test that already exists.

## THE UNIFORMITY REVIEW (standing, once per operator "era")
When a new model takes over (or monthly): walk this map top to bottom and re-mark every STATE
honestly. The output is not a report — it's the next 3 board rows per lane, picked to RAISE THE
FLOOR (the worst rows), not the ceiling. Excellence here means *even*, then *high*.
